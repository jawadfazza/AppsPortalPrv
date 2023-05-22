using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Library.MimeDetective;
using AppsPortal.PCR.ViewModels;
using AppsPortal.ViewModels;
using AutoMapper;
using FineUploader;
using LinqKit;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PCR_DAL.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.PCR.Controllers
{
    public class PartnerReportsController : PCRBaseController
    {
        #region Partner Reports

        [Route("PCR/PartnerReports/")]
        public ActionResult PartnerReportsIndex()
        {
            if (!CMS.HasAction(Permissions.UploadPartnerReport.Access, Apps.PCR))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/PCR/Views/PartnerReports/Index.cshtml");
        }

        [Route("PCR/PartnerReportsDataTable/")]
        public JsonResult PartnerReportsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<PartnerReportsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<PartnerReportsDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.CommunityCenterCode.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var All = (from a in DbPCR.dataPartnerReport.AsExpandable().Where(x => x.codePartnerCenter.Active && AuthorizedList.Contains(x.codePartnerCenter.OrganizationInstanceGUID + "," + x.codePartnerCenter.DutyStationGUID.ToString()))
                       join b in DbPCR.codeOrganizationsInstancesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.codePartnerCenter.OrganizationInstanceGUID equals b.OrganizationInstanceGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join c in DbPCR.codeDutyStationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.codePartnerCenter.DutyStationGUID equals c.DutyStationGUID into LJ2
                       from R2 in LJ2.DefaultIfEmpty()
                       join d in DbPCR.codePartnerCenterLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.PartnerCenterGUID equals d.PartnerCenterGUID into LJ3
                       from R3 in LJ3.DefaultIfEmpty()
                       join e in DbPCR.codeOrganizations on R1.codeOrganizationsInstances.OrganizationGUID equals e.OrganizationGUID

                       select new PartnerReportsDataTableModel
                       {
                           PartnerReportGUID = a.PartnerReportGUID,
                           OrganizationInstanceGUID = R1.OrganizationInstanceGUID.ToString(),
                           DutyStationGUID = R2.DutyStationGUID.ToString(),
                           PartnerCenterDescription = R3.PartnerCenterDescription,
                           DutyStationDescription = R2.DutyStationDescription,
                           OrganizationInstanceDescription = e.OrganizationShortName,
                           Active = a.Active,
                           dataPartnerReportRowVersion = a.dataPartnerReportRowVersion

                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<PartnerReportsDataTableModel> Result = Mapper.Map<List<PartnerReportsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("PCR/PartnerReports/Create/")]
        public ActionResult PartnerReportCreate()
        {
            if (!CMS.HasAction(Permissions.UploadPartnerReport.Create, Apps.PCR))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/PCR/Views/PartnerReports/PartnerReport.cshtml", new PartnerReportUpdateModel());
        }

        [Route("PCR/PartnerReports/Update/{PK}")]
        public ActionResult PartnerReportUpdate(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.PartnerCenter.Access, Apps.PCR))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var model = (from a in DbPCR.dataPartnerReport.WherePK(PK)
                         join b in DbPCR.codePartnerCenter on a.PartnerCenterGUID equals b.PartnerCenterGUID
                         select new PartnerReportUpdateModel
                         {
                             PartnerReportGUID = a.PartnerReportGUID,
                             PartnerCenterGUID = a.PartnerCenterGUID,
                             OrganizationInstanceGUID = b.OrganizationInstanceGUID,
                             DutyStationGUID = b.DutyStationGUID,
                             Active = a.Active,
                             dataPartnerReportRowVersion = a.dataPartnerReportRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("PartnerReport", "PartnerReports", new { Area = "PCR" }));

            return View("~/Areas/PCR/Views/PartnerReports/PartnerReport.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PartnerReportCreate(PartnerReportUpdateModel model)
        {
            model.DutyStationGUID = DbPCR.codePartnerCenter.Find(model.PartnerCenterGUID).DutyStationGUID;
            if (!CMS.HasAction(Permissions.CommunityCenterCode.Create, Apps.PCR, model.OrganizationInstanceGUID + "," + model.DutyStationGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActivePartnerReport(model)) return PartialView("~/Areas/PCR/Views/PartnerReports/_PartnerReportForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            Guid EntityPK = Guid.NewGuid();

            dataPartnerReport PartnerReport = Mapper.Map(model, new dataPartnerReport());
            PartnerReport.PartnerReportGUID = EntityPK;
            DbPCR.Create(PartnerReport, Permissions.UploadPartnerReport.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.PartnerReportCompiledsDataTable, ControllerContext, "PartnerReportCompiledsContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.UploadPartnerReport.Create, Apps.PCR, new UrlHelper(Request.RequestContext).Action("Create", "PartnerReports", new { Area = "PCR" })), Container = "PartnerReportFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.UploadPartnerReport.Update, Apps.PCR), Container = "PartnerReportFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.UploadPartnerReport.Delete, Apps.PCR), Container = "PartnerReportFormControls" });

            try
            {
                DbPCR.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPCR.SingleCreateMessage(DbPCR.PrimaryKeyControl(PartnerReport), DbPCR.RowVersionControls(new List<dataPartnerReport>() { PartnerReport }), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbPCR.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PartnerReportUpdate(PartnerReportUpdateModel model)
        {
            var guid = DbPCR.codePartnerCenter.Find(model.PartnerCenterGUID);
            if (!CMS.HasAction(Permissions.CommunityCenterCode.Update, Apps.PCR, guid.OrganizationInstanceGUID + "," + guid.DutyStationGUID))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActivePartnerReport(model)) return PartialView("~/Areas/PCR/Views/PartnerReports/_PartnerReportForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataPartnerReport PartnerReport = Mapper.Map(model, new dataPartnerReport());
            DbPCR.Update(PartnerReport, Permissions.UploadPartnerReport.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbPCR.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPCR.SingleUpdateMessage(null, null, DbPCR.RowVersionControls(PartnerReport, new dataPartnerReportCompiled())));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyPartnerReport(model.PartnerReportGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPCR.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PartnerReportDelete(dataPartnerReport model)
        {
            //var guid = DbPCR.dataPartnerReport.Where(x => x.PartnerReportGUID == model.PartnerReportGUID ).FirstOrDefault();
            //if (!CMS.HasAction(Permissions.PartnerCenter.Delete, Apps.PCR, guid.codePartnerCenter.OrganizationInstanceGUID + "," + guid.codePartnerCenter.DutyStationGUID))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            List<dataPartnerReport> DeletedPartnerReport = DeletePartnerReports(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.UploadPartnerReport.Restore, Apps.PCR), Container = "PartnerReportFormControls" });

            try
            {
                int CommitedRows = DbPCR.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPCR.SingleDeleteMessage(CommitedRows, DeletedPartnerReport.FirstOrDefault(), "PartnerReportCompiledsContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyPartnerReport(model.PartnerReportGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPCR.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PartnerReportRestore(dataPartnerReport model)
        {
            //var guid = DbPCR.dataPartnerReport.Where(x => x.PartnerReportGUID == model.PartnerReportGUID).FirstOrDefault();
            //if (!CMS.HasAction(Permissions.PartnerCenter.Restore, Apps.PCR, guid.codePartnerCenter.OrganizationInstanceGUID + "," + guid.codePartnerCenter.DutyStationGUID))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            if (ActivePartnerReport(model))
            {
                return Json(DbPCR.RecordExists());
            }

            List<dataPartnerReport> RestoredPartnerReports = RestorePartnerReports(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.UploadPartnerReport.Create, Apps.PCR, new UrlHelper(Request.RequestContext).Action("PartnerReportCreate", "Configuration", new { Area = "PCR" })), Container = "PartnerReportFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.UploadPartnerReport.Update, Apps.PCR), Container = "PartnerReportFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.UploadPartnerReport.Delete, Apps.PCR), Container = "PartnerReportFormControls" });

            try
            {
                int CommitedRows = DbPCR.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPCR.SingleRestoreMessage(CommitedRows, RestoredPartnerReports, DbPCR.PrimaryKeyControl(RestoredPartnerReports.FirstOrDefault()), Url.Action(DataTableNames.PartnerReportCompiledsDataTable, Portal.GetControllerName(ControllerContext)), "PartnerReportCompiledsContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyPartnerReport(model.PartnerReportGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPCR.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult PartnerReportsDataTableDelete(List<dataPartnerReport> models)
        {
            if (!CMS.HasAction(Permissions.UploadPartnerReport.Delete, Apps.PCR))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPartnerReport> DeletedPartnerReports = DeletePartnerReports(models);

            try
            {
                DbPCR.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPCR.PartialDeleteMessage(DeletedPartnerReports, models, DataTableNames.PartnerReportsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPCR.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult PartnerReportsDataTableRestore(List<dataPartnerReport> models)
        {
            if (!CMS.HasAction(Permissions.UploadPartnerReport.Restore, Apps.PCR))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPartnerReport> RestoredPartnerReports = RestorePartnerReports(models);

            try
            {
                DbPCR.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPCR.PartialRestoreMessage(RestoredPartnerReports, models, DataTableNames.PartnerReportsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPCR.ErrorMessage(ex.Message));
            }
        }

        private List<dataPartnerReport> DeletePartnerReports(List<dataPartnerReport> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataPartnerReport> DeletedPartnerReports = new List<dataPartnerReport>();


            string baseQuery = "SELECT PCR.dataPartnerReport.PartnerReportGUID,CONVERT(varchar(50), code.codePartnerCenter.OrganizationInstanceGUID) +','+ CONVERT(varchar(50), code.codePartnerCenter.DutyStationGUID)  as C2 , PCR.dataPartnerReport.dataPartnerReportRowVersion FROM " +
            " PCR.dataPartnerReport Inner join code.codePartnerCenter on PCR.dataPartnerReport.PartnerCenterGUID = code.codePartnerCenter.PartnerCenterGUID" +
           " where PCR.dataPartnerReport.PartnerReportGUID in (" + string.Join("','", models.Select(x => "'" + x.PartnerReportGUID + "'").ToArray()) + ")";

            string query = DbPCR.QueryBuilder(models, Permissions.CommunityCenterCode.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbPCR.Database.SqlQuery<dataPartnerReport>(query).ToList();
            foreach (var record in Records)
            {
                DeletedPartnerReports.Add(DbPCR.Delete(record, ExecutionTime, Permissions.CommunityCenterCode.DeleteGuid, DbCMS));
            }

            var CompiledReports = DeletedPartnerReports.SelectMany(a => a.dataPartnerReportCompiled).Where(l => l.Active).ToList();
            foreach (var CompiledReport in CompiledReports)
            {
                DbPCR.Delete(CompiledReport, ExecutionTime, Permissions.CommunityCenterCode.DeleteGuid, DbCMS);
                DbPCR.dataPartnerReportDetail.Where(x => x.PartnerReportCompiledGUID == CompiledReport.PartnerReportCompiledGUID).ToList().ForEach(x => x.Active = false);

            }
            return DeletedPartnerReports;
        }

        private List<dataPartnerReport> RestorePartnerReports(List<dataPartnerReport> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataPartnerReport> RestoredPartnerReports = new List<dataPartnerReport>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "SELECT PCR.dataPartnerReport.PartnerReportGUID,CONVERT(varchar(50), code.codePartnerCenter.OrganizationInstanceGUID) +','+ CONVERT(varchar(50), code.codePartnerCenter.DutyStationGUID)  as C2 , PCR.dataPartnerReport.dataPartnerReportRowVersion FROM " +
                " PCR.dataPartnerReport Inner join code.codePartnerCenter on PCR.dataPartnerReport.PartnerCenterGUID = code.codePartnerCenter.PartnerCenterGUID" +
                " where PCR.dataPartnerReport.PartnerReportGUID in (" + string.Join("','", models.Select(x => "'" + x.PartnerReportGUID + "'").ToArray()) + ")";
            //string baseQuery = "";
            string query = DbPCR.QueryBuilder(models, Permissions.CommunityCenterCode.RestoreGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbPCR.Database.SqlQuery<dataPartnerReport>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActivePartnerReport(record))
                {
                    RestoredPartnerReports.Add(DbPCR.Restore(record, Permissions.CommunityCenterCode.RestoreGuid, Permissions.CommunityCenterCode.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var CompiledReports = RestoredPartnerReports.SelectMany(x => x.dataPartnerReportCompiled.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var CompiledReport in CompiledReports)
            {
                DbPCR.Restore(CompiledReport, Permissions.CommunityCenterCode.DeleteGuid, Permissions.CommunityCenterCode.RestoreGuid, RestoringTime, DbCMS);
                DbPCR.dataPartnerReportDetail.Where(x => x.PartnerReportCompiledGUID == CompiledReport.PartnerReportCompiledGUID).ToList().ForEach(x => x.Active = false);
            }

            return RestoredPartnerReports;
        }

        private JsonResult ConcurrencyPartnerReport(Guid PK)
        {
            PartnerReportUpdateModel dbModel = new PartnerReportUpdateModel();

            var PartnerReport = DbPCR.dataPartnerReport.Where(x => x.PartnerReportGUID == PK).FirstOrDefault();
            var dbPartnerReport = DbPCR.Entry(PartnerReport).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbPartnerReport, dbModel);

            var CompiledReport = DbPCR.dataPartnerReportCompiled.Where(x => x.PartnerReportGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataPartnerReport.DeletedOn)).FirstOrDefault();
            var dbCompiledReport = DbPCR.Entry(CompiledReport).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbCompiledReport, dbModel);

            if (PartnerReport.dataPartnerReportRowVersion.SequenceEqual(dbModel.dataPartnerReportRowVersion) && CompiledReport.dataPartnerReportCompiledRowVersion.SequenceEqual(dbModel.dataPartnerReportRowVersion))
            {
                return Json(DbPCR.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbPCR, dbModel, "PartnerReportCompiledsContainer"));
        }

        private bool ActivePartnerReport(Object model)
        {
            dataPartnerReport PartnerReport = Mapper.Map(model, new dataPartnerReport());
            int PartnerReportDescription = DbPCR.dataPartnerReport
                                    .Where(x =>
                                                x.PartnerCenterGUID == PartnerReport.PartnerCenterGUID &&
                                                x.Active).Count();
            if (PartnerReportDescription > 0)
            {
                ModelState.AddModelError("PartnerReportDescription", "Partner Center is already exists");
            }
            return (PartnerReportDescription > 0);
        }

        #endregion

        #region Partner Report Compiled

        //[Route("PCR/PartnerReportsDataTable/{PK}")]
        public ActionResult PartnerReportCompiledsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/PCR/Views/PartnerReports/_PartnerReportsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<PartnerReportCompiledsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<PartnerReportCompiledsDataTableModel>(DataTable.Filters);
            }

            var Result = (from a in DbPCR.dataPartnerReportCompiled.AsNoTracking().AsExpandable().Where(x => x.PartnerReportGUID == PK).Where(x => x.ReportGUID == ReportGUIDs.Services)
                          join b in DbPCR.dataFileReport on a.FileReportGUID equals b.FileReportGUID
                          select new PartnerReportCompiledsDataTableModel
                          {
                              FileReportGUID = b.FileReportGUID,
                              //EndDate = a.EndDate,
                              Month = a.EndDate.Value.Month.ToString(),
                              Year = a.EndDate.Value.Year.ToString(),
                              UploadDateTime = a.UploadDateTime,
                              UploadByUser = a.UploadByUser,
                              ErrorFound = b.ErrorFound,
                              ShowReport = b.ShowReport,
                              Active = b.Active,
                              dataFileReportRowVersion = b.dataFileReportRowVersion
                          }).Where(Predicate);

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult PartnerReportCompiledCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.UploadPartnerReport.Create, Apps.PCR))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/PCR/Views/PartnerReports/_PartnerReportUpdateModal.cshtml",
                new dataPartnerReportCompiled { PartnerReportGUID = FK });
        }

        public ActionResult BulkPartnerReportCompiledCreate()
        {
            if (!CMS.HasAction(Permissions.UploadPartnerReport.Create, Apps.PCR))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/PCR/Views/PartnerReports/_BulkPartnerReportUpdateModal.cshtml",
                new dataPartnerReportCompiled());
        }

        public ActionResult PartnerReportCompiledUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.UploadPartnerReport.Access, Apps.PCR))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/PCR/Views/PartnerReports/_PartnerReportUpdateModal.cshtml", DbPCR.dataPartnerReportCompiled.Find(PK));
        }

        //[HttpPost, ValidateAntiForgeryToken]
        //public ActionResult PartnerReportCompiledCreate(dataPartnerReportCompiled model)
        //{
        //    if (!CMS.HasAction(Permissions.PartnerReport.Create, Apps.PCR))
        //    {
        //        throw new HttpException(401, "Unauthorized access");
        //    }
        //    if (!ModelState.IsValid) return PartialView("~/Areas/PCR/Views/PartnerReports/_PartnerReportUpdateModal.cshtml", model);

        //    DateTime ExecutionTime = DateTime.Now;

        //    DbPCR.Create(model, Permissions.PartnerReport.CreateGuid, ExecutionTime, DbCMS);

        //    try
        //    {
        //        DbPCR.SaveChanges();
        //        DbCMS.SaveChanges();
        //        return Json(DbPCR.SingleUpdateMessage(DataTableNames.PartnerReportCompiledsDataTable, DbPCR.PrimaryKeyControl(model), DbPCR.RowVersionControls(Portal.SingleToList(model))));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(DbPCR.ErrorMessage(ex.Message));
        //    }
        //}
        [HttpPost]
        public ActionResult PartnerReportCompiledCreate(dataPartnerReportCompiled PartnerReportCompiled, FineUpload upload)
        {
            if (!CMS.HasAction(Permissions.UploadPartnerReport.Create, Apps.PCR))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/PCR/Views/PartnerReports/_PartnerReportUpdateModal.cshtml", PartnerReportCompiled);
            dataPartnerReportCompiled model = null;
            string[] Reports = new string[] { "00000000-0000-0000-0000-000000000001", "00000000-0000-0000-0000-000000000002", "00000000-0000-0000-0000-000000000004" };
            Guid FileReportGUID = Guid.NewGuid();
            DateTime ExecutionTime = DateTime.Now;

            dataFileReport fileReport = new dataFileReport();
            fileReport.FileReportGUID = FileReportGUID;
            fileReport.ErrorFound = true;
            fileReport.ShowReport = false;
            DbPCR.Create(fileReport, Permissions.UploadPartnerReport.CreateGuid, ExecutionTime, DbCMS);

            foreach (string Report in Reports)
            {
                PartnerReportCompiled.ReportGUID = Guid.Parse(Report);
                if (ActivePartnerReportCompiled(PartnerReportCompiled)) return new FineUploaderResult(false, new { path = "" });
                model = new dataPartnerReportCompiled();
                model.PartnerReportGUID = PartnerReportCompiled.PartnerReportGUID;
                model.PartnerReportCompiledGUID = Guid.NewGuid();
                model.ReportGUID = Guid.Parse(Report);
                model.FileReportGUID = FileReportGUID;
                model.StartDate = PartnerReportCompiled.StartDate;
                model.EndDate = PartnerReportCompiled.EndDate;
                model.UploadByUser = PartnerReportCompiled.UploadByUser;
                model.UploadDateTime = ExecutionTime;

                try
                {
                    DbPCR.Create(model, Permissions.UploadPartnerReport.CreateGuid, ExecutionTime, DbCMS);
                    DbPCR.SaveChanges();
                    DbCMS.SaveChanges();

                    Upload(upload, model.PartnerReportCompiledGUID, Guid.Parse(Report), FileReportGUID, ExecutionTime);
                }
                catch (Exception ex)
                {
                    DbPCR.dataPartnerReportCompiled.RemoveRange(DbPCR.dataPartnerReportCompiled.Where(x => x.FileReportGUID == FileReportGUID).ToList());
                    DbPCR.SaveChanges();
                    return new FineUploaderResult(false, new { path = "check file name" });
                    //return Json(DbPCR.ErrorMessage(ex.Message));
                }
            }
            if (DbPCR.dataPartnerReportCompiled.Where(x => x.ErrorFound.Value && x.FileReportGUID == FileReportGUID).FirstOrDefault() != null) { DbPCR.dataFileReport.Where(x => x.FileReportGUID == FileReportGUID).FirstOrDefault().ErrorFound = true; }
            DbPCR.SaveChanges();
            return new FineUploaderResult(true, new { path = "", success = true });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PartnerReportCompiledUpdate(dataPartnerReportCompiled model)
        {
            if (!CMS.HasAction(Permissions.UploadPartnerReport.Update, Apps.PCR))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActivePartnerReportCompiled(model)) return PartialView("~/Areas/PCR/Views/PartnerReports/_PartnerReportUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbPCR.Update(model, Permissions.UploadPartnerReport.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbPCR.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPCR.SingleUpdateMessage(DataTableNames.PartnerReportCompiledsDataTable,
                    DbPCR.PrimaryKeyControl(model),
                    DbPCR.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyPartnerReportCompiled(model.PartnerReportCompiledGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPCR.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PartnerReportCompiledDelete(dataPartnerReportCompiled model)
        {
            if (!CMS.HasAction(Permissions.UploadPartnerReport.Delete, Apps.PCR))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var models = DbPCR.dataPartnerReportCompiled.AsNoTracking().Where(x => x.FileReportGUID == model.FileReportGUID).ToList();
            List<dataPartnerReportCompiled> DeletedPartnerReportCompileds = DeletePartnerReportCompileds(models);

            try
            {
                DbPCR.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPCR.SingleDeleteMessage(DeletedPartnerReportCompileds, DataTableNames.PartnerReportCompiledsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyPartnerReportCompiled(model.PartnerReportCompiledGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPCR.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PartnerReportCompiledRestore(dataPartnerReportCompiled model)
        {
            if (!CMS.HasAction(Permissions.UploadPartnerReport.Restore, Apps.PCR))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActivePartnerReportCompiled(model))
            {
                return Json(DbPCR.RecordExists());
            }
            var models = DbPCR.dataPartnerReportCompiled.AsNoTracking().Where(x => x.FileReportGUID == model.FileReportGUID).ToList();
            List<dataPartnerReportCompiled> RestoredPartnerReportCompileds = RestorePartnerReportCompileds(models);

            try
            {
                DbPCR.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPCR.SingleRestoreMessage(RestoredPartnerReportCompileds, DataTableNames.PartnerReportCompiledsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyPartnerReportCompiled(model.PartnerReportCompiledGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPCR.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult PartnerReportCompiledsDataTableDelete(List<dataPartnerReportCompiled> models)
        {
            if (!CMS.HasAction(Permissions.UploadPartnerReport.Delete, Apps.PCR))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<string> FileReports = models.Select(x => x.FileReportGUID.Value.ToString()).Distinct().ToList();
            var modelList = DbPCR.dataPartnerReportCompiled.AsNoTracking().Where(x => FileReports.Contains(x.FileReportGUID.ToString())).ToList();
            List<dataPartnerReportCompiled> DeletedPartnerReportCompileds = DeletePartnerReportCompileds(modelList);

            try
            {
                DbPCR.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPCR.PartialDeleteMessage(DeletedPartnerReportCompileds, models, DataTableNames.PartnerReportCompiledsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPCR.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult PartnerReportCompiledsDataTableRestore(List<dataPartnerReportCompiled> models)
        {
            if (!CMS.HasAction(Permissions.UploadPartnerReport.Restore, Apps.PCR))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<string> FileReports = models.Select(x => x.FileReportGUID.Value.ToString()).Distinct().ToList();

            var modelList = DbPCR.dataPartnerReportCompiled.AsNoTracking().Where(x => FileReports.Contains(x.FileReportGUID.ToString())).ToList();
            List<dataPartnerReportCompiled> RestoredPartnerReportCompileds = RestorePartnerReportCompileds(modelList);

            try
            {
                DbPCR.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPCR.PartialRestoreMessage(RestoredPartnerReportCompileds, models, DataTableNames.PartnerReportCompiledsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPCR.ErrorMessage(ex.Message));
            }
        }

        private List<dataPartnerReportCompiled> DeletePartnerReportCompileds(List<dataPartnerReportCompiled> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataPartnerReportCompiled> DeletedPartnerReportCompileds = new List<dataPartnerReportCompiled>();

            string query = DbPCR.QueryBuilder(models, Permissions.UploadPartnerReport.DeleteGuid, SubmitTypes.Delete, "");

            var PartnerReportCompileds = DbPCR.Database.SqlQuery<dataPartnerReportCompiled>(query).ToList();

            foreach (var PartnerReportCompiled in PartnerReportCompileds)
            {

                DeletedPartnerReportCompileds.Add(DbPCR.Delete(PartnerReportCompiled, ExecutionTime, Permissions.UploadPartnerReport.DeleteGuid, DbCMS));
                DbPCR.dataFileReport.Where(x => x.FileReportGUID == PartnerReportCompiled.FileReportGUID).FirstOrDefault().Active = false;
                DbPCR.dataPartnerReportDetail.Where(x => x.PartnerReportCompiledGUID == PartnerReportCompiled.PartnerReportCompiledGUID).ToList().ForEach(x => x.Active = false);
            }


            return DeletedPartnerReportCompileds;
        }

        private List<dataPartnerReportCompiled> RestorePartnerReportCompileds(List<dataPartnerReportCompiled> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataPartnerReportCompiled> RestoredPartnerReportCompileds = new List<dataPartnerReportCompiled>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbPCR.QueryBuilder(models, Permissions.UploadPartnerReport.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var PartnerReportCompileds = DbPCR.Database.SqlQuery<dataPartnerReportCompiled>(query).ToList();
            foreach (var PartnerReportCompiled in PartnerReportCompileds)
            {
                if (!ActivePartnerReportCompiled(PartnerReportCompiled))
                {
                    RestoredPartnerReportCompileds.Add(DbPCR.Restore(PartnerReportCompiled, Permissions.UploadPartnerReport.DeleteGuid, Permissions.UploadPartnerReport.RestoreGuid, RestoringTime, DbCMS));
                    DbPCR.dataFileReport.Where(x => x.FileReportGUID == PartnerReportCompiled.FileReportGUID).FirstOrDefault().Active = true;
                    DbPCR.dataPartnerReportDetail.Where(x => x.PartnerReportCompiledGUID == PartnerReportCompiled.PartnerReportCompiledGUID).ToList().ForEach(x => x.Active = true);
                }
            }

            return RestoredPartnerReportCompileds;
        }

        private JsonResult ConcrrencyPartnerReportCompiled(Guid PK)
        {
            dataPartnerReportCompiled dbModel = new dataPartnerReportCompiled();

            var PartnerReportCompiled = DbPCR.dataPartnerReportCompiled.Where(l => l.PartnerReportCompiledGUID == PK).FirstOrDefault();
            var dbPartnerReportCompiled = DbPCR.Entry(PartnerReportCompiled).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbPartnerReportCompiled, dbModel);

            if (PartnerReportCompiled.dataPartnerReportCompiledRowVersion.SequenceEqual(dbModel.dataPartnerReportCompiledRowVersion))
            {
                return Json(DbPCR.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbPCR, dbModel, "PartnerReportCompiledsContainer"));
        }

        private bool ActivePartnerReportCompiled(dataPartnerReportCompiled model)
        {
            int PartnerReportCompiled = DbPCR.dataPartnerReportCompiled
                                 .Where(x => x.StartDate == model.StartDate && x.EndDate == model.EndDate && x.ReportGUID == model.ReportGUID && x.PartnerReportGUID == model.PartnerReportGUID &&
                                             x.Active).Count();

            if (PartnerReportCompiled > 0)
            {
                ModelState.AddModelError("LanguageID", "Partner Report already Uploaded");
            }

            return (PartnerReportCompiled > 0);
        }

        [HttpGet]
        public ActionResult UploadFiles(Guid PK)
        {
            return PartialView("~/Areas/PCR/Views/PartnerReports/_FileUpload.cshtml", DbPCR.dataFileReport.Find(PK));
        }

        [HttpPost]
        public FineUploaderResult UploadFiles(FineUpload upload, dataPartnerReportCompiled PartnerReportCompiled, dataFileReport dataFile)
        {
            DateTime ExecutionTime = DateTime.Now;
            Guid FileReportGUID = Guid.NewGuid();
            if (dataFile.FileReportGUID == Guid.Empty)
            {
                FileReportGUID = Guid.NewGuid();
                dataFileReport fileReport = new dataFileReport();
                fileReport.FileReportGUID = FileReportGUID;
                fileReport.ErrorFound = true;
                fileReport.ShowReport = false;
                DbPCR.Create(fileReport, Permissions.UploadPartnerReport.CreateGuid, ExecutionTime, DbCMS);
                DbPCR.SaveChanges();
            }
            else
            {
                FileReportGUID = dataFile.FileReportGUID;
            }
            string error = "";
            if (PartnerReportCompiled.PartnerReportCompiledGUID == Guid.Empty)
            {
                dataPartnerReportCompiled model = null;
                //JAWAD
                string[] Reports = new string[] { "00000000-0000-0000-0000-000000000001", "00000000-0000-0000-0000-000000000002", "00000000-0000-0000-0000-000000000004" };
                foreach (string Report in Reports)
                {
                    string _name = upload.FileName.Split('.')[0];
                    if (DbPCR.codePartnerCenterLanguage.Where(x => x.codePartnerCenter.Sequence.ToString() == _name && x.Active).FirstOrDefault() != null)
                    {
                        var Result = DbPCR.dataPartnerReportCompiled.Where(x => x.dataPartnerReport.codePartnerCenter.codePartnerCenterLanguage.Where(y => y.LanguageID == "EN").FirstOrDefault().codePartnerCenter.Sequence.ToString() == _name).FirstOrDefault();
                        if (Result == null)
                        {
                            model = new dataPartnerReportCompiled();
                            dataPartnerReport PartnerReport = new dataPartnerReport();
                            PartnerReport.PartnerReportGUID = Guid.NewGuid();
                            PartnerReport.PartnerCenterGUID = DbPCR.codePartnerCenterLanguage.Where(x => x.codePartnerCenter.Sequence.ToString() == _name && x.Active && x.LanguageID == "EN").FirstOrDefault().PartnerCenterGUID;
                            PartnerReport.Active = true;
                            DbPCR.Create(PartnerReport, Permissions.UploadPartnerReport.CreateGuid, ExecutionTime, DbCMS);
                            PartnerReportCompiled.PartnerReportGUID = PartnerReport.PartnerReportGUID;
                            PartnerReportCompiled.PartnerReportCompiledGUID = Guid.NewGuid();
                            PartnerReportCompiled.ReportGUID = Guid.Parse(Report);
                            model.StartDate = PartnerReportCompiled.StartDate;
                            model.EndDate = PartnerReportCompiled.EndDate;
                            model.UploadByUser = PartnerReportCompiled.UploadByUser;
                            model.UploadDateTime = PartnerReportCompiled.UploadDateTime;
                            DbPCR.Create(PartnerReportCompiled, Permissions.UploadPartnerReport.CreateGuid, ExecutionTime, DbCMS);
                            if (ActivePartnerReportCompiled(PartnerReportCompiled)) { return new FineUploaderResult(false); }

                            DbPCR.SaveChanges();
                        }
                        else
                        {
                            //if (FileReportGUID != Guid.Empty) { PartnerReportCompiled = DbPCR.dataPartnerReportCompiled.AsNoTracking().Where(x => x.FileReportGUID == FileReportGUID && x.ReportGUID.ToString() == Report).FirstOrDefault(); }
                            Guid PartnerCenterGUID = DbPCR.codePartnerCenterLanguage.Where(x => x.codePartnerCenter.Sequence.ToString() == _name && x.Active && x.LanguageID == "EN").FirstOrDefault().PartnerCenterGUID;
                            model = DbPCR.dataPartnerReportCompiled.Where(x => x.dataPartnerReport.PartnerCenterGUID == PartnerCenterGUID && x.ReportGUID.ToString() == Report && x.StartDate == PartnerReportCompiled.StartDate && x.EndDate == PartnerReportCompiled.EndDate && x.Active).FirstOrDefault();
                            if (model == null)
                            {
                                model = new dataPartnerReportCompiled();
                                Mapper.Map(PartnerReportCompiled, model);
                                model.PartnerReportGUID = Result.PartnerReportGUID;
                                model.PartnerReportCompiledGUID = Guid.NewGuid();
                                model.ReportGUID = Guid.Parse(Report);
                                model.FileReportGUID = FileReportGUID;
                                model.StartDate = PartnerReportCompiled.StartDate;
                                model.EndDate = PartnerReportCompiled.EndDate;
                                model.UploadByUser = PartnerReportCompiled.UploadByUser;
                                model.UploadDateTime = PartnerReportCompiled.UploadDateTime;
                                DbPCR.Create(model, Permissions.UploadPartnerReport.CreateGuid, ExecutionTime, DbCMS);
                            }


                            if (ActivePartnerReportCompiled(PartnerReportCompiled)) { return new FineUploaderResult(false); }
                            DbPCR.SaveChanges();
                        }

                        error = Upload(upload, model.PartnerReportCompiledGUID, Guid.Parse(Report), model.FileReportGUID.Value, ExecutionTime);

                    }
                    else
                    {
                        return new FineUploaderResult(false);
                    }
                }
            }
            else
            {
                error = Upload(upload, PartnerReportCompiled.PartnerReportCompiledGUID, PartnerReportCompiled.ReportGUID, PartnerReportCompiled.FileReportGUID.Value, ExecutionTime);
            }
            return new FineUploaderResult(true, new { Error = error, path = "", success = true });
        }

        public string Upload(FineUpload upload, Guid PK, Guid Reports, Guid FileReportGUID, DateTime ExecutionTime)
        {
            var PartnerReportCompiled = new dataPartnerReportCompiled();
            PartnerReportCompiled = DbPCR.dataPartnerReportCompiled.Where(x => x.PartnerReportCompiledGUID == PK).FirstOrDefault();
            DbPCR.dataPartnerReportDetail.RemoveRange(DbPCR.dataPartnerReportDetail.Where(x => x.PartnerReportCompiledGUID == PartnerReportCompiled.PartnerReportCompiledGUID).ToList());
            DbPCR.SaveChanges();

            string FilePath = ConfigurationManager.AppSettings["DataFolder"] + "\\Uploads\\PCR\\" + FileReportGUID + ".xlsx";
            string _ext = Path.GetExtension(upload.FileName).Split('.')[1];
            if (_ext.ToLower() == "xls" || _ext.ToLower() == "xlsx")
            {

                if (!System.IO.File.Exists(FilePath))
                {
                    using (var fileStream = System.IO.File.Create(FilePath))
                    {
                        upload.InputStream.Seek(0, SeekOrigin.Begin);
                        upload.InputStream.CopyTo(fileStream);
                    }
                }
                //    if (FileTypeValidator.IsExcel(upload.InputStream))
                //{
                //upload.SaveAs(FilePath);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(FilePath)))
                {
                    bool ErrorFound = false;
                    int SheetIndex = 0;
                    int val = 0;
                    if (Reports == ReportGUIDs.Vulnerabilities) { SheetIndex = 1; }
                    if (Reports == ReportGUIDs.Services) { SheetIndex = 2; }
                    if (Reports == ReportGUIDs.Referral) { SheetIndex = 2; }

                    ExcelWorksheet workSheet = package.Workbook.Worksheets[SheetIndex];
                    workSheet.View.TabSelected = true;
                    if (workSheet != null)
                    {
                        int totalRows = workSheet.Dimension.End.Row;

                        List<dataPartnerReportDetail> PartnerReportDetail = new List<dataPartnerReportDetail>();
                        var CategoryReport = DbPCR.codeCategoryReport.Where(x => x.CategoryLevel == 3 && x.ReportGUID == PartnerReportCompiled.ReportGUID).ToList();

                        if (Reports == ReportGUIDs.Referral)
                        {

                            int TotalVal = 0;
                            var Resons = DbPCR.codeAggregation.Where(x => x.ReportGUID == Reports).ToList();
                            foreach (var cat in CategoryReport)
                            {
                                //External Referral && Referral to ORV
                                try { val = workSheet.Cells[cat.ColumnCharacter + cat.RowSequance].Value == null ? 0 : Convert.ToInt32(workSheet.Cells[cat.ColumnCharacter + cat.RowSequance].Value); }
                                catch { val = 0; }

                                dataPartnerReportDetail partnerReportDetail = new dataPartnerReportDetail
                                {
                                    AggregationValue = val,
                                    AggregationGUID = Guid.Parse("00000000-0000-0000-0000-000000000028"),
                                    CategoryReportGUID = cat.CategoryReportGUID,
                                    PartnerReportCompiledGUID = PK,
                                    PartnerMonthlyReportDetailGUID = Guid.NewGuid(),
                                    Active = true
                                };
                                PartnerReportDetail.Add(partnerReportDetail);
                                //Sum of the External Referral should match the aggregation below

                                if (cat.CategoryReportGUID == Guid.Parse("00000000-0000-0000-0000-000000000229"))
                                {
                                    for (int i = cat.RowSequance.Value; i < (cat.RowSequance.Value + 5); i++)
                                    {
                                        var Reson = Resons.Where(x => x.AggregationDescription.Trim().Contains(workSheet.Cells["E" + i].Value.ToString().Trim())).FirstOrDefault();
                                        if (Reson != null) // ---> E71
                                        {
                                            try { val = workSheet.Cells[Reson.ColumnCharacter + i].Value == null ? 0 : Convert.ToInt32(workSheet.Cells[Reson.ColumnCharacter + i].Value); }
                                            catch { val = 0; }
                                            partnerReportDetail = new dataPartnerReportDetail
                                            {
                                                AggregationValue = val,
                                                AggregationGUID = Reson.AggregationGUID,
                                                CategoryReportGUID = cat.CategoryReportGUID,
                                                PartnerReportCompiledGUID = PK,
                                                PartnerMonthlyReportDetailGUID = Guid.NewGuid(),
                                                Details = workSheet.Cells["D" + i].Value.ToString().Replace(" ", "_").Replace("-", "_"),
                                                Active = true
                                            };
                                            TotalVal += val;
                                            PartnerReportDetail.Add(partnerReportDetail);
                                            ErrorFound = ValidateReferal(workSheet, i, ErrorFound);
                                        }
                                    }


                                    //check the Totals of Number of Individuals Referred  Match With External Referral
                                    if (TotalVal != PartnerReportDetail.Where(x => x.CategoryReportGUID.ToString() == "00000000-0000-0000-0000-000000000229" && x.AggregationGUID == Guid.Parse("00000000-0000-0000-0000-000000000028")).FirstOrDefault().AggregationValue)
                                    {
                                        SetWorkSeetError(workSheet, "C73", "C79", "Totals of Number of Individuals Referred not Match With External Referral");
                                        SetWorkSeetError(workSheet, "F73", "C79", "Totals of Number of Individuals Referred not Match With External Referral");
                                        SetWorkSeetError(workSheet, "F74", "C79", "Totals of Number of Individuals Referred not Match With External Referral");
                                        SetWorkSeetError(workSheet, "F75", "C79", "Totals of Number of Individuals Referred not Match With External Referral");
                                        SetWorkSeetError(workSheet, "F76", "C79", "Totals of Number of Individuals Referred not Match With External Referral");
                                        SetWorkSeetError(workSheet, "F77", "C79", "Totals of Number of Individuals Referred not Match With External Referral");
                                    }
                                }

                            }
                            PartnerReportCompiled.UploadByUser = CMS.GetFullName(UserGUID, "EN");
                            PartnerReportCompiled.UploadDateTime = ExecutionTime;
                            DbPCR.dataPartnerReportDetail.AddRange(PartnerReportDetail);
                            PartnerReportCompiled.ErrorFound = ErrorFound;
                            if (!ErrorFound) { DbPCR.dataFileReport.Where(x => x.FileReportGUID == PartnerReportCompiled.FileReportGUID).FirstOrDefault().ErrorFound = false; }
                            DbPCR.SaveChanges();
                            package.Save();
                        }
                        else
                        {

                            var Aggregations = DbPCR.codeAggregation.Where(x => x.ReportGUID == ReportGUIDs.Vulnerabilities).ToList();

                            foreach (var cat in CategoryReport)
                            {
                                if (cat.CategoryReportGUID == Guid.Parse("00000000-0000-0000-0000-000000000219"))
                                {

                                }
                                string text = "";
                                try
                                {
                                    text = workSheet.Cells[cat.ColumnCharacter + cat.RowSequance].Value.ToString();
                                    if (text == "New registration") { text = "Registered"; }
                                }
                                catch { }
                                if (text.Trim().ToLower().Replace("ed", "").Contains(cat.CategoryDescription.Trim().ToLower().Replace("ed", "")))
                                {
                                    List<dataPartnerReportDetail> PartnerReportDetailRow = new List<dataPartnerReportDetail>();
                                    foreach (var agg in Aggregations)
                                    {
                                        if (workSheet.Cells[agg.ColumnCharacter + cat.RowSequance].Value == null)
                                        {
                                            if (cat.RowSequance >= 33 && Reports == ReportGUIDs.Vulnerabilities)
                                            { }
                                            else
                                            {
                                                ErrorFound = true;
                                                workSheet.Cells[agg.ColumnCharacter + cat.RowSequance].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                workSheet.Cells[agg.ColumnCharacter + cat.RowSequance].Style.Fill.BackgroundColor.SetColor(Color.Red);
                                            }
                                        }
                                        try { val = workSheet.Cells[agg.ColumnCharacter + cat.RowSequance].Value == null ? 0 : Convert.ToInt32(workSheet.Cells[agg.ColumnCharacter + cat.RowSequance].Value); }
                                        catch
                                        {

                                            val = 0;
                                        }
                                        dataPartnerReportDetail partnerReportDetail = new dataPartnerReportDetail
                                        {
                                            AggregationValue = val,
                                            AggregationGUID = agg.AggregationGUID,
                                            CategoryReportGUID = cat.CategoryReportGUID,
                                            PartnerReportCompiledGUID = PK,
                                            PartnerMonthlyReportDetailGUID = Guid.NewGuid(),
                                            Active = true
                                        };
                                        PartnerReportDetail.Add(partnerReportDetail);
                                    }
                                }
                                else
                                {
                                    workSheet.Cells["X" + cat.RowSequance].Value = cat.CategoryDescription + "  { Title Not Match }";
                                    workSheet.Cells["C" + cat.RowSequance.ToString() + ":X" + cat.RowSequance.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    workSheet.Cells["C" + cat.RowSequance.ToString() + ":X" + cat.RowSequance.ToString()].Style.Fill.BackgroundColor.SetColor(Color.Red);
                                    ErrorFound = true;
                                }
                            }

                            PartnerReportCompiled.UploadByUser = CMS.GetFullName(UserGUID, "EN");
                            PartnerReportCompiled.UploadDateTime = ExecutionTime;
                            DbPCR.dataPartnerReportDetail.AddRange(PartnerReportDetail);
                            if (PartnerReportDetail.Count == 0)
                            {
                                package.Save(); PartnerReportCompiled.ErrorFound = true; DbPCR.SaveChanges();
                            }
                            else
                            {
                                DbPCR.SaveChanges();
                                ErrorFound = Validate(PartnerReportCompiled, workSheet, ErrorFound);
                                PartnerReportCompiled.ErrorFound = ErrorFound;

                                DbPCR.SaveChanges();
                                package.Save();
                            }
                        }

                    }
                }
            }
            return FilePath;
        }



        /// <summary>
        /// Set Sheet Error
        /// </summary>
        /// <param name="workSheet"></param>
        /// <param name="colRowError"></param>
        /// <param name="colRowMessage"></param>
        /// <param name="messageStr"></param>
        public void SetWorkSeetError(ExcelWorksheet workSheet, string colRowError, string colRowMessage, string messageStr)
        {
            workSheet.Cells[colRowError].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[colRowError].Style.Fill.BackgroundColor.SetColor(Color.Red);
            if (colRowMessage != "")
            {
                workSheet.Cells[colRowMessage].Value = messageStr;
                workSheet.Cells[colRowMessage].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells[colRowMessage].Style.Fill.BackgroundColor.SetColor(Color.YellowGreen);
            }
        }

        /// <summary>
        /// Validate the Referral Section
        /// </summary>
        /// <param name="workSheet"></param>
        /// <param name="RowError"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool ValidateReferal(ExcelWorksheet workSheet, int RowError, bool status)
        {
            bool Error = status;
            object D = workSheet.Cells["D" + RowError].Value;
            object E = workSheet.Cells["E" + RowError].Value;
            int F = 0;
            try { F = workSheet.Cells["F" + RowError].Value == null ? 0 : Convert.ToInt32(workSheet.Cells["F" + RowError].Value); }
            catch { F = 0; }
            if (F == 0 && E.ToString() != "-" && D.ToString() != "-")
            {
                SetWorkSeetError(workSheet, "F" + RowError, "", "");
                Error = true;
            }
            if (F == 0 && E.ToString() == "-" && D.ToString() != "-")
            {
                SetWorkSeetError(workSheet, "E" + RowError, "", "");
                Error = true;
            }
            if (F == 0 && E.ToString() != "-" && D.ToString() == "-")
            {
                SetWorkSeetError(workSheet, "D" + RowError, "", "");
                Error = true;
            }
            if (F != 0 && E.ToString() == "-" && D.ToString() == "-")
            {
                SetWorkSeetError(workSheet, "D" + RowError, "", "");
                SetWorkSeetError(workSheet, "E" + RowError, "", "");
                Error = true;
            }
            if (F != 0 && E.ToString() != "-" && D.ToString() == "-")
            {
                SetWorkSeetError(workSheet, "D" + RowError, "", "");
                Error = true;
            }
            if (F != 0 && E.ToString() == "-" && D.ToString() != "-")
            {
                SetWorkSeetError(workSheet, "E" + RowError, "", "");
                Error = true;
            }
            return Error;
        }

        /// <summary>
        /// Validate th Excel 
        /// </summary>
        /// <param name="reportCompiled"></param>
        /// <param name="workSheet"></param>
        /// <returns></returns>
        private bool Validate(dataPartnerReportCompiled reportCompiled, ExcelWorksheet workSheet, bool status)
        {

            var Aggregations = DbPCR.codeAggregation.Where(x => x.ReportGUID == ReportGUIDs.Vulnerabilities).ToList();
            var CategoryReport = DbPCR.codeCategoryReport.Where(x => x.CategoryLevel == 3 && x.ReportGUID == reportCompiled.ReportGUID).ToList();
            bool Error = status;

            foreach (var cat in CategoryReport)
            {
                if (cat.CategoryReportGUID == Guid.Parse("00000000-0000-0000-0000-000000000219"))
                {

                }
                var RD = DbPCR.dataPartnerReportDetail.Where(x => x.Active && x.PartnerReportCompiledGUID == reportCompiled.PartnerReportCompiledGUID && x.CategoryReportGUID == cat.CategoryReportGUID).ToList();
                //Check the Assisted value 
                foreach (var agg in Aggregations)
                {
                    if (reportCompiled.ReportGUID == ReportGUIDs.Services)
                    {
                        var Row = RD.Where(x => x.AggregationGUID == agg.AggregationGUID && x.CategoryReportGUID == cat.CategoryReportGUID).FirstOrDefault();
                        if (Row.codeCategoryReport.CategoryDescription.Contains("Assisted"))
                        {
                            string cellStrAssisted = RD.Where(x => x.codeAggregation.AggregationGUID == agg.AggregationGUID).FirstOrDefault().codeAggregation.ColumnCharacter + (RD.FirstOrDefault().codeCategoryReport.RowSequance.Value);
                            string cellStrRegistered = RD.Where(x => x.codeAggregation.AggregationGUID == agg.AggregationGUID).FirstOrDefault().codeAggregation.ColumnCharacter + (RD.FirstOrDefault().codeCategoryReport.RowSequance.Value - 1);
                            int cellIntAssisted = 0;
                            int cellIntRegistered = 0;
                            try { cellIntAssisted = workSheet.Cells[cellStrAssisted].Value == null ? 0 : Convert.ToInt32(workSheet.Cells[cellStrAssisted].Value); } catch { cellIntAssisted = 0; }
                            try { cellIntRegistered = workSheet.Cells[cellStrRegistered].Value == null ? 0 : Convert.ToInt32(workSheet.Cells[cellStrRegistered].Value); } catch { cellIntRegistered = 0; }

                            if (cellIntAssisted > cellIntRegistered)
                            {
                                workSheet.Cells[cellStrAssisted].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                workSheet.Cells[cellStrAssisted].Style.Fill.BackgroundColor.SetColor(Color.Red);
                                workSheet.Cells[cellStrAssisted].Style.Font.Bold = true;
                                Error = true;
                            }
                        }
                    }
                }
                //Family < Individuals
                if (RD.Where(x => x.codeAggregation.GroupGUID == Group.Family && x.codeAggregation.GenderGUID == null).Select(x => x.AggregationValue).Sum() >
                    RD.Where(x => x.codeAggregation.GroupGUID == Group.Individuals && x.codeAggregation.GenderGUID == null).Select(x => x.AggregationValue).Sum())
                {
                    if (reportCompiled.ReportGUID == ReportGUIDs.Vulnerabilities && RD.FirstOrDefault().codeCategoryReport.RowSequance.Value >= 33)
                    {
                        //do nothing
                    }
                    else
                    {
                        string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Family).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;

                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                        Error = true;
                    }
                }
                //Total Individuals
                int val1 = RD.Where(x => x.codeAggregation.GroupGUID == Group.Individuals && x.codeAggregation.GenderGUID == null).Select(x => x.AggregationValue).Sum();
                int val2 = RD.Where(x => x.codeAggregation.GroupGUID == Group.Individuals && x.codeAggregation.GenderGUID != null).Select(x => x.AggregationValue).Sum();
                if (val1 != val2)
                {
                    string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Individuals && x.codeAggregation.GenderGUID == null).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                    workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                    workSheet.Cells[cellStr].Style.Font.Bold = true;
                    Error = true;
                }
                //Total Individuals to Age Male
                val1 = RD.Where(x => x.codeAggregation.GroupGUID == Group.Individuals && x.codeAggregation.GenderGUID == Gender.Male).Select(x => x.AggregationValue).Sum();
                val2 = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.GenderGUID == Gender.Male).Select(x => x.AggregationValue).Sum();
                if (val1 != val2)
                {
                    string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Individuals && x.codeAggregation.GenderGUID == Gender.Male).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                    workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                    workSheet.Cells[cellStr].Style.Font.Bold = true;
                    foreach (var cell in RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.GenderGUID == Gender.Male).ToList())
                    {
                        cellStr = cell.codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                    }
                    Error = true;
                }
                //Total Individuals to Age Female
                val1 = RD.Where(x => x.codeAggregation.GroupGUID == Group.Individuals && x.codeAggregation.GenderGUID == Gender.Female).Select(x => x.AggregationValue).Sum();
                val2 = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.GenderGUID == Gender.Female).Select(x => x.AggregationValue).Sum();
                if (val1 != val2)
                {
                    string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Individuals && x.codeAggregation.GenderGUID == Gender.Female).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                    workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                    workSheet.Cells[cellStr].Style.Font.Bold = true;
                    foreach (var cell in RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.GenderGUID == Gender.Female).ToList())
                    {
                        cellStr = cell.codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                    }
                    Error = true;
                }
                //Total Individuals to Profile Male
                val1 = RD.Where(x => x.codeAggregation.GroupGUID == Group.Individuals && x.codeAggregation.GenderGUID == Gender.Male).Select(x => x.AggregationValue).Sum();
                val2 = RD.Where(x => x.codeAggregation.GroupGUID == Group.Profile && x.codeAggregation.GenderGUID == Gender.Male).Select(x => x.AggregationValue).Sum();
                if (val1 != val2)
                {
                    string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Individuals && x.codeAggregation.GenderGUID == Gender.Male).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                    workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                    workSheet.Cells[cellStr].Style.Font.Bold = true;
                    foreach (var cell in RD.Where(x => x.codeAggregation.GroupGUID == Group.Profile && x.codeAggregation.GenderGUID == Gender.Male).ToList())
                    {
                        cellStr = cell.codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                    }
                    Error = true;
                }
                //Total Individuals to Profile Female
                val1 = RD.Where(x => x.codeAggregation.GroupGUID == Group.Individuals && x.codeAggregation.GenderGUID == Gender.Female).Select(x => x.AggregationValue).Sum();
                val2 = RD.Where(x => x.codeAggregation.GroupGUID == Group.Profile && x.codeAggregation.GenderGUID == Gender.Female).Select(x => x.AggregationValue).Sum();
                if (val1 != val2)
                {
                    string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Individuals && x.codeAggregation.GenderGUID == Gender.Female).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                    workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                    workSheet.Cells[cellStr].Style.Font.Bold = true;
                    foreach (var cell in RD.Where(x => x.codeAggregation.GroupGUID == Group.Profile && x.codeAggregation.GenderGUID == Gender.Female).ToList())
                    {
                        cellStr = cell.codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                    }
                    Error = true;
                }
                //Elderly at risk Vulnerability sheet: Rows 14 & 15: the accepted age groups are only 60+
                if (cat.ParentCategoryReportGUID.ToString() == "00000000-0000-0000-0000-000000000004")
                {
                    //0 - 17  Male
                    int val = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_17_Male).Select(x => x.AggregationValue).Sum();
                    if (val > 0)
                    {
                        string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_17_Male).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                        workSheet.Cells["Y" + RD.FirstOrDefault().codeCategoryReport.RowSequance].Value = "0 - 17  Male should be 0 value";
                        Error = true;

                    }
                    //18 - 59  Male
                    val = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_18_59_Male).Select(x => x.AggregationValue).Sum();
                    if (val > 0)
                    {
                        string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_18_59_Male).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                        workSheet.Cells["Z" + RD.FirstOrDefault().codeCategoryReport.RowSequance].Value = "18 - 59  Male should be 0 value";
                        Error = true;

                    }
                    //0 - 17  Female
                    val = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_17_Female).Select(x => x.AggregationValue).Sum();
                    if (val > 0)
                    {
                        string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_17_Female).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                        workSheet.Cells["AA" + RD.FirstOrDefault().codeCategoryReport.RowSequance].Value = "0 - 17  Female should be 0 value";
                        Error = true;
                    }
                    //18 - 59  Female
                    val = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_18_59_Female).Select(x => x.AggregationValue).Sum();
                    if (val > 0)
                    {
                        string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_18_59_Female).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                        workSheet.Cells["AB" + RD.FirstOrDefault().codeCategoryReport.RowSequance].Value = "18 - 59  Female should be 0 value";
                        Error = true;
                    }
                }
                // Vulnerability sheet
                // Child at risk Vulnerability sheet: Rows from 16 to 23: the accepted age group are 0-17 and 18-59 only
                if (cat.ParentCategoryReportGUID.ToString() == "00000000-0000-0000-0000-000000000005")
                {
                    // +60  Male
                    int val = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_60_Male).Select(x => x.AggregationValue).Sum();
                    if (val > 0)
                    {
                        string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_60_Male).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                        workSheet.Cells["Y" + RD.FirstOrDefault().codeCategoryReport.RowSequance].Value = "+60  Male should be 0 value (the accepted age group are 0-17 and 18-59 only)";
                        Error = true;

                    }
                    // +60 Female
                    val = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_60_Female).Select(x => x.AggregationValue).Sum();
                    if (val > 0)
                    {
                        string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_60_Female).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                        workSheet.Cells["Z" + RD.FirstOrDefault().codeCategoryReport.RowSequance].Value = "+60  Female should be 0 value (the accepted age group are 0-17 and 18-59 only)";
                        Error = true;
                    }
                }
                //Woman at risk Vulnerability sheet: Rows 24 & 25: the accepted gender is female only and the accepted age groups are 18-59 and 60+ only
                if (cat.ParentCategoryReportGUID.ToString() == "00000000-0000-0000-0000-000000000006" || cat.ParentCategoryReportGUID.ToString() == "00000000-0000-0000-0000-000000000007")
                {
                    // 0 - 17  Male
                    int val = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_17_Male).Select(x => x.AggregationValue).Sum();
                    if (val > 0)
                    {
                        string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_17_Male).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                        workSheet.Cells["Y" + RD.FirstOrDefault().codeCategoryReport.RowSequance].Value = "0 - 17  Male should be 0 value";
                        Error = true;
                    }
                    // 0 - 17 Female
                    val = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_17_Female).Select(x => x.AggregationValue).Sum();
                    if (val > 0)
                    {
                        string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_17_Female).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                        workSheet.Cells["Z" + RD.FirstOrDefault().codeCategoryReport.RowSequance].Value = "0 - 17 Female should be 0 value";
                        Error = true;
                    }
                    // 18 - 59 Male
                    val = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_18_59_Male).Select(x => x.AggregationValue).Sum();
                    if (val > 0)
                    {
                        string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_18_59_Male).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                        workSheet.Cells["AA" + RD.FirstOrDefault().codeCategoryReport.RowSequance].Value = "18 - 59 Male should be 0 value";
                        Error = true;
                    }
                    //60+ Male
                    val = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_60_Male).Select(x => x.AggregationValue).Sum();
                    if (val > 0)
                    {
                        string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_60_Male).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                        workSheet.Cells["AB" + RD.FirstOrDefault().codeCategoryReport.RowSequance].Value = "60+ Male should be 0 value";
                        Error = true;
                    }
                }
                //Single male parent or caregiver  Vulnerability sheet: Row  28 the accepted gender is male only and the accepted age groups are 18-59 and 60+ only
                if (cat.ParentCategoryReportGUID.ToString() == "00000000-0000-0000-0000-000000000010")
                {
                    // 0 - 17  Male
                    int val = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_17_Male).Select(x => x.AggregationValue).Sum();
                    if (val > 0)
                    {
                        string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_17_Male).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                        workSheet.Cells["Y" + RD.FirstOrDefault().codeCategoryReport.RowSequance].Value = "0 - 17  Male should be 0 value ( the accepted gender is male only and the accepted age groups are 18-59 and 60+ only)";
                        Error = true;
                    }
                    // 0 - 17 Female
                    val = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_17_Female).Select(x => x.AggregationValue).Sum();
                    if (val > 0)
                    {
                        string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_17_Female).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                        workSheet.Cells["Z" + RD.FirstOrDefault().codeCategoryReport.RowSequance].Value = "0 - 17 Female should be 0 value ( the accepted gender is male only and the accepted age groups are 18-59 and 60+ only)";
                        Error = true;
                    }
                    val = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_18_59_Female).Select(x => x.AggregationValue).Sum();
                    if (val > 0)
                    {
                        string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_18_59_Female).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                        workSheet.Cells["AA" + RD.FirstOrDefault().codeCategoryReport.RowSequance].Value = "18 - 59 Female should be 0 value ( the accepted gender is male only and the accepted age groups are 18-59 and 60+ only)";
                        Error = true;
                    }
                    val = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_60_Female).Select(x => x.AggregationValue).Sum();
                    if (val > 0)
                    {
                        string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_60_Female).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                        workSheet.Cells["AB" + RD.FirstOrDefault().codeCategoryReport.RowSequance].Value = "60 Female should be 0 value ( the accepted gender is male only and the accepted age groups are 18-59 and 60+ only)";
                        Error = true;
                    }

                }
                // Services Sheet
                //Child Protection && Education , Services Sheet: Rows from 5 to 12 and rows from 19 to 26: Accepted age groups are 0-17 only.
                if (cat.ParentCategoryReportGUID.ToString() == "00000000-0000-0000-0000-000000000041" || cat.ParentCategoryReportGUID.ToString() == "00000000-0000-0000-0000-000000000043")
                {
                    // 18 - 59 Male
                    int val = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_18_59_Male).Select(x => x.AggregationValue).Sum();
                    if (val > 0)
                    {
                        string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_18_59_Male).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                        workSheet.Cells["Z" + RD.FirstOrDefault().codeCategoryReport.RowSequance].Value = "18 - 59 Male should be 0 value (Accepted age groups are 0-17 only)";
                        Error = true;
                    }
                    //60+ Male
                    val = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_60_Male).Select(x => x.AggregationValue).Sum();
                    if (val > 0)
                    {
                        string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_60_Male).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                        workSheet.Cells["AA" + RD.FirstOrDefault().codeCategoryReport.RowSequance].Value = "60+ Male should be 0 value (Accepted age groups are 0-17 only)";
                        Error = true;
                    }

                    // 18 - 59 Female
                    val = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_18_59_Female).Select(x => x.AggregationValue).Sum();
                    if (val > 0)
                    {
                        string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_18_59_Female).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                        workSheet.Cells["X" + RD.FirstOrDefault().codeCategoryReport.RowSequance].Value = "18 - 59 Female should be 0 value (Accepted age groups are 0-17 only)";
                        Error = true;
                    }
                    //60+ Female
                    val = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_60_Female).Select(x => x.AggregationValue).Sum();
                    if (val > 0)
                    {
                        string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_60_Female).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                        workSheet.Cells["AB" + RD.FirstOrDefault().codeCategoryReport.RowSequance].Value = "60+ Female should be 0 value (Accepted age groups are 0-17 only)";
                        Error = true;
                    }
                }
                //Social and recreational activities for adults , Services Sheet: Rows 43 & 44: Accepted age groups are 18-59 and 60+ only
                if (cat.CategoryReportGUID.ToString() == "00000000-0000-0000-0000-000000000087" || cat.CategoryReportGUID.ToString() == "00000000-0000-0000-0000-000000000088")
                {
                    // 0 - 17  Male
                    int val = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_17_Male).Select(x => x.AggregationValue).Sum();
                    if (val > 0)
                    {
                        string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_17_Male).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                        workSheet.Cells["Y" + RD.FirstOrDefault().codeCategoryReport.RowSequance].Value = "0 - 17  Male should be 0 value";
                        Error = true;

                    }
                    // 0 - 17 Female
                    val = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_17_Female).Select(x => x.AggregationValue).Sum();
                    if (val > 0)
                    {
                        string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_17_Female).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                        workSheet.Cells["Z" + RD.FirstOrDefault().codeCategoryReport.RowSequance].Value = "0 - 17 Female should be 0 value";
                        Error = true;
                    }
                }
                //Home-based care attendant  programme for older persons , Services Sheet: Rows 51 & 52: Accepted age groups are 60+ only.     
                if (cat.CategoryReportGUID.ToString() == "00000000-0000-0000-0000-000000000095" || cat.CategoryReportGUID.ToString() == "00000000-0000-0000-0000-000000000096")
                {
                    //0 - 17  Male  , Services Sheet:Rows 51 & 52: Accepted age groups are 60+ only.     
                    int val = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_17_Male).Select(x => x.AggregationValue).Sum();
                    if (val > 0)
                    {
                        string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_17_Male).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                        workSheet.Cells["Y" + RD.FirstOrDefault().codeCategoryReport.RowSequance].Value = "0 - 17  Male should be 0 value";
                        Error = true;
                    }
                    //18 - 59  Male
                    val = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_18_59_Male).Select(x => x.AggregationValue).Sum();
                    if (val > 0)
                    {
                        string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_18_59_Male).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                        workSheet.Cells["Z" + RD.FirstOrDefault().codeCategoryReport.RowSequance].Value = "18 - 59  Male should be 0 value";
                        Error = true;
                    }
                    //0 - 17  Female
                    val = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_17_Female).Select(x => x.AggregationValue).Sum();
                    if (val > 0)
                    {
                        string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_17_Female).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                        workSheet.Cells["AA" + RD.FirstOrDefault().codeCategoryReport.RowSequance].Value = "0 - 17  Female should be 0 value";
                        Error = true;
                    }
                    //18 - 59  Female
                    val = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_18_59_Female).Select(x => x.AggregationValue).Sum();
                    if (val > 0)
                    {
                        string cellStr = RD.Where(x => x.codeAggregation.GroupGUID == Group.Age && x.codeAggregation.AggregationGUID == AgeGroup.Age_18_59_Female).FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                        workSheet.Cells["AB" + RD.FirstOrDefault().codeCategoryReport.RowSequance].Value = "18 - 59  Female should be 0 value";
                        Error = true;
                    }
                }

                //Vulnerability sheet: 
                //-Sum of cells 33:38 should equal the value of cell D6.
                if (cat.CategoryReportGUID.ToString() == "00000000-0000-0000-0000-000000000039")
                {
                    //Channels
                    int valChannels = DbPCR.dataPartnerReportDetail.Where(x => x.Active && x.PartnerReportCompiledGUID == reportCompiled.PartnerReportCompiledGUID && x.codeAggregation.AggregationGUID == Aggregation.family && x.codeCategoryReport.ParentCategoryReportGUID.ToString() == "00000000-0000-0000-0000-000000000221").Select(x => x.AggregationValue).Sum();
                    int valRegistered = RD.Where(x => x.CategoryReportGUID.ToString() == "00000000-0000-0000-0000-000000000039" && x.codeAggregation.AggregationGUID == Aggregation.family).Select(x => x.AggregationValue).Sum();
                    if (valChannels != valRegistered)
                    {
                        string cellStr = RD.Where(x => x.codeAggregation.AggregationGUID == Aggregation.family && x.CategoryReportGUID.ToString() == "00000000-0000-0000-0000-000000000039").FirstOrDefault().codeAggregation.ColumnCharacter + RD.FirstOrDefault().codeCategoryReport.RowSequance;
                        workSheet.Cells[cellStr].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        workSheet.Cells[cellStr].Style.Font.Bold = true;
                        workSheet.Cells["Z" + RD.FirstOrDefault().codeCategoryReport.RowSequance].Value = "Total Channels not Equal to Registered families";
                        Error = true;
                    }
                }
            }
            // Sum cells 44 to 48 should equal the value of cell B49.
            int ValB49 = workSheet.Cells["B49"].Value == null ? 0 : Convert.ToInt32(workSheet.Cells["B49"].Value);
            int ValD6 = workSheet.Cells["D6"].Value == null ? 0 : Convert.ToInt32(workSheet.Cells["D6"].Value);
            int ValB44To48 = 0;
            for (int i = 44; i <= 48; i++)
            {
                ValB44To48 += workSheet.Cells["B49"].Value == null ? 0 : Convert.ToInt32(workSheet.Cells["B" + i].Value);
            }
            if (ValB49 != ValD6)
            {
                workSheet.Cells["B49"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells["B49"].Style.Fill.BackgroundColor.SetColor(Color.Red);
                workSheet.Cells["B49"].Style.Font.Bold = true;
                workSheet.Cells["B50"].Value = "Total Address B49 not Equal to Registered families D6";
            }
            if (ValB49 != ValB44To48)
            {
                workSheet.Cells["B49"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells["B49"].Style.Fill.BackgroundColor.SetColor(Color.Red);
                workSheet.Cells["B49"].Style.Font.Bold = true;
                workSheet.Cells["B51"].Value = "Total Address Val B44 To B48 not Equal to Total in B49";
            }
            return Error;


        }

        public class Gender
        {
            public static Guid Male = Guid.Parse("688B11E0-24FB-44B8-94CE-D8568C4742C7");
            public static Guid Female = Guid.Parse("DBF9D307-CE9F-4029-BD1E-D7AF6739975C");
        }
        public class Group
        {
            public static Guid Family = Guid.Parse("00000000-0000-0000-0000-000000000001");
            public static Guid Individuals = Guid.Parse("00000000-0000-0000-0000-000000000002");
            public static Guid Age = Guid.Parse("00000000-0000-0000-0000-000000000003");
            public static Guid Profile = Guid.Parse("00000000-0000-0000-0000-000000000004");
        }
        public class AgeGroup
        {
            public static Guid Age_17_Male = Guid.Parse("00000000-0000-0000-0000-000000000005");
            public static Guid Age_17_Female = Guid.Parse("00000000-0000-0000-0000-000000000008");
            public static Guid Age_18_59_Male = Guid.Parse("00000000-0000-0000-0000-000000000006");
            public static Guid Age_18_59_Female = Guid.Parse("00000000-0000-0000-0000-000000000009");
            public static Guid Age_60_Male = Guid.Parse("00000000-0000-0000-0000-000000000007");
            public static Guid Age_60_Female = Guid.Parse("00000000-0000-0000-0000-000000000010");
        }
        public class ReportGUIDs
        {
            public static Guid Vulnerabilities = Guid.Parse("00000000-0000-0000-0000-000000000001");
            public static Guid Services = Guid.Parse("00000000-0000-0000-0000-000000000002");
            public static Guid GeneralStatistics = Guid.Parse("00000000-0000-0000-0000-000000000003");
            public static Guid Referral = Guid.Parse("00000000-0000-0000-0000-000000000004");
        }


        #endregion


        #region Publish Reports


        public ActionResult ManageReportVisibilityUpdate()
        {
            return PartialView("~/Areas/PCR/Views/PartnerReports/_ManageReportVisibility.cshtml", new ReportsVisibilityModel());
        }

        [HttpPost]
        public ActionResult ManageReportVisibilityUpdate(ReportsVisibilityModel model)
        {
            if (!CMS.HasAction(Permissions.PublishReports.Access, Apps.PCR))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            DateTime ExecutionTime = DateTime.Now;

            var dataFileReport = (from a in DbPCR.dataPartnerReport.Where(x => x.Active && model.CenterGUIDs_forPublish.Contains(x.PartnerCenterGUID))
                                  join b in DbPCR.dataPartnerReportCompiled.Where(x => x.Active && x.EndDate == model.EndDate_forPublish) on a.PartnerReportGUID equals b.PartnerReportGUID
                                  join c in DbPCR.dataFileReport.Where(x => x.Active) on b.FileReportGUID equals c.FileReportGUID
                                  select c).Distinct().ToList();
            dataFileReport.ForEach(x => x.ShowReport = model.ShowReport_forPublish);

            DbPCR.UpdateBulk(dataFileReport, Permissions.PublishReports.AccessGuid, ExecutionTime, DbCMS);

            try
            {
                DbPCR.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage());
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        public FileResult DownloadFile(string PK)
        {
            string FilePath = ConfigurationManager.AppSettings["DataFolder"] + "\\Uploads\\PCR\\" + PK + ".xlsx";
            byte[] fileBytes = System.IO.File.ReadAllBytes(@FilePath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, PK + ".xlsx");
        }

        #endregion
    }
}
