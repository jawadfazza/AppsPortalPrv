using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using AppsPortal.SHM.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using SHM_DAL.Model;

namespace AppsPortal.Areas.SHM.Controllers
{
    public class ShuttleVehiclesController : SHMBaseController
    {
        #region Shuttle Vehicles


        public ActionResult ShuttleVehicleCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ShuttleVehicle.Create, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/SHM/Views/ShuttleVehicles/_ShuttleVehicleForm.cshtml",
                new dataShuttleVehicle { ShuttleGUID = FK });
        }
        [Route("SHM/ShuttleVehicle/Update/{PK}")]
        public ActionResult ShuttleVehicleUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ShuttleVehicle.Access, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/SHM/Views/ShuttleVehicles/_ShuttleVehicleForm.cshtml", DbSHM.dataShuttleVehicle.Find(PK));
        }

        [HttpPost]
        public ActionResult ShuttleVehicleCreate(dataShuttleVehicle model)
        {
            if (!CMS.HasAction(Permissions.ShuttleVehicle.Create, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveShuttleVehicle(model)) return PartialView("~/Areas/SHM/Views/ShuttleVehicles/_ShuttleVehicleForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbSHM.Create(model, Permissions.ShuttleVehicle.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleUpdateMessage(DataTableNames.ShuttleVehiclesDataTable, DbSHM.PrimaryKeyControl(model), DbSHM.RowVersionControls(Portal.SingleToList(model)), "CalendarRefresh();"));
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost]
        public ActionResult ShuttleVehicleUpdate(dataShuttleVehicle model)
        {
            if (!CMS.HasAction(Permissions.ShuttleVehicle.Update, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveShuttleVehicle(model)) return PartialView("~/Areas/SHM/Views/ShuttleVehicles/_ShuttleVehicleForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbSHM.Update(model, Permissions.ShuttleVehicle.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleUpdateMessage(DataTableNames.ShuttleVehiclesDataTable,
                    DbSHM.PrimaryKeyControl(model),
                    DbSHM.RowVersionControls(Portal.SingleToList(model)), "CalendarRefresh();"));
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost]
        public ActionResult ShuttleVehicleUpdateShuttle(dataShuttleVehicle model)
        {
            if (!CMS.HasAction(Permissions.ShuttleVehicle.Update, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            DateTime ExecutionTime = DateTime.Now;
            var ShuttleVehicle = DbSHM.dataShuttleVehicle.Where(x => x.ShuttleVehicleGUID == model.ShuttleVehicleGUID).FirstOrDefault();
            ShuttleVehicle.ShuttleGUID = model.ShuttleGUID;
            DbSHM.Update(ShuttleVehicle, Permissions.ShuttleVehicle.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleUpdateMessage(DataTableNames.ShuttleVehiclesDataTable,
                    DbSHM.PrimaryKeyControl(model),
                    DbSHM.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleVehicleDelete(dataShuttleVehicle model)
        {
            if (!CMS.HasAction(Permissions.ShuttleVehicle.Delete, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataShuttleVehicle> Deleteds = DeleteShuttleVehicles(new List<dataShuttleVehicle> { model });

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleDeleteMessage(Deleteds, DataTableNames.ShuttleVehiclesDataTable, "CalendarRefresh();"));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyShuttleVehicle(model.ShuttleVehicleGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

      

        private List<dataShuttleVehicle> DeleteShuttleVehicles(List<dataShuttleVehicle> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataShuttleVehicle> DeletedShuttleVehicles = new List<dataShuttleVehicle>();

            string query = DbSHM.QueryBuilder(models, Permissions.ShuttleVehicle.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbSHM.Database.SqlQuery<dataShuttleVehicle>(query).ToList();

            foreach (var language in languages)
            {
                DeletedShuttleVehicles.Add(DbSHM.Delete(language, ExecutionTime, Permissions.ShuttleVehicle.DeleteGuid, DbCMS));
            }

            return DeletedShuttleVehicles;
        }


        private JsonResult ConcrrencyShuttleVehicle(Guid PK)
        {
            dataShuttleVehicle dbModel = new dataShuttleVehicle();

            var ShuttleVehicle = DbSHM.dataShuttleVehicle.Where(l => l.ShuttleVehicleGUID == PK).FirstOrDefault();
            var dbShuttleVehicle = DbSHM.Entry(ShuttleVehicle).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbShuttleVehicle, dbModel);

            if (ShuttleVehicle.dataShuttleVehicleRowVersion.SequenceEqual(dbModel.dataShuttleVehicleRowVersion))
            {
                return Json(DbSHM.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbSHM, dbModel, "TabContainer"));
        }

        private bool ActiveShuttleVehicle(dataShuttleVehicle model)
        {
            int ShuttleDriverID = DbSHM.dataShuttleVehicle
                                  .Where(x => x.ShuttleGUID == model.ShuttleGUID &&
                                  x.ShuttleVehicleGUID != model.ShuttleVehicleGUID &&
                                  x.UserDriverGUID == model.UserDriverGUID &&
                                  x.UserDriverGUID!=null &&
                                              x.Active).Count();

            if (ShuttleDriverID > 0)
            {
                ModelState.AddModelError("UserGUID", "Driver already exists"); //From resource ?????? Amer  
            }

            int ShuttleVehicleID = DbSHM.dataShuttleVehicle
                                  .Where(x => x.ShuttleGUID == model.ShuttleGUID &&
                                  x.ShuttleVehicleGUID != model.ShuttleVehicleGUID &&
                                  x.VehicleGUID == model.VehicleGUID &&
                                              x.Active).Count();
            if (ShuttleVehicleID > 0)
            {
                ModelState.AddModelError("VehicleGUID", "Vehicle already exists"); //From resource ?????? Amer  
            }

            return (ShuttleVehicleID + ShuttleDriverID > 0);
        }


        [HttpPost]
        public ActionResult CheckShuttleVehicle(dataShuttleVehicle model)
        {
            string MessageError = "";
            int ShuttleDriverID = DbSHM.dataShuttleVehicle
                                  .Where(x => x.ShuttleGUID == model.ShuttleGUID &&

                                  x.UserDriverGUID == model.UserDriverGUID &&
                                  x.UserDriverGUID != null &&
                                              x.Active).Count();
            if (ShuttleDriverID > 0)
            {
                MessageError += "Driver already exists,";
            }

            int ShuttleVehicleID = DbSHM.dataShuttleVehicle
                                  .Where(x => x.ShuttleGUID == model.ShuttleGUID &&

                                  x.VehicleGUID == model.VehicleGUID &&
                                              x.Active).Count();
            if (ShuttleVehicleID > 0)
            {
                MessageError += "Vehicle already exists,";
            }
            return Json(new { MessageError });
        }

        #endregion
    }
}