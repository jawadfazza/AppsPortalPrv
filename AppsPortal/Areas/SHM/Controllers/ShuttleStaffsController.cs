using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.SHM.ViewModels;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using SHM_DAL.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.SHM.Controllers
{
    public class ShuttleStaffsController : SHMBaseController
    {
        #region Shuttle Staffs

        //[Route("SHM/ShuttleStaffsDataTable/{PK}")]
        //public ActionResult ShuttleStaffsDataTable(DataTableRecievedOptions options, Guid PK)
        //{
        //    if (options.columns == null) return PartialView("~/Areas/SHM/Views/ShuttleStaffs/_Index.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

        //    DataTableOptions DataTable = ConvertOptions.Fill(options);

        //    Expression<Func<ShuttleStaffDataTable, bool>> Predicate = p => true;

        //    if (DataTable.Filters.FilterRules != null)
        //    {
        //        Predicate = SearchHelper.CreateSearchPredicate<ShuttleStaffDataTable>(DataTable.Filters);
        //    }

        //    var Result = (from a in DbSHM.dataShuttleStaff.AsNoTracking().AsExpandable().Where(x => x.ShuttleGUID == PK)
        //                  join b in DbSHM.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.UserGUID equals b.UserGUID into LJ1
        //                  from R1 in LJ1.DefaultIfEmpty()
        //                  join c in DbSHM.codeShuttleTravelPurposeLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.ShuttleTravelPurposeGUID equals c.ShuttleTravelPurposeGUID into LJ2
        //                  from R2 in LJ2.DefaultIfEmpty()
        //                  select new ShuttleStaffDataTable
        //                  {
        //                      ShuttleStaffGUID = a.ShuttleStaffGUID,
        //                      ShuttleGUID = a.ShuttleGUID,
        //                      ShuttleTravelPurposeGUID = R2.ShuttleTravelPurposeGUID,
        //                      TravelPurpose = R2.ShuttleTravelPurposeDescription,
        //                      UserGUID = R1.FirstName + " " + R1.Surname,
        //                      Active = a.Active,
        //                      dataShuttleStaffRowVersion = a.dataShuttleStaffRowVersion,
        //                  }).Where(Predicate);

        //    Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

        //    return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        //}

        public ActionResult ShuttleStaffCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ShuttleStaff.Create, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var shuttle = (from a in DbSHM.dataShuttle
                           join b in DbSHM.dataShuttleVehicle.Where(x => x.ShuttleVehicleGUID == FK ) on a.ShuttleGUID equals b.ShuttleGUID
                           select a).FirstOrDefault();

            return PartialView("~/Areas/SHM/Views/ShuttleStaffs/_ShuttleStaffForm.cshtml",
                new dataShuttleStaff { ShuttleVehicleGUID = FK,DeparturePointGUID= shuttle.DeparturePointGUID, DropOffPointGUID = shuttle.DropOffPointGUID });
        }

        [Route("SHM/ShuttleStaff/Update/{PK}")]
        public ActionResult ShuttleStaffUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ShuttleStaff.Access, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var staff =DbSHM.dataShuttleStaff.Find(PK);
            staff.DeparturePointGUID = staff.dataShuttleVehicle.dataShuttle.DeparturePointGUID;
            staff.DropOffPointGUID = staff.dataShuttleVehicle.dataShuttle.DropOffPointGUID;
            return PartialView("~/Areas/SHM/Views/ShuttleStaffs/_ShuttleStaffForm.cshtml", staff);
        }

        [HttpPost]
        public ActionResult ShuttleStaffCreate(dataShuttleStaffUpdateModel modelMultible)
        {
            if (!CMS.HasAction(Permissions.ShuttleStaff.Create, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            dataShuttleStaff model = new dataShuttleStaff();
            if (modelMultible.UserPassengerGUID == null)
            {
                Mapper.Map(modelMultible, model);
                if (!ModelState.IsValid || ActiveShuttleStaff(model)) return PartialView("~/Areas/SHM/Views/ShuttleStaffs/_ShuttleStaffForm.cshtml", model);

                DateTime ExecutionTime = DateTime.Now;
                model.Confirmed = true;

                DbSHM.Create(model, Permissions.ShuttleStaff.CreateGuid, ExecutionTime, DbCMS);

                try
                {
                    DbSHM.SaveChanges();
                    DbCMS.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(DbSHM.ErrorMessage(ex.Message));
                }
            }
            else
            {
                foreach (Guid guid in modelMultible.UserPassengerGUID)
                {
                    model = new dataShuttleStaff();
                    Mapper.Map(modelMultible, model);
                    model.UserPassengerGUID = guid;
                    if (!ModelState.IsValid || ActiveShuttleStaff(model)) return PartialView("~/Areas/SHM/Views/ShuttleStaffs/_ShuttleStaffForm.cshtml", model);

                    DateTime ExecutionTime = DateTime.Now;
                    model.Confirmed = true;

                    DbSHM.Create(model, Permissions.ShuttleStaff.CreateGuid, ExecutionTime, DbCMS);

                    try
                    {
                        DbSHM.SaveChanges();
                        DbCMS.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return Json(DbSHM.ErrorMessage(ex.Message));
                    }
                }

            }
            return Json(DbSHM.SingleUpdateMessage(DataTableNames.ShuttleStaffsDataTable, DbSHM.PrimaryKeyControl(model), DbSHM.RowVersionControls(Portal.SingleToList(model)), "CalendarRefresh();"));

        }

        [HttpPost]
        public ActionResult ShuttleStaffUpdate(dataShuttleStaff model)
        {
            if (!CMS.HasAction(Permissions.ShuttleStaff.Update, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveShuttleStaff(model)) return PartialView("~/Areas/SHM/Views/ShuttleStaffs/_ShuttleStaffForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbSHM.Update(model, Permissions.ShuttleStaff.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleUpdateMessage(DataTableNames.ShuttleStaffsDataTable,
                    DbSHM.PrimaryKeyControl(model),
                    DbSHM.RowVersionControls(Portal.SingleToList(model)), "CalendarRefresh();"));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyShuttleStaff(model.ShuttleStaffGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost]
        public ActionResult ShuttleStaffUpdateVehicle(dataShuttleStaff model)
        {
            if (!CMS.HasAction(Permissions.ShuttleStaff.Update, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            DateTime ExecutionTime = DateTime.Now;
            var ShuttleStaff = DbSHM.dataShuttleStaff.Where(x => x.ShuttleStaffGUID == model.ShuttleStaffGUID).FirstOrDefault();
            ShuttleStaff.ShuttleVehicleGUID = model.ShuttleVehicleGUID;
            DbSHM.Update(ShuttleStaff, Permissions.ShuttleStaff.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleUpdateMessage(DataTableNames.ShuttleStaffsDataTable,
                    DbSHM.PrimaryKeyControl(model),
                    DbSHM.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyShuttleStaff(model.ShuttleStaffGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleStaffDelete(dataShuttleStaff model)
        {
            if (!CMS.HasAction(Permissions.ShuttleStaff.Delete, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataShuttleStaff> Deleteds = DeleteShuttleStaffs(new List<dataShuttleStaff> { model });

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleDeleteMessage(Deleteds, DataTableNames.ShuttleStaffsDataTable, "CalendarRefresh();"));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyShuttleStaff(model.ShuttleStaffGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        //[HttpPost, ValidateAntiForgeryToken]
        //public ActionResult ShuttleStaffRestore(dataShuttleStaff model)
        //{
        //    if (!CMS.HasAction(Permissions.ShuttleStaff.Restore, Apps.SHM))
        //    {
        //        throw new HttpException(401, "Unauthorized access");
        //    }
        //    if (ActiveShuttleStaff(model))
        //    {
        //        return Json(DbSHM.RecordExists());
        //    }

        //    List<dataShuttleStaff> Restoreds = RestoreShuttleStaffs(Portal.SingleToList(model));

        //    try
        //    {
        //        DbSHM.SaveChanges();
        //        DbCMS.SaveChanges();
        //        return Json(DbSHM.SingleRestoreMessage(Restoreds, DataTableNames.ShuttleStaffsDataTable));
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        return ConcrrencyShuttleStaff(model.ShuttleStaffGUID);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(DbSHM.ErrorMessage(ex.Message));
        //    }
        //}

        //[HttpPost, ValidateAntiForgeryToken]
        //public JsonResult ShuttleStaffsDataTableDelete(List<dataShuttleStaff> models)
        //{
        //    if (!CMS.HasAction(Permissions.ShuttleStaff.Delete, Apps.SHM))
        //    {
        //        throw new HttpException(401, "Unauthorized access");
        //    }
        //    List<dataShuttleStaff> Deleteds = DeleteShuttleStaffs(models);

        //    try
        //    {
        //        DbSHM.SaveChanges();
        //        DbCMS.SaveChanges();
        //        return Json(DbSHM.PartialDeleteMessage(Deleteds, models, DataTableNames.ShuttleStaffsDataTable));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(DbSHM.ErrorMessage(ex.Message));
        //    }
        //}

        //[HttpPost, ValidateAntiForgeryToken]
        //public JsonResult ShuttleStaffsDataTableRestore(List<dataShuttleStaff> models)
        //{
        //    if (!CMS.HasAction(Permissions.ShuttleStaff.Restore, Apps.SHM))
        //    {
        //        throw new HttpException(401, "Unauthorized access");
        //    }
        //    List<dataShuttleStaff> Restoreds = RestoreShuttleStaffs(models);

        //    try
        //    {
        //        DbSHM.SaveChanges();
        //        DbCMS.SaveChanges();
        //        return Json(DbSHM.PartialRestoreMessage(Restoreds, models, DataTableNames.ShuttleStaffsDataTable));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(DbSHM.ErrorMessage(ex.Message));
        //    }
        //}

        private List<dataShuttleStaff> DeleteShuttleStaffs(List<dataShuttleStaff> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataShuttleStaff> DeletedShuttleStaffs = new List<dataShuttleStaff>();

            string query = DbSHM.QueryBuilder(models, Permissions.ShuttleStaff.DeleteGuid, SubmitTypes.Delete, "");

            var staffs = DbSHM.Database.SqlQuery<dataShuttleStaff>(query).ToList();

            foreach (var staff in staffs)
            {
                DeletedShuttleStaffs.Add(DbSHM.Delete(staff, ExecutionTime, Permissions.ShuttleStaff.DeleteGuid, DbCMS));
                var ShuttleRequestStaff = DbSHM.dataShuttleRequestStaff.Where(x => x.ShuttleRequestGUID == staff.ShuttleRequestGUID && x.UserGUID == staff.UserPassengerGUID).FirstOrDefault();
                if (ShuttleRequestStaff != null)
                {
                    if (ShuttleRequestStaff.dataShuttleRequest.DepartureDate > DateTime.Now) { ShuttleRequestStaff.ReferralStatusGUID = ShuttleRequestStatus.Pending; }
                    else { ShuttleRequestStaff.ReferralStatusGUID = ShuttleRequestStatus.Canceled; }
                }
                DbSHM.SaveChanges();
            }


            return DeletedShuttleStaffs;
        }

        //private List<dataShuttleStaff> RestoreShuttleStaffs(List<dataShuttleStaff> models)
        //{
        //    DateTime RestoringTime = DateTime.Now;

        //    List<dataShuttleStaff> Restoreds = new List<dataShuttleStaff>();

        //    //Select the table and all the factors from other tables into one query.
        //    string baseQuery = "";

        //    string query = DbSHM.QueryBuilder(models, Permissions.ShuttleStaff.DeleteGuid, SubmitTypes.Restore, baseQuery);

        //    var staffs = DbSHM.Database.SqlQuery<dataShuttleStaff>(query).ToList();
        //    foreach (var staff in staffs)
        //    {
        //        if (!ActiveShuttleStaff(staff))
        //        {
        //            Restoreds.Add(DbSHM.Restore(staff, Permissions.ShuttleStaff.DeleteGuid, Permissions.ShuttleStaff.RestoreGuid, RestoringTime, DbCMS));
        //            var ShuttleRequestStaff = DbSHM.dataShuttleRequestStaff.Where(x => x.ShuttleRequestGUID == staff.ShuttleRequestGUID && x.UserGUID == staff.UserGUID).FirstOrDefault();
        //            if (ShuttleRequestStaff != null)
        //            {
        //                if (ShuttleRequestStaff.dataShuttleRequest.DepartureDate < DateTime.Now) { ShuttleRequestStaff.ReferralStatusGUID = staff.dataShuttle.ReferralStatusGUID.Value; }
        //                else { ShuttleRequestStaff.ReferralStatusGUID = ShuttleRequestStatus.Progressing; }
        //            }
        //        }
        //    }
        //    DbSHM.SaveChanges();

        //    return Restoreds;
        //}

        private JsonResult ConcrrencyShuttleStaff(Guid PK)
        {
            dataShuttleStaff dbModel = new dataShuttleStaff();

            var ShuttleStaff = DbSHM.dataShuttleStaff.Where(l => l.ShuttleStaffGUID == PK).FirstOrDefault();
            var dbShuttleStaff = DbSHM.Entry(ShuttleStaff).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbShuttleStaff, dbModel);

            if (ShuttleStaff.dataShuttleStaffRowVersion.SequenceEqual(dbModel.dataShuttleStaffRowVersion))
            {
                return Json(DbSHM.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbSHM, dbModel, "TabContainer"));
        }

        private bool ActiveShuttleStaff(dataShuttleStaff model)
        {
            var ShuttleVehicle = DbSHM.dataShuttleVehicle.Where(x => x.ShuttleVehicleGUID == model.ShuttleVehicleGUID).FirstOrDefault();
            int ShuttleStaffID = 0;
            if (!model.IsUNAgencyStaff)
            {
                ShuttleStaffID = DbSHM.dataShuttleStaff
                                      .Where(x => x.dataShuttleVehicle.ShuttleGUID == ShuttleVehicle.ShuttleGUID &&
                                                  x.ShuttleStaffGUID != model.ShuttleStaffGUID &&
                                                  x.UserPassengerGUID == model.UserPassengerGUID &&
                                                  x.Active).Count();
            }
            if (ShuttleStaffID > 0)
            {
                ModelState.AddModelError("UserPassengerGUID", "Passenger already exists"); //From resource ?????? Amer  
            }

            return (ShuttleStaffID > 0);
        }

        [HttpPost]
        public ActionResult CheckShuttleStaff(ShuttleStaffDataTable model)
        {
            string MessageError = "";
            int ShuttleDriverID = DbSHM.dataShuttleStaff
                                  .Where(x => x.dataShuttleVehicle.ShuttleGUID == model.ShuttleGUID &&
                                  x.UserPassengerGUID.ToString() == model.UserPassengerGUID &&
                                  x.ShuttleStaffGUID != model.ShuttleStaffGUID &&
                                              x.Active).Count();
            int PassengerCount = DbSHM.dataShuttleStaff
                                  .Where(x => x.dataShuttleVehicle.ShuttleVehicleGUID == model.ShuttleVehicleGUID &&
                                              x.Active).Count();
            if (ShuttleDriverID > 0)
            {
                MessageError += "Passenger already exists,";
            }
            if (PassengerCount >= 2)
            {
                MessageError += "No more than two Passenger in the same vehicle,";
            }

            return Json(new { MessageError });
        }


        [HttpPost]
        public ActionResult UpdateStaffConfirmation(Guid ShuttleStaffGUID)
        {
            string MessageError = "";
            var shuttleStaff = DbSHM.dataShuttleStaff
                                  .Where(x =>  x.ShuttleStaffGUID == ShuttleStaffGUID &&
                                              x.Active).FirstOrDefault();
            if (shuttleStaff.Confirmed) { shuttleStaff.Confirmed = false; }
            else { shuttleStaff.Confirmed = true; }
            DbSHM.SaveChanges();


            return Json(new { MessageError });
        }

        #endregion
    }
}