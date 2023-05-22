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
    public class StockOverviewController : ISSBaseController
    {
        // GET: ISS/StockOverview
        [Route("ISS/StockOverview/")]
        public ActionResult StockItemOverViewIndex()
        {
            if (!CMS.HasAction(Permissions.ItemOverAllStock.Access, Apps.ISS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/ISS/Views/StockOverview/Index.cshtml");
        }

        [Route("ISS/ISSStockItemOverviewDataTable/")]
        public JsonResult ISSStockItemOverViewDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<StockItemOverviewDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<StockItemOverviewDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.StockItemOverView.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
            var All = (

                from a in DbISS.dataItemStockEmergencyReserve.Where(x => x.Active && x.dataItemStockEmergencyUpload.IsLastUpload == true).AsExpandable()
                join b in DbISS.codeISSItemLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.DeletedOn) && x.LanguageID == LAN) on a.ItemGUID equals b.ItemGUID
                join c in DbISS.dataItemStockEmergencyUpload.Where(x => x.IsLastUpload == true && (x.Active == true ? x.Active : x.DeletedOn == x.DeletedOn)) on a.ItemStockEmergencyUploadGUID equals c.ItemStockEmergencyUploadGUID

                into LJ1
                from R1 in LJ1.DefaultIfEmpty()

                select new StockItemOverviewDataTableModel
                {
                    ItemStockEmergencyReserveGUID = a.ItemStockEmergencyReserveGUID,
                    Active = a.Active,
                    TrackStockUploadGUID = a.ItemStockEmergencyUploadGUID,
                    ItemGUID = a.ItemGUID.ToString(),
                    ItemName = b.ItemDescription,
                    QuantityToBeReserved = a.QuantityToBeReserved,
                    ForWhere = a.ForWhere,
                    ExpectedDateToDispatch = a.ExpectedDateToDispatch,
                    TotalItemInAllStock = a.TotalItemInAllStock,
                    PipelineOrdersPlaced = a.PipelineOrdersPlaced,
                    TotalStockWithPipeline = a.TotalStockWithPipeline,
                    TotalReservedforEmergency = a.TotalReservedforEmergency,



                    dataItemStockEmergencyReserveRowVersion = a.dataItemStockEmergencyReserveRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<StockItemOverviewDataTableModel> Result = Mapper.Map<List<StockItemOverviewDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        #region Upload 
        [HttpGet]
        public ActionResult UploadFiles()
        {
            return PartialView("~/Areas/ISS/Views/StockOverview/_BulkStockOverviewUpload.cshtml",
                new dataItemStockEmergencyReserve());
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
            dataItemStockEmergencyReserve itemInput = new dataItemStockEmergencyReserve();
            itemInput.ItemStockEmergencyReserveGUID = Guid.NewGuid();
            string FilePath = Server.MapPath("~/Uploads/ISS/temp/" + itemInput.ItemStockEmergencyReserveGUID + ".xlsx");
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
                    List<dataItemStockBalance> itemStocks = new List<dataItemStockBalance>();
                    bool ok = Validate(workSheet);
                    package.Save();
                    if (ok)
                    {
                        int totalRows = workSheet.Dimension.End.Row;
                        List<dataItemStockEmergencyReserve> inputBalances = new List<dataItemStockEmergencyReserve>();
                        var items = DbISS.codeISSItemLanguage.Where(x => x.LanguageID == LAN).ToList();
                        var warehouses = DbISS.codeISSStockLanguage.Where(x => x.LanguageID == LAN).ToList();
                        var uploads = DbISS.dataItemStockEmergencyUpload.ToList();
                        foreach (var item in uploads)
                        {
                            item.IsLastUpload = false;
                        }
                        dataItemStockEmergencyUpload stockUpload = new dataItemStockEmergencyUpload
                        {
                            ItemStockEmergencyUploadGUID = Guid.NewGuid(),
                            UploadDate = ExecutionTime,
                            IsLastUpload = true,
                            CreateDate = ExecutionTime,
                            CreatedByGUID = UserGUID,
                            Active = true
                        };
                        for (int i = 2; i <= totalRows; i++)
                        {
                            dataItemStockEmergencyReserve ToAddInput = new dataItemStockEmergencyReserve();
                            var ItemName = workSheet.Cells["A" + i].Value;
                            if (ItemName == null)
                                break;
                            int? QuantityToBeReserved = Convert.ToInt32(workSheet.Cells["B" + i].Value);
                            var ForWhere = workSheet.Cells["C" + i].Value;
                            var ExpectedDateToDispatch = workSheet.Cells["D" + i].Value;
                            DateTime inputdate = DateTime.Now;
                            if (string.IsNullOrEmpty(ExpectedDateToDispatch.ToString()))
                            {
                                inputdate = DateTime.Now;
                            }
                            else
                            {
                                inputdate = DateTime.ParseExact(ExpectedDateToDispatch.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            }

                            var TotalItemInAllStock = Convert.ToInt32(workSheet.Cells["E" + i].Value);
                            var PipelineOrdersPlaced = Convert.ToInt32(workSheet.Cells["F" + i].Value);
                            var TotalStockWithPipeline = Convert.ToInt32(workSheet.Cells["G" + i].Value);
                            var TotalReservedforEmergency = Convert.ToInt32(workSheet.Cells["H" + i].Value);
                            //var TotalItem = workSheet.Cells["C" + i].Value;
                            var CheckItem = items.Where(x => x.ItemDescription.ToString().ToLower() == ItemName.ToString().ToLower()).FirstOrDefault();



                            dataItemStockEmergencyReserve balance = new dataItemStockEmergencyReserve
                            {
                                ItemStockEmergencyReserveGUID = Guid.NewGuid(),
                                ItemStockEmergencyUploadGUID = stockUpload.ItemStockEmergencyUploadGUID,
                                ItemGUID = CheckItem.ItemGUID,
                                QuantityToBeReserved = QuantityToBeReserved,
                                ForWhere = ForWhere.ToString(),
                                ExpectedDateToDispatch = inputdate,
                                TotalItemInAllStock = TotalItemInAllStock,
                                PipelineOrdersPlaced = PipelineOrdersPlaced,
                                TotalStockWithPipeline = TotalStockWithPipeline,
                                TotalReservedforEmergency = TotalReservedforEmergency,
                                Active = true
                            };
                            inputBalances.Add(balance);
                        }
                        DbISS.UpdateBulk(uploads, Permissions.ItemOverAllStock.UpdateGuid, DateTime.Now, DbCMS);
                        DbISS.Create(stockUpload, Permissions.ItemOverAllStock.CreateGuid, DateTime.Now, DbCMS);
                        DbISS.CreateBulk(inputBalances, Permissions.ItemOverAllStock.CreateGuid, DateTime.Now, DbCMS);
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
            return "~/Uploads/ISS/temp/" + itemInput.ItemStockEmergencyReserveGUID + ".xlsx";
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

            for (int i = 2; i < totalRows; i++)
            {
                var ItemName = workSheet.Cells["A" + i].Value;
                if (ItemName == null)
                    break;



                var CheckItemName = items.Where(x => x.ItemDescription.ToString().ToLower() == ItemName.ToString().ToLower()).FirstOrDefault();


                if ((CheckItemName == null))
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
    }
}