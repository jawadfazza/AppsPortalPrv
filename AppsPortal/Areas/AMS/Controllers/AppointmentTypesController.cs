using AMS_DAL.Model;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.AMS.ViewModels;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.AMS.Controllers
{
    public class AppointmentTypesController : AMSBaseController
    {
        #region Appointment Types

        public ActionResult Index()
        {
            return View();
        }

        [Route("AMS/AppointmentTypes/")]
        public ActionResult AppointmentTypesIndex()
        {
            if (!CMS.HasAction(Permissions.AppointmentType.Access, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/AMS/Views/AppointmentTypes/Index.cshtml");
        }

        [Route("AMS/AppointmentTypesDataTable/")]
        public JsonResult AppointmentTypesDataTable(DataTableRecievedOptions options)
        {
            var app = DbAMS.codeAppointmentType.ToList();
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<AppointmentTypesDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<AppointmentTypesDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.AppointmentType.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();


            var All = (from a in DbAMS.codeAppointmentType.AsExpandable().Where(x => AuthorizedList.Contains(x.DepartmentGUID.ToString()))
                       join b in DbAMS.codeAppointmentTypeLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeAppointmentType.DeletedOn) && x.LanguageID == LAN) on a.AppointmentTypeGUID equals b.AppointmentTypeGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join c in DbAMS.codeDepartmentsLanguages.Where(x => x.LanguageID == LAN && x.codeDepartments.Active && x.Active) on a.DepartmentGUID equals c.DepartmentGUID into LJ2
                       from R2 in LJ2.DefaultIfEmpty()
                       select new AppointmentTypesDataTableModel
                       {
                           Code=a.Code,
                           AppointmentTypeGUID = a.AppointmentTypeGUID,
                           AppointmentTypeDescription = R1.AppointmentTypeDescription,
                           DepartmentDescription= R2.DepartmentDescription,
                           Active = a.Active,
                           codeAppointmentTypeRowVersion = a.codeAppointmentTypeRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<AppointmentTypesDataTableModel> Result = Mapper.Map<List<AppointmentTypesDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("AMS/AppointmentTypes/Create/")]
        public ActionResult AppointmentTypeCreate()
        {
            if (!CMS.HasAction(Permissions.AppointmentType.Create, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/AMS/Views/AppointmentTypes/AppointmentType.cshtml", new AppointmentTypeUpdateModel());
        }

        [Route("AMS/AppointmentTypes/Update/{PK}")]
        public ActionResult AppointmentTypeUpdate(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.AppointmentType.Access, Apps.AMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var model = (from a in DbAMS.codeAppointmentType.WherePK(PK)
                         join b in DbAMS.codeAppointmentTypeLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeAppointmentType.DeletedOn) && x.LanguageID == LAN)
                         on a.AppointmentTypeGUID equals b.AppointmentTypeGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new AppointmentTypeUpdateModel
                         {
                             AppointmentTypeGUID = a.AppointmentTypeGUID,
                             Code=a.Code,
                             DepartmentGUID=a.DepartmentGUID,
                             Sort=a.Sort,
                             AppointmentTypeDescription = R1.AppointmentTypeDescription,
                             Active = a.Active,
                             codeAppointmentTypeRowVersion = a.codeAppointmentTypeRowVersion,
                             codeAppointmentTypeLanguageRowVersion = R1.codeAppointmentTypeLanguageRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("AppointmentType", "AppointmentTypes", new { Area = "AMS" }));

            return View("~/Areas/AMS/Views/AppointmentTypes/AppointmentType.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AppointmentTypeCreate(AppointmentTypeUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.AppointmentType.Create, Apps.AMS,model.DepartmentGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveAppointmentType(model)) return PartialView("~/Areas/AMS/Views/AppointmentTypes/_AppointmentTypeForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeAppointmentType AppointmentType = Mapper.Map(model, new codeAppointmentType());
            AppointmentType.AppointmentTypeGUID = EntityPK;
            DbAMS.Create(AppointmentType, Permissions.AppointmentType.CreateGuid, ExecutionTime, DbCMS);

            codeAppointmentTypeLanguage Language = Mapper.Map(model, new codeAppointmentTypeLanguage());
            Language.AppointmentTypeGUID = EntityPK;

            DbAMS.Create(Language, Permissions.AppointmentType.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.AppointmentTypeLanguagesDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.AppointmentType.Create, Apps.AMS, new UrlHelper(Request.RequestContext).Action("Create", "AppointmentTypes", new { Area = "AMS" })), Container = "AppointmentTypeFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.AppointmentType.Update, Apps.AMS), Container = "AppointmentTypeFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.AppointmentType.Delete, Apps.AMS), Container = "AppointmentTypeFormControls" });

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.SingleCreateMessage(DbAMS.PrimaryKeyControl(AppointmentType), DbAMS.RowVersionControls(AppointmentType, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AppointmentTypeUpdate(AppointmentTypeUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.AppointmentType.Update, Apps.AMS, model.DepartmentGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveAppointmentType(model)) return PartialView("~/Areas/AMS/Views/AppointmentTypes/_AppointmentTypeForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeAppointmentType AppointmentType = Mapper.Map(model, new codeAppointmentType());
            DbAMS.Update(AppointmentType, Permissions.AppointmentType.UpdateGuid, ExecutionTime, DbCMS);

            var Language = DbAMS.codeAppointmentTypeLanguage.Where(l => l.AppointmentTypeGUID == model.AppointmentTypeGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.AppointmentTypeGUID = AppointmentType.AppointmentTypeGUID;
                DbAMS.Create(Language, Permissions.AppointmentType.CreateGuid, ExecutionTime, DbCMS);
            }
            else if (Language.AppointmentTypeDescription != model.AppointmentTypeDescription)
            {
                Language = Mapper.Map(model, Language);
                DbAMS.Update(Language, Permissions.AppointmentType.UpdateGuid, ExecutionTime, DbCMS);
            }

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.SingleUpdateMessage(null, null, DbAMS.RowVersionControls(AppointmentType, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyAppointmentType(model.AppointmentTypeGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AppointmentTypeDelete(codeAppointmentType model)
        {
            if (!CMS.HasAction(Permissions.AppointmentType.Delete, Apps.AMS,model.DepartmentGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeAppointmentType> DeletedAppointmentType = DeleteAppointmentTypes(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.AppointmentType.Restore, Apps.AMS), Container = "AppointmentTypeFormControls" });

            try
            {
                int CommitedRows = DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.SingleDeleteMessage(CommitedRows, DeletedAppointmentType.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyAppointmentType(model.AppointmentTypeGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AppointmentTypeRestore(codeAppointmentType model)
        {
            if (!CMS.HasAction(Permissions.AppointmentType.Restore, Apps.AMS, model.DepartmentGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveAppointmentType(model))
            {
                return Json(DbAMS.RecordExists());
            }

            List<codeAppointmentType> RestoredAppointmentTypes = RestoreAppointmentTypes(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.AppointmentType.Create, Apps.AMS, new UrlHelper(Request.RequestContext).Action("AppointmentTypeCreate", "Configuration", new { Area = "AMS" })), Container = "AppointmentTypeFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.AppointmentType.Update, Apps.AMS), Container = "AppointmentTypeFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.AppointmentType.Delete, Apps.AMS), Container = "AppointmentTypeFormControls" });

            try
            {
                int CommitedRows = DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.SingleRestoreMessage(CommitedRows, RestoredAppointmentTypes, DbAMS.PrimaryKeyControl(RestoredAppointmentTypes.FirstOrDefault()), Url.Action(DataTableNames.AppointmentTypeLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyAppointmentType(model.AppointmentTypeGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult AppointmentTypesDataTableDelete(List<codeAppointmentType> models)
        {
            if (!CMS.HasAction(Permissions.AppointmentType.Delete, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeAppointmentType> DeletedAppointmentTypes = DeleteAppointmentTypes(models);

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.PartialDeleteMessage(DeletedAppointmentTypes, models, DataTableNames.AppointmentTypesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult AppointmentTypesDataTableRestore(List<codeAppointmentType> models)
        {
            if (!CMS.HasAction(Permissions.AppointmentType.Restore, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeAppointmentType> RestoredAppointmentTypes = RestoreAppointmentTypes(models);

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.PartialRestoreMessage(RestoredAppointmentTypes, models, DataTableNames.AppointmentTypesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeAppointmentType> DeleteAppointmentTypes(List<codeAppointmentType> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<codeAppointmentType> DeletedAppointmentTypes = new List<codeAppointmentType>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "SELECT AppointmentTypeGUID,CONVERT(varchar(50), DepartmentGUID) as C2 ,codeAppointmentTypeRowVersion FROM code.codeAppointmentType where AppointmentTypeGUID in (" + string.Join(",", models.Select(x => "'" + x.AppointmentTypeGUID + "'").ToArray()) + ")";

            string query = DbAMS.QueryBuilder(models, Permissions.AppointmentType.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbAMS.Database.SqlQuery<codeAppointmentType>(query).ToList();
            foreach (var record in Records)
            {
                DeletedAppointmentTypes.Add(DbAMS.Delete(record, ExecutionTime, Permissions.AppointmentType.DeleteGuid, DbCMS));
            }

            var Languages = DeletedAppointmentTypes.SelectMany(a => a.codeAppointmentTypeLanguage).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbAMS.Delete(language, ExecutionTime, Permissions.AppointmentType.DeleteGuid, DbCMS);
            }
            return DeletedAppointmentTypes;
        }

        private List<codeAppointmentType> RestoreAppointmentTypes(List<codeAppointmentType> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<codeAppointmentType> RestoredAppointmentTypes = new List<codeAppointmentType>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "SELECT AppointmentTypeGUID,CONVERT(varchar(50), DepartmentGUID) as C2 ,codeAppointmentTypeRowVersion FROM code.codeAppointmentType where AppointmentTypeGUID in (" + string.Join(",", models.Select(x => "'" + x.AppointmentTypeGUID + "'").ToArray()) + ")";

            string query = DbAMS.QueryBuilder(models, Permissions.AppointmentType.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbAMS.Database.SqlQuery<codeAppointmentType>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveAppointmentType(record))
                {
                    RestoredAppointmentTypes.Add(DbAMS.Restore(record, Permissions.AppointmentType.DeleteGuid, Permissions.AppointmentType.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var Languages = RestoredAppointmentTypes.SelectMany(x => x.codeAppointmentTypeLanguage.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbAMS.Restore(language, Permissions.AppointmentType.DeleteGuid, Permissions.AppointmentType.RestoreGuid, RestoringTime, DbCMS);
            }

            return RestoredAppointmentTypes;
        }

        private JsonResult ConcurrencyAppointmentType(Guid PK)
        {
            AppointmentTypeUpdateModel dbModel = new AppointmentTypeUpdateModel();

            var AppointmentType = DbAMS.codeAppointmentType.Where(x => x.AppointmentTypeGUID == PK).FirstOrDefault();
            var dbAppointmentType = DbAMS.Entry(AppointmentType).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbAppointmentType, dbModel);

            var Language = DbAMS.codeAppointmentTypeLanguage.Where(x => x.AppointmentTypeGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeAppointmentType.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbAMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (AppointmentType.codeAppointmentTypeRowVersion.SequenceEqual(dbModel.codeAppointmentTypeRowVersion) && Language.codeAppointmentTypeLanguageRowVersion.SequenceEqual(dbModel.codeAppointmentTypeLanguageRowVersion))
            {
                return Json(DbAMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbAMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveAppointmentType(Object model)
        {
            codeAppointmentTypeLanguage AppointmentType = Mapper.Map(model, new codeAppointmentTypeLanguage());
            int AppointmentTypeDescription = DbAMS.codeAppointmentTypeLanguage
                                    .Where(x => x.AppointmentTypeDescription == AppointmentType.AppointmentTypeDescription &&
                                                x.AppointmentTypeGUID != AppointmentType.AppointmentTypeGUID &&
                                                x.Active).Count();
            if (AppointmentTypeDescription > 0)
            {
                ModelState.AddModelError("AppointmentTypeDescription", "AppointmentType is already exists");
            }
            return (AppointmentTypeDescription > 0);
        }

        #endregion

        #region Appointment Types Language

        //[Route("AMS/AppointmentTypeLanguagesDataTable/{PK}")]
        public ActionResult AppointmentTypeLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/AMS/Views/AppointmentTypes/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeAppointmentTypeLanguage, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeAppointmentTypeLanguage>(DataTable.Filters);
            }

            var Result = DbAMS.codeAppointmentTypeLanguage.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.AppointmentTypeGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.AppointmentTypeLanguageGUID,
                                  x.LanguageID,
                                  x.AppointmentTypeDescription,
                                  x.codeAppointmentTypeLanguageRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AppointmentTypeLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.AppointmentType.Create, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/AMS/Views/AppointmentTypes/_LanguageUpdateModal.cshtml",
                new codeAppointmentTypeLanguage { AppointmentTypeGUID = FK });
        }

        public ActionResult AppointmentTypeLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.AppointmentType.Access, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/AMS/Views/AppointmentTypes/_LanguageUpdateModal.cshtml", DbAMS.codeAppointmentTypeLanguage.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AppointmentTypeLanguageCreate(codeAppointmentTypeLanguage model)
        {
            if (!CMS.HasAction(Permissions.AppointmentType.Create, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveAppointmentTypeLanguage(model)) return PartialView("~/Areas/AMS/Views/AppointmentTypes/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbAMS.Create(model, Permissions.AppointmentType.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.SingleUpdateMessage(DataTableNames.AppointmentTypeLanguagesDataTable, DbAMS.PrimaryKeyControl(model), DbAMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AppointmentTypeLanguageUpdate(codeAppointmentTypeLanguage model)
        {
            if (!CMS.HasAction(Permissions.AppointmentType.Update, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveAppointmentTypeLanguage(model)) return PartialView("~/Areas/AMS/Views/AppointmentTypes/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbAMS.Update(model, Permissions.AppointmentType.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.SingleUpdateMessage(DataTableNames.AppointmentTypeLanguagesDataTable,
                    DbAMS.PrimaryKeyControl(model),
                    DbAMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyAppointmentTypeLanguage(model.AppointmentTypeLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AppointmentTypeLanguageDelete(codeAppointmentTypeLanguage model)
        {
            if (!CMS.HasAction(Permissions.AppointmentType.Delete, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeAppointmentTypeLanguage> DeletedLanguages = DeleteAppointmentTypeLanguages(new List<codeAppointmentTypeLanguage> { model });

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.AppointmentTypeLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyAppointmentTypeLanguage(model.AppointmentTypeLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AppointmentTypeLanguageRestore(codeAppointmentTypeLanguage model)
        {
            if (!CMS.HasAction(Permissions.AppointmentType.Restore, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveAppointmentTypeLanguage(model))
            {
                return Json(DbAMS.RecordExists());
            }

            List<codeAppointmentTypeLanguage> RestoredLanguages = RestoreAppointmentTypeLanguages(Portal.SingleToList(model));

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.AppointmentTypeLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyAppointmentTypeLanguage(model.AppointmentTypeLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult AppointmentTypeLanguagesDataTableDelete(List<codeAppointmentTypeLanguage> models)
        {
            if (!CMS.HasAction(Permissions.AppointmentType.Delete, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeAppointmentTypeLanguage> DeletedLanguages = DeleteAppointmentTypeLanguages(models);

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.AppointmentTypeLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult AppointmentTypeLanguagesDataTableRestore(List<codeAppointmentTypeLanguage> models)
        {
            if (!CMS.HasAction(Permissions.AppointmentType.Restore, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeAppointmentTypeLanguage> RestoredLanguages = RestoreAppointmentTypeLanguages(models);

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.AppointmentTypeLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeAppointmentTypeLanguage> DeleteAppointmentTypeLanguages(List<codeAppointmentTypeLanguage> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeAppointmentTypeLanguage> DeletedAppointmentTypeLanguages = new List<codeAppointmentTypeLanguage>();

            string query = DbAMS.QueryBuilder(models, Permissions.AppointmentType.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbAMS.Database.SqlQuery<codeAppointmentTypeLanguage>(query).ToList();

            foreach (var language in languages)
            {
                DeletedAppointmentTypeLanguages.Add(DbAMS.Delete(language, ExecutionTime, Permissions.AppointmentType.DeleteGuid, DbCMS));
            }

            return DeletedAppointmentTypeLanguages;
        }

        private List<codeAppointmentTypeLanguage> RestoreAppointmentTypeLanguages(List<codeAppointmentTypeLanguage> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<codeAppointmentTypeLanguage> RestoredLanguages = new List<codeAppointmentTypeLanguage>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbAMS.QueryBuilder(models, Permissions.AppointmentType.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbAMS.Database.SqlQuery<codeAppointmentTypeLanguage>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveAppointmentTypeLanguage(language))
                {
                    RestoredLanguages.Add(DbAMS.Restore(language, Permissions.AppointmentType.DeleteGuid, Permissions.AppointmentType.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyAppointmentTypeLanguage(Guid PK)
        {
            codeAppointmentTypeLanguage dbModel = new codeAppointmentTypeLanguage();

            var Language = DbAMS.codeAppointmentTypeLanguage.Where(l => l.AppointmentTypeLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbAMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeAppointmentTypeLanguageRowVersion.SequenceEqual(dbModel.codeAppointmentTypeLanguageRowVersion))
            {
                return Json(DbAMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbAMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveAppointmentTypeLanguage(codeAppointmentTypeLanguage model)
        {
            int LanguageID = DbAMS.codeAppointmentTypeLanguage
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.AppointmentTypeGUID == model.AppointmentTypeGUID &&
                                              x.AppointmentTypeLanguageGUID != model.AppointmentTypeLanguageGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Telecom Company Name in selected language already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }

        #endregion
    }
}