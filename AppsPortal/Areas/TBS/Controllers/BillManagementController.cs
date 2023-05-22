using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Library.MimeDetective;
using AutoMapper;
using LinqKit;
using OfficeOpenXml;
using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using TBS_DAL.Model;
using Z.EntityFramework.Extensions;


namespace AppsPortal.Areas.TBS.Controllers
{
    public class BillManagementController : TBSBaseController
    {

        private Guid _BillForTypeGUID_Mobile = Guid.Parse("9D8A1EB9-C2AC-4D78-95FF-874E46074321");
        private Guid _SyriatelCompany = Guid.Parse("3520bbcc-cabb-4855-83a4-02b7b5c65e11");
        private Guid _MTNCompany = Guid.Parse("87b3058b-34fe-496b-bf45-2b14d8fe5f9f");

        private Guid SourceCountryGUID = Guid.Parse("710BFD1B-50CC-4F1E-92A9-A70583CFA5E0");
        private Guid SyriaCountryGUID = Guid.Parse("710BFD1B-50CC-4F1E-92A9-A70583CFA5E0");
        private Guid _BillForTypeGUID_Land = Guid.Parse("b2980068-688d-428d-9e5c-494656ba9d2c");

        DateTime RoundUp(DateTime dt, TimeSpan d)
        {
            return new DateTime((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, dt.Kind);
        }

        [Route("TBS/BillManagement/Bills")]
        public ActionResult Index()
        {
            return View("~/Areas/TBS/Views/BillManagement/Index.cshtml");
        }

        [Route("TBS/BillManagement/DataBillsDataTable/")]
        public JsonResult BillsDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<DataBillsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<DataBillsDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbTBS.dataBill.AsExpandable()
                       join b in DbTBS.configTelecomCompanyOperation.AsExpandable() on a.TelecomCompanyOperationConfigGUID equals b.TelecomCompanyOperationConfigGUID
                       join c in DbTBS.codeTelecomCompanyOperation.AsExpandable() on b.TelecomCompanyOperationGUID equals c.TelecomCompanyOperationGUID
                       join d in DbTBS.codeTelecomCompanyLanguages.AsExpandable().Where(x => x.LanguageID == LAN && x.Active) on c.TelecomCompanyGUID equals d.TelecomCompanyGUID
                       join e in DbTBS.codeOperationsLanguages.AsExpandable().Where(x => x.LanguageID == LAN && x.Active) on c.OperationGUID equals e.OperationGUID
                       join f in DbTBS.codeTablesValuesLanguages.AsExpandable().Where(x => x.LanguageID == LAN && x.Active) on a.BillForTypeGUID equals f.ValueGUID
                       join j in DbTBS.userPersonalDetailsLanguage.AsExpandable().Where(x => x.LanguageID == LAN && x.Active) on a.UploadedBy equals j.UserGUID
                       join h in DbTBS.userPersonalDetailsLanguage.AsExpandable().Where(x => x.LanguageID == LAN && x.Active) on a.ProcessedBy equals h.UserGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       select new DataBillsDataTableModel
                       {
                           BillGUID = a.BillGUID,
                           BillForTypeDescription = f.ValueDescription,
                           BillForMonth = a.BillForMonth,
                           BillForYear = a.BillForYear,
                           BillPeriodStartDate = a.BillPeriodStartDate,
                           UploadDate = a.UploadDate,
                           Active = a.Active,
                           dataBillRowVersion = a.dataBillRowVersion,
                           TelecomCompanyDescription = d.TelecomCompanyDescription,
                           OperationDescription = e.OperationDescription,
                           UploadedByGUID = a.UploadedBy,
                           UploadedBy = j.FirstName + " " + j.Surname,
                           IsProccessed = a.IsProcessed,
                           ProcessedByGUID = a.ProcessedBy,
                           ProcessedBy = R1.FirstName + " " + R1.Surname,
                           ProcessedOn = a.ProcessedOn
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<DataBillsDataTableModel> Result = Mapper.Map<List<DataBillsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("TBS/BillManagement/Create")]
        public ActionResult BillCreate()
        {
            //return Json(DbTBS.ErrorMessage("Stopped Temporary"));
            //if (!CMS.HasAction(Permissions.TelecomCompanies.Create, Apps.TBS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            return View("~/Areas/TBS/Views/BillManagement/Bill.cshtml", new DataBillUpdateModel());
        }

        private static IDictionary<Guid, int> createBillTask = new Dictionary<Guid, int>();

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult BillCreate(DataBillUpdateModel model)
        {

            //return Json(DbTBS.ErrorMessage("Stopped Temporary"));

            lock (this)
            {
                try
                {
                    if (!ModelState.IsValid) return PartialView("~/Areas/TBS/Views/BillManagement/_BillForm.cshtml", model);

                    if (!ValidateNewBill(model))
                    {
                        model.Active = true;
                        return PartialView("~/Areas/TBS/Views/BillManagement/_BillForm.cshtml", model);
                    }

                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        if (!FileTypeValidator.IsExcel(Request.Files[i].InputStream))
                        {
                            ModelState.AddModelError("", "Bill files types allowed is excel only");
                            return PartialView("~/Areas/TBS/Views/BillManagement/_BillForm.cshtml", model);
                        }
                    }

                    int _totalRowsCount = 0;

                  
                     

                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        var _tempstearmX = Request.Files[i].InputStream;

                        using (var package = new ExcelPackage(_tempstearmX))
                        {
                            foreach (var ws in package.Workbook.Worksheets)
                            {
                                _totalRowsCount += ws.Dimension.End.Row;
                            }
                        }
                    }



                    DateTime ExecutionTime = DateTime.Now;
                    Guid EntityPK = Guid.NewGuid();
                    var createBillTaskID = Guid.NewGuid();
                    createBillTask.Add(createBillTaskID, 0);
                    List<string> BillFiles = new List<string>();
                    if (model.TelecomCompanyOperationConfigGUID == _SyriatelCompany)        //syriatel syria
                    {
                        int filesCount = Request.Files.Count;
                        var file = Request.Files[0];
                        var _stearm = file.InputStream;
                        string _ext = Path.GetExtension(file.FileName);
                        BillFiles.Add("~/Areas/TBS/MobileFiles/SYRDA/SYRIATEL/" + EntityPK.ToString() + _ext);
                        file.SaveAs(Server.MapPath("~/Areas/TBS/MobileFiles/SYRDA/SYRIATEL/" + EntityPK.ToString() + _ext));
                    }
                    else if (model.TelecomCompanyOperationConfigGUID == _MTNCompany)   //mtn syria
                    {
                        for (int i = 0; i < Request.Files.Count; i++)
                        {
                            var file = Request.Files[i];
                            var _stearm = file.InputStream;
                            string _ext = Path.GetExtension(file.FileName);
                            BillFiles.Add("~/Areas/TBS/MobileFiles/SYRDA/MTN/" + EntityPK.ToString() + _ext);
                            file.SaveAs(Server.MapPath("~/Areas/TBS/MobileFiles/SYRDA/MTN/" + EntityPK.ToString() + _ext));
                        }
                    }

                    Task.Factory.StartNew(() =>
                    {
                        List<dataBill> dataBills = new List<dataBill>();
                        dataBill dataBill = new dataBill();
                        dataBill = Mapper.Map(model, dataBill);
                        dataBill.BillGUID = EntityPK;
                        dataBill.BillForYear = model.BillForYear;
                        dataBill.BillForMonth = model.BillForMonth;
                        dataBill.BillDeadLine = model.BillDeadLine;
                        dataBill.UploadedBy = UserGUID;
                        dataBill.UploadDate = ExecutionTime;
                        dataBill.BillPeriodStartDate = new DateTime(model.BillForYear, model.BillForMonth, 1);
                        dataBill.IsProcessed = false;
                        dataBill.Active = true;
                        dataBills.Add(dataBill);

                        DbTBS.BulkInsert(dataBills);
                        //mobile
                        //syriatel syria
                        if (model.TelecomCompanyOperationConfigGUID == _SyriatelCompany)
                        {
                            List<dataBillToProcess> billToProcessList = new List<dataBillToProcess>();
                            List<dataBillFile> dataBillFiles = new List<dataBillFile>();
                            configTelecomCompanyOperationMobileColumn configTelecomCompanyOperationMobileColumns = DbTBS.configTelecomCompanyOperation.Where(x => x.Active && x.TelecomCompanyOperationConfigGUID == dataBill.TelecomCompanyOperationConfigGUID).FirstOrDefault().configTelecomCompanyOperationMobileColumn.Where(x => x.Active).FirstOrDefault();

                            foreach (string billFilePath in BillFiles)
                            {
                                dataBillFile dataBillFile = new dataBillFile();
                                dataBillFile.BillFileGUID = Guid.NewGuid();
                                dataBillFile.BillGUID = EntityPK;
                                dataBillFile.Active = true;
                                dataBillFile.FilePath = billFilePath;
                                dataBillFiles.Add(dataBillFile);
                                FileInfo billFileInfo = new FileInfo(Server.MapPath(dataBillFile.FilePath));

                                using (var package = new ExcelPackage(billFileInfo))
                                {

                                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                                    for (int row = 2; worksheet.Cells[row, 1].Value != null; row++)
                                    {
                                        dataBillToProcess dataBillToProcess = new dataBillToProcess();
                                        dataBillToProcess.IsPrivate = false;
                                        try
                                        {
                                            string callerCell = worksheet.Cells[row, configTelecomCompanyOperationMobileColumns.CallerNumberColumnIndex + 1].Value.ToString().Trim();
                                            #region

                                            #region used code
                                            //add user to user bills if not exists and bind the details to it
                                            string callDateCell = worksheet.Cells[row, configTelecomCompanyOperationMobileColumns.CallDateTimeColumnIndex + 1].Value.ToString().Trim();

                                            DateTime callDateTime;
                                            DateTime callDateTimeUniversal;

                                            if (DateTime.TryParse(callDateCell, out callDateTime))
                                            {
                                                DateTime.TryParse(callDateCell, out callDateTime);
                                                callDateTimeUniversal = callDateTime.ToUniversalTime();
                                            }
                                            else
                                            {
                                                callDateTime = DateTime.FromOADate(Convert.ToDouble(callDateCell));
                                                callDateTimeUniversal = callDateTime.ToUniversalTime();
                                            }


                                            string receiverCell = worksheet.Cells[row, configTelecomCompanyOperationMobileColumns.ReceiverNumberColumnIndex + 1].Value.ToString().Trim();
                                            string destinationTypeCell = worksheet.Cells[row, configTelecomCompanyOperationMobileColumns.DestinationTypeColumnIndex + 1].Value.ToString().Trim();
                                            string costCell = worksheet.Cells[row, configTelecomCompanyOperationMobileColumns.CostColumnIndex + 1].Value.ToString().Trim();
                                            //if (costCell.Length == 0 || costCell == "0")
                                            //{
                                            //    continue;
                                            //}
                                            string durationCell = worksheet.Cells[row, configTelecomCompanyOperationMobileColumns.CallDurationColumnIndex + 1].Value.ToString().Trim();


                                            dataBillToProcess.BillToProcessGUID = Guid.NewGuid();
                                            dataBillToProcess.Active = true;
                                            dataBillToProcess.BillFileGUID = dataBillFile.BillFileGUID;
                                            dataBillToProcess.CallDateTime = callDateTime;
                                            dataBillToProcess.ReceiverNumber = receiverCell;
                                            dataBillToProcess.CallCost = Convert.ToDouble(costCell);
                                            dataBillToProcess.CallDurationInMinutes = Convert.ToInt32(durationCell);
                                            dataBillToProcess.CallerNumber = callerCell;
                                            if (destinationTypeCell == "موبايل")
                                            {
                                                dataBillToProcess.DestinationType = MobileCallTypes.MobileLine;
                                            }
                                            else if (destinationTypeCell == "ثابت")
                                            {
                                                dataBillToProcess.DestinationType = MobileCallTypes.LandLine;
                                            }
                                            else if (destinationTypeCell == "رسالة")
                                            {
                                                dataBillToProcess.DestinationType = MobileCallTypes.SMS;
                                            }
                                            else if (destinationTypeCell == "دولي")
                                            {
                                                dataBillToProcess.DestinationType = MobileCallTypes.International;
                                            }
                                            else if (destinationTypeCell == "تجوال")
                                            {
                                                dataBillToProcess.DestinationType = MobileCallTypes.Roaming;
                                            }
                                            else if (destinationTypeCell == "حزمة")
                                            {
                                                dataBillToProcess.DestinationType = MobileCallTypes.Bundle;
                                            }
                                            else if (destinationTypeCell == "إ.تجوال")
                                            {
                                                dataBillToProcess.DestinationType = MobileCallTypes.RoamingInternet;
                                            }
                                            else if (destinationTypeCell == "ر.تجوال")
                                            {
                                                dataBillToProcess.DestinationType = MobileCallTypes.RoamingSMS;
                                            }
                                            else if (destinationTypeCell == "رنة")
                                            {
                                                dataBillToProcess.DestinationType = MobileCallTypes.Ring;
                                            }
                                            else if (destinationTypeCell == "خدمة")
                                            {
                                                dataBillToProcess.DestinationType = MobileCallTypes.Service;
                                            }
                                            else if (destinationTypeCell == "ملتيميديا")
                                            {
                                                dataBillToProcess.DestinationType = MobileCallTypes.Multimedia;
                                            }
                                            else
                                            {
                                                dataBillToProcess.DestinationType = MobileCallTypes.Other;
                                            }
                                            #endregion
                                            billToProcessList.Add(dataBillToProcess);
                                            #endregion
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        createBillTask[createBillTaskID] = row;
                                    }
                                }
                            }

                            DbTBS.BulkInsert(dataBillFiles);
                            DbTBS.BulkInsert(billToProcessList);


                        }
                        //mtn syria
                        else if (model.TelecomCompanyOperationConfigGUID == _MTNCompany)
                        {
                            List<dataBillToProcess> billToProcessList = new List<dataBillToProcess>();
                            List<dataBillFile> dataBillFiles = new List<dataBillFile>();
                            configTelecomCompanyOperationMobileColumn configTelecomCompanyOperationMobileColumns = DbTBS.configTelecomCompanyOperation.Where(x => x.Active && x.TelecomCompanyOperationConfigGUID == dataBill.TelecomCompanyOperationConfigGUID).FirstOrDefault().configTelecomCompanyOperationMobileColumn.Where(x => x.Active).FirstOrDefault();
                            int _totalSheetRow = 2;
                            foreach (string billFilePath in BillFiles)
                            {

                                dataBillFile dataBillFile = new dataBillFile();
                                dataBillFile.BillFileGUID = Guid.NewGuid();
                                dataBillFile.BillGUID = EntityPK;
                                dataBillFile.FilePath = billFilePath;
                                dataBillFile.Active = true;
                                dataBillFiles.Add(dataBillFile);
                                FileInfo billFileInfo = new FileInfo(Server.MapPath(dataBillFile.FilePath));
                                using (var package = new ExcelPackage(billFileInfo))
                                {

                                    foreach (var worksheet in package.Workbook.Worksheets)
                                    {
                                        try
                                        {

                                            for (int row = 2; worksheet.Cells[row, 1].Value != null; row++)
                                            {
                                                try
                                                {
                                                    _totalSheetRow++;
                                                    dataBillToProcess dataBillToProcess = new dataBillToProcess();
                                                    dataBillToProcess.IsPrivate = false;
                                                    #region used code
                                                    string callDateCell = worksheet.Cells[row, configTelecomCompanyOperationMobileColumns.CallDateTimeColumnIndex + 1].Value.ToString().Trim();

                                                    DateTime callDateTime;
                                                    DateTime callDateTimeUniversal;
                                                    if (DateTime.TryParse(callDateCell, out callDateTime))
                                                    {
                                                        DateTime.TryParse(callDateCell, out callDateTime);
                                                        callDateTimeUniversal = callDateTime.ToUniversalTime();
                                                    }
                                                    else
                                                    {
                                                        callDateTime = DateTime.FromOADate(Convert.ToDouble(callDateCell));
                                                        callDateTimeUniversal = callDateTime.ToUniversalTime();
                                                    }
                                                    //DateTime callDateTime = DateTime.FromOADate(Convert.ToDouble(callDateCell));
                                                    //DateTime callDateTimeUniversal = callDateTime.ToUniversalTime();

                                                    string receiverCell = worksheet.Cells[row, configTelecomCompanyOperationMobileColumns.ReceiverNumberColumnIndex + 1].Value.ToString().Trim();
                                                    string destinationTypeCell = "";
                                                    try
                                                    {
                                                        destinationTypeCell = worksheet.Cells[row, configTelecomCompanyOperationMobileColumns.DestinationTypeColumnIndex + 1].Value.ToString().Trim();
                                                    }
                                                    catch
                                                    {
                                                        destinationTypeCell = "CALL";
                                                    }


                                                    string costCell = worksheet.Cells[row, configTelecomCompanyOperationMobileColumns.CostColumnIndex + 1].Value.ToString().Trim();
                                                    //if (costCell.Length == 0 || costCell == "0")
                                                    //{
                                                    //    continue;
                                                    //}
                                                    string durationCell = "";
                                                    try
                                                    {
                                                        durationCell = worksheet.Cells[row, configTelecomCompanyOperationMobileColumns.CallDurationColumnIndex + 1].Value.ToString().Trim();
                                                    }
                                                    catch
                                                    {
                                                        durationCell = "0";
                                                    }

                                                    string callerCell = worksheet.Cells[row, configTelecomCompanyOperationMobileColumns.CallerNumberColumnIndex + 1].Value.ToString().Trim();


                                                    dataBillToProcess.BillToProcessGUID = Guid.NewGuid();
                                                    dataBillToProcess.Active = true;
                                                    dataBillToProcess.BillFileGUID = dataBillFile.BillFileGUID;
                                                    dataBillToProcess.CallDateTime = callDateTime;
                                                    dataBillToProcess.ReceiverNumber = receiverCell;
                                                    dataBillToProcess.CallCost = Convert.ToDouble(costCell.Replace("KB", "").Trim());
                                                    dataBillToProcess.CallDurationInMinutes = Convert.ToInt32(durationCell.Replace("KB", "").Trim());
                                                    dataBillToProcess.CallerNumber = callerCell.Remove(0, 4);


                                                    if (destinationTypeCell.ToUpper() == "CALL".ToUpper())
                                                    {
                                                        dataBillToProcess.DestinationType = MobileCallTypes.Call;
                                                    }
                                                    else if (destinationTypeCell.ToUpper() == "GPRS".ToUpper())
                                                    {
                                                        dataBillToProcess.DestinationType = MobileCallTypes.LandLine;
                                                    }
                                                    else if (destinationTypeCell.ToUpper() == "SMS".ToUpper())
                                                    {
                                                        dataBillToProcess.DestinationType = MobileCallTypes.SMS;
                                                    }
                                                    else if (destinationTypeCell == "دولي")
                                                    {
                                                        dataBillToProcess.DestinationType = MobileCallTypes.International;
                                                    }
                                                    else if (destinationTypeCell == "تجوال")
                                                    {
                                                        dataBillToProcess.DestinationType = MobileCallTypes.Roaming;
                                                    }
                                                    else if (destinationTypeCell == "حزمة")
                                                    {
                                                        dataBillToProcess.DestinationType = MobileCallTypes.Bundle;
                                                    }
                                                    else if (destinationTypeCell == "إ.تجوال")
                                                    {
                                                        dataBillToProcess.DestinationType = MobileCallTypes.RoamingInternet;
                                                    }
                                                    else if (destinationTypeCell == "ر.تجوال")
                                                    {
                                                        dataBillToProcess.DestinationType = MobileCallTypes.RoamingSMS;
                                                    }
                                                    else if (destinationTypeCell == "رنة")
                                                    {
                                                        dataBillToProcess.DestinationType = MobileCallTypes.Ring;
                                                    }
                                                    else
                                                    {
                                                        dataBillToProcess.DestinationType = MobileCallTypes.Other;
                                                    }
                                                    #endregion
                                                    billToProcessList.Add(dataBillToProcess);
                                                }
                                                catch (Exception ex)
                                                {
                                                    var asd = row;
                                                }

                                                createBillTask[createBillTaskID] = _totalSheetRow;
                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }

                                }
                            }

                            DbTBS.BulkInsert(dataBillFiles);
                            DbTBS.BulkInsert(billToProcessList);
                        }

                        try
                        {
                            DbTBS.BulkSaveChanges();
                            createBillTask.Remove(createBillTaskID);
                            return Json(DbTBS.SingleCreateMessage("Saved, file will be processed !"));
                        }
                        catch (Exception ex)
                        {
                            return Json(DbTBS.ErrorMessage(ex.Message));
                        }
                    });
                    return Json(new { createBillTaskID = createBillTaskID, TotalRecords = _totalRowsCount });


                }
                catch (Exception ex)
                {
                    return Json(DbTBS.ErrorMessage(ex.Message));
                }
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult BillCreateOriginal(DataBillUpdateModel model)
        {

            try
            {
                if (!ModelState.IsValid) return PartialView("~/Areas/TBS/Views/BillManagement/_BillForm.cshtml", model);

                if (!ValidateNewBill(model))
                {
                    model.Active = true;
                    return PartialView("~/Areas/TBS/Views/BillManagement/_BillForm.cshtml", model);
                }

                int _totalRowsCount = 0;
                //foreach (HttpPostedFileBase _file in Request.Files)
                //{
                //    var _tempstearm = _file.InputStream;
                //    using (var package = new ExcelPackage(_tempstearm))
                //    {
                //        foreach (var ws in package.Workbook.Worksheets)
                //        {
                //            _totalRowsCount += ws.Dimension.End.Row;
                //        }
                //    }
                //}
                var _tempfile = Request.Files[0];
                var _tempstearm = _tempfile.InputStream;

                using (var package = new ExcelPackage(_tempstearm))
                {
                    foreach (var ws in package.Workbook.Worksheets)
                    {
                        _totalRowsCount += ws.Dimension.End.Row;
                    }
                }

                DateTime ExecutionTime = DateTime.Now;
                Guid EntityPK = Guid.NewGuid();
                var createBillTaskID = Guid.NewGuid();
                createBillTask.Add(createBillTaskID, 0);
                int filesCount = Request.Files.Count;
                var file = Request.Files[0];
                var _stearm = file.InputStream;
                string _ext = Path.GetExtension(file.FileName);
                if (model.TelecomCompanyOperationConfigGUID == _SyriatelCompany)        //syriatel syria
                {
                    file.SaveAs(Server.MapPath("~/Areas/TBS/MobileFiles/SYRDA/SYRIATEL/" + EntityPK.ToString() + _ext));
                }
                else if (model.TelecomCompanyOperationConfigGUID == _MTNCompany)   //mtn syria
                {
                    file.SaveAs(Server.MapPath("~/Areas/TBS/MobileFiles/SYRDA/MTN/" + EntityPK.ToString() + _ext));
                }
                Task.Factory.StartNew(() =>
                {
                    List<dataBill> dataBills = new List<dataBill>();
                    dataBill dataBill = new dataBill();
                    dataBill = Mapper.Map(model, dataBill);
                    dataBill.BillGUID = EntityPK;
                    dataBill.BillForYear = model.BillForYear;
                    dataBill.BillForMonth = model.BillForMonth;
                    dataBill.UploadedBy = UserGUID;
                    dataBill.UploadDate = ExecutionTime;
                    dataBill.BillPeriodStartDate = new DateTime(model.BillForYear, model.BillForMonth, 1);
                    dataBill.IsProcessed = false;
                    dataBill.Active = true;
                    dataBills.Add(dataBill);
                    DbTBS.BulkInsert(dataBills);
                    //mobile
                    //syriatel syria
                    if (model.TelecomCompanyOperationConfigGUID == _SyriatelCompany)
                    {
                        dataBillFile dataBillFile = new dataBillFile();
                        dataBillFile.BillFileGUID = Guid.NewGuid();
                        dataBillFile.BillGUID = EntityPK;
                        dataBillFile.Active = true;
                        dataBillFile.FilePath = "~/Areas/TBS/MobileFiles/SYRDA/SYRIATEL/" + EntityPK.ToString() + _ext;
                        List<dataBillToProcess> billToProcessList = new List<dataBillToProcess>();
                        configTelecomCompanyOperationMobileColumn configTelecomCompanyOperationMobileColumns = DbTBS.configTelecomCompanyOperation.Where(x => x.Active && x.TelecomCompanyOperationConfigGUID == dataBill.TelecomCompanyOperationConfigGUID).FirstOrDefault().configTelecomCompanyOperationMobileColumn.Where(x => x.Active).FirstOrDefault();
                        FileInfo billFileInfo = new FileInfo(Server.MapPath(dataBillFile.FilePath));
                        using (var package = new ExcelPackage(billFileInfo))
                        {

                            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                            for (int row = 2; worksheet.Cells[row, 1].Value != null; row++)
                            {
                                dataBillToProcess dataBillToProcess = new dataBillToProcess();
                                dataBillToProcess.IsPrivate = false;
                                try
                                {
                                    string callerCell = worksheet.Cells[row, configTelecomCompanyOperationMobileColumns.CallerNumberColumnIndex + 1].Value.ToString().Trim();
                                    if (callerCell == "30033941")
                                    {
                                        int x = 0;
                                    }
                                    #region

                                    #region used code
                                    //add user to user bills if not exists and bind the details to it
                                    string callDateCell = worksheet.Cells[row, configTelecomCompanyOperationMobileColumns.CallDateTimeColumnIndex + 1].Value.ToString().Trim();

                                    DateTime callDateTime;
                                    DateTime callDateTimeUniversal;

                                    if (DateTime.TryParse(callDateCell, out callDateTime))
                                    {
                                        DateTime.TryParse(callDateCell, out callDateTime);
                                        callDateTimeUniversal = callDateTime.ToUniversalTime();
                                    }
                                    else
                                    {
                                        callDateTime = DateTime.FromOADate(Convert.ToDouble(callDateCell));
                                        callDateTimeUniversal = callDateTime.ToUniversalTime();
                                    }


                                    string receiverCell = worksheet.Cells[row, configTelecomCompanyOperationMobileColumns.ReceiverNumberColumnIndex + 1].Value.ToString().Trim();
                                    string destinationTypeCell = worksheet.Cells[row, configTelecomCompanyOperationMobileColumns.DestinationTypeColumnIndex + 1].Value.ToString().Trim();
                                    string costCell = worksheet.Cells[row, configTelecomCompanyOperationMobileColumns.CostColumnIndex + 1].Value.ToString().Trim();
                                    //if (costCell.Length == 0 || costCell == "0")
                                    //{
                                    //    continue;
                                    //}
                                    string durationCell = worksheet.Cells[row, configTelecomCompanyOperationMobileColumns.CallDurationColumnIndex + 1].Value.ToString().Trim();


                                    dataBillToProcess.BillToProcessGUID = Guid.NewGuid();
                                    dataBillToProcess.Active = true;
                                    dataBillToProcess.BillFileGUID = dataBillFile.BillFileGUID;
                                    dataBillToProcess.CallDateTime = callDateTime;
                                    dataBillToProcess.ReceiverNumber = receiverCell;
                                    dataBillToProcess.CallCost = Convert.ToDouble(costCell);
                                    dataBillToProcess.CallDurationInMinutes = Convert.ToInt32(durationCell);
                                    dataBillToProcess.CallerNumber = callerCell;
                                    if (destinationTypeCell == "موبايل")
                                    {
                                        dataBillToProcess.DestinationType = MobileCallTypes.MobileLine;
                                    }
                                    else if (destinationTypeCell == "ثابت")
                                    {
                                        dataBillToProcess.DestinationType = MobileCallTypes.LandLine;
                                    }
                                    else if (destinationTypeCell == "رسالة")
                                    {
                                        dataBillToProcess.DestinationType = MobileCallTypes.SMS;
                                    }
                                    else if (destinationTypeCell == "دولي")
                                    {
                                        dataBillToProcess.DestinationType = MobileCallTypes.International;
                                    }
                                    else if (destinationTypeCell == "تجوال")
                                    {
                                        dataBillToProcess.DestinationType = MobileCallTypes.Roaming;
                                    }
                                    else if (destinationTypeCell == "حزمة")
                                    {
                                        dataBillToProcess.DestinationType = MobileCallTypes.Bundle;
                                    }
                                    else if (destinationTypeCell == "إ.تجوال")
                                    {
                                        dataBillToProcess.DestinationType = MobileCallTypes.RoamingInternet;
                                    }
                                    else if (destinationTypeCell == "ر.تجوال")
                                    {
                                        dataBillToProcess.DestinationType = MobileCallTypes.RoamingSMS;
                                    }
                                    else if (destinationTypeCell == "رنة")
                                    {
                                        dataBillToProcess.DestinationType = MobileCallTypes.Ring;
                                    }
                                    else if (destinationTypeCell == "خدمة")
                                    {
                                        dataBillToProcess.DestinationType = MobileCallTypes.Service;
                                    }
                                    else if (destinationTypeCell == "ملتيميديا")
                                    {
                                        dataBillToProcess.DestinationType = MobileCallTypes.Multimedia;
                                    }
                                    else
                                    {
                                        dataBillToProcess.DestinationType = MobileCallTypes.Other;
                                    }
                                    #endregion
                                    billToProcessList.Add(dataBillToProcess);
                                    #endregion
                                }
                                catch (Exception ex)
                                {

                                }

                                createBillTask[createBillTaskID] = row;
                            }
                        }
                        List<dataBillFile> dataBillFiles = new List<dataBillFile>();
                        dataBillFiles.Add(dataBillFile);
                        DbTBS.BulkInsert(dataBillFiles);
                        DbTBS.BulkInsert(billToProcessList);
                    }
                    //mtn syria
                    else if (model.TelecomCompanyOperationConfigGUID == _MTNCompany)
                    {
                        List<dataBillToProcess> billToProcessList = new List<dataBillToProcess>();
                        dataBillFile dataBillFile = new dataBillFile();
                        dataBillFile.BillFileGUID = Guid.NewGuid();
                        dataBillFile.BillGUID = EntityPK;
                        dataBillFile.FilePath = "~/Areas/TBS/MobileFiles/SYRDA/MTN/" + EntityPK.ToString() + _ext;
                        dataBillFile.Active = true;
                        configTelecomCompanyOperationMobileColumn configTelecomCompanyOperationMobileColumns = DbTBS.configTelecomCompanyOperation.Where(x => x.Active && x.TelecomCompanyOperationConfigGUID == dataBill.TelecomCompanyOperationConfigGUID).FirstOrDefault().configTelecomCompanyOperationMobileColumn.Where(x => x.Active).FirstOrDefault();
                        FileInfo billFileInfo = new FileInfo(Server.MapPath(dataBillFile.FilePath));
                        using (var package = new ExcelPackage(billFileInfo))
                        {
                            int _totalSheetRow = 2;
                            foreach (var worksheet in package.Workbook.Worksheets)
                            {
                                try
                                {

                                    for (int row = 2; worksheet.Cells[row, 1].Value != null; row++)
                                    {
                                        try
                                        {
                                            _totalSheetRow++;
                                            dataBillToProcess dataBillToProcess = new dataBillToProcess();
                                            dataBillToProcess.IsPrivate = false;
                                            #region used code
                                            string callDateCell = worksheet.Cells[row, configTelecomCompanyOperationMobileColumns.CallDateTimeColumnIndex + 1].Value.ToString().Trim();

                                            DateTime callDateTime;
                                            DateTime callDateTimeUniversal;
                                            if (DateTime.TryParse(callDateCell, out callDateTime))
                                            {
                                                DateTime.TryParse(callDateCell, out callDateTime);
                                                callDateTimeUniversal = callDateTime.ToUniversalTime();
                                            }
                                            else
                                            {
                                                callDateTime = DateTime.FromOADate(Convert.ToDouble(callDateCell));
                                                callDateTimeUniversal = callDateTime.ToUniversalTime();
                                            }
                                            //DateTime callDateTime = DateTime.FromOADate(Convert.ToDouble(callDateCell));
                                            //DateTime callDateTimeUniversal = callDateTime.ToUniversalTime();

                                            string receiverCell = worksheet.Cells[row, configTelecomCompanyOperationMobileColumns.ReceiverNumberColumnIndex + 1].Value.ToString().Trim();
                                            string destinationTypeCell = worksheet.Cells[row, configTelecomCompanyOperationMobileColumns.DestinationTypeColumnIndex + 1].Value.ToString().Trim();
                                            string costCell = worksheet.Cells[row, configTelecomCompanyOperationMobileColumns.CostColumnIndex + 1].Value.ToString().Trim();
                                            //if (costCell.Length == 0 || costCell == "0")
                                            //{
                                            //    continue;
                                            //}
                                            string durationCell = worksheet.Cells[row, configTelecomCompanyOperationMobileColumns.CallDurationColumnIndex + 1].Value.ToString().Trim();
                                            string callerCell = worksheet.Cells[row, configTelecomCompanyOperationMobileColumns.CallerNumberColumnIndex + 1].Value.ToString().Trim();

                                            dataBillToProcess.BillFileGUID = dataBillFile.BillFileGUID;
                                            dataBillToProcess.CallDateTime = callDateTime;
                                            dataBillToProcess.ReceiverNumber = receiverCell;
                                            dataBillToProcess.CallCost = Convert.ToDouble(costCell.Replace("KB", "").Trim());
                                            dataBillToProcess.CallDurationInMinutes = Convert.ToInt32(durationCell.Replace("KB", "").Trim());
                                            dataBillToProcess.CallerNumber = callerCell.Remove(0, 4);

                                            if (destinationTypeCell.ToUpper() == "CALL".ToUpper())
                                            {
                                                dataBillToProcess.DestinationType = MobileCallTypes.Call;
                                            }
                                            else if (destinationTypeCell.ToUpper() == "GPRS".ToUpper())
                                            {
                                                dataBillToProcess.DestinationType = MobileCallTypes.LandLine;
                                            }
                                            else if (destinationTypeCell.ToUpper() == "SMS".ToUpper())
                                            {
                                                dataBillToProcess.DestinationType = MobileCallTypes.SMS;
                                            }
                                            else if (destinationTypeCell == "دولي")
                                            {
                                                dataBillToProcess.DestinationType = MobileCallTypes.International;
                                            }
                                            else if (destinationTypeCell == "تجوال")
                                            {
                                                dataBillToProcess.DestinationType = MobileCallTypes.Roaming;
                                            }
                                            else if (destinationTypeCell == "حزمة")
                                            {
                                                dataBillToProcess.DestinationType = MobileCallTypes.Bundle;
                                            }
                                            else if (destinationTypeCell == "إ.تجوال")
                                            {
                                                dataBillToProcess.DestinationType = MobileCallTypes.RoamingInternet;
                                            }
                                            else if (destinationTypeCell == "ر.تجوال")
                                            {
                                                dataBillToProcess.DestinationType = MobileCallTypes.RoamingSMS;
                                            }
                                            else if (destinationTypeCell == "رنة")
                                            {
                                                dataBillToProcess.DestinationType = MobileCallTypes.Ring;
                                            }
                                            else
                                            {
                                                dataBillToProcess.DestinationType = MobileCallTypes.Other;
                                            }
                                            #endregion
                                            billToProcessList.Add(dataBillToProcess);
                                        }
                                        catch (Exception ex)
                                        {
                                            var asd = row;
                                        }

                                        createBillTask[createBillTaskID] = _totalSheetRow;
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                            }

                        }
                        List<dataBillFile> dataBillFiles = new List<dataBillFile>();
                        dataBillFiles.Add(dataBillFile);
                        DbTBS.BulkInsert(dataBillFiles);
                        DbTBS.BulkInsert(billToProcessList);
                    }

                    try
                    {
                        DbTBS.BulkSaveChanges();
                        createBillTask.Remove(createBillTaskID);
                        return Json(DbTBS.SingleCreateMessage("Saved, file will be processed !"));
                    }
                    catch (Exception ex)
                    {
                        return Json(DbTBS.ErrorMessage(ex.Message));
                    }
                });
                return Json(new { createBillTaskID = createBillTaskID, TotalRecords = _totalRowsCount });


            }
            catch (Exception ex)
            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }

        [Route("TBS/BillManagement/BillOriginalFileDownload/{PK}")]
        public ActionResult BillOriginalFileDownload(Guid PK)
        {
            dataBillFile dataBillFile = (from a in DbTBS.dataBillFile
                                         where a.BillGUID == PK
                                         select a).FirstOrDefault();

            string fileDownloadPath = Server.MapPath(dataBillFile.FilePath);
            string ext = dataBillFile.FilePath.Split('.').Last();
            Guid TelecomCompanyGUID = dataBillFile.dataBill.configTelecomCompanyOperation.codeTelecomCompanyOperation.TelecomCompanyGUID;
            var TelecomCompanyDescription = (from a in DbTBS.codeTelecomCompanyLanguages
                                             where a.Active && a.LanguageID == "EN"
                                             && a.TelecomCompanyGUID == TelecomCompanyGUID
                                             select a.TelecomCompanyDescription).First();
            string fileName = TelecomCompanyDescription + "_" + dataBillFile.dataBill.BillForMonth + "_" + +dataBillFile.dataBill.BillForYear + "." + ext;
            byte[] fileBytes = System.IO.File.ReadAllBytes(@"" + fileDownloadPath + "");

            try
            {
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //[HttpPost, ValidateAntiForgeryToken]
        //public ActionResult CheckBilllFile()
        //{
        //    var file = Request.Files[0];
        //    var stearm = file.InputStream;
        //    using (var package = new ExcelPackage(stearm))
        //    {

        //    }
        //    return Json(new { success = true });
        //}

        private bool ValidateNewBill(DataBillUpdateModel model)
        {
            bool success = true;
            var found = (from a in DbTBS.dataBill
                         where a.Active
                         && a.BillForMonth == model.BillForMonth
                         && a.BillForYear == model.BillForYear
                         && a.BillForTypeGUID == model.BillForTypeGUID
                         && a.TelecomCompanyOperationConfigGUID == model.TelecomCompanyOperationConfigGUID
                         select a).Count();
            if (found > 0)
            {
                ModelState.AddModelError("", "Same bill (month, year and telecom company) already exists");
                success = false;
            }


            return success;

        }

        public ActionResult BillCreateProgres(Guid id)
        {
            return Json(createBillTask.Keys.Where(x => x.ToString() == id.ToString()).Contains(id) ? createBillTask[id] : -100);
        }

        public ActionResult ProcessCalculation()
        {
            return PartialView("~/Areas/TBS/Views/BillManagement/_ProcessCalculation.cshtml");
        }

        private class stiCardUserModel
        {
            public Guid? STIUserGUID { get; set; }
            public Guid? WarehouseGUID { get; set; }
            public string WarehouseDescription { get; set; }
            public string EmailAddress { get; set; }
            public string GSMHolderName { get; set; }
        }

        public class SendEmailModel
        {
            public Guid UserBillGUID { get; set; }
            public string EmailAddress { get; set; }
            public string StaffName { get; set; }
        }

        private static IDictionary<Guid, int> processCalculationTask = new Dictionary<Guid, int>();


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProcessCalculationCreate(ProcessMobileBillsCalculation model)
        {

            lock (this)
            {
                var processCalculationTaskId = Guid.NewGuid();
                processCalculationTask.Add(processCalculationTaskId, 0);
                DateTime ExecutionDate = DateTime.Now;
                List<dataBillToProcess> dataBillToProcess = (from a in DbTBS.dataBillToProcess.Where(x => x.Active && x.IsProcessed == false)
                                                             join b in DbTBS.dataBillFile.Where(x => x.Active) on a.BillFileGUID equals b.BillFileGUID
                                                             join c in DbTBS.dataBill.Where(x => x.Active && x.IsProcessed == false) on b.BillGUID equals c.BillGUID
                                                             where c.DutyStationGUID == model.DutyStationGUID
                                                             && c.BillForTypeGUID == _BillForTypeGUID_Mobile
                                                             && c.TelecomCompanyOperationConfigGUID == model.TelecomCompanyOperationConfigGUID
                                                             //&& a.CallCost > 0
                                                             select a).ToList();
                //there is nothing new to process
                if (dataBillToProcess.Count == 0)
                {
                    return Json(new { NothingToProcess = true }, JsonRequestBehavior.AllowGet);
                }
                Task.Factory.StartNew(() =>
                {

                    List<SendEmailModel> sendEmailsList = new List<SendEmailModel>();
                    DbTBS = new TBSEntities();
                    DbCMS = new Models.CMSEntities();

                    dataBillToProcess = (from a in DbTBS.dataBillToProcess.Where(x => x.Active && x.IsProcessed == false)
                                         join b in DbTBS.dataBillFile.Where(x => x.Active) on a.BillFileGUID equals b.BillFileGUID
                                         join c in DbTBS.dataBill.Where(x => x.Active && x.IsProcessed == false) on b.BillGUID equals c.BillGUID
                                         where c.DutyStationGUID == model.DutyStationGUID
                                          && c.BillForTypeGUID == _BillForTypeGUID_Mobile
                                          && c.TelecomCompanyOperationConfigGUID == model.TelecomCompanyOperationConfigGUID
                                          //&& a.CallCost > 0
                                         select a).ToList();
                    List<v_STISIMCardCustodian> v_STISIMCardCustodian = (from a in DbTBS.v_STISIMCardCustodian
                                                                         where a.ItemType == "Syriatel 2G" || a.ItemType == "MTN 2G"
                                                                         select a).ToList();

                    #region ASD
                    List<dataUserBill> dataUserBills = new List<dataUserBill>();
                    List<dataUserBillDetail> dataUserBillDetails = new List<dataUserBillDetail>();
                    for (int i = 0; i < dataBillToProcess.Count; i++)
                    {

                        Guid _BillGUID = dataBillToProcess[i].dataBillFile.BillGUID;
                        dataBillToProcess[i].dataBillFile.dataBill.IsProcessed = true;
                        dataBillToProcess[i].dataBillFile.dataBill.ProcessedBy = UserGUID;
                        dataBillToProcess[i].dataBillFile.dataBill.ProcessedOn = ExecutionDate;
                        //get the owner of number
                        var _STIUserGUID = (from a in v_STISIMCardCustodian
                                            where dataBillToProcess[i].CallerNumber.EndsWith(a.GSM.Trim())
                                            && a.TakenDate <= dataBillToProcess[i].CallDateTime
                                            && (a.ActualReturenedDate.HasValue ? a.ActualReturenedDate.Value >= dataBillToProcess[i].CallDateTime : true)
                                            && a.UserGUID.HasValue
                                            select new stiCardUserModel
                                            {
                                                STIUserGUID = a.UserGUID,
                                                EmailAddress = a.EmailAddress,
                                                WarehouseGUID = a.WarehouseGUID,
                                                WarehouseDescription = a.WarehouseDescription,
                                                GSMHolderName = a.GSMHolderName
                                            }).FirstOrDefault();
                        if (_STIUserGUID == null || !_STIUserGUID.STIUserGUID.HasValue)
                        {
                            _STIUserGUID = new stiCardUserModel();
                            _STIUserGUID.STIUserGUID = Guid.Empty;
                            _STIUserGUID.WarehouseGUID = Guid.Empty;
                            _STIUserGUID.WarehouseDescription = "NULL";
                            //continue;
                        }



                        //insert new record into dataUserBill
                        //first check if user already exists in dataUserBill for same bill
                        dataUserBill dataUserBillFound = dataUserBills.Where(x => x.UserGUID == _STIUserGUID.STIUserGUID.Value && x.BillGUID == _BillGUID).FirstOrDefault();

                        Guid _UserBillGUID = Guid.NewGuid();

                        //if user is not found -> insert new record into dataUserBill
                        if (dataUserBillFound == null)
                        {
                            dataUserBill dataUserBill = new dataUserBill
                            {
                                BillGUID = _BillGUID,
                                IsConfirmed = false,
                                UserBillGUID = _UserBillGUID,
                                Active = true,
                                UserGUID = _STIUserGUID.STIUserGUID.Value,
                                DoPayInCash = false,
                                PayInCashAmount = 0,
                                DeductFromSalaryAmount = 0,
                                IsFirstOpen = true,
                            };
                            dataUserBills.Add(dataUserBill);
                            ////AMER ADDED TO CHECK PERFORMANCE ON BATCH
                            //DbTBS.dataUserBill.Add(dataUserBill);
                            ////AMER ADDED TO CHECK PERFORMANCE ON BATCH


                        }
                        //if user is found -> do nothing, just get the UserBillGUID
                        else
                        {
                            _UserBillGUID = dataUserBillFound.UserBillGUID;
                        }
                        //process the bill details
                        dataUserBillDetail dataUserBillDetail = new dataUserBillDetail();
                        dataUserBillDetail.UserBillDetailGUID = Guid.NewGuid();
                        dataUserBillDetail.SIMWarehouseGUID = _STIUserGUID.WarehouseGUID.Value;
                        dataUserBillDetail.SIMWarehouseDescription = _STIUserGUID.WarehouseDescription;
                        dataUserBillDetail.CallingNumber = dataBillToProcess[i].CallerNumber;
                        dataUserBillDetail.CalledNumber = dataBillToProcess[i].ReceiverNumber;
                        dataUserBillDetail.CallCost = dataBillToProcess[i].CallCost;
                        dataUserBillDetail.CallType = dataBillToProcess[i].DestinationType;
                        dataUserBillDetail.Service = dataBillToProcess[i].DestinationType;
                        dataUserBillDetail.dateTimeConnect = dataBillToProcess[i].CallDateTime;
                        dataUserBillDetail.DurationInMinutes = dataBillToProcess[i].CallDurationInMinutes;
                        dataUserBillDetail.DurationInSeconds = dataBillToProcess[i].CallDurationInMinutes * 60;
                        dataUserBillDetail.UserBillGUID = _UserBillGUID;
                        dataUserBillDetail.Active = true;
                        dataUserBillDetail.IsConfirmed = false;
                        dataUserBillDetail.BillToProcessGUID = dataBillToProcess[i].BillToProcessGUID;
                        //check for previous history if we have to force pick private or not
                        //bool isPrivate = (from a in DbTBS.dataStaffPrivateCall.Where(x => x.Active)
                        //                  where a.DestinationNumber.EndsWith(dataUserBillDetail.CalledNumber)
                        //                  select a).Count() > 0;
                        dataUserBillDetail.IsPrivate = false;

                        //if (true)
                        //{
                        //    var isPreviouslyOfficial = (from a in DbTBS.dataUserBillDetail.AsNoTracking()
                        //                                join b in DbTBS.dataUserBill on a.UserBillGUID equals b.UserBillGUID
                        //                                where dataUserBillDetail.CalledNumber.EndsWith(a.CalledNumber)
                        //                                && b.UserGUID == _STIUserGUID.STIUserGUID
                        //                                orderby b.ConfirmationDate.Value descending
                        //                                select a.IsPrivate == false).FirstOrDefault();
                        //    if (isPreviouslyOfficial)
                        //    {
                        //        dataUserBillDetail.IsPrivate = true;
                        //        dataUserBillDetail.IsConfirmed = true;
                        //    }

                        //}

                        //get destination person name if exists
                        var _destinationUser = (from a in v_STISIMCardCustodian
                                                where dataBillToProcess[i].ReceiverNumber.EndsWith(a.GSM) && a.TakenDate <= dataBillToProcess[i].CallDateTime
                                                && (a.ActualReturenedDate.HasValue ? a.ActualReturenedDate.Value >= dataBillToProcess[i].CallDateTime : true)
                                                && a.UserGUID.HasValue
                                                select a.GSMHolderName).FirstOrDefault();
                        if (_destinationUser != null)
                        {
                            dataUserBillDetail.CalledStaffName = _destinationUser.ToString();
                        }

                        dataUserBillDetails.Add(dataUserBillDetail);

                        ////AMER ADDED TO CHECK PERFORMANCE ON BATCH
                        //DbTBS.dataUserBillDetail.Add(dataUserBillDetail);
                        ////AMER ADDED TO CHECK PERFORMANCE ON BATCH
                        //set the record to processed
                        dataBillToProcess[i].IsProcessed = true;
                        if (_STIUserGUID.STIUserGUID == Guid.Empty)
                        {
                            continue;
                        }

                        if (sendEmailsList.Where(x => x.EmailAddress == _STIUserGUID.EmailAddress).Count() == 0)
                        {
                            sendEmailsList.Add(new SendEmailModel
                            {
                                EmailAddress = _STIUserGUID.EmailAddress,
                                StaffName = _STIUserGUID.GSMHolderName,
                                UserBillGUID = _UserBillGUID,

                            });
                        }


                        processCalculationTask[processCalculationTaskId] = i + 1;

                    }

                    processCalculationTask[processCalculationTaskId] = dataBillToProcess.Count;

                    try
                    {
                        //DateTime ExecutionDate = DateTime.Now;
                        DbTBS.BulkInsert(dataUserBills);
                        DbTBS.BulkInsert(dataUserBillDetails);
                        DbTBS.BulkSaveChanges();
                        DbTBS.SaveChanges();
                        ////AMER REMOVED TO CHECK PERFORMANCE ON BATCH
                        ////////////////////////////////////////DbTBS.dataUserBill.AddRange(dataUserBills);
                        ////////////////////////////////////////DbTBS.dataUserBillDetail.AddRange(dataUserBillDetails);
                        ////AMER REMOVED TO CHECK PERFORMANCE ON BATCH
                        ///

                        // 2. CALL DetectChanges before SaveChanges
                        //DbTBS.ChangeTracker.DetectChanges();

                        //DbTBS.SaveChanges();


                        #region Sending Emails
                        dataBill _dataBill = dataBillToProcess.First().dataBillFile.dataBill;
                        string BillDeadLine = _dataBill.BillDeadLine.ToShortDateString();
                        int BillMonthNumber = _dataBill.BillForMonth;
                        string BillMonthName = new System.Globalization.DateTimeFormatInfo().GetMonthName(BillMonthNumber);
                        string BillYear = _dataBill.BillForYear.ToString();

                        string TelecomCompanyName = (from a in DbTBS.configTelecomCompanyOperation
                                                     join b in DbTBS.codeTelecomCompanyOperation on a.TelecomCompanyOperationGUID equals b.TelecomCompanyOperationGUID
                                                     join c in DbTBS.codeTelecomCompanyLanguages on b.TelecomCompanyGUID equals c.TelecomCompanyGUID
                                                     where c.LanguageID == "EN"
                                                     && a.TelecomCompanyOperationConfigGUID == _dataBill.TelecomCompanyOperationConfigGUID
                                                     select c.TelecomCompanyDescription).FirstOrDefault();

                        string subject = "DO NOT reply | SYRIA Telephony Billing System - " + TelecomCompanyName + " Mobile Bills --" + BillMonthName + "/" + BillYear;
                        foreach (SendEmailModel item in sendEmailsList)
                        {

                            string URL = AppSettingsKeys.Domain + "/TBS/UserBills/Update/" + item.UserBillGUID;
                            string Anchor = "<a href='" + URL + "' target='_blank'>Here</a>";
                            string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                            string UserGuidURL = "https://unhcr365.sharepoint.com/teams/mena-syr-ICTUnit/Shared%20Documents/ICT%20Unit/Documentation/Guides%20User/Telephone%20Bills_V2_Usermanual.pdf";
                            string UserGuidAnchor = "<a href='" + UserGuidURL + "' target='_blank'>User Guide</a>";
                            string body_format = "HTML";
                            string importance = "Normal";

                            string Message = resxEmails.TBSBillReadyEmail
                                .Replace("$FullName", item.StaffName)
                                .Replace("$BillType", TelecomCompanyName + " mobile")
                                .Replace("$BillMonthYear", BillMonthName + "/" + BillYear)
                                .Replace("$Deadline", BillDeadLine)
                                .Replace("$IdentifyBillLinkIn", Anchor)
                                .Replace("$UserGuidLink", UserGuidAnchor);

                            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";

                            //DbCMS.SendEmail("karkoush@unhcr.org", "karkoush@unhcr.org", "karkoush@unhcr.org", subject, _body, body_format, importance, null, "Telephony Billing System <syrdait@unhcr.org>");
                            DbCMS.SendEmail(item.EmailAddress, "billing@unhcr.org", "karkoush@unhcr.org", subject, _body, body_format, importance, null, "Telephony Billing System <billing@unhcr.org>");

                            //if (item.EmailAddress.Trim().ToUpper() == "karkoush@unhcr.org".ToUpper())
                            //{


                            //}


                        }

                        #endregion
                        //
                        processCalculationTask.Remove(processCalculationTaskId);
                        //DbTBS.CreateBulk(dataUserBills, Permissions.BillsManagement.CreateGuid, ExecutionDate, DbCMS);
                        //DbTBS.CreateBulk(dataUserBillDetails, Permissions.BillsManagement.CreateGuid, ExecutionDate, DbCMS);
                        //DbTBS.SaveChanges();
                        //DbCMS.SaveChanges();

                        return Json(DbTBS.SingleCreateMessage("Saved, file will be processed !"));
                    }
                    catch (Exception ex)
                    {
                        return Json(DbTBS.ErrorMessage(ex.Message));
                    }

                    #endregion

                });

                return Json(new { processCalculationTaskId = processCalculationTaskId, TotalRecords = dataBillToProcess.Count });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProcessCalculationCreateNotXXXXXXXXX(ProcessMobileBillsCalculation model)
        {

            var processCalculationTaskId = Guid.NewGuid();
            processCalculationTask.Add(processCalculationTaskId, 0);
            DateTime ExecutionDate = DateTime.Now;
            List<dataBillToProcess> dataBillToProcess = (from a in DbTBS.dataBillToProcess.Where(x => x.Active && x.IsProcessed == false)
                                                         join b in DbTBS.dataBillFile.Where(x => x.Active) on a.BillFileGUID equals b.BillFileGUID
                                                         join c in DbTBS.dataBill.Where(x => x.Active && x.IsProcessed == false) on b.BillGUID equals c.BillGUID
                                                         where c.DutyStationGUID == model.DutyStationGUID
                                                         && c.BillForTypeGUID == _BillForTypeGUID_Mobile
                                                         && c.TelecomCompanyOperationConfigGUID == model.TelecomCompanyOperationConfigGUID
                                                         && a.CallCost > 0
                                                         select a).ToList();
            //there is nothing new to process
            if (dataBillToProcess.Count == 0)
            {
                return Json(new { NothingToProcess = true }, JsonRequestBehavior.AllowGet);
            }
            Task.Factory.StartNew(() =>
            {

                List<SendEmailModel> sendEmailsList = new List<SendEmailModel>();
                DbTBS = new TBSEntities();
                DbCMS = new Models.CMSEntities();

                dataBillToProcess = (from a in DbTBS.dataBillToProcess.Where(x => x.Active && x.IsProcessed == false)
                                     join b in DbTBS.dataBillFile.Where(x => x.Active) on a.BillFileGUID equals b.BillFileGUID
                                     join c in DbTBS.dataBill.Where(x => x.Active && x.IsProcessed == false) on b.BillGUID equals c.BillGUID
                                     where c.DutyStationGUID == model.DutyStationGUID
                                      && c.BillForTypeGUID == _BillForTypeGUID_Mobile
                                      && c.TelecomCompanyOperationConfigGUID == model.TelecomCompanyOperationConfigGUID
                                      && a.CallCost > 0
                                     select a).ToList();
                List<v_STISIMCardCustodian> v_STISIMCardCustodian = (from a in DbTBS.v_STISIMCardCustodian
                                                                     where a.ItemType == "Syriatel 2G" || a.ItemType == "MTN 2G"
                                                                     select a).ToList();

                List<v_LatestUserCalls> v_LatestUserCalls = (from a in DbTBS.v_LatestUserCalls.AsNoTracking()
                                                             select a).ToList();
                #region ASD
                List<dataUserBill> dataUserBills = new List<dataUserBill>();
                List<dataUserBillDetail> dataUserBillDetails = new List<dataUserBillDetail>();
                for (int i = 0; i < dataBillToProcess.Count; i++)
                {

                    Guid _BillGUID = dataBillToProcess[i].dataBillFile.BillGUID;
                    dataBillToProcess[i].dataBillFile.dataBill.IsProcessed = true;
                    dataBillToProcess[i].dataBillFile.dataBill.ProcessedBy = UserGUID;
                    dataBillToProcess[i].dataBillFile.dataBill.ProcessedOn = ExecutionDate;
                    //get the owner of number
                    var _STIUserGUID = (from a in v_STISIMCardCustodian
                                        where dataBillToProcess[i].CallerNumber.EndsWith(a.GSM.Trim())
                                        && a.TakenDate <= dataBillToProcess[i].CallDateTime
                                        && (a.ActualReturenedDate.HasValue ? a.ActualReturenedDate.Value >= dataBillToProcess[i].CallDateTime : true)
                                        && a.UserGUID.HasValue
                                        select new stiCardUserModel
                                        {
                                            STIUserGUID = a.UserGUID,
                                            EmailAddress = a.EmailAddress,
                                            WarehouseGUID = a.WarehouseGUID,
                                            WarehouseDescription = a.WarehouseDescription,
                                            GSMHolderName = a.GSMHolderName
                                        }).FirstOrDefault();
                    if (_STIUserGUID == null || !_STIUserGUID.STIUserGUID.HasValue)
                    {
                        _STIUserGUID = new stiCardUserModel();
                        _STIUserGUID.STIUserGUID = Guid.Empty;
                        _STIUserGUID.WarehouseGUID = Guid.Empty;
                        _STIUserGUID.WarehouseDescription = "NULL";
                        //continue;
                    }



                    //insert new record into dataUserBill
                    //first check if user already exists in dataUserBill for same bill
                    dataUserBill dataUserBillFound = dataUserBills.Where(x => x.UserGUID == _STIUserGUID.STIUserGUID.Value && x.BillGUID == _BillGUID).FirstOrDefault();

                    Guid _UserBillGUID = Guid.NewGuid();

                    //if user is not found -> insert new record into dataUserBill
                    if (dataUserBillFound == null)
                    {
                        dataUserBill dataUserBill = new dataUserBill
                        {
                            BillGUID = _BillGUID,
                            IsConfirmed = false,
                            UserBillGUID = _UserBillGUID,
                            Active = true,
                            UserGUID = _STIUserGUID.STIUserGUID.Value,
                            DoPayInCash = false,
                            PayInCashAmount = 0,
                            DeductFromSalaryAmount = 0
                        };
                        dataUserBills.Add(dataUserBill);
                        ////AMER ADDED TO CHECK PERFORMANCE ON BATCH
                        //DbTBS.dataUserBill.Add(dataUserBill);
                        ////AMER ADDED TO CHECK PERFORMANCE ON BATCH


                    }
                    //if user is found -> do nothing, just get the UserBillGUID
                    else
                    {
                        _UserBillGUID = dataUserBillFound.UserBillGUID;
                    }
                    //process the bill details
                    dataUserBillDetail dataUserBillDetail = new dataUserBillDetail();
                    dataUserBillDetail.UserBillDetailGUID = Guid.NewGuid();
                    dataUserBillDetail.SIMWarehouseGUID = _STIUserGUID.WarehouseGUID.Value;
                    dataUserBillDetail.SIMWarehouseDescription = _STIUserGUID.WarehouseDescription;
                    dataUserBillDetail.CallingNumber = dataBillToProcess[i].CallerNumber;
                    dataUserBillDetail.CalledNumber = dataBillToProcess[i].ReceiverNumber;
                    dataUserBillDetail.CallCost = dataBillToProcess[i].CallCost;
                    dataUserBillDetail.CallType = dataBillToProcess[i].DestinationType;
                    dataUserBillDetail.Service = dataBillToProcess[i].DestinationType;
                    dataUserBillDetail.dateTimeConnect = dataBillToProcess[i].CallDateTime;
                    dataUserBillDetail.DurationInMinutes = dataBillToProcess[i].CallDurationInMinutes;
                    dataUserBillDetail.DurationInSeconds = dataBillToProcess[i].CallDurationInMinutes * 60;
                    dataUserBillDetail.UserBillGUID = _UserBillGUID;
                    dataUserBillDetail.Active = true;
                    dataUserBillDetail.IsConfirmed = false;
                    dataUserBillDetail.BillToProcessGUID = dataBillToProcess[i].BillToProcessGUID;
                    //check for previous history if we have to force pick private or not
                    //bool isPrivate = (from a in DbTBS.dataStaffPrivateCall.Where(x => x.Active)
                    //                  where a.DestinationNumber.EndsWith(dataUserBillDetail.CalledNumber)
                    //                  select a).Count() > 0;
                    dataUserBillDetail.IsPrivate = false;

                    if (false)
                    {
                        var isPreviouslyOfficial = (from a in v_LatestUserCalls
                                                    where dataUserBillDetail.CalledNumber.EndsWith(a.CalledNumber)
                                                    && a.UserGUID == _STIUserGUID.STIUserGUID
                                                    select a.IsPrivate == false).FirstOrDefault();
                        if (isPreviouslyOfficial)
                        {
                            dataUserBillDetail.IsPrivate = false;
                            dataUserBillDetail.IsConfirmed = true;
                        }

                    }

                    //get destination person name if exists
                    var _destinationUser = (from a in v_STISIMCardCustodian
                                            where dataBillToProcess[i].ReceiverNumber.EndsWith(a.GSM) && a.TakenDate <= dataBillToProcess[i].CallDateTime
                                            && (a.ActualReturenedDate.HasValue ? a.ActualReturenedDate.Value >= dataBillToProcess[i].CallDateTime : true)
                                            && a.UserGUID.HasValue
                                            select a.GSMHolderName).FirstOrDefault();
                    if (_destinationUser != null)
                    {
                        dataUserBillDetail.CalledStaffName = _destinationUser.ToString();
                    }

                    dataUserBillDetails.Add(dataUserBillDetail);

                    ////AMER ADDED TO CHECK PERFORMANCE ON BATCH
                    //DbTBS.dataUserBillDetail.Add(dataUserBillDetail);
                    ////AMER ADDED TO CHECK PERFORMANCE ON BATCH
                    //set the record to processed
                    dataBillToProcess[i].IsProcessed = true;
                    if (_STIUserGUID.STIUserGUID == Guid.Empty)
                    {
                        continue;
                    }

                    if (sendEmailsList.Where(x => x.EmailAddress == _STIUserGUID.EmailAddress).Count() == 0)
                    {
                        sendEmailsList.Add(new SendEmailModel
                        {
                            EmailAddress = _STIUserGUID.EmailAddress,
                            StaffName = _STIUserGUID.GSMHolderName,
                            UserBillGUID = _UserBillGUID,

                        });
                    }


                    processCalculationTask[processCalculationTaskId] = i + 1;

                }


                var distinctsLists = (from a in dataUserBills
                                      join b in dataUserBillDetails on a.UserBillGUID equals b.UserBillGUID
                                      select new
                                      {
                                          a.UserGUID,
                                          b.CallingNumber,
                                          b.CalledNumber
                                      }).Distinct().ToList();

                var asdasd = (from a in DbTBS.v_LatestUserCalls
                              where distinctsLists.Select(x => x.UserGUID).Contains(a.UserGUID)
                              && distinctsLists.Select(x => x.CallingNumber).Contains(a.CallingNumber)
                              && distinctsLists.Select(x => x.CalledNumber).Contains(a.CalledNumber)
                              select a).ToList();


                processCalculationTask[processCalculationTaskId] = dataBillToProcess.Count;

                try
                {
                    //DateTime ExecutionDate = DateTime.Now;
                    DbTBS.BulkInsert(dataUserBills);
                    DbTBS.BulkInsert(dataUserBillDetails);
                    DbTBS.BulkSaveChanges();
                    DbTBS.SaveChanges();
                    ////AMER REMOVED TO CHECK PERFORMANCE ON BATCH
                    ////////////////////////////////////////DbTBS.dataUserBill.AddRange(dataUserBills);
                    ////////////////////////////////////////DbTBS.dataUserBillDetail.AddRange(dataUserBillDetails);
                    ////AMER REMOVED TO CHECK PERFORMANCE ON BATCH
                    ///

                    // 2. CALL DetectChanges before SaveChanges
                    //DbTBS.ChangeTracker.DetectChanges();

                    //DbTBS.SaveChanges();


                    #region Sending Emails
                    dataBill _dataBill = dataBillToProcess.First().dataBillFile.dataBill;
                    string BillDeadLine = _dataBill.BillDeadLine.ToShortDateString();
                    int BillMonthNumber = _dataBill.BillForMonth;
                    string BillMonthName = new System.Globalization.DateTimeFormatInfo().GetMonthName(BillMonthNumber);
                    string BillYear = _dataBill.BillForYear.ToString();

                    string TelecomCompanyName = (from a in DbTBS.configTelecomCompanyOperation
                                                 join b in DbTBS.codeTelecomCompanyOperation on a.TelecomCompanyOperationGUID equals b.TelecomCompanyOperationGUID
                                                 join c in DbTBS.codeTelecomCompanyLanguages on b.TelecomCompanyGUID equals c.TelecomCompanyGUID
                                                 where c.LanguageID == "EN"
                                                 && a.TelecomCompanyOperationConfigGUID == _dataBill.TelecomCompanyOperationConfigGUID
                                                 select c.TelecomCompanyDescription).FirstOrDefault();

                    string subject = "DO NOT reply | SYRIA Telephony Billing System - Mobile Bills For " + TelecomCompanyName + " " + BillMonthName + " " + BillYear;
                    foreach (SendEmailModel item in sendEmailsList)
                    {
                        //if (item.EmailAddress.Trim().ToUpper() == "karkoush@unhcr.org".ToUpper() || item.EmailAddress.Trim().ToUpper() == "isac@unhcr.org".ToUpper() || item.EmailAddress.Trim().ToUpper() == "alfazzaa@unhcr.org".ToUpper())
                        //{

                        //    string URL = AppSettingsKeys.Domain + "/TBS/UserBills/Update/" + item.UserBillGUID;
                        //    string Anchor = "<a href='" + URL + "' target='_blank'>Here</a>";
                        //    string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                        //    string UserGuidURL = AppSettingsKeys.Domain + "/TBS/UserBills/DownloadUserGuide";
                        //    string UserGuidAnchor = "<a href='" + UserGuidURL + "' target='_blank'>User Guide</a>";
                        //    string body_format = "HTML";
                        //    string importance = "Normal";

                        //    string Message = resxEmails.TBSBillReadyEmail
                        //        .Replace("$FullName", item.StaffName)
                        //        .Replace("$BillType", TelecomCompanyName + " mobile")
                        //        .Replace("$BillMonthYear", BillMonthName + " " + BillYear)
                        //        .Replace("$Deadline", BillDeadLine)
                        //        .Replace("$IdentifyBillLinkIn", Anchor)
                        //        .Replace("$UserGuidLink", UserGuidAnchor);

                        //    string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";

                        //    DbCMS.SendEmail(item.EmailAddress, null, null, subject, _body, body_format, importance, null, "Telephony Billing System");
                        //}


                        //  if (item.EmailAddress.Trim().ToUpper() == "karkoush@unhcr.org".ToUpper()
                        //|| item.EmailAddress.Trim().ToUpper() == "isac@unhcr.org".ToUpper()
                        //|| item.EmailAddress.Trim().ToUpper() == "alfazzaa@unhcr.org".ToUpper()
                        //|| item.EmailAddress.Trim().ToUpper() == "shaban@unhcr.org".ToUpper()
                        //|| item.EmailAddress.Trim().ToUpper() == "maksoud@unhcr.org".ToUpper()
                        //|| item.EmailAddress.Trim().ToUpper() == "karkoush@unhcr.org".ToUpper()
                        //|| item.EmailAddress.Trim().ToUpper() == "sahhar@unhcr.org".ToUpper()
                        //|| item.EmailAddress.Trim().ToUpper() == "ASHOURF@unhcr.org".ToUpper()
                        //|| item.EmailAddress.Trim().ToUpper() == "AYDI@unhcr.org".ToUpper()
                        //|| item.EmailAddress.Trim().ToUpper() == "daqaq@unhcr.org".ToUpper()
                        //|| item.EmailAddress.Trim().ToUpper() == "shaglil@unhcr.org".ToUpper()
                        //|| item.EmailAddress.Trim().ToUpper() == "rostom@unhcr.org".ToUpper()
                        //|| item.EmailAddress.Trim().ToUpper() == "HOMSSI@unhcr.org".ToUpper()
                        //|| item.EmailAddress.Trim().ToUpper() == "alkady@unhcr.org".ToUpper()
                        //|| item.EmailAddress.Trim().ToUpper() == "ibrahiki@unhcr.org".ToUpper()
                        //|| item.EmailAddress.Trim().ToUpper() == "arraj@unhcr.org".ToUpper()
                        //|| item.EmailAddress.Trim().ToUpper() == "kutienim@unhcr.org".ToUpper()
                        //|| item.EmailAddress.Trim().ToUpper() == "NASSERMO@unhcr.org".ToUpper())
                        if (item.EmailAddress.Trim().ToUpper() == "karkoush@unhcr.org".ToUpper())
                        {

                            string URL = AppSettingsKeys.Domain + "/TBS/UserBills/Update/" + item.UserBillGUID;
                            string Anchor = "<a href='" + URL + "' target='_blank'>Here</a>";
                            string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                            string UserGuidURL = AppSettingsKeys.Domain + "/TBS/UserBills/DownloadUserGuide";
                            string UserGuidAnchor = "<a href='" + UserGuidURL + "' target='_blank'>User Guide</a>";
                            string body_format = "HTML";
                            string importance = "Normal";

                            string Message = resxEmails.TBSBillReadyEmail
                                .Replace("$FullName", item.StaffName)
                                .Replace("$BillType", TelecomCompanyName + " mobile")
                                .Replace("$BillMonthYear", BillMonthName + " " + BillYear)
                                .Replace("$Deadline", BillDeadLine)
                                .Replace("$IdentifyBillLinkIn", Anchor)
                                .Replace("$UserGuidLink", UserGuidAnchor);

                            string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";

                            DbCMS.SendEmail(item.EmailAddress, null, null, subject, _body, body_format, importance, null, "Telephony Billing System");
                        }


                    }

                    #endregion
                    //
                    processCalculationTask.Remove(processCalculationTaskId);
                    //DbTBS.CreateBulk(dataUserBills, Permissions.BillsManagement.CreateGuid, ExecutionDate, DbCMS);
                    //DbTBS.CreateBulk(dataUserBillDetails, Permissions.BillsManagement.CreateGuid, ExecutionDate, DbCMS);
                    //DbTBS.SaveChanges();
                    //DbCMS.SaveChanges();

                    return Json(DbTBS.SingleCreateMessage("Saved, file will be processed !"));
                }
                catch (Exception ex)
                {
                    return Json(DbTBS.ErrorMessage(ex.Message));
                }

                #endregion

            });

            return Json(new { processCalculationTaskId = processCalculationTaskId, TotalRecords = dataBillToProcess.Count });
        }


        public ActionResult ProcessCalculationProgres(Guid id)
        {
            return Json(processCalculationTask.Keys.Where(x => x.ToString() == id.ToString()).Contains(id) ? processCalculationTask[id] : -100);
        }

        public class testtest
        {
            public string callingPartyNumber { get; set; }
            public string originalCalledPartyNumber { get; set; }
            public string finalCalledPartyNumber { get; set; }
            public string dateTimeConnect { get; set; }
            public string dateTimeDisconnect { get; set; }
            public string originalCalledPartyNumberPartition { get; set; }
            public string finalCalledPartyNumberPartition { get; set; }
            public string duration { get; set; }
            public string authCodeDescription { get; set; }
        }

        public class CSVReaderHelper
        {
            public static DataTable GetCSVData(string localDestination)
            {
                //Instantiating Data Table
                var dt = new DataTable();

                try
                {
                    if (System.IO.File.Exists(localDestination))
                    {
                        using (StreamReader streamReader = new StreamReader(localDestination))
                        {
                            string[] headers = streamReader.ReadLine().Split(',');

                            foreach (string header in headers)
                            {
                                dt.Columns.Add(header);
                            }

                            while (!streamReader.EndOfStream)
                            {
                                string[] rows = streamReader.ReadLine().Split(',');

                                if (rows.Length > 1)
                                {
                                    DataRow dr = dt.NewRow();

                                    for (int i = 0; i < headers.Length; i++)
                                    {
                                        dr[i] = rows[i].Trim();
                                    }

                                    dt.Rows.Add(dr);
                                }

                            }
                        }
                    }

                    return dt;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }


        //SEND EMAIL
        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                                                                         viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                             ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }


        public ActionResult GetPendingToProcessByTelecomCompany(Guid TelecomCompanyOperationConfigGUID)
        {
            List<PendingBillsToProcessModel> model = (from a in DbTBS.dataBill.Where(x => x.Active && x.TelecomCompanyOperationConfigGUID == TelecomCompanyOperationConfigGUID && x.IsProcessed == false)
                                                      select new PendingBillsToProcessModel
                                                      {
                                                          BillGUID = a.BillGUID,
                                                          BillForMonth = a.BillForMonth,
                                                          BillForYear = a.BillForYear
                                                      }).ToList();

            return PartialView("~/Areas/TBS/Views/BillManagement/_PendingBillsToProcess.cshtml", model);
        }


        public JsonResult GetProgresBarValue()
        {
            int TBSMobileProcessCurrnetValue = 0;
            int TBSMobileProcessTotalValue = 0;
            try
            {
                TBSMobileProcessCurrnetValue = Convert.ToInt32(Session["TBSMobileProcessCurrnetValue"].ToString());
                TBSMobileProcessTotalValue = Convert.ToInt32(Session["TBSMobileProcessTotalValue"].ToString());

            }
            catch { }
            return Json(new { TBSMobileProcessCurrnetValue = TBSMobileProcessCurrnetValue, TBSMobileProcessTotalValue = TBSMobileProcessTotalValue }, JsonRequestBehavior.AllowGet);
        }



        #region Process LandLine Bills

        [Route("TBS/BillManagement/LandlineBills")]
        public ActionResult LandlineBills()
        {
            return View("~/Areas/TBS/Views/LandlineBills/Index.cshtml");
        }

        [Route("TBS/BillManagement/DataLandLinsBillsDataTable")]
        public JsonResult LandlineBillsDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<DataLandLinsBillsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<DataLandLinsBillsDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbTBS.v_BillsDataTable.AsExpandable()
                       where a.BillTypeLanguageID == LAN
                       && a.TelecomCompanyLanguageID == LAN
                       && a.OperationLanguageID == LAN
                       && a.BillForTypeGUID.ToString() == "B2980068-688D-428D-9E5C-494656BA9D2C"
                       select new DataLandLinsBillsDataTableModel
                       {
                           BillGUID = a.BillGUID,
                           BillForTypeDescription = a.ValueDescription,
                           TelecomCompanyGUID = a.TelecomCompanyGUID,
                           TelecomCompanyDescription = a.TelecomCompanyDescription,
                           OperationGUID = a.OperationGUID,
                           OperationDescription = a.OperationDescription,
                           BillForMonth = a.BillForMonth,
                           BillForYear = a.BillForYear
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<DataLandLinsBillsDataTableModel> Result = Mapper.Map<List<DataLandLinsBillsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }


        #endregion


        #region Temp Progres Bar
        private static IDictionary<Guid, int> tasks = new Dictionary<Guid, int>();

        public ActionResult Start()
        {
            var taskId = Guid.NewGuid();
            tasks.Add(taskId, 0);

            Task.Factory.StartNew(() =>
            {
                for (var i = 0; i <= 100; i++)
                {
                    tasks[taskId] = i; // update task progress
                    Thread.Sleep(50); // simulate long running operation
                }
                tasks.Remove(taskId);
            });

            return Json(taskId);
        }

        public ActionResult Progress(Guid id)
        {
            return Json(tasks.Keys.Contains(id) ? tasks[id] : 100);
        }
        #endregion
    }
}


#region trash
//                              else if (BillForTypeGUID == _BillForTypeGUID_Land) //land
//                    {

//                        //process the bill details
//                        dataUserBillDetail dataUserBillDetail = new dataUserBillDetail();
//dataUserBillDetail.UserBillDetailGUID = Guid.NewGuid();
//                        dataUserBillDetail.CallingNumber = dataCall.CallerNumber;
//                        dataUserBillDetail.CalledNumber = dataCall.ReceiverNumber;

//                        ////calculate cost////
//                        //four Number Free cost
//                        Match caseCost1 = Regex.Match(dataCall.ReceiverNumber, @"\d{4}");
//                        if (caseCost1.Success && dataCall.ReceiverNumber.Length == 4)
//                        {
//                            dataUserBillDetail.CallCost = 0;
//                            dataUserBillDetail.DestinationCountryGUID = SyriaCountryGUID;
//                            dataUserBillDetail.CallType = MobileCallTypes.Local;
//                        }

//                        //ten number cost
//                        //1- land local
//                        Match caseCost2 = Regex.Match(dataCall.ReceiverNumber, @"\d{7}");
//                        if (caseCost2.Success && dataCall.ReceiverNumber.Length == 7)
//                        {
//                            //double min = dataCall.CallDurationInMinutes / 3;
//                            //int minutes = Convert.ToInt32(Math.Ceiling(min));
//                            dataUserBillDetail.CallCost = dataCall.CallDurationInMinutes* 0.33;
//                            dataUserBillDetail.DestinationCountryGUID = SyriaCountryGUID;
//                            dataUserBillDetail.CallType = MobileCallTypes.Local;
//                        }
//                        //2- land country
//                        Match caseCost3 = Regex.Match(dataCall.ReceiverNumber, @"0\d{9}");
//                        if (caseCost3.Success && dataCall.ReceiverNumber.Length == 10)
//                        {
//                            dataUserBillDetail.CallCost = dataCall.CallDurationInMinutes* 3;
//                            dataUserBillDetail.DestinationCountryGUID = SyriaCountryGUID;
//                            dataUserBillDetail.CallType = MobileCallTypes.CountryWide;
//                        }
//                        //3- mobile
//                        Match caseCost4 = Regex.Match(dataCall.ReceiverNumber, @"09\d{8}");
//                        if (caseCost4.Success && dataCall.ReceiverNumber.Length == 10)
//                        {
//                            dataUserBillDetail.CallCost = dataCall.CallDurationInMinutes* 14;
//                            dataUserBillDetail.DestinationCountryGUID = SyriaCountryGUID;
//                            dataUserBillDetail.CallType = MobileCallTypes.MobileLine;
//                            //dc.CodeCountry = "SYR";
//                        }
//                        //twelve Number cost
//                        Match caseCost5 = Regex.Match(dataCall.ReceiverNumber, @"\d{12}");
//                        if (caseCost5.Success && dataCall.ReceiverNumber.Length >= 11)
//                        {
//                            string zipcode = dataCall.ReceiverNumber.Substring(0, 3);
//var country = (from a in codeCallCosts where a.DestinationCountryCode.ToString() == zipcode select a).FirstOrDefault();
//dataUserBillDetail.CallCost = dataCall.CallDurationInMinutes* country.CallCost;
//dataUserBillDetail.DestinationCountryGUID = country.DestinationCountryGUID;
//                            dataUserBillDetail.CallType = MobileCallTypes.International;
//                        }
//                        ////calculate cost////



//                        dataUserBillDetail.CallType = dataCall.DestinationType;
//                        dataUserBillDetail.Service = dataCall.DestinationType;
//                        dataUserBillDetail.dateTimeConnect = dataCall.CallDateTime;
//                        dataUserBillDetail.DurationInMinutes = dataCall.CallDurationInMinutes;
//                        dataUserBillDetail.DurationInSeconds = dataCall.CallDurationInMinutes* 60;
//                        dataUserBillDetail.UserBillGUID = _UserBillGUID;
//                        dataUserBillDetail.Active = true;
//                        dataUserBillDetail.IsPrivate = dataCall.IsPrivate;

//                        //set the record to processed
//                        dataCall.IsProcessed = true;

//                    }
#endregion