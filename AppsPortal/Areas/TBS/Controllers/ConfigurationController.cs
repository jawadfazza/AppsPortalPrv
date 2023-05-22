using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
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
using TBS_DAL.Model;

namespace AppsPortal.Areas.TBS.Controllers
{
    public class ConfigurationController : TBSBaseController
    {

        public ActionResult Index()
        {
            CMS.SetUserToken(UserProfileGUID, Apps.SRS);
            Session[SessionKeys.CurrentApp] = Apps.SRS;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }

        #region CODE XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX Amer
        #region Telecom Companies

        [Route("TBS/Configuration/TelecomCompanies/")]
        public ActionResult TelecomCompaniesIndex()
        {
            if (!CMS.HasAction(Permissions.TelecomCompanies.Access, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/TBS/Views/Configuration/TelecomCompany/Index.cshtml");
        }

        [Route("TBS/Configuration/TelecomCompaniesDataTable/")]
        public JsonResult TelecomCompaniesDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<TelecomCompaniesDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<TelecomCompaniesDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbTBS.codeTelecomCompany.AsExpandable()
                       join b in DbTBS.codeTelecomCompanyLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeTelecomCompany.DeletedOn) && x.LanguageID == LAN) on a.TelecomCompanyGUID equals b.TelecomCompanyGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       select new TelecomCompaniesDataTableModel
                       {
                           TelecomCompanyGUID = a.TelecomCompanyGUID,
                           TelecomCompanyAcronym = a.TelecomCompanyAcronym,
                           TelecomCompanyDescription = R1.TelecomCompanyDescription,
                           Active = a.Active,
                           codeTelecomCompanyRowVersion = a.codeTelecomCompanyRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<TelecomCompaniesDataTableModel> Result = Mapper.Map<List<TelecomCompaniesDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("TBS/Configuration/TelecomCompanies/Create/")]
        public ActionResult TelecomCompanyCreate()
        {
            if (!CMS.HasAction(Permissions.TelecomCompanies.Create, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/TBS/Views/Configuration/TelecomCompany/TelecomCompany.cshtml", new TelecomCompanyUpdateModel());
        }

        [Route("TBS/Configuration/TelecomCompanies/Update/{PK}")]
        public ActionResult TelecomCompanyUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.TelecomCompanies.Access, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbTBS.codeTelecomCompany.WherePK(PK)
                         join b in DbTBS.codeTelecomCompanyLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeTelecomCompany.DeletedOn) && x.LanguageID == LAN)
                         on a.TelecomCompanyGUID equals b.TelecomCompanyGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new TelecomCompanyUpdateModel
                         {
                             TelecomCompanyGUID = a.TelecomCompanyGUID,
                             TelecomCompanyAcronym = a.TelecomCompanyAcronym,
                             Active = a.Active,
                             codeTelecomCompanyRowVersion = a.codeTelecomCompanyRowVersion,
                             TelecomCompanyDescription = R1.TelecomCompanyDescription,
                             codeTelecomCompanyLanguagesRowVersion = R1.codeTelecomCompanyLanguagesRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("TelecomCompany", "Configuration", new { Area = "TBS" }));

            return View("~/Areas/TBS/Views/Configuration/TelecomCompany/TelecomCompany.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TelecomCompanyCreate(TelecomCompanyUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.TelecomCompanies.Create, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveTelecomCompany(model)) return PartialView("~/Areas/TBS/Views/Configuration/TelecomCompany/_TelecomCompanyForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeTelecomCompany TelecomCompany = Mapper.Map(model, new codeTelecomCompany());
            TelecomCompany.TelecomCompanyGUID = EntityPK;
            DbTBS.Create(TelecomCompany, Permissions.TelecomCompanies.CreateGuid, ExecutionTime, DbCMS);

            codeTelecomCompanyLanguages Language = Mapper.Map(model, new codeTelecomCompanyLanguages());
            Language.TelecomCompanyGUID = EntityPK;

            DbTBS.Create(Language, Permissions.TelecomCompaniesLanguages.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.TelecomCompanyLanguagesDataTable, ControllerContext, "LanguagesContainer"));
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.TelecomCompanyOperationsDataTable, ControllerContext, "OperationsContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.TelecomCompanies.Create, Apps.TBS, new UrlHelper(Request.RequestContext).Action("TelecomCompanyCreate", "Configuration", new { Area = "TBS" })), Container = "TelecomCompanyFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.TelecomCompanies.Update, Apps.TBS), Container = "TelecomCompanyFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.TelecomCompanies.Delete, Apps.TBS), Container = "TelecomCompanyFormControls" });

            try
            {
                DbTBS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTBS.SingleCreateMessage(DbTBS.PrimaryKeyControl(TelecomCompany), DbTBS.RowVersionControls(TelecomCompany, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TelecomCompanyUpdate(TelecomCompanyUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.TelecomCompanies.Update, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveTelecomCompany(model)) return PartialView("~/Areas/TBS/Views/Configuration/TelecomCompany/_TelecomCompanyForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeTelecomCompany TelecomCompany = Mapper.Map(model, new codeTelecomCompany());
            DbTBS.Update(TelecomCompany, Permissions.TelecomCompanies.UpdateGuid, ExecutionTime, DbCMS);

            var Language = DbTBS.codeTelecomCompanyLanguages.Where(l => l.TelecomCompanyGUID == model.TelecomCompanyGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.TelecomCompanyGUID = TelecomCompany.TelecomCompanyGUID;
                DbTBS.Create(Language, Permissions.TelecomCompaniesLanguages.CreateGuid, ExecutionTime, DbCMS);
            }
            else if (Language.TelecomCompanyDescription != model.TelecomCompanyDescription)
            {
                Language = Mapper.Map(model, Language);
                DbTBS.Update(Language, Permissions.TelecomCompaniesLanguages.UpdateGuid, ExecutionTime, DbCMS);
            }

            try
            {
                DbTBS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTBS.SingleUpdateMessage(null, null, DbTBS.RowVersionControls(TelecomCompany, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyTelecomCompany(model.TelecomCompanyGUID);
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TelecomCompanyDelete(codeTelecomCompany model)
        {
            if (!CMS.HasAction(Permissions.TelecomCompanies.Delete, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeTelecomCompany> DeletedTelecomCompany = DeleteTelecomCompanies(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.TelecomCompanies.Restore, Apps.TBS), Container = "TelecomCompanyFormControls" });


            try
            {
                int CommitedRows = DbTBS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTBS.SingleDeleteMessage(CommitedRows, DeletedTelecomCompany.FirstOrDefault(), "LanguagesContainer,OperationsContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyTelecomCompany(model.TelecomCompanyGUID);
            }
            catch (Exception ex)
            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TelecomCompanyRestore(codeTelecomCompany model)
        {
            if (!CMS.HasAction(Permissions.TelecomCompanies.Restore, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveTelecomCompany(model))
            {
                return Json(DbTBS.RecordExists());
            }

            List<codeTelecomCompany> RestoredTelecomCompanies = RestoreTelecomCompanies(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.TelecomCompanies.Create, Apps.TBS, new UrlHelper(Request.RequestContext).Action("TelecomCompanyCreate", "Configuration", new { Area = "TBS" })), Container = "TelecomCompanyFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.TelecomCompanies.Update, Apps.TBS), Container = "TelecomCompanyFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.TelecomCompanies.Delete, Apps.TBS), Container = "TelecomCompanyFormControls" });

            try
            {
                int CommitedRows = DbTBS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTBS.SingleRestoreMessage(CommitedRows, RestoredTelecomCompanies, DbTBS.PrimaryKeyControl(RestoredTelecomCompanies.FirstOrDefault()), Url.Action(DataTableNames.TelecomCompanyLanguagesDataTable, Portal.GetControllerName(ControllerContext)) + "," + Url.Action(DataTableNames.TelecomCompanyOperationsDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer,OperationsContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyTelecomCompany(model.TelecomCompanyGUID);
            }
            catch (Exception ex)
            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult TelecomCompaniesDataTableDelete(List<codeTelecomCompany> models)
        {
            if (!CMS.HasAction(Permissions.TelecomCompanies.Delete, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeTelecomCompany> DeletedTelecomCompanies = DeleteTelecomCompanies(models);

            try
            {
                DbTBS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTBS.PartialDeleteMessage(DeletedTelecomCompanies, models, DataTableNames.TelecomCompaniesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult TelecomCompaniesDataTableRestore(List<codeTelecomCompany> models)
        {
            if (!CMS.HasAction(Permissions.TelecomCompanies.Restore, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeTelecomCompany> RestoredTelecomCompanies = RestoreTelecomCompanies(models);

            try
            {
                DbTBS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTBS.PartialRestoreMessage(RestoredTelecomCompanies, models, DataTableNames.TelecomCompaniesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }

        private List<codeTelecomCompany> DeleteTelecomCompanies(List<codeTelecomCompany> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<codeTelecomCompany> DeletedTelecomCompanies = new List<codeTelecomCompany>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbTBS.QueryBuilder(models, Permissions.TelecomCompanies.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbTBS.Database.SqlQuery<codeTelecomCompany>(query).ToList();
            foreach (var record in Records)
            {
                DeletedTelecomCompanies.Add(DbTBS.Delete(record, ExecutionTime, Permissions.TelecomCompanies.DeleteGuid, DbCMS));
            }

            var Languages = DeletedTelecomCompanies.SelectMany(a => a.codeTelecomCompanyLanguages).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbTBS.Delete(language, ExecutionTime, Permissions.TelecomCompanies.DeleteGuid, DbCMS);
            }
            return DeletedTelecomCompanies;
        }

        private List<codeTelecomCompany> RestoreTelecomCompanies(List<codeTelecomCompany> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<codeTelecomCompany> RestoredTelecomCompanies = new List<codeTelecomCompany>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbTBS.QueryBuilder(models, Permissions.TelecomCompanies.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbTBS.Database.SqlQuery<codeTelecomCompany>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveTelecomCompany(record))
                {
                    RestoredTelecomCompanies.Add(DbTBS.Restore(record, Permissions.TelecomCompanies.DeleteGuid, Permissions.TelecomCompanies.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var Languages = RestoredTelecomCompanies.SelectMany(x => x.codeTelecomCompanyLanguages.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbTBS.Restore(language, Permissions.TelecomCompanies.DeleteGuid, Permissions.TelecomCompanies.RestoreGuid, RestoringTime, DbCMS);
            }

            return RestoredTelecomCompanies;
        }

        private JsonResult ConcurrencyTelecomCompany(Guid PK)
        {
            TelecomCompanyUpdateModel dbModel = new TelecomCompanyUpdateModel();

            var TelecomCompany = DbTBS.codeTelecomCompany.Where(x => x.TelecomCompanyGUID == PK).FirstOrDefault();
            var dbTelecomCompany = DbTBS.Entry(TelecomCompany).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbTelecomCompany, dbModel);

            var Language = DbTBS.codeTelecomCompanyLanguages.Where(x => x.TelecomCompanyGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeTelecomCompany.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbTBS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (TelecomCompany.codeTelecomCompanyRowVersion.SequenceEqual(dbModel.codeTelecomCompanyRowVersion) && Language.codeTelecomCompanyLanguagesRowVersion.SequenceEqual(dbModel.codeTelecomCompanyLanguagesRowVersion))
            {
                return Json(DbTBS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbTBS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveTelecomCompany(Object model)
        {
            codeTelecomCompanyLanguages TelecomCompany = Mapper.Map(model, new codeTelecomCompanyLanguages());
            int TelecomCompanyDescription = DbTBS.codeTelecomCompanyLanguages
                                    .Where(x => x.TelecomCompanyDescription == TelecomCompany.TelecomCompanyDescription &&
                                                x.TelecomCompanyGUID != TelecomCompany.TelecomCompanyGUID &&
                                                x.Active).Count();
            if (TelecomCompanyDescription > 0)
            {
                ModelState.AddModelError("TelecomCompanyDescription", "TelecomCompany is already exists");
            }
            return (TelecomCompanyDescription > 0);
        }

        #endregion

        #region Telecom Companies Language
        public ActionResult TelecomCompanyLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/TBS/Views/Configuration/TelecomCompany/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeTelecomCompanyLanguages, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeTelecomCompanyLanguages>(DataTable.Filters);
            }

            var Result = DbTBS.codeTelecomCompanyLanguages.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.TelecomCompanyGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.TelecomCompanyLanguageGUID,
                                  x.LanguageID,
                                  x.TelecomCompanyDescription,
                                  x.codeTelecomCompanyLanguagesRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult TelecomCompanyLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.TelecomCompanies.Create, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/TBS/Views/Configuration/TelecomCompany/_LanguageUpdateModal.cshtml",
                new codeTelecomCompanyLanguages { TelecomCompanyGUID = FK });
        }

        public ActionResult TelecomCompanyLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.TelecomCompanies.Access, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/TBS/Views/Configuration/TelecomCompany/_LanguageUpdateModal.cshtml", DbTBS.codeTelecomCompanyLanguages.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TelecomCompanyLanguageCreate(codeTelecomCompanyLanguages model)
        {
            if (!CMS.HasAction(Permissions.TelecomCompanies.Create, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveTelecomCompanyLanguage(model)) return PartialView("~/Areas/TBS/Views/Configuration/TelecomCompany/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbTBS.Create(model, Permissions.TelecomCompaniesLanguages.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbTBS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTBS.SingleUpdateMessage(DataTableNames.TelecomCompanyLanguagesDataTable, DbTBS.PrimaryKeyControl(model), DbTBS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TelecomCompanyLanguageUpdate(codeTelecomCompanyLanguages model)
        {
            if (!CMS.HasAction(Permissions.TelecomCompanies.Update, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveTelecomCompanyLanguage(model)) return PartialView("~/Areas/TBS/Views/Configuration/TelecomCompany/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbTBS.Update(model, Permissions.TelecomCompaniesLanguages.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbTBS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTBS.SingleUpdateMessage(DataTableNames.TelecomCompanyLanguagesDataTable,
                    DbTBS.PrimaryKeyControl(model),
                    DbTBS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyTelecomCompanyLanguage(model.TelecomCompanyLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TelecomCompanyLanguageDelete(codeTelecomCompanyLanguages model)
        {
            if (!CMS.HasAction(Permissions.TelecomCompanies.Delete, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeTelecomCompanyLanguages> DeletedLanguages = DeleteTelecomCompanyLanguages(new List<codeTelecomCompanyLanguages> { model });

            try
            {
                DbTBS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTBS.SingleDeleteMessage(DeletedLanguages, DataTableNames.TelecomCompanyLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyTelecomCompanyLanguage(model.TelecomCompanyLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TelecomCompanyLanguageRestore(codeTelecomCompanyLanguages model)
        {
            if (!CMS.HasAction(Permissions.TelecomCompanies.Restore, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveTelecomCompanyLanguage(model))
            {
                return Json(DbTBS.RecordExists());
            }

            List<codeTelecomCompanyLanguages> RestoredLanguages = RestoreTelecomCompanyLanguages(Portal.SingleToList(model));

            try
            {
                DbTBS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTBS.SingleRestoreMessage(RestoredLanguages, DataTableNames.TelecomCompanyLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyTelecomCompanyLanguage(model.TelecomCompanyLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult TelecomCompanyLanguagesDataTableDelete(List<codeTelecomCompanyLanguages> models)
        {
            if (!CMS.HasAction(Permissions.TelecomCompanies.Delete, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeTelecomCompanyLanguages> DeletedLanguages = DeleteTelecomCompanyLanguages(models);

            try
            {
                DbTBS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTBS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.TelecomCompanyLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult TelecomCompanyLanguagesDataTableRestore(List<codeTelecomCompanyLanguages> models)
        {
            if (!CMS.HasAction(Permissions.TelecomCompanies.Restore, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeTelecomCompanyLanguages> RestoredLanguages = RestoreTelecomCompanyLanguages(models);

            try
            {
                DbTBS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTBS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.TelecomCompanyLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }

        private List<codeTelecomCompanyLanguages> DeleteTelecomCompanyLanguages(List<codeTelecomCompanyLanguages> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeTelecomCompanyLanguages> DeletedTelecomCompanyLanguages = new List<codeTelecomCompanyLanguages>();

            string query = DbTBS.QueryBuilder(models, Permissions.TelecomCompaniesLanguages.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbTBS.Database.SqlQuery<codeTelecomCompanyLanguages>(query).ToList();

            foreach (var language in languages)
            {
                DeletedTelecomCompanyLanguages.Add(DbTBS.Delete(language, ExecutionTime, Permissions.TelecomCompaniesLanguages.DeleteGuid, DbCMS));
            }

            return DeletedTelecomCompanyLanguages;
        }

        private List<codeTelecomCompanyLanguages> RestoreTelecomCompanyLanguages(List<codeTelecomCompanyLanguages> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<codeTelecomCompanyLanguages> RestoredLanguages = new List<codeTelecomCompanyLanguages>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbTBS.QueryBuilder(models, Permissions.TelecomCompaniesLanguages.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbTBS.Database.SqlQuery<codeTelecomCompanyLanguages>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveTelecomCompanyLanguage(language))
                {
                    RestoredLanguages.Add(DbTBS.Restore(language, Permissions.TelecomCompaniesLanguages.DeleteGuid, Permissions.TelecomCompaniesLanguages.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyTelecomCompanyLanguage(Guid PK)
        {
            codeTelecomCompanyLanguages dbModel = new codeTelecomCompanyLanguages();

            var Language = DbTBS.codeTelecomCompanyLanguages.Where(l => l.TelecomCompanyLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbTBS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeTelecomCompanyLanguagesRowVersion.SequenceEqual(dbModel.codeTelecomCompanyLanguagesRowVersion))
            {
                return Json(DbTBS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbTBS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveTelecomCompanyLanguage(codeTelecomCompanyLanguages model)
        {
            int LanguageID = DbTBS.codeTelecomCompanyLanguages
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.TelecomCompanyGUID == model.TelecomCompanyGUID &&
                                              x.TelecomCompanyLanguageGUID != model.TelecomCompanyLanguageGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Telecom Company Name in selected language already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }

        #endregion
        #endregion

        #region Telecom Companies Operation
        public ActionResult TelecomCompanyOperationsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/TBS/Views/Configuration/TelecomCompany/_OperationsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<TelecomCompanyOperationsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<TelecomCompanyOperationsDataTableModel>(DataTable.Filters);
            }
            var Result = (from a in DbTBS.codeTelecomCompanyOperation.AsNoTracking().AsExpandable().Where(x => x.TelecomCompanyGUID == PK)
                          join b in DbTBS.codeTelecomCompanyLanguages.AsNoTracking().AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.TelecomCompanyGUID equals b.TelecomCompanyGUID
                          join c in DbTBS.codeOperationsLanguages.AsNoTracking().AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.OperationGUID equals c.OperationGUID
                          select new TelecomCompanyOperationsDataTableModel
                          {
                              TelecomCompanyOperationGUID = a.TelecomCompanyOperationGUID,
                              TelecomCompanyDescription = b.TelecomCompanyDescription,
                              OperationDescription = c.OperationDescription,
                              codeTelecomCompanyOperationRowVersion = a.codeTelecomCompanyOperationRowVersion,
                              Active = a.Active,
                          }).Where(Predicate);

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }


        public ActionResult TelecomCompanyOperationCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.TelecomCompanies.Create, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            var Operations = (from a in DbTBS.codeOperationsLanguages.Where(x => x.Active && x.LanguageID == LAN)
                              where !DbTBS.codeTelecomCompanyOperation.Where(x => x.TelecomCompanyGUID == FK).Select(x => x.OperationGUID).Contains(a.OperationGUID)
                              orderby a.OperationDescription
                              select new CheckBoxList
                              {
                                  Value = a.OperationGUID.ToString(),
                                  Description = a.OperationDescription,
                                  SearchKey = a.OperationDescription
                              }).ToList();

            ConfigurationModel ConfigurationModel = new ConfigurationModel
            {
                ValueGuid = FK,
                CheckBoxList = Operations,
            };
            return PartialView("~/Areas/TBS/Views/Configuration/TelecomCompany/_OperationUpdateModal.cshtml", ConfigurationModel);
        }

        public ActionResult TelecomCompanyOperationUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.TelecomCompanies.Access, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/TBS/Views/Configuration/TelecomCompany/_OperationUpdateModal.cshtml", DbTBS.codeTelecomCompanyOperation.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TelecomCompanyOperationCreate(ConfigurationModel model)
        {
            if (!CMS.HasAction(Permissions.TelecomCompanies.Create, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/TBS/Views/Configuration/TelecomCompany/_OperationUpdateModel.cshtml", model);



            //Guid ActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;

            List<string> mdList = model.CheckBoxList.Where(x => x.Checked == true).Select(x => x.Value).ToList();
            List<codeTelecomCompanyOperation> dbList = DbTBS.codeTelecomCompanyOperation.Where(s => s.TelecomCompanyGUID == model.ValueGuid && s.Active).ToList();

            var toAddList = mdList.Where(s => !dbList.Select(x => x.OperationGUID.ToString()).Contains(s)).Select(s => new codeTelecomCompanyOperation
            {
                TelecomCompanyOperationGUID = Guid.NewGuid(),
                TelecomCompanyGUID = model.ValueGuid,
                OperationGUID = Guid.Parse(s),
            }).ToList();

            DbTBS.CreateBulk(toAddList, Permissions.TelecomCompanies.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbTBS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTBS.SingleUpdateMessage(DataTableNames.TelecomCompanyOperationsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
            }
            catch (Exception ex)
            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }

        //[HttpPost, ValidateAntiForgeryToken]
        //public ActionResult TelecomCompanyOperationUpdate(dataTelecomCompanyOperations model)
        //{
        //    if (!CMS.HasAction(Permissions.TelecomCompanies.Update, Apps.TBS))
        //    {
        //        throw new HttpException(401, "Unauthorized access");
        //    }
        //    if (!ModelState.IsValid || ActiveTelecomCompanyOperation(model)) return PartialView("~/Areas/TBS/Views/Configuration/TelecomCompany/_OperationUpdateModal.cshtml", model);

        //    DateTime ExecutionTime = DateTime.Now;

        //    DbTBS.Update(model, Permissions.TelecomCompaniesLanguages.UpdateGuid, ExecutionTime, DbCMS);

        //    try
        //    {
        //        DbTBS.SaveChanges();
        //        DbCMS.SaveChanges();
        //        return Json(DbTBS.SingleUpdateMessage(DataTableNames.TelecomCompanyOperationsDataTable,
        //            DbTBS.PrimaryKeyControl(model),
        //            DbTBS.RowVersionControls(Portal.SingleToList(model))));
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        return ConcrrencyTelecomCompanyOperation(model.TelecomCompanyOperationGUID);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(DbTBS.ErrorMessage(ex.Message));
        //    }
        //}

        //[HttpPost, ValidateAntiForgeryToken]
        //public ActionResult TelecomCompanyOperationDelete(dataTelecomCompanyOperations model)
        //{
        //    if (!CMS.HasAction(Permissions.TelecomCompanies.Delete, Apps.TBS))
        //    {
        //        throw new HttpException(401, "Unauthorized access");
        //    }
        //    List<dataTelecomCompanyOperations> DeletedOperations = DeleteTelecomCompanyOperations(new List<dataTelecomCompanyOperations> { model });

        //    try
        //    {
        //        DbTBS.SaveChanges();
        //        DbCMS.SaveChanges();
        //        return Json(DbTBS.SingleDeleteMessage(DeletedOperations, DataTableNames.TelecomCompanyOperationsDataTable));
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        return ConcrrencyTelecomCompanyOperation(model.TelecomCompanyOperationGUID);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(DbTBS.ErrorMessage(ex.Message));
        //    }
        //}

        //[HttpPost, ValidateAntiForgeryToken]
        //public ActionResult TelecomCompanyOperationRestore(dataTelecomCompanyOperations model)
        //{
        //    if (!CMS.HasAction(Permissions.TelecomCompanies.Restore, Apps.TBS))
        //    {
        //        throw new HttpException(401, "Unauthorized access");
        //    }
        //    if (ActiveTelecomCompanyOperation(model))
        //    {
        //        return Json(DbTBS.RecordExists());
        //    }

        //    List<dataTelecomCompanyOperations> RestoredOperations = RestoreTelecomCompanyOperations(Portal.SingleToList(model));

        //    try
        //    {
        //        DbTBS.SaveChanges();
        //        DbCMS.SaveChanges();
        //        return Json(DbTBS.SingleRestoreMessage(RestoredOperations, DataTableNames.TelecomCompanyOperationsDataTable));
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        return ConcrrencyTelecomCompanyLanguage(model.TelecomCompanyOperationGUID);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(DbTBS.ErrorMessage(ex.Message));
        //    }
        //}

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult TelecomCompanyOperationsDataTableDelete(List<codeTelecomCompanyOperation> models)
        {
            if (!CMS.HasAction(Permissions.TelecomCompanies.Delete, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            DateTime ExecutionTime = DateTime.Now;
            Guid DeleteActionGUID = Permissions.TelecomCompanies.DeleteGuid;
            List<codeTelecomCompanyOperation> DeletedTelecomCompanyOperations = new List<codeTelecomCompanyOperation>();
            List<configTelecomCompanyOperation> DeletedConfiguration = new List<configTelecomCompanyOperation>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";
            //Permissions.TelecomCompanies.RemoveGUID should be
            string query = DbTBS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Delete, baseQuery);

            var Records = DbTBS.Database.SqlQuery<codeTelecomCompanyOperation>(query).ToList();
            foreach (var record in Records)
            {
                DeletedTelecomCompanyOperations.Add(DbTBS.Delete(record, ExecutionTime, Permissions.TelecomCompanies.DeleteGuid, DbCMS));
            }


            var Configurations = DeletedTelecomCompanyOperations.SelectMany(a => a.configTelecomCompanyOperation).Where(l => l.Active).ToList();
            foreach (var configuration in Configurations)
            {
                DeletedConfiguration.Add(DbTBS.Delete(configuration, ExecutionTime, DeleteActionGUID, DbCMS));
            }

            var MobilelineConfigurations = DeletedConfiguration.SelectMany(a => a.configTelecomCompanyOperationMobileColumn).Where(l => l.Active).ToList();
            foreach (var mobilelineConfiguration in MobilelineConfigurations)
            {
                DbTBS.Delete(mobilelineConfiguration, ExecutionTime, DeleteActionGUID, DbCMS);
            }

            var LandlineConfigurations = DeletedConfiguration.SelectMany(a => a.configTelecomCompanyOperationLandColumn).Where(l => l.Active).ToList();
            foreach (var landlineConfiguration in LandlineConfigurations)
            {
                DbTBS.Delete(landlineConfiguration, ExecutionTime, DeleteActionGUID, DbCMS);
            }
            try
            {
                DbTBS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTBS.PartialDeleteMessage(DeletedTelecomCompanyOperations, models, DataTableNames.TelecomCompanyOperationsDataTable));
            }
            catch (Exception ex)

            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult TelecomCompanyOperationsDataTableRestore(List<codeTelecomCompanyOperation> models)
        {
            if (!CMS.HasAction(Permissions.TelecomCompanies.Restore, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            Guid DeleteActionGUID = Permissions.TelecomCompanies.DeleteGuid;
            Guid RestoreActionGUID = Permissions.TelecomCompanies.RestoreGuid;
            DateTime RestoringTime = DateTime.Now;

            List<codeTelecomCompanyOperation> RestoredTelecomCompanyOperations = new List<codeTelecomCompanyOperation>();
            List<configTelecomCompanyOperation> RestoredConfig = new List<configTelecomCompanyOperation>();

            string query = DbTBS.QueryBuilder(models, Permissions.TelecomCompanies.DeleteGuid, SubmitTypes.Restore, "");

            var Records = DbTBS.Database.SqlQuery<codeTelecomCompanyOperation>(query).ToList();

            foreach (var record in Records)
            {
                RestoredTelecomCompanyOperations.Add(DbTBS.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS));
            }

            var Configurations = RestoredTelecomCompanyOperations.SelectMany(x => x.configTelecomCompanyOperation.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var configuration in Configurations)
            {

                RestoredConfig.Add(DbTBS.Restore(configuration, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS));
            }


            var MobilelineConfigurations = RestoredConfig.SelectMany(x => x.configTelecomCompanyOperationMobileColumn.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var mobilelineConfiguration in MobilelineConfigurations)
            {

                DbTBS.Restore(mobilelineConfiguration, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS);
            }

            var LandlineConfigurations = RestoredConfig.SelectMany(x => x.configTelecomCompanyOperationLandColumn.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var landlineConfiguration in LandlineConfigurations)
            {

                DbTBS.Restore(landlineConfiguration, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS);
            }

            try
            {
                DbTBS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTBS.PartialRestoreMessage(RestoredTelecomCompanyOperations, models, DataTableNames.TelecomCompanyOperationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }

        private bool ActiveTelecomCompanyOperation(codeTelecomCompanyOperation model)
        {
            return false;
        }

        private JsonResult ConcrrencyTelecomCompanyOperation(Guid PK)
        {
            codeTelecomCompanyOperation dbModel = new codeTelecomCompanyOperation();

            var Language = DbTBS.codeTelecomCompanyOperation.Where(l => l.OperationGUID == PK).FirstOrDefault();
            var dbLanguage = DbTBS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeTelecomCompanyOperationRowVersion.SequenceEqual(dbModel.codeTelecomCompanyOperationRowVersion))
            {
                return Json(DbTBS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbTBS, dbModel, "OperationsContainer"));
        }

        #endregion

        #region Telecom Companies Operation Configuration
        public ActionResult TelecomCompanyOperationConfigurationUpdate(Guid PK)
        {
            TelecomCompanyOperationConfigUpdateModel model = new TelecomCompanyOperationConfigUpdateModel();

            model.MobileConfigModel = new MobileConfigModel();
            model.MobileConfigModel.configTelecomCompanyOperation = new configTelecomCompanyOperation();
            model.MobileConfigModel.configTelecomCompanyOperationMobileColumns = new configTelecomCompanyOperationMobileColumn();


            model.LandLineConfigModel = new LandLineConfigModel();
            model.LandLineConfigModel.configTelecomCompanyOperation = new configTelecomCompanyOperation();
            model.LandLineConfigModel.configTelecomCompanyOperationLandColumns = new configTelecomCompanyOperationLandColumn();

            model.MobileConfigModel.configTelecomCompanyOperation.TelecomCompanyOperationGUID = PK;
            model.MobileConfigModel.configTelecomCompanyOperation.ConfigType = BillTypes.MobileLine;

            model.LandLineConfigModel.configTelecomCompanyOperation.TelecomCompanyOperationGUID = PK;
            model.LandLineConfigModel.configTelecomCompanyOperation.ConfigType = BillTypes.LandLine;

            var test = (from a in DbTBS.configTelecomCompanyOperation.Where(x => x.TelecomCompanyOperationGUID == PK && x.Active) select a).ToList();
            if (test.Count == 0)
            {
                return PartialView("~/Areas/TBS/Views/Configuration/TelecomCompany/_TelecomCompanyOperationConfigurationUpdateModal.cshtml", model);
            }

            model.MobileConfigModel.configTelecomCompanyOperation = Mapper.Map(test.FirstOrDefault(), model.MobileConfigModel.configTelecomCompanyOperation);
            model.LandLineConfigModel.configTelecomCompanyOperation = Mapper.Map(test.FirstOrDefault(), model.LandLineConfigModel.configTelecomCompanyOperation);
            model.MobileConfigModel.configTelecomCompanyOperationMobileColumns = Mapper.Map(test.FirstOrDefault().configTelecomCompanyOperationMobileColumn.FirstOrDefault(), model.MobileConfigModel.configTelecomCompanyOperationMobileColumns);
            model.LandLineConfigModel.configTelecomCompanyOperationLandColumns = Mapper.Map(test.FirstOrDefault().configTelecomCompanyOperationLandColumn.FirstOrDefault(), model.LandLineConfigModel.configTelecomCompanyOperationLandColumns);

            //foreach (var item in test)
            //{
            //    if (item.ConfigType == BillTypes.MobileLine) //Mobile Config
            //    {
            //        model.MobileConfigModel.configTelecomCompanyOperation = Mapper.Map(item, model.MobileConfigModel.configTelecomCompanyOperation);
            //        if (item.configTelecomCompanyOperationMobileColumn != null)
            //        {
            //            model.MobileConfigModel.configTelecomCompanyOperationMobileColumns = Mapper.Map(item.configTelecomCompanyOperationMobileColumn.FirstOrDefault(), model.MobileConfigModel.configTelecomCompanyOperationMobileColumns);
            //        }
            //        else
            //        {
            //            model.MobileConfigModel.configTelecomCompanyOperationMobileColumns = new configTelecomCompanyOperationMobileColumn();
            //        }

            //    }
            //    else if (item.ConfigType == BillTypes.LandLine) //Landline Config
            //    {
            //        model.LandLineConfigModel.configTelecomCompanyOperation = Mapper.Map(item, model.LandLineConfigModel.configTelecomCompanyOperation);
            //        if (item.configTelecomCompanyOperationLandColumn != null)
            //        {
            //            model.LandLineConfigModel.configTelecomCompanyOperationLandColumns = Mapper.Map(item.configTelecomCompanyOperationLandColumn, model.LandLineConfigModel.configTelecomCompanyOperationLandColumns);
            //        }
            //        else
            //        {
            //            model.LandLineConfigModel.configTelecomCompanyOperationLandColumns = new configTelecomCompanyOperationLandColumn();
            //        }
            //    }
            //}

            return PartialView("~/Areas/TBS/Views/Configuration/TelecomCompany/_TelecomCompanyOperationConfigurationUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TelecomCompanyOperationConfigurationUpdate(TelecomCompanyOperationConfigUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.TelecomCompanies.Create, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            DateTime ExecutionTime = DateTime.Now;

            configTelecomCompanyOperation configTelecomCompanyOperation = Mapper.Map(model.MobileConfigModel.configTelecomCompanyOperation, new configTelecomCompanyOperation());
            configTelecomCompanyOperationMobileColumn configTelecomCompanyOperationMobileColumns = Mapper.Map(model.MobileConfigModel.configTelecomCompanyOperationMobileColumns, new configTelecomCompanyOperationMobileColumn());
            configTelecomCompanyOperationLandColumn configTelecomCompanyOperationLandColumns = Mapper.Map(model.LandLineConfigModel.configTelecomCompanyOperationLandColumns, new configTelecomCompanyOperationLandColumn());

            Guid EntityPK = Guid.NewGuid();

            if (model.MobileConfigModel.configTelecomCompanyOperation.TelecomCompanyOperationConfigGUID == Guid.Empty)
            {
                configTelecomCompanyOperation.TelecomCompanyOperationConfigGUID = EntityPK;
                DbTBS.Create(configTelecomCompanyOperation, Permissions.TelecomCompanies.CreateGuid, ExecutionTime, DbCMS);
            }
            else
            {
                DbTBS.Update(configTelecomCompanyOperation, Permissions.TelecomCompanies.CreateGuid, ExecutionTime, DbCMS);
            }

            if (model.MobileConfigModel.configTelecomCompanyOperationMobileColumns.TelecomCompanyOperationFileColumnGUID == Guid.Empty)
            {
                configTelecomCompanyOperationMobileColumns.TelecomCompanyOperationConfigGUID = configTelecomCompanyOperation.TelecomCompanyOperationConfigGUID;
                DbTBS.Create(configTelecomCompanyOperationMobileColumns, Permissions.TelecomCompanies.CreateGuid, ExecutionTime, DbCMS);
            }
            else
            {
                DbTBS.Update(configTelecomCompanyOperationMobileColumns, Permissions.TelecomCompanies.CreateGuid, ExecutionTime, DbCMS);
            }

            if (model.LandLineConfigModel.configTelecomCompanyOperationLandColumns.TelecomCompanyOperationFileColumnGUID == Guid.Empty)
            {
                configTelecomCompanyOperationLandColumns.TelecomCompanyOperationConfigGUID = configTelecomCompanyOperation.TelecomCompanyOperationConfigGUID;
                DbTBS.Create(configTelecomCompanyOperationLandColumns, Permissions.TelecomCompanies.CreateGuid, ExecutionTime, DbCMS);
            }
            else
            {
                DbTBS.Update(configTelecomCompanyOperationLandColumns, Permissions.TelecomCompanies.CreateGuid, ExecutionTime, DbCMS);
            }

            try
            {
                DbTBS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTBS.SingleCreateMessage(DataTableNames.TelecomCompanyOperationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }


        public ActionResult TelecomCompanyOperationMobileConfigurationUpdate(Guid PK)
        {
            Guid TelecomCompanyOperationGUID = PK;


            TelecomCompanyOperationMobileConfigUpdateModel model = new TelecomCompanyOperationMobileConfigUpdateModel();
            model.configTelecomCompanyOperation = new configTelecomCompanyOperation();
            model.configTelecomCompanyOperationMobileColumns = new configTelecomCompanyOperationMobileColumn();

            model.configTelecomCompanyOperation.TelecomCompanyOperationGUID = TelecomCompanyOperationGUID;
            model.configTelecomCompanyOperation.ConfigType = BillTypes.MobileLine;


            configTelecomCompanyOperation configTelecomCompanyOperation = (from a in DbTBS.configTelecomCompanyOperation.Where(x => x.TelecomCompanyOperationGUID == PK && x.ConfigType == BillTypes.MobileLine && x.Active) select a).FirstOrDefault();

            if (configTelecomCompanyOperation != null)
            {
                model.configTelecomCompanyOperation = Mapper.Map(configTelecomCompanyOperation, model.configTelecomCompanyOperation);
                model.configTelecomCompanyOperationMobileColumns = Mapper.Map(configTelecomCompanyOperation.configTelecomCompanyOperationMobileColumn.FirstOrDefault(), model.configTelecomCompanyOperationMobileColumns);
            }
            return PartialView("~/Areas/TBS/Views/Configuration/TelecomCompany/_TelecomCompanyOperationMobileConfigUpdateModal.cshtml", model);


        }

        public ActionResult TelecomCompanyOperationLandlineConfigurationUpdate(Guid PK)
        {
            Guid TelecomCompanyOperationGUID = PK;


            TelecomCompanyOperationLandlineConfigUpdateModel model = new TelecomCompanyOperationLandlineConfigUpdateModel();
            model.configTelecomCompanyOperation = new configTelecomCompanyOperation();
            model.configTelecomCompanyOperationLandColumns = new configTelecomCompanyOperationLandColumn();

            model.configTelecomCompanyOperation.TelecomCompanyOperationGUID = TelecomCompanyOperationGUID;
            model.configTelecomCompanyOperation.ConfigType = BillTypes.LandLine;


            configTelecomCompanyOperation configTelecomCompanyOperation = (from a in DbTBS.configTelecomCompanyOperation.Where(x => x.TelecomCompanyOperationGUID == PK && x.ConfigType == BillTypes.LandLine && x.Active) select a).FirstOrDefault();

            if (configTelecomCompanyOperation != null)
            {
                model.configTelecomCompanyOperation = Mapper.Map(configTelecomCompanyOperation, model.configTelecomCompanyOperation);
                model.configTelecomCompanyOperationLandColumns = Mapper.Map(configTelecomCompanyOperation.configTelecomCompanyOperationLandColumn.FirstOrDefault(), model.configTelecomCompanyOperationLandColumns);
            }
            return PartialView("~/Areas/TBS/Views/Configuration/TelecomCompany/_TelecomCompanyOperationLandlineConfigUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TelecomCompanyOperationMobileConfigurationUpdate(TelecomCompanyOperationMobileConfigUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.TelecomCompanies.Create, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            DateTime ExecutionTime = DateTime.Now;

            configTelecomCompanyOperation configTelecomCompanyOperation = Mapper.Map(model.configTelecomCompanyOperation, new configTelecomCompanyOperation());
            configTelecomCompanyOperationMobileColumn configTelecomCompanyOperationMobileColumns = Mapper.Map(model.configTelecomCompanyOperationMobileColumns, new configTelecomCompanyOperationMobileColumn());


            Guid EntityPK = Guid.NewGuid();

            if (model.configTelecomCompanyOperation.TelecomCompanyOperationConfigGUID == Guid.Empty)
            {
                configTelecomCompanyOperation.TelecomCompanyOperationConfigGUID = EntityPK;
                DbTBS.Create(configTelecomCompanyOperation, Permissions.TelecomCompanies.CreateGuid, ExecutionTime, DbCMS);
            }
            else
            {
                DbTBS.Update(configTelecomCompanyOperation, Permissions.TelecomCompanies.CreateGuid, ExecutionTime, DbCMS);
            }

            if (model.configTelecomCompanyOperationMobileColumns.TelecomCompanyOperationFileColumnGUID == Guid.Empty)
            {
                configTelecomCompanyOperationMobileColumns.TelecomCompanyOperationConfigGUID = configTelecomCompanyOperation.TelecomCompanyOperationConfigGUID;
                DbTBS.Create(configTelecomCompanyOperationMobileColumns, Permissions.TelecomCompanies.CreateGuid, ExecutionTime, DbCMS);
            }
            else
            {
                DbTBS.Update(configTelecomCompanyOperationMobileColumns, Permissions.TelecomCompanies.CreateGuid, ExecutionTime, DbCMS);
            }

            try
            {
                DbTBS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTBS.SingleCreateMessage(DataTableNames.TelecomCompanyOperationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TelecomCompanyOperationLandlineConfigurationUpdate(TelecomCompanyOperationLandlineConfigUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.TelecomCompanies.Create, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            DateTime ExecutionTime = DateTime.Now;

            configTelecomCompanyOperation configTelecomCompanyOperation = Mapper.Map(model.configTelecomCompanyOperation, new configTelecomCompanyOperation());
            configTelecomCompanyOperationLandColumn configTelecomCompanyOperationLandColumns = Mapper.Map(model.configTelecomCompanyOperationLandColumns, new configTelecomCompanyOperationLandColumn());

            Guid EntityPK = Guid.NewGuid();

            if (model.configTelecomCompanyOperation.TelecomCompanyOperationConfigGUID == Guid.Empty)
            {
                configTelecomCompanyOperation.TelecomCompanyOperationConfigGUID = EntityPK;
                DbTBS.Create(configTelecomCompanyOperation, Permissions.TelecomCompanies.CreateGuid, ExecutionTime, DbCMS);
            }
            else
            {
                DbTBS.Update(configTelecomCompanyOperation, Permissions.TelecomCompanies.CreateGuid, ExecutionTime, DbCMS);
            }

            if (model.configTelecomCompanyOperationLandColumns.TelecomCompanyOperationFileColumnGUID == Guid.Empty)
            {
                configTelecomCompanyOperationLandColumns.TelecomCompanyOperationConfigGUID = configTelecomCompanyOperation.TelecomCompanyOperationConfigGUID;
                DbTBS.Create(configTelecomCompanyOperationLandColumns, Permissions.TelecomCompanies.CreateGuid, ExecutionTime, DbCMS);
            }
            else
            {
                DbTBS.Update(configTelecomCompanyOperationLandColumns, Permissions.TelecomCompanies.CreateGuid, ExecutionTime, DbCMS);
            }

            try
            {
                DbTBS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTBS.SingleCreateMessage(DataTableNames.TelecomCompanyOperationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }

        #endregion

        #region Staff Phones
        [Route("TBS/Configuration/StaffPhones/")]
        public ActionResult StaffPhonesIndex()
        {
            if (!CMS.HasAction(Permissions.StaffPhones.Access, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/TBS/Views/Configuration/StaffPhones/Index.cshtml");
        }

        [Route("TBS/Configuration/StaffPhonesDataTable/")]
        public ActionResult StaffPhonesDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<StaffPhonesDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<StaffPhonesDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbTBS.dataStaffPhone.AsExpandable().Where(x => x.Active)
                       join b in DbTBS.codeTelecomCompanyOperation.AsExpandable().Where(x => x.Active) on a.TelecomCompanyOperationGUID equals b.TelecomCompanyOperationGUID
                       join c in DbTBS.codeTelecomCompanyLanguages.AsExpandable().Where(x => x.LanguageID == LAN && x.Active) on b.TelecomCompanyGUID equals c.TelecomCompanyGUID
                       join e in DbTBS.userPersonalDetailsLanguage.AsExpandable().Where(x => x.LanguageID == LAN && x.Active) on a.UserGUID equals e.UserGUID
                       join f in DbTBS.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.PhoneTypeGUID equals f.ValueGUID
                       join j in DbTBS.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.PhoneUsageTypeGUID equals j.ValueGUID
                       select new StaffPhonesDataTableModel
                       {
                           StaffSimGUID = a.StaffSimGUID,
                           StaffName = e.FirstName + " " + e.Surname,
                           PhoneNumber = a.PhoneNumber,
                           PhoneTypeDescription = f.ValueDescription,
                           PhoneTypeUsageDescription = j.ValueDescription,
                           DepartmentDescription = (from aa in DbTBS.userServiceHistory.Where(x => x.Active && x.UserGUID == a.UserGUID)
                                                    join bb in DbTBS.userProfiles.Where(x => x.Active) on aa.ServiceHistoryGUID equals bb.ServiceHistoryGUID
                                                    join cc in DbTBS.codeDepartmentsLanguages.Where(x => x.Active) on bb.DepartmentGUID equals cc.DepartmentGUID
                                                    join dd in DbTBS.codeDutyStationsLanguages.Where(x => x.Active) on bb.DutyStationGUID equals dd.DutyStationGUID
                                                    orderby bb.FromDate descending
                                                    select cc.DepartmentDescription).FirstOrDefault(),
                           DutyStationDescription = (from aa in DbTBS.userServiceHistory.Where(x => x.Active && x.UserGUID == a.UserGUID)
                                                     join bb in DbTBS.userProfiles.Where(x => x.Active) on aa.ServiceHistoryGUID equals bb.ServiceHistoryGUID
                                                     join cc in DbTBS.codeDepartmentsLanguages.Where(x => x.Active) on bb.DepartmentGUID equals cc.DepartmentGUID
                                                     join dd in DbTBS.codeDutyStationsLanguages.Where(x => x.Active) on bb.DutyStationGUID equals dd.DutyStationGUID
                                                     orderby bb.FromDate descending
                                                     select dd.DutyStationDescription).FirstOrDefault(),
                           StaffEmail = (from aa in DbTBS.userServiceHistory.Where(x => x.Active && x.UserGUID == a.UserGUID)
                                         join bb in DbTBS.userProfiles.Where(x => x.Active) on aa.ServiceHistoryGUID equals bb.ServiceHistoryGUID
                                         orderby bb.FromDate descending
                                         select aa.EmailAddress).FirstOrDefault(),
                           FromDate = a.FromDate,
                           ToDate = a.ToDate,
                           Active = a.Active,
                           dataStaffPhoneRowVersion = a.dataStaffPhoneRowVersion,
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<StaffPhonesDataTableModel> Result = Mapper.Map<List<StaffPhonesDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("TBS/Configuration/StaffPhones/Create")]
        public ActionResult StaffPhonesCreate()
        {
            if (!CMS.HasAction(Permissions.StaffPhones.Create, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/TBS/Views/Configuration/StaffPhones/_StaffPhoneUpdateModal.cshtml", new StaffPhoneUpdateModel());
        }

        [Route("TBS/Configuration/StaffPhones/Update/{PK}")]
        public ActionResult StaffPhonesUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.StaffPhones.Update, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            var model = (from a in DbTBS.dataStaffPhone.WherePK(PK)
                         select new StaffPhoneUpdateModel
                         {
                             StaffSimGUID = a.StaffSimGUID,
                             UserGUID = a.UserGUID,
                             PhoneNumber = a.PhoneNumber,
                             TelecomCompanyOperationGUID = a.TelecomCompanyOperationGUID,
                             PhoneTypeGUID = a.PhoneTypeGUID,
                             PhoneUsageTypeGUID = a.PhoneUsageTypeGUID,
                             FromDate = a.FromDate,
                             ToDate = a.ToDate,
                             Active = a.Active,
                             dataStaffPhoneRowVersion = a.dataStaffPhoneRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("StaffPhones", "Configuration", new { Area = "TBS" }));

            return PartialView("~/Areas/TBS/Views/Configuration/StaffPhones/_StaffPhoneUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffPhonesCreate(StaffPhoneUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.StaffPhones.Create, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid) return PartialView("~/Areas/TBS/Views/Configuration/StaffPhones/_StaffPhoneUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();
            dataStaffPhone StaffPhone = Mapper.Map(model, new dataStaffPhone());
            StaffPhone.StaffSimGUID = EntityPK;
            DbTBS.Create(StaffPhone, Permissions.TelecomCompanies.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbTBS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTBS.SingleUpdateMessage(DataTableNames.StaffPhonesDataTable,
                   DbTBS.PrimaryKeyControl(StaffPhone),
                   DbTBS.RowVersionControls(Portal.SingleToList(StaffPhone))));
            }
            catch (Exception ex)
            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffPhonesUpdate(StaffPhoneUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.StaffPhones.Update, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid) return PartialView("~/Areas/TBS/Views/Configuration/StaffPhones/_StaffPhoneForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataStaffPhone StaffPhone = Mapper.Map(model, new dataStaffPhone());

            DbTBS.Update(StaffPhone, Permissions.StaffPhones.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbTBS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTBS.SingleUpdateMessage(DataTableNames.StaffPhonesDataTable,
                  DbTBS.PrimaryKeyControl(StaffPhone),
                  DbTBS.RowVersionControls(Portal.SingleToList(StaffPhone))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffPhones(model.StaffSimGUID);
            }
            catch (Exception ex)
            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffPhonesDelete(dataStaffPhone model)
        {
            if (!CMS.HasAction(Permissions.StaffPhones.Delete, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataStaffPhone> DeletedStaffPhones = DeleteStaffPhones(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.TelecomCompanies.Restore, Apps.TBS), Container = "StaffPhoneFormControls" });

            try
            {
                int CommitedRows = DbTBS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTBS.SingleDeleteMessage(DeletedStaffPhones, DataTableNames.StaffPhonesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffPhones(model.StaffSimGUID);
            }
            catch (Exception ex)
            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffPhonesRestore(dataStaffPhone model)
        {
            if (!CMS.HasAction(Permissions.StaffPhones.Restore, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            //if (ActiveApplication(model))
            //{
            //    return Json(DbCMS.RecordExists());
            //}

            List<dataStaffPhone> RestoredStaffPhones = RestoreStaffPhones(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.TelecomCompanies.Create, Apps.TBS, new UrlHelper(Request.RequestContext).Action("StaffPhones/Create", "Configuration", new { Area = "TBS" })), Container = "StaffPhoneFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.TelecomCompanies.Update, Apps.TBS), Container = "StaffPhoneFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.TelecomCompanies.Restore, Apps.TBS), Container = "StaffPhoneFormControls" });

            try
            {
                int CommitedRows = DbTBS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTBS.SingleRestoreMessage(CommitedRows, RestoredStaffPhones, DbTBS.PrimaryKeyControl(RestoredStaffPhones.FirstOrDefault()), Url.Action(DataTableNames.StaffPhonesDataTable, Portal.GetControllerName(ControllerContext)), null, UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffPhones(model.StaffSimGUID);
            }
            catch (Exception ex)
            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffPhonesDataTableDelete(List<dataStaffPhone> models)
        {
            if (!CMS.HasAction(Permissions.StaffPhones.Delete, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataStaffPhone> DeletedStaffPhones = DeleteStaffPhones(models);

            try
            {
                DbTBS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTBS.PartialDeleteMessage(DeletedStaffPhones, models, DataTableNames.StaffPhonesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffPhonesDataTableRestore(List<dataStaffPhone> models)
        {
            if (!CMS.HasAction(Permissions.StaffPhones.Restore, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataStaffPhone> RestoredStaffPhones = RestoreStaffPhones(models);

            try
            {
                DbTBS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTBS.PartialRestoreMessage(RestoredStaffPhones, models, DataTableNames.StaffPhonesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }

        private List<dataStaffPhone> DeleteStaffPhones(List<dataStaffPhone> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataStaffPhone> DeletedStaffPhones = new List<dataStaffPhone>();

            //THIS IS FOR PERMISSION TEST//
            //DO NOT DELETE THIS CODE
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = (from a in DbCMS.codeApplications
            //                    from f in DbCMS.codeApplicationsFactorForTest
            //                    where a.ApplicationGUID == f.ApplicationGUID
            //                    select new { a.ApplicationGUID, a.codeApplicationsRowVersion, f.OperationGUID, f.OrganizationGUID, f.CountryGUID }).AsQueryable().ToString();

            string query = DbTBS.QueryBuilder(models, Permissions.StaffPhones.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbTBS.Database.SqlQuery<dataStaffPhone>(query).ToList();
            foreach (var record in Records)
            {
                DeletedStaffPhones.Add(DbTBS.Delete(record, ExecutionTime, Permissions.StaffPhones.DeleteGuid, DbCMS));
            }


            return DeletedStaffPhones;
        }

        private List<dataStaffPhone> RestoreStaffPhones(List<dataStaffPhone> models)
        {
            Guid DeleteActionGUID = Permissions.StaffPhones.DeleteGuid;
            Guid RestoreActionGUID = Permissions.StaffPhones.RestoreGuid;
            DateTime RestoringTime = DateTime.Now;

            List<dataStaffPhone> RestoredStaffPhones = new List<dataStaffPhone>();

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

            string query = DbTBS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, "");


            var Records = DbTBS.Database.SqlQuery<dataStaffPhone>(query).ToList();
            foreach (var record in Records)
            {
                //if (!ActiveApplication(record))
                //{
                RestoredStaffPhones.Add(DbTBS.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS));
                //}
            }
            return RestoredStaffPhones;
        }

        private JsonResult ConcrrencyStaffPhones(Guid PK)
        {
            StaffPhoneUpdateModel dbModel = new StaffPhoneUpdateModel();

            var StaffPhone = DbTBS.dataStaffPhone.Where(a => a.StaffSimGUID == PK).FirstOrDefault();
            var dbStaffPhone = DbTBS.Entry(StaffPhone).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbStaffPhone, dbModel);


            if (StaffPhone.dataStaffPhoneRowVersion.SequenceEqual(dbModel.dataStaffPhoneRowVersion))
            {
                return Json(DbTBS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbTBS, dbModel, null));
        }

        private bool ActiveStaffPhone(Object model)
        {
            dataStaffPhone dataStaffPhone = Mapper.Map(model, new dataStaffPhone());

            //Check if same phone exists with different staff member

            //Check if same phone exists with another staff member








            //int ApplicationID = DbTBS.dataStaffSims
            //                         .Where(x => x.StaffName == Application.ApplicationID &&
            //                                     x.ApplicationGUID != Application.ApplicationGUID &&
            //                                     x.Active).Count();
            //if (ApplicationID > 0)
            //{
            //    ModelState.AddModelError("ApplicationID", "Application ID is already exists");
            //}
            //int ApplicationAcrynom = DbCMS.codeApplications
            //                              .Where(x => x.ApplicationAcrynom == Application.ApplicationAcrynom &&
            //                                          x.ApplicationGUID != Application.ApplicationGUID &&
            //                                          x.Active).Count();
            //if (ApplicationAcrynom > 0)
            //{
            //    ModelState.AddModelError("ApplicationAcrynom", "Application Acrynom is already exists");
            //}
            //int ApplicationDescription = DbCMS.codeApplicationsLanguages
            //                                  .Where(x => x.ApplicationDescription == ApplicationLanguages.ApplicationDescription &&
            //                                              x.ApplicationGUID != Application.ApplicationGUID &&
            //                                              x.LanguageID == LAN &&
            //                                              x.Active).Count();
            //if (ApplicationDescription > 0)
            //{
            //    ModelState.AddModelError("ApplicationDescription", "Application Description in selected language already exists");
            //}
            //return (ApplicationID + ApplicationAcrynom + ApplicationDescription) > 0;
            return false;
        }
        #endregion

        #region
        [Route("TBS/Configuration/FTPLocations/")]
        public ActionResult FTPLocationsIndex()
        {
            if (!CMS.HasAction(Permissions.CDRFTPLocationsManagement.Access, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/TBS/Views/Configuration/FTPLocations/Index.cshtml");
        }

        [Route("TBS/Configuration/CDRFTPLocationDataTable/")]
        public JsonResult CDRFTPLocationDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<CDRFTPLocationDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<CDRFTPLocationDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbTBS.codeCDRLocation.AsExpandable()
                       join b in DbTBS.codeDutyStationsLanguages.Where(x => x.LanguageID == LAN) on a.DutyStationGUID equals b.DutyStationGUID into LJ1
                       from RJ1 in LJ1.DefaultIfEmpty()
                       select new CDRFTPLocationDataTableModel
                       {
                           CDRLocationGUID = a.CDRLocationGUID,
                           DutyStationGUID = a.DutyStationGUID,
                           DutyStationDescription = RJ1.DutyStationDescription,
                           FTPPath = a.FTPPath,
                           FTPUsr = a.FTPUsr,
                           FTPPass = "******",
                           codeCDRLocationRowVersion = a.codeCDRLocationRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<CDRFTPLocationDataTableModel> Result = Mapper.Map<List<CDRFTPLocationDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("TBS/Configuration/CDRFTPLocationCreate/")]
        public ActionResult CDRFTPLocationCreate()
        {
            if (!CMS.HasAction(Permissions.CDRFTPLocationsManagement.Create, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/TBS/Views/Configuration/FTPLocations/_CDRFTPLocationUpdateModal.cshtml", new codeCDRLocation { CDRLocationGUID = Guid.Empty });
        }

        [Route("TBS/Configuration/CDRFTPLocationUpdate/{PK}")]
        public ActionResult CDRFTPLocationUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.CDRFTPLocationsManagement.Update, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/TBS/Views/Configuration/FTPLocations/_CDRFTPLocationUpdateModal.cshtml", DbTBS.codeCDRLocation.Find(PK));
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Route("TBS/Configuration/CDRFTPLocationCreate")]
        public ActionResult CDRFTPLocationCreate(codeCDRLocation model)
        {
            if (!CMS.HasAction(Permissions.CDRFTPLocationsManagement.Create, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveFTPLocation(model)) return PartialView("~/Areas/TBS/Views/Configuration/FTPLocations/_CDRFTPLocationUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbTBS.Create(model, Permissions.CDRFTPLocationsManagement.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbTBS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTBS.SingleUpdateMessage(DataTableNames.CDRFTPLocationDataTable,
                   DbTBS.PrimaryKeyControl(model),
                   DbTBS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }

        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("TBS/Configuration/CDRFTPLocationUpdate")]
        public ActionResult CDRFTPLocationUpdate(codeCDRLocation model)
        {
            if (!CMS.HasAction(Permissions.CDRFTPLocationsManagement.Update, Apps.TBS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveFTPLocation(model)) return PartialView("~/Areas/TBS/Views/Configuration/FTPLocations/_CDRFTPLocationUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbTBS.Update(model, Permissions.CDRFTPLocationsManagement.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbTBS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTBS.SingleUpdateMessage(DataTableNames.CDRFTPLocationDataTable,
                    DbTBS.PrimaryKeyControl(model),
                    DbTBS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyCDRFTPLocation(model.CDRLocationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }

        private bool ActiveFTPLocation(codeCDRLocation model)
        {
            int FTPExist = DbTBS.codeCDRLocation
                                 .Where(l => l.DutyStationGUID == model.DutyStationGUID &&
                                             l.CDRLocationGUID != model.CDRLocationGUID &&
                                             l.Active).Count();
            if (FTPExist > 0)
            {
                ModelState.AddModelError("DutyStationGUID", "CDR FTP For The Same DutyStation Already Exists."); //From resource ?????? Amer  
            }

            return (FTPExist > 0);
        }

        private JsonResult ConcrrencyCDRFTPLocation(Guid PK)
        {
            codeCDRLocation dbModel = new codeCDRLocation();

            var CDRLocation = DbTBS.codeCDRLocation.Where(l => l.CDRLocationGUID == PK).FirstOrDefault();
            var dbCDRLocation = DbTBS.Entry(CDRLocation).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(CDRLocation, dbModel);

            if (CDRLocation.codeCDRLocationRowVersion.SequenceEqual(dbModel.codeCDRLocationRowVersion))
            {
                return Json(DbTBS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbTBS, dbModel, "LanguagesContainer"));
        }
        #endregion
    }
}