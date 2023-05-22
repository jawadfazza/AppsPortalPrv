using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.REF.ViewModels;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using REF_DAL.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.REF.Controllers
{
    public class ReferralStatusController : REFBaseController
    {
        #region Referral Status

        public ActionResult Index()
        {
            return View();
        }

        [Route("REF/ReferralStatus/")]
        public ActionResult ReferralStatusIndex()
        {
            if (!CMS.HasAction(Permissions.ReferralStatus.Access, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/REF/Views/ReferralStatus/Index.cshtml");
        }

        [Route("REF/ReferralStatusDataTable/")]
        public JsonResult ReferralStatusDataTable(DataTableRecievedOptions options)
        {
            //var app = DbREF.codeReferralStatus.ToList();
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ReferralStatusDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ReferralStatusDataTableModel>(DataTable.Filters);
            }


            var All = (from a in DbREF.codeReferralStatus.AsExpandable()
                       join b in DbREF.codeReferralStatusLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeReferralStatus.DeletedOn) && x.LanguageID == LAN) on a.ReferralStatusGUID equals b.ReferralStatusGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       select new ReferralStatusDataTableModel
                       {
                           ReferralStatusGUID = a.ReferralStatusGUID,
                           Description=R1.Description,
                           Active = a.Active,
                           codeReferralStatusRowVersion = a.codeReferralStatusRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ReferralStatusDataTableModel> Result = Mapper.Map<List<ReferralStatusDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("REF/ReferralStatus/Create/")]
        public ActionResult ReferralStatusCreate()
        {
            if (!CMS.HasAction(Permissions.ReferralStatus.Create, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/REF/Views/ReferralStatus/ReferralStatus.cshtml", new ReferralStatusUpdateModel());
        }

        [Route("REF/ReferralStatus/Update/{PK}")]
        public ActionResult ReferralStatusUpdate(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.ReferralStatus.Access, Apps.REF))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var model = (from a in DbREF.codeReferralStatus.WherePK(PK)
                         join b in DbREF.codeReferralStatusLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeReferralStatus.DeletedOn) && x.LanguageID == LAN)
                         on a.ReferralStatusGUID equals b.ReferralStatusGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new ReferralStatusUpdateModel
                         {
                             ReferralStatusGUID = a.ReferralStatusGUID,
                             Description=R1.Description,
                             ApplicationGUID=a.ApplicationGUID.Value,
                             Value=a.Value,
                             Active = a.Active,
                             codeReferralStatusRowVersion = a.codeReferralStatusRowVersion,
                             codeReferralStatusLanguageRowVersion = R1.codeReferralStatusLanguageRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("ReferralStatus", "ReferralStatus", new { Area = "REF" }));

            return View("~/Areas/REF/Views/ReferralStatus/ReferralStatus.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralStatusCreate(ReferralStatusUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.ReferralStatus.Create, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveReferralStatus(model)) return PartialView("~/Areas/REF/Views/ReferralStatus/_ReferralStatusForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeReferralStatus ReferralStatus = Mapper.Map(model, new codeReferralStatus());
            ReferralStatus.ReferralStatusGUID = EntityPK;
            DbREF.Create(ReferralStatus, Permissions.ReferralStatus.CreateGuid, ExecutionTime, DbCMS);

            codeReferralStatusLanguage Language = Mapper.Map(model, new codeReferralStatusLanguage());
            Language.ReferralStatusGUID = EntityPK;

            DbREF.Create(Language, Permissions.ReferralStatus.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ReferralStatusLanguagesDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.ReferralStatus.Create, Apps.REF, new UrlHelper(Request.RequestContext).Action("Create", "ReferralStatus", new { Area = "REF" })), Container = "ReferralStatusFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.ReferralStatus.Update, Apps.REF), Container = "ReferralStatusFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.ReferralStatus.Delete, Apps.REF), Container = "ReferralStatusFormControls" });

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleCreateMessage(DbREF.PrimaryKeyControl(ReferralStatus), DbREF.RowVersionControls(ReferralStatus, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralStatusUpdate(ReferralStatusUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.ReferralStatus.Update, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveReferralStatus(model)) return PartialView("~/Areas/REF/Views/ReferralStatus/_ReferralStatusForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeReferralStatus ReferralStatus = Mapper.Map(model, new codeReferralStatus());
            DbREF.Update(ReferralStatus, Permissions.ReferralStatus.UpdateGuid, ExecutionTime, DbCMS);

            var Language = DbREF.codeReferralStatusLanguage.Where(l => l.ReferralStatusGUID == model.ReferralStatusGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.ReferralStatusGUID = ReferralStatus.ReferralStatusGUID;
                DbREF.Create(Language, Permissions.ReferralStatus.CreateGuid, ExecutionTime, DbCMS);
            }
            else if (Language.Description != model.Description)
            {
                Language = Mapper.Map(model, Language);
                DbREF.Update(Language, Permissions.ReferralStatus.UpdateGuid, ExecutionTime, DbCMS);
            }

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleUpdateMessage(null, null, DbREF.RowVersionControls(ReferralStatus, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyReferralStatus(model.ReferralStatusGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralStatusDelete(codeReferralStatus model)
        {
            if (!CMS.HasAction(Permissions.ReferralStatus.Delete, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeReferralStatus> DeletedReferralStatus = DeleteReferralStatus(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.ReferralStatus.Restore, Apps.REF), Container = "ReferralStatusFormControls" });

            try
            {
                int CommitedRows = DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleDeleteMessage(CommitedRows, DeletedReferralStatus.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyReferralStatus(model.ReferralStatusGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralStatusRestore(codeReferralStatus model)
        {
            if (!CMS.HasAction(Permissions.ReferralStatus.Restore, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveReferralStatus(model))
            {
                return Json(DbREF.RecordExists());
            }

            List<codeReferralStatus> RestoredReferralStatus = RestoreReferralStatus(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.ReferralStatus.Create, Apps.REF, new UrlHelper(Request.RequestContext).Action("ReferralStatusCreate", "Configuration", new { Area = "REF" })), Container = "ReferralStatusFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.ReferralStatus.Update, Apps.REF), Container = "ReferralStatusFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.ReferralStatus.Delete, Apps.REF), Container = "ReferralStatusFormControls" });

            try
            {
                int CommitedRows = DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleRestoreMessage(CommitedRows, RestoredReferralStatus, DbREF.PrimaryKeyControl(RestoredReferralStatus.FirstOrDefault()), Url.Action(DataTableNames.ReferralStatusLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyReferralStatus(model.ReferralStatusGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ReferralStatusDataTableDelete(List<codeReferralStatus> models)
        {
            if (!CMS.HasAction(Permissions.ReferralStatus.Delete, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeReferralStatus> DeletedReferralStatus = DeleteReferralStatus(models);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.PartialDeleteMessage(DeletedReferralStatus, models, DataTableNames.ReferralStatusDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ReferralStatusDataTableRestore(List<codeReferralStatus> models)
        {
            if (!CMS.HasAction(Permissions.ReferralStatus.Restore, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeReferralStatus> RestoredReferralStatus = RestoreReferralStatus(models);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.PartialRestoreMessage(RestoredReferralStatus, models, DataTableNames.ReferralStatusDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        private List<codeReferralStatus> DeleteReferralStatus(List<codeReferralStatus> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<codeReferralStatus> DeletedReferralStatus = new List<codeReferralStatus>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbREF.QueryBuilder(models, Permissions.ReferralStatus.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbREF.Database.SqlQuery<codeReferralStatus>(query).ToList();
            foreach (var record in Records)
            {
                DeletedReferralStatus.Add(DbREF.Delete(record, ExecutionTime, Permissions.ReferralStatus.DeleteGuid, DbCMS));
            }

            var Languages = DeletedReferralStatus.SelectMany(a => a.codeReferralStatusLanguage).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbREF.Delete(language, ExecutionTime, Permissions.ReferralStatus.DeleteGuid, DbCMS);
            }
            return DeletedReferralStatus;
        }

        private List<codeReferralStatus> RestoreReferralStatus(List<codeReferralStatus> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<codeReferralStatus> RestoredReferralStatus = new List<codeReferralStatus>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbREF.QueryBuilder(models, Permissions.ReferralStatus.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbREF.Database.SqlQuery<codeReferralStatus>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveReferralStatus(record))
                {
                    RestoredReferralStatus.Add(DbREF.Restore(record, Permissions.ReferralStatus.DeleteGuid, Permissions.ReferralStatus.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var Languages = RestoredReferralStatus.SelectMany(x => x.codeReferralStatusLanguage.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbREF.Restore(language, Permissions.ReferralStatus.DeleteGuid, Permissions.ReferralStatus.RestoreGuid, RestoringTime, DbCMS);
            }

            return RestoredReferralStatus;
        }

        private JsonResult ConcurrencyReferralStatus(Guid PK)
        {
            ReferralStatusUpdateModel dbModel = new ReferralStatusUpdateModel();

            var ReferralStatus = DbREF.codeReferralStatus.Where(x => x.ReferralStatusGUID == PK).FirstOrDefault();
            var dbReferralStatus = DbREF.Entry(ReferralStatus).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbReferralStatus, dbModel);

            var Language = DbREF.codeReferralStatusLanguage.Where(x => x.ReferralStatusGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeReferralStatus.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbREF.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (ReferralStatus.codeReferralStatusRowVersion.SequenceEqual(dbModel.codeReferralStatusRowVersion) && Language.codeReferralStatusLanguageRowVersion.SequenceEqual(dbModel.codeReferralStatusLanguageRowVersion))
            {
                return Json(DbREF.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbREF, dbModel, "LanguagesContainer"));
        }

        private bool ActiveReferralStatus(Object model)
        {
            codeReferralStatusLanguage ReferralStatus = Mapper.Map(model, new codeReferralStatusLanguage());
            int ReferralStatusDescription = DbREF.codeReferralStatusLanguage
                                    .Where(x => x.Description == ReferralStatus.Description &&
                                                x.ReferralStatusGUID != ReferralStatus.ReferralStatusGUID &&
                                                x.Active).Count();
            if (ReferralStatusDescription > 0)
            {
                ModelState.AddModelError("ReferralStatusDescription", "ReferralStatus is already exists");
            }
            return (ReferralStatusDescription > 0);
        }

        #endregion

        #region Referral Status Language

        //[Route("REF/ReferralStatusLanguagesDataTable/{PK}")]
        public ActionResult ReferralStatusLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/REF/Views/ReferralStatus/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeReferralStatusLanguage, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeReferralStatusLanguage>(DataTable.Filters);
            }

            var Result = DbREF.codeReferralStatusLanguage.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.ReferralStatusGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.ReferralStatusLanguageGUID,
                                  x.LanguageID,
                                  x.Description,
                                  x.codeReferralStatusLanguageRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReferralStatusLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ReferralStatus.Create, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/REF/Views/ReferralStatus/_LanguageUpdateModal.cshtml",
                new codeReferralStatusLanguage { ReferralStatusGUID = FK });
        }

        public ActionResult ReferralStatusLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ReferralStatus.Access, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/REF/Views/ReferralStatus/_LanguageUpdateModal.cshtml", DbREF.codeReferralStatusLanguage.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralStatusLanguageCreate(codeReferralStatusLanguage model)
        {
            if (!CMS.HasAction(Permissions.ReferralStatus.Create, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveReferralStatusLanguage(model)) return PartialView("~/Areas/REF/Views/ReferralStatus/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbREF.Create(model, Permissions.ReferralStatus.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleUpdateMessage(DataTableNames.ReferralStatusLanguagesDataTable, DbREF.PrimaryKeyControl(model), DbREF.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralStatusLanguageUpdate(codeReferralStatusLanguage model)
        {
            if (!CMS.HasAction(Permissions.ReferralStatus.Update, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveReferralStatusLanguage(model)) return PartialView("~/Areas/REF/Views/ReferralStatus/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbREF.Update(model, Permissions.ReferralStatus.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleUpdateMessage(DataTableNames.ReferralStatusLanguagesDataTable,
                    DbREF.PrimaryKeyControl(model),
                    DbREF.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyReferralStatusLanguage(model.ReferralStatusLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralStatusLanguageDelete(codeReferralStatusLanguage model)
        {
            if (!CMS.HasAction(Permissions.ReferralStatus.Delete, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeReferralStatusLanguage> DeletedLanguages = DeleteReferralStatusLanguages(new List<codeReferralStatusLanguage> { model });

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleDeleteMessage(DeletedLanguages, DataTableNames.ReferralStatusLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyReferralStatusLanguage(model.ReferralStatusLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralStatusLanguageRestore(codeReferralStatusLanguage model)
        {
            if (!CMS.HasAction(Permissions.ReferralStatus.Restore, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveReferralStatusLanguage(model))
            {
                return Json(DbREF.RecordExists());
            }

            List<codeReferralStatusLanguage> RestoredLanguages = RestoreReferralStatusLanguages(Portal.SingleToList(model));

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleRestoreMessage(RestoredLanguages, DataTableNames.ReferralStatusLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyReferralStatusLanguage(model.ReferralStatusLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ReferralStatusLanguagesDataTableDelete(List<codeReferralStatusLanguage> models)
        {
            if (!CMS.HasAction(Permissions.ReferralStatus.Delete, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeReferralStatusLanguage> DeletedLanguages = DeleteReferralStatusLanguages(models);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.ReferralStatusLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ReferralStatusLanguagesDataTableRestore(List<codeReferralStatusLanguage> models)
        {
            if (!CMS.HasAction(Permissions.ReferralStatus.Restore, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeReferralStatusLanguage> RestoredLanguages = RestoreReferralStatusLanguages(models);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.ReferralStatusLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        private List<codeReferralStatusLanguage> DeleteReferralStatusLanguages(List<codeReferralStatusLanguage> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeReferralStatusLanguage> DeletedReferralStatusLanguages = new List<codeReferralStatusLanguage>();

            string query = DbREF.QueryBuilder(models, Permissions.ReferralStatus.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbREF.Database.SqlQuery<codeReferralStatusLanguage>(query).ToList();

            foreach (var language in languages)
            {
                DeletedReferralStatusLanguages.Add(DbREF.Delete(language, ExecutionTime, Permissions.ReferralStatus.DeleteGuid, DbCMS));
            }

            return DeletedReferralStatusLanguages;
        }

        private List<codeReferralStatusLanguage> RestoreReferralStatusLanguages(List<codeReferralStatusLanguage> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<codeReferralStatusLanguage> RestoredLanguages = new List<codeReferralStatusLanguage>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbREF.QueryBuilder(models, Permissions.ReferralStatus.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbREF.Database.SqlQuery<codeReferralStatusLanguage>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveReferralStatusLanguage(language))
                {
                    RestoredLanguages.Add(DbREF.Restore(language, Permissions.ReferralStatus.DeleteGuid, Permissions.ReferralStatus.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyReferralStatusLanguage(Guid PK)
        {
            codeReferralStatusLanguage dbModel = new codeReferralStatusLanguage();

            var Language = DbREF.codeReferralStatusLanguage.Where(l => l.ReferralStatusLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbREF.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeReferralStatusLanguageRowVersion.SequenceEqual(dbModel.codeReferralStatusLanguageRowVersion))
            {
                return Json(DbREF.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbREF, dbModel, "LanguagesContainer"));
        }

        private bool ActiveReferralStatusLanguage(codeReferralStatusLanguage model)
        {
            int LanguageID = DbREF.codeReferralStatusLanguage
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.ReferralStatusGUID == model.ReferralStatusGUID &&
                                              x.ReferralStatusLanguageGUID != model.ReferralStatusLanguageGUID &&
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