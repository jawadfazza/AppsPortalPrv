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
using iTextSharp.text.pdf.qrcode;

namespace AppsPortal.Areas.SHM.Controllers
{
    public class ShuttleRequestRoutesController : SHMBaseController
    {
        #region Shuttle Request Route

        public ActionResult Index()
        {
            return View();
        }

        [Route("SHM/ShuttleRequestRoutes/")]
        public ActionResult ShuttleRequestRoutesIndex()
        {
            //if (!CMS.HasAction(Permissions.ShuttleRequestRoute.Access, Apps.SHM))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            return View("~/Areas/SHM/Views/ShuttleRequestRoutes/Index.cshtml");
        }

        [Route("SHM/ShuttleRequestRoutesDataTable/")]
        public JsonResult ShuttleRequestRoutesDataTable(DataTableRecievedOptions options)
        {
            var app = DbSHM.dataShuttleRequestRoute.ToList();
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ShuttleRequestRoutesDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ShuttleRequestRoutesDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.ShuttleRequestRoute.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();


            var All = (from a in DbSHM.dataShuttleRequestRoute.AsExpandable()
                       join b in DbSHM.codeLocationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.StartLocationGUID equals b.LocationGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join c in DbSHM.codeLocationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.EndLocationGUID equals c.LocationGUID into LJ2
                       from R2 in LJ2.DefaultIfEmpty()
                       select new ShuttleRequestRoutesDataTableModel
                       {
                           ShuttleRequestRouteGUID = a.ShuttleRequestRouteGUID,
                           EndLocationGUID=a.EndLocationGUID,
                           StartLocationGUID=a.StartLocationGUID,
                           EndLocation = R2.LocationDescription,
                           StartLocation = R1.LocationDescription,
                           Active = a.Active,
                           dataShuttleRequestRouteRowVersion = a.dataShuttleRequestRouteRowVersion,
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ShuttleRequestRoutesDataTableModel> Result = Mapper.Map<List<ShuttleRequestRoutesDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("SHM/ShuttleRequestRoutes/Create/")]
        public ActionResult ShuttleRequestRouteCreate()
        {
            if (!CMS.HasAction(Permissions.ShuttleRequestRoute.Create, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/SHM/Views/ShuttleRequestRoutes/ShuttleRequestRoute.cshtml", new ShuttleRequestRouteUpdateModel());
        }

        [Route("SHM/ShuttleRequestRoutes/Update/{PK}")]
        public ActionResult ShuttleRequestRouteUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ShuttleRequestRoute.Access, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbSHM.dataShuttleRequestRoute.WherePK(PK)
                         join b in DbSHM.codeLocations on a.StartLocationGUID equals b.LocationGUID
                         join c in DbSHM.codeLocations on a.EndLocationGUID equals c.LocationGUID
                         select new ShuttleRequestRouteUpdateModel
                         {
                             ShuttleRequestRouteGUID = a.ShuttleRequestRouteGUID,
                             CountryGUID=b.CountryGUID,
                             CountryGUID1=c.CountryGUID,
                             EndLocationGUID1=a.EndLocationGUID,
                             StartLocationGUID1=a.StartLocationGUID,
                             Active = a.Active,
                             dataShuttleRequestRouteRowVersion = a.dataShuttleRequestRouteRowVersion,
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("ShuttleRequestRoute", "ShuttleRequestRoutes", new { Area = "SHM" }));

            return View("~/Areas/SHM/Views/ShuttleRequestRoutes/ShuttleRequestRoute.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleRequestRouteCreate(ShuttleRequestRouteUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.ShuttleRequestRoute.Create, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveShuttleRequestRoute(model)) return PartialView("~/Areas/SHM/Views/ShuttleRequestRoutes/_ShuttleRequestRouteForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            dataShuttleRequestRoute ShuttleRequestRoute = Mapper.Map(model, new dataShuttleRequestRoute());
            ShuttleRequestRoute.StartLocationGUID = model.StartLocationGUID1;
            ShuttleRequestRoute.EndLocationGUID = model.EndLocationGUID1;

            ShuttleRequestRoute.ShuttleRequestRouteGUID = EntityPK;
            DbSHM.Create(ShuttleRequestRoute, Permissions.ShuttleRequestRoute.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ShuttleRequestRouteStepsDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.ShuttleRequestRoute.Create, Apps.SHM, new UrlHelper(Request.RequestContext).Action("Create", "ShuttleRequestRoutes", new { Area = "SHM" })), Container = "ShuttleRequestRouteFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.ShuttleRequestRoute.Update, Apps.SHM), Container = "ShuttleRequestRouteFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.ShuttleRequestRoute.Delete, Apps.SHM), Container = "ShuttleRequestRouteFormControls" });

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleCreateMessage(DbSHM.PrimaryKeyControl(ShuttleRequestRoute), DbSHM.RowVersionControls(new List<dataShuttleRequestRoute>() { ShuttleRequestRoute }), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleRequestRouteUpdate(ShuttleRequestRouteUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.ShuttleRequestRoute.Update, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveShuttleRequestRoute(model)) return PartialView("~/Areas/SHM/Views/ShuttleRequestRoutes/_ShuttleRequestRouteForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataShuttleRequestRoute ShuttleRequestRoute = Mapper.Map(model, new dataShuttleRequestRoute());
            DbSHM.Update(ShuttleRequestRoute, Permissions.ShuttleRequestRoute.UpdateGuid, ExecutionTime, DbCMS);


            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleUpdateMessage(null, null, DbSHM.RowVersionControls(new List<dataShuttleRequestRoute>() { ShuttleRequestRoute })));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyShuttleRequestRoute(model.ShuttleRequestRouteGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleRequestRouteDelete(dataShuttleRequestRoute model)
        {
            if (!CMS.HasAction(Permissions.ShuttleRequestRoute.Delete, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataShuttleRequestRoute> DeletedShuttleRequestRoute = DeleteShuttleRequestRoutes(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.ShuttleRequestRoute.Restore, Apps.SHM), Container = "ShuttleRequestRouteFormControls" });

            try
            {
                int CommitedRows = DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleDeleteMessage(CommitedRows, DeletedShuttleRequestRoute.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyShuttleRequestRoute(model.ShuttleRequestRouteGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleRequestRouteRestore(dataShuttleRequestRoute model)
        {
            if (!CMS.HasAction(Permissions.ShuttleRequestRoute.Restore, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveShuttleRequestRoute(model))
            {
                return Json(DbSHM.RecordExists());
            }

            List<dataShuttleRequestRoute> RestoredShuttleRequestRoutes = RestoreShuttleRequestRoutes(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.ShuttleRequestRoute.Create, Apps.SHM, new UrlHelper(Request.RequestContext).Action("ShuttleRequestRouteCreate", "Configuration", new { Area = "SHM" })), Container = "ShuttleRequestRouteFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.ShuttleRequestRoute.Update, Apps.SHM), Container = "ShuttleRequestRouteFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.ShuttleRequestRoute.Delete, Apps.SHM), Container = "ShuttleRequestRouteFormControls" });

            try
            {
                int CommitedRows = DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleRestoreMessage(CommitedRows, RestoredShuttleRequestRoutes, DbSHM.PrimaryKeyControl(RestoredShuttleRequestRoutes.FirstOrDefault()), Url.Action(DataTableNames.ShuttleRequestRouteStepsDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyShuttleRequestRoute(model.ShuttleRequestRouteGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ShuttleRequestRoutesDataTableDelete(List<dataShuttleRequestRoute> models)
        {
            if (!CMS.HasAction(Permissions.ShuttleRequestRoute.Delete, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataShuttleRequestRoute> DeletedShuttleRequestRoutes = DeleteShuttleRequestRoutes(models);

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.PartialDeleteMessage(DeletedShuttleRequestRoutes, models, DataTableNames.ShuttleRequestRoutesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ShuttleRequestRoutesDataTableRestore(List<dataShuttleRequestRoute> models)
        {
            if (!CMS.HasAction(Permissions.ShuttleRequestRoute.Restore, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataShuttleRequestRoute> RestoredShuttleRequestRoutes = RestoreShuttleRequestRoutes(models);

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.PartialRestoreMessage(RestoredShuttleRequestRoutes, models, DataTableNames.ShuttleRequestRoutesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        private List<dataShuttleRequestRoute> DeleteShuttleRequestRoutes(List<dataShuttleRequestRoute> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataShuttleRequestRoute> DeletedShuttleRequestRoutes = new List<dataShuttleRequestRoute>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbSHM.QueryBuilder(models, Permissions.ShuttleRequestRoute.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbSHM.Database.SqlQuery<dataShuttleRequestRoute>(query).ToList();
            foreach (var record in Records)
            {
                DeletedShuttleRequestRoutes.Add(DbSHM.Delete(record, ExecutionTime, Permissions.ShuttleRequestRoute.DeleteGuid, DbCMS));
            }

            var Languages = DeletedShuttleRequestRoutes.SelectMany(a => a.dataShuttleRequestRouteStep).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbSHM.Delete(language, ExecutionTime, Permissions.ShuttleRequestRoute.DeleteGuid, DbCMS);
            }
            return DeletedShuttleRequestRoutes;
        }

        private List<dataShuttleRequestRoute> RestoreShuttleRequestRoutes(List<dataShuttleRequestRoute> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataShuttleRequestRoute> RestoredShuttleRequestRoutes = new List<dataShuttleRequestRoute>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbSHM.QueryBuilder(models, Permissions.ShuttleRequestRoute.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbSHM.Database.SqlQuery<dataShuttleRequestRoute>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveShuttleRequestRoute(record))
                {
                    RestoredShuttleRequestRoutes.Add(DbSHM.Restore(record, Permissions.ShuttleRequestRoute.DeleteGuid, Permissions.ShuttleRequestRoute.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var Languages = RestoredShuttleRequestRoutes.SelectMany(x => x.dataShuttleRequestRouteStep.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbSHM.Restore(language, Permissions.ShuttleRequestRoute.DeleteGuid, Permissions.ShuttleRequestRoute.RestoreGuid, RestoringTime, DbCMS);
            }

            return RestoredShuttleRequestRoutes;
        }

        private JsonResult ConcurrencyShuttleRequestRoute(Guid PK)
        {
            ShuttleRequestRouteUpdateModel dbModel = new ShuttleRequestRouteUpdateModel();

            var ShuttleRequestRoute = DbSHM.dataShuttleRequestRoute.Where(x => x.ShuttleRequestRouteGUID == PK).FirstOrDefault();
            var dbShuttleRequestRoute = DbSHM.Entry(ShuttleRequestRoute).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbShuttleRequestRoute, dbModel);

            var Language = DbSHM.dataShuttleRequestRouteStep.Where(x => x.ShuttleRequestRouteGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataShuttleRequestRoute.DeletedOn) ).FirstOrDefault();
            var dbLanguage = DbSHM.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (ShuttleRequestRoute.dataShuttleRequestRouteRowVersion.SequenceEqual(dbModel.dataShuttleRequestRouteRowVersion))
            {
                return Json(DbSHM.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbSHM, dbModel, "LanguagesContainer"));
        }

        private bool ActiveShuttleRequestRoute(Object model)
        {
            dataShuttleRequestRouteStep ShuttleRequestRoute = Mapper.Map(model, new dataShuttleRequestRouteStep());
            int ShuttleRequestRouteDescription = DbSHM.dataShuttleRequestRouteStep
                                    .Where(x =>
                                               x.ShuttleRequestRouteGUID != ShuttleRequestRoute.ShuttleRequestRouteGUID &&
                                              x.StartLocationGUID == ShuttleRequestRoute.StartLocationGUID &&
                                              x.EndLocationGUID == ShuttleRequestRoute.EndLocationGUID &&
                                                x.Active).Count();
            if (ShuttleRequestRouteDescription > 0)
            {
                ModelState.AddModelError("ShuttleRequestRouteDescription", "ShuttleRequestRoute is already exists");
            }
            return (ShuttleRequestRouteDescription > 0);
        }

        #endregion

        #region Shuttle Request Route Step

        //[Route("SHM/ShuttleRequestRouteStepsDataTable/{PK}")]
        public ActionResult ShuttleRequestRouteStepsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/SHM/Views/ShuttleRequestRoutes/_ShuttleRequestRouteStepsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<dataShuttleRequestRouteStep, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<dataShuttleRequestRouteStep>(DataTable.Filters);
            }

            var Result =
                (from a in DbSHM.dataShuttleRequestRouteStep.AsExpandable().Where(x => x.ShuttleRequestRouteGUID == PK).Where(Predicate)
                 join b in DbSHM.codeLocationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.StartLocationGUID equals b.LocationGUID into LJ1
                 from R1 in LJ1.DefaultIfEmpty()
                 join c in DbSHM.codeLocationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.EndLocationGUID equals c.LocationGUID into LJ2
                 from R2 in LJ2.DefaultIfEmpty()
                 select  new
                 {
                     a.ShuttleRequestRouteStepGUID,
                     a.StartLocationGUID,
                     a.EndLocationGUID,
                     StartLocation=R1.LocationDescription,
                     EndLocation=R2.LocationDescription,
                     CountryGUID=R1.codeLocations.CountryGUID,
                     CountryGUID1 = R2.codeLocations.CountryGUID,
                     a.OrderStep,
                     a.Active,
                     a.dataShuttleRequestRouteStepRowVersion
                 });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShuttleRequestRouteStepCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ShuttleRequestRoute.Create, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/SHM/Views/ShuttleRequestRoutes/_ShuttleRequestRouteStepUpdateModal.cshtml",
                new dataShuttleRequestRouteStep { ShuttleRequestRouteGUID = FK });
        }

        public ActionResult ShuttleRequestRouteStepUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ShuttleRequestRoute.Access, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/SHM/Views/ShuttleRequestRoutes/_ShuttleRequestRouteStepUpdateModal.cshtml", DbSHM.dataShuttleRequestRouteStep.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleRequestRouteStepCreate(dataShuttleRequestRouteStep model)
        {
            if (!CMS.HasAction(Permissions.ShuttleRequestRoute.Create, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveShuttleRequestRouteStep(model)) return PartialView("~/Areas/SHM/Views/ShuttleRequestRoutes/_ShuttleRequestRouteStepUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbSHM.Create(model, Permissions.ShuttleRequestRoute.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleUpdateMessage(DataTableNames.ShuttleRequestRouteStepsDataTable, DbSHM.PrimaryKeyControl(model), DbSHM.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleRequestRouteStepUpdate(dataShuttleRequestRouteStep model)
        {
            if (!CMS.HasAction(Permissions.ShuttleRequestRoute.Update, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveShuttleRequestRouteStep(model)) return PartialView("~/Areas/SHM/Views/ShuttleRequestRoutes/_ShuttleRequestRouteStepUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbSHM.Update(model, Permissions.ShuttleRequestRoute.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleUpdateMessage(DataTableNames.ShuttleRequestRouteStepsDataTable,
                    DbSHM.PrimaryKeyControl(model),
                    DbSHM.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyShuttleRequestRouteStep(model.ShuttleRequestRouteStepGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleRequestRouteStepDelete(dataShuttleRequestRouteStep model)
        {
            if (!CMS.HasAction(Permissions.ShuttleRequestRoute.Delete, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataShuttleRequestRouteStep> DeletedLanguages = DeleteShuttleRequestRouteSteps(new List<dataShuttleRequestRouteStep> { model });

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleDeleteMessage(DeletedLanguages, DataTableNames.ShuttleRequestRouteStepsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyShuttleRequestRouteStep(model.ShuttleRequestRouteStepGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleRequestRouteStepRestore(dataShuttleRequestRouteStep model)
        {
            if (!CMS.HasAction(Permissions.ShuttleRequestRoute.Restore, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveShuttleRequestRouteStep(model))
            {
                return Json(DbSHM.RecordExists());
            }

            List<dataShuttleRequestRouteStep> RestoredLanguages = RestoreShuttleRequestRouteSteps(Portal.SingleToList(model));

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleRestoreMessage(RestoredLanguages, DataTableNames.ShuttleRequestRouteStepsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyShuttleRequestRouteStep(model.ShuttleRequestRouteStepGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ShuttleRequestRouteStepsDataTableDelete(List<dataShuttleRequestRouteStep> models)
        {
            if (!CMS.HasAction(Permissions.ShuttleRequestRoute.Delete, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataShuttleRequestRouteStep> DeletedLanguages = DeleteShuttleRequestRouteSteps(models);

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.ShuttleRequestRouteStepsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ShuttleRequestRouteStepsDataTableRestore(List<dataShuttleRequestRouteStep> models)
        {
            if (!CMS.HasAction(Permissions.ShuttleRequestRoute.Restore, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataShuttleRequestRouteStep> RestoredLanguages = RestoreShuttleRequestRouteSteps(models);

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.ShuttleRequestRouteStepsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        private List<dataShuttleRequestRouteStep> DeleteShuttleRequestRouteSteps(List<dataShuttleRequestRouteStep> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataShuttleRequestRouteStep> DeletedShuttleRequestRouteSteps = new List<dataShuttleRequestRouteStep>();

            string query = DbSHM.QueryBuilder(models, Permissions.ShuttleRequestRoute.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbSHM.Database.SqlQuery<dataShuttleRequestRouteStep>(query).ToList();

            foreach (var language in languages)
            {
                DeletedShuttleRequestRouteSteps.Add(DbSHM.Delete(language, ExecutionTime, Permissions.ShuttleRequestRoute.DeleteGuid, DbCMS));
            }

            return DeletedShuttleRequestRouteSteps;
        }

        private List<dataShuttleRequestRouteStep> RestoreShuttleRequestRouteSteps(List<dataShuttleRequestRouteStep> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataShuttleRequestRouteStep> RestoredLanguages = new List<dataShuttleRequestRouteStep>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbSHM.QueryBuilder(models, Permissions.ShuttleRequestRoute.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbSHM.Database.SqlQuery<dataShuttleRequestRouteStep>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveShuttleRequestRouteStep(language))
                {
                    RestoredLanguages.Add(DbSHM.Restore(language, Permissions.ShuttleRequestRoute.DeleteGuid, Permissions.ShuttleRequestRoute.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyShuttleRequestRouteStep(Guid PK)
        {
            dataShuttleRequestRouteStep dbModel = new dataShuttleRequestRouteStep();

            var Language = DbSHM.dataShuttleRequestRouteStep.Where(l => l.ShuttleRequestRouteStepGUID == PK).FirstOrDefault();
            var dbLanguage = DbSHM.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataShuttleRequestRouteStepRowVersion.SequenceEqual(dbModel.dataShuttleRequestRouteStepRowVersion))
            {
                return Json(DbSHM.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbSHM, dbModel, "LanguagesContainer"));
        }

        private bool ActiveShuttleRequestRouteStep(dataShuttleRequestRouteStep model)
        {
            int LanguageID = DbSHM.dataShuttleRequestRouteStep
                                  .Where(x => 
                                              x.ShuttleRequestRouteGUID == model.ShuttleRequestRouteGUID &&
                                              x.ShuttleRequestRouteStepGUID != model.ShuttleRequestRouteStepGUID &&
                                              x.StartLocationGUID == model.StartLocationGUID &&
                                              x.EndLocationGUID == model.EndLocationGUID &&
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