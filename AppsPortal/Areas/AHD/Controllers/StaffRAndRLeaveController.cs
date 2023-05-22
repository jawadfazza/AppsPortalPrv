using AHD_DAL.Model;
using AHD_DAL.ViewModels;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Library.MimeDetective;
using AppsPortal.ViewModels;
using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using FineUploader;
using iTextSharp.text.pdf.qrcode;
using LinqKit;

using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using WebGrease.Css.Ast;

namespace AppsPortal.Areas.AHD.Controllers
{
    public class StaffRAndRLeaveController : AHDBaseController
    {
        #region Temp

        public JsonResult InternationalTempStaffRAndRDatesDataTable(DataTableRecievedOptions options, Guid PK)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<TempRestAndRecuperationRequestLeaveDateDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<TempRestAndRecuperationRequestLeaveDateDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.NationalStaffDangerPayManagement.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
            var All = (
                from a in DbAHD.dataTempRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == PK).AsExpandable()
                    //join b in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.FlowStatusGUID equals b.ValueGUID into LJ1
                    //from R1 in LJ1.DefaultIfEmpty()

                select new TempRestAndRecuperationRequestLeaveDateDataTableModel
                {
                    TempRestAndRecuperationRequestLeaveDateGUID = a.TempRestAndRecuperationRequestLeaveDateGUID.ToString(),
                    RestAndRecuperationLeaveGUID = a.RestAndRecuperationLeaveGUID.ToString(),
                    LeaveTypeGUID = a.LeaveTypeGUID.ToString(),
                    LeaveTypeName = a.LeaveTypeName,
                    Active = a.Active,
                    TravelTimeIn = a.TravelTimeIn,
                    TravelTimeOut = a.TravelTimeOut,
                    Comments = a.Comments,
                    dataTempRestAndRecuperationRequestLeaveDateRowVersion = a.dataTempRestAndRecuperationRequestLeaveDateRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<TempRestAndRecuperationRequestLeaveDateDataTableModel> Result = Mapper.Map<List<TempRestAndRecuperationRequestLeaveDateDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }





        public ActionResult TempTravelTimeOutCreate(Guid FK)
        {

            Guid hrAccessGUID = Guid.Parse("DE411F27-0EA8-49FB-964D-00833BB012F8");
            var staffs = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == FK).FirstOrDefault();

            var checkCurrUser = DbCMS.userProfiles.Where(x => x.userServiceHistory.UserGUID == UserGUID && x.Active).Select(x => x.UserProfileGUID).FirstOrDefault();
            var hasHRPermissionsGuid = DbCMS.userPermissions.Where(x => x.ActionGUID == hrAccessGUID && x.UserProfileGUID == checkCurrUser && x.Active).
                Select(x => x.UserProfileGUID).ToList();







            if ((staffs != null && staffs.CreatedByGUID != UserGUID) && (staffs != null && hasHRPermissionsGuid.Count <= 0))
            {
                return Json(DbAHD.PermissionError());
            }
            else
            {

                return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_TempTravelTimeOutUpdateModal.cshtml",
                    new TempRestAndRecuperationRequestLeaveDateModel { RestAndRecuperationLeaveGUID = FK, LeaveTypeGUID = Guid.Parse("A1A0A314-388C-4E21-AB91-B919439AA794") });
            }
        }



        public ActionResult TempRestAndRecuperationLeaveDateCreate(Guid FK)
        {

            Guid hrAccessGUID = Guid.Parse("DE411F27-0EA8-49FB-964D-00833BB012F8");
            var staffs = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == FK).FirstOrDefault();

            var checkCurrUser = DbCMS.userProfiles.Where(x => x.userServiceHistory.UserGUID == UserGUID && x.Active).Select(x => x.UserProfileGUID).FirstOrDefault();
            var hasHRPermissionsGuid = DbCMS.userPermissions.Where(x => x.ActionGUID == hrAccessGUID && x.UserProfileGUID == checkCurrUser && x.Active).
                Select(x => x.UserProfileGUID).ToList();







            if ((staffs != null && staffs.CreatedByGUID != UserGUID) && (staffs != null && hasHRPermissionsGuid.Count <= 0))
            {
                return Json(DbAHD.PermissionError());
            }
            else
            {

                return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_TempLeaveDatesUpdateModal.cshtml",
                    new TempRestAndRecuperationRequestLeaveDateModel { RestAndRecuperationLeaveGUID = FK });
            }
        }

        public ActionResult TempRestAndRecuperationRRCreate(Guid FK)
        {

            Guid hrAccessGUID = Guid.Parse("DE411F27-0EA8-49FB-964D-00833BB012F8");
            var staffs = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == FK).FirstOrDefault();

            var checkCurrUser = DbCMS.userProfiles.Where(x => x.userServiceHistory.UserGUID == UserGUID && x.Active).Select(x => x.UserProfileGUID).FirstOrDefault();
            var hasHRPermissionsGuid = DbCMS.userPermissions.Where(x => x.ActionGUID == hrAccessGUID && x.UserProfileGUID == checkCurrUser && x.Active).
                Select(x => x.UserProfileGUID).ToList();

            if ((staffs != null && staffs.CreatedByGUID != UserGUID) && (staffs != null && hasHRPermissionsGuid.Count <= 0))
            {
                return Json(DbAHD.PermissionError());
            }
            else
            {

                return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_TempRRUpdateModal.cshtml",
                    new TempRestAndRecuperationRequestLeaveDateModel { RestAndRecuperationLeaveGUID = FK, LeaveTypeGUID = coddeInternationalStaffAttendanceTypeAttendanceTable.RR });
            }
        }

        public ActionResult TempRestAndRecuperationALCreate(Guid FK)
        {

            Guid hrAccessGUID = Guid.Parse("DE411F27-0EA8-49FB-964D-00833BB012F8");
            var staffs = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == FK).FirstOrDefault();

            var checkCurrUser = DbCMS.userProfiles.Where(x => x.userServiceHistory.UserGUID == UserGUID && x.Active).Select(x => x.UserProfileGUID).FirstOrDefault();
            var hasHRPermissionsGuid = DbCMS.userPermissions.Where(x => x.ActionGUID == hrAccessGUID && x.UserProfileGUID == checkCurrUser && x.Active).
                Select(x => x.UserProfileGUID).ToList();

            if ((staffs != null && staffs.CreatedByGUID != UserGUID) && (staffs != null && hasHRPermissionsGuid.Count <= 0))
            {
                return Json(DbAHD.PermissionError());
            }
            else
            {

                return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_TempALUpdateModal.cshtml",
                    new TempRestAndRecuperationRequestLeaveDateModel { RestAndRecuperationLeaveGUID = FK, LeaveTypeGUID = coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave });
            }
        }

        public ActionResult TempCustomUpdateLeaveRestAndRecuperationLeaveDate(Guid FK)
        {
            var staffs = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == FK).FirstOrDefault();



            TempRestAndRecuperationRequestLeaveDateModel model = new TempRestAndRecuperationRequestLeaveDateModel();
            model.RestAndRecuperationLeaveGUID = FK;

            return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_StaffUpdateCustomLeaveRequest.cshtml",
                model);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TempCustomUpdateLeaveRestAndRecuperationLeaveDateCreate(TempRestAndRecuperationRequestLeaveDateModel model)
        {

            if (!ModelState.IsValid) return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_StaffCustomLeaveRequest.cshtml", model);


            if (model.TravelStartDate == null || model.TravelReturnDate == null)
            {
                ModelState.AddModelError("Error: ", "Travel Time date is required ");
                return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_StaffCustomLeaveRequest.cshtml", model);
            }

            if (model.TravelStartDate > model.TravelReturnDate)
            {
                ModelState.AddModelError("Error: ", "Leave start date must be less than leave end date");
                return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_StaffCustomLeaveRequest.cshtml", model);
            }

            if ((model.ALStartDate != null && model.ALStartDate > model.TravelReturnDate) || model.RRStartDate > model.TravelReturnDate
            || model.RRReturnDate > model.TravelReturnDate || (model.ALReturnDate != null && model.ALReturnDate > model.TravelReturnDate))
            {
                ModelState.AddModelError("Error: ", "Leave start date must be less than leave end date");
                return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_StaffCustomLeaveRequest.cshtml", model);
            }
            else if (model.RRStartDate != null && model.RRStartDate > model.RRReturnDate)
            {
                ModelState.AddModelError("Error: ", "RR Leave start date must be less than annual leave date ");
                return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_StaffCustomLeaveRequest.cshtml", model);
            }
            else if (model.ALStartDate != null && model.ALStartDate > model.ALReturnDate)
            {
                ModelState.AddModelError("Error: ", "Annual Leave start date must be less than annual leave end date ");
                return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_StaffCustomLeaveRequest.cshtml", model);
            }

            //ModelState.AddModelError("Error: ", "Kindly revise the entry data to avoid conflict in dates");


            //var toCheckallTemp = DbAHD.dataTempRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID).ToList();
            //foreach (var model2 in toCheckallTemp)
            //{
            //    if ((model.TravelTimeIn <= model2.TravelTimeOut)
            //          || (model2.TravelTimeIn <= model.TravelTimeOut && (model2.TravelTimeOut >= model.TravelTimeIn)))
            //    {

            //        ModelState.AddModelError("Error: ", "Start date is overlapping with previous RR Records Revise start date");
            //        return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_TempLeaveDatesUpdateModal.cshtml", model);
            //    }


            //}
            DateTime ExecutionTime = DateTime.Now;
            //delete 

            var delete = DbAHD.dataTempRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID).ToList();
            DbAHD.dataTempRestAndRecuperationRequestLeaveDate.RemoveRange(delete);

            var deleteMain = DbAHD.dataRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID).ToList();
            DbAHD.dataRestAndRecuperationRequestLeaveDate.RemoveRange(deleteMain);
            var attendacnces = DbAHD.dataInternationalStaffAttendance.Where(x => x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID).ToList();
            DbAHD.dataInternationalStaffAttendance.RemoveRange(attendacnces);

            DbAHD.SaveChanges();
            DbCMS.SaveChanges();


            var tocheckAll = DbAHD.dataRestAndRecuperationRequestLeaveDate.Where(x => x.dataRestAndRecuperationRequest.StaffGUID == UserGUID).ToList();


            foreach (var model2 in tocheckAll)
            {
                if ((model.TravelStartDate <= model2.TravelTimeOut)
                    || (model.RRStartDate <= model2.TravelTimeOut)
                    || (model.ALStartDate <= model2.TravelTimeOut)
                      || (

                      (
                      model2.TravelTimeIn <= model.TravelReturnDate
                      ||
                     (model.ALReturnDate != null && model2.TravelTimeIn <= model.ALReturnDate

                     )
                      ||
                      model2.TravelTimeIn <= model.RRReturnDate

                      )

                      && (

                      model2.TravelTimeOut >= model.TravelStartDate
                      ||
                      model2.TravelTimeOut >= model.RRStartDate
                      ||
                      (
                      model.ALStartDate != null &&
                      model2.TravelTimeOut >= model.ALStartDate

                      )

                      )))
                {
                    ModelState.AddModelError("Error: ", "Kindly revise the entry data to avoid conflict in dates");
                    return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_TempLeaveDatesUpdateModal.cshtml", model);
                }


            }


            List<dataTempRestAndRecuperationRequestLeaveDate> allleaves = new List<dataTempRestAndRecuperationRequestLeaveDate>();
            dataTempRestAndRecuperationRequestLeaveDate temp = new dataTempRestAndRecuperationRequestLeaveDate();
            List<StaffLeaveHolidayDates> _realleaves = new List<StaffLeaveHolidayDates>();

            temp = new dataTempRestAndRecuperationRequestLeaveDate();
            temp.TempRestAndRecuperationRequestLeaveDateGUID = Guid.NewGuid();
            temp.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;
            temp.LeaveTypeName = "Travel Time";
            temp.LeaveTypeGUID = coddeInternationalStaffAttendanceTypeAttendanceTable.TravelTime;
            temp.TravelTimeOut = model.TravelStartDate;
            temp.TravelTimeIn = model.TravelStartDate;
            temp.Comments = model.Comments;
            allleaves.Add(temp);

            //temp = new dataTempRestAndRecuperationRequestLeaveDate();
            //temp.TempRestAndRecuperationRequestLeaveDateGUID = Guid.NewGuid();
            //temp.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;
            //temp.LeaveTypeName = "Travel Time";
            //temp.LeaveTypeGUID = coddeInternationalStaffAttendanceTypeAttendanceTable.TravelTime;
            //temp.TravelTimeOut = model.TravelReturnDate;
            //temp.TravelTimeIn = model.TravelReturnDate;
            //temp.Comments = model.Comments;
            //allleaves.Add(temp);




            //var leaveName = DbAHD.codeTablesValuesLanguages.Where(x => x.ValueGUID == model.LeaveTypeGUID && x.LanguageID == LAN).Select(x => x.ValueDescription).FirstOrDefault();



            _realleaves = model.ALStartDate == null && model.RRStartDate != null ?

                CheckCustomLeaveDays((DateTime)model.RRStartDate, (DateTime)model.RRReturnDate, coddeInternationalStaffAttendanceTypeAttendanceTable.RR, "R And R", (Guid)model.RestAndRecuperationLeaveGUID, (DateTime)model.TravelReturnDate)
                :

                 model.ALStartDate != null && model.RRStartDate == null ?
                 CheckCustomLeaveDays((DateTime)model.ALStartDate, (DateTime)model.ALReturnDate, coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave, "Annual Leave", (Guid)model.RestAndRecuperationLeaveGUID, (DateTime)model.TravelReturnDate) :
                  model.ALStartDate != null && model.RRStartDate != null &&
                  model.RRStartDate > model.ALStartDate ?
                  CheckCustomLeaveDays((DateTime)model.ALStartDate, (DateTime)model.ALReturnDate, coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave, "Annual Leave", (Guid)model.RestAndRecuperationLeaveGUID, (DateTime)model.TravelReturnDate) :

                  model.ALStartDate > model.RRStartDate ?
                  CheckCustomLeaveDays((DateTime)model.RRStartDate, (DateTime)model.RRReturnDate, coddeInternationalStaffAttendanceTypeAttendanceTable.RR, "R And R", (Guid)model.RestAndRecuperationLeaveGUID, (DateTime)model.TravelReturnDate) :
                  new List<StaffLeaveHolidayDates>()

                ;

            Guid leaveTypeguid = Guid.Empty;

            foreach (var item in _realleaves.Where(x => x.LeaveTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend || x.LeaveTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday))
            {
                temp = new dataTempRestAndRecuperationRequestLeaveDate();
                temp.TempRestAndRecuperationRequestLeaveDateGUID = Guid.NewGuid();
                temp.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;
                temp.LeaveTypeName = item.LeaveName;
                temp.LeaveTypeGUID = item.LeaveTypeGUID;
                temp.TravelTimeOut = item.startdateName;
                temp.TravelTimeIn = item.startdateName;
                temp.Comments = model.Comments;
                allleaves.Add(temp);
            }
            if ((model.ALStartDate == null && model.RRStartDate != null) || (model.ALStartDate != null && model.ALStartDate > model.RRStartDate))
            {
                leaveTypeguid = coddeInternationalStaffAttendanceTypeAttendanceTable.RR;
            }
            else if ((model.ALStartDate != null && model.RRStartDate > model.ALStartDate))
            {
                leaveTypeguid = coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave;
            }

            foreach (var item in _realleaves.Where(x => x.LeaveTypeGUID == leaveTypeguid))
            {
                temp = new dataTempRestAndRecuperationRequestLeaveDate();
                temp.TempRestAndRecuperationRequestLeaveDateGUID = Guid.NewGuid();
                temp.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;
                temp.LeaveTypeName = item.LeaveName;
                temp.LeaveTypeGUID = item.LeaveTypeGUID;
                temp.TravelTimeOut = item.enddateName;
                temp.TravelTimeIn = item.startdateName;
                temp.Comments = model.Comments;
                allleaves.Add(temp);
            }


            if (model.RRStartDate != null && model.ALStartDate != null)
            {
                List<StaffLeaveHolidayDates> _realleaves_last = new List<StaffLeaveHolidayDates>();
                var lastRelative = _realleaves.OrderByDescending(x => x.enddateName).FirstOrDefault();
                DateTime _start = lastRelative.enddateName.Value.AddDays(1);
                var du = model.RRStartDate > model.ALStartDate ? (model.RRReturnDate - model.RRStartDate).Value.Days + 1 :
                    (model.ALReturnDate - model.ALStartDate).Value.Days + 1
                    ;
                DateTime _end = _start.AddDays(du);

                _realleaves_last = model.RRStartDate > model.ALStartDate ?

                   CheckCustomLeaveDays((DateTime)_start, _end, coddeInternationalStaffAttendanceTypeAttendanceTable.RR, "R And R", (Guid)model.RestAndRecuperationLeaveGUID, (DateTime)model.TravelReturnDate) :
                   model.ALStartDate > model.RRStartDate ?

                   CheckCustomLeaveDays(_start, _end, coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave, "Annual Leave", (Guid)model.RestAndRecuperationLeaveGUID, (DateTime)model.TravelReturnDate) :

                 new List<StaffLeaveHolidayDates>();
                foreach (var item in _realleaves_last.Where(x => x.LeaveTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend || x.LeaveTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday))
                {
                    temp = new dataTempRestAndRecuperationRequestLeaveDate();
                    temp.TempRestAndRecuperationRequestLeaveDateGUID = Guid.NewGuid();
                    temp.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;
                    temp.LeaveTypeName = item.LeaveName;
                    temp.LeaveTypeGUID = item.LeaveTypeGUID;
                    temp.TravelTimeOut = item.startdateName;
                    temp.TravelTimeIn = item.startdateName;
                    temp.Comments = model.Comments;
                    allleaves.Add(temp);
                }
                if ((model.ALStartDate == null && model.RRStartDate != null) || (model.ALStartDate != null && model.RRStartDate > model.ALStartDate))
                {
                    leaveTypeguid = coddeInternationalStaffAttendanceTypeAttendanceTable.RR;
                }
                else if ((model.ALStartDate != null && model.ALStartDate > model.RRStartDate))
                {
                    leaveTypeguid = coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave;
                }

                foreach (var item in _realleaves_last.Where(x => x.LeaveTypeGUID == leaveTypeguid))
                {
                    temp = new dataTempRestAndRecuperationRequestLeaveDate();
                    temp.TempRestAndRecuperationRequestLeaveDateGUID = Guid.NewGuid();
                    temp.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;
                    temp.LeaveTypeName = item.LeaveName;
                    temp.LeaveTypeGUID = item.LeaveTypeGUID;
                    temp.TravelTimeOut = item.enddateName;
                    temp.TravelTimeIn = item.startdateName;
                    temp.Comments = model.Comments;
                    allleaves.Add(temp);
                }


            }

            var lastdate = allleaves.OrderByDescending(x => x.TravelTimeOut).FirstOrDefault();
            temp = new dataTempRestAndRecuperationRequestLeaveDate();
            temp.TempRestAndRecuperationRequestLeaveDateGUID = Guid.NewGuid();
            temp.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;
            temp.LeaveTypeName = "Travel Time";
            temp.LeaveTypeGUID = coddeInternationalStaffAttendanceTypeAttendanceTable.TravelTime;
            temp.TravelTimeOut = lastdate.TravelTimeOut.Value.AddDays(1);
            temp.TravelTimeIn = lastdate.TravelTimeOut.Value.AddDays(1);
            temp.Comments = model.Comments;
            allleaves.Add(temp);


            DbAHD.CreateBulk(allleaves, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);





            try
            {

                DbAHD.SaveChanges();
                DbCMS.SaveChanges();

                var temps = DbAHD.dataTempRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID).ToList();
                if (temps.Count > 0)
                {
                    var olddates = DbAHD.dataRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID).ToList();
                    DbAHD.dataRestAndRecuperationRequestLeaveDate.RemoveRange(olddates);
                    //List<dataMissionActionRequired> missionRequiredActions = new List<dataMissionActionRequired>();
                    //List<dataMissionActionTaken> missionTakendActions = new List<dataMissionActionTaken>();
                    foreach (var item in temps.OrderBy(x => x.TravelTimeIn))
                    {
                        dataRestAndRecuperationRequestLeaveDate myTemp = new dataRestAndRecuperationRequestLeaveDate
                        {
                            RestAndRecuperationRequestLeaveDateGUID = temp.TempRestAndRecuperationRequestLeaveDateGUID,
                            RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID,
                            LeaveTypeGUID = item.LeaveTypeGUID,
                            LeaveTypeName = item.LeaveTypeName,
                            TravelTimeOut = item.TravelTimeOut,
                            TravelTimeIn = item.TravelTimeIn,
                            Comments = item.Comments
                        };
                        //missionRequiredActions.Add(myActionRequired);
                        DbAHD.CreateNoAudit(myTemp);


                    }
                }

                DbAHD.SaveChanges();
                DbCMS.SaveChanges();

                var leaveRequest = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID).FirstOrDefault();

                leaveRequest.LeaveInDate = temps.Select(x => x.TravelTimeIn).Min();
                leaveRequest.LeaveOutDate = temps.Select(x => x.TravelTimeOut).Max();

                var toAddAttendace = DbAHD.dataRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID).ToList();
                Guid r_rGUID = Guid.Parse("67979D6D-5B7B-4A85-AA6D-CD604A1BDF75");
                Guid annual_leave = Guid.Parse("13AFCA94-4FA0-479A-85DB-8AEF4BC64CBB");
                Guid _travelTime = Guid.Parse("a1a0a314-388c-4e21-ab91-b919439aa794");
                Guid _weekend = coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend;
                Guid _officialholiday = coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday;
                var myUser = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == leaveRequest.StaffGUID
                                                                 && x.LanguageID == LAN && x.Active).FirstOrDefault();

                //attendaces 
                List<dataInternationalStaffAttendance> allAttendacnes = new List<dataInternationalStaffAttendance>();
                foreach (var item in toAddAttendace)
                {
                    if (item.LeaveTypeGUID != Guid.Empty)
                    {
                        dataInternationalStaffAttendance attendance = new dataInternationalStaffAttendance
                        {
                            InternationalStaffAttendanceGUID = Guid.NewGuid(),
                            InternationalStaffAttendanceTypeGUID = item.LeaveTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR ? r_rGUID : (item.LeaveTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave ? annual_leave : item.LeaveTypeGUID == _travelTime ? _travelTime : item.LeaveTypeGUID == _weekend ? _weekend : item.LeaveTypeGUID == _officialholiday ? _officialholiday : Guid.Empty),
                            StaffGUID = leaveRequest.StaffGUID,
                            StaffName = myUser.FirstName + " " + myUser.Surname,
                            LeaveLocation = item.dataRestAndRecuperationRequest.StaffLoction,
                            TotalDays = item.TravelTimeOut != null ? ((int)(item.TravelTimeOut - item.TravelTimeIn).Value.TotalDays + 1) : 1,
                            Comments = item.Comments,
                            IsAutomated = false,
                            FromDate = item.TravelTimeIn,
                            ToDate = item.TravelTimeOut,
                            CreatedByGUID = UserGUID,
                            CreateDate = ExecutionTime,
                            RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID,
                            RestAndRecuperationRequestLeaveDateGUID = item.RestAndRecuperationRequestLeaveDateGUID

                        };
                        allAttendacnes.Add(attendance);

                    }

                }

                DbAHD.CreateBulk(allAttendacnes, Permissions.InternationalStaffRestAndRecuperationLeaveHRReview.CreateGuid, ExecutionTime, DbCMS);
                DbAHD.Update(leaveRequest, Permissions.InternationalStaffRestAndRecuperationLeaveHRReview.CreateGuid, ExecutionTime, DbCMS);
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, null, "InitializeCalendarLeavs();"));

                // return Json(DbAHD.SingleUpdateMessage(DataTableNames.InternationalTempStaffRAndRDatesDataTable, DbAHD.PrimaryKeyControl(allleaves.FirstOrDefault()), DbAHD.RowVersionControls(Portal.SingleToList(allleaves.FirstOrDefault()))));

            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }


        public ActionResult TempCustomLeaveRestAndRecuperationLeaveDate(Guid FK)
        {
            var request = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == FK).FirstOrDefault();
            var currStaff = DbAHD.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            if (request != null && request.CreatedByGUID != UserGUID && request.FlowStatusGUID == InternationalStaffRAndRLeaveFlowStatus.Closed)
            {
                return Json(DbAHD.PermissionError());
            }
            else
            {
                TempRestAndRecuperationRequestLeaveDateModel model = new TempRestAndRecuperationRequestLeaveDateModel();
                model.RestAndRecuperationLeaveGUID = FK;
                if (currStaff != null && currStaff.ReturnDateFromLastRAndRLeave != null)
                {
                    model.RRStartDate = currStaff.ReturnDateFromLastRAndRLeave.Value.AddDays(28);
                    model.RRReturnDate = currStaff.ReturnDateFromLastRAndRLeave.Value.AddDays(33);

                }
                return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_StaffCustomLeaveRequest.cshtml",
                    model);
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TempCustomLeaveRestAndRecuperationLeaveDateCreate(TempRestAndRecuperationRequestLeaveDateModel model)
        {

            if (!ModelState.IsValid) return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_StaffCustomLeaveRequest.cshtml", model);


            if (model.TravelStartDate == null || model.TravelReturnDate == null)
            {
                ModelState.AddModelError("Error: ", "Travel Time date is required ");
                return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_StaffCustomLeaveRequest.cshtml", model);
            }

            if (model.TravelStartDate > model.TravelReturnDate)
            {
                ModelState.AddModelError("Error: ", "Leave start date must be less than leave end date");
                return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_StaffCustomLeaveRequest.cshtml", model);
            }

            if ((model.ALStartDate != null && model.ALStartDate > model.TravelReturnDate) || model.RRStartDate > model.TravelReturnDate
            || model.RRReturnDate > model.TravelReturnDate || (model.ALReturnDate != null && model.ALReturnDate > model.TravelReturnDate))
            {
                ModelState.AddModelError("Error: ", "Leave start date must be less than leave end date");
                return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_StaffCustomLeaveRequest.cshtml", model);
            }
            else if (model.RRStartDate != null && model.RRStartDate > model.RRReturnDate)
            {
                ModelState.AddModelError("Error: ", "RR Leave start date must be less than annual leave date ");
                return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_StaffCustomLeaveRequest.cshtml", model);
            }
            else if (model.ALStartDate != null && model.ALStartDate > model.ALReturnDate)
            {
                ModelState.AddModelError("Error: ", "Annual Leave start date must be less than annual leave end date ");
                return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_StaffCustomLeaveRequest.cshtml", model);
            }

            //ModelState.AddModelError("Error: ", "Kindly revise the entry data to avoid conflict in dates");


            //var toCheckallTemp = DbAHD.dataTempRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID).ToList();
            //foreach (var model2 in toCheckallTemp)
            //{
            //    if ((model.TravelTimeIn <= model2.TravelTimeOut)
            //          || (model2.TravelTimeIn <= model.TravelTimeOut && (model2.TravelTimeOut >= model.TravelTimeIn)))
            //    {

            //        ModelState.AddModelError("Error: ", "Start date is overlapping with previous RR Records Revise start date");
            //        return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_TempLeaveDatesUpdateModal.cshtml", model);
            //    }


            //}

            var tocheckAll = DbAHD.dataRestAndRecuperationRequestLeaveDate.Where(x => x.dataRestAndRecuperationRequest.StaffGUID == UserGUID).ToList();


            foreach (var model2 in tocheckAll)
            {
                if ((model.TravelStartDate <= model2.TravelTimeOut)
                    || (model.RRStartDate <= model2.TravelTimeOut)
                    || (model.ALStartDate <= model2.TravelTimeOut)
                      || (

                      (
                      model2.TravelTimeIn <= model.TravelReturnDate
                      ||
                     (model.ALReturnDate != null && model2.TravelTimeIn <= model.ALReturnDate

                     )
                      ||
                      model2.TravelTimeIn <= model.RRReturnDate

                      )

                      && (

                      model2.TravelTimeOut >= model.TravelStartDate
                      ||
                      model2.TravelTimeOut >= model.RRStartDate
                      ||
                      (
                      model.ALStartDate != null &&
                      model2.TravelTimeOut >= model.ALStartDate

                      )

                      )))
                {
                    ModelState.AddModelError("Error: ", "Kindly revise the entry data to avoid conflict in dates");
                    return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_TempLeaveDatesUpdateModal.cshtml", model);
                }


            }

            DateTime ExecutionTime = DateTime.Now;
            List<dataTempRestAndRecuperationRequestLeaveDate> allleaves = new List<dataTempRestAndRecuperationRequestLeaveDate>();
            dataTempRestAndRecuperationRequestLeaveDate temp = new dataTempRestAndRecuperationRequestLeaveDate();
            List<StaffLeaveHolidayDates> _realleaves = new List<StaffLeaveHolidayDates>();

            temp = new dataTempRestAndRecuperationRequestLeaveDate();
            temp.TempRestAndRecuperationRequestLeaveDateGUID = Guid.NewGuid();
            temp.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;
            temp.LeaveTypeName = "Travel Time";
            temp.LeaveTypeGUID = coddeInternationalStaffAttendanceTypeAttendanceTable.TravelTime;
            temp.TravelTimeOut = model.TravelStartDate;
            temp.TravelTimeIn = model.TravelStartDate;
            temp.Comments = model.Comments;
            allleaves.Add(temp);

            //temp = new dataTempRestAndRecuperationRequestLeaveDate();
            //temp.TempRestAndRecuperationRequestLeaveDateGUID = Guid.NewGuid();
            //temp.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;
            //temp.LeaveTypeName = "Travel Time";
            //temp.LeaveTypeGUID = coddeInternationalStaffAttendanceTypeAttendanceTable.TravelTime;
            //temp.TravelTimeOut = model.TravelReturnDate;
            //temp.TravelTimeIn = model.TravelReturnDate;
            //temp.Comments = model.Comments;
            //allleaves.Add(temp);




            //var leaveName = DbAHD.codeTablesValuesLanguages.Where(x => x.ValueGUID == model.LeaveTypeGUID && x.LanguageID == LAN).Select(x => x.ValueDescription).FirstOrDefault();



            _realleaves = model.ALStartDate == null && model.RRStartDate != null ?

                CheckCustomLeaveDays((DateTime)model.RRStartDate, (DateTime)model.RRReturnDate, coddeInternationalStaffAttendanceTypeAttendanceTable.RR, "R And R", (Guid)model.RestAndRecuperationLeaveGUID, (DateTime)model.TravelReturnDate)
                :

                 model.ALStartDate != null && model.RRStartDate == null ?
                 CheckCustomLeaveDays((DateTime)model.ALStartDate, (DateTime)model.ALReturnDate, coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave, "Annual Leave", (Guid)model.RestAndRecuperationLeaveGUID, (DateTime)model.TravelReturnDate) :
                  model.ALStartDate != null && model.RRStartDate != null &&
                  model.RRStartDate > model.ALStartDate ?
                  CheckCustomLeaveDays((DateTime)model.ALStartDate, (DateTime)model.ALReturnDate, coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave, "Annual Leave", (Guid)model.RestAndRecuperationLeaveGUID, (DateTime)model.TravelReturnDate) :

                  model.ALStartDate > model.RRStartDate ?
                  CheckCustomLeaveDays((DateTime)model.RRStartDate, (DateTime)model.RRReturnDate, coddeInternationalStaffAttendanceTypeAttendanceTable.RR, "R And R", (Guid)model.RestAndRecuperationLeaveGUID, (DateTime)model.TravelReturnDate) :
                  new List<StaffLeaveHolidayDates>()

                ;

            Guid leaveTypeguid = Guid.Empty;

            foreach (var item in _realleaves.Where(x => x.LeaveTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend || x.LeaveTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday))
            {
                temp = new dataTempRestAndRecuperationRequestLeaveDate();
                temp.TempRestAndRecuperationRequestLeaveDateGUID = Guid.NewGuid();
                temp.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;
                temp.LeaveTypeName = item.LeaveName;
                temp.LeaveTypeGUID = item.LeaveTypeGUID;
                temp.TravelTimeOut = item.startdateName;
                temp.TravelTimeIn = item.startdateName;
                temp.Comments = model.Comments;
                allleaves.Add(temp);
            }
            if ((model.ALStartDate == null && model.RRStartDate != null) || (model.ALStartDate != null && model.ALStartDate > model.RRStartDate))
            {
                leaveTypeguid = coddeInternationalStaffAttendanceTypeAttendanceTable.RR;
            }
            else if ((model.ALStartDate != null && model.RRStartDate > model.ALStartDate))
            {
                leaveTypeguid = coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave;
            }

            foreach (var item in _realleaves.Where(x => x.LeaveTypeGUID == leaveTypeguid))
            {
                temp = new dataTempRestAndRecuperationRequestLeaveDate();
                temp.TempRestAndRecuperationRequestLeaveDateGUID = Guid.NewGuid();
                temp.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;
                temp.LeaveTypeName = item.LeaveName;
                temp.LeaveTypeGUID = item.LeaveTypeGUID;
                temp.TravelTimeOut = item.enddateName;
                temp.TravelTimeIn = item.startdateName;
                temp.Comments = model.Comments;
                allleaves.Add(temp);
            }


            if (model.RRStartDate != null && model.ALStartDate != null)
            {
                List<StaffLeaveHolidayDates> _realleaves_last = new List<StaffLeaveHolidayDates>();
                var lastRelative = _realleaves.OrderByDescending(x => x.enddateName).FirstOrDefault();
                DateTime _start = lastRelative.enddateName.Value.AddDays(1);
                var du = model.RRStartDate > model.ALStartDate ? (model.RRReturnDate - model.RRStartDate).Value.Days + 1 :
                    (model.ALReturnDate - model.ALStartDate).Value.Days + 1
                    ;
                DateTime _end = _start.AddDays(du);

                _realleaves_last = model.RRStartDate > model.ALStartDate ?

                   CheckCustomLeaveDays((DateTime)_start, _end, coddeInternationalStaffAttendanceTypeAttendanceTable.RR, "R And R", (Guid)model.RestAndRecuperationLeaveGUID, (DateTime)model.TravelReturnDate) :
                   model.ALStartDate > model.RRStartDate ?

                   CheckCustomLeaveDays(_start, _end, coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave, "Annual Leave", (Guid)model.RestAndRecuperationLeaveGUID, (DateTime)model.TravelReturnDate) :

                 new List<StaffLeaveHolidayDates>();
                foreach (var item in _realleaves_last.Where(x => x.LeaveTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend || x.LeaveTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday))
                {
                    temp = new dataTempRestAndRecuperationRequestLeaveDate();
                    temp.TempRestAndRecuperationRequestLeaveDateGUID = Guid.NewGuid();
                    temp.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;
                    temp.LeaveTypeName = item.LeaveName;
                    temp.LeaveTypeGUID = item.LeaveTypeGUID;
                    temp.TravelTimeOut = item.startdateName;
                    temp.TravelTimeIn = item.startdateName;
                    temp.Comments = model.Comments;
                    allleaves.Add(temp);
                }
                if ((model.ALStartDate == null && model.RRStartDate != null) || (model.ALStartDate != null && model.RRStartDate > model.ALStartDate))
                {
                    leaveTypeguid = coddeInternationalStaffAttendanceTypeAttendanceTable.RR;
                }
                else if ((model.ALStartDate != null && model.ALStartDate > model.RRStartDate))
                {
                    leaveTypeguid = coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave;
                }

                foreach (var item in _realleaves_last.Where(x => x.LeaveTypeGUID == leaveTypeguid))
                {
                    temp = new dataTempRestAndRecuperationRequestLeaveDate();
                    temp.TempRestAndRecuperationRequestLeaveDateGUID = Guid.NewGuid();
                    temp.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;
                    temp.LeaveTypeName = item.LeaveName;
                    temp.LeaveTypeGUID = item.LeaveTypeGUID;
                    temp.TravelTimeOut = item.enddateName;
                    temp.TravelTimeIn = item.startdateName;
                    temp.Comments = model.Comments;
                    allleaves.Add(temp);
                }


            }

            var lastdate = allleaves.OrderByDescending(x => x.TravelTimeOut).FirstOrDefault();
            temp = new dataTempRestAndRecuperationRequestLeaveDate();
            temp.TempRestAndRecuperationRequestLeaveDateGUID = Guid.NewGuid();
            temp.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;
            temp.LeaveTypeName = "Travel Time";
            temp.LeaveTypeGUID = coddeInternationalStaffAttendanceTypeAttendanceTable.TravelTime;
            temp.TravelTimeOut = lastdate.TravelTimeOut.Value.AddDays(1);
            temp.TravelTimeIn = lastdate.TravelTimeOut.Value.AddDays(1);
            temp.Comments = model.Comments;
            allleaves.Add(temp);


            DbAHD.CreateBulk(allleaves, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);




            try
            {

                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, null, "InitializeCalendarLeavs();"));

                // return Json(DbAHD.SingleUpdateMessage(DataTableNames.InternationalTempStaffRAndRDatesDataTable, DbAHD.PrimaryKeyControl(allleaves.FirstOrDefault()), DbAHD.RowVersionControls(Portal.SingleToList(allleaves.FirstOrDefault()))));

            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }
        public List<StaffLeaveHolidayDates> CheckCustomLeaveDays(DateTime _startDate, DateTime _endDate, Guid _leaveTypeGUID, string leaveName, Guid _restAndRecuperationLeaveGUID, DateTime lastDay)
        {
            List<StaffLeaveHolidayDates> _holidayleaves = new List<StaffLeaveHolidayDates>();

            var _holiday = DbAHD.codeOrganizationHoliday.ToList();
            int j = 0;
            var lastdate = _endDate;
            var myStart = _startDate;
            int _totalRRLeaves = 0;

            for (var day = _startDate; day <= lastdate; day = day.AddDays(1))
            {
                int _res = 0;
                if (_totalRRLeaves == 5 && _leaveTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR)
                {
                    break;
                }

                if ((_leaveTypeGUID != coddeInternationalStaffAttendanceTypeAttendanceTable.RR) && ((_leaveTypeGUID != coddeInternationalStaffAttendanceTypeAttendanceTable.TravelTime)) && (day.DayOfWeek.ToString() == "Friday" || day.DayOfWeek.ToString() == "Saturday"))
                {
                    StaffLeaveHolidayDates myHoliday = new StaffLeaveHolidayDates
                    {
                        startdateName = day,
                        LeaveTypeGUID = coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend,
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
                                    LeaveTypeGUID = coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday,
                                    LeaveName = "Official Holiday"
                                };
                                j++;
                                _res = 1;
                                //lastdate = lastdate.AddDays(1);
                                _holidayleaves.Add(myHoliday);

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
            var checkLeaves = DbAHD.dataTempRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == _restAndRecuperationLeaveGUID).ToList();

            var checkPri = DbAHD.dataTempRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == _restAndRecuperationLeaveGUID).OrderByDescending(x => x.TravelTimeOut).FirstOrDefault();
            var start_priDay_oneday = _startDate.AddDays(-1);
            var start_priDay = _startDate.AddDays(-2);
            var start_priTwoDay = _startDate.AddDays(-3);
            var start_prithreeDay = _startDate.AddDays(-4);

            if (checkPri != null && (start_priDay == checkPri.TravelTimeOut || start_priTwoDay == checkPri.TravelTimeOut || start_prithreeDay == checkPri.TravelTimeOut))
            {

                foreach (var item in _holiday)
                {
                    for (var dayhol = item.HolidayStartDate; dayhol <= item.HolidayEndDate; dayhol = dayhol.AddDays(1))
                    {
                        var checkleaveexit = checkLeaves.Where(x => x.TravelTimeIn <= dayhol && x.TravelTimeOut >= dayhol).FirstOrDefault();
                        if (checkleaveexit != null)
                        {
                            bool funct = true;
                        }
                        else
                        {

                            if (start_priDay == dayhol || start_priDay_oneday == dayhol || start_prithreeDay == dayhol || start_priTwoDay == dayhol)
                            {
                                StaffLeaveHolidayDates myHoliday = new StaffLeaveHolidayDates
                                {
                                    startdateName = dayhol,
                                    LeaveTypeGUID = coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday,
                                    LeaveName = "Official Holiday"
                                };

                                _holidayleaves.Add(myHoliday);

                            }
                        }

                    }

                }

                if (start_priDay.DayOfWeek.ToString() == "Friday" || start_priDay.DayOfWeek.ToString() == "Saturday")
                {
                    var checkleaveexit = checkLeaves.Where(x => x.TravelTimeIn <= start_priDay && x.TravelTimeOut >= start_priDay).FirstOrDefault();
                    if (checkleaveexit != null)
                    {
                        bool funct = true;
                    }
                    else
                    {
                        StaffLeaveHolidayDates myHoliday = new StaffLeaveHolidayDates
                        {
                            startdateName = start_priDay,
                            LeaveTypeGUID = coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend,
                            LeaveName = "Weekend"
                        };



                        _holidayleaves.Add(myHoliday);
                    }
                }
                if (start_priDay_oneday.DayOfWeek.ToString() == "Friday" || start_priDay_oneday.DayOfWeek.ToString() == "Saturday")
                {
                    var checkleaveexit = checkLeaves.Where(x => x.TravelTimeIn <= start_priDay_oneday && x.TravelTimeOut >= start_priDay_oneday).FirstOrDefault();
                    if (checkleaveexit != null)
                    {
                        bool funct = true;
                    }
                    else
                    {
                        StaffLeaveHolidayDates myHoliday = new StaffLeaveHolidayDates
                        {
                            startdateName = start_priDay_oneday,
                            LeaveTypeGUID = coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend,
                            LeaveName = "Weekend"
                        };



                        _holidayleaves.Add(myHoliday);
                    }
                }


            }
            return _holidayleaves;

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TempRestAndRecuperationLeaveDateCreate(TempRestAndRecuperationRequestLeaveDateModel model)
        {

            if (!ModelState.IsValid) return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_TempTravelTimeOutUpdateModal.cshtml", model);
            var toCheck = DbAHD.dataTempRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID
            && (x.TravelTimeIn >= model.TravelTimeIn
                      && x.TravelTimeOut <= model.TravelTimeOut)).FirstOrDefault();
            if (model.LeaveTypeGUID == null || model.TravelTimeIn == null || model.TravelTimeOut == null || model.TravelTimeIn > model.TravelTimeOut
                || (toCheck != null))
            {
                if (model.TravelTimeIn > model.TravelTimeOut)
                {
                    ModelState.AddModelError("Error: ", "Leave start must be less than leave end date");
                }
                else if (model.TravelTimeIn == null)
                {
                    ModelState.AddModelError("Error: ", "Leave start date is required ");
                }
                else if (model.TravelTimeOut == null)
                {
                    ModelState.AddModelError("Error: ", "Leave end date is required ");
                }
                else if (model.LeaveTypeGUID == null)
                {
                    ModelState.AddModelError("Error: ", "Leave type  is required ");
                }
                //ModelState.AddModelError("Error: ", "Kindly revise the entry data to avoid conflict in dates");
                return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_TempTravelTimeOutUpdateModal.cshtml", model);
            }
            var toCheckallTemp = DbAHD.dataTempRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID).ToList();
            foreach (var model2 in toCheckallTemp)
            {
                if ((model2.TravelTimeIn <= model.TravelTimeOut && (model2.TravelTimeOut >= model.TravelTimeIn)))
                {

                    ModelState.AddModelError("Error: ", "Start date is overlapping with previous RR Records Revise start date");
                    return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_TempTravelTimeOutUpdateModal.cshtml", model);
                }


            }

            var tocheckAll = DbAHD.dataRestAndRecuperationRequestLeaveDate.Where(x => x.dataRestAndRecuperationRequest.StaffGUID == UserGUID).ToList();


            foreach (var model2 in tocheckAll)
            {
                if ((model.TravelTimeIn <= model2.TravelTimeOut)
                      || (model2.TravelTimeIn <= model.TravelTimeOut && (model2.TravelTimeOut >= model.TravelTimeIn)))
                {
                    ModelState.AddModelError("Error: ", "Kindly revise the entry data to avoid conflict in dates");
                    return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_TempTravelTimeOutUpdateModal.cshtml", model);
                }


            }

            DateTime ExecutionTime = DateTime.Now;
            List<dataTempRestAndRecuperationRequestLeaveDate> allleaves = new List<dataTempRestAndRecuperationRequestLeaveDate>();
            dataTempRestAndRecuperationRequestLeaveDate temp = new dataTempRestAndRecuperationRequestLeaveDate();

            var leaveName = DbAHD.codeTablesValuesLanguages.Where(x => x.ValueGUID == model.LeaveTypeGUID && x.LanguageID == LAN).Select(x => x.ValueDescription).FirstOrDefault();
            //travel time 
            //if (model.LeaveTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TravelTime)
            //{
            //    var check=DbAHD.dataTempRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID
            //    ).ToList();
            //    var lastleave = check.OrderByDescending(x => x.TravelTimeOut).FirstOrDefault();

            //    //if (lastleave.TravelTimeOut)
            //    temp = new dataTempRestAndRecuperationRequestLeaveDate();
            //    temp.TempRestAndRecuperationRequestLeaveDateGUID = Guid.NewGuid();
            //    temp.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;
            //    temp.LeaveTypeName = "";
            //    temp.LeaveTypeGUID = model.LeaveTypeGUID;
            //    temp.TravelTimeOut = model.TravelTimeIn;
            //    temp.TravelTimeIn = model.TravelTimeOut;
            //    temp.Comments = model.Comments;
            //    allleaves.Add(temp);
            //    DbAHD.CreateBulk(allleaves, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);

            //    try
            //    {

            //        DbAHD.SaveChanges();
            //        DbCMS.SaveChanges();
            //        return Json(DbCMS.SingleUpdateMessage(null, null, null, "InitializeCalendarLeavs();"));

            //        // return Json(DbAHD.SingleUpdateMessage(DataTableNames.InternationalTempStaffRAndRDatesDataTable, DbAHD.PrimaryKeyControl(allleaves.FirstOrDefault()), DbAHD.RowVersionControls(Portal.SingleToList(allleaves.FirstOrDefault()))));

            //    }
            //    catch (Exception ex)
            //    {
            //        return Json(DbAHD.ErrorMessage(ex.Message));
            //    }

            //}

            List<StaffLeaveHolidayDates> _realleaves = CheckCustomLeaveDays((DateTime)model.TravelTimeIn, (DateTime)model.TravelTimeOut, (Guid)model.LeaveTypeGUID, leaveName, (Guid)model.RestAndRecuperationLeaveGUID, (DateTime)model.TravelTimeOut);


            foreach (var item in _realleaves.Where(x => x.LeaveTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend || x.LeaveTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday))
            {
                temp = new dataTempRestAndRecuperationRequestLeaveDate();
                temp.TempRestAndRecuperationRequestLeaveDateGUID = Guid.NewGuid();
                temp.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;
                temp.LeaveTypeName = item.LeaveName;
                temp.LeaveTypeGUID = item.LeaveTypeGUID;
                temp.TravelTimeOut = item.startdateName;
                temp.TravelTimeIn = item.startdateName;
                temp.Comments = model.Comments;
                allleaves.Add(temp);
            }

            foreach (var item in _realleaves.Where(x => x.LeaveTypeGUID == model.LeaveTypeGUID))
            {
                temp = new dataTempRestAndRecuperationRequestLeaveDate();
                temp.TempRestAndRecuperationRequestLeaveDateGUID = Guid.NewGuid();
                temp.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;
                temp.LeaveTypeName = item.LeaveName;
                temp.LeaveTypeGUID = item.LeaveTypeGUID;
                temp.TravelTimeOut = item.enddateName;
                temp.TravelTimeIn = item.startdateName;
                temp.Comments = model.Comments;
                allleaves.Add(temp);
            }



            DbAHD.CreateBulk(allleaves, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);

            try
            {

                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, null, "InitializeCalendarLeavs();"));

                // return Json(DbAHD.SingleUpdateMessage(DataTableNames.InternationalTempStaffRAndRDatesDataTable, DbAHD.PrimaryKeyControl(allleaves.FirstOrDefault()), DbAHD.RowVersionControls(Portal.SingleToList(allleaves.FirstOrDefault()))));

            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }



        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TempRestAndRecuperationRRALDateCreate(TempRestAndRecuperationRequestLeaveDateModel model)
        {

            if (!ModelState.IsValid) return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_TempLeaveDatesUpdateModal.cshtml", model);
            var toCheck = DbAHD.dataTempRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID
            && (x.TravelTimeIn >= model.TravelTimeIn
                      && x.TravelTimeOut <= model.TravelTimeOut)).FirstOrDefault();
            if (model.TravelTimeIn == null || model.TravelTimeOut == null || model.TravelTimeIn > model.TravelTimeOut
                || (toCheck != null))
            {
                if (model.TravelTimeIn > model.TravelTimeOut)
                {
                    ModelState.AddModelError("Error: ", "Leave start must be less than leave end date");
                }
                else if (model.TravelTimeIn == null)
                {
                    ModelState.AddModelError("Error: ", "Leave start date is required ");
                }
                else if (model.TravelTimeOut == null)
                {
                    ModelState.AddModelError("Error: ", "Leave end date is required ");
                }
                else if (model.LeaveTypeGUID == null)
                {
                    ModelState.AddModelError("Error: ", "Leave type  is required ");
                }
                //ModelState.AddModelError("Error: ", "Kindly revise the entry data to avoid conflict in dates");
                return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_TempLeaveDatesUpdateModal.cshtml", model);
            }
            var toCheckallTemp = DbAHD.dataTempRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID).ToList();
            foreach (var model2 in toCheckallTemp)
            {
                if ((model2.TravelTimeIn <= model.TravelTimeOut && (model2.TravelTimeOut >= model.TravelTimeIn)))
                {

                    ModelState.AddModelError("Error: ", "Start date is overlapping with previous RR Records Revise start date");
                    return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_TempLeaveDatesUpdateModal.cshtml", model);
                }


            }

            var tocheckAll = DbAHD.dataRestAndRecuperationRequestLeaveDate.Where(x => x.dataRestAndRecuperationRequest.StaffGUID == UserGUID).ToList();


            foreach (var model2 in tocheckAll)
            {
                if ((model.TravelTimeIn <= model2.TravelTimeOut)
                      || (model2.TravelTimeIn <= model.TravelTimeOut && (model2.TravelTimeOut >= model.TravelTimeIn)))
                {
                    ModelState.AddModelError("Error: ", "Kindly revise the entry data to avoid conflict in dates");
                    return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_TempLeaveDatesUpdateModal.cshtml", model);
                }


            }

            DateTime ExecutionTime = DateTime.Now;
            List<dataTempRestAndRecuperationRequestLeaveDate> allleaves = new List<dataTempRestAndRecuperationRequestLeaveDate>();
            dataTempRestAndRecuperationRequestLeaveDate temp = new dataTempRestAndRecuperationRequestLeaveDate();

            var leaveName = DbAHD.codeTablesValuesLanguages.Where(x => x.ValueGUID == model.LeaveTypeGUID && x.LanguageID == LAN).Select(x => x.ValueDescription).FirstOrDefault();


            List<StaffLeaveHolidayDates> _realleaves = CheckCustomLeaveDays((DateTime)model.TravelTimeIn, (DateTime)model.TravelTimeOut, (Guid)model.LeaveTypeGUID, leaveName, (Guid)model.RestAndRecuperationLeaveGUID, (DateTime)model.TravelTimeOut);


            foreach (var item in _realleaves.Where(x => x.LeaveTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend || x.LeaveTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday))
            {
                temp = new dataTempRestAndRecuperationRequestLeaveDate();
                temp.TempRestAndRecuperationRequestLeaveDateGUID = Guid.NewGuid();
                temp.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;
                temp.LeaveTypeName = item.LeaveName;
                temp.LeaveTypeGUID = item.LeaveTypeGUID;
                temp.TravelTimeOut = item.startdateName;
                temp.TravelTimeIn = item.startdateName;
                temp.Comments = model.Comments;
                allleaves.Add(temp);
            }

            foreach (var item in _realleaves.Where(x => x.LeaveTypeGUID == model.LeaveTypeGUID))
            {
                temp = new dataTempRestAndRecuperationRequestLeaveDate();
                temp.TempRestAndRecuperationRequestLeaveDateGUID = Guid.NewGuid();
                temp.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;
                temp.LeaveTypeName = item.LeaveName;
                temp.LeaveTypeGUID = item.LeaveTypeGUID;
                temp.TravelTimeOut = item.enddateName;
                temp.TravelTimeIn = item.startdateName;
                temp.Comments = model.Comments;
                allleaves.Add(temp);
            }



            DbAHD.CreateBulk(allleaves, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);

            try
            {

                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, null, "InitializeCalendarLeavs();"));

                // return Json(DbAHD.SingleUpdateMessage(DataTableNames.InternationalTempStaffRAndRDatesDataTable, DbAHD.PrimaryKeyControl(allleaves.FirstOrDefault()), DbAHD.RowVersionControls(Portal.SingleToList(allleaves.FirstOrDefault()))));

            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }



        public List<StaffLeaveHolidayDates> CheckLeaveDays(DateTime _startDate, DateTime _endDate, Guid _leaveTypeGUID, string leaveName, Guid _restAndRecuperationLeaveGUID)
        {
            List<StaffLeaveHolidayDates> _holidayleaves = new List<StaffLeaveHolidayDates>();

            var _holiday = DbAHD.codeOrganizationHoliday.ToList();
            int j = 0;
            var lastdate = _endDate;
            var myStart = _startDate;

            for (var day = _startDate; day <= lastdate; day = day.AddDays(1))
            {
                int _res = 0;

                if (day.DayOfWeek.ToString() == "Friday" || day.DayOfWeek.ToString() == "Saturday")
                {
                    StaffLeaveHolidayDates myHoliday = new StaffLeaveHolidayDates
                    {
                        startdateName = day,
                        LeaveTypeGUID = coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend,
                        LeaveName = "Weekend"
                    };
                    j++;
                    lastdate = lastdate.AddDays(1);

                    _holidayleaves.Add(myHoliday);
                    _res = 1;

                }
                if (_res == 0)
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
                                    LeaveTypeGUID = coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday,
                                    LeaveName = "Official Holiday"
                                };
                                j++;
                                lastdate = lastdate.AddDays(1);
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

                }

            }

            var checkPri = DbAHD.dataTempRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == _restAndRecuperationLeaveGUID).OrderByDescending(x => x.TravelTimeOut).FirstOrDefault();
            var start_priDay_oneday = _startDate.AddDays(-1);
            var start_priDay = _startDate.AddDays(-2);
            var start_priTwoDay = _startDate.AddDays(-3);

            if (checkPri != null && (start_priDay == checkPri.TravelTimeOut || start_priTwoDay == checkPri.TravelTimeOut))
            {
                foreach (var item in _holiday)
                {
                    for (var dayhol = item.HolidayStartDate; dayhol <= item.HolidayEndDate; dayhol = dayhol.AddDays(1))
                    {

                        if (start_priDay == dayhol || start_priDay_oneday == dayhol)
                        {
                            StaffLeaveHolidayDates myHoliday = new StaffLeaveHolidayDates
                            {
                                startdateName = dayhol,
                                LeaveTypeGUID = coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday,
                                LeaveName = "Official Holiday"
                            };

                            _holidayleaves.Add(myHoliday);

                        }

                    }


                }
                if ((start_priDay.DayOfWeek.ToString() == "Friday" || start_priDay.DayOfWeek.ToString() == "Saturday")
                    && (start_priDay_oneday.DayOfWeek.ToString() != "Sunday"))
                {
                    StaffLeaveHolidayDates myHoliday = new StaffLeaveHolidayDates
                    {
                        startdateName = start_priDay,
                        LeaveTypeGUID = coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend,
                        LeaveName = "Weekend"
                    };



                    _holidayleaves.Add(myHoliday);
                }
                if (start_priDay_oneday.DayOfWeek.ToString() == "Friday" || start_priDay_oneday.DayOfWeek.ToString() == "Saturday")
                {
                    StaffLeaveHolidayDates myHoliday = new StaffLeaveHolidayDates
                    {
                        startdateName = start_priTwoDay,
                        LeaveTypeGUID = coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend,
                        LeaveName = "Weekend"
                    };



                    _holidayleaves.Add(myHoliday);
                }





            }
            return _holidayleaves;

        }

        public ActionResult TempRestAndRecuperationLeaveDateUpdate(Guid PK)
        {




            var model = (from a in DbAHD.dataTempRestAndRecuperationRequestLeaveDate.WherePK(PK)

                         join b in DbAHD.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN) on a.LeaveTypeGUID equals b.ValueGUID into LJ2
                         from R2 in LJ2.DefaultIfEmpty()
                         select new TempRestAndRecuperationRequestLeaveDateModel
                         {
                             TempRestAndRecuperationRequestLeaveDateGUID = a.TempRestAndRecuperationRequestLeaveDateGUID,
                             RestAndRecuperationLeaveGUID = a.RestAndRecuperationLeaveGUID,



                             LeaveTypeGUID = a.LeaveTypeGUID,
                             LeaveTypeName = R2.ValueDescription,
                             TravelTimeIn = a.TravelTimeIn,
                             TravelTimeOut = a.TravelTimeOut,
                             Comments = a.Comments,

                             Active = a.Active,

                         }).FirstOrDefault();


            Guid hrAccessGUID = Guid.Parse("DE411F27-0EA8-49FB-964D-00833BB012F8");
            var myModel = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID).FirstOrDefault();

            var checkCurrUser = DbCMS.userProfiles.Where(x => x.userServiceHistory.UserGUID == UserGUID && x.Active).Select(x => x.UserProfileGUID).FirstOrDefault();
            var hasHRPermissionsGuid = DbCMS.userPermissions.Where(x => x.ActionGUID == hrAccessGUID && x.UserProfileGUID == checkCurrUser && x.Active).
                Select(x => x.UserProfileGUID).ToList();



            if ((myModel != null && UserGUID != myModel.CreatedByGUID) && hasHRPermissionsGuid.Count <= 0)
            {
                //myModel.AccessLevel = 1;
                //ViewBag.userAccessLevel = 1;
                return Json(DbAHD.PermissionError());
            }




            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("TempRestAndRecuperationLeaveDate", "StaffRAndRLeave", new { Area = "AHD" }));

            return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_TempLeaveDatesUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TempRestAndRecuperationLeaveDateUpdate(TempRestAndRecuperationRequestLeaveDateModel model)
        {
            Guid hrAccessGUID = Guid.Parse("DE411F27-0EA8-49FB-964D-00833BB012F8");
            var myModel = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID).FirstOrDefault();
            var tempdates = DbAHD.dataTempRestAndRecuperationRequestLeaveDate.Where(x => x.TempRestAndRecuperationRequestLeaveDateGUID == model.TempRestAndRecuperationRequestLeaveDateGUID).FirstOrDefault();

            var checkCurrUser = DbCMS.userProfiles.Where(x => x.userServiceHistory.UserGUID == UserGUID && x.Active).Select(x => x.UserProfileGUID).FirstOrDefault();
            var hasHRPermissionsGuid = DbCMS.userPermissions.Where(x => x.ActionGUID == hrAccessGUID && x.UserProfileGUID == checkCurrUser && x.Active).
                Select(x => x.UserProfileGUID).ToList();
            //here


            if ((myModel != null && UserGUID != myModel.StaffGUID) && hasHRPermissionsGuid.Count <= 0)
            {
                //myModel.AccessLevel = 1;
                //ViewBag.userAccessLevel = 1;
                return Json(DbAHD.PermissionError());
            }

            if (!ModelState.IsValid) return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_TempLeaveDatesUpdateModal.cshtml", model);


            if (model.LeaveTypeGUID == null || model.TravelTimeOut == null || model.TravelTimeIn == null)
            {
                return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_TempLeaveDatesUpdateModal.cshtml", model);
            }

            var _mytempLeave = DbAHD.dataTempRestAndRecuperationRequestLeaveDate.Where(x => x.TempRestAndRecuperationRequestLeaveDateGUID == model.TempRestAndRecuperationRequestLeaveDateGUID).FirstOrDefault();
            var _myLeave = DbAHD.dataRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationRequestLeaveDateGUID == model.TempRestAndRecuperationRequestLeaveDateGUID).FirstOrDefault();
            if (_mytempLeave != null)
            {
                DbAHD.dataTempRestAndRecuperationRequestLeaveDate.Remove(_mytempLeave);
            }
            if (_myLeave != null)
            {
                DbAHD.dataRestAndRecuperationRequestLeaveDate.Remove(_myLeave);
            }


            DbAHD.SaveChanges();


            DateTime ExecutionTime = DateTime.Now;
            List<dataTempRestAndRecuperationRequestLeaveDate> allleaves = new List<dataTempRestAndRecuperationRequestLeaveDate>();
            dataTempRestAndRecuperationRequestLeaveDate temp = new dataTempRestAndRecuperationRequestLeaveDate();

            var leaveName = DbAHD.codeTablesValuesLanguages.Where(x => x.ValueGUID == model.LeaveTypeGUID && x.LanguageID == LAN).Select(x => x.ValueDescription).FirstOrDefault();

            List<StaffLeaveHolidayDates> _realleaves = CheckCustomLeaveDays((DateTime)model.TravelTimeIn, (DateTime)model.TravelTimeOut, (Guid)model.LeaveTypeGUID, leaveName, (Guid)model.RestAndRecuperationLeaveGUID, (DateTime)model.TravelTimeOut);


            foreach (var item in _realleaves.Where(x => x.LeaveTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend || x.LeaveTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday))
            {
                temp = new dataTempRestAndRecuperationRequestLeaveDate();
                temp.TempRestAndRecuperationRequestLeaveDateGUID = Guid.NewGuid();
                temp.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;
                temp.LeaveTypeName = item.LeaveName;
                temp.LeaveTypeGUID = item.LeaveTypeGUID;
                temp.TravelTimeOut = item.startdateName;
                temp.TravelTimeIn = item.startdateName;
                temp.Comments = model.Comments;
                allleaves.Add(temp);
            }

            foreach (var item in _realleaves.Where(x => x.LeaveTypeGUID == model.LeaveTypeGUID))
            {
                temp = new dataTempRestAndRecuperationRequestLeaveDate();
                temp.TempRestAndRecuperationRequestLeaveDateGUID = Guid.NewGuid();
                temp.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;
                temp.LeaveTypeName = item.LeaveName;
                temp.LeaveTypeGUID = item.LeaveTypeGUID;
                temp.TravelTimeOut = item.enddateName;
                temp.TravelTimeIn = item.startdateName;
                temp.Comments = model.Comments;
                allleaves.Add(temp);
            }



            DbAHD.CreateBulk(allleaves, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);
            dataRestAndRecuperationRequestLeaveDate _dateleave = new dataRestAndRecuperationRequestLeaveDate();
            List<dataRestAndRecuperationRequestLeaveDate> _alldateleave = new List<dataRestAndRecuperationRequestLeaveDate>();

            foreach (var item in allleaves)
            {
                _dateleave = new dataRestAndRecuperationRequestLeaveDate();
                _dateleave.RestAndRecuperationRequestLeaveDateGUID = item.TempRestAndRecuperationRequestLeaveDateGUID;
                _dateleave.RestAndRecuperationLeaveGUID = myModel.RestAndRecuperationLeaveGUID;
                _dateleave.LeaveTypeGUID = item.LeaveTypeGUID;
                _dateleave.LeaveTypeName = item.LeaveTypeName;
                _dateleave.TravelTimeOut = item.TravelTimeOut;
                _dateleave.TravelTimeIn = item.TravelTimeIn;
                _dateleave.Comments = item.Comments;

                _alldateleave.Add(_dateleave);
            }

            DbAHD.CreateBulk(_alldateleave, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);



            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                var toremove = DbAHD.dataInternationalStaffAttendance.Where(x => x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID).ToList();
                if (toremove.Count > 0)
                {
                    DbAHD.dataInternationalStaffAttendance.RemoveRange(toremove);
                    var toAddAttendace = DbAHD.dataRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID).ToList();
                    var staffrequest = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID).FirstOrDefault();
                    var userperson = DbAHD.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active == true && x.UserGUID == staffrequest.StaffGUID).FirstOrDefault();

                    //attendaces 
                    List<dataInternationalStaffAttendance> allAttendacnes = new List<dataInternationalStaffAttendance>();
                    foreach (var item in toAddAttendace)
                    {
                        if (item.LeaveTypeGUID != Guid.Empty)
                        {
                            dataInternationalStaffAttendance attendance = new dataInternationalStaffAttendance
                            {
                                InternationalStaffAttendanceGUID = Guid.NewGuid(),
                                InternationalStaffAttendanceTypeGUID = item.LeaveTypeGUID,
                                StaffGUID = staffrequest.StaffGUID,
                                StaffName = userperson.FirstName + " " + userperson.Surname,
                                LeaveLocation = item.dataRestAndRecuperationRequest.StaffLoction,
                                TotalDays = item.TravelTimeOut != null ? ((int)(item.TravelTimeOut - item.TravelTimeIn).Value.TotalDays + 1) : 1,
                                Comments = item.Comments,
                                IsAutomated = false,
                                FromDate = item.TravelTimeIn,
                                ToDate = item.TravelTimeOut,
                                CreatedByGUID = UserGUID,
                                CreateDate = ExecutionTime,
                                RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID,
                                RestAndRecuperationRequestLeaveDateGUID = item.RestAndRecuperationRequestLeaveDateGUID

                            };
                            allAttendacnes.Add(attendance);

                        }

                    }

                    DbAHD.CreateBulk(allAttendacnes, Permissions.InternationalStaffRestAndRecuperationLeaveHRReview.CreateGuid, ExecutionTime, DbCMS);
                    DbAHD.SaveChanges();
                    DbCMS.SaveChanges();
                }




                return Json(DbCMS.SingleUpdateMessage(null, null, null, "InitializeCalendarLeavs();"));
                //return Json(DbAHD.SingleUpdateMessage(DataTableNames.InternationalTempStaffRAndRDatesDataTable, DbAHD.PrimaryKeyControl(tempdates), DbAHD.RowVersionControls(Portal.SingleToList(tempdates))));

            }
            catch (DbUpdateConcurrencyException)
            {
                //return ConcurrencyItemModel(model.WarehouseItemModelGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
            return Json(DbAHD.SingleUpdateMessage(null, null, DbAHD.RowVersionControls(tempdates, tempdates)));
        }
        [HttpPost]
        public ActionResult NotifyStaffByNewLeaveDates(Guid FK, string _travelLocaionDescription)
        {
            var myModel = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == FK).FirstOrDefault();
            myModel.TravelLocaionDescription = _travelLocaionDescription;

            try
            {
                DbAHD.UpdateNoAudit(myModel);
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();

            }
            catch (Exception)
            {

                throw;
            }


            string SubjectMessage = "R And R Leave Review";
            var URL = AppSettingsKeys.Domain + "/AHD/StaffRAndRLeave/ConfirmHRApproval/?PK=" + new Portal().GUIDToString(myModel.RestAndRecuperationLeaveGUID);
            var Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickToConfirm + "</a>";
            var Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";

            var myUser = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == myModel.StaffGUID
                                                                 && x.LanguageID == LAN && x.Active).FirstOrDefault();

            var hrUser = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == UserGUID
                                                                 && x.LanguageID == LAN && x.Active).FirstOrDefault();
            var emails = DbAHD.StaffCoreData.Where(x => x.UserGUID == myUser.UserGUID || x.UserGUID == hrUser.UserGUID).ToList();


            string myFirstName = myUser.FirstName;
            string mySurName = myUser.Surname;


            string _message = resxEmails.RAndRStaffCheckAfterHRUpdate
                .Replace("$FullName", myFirstName + " " + mySurName)
                .Replace("$hrstaff", hrUser.FirstName + " " + hrUser.Surname)
                .Replace("$VerifyLink", Anchor)
                .Replace("$LeaveNumber", myModel.LeaveNumber)
               ;
            if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
            int isRec = 1;
            var myEmail = emails.Where(x => x.UserGUID == myUser.UserGUID).Select(x => x.EmailAddress).FirstOrDefault();

            var myEmails = emails.Where(x => x.UserGUID != myUser.UserGUID).Select(x => x.EmailAddress).ToList();
            string copyEmails = string.Join(" ;", myEmails);


            SendCopy(myEmail, SubjectMessage, _message, isRec, copyEmails);

            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public ActionResult TempRestAndRecuperationLeaveDateDelete(TempRestAndRecuperationRequestLeaveDateModel model)
        {
            Guid hrAccessGUID = Guid.Parse("DE411F27-0EA8-49FB-964D-00833BB012F8");

            var tempdates = DbAHD.dataTempRestAndRecuperationRequestLeaveDate.Where(x => x.TempRestAndRecuperationRequestLeaveDateGUID == model.TempRestAndRecuperationRequestLeaveDateGUID).FirstOrDefault();

            var checkCurrUser = DbCMS.userProfiles.Where(x => x.userServiceHistory.UserGUID == UserGUID && x.Active).Select(x => x.UserProfileGUID).FirstOrDefault();
            var hasHRPermissionsGuid = DbCMS.userPermissions.Where(x => x.ActionGUID == hrAccessGUID && x.UserProfileGUID == checkCurrUser && x.Active).
                Select(x => x.UserProfileGUID).ToList();
            //here
            var templeave = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == tempdates.RestAndRecuperationLeaveGUID).FirstOrDefault();

            if ((tempdates != null && UserGUID != templeave.StaffGUID) && hasHRPermissionsGuid.Count <= 0)
            {
                //myModel.AccessLevel = 1;
                //ViewBag.userAccessLevel = 1;
                return Json(DbAHD.PermissionError());
            }



            var _mytempLeave = DbAHD.dataTempRestAndRecuperationRequestLeaveDate.Where(x => x.TempRestAndRecuperationRequestLeaveDateGUID == model.TempRestAndRecuperationRequestLeaveDateGUID).FirstOrDefault();
            var _myLeave = DbAHD.dataRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == model.TempRestAndRecuperationRequestLeaveDateGUID).FirstOrDefault();
            var _attendanceremove = DbAHD.dataInternationalStaffAttendance.Where(x => x.RestAndRecuperationRequestLeaveDateGUID == model.TempRestAndRecuperationRequestLeaveDateGUID).FirstOrDefault();

            if (_mytempLeave != null)
            {
                DbAHD.dataTempRestAndRecuperationRequestLeaveDate.Remove(_mytempLeave);
            }
            if (_myLeave != null)
            {
                DbAHD.dataRestAndRecuperationRequestLeaveDate.Remove(_myLeave);
            }
            if (_attendanceremove != null)
            {
                DbAHD.dataInternationalStaffAttendance.Remove(_attendanceremove);
            }

            try
            {
                DbAHD.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, null, "InitializeCalendarLeavs();"));
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Request

        public ActionResult StaffLeavsIndex()
        {
            //if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeave.Access, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}

            return View("~/Areas/AHD/Views/StaffRAndRLeaveRequest/Index.cshtml");
        }

        public ActionResult ShowRAndRLeaveRequestIndex()
        {
            //if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeave.Access, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            var staff = DbAHD.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            if (staff.RecruitmentTypeGUID == Guid.Empty || staff.RecruitmentTypeGUID != Guid.Parse("FC4B2E79-2B97-4252-A50B-915B07A12310"))
            {
                return Json(DbAHD.PermissionError());
            }

            return View("~/Areas/AHD/Views/StaffRAndRLeaveRequest/Index.cshtml");
        }
        public ActionResult StaffRAndRLeaveRequestForInternationalStaffIndex()
        {
            //if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeave.Access, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}

            return View("~/Areas/AHD/Views/StaffRAndRLeaveRequest/Index.cshtml");
        }

        // GET: AHD/StaffRAndRLeave
        [Route("AHD/StaffRAndRLeaveRequest/")]
        public ActionResult StaffRAndRLeaveRequestIndex()
        {
            //if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeave.Access, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}

            return View("~/Areas/AHD/Views/StaffRAndRLeaveRequest/Index.cshtml");
        }

        [Route("AHD/StaffRAndRLeaveRequest/IndexPage/")]
        public ActionResult StaffRAndRLeaveRequestIndexPage()
        {
            var staff = DbAHD.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            if (staff.RecruitmentTypeGUID == Guid.Empty || staff.RecruitmentTypeGUID != Guid.Parse("FC4B2E79-2B97-4252-A50B-915B07A12310"))
            {
                return Json(DbAHD.PermissionError());
            }

            return View("~/Areas/AHD/Views/StaffRAndRLeaveRequest/Index.cshtml");
        }
        [Route("AHD/StaffRAndRLeaveRequestDataTable/")]
        public JsonResult StaffRAndRLeaveRequestDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<StaffRAndRLeaveRequestDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<StaffRAndRLeaveRequestDataTableModel>(DataTable.Filters);
            }

            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.NationalStaffDangerPayManagement.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
            Guid actionGUID = Guid.Parse("6102e16b-e9b5-4e6f-ae65-7d2e994e64b7");
            Guid repGUID = Guid.Parse("B3BDF367-3F74-45E4-AF0A-1AEE013C0E81");

            var userProfilesGUIDs = DbCMS.userPermissions.Where(x => (x.ActionGUID == actionGUID || x.ActionGUID == repGUID) && x.Active == true).Select(x => x.UserProfileGUID).ToList();
            var userGUids = DbCMS.userProfiles.Where(x => userProfilesGUIDs.Contains(x.UserProfileGUID) && x.userServiceHistory.UserGUID == UserGUID).Select(x => x.userServiceHistory.UserGUID).FirstOrDefault();


            bool hr = false;
            if (userGUids != Guid.Empty && userGUids != null)
                hr = true;
            var All = (
                from a in DbAHD.dataRestAndRecuperationRequest.Where(x => (x.StaffGUID == UserGUID || x.SupervisorGUID == UserGUID) || (hr == true)).AsExpandable()
                    //join b in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.FlowStatusGUID equals b.ValueGUID into LJ1
                    //from R1 in LJ1.DefaultIfEmpty()

                select new StaffRAndRLeaveRequestDataTableModel
                {
                    RestAndRecuperationLeaveGUID = a.RestAndRecuperationLeaveGUID.ToString(),
                    StaffGUID = a.StaffGUID.ToString(),
                    SubmittedDate = a.SubmittedDate,
                    FlowStatusGUID = a.FlowStatusGUID.ToString(),
                    RequestStatus = a.LastFlowStatusName,
                    Active = a.Active,
                    LeaveNumber = a.LeaveNumber,
                    StaffName = a.StaffName,
                    StaffJobTitle = a.StaffJobTitle,
                    StaffLevel = a.StaffLevel,
                    StaffLoction = a.StaffLoction,
                    BackupArrangementGUID = a.BackupArrangementGUID.ToString(),
                    BackupArrangementName = a.BackupArrangementName,
                    ReturnDateFromLastRLeave = a.ReturnDateFromLastRLeave,
                    ExpiryDateOfResidency = a.ExpiryDateOfResidency,

                    TypeDateOfLastTravelInterrupting = a.TypeDateOfLastTravelInterrupting,
                    LeaveInDate = a.LeaveInDate,
                    LeaveOutDate = a.LeaveOutDate,
                    ActualLeaveInDate = a.ActualLeaveInDate,
                    ActualLeaveOutDate = a.ActualLeaveOutDate,
                    HRComments = a.HRComments,
                    dataRestAndRecuperationRequestRowVersion = a.dataRestAndRecuperationRequestRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<StaffRAndRLeaveRequestDataTableModel> Result = Mapper.Map<List<StaffRAndRLeaveRequestDataTableModel>>(All.OrderByDescending(x => x.SubmittedDate).Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("AHD/StaffRAndRLeaveRequestForm/Create/")]
        public ActionResult StaffRAndRLeaveRequestFormCreate()
        {
            StaffRAndRLeaveRequestModel model = new StaffRAndRLeaveRequestModel();
            var staffCore = DbAHD.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            var pserson = DbAHD.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.UserGUID == staffCore.UserGUID).FirstOrDefault();

            if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeave.Create, Apps.AHD))
            {
                if (staffCore.RecruitmentTypeGUID != Guid.Parse("FC4B2E79-2B97-4252-A50B-915B07A12310"))
                {
                    return Json(DbAHD.PermissionError());
                }
            }


            //if (staffCore.RecruitmentTypeGUID != Guid.Parse("FC4B2E79-2B97-4252-A50B-915B07A12310"))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            var _pri = DbAHD.dataRestAndRecuperationRequest.Where(x => x.StaffGUID == UserGUID).OrderByDescending(x => x.ActualLeaveOutDate).Take(1).FirstOrDefault();

            model.RestAndRecuperationLeaveGUID = Guid.NewGuid();
            model.CurrentStep = 0;
            model.AccessLevel = 1;
            ViewBag.userAccessLevel = 1;
            ViewBag.currentStep = 1;
            model.StaffName = pserson.FirstName + " " + pserson.Surname;
            model.ReturnDateFromLastRLeave = staffCore.ReturnDateFromLastRAndRLeave;
            model.ExpiryDateOfResidency = staffCore.ExpiryOfResidencyVisa;
            if (_pri != null)
            {
                model.SupervisorGUID = (Guid)_pri.SupervisorGUID;
                model.BackupArrangementGUID = (Guid)_pri.BackupArrangementGUID;
            }
            else
            {
                model.SupervisorGUID = (Guid)staffCore.ReportToGUID;
            }

            return View("~/Areas/AHD/Views/StaffRAndRLeaveRequest/RAndRStaffRequest.cshtml", model);
        }



        //[Route("AHD/StaffRAndRLeaveRequestForm/Create/")]
        //public ActionResult StaffRAndRLeaveRequestFormCreate()
        //{
        //    StaffRAndRLeaveRequestModel model = new StaffRAndRLeaveRequestModel();
        //    var staffCore = DbAHD.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
        //    var pserson = DbAHD.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.UserGUID == staffCore.UserGUID).FirstOrDefault();

        //    if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeave.Create, Apps.AHD))
        //    {
        //        if (staffCore.RecruitmentTypeGUID != Guid.Parse("FC4B2E79-2B97-4252-A50B-915B07A12310"))
        //        {
        //            return Json(DbAHD.PermissionError());
        //        }
        //    }


        //    //if (staffCore.RecruitmentTypeGUID != Guid.Parse("FC4B2E79-2B97-4252-A50B-915B07A12310"))
        //    //{
        //    //    return Json(DbAHD.PermissionError());
        //    //}

        //    model.RestAndRecuperationLeaveGUID = Guid.NewGuid();
        //    model.CurrentStep = 0;
        //    model.AccessLevel = 1;
        //    ViewBag.userAccessLevel = 1;
        //    ViewBag.currentStep = 1; 
        //    model.StaffName = pserson.FirstName + " " + pserson.Surname;
        //    model.ReturnDateFromLastRLeave = staffCore.ReturnDateFromLastRAndRLeave;
        //model.ExpiryDateOfResidency = staffCore.ExpiryOfResidencyVisa;-
        //    return View("~/Areas/AHD/Views/StaffRAndRLeaveRequest/RAndRLeaveRequestForm.cshtml", model);
        //}

        [Route("AHD/StaffRAndRLeaveRequest/Create/")]
        public ActionResult StaffRAndRLeaveRequestCreate()
        {
            //if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeave.Create, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            StaffRAndRLeaveRequestModel model = new StaffRAndRLeaveRequestModel();
            var staffCore = DbAHD.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();


            model.RestAndRecuperationLeaveGUID = Guid.NewGuid();
            model.CurrentStep = 0;
            model.AccessLevel = 1;
            model.ReturnDateFromLastRLeave = staffCore.ReturnDateFromLastRAndRLeave;
            return View("~/Areas/AHD/Views/StaffRAndRLeaveRequest/RAndRLeaveRequest.cshtml", model);
        }

        [Route("AHD/StaffRAndRLeaveRequest/Update/{PK}")]
        public ActionResult StaffRAndRLeaveRequestUpdate(Guid PK)
        {

            var model = (from a in DbAHD.dataRestAndRecuperationRequest.WherePK(PK)
                             //join b in DbAHD.dataRestAndRecuperationRequestLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataRestAndRecuperationRequest.DeletedOn) && x.LanguageID == LAN)
                             //on a.RestAndRecuperationLeaveGUID equals b.RestAndRecuperationLeaveGUID into LJ1
                             //from R1 in LJ1.DefaultIfEmpty()
                         select new StaffRAndRLeaveRequestModel
                         {
                             RestAndRecuperationLeaveGUID = a.RestAndRecuperationLeaveGUID,
                             StaffGUID = a.StaffGUID.ToString(),
                             LeaveNumber = a.LeaveNumber,
                             StaffName = a.StaffName,
                             BackupArrangementGUID = (Guid)a.BackupArrangementGUID,

                             BackupArrangementName = a.BackupArrangementName,
                             ReturnDateFromLastRLeave = a.ReturnDateFromLastRLeave,
                             ExpiryDateOfResidency = a.ExpiryDateOfResidency,
                             LeaveInDate = a.LeaveInDate,
                             LeaveOutDate = a.LeaveOutDate,
                             ActualLeaveInDate = a.ActualLeaveInDate,
                             ActualLeaveOutDate = a.ActualLeaveOutDate,
                             HRComments = a.HRComments,
                             EmployeeComments = a.EmployeeComments,
                             DestinationCountry = a.DestinationCountry,
                             EligibleDate = a.EligibleDate,
                             PT8Number = a.PT8Number,
                             CurrentStep = a.CurrentStep,


                             Active = a.Active,
                             dataRestAndRecuperationRequestRowVersion = a.dataRestAndRecuperationRequestRowVersion,

                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("StaffRAndRLeaveRequestCreate", "StaffRAndRLeave", new { Area = "AHD" }));

            return View("~/Areas/AHD/Views/StaffRAndRLeaveRequest/RAndRLeaveRequest.cshtml", model);
        }
        public ActionResult Test(Guid? RestAndRecuperationLeaveGUID, DateTime? ReturnDateFromLastRLeave, Guid? BackupArrangementGUID, DateTime? ExpiryDateOfResidency, string EmployeeComments, string DestinationCountry)
        {
            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult StaffRAndRLeaveRequestCreate(Guid? RestAndRecuperationLeaveGUID, DateTime? ReturnDateFromLastRLeave, Guid? BackupArrangementGUID,
                                                         DateTime? ExpiryDateOfResidency, string EmployeeComments, string DestinationCountry, Guid? CountryGUID,
                                                         Guid? _DeparturePointGUID,
                                                         Guid? _DropOffPointGUID,
                                                         Guid? _StartLocationGUID,
                                                         Guid? _EndLocationGUID,
                                                         Guid _MySupervisorGUID
                                                         )
        {

            var temps = DbAHD.dataTempRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == RestAndRecuperationLeaveGUID).ToList();
            var lastleave = temps.OrderByDescending(x => x.TravelTimeOut).FirstOrDefault();
            var firstLeave = temps.OrderBy(x => x.TravelTimeIn).FirstOrDefault();
            //var reportToGUID = DbAHD.StaffCoreData.Where(x => x.UserGUID == UserGUID).Select(x => x.ReportToGUID).FirstOrDefault();
            if ((lastleave.LeaveTypeGUID != coddeInternationalStaffAttendanceTypeAttendanceTable.TravelTime))
            {
                return Json(new { success = -3 }, JsonRequestBehavior.AllowGet);
            }
            if ((firstLeave.LeaveTypeGUID != coddeInternationalStaffAttendanceTypeAttendanceTable.TravelTime))
            {
                return Json(new { success = -4 }, JsonRequestBehavior.AllowGet);
            }
            if (ReturnDateFromLastRLeave == null || ExpiryDateOfResidency == null || temps.Count == 0 || CountryGUID == null || _MySupervisorGUID == null)
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
                return Json(DbAHD.PermissionError());
            }




            // if (model.WarehouseItemDescription == null || ActiveItem(model)) return PartialView("~/Areas/AHD/Views/Items/_ItemForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();
            var staffCore = DbAHD.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            var staffPers = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == UserGUID && x.LanguageID == LAN).FirstOrDefault();
            var codeTablesValue = DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN).AsQueryable();
            var codeJobTitles = DbCMS.codeJobTitlesLanguages.Where(x => x.LanguageID == LAN && x.Active).ToList();
            var codeDutyStations = DbCMS.codeDutyStationsLanguages.Where(x => x.LanguageID == LAN && x.Active).ToList();


            dataRestAndRecuperationRequest myModel = new dataRestAndRecuperationRequest
            {
                RestAndRecuperationLeaveGUID = (Guid)RestAndRecuperationLeaveGUID,
                ReturnDateFromLastRLeave = ReturnDateFromLastRLeave,
                BackupArrangementGUID = BackupArrangementGUID,
                ExpiryDateOfResidency = ExpiryDateOfResidency,
                CountryGUID = CountryGUID,
                EmployeeComments = EmployeeComments,
                DestinationCountry = DestinationCountry,
                DeparturePointGUID = _DeparturePointGUID,
                DropOffPointGUID = _DropOffPointGUID,
                StartLocationGUID = _StartLocationGUID,
                EndLocationGUID = _EndLocationGUID,

                CurrentStep = 1,
                StaffName = staffPers.FirstName + " " + staffPers.Surname,
                FullStaffInformation = "Staff Name :" + staffPers.FirstName + " " + staffPers.Surname + " /Grade :" + codeTablesValue.Where(x => x.ValueGUID == staffCore.StaffGradeGUID).Select(x => x.ValueDescription).FirstOrDefault() + " /Title :" + codeJobTitles.Where(x => x.JobTitleGUID == staffCore.JobTitleGUID).FirstOrDefault().JobTitleDescription + "/ Location :" + codeDutyStations.Where(x => x.DutyStationGUID == staffCore.DutyStationGUID).FirstOrDefault().DutyStationDescription,
                StaffJobTitle = codeJobTitles.Where(x => x.JobTitleGUID == staffCore.JobTitleGUID).FirstOrDefault().JobTitleDescription

            };
            // myModel.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;
            var userper = DbAHD.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.UserGUID == UserGUID).FirstOrDefault();
            var backup = DbAHD.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.UserGUID == BackupArrangementGUID).FirstOrDefault();
            myModel.StaffGUID = UserGUID;
            myModel.StaffName = userper.FirstName + " " + userper.Surname;
            var supervisor = DbAHD.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.UserGUID == _MySupervisorGUID).FirstOrDefault();
            myModel.SupervisorGUID = _MySupervisorGUID;
            myModel.SupervisorName = supervisor.FirstName + " " + supervisor.Surname;
            myModel.BackupArrangementName = backup.FirstName + " " + backup.Surname;




            if (temps.Count > 0)
            {
                var olddates = DbAHD.dataRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == RestAndRecuperationLeaveGUID).ToList();
                DbAHD.dataRestAndRecuperationRequestLeaveDate.RemoveRange(olddates);
                //List<dataMissionActionRequired> missionRequiredActions = new List<dataMissionActionRequired>();
                //List<dataMissionActionTaken> missionTakendActions = new List<dataMissionActionTaken>();
                foreach (var item in temps)
                {
                    dataRestAndRecuperationRequestLeaveDate myTemp = new dataRestAndRecuperationRequestLeaveDate
                    {
                        RestAndRecuperationRequestLeaveDateGUID = item.TempRestAndRecuperationRequestLeaveDateGUID,
                        RestAndRecuperationLeaveGUID = myModel.RestAndRecuperationLeaveGUID,
                        LeaveTypeGUID = item.LeaveTypeGUID,
                        LeaveTypeName = item.LeaveTypeName,
                        TravelTimeOut = item.TravelTimeOut,
                        TravelTimeIn = item.TravelTimeIn,
                        Comments = item.Comments
                    };
                    //missionRequiredActions.Add(myActionRequired);
                    DbAHD.CreateNoAudit(myTemp);


                }
            }
            Guid R_RGuid = coddeInternationalStaffAttendanceTypeAttendanceTable.RR;
            if (temps.Where(x => x.LeaveTypeGUID == R_RGuid).Select(x => x.TravelTimeOut).Max() != null)
            {
                staffCore.ReturnDateFromLastRAndRLeave = temps.OrderByDescending(x => x.TravelTimeOut).FirstOrDefault().TravelTimeOut;
            }

            DbAHD.Update(staffCore, Permissions.InternationalStaffRestAndRecuperationLeave.UpdateGuid, ExecutionTime, DbCMS);

            myModel.LeaveInDate = temps.Select(x => x.TravelTimeIn).Min();
            myModel.LeaveOutDate = temps.Select(x => x.TravelTimeOut).Max();
            myModel.SubmittedDate = ExecutionTime;
            myModel.FlowStatusGUID = InternationalStaffRAndRLeaveFlowStatus.PendingHRReview;

            myModel.LastFlowStatusName = "Pending HR Review";
            myModel.CreatedByGUID = UserGUID;
            int currYear = DateTime.Now.Year;
            int? codeNumber = DbAHD.dataRestAndRecuperationRequest.Where(x => x.YearNumber == currYear).Select(x => x.LeaveNumberCode).Max() + 1;
            if (codeNumber == null || codeNumber == 0)
                codeNumber = 1;

            myModel.LeaveNumber = DateTime.Now.Year + "/" + "SYR" + "/" + codeNumber;
            myModel.LeaveNumberCode = codeNumber;
            myModel.YearNumber = currYear;
            DbAHD.Create(myModel, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);

            dataRestAndRecuperationRequestLeaveFlow flow = new dataRestAndRecuperationRequestLeaveFlow
            {
                dataRestAndRecuperationRequestLeaveFlowGUID = Guid.NewGuid(),
                RestAndRecuperationLeaveGUID = myModel.RestAndRecuperationLeaveGUID,
                StaffGUID = UserGUID,
                FlowStatusGUID = InternationalStaffRAndRLeaveFlowStatus.PendingHRReview,
                ActionDate = ExecutionTime,
                IsLastAction = true




            };
            DbAHD.Create(flow, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);
            //dataRestAndRecuperationRequestLanguage Language = Mapper.Map(model, new dataRestAndRecuperationRequestLanguage());
            //Language.WarehouseItemLanguagGUID = EntityPK;
            //Language.RestAndRecuperationLeaveGUID = Item.RestAndRecuperationLeaveGUID;

            //DbAHD.Create(Language, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.InternationalTempStaffRAndRDatesDataTable, ControllerContext, "InternationalTempStaffRAndRDatesDataTable"));


            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.InternationalStaffRestAndRecuperationLeave.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("Create", "StaffRAndRLeave", new { Area = "AHD" })), Container = "ItemModelDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.InternationalStaffRestAndRecuperationLeave.Update, Apps.AHD), Container = "ItemModelDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.InternationalStaffRestAndRecuperationLeave.Delete, Apps.AHD), Container = "ItemModelDetailFormControls" });

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                SendHRApprovalReviewMail((Guid)RestAndRecuperationLeaveGUID);
                //return RedirectToAction("StaffLeavsIndex");

                //return RedirectToAction("StaffLeavsIndex");
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
                //return Json(DbAHD.SingleCreateMessage(DbAHD.PrimaryKeyControl(myModel), DbAHD.RowVersionControls(myModel, myModel), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }
        [HttpPost]
        public ActionResult HRReviewApprovalCreate(Guid? restAndRecuperationLeaveGUID, DateTime EligibleDate, Guid ApprovedByGUID)
        {
            var priFlow = DbAHD.dataRestAndRecuperationRequestLeaveFlow.Where(x => x.RestAndRecuperationLeaveGUID == restAndRecuperationLeaveGUID && x.IsLastAction == true).FirstOrDefault();
            if (priFlow.FlowStatusGUID == InternationalStaffRAndRLeaveFlowStatus.PendingRepresentativeApproval)
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);

            }
            //if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeave.Create, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            // if (model.WarehouseItemDescription == null || ActiveItem(model)) return PartialView("~/Areas/AHD/Views/Items/_ItemForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            var hrReview = DbAHD.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.UserGUID == UserGUID).FirstOrDefault();

            Guid EntityPK = Guid.NewGuid();
            var model = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == restAndRecuperationLeaveGUID).FirstOrDefault();
            model.LastFlowStatusName = "Pending Supervisor Approval";
            model.FlowStatusGUID = InternationalStaffRAndRLeaveFlowStatus.PendingSupervisorApproval;
            model.CurrentStep = 2;

            model.ReviewedByHRStaffGUID = UserGUID;
            model.ReviewedByHRStaffName = hrReview.FirstName + " " + hrReview.Surname;
            model.ReviewedDate = ExecutionTime;
            model.EligibleDate = EligibleDate;
            model.ApprovedByGUID = ApprovedByGUID;
            // myModel.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;

            priFlow.IsLastAction = false;
            DbAHD.Update(priFlow, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);
            DbAHD.Update(model, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);
            dataRestAndRecuperationRequestLeaveFlow flow = new dataRestAndRecuperationRequestLeaveFlow
            {
                dataRestAndRecuperationRequestLeaveFlowGUID = Guid.NewGuid(),
                RestAndRecuperationLeaveGUID = restAndRecuperationLeaveGUID,
                StaffGUID = UserGUID,
                FlowStatusGUID = InternationalStaffRAndRLeaveFlowStatus.PendingSupervisorApproval,
                IsLastAction = true,
                ActionDate = ExecutionTime,



            };
            DbAHD.Create(flow, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);
            //dataRestAndRecuperationRequestLanguage Language = Mapper.Map(model, new dataRestAndRecuperationRequestLanguage());
            //Language.WarehouseItemLanguagGUID = EntityPK;
            //Language.RestAndRecuperationLeaveGUID = Item.RestAndRecuperationLeaveGUID;

            //DbAHD.Create(Language, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.InternationalTempStaffRAndRDatesDataTable, ControllerContext, "InternationalTempStaffRAndRDatesDataTable"));


            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.InternationalStaffRestAndRecuperationLeave.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("Create", "StaffRAndRLeave", new { Area = "AHD" })), Container = "ItemModelDetailFormControls" });


            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                SendSupervisorApprovalReviewMail((Guid)restAndRecuperationLeaveGUID);
                //return RedirectToAction("StaffLeavsIndex");
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost]
        public ActionResult SuperVisorApprovalCreate(Guid? restAndRecuperationLeaveGUID, string SupervisorComments)
        {
            var priFlow = DbAHD.dataRestAndRecuperationRequestLeaveFlow.Where(x => x.RestAndRecuperationLeaveGUID == restAndRecuperationLeaveGUID && x.IsLastAction == true).FirstOrDefault();
            if (priFlow.FlowStatusGUID == InternationalStaffRAndRLeaveFlowStatus.PendingHRReview)
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);

            }
            var model = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == restAndRecuperationLeaveGUID).FirstOrDefault();
            if (model.SupervisorGUID != UserGUID)
            {
                return Json(DbAHD.PermissionError());
            }
            // if (model.WarehouseItemDescription == null || ActiveItem(model)) return PartialView("~/Areas/AHD/Views/Items/_ItemForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            var supervisor = DbAHD.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.UserGUID == UserGUID).FirstOrDefault();

            Guid EntityPK = Guid.NewGuid();

            model.LastFlowStatusName = "Pending Representative Approval";
            model.FlowStatusGUID = InternationalStaffRAndRLeaveFlowStatus.PendingRepresentativeApproval;
            model.SupervisorName = supervisor.FirstName + " " + supervisor.Surname;
            model.SupervisorGUID = UserGUID;
            model.SupervisorApprovedDate = ExecutionTime;
            model.CurrentStep = 4;
            model.SupervisorComments = SupervisorComments;

            // myModel.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;

            priFlow.IsLastAction = false;
            DbAHD.Update(priFlow, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);
            DbAHD.Update(model, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);
            dataRestAndRecuperationRequestLeaveFlow flow = new dataRestAndRecuperationRequestLeaveFlow
            {
                dataRestAndRecuperationRequestLeaveFlowGUID = Guid.NewGuid(),
                RestAndRecuperationLeaveGUID = restAndRecuperationLeaveGUID,
                StaffGUID = UserGUID,
                FlowStatusGUID = InternationalStaffRAndRLeaveFlowStatus.PendingRepresentativeApproval,
                IsLastAction = true,
                ActionDate = ExecutionTime,



            };
            DbAHD.Create(flow, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);
            //dataRestAndRecuperationRequestLanguage Language = Mapper.Map(model, new dataRestAndRecuperationRequestLanguage());
            //Language.WarehouseItemLanguagGUID = EntityPK;
            //Language.RestAndRecuperationLeaveGUID = Item.RestAndRecuperationLeaveGUID;

            //DbAHD.Create(Language, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.InternationalTempStaffRAndRDatesDataTable, ControllerContext, "InternationalTempStaffRAndRDatesDataTable"));


            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.InternationalStaffRestAndRecuperationLeave.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("Create", "StaffRAndRLeave", new { Area = "AHD" })), Container = "ItemModelDetailFormControls" });


            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                if (model.SupervisorGUID != model.ApprovedByGUID)
                {
                    SendHRApporvlConfirmationAndRepresentativeMail((Guid)restAndRecuperationLeaveGUID);
                }
                else if (model.SupervisorGUID == model.ApprovedByGUID)
                {
                    RepresentativeApproveRAndRLeaveCreate((Guid)restAndRecuperationLeaveGUID, "");
                }

                //return RedirectToAction("StaffLeavsIndex");
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost]
        public ActionResult SuperVisorRejectCreate(Guid? restAndRecuperationLeaveGUID, string SupervisorComments)
        {
            var priFlow = DbAHD.dataRestAndRecuperationRequestLeaveFlow.Where(x => x.RestAndRecuperationLeaveGUID == restAndRecuperationLeaveGUID && x.IsLastAction == true).FirstOrDefault();
            if (priFlow.FlowStatusGUID == InternationalStaffRAndRLeaveFlowStatus.PendingHRReview)
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);

            }
            var model = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == restAndRecuperationLeaveGUID).FirstOrDefault();
            if (model.SupervisorGUID != UserGUID)
            {
                return Json(DbAHD.PermissionError());
            }
            // if (model.WarehouseItemDescription == null || ActiveItem(model)) return PartialView("~/Areas/AHD/Views/Items/_ItemForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            var supervisor = DbAHD.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.UserGUID == UserGUID).FirstOrDefault();

            Guid EntityPK = Guid.NewGuid();

            model.LastFlowStatusName = "Rejected by Supervisor";
            model.FlowStatusGUID = InternationalStaffRAndRLeaveFlowStatus.RejectedBySupervior;
            model.SupervisorName = supervisor.FirstName + " " + supervisor.Surname;
            model.SupervisorGUID = UserGUID;
            model.SupervisorApprovedDate = ExecutionTime;
            model.CurrentStep = 3;
            model.SupervisorComments = SupervisorComments;

            // myModel.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;

            priFlow.IsLastAction = false;
            DbAHD.Update(priFlow, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);
            DbAHD.Update(model, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);
            dataRestAndRecuperationRequestLeaveFlow flow = new dataRestAndRecuperationRequestLeaveFlow
            {
                dataRestAndRecuperationRequestLeaveFlowGUID = Guid.NewGuid(),
                RestAndRecuperationLeaveGUID = restAndRecuperationLeaveGUID,
                StaffGUID = UserGUID,
                FlowStatusGUID = InternationalStaffRAndRLeaveFlowStatus.RejectedBySupervior,
                IsLastAction = true,
                ActionDate = ExecutionTime,



            };
            DbAHD.Create(flow, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);
            //dataRestAndRecuperationRequestLanguage Language = Mapper.Map(model, new dataRestAndRecuperationRequestLanguage());
            //Language.WarehouseItemLanguagGUID = EntityPK;
            //Language.RestAndRecuperationLeaveGUID = Item.RestAndRecuperationLeaveGUID;

            //DbAHD.Create(Language, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.InternationalTempStaffRAndRDatesDataTable, ControllerContext, "InternationalTempStaffRAndRDatesDataTable"));


            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.InternationalStaffRestAndRecuperationLeave.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("Create", "StaffRAndRLeave", new { Area = "AHD" })), Container = "ItemModelDetailFormControls" });


            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                SendSupervisorRejectR_R((Guid)restAndRecuperationLeaveGUID);
                //return RedirectToAction("StaffLeavsIndex");
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost]
        public ActionResult HRApprovalCreate(Guid? restAndRecuperationLeaveGUID)
        {
            var priFlow = DbAHD.dataRestAndRecuperationRequestLeaveFlow.Where(x => x.RestAndRecuperationLeaveGUID == restAndRecuperationLeaveGUID && x.IsLastAction == true).FirstOrDefault();
            if (priFlow.FlowStatusGUID == InternationalStaffRAndRLeaveFlowStatus.PendingSupervisorApproval)
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);

            }
            if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeave.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            // if (model.WarehouseItemDescription == null || ActiveItem(model)) return PartialView("~/Areas/AHD/Views/Items/_ItemForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            var hrUser = DbAHD.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.UserGUID == UserGUID).FirstOrDefault();

            Guid EntityPK = Guid.NewGuid();
            var model = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == restAndRecuperationLeaveGUID).FirstOrDefault();
            model.LastFlowStatusName = "Pending Representative Approval";
            model.FlowStatusGUID = InternationalStaffRAndRLeaveFlowStatus.PendingRepresentativeApproval;
            model.CurrentStep = 4;
            model.HRApprovedDate = ExecutionTime;
            model.ApprovedByHRStaffGUID = UserGUID;
            model.ApprovedByHRStaffName = hrUser.FirstName + " " + hrUser.Surname;

            // myModel.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;

            priFlow.IsLastAction = false;
            DbAHD.Update(priFlow, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);
            DbAHD.Update(model, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);
            dataRestAndRecuperationRequestLeaveFlow flow = new dataRestAndRecuperationRequestLeaveFlow
            {
                dataRestAndRecuperationRequestLeaveFlowGUID = Guid.NewGuid(),
                RestAndRecuperationLeaveGUID = restAndRecuperationLeaveGUID,
                StaffGUID = UserGUID,
                FlowStatusGUID = InternationalStaffRAndRLeaveFlowStatus.PendingRepresentativeApproval,
                IsLastAction = true,
                ActionDate = ExecutionTime,



            };
            DbAHD.Create(flow, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);

            var userperson = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == model.StaffGUID && x.LanguageID == LAN).FirstOrDefault();
            var toAddAttendace = DbAHD.dataRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == restAndRecuperationLeaveGUID).ToList();
            Guid r_rGUID = Guid.Parse("67979D6D-5B7B-4A85-AA6D-CD604A1BDF75");
            Guid annual_leave = Guid.Parse("13AFCA94-4FA0-479A-85DB-8AEF4BC64CBB");

            //attendaces 



            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.InternationalTempStaffRAndRDatesDataTable, ControllerContext, "InternationalTempStaffRAndRDatesDataTable"));


            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.InternationalStaffRestAndRecuperationLeave.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("Create", "StaffRAndRLeave", new { Area = "AHD" })), Container = "ItemModelDetailFormControls" });


            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                SendHRApporvlConfirmationAndRepresentativeMail((Guid)restAndRecuperationLeaveGUID);
                //return RedirectToAction("StaffLeavsIndex");
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }



        [HttpPost]
        public ActionResult RepresentativeApproveRAndRLeaveCreate(Guid? restAndRecuperationLeaveGUID, string RepresentativeComments)
        {

            var priFlow = DbAHD.dataRestAndRecuperationRequestLeaveFlow.Where(x => x.RestAndRecuperationLeaveGUID == restAndRecuperationLeaveGUID && x.IsLastAction == true).FirstOrDefault();
            if (priFlow.FlowStatusGUID != InternationalStaffRAndRLeaveFlowStatus.PendingRepresentativeApproval)
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);

            }
            if (!CMS.HasAction(Permissions.InternationalStaffR_RRepresentativeReview.Access, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }

            var olddates = DbAHD.dataRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == restAndRecuperationLeaveGUID).ToList();
            if (olddates.Count > 0)
            {
                DbAHD.dataRestAndRecuperationRequestLeaveDate.RemoveRange(olddates);
                DbAHD.SaveChanges();
            }
            var temps = DbAHD.dataTempRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == restAndRecuperationLeaveGUID).ToList();

            //var _checkLeaves = DbAHD.dataTempRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == restAndRecuperationLeaveGUID).ToList();
            if (temps.Count > 0)
            {
                List<dataRestAndRecuperationRequestLeaveDate> toAdd = new List<dataRestAndRecuperationRequestLeaveDate>();
                foreach (var item in temps.OrderBy(x => x.TravelTimeIn))
                {
                    dataRestAndRecuperationRequestLeaveDate myTemp = new dataRestAndRecuperationRequestLeaveDate
                    {
                        RestAndRecuperationRequestLeaveDateGUID = Guid.NewGuid(),
                        RestAndRecuperationLeaveGUID = item.RestAndRecuperationLeaveGUID,
                        LeaveTypeGUID = item.LeaveTypeGUID,
                        LeaveTypeName = item.LeaveTypeName,
                        TravelTimeOut = item.TravelTimeOut,
                        TravelTimeIn = item.TravelTimeIn,
                        Comments = item.Comments
                    };
                    toAdd.Add(myTemp);
                    //missionRequiredActions.Add(myActionRequired);




                }
                DbAHD.CreateBulkNoAudit(toAdd);
                DbAHD.SaveChanges();
            }

            // if (model.WarehouseItemDescription == null || ActiveItem(model)) return PartialView("~/Areas/AHD/Views/Items/_ItemForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            var hrUser = DbAHD.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.UserGUID == UserGUID).FirstOrDefault();

            Guid EntityPK = Guid.NewGuid();
            var model = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == restAndRecuperationLeaveGUID).FirstOrDefault();
            model.LastFlowStatusName = "Approved And Confirmed";

            model.FlowStatusGUID = InternationalStaffRAndRLeaveFlowStatus.Approved;
            model.CurrentStep = 5;

            model.ApprovedByGUID = UserGUID;
            model.ApprovedByName = hrUser.FirstName + " " + hrUser.Surname;
            model.RepresentativeComments = RepresentativeComments;
            model.RepresentativeDecisionDate = ExecutionTime;

            // myModel.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;

            priFlow.IsLastAction = false;
            DbAHD.Update(priFlow, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);
            DbAHD.Update(model, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);
            dataRestAndRecuperationRequestLeaveFlow flow = new dataRestAndRecuperationRequestLeaveFlow
            {
                dataRestAndRecuperationRequestLeaveFlowGUID = Guid.NewGuid(),
                RestAndRecuperationLeaveGUID = restAndRecuperationLeaveGUID,
                StaffGUID = UserGUID,
                FlowStatusGUID = InternationalStaffRAndRLeaveFlowStatus.Approved,
                IsLastAction = true,
                ActionDate = ExecutionTime,



            };
            DbAHD.Create(flow, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);

            var userperson = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == model.StaffGUID && x.LanguageID == LAN).FirstOrDefault();
            var toAddAttendace = DbAHD.dataRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == restAndRecuperationLeaveGUID).ToList();
            Guid r_rGUID = Guid.Parse("67979D6D-5B7B-4A85-AA6D-CD604A1BDF75");
            Guid annual_leave = Guid.Parse("13AFCA94-4FA0-479A-85DB-8AEF4BC64CBB");
            Guid _travelTime = Guid.Parse("a1a0a314-388c-4e21-ab91-b919439aa794");
            Guid _weekend = coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend;
            Guid _officialholiday = coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday;

            //attendaces 
            List<dataInternationalStaffAttendance> allAttendacnes = new List<dataInternationalStaffAttendance>();
            foreach (var item in toAddAttendace)
            {
                if (item.LeaveTypeGUID != Guid.Empty)
                {
                    dataInternationalStaffAttendance attendance = new dataInternationalStaffAttendance
                    {
                        InternationalStaffAttendanceGUID = Guid.NewGuid(),
                        InternationalStaffAttendanceTypeGUID = item.LeaveTypeGUID,
                        StaffGUID = model.StaffGUID,
                        StaffName = userperson.FirstName + " " + userperson.Surname,
                        LeaveLocation = item.dataRestAndRecuperationRequest.StaffLoction,
                        TotalDays = item.TravelTimeOut != null ? ((int)(item.TravelTimeOut - item.TravelTimeIn).Value.TotalDays + 1) : 1,
                        Comments = item.Comments,
                        IsAutomated = false,
                        FromDate = item.TravelTimeIn,
                        ToDate = item.TravelTimeOut,
                        CreatedByGUID = UserGUID,
                        CreateDate = ExecutionTime,
                        RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID,

                    };
                    allAttendacnes.Add(attendance);

                }

            }

            DbAHD.CreateBulk(allAttendacnes, Permissions.InternationalStaffRestAndRecuperationLeaveHRReview.CreateGuid, ExecutionTime, DbCMS);

            //Shuttle 
            dataShuttleRequest shuttleRequest = new dataShuttleRequest
            {
                ShuttleRequestGUID = Guid.NewGuid(),
                DepartureDate = (DateTime)toAddAttendace.Select(x => x.TravelTimeIn).Min(),
                ReturnDateTime = (DateTime)toAddAttendace.Select(x => x.TravelTimeIn).Max(),
                ReferralStatusGUID = Guid.Parse("A73E5015-896A-42CD-8845-D5FCE5B80E15"),
                UserGUID = UserGUID,
                ShuttleTravelPurposeGUID = Guid.Parse("D142182E-1C48-41FB-8B82-8BF330267F2F"),
                DeparturePointGUID = model.DeparturePointGUID,
                DropOffPointGUID = model.DropOffPointGUID,
                StartLocationGUID = model.StartLocationGUID,
                EndLocationGUID = model.EndLocationGUID,

                Active = true,


            };

            dataShuttleRequestStaff shuttleRequeststaff = new dataShuttleRequestStaff
            {
                ShuttleRequestStaffGUID = Guid.NewGuid(),
                ShuttleRequestGUID = shuttleRequest.ShuttleRequestGUID,
                ReferralStatusGUID = Guid.Parse("A73E5015-896A-42CD-8845-D5FCE5B80E15"),
                Active = true,




                UserGUID = UserGUID,



            };
            shuttleRequest.OrganizationInstanceGUID = Guid.Parse("E156C022-EC72-4A5A-BE09-163BD85C68EF");
            shuttleRequest.DutyStationGUID = DbAHD.StaffCoreData.Where(x => x.UserGUID == model.StaffGUID).FirstOrDefault().DutyStationGUID;
            DbAHD.Create(shuttleRequest, Permissions.InternationalStaffRestAndRecuperationLeaveHRReview.CreateGuid, ExecutionTime, DbCMS);
            DbAHD.Create(shuttleRequeststaff, Permissions.InternationalStaffRestAndRecuperationLeaveHRReview.CreateGuid, ExecutionTime, DbCMS);


            //dataRestAndRecuperationRequestLanguage Language = Mapper.Map(model, new dataRestAndRecuperationRequestLanguage());
            //Language.WarehouseItemLanguagGUID = EntityPK;
            //Language.RestAndRecuperationLeaveGUID = Item.RestAndRecuperationLeaveGUID;

            //DbAHD.Create(Language, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.InternationalTempStaffRAndRDatesDataTable, ControllerContext, "InternationalTempStaffRAndRDatesDataTable"));


            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.InternationalStaffRestAndRecuperationLeave.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("Create", "StaffRAndRLeave", new { Area = "AHD" })), Container = "ItemModelDetailFormControls" });


            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                //SendApprovalRepresentativeAndStaffConfirmationMail((Guid)restAndRecuperationLeaveGUID);
                //return RedirectToAction("StaffLeavsIndex");
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }


        [HttpPost]
        public ActionResult RepresentativeRejectRAndRLeaveCreate(Guid? restAndRecuperationLeaveGUID, string RepresentativeComments)
        {

            var priFlow = DbAHD.dataRestAndRecuperationRequestLeaveFlow.Where(x => x.RestAndRecuperationLeaveGUID == restAndRecuperationLeaveGUID && x.IsLastAction == true).FirstOrDefault();
            if (priFlow.FlowStatusGUID != InternationalStaffRAndRLeaveFlowStatus.PendingRepresentativeApproval)
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);

            }
            if (!CMS.HasAction(Permissions.InternationalStaffR_RRepresentativeReview.Access, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            // if (model.WarehouseItemDescription == null || ActiveItem(model)) return PartialView("~/Areas/AHD/Views/Items/_ItemForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            var hrUser = DbAHD.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.UserGUID == UserGUID).FirstOrDefault();

            Guid EntityPK = Guid.NewGuid();
            var model = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == restAndRecuperationLeaveGUID).FirstOrDefault();
            model.LastFlowStatusName = "Rejected by Representative";

            model.FlowStatusGUID = InternationalStaffRAndRLeaveFlowStatus.RejectedByRepresentative;
            model.CurrentStep = 5;

            model.ApprovedByGUID = UserGUID;
            model.ApprovedByName = hrUser.FirstName + " " + hrUser.Surname;
            model.RepresentativeComments = RepresentativeComments;
            model.RepresentativeDecisionDate = ExecutionTime;

            // myModel.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;

            priFlow.IsLastAction = false;
            DbAHD.Update(priFlow, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);
            DbAHD.Update(model, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);
            dataRestAndRecuperationRequestLeaveFlow flow = new dataRestAndRecuperationRequestLeaveFlow
            {
                dataRestAndRecuperationRequestLeaveFlowGUID = Guid.NewGuid(),
                RestAndRecuperationLeaveGUID = restAndRecuperationLeaveGUID,
                StaffGUID = UserGUID,
                FlowStatusGUID = InternationalStaffRAndRLeaveFlowStatus.RejectedByRepresentative,
                IsLastAction = true,
                ActionDate = ExecutionTime,



            };
            DbAHD.Create(flow, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);



            //attendaces 


            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                SendRepresentativeRejectR_R((Guid)restAndRecuperationLeaveGUID);
                //return RedirectToAction("StaffLeavsIndex");
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }




        [HttpPost]
        public ActionResult CloseRAndRLeaveCreate(Guid? restAndRecuperationLeaveGUID, Guid _travelPlanStatusGUID)
        {
            var priFlow = DbAHD.dataRestAndRecuperationRequestLeaveFlow.Where(x => x.RestAndRecuperationLeaveGUID == restAndRecuperationLeaveGUID && x.IsLastAction == true).FirstOrDefault();
            if (priFlow.FlowStatusGUID != InternationalStaffRAndRLeaveFlowStatus.Approved)
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);

            }
            if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeave.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            // if (model.WarehouseItemDescription == null || ActiveItem(model)) return PartialView("~/Areas/AHD/Views/Items/_ItemForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();
            var model = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == restAndRecuperationLeaveGUID).FirstOrDefault();
            model.LastFlowStatusName = "Closed";
            model.CurrentStep = 6;
            model.FlowStatusGUID = InternationalStaffRAndRLeaveFlowStatus.Closed;
            model.TravelPlanStatusGUID = _travelPlanStatusGUID;

            // myModel.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;

            priFlow.IsLastAction = false;
            DbAHD.Update(priFlow, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);
            dataRestAndRecuperationRequestLeaveFlow flow = new dataRestAndRecuperationRequestLeaveFlow
            {
                dataRestAndRecuperationRequestLeaveFlowGUID = Guid.NewGuid(),
                RestAndRecuperationLeaveGUID = restAndRecuperationLeaveGUID,
                StaffGUID = UserGUID,
                FlowStatusGUID = InternationalStaffRAndRLeaveFlowStatus.Closed,
                IsLastAction = true,
                ActionDate = ExecutionTime,



            };
            DbAHD.Create(flow, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);
            Guid Pending = Guid.Parse("a73e5015-896a-42cd-8845-d5fce5b80e15");
            Guid R_R = Guid.Parse("D142182E-1C48-41FB-8B82-8BF330267F2F");
            var allLeaveDates = DbAHD.dataRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID).ToList();

            var attendacnces = DbAHD.dataInternationalStaffAttendance.Where(x => x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID).ToList();


            //List<dataInternationalStaffAttendance> allAttendance = new List<dataInternationalStaffAttendance>();
            //List<dataInternationalStaffAttendance> attendacnesToAdd = (from x in allLeaveDates
            //                                               let Guid = Guid.NewGuid()
            //                                               select

            //          new dataInternationalStaffAttendance
            //          {
            //              InternationalStaffAttendanceGUID = Guid,
            //              //InternationalStaffAttendanceTypeGUID = ,
            //              FromDate = x.TravelTimeOut,
            //              ToDate = x.TravelTimeIn,


            //          }
            //    ).ToList();
            //DbAHD.dataInternationalStaffAttendance.AddRange(attendacnesToAdd);


            //dataShuttleRequest shuttleRequest =
            //    new dataShuttleRequest
            //    {
            //        ShuttleRequestGUID = Guid.NewGuid(),
            //        DepartureDate = (DateTime)model.LeaveInDate,
            //        ReturnDateTime = (DateTime)model.LeaveOutDate,
            //        ReferralStatusGUID = Pending,
            //        UserGUID = (Guid)model.StaffGUID,
            //        ShuttleTravelPurposeGUID = R_R


            //    };
            //DbAHD.Create(shuttleRequest, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);


            //dataRestAndRecuperationRequestLanguage Language = Mapper.Map(model, new dataRestAndRecuperationRequestLanguage());
            //Language.WarehouseItemLanguagGUID = EntityPK;
            //Language.RestAndRecuperationLeaveGUID = Item.RestAndRecuperationLeaveGUID;

            //DbAHD.Create(Language, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.InternationalTempStaffRAndRDatesDataTable, ControllerContext, "InternationalTempStaffRAndRDatesDataTable"));


            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.InternationalStaffRestAndRecuperationLeave.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("Create", "StaffRAndRLeave", new { Area = "AHD" })), Container = "ItemModelDetailFormControls" });


            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                SendSupervisorAndStaffLeaveReadyAndClosedMail((Guid)restAndRecuperationLeaveGUID);
                //return RedirectToAction("StaffLeavsIndex");
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }




        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffRAndRLeaveRequestUpdate(StaffRAndRLeaveRequestModel model)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeave.Update, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            // if (model.WarehouseItemDescription == null || ActiveItem(model)) return PartialView("~/Areas/AHD/Views/Items/_ItemForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataRestAndRecuperationRequest RestAndRecuperationRequest = Mapper.Map(model, new dataRestAndRecuperationRequest());

            DbAHD.Update(RestAndRecuperationRequest, Permissions.InternationalStaffRestAndRecuperationLeave.UpdateGuid, ExecutionTime, DbCMS);

            //var Language = DbAHD.dataRestAndRecuperationRequestLanguage.Where(l => l.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            //if (Language == null)
            //{
            //    Language = Mapper.Map(model, Language);
            //    Language.RestAndRecuperationLeaveGUID = ItemModel.RestAndRecuperationLeaveGUID;
            //    DbAHD.Create(Language, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);
            //}
            //else if (Language.WarehouseItemDescription != model.WarehouseItemDescription)
            //{
            //    Language = Mapper.Map(model, Language);
            //    DbAHD.Update(Language, Permissions.InternationalStaffRestAndRecuperationLeave.UpdateGuid, ExecutionTime, DbCMS);
            //}

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleUpdateMessage(null, null, DbAHD.RowVersionControls(RestAndRecuperationRequest, RestAndRecuperationRequest)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyItem((Guid)model.RestAndRecuperationLeaveGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }



        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffRAndRLeaveRequestDelete(dataRestAndRecuperationRequest model)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeave.Delete, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            List<dataRestAndRecuperationRequest> DeletedItemModel = DeleteItem(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.InternationalStaffRestAndRecuperationLeave.Restore, Apps.AHD), Container = "ItemFormControls" });

            try
            {
                int CommitedRows = DbAHD.SaveChanges();
                DbAHD.SaveChanges();
                return Json(DbAHD.SingleDeleteMessage(CommitedRows, DeletedItemModel.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyItem(model.RestAndRecuperationLeaveGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffRAndRLeaveRequestRestore(dataRestAndRecuperationRequest model)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeave.Restore, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            if (ActiveItem(model))
            {
                return Json(DbAHD.RecordExists());
            }

            List<dataRestAndRecuperationRequest> RestoredItemModel = RestoreItems(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.InternationalStaffRestAndRecuperationLeave.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("ItemModelCreate", "Configuration", new { Area = "AHD" })), Container = "ItemModelFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.InternationalStaffRestAndRecuperationLeave.Update, Apps.AHD), Container = "ItemModelFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.InternationalStaffRestAndRecuperationLeave.Delete, Apps.AHD), Container = "ItemModelFormControls" });

            try
            {
                int CommitedRows = DbAHD.SaveChanges();
                DbAHD.SaveChanges();
                return Json(DbAHD.SingleRestoreMessage(CommitedRows, RestoredItemModel, DbAHD.PrimaryKeyControl(RestoredItemModel.FirstOrDefault()), Url.Action(DataTableNames.ItemsDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyItem(model.RestAndRecuperationLeaveGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffRAndRLeaveRequestDataTableDelete(List<dataRestAndRecuperationRequest> models)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeave.Delete, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            List<dataRestAndRecuperationRequest> DeletedItem = DeleteItem(models);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialDeleteMessage(DeletedItem, models, DataTableNames.ItemsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffRAndRLeaveRequestDataTableRestore(List<dataRestAndRecuperationRequest> models)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeave.Restore, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            List<dataRestAndRecuperationRequest> RestoredItemModel = DeleteItem(models);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialRestoreMessage(RestoredItemModel, models, DataTableNames.ItemsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        private List<dataRestAndRecuperationRequest> DeleteItem(List<dataRestAndRecuperationRequest> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataRestAndRecuperationRequest> DeletedItemModel = new List<dataRestAndRecuperationRequest>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT WarehouseItemModelGUID,CONVERT(varchar(50), WarehouseItemModelGUID) as C2 ,dataRestAndRecuperationRequestModelRowVersion FROM code.dataRestAndRecuperationRequestModel where WarehouseItemModelGUID in (" + string.Join(",", models.Select(x => "'" + x.WarehouseItemModelGUID + "'").ToArray()) + ")";

            string query = DbAHD.QueryBuilder(models, Permissions.InternationalStaffRestAndRecuperationLeave.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbAHD.Database.SqlQuery<dataRestAndRecuperationRequest>(query).ToList();
            foreach (var record in Records)
            {
                DeletedItemModel.Add(DbAHD.Delete(record, ExecutionTime, Permissions.InternationalStaffRestAndRecuperationLeave.DeleteGuid, DbCMS));
            }

            //var Languages = DeletedItemModel.SelectMany(a => a.dataRestAndRecuperationRequestLanguage).Where(l => l.Active).ToList();
            //foreach (var language in Languages)
            //{
            //    DbAHD.Delete(language, ExecutionTime, Permissions.InternationalStaffRestAndRecuperationLeave.DeleteGuid, DbCMS);
            //}
            return DeletedItemModel;
        }

        private List<dataRestAndRecuperationRequest> RestoreItems(List<dataRestAndRecuperationRequest> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataRestAndRecuperationRequest> RestoredItem = new List<dataRestAndRecuperationRequest>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT WarehouseItemModelGUID,CONVERT(varchar(50), WarehouseItemModelGUID) as C2 ,dataRestAndRecuperationRequestModelRowVersion FROM code.dataRestAndRecuperationRequestModel where WarehouseItemModelGUID in (" + string.Join(",", models.Select(x => "'" + x.WarehouseItemModelGUID + "'").ToArray()) + ")";

            string query = DbAHD.QueryBuilder(models, Permissions.InternationalStaffRestAndRecuperationLeave.DeleteGuid, SubmitTypes.Restore, "");

            var Records = DbAHD.Database.SqlQuery<dataRestAndRecuperationRequest>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveItem(record))
                {
                    RestoredItem.Add(DbAHD.Restore(record, Permissions.InternationalStaffRestAndRecuperationLeave.DeleteGuid, Permissions.InternationalStaffRestAndRecuperationLeave.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            //var Languages = RestoredItem.SelectMany(x => x.dataRestAndRecuperationRequestLanguage.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            //foreach (var language in Languages)
            //{
            //    DbAHD.Restore(language, Permissions.InternationalStaffRestAndRecuperationLeave.DeleteGuid, Permissions.InternationalStaffRestAndRecuperationLeave.RestoreGuid, RestoringTime, DbCMS);
            //}

            return RestoredItem;
        }

        private JsonResult ConcurrencyItem(Guid PK)
        {
            StaffRAndRLeaveRequestModel dbModel = new StaffRAndRLeaveRequestModel();

            var ItemModel = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == PK).FirstOrDefault();
            var dbItemModel = DbAHD.Entry(ItemModel).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbItemModel, dbModel);

            //var Language = DbAHD.dataRestAndRecuperationRequestLanguage.Where(x => x.RestAndRecuperationLeaveGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataRestAndRecuperationRequest.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            //var dbLanguage = DbAHD.Entry(Language).GetDatabaseValues().ToObject();
            //dbModel = Mapper.Map(dbLanguage, dbModel);

            if (ItemModel.dataRestAndRecuperationRequestRowVersion.SequenceEqual(dbModel.dataRestAndRecuperationRequestRowVersion))
            {
                return Json(DbAHD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbAHD, dbModel, "LanguagesContainer"));
        }

        private bool ActiveItem(Object model)
        {
            dataRestAndRecuperationRequest ItemModel = Mapper.Map(model, new dataRestAndRecuperationRequest());
            int ModelDescription = DbAHD.dataRestAndRecuperationRequest
                                    .Where(x => x.StaffGUID == ItemModel.StaffGUID &&
                                                x.LeaveInDate == x.LeaveInDate &&
                                                x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("Leave", "Request Already Exist");
            }
            return (ModelDescription > 0);
        }

        #endregion

        #region Mails
        public void SendHRApprovalReviewMail(Guid id)
        {
            string URL = "";
            string Anchor = "";
            string Link = "";
            var myRequest = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == id).FirstOrDefault();
            Guid actionGUID = Guid.Parse("6102e16b-e9b5-4e6f-ae65-7d2e994e64b7");

            var userProfilesGUIDs = DbCMS.userPermissions.Where(x => x.ActionGUID == actionGUID && x.Active == true).Select(x => x.UserProfileGUID).ToList();
            var userGUids = DbCMS.userProfiles.Where(x => userProfilesGUIDs.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

            var allUsers = DbAHD.userPersonalDetailsLanguage.Where(x => userGUids.Contains(x.UserGUID)
                                                                          && x.LanguageID == LAN).ToList();
            var alluserAccounts = DbAHD.userServiceHistory.Where(x => userGUids.Contains(x.UserGUID)).ToList();


            string SubjectMessage = "R And R Leave Review";



            //foreach (var user in allUsers)
            //{
            //    //var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();

            //}

            URL = AppSettingsKeys.Domain + "/AHD/StaffRAndRLeave/ConfirmHRApproval/?PK=" + new Portal().GUIDToString(id);
            Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickToConfirm + "</a>";
            Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
            //string myFirstName = user.FirstName;
            //string mySurName = user.Surname;

            //staff to HR
            string _message = resxEmails.R_RHRToReview
                //.Replace("$FullName", user.FirstName + " " + user.Surname)
                .Replace("$RequestedBy", myRequest.StaffName)
                .Replace("$VerifyLink", Anchor)
               ;
            if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
            int isRec = 1;
            var _staff = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == myRequest.StaffGUID).FirstOrDefault();
            var myEmails = alluserAccounts.Select(x => x.EmailAddress).Distinct().ToList();
            string copyEmails = string.Join(" ;", myEmails);
            SendCopy(copyEmails, SubjectMessage, _message, isRec, _staff.EmailAddress);

            //################ Send for staff 
            //send for staff 




            //string _message1 = resxEmails.R_RInformStaffBySupervisorApproval
            //    .Replace("$FullName", myRequest.StaffName)
            //    .Replace("$leaveNumber", myRequest.LeaveNumber);
            //if (LAN == "AR") { _message1 = "<p align='right'>" + _message1 + "</p>"; }
            //int isRec1 = 1;
            //var myEmail1 = alluserAccounts.Where(x => x.UserGUID == myRequest.CreatedByGUID).Select(x => x.EmailAddress).FirstOrDefault();
            //Send(myEmail1, SubjectMessage, _message1, isRec1);



        }

        public void SendSupervisorRejectR_R(Guid id)
        {
            string URL = "";
            string Anchor = "";
            string Link = "";
            var myRequest = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == id).FirstOrDefault();
            Guid actionGUID = Guid.Parse("6102e16b-e9b5-4e6f-ae65-7d2e994e64b7");

            var userProfilesGUIDs = DbCMS.userPermissions.Where(x => x.ActionGUID == actionGUID && x.Active == true).Select(x => x.UserProfileGUID).ToList();
            var userGUids = DbCMS.userProfiles.Where(x => userProfilesGUIDs.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

            var allUsers = DbAHD.userPersonalDetailsLanguage.Where(x => userGUids.Contains(x.UserGUID)
                                                                          && x.LanguageID == LAN).ToList();
            var alluserAccounts = DbAHD.userServiceHistory.Where(x => userGUids.Contains(x.UserGUID)).ToList();


            string SubjectMessage = "R And R Leave Reject";



            //foreach (var user in allUsers)
            //{
            //    //var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();

            //}


            //string myFirstName = user.FirstName;
            //string mySurName = user.Surname;
            var _staff = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == myRequest.StaffGUID).FirstOrDefault();

            string _message = resxEmails.R_RSupervisorReviewRejected
                .Replace("$FullName", _staff.FullName)
                .Replace("$RequestedBy", myRequest.StaffName)
                .Replace("$leaveNumber", myRequest.LeaveNumber)
                .Replace("$RejectReason", myRequest.SupervisorComments)
                .Replace("$RejecterName", myRequest.SupervisorName)
               ;
            if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
            int isRec = 1;
            //var myEmail = alluserAccounts.Where(x => x.UserGUID == user.UserGUID).Select(x => x.EmailAddress).FirstOrDefault();

            var myEmails = alluserAccounts.Select(x => x.EmailAddress).Distinct().ToList();
            var _supervisor = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == myRequest.SupervisorGUID).FirstOrDefault();
            string copyEmails = string.Join(_supervisor.EmailAddress, myEmails);

            SendCopy(_staff.EmailAddress, SubjectMessage, _message, isRec, copyEmails);

            //################ Send for staff 
            //send for staff 




            //string _message1 = resxEmails.R_RInformStaffBySupervisorApproval
            //    .Replace("$FullName", myRequest.StaffName)
            //    .Replace("$leaveNumber", myRequest.LeaveNumber);
            //if (LAN == "AR") { _message1 = "<p align='right'>" + _message1 + "</p>"; }
            //int isRec1 = 1;
            //var myEmail1 = alluserAccounts.Where(x => x.UserGUID == myRequest.CreatedByGUID).Select(x => x.EmailAddress).FirstOrDefault();
            //Send(myEmail1, SubjectMessage, _message1, isRec1);



        }

        public void SendRepresentativeRejectR_R(Guid id)
        {
            string URL = "";
            string Anchor = "";
            string Link = "";
            var myRequest = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == id).FirstOrDefault();
            Guid actionGUID = Guid.Parse("6102e16b-e9b5-4e6f-ae65-7d2e994e64b7");

            var userProfilesGUIDs = DbCMS.userPermissions.Where(x => x.ActionGUID == actionGUID && x.Active == true).Select(x => x.UserProfileGUID).ToList();
            var userGUids = DbCMS.userProfiles.Where(x => userProfilesGUIDs.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

            var allUsers = DbAHD.userPersonalDetailsLanguage.Where(x => userGUids.Contains(x.UserGUID)
                                                                          && x.LanguageID == LAN).ToList();
            var alluserAccounts = DbAHD.userServiceHistory.Where(x => userGUids.Contains(x.UserGUID)).ToList();


            string SubjectMessage = "R And R Leave Reject";



            //foreach (var user in allUsers)
            //{
            //    //var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();


            //}


            //string myFirstName = user.FirstName;
            //string mySurName = user.Surname;
            var _staff = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == myRequest.StaffGUID).FirstOrDefault();
            var _super = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == myRequest.SupervisorGUID).FirstOrDefault();
            var _rep = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == myRequest.ApprovedByGUID).FirstOrDefault();

            var myEmails = alluserAccounts.Select(x => x.EmailAddress).Distinct().ToList();



            string _message = resxEmails.R_RSupervisorReviewRejected
                .Replace("$FullName", _staff.FullName)
                .Replace("$RequestedBy", myRequest.StaffName)
                .Replace("$leaveNumber", myRequest.LeaveNumber)
                .Replace("$RejecterName", myRequest.ApprovedByName)
                .Replace("$RejectReason", myRequest.RepresentativeComments)
               ;
            if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
            int isRec = 1;
            string copyEmails = string.Join(";", _super.EmailAddress, _staff.EmailAddress, myEmails);
            SendCopy(_staff.EmailAddress, SubjectMessage, _message, isRec, copyEmails);
            //################ Send for staff 
            //send for staff 




            //string _message1 = resxEmails.R_RInformStaffBySupervisorApproval
            //    .Replace("$FullName", myRequest.StaffName)
            //    .Replace("$leaveNumber", myRequest.LeaveNumber);
            //if (LAN == "AR") { _message1 = "<p align='right'>" + _message1 + "</p>"; }
            //int isRec1 = 1;
            //var myEmail1 = alluserAccounts.Where(x => x.UserGUID == myRequest.CreatedByGUID).Select(x => x.EmailAddress).FirstOrDefault();
            //Send(myEmail1, SubjectMessage, _message1, isRec1);



        }

        public void SendSupervisorApprovalReviewMail(Guid id)
        {
            string URL = "";
            string Anchor = "";
            string Link = "";
            var myRequest = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == id).FirstOrDefault();
            //Guid myActionCategoryGUID = Guid.Parse("B123F453-79A1-4157-83BC-651094C80915");
            Guid _permissionGUID = Guid.Parse("DE411F27-0EA8-49FB-964D-00833BB012F8");
            var tempPermGUIDs = DbCMS.userPermissions.Where(x => (x.ActionGUID == _permissionGUID && x.Active == true
                         ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();

            var _userGuids = DbCMS.userProfiles.Where(x => tempPermGUIDs.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

            var _backupUsers = DbCMS.userServiceHistory.Where(x => _userGuids.Contains(x.UserGUID)).Select(x => x.EmailAddress).Distinct().ToList();





            //var reportGUID = DbCMS.StaffCoreData.Where(x => x.UserGUID == myRequest.StaffGUID).Select(x => x.ReportToGUID).FirstOrDefault();

            var allUsers = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == myRequest.SupervisorGUID
                                                                          && x.LanguageID == LAN).ToList();
            var alluserAccounts = DbAHD.userServiceHistory.Where(x => x.UserGUID == myRequest.SupervisorGUID).FirstOrDefault();


            string SubjectMessage = "R And R Leave Review";



            //foreach (var user in allUsers)
            //{
            //    //var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();

            //}
            URL = AppSettingsKeys.Domain + "/AHD/StaffRAndRLeave/ConfirmHRApproval/?PK=" + new Portal().GUIDToString(id);
            Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickToConfirm + "</a>";
            Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
            //string myFirstName = user.FirstName;
            //string mySurName = user.Surname;
            ////f
            ///
            var _staff = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == myRequest.StaffGUID).FirstOrDefault();
            var _super = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == myRequest.SupervisorGUID).FirstOrDefault();
            var myEmails = alluserAccounts.EmailAddress.Distinct();
            _backupUsers.Add(_staff.EmailAddress);
            string copyEmails = string.Join(" ;", _backupUsers);
            //fffffffff
            //string copyEmails = string.Join(_staff.EmailAddress," ;" , myEmails);
            //f

            string _message = resxEmails.R_RSupervisorReveiwEmail
                .Replace("$FullName", _super.FullName)
                .Replace("$RequestedBy", myRequest.StaffName)
                .Replace("$VerifyLink", Anchor)
               ;
            if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
            int isRec = 1;
            //var myEmail = alluserAccounts.Where(x => x.UserGUID == user.UserGUID).Select(x => x.EmailAddress).FirstOrDefault();
            SendCopy(_super.EmailAddress, SubjectMessage, _message, isRec, copyEmails);
            //send to staff 

            //string _message1 = resxEmails.R_RInformUserByHRReview
            //    .Replace("$FullName", myRequest.StaffName)
            //    .Replace("$leaveNumber", myRequest.LeaveNumber);
            //if (LAN == "AR") { _message1 = "<p align='right'>" + _message1 + "</p>"; }
            //int isRec1 = 1;
            //var myEmail1 = alluserAccounts.Where(x => x.UserGUID == myRequest.CreatedByGUID).Select(x => x.EmailAddress).FirstOrDefault();
            //Send(myEmail1, SubjectMessage, _message1, isRec1);


        }



        public void SendHRApporvlConfirmationAndRepresentativeMail(Guid id)
        {
            string URL = "";
            string Anchor = "";
            string Link = "";
            var myRequest = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == id).FirstOrDefault();
            Guid _permissionGUID = Guid.Parse("DE411F27-0EA8-49FB-964D-00833BB012F8");
            var tempPermGUIDs = DbCMS.userPermissions.Where(x => (x.ActionGUID == _permissionGUID && x.Active == true
                         ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();

            var _userGuids = DbCMS.userProfiles.Where(x => tempPermGUIDs.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

            //var _backupUsers = DbCMS.userServiceHistory.Where(x => _userGuids.Contains(x.UserGUID)).Select(x => x.EmailAddress).Distinct().ToList();




            //var reportGUID = DbCMS.StaffCoreData.Where(x => x.UserGUID == myRequest.StaffGUID).Select(x => x.ReportToGUID).FirstOrDefault();

            var allUsers = DbAHD.userPersonalDetailsLanguage.Where(x => (x.UserGUID == myRequest.SupervisorGUID ||
                                                 x.UserGUID == myRequest.StaffGUID ||
                                                 x.UserGUID == myRequest.CreatedByGUID
                                           || _userGuids.Contains(x.UserGUID)) && x.LanguageID == LAN).Select(x => x.UserGUID).Distinct().ToList();
            var _backupUsersEmails = DbCMS.userServiceHistory.Where(x => allUsers.Contains(x.UserGUID)).Select(x => x.EmailAddress).Distinct().ToList();



            //var alluserAccounts = DbAHD.userServiceHistory.Where(x => x.UserGUID == myRequest.SupervisorGUID).ToList();

            //f
            string SubjectMessage = "R And R Leave Review";




            var allUsersRep = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == myRequest.ApprovedByGUID
                                                                     && x.LanguageID == LAN).ToList();




            var _staff = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == myRequest.StaffGUID).FirstOrDefault();
            var _super = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == myRequest.SupervisorGUID).FirstOrDefault();
            var _rep = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == myRequest.ApprovedByGUID).FirstOrDefault();
            //var myEmails = alluserAccounts.Select(x => x.EmailAddress).Distinct().ToList();
            // string copyEmails = string.Join(" ;", myEmails);


            URL = AppSettingsKeys.Domain + "/AHD/StaffRAndRLeave/ConfirmHRApproval/?PK=" + new Portal().GUIDToString(id);
            Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickToConfirm + "</a>";
            Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";

            //f

            string _message = resxEmails.R_RSupervisorReveiwEmail
                .Replace("$FullName", _rep.FullName)
                .Replace("$RequestedBy", myRequest.StaffName)
                .Replace("$VerifyLink", Anchor)
               ;
            if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
            int isRec = 1;
            string copyEmails = string.Join(";", _backupUsersEmails);
            // var myEmail = alluserAccounts.Where(x => x.UserGUID == rep.UserGUID).Select(x => x.EmailAddress).FirstOrDefault();
            SendCopy(_rep.EmailAddress, SubjectMessage, _message, isRec, copyEmails);



        }

        public void SendApprovalRepresentativeAndStaffConfirmationMail(Guid id)
        {
            string URL = "";
            string Anchor = "";
            string Link = "";

            var myRequest = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == id).FirstOrDefault();
            Guid _permissionGUID = Guid.Parse("DE411F27-0EA8-49FB-964D-00833BB012F8");
            Guid _adminGUID = Guid.Parse("4EDD5DBB-3348-4C58-B246-0CFB46D8B207");
            Guid _shuttleGuid = Guid.Parse("2F1BB65B-ED02-4CCA-AE93-477BB8C329F2");
            Guid _depAdmin = Guid.Parse("4778E122-8E60-4ACE-866E-327A505F7B5A");
            var _adminUsers = DbCMS.StaffCoreData.Where(x => x.DepartmentGUID == _depAdmin).Select(x => x.UserGUID).Distinct();

            var _adminServiceGUID = DbCMS.userServiceHistory.Where(x => _adminUsers.Contains(x.UserGUID)).Select(x => x.ServiceHistoryGUID).ToList();
            var _depProfileAdminGUID = DbCMS.userProfiles.Where(x => _adminServiceGUID.Contains(x.ServiceHistoryGUID)).
                Select(x => x.UserProfileGUID).Distinct();



            var tempPermGUIDs = DbCMS.userPermissions.Where(x => ((x.ActionGUID == _permissionGUID || x.ActionGUID == _adminGUID ||
                                    (x.ActionGUID == _shuttleGuid && _depProfileAdminGUID.Contains(x.UserProfileGUID))
                                    ) && x.Active == true
                                                 ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();

            var _userGuids = DbCMS.userProfiles.Where(x => tempPermGUIDs.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

            var allUsers = DbCMS.userPersonalDetailsLanguage.Where(x => (x.UserGUID == myRequest.SupervisorGUID ||
                                                 x.UserGUID == myRequest.ApprovedByGUID ||
                                                 x.UserGUID == myRequest.CreatedByGUID
                                           || _userGuids.Contains(x.UserGUID)) && x.LanguageID == LAN).Select(x => x.UserGUID).Distinct().ToList();
            var _backupUsersEmails = DbCMS.userServiceHistory.Where(x => allUsers.Contains(x.UserGUID)).Select(x => x.EmailAddress).Distinct().ToList();

            string SubjectMessage = "R And R Leave Approval";




            var _staff = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == myRequest.StaffGUID).FirstOrDefault();


            string copyEmails = string.Join(";", _backupUsersEmails);
            string _message = resxEmails.R_RLeaveRepresentativeFinalApproval
                .Replace("$FullName", _staff.FullName)
                .Replace("$RequestedBy", myRequest.StaffName)

                .Replace("$LeaveNumber", myRequest.LeaveNumber)
               ;
            if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
            int isRec = 1;

            SendCopy(_staff.EmailAddress, SubjectMessage, _message, isRec, copyEmails);



        }

        public void SendSupervisorAndStaffLeaveReadyAndClosedMail(Guid id)
        {
            string URL = "";
            string Anchor = "";
            string Link = "";
            var myRequest = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == id).FirstOrDefault();
            Guid _permissionGUID = Guid.Parse("DE411F27-0EA8-49FB-964D-00833BB012F8");
            var tempPermGUIDs = DbCMS.userPermissions.Where(x => (x.ActionGUID == _permissionGUID && x.Active == true
                         ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();

            var _userGuids = DbCMS.userProfiles.Where(x => tempPermGUIDs.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

            var allUsers = DbAHD.userPersonalDetailsLanguage.Where(x => (x.UserGUID == myRequest.SupervisorGUID ||
                                                 x.UserGUID == myRequest.ApprovedByGUID ||
                                                 x.UserGUID == myRequest.StaffGUID ||
                                                 x.UserGUID == myRequest.CreatedByGUID
                                           || _userGuids.Contains(x.UserGUID)) && x.LanguageID == LAN).Select(x => x.UserGUID).Distinct().ToList();
            var _backupUsersEmails = DbCMS.userServiceHistory.Where(x => allUsers.Contains(x.UserGUID)).Select(x => x.EmailAddress).Distinct().ToList();

            string SubjectMessage = "R And R Leave Review";


            URL = AppSettingsKeys.Domain + "/AHD/StaffRAndRLeave/ConfirmHRApproval/?PK=" + new Portal().GUIDToString(id);
            Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickToConfirm + "</a>";
            Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";

            var _staff = DbCMS.v_StaffProfileInformation.Where(x => x.UserGUID == myRequest.StaffGUID).FirstOrDefault();
            //var myEmails = alluserAccounts.Select(x => x.EmailAddress).Distinct().ToList();
            string copyEmails = string.Join(" ;", _backupUsersEmails);


            string _message = resxEmails.R_RLeaveInformStaffToClose
                .Replace("$FullName", _staff.FullName)
                .Replace("$RequestedBy", myRequest.StaffName)
                .Replace("$VerifyLink", Anchor)
                .Replace("$LeaveNumber", myRequest.LeaveNumber)
               ;
            if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
            int isRec = 1;
            //var myEmail = alluserAccounts.Where(x => x.UserGUID == user.UserGUID).Select(x => x.EmailAddress).FirstOrDefault();
            SendCopy(_staff.EmailAddress, SubjectMessage, _message, isRec, copyEmails);


        }
        //public void Send(string recipients, string subject, string body, int? isRec)
        //{
        //    string copy_recipients = "";
        //    string blind_copy_recipients = null;
        //    string body_format = "HTML";
        //    string importance = "Normal";
        //    string file_attachments = null;
        //    string _body = "<div style='font-family:Arial;'>" + body.Replace("\r\n", "<br/>") + "</div>";
        //    if (LAN == "AR") { _body = "<p align='right'>" + _body + "</p>"; }
        //   DbAHD.SendEmailHR("maksoud@unhcr.org", "", blind_copy_recipients, subject, _body, body_format, importance, file_attachments);
        //  //  DbCMS.SendEmailHR(recipients, copy_recipients, blind_copy_recipients, subject, _body, body_format, importance, file_attachments);
        //}
        public void SendCopy(string recipients, string subject, string body, int? isRec, string copy_recipients)
        {

            string blind_copy_recipients = null;
            string body_format = "HTML";
            string importance = "Normal";
            string file_attachments = null;
            string _body = "<div style='font-family:Arial;'>" + body.Replace("\r\n", "<br/>") + "</div>";
            if (LAN == "AR") { _body = "<p align='right'>" + _body + "</p>"; }
            //DbAHD.SendEmailHR("maksoud@unhcr.org", "", blind_copy_recipients, subject, _body, body_format, importance, file_attachments);
           DbCMS.SendEmailHR(recipients, copy_recipients, blind_copy_recipients, subject, _body, body_format, importance, file_attachments);
        }
        public ActionResult ConfirmHRApproval(Guid PK)
        {
            Guid R_R = coddeInternationalStaffAttendanceTypeAttendanceTable.RR;
            Guid annual_leave = Guid.Parse("13AFCA94-4FA0-479A-85DB-8AEF4BC64CBB");

            var model = (from a in DbAHD.dataRestAndRecuperationRequest.WherePK(PK)
                         join b in DbAHD.codeCountriesLanguages.Where(x => (x.Active == true) && x.LanguageID == LAN) on a.CountryGUID equals b.CountryGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()

                         join c in DbAHD.v_StaffProfileInformation on a.SupervisorGUID equals c.UserGUID into LJ2
                         from R2 in LJ2.DefaultIfEmpty()

                         join d in DbAHD.v_StaffProfileInformation on a.ApprovedByGUID equals d.UserGUID into LJ3
                         from R3 in LJ3.DefaultIfEmpty()
                         select new StaffRAndRLeaveRequestModel
                         {
                             RestAndRecuperationLeaveGUID = a.RestAndRecuperationLeaveGUID,
                             StaffGUID = a.StaffGUID.ToString(),
                             LeaveNumber = a.LeaveNumber,
                             BackupArrangementGUID = (Guid)a.BackupArrangementGUID,
                             FlowStatusGUID = a.FlowStatusGUID,
                             LastFlowStatusName = a.LastFlowStatusName,
                             SubmittedDate = a.SubmittedDate,
                             StaffName = a.StaffName,
                             BackupArrangementName = a.BackupArrangementName,
                             ReturnDateFromLastRLeave = a.ReturnDateFromLastRLeave,
                             ExpiryDateOfResidency = a.ExpiryDateOfResidency,
                             LeaveInDate = a.LeaveInDate,
                             LeaveInDateStringFormat = a.LeaveInDate.ToString(),
                             LeaveOutDateStringFormat = a.LeaveOutDate.ToString(),
                             HRReviewDateFormat = a.ReviewedDate.ToString(),
                             SupervisorApprovedDateFormat = a.SupervisorApprovedDate.ToString(),
                             DeparturePointGUID = a.DeparturePointGUID,
                             DropOffPointGUID = a.DropOffPointGUID,
                             StartLocationGUID = a.StartLocationGUID,
                             EndLocationGUID = a.EndLocationGUID,
                             TravelLocaionDescription = a.TravelLocaionDescription,


                             DeparturePoint = a.DeparturePointGUID != null ? DbAHD.codeLocationsLanguages.Where(x => x.LocationGUID == a.DeparturePointGUID && x.LanguageID == LAN).FirstOrDefault().LocationDescription : "",
                             DropOffPoint = a.DropOffPointGUID != null ? DbAHD.codeLocationsLanguages.Where(x => x.LocationGUID == a.DropOffPointGUID && x.LanguageID == LAN).FirstOrDefault().LocationDescription : "",
                             StartLocation = a.StartLocationGUID != null ? DbAHD.codeLocationsLanguages.Where(x => x.LocationGUID == a.StartLocationGUID && x.LanguageID == LAN).FirstOrDefault().LocationDescription : "",
                             EndLocation = a.EndLocationGUID != null ? DbAHD.codeLocationsLanguages.Where(x => x.LocationGUID == a.EndLocationGUID && x.LanguageID == LAN).FirstOrDefault().LocationDescription : "",

                             LeaveOutDate = a.LeaveOutDate,
                             ActualLeaveInDate = a.ActualLeaveInDate,
                             ActualLeaveOutDate = a.ActualLeaveOutDate,
                             HRComments = a.HRComments,
                             ReviewedByHRStaffName = a.ReviewedByHRStaffName,
                             HRReviewDate = a.ReviewedDate,
                             CountryName = R1.CountryDescription,
                             RRLeaveDates = a.dataRestAndRecuperationRequestLeaveDate.Where(x => x.LeaveTypeGUID == R_R).FirstOrDefault() != null ?
                                                ("Travel In :" + a.dataRestAndRecuperationRequestLeaveDate.Where(x => x.LeaveTypeGUID == R_R).FirstOrDefault().TravelTimeIn + " "
                                                + "Travel Out :" + a.dataRestAndRecuperationRequestLeaveDate.Where(x => x.LeaveTypeGUID == R_R).FirstOrDefault().TravelTimeOut
                                                )
                                                 : "",
                             AnualLeaveDates = a.dataRestAndRecuperationRequestLeaveDate.Where(x => x.LeaveTypeGUID == annual_leave).FirstOrDefault() != null ?
                                                        ("Travel In :" + a.dataRestAndRecuperationRequestLeaveDate.Where(x => x.LeaveTypeGUID == annual_leave).FirstOrDefault().TravelTimeIn + " "
                                                        + "Travel Out :" + a.dataRestAndRecuperationRequestLeaveDate.Where(x => x.LeaveTypeGUID == annual_leave).FirstOrDefault().TravelTimeOut
                                                        )
                                                         : "",
                             SupervisorComments = a.SupervisorComments,
                             RepresentativeComments = a.RepresentativeComments,
                             SupervisorApprovedDate = a.SupervisorApprovedDate,

                             ToVerifyBy = R2.FullName,
                             ToCertifyBy = R3.FullName,

                             EmployeeComments = a.EmployeeComments,
                             DestinationCountry = a.DestinationCountry,
                             EligibleDate = a.EligibleDate,
                             PT8Number = a.PT8Number,
                             CreatedByGUID = (Guid)a.CreatedByGUID,
                             SupervisorToChangeGUID = (Guid)a.SupervisorGUID,
                             SupervisorGUID = (Guid)a.SupervisorGUID,
                             ReviewedByHRStaffGUID = a.ReviewedByHRStaffGUID,
                             ApprovedByHRStaffGUID = a.ApprovedByHRStaffGUID,
                             ApprovedByName = a.ApprovedByName,
                             RepresentativeDecisionDate = a.RepresentativeDecisionDate,
                             CurrentStep = a.CurrentStep,
                             Active = a.Active,
                             IsSameUser = 0,
                             dataRestAndRecuperationRequestRowVersion = a.dataRestAndRecuperationRequestRowVersion,

                         }).FirstOrDefault();

            model.LeaveInDateStringFormat = Convert.ToDateTime(model.LeaveInDate).ToString("dd-MM-yyyy");
            model.LeaveOutDateStringFormat = Convert.ToDateTime(model.LeaveOutDate).ToString("dd-MM-yyyy");
            model.SubmittedDateFormat = Convert.ToDateTime(model.SubmittedDate).ToString("dd-MM-yyyy");
            Guid RRLeave = Guid.Parse("67979D6D-5B7B-4A85-AA6D-CD604A1BDF75");
            Guid currStaff = Guid.Parse(model.StaffGUID);
            var currRR = DbAHD.dataRestAndRecuperationRequestLeaveDate.Where(x => x.LeaveTypeGUID == R_R && x.dataRestAndRecuperationRequest.StaffGUID == currStaff
            && x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID).FirstOrDefault();
            ViewBag.TotalDaysFromLastRR = null;
            Guid _testStaff = Guid.Parse(model.StaffGUID);
            model.EligibleDate = DbAHD.dataInternationalStaffAttendance.Where(x => x.StaffGUID == _testStaff).Select(x => x.ToDate).Max();
            if (currRR != null)
            {
                var test = DbAHD.dataInternationalStaffAttendance.Where(x => x.InternationalStaffAttendanceTypeGUID == RRLeave && x.StaffGUID == currStaff
                 ).OrderByDescending(x => x.FromDate).ToList();
                var test2 = currRR.TravelTimeIn;
                var lastRR = DbAHD.dataInternationalStaffAttendance.Where(x => x.InternationalStaffAttendanceTypeGUID == RRLeave && x.StaffGUID == currStaff
                && x.FromDate < currRR.TravelTimeIn).OrderByDescending(x => x.FromDate).FirstOrDefault();


                ViewBag.TotalDaysFromLastRR = lastRR != null ? (currRR.TravelTimeIn - lastRR.ToDate).Value.Days : 0;
            }

            if (!String.IsNullOrEmpty(model.HRReviewDateFormat))
            {
                model.HRReviewDateFormat = Convert.ToDateTime(model.HRReviewDateFormat).ToString("dd-MM-yyyy");
            }
            if (!String.IsNullOrEmpty(model.SupervisorApprovedDateFormat))
            {
                model.SupervisorApprovedDateFormat = Convert.ToDateTime(model.SupervisorApprovedDateFormat).ToString("dd-MM-yyyy");
            }
            Guid hrAccessGUID = Guid.Parse("DE411F27-0EA8-49FB-964D-00833BB012F8");
            Guid reprReview = Guid.Parse("B3BDF367-3F74-45E4-AF0A-1AEE013C0E81");
            var actionGUIDs = DbCMS.codeActions.Where(x => x.ActionGUID == hrAccessGUID && x.Active).Select(x => x.ActionGUID).FirstOrDefault();
            var checkCurrUser = DbCMS.userProfiles.Where(x => x.userServiceHistory.UserGUID == UserGUID && x.Active).Select(x => x.UserProfileGUID).FirstOrDefault();
            var hasHRPermissionsGuid = DbCMS.userPermissions.Where(x => x.ActionGUID == actionGUIDs && x.UserProfileGUID == checkCurrUser && x.Active).Select(x => x.UserProfileGUID).ToList();
            var hasReprsenttivePermissions = DbCMS.userPermissions.Where(x => x.ActionGUID == reprReview && x.UserProfileGUID == checkCurrUser && x.Active).Select(x => x.UserProfileGUID).ToList();


            if (UserGUID == model.CreatedByGUID)
            {
                model.AccessLevel = 1;
                ViewBag.userAccessLevel = 1;
            }


            else if (UserGUID == model.ApprovedByHRStaffGUID || UserGUID == model.ReviewedByHRStaffGUID || (hasHRPermissionsGuid.Count > 0))
                model.AccessLevel = 2;
            else if (UserGUID == model.SupervisorGUID)
                model.AccessLevel = 3;
            else if (hasReprsenttivePermissions.Count > 0)
            {
                model.AccessLevel = 5;

            }
            if (model.FlowStatusGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c773777") || model.FlowStatusGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c773772"))
            {
                model.RejectAccess = 1;
            }


            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("StaffRAndRLeaveRequestCreate", "StaffRAndRLeave", new { Area = "AHD" }));
            ViewBag.currentStep = model.CurrentStep;
            if (UserGUID == Guid.Parse(model.StaffGUID))
            {
                model.IsSameUser = 1;
            }

            return View("~/Areas/AHD/Views/StaffRAndRLeaveRequest/RAndRLeaveRequestForm.cshtml", model);
        }
        #endregion

        #region R_R leave
        [HttpPost]
        public ActionResult GetCalanderRRLeaves(DateTime start, DateTime end, Guid guid)
        {
            //Access is authorized by Access Action Department
            //List<string> AuthorizedListDepartment = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.AppointmentType.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();
            ////Access is authorized by Access Action DutyStation
            //List<string> AuthorizedListDutyStation = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.InternationalStaffAttendancePresence.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();

            // var userProfiles = DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            var Events = (from a in DbAHD.dataTempRestAndRecuperationRequestLeaveDate
                          .Where(x => x.TravelTimeIn >= start && x.TravelTimeOut <= end && x.Active && x.RestAndRecuperationLeaveGUID == guid).ToList()
                              //.Where(x => AuthorizedListDepartment.Contains(x.codeAppointmentType.DepartmentGUID.ToString()))
                              //.Where(x => AuthorizedListDutyStation.Contains(x.DutyStationGUID.ToString()))
                          join b in DbAHD.codeAHDInternationalStaffAttendanceType.Where(x => x.Active) on a.LeaveTypeGUID equals b.InternationalStaffAttendanceTypeGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          select new CalendarEvents
                          {
                              EventId = a.TempRestAndRecuperationRequestLeaveDateGUID,
                              UserGUID = a.RestAndRecuperationLeaveGUID,
                              EventStartDate = a.TravelTimeIn,
                              EventEndDate = a.TravelTimeOut.Value.AddDays(1),
                              Title = R1.AttendanceTypeName,
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
                              R1.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting ? "#FFBD33" : "",
                              borderColor = R1.InternationalStaffAttendanceTypeGUID.ToString().ToUpper() == "13afca94-4fa0-479a-85db-8aef4bc64cbb" ? "#00c0ef" : "#00c0ef",


                          }).ToList();
            return Json(new { CalendarEvents = Events }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RRLeaveCalendarUpdate(Guid PK)
        {

            var model = (from a in DbAHD.dataTempRestAndRecuperationRequestLeaveDate.WherePK(PK)

                         join b in DbAHD.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN) on a.LeaveTypeGUID equals b.ValueGUID into LJ2
                         from R2 in LJ2.DefaultIfEmpty()
                         select new TempRestAndRecuperationRequestLeaveDateModel
                         {
                             TempRestAndRecuperationRequestLeaveDateGUID = a.TempRestAndRecuperationRequestLeaveDateGUID,
                             RestAndRecuperationLeaveGUID = a.RestAndRecuperationLeaveGUID,

                             LeaveTypeGUID = a.LeaveTypeGUID,
                             LeaveTypeName = R2.ValueDescription,
                             TravelTimeIn = a.TravelTimeIn,
                             TravelTimeOut = a.TravelTimeOut,
                             Comments = a.Comments,

                             Active = a.Active,

                         }).FirstOrDefault();


            Guid hrAccessGUID = Guid.Parse("DE411F27-0EA8-49FB-964D-00833BB012F8");
            var myModel = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID).FirstOrDefault();

            var checkCurrUser = DbCMS.userProfiles.Where(x => x.userServiceHistory.UserGUID == UserGUID && x.Active).Select(x => x.UserProfileGUID).FirstOrDefault();
            var hasHRPermissionsGuid = DbCMS.userPermissions.Where(x => x.ActionGUID == hrAccessGUID && x.UserProfileGUID == checkCurrUser && x.Active).
                Select(x => x.UserProfileGUID).ToList();



            if ((myModel != null && UserGUID != myModel.CreatedByGUID) && hasHRPermissionsGuid.Count <= 0)
            {
                //myModel.AccessLevel = 1;
                //ViewBag.userAccessLevel = 1;
                return Json(DbAHD.PermissionError());
            }




            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("TempRestAndRecuperationLeaveDate", "StaffRAndRLeave", new { Area = "AHD" }));

            return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_TempLeaveDatesUpdateModal.cshtml", model);
        }
        #endregion

        #region RR Documents



        public ActionResult RAndRDocumentsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/AHD/Views/RAndRDocuments/_DocumentDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<RestAndRecuperationRequestDocumentDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<RestAndRecuperationRequestDocumentDataTableModel>(DataTable.Filters);
            }

            Guid myDocumentTypeGUID = Guid.Parse("2d0b59f3-347b-4fa1-8793-a7741d4c35bd");
            var Result = (from a in DbAHD.dataRestAndRecuperationRequestDocument.AsExpandable().Where(x => x.Active && (x.RestAndRecuperationLeaveGUID == PK))

                          join b in DbAHD.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN && x.codeTablesValues.TableGUID == myDocumentTypeGUID) on a.DocumentTypeGUID equals b.ValueGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          select new RestAndRecuperationRequestDocumentDataTableModel
                          {
                              RestAndRecuperationRequestDocumentGUID = a.RestAndRecuperationRequestDocumentGUID,
                              RestAndRecuperationLeaveGUID = a.RestAndRecuperationLeaveGUID.ToString(),

                              DocumentTypeGUID = a.DocumentTypeGUID.ToString(),
                              DocumentType = R1.ValueDescription,
                              Comments = a.Comments,

                              Active = a.Active,
                              dataRestAndRecuperationRequestDocumentRowVersion = a.dataRestAndRecuperationRequestDocumentRowVersion

                          }).Where(Predicate);


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult RAndRDocumentsCreate(Guid FK)
        {
            //if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeave.Create, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            return PartialView("~/Areas/AHD/Views/RAndRDocuments/_DocumentUpdateModal.cshtml",
                new RestAndRecuperationRequestDocumentUpdateModel { RestAndRecuperationLeaveGUID = FK });
        }

        public ActionResult DownloadStaffDocumentFile(Guid id)
        {
            var model = DbAHD.dataRestAndRecuperationRequestDocument.Where(x => x.RestAndRecuperationRequestDocumentGUID == id).FirstOrDefault();
            var fullPath = model.RestAndRecuperationRequestDocumentGUID + "." + model.DocumentExtension;
            string sourceFile = Server.MapPath("~/Areas/AHD/UploadedDocuments/RRLeaveDocuments/" + fullPath);
            byte[] fileBytes = System.IO.File.ReadAllBytes(sourceFile);

            string fileName = DateTime.Now.ToString("yyMMdd") + fullPath;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            // return Json(DbCMS.SingleUpdateMessage(null, null, null, null, "Please Wait...."));
        }

        [HttpPost]
        public FineUploaderResult UploadRAndRDocuments(FineUpload upload, Guid RestAndRecuperationLeaveGUID, Guid DocumentTypeGUID, string comments)
        {
            string error = "Error ";

            if (FileTypeValidator.IsPDF(upload.InputStream) || FileTypeValidator.IsImage(upload.InputStream) ||
                FileTypeValidator.IsExcel(upload.InputStream) ||
                FileTypeValidator.IsWord(upload.InputStream)
                )
            {

                return new FineUploaderResult(true, new { path = UploadDocument(upload, RestAndRecuperationLeaveGUID, DocumentTypeGUID, comments), success = true });
            }
            return new FineUploaderResult((error == "" ? true : false), new { Error = error, path = "", success = (error == "" ? true : false) });
        }

        public string UploadDocument(FineUpload upload, Guid RestAndRecuperationLeaveGUID, Guid DocumentTypeGUID, string comments)
        {
            var _stearm = upload.InputStream;
            DateTime ExecutionTime = DateTime.Now;
            //string _ext = Path.GetExtension(upload.FileName).Split('.')[1];
            dataRestAndRecuperationRequestDocument documentUplod = new dataRestAndRecuperationRequestDocument();
            documentUplod.RestAndRecuperationRequestDocumentGUID = Guid.NewGuid();
            //string FilePath = Server.MapPath("~/Areas/AHD/UploadedDocuments/" + documentUplod.ItemIntpuDetailUploadedDocumentGUID + _ext);

            string _ext = Path.GetExtension(upload.FileName).Split('.')[1];

            string FolderPath = Server.MapPath("~/Areas/AHD/UploadedDocuments/RRLeaveDocuments/");
            Directory.CreateDirectory(FolderPath);
            //int LatestFileVersion = 0;
            //try { LatestFileVersion = (from a in DbPPA.PPAFileVersion.Where(x => x.PPAOriginalFileGUID == PPAOriginalFileGUID && x.FileActionByUserGUID == UserGUID) select a.FileVersion).Max(); } catch { }
            //if (LatestFileVersion == -1) LatestFileVersion = 0;



            string FilePath = FolderPath + "/" + documentUplod.RestAndRecuperationRequestDocumentGUID.ToString() + "." + _ext;

            using (var fileStream = System.IO.File.Create(FilePath))
            {
                upload.InputStream.Seek(0, SeekOrigin.Begin);
                upload.InputStream.CopyTo(fileStream);
            }
            documentUplod.RestAndRecuperationLeaveGUID = RestAndRecuperationLeaveGUID;

            documentUplod.DocumentExtension = _ext;
            documentUplod.DocumentTypeGUID = DocumentTypeGUID;
            documentUplod.Comments = comments;


            //documentUplod.Comments = ItemInputDetailGUID;
            //documentUplod.CreatedByGUID = UserGUID;
            //documentUplod.CreatedDate = ExecutionTime;
            DbAHD.Create(documentUplod, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }


            //Server.MapPath("~/Areas/AHD/temp/Standard_Operating_Procedure_for_issuing_ICT_equipment_to_staff" + DateTime.Now.ToBinary() + ".pdf");


            return "~/Areas/AHD/UploadedDocuments/RRLeaveDocuments/" + documentUplod.RestAndRecuperationRequestDocumentGUID + ".xlsx";
        }

        public ActionResult RAndRDocumentsUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeave.Access, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            RestAndRecuperationRequestDocumentUpdateModel model = DbAHD.dataRestAndRecuperationRequestDocument.Where(x => x.RestAndRecuperationRequestDocumentGUID == PK).Select(f => new RestAndRecuperationRequestDocumentUpdateModel
            {

                DocumentTypeGUID = (Guid)f.DocumentTypeGUID,
                RestAndRecuperationLeaveGUID = (Guid)f.RestAndRecuperationLeaveGUID,

                RestAndRecuperationRequestDocumentGUID = f.RestAndRecuperationRequestDocumentGUID,
                Comments = f.Comments,
                Active = f.Active

            }).FirstOrDefault();
            return PartialView("~/Areas/AHD/Views/RAndRDocuments/_DocumentUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RAndRDocumentsCreate(dataRestAndRecuperationRequestDocument model)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeave.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            if (!ModelState.IsValid || (model.DocumentTypeGUID == null)) return PartialView("~/Areas/AHD/Views/RAndRDocuments/_DocumentUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbAHD.Create(model, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.RAndRDocumentsDataTable, DbAHD.PrimaryKeyControl(model), DbAHD.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RAndRDocumentsUpdate(dataRestAndRecuperationRequestDocument model)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeave.Update, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            if (!ModelState.IsValid || model.DocumentTypeGUID == null) return PartialView("~/Areas/AHD/Views/RAndRDocuments/__DocumentUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbAHD.Update(model, Permissions.InternationalStaffRestAndRecuperationLeave.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.RAndRDocumentsDataTable,
                    DbAHD.PrimaryKeyControl(model),
                    DbAHD.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyRAndRDocuments(model.RestAndRecuperationRequestDocumentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RAndRDocumentsDelete(dataRestAndRecuperationRequestDocument model)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeave.Delete, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            List<dataRestAndRecuperationRequestDocument> DeletedLanguages = DeleteRAndRDocuments(new List<dataRestAndRecuperationRequestDocument> { model });

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleDeleteMessage(DeletedLanguages, DataTableNames.RAndRDocumentsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyRAndRDocuments(model.RestAndRecuperationRequestDocumentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RAndRDocumentsRestore(dataRestAndRecuperationRequestDocument model)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeave.Restore, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            if (ActiveRAndRDocuments(model))
            {
                return Json(DbAHD.RecordExists());
            }

            List<dataRestAndRecuperationRequestDocument> RestoredLanguages = RestoreStaffDocument(Portal.SingleToList(model));

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleRestoreMessage(RestoredLanguages, DataTableNames.RAndRDocumentsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyRAndRDocuments(model.RestAndRecuperationRequestDocumentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult RAndRDocumentsDataTableDelete(List<dataRestAndRecuperationRequestDocument> models)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeave.Delete, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            List<dataRestAndRecuperationRequestDocument> DeletedLanguages = DeleteRAndRDocuments(models);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.RAndRDocumentsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult RestAndRecuperationRequestDocumentDataTableModelRestore(List<dataRestAndRecuperationRequestDocument> models)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeave.Restore, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            List<dataRestAndRecuperationRequestDocument> RestoredLanguages = RestoreStaffDocument(models);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.RAndRDocumentsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        private List<dataRestAndRecuperationRequestDocument> DeleteRAndRDocuments(List<dataRestAndRecuperationRequestDocument> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataRestAndRecuperationRequestDocument> DeletedStaffBankAccount = new List<dataRestAndRecuperationRequestDocument>();

            string query = DbAHD.QueryBuilder(models, Permissions.InternationalStaffRestAndRecuperationLeave.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbAHD.Database.SqlQuery<dataRestAndRecuperationRequestDocument>(query).ToList();

            foreach (var language in languages)
            {
                DeletedStaffBankAccount.Add(DbAHD.Delete(language, ExecutionTime, Permissions.InternationalStaffRestAndRecuperationLeave.DeleteGuid, DbCMS));
            }

            return DeletedStaffBankAccount;
        }

        private List<dataRestAndRecuperationRequestDocument> RestoreStaffDocument(List<dataRestAndRecuperationRequestDocument> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataRestAndRecuperationRequestDocument> RestoredLanguages = new List<dataRestAndRecuperationRequestDocument>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbAHD.QueryBuilder(models, Permissions.InternationalStaffRestAndRecuperationLeave.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbAHD.Database.SqlQuery<dataRestAndRecuperationRequestDocument>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveRAndRDocuments(language))
                {
                    RestoredLanguages.Add(DbAHD.Restore(language, Permissions.InternationalStaffRestAndRecuperationLeave.DeleteGuid, Permissions.InternationalStaffRestAndRecuperationLeave.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyRAndRDocuments(Guid PK)
        {
            dataRestAndRecuperationRequestDocument dbModel = new dataRestAndRecuperationRequestDocument();

            var Language = DbAHD.dataRestAndRecuperationRequestDocument.Where(l => l.RestAndRecuperationRequestDocumentGUID == PK).FirstOrDefault();
            var dbLanguage = DbAHD.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataRestAndRecuperationRequestDocumentRowVersion.SequenceEqual(dbModel.dataRestAndRecuperationRequestDocumentRowVersion))
            {
                return Json(DbAHD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbAHD, dbModel, "LanguagesContainer"));
        }

        private bool ActiveRAndRDocuments(dataRestAndRecuperationRequestDocument model)
        {
            int LanguageID = DbAHD.dataRestAndRecuperationRequestDocument
                                  .Where(x =>
                                              x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID &&
                                              x.DocumentTypeGUID == model.DocumentTypeGUID &&

                                              x.Active).Count();
            if (LanguageID > 0)
            {
                //Fix
                ModelState.AddModelError("LanguageID", "Already Exist");
            }

            return (LanguageID > 0);
        }

        #endregion RRDocuments

        #region Edit 
        public ActionResult ChangeRequestInfo(Guid FK)
        {

            Guid hrAccessGUID = Guid.Parse("DE411F27-0EA8-49FB-964D-00833BB012F8");
            var staffs = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == FK).FirstOrDefault();

            var checkCurrUser = DbCMS.userProfiles.Where(x => x.userServiceHistory.UserGUID == UserGUID && x.Active).Select(x => x.UserProfileGUID).FirstOrDefault();
            var hasHRPermissionsGuid = DbCMS.userPermissions.Where(x => x.ActionGUID == hrAccessGUID && x.UserProfileGUID == checkCurrUser && x.Active).
                Select(x => x.UserProfileGUID).ToList();







            if ((staffs != null && staffs.CreatedByGUID != UserGUID) && (staffs != null && hasHRPermissionsGuid.Count <= 0))
            {
                return Json(DbAHD.PermissionError());
            }
            else
            {

                return PartialView("~/Areas/AHD/Views/StaffRAndRLeaveRequest/_EditRAndRRequest.cshtml",
                    new StaffRAndRLeaveRequestModel { RestAndRecuperationLeaveGUID = FK });
            }
        }


        //[HttpPost, ValidateAntiForgeryToken]
        //public ActionResult EditRAndRRequestUpdate(StaffRAndRLeaveRequestModel model)
        //{
        //    Guid hrAccessGUID = Guid.Parse("DE411F27-0EA8-49FB-964D-00833BB012F8");
        //    var myModel = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID).FirstOrDefault();
        //    var tempdates = DbAHD.dataTempRestAndRecuperationRequestLeaveDate.Where(x => x.TempRestAndRecuperationRequestLeaveDateGUID == model.TempRestAndRecuperationRequestLeaveDateGUID).FirstOrDefault();

        //    var checkCurrUser = DbCMS.userProfiles.Where(x => x.userServiceHistory.UserGUID == UserGUID && x.Active).Select(x => x.UserProfileGUID).FirstOrDefault();
        //    var hasHRPermissionsGuid = DbCMS.userPermissions.Where(x => x.ActionGUID == hrAccessGUID && x.UserProfileGUID == checkCurrUser && x.Active).
        //        Select(x => x.UserProfileGUID).ToList();
        //    //here


        //    if ((myModel != null && UserGUID != myModel.StaffGUID) && hasHRPermissionsGuid.Count <= 0)
        //    {
        //        //myModel.AccessLevel = 1;
        //        //ViewBag.userAccessLevel = 1;
        //        return Json(DbAHD.PermissionError());
        //    }

        //    if (!ModelState.IsValid) return PartialView("~/Areas/AHD/Views/TempRAndRLeaveDates/_TempLeaveDatesUpdateModal.cshtml", model);



        //    foreach (var item in _realleaves.Where(x => x.LeaveTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend || x.LeaveTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday))
        //    {
        //        temp = new dataTempRestAndRecuperationRequestLeaveDate();
        //        temp.TempRestAndRecuperationRequestLeaveDateGUID = Guid.NewGuid();
        //        temp.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;
        //        temp.LeaveTypeName = item.LeaveName;
        //        temp.LeaveTypeGUID = item.LeaveTypeGUID;
        //        temp.TravelTimeOut = item.startdateName;
        //        temp.TravelTimeIn = item.startdateName;
        //        temp.Comments = model.Comments;
        //        allleaves.Add(temp);
        //    }

        //    foreach (var item in _realleaves.Where(x => x.LeaveTypeGUID == model.LeaveTypeGUID))
        //    {
        //        temp = new dataTempRestAndRecuperationRequestLeaveDate();
        //        temp.TempRestAndRecuperationRequestLeaveDateGUID = Guid.NewGuid();
        //        temp.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;
        //        temp.LeaveTypeName = item.LeaveName;
        //        temp.LeaveTypeGUID = item.LeaveTypeGUID;
        //        temp.TravelTimeOut = item.enddateName;
        //        temp.TravelTimeIn = item.startdateName;
        //        temp.Comments = model.Comments;
        //        allleaves.Add(temp);
        //    }



        //    DbAHD.CreateBulk(allleaves, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);
        //    dataRestAndRecuperationRequestLeaveDate _dateleave = new dataRestAndRecuperationRequestLeaveDate();
        //    List<dataRestAndRecuperationRequestLeaveDate> _alldateleave = new List<dataRestAndRecuperationRequestLeaveDate>();

        //    foreach (var item in allleaves)
        //    {
        //        _dateleave = new dataRestAndRecuperationRequestLeaveDate();
        //        _dateleave.RestAndRecuperationRequestLeaveDateGUID = item.TempRestAndRecuperationRequestLeaveDateGUID;
        //        _dateleave.RestAndRecuperationLeaveGUID = myModel.RestAndRecuperationLeaveGUID;
        //        _dateleave.LeaveTypeGUID = item.LeaveTypeGUID;
        //        _dateleave.LeaveTypeName = item.LeaveTypeName;
        //        _dateleave.TravelTimeOut = item.TravelTimeOut;
        //        _dateleave.TravelTimeIn = item.TravelTimeIn;
        //        _dateleave.Comments = item.Comments;

        //        _alldateleave.Add(_dateleave);
        //    }

        //    DbAHD.CreateBulk(_alldateleave, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);



        //    try
        //    {
        //        DbAHD.SaveChanges();
        //        DbCMS.SaveChanges();
        //        var toremove = DbAHD.dataInternationalStaffAttendance.Where(x => x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID).ToList();
        //        if (toremove.Count > 0)
        //        {
        //            DbAHD.dataInternationalStaffAttendance.RemoveRange(toremove);
        //            var toAddAttendace = DbAHD.dataRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID).ToList();
        //            var staffrequest = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == model.RestAndRecuperationLeaveGUID).FirstOrDefault();
        //            var userperson = DbAHD.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active == true && x.UserGUID == staffrequest.StaffGUID).FirstOrDefault();

        //            //attendaces 
        //            List<dataInternationalStaffAttendance> allAttendacnes = new List<dataInternationalStaffAttendance>();
        //            foreach (var item in toAddAttendace)
        //            {
        //                if (item.LeaveTypeGUID != Guid.Empty)
        //                {
        //                    dataInternationalStaffAttendance attendance = new dataInternationalStaffAttendance
        //                    {
        //                        InternationalStaffAttendanceGUID = Guid.NewGuid(),
        //                        InternationalStaffAttendanceTypeGUID = item.LeaveTypeGUID,
        //                        StaffGUID = staffrequest.StaffGUID,
        //                        StaffName = userperson.FirstName + " " + userperson.Surname,
        //                        LeaveLocation = item.dataRestAndRecuperationRequest.StaffLoction,
        //                        TotalDays = item.TravelTimeOut != null ? ((int)(item.TravelTimeOut - item.TravelTimeIn).Value.TotalDays + 1) : 1,
        //                        Comments = item.Comments,
        //                        IsAutomated = false,
        //                        FromDate = item.TravelTimeIn,
        //                        ToDate = item.TravelTimeOut,
        //                        CreatedByGUID = UserGUID,
        //                        CreateDate = ExecutionTime,
        //                        RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID,
        //                        RestAndRecuperationRequestLeaveDateGUID = item.RestAndRecuperationRequestLeaveDateGUID

        //                    };
        //                    allAttendacnes.Add(attendance);

        //                }

        //            }

        //            DbAHD.CreateBulk(allAttendacnes, Permissions.InternationalStaffRestAndRecuperationLeaveHRReview.CreateGuid, ExecutionTime, DbCMS);
        //            DbAHD.SaveChanges();
        //            DbCMS.SaveChanges();
        //        }




        //        return Json(DbCMS.SingleUpdateMessage(null, null, null, "InitializeCalendarLeavs();"));
        //        //return Json(DbAHD.SingleUpdateMessage(DataTableNames.InternationalTempStaffRAndRDatesDataTable, DbAHD.PrimaryKeyControl(tempdates), DbAHD.RowVersionControls(Portal.SingleToList(tempdates))));

        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        //return ConcurrencyItemModel(model.WarehouseItemModelGUID);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(DbAHD.ErrorMessage(ex.Message));
        //    }
        //    return Json(DbAHD.SingleUpdateMessage(null, null, DbAHD.RowVersionControls(tempdates, tempdates)));
        //}
        #endregion

        #region Send Reminder
        public JsonResult SendReminderToRRForm(Guid _restAndRecuperationLeaveGUID)
        {

            string URL = "";
            string Anchor = "";
            string Link = "";
            DateTime ExecutionTime = DateTime.Now;
            var model = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == _restAndRecuperationLeaveGUID).FirstOrDefault();
            if (model.FlowStatusGUID == Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E9992"))
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
            }

            if (model.FlowStatusGUID == InternationalStaffRAndRLeaveFlowStatus.PendingHRReview)
            {

                SendHRApprovalReviewMail((Guid)_restAndRecuperationLeaveGUID);


            }
            else if (model.FlowStatusGUID == InternationalStaffRAndRLeaveFlowStatus.PendingSupervisorApproval)
            {

                SendSupervisorApprovalReviewMail((Guid)_restAndRecuperationLeaveGUID);


            }
            else if (model.FlowStatusGUID == InternationalStaffRAndRLeaveFlowStatus.PendingSupervisorApproval)
            {

                SendSupervisorApprovalReviewMail((Guid)_restAndRecuperationLeaveGUID);


            }
            else if (model.FlowStatusGUID == InternationalStaffRAndRLeaveFlowStatus.PendingRepresentativeApproval)
            {
                if (model.SupervisorGUID != model.ApprovedByGUID)
                {
                    SendHRApporvlConfirmationAndRepresentativeMail((Guid)_restAndRecuperationLeaveGUID);
                }
                else if (model.SupervisorGUID == model.ApprovedByGUID)
                {
                    RepresentativeApproveRAndRLeaveCreate((Guid)_restAndRecuperationLeaveGUID, "");
                }

            }
            else if (model.FlowStatusGUID == InternationalStaffRAndRLeaveFlowStatus.Approved)
            {
                SendApprovalRepresentativeAndStaffConfirmationMail((Guid)_restAndRecuperationLeaveGUID);
            }

            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);


        }


        #endregion

        public JsonResult DeleteRRRequest(Guid _restAndRecuperationLeaveGUID)
        {

       
            if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeaveHRReview.Access, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            var model = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == _restAndRecuperationLeaveGUID).FirstOrDefault();

            var doc = DbAHD.dataRestAndRecuperationRequestDocument.Where(x => x.RestAndRecuperationLeaveGUID == _restAndRecuperationLeaveGUID).ToList();
            var country = DbAHD.dataRestAndRecuperationRequestLeaveCountry.Where(x => x.RestAndRecuperationLeaveGUID == _restAndRecuperationLeaveGUID).ToList();
            var dates = DbAHD.dataRestAndRecuperationRequestLeaveDate.Where(x => x.RestAndRecuperationLeaveGUID == _restAndRecuperationLeaveGUID).ToList();
            var flow = DbAHD.dataRestAndRecuperationRequestLeaveFlow.Where(x => x.RestAndRecuperationLeaveGUID == _restAndRecuperationLeaveGUID).ToList();
            var _attendances = DbAHD.dataInternationalStaffAttendance.Where(x => x.RestAndRecuperationLeaveGUID == _restAndRecuperationLeaveGUID).ToList();

            DbAHD.dataRestAndRecuperationRequest.Remove(model);
            DbAHD.dataRestAndRecuperationRequestDocument.RemoveRange(doc);
            DbAHD.dataRestAndRecuperationRequestLeaveCountry.RemoveRange(country);
            DbAHD.dataRestAndRecuperationRequestLeaveDate.RemoveRange(dates);
            DbAHD.dataRestAndRecuperationRequestLeaveFlow.RemoveRange(flow);
            DbAHD.dataInternationalStaffAttendance.RemoveRange(_attendances);
            DbAHD.SaveChanges();


            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public ActionResult ChangeRepName(Guid? restAndRecuperationLeaveGUID, Guid ApprovedByGUID)
        {
            var priFlow = DbAHD.dataRestAndRecuperationRequestLeaveFlow.Where(x => x.RestAndRecuperationLeaveGUID == restAndRecuperationLeaveGUID && x.IsLastAction == true).FirstOrDefault();
            if (priFlow.FlowStatusGUID == InternationalStaffRAndRLeaveFlowStatus.Approved
                  || priFlow.FlowStatusGUID == InternationalStaffRAndRLeaveFlowStatus.Closed)
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);

            }
            //if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeave.Create, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            // if (model.WarehouseItemDescription == null || ActiveItem(model)) return PartialView("~/Areas/AHD/Views/Items/_ItemForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            var hrReview = DbAHD.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.UserGUID == UserGUID).FirstOrDefault();

            Guid EntityPK = Guid.NewGuid();
            var model = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == restAndRecuperationLeaveGUID).FirstOrDefault();



            model.ApprovedByGUID = ApprovedByGUID;
            // myModel.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;


            DbAHD.Update(model, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.InternationalStaffRestAndRecuperationLeave.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("Create", "StaffRAndRLeave", new { Area = "AHD" })), Container = "ItemModelDetailFormControls" });


            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();

                //return RedirectToAction("StaffLeavsIndex");
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost]
        public ActionResult ChangeSupervisorName(Guid? restAndRecuperationLeaveGUID, Guid SupervisorGUID)
        {
            var priFlow = DbAHD.dataRestAndRecuperationRequestLeaveFlow.Where(x => x.RestAndRecuperationLeaveGUID == restAndRecuperationLeaveGUID && x.IsLastAction == true).FirstOrDefault();
            if (priFlow.FlowStatusGUID == InternationalStaffRAndRLeaveFlowStatus.Approved
                  || priFlow.FlowStatusGUID == InternationalStaffRAndRLeaveFlowStatus.Closed
                     || priFlow.FlowStatusGUID == InternationalStaffRAndRLeaveFlowStatus.PendingRepresentativeApproval
                  )
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);

            }
            //if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeave.Create, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            // if (model.WarehouseItemDescription == null || ActiveItem(model)) return PartialView("~/Areas/AHD/Views/Items/_ItemForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            var hrReview = DbAHD.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.UserGUID == UserGUID).FirstOrDefault();

            Guid EntityPK = Guid.NewGuid();
            var model = DbAHD.dataRestAndRecuperationRequest.Where(x => x.RestAndRecuperationLeaveGUID == restAndRecuperationLeaveGUID).FirstOrDefault();



            model.SupervisorGUID = SupervisorGUID;
            var supervisor = DbAHD.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.UserGUID == SupervisorGUID).FirstOrDefault();

            model.SupervisorName = supervisor.FirstName + " " + supervisor.Surname;
            // myModel.RestAndRecuperationLeaveGUID = model.RestAndRecuperationLeaveGUID;


            DbAHD.Update(model, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.InternationalStaffRestAndRecuperationLeave.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("Create", "StaffRAndRLeave", new { Area = "AHD" })), Container = "ItemModelDetailFormControls" });


            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();

                //return RedirectToAction("StaffLeavsIndex");
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }


    }
}