using AppsPortal.BaseControllers;
using AppsPortal.Data;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Models;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace AppsPortal.Areas.CMS.Controllers
{

    public class CodesController : PortalBaseController
    {
        #region Applications

        [Route("CMS/Codes/Applications/")]
        public ActionResult ApplicationsIndex()
        {
            if (!CMS.HasAction(Permissions.Applications.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/Applications/Index.cshtml");
        }

        [Route("CMS/Codes/ApplicationsDataTable/")]
        public JsonResult ApplicationsDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ApplicationDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ApplicationDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbCMS.codeApplications.AsExpandable()
                       join b in DbCMS.codeApplicationsLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeApplications.DeletedOn) && x.LanguageID == LAN) on a.ApplicationGUID equals b.ApplicationGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ApplicationStatusGUID equals c.ValueGUID into LJ2
                       from R2 in LJ2.DefaultIfEmpty()
                       select new ApplicationDataTableModel
                       {
                           ApplicationGUID = a.ApplicationGUID,
                           ApplicationID = a.ApplicationID,
                           Active = a.Active,
                           ApplicationAcrynom = a.ApplicationAcrynom,
                           SortID = a.SortID == null ? 0 : a.SortID.Value,
                           ApplicationStatus = R2.ValueDescription,
                           codeApplicationsRowVersion = a.codeApplicationsRowVersion,
                           ApplicationDescription = R1.ApplicationDescription
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ApplicationDataTableModel> Result = Mapper.Map<List<ApplicationDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("CMS/Codes/Applications/Create/")]
        public ActionResult ApplicationCreate()
        {
            if (!CMS.HasAction(Permissions.Applications.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/Applications/Application.cshtml", new ApplicationUpdateModel());
        }


        [Route("CMS/Codes/Applications/Update/{PK}")]
        public ActionResult ApplicationUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.Applications.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            var model = (from x in DbCMS.codeApplications.Where(x => x.ApplicationGUID == PK)
                         join l in DbCMS.codeApplicationsLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeApplications.DeletedOn) && x.LanguageID == LAN)
                         on x.ApplicationGUID equals l.ApplicationGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new ApplicationUpdateModel
                         {
                             ApplicationGUID = x.ApplicationGUID,
                             ApplicationID = x.ApplicationID,
                             ApplicationAcrynom = x.ApplicationAcrynom,
                             ApplicationStatusGUID = x.ApplicationStatusGUID,
                             ClientServerAccessibility = x.ServerAccessibility,
                             SortID = x.SortID,
                             Active = x.Active,
                             codeApplicationsRowVersion = x.codeApplicationsRowVersion,
                             ApplicationDescription = R1.ApplicationDescription,
                             codeApplicationsLanguagesRowVersion = R1.codeApplicationsLanguagesRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("Applications", "Codes", new { Area = "CMS" }));
            model.ServerAccessibility = model.ClientServerAccessibility.Split(',').ToList();
            return View("~/Areas/CMS/Views/Codes/Applications/Application.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ApplicationCreate(ApplicationUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Applications.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid || ActiveApplication(model)) return PartialView("~/Areas/CMS/Views/Codes/Applications/_ApplicationForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            Guid EntityPK = Guid.NewGuid();
            if (model.ApplicationGUID != Guid.Empty)
            {
                EntityPK = model.ApplicationGUID;
            }

            codeApplications Application = Mapper.Map(model, new codeApplications());
            Application.ServerAccessibility = string.Join(",", model.ServerAccessibility);
            Application.ApplicationGUID = EntityPK;
            DbCMS.Create(Application, Permissions.Applications.CreateGuid, ExecutionTime);

            codeApplicationsLanguages Language = Mapper.Map(model, new codeApplicationsLanguages());
            Language.ApplicationGUID = EntityPK;
            DbCMS.Create(Language, Permissions.ApplicationsLanguages.CreateGuid, ExecutionTime);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ApplicationLanguagesDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Applications.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("Application/Create", "Codes", new { Area = "CMS" })), Container = "ApplicationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Applications.Update, Apps.CMS), Container = "ApplicationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Applications.Delete, Apps.CMS), Container = "ApplicationFormControls" });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleCreateMessage(DbCMS.PrimaryKeyControl(Application), DbCMS.RowVersionControls(Application, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ApplicationUpdate(ApplicationUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Applications.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid || ActiveApplication(model)) return PartialView("~/Areas/CMS/Views/Codes/Applications/_ApplicationForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeApplications Application = Mapper.Map(model, new codeApplications());
            Application.ServerAccessibility = string.Join(",", model.ServerAccessibility);
            DbCMS.Update(Application, Permissions.Applications.UpdateGuid, ExecutionTime);

            var Language = DbCMS.codeApplicationsLanguages.Where(x => x.ApplicationGUID == model.ApplicationGUID && x.LanguageID == LAN && x.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.ApplicationGUID = Application.ApplicationGUID;
                DbCMS.Create(Language, Permissions.ApplicationsLanguages.CreateGuid, ExecutionTime);
            }
            else if (Language.ApplicationDescription != model.ApplicationDescription)
            {
                Language = Mapper.Map(model, Language);
                DbCMS.Update(Language, Permissions.ApplicationsLanguages.UpdateGuid, ExecutionTime);
            }

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, DbCMS.RowVersionControls(Application, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyApplication(model.ApplicationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ApplicationDelete(codeApplications model)
        {
            if (!CMS.HasAction(Permissions.Applications.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<codeApplications> DeletedApplications = DeleteApplications(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.Applications.Restore, Apps.CMS), Container = "ApplicationFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(CommitedRows, DeletedApplications.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyApplication(model.ApplicationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ApplicationRestore(codeApplications model)
        {
            if (!CMS.HasAction(Permissions.Applications.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (ActiveApplication(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeApplications> RestoredApplications = RestoreApplications(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Applications.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("Application/Create", "Codes", new { Area = "CMS" })), Container = "ApplicationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Applications.Update, Apps.CMS), Container = "ApplicationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Applications.Restore, Apps.CMS), Container = "ApplicationFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(CommitedRows, RestoredApplications, DbCMS.PrimaryKeyControl(RestoredApplications.FirstOrDefault()), Url.Action(DataTableNames.ApplicationLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyApplication(model.ApplicationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ApplicationsDataTableDelete(List<codeApplications> models)
        {
            if (!CMS.HasAction(Permissions.Applications.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeApplications> DeletedApplications = DeleteApplications(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedApplications, models, DataTableNames.ApplicationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ApplicationsDataTableRestore(List<codeApplications> models)
        {
            if (!CMS.HasAction(Permissions.Applications.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeApplications> RestoredApplications = RestoreApplications(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredApplications, models, DataTableNames.ApplicationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeApplications> DeleteApplications(List<codeApplications> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<codeApplications> DeletedApplications = new List<codeApplications>();

            //THIS IS FOR PERMISSION TEST//
            //DO NOT DELETE THIS CODE
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = (from a in DbCMS.codeApplications
            //                    from f in DbCMS.codeApplicationsFactorForTest
            //                    where a.ApplicationGUID == f.ApplicationGUID
            //                    select new { a.ApplicationGUID, a.codeApplicationsRowVersion, f.OperationGUID, f.OrganizationGUID, f.CountryGUID }).AsQueryable().ToString();

            string query = DbCMS.QueryBuilder(models, Permissions.Applications.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbCMS.Database.SqlQuery<codeApplications>(query).ToList();
            foreach (var record in Records)
            {
                DeletedApplications.Add(DbCMS.Delete(record, ExecutionTime, Permissions.Applications.DeleteGuid));
            }

            var Languages = DeletedApplications.SelectMany(a => a.codeApplicationsLanguages).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Delete(language, ExecutionTime, Permissions.ApplicationsLanguages.DeleteGuid);
            }
            return DeletedApplications;
        }

        private List<codeApplications> RestoreApplications(List<codeApplications> models)
        {
            Guid DeleteActionGUID = Permissions.Applications.DeleteGuid;
            Guid RestoreActionGUID = Permissions.Applications.RestoreGuid;
            DateTime RestoringTime = DateTime.Now;

            List<codeApplications> RestoredApplications = new List<codeApplications>();

            //THIS IS FOR PERMISSION TEST//
            //DO NOT DELETE THIS CODE
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = (from a in DbCMS.codeApplications
            //                    from f in DbCMS.codeApplicationsFactorForTest
            //                    where a.ApplicationGUID == f.ApplicationGUID
            //                    select new
            //                    {
            //                        a.ApplicationGUID,
            //                        a.codeApplicationsRowVersion,
            //                        C2 = f.OperationGUID + "," + f.OrganizationGUID,
            //                    }).AsQueryable().ToString();//.Replace("[C2]", "[FactorsToken]");

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, "");


            var Records = DbCMS.Database.SqlQuery<codeApplications>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveApplication(record))
                {
                    RestoredApplications.Add(DbCMS.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            var Languages = RestoredApplications.SelectMany(x => x.codeApplicationsLanguages.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveApplicationLanguage(language))
                {
                    DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime);
                }
            }

            return RestoredApplications;
        }

        private JsonResult ConcrrencyApplication(Guid PK)
        {
            ApplicationUpdateModel dbModel = new ApplicationUpdateModel();

            var Application = DbCMS.codeApplications.Where(a => a.ApplicationGUID == PK).FirstOrDefault();
            var dbApplication = DbCMS.Entry(Application).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbApplication, dbModel);

            var Language = DbCMS.codeApplicationsLanguages.Where(x => x.ApplicationGUID == PK).Where(p => (p.Active == true ? p.Active : p.DeletedOn == p.codeApplications.DeletedOn) && p.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Application.codeApplicationsRowVersion.SequenceEqual(dbModel.codeApplicationsRowVersion) && Language.codeApplicationsLanguagesRowVersion.SequenceEqual(dbModel.codeApplicationsLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveApplication(Object model)
        {
            codeApplications Application = Mapper.Map(model, new codeApplications());
            codeApplicationsLanguages ApplicationLanguages = Mapper.Map(model, new codeApplicationsLanguages());

            int ApplicationID = DbCMS.codeApplications
                                     .Where(x => x.ApplicationID == Application.ApplicationID &&
                                                 x.ApplicationGUID != Application.ApplicationGUID &&
                                                 x.Active).Count();
            if (ApplicationID > 0)
            {
                ModelState.AddModelError("ApplicationID", "Application ID is already exists");
            }
            int ApplicationAcrynom = DbCMS.codeApplications
                                          .Where(x => x.ApplicationAcrynom == Application.ApplicationAcrynom &&
                                                      x.ApplicationGUID != Application.ApplicationGUID &&
                                                      x.Active).Count();
            if (ApplicationAcrynom > 0)
            {
                ModelState.AddModelError("ApplicationAcrynom", "Application Acrynom is already exists");
            }
            int ApplicationDescription = DbCMS.codeApplicationsLanguages
                                              .Where(x => x.ApplicationDescription == ApplicationLanguages.ApplicationDescription &&
                                                          x.ApplicationGUID != Application.ApplicationGUID &&
                                                          x.LanguageID == LAN &&
                                                          x.Active).Count();

            if (ApplicationDescription > 0)
            {
                ModelState.AddModelError("ApplicationDescription", "Application Description in selected language already exists");
            }
            return (ApplicationID + ApplicationAcrynom + ApplicationDescription) > 0;
        }

        public ActionResult ApplicationAuditHistory(Guid RecordGUID)
        {
            List<Guid> ChildrenGUIDs = new List<Guid>();

            DbCMS.codeApplicationsLanguages.AsNoTracking().Where(x => x.ApplicationGUID == RecordGUID).ToList().ForEach(x => ChildrenGUIDs.Add(x.ApplicationLanguageGUID));
            ChildrenGUIDs.Add(RecordGUID);

            List<Guid> ChildrenActions = new List<Guid>
            {
                Permissions.ApplicationsLanguages.UpdateGuid
            };
            return new AppsPortal.Controllers.AuditController().GetAuditHistoryGlobalizationVersion(RecordGUID, ChildrenActions, ChildrenGUIDs);
        }

        #endregion 

        #region Applications Language
        public ActionResult ApplicationLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/CMS/Views/Codes/Applications/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeApplicationsLanguages, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeApplicationsLanguages>(DataTable.Filters);
            }

            var Result = DbCMS.codeApplicationsLanguages.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.ApplicationGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.ApplicationLanguageGUID,
                                  x.LanguageID,
                                  x.ApplicationDescription,
                                  x.codeApplicationsLanguagesRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result.ToList()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ApplicationLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.Applications.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/Applications/_LanguageUpdateModal.cshtml",
                new codeApplicationsLanguages { ApplicationGUID = FK });
        }

        public ActionResult ApplicationLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.Applications.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/Applications/_LanguageUpdateModal.cshtml", DbCMS.codeApplicationsLanguages.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ApplicationLanguageCreate(codeApplicationsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Applications.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveApplicationLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/Applications/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Create(model, Permissions.ApplicationsLanguages.CreateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.ApplicationLanguagesDataTable,
                   DbCMS.PrimaryKeyControl(model),
                   DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ApplicationLanguageUpdate(codeApplicationsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Applications.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveApplicationLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/Applications/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Update(model, Permissions.ApplicationsLanguages.UpdateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.ApplicationLanguagesDataTable,
                    DbCMS.PrimaryKeyControl(model),
                    DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyApplicationLanguage(model.ApplicationLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ApplicationLanguageDelete(codeApplicationsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Applications.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeApplicationsLanguages> DeletedLanguages = DeleteApplicationLanguages(new List<codeApplicationsLanguages> { model });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.ApplicationLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyApplicationLanguage(model.ApplicationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ApplicationLanguageRestore(codeApplicationsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Applications.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveApplicationLanguage(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeApplicationsLanguages> RestoredLanguages = RestoreApplicationLanguages(Portal.SingleToList(model));

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.ApplicationLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyApplicationLanguage(model.ApplicationLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ApplicationLanguagesDataTableDelete(List<codeApplicationsLanguages> models)
        {
            if (!CMS.HasAction(Permissions.Applications.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeApplicationsLanguages> DeletedLanguages = DeleteApplicationLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.ApplicationLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ApplicationLanguagesDataTableRestore(List<codeApplicationsLanguages> models)
        {

            if (!CMS.HasAction(Permissions.Applications.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeApplicationsLanguages> RestoredLanguages = RestoreApplicationLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.ApplicationLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeApplicationsLanguages> DeleteApplicationLanguages(List<codeApplicationsLanguages> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeApplicationsLanguages> DeletedApplicationLanguages = new List<codeApplicationsLanguages>();

            string query = DbCMS.QueryBuilder(models, Permissions.Applications.DeleteGuid, SubmitTypes.Delete, "");

            var Languages = DbCMS.Database.SqlQuery<codeApplicationsLanguages>(query).ToList();

            foreach (var language in Languages)
            {
                DeletedApplicationLanguages.Add(DbCMS.Delete(language, ExecutionTime, Permissions.ApplicationsLanguages.DeleteGuid));
            }

            return DeletedApplicationLanguages;
        }

        private List<codeApplicationsLanguages> RestoreApplicationLanguages(List<codeApplicationsLanguages> models)
        {
            Guid DeleteActionGUID = Permissions.ApplicationsLanguages.DeleteGuid;
            Guid RestoreActionGUID = Permissions.ApplicationsLanguages.RestoreGuid;
            DateTime RestoringTime = DateTime.Now;

            List<codeApplicationsLanguages> RestoredLanguages = new List<codeApplicationsLanguages>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Languages = DbCMS.Database.SqlQuery<codeApplicationsLanguages>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveApplicationLanguage(language))
                {
                    RestoredLanguages.Add(DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyApplicationLanguage(Guid PK)
        {
            codeApplicationsLanguages dbModel = new codeApplicationsLanguages();

            var Language = DbCMS.codeApplicationsLanguages.Where(x => x.ApplicationLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeApplicationsLanguagesRowVersion.SequenceEqual(dbModel.codeApplicationsLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveApplicationLanguage(codeApplicationsLanguages model)
        {
            int LanguageID = DbCMS.codeApplicationsLanguages
                                  .Where(l => l.LanguageID == model.LanguageID &&
                                              l.ApplicationGUID == model.ApplicationGUID &&
                                              l.ApplicationLanguageGUID != model.ApplicationLanguageGUID &&
                                              l.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Application Name in selected language already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }

        #endregion

        #region Menus
        [Route("CMS/Codes/Menus/")]
        public ActionResult MenusIndex()
        {
            if (!CMS.HasAction(Permissions.NavigationMenus.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/Menus/Index.cshtml");
        }

        [Route("CMS/Codes/MenusDataTable/")]
        public JsonResult MenusDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MenuDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MenuDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbCMS.codeMenus.AsExpandable()
                       join b in DbCMS.codeApplicationsLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeApplications.DeletedOn) && x.LanguageID == LAN) on a.ApplicationGUID equals b.ApplicationGUID
                       join d in DbCMS.codeMenusLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeMenus.DeletedOn) && x.LanguageID == LAN) on a.MenuGUID equals d.MenuGUID into LJ1
                       from l in LJ1.DefaultIfEmpty()
                       select new MenuDataTableModel
                       {
                           MenuGUID = a.MenuGUID,
                           MenuDescription = l.MenuDescription,
                           ApplicationDescription = b.ApplicationDescription,
                           ParentMenuDescription = DbCMS.codeMenusLanguages.Where(pl => pl.LanguageID == LAN && pl.MenuGUID == a.ParentGUID).FirstOrDefault().MenuDescription,
                           ActionUrl = a.ActionUrl,
                           Active = a.Active,
                           IsPublic = a.IsPublic,
                           SortID = a.SortID.ToString(),
                           codeMenusRowVersion = a.codeMenusRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<MenuDataTableModel> Result = Mapper.Map<List<MenuDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("CMS/Codes/Menus/Create/")]
        public ActionResult MenuCreate()
        {
            if (!CMS.HasAction(Permissions.NavigationMenus.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/Menus/Menu.cshtml", new MenuUpdateModel());
        }

        [Route("CMS/Codes/Menus/Update/{PK}")]
        public ActionResult MenuUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.NavigationMenus.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbCMS.codeMenus.WherePK(PK)
                         join b in DbCMS.codeMenusLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeMenus.DeletedOn) && x.LanguageID == LAN) on a.MenuGUID equals b.MenuGUID into LJ1

                         from R1 in LJ1.DefaultIfEmpty()
                         join c in DbCMS.codeActions.Where(a => a.Active) on a.ActionGUID equals c.ActionGUID into LJ2

                         from R2 in LJ2.DefaultIfEmpty()

                         select new MenuUpdateModel
                         {
                             MenuGUID = a.MenuGUID,
                             MenuDescription = R1.MenuDescription,
                             ApplicationGUID = a.ApplicationGUID,
                             ActionCategoryGUID = R2.ActionCategoryGUID,
                             ActionGUID = a.ActionGUID,
                             ParentGUID = a.ParentGUID,
                             ActionUrl = a.ActionUrl,
                             IsPublic = a.IsPublic,
                             SortID = a.SortID,
                             Active = a.Active,
                             codeMenusRowVersion = a.codeMenusRowVersion,
                             codeMenusLanguagesRowVersion = R1.codeMenusLanguagesRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("Menus", "Codes", new { Area = "CMS" }));

            return View("~/Areas/CMS/Views/Codes/Menus/Menu.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MenuCreate(MenuUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.NavigationMenus.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMenu(model)) return PartialView("~/Areas/CMS/Views/Codes/Menus/_MenuForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeMenus Menu = Mapper.Map(model, new codeMenus());
            Menu.MenuGUID = EntityPK;
            DbCMS.Create(Menu, Permissions.NavigationMenus.CreateGuid, ExecutionTime);

            codeMenusLanguages Language = Mapper.Map(model, new codeMenusLanguages());
            Language.MenuGUID = Menu.MenuGUID;
            DbCMS.Create(Language, Permissions.NavigationMenusLanguages.CreateGuid, ExecutionTime);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.MenuLanguagesDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.NavigationMenus.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("Menu/Create", "Codes", new { Area = "CMS" })), Container = "MenusFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.NavigationMenus.Update, Apps.CMS), Container = "MenusFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.NavigationMenus.Delete, Apps.CMS), Container = "MenusFormControls" });


            try
            {
                DbCMS.SaveChanges();

                CMS.BuildPublicMenus();
                List<Guid> OnlineUsers = (List<Guid>)HttpContext.Application["OnlineUsers"];
                foreach (var userGuid in OnlineUsers)
                {
                    CMS.BuildUserMenus(userGuid, LAN);
                }

                return Json(DbCMS.SingleCreateMessage(DbCMS.PrimaryKeyControl(Menu), DbCMS.RowVersionControls(Menu, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MenuUpdate(MenuUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.NavigationMenus.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMenu(model)) return PartialView("~/Areas/CMS/Views/Codes/Menus/_MenuForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeMenus Menu = Mapper.Map(model, new codeMenus());
            DbCMS.Update(Menu, Permissions.NavigationMenus.UpdateGuid, ExecutionTime);

            var Language = DbCMS.codeMenusLanguages.Where(l => l.MenuGUID == model.MenuGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.MenuGUID = Menu.MenuGUID;
                DbCMS.Create(Language, Permissions.NavigationMenusLanguages.CreateGuid, ExecutionTime);
            }
            else if (Language.MenuDescription != model.MenuDescription)
            {
                Language = Mapper.Map(model, Language);
                DbCMS.Update(Language, Permissions.NavigationMenusLanguages.UpdateGuid, ExecutionTime);
            }

            try
            {
                DbCMS.SaveChanges();

                CMS.BuildPublicMenus();
                List<Guid> OnlineUsers = (List<Guid>)HttpContext.Application["OnlineUsers"];
                foreach (var userGuid in OnlineUsers)
                {
                    CMS.BuildUserMenus(userGuid, LAN);
                }

                return Json(DbCMS.SingleUpdateMessage(null, null, DbCMS.RowVersionControls(Menu, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMenu(model.MenuGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MenuDelete(codeMenus model)
        {
            if (!CMS.HasAction(Permissions.NavigationMenus.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeMenus> DeletedMenus = DeleteMenus(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.NavigationMenus.Restore, Apps.CMS), Container = "MenusFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();

                CMS.BuildPublicMenus();
                List<Guid> OnlineUsers = (List<Guid>)HttpContext.Application["OnlineUsers"];
                foreach (var userGuid in OnlineUsers)
                {
                    CMS.BuildUserMenus(userGuid, LAN);
                }

                return Json(DbCMS.SingleDeleteMessage(CommitedRows, DeletedMenus.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMenu(model.MenuGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MenuRestore(codeMenus model)
        {
            if (!CMS.HasAction(Permissions.NavigationMenus.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveMenu(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeMenus> RestoredMenus = RestoreMenus(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.NavigationMenus.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("Menu/Create", "Codes", new { Area = "CMS" })), Container = "MenusFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.NavigationMenus.Update, Apps.CMS), Container = "MenusFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.NavigationMenus.Restore, Apps.CMS), Container = "MenusFormControls" });


            try
            {
                int CommitedRows = DbCMS.SaveChanges();

                CMS.BuildPublicMenus();
                List<Guid> OnlineUsers = (List<Guid>)HttpContext.Application["OnlineUsers"];
                foreach (var userGuid in OnlineUsers)
                {
                    CMS.BuildUserMenus(userGuid, LAN);
                }

                return Json(DbCMS.SingleRestoreMessage(CommitedRows, RestoredMenus, DbCMS.PrimaryKeyControl(RestoredMenus.FirstOrDefault()), Url.Action(DataTableNames.MenuLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMenu(model.MenuGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MenusDataTableDelete(List<codeMenus> models)
        {
            if (!CMS.HasAction(Permissions.NavigationMenus.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeMenus> DeletedMenus = DeleteMenus(models);

            try
            {
                DbCMS.SaveChanges();
                CMS.BuildPublicMenus();
                List<Guid> OnlineUsers = (List<Guid>)HttpContext.Application["OnlineUsers"];
                foreach (var userGuid in OnlineUsers)
                {
                    CMS.BuildUserMenus(userGuid, LAN);
                }

                return Json(DbCMS.PartialDeleteMessage(DeletedMenus, models, DataTableNames.MenusDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MenusDataTableRestore(List<codeMenus> models)
        {
            if (!CMS.HasAction(Permissions.NavigationMenus.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeMenus> RestoredMenus = RestoreMenus(models);

            try
            {
                DbCMS.SaveChanges();
                CMS.BuildPublicMenus();
                List<Guid> OnlineUsers = (List<Guid>)HttpContext.Application["OnlineUsers"];
                foreach (var userGuid in OnlineUsers)
                {
                    CMS.BuildUserMenus(userGuid, LAN);
                }

                return Json(DbCMS.PartialRestoreMessage(RestoredMenus, models, DataTableNames.MenusDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeMenus> DeleteMenus(List<codeMenus> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;
            List<codeMenus> DeletedMenus = new List<codeMenus>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Delete, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeMenus>(query).ToList();
            foreach (var record in Records)
            {
                DeletedMenus.Add(DbCMS.Delete(record, ExecutionTime, DeleteActionGUID));
            }

            var Languages = DeletedMenus.SelectMany(a => a.codeMenusLanguages).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Delete(language, ExecutionTime, DeleteActionGUID);
            }
            return DeletedMenus;
        }

        private List<codeMenus> RestoreMenus(List<codeMenus> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeMenus> RestoredMenus = new List<codeMenus>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeMenus>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveMenu(record))
                {
                    RestoredMenus.Add(DbCMS.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            var Languages = RestoredMenus.SelectMany(l => l.codeMenusLanguages.Where(x => x.DeletedOn == l.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime);
            }

            return RestoredMenus;
        }

        private JsonResult ConcrrencyMenu(Guid PK)
        {
            MenuUpdateModel dbModel = new MenuUpdateModel();

            var Menu = DbCMS.codeMenus.Where(a => a.MenuGUID == PK).FirstOrDefault();
            var dbMenu = DbCMS.Entry(Menu).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbMenu, dbModel);

            var Language = DbCMS.codeMenusLanguages.Where(x => x.MenuGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeMenus.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Menu.codeMenusRowVersion.SequenceEqual(dbModel.codeMenusRowVersion) && Language.codeMenusLanguagesRowVersion.SequenceEqual(dbModel.codeMenusLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveMenu(Object model)
        {
            codeMenus Menu = Mapper.Map(model, new codeMenus());
            codeMenusLanguages MenuLanguage = Mapper.Map(model, new codeMenusLanguages());

            //Should not allow two menu has same description under the same root menu.
            int MenuDescription = (from a in DbCMS.codeMenus.Where(x => x.MenuGUID != Menu.MenuGUID && x.ParentGUID == Menu.ParentGUID)
                                   join b in DbCMS.codeMenusLanguages.Where(x => x.MenuDescription == MenuLanguage.MenuDescription) on a.MenuGUID equals b.MenuGUID
                                   where a.ApplicationGUID == Menu.ApplicationGUID
                                   select a).Count();

            if (MenuDescription > 0)
            {
                ModelState.AddModelError("MenuDescription", "Menu Description is already exists");
            }

            return MenuDescription > 0;
        }

        #endregion

        #region Menus Language
        public ActionResult MenuLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/CMS/Views/Codes/Menus/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeMenusLanguages, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeMenusLanguages>(DataTable.Filters);
            }

            var Result = DbCMS.codeMenusLanguages.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.MenuGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.MenuLanguageGUID,
                                  x.LanguageID,
                                  x.MenuDescription,
                                  x.codeMenusLanguagesRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult MenuLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.NavigationMenus.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/Menus/_LanguageUpdateModal.cshtml",
                new codeMenusLanguages { MenuGUID = FK });
        }

        public ActionResult MenuLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.NavigationMenus.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/Menus/_LanguageUpdateModal.cshtml", DbCMS.codeMenusLanguages.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MenuLanguageCreate(codeMenusLanguages model)
        {
            if (!CMS.HasAction(Permissions.NavigationMenus.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMenuLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/Menus/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Create(model, Permissions.NavigationMenusLanguages.CreateGuid, ExecutionTime);
            try
            {
                DbCMS.SaveChanges();

                CMS.BuildPublicMenus();
                List<Guid> OnlineUsers = (List<Guid>)HttpContext.Application["OnlineUsers"];
                foreach (var userGuid in OnlineUsers)
                {
                    CMS.BuildUserMenus(userGuid, LAN);
                }

                return Json(DbCMS.SingleUpdateMessage(DataTableNames.MenuLanguagesDataTable,
                            DbCMS.PrimaryKeyControl(model),
                            DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MenuLanguageUpdate(codeMenusLanguages model)
        {
            if (!CMS.HasAction(Permissions.NavigationMenus.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMenuLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/Menus/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;


            DbCMS.Update(model, Permissions.NavigationMenusLanguages.UpdateGuid, ExecutionTime);
            try
            {
                DbCMS.SaveChanges();

                CMS.BuildPublicMenus();
                List<Guid> OnlineUsers = (List<Guid>)HttpContext.Application["OnlineUsers"];
                foreach (var userGuid in OnlineUsers)
                {
                    CMS.BuildUserMenus(userGuid, LAN);
                }

                return Json(DbCMS.SingleUpdateMessage(DataTableNames.MenuLanguagesDataTable,
                            DbCMS.PrimaryKeyControl(model),
                            DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMenuLanguage(model.MenuLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MenuLanguageDelete(codeMenusLanguages model)
        {
            if (!CMS.HasAction(Permissions.NavigationMenus.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeMenusLanguages> DeletedLanguages = DeleteMenuLanguages(new List<codeMenusLanguages> { model });

            try
            {
                DbCMS.SaveChanges();

                CMS.BuildPublicMenus();
                List<Guid> OnlineUsers = (List<Guid>)HttpContext.Application["OnlineUsers"];
                foreach (var userGuid in OnlineUsers)
                {
                    CMS.BuildUserMenus(userGuid, LAN);
                }

                return Json(DbCMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.MenuLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMenuLanguage(model.MenuGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MenuLanguageRestore(codeMenusLanguages model)
        {
            if (!CMS.HasAction(Permissions.NavigationMenus.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveMenuLanguage(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeMenusLanguages> RestoredLanguages = RestoreMenuLanguages(Portal.SingleToList(model));

            try
            {
                DbCMS.SaveChanges();

                CMS.BuildPublicMenus();
                List<Guid> OnlineUsers = (List<Guid>)HttpContext.Application["OnlineUsers"];
                foreach (var userGuid in OnlineUsers)
                {
                    CMS.BuildUserMenus(userGuid, LAN);
                }

                return Json(DbCMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.MenuLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMenuLanguage(model.MenuLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MenuLanguagesDataTableDelete(List<codeMenusLanguages> models)
        {
            if (!CMS.HasAction(Permissions.NavigationMenus.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeMenusLanguages> DeletedLanguages = DeleteMenuLanguages(models);

            try
            {
                DbCMS.SaveChanges();

                CMS.BuildPublicMenus();
                List<Guid> OnlineUsers = (List<Guid>)HttpContext.Application["OnlineUsers"];
                foreach (var userGuid in OnlineUsers)
                {
                    CMS.BuildUserMenus(userGuid, LAN);
                }

                return Json(DbCMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.MenuLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MenuLanguagesDataTableRestore(List<codeMenusLanguages> models)
        {
            if (!CMS.HasAction(Permissions.NavigationMenus.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeMenusLanguages> RestoredLanguages = RestoreMenuLanguages(models);

            try
            {
                DbCMS.SaveChanges();

                CMS.BuildPublicMenus();
                List<Guid> OnlineUsers = (List<Guid>)HttpContext.Application["OnlineUsers"];
                foreach (var userGuid in OnlineUsers)
                {
                    CMS.BuildUserMenus(userGuid, LAN);
                }

                return Json(DbCMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.MenuLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeMenusLanguages> DeleteMenuLanguages(List<codeMenusLanguages> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeMenusLanguages> DeletedMenuLanguages = new List<codeMenusLanguages>();

            string query = DbCMS.QueryBuilder(models, Permissions.NavigationMenusLanguages.DeleteGuid, SubmitTypes.Delete, "");

            var Languages = DbCMS.Database.SqlQuery<codeMenusLanguages>(query).ToList();

            foreach (var language in Languages)
            {
                DeletedMenuLanguages.Add(DbCMS.Delete(language, ExecutionTime, Permissions.NavigationMenusLanguages.DeleteGuid));
            }

            return DeletedMenuLanguages;
        }

        private List<codeMenusLanguages> RestoreMenuLanguages(List<codeMenusLanguages> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeMenusLanguages> RestoredLanguages = new List<codeMenusLanguages>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Languages = DbCMS.Database.SqlQuery<codeMenusLanguages>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveMenuLanguage(language))
                {
                    RestoredLanguages.Add(DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyMenuLanguage(Guid PK)
        {
            codeMenusLanguages dbModel = new codeMenusLanguages();

            var Language = DbCMS.codeMenusLanguages.Where(l => l.MenuLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeMenusLanguagesRowVersion.SequenceEqual(dbModel.codeMenusLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveMenuLanguage(codeMenusLanguages model)
        {
            int LanguageID = DbCMS.codeMenusLanguages
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.MenuGUID == model.MenuGUID &&
                                              x.MenuLanguageGUID != model.MenuLanguageGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Menu Name in selected language already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }

        #endregion

        #region Departments
        [Route("CMS/Codes/Departments/")]
        public ActionResult DepartmentsIndex()
        {
            if (!CMS.HasAction(Permissions.Departments.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/Departments/Index.cshtml");
        }

        [Route("CMS/Codes/DepartmentsDataTable/")]
        public JsonResult DepartmentsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<DepartmentDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<DepartmentDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbCMS.codeDepartments.AsExpandable()
                       join b in DbCMS.codeDepartmentsLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeDepartments.DeletedOn) && x.LanguageID == LAN) on a.DepartmentGUID equals b.DepartmentGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       select new DepartmentDataTableModel
                       {
                           DepartmentGUID = a.DepartmentGUID,
                           DepartmentDescription = R1.DepartmentDescription,
                           DepartmentCode = a.DepartmentCode,
                           Active = a.Active,
                           SortID = a.SortID,
                           codeDepartmentsRowVersion = a.codeDepartmentsRowVersion,
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<DepartmentDataTableModel> Result = Mapper.Map<List<DepartmentDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("CMS/Codes/Departments/Create/")]
        public ActionResult DepartmentCreate()
        {
            if (!CMS.HasAction(Permissions.Departments.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/Departments/Department.cshtml", new DepartmentUpdateModel());
        }

        [Route("CMS/Codes/Departments/Update/{PK}")]
        public ActionResult DepartmentUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.Departments.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbCMS.codeDepartments.WherePK(PK)
                         join b in DbCMS.codeDepartmentsLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeDepartments.DeletedOn) && x.LanguageID == LAN)
                         on a.DepartmentGUID equals b.DepartmentGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new DepartmentUpdateModel
                         {
                             DepartmentGUID = a.DepartmentGUID,
                             DepartmentCode = a.DepartmentCode,
                             SortID = a.SortID,
                             Active = a.Active,
                             codeDepartmentsRowVersion = a.codeDepartmentsRowVersion,
                             DepartmentDescription = R1.DepartmentDescription,
                             codeDepartmentsLanguagesRowVersion = R1.codeDepartmentsLanguagesRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("Departments", "Codes", new { Area = "CMS" }));

            return View("~/Areas/CMS/Views/Codes/Departments/Department.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DepartmentCreate(DepartmentUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Departments.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveDepartment(model)) return PartialView("~/Areas/CMS/Views/Codes/Departments/_DepartmentForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeDepartments Department = Mapper.Map(model, new codeDepartments());
            Department.DepartmentGUID = EntityPK;
            DbCMS.Create(Department, Permissions.Departments.CreateGuid, ExecutionTime);

            codeDepartmentsLanguages Language = Mapper.Map(model, new codeDepartmentsLanguages());
            Language.DepartmentGUID = EntityPK;
            DbCMS.Create(Language, Permissions.DepartmentsLanguages.CreateGuid, ExecutionTime);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.DepartmentLanguagesDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Departments.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("Department/Create", "Codes", new { Area = "CMS" })), Container = "DepartmentFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Departments.Update, Apps.CMS), Container = "DepartmentFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Departments.Delete, Apps.CMS), Container = "DepartmentFormControls" });


            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleCreateMessage(DbCMS.PrimaryKeyControl(Department), DbCMS.RowVersionControls(Department, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DepartmentUpdate(DepartmentUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Departments.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveDepartment(model)) return PartialView("~/Areas/CMS/Views/Codes/Departments/_DepartmentForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeDepartments Department = Mapper.Map(model, new codeDepartments());
            DbCMS.Update(Department, Permissions.Departments.UpdateGuid, ExecutionTime);

            var Language = DbCMS.codeDepartmentsLanguages.Where(x => x.DepartmentGUID == model.DepartmentGUID && x.LanguageID == LAN && x.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.DepartmentGUID = Department.DepartmentGUID;
                DbCMS.Create(Language, Permissions.DepartmentsLanguages.CreateGuid, ExecutionTime);
            }
            else if (Language.DepartmentDescription != model.DepartmentDescription)
            {
                Language = Mapper.Map(model, Language);
                DbCMS.Update(Language, Permissions.DepartmentsLanguages.UpdateGuid, ExecutionTime);
            }

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, DbCMS.RowVersionControls(Department, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyDepartment(model.DepartmentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DepartmentDelete(codeDepartments model)
        {
            if (!CMS.HasAction(Permissions.Departments.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDepartments> DeletedDepartments = DeleteDepartments(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.Departments.Restore, Apps.CMS), Container = "DepartmentFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(CommitedRows, DeletedDepartments.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyDepartment(model.DepartmentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DepartmentRestore(codeDepartments model)
        {
            if (!CMS.HasAction(Permissions.Departments.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveDepartment(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeDepartments> RestoredDepartments = RestoreDepartments(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Departments.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("Department/Create", "Codes", new { Area = "CMS" })), Container = "DepartmentFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Departments.Update, Apps.CMS), Container = "DepartmentFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Departments.Restore, Apps.CMS), Container = "DepartmentFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(CommitedRows, RestoredDepartments, DbCMS.PrimaryKeyControl(RestoredDepartments.FirstOrDefault()), Url.Action(DataTableNames.DepartmentLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyDepartment(model.DepartmentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DepartmentsDataTableDelete(List<codeDepartments> models)
        {
            if (!CMS.HasAction(Permissions.Departments.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDepartments> DeletedDepartments = DeleteDepartments(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedDepartments, models, DataTableNames.DepartmentsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DepartmentsDataTableRestore(List<codeDepartments> models)
        {
            if (!CMS.HasAction(Permissions.Departments.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDepartments> RestoredDepartments = RestoreDepartments(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredDepartments, models, DataTableNames.DepartmentsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeDepartments> DeleteDepartments(List<codeDepartments> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;
            List<codeDepartments> DeletedDepartments = new List<codeDepartments>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Delete, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeDepartments>(query).ToList();
            foreach (var record in Records)
            {
                DeletedDepartments.Add(DbCMS.Delete(record, ExecutionTime, DeleteActionGUID));
            }

            var Languages = DeletedDepartments.SelectMany(a => a.codeDepartmentsLanguages).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Delete(language, ExecutionTime, DeleteActionGUID);
            }
            return DeletedDepartments;
        }

        private List<codeDepartments> RestoreDepartments(List<codeDepartments> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeDepartments> RestoredDepartments = new List<codeDepartments>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeDepartments>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveDepartment(record))
                {
                    RestoredDepartments.Add(DbCMS.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            var Languages = RestoredDepartments.SelectMany(x => x.codeDepartmentsLanguages.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveDepartmentLanguage(language))
                {
                    DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime);
                }
            }

            return RestoredDepartments;
        }

        private JsonResult ConcrrencyDepartment(Guid PK)
        {
            DepartmentUpdateModel dbModel = new DepartmentUpdateModel();

            var Department = DbCMS.codeDepartments.Where(x => x.DepartmentGUID == PK).FirstOrDefault();
            var dbDepartment = DbCMS.Entry(Department).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbDepartment, dbModel);

            var Language = DbCMS.codeDepartmentsLanguages.Where(x => x.DepartmentGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeDepartments.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Department.codeDepartmentsRowVersion.SequenceEqual(dbModel.codeDepartmentsRowVersion) && Language.codeDepartmentsLanguagesRowVersion.SequenceEqual(dbModel.codeDepartmentsLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveDepartment(Object model)
        {
            codeDepartments Department = Mapper.Map(model, new codeDepartments());
            codeDepartmentsLanguages DepartmentLanguage = Mapper.Map(model, new codeDepartmentsLanguages());

            int DepartmentCode = DbCMS.codeDepartments
                                      .Where(x => x.DepartmentCode == Department.DepartmentCode &&
                                                  x.DepartmentGUID != Department.DepartmentGUID &&
                                                  x.Active).Count();

            if (DepartmentCode > 0)
            {
                ModelState.AddModelError("DepartmentCode", "Department Code is already exists");
            }

            int DepartmentDescription = DbCMS.codeDepartmentsLanguages
                                             .Where(x => x.DepartmentDescription == DepartmentLanguage.DepartmentDescription &&
                                                         x.DepartmentGUID != Department.DepartmentGUID &&
                                                         x.LanguageID == LAN &&
                                                         x.Active).Count();

            if (DepartmentDescription > 0)
            {
                ModelState.AddModelError("DepartmentDescription", "Department description in selected language already exists");
            }

            return (DepartmentCode + DepartmentDescription) > 0;
        }
        #endregion

        #region Department Languages
        public ActionResult DepartmentLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/CMS/Views/Codes/Departments/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeDepartmentsLanguages, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeDepartmentsLanguages>(DataTable.Filters);
            }

            var Result = DbCMS.codeDepartmentsLanguages.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.DepartmentGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.DepartmentLanguageGUID,
                                  x.LanguageID,
                                  x.DepartmentDescription,
                                  x.codeDepartmentsLanguagesRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DepartmentLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.Departments.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/Departments/_LanguageUpdateModal.cshtml",
                new codeDepartmentsLanguages { DepartmentGUID = FK });
        }

        public ActionResult DepartmentLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.Departments.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/Departments/_LanguageUpdateModal.cshtml", DbCMS.codeDepartmentsLanguages.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DepartmentLanguageCreate(codeDepartmentsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Departments.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveDepartmentLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/Departments/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Create(model, Permissions.DepartmentsLanguages.CreateGuid, ExecutionTime);
            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.DepartmentLanguagesDataTable,
                            DbCMS.PrimaryKeyControl(model),
                            DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DepartmentLanguageUpdate(codeDepartmentsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Departments.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveDepartmentLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/Departments/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Update(model, Permissions.DepartmentsLanguages.UpdateGuid, ExecutionTime);
            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.DepartmentLanguagesDataTable,
                            DbCMS.PrimaryKeyControl(model),
                            DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyDepartmentLanguage(model.DepartmentLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DepartmentLanguageDelete(codeDepartmentsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Departments.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDepartmentsLanguages> DeletedLanguages = DeleteDepartmentLanguages(new List<codeDepartmentsLanguages> { model });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.DepartmentLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyDepartmentLanguage(model.DepartmentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DepartmentLanguageRestore(codeDepartmentsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Departments.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveDepartmentLanguage(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeDepartmentsLanguages> RestoredLanguages = RestoreDepartmentLanguages(Portal.SingleToList(model));

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.DepartmentLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyDepartmentLanguage(model.DepartmentLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DepartmentLanguagesDataTableDelete(List<codeDepartmentsLanguages> models)
        {
            if (!CMS.HasAction(Permissions.Departments.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDepartmentsLanguages> DeletedLanguages = DeleteDepartmentLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.DepartmentLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DepartmentLanguagesDataTableRestore(List<codeDepartmentsLanguages> models)
        {
            if (!CMS.HasAction(Permissions.Departments.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDepartmentsLanguages> RestoredLanguages = RestoreDepartmentLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.DepartmentLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeDepartmentsLanguages> DeleteDepartmentLanguages(List<codeDepartmentsLanguages> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeDepartmentsLanguages> DeletedDepartmentLanguages = new List<codeDepartmentsLanguages>();

            string query = DbCMS.QueryBuilder(models, Permissions.DepartmentsLanguages.DeleteGuid, SubmitTypes.Delete, "");

            var Languages = DbCMS.Database.SqlQuery<codeDepartmentsLanguages>(query).ToList();

            foreach (var language in Languages)
            {
                DeletedDepartmentLanguages.Add(DbCMS.Delete(language, ExecutionTime, Permissions.DepartmentsLanguages.DeleteGuid));
            }

            return DeletedDepartmentLanguages;
        }

        private List<codeDepartmentsLanguages> RestoreDepartmentLanguages(List<codeDepartmentsLanguages> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<codeDepartmentsLanguages> RestoredLanguages = new List<codeDepartmentsLanguages>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, Permissions.DepartmentsLanguages.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbCMS.Database.SqlQuery<codeDepartmentsLanguages>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveDepartmentLanguage(language))
                {
                    RestoredLanguages.Add(DbCMS.Restore(language, Permissions.DepartmentsLanguages.DeleteGuid, Permissions.DepartmentsLanguages.RestoreGuid, RestoringTime));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyDepartmentLanguage(Guid PK)
        {
            codeDepartmentsLanguages dbModel = new codeDepartmentsLanguages();

            var Language = DbCMS.codeDepartmentsLanguages.Where(l => l.DepartmentLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeDepartmentsLanguagesRowVersion.SequenceEqual(dbModel.codeDepartmentsLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveDepartmentLanguage(codeDepartmentsLanguages model)
        {
            int LanguageID = DbCMS.codeDepartmentsLanguages
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.DepartmentGUID == model.DepartmentGUID &&
                                              x.DepartmentLanguageGUID != model.DepartmentLanguageGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Department Description in selected language already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }
        #endregion


        #region Organizations
        [Route("CMS/Codes/Organizations/")]
        public ActionResult OrganizationsIndex()
        {
            if (!CMS.HasAction(Permissions.Organizations.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/Organizations/Index.cshtml");
        }

        [Route("CMS/Codes/OrganizationsDataTable/")]
        public JsonResult OrganizationsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<OrganizationDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<OrganizationDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbCMS.codeOrganizations.AsExpandable()
                       join b in DbCMS.codeOrganizationsLanguages.Where(p => (p.Active == true ? p.Active : p.DeletedOn == p.codeOrganizations.DeletedOn) && p.LanguageID == LAN) on a.OrganizationGUID equals b.OrganizationGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join c in DbCMS.codeTablesValuesLanguages.Where(l => l.LanguageID == LAN && l.Active) on a.OrganizationTypeGUID equals c.ValueGUID into LJ2
                       from R2 in LJ2.DefaultIfEmpty()
                       select new OrganizationDataTableModel
                       {
                           OrganizationGUID = a.OrganizationGUID,
                           OrganizationFocusCode = a.OrganizationFocusCode,
                           OrganizationShortName = a.OrganizationShortName,
                           OrganizationDescription = R1.OrganizationDescription,
                           OrganizationType = R2.ValueDescription,
                           codeOrganizationsRowVersion = a.codeOrganizationsRowVersion,
                           Active = a.Active,
                           //OperationsCount = DbCMS.codeOrganizationsOperations.Where(oc => oc.OrganizationGUID == a.OrganizationGUID).Count()
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<OrganizationDataTableModel> Result = Mapper.Map<List<OrganizationDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("CMS/Codes/Organizations/Create/")]
        public ActionResult OrganizationCreate()
        {
            if (!CMS.HasAction(Permissions.Organizations.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/Organizations/Organization.cshtml", new OrganizationUpdateModel());
        }

        [Route("CMS/Codes/Organizations/Update/{PK}")]
        public ActionResult OrganizationUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.Organizations.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbCMS.codeOrganizations.WherePK(PK)
                         join b in DbCMS.codeOrganizationsLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeOrganizations.DeletedOn) && x.LanguageID == LAN)
                         on a.OrganizationGUID equals b.OrganizationGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new OrganizationUpdateModel
                         {
                             OrganizationGUID = a.OrganizationGUID,
                             OrganizationTypeGUID = a.OrganizationTypeGUID,
                             CountryGUID = a.CountryGUID,
                             OrganizationFocusCode = a.OrganizationFocusCode,
                             OrganizationShortName = a.OrganizationShortName,
                             OrganizationDescription = R1.OrganizationDescription,
                             OrganizationWebSite = a.OrganizationWebSite,
                             DirectorEmail = a.DirectorEmail,
                             DirectorPhone = a.DirectorPhone,
                             HQAddress = a.HQAddress,
                             DirectorName = a.DirectorName,
                             Active = a.Active,
                             codeOrganizationsRowVersion = a.codeOrganizationsRowVersion,
                             codeOrganizationsLanguagesRowVersion = R1.codeOrganizationsLanguagesRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("Organizations", "Codes", new { Area = "CMS" }));

            model.MediaPath = new Portal().OrganizationLogo(model.OrganizationGUID);

            return View("~/Areas/CMS/Views/Codes/Organizations/Organization.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OrganizationCreate(OrganizationUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Organizations.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid || ActiveOrganization(model)) return PartialView("~/Areas/CMS/Views/Codes/Organizations/_OrganizationForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeOrganizations Organization = Mapper.Map(model, new codeOrganizations());
            Organization.OrganizationGUID = EntityPK;
            DbCMS.Create(Organization, Permissions.Organizations.CreateGuid, ExecutionTime);

            codeOrganizationsLanguages Language = Mapper.Map(model, new codeOrganizationsLanguages());
            Language.OrganizationGUID = EntityPK;
            DbCMS.Create(Language, Permissions.OrganizationsLanguages.CreateGuid, ExecutionTime);

            SyncOrganizationInstanceOnCreate(model, EntityPK);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.OrganizationLanguagesDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Organizations.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("Organization/Create", "Codes", new { Area = "CMS" })), Container = "OrganizationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Organizations.Update, Apps.CMS), Container = "OrganizationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Organizations.Delete, Apps.CMS), Container = "OrganizationFormControls" });

            try
            {
                DbCMS.SaveChanges();
                new Portal().CutFile(model.MediaName, "Logos\\" + Organization.OrganizationGUID + ".png");
                return Json(DbCMS.SingleCreateMessage(DbCMS.PrimaryKeyControl(Organization), DbCMS.RowVersionControls(Organization, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OrganizationUpdate(OrganizationUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Organizations.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid || ActiveOrganization(model)) return PartialView("~/Areas/CMS/Views/Codes/Organizations/_OrganizationForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeOrganizations Organization = Mapper.Map(model, new codeOrganizations());
            DbCMS.Update(Organization, Permissions.Organizations.UpdateGuid, ExecutionTime);

            var Language = DbCMS.codeOrganizationsLanguages.Where(x => x.OrganizationGUID == model.OrganizationGUID && x.LanguageID == LAN && x.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.OrganizationGUID = Organization.OrganizationGUID;
                DbCMS.Create(Language, Permissions.OrganizationsLanguages.CreateGuid, ExecutionTime);
            }
            else if (Language.OrganizationDescription != model.OrganizationDescription)
            {
                Language = Mapper.Map(model, Language);
                DbCMS.Update(Language, Permissions.OrganizationsLanguages.UpdateGuid, ExecutionTime);
                SyncOrganizationInstanceOnUpdate(Language, LAN);
            }

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, DbCMS.RowVersionControls(Organization, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyOrganization(model.OrganizationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OrganizationDelete(codeOrganizations model)
        {
            if (!CMS.HasAction(Permissions.Organizations.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeOrganizations> DeletedOrganizations = DeleteOrganizations(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.Organizations.Restore, Apps.CMS), Container = "OrganizationFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(CommitedRows, DeletedOrganizations.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyOrganization(model.OrganizationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OrganizationRestore(codeOrganizations model)
        {
            if (!CMS.HasAction(Permissions.Organizations.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveOrganization(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeOrganizations> RestoredOrganizations = RestoreOrganizations(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Organizations.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("Organization/Create", "Codes", new { Area = "CMS" })), Container = "OrganizationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Organizations.Update, Apps.CMS), Container = "OrganizationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Organizations.Delete, Apps.CMS), Container = "OrganizationFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(CommitedRows, RestoredOrganizations, DbCMS.PrimaryKeyControl(RestoredOrganizations.FirstOrDefault()), Url.Action(DataTableNames.OrganizationLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyOrganization(model.OrganizationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult OrganizationsDataTableDelete(List<codeOrganizations> models)
        {
            if (!CMS.HasAction(Permissions.Organizations.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeOrganizations> DeletedOrganizations = DeleteOrganizations(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedOrganizations, models, DataTableNames.OrganizationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult OrganizationsDataTableRestore(List<codeOrganizations> models)
        {
            if (!CMS.HasAction(Permissions.Organizations.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeOrganizations> RestoredOrganizations = RestoreOrganizations(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredOrganizations, models, DataTableNames.OrganizationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        public ActionResult OrganizationOrganizationLogoDelete(OrganizationUpdateModel model)
        {
            //This function will delete the photo only.
            System.IO.File.Delete(WebConfigurationManager.AppSettings["DataFolder"] + "\\Logos\\" + model.OrganizationGUID.ToString() + ".png");
            return Json(DbCMS.SingleUpdateMessage(null, null, null, "RemoveOrganizationLogo('" + Constants.NoPhotoTemplate + "')", "Logo deleted successfully"));
        }

        private List<codeOrganizations> DeleteOrganizations(List<codeOrganizations> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;
            List<codeOrganizations> DeletedOrganizations = new List<codeOrganizations>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Delete, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeOrganizations>(query).ToList();
            foreach (var record in Records)
            {
                DeletedOrganizations.Add(DbCMS.Delete(record, ExecutionTime, DeleteActionGUID));
            }

            var Languages = DeletedOrganizations.SelectMany(x => x.codeOrganizationsLanguages).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Delete(language, ExecutionTime, DeleteActionGUID);
            }

            return DeletedOrganizations;
        }

        private List<codeOrganizations> RestoreOrganizations(List<codeOrganizations> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeOrganizations> RestoredOrganizations = new List<codeOrganizations>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Organization = DbCMS.Database.SqlQuery<codeOrganizations>(query).ToList();
            foreach (var organization in Organization)
            {
                if (!ActiveOrganization(organization))
                {
                    RestoredOrganizations.Add(DbCMS.Restore(organization, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            var Languages = RestoredOrganizations.SelectMany(l => l.codeOrganizationsLanguages.Where(x => x.DeletedOn == l.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveOrganizationLanguage(language))
                {
                    DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime);
                }
            }

            return RestoredOrganizations;
        }

        private JsonResult ConcrrencyOrganization(Guid PK)
        {
            OrganizationUpdateModel dbModel = new OrganizationUpdateModel();

            var Organization = DbCMS.codeOrganizations.Where(x => x.OrganizationGUID == PK).FirstOrDefault();
            var dbOrganization = DbCMS.Entry(Organization).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbOrganization, dbModel);

            var Language = DbCMS.codeOrganizationsLanguages.Where(x => x.OrganizationGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeOrganizations.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Organization.codeOrganizationsRowVersion.SequenceEqual(dbModel.codeOrganizationsRowVersion) && Language.codeOrganizationsLanguagesRowVersion.SequenceEqual(dbModel.codeOrganizationsLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveOrganization(Object model)
        {
            codeOrganizations Organization = Mapper.Map(model, new codeOrganizations());
            codeOrganizationsLanguages OrganizationLanguage = Mapper.Map(model, new codeOrganizationsLanguages());

            int OrganizationShortName = DbCMS.codeOrganizations
                                             .Where(x => x.OrganizationShortName == Organization.OrganizationShortName &&
                                                         x.OrganizationGUID != Organization.OrganizationGUID &&
                                                         x.Active).Count();

            if (OrganizationShortName > 0)
            {
                ModelState.AddModelError("OrganizationShortName", "Organization short name is already exists");
            }

            int OrganizationFocusCode = DbCMS.codeOrganizations
                                             .Where(x => x.OrganizationFocusCode == Organization.OrganizationFocusCode &&
                                                         x.OrganizationGUID != Organization.OrganizationGUID &&
                                                         x.Active).Count();

            if (OrganizationFocusCode > 0)
            {
                ModelState.AddModelError("OrganizationFocusCode", "Organization focus code is already exists");
            }

            int OrganizationDescription = DbCMS.codeOrganizationsLanguages
                                               .Where(x => x.OrganizationDescription == OrganizationLanguage.OrganizationDescription &&
                                                           x.OrganizationGUID != Organization.OrganizationGUID &&
                                                           x.LanguageID == LAN &&
                                                           x.Active).Count();

            if (OrganizationDescription > 0)
            {
                ModelState.AddModelError("OrganizationDescription", "Organization description in selected language already exists");
            }

            return (OrganizationShortName + OrganizationFocusCode + OrganizationDescription) > 0;
        }
        #endregion

        #region Organizations Language
        public ActionResult OrganizationLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/CMS/Views/Codes/Organizations/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeOrganizationsLanguages, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeOrganizationsLanguages>(DataTable.Filters);
            }

            var Result = DbCMS.codeOrganizationsLanguages.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.OrganizationGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.OrganizationLanguageGUID,
                                  x.LanguageID,
                                  x.OrganizationDescription,
                                  x.codeOrganizationsLanguagesRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult OrganizationLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.Organizations.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/Organizations/_LanguageUpdateModal.cshtml",
                new codeOrganizationsLanguages { OrganizationGUID = FK });
        }

        public ActionResult OrganizationLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.Organizations.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/Organizations/_LanguageUpdateModal.cshtml", DbCMS.codeOrganizationsLanguages.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OrganizationLanguageCreate(codeOrganizationsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Organizations.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveOrganizationLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/Organizations/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Create(model, Permissions.OrganizationsLanguages.CreateGuid, ExecutionTime);

            //Check if Oranization is Not Intenational and does not have the inserted language
            Guid _OrganizationType = DbCMS.codeOrganizations.AsNoTracking().Where(x => x.OrganizationGUID == model.OrganizationGUID).FirstOrDefault().OrganizationTypeGUID;
            if (_OrganizationType != OrganizationsTypes.International) //!= International 
            {
                //Does the instance has the langauge or not?
                bool LanguageNotExists = DbCMS.codeOrganizationsInstancesLanguages.AsNoTracking().Where(x => x.LanguageID == model.LanguageID && x.Active).Count() == 0;
                if (LanguageNotExists)
                {
                    Guid _OrganizationInstanceGUID = DbCMS.codeOrganizationsInstances.AsNoTracking().Where(x => x.OrganizationGUID == model.OrganizationGUID).FirstOrDefault().OrganizationInstanceGUID;

                    codeOrganizationsInstancesLanguages InstanceLanguage = new codeOrganizationsInstancesLanguages
                    {
                        OrganizationInstanceLanguageGUID = Guid.NewGuid(),
                        OrganizationInstanceGUID = _OrganizationInstanceGUID,
                        LanguageID = model.LanguageID,
                        OrganizationInstanceDescription = model.OrganizationDescription,
                        Active = true
                    };
                    DbCMS.Create(InstanceLanguage, Permissions.OrganizationsInstancesLanguages.CreateGuid, ExecutionTime);
                }
            }

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.OrganizationLanguagesDataTable,
                   DbCMS.PrimaryKeyControl(model),
                   DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OrganizationLanguageUpdate(codeOrganizationsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Organizations.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveOrganizationLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/Organizations/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;


            DbCMS.Update(model, Permissions.OrganizationsLanguages.UpdateGuid, ExecutionTime);
            SyncOrganizationInstanceOnUpdate(model, model.LanguageID);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.OrganizationLanguagesDataTable,
                                   DbCMS.PrimaryKeyControl(model),
                                   DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyOrganizationLanguage(model.OrganizationLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OrganizationLanguageDelete(codeOrganizationsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Organizations.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeOrganizationsLanguages> DeletedLanguages = DeleteOrganizationLanguages(new List<codeOrganizationsLanguages> { model });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.OrganizationLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyOrganizationLanguage(model.OrganizationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OrganizationLanguageRestore(codeOrganizationsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Organizations.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveOrganizationLanguage(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeOrganizationsLanguages> RestoredLanguages = RestoreOrganizationLanguages(Portal.SingleToList(model));

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.OrganizationLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyOrganizationLanguage(model.OrganizationLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult OrganizationLanguagesDataTableDelete(List<codeOrganizationsLanguages> models)
        {
            if (!CMS.HasAction(Permissions.Organizations.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeOrganizationsLanguages> DeletedLanguages = DeleteOrganizationLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.OrganizationLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult OrganizationLanguagesDataTableRestore(List<codeOrganizationsLanguages> models)
        {
            if (!CMS.HasAction(Permissions.Organizations.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeOrganizationsLanguages> RestoredLanguages = RestoreOrganizationLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.OrganizationLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeOrganizationsLanguages> DeleteOrganizationLanguages(List<codeOrganizationsLanguages> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeOrganizationsLanguages> DeletedOrganizationLanguages = new List<codeOrganizationsLanguages>();

            string query = DbCMS.QueryBuilder(models, Permissions.OrganizationsLanguages.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbCMS.Database.SqlQuery<codeOrganizationsLanguages>(query).ToList();

            foreach (var language in languages)
            {
                DeletedOrganizationLanguages.Add(DbCMS.Delete(language, ExecutionTime, Permissions.OrganizationsLanguages.DeleteGuid));
            }

            return DeletedOrganizationLanguages;
        }

        private List<codeOrganizationsLanguages> RestoreOrganizationLanguages(List<codeOrganizationsLanguages> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeOrganizationsLanguages> RestoredLanguages = new List<codeOrganizationsLanguages>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Languages = DbCMS.Database.SqlQuery<codeOrganizationsLanguages>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveOrganizationLanguage(language))
                {
                    RestoredLanguages.Add(DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyOrganizationLanguage(Guid PK)
        {
            codeOrganizationsLanguages dbModel = new codeOrganizationsLanguages();

            var Language = DbCMS.codeOrganizationsLanguages.Where(x => x.OrganizationLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeOrganizationsLanguagesRowVersion.SequenceEqual(dbModel.codeOrganizationsLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveOrganizationLanguage(codeOrganizationsLanguages model)
        {
            int LanguageID = DbCMS.codeOrganizationsLanguages
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.OrganizationGUID == model.OrganizationGUID &&
                                              x.OrganizationLanguageGUID != model.OrganizationLanguageGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Organization Name in selected language already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }
        #endregion


        #region Organizations Instances
        [Route("CMS/Codes/OrganizationsInstances/")]
        public ActionResult OrganizationsInstancesIndex()
        {
            if (!CMS.HasAction(Permissions.OrganizationsInstances.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/OrganizationsInstances/Index.cshtml");
        }

        [Route("CMS/Codes/OrganizationsInstancesDataTable/")]
        public JsonResult OrganizationsInstancesDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<OrganizationInstanceDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<OrganizationInstanceDataTableModel>(DataTable.Filters);
            }

            List<Guid> AuthorizedList = CMS.GetAuthorizedList(Permissions.OrganizationsInstances.AccessGuid, FactorTypes.Organizations);

            var All = (from a in DbCMS.codeOrganizationsInstances.AsExpandable().Where(x => AuthorizedList.Contains(x.OrganizationGUID))
                       join b in DbCMS.codeOrganizationsInstancesLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeOrganizationsInstances.DeletedOn) && x.LanguageID == LAN) on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                       join c in DbCMS.codeOrganizationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OrganizationGUID equals c.OrganizationGUID
                       join d in DbCMS.codeOperationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OperationGUID equals d.OperationGUID into LJ1
                       from e in LJ1.DefaultIfEmpty()
                       select new OrganizationInstanceDataTableModel
                       {
                           OrganizationInstanceGUID = a.OrganizationInstanceGUID,
                           OrganizationInstanceDescription = b.OrganizationInstanceDescription,
                           OrganizationDescription = c.OrganizationDescription,
                           OperationDescription = e.OperationDescription,
                           codeOrganizationsInstancesRowVersion = a.codeOrganizationsInstancesRowVersion,
                           Active = a.Active,
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<OrganizationInstanceDataTableModel> Result = Mapper.Map<List<OrganizationInstanceDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("CMS/Codes/OrganizationsInstances/Create/")]
        public ActionResult OrganizationInstanceCreate()
        {
            if (!CMS.HasAction(Permissions.OrganizationsInstances.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/OrganizationsInstances/OrganizationInstance.cshtml", new OrganizationInstanceUpdateModel());
        }

        [Route("CMS/Codes/OrganizationsInstances/Update/{PK}")]
        public ActionResult OrganizationInstanceUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.OrganizationsInstances.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbCMS.codeOrganizationsInstances.WherePK(PK)
                         join b in DbCMS.codeOrganizationsInstancesLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeOrganizationsInstances.DeletedOn) && x.LanguageID == LAN) on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                         select new OrganizationInstanceUpdateModel
                         {
                             OrganizationInstanceGUID = a.OrganizationInstanceGUID,
                             OrganizationInstanceDescription = b.OrganizationInstanceDescription,
                             OrganizationGUID = a.OrganizationGUID,
                             OperationGUID = a.OperationGUID,
                             Active = a.Active,
                             codeOrganizationsInstancesRowVersion = a.codeOrganizationsInstancesRowVersion,
                             codeOrganizationsInstancesLanguagesRowVersion = b.codeOrganizationsInstancesLanguagesRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("OrganizationsInstances", "Codes", new { Area = "CMS" }));

            return View("~/Areas/CMS/Views/Codes/OrganizationsInstances/OrganizationInstance.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OrganizationInstanceCreate(OrganizationInstanceUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.OrganizationsInstances.Create, Apps.CMS, model.OrganizationGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveOrganizationInstance(model)) return PartialView("~/Areas/CMS/Views/Codes/OrganizationsInstances/_OrganizationInstanceForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeOrganizationsInstances OrganizationInstance = Mapper.Map(model, new codeOrganizationsInstances());
            OrganizationInstance.OrganizationInstanceGUID = EntityPK;
            DbCMS.Create(OrganizationInstance, Permissions.OrganizationsInstances.CreateGuid, ExecutionTime);

            codeOrganizationsInstancesLanguages Language = Mapper.Map(model, new codeOrganizationsInstancesLanguages());
            Language.OrganizationInstanceGUID = EntityPK;
            DbCMS.Create(Language, Permissions.OrganizationsInstancesLanguages.CreateGuid, ExecutionTime);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.OrganizationInstanceLanguagesDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.OrganizationsInstances.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("OrganizationInstance/Create", "Codes", new { Area = "CMS" })), Container = "OrganizationInstanceFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.OrganizationsInstances.Update, Apps.CMS), Container = "OrganizationInstanceFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.OrganizationsInstances.Delete, Apps.CMS), Container = "OrganizationInstanceFormControls" });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleCreateMessage(DbCMS.PrimaryKeyControl(OrganizationInstance), DbCMS.RowVersionControls(OrganizationInstance, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OrganizationInstanceUpdate(OrganizationInstanceUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.OrganizationsInstances.Update, Apps.CMS, model.OrganizationGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveOrganizationInstance(model)) return PartialView("~/Areas/CMS/Views/Codes/OrganizationsInstances/_OrganizationInstanceForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeOrganizationsInstances OrganizationInstance = Mapper.Map(model, new codeOrganizationsInstances());
            DbCMS.Update(OrganizationInstance, Permissions.OrganizationsInstances.UpdateGuid, ExecutionTime);

            var Language = DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.OrganizationInstanceGUID == model.OrganizationInstanceGUID && x.LanguageID == LAN && x.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.OrganizationInstanceGUID = OrganizationInstance.OrganizationInstanceGUID;
                DbCMS.Create(Language, Permissions.OrganizationsInstancesLanguages.CreateGuid, ExecutionTime);
            }
            else if (Language.OrganizationInstanceDescription != model.OrganizationInstanceDescription)
            {
                Language = Mapper.Map(model, Language);
                DbCMS.Update(Language, Permissions.OrganizationsInstancesLanguages.UpdateGuid, ExecutionTime);
            }

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, DbCMS.RowVersionControls(OrganizationInstance, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyOrganizationInstance(model.OrganizationInstanceGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OrganizationInstanceDelete(codeOrganizationsInstances model)
        {
            if (!CMS.HasAction(Permissions.OrganizationsInstances.Delete, Apps.CMS, model.OrganizationGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeOrganizationsInstances> DeletedOrganizationsInstances = DeleteOrganizationsInstances(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.OrganizationsInstances.Delete, Apps.CMS), Container = "OrganizationInstanceFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(CommitedRows, DeletedOrganizationsInstances.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyOrganizationInstance(model.OrganizationInstanceGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OrganizationInstanceRestore(codeOrganizationsInstances model)
        {
            if (!CMS.HasAction(Permissions.OrganizationsInstances.Restore, Apps.CMS, model.OrganizationGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveOrganizationInstance(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeOrganizationsInstances> RestoredOrganizationsInstances = RestoreOrganizationsInstances(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.OrganizationsInstances.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("OrganizationInstance/Create", "Codes", new { Area = "CMS" })), Container = "OrganizationInstanceFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.OrganizationsInstances.Update, Apps.CMS), Container = "OrganizationInstanceFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.OrganizationsInstances.Delete, Apps.CMS), Container = "OrganizationInstanceFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(CommitedRows, RestoredOrganizationsInstances, DbCMS.PrimaryKeyControl(RestoredOrganizationsInstances.FirstOrDefault()), Url.Action(DataTableNames.OrganizationInstanceLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyOrganizationInstance(model.OrganizationInstanceGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult OrganizationsInstancesDataTableDelete(List<codeOrganizationsInstances> models)
        {
            if (!CMS.HasAction(Permissions.OrganizationsInstances.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeOrganizationsInstances> DeletedOrganizationsInstances = DeleteOrganizationsInstances(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedOrganizationsInstances, models, DataTableNames.OrganizationsInstancesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult OrganizationsInstancesDataTableRestore(List<codeOrganizationsInstances> models)
        {
            if (!CMS.HasAction(Permissions.OrganizationsInstances.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeOrganizationsInstances> RestoredOrganizationsInstances = RestoreOrganizationsInstances(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredOrganizationsInstances, models, DataTableNames.OrganizationsInstancesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeOrganizationsInstances> DeleteOrganizationsInstances(List<codeOrganizationsInstances> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;
            List<codeOrganizationsInstances> DeletedOrganizationsInstances = new List<codeOrganizationsInstances>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Delete, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeOrganizationsInstances>(query).ToList();
            foreach (var record in Records)
            {
                DeletedOrganizationsInstances.Add(DbCMS.Delete(record, ExecutionTime, DeleteActionGUID));
            }

            var Languages = DeletedOrganizationsInstances.SelectMany(x => x.codeOrganizationsInstancesLanguages).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Delete(language, ExecutionTime, DeleteActionGUID);
            }

            return DeletedOrganizationsInstances;
        }

        private List<codeOrganizationsInstances> RestoreOrganizationsInstances(List<codeOrganizationsInstances> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeOrganizationsInstances> RestoredOrganizationsInstances = new List<codeOrganizationsInstances>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var OrganizationsInstances = DbCMS.Database.SqlQuery<codeOrganizationsInstances>(query).ToList();
            foreach (var OrganizationInstance in OrganizationsInstances)
            {
                if (!ActiveOrganizationInstance(OrganizationInstance))
                {
                    RestoredOrganizationsInstances.Add(DbCMS.Restore(OrganizationInstance, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            var Languages = RestoredOrganizationsInstances.SelectMany(l => l.codeOrganizationsInstancesLanguages.Where(x => x.DeletedOn == l.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveOrganizationInstanceLanguage(language))
                {
                    DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime);
                }
            }

            return RestoredOrganizationsInstances;
        }

        private JsonResult ConcrrencyOrganizationInstance(Guid PK)
        {
            OrganizationInstanceUpdateModel dbModel = new OrganizationInstanceUpdateModel();

            var OrganizationInstance = DbCMS.codeOrganizationsInstances.Where(x => x.OrganizationInstanceGUID == PK).FirstOrDefault();
            var dbOrganization = DbCMS.Entry(OrganizationInstance).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbOrganization, dbModel);

            var Language = DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.OrganizationInstanceGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeOrganizationsInstances.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (OrganizationInstance.codeOrganizationsInstancesRowVersion.SequenceEqual(dbModel.codeOrganizationsInstancesRowVersion) && Language.codeOrganizationsInstancesLanguagesRowVersion.SequenceEqual(dbModel.codeOrganizationsInstancesLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveOrganizationInstance(Object model)
        {
            codeOrganizationsInstances Organization = Mapper.Map(model, new codeOrganizationsInstances());
            codeOrganizationsInstancesLanguages OrganizationLanguage = Mapper.Map(model, new codeOrganizationsInstancesLanguages());

            int OrganizationInstanceDescription = DbCMS.codeOrganizationsInstancesLanguages
                                               .Where(x => x.OrganizationInstanceDescription == OrganizationLanguage.OrganizationInstanceDescription &&
                                                           x.OrganizationInstanceGUID != Organization.OrganizationInstanceGUID &&
                                                           x.LanguageID == LAN &&
                                                           x.Active).Count();

            if (OrganizationInstanceDescription > 0)
            {
                ModelState.AddModelError("OrganizationInstanceDescription", "Organization entity description in selected language already exists");
            }

            return OrganizationInstanceDescription > 0;
        }
        #endregion

        #region Organizations Instances Language
        public ActionResult OrganizationInstanceLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/CMS/Views/Codes/OrganizationsInstances/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeOrganizationsInstancesLanguages, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeOrganizationsInstancesLanguages>(DataTable.Filters);
            }

            var Result = DbCMS.codeOrganizationsInstancesLanguages.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.OrganizationInstanceGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.OrganizationInstanceLanguageGUID,
                                  x.LanguageID,
                                  x.OrganizationInstanceDescription,
                                  x.codeOrganizationsInstancesLanguagesRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult OrganizationInstanceLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.OrganizationsInstances.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/OrganizationsInstances/_LanguageUpdateModal.cshtml",
                new codeOrganizationsInstancesLanguages { OrganizationInstanceGUID = FK });
        }

        public ActionResult OrganizationInstanceLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.OrganizationsInstances.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/OrganizationsInstances/_LanguageUpdateModal.cshtml", DbCMS.codeOrganizationsInstancesLanguages.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OrganizationInstanceLanguageCreate(codeOrganizationsInstancesLanguages model)
        {
            if (!CMS.HasAction(Permissions.OrganizationsInstances.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveOrganizationInstanceLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/OrganizationsInstances/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Create(model, Permissions.OrganizationsInstancesLanguages.CreateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.OrganizationInstanceLanguagesDataTable,
                   DbCMS.PrimaryKeyControl(model),
                   DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OrganizationInstanceLanguageUpdate(codeOrganizationsInstancesLanguages model)
        {
            if (!CMS.HasAction(Permissions.OrganizationsInstances.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveOrganizationInstanceLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/OrganizationsInstances/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Update(model, Permissions.OrganizationsInstancesLanguages.UpdateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.OrganizationInstanceLanguagesDataTable,
                                   DbCMS.PrimaryKeyControl(model),
                                   DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyOrganizationInstanceLanguage(model.OrganizationInstanceLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OrganizationInstanceLanguageDelete(codeOrganizationsInstancesLanguages model)
        {
            if (!CMS.HasAction(Permissions.OrganizationsInstances.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeOrganizationsInstancesLanguages> DeletedLanguages = DeleteOrganizationInstanceLanguages(new List<codeOrganizationsInstancesLanguages> { model });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.OrganizationLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyOrganizationInstanceLanguage(model.OrganizationInstanceGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OrganizationInstanceLanguageRestore(codeOrganizationsInstancesLanguages model)
        {
            if (!CMS.HasAction(Permissions.OrganizationsInstances.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveOrganizationInstanceLanguage(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeOrganizationsInstancesLanguages> RestoredLanguages = RestoreOrganizationInstanceLanguages(Portal.SingleToList(model));

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.OrganizationLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyOrganizationInstanceLanguage(model.OrganizationInstanceLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult OrganizationInstanceLanguagesDataTableDelete(List<codeOrganizationsInstancesLanguages> models)
        {
            if (!CMS.HasAction(Permissions.OrganizationsInstances.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeOrganizationsInstancesLanguages> DeletedLanguages = DeleteOrganizationInstanceLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.OrganizationInstanceLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult OrganizationInstanceLanguagesDataTableRestore(List<codeOrganizationsInstancesLanguages> models)
        {
            if (!CMS.HasAction(Permissions.OrganizationsInstances.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeOrganizationsInstancesLanguages> RestoredLanguages = RestoreOrganizationInstanceLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.OrganizationInstanceLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeOrganizationsInstancesLanguages> DeleteOrganizationInstanceLanguages(List<codeOrganizationsInstancesLanguages> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeOrganizationsInstancesLanguages> DeletedOrganizationLanguages = new List<codeOrganizationsInstancesLanguages>();

            string query = DbCMS.QueryBuilder(models, Permissions.OrganizationsInstancesLanguages.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbCMS.Database.SqlQuery<codeOrganizationsInstancesLanguages>(query).ToList();

            foreach (var language in languages)
            {
                DeletedOrganizationLanguages.Add(DbCMS.Delete(language, ExecutionTime, Permissions.OrganizationsInstancesLanguages.DeleteGuid));
            }

            return DeletedOrganizationLanguages;
        }

        private List<codeOrganizationsInstancesLanguages> RestoreOrganizationInstanceLanguages(List<codeOrganizationsInstancesLanguages> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<codeOrganizationsInstancesLanguages> RestoredLanguages = new List<codeOrganizationsInstancesLanguages>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, Permissions.OrganizationsInstancesLanguages.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbCMS.Database.SqlQuery<codeOrganizationsInstancesLanguages>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveOrganizationInstanceLanguage(language))
                {
                    RestoredLanguages.Add(DbCMS.Restore(language, Permissions.OrganizationsInstancesLanguages.DeleteGuid, Permissions.OrganizationsInstancesLanguages.RestoreGuid, RestoringTime));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyOrganizationInstanceLanguage(Guid PK)
        {
            codeOrganizationsInstancesLanguages dbModel = new codeOrganizationsInstancesLanguages();

            var Language = DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.OrganizationInstanceLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();

            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeOrganizationsInstancesLanguagesRowVersion.SequenceEqual(dbModel.codeOrganizationsInstancesLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveOrganizationInstanceLanguage(codeOrganizationsInstancesLanguages model)
        {
            int LanguageID = DbCMS.codeOrganizationsInstancesLanguages
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.OrganizationInstanceGUID == model.OrganizationInstanceGUID &&
                                              x.OrganizationInstanceLanguageGUID != model.OrganizationInstanceLanguageGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Organization Name in selected language already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }

        private void SyncOrganizationInstanceOnCreate(OrganizationUpdateModel model, Guid EntityPK)
        {
            //Special Measures to duplicate Non International Organization Instances.
            DateTime ExecutionTime = DateTime.Now;

            if (model.OrganizationTypeGUID != OrganizationsTypes.International)
            {
                Guid OrganizationInstancePK = Guid.NewGuid();
                Guid _OperationGUID = DbCMS.codeOperations.AsNoTracking().Where(x => x.CountryGUID == model.CountryGUID).FirstOrDefault().OperationGUID;
                codeOrganizationsInstances OrganizationInstance = new codeOrganizationsInstances
                {
                    OrganizationInstanceGUID = OrganizationInstancePK,
                    OrganizationGUID = EntityPK,
                    OperationGUID = _OperationGUID,
                    Active = true,
                };
                DbCMS.Create(OrganizationInstance, Permissions.OrganizationsInstances.CreateGuid, ExecutionTime);

                codeOrganizationsInstancesLanguages OranizationInstanceLanguage = new codeOrganizationsInstancesLanguages
                {
                    OrganizationInstanceLanguageGUID = Guid.NewGuid(),
                    OrganizationInstanceGUID = OrganizationInstancePK,
                    Active = true,
                    OrganizationInstanceDescription = model.OrganizationDescription,
                };
                DbCMS.Create(OranizationInstanceLanguage, Permissions.OrganizationsInstancesLanguages.CreateGuid, ExecutionTime);
            }
        }

        private void SyncOrganizationInstanceOnUpdate(codeOrganizationsLanguages model, string LanguageID)
        {
            //Special Measures to duplicate Non International Organization Instances.
            Guid _OrganizationTypeGUID = DbCMS.codeOrganizations.AsNoTracking().Where(x => x.OrganizationGUID == model.OrganizationGUID).FirstOrDefault().OrganizationTypeGUID;
            DateTime ExecutionTime = DateTime.Now;

            if (_OrganizationTypeGUID != OrganizationsTypes.International)
            {
                var Instance = (from a in DbCMS.codeOrganizationsLanguages.Where(x => x.Active && x.LanguageID == LanguageID && x.OrganizationGUID == model.OrganizationGUID)
                                join b in DbCMS.codeOrganizationsInstances.Where(x => x.Active) on a.OrganizationGUID equals b.OrganizationGUID
                                join c in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.Active && x.LanguageID == LanguageID) on b.OrganizationInstanceGUID equals c.OrganizationInstanceGUID
                                select c).FirstOrDefault();
                Instance.OrganizationInstanceDescription = model.OrganizationDescription;
                DbCMS.Update(Instance, Permissions.OrganizationsInstances.UpdateGuid, ExecutionTime);
            }
        }
        #endregion


        #region Duty Stations
        [Route("CMS/Codes/DutyStations/")]
        public ActionResult DutyStationsIndex()
        {
            if (!CMS.HasAction(Permissions.DutyStations.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/DutyStations/Index.cshtml");
        }

        [Route("CMS/Codes/DutyStationsDataTable/")]
        public JsonResult DutyStationsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<DutyStationDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<DutyStationDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbCMS.codeDutyStations.AsExpandable()
                       join b in DbCMS.codeDutyStationsLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeDutyStations.DeletedOn) && x.LanguageID == LAN) on a.DutyStationGUID equals b.DutyStationGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join c in DbCMS.codeCountriesLanguages.Where(x => x.LanguageID == LAN) on a.CountryGUID equals c.CountryGUID into LJ2
                       from R2 in LJ2.DefaultIfEmpty()
                       select new DutyStationDataTableModel
                       {
                           DutyStationGUID = a.DutyStationGUID,
                           CountryDescription = R2.CountryDescription,
                           DutyStationDescription = R1.DutyStationDescription,
                           codeDutyStationsRowVersion = a.codeDutyStationsRowVersion,
                           Active = a.Active
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<DutyStationDataTableModel> Result = Mapper.Map<List<DutyStationDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("CMS/Codes/DutyStations/Create/")]
        public ActionResult DutyStationCreate()
        {
            if (!CMS.HasAction(Permissions.DutyStations.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/DutyStations/DutyStation.cshtml", new DutyStationsUpdateModel());
        }

        [Route("CMS/Codes/DutyStations/Update/{PK}")]
        public ActionResult DutyStationUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.DutyStations.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbCMS.codeDutyStations.WherePK(PK)
                         join b in DbCMS.codeDutyStationsLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeDutyStations.DeletedOn) && x.LanguageID == LAN)
                         on a.DutyStationGUID equals b.DutyStationGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new DutyStationsUpdateModel
                         {
                             DutyStationGUID = a.DutyStationGUID,
                             CountryGUID = a.CountryGUID,
                             DutyStationDescription = R1.DutyStationDescription,
                             Active = a.Active,
                             Latitude = a.Latitude,
                             Longitude = a.Longitude,
                             codeDutyStationsRowVersion = a.codeDutyStationsRowVersion,
                             codeDutyStationsLanguagesRowVersion = R1.codeDutyStationsLanguagesRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("DutyStations", "Codes", new { Area = "CMS" }));

            return View("~/Areas/CMS/Views/Codes/DutyStations/DutyStation.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DutyStationCreate(DutyStationsUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.DutyStations.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveDutyStation(model)) return PartialView("~/Areas/CMS/Views/Codes/DutyStations/_DutyStationForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeDutyStations DutyStation = Mapper.Map(model, new codeDutyStations());
            DutyStation.DutyStationGUID = EntityPK;
            DbCMS.Create(DutyStation, Permissions.DutyStations.CreateGuid, ExecutionTime);

            codeDutyStationsLanguages Language = Mapper.Map(model, new codeDutyStationsLanguages());
            Language.DutyStationGUID = EntityPK;
            DbCMS.Create(Language, Permissions.DutyStationsLanguages.CreateGuid, ExecutionTime);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.DutyStationLanguagesDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.DutyStations.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("DutyStation/Create", "Codes", new { Area = "CMS" })), Container = "DutyStationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.DutyStations.Update, Apps.CMS), Container = "DutyStationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.DutyStations.Delete, Apps.CMS), Container = "DutyStationFormControls" });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleCreateMessage(DbCMS.PrimaryKeyControl(DutyStation), DbCMS.RowVersionControls(DutyStation, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DutyStationUpdate(DutyStationsUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.DutyStations.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveDutyStation(model)) return PartialView("~/Areas/CMS/Views/Codes/DutyStations/_DutyStationForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeDutyStations DutyStation = Mapper.Map(model, new codeDutyStations());
            DbCMS.Update(DutyStation, Permissions.DutyStations.UpdateGuid, ExecutionTime);

            var Language = DbCMS.codeDutyStationsLanguages.Where(x => x.DutyStationGUID == model.DutyStationGUID && x.LanguageID == LAN && x.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.DutyStationGUID = DutyStation.DutyStationGUID;
                DbCMS.Create(Language, Permissions.DutyStationsLanguages.CreateGuid, ExecutionTime);
            }
            else if (Language.DutyStationDescription != model.DutyStationDescription)
            {
                Language = Mapper.Map(model, Language);
                DbCMS.Update(Language, Permissions.DutyStationsLanguages.UpdateGuid, ExecutionTime);
            }

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, DbCMS.RowVersionControls(DutyStation, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyDutyStation(model.DutyStationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DutyStationDelete(codeDutyStations model)
        {
            if (!CMS.HasAction(Permissions.DutyStations.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDutyStations> DeletedDutyStations = DeleteDutyStations(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.DutyStations.Delete, Apps.CMS), Container = "DutyStationFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(CommitedRows, DeletedDutyStations.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyDutyStation(model.DutyStationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DutyStationRestore(codeDutyStations model)
        {
            if (!CMS.HasAction(Permissions.DutyStations.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveDutyStation(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeDutyStations> RestoredDutyStations = RestoreDutyStations(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.DutyStations.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("DutyStation/Create", "Codes", new { Area = "CMS" })), Container = "DutyStationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.DutyStations.Update, Apps.CMS), Container = "DutyStationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.DutyStations.Delete, Apps.CMS), Container = "DutyStationFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(CommitedRows, RestoredDutyStations, DbCMS.PrimaryKeyControl(RestoredDutyStations.FirstOrDefault()), Url.Action(DataTableNames.DutyStationLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyDutyStation(model.DutyStationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DutyStationsDataTableDelete(List<codeDutyStations> models)
        {
            if (!CMS.HasAction(Permissions.DutyStations.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDutyStations> DeletedDutyStations = DeleteDutyStations(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedDutyStations, models, DataTableNames.DutyStationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DutyStationsDataTableRestore(List<codeDutyStations> models)
        {
            if (!CMS.HasAction(Permissions.DutyStations.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDutyStations> RestoredDutyStations = RestoreDutyStations(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredDutyStations, models, DataTableNames.DutyStationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeDutyStations> DeleteDutyStations(List<codeDutyStations> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;
            List<codeDutyStations> DeletedDutyStations = new List<codeDutyStations>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Delete, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeDutyStations>(query).ToList();
            foreach (var record in Records)
            {
                DeletedDutyStations.Add(DbCMS.Delete(record, ExecutionTime, DeleteActionGUID));
            }

            var Languages = DeletedDutyStations.SelectMany(x => x.codeDutyStationsLanguages).Where(x => x.Active).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Delete(language, ExecutionTime, DeleteActionGUID);
            }
            return DeletedDutyStations;
        }

        private List<codeDutyStations> RestoreDutyStations(List<codeDutyStations> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeDutyStations> RestoredDutyStations = new List<codeDutyStations>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeDutyStations>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveDutyStation(record))
                {
                    RestoredDutyStations.Add(DbCMS.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            var Languages = RestoredDutyStations.SelectMany(x => x.codeDutyStationsLanguages.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime);
            }

            return RestoredDutyStations;
        }

        private JsonResult ConcrrencyDutyStation(Guid PK)
        {
            DutyStationsUpdateModel dbModel = new DutyStationsUpdateModel();

            var DutyStation = DbCMS.codeDutyStations.Where(x => x.DutyStationGUID == PK).FirstOrDefault();
            var dbDutyStation = DbCMS.Entry(DutyStation).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbDutyStation, dbModel);

            var Language = DbCMS.codeDutyStationsLanguages.Where(x => x.DutyStationGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeDutyStations.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);


            if (DutyStation.codeDutyStationsRowVersion.SequenceEqual(dbModel.codeDutyStationsRowVersion) && Language.codeDutyStationsLanguagesRowVersion.SequenceEqual(dbModel.codeDutyStationsLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }


        private bool ActiveDutyStation(Object model)
        {
            codeDutyStations DutyStation = Mapper.Map(model, new codeDutyStations());
            codeDutyStationsLanguages DutyStationLanguage = Mapper.Map(model, new codeDutyStationsLanguages());

            int DutyStationDescription = DbCMS.codeDutyStationsLanguages
                                              .Where(x => x.DutyStationDescription == DutyStationLanguage.DutyStationDescription &&
                                                          x.DutyStationGUID != DutyStation.DutyStationGUID &&
                                                          x.LanguageID == LAN &&
                                                          x.Active).Count();

            if (DutyStationDescription > 0)
            {
                ModelState.AddModelError("DutyStationDescription", "Duty station description in selected language is already exists");
            }

            return DutyStationDescription > 0;
        }
        #endregion

        #region Duty Stations Language
        public ActionResult DutyStationLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/CMS/Views/Codes/DutyStations/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeDutyStationsLanguages, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeDutyStationsLanguages>(DataTable.Filters);
            }

            var Result = DbCMS.codeDutyStationsLanguages.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.DutyStationGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.DutyStationLanguageGUID,
                                  x.LanguageID,
                                  x.DutyStationDescription,
                                  x.codeDutyStationsLanguagesRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DutyStationLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.DutyStations.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/DutyStations/_LanguageUpdateModal.cshtml",
                new codeDutyStationsLanguages { DutyStationGUID = FK });
        }

        public ActionResult DutyStationLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.DutyStations.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/DutyStations/_LanguageUpdateModal.cshtml", DbCMS.codeDutyStationsLanguages.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DutyStationLanguageCreate(codeDutyStationsLanguages model)
        {
            if (!CMS.HasAction(Permissions.DutyStations.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveDutyStationLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/DutyStations/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Create(model, Permissions.DutyStationsLanguages.CreateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.DutyStationLanguagesDataTable,
                                   DbCMS.PrimaryKeyControl(model),
                                   DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DutyStationLanguageUpdate(codeDutyStationsLanguages model)
        {
            if (!CMS.HasAction(Permissions.DutyStations.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveDutyStationLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/DutyStations/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Update(model, Permissions.DutyStationsLanguages.UpdateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.DutyStationLanguagesDataTable,
                                   DbCMS.PrimaryKeyControl(model),
                                   DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyDutyStationLanguage(model.DutyStationLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DutyStationLanguageDelete(codeDutyStationsLanguages model)
        {
            if (!CMS.HasAction(Permissions.DutyStations.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDutyStationsLanguages> DeletedLanguages = DeleteDutyStationLanguages(new List<codeDutyStationsLanguages> { model });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.DutyStationLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyDutyStationLanguage(model.DutyStationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DutyStationLanguageRestore(codeDutyStationsLanguages model)
        {
            if (!CMS.HasAction(Permissions.DutyStations.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveDutyStationLanguage(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeDutyStationsLanguages> RestoredLanguages = RestoreDutyStationLanguages(Portal.SingleToList(model));

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.DutyStationLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyDutyStationLanguage(model.DutyStationLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DutyStationLanguagesDataTableDelete(List<codeDutyStationsLanguages> models)
        {
            if (!CMS.HasAction(Permissions.DutyStations.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDutyStationsLanguages> DeletedLanguages = DeleteDutyStationLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.DutyStationLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DutyStationLanguagesDataTableRestore(List<codeDutyStationsLanguages> models)
        {
            if (!CMS.HasAction(Permissions.DutyStations.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDutyStationsLanguages> RestoredLanguages = RestoreDutyStationLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.DutyStationLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeDutyStationsLanguages> DeleteDutyStationLanguages(List<codeDutyStationsLanguages> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeDutyStationsLanguages> DeletedDutyStationLanguages = new List<codeDutyStationsLanguages>();

            string query = DbCMS.QueryBuilder(models, Permissions.DutyStationsLanguages.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbCMS.Database.SqlQuery<codeDutyStationsLanguages>(query).ToList();

            foreach (var language in languages)
            {
                DeletedDutyStationLanguages.Add(DbCMS.Delete(language, ExecutionTime, Permissions.DutyStationsLanguages.DeleteGuid));
            }

            return DeletedDutyStationLanguages;
        }

        private List<codeDutyStationsLanguages> RestoreDutyStationLanguages(List<codeDutyStationsLanguages> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeDutyStationsLanguages> RestoredLanguages = new List<codeDutyStationsLanguages>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Languages = DbCMS.Database.SqlQuery<codeDutyStationsLanguages>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveDutyStationLanguage(language))
                {
                    RestoredLanguages.Add(DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyDutyStationLanguage(Guid PK)
        {
            codeDutyStationsLanguages dbModel = new codeDutyStationsLanguages();

            var Language = DbCMS.codeDutyStationsLanguages.Where(l => l.DutyStationLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeDutyStationsLanguagesRowVersion.SequenceEqual(dbModel.codeDutyStationsLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveDutyStationLanguage(codeDutyStationsLanguages model)
        {
            int LanguageID = DbCMS.codeDutyStationsLanguages
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.DutyStationGUID == model.DutyStationGUID &&
                                              x.DutyStationLanguageGUID != model.DutyStationLanguageGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Duty Station Name in selected language already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }

        #endregion


        #region Countries
        [Route("CMS/Codes/Countries/")]
        public ActionResult CountriesIndex()
        {
            if (!CMS.HasAction(Permissions.Countries.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/Countries/Index.cshtml");
        }

        [Route("CMS/Codes/CountriesDataTable/")]
        public JsonResult CountriesDataTable(DataTableRecievedOptions options)
        {
            Guid OrganizationInstanceGUID = Guid.Parse(Session[SessionKeys.OrganizationInstanceGUID].ToString());

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<CountryDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<CountryDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbCMS.codeCountries.AsExpandable()
                       join b in DbCMS.codeCountriesConfigurations.Where(x => x.OrganizationInstanceGUID == OrganizationInstanceGUID && x.Active) on a.CountryGUID equals b.CountryGUID
                       join c in DbCMS.codeCountriesLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeCountries.DeletedOn) && x.LanguageID == LAN) on a.CountryGUID equals c.CountryGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       select new CountryDataTableModel
                       {
                           CountryGUID = a.CountryGUID,
                           CountryA2Code = a.CountryA2Code,
                           CountryA3Code = a.CountryA3Code,
                           PhoneCode = a.PhoneCode,
                           Active = a.Active,
                           codeCountriesRowVersion = a.codeCountriesRowVersion,
                           CountryDescription = R1.CountryDescription,
                           Nationality = R1.Nationality,
                           Latitude = a.Latitude,
                           Longitude = a.Longitude,
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<CountryDataTableModel> Result = Mapper.Map<List<CountryDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("CMS/Codes/Countries/Create/")]
        public ActionResult CountryCreate()
        {
            if (!CMS.HasAction(Permissions.Countries.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/Countries/Country.cshtml", new CountryUpdateModel());
        }

        [Route("CMS/Codes/Countries/Update/{PK}")]
        public ActionResult CountryUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.Countries.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbCMS.codeCountries.WherePK(PK)
                         join b in DbCMS.codeCountriesLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeCountries.DeletedOn) && x.LanguageID == LAN)
                         on a.CountryGUID equals b.CountryGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new CountryUpdateModel
                         {
                             CountryGUID = a.CountryGUID,
                             CountryA2Code = a.CountryA2Code,
                             CountryA3Code = a.CountryA3Code,
                             Latitude = a.Latitude,
                             Longitude = a.Longitude,
                             PhoneCode = a.PhoneCode,
                             Active = a.Active,
                             codeCountriesRowVersion = a.codeCountriesRowVersion,
                             CountryDescription = R1.CountryDescription,
                             Nationality = R1.Nationality,
                             codeCountriesLanguagesRowVersion = R1.codeCountriesLanguagesRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("Countries", "Codes", new { Area = "CMS" }));

            return View("~/Areas/CMS/Views/Codes/Countries/Country.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CountryCreate(CountryUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Countries.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveCountry(model)) return PartialView("~/Areas/CMS/Views/Codes/Countries/_CountryForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeCountries Country = Mapper.Map(model, new codeCountries());
            Country.CountryGUID = EntityPK;
            DbCMS.Create(Country, Permissions.Countries.CreateGuid, ExecutionTime);

            Guid OrganizationInstanceGUID = Guid.Parse(HttpContext.Session[SessionKeys.OrganizationInstanceGUID].ToString());

            codeCountriesConfigurations CountryConfig = Mapper.Map(model, new codeCountriesConfigurations());
            CountryConfig.CountryConfigurationGUID = EntityPK;
            CountryConfig.OrganizationInstanceGUID = OrganizationInstanceGUID;
            DbCMS.Create(CountryConfig, Permissions.Countries.CreateGuid, ExecutionTime);


            codeCountriesLanguages Language = Mapper.Map(model, new codeCountriesLanguages());
            Language.CountryGUID = EntityPK;
            DbCMS.Create(Language, Permissions.CountriesLanguages.CreateGuid, ExecutionTime);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.CountryLanguagesDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Countries.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("Country/Create", "Codes", new { Area = "CMS" })), Container = "CountryFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Countries.Update, Apps.CMS), Container = "CountryFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Countries.Delete, Apps.CMS), Container = "CountryFormControls" });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleCreateMessage(DbCMS.PrimaryKeyControl(Country), DbCMS.RowVersionControls(Country, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CountryUpdate(CountryUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Countries.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveCountry(model)) return PartialView("~/Areas/CMS/Views/Codes/Countries/_CountryForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeCountries Country = Mapper.Map(model, new codeCountries());
            DbCMS.Update(Country, Permissions.Countries.UpdateGuid, ExecutionTime);

            var Language = DbCMS.codeCountriesLanguages.Where(x => x.CountryGUID == model.CountryGUID && x.LanguageID == LAN && x.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.CountryGUID = Country.CountryGUID;
                Language.Nationality = model.Nationality;
                DbCMS.Create(Language, Permissions.CountriesLanguages.CreateGuid, ExecutionTime);
            }
            else if (Language.CountryDescription != model.CountryDescription)
            {
                Language = Mapper.Map(model, Language);
                DbCMS.Update(Language, Permissions.CountriesLanguages.UpdateGuid, ExecutionTime);
            }

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, DbCMS.RowVersionControls(Country, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyCountry(model.CountryGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CountryDelete(codeCountries model)
        {
            if (!CMS.HasAction(Permissions.Countries.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeCountries> DeletedCountries = DeleteCountries(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.Countries.Delete, Apps.CMS), Container = "CountryFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(CommitedRows, DeletedCountries.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyCountry(model.CountryGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CountryRestore(codeCountries model)
        {
            if (!CMS.HasAction(Permissions.Countries.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveCountry(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeCountries> RestoredCountries = RestoreCountries(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Countries.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("Country/Create", "Codes", new { Area = "CMS" })), Container = "CountryFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Countries.Update, Apps.CMS), Container = "CountryFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Countries.Delete, Apps.CMS), Container = "CountryFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(CommitedRows, RestoredCountries, DbCMS.PrimaryKeyControl(RestoredCountries.FirstOrDefault()), Url.Action(DataTableNames.CountryLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyCountry(model.CountryGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CountriesDataTableDelete(List<codeCountries> models)
        {
            if (!CMS.HasAction(Permissions.Countries.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeCountries> DeletedCountries = DeleteCountries(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedCountries, models, DataTableNames.CountriesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CountriesDataTableRestore(List<codeCountries> models)
        {
            if (!CMS.HasAction(Permissions.Countries.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeCountries> RestoredCountries = RestoreCountries(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredCountries, models, DataTableNames.CountriesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeCountries> DeleteCountries(List<codeCountries> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;
            List<codeCountries> DeletedCountries = new List<codeCountries>();

            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Delete, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeCountries>(query).ToList();
            foreach (var record in Records)
            {
                DeletedCountries.Add(DbCMS.Delete(record, ExecutionTime, DeleteActionGUID));
            }

            var Languages = DeletedCountries.SelectMany(x => x.codeCountriesLanguages).Where(x => x.Active).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Delete(language, ExecutionTime, DeleteActionGUID);
            }
            return DeletedCountries;
        }

        private List<codeCountries> RestoreCountries(List<codeCountries> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeCountries> RestoredCountries = new List<codeCountries>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeCountries>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveCountry(record))
                {
                    RestoredCountries.Add(DbCMS.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            var Languages = RestoredCountries.SelectMany(l => l.codeCountriesLanguages.Where(x => x.DeletedOn == l.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveCountryLanguage(language))
                {
                    DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime);
                }
            }

            return RestoredCountries;
        }

        private JsonResult ConcrrencyCountry(Guid PK)
        {
            CountryUpdateModel dbModel = new CountryUpdateModel();

            var Country = DbCMS.codeCountries.Where(a => a.CountryGUID == PK).FirstOrDefault();
            var dbCountry = DbCMS.Entry(Country).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbCountry, dbModel);

            var Language = DbCMS.codeCountriesLanguages.Where(x => x.CountryGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeCountries.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Country.codeCountriesRowVersion.SequenceEqual(dbModel.codeCountriesRowVersion) && Language.codeCountriesLanguagesRowVersion.SequenceEqual(dbModel.codeCountriesLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveCountry(Object model)
        {
            codeCountries Country = Mapper.Map(model, new codeCountries());
            codeCountriesLanguages CountryLanguages = Mapper.Map(model, new codeCountriesLanguages());

            int CountryA2Code = DbCMS.codeCountries
                                     .Where(x => x.CountryA2Code == Country.CountryA2Code &&
                                                 x.CountryGUID != Country.CountryGUID &&
                                                 x.Active).Count();
            if (CountryA2Code > 0)
            {
                ModelState.AddModelError("CountryA2Code", "Country A2 Code is already exists");
            }

            int CountryA3Code = DbCMS.codeCountries
                                     .Where(x => x.CountryA3Code == Country.CountryA3Code &&
                                                 x.CountryGUID != Country.CountryGUID &&
                                                 x.Active).Count();
            if (CountryA2Code > 0)
            {
                ModelState.AddModelError("CountryA3Code", "Country A3 Code is already exists");
            }

            int CountryDescription = DbCMS.codeCountriesLanguages
                                          .Where(x => x.CountryDescription == CountryLanguages.CountryDescription &&
                                                      x.CountryGUID != Country.CountryGUID &&
                                                      x.LanguageID == LAN &&
                                                      x.Active).Count();

            if (CountryDescription > 0)
            {
                ModelState.AddModelError("CountryDescription", "Country Description in selected language already exists");
            }

            int Nationality = DbCMS.codeCountriesLanguages
                                   .Where(x => x.Nationality == CountryLanguages.Nationality &&
                                               x.CountryGUID != Country.CountryGUID &&
                                               x.LanguageID == LAN &&
                                               x.Active).Count();

            if (Nationality > 0)
            {
                ModelState.AddModelError("Nationality", "Nationality Description in selected language already exists");
            }

            return (CountryA2Code + CountryA3Code + CountryDescription + Nationality) > 0;
        }
        #endregion

        #region Country Languages
        public ActionResult CountryLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/CMS/Views/Codes/Countries/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeCountriesLanguages, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeCountriesLanguages>(DataTable.Filters);
            }

            var Result = DbCMS.codeCountriesLanguages.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.CountryGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.CountryLanguageGUID,
                                  x.LanguageID,
                                  x.CountryDescription,
                                  x.Nationality,
                                  x.codeCountriesLanguagesRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CountryLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.Countries.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/Countries/_LanguageUpdateModal.cshtml",
                new codeCountriesLanguages { CountryGUID = FK });
        }

        public ActionResult CountryLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.Countries.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/Countries/_LanguageUpdateModal.cshtml", DbCMS.codeCountriesLanguages.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CountryLanguageCreate(codeCountriesLanguages model)
        {
            if (!CMS.HasAction(Permissions.Countries.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveCountryLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/Countries/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Create(model, Permissions.CountriesLanguages.CreateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.CountryLanguagesDataTable,
                                   DbCMS.PrimaryKeyControl(model),
                                   DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CountryLanguageUpdate(codeCountriesLanguages model)
        {
            if (!CMS.HasAction(Permissions.Countries.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveCountryLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/Countries/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Update(model, Permissions.CountriesLanguages.UpdateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.CountryLanguagesDataTable,
                                   DbCMS.PrimaryKeyControl(model),
                                   DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyCountryLanguage(model.CountryLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CountryLanguageDelete(codeCountriesLanguages model)
        {
            if (!CMS.HasAction(Permissions.Countries.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeCountriesLanguages> DeletedLanguages = DeleteCountryLanguages(new List<codeCountriesLanguages> { model });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.CountryLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyCountryLanguage(model.CountryGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CountryLanguageRestore(codeCountriesLanguages model)
        {
            if (!CMS.HasAction(Permissions.Countries.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveCountryLanguage(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeCountriesLanguages> RestoredLanguages = RestoreCountryLanguages(Portal.SingleToList(model));

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.CountryLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyCountryLanguage(model.CountryLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CountryLanguagesDataTableDelete(List<codeCountriesLanguages> models)
        {
            if (!CMS.HasAction(Permissions.Countries.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeCountriesLanguages> DeletedLanguages = DeleteCountryLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.CountryLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CountryLanguagesDataTableRestore(List<codeCountriesLanguages> models)
        {
            if (!CMS.HasAction(Permissions.Countries.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeCountriesLanguages> RestoredLanguages = RestoreCountryLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.CountryLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeCountriesLanguages> DeleteCountryLanguages(List<codeCountriesLanguages> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeCountriesLanguages> DeletedCountryLanguages = new List<codeCountriesLanguages>();

            string query = DbCMS.QueryBuilder(models, Permissions.CountriesLanguages.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbCMS.Database.SqlQuery<codeCountriesLanguages>(query).ToList();

            foreach (var language in languages)
            {
                DeletedCountryLanguages.Add(DbCMS.Delete(language, ExecutionTime, Permissions.CountriesLanguages.DeleteGuid));
            }

            return DeletedCountryLanguages;
        }

        private List<codeCountriesLanguages> RestoreCountryLanguages(List<codeCountriesLanguages> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeCountriesLanguages> RestoredLanguages = new List<codeCountriesLanguages>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Languages = DbCMS.Database.SqlQuery<codeCountriesLanguages>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveCountryLanguage(language))
                {
                    RestoredLanguages.Add(DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyCountryLanguage(Guid PK)
        {
            codeCountriesLanguages dbModel = new codeCountriesLanguages();

            var Language = DbCMS.codeCountriesLanguages.Where(l => l.CountryLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeCountriesLanguagesRowVersion.SequenceEqual(dbModel.codeCountriesLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }
        private bool ActiveCountryLanguage(codeCountriesLanguages model)
        {
            int LanguageID = DbCMS.codeCountriesLanguages
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.CountryGUID == model.CountryGUID &&
                                              x.CountryLanguageGUID != model.CountryLanguageGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Country Name in selected language already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }

        #endregion


        #region Locations
        [Route("CMS/Codes/Locations/")]
        public ActionResult LocationsIndex()
        {
            return View("~/Areas/CMS/Views/Codes/Locations/Index.cshtml");
        }

        [Route("CMS/Codes/LocationsDataTable/")]
        public JsonResult LocationsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<LocationDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<LocationDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.Locations.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var All = (from a in DbCMS.codeLocations.AsExpandable().Where(x => AuthorizedList.Contains(x.CountryGUID.ToString()))
                       join b in DbCMS.codeLocationsLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeLocations.DeletedOn) && x.LanguageID == LAN) on a.LocationGUID equals b.LocationGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       select new LocationDataTableModel
                       {
                           LocationGUID = a.LocationGUID,
                           LocationDescription = R1.LocationDescription,
                           LocationTypeGUID = DbCMS.codeTablesValuesLanguages.Where(o => o.ValueGUID == a.LocationTypeGUID).Where(ol => ol.LanguageID == LAN).FirstOrDefault().ValueDescription,
                           CountryGUID = DbCMS.codeCountriesLanguages.Where(cl => cl.CountryGUID == a.CountryGUID && cl.LanguageID == LAN).FirstOrDefault().CountryDescription,
                           LocationlevelID = a.LocationlevelID,
                           LocationParentGUID = DbCMS.codeLocationsLanguages.Where(ll => ll.LocationGUID == a.LocationParentGUID && ll.LanguageID == LAN).FirstOrDefault().LocationDescription,
                           codeLocationsRowVersion = a.codeLocationsRowVersion,
                           Active = a.Active
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<LocationDataTableModel> Result = Mapper.Map<List<LocationDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        [Route("CMS/Codes/GetLocations")]
        public JsonResult GetLocations()
        {
            var All = (from a in DbCMS.codeLocations.AsExpandable()
                       join b in DbCMS.codeLocationsLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeLocations.DeletedOn) && x.LanguageID == LAN) on a.LocationGUID equals b.LocationGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       select new LocationDataTableModel
                       {
                           Latitude = a.Latitude,
                           Longitude = a.Longitude,
                           LocationGUID = a.LocationGUID,
                           LocationDescription = R1.LocationDescription,
                           LocationTypeGUID = DbCMS.codeTablesValuesLanguages.Where(o => o.ValueGUID == a.LocationTypeGUID).Where(ol => ol.LanguageID == LAN).FirstOrDefault().ValueDescription,
                           CountryGUID = DbCMS.codeCountriesLanguages.Where(cl => cl.CountryGUID == a.CountryGUID && cl.LanguageID == LAN).FirstOrDefault().CountryDescription,
                           LocationlevelID = a.LocationlevelID,
                           LocationParentGUID = DbCMS.codeLocationsLanguages.Where(ll => ll.LocationGUID == a.LocationParentGUID && ll.LanguageID == LAN).FirstOrDefault().LocationDescription,
                           codeLocationsRowVersion = a.codeLocationsRowVersion,
                           Active = a.Active
                       }).ToList();
            return Json(All, JsonRequestBehavior.AllowGet);
        }

        [Route("CMS/Codes/Locations/Create/")]
        public ActionResult LocationCreate()
        {
            if (!CMS.HasAction(Permissions.Locations.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/Locations/Location.cshtml", new LocationsUpdateModel());
        }

        [Route("CMS/Codes/Locations/Update/{PK}")]
        public ActionResult LocationUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.Locations.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbCMS.codeLocations.WherePK(PK)
                         join b in DbCMS.codeLocationsLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeLocations.DeletedOn) && x.LanguageID == LAN)
                         on a.LocationGUID equals b.LocationGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new LocationsUpdateModel
                         {
                             LocationGUID = a.LocationGUID,
                             CountryGUID = a.CountryGUID,
                             Latitude = a.Latitude,
                             Longitude = a.Longitude,
                             LocationlevelID = a.LocationlevelID,
                             LocationPCode = a.LocationPCode,
                             LocationParentGUID = a.LocationParentGUID,
                             LocationTypeGUID = a.LocationTypeGUID,
                             LocationDescription = R1.LocationDescription,
                             Active = a.Active,
                             codeLocationsRowVersion = a.codeLocationsRowVersion,
                             codeLocationsLanguagesRowVersion = R1.codeLocationsLanguagesRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("Locations", "Codes", new { Area = "CMS" }));

            return View("~/Areas/CMS/Views/Codes/Locations/Location.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult LocationCreate(LocationsUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Locations.Create, Apps.CMS, model.CountryGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveLocation(model)) return PartialView("~/Areas/CMS/Views/Codes/Locations/_LocationForm.cshtml", model);
            if (model.LocationlevelID == 1) model.LocationParentGUID = null; // to skip the parent when location is level 1 (Governorate): REMEMBER: ParentGUID should be not null on the model while on db should allow null

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeLocations Location = Mapper.Map(model, new codeLocations());

            Location.LocationGUID = EntityPK;
            DbCMS.Create(Location, Permissions.Locations.CreateGuid, ExecutionTime);

            codeLocationsLanguages Language = Mapper.Map(model, new codeLocationsLanguages());
            Language.LocationGUID = EntityPK;
            DbCMS.Create(Language, Permissions.LocationsLanguages.CreateGuid, ExecutionTime);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.LocationLanguagesDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Locations.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("Location/Create", "Codes", new { Area = "CMS" })), Container = "LocationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Locations.Update, Apps.CMS), Container = "LocationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Locations.Delete, Apps.CMS), Container = "LocationFormControls" });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleCreateMessage(DbCMS.PrimaryKeyControl(Location), DbCMS.RowVersionControls(Location, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult LocationUpdate(LocationsUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Locations.Update, Apps.CMS, model.CountryGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveLocation(model)) return PartialView("~/Areas/CMS/Views/Codes/Locations/_LocationForm.cshtml", model);
            if (model.LocationlevelID == 1) model.LocationParentGUID = null; // to skip the parent when location is level 1 (Governorate): REMEMBER: ParentGUID should be not null on the model while on db should allow null

            DateTime ExecutionTime = DateTime.Now;

            codeLocations Location = Mapper.Map(model, new codeLocations());
            DbCMS.Update(Location, Permissions.Locations.UpdateGuid, ExecutionTime);

            var Language = DbCMS.codeLocationsLanguages.Where(l => l.LocationGUID == model.LocationGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.LocationGUID = Location.LocationGUID;
                DbCMS.Create(Language, Permissions.LocationsLanguages.CreateGuid, ExecutionTime);
            }
            else if (Language.LocationDescription != model.LocationDescription)
            {
                Language = Mapper.Map(model, Language);
                DbCMS.Update(Language, Permissions.LocationsLanguages.UpdateGuid, ExecutionTime);
            }

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, DbCMS.RowVersionControls(Location, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyLocation(model.LocationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult LocationDelete(codeLocations model)
        {
            if (!CMS.HasAction(Permissions.Locations.Delete, Apps.CMS, model.CountryGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeLocations> DeletedLocations = DeleteLocations(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.Locations.Delete, Apps.CMS), Container = "LocationFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(CommitedRows, DeletedLocations.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyLocation(model.LocationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult LocationRestore(codeLocations model)
        {
            if (!CMS.HasAction(Permissions.Locations.Restore, Apps.CMS, model.CountryGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveLocation(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeLocations> RestoredLocations = RestoreLocations(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Locations.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("Location/Create", "Codes", new { Area = "CMS" })), Container = "LocationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Locations.Update, Apps.CMS), Container = "LocationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Locations.Delete, Apps.CMS), Container = "LocationFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(CommitedRows, RestoredLocations, DbCMS.PrimaryKeyControl(RestoredLocations.FirstOrDefault()), Url.Action(DataTableNames.LocationLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyLocation(model.LocationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult LocationsDataTableDelete(List<codeLocations> models)
        {
            if (!CMS.HasAction(Permissions.Locations.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeLocations> DeletedLocations = DeleteLocations(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedLocations, models, DataTableNames.LocationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult LocationsDataTableRestore(List<codeLocations> models)
        {
            if (!CMS.HasAction(Permissions.Locations.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeLocations> RestoredLocations = RestoreLocations(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredLocations, models, DataTableNames.LocationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeLocations> DeleteLocations(List<codeLocations> models)
        {
            Guid DeleteActionGUID = Permissions.Locations.DeleteGuid;
            DateTime ExecutionTime = DateTime.Now;
            List<codeLocations> DeletedLocations = new List<codeLocations>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "SELECT LocationGUID,CONVERT(varchar(50), CountryGUID) as C2 ,codeLocationsRowVersion FROM code.codeLocations where LocationGUID in (" + string.Join(",", models.Select(x => "'" + x.LocationGUID + "'").ToArray()) + ")";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Delete, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeLocations>(query).ToList();
            foreach (var record in Records)
            {
                DeletedLocations.Add(DbCMS.Delete(record, ExecutionTime, DeleteActionGUID));
            }

            var Languages = DeletedLocations.SelectMany(a => a.codeLocationsLanguages).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Delete(language, ExecutionTime, DeleteActionGUID);
            }
            return DeletedLocations;
        }

        private List<codeLocations> RestoreLocations(List<codeLocations> models)
        {
            Guid RestoreActionGUID = Permissions.Locations.RestoreGuid;
            DateTime RestoringTime = DateTime.Now;

            List<codeLocations> RestoredLocations = new List<codeLocations>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "SELECT LocationGUID,CONVERT(varchar(100), CountryGUID) as C2 ,codeLocationsRowVersion FROM code.codeLocations where LocationGUID in (" + string.Join(",", models.Select(x => "'" + x.LocationGUID + "'").ToArray()) + ")";

            string query = DbCMS.QueryBuilder(models, RestoreActionGUID, SubmitTypes.Restore, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeLocations>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveLocation(record))
                {
                    RestoredLocations.Add(DbCMS.Restore(record, RestoreActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            var Languages = RestoredLocations.SelectMany(l => l.codeLocationsLanguages.Where(x => x.DeletedOn == l.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Restore(language, RestoreActionGUID, RestoreActionGUID, RestoringTime);
            }

            return RestoredLocations;
        }

        private JsonResult ConcrrencyLocation(Guid PK)
        {
            LocationsUpdateModel dbModel = new LocationsUpdateModel();

            var Location = DbCMS.codeLocations.Where(a => a.LocationGUID == PK).FirstOrDefault();
            var dbLocation = DbCMS.Entry(Location).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLocation, dbModel);

            var Language = DbCMS.codeLocationsLanguages.Where(x => x.LocationGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeLocations.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Location.codeLocationsRowVersion.SequenceEqual(dbModel.codeLocationsRowVersion) && Language.codeLocationsLanguagesRowVersion.SequenceEqual(dbModel.codeLocationsLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveLocation(Object model)
        {
            codeLocationsLanguages LocationLan = Mapper.Map(model, new codeLocationsLanguages());
            codeLocations Location = Mapper.Map(model, new codeLocations());
            int LocationDescription = DbCMS.codeLocationsLanguages
                                    .Where(x => x.LocationDescription == LocationLan.LocationDescription &&
                                                x.LocationGUID != LocationLan.LocationGUID &&
                                                x.codeLocations.LocationTypeGUID == Location.LocationTypeGUID &&
                                                x.Active).Count();
            if (LocationDescription > 0)
            {
                ModelState.AddModelError("LocationDescription", "Location is already exists");
            }

            return (LocationDescription > 0);
        }

        #endregion

        #region Locations Language
        public ActionResult LocationLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/CMS/Views/Codes/Locations/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeLocationsLanguages, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeLocationsLanguages>(DataTable.Filters);
            }

            var Result = DbCMS.codeLocationsLanguages.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.LocationGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.LocationLanguageGUID,
                                  x.LanguageID,
                                  x.LocationDescription,
                                  x.codeLocationsLanguagesRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LocationLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.Locations.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/Locations/_LanguageUpdateModal.cshtml",
                new codeLocationsLanguages { LocationGUID = FK });
        }

        public ActionResult LocationLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.Locations.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/Locations/_LanguageUpdateModal.cshtml", DbCMS.codeLocationsLanguages.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult LocationLanguageCreate(codeLocationsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Locations.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveLocationLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/Locations/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Create(model, Permissions.LocationsLanguages.CreateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.LocationLanguagesDataTable,
                                   DbCMS.PrimaryKeyControl(model),
                                   DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult LocationLanguageUpdate(codeLocationsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Locations.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveLocationLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/Locations/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Update(model, Permissions.LocationsLanguages.UpdateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.LocationLanguagesDataTable,
                                   DbCMS.PrimaryKeyControl(model),
                                   DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyLocationLanguage(model.LocationLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult LocationLanguageDelete(codeLocationsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Locations.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeLocationsLanguages> DeletedLanguages = DeleteLocationLanguages(new List<codeLocationsLanguages> { model });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.LocationLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyLocationLanguage(model.LocationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult LocationLanguageRestore(codeLocationsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Locations.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveLocationLanguage(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeLocationsLanguages> RestoredLanguages = RestoreLocationLanguages(Portal.SingleToList(model));

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.LocationLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyLocationLanguage(model.LocationLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult LocationLanguagesDataTableDelete(List<codeLocationsLanguages> models)
        {
            if (!CMS.HasAction(Permissions.Locations.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeLocationsLanguages> DeletedLanguages = DeleteLocationLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.LocationLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult LocationLanguagesDataTableRestore(List<codeLocationsLanguages> models)
        {
            if (!CMS.HasAction(Permissions.Locations.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeLocationsLanguages> RestoredLanguages = RestoreLocationLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.LocationLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeLocationsLanguages> DeleteLocationLanguages(List<codeLocationsLanguages> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeLocationsLanguages> DeletedLocationLanguages = new List<codeLocationsLanguages>();

            string query = DbCMS.QueryBuilder(models, Permissions.LocationsLanguages.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbCMS.Database.SqlQuery<codeLocationsLanguages>(query).ToList();

            foreach (var language in languages)
            {
                DeletedLocationLanguages.Add(DbCMS.Delete(language, ExecutionTime, Permissions.LocationsLanguages.DeleteGuid));
            }

            return DeletedLocationLanguages;
        }

        private List<codeLocationsLanguages> RestoreLocationLanguages(List<codeLocationsLanguages> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeLocationsLanguages> RestoredLanguages = new List<codeLocationsLanguages>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Languages = DbCMS.Database.SqlQuery<codeLocationsLanguages>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveLocationLanguage(language))
                {
                    RestoredLanguages.Add(DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyLocationLanguage(Guid PK)
        {
            codeLocationsLanguages dbModel = new codeLocationsLanguages();

            var Language = DbCMS.codeLocationsLanguages.Where(l => l.LocationLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeLocationsLanguagesRowVersion.SequenceEqual(dbModel.codeLocationsLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveLocationLanguage(codeLocationsLanguages model)
        {
            int LanguageID = DbCMS.codeLocationsLanguages
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.LocationGUID == model.LocationGUID &&
                                              x.LocationLanguageGUID != model.LocationLanguageGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Location Name in selected language already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }
        #endregion


        #region Lookup Tables Values
        [Route("CMS/Codes/VCT/{TableID}/")]
        public ActionResult TablesValuesIndex(Guid TableID)
        {
            if (!CMS.HasAction(Permissions.LookupValues.Access, Apps.CMS, TableID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            var CodeTable = DbCMS.codeTables.Find(TableID);

            return View("~/Areas/CMS/Views/Codes/LookupValues/Index.cshtml", null, new LookupTable { TableID = TableID, TableName = CodeTable.TableName, IndexSitemapID = CodeTable.IndexSitemapGUID });
        }

        [Route("CMS/Codes/TableValuesDataTable/{PK}")]
        public JsonResult TableValuesDataTable(DataTableRecievedOptions options, Guid PK)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<TableValuesDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<TableValuesDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbCMS.codeTablesValues.Where(tv => tv.TableGUID == PK).AsExpandable()
                       join b in DbCMS.codeTablesValuesLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeTablesValues.DeletedOn) && x.LanguageID == LAN) on a.ValueGUID equals b.ValueGUID into LJ1

                       from R1 in LJ1.DefaultIfEmpty()
                       select new TableValuesDataTableModel
                       {
                           ValueGUID = a.ValueGUID,
                           ValueDescription = R1.ValueDescription,
                           SortID = a.SortID,
                           codeTablesValuesRowVersion = a.codeTablesValuesRowVersion,
                           Active = a.Active

                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);
            List<TableValuesDataTableModel> Result = Mapper.Map<List<TableValuesDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());
            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("CMS/Codes/Create/{FK}")]
        public ActionResult TablesValueCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.LookupValues.Create, Apps.CMS, FK.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            TableValuesUpdateModel TableValuesUpdateModel = new TableValuesUpdateModel();
            TableValuesUpdateModel.DetailsSitemapGUID = DbCMS.codeTables.Find(FK).DetailsSitemapGUID;
            TableValuesUpdateModel.TableGUID = FK;

            return View("~/Areas/CMS/Views/Codes/LookupValues/TableValue.cshtml", TableValuesUpdateModel);
        }

        [Route("CMS/Codes/Update/{PK}")]
        public ActionResult TableValuesUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.LookupValues.Access, Apps.CMS, FactorsCollector.LookupValue(PK)))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbCMS.codeTablesValues.WherePK(PK)
                         join b in DbCMS.codeTablesValuesLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeTablesValues.DeletedOn) && x.LanguageID == LAN) on a.ValueGUID equals b.ValueGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new TableValuesUpdateModel
                         {
                             ValueGUID = a.ValueGUID,
                             SortID = a.SortID,
                             TableGUID = a.TableGUID,
                             DetailsSitemapGUID = a.codeTables.DetailsSitemapGUID,
                             ValueDescription = R1.ValueDescription,
                             Active = a.Active,
                             codeTablesValuesRowVersion = a.codeTablesValuesRowVersion,
                             codeTablesValuesLanguagesRowVersion = R1.codeTablesValuesLanguagesRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("TablesValues", "Codes", new { Area = "CMS" }));

            return View("~/Areas/CMS/Views/Codes/LookupValues/TableValue.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TableValuesCreate(TableValuesUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.LookupValues.Create, Apps.CMS, model.TableGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveTablesValue(model)) return PartialView("~/Areas/CMS/Views/Codes/LookupValues/_TableValueForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeTablesValues TablesValue = Mapper.Map(model, new codeTablesValues());
            TablesValue.ValueGUID = EntityPK;
            DbCMS.Create(TablesValue, Permissions.LookupValues.CreateGuid, ExecutionTime);

            codeTablesValuesLanguages Language = Mapper.Map(model, new codeTablesValuesLanguages());
            Language.ValueGUID = EntityPK;
            DbCMS.Create(Language, Permissions.LookupValuesLanguages.CreateGuid, ExecutionTime);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.TableValuesLanguagesDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.LookupValues.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("TableValues/Create", "Codes", new { Area = "CMS" })), Container = "TableValuesFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.LookupValues.Update, Apps.CMS), Container = "TableValuesFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.LookupValues.Delete, Apps.CMS), Container = "TableValuesFormControls" });


            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleCreateMessage(DbCMS.PrimaryKeyControl(TablesValue), DbCMS.RowVersionControls(TablesValue, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TableValuesUpdate(TableValuesUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.LookupValues.Update, Apps.CMS, model.TableGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveTablesValue(model)) return PartialView("~/Areas/CMS/Views/Codes/LookupValues/_TablesValueForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeTablesValues TablesValue = Mapper.Map(model, new codeTablesValues());
            DbCMS.Update(TablesValue, Permissions.LookupValues.UpdateGuid, ExecutionTime);

            var Language = DbCMS.codeTablesValuesLanguages.Where(l => l.ValueGUID == model.ValueGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.ValueGUID = TablesValue.ValueGUID;
                DbCMS.Create(Language, Permissions.LookupValuesLanguages.CreateGuid, ExecutionTime);
            }
            else if (Language.ValueDescription != model.ValueDescription)
            {
                Language = Mapper.Map(model, Language);
                DbCMS.Update(Language, Permissions.LookupValuesLanguages.UploadGuid, ExecutionTime);
            }

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, DbCMS.RowVersionControls(TablesValue, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyTablesValue(model.ValueGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TableValuesDelete(codeTablesValues model)
        {
            if (!CMS.HasAction(Permissions.LookupValues.Delete, Apps.CMS, model.TableGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeTablesValues> DeletedTablesValues = DeleteTableValues(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.LookupValues.Restore, Apps.CMS), Container = "TableValuesFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(CommitedRows, DeletedTablesValues.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyTablesValue(model.ValueGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TableValuesRestore(codeTablesValues model)
        {
            if (!CMS.HasAction(Permissions.LookupValues.Restore, Apps.CMS, model.TableGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveTablesValue(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeTablesValues> RestoredTablesValues = RestoreTableValues(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.LookupValues.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("TableValues/Create", "Codes", new { Area = "CMS" })), Container = "TableValuesCreate" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.LookupValues.Update, Apps.CMS), Container = "TableValuesCreate" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.LookupValues.Delete, Apps.CMS), Container = "TableValuesCreate" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(CommitedRows, RestoredTablesValues, DbCMS.PrimaryKeyControl(RestoredTablesValues.FirstOrDefault()), Url.Action(DataTableNames.TableValuesLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyTablesValue(model.ValueGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult TableValuesDataTableDelete(List<codeTablesValues> models)
        {
            if (!CMS.HasAction(Permissions.LookupValues.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeTablesValues> DeletedTablesValues = DeleteTableValues(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedTablesValues, models, DataTableNames.TableValuesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult TableValuesDataTableRestore(List<codeTablesValues> models)
        {
            if (!CMS.HasAction(Permissions.LookupValues.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeTablesValues> RestoredTablesValues = RestoreTableValues(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredTablesValues, models, DataTableNames.TableValuesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeTablesValues> DeleteTableValues(List<codeTablesValues> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;
            List<codeTablesValues> DeletedTablesValues = new List<codeTablesValues>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Delete, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeTablesValues>(query).ToList();
            foreach (var record in Records)
            {
                DeletedTablesValues.Add(DbCMS.Delete(record, ExecutionTime, DeleteActionGUID));
            }

            var Languages = DeletedTablesValues.SelectMany(a => a.codeTablesValuesLanguages).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Delete(language, ExecutionTime, DeleteActionGUID);
            }
            return DeletedTablesValues;
        }

        private List<codeTablesValues> RestoreTableValues(List<codeTablesValues> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeTablesValues> RestoredTablesValues = new List<codeTablesValues>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeTablesValues>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveTablesValue(record))
                {
                    RestoredTablesValues.Add(DbCMS.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            var Languages = RestoredTablesValues.SelectMany(x => x.codeTablesValuesLanguages.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveTablesValueLanguage(language))
                {
                    DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime);
                }
            }

            return RestoredTablesValues;
        }

        private JsonResult ConcrrencyTablesValue(Guid PK)
        {
            TableValuesUpdateModel dbModel = new TableValuesUpdateModel();

            var TablesValue = DbCMS.codeTablesValues.Where(a => a.ValueGUID == PK).FirstOrDefault();
            var dbTablesValue = DbCMS.Entry(TablesValue).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbTablesValue, dbModel);

            var Language = DbCMS.codeTablesValuesLanguages.Where(p => p.ValueGUID == PK).Where(p => (p.Active == true ? p.Active : p.DeletedOn == p.codeTablesValues.DeletedOn) && p.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (TablesValue.codeTablesValuesRowVersion.SequenceEqual(dbModel.codeTablesValuesRowVersion) && Language.codeTablesValuesLanguagesRowVersion.SequenceEqual(dbModel.codeTablesValuesLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveTablesValue(Object model)
        {
            return false;
            codeTablesValuesLanguages TablesValue = Mapper.Map(model, new codeTablesValuesLanguages());
            int TablesValueDescription = DbCMS.codeTablesValuesLanguages
                                    .Where(x => x.ValueDescription == TablesValue.ValueDescription &&
                                                x.ValueGUID != TablesValue.ValueGUID &&
                                                x.Active).Count();
            if (TablesValueDescription > 0)
            {
                ModelState.AddModelError("", "Value is already exists");
            }

            return (TablesValueDescription > 0);
        }
        #endregion

        #region Lookup Tables Values Language
        public ActionResult TableValuesLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/CMS/Views/Codes/LookupValues/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeTablesValuesLanguages, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeTablesValuesLanguages>(DataTable.Filters);
            }

            var Result = DbCMS.codeTablesValuesLanguages.AsExpandable().Where(x => x.LanguageID != LAN && x.ValueGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.TableValueLanguageGUID,
                                  x.LanguageID,
                                  x.ValueDescription,
                                  x.codeTablesValuesLanguagesRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }


        public ActionResult TableValuesLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.LookupValues.Create, Apps.CMS, FactorsCollector.LookupValue(FK)))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/LookupValues/_LanguageUpdateModal.cshtml",
                new codeTablesValuesLanguages { ValueGUID = FK });
        }

        public ActionResult TableValuesLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.LookupValues.Access, Apps.CMS, FactorsCollector.LookupValueLanguage(PK)))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/LookupValues/_LanguageUpdateModal.cshtml", DbCMS.codeTablesValuesLanguages.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TableValuesLanguageCreate(codeTablesValuesLanguages model)
        {
            if (!CMS.HasAction(Permissions.LookupValues.Create, Apps.CMS, FactorsCollector.LookupValue(model.ValueGUID)))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveTablesValueLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/LookupValues/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Create(model, Permissions.LookupValuesLanguages.CreateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.TableValuesLanguagesDataTable,
                                   DbCMS.PrimaryKeyControl(model),
                                   DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TableValuesLanguageUpdate(codeTablesValuesLanguages model)
        {
            if (!CMS.HasAction(Permissions.LookupValues.Update, Apps.CMS, FactorsCollector.LookupValue(model.ValueGUID)))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveTablesValueLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/LookupValues/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Update(model, Permissions.LookupValuesLanguages.UploadGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.TableValuesLanguagesDataTable,
                                   DbCMS.PrimaryKeyControl(model),
                                   DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyTablesValueLanguage(model.TableValueLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TableValuesLanguageDelete(codeTablesValuesLanguages model)
        {
            if (!CMS.HasAction(Permissions.LookupValues.Delete, Apps.CMS, FactorsCollector.LookupValue(model.ValueGUID)))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<codeTablesValuesLanguages> DeletedLanguages = DeleteTableValuesLanguages(new List<codeTablesValuesLanguages> { model });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.TableValuesLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyTablesValueLanguage(model.ValueGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TableValuesLanguageRestore(codeTablesValuesLanguages model)
        {
            if (!CMS.HasAction(Permissions.LookupValues.Restore, Apps.CMS, FactorsCollector.LookupValue(model.ValueGUID)))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (ActiveTablesValueLanguage(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeTablesValuesLanguages> RestoredLanguages = RestoreTableValueLanguages(Portal.SingleToList(model));

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.TableValuesLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyTablesValueLanguage(model.TableValueLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult TableValuesLanguagesDataTableDelete(List<codeTablesValuesLanguages> models)
        {
            if (!CMS.HasAction(Permissions.LookupValues.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeTablesValuesLanguages> DeletedLanguages = DeleteTableValuesLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.TableValuesLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult TableValuesLanguagesDataTableRestore(List<codeTablesValuesLanguages> models)
        {
            if (!CMS.HasAction(Permissions.LookupValues.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeTablesValuesLanguages> RestoredLanguages = RestoreTableValueLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.TableValuesLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeTablesValuesLanguages> DeleteTableValuesLanguages(List<codeTablesValuesLanguages> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeTablesValuesLanguages> DeletedTablesValueLanguages = new List<codeTablesValuesLanguages>();

            string query = DbCMS.QueryBuilder(models, Permissions.LookupValuesLanguages.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbCMS.Database.SqlQuery<codeTablesValuesLanguages>(query).ToList();

            foreach (var language in languages)
            {
                DeletedTablesValueLanguages.Add(DbCMS.Delete(language, ExecutionTime, Permissions.LookupValuesLanguages.DeleteGuid));
            }

            return DeletedTablesValueLanguages;
        }

        private List<codeTablesValuesLanguages> RestoreTableValueLanguages(List<codeTablesValuesLanguages> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeTablesValuesLanguages> RestoredLanguages = new List<codeTablesValuesLanguages>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Languages = DbCMS.Database.SqlQuery<codeTablesValuesLanguages>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveTablesValueLanguage(language))
                {
                    RestoredLanguages.Add(DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyTablesValueLanguage(Guid PK)
        {
            codeTablesValuesLanguages dbModel = new codeTablesValuesLanguages();

            var Language = DbCMS.codeTablesValuesLanguages.Where(l => l.TableValueLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeTablesValuesLanguagesRowVersion.SequenceEqual(dbModel.codeTablesValuesLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveTablesValueLanguage(codeTablesValuesLanguages model)
        {
            int LanguageID = DbCMS.codeTablesValuesLanguages
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.ValueGUID == model.ValueGUID &&
                                              x.TableValueLanguageGUID != model.TableValueLanguageGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Tables Value Name in selected language already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }
        #endregion


        #region Operations
        [Route("CMS/Codes/Operations/")]
        public ActionResult OperationsIndex()
        {
            if (!CMS.HasAction(Permissions.Operations.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/Operations/Index.cshtml");
        }

        [Route("CMS/Codes/OperationsDataTable/")]
        public JsonResult OperationsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<OperationDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<OperationDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbCMS.codeOperations.AsExpandable()
                       join b in DbCMS.codeOperationsLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeOperations.DeletedOn) && x.LanguageID == LAN) on a.OperationGUID equals b.OperationGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join c in DbCMS.codeCountriesLanguages.Where(x => x.LanguageID == LAN) on a.CountryGUID equals c.CountryGUID into LJ2
                       from R2 in LJ2.DefaultIfEmpty()
                       join e in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN) on a.BureauGUID equals e.ValueGUID into LJ3
                       from R3 in LJ3.DefaultIfEmpty()

                       select new OperationDataTableModel
                       {
                           OperationGUID = a.OperationGUID,
                           OperationDescription = R1.OperationDescription,
                           BureauDescription = R3.ValueDescription,
                           CountryDescription = R2.CountryDescription,
                           Active = a.Active,
                           codeOperationsRowVersion = a.codeOperationsRowVersion,
                       }).Where(Predicate);


            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<OperationDataTableModel> Result = Mapper.Map<List<OperationDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("CMS/Codes/Operations/Create/")]
        public ActionResult OperationCreate()
        {
            if (!CMS.HasAction(Permissions.Operations.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/Operations/Operation.cshtml", new OperationUpdateModel());
        }

        [Route("CMS/Codes/Operations/Update/{PK}")]
        public ActionResult OperationUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.Operations.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbCMS.codeOperations.WherePK(PK)
                         join b in DbCMS.codeOperationsLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeOperations.DeletedOn) && x.LanguageID == LAN)
                         on a.OperationGUID equals b.OperationGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new OperationUpdateModel
                         {
                             OperationGUID = a.OperationGUID,
                             OperationDescription = R1.OperationDescription,
                             BureauGUID = a.BureauGUID,
                             CountryGUID = a.CountryGUID,
                             Active = a.Active,
                             codeOperationsRowVersion = a.codeOperationsRowVersion,
                             codeOperationsLanguagesRowVersion = R1.codeOperationsLanguagesRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("Operations", "Codes", new { Area = "CMS" }));

            return View("~/Areas/CMS/Views/Codes/Operations/Operation.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OperationCreate(OperationUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Operations.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid /*|| ActiveOperation(Operation)*/) return PartialView("~/Areas/CMS/Views/Codes/Operations/_OperationForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeOperations Operation = Mapper.Map(model, new codeOperations());
            Operation.OperationGUID = EntityPK;
            DbCMS.Create(Operation, Permissions.Operations.CreateGuid, ExecutionTime);

            codeOperationsLanguages Languages = Mapper.Map(model, new codeOperationsLanguages());
            Languages.OperationGUID = Operation.OperationGUID;
            DbCMS.Create(Languages, Permissions.OperationsLanguages.CreateGuid, ExecutionTime);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.OperationLanguagesDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Operations.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("Opreation/Create", "Codes", new { Area = "CMS" })), Container = "OperationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Operations.Update, Apps.CMS), Container = "OperationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Operations.Delete, Apps.CMS), Container = "OperationFormControls" });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleCreateMessage(DbCMS.PrimaryKeyControl(Operation), DbCMS.RowVersionControls(Operation, Languages), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OperationUpdate(OperationUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Operations.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveOperation(model)) return PartialView("~/Areas/CMS/Views/Codes/Operations/_OperationForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeOperations Operation = Mapper.Map(model, new codeOperations());
            DbCMS.Update(Operation, Permissions.Operations.UpdateGuid, ExecutionTime);

            var Language = DbCMS.codeOperationsLanguages.Where(l => l.OperationGUID == model.OperationGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.OperationGUID = Operation.OperationGUID;
                DbCMS.Create(Language, Permissions.OperationsLanguages.CreateGuid, ExecutionTime);
            }
            else if (Language.OperationDescription != model.OperationDescription)
            {
                Language = Mapper.Map(model, Language);
                DbCMS.Update(Language, Permissions.OperationsLanguages.UpdateGuid, ExecutionTime);
            }

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, DbCMS.RowVersionControls(Operation, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyOperation(model.OperationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OperationDelete(codeOperations model)
        {
            if (!CMS.HasAction(Permissions.Operations.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeOperations> DeletedOperations = DeleteOperations(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.Operations.Restore, Apps.CMS), Container = "OperationFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(CommitedRows, DeletedOperations.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyOperation(model.OperationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OperationRestore(codeOperations model)
        {
            if (!CMS.HasAction(Permissions.Operations.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveOperation(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeOperations> RestoredOperations = RestoreOperations(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Operations.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("Opreation/Create", "Codes", new { Area = "CMS" })), Container = "OperationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Operations.Update, Apps.CMS), Container = "OperationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Operations.Delete, Apps.CMS), Container = "OperationFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(CommitedRows, RestoredOperations, DbCMS.PrimaryKeyControl(RestoredOperations.FirstOrDefault()), Url.Action(DataTableNames.OperationLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyOperation(model.OperationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult OperationsDataTableDelete(List<codeOperations> models)
        {
            if (!CMS.HasAction(Permissions.Operations.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeOperations> DeletedOperations = DeleteOperations(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedOperations, models, DataTableNames.OperationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult OperationsDataTableRestore(List<codeOperations> models)
        {
            if (!CMS.HasAction(Permissions.Operations.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeOperations> RestoredOperations = RestoreOperations(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredOperations, models, DataTableNames.OperationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeOperations> DeleteOperations(List<codeOperations> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;
            List<codeOperations> DeletedOperations = new List<codeOperations>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Delete, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeOperations>(query).ToList();
            foreach (var record in Records)
            {
                DeletedOperations.Add(DbCMS.Delete(record, ExecutionTime, DeleteActionGUID));
            }

            var Languages = DeletedOperations.SelectMany(a => a.codeOperationsLanguages).Where(x => x.Active).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Delete(language, ExecutionTime, DeleteActionGUID);
            }
            return DeletedOperations;
        }

        private List<codeOperations> RestoreOperations(List<codeOperations> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeOperations> RestoredOperations = new List<codeOperations>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeOperations>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveOperation(record))
                {
                    RestoredOperations.Add(DbCMS.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            var Languages = RestoredOperations.SelectMany(x => x.codeOperationsLanguages.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime);
            }

            return RestoredOperations;
        }

        private JsonResult ConcrrencyOperation(Guid PK)
        {
            OperationUpdateModel dbModel = new OperationUpdateModel();

            var Operation = DbCMS.codeOperations.Where(x => x.OperationGUID == PK).FirstOrDefault();
            var dbOperation = DbCMS.Entry(Operation).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbOperation, dbModel);

            var Language = DbCMS.codeOperationsLanguages.Where(x => x.OperationGUID == PK).Where(p => (p.Active == true ? p.Active : p.DeletedOn == p.codeOperations.DeletedOn) && p.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Operation.codeOperationsRowVersion.SequenceEqual(dbModel.codeOperationsRowVersion) && Language.codeOperationsLanguagesRowVersion.SequenceEqual(dbModel.codeOperationsLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveOperation(Object model)
        {
            //This function is not no longer needed. Ayas
            return false;
        }

        #endregion

        #region Operation Languages
        public ActionResult OperationLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/CMS/Views/Codes/Operations/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeOperationsLanguages, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeOperationsLanguages>(DataTable.Filters);
            }

            var Result = DbCMS.codeOperationsLanguages.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.OperationGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.OperationLanguageGUID,
                                  x.LanguageID,
                                  x.OperationDescription,
                                  x.codeOperationsLanguagesRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult OperationLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.Operations.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/Operations/_LanguageUpdateModal.cshtml",
                new codeOperationsLanguages { OperationGUID = FK });
        }

        public ActionResult OperationLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.Operations.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/Operations/_LanguageUpdateModal.cshtml", DbCMS.codeOperationsLanguages.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OperationLanguageCreate(codeOperationsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Operations.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveOperationLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/Operations/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Create(model, Permissions.OperationsLanguages.CreateGuid, ExecutionTime);
            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleCreateMessage(DataTableNames.OperationLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OperationLanguageUpdate(codeOperationsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Operations.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/CMS/Views/Codes/Operations/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            //Checking if already exists in same language
            if (ActiveOperationLanguage(model))
            {
                return PartialView("~/Areas/CMS/Views/Codes/Operations/_LanguageUpdateModal.cshtml", model);
            }
            //End Checking

            DbCMS.Update(model, Permissions.OperationsLanguages.UpdateGuid, ExecutionTime);
            try
            {
                DbCMS.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyOperationLanguage(model.OperationLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
            return Json(DbCMS.SingleUpdateMessage(DataTableNames.OperationLanguagesDataTable));

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OperationLanguageDelete(codeOperationsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Operations.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeOperationsLanguages> DeletedLanguages = DeleteOperationLanguages(new List<codeOperationsLanguages> { model });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.OperationLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyOperationLanguage(model.OperationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OperationLanguageRestore(codeOperationsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Operations.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveOperationLanguage(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeOperationsLanguages> RestoredLanguages = RestoreOperationLanguages(Portal.SingleToList(model));

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.OperationLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyOperationLanguage(model.OperationLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult OperationLanguagesDataTableDelete(List<codeOperationsLanguages> models)
        {
            if (!CMS.HasAction(Permissions.Operations.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeOperationsLanguages> DeletedLanguages = DeleteOperationLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.OperationLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult OperationLanguagesDataTableRestore(List<codeOperationsLanguages> models)
        {
            if (!CMS.HasAction(Permissions.Operations.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeOperationsLanguages> RestoredLanguages = RestoreOperationLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.OperationLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeOperationsLanguages> DeleteOperationLanguages(List<codeOperationsLanguages> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeOperationsLanguages> DeletedOperationLanguages = new List<codeOperationsLanguages>();

            string query = DbCMS.QueryBuilder(models, Permissions.OperationsLanguages.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbCMS.Database.SqlQuery<codeOperationsLanguages>(query).ToList();

            foreach (var language in languages)
            {
                DeletedOperationLanguages.Add(DbCMS.Delete(language, ExecutionTime, Permissions.OperationsLanguages.DeleteGuid));
            }

            return DeletedOperationLanguages;
        }

        private List<codeOperationsLanguages> RestoreOperationLanguages(List<codeOperationsLanguages> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeOperationsLanguages> RestoredLanguages = new List<codeOperationsLanguages>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Languages = DbCMS.Database.SqlQuery<codeOperationsLanguages>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveOperationLanguage(language))
                {
                    RestoredLanguages.Add(DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyOperationLanguage(Guid PK)
        {
            codeOperationsLanguages dbModel = new codeOperationsLanguages();

            var Language = DbCMS.codeOperationsLanguages.Where(x => x.OperationLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeOperationsLanguagesRowVersion.SequenceEqual(dbModel.codeOperationsLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveOperationLanguage(codeOperationsLanguages model)
        {
            int LanguageID = DbCMS.codeOperationsLanguages
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.OperationGUID == model.OperationGUID &&
                                              x.OperationLanguageGUID != model.OperationLanguageGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Operation Name in selected language already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }
        #endregion


        #region Job Titles
        [Route("CMS/Codes/JobTitles/")]
        public ActionResult JobTitlesIndex()
        {
            if (!CMS.HasAction(Permissions.JobTitles.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/JobTitles/Index.cshtml");
        }

        [Route("CMS/Codes/JobTitlesDataTable/")]
        public JsonResult JobTitlesDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<JobTitleDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<JobTitleDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbCMS.codeJobTitles.AsExpandable()
                       join b in DbCMS.codeJobTitlesLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeJobTitles.DeletedOn) && x.LanguageID == LAN) on a.JobTitleGUID equals b.JobTitleGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       select new JobTitleDataTableModel
                       {
                           JobTitleGUID = a.JobTitleGUID,
                           Active = a.Active,
                           codeJobTitlesRowVersion = a.codeJobTitlesRowVersion,
                           JobTitleDescription = R1.JobTitleDescription
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<JobTitleDataTableModel> Result = Mapper.Map<List<JobTitleDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("CMS/Codes/JobTitles/Create/")]
        public ActionResult JobTitleCreate()
        {
            if (!CMS.HasAction(Permissions.JobTitles.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/JobTitles/JobTitle.cshtml", new JobTitleUpdateModel());
        }

        [Route("CMS/Codes/JobTitles/Update/{PK}")]
        public ActionResult JobTitleUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.JobTitles.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbCMS.codeJobTitles.WherePK(PK)
                         join b in DbCMS.codeJobTitlesLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeJobTitles.DeletedOn) && x.LanguageID == LAN)
                         on a.JobTitleGUID equals b.JobTitleGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new JobTitleUpdateModel
                         {
                             JobTitleGUID = a.JobTitleGUID,
                             Active = a.Active,
                             codeJobTitlesRowVersion = a.codeJobTitlesRowVersion,
                             JobTitleDescription = R1.JobTitleDescription,
                             codeJobTitleLanguagesRowVersion = R1.codeJobTitlesLanguagesRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("JobTitles", "Codes", new { Area = "CMS" }));

            return View("~/Areas/CMS/Views/Codes/JobTitles/JobTitle.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult JobTitleCreate(JobTitleUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.JobTitles.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveJobTitle(model)) return PartialView("~/Areas/CMS/Views/Codes/JobTitles/_JobTitleForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeJobTitles JobTitle = Mapper.Map(model, new codeJobTitles());
            JobTitle.JobTitleGUID = EntityPK;
            
            DbCMS.Create(JobTitle, Permissions.JobTitles.CreateGuid, ExecutionTime);

            codeJobTitlesLanguages Language = Mapper.Map(model, new codeJobTitlesLanguages());
            Language.JobTitleGUID = EntityPK;
            Language.JobTitleDescription = model.JobTitleDescription;
            
            DbCMS.Create(Language, Permissions.JobTitlesLanguages.CreateGuid, ExecutionTime);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.JobTitleLanguagesDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.JobTitles.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("JobTitle/Create", "Codes", new { Area = "CMS" })), Container = "JobTitleFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.JobTitles.Update, Apps.CMS), Container = "JobTitleFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.JobTitles.Delete, Apps.CMS), Container = "JobTitleFormControls" });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleCreateMessage(DbCMS.PrimaryKeyControl(JobTitle), DbCMS.RowVersionControls(JobTitle, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult JobTitleUpdate(JobTitleUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.JobTitles.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveJobTitle(model)) return PartialView("~/Areas/CMS/Views/Codes/JobTitles/_JobTitleForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeJobTitles JobTitle = Mapper.Map(model, new codeJobTitles());
            DbCMS.Update(JobTitle, Permissions.JobTitles.UpdateGuid, ExecutionTime);

            var Language = DbCMS.codeJobTitlesLanguages.Where(l => l.JobTitleGUID == model.JobTitleGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.JobTitleGUID = JobTitle.JobTitleGUID;
                DbCMS.Create(Language, Permissions.JobTitlesLanguages.CreateGuid, ExecutionTime);
            }
            else if (Language.JobTitleDescription != model.JobTitleDescription)
            {
                Language = Mapper.Map(model, Language);
                DbCMS.Update(Language, Permissions.JobTitlesLanguages.UpdateGuid, ExecutionTime);
            }

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, DbCMS.RowVersionControls(JobTitle, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyJobTitle(model.JobTitleGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult JobTitleDelete(codeJobTitles model)
        {
            if (!CMS.HasAction(Permissions.JobTitles.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeJobTitles> DeletedJobTitles = DeleteJobTitles(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.JobTitles.Restore, Apps.CMS), Container = "JobTitleFormControls" });


            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(CommitedRows, DeletedJobTitles.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyJobTitle(model.JobTitleGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult JobTitleRestore(codeJobTitles model)
        {
            if (!CMS.HasAction(Permissions.JobTitles.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveJobTitle(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeJobTitles> RestoredJobTitles = RestoreJobTitles(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.JobTitles.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("JobTitle/Create", "Codes", new { Area = "CMS" })), Container = "JobTitleFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.JobTitles.Update, Apps.CMS), Container = "JobTitleFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.JobTitles.Delete, Apps.CMS), Container = "JobTitleFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(CommitedRows, RestoredJobTitles, DbCMS.PrimaryKeyControl(RestoredJobTitles.FirstOrDefault()), Url.Action(DataTableNames.JobTitleLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyJobTitle(model.JobTitleGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult JobTitlesDataTableDelete(List<codeJobTitles> models)
        {
            if (!CMS.HasAction(Permissions.JobTitles.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeJobTitles> DeletedJobTitles = DeleteJobTitles(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedJobTitles, models, DataTableNames.JobTitlesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult JobTitlesDataTableRestore(List<codeJobTitles> models)
        {
            if (!CMS.HasAction(Permissions.JobTitles.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeJobTitles> RestoredJobTitles = RestoreJobTitles(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredJobTitles, models, DataTableNames.JobTitlesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeJobTitles> DeleteJobTitles(List<codeJobTitles> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;
            List<codeJobTitles> DeletedJobTitles = new List<codeJobTitles>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Delete, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeJobTitles>(query).ToList();
            foreach (var record in Records)
            {
                DeletedJobTitles.Add(DbCMS.Delete(record, ExecutionTime, DeleteActionGUID));
            }

            var Languages = DeletedJobTitles.SelectMany(a => a.codeJobTitlesLanguages).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Delete(language, ExecutionTime, DeleteActionGUID);
            }
            return DeletedJobTitles;
        }

        private List<codeJobTitles> RestoreJobTitles(List<codeJobTitles> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeJobTitles> RestoredJobTitles = new List<codeJobTitles>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeJobTitles>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveJobTitle(record))
                {
                    RestoredJobTitles.Add(DbCMS.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            var Languages = RestoredJobTitles.SelectMany(x => x.codeJobTitlesLanguages.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime);
            }

            return RestoredJobTitles;
        }

        private JsonResult ConcrrencyJobTitle(Guid PK)
        {
            JobTitleUpdateModel dbModel = new JobTitleUpdateModel();

            var JobTitle = DbCMS.codeJobTitles.Where(x => x.JobTitleGUID == PK).FirstOrDefault();
            var dbJobTitle = DbCMS.Entry(JobTitle).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbJobTitle, dbModel);

            var Language = DbCMS.codeJobTitlesLanguages.Where(x => x.JobTitleGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeJobTitles.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (JobTitle.codeJobTitlesRowVersion.SequenceEqual(dbModel.codeJobTitlesRowVersion) && Language.codeJobTitlesLanguagesRowVersion.SequenceEqual(dbModel.codeJobTitleLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveJobTitle(Object model)
        {
            codeJobTitlesLanguages JobTitle = Mapper.Map(model, new codeJobTitlesLanguages());
            int JobTitleDescription = DbCMS.codeJobTitlesLanguages
                                    .Where(x => x.JobTitleDescription == JobTitle.JobTitleDescription &&
                                                x.JobTitleGUID != JobTitle.JobTitleGUID &&
                                                x.Active).Count();
            if (JobTitleDescription > 0)
            {
                ModelState.AddModelError("JobTitleDescription", "JobTitle is already exists");
            }

            return (JobTitleDescription > 0);
        }

        #endregion

        #region Job Titles Language
        public ActionResult JobTitleLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/CMS/Views/Codes/JobTitles/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeJobTitlesLanguages, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeJobTitlesLanguages>(DataTable.Filters);
            }

            var Result = DbCMS.codeJobTitlesLanguages.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.JobTitleGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.JobTitleLanguageGUID,
                                  x.LanguageID,
                                  x.JobTitleDescription,
                                  x.codeJobTitlesLanguagesRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult JobTitleLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.JobTitles.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/JobTitles/_LanguageUpdateModal.cshtml",
                new codeJobTitlesLanguages { JobTitleGUID = FK });
        }

        public ActionResult JobTitleLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.JobTitles.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/JobTitles/_LanguageUpdateModal.cshtml", DbCMS.codeJobTitlesLanguages.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult JobTitleLanguageCreate(codeJobTitlesLanguages model)
        {
            if (!CMS.HasAction(Permissions.JobTitles.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveJobTitleLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/JobTitles/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Create(model, Permissions.JobTitlesLanguages.CreateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.JobTitleLanguagesDataTable, DbCMS.PrimaryKeyControl(model), DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult JobTitleLanguageUpdate(codeJobTitlesLanguages model)
        {
            if (!CMS.HasAction(Permissions.JobTitles.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveJobTitleLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/JobTitles/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Update(model, Permissions.JobTitlesLanguages.UpdateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.JobTitleLanguagesDataTable,
                    DbCMS.PrimaryKeyControl(model),
                    DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyJobTitleLanguage(model.JobTitleLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult JobTitleLanguageDelete(codeJobTitlesLanguages model)
        {
            if (!CMS.HasAction(Permissions.JobTitles.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeJobTitlesLanguages> DeletedLanguages = DeleteJobTitleLanguages(new List<codeJobTitlesLanguages> { model });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.JobTitleLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyJobTitleLanguage(model.JobTitleGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult JobTitleLanguageRestore(codeJobTitlesLanguages model)
        {
            if (!CMS.HasAction(Permissions.JobTitles.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveJobTitleLanguage(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeJobTitlesLanguages> RestoredLanguages = RestoreJobTitleLanguages(Portal.SingleToList(model));

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.JobTitleLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyJobTitleLanguage(model.JobTitleLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult JobTitleLanguagesDataTableDelete(List<codeJobTitlesLanguages> models)
        {
            if (!CMS.HasAction(Permissions.JobTitles.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeJobTitlesLanguages> DeletedLanguages = DeleteJobTitleLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.JobTitleLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult JobTitleLanguagesDataTableRestore(List<codeJobTitlesLanguages> models)
        {
            if (!CMS.HasAction(Permissions.JobTitles.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeJobTitlesLanguages> RestoredLanguages = RestoreJobTitleLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.JobTitleLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeJobTitlesLanguages> DeleteJobTitleLanguages(List<codeJobTitlesLanguages> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeJobTitlesLanguages> DeletedJobTitleLanguages = new List<codeJobTitlesLanguages>();

            string query = DbCMS.QueryBuilder(models, Permissions.JobTitlesLanguages.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbCMS.Database.SqlQuery<codeJobTitlesLanguages>(query).ToList();

            foreach (var language in languages)
            {
                DeletedJobTitleLanguages.Add(DbCMS.Delete(language, ExecutionTime, Permissions.JobTitlesLanguages.DeleteGuid));
            }

            return DeletedJobTitleLanguages;
        }

        private List<codeJobTitlesLanguages> RestoreJobTitleLanguages(List<codeJobTitlesLanguages> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeJobTitlesLanguages> RestoredLanguages = new List<codeJobTitlesLanguages>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Languages = DbCMS.Database.SqlQuery<codeJobTitlesLanguages>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveJobTitleLanguage(language))
                {
                    RestoredLanguages.Add(DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyJobTitleLanguage(Guid PK)
        {
            codeJobTitlesLanguages dbModel = new codeJobTitlesLanguages();

            var Language = DbCMS.codeJobTitlesLanguages.Where(l => l.JobTitleLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeJobTitlesLanguagesRowVersion.SequenceEqual(dbModel.codeJobTitlesLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }
        private bool ActiveJobTitleLanguage(codeJobTitlesLanguages model)
        {
            int LanguageID = DbCMS.codeJobTitlesLanguages
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.JobTitleGUID == model.JobTitleGUID &&
                                              x.JobTitleLanguageGUID != model.JobTitleLanguageGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Job Title Name in selected language already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }
        #endregion


        #region Offices
        [Route("CMS/Codes/Offices/")]
        public ActionResult OfficesIndex()
        {
            //No Action checking needed as this on time being is public for REGISTERED USER As Amer mensioned
            //if (!CMS.HasAction(Permissions.Offices.Access, Apps.CMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            return View("~/Areas/CMS/Views/Codes/Offices/Index.cshtml");
        }

        [Route("CMS/Codes/OfficesDataTable/")]
        public JsonResult OfficesDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<OfficeDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<OfficeDataTableModel>(DataTable.Filters);
            }

            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.Offices.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();


            var All = (from a in DbCMS.codeOffices.AsExpandable().Where(x => AuthorizedList.Contains(x.OrganizationGUID.ToString()))
                       join b in DbCMS.codeOfficesLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeOffices.DeletedOn) && x.LanguageID == LAN) on a.OfficeGUID equals b.OfficeGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join c in DbCMS.codeCountriesLanguages.Where(x => x.LanguageID == LAN) on a.CountryGUID equals c.CountryGUID
                       join e in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN) on a.OfficeTypeGUID equals e.ValueGUID

                       select new OfficeDataTableModel
                       {
                           OfficeGUID = a.OfficeGUID,
                           codeOfficesRowVersion = a.codeOfficesRowVersion,
                           OfficeDescription = R1.OfficeDescription,
                           OfficeAddress = R1.OfficeAddress,
                           OfficePhoneNumber = a.OfficePhoneCountryCode + " " + a.OfficePhoneNumber,
                           CountryGUID = c.CountryDescription,
                           OfficeTypeGUID = e.ValueDescription,
                           Active = a.Active,
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<OfficeDataTableModel> Result = Mapper.Map<List<OfficeDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        private void SetddlCountryPhoneCodes()
        {
            //Needs Review why select from function. Ayas
            ViewBag.JsonCountries = JsonConvert.SerializeObject(DropDownList.CountriesPhoneCode());
            ViewBag.ddlCountriesPhoneCode = DropDownList.CountriesPhoneCode().Select(ddl => new { Value = ddl.id, Text = ddl.text }).ToList();
        }
        [Route("CMS/Codes/Offices/Create/")]
        public ActionResult OfficeCreate()
        {
            if (!CMS.HasAction(Permissions.Offices.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            SetddlCountryPhoneCodes();
            return View("~/Areas/CMS/Views/Codes/Offices/Office.cshtml", new OfficeUpdateModel());
        }

        [Route("CMS/Codes/Offices/Update/{PK}")]
        public ActionResult OfficeUpdate(Guid PK)
        {

            //No Action checking needed as this on time being is public for REGISTERED USER As Amer mensioned
            //if (!CMS.HasAction(Permissions.Offices.Access, Apps.CMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            SetddlCountryPhoneCodes();
            var model = (from a in DbCMS.codeOffices.WherePK(PK)
                         join b in DbCMS.codeOfficesLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeOffices.DeletedOn) && x.LanguageID == LAN)
                         on a.OfficeGUID equals b.OfficeGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new OfficeUpdateModel
                         {
                             OfficeGUID = a.OfficeGUID,
                             Active = a.Active,
                             codeOfficesRowVersion = a.codeOfficesRowVersion,
                             codeOfficesLanguagesRowVersion = R1.codeOfficesLanguagesRowVersion,
                             OfficeDescription = R1.OfficeDescription,
                             CountryGUID = a.CountryGUID,
                             DutyStationGUID = a.DutyStationGUID.HasValue ? a.DutyStationGUID.Value : Guid.Empty,
                             OfficeAddress = R1.OfficeAddress,
                             Latitude = a.Latitude,
                             Longitude = a.Longitude,
                             OfficePhoneCountryCode = a.OfficePhoneCountryCode,
                             OfficePhoneNumber = a.OfficePhoneNumber,
                             VTCIP = a.VTCIP,
                             OfficeTypeGUID = a.OfficeTypeGUID,
                             OrganizationGUID = a.OrganizationGUID
                         }).FirstOrDefault();


            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("Offices", "Codes", new { Area = "CMS" }));

            model.MediaPath = new Portal().OfficePhoto(model.OfficeGUID);

            return View("~/Areas/CMS/Views/Codes/Offices/Office.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OfficeCreate(OfficeUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Offices.Create, Apps.CMS, model.OrganizationGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveOffice(model)) { SetddlCountryPhoneCodes(); return PartialView("~/Areas/CMS/Views/Codes/Offices/_OfficeForm.cshtml", model); }

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeOffices Office = Mapper.Map(model, new codeOffices());
            Office.OfficeGUID = EntityPK;
            DbCMS.Create(Office, Permissions.Offices.CreateGuid, ExecutionTime);

            codeOfficesLanguages Language = Mapper.Map(model, new codeOfficesLanguages());
            Language.OfficeGUID = EntityPK;
            DbCMS.Create(Language, Permissions.OfficesLanguages.CreateGuid, ExecutionTime);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.OfficeLanguagesDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Offices.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("Offices/Create", "Codes", new { Area = "CMS" })), Container = "OfficeFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Offices.Update, Apps.CMS), Container = "OfficeFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Offices.Delete, Apps.CMS), Container = "OfficeFormControls" });

            try
            {
                DbCMS.SaveChanges();
                new Portal().CutFile(model.MediaName, "Offices\\" + Office.OfficeGUID + ".jpg");
                return Json(DbCMS.SingleCreateMessage(DbCMS.PrimaryKeyControl(Office), DbCMS.RowVersionControls(Office, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OfficeUpdate(OfficeUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Offices.Update, Apps.CMS, FactorsCollector.Office(model.OfficeGUID)))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid || ActiveOffice(model)) { SetddlCountryPhoneCodes(); return PartialView("~/Areas/CMS/Views/Codes/Offices/_OfficeForm.cshtml", model); }

            DateTime ExecutionTime = DateTime.Now;

            codeOffices Office = Mapper.Map(model, new codeOffices());
            DbCMS.Update(Office, Permissions.Offices.UpdateGuid, ExecutionTime);

            var Language = DbCMS.codeOfficesLanguages.Where(x => x.OfficeGUID == model.OfficeGUID && x.LanguageID == LAN && x.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.OfficeGUID = Office.OfficeGUID;
                DbCMS.Create(Language, Permissions.OfficesLanguages.CreateGuid, ExecutionTime);
            }
            else if (Language.OfficeDescription != model.OfficeDescription || Language.OfficeAddress != model.OfficeAddress)
            {
                Language = Mapper.Map(model, Language);
                DbCMS.Update(Language, Permissions.OfficesLanguages.UpdateGuid, ExecutionTime);
            }

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, DbCMS.RowVersionControls(Office, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyOffice(model.OfficeGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OfficeDelete(codeOffices model)
        {
            if (!CMS.HasAction(Permissions.Offices.Delete, Apps.CMS, model.OrganizationGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeOffices> DeletedOffices = DeleteOffices(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.Offices.Restore, Apps.CMS), Container = "OfficeFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(CommitedRows, DeletedOffices.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyOffice(model.OfficeGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OfficeRestore(codeOffices model)
        {
            if (!CMS.HasAction(Permissions.Offices.Restore, Apps.CMS, model.OrganizationGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveOffice(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeOffices> RestoredOffices = RestoreOffices(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Offices.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("Offices/Create", "Codes", new { Area = "CMS" })), Container = "OfficeFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Offices.Update, Apps.CMS, FactorsCollector.Office(model.OfficeGUID)), Container = "OfficeFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Offices.Delete, Apps.CMS, FactorsCollector.Office(model.OfficeGUID)), Container = "OfficeFormControls" });


            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(CommitedRows, RestoredOffices, DbCMS.PrimaryKeyControl(RestoredOffices.FirstOrDefault()), Url.Action(DataTableNames.OfficeLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyOffice(model.OfficeGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult OfficesDataTableDelete(List<codeOffices> models)
        {
            if (!CMS.HasAction(Permissions.Offices.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeOffices> DeletedOffices = DeleteOffices(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedOffices, models, DataTableNames.OfficesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult OfficesDataTableRestore(List<codeOffices> models)
        {
            if (!CMS.HasAction(Permissions.Offices.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeOffices> RestoredOffices = RestoreOffices(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredOffices, models, DataTableNames.OfficesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        public ActionResult OfficeOfficePhotoDelete(codeOffices model)
        {
            //This function will delete the photo only.
            System.IO.File.Delete(WebConfigurationManager.AppSettings["DataFolder"] + "\\Offices\\" + model.OfficeGUID.ToString() + ".jpg");
            return Json(DbCMS.SingleUpdateMessage(null, null, null, "RemoveOfficePhoto('" + Constants.NoPhotoTemplate + "')", "Photo deleted successfully"));
        }

        private List<codeOffices> DeleteOffices(List<codeOffices> models)
        {
            Guid DeleteActionGUID = Permissions.Offices.DeleteGuid;
            DateTime ExecutionTime = DateTime.Now;
            List<codeOffices> DeletedOffices = new List<codeOffices>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "SELECT OfficeGUID,CONVERT(varchar(50), OrganizationGUID) as C2 ,codeOfficesRowVersion FROM codeOffices where OfficeGUID in (" + string.Join(",", models.Select(x => "'" + x.OfficeGUID + "'").ToArray()) + ")";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Delete, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeOffices>(query).ToList();
            foreach (var record in Records)
            {
                DeletedOffices.Add(DbCMS.Delete(record, ExecutionTime, DeleteActionGUID));
            }

            var Languages = DeletedOffices.SelectMany(a => a.codeOfficesLanguages).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Delete(language, ExecutionTime, DeleteActionGUID);
            }
            return DeletedOffices;
        }

        private List<codeOffices> RestoreOffices(List<codeOffices> models)
        {

            Guid RestoreActionGUID = Permissions.Offices.RestoreGuid;
            DateTime RestoringTime = DateTime.Now;

            List<codeOffices> RestoredOffices = new List<codeOffices>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "SELECT OfficeGUID,CONVERT(varchar(50), OrganizationGUID) as C2 ,codeOfficesRowVersion FROM codeOffices where OfficeGUID in (" + string.Join(",", models.Select(x => "'" + x.OfficeGUID + "'").ToArray()) + ")";

            string query = DbCMS.QueryBuilder(models, RestoreActionGUID, SubmitTypes.Restore, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeOffices>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveOffice(record))
                {
                    RestoredOffices.Add(DbCMS.Restore(record, RestoreActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            var Languages = RestoredOffices.SelectMany(l => l.codeOfficesLanguages.Where(x => x.DeletedOn == l.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Restore(language, RestoreActionGUID, RestoreActionGUID, RestoringTime);
            }

            return RestoredOffices;
        }

        private JsonResult ConcrrencyOffice(Guid PK)
        {
            OfficeUpdateModel dbModel = new OfficeUpdateModel();

            var Office = DbCMS.codeOffices.Where(a => a.OfficeGUID == PK).FirstOrDefault();
            var dbOffice = DbCMS.Entry(Office).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbOffice, dbModel);

            var Language = DbCMS.codeOfficesLanguages.Where(x => x.OfficeGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeOffices.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Office.codeOfficesRowVersion.SequenceEqual(dbModel.codeOfficesRowVersion) && Language.codeOfficesLanguagesRowVersion.SequenceEqual(dbModel.codeOfficesLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveOffice(Object model)
        {
            codeOfficesLanguages Office = Mapper.Map(model, new codeOfficesLanguages());
            int OfficeDescription = DbCMS.codeOfficesLanguages
                                         .Where(x => x.OfficeDescription == Office.OfficeDescription &&
                                                     x.OfficeGUID != Office.OfficeGUID &&
                                                     x.Active).Count();
            if (OfficeDescription > 0)
            {
                ModelState.AddModelError("OfficeDescription", "Office is already exists");
            }

            return (OfficeDescription > 0);
        }

        #endregion

        #region Office Languages
        public ActionResult OfficeLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/CMS/Views/Codes/Offices/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeOfficesLanguages, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeOfficesLanguages>(DataTable.Filters);
            }

            var Result = DbCMS.codeOfficesLanguages.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.OfficeGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.OfficeLanguageGUID,
                                  x.codeOfficesLanguagesRowVersion,
                                  x.LanguageID,
                                  x.OfficeDescription,
                                  x.OfficeAddress,
                                  x.OfficeGUID
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult OfficeLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.Offices.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/Offices/_LanguageUpdateModal.cshtml",
                new codeOfficesLanguages { OfficeGUID = FK });
        }

        public ActionResult OfficeLanguageUpdate(Guid PK)
        {

            if (!CMS.HasAction(Permissions.Offices.Access, Apps.CMS, FactorsCollector.Office(DbCMS.codeOfficesLanguages.Find(PK).OfficeGUID))) //we know its two hits
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return PartialView("~/Areas/CMS/Views/Codes/Offices/_LanguageUpdateModal.cshtml", DbCMS.codeOfficesLanguages.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OfficeLanguageCreate(codeOfficesLanguages model)
        {

            if (!CMS.HasAction(Permissions.Offices.Create, Apps.CMS, FactorsCollector.Office(model.OfficeGUID)))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveOfficeLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/Offices/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Create(model, Permissions.OfficesLanguages.CreateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.OfficeLanguagesDataTable, DbCMS.PrimaryKeyControl(model), DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OfficeLanguageUpdate(codeOfficesLanguages model)
        {
            if (!CMS.HasAction(Permissions.Offices.Update, Apps.CMS, FactorsCollector.Office(model.OfficeGUID)))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveOfficeLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/Offices/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Update(model, Permissions.OfficesLanguages.UpdateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.OfficeLanguagesDataTable,
                    DbCMS.PrimaryKeyControl(model),
                    DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyOfficeLanguage(model.OfficeLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OfficeLanguageDelete(codeOfficesLanguages model)
        {
            if (!CMS.HasAction(Permissions.Offices.Delete, Apps.CMS, FactorsCollector.Office(model.OfficeGUID)))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeOfficesLanguages> DeletedLanguages = DeleteOfficeLanguages(new List<codeOfficesLanguages> { model });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.OfficeLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyOfficeLanguage(model.OfficeGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OfficeLanguageRestore(codeOfficesLanguages model)
        {
            if (!CMS.HasAction(Permissions.Offices.Restore, Apps.CMS, FactorsCollector.Office(model.OfficeGUID)))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveOfficeLanguage(model))
            {
                return Json(DbCMS.RecordExists());
            }
            codeOfficesLanguages codeOfficesLanguages = new codeOfficesLanguages();

            List<codeOfficesLanguages> RestoredLanguages = RestoreOfficeLanguages(Portal.SingleToList(model));

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.OfficeLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyOfficeLanguage(model.OfficeLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult OfficeLanguagesDataTableDelete(List<codeOfficesLanguages> models)
        {
            if (!CMS.HasAction(Permissions.Offices.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeOfficesLanguages> DeletedLanguages = DeleteOfficeLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.OfficeLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult OfficeLanguagesDataTableRestore(List<codeOfficesLanguages> models)
        {
            if (!CMS.HasAction(Permissions.Offices.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeOfficesLanguages> RestoredLanguages = RestoreOfficeLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.OfficeLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeOfficesLanguages> DeleteOfficeLanguages(List<codeOfficesLanguages> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeOfficesLanguages> DeletedOfficeLanguages = new List<codeOfficesLanguages>();

            string query = DbCMS.QueryBuilder(models, Permissions.OfficesLanguages.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbCMS.Database.SqlQuery<codeOfficesLanguages>(query).ToList();

            foreach (var language in languages)
            {
                DeletedOfficeLanguages.Add(DbCMS.Delete(language, ExecutionTime, Permissions.OfficesLanguages.DeleteGuid));
            }

            return DeletedOfficeLanguages;
        }

        private List<codeOfficesLanguages> RestoreOfficeLanguages(List<codeOfficesLanguages> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeOfficesLanguages> RestoredLanguages = new List<codeOfficesLanguages>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Languages = DbCMS.Database.SqlQuery<codeOfficesLanguages>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveOfficeLanguage(language))
                {
                    RestoredLanguages.Add(DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyOfficeLanguage(Guid PK)
        {
            codeOfficesLanguages dbModel = new codeOfficesLanguages();

            var Language = DbCMS.codeOfficesLanguages.Where(l => l.OfficeLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeOfficesLanguagesRowVersion.SequenceEqual(dbModel.codeOfficesLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveOfficeLanguage(codeOfficesLanguages model)
        {
            int LanguageID = DbCMS.codeOfficesLanguages
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.OfficeGUID == model.OfficeGUID &&
                                              x.OfficeLanguageGUID != model.OfficeLanguageGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Office Name in selected language already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }
        #endregion


        #region Factors
        [Route("CMS/Codes/Factors/")]
        public ActionResult FactorsIndex()
        {
            if (!CMS.HasAction(Permissions.Factors.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/Factors/Index.cshtml");
        }

        [Route("CMS/Codes/FactorsDataTable/")]
        public JsonResult FactorsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<FactorDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<FactorDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbCMS.codeFactors.AsExpandable()
                       join b in DbCMS.codeFactorsLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeFactors.DeletedOn) && x.LanguageID == LAN) on a.FactorGUID equals b.FactorGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       select new FactorDataTableModel
                       {
                           FactorGUID = a.FactorGUID,
                           Active = a.Active,
                           SortID = a.SortID,
                           codeFactorsRowVersion = a.codeFactorsRowVersion,
                           FactorDescription = R1.FactorDescription,
                           FactorColumnName = a.FactorColumnName
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<FactorDataTableModel> Result = Mapper.Map<List<FactorDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("CMS/Codes/Factors/Create/")]
        public ActionResult FactorCreate()
        {
            if (!CMS.HasAction(Permissions.Factors.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/Factors/Factor.cshtml", new FactorUpdateModel());
        }

        [Route("CMS/Codes/Factors/Update/{PK}")]
        public ActionResult FactorUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.Factors.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbCMS.codeFactors.WherePK(PK)
                         join b in DbCMS.codeFactorsLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeFactors.DeletedOn) && x.LanguageID == LAN)
                         on a.FactorGUID equals b.FactorGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new FactorUpdateModel
                         {
                             FactorGUID = a.FactorGUID,
                             Active = a.Active,
                             SortID = a.SortID,
                             codeFactorsRowVersion = a.codeFactorsRowVersion,
                             FactorDescription = R1.FactorDescription,
                             FactorColumnName = a.FactorColumnName,
                             codeFactorsLanguagesRowVersion = R1.codeFactorsLanguagesRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("Factors", "Codes", new { Area = "CMS" }));

            return View("~/Areas/CMS/Views/Codes/Factors/Factor.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FactorCreate(FactorUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Factors.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveFactor(model)) return PartialView("~/Areas/CMS/Views/Codes/Factors/_FactorForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeFactors Factor = Mapper.Map(model, new codeFactors());
            Factor.FactorGUID = EntityPK;
            DbCMS.Create(Factor, Permissions.Factors.CreateGuid, ExecutionTime);

            codeFactorsLanguages Language = Mapper.Map(model, new codeFactorsLanguages());
            Language.FactorGUID = EntityPK;//BA3SA
            DbCMS.Create(Language, Permissions.FactorsLanguages.CreateGuid, ExecutionTime);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.FactorLanguagesDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Factors.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("Factor", "Codes", new { Area = "CMS" })), Container = "FactorFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Factors.Update, Apps.CMS), Container = "FactorFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Factors.Delete, Apps.CMS), Container = "FactorFormControls" });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleCreateMessage(DbCMS.PrimaryKeyControl(Factor), DbCMS.RowVersionControls(Factor, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FactorUpdate(FactorUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Factors.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveFactor(model)) return PartialView("~/Areas/CMS/Views/Codes/Factors/_FactorForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeFactors Factor = Mapper.Map(model, new codeFactors());
            DbCMS.Update(Factor, Permissions.Factors.UpdateGuid, ExecutionTime);

            var Language = DbCMS.codeFactorsLanguages.Where(l => l.FactorGUID == model.FactorGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.FactorGUID = Factor.FactorGUID;
                DbCMS.Create(Language, Permissions.FactorsLanguages.CreateGuid, ExecutionTime);
            }
            else if (Language.FactorDescription != model.FactorDescription)
            {
                Language = Mapper.Map(model, Language);
                DbCMS.Update(Language, Permissions.FactorsLanguages.UpdateGuid, ExecutionTime);
            }

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, DbCMS.RowVersionControls(Factor, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyFactor(model.FactorGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FactorDelete(codeFactors model)
        {
            if (!CMS.HasAction(Permissions.Factors.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeFactors> DeletedFactors = DeleteFactors(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.Factors.Restore, Apps.CMS), Container = "FactorFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(CommitedRows, DeletedFactors.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyFactor(model.FactorGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FactorRestore(codeFactors model)
        {
            if (!CMS.HasAction(Permissions.Factors.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveFactor(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeFactors> RestoredFactors = RestoreFactors(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Factors.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("Factors/Create", "Codes", new { Area = "CMS" })), Container = "FactorFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Factors.Update, Apps.CMS), Container = "FactorFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Factors.Delete, Apps.CMS), Container = "FactorFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(CommitedRows, RestoredFactors, DbCMS.PrimaryKeyControl(RestoredFactors.FirstOrDefault()), Url.Action(DataTableNames.FactorLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyFactor(model.FactorGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult FactorsDataTableDelete(List<codeFactors> models)
        {
            if (!CMS.HasAction(Permissions.Factors.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeFactors> DeletedFactors = DeleteFactors(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedFactors, models, DataTableNames.FactorsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult FactorsDataTableRestore(List<codeFactors> models)
        {
            if (!CMS.HasAction(Permissions.Factors.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeFactors> RestoredFactors = RestoreFactors(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredFactors, models, DataTableNames.FactorsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeFactors> DeleteFactors(List<codeFactors> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;
            List<codeFactors> DeletedFactors = new List<codeFactors>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Delete, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeFactors>(query).ToList();
            foreach (var record in Records)
            {
                DeletedFactors.Add(DbCMS.Delete(record, ExecutionTime, DeleteActionGUID));
            }

            var Languages = DeletedFactors.SelectMany(x => x.codeFactorsLanguages).Where(x => x.Active).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Delete(language, ExecutionTime, DeleteActionGUID);
            }
            return DeletedFactors;
        }

        private List<codeFactors> RestoreFactors(List<codeFactors> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeFactors> RestoredFactors = new List<codeFactors>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeFactors>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveFactor(record))
                {
                    RestoredFactors.Add(DbCMS.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            var Languages = RestoredFactors.SelectMany(x => x.codeFactorsLanguages.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime);
            }

            return RestoredFactors;
        }

        private JsonResult ConcrrencyFactor(Guid PK)
        {
            FactorUpdateModel dbModel = new FactorUpdateModel();

            var Factor = DbCMS.codeFactors.Where(a => a.FactorGUID == PK).FirstOrDefault();
            var dbFactor = DbCMS.Entry(Factor).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbFactor, dbModel);

            var Language = DbCMS.codeFactorsLanguages.Where(x => x.FactorGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeFactors.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Factor.codeFactorsRowVersion.SequenceEqual(dbModel.codeFactorsRowVersion) && Language.codeFactorsLanguagesRowVersion.SequenceEqual(dbModel.codeFactorsLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveFactor(Object model)
        {
            codeFactorsLanguages Factor = Mapper.Map(model, new codeFactorsLanguages());
            int FactorDescription = DbCMS.codeFactorsLanguages
                                         .Where(x => x.FactorDescription == Factor.FactorDescription &&
                                                     x.FactorGUID != Factor.FactorGUID &&
                                                     x.Active).Count();
            if (FactorDescription > 0)
            {
                ModelState.AddModelError("FactorDescription", "Factor is already exists");
            }

            return (FactorDescription > 0);
        }

        #endregion

        #region Factors Language
        public ActionResult FactorLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/CMS/Views/Codes/Factors/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeFactorsLanguages, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeFactorsLanguages>(DataTable.Filters);
            }

            var Result = DbCMS.codeFactorsLanguages.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.FactorGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.FactorLanguageGUID,
                                  x.LanguageID,
                                  x.FactorDescription,
                                  x.codeFactorsLanguagesRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult FactorLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.Factors.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/Factors/_LanguageUpdateModal.cshtml",
                new codeFactorsLanguages { FactorGUID = FK });
        }

        public ActionResult FactorLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.Factors.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/Factors/_LanguageUpdateModal.cshtml", DbCMS.codeFactorsLanguages.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FactorLanguageCreate(codeFactorsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Factors.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveFactorLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/Factors/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Create(model, Permissions.FactorsLanguages.CreateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.FactorLanguagesDataTable, DbCMS.PrimaryKeyControl(model), DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FactorLanguageUpdate(codeFactorsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Factors.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveFactorLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/Factors/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Update(model, Permissions.FactorsLanguages.UpdateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.FactorLanguagesDataTable,
                    DbCMS.PrimaryKeyControl(model),
                    DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyFactorLanguage(model.FactorLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }



        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FactorLanguageDelete(codeFactorsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Factors.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeFactorsLanguages> DeletedLanguages = DeleteFactorLanguages(new List<codeFactorsLanguages> { model });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.FactorLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyFactorLanguage(model.FactorGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FactorLanguageRestore(codeFactorsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Factors.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveFactorLanguage(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeFactorsLanguages> RestoredLanguages = RestoreFactorLanguages(Portal.SingleToList(model));

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.FactorLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyFactorLanguage(model.FactorLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult FactorLanguagesDataTableDelete(List<codeFactorsLanguages> models)
        {
            if (!CMS.HasAction(Permissions.Factors.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeFactorsLanguages> DeletedLanguages = DeleteFactorLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.FactorLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult FactorLanguagesDataTableRestore(List<codeFactorsLanguages> models)
        {
            if (!CMS.HasAction(Permissions.Factors.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeFactorsLanguages> RestoredLanguages = RestoreFactorLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.FactorLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeFactorsLanguages> DeleteFactorLanguages(List<codeFactorsLanguages> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeFactorsLanguages> DeletedFactorLanguages = new List<codeFactorsLanguages>();

            string query = DbCMS.QueryBuilder(models, Permissions.FactorsLanguages.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbCMS.Database.SqlQuery<codeFactorsLanguages>(query).ToList();

            foreach (var language in languages)
            {
                DeletedFactorLanguages.Add(DbCMS.Delete(language, ExecutionTime, Permissions.FactorsLanguages.DeleteGuid));
            }

            return DeletedFactorLanguages;
        }

        private List<codeFactorsLanguages> RestoreFactorLanguages(List<codeFactorsLanguages> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeFactorsLanguages> RestoredLanguages = new List<codeFactorsLanguages>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Languages = DbCMS.Database.SqlQuery<codeFactorsLanguages>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveFactorLanguage(language))
                {
                    RestoredLanguages.Add(DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyFactorLanguage(Guid PK)
        {
            codeFactorsLanguages dbModel = new codeFactorsLanguages();

            var Language = DbCMS.codeFactorsLanguages.Where(l => l.FactorLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeFactorsLanguagesRowVersion.SequenceEqual(dbModel.codeFactorsLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveFactorLanguage(codeFactorsLanguages model)
        {
            int LanguageID = DbCMS.codeFactorsLanguages
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.FactorGUID == model.FactorGUID &&
                                              x.FactorLanguageGUID != model.FactorLanguageGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Factor Name in selected language already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }
        #endregion


        #region Action Categories
        [Route("CMS/Codes/ActionCategories/")]
        public ActionResult ActionCategoriesIndex()
        {
            if (!CMS.HasAction(Permissions.ActionCategories.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/ActionCategories/Index.cshtml");
        }

        [Route("CMS/Codes/ActionCategoriesDataTable/")]
        public JsonResult ActionCategoriesDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ActionCategoriesDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ActionCategoriesDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbCMS.codeActionsCategories.AsExpandable()
                       join b in DbCMS.codeActionsCategoriesLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeActionsCategories.DeletedOn) && x.LanguageID == LAN) on a.ActionCategoryGUID equals b.ActionCategoryGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join c in DbCMS.codeApplicationsLanguages.Where(x => x.LanguageID == LAN) on a.ApplicationGUID equals c.ApplicationGUID

                       select new ActionCategoriesDataTableModel
                       {
                           ActionCategoryGUID = a.ActionCategoryGUID,
                           ApplicationDescription = c.ApplicationDescription,
                           Active = a.Active,
                           SortID = a.SortID,
                           codeActionsCategoriesRowVersion = a.codeActionsCategoriesRowVersion,
                           ActionCategoryDescription = R1.ActionCategoryDescription,
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ActionCategoriesDataTableModel> Result = Mapper.Map<List<ActionCategoriesDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("CMS/Codes/ActionCategories/Create/")]
        public ActionResult ActionCategoryCreate()
        {
            if (!CMS.HasAction(Permissions.ActionCategories.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/ActionCategories/ActionCategory.cshtml", new ActionCategoryUpdateModel());
        }

        [Route("CMS/Codes/ActionCategories/Update/{PK}")]
        public ActionResult ActionCategoryUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ActionCategories.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbCMS.codeActionsCategories.WherePK(PK)
                         join b in DbCMS.codeActionsCategoriesLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeActionsCategories.DeletedOn) && x.LanguageID == LAN)
                         on a.ActionCategoryGUID equals b.ActionCategoryGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new ActionCategoryUpdateModel
                         {
                             ActionCategoryGUID = a.ActionCategoryGUID,
                             Active = a.Active,
                             SortID = a.SortID,
                             ApplicationGUID = a.ApplicationGUID,
                             codeActionsCategoriesRowVersion = a.codeActionsCategoriesRowVersion,
                             ActionCategoryDescription = R1.ActionCategoryDescription,
                             codeActionsCategoriesLanguagesRowVersion = R1.codeActionsCategoriesLanguagesRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("ActionCategories", "Codes", new { Area = "CMS" }));

            return View("~/Areas/CMS/Views/Codes/ActionCategories/ActionCategory.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionCategoryCreate(ActionCategoryUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.ActionCategories.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveActionCategories(model)) return PartialView("~/Areas/CMS/Views/Codes/ActionCategories/_ActionCategoriesForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeActionsCategories ActionCategories = Mapper.Map(model, new codeActionsCategories());
            ActionCategories.ActionCategoryGUID = EntityPK;
            DbCMS.Create(ActionCategories, Permissions.ActionCategories.CreateGuid, ExecutionTime);

            codeActionsCategoriesLanguages Language = Mapper.Map(model, new codeActionsCategoriesLanguages());
            Language.ActionCategoryGUID = ActionCategories.ActionCategoryGUID;//BA3SA
            DbCMS.Create(Language, Permissions.ActionCategoriesLanguages.CreateGuid, ExecutionTime);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.FactorsDependencyDataTable, ControllerContext, "FactorsContainer"));
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ActionCategoryLanguagesDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.ActionCategories.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("ActionCategory/Create", "Codes", new { Area = "CMS" })), Container = "ActionCategoryFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.ActionCategories.Update, Apps.CMS), Container = "ActionCategoryFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.ActionCategories.Delete, Apps.CMS), Container = "ActionCategoryFormControls" });


            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleCreateMessage(DbCMS.PrimaryKeyControl(ActionCategories), DbCMS.RowVersionControls(ActionCategories, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionCategoryUpdate(ActionCategoryUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.ActionCategories.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveActionCategories(model)) return PartialView("~/Areas/CMS/Views/Codes/ActionCategories/_ActionCategoriesForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeActionsCategories ActionCategories = Mapper.Map(model, new codeActionsCategories());
            DbCMS.Update(ActionCategories, Permissions.ActionCategories.UpdateGuid, ExecutionTime);

            var Language = DbCMS.codeActionsCategoriesLanguages.Where(l => l.ActionCategoryGUID == model.ActionCategoryGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.ActionCategoryGUID = ActionCategories.ActionCategoryGUID;
                DbCMS.Create(Language, Permissions.ActionCategoriesLanguages.CreateGuid, ExecutionTime);
            }
            else if (Language.ActionCategoryDescription != model.ActionCategoryDescription)
            {
                Language = Mapper.Map(model, Language);
                DbCMS.Update(Language, Permissions.ActionCategoriesLanguages.UpdateGuid, ExecutionTime);
            }

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, DbCMS.RowVersionControls(ActionCategories, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyActionCategories(model.ActionCategoryGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionCategoryDelete(codeActionsCategories model)
        {
            if (!CMS.HasAction(Permissions.ActionCategories.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeActionsCategories> DeletedActionCategories = DeleteActionCategories(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.ActionCategories.Restore, Apps.CMS), Container = "ActionCategoryFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(CommitedRows, DeletedActionCategories.FirstOrDefault(), "LanguagesContainer,FactorsContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyActionCategories(model.ActionCategoryGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionCategoryRestore(codeActionsCategories model)
        {
            if (!CMS.HasAction(Permissions.ActionCategories.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveActionCategories(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeActionsCategories> RestoredActionCategories = RestoreActionCategories(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.ActionCategories.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("ActionCategory/Create", "Codes", new { Area = "CMS" })), Container = "ActionCategoryFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.ActionCategories.Update, Apps.CMS), Container = "ActionCategoryFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.ActionCategories.Delete, Apps.CMS), Container = "ActionCategoryFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(CommitedRows, RestoredActionCategories, DbCMS.PrimaryKeyControl(RestoredActionCategories.FirstOrDefault()), Url.Action(DataTableNames.ActionCategoryLanguagesDataTable, Portal.GetControllerName(ControllerContext)) + "," + Url.Action(DataTableNames.FactorsDependencyDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer,FactorsContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyActionCategories(model.ActionCategoryGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ActionCategoriesDataTableDelete(List<codeActionsCategories> models)
        {
            if (!CMS.HasAction(Permissions.ActionCategories.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeActionsCategories> DeletedActionCategories = DeleteActionCategories(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedActionCategories, models, DataTableNames.ActionCategoriesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ActionCategoriesDataTableRestore(List<codeActionsCategories> models)
        {
            if (!CMS.HasAction(Permissions.ActionCategories.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeActionsCategories> RestoredActionCategories = RestoreActionCategories(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredActionCategories, models, DataTableNames.ActionCategoriesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeActionsCategories> DeleteActionCategories(List<codeActionsCategories> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;
            List<codeActionsCategories> DeletedActionCategories = new List<codeActionsCategories>();

            //Select the table and all the ActionCategories from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Delete, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeActionsCategories>(query).ToList();
            foreach (var record in Records)
            {
                DeletedActionCategories.Add(DbCMS.Delete(record, ExecutionTime, DeleteActionGUID));
            }

            var Languages = DeletedActionCategories.SelectMany(a => a.codeActionsCategoriesLanguages).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Delete(language, ExecutionTime, DeleteActionGUID);
            }
            return DeletedActionCategories;
        }

        private List<codeActionsCategories> RestoreActionCategories(List<codeActionsCategories> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeActionsCategories> RestoredActionCategories = new List<codeActionsCategories>();

            //Select the table and all the ActionCategories from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeActionsCategories>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveActionCategories(record))
                {
                    RestoredActionCategories.Add(DbCMS.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            var Languages = RestoredActionCategories.SelectMany(l => l.codeActionsCategoriesLanguages.Where(x => x.DeletedOn == l.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime);
            }

            return RestoredActionCategories;
        }

        private JsonResult ConcrrencyActionCategories(Guid PK)
        {
            ActionCategoryUpdateModel dbModel = new ActionCategoryUpdateModel();

            var ActionCategories = DbCMS.codeActionsCategories.Where(a => a.ActionCategoryGUID == PK).FirstOrDefault();
            var dbActionCategories = DbCMS.Entry(ActionCategories).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbActionCategories, dbModel);

            var Language = DbCMS.codeActionsCategoriesLanguages.Where(p => p.ActionCategoryGUID == PK).Where(p => (p.Active == true ? p.Active : p.DeletedOn == p.codeActionsCategories.DeletedOn) && p.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (ActionCategories.codeActionsCategoriesRowVersion.SequenceEqual(dbModel.codeActionsCategoriesRowVersion) && Language.codeActionsCategoriesLanguagesRowVersion.SequenceEqual(dbModel.codeActionsCategoriesLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveActionCategories(Object model)
        {
            codeActionsCategoriesLanguages ActionsCategory = Mapper.Map(model, new codeActionsCategoriesLanguages());
            int ActionCategoryDescription = DbCMS.codeActionsCategoriesLanguages
                                                 .Where(x => x.ActionCategoryDescription == ActionsCategory.ActionCategoryDescription &&
                                                             x.ActionCategoryGUID != ActionsCategory.ActionCategoryGUID &&
                                                             x.Active).Count();
            if (ActionCategoryDescription > 0)
            {
                ModelState.AddModelError("ActionCategoryDescription", "Action Category is already exists");
            }

            return (ActionCategoryDescription > 0);
        }


        #endregion

        #region Action Categories Language
        public ActionResult ActionCategoryLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/CMS/Views/Codes/ActionCategories/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            if (!CMS.HasAction(Permissions.ActionCategories.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeActionsCategoriesLanguages, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeActionsCategoriesLanguages>(DataTable.Filters);
            }

            var Result = DbCMS.codeActionsCategoriesLanguages.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.ActionCategoryGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.ActionCategoryLanguageGUID,
                                  x.LanguageID,
                                  x.ActionCategoryDescription,
                                  x.codeActionsCategoriesLanguagesRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ActionCategoryLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ActionCategories.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/ActionCategories/_LanguageUpdateModal.cshtml",
                new codeActionsCategoriesLanguages { ActionCategoryGUID = FK });
        }

        public ActionResult ActionCategoryLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ActionCategories.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/ActionCategories/_LanguageUpdateModal.cshtml", DbCMS.codeActionsCategoriesLanguages.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionCategoryLanguageCreate(codeActionsCategoriesLanguages model)
        {
            if (!CMS.HasAction(Permissions.ActionCategories.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveActionCategoriesLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/ActionCategories/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Create(model, Permissions.ActionCategoriesLanguages.CreateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.ActionCategoryLanguagesDataTable, DbCMS.PrimaryKeyControl(model), DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionCategoryLanguageUpdate(codeActionsCategoriesLanguages model)
        {
            if (!CMS.HasAction(Permissions.ActionCategories.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveActionCategoriesLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/ActionCategories/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Update(model, Permissions.ActionCategoriesLanguages.UpdateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.ActionCategoryLanguagesDataTable,
                    DbCMS.PrimaryKeyControl(model),
                    DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyActionCategoriesLanguage(model.ActionCategoryLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }



        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionCategoryLanguageDelete(codeActionsCategoriesLanguages model)
        {
            if (!CMS.HasAction(Permissions.ActionCategories.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeActionsCategoriesLanguages> DeletedLanguages = DeleteActionCategoriesLanguages(new List<codeActionsCategoriesLanguages> { model });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.ActionCategoryLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyActionCategoriesLanguage(model.ActionCategoryGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionCategoryLanguageRestore(codeActionsCategoriesLanguages model)
        {
            if (!CMS.HasAction(Permissions.ActionCategories.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveActionCategoriesLanguage(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeActionsCategoriesLanguages> RestoredLanguages = RestoreActionCategoriesLanguages(Portal.SingleToList(model));

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.ActionCategoryLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyActionCategoriesLanguage(model.ActionCategoryLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ActionCategoryLanguagesDataTableDelete(List<codeActionsCategoriesLanguages> models)
        {
            if (!CMS.HasAction(Permissions.ActionCategories.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeActionsCategoriesLanguages> DeletedLanguages = DeleteActionCategoriesLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.ActionCategoryLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ActionCategoryLanguagesDataTableRestore(List<codeActionsCategoriesLanguages> models)
        {
            if (!CMS.HasAction(Permissions.ActionCategories.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeActionsCategoriesLanguages> RestoredLanguages = RestoreActionCategoriesLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.ActionCategoryLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeActionsCategoriesLanguages> DeleteActionCategoriesLanguages(List<codeActionsCategoriesLanguages> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeActionsCategoriesLanguages> DeletedActionCategoriesLanguages = new List<codeActionsCategoriesLanguages>();

            string query = DbCMS.QueryBuilder(models, Permissions.ActionCategoriesLanguages.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbCMS.Database.SqlQuery<codeActionsCategoriesLanguages>(query).ToList();

            foreach (var language in languages)
            {
                DeletedActionCategoriesLanguages.Add(DbCMS.Delete(language, ExecutionTime, Permissions.ActionCategoriesLanguages.DeleteGuid));
            }

            return DeletedActionCategoriesLanguages;
        }

        private List<codeActionsCategoriesLanguages> RestoreActionCategoriesLanguages(List<codeActionsCategoriesLanguages> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeActionsCategoriesLanguages> RestoredLanguages = new List<codeActionsCategoriesLanguages>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Languages = DbCMS.Database.SqlQuery<codeActionsCategoriesLanguages>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveActionCategoriesLanguage(language))
                {
                    RestoredLanguages.Add(DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            return RestoredLanguages;
        }

        private bool ActiveActionCategoriesLanguage(codeActionsCategoriesLanguages model)
        {
            int LanguageID = DbCMS.codeActionsCategoriesLanguages
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.ActionCategoryGUID == model.ActionCategoryGUID &&
                                              x.ActionCategoryLanguageGUID != model.ActionCategoryLanguageGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Action Category Name in selected language already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }

        private JsonResult ConcrrencyActionCategoriesLanguage(Guid PK)
        {
            codeActionsCategoriesLanguages dbModel = new codeActionsCategoriesLanguages();

            var Language = DbCMS.codeActionsCategoriesLanguages.Where(l => l.ActionCategoryLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeActionsCategoriesLanguagesRowVersion.SequenceEqual(dbModel.codeActionsCategoriesLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }
        #endregion

        #region Action Categories Factors Dependency
        public ActionResult FactorsDependencyDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/CMS/Views/Codes/ActionCategories/_FactorsDependencyDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            var Result = (from a in DbCMS.codeActionsCategoriesFactors.AsExpandable().Where(x => x.Active && x.ActionCategoryGUID == PK)

                          select new
                          {
                              ActionCategoryFactorGUID = a.ActionCategoryFactorGUID,
                              FactorDescription = a.codeFactors.codeFactorsLanguages.Where(xl => xl.LanguageID == LAN).FirstOrDefault().FactorDescription,
                              FactorTreeLevel = a.FactorTreeLevel,
                              DependsOn = a.DependsOn == null ? "-" : a.DependsOn,
                              IsValuePurpose = a.IsValuePurpose ? "Value" : "Grouping",
                              codeActionsCategoriesFactorsRowVersion = a.codeActionsCategoriesFactorsRowVersion
                          });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult FactorsDependencyCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ActionCategories.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/ActionCategories/_FactorsDependencyUpdateModal.cshtml",
                new codeActionsCategoriesFactors { ActionCategoryGUID = FK });
        }

        public ActionResult FactorsDependencyUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ActionCategories.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = DbCMS.codeActionsCategoriesFactors.Find(PK);

            return PartialView("~/Areas/CMS/Views/Codes/ActionCategories/_FactorsDependencyUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FactorsDependencyCreate(codeActionsCategoriesFactors model)
        {
            if (!CMS.HasAction(Permissions.ActionCategories.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/CMS/Views/Codes/ActionCategories/_FactorsDependencyUpdateModal.cshtml", model);

            Guid ActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Create(model, ActionGUID, ExecutionTime);
            RemoveUsersPermissions(model.ActionCategoryGUID);
            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.FactorsDependencyDataTable, DbCMS.PrimaryKeyControl(model), DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FactorsDependencyUpdate(codeActionsCategoriesFactors model)
        {
            if (!CMS.HasAction(Permissions.ActionCategories.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid) return PartialView("~/Areas/CMS/Views/Codes/ActionCategories/_FactorsDependencyUpdateModal.cshtml", model);

            Guid ActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Update(model, ActionGUID, ExecutionTime);
            RemoveUsersPermissions(model.ActionCategoryGUID);
            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.FactorsDependencyDataTable,
                    DbCMS.PrimaryKeyControl(model),
                    DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyFactorsDependencyLanguage(model.ActionCategoryFactorGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FactorsDependencyDelete(codeActionsCategoriesFactors model)
        {
            if (!CMS.HasAction(Permissions.ActionCategories.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeActionsCategoriesFactors> DeletedFactorsDependencies = DeleteFactorsDependency(new List<codeActionsCategoriesFactors> { model });
            RemoveUsersPermissions(model.ActionCategoryGUID);
            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(DeletedFactorsDependencies, DataTableNames.FactorsDependencyDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyActionCategoriesLanguage(model.ActionCategoryGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]

        public JsonResult FactorsDependencyDataTableDelete(List<codeActionsCategoriesFactors> models)
        {
            if (!CMS.HasAction(Permissions.ActionCategories.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeActionsCategoriesFactors> DeletedFactorsDependencies = DeleteFactorsDependency(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedFactorsDependencies, models, DataTableNames.FactorsDependencyDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeActionsCategoriesFactors> DeleteFactorsDependency(List<codeActionsCategoriesFactors> models)
        {
            Guid ActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;

            List<codeActionsCategoriesFactors> DeletedFactorsDependencies = new List<codeActionsCategoriesFactors>();

            string query = DbCMS.QueryBuilder(models, ActionGUID, SubmitTypes.Delete, "");

            var factorsDependecies = DbCMS.Database.SqlQuery<codeActionsCategoriesFactors>(query).ToList();

            foreach (var factorsDependecy in factorsDependecies)
            {
                DeletedFactorsDependencies.Add(DbCMS.Delete(factorsDependecy, ExecutionTime, ActionGUID));
            }

            return DeletedFactorsDependencies;
        }

        private JsonResult ConcrrencyFactorsDependencyLanguage(Guid PK)
        {
            codeActionsCategoriesFactors dbModel = new codeActionsCategoriesFactors();

            var factorsDependecies = DbCMS.codeActionsCategoriesFactors.Where(l => l.ActionCategoryFactorGUID == PK).FirstOrDefault();
            var dbFactorsDependecies = DbCMS.Entry(factorsDependecies).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbFactorsDependecies, dbModel);

            if (factorsDependecies.codeActionsCategoriesFactorsRowVersion.SequenceEqual(dbModel.codeActionsCategoriesFactorsRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "FactorsContainer"));
        }

        private void RemoveUsersPermissions(Guid actionCategoryGUID)
        {
            var ActionList = DbCMS.codeActionsCategories.Where(x => x.ActionCategoryGUID == actionCategoryGUID).FirstOrDefault().codeActions.Select(x => x.ActionGUID).ToList();
            var userPermissions = DbCMS.userPermissions.Where(x => ActionList.Contains(x.ActionGUID)).ToList();
            DbCMS.userPermissions.RemoveRange(userPermissions);
        }
        #endregion


        #region Actions
        [Route("CMS/Codes/Actions/")]
        public ActionResult ActionsIndex()
        {
            if (!CMS.HasAction(Permissions.Actions.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/Actions/Index.cshtml");
        }

        [Route("CMS/Codes/ActionsDataTable/")]
        public JsonResult ActionsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ActionsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ActionsDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbCMS.codeActions.AsExpandable()
                       join b in DbCMS.codeActionsLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeActions.DeletedOn) && x.LanguageID == LAN) on a.ActionGUID equals b.ActionGUID
                       join c in DbCMS.codeActionsCategoriesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.ActionCategoryGUID equals c.ActionCategoryGUID
                       join d in DbCMS.codeActionsVerbsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ActionVerbGUID equals d.ActionVerbGUID
                       join e in DbCMS.codeActionsEntitiesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ActionEntityGUID equals e.ActionEntityGUID

                       select new ActionsDataTableModel
                       {
                           ActionGUID = a.ActionGUID,
                           ActionID = a.ActionID,
                           ActionType = a.ForAuditPurpose ? "Audit Only" : "Authorization",
                           Active = a.Active,
                           codeActionsRowVersion = a.codeActionsRowVersion,
                           ActionVerbDescription = d.ActionVerbDescription + " " + e.ActionEntityDescription,
                           ActionCategoryDescription = c.ActionCategoryDescription,
                           ApplicationDescription = c.codeActionsCategories.codeApplications.codeApplicationsLanguages.FirstOrDefault().ApplicationDescription
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ActionsDataTableModel> Result = Mapper.Map<List<ActionsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("CMS/Codes/Actions/Create/")]
        public ActionResult ActionCreate()
        {
            if (!CMS.HasAction(Permissions.Actions.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/Actions/Action.cshtml", new ActionUpdateModel());
        }

        [Route("CMS/Codes/Actions/Update/{PK}")]
        public ActionResult ActionUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.Actions.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            var model = (from a in DbCMS.codeActions.WherePK(PK)
                         join b in DbCMS.codeActionsLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeActions.DeletedOn) && x.LanguageID == LAN)
                         on a.ActionGUID equals b.ActionGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new ActionUpdateModel
                         {
                             ApplicationGUID = a.codeActionsCategories.ApplicationGUID,
                             ActionGUID = a.ActionGUID,
                             ActionCategoryGUID = a.ActionCategoryGUID,
                             ActionID = a.ActionID,
                             Active = a.Active,
                             ForAuditPurpose = a.ForAuditPurpose,
                             ActionVerbGUID = a.ActionVerbGUID,
                             codeActionsRowVersion = a.codeActionsRowVersion,
                             ActionEntityGUID = a.ActionEntityGUID,
                             ActionDetails = R1.ActionDetails,
                             codeActionsLanguagesRowVersion = R1.codeActionsLanguagesRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("Actions", "Codes", new { Area = "CMS" }));

            return View("~/Areas/CMS/Views/Codes/Actions/Action.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionCreate(ActionUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Actions.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            lock (this)
            {
                if (!ModelState.IsValid || ActiveActions(model)) return PartialView("~/Areas/CMS/Views/Codes/Actions/_ActionForm.cshtml", model);

                DateTime ExecutionTime = DateTime.Now;

                Guid EntityPK = Guid.NewGuid();

                codeActions Actions = Mapper.Map(model, new codeActions());
                Actions.ActionGUID = EntityPK;

                int MaxActionID = -1;

                if (model.ForAuditPurpose == false)
                {
                    //get max action id, Max will return null if there is no action, this is way the below code may looks idiotic.
                    var Result = (from a in DbCMS.codeActions
                                  join b in DbCMS.codeActionsCategories on a.ActionCategoryGUID equals b.ActionCategoryGUID
                                  join c in DbCMS.codeApplications.Where(x => x.ApplicationGUID == model.ApplicationGUID) on b.ApplicationGUID equals c.ApplicationGUID
                                  select a).ToList();
                    if (Result.Count == 0)
                    {
                        MaxActionID = 1;
                    }
                    else
                    {
                        MaxActionID = Result.Max(x => x.ActionID) + 1;
                    }
                }

                Actions.ActionID = MaxActionID;
                DbCMS.Create(Actions, Permissions.Actions.CreateGuid, ExecutionTime);

                codeActionsLanguages Language = Mapper.Map(model, new codeActionsLanguages());
                Language.ActionGUID = Actions.ActionGUID;//BA3SA
                DbCMS.Create(Language, Permissions.ActionsLanguages.CreateGuid, ExecutionTime);

                //rebuild user application token
                DbCMS.userApplicationToken
                    .Where(s => s.ApplicationGUID == model.ApplicationGUID)
                    .ToList().ForEach(s => s.Token = s.Token + "0");

                List<PartialViewModel> Partials = new List<PartialViewModel>();
                Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ActionLanguagesDataTable, ControllerContext, "LanguagesContainer"));


                List<UIButtons> UIButtons = new List<UIButtons>();
                UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Actions.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("Actions/Create", "Codes", new { Area = "CMS" })), Container = "ActionFormControls" });
                UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Actions.Update, Apps.CMS), Container = "ActionFormControls" });
                UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Actions.Delete, Apps.CMS), Container = "ActionFormControls" });

                try
                {
                    DbCMS.SaveChanges();
                    return Json(DbCMS.SingleCreateMessage(DbCMS.PrimaryKeyControl(Actions), DbCMS.RowVersionControls(Actions, Language), Partials, "", UIButtons));
                }
                catch (Exception ex)
                {
                    return Json(DbCMS.ErrorMessage(ex.Message));
                }
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionUpdate(ActionUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Actions.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveActions(model)) return PartialView("~/Areas/CMS/Views/Codes/Actions/_ActionForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeActions Actions = Mapper.Map(model, new codeActions());
            DbCMS.Update(Actions, Permissions.Actions.UpdateGuid, ExecutionTime);

            var Language = DbCMS.codeActionsLanguages.Where(x => x.ActionGUID == model.ActionGUID && x.LanguageID == LAN && x.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.ActionGUID = Actions.ActionGUID;
                DbCMS.Create(Language, Permissions.ActionsLanguages.CreateGuid, ExecutionTime);
            }
            else if (Language.ActionDetails != model.ActionDetails)
            {
                Language = Mapper.Map(model, Language);
                DbCMS.Update(Language, Permissions.ActionsLanguages.UpdateGuid, ExecutionTime);
            }

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, DbCMS.RowVersionControls(Actions, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyActions(model.ActionGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionDelete(codeActions model)
        {
            if (!CMS.HasAction(Permissions.Actions.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeActions> DeletedActions = DeleteActions(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.Actions.Restore, Apps.CMS), Container = "ActionFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(CommitedRows, DeletedActions.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyActions(model.ActionGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionRestore(codeActions model)
        {
            if (!CMS.HasAction(Permissions.Actions.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveActions(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeActions> RestoredActions = RestoreActions(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Actions.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("Actions/Create", "Codes", new { Area = "CMS" })), Container = "ActionFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Actions.Update, Apps.CMS), Container = "ActionFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Actions.Delete, Apps.CMS), Container = "ActionFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(CommitedRows, RestoredActions, DbCMS.PrimaryKeyControl(RestoredActions.FirstOrDefault()), Url.Action(DataTableNames.ActionLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyActions(model.ActionGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ActionsDataTableDelete(List<codeActions> models)
        {
            if (!CMS.HasAction(Permissions.Actions.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeActions> DeletedActions = DeleteActions(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedActions, models, DataTableNames.ActionsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ActionsDataTableRestore(List<codeActions> models)
        {
            if (!CMS.HasAction(Permissions.Actions.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeActions> RestoredActions = RestoreActions(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredActions, models, DataTableNames.ActionsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeActions> DeleteActions(List<codeActions> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;
            List<codeActions> DeletedActions = new List<codeActions>();

            //Select the table and all the Actions from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Delete, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeActions>(query).ToList();
            foreach (var record in Records)
            {
                DeletedActions.Add(DbCMS.Delete(record, ExecutionTime, DeleteActionGUID));
            }

            var Languages = DeletedActions.SelectMany(a => a.codeActionsLanguages).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Delete(language, ExecutionTime, DeleteActionGUID);
            }
            return DeletedActions;
        }

        private List<codeActions> RestoreActions(List<codeActions> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeActions> RestoredActions = new List<codeActions>();

            //Select the table and all the Actions from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeActions>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveActions(record))
                {
                    RestoredActions.Add(DbCMS.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            var Languages = RestoredActions.SelectMany(x => x.codeActionsLanguages.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime);
            }

            return RestoredActions;
        }

        private JsonResult ConcrrencyActions(Guid PK)
        {
            ActionUpdateModel dbModel = new ActionUpdateModel();

            var Actions = DbCMS.codeActions.Where(x => x.ActionGUID == PK).FirstOrDefault();
            var dbActions = DbCMS.Entry(Actions).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbActions, dbModel);

            var Language = DbCMS.codeActionsLanguages.Where(x => x.ActionGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeActions.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Actions.codeActionsRowVersion.SequenceEqual(dbModel.codeActionsRowVersion) && Language.codeActionsLanguagesRowVersion.SequenceEqual(dbModel.codeActionsLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }
        private bool ActiveActions(Object model)
        {
            codeActions Action = Mapper.Map(model, new codeActions());
            int ActionVerbDescription = DbCMS.codeActions.Where(x => x.ActionVerbGUID == Action.ActionVerbGUID &&
                                                               x.ActionEntityGUID == Action.ActionEntityGUID &&
                                                               x.ActionGUID != Action.ActionGUID &&
                                                               x.Active).Count();
            if (ActionVerbDescription > 0)
            {
                ModelState.AddModelError("ActionVerbDescription", "Action Description is already exists");
            }

            return (ActionVerbDescription > 0);
        }
        #endregion

        #region Action Languages
        public ActionResult ActionLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/CMS/Views/Codes/Actions/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeActionsLanguages, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeActionsLanguages>(DataTable.Filters);
            }

            var Result = DbCMS.codeActionsLanguages.AsNoTracking().AsExpandable().Where(l => l.LanguageID != LAN && l.ActionGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.ActionLanguageGUID,
                                  x.LanguageID,
                                  x.ActionDetails,
                                  x.codeActionsLanguagesRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ActionLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.Actions.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/Actions/_LanguageUpdateModal.cshtml",
                new codeActionsLanguages { ActionGUID = FK });
        }

        public ActionResult ActionLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.Actions.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/Actions/_LanguageUpdateModal.cshtml", DbCMS.codeActionsLanguages.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionLanguageCreate(codeActionsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Actions.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveActionsLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/Actions/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Create(model, Permissions.ActionsLanguages.CreateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.ActionLanguagesDataTable, DbCMS.PrimaryKeyControl(model), DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionLanguageUpdate(codeActionsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Actions.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveActionsLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/Actions/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Update(model, Permissions.ActionsLanguages.UpdateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.ActionLanguagesDataTable,
                    DbCMS.PrimaryKeyControl(model),
                    DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyActionsLanguage(model.ActionLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }



        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionLanguageDelete(codeActionsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Actions.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeActionsLanguages> DeletedLanguages = DeleteActionsLanguages(new List<codeActionsLanguages> { model });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.ActionLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyActionsLanguage(model.ActionGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionLanguageRestore(codeActionsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Actions.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveActionsLanguage(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeActionsLanguages> RestoredLanguages = RestoreActionsLanguages(Portal.SingleToList(model));

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.ActionLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyActionsLanguage(model.ActionLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ActionLanguagesDataTableDelete(List<codeActionsLanguages> models)
        {
            if (!CMS.HasAction(Permissions.Actions.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeActionsLanguages> DeletedLanguages = DeleteActionsLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.ActionLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ActionLanguagesDataTableRestore(List<codeActionsLanguages> models)
        {
            if (!CMS.HasAction(Permissions.Actions.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeActionsLanguages> RestoredLanguages = RestoreActionsLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.ActionLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeActionsLanguages> DeleteActionsLanguages(List<codeActionsLanguages> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeActionsLanguages> DeletedActionsLanguages = new List<codeActionsLanguages>();

            string query = DbCMS.QueryBuilder(models, Permissions.ActionsLanguages.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbCMS.Database.SqlQuery<codeActionsLanguages>(query).ToList();

            foreach (var language in languages)
            {
                DeletedActionsLanguages.Add(DbCMS.Delete(language, ExecutionTime, Permissions.ActionsLanguages.DeleteGuid));
            }

            return DeletedActionsLanguages;
        }

        private List<codeActionsLanguages> RestoreActionsLanguages(List<codeActionsLanguages> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeActionsLanguages> RestoredLanguages = new List<codeActionsLanguages>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Languages = DbCMS.Database.SqlQuery<codeActionsLanguages>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveActionsLanguage(language))
                {
                    RestoredLanguages.Add(DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            return RestoredLanguages;
        }

        private bool ActiveActionsLanguage(codeActionsLanguages model)
        {
            int LanguageID = DbCMS.codeActionsLanguages
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.ActionGUID == model.ActionGUID &&
                                              x.ActionLanguageGUID != model.ActionLanguageGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Action Description in selected language already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }

        private JsonResult ConcrrencyActionsLanguage(Guid PK)
        {
            codeActionsLanguages dbModel = new codeActionsLanguages();

            var Language = DbCMS.codeActionsLanguages.Where(x => x.ActionLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeActionsLanguagesRowVersion.SequenceEqual(dbModel.codeActionsLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }
        #endregion


        #region Sitemaps
        [Route("CMS/Codes/Sitemaps/")]
        public ActionResult SitemapsIndex()
        {
            if (!CMS.HasAction(Permissions.Sitemap.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/Sitemaps/Index.cshtml");
        }

        [Route("CMS/Codes/SitemapsDataTable/")]
        public JsonResult SitemapsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<SitemapDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<SitemapDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbCMS.codeSitemaps.AsExpandable()
                       join b in DbCMS.codeSitemapsLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeSitemaps.DeletedOn) && x.LanguageID == LAN) on a.SitemapGUID equals b.SitemapGUID
                       join c in DbCMS.codeSitemapsLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeSitemaps.DeletedOn) && x.LanguageID == LAN) on a.ParentGUID equals c.SitemapGUID into LJ1
                       from l in LJ1.DefaultIfEmpty()
                       select new SitemapDataTableModel
                       {
                           SitemapGUID = a.SitemapGUID,
                           SitemapDescription = b.SitemapDescription,
                           ActionUrl = a.ActionUrl,
                           SortID = a.SortID,
                           Active = a.Active,
                           ParentDescription = l.SitemapDescription,
                           codeSitemapsRowVersion = a.codeSitemapsRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<SitemapDataTableModel> Result = Mapper.Map<List<SitemapDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("CMS/Codes/Sitemaps/Create/")]
        public ActionResult SitemapCreate()
        {
            if (!CMS.HasAction(Permissions.Sitemap.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/Sitemaps/Sitemap.cshtml", new SitemapUpdateModel());
        }

        [Route("CMS/Codes/Sitemaps/Update/{PK}")]
        public ActionResult SitemapUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.Sitemap.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbCMS.codeSitemaps.WherePK(PK)
                         join b in DbCMS.codeSitemapsLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeSitemaps.DeletedOn) && x.LanguageID == LAN) on a.SitemapGUID equals b.SitemapGUID into LJ1

                         from R1 in LJ1.DefaultIfEmpty()

                         select new SitemapUpdateModel
                         {
                             SitemapGUID = a.SitemapGUID,
                             SitemapDescription = R1.SitemapDescription,
                             ParentGUID = a.ParentGUID,
                             ActionUrl = a.ActionUrl,
                             SortID = a.SortID,
                             Active = a.Active,
                             codeSitemapsRowVersion = a.codeSitemapsRowVersion,
                             codeSitemapsLanguagesRowVersion = R1.codeSitemapsLanguagesRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("Sitemaps", "Codes", new { Area = "CMS" }));

            return View("~/Areas/CMS/Views/Codes/Sitemaps/Sitemap.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SitemapCreate(SitemapUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Sitemap.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveSitemap(model)) return PartialView("~/Areas/CMS/Views/Codes/Sitemaps/_SitemapForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeSitemaps Sitemap = Mapper.Map(model, new codeSitemaps());
            Sitemap.SitemapGUID = EntityPK;
            DbCMS.Create(Sitemap, Permissions.Sitemap.CreateGuid, ExecutionTime);

            codeSitemapsLanguages Language = Mapper.Map(model, new codeSitemapsLanguages());
            Language.SitemapGUID = Sitemap.SitemapGUID;
            DbCMS.Create(Language, Permissions.SitemapLanguages.CreateGuid, ExecutionTime);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.SitemapLanguagesDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Sitemap.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("Sitemaps/Create", "Codes", new { Area = "CMS" })), Container = "SitemapsFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Sitemap.Update, Apps.CMS), Container = "SitemapsFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Sitemap.Delete, Apps.CMS), Container = "SitemapsFormControls" });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleCreateMessage(DbCMS.PrimaryKeyControl(Sitemap), DbCMS.RowVersionControls(Sitemap, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SitemapUpdate(SitemapUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Sitemap.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveSitemap(model)) return PartialView("~/Areas/CMS/Views/Codes/Sitemaps/_SitemapForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeSitemaps Sitemap = Mapper.Map(model, new codeSitemaps());
            DbCMS.Update(Sitemap, Permissions.Sitemap.UpdateGuid, ExecutionTime);

            var Language = DbCMS.codeSitemapsLanguages.Where(x => x.SitemapGUID == model.SitemapGUID && x.LanguageID == LAN && x.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.SitemapGUID = Sitemap.SitemapGUID;
                DbCMS.Create(Language, Permissions.SitemapLanguages.CreateGuid, ExecutionTime);
            }
            else if (Language.SitemapDescription != model.SitemapDescription)
            {
                Language = Mapper.Map(model, Language);
                DbCMS.Update(Language, Permissions.SitemapLanguages.UpdateGuid, ExecutionTime);
            }


            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, DbCMS.RowVersionControls(Sitemap, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencySitemap(model.SitemapGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SitemapDelete(codeSitemaps model)
        {
            if (!CMS.HasAction(Permissions.Sitemap.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeSitemaps> DeletedSitemaps = DeleteSitemaps(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.Sitemap.Restore, Apps.CMS), Container = "SitemapsFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(CommitedRows, DeletedSitemaps.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencySitemap(model.SitemapGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SitemapRestore(codeSitemaps model)
        {
            if (!CMS.HasAction(Permissions.Sitemap.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveSitemap(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeSitemaps> RestoredSitemaps = RestoreSitemaps(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Sitemap.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("Sitemap/Create", "Codes", new { Area = "CMS" })), Container = "SitemapsFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Sitemap.Update, Apps.CMS), Container = "SitemapsFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Sitemap.Delete, Apps.CMS), Container = "SitemapsFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(CommitedRows, RestoredSitemaps, DbCMS.PrimaryKeyControl(RestoredSitemaps.FirstOrDefault()), Url.Action(DataTableNames.SitemapLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencySitemap(model.SitemapGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult SitemapsDataTableDelete(List<codeSitemaps> models)
        {
            if (!CMS.HasAction(Permissions.Sitemap.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeSitemaps> DeletedSitemaps = DeleteSitemaps(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedSitemaps, models, DataTableNames.SitemapsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult SitemapsDataTableRestore(List<codeSitemaps> models)
        {
            if (!CMS.HasAction(Permissions.Sitemap.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeSitemaps> RestoredSitemaps = RestoreSitemaps(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredSitemaps, models, DataTableNames.SitemapsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeSitemaps> DeleteSitemaps(List<codeSitemaps> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;
            List<codeSitemaps> DeletedSitemaps = new List<codeSitemaps>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Delete, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeSitemaps>(query).ToList();
            foreach (var record in Records)
            {
                DeletedSitemaps.Add(DbCMS.Delete(record, ExecutionTime, DeleteActionGUID));
            }

            var Languages = DeletedSitemaps.SelectMany(x => x.codeSitemapsLanguages).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Delete(language, ExecutionTime, DeleteActionGUID);
            }
            return DeletedSitemaps;
        }

        private List<codeSitemaps> RestoreSitemaps(List<codeSitemaps> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeSitemaps> RestoredSitemaps = new List<codeSitemaps>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeSitemaps>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveSitemap(record))
                {
                    RestoredSitemaps.Add(DbCMS.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            var Languages = RestoredSitemaps.SelectMany(x => x.codeSitemapsLanguages.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime);
            }

            return RestoredSitemaps;
        }

        private JsonResult ConcrrencySitemap(Guid PK)
        {
            SitemapUpdateModel dbModel = new SitemapUpdateModel();

            var Sitemap = DbCMS.codeSitemaps.Where(a => a.SitemapGUID == PK).FirstOrDefault();
            var dbSitemap = DbCMS.Entry(Sitemap).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbSitemap, dbModel);

            var Language = DbCMS.codeSitemapsLanguages.Where(x => x.SitemapGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeSitemaps.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Sitemap.codeSitemapsRowVersion.SequenceEqual(dbModel.codeSitemapsRowVersion) && Language.codeSitemapsLanguagesRowVersion.SequenceEqual(dbModel.codeSitemapsLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveSitemap(Object model)
        {
            codeSitemaps Sitemap = Mapper.Map(model, new codeSitemaps());
            codeSitemapsLanguages SitemapLanguage = Mapper.Map(model, new codeSitemapsLanguages());

            //Should not allow two Sitemap has same description under the same root Sitemap.
            int SitemapDescription = (from a in DbCMS.codeSitemaps
                                      join b in DbCMS.codeSitemapsLanguages on a.SitemapGUID equals b.SitemapGUID
                                      where a.SitemapGUID != Sitemap.SitemapGUID &&
                                      a.ParentGUID == Sitemap.ParentGUID &&
                                      b.SitemapDescription == SitemapLanguage.SitemapDescription
                                      select a).Count();

            if (SitemapDescription > 0)
            {
                ModelState.AddModelError("SitemapDescription", "Sitemap Description is already exists");
            }

            return SitemapDescription > 0;
        }

        #endregion

        #region Sitemaps Language
        public ActionResult SitemapLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/CMS/Views/Codes/Sitemaps/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeSitemapsLanguages, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeSitemapsLanguages>(DataTable.Filters);
            }

            var Result = DbCMS.codeSitemapsLanguages.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.SitemapGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.SitemapLanguagesGUID,
                                  x.LanguageID,
                                  x.SitemapDescription,
                                  x.codeSitemapsLanguagesRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult SitemapLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.Sitemap.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/Sitemaps/_LanguageUpdateModal.cshtml",
                new codeSitemapsLanguages { SitemapGUID = FK });
        }

        public ActionResult SitemapLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.Sitemap.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/Sitemaps/_LanguageUpdateModal.cshtml", DbCMS.codeSitemapsLanguages.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SitemapLanguageCreate(codeSitemapsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Sitemap.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveSitemapLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/Sitemaps/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Create(model, Permissions.SitemapLanguages.CreateGuid, ExecutionTime);
            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.SitemapLanguagesDataTable,
                            DbCMS.PrimaryKeyControl(model),
                            DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SitemapLanguageUpdate(codeSitemapsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Sitemap.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveSitemapLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/Sitemaps/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Update(model, Permissions.SitemapLanguages.UpdateGuid, ExecutionTime);
            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.SitemapLanguagesDataTable,
                            DbCMS.PrimaryKeyControl(model),
                            DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencySitemapLanguage(model.SitemapLanguagesGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SitemapLanguageDelete(codeSitemapsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Sitemap.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeSitemapsLanguages> DeletedLanguages = DeleteSitemapLanguages(new List<codeSitemapsLanguages> { model });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.SitemapLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencySitemapLanguage(model.SitemapGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SitemapLanguageRestore(codeSitemapsLanguages model)
        {
            if (!CMS.HasAction(Permissions.Sitemap.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveSitemapLanguage(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeSitemapsLanguages> RestoredLanguages = RestoreSitemapLanguages(Portal.SingleToList(model));

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.SitemapLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencySitemapLanguage(model.SitemapLanguagesGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult SitemapLanguagesDataTableDelete(List<codeSitemapsLanguages> models)
        {
            if (!CMS.HasAction(Permissions.Sitemap.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeSitemapsLanguages> DeletedLanguages = DeleteSitemapLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.SitemapLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult SitemapLanguagesDataTableRestore(List<codeSitemapsLanguages> models)
        {
            if (!CMS.HasAction(Permissions.Sitemap.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeSitemapsLanguages> RestoredLanguages = RestoreSitemapLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.SitemapLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeSitemapsLanguages> DeleteSitemapLanguages(List<codeSitemapsLanguages> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeSitemapsLanguages> DeletedSitemapLanguages = new List<codeSitemapsLanguages>();

            string query = DbCMS.QueryBuilder(models, Permissions.SitemapLanguages.DeleteGuid, SubmitTypes.Delete, "");

            var Languages = DbCMS.Database.SqlQuery<codeSitemapsLanguages>(query).ToList();

            foreach (var language in Languages)
            {
                DeletedSitemapLanguages.Add(DbCMS.Delete(language, ExecutionTime, Permissions.SitemapLanguages.DeleteGuid));
            }

            return DeletedSitemapLanguages;
        }

        private List<codeSitemapsLanguages> RestoreSitemapLanguages(List<codeSitemapsLanguages> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeSitemapsLanguages> RestoredLanguages = new List<codeSitemapsLanguages>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Languages = DbCMS.Database.SqlQuery<codeSitemapsLanguages>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveSitemapLanguage(language))
                {
                    RestoredLanguages.Add(DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencySitemapLanguage(Guid PK)
        {
            codeSitemapsLanguages dbModel = new codeSitemapsLanguages();

            var Language = DbCMS.codeSitemapsLanguages.Where(l => l.SitemapLanguagesGUID == PK).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeSitemapsLanguagesRowVersion.SequenceEqual(dbModel.codeSitemapsLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveSitemapLanguage(codeSitemapsLanguages model)
        {
            int LanguageID = DbCMS.codeSitemapsLanguages
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.SitemapGUID == model.SitemapGUID &&
                                              x.SitemapLanguagesGUID != model.SitemapLanguagesGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Sitemap Name in selected language already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }

        #endregion


        #region code Table
        [Route("CMS/Codes/codeTables/")]
        public ActionResult codeTablesIndex()
        {
            if (!CMS.HasAction(Permissions.CodeTables.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/Tables/Index.cshtml");
        }

        [Route("CMS/Codes/codeTablesDataTable/")]
        public JsonResult codeTablesDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeTablesModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeTablesModel>(DataTable.Filters);
            }

            var All = (from a in DbCMS.codeTables.AsExpandable()
                       join b in DbCMS.codeSitemapsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.IndexSitemapGUID equals b.SitemapGUID
                       join c in DbCMS.codeSitemapsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.DetailsSitemapGUID equals c.SitemapGUID
                       select new codeTablesModel
                       {
                           TableGUID = a.TableGUID,
                           IndexSitemapDesciption = b.SitemapDescription,
                           DetailsSitemapDesciption = c.SitemapDescription,
                           Active = a.Active,
                           TableName = a.TableName,
                           codeTablesRowVersion = a.codeTablesRowVersion,
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<codeTablesModel> Result = Mapper.Map<List<codeTablesModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("CMS/Codes/Tables/Create/")]
        public ActionResult codeTableCreate()
        {
            if (!CMS.HasAction(Permissions.CodeTables.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/Tables/_UpdateModal.cshtml", new codeTables());
        }

        [Route("CMS/Codes/Tables/Update/{PK}")]
        public ActionResult codeTableUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.CodeTables.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbCMS.codeTables.WherePK(PK)
                         select a).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("codeTables", "Codes", new { Area = "CMS" }));

            return PartialView("~/Areas/CMS/Views/Codes/Tables/_UpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult codeTableCreate(codeTables model)
        {
            if (!CMS.HasAction(Permissions.CodeTables.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActivecodeTable(model)) return PartialView("~/Areas/CMS/Views/Codes/codeTables/_codeTableForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Create(model, Permissions.CodeTables.CreateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.codeTablesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult codeTableUpdate(codeTables model)
        {
            if (!CMS.HasAction(Permissions.CodeTables.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActivecodeTable(model)) return PartialView("~/Areas/CMS/Views/Codes/codeTables/_codeTableForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Update(model, Permissions.CodeTables.UpdateGuid, ExecutionTime);
            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.codeTablesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencycodeTable(model.TableGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult codeTableDelete(codeTables model)
        {
            if (!CMS.HasAction(Permissions.CodeTables.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeTables> DeletedcodeTables = DeletecodeTables(Portal.SingleToList(model));

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.codeTablesDataTable));
                //return Json(DbCMS.SingleDeleteMessage(CommitedRows, DeletedcodeTables.FirstOrDefault(), "LanguagesContainer"));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencycodeTable(model.TableGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult codeTableRestore(codeTables model)
        {
            if (!CMS.HasAction(Permissions.CodeTables.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActivecodeTable(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeTables> RestoredcodeTables = RestorecodeTables(Portal.SingleToList(model));

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(CommitedRows, RestoredcodeTables, DbCMS.PrimaryKeyControl(RestoredcodeTables.FirstOrDefault()), Url.Action(DataTableNames.codeTablesDataTable, Portal.GetControllerName(ControllerContext)), "page--body", null));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencycodeTable(model.TableGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult codeTablesDataTableDelete(List<codeTables> models)
        {
            if (!CMS.HasAction(Permissions.CodeTables.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeTables> DeletedcodeTables = DeletecodeTables(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedcodeTables, models, DataTableNames.codeTablesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult codeTablesDataTableRestore(List<codeTables> models)
        {
            if (!CMS.HasAction(Permissions.CodeTables.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeTables> RestoredcodeTables = RestorecodeTables(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredcodeTables, models, DataTableNames.codeTablesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeTables> DeletecodeTables(List<codeTables> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;
            List<codeTables> DeletedcodeTables = new List<codeTables>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Delete, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeTables>(query).ToList();
            foreach (var record in Records)
            {
                DeletedcodeTables.Add(DbCMS.Delete(record, ExecutionTime, DeleteActionGUID));
            }

            return DeletedcodeTables;
        }

        private List<codeTables> RestorecodeTables(List<codeTables> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeTables> RestoredcodeTables = new List<codeTables>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeTables>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActivecodeTable(record))
                {
                    RestoredcodeTables.Add(DbCMS.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            return RestoredcodeTables;
        }

        private JsonResult ConcrrencycodeTable(Guid PK)
        {
            codeTables dbModel = new codeTables();

            var codeTable = DbCMS.codeTables.Where(a => a.TableGUID == PK).FirstOrDefault();
            var dbcodeTable = DbCMS.Entry(codeTable).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbcodeTable, dbModel);

            if (codeTable.codeTablesRowVersion.SequenceEqual(dbModel.codeTablesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActivecodeTable(Object model)
        {
            codeTables codeTable = Mapper.Map(model, new codeTables());

            //Should not allow two codeTable has same description.
            int codeTableDescription = (from a in DbCMS.codeTables
                                        where a.TableGUID != codeTable.TableGUID && a.TableName == codeTable.TableName && a.Active
                                        select a).Count();

            if (codeTableDescription > 0)
            {
                ModelState.AddModelError("codeTableDescription", "codeTable Description is already exists");
            }

            return codeTableDescription > 0;
        }

        #endregion


        #region Action Verbs
        [Route("CMS/Codes/ActionsVerbs/")]
        public ActionResult ActionsVerbsIndex()
        {
            if (!CMS.HasAction(Permissions.ActionsVerbs.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/ActionsVerbs/Index.cshtml");
        }

        [Route("CMS/Codes/ActionsVerbsDataTable/")]
        public JsonResult ActionsVerbsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ActionsVerbsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ActionsVerbsDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbCMS.codeActionsVerbs.AsExpandable()
                       join b in DbCMS.codeActionsVerbsLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeActionsVerbs.DeletedOn) && x.LanguageID == LAN) on a.ActionVerbGUID equals b.ActionVerbGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       select new ActionsVerbsDataTableModel
                       {
                           ActionVerbGUID = a.ActionVerbGUID,
                           Active = a.Active,
                           codeActionsVerbsRowVersion = a.codeActionsVerbsRowVersion,
                           ActionVerbDescription = R1.ActionVerbDescription
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ActionsVerbsDataTableModel> Result = Mapper.Map<List<ActionsVerbsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("CMS/Codes/ActionsVerbs/Create/")]
        public ActionResult ActionVerbCreate()
        {
            if (!CMS.HasAction(Permissions.ActionsVerbs.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/ActionsVerbs/ActionVerb.cshtml", new ActionVerbUpdateModel());
        }

        [Route("CMS/Codes/ActionsVerbs/Update/{PK}")]
        public ActionResult ActionVerbUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ActionsVerbs.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbCMS.codeActionsVerbs.WherePK(PK)
                         join b in DbCMS.codeActionsVerbsLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeActionsVerbs.DeletedOn) && x.LanguageID == LAN)
                         on a.ActionVerbGUID equals b.ActionVerbGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new ActionVerbUpdateModel
                         {
                             ActionVerbGUID = a.ActionVerbGUID,
                             Active = a.Active,
                             codeActionsVerbsRowVersion = a.codeActionsVerbsRowVersion,
                             ActionVerbDescription = R1.ActionVerbDescription,
                             codeActionsVerbsLanguagesRowVersion = R1.codeActionsVerbsLanguagesRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("ActionsVerbs", "Codes", new { Area = "CMS" }));

            return View("~/Areas/CMS/Views/Codes/ActionsVerbs/ActionVerb.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionVerbCreate(ActionVerbUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.ActionsVerbs.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveActionVerb(model)) return PartialView("~/Areas/CMS/Views/Codes/ActionsVerbs/_ActionVerbForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeActionsVerbs ActionVerb = Mapper.Map(model, new codeActionsVerbs());
            ActionVerb.ActionVerbGUID = EntityPK;
            DbCMS.Create(ActionVerb, Permissions.ActionsVerbs.CreateGuid, ExecutionTime);

            codeActionsVerbsLanguages Language = Mapper.Map(model, new codeActionsVerbsLanguages());
            Language.ActionVerbGUID = EntityPK;
            DbCMS.Create(Language, Permissions.ActionsVerbs.CreateGuid, ExecutionTime);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ActionVerbLanguagesDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.ActionsVerbs.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("ActionVerb/Create", "Codes", new { Area = "CMS" })), Container = "ActionVerbFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.ActionsVerbs.Update, Apps.CMS), Container = "ActionVerbFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.ActionsVerbs.Delete, Apps.CMS), Container = "ActionVerbFormControls" });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleCreateMessage(DbCMS.PrimaryKeyControl(ActionVerb), DbCMS.RowVersionControls(ActionVerb, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionVerbUpdate(ActionVerbUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.ActionsVerbs.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveActionVerb(model)) return PartialView("~/Areas/CMS/Views/Codes/ActionsVerbs/_ActionVerbForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeActionsVerbs ActionVerb = Mapper.Map(model, new codeActionsVerbs());
            DbCMS.Update(ActionVerb, Permissions.ActionsVerbs.UpdateGuid, ExecutionTime);

            var Language = DbCMS.codeActionsVerbsLanguages.Where(l => l.ActionVerbGUID == model.ActionVerbGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.ActionVerbGUID = ActionVerb.ActionVerbGUID;
                DbCMS.Create(Language, Permissions.ActionsVerbs.CreateGuid, ExecutionTime);
            }
            else if (Language.ActionVerbDescription != model.ActionVerbDescription)
            {
                Language = Mapper.Map(model, Language);
                DbCMS.Update(Language, Permissions.ActionsVerbs.UpdateGuid, ExecutionTime);
            }

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, DbCMS.RowVersionControls(ActionVerb, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyActionVerb(model.ActionVerbGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionVerbDelete(codeActionsVerbs model)
        {
            if (!CMS.HasAction(Permissions.ActionsVerbs.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeActionsVerbs> DeletedActionsVerbs = DeleteActionsVerbs(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.ActionsVerbs.Restore, Apps.CMS), Container = "ActionVerbFormControls" });


            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(CommitedRows, DeletedActionsVerbs.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyActionVerb(model.ActionVerbGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionVerbRestore(codeActionsVerbs model)
        {
            if (!CMS.HasAction(Permissions.ActionsVerbs.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveActionVerb(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeActionsVerbs> RestoredActionsVerbs = RestoreActionsVerbs(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.ActionsVerbs.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("ActionVerb/Create", "Codes", new { Area = "CMS" })), Container = "ActionVerbFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.ActionsVerbs.Update, Apps.CMS), Container = "ActionVerbFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.ActionsVerbs.Delete, Apps.CMS), Container = "ActionVerbFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(CommitedRows, RestoredActionsVerbs, DbCMS.PrimaryKeyControl(RestoredActionsVerbs.FirstOrDefault()), Url.Action(DataTableNames.ActionVerbLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyActionVerb(model.ActionVerbGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ActionsVerbsDataTableDelete(List<codeActionsVerbs> models)
        {
            if (!CMS.HasAction(Permissions.ActionsVerbs.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeActionsVerbs> DeletedActionsVerbs = DeleteActionsVerbs(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedActionsVerbs, models, DataTableNames.ActionsVerbsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ActionsVerbsDataTableRestore(List<codeActionsVerbs> models)
        {
            if (!CMS.HasAction(Permissions.ActionsVerbs.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeActionsVerbs> RestoredActionsVerbs = RestoreActionsVerbs(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredActionsVerbs, models, DataTableNames.ActionsVerbsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeActionsVerbs> DeleteActionsVerbs(List<codeActionsVerbs> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;
            List<codeActionsVerbs> DeletedActionsVerbs = new List<codeActionsVerbs>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Delete, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeActionsVerbs>(query).ToList();
            foreach (var record in Records)
            {
                DeletedActionsVerbs.Add(DbCMS.Delete(record, ExecutionTime, DeleteActionGUID));
            }

            var Languages = DeletedActionsVerbs.SelectMany(a => a.codeActionsVerbsLanguages).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Delete(language, ExecutionTime, DeleteActionGUID);
            }
            return DeletedActionsVerbs;
        }

        private List<codeActionsVerbs> RestoreActionsVerbs(List<codeActionsVerbs> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeActionsVerbs> RestoredActionsVerbs = new List<codeActionsVerbs>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeActionsVerbs>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveActionVerb(record))
                {
                    RestoredActionsVerbs.Add(DbCMS.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            var Languages = RestoredActionsVerbs.SelectMany(x => x.codeActionsVerbsLanguages.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime);
            }

            return RestoredActionsVerbs;
        }

        private JsonResult ConcrrencyActionVerb(Guid PK)
        {
            ActionVerbUpdateModel dbModel = new ActionVerbUpdateModel();

            var ActionVerb = DbCMS.codeActionsVerbs.Where(x => x.ActionVerbGUID == PK).FirstOrDefault();
            var dbActionVerb = DbCMS.Entry(ActionVerb).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbActionVerb, dbModel);

            var Language = DbCMS.codeActionsVerbsLanguages.Where(x => x.ActionVerbGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeActionsVerbs.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (ActionVerb.codeActionsVerbsRowVersion.SequenceEqual(dbModel.codeActionsVerbsRowVersion) && Language.codeActionsVerbsLanguagesRowVersion.SequenceEqual(dbModel.codeActionsVerbsLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveActionVerb(Object model)
        {
            codeActionsVerbsLanguages ActionVerb = Mapper.Map(model, new codeActionsVerbsLanguages());
            int ActionVerbDescription = DbCMS.codeActionsVerbsLanguages
                                    .Where(x => x.ActionVerbDescription == ActionVerb.ActionVerbDescription &&
                                                x.ActionVerbGUID != ActionVerb.ActionVerbGUID &&
                                                x.Active).Count();
            if (ActionVerbDescription > 0)
            {
                ModelState.AddModelError("ActionVerbDescription", "ActionVerb is already exists");
            }

            return (ActionVerbDescription > 0);
        }

        #endregion

        #region Action Verbs Language
        public ActionResult ActionVerbLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/CMS/Views/Codes/ActionsVerbs/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeActionsVerbsLanguages, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeActionsVerbsLanguages>(DataTable.Filters);
            }

            var Result = DbCMS.codeActionsVerbsLanguages.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.ActionVerbGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.ActionVerbLanguageGUID,
                                  x.LanguageID,
                                  x.ActionVerbDescription,
                                  x.codeActionsVerbsLanguagesRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ActionVerbLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ActionsVerbs.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/ActionsVerbs/_LanguageUpdateModal.cshtml",
                new codeActionsVerbsLanguages { ActionVerbGUID = FK });
        }

        public ActionResult ActionVerbLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ActionsVerbs.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/ActionsVerbs/_LanguageUpdateModal.cshtml", DbCMS.codeActionsVerbsLanguages.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionVerbLanguageCreate(codeActionsVerbsLanguages model)
        {
            if (!CMS.HasAction(Permissions.ActionsVerbs.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveActionVerbLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/ActionsVerbs/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Create(model, Permissions.ActionsVerbs.CreateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.ActionVerbLanguagesDataTable, DbCMS.PrimaryKeyControl(model), DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionVerbLanguageUpdate(codeActionsVerbsLanguages model)
        {
            if (!CMS.HasAction(Permissions.ActionsVerbs.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveActionVerbLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/ActionsVerbs/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Update(model, Permissions.ActionsVerbs.UpdateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.ActionVerbLanguagesDataTable,
                    DbCMS.PrimaryKeyControl(model),
                    DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyActionVerbLanguage(model.ActionVerbLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionVerbLanguageDelete(codeActionsVerbsLanguages model)
        {
            if (!CMS.HasAction(Permissions.ActionsVerbs.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeActionsVerbsLanguages> DeletedLanguages = DeleteActionVerbLanguages(new List<codeActionsVerbsLanguages> { model });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.ActionVerbLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyActionVerbLanguage(model.ActionVerbGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionVerbLanguageRestore(codeActionsVerbsLanguages model)
        {
            if (!CMS.HasAction(Permissions.ActionsVerbs.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveActionVerbLanguage(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeActionsVerbsLanguages> RestoredLanguages = RestoreActionVerbLanguages(Portal.SingleToList(model));

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.ActionVerbLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyActionVerbLanguage(model.ActionVerbLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ActionVerbLanguagesDataTableDelete(List<codeActionsVerbsLanguages> models)
        {
            if (!CMS.HasAction(Permissions.ActionsVerbs.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeActionsVerbsLanguages> DeletedLanguages = DeleteActionVerbLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.ActionVerbLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ActionVerbLanguagesDataTableRestore(List<codeActionsVerbsLanguages> models)
        {
            if (!CMS.HasAction(Permissions.ActionsVerbs.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeActionsVerbsLanguages> RestoredLanguages = RestoreActionVerbLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.ActionVerbLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeActionsVerbsLanguages> DeleteActionVerbLanguages(List<codeActionsVerbsLanguages> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeActionsVerbsLanguages> DeletedActionVerbLanguages = new List<codeActionsVerbsLanguages>();

            string query = DbCMS.QueryBuilder(models, Permissions.ActionsVerbs.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbCMS.Database.SqlQuery<codeActionsVerbsLanguages>(query).ToList();

            foreach (var language in languages)
            {
                DeletedActionVerbLanguages.Add(DbCMS.Delete(language, ExecutionTime, Permissions.ActionsVerbs.DeleteGuid));
            }

            return DeletedActionVerbLanguages;
        }

        private List<codeActionsVerbsLanguages> RestoreActionVerbLanguages(List<codeActionsVerbsLanguages> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeActionsVerbsLanguages> RestoredLanguages = new List<codeActionsVerbsLanguages>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Languages = DbCMS.Database.SqlQuery<codeActionsVerbsLanguages>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveActionVerbLanguage(language))
                {
                    RestoredLanguages.Add(DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyActionVerbLanguage(Guid PK)
        {
            codeActionsVerbsLanguages dbModel = new codeActionsVerbsLanguages();

            var Language = DbCMS.codeActionsVerbsLanguages.Where(l => l.ActionVerbLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeActionsVerbsLanguagesRowVersion.SequenceEqual(dbModel.codeActionsVerbsLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }
        private bool ActiveActionVerbLanguage(codeActionsVerbsLanguages model)
        {
            int LanguageID = DbCMS.codeActionsVerbsLanguages
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.ActionVerbGUID == model.ActionVerbGUID &&
                                              x.ActionVerbLanguageGUID != model.ActionVerbLanguageGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Action Verb Name in selected language already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }
        #endregion

        #region Action Entities
        [Route("CMS/Codes/ActionsEntities/")]
        public ActionResult ActionsEntitiesIndex()
        {
            if (!CMS.HasAction(Permissions.ActionsEntities.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/ActionsEntities/Index.cshtml");
        }

        [Route("CMS/Codes/ActionsEntitiesDataTable/")]
        public JsonResult ActionsEntitiesDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ActionsEntitiesDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ActionsEntitiesDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbCMS.codeActionsEntities.AsExpandable()
                       join b in DbCMS.codeActionsEntitiesLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeActionsEntities.DeletedOn) && x.LanguageID == LAN) on a.ActionEntityGUID equals b.ActionEntityGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       select new ActionsEntitiesDataTableModel
                       {
                           ActionEntityGUID = a.ActionEntityGUID,
                           Active = a.Active,
                           codeActionsEntitiesRowVersion = a.codeActionsEntitiesRowVersion,
                           ActionEntityDescription = R1.ActionEntityDescription
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ActionsEntitiesDataTableModel> Result = Mapper.Map<List<ActionsEntitiesDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("CMS/Codes/ActionsEntities/Create/")]
        public ActionResult ActionEntityCreate()
        {
            if (!CMS.HasAction(Permissions.ActionsEntities.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/ActionsEntities/ActionEntity.cshtml", new ActionEntityUpdateModel());
        }

        [Route("CMS/Codes/ActionsEntities/Update/{PK}")]
        public ActionResult ActionEntityUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ActionsEntities.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbCMS.codeActionsEntities.WherePK(PK)
                         join b in DbCMS.codeActionsEntitiesLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeActionsEntities.DeletedOn) && x.LanguageID == LAN)
                         on a.ActionEntityGUID equals b.ActionEntityGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new ActionEntityUpdateModel
                         {
                             ActionEntityGUID = a.ActionEntityGUID,
                             Active = a.Active,
                             codeActionsEntitiesRowVersion = a.codeActionsEntitiesRowVersion,
                             ActionEntityDescription = R1.ActionEntityDescription,
                             codeActionsEntitiesLanguagesRowVersion = R1.codeActionsEntitiesLanguagesRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("ActionsEntities", "Codes", new { Area = "CMS" }));

            return View("~/Areas/CMS/Views/Codes/ActionsEntities/ActionEntity.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionEntityCreate(ActionEntityUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.ActionsEntities.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveActionEntity(model)) return PartialView("~/Areas/CMS/Views/Codes/ActionsEntities/_ActionEntityForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeActionsEntities ActionEntity = Mapper.Map(model, new codeActionsEntities());
            ActionEntity.ActionEntityGUID = EntityPK;
            DbCMS.Create(ActionEntity, Permissions.ActionsEntities.CreateGuid, ExecutionTime);

            codeActionsEntitiesLanguages Language = Mapper.Map(model, new codeActionsEntitiesLanguages());
            Language.ActionEntityGUID = EntityPK;
            DbCMS.Create(Language, Permissions.ActionsEntities.CreateGuid, ExecutionTime);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ActionEntityLanguageDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.ActionsEntities.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("ActionEntity/Create", "Codes", new { Area = "CMS" })), Container = "ActionEntityFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.ActionsEntities.Update, Apps.CMS), Container = "ActionEntityFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.ActionsEntities.Delete, Apps.CMS), Container = "ActionEntityFormControls" });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleCreateMessage(DbCMS.PrimaryKeyControl(ActionEntity), DbCMS.RowVersionControls(ActionEntity, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionEntityUpdate(ActionEntityUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.ActionsEntities.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveActionEntity(model)) return PartialView("~/Areas/CMS/Views/Codes/ActionsEntities/_ActionEntityForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeActionsEntities ActionEntity = Mapper.Map(model, new codeActionsEntities());
            DbCMS.Update(ActionEntity, Permissions.ActionsEntities.UpdateGuid, ExecutionTime);

            var Language = DbCMS.codeActionsEntitiesLanguages.Where(l => l.ActionEntityGUID == model.ActionEntityGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.ActionEntityGUID = ActionEntity.ActionEntityGUID;
                DbCMS.Create(Language, Permissions.ActionsEntities.CreateGuid, ExecutionTime);
            }
            else if (Language.ActionEntityDescription != model.ActionEntityDescription)
            {
                Language = Mapper.Map(model, Language);
                DbCMS.Update(Language, Permissions.ActionsEntities.UpdateGuid, ExecutionTime);
            }

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, DbCMS.RowVersionControls(ActionEntity, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyActionEntity(model.ActionEntityGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionEntityDelete(codeActionsEntities model)
        {
            if (!CMS.HasAction(Permissions.ActionsEntities.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeActionsEntities> DeletedActionsEntities = DeleteActionsEntities(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.ActionsEntities.Restore, Apps.CMS), Container = "ActionEntityFormControls" });


            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(CommitedRows, DeletedActionsEntities.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyActionEntity(model.ActionEntityGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionEntityRestore(codeActionsEntities model)
        {
            if (!CMS.HasAction(Permissions.ActionsEntities.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveActionEntity(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeActionsEntities> RestoredActionsEntities = RestoreActionsEntities(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.ActionsEntities.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("ActionEntity/Create", "Codes", new { Area = "CMS" })), Container = "ActionEntityFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.ActionsEntities.Update, Apps.CMS), Container = "ActionEntityFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.ActionsEntities.Delete, Apps.CMS), Container = "ActionEntityFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(CommitedRows, RestoredActionsEntities, DbCMS.PrimaryKeyControl(RestoredActionsEntities.FirstOrDefault()), Url.Action(DataTableNames.ActionEntityLanguageDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyActionEntity(model.ActionEntityGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ActionsEntitiesDataTableDelete(List<codeActionsEntities> models)
        {
            if (!CMS.HasAction(Permissions.ActionsEntities.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeActionsEntities> DeletedActionsEntities = DeleteActionsEntities(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedActionsEntities, models, DataTableNames.ActionsEntitiesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ActionsEntitiesDataTableRestore(List<codeActionsEntities> models)
        {
            if (!CMS.HasAction(Permissions.ActionsEntities.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeActionsEntities> RestoredActionsEntities = RestoreActionsEntities(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredActionsEntities, models, DataTableNames.ActionsEntitiesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeActionsEntities> DeleteActionsEntities(List<codeActionsEntities> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;
            List<codeActionsEntities> DeletedActionsEntities = new List<codeActionsEntities>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Delete, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeActionsEntities>(query).ToList();
            foreach (var record in Records)
            {
                DeletedActionsEntities.Add(DbCMS.Delete(record, ExecutionTime, DeleteActionGUID));
            }

            var Languages = DeletedActionsEntities.SelectMany(a => a.codeActionsEntitiesLanguages).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Delete(language, ExecutionTime, DeleteActionGUID);
            }
            return DeletedActionsEntities;
        }

        private List<codeActionsEntities> RestoreActionsEntities(List<codeActionsEntities> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeActionsEntities> RestoredActionsEntities = new List<codeActionsEntities>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeActionsEntities>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveActionEntity(record))
                {
                    RestoredActionsEntities.Add(DbCMS.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            var Languages = RestoredActionsEntities.SelectMany(x => x.codeActionsEntitiesLanguages.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime);
            }

            return RestoredActionsEntities;
        }

        private JsonResult ConcrrencyActionEntity(Guid PK)
        {
            ActionEntityUpdateModel dbModel = new ActionEntityUpdateModel();

            var ActionEntity = DbCMS.codeActionsEntities.Where(x => x.ActionEntityGUID == PK).FirstOrDefault();
            var dbActionEntity = DbCMS.Entry(ActionEntity).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbActionEntity, dbModel);

            var Language = DbCMS.codeActionsEntitiesLanguages.Where(x => x.ActionEntityGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeActionsEntities.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (ActionEntity.codeActionsEntitiesRowVersion.SequenceEqual(dbModel.codeActionsEntitiesRowVersion) && Language.codeActionsEntitiesLanguagesRowVersion.SequenceEqual(dbModel.codeActionsEntitiesLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveActionEntity(Object model)
        {
            codeActionsEntitiesLanguages ActionEntity = Mapper.Map(model, new codeActionsEntitiesLanguages());
            int ActionEntityDescription = DbCMS.codeActionsEntitiesLanguages
                                    .Where(x => x.ActionEntityDescription == ActionEntity.ActionEntityDescription &&
                                                x.ActionEntityGUID != ActionEntity.ActionEntityGUID &&
                                                x.Active).Count();
            if (ActionEntityDescription > 0)
            {
                ModelState.AddModelError("ActionEntityDescription", "ActionEntity is already exists");
            }

            return (ActionEntityDescription > 0);
        }

        #endregion

        #region Action Entities Language
        public ActionResult ActionEntityLanguageDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/CMS/Views/Codes/ActionsEntities/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeActionsEntitiesLanguages, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeActionsEntitiesLanguages>(DataTable.Filters);
            }

            var Result = DbCMS.codeActionsEntitiesLanguages.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.ActionEntityGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.ActionEntityLanguageGUID,
                                  x.LanguageID,
                                  x.ActionEntityDescription,
                                  x.codeActionsEntitiesLanguagesRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        //ActionsEntitiesLanguagesCreate
        public ActionResult ActionEntityLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ActionsEntities.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/ActionsEntities/_LanguageUpdateModal.cshtml",
                new codeActionsEntitiesLanguages { ActionEntityGUID = FK });
        }

        public ActionResult ActionEntityLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ActionsEntities.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/ActionsEntities/_LanguageUpdateModal.cshtml", DbCMS.codeActionsEntitiesLanguages.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionEntityLanguageCreate(codeActionsEntitiesLanguages model)
        {
            if (!CMS.HasAction(Permissions.ActionsEntities.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveActionEntityLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/ActionsEntities/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Create(model, Permissions.ActionsEntities.CreateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.ActionEntityLanguageDataTable, DbCMS.PrimaryKeyControl(model), DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionEntityLanguageUpdate(codeActionsEntitiesLanguages model)
        {
            if (!CMS.HasAction(Permissions.ActionsEntities.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveActionEntityLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/ActionsEntities/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Update(model, Permissions.ActionsEntities.UpdateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.ActionEntityLanguageDataTable,
                    DbCMS.PrimaryKeyControl(model),
                    DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyActionEntityLanguage(model.ActionEntityLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionEntityLanguageDelete(codeActionsEntitiesLanguages model)
        {
            if (!CMS.HasAction(Permissions.ActionsEntities.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeActionsEntitiesLanguages> DeletedLanguages = DeleteActionEntityLanguages(new List<codeActionsEntitiesLanguages> { model });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.ActionEntityLanguageDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyActionEntityLanguage(model.ActionEntityGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ActionEntityLanguageRestore(codeActionsEntitiesLanguages model)
        {
            if (!CMS.HasAction(Permissions.ActionsEntities.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveActionEntityLanguage(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeActionsEntitiesLanguages> RestoredLanguages = RestoreActionEntityLanguages(Portal.SingleToList(model));

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.ActionEntityLanguageDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyActionEntityLanguage(model.ActionEntityLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ActionEntityLanguageDataTableDelete(List<codeActionsEntitiesLanguages> models)
        {
            if (!CMS.HasAction(Permissions.ActionsEntities.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeActionsEntitiesLanguages> DeletedLanguages = DeleteActionEntityLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.ActionEntityLanguageDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ActionEntityLanguageDataTableRestore(List<codeActionsEntitiesLanguages> models)
        {
            if (!CMS.HasAction(Permissions.ActionsEntities.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeActionsEntitiesLanguages> RestoredLanguages = RestoreActionEntityLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.ActionEntityLanguageDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeActionsEntitiesLanguages> DeleteActionEntityLanguages(List<codeActionsEntitiesLanguages> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeActionsEntitiesLanguages> DeletedActionEntityLanguages = new List<codeActionsEntitiesLanguages>();

            string query = DbCMS.QueryBuilder(models, Permissions.ActionsEntities.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbCMS.Database.SqlQuery<codeActionsEntitiesLanguages>(query).ToList();

            foreach (var language in languages)
            {
                DeletedActionEntityLanguages.Add(DbCMS.Delete(language, ExecutionTime, Permissions.ActionsEntities.DeleteGuid));
            }

            return DeletedActionEntityLanguages;
        }

        private List<codeActionsEntitiesLanguages> RestoreActionEntityLanguages(List<codeActionsEntitiesLanguages> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeActionsEntitiesLanguages> RestoredLanguages = new List<codeActionsEntitiesLanguages>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Languages = DbCMS.Database.SqlQuery<codeActionsEntitiesLanguages>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveActionEntityLanguage(language))
                {
                    RestoredLanguages.Add(DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyActionEntityLanguage(Guid PK)
        {
            codeActionsEntitiesLanguages dbModel = new codeActionsEntitiesLanguages();

            var Language = DbCMS.codeActionsEntitiesLanguages.Where(l => l.ActionEntityLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeActionsEntitiesLanguagesRowVersion.SequenceEqual(dbModel.codeActionsEntitiesLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }
        private bool ActiveActionEntityLanguage(codeActionsEntitiesLanguages model)
        {
            int LanguageID = DbCMS.codeActionsEntitiesLanguages
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.ActionEntityGUID == model.ActionEntityGUID &&
                                              x.ActionEntityLanguageGUID != model.ActionEntityLanguageGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Action Entity Name in selected language already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }
        #endregion


        #region Connection String
        [Route("CMS/Codes/Connections/")]
        public ActionResult ConnectionsIndex()
        {
            return View("~/Areas/CMS/Views/Codes/Connections/Index.cshtml");
        }

        [Route("CMS/Codes/ConnectionsDataTable/")]
        public JsonResult ConnectionsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ConnectionDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ConnectionDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.Connection.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var All = (from a in DbCMS.codeConnections.AsNoTracking().AsExpandable().Where(x => AuthorizedList.Contains(x.OrganizationsInstancesGUID + "," + x.DutyStationGUID))
                       join b in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.LanguageID == LAN) on a.OrganizationsInstancesGUID equals b.OrganizationInstanceGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       select new ConnectionDataTableModel
                       {
                           ApplicationGUID = a.ApplicationGUID.ToString(),
                           ConnectionGUID = a.ConnectionGUID,
                           Active = a.Active,
                           codeConnectionsRowVersion = a.codeConnectionsRowVersion,
                           InstanceDescription = a.InstanceDescription,
                           DataSource = a.DataSource,
                           InitialCatalog = a.InitialCatalog,
                           OrganizationsInstancesDescription = R1.OrganizationInstanceDescription
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ConnectionDataTableModel> Result = Mapper.Map<List<ConnectionDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("CMS/Codes/Connections/Create/")]
        public ActionResult ConnectionCreate()
        {
            if (!CMS.HasAction(Permissions.Connection.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/Connections/Connection.cshtml", new ConnectionUpdateModel());
        }

        [Route("CMS/Codes/Connections/Update/{PK}")]
        public ActionResult ConnectionUpdate(Guid PK)
        {
            var model = (from a in DbCMS.codeConnections.WherePK(PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.DeletedOn))
                         select new ConnectionUpdateModel
                         {
                             ConnectionGUID = a.ConnectionGUID,
                             Active = a.Active,
                             codeConnectionsRowVersion = a.codeConnectionsRowVersion,
                             InstanceDescription = a.InstanceDescription,
                             Metadata = a.Metadata,
                             Provider = a.Provider,
                             DataSource = a.DataSource,
                             InitialCatalog = a.InitialCatalog,
                             UserID = a.UserID,
                             Password = a.Password,
                             ApplicationGUID = a.ApplicationGUID,
                             OrganizationsInstancesGUID = a.OrganizationsInstancesGUID,
                             DutyStationGUID = a.DutyStationGUID
                         }).FirstOrDefault();
            if (!CMS.HasAction(Permissions.Connection.Access, Apps.CMS, model.OrganizationsInstancesGUID + "," + model.DutyStationGUID))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("Connections", "Codes", new { Area = "CMS" }));

            return View("~/Areas/CMS/Views/Codes/Connections/Connection.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConnectionCreate(ConnectionUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Connection.Create, Apps.CMS, model.OrganizationsInstancesGUID + "," + model.DutyStationGUID))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveConnection(model)) return PartialView("~/Areas/CMS/Views/Codes/Connections/_ConnectionForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeConnections Connection = Mapper.Map(model, new codeConnections());
            Connection.ConnectionGUID = EntityPK;
            DbCMS.Create(Connection, Permissions.Connection.CreateGuid, ExecutionTime);


            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Connection.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("Connection/Create", "Codes", new { Area = "CMS" })), Container = "ConnectionFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Connection.Update, Apps.CMS), Container = "ConnectionFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Connection.Delete, Apps.CMS), Container = "ConnectionFormControls" });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleCreateMessage(DbCMS.PrimaryKeyControl(Connection), DbCMS.RowVersionControls(Portal.SingleToList(Connection)), null, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConnectionUpdate(ConnectionUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Connection.Update, Apps.CMS, model.OrganizationsInstancesGUID + "," + model.DutyStationGUID))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveConnection(model)) return PartialView("~/Areas/CMS/Views/Codes/Connections/_ConnectionForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeConnections Connection = Mapper.Map(model, new codeConnections());
            DbCMS.Update(Connection, Permissions.Connection.UpdateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, DbCMS.RowVersionControls(Portal.SingleToList(Connection))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyConnection(model.ConnectionGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConnectionDelete(codeConnections model)
        {
            if (!CMS.HasAction(Permissions.Connection.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeConnections> DeletedConnections = DeleteConnections(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.Connection.Restore, Apps.CMS), Container = "ConnectionFormControls" });


            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(CommitedRows, DeletedConnections.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyConnection(model.ConnectionGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConnectionRestore(codeConnections model)
        {
            if (!CMS.HasAction(Permissions.Connection.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveConnection(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeConnections> RestoredConnections = RestoreConnections(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Connection.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("Connection/Create", "Codes", new { Area = "CMS" })), Container = "ConnectionFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Connection.Update, Apps.CMS), Container = "ConnectionFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Connection.Delete, Apps.CMS), Container = "ConnectionFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(CommitedRows, RestoredConnections, DbCMS.PrimaryKeyControl(RestoredConnections.FirstOrDefault()),
                    null, null, UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyConnection(model.ConnectionGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ConnectionsDataTableDelete(List<codeConnections> models)
        {
            if (!CMS.HasAction(Permissions.Connection.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeConnections> DeletedConnections = DeleteConnections(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedConnections, models, DataTableNames.ConnectionsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ConnectionsDataTableRestore(List<codeConnections> models)
        {
            if (!CMS.HasAction(Permissions.Connection.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeConnections> RestoredConnections = RestoreConnections(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredConnections, models, DataTableNames.ConnectionsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeConnections> DeleteConnections(List<codeConnections> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;
            List<codeConnections> DeletedConnections = new List<codeConnections>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "SELECT ConnectionGUID,CONVERT(varchar(50), OrganizationsInstancesGUID)+','+CONVERT(varchar(50), DutyStationGUID)  as C2 ,codeConnectionsRowVersion FROM codeConnections where ConnectionGUID in (" + string.Join(",", models.Select(x => "'" + x.ConnectionGUID + "'").ToArray()) + ")";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Delete, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeConnections>(query).ToList();
            foreach (var record in Records)
            {
                DeletedConnections.Add(DbCMS.Delete(record, ExecutionTime, DeleteActionGUID));
            }


            return DeletedConnections;
        }

        private List<codeConnections> RestoreConnections(List<codeConnections> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeConnections> RestoredConnections = new List<codeConnections>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeConnections>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveConnection(record))
                {
                    RestoredConnections.Add(DbCMS.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }



            return RestoredConnections;
        }

        private JsonResult ConcrrencyConnection(Guid PK)
        {
            ConnectionUpdateModel dbModel = new ConnectionUpdateModel();

            var Connection = DbCMS.codeConnections.Where(x => x.ConnectionGUID == PK).FirstOrDefault();
            var dbConnection = DbCMS.Entry(Connection).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbConnection, dbModel);

            var Language = DbCMS.codeConnections.Where(x => x.ConnectionGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.DeletedOn)).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Connection.codeConnectionsRowVersion.SequenceEqual(dbModel.codeConnectionsRowVersion) && Language.codeConnectionsRowVersion.SequenceEqual(dbModel.codeConnectionsRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveConnection(Object model)
        {
            codeConnections Connection = Mapper.Map(model, new codeConnections());
            int ConnectionDescription = DbCMS.codeConnections
                                    .Where(x => x.InstanceDescription == Connection.InstanceDescription &&
                                                x.ConnectionGUID != Connection.ConnectionGUID &&
                                                x.Active).Count();
            if (ConnectionDescription > 0)
            {
                ModelState.AddModelError("InstanceDescription", "Instance Description is already exists");
            }

            return (ConnectionDescription > 0);
        }

        #endregion

        #region Notifications
        [Route("CMS/Codes/Notifications/")]
        public ActionResult NotificationsIndex()
        {
            if (!CMS.HasAction(Permissions.Notifications.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/Notifications/Index.cshtml");
        }

        [Route("CMS/Codes/NotificationsDataTable/")]
        public JsonResult NotificationsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<NotificationDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<NotificationDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbCMS.codeNotifications.AsExpandable()
                       join b in DbCMS.codeNotificationLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeNotifications.DeletedOn) && x.LanguageID == LAN) on a.NotificationGUID equals b.NotificationGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join c in DbCMS.codeApplicationsLanguages.Where(x => x.Active == true && x.LanguageID == LAN) on a.ApplicationGUID equals c.ApplicationGUID into LJ2
                       from R2 in LJ2.DefaultIfEmpty()
                       select new NotificationDataTableModel
                       {
                           ApplicationGUID = a.ApplicationGUID.ToString(),
                           NotificationGUID = a.NotificationGUID,
                           Active = a.Active,
                           codeNotificationsRowVersion = a.codeNotificationsRowVersion,
                           TitleTemplete = R1.TitleTemplete,
                           ApplicationDescription = R2.ApplicationDescription

                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<NotificationDataTableModel> Result = Mapper.Map<List<NotificationDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("CMS/Codes/Notifications/Create/")]
        public ActionResult NotificationCreate()
        {
            if (!CMS.HasAction(Permissions.Notifications.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/CMS/Views/Codes/Notifications/Notification.cshtml", new NotificationUpdateModel());
        }

        [Route("CMS/Codes/Notifications/Update/{PK}")]
        public ActionResult NotificationUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.Notifications.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbCMS.codeNotifications.WherePK(PK)
                         join b in DbCMS.codeNotificationLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeNotifications.DeletedOn) && x.LanguageID == LAN)
                         on a.NotificationGUID equals b.NotificationGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         join c in DbCMS.codeApplicationsLanguages.Where(x => x.Active == true && x.LanguageID == LAN) on a.ApplicationGUID equals c.ApplicationGUID into LJ2
                         from R2 in LJ2.DefaultIfEmpty()
                         select new NotificationUpdateModel
                         {
                             NotificationGUID = a.NotificationGUID,
                             Active = a.Active,
                             codeNotificationsRowVersion = a.codeNotificationsRowVersion,
                             ApplicationGUID = R2.ApplicationGUID,
                             codeNotificationLanguagesRowVersion = R1.codeNotificationLanguagesRowVersion,
                             TitleTemplete = R1.TitleTemplete,
                             DetailsTemplete = R1.DetailsTemplete,
                             MailTemplete = R1.MailTemplete,
                             RedirectURL = a.RedirectURL,
                             Icon = a.Icon,
                             UserProfileIcon = a.UserProfileIcon

                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("Notifications", "Codes", new { Area = "CMS" }));

            return View("~/Areas/CMS/Views/Codes/Notifications/Notification.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NotificationCreate(NotificationUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Notifications.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveNotification(model)) return PartialView("~/Areas/CMS/Views/Codes/Notifications/_NotificationForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeNotifications Notification = Mapper.Map(model, new codeNotifications());
            Notification.NotificationGUID = EntityPK;
            DbCMS.Create(Notification, Permissions.Notifications.CreateGuid, ExecutionTime);

            codeNotificationLanguages Language = Mapper.Map(model, new codeNotificationLanguages());
            Language.NotificationGUID = EntityPK;
            DbCMS.Create(Language, Permissions.Notifications.CreateGuid, ExecutionTime);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.NotificationLanguagesDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Notifications.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("Notifications/Create", "Codes", new { Area = "CMS" })), Container = "NotificationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Notifications.Update, Apps.CMS), Container = "NotificationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Notifications.Delete, Apps.CMS), Container = "NotificationFormControls" });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleCreateMessage(DbCMS.PrimaryKeyControl(Notification), DbCMS.RowVersionControls(Notification, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NotificationUpdate(NotificationUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Notifications.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveNotification(model)) return PartialView("~/Areas/CMS/Views/Codes/Notifications/_NotificationForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeNotifications Notification = Mapper.Map(model, new codeNotifications());
            DbCMS.Update(Notification, Permissions.Notifications.UpdateGuid, ExecutionTime);

            var Language = DbCMS.codeNotificationLanguages.Where(l => l.NotificationGUID == model.NotificationGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.NotificationGUID = Notification.NotificationGUID;
                DbCMS.Create(Language, Permissions.Notifications.CreateGuid, ExecutionTime);
            }
            else if (Language.TitleTemplete != model.TitleTemplete)
            {
                Language = Mapper.Map(model, Language);
                DbCMS.Update(Language, Permissions.Notifications.UpdateGuid, ExecutionTime);
            }

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, DbCMS.RowVersionControls(Notification, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyNotification(model.NotificationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NotificationDelete(codeNotifications model)
        {
            if (!CMS.HasAction(Permissions.Notifications.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeNotifications> DeletedNotifications = DeleteNotifications(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.Notifications.Restore, Apps.CMS), Container = "NotificationFormControls" });


            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(CommitedRows, DeletedNotifications.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyNotification(model.NotificationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NotificationRestore(codeNotifications model)
        {
            if (!CMS.HasAction(Permissions.Notifications.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveNotification(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeNotifications> RestoredNotifications = RestoreNotifications(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Notifications.Create, Apps.CMS, new UrlHelper(Request.RequestContext).Action("Notification/Create", "Codes", new { Area = "CMS" })), Container = "NotificationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Notifications.Update, Apps.CMS), Container = "NotificationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Notifications.Delete, Apps.CMS), Container = "NotificationFormControls" });

            try
            {
                int CommitedRows = DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(CommitedRows, RestoredNotifications, DbCMS.PrimaryKeyControl(RestoredNotifications.FirstOrDefault()), Url.Action(DataTableNames.NotificationLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyNotification(model.NotificationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult NotificationsDataTableDelete(List<codeNotifications> models)
        {
            if (!CMS.HasAction(Permissions.Notifications.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeNotifications> DeletedNotifications = DeleteNotifications(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedNotifications, models, DataTableNames.NotificationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult NotificationsDataTableRestore(List<codeNotifications> models)
        {
            if (!CMS.HasAction(Permissions.Notifications.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeNotifications> RestoredNotifications = RestoreNotifications(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredNotifications, models, DataTableNames.NotificationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeNotifications> DeleteNotifications(List<codeNotifications> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;
            List<codeNotifications> DeletedNotifications = new List<codeNotifications>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Delete, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeNotifications>(query).ToList();
            foreach (var record in Records)
            {
                DeletedNotifications.Add(DbCMS.Delete(record, ExecutionTime, DeleteActionGUID));
            }

            var Languages = DeletedNotifications.SelectMany(a => a.codeNotificationLanguages).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Delete(language, ExecutionTime, DeleteActionGUID);
            }
            return DeletedNotifications;
        }

        private List<codeNotifications> RestoreNotifications(List<codeNotifications> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeNotifications> RestoredNotifications = new List<codeNotifications>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeNotifications>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveNotification(record))
                {
                    RestoredNotifications.Add(DbCMS.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            var Languages = RestoredNotifications.SelectMany(x => x.codeNotificationLanguages.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime);
            }

            return RestoredNotifications;
        }

        private JsonResult ConcrrencyNotification(Guid PK)
        {
            NotificationUpdateModel dbModel = new NotificationUpdateModel();

            var Notification = DbCMS.codeNotifications.Where(x => x.NotificationGUID == PK).FirstOrDefault();
            var dbNotification = DbCMS.Entry(Notification).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbNotification, dbModel);

            var Language = DbCMS.codeNotificationLanguages.Where(x => x.NotificationGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeNotifications.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Notification.codeNotificationsRowVersion.SequenceEqual(dbModel.codeNotificationsRowVersion) && Language.codeNotificationLanguagesRowVersion.SequenceEqual(dbModel.codeNotificationLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveNotification(Object model)
        {
            codeNotificationLanguages Notification = Mapper.Map(model, new codeNotificationLanguages());
            int NotificationDescription = DbCMS.codeNotificationLanguages
                                    .Where(x => x.TitleTemplete == Notification.TitleTemplete &&
                                                x.NotificationGUID != Notification.NotificationGUID &&
                                                x.Active).Count();
            if (NotificationDescription > 0)
            {
                ModelState.AddModelError("NotificationDescription", "Notification is already exists");
            }

            return (NotificationDescription > 0);
        }

        #endregion

        #region Notification Languages
        public ActionResult NotificationLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/CMS/Views/Codes/Notifications/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeNotificationLanguages, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeNotificationLanguages>(DataTable.Filters);
            }

            var Result = DbCMS.codeNotificationLanguages.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.NotificationGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.NotificationLanguageGUID,
                                  x.LanguageID,
                                  x.TitleTemplete,
                                  x.DetailsTemplete,
                                  x.codeNotificationLanguagesRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult NotificationLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.Notifications.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/Notifications/_LanguageUpdateModal.cshtml",
                new codeNotificationLanguages { NotificationGUID = FK });
        }

        public ActionResult NotificationLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.Notifications.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/CMS/Views/Codes/Notifications/_LanguageUpdateModal.cshtml", DbCMS.codeNotificationLanguages.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NotificationLanguageCreate(codeNotificationLanguages model)
        {
            if (!CMS.HasAction(Permissions.Notifications.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveNotificationLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/Notifications/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Create(model, Permissions.Notifications.CreateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.NotificationLanguagesDataTable, DbCMS.PrimaryKeyControl(model), DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NotificationLanguageUpdate(codeNotificationLanguages model)
        {
            if (!CMS.HasAction(Permissions.Notifications.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveNotificationLanguage(model)) return PartialView("~/Areas/CMS/Views/Codes/Notifications/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Update(model, Permissions.Notifications.UpdateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.NotificationLanguagesDataTable,
                    DbCMS.PrimaryKeyControl(model),
                    DbCMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyNotificationLanguage(model.NotificationLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NotificationLanguageDelete(codeNotificationLanguages model)
        {
            if (!CMS.HasAction(Permissions.Notifications.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeNotificationLanguages> DeletedLanguages = DeleteNotificationLanguages(new List<codeNotificationLanguages> { model });

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.NotificationLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyNotificationLanguage(model.NotificationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NotificationLanguageRestore(codeNotificationLanguages model)
        {
            if (!CMS.HasAction(Permissions.Notifications.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveNotificationLanguage(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<codeNotificationLanguages> RestoredLanguages = RestoreNotificationLanguages(Portal.SingleToList(model));

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.NotificationLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyNotificationLanguage(model.NotificationLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult NotificationLanguagesDataTableDelete(List<codeNotificationLanguages> models)
        {
            if (!CMS.HasAction(Permissions.Notifications.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeNotificationLanguages> DeletedLanguages = DeleteNotificationLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.NotificationLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult NotificationLanguagesDataTableRestore(List<codeNotificationLanguages> models)
        {
            if (!CMS.HasAction(Permissions.Notifications.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeNotificationLanguages> RestoredLanguages = RestoreNotificationLanguages(models);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.NotificationLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeNotificationLanguages> DeleteNotificationLanguages(List<codeNotificationLanguages> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeNotificationLanguages> DeletedNotificationLanguages = new List<codeNotificationLanguages>();

            string query = DbCMS.QueryBuilder(models, Permissions.Notifications.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbCMS.Database.SqlQuery<codeNotificationLanguages>(query).ToList();

            foreach (var language in languages)
            {
                DeletedNotificationLanguages.Add(DbCMS.Delete(language, ExecutionTime, Permissions.Notifications.DeleteGuid));
            }

            return DeletedNotificationLanguages;
        }

        private List<codeNotificationLanguages> RestoreNotificationLanguages(List<codeNotificationLanguages> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid RestoreActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            DateTime RestoringTime = DateTime.Now;

            List<codeNotificationLanguages> RestoredLanguages = new List<codeNotificationLanguages>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var Languages = DbCMS.Database.SqlQuery<codeNotificationLanguages>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveNotificationLanguage(language))
                {
                    RestoredLanguages.Add(DbCMS.Restore(language, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyNotificationLanguage(Guid PK)
        {
            codeNotificationLanguages dbModel = new codeNotificationLanguages();

            var Language = DbCMS.codeNotificationLanguages.Where(l => l.NotificationLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbCMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeNotificationLanguagesRowVersion.SequenceEqual(dbModel.codeNotificationLanguagesRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }
        private bool ActiveNotificationLanguage(codeNotificationLanguages model)
        {
            int LanguageID = DbCMS.codeNotificationLanguages
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.NotificationGUID == model.NotificationGUID &&
                                              x.NotificationLanguageGUID != model.NotificationLanguageGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Job Title Name in selected language already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }
        #endregion
    }
}