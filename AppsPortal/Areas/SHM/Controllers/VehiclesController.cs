using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using SHM_DAL.Model;
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
    public class VehicleController : SHMBaseController
    {
        #region Vehicle

        public ActionResult Index()
        {
            return View();
        }

        [Route("SHM/Vehicles/")]
        public ActionResult VehiclesIndex()
        {
            if (!CMS.HasAction(Permissions.Vehicle.Access, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/SHM/Views/Vehicles/Index.cshtml");
        }

        //[Route("SHM/VehiclesDataTable/")]
        public JsonResult VehiclesDataTable(DataTableRecievedOptions options)
        {
            //var app = DbSHM.codeVehicle.ToList();
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<VehiclesDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<VehiclesDataTableModel>(DataTable.Filters);
            }

            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.Shuttle.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();

            var All = (from a in DbSHM.codeVehicle.AsExpandable().Where(x => AuthorizedList.Contains(x.DutyStationGUID.Value.ToString()))
                       join b in DbSHM.codeDutyStationsLanguages.Where(x=>x.LanguageID==LAN) on a.DutyStationGUID equals b.DutyStationGUID
                       join c in DbSHM.codeOrganizationsInstancesLanguages.Where(x => x.LanguageID == LAN) on a.OrganizationInstanceGUID equals c.OrganizationInstanceGUID

                       select new VehiclesDataTableModel
                       {
                           VehicleGUID = a.VehicleGUID,
                           VehicleNumber = a.VehicleNumber,
                           Available = a.Available,
                           Comment = a.Comment,
                           DutyStationDescription = b.DutyStationDescription,
                           OrganizationInstanceDescription = c.OrganizationInstanceDescription,
                           DutyStationGUID = b.DutyStationGUID.ToString(),
                           OrganizationInstanceGUID = c.OrganizationInstanceGUID.ToString(),
                           Active = a.Active,
                           codeVehicleRowVersion = a.codeVehicleRowVersion,
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<VehiclesDataTableModel> Result = Mapper.Map<List<VehiclesDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("SHM/Vehicles/Create/")]
        public ActionResult VehicleCreate()
        {
            if (!CMS.HasAction(Permissions.Vehicle.Create, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/SHM/Views/Vehicles/Vehicle.cshtml", new VehicleUpdateModel());
        }

        [Route("SHM/Vehicles/Update/{PK}")]
        public ActionResult VehicleUpdate(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.Vehicle.Access, Apps.SHM))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var model = (from a in DbSHM.codeVehicle.WherePK(PK)
                         select new VehicleUpdateModel
                         {
                             VehicleGUID = a.VehicleGUID,
                             Active = a.Active,
                             Available = a.Available,
                             Comment = a.Comment,
                             VehicleNumber = a.VehicleNumber,
                             codeVehicleRowVersion = a.codeVehicleRowVersion,
                             DutyStationGUID = a.DutyStationGUID,
                             OrganizationInstanceGUID = a.OrganizationInstanceGUID,
                             ChassisNumber=a.ChassisNumber,
                             EngineNumber=a.EngineNumber,
                             LastRenewalDate=a.LastRenewalDate,
                             LicenseExpiryDate=a.LicenseExpiryDate,
                             ManufacturingYear=a.ManufacturingYear,
                             PlateNumber=a.PlateNumber,
                             VechileModelGUID=a.VechileModelGUID,
                             VehicleTypeGUID=a.VehicleTypeGUID,
                             VehileColorGUID=a.VehileColorGUID
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("Vehicle", "Vehicles", new { Area = "SHM" }));

            return View("~/Areas/SHM/Views/Vehicles/Vehicle.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult VehicleCreate(VehicleUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Vehicle.Create, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveVehicle(model)) return PartialView("~/Areas/SHM/Views/Vehicles/_VehicleForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeVehicle Vehicle = Mapper.Map(model, new codeVehicle());
            Vehicle.VehicleGUID = EntityPK;
            DbSHM.Create(Vehicle, Permissions.Vehicle.CreateGuid, ExecutionTime, DbCMS);

           
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Vehicle.Create, Apps.SHM, new UrlHelper(Request.RequestContext).Action("Create", "Vehicles", new { Area = "SHM" })), Container = "VehicleFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Vehicle.Update, Apps.SHM), Container = "VehicleFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Vehicle.Delete, Apps.SHM), Container = "VehicleFormControls" });

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleCreateMessage(DbSHM.PrimaryKeyControl(Vehicle), DbSHM.RowVersionControls(new List<codeVehicle>() { Vehicle}), null, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult VehicleUpdate(VehicleUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Vehicle.Update, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveVehicle(model)) return PartialView("~/Areas/SHM/Views/Vehicles/_VehicleForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            var Vehicle = DbSHM.codeVehicle.Where(x => x.VehicleGUID == model.VehicleGUID).FirstOrDefault();
            Vehicle = Mapper.Map(model, Vehicle);
            DbSHM.Update(Vehicle, Permissions.Vehicle.UpdateGuid, ExecutionTime, DbCMS);
            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleUpdateMessage(null, null, DbSHM.RowVersionControls(DbSHM.RowVersionControls(Portal.SingleToList(Vehicle)))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyVehicle(model.VehicleGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult VehicleDelete(codeVehicle model)
        {
            if (!CMS.HasAction(Permissions.Vehicle.Delete, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeVehicle> DeletedVehicle = DeleteVehicles(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.Vehicle.Restore, Apps.SHM), Container = "VehicleFormControls" });

            try
            {
                int CommitedRows = DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleDeleteMessage(CommitedRows, DeletedVehicle.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyVehicle(model.VehicleGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult VehicleRestore(codeVehicle model)
        {
            if (!CMS.HasAction(Permissions.Vehicle.Restore, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveVehicle(model))
            {
                return Json(DbSHM.RecordExists());
            }

            List<codeVehicle> RestoredVehicles = RestoreVehicles(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Vehicle.Create, Apps.SHM, new UrlHelper(Request.RequestContext).Action("Vehicles/Create", "Vehicle", new { Area = "SHM" })), Container = "VehicleFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Vehicle.Update, Apps.SHM), Container = "VehicleFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Vehicle.Delete, Apps.SHM), Container = "VehicleFormControls" });

            try
            {
                int CommitedRows = DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleRestoreMessage(CommitedRows, RestoredVehicles, DbSHM.PrimaryKeyControl(RestoredVehicles.FirstOrDefault()), null, null, UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyVehicle(model.VehicleGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult VehiclesDataTableDelete(List<codeVehicle> models)
        {
            if (!CMS.HasAction(Permissions.Vehicle.Delete, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeVehicle> DeletedVehicles = DeleteVehicles(models);

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.PartialDeleteMessage(DeletedVehicles, models, DataTableNames.VehiclesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult VehiclesDataTableRestore(List<codeVehicle> models)
        {
            if (!CMS.HasAction(Permissions.Vehicle.Restore, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeVehicle> RestoredVehicles = RestoreVehicles(models);

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.PartialRestoreMessage(RestoredVehicles, models, DataTableNames.VehiclesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        private List<codeVehicle> DeleteVehicles(List<codeVehicle> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<codeVehicle> DeletedVehicles = new List<codeVehicle>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbSHM.QueryBuilder(models, Permissions.Vehicle.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbSHM.Database.SqlQuery<codeVehicle>(query).ToList();
            foreach (var record in Records)
            {
                DeletedVehicles.Add(DbSHM.Delete(record, ExecutionTime, Permissions.Vehicle.DeleteGuid, DbCMS));
            }

           
            return DeletedVehicles;
        }

        private List<codeVehicle> RestoreVehicles(List<codeVehicle> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<codeVehicle> RestoredVehicles = new List<codeVehicle>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbSHM.QueryBuilder(models, Permissions.Vehicle.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbSHM.Database.SqlQuery<codeVehicle>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveVehicle(record))
                {
                    RestoredVehicles.Add(DbSHM.Restore(record, Permissions.Vehicle.DeleteGuid, Permissions.Vehicle.RestoreGuid, RestoringTime, DbCMS));
                }
            }

           

            return RestoredVehicles;
        }

        private JsonResult ConcurrencyVehicle(Guid PK)
        {
            VehicleUpdateModel dbModel = new VehicleUpdateModel();

            var Vehicle = DbSHM.codeVehicle.Where(x => x.VehicleGUID == PK).FirstOrDefault();
            var dbVehicle = DbSHM.Entry(Vehicle).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbVehicle, dbModel);

            if (Vehicle.codeVehicleRowVersion.SequenceEqual(dbModel.codeVehicleRowVersion))
            {
                return Json(DbSHM.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbSHM, dbModel, "LanguagesContainer"));
        }

        private bool ActiveVehicle(Object model)
        {
            codeVehicle Vehicle = Mapper.Map(model, new codeVehicle());
            int VehicleDescription = DbSHM.codeVehicle
                                    .Where(x => x.VehicleGUID!=Vehicle.VehicleGUID &&
                                    x.VehicleNumber == Vehicle.VehicleNumber &&
                                                x.Active).Count();
            if (VehicleDescription > 0)
            {
                ModelState.AddModelError("VehicleNumber", "Vehicle is already exists");
            }
            return (VehicleDescription > 0);
        }

        #endregion
    }
}