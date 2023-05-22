using AMS_DAL.Model;
using AppsPortal.AMS.ViewModels;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.AMS.Controllers
{
    public class AppointmentTypeCalendarsController : AMSBaseController
    {
        // GET: AMS/AppointmentTypeCalendars


        #region Appointment Type Calendar

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetCalendarDataFromDatabase(DateTime start, DateTime end)
        {
            //Access is authorized by Access Action Department
            List<string> AuthorizedListDepartment = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.AppointmentType.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();
            //Access is authorized by Access Action DutyStation
            List<string> AuthorizedListDutyStation = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.AppointmentTypeCalendar.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();

            var userProfiles = DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            var Events = (from a in DbAMS.dataAppointmentTypeCalendar
                          .Where(x => x.EventStartDate>=start && x.EventEndDate<=end && x.OrganizationInstanceGUID == userProfiles.OrganizationInstanceGUID && x.Active)
                          .Where(x => AuthorizedListDepartment.Contains(x.codeAppointmentType.DepartmentGUID.ToString()))
                          .Where(x => AuthorizedListDutyStation.Contains(x.DutyStationGUID.ToString()))
                          join b in DbAMS.codeAppointmentTypeLanguage.Where(x=>x.Active && x.LanguageID==LAN) on a.AppointmentTypeGUID equals b.AppointmentTypeGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          select new CalendarEvents
                          {
                              EventId = a.AppointmentTypeCalenderGUID,
                              EventStartDate = a.EventStartDate,
                              EventEndDate = a.EventEndDate,
                              Title = a.PublicHolday? "Public Holday ("+ a.Comment +")" : R1.AppointmentTypeDescription + " ,[" + a.SlotAvailable+ "/" + a.SlotClosed+"] ("+ a.Comment + ")",
                              EventDescription = a.Comment,
                              AllDayEvent = false,
                              PreventAppointments=a.PreventAppointments,
                              PublicHolday=a.PublicHolday
                          }).ToList();
            return Json(new JsonReturn { CalendarEvents = Events });
        }

        [HttpPost]
        public ActionResult GetWorkingDay()
        {
            var userProfiles = DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            Guid DutyStationConfigurationGUID = DbCMS.codeDutyStationsConfigurations.Where(x => x.DutyStationGUID == userProfiles.DutyStationGUID && x.OrganizationInstanceGUID == userProfiles.OrganizationInstanceGUID).FirstOrDefault().DutyStationConfigurationGUID;

            var WorkingDay = (from a in DbAMS.codeWorkingDaysConfigurations.Where(x => x.Active && x.DutyStationConfigurationGUID == DutyStationConfigurationGUID)
                              join b in DbAMS.codeTablesValues on a.DayGUID equals b.ValueGUID
                              select new WorkDay { Day = b.SortID.Value }).ToList();
            return Json(new JsonReturn { WorkDays = WorkingDay });
        }

        private ActionResult Json(List<CalendarEvents> events, List<object> workingDay)
        {
            throw new NotImplementedException();
        }

        public ActionResult AppointmentTypeCalendarCreate()
        {
            if (!CMS.HasAction(Permissions.AppointmentTypeCalendar.Create, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var userStaffCore = DbAMS.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();

            return PartialView("~/Areas/AMS/Views/AppointmentTypeCalendars/_AppointmentTypeCalendarModal.cshtml",
                new AppointmentTypeCalenderUpdateModel { OrganizationInstanceGUID=userStaffCore.OrganizationInstanceGUID,DutyStationGUID=userStaffCore.DutyStationGUID ,EventEachDay=true});
        }

        public ActionResult AppointmentTypeCalendarUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.AppointmentTypeCalendar.Access, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            AppointmentTypeCalenderUpdateModel model = new AppointmentTypeCalenderUpdateModel();
            var Event = DbAMS.dataAppointmentTypeCalendar.Find(PK);
            Mapper.Map(Event, model);
            model.DepartmentGUID = Event.codeAppointmentType.DepartmentGUID.Value;
            return PartialView("~/Areas/AMS/Views/AppointmentTypeCalendars/_AppointmentTypeCalendarModal.cshtml", model);
        }
        [HttpPost]
        public ActionResult AppointmentTypeCalendarUpdateDropEvent(Guid PK,DateTime? startDate,DateTime? endDate)
        {
            if (!CMS.HasAction(Permissions.AppointmentTypeCalendar.Access, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            var model= DbAMS.dataAppointmentTypeCalendar.Find(PK) ;
            model.EventStartDate = startDate.Value;
            model.EventEndDate = endDate.Value;
            if (!ModelState.IsValid || ActiveAppointmentTypeCalendar(model))
            {
                string desc = "";
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        desc = desc + " ," + error.ErrorMessage.ToString();  //here 'error.ErrorMessage' will have required error message                                              //DoSomethingWith(error.ErrorMessage.ToString());
                    }
                }
                return Json(DbAMS.ErrorMessage(desc));
            }
            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, null, null));
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AppointmentTypeCalendarCreate(AppointmentTypeCalenderUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.AppointmentTypeCalendar.Create, Apps.AMS,model.DutyStationGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid ) return PartialView("~/Areas/AMS/Views/AppointmentTypeCalendars/_AppointmentTypeCalendarModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DateTime StartDate = model.EventStartDate.Value;
            var appointmentTypeCalendars=new List<dataAppointmentTypeCalendar>();
            dataAppointmentTypeCalendar appointmentTypeCalendar ;
            var days = DbAMS.codeTablesValuesLanguages.Where(x => x.LanguageID == "EN" && x.codeTablesValues.TableGUID == LookupTables.WorkingDays).ToList();

            Guid DutyStationConfigurationGUID = DbCMS.codeDutyStationsConfigurations.Where(x => x.DutyStationGUID == model.DutyStationGUID && x.OrganizationInstanceGUID == model.OrganizationInstanceGUID).FirstOrDefault().DutyStationConfigurationGUID;
            var WorkingDay = DbAMS.codeWorkingDaysConfigurations.Where(x => x.Active && x.DutyStationConfigurationGUID == DutyStationConfigurationGUID).ToList();
            if (model.EventEachDay)
            {
                while (StartDate <= model.EventEndDate)
                {
                    appointmentTypeCalendar = new dataAppointmentTypeCalendar();
                    Mapper.Map(model, appointmentTypeCalendar);
                    appointmentTypeCalendar.EventStartDate = StartDate;
                    appointmentTypeCalendar.EventEndDate = new DateTime(StartDate.Year,StartDate.Month, StartDate.Day, model.EventEndDate.Value.Hour, model.EventEndDate.Value.Minute, 0);
                    if (appointmentTypeCalendar.PublicHolday == true)
                    {
                        appointmentTypeCalendar.AppointmentTypeGUID = DbAMS.codeAppointmentType.Where(x => x.Code == "HOLIDAY").FirstOrDefault().AppointmentTypeGUID;
                    }
                    string dayName = StartDate.DayOfWeek.ToString();
                    Guid DayGuid = days.Where(x => x.ValueDescription == dayName).FirstOrDefault().ValueGUID;
                    //if (WorkingDay.Where(x => x.DayGUID == DayGuid && x.Active).FirstOrDefault() == null)
                    //{
                    //    appointmentTypeCalendar.PublicHolday = true;
                    //    appointmentTypeCalendar.SlotAvailable = 0;
                    //}
                    //else
                    //{
                    //    appointmentTypeCalendar.PublicHolday = false;
                    //}
                    appointmentTypeCalendars.Add(appointmentTypeCalendar); 
                    StartDate = StartDate.AddDays(1);
                }
                DbAMS.CreateBulk(appointmentTypeCalendars, Permissions.AppointmentTypeCalendar.CreateGuid, ExecutionTime, DbCMS);
            }
            else
            {
                appointmentTypeCalendar = new dataAppointmentTypeCalendar();
                Mapper.Map(model, appointmentTypeCalendar);
                DbAMS.Create(appointmentTypeCalendar, Permissions.AppointmentTypeCalendar.CreateGuid, ExecutionTime, DbCMS);
            }
            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, null, "InitializeCalendar();"));
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AppointmentTypeCalendarUpdate(dataAppointmentTypeCalendar model)
        {
            if (!CMS.HasAction(Permissions.AppointmentTypeCalendar.Update, Apps.AMS, model.DutyStationGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveAppointmentTypeCalendar(model))
            {
                string desc = "";
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        desc = desc + " ," + error.ErrorMessage.ToString();  //here 'error.ErrorMessage' will have required error message                                              //DoSomethingWith(error.ErrorMessage.ToString());
                    }
                }
                return Json(DbAMS.ErrorMessage(desc));
            }
            DateTime ExecutionTime = DateTime.Now;

            DbAMS.Update(model, Permissions.AppointmentTypeCalendar.UpdateGuid, ExecutionTime, DbCMS);
            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, null, "UpadetSlotEvents('"+model.AppointmentTypeCalenderGUID+"');"));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyAppointmentTypeCalendar(model.AppointmentTypeCalenderGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AppointmentTypeCalendarDelete(dataAppointmentTypeCalendar model)
        {
            if (!CMS.HasAction(Permissions.AppointmentTypeCalendar.Delete, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataAppointmentTypeCalendar> DeletedLanguages = DeleteAppointmentTypeCalendars(new List<dataAppointmentTypeCalendar> { model });

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, null, "UpadetSlotEvents('" + model.AppointmentTypeCalenderGUID + "');"));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyAppointmentTypeCalendar(model.AppointmentTypeCalenderGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AppointmentTypeCalendarRestore(dataAppointmentTypeCalendar model)
        {
            if (!CMS.HasAction(Permissions.AppointmentTypeCalendar.Restore, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveAppointmentTypeCalendar(model))
            {
                return Json(DbAMS.RecordExists());
            }

            List<dataAppointmentTypeCalendar> RestoredLanguages = RestoreAppointmentTypeCalendars(Portal.SingleToList(model));

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, null, null));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyAppointmentTypeCalendar(model.AppointmentTypeCalenderGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult AppointmentTypeCalendarsDataTableDelete(List<dataAppointmentTypeCalendar> models)
        {
            if (!CMS.HasAction(Permissions.AppointmentTypeCalendar.Delete, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataAppointmentTypeCalendar> DeletedLanguages = DeleteAppointmentTypeCalendars(models);

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, null, null));
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult AppointmentTypeCalendarsDataTableRestore(List<dataAppointmentTypeCalendar> models)
        {
            if (!CMS.HasAction(Permissions.AppointmentTypeCalendar.Restore, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataAppointmentTypeCalendar> RestoredLanguages = RestoreAppointmentTypeCalendars(models);

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, null, "InitializeCalendar();"));
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        private List<dataAppointmentTypeCalendar> DeleteAppointmentTypeCalendars(List<dataAppointmentTypeCalendar> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataAppointmentTypeCalendar> DeletedAppointmentTypeCalendars = new List<dataAppointmentTypeCalendar>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "SELECT AppointmentTypeCalenderGUID,CONVERT(varchar(50), DutyStationGUID) as C2 ,dataAppointmentTypeCalendarRowVersion FROM ams.dataAppointmentTypeCalendar where AppointmentTypeCalenderGUID in (" + string.Join(",", models.Select(x => "'" + x.AppointmentTypeCalenderGUID + "'").ToArray()) + ")";

            string query = DbAMS.QueryBuilder(models, Permissions.AppointmentTypeCalendar.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var languages = DbAMS.Database.SqlQuery<dataAppointmentTypeCalendar>(query).ToList();

            foreach (var language in languages)
            {
                DeletedAppointmentTypeCalendars.Add(DbAMS.Delete(language, ExecutionTime, Permissions.AppointmentTypeCalendar.DeleteGuid, DbCMS));
            }

            return DeletedAppointmentTypeCalendars;
        }

        private List<dataAppointmentTypeCalendar> RestoreAppointmentTypeCalendars(List<dataAppointmentTypeCalendar> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataAppointmentTypeCalendar> RestoredLanguages = new List<dataAppointmentTypeCalendar>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbAMS.QueryBuilder(models, Permissions.AppointmentTypeCalendar.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbAMS.Database.SqlQuery<dataAppointmentTypeCalendar>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveAppointmentTypeCalendar(language))
                {
                    RestoredLanguages.Add(DbAMS.Restore(language, Permissions.AppointmentTypeCalendar.DeleteGuid, Permissions.AppointmentTypeCalendar.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyAppointmentTypeCalendar(Guid PK)
        {
            dataAppointmentTypeCalendar dbModel = new dataAppointmentTypeCalendar();

            var Language = DbAMS.dataAppointmentTypeCalendar.Where(l => l.AppointmentTypeCalenderGUID == PK).FirstOrDefault();
            var dbLanguage = DbAMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataAppointmentTypeCalendarRowVersion.SequenceEqual(dbModel.dataAppointmentTypeCalendarRowVersion))
            {
                return Json(DbAMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbAMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveAppointmentTypeCalendar(dataAppointmentTypeCalendar model)
        {
            int error = 0;
            string DayOfWeek = model.EventStartDate.DayOfWeek.ToString();
            var userProfiles = DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            Guid dutyStationConfigurationGUID = DbCMS.codeDutyStationsConfigurations.Where(x => x.DutyStationGUID == userProfiles.DutyStationGUID && x.OrganizationInstanceGUID == userProfiles.OrganizationInstanceGUID).FirstOrDefault().DutyStationConfigurationGUID;
            int WorkingDay = (from a in DbAMS.codeWorkingDaysConfigurations.Where(x => x.Active && x.DutyStationConfigurationGUID == dutyStationConfigurationGUID)
                              join b in DbAMS.codeTablesValuesLanguages.Where(x => x.ValueDescription == DayOfWeek) on a.DayGUID equals b.ValueGUID
                              select b).Count();



            if (WorkingDay == 0)
            {
                error++;
                ModelState.AddModelError("EventStartDate", "It's Holday on " + model.EventStartDate.DayOfWeek + " (" + model.EventStartDate + ")");
            }
            DayOfWeek = model.EventEndDate.DayOfWeek.ToString();
            WorkingDay = (from a in DbAMS.codeWorkingDaysConfigurations.Where(x => x.Active && x.DutyStationConfigurationGUID == dutyStationConfigurationGUID)
                          join b in DbAMS.codeTablesValuesLanguages.Where(x => x.ValueDescription == DayOfWeek) on a.DayGUID equals b.ValueGUID
                          select b).Count();
            if (WorkingDay == 0)
            {
                error++;
                ModelState.AddModelError("EventEndDate", "It's Holday on " + model.EventEndDate.DayOfWeek + " (" + model.EventEndDate + ")");
            }
            return (error > 0);
        }

        [HttpPost]
        public ActionResult UpadetSlotEvents(Guid PK)
        {
            return Json(new
            {
                AppointmentTypeCalender = DbAMS.dataAppointmentTypeCalendar.Where(x => x.AppointmentTypeCalenderGUID == PK).Select(x => new
                {
                    x.AppointmentTypeCalenderGUID,
                    x.PublicHolday,
                    x.PreventAppointments,
                    x.EventStartDate,
                    x.EventEndDate
                })
            });
        }


        #endregion
    }
}