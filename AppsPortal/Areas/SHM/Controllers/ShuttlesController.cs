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
using System.Net;
using System.Web;
using System.Web.Mvc;
using SHM_DAL.Model;
using OfficeOpenXml;
using System.IO;
using iTextSharp.text.pdf.qrcode;
using System.Collections;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace AppsPortal.Areas.SHM.Controllers
{
    public class ShuttlesController : SHMBaseController
    {
        #region Shuttles
        public ActionResult Index()
        {
            return View();
        }

        [Route("SHM/Shuttles/")]
        public ActionResult ShuttlesIndex()
        {
            if (!CMS.HasAction(Permissions.Shuttle.Access, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/SHM/Views/Shuttles/Index.cshtml");
        }


        [Route("SHM/Shuttles/Create")]
        public ActionResult ShuttleCreate()
        {
            if (!CMS.HasAction(Permissions.Shuttle.Create, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.Shuttle.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();

            DateTime date = Convert.ToDateTime(Request.Params["date"]);
            Guid OrganizationInstanceGUID = Guid.Parse(Session[SessionKeys.OrganizationInstanceGUID].ToString());
            var userProfile = DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            var staffCore = DbSHM.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            if (userProfile != null)
            {
                return PartialView("~/Areas/SHM/Views/Shuttles/_ShuttleForm.cshtml", new ShuttleUpdateModel()
                {
                    OrganizationInstanceGUID = OrganizationInstanceGUID,
                    DutyStationGUID = AuthorizedList.Count()==1? Guid.Parse(AuthorizedList[0]):staffCore.DutyStationGUID,
                    DepartureDateTime = date,
                    ReturnDateTime = date,
                });
            }
            else { return PartialView("~/Areas/SHM/Views/Shuttles/_ShuttleForm.cshtml", new ShuttleUpdateModel()); }
        }



        [Route("SHM/Shuttles/Update/{PK}")]
        public ActionResult ShuttleUpdate(Guid PK, bool? Driver)
        {
            if (!CMS.HasAction(Permissions.Shuttle.Access, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbSHM.dataShuttle.WherePK(PK)
                         join b in DbSHM.codeLocations on a.StartLocationGUID equals b.LocationGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         join c in DbSHM.codeLocations on a.EndLocationGUID equals c.LocationGUID into LJ2
                         from R2 in LJ2.DefaultIfEmpty()
                         select new ShuttleUpdateModel
                         {
                             ShuttleGUID = a.ShuttleGUID,
                             DepartureDateTime = a.DepartureDateTime,
                             ReturnDateTime = a.ReturnDateTime,
                             DutyStationGUID = a.DutyStationGUID,
                             OrganizationInstanceGUID = a.OrganizationInstanceGUID,
                             CountryGUID = R1.CountryGUID,
                             CountryGUID1 = R2.CountryGUID,
                             StartLocationGUID = a.StartLocationGUID,
                             EndLocationGUID = a.EndLocationGUID,
                             DeparturePointGUID = a.DeparturePointGUID,
                             DropOffPointGUID = a.DropOffPointGUID,
                             PassByLocations = a.PassByLocations,
                             DeparturePointFreeText = a.DeparturePointFreeText,
                             DropOffPointFreeText = a.DropOffPointFreeText,
                             Active = a.Active,
                             dataShuttleRowVersion = a.dataShuttleRowVersion,
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("Shuttle", "Shuttles", new { Area = "SHM" }));

            return PartialView("~/Areas/SHM/Views/Shuttles/_ShuttleForm.cshtml", model);
        }



        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleCreate(ShuttleUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Shuttle.Create, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveShuttle(model)) return PartialView("~/Areas/SHM/Views/Shuttles/_ShuttleForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            dataShuttle Shuttle = Mapper.Map(model, new dataShuttle());
            Shuttle.ShuttleGUID = EntityPK;
            DbSHM.Create(Shuttle, Permissions.Shuttle.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ShuttleStaffsDataTable, "ShuttleStaffs", "TabContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Shuttle.Create, Apps.SHM, new UrlHelper(Request.RequestContext).Action("Create", "Shuttles", new { Area = "SHM" })), Container = "ShuttleFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Shuttle.Update, Apps.SHM), Container = "ShuttleFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Shuttle.Delete, Apps.SHM), Container = "ShuttleFormControls" });
            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbSHM.SingleCreateMessage(DbSHM.PrimaryKeyControl(Shuttle),
                    DbSHM.RowVersionControls(new List<dataShuttle>() { Shuttle }), Partials, "CalendarRefresh();", UIButtons));

            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost]
        public ActionResult ShuttleUpdate(ShuttleUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Shuttle.Update, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveShuttle(model)) return PartialView("~/Areas/SHM/Views/Shuttles/_ShuttleForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            dataShuttle Shuttle = DbSHM.dataShuttle.Where(x => x.ShuttleGUID == model.ShuttleGUID).FirstOrDefault();
            var staffs = (from a in DbSHM.dataShuttleStaff.Where(x => x.DropOffPointGUID == Shuttle.DropOffPointGUID && x.DeparturePointGUID == Shuttle.DeparturePointGUID)
                          join b in DbSHM.dataShuttleVehicle.Where(x => x.Active && x.ShuttleGUID == Shuttle.ShuttleGUID) on a.ShuttleVehicleGUID equals b.ShuttleVehicleGUID
                          select a).ToList();
            Mapper.Map(model, Shuttle);
            DbSHM.Update(Shuttle, Permissions.Shuttle.UpdateGuid, ExecutionTime, DbCMS);

            foreach (var staff in staffs)
            {
                staff.DeparturePointGUID = Shuttle.DeparturePointGUID;
                staff.DropOffPointGUID = Shuttle.DropOffPointGUID;
            }
            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbSHM.SingleUpdateMessage(null, null, DbSHM.RowVersionControls(new List<dataShuttle>() { Shuttle }), "CalendarRefresh();"));
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost]
        public ActionResult ShuttleUpdateDate(ShuttleUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Shuttle.Update, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            DateTime ExecutionTime = DateTime.Now;

            dataShuttle Shuttle = DbSHM.dataShuttle.Where(x => x.ShuttleGUID == model.ShuttleGUID).FirstOrDefault();
            DateTime current = Shuttle.DepartureDateTime;
            Shuttle.DepartureDateTime = model.DepartureDateTime.Value.AddHours(current.Hour).AddMinutes(current.Minute);
            DbSHM.Update(Shuttle, Permissions.Shuttle.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbSHM.SingleUpdateMessage(null, null, DbSHM.RowVersionControls(new List<dataShuttle>() { Shuttle })));
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleDelete(dataShuttle model)
        {
            if (!CMS.HasAction(Permissions.Shuttle.Delete, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataShuttle> DeletedShuttle = DeleteShuttles(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.Shuttle.Restore, Apps.SHM), Container = "ShuttleFormControls" });

            try
            {
                int CommitedRows = DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleDeleteMessage(DeletedShuttle, DataTableNames.ShuttlesDataTable, "CalendarRefresh();"));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyShuttle(model.ShuttleGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        private List<dataShuttle> DeleteShuttles(List<dataShuttle> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataShuttle> DeletedShuttles = new List<dataShuttle>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";// "SELECT ShuttleGUID,CONVERT(varchar(50), OrganizationInstanceGUID) as C2 ,dataShuttleRowVersion FROM ams.dataShuttle where ShuttleGUID in (" + string.Join(",", models.Select(x => "'" + x.ShuttleGUID + "'").ToArray()) + ")";


            string query = DbSHM.QueryBuilder(models, Permissions.Shuttle.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbSHM.Database.SqlQuery<dataShuttle>(query).ToList();
            foreach (var record in Records)
            {
                DeletedShuttles.Add(DbSHM.Delete(record, ExecutionTime, Permissions.Shuttle.DeleteGuid, DbCMS));
            }


            var ShuttleVehicles = DeletedShuttles.SelectMany(a => a.dataShuttleVehicle).Where(l => l.Active).ToList();
            foreach (var ShuttleVehicle in ShuttleVehicles)
            {
                DbSHM.Delete(ShuttleVehicle, ExecutionTime, Permissions.Shuttle.DeleteGuid, DbCMS);

            }

            var ShuttleStaffs = ShuttleVehicles.SelectMany(a => a.dataShuttleStaff).Where(l => l.Active).ToList();
            foreach (var ShuttleStaff in ShuttleStaffs)
            {
                DbSHM.Delete(ShuttleStaff, ExecutionTime, Permissions.Shuttle.DeleteGuid, DbCMS);
                var dataShuttleRequestStaffs = DbSHM.dataShuttleRequestStaff.Where(x => x.ShuttleRequestGUID == ShuttleStaff.ShuttleRequestGUID).ToList();
                foreach (var dataShuttleRequestStaff in dataShuttleRequestStaffs)
                {
                    if (dataShuttleRequestStaff != null)
                    {
                        if (dataShuttleRequestStaff.dataShuttleRequest.DepartureDate > DateTime.Now)
                        {
                            if (dataShuttleRequestStaff.dataShuttleRequest.ReturnDateTime == null)
                            {

                                dataShuttleRequestStaff.ReferralStatusGUID = ShuttleRequestStatus.Pending;
                                dataShuttleRequestStaff.dataShuttleRequest.ReferralStatusGUID = ShuttleRequestStatus.Pending;
                            }
                            else if (dataShuttleRequestStaff.dataShuttleRequest.ReturnDateTime != null && dataShuttleRequestStaff.dataShuttleRequest.ReferralStatusGUID == ShuttleRequestStatus.Progressing)
                            {
                                dataShuttleRequestStaff.ReferralStatusGUID = ShuttleRequestStatus.Pending;
                                dataShuttleRequestStaff.dataShuttleRequest.ReferralStatusGUID = ShuttleRequestStatus.Pending;
                            }
                            else if (dataShuttleRequestStaff.dataShuttleRequest.ReturnDateTime != null && dataShuttleRequestStaff.dataShuttleRequest.ReferralStatusGUID == ShuttleRequestStatus.Completed)
                            {
                                dataShuttleRequestStaff.ReferralStatusGUID = ShuttleRequestStatus.Progressing;
                                dataShuttleRequestStaff.dataShuttleRequest.ReferralStatusGUID = ShuttleRequestStatus.Progressing;
                            }
                        }
                        else
                        {
                            dataShuttleRequestStaff.ReferralStatusGUID = ShuttleRequestStatus.Canceled;
                            dataShuttleRequestStaff.dataShuttleRequest.ReferralStatusGUID = ShuttleRequestStatus.Canceled;
                        }
                    }
                }


            }

            return DeletedShuttles;
        }

        private JsonResult ConcurrencyShuttle(Guid PK)
        {
            ShuttleUpdateModel dbModel = new ShuttleUpdateModel();

            var Shuttle = DbSHM.dataShuttle.Where(x => x.ShuttleGUID == PK).FirstOrDefault();
            var dbShuttle = DbSHM.Entry(Shuttle).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbShuttle, dbModel);

            var Language = DbSHM.dataShuttle.Where(x => x.ShuttleGUID == PK).Where(x => x.Active == true).FirstOrDefault();
            var dbLanguage = DbSHM.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Shuttle.dataShuttleRowVersion.SequenceEqual(dbModel.dataShuttleRowVersion) && Language.dataShuttleRowVersion.SequenceEqual(dbModel.dataShuttleRowVersion))
            {
                return Json(DbSHM.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbSHM, dbModel, "TabContainer"));
        }

        private bool ActiveShuttle(Object model)
        {
            dataShuttle Shuttle = Mapper.Map(model, new dataShuttle());
            int ShuttleError = 0;

            if (Shuttle.DutyStationGUID == null)
            {
                ShuttleError += 1;
                ModelState.AddModelError("DutyStationGUID", "Kindly Update Your Profile by Adding Work Duty Station!"); //From resource ?????? Amer 

            }

            return (ShuttleError > 0);
        }
        [HttpGet]
        public ActionResult ShuttleCalendar()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ShuttleRequestShuttleCreate(ShuttleRequestsDataTableModel model)
        {
            if (!CMS.HasAction(Permissions.Shuttle.Create, Apps.SHM))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var request = (from a in DbSHM.dataShuttleRequest
                           join b in DbSHM.dataShuttleRequestBoxState.Where(x => x.ShuttleRequestBoxStateGUID == model.ShuttleRequestBoxStateGUID)
                           on a.ShuttleRequestGUID equals b.ShuttleRequestGUID select a).FirstOrDefault();
            var BoxState = DbSHM.dataShuttleRequestBoxState.Where(x => x.ShuttleRequestBoxStateGUID == model.ShuttleRequestBoxStateGUID).FirstOrDefault();

            Guid ReferralStatus = Guid.Empty;
            //
            if (request.ReferralStatusGUID == ShuttleRequestStatus.Pending) { ReferralStatus = ShuttleRequestStatus.Progressing; }
            else if (request.ReferralStatusGUID == ShuttleRequestStatus.Progressing) { ReferralStatus = ShuttleRequestStatus.Completed; }
            else if (request.ReferralStatusGUID == ShuttleRequestStatus.Completed) { ReferralStatus = ShuttleRequestStatus.Completed; }

            DateTime ExecutionTime = DateTime.Now;

            #region Create Shuttle 
            model.DepartureDate = (BoxState.IsDeparture ? request.DepartureDate : request.ReturnDateTime.Value);
            DateTime sTime = model.DepartureDate;
            DateTime date = (new DateTime(request.DepartureDate.Year, request.DepartureDate.Month, request.DepartureDate.Day).AddHours(23).AddMinutes(59));
            int hours = (date - request.DepartureDate).Hours;
            DateTime eTime = model.DepartureDate.AddHours(hours);

            dataShuttle Shuttle = Mapper.Map(request, new dataShuttle());
            var findShuttle = DbSHM.dataShuttle.Where(x => x.StartLocationGUID == BoxState.StartLocationGUID && x.EndLocationGUID == BoxState.EndLocationGUID && x.Active && x.DepartureDateTime >= sTime && x.DepartureDateTime <= eTime).FirstOrDefault();
            if (findShuttle == null)
            {
                Guid EntityPK = Guid.NewGuid();
                if (!BoxState.IsDeparture)
                {
                    Shuttle.DepartureDateTime = request.ReturnDateTime.Value;
                    Shuttle.StartLocationGUID = BoxState.StartLocationGUID.Value;
                    Shuttle.EndLocationGUID = BoxState.EndLocationGUID.Value;
                    Shuttle.DeparturePointGUID = request.DeparturePointReturnGUID.Value;
                    Shuttle.DropOffPointGUID = request.DropOffPointReturnGUID.Value;
                }
                else if (BoxState.IsDeparture)
                {
                    Shuttle.DepartureDateTime = request.DepartureDate;
                    Shuttle.StartLocationGUID = BoxState.StartLocationGUID.Value;
                    Shuttle.EndLocationGUID = BoxState.EndLocationGUID.Value;
                }
                Shuttle.ShuttleGUID = EntityPK;
                Guid OrganizationInstanceGUID = Guid.Parse(Session[SessionKeys.OrganizationInstanceGUID].ToString());
                var staffCore = DbSHM.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
                Shuttle.OrganizationInstanceGUID = OrganizationInstanceGUID;
                Shuttle.DutyStationGUID = staffCore.DutyStationGUID;

                DbSHM.Create(Shuttle, Permissions.Shuttle.CreateGuid, ExecutionTime, DbCMS);
            }
            else
            {
                Shuttle = findShuttle;
            }

            var boxDroprd = DbSHM.dataShuttleRequestBoxState.Where(x => x.ShuttleRequestBoxStateGUID == model.ShuttleRequestBoxStateGUID).FirstOrDefault();
            if (boxDroprd != null)
            {
                boxDroprd.IsBoxStateDroped = true;
            }
            #endregion

            #region Create Shuttle Vehicle
            dataShuttleVehicle shuttleVehicle = new dataShuttleVehicle();
            shuttleVehicle.ShuttleVehicleGUID = Guid.NewGuid();
            shuttleVehicle.ShuttleGUID = Shuttle.ShuttleGUID;
            DbSHM.Create(shuttleVehicle, Permissions.ShuttleVehicle.CreateGuid, ExecutionTime, DbCMS);
            #endregion

            #region Create Shuttle Staff
            List<dataShuttleStaff> staffs = new List<dataShuttleStaff>();
            var RequestStaffs = DbSHM.dataShuttleRequestStaff.Where(x => x.ShuttleRequestGUID == request.ShuttleRequestGUID && x.Active).ToList();
            foreach (var RequestStaff in RequestStaffs)
            {
                RequestStaff.ReferralStatusGUID = ShuttleRequestStatus.Progressing;
                dataShuttleStaff staff = new dataShuttleStaff();
                staff.ShuttleStaffGUID = Guid.NewGuid();
                staff.ShuttleVehicleGUID = shuttleVehicle.ShuttleVehicleGUID;
                staff.UserPassengerGUID = RequestStaff.UserGUID.Value;
                staff.ShuttleTravelPurposeGUID = request.ShuttleTravelPurposeGUID;
                staff.ShuttleRequestGUID = request.ShuttleRequestGUID;

                staff.Active = true;

                if (!ActiveShuttleStaff(staff))
                {
                    DbSHM.Create(staff, Permissions.ShuttleStaff.CreateGuid, ExecutionTime, DbCMS);
                }
            }
            #endregion

            try
            {
                request.ReferralStatusGUID = ReferralStatus;
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleCreateMessage(new PrimaryKeyControl(), null, null, "CalendarRefresh();", null));

            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage("no Error"));
            }
        }

        private bool ActiveShuttleStaff(dataShuttleStaff model)
        {
            int ShuttleStaffID = DbSHM.dataShuttleStaff
                                  .Where(x => x.ShuttleVehicleGUID == model.ShuttleVehicleGUID &&
                                              x.UserPassengerGUID == model.UserPassengerGUID &&
                                              x.Active).Count();
            if (ShuttleStaffID > 0)
            {
                ModelState.AddModelError("UserGUID", "Staff already exists"); //From resource ?????? Amer  
            }

            return (ShuttleStaffID > 0);
        }

        public class ShuttleEmail
        {
            public string Table { get; set; }
            public string emailsTo { get; set; }
            public string emailsCC { get; set; }
            public string emailsBCC { get; set; }
            public List<Guid?> guids { get; set; }
        }
        [HttpPost]
        public ActionResult ShuttleSend(Guid ShuttleGUID, bool ShareWithMe)
        {
            var SuttleDetails = DbSHM.dataShuttle.Where(x => x.ShuttleGUID == ShuttleGUID).FirstOrDefault();
            ShuttleEmail shuttleEmail = BuldShuttleTable(ShuttleGUID + ",", false, ShareWithMe);
            ShuttleEmail shuttleEmailUNAgancy = BuldShuttleTable(ShuttleGUID + ",", true, ShareWithMe);
            if (ShareWithMe)
            {
                if (shuttleEmail != null) { new Email().SendShuttleCompleted(CMS.GetCurrentUserEmail(UserGUID), "", "", shuttleEmail.Table, SuttleDetails.DepartureDateTime); }
                if (shuttleEmailUNAgancy != null) { new Email().SendShuttleCompleted(CMS.GetCurrentUserEmail(UserGUID), "", "", shuttleEmailUNAgancy.Table, SuttleDetails.DepartureDateTime); }
            }
            else
            {
                if (shuttleEmail != null) {
                    //change the status of the shuttle request to completed.
                    foreach (var sr in DbSHM.dataShuttleRequest.Where(x => shuttleEmail.guids.Contains(x.ShuttleRequestGUID)).ToList())
                    {
                        if (sr.ReturnDateTime == null)
                        {
                            sr.ReferralStatusGUID = ShuttleRequestStatus.Completed;
                            DbSHM.dataShuttleRequestStaff.Where(x => x.ShuttleRequestGUID == sr.ShuttleRequestGUID).ToList().ForEach(x => x.ReferralStatusGUID = ShuttleRequestStatus.Completed);
                        }
                        else
                        {
                            sr.ReferralStatusGUID = ShuttleRequestStatus.Progressing;
                            DbSHM.dataShuttleRequestStaff.Where(x => x.ShuttleRequestGUID == sr.ShuttleRequestGUID).ToList().ForEach(x => x.ReferralStatusGUID = ShuttleRequestStatus.Progressing);
                        }
                    }

                    new Email().SendShuttleCompleted(shuttleEmail.emailsTo, shuttleEmail.emailsCC, shuttleEmail.emailsBCC, shuttleEmail.Table, SuttleDetails.DepartureDateTime);
                }
                if (shuttleEmailUNAgancy != null) { new Email().SendShuttleCompleted(shuttleEmailUNAgancy.emailsTo, shuttleEmailUNAgancy.emailsCC, shuttleEmailUNAgancy.emailsBCC, shuttleEmailUNAgancy.Table, SuttleDetails.DepartureDateTime); }
            }


            return Json(new { Message = "Mail Shared Successfully" });
        }

        [HttpPost]
        public ActionResult ShuttleSendAll(string shuttleGUIDs, bool ShareWithMe)
        {

            //var shuttleGUIDs = DbSHM.dataShuttle.Where(x => ).Select(x=>x.ShuttleGUID.ToString()).ToList();
            if (shuttleGUIDs != "")
            {
                string ShuttleGUID = shuttleGUIDs.Split(',').Where(x => x != "").ToList().FirstOrDefault();
                var SuttleDetails = DbSHM.dataShuttle.Where(x => x.ShuttleGUID.ToString() == ShuttleGUID).FirstOrDefault();

                ShuttleEmail shuttleEmail = BuldShuttleTable(shuttleGUIDs, false, ShareWithMe);
                ShuttleEmail shuttleEmailUNAgancy = BuldShuttleTable(shuttleGUIDs, true, ShareWithMe);
                if (ShareWithMe)
                {
                    if (shuttleEmail != null) { new Email().SendShuttleCompleted(CMS.GetCurrentUserEmail(UserGUID), "", "", shuttleEmail.Table, SuttleDetails.DepartureDateTime); }
                    if (shuttleEmailUNAgancy != null) { new Email().SendShuttleCompleted(CMS.GetCurrentUserEmail(UserGUID), "", "", shuttleEmailUNAgancy.Table, SuttleDetails.DepartureDateTime); }
                }
                else
                {
                    //change the status of the shuttle request to completed.
                    foreach (var sr in DbSHM.dataShuttleRequest.Where(x => shuttleEmail.guids.Contains(x.ShuttleRequestGUID)).ToList())
                    {
                        if (sr.ReturnDateTime == null)
                        {
                            sr.ReferralStatusGUID = ShuttleRequestStatus.Completed;
                            DbSHM.dataShuttleRequestStaff.Where(x => x.ShuttleRequestGUID == sr.ShuttleRequestGUID).ToList().ForEach(x => x.ReferralStatusGUID = ShuttleRequestStatus.Completed);
                        }
                        else
                        {
                            sr.ReferralStatusGUID = ShuttleRequestStatus.Progressing;
                            DbSHM.dataShuttleRequestStaff.Where(x => x.ShuttleRequestGUID == sr.ShuttleRequestGUID).ToList().ForEach(x => x.ReferralStatusGUID = ShuttleRequestStatus.Progressing);
                        }
                    }
                    if (shuttleEmail != null) { new Email().SendShuttleCompleted(shuttleEmail.emailsTo, shuttleEmail.emailsCC, shuttleEmail.emailsBCC, shuttleEmail.Table, SuttleDetails.DepartureDateTime); }
                    if (shuttleEmailUNAgancy != null) { new Email().SendShuttleCompleted(shuttleEmailUNAgancy.emailsTo, shuttleEmailUNAgancy.emailsCC, shuttleEmail.emailsBCC, shuttleEmailUNAgancy.Table, SuttleDetails.DepartureDateTime); }
                }
            }
            return Json(new { Message = "Mail Shared Successfully" });
        }

        public ShuttleEmail BuldShuttleTable(string ShuttleGUIDStr, bool UNAgancy, bool ShareWithMe)
        {
            List<string> ShuttleGUIDs = ShuttleGUIDStr.Split(',').Where(x => x != "").ToList();

            var shuttleInfo = (from a in DbSHM.dataShuttle.Where(x => ShuttleGUIDs.Contains(x.ShuttleGUID.ToString()) && x.Active)
                               join b in DbSHM.dataShuttleVehicle.Where(x => x.Active) on a.ShuttleGUID equals b.ShuttleGUID
                               join c in DbSHM.dataShuttleStaff.Where(x => x.Active && x.Confirmed && x.IsUNAgencyStaff == UNAgancy) on b.ShuttleVehicleGUID equals c.ShuttleVehicleGUID //into LJ0
                               //from R0 in LJ0.DefaultIfEmpty()
                                   //Shuttle Details
                               join e in DbSHM.codeLocationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.StartLocationGUID equals e.LocationGUID into LJ1
                               from R1 in LJ1.DefaultIfEmpty()
                               join f in DbSHM.codeLocationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.EndLocationGUID equals f.LocationGUID into LJ2
                               from R2 in LJ2.DefaultIfEmpty()
                               join g in DbSHM.codeLocationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on c.DeparturePointGUID equals g.LocationGUID into LJ3
                               from R3 in LJ3.DefaultIfEmpty()
                               join h in DbSHM.codeLocationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on c.DropOffPointGUID equals h.LocationGUID into LJ4
                               from R4 in LJ4.DefaultIfEmpty()

                                   //Staff Details
                               join j in DbSHM.StaffCoreData on c.UserPassengerGUID equals j.UserGUID into LJ5
                               from R5 in LJ5.DefaultIfEmpty()
                               join k in DbSHM.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on c.UserPassengerGUID equals k.UserGUID into LJ6
                               from R6 in LJ6.DefaultIfEmpty()
                               join l in DbSHM.codeShuttleTravelPurposeLanguage.Where(x => x.LanguageID == LAN && x.Active) on c.ShuttleTravelPurposeGUID equals l.ShuttleTravelPurposeGUID into LJ7
                               from R7 in LJ7.DefaultIfEmpty()

                                   //Vehicle Details
                               join i in DbSHM.codeVehicle on b.VehicleGUID equals i.VehicleGUID into LJ10
                               from R10 in LJ10.DefaultIfEmpty()
                               join m in DbSHM.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on b.UserDriverGUID equals m.UserGUID into LJ8
                               from R8 in LJ8.DefaultIfEmpty()
                               join n in DbSHM.StaffCoreData on R8.UserGUID equals n.UserGUID into LJ9
                               from R9 in LJ9.DefaultIfEmpty()
                               orderby b.VehicleGUID
                               select new
                               {
                                   ShuttleGUID = a.ShuttleGUID.ToString(),
                                   DepartureDateTime = a.DepartureDateTime,
                                   StartLocation = R1.LocationDescription != null ? R1.LocationDescription : "TBC",
                                   EndLocation = R2.LocationDescription != null ? R2.LocationDescription : "TBC",
                                   DeparturePoint = (R3.LocationDescription != null ? R3.LocationDescription : a.DeparturePointFreeText != "" ? a.DeparturePointFreeText : "TBC"),
                                   DropOffPoint = (R4.LocationDescription != null ? R4.LocationDescription : a.DropOffPointFreeText != "" ? a.DropOffPointFreeText : "TBC"),

                                   DriverGUID = b.IsUNAgencyVehicle ? Guid.Empty : R8.UserGUID,
                                   PlateNumber = b.IsUNAgencyVehicle ? b.UNAgencyVehicleNumber : R10.VehicleNumber,
                                   DriverName = b.IsUNAgencyVehicle ? b.UNAgency : R8.FirstName + " " + R8.Surname,
                                   DriverSyrianPhoneNumber = b.IsUNAgencyVehicle ? b.UNAgencyPhoneNumber : R9.OfficialMobileNumber,
                                   DriverLebanesePhoneNumber = "+" + b.PhoneNumber,


                                   StaffGUID = c.IsUNAgencyStaff ? Guid.Empty : R6.UserGUID,
                                   StaffPhoneNumber = c.IsUNAgencyStaff ? c.UNAgencyPhoneNumber : R5.OfficialMobileNumber,
                                   StaffName = c.IsUNAgencyStaff ? c.UNAgencyStaffName : R6.FirstName + " " + R6.Surname,
                                   TravelPurpose = R7.ShuttleTravelPurposeDescription,
                                   UNAgencyEmailAddress = c.IsUNAgencyStaff ? c.UNAgencyEmailAddress : "",
                                   ShuttleRequestGUID = c.IsUNAgencyStaff ? Guid.Empty : c.ShuttleRequestGUID,
                                   DutystationStaffGUID = c.IsUNAgencyStaff ? Guid.Empty : R5.DutyStationGUID
                               }).OrderBy(x => new { x.StartLocation,x.EndLocation,x.ShuttleGUID, x.DriverName}).ToList();
            if (shuttleInfo.Count != 0)
            {
                var ShareUser = DbSHM.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active && x.UserGUID == UserGUID).FirstOrDefault();
                var ShuttleUpdate = DbSHM.dataShuttle.Where(x => ShuttleGUIDs.Contains(x.ShuttleGUID.ToString())).ToList();
                foreach (var su in ShuttleUpdate)
                {
                    su.SharedBy = (ShareWithMe ? "0" : ShareUser.FirstName + " " + ShareUser.Surname);
                    su.ShareDatetime = DateTime.Now;
                }

                DbSHM.SaveChanges();

                string table = "";
                table += "<p>Dear All,</p>";
                table += "<p>Kindly find below the mission plan for  " + shuttleInfo.FirstOrDefault().DepartureDateTime.ToString("dd-MMM-yyyy") + "</p>";
                table += "<br/>";
                table += "<table style='border-style: solid; border-width: thin; width: 100%;'>";
                table += "" +
             "<tr style='background-color: #F8CBAD; text-align: center;'>" +
                 "<td colspan='10'  style='font-size: small; font-weight: bold;border-style: solid; border-width: 1px;'>UNHCR Shuttle Details</td>" +
                " <td colspan='4'  style='font-size: small; font-weight: bold;border-style: solid; border-width: 1px;'>UNHCR Fleet Details</td>" +
            " </tr>" +
             "<tr style=' background-color: #F8CBAD; text-align: center;border-collapse: collapse;'>" +
               "  <td  style='font-size: small; font-weight: bold;border-style: solid; border-width: 1px;'>Date</td>" +
               "  <td colspan='2'  style='font-size: small; font-weight: bold;border-style: solid; border-width: 1px;'>Route</td>" +
               "  <td  style='font-size: small; font-weight: bold;border-style: solid; border-width: 1px;'>Seq</td>" +
               "  <td  style='font-size: small; font-weight: bold;border-style: solid; border-width: 1px;'>Name</td>" +
               "  <td  style='font-size: small; font-weight: bold;border-style: solid; border-width: 1px;'>Staff mobile No.</td>" +
               "  <td  style='font-size: small; font-weight: bold;border-style: solid; border-width: 1px;'>Departure time</td>" +
               "  <td  style='font-size: small; font-weight: bold;border-style: solid; border-width: 1px;'>Departure point</td>" +
               "  <td  style='font-size: small; font-weight: bold;border-style: solid; border-width: 1px;'>Drop off point</td>" +
               "  <td  style='font-size: small; font-weight: bold;border-style: solid; border-width: 1px;'>Purpose of travel</td>" +
               "  <td  style='font-size: small; font-weight: bold;border-style: solid; border-width: 1px;'>Driver name</td>" +
               "  <td  style='font-size: small; font-weight: bold;border-style: solid; border-width: 1px;'>Local number</td>" +
               "  <td  style='font-size: small; font-weight: bold;border-style: solid; border-width: 1px;'>Other number</td>" +
               "  <td  style='font-size: small; font-weight: bold;border-style: solid; border-width: 1px;'>Vehicle's plate number </td>" +
             "</tr>";

                string row = "" +
                    " <tr style='text-align: center;'>" +
                        "<td  style = 'border-style: none solid none solid; font-size: small; border-right-width: thin; border-left-width: thin;' >#Date</td>" +
                        "<td  style = 'border-style: none solid none solid; font-size: small; border-right-width: thin; border-left-width: thin;' >#RouteFrom</td>" +
                        "<td  style = 'border-style: none solid none solid; font-size: small; border-right-width: thin; border-left-width: thin;' >#RouteTo</td>" +
                        "<td  style = 'border-style: solid; border-width: thin; font-size: small' >#Seq</td>" +
                        "<td  style = 'border-style: solid; border-width: thin; font-size: small' >#Name</td>" +
                        "<td  style = 'border-style: solid; border-width: thin; font-size: small' >#StaffMobileNo</td>" +
                        "<td  style = 'border-style: none solid none solid; font-size: small; border-right-width: thin; border-left-width: thin;' >#DepartureTime</td>" +
                        "<td  style = 'border-style: none solid none solid; font-size: small; border-right-width: thin; border-left-width: thin;' >#DeparturePoint</td>" +
                        "<td  style = 'border-style: none solid none solid; font-size: small; border-right-width: thin; border-left-width: thin;' >#Dropoffpoint</td>" +
                        "<td  style = 'border-style: solid; border-width: thin; font-size: small' >#PurposeOfTravel</td>" +
                        "<td  style = 'border-style: none solid none solid; font-size: small; border-right-width: thin; border-left-width: thin;' >#DriverName</td>" +
                        "<td  style = 'border-style: none solid none solid; font-size: small; border-right-width: thin; border-left-width: thin;' >#SyrianNumber</td>" +
                        "<td  style = 'border-style: none solid none solid; font-size: small; border-right-width: thin; border-left-width: thin;' >#LebaneseNumber</td>" +
                        "<td  style = 'border-style: none solid none solid; font-size: small; border-right-width: thin; border-left-width: thin;' >#PlateNumber</td>" +
                    "</tr > ";
                string OldStyle = "border-style: none solid none solid; font-size: small; border-right-width: thin; border-left-width: thin;";
                string NewStyle = "border-style: solid solid none solid; font-size: small; border-right-width: thin; border-left-width: thin; border-top-width: thin;";
                string LastRow = "border-style: solid solid solid solid; font-size: small; border-width: 1px;";
                string LastRowOpenTop = "border-style: none solid solid solid; font-size: small; border-width: 1px;";

                string DriverName = "";
                string DeparturePoint = "";
                string Dropoffpoint = "";
                string StartLocation = "";
                string EndLocation = "";
                DateTime DepartureTime = new DateTime();
                int seq = 1;
                int index = 1;
                var ColCount = shuttleInfo.Count();
                foreach (var val in shuttleInfo)
                {

                    if (DriverName != val.DriverName || StartLocation != val.StartLocation || EndLocation != val.EndLocation)
                    {
                        seq = 1;

                        DriverName = val.DriverName;
                        DeparturePoint = val.DeparturePoint;
                        Dropoffpoint = val.DropOffPoint;
                        DepartureTime = val.DepartureDateTime;
                        StartLocation = val.StartLocation;
                        EndLocation = val.EndLocation;

                        table += row.Replace("#Date", val.DepartureDateTime.ToString("dd MMM yyyy"))
                      .Replace("#RouteFrom", val.StartLocation)
                       .Replace("#RouteTo", val.EndLocation)
                        .Replace("#Seq", seq.ToString())
                         .Replace("#Name", val.StaffName)
                          .Replace("#StaffMobileNo", val.StaffPhoneNumber)
                           .Replace("#DepartureTime", val.DepartureDateTime.ToShortTimeString())
                            .Replace("#DeparturePoint", val.DeparturePoint)
                             .Replace("#Dropoffpoint", val.DropOffPoint)
                              .Replace("#PurposeOfTravel", val.TravelPurpose)
                               .Replace("#DriverName", val.DriverName)
                                .Replace("#SyrianNumber", val.DriverSyrianPhoneNumber)
                                 .Replace("#LebaneseNumber", val.DriverLebanesePhoneNumber)
                                  .Replace("#PlateNumber", val.PlateNumber)
                                  .Replace(OldStyle, (index == ColCount ? LastRow : NewStyle));

                    }
                    else
                    {
                        table += row.Replace("#Date", "")
                          .Replace("#RouteFrom", "")
                           .Replace("#RouteTo", "")
                            .Replace("#Seq", seq.ToString())
                             .Replace("#Name", val.StaffName)
                              .Replace("#StaffMobileNo", val.StaffPhoneNumber)
                               .Replace("#DepartureTime", DepartureTime != val.DepartureDateTime ? "<hr />" + val.DepartureDateTime.ToShortTimeString() : "")
                                .Replace("#DeparturePoint", DeparturePoint != val.DeparturePoint ? "<hr />" + val.DeparturePoint : "")
                                 .Replace("#Dropoffpoint", Dropoffpoint != val.DropOffPoint ? "<hr />" + val.DropOffPoint : "")
                                  .Replace("#PurposeOfTravel", val.TravelPurpose)
                                   .Replace("#DriverName", "")
                                    .Replace("#SyrianNumber", "")
                                     .Replace("#LebaneseNumber", "")
                                      .Replace("#PlateNumber", "")
                                        .Replace(OldStyle, (index == ColCount ? LastRowOpenTop : OldStyle));
                        DeparturePoint = val.DeparturePoint;
                        Dropoffpoint = val.DropOffPoint;
                        DepartureTime = val.DepartureDateTime;
                        StartLocation = val.StartLocation;
                        EndLocation = val.EndLocation;
                    }
                    seq++;
                    index++;
                }
                table += "</table>";


                var staffCore = DbSHM.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
                var CurrentFleetManager = (from a in DbSHM.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN && x.UserGUID == UserGUID)
                                           join b in DbSHM.StaffCoreData on a.UserGUID equals b.UserGUID
                                           join c in DbSHM.codeJobTitlesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.JobTitleGUID equals c.JobTitleGUID into LJ1
                                           from R1 in LJ1.DefaultIfEmpty()
                                           join d in DbSHM.codeDutyStationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.DutyStationGUID equals d.DutyStationGUID into LJ2
                                           from R2 in LJ2.DefaultIfEmpty()
                                           join e in DbSHM.codeCountriesLanguages.Where(x => x.LanguageID == LAN && x.Active) on R2.codeDutyStations.CountryGUID equals e.CountryGUID
                                           select new
                                           {
                                               b.OfficialMobileNumber,
                                               StaffName = a.FirstName + " " + a.Surname,
                                               R1.JobTitleDescription,
                                               b.OfficialExtensionNumber,
                                               R2.DutyStationDescription,
                                               e.CountryDescription
                                           }).FirstOrDefault();

                table += "<br/><br/>Best Regards<br/>";
                table += "" + CurrentFleetManager.StaffName + "<br/>";
                table += "" + CurrentFleetManager.JobTitleDescription + "<br/>";
                table += "Office: "+ GetLocalExtension(staffCore.OrganizationInstanceGUID.ToString()) + CurrentFleetManager.OfficialExtensionNumber + " | Mobile: " + CurrentFleetManager.OfficialMobileNumber + "<br/>";

                table += "" + CurrentFleetManager.CountryDescription + " , " + CurrentFleetManager.DutyStationDescription + "<br/>";
                //table += "<img src='" + AppSettingsKeys.Domain + "Assets/Images/unhcr.png?v=1' alt='UNHCR logo'>";


                string emailsTo = "";
                string emailsCC = "";
                string emailsBCC = "";
                if (UNAgancy) { emailsTo = string.Join(";", shuttleInfo.Select(x => x.UNAgencyEmailAddress).Distinct().ToArray()); }
                foreach (var guid in shuttleInfo.Select(x => x.StaffGUID).Distinct())
                {
                    emailsTo += CMS.GetCurrentUserEmail(guid) + ";";

                }
                foreach (var guid in shuttleInfo.Select(x => x.DriverGUID).Distinct())
                {
                    emailsCC += CMS.GetCurrentUserEmail(guid) + ";";

                }
                string[] DutystationStaffGUID = shuttleInfo.Select(x => x.DutystationStaffGUID.ToString()).Distinct().ToArray();

                var StartLocationGUIDs = DbSHM.dataShuttle.Where(x => ShuttleGUIDStr.Contains(x.ShuttleGUID.ToString())).Select(x=>x.StartLocationGUID.ToString()).ToList();
                var EndLocationGUIDs = DbSHM.dataShuttle.Where(x => ShuttleGUIDStr.Contains(x.ShuttleGUID.ToString())).Select(x => x.EndLocationGUID.ToString()).ToList();

                string[] DutystationFoicalPointGUID = getDutystationLocation().Where(x => StartLocationGUIDs.Contains(x.LocationGUID) || EndLocationGUIDs.Contains(x.LocationGUID)).Select(x=>x.DutystationGUID).Distinct().ToArray();
               
                string FoaclPointStaffs = string.Join(";", (from a in DbCMS.configFocalPointStaff.Where(x => x.configFocalPoint.ApplicationGUID == Apps.SHM
                                         && (x.configFocalPoint.DutyStationGUID == staffCore.DutyStationGUID
                                         || DutystationStaffGUID.Contains(x.configFocalPoint.DutyStationGUID.ToString())
                                         || DutystationFoicalPointGUID.Contains(x.configFocalPoint.DutyStationGUID.ToString()))

                                         && x.Active && x.configFocalPoint.Active)
                                                            join b in DbCMS.StaffCoreData on a.UserGUID equals b.UserGUID
                                                            select b.EmailAddress).Distinct().ToArray());
                emailsCC += CMS.GetCurrentUserEmail(UserGUID) + ";";
                emailsCC += GetEmailsCC(staffCore.OrganizationInstanceGUID.ToString());
                emailsBCC += GetEmailsBCC(staffCore.OrganizationInstanceGUID.ToString());
                emailsCC += FoaclPointStaffs;

                //chnange the status of the shuttle request to completed.
                List<Guid?> guids = shuttleInfo.Select(x => x.ShuttleRequestGUID).Distinct().ToList();
                return new ShuttleEmail()
                {
                    Table = table,
                    emailsCC = emailsCC,
                    emailsTo = emailsTo,
                    emailsBCC = emailsBCC,
                    guids = guids
                };
            }
            return null;

        }

        private string GetLocalExtension(string organizationInstanceGUID)
        {
            //Syria
            if (organizationInstanceGUID.ToUpper() == "E156C022-EC72-4A5A-BE09-163BD85C68EF")
            {
                return "+963 11 2181 ";
            }
            return "";
        }

        private string GetEmailsBCC(string organizationInstanceGUID)
        {
            string emailsBCC = "";
            //Syria
            if (organizationInstanceGUID.ToUpper() == "E156C022-EC72-4A5A-BE09-163BD85C68EF")
            {
                var staffCore = DbSHM.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
                //Damascues
                if (staffCore.DutyStationGUID.ToString().ToUpper() == "6D7397D6-3D7F-48FC-BFD2-18E69673AC92")
                {
                    emailsBCC = string.Join(";", GetAllMembers("DLDamascus-AllFieldSa").ToArray()) + ";";
                    emailsBCC += string.Join(";", GetAllMembers("DLDamascus-HR").ToArray()) + ";";
                    emailsBCC += string.Join(";", GetAllMembers("dldamascus-admin-trav").ToArray()) + ";";
                    emailsBCC += string.Join(";", GetAllMembers("Damascus - All Drivers").ToArray()) + ";";
                }
                //Qamishli
                if (staffCore.DutyStationGUID.ToString().ToUpper() == "569F0F7F-4405-40E9-BEE8-F654FAC55EFA")
                {
                    emailsBCC = string.Join(";", GetAllMembers("Qamishli - All Drivers").ToArray()) + ";";
                }
            }
            return emailsBCC;
        }

        private string GetEmailsCC(string organizationInstanceGUID)
        {
            string emailsCC = "";
            
            var staffCore = DbSHM.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();

            if (organizationInstanceGUID.ToUpper() == "E156C022-EC72-4A5A-BE09-163BD85C68EF")
            {
                //Damascues
                if (staffCore.DutyStationGUID.ToString().ToUpper() == "6D7397D6-3D7F-48FC-BFD2-18E69673AC92")
                {
                    emailsCC += "DLDamascus-AllFieldSa@unhcr.org;DLDamascus-HR@unhcr.org;Damascus-AllDrivers@unhcr.org;dldamascus-admin-trav@unhcr.org;";
                }
                //Qamishli
                if (staffCore.DutyStationGUID.ToString().ToUpper() == "569F0F7F-4405-40E9-BEE8-F654FAC55EFA")
                {
                    emailsCC += "DLQamishliAllDrivers@unhcr.org";
                }

            }
            return emailsCC;
        }

        public List<LocationMatchDutystaion> getDutystationLocation()
        {
            return new List<LocationMatchDutystaion>(){
                //Syria
                {new LocationMatchDutystaion(){DutystationGUID="6D7397D6-3D7F-48FC-BFD2-18E69673AC92",LocationGUID="00000000-0000-0000-0000-000000000001"} }, //Damascus
                {new LocationMatchDutystaion(){DutystationGUID="6D7397D6-3D7F-48FC-BFD2-18E69673AC94",LocationGUID="00000000-0000-0000-0000-000000000002"} }, //Aleppo
                {new LocationMatchDutystaion(){DutystationGUID="6d7397d6-3d7f-48fc-bfd2-18e69673ac55",LocationGUID="00000000-0000-0000-0000-000000000009"} }, //Deir Ez Zor
                {new LocationMatchDutystaion(){DutystationGUID="be776f40-af39-42de-a54d-e4fd0397fbc0",LocationGUID="00000000-0000-0000-0000-000000000005"} },//Hama
                {new LocationMatchDutystaion(){DutystationGUID="bf5d3be8-df6d-460a-86dc-5eb3a03f9e44",LocationGUID="00000000-0000-0000-0000-000000000004"} },//Homs
                {new LocationMatchDutystaion(){DutystationGUID="569f0f7f-4405-40e9-bee8-f654fac55efa",LocationGUID="1b32da1b-3c87-4ab4-9ca2-f0414de2bad3"} },//Qamishli
                {new LocationMatchDutystaion(){DutystationGUID="9dc67413-952d-4e46-a13d-3f484bc40956",LocationGUID="00000000-0000-0000-0000-000000000013"} },//Sweida
                {new LocationMatchDutystaion(){DutystationGUID="6cd6d68d-eac1-440b-904f-7d34b4fd3863",LocationGUID="00000000-0000-0000-0000-000000000010"} },//Tartous
                //UKRAINE
                {new LocationMatchDutystaion(){DutystationGUID="043C1D7C-62EA-4A90-8B43-8E7209B9E123",LocationGUID="a4449dec-25a5-4d4e-b5d4-17d7162ae566"} },//KYIV
                {new LocationMatchDutystaion(){DutystationGUID="A7DB71A0-D608-4B68-9E89-E932ECA9A9A1",LocationGUID="366c9fa4-7e1f-45f9-ae87-50e0f84fd006"} },//CHERNITSI
                {new LocationMatchDutystaion(){DutystationGUID="9CF2A1E5-D8D1-49F5-B550-522F9AA19E21",LocationGUID="8a88e440-7a90-4ff0-8186-1b741ff7127a"} },//VINNITSYA
                {new LocationMatchDutystaion(){DutystationGUID="A045893D-5946-482F-B3A7-57D2A32A8AE8",LocationGUID="a660ed16-22da-49b9-a84e-a3f5f08860b8"} },//LVIV
                {new LocationMatchDutystaion(){DutystationGUID="AA30E674-D854-4C12-9475-7E23222B0070",LocationGUID="6075f5a0-abb6-44eb-ad1b-d00257e08b3a"} },//DNIPRO
                {new LocationMatchDutystaion(){DutystationGUID="A9D670C8-BC18-4E54-91E9-A6AC107FADA4",LocationGUID="a078ce44-1159-47a7-854e-8c05c9abd1b7"} },//UZHGOROD
            };
        }


        private static List<string> GetAllMembers(string groupName)
        {
            List<string> answer = new List<string>();
            DirectoryEntry entry = new DirectoryEntry("LDAP://unhcr.local/OU=Organisation,DC=UNHCR,DC=LOCAL", "SA-SYRDAFNP-ADM", "6osi6wogo@ih&q+sPabi", AuthenticationTypes.Secure);
            DirectorySearcher ds = new DirectorySearcher(entry);
            ds.Filter = String.Format("(&(cn={0})(objectClass=group))", groupName);
            ds.PropertiesToLoad.Add("member");
            SearchResult sr = ds.FindOne();
            try
            {
                for (int i = 0; i < sr.Properties["member"].Count; i++)
                    answer.Add(sr.Properties["member"][i].ToString().Split(',')[0].Split('=')[1] + "@unhcr.org");
            }
            catch { }
            return answer;
        }


        [HttpPost]
        public ActionResult LocationCreate(LocationsUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Locations.Create, Apps.CMS, model.CountryGUID.ToString()))
            {
                // throw new HttpException(401, "Unauthorized access");
                return Json(DbCMS.ErrorMessage("401 - Unauthorized access"));
            }
            if (!ModelState.IsValid) return Json(DbCMS.ErrorMessage("Model Error"));
            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeLocations Location = Mapper.Map(model, new codeLocations());

            Location.LocationGUID = EntityPK;
            DbSHM.Create(Location, Permissions.Locations.CreateGuid, ExecutionTime, DbCMS);

            codeLocationsLanguages Language = Mapper.Map(model, new codeLocationsLanguages());
            Language.LocationGUID = EntityPK;
            DbSHM.Create(Language, Permissions.LocationsLanguages.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleCreateMessage(DbCMS.PrimaryKeyControl(Location), null, null, "", null));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }
        #endregion

        #region  query 

        [HttpPost]
        public ActionResult getVehicle(string txtStr)
        {
            Guid OrganizationInstanceGUID = Guid.Parse(Session[SessionKeys.OrganizationInstanceGUID].ToString());
            var model = (from a in DbSHM.codeVehicle.Where(x => x.Active && x.VehicleNumber.Contains(txtStr) && x.OrganizationInstanceGUID==OrganizationInstanceGUID)
                         select new
                         {
                             text = a.VehicleNumber,
                             value = a.VehicleGUID
                         }
                                      ).ToList();
            return Json(new { model });
        }

        [HttpPost]
        public ActionResult getStaff(string txtStr)
        {
            Guid staffIsActiveGUID = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3611");
            Guid OrganizationInstanceGUID = Guid.Parse(Session[SessionKeys.OrganizationInstanceGUID].ToString());
            var model = (from a in DbSHM.StaffCoreData.Where(x => x.Active && x.OrganizationInstanceGUID==OrganizationInstanceGUID)
                         join b in DbSHM.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN && (x.FirstName +" "+ x.Surname).Contains(txtStr)) on a.UserGUID equals b.UserGUID
                         where a.StaffStatusGUID == staffIsActiveGUID
                         select new
                         {
                             text = (b.FirstName + " " + b.Surname),
                             value = a.UserGUID
                         }).OrderBy(x => (x.text)).Take(20).ToList();
            return Json(new { model });
        }

        [HttpPost]
        public ActionResult ShuttleRequests(DateTime StartDate, DateTime EndDate)
        {
            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.Shuttle.AccessGuid && x.UserProfileGUID == UserProfileGUID && x.FactorsToken!="").Select(x => x.FactorsToken).Distinct().ToList();

            var ShuttleRequest = (from a in DbSHM.dataShuttleRequest .Where(x =>  x.ReferralStatusGUID == ShuttleRequestStatus.Pending && x.Active && x.DepartureDate >= StartDate && x.DepartureDate <= EndDate).Where(x => AuthorizedList.Contains(x.DutyStationGUID.Value.ToString()))

                                  join b in DbSHM.dataShuttleRequestBoxState.Where(x => !x.IsBoxStateDroped && x.IsDeparture) on   a.ShuttleRequestGUID  equals   b.ShuttleRequestGUID //into LJ1
                                  //from R1 in LJ1.DefaultIfEmpty()

                                  //join c in DbSHM.dataShuttleRequestRouteStep on R1.ShuttleRequestRouteStepGUID equals c.ShuttleRequestRouteStepGUID into LJ2
                                  //from R2 in LJ2.DefaultIfEmpty()
                                 
                                  join e in DbSHM.codeLocationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.StartLocationGUID equals e.LocationGUID into LJ4
                                  from R4 in LJ4.DefaultIfEmpty()
                                  join f in DbSHM.codeLocationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.EndLocationGUID equals f.LocationGUID into LJ5
                                  from R5 in LJ5.DefaultIfEmpty()
                                 
                                  join j in DbSHM.codeShuttleTravelPurposeLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.ShuttleTravelPurposeGUID equals j.ShuttleTravelPurposeGUID
                                  join h in DbSHM.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.UserGUID equals h.UserGUID
                                  orderby a.DepartureDate
                                  select new ShuttleRequestsDataTableModel
                                  {
                                      ShuttleRequestGUID = a.ShuttleRequestGUID,
                                      ShuttleRequestBoxStateGUID= b.ShuttleRequestBoxStateGUID,
                                      DepartureDate = a.DepartureDate,
                                      StartLocation = R4.LocationDescription,
                                      EndLocation = R5.LocationDescription,
                                      ShuttleTravelPurposeDescription = j.ShuttleTravelPurposeDescription,
                                      StaffName = h.FirstName + " " + h.Surname
                                  }).ToList();

            List<Guid> GUIDs = ShuttleRequest.Select(x => x.ShuttleRequestGUID).ToList();
            var ShuttleRequestStaff = (from a in DbSHM.dataShuttleRequestStaff.Where(x =>/* x.dataShuttleRequest.ReferralStatusGUID == ShuttleRequestStatus.Pending &&*/ x.Active && GUIDs.Contains(x.ShuttleRequestGUID.Value))
                                       join b in DbSHM.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.UserGUID equals b.UserGUID into LJ1
                                       from R1 in LJ1.DefaultIfEmpty()
                                       join c in DbSHM.codeReferralStatusLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.ReferralStatusGUID equals c.ReferralStatusGUID into LJ2
                                       from R2 in LJ2.DefaultIfEmpty()
                                       select new ShuttleRequestStaffDataTable
                                       {
                                           ShuttleRequestGUID = a.ShuttleRequestGUID,
                                           ShuttleRequestStaffGUID = a.ShuttleRequestStaffGUID,
                                           StaffName = R1.FirstName + " " + R1.Surname,
                                           Active = a.Active,
                                           ReferralStatusDescription = R2.Description
                                       }).ToList();

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            var ShuttleReturnRequest = (from a in DbSHM.dataShuttleRequest.Where(x => /*x.Active && x.ReferralStatusGUID == ShuttleRequestStatus.Progressing &&*/ x.Active && x.ReturnDateTime >= StartDate && x.ReturnDateTime <= EndDate && x.ReturnDateTime != null && x.StartLocationReturnGUID!=null && x.EndLocationReturnGUID!=null).Where(x => AuthorizedList.Contains(x.DutyStationGUID.Value.ToString()))

                                        join b in DbSHM.dataShuttleRequestBoxState.Where(x => !x.IsBoxStateDroped && !x.IsDeparture) on a.ShuttleRequestGUID  equals b.ShuttleRequestGUID
                                        //from R1 in LJ1.DefaultIfEmpty()

                                        //join c in DbSHM.dataShuttleRequestRouteStep on b.ShuttleRequestRouteStepGUID equals c.ShuttleRequestRouteStepGUID into LJ2
                                        //from R2 in LJ2.DefaultIfEmpty()

                                        join e in DbSHM.codeLocationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.StartLocationGUID equals e.LocationGUID into LJ4
                                        from R4 in LJ4.DefaultIfEmpty()
                                        join f in DbSHM.codeLocationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.EndLocationGUID equals f.LocationGUID into LJ5
                                        from R5 in LJ5.DefaultIfEmpty()

                                        join j in DbSHM.codeShuttleTravelPurposeLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.ShuttleTravelPurposeGUID equals j.ShuttleTravelPurposeGUID
                                        join h in DbSHM.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.UserGUID equals h.UserGUID
                                        orderby a.DepartureDate
                                        select new ShuttleRequestsDataTableModel
                                        {
                                            ShuttleRequestGUID = b.ShuttleRequestGUID,
                                            ShuttleRequestBoxStateGUID=b.ShuttleRequestBoxStateGUID,
                                            DepartureDate = a.ReturnDateTime.Value,
                                            StartLocation = R4.LocationDescription,
                                            EndLocation = R5.LocationDescription,
                                            ShuttleTravelPurposeDescription = j.ShuttleTravelPurposeDescription,
                                            StaffName = h.FirstName + " " + h.Surname
                                        }).ToList();
            GUIDs = ShuttleReturnRequest.Select(x => x.ShuttleRequestGUID).ToList();

            var ShuttleReturnRequestStaff = (from a in DbSHM.dataShuttleRequestStaff.Where(x => /*x.dataShuttleRequest.ReferralStatusGUID == ShuttleRequestStatus.Progressing &&*/ x.Active && GUIDs.Contains(x.ShuttleRequestGUID.Value))
                                             join b in DbSHM.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.UserGUID equals b.UserGUID into LJ1
                                             from R1 in LJ1.DefaultIfEmpty()
                                             join c in DbSHM.codeReferralStatusLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.ReferralStatusGUID equals c.ReferralStatusGUID into LJ2
                                             from R2 in LJ2.DefaultIfEmpty()
                                             select new ShuttleRequestStaffDataTable
                                             {
                                                 ShuttleRequestGUID = a.ShuttleRequestGUID,
                                                 ShuttleRequestStaffGUID = a.ShuttleRequestStaffGUID,
                                                 StaffName = R1.FirstName + " " + R1.Surname,
                                                 Active = a.Active,
                                                 ReferralStatusDescription = R2.Description
                                             }).ToList();

               return Json(new { ShuttleRequest, ShuttleRequestStaff, ShuttleReturnRequest, ShuttleReturnRequestStaff });
        }

        [HttpPost]
        public ActionResult AvtiveShuttles(DateTime StartDate, DateTime EndDate)
        {
            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.Shuttle.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();

            var Shuttle = (from a in DbSHM.dataShuttle.Where(x => x.Active && x.DepartureDateTime >= StartDate && x.DepartureDateTime <= EndDate).Where(x => AuthorizedList.Contains(x.DutyStationGUID.Value.ToString()))
                           join e in DbSHM.codeLocationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.StartLocationGUID equals e.LocationGUID into LJ4
                           from R4 in LJ4.DefaultIfEmpty()
                           join f in DbSHM.codeLocationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.EndLocationGUID equals f.LocationGUID into LJ5
                           from R5 in LJ5.DefaultIfEmpty()
                           orderby a.DepartureDateTime
                           select new ShuttlesDataTableModel
                           {
                               ShuttleGUID = a.ShuttleGUID,
                               DepartureDateTime = a.DepartureDateTime,
                               StartLocation = R4.LocationDescription,
                               EndLocation = R5.LocationDescription,
                               SharedBy = a.SharedBy == "" ? null : a.SharedBy
                           }).ToList();

            var ShuttleVehicle = (from a in DbSHM.dataShuttleVehicle.AsNoTracking().AsExpandable().Where(x => x.Active && x.dataShuttle.DepartureDateTime >= StartDate && x.dataShuttle.DepartureDateTime <= EndDate)
                                  join b in DbSHM.codeVehicle on a.VehicleGUID equals b.VehicleGUID into LJ1
                                  from R1 in LJ1.DefaultIfEmpty()
                                  join c in DbSHM.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.UserDriverGUID equals c.UserGUID into LJ2
                                  from R2 in LJ2.DefaultIfEmpty()
                                  select new ShuttleVehicleDataTable
                                  {
                                      ShuttleVehicleGUID = a.ShuttleVehicleGUID,
                                      ShuttleGUID = a.ShuttleGUID,
                                      UserGUID = a.UserDriverGUID,
                                      DriverName = a.IsUNAgencyVehicle ? "UN Agancy Driver" : R2.FirstName + " " + R2.Surname,
                                      VehicleNumber = a.IsUNAgencyVehicle ? a.UNAgencyVehicleNumber : R1.VehicleNumber,
                                      VehicleGUID = a.VehicleGUID,
                                  }).ToList();

            var ShuttleStaff = (from a in DbSHM.dataShuttleStaff.Where(x => x.Active)
                                join b in DbSHM.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.UserPassengerGUID equals b.UserGUID into LJ1
                                from R1 in LJ1.DefaultIfEmpty()

                                select new ShuttleStaffDataTable
                                {
                                    ShuttleVehicleGUID = a.ShuttleVehicleGUID,
                                    ShuttleGUID = a.dataShuttleVehicle.ShuttleGUID,
                                    ShuttleStaffGUID = a.ShuttleStaffGUID,
                                    ShuttleTravelPurposeGUID = a.ShuttleTravelPurposeGUID,
                                    UserPassengerGUID = a.UserPassengerGUID.ToString(),
                                    StaffName = a.IsUNAgencyStaff ? a.UNAgencyStaffName : R1.FirstName + " " + R1.Surname,
                                    Confirmed = a.Confirmed
                                }).ToList();

            return Json(new { Shuttle, ShuttleVehicle, ShuttleStaff });
        }

        public class ShuttlePara
        {
            public string PassengerGUIDs { get; set; }
            public string DriverGUIDs { get; set; }
            public string AVGUIDs { get; set; }
        }
        [HttpPost]
        public ActionResult FilterShuttles(ShuttlePara para)
        {
            FillPara(para);

            string[] PassengerGUIDsList = para.PassengerGUIDs.Split(',');
            string[] DriverGUIDsList = para.DriverGUIDs.Split(',');
            string[] AVGUIDsList = para.AVGUIDs.Split(',');


            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.Shuttle.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();

            var Shuttle = (from a in DbSHM.dataShuttle.Where(x => x.Active).Where(x => AuthorizedList.Contains(x.DutyStationGUID.Value.ToString()))
                           from b in a.dataShuttleVehicle.Where(y => AVGUIDsList.Contains(y.VehicleGUID.Value.ToString()) && DriverGUIDsList.Contains(y.UserDriverGUID.Value.ToString()))
                           from c in b.dataShuttleStaff.Where(x => PassengerGUIDsList.Contains(x.UserPassengerGUID.Value.ToString()))
                           join e in DbSHM.codeLocationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.StartLocationGUID equals e.LocationGUID into LJ4
                           from R4 in LJ4.DefaultIfEmpty()
                           join f in DbSHM.codeLocationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.EndLocationGUID equals f.LocationGUID into LJ5
                           from R5 in LJ5.DefaultIfEmpty()

                           orderby a.DepartureDateTime
                           select new ShuttlesDataTableModel
                           {
                               ShuttleGUID = a.ShuttleGUID,
                               DepartureDateTime = a.DepartureDateTime,
                               StartLocation = R4.LocationDescription,
                               EndLocation = R5.LocationDescription,
                               SharedBy = a.SharedBy == "" ? null : a.SharedBy
                           }).Distinct().ToList();

            var ShuttleVehicle = (from a in DbSHM.dataShuttleVehicle.AsNoTracking().AsExpandable().Where(x => x.Active).Where(y => AVGUIDsList.Contains(y.VehicleGUID.Value.ToString()) && DriverGUIDsList.Contains(y.UserDriverGUID.Value.ToString()))
                                  join b in DbSHM.codeVehicle on a.VehicleGUID equals b.VehicleGUID into LJ1
                                  from R1 in LJ1.DefaultIfEmpty()
                                  join c in DbSHM.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.UserDriverGUID equals c.UserGUID into LJ2
                                  from R2 in LJ2.DefaultIfEmpty()
                                  select new ShuttleVehicleDataTable
                                  {
                                      ShuttleVehicleGUID = a.ShuttleVehicleGUID,
                                      ShuttleGUID = a.ShuttleGUID,
                                      UserGUID = a.UserDriverGUID,
                                      DriverName = a.IsUNAgencyVehicle ? "UN Agancy Driver" : R2.FirstName + " " + R2.Surname,
                                      VehicleNumber = a.IsUNAgencyVehicle ? a.UNAgencyVehicleNumber : R1.VehicleNumber,
                                      VehicleGUID = a.VehicleGUID,
                                  }).ToList();

            var ShuttleStaff = (from a in DbSHM.dataShuttleStaff.Where(x => x.Active).Where(x => PassengerGUIDsList.Contains(x.UserPassengerGUID.Value.ToString()))
                                join b in DbSHM.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.UserPassengerGUID equals b.UserGUID into LJ1
                                from R1 in LJ1.DefaultIfEmpty()

                                select new ShuttleStaffDataTable
                                {
                                    ShuttleVehicleGUID = a.ShuttleVehicleGUID,
                                    ShuttleGUID = a.dataShuttleVehicle.ShuttleGUID,
                                    ShuttleStaffGUID = a.ShuttleStaffGUID,
                                    ShuttleTravelPurposeGUID = a.ShuttleTravelPurposeGUID,
                                    UserPassengerGUID = a.UserPassengerGUID.ToString(),
                                    StaffName = a.IsUNAgencyStaff ? a.UNAgencyStaffName : R1.FirstName + " " + R1.Surname,
                                    Confirmed = a.Confirmed
                                }).ToList();
            var dates = Shuttle.Select(x => x.DepartureDateTime.Date).OrderBy(x => x.Date).Distinct().ToList();
            return Json(new { Shuttle, ShuttleVehicle, ShuttleStaff, dates });
        }

        private ShuttlePara FillPara(ShuttlePara para)
        {
            if (para.DriverGUIDs == null) { para.DriverGUIDs = string.Join(",", DropDownList.ShuttleDrivers().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray()); }
            if (para.PassengerGUIDs == null) { para.PassengerGUIDs = string.Join(",", DropDownList.ShuttlePassanger().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray()); }
            if (para.AVGUIDs == null) { para.AVGUIDs = string.Join(",", DropDownList.Vehicles().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray()); }

            return para;
        }


        public ActionResult ShuttleNotifyStaffRequest(string shuttleRequestGUID)
        {
            string domain = AppSettingsKeys.ServerAccessibility + ".unhcrsyria.org/";
            var shuttleRequest = DbSHM.dataShuttleRequest.Where(x => x.ShuttleRequestGUID.ToString() == shuttleRequestGUID).FirstOrDefault();
            var userInfo = DbSHM.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active && x.UserGUID == shuttleRequest.UserGUID).FirstOrDefault();

            string RecipientEmail = DbSHM.StaffCoreData.Where(x => x.UserGUID == shuttleRequest.UserGUID).FirstOrDefault().EmailAddress;
            string copy_recipients = Session[SessionKeys.EmailAddress].ToString();

            string table = "";
            table += "<p>Dear "+ userInfo.FirstName+ " "+ userInfo.Surname + ",</p>";
            table += "<p>Kindly complete the shuttle request (Return back From Travel) Tab before " + shuttleRequest.ReturnDateTime.Value.AddDays(-2).ToString("dd-MMM-yyyy") + " by following the below link</p>";

            table += "<a href='https://"+ domain + "/SHM/ShuttleRequests/Update/"+ shuttleRequestGUID + "' >Click Here</a>";
            table += "<br/>";
            table += "Best regards,";
            table += "<br/>";
            table += "Shuttle Fleet Managements";
            new Email().ShuttleNotifyStaffRequest(RecipientEmail, copy_recipients, table);
            shuttleRequest.ReferralStatusGUID = ShuttleRequestStatus.Completed;
            DbSHM.SaveChanges();

            return Json(new { Message = "Mail Shared Successfully" });
        }


        public ActionResult RemoveShuttleRequest(Guid ShuttleRequestBoxStateGUID)
        {
            //var request = (from a in DbSHM.dataShuttleRequest
            //               join b in DbSHM.dataShuttleRequestBoxState.Where(x => x.ShuttleRequestBoxStateGUID == ShuttleRequestBoxStateGUID)
            //               on a.ShuttleRequestGUID equals b.ShuttleRequestGUID
            //               select a).FirstOrDefault();
            //request.ReferralStatusGUID=(request!=null:ShuttleRequestStatus.)
            var BoxState = DbSHM.dataShuttleRequestBoxState.Where(x => x.ShuttleRequestBoxStateGUID == ShuttleRequestBoxStateGUID).FirstOrDefault();
            BoxState.IsBoxStateDroped= (BoxState != null?  true:false);
            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(new { Message = "Request Removed Successfully" });
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage("Error!"));
            }

            
        }
        #endregion
    }
    public class ShuttleRequestStatus
    {
        public readonly static Guid Pending = Guid.Parse("a73e5015-896a-42cd-8845-d5fce5b80e15");
        public readonly static Guid Canceled = Guid.Parse("4adbdc93-b118-4d23-acf8-cae98cfc959b");
        public readonly static Guid Completed = Guid.Parse("9ea61a3f-ea90-4f4b-b573-0fe3660dcc72");
        public readonly static Guid Progressing = Guid.Parse("e58a961c-d300-4a5a-8df4-f75b5e873cd4");
    }
    public class LocationMatchDutystaion
    {
        public string DutystationGUID { get; set; }
        public string LocationGUID { get; set; }
    }
}