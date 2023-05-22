using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using EMT_DAL.Model;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.EMT.Controllers
{
    public class MedicalGenericNamesController : EMTBaseController
    {
        #region Medical Manufacturers

        public ActionResult Index()
        {
            return View();
        }

        [Route("EMT/MedicalGenericNames/")]
        public ActionResult MedicalGenericNamesIndex()
        {
            if (!CMS.HasAction(Permissions.MedicalGenericName.Access, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/EMT/Views/MedicalGenericNames/Index.cshtml");
        }

        [Route("EMT/MedicalGenericNamesDataTable/")]
        public JsonResult MedicalGenericNamesDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MedicalGenericNamesDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MedicalGenericNamesDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.MedicalGenericName.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();


            var All = (from a in DbEMT.codeMedicalGenericName.AsExpandable()
                       join b in DbEMT.codeMedicalGenericNameLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeMedicalGenericName.DeletedOn) && x.LanguageID == LAN) on a.MedicalGenericNameGUID equals b.MedicalGenericNameGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       select new MedicalGenericNamesDataTableModel
                       {
                          
                           MedicalGenericNameGUID = a.MedicalGenericNameGUID,
                           MedicalGenericNameDescription = R1.MedicalGenericNameDescription,
                           Classification=a.Classification,
                           Dose=a.Dose,
                           Form=a.Form,
                           Sort=a.Sort,
                           Active = a.Active,
                           codeMedicalGenericNameRowVersion = a.codeMedicalGenericNameRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<MedicalGenericNamesDataTableModel> Result = Mapper.Map<List<MedicalGenericNamesDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("EMT/MedicalGenericNames/Create/")]
        public ActionResult MedicalGenericNameCreate()
        {
            if (!CMS.HasAction(Permissions.MedicalGenericName.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/EMT/Views/MedicalGenericNames/MedicalGenericName.cshtml", new MedicalGenericNameUpdateModel());
        }

        [Route("EMT/MedicalGenericNames/Update/{PK}")]
        public ActionResult MedicalGenericNameUpdate(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.MedicalGenericName.Access, Apps.EMT))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var model = (from a in DbEMT.codeMedicalGenericName.WherePK(PK)
                         join b in DbEMT.codeMedicalGenericNameLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeMedicalGenericName.DeletedOn) && x.LanguageID == LAN)
                         on a.MedicalGenericNameGUID equals b.MedicalGenericNameGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new MedicalGenericNameUpdateModel
                         {
                             MedicalGenericNameGUID = a.MedicalGenericNameGUID,
                             Sort = a.Sort,
                             Classification=a.Classification,
                             Dose=a.Dose,
                             Form=a.Form,
                             MedicalGenericNameDescription = R1.MedicalGenericNameDescription,
                             Active = a.Active,
                             codeMedicalGenericNameRowVersion = a.codeMedicalGenericNameRowVersion,
                             codeMedicalGenericNameLanguageRowVersion = R1.codeMedicalGenericNameLanguageRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("MedicalGenericName", "MedicalGenericNames", new { Area = "EMT" }));

            return View("~/Areas/EMT/Views/MedicalGenericNames/MedicalGenericName.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalGenericNameCreate(MedicalGenericNameUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.MedicalGenericName.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalGenericName(model)) return PartialView("~/Areas/EMT/Views/MedicalGenericNames/_MedicalGenericNameForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeMedicalGenericName MedicalGenericName = Mapper.Map(model, new codeMedicalGenericName());
            MedicalGenericName.MedicalGenericNameGUID = EntityPK;
            DbEMT.Create(MedicalGenericName, Permissions.MedicalGenericName.CreateGuid, ExecutionTime, DbCMS);

            codeMedicalGenericNameLanguage Language = Mapper.Map(model, new codeMedicalGenericNameLanguage());
            Language.MedicalGenericNameGUID = EntityPK;

            DbEMT.Create(Language, Permissions.MedicalGenericName.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.MedicalGenericNameLanguagesDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.MedicalGenericName.Create, Apps.EMT, new UrlHelper(Request.RequestContext).Action("Create", "MedicalGenericNames", new { Area = "EMT" })), Container = "MedicalGenericNameFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.MedicalGenericName.Update, Apps.EMT), Container = "MedicalGenericNameFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.MedicalGenericName.Delete, Apps.EMT), Container = "MedicalGenericNameFormControls" });

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleCreateMessage(DbEMT.PrimaryKeyControl(MedicalGenericName), DbEMT.RowVersionControls(MedicalGenericName, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalGenericNameUpdate(MedicalGenericNameUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.MedicalGenericName.Update, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalGenericName(model)) return PartialView("~/Areas/EMT/Views/MedicalGenericNames/_MedicalGenericNameForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeMedicalGenericName MedicalGenericName = Mapper.Map(model, new codeMedicalGenericName());
            DbEMT.Update(MedicalGenericName, Permissions.MedicalGenericName.UpdateGuid, ExecutionTime, DbCMS);

            var Language = DbEMT.codeMedicalGenericNameLanguage.Where(l => l.MedicalGenericNameGUID == model.MedicalGenericNameGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.MedicalGenericNameGUID = MedicalGenericName.MedicalGenericNameGUID;
                DbEMT.Create(Language, Permissions.MedicalGenericName.CreateGuid, ExecutionTime, DbCMS);
            }
            else if (Language.MedicalGenericNameDescription != model.MedicalGenericNameDescription)
            {
                Language = Mapper.Map(model, Language);
                DbEMT.Update(Language, Permissions.MedicalGenericName.UpdateGuid, ExecutionTime, DbCMS);
            }

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(null, null, DbEMT.RowVersionControls(MedicalGenericName, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMedicalGenericName(model.MedicalGenericNameGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalGenericNameDelete(codeMedicalGenericName model)
        {
            if (!CMS.HasAction(Permissions.MedicalGenericName.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeMedicalGenericName> DeletedMedicalGenericName = DeleteMedicalGenericNames(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.MedicalGenericName.Restore, Apps.EMT), Container = "MedicalGenericNameFormControls" });

            try
            {
                int CommitedRows = DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleDeleteMessage(CommitedRows, DeletedMedicalGenericName.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMedicalGenericName(model.MedicalGenericNameGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalGenericNameRestore(codeMedicalGenericName model)
        {
            if (!CMS.HasAction(Permissions.MedicalGenericName.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveMedicalGenericName(model))
            {
                return Json(DbEMT.RecordExists());
            }

            List<codeMedicalGenericName> RestoredMedicalGenericNames = RestoreMedicalGenericNames(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.MedicalGenericName.Create, Apps.EMT, new UrlHelper(Request.RequestContext).Action("MedicalGenericNameCreate", "Configuration", new { Area = "EMT" })), Container = "MedicalGenericNameFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.MedicalGenericName.Update, Apps.EMT), Container = "MedicalGenericNameFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.MedicalGenericName.Delete, Apps.EMT), Container = "MedicalGenericNameFormControls" });

            try
            {
                int CommitedRows = DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleRestoreMessage(CommitedRows, RestoredMedicalGenericNames, DbEMT.PrimaryKeyControl(RestoredMedicalGenericNames.FirstOrDefault()), Url.Action(DataTableNames.MedicalGenericNameLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMedicalGenericName(model.MedicalGenericNameGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalGenericNamesDataTableDelete(List<codeMedicalGenericName> models)
        {
            if (!CMS.HasAction(Permissions.MedicalGenericName.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeMedicalGenericName> DeletedMedicalGenericNames = DeleteMedicalGenericNames(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialDeleteMessage(DeletedMedicalGenericNames, models, DataTableNames.MedicalGenericNamesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalGenericNamesDataTableRestore(List<codeMedicalGenericName> models)
        {
            if (!CMS.HasAction(Permissions.MedicalGenericName.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeMedicalGenericName> RestoredMedicalGenericNames = RestoreMedicalGenericNames(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialRestoreMessage(RestoredMedicalGenericNames, models, DataTableNames.MedicalGenericNamesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        private List<codeMedicalGenericName> DeleteMedicalGenericNames(List<codeMedicalGenericName> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<codeMedicalGenericName> DeletedMedicalGenericNames = new List<codeMedicalGenericName>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalGenericName.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbEMT.Database.SqlQuery<codeMedicalGenericName>(query).ToList();
            foreach (var record in Records)
            {
                DeletedMedicalGenericNames.Add(DbEMT.Delete(record, ExecutionTime, Permissions.MedicalGenericName.DeleteGuid, DbCMS));
            }

            var Languages = DeletedMedicalGenericNames.SelectMany(a => a.codeMedicalGenericNameLanguage).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbEMT.Delete(language, ExecutionTime, Permissions.MedicalGenericName.DeleteGuid, DbCMS);
            }
            return DeletedMedicalGenericNames;
        }

        private List<codeMedicalGenericName> RestoreMedicalGenericNames(List<codeMedicalGenericName> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<codeMedicalGenericName> RestoredMedicalGenericNames = new List<codeMedicalGenericName>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalGenericName.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbEMT.Database.SqlQuery<codeMedicalGenericName>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveMedicalGenericName(record))
                {
                    RestoredMedicalGenericNames.Add(DbEMT.Restore(record, Permissions.MedicalGenericName.DeleteGuid, Permissions.MedicalGenericName.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var Languages = RestoredMedicalGenericNames.SelectMany(x => x.codeMedicalGenericNameLanguage.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbEMT.Restore(language, Permissions.MedicalGenericName.DeleteGuid, Permissions.MedicalGenericName.RestoreGuid, RestoringTime, DbCMS);
            }

            return RestoredMedicalGenericNames;
        }

        private JsonResult ConcurrencyMedicalGenericName(Guid PK)
        {
            MedicalGenericNameUpdateModel dbModel = new MedicalGenericNameUpdateModel();

            var MedicalGenericName = DbEMT.codeMedicalGenericName.Where(x => x.MedicalGenericNameGUID == PK).FirstOrDefault();
            var dbMedicalGenericName = DbEMT.Entry(MedicalGenericName).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbMedicalGenericName, dbModel);

            var Language = DbEMT.codeMedicalGenericNameLanguage.Where(x => x.MedicalGenericNameGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeMedicalGenericName.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbEMT.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (MedicalGenericName.codeMedicalGenericNameRowVersion.SequenceEqual(dbModel.codeMedicalGenericNameRowVersion) && Language.codeMedicalGenericNameLanguageRowVersion.SequenceEqual(dbModel.codeMedicalGenericNameLanguageRowVersion))
            {
                return Json(DbEMT.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbEMT, dbModel, "LanguagesContainer"));
        }

        private bool ActiveMedicalGenericName(Object model)
        {
            return false;
        }

        #endregion

        #region  Medical Manufacturer Language

        //[Route("EMT/MedicalGenericNameLanguagesDataTable/{PK}")]
        public ActionResult MedicalGenericNameLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/EMT/Views/MedicalGenericNames/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeMedicalGenericNameLanguage, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeMedicalGenericNameLanguage>(DataTable.Filters);
            }

            var Result = DbEMT.codeMedicalGenericNameLanguage.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.MedicalGenericNameGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.MedicalGenericNameLanguageGUID,
                                  x.LanguageID,
                                  x.MedicalGenericNameDescription,
                                  x.codeMedicalGenericNameLanguageRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult MedicalGenericNameLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.MedicalGenericName.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/EMT/Views/MedicalGenericNames/_LanguageUpdateModal.cshtml",
                new codeMedicalGenericNameLanguage { MedicalGenericNameGUID = FK });
        }

        public ActionResult MedicalGenericNameLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.MedicalGenericName.Access, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/EMT/Views/MedicalGenericNames/_LanguageUpdateModal.cshtml", DbEMT.codeMedicalGenericNameLanguage.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalGenericNameLanguageCreate(codeMedicalGenericNameLanguage model)
        {
            if (!CMS.HasAction(Permissions.MedicalGenericName.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalGenericNameLanguage(model)) return PartialView("~/Areas/EMT/Views/MedicalGenericNames/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbEMT.Create(model, Permissions.MedicalGenericName.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(DataTableNames.MedicalGenericNameLanguagesDataTable, DbEMT.PrimaryKeyControl(model), DbEMT.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalGenericNameLanguageUpdate(codeMedicalGenericNameLanguage model)
        {
            if (!CMS.HasAction(Permissions.MedicalGenericName.Update, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalGenericNameLanguage(model)) return PartialView("~/Areas/EMT/Views/MedicalGenericNames/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbEMT.Update(model, Permissions.MedicalGenericName.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(DataTableNames.MedicalGenericNameLanguagesDataTable,
                    DbEMT.PrimaryKeyControl(model),
                    DbEMT.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalGenericNameLanguage(model.MedicalGenericNameLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalGenericNameLanguageDelete(codeMedicalGenericNameLanguage model)
        {
            if (!CMS.HasAction(Permissions.MedicalGenericName.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeMedicalGenericNameLanguage> DeletedLanguages = DeleteMedicalGenericNameLanguages(new List<codeMedicalGenericNameLanguage> { model });

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleDeleteMessage(DeletedLanguages, DataTableNames.MedicalGenericNameLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalGenericNameLanguage(model.MedicalGenericNameLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalGenericNameLanguageRestore(codeMedicalGenericNameLanguage model)
        {
            if (!CMS.HasAction(Permissions.MedicalGenericName.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveMedicalGenericNameLanguage(model))
            {
                return Json(DbEMT.RecordExists());
            }

            List<codeMedicalGenericNameLanguage> RestoredLanguages = RestoreMedicalGenericNameLanguages(Portal.SingleToList(model));

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleRestoreMessage(RestoredLanguages, DataTableNames.MedicalGenericNameLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalGenericNameLanguage(model.MedicalGenericNameLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalGenericNameLanguagesDataTableDelete(List<codeMedicalGenericNameLanguage> models)
        {
            if (!CMS.HasAction(Permissions.MedicalGenericName.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeMedicalGenericNameLanguage> DeletedLanguages = DeleteMedicalGenericNameLanguages(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.MedicalGenericNameLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalGenericNameLanguagesDataTableRestore(List<codeMedicalGenericNameLanguage> models)
        {
            if (!CMS.HasAction(Permissions.MedicalGenericName.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeMedicalGenericNameLanguage> RestoredLanguages = RestoreMedicalGenericNameLanguages(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.MedicalGenericNameLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        private List<codeMedicalGenericNameLanguage> DeleteMedicalGenericNameLanguages(List<codeMedicalGenericNameLanguage> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeMedicalGenericNameLanguage> DeletedMedicalGenericNameLanguages = new List<codeMedicalGenericNameLanguage>();

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalGenericName.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbEMT.Database.SqlQuery<codeMedicalGenericNameLanguage>(query).ToList();

            foreach (var language in languages)
            {
                DeletedMedicalGenericNameLanguages.Add(DbEMT.Delete(language, ExecutionTime, Permissions.MedicalGenericName.DeleteGuid, DbCMS));
            }

            return DeletedMedicalGenericNameLanguages;
        }

        private List<codeMedicalGenericNameLanguage> RestoreMedicalGenericNameLanguages(List<codeMedicalGenericNameLanguage> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<codeMedicalGenericNameLanguage> RestoredLanguages = new List<codeMedicalGenericNameLanguage>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalGenericName.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbEMT.Database.SqlQuery<codeMedicalGenericNameLanguage>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveMedicalGenericNameLanguage(language))
                {
                    RestoredLanguages.Add(DbEMT.Restore(language, Permissions.MedicalGenericName.DeleteGuid, Permissions.MedicalGenericName.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyMedicalGenericNameLanguage(Guid PK)
        {
            codeMedicalGenericNameLanguage dbModel = new codeMedicalGenericNameLanguage();

            var Language = DbEMT.codeMedicalGenericNameLanguage.Where(l => l.MedicalGenericNameLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbEMT.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeMedicalGenericNameLanguageRowVersion.SequenceEqual(dbModel.codeMedicalGenericNameLanguageRowVersion))
            {
                return Json(DbEMT.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbEMT, dbModel, "LanguagesContainer"));
        }

        private bool ActiveMedicalGenericNameLanguage(codeMedicalGenericNameLanguage model)
        {
            int LanguageID = DbEMT.codeMedicalGenericNameLanguage
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.MedicalGenericNameGUID == model.MedicalGenericNameGUID &&
                                              x.MedicalGenericNameLanguageGUID != model.MedicalGenericNameLanguageGUID &&
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