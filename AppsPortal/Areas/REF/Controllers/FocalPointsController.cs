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
    public class FocalPointsController : REFBaseController
    {
        #region Focal Points

        public ActionResult Index()
        {
            return View();
        }

        [Route("REF/FocalPoints/")]
        public ActionResult FocalPointsIndex()
        {
            if (!CMS.HasAction(Permissions.FocalPointsManagement.Access, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/REF/Views/FocalPoints/Index.cshtml");
        }

        [Route("REF/FocalPointsDataTable/")]
        public JsonResult FocalPointsDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<FocalPointsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<FocalPointsDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.FocalPointsManagement.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();


            var All = (from a in DbREF.configFocalPoint.AsExpandable()
                       join c in DbREF.codeDepartmentsConfigurations on a.DepartmentConfigurationGUID equals c.DepartmentConfigurationGUID
                       join d in DbREF.codeDepartmentsLanguages.Where(x=>x.Active && x.LanguageID==LAN) on c.DepartmentGUID equals d.DepartmentGUID into LJ1
                       join e in DbREF.codeApplicationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.ApplicationGUID equals e.ApplicationGUID into LJ2
                       join f in DbREF.codeDutyStationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DutyStationGUID equals f.DutyStationGUID into LJ3
                       from R1 in LJ1.DefaultIfEmpty()
                       from R2 in LJ2.DefaultIfEmpty()
                       from R3 in LJ3.DefaultIfEmpty()
                       select new FocalPointsDataTableModel
                       {
                           FocalPointGUID = a.FocalPointGUID,
                           
                           DepartmentGUID=c.DepartmentGUID.ToString(),
                           DepartmentDescription = R1.DepartmentDescription,
                           ApplicationGUID = a.ApplicationGUID.ToString(),
                           ApplicationDescription = R2.ApplicationDescription,
                           DutyStationGUID = a.DutyStationGUID.ToString(),
                           DutyStationDescription = R3.DutyStationDescription,
                           Active = a.Active,
                           configFocalPointRowVersion = a.configFocalPointRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<FocalPointsDataTableModel> Result = Mapper.Map<List<FocalPointsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("REF/FocalPoints/Create/")]
        public ActionResult FocalPointsCreate()
        {
            if (!CMS.HasAction(Permissions.FocalPointsManagement.Access, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/REF/Views/FocalPoints/FocalPoint.cshtml", new configFocalPoint());
        }

        [Route("REF/FocalPoints/Update/{PK}")]
        public ActionResult FocalPointsUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.FocalPointsManagement.Access, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbREF.configFocalPoint.WherePK(PK)
                         select a).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("FocalPoints", "FocalPoints", new { Area = "REF" }));

            return View("~/Areas/REF/Views/FocalPoints/FocalPoint.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FocalPointsCreate(configFocalPoint model)
        {
            if (!CMS.HasAction(Permissions.FocalPointsManagement.Create, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveFocalPoints(model)) return PartialView("~/Areas/REF/Views/FocalPoints/_FocalPointForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            configFocalPoint FocalPoints = Mapper.Map(model, new configFocalPoint());
            FocalPoints.FocalPointGUID = EntityPK;
            DbREF.Create(FocalPoints, Permissions.FocalPointsManagement.CreateGuid, ExecutionTime, DbCMS);

            configFocalPointStaff FocalPointStaff = Mapper.Map(model, new configFocalPointStaff());
            FocalPointStaff.FocalPointGUID = EntityPK;

            DbREF.Create(FocalPointStaff, Permissions.FocalPointsManagement.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.FocalPointStaffsDataTable, ControllerContext, "FocalPointStaffsContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.FocalPointsManagement.Create, Apps.REF, new UrlHelper(Request.RequestContext).Action("Create", "FocalPoints", new { Area = "REF" })), Container = "FocalPointsFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.FocalPointsManagement.Update, Apps.REF), Container = "FocalPointsFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.FocalPointsManagement.Delete, Apps.REF), Container = "FocalPointsFormControls" });

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleCreateMessage(DbREF.PrimaryKeyControl(FocalPoints), DbREF.RowVersionControls(FocalPoints, FocalPointStaff), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FocalPointsUpdate(configFocalPoint model)
        {
            if (!CMS.HasAction(Permissions.FocalPointsManagement.Update, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveFocalPoints(model)) return PartialView("~/Areas/REF/Views/FocalPoints/_FocalPointsForm.cshtml", model);
            DateTime ExecutionTime = DateTime.Now;

            DbREF.Update(model, Permissions.FocalPointsManagement.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleUpdateMessage(null, null, DbREF.RowVersionControls(new List<configFocalPoint> { model })));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyFocalPoints(model.FocalPointGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FocalPointsDelete(configFocalPoint model)
        {
            if (!CMS.HasAction(Permissions.FocalPointsManagement.Delete, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<configFocalPoint> DeletedFocalPoints = DeleteFocalPoints(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.FocalPointsManagement.Restore, Apps.REF), Container = "FocalPointFormControls" });

            try
            {
                int CommitedRows = DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleDeleteMessage(CommitedRows, DeletedFocalPoints.FirstOrDefault(), "FocalPointStaffsContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyFocalPoints(model.FocalPointGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FocalPointsRestore(configFocalPoint model)
        {
            if (!CMS.HasAction(Permissions.FocalPointsManagement.Restore, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveFocalPoints(model))
            {
                return Json(DbREF.RecordExists());
            }

            List<configFocalPoint> RestoredFocalPoints = RestoreFocalPoints(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.FocalPointsManagement.Create, Apps.REF, new UrlHelper(Request.RequestContext).Action("FocalPointsCreate", "Configuration", new { Area = "REF" })), Container = "FocalPointFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.FocalPointsManagement.Update, Apps.REF), Container = "FocalPointFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.FocalPointsManagement.Delete, Apps.REF), Container = "FocalPointFormControls" });

            try
            {
                int CommitedRows = DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleRestoreMessage(CommitedRows, RestoredFocalPoints, DbREF.PrimaryKeyControl(RestoredFocalPoints.FirstOrDefault()), Url.Action(DataTableNames.FocalPointStaffsDataTable, Portal.GetControllerName(ControllerContext)), "FocalPointStaffsContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyFocalPoints(model.FocalPointGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult FocalPointsDataTableDelete(List<configFocalPoint> models)
        {
            if (!CMS.HasAction(Permissions.FocalPointsManagement.Delete, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<configFocalPoint> DeletedFocalPoints = DeleteFocalPoints(models);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.PartialDeleteMessage(DeletedFocalPoints, models, DataTableNames.FocalPointsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult FocalPointsDataTableRestore(List<configFocalPoint> models)
        {
            if (!CMS.HasAction(Permissions.FocalPointsManagement.Restore, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<configFocalPoint> RestoredFocalPoints = RestoreFocalPoints(models);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.PartialRestoreMessage(RestoredFocalPoints, models, DataTableNames.FocalPointsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        private List<configFocalPoint> DeleteFocalPoints(List<configFocalPoint> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<configFocalPoint> DeletedFocalPoints = new List<configFocalPoint>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbREF.QueryBuilder(models, Permissions.FocalPointsManagement.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbREF.Database.SqlQuery<configFocalPoint>(query).ToList();
            foreach (var record in Records)
            {
                DeletedFocalPoints.Add(DbREF.Delete(record, ExecutionTime, Permissions.FocalPointsManagement.DeleteGuid, DbCMS));
            }

            var FocalPointStaffs = DeletedFocalPoints.SelectMany(a => a.configFocalPointStaff).Where(l => l.Active).ToList();
            foreach (var language in FocalPointStaffs)
            {
                DbREF.Delete(language, ExecutionTime, Permissions.FocalPointsManagement.DeleteGuid, DbCMS);
            }
            return DeletedFocalPoints;
        }

        private List<configFocalPoint> RestoreFocalPoints(List<configFocalPoint> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<configFocalPoint> RestoredFocalPoints = new List<configFocalPoint>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbREF.QueryBuilder(models, Permissions.FocalPointsManagement.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbREF.Database.SqlQuery<configFocalPoint>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveFocalPoints(record))
                {
                    RestoredFocalPoints.Add(DbREF.Restore(record, Permissions.FocalPointsManagement.DeleteGuid, Permissions.FocalPointsManagement.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var FocalPointStaffs = RestoredFocalPoints.SelectMany(x => x.configFocalPointStaff.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in FocalPointStaffs)
            {
                DbREF.Restore(language, Permissions.FocalPointsManagement.DeleteGuid, Permissions.FocalPointsManagement.RestoreGuid, RestoringTime, DbCMS);
            }

            return RestoredFocalPoints;
        }

        private JsonResult ConcurrencyFocalPoints(Guid PK)
        {
            configFocalPoint dbModel = new configFocalPoint();

            var FocalPoints = DbREF.configFocalPoint.Where(x => x.FocalPointGUID == PK).FirstOrDefault();
            var dbFocalPoints = DbREF.Entry(FocalPoints).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbFocalPoints, dbModel);


            if (FocalPoints.configFocalPointRowVersion.SequenceEqual(dbModel.configFocalPointRowVersion) )
            {
                return Json(DbREF.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbREF, dbModel, "FocalPointStaffsContainer"));
        }

        private bool ActiveFocalPoints(configFocalPoint model)
        {
            configFocalPoint FocalPoints = Mapper.Map(model, new configFocalPoint());
            int FocalPointsDescription = DbREF.configFocalPoint
                                    .Where(x => 
                                             x.ApplicationGUID==model.ApplicationGUID 
                                          && x.DutyStationGUID==model.DutyStationGUID 
                                          && x.DepartmentConfigurationGUID==model.DepartmentConfigurationGUID 
                                          && x.SiteCategoryGUID == model.SiteCategoryGUID
                                          && x.FocalPointGUID!= model.FocalPointGUID ).Count();
            if (FocalPointsDescription > 0)
            {
                ModelState.AddModelError("ApplicationGUID", "FocalPoints is already exists");
                //ModelState.AddModelError("DutyStationGUID", "");
                //ModelState.AddModelError("DepartmentConfigurationGUID", "");
            }
            return (FocalPointsDescription > 0);
        }

        #endregion

        #region Focal Point Staff

        //[Route("REF/FocalPointStaffsDataTable/{PK}")]
        public ActionResult FocalPointStaffsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/REF/Views/FocalPoints/_FocalPointStaffsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<FocalPointStaffDataTable, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<FocalPointStaffDataTable>(DataTable.Filters);
            }

            var Result = (from a in DbREF.configFocalPointStaff.AsNoTracking().AsExpandable().Where(x => x.FocalPointGUID == PK)
                          join b in DbREF.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.UserGUID equals b.UserGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          select new FocalPointStaffDataTable
                          {
                              FocalPointStaffGUID = a.FocalPointStaffGUID,
                              ActiveUntil = a.ActiveUntil,
                              Active = a.Active,
                              configFocalPointStaffRowVersion = a.configFocalPointStaffRowVersion,
                              FullName = R1.FirstName + " " + R1.Surname
                          }).Where(Predicate);

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult FocalPointStaffCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.FocalPointsManagement.Create, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/REF/Views/FocalPoints/_FocalPointStaffUpdateModal.cshtml",
                new configFocalPointStaff { FocalPointGUID = FK });
        }

        public ActionResult FocalPointStaffUpdate(Guid? PK)
        {
            if (!CMS.HasAction(Permissions.FocalPointsManagement.Access, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/REF/Views/FocalPoints/_FocalPointStaffUpdateModal.cshtml", DbREF.configFocalPointStaff.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FocalPointStaffCreate(configFocalPointStaff model)
        {
            if (!CMS.HasAction(Permissions.FocalPointsManagement.Create, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveFocalPointStaff(model)) return PartialView("~/Areas/REF/Views/FocalPoints/_FocalPointStaffUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbREF.Create(model, Permissions.FocalPointsManagement.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleUpdateMessage(DataTableNames.FocalPointStaffsDataTable, DbREF.PrimaryKeyControl(model), DbREF.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FocalPointStaffUpdate(configFocalPointStaff model)
        {
            if (!CMS.HasAction(Permissions.FocalPointsManagement.Update, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveFocalPointStaff(model)) return PartialView("~/Areas/REF/Views/FocalPoints/_FocalPointStaffUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbREF.Update(model, Permissions.FocalPointsManagement.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleUpdateMessage(DataTableNames.FocalPointStaffsDataTable,
                    DbREF.PrimaryKeyControl(model),
                    DbREF.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyFocalPointStaff(model.FocalPointStaffGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FocalPointStaffDelete(configFocalPointStaff model)
        {
            if (!CMS.HasAction(Permissions.FocalPointsManagement.Delete, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<configFocalPointStaff> DeletedFocalPointStaffs = DeleteFocalPointStaffs(new List<configFocalPointStaff> { model });

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleDeleteMessage(DeletedFocalPointStaffs, DataTableNames.FocalPointStaffsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyFocalPointStaff(model.FocalPointStaffGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FocalPointStaffRestore(configFocalPointStaff model)
        {
            if (!CMS.HasAction(Permissions.FocalPointsManagement.Restore, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveFocalPointStaff(model))
            {
                return Json(DbREF.RecordExists());
            }

            List<configFocalPointStaff> RestoredFocalPointStaffs = RestoreFocalPointStaffs(Portal.SingleToList(model));

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleRestoreMessage(RestoredFocalPointStaffs, DataTableNames.FocalPointStaffsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyFocalPointStaff(model.FocalPointStaffGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult FocalPointStaffsDataTableDelete(List<configFocalPointStaff> models)
        {
            if (!CMS.HasAction(Permissions.FocalPointsManagement.Delete, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<configFocalPointStaff> DeletedFocalPointStaffs = DeleteFocalPointStaffs(models);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.PartialDeleteMessage(DeletedFocalPointStaffs, models, DataTableNames.FocalPointStaffsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult FocalPointStaffsDataTableRestore(List<configFocalPointStaff> models)
        {
            if (!CMS.HasAction(Permissions.FocalPointsManagement.Restore, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<configFocalPointStaff> RestoredFocalPointStaffs = RestoreFocalPointStaffs(models);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.PartialRestoreMessage(RestoredFocalPointStaffs, models, DataTableNames.FocalPointStaffsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        private List<configFocalPointStaff> DeleteFocalPointStaffs(List<configFocalPointStaff> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<configFocalPointStaff> DeletedFocalPointsFocalPointStaffs = new List<configFocalPointStaff>();

            string query = DbREF.QueryBuilder(models, Permissions.FocalPointsManagement.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbREF.Database.SqlQuery<configFocalPointStaff>(query).ToList();

            foreach (var language in languages)
            {
                DeletedFocalPointsFocalPointStaffs.Add(DbREF.Delete(language, ExecutionTime, Permissions.FocalPointsManagement.DeleteGuid, DbCMS));
            }

            return DeletedFocalPointsFocalPointStaffs;
        }

        private List<configFocalPointStaff> RestoreFocalPointStaffs(List<configFocalPointStaff> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<configFocalPointStaff> RestoredFocalPointStaffs = new List<configFocalPointStaff>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbREF.QueryBuilder(models, Permissions.FocalPointsManagement.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var FocalPointStaffs = DbREF.Database.SqlQuery<configFocalPointStaff>(query).ToList();
            foreach (var language in FocalPointStaffs)
            {
                if (!ActiveFocalPointStaff(language))
                {
                    RestoredFocalPointStaffs.Add(DbREF.Restore(language, Permissions.FocalPointsManagement.DeleteGuid, Permissions.FocalPointsManagement.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredFocalPointStaffs;
        }

        private JsonResult ConcrrencyFocalPointStaff(Guid PK)
        {
            configFocalPointStaff dbModel = new configFocalPointStaff();

            var FocalPointStaff = DbREF.configFocalPointStaff.Where(l => l.FocalPointStaffGUID == PK).FirstOrDefault();
            var dbFocalPointStaff = DbREF.Entry(FocalPointStaff).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbFocalPointStaff, dbModel);

            if (FocalPointStaff.configFocalPointStaffRowVersion.SequenceEqual(dbModel.configFocalPointStaffRowVersion))
            {
                return Json(DbREF.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbREF, dbModel, "FocalPointStaffsContainer"));
        }

        private bool ActiveFocalPointStaff(configFocalPointStaff model)
        {
            int FocalPointStaff = DbREF.configFocalPointStaff
                                  .Where(x => x.FocalPointGUID == model.FocalPointGUID &&
                                              x.FocalPointStaffGUID != model.FocalPointStaffGUID &&
                                              x.UserGUID ==model.UserGUID &&
                                              x.Active).Count();
            if (FocalPointStaff > 0)
            {
                ModelState.AddModelError("UserGUID", "User selected already exists"); //From resource ?????? Amer  
            }

            return (FocalPointStaff > 0);
        }

        #endregion
    }
}