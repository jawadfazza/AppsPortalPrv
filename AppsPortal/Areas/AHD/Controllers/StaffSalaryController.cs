using AHD_DAL.Model;
using AHD_DAL.ViewModels;
using AppsPortal.Areas.AHD.Service;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using FineUploader;
using LinqKit;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
namespace AppsPortal.Areas.AHD.Controllers
{
    public class StaffSalaryController : AHDBaseController
    {
        // GET: AHD/StaffSalary

        #region Salary Cycle Period

        [Route("AHD/SalaryCyclePeriodIndex/")]
        public ActionResult SalaryCyclePeriodIndex()
        {
            if (!CMS.HasAction(Permissions.SalaryCycle.Access, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return View("~/Areas/AHD/Views/SalaryCyclePeriod/Index.cshtml");
        }
        [Route("AHD/SalaryCyclePeriodDataTable/")]
        public JsonResult SalaryCyclePeriodDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<SalaryCyclePeriodDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<SalaryCyclePeriodDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.SalaryCycle.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix

            var All = (
                from a in DbAHD.dataSalaryCycle.Where(x => x.Active).AsExpandable()
                join b in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.LastFlowStatusGUID equals b.ValueGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.CreateByGUID equals c.UserGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()
                select new SalaryCyclePeriodDataTableModel
                {
                    SalaryCycleGUID = a.SalaryCycleGUID.ToString(),
                    Year = a.Year,
                    MonthName = a.MonthName,
                    Active = a.Active,
                    CycleName = a.CycleName,
                    CreateDate = a.CreateDate,
                    CreateByGUID = a.CreateByGUID.ToString(),
                    LastFlowStatusGUID = a.LastFlowStatusGUID.ToString(),
                    LastFlowName = R1.ValueDescription,
                    CreatedBy = R2.FirstName + " " + R2.Surname,
                    //FlowStatusGUID = a.FlowStatusGUID,
                    //PaymentDurationName = a.PaymentDurationName,
                    //TotalStaffConfirm = a.dataSalaryCyclePeriod1.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Confirmed).Count(),
                    //TotalStaffNotConfirm = a.dataSalaryCyclePeriod1.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Pending).Count(),

                    //OrderId = a.OrderId,
                    //CreateDate = a.CreateDate,
                    dataSalaryCycleRowVersion = a.dataSalaryCycleRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<SalaryCyclePeriodDataTableModel> Result = Mapper.Map<List<SalaryCyclePeriodDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).OrderByDescending(x => x.CreateDate).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }


        public ActionResult SalaryCyclePeriodClosePeriod(Guid id)
        {
            if (!CMS.HasAction(Permissions.SalaryCycle.Create, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            DateTime ExecutionTime = DateTime.Now;
            var nationalInformation = DbAHD.dataSalaryCycle.Where(x => x.SalaryCycleGUID == id).FirstOrDefault();
            nationalInformation.LastFlowStatusGUID = NationalStaffDangerPaConfirmationStatus.Confirmed;
            //var staffDangers = DbAHD.dataSalaryCycle.Where(x =>
            // x.LastFlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Confirmed
            //  ).ToList();
            //staffDangers.ForEach(x => x.IsPayed = true);
            //DbAHD.UpdateBulk(staffDangers, Permissions.SalaryCycle.CreateGuid, ExecutionTime, DbCMS);


            DbAHD.Update(nationalInformation, Permissions.SalaryCycle.UpdateGuid, ExecutionTime, DbCMS);


            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return RedirectToAction("ConfirmationView");

                //return Json(DbAHD.SingleUpdateMessage(DataTableNames.SalaryCyclePeriodDataTable, DbAHD.PrimaryKeyControl(nationalInformation), DbAHD.RowVersionControls(Portal.SingleToList(nationalInformation))));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        //[Route("AHD/SalaryCyclePeriod/Create/")]
        public ActionResult SalaryCyclePeriodCreate()
        {
            if (!CMS.HasAction(Permissions.SalaryCycle.Create, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/AHD/Views/SalaryCyclePeriod/_SalaryCyclePeriodUpdateModal.cshtml", new SalaryCyclePeriodUpdateModel { SalaryCycleGUID = Guid.Empty });

        }

        //[Route("AHD/SalaryCyclePeriod/Update/{PK}")]
        public ActionResult SalaryCyclePeriodUpdate(Guid PK)
        {
            var model = (from a in DbAHD.dataSalaryCycle.WherePK(PK)
                         select new SalaryCyclePeriodUpdateModel
                         {
                             SalaryCycleGUID = a.SalaryCycleGUID,
                             Year = a.Year,
                             MonthName = a.MonthName,
                             CreateDate = a.CreateDate,
                             CreateByGUID = a.CreateByGUID,
                             LastFlowStatusGUID = a.LastFlowStatusGUID,
                             Comments = a.Comments,
                             dataSalaryCycleRowVersion = a.dataSalaryCycleRowVersion,

                             Active = a.Active,
                             //DangerPaymentConfirmationStatus= R1.ValueDescription,


                         }).FirstOrDefault();
            ViewBag.DangerPayInformationGUID = DbAHD.dataDangerPayInformation.OrderByDescending(x => x.OrderId).Select(x => x.DangerPayInformationGUID).FirstOrDefault();
            ViewBag.PeriodEntitlementGUID = DbAHD.dataAHDPeriodEntitlement.OrderByDescending(x => x.OrderId).Select(x => x.PeriodEntitlementGUID).FirstOrDefault();
            //ViewBag.SalaryCycleGUID = PK;
            //ViewBag.TotalStaffNotConfirmed = DbAHD.dataSalaryCycle.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Pending && a.SalaryCycleGUID == PK).Count();
            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("SalaryCyclePeriod", "SalaryCyclePeriods", new { Area = "AHD" }));
            return View("~/Areas/AHD/Views/SalaryCyclePeriod/FlowStep/CycleFlowStep.cshtml", model);
            //            return View("~/Areas/AHD/Views/SalaryCyclePeriod/SalaryCyclePeriod.cshtml", model);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SalaryCyclePeriodCreate(SalaryCyclePeriodUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.SalaryCycle.Create, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveSalaryCyclePeriod(model)) return PartialView("~/Areas/AHD/Views/SalaryCyclePeriods/_SalaryCyclePeriodForm.cshtml", model);


            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            dataSalaryCycle SalaryCyclePeriod = Mapper.Map(model, new dataSalaryCycle());
            SalaryCyclePeriod.SalaryCycleGUID = EntityPK;
            SalaryCyclePeriod.Year = model.Year;
            SalaryCyclePeriod.MonthName = model.MonthName;
            SalaryCyclePeriod.CycleName = model.MonthName + "-" + model.Year;
            //SalaryCyclePeriod.RequestYear = model.RequestDate.Value.Year;
            SalaryCyclePeriod.LastFlowStatusGUID = VehicleMaintenanceRequestFlowStatus.Submitted;
            //SalaryCyclePeriod.LastFlowStatus = "Submitted";
            SalaryCyclePeriod.Comments = model.Comments;
            Guid _activeStaff = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3611");
            Guid internationaStaffGUID = Guid.Parse("FC4B2E79-2B97-4252-A50B-915B07A12310");
            var _staffs = DbAHD.v_staffCoreDataOverview.
                Where(x => x.StaffStatusGUID == _activeStaff && x.RecruitmentTypeGUID != internationaStaffGUID).ToList();
            Guid _salaryStep = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7359");
            Guid _pending = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7578");
            Guid _confimed = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7577");
            var _allsteps = DbAHD.codeTablesValuesLanguages.Where(x => x.codeTablesValues.TableGUID == _salaryStep && x.LanguageID == LAN && x.Active).OrderBy(x => x.codeTablesValues.SortID).ToList();
            List<dataSalaryCycleStep> _steps = (from x in _allsteps
                                                let Guid = Guid.NewGuid()
                                                select
                           new dataSalaryCycleStep
                           {
                               SalaryCycleStepGUID = Guid,
                               SalaryCycleGUID = EntityPK,
                               StepGUID = x.ValueGUID,
                               LastFlowStatusGUID = _pending,
                               OrderID = x.codeTablesValues.SortID,
                               ActionDate = ExecutionTime,
                               CreateByGUID = UserGUID,
                           }
                ).ToList();
            Guid _pendingGuid = Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E7547");
            Guid _salaryConfrmed = Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E7542");
            var _salaryAdvance = DbAHD.dataStaffSalaryInAdvance.Where(x => x.TotalRemining > 0 && x.StartMonthToDeduct <= ExecutionTime
           && x.LastFlowStatusGUID == _salaryConfrmed).ToList();

            List<dataStaffSalary> _staffSalary = (from x in _staffs
                                                  let Guid = Guid.NewGuid()
                                                  select

             new dataStaffSalary
             {
                 StaffSalaryGUID = Guid,
                 UserGUID = x.UserGUID,
                 SalaryCycleGUID = EntityPK,
                 StaffConfirmationStatusGUID = StaffSalaryFlowStatus.PendingBankConfirmation,
                 FlowStatusGUID = _pendingGuid,
                 AdvancedSalary = _salaryAdvance.Where(f => f.UserGUID == x.UserGUID) != null ? _salaryAdvance.Where(f => f.UserGUID == x.UserGUID).FirstOrDefault().MonthlyPyament : 0,


             }
                ).ToList();



            DbAHD.Create(SalaryCyclePeriod, Permissions.SalaryCycle.CreateGuid, ExecutionTime, DbCMS);
            DbAHD.CreateBulk(_staffSalary, Permissions.SalaryCycle.CreateGuid, DateTime.Now, DbCMS);
            DbAHD.CreateBulk(_steps, Permissions.SalaryCycle.CreateGuid, DateTime.Now, DbCMS);
            _salaryAdvance.ForEach(x => x.TotalRemining = (x.TotalRemining - x.MonthlyPyament) > 0 ? (x.TotalRemining - x.MonthlyPyament) : 0);

            DbAHD.UpdateBulk(_salaryAdvance, Permissions.SalaryCycle.UpdateGuid, DateTime.Now, DbCMS);

            try
            {

                DbAHD.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbAHD.SingleUpdateMessage(DataTableNames.SalaryCyclePeriodDataTable, DbAHD.PrimaryKeyControl(SalaryCyclePeriod), DbAHD.RowVersionControls(Portal.SingleToList(SalaryCyclePeriod))));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SalaryCyclePeriodUpdate(SalaryCyclePeriodUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.SalaryCycle.Update, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/AHD/Views/SalaryCyclePeriods/_SalaryCyclePeriodForm.cshtml", model);
            DateTime ExecutionTime = DateTime.Now;
            dataSalaryCycle SalaryCyclePeriod = Mapper.Map(model, new dataSalaryCycle());
            DbAHD.Update(SalaryCyclePeriod, Permissions.SalaryCycle.UpdateGuid, ExecutionTime, DbCMS);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.SalaryCyclePeriodDataTable, DbAHD.PrimaryKeyControl(SalaryCyclePeriod), DbAHD.RowVersionControls(Portal.SingleToList(SalaryCyclePeriod))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencySalaryCyclePeriod(model.SalaryCycleGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SalaryCyclePeriodDelete(dataSalaryCycle model)
        {
            if (!CMS.HasAction(Permissions.SalaryCycle.Delete, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataSalaryCycle> DeletedSalaryCyclePeriod = DeleteSalaryCyclePeriod(Portal.SingleToList(model));
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.SalaryCycle.Restore, Apps.AHD), Container = "SalaryCyclePeriodFormControls" });
            try
            {
                int CommitedRows = DbAHD.SaveChanges();
                DbAHD.SaveChanges();
                return Json(DbAHD.SingleDeleteMessage(CommitedRows, DeletedSalaryCyclePeriod.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencySalaryCyclePeriod(model.SalaryCycleGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SalaryCyclePeriodRestore(dataSalaryCycle model)
        {
            if (!CMS.HasAction(Permissions.SalaryCycle.Restore, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveSalaryCyclePeriod(model))
            {
                return Json(DbAHD.RecordExists());
            }
            List<dataSalaryCycle> RestoredSalaryCyclePeriod = RestoreSalaryCyclePeriods(Portal.SingleToList(model));
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.SalaryCycle.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("SalaryCyclePeriodCreate", "Configuration", new { Area = "AHD" })), Container = "SalaryCyclePeriodFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.SalaryCycle.Update, Apps.AHD), Container = "SalaryCyclePeriodFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.SalaryCycle.Delete, Apps.AHD), Container = "SalaryCyclePeriodFormControls" });
            try
            {
                int CommitedRows = DbAHD.SaveChanges();
                DbAHD.SaveChanges();
                return Json(DbAHD.SingleRestoreMessage(CommitedRows, RestoredSalaryCyclePeriod, DbAHD.PrimaryKeyControl(RestoredSalaryCyclePeriod.FirstOrDefault()), Url.Action(DataTableNames.SalaryCyclePeriodDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencySalaryCyclePeriod(model.SalaryCycleGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult SalaryCyclePeriodDataTableDelete(List<dataSalaryCycle> models)
        {
            if (!CMS.HasAction(Permissions.SalaryCycle.Delete, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataSalaryCycle> DeletedSalaryCyclePeriod = DeleteSalaryCyclePeriod(models);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialDeleteMessage(DeletedSalaryCyclePeriod, models, DataTableNames.SalaryCyclePeriodDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult SalaryCyclePeriodDataTableRestore(List<dataSalaryCycle> models)
        {
            if (!CMS.HasAction(Permissions.SalaryCycle.Restore, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataSalaryCycle> RestoredSalaryCyclePeriod = DeleteSalaryCyclePeriod(models);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialRestoreMessage(RestoredSalaryCyclePeriod, models, DataTableNames.SalaryCyclePeriodDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        private List<dataSalaryCycle> DeleteSalaryCyclePeriod(List<dataSalaryCycle> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataSalaryCycle> DeletedSalaryCyclePeriod = new List<dataSalaryCycle>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT SalaryCycleGUID,CONVERT(varchar(50), SalaryCycleGUID) as C2 ,dataSalaryCycleRowVersion FROM code.dataSalaryCycle where SalaryCycleGUID in (" + string.Join(",", models.Select(x => "'" + x.SalaryCycleGUID + "'").ToArray()) + ")";
            string query = DbAHD.QueryBuilder(models, Permissions.SalaryCycle.DeleteGuid, SubmitTypes.Delete, "");
            var Records = DbAHD.Database.SqlQuery<dataSalaryCycle>(query).ToList();
            foreach (var record in Records)
            {
                DeletedSalaryCyclePeriod.Add(DbAHD.Delete(record, ExecutionTime, Permissions.SalaryCycle.DeleteGuid, DbCMS));
            }
            return DeletedSalaryCyclePeriod;
        }
        private List<dataSalaryCycle> RestoreSalaryCyclePeriods(List<dataSalaryCycle> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataSalaryCycle> RestoredSalaryCyclePeriod = new List<dataSalaryCycle>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT SalaryCycleGUID,CONVERT(varchar(50), SalaryCycleGUID) as C2 ,dataSalaryCycleRowVersion FROM code.dataSalaryCycle where SalaryCycleGUID in (" + string.Join(",", models.Select(x => "'" + x.SalaryCycleGUID + "'").ToArray()) + ")";

            string query = DbAHD.QueryBuilder(models, Permissions.SalaryCycle.DeleteGuid, SubmitTypes.Restore, "");

            var Records = DbAHD.Database.SqlQuery<dataSalaryCycle>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveSalaryCyclePeriod(record))
                {
                    RestoredSalaryCyclePeriod.Add(DbAHD.Restore(record, Permissions.SalaryCycle.DeleteGuid, Permissions.SalaryCycle.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredSalaryCyclePeriod;
        }

        private JsonResult ConcurrencySalaryCyclePeriod(Guid PK)
        {
            SalaryCyclePeriodDataTableModel dbModel = new SalaryCyclePeriodDataTableModel();

            var SalaryCyclePeriod = DbAHD.dataSalaryCycle.Where(x => x.SalaryCycleGUID == PK).FirstOrDefault();
            var dbSalaryCyclePeriod = DbAHD.Entry(SalaryCyclePeriod).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbSalaryCyclePeriod, dbModel);

            if (SalaryCyclePeriod.dataSalaryCycleRowVersion.SequenceEqual(dbModel.dataSalaryCycleRowVersion))
            {
                return Json(DbAHD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbAHD, dbModel, "LanguagesContainer"));
        }

        private bool ActiveSalaryCyclePeriod(Object model)
        {
            dataSalaryCycle SalaryCyclePeriod = Mapper.Map(model, new dataSalaryCycle());
            int ModelDescription = DbAHD.dataSalaryCycle
                                    .Where(x => x.MonthName == SalaryCyclePeriod.MonthName &&
                                                x.Year == SalaryCyclePeriod.Year &&
                                                x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("Record ", " already exists");
            }
            return (ModelDescription > 0);
        }


        #endregion

        #region Salary HQ Process
        public JsonResult StaffHQSalaryDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<HQStaffSalaryDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<HQStaffSalaryDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.SalaryCycle.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix

            var All = (
                from a in DbAHD.dataTempHQStaffSalary.Where(x => x.Active).AsExpandable()

                select new HQStaffSalaryDataTableModel
                {
                    TempHQStaffSalaryGUID = a.TempHQStaffSalaryGUID.ToString(),
                    UserGUID = a.UserGUID.ToString(),
                    SalaryCycleGUID = a.SalaryCycleGUID.ToString(),
                    VendorID = a.VendorID.ToString(),
                    EmployeeID = a.EmployeeID.ToString(),
                    FullName = a.FullName.ToString(),
                    Active = a.Active,
                    HQSalary = (float)a.HQSalary,

                    dataTempHQStaffSalaryowVersion = a.dataTempHQStaffSalaryowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<HQStaffSalaryDataTableModel> Result = Mapper.Map<List<HQStaffSalaryDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).OrderByDescending(x => x.FullName).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult StaffHQSalaryCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.SalaryCycle.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            return PartialView("~/Areas/AHD/Views/SalaryCyclePeriod/HQSalary/_HQSalaryUpload.cshtml",
                new HQStaffSalaryUpdateModel { SalaryCycleGUID = FK });
        }

        [HttpPost]
        public FineUploaderResult UploadHQSalary(FineUpload upload, Guid SalaryCycleGUID)
        {
            if (!CMS.HasAction(Permissions.SalaryCycle.Create, Apps.AHD))
            {
                return new FineUploaderResult(false, new { path = Upload(upload, SalaryCycleGUID), success = false });
            }
            return new FineUploaderResult(true, new { path = Upload(upload, SalaryCycleGUID), success = true });
        }

        public string Upload(FineUpload upload, Guid SalaryCycleGUID)
        {
            var _stearm = upload.InputStream;
            string _ext = Path.GetExtension(upload.FileName).Split('.')[1];


            string FilePath = Server.MapPath("~/Areas/AHD/Templates/StaffSalary/HQSalary/" + SalaryCycleGUID + ".xlsx");

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
                    List<dataTempHQStaffSalary> _toaddAll = new List<dataTempHQStaffSalary>();

                    bool ok = Validate(workSheet);

                    package.Save();

                    if (ok)
                    {
                        int totalRows = workSheet.Dimension.End.Row;
                        var _toremoveTemp = DbAHD.dataTempHQStaffSalary.Where(x => x.SalaryCycleGUID == SalaryCycleGUID).ToList();
                        var _olduser = _toremoveTemp.Select(x => x.UserGUID).Distinct().ToList();
                        var _oldSalaries = DbAHD.dataStaffSalary.Where(x => x.SalaryCycleGUID == SalaryCycleGUID && _olduser.Contains(x.UserGUID)).ToList();
                        _oldSalaries.ForEach(x => x.HQSalary = null);
                        DbAHD.dataTempHQStaffSalary.RemoveRange(_toremoveTemp);
                        DbAHD.UpdateBulk(_oldSalaries, Permissions.SalaryCycle.CreateGuid, DateTime.Now, DbCMS);




                        DbAHD.SaveChanges();
                        DbCMS.SaveChanges();
                        var allStaffCore = DbAHD.v_staffCoreDataOverview.ToList();
                        for (int i = 2; i <= totalRows; i++)
                        {

                            var employeeID = workSheet.Cells["A" + i].Value;

                            var staffinfo = allStaffCore.Where(x => x.EmploymentID.ToString() == employeeID.ToString()).FirstOrDefault();
                            if (staffinfo == null)
                                break;



                            var _salary = workSheet.Cells["B" + i].Value;

                            var _vendorID = workSheet.Cells["C" + i].Value;



                            if (staffinfo != null)
                            {



                                Guid EntityPK = Guid.NewGuid();
                                int _emp = int.Parse(_vendorID.ToString());



                                // Guid damascusWarehosue = Guid.Parse("7E208D24-8F61-4403-A7A7-C1C2A4BE55B4");
                                dataTempHQStaffSalary toAd = new dataTempHQStaffSalary
                                {
                                    TempHQStaffSalaryGUID = Guid.NewGuid(),
                                    UserGUID = staffinfo.UserGUID,
                                    SalaryCycleGUID = SalaryCycleGUID,
                                    HQSalary = (double)_salary,
                                    VendorID = staffinfo.VendorID,
                                    EmployeeID = staffinfo.EmploymentID.ToString(),


                                    FullName = staffinfo.FullName,





                                };



                                _toaddAll.Add(toAd);
                            }

                        }

                        DbAHD.CreateBulk(_toaddAll, Permissions.SalaryCycle.CreateGuid, DateTime.Now, DbCMS);
                        var _selectedUser = _toaddAll.Select(x => x.UserGUID).Distinct().ToList();
                        var _allSalaries = DbAHD.dataStaffSalary.Where(x => x.SalaryCycleGUID == SalaryCycleGUID && _selectedUser.Contains(x.UserGUID)).ToList();
                        _allSalaries.ForEach(x => x.HQSalary = _toaddAll.Where(f => f.SalaryCycleGUID == x.SalaryCycleGUID).FirstOrDefault().HQSalary);
                        DbAHD.UpdateBulk(_allSalaries, Permissions.SalaryCycle.CreateGuid, DateTime.Now, DbCMS);


                        try
                        {
                            DbAHD.SaveChanges();
                            DbCMS.SaveChanges();


                            // return Json(DbAHD.SingleUpdateMessage(DataTableNames.WarehouseModelEntryMovementsDataTable, DbAHD.PrimaryKeyControl(EntryModel), DbAHD.RowVersionControls(Portal.SingleToList(EntryModel))));
                        }
                        catch (Exception ex)
                        {
                            var error = DbAHD.ErrorMessage(ex.Message);
                        }
                    }
                }
            }
            return "~/Areas/AHD/Templates/StaffSalary/HQSalary/" + SalaryCycleGUID + ".xlsx";
        }

        private bool Validate(ExcelWorksheet workSheet)
        {
            var allStaffCore = DbAHD.v_staffCoreDataOverview.ToList();

            int totalRows = workSheet.Dimension.End.Row;
            bool valid = true;
            for (int i = 2; i < totalRows; i++)
            {
                var employeeID = workSheet.Cells["A" + i].Value;

                var staffinfo = allStaffCore.Where(x => x.EmploymentID.ToString() == employeeID.ToString()).FirstOrDefault();
                if (employeeID == null || staffinfo == null)
                    break;

                var _vendor = workSheet.Cells["B" + i].Value;
                var _Salary = workSheet.Cells["C" + i].Value;





                if (_Salary == null || _vendor == null || staffinfo == null)
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
        #region Medical Payments

        [Route("AHD/StaffMedicalPaymentIndex/")]
        public ActionResult StaffMedicalPaymentIndex()
        {
            if (!CMS.HasAction(Permissions.StaffSalaryProcess.Access, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return View("~/Areas/AHD/Views/StaffMedicalPayment/Index.cshtml");
        }
        [Route("AHD/StaffMedicalPaymentDataTable/")]
        public JsonResult StaffMedicalPaymentDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<StaffMedicalPaymentDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<StaffMedicalPaymentDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.StaffSalaryProcess.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix

            var All = (
                from a in DbAHD.dataStaffMedicalPayment.Where(x => x.Active).AsExpandable()
                join b in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.LastFlowStatusGUID equals b.ValueGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.CreatedByGUID equals c.UserGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()
                join d in DbAHD.v_StaffProfileInformation on a.UserGUID equals d.UserGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()
                select new StaffMedicalPaymentDataTableModel
                {
                    StaffMedicalPaymentGUID = a.StaffMedicalPaymentGUID.ToString(),
                    Year = a.Year.ToString(),

                    Active = a.Active,
                    ActionDate = a.ActionDate,
                    Month = a.Month.ToString(),
                    ClaimNumber = a.ClaimNumber,
                    AmountUSD = (float)a.AmountUSD,
                    //CurrencyExchangeUSD = (float)a.CurrencyExchangeUSD,
                    MedicalReason = a.MedicalReason,
                    StaffName = R3.FirstName + " " + R3.SurName,

                    CreatedByGUID = a.CreatedByGUID.ToString(),
                    LastFlowStatusGUID = a.LastFlowStatusGUID.ToString(),
                    Status = R1.ValueDescription,
                    CreateBy = R2.FirstName + " " + R2.Surname,
                    //FlowStatusGUID = a.FlowStatusGUID,
                    //PaymentDurationName = a.PaymentDurationName,
                    //TotalStaffConfirm = a.dataStaffMedicalPayment1.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Confirmed).Count(),
                    //TotalStaffNotConfirm = a.dataStaffMedicalPayment1.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Pending).Count(),

                    //OrderId = a.OrderId,
                    //CreateDate = a.CreateDate,
                    dataStaffMedicalPaymentRowVersion = a.dataStaffMedicalPaymentRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<StaffMedicalPaymentDataTableModel> Result = Mapper.Map<List<StaffMedicalPaymentDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).OrderByDescending(x => x.ActionDate).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }


        public JsonResult StaffMedicalPaymentForSalaryDataTable(DataTableRecievedOptions options, Guid PK)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<StaffMedicalPaymentDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<StaffMedicalPaymentDataTableModel>(DataTable.Filters);
            }
            var _cycleSalary = DbAHD.dataSalaryCycle.Where(x => x.SalaryCycleGUID == PK).FirstOrDefault();
            var _codeMonth = DbAHD.codeMonth.Where(x => x.MonthCode == _cycleSalary.MonthName).FirstOrDefault();

            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.StaffSalaryProcess.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix

            var All = (
                from a in DbAHD.dataStaffMedicalPayment.Where(x => x.Active && x.Month == _codeMonth.MonthCode && x.Year == _cycleSalary.Year).AsExpandable()
                join b in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.LastFlowStatusGUID equals b.ValueGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.CreatedByGUID equals c.UserGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()
                join d in DbAHD.v_StaffProfileInformation on a.UserGUID equals d.UserGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()
                select new StaffMedicalPaymentDataTableModel
                {
                    StaffMedicalPaymentGUID = a.StaffMedicalPaymentGUID.ToString(),
                    Year = a.Year.ToString(),

                    Active = a.Active,
                    ActionDate = a.ActionDate,
                    Month = a.Month.ToString(),
                    ClaimNumber = a.ClaimNumber,
                    AmountUSD = (float)a.AmountUSD,
                    //CurrencyExchangeUSD = (float)a.CurrencyExchangeUSD,
                    MedicalReason = a.MedicalReason,
                    StaffName = R3.FirstName + " " + R3.SurName,

                    CreatedByGUID = a.CreatedByGUID.ToString(),
                    LastFlowStatusGUID = a.LastFlowStatusGUID.ToString(),
                    Status = R1.ValueDescription,
                    CreateBy = R2.FirstName + " " + R2.Surname,
                    //FlowStatusGUID = a.FlowStatusGUID,
                    //PaymentDurationName = a.PaymentDurationName,
                    //TotalStaffConfirm = a.dataStaffMedicalPayment1.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Confirmed).Count(),
                    //TotalStaffNotConfirm = a.dataStaffMedicalPayment1.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Pending).Count(),

                    //OrderId = a.OrderId,
                    //CreateDate = a.CreateDate,
                    dataStaffMedicalPaymentRowVersion = a.dataStaffMedicalPaymentRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<StaffMedicalPaymentDataTableModel> Result = Mapper.Map<List<StaffMedicalPaymentDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).OrderByDescending(x => x.ActionDate).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public JsonResult StaffMedicalPaymentPKDataTable(DataTableRecievedOptions options, Guid PK)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<StaffMedicalPaymentDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<StaffMedicalPaymentDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.StaffSalaryProcess.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
            var _salary = DbAHD.dataSalaryCycle.Where(x => x.SalaryCycleGUID == PK).FirstOrDefault();
            var All = (
                from a in DbAHD.dataStaffMedicalPayment.Where(x => x.Active && (x.Year == _salary.Year && x.Month == _salary.MonthName)).AsExpandable()
                join b in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.LastFlowStatusGUID equals b.ValueGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.CreatedByGUID equals c.UserGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()
                join d in DbAHD.v_StaffProfileInformation on a.UserGUID equals d.UserGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()
                select new StaffMedicalPaymentDataTableModel
                {
                    StaffMedicalPaymentGUID = a.StaffMedicalPaymentGUID.ToString(),
                    Year = a.Year.ToString(),

                    Active = a.Active,
                    ActionDate = a.ActionDate,
                    Month = a.Month.ToString(),
                    ClaimNumber = a.ClaimNumber,
                    AmountUSD = (float)a.AmountUSD,

                    MedicalReason = a.MedicalReason,
                    StaffName = R3.FirstName + " " + R3.SurName,

                    CreatedByGUID = a.CreatedByGUID.ToString(),
                    LastFlowStatusGUID = a.LastFlowStatusGUID.ToString(),
                    Status = R1.ValueDescription,
                    CreateBy = R2.FirstName + " " + R2.Surname,
                    //FlowStatusGUID = a.FlowStatusGUID,
                    //PaymentDurationName = a.PaymentDurationName,
                    //TotalStaffConfirm = a.dataStaffMedicalPayment1.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Confirmed).Count(),
                    //TotalStaffNotConfirm = a.dataStaffMedicalPayment1.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Pending).Count(),

                    //OrderId = a.OrderId,
                    //CreateDate = a.CreateDate,
                    dataStaffMedicalPaymentRowVersion = a.dataStaffMedicalPaymentRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<StaffMedicalPaymentDataTableModel> Result = Mapper.Map<List<StaffMedicalPaymentDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).OrderByDescending(x => x.ActionDate).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }


        //[Route("AHD/StaffMedicalPayment/Create/")]
        public ActionResult StaffMedicalPaymentCreate()
        {
            if (!CMS.HasAction(Permissions.StaffSalaryProcess.Create, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/AHD/Views/StaffMedicalPayment/_StaffMedicalPaymentUpdateModal.cshtml", new StaffMedicalPaymentUpdateModel { StaffMedicalPaymentGUID = Guid.Empty });

        }

        //[Route("AHD/StaffMedicalPayment/Update/{PK}")]
        public ActionResult StaffMedicalPaymentUpdate(Guid PK)
        {
            var model = (from a in DbAHD.dataStaffMedicalPayment.WherePK(PK)
                         select new StaffMedicalPaymentUpdateModel
                         {
                             StaffMedicalPaymentGUID = a.StaffMedicalPaymentGUID,

                             Year = a.Year,


                             ActionDate = a.ActionDate,
                             Month = a.Month.ToString(),

                             AmountUSD = (float)a.AmountUSD,
                             ClaimNumber = a.ClaimNumber,
                             MedicalReason = a.MedicalReason,


                             UserGUID = a.UserGUID,

                             CreatedByGUID = a.CreatedByGUID,
                             CreateDate = a.CreateDate,
                             UpdatedByGUID = a.UpdatedByGUID,
                             UpdateDate = a.UpdateDate,
                             LastFlowStatusGUID = a.LastFlowStatusGUID,
                             Comments = a.Comments,
                             dataStaffMedicalPaymentRowVersion = a.dataStaffMedicalPaymentRowVersion,

                             Active = a.Active,
                             //DangerPaymentConfirmationStatus= R1.ValueDescription,


                         }).FirstOrDefault();
            //ViewBag.StaffMedicalPaymentGUID = PK;
            //ViewBag.TotalStaffNotConfirmed = DbAHD.dataStaffMedicalPayment.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Pending && a.StaffMedicalPaymentGUID == PK).Count();
            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("StaffMedicalPayment", "StaffMedicalPayments", new { Area = "AHD" }));
            return PartialView("~/Areas/AHD/Views/StaffMedicalPayment/_StaffMedicalPaymentUpdateModal.cshtml", model);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffMedicalPaymentCreate(StaffMedicalPaymentUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.StaffSalaryProcess.Create, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveStaffMedicalPayment(model)) return PartialView("~/Areas/AHD/Views/StaffMedicalPayments/_StaffMedicalPaymentForm.cshtml", model);


            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            dataStaffMedicalPayment StaffMedicalPayment = Mapper.Map(model, new dataStaffMedicalPayment());
            StaffMedicalPayment.StaffMedicalPaymentGUID = EntityPK;
            StaffMedicalPayment.Year = model.Year;
            StaffMedicalPayment.ActionDate = model.ActionDate;
            StaffMedicalPayment.Month = model.Month;
            StaffMedicalPayment.CreateDate = ExecutionTime;

            StaffMedicalPayment.CreateDate = ExecutionTime;

            //StaffMedicalPayment.RequestYear = model.RequestDate.Value.Year;
            StaffMedicalPayment.LastFlowStatusGUID = AHDActionFlowStatus.Pending;
            //StaffMedicalPayment.LastFlowStatus = "Submitted";
            StaffMedicalPayment.Comments = model.Comments;


            DbAHD.Create(StaffMedicalPayment, Permissions.StaffSalaryProcess.CreateGuid, ExecutionTime, DbCMS);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbAHD.SingleUpdateMessage(DataTableNames.StaffMedicalPaymentDataTable, DbAHD.PrimaryKeyControl(StaffMedicalPayment), DbAHD.RowVersionControls(Portal.SingleToList(StaffMedicalPayment))));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffMedicalPaymentUpdate(StaffMedicalPaymentUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.StaffSalaryProcess.Update, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/AHD/Views/StaffMedicalPayments/_StaffMedicalPaymentForm.cshtml", model);
            DateTime ExecutionTime = DateTime.Now;

            dataStaffMedicalPayment StaffMedicalPayment = Mapper.Map(model, new dataStaffMedicalPayment());
            StaffMedicalPayment.UpdateDate = ExecutionTime;
            StaffMedicalPayment.UpdatedByGUID = UserGUID;
            DbAHD.Update(StaffMedicalPayment, Permissions.StaffSalaryProcess.UpdateGuid, ExecutionTime, DbCMS);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.StaffMedicalPaymentDataTable, DbAHD.PrimaryKeyControl(StaffMedicalPayment), DbAHD.RowVersionControls(Portal.SingleToList(StaffMedicalPayment))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyStaffMedicalPayment(model.StaffMedicalPaymentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffMedicalPaymentDelete(dataStaffMedicalPayment model)
        {
            if (!CMS.HasAction(Permissions.StaffSalaryProcess.Delete, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataStaffMedicalPayment> DeletedStaffMedicalPayment = DeleteStaffMedicalPayment(Portal.SingleToList(model));
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.StaffSalaryProcess.Restore, Apps.AHD), Container = "StaffMedicalPaymentFormControls" });
            try
            {
                int CommitedRows = DbAHD.SaveChanges();
                DbAHD.SaveChanges();
                return Json(DbAHD.SingleDeleteMessage(CommitedRows, DeletedStaffMedicalPayment.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyStaffMedicalPayment(model.StaffMedicalPaymentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffMedicalPaymentRestore(dataStaffMedicalPayment model)
        {
            if (!CMS.HasAction(Permissions.StaffSalaryProcess.Restore, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveStaffMedicalPayment(model))
            {
                return Json(DbAHD.RecordExists());
            }
            List<dataStaffMedicalPayment> RestoredStaffMedicalPayment = RestoreStaffMedicalPayments(Portal.SingleToList(model));
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.StaffSalaryProcess.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("StaffMedicalPaymentCreate", "Configuration", new { Area = "AHD" })), Container = "StaffMedicalPaymentFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.StaffSalaryProcess.Update, Apps.AHD), Container = "StaffMedicalPaymentFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.StaffSalaryProcess.Delete, Apps.AHD), Container = "StaffMedicalPaymentFormControls" });
            try
            {
                int CommitedRows = DbAHD.SaveChanges();
                DbAHD.SaveChanges();
                return Json(DbAHD.SingleRestoreMessage(CommitedRows, RestoredStaffMedicalPayment, DbAHD.PrimaryKeyControl(RestoredStaffMedicalPayment.FirstOrDefault()), Url.Action(DataTableNames.StaffMedicalPaymentDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyStaffMedicalPayment(model.StaffMedicalPaymentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffMedicalPaymentDataTableDelete(List<dataStaffMedicalPayment> models)
        {
            if (!CMS.HasAction(Permissions.StaffSalaryProcess.Delete, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataStaffMedicalPayment> DeletedStaffMedicalPayment = DeleteStaffMedicalPayment(models);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialDeleteMessage(DeletedStaffMedicalPayment, models, DataTableNames.StaffMedicalPaymentDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffMedicalPaymentDataTableRestore(List<dataStaffMedicalPayment> models)
        {
            if (!CMS.HasAction(Permissions.StaffSalaryProcess.Restore, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataStaffMedicalPayment> RestoredStaffMedicalPayment = DeleteStaffMedicalPayment(models);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialRestoreMessage(RestoredStaffMedicalPayment, models, DataTableNames.StaffMedicalPaymentDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        private List<dataStaffMedicalPayment> DeleteStaffMedicalPayment(List<dataStaffMedicalPayment> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataStaffMedicalPayment> DeletedStaffMedicalPayment = new List<dataStaffMedicalPayment>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT StaffMedicalPaymentGUID,CONVERT(varchar(50), StaffMedicalPaymentGUID) as C2 ,dataStaffMedicalPaymentRowVersion FROM code.dataStaffMedicalPayment where StaffMedicalPaymentGUID in (" + string.Join(",", models.Select(x => "'" + x.StaffMedicalPaymentGUID + "'").ToArray()) + ")";
            string query = DbAHD.QueryBuilder(models, Permissions.StaffSalaryProcess.DeleteGuid, SubmitTypes.Delete, "");
            var Records = DbAHD.Database.SqlQuery<dataStaffMedicalPayment>(query).ToList();
            foreach (var record in Records)
            {
                DeletedStaffMedicalPayment.Add(DbAHD.Delete(record, ExecutionTime, Permissions.StaffSalaryProcess.DeleteGuid, DbCMS));
            }
            return DeletedStaffMedicalPayment;
        }
        private List<dataStaffMedicalPayment> RestoreStaffMedicalPayments(List<dataStaffMedicalPayment> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataStaffMedicalPayment> RestoredStaffMedicalPayment = new List<dataStaffMedicalPayment>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT StaffMedicalPaymentGUID,CONVERT(varchar(50), StaffMedicalPaymentGUID) as C2 ,dataStaffMedicalPaymentRowVersion FROM code.dataStaffMedicalPayment where StaffMedicalPaymentGUID in (" + string.Join(",", models.Select(x => "'" + x.StaffMedicalPaymentGUID + "'").ToArray()) + ")";

            string query = DbAHD.QueryBuilder(models, Permissions.StaffSalaryProcess.DeleteGuid, SubmitTypes.Restore, "");

            var Records = DbAHD.Database.SqlQuery<dataStaffMedicalPayment>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveStaffMedicalPayment(record))
                {
                    RestoredStaffMedicalPayment.Add(DbAHD.Restore(record, Permissions.StaffSalaryProcess.DeleteGuid, Permissions.StaffSalaryProcess.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredStaffMedicalPayment;
        }

        private JsonResult ConcurrencyStaffMedicalPayment(Guid PK)
        {
            StaffMedicalPaymentDataTableModel dbModel = new StaffMedicalPaymentDataTableModel();

            var StaffMedicalPayment = DbAHD.dataStaffMedicalPayment.Where(x => x.StaffMedicalPaymentGUID == PK).FirstOrDefault();
            var dbStaffMedicalPayment = DbAHD.Entry(StaffMedicalPayment).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbStaffMedicalPayment, dbModel);

            if (StaffMedicalPayment.dataStaffMedicalPaymentRowVersion.SequenceEqual(dbModel.dataStaffMedicalPaymentRowVersion))
            {
                return Json(DbAHD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbAHD, dbModel, "LanguagesContainer"));
        }

        private bool ActiveStaffMedicalPayment(Object model)
        {
            dataStaffMedicalPayment StaffMedicalPayment = Mapper.Map(model, new dataStaffMedicalPayment());
            int ModelDescription = DbAHD.dataStaffMedicalPayment
                                    .Where(x => x.Month == StaffMedicalPayment.Month &&
                                                x.Year == StaffMedicalPayment.Year &&
                                                x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("Record ", " already exists");
            }
            return (ModelDescription > 0);
        }


        #endregion

        #region Salary Bank Destenation

        public JsonResult CycleSalaryStaffBankDestenationDataTable(DataTableRecievedOptions options, Guid PK)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<StaffSalaryDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<StaffSalaryDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.SalaryCycle.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix

            var All = (
                from a in DbAHD.dataStaffSalary.Where(x => x.Active && x.SalaryCycleGUID == PK).AsExpandable()
                join b in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.StaffConfirmationStatusGUID equals b.ValueGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.UserGUID equals c.UserGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()
                join d in DbAHD.codeBankLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.BankGUID equals d.BankGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()
                join e in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.BankBranchGUID equals e.ValueGUID into LJ4
                from R4 in LJ4.DefaultIfEmpty()
                select new StaffSalaryDataTableModel
                {
                    StaffSalaryGUID = a.StaffSalaryGUID.ToString(),
                    SalaryCycleGUID = a.SalaryCycleGUID.ToString(),
                    UserGUID = a.UserGUID.ToString(),
                    BankGUID = a.BankGUID.ToString(),
                    BankName = R3.BankDescription,
                    AccountNumber = a.AccountNumber,
                    StaffName = R2.FirstName + " " + R2.Surname,
                    StaffConfirmationStatusGUID = a.StaffConfirmationStatusGUID.ToString(),
                    Status = R1.ValueDescription,
                    BranchName = R4.ValueDescription,
                    Active = a.Active,


                    dataStaffSalaryRowVersion = a.dataStaffSalaryRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<StaffSalaryDataTableModel> Result = Mapper.Map<List<StaffSalaryDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).OrderBy(x => x.StaffName).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult SendReminderForPendingBankDestenationConfirmation(Guid FK)
        {

            string URL = "";
            string Anchor = "";
            string Link = "";
            //Guid _test = Guid.Parse("8F7EF83F-FD3E-4F8C-8735-8A22D3D61B75");
            var _myPendingConfirmation = DbAHD.dataStaffSalary.Where(x => x.SalaryCycleGUID == FK
                   && x.StaffConfirmationStatusGUID == StaffSalaryFlowStatus.PendingBankConfirmation
                   //&&x.UserGUID==_test
                   ).ToList();
            var staffGUIDs = _myPendingConfirmation.Select(x => x.UserGUID).ToList();

            var allUsers = DbAHD.userPersonalDetailsLanguage.Where(x => staffGUIDs.Contains(x.UserGUID)
                                                                          && x.LanguageID == LAN).ToList();
            var alluserAccounts = DbAHD.userServiceHistory.Where(x => staffGUIDs.Contains(x.UserGUID)).ToList();
            //var allDan = DbAHD.dataNationalStaffDangerPay.Where(x => x.DangerPayInformationGUID == FK).ToList();
            var _salaryCycle = DbAHD.dataSalaryCycle.Where(x => x.SalaryCycleGUID == FK).FirstOrDefault();

            string SubjectMessage = resxEmails.StaffSalaryBankDestenationConfirmationSubject.Replace("$Month", _salaryCycle.MonthName).Replace("$Year", _salaryCycle.Year.ToString());


            for (int i = 0; i < allUsers.Count(); i += 30)
            {
                var target = allUsers.Skip(i).Take(30);

                foreach (var user in target)
                {
                    //{
                    var _currStaffSalary = _myPendingConfirmation.Where(x => x.UserGUID == user.UserGUID).FirstOrDefault();
                    URL = AppSettingsKeys.Domain + "/AHD/StaffSalary/GetStaffSalaryBankDestination/?id=" + new Portal().GUIDToString(_currStaffSalary.StaffSalaryGUID);
                    //URL = AppSettingsKeys.Domain + "/AHD/NationalStaffDangerPayCalcualtion/TrackStaffDangerPayments/?id=" + new Portal().GUIDToString(currentDanger.NationalStaffDangerPayGUID);
                    Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmDangerPayReceiving + "</a>";
                    Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                    string myFirstName = user.FirstName;
                    string mySurName = user.Surname;


                    string _message = resxEmails.StaffSalaryBankStaffConfirmation
                        .Replace("$FullName", user.FirstName + " " + user.Surname)
                        .Replace("$VerifyLink", Anchor)
                        .Replace("$period", _salaryCycle.MonthName + " " + _salaryCycle.Year.ToString())

                        ;
                    if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                    int isRec = 1;
                    var myEmail = alluserAccounts.Where(x => x.UserGUID == user.UserGUID).Select(x => x.EmailAddress).FirstOrDefault();

                    Send(myEmail, SubjectMessage, _message, isRec, null);
                }
            }
            return RedirectToAction("ConfirmationView");


        }
        public ActionResult ConfirmationView(string result)
        {
            return View("~/Areas/AHD/Views/Confirmation/Confirm.cshtml");

        }


        public ActionResult GetStaffSalaryBankDestination(Guid id)
        {
            var _salary = DbAHD.dataStaffSalary.Where(x => x.StaffSalaryGUID == id).FirstOrDefault();
            if (_salary.UserGUID != UserGUID || _salary.StaffConfirmationStatusGUID != StaffSalaryFlowStatus.PendingBankConfirmation)
            {
                return Json(DbAHD.PermissionError());
            }
            StaffSalaryUpdateModel model = new StaffSalaryUpdateModel();

            var _staff = DbAHD.v_staffCoreDataOverview.Where(x => x.UserGUID == _salary.UserGUID).FirstOrDefault();

            model.StaffConfirmationStatusGUID = _salary.StaffConfirmationStatusGUID;
            model.StaffSalaryGUID = _salary.StaffSalaryGUID;
            return View("~/Areas/AHD/Views/SalaryCyclePeriod/StaffBankDestenation/StaffBankReview.cshtml", model);
        }



        public JsonResult ConfirmStaffSalaryBankDestination(Guid _staffSalaryGUID, Guid _bankGUID, Guid? _bankBranchGUID, string _accountNumber)
        {
            var _staffSalary = DbAHD.dataStaffSalary.Where(x => x.StaffSalaryGUID == _staffSalaryGUID).FirstOrDefault();
            if (_staffSalary.UserGUID != UserGUID || string.IsNullOrEmpty(_accountNumber) || _staffSalary.StaffConfirmationStatusGUID != StaffSalaryFlowStatus.PendingBankConfirmation)
            {
                return Json(DbAHD.PermissionError());
            }
            string URL = "";
            string Anchor = "";
            string Link = "";
            DateTime ExecutionTime = DateTime.Now;

            if (_staffSalary.StaffConfirmationStatusGUID == StaffSalaryFlowStatus.PendingBankConfirmation)
            {
                //var toChange = DbAHD.dataStaffSalaryFlow.Where(x => x.StaffSalaryGUID == _staffSalaryGUID
                //                  && x.IsLastAction == true
                //                  ).FirstOrDefault();
                //toChange.IsLastAction = false;
                dataStaffSalaryFlow newFlowToReview = new dataStaffSalaryFlow
                {

                    StaffSalaryFlowGUID = Guid.NewGuid(),
                    StaffSalaryGUID = _staffSalaryGUID,
                    CreatedByGUID = UserGUID,
                    FlowStatusGUID = StaffSalaryFlowStatus.BankConfirmed,
                    ActionDate = ExecutionTime,
                    IsLastAction = true,
                    OrderID = 1,


                };
                _staffSalary.StaffConfirmationStatusGUID = StaffSalaryFlowStatus.BankConfirmed;
                _staffSalary.BankBranchGUID = _bankBranchGUID;
                _staffSalary.BankGUID = _bankGUID;
                _staffSalary.AccountNumber = _accountNumber;


                DbAHD.CreateNoAudit(newFlowToReview);
                DbAHD.UpdateNoAudit(_staffSalary);
                //  DbAHD.UpdateNoAudit(toChange);
                DbAHD.SaveChanges();




                var _staffName = DbAHD.v_staffCoreDataOverview.Where(x => x.UserGUID == _staffSalary.UserGUID).FirstOrDefault();
                string SubjectMessage = resxEmails.SalaryBankDestinationSubject.Replace("$period", _staffSalary.dataSalaryCycle.MonthName);


                //to send mail to staff 
                // var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();
                //URL = AppSettingsKeys.Domain + "/AHD/StaffOvertime/StaffOvertimeConfirmByReviewer/?PK=" + new Portal().GUIDToString(_overtime.StaffOvertimeGUID);
                //Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmDangerPayReceiving + "</a>";
                //Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";



                string _message = resxEmails.StaffBankDestinationAfterSubmission
                    .Replace("$FullName", _staffName.FullName.ToString())
                    //.Replace("$VerifyLink", Anchor)

                    ;

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;

                //Guid _overtimeReviewer = Guid.Parse("3260e610-8182-4c57-9412-8cb15315cce3");
                //var tempPermGUIDs = DbCMS.userPermissions.Where(x => (x.ActionGUID == _overtimeReviewer && x.Active == true
                //                 ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();
                //var _userGuids = DbCMS.userProfiles.Where(x => tempPermGUIDs.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

                //var _backupUsers = DbCMS.userServiceHistory.Where(x => _userGuids.Contains(x.UserGUID)
                //&& x.UserGUID != _overtime.UserGUID).Select(x => x.EmailAddress).Distinct().ToList();

                //_backupUsers.Add(_staffName.EmailAddress);
                //_backupUsers = _backupUsers.Distinct().ToList();
                //string copyEmails = string.Join(" ;", _backupUsers);






                //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
                //string copy_recipients = _staffName.EmailAddress;
                Send(_staffName.EmailAddress, SubjectMessage, _message, isRec, null);

                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);

            }

            //}
            return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
        }
        public void Send(string recipients, string subject, string body, int? isRec, string copy_recipients)
        {
            //string copy_recipients = "";
            string blind_copy_recipients = null;
            string body_format = "HTML";
            string importance = "Normal";
            string file_attachments = null;
            string _body = "<div style='font-family:Arial;'>" + body.Replace("\r\n", "<br/>") + "</div>";
            if (LAN == "AR") { _body = "<p align='right'>" + _body + "</p>"; }
            DbAHD.SendEmailHR("maksoud@unhcr.org", "", blind_copy_recipients, subject, _body, body_format, importance, file_attachments);
            // DbCMS.SendEmailHR(recipients, copy_recipients, blind_copy_recipients, subject, _body, body_format, importance, file_attachments);
        }

        #endregion

        #region Staff Salary Pyaments

        public JsonResult StaffSalaryCyclePaymentDataTable(DataTableRecievedOptions options, Guid PK)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<StaffSalaryDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<StaffSalaryDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.SalaryCycle.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix

            var All = (
                from a in DbAHD.dataStaffSalary.Where(x => x.Active && x.SalaryCycleGUID == PK).AsExpandable()
                join b in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.FlowStatusGUID equals b.ValueGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.UserGUID equals c.UserGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()
                select new StaffSalaryDataTableModel
                {
                    StaffSalaryGUID = a.StaffSalaryGUID.ToString(),
                    SalaryCycleGUID = a.SalaryCycleGUID.ToString(),
                    UserGUID = a.UserGUID.ToString(),
                    BankGUID = a.BankGUID.ToString(),
                    BankName = a.BankName,
                    AccountNumber = a.AccountNumber,
                    StaffName = R2.FirstName + " " + R2.Surname,
                    HQSalary = a.HQSalary,
                    AdvancedSalary = a.AdvancedSalary,
                    TeleBills = a.TeleBills,
                    Internet4G = a.Internet4G,
                    PhoneCall = a.PhoneCall,
                    AddOthers = a.AddOthers,
                    DangerPay = a.DangerPay,
                    Overtime = a.Overtime,
                    DeductionOthers = a.DeductionOthers,
                    MIP = a.MIP,
                    TotalSalary = a.TotalSalary,


                    Active = a.Active,

                    FlowStatusGUID = a.FlowStatusGUID.ToString(),
                    LastFlowStatusName = R1.ValueDescription,
                    //CreatedBy = R2.FirstName + " " + R2.Surname,
                    //FlowStatusGUID = a.FlowStatusGUID,
                    //PaymentDurationName = a.PaymentDurationName,
                    //TotalStaffConfirm = a.dataSalaryCyclePeriod1.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Confirmed).Count(),
                    //TotalStaffNotConfirm = a.dataSalaryCyclePeriod1.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Pending).Count(),

                    //OrderId = a.OrderId,
                    //CreateDate = a.CreateDate,
                    dataStaffSalaryRowVersion = a.dataStaffSalaryRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<StaffSalaryDataTableModel> Result = Mapper.Map<List<StaffSalaryDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).OrderBy(x => x.StaffName).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult StaffSalaryPaymentUpdate(Guid PK)
        {
            var myModel = DbAHD.dataStaffSalary.Where(x => x.StaffSalaryGUID == PK).FirstOrDefault(); ;
            StaffSalaryUpdateModel model = Mapper.Map(myModel, new StaffSalaryUpdateModel());
            var staff = DbAHD.v_staffCoreDataOverview.Where(x => x.UserGUID == myModel.UserGUID).FirstOrDefault();
            model.StaffName = staff.FullName;
            //var model = (from a in DbAHD.dataStaffSalary.WherePK(PK)
            //             select new StaffSalaryUpdateModel
            //             {
            //                 StaffSalaryGUID = a.StaffSalaryGUID,

            //                 UserGUID = a.Year,


            //                 ActionDate = a.ActionDate,
            //                 Month = a.Month.ToString(),

            //                 AmountUSD = (float)a.AmountUSD,
            //                 ClaimNumber = a.ClaimNumber,
            //                 MedicalReason = a.MedicalReason,


            //                 UserGUID = a.UserGUID,

            //                 CreatedByGUID = a.CreatedByGUID,
            //                 CreateDate = a.CreateDate,
            //                 UpdatedByGUID = a.UpdatedByGUID,
            //                 UpdateDate = a.UpdateDate,
            //                 LastFlowStatusGUID = a.LastFlowStatusGUID,
            //                 Comments = a.Comments,
            //                 dataStaffMedicalPaymentRowVersion = a.dataStaffMedicalPaymentRowVersion,

            //                 Active = a.Active,
            //                 //DangerPaymentConfirmationStatus= R1.ValueDescription,


            //             }).FirstOrDefault();
            //ViewBag.StaffMedicalPaymentGUID = PK;
            //ViewBag.TotalStaffNotConfirmed = DbAHD.dataStaffMedicalPayment.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Pending && a.StaffMedicalPaymentGUID == PK).Count();
            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("StaffMedicalPayment", "StaffMedicalPayments", new { Area = "AHD" }));
            return PartialView("~/Areas/AHD/Views/StaffSalary/_StaffSalaryPaymentUpdateModal.cshtml", model);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffSalaryPaymentUpdate(StaffSalaryUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.SalaryCycle.Update, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/AHD/Views/StaffSalary/_StaffSalaryPaymentUpdateModal.cshtml", model);
            DateTime ExecutionTime = DateTime.Now;

            dataStaffSalary staffsalary = Mapper.Map(model, new dataStaffSalary());
            //StaffMedicalPayment.UpdateDate = ExecutionTime;
            //StaffMedicalPayment.UpdatedByGUID = UserGUID;
            DbAHD.Update(staffsalary, Permissions.StaffSalaryProcess.UpdateGuid, ExecutionTime, DbCMS);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.StaffMedicalPaymentDataTable, DbAHD.PrimaryKeyControl(staffsalary), DbAHD.RowVersionControls(Portal.SingleToList(staffsalary))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyStaffSalaryPayment(model.StaffSalaryGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        private JsonResult ConcurrencyStaffSalaryPayment(Guid PK)
        {
            StaffSalaryUpdateModel dbModel = new StaffSalaryUpdateModel();

            var StaffMedicalPayment = DbAHD.dataStaffSalary.Where(x => x.StaffSalaryGUID == PK).FirstOrDefault();
            var dbStaffMedicalPayment = DbAHD.Entry(StaffMedicalPayment).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbStaffMedicalPayment, dbModel);

            if (StaffMedicalPayment.dataStaffSalaryRowVersion.SequenceEqual(dbModel.dataStaffSalaryRowVersion))
            {
                return Json(DbAHD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbAHD, dbModel, "LanguagesContainer"));
        }

        #endregion

        #region Cylce Flow Step

        public ActionResult CycleSalaryFlowStepIndex(Guid PK)
        {
            if (!CMS.HasAction(Permissions.SalaryCycle.Access, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            SalaryCyclePeriodUpdateModel model = new SalaryCyclePeriodUpdateModel();
            var Mymodel = DbAHD.dataSalaryCycle.Where(x => x.SalaryCycleGUID == PK).FirstOrDefault();
            model.SalaryCycleGUID = PK;
            model.CycleName = Mymodel.CycleName;



            return View("~/Areas/AHD/Views/SalaryCyclePeriod/Index.cshtml", model);
        }

        public JsonResult CycleSalaryFlowStepDataTable(DataTableRecievedOptions options, Guid PK)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<SalaryCycleStepDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<SalaryCycleStepDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.StaffSalaryProcess.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix

            var All = (
                from a in DbAHD.dataSalaryCycleStep.Where(x => x.SalaryCycleGUID == PK).AsExpandable()
                join b in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.LastFlowStatusGUID equals b.ValueGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.CreateByGUID equals c.UserGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()
                join d in DbAHD.v_StaffProfileInformation on a.UserGUID equals d.UserGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()
                join e in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.StepGUID equals e.ValueGUID into LJ4
                from R4 in LJ4.DefaultIfEmpty()
                select new SalaryCycleStepDataTableModel
                {
                    SalaryCycleStepGUID = a.SalaryCycleStepGUID.ToString(),
                    SalaryCycleGUID = a.SalaryCycleGUID.ToString(),
                    Active = a.Active,
                    ActionDate = a.ActionDate,
                    LastFlowStatusGUID = a.LastFlowStatusGUID.ToString(),
                    Status = R1.ValueDescription,
                    Step = R4.ValueDescription,
                    SortID = R4.codeTablesValues.SortID,
                    CreateBy = R2.FirstName + " " + R2.Surname,
                    AccessLevl = 0,
                    //FlowStatusGUID = a.FlowStatusGUID,
                    //PaymentDurationName = a.PaymentDurationName,
                    //TotalStaffConfirm = a.dataStaffMedicalPayment1.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Confirmed).Count(),
                    //TotalStaffNotConfirm = a.dataStaffMedicalPayment1.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Pending).Count(),

                    //OrderId = a.OrderId,
                    //CreateDate = a.CreateDate,
                    dataSalaryCycleStepRowVersion = a.dataSalaryCycleStepRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<SalaryCycleStepDataTableModel> Result = Mapper.Map<List<SalaryCycleStepDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).OrderBy(x => x.SortID).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        [Route("AHD/StaffSalaryStep/Update/{PK}")]
        public ActionResult StaffSalaryStep(Guid PK)
        {
            if (!CMS.HasAction(Permissions.SalaryCycle.Access, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            var mystep = DbAHD.dataSalaryCycleStep.Where(x => x.SalaryCycleStepGUID == PK).FirstOrDefault();
            SalaryCyclePeriodUpdateModel model = DbAHD.dataSalaryCycle.Where(x => x.SalaryCycleGUID == mystep.SalaryCycleGUID).Select(x => new SalaryCyclePeriodUpdateModel { SalaryCycleGUID = x.SalaryCycleGUID }).FirstOrDefault();
            if (mystep.StepGUID == CycleSalaryFlowStep.NotifyStaffToConfirmBankInformation)
            {
                return View("~/Areas/AHD/Views/SalaryCyclePeriod/StaffBankDestenation/StaffBankDestenation.cshtml", null, model);
            }
            else if (mystep.StepGUID == CycleSalaryFlowStep.ImportHQSalary)
            {
                return View("~/Areas/AHD/Views/SalaryCyclePeriod/HQSalary/HQSalary.cshtml", null, model);
            }
            else if (mystep.StepGUID == CycleSalaryFlowStep.DangerPay)
            {
                return View("~/Areas/AHD/Views/SalaryCyclePeriod/DangerPay/DangerPay.cshtml", null, model);
            }
            else if (mystep.StepGUID == CycleSalaryFlowStep.Overtime)
            {

                return View("~/Areas/AHD/Views/SalaryCyclePeriod/Overtime/Overtime.cshtml", null, model);
            }
            else if (mystep.StepGUID == CycleSalaryFlowStep.MIP)
            {
                return View("~/Areas/AHD/Views/SalaryCyclePeriod/Medical/MedicalIndex.cshtml", null, model);

            }
            else if (mystep.StepGUID == CycleSalaryFlowStep.Billing)
            {
                return View("~/Areas/AHD/Views/SalaryCyclePeriod/Billing/Billing.cshtml", null, model);
            }
            else if (mystep.StepGUID == CycleSalaryFlowStep.SalaryOverview)
            {
                return View("~/Areas/AHD/Views/StaffSalary/Index.cshtml", null, model);
            }
            return Json(DbAHD.PermissionError());
        }

        public ActionResult StaffSalaryStepOtherUpdate(Guid PK)
        {
            var myModel = DbAHD.dataStaffSalary.Where(x => x.StaffSalaryGUID == PK).FirstOrDefault(); ;
            StaffSalaryUpdateModel model = Mapper.Map(myModel, new StaffSalaryUpdateModel());
            var staff = DbAHD.v_staffCoreDataOverview.Where(x => x.UserGUID == myModel.UserGUID).FirstOrDefault();
            model.StaffName = staff.FullName;

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("StaffMedicalPayment", "StaffMedicalPayments", new { Area = "AHD" }));
            return PartialView("~/Areas/AHD/Views/StaffSalary/_StaffSalaryPaymentUpdateModal.cshtml", model);

        }






        #endregion

        #region Billing
        public ActionResult CycleSalaryStaffBillingDataTable(DataTableRecievedOptions options, Guid PK)
        {
            //ss
            if (!CMS.HasAction(Permissions.SalaryCycle.Create, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<CycleSalryBillingDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<CycleSalryBillingDataTableModel>(DataTable.Filters);
            }
            var _cycleSalary = DbAHD.dataSalaryCycle.Where(x => x.SalaryCycleGUID == PK).FirstOrDefault();
            var _codeMonth = DbAHD.codeMonth.Where(x => x.MonthCode == _cycleSalary.MonthName).FirstOrDefault();
            // var _dangerPay = DbAHD.dataDangerPayInformation.Where(x => x.MonthId == _codeMonth.MonthId && x.YearId == _cycleSalary.Year).FirstOrDefault();
            var All = (
               from a in DbAHD.v_UserPhoneBillsPerMonth.AsExpandable().Where(x => x.BillForYear == _cycleSalary.Year
               && x.BillForMonth == _codeMonth.MonthId)
               join b in DbAHD.StaffCoreData.Where(x => x.Active) on a.UserGUID equals b.UserGUID into LJ1
               from R1 in LJ1.DefaultIfEmpty()

               select new CycleSalryBillingDataTableModel
               {
                   UserGUID = a.UserGUID.ToString(),
                   StaffName = a.StaffName,
                   SIMWarehouseDescription = a.SIMWarehouseDescription,
                   DeductFromSalaryAmount = a.TotalDeductFromSalaryAmount,
                   SumPayInCashAmount = a.SumPayInCashAmount,
                   Active = true,



                   StaffCoreDataRowVersion = R1.StaffCoreDataRowVersion
               }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<CycleSalryBillingDataTableModel> Result = Mapper.Map<List<CycleSalryBillingDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);

        }

        #endregion
        #region Update Steps
        public JsonResult UpdateStaffDangerPaymentToSalary(Guid _SalaryCycleGUID)
        {
            DateTime ExecutionTime = DateTime.Now;
            var _salary = DbAHD.dataSalaryCycle.Where(x => x.SalaryCycleGUID == _SalaryCycleGUID).FirstOrDefault();
            var _month = DbAHD.codeMonth.Where(x => x.MonthCode == _salary.MonthName).FirstOrDefault();
            var _dangerPeriod = DbAHD.dataDangerPayInformation.Where(x => x.YearId == _salary.Year && x.MonthId == _month.MonthId).FirstOrDefault();

            var result = DbAHD.v_NationalStaffDangerPayment.Where(x => x.DangerPayInformationGUID == _dangerPeriod.DangerPayInformationGUID).ToList();
            var priResult = DbAHD.dataNationalStaffDangerPay.Where(x => x.IsPayed == false && x.DangerPayInformationGUID != _dangerPeriod.DangerPayInformationGUID
            && x.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Confirmed
            && x.ParentDangerPayInformationGUID == _dangerPeriod.DangerPayInformationGUID).ToList();
            var resultUser = result.Select(x => x.UserGUID).Distinct().ToList();
            var _priResult = priResult.Select(x => x.UserGUID).Distinct().ToList();
            var _allSalaries = DbAHD.dataStaffSalary.Where(x => x.SalaryCycleGUID == _SalaryCycleGUID &&

            (resultUser.Contains((Guid)x.UserGUID) || _priResult.Contains(x.UserGUID))).ToList();

            _allSalaries.ForEach(x => x.DangerPay = (double)(result.Where(f => f.UserGUID == x.UserGUID).FirstOrDefault().ActualDangerPayAmount != null ? (priResult.Where(f => f.UserGUID == x.UserGUID).Select(f => f.ActualDangerPayAmount).Sum() > 0 ? priResult.Where(f => f.UserGUID == x.UserGUID).Select(f => f.ActualDangerPayAmount).Sum() + result.Where(f => f.UserGUID == x.UserGUID).FirstOrDefault().ActualDangerPayAmount : result.Where(f => f.UserGUID == x.UserGUID).FirstOrDefault().ActualDangerPayAmount) : (priResult.Where(f => f.UserGUID == x.UserGUID).Select(f => f.ActualDangerPayAmount).Sum() > 0 ? priResult.Where(f => f.UserGUID == x.UserGUID).Select(f => f.ActualDangerPayAmount).Sum() : 0)));
            //foreach (var item in _allSalaries)
            //{
            //    var _temp1=result.Where(x => x.UserGUID == item.UserGUID).Select(x=>x.ActualDangerPayAmount).Sum();
            //    var _temp2 = priResult.Where(x => x.UserGUID == item.UserGUID).Select(x => x.ActualDangerPayAmount).Sum();
            //    item.DangerPay = _temp1 != null ? (double)(_temp1 + _temp2 ): (double)_temp2;

            ////    item.DangerPay=(double) (result.Where(x=>x.UserGUID==item.UserGUID) != null ? 
            ////        (priResult.Where(x => x.UserGUID == item.UserGUID).Select(x => x.ActualDangerPayAmount).Sum() > 0 ?
            ////        priResult.Where(x => x.UserGUID == item.UserGUID).Select(x => x.ActualDangerPayAmount).Sum() + 
            ////        result.Where(x => x.UserGUID == item.UserGUID).FirstOrDefault().ActualDangerPayAmount :
            ////        result.Where(x => x.UserGUID == item.UserGUID).FirstOrDefault().ActualDangerPayAmount) : 
            ////        (priResult.Where(x => x.UserGUID == item.UserGUID).Select(x => x.ActualDangerPayAmount).Sum() 
            ////        > 0 ? priResult.Where(x => x.UserGUID == item.UserGUID).Select(x => x.ActualDangerPayAmount).Sum() : 0));
            //}
            DbAHD.UpdateBulk(_allSalaries, Permissions.SalaryCycle.CreateGuid, ExecutionTime, DbCMS);
            Guid _dangerStepGUID = Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E7354");
            Guid _confimed = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7577");
            var _step = DbAHD.dataSalaryCycleStep.Where(x => x.StepGUID == _dangerStepGUID && x.SalaryCycleGUID == _salary.SalaryCycleGUID).FirstOrDefault();
            _step.LastFlowStatusGUID = _confimed;

            DbAHD.Update(_step, Permissions.SalaryCycle.CreateGuid, ExecutionTime, DbCMS);

            DbAHD.SaveChanges();
            DbCMS.SaveChanges();

            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateStaffOvertimeToSalary(Guid _SalaryCycleGUID)
        {
            DateTime ExecutionTime = DateTime.Now;
            var _salary = DbAHD.dataSalaryCycle.Where(x => x.SalaryCycleGUID == _SalaryCycleGUID).FirstOrDefault();
            var _month = DbAHD.codeMonth.Where(x => x.MonthCode == _salary.MonthName).FirstOrDefault();
            var _cycle = DbAHD.dataOvertimeMonthCycle.Where(x => x.Year == _salary.Year && x.Month == _month.MonthName).FirstOrDefault();

            var result = DbAHD.dataOvertimeMonthCycleStaff.Where(x => x.OvertimeMonthCycleGUID ==
                    _cycle.OvertimeMonthCycleGUID && x.LastFlowStatusGUID == OvertimeFlowStatus.Approved && (x.TotalPay != null && x.TotalPay < 0)).ToList();

            var resultUser = result.Select(x => x.UserGUID).Distinct().ToList();

            var _allSalaries = DbAHD.dataStaffSalary.Where(x => x.SalaryCycleGUID == _SalaryCycleGUID &&

            (resultUser.Contains((Guid)x.UserGUID))).ToList();

            _allSalaries.ForEach(x => x.DangerPay = (double)(result.Where(f => f.UserGUID == x.UserGUID).FirstOrDefault().TotalPay));

            DbAHD.UpdateBulk(_allSalaries, Permissions.SalaryCycle.CreateGuid, ExecutionTime, DbCMS);
            Guid _overitemGUID = Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E7335");
            Guid _confimed = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7577");
            var _step = DbAHD.dataSalaryCycleStep.Where(x => x.StepGUID == _overitemGUID && x.SalaryCycleGUID == _salary.SalaryCycleGUID).FirstOrDefault();
            _step.LastFlowStatusGUID = _confimed;

            DbAHD.Update(_step, Permissions.SalaryCycle.CreateGuid, ExecutionTime, DbCMS);

            DbAHD.SaveChanges();
            DbCMS.SaveChanges();

            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateStaffBillingToSalary(Guid _SalaryCycleGUID)
        {
            DateTime ExecutionTime = DateTime.Now;
            var _salary = DbAHD.dataSalaryCycle.Where(x => x.SalaryCycleGUID == _SalaryCycleGUID).FirstOrDefault();
            var _month = DbAHD.codeMonth.Where(x => x.MonthCode == _salary.MonthName).FirstOrDefault();


            var result = DbAHD.v_UserPhoneBillsPerMonth.Where(x => x.BillForYear == _salary.Year &&
                                        x.BillForMonth == _month.MonthId && x.TotalDeductFromSalaryAmount > 0).ToList();

            var resultUser = result.Select(x => x.UserGUID).Distinct().ToList();

            var _allSalaries = DbAHD.dataStaffSalary.Where(x => x.SalaryCycleGUID == _SalaryCycleGUID &&

            (resultUser.Contains((Guid)x.UserGUID))).ToList();

            _allSalaries.ForEach(x => x.PhoneCall = (double)(result.Where(f => f.UserGUID == x.UserGUID).FirstOrDefault().TotalDeductFromSalaryAmount));

            DbAHD.UpdateBulk(_allSalaries, Permissions.SalaryCycle.CreateGuid, ExecutionTime, DbCMS);
            Guid _stepcurr = Guid.Parse("66CD375C-A576-4AA4-8AF4-F43C1C5E7357");
            Guid _confimed = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7577");
            var _step = DbAHD.dataSalaryCycleStep.Where(x => x.StepGUID == _stepcurr && x.SalaryCycleGUID == _salary.SalaryCycleGUID).FirstOrDefault();
            _step.LastFlowStatusGUID = _confimed;

            DbAHD.Update(_step, Permissions.SalaryCycle.CreateGuid, ExecutionTime, DbCMS);

            DbAHD.SaveChanges();
            DbCMS.SaveChanges();

            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateMedicalPaymentToSalary(Guid _SalaryCycleGUID)
        {
            DateTime ExecutionTime = DateTime.Now;
            var _salary = DbAHD.dataSalaryCycle.Where(x => x.SalaryCycleGUID == _SalaryCycleGUID).FirstOrDefault();
            var _month = DbAHD.codeMonth.Where(x => x.MonthCode == _salary.MonthName).FirstOrDefault();


            var result = DbAHD.dataStaffMedicalPayment.Where(x => x.Year == _salary.Year &&
                                        x.Month == _month.MonthCode && x.AmountUSD > 0).ToList();

            var resultUser = result.Select(x => x.UserGUID).Distinct().ToList();

            var _allSalaries = DbAHD.dataStaffSalary.Where(x => x.SalaryCycleGUID == _SalaryCycleGUID &&

            (resultUser.Contains((Guid)x.UserGUID))).ToList();

            _allSalaries.ForEach(x => x.MIP = (double)(result.Where(f => f.UserGUID == x.UserGUID).FirstOrDefault().AmountUSD));

            DbAHD.UpdateBulk(_allSalaries, Permissions.SalaryCycle.CreateGuid, ExecutionTime, DbCMS);
            Guid _stepcurr = Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E7356");
            Guid _confimed = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7577");
            var _step = DbAHD.dataSalaryCycleStep.Where(x => x.StepGUID == _stepcurr && x.SalaryCycleGUID == _salary.SalaryCycleGUID).FirstOrDefault();
            _step.LastFlowStatusGUID = _confimed;

            DbAHD.Update(_step, Permissions.SalaryCycle.CreateGuid, ExecutionTime, DbCMS);

            DbAHD.SaveChanges();
            DbCMS.SaveChanges();

            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult CalculateStaffSalary(Guid _SalaryCycleGUID)
        {
            DateTime ExecutionTime = DateTime.Now;
            var _salary = DbAHD.dataSalaryCycle.Where(x => x.SalaryCycleGUID == _SalaryCycleGUID).FirstOrDefault();
            var _salaries = DbAHD.dataStaffSalary.Where(x => x.SalaryCycleGUID == _SalaryCycleGUID).ToList();
            _salaries.ForEach(x => x.TotalSalary = (
                             (x.HQSalary + x.AdvancedSalary + x.AddOthers + x.DangerPay + x.Overtime + x.MIP)
                            - (x.PhoneCall + x.Internet4G + x.DeductionOthers + x.DeductionOthers)));

            DbAHD.UpdateBulk(_salaries, Permissions.SalaryCycle.CreateGuid, ExecutionTime, DbCMS);
            //Guid _stepcurr = Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E7356");
            //Guid _confimed = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7577");
            //var _step = DbAHD.dataSalaryCycleStep.Where(x => x.StepGUID == _stepcurr && x.SalaryCycleGUID == _salary.SalaryCycleGUID).FirstOrDefault();
            //_step.LastFlowStatusGUID = _confimed;

            //DbAHD.Update(_step, Permissions.SalaryCycle.CreateGuid, ExecutionTime, DbCMS);

            DbAHD.SaveChanges();
            DbCMS.SaveChanges();

            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
        }


        #endregion


   


    }
}