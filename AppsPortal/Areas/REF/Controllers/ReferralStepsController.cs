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
    public class ReferralStepsController : REFBaseController
    {
        #region Referral Step

        public ActionResult Index()
        {
            return View();
        }

        [Route("REF/ReferralSteps/")]
        public ActionResult ReferralStepsIndex()
        {
            if (!CMS.HasAction(Permissions.ReferralStep.Access, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/REF/Views/ReferralSteps/Index.cshtml");
        }

        [Route("REF/ReferralStepsDataTable/")]
        public JsonResult ReferralStepsDataTable(DataTableRecievedOptions options)
        {
            var app = DbREF.configReferralStep.ToList();
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ReferralStepsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ReferralStepsDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbREF.configReferralStep.AsExpandable()
                       join b in DbREF.configReferralStepLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.configReferralStep.DeletedOn) && x.LanguageID == LAN) on a.ReferralStepGUID equals b.ReferralStepGUID into LJ1
                       join c in DbREF.configReferralLanguage.Where(x => x.LanguageID == LAN &&  x.Active) on a.ReferralGUID equals c.ReferralGUID into LJ2
                       join e in DbREF.codeApplicationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.configReferral.ApplicationGUID equals e.ApplicationGUID into LJ3
                       from R1 in LJ1.DefaultIfEmpty()
                       from R2 in LJ2.DefaultIfEmpty()
                       from R3 in LJ3.DefaultIfEmpty()
                       select new ReferralStepsDataTableModel
                       {
                           ReferralStepGUID = a.ReferralStepGUID,
                           Description=R1.Description,
                           StepSequence=a.StepSequence,
                           ReferralDescription = R2.ReferralDescription,
                           ReferralGUID= R2.ReferralGUID.ToString(),
                           ApplicationDescription = R3.ApplicationDescription,
                           ApplicationGUID= R3.ApplicationGUID.ToString(),
                           Active = a.Active,
                           configReferralStepRowVersion = a.configReferralStepRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ReferralStepsDataTableModel> Result = Mapper.Map<List<ReferralStepsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("REF/ReferralSteps/Create/")]
        public ActionResult ReferralStepCreate()
        {
            if (!CMS.HasAction(Permissions.ReferralStep.Create, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/REF/Views/ReferralSteps/ReferralStep.cshtml", new ReferralStepUpdateModel());
        }

        [Route("REF/ReferralSteps/Update/{PK}")]
        public ActionResult ReferralStepUpdate(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.ReferralStep.Access, Apps.REF))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var model = (from a in DbREF.configReferralStep.WherePK(PK)
                         join b in DbREF.configReferralStepLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.configReferralStep.DeletedOn) && x.LanguageID == LAN)
                         on a.ReferralStepGUID equals b.ReferralStepGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new ReferralStepUpdateModel
                         {
                             ReferralStepGUID = a.ReferralStepGUID,
                             DependOnReferralStepGUID=a.DependOnReferralStepGUID,
                             Description=R1.Description,
                             ReferralGUID=a.ReferralGUID,
                             StepSequence=a.StepSequence,
                             Active = a.Active,
                             configReferralStepRowVersion = a.configReferralStepRowVersion,
                             configReferralStepLanguageRowVersion = R1.configReferralStepLanguageRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("ReferralStep", "ReferralSteps", new { Area = "REF" }));

            return View("~/Areas/REF/Views/ReferralSteps/ReferralStep.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralStepCreate(ReferralStepUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.ReferralStep.Create, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveReferralStep(model)) return PartialView("~/Areas/REF/Views/ReferralSteps/_ReferralStepForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            configReferralStep ReferralStep = Mapper.Map(model, new configReferralStep());
            ReferralStep.ReferralStepGUID = EntityPK;
            DbREF.Create(ReferralStep, Permissions.ReferralStep.CreateGuid, ExecutionTime, DbCMS);

            configReferralStepLanguage Language = Mapper.Map(model, new configReferralStepLanguage());
            Language.ReferralStepGUID = EntityPK;

            DbREF.Create(Language, Permissions.ReferralStep.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ReferralStepLanguagesDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.ReferralStep.Create, Apps.REF, new UrlHelper(Request.RequestContext).Action("Create", "ReferralSteps", new { Area = "REF" })), Container = "ReferralStepFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.ReferralStep.Update, Apps.REF), Container = "ReferralStepFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.ReferralStep.Delete, Apps.REF), Container = "ReferralStepFormControls" });

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleCreateMessage(DbREF.PrimaryKeyControl(ReferralStep), DbREF.RowVersionControls(ReferralStep, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralStepUpdate(ReferralStepUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.ReferralStep.Update, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveReferralStep(model)) return PartialView("~/Areas/REF/Views/ReferralSteps/_ReferralStepForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            configReferralStep ReferralStep = Mapper.Map(model, new configReferralStep());
            DbREF.Update(ReferralStep, Permissions.ReferralStep.UpdateGuid, ExecutionTime, DbCMS);

            var Language = DbREF.configReferralStepLanguage.Where(l => l.ReferralStepGUID == model.ReferralStepGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.ReferralStepGUID = ReferralStep.ReferralStepGUID;
                DbREF.Create(Language, Permissions.ReferralStep.CreateGuid, ExecutionTime, DbCMS);
            }
            else if (Language.Description != model.Description)
            {
                Language = Mapper.Map(model, Language);
                DbREF.Update(Language, Permissions.ReferralStep.UpdateGuid, ExecutionTime, DbCMS);
            }

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleUpdateMessage(null, null, DbREF.RowVersionControls(ReferralStep, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyReferralStep(model.ReferralStepGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralStepDelete(configReferralStep model)
        {
            if (!CMS.HasAction(Permissions.ReferralStep.Delete, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<configReferralStep> DeletedReferralStep = DeleteReferralSteps(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.ReferralStep.Restore, Apps.REF), Container = "ReferralStepFormControls" });

            try
            {
                int CommitedRows = DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleDeleteMessage(CommitedRows, DeletedReferralStep.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyReferralStep(model.ReferralStepGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralStepRestore(configReferralStep model)
        {
            if (!CMS.HasAction(Permissions.ReferralStep.Restore, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveReferralStep(model))
            {
                return Json(DbREF.RecordExists());
            }

            List<configReferralStep> RestoredReferralSteps = RestoreReferralSteps(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.ReferralStep.Create, Apps.REF, new UrlHelper(Request.RequestContext).Action("ReferralStepCreate", "Configuration", new { Area = "REF" })), Container = "ReferralStepFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.ReferralStep.Update, Apps.REF), Container = "ReferralStepFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.ReferralStep.Delete, Apps.REF), Container = "ReferralStepFormControls" });

            try
            {
                int CommitedRows = DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleRestoreMessage(CommitedRows, RestoredReferralSteps, DbREF.PrimaryKeyControl(RestoredReferralSteps.FirstOrDefault()), Url.Action(DataTableNames.ReferralStepLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyReferralStep(model.ReferralStepGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ReferralStepsDataTableDelete(List<configReferralStep> models)
        {
            if (!CMS.HasAction(Permissions.ReferralStep.Delete, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<configReferralStep> DeletedReferralSteps = DeleteReferralSteps(models);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.PartialDeleteMessage(DeletedReferralSteps, models, DataTableNames.ReferralStepsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ReferralStepsDataTableRestore(List<configReferralStep> models)
        {
            if (!CMS.HasAction(Permissions.ReferralStep.Restore, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<configReferralStep> RestoredReferralSteps = RestoreReferralSteps(models);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.PartialRestoreMessage(RestoredReferralSteps, models, DataTableNames.ReferralStepsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        private List<configReferralStep> DeleteReferralSteps(List<configReferralStep> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<configReferralStep> DeletedReferralSteps = new List<configReferralStep>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbREF.QueryBuilder(models, Permissions.ReferralStep.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbREF.Database.SqlQuery<configReferralStep>(query).ToList();
            foreach (var record in Records)
            {
                DeletedReferralSteps.Add(DbREF.Delete(record, ExecutionTime, Permissions.ReferralStep.DeleteGuid, DbCMS));
            }

            var Languages = DeletedReferralSteps.SelectMany(a => a.configReferralStepLanguage).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbREF.Delete(language, ExecutionTime, Permissions.ReferralStep.DeleteGuid, DbCMS);
            }
            return DeletedReferralSteps;
        }

        private List<configReferralStep> RestoreReferralSteps(List<configReferralStep> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<configReferralStep> RestoredReferralSteps = new List<configReferralStep>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbREF.QueryBuilder(models, Permissions.ReferralStep.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbREF.Database.SqlQuery<configReferralStep>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveReferralStep(record))
                {
                    RestoredReferralSteps.Add(DbREF.Restore(record, Permissions.ReferralStep.DeleteGuid, Permissions.ReferralStep.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var Languages = RestoredReferralSteps.SelectMany(x => x.configReferralStepLanguage.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbREF.Restore(language, Permissions.ReferralStep.DeleteGuid, Permissions.ReferralStep.RestoreGuid, RestoringTime, DbCMS);
            }

            return RestoredReferralSteps;
        }

        private JsonResult ConcurrencyReferralStep(Guid PK)
        {
            ReferralStepUpdateModel dbModel = new ReferralStepUpdateModel();

            var ReferralStep = DbREF.configReferralStep.Where(x => x.ReferralStepGUID == PK).FirstOrDefault();
            var dbReferralStep = DbREF.Entry(ReferralStep).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbReferralStep, dbModel);

            var Language = DbREF.configReferralStepLanguage.Where(x => x.ReferralStepGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.configReferralStep.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbREF.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (ReferralStep.configReferralStepRowVersion.SequenceEqual(dbModel.configReferralStepRowVersion) && Language.configReferralStepLanguageRowVersion.SequenceEqual(dbModel.configReferralStepLanguageRowVersion))
            {
                return Json(DbREF.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbREF, dbModel, "LanguagesContainer"));
        }

        private bool ActiveReferralStep(Object model)
        {
            configReferralStepLanguage ReferralStep = Mapper.Map(model, new configReferralStepLanguage());
            int ReferralStepDescription = DbREF.configReferralStepLanguage
                                    .Where(x => x.Description == ReferralStep.Description &&
                                                x.ReferralStepGUID != ReferralStep.ReferralStepGUID &&
                                                x.Active).Count();
            if (ReferralStepDescription > 0)
            {
                ModelState.AddModelError("ReferralStepDescription", "ReferralStep is already exists");
            }
            return (ReferralStepDescription > 0);
        }

        #endregion

        #region Referral Step Language

        //[Route("REF/ReferralStepLanguagesDataTable/{PK}")]
        public ActionResult ReferralStepLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/REF/Views/ReferralSteps/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<configReferralStepLanguage, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<configReferralStepLanguage>(DataTable.Filters);
            }

            var Result = DbREF.configReferralStepLanguage.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.ReferralStepGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.ReferralStepLanguageGUID,
                                  x.LanguageID,
                                  x.Description,
                                  x.configReferralStepLanguageRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReferralStepLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ReferralStep.Create, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/REF/Views/ReferralSteps/_LanguageUpdateModal.cshtml",
                new configReferralStepLanguage { ReferralStepGUID = FK });
        }

        public ActionResult ReferralStepLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ReferralStep.Access, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/REF/Views/ReferralSteps/_LanguageUpdateModal.cshtml", DbREF.configReferralStepLanguage.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralStepLanguageCreate(configReferralStepLanguage model)
        {
            if (!CMS.HasAction(Permissions.ReferralStep.Create, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveReferralStepLanguage(model)) return PartialView("~/Areas/REF/Views/ReferralSteps/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbREF.Create(model, Permissions.ReferralStep.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleUpdateMessage(DataTableNames.ReferralStepLanguagesDataTable, DbREF.PrimaryKeyControl(model), DbREF.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralStepLanguageUpdate(configReferralStepLanguage model)
        {
            if (!CMS.HasAction(Permissions.ReferralStep.Update, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveReferralStepLanguage(model)) return PartialView("~/Areas/REF/Views/ReferralSteps/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbREF.Update(model, Permissions.ReferralStep.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleUpdateMessage(DataTableNames.ReferralStepLanguagesDataTable,
                    DbREF.PrimaryKeyControl(model),
                    DbREF.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyReferralStepLanguage(model.ReferralStepLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralStepLanguageDelete(configReferralStepLanguage model)
        {
            if (!CMS.HasAction(Permissions.ReferralStep.Delete, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<configReferralStepLanguage> DeletedLanguages = DeleteReferralStepLanguages(new List<configReferralStepLanguage> { model });

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleDeleteMessage(DeletedLanguages, DataTableNames.ReferralStepLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyReferralStepLanguage(model.ReferralStepLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralStepLanguageRestore(configReferralStepLanguage model)
        {
            if (!CMS.HasAction(Permissions.ReferralStep.Restore, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveReferralStepLanguage(model))
            {
                return Json(DbREF.RecordExists());
            }

            List<configReferralStepLanguage> RestoredLanguages = RestoreReferralStepLanguages(Portal.SingleToList(model));

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleRestoreMessage(RestoredLanguages, DataTableNames.ReferralStepLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyReferralStepLanguage(model.ReferralStepLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ReferralStepLanguagesDataTableDelete(List<configReferralStepLanguage> models)
        {
            if (!CMS.HasAction(Permissions.ReferralStep.Delete, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<configReferralStepLanguage> DeletedLanguages = DeleteReferralStepLanguages(models);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.ReferralStepLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ReferralStepLanguagesDataTableRestore(List<configReferralStepLanguage> models)
        {
            if (!CMS.HasAction(Permissions.ReferralStep.Restore, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<configReferralStepLanguage> RestoredLanguages = RestoreReferralStepLanguages(models);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.ReferralStepLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        private List<configReferralStepLanguage> DeleteReferralStepLanguages(List<configReferralStepLanguage> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<configReferralStepLanguage> DeletedReferralStepLanguages = new List<configReferralStepLanguage>();

            string query = DbREF.QueryBuilder(models, Permissions.ReferralStep.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbREF.Database.SqlQuery<configReferralStepLanguage>(query).ToList();

            foreach (var language in languages)
            {
                DeletedReferralStepLanguages.Add(DbREF.Delete(language, ExecutionTime, Permissions.ReferralStep.DeleteGuid, DbCMS));
            }

            return DeletedReferralStepLanguages;
        }

        private List<configReferralStepLanguage> RestoreReferralStepLanguages(List<configReferralStepLanguage> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<configReferralStepLanguage> RestoredLanguages = new List<configReferralStepLanguage>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbREF.QueryBuilder(models, Permissions.ReferralStep.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbREF.Database.SqlQuery<configReferralStepLanguage>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveReferralStepLanguage(language))
                {
                    RestoredLanguages.Add(DbREF.Restore(language, Permissions.ReferralStep.DeleteGuid, Permissions.ReferralStep.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyReferralStepLanguage(Guid PK)
        {
            configReferralStepLanguage dbModel = new configReferralStepLanguage();

            var Language = DbREF.configReferralStepLanguage.Where(l => l.ReferralStepLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbREF.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.configReferralStepLanguageRowVersion.SequenceEqual(dbModel.configReferralStepLanguageRowVersion))
            {
                return Json(DbREF.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbREF, dbModel, "LanguagesContainer"));
        }

        private bool ActiveReferralStepLanguage(configReferralStepLanguage model)
        {
            int LanguageID = DbREF.configReferralStepLanguage
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.ReferralStepGUID == model.ReferralStepGUID &&
                                              x.ReferralStepLanguageGUID != model.ReferralStepLanguageGUID &&
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