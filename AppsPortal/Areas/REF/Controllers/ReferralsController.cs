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
    public class ReferralsController : REFBaseController
    {
        #region Referrals 
        public ActionResult Index()
        {
            return View();
        }

        [Route("REF/Referrals/")]
        public ActionResult ReferralsIndex()
        {
            if (!CMS.HasAction(Permissions.ReferralConfigManagement.Access, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/REF/Views/Referrals/Index.cshtml");
        }

        [Route("REF/ReferralsDataTable/")]
        public JsonResult ReferralsDataTable(DataTableRecievedOptions options)
        {
            var app = DbREF.configReferral.ToList();
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ReferralsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ReferralsDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.ReferralConfigManagement.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();

            var All = (from a in DbREF.configReferralLanguage.AsExpandable().Where(x => x.LanguageID == LAN)
                       join b in DbREF.codeApplicationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.configReferral.ApplicationGUID equals b.ApplicationGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()

                       select new ReferralsDataTableModel
                       {
                           ReferralGUID = a.ReferralGUID,
                           ApplicationDescription = R1.ApplicationDescription,
                           ApplicationGUID = R1.ApplicationGUID.ToString(),
                           ReferralDescription =a.ReferralDescription,
                           configReferralRowVersion = a.configReferral.configReferralRowVersion,
                           Active = a.Active
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ReferralsDataTableModel> Result = Mapper.Map<List<ReferralsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("REF/Referrals/Create/")]
        public ActionResult ReferralCreate()
        {
            if (!CMS.HasAction(Permissions.ReferralConfigManagement.Create, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return View("~/Areas/REF/Views/Referrals/Referral.cshtml", new ReferralUpdateModel());
        }

        [Route("REF/Referrals/Update/{PK}")]
        public ActionResult ReferralUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ReferralConfigManagement.Access, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbREF.configReferral.WherePK(PK)
                         join b in DbREF.configReferralLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.ReferralGUID equals b.ReferralGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new ReferralUpdateModel
                         {
                             ReferralGUID = a.ReferralGUID,
                             ApplicationGUID = a.ApplicationGUID,
                             ReferralDescription = R1.ReferralDescription,
                             Active = a.Active,
                             configReferralLanguageRowVersion = R1.configReferralLanguageRowVersion,
                             configReferralRowVersion = a.configReferralRowVersion,
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("Referral", "Referrals", new { Area = "REF" }));

            return View("~/Areas/REF/Views/Referrals/Referral.cshtml", model);
        }

      

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralCreate(ReferralUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.ReferralConfigManagement.Create, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveReferral(model)) return PartialView("~/Areas/REF/Views/Referrals/_ReferralForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            configReferral Referral = Mapper.Map(model, new configReferral());
            Referral.ReferralGUID = EntityPK;
            DbREF.Create(Referral, Permissions.ReferralConfigManagement.CreateGuid, ExecutionTime, DbCMS);

            configReferralLanguage Language = Mapper.Map(model, new configReferralLanguage());
            Language.ReferralGUID = EntityPK;

            DbREF.Create(Language, Permissions.ReferralConfigManagement.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ReferralLanguagesDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.ReferralConfigManagement.Create, Apps.REF, new UrlHelper(Request.RequestContext).Action("Create", "Referrals", new { Area = "REF" })), Container = "ReferralFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.ReferralConfigManagement.Update, Apps.REF), Container = "ReferralFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.ReferralConfigManagement.Delete, Apps.REF), Container = "ReferralFormControls" });

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleCreateMessage(DbREF.PrimaryKeyControl(Referral), DbREF.RowVersionControls(Referral, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralUpdate(ReferralUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.ReferralConfigManagement.Update, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveReferral(model)) return PartialView("~/Areas/REF/Views/Referrals/_ReferralForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            configReferral Referral = Mapper.Map(model, new configReferral());
            DbREF.Update(Referral, Permissions.ReferralConfigManagement.UpdateGuid, ExecutionTime, DbCMS);

            var Language = DbREF.configReferralLanguage.Where(l => l.ReferralGUID == model.ReferralGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.ReferralGUID = Referral.ReferralGUID;
                DbREF.Create(Language, Permissions.ReferralConfigManagement.CreateGuid, ExecutionTime, DbCMS);
            }
            else if (Language.ReferralDescription != model.ReferralDescription)
            {
                Language = Mapper.Map(model, Language);
                DbREF.Update(Language, Permissions.ReferralConfigManagement.UpdateGuid, ExecutionTime, DbCMS);
            }

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleUpdateMessage(null, null, DbREF.RowVersionControls(Referral, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyReferral(model.ReferralGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralDelete(configReferral model)
        {
            if (!CMS.HasAction(Permissions.ReferralConfigManagement.Delete, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<configReferral> DeletedReferral = DeleteReferrals(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.ReferralConfigManagement.Restore, Apps.REF), Container = "ReferralFormControls" });

            try
            {
                int CommitedRows = DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleDeleteMessage(CommitedRows, DeletedReferral.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyReferral(model.ReferralGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralRestore(configReferral model)
        {
            if (!CMS.HasAction(Permissions.ReferralConfigManagement.Restore, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveReferral(model))
            {
                return Json(DbREF.RecordExists());
            }

            List<configReferral> RestoredReferrals = RestoreReferrals(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.ReferralConfigManagement.Create, Apps.REF, new UrlHelper(Request.RequestContext).Action("ReferralCreate", "Referrals", new { Area = "REF" })), Container = "ReferralFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.ReferralConfigManagement.Update, Apps.REF), Container = "ReferralFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.ReferralConfigManagement.Delete, Apps.REF), Container = "ReferralFormControls" });

            try
            {
                int CommitedRows = DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleRestoreMessage(CommitedRows, RestoredReferrals, 
                    DbREF.PrimaryKeyControl(RestoredReferrals.FirstOrDefault()), Url.Action(DataTableNames.ReferralLanguagesDataTable,
                    Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyReferral(model.ReferralGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ReferralsDataTableDelete(List<configReferral> models)
        {
            if (!CMS.HasAction(Permissions.ReferralConfigManagement.Delete, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<configReferral> DeletedReferrals = DeleteReferrals(models);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.PartialDeleteMessage(DeletedReferrals, models, DataTableNames.ReferralsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ReferralsDataTableRestore(List<configReferral> models)
        {
            if (!CMS.HasAction(Permissions.ReferralConfigManagement.Restore, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<configReferral> RestoredReferrals = RestoreReferrals(models);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.PartialRestoreMessage(RestoredReferrals, models, DataTableNames.ReferralsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        private List<configReferral> DeleteReferrals(List<configReferral> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<configReferral> DeletedReferrals = new List<configReferral>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";


            string query = DbREF.QueryBuilder(models, Permissions.ReferralConfigManagement.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbREF.Database.SqlQuery<configReferral>(query).ToList();
            foreach (var record in Records)
            {
                DeletedReferrals.Add(DbREF.Delete(record, ExecutionTime, Permissions.ReferralConfigManagement.DeleteGuid, DbCMS));
            }

            var languages = DeletedReferrals.SelectMany(a => a.configReferralLanguage).Where(l => l.Active).ToList();
            foreach (var language in languages)
            {
                DbREF.Delete(language, ExecutionTime, Permissions.ReferralConfigManagement.DeleteGuid, DbCMS);
            }
            return DeletedReferrals;
        }

        private List<configReferral> RestoreReferrals(List<configReferral> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<configReferral> RestoredReferrals = new List<configReferral>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbREF.QueryBuilder(models, Permissions.ReferralConfigManagement.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbREF.Database.SqlQuery<configReferral>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveReferral(record))
                {
                    RestoredReferrals.Add(DbREF.Restore(record, Permissions.Appointment.DeleteGuid, Permissions.ReferralConfigManagement.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var languages = RestoredReferrals.SelectMany(x => x.configReferralLanguage.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in languages)
            {
                DbREF.Restore(language, Permissions.Appointment.DeleteGuid, Permissions.Appointment.RestoreGuid, RestoringTime, DbCMS);
            }

            return RestoredReferrals;
        }

        private JsonResult ConcurrencyReferral(Guid PK)
        {
            ReferralUpdateModel dbModel = new ReferralUpdateModel();

            var Referral = DbREF.configReferral.Where(x => x.ReferralGUID == PK).FirstOrDefault();
            var dbReferral = DbREF.Entry(Referral).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbReferral, dbModel);

            var Language = DbREF.configReferral.Where(x => x.ReferralGUID == PK).Where(x => x.Active == true).FirstOrDefault();
            var dbLanguage = DbREF.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Referral.configReferralRowVersion.SequenceEqual(dbModel.configReferralRowVersion) && Language.configReferralRowVersion.SequenceEqual(dbModel.configReferralRowVersion))
            {
                return Json(DbREF.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbREF, dbModel, "TabContainer"));
        }

        private bool ActiveReferral(Object model)
        {
            configReferralLanguage Referral = Mapper.Map(model, new configReferralLanguage());
            int ReferralDescription = DbREF.configReferralLanguage
                                    .Where(x => x.ReferralDescription == Referral.ReferralDescription &&
                                                x.ReferralGUID != Referral.ReferralGUID &&
                                                x.Active).Count();
            if (ReferralDescription > 0)
            {
                ModelState.AddModelError("ReferralDescription", "Referral is already exists");
            }
            return (ReferralDescription > 0);
        }

        #endregion

        #region Referral  Language

        //[Route("REF/ReferralLanguagesDataTable/{PK}")]
        public ActionResult ReferralLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/REF/Views/Referrals/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<configReferralLanguage, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<configReferralLanguage>(DataTable.Filters);
            }

            var Result = DbREF.configReferralLanguage.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.ReferralGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.ReferralLanguageGUID,
                                  x.LanguageID,
                                  x.ReferralDescription,
                                  x.configReferralLanguageRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReferralLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ReferralConfigManagement.Create, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/REF/Views/Referrals/_LanguageUpdateModal.cshtml",
                new configReferralLanguage { ReferralGUID = FK });
        }

        public ActionResult ReferralLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ReferralConfigManagement.Access, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/REF/Views/Referrals/_LanguageUpdateModal.cshtml", DbREF.configReferralLanguage.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralLanguageCreate(configReferralLanguage model)
        {
            if (!CMS.HasAction(Permissions.ReferralConfigManagement.Create, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveReferralLanguage(model)) return PartialView("~/Areas/REF/Views/Referrals/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbREF.Create(model, Permissions.ReferralConfigManagement.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleUpdateMessage(DataTableNames.ReferralLanguagesDataTable, DbREF.PrimaryKeyControl(model), DbREF.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralLanguageUpdate(configReferralLanguage model)
        {
            if (!CMS.HasAction(Permissions.ReferralConfigManagement.Update, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveReferralLanguage(model)) return PartialView("~/Areas/REF/Views/Referrals/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbREF.Update(model, Permissions.ReferralConfigManagement.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleUpdateMessage(DataTableNames.ReferralLanguagesDataTable,
                    DbREF.PrimaryKeyControl(model),
                    DbREF.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyReferralLanguage(model.ReferralLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralLanguageDelete(configReferralLanguage model)
        {
            if (!CMS.HasAction(Permissions.ReferralConfigManagement.Delete, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<configReferralLanguage> DeletedLanguages = DeleteReferralLanguages(new List<configReferralLanguage> { model });

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleDeleteMessage(DeletedLanguages, DataTableNames.ReferralLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyReferralLanguage(model.ReferralLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralLanguageRestore(configReferralLanguage model)
        {
            if (!CMS.HasAction(Permissions.ReferralConfigManagement.Restore, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveReferralLanguage(model))
            {
                return Json(DbREF.RecordExists());
            }

            List<configReferralLanguage> RestoredLanguages = RestoreReferralLanguages(Portal.SingleToList(model));

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleRestoreMessage(RestoredLanguages, DataTableNames.ReferralLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyReferralLanguage(model.ReferralLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ReferralLanguagesDataTableDelete(List<configReferralLanguage> models)
        {
            if (!CMS.HasAction(Permissions.ReferralConfigManagement.Delete, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<configReferralLanguage> DeletedLanguages = DeleteReferralLanguages(models);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.ReferralLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ReferralLanguagesDataTableRestore(List<configReferralLanguage> models)
        {
            if (!CMS.HasAction(Permissions.ReferralConfigManagement.Restore, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<configReferralLanguage> RestoredLanguages = RestoreReferralLanguages(models);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.ReferralLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        private List<configReferralLanguage> DeleteReferralLanguages(List<configReferralLanguage> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<configReferralLanguage> DeletedReferralLanguages = new List<configReferralLanguage>();

            string query = DbREF.QueryBuilder(models, Permissions.ReferralConfigManagement.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbREF.Database.SqlQuery<configReferralLanguage>(query).ToList();

            foreach (var language in languages)
            {
                DeletedReferralLanguages.Add(DbREF.Delete(language, ExecutionTime, Permissions.ReferralConfigManagement.DeleteGuid, DbCMS));
            }

            return DeletedReferralLanguages;
        }

        private List<configReferralLanguage> RestoreReferralLanguages(List<configReferralLanguage> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<configReferralLanguage> RestoredLanguages = new List<configReferralLanguage>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbREF.QueryBuilder(models, Permissions.ReferralConfigManagement.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbREF.Database.SqlQuery<configReferralLanguage>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveReferralLanguage(language))
                {
                    RestoredLanguages.Add(DbREF.Restore(language, Permissions.ReferralConfigManagement.DeleteGuid, Permissions.ReferralConfigManagement.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyReferralLanguage(Guid PK)
        {
            configReferralLanguage dbModel = new configReferralLanguage();

            var Language = DbREF.configReferralLanguage.Where(l => l.ReferralLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbREF.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.configReferralLanguageRowVersion.SequenceEqual(dbModel.configReferralLanguageRowVersion))
            {
                return Json(DbREF.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbREF, dbModel, "LanguagesContainer"));
        }

        private bool ActiveReferralLanguage(configReferralLanguage model)
        {
            int LanguageID = DbREF.configReferralLanguage
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.ReferralGUID == model.ReferralGUID &&
                                              x.ReferralLanguageGUID != model.ReferralLanguageGUID &&
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