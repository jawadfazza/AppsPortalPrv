using AHD_DAL.Model;
using AHD_DAL.ViewModels;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Models;
using AppsPortal.ViewModels;
using AutoMapper;
using OfficeOpenXml;
using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AppsPortal.Areas.AHD.Controllers
{
    public class InternationalStaffPresenceTrackingController : AHDBaseController
    {
        // GET: AHD/InternationalStaffPresenceTracking
        public ActionResult Index()
        {
            return View();
        }
        [Route("AHD/StaffPresenceTrackingIndex/")]
        public ActionResult StaffPresenceTrackingIndex()
        {
            //if (!cms.hasaction(permissions.internationalstaffattendancepresence.access, apps.ahd))
            //{
            //    return json(dbahd.permissionerror());
            //}

            return View("~/Areas/AHD/Views/InternationalStaffPresenceTracking/Index.cshtml");
        }
        [HttpPost]
        public ActionResult GetWorkingDay()
        {
            var _user = DbCMS.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            Guid DutyStationConfigurationGUID = DbCMS.codeDutyStationsConfigurations.Where(x => x.DutyStationGUID == _user.DutyStationGUID).FirstOrDefault().DutyStationConfigurationGUID;

            var WorkingDay = (from a in DbCMS.codeWorkingDaysConfigurations.Where(x => x.Active && x.DutyStationConfigurationGUID == DutyStationConfigurationGUID)
                              join b in DbCMS.codeTablesValues on a.DayGUID equals b.ValueGUID
                              select new WorkDay { Day = b.SortID.Value }).ToList();
            return Json(new JsonReturn { WorkDays = WorkingDay });
        }


        [HttpPost]
        public ActionResult GetCalendarDataFromDatabase(DateTime start, DateTime end)
        {
            //Access is authorized by Access Action Department
            //List<string> AuthorizedListDepartment = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.AppointmentType.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();
            ////Access is authorized by Access Action DutyStation
            //List<string> AuthorizedListDutyStation = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.InternationalStaffAttendancePresence.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();

            var userProfiles = DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            var Events = (from a in DbAHD.dataInternationalStaffAttendance
                          .Where(x => x.FromDate >= start && x.ToDate <= end && x.Active).ToList()
                              //.Where(x => AuthorizedListDepartment.Contains(x.codeAppointmentType.DepartmentGUID.ToString()))
                              //.Where(x => AuthorizedListDutyStation.Contains(x.DutyStationGUID.ToString()))
                          join b in DbAHD.codeAHDInternationalStaffAttendanceType.Where(x => x.Active) on a.InternationalStaffAttendanceTypeGUID equals b.InternationalStaffAttendanceTypeGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          select new CalendarEvents
                          {
                              EventId = a.InternationalStaffAttendanceGUID,
                              UserGUID = a.StaffGUID,
                              EventStartDate = a.FromDate,
                              EventEndDate = a.ToDate.Value.AddDays(1),
                              Title = a.StaffName + " " + R1.AttendanceTypeName,
                              EventDescription = a.Comments,
                              AllDayEvent = false,

                              backgroundColor = R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave ? "#0066CC" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR ? "#FF3355" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave ? "#8133FF" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave ? "#FFB833" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.SLWFP ? "#42d1f5" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TravelTime ? "#44bcd8" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday ? "#33FFBD" :
                                  R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Mission ? "#707339" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Quarantine ? "#6bafe3" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave ? "#fcba03" :
                                R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.HomeLeave ? "#a89b32" :
                                R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.PaternityLeave ? "#34a832" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TravelPurpose ? "#a89ba2" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend ? "#945142" :

                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting ? "#FFBD33" : "",

                              borderColor = R1.InternationalStaffAttendanceTypeGUID.ToString() == "13afca94-4fa0-479a-85db-8aef4bc64cbb" ? "#00c0ef" : "#00c0ef",


                          }).ToList();
            return Json(new { CalendarEvents = Events }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GetCalendarDataFromDatabaseStaffFilter(Guid filterGUID)
        {
            //Access is authorized by Access Action Department
            //List<string> AuthorizedListDepartment = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.AppointmentType.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();
            ////Access is authorized by Access Action DutyStation
            //List<string> AuthorizedListDutyStation = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.InternationalStaffAttendancePresence.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();

            var userProfiles = DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            var Events = (from a in DbAHD.dataInternationalStaffAttendance
                          .Where(x => x.Active && x.StaffGUID == filterGUID).ToList()
                              //.Where(x => AuthorizedListDepartment.Contains(x.codeAppointmentType.DepartmentGUID.ToString()))
                              //.Where(x => AuthorizedListDutyStation.Contains(x.DutyStationGUID.ToString()))
                          join b in DbAHD.codeAHDInternationalStaffAttendanceType.Where(x => x.Active) on a.InternationalStaffAttendanceTypeGUID equals b.InternationalStaffAttendanceTypeGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          select new CalendarEvents
                          {
                              EventId = a.InternationalStaffAttendanceGUID,
                              UserGUID = a.StaffGUID,
                              EventStartDate = a.FromDate,
                              EventEndDate = a.ToDate.Value.AddDays(1),
                              Title = a.StaffName + " " + R1.AttendanceTypeName,
                              EventDescription = a.Comments,
                              AllDayEvent = false,
                              backgroundColor = R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave ? "#0066CC" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR ? "#FF3355" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave ? "#8133FF" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave ? "#FFB833" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.SLWFP ? "#42d1f5" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TravelTime ? "#44bcd8" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday ? "#33FFBD" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend ? "#945142" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Mission ? "#707339" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Quarantine ? "#6bafe3" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave ? "#fcba03" :
                                R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TravelPurpose ? "#a89ba2" :
                                R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.HomeLeave ? "#a89b32" :
                                R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.PaternityLeave ? "#34a832" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting ? "#FFBD33" : "",


                              borderColor = R1.InternationalStaffAttendanceTypeGUID.ToString() == "13afca94-4fa0-479a-85db-8aef4bc64cbb" ? "#00c0ef" : "#00c0ef",


                          }).ToList();
            return Json(new { CalendarEvents = Events }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetCalendarDataFromDatabaseLeaveTypefFilter(Guid filterGUID)
        {
            //Access is authorized by Access Action Department
            //List<string> AuthorizedListDepartment = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.AppointmentType.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();
            ////Access is authorized by Access Action DutyStation
            //List<string> AuthorizedListDutyStation = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.InternationalStaffAttendancePresence.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();

            var userProfiles = DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            var Events = (from a in DbAHD.dataInternationalStaffAttendance
                          .Where(x => x.Active && x.InternationalStaffAttendanceTypeGUID == filterGUID).ToList()
                              //.Where(x => AuthorizedListDepartment.Contains(x.codeAppointmentType.DepartmentGUID.ToString()))
                              //.Where(x => AuthorizedListDutyStation.Contains(x.DutyStationGUID.ToString()))
                          join b in DbAHD.codeAHDInternationalStaffAttendanceType.Where(x => x.Active) on a.InternationalStaffAttendanceTypeGUID equals b.InternationalStaffAttendanceTypeGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          select new CalendarEvents
                          {
                              EventId = a.InternationalStaffAttendanceGUID,
                              UserGUID = a.StaffGUID,
                              EventStartDate = a.FromDate,
                              EventEndDate = a.ToDate.Value.AddDays(1),
                              Title = a.StaffName + " " + R1.AttendanceTypeName,
                              EventDescription = a.Comments,
                              AllDayEvent = false,
                              backgroundColor = R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave ? "#0066CC" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR ? "#FF3355" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave ? "#8133FF" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave ? "#FFB833" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.SLWFP ? "#42d1f5" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TravelTime ? "#44bcd8" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday ? "#33FFBD" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend ? "#945142" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Mission ? "#707339" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Quarantine ? "#6bafe3" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave ? "#fcba03" :
                                R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.HomeLeave ? "#a89b32" :
                                R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.PaternityLeave ? "#34a832" :
                                R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TravelPurpose ? "#a89ba2" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting ? "#FFBD33" : "",
                              borderColor = R1.InternationalStaffAttendanceTypeGUID.ToString() == "13afca94-4fa0-479a-85db-8aef4bc64cbb" ? "#00c0ef" : "#00c0ef",


                          }).ToList();
            return Json(new { CalendarEvents = Events }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GetCalendarDataFromDatabaseFilterByUser(DateTime start, DateTime end, Guid staffGUID)
        {
            //Access is authorized by Access Action Department
            //List<string> AuthorizedListDepartment = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.AppointmentType.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();
            ////Access is authorized by Access Action DutyStation
            //List<string> AuthorizedListDutyStation = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.InternationalStaffAttendancePresence.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();

            // var userProfiles = DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            var Events = (from a in DbAHD.dataInternationalStaffAttendance
                          .Where(x => x.FromDate >= start && x.ToDate <= end && x.Active && x.StaffGUID == staffGUID).ToList()
                              //.Where(x => AuthorizedListDepartment.Contains(x.codeAppointmentType.DepartmentGUID.ToString()))
                              //.Where(x => AuthorizedListDutyStation.Contains(x.DutyStationGUID.ToString()))
                          join b in DbAHD.codeAHDInternationalStaffAttendanceType.Where(x => x.Active) on a.InternationalStaffAttendanceTypeGUID equals b.InternationalStaffAttendanceTypeGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          select new CalendarEvents
                          {
                              EventId = a.InternationalStaffAttendanceGUID,
                              UserGUID = a.StaffGUID,
                              EventStartDate = a.FromDate,
                              EventEndDate = a.ToDate.Value.AddDays(1),
                              Title = a.StaffName + " " + R1.AttendanceTypeName,
                              EventDescription = a.Comments,
                              AllDayEvent = false,
                              backgroundColor = R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave ? "#0066CC" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR ? "#FF3355" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave ? "#8133FF" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave ? "#FFB833" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.SLWFP ? "#42d1f5" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TravelTime ? "#44bcd8" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday ? "#33FFBD" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend ? "#945142" :
                                  R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Mission ? "#707339" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Quarantine ? "#6bafe3" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave ? "#fcba03" :
                                R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.HomeLeave ? "#a89b32" :
                                R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.PaternityLeave ? "#34a832" :
                                R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TravelPurpose ? "#a89ba2" :
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting ? "#FFBD33" : "",
                              borderColor = R1.InternationalStaffAttendanceTypeGUID.ToString().ToUpper() == "13afca94-4fa0-479a-85db-8aef4bc64cbb" ? "#00c0ef" : "#00c0ef",


                          }).ToList();
            return Json(new { CalendarEvents = Events }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AttendanceLeaveCalendarUpdate(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.InternationalStaffAttendancePresence.Access, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            InternationalStaffPresenceAttendanceUpdateModel model = new InternationalStaffPresenceAttendanceUpdateModel();
            var Event = DbAHD.dataInternationalStaffAttendance.Find(PK);
            Mapper.Map(Event, model);
            model.InternationalStaffAttendanceTypeGUID = Event.codeAHDInternationalStaffAttendanceType.InternationalStaffAttendanceTypeGUID;
            return PartialView("~/Areas/AHD/Views/InternationalStaffPresenceTracking/_StaffLeaveModal.cshtml", model);
        }
        public ActionResult AttendanceLeaveCalendarCreate()
        {
            //if (!CMS.HasAction(Permissions.InternationalStaffAttendancePresence.Create, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}

            InternationalStaffPresenceAttendanceUpdateModel model = new InternationalStaffPresenceAttendanceUpdateModel();

            return PartialView("~/Areas/AHD/Views/InternationalStaffPresenceTracking/_StaffLeaveModal.cshtml", model);

            //return PartialView("~/Areas/AHD/Views/AppointmentTypeCalendars/_AppointmentTypeCalendarModal.cshtml",
            //    new AppointmentTypeCalenderUpdateModel { OrganizationInstanceGUID = userProfiles.OrganizationInstanceGUID, DutyStationGUID = userProfiles.DutyStationGUID, EventEachDay = true });
        }

        [HttpPost]
        public ActionResult AttendanceLeaveCalendarDelete(InternationalStaffPresenceAttendanceUpdateModel model)
        {
            var toudel = DbAHD.dataInternationalStaffAttendance.Where(x => x.InternationalStaffAttendanceGUID == model.InternationalStaffAttendanceGUID).FirstOrDefault();

            DbAHD.dataInternationalStaffAttendance.Remove(toudel);
            var _checkPeriod = DbAHD.dataAHDPeriodEntitlement.Where(x => ((x.StartMonth <= model.FromDate) && (x.EndMonth >= model.ToDate))
                                                                       || ((x.StartMonth >= model.FromDate) && (x.StartMonth <= model.ToDate)
                                                                       && (x.EndMonth >= model.ToDate))
                                                                       || ((x.StartMonth <= model.FromDate) && (x.EndMonth <= model.ToDate)
                                                                       && (x.EndMonth >= model.FromDate))
                                                                     ).FirstOrDefault();
            if (_checkPeriod != null)
            {
                var _staff = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == model.StaffGUID).FirstOrDefault();
                var _changer = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
                Guid hazarGUID = Guid.Parse("1F9F00E7-CBFB-40FD-B800-8C88C2BEF247");
                var _emailTo = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == hazarGUID).FirstOrDefault();
                string SubjectMessage = "Attendance Records for " + _staff.FullName;


                //to send mail to staff 
                // var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();



                string _message = resxEmails.AttendanceUpdateAfterEntitilementsCreation
                    .Replace("$StaffName", _staff.FullName)
                    .Replace("$changer", _changer.FullName)
                  ;

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                //var myEmail = _staff.Select(x => x.EmailAddress).FirstOrDefault();
                //string copy_recipients = currPrepartion.EmailAddress;
                Send(_emailTo.EmailAddress, SubjectMessage, _message, isRec, "");
            }
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, null, "InitializeCalendarLeavs();"));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost]
        public ActionResult AttendanceLeaveCalendarCreate(InternationalStaffPresenceAttendanceUpdateModel model)
        {
            //if (!CMS.HasAction(Permissions.InternationalStaffAttendancePresence.Create, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}



            if (!ModelState.IsValid || model.InternationalStaffAttendanceTypeGUID == null || (model.FromDate > model.ToDate)) return PartialView("~/Areas/AHD/Views/InternationalStaffPresenceTracking/_StaffLeaveModal.cshtml", model);
            var allStaffAttendacnes = DbAHD.dataInternationalStaffAttendance.Where(x => x.StaffGUID == model.StaffGUID).ToList();
            foreach (var model2 in allStaffAttendacnes)
            {
                if ((model2.FromDate <= model.ToDate && (model2.ToDate >= model.FromDate)))
                {
                    ModelState.AddModelError("Error: ", "Kindly revise the entry data to avoid conflict in dates");
                    return PartialView("~/Areas/AHD/Views/InternationalStaffPresenceTracking/_StaffLeaveModal.cshtml", model);
                }


            }

            DateTime ExecutionTime = DateTime.Now;

            DateTime StartDate = model.FromDate.Value;
            //var appointmentTypeCalendars = new List<dataInternationalStaffAttendance>();
            var userperson = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == model.StaffGUID && x.LanguageID == LAN).FirstOrDefault();
            var _attendance = DbAHD.codeAHDInternationalStaffAttendanceType.Where(x => x.Active && x.InternationalStaffAttendanceTypeGUID == model.InternationalStaffAttendanceTypeGUID).FirstOrDefault();

            List<StaffLeaveHolidayDates> _realleaves = new List<StaffLeaveHolidayDates>();
            List<dataInternationalStaffAttendance> allleaves = new List<dataInternationalStaffAttendance>();

            _realleaves = CheckCustomLeaveDays((DateTime)model.FromDate, (DateTime)model.ToDate, (Guid)model.InternationalStaffAttendanceTypeGUID, _attendance.AttendanceTypeName, (Guid)model.StaffGUID);
            dataInternationalStaffAttendance temp = new dataInternationalStaffAttendance();
            foreach (var item in _realleaves.Where(x => x.LeaveTypeGUID == Guid.Parse("A1A0A314-388C-4E21-AB91-B919439AA798") || x.LeaveTypeGUID == Guid.Parse("A1A0A314-388C-4E21-AB91-B919439AA797")))
            {
                temp = new dataInternationalStaffAttendance();
                temp.InternationalStaffAttendanceGUID = Guid.NewGuid();
                temp.InternationalStaffAttendanceTypeGUID = item.LeaveTypeGUID;
                temp.StaffGUID = model.StaffGUID;
                temp.StaffName = userperson.FirstName + " " + userperson.Surname;
                temp.CountryGUID = model.CountryGUID;
                temp.LeaveLocation = model.LeaveLocation;

                temp.ToDate = item.startdateName;
                temp.FromDate = item.startdateName;
                temp.Comments = model.Comments;
                temp.TotalDays = 1;
                temp.CreatedByGUID = UserGUID;
                temp.CreateDate = ExecutionTime;
                temp.TotalWeekendDays = 0;

                allleaves.Add(temp);
            }



            foreach (var item in _realleaves.Where(x => x.LeaveTypeGUID != Guid.Parse("A1A0A314-388C-4E21-AB91-B919439AA798") && x.LeaveTypeGUID != Guid.Parse("A1A0A314-388C-4E21-AB91-B919439AA797") && x.LeaveTypeGUID == model.InternationalStaffAttendanceTypeGUID))
            {
                temp = new dataInternationalStaffAttendance();
                temp.InternationalStaffAttendanceGUID = Guid.NewGuid();
                temp.InternationalStaffAttendanceTypeGUID = item.LeaveTypeGUID;
                temp.StaffGUID = model.StaffGUID;
                temp.StaffName = userperson.FirstName + " " + userperson.Surname;
                temp.CountryGUID = model.CountryGUID;
                temp.LeaveLocation = model.LeaveLocation;

                temp.ToDate = item.enddateName;
                temp.FromDate = item.startdateName;
                temp.Comments = model.Comments;
                temp.TotalDays = item.enddateName != null ? ((int)((item.enddateName - item.startdateName).Value.TotalDays) + 1) : 1;
                temp.CreatedByGUID = UserGUID;
                temp.CreateDate = ExecutionTime;
                temp.TotalWeekendDays = 0;
                allleaves.Add(temp);
            }





            //};

            DbAHD.CreateBulk(allleaves, Permissions.InternationalStaffAttendancePresence.CreateGuid, ExecutionTime, DbCMS);
            var _checkPeriod = DbAHD.dataAHDPeriodEntitlement.Where(x => ((x.StartMonth <= model.FromDate) && (x.EndMonth >= model.ToDate))
                                                                  || ((x.StartMonth >= model.FromDate) && (x.StartMonth <= model.ToDate)
                                                                  && (x.EndMonth >= model.ToDate))
                                                                  || ((x.StartMonth <= model.FromDate) && (x.EndMonth <= model.ToDate)
                                                                  && (x.EndMonth >= model.FromDate))
                                                                ).FirstOrDefault();
            if (_checkPeriod != null)
            {
                var _staff = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == model.StaffGUID).FirstOrDefault();
                var _changer = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
                Guid hazarGUID = Guid.Parse("1F9F00E7-CBFB-40FD-B800-8C88C2BEF247");
                var _emailTo = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == hazarGUID).FirstOrDefault();
                string SubjectMessage = "Attendance Records for " + _staff.FullName;


                //to send mail to staff 
                // var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();



                string _message = resxEmails.AttendanceUpdateAfterEntitilementsCreation
                    .Replace("$StaffName", _staff.FullName)
                    .Replace("$changer", _changer.FullName)
                  ;

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                //var myEmail = _staff.Select(x => x.EmailAddress).FirstOrDefault();
                //string copy_recipients = currPrepartion.EmailAddress;
                Send(_emailTo.EmailAddress, SubjectMessage, _message, isRec, "");
            }
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                ViewBag.CurrUserGUID = model.StaffGUID;
                return Json(DbCMS.SingleUpdateMessage(null, null, null, "InitializeCalendarLeavs();"));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }


        }
        public void Send(string recipients, string subject, string body, int? isRec, string copy_recipients)
        {
            //string copy_recipients = "";
            string blind_copy_recipients = null;
            string body_format = "HTML";
            string importance = "Normal";
            string file_attachments = null;
            string _body = "<div style='font-family:Arial;'>" + body.Replace("\r\n", "<br/>") + "</div>";
            if (LAN == "AR") { _body = "<p align='right'>" + _body + "</p>"; }
           // DbAHD.SendEmailHR("maksoud@unhcr.org", "", blind_copy_recipients, subject, _body, body_format, importance, file_attachments);
            DbCMS.SendEmailHR(recipients, copy_recipients, blind_copy_recipients, subject, _body, body_format, importance, file_attachments);
        }


        [HttpPost]
        public ActionResult AttendanceLeaveCalendarUpdate(InternationalStaffPresenceAttendanceUpdateModel model)
        {
            //if (!CMS.HasAction(Permissions.InternationalStaffAttendancePresence.Create, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}

            var toudel = DbAHD.dataInternationalStaffAttendance.Where(x => x.InternationalStaffAttendanceGUID == model.InternationalStaffAttendanceGUID).FirstOrDefault();
            try
            {
                DbAHD.dataInternationalStaffAttendance.Remove(toudel);
                DbAHD.SaveChanges();
            }
            catch (Exception)
            {

                return PartialView("~/Areas/AHD/Views/InternationalStaffPresenceTracking/_StaffLeaveModal.cshtml", model);
            }


            if (!ModelState.IsValid || model.InternationalStaffAttendanceTypeGUID == null || (model.FromDate > model.ToDate)) return PartialView("~/Areas/AHD/Views/InternationalStaffPresenceTracking/_StaffLeaveModal.cshtml", model);
            var allStaffAttendacnes = DbAHD.dataInternationalStaffAttendance.Where(x => x.StaffGUID == model.StaffGUID).ToList();
            foreach (var model2 in allStaffAttendacnes)
            {
                if ((model2.FromDate <= model.ToDate && (model2.ToDate >= model.FromDate)))
                {
                    ModelState.AddModelError("Error: ", "Kindly revise the entry data to avoid conflict in dates");
                    return PartialView("~/Areas/AHD/Views/InternationalStaffPresenceTracking/_StaffLeaveModal.cshtml", model);
                }


            }

            DateTime ExecutionTime = DateTime.Now;

            DateTime StartDate = model.FromDate.Value;
            //var appointmentTypeCalendars = new List<dataInternationalStaffAttendance>();
            var userperson = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == model.StaffGUID && x.LanguageID == LAN).FirstOrDefault();
            var _attendance = DbAHD.codeAHDInternationalStaffAttendanceType.Where(x => x.Active && x.InternationalStaffAttendanceTypeGUID == model.InternationalStaffAttendanceTypeGUID).FirstOrDefault();

            List<StaffLeaveHolidayDates> _realleaves = new List<StaffLeaveHolidayDates>();
            List<dataInternationalStaffAttendance> allleaves = new List<dataInternationalStaffAttendance>();

            _realleaves = CheckCustomLeaveDays((DateTime)model.FromDate, (DateTime)model.ToDate, (Guid)model.InternationalStaffAttendanceTypeGUID, _attendance.AttendanceTypeName, (Guid)model.StaffGUID);
            dataInternationalStaffAttendance temp = new dataInternationalStaffAttendance();
            foreach (var item in _realleaves.Where(x => x.LeaveTypeGUID == Guid.Parse("A1A0A314-388C-4E21-AB91-B919439AA798") || x.LeaveTypeGUID == Guid.Parse("A1A0A314-388C-4E21-AB91-B919439AA797")))
            {
                temp = new dataInternationalStaffAttendance();
                temp.InternationalStaffAttendanceGUID = Guid.NewGuid();
                temp.InternationalStaffAttendanceTypeGUID = item.LeaveTypeGUID;
                temp.StaffGUID = model.StaffGUID;
                temp.StaffName = userperson.FirstName + " " + userperson.Surname;
                temp.CountryGUID = model.CountryGUID;
                temp.LeaveLocation = model.LeaveLocation;

                temp.ToDate = item.startdateName;
                temp.FromDate = item.startdateName;
                temp.Comments = model.Comments;
                temp.TotalDays = 1;
                temp.CreatedByGUID = UserGUID;
                temp.CreateDate = ExecutionTime;
                temp.TotalWeekendDays = 0;

                allleaves.Add(temp);
            }



            foreach (var item in _realleaves.Where(x => x.LeaveTypeGUID != Guid.Parse("A1A0A314-388C-4E21-AB91-B919439AA798") && x.LeaveTypeGUID != Guid.Parse("A1A0A314-388C-4E21-AB91-B919439AA797") && x.LeaveTypeGUID == model.InternationalStaffAttendanceTypeGUID))
            {
                temp = new dataInternationalStaffAttendance();
                temp.InternationalStaffAttendanceGUID = Guid.NewGuid();
                temp.InternationalStaffAttendanceTypeGUID = item.LeaveTypeGUID;
                temp.StaffGUID = model.StaffGUID;
                temp.StaffName = userperson.FirstName + " " + userperson.Surname;
                temp.CountryGUID = model.CountryGUID;
                temp.LeaveLocation = model.LeaveLocation;

                temp.ToDate = item.enddateName;
                temp.FromDate = item.startdateName;
                temp.Comments = model.Comments;
                temp.TotalDays = item.enddateName != null ? ((int)((item.enddateName - item.startdateName).Value.TotalDays) + 1) : 1;
                temp.CreatedByGUID = UserGUID;
                temp.CreateDate = ExecutionTime;
                temp.TotalWeekendDays = 0;
                allleaves.Add(temp);
            }





            //};

            DbAHD.CreateBulk(allleaves, Permissions.InternationalStaffAttendancePresence.CreateGuid, ExecutionTime, DbCMS);

            var _checkPeriod = DbAHD.dataAHDPeriodEntitlement.Where(x => ((x.StartMonth <= model.FromDate) && (x.EndMonth >= model.ToDate))
                                                             || ((x.StartMonth >= model.FromDate) && (x.StartMonth <= model.ToDate)
                                                             && (x.EndMonth >= model.ToDate))
                                                             || ((x.StartMonth <= model.FromDate) && (x.EndMonth <= model.ToDate)
                                                             && (x.EndMonth >= model.FromDate))
                                                           ).FirstOrDefault();
            if (_checkPeriod != null)
            {
                var _staff = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == model.StaffGUID).FirstOrDefault();
                var _changer = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
                Guid hazarGUID = Guid.Parse("1F9F00E7-CBFB-40FD-B800-8C88C2BEF247");
                var _emailTo = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == hazarGUID).FirstOrDefault();
                string SubjectMessage = "Attendance Records for " + _staff.FullName;


                //to send mail to staff 
                // var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();



                string _message = resxEmails.AttendanceUpdateAfterEntitilementsCreation
                    .Replace("$StaffName", _staff.FullName)
                    .Replace("$changer", _changer.FullName)
                  ;

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                //var myEmail = _staff.Select(x => x.EmailAddress).FirstOrDefault();
                //string copy_recipients = currPrepartion.EmailAddress;
                Send(_emailTo.EmailAddress, SubjectMessage, _message, isRec, "");
            }
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, null, "InitializeCalendarLeavs();"));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }


        }



        public List<StaffLeaveHolidayDates> CheckCustomLeaveDays(DateTime _startDate, DateTime _endDate, Guid _leaveTypeGUID, string leaveName, Guid staffGUID)
        {
            List<StaffLeaveHolidayDates> _holidayleaves = new List<StaffLeaveHolidayDates>();

            var _holiday = DbAHD.codeOrganizationHoliday.ToList();
            int j = 0;
            var lastdate = _endDate;
            var myStart = _startDate;
            int _totalRRLeaves = 0;
            var checkleaves = DbAHD.dataInternationalStaffAttendance.Where(x => x.StaffGUID == staffGUID).ToList();

            for (var day = _startDate; day <= lastdate; day = day.AddDays(1))
            {
                int _res = 0;
                if (_totalRRLeaves == 5 && _leaveTypeGUID == Guid.Parse("67979D6D-5B7B-4A85-AA6D-CD604A1BDF75"))
                {
                    break;
                }


                if ((_leaveTypeGUID != coddeInternationalStaffAttendanceTypeAttendanceTable.RR) && ((_leaveTypeGUID != coddeInternationalStaffAttendanceTypeAttendanceTable.TravelTime)) && (day.DayOfWeek.ToString() == "Friday" || day.DayOfWeek.ToString() == "Saturday"))
                {

                    StaffLeaveHolidayDates myHoliday = new StaffLeaveHolidayDates
                    {
                        startdateName = day,
                        enddateName = day,
                        LeaveTypeGUID = Guid.Parse("A1A0A314-388C-4E21-AB91-B919439AA798"),
                        LeaveName = "Weekend"
                    };
                    j++;
                    //lastdate = lastdate.AddDays(1);

                    _holidayleaves.Add(myHoliday);
                    _res = 1;

                }
                if (_res == 0 && _leaveTypeGUID != coddeInternationalStaffAttendanceTypeAttendanceTable.RR && (_leaveTypeGUID != coddeInternationalStaffAttendanceTypeAttendanceTable.TravelTime))
                {

                    foreach (var item in _holiday)
                    {
                        for (var dayhol = item.HolidayStartDate; dayhol <= item.HolidayEndDate; dayhol = dayhol.AddDays(1))
                        {

                            if (day == dayhol)
                            {
                                StaffLeaveHolidayDates myHoliday = new StaffLeaveHolidayDates
                                {
                                    startdateName = day,
                                    enddateName = day,
                                    LeaveTypeGUID = Guid.Parse("A1A0A314-388C-4E21-AB91-B919439AA797"),
                                    LeaveName = "Official Holiday"
                                };
                                j++;
                                //lastdate = lastdate.AddDays(1);
                                _holidayleaves.Add(myHoliday);
                                _res = 1;

                            }

                        }

                    }
                }

                if (_res == 0)
                {
                    var check = _holidayleaves.Where(x => x.LeaveTypeGUID == _leaveTypeGUID && x.enddateName == day.AddDays(-1)).FirstOrDefault();
                    if (check != null)
                    {
                        _holidayleaves.Remove(check);
                        StaffLeaveHolidayDates toAdd = new StaffLeaveHolidayDates
                        {
                            startdateName = check.startdateName,
                            enddateName = day,
                            LeaveTypeGUID = _leaveTypeGUID,
                            LeaveName = leaveName
                        };
                        _holidayleaves.Add(toAdd);
                    }
                    else
                    {
                        StaffLeaveHolidayDates myHoliday = new StaffLeaveHolidayDates
                        {
                            startdateName = day,
                            enddateName = day,
                            LeaveTypeGUID = _leaveTypeGUID,
                            LeaveName = leaveName
                        };
                        _holidayleaves.Add(myHoliday);
                    }
                    _totalRRLeaves++;

                }

            }

            var checkPri = DbAHD.dataInternationalStaffAttendance.Where(x => x.StaffGUID == staffGUID).OrderByDescending(x => x.ToDate).FirstOrDefault();
            var start_priDay_oneday = _startDate.AddDays(-1);
            var start_priDay = _startDate.AddDays(-2);
            var start_priTwoDay = _startDate.AddDays(-3);
            var start_prithreeDay = _startDate.AddDays(-4);

            if (checkPri != null && (start_priDay == checkPri.ToDate || start_priTwoDay == checkPri.ToDate || start_prithreeDay == checkPri.ToDate))
            {

                foreach (var item in _holiday)
                {


                    for (var dayhol = item.HolidayStartDate; dayhol <= item.HolidayEndDate; dayhol = dayhol.AddDays(1))
                    {

                        if (start_priDay == dayhol || start_priDay_oneday == dayhol || start_prithreeDay == dayhol || start_priTwoDay == dayhol)
                        {
                            var checkleaveexit = checkleaves.Where(x => x.FromDate <= dayhol && x.ToDate >= dayhol).FirstOrDefault();
                            if (checkleaveexit != null)
                            {
                                bool funct = true;
                            }
                            else
                            {

                                StaffLeaveHolidayDates myHoliday = new StaffLeaveHolidayDates
                                {
                                    startdateName = dayhol,
                                    LeaveTypeGUID = Guid.Parse("A1A0A314-388C-4E21-AB91-B919439AA797"),
                                    LeaveName = "Official Holiday"
                                };

                                _holidayleaves.Add(myHoliday);
                            }

                        }

                    }

                }

                if ((start_priDay.DayOfWeek.ToString() == "Friday" || start_priDay.DayOfWeek.ToString() == "Saturday")
                    && (start_priDay_oneday.DayOfWeek.ToString() != "Sunday"))
                {
                    var checkleaveexit = checkleaves.Where(x => x.FromDate <= start_priDay && x.ToDate >= start_priDay).FirstOrDefault();
                    if (checkleaveexit != null)
                    {
                        bool funct = true;
                    }
                    else
                    {
                        StaffLeaveHolidayDates myHoliday = new StaffLeaveHolidayDates
                        {
                            startdateName = start_priDay,
                            enddateName = start_priDay,
                            LeaveTypeGUID = Guid.Parse("A1A0A314-388C-4E21-AB91-B919439AA798"),
                            LeaveName = "Weekend"
                        };



                        _holidayleaves.Add(myHoliday);
                    }
                }
                if (start_priDay_oneday.DayOfWeek.ToString() == "Friday" || start_priDay_oneday.DayOfWeek.ToString() == "Saturday")
                {
                    var checkleaveexit = checkleaves.Where(x => x.FromDate <= start_priDay_oneday && x.ToDate >= start_priDay_oneday).FirstOrDefault();
                    if (checkleaveexit != null)
                    {
                        bool funct = true;
                    }
                    else
                    {
                        StaffLeaveHolidayDates myHoliday = new StaffLeaveHolidayDates
                        {
                            startdateName = start_priDay_oneday,
                            enddateName = start_priDay_oneday,
                            LeaveTypeGUID = Guid.Parse("A1A0A314-388C-4E21-AB91-B919439AA798"),
                            LeaveName = "Weekend"
                        };




                        _holidayleaves.Add(myHoliday);
                    }

                }



            }
            return _holidayleaves;

        }


        [HttpPost]
        public ActionResult AttendanceLeaveUpdateDropEvent(Guid PK, DateTime? startDate, DateTime? endDate)
        {
            //if (!CMS.HasAction(Permissions.InternationalStaffAttendancePresence.Access, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}

            var model = DbAHD.dataInternationalStaffAttendance.Find(PK);
            model.FromDate = startDate.Value;
            model.ToDate = endDate.Value;
            if (!ModelState.IsValid)
            {
                string desc = "";
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        desc = desc + " ," + error.ErrorMessage.ToString();  //here 'error.ErrorMessage' will have required error message                                              //DoSomethingWith(error.ErrorMessage.ToString());
                    }
                }
                return Json(DbAHD.ErrorMessage(desc));
            }
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, null, null));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }



        #region Reports
        [Route("AHD/StaffPresenceTrackingReportIndex/")]
        public ActionResult StaffPresenceTrackingReportIndex()
        {
            //if (!cms.hasaction(permissions.internationalstaffattendancepresence.access, apps.ahd))
            //{
            //    return json(dbahd.permissionerror());
            //}

            return View("~/Areas/AHD/Views/InternationalStaffPresenceTracking/Reports/Index.cshtml");
        }
        public ActionResult GenerateAttendanceReport(GeneralAttendanceReport reportParameters)
        {
            //if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Update, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            Guid orgGuid = (Guid)Session[SessionKeys.OrganizationInstanceGUID];

            #region Report summary 
            DataSourceSelectArguments args = new DataSourceSelectArguments();
            DateTime today = DateTime.Now;
            var myQuery = DbAHD.dataInternationalStaffAttendance.AsQueryable();
            if (reportParameters.StaffGuids != null && reportParameters.StaffGuids.Count > 0)
            {
                myQuery = myQuery.Where(x =>
                   reportParameters.StaffGuids.Contains(x.StaffGUID)
                 );
            }
            if (reportParameters.FromDate != null && reportParameters.ToDate != null)
            {
                myQuery = myQuery.Where(x => x.FromDate >= reportParameters.FromDate && x.FromDate <= reportParameters.ToDate);
            }

            var result = myQuery.ToList();
            if (result.Count > 0)
            {
                string sourceFile = Server.MapPath("~/Areas/AHD/Templates/InternationalStaffAttendanceReport.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/AHD/Temp/InternationalStaffAttendanceReport" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Staff Name", typeof(string));
                    dt.Columns.Add("Attendance Type Name", typeof(string));
                    dt.Columns.Add("From Date", typeof(string));
                    dt.Columns.Add("To Date", typeof(string));


                    //dt.Columns.Add("Finance Approved Date", typeof(DateTime));

                    var _attendanceTypes = DbAHD.codeAHDInternationalStaffAttendanceType.ToList();
                    foreach (var item in result.OrderBy(x => x.StaffName).ThenBy(x => x.FromDate))
                    {

                        DataRow dr;
                        dr = dt.NewRow();
                        #region calc
                        var _myLeave = _attendanceTypes.Where(x => x.InternationalStaffAttendanceTypeGUID == item.InternationalStaffAttendanceTypeGUID).FirstOrDefault();

                        #endregion

                        dr[0] = item.StaffName;
                        dr[1] = _myLeave.AttendanceTypeName;
                        dr[2] = item.FromDate;
                        dr[3] = item.ToDate;


                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["B4"].LoadFromDataTable(dt, true);
                    //workSheet.Cells["A1"].Value = "_List of  national staff danger pay as of " + result.Select(x => x.PaymentDurationName).FirstOrDefault();

                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = " Staff Attendance Report" + Guid.NewGuid() + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }

            string success = "No Data Available for this period";
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);


            #endregion


            return View();


        }

        #endregion



    }
}