using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using Microsoft.Ajax.Utilities;
using OfficeOpenXml;
using ISS_DAL.Model;
using ISS_DAL.ViewModels;
using FineUploader;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Globalization;

namespace AppsPortal.Areas.ISS.Controllers
{
    public class ItemPipelineController : ISSBaseController
    {
        // GET: ISS/ItemPipeline
        
        [Route("ISS/ItemPipeline/")]
        public ActionResult ItemPipelineIndex()
        {
            if (!CMS.HasAction(Permissions.StockItemDistribution.Access, Apps.ISS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var lastUpload = DbISS.dataItemPipelineUpload.Where(x => x.IsLastUpload == true).FirstOrDefault();
            ViewBag.LastUploadDate = lastUpload.UploadDate.Value.ToShortDateString();
            return View("~/Areas/ISS/Views/ItemPipeline/Index.cshtml");
        }

        [Route("ISS/ISSItemPipelineDataTable/")]
        public JsonResult ISSItemPipelineDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ItemPipelineDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ItemPipelineDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.StockItemDistribution.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
            var All = (

                from a in DbISS.dataItemPipeline.Where(x => x.Active && x.dataItemPipelineUpload.IsLastUpload == true).AsExpandable()
                join b in DbISS.codeISSItemLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.DeletedOn) && x.LanguageID == LAN) on a.ItemGUID equals b.ItemGUID   into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbISS.codeISSStockLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.DeletedOn) && x.LanguageID == LAN) on a.StockGUID equals c.StockGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()

                select new ItemPipelineDataTableModel
                {
                    ItemPipelineGUID = a.ItemPipelineGUID,
                    Active = a.Active,
                    ItemPipelineUploadGUID = a.ItemPipelineUploadGUID,
                    ItemGUID = a.ItemGUID.ToString(),
                    StockGUID=a.StockGUID.ToString(),
                    StockName=R2.StockDescription,
                    ItemName = R1.ItemDescription,
                    ETA=a.ETA,
                    Quantity = a.Quantity,

                    dataItemPipelineRowVersion = a.dataItemPipelineRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ItemPipelineDataTableModel> Result = Mapper.Map<List<ItemPipelineDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        #region Upload 
        [HttpGet]
        public ActionResult UploadFiles()
        {
            return PartialView("~/Areas/ISS/Views/ItemPipeline/_BulkItemPipelineUpload.cshtml",
                new dataItemPipeline());
        }
        [HttpPost]
        public FineUploaderResult UploadFiles(FineUpload upload)
        {

            return new FineUploaderResult(true, new { path = Upload(upload), success = true });
        }

        public string Upload(FineUpload upload)
        {
            var _stearm = upload.InputStream;
            DateTime ExecutionTime = DateTime.Now;
            string _ext = Path.GetExtension(upload.FileName).Split('.')[1];
            dataItemPipeline itemInput = new dataItemPipeline();
            itemInput.ItemPipelineGUID = Guid.NewGuid();
            string FilePath = Server.MapPath("~/Uploads/ISS/temp/" + itemInput.ItemPipelineGUID + ".xlsx");
            //Server.MapPath("~/Areas/WMS/temp/Standard_Operating_Procedure_for_issuing_ICT_equipment_to_staff" + DateTime.Now.ToBinary() + ".pdf");

            using (var fileStream = System.IO.File.Create(FilePath))
            {
                upload.InputStream.Seek(0, SeekOrigin.Begin);
                upload.InputStream.CopyTo(fileStream);
            }
            if (_ext.ToLower() == "xls" || _ext.ToLower() == "xlsx")
            {
                using (ExcelPackage package = new ExcelPackage(new FileInfo(FilePath)))
                {
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                    List<dataItemPipeline> itemStocks = new List<dataItemPipeline>();
                    bool ok = Validate(workSheet);
                    package.Save();
                    if (ok)
                    {
                        int totalRows = workSheet.Dimension.End.Row;
                        List<dataItemPipeline> inputBalances = new List<dataItemPipeline>();
                        var items = DbISS.codeISSItemLanguage.Where(x => x.LanguageID == LAN).ToList();
                        var warehouses = DbISS.codeISSStockLanguage.Where(x => x.LanguageID == LAN).ToList();
                        var uploads = DbISS.dataItemPipelineUpload.ToList();

                        
                        foreach (var item in uploads)
                        {
                            item.IsLastUpload = false;
                        }
                        dataItemPipelineUpload stockUpload = new dataItemPipelineUpload
                        {
                            ItemPipelineUploadGUID = Guid.NewGuid(),
                            UploadDate = ExecutionTime,
                            IsLastUpload = true,
                            CreateDate = ExecutionTime,
                            CreatedByGUID = UserGUID,
                            Active = true
                        };
                        List<dataItemPipeline> stockAlls = new List<dataItemPipeline>();
                        Guid stockAllGUid = Guid.Parse("972cbdc5-6444-49da-b71f-af1375df1ea4");
                        for (int i = 2; i <= totalRows; i++)
                        {
                            dataItemStockBalance ToAddInput = new dataItemStockBalance();
                            var StockName = workSheet.Cells["A" + i].Value;
                            if (StockName == null)
                                break;
                            var ItemName = workSheet.Cells["B" + i].Value;
                            //var TotalItem = workSheet.Cells["C" + i].Value;
                            var CheckItem = items.Where(x => x.ItemDescription.ToString().ToLower() == ItemName.ToString().ToLower()).FirstOrDefault();
                            var CheckStock = warehouses.Where(x => x.StockDescription.ToString().ToLower() == StockName.ToString().ToLower()).FirstOrDefault();
                            int? TotalItem = Convert.ToInt32(workSheet.Cells["C" + i].Value);
                            var checkExist = inputBalances.Where(x => x.ItemGUID == CheckItem.ItemGUID && x.StockGUID == CheckStock.StockGUID).FirstOrDefault();
                            var ETA = workSheet.Cells["D" + i].Value;
                            string[] namesArray ;
                            string fullDate = null;
                            if (ETA != null) {
                                namesArray = ETA.ToString().Split('/');
                                string currentDay = String.Empty;
                                string currentMonth = String.Empty;
                                string currentYear = String.Empty;

                                for (int j = 0; j < namesArray.Length; j++)
                                {
                                    if (j == 0)
                                    {
                                        if (namesArray[j].Length == 1)
                                        {
                                            string day = "0" + namesArray[j];
                                            currentDay = day;
                                        }
                                        else
                                        {
                                            currentDay = namesArray[j];
                                        }

                                    }
                                    if (j == 1)
                                    {
                                        if (namesArray[j].Length == 1)
                                        {
                                            string month = "0" + namesArray[j];
                                            currentMonth = month;
                                        }
                                        else
                                        {
                                            currentMonth = namesArray[j];
                                        }


                                    }
                                    if (j == 2)
                                    {
                                        currentYear = namesArray[j];
                                    }

                                }

                                 fullDate = currentMonth + "/" + currentDay + "/" + currentYear;
                            }
             



                     
                            DateTime? myETA = DateTime.Now;
                            if (fullDate != null)
                            {
                             
                                if (ETA != null)
                                {
                                    myETA = Convert.ToDateTime(fullDate, System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat);
                                    //myETA = DateTime.ParseExact(fullDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                }
                            }
                            else
                            {
                                myETA = null;
                            }
                             if (checkExist != null)
                            {
                                int? total = checkExist.Quantity + TotalItem;

                                inputBalances.Remove(checkExist);
                                dataItemPipeline balance = new dataItemPipeline
                                {
                                    ItemPipelineGUID = Guid.NewGuid(),
                                    ItemPipelineUploadGUID = stockUpload.ItemPipelineUploadGUID,
                                    ItemGUID = CheckItem.ItemGUID,
                                    StockGUID = CheckStock.StockGUID,
                                    ETA= myETA,
                                    Quantity = total,
                                    Active = true
                                };
                                inputBalances.Add(balance);
                            }
                            else
                            {
                                dataItemPipeline balance = new dataItemPipeline
                                {
                                    ItemPipelineGUID = Guid.NewGuid(),
                                    ItemPipelineUploadGUID = stockUpload.ItemPipelineUploadGUID,
                                    ItemGUID = CheckItem.ItemGUID,
                                    StockGUID = CheckStock.StockGUID,
                                    Quantity = TotalItem,
                                    ETA = myETA,
                                    Active = true
                                };
                                inputBalances.Add(balance);
                            }

                            var currAll = stockAlls.Where(x => x.ItemGUID == CheckItem.ItemGUID).FirstOrDefault();
                            if (currAll == null)
                            {
                                dataItemPipeline myStockAll = new dataItemPipeline
                                {
                                    ItemPipelineGUID = Guid.NewGuid(),
                                    ItemPipelineUploadGUID = stockUpload.ItemPipelineUploadGUID,
                                    ItemGUID = CheckItem.ItemGUID,
                                    StockGUID = stockAllGUid,
                                    Quantity = TotalItem,
                                    ETA = myETA,
                                    Active = true

                                };
                                stockAlls.Add(myStockAll);
                            }
                            else
                            {
                                currAll.Quantity = currAll.Quantity + TotalItem;
                                stockAlls.Remove(currAll);

                                stockAlls.Add(currAll);
                            }
                        }
                        inputBalances.AddRange(stockAlls);
                        DbISS.UpdateBulk(uploads, Permissions.StockItemDistribution.UpdateGuid, DateTime.Now, DbCMS);
                        DbISS.Create(stockUpload, Permissions.StockItemDistribution.CreateGuid, DateTime.Now, DbCMS);
                        DbISS.CreateBulk(inputBalances, Permissions.StockItemDistribution.CreateGuid, DateTime.Now, DbCMS);
                   


                        try
                        {
                            DbISS.SaveChanges();
                            DbCMS.SaveChanges();
                            // return Json(DbISS.SingleUpdateMessage(DataTableNames.WarehouseModelEntryMovementsDataTable, DbISS.PrimaryKeyControl(EntryModel), DbISS.RowVersionControls(Portal.SingleToList(EntryModel))));
                        }
                        catch (Exception ex)
                        {
                            var error = DbISS.ErrorMessage(ex.Message);
                        }
                    }
                }
            }
            return "~/Uploads/ISS/temp/" + itemInput.ItemPipelineGUID + ".xlsx";
        }

        public FileResult ShowProcessdEntryUploadFile(string FilePath)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
            string fileName = "ItemSotck " + DateTime.Now.ToString("dddd, dd MMMM yyyy") + ".xlsx";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        /// <summary>
        /// Validate th Excel 
        /// </summary>
        /// <param name="workSheet"></param>
        /// <returns></returns>
        private bool Validate(ExcelWorksheet workSheet)
        {

            int totalRows = workSheet.Dimension.End.Row;
            bool valid = true;
            var items = DbISS.codeISSItemLanguage.Where(x => x.LanguageID == LAN).ToList();
            var warehouses = DbISS.codeISSStockLanguage.Where(x => x.LanguageID == LAN).ToList();
            for (int i = 2; i < totalRows; i++)
            {
                var stockName = workSheet.Cells["A" + i].Value;
                if (stockName == null)
                    break;
                var ItemName = workSheet.Cells["b" + i].Value;
                if (ItemName == null)
                    break;
                var quantity = workSheet.Cells["C" + i].Value;
                

                

                var CheckItemName = items.Where(x => x.ItemDescription.ToString().ToLower() == ItemName.ToString().ToLower()).FirstOrDefault();


                if (( CheckItemName == null || quantity == null))
                {
                    //workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                    //workSheet.Cells[cellStr].Style.Font.Bold = true;

                    workSheet.Cells["A" + i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Cells["A" + i].Style.Fill.BackgroundColor.SetColor(Color.Red);

                    valid = false;
                }
                else
                {
                    workSheet.Cells["A" + i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Cells["A" + i].Style.Fill.BackgroundColor.SetColor(Color.Green);
                }
            }

            return valid;

        }

        #endregion

        #region Export items 
        public ActionResult ExportItemPiplineTemplate()
        {
            var result = DbISS.codeISSItemLanguage.Where(x => x.LanguageID == LAN).ToList();
            if (result.Count > 0)
            {
                string sourceFile = Server.MapPath("~/Areas/ISS/Templates/PIPELINE.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/ISS/temp/PIPELINE" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();
                    dt.Columns.Add("Item Name", typeof(string));


                    foreach (var item in result)
                    {

                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.ItemDescription;

                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["K1"].LoadFromDataTable(dt, true);
                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = DateTime.Now.ToString("yyMMdd") + "_Template for stock pipline " + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }

            string success = "No Data Available for this items";
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);

        }
        #endregion


    }
}