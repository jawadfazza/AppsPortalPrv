
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
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
using OSA.Model;
using iTextSharp.text.pdf.qrcode;
using Microsoft.Reporting.WebForms;
using AppsPortal.Areas.OSA.RDLC;
using AppsPortal.Areas.OSA.RDLC.OSADataSetTableAdapters;
using DocumentFormat.OpenXml.Drawing.ChartDrawing;
using System.Data;
using System.Threading.Tasks;
using RES_Repo.Globalization;

namespace AppsPortal.Areas.OSA.Controllers
{
    public class OfficeStaffAttendancesController : OSABaseController
    {
        #region Office Staff Attendance 

        public ActionResult Index()
        {
            return View("~/Areas/OSA/Views/OfficeStaffAttendances/Index.cshtml");
        }
        public ActionResult OfficeStaffAttendanceConfirmationsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);
            Expression<Func<dataOfficeStaffAttendanceConfirmationsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<dataOfficeStaffAttendanceConfirmationsDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            var All = (from a in DbOSA.dataOfficeStaffAttendanceConfirmation.AsNoTracking().AsExpandable().Where(x => x.UserGUID == UserGUID)
                       let payment= DbOSA.dataOfficeStaffAttendance.Where(x => x.AttendanceFromDatetime.Month == a.ConfirmedMonth &&
                             x.AttendanceFromDatetime.Year == a.ConfirmedYear && x.IsAttend && x.IsConfirmed && x.Active && x.UserGUID == UserGUID).Count()
                       select new dataOfficeStaffAttendanceConfirmationsDataTableModel
                       {
                           UserGUID = a.UserGUID,
                           MonthYear = a.ConfirmedYear + "-" + a.ConfirmedMonth + "-1" ,
                           ConfirmedBy =a.ConfirmedBy,
                           ConfirmedDate=a.ConfirmedDate,
                           ConfirmedMonth=a.ConfirmedMonth,
                           ConfirmedYear=a.ConfirmedYear,
                           PaymentConfirmedBy=a.PaymentConfirmedBy,
                           PaymentConfirmedDate=a.PaymentConfirmedDate,
                           OfficeStaffAttendanceConfirmationGUID=a.OfficeStaffAttendanceConfirmationGUID,
                           PaymentAmount= (payment>20?20:payment)*10,
                           Active = a.Active,
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<dataOfficeStaffAttendanceConfirmationsDataTableModel> Result = Mapper.Map<List<dataOfficeStaffAttendanceConfirmationsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        public ActionResult OfficeStaffAttendanceCreate()
        {
            var staffInfo = DbOSA.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            return PartialView("~/Areas/OSA/Views/OfficeStaffAttendances/_OfficeStaffAttendanceForm.cshtml", new OfficeStaffAttendanceUpdateModel()
            {
                //IsInternationalStaff = /*staffInfo.IsInternational.Value ? true : false,*/ false,
                UserGUID = UserGUID,
                IsConfirmed = true,
                DutyStationGUID = staffInfo.DutyStationGUID
            });

        }
        public ActionResult OfficeStaffAttendanceNeedConfirmCreate()
        {
            var staffInfo = DbOSA.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            return PartialView("~/Areas/OSA/Views/OfficeStaffAttendances/_OfficeStaffAttendanceNeedConfirmForm.cshtml", new OfficeStaffAttendanceUpdateModel()
            {
                //IsInternationalStaff = /*staffInfo.IsInternational.Value ? true : false,*/ false,
                UserGUID = UserGUID,
                IsConfirmed = false,
                DutyStationGUID = staffInfo.DutyStationGUID
            });

        }
        public ActionResult OfficeStaffAttendanceUpdate(Guid PK)
        {
            var staffInfo = DbOSA.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();

            var staffAttendance = (from a in DbOSA.dataOfficeStaffAttendance.Where(x => x.OfficeStaffAttendanceGUID == PK)
                                   join b in DbOSA.codeOfficeFloorRoom on a.OfficeFloorRoomGUID equals b.OfficeFloorRoomGUID into LJ1
                                   from R1 in LJ1.DefaultIfEmpty()
                                   join c in DbOSA.codeOfficeFloor on R1.OfficeFloorGUID equals c.OfficeFloorGUID into LJ2
                                   from R2 in LJ2.DefaultIfEmpty()
                                   join d in DbOSA.codeOffices on R2.OfficeGUID equals d.OfficeGUID into LJ3
                                   from R3 in LJ3.DefaultIfEmpty()
                                   select new OfficeStaffAttendanceUpdateModel
                                   {
                                       AttendanceFromDatetime = a.AttendanceFromDatetime,
                                       AttendanceToDatetime = a.AttendanceToDatetime,
                                       Active = a.Active,
                                       UserGUID = a.UserGUID,
                                       DutyStationGUID = R3.DutyStationGUID != null ? R3.DutyStationGUID : staffInfo.DutyStationGUID,
                                       OfficeGUID = R3.OfficeGUID,
                                       OfficeFloorGUID = R2.OfficeFloorGUID,
                                       OfficeFloorRoomGUID = R1.OfficeFloorRoomGUID,
                                       dataOfficeStaffAttendanceRowVersion = a.dataOfficeStaffAttendanceRowVersion,
                                       IsAttend = a.IsAttend,
                                       IsConfirmed = a.IsConfirmed,
                                       OfficeStaffAttendanceGUID = a.OfficeStaffAttendanceGUID,
                                       ShuttleDepartureEveningTime = a.ShuttleDepartureEveningTime,
                                       ShuttleDepartureMorningTime = a.ShuttleDepartureMorningTime

                                   }
                                   ).FirstOrDefault();

            return PartialView("~/Areas/OSA/Views/OfficeStaffAttendances/_OfficeStaffAttendanceForm.cshtml", staffAttendance);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OfficeStaffAttendanceCreate(OfficeStaffAttendanceUpdateModel model)
        {
            DateTime ExecutionTime = DateTime.Now;


            /////////////////////////////////////////
            if (!ModelState.IsValid || ActiveOfficeStaffAttendance(model)) return PartialView("~/Areas/OSA/Views/OfficeStaffAttendances/_OfficeStaffAttendanceForm.cshtml", model);
            if (model.AttendanceToDatetime == null)
            {
                model.AttendanceToDatetime = model.AttendanceFromDatetime;
            }
            DateTime StartDate = model.AttendanceFromDatetime.Value;
            while (StartDate <= model.AttendanceToDatetime)
            {
                dataOfficeStaffAttendance OfficeStaffAttendance = new dataOfficeStaffAttendance();
                Mapper.Map(model, OfficeStaffAttendance);
                OfficeStaffAttendance.OfficeStaffAttendanceGUID = Guid.NewGuid();
                OfficeStaffAttendance.AttendanceFromDatetime = StartDate;
                OfficeStaffAttendance.AttendanceToDatetime = StartDate;
                OfficeStaffAttendanceUpdateModel modelCopy = new OfficeStaffAttendanceUpdateModel();
                Mapper.Map(OfficeStaffAttendance, modelCopy);

                if (ActiveOfficeStaffAttendance(modelCopy))
                {
                    // return PartialView("~/Areas/OSA/Views/OfficeStaffAttendances/_OfficeStaffAttendanceForm.cshtml", model);
                }
                else
                {
                    DbOSA.Create(OfficeStaffAttendance, Permissions.OfficeStaffAttendance.CreateGuid, ExecutionTime, DbCMS);
                }

                StartDate = StartDate.AddDays(1);

            }


            try
            {
                string callBackFunc = "InitializeOfficeStaffAttendanceCalendar(true)";
                DbOSA.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOSA.SingleUpdateMessage(DataTableNames.OfficeStaffAttendancesDataTable, null, null, callBackFunc, null));
            }
            catch (Exception ex)
            {
                return Json(DbOSA.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OfficeStaffAttendanceUpdate(OfficeStaffAttendanceUpdateModel model)
        {

            var Audit = DbCMS.spAuditHistory(LAN, model.OfficeStaffAttendanceGUID).OrderBy(x => x.ExecutionTime).FirstOrDefault() != null ? DbCMS.spAuditHistory(LAN, model.OfficeStaffAttendanceGUID).OrderBy(x => x.ExecutionTime).FirstOrDefault().ExecutedBy : "";
            var staffInfo = DbCMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == UserGUID && x.Active && x.LanguageID == LAN).FirstOrDefault();
            var staffReportTo = DbOSA.StaffCoreData.Where(x => x.UserGUID == model.UserGUID && x.ReportToGUID == UserGUID).FirstOrDefault();
            string StaffName = staffInfo.FirstName + " " + staffInfo.Surname;
            //Ammar Al-Joudeh,Jawad Alfazzaa,Amjad Shams Eddin,Kinan Ibrahem,Moaaz Al Sabbagh,Mohanad Kutieni,Mayy Hatouzouk

            string[] AdminUsers = new string[] { "66BEA969-8D70-4AAB-8A84-AEF4C8D13AE0", "20E6CF0C-5BD7-465E-A0BD-2170D85663E8", "6dd7ff17-050d-4349-ab41-bdfe48133057", "6DD7FF17-050D-4349-AB41-BDFE48133057", "4c4ea9e0-ebce-444e-83d9-0af1b3c3e279", "2E72179B-72FD-495B-8723-76FBA09A882D" , "7524301F-8536-4772-96A0-CD355DA1319E" };
            if (AdminUsers.Contains(UserGUID.ToString().ToUpper()) || StaffName == Audit || CMS.HasAction(Permissions.OfficeStaffAttendance.Confirm, Apps.OSA) || staffReportTo != null)
            {
                if (!ModelState.IsValid || ActiveOfficeStaffAttendance(model)) return PartialView("~/Areas/OSA/Views/OfficeStaffAttendances/_OfficeStaffAttendanceForm.cshtml", model);
                DateTime ExecutionTime = DateTime.Now;
                dataOfficeStaffAttendance OfficeStaffAttendance = Mapper.Map(model, new dataOfficeStaffAttendance());
                OfficeStaffAttendance.IsConfirmed = false;
                if (CMS.HasAction(Permissions.OfficeStaffAttendance.Update, Apps.OSA))
                {
                    OfficeStaffAttendance.IsConfirmed = true;
                }
                DbOSA.Update(OfficeStaffAttendance, Permissions.OfficeStaffAttendance.UpdateGuid, ExecutionTime, DbCMS);

                try
                {

                    DbOSA.SaveChanges();
                    DbCMS.SaveChanges();
                    string callBackFunc = "InitializeOfficeStaffAttendanceCalendar(true)";
                    return Json(DbCMS.SingleUpdateMessage(DataTableNames.OfficeStaffAttendancesDataTable, null, null, callBackFunc));
                }
                catch (DbUpdateConcurrencyException)
                {
                    return ConcrrencyOfficeStaffAttendance(model.OfficeStaffAttendanceGUID);
                }
                catch (Exception ex)
                {
                    return Json(DbOSA.ErrorMessage(ex.Message));
                }
            }
            else
            {
                return Json(DbOSA.ErrorMessage("You don't have Owner Permissions.."));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OfficeStaffAttendanceDelete(dataOfficeStaffAttendance model)
        {

            var Audit = DbCMS.spAuditHistory(LAN, model.OfficeStaffAttendanceGUID).OrderBy(x => x.ExecutionTime).FirstOrDefault()!=null? DbCMS.spAuditHistory(LAN, model.OfficeStaffAttendanceGUID).OrderBy(x => x.ExecutionTime).FirstOrDefault().ExecutedBy : "";
            string staffName = DbCMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == UserGUID && x.Active && x.LanguageID == LAN).Select(x => x.FirstName + " " + x.Surname).FirstOrDefault();
            //Ammar Al-Joudeh,Jawad Alfazzaa,Amjad Shams Eddin,Kinan Ibrahem,Moaaz Al Sabbagh,Mohanad Kutieni,Mayy Hatouzouk

            string[] AdminUsers = new string[] { "66BEA969-8D70-4AAB-8A84-AEF4C8D13AE0", "20E6CF0C-5BD7-465E-A0BD-2170D85663E8", "6dd7ff17-050d-4349-ab41-bdfe48133057", "6DD7FF17-050D-4349-AB41-BDFE48133057", "4c4ea9e0-ebce-444e-83d9-0af1b3c3e279", "2E72179B-72FD-495B-8723-76FBA09A882D", "7524301F-8536-4772-96A0-CD355DA1319E" };
            if (staffName == Audit || AdminUsers.Contains(UserGUID.ToString().ToUpper()) || !model.IsConfirmed)
            {
                List<dataOfficeStaffAttendance> DeletedLanguages = DeleteOfficeStaffAttendances(new List<dataOfficeStaffAttendance> { model });

                try
                {
                    DbOSA.SaveChanges();
                    DbCMS.SaveChanges();
                    string callBackFunc = "UpadetOfficeStaffAttendanceEvents('" + model.OfficeStaffAttendanceGUID + "')";
                    return Json(DbOSA.SingleDeleteMessage(DeletedLanguages, DataTableNames.OfficeStaffAttendancesDataTable, callBackFunc));
                }
                catch (DbUpdateConcurrencyException)
                {
                    return ConcrrencyOfficeStaffAttendance(model.OfficeStaffAttendanceGUID);
                }
                catch (Exception ex)
                {
                    return Json(DbOSA.ErrorMessage(ex.Message));
                }
            }
            else
            {
                return Json(DbOSA.ErrorMessage("You don't have Owner Permissions.."));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult OfficeStaffAttendancesDataTableDelete(List<dataOfficeStaffAttendance> models)
        {
            if (!CMS.HasAction(Permissions.OfficeStaffAttendance.Delete, Apps.OSA))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataOfficeStaffAttendance> DeletedLanguages = DeleteOfficeStaffAttendances(models);

            try
            {
                DbOSA.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOSA.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.OfficeStaffAttendancesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbOSA.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        private List<dataOfficeStaffAttendance> DeleteOfficeStaffAttendances(List<dataOfficeStaffAttendance> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataOfficeStaffAttendance> DeletedOfficeStaffAttendances = new List<dataOfficeStaffAttendance>();

            string baseQuery = "";

            string query = DbOSA.QueryBuilder(models, Permissions.OfficeStaffAttendance.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var OfficeStaffAttendances = DbOSA.Database.SqlQuery<dataOfficeStaffAttendance>(query).ToList();
            var OfficeStaffAttendanceGUIDs = OfficeStaffAttendances.Select(y => y.OfficeStaffAttendanceGUID).ToList();

            DbOSA.SaveChanges();

            foreach (var OfficeStaffAttendance in OfficeStaffAttendances)
            {
                DeletedOfficeStaffAttendances.Add(DbOSA.Delete(OfficeStaffAttendance, ExecutionTime, Permissions.OfficeStaffAttendance.DeleteGuid, DbCMS));
            }
            return DeletedOfficeStaffAttendances;
        }

        private JsonResult ConcrrencyOfficeStaffAttendance(Guid PK)
        {
            dataOfficeStaffAttendance dbModel = new dataOfficeStaffAttendance();

            var Language = DbOSA.dataOfficeStaffAttendance.Where(l => l.OfficeStaffAttendanceGUID == PK).FirstOrDefault();
            var dbLanguage = DbOSA.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataOfficeStaffAttendanceRowVersion.SequenceEqual(dbModel.dataOfficeStaffAttendanceRowVersion))
            {
                return Json(DbOSA.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbOSA, dbModel, "OfficeStaffAttendancesContainer"));
        }

        private bool ActiveOfficeStaffAttendance(OfficeStaffAttendanceUpdateModel model)
        {
            int error = 0;
            if (model.AttendanceFromDatetime > model.AttendanceToDatetime)
            {
                error++;
                ModelState.AddModelError("AttendanceToDatetime", "(Office presence to) Date Should Be Greater then  (Office presence from) Date.");

            }
            var staffFound = DbOSA.dataOfficeStaffAttendance.Where(x => x.Active && x.UserGUID == model.UserGUID && x.AttendanceFromDatetime == model.AttendanceFromDatetime && x.OfficeStaffAttendanceGUID != model.OfficeStaffAttendanceGUID).FirstOrDefault();
            if (staffFound != null)
            {
                error++;
                string staffName = DbCMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == model.UserGUID && x.Active && x.LanguageID == LAN).Select(x => x.FirstName + " " + x.Surname).FirstOrDefault();

                ModelState.AddModelError("UserGUID", staffName + " already has a (Presence Event) on the same day " + model.AttendanceFromDatetime.Value.ToString("dd MMMM yyyy") + ".");

            }
            //var paymentComermation = DbOSA.dataOfficeStaffAttendanceConfirmation.Where(x => x.UserGUID == UserGUID && x.ConfirmedMonth == model.AttendanceFromDatetime.Value.Month && x.ConfirmedYear == model.AttendanceFromDatetime.Value.Year && x.PaymentConfirmedDate==null).FirstOrDefault();
            //if (paymentComermation != null)
            //{
            //    error++;
            //    string staffName = DbCMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == model.UserGUID && x.Active && x.LanguageID == LAN).Select(x => x.FirstName + " " + x.Surname).FirstOrDefault();

            //    ModelState.AddModelError("UserGUID", staffName + " already has a (Presence Event) on the same day " + model.AttendanceFromDatetime.Value.ToString("dd MMMM yyyy") + ".");

            //}
            var staffInfo = DbOSA.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            //Damascues Room Field Required
            if (staffInfo.DutyStationGUID.ToString().ToUpper() == "6D7397D6-3D7F-48FC-BFD2-18E69673AC92")
            {
                //if (model.OfficeGUID == null)
                //{
                //    error++;
                //    ModelState.AddModelError("OfficeGUID", "Building field is required");
                //}
                //if (model.OfficeFloorGUID == null)
                //{
                //    error++;
                //    ModelState.AddModelError("OfficeFloorGUID", "Floor field is required");
                //}
                //if (model.OfficeFloorRoomGUID == null)
                //{
                //    error++;
                //    ModelState.AddModelError("OfficeFloorRoomGUID", "Room field is required");
                //}
                //else
                //{

                //    var RoomAvailability = (from a in DbOSA.dataOfficeStaffAttendance.Where(x => x.Active && x.OfficeFloorRoomGUID == model.OfficeFloorRoomGUID && x.AttendanceFromDatetime == model.AttendanceFromDatetime)
                //                            join b in DbOSA.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.UserGUID equals b.UserGUID

                //                            select new
                //                            {
                //                                StaffName = b.FirstName + " " + b.Surname
                //                            }
                //                           ).ToList();
                //    //Drivers are Exculded from the rule
                //    var staffCore = DbOSA.StaffCoreData.Where(x => x.UserGUID == model.UserGUID).FirstOrDefault();
                //    string[] JobTitleGuids = new string[] { "7FFA5C6F-FA42-4DC4-A072-2BD5C4ED53C8", "4521C4A1-CDDF-4889-9626-2BDF8CF74F02", "1994A889-1F22-43C0-BF40-759143CFF286" };
                //    string[] OfficeGuids = new string[] { "84687DFA-D7AE-483C-9E29-9626E5A80111" };
                //    //if (!JobTitleGuids.Contains(staffCore.JobTitleGUID.ToString().ToUpper()))
                //    //{
                //    //    if (!OfficeGuids.Contains(model.OfficeFloorRoomGUID.ToString().ToUpper()))
                //    //    {
                //    //        //20% Staff Presence 
                //    //        if (RoomAvailability.Count() >= 1)
                //    //        {
                //    //            if (!CMS.HasAction(Permissions.OfficeStaffAttendance.Confirm, Apps.OSA))
                //    //            {
                //    //                error++;
                //    //                ModelState.AddModelError("OfficeFloorRoomGUID", "The selected room is already booked by " + RoomAvailability.FirstOrDefault().StaffName + ". Please select another room.");
                //    //            }
                //    //        }
                //    //    }
                //    //}

                //    //if (!JobTitleGuids.Contains(staffCore.JobTitleGUID.ToString().ToUpper()))
                //    //{
                //    //    if (!OfficeGuids.Contains(model.OfficeFloorRoomGUID.ToString().ToUpper()))
                //    //    {

                //    //        //50% Staff Presence 
                //    //        if (!CMS.HasAction(Permissions.OfficeStaffAttendance.Confirm, Apps.OSA))
                //    //        {
                //    //            var RoomSize = DbOSA.codeOfficeFloorRoom.Where(x => x.OfficeFloorRoomGUID == model.OfficeFloorRoomGUID).FirstOrDefault();
                //    //            if (RoomSize.RoomSize > 0 && RoomSize.RoomSize <= 10)
                //    //            {
                //    //                if (RoomAvailability.Count() == 1)
                //    //                {
                //    //                    error++;
                //    //                    ModelState.AddModelError("OfficeFloorRoomGUID", "Room Already Booked Max 1 Person," + " Booked By " + RoomAvailability[0].StaffName);
                //    //                }
                //    //            }
                //    //            if (RoomSize.RoomSize > 10 && RoomSize.RoomSize <= 18)
                //    //            {
                //    //                if (RoomAvailability.Count() == 2)
                //    //                {
                //    //                    error++;
                //    //                    ModelState.AddModelError("OfficeFloorRoomGUID", "Room Already Booked Max 2 Person," + " Booked By " + RoomAvailability[0].StaffName + ", " + RoomAvailability[1].StaffName);
                //    //                }
                //    //            }
                //    //            if (RoomSize.RoomSize > 18)
                //    //            {
                //    //                if (RoomAvailability.Count() == 3)
                //    //                {
                //    //                    error++;
                //    //                    ModelState.AddModelError("OfficeFloorRoomGUID", "Room Already Booked Max 3 Person," + " Booked By " + RoomAvailability[0].StaffName + ", " + RoomAvailability[1].StaffName + " and " + RoomAvailability[2].StaffName);
                //    //                }
                //    //            }
                //    //        }
                //    //    }
                //    //}

                //}
            }
            return (error > 0);
        }

        public ActionResult OfficeStaffAttendanceStaffCalendar()
        {
            DateTime date = Convert.ToDateTime(Request.Params["Date"]);
            ViewBag.MonthConfirmed = DbOSA.dataOfficeStaffAttendanceConfirmation.Where(x => x.UserGUID == UserGUID && x.ConfirmedMonth == date.Month && x.ConfirmedYear == date.Year && x.Active).Count();

                return View();
        }

        public ActionResult GetOfficeStaffAttendanceStaffCalendarData(DateTime start, DateTime end, Guid? Guid)
        {
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.OfficeStaffAttendance.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();
            //Guid excludeDepartment = Guid.Parse("B614D2F6-B19D-4E12-A4D5-604FCF77B234");
            //Guid includeDutyStation = Guid.Parse("6d7397d6-3d7f-48fc-bfd2-18e69673ac92");
            //var userProfiles = DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            Guid currentUserGUID = Guid == null ? UserGUID : Guid.Value;
            var Result = (from a in DbOSA.dataOfficeStaffAttendance.Where(x => x.Active && x.AttendanceFromDatetime >= start && x.AttendanceFromDatetime <= end && x.UserGUID == currentUserGUID)
                          join b in DbOSA.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.UserGUID equals b.UserGUID
                          join c in DbOSA.StaffCoreData on a.UserGUID equals c.UserGUID
                          let start_ = a.AttendanceFromDatetime
                          let end_ = (a.AttendanceToDatetime.Value != null ? a.AttendanceToDatetime.Value : a.AttendanceFromDatetime)
                          orderby b.FirstName + " " + b.Surname
                          select new CalendarEvents
                          {
                              id = a.OfficeStaffAttendanceGUID,
                              start = start_,
                              end = end_,
                              Month = start_.Month,
                              Day = start_.Day,
                              IsAttend = a.IsAttend,
                              title = b.FirstName + " " + b.Surname,
                              DutyStationGUID = c.DutyStationGUID,
                              DepartmentGUID =c.DepartmentGUID,
                              color = !a.IsConfirmed ? "red" : (a.IsAttend ? "#6aa16a" : "#337ab7"),
                              IsConfirmed = a.IsConfirmed,
                              UserGUID = a.UserGUID,

                          }).ToList();

            var DutyStationGUID = (from a in DbOSA.StaffCoreData where a.UserGUID == UserGUID select a.DutyStationGUID).FirstOrDefault();
            var DutyStationStaffCount = (from a in DbOSA.StaffCoreData
                                         where a.Active
                                          && a.DutyStationGUID == DutyStationGUID
                                         select a).Count();
            var jsonResult = Json(new { Result, DutyStationStaffCount, DutyStationGUID }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult OfficeStaffAttendanceCalendar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetOfficeStaffAttendanceCalendarData(DateTime start, DateTime end, Guid? ParaUserGUID)
        {
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.OfficeStaffAttendance.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();
            Guid excludeDepartment = Guid.Parse("B614D2F6-B19D-4E12-A4D5-604FCF77B234");
            //Guid includeDutyStation = Guid.Parse("6d7397d6-3d7f-48fc-bfd2-18e69673ac92");
            var userProfiles = DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            var Result = (from a in DbOSA.dataOfficeStaffAttendance.Where(x => x.Active && x.AttendanceFromDatetime >= start && x.AttendanceFromDatetime <= end)
                          join b in DbOSA.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.UserGUID equals b.UserGUID
                          join c in DbOSA.StaffCoreData on a.UserGUID equals c.UserGUID
                          let start_ = a.AttendanceFromDatetime
                          let end_ = (a.AttendanceToDatetime.Value != null ? a.AttendanceToDatetime.Value : a.AttendanceFromDatetime)
                          orderby b.FirstName + " " + b.Surname
                          select new CalendarEvents
                          {
                              id = a.OfficeStaffAttendanceGUID,
                              start = start_,
                              end = end_,
                              Month = start_.Month,
                              Day = start_.Day,
                              IsAttend = a.IsAttend,
                              title = b.FirstName + " " + b.Surname,
                              DutyStationGUID = c.DutyStationGUID,
                              DepartmentGUID = c.DepartmentGUID == null ? Guid.Empty : c.DepartmentGUID,
                              color = a.IsAttend ? "#6aa16a" : "#337ab7",
                              IsConfirmed = a.IsConfirmed,
                              UserGUID = a.UserGUID,

                          }).ToList();
            if (ParaUserGUID != null)
            {
                Result = Result.Where(x => x.UserGUID == ParaUserGUID).ToList();
            }
            var DutyStationGUID = (from a in DbOSA.StaffCoreData where a.UserGUID == UserGUID select a.DutyStationGUID).FirstOrDefault();
            var DutyStationStaffCount = (from a in DbOSA.StaffCoreData
                                         where a.Active
                                          && a.DepartmentGUID != excludeDepartment && a.DutyStationGUID == DutyStationGUID
                                         select a).Count();
            var jsonResult = Json(new { Result, DutyStationStaffCount, DutyStationGUID }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult GetWorkingDay()
        {
            Guid OrganizationInstanceGUID = Guid.Parse("e156c022-ec72-4a5a-be09-163bd85c68ef");
            var userProfiles = DbOSA.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            Guid DutyStationConfigurationGUID = DbCMS.codeDutyStationsConfigurations.Where(x => x.DutyStationGUID == userProfiles.DutyStationGUID && x.OrganizationInstanceGUID == OrganizationInstanceGUID).FirstOrDefault().DutyStationConfigurationGUID;

            var WorkingDay = (from a in DbCMS.codeWorkingDaysConfigurations.Where(x => x.Active && x.DutyStationConfigurationGUID == DutyStationConfigurationGUID)
                              join b in DbCMS.codeTablesValues on a.DayGUID equals b.ValueGUID
                              select new WorkDay { Day = b.SortID.Value }).ToList();
            return Json(new JsonReturn { WorkDays = WorkingDay });
        }

        [HttpPost]
        public ActionResult UpdateOfficeStaffAttendanceDate(Guid PK, DateTime NewDate)
        {
            DateTime ExecutionTime = DateTime.Now;
            var OfficeStaffAttendance = DbOSA.dataOfficeStaffAttendance.Where(x => x.OfficeStaffAttendanceGUID == PK).FirstOrDefault();
            var Audit = DbCMS.spAuditHistory(LAN, OfficeStaffAttendance.OfficeStaffAttendanceGUID).OrderBy(x => x.ExecutionTime).FirstOrDefault();
            string staffName = DbCMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == UserGUID && x.Active && x.LanguageID == LAN).Select(x => x.FirstName + " " + x.Surname).FirstOrDefault();
            if (staffName == Audit.ExecutedBy)
            {
                OfficeStaffAttendance.AttendanceFromDatetime = NewDate;
                OfficeStaffAttendance.AttendanceToDatetime = NewDate;
                OfficeStaffAttendanceUpdateModel model = new OfficeStaffAttendanceUpdateModel();
                Mapper.Map(OfficeStaffAttendance, model);
                if (!ModelState.IsValid || ActiveOfficeStaffAttendance(model))
                {
                    string desc = "";
                    foreach (ModelState modelState in ViewData.ModelState.Values)
                    {
                        foreach (ModelError error in modelState.Errors)
                        {
                            desc = desc + " " + error.ErrorMessage.ToString();  //here 'error.ErrorMessage' will have required error message                                              //DoSomethingWith(error.ErrorMessage.ToString());
                        }
                    }
                    return Json(DbOSA.ErrorMessage(desc));
                }

                DbOSA.Update(OfficeStaffAttendance, Permissions.OfficeStaffAttendance.UpdateGuid, ExecutionTime, DbCMS);

                try
                {
                    DbOSA.SaveChanges();
                    DbCMS.SaveChanges();
                    return Json(DbOSA.SingleUpdateMessage(DataTableNames.OfficeStaffAttendancesDataTable, DbOSA.PrimaryKeyControl(OfficeStaffAttendance), DbOSA.RowVersionControls(Portal.SingleToList(OfficeStaffAttendance))));
                }
                catch (Exception ex)
                {
                    return Json(DbOSA.ErrorMessage(ex.Message));
                }
            }
            else
            {
                return Json(DbOSA.ErrorMessage("You don't have Owner Permissions.."));

            }
        }

        [HttpPost]
        public ActionResult UpadetOfficeStaffAttendanceEvents(Guid PK)
        {
            return Json(new
            {
                OfficeStaffAttendance = DbOSA.dataOfficeStaffAttendance.Where(x => x.OfficeStaffAttendanceGUID == PK).Select(x => new
                {
                    x.OfficeStaffAttendanceGUID,
                    x.IsAttend,
                    x.AttendanceFromDatetime
                })
            });
        }

        [HttpPost]
        public ActionResult RP_StaffPresence(DateTime date)
        {
            if (!CMS.HasAction(Permissions.OfficeStaffAttendance.Print, Apps.OSA))
            {
                return Json(new { Success = false });
            }
            else
            {
                string URL = "/OfficeStaffAttendances/RP_StaffPresenceLoad?date=" + date;
                return Json(new { URL = URL, Success = true });
            }
        }
        public void RP_StaffPresenceLoad(DateTime date)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;

            reportViewer.AsyncRendering = true;
            reportViewer.LocalReport.DataSources.Clear();
            OSADataSet osaDataSet_EN = new OSADataSet();

            StaffPresenceTableAdapter RP_NoteVerbale_EN = new StaffPresenceTableAdapter();
            RP_NoteVerbale_EN.Fill(osaDataSet_EN.StaffPresence, date.Month, date.Year);
            ReportDataSource reportDataSource_EN = new ReportDataSource("StaffPresence", osaDataSet_EN.Tables["StaffPresence"]);
            reportViewer.LocalReport.DataSources.Add(reportDataSource_EN);
            

            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Areas/OSA/Rdlc/StaffPresence.rdlc";
            ReportParameter paramMonth = new ReportParameter("Month", date.Month.ToString(), false);
            ReportParameter paramYear = new ReportParameter("Year", date.Year.ToString(), false);
            reportViewer.LocalReport.SetParameters(paramMonth);
            reportViewer.LocalReport.SetParameters(paramYear);
            ViewBag.ReportViewer = reportViewer;

            Warning[] warnings;
            string[] streamIds;
            string contentType;
            string encoding;
            string extension;

            //Export the RDLC Report to Byte Array.
            byte[] bytes = reportViewer.LocalReport.Render("EXCEL", null, out contentType, out encoding, out extension, out streamIds, out warnings);

            //Download the RDLC Report in Word, Excel, PDF and Image formats.
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = contentType;
            Response.AppendHeader("Content-Disposition", "attachment; filename=RDLC." + extension);
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }

        [HttpPost]
        public ActionResult InformSupervisor(DateTime date,Guid? reportToGUID)
        {

            bool recordFound = false;
            var staffInfo = DbOSA.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            var confirmed = DbOSA.dataOfficeStaffAttendanceConfirmation.Where(x => x.UserGUID == UserGUID && x.ConfirmedMonth == date.Month && x.ConfirmedYear == date.Year && x.Active).FirstOrDefault();

            if (confirmed == null)
            {
                new Email().InformSupervisorAttendances(UserGUID, reportToGUID.Value, date);
                recordFound = true;
            }
            return Json(new { MailSuccess = recordFound });
        }

        public ActionResult NotifyDirectSupuervisor()
        {
            var staffInfo = DbOSA.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            return PartialView("~/Areas/OSA/Views/OfficeStaffAttendances/_NotifyDirectSupuervisor.cshtml",
            new MasterRecordStatus{
                ParentGUID = staffInfo.ReportToGUID.Value,
            });

        }

        [HttpPost]
        public ActionResult ConfirmSupervisor(Guid ConfirmedUserGUID, DateTime date)
        {
            bool recordFound = false;
            Guid ReportToGuid = UserGUID;
            var confirmed = DbOSA.dataOfficeStaffAttendanceConfirmation.Where(x => x.UserGUID == ConfirmedUserGUID && x.ConfirmedMonth == date.Month && x.ConfirmedYear == date.Year && x.Active).FirstOrDefault();
            if (confirmed == null)
            {
                dataOfficeStaffAttendanceConfirmation confirmation = new dataOfficeStaffAttendanceConfirmation();
                confirmation.ConfirmedBy = CMS.GetFullName(UserGUID, LAN);
                confirmation.ConfirmedDate = DateTime.Now;
                confirmation.ConfirmedYear = date.Year;
                confirmation.ConfirmedMonth = date.Month;
                confirmation.OfficeStaffAttendanceConfirmationGUID = Guid.NewGuid();
                confirmation.ReportToGUID = ReportToGuid;
                confirmation.UserGUID = ConfirmedUserGUID;
                confirmation.Active = true;
                DbOSA.dataOfficeStaffAttendanceConfirmation.Add(confirmation);

                var UpdateListNotConfirmed = DbOSA.dataOfficeStaffAttendance.Where(x => x.UserGUID == ConfirmedUserGUID && x.AttendanceFromDatetime.Month == date.Month && x.AttendanceFromDatetime.Year == date.Year && x.Active && !x.IsConfirmed).ToList();
                UpdateListNotConfirmed.ForEach(x =>
                {
                    x.IsConfirmed = true;
                    x.IsAttend = true;
                    x.OfficeStaffAttendanceConfirmationGUID = confirmation.OfficeStaffAttendanceConfirmationGUID;
                });

                var UpdateListConfirmed = DbOSA.dataOfficeStaffAttendance.Where(x => x.UserGUID == ConfirmedUserGUID && x.AttendanceFromDatetime.Month == date.Month && x.AttendanceFromDatetime.Year == date.Year && x.Active && x.IsConfirmed).ToList();
                UpdateListConfirmed.ForEach(x =>
                {
                    x.OfficeStaffAttendanceConfirmationGUID = confirmation.OfficeStaffAttendanceConfirmationGUID;
                });
                DbOSA.SaveChanges();
                recordFound = true;

                //new Email().SupervisorAttendancesConfirmation(ConfirmedUserGUID, ReportToGuid);

                //new Email().SupervisorAttendancesConfirmation(ConfirmedUserGUID, ReportToGuid);
            }
            return Json(new { MailSuccess = recordFound });
        }

        [HttpPost]
        public ActionResult CancelSupervisor(Guid ConfirmedUserGUID, DateTime date)
        {
            bool recordFound = false;
            var confirmed = DbOSA.dataOfficeStaffAttendance.Where(x => x.UserGUID == ConfirmedUserGUID && !x.IsConfirmed && x.AttendanceFromDatetime.Month == date.Month && x.AttendanceFromDatetime.Year == date.Year && x.Active).ToList();
            if (confirmed.Count > 0)
            {
                DbOSA.dataOfficeStaffAttendance.RemoveRange(confirmed);
                DbOSA.SaveChanges();

                recordFound = true;
            }
            new Email().SupervisorAttendancesNoConfirmation(ConfirmedUserGUID);
            return Json(new { MailSuccess = recordFound });
        }

        [HttpPost]
        public ActionResult ConfirmOfficeAttendance(Guid PK)
        {
            var confirmed = DbOSA.dataOfficeStaffAttendance.Where(x => x.OfficeStaffAttendanceGUID == PK && x.Active).FirstOrDefault();
            confirmed.IsConfirmed = false;
            DbOSA.SaveChanges();
            return Json(new { Success = true });
        }


        [HttpPost]
        public ActionResult BroadCastMail(DateTime date)
        {
            if (!CMS.HasAction(Permissions.OfficeStaffAttendance.Send, Apps.OSA))
            {
                return Json(new { Success = false }) ;
            }
            else
            {
                Guid OrganizationInstanceGUID = Session[SessionKeys.OrganizationInstanceGUID] != null ? Guid.Parse(Session[SessionKeys.OrganizationInstanceGUID].ToString()) : Guid.Empty;
 
                var activeStaff = DbAHD.v_staffCoreDataOverview.Where(x => x.StaffStatus == "Active(Still in the operation)" && x.RecruitmentType == "National" && x.OrganizationInstanceGUID == OrganizationInstanceGUID).ToList();
                var confirmedStaffs = DbOSA.dataOfficeStaffAttendanceConfirmation.Where(x => x.ConfirmedMonth == date.Month && x.ConfirmedYear == date.Year).ToList();
                foreach (var staff in activeStaff)
                {
                    if (staff.EmailAddress.Contains("HAFEZ"))
                    {
                        if (confirmedStaffs.Where(x => x.UserGUID == staff.UserGUID).FirstOrDefault() == null)
                        {

                            new Email().BroadCastMail(date, staff.UserGUID);


                    }
                    }
                }
                return Json(new { Success = true });
            }
        }


        public ActionResult AttendanceStaffImportCheck()
        {
            if (!CMS.HasAction(Permissions.OfficeStaffAttendance.Import, Apps.OSA))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return PartialView("~/Areas/OSA/Views/OfficeStaffAttendances/_StaffImportModal.cshtml",
                new OfficeStaffAttendanceUpdateModel() );
        }

        [HttpPost]
        public ActionResult AttendanceStaffImportCheck(OfficeStaffAttendanceUpdateModel model)
        {
            if (HttpContext != null && HttpContext.Request.Files["file"].ContentLength > 0)
            {
                DataTable ds = importFile.ImportDataSet();
                List<StaffImportModel> staffs = new List<StaffImportModel>();
                var staffList = (from a in DbOSA.StaffCoreData 
                                 join b in DbOSA.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.UserGUID equals b.UserGUID
                                 select new
                                 {
                                     UserGUID = a.UserGUID,
                                     FullName = b.FirstName + " " + b.Surname,
                                     EmploymentID = a.EmploymentID
                                 }).ToList();
                int EmploymentID = 0;
                for (int i = 0; i <= ds.Rows.Count; i++)
                {

                    try
                    {
                        StaffImportModel staff = new StaffImportModel();
                        if (ds.Rows[i][1].ToString() != "")
                        {
                            EmploymentID = Convert.ToInt32(ds.Rows[i][0]);
                            staff.FullName = ds.Rows[i][1].ToString();
                            staff.DateKey = Convert.ToDateTime(ds.Rows[i][2]).Date;
                            staff.UserGUID = (staffList.Where(x => x.FullName == staff.FullName).FirstOrDefault() != null ?
                                staffList.Where(x => x.FullName == staff.FullName).FirstOrDefault().UserGUID :
                                staffList.Where(x => x.EmploymentID == EmploymentID).FirstOrDefault().UserGUID);
                            try
                            {
                                if (ds.Rows[i][3].ToString() != "")
                                {
                                    staff.WorkingTime = int.Parse(ds.Rows[i][3].ToString()).ToString();
                                }
                                else
                                {
                                    staff.WorkingTime = "0";
                                }
                            }
                            catch
                            {
                                DateTime dateVal = Convert.ToDateTime(ds.Rows[i + 1][2]).Date;
                                if (dateVal == staff.DateKey)
                                {
                                    TimeSpan duration = DateTime.Parse(ds.Rows[i + 1][3].ToString()).Subtract(DateTime.Parse(ds.Rows[i][3].ToString()));
                                    staff.WorkingTime = ((duration.Hours * 60) + duration.Minutes).ToString();
                                    i = i + 1;
                                }
                                else
                                {
                                    staff.WorkingTime = TimeSpan.Parse(ds.Rows[i][3].ToString().Replace("h", ":").Replace("min", "")).TotalMinutes.ToString();
                                }
                            }

                            if (Convert.ToInt32(staff.WorkingTime) >= 210)
                            {
                                var test1 = DbOSA.dataOfficeStaffAttendance.Where(x => x.AttendanceFromDatetime == staff.DateKey.Date && x.UserGUID == staff.UserGUID && x.IsAttend && x.Active).FirstOrDefault();
                                var test2 = staffs.Where(x => x.DateKey == staff.DateKey.Date && x.UserGUID == staff.UserGUID).FirstOrDefault();

                                if (test1 == null && test2 == null)
                                {
                                    staffs.Add(staff);
                                }
                            }
                            //ramandan work time 2023
                            DateTime fromDate = new DateTime(2023, 3, 23);
                            DateTime ToDate = new DateTime(2023, 4, 21);
                            if (Convert.ToInt32(staff.WorkingTime) >= 180 && staff.DateKey>= fromDate && staff.DateKey<=ToDate) 
                            {
                                var test1 = DbOSA.dataOfficeStaffAttendance.Where(x => x.AttendanceFromDatetime == staff.DateKey.Date && x.UserGUID == staff.UserGUID && x.IsAttend && x.Active).FirstOrDefault();
                                var test2 = staffs.Where(x => x.DateKey == staff.DateKey.Date && x.UserGUID == staff.UserGUID).FirstOrDefault();

                                if (test1 == null && test2 == null)
                                {
                                    staffs.Add(staff);
                                }
                            }
                        }
                    }
                    catch (Exception ex) { }
                }

                return Json(new { data = staffs }, JsonRequestBehavior.AllowGet);
            }
            return Json(DbOSA.ErrorMessage("Please Chose file"));

        }

        private static IDictionary<Guid, int> processCalculationTask = new Dictionary<Guid, int>();

        [HttpPost]
        public ActionResult AttendanceStaffImport(List<StaffImportModel> model)
        {
            if (!CMS.HasAction(Permissions.OfficeStaffAttendance.Import, Apps.OSA))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var processCalculationTaskId = Guid.NewGuid();
            processCalculationTask.Add(processCalculationTaskId, 0);

            
            if (model.Count == 0)
            {
                return Json(new { NothingToProcess = true }, JsonRequestBehavior.AllowGet);
            }
            Task.Factory.StartNew(() =>
            {
                DbOSA = new Entities();
                DbCMS = new Models.CMSEntities();

                List<dataOfficeStaffAttendance> staffsToBeAdded = new List<dataOfficeStaffAttendance>();
                List<dataOfficeStaffAttendance> staffsToBeUpdated = new List<dataOfficeStaffAttendance>();
                DateTime executionTime = DateTime.Now;
                int index = 0;

                foreach (var item in model)
                {
                    processCalculationTask[processCalculationTaskId] = index++;
                    dataOfficeStaffAttendance staff =
                            DbOSA.dataOfficeStaffAttendance.Where(a => a.AttendanceFromDatetime == item.DateKey & a.UserGUID == item.UserGUID).FirstOrDefault();
                    if (staff == null)
                    {
                        staff = new dataOfficeStaffAttendance();
                        staff.OfficeStaffAttendanceGUID = Guid.NewGuid();
                        staff.AttendanceFromDatetime = item.DateKey;
                        staff.AttendanceToDatetime = item.DateKey;
                        staff.UserGUID = item.UserGUID;
                        staff.WorkingHours = int.Parse(item.WorkingTime);
                        staff.IsConfirmed = false;
                        staff.Active = true;
                        staff.IsAttend = true;
                        staffsToBeAdded.Add(staff);
                    }
                    else
                    {
                        staff.IsAttend = true;
                        staff.Active = true;
                        staffsToBeUpdated.Add(staff);
                    }
                }
                try
                {
                    DbOSA.dataOfficeStaffAttendance.AddRange(staffsToBeAdded);
                    DbOSA.SaveChanges();
                    DbCMS.SaveChanges();
                    processCalculationTask[processCalculationTaskId] = index++;
                }
                catch (Exception ex)
                {
                }
            });

            return Json(new { processCalculationTaskId = processCalculationTaskId, TotalRecords = model.Count });



        }

        public ActionResult ProcessCalculationProgres(Guid id)
        {
            return Json(processCalculationTask.Keys.Where(x => x.ToString() == id.ToString()).Contains(id) ? processCalculationTask[id] : -100);
        }


        [HttpPost]
        public ActionResult ConfirmPayment(DateTime date, string FulteredUsers)
        {
            if (!CMS.HasAction(Permissions.OfficeStaffAttendance.ValidateData, Apps.OSA))
            {
                return Json(new { Success = false });
            }
            else
             {
                Guid? FulteredUsersGuid;
                if (FulteredUsers != null)
                {
                    FulteredUsersGuid = (from a in DbOSA.userPersonalDetailsLanguage.Where(x => (x.FirstName + " " + x.Surname) == FulteredUsers && x.Active)
                                         join b in DbOSA.StaffCoreData on a.UserGUID equals b.UserGUID
                                         select b).FirstOrDefault().UserGUID;
                    var paymentComermation = DbOSA.dataOfficeStaffAttendanceConfirmation.Where(x => x.UserGUID == FulteredUsersGuid && x.ConfirmedMonth == date.Month).FirstOrDefault();
                    if (paymentComermation != null)
                    {
                        if (paymentComermation.PaymentConfirmedDate == null)
                        {
                            paymentComermation.PaymentConfirmedBy = CMS.GetFullName(UserGUID, LAN);
                            paymentComermation.PaymentConfirmedDate = DateTime.Now;
                            DbOSA.SaveChanges();
                            return Json(new { Success = true });
                        }
                    }
                    return Json(new { Success = "" });
                }
                return Json(new { Success = true });

            }
        }

        [HttpPost]
        public ActionResult CancelConfirmation(DateTime date, string FulteredUsers)
        {
            if (!CMS.HasAction(Permissions.OfficeStaffAttendance.Remove, Apps.OSA))
            {
                return Json(new { Success = false });
            }
            else
            {
                Guid? FulteredUsersGuid;
                if (FulteredUsers != null)
                {
                    FulteredUsersGuid = (from a in DbOSA.userPersonalDetailsLanguage.Where(x => (x.FirstName + " " + x.Surname) == FulteredUsers && x.Active)
                                         join b in DbOSA.StaffCoreData on a.UserGUID equals b.UserGUID
                                         select b).FirstOrDefault().UserGUID; 
                    var cancelConfirmation = DbOSA.dataOfficeStaffAttendanceConfirmation.Where(x => x.UserGUID == FulteredUsersGuid && x.ConfirmedMonth == date.Month).FirstOrDefault();
                    if (cancelConfirmation != null)
                    {
                        DbOSA.dataOfficeStaffAttendanceConfirmation.Remove(cancelConfirmation);
                        DbOSA.SaveChanges();

                        return Json(new { Success = true });
                    }
                    return Json(new { Success = "" });
                }
                return Json(new { Success = true });
            }
        }


        public ActionResult AttendancePaymentImportCheck()
        {
            if (!CMS.HasAction(Permissions.OfficeStaffAttendance.Import, Apps.OSA))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return PartialView("~/Areas/OSA/Views/OfficeStaffAttendances/_PaymentImportModal.cshtml",
                new OfficeStaffAttendanceUpdateModel());
        }

        [HttpPost]
        public ActionResult AttendancePaymentImportCheck(OfficeStaffAttendanceUpdateModel model)
        {
            if (HttpContext != null && HttpContext.Request.Files["file"].ContentLength > 0)
            {
                DataTable ds = importFile.ImportDataSet();
                List<StaffImportModel> staffs = new List<StaffImportModel>();
                var staffList = (from a in DbOSA.StaffCoreData
                                 join b in DbOSA.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.UserGUID equals b.UserGUID
                                 select new
                                 {
                                     UserGUID = a.UserGUID,
                                     FullName = b.FirstName + " " + b.Surname,
                                     EmploymentID = a.EmploymentID
                                 }).ToList();
              
                for (int i = 1; i <= ds.Rows.Count; i++)
                {

                    try
                    {
                        StaffImportModel staff = new StaffImportModel();

                             staff.EmployeeID =Convert.ToInt32( ds.Rows[i][0]);
                            staff.ConfirmedDate = Convert.ToDateTime(ds.Rows[i][1]).Date;
                            staff.ConfirmedBy = ds.Rows[i][2].ToString();
                            staff.Month = Convert.ToInt32(ds.Rows[i][3].ToString());
                            staff.Year = Convert.ToInt32(ds.Rows[i][4].ToString());
                            staff.UserGUID = staffList.Where(x => x.EmploymentID == staff.EmployeeID).FirstOrDefault().UserGUID;
                        staffs.Add(staff);
                    }
                    catch (Exception ex) { }
                }

                return Json(new { data = staffs }, JsonRequestBehavior.AllowGet);
            }
            return Json(DbOSA.ErrorMessage("Please Chose file"));

        }

        [HttpPost]
        public ActionResult AttendancePaymentImport(List<StaffImportModel> model)
        {
            if (!CMS.HasAction(Permissions.OfficeStaffAttendance.ValidateData, Apps.OSA))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var processCalculationTaskId = Guid.NewGuid();
            processCalculationTask.Add(processCalculationTaskId, 0);


            if (model.Count == 0)
            {
                return Json(new { NothingToProcess = true }, JsonRequestBehavior.AllowGet);
            }
            Task.Factory.StartNew(() =>
            {
                DbOSA = new Entities();

                int index = 0;

                foreach (var item in model)
                {
                    processCalculationTask[processCalculationTaskId] = index++;
                    var staffConfirmPayment = DbOSA.dataOfficeStaffAttendanceConfirmation.Where(a => a.UserGUID == item.UserGUID && a.ConfirmedMonth==item.Month && a.ConfirmedYear==item.Year ).FirstOrDefault();
                    if (staffConfirmPayment != null)
                    {
                        staffConfirmPayment.PaymentConfirmedBy = item.ConfirmedBy;
                        staffConfirmPayment.PaymentConfirmedDate = item.ConfirmedDate;
                    }
                }
                try
                {
                    DbOSA.SaveChanges();
                    DbCMS.SaveChanges();
                    processCalculationTask[processCalculationTaskId] = index++;
                }
                catch (Exception ex){}
            });

            return Json(new { processCalculationTaskId = processCalculationTaskId, TotalRecords = model.Count });



        }
        #endregion
    }
    }