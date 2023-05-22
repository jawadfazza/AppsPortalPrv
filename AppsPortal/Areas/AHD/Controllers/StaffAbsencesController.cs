
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
using iTextSharp.text.pdf.qrcode;
using Microsoft.Reporting.WebForms;
using AppsPortal.Areas.AHD.RDLC;
using AppsPortal.Areas.AHD.RDLC.AHDDataSetTableAdapters;
using DocumentFormat.OpenXml.Drawing.ChartDrawing;
using AHD_DAL.Model;
using AHD_DAL.ViewModels;
using System.Configuration;
using System.IO;
using OfficeOpenXml;
using System.Drawing;

namespace AppsPortal.Areas.AHD.Controllers
{
    public class StaffAbsencesController : AHDBaseController
    {
        #region  Staff Absences 

        public ActionResult Index()
        {
            return View("~/Areas/AHD/Views/StaffAbsence/Index.cshtml");
        }
        public ActionResult IndexConfirming()
        {
            return View("~/Areas/AHD/Views/StaffAbsence/IndexConfirming.cshtml");
        }

        //[Route("EMT/StaffAbsencesDataTable/{PK}")]
        public ActionResult StaffAbsencesDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<StaffAbsencesDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<StaffAbsencesDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action

            var All = (from a in DbAHD.dataStaffAbsence.AsNoTracking().AsExpandable().Where(x => x.UserGUID == UserGUID)
                       join b in DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.UserGUID equals b.UserGUID
                       join c in DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.DirectSupervisor equals c.UserGUID
                       join d in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.AbsenceTypeGuid equals d.ValueGUID
                       select new StaffAbsencesDataTableModel
                       {
                           AbsenceTypeGuid = a.AbsenceTypeGuid.ToString(),
                           AbsenceFrom = a.AbsenceFrom,
                           AbsenceTo = a.AbsenceTo,
                           UserGUID = a.UserGUID.ToString(),
                           AbsenceType = d.ValueDescription,
                           StaffName = b.FirstName + " " + b.Surname,
                           DutyStationGUID = a.DutyStationGUID,
                           SupervisorComfirmed = a.SupervisorComfirmed ? "Yes" : "No",
                           dataStaffAbsenceGuid = a.dataStaffAbsenceGuid,
                           DirectSupervisor = c.FirstName + " " + c.Surname,
                           SupervisorComfirmedDate = a.SupervisorComfirmedDate,
                           Active = a.Active,
                           dataStaffAbsenceRowVersion = a.dataStaffAbsenceRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<StaffAbsencesDataTableModel> Result = Mapper.Map<List<StaffAbsencesDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult StaffAbsencesConfirmingDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<StaffAbsencesDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<StaffAbsencesDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action

            var All = (from a in DbAHD.dataStaffAbsence.AsNoTracking().AsExpandable().Where(x => x.DirectSupervisor == UserGUID)
                       join b in DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.UserGUID equals b.UserGUID
                       join c in DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.DirectSupervisor equals c.UserGUID
                       join d in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.AbsenceTypeGuid equals d.ValueGUID
                       select new StaffAbsencesDataTableModel
                       {
                           AbsenceTypeGuid = a.AbsenceTypeGuid.ToString(),
                           AbsenceFrom = a.AbsenceFrom,
                           AbsenceTo = a.AbsenceTo,
                           UserGUID = a.UserGUID.ToString(),
                           AbsenceType = d.ValueDescription,
                           StaffName = b.FirstName + " " + b.Surname,
                           DutyStationGUID = a.DutyStationGUID,
                           SupervisorComfirmed = a.SupervisorComfirmed ? "Yes" : "No",
                           dataStaffAbsenceGuid = a.dataStaffAbsenceGuid,
                           DirectSupervisor = c.FirstName + " " + c.Surname,
                           SupervisorComfirmedDate = a.SupervisorComfirmedDate,
                           AbsenceDuration = a.AbsenceDays.Value * a.AbsenceDuration,
                           Active = a.Active,
                           dataStaffAbsenceRowVersion = a.dataStaffAbsenceRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<StaffAbsencesDataTableModel> Result = Mapper.Map<List<StaffAbsencesDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }



        public ActionResult StaffAbsenceCreate()
        {

            //if (!CMS.HasAction(Permissions.StaffAbsence.Create, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var staffInfo = DbAHD.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            return PartialView("~/Areas/AHD/Views/StaffAbsence/_StaffAbsenceForm.cshtml", new StaffAbsenceUpdateModel()
            {
                //IsInternationalStaff = /*staffInfo.IsInternational.Value ? true : false,*/ false,
                SupervisorComfirmed = false,
                UserGUID = UserGUID,
                DutyStationGUID = staffInfo.DutyStationGUID,
                DirectSupervisor = staffInfo.ReportToGUID.ToString()
            });

        }
        public ActionResult StaffAbsenceUpdate(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.StaffAbsence.Access, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var staffInfo = DbAHD.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();

            var staffAttendance = (from a in DbAHD.dataStaffAbsence.Where(x => x.dataStaffAbsenceGuid == PK)
                                   join c in DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.DirectSupervisor equals c.UserGUID
                                   select new StaffAbsenceUpdateModel
                                   {
                                       AbsenceTypeGuid = a.AbsenceTypeGuid,
                                       AbsenceFrom = a.AbsenceFrom,
                                       AbsenceTo = a.AbsenceTo,
                                       UserGUID = a.UserGUID,
                                       AbsenceDays = a.AbsenceDays,
                                       DutyStationGUID = a.DutyStationGUID,
                                       SupervisorComfirmed = a.SupervisorComfirmed,
                                       dataStaffAbsenceGuid = a.dataStaffAbsenceGuid,
                                       AbsenceDuration =  a.AbsenceDuration,
                                       DirectSupervisor = a.DirectSupervisor.ToString(),
                                       SupervisorComfirmedDate = a.SupervisorComfirmedDate,
                                       Active = a.Active,
                                       dataStaffAbsenceRowVersion = a.dataStaffAbsenceRowVersion,
                                       CurrentUserGUID = UserGUID,
                                       StaffName = c.FirstName + " " + c.Surname
                                   }
                                   ).FirstOrDefault();

            return PartialView("~/Areas/AHD/Views/StaffAbsence/_StaffAbsenceForm.cshtml", staffAttendance);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffAbsenceCreate(StaffAbsenceUpdateModel model)
        {
            DateTime ExecutionTime = DateTime.Now;
            //if (!CMS.HasAction(Permissions.StaffAbsence.Create, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            /////////////////////////////////////////
            if (!ModelState.IsValid || ActiveStaffAbsence(model)) return PartialView("~/Areas/AHD/Views/StaffAbsence/_StaffAbsenceForm.cshtml", model);
            dataStaffAbsence StaffAbsence = new dataStaffAbsence();
            List<DateTime> holiday = new List<DateTime>();
            holiday.AddRange(DbAHD.codeOrganizationHoliday.Select(x => x.HolidayStartDate).ToList());
            holiday.AddRange(DbAHD.codeOrganizationHoliday.Select(x => x.HolidayEndDate).ToList());
            Mapper.Map(model, StaffAbsence);
            StaffAbsence.dataStaffAbsenceGuid = Guid.NewGuid();
            StaffAbsence.AbsenceFrom = model.AbsenceFrom.Value;
            StaffAbsence.AbsenceTo = model.AbsenceTo.Value.AddHours(23);
            StaffAbsence.AbsenceDays = BusinessDaysUntil(model.AbsenceFrom.Value, model.AbsenceTo.Value, holiday.Distinct().ToArray());
            var balanceUpdate = DbAHD.dataStaffAbsenceBalance.Where(x => x.UserGUID == model.UserGUID && x.AbsenceTypeGuid == model.AbsenceTypeGuid && x.Active).FirstOrDefault();
            if (balanceUpdate != null)
            {
                balanceUpdate.Balance = balanceUpdate.Balance - StaffAbsence.AbsenceDays;
            }

            DbAHD.Create(StaffAbsence, Permissions.StaffAbsence.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                string callBackFunc = "if( $('#StaffAbsencesConfirmingDataTable').length!=0 ) {DataTableRefresh('StaffAbsencesConfirmingDataTable');}InitializeStaffAbsenceCalendar(true);";
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                new Email().InformSupervisor(model.UserGUID.Value, Guid.Parse(model.DirectSupervisor));
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.StaffAbsencesDataTable, DbAHD.PrimaryKeyControl(StaffAbsence), DbAHD.RowVersionControls(Portal.SingleToList(StaffAbsence)), callBackFunc));

            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffAbsenceUpdate(StaffAbsenceUpdateModel model)
        {
            //if (!CMS.HasAction(Permissions.StaffAbsence.Update, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            if (!ModelState.IsValid || ActiveStaffAbsence(model)) return PartialView("~/Areas/AHD/Views/StaffAbsence/_StaffAbsenceForm.cshtml", model);
            DateTime ExecutionTime = DateTime.Now;
            dataStaffAbsence StaffAbsence = Mapper.Map(model, new dataStaffAbsence());
            List<DateTime> holiday = new List<DateTime>();
            holiday.AddRange(DbAHD.codeOrganizationHoliday.Select(x => x.HolidayStartDate).ToList());
            holiday.AddRange(DbAHD.codeOrganizationHoliday.Select(x => x.HolidayEndDate).ToList());
            StaffAbsence.AbsenceDays = BusinessDaysUntil(model.AbsenceFrom.Value, model.AbsenceTo.Value, holiday.Distinct().ToArray());

            if (StaffAbsence.SupervisorComfirmed)
            {
                StaffAbsence.SupervisorComfirmedDate = DateTime.Now;
                new Email().SupervisorConfirmation(model.UserGUID.Value, Guid.Parse(model.DirectSupervisor), model.AbsenceType);

            }
            var balanceUpdate = DbAHD.dataStaffAbsenceBalance.Where(x => x.UserGUID == model.UserGUID && x.AbsenceTypeGuid == model.AbsenceTypeGuid && x.Active).FirstOrDefault();
            if (balanceUpdate != null)
            {
                balanceUpdate.Balance = balanceUpdate.Balance - StaffAbsence.AbsenceDays + model.AbsenceDays;
            }
            DbAHD.Update(StaffAbsence, Permissions.StaffAbsence.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                string callBackFunc = "if( $('#StaffAbsencesConfirmingDataTable').length!=0 ) {DataTableRefresh('StaffAbsencesConfirmingDataTable');}InitializeStaffAbsenceCalendar(true);";

                //return Json(DbCMS.SingleUpdateMessage(DataTableNames.StaffAbsencesDataTable, null, null, callBackFunc));
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.StaffAbsencesDataTable, DbAHD.PrimaryKeyControl(StaffAbsence), DbAHD.RowVersionControls(Portal.SingleToList(StaffAbsence)), callBackFunc));

            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffAbsence(model.dataStaffAbsenceGuid);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffAbsenceDelete(dataStaffAbsence model)
        {
            //if (!CMS.HasAction(Permissions.StaffAbsence.Delete, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var Audit = DbCMS.spAuditHistory(LAN, model.dataStaffAbsenceGuid).OrderBy(x => x.ExecutionTime).FirstOrDefault();
            string staffName = DbCMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == UserGUID && x.Active && x.LanguageID == LAN).Select(x => x.FirstName + " " + x.Surname).FirstOrDefault();
            //Ammar Al-Joudeh,Jawad Alfazzaa,Amjad Shams Eddin



            if (!model.SupervisorComfirmed)
            {
                List<dataStaffAbsence> DeletedLanguages = DeleteStaffAbsences(new List<dataStaffAbsence> { model });

                try
                {
                    DbAHD.SaveChanges();
                    DbCMS.SaveChanges();
                    string callBackFunc = "if( $('#StaffAbsencesConfirmingDataTable').length!=0 ) {DataTableRefresh('StaffAbsencesConfirmingDataTable');}InitializeStaffAbsenceCalendar(true);";

                    return Json(DbAHD.SingleDeleteMessage(DeletedLanguages, DataTableNames.StaffAbsencesDataTable, callBackFunc));
                }
                catch (DbUpdateConcurrencyException)
                {
                    return ConcrrencyStaffAbsence(model.dataStaffAbsenceGuid);
                }
                catch (Exception ex)
                {
                    return Json(DbAHD.ErrorMessage(ex.Message));
                }
            }
            else
            {
                return Json(DbAHD.ErrorMessage("The Absence already Confirmed By the Supervisor!"));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffAbsencesDataTableDelete(List<dataStaffAbsence> models)
        {
            //if (!CMS.HasAction(Permissions.StaffAbsence.Delete, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            List<dataStaffAbsence> DeletedLanguages = DeleteStaffAbsences(models);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                string callBackFunc = "if( $('#StaffAbsencesConfirmingDataTable').length!=0 ) {DataTableRefresh('StaffAbsencesConfirmingDataTable');}InitializeStaffAbsenceCalendar(true);";
                
                return Json(DbAHD.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.StaffAbsencesDataTable),callBackFunc);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        private List<dataStaffAbsence> DeleteStaffAbsences(List<dataStaffAbsence> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataStaffAbsence> DeletedStaffAbsences = new List<dataStaffAbsence>();

            string baseQuery = "";

            string query = DbAHD.QueryBuilder(models, Permissions.StaffAbsence.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var StaffAbsences = DbAHD.Database.SqlQuery<dataStaffAbsence>(query).ToList();
            var StaffAbsenceGUIDs = StaffAbsences.Select(y => y.dataStaffAbsenceGuid).ToList();

            

            foreach (var StaffAbsence in StaffAbsences)
            {
                var balanceUpdate = DbAHD.dataStaffAbsenceBalance.Where(x => x.UserGUID == StaffAbsence.UserGUID && x.AbsenceTypeGuid == StaffAbsence.AbsenceTypeGuid && x.Active).FirstOrDefault();
              
                if (!StaffAbsence.SupervisorComfirmed)
                {
                    if (balanceUpdate != null)
                    {
                        balanceUpdate.Balance = balanceUpdate.Balance + StaffAbsence.AbsenceDays * StaffAbsence.AbsenceDuration;
                    }
                    DeletedStaffAbsences.Add(DbAHD.Delete(StaffAbsence, ExecutionTime, Permissions.StaffAbsence.DeleteGuid, DbCMS));
                }
                else
                {
                    if (CMS.HasAction(Permissions.StaffAbsence.Delete, Apps.AHD))
                    {
                        if (balanceUpdate != null)
                        {
                            balanceUpdate.Balance = balanceUpdate.Balance + StaffAbsence.AbsenceDays * StaffAbsence.AbsenceDuration;
                        }
                        DeletedStaffAbsences.Add(DbAHD.Delete(StaffAbsence, ExecutionTime, Permissions.StaffAbsence.DeleteGuid, DbCMS));
                        new Email().SupervisorCancelAbsence(StaffAbsence.UserGUID.Value, StaffAbsence.DirectSupervisor);
                    }
                   
                }
            }
            DbAHD.SaveChanges();
            return DeletedStaffAbsences;
        }

        private JsonResult ConcrrencyStaffAbsence(Guid PK)
        {
            dataStaffAbsence dbModel = new dataStaffAbsence();

            var Language = DbAHD.dataStaffAbsence.Where(l => l.dataStaffAbsenceGuid == PK).FirstOrDefault();
            var dbLanguage = DbAHD.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataStaffAbsenceRowVersion.SequenceEqual(dbModel.dataStaffAbsenceRowVersion))
            {
                return Json(DbAHD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbAHD, dbModel, "StaffAbsencesContainer"));
        }



        private bool ActiveStaffAbsence(StaffAbsenceUpdateModel model)
        {
            int error = 0;
            string strError = "";
            var StaffAbsenceType = DbAHD.dataStaffAbsenceBalance.Where(x => x.Active && x.UserGUID == model.UserGUID && x.AbsenceTypeGuid == model.AbsenceTypeGuid).FirstOrDefault();
            var staffFound = DbAHD.dataStaffAbsence.Where(x => x.Active && x.UserGUID == model.UserGUID && (x.AbsenceFrom >= model.AbsenceFrom && x.AbsenceTo <= model.AbsenceTo) && x.dataStaffAbsenceGuid != model.dataStaffAbsenceGuid).FirstOrDefault();
            string[] ExculdeListWithoutbalance = { AbsenceType.CompensatoryTimeOff.ToString(), AbsenceType.Teleworking.ToString() };
            string[] ExculdeListNeedPermissions = { AbsenceType.MaternityLeave.ToString(), AbsenceType.PaternityLeave.ToString(), AbsenceType.CertifiedSickLeave.ToString() };

            List<DateTime> holiday = new List<DateTime>();
            holiday.AddRange(DbAHD.codeOrganizationHoliday.Select(x => x.HolidayStartDate).ToList());
            holiday.AddRange(DbAHD.codeOrganizationHoliday.Select(x => x.HolidayEndDate).ToList());
            if (!model.SupervisorComfirmed)
            {
                if (ExculdeListNeedPermissions.Contains(model.AbsenceTypeGuid.ToString()))
                {
                    if (!CMS.HasAction(Permissions.StaffAbsence.FullAccess, Apps.AHD))
                    {
                        error++;
                        strError = "Unauthorized access (No access is granted to apply for such absence leave, Please contact the HR focal point!).";
                        ModelState.AddModelError("AbsenceTypeGuid", strError);
                    }
                }
                if (model.AbsenceFrom > model.AbsenceTo)
                {
                    error++;
                    strError = "(Absence To) Date Should Be Greater then  (Absence from) Date.";
                    ModelState.AddModelError("AbsenceTo", strError);
                }
                if (model.SupervisorComfirmed && model.DirectSupervisor != UserGUID.ToString())
                {
                    error++;
                    strError = "The absence has already been approved by the direct supervisor.";
                    ModelState.AddModelError("UserGUID", strError);
                }
                if (staffFound != null)
                {
                    error++;
                    strError = " already has a (Absence Event) cover peroid from: " + model.AbsenceFrom.Value.ToString("dd MMMM yyyy") + " -To: " + model.AbsenceTo.Value.ToString("dd MMMM yyyy") + ".";
                    ModelState.AddModelError("AbsenceTo", strError);
                }
                if (!ExculdeListWithoutbalance.Contains(model.AbsenceTypeGuid.ToString()))
                {
                    if (StaffAbsenceType == null)
                    {
                        error++;
                        strError = "No available balance contact the HR unit.";
                        ModelState.AddModelError("AbsenceTypeGuid", strError);
                    }
                    else
                    {
                        if (StaffAbsenceType.Balance < BusinessDaysUntil(model.AbsenceFrom.Value, model.AbsenceTo.Value, holiday.Distinct().ToArray()))
                        {
                            error++;
                            strError = "Your Balance is " + StaffAbsenceType.Balance + " days, you are unable to submit the leave.";
                            ModelState.AddModelError("AbsenceTypeGuid", strError);
                        }
                    }
                }
                if (BusinessDaysUntil(model.AbsenceFrom.Value, model.AbsenceTo.Value, holiday.Distinct().ToArray()) > 1 && model.AbsenceDuration == 0.5)
                {
                    error++;
                    strError = "(Half Day) option for no more the one day!";
                    ModelState.AddModelError("AbsenceDuration", strError);
                }
                if (AbsenceType.UncertifiedSickLeaveAndFamilyLeave == model.AbsenceTypeGuid && BusinessDaysUntil(model.AbsenceFrom.Value, model.AbsenceTo.Value, holiday.Distinct().ToArray()) > 3)
                {
                    error++;
                    strError = "No more Than 3 day for absense leave (Uncertified Sick Leave)";
                    ModelState.AddModelError("AbsenceTypeGuid", strError);
                }
                else
                {
                    if (!ExculdeListWithoutbalance.Contains(model.AbsenceTypeGuid.ToString()))
                    {
                        if (StaffAbsenceType.Balance.Value < BusinessDaysUntil(model.AbsenceFrom.Value, model.AbsenceTo.Value, null) * model.AbsenceDuration)
                        {
                            error++;
                            ModelState.AddModelError("AbsenceTo", "Your available balance is " + StaffAbsenceType.Balance + " days");
                        }
                    }
                }
            }
            Session["strError"] = strError;
            return (error > 0);
        }

        ///  - weekends (Saturdays and Sundays)
        ///  - bank holidays in the middle of the week
        /// </summary>
        /// <param name="firstDay">First day in the time interval</param>
        /// <param name="lastDay">Last day in the time interval</param>
        /// <param name="bankHolidays">List of bank holidays excluding weekends</param>
        /// <returns>Number of business days during the 'span'</returns>
        public int BusinessDaysUntil(DateTime firstDay, DateTime lastDay, params DateTime[] bankHolidays)
        {
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            //if (firstDay > lastDay)
            //throw new ArgumentException("Incorrect last day " + lastDay);

            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days + 1;
            int fullWeekCount = businessDays / 7;
            // find out if there are weekends during the time exceedng the full weeks
            if (businessDays > fullWeekCount * 7)
            {
                // we are here to find out if there is a 1-day or 2-days weekend
                // in the time interval remaining after subtracting the complete weeks
                int firstDayOfWeek = (int)firstDay.DayOfWeek;
                int lastDayOfWeek = (int)lastDay.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;
                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 6)// Both Saturday and Firdy are in the remaining time interval
                        businessDays -= 2;
                    else if (lastDayOfWeek >= 5)// Only Firdy is in the remaining time interval
                        businessDays -= 1;
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)// Only Sunday is in the remaining time interval
                    businessDays -= 1;
            }

            // subtract the weekends during the full weeks in the interval
            businessDays -= fullWeekCount + fullWeekCount;
            if (bankHolidays != null)
            {
                // subtract the number of bank holidays during the time interval
                foreach (DateTime bankHoliday in bankHolidays)
                {
                    DateTime bh = bankHoliday.Date;
                    if (firstDay <= bh && bh <= lastDay)
                        --businessDays;
                }
            }

            return businessDays;
        }


        public ActionResult StaffAbsenceCalendar()
        {
            return View("~/Areas/AHD/Views/StaffAbsence/StaffAbsencesCalendar.cshtml");
        }

        [HttpPost]
        public ActionResult GetStaffAbsenceCalendarData(DateTime start, DateTime end, Guid? ParaUserGUID)
        {
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.StaffAbsence.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();
            var Result = (from a in DbAHD.dataStaffAbsence.Where(x => x.Active && x.AbsenceFrom >= start && x.AbsenceTo <= end)
                          join b in DbAHD.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.UserGUID equals b.UserGUID
                          join c in DbAHD.StaffCoreData on a.UserGUID equals c.UserGUID
                          join d in DbAHD.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.AbsenceTypeGuid equals d.ValueGUID
                          let start_ = a.AbsenceFrom
                          let end_ = (a.AbsenceTo != null ? a.AbsenceTo : a.AbsenceFrom)
                          orderby b.FirstName + " " + b.Surname
                          select new CalendarEventStaffAbsences
                          {
                              id = a.dataStaffAbsenceGuid,
                              start = start_,
                              end = end_,
                              Month = start_.Month,
                              Day = start_.Day,
                              IsAttend = a.SupervisorComfirmed,
                              title = b.FirstName + " " + b.Surname + " - " + d.ValueDescription,
                              DutyStationGUID = c.DutyStationGUID,
                              DepartmentGUID = c.DepartmentGUID == null ? Guid.Empty : c.DepartmentGUID,
                              color = (a.SupervisorComfirmed ? "#6aa16a" : "#337ab7"),
                              IsConfirmed = a.SupervisorComfirmed,
                              UserGUID = c.UserGUID

                          }).ToList();
            if (ParaUserGUID != null)
            {
                Result = Result.Where(x => x.UserGUID == ParaUserGUID).ToList();
            }
            var DutyStationGUID = (from a in DbAHD.StaffCoreData where a.UserGUID == UserGUID select a.DutyStationGUID).FirstOrDefault();

            var jsonResult = Json(new { Result, DutyStationGUID }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult GetWorkingDay()
        {
            Guid OrganizationInstanceGUID = Guid.Parse("e156c022-ec72-4a5a-be09-163bd85c68ef");
            var userProfiles = DbAHD.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            Guid DutyStationConfigurationGUID = DbCMS.codeDutyStationsConfigurations.Where(x => x.DutyStationGUID == userProfiles.DutyStationGUID && x.OrganizationInstanceGUID == OrganizationInstanceGUID).FirstOrDefault().DutyStationConfigurationGUID;

            var WorkingDay = (from a in DbCMS.codeWorkingDaysConfigurations.Where(x => x.Active && x.DutyStationConfigurationGUID == DutyStationConfigurationGUID)
                              join b in DbCMS.codeTablesValues on a.DayGUID equals b.ValueGUID
                              select new WorkDay { Day = b.SortID.Value }).ToList();
            return Json(new JsonReturn { WorkDays = WorkingDay });
        }

        [HttpPost]
        public ActionResult UpdateStaffAbsenceDate(Guid PK, DateTime NewDate)
        {
            DateTime ExecutionTime = DateTime.Now;
            var StaffAbsence = DbAHD.dataStaffAbsence.Where(x => x.dataStaffAbsenceGuid == PK).FirstOrDefault();
            StaffAbsenceUpdateModel model = new StaffAbsenceUpdateModel();
            Mapper.Map(StaffAbsence, model);
            if (!ModelState.IsValid || ActiveStaffAbsence(model))
            {
                return Json(DbAHD.ErrorMessage(Session["strError"].ToString()));

            }


            DateTime toDate = NewDate.AddDays((StaffAbsence.AbsenceTo - StaffAbsence.AbsenceFrom).TotalDays);
            StaffAbsence.AbsenceFrom = NewDate;
            StaffAbsence.AbsenceTo = toDate;

            DbAHD.Update(StaffAbsence, Permissions.StaffAbsence.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.StaffAbsencesDataTable, DbAHD.PrimaryKeyControl(StaffAbsence), DbAHD.RowVersionControls(Portal.SingleToList(StaffAbsence))));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost]
        public ActionResult UpadetStaffAbsenceEvents(Guid PK)
        {
            return Json(new
            {
                StaffAbsence = DbAHD.dataStaffAbsence.Where(x => x.dataStaffAbsenceGuid == PK).Select(x => new
                {
                    x.dataStaffAbsenceGuid,
                    x.SupervisorComfirmed,
                    x.AbsenceFrom
                })
            });
        }

        [HttpPost]
        public ActionResult RP_StaffAbsences(DateTime date, string FulteredUsers)
        {

            Guid? FulteredUsersGuid;
            string URL="";
            if (FulteredUsers != null)
            {
                FulteredUsersGuid =DbCMS.userPersonalDetailsLanguage.Where(x => (x.FirstName + " " + x.Surname) == FulteredUsers).FirstOrDefault().UserGUID;
                URL = "/AHD/StaffAbsences/RP_StaffAbsenceLoad?date=" + date + "&FulteredUsersGuid=" + FulteredUsersGuid;
            }
            else
            {
                URL = "/AHD/StaffAbsences/RP_StaffAbsenceLoad?date=" + date ;
            }
            return Json(new { URL = URL });

        }
        public void RP_StaffAbsenceLoad(DateTime date,Guid? FulteredUsersGuid)
        {
            FulteredUsersGuid = (FulteredUsersGuid == null ? UserGUID : FulteredUsersGuid.Value);
            string FileDestenation = ConfigurationManager.AppSettings["DataFolder"] + "\\Uploads\\AHD\\" + FulteredUsersGuid + ".xlsx";
            string FileSource =Server.MapPath( "/Areas/AHD/Views/StaffAbsence/Leave_Balane_sheet.xlsx");
            System.IO.File.Delete(FileDestenation);
            System.IO.File.Copy(FileSource, FileDestenation);
            string[] ColumnCharacters = {"B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z", "AA", "AB", "AC", "AD", "AE", "AF" };
            int[] MonthRows = { 7, 9, 11, 13, 15, 17, 19, 21, 23, 25, 27, 29 };

            List<DateTime> holiday = new List<DateTime>();
            holiday.AddRange(DbAHD.codeOrganizationHoliday.Select(x => x.HolidayStartDate).ToList());
            holiday.AddRange(DbAHD.codeOrganizationHoliday.Select(x => x.HolidayEndDate).ToList());


            List<AbsenceTypeReportShortcut> shortcuts= getAbsenceTypeReportShortcut();
            using (ExcelPackage package = new ExcelPackage(new FileInfo(FileDestenation)))
            {
                double HolidayNumberAnnualBalance = 0;
                double HolidayNumberSickBalance = 0;
                ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                var leaves = DbAHD.dataStaffAbsence.Where(x => x.UserGUID == FulteredUsersGuid && x.Active && x.SupervisorComfirmed).ToList();
                for (int m = 0; m <= 11; m++)
                {
                    HolidayNumberAnnualBalance = 0;
                    HolidayNumberSickBalance = 0;
                    for (int d = 0; d <= 30; d++)
                    {
                        try
                        {
                            DateTime Today = new DateTime(date.Year, (m + 1), (d + 1));
                            workSheet.Cells[ColumnCharacters[d] + MonthRows[m]].Value = Today.ToString("dddd").Substring(0,1);
                            dataStaffAbsence absence = leaves.Where(x => x.AbsenceFrom <= Today && x.AbsenceTo >= Today ).FirstOrDefault();
                            
                            if (absence != null && BusinessDaysUntil(Today, Today, holiday.Distinct().ToArray()) >= 1)
                            {
                                var shortcut = shortcuts.Where(x => x.AbsenceTypeGUID == absence.AbsenceTypeGuid && x.AbsenceDuration==absence.AbsenceDuration).FirstOrDefault();
                                workSheet.Cells[ColumnCharacters[d] + (MonthRows[(m)] + 1)].Value = shortcut.AbsenceTypeShortcut;
                                workSheet.Cells[ColumnCharacters[d] + (MonthRows[(m)]+1)].Style.Fill.BackgroundColor.SetColor(shortcut.color);
                                _ = absence.AbsenceTypeGuid == AbsenceType.AnnualLeave ?( HolidayNumberAnnualBalance = HolidayNumberAnnualBalance + 1 *shortcut.AbsenceDuration) : absence.AbsenceTypeGuid == AbsenceType.CertifiedSickLeave || absence.AbsenceTypeGuid == AbsenceType.UncertifiedSickLeaveAndFamilyLeave ? (HolidayNumberSickBalance= HolidayNumberSickBalance + 1 * shortcut.AbsenceDuration):0;
                            }
                        }
                        catch { }
                    }
                    
                    workSheet.Cells[ReportValues.AnnualBalance+ MonthRows[m]].Value = HolidayNumberAnnualBalance;// (double) leaves.Where(x => x.AbsenceTypeGuid == AbsenceType.AnnualLeave && x.AbsenceFrom.Month== (m + 1) && x.AbsenceFrom.Year == date.Year).Sum(x=>x.AbsenceDays * x.AbsenceDuration);
                    workSheet.Cells[ReportValues.SickBalance + MonthRows[m]].Value = HolidayNumberSickBalance;// (double)leaves.Where(x => x.AbsenceTypeGuid == AbsenceType.UncertifiedSickLeaveAndFamilyLeave || x.AbsenceTypeGuid == AbsenceType.CertifiedSickLeave && x.AbsenceFrom.Month == (m + 1) && x.AbsenceFrom.Year == date.Year).Sum(x => x.AbsenceDays * x.AbsenceDuration);
                }

                workSheet.Cells[ReportValues.StaffName].Value = CMS.GetFullName(FulteredUsersGuid.Value, LAN) ;
                workSheet.Cells[ReportValues.BalanceYearBefore].Value = (date.Year-1).ToString()+ " Balance";
                workSheet.Cells[ReportValues.TotalLeaves].Value = "Total Leaves "+(date.Year).ToString();
                workSheet.Cells[ReportValues.UNOPSAttendanceRecordCard].Value = "UNOPS Attendance Record Card " + (date.Year).ToString();
              
                try
                {
                    var balance = DbAHD.dataStaffAbsenceBalance.Where(x => x.UserGUID == FulteredUsersGuid).ToList();
                    workSheet.Cells[ReportValues.NumberBalanceYearBeforeAnualLeave].Value =  balance.Where(x => x.AbsenceTypeGuid == AbsenceType.AnnualLeave).FirstOrDefault().InitialBalance;
                    workSheet.Cells[ReportValues.NumberBalanceYearBeforeSick].Value = balance.Where(x => x.AbsenceTypeGuid == AbsenceType.CertifiedSickLeave).FirstOrDefault().InitialBalance ;
                }
                catch { }

                package.Save();
                byte[] fileBytes = System.IO.File.ReadAllBytes(FileDestenation);
                System.IO.File.Delete(FileDestenation);
                this.Response.ClearContent();
                this.Response.ClearHeaders();
                this.Response.Clear();
                this.Response.AddHeader("content-disposition", "inline;filename=" + CMS.GetFullName(FulteredUsersGuid.Value, LAN)+" Absence Summary" + ".xlsx");  //Filename example (1.pdf)
                this.Response.ContentType = "Application/XSLX";
                this.Response.BinaryWrite(fileBytes);
                this.Response.Flush();
                this.Response.End();

            }


        }

        private List<AbsenceTypeReportShortcut> getAbsenceTypeReportShortcut()
        {
            return new  List<AbsenceTypeReportShortcut>(){
                {new AbsenceTypeReportShortcut(){AbsenceTypeGUID=Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E9901"),AbsenceTypeShortcut="AL",AbsenceDuration=1,color=Color.GreenYellow} },
                {new AbsenceTypeReportShortcut(){AbsenceTypeGUID=Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E9901"),AbsenceTypeShortcut="AH",AbsenceDuration=0.5,color=Color.LightSeaGreen} },
                {new AbsenceTypeReportShortcut(){AbsenceTypeGUID=Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E9902"),AbsenceTypeShortcut="CS",AbsenceDuration=1,color=Color.BlueViolet } },
                {new AbsenceTypeReportShortcut(){AbsenceTypeGUID=Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E9903"),AbsenceTypeShortcut="US",AbsenceDuration=1,color=Color.Yellow } },
                {new AbsenceTypeReportShortcut(){AbsenceTypeGUID=Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E9903"),AbsenceTypeShortcut="UH",AbsenceDuration=0.5,color=Color.LightYellow } },
                {new AbsenceTypeReportShortcut(){AbsenceTypeGUID=Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E9904"),AbsenceTypeShortcut="ML",AbsenceDuration=1,color=Color.Orange } },
                {new AbsenceTypeReportShortcut(){AbsenceTypeGUID=Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E9905"),AbsenceTypeShortcut="PL",AbsenceDuration=1,color=Color.OrangeRed } },
                {new AbsenceTypeReportShortcut(){AbsenceTypeGUID=Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E9906"),AbsenceTypeShortcut="CT",AbsenceDuration=1,color=Color.Purple } },
                {new AbsenceTypeReportShortcut(){AbsenceTypeGUID=Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E9907"),AbsenceTypeShortcut="TW",AbsenceDuration=1,color=Color.LightGray } },
            };
        }
       
        [HttpPost]
        public ActionResult InformSupervisor(Guid ReportToGUID)
        {
            bool recordFound = false;
            var staffInfo = DbAHD.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();

            var confirmedCount = DbAHD.dataStaffAbsence.Where(x => x.UserGUID == UserGUID && !x.SupervisorComfirmed).Count();
            if (confirmedCount > 0)
            {
               
                recordFound = true;
            }
            return Json(new { MailSuccess = recordFound });
        }

        [HttpPost]
        public ActionResult ConfirmSupervisor(Guid ConfirmedUserGUID)
        {
            bool recordFound = false;
            var confirmed = DbAHD.dataStaffAbsence.Where(x => x.UserGUID == ConfirmedUserGUID && !x.SupervisorComfirmed).ToList();
            if (confirmed.Count > 0)
            {
                confirmed.ForEach(x => x.SupervisorComfirmed = true);
                DbAHD.SaveChanges();
                recordFound = true;
            }
            return Json(new { MailSuccess = recordFound });
        }

        [HttpPost]
        public ActionResult CancelSupervisor(Guid ConfirmedUserGUID)
        {
            bool recordFound = false;
            var confirmed = DbAHD.dataStaffAbsence.Where(x => x.UserGUID == ConfirmedUserGUID && !x.SupervisorComfirmed).ToList();
            if (confirmed.Count > 0)
            {
                DbAHD.dataStaffAbsence.RemoveRange(confirmed);
                DbAHD.SaveChanges();
                recordFound = true;
            }
            return Json(new { MailSuccess = recordFound });
        }

        [HttpPost]
        public ActionResult RemoteBusinessDaysUntil(Guid PK, DateTime DateFrom, DateTime DateTo ,double? AbsenceDuration,Guid CurrentUserGUID)
        {
            string Message = "";
            
            try
            {
                List<DateTime> holiday = new List<DateTime>();
                holiday.AddRange(DbAHD.codeOrganizationHoliday.Select(x => x.HolidayStartDate).ToList());
                holiday.AddRange(DbAHD.codeOrganizationHoliday.Select(x => x.HolidayEndDate).ToList());

                var StaffAbsenceType = DbAHD.dataStaffAbsenceBalance.Where(x => x.Active && x.UserGUID == CurrentUserGUID && x.AbsenceTypeGuid == PK).FirstOrDefault();
                double AbsenceDurationVal = 1;
                AbsenceDurationVal=(AbsenceDuration == null ? 1 :  AbsenceDuration.Value);
                double BusinessDaysUntilCount = BusinessDaysUntil(DateFrom, DateTo, holiday.Distinct().ToArray()) * AbsenceDurationVal;
                if (DateFrom > DateTo)
                {
                    Message = "(Absence To) Date Should Be Greater then  (Absence from) Date.";
                }
                else if (BusinessDaysUntil(DateFrom, DateTo, holiday.Distinct().ToArray()) >= 1 && AbsenceDuration.Value == 0.5)
                {
                    Message = "(Half Day) option for no more the one day!";
                }else if(AbsenceType.UncertifiedSickLeaveAndFamilyLeave == PK && BusinessDaysUntilCount > 3)
                {
                    Message = "No more Than 3 day for absense type (Uncertified Sick Leave)";
                }
                else
                {
                    
                    if (StaffAbsenceType == null)
                    {
                        Message = "No available balance for such Absence!";
                    }
                    else
                    {
                        if (StaffAbsenceType.Balance.Value > BusinessDaysUntilCount)
                        {
                            Message = "Your absence is valid (Absence Business Days is " + BusinessDaysUntilCount + "), Current balance: (" + StaffAbsenceType.Balance.Value + " Days)";
                        }
                        else
                        {

                        }
                    }
                }
            }
            catch(Exception ex) { Message = ex.Message; }

            return Json(new { Message = Message });
        }
        #endregion
    }

    public class AbsenceType
    {
        public static Guid AnnualLeave = Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E9901");
        public static Guid CertifiedSickLeave = Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E9902");
        public static Guid UncertifiedSickLeaveAndFamilyLeave = Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E9903");
        public static Guid MaternityLeave = Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E9904");
        public static Guid PaternityLeave = Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E9905");
        public static Guid CompensatoryTimeOff = Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E9906");
        public static Guid Teleworking = Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E9907");
    }

    public class AbsenceTypeReportShortcut
    {
        public Guid AbsenceTypeGUID { get; set; }
        public string AbsenceTypeShortcut { get; set; }
        public double AbsenceDuration { get; set; }
        public Color color { get; set; }
    }

    

    public class ReportValues
    {
        public static string StaffName = "E2";
        public static string NumberBalanceYearBeforeAnualLeave = "AI2";
        public static string NumberBalanceYearBeforeSick = "AL2";
        public static string AnnualBalance = "AH";
        public static string SickBalance = "AK";
        public static string UNOPSAttendanceRecordCard= "F1";
        public static string TotalLeaves = "AG31";
        public static string BalanceYearBefore = "AG2";
    }

}