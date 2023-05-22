using SHM_DAL.Model;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.SHM.ViewModels;
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

namespace AppsPortal.Areas.SHM.Controllers
{
    public class ShuttleTravelPurposesController : SHMBaseController
    {
        #region Appointment Types

        public ActionResult Index()
        {
            return View();
        }

        [Route("SHM/ShuttleTravelPurposes/")]
        public ActionResult ShuttleTravelPurposesIndex()
        {
            //if (!CMS.HasAction(Permissions.ShuttleTravelPurpose.Access, Apps.SHM))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            return View("~/Areas/SHM/Views/ShuttleTravelPurposes/Index.cshtml");
        }

        [Route("SHM/ShuttleTravelPurposesDataTable/")]
        public JsonResult ShuttleTravelPurposesDataTable(DataTableRecievedOptions options)
        {
            var app = DbSHM.codeShuttleTravelPurpose.ToList();
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ShuttleTravelPurposesDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ShuttleTravelPurposesDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.ShuttleTravelPurpose.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();


            var All = (from a in DbSHM.codeShuttleTravelPurpose.AsExpandable()
                       join b in DbSHM.codeShuttleTravelPurposeLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeShuttleTravelPurpose.DeletedOn) && x.LanguageID == LAN) on a.ShuttleTravelPurposeGUID equals b.ShuttleTravelPurposeGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       select new ShuttleTravelPurposesDataTableModel
                       {
                           ShuttleTravelPurposeGUID = a.ShuttleTravelPurposeGUID,
                           ShuttleTravelPurposeDescription = R1.ShuttleTravelPurposeDescription,
                           Active = a.Active,
                           Priority = a.Priority,
                           codeShuttleTravelPurposeRowVersion = a.codeShuttleTravelPurposeRowVersion,
                           codeShuttleTravelPurposeLanguageRowVersion = R1.codeShuttleTravelPurposeLanguageRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ShuttleTravelPurposesDataTableModel> Result = Mapper.Map<List<ShuttleTravelPurposesDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("SHM/ShuttleTravelPurposes/Create/")]
        public ActionResult ShuttleTravelPurposeCreate()
        {
            if (!CMS.HasAction(Permissions.ShuttleTravelPurpose.Create, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/SHM/Views/ShuttleTravelPurposes/ShuttleTravelPurpose.cshtml", new ShuttleTravelPurposeUpdateModel());
        }

        [Route("SHM/ShuttleTravelPurposes/Update/{PK}")]
        public ActionResult ShuttleTravelPurposeUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ShuttleTravelPurpose.Access, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbSHM.codeShuttleTravelPurpose.WherePK(PK)
                         join b in DbSHM.codeShuttleTravelPurposeLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeShuttleTravelPurpose.DeletedOn) && x.LanguageID == LAN)
                         on a.ShuttleTravelPurposeGUID equals b.ShuttleTravelPurposeGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new ShuttleTravelPurposeUpdateModel
                         {
                             ShuttleTravelPurposeGUID = a.ShuttleTravelPurposeGUID,
                             ShuttleTravelPurposeDescription = R1.ShuttleTravelPurposeDescription,
                             Priority = a.Priority,
                             WithReturnDate=a.WithReturnDate.Value,
                            
                             Active = a.Active,
                             codeShuttleTravelPurposeRowVersion = a.codeShuttleTravelPurposeRowVersion,
                             codeShuttleTravelPurposeLanguageRowVersion = R1.codeShuttleTravelPurposeLanguageRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("ShuttleTravelPurpose", "ShuttleTravelPurposes", new { Area = "SHM" }));

            return View("~/Areas/SHM/Views/ShuttleTravelPurposes/ShuttleTravelPurpose.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleTravelPurposeCreate(ShuttleTravelPurposeUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.ShuttleTravelPurpose.Create, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveShuttleTravelPurpose(model)) return PartialView("~/Areas/SHM/Views/ShuttleTravelPurposes/_ShuttleTravelPurposeForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeShuttleTravelPurpose ShuttleTravelPurpose = Mapper.Map(model, new codeShuttleTravelPurpose());
            ShuttleTravelPurpose.ShuttleTravelPurposeGUID = EntityPK;
            DbSHM.Create(ShuttleTravelPurpose, Permissions.ShuttleTravelPurpose.CreateGuid, ExecutionTime, DbCMS);

            codeShuttleTravelPurposeLanguage Language = Mapper.Map(model, new codeShuttleTravelPurposeLanguage());
            Language.ShuttleTravelPurposeGUID = EntityPK;

            DbSHM.Create(Language, Permissions.ShuttleTravelPurpose.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ShuttleTravelPurposeLanguagesDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.ShuttleTravelPurpose.Create, Apps.SHM, new UrlHelper(Request.RequestContext).Action("Create", "ShuttleTravelPurposes", new { Area = "SHM" })), Container = "ShuttleTravelPurposeFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.ShuttleTravelPurpose.Update, Apps.SHM), Container = "ShuttleTravelPurposeFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.ShuttleTravelPurpose.Delete, Apps.SHM), Container = "ShuttleTravelPurposeFormControls" });

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleCreateMessage(DbSHM.PrimaryKeyControl(ShuttleTravelPurpose), DbSHM.RowVersionControls(ShuttleTravelPurpose, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleTravelPurposeUpdate(ShuttleTravelPurposeUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.ShuttleTravelPurpose.Update, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveShuttleTravelPurpose(model)) return PartialView("~/Areas/SHM/Views/ShuttleTravelPurposes/_ShuttleTravelPurposeForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeShuttleTravelPurpose ShuttleTravelPurpose = Mapper.Map(model, new codeShuttleTravelPurpose());
            DbSHM.Update(ShuttleTravelPurpose, Permissions.ShuttleTravelPurpose.UpdateGuid, ExecutionTime, DbCMS);

            var Language = DbSHM.codeShuttleTravelPurposeLanguage.Where(l => l.ShuttleTravelPurposeGUID == model.ShuttleTravelPurposeGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.ShuttleTravelPurposeGUID = ShuttleTravelPurpose.ShuttleTravelPurposeGUID;
                DbSHM.Create(Language, Permissions.ShuttleTravelPurpose.CreateGuid, ExecutionTime, DbCMS);
            }
            else if (Language.ShuttleTravelPurposeDescription != model.ShuttleTravelPurposeDescription)
            {
                Language = Mapper.Map(model, Language);
                DbSHM.Update(Language, Permissions.ShuttleTravelPurpose.UpdateGuid, ExecutionTime, DbCMS);
            }

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleUpdateMessage(null, null, DbSHM.RowVersionControls(ShuttleTravelPurpose, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyShuttleTravelPurpose(model.ShuttleTravelPurposeGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleTravelPurposeDelete(codeShuttleTravelPurpose model)
        {
            if (!CMS.HasAction(Permissions.ShuttleTravelPurpose.Delete, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeShuttleTravelPurpose> DeletedShuttleTravelPurpose = DeleteShuttleTravelPurposes(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.ShuttleTravelPurpose.Restore, Apps.SHM), Container = "ShuttleTravelPurposeFormControls" });

            try
            {
                int CommitedRows = DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleDeleteMessage(CommitedRows, DeletedShuttleTravelPurpose.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyShuttleTravelPurpose(model.ShuttleTravelPurposeGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleTravelPurposeRestore(codeShuttleTravelPurpose model)
        {
            if (!CMS.HasAction(Permissions.ShuttleTravelPurpose.Restore, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveShuttleTravelPurpose(model))
            {
                return Json(DbSHM.RecordExists());
            }

            List<codeShuttleTravelPurpose> RestoredShuttleTravelPurposes = RestoreShuttleTravelPurposes(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.ShuttleTravelPurpose.Create, Apps.SHM, new UrlHelper(Request.RequestContext).Action("ShuttleTravelPurposeCreate", "Configuration", new { Area = "SHM" })), Container = "ShuttleTravelPurposeFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.ShuttleTravelPurpose.Update, Apps.SHM), Container = "ShuttleTravelPurposeFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.ShuttleTravelPurpose.Delete, Apps.SHM), Container = "ShuttleTravelPurposeFormControls" });

            try
            {
                int CommitedRows = DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleRestoreMessage(CommitedRows, RestoredShuttleTravelPurposes, DbSHM.PrimaryKeyControl(RestoredShuttleTravelPurposes.FirstOrDefault()), Url.Action(DataTableNames.ShuttleTravelPurposeLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyShuttleTravelPurpose(model.ShuttleTravelPurposeGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ShuttleTravelPurposesDataTableDelete(List<codeShuttleTravelPurpose> models)
        {
            if (!CMS.HasAction(Permissions.ShuttleTravelPurpose.Delete, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeShuttleTravelPurpose> DeletedShuttleTravelPurposes = DeleteShuttleTravelPurposes(models);

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.PartialDeleteMessage(DeletedShuttleTravelPurposes, models, DataTableNames.ShuttleTravelPurposesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ShuttleTravelPurposesDataTableRestore(List<codeShuttleTravelPurpose> models)
        {
            if (!CMS.HasAction(Permissions.ShuttleTravelPurpose.Restore, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeShuttleTravelPurpose> RestoredShuttleTravelPurposes = RestoreShuttleTravelPurposes(models);

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.PartialRestoreMessage(RestoredShuttleTravelPurposes, models, DataTableNames.ShuttleTravelPurposesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        private List<codeShuttleTravelPurpose> DeleteShuttleTravelPurposes(List<codeShuttleTravelPurpose> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<codeShuttleTravelPurpose> DeletedShuttleTravelPurposes = new List<codeShuttleTravelPurpose>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbSHM.QueryBuilder(models, Permissions.ShuttleTravelPurpose.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbSHM.Database.SqlQuery<codeShuttleTravelPurpose>(query).ToList();
            foreach (var record in Records)
            {
                DeletedShuttleTravelPurposes.Add(DbSHM.Delete(record, ExecutionTime, Permissions.ShuttleTravelPurpose.DeleteGuid, DbCMS));
            }

            var Languages = DeletedShuttleTravelPurposes.SelectMany(a => a.codeShuttleTravelPurposeLanguage).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbSHM.Delete(language, ExecutionTime, Permissions.ShuttleTravelPurpose.DeleteGuid, DbCMS);
            }
            return DeletedShuttleTravelPurposes;
        }

        private List<codeShuttleTravelPurpose> RestoreShuttleTravelPurposes(List<codeShuttleTravelPurpose> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<codeShuttleTravelPurpose> RestoredShuttleTravelPurposes = new List<codeShuttleTravelPurpose>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbSHM.QueryBuilder(models, Permissions.ShuttleTravelPurpose.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbSHM.Database.SqlQuery<codeShuttleTravelPurpose>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveShuttleTravelPurpose(record))
                {
                    RestoredShuttleTravelPurposes.Add(DbSHM.Restore(record, Permissions.ShuttleTravelPurpose.DeleteGuid, Permissions.ShuttleTravelPurpose.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var Languages = RestoredShuttleTravelPurposes.SelectMany(x => x.codeShuttleTravelPurposeLanguage.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbSHM.Restore(language, Permissions.ShuttleTravelPurpose.DeleteGuid, Permissions.ShuttleTravelPurpose.RestoreGuid, RestoringTime, DbCMS);
            }

            return RestoredShuttleTravelPurposes;
        }

        private JsonResult ConcurrencyShuttleTravelPurpose(Guid PK)
        {
            ShuttleTravelPurposeUpdateModel dbModel = new ShuttleTravelPurposeUpdateModel();

            var ShuttleTravelPurpose = DbSHM.codeShuttleTravelPurpose.Where(x => x.ShuttleTravelPurposeGUID == PK).FirstOrDefault();
            var dbShuttleTravelPurpose = DbSHM.Entry(ShuttleTravelPurpose).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbShuttleTravelPurpose, dbModel);

            var Language = DbSHM.codeShuttleTravelPurposeLanguage.Where(x => x.ShuttleTravelPurposeGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeShuttleTravelPurpose.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbSHM.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (ShuttleTravelPurpose.codeShuttleTravelPurposeRowVersion.SequenceEqual(dbModel.codeShuttleTravelPurposeRowVersion) && Language.codeShuttleTravelPurposeLanguageRowVersion.SequenceEqual(dbModel.codeShuttleTravelPurposeLanguageRowVersion))
            {
                return Json(DbSHM.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbSHM, dbModel, "LanguagesContainer"));
        }

        private bool ActiveShuttleTravelPurpose(Object model)
        {
            codeShuttleTravelPurposeLanguage ShuttleTravelPurpose = Mapper.Map(model, new codeShuttleTravelPurposeLanguage());
            int ShuttleTravelPurposeDescription = DbSHM.codeShuttleTravelPurposeLanguage
                                    .Where(x => x.ShuttleTravelPurposeDescription == ShuttleTravelPurpose.ShuttleTravelPurposeDescription &&
                                                x.ShuttleTravelPurposeGUID != ShuttleTravelPurpose.ShuttleTravelPurposeGUID &&
                                                x.Active).Count();
            if (ShuttleTravelPurposeDescription > 0)
            {
                ModelState.AddModelError("ShuttleTravelPurposeDescription", "ShuttleTravelPurpose is already exists");
            }
            return (ShuttleTravelPurposeDescription > 0);
        }

        #endregion

        #region Appointment Types Language

        //[Route("SHM/ShuttleTravelPurposeLanguagesDataTable/{PK}")]
        public ActionResult ShuttleTravelPurposeLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/SHM/Views/ShuttleTravelPurposes/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeShuttleTravelPurposeLanguage, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeShuttleTravelPurposeLanguage>(DataTable.Filters);
            }

            var Result = DbSHM.codeShuttleTravelPurposeLanguage.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.ShuttleTravelPurposeGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.ShuttleTravelPurposeLanguageGUID,
                                  x.LanguageID,
                                  x.ShuttleTravelPurposeDescription,
                                  x.codeShuttleTravelPurposeLanguageRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShuttleTravelPurposeLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ShuttleTravelPurpose.Create, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/SHM/Views/ShuttleTravelPurposes/_LanguageUpdateModal.cshtml",
                new codeShuttleTravelPurposeLanguage { ShuttleTravelPurposeGUID = FK });
        }

        public ActionResult ShuttleTravelPurposeLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ShuttleTravelPurpose.Access, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/SHM/Views/ShuttleTravelPurposes/_LanguageUpdateModal.cshtml", DbSHM.codeShuttleTravelPurposeLanguage.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleTravelPurposeLanguageCreate(codeShuttleTravelPurposeLanguage model)
        {
            if (!CMS.HasAction(Permissions.ShuttleTravelPurpose.Create, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveShuttleTravelPurposeLanguage(model)) return PartialView("~/Areas/SHM/Views/ShuttleTravelPurposes/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbSHM.Create(model, Permissions.ShuttleTravelPurpose.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleUpdateMessage(DataTableNames.ShuttleTravelPurposeLanguagesDataTable, DbSHM.PrimaryKeyControl(model), DbSHM.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleTravelPurposeLanguageUpdate(codeShuttleTravelPurposeLanguage model)
        {
            if (!CMS.HasAction(Permissions.ShuttleTravelPurpose.Update, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveShuttleTravelPurposeLanguage(model)) return PartialView("~/Areas/SHM/Views/ShuttleTravelPurposes/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbSHM.Update(model, Permissions.ShuttleTravelPurpose.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleUpdateMessage(DataTableNames.ShuttleTravelPurposeLanguagesDataTable,
                    DbSHM.PrimaryKeyControl(model),
                    DbSHM.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyShuttleTravelPurposeLanguage(model.ShuttleTravelPurposeLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleTravelPurposeLanguageDelete(codeShuttleTravelPurposeLanguage model)
        {
            if (!CMS.HasAction(Permissions.ShuttleTravelPurpose.Delete, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeShuttleTravelPurposeLanguage> DeletedLanguages = DeleteShuttleTravelPurposeLanguages(new List<codeShuttleTravelPurposeLanguage> { model });

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleDeleteMessage(DeletedLanguages, DataTableNames.ShuttleTravelPurposeLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyShuttleTravelPurposeLanguage(model.ShuttleTravelPurposeLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleTravelPurposeLanguageRestore(codeShuttleTravelPurposeLanguage model)
        {
            if (!CMS.HasAction(Permissions.ShuttleTravelPurpose.Restore, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveShuttleTravelPurposeLanguage(model))
            {
                return Json(DbSHM.RecordExists());
            }

            List<codeShuttleTravelPurposeLanguage> RestoredLanguages = RestoreShuttleTravelPurposeLanguages(Portal.SingleToList(model));

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleRestoreMessage(RestoredLanguages, DataTableNames.ShuttleTravelPurposeLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyShuttleTravelPurposeLanguage(model.ShuttleTravelPurposeLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ShuttleTravelPurposeLanguagesDataTableDelete(List<codeShuttleTravelPurposeLanguage> models)
        {
            if (!CMS.HasAction(Permissions.ShuttleTravelPurpose.Delete, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeShuttleTravelPurposeLanguage> DeletedLanguages = DeleteShuttleTravelPurposeLanguages(models);

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.ShuttleTravelPurposeLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ShuttleTravelPurposeLanguagesDataTableRestore(List<codeShuttleTravelPurposeLanguage> models)
        {
            if (!CMS.HasAction(Permissions.ShuttleTravelPurpose.Restore, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeShuttleTravelPurposeLanguage> RestoredLanguages = RestoreShuttleTravelPurposeLanguages(models);

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.ShuttleTravelPurposeLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        private List<codeShuttleTravelPurposeLanguage> DeleteShuttleTravelPurposeLanguages(List<codeShuttleTravelPurposeLanguage> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeShuttleTravelPurposeLanguage> DeletedShuttleTravelPurposeLanguages = new List<codeShuttleTravelPurposeLanguage>();

            string query = DbSHM.QueryBuilder(models, Permissions.ShuttleTravelPurpose.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbSHM.Database.SqlQuery<codeShuttleTravelPurposeLanguage>(query).ToList();

            foreach (var language in languages)
            {
                DeletedShuttleTravelPurposeLanguages.Add(DbSHM.Delete(language, ExecutionTime, Permissions.ShuttleTravelPurpose.DeleteGuid, DbCMS));
            }

            return DeletedShuttleTravelPurposeLanguages;
        }

        private List<codeShuttleTravelPurposeLanguage> RestoreShuttleTravelPurposeLanguages(List<codeShuttleTravelPurposeLanguage> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<codeShuttleTravelPurposeLanguage> RestoredLanguages = new List<codeShuttleTravelPurposeLanguage>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbSHM.QueryBuilder(models, Permissions.ShuttleTravelPurpose.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbSHM.Database.SqlQuery<codeShuttleTravelPurposeLanguage>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveShuttleTravelPurposeLanguage(language))
                {
                    RestoredLanguages.Add(DbSHM.Restore(language, Permissions.ShuttleTravelPurpose.DeleteGuid, Permissions.ShuttleTravelPurpose.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyShuttleTravelPurposeLanguage(Guid PK)
        {
            codeShuttleTravelPurposeLanguage dbModel = new codeShuttleTravelPurposeLanguage();

            var Language = DbSHM.codeShuttleTravelPurposeLanguage.Where(l => l.ShuttleTravelPurposeLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbSHM.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeShuttleTravelPurposeLanguageRowVersion.SequenceEqual(dbModel.codeShuttleTravelPurposeLanguageRowVersion))
            {
                return Json(DbSHM.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbSHM, dbModel, "LanguagesContainer"));
        }

        private bool ActiveShuttleTravelPurposeLanguage(codeShuttleTravelPurposeLanguage model)
        {
            int LanguageID = DbSHM.codeShuttleTravelPurposeLanguage
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.ShuttleTravelPurposeGUID == model.ShuttleTravelPurposeGUID &&
                                              x.ShuttleTravelPurposeLanguageGUID != model.ShuttleTravelPurposeLanguageGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Shuttle Travel Purpose in selected language already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }

        #endregion
    }
}