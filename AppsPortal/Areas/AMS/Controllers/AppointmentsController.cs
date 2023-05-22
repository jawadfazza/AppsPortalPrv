using AMS_DAL.Model;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.AMS.ViewModels;
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
using System.Text;

namespace AppsPortal.Areas.AMS.Controllers
{
    public class AppointmentsController : AMSBaseController
    {
        #region Appointment 

        //[Route("AMS/AppointmentsDataTable/{PK}")]
        public ActionResult AppointmentsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/AMS/Views/Appointments/_Index.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<dataAppointment, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<dataAppointment>(DataTable.Filters);
            }

            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.Appointment.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();
            List<string> AuthorizedList2 = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.AppointmentType.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();

            var Result = (from a in DbAMS.dataAppointment.AsNoTracking().AsExpandable().Where(Predicate).Where(x => x.CaseGUID == PK).Where(x => AuthorizedList.Contains(x.DutyStationGUID.ToString()) )
                          join b in DbAMS.codeAppointmentTypeLanguage.Where(x => x.LanguageID == LAN && x.Active).Where(x =>  AuthorizedList2.Contains(x.codeAppointmentType.DepartmentGUID.ToString())) on a.AppointmentTypeGUID equals b.AppointmentTypeGUID 

                          select new AppointmentsDataTableModel
                          {
                              AppointmentGUID = a.AppointmentGUID,
                              AppointmentDateTime = a.AppointmentDateTime,
                              AppointmentTypeDescription = b.AppointmentTypeDescription,
                              Active = a.Active,
                              Arrived = a.Arrived,
                              Cancelled = a.Cancelled,
                              dataAppointmentRowVersion = a.dataAppointmentRowVersion
                          });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);
            
            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        public ActionResult AppointmentCreate(Guid FK)
        {
            string DutyStationGUID = DbAMS.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault().DutyStationGUID.ToString();
            if (!CMS.HasAction(Permissions.Appointment.Create, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            
            return PartialView("~/Areas/AMS/Views/Appointments/_AppointmentForm.cshtml", new AppointmentUpdateModel { CaseGUID = FK, DutyStationGUID = Guid.Parse( DutyStationGUID) });

        }
        public ActionResult AppointmentUpdate(Guid PK)
        {
            string DutyStationGUID = DbAMS.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault().DutyStationGUID.ToString();
            if (!CMS.HasAction(Permissions.Appointment.Access, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            StringBuilder ReasonHistory = new StringBuilder();
            var appointmentReschedul = DbAMS.dataAppointmentReschedul.Where(x => x.AppointmentGUID == PK).ToList();
            foreach (dataAppointmentReschedul x in appointmentReschedul)
            {
                ReasonHistory.Append("-" + x.LastApointmentDate);
                ReasonHistory.AppendLine();
                ReasonHistory.Append("-" + x.Reason);
                ReasonHistory.AppendLine();
                ReasonHistory.AppendLine();

            }
            var appointment =  DbAMS.dataAppointment.Find(PK);
            var caseInfo = DbAMS.dataCase.Find(appointment.CaseGUID);
            return PartialView("~/Areas/AMS/Views/Appointments/_AppointmentForm.cshtml", Mapper.Map(appointment, new AppointmentUpdateModel() { ReasonHistory = ReasonHistory.ToString(),FileNumber=caseInfo.FileNumber,ICNameEN=caseInfo.ICNameEN,ICNameOtherLanguages=caseInfo.ICNameOtherLanguages }));
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AppointmentCreate(AppointmentUpdateModel model)
        {
            DateTime ExecutionTime = DateTime.Now;
            if (!CMS.HasAction(Permissions.Appointment.Create, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var staff = DbAMS.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            dataAppointment appointment = new dataAppointment();
            Mapper.Map(model, appointment);
            appointment.AppointmentGUID = Guid.NewGuid();
            appointment.CreatedBy = CMS.GetFullName(UserGUID, LAN);
            appointment.CreatedDate = DateTime.Now;
            //Get The Appointment Sequance
            appointment.Sequence= GetAppointmentSequence();
            if (!ModelState.IsValid || ActiveAppointment(appointment)) return PartialView("~/Areas/AMS/Views/Appointments/_AppointmentForm.cshtml", model);
            DbAMS.Create(appointment, Permissions.Appointment.CreateGuid, ExecutionTime, DbCMS);

            DateTime dateFrom = new DateTime(appointment.AppointmentDateTime.Year, appointment.AppointmentDateTime.Month, appointment.AppointmentDateTime.Day);
            DateTime dateTo = new DateTime(appointment.AppointmentDateTime.Year, appointment.AppointmentDateTime.Month, appointment.AppointmentDateTime.Day, 23, 59, 59);
            var appointmentTypeCalendar = DbAMS.dataAppointmentTypeCalendar.Where(x =>x.Active && x.AppointmentTypeGUID == model.AppointmentTypeGUID && x.EventStartDate>=dateFrom && x.EventStartDate<=dateTo).FirstOrDefault();

            //Check the Appointment Slots Availability
            if (appointmentTypeCalendar != null)
            {
                appointmentTypeCalendar.SlotClosed = DbAMS.dataAppointment.Where(x=>x.AppointmentTypeGUID==model.AppointmentTypeGUID && x.AppointmentDateTime>=dateFrom && x.AppointmentDateTime <= dateTo).Count() + 1;
            }
            else
            {
                dataAppointmentTypeCalendar calendar = new dataAppointmentTypeCalendar();
                calendar.AppointmentTypeCalenderGUID = Guid.NewGuid();
                calendar.AppointmentTypeGUID = appointment.AppointmentTypeGUID;
                calendar.EventStartDate = dateFrom;
                calendar.EventEndDate = dateTo;
                calendar.SlotAvailable = 0;
                calendar.SlotClosed =  1;
                calendar.PreventAppointments = false;
                calendar.PublicHolday = false;
                calendar.OrganizationInstanceGUID = staff.OrganizationInstanceGUID;
                calendar.DutyStationGUID = staff.DutyStationGUID;
                calendar.Active = true;

                DbAMS.Create(calendar, Permissions.Appointment.CreateGuid, ExecutionTime, DbCMS);
            }
            
            try
            {
                //appointment.StatusGUID = ReferralStatusConstants.NotStarted;
                //string FileNumber = DbAMS.dataCase.Where(x => x.CaseGUID == appointment.CaseGUID).FirstOrDefault().FileNumber;
                //appointment = new Email().AppointmentNotify(appointment, FileNumber, new List<Guid>() {
                //    ReferralStepConstants.AppointmentAssigned ,
                //     ReferralStepConstants.AppointmentArrivedCanceled,
                //       ReferralStepConstants.AppointmentCompleted });

                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.SingleUpdateMessage(DataTableNames.AppointmentsDataTable, DbAMS.PrimaryKeyControl(appointment), DbAMS.RowVersionControls(Portal.SingleToList(appointment)), "window.open('/AMS/Reports/AppointmentsSlip/"+ appointment.AppointmentGUID + "','_blank');"));
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        private string GetAppointmentSequence()
        {
            Guid DutyStationGUID = DbAMS.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault().DutyStationGUID;

            int AppCount = DbAMS.dataAppointment.Where(x => x.DutyStationGUID == DutyStationGUID).Count();
            return  AppCount + " / " + DbCMS.codeDutyStationsLanguages.Where(x => x.LanguageID == "EN" && x.DutyStationGUID==DutyStationGUID).FirstOrDefault().DutyStationDescription;

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AppointmentUpdate(AppointmentUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.Appointment.Update, Apps.AMS, model.DutyStationGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            DateTime ExecutionTime = DateTime.Now;
            var appointment = DbAMS.dataAppointment.Where(x => x.AppointmentGUID == model.AppointmentGUID).FirstOrDefault();
            
            //Reschedual Appointment date
            if (appointment.AppointmentDateTime != model.AppointmentDateTime)
            {
                model.UpdatedBy = CMS.GetFullName(UserGUID, LAN);
                model.UpdatedDate = DateTime.Now;
                dataAppointmentReschedul appointmentReschedul = new dataAppointmentReschedul();
                Mapper.Map(model, appointmentReschedul);
                appointmentReschedul.AppointmentReschedulGUID = Guid.NewGuid();
                appointmentReschedul.LastApointmentDate = appointment.AppointmentDateTime;
                DbAMS.Create(appointmentReschedul, Permissions.Appointment.UpdateGuid, ExecutionTime, DbCMS);
                string FileNumber = DbAMS.dataCase.Where(x => x.CaseGUID == appointment.CaseGUID).FirstOrDefault().FileNumber;
                appointment= new Email().AppointmentNotify(appointment, FileNumber, new List<Guid>() {
                       ReferralStepConstants.AppointmentRescheduled });
            }
           
            if (model.Arrived && model.ArrivedBy==null)
            {
                model.ArrivedBy = CMS.GetFullName(UserGUID, LAN);
                model.ArrivedDate = DateTime.Now;
            }
            if (model.Cancelled && model.CancelledBy == null)
            {
                model.CancelledBy = CMS.GetFullName(UserGUID, LAN);
                model.CancelledDate = DateTime.Now;
            }
            Mapper.Map(model, appointment);
            if (!ModelState.IsValid || ActiveAppointment(appointment)) return PartialView("~/Areas/AMS/Views/Appointments/_AppointmentForm.cshtml", model);
           
            DbAMS.Update(appointment, Permissions.Appointment.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                string FileNumber = DbAMS.dataCase.Where(x => x.CaseGUID == appointment.CaseGUID).FirstOrDefault().FileNumber;
                //appointment= new Email().AppointmentNotify(appointment, FileNumber , new List<Guid>() {
                //    ReferralStepConstants.AppointmentAssigned ,
                //     ReferralStepConstants.AppointmentArrivedCanceled,
                //       ReferralStepConstants.AppointmentCompleted });

                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                string callBackFunc = "UpadetAppointmentEvents('"+ model.AppointmentGUID + "')";
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.AppointmentsDataTable ,DbAMS.PrimaryKeyControl(appointment), DbAMS.RowVersionControls(Portal.SingleToList(appointment)), callBackFunc));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyAppointment(model.AppointmentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AppointmentDelete(dataAppointment model)
        {
            if (!CMS.HasAction(Permissions.Appointment.Delete, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataAppointment> DeletedLanguages = DeleteAppointments(new List<dataAppointment> { model });

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                string callBackFunc = "UpadetAppointmentEvents('" + model.AppointmentGUID + "')";
                return Json(DbAMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.AppointmentsDataTable,callBackFunc));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyAppointment(model.AppointmentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AppointmentRestore(dataAppointment model)
        {
            if (!CMS.HasAction(Permissions.Appointment.Restore, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveAppointment(model))
            {
                return Json(DbAMS.RecordExists());
            }

            List<dataAppointment> RestoredLanguages = RestoreAppointments(Portal.SingleToList(model));

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.AppointmentsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyAppointment(model.AppointmentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult AppointmentsDataTableDelete(List<dataAppointment> models)
        {
            if (!CMS.HasAction(Permissions.Appointment.Delete, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataAppointment> DeletedLanguages = DeleteAppointments(models);

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.AppointmentsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult AppointmentsDataTableRestore(List<dataAppointment> models)
        {
            if (!CMS.HasAction(Permissions.Appointment.Restore, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataAppointment> RestoredLanguages = RestoreAppointments(models);

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.AppointmentsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }
        private List<dataAppointment> DeleteAppointments(List<dataAppointment> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataAppointment> DeletedAppointments = new List<dataAppointment>();

            var AppointmentGUIDs = models.Select(y => y.AppointmentGUID).ToList();

            var Appointments = DbAMS.dataAppointment.Where(x => AppointmentGUIDs.Contains(x.AppointmentGUID)).ToList();
            var AppointmentTypeGUIDs = models.Select(y => y.AppointmentTypeGUID).ToList();

            foreach (var appointment in Appointments)
            {
                DeletedAppointments.Add(DbAMS.Delete(appointment, ExecutionTime, Permissions.Appointment.DeleteGuid, DbCMS));
                DateTime dateFrom = new DateTime(appointment.AppointmentDateTime.Year, appointment.AppointmentDateTime.Month, appointment.AppointmentDateTime.Day);
                DateTime dateTo = new DateTime(appointment.AppointmentDateTime.Year, appointment.AppointmentDateTime.Month, appointment.AppointmentDateTime.Day, 23, 59, 59);
                var appointmentTypeCalendar = DbAMS.dataAppointmentTypeCalendar.Where(x => x.Active && x.AppointmentTypeGUID == appointment.AppointmentTypeGUID && x.EventStartDate >= dateFrom && x.EventStartDate <= dateTo).FirstOrDefault();
                if (appointmentTypeCalendar != null) { appointmentTypeCalendar.SlotClosed--; };
            }
            return DeletedAppointments;
        }
        private List<dataAppointment> RestoreAppointments(List<dataAppointment> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataAppointment> RestoredLanguages = new List<dataAppointment>();

            var AppointmentGUIDs = models.Select(y => y.AppointmentGUID).ToList();
            var Appointments = DbAMS.dataAppointment.Where(x => AppointmentGUIDs.Contains(x.AppointmentGUID)).ToList();

            foreach (var appointment in Appointments)
            {
                    RestoredLanguages.Add(DbAMS.Restore(appointment, Permissions.Appointment.DeleteGuid, Permissions.Appointment.RestoreGuid, RestoringTime, DbCMS));
                DateTime dateFrom = new DateTime(appointment.AppointmentDateTime.Year, appointment.AppointmentDateTime.Month, appointment.AppointmentDateTime.Day);
                DateTime dateTo = new DateTime(appointment.AppointmentDateTime.Year, appointment.AppointmentDateTime.Month, appointment.AppointmentDateTime.Day, 23, 59, 59);
                var appointmentTypeCalendar = DbAMS.dataAppointmentTypeCalendar.Where(x => x.Active && x.AppointmentTypeGUID == appointment.AppointmentTypeGUID && x.EventStartDate >= dateFrom && x.EventStartDate <= dateTo).FirstOrDefault();

                if (appointmentTypeCalendar != null) { appointmentTypeCalendar.SlotClosed++; };
            }

            return RestoredLanguages;
        }
        private JsonResult ConcrrencyAppointment(Guid PK)
        {
            dataAppointment dbModel = new dataAppointment();

            var Language = DbAMS.dataAppointment.Where(l => l.AppointmentGUID == PK).FirstOrDefault();
            var dbLanguage = DbAMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataAppointmentRowVersion.SequenceEqual(dbModel.dataAppointmentRowVersion))
            {
                return Json(DbAMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbAMS, dbModel, "AppointmentsContainer"));
        }
        private bool ActiveAppointment(dataAppointment model)
        {
            int error = 0;
            Guid NewRegistration = Guid.Parse("a991743b-7b03-45cc-8a2d-4055ae32d94a");
            Guid UNHCR = Guid.Parse("E156C022-EC72-4A5A-BE09-163BD85C68EF");
            string DayOfWeek = model.AppointmentDateTime.DayOfWeek.ToString();
            var userProfiles = DbAMS.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            var appointmentType = DbAMS.codeAppointmentType.Where(x => x.AppointmentTypeGUID == model.AppointmentTypeGUID).FirstOrDefault();
            Guid dutyStationConfigurationGUID = DbCMS.codeDutyStationsConfigurations.Where(x => x.DutyStationGUID == userProfiles.DutyStationGUID && x.OrganizationInstanceGUID == UNHCR).FirstOrDefault().DutyStationConfigurationGUID;

            int WorkingDay = (from a in DbAMS.codeWorkingDaysConfigurations.Where(x => x.Active && x.DutyStationConfigurationGUID == dutyStationConfigurationGUID)
                              join b in DbAMS.codeTablesValuesLanguages.Where(x => x.ValueDescription == DayOfWeek) on a.DayGUID equals b.ValueGUID
                              select b).Count();
            int appointmentCount = DbAMS.dataAppointment.Where(x => x.CaseGUID == model.CaseGUID).Count();
            var appointmentIsNull = DbAMS.dataAppointment.Where(x => x.AppointmentGUID== model.AppointmentGUID).FirstOrDefault();

            int PublicHolday = DbAMS.dataAppointmentTypeCalendar.Where(x =>
             x.EventStartDate <= model.AppointmentDateTime &&
             x.EventEndDate >= model.AppointmentDateTime &&
             //x.OrganizationInstanceGUID == UNHCR &&
             x.DutyStationGUID == userProfiles.DutyStationGUID &&
             x.PublicHolday).Count();

            int PreventAppointments = DbAMS.dataAppointmentTypeCalendar.Where(x =>
            x.EventStartDate <= model.AppointmentDateTime &&
            x.EventEndDate >= model.AppointmentDateTime &&
            //x.OrganizationInstanceGUID == UNHCR &&
            x.DutyStationGUID == userProfiles.DutyStationGUID &&
            x.AppointmentTypeGUID == appointmentType.AppointmentTypeGUID &&
            x.PreventAppointments).Count();

            
            if(appointmentCount!=0 && NewRegistration == model.AppointmentTypeGUID && appointmentIsNull == null)
            {
                error++;
                ModelState.AddModelError("AppointmentTypeGUID", "(New Registration) just for the new opened case");
            }
            if (WorkingDay == 0)
            {
                error++;
                ModelState.AddModelError("AppointmentDateTime", "It's Holday on " + model.AppointmentDateTime.DayOfWeek + " (" + model.AppointmentDateTime + ")");
            }
            if (PublicHolday > 0)
            {
                error++;
                ModelState.AddModelError("AppointmentDateTime", "It's Public Holday on " + model.AppointmentDateTime.DayOfWeek + " (" + model.AppointmentDateTime + ")");
            }
            if (PreventAppointments > 0)
            {
                error++;
                string AppointmentTypeDescription = appointmentType.codeAppointmentTypeLanguage.Where(x => x.LanguageID == LAN).FirstOrDefault().AppointmentTypeDescription;
                ModelState.AddModelError("AppointmentDateTime", "The Appointments Prevented on " + model.AppointmentDateTime.DayOfWeek + " (" + model.AppointmentDateTime + ")" + " For " + AppointmentTypeDescription);
            }

            return (error > 0);
        }
        [HttpPost]
        public ActionResult AppointmentSlote(Guid PK)
        {
            var appSlotes = DbAMS.dataAppointmentTypeCalendar.AsNoTracking().Where(x => x.AppointmentTypeCalenderGUID == PK).Select(x => new
            {
                x.SlotAvailable,
                x.EventStartDate,
                x.EventEndDate,
                x.AppointmentTypeCalenderGUID,
                x.PublicHolday,
                x.PreventAppointments
            }).FirstOrDefault();
            return Json(appSlotes, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AppointmentCalendar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetAppointmentCalendarData(DateTime start, DateTime end)
        {
            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.Appointment.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();
            Guid UNHCR = Guid.Parse("E156C022-EC72-4A5A-BE09-163BD85C68EF");
            var userProfiles = DbCMS.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            var Appointments = (from a in DbAMS.dataAppointment.Where(x =>  x.Active && x.AppointmentDateTime >= start && x.AppointmentDateTime <= end)
                                .Where(x => AuthorizedList.Contains(x.DutyStationGUID.ToString()))
                                join b in DbAMS.dataCase.Where(x =>x.Active) on a.CaseGUID equals b.CaseGUID
                                join c in DbAMS.codeAppointmentTypeLanguage.Where(x=>x.Active && x.LanguageID==LAN )on a.AppointmentTypeGUID equals c.AppointmentTypeGUID into LJ1
                                from R1 in LJ1.DefaultIfEmpty()
                                join d in DbAMS.v_AppointmentContactNumber on a.CaseGUID equals d.CaseGUID into LJ2
                                from R2 in LJ2.DefaultIfEmpty()
                                orderby R1.AppointmentTypeGUID
                                select new CalendarEvents
                                {
                                    EventId = a.AppointmentGUID,
                                    EventStartDate = a.AppointmentDateTime,
                                    EventEndDate = a.AppointmentDateTime,
                                    Cancelled = a.Cancelled,
                                    Arrived = a.Arrived,
                                    Title =  b.FileNumber + "&&" + R1.AppointmentTypeDescription + "&&" + b.ICNameEN+ "&&" + b.CaseSize + "&&" + a.TokenNumber + "&&" + R2.PhoneNumber + "&&" + a.Comments,
                                    AllDayEvent = false
                                }).OrderBy(x=>x.EventStartDate).ToList();
            return Json(new JsonReturn { CalendarEvents = Appointments });
        }

        [HttpPost]
        public ActionResult GetWorkingDay()
        {
            Guid UNHCR = Guid.Parse("E156C022-EC72-4A5A-BE09-163BD85C68EF");
            var userProfiles = DbCMS.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            Guid DutyStationConfigurationGUID = DbCMS.codeDutyStationsConfigurations.Where(x => x.DutyStationGUID == userProfiles.DutyStationGUID && x.OrganizationInstanceGUID == UNHCR).FirstOrDefault().DutyStationConfigurationGUID;
            var WorkingDay = (from a in DbAMS.codeWorkingDaysConfigurations.Where(x => x.Active && x.DutyStationConfigurationGUID == DutyStationConfigurationGUID)
                              join b in DbAMS.codeTablesValues on a.DayGUID equals b.ValueGUID
                              select new WorkDay { Day = b.SortID.Value }).ToList();
            return Json(new JsonReturn { WorkDays = WorkingDay });
        }

        [HttpPost]
        public ActionResult UpdateAppointmentDate(Guid PK, DateTime NewDate)
        {
            DateTime ExecutionTime = DateTime.Now;
            var appointment = DbAMS.dataAppointment.Where(x => x.AppointmentGUID == PK).FirstOrDefault();
            if (appointment.AppointmentDateTime != NewDate)
            {
                dataAppointmentReschedul appointmentReschedul = new dataAppointmentReschedul();
                appointmentReschedul.Active = true;
                appointmentReschedul.AppointmentGUID = PK;
                appointmentReschedul.AppointmentReschedulGUID = Guid.NewGuid();
                appointmentReschedul.LastApointmentDate = appointment.AppointmentDateTime;
                DbAMS.Create(appointmentReschedul, Permissions.Appointment.UpdateGuid, ExecutionTime, DbCMS);
            }
            appointment.AppointmentDateTime = NewDate;

            if (!ModelState.IsValid || ActiveAppointment(appointment))
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

            DbAMS.Update(appointment, Permissions.Appointment.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.SingleUpdateMessage(DataTableNames.AppointmentsDataTable, DbAMS.PrimaryKeyControl(appointment), DbAMS.RowVersionControls(Portal.SingleToList(appointment))));
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }
        [HttpPost]
        public ActionResult UpadetAppointmentEvents(Guid PK)
        {
            return Json(new { Appointment = DbAMS.dataAppointment.Where(x => x.AppointmentGUID == PK).Select(x=> new
            {
                x.AppointmentGUID,
                x.Arrived,
                x.Cancelled,
                x.AppointmentDateTime
            }) });
        }
        [HttpPost]
        public ActionResult AppointmentCount(string PK,DateTime Date )
        {
            var userProfiles = DbCMS.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            DateTime Start = Date;
            DateTime end = Date.AddHours(23).AddMinutes(59);
            string[] PKs = PK.Split(',');
            var appointments = DbAMS.dataAppointment.Where(x =>
            x.Active &&
             PK.Contains(   x.AppointmentTypeGUID.ToString())   && 
            x.AppointmentDateTime >= Start && 
            x.AppointmentDateTime <= end &&
            x.DutyStationGUID == userProfiles.DutyStationGUID).ToList();

            return Json(new
            {
                Total= appointments.Count(),
                Arrived = appointments.Where(x=> x.Arrived).Count(),
                Cancelled = appointments.Where(x => x.Cancelled).Count(),
            });
        }
        #endregion
    }
}