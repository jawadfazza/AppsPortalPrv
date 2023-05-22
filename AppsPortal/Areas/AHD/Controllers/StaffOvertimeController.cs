using AHD_DAL.Model;
using AHD_DAL.ViewModels;
using AppsPortal.Areas.AHD.Service;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using OfficeOpenXml;
using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;


namespace AppsPortal.Areas.AHD.Controllers
{
    public class StaffOvertimeController : AHDBaseController
    {
        // GET: AHD/StaffOvertime
        #region Over Time
        [Route("AHD/test/")]
        public ActionResult test()
        {
            return View("~/Areas/AHD/Views/StaffOvertime/test.cshtml");
        }

        [Route("AHD/StaffOvertimeIndex/")]
        public ActionResult StaffOvertimeIndex()
        {
            if (!CMS.HasAction(Permissions.StaffOvertime.Access, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return View("~/Areas/AHD/Views/StaffOvertime/Index.cshtml");
        }
        [Route("AHD/AllMonthCycleOvertimeIndex/")]
        public ActionResult AllMonthCycleOvertimeIndex()
        {
            if (!CMS.HasAction(Permissions.StaffOvertime.Access, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return View("~/Areas/AHD/Views//StaffOvertime/MonthCycles/AllMonthCyclesIndex.cshtml");
        }
        //[Route("AHD/AllCycleMonthOvertime/")]
        //public ActionResult AllCycleMonthOvertime()
        //{
        //    if (!CMS.HasAction(Permissions.StaffOvertime.Access, Apps.AHD))
        //    {
        //        throw new HttpException(401, "Unauthorized access");
        //    }


        //    return View("~/Areas/AHD/Views/StaffOvertime/AllOvertime.cshtml");
        //}
        [Route("AHD/StaffOvertimeDataTable/")]
        public JsonResult StaffOvertimeDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<StaffOvertimeDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<StaffOvertimeDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.StaffOvertime.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix

            var All = (
                from a in DbAHD.dataStaffOvertime.Where(x => x.Active).AsExpandable()
                join b in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.LastFlowStatusGUID equals b.ValueGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.CreatedByGUID equals c.UserGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()
                join d in DbAHD.v_StaffProfileInformation on a.UserGUID equals d.UserGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()
                join e in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.StepGUID equals e.ValueGUID into LJ4
                from R4 in LJ4.DefaultIfEmpty()
                select new StaffOvertimeDataTableModel
                {
                    StaffOvertimeGUID = a.StaffOvertimeGUID.ToString(),
                    Year = a.Year.ToString(),
                    Month = a.Month,
                    Active = a.Active,
                    ActionDate = a.ActionDate,
                    TimeIn = a.TimeIn,
                    TimeOut = a.TimeOut,
                    TotalHours = (float)a.TotalHours,
                    OvertimeReason = a.OvertimeReason,
                    CTO = a.CTO,
                    PerformedHours = a.PerformedHours,
                    Step = R4.ValueDescription,
                    StepGUID = a.StepGUID.ToString(),
                    Grade = R3.Grade,
                    GradeGUID = R3.GradeGUID.ToString(),

                    TotalPay = (float)a.TotalPay,
                    StaffName = R3.FullName,
                    DutyStation = R3.DutyStation,
                    DutyStationGUID = R3.DutyStationGUID.ToString(),
                    JobTitleGUID = R3.JobTitleGUID.ToString(),

                    JobTitle = R3.JobTitle,
                    CreatedByGUID = a.CreatedByGUID.ToString(),
                    LastFlowStatusGUID = a.LastFlowStatusGUID.ToString(),
                    LastFlowName = R1.ValueDescription,
                    CreatedBy = R2.FirstName + " " + R2.Surname,
                    //FlowStatusGUID = a.FlowStatusGUID,
                    //PaymentDurationName = a.PaymentDurationName,
                    //TotalStaffConfirm = a.dataStaffOvertime1.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Confirmed).Count(),
                    //TotalStaffNotConfirm = a.dataStaffOvertime1.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Pending).Count(),

                    //OrderId = a.OrderId,
                    //CreateDate = a.CreateDate,
                    dataStaffOvertimeRowVersion = a.dataStaffOvertimeRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<StaffOvertimeDataTableModel> Result = Mapper.Map<List<StaffOvertimeDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).OrderByDescending(x => x.CreatedBy).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }


        public JsonResult StaffOvertimeForSalaryDataTable(DataTableRecievedOptions options, Guid PK)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<StaffOvertimeForSalaryDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<StaffOvertimeForSalaryDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.StaffOvertime.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
            var _cycleSalary = DbAHD.dataSalaryCycle.Where(x => x.SalaryCycleGUID == PK).FirstOrDefault();
            var _codeMonth = DbAHD.codeMonth.Where(x => x.MonthCode == _cycleSalary.MonthName).FirstOrDefault();

            var All = (
                from a in DbAHD.dataOvertimeMonthCycle.Where(x => x.Active && x.Year == _cycleSalary.Year && x.Month == _codeMonth.MonthName)
                join b in DbAHD.dataOvertimeMonthCycleStaff on a.OvertimeMonthCycleGUID equals b.OvertimeMonthCycleGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()

                join c in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on R1.LastFlowStatusGUID equals c.ValueGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()

                join d in DbAHD.v_StaffProfileInformation on R1.UserGUID equals d.UserGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()
                join e in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on R1.StepGUID equals e.ValueGUID into LJ4
                from R4 in LJ4.DefaultIfEmpty()

                select new StaffOvertimeForSalaryDataTableModel
                {
                    OvertimeMonthCycleStaffGUID = R1.OvertimeMonthCycleStaffGUID.ToString(),
                    OvertimeMonthCycleGUID = a.OvertimeMonthCycleGUID.ToString(),
                    Year = a.Year.ToString(),
                    Month = a.Month,
                    Active = a.Active,
                    CycleName = a.Month + " " + a.Year,
                    TotalHoursPayed = (float)R1.TotalHoursPayed,
                    TotalPerformedHours = R1.TotalPerformedHours,
                    //CTO = R1.Ct,

                    Step = R4.ValueDescription,
                    StepGUID = R1.StepGUID.ToString(),
                    Grade = R3.Grade,
                    GradeGUID = R3.GradeGUID.ToString(),

                    TotalPay = (float)R1.TotalPay,
                    StaffName = R3.FullName,
                    DutyStation = R3.DutyStation,
                    DutyStationGUID = R3.DutyStationGUID.ToString(),
                    JobTitleGUID = R3.JobTitleGUID.ToString(),

                    JobTitle = R3.JobTitle,

                    LastFlowStatusGUID = R1.LastFlowStatusGUID.ToString(),
                    LastFlowName = R2.ValueDescription,

                    //FlowStatusGUID = a.FlowStatusGUID,
                    //PaymentDurationName = a.PaymentDurationName,
                    //TotalStaffConfirm = a.dataStaffOvertime1.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Confirmed).Count(),
                    //TotalStaffNotConfirm = a.dataStaffOvertime1.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Pending).Count(),

                    //OrderId = a.OrderId,
                    //CreateDate = a.CreateDate,
                    dataOvertimeMonthCycleStaffRowVersion = R1.dataOvertimeMonthCycleStaffRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<StaffOvertimeForSalaryDataTableModel> Result = Mapper.Map<List<StaffOvertimeForSalaryDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).OrderByDescending(x => x.TotalPay).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }




        //[Route("AHD/StaffOvertime/Create/")]
        public ActionResult StaffOvertimeCreate(Guid _OvertimeMonthCycleGUID)
        {
            //if (!CMS.HasAction(Permissions.StaffOvertime.Create, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var _lastovertime = DbAHD.dataStaffOvertime.Where(x => x.UserGUID == UserGUID).OrderByDescending(x => x.TimeOut).FirstOrDefault();

            return PartialView("~/Areas/AHD/Views/StaffOvertime/_StaffOvertimeUpdateModal.cshtml", new StaffOvertimeUpdateModel
            {
                StaffOvertimeGUID = Guid.Empty,
                StepGUID = _lastovertime != null ? _lastovertime.StepGUID : null,
                OvertimeMonthCycleGUID = _OvertimeMonthCycleGUID,
                OvertimeMonthCycleStaffGUID = Guid.NewGuid()
            });

        }

        //[Route("AHD/StaffOvertime/Update/{PK}")]
        public ActionResult StaffOvertimeUpdate(Guid PK)
        {
            var model = (from a in DbAHD.dataStaffOvertime.WherePK(PK)
                         join b in DbAHD.dataOvertimeMonthCycleStaff on a.OvertimeMonthCycleStaffGUID equals b.OvertimeMonthCycleStaffGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new StaffOvertimeUpdateModel
                         {
                             StaffOvertimeGUID = a.StaffOvertimeGUID,
                             Year = a.Year,
                             Month = a.Month,
                             ActionDate = a.ActionDate,
                             UserGUID = a.UserGUID,
                             TimeIn = a.TimeIn,
                             TimeOut = a.TimeOut,
                             HourIn = a.HourIn,
                             HourOut = a.HourOut,
                             TotalHours = (float)a.TotalHours,
                             TotalPay = (float)a.TotalPay,
                             CreatedByGUID = a.CreatedByGUID,
                             OvertimeMonthCycleStaffGUID = a.OvertimeMonthCycleStaffGUID,
                             LastFlowStatusGUID = a.LastFlowStatusGUID,
                             Comments = a.Comments,
                             dataStaffOvertimeRowVersion = a.dataStaffOvertimeRowVersion,
                             StepGUID = a.StepGUID,
                             DayWorkingTypeGUID = a.DayWorkingTypeGUID,
                             OvertimeReason = a.OvertimeReason,
                             CTO = a.CTO,
                             GradeGUID = a.GradeGUID,
                             JobtitleGUID = a.JobtitleGUID,
                             CreateDate = a.CreateDate,
                             OvertimeMonthCycleGUID = R1.OvertimeMonthCycleGUID,




                             Active = a.Active,
                             //DangerPaymentConfirmationStatus= R1.ValueDescription,


                         }).FirstOrDefault();
            //ViewBag.StaffOvertimeGUID = PK;
            //ViewBag.TotalStaffNotConfirmed = DbAHD.dataStaffOvertime.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Pending && a.StaffOvertimeGUID == PK).Count();
            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("StaffOvertime", "StaffOvertimes", new { Area = "AHD" }));
            ViewBag.UserGUID = model.UserGUID;
            return PartialView("~/Areas/AHD/Views/StaffOvertime/_StaffOvertimeUpdateModal.cshtml", model);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffOvertimeCreate(StaffOvertimeUpdateModel model)
        {
            //if (!CMS.HasAction(Permissions.StaffOvertime.Create, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            if (!ModelState.IsValid || (model.StepGUID == null) || (string.IsNullOrEmpty(model.HourOut)) || (string.IsNullOrEmpty(model.HourIn)) || (model.TimeIn == null) || (model.DayWorkingTypeGUID == null) || (string.IsNullOrEmpty(model.OvertimeReason)) ) return PartialView("~/Areas/AHD/Views/StaffOvertimes/_StaffOvertimeForm.cshtml", model);

            //if (!ModelState.IsValid || (string.IsNullOrEmpty(model.HourOut)) || (string.IsNullOrEmpty(model.HourIn)) || (string.IsNullOrEmpty(model.OvertimeReason))) return PartialView("~/Areas/AHD/Views/StaffOvertimes/_StaffOvertimeForm.cshtml", model);

            //if ((model.TimeOut < model.TimeIn))
            //{
            //    ModelState.AddModelError("Error: ", "Kindly revise your entry not allowed to enter date out bigger than date in");
            //    return PartialView("~/Areas/AHD/Views/StaffOvertimes/_StaffOvertimeForm.cshtml", model);
            //}
            var _firstPart = model.TimeIn;
            DateTime newDateTimeIn = model.TimeIn.Value.Add(TimeSpan.Parse(model.HourIn));
            DateTime newDateTimeOut = model.TimeIn.Value.Add(TimeSpan.Parse(model.HourOut));

            model.TimeIn = newDateTimeIn;
            model.TimeOut = newDateTimeOut;
            //model.TimeIn=DateTime.Parse(model.TimeIn + " " + model.HourIn);
            //model.TimeOut = DateTime.Parse(model.TimeOut + " " + model.HourOut);

            dataOvertimeMonthCycleStaff dataOvertimeMonthCycleStaff = DbAHD.dataOvertimeMonthCycleStaff.Where(x =>
                            x.UserGUID == UserGUID && x.OvertimeMonthCycleGUID == model.OvertimeMonthCycleGUID).FirstOrDefault();
            Guid _overStaffGUID = Guid.NewGuid();
            dataOvertimeMonthCycleStaff _monthStaff = new dataOvertimeMonthCycleStaff();
            if (dataOvertimeMonthCycleStaff == null)
            {
                _monthStaff = new dataOvertimeMonthCycleStaff
                {
                    OvertimeMonthCycleStaffGUID = _overStaffGUID,
                    UserGUID = UserGUID,
                    OvertimeMonthCycleGUID = model.OvertimeMonthCycleGUID,
                    TotalHoursPayed = 0,
                    TotalPerformedHours = 0,
                    TotalPay = 0,
                    LastFlowStatusGUID = OvertimeFlowStatus.Submitted,


                };
                DbAHD.CreateNoAudit(_monthStaff);
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
            }
            var _allovertime = DbAHD.dataStaffOvertime.Where(x => x.UserGUID == UserGUID).ToList();
            foreach (var model2 in _allovertime)
            {
                if ((model2.TimeOut == model.TimeOut) && (model2.TimeIn == model.TimeIn)
                    && (model.TimeIn.Value.Hour <= model2.TimeOut.Hour && (model2.TimeOut.Hour >= model.TimeOut.Value.Hour)))
                {
                    ModelState.AddModelError("Error: ", "Kindly revise the entry data to avoid conflict in dates");
                    return PartialView("~/Areas/AHD/Views/StaffOvertimes/_StaffOvertimeForm.cshtml", model);
                }


            }


            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();
            var _staff = DbAHD.v_staffCoreDataOverview.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            var _workingHour = DbAHD.codeAHDOrgnizationWorkingHour.ToList();

            dataStaffOvertime StaffOvertime = new dataStaffOvertime();


            StaffOvertime.StaffOvertimeGUID = EntityPK;
            StaffOvertime.Year = model.TimeIn.Value.Year;
            StaffOvertime.TimeIn = (DateTime)model.TimeIn;
            StaffOvertime.TimeOut = (DateTime)model.TimeOut;
            StaffOvertime.HourIn = model.HourIn;
            StaffOvertime.HourOut = model.HourOut;
            StaffOvertime.DayWorkingTypeGUID = model.DayWorkingTypeGUID;
            StaffOvertime.Comments = model.Comments;
            //StaffOvertime.StepGUID = model.StepGUID;
            //StaffOvertime.OvertimeReason = model.OvertimeReason;
            StaffOvertime.ActionDate = ExecutionTime;
            StaffOvertime.Month = ProcessData.GetMonthName(model.TimeIn.Value.Month);
            StaffOvertime.UserGUID = UserGUID;
            StaffOvertime.JobtitleGUID = _staff.JobTitleGUID;
            StaffOvertime.GradeGUID = _staff.GradeGUID;
            StaffOvertime.OvertimeReason = model.OvertimeReason;
            StaffOvertime.CreatedByGUID = UserGUID;
            StaffOvertime.StepGUID = model.StepGUID;
            StaffOvertime.OvertimeMonthCycleStaffGUID = dataOvertimeMonthCycleStaff != null ? dataOvertimeMonthCycleStaff.OvertimeMonthCycleStaffGUID : _monthStaff.OvertimeMonthCycleStaffGUID;
            //StaffOvertime.TimeIn =(DateTime) model.TimeIn;
            //StaffOvertime.TimeOut = model.TimeOut;
            TimeSpan v = (model.TimeOut - model.TimeIn).Value;
            float vTotalHours = 0;
            int _totalminutes = v.Minutes;
            float _tempMinutes = (float)(v.TotalHours * 60);
            if (_totalminutes < 15)
            {

                vTotalHours = v.Hours;
            }
            else if (_totalminutes >= 15 && _totalminutes <= 30)
            {

                vTotalHours = ((_tempMinutes - _totalminutes) + 30) / 60;

            }
            else if (_totalminutes > 30 && _totalminutes < 45)
            {

                vTotalHours = ((_tempMinutes - _totalminutes) + 30) / 60;

            }
            else if (_totalminutes >= 15 && _totalminutes >= 45)
            {

                vTotalHours = ((_tempMinutes - _totalminutes) + 60) / 60;

            }




            Guid _ramdanGUID = Guid.Parse("21E1C315-0100-4CB1-A891-EADB9B0FE5C2");

            int _typeDriver = 0;
            Guid? _staffType = Guid.Parse("7FFA5C6F-FA42-4DC4-A072-2BD5C4ED53C8");
            if (_staff.JobTitleGUID == Guid.Parse("7FFA5C6F-FA42-4DC4-A072-2BD5C4ED53C8"))
            {
                _staffType = null;


            }
            var _ramdan = _workingHour.Where(x => x.WorkingDayTypeGUID == _ramdanGUID &&
                   (model.ActionDate >= x.StartDate && model.ActionDate <= x.EndDate)
                   && x.StaffTypeGUID == _staffType).FirstOrDefault();
            var _normal = _workingHour.Where(x => x.WorkingDayTypeGUID != _ramdanGUID && (x.EndDate == null)
            && x.StaffTypeGUID == _staffType).FirstOrDefault();
            var _staffCore = DbAHD.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            var _staffStatus = DbAHD.codeAHDOvertimeBaseSalary.Where(x => x.GradeGUID == _staffCore.StaffGradeGUID && x.StepGUID == model.StepGUID).FirstOrDefault();

            if (model.DayWorkingTypeGUID == Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E7832"))
            {
                var _checkbeforeHas4Hours = DbAHD.dataStaffOvertime.Where(x => x.TimeIn == model.TimeIn && x.UserGUID == UserGUID && x.TotalHours == 4).FirstOrDefault();
                if (_checkbeforeHas4Hours == null)
                {
                    StaffOvertime.PerformedHours = vTotalHours;
                    if (vTotalHours < 4 && vTotalHours >= 0.5)
                        vTotalHours = 4;
                    StaffOvertime.TotalHours = vTotalHours;
                }


            }
            else
            {
                if (_ramdan!=null &&((_ramdan != null && _ramdan.TotalHour > vTotalHours)) ||
                    (_ramdan != null && (_ramdan == null && _normal.TotalHour > vTotalHours)))
                {
                    StaffOvertime.TotalHours = 0;
                    StaffOvertime.PerformedHours = 0;
                }
                else
                {

                    StaffOvertime.TotalHours = _ramdan != null ? vTotalHours - _ramdan.TotalHour : vTotalHours - (_normal!=null? _normal.TotalHour:0);
                    StaffOvertime.PerformedHours = _ramdan != null ? vTotalHours - _ramdan.TotalHour : vTotalHours - (_normal != null ? _normal.TotalHour:0);
                }
            }
            float? _checkbrake = (float)(StaffOvertime.TotalHours / 6.5);
            if (_checkbrake > 1)
            {
                StaffOvertime.TotalHours = StaffOvertime.TotalHours - (_checkbrake < 2 ? 0.5 : _checkbrake < 3 ? 1 : _checkbrake < 4 ? 1.5 : 0);
                StaffOvertime.PerformedHours = StaffOvertime.PerformedHours - (_checkbrake < 2 ? 0.5 : _checkbrake < 3 ? 1 : _checkbrake < 4 ? 1.5 : 0);
            }
            double extraHour = (double)((model.DayWorkingTypeGUID == Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E7832")) ? StaffOvertime.TotalHours * 2 : StaffOvertime.TotalHours * 1.5);
            StaffOvertime.TotalPay = extraHour * (_staffStatus.AmountPerHour / 2088);
            StaffOvertime.TotalPay = Math.Round((double)StaffOvertime.TotalPay, 0);

            StaffOvertime.CreateDate = ExecutionTime;

            //StaffOvertime.RequestYear = model.RequestDate.Value.Year;
            StaffOvertime.LastFlowStatusGUID = OvertimeFlowStatus.Submitted;
            //StaffOvertime.LastFlowStatus = "Submitted";
            StaffOvertime.Comments = model.Comments;
            var _checkAllTotalHours = DbAHD.dataStaffOvertime.Where(x => x.UserGUID == UserGUID && x.Month == StaffOvertime.Month && x.Year == StaffOvertime.Year).Select(x => x.TotalHours).Sum();
            float _allhours = 0;
            if (_checkAllTotalHours > 0)
            {
                _allhours = (float)(_checkAllTotalHours > 0 ? _checkAllTotalHours + StaffOvertime.TotalHours : 0);
            }
            else
            {
                _allhours = (float)StaffOvertime.TotalHours;
            }
            if (_staffCore.JobTitleGUID == Guid.Parse("7FFA5C6F-FA42-4DC4-A072-2BD5C4ED53C8"))
            {
                if (_allhours > 50)
                {
                    StaffOvertime.TotalHours = _allhours - 50;
                    StaffOvertime.PerformedHours = _allhours - 50;
                    if (StaffOvertime.TotalHours < 0)
                        StaffOvertime.TotalHours = 0;
                    double _ex = (double)((model.DayWorkingTypeGUID == Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E7832")) ? StaffOvertime.TotalHours * 2 : StaffOvertime.TotalHours * 1.5);
                    StaffOvertime.TotalPay = _ex * (_staffStatus.AmountPerHour / 2088);
                    StaffOvertime.TotalPay = Math.Round((double)StaffOvertime.TotalPay, 0);
                    StaffOvertime.CTO = (float)(_allhours - 50);
                }
            }
            else
            {
                if (_allhours > 40)
                {
                    StaffOvertime.TotalHours = _allhours - 40;
                    StaffOvertime.PerformedHours = _allhours - 40;
                    if (StaffOvertime.TotalHours < 0)
                        StaffOvertime.TotalHours = 0;
                    double _ex = (double)((model.DayWorkingTypeGUID == Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E7832")) ? StaffOvertime.TotalHours * 2 : StaffOvertime.TotalHours * 1.5);
                    StaffOvertime.TotalPay = _ex * (_staffStatus.AmountPerHour / 2088);
                    StaffOvertime.TotalPay = Math.Round((double)StaffOvertime.TotalPay, 0);
                    StaffOvertime.CTO = (float)(_allhours - 40);
                }
            }
            DbAHD.CreateNoAudit(StaffOvertime);

            dataStaffOvertimeFlow newFlowToReview = new dataStaffOvertimeFlow
            {

                StaffOvertimeFlowGUID = Guid.NewGuid(),
                StaffOvertimeGUID = StaffOvertime.StaffOvertimeGUID,
                CreatedByGUID = UserGUID,
                FlowStatusGUID = OvertimeFlowStatus.Submitted,
                ActionDate = ExecutionTime,
                IsLastAction = true,
                OrderID = 1,


            };

            //StaffOvertime.LastFlowStatusName = "Pending Verification By Staff";

            //Entitlemnt.FinanceApprovedByGUID = financeApprovedGUID;
            DbAHD.CreateNoAudit(newFlowToReview);


            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();


                var _currCycleStaff = DbAHD.dataOvertimeMonthCycleStaff.Where(x => x.UserGUID == StaffOvertime.UserGUID &&
                 x.OvertimeMonthCycleGUID == model.OvertimeMonthCycleGUID && x.Active == true).FirstOrDefault();
                var _detail = DbAHD.dataStaffOvertime.Where(x =>
                              x.OvertimeMonthCycleStaffGUID == _currCycleStaff.OvertimeMonthCycleStaffGUID
                              && x.Active == true).ToList();
                _currCycleStaff.TotalHoursPayed = _detail.Select(x => x.TotalHours).Sum();
                _currCycleStaff.TotalPerformedHours = _detail.Select(x => x.PerformedHours).Sum();
                _currCycleStaff.TotalPay = _detail.Select(x => x.TotalPay).Sum();
                _currCycleStaff.TotalPay = Math.Round((double)_currCycleStaff.TotalPay, 0);
                DbAHD.UpdateNoAudit(_currCycleStaff);
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();

                //#region Send Mail
                //string URL = "";
                //string Anchor = "";
                //string Link = "";
                ////string _month = "2020 Dec";
                //string _month = StaffOvertime.Month.ToString() + " " + StaffOvertime.Year.ToString();
                //var _staffName = DbAHD.v_staffCoreDataOverview.Where(x => x.UserGUID == StaffOvertime.UserGUID).FirstOrDefault();
                //string SubjectMessage = resxEmails.OvertimeSubject.Replace("$StaffName", _staffName.FullName).Replace("$month", _month);


                ////to send mail to staff 
                //// var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();
                //URL = AppSettingsKeys.Domain + "/AHD/StaffOvertime/StaffOvertimeConfirmByReviewer/?PK=" + new Portal().GUIDToString(StaffOvertime.StaffOvertimeGUID);
                //Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmDangerPayReceiving + "</a>";
                //Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";



                //string _message = resxEmails.OvertimeByReviewer
                //    .Replace("$StaffName", _staffName.FullName.ToString())
                //    .Replace("$month", _month)

                //    .Replace("$VerifyLink", Anchor)

                //    ;

                //if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                //int isRec = 1;

                //Guid _overtimeReviewer = Guid.Parse("3260e610-8182-4c57-9412-8cb15315cce3");
                //var tempPermGUIDs = DbCMS.userPermissions.Where(x => (x.ActionGUID == _overtimeReviewer && x.Active == true
                //                 ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();
                //var _userGuids = DbCMS.userProfiles.Where(x => tempPermGUIDs.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

                //var _backupUsers = DbCMS.userServiceHistory.Where(x => _userGuids.Contains(x.UserGUID)).Select(x => x.EmailAddress).Distinct().ToList();


                //string copyEmails = string.Join(" ;", _backupUsers);


                ////var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
                //string copy_recipients = _staffName.EmailAddress;
                //Send(copyEmails, SubjectMessage, _message, isRec, copy_recipients);

                //#endregion


                return Json(DbCMS.SingleUpdateMessage(null, null, null, "InitializeCalendarLeavs();"));

                //return Json(DbAHD.SingleUpdateMessage(DataTableNames.StaffOvertimeDataTable, DbAHD.PrimaryKeyControl(StaffOvertime), DbAHD.RowVersionControls(Portal.SingleToList(StaffOvertime))));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }


        public ActionResult StaffOvertimeConfirmByReviewer(Guid PK)
        {

            if (!CMS.HasAction(Permissions.StaffOvertime.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            StffCalendarOvertimeUpdateModel model = new StffCalendarOvertimeUpdateModel();
            var result = DbAHD.dataOvertimeMonthCycleStaff.Where(x => x.OvertimeMonthCycleStaffGUID == PK).FirstOrDefault();
            var _myFirstOver = DbAHD.dataStaffOvertime.Where(x => x.OvertimeMonthCycleStaffGUID == PK).FirstOrDefault();
            var _staff = DbAHD.v_staffCoreDataOverview.Where(x => x.UserGUID == result.UserGUID).FirstOrDefault();

            model.OvertimeMonthCycleStaffGUID = result.OvertimeMonthCycleStaffGUID;
            model.UserGUID = (Guid)result.UserGUID;
            model.OvertimeMonthCycleGUID = (Guid)result.OvertimeMonthCycleGUID;
            model.JobTitleGUID = _staff.JobTitleGUID;
            model.FlowStatusGUID = result.LastFlowStatusGUID;
            model.Grade = _staff.Grade;
            model.JobTitle = _staff.JobTitle;
            model.FullName = _staff.FullName;
            model.Step = DbAHD.codeTablesValuesLanguages.Where(x => x.ValueGUID == _myFirstOver.StepGUID && x.LanguageID == LAN && x.Active).FirstOrDefault().ValueDescription;


            return View("~/Areas/AHD/Views/StaffOvertime/Calendar/ReviewStaffOverTimeCalendar.cshtml", model);
        }

        public JsonResult DeleteOvertimeRecord(Guid _StaffOvertimeGUID)
        {
            var _overtime = DbAHD.dataStaffOvertime.Where(x => x.StaffOvertimeGUID == _StaffOvertimeGUID).FirstOrDefault();
            var _monthStaffcycle = DbAHD.dataOvertimeMonthCycleStaff.Where(x => x.OvertimeMonthCycleStaffGUID == _overtime.OvertimeMonthCycleStaffGUID).FirstOrDefault();
            var _detailflow = DbAHD.dataStaffOvertimeFlow.Where(x => x.StaffOvertimeGUID == _StaffOvertimeGUID).ToList();
            DbAHD.dataStaffOvertime.Remove(_overtime);
            DbAHD.dataStaffOvertimeFlow.RemoveRange(_detailflow);
            DbAHD.SaveChanges();
            DbCMS.SaveChanges();

            var _currCycleStaff = DbAHD.dataOvertimeMonthCycleStaff.Where(x => x.OvertimeMonthCycleStaffGUID == _monthStaffcycle.OvertimeMonthCycleStaffGUID
             && x.Active == true).FirstOrDefault();
            var _detail = DbAHD.dataStaffOvertime.Where(x =>
                          x.OvertimeMonthCycleStaffGUID == _currCycleStaff.OvertimeMonthCycleStaffGUID
                          && x.Active == true).ToList();
            _currCycleStaff.TotalHoursPayed = _detail.Select(x => x.TotalHours).Sum();
            _currCycleStaff.TotalPerformedHours = _detail.Select(x => x.PerformedHours).Sum();
            _currCycleStaff.TotalPay = _detail.Select(x => x.TotalPay).Sum();
            _currCycleStaff.TotalPay = Math.Round((double)_currCycleStaff.TotalPay, 0);
            DbAHD.UpdateNoAudit(_currCycleStaff);
            DbAHD.SaveChanges();
            DbCMS.SaveChanges();
            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffOvertimeUpdate(StaffOvertimeUpdateModel model)
        {
            //if (!CMS.HasAction(Permissions.StaffOvertime.Update, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            if (!ModelState.IsValid || (model.StepGUID == null) || (string.IsNullOrEmpty(model.HourOut)) || (string.IsNullOrEmpty(model.HourIn)) || (model.TimeIn == null) || (model.DayWorkingTypeGUID == null) || (string.IsNullOrEmpty(model.OvertimeReason))) return PartialView("~/Areas/AHD/Views/StaffOvertimes/_StaffOvertimeForm.cshtml", model);
            DateTime ExecutionTime = DateTime.Now;
            DateTime newDateTimeIn = model.TimeIn.Value.Add(TimeSpan.Parse(model.HourIn));
            DateTime newDateTimeOut = model.TimeIn.Value.Add(TimeSpan.Parse(model.HourOut));

            model.TimeIn = newDateTimeIn;
            model.TimeOut = newDateTimeOut;


            dataStaffOvertime StaffOvertime = Mapper.Map(model, new dataStaffOvertime());
            var _staff = DbAHD.v_staffCoreDataOverview.Where(x => x.UserGUID == model.UserGUID).FirstOrDefault();
            var _workingHour = DbAHD.codeAHDOrgnizationWorkingHour.ToList();
            TimeSpan v = (model.TimeOut - model.TimeIn).Value;
            float vTotalHours = 0;
            int _totalminutes = v.Minutes;
            float _tempMinutes = (float)(v.TotalHours * 60);
            if (_totalminutes < 15)
            {

                vTotalHours = v.Hours;
            }
            else if (_totalminutes >= 15 && _totalminutes <= 30)
            {

                vTotalHours = ((_tempMinutes - _totalminutes) + 30) / 60;

            }
            else if (_totalminutes > 30 && _totalminutes < 45)
            {

                vTotalHours = ((_tempMinutes - _totalminutes) + 30) / 60;

            }
            else if (_totalminutes >= 15 && _totalminutes >= 45)
            {

                vTotalHours = ((_tempMinutes - _totalminutes) + 60) / 60;

            }




            Guid _ramdanGUID = Guid.Parse("21E1C315-0100-4CB1-A891-EADB9B0FE5C2");

            int _typeDriver = 0;
            Guid? _staffType = Guid.Parse("7FFA5C6F-FA42-4DC4-A072-2BD5C4ED53C8");
            if (_staff.JobTitleGUID == Guid.Parse("7FFA5C6F-FA42-4DC4-A072-2BD5C4ED53C8"))
            {
                _staffType = null;


            }
            var _ramdan = _workingHour.Where(x => x.WorkingDayTypeGUID == _ramdanGUID &&
                   (model.ActionDate >= x.StartDate && model.ActionDate <= x.EndDate)
                   && x.StaffTypeGUID == _staffType).FirstOrDefault();
            var _normal = _workingHour.Where(x => x.WorkingDayTypeGUID != _ramdanGUID && (x.EndDate == null)
            && x.StaffTypeGUID == _staffType).FirstOrDefault();
            var _staffCore = DbAHD.StaffCoreData.Where(x => x.UserGUID == model.UserGUID).FirstOrDefault();
            var _staffStatus = DbAHD.codeAHDOvertimeBaseSalary.Where(x => x.GradeGUID == _staffCore.StaffGradeGUID && x.StepGUID == model.StepGUID).FirstOrDefault();

            if (model.DayWorkingTypeGUID == Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E7832"))
            {
                var _checkbeforeHas4Hours = DbAHD.dataStaffOvertime.Where(x => x.TimeIn == model.TimeIn && x.UserGUID == model.UserGUID && x.TotalHours == 4).FirstOrDefault();
                if (_checkbeforeHas4Hours == null)
                {
                    StaffOvertime.PerformedHours = vTotalHours;
                    if (vTotalHours < 4 && vTotalHours >= 0.5)
                        vTotalHours = 4;
                    StaffOvertime.TotalHours = vTotalHours;
                }


            }
            else
            {
                if ((_ramdan != null && _ramdan.TotalHour > vTotalHours) ||
                    (_ramdan == null && _normal.TotalHour > vTotalHours))
                {
                    StaffOvertime.TotalHours = 0;
                    StaffOvertime.PerformedHours = 0;
                }
                else
                {

                    StaffOvertime.TotalHours = _ramdan != null ? vTotalHours - _ramdan.TotalHour : vTotalHours - _normal.TotalHour;
                    StaffOvertime.PerformedHours = _ramdan != null ? vTotalHours - _ramdan.TotalHour : vTotalHours - _normal.TotalHour;
                }
            }
            float? _checkbrake = (float)(StaffOvertime.TotalHours / 6.5);
            if (_checkbrake > 1)
            {
                StaffOvertime.TotalHours = StaffOvertime.TotalHours - (_checkbrake < 2 ? 0.5 : _checkbrake < 3 ? 1 : _checkbrake < 4 ? 1.5 : 0);
                StaffOvertime.PerformedHours = StaffOvertime.PerformedHours - (_checkbrake < 2 ? 0.5 : _checkbrake < 3 ? 1 : _checkbrake < 4 ? 1.5 : 0);
            }
            double extraHour = (double)((model.DayWorkingTypeGUID == Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E7832")) ? StaffOvertime.TotalHours * 2 : StaffOvertime.TotalHours * 1.5);
            StaffOvertime.TotalPay = extraHour * (_staffStatus.AmountPerHour / 2088);

            StaffOvertime.TotalPay = Math.Round((double)StaffOvertime.TotalPay, 0);
            StaffOvertime.CreateDate = ExecutionTime;

            //StaffOvertime.RequestYear = model.RequestDate.Value.Year;
            //StaffOvertime.LastFlowStatusGUID = OvertimeFlowStatus.PendingSupervisorReview;
            //StaffOvertime.LastFlowStatus = "Submitted";
            StaffOvertime.Comments = model.Comments;
            var _checkAllTotalHours = DbAHD.dataStaffOvertime.Where(x => x.UserGUID == model.UserGUID && x.Month == StaffOvertime.Month && x.Year == StaffOvertime.Year).Select(x => x.TotalHours).Sum();
            float _allhours = 0;
            if (_checkAllTotalHours > 0)
            {
                _allhours = (float)(_checkAllTotalHours > 0 ? _checkAllTotalHours + StaffOvertime.TotalHours : 0);
            }
            else
            {
                _allhours = (float)StaffOvertime.TotalHours;
            }
            if (_staffCore.JobTitleGUID == Guid.Parse("7FFA5C6F-FA42-4DC4-A072-2BD5C4ED53C8"))
            {
                if (_allhours > 50)
                {
                    StaffOvertime.TotalHours = _allhours - 50;
                    StaffOvertime.PerformedHours = _allhours - 50;
                    if (StaffOvertime.TotalHours < 0)
                        StaffOvertime.TotalHours = 0;
                    double _ex = (double)((model.DayWorkingTypeGUID == Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E7832")) ? StaffOvertime.TotalHours * 2 : StaffOvertime.TotalHours * 1.5);
                    StaffOvertime.TotalPay = _ex * (_staffStatus.AmountPerHour / 2088);
                    StaffOvertime.TotalPay = Math.Round((double)StaffOvertime.TotalPay, 0);
                    StaffOvertime.CTO = (float)(_allhours - 50);
                }
            }
            else
            {
                if (_allhours > 40)
                {
                    StaffOvertime.TotalHours = _allhours - 40;
                    StaffOvertime.PerformedHours = _allhours - 40;
                    if (StaffOvertime.TotalHours < 0)
                        StaffOvertime.TotalHours = 0;
                    double _ex = (double)((model.DayWorkingTypeGUID == Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E7832")) ? StaffOvertime.TotalHours * 2 : StaffOvertime.TotalHours * 1.5);
                    StaffOvertime.TotalPay = _ex * (_staffStatus.AmountPerHour / 2088);
                    StaffOvertime.TotalPay = Math.Round((double)StaffOvertime.TotalPay, 0);
                    StaffOvertime.CTO = (float)(_allhours - 40);
                }
            }
            DbAHD.UpdateNoAudit(StaffOvertime);//, Permissions.StaffOvertime.UpdateGuid, ExecutionTime, DbCMS);
            try
            {

                DbAHD.SaveChanges();
                DbCMS.SaveChanges();

                var _currCycleStaff = DbAHD.dataOvertimeMonthCycleStaff.Where(x => x.UserGUID == StaffOvertime.UserGUID &&
                          x.OvertimeMonthCycleGUID == model.OvertimeMonthCycleGUID
                          && x.Active == true).FirstOrDefault();
                var _detail = DbAHD.dataStaffOvertime.Where(x =>
                                x.OvertimeMonthCycleStaffGUID == _currCycleStaff.OvertimeMonthCycleStaffGUID
                                && x.Active == true).ToList();
                _currCycleStaff.TotalHoursPayed = _detail.Select(x => x.TotalHours).Sum();
                _currCycleStaff.TotalPerformedHours = _detail.Select(x => x.PerformedHours).Sum();
                _currCycleStaff.TotalPay = _detail.Select(x => x.TotalPay).Sum();
                _currCycleStaff.TotalPay = Math.Round((double)_currCycleStaff.TotalPay, 0);
                DbAHD.UpdateNoAudit(_currCycleStaff);
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();


                return Json(DbAHD.SingleUpdateMessage(DataTableNames.StaffOvertimeDataTable, DbAHD.PrimaryKeyControl(StaffOvertime), DbAHD.RowVersionControls(Portal.SingleToList(StaffOvertime))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyStaffOvertime(model.StaffOvertimeGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffOvertimeDelete(dataStaffOvertime model)
        {
            if (!CMS.HasAction(Permissions.StaffOvertime.Delete, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataStaffOvertime> DeletedStaffOvertime = DeleteStaffOvertime(Portal.SingleToList(model));
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.StaffOvertime.Restore, Apps.AHD), Container = "StaffOvertimeFormControls" });
            try
            {
                int CommitedRows = DbAHD.SaveChanges();
                DbAHD.SaveChanges();
                return Json(DbAHD.SingleDeleteMessage(CommitedRows, DeletedStaffOvertime.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyStaffOvertime(model.StaffOvertimeGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffOvertimeRestore(dataStaffOvertime model)
        {
            if (!CMS.HasAction(Permissions.StaffOvertime.Restore, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveStaffOvertime(model))
            {
                return Json(DbAHD.RecordExists());
            }
            List<dataStaffOvertime> RestoredStaffOvertime = RestoreStaffOvertimes(Portal.SingleToList(model));
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.StaffOvertime.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("StaffOvertimeCreate", "Configuration", new { Area = "AHD" })), Container = "StaffOvertimeFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.StaffOvertime.Update, Apps.AHD), Container = "StaffOvertimeFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.StaffOvertime.Delete, Apps.AHD), Container = "StaffOvertimeFormControls" });
            try
            {
                int CommitedRows = DbAHD.SaveChanges();
                DbAHD.SaveChanges();
                return Json(DbAHD.SingleRestoreMessage(CommitedRows, RestoredStaffOvertime, DbAHD.PrimaryKeyControl(RestoredStaffOvertime.FirstOrDefault()), Url.Action(DataTableNames.StaffOvertimeDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyStaffOvertime(model.StaffOvertimeGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffOvertimeDataTableDelete(List<dataStaffOvertime> models)
        {
            if (!CMS.HasAction(Permissions.StaffOvertime.Delete, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataStaffOvertime> DeletedStaffOvertime = DeleteStaffOvertime(models);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialDeleteMessage(DeletedStaffOvertime, models, DataTableNames.StaffOvertimeDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffOvertimeDataTableRestore(List<dataStaffOvertime> models)
        {
            if (!CMS.HasAction(Permissions.StaffOvertime.Restore, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataStaffOvertime> RestoredStaffOvertime = DeleteStaffOvertime(models);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialRestoreMessage(RestoredStaffOvertime, models, DataTableNames.StaffOvertimeDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        private List<dataStaffOvertime> DeleteStaffOvertime(List<dataStaffOvertime> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataStaffOvertime> DeletedStaffOvertime = new List<dataStaffOvertime>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT StaffOvertimeGUID,CONVERT(varchar(50), StaffOvertimeGUID) as C2 ,dataStaffOvertimeRowVersion FROM code.dataStaffOvertime where StaffOvertimeGUID in (" + string.Join(",", models.Select(x => "'" + x.StaffOvertimeGUID + "'").ToArray()) + ")";
            string query = DbAHD.QueryBuilder(models, Permissions.StaffOvertime.DeleteGuid, SubmitTypes.Delete, "");
            var Records = DbAHD.Database.SqlQuery<dataStaffOvertime>(query).ToList();
            foreach (var record in Records)
            {
                DeletedStaffOvertime.Add(DbAHD.Delete(record, ExecutionTime, Permissions.StaffOvertime.DeleteGuid, DbCMS));
            }
            return DeletedStaffOvertime;
        }
        private List<dataStaffOvertime> RestoreStaffOvertimes(List<dataStaffOvertime> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataStaffOvertime> RestoredStaffOvertime = new List<dataStaffOvertime>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT StaffOvertimeGUID,CONVERT(varchar(50), StaffOvertimeGUID) as C2 ,dataStaffOvertimeRowVersion FROM code.dataStaffOvertime where StaffOvertimeGUID in (" + string.Join(",", models.Select(x => "'" + x.StaffOvertimeGUID + "'").ToArray()) + ")";

            string query = DbAHD.QueryBuilder(models, Permissions.StaffOvertime.DeleteGuid, SubmitTypes.Restore, "");

            var Records = DbAHD.Database.SqlQuery<dataStaffOvertime>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveStaffOvertime(record))
                {
                    RestoredStaffOvertime.Add(DbAHD.Restore(record, Permissions.StaffOvertime.DeleteGuid, Permissions.StaffOvertime.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredStaffOvertime;
        }

        private JsonResult ConcurrencyStaffOvertime(Guid PK)
        {
            StaffOvertimeDataTableModel dbModel = new StaffOvertimeDataTableModel();

            var StaffOvertime = DbAHD.dataStaffOvertime.Where(x => x.StaffOvertimeGUID == PK).FirstOrDefault();
            var dbStaffOvertime = DbAHD.Entry(StaffOvertime).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbStaffOvertime, dbModel);

            if (StaffOvertime.dataStaffOvertimeRowVersion.SequenceEqual(dbModel.dataStaffOvertimeRowVersion))
            {
                return Json(DbAHD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbAHD, dbModel, "LanguagesContainer"));
        }

        private bool ActiveStaffOvertime(Object model)
        {
            StaffOvertimeUpdateModel StaffOvertime = Mapper.Map(model, new StaffOvertimeUpdateModel());
            int ModelDescription = DbAHD.dataStaffOvertime
                                    .Where(x => x.Month == StaffOvertime.Month &&
                                                x.Year == StaffOvertime.Year
                                                &&x.UserGUID== StaffOvertime.UserGUID
                                                &&x.TimeIn==StaffOvertime.TimeIn
                                                &&
                                                x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("Record ", " already exists");
            }
            return (ModelDescription > 0);
        }



        #endregion
        #region Overtime
        public JsonResult ApproveOvertimeByReviewer(Guid overtimeGUID)
        {
            if (!CMS.HasAction(Permissions.StaffOvertime.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            string URL = "";
            string Anchor = "";
            string Link = "";
            DateTime ExecutionTime = DateTime.Now;
            var _overtime = DbAHD.dataStaffOvertime.Where(x =>
            x.StaffOvertimeGUID == overtimeGUID).FirstOrDefault();
            if (_overtime.LastFlowStatusGUID == OvertimeFlowStatus.PendingSupervisorReview)
            {
                var toChange = DbAHD.dataStaffOvertimeFlow.Where(x => x.StaffOvertimeGUID == overtimeGUID
                                  && x.IsLastAction == true
                                  ).FirstOrDefault();
                toChange.IsLastAction = false;
                dataStaffOvertimeFlow newFlowToReview = new dataStaffOvertimeFlow
                {

                    StaffOvertimeFlowGUID = Guid.NewGuid(),
                    StaffOvertimeGUID = overtimeGUID,
                    CreatedByGUID = UserGUID,
                    FlowStatusGUID = OvertimeFlowStatus.PendingCertifying,
                    ActionDate = ExecutionTime,
                    IsLastAction = true,
                    OrderID = toChange.OrderID + 1,


                };
                _overtime.LastFlowStatusGUID = OvertimeFlowStatus.PendingCertifying;


                DbAHD.Create(newFlowToReview, Permissions.StaffOvertime.CreateGuid, ExecutionTime, DbCMS);
                DbAHD.Update(_overtime, Permissions.StaffOvertime.CreateGuid, ExecutionTime, DbCMS);
                DbAHD.Update(toChange, Permissions.StaffOvertime.CreateGuid, ExecutionTime, DbCMS);
                DbAHD.SaveChanges();


                //var staffCore = DbAHD.StaffCoreData.Where(x => x.UserGUID == Entitlemnt.StaffGUID).FirstOrDefault();

                //if (staffCore.ReportToGUID != null)
                //{
                string _month = _overtime.Year.ToString() + " " + _overtime.Month.ToString();
                var _staffName = DbAHD.v_staffCoreDataOverview.Where(x => x.UserGUID == _overtime.UserGUID).FirstOrDefault();
                string SubjectMessage = resxEmails.OvertimeSubject.Replace("$StaffName", _staffName.FullName).Replace("$month", _month);


                //to send mail to staff 
                // var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();
                URL = AppSettingsKeys.Domain + "/AHD/StaffOvertime/StaffOvertimeConfirmByReviewer/?PK=" + new Portal().GUIDToString(_overtime.StaffOvertimeGUID);
                Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmDangerPayReceiving + "</a>";
                Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";



                string _message = resxEmails.OvertimeByReviewer
                    .Replace("$StaffName", _staffName.FullName.ToString())
                    .Replace("$month", _month)
                    .Replace("$VerifyLink", Anchor)

                    ;

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;

                Guid _overtimeReviewer = Guid.Parse("3260e610-8182-4c57-9412-8cb15315cce3");
                var tempPermGUIDs = DbCMS.userPermissions.Where(x => (x.ActionGUID == _overtimeReviewer && x.Active == true
                                 ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();
                var _userGuids = DbCMS.userProfiles.Where(x => tempPermGUIDs.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

                var _backupUsers = DbCMS.userServiceHistory.Where(x => _userGuids.Contains(x.UserGUID)
                && x.UserGUID != _overtime.UserGUID).Select(x => x.EmailAddress).Distinct().ToList();

                _backupUsers.Add(_staffName.EmailAddress);
                _backupUsers = _backupUsers.Distinct().ToList();
                string copyEmails = string.Join(" ;", _backupUsers);


                Guid _overtimeCertifier = Guid.Parse("b70578a3-3f0b-4299-8116-058bf8062311");
                var tempcertGUIDs = DbCMS.userPermissions.Where(x => (x.ActionGUID == _overtimeCertifier && x.Active == true

                                 ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();
                var _usercertGuids = DbCMS.userProfiles.Where(x => tempcertGUIDs.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

                var _backupcertUsers = DbCMS.userServiceHistory.Where(x => _usercertGuids.Contains(x.UserGUID)
                && x.UserGUID != _overtime.UserGUID).Select(x => x.EmailAddress).Distinct().ToList();
                _backupcertUsers = _backupcertUsers.Distinct().ToList();

                string copyCertEmails = string.Join(" ;", _backupcertUsers);



                //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
                //string copy_recipients = _staffName.EmailAddress;
                Send(copyCertEmails, SubjectMessage, _message, isRec, copyEmails);

                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);

            }

            //}
            return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ApproveOvertimeByCertifier(Guid overtimeGUID)
        {
            if (!CMS.HasAction(Permissions.StaffOvertime.Update, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            string URL = "";
            string Anchor = "";
            string Link = "";
            DateTime ExecutionTime = DateTime.Now;
            var _overtime = DbAHD.dataStaffOvertime.Where(x =>
            x.StaffOvertimeGUID == overtimeGUID).FirstOrDefault();
            if (_overtime.LastFlowStatusGUID == OvertimeFlowStatus.PendingCertifying)
            {
                var toChange = DbAHD.dataStaffOvertimeFlow.Where(x => x.StaffOvertimeGUID == overtimeGUID
                                  && x.IsLastAction == true
                                  ).FirstOrDefault();
                toChange.IsLastAction = false;
                dataStaffOvertimeFlow newFlowToReview = new dataStaffOvertimeFlow
                {

                    StaffOvertimeFlowGUID = Guid.NewGuid(),
                    StaffOvertimeGUID = overtimeGUID,
                    CreatedByGUID = UserGUID,
                    FlowStatusGUID = OvertimeFlowStatus.Approved,
                    ActionDate = ExecutionTime,
                    IsLastAction = true,
                    OrderID = toChange.OrderID + 1,


                };
                _overtime.LastFlowStatusGUID = OvertimeFlowStatus.Approved;


                DbAHD.Create(newFlowToReview, Permissions.StaffOvertime.CreateGuid, ExecutionTime, DbCMS);
                DbAHD.Update(_overtime, Permissions.StaffOvertime.CreateGuid, ExecutionTime, DbCMS);
                DbAHD.Update(toChange, Permissions.StaffOvertime.CreateGuid, ExecutionTime, DbCMS);
                DbAHD.SaveChanges();


                //var staffCore = DbAHD.StaffCoreData.Where(x => x.UserGUID == Entitlemnt.StaffGUID).FirstOrDefault();

                //if (staffCore.ReportToGUID != null)
                //{

                var _staffName = DbAHD.v_staffCoreDataOverview.Where(x => x.UserGUID == _overtime.UserGUID).FirstOrDefault();

                string SubjectMessage = resxEmails.OvertimeSubject.Replace("$StaffName", _staffName.FullName).Replace("$month", _overtime.Year + " " + _overtime.Month);


                //to send mail to staff 
                // var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();
                //URL = AppSettingsKeys.Domain + "/AHD/StaffOvertime/StaffOvertimeConfirmByReviewer/?PK=" + new Portal().GUIDToString(_overtime.StaffOvertimeGUID);
                //Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmDangerPayReceiving + "</a>";
                //Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";



                string _message = resxEmails.OvertimeApprovalConfirmation
                    .Replace("$StaffName", _staffName.FullName.ToString())


                    ;

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;

                Guid _overtimeReviewer = Guid.Parse("3260e610-8182-4c57-9412-8cb15315cce3");
                var tempPermGUIDs = DbCMS.userPermissions.Where(x => (x.ActionGUID == _overtimeReviewer && x.Active == true
                                 ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();
                var _userGuids = DbCMS.userProfiles.Where(x => tempPermGUIDs.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

                var _backupUsers = DbCMS.userServiceHistory.Where(x => _userGuids.Contains(x.UserGUID)).Select(x => x.EmailAddress).Distinct().ToList();

                //_backupUsers.Add(_staffName.EmailAddress);
                //string copyEmails = string.Join(" ;", _backupUsers);


                Guid _overtimeCertifier = Guid.Parse("b70578a3-3f0b-4299-8116-058bf8062311");
                var tempcertGUIDs = DbCMS.userPermissions.Where(x => (x.ActionGUID == _overtimeReviewer && x.Active == true
                                 ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();
                var _usercertGuids = DbCMS.userProfiles.Where(x => tempcertGUIDs.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

                var _backupcertUsers = DbCMS.userServiceHistory.Where(x => _usercertGuids.Contains(x.UserGUID)).Select(x => x.EmailAddress).Distinct().ToList();

                _backupUsers.AddRange(_backupcertUsers);
                string copyCertEmails = string.Join(" ;", _backupUsers);



                //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
                //string copy_recipients = _staffName.EmailAddress;
                Send(_staffName.EmailAddress, SubjectMessage, _message, isRec, copyCertEmails);

                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);

            }

            //}
            return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
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
        #endregion

        #region Review New
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffOvertimeReviewUpdate(StaffOvertimeUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.StaffOvertime.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            Guid overtimeGUID = model.StaffOvertimeGUID;
            string URL = "";
            string Anchor = "";
            string Link = "";
            DateTime ExecutionTime = DateTime.Now;
            var _overtime = DbAHD.dataStaffOvertime.Where(x => x.StaffOvertimeGUID == overtimeGUID).FirstOrDefault();

            if (_overtime.LastFlowStatusGUID == OvertimeFlowStatus.PendingSupervisorReview)
            {
                var toChange = DbAHD.dataStaffOvertimeFlow.Where(x => x.StaffOvertimeGUID == overtimeGUID
                                  && x.IsLastAction == true
                                  ).FirstOrDefault();
                toChange.IsLastAction = false;
                dataStaffOvertimeFlow newFlowToReview = new dataStaffOvertimeFlow
                {

                    StaffOvertimeFlowGUID = Guid.NewGuid(),
                    StaffOvertimeGUID = overtimeGUID,
                    CreatedByGUID = UserGUID,
                    FlowStatusGUID = OvertimeFlowStatus.PendingCertifying,
                    ActionDate = ExecutionTime,
                    IsLastAction = true,
                    OrderID = toChange.OrderID + 1,


                };
                _overtime.LastFlowStatusGUID = OvertimeFlowStatus.PendingCertifying;


                DbAHD.Create(newFlowToReview, Permissions.StaffOvertime.CreateGuid, ExecutionTime, DbCMS);
                DbAHD.Update(_overtime, Permissions.StaffOvertime.CreateGuid, ExecutionTime, DbCMS);
                DbAHD.Update(toChange, Permissions.StaffOvertime.CreateGuid, ExecutionTime, DbCMS);
                DbAHD.SaveChanges();


                //var staffCore = DbAHD.StaffCoreData.Where(x => x.UserGUID == Entitlemnt.StaffGUID).FirstOrDefault();

                //if (staffCore.ReportToGUID != null)
                //{
                string _month = _overtime.Year.ToString() + " " + _overtime.Month.ToString();
                var _staffName = DbAHD.v_staffCoreDataOverview.Where(x => x.UserGUID == _overtime.UserGUID).FirstOrDefault();
                string SubjectMessage = resxEmails.OvertimeSubject.Replace("$StaffName", _staffName.FullName).Replace("$month", _month);


                //to send mail to staff 
                // var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();
                URL = AppSettingsKeys.Domain + "/AHD/StaffOvertime/StaffOvertimeConfirmByReviewer/?PK=" + new Portal().GUIDToString(_overtime.StaffOvertimeGUID);
                Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmDangerPayReceiving + "</a>";
                Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";



                string _message = resxEmails.OvertimeByReviewer
                    .Replace("$StaffName", _staffName.FullName.ToString())
                    .Replace("$month", _month)
                    .Replace("$VerifyLink", Anchor)

                    ;

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;

                Guid _overtimeReviewer = Guid.Parse("3260e610-8182-4c57-9412-8cb15315cce3");
                var tempPermGUIDs = DbCMS.userPermissions.Where(x => (x.ActionGUID == _overtimeReviewer && x.Active == true
                                 ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();
                var _userGuids = DbCMS.userProfiles.Where(x => tempPermGUIDs.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

                var _backupUsers = DbCMS.userServiceHistory.Where(x => _userGuids.Contains(x.UserGUID)
                && x.UserGUID != _overtime.UserGUID).Select(x => x.EmailAddress).Distinct().ToList();

                _backupUsers.Add(_staffName.EmailAddress);
                _backupUsers = _backupUsers.Distinct().ToList();
                string copyEmails = string.Join(" ;", _backupUsers);


                Guid _overtimeCertifier = Guid.Parse("b70578a3-3f0b-4299-8116-058bf8062311");
                var tempcertGUIDs = DbCMS.userPermissions.Where(x => (x.ActionGUID == _overtimeCertifier && x.Active == true

                                 ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();
                var _usercertGuids = DbCMS.userProfiles.Where(x => tempcertGUIDs.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

                var _backupcertUsers = DbCMS.userServiceHistory.Where(x => _usercertGuids.Contains(x.UserGUID)
                && x.UserGUID != _overtime.UserGUID).Select(x => x.EmailAddress).Distinct().ToList();
                _backupcertUsers = _backupcertUsers.Distinct().ToList();

                string copyCertEmails = string.Join(" ;", _backupcertUsers);



                //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
                //string copy_recipients = _staffName.EmailAddress;
                Send(copyCertEmails, SubjectMessage, _message, isRec, copyEmails);



            }

            try
            {
                dataStaffOvertime StaffOvertime = DbAHD.dataStaffOvertime.Where(x => x.StaffOvertimeGUID == model.StaffOvertimeGUID).FirstOrDefault();
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.StaffOvertimeDataTable, DbAHD.PrimaryKeyControl(StaffOvertime), DbAHD.RowVersionControls(Portal.SingleToList(StaffOvertime))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyStaffOvertime(model.StaffOvertimeGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffOvertimeCertifyingUpdate(StaffOvertimeUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.StaffOvertime.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            Guid overtimeGUID = model.StaffOvertimeGUID;
            string URL = "";
            string Anchor = "";
            string Link = "";
            DateTime ExecutionTime = DateTime.Now;
            var _overtime = DbAHD.dataStaffOvertime.Where(x => x.StaffOvertimeGUID == overtimeGUID).FirstOrDefault();

            if (_overtime.LastFlowStatusGUID == OvertimeFlowStatus.PendingCertifying)
            {
                var toChange = DbAHD.dataStaffOvertimeFlow.Where(x => x.StaffOvertimeGUID == overtimeGUID
                                  && x.IsLastAction == true
                                  ).FirstOrDefault();
                toChange.IsLastAction = false;
                dataStaffOvertimeFlow newFlowToReview = new dataStaffOvertimeFlow
                {

                    StaffOvertimeFlowGUID = Guid.NewGuid(),
                    StaffOvertimeGUID = overtimeGUID,
                    CreatedByGUID = UserGUID,
                    FlowStatusGUID = OvertimeFlowStatus.Approved,
                    ActionDate = ExecutionTime,
                    IsLastAction = true,
                    OrderID = toChange.OrderID + 1,


                };
                _overtime.LastFlowStatusGUID = OvertimeFlowStatus.Approved;


                DbAHD.Create(newFlowToReview, Permissions.StaffOvertime.CreateGuid, ExecutionTime, DbCMS);
                DbAHD.Update(_overtime, Permissions.StaffOvertime.CreateGuid, ExecutionTime, DbCMS);
                DbAHD.Update(toChange, Permissions.StaffOvertime.CreateGuid, ExecutionTime, DbCMS);
                DbAHD.SaveChanges();


                //var staffCore = DbAHD.StaffCoreData.Where(x => x.UserGUID == Entitlemnt.StaffGUID).FirstOrDefault();

                //if (staffCore.ReportToGUID != null)
                //{

                var _staffName = DbAHD.v_staffCoreDataOverview.Where(x => x.UserGUID == _overtime.UserGUID).FirstOrDefault();
                string SubjectMessage = resxEmails.OvertimeSubject.Replace("$StaffName", _staffName.FullName).Replace("$month", _overtime.Year + " " + _overtime.Month);


                //to send mail to staff 
                // var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();
                //URL = AppSettingsKeys.Domain + "/AHD/StaffOvertime/StaffOvertimeConfirmByReviewer/?PK=" + new Portal().GUIDToString(_overtime.StaffOvertimeGUID);
                //Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmDangerPayReceiving + "</a>";
                //Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";



                string _message = resxEmails.OvertimeApprovalConfirmation
                    .Replace("$StaffName", _staffName.FullName.ToString())


                    ;

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;

                Guid _overtimeReviewer = Guid.Parse("3260e610-8182-4c57-9412-8cb15315cce3");
                var tempPermGUIDs = DbCMS.userPermissions.Where(x => (x.ActionGUID == _overtimeReviewer && x.Active == true
                                 ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();
                var _userGuids = DbCMS.userProfiles.Where(x => tempPermGUIDs.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

                var _backupUsers = DbCMS.userServiceHistory.Where(x => _userGuids.Contains(x.UserGUID)).Select(x => x.EmailAddress).Distinct().ToList();

                //_backupUsers.Add(_staffName.EmailAddress);
                //string copyEmails = string.Join(" ;", _backupUsers);


                Guid _overtimeCertifier = Guid.Parse("b70578a3-3f0b-4299-8116-058bf8062311");
                var tempcertGUIDs = DbCMS.userPermissions.Where(x => (x.ActionGUID == _overtimeReviewer && x.Active == true
                                 ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();
                var _usercertGuids = DbCMS.userProfiles.Where(x => tempcertGUIDs.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

                var _backupcertUsers = DbCMS.userServiceHistory.Where(x => _usercertGuids.Contains(x.UserGUID)).Select(x => x.EmailAddress).Distinct().ToList();

                _backupUsers.AddRange(_backupcertUsers);
                string copyCertEmails = string.Join(" ;", _backupUsers);



                //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
                //string copy_recipients = _staffName.EmailAddress;
                Send(_staffName.EmailAddress, SubjectMessage, _message, isRec, copyCertEmails);



            }

            try
            {
                dataStaffOvertime StaffOvertime = DbAHD.dataStaffOvertime.Where(x => x.StaffOvertimeGUID == model.StaffOvertimeGUID).FirstOrDefault();
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.StaffOvertimeDataTable, DbAHD.PrimaryKeyControl(StaffOvertime), DbAHD.RowVersionControls(Portal.SingleToList(StaffOvertime))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyStaffOvertime(model.StaffOvertimeGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }


        #endregion

        #region My Information
        [Route("AHD/StaffOvertime/WorkplaceStaffOvertime/")]
        public ActionResult WorkplaceStaffOvertime(Guid? id)
        {
            if (id == null)
                id = UserGUID;
            var myModel = DbAHD.dataStaffOvertime.Where(x => x.UserGUID == id

                       ).FirstOrDefault();
            if (myModel == null || id != UserGUID)
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return View("~/Areas/AHD/Views/StaffOvertime/MyOvertime.cshtml", new StaffOvertimeUpdateModel { UserGUID = myModel.UserGUID });
        }

        public JsonResult StaffMyOvertimefDataTable(DataTableRecievedOptions options, Guid PK)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<StaffOvertimeDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<StaffOvertimeDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.OvertimeReviewForDrivers.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix




            var All = (
                from a in DbAHD.dataStaffOvertime.Where(x => x.UserGUID == PK).AsExpandable()
                join b in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.LastFlowStatusGUID equals b.ValueGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.CreatedByGUID equals c.UserGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()
                join d in DbAHD.v_StaffProfileInformation on a.UserGUID equals d.UserGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()
                select new StaffOvertimeDataTableModel
                {
                    StaffOvertimeGUID = a.StaffOvertimeGUID.ToString(),
                    Year = a.Year.ToString(),
                    Month = a.Month,
                    Active = a.Active,
                    ActionDate = a.ActionDate,
                    TimeIn = a.TimeIn,
                    TimeOut = a.TimeOut,
                    TotalHours = (float)a.TotalHours,
                    TotalPay = (float)a.TotalPay,
                    StaffName = R3.FullName,
                    DutyStation = R3.DutyStation,
                    JobTitle = R3.JobTitle,
                    CreatedByGUID = a.CreatedByGUID.ToString(),
                    LastFlowStatusGUID = a.LastFlowStatusGUID.ToString(),
                    LastFlowName = R1.ValueDescription,
                    CreatedBy = R2.FirstName + " " + R2.Surname,
                    //FlowStatusGUID = a.FlowStatusGUID,
                    //PaymentDurationName = a.PaymentDurationName,
                    //TotalStaffConfirm = a.dataStaffOvertime1.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Confirmed).Count(),
                    //TotalStaffNotConfirm = a.dataStaffOvertime1.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Pending).Count(),

                    //OrderId = a.OrderId,
                    //CreateDate = a.CreateDate,
                    dataStaffOvertimeRowVersion = a.dataStaffOvertimeRowVersion
                }).Where(Predicate);
            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<StaffOvertimeDataTableModel> Result = Mapper.Map<List<StaffOvertimeDataTableModel>>(All.OrderBy(x => x.StaffName).Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Calendar

        public ActionResult GetByStaffCycleOvertimeCalendarForReview(Guid PK)
        {


            StffCalendarOvertimeUpdateModel model = new StffCalendarOvertimeUpdateModel();
            var _staffCycleMonth = DbAHD.dataOvertimeMonthCycleStaff.Where(x => x.OvertimeMonthCycleStaffGUID == PK).FirstOrDefault();
            var _staffovertime = DbAHD.dataStaffOvertime.Where(x => x.UserGUID == _staffCycleMonth.UserGUID
            && x.OvertimeMonthCycleStaffGUID == _staffCycleMonth.OvertimeMonthCycleStaffGUID).FirstOrDefault();
            var _staffInfo = DbAHD.v_staffCoreDataOverview.Where(x => x.UserGUID == _staffCycleMonth.UserGUID).FirstOrDefault();
            model.UserGUID = (Guid)_staffCycleMonth.UserGUID;
            model.OvertimeMonthCycleGUID = (Guid)_staffCycleMonth.OvertimeMonthCycleGUID;
            model.OvertimeMonthCycleStaffGUID = _staffCycleMonth.OvertimeMonthCycleStaffGUID;



            //ViewBag.TotalHoursPayed = _staffCycleMonth != null ? _staffCycleMonth.TotalHoursPayed : 0;
            //ViewBag.TotalPerformedHours = _staffCycleMonth != null ? _staffCycleMonth.TotalHoursPayed : 0;

            //ViewBag.TotalPay = _staffCycleMonth != null ? _staffCycleMonth.TotalPay : 0;

            model.TotalHoursPayed = _staffCycleMonth != null ? _staffCycleMonth.TotalHoursPayed : 0;
            model.TotalPerformedHours = _staffCycleMonth != null ? _staffCycleMonth.TotalPerformedHours : 0;
            model.TotalPay = _staffCycleMonth != null ? _staffCycleMonth.TotalPay : 0;

            model.Grade = _staffInfo.Grade;
            model.JobTitle = _staffInfo.JobTitle;
            model.FullName = _staffInfo.FullName;
            //model.Step = DbAHD.codeTablesValuesLanguages.Where(x => x.ValueGUID == _staffovertime.StepGUID && 
            //x.LanguageID == LAN && x.Active).FirstOrDefault().ValueDescription;


            model.FirstDateInMonth = _staffCycleMonth.dataOvertimeMonthCycle.StartMonthDate;
            ViewBag.hasCycleValue = 1;
            model.JobTitleGUID = _staffInfo.JobTitleGUID;
            model.FlowStatusGUID = _staffCycleMonth.LastFlowStatusGUID;
            return View("~/Areas/AHD/Views/StaffOvertime/Calendar/ReviewStaffOverTimeCalendar.cshtml", model);
        }

        public ActionResult StaffOvertimeCalendarByStaff(Guid PK)
        {


            StffCalendarOvertimeUpdateModel model = new StffCalendarOvertimeUpdateModel();

            model.UserGUID = UserGUID;
            model.OvertimeMonthCycleGUID = PK;

            var _staffCycleMonth = DbAHD.dataOvertimeMonthCycleStaff.Where(x => x.UserGUID == UserGUID && x.OvertimeMonthCycleGUID == PK).FirstOrDefault();

            //ViewBag.TotalHoursPayed = _staffCycleMonth != null ? _staffCycleMonth.TotalHoursPayed : 0;
            //ViewBag.TotalPerformedHours = _staffCycleMonth != null ? _staffCycleMonth.TotalHoursPayed : 0;
            //ViewBag.TotalPay = _staffCycleMonth != null ? _staffCycleMonth.TotalPay : 0;

            model.TotalHoursPayed = _staffCycleMonth != null ? _staffCycleMonth.TotalHoursPayed : 0;
            model.TotalPerformedHours = _staffCycleMonth != null ? _staffCycleMonth.TotalPerformedHours : 0;
            model.TotalPay = _staffCycleMonth != null ? _staffCycleMonth.TotalPay : 0;

            ViewBag.hasCycleValue = 1;

            model.FirstDateInMonth = DbAHD.dataOvertimeMonthCycle.Where(x => x.OvertimeMonthCycleGUID == PK).FirstOrDefault().StartMonthDate;

            model.FlowStatusGUID = _staffCycleMonth != null ? _staffCycleMonth.LastFlowStatusGUID : OvertimeFlowStatus.Submitted;
            model.OvertimeMonthCycleStaffGUID = _staffCycleMonth != null ? _staffCycleMonth.OvertimeMonthCycleStaffGUID : Guid.Empty;
            return View("~/Areas/AHD/Views/StaffOvertime/Calendar/StaffOverTimeCalendar.cshtml", model);
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

            var userProfiles = DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            var Events = (from a in DbAHD.dataStaffOvertime
                          .Where(x => x.TimeIn >= start && x.TimeOut <= end && x.Active).ToList()
                              //.Where(x => AuthorizedListDepartment.Contains(x.codeAppointmentType.DepartmentGUID.ToString()))
                              //.Where(x => AuthorizedListDutyStation.Contains(x.DutyStationGUID.ToString()))
                          join b in DbAHD.v_StaffProfileInformation on a.UserGUID equals b.UserGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          join c in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DayWorkingTypeGUID equals c.ValueGUID into LJ2

                          from R2 in LJ2.DefaultIfEmpty()
                          select new CalendarEvents
                          {
                              EventId = a.StaffOvertimeGUID,
                              UserGUID = a.UserGUID,

                              EventStartDate = a.TimeIn,
                              EventEndDate = a.TimeOut,
                              Title = R1.FullName + " " + R2.ValueDescription,
                              EventDescription = a.OvertimeReason,
                              AllDayEvent = false,
                              backgroundColor = R2.ValueGUID == Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E7832") ? "#42d4f5" :
                                        R2.ValueGUID == Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E7835") ? "#4287f5" : "#f54242",


                          }).ToList();
            return Json(new { CalendarEvents = Events }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetCalendarDataFromDatabaseFilterByUser(DateTime start, DateTime end, Guid staffGUID, Guid _overtimeMonthCycleGUID)
        {



            // var userProfiles = DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            var Events = (from a in DbAHD.dataStaffOvertime.Where(x => x.UserGUID == staffGUID)
                          .Where(x => x.TimeIn >= start && x.TimeOut <= end && x.Active).ToList()
                              //.Where(x => AuthorizedListDepartment.Contains(x.codeAppointmentType.DepartmentGUID.ToString()))
                              //.Where(x => AuthorizedListDutyStation.Contains(x.DutyStationGUID.ToString()))
                          join b in DbAHD.v_StaffProfileInformation on a.UserGUID equals b.UserGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          join c in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DayWorkingTypeGUID equals c.ValueGUID into LJ2

                          from R2 in LJ2.DefaultIfEmpty()
                          select new CalendarEvents
                          {
                              EventId = a.StaffOvertimeGUID,
                              UserGUID = a.UserGUID,

                              EventStartDate = a.TimeIn,
                              EventEndDate = a.TimeOut,

                              //Title ="Hours : "+a.TotalHours +" Payed: "+ Math.Round((Double)a.TotalPay, 2),
                              Title = "Hours : " + a.TotalHours + " In: " + a.TimeIn.Hour + ":" + a.TimeIn.Minute + " Out:" + a.TimeOut.Hour + ":" + a.TimeOut.Minute,
                              EventDescription = a.OvertimeReason,
                              AllDayEvent = false,

                              backgroundColor = R2.ValueGUID == Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E7832") ? "#42d4f5" :
                                        R2.ValueGUID == Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E7835") ? "#4287f5" : "#f54242",


                          }).ToList();
            return Json(new { CalendarEvents = Events }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult OvertimeUpdateDropEvent(Guid PK, DateTime? startDate, DateTime? endDate)
        {
            //if (!CMS.HasAction(Permissions.InternationalStaffAttendancePresence.Access, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}

            var model = DbAHD.dataStaffOvertime.Find(PK);
            model.TimeIn = startDate.Value;
            model.TimeOut = endDate.Value;
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

        #endregion

        #region Cycle Month 





        [Route("AHD/StaffOvertime/WorkplaceMonthCycleOvertimeIndex/")]
        public ActionResult WorkplaceMonthCycleOvertimeIndex()
        {
            //if (!CMS.HasAction(Permissions.StaffOvertime.Access, Apps.AHD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            DateTime ExecutionTime = DateTime.Now;
            var check = DbAHD.dataOvertimeMonthCycle.Where(x => x.StartMonthDate <= ExecutionTime && x.EndMonthDate >= ExecutionTime).FirstOrDefault();
            if (check == null)
            {
                var firstDayOfMonth = new DateTime(ExecutionTime.Year, ExecutionTime.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                var _max = DbAHD.dataOvertimeMonthCycle.Select(x => x.OrderID).Max();
                dataOvertimeMonthCycle toAdd = new dataOvertimeMonthCycle
                {
                    OvertimeMonthCycleGUID = Guid.NewGuid(),
                    Year = ExecutionTime.Year,
                    Month = ProcessData.GetMonthName(ExecutionTime.Month),
                    StartMonthDate = firstDayOfMonth,
                    EndMonthDate = lastDayOfMonth,

                    MonthCycleStatusGUID = AHDActionFlowStatus.Pending,
                    OrderID = _max == null ? 1 : _max + 1

                };
                DbAHD.CreateNoAudit(toAdd);
                try
                {
                    DbAHD.SaveChanges();
                    DbCMS.SaveChanges();

                }
                catch (Exception ex)
                {
                    return Json(DbAHD.ErrorMessage(ex.Message));
                }
            }

            return View("~/Areas/AHD/Views//StaffOvertime/MonthCycles/MonthCyclesIndex.cshtml");
        }


        //[Route("AHD/MonthCycleOvertimeIndex/")]
        //public ActionResult MonthCycleOvertimeIndex()
        //{

        //}
        [Route("AHD/OvertimeMonthCycleDataTable/")]
        public JsonResult OvertimeMonthCycleDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<OvertimeMonthCycleDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<OvertimeMonthCycleDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.StaffOvertime.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix

            var All = (
                from a in DbAHD.dataOvertimeMonthCycle.Where(x => x.Active).AsExpandable()
                join b in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.MonthCycleStatusGUID equals b.ValueGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                    //join c in DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.CreatedByGUID equals c.UserGUID into LJ2
                    //from R2 in LJ2.DefaultIfEmpty()
                    //join d in DbAHD.v_StaffProfileInformation on a.UserGUID equals d.UserGUID into LJ3
                    //from R3 in LJ3.DefaultIfEmpty()
                    //join e in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.StepGUID equals e.ValueGUID into LJ4
                    //from R4 in LJ4.DefaultIfEmpty()
                select new OvertimeMonthCycleDataTableModel
                {
                    OvertimeMonthCycleGUID = a.OvertimeMonthCycleGUID.ToString(),
                    Year = a.Year.ToString(),
                    Month = a.Month,
                    CycleName = a.Month + " " + a.Year.ToString(),
                    OrderID = a.OrderID,
                    Status = R1.ValueDescription,

                    dataOvertimeMonthCycleRowVersion = a.dataOvertimeMonthCycleRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<OvertimeMonthCycleDataTableModel> Result = Mapper.Map<List<OvertimeMonthCycleDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).OrderByDescending(x => x.OrderID).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }




        [Route("AHD/AllOvertimeMonthCycleDataTable/")]
        public JsonResult AllOvertimeMonthCycleDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<OvertimeMonthCycleDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<OvertimeMonthCycleDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.StaffOvertime.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix

            var All = (
                from a in DbAHD.dataOvertimeMonthCycle.Where(x => x.Active).AsExpandable()
                join b in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.MonthCycleStatusGUID equals b.ValueGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                    //join c in DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.CreatedByGUID equals c.UserGUID into LJ2
                    //from R2 in LJ2.DefaultIfEmpty()
                    //join d in DbAHD.v_StaffProfileInformation on a.UserGUID equals d.UserGUID into LJ3
                    //from R3 in LJ3.DefaultIfEmpty()
                    //join e in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.StepGUID equals e.ValueGUID into LJ4
                    //from R4 in LJ4.DefaultIfEmpty()
                select new OvertimeMonthCycleDataTableModel
                {
                    OvertimeMonthCycleGUID = a.OvertimeMonthCycleGUID.ToString(),
                    Year = a.Year.ToString(),
                    Month = a.Month,
                    CycleName = a.Month + " " + a.Year.ToString(),
                    OrderID = a.OrderID,
                    Status = R1.ValueDescription,

                    dataOvertimeMonthCycleRowVersion = a.dataOvertimeMonthCycleRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<OvertimeMonthCycleDataTableModel> Result = Mapper.Map<List<OvertimeMonthCycleDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).OrderByDescending(x => x.OrderID).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetOvertimeAllStaffIndex(Guid PK)
        {


            OvertimeMonthCycleUpdateModel model = new OvertimeMonthCycleUpdateModel();
            var myModel = DbAHD.dataOvertimeMonthCycle.Where(x => x.OvertimeMonthCycleGUID == PK).FirstOrDefault();
            model.OvertimeMonthCycleGUID = PK;
            model.CycleName = myModel.Month + " " + myModel.Year;
            model.Active = myModel.Active;


            return View("~/Areas/AHD/Views/StaffOvertime/StaffCycle/StaffCycleIndex.cshtml", model);
        }

        public JsonResult AllStaffCycleOvertimeMonthDataTable(DataTableRecievedOptions options, Guid PK)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<OvertimeMonthCycleStaffDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<OvertimeMonthCycleStaffDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.StaffSalaryProcess.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix

            var All = (
                from a in DbAHD.dataOvertimeMonthCycleStaff.Where(x => x.OvertimeMonthCycleGUID == PK).AsExpandable()
                join b in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.LastFlowStatusGUID equals b.ValueGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.UserGUID equals c.UserGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()
                join d in DbAHD.v_StaffProfileInformation on a.UserGUID equals d.UserGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()
                join e in DbAHD.dataOvertimeMonthCycle on a.OvertimeMonthCycleGUID equals e.OvertimeMonthCycleGUID into LJ4
                from R4 in LJ4.DefaultIfEmpty()

                select new OvertimeMonthCycleStaffDataTableModel
                {
                    OvertimeMonthCycleStaffGUID = a.OvertimeMonthCycleStaffGUID.ToString(),
                    UserGUID = a.UserGUID.ToString(),
                    CycleName = R4.Year + " " + R4.Month,
                    StaffName = R3.FullName,
                    TotalHoursPayed = a.TotalHoursPayed,
                    TotalPay = a.TotalPay,
                    TotalPerformedHours = a.TotalPerformedHours,
                    OvertimeMonthCycleGUID = a.OvertimeMonthCycleGUID.ToString(),
                    Active = a.Active,
                    OrderID = R4.OrderID,
                    LastFlowStatusGUID = a.LastFlowStatusGUID.ToString(),
                    Status = R1.ValueDescription,


                    dataOvertimeMonthCycleStaffRowVersion = a.dataOvertimeMonthCycleStaffRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<OvertimeMonthCycleStaffDataTableModel> Result = Mapper.Map<List<OvertimeMonthCycleStaffDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).OrderBy(x => x.OrderID).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Approve Overtime
        public JsonResult SendOvertimeToReviewByManager(Guid _OvertimeMonthCycleStaffGUID)
        {
            #region Send Mail
            string URL = "";
            string Anchor = "";
            string Link = "";
            //string _month = "2020 Dec";

            var _staffCycle = DbAHD.dataOvertimeMonthCycleStaff.Where(x => x.OvertimeMonthCycleStaffGUID == _OvertimeMonthCycleStaffGUID).FirstOrDefault();
            string _month = _staffCycle.dataOvertimeMonthCycle.Month.ToString() + " " + _staffCycle.dataOvertimeMonthCycle.Year.ToString();
            var _staffName = DbAHD.v_staffCoreDataOverview.Where(x => x.UserGUID == _staffCycle.UserGUID).FirstOrDefault();
            string SubjectMessage = resxEmails.OvertimeSubject.Replace("$StaffName", _staffName.FullName).Replace("$month", _month);

            _staffCycle.LastFlowStatusGUID = OvertimeFlowStatus.PendingSupervisorReview;








            DbAHD.UpdateNoAudit(_staffCycle);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();

            }
            catch (Exception)
            {

                throw;
            }

            //to send mail to staff 
            // var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();
            URL = AppSettingsKeys.Domain + "/AHD/StaffOvertime/StaffOvertimeConfirmByReviewer/?PK=" + new Portal().GUIDToString(_staffCycle.OvertimeMonthCycleStaffGUID);
            Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmDangerPayReceiving + "</a>";
            Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";



            string _message = resxEmails.OvertimeByReviewer
                .Replace("$StaffName", _staffName.FullName.ToString())
                .Replace("$month", _month)

                .Replace("$VerifyLink", Anchor)

                ;

            if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
            int isRec = 1;
            if (_staffName.JobTitleGUID == Guid.Parse("7FFA5C6F-FA42-4DC4-A072-2BD5C4ED53C8"))
            {
                Guid _overtimeReviewer = Guid.Parse("3260e610-8182-4c57-9412-8cb15315cce3");
                var tempPermGUIDs = DbCMS.userPermissions.Where(x => (x.ActionGUID == _overtimeReviewer && x.Active == true
                                 ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();
                var _userGuids = DbCMS.userProfiles.Where(x => tempPermGUIDs.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

                var _backupUsers = DbCMS.userServiceHistory.Where(x => _userGuids.Contains(x.UserGUID)).Select(x => x.EmailAddress).Distinct().ToList();


                string copyEmails = string.Join(" ;", _backupUsers);


                //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
                string copy_recipients = _staffName.EmailAddress;
                Send(copyEmails, SubjectMessage, _message, isRec, copy_recipients);
            }
            else
            {
                var _rep = DbAHD.StaffCoreData.Where(x => x.UserGUID == _staffName.UserGUID).FirstOrDefault();
                var repTo = DbAHD.StaffCoreData.Where(x => x.UserGUID == _rep.ReportToGUID).FirstOrDefault();
                string copy_recipients = _staffName.EmailAddress;
                Send(repTo.EmailAddress, SubjectMessage, _message, isRec, copy_recipients);
            }


            #endregion
            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
        }




        public JsonResult ApproveDriverSupervisorOvertime(Guid _OvertimeMonthCycleStaffGUID)
        {
            StffCalendarOvertimeUpdateModel model = new StffCalendarOvertimeUpdateModel();
            var _MonthCycleStaff = DbAHD.dataOvertimeMonthCycleStaff.Where(x => x.OvertimeMonthCycleStaffGUID == _OvertimeMonthCycleStaffGUID).FirstOrDefault();
            if (!CMS.HasAction(Permissions.OvertimeReviewForDrivers.Confirm, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }


            //if (!CMS.HasAction(Permissions.OvertimeReviewForDriversCertifying.Create, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            string URL = "";
            string Anchor = "";
            string Link = "";
            DateTime ExecutionTime = DateTime.Now;

            if (_MonthCycleStaff.LastFlowStatusGUID == OvertimeFlowStatus.PendingSupervisorReview)
            {
                //var toChange = DbAHD.dataOvertimeMonthCycleStaffFlow.Where(x => x.OvertimeMonthCycleStaffGUID == _OvertimeMonthCycleStaffGUID
                //                  && x.IsLastAction == true
                //                  ).FirstOrDefault();
                //toChange.IsLastAction = false;
                dataOvertimeMonthCycleStaffFlow newFlowToReview = new dataOvertimeMonthCycleStaffFlow
                {

                    OvertimeMonthCycleStaffFlowGUID = Guid.NewGuid(),
                    OvertimeMonthCycleStaffGUID = _OvertimeMonthCycleStaffGUID,
                    CreatedByGUID = UserGUID,
                    FlowStatusGUID = OvertimeFlowStatus.PendingCertifying,
                    ActionDate = ExecutionTime,
                    IsLastAction = true,
                    OrderID = 1,


                };
                _MonthCycleStaff.LastFlowStatusGUID = OvertimeFlowStatus.PendingCertifying;
                _MonthCycleStaff.ApprovedByGUID = UserGUID;
                _MonthCycleStaff.ApprovedDate = ExecutionTime;






                DbAHD.Create(newFlowToReview, Permissions.OvertimeReviewForDrivers.ConfirmGuid, ExecutionTime, DbCMS);
                DbAHD.Update(_MonthCycleStaff, Permissions.OvertimeReviewForDrivers.ConfirmGuid, ExecutionTime, DbCMS);
                //DbAHD.Update(toChange, Permissions.OvertimeReviewForDrivers.ConfirmGuid, ExecutionTime, DbCMS);
                DbAHD.SaveChanges();



                //var staffCore = DbAHD.StaffCoreData.Where(x => x.UserGUID == Entitlemnt.StaffGUID).FirstOrDefault();

                //if (staffCore.ReportToGUID != null)
                //{

                var currStaff = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == _MonthCycleStaff.UserGUID
                                                                                  && x.LanguageID == LAN).FirstOrDefault();
                Guid _CertifiedPerGUID = Guid.Parse("c86dc3b1-898b-4a05-8f7f-946d342ad2c1");
                var tempPermGUIDs = DbCMS.userPermissions.Where(x => (x.ActionGUID == _CertifiedPerGUID && x.Active == true
                                 ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();
                var _userGuids = DbCMS.userProfiles.Where(x => tempPermGUIDs.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

                var _backupUsers = DbCMS.userServiceHistory.Where(x => _userGuids.Contains(x.UserGUID)).Select(x => x.EmailAddress).Distinct().ToList();


                string copyEmails = string.Join(" ;", _backupUsers);



                //var _currManagerPersonal = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.FinanceApprovedByGUID
                //                                                               && x.LanguageID == LAN).FirstOrDefault();
                //f
                //var currAccount = DbAHD.userServiceHistory.Where(x => x.UserGUID == Entitlemnt.FinanceApprovedByGUID).FirstOrDefault();

                //var _currManagerAccount = DbAHD.userServiceHistory.Where(x => x.UserGUID == staffCore.ReportToGUID).ToList();

                var _monthName = _MonthCycleStaff.dataOvertimeMonthCycle.Month + " " + _MonthCycleStaff.dataOvertimeMonthCycle.Year;
                var _staff = DbAHD.v_staffCoreDataOverview.Where(x => x.UserGUID == _MonthCycleStaff.UserGUID).FirstOrDefault();
                var _issuer = DbAHD.v_staffCoreDataOverview.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
                string SubjectMessage = resxEmails.OvertimeSubject.Replace("$StaffName", _staff.FullName).Replace("$month", _monthName);



                //to send mail to staff 
                // var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();
                URL = AppSettingsKeys.Domain + "/AHD/StaffOvertime/StaffOvertimeConfirmByReviewer/?PK=" + new Portal().GUIDToString(_MonthCycleStaff.OvertimeMonthCycleStaffGUID);
                //URL = AppSettingsKeys.Domain + "/AHD/Reports/PrintInternationalEntitlementReport/?PK=" + new Portal().GUIDToString(_MonthCycleStaff.OvertimeMonthCycleStaffGUID);
                Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmDangerPayReceiving + "</a>";
                Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                //string _financeFirstName = _currManagerPersonal.FirstName;
                //string _financeSurName = _currManagerPersonal.Surname;


                string _message = resxEmails.OvertimeByReviewer
                                  .Replace("$StaffName", _staff.FullName.ToString())
                                  .Replace("$month", _monthName)

                                  .Replace("$VerifyLink", Anchor)

                                  ;

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
                string copy_recipients = _issuer.EmailAddress + ";" + _staff.EmailAddress;
                Send(copyEmails, SubjectMessage, _message, isRec, copy_recipients);
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);


            }

            //}
            return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ApproveDriverAuthroizorOvertime(Guid _OvertimeMonthCycleStaffGUID)
        {
            StffCalendarOvertimeUpdateModel model = new StffCalendarOvertimeUpdateModel();
            var _MonthCycleStaff = DbAHD.dataOvertimeMonthCycleStaff.Where(x => x.OvertimeMonthCycleStaffGUID == _OvertimeMonthCycleStaffGUID).FirstOrDefault();
            if (!CMS.HasAction(Permissions.OvertimeReviewForDrivers.ValidateData, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }


            //if (!CMS.HasAction(Permissions.OvertimeReviewForDriversCertifying.Create, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            string URL = "";
            string Anchor = "";
            string Link = "";
            DateTime ExecutionTime = DateTime.Now;

            if (_MonthCycleStaff.LastFlowStatusGUID == OvertimeFlowStatus.PendingCertifying)
            {
                var toChange = DbAHD.dataOvertimeMonthCycleStaffFlow.Where(x => x.OvertimeMonthCycleStaffGUID == _OvertimeMonthCycleStaffGUID
                                  && x.IsLastAction == true
                                  ).FirstOrDefault();
                toChange.IsLastAction = false;
                dataOvertimeMonthCycleStaffFlow newFlowToReview = new dataOvertimeMonthCycleStaffFlow
                {

                    OvertimeMonthCycleStaffFlowGUID = Guid.NewGuid(),
                    OvertimeMonthCycleStaffGUID = _OvertimeMonthCycleStaffGUID,
                    CreatedByGUID = UserGUID,
                    FlowStatusGUID = OvertimeFlowStatus.Approved,
                    ActionDate = ExecutionTime,
                    IsLastAction = true,
                    OrderID = toChange.OrderID + 1,


                };
                _MonthCycleStaff.LastFlowStatusGUID = OvertimeFlowStatus.Approved;
                _MonthCycleStaff.CertifiedByGUID = UserGUID;
                _MonthCycleStaff.AuthorizedDate = ExecutionTime;






                DbAHD.Create(newFlowToReview, Permissions.OvertimeReviewForDrivers.ValidateDataGuid, ExecutionTime, DbCMS);
                DbAHD.Update(_MonthCycleStaff, Permissions.OvertimeReviewForDrivers.ValidateDataGuid, ExecutionTime, DbCMS);
                DbAHD.Update(toChange, Permissions.OvertimeReviewForDrivers.ValidateDataGuid, ExecutionTime, DbCMS);
                DbAHD.SaveChanges();



                //var staffCore = DbAHD.StaffCoreData.Where(x => x.UserGUID == Entitlemnt.StaffGUID).FirstOrDefault();

                //if (staffCore.ReportToGUID != null)
                //{

                var currStaff = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == _MonthCycleStaff.UserGUID
                                                                                  && x.LanguageID == LAN).FirstOrDefault();
                Guid _CertifiedPerGUID = Guid.Parse("c86dc3b1-898b-4a05-8f7f-946d342ad2c1");
                var tempPermGUIDs = DbCMS.userPermissions.Where(x => (x.ActionGUID == _CertifiedPerGUID && x.Active == true
                                 ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();
                var _userGuids = DbCMS.userProfiles.Where(x => tempPermGUIDs.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

                var _backupUsers = DbCMS.userServiceHistory.Where(x => _userGuids.Contains(x.UserGUID)).Select(x => x.EmailAddress).Distinct().ToList();


                var _ApproverUser = DbCMS.userServiceHistory.Where(x => x.UserGUID == _MonthCycleStaff.UserGUID).Select(x => x.EmailAddress).Distinct().FirstOrDefault();
                _backupUsers.Add(_ApproverUser);
                string copyEmails = string.Join(" ;", _backupUsers);



                //var _currManagerPersonal = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.FinanceApprovedByGUID
                //                                                               && x.LanguageID == LAN).FirstOrDefault();
                //f
                //var currAccount = DbAHD.userServiceHistory.Where(x => x.UserGUID == Entitlemnt.FinanceApprovedByGUID).FirstOrDefault();

                //var _currManagerAccount = DbAHD.userServiceHistory.Where(x => x.UserGUID == staffCore.ReportToGUID).ToList();

                var _monthName = _MonthCycleStaff.dataOvertimeMonthCycle.Month + " " + _MonthCycleStaff.dataOvertimeMonthCycle.Year;
                var _staff = DbAHD.v_staffCoreDataOverview.Where(x => x.UserGUID == _MonthCycleStaff.UserGUID).FirstOrDefault();
                var _issuer = DbAHD.v_staffCoreDataOverview.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
                string SubjectMessage = resxEmails.OvertimeSubject.Replace("$StaffName", _staff.FullName).Replace("$month", _monthName);



                //to send mail to staff 
                // var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();
                //URL = AppSettingsKeys.Domain + "/AHD/StaffOvertime/StaffOvertimeConfirmByReviewer/?PK=" + new Portal().GUIDToString(_MonthCycleStaff.OvertimeMonthCycleStaffGUID);
                ////URL = AppSettingsKeys.Domain + "/AHD/Reports/PrintInternationalEntitlementReport/?PK=" + new Portal().GUIDToString(_MonthCycleStaff.OvertimeMonthCycleStaffGUID);
                //Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmDangerPayReceiving + "</a>";
                //Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                //string _financeFirstName = _currManagerPersonal.FirstName;
                //string _financeSurName = _currManagerPersonal.Surname;


                string _message = resxEmails.OvertimeApprovalConfirmation
                                  .Replace("$StaffName", _staff.FullName.ToString())
                                  .Replace("$month", _monthName)

                                  //.Replace("$VerifyLink", Anchor)

                                  ;

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
                //string copy_recipients =_staff.EmailAddress;
                Send(_staff.EmailAddress, SubjectMessage, _message, isRec, copyEmails);
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);

            }

            //}
            return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ApproveStaffSupervisorOvertime(Guid _OvertimeMonthCycleStaffGUID)
        {
            StffCalendarOvertimeUpdateModel model = new StffCalendarOvertimeUpdateModel();
            var _MonthCycleStaff = DbAHD.dataOvertimeMonthCycleStaff.Where(x => x.OvertimeMonthCycleStaffGUID == _OvertimeMonthCycleStaffGUID).FirstOrDefault();
            if (!CMS.HasAction(Permissions.OvertimeReviewForStaff.Confirm, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }


            //if (!CMS.HasAction(Permissions.OvertimeReviewForDriversCertifying.Create, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            string URL = "";
            string Anchor = "";
            string Link = "";
            DateTime ExecutionTime = DateTime.Now;

            if (_MonthCycleStaff.LastFlowStatusGUID == OvertimeFlowStatus.PendingSupervisorReview)
            {
                //var toChange = DbAHD.dataOvertimeMonthCycleStaffFlow.Where(x => x.OvertimeMonthCycleStaffGUID == _OvertimeMonthCycleStaffGUID
                //                  && x.IsLastAction == true
                //                  ).FirstOrDefault();
                //toChange.IsLastAction = false;
                dataOvertimeMonthCycleStaffFlow newFlowToReview = new dataOvertimeMonthCycleStaffFlow
                {

                    OvertimeMonthCycleStaffFlowGUID = Guid.NewGuid(),
                    OvertimeMonthCycleStaffGUID = _OvertimeMonthCycleStaffGUID,
                    CreatedByGUID = UserGUID,
                    FlowStatusGUID = OvertimeFlowStatus.PendingCertifying,
                    ActionDate = ExecutionTime,
                    IsLastAction = true,
                    OrderID = 1,


                };
                _MonthCycleStaff.LastFlowStatusGUID = OvertimeFlowStatus.PendingCertifying;
                _MonthCycleStaff.ApprovedByGUID = UserGUID;
                _MonthCycleStaff.ApprovedDate = ExecutionTime;






                DbAHD.Create(newFlowToReview, Permissions.OvertimeReviewForStaff.ConfirmGuid, ExecutionTime, DbCMS);
                DbAHD.Update(_MonthCycleStaff, Permissions.OvertimeReviewForStaff.ConfirmGuid, ExecutionTime, DbCMS);
                // DbAHD.Update(toChange, Permissions.OvertimeReviewForStaff.ConfirmGuid, ExecutionTime, DbCMS);
                DbAHD.SaveChanges();



                //var staffCore = DbAHD.StaffCoreData.Where(x => x.UserGUID == Entitlemnt.StaffGUID).FirstOrDefault();

                //if (staffCore.ReportToGUID != null)
                //{

                var currStaff = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == _MonthCycleStaff.UserGUID
                                                                                  && x.LanguageID == LAN).FirstOrDefault();
                Guid _CertifiedPerGUID = Guid.Parse("f6c90666-4258-43c2-8125-fe95a6f9056f");
                var tempPermGUIDs = DbCMS.userPermissions.Where(x => (x.ActionGUID == _CertifiedPerGUID && x.Active == true
                                 ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();
                var _userGuids = DbCMS.userProfiles.Where(x => tempPermGUIDs.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

                var _backupUsers = DbCMS.userServiceHistory.Where(x => _userGuids.Contains(x.UserGUID)).Select(x => x.EmailAddress).Distinct().ToList();


                string copyEmails = string.Join(" ;", _backupUsers);



                //var _currManagerPersonal = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.FinanceApprovedByGUID
                //                                                               && x.LanguageID == LAN).FirstOrDefault();
                //f
                //var currAccount = DbAHD.userServiceHistory.Where(x => x.UserGUID == Entitlemnt.FinanceApprovedByGUID).FirstOrDefault();

                //var _currManagerAccount = DbAHD.userServiceHistory.Where(x => x.UserGUID == staffCore.ReportToGUID).ToList();

                var _monthName = _MonthCycleStaff.dataOvertimeMonthCycle.Month + " " + _MonthCycleStaff.dataOvertimeMonthCycle.Year;
                var _staff = DbAHD.v_staffCoreDataOverview.Where(x => x.UserGUID == _MonthCycleStaff.UserGUID).FirstOrDefault();
                var _issuer = DbAHD.v_staffCoreDataOverview.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
                string SubjectMessage = resxEmails.OvertimeSubject.Replace("$StaffName", _staff.FullName).Replace("$month", _monthName);



                //to send mail to staff 
                // var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();
                URL = AppSettingsKeys.Domain + "/AHD/StaffOvertime/StaffOvertimeConfirmByReviewer/?PK=" + new Portal().GUIDToString(_MonthCycleStaff.OvertimeMonthCycleStaffGUID);
                //URL = AppSettingsKeys.Domain + "/AHD/Reports/PrintInternationalEntitlementReport/?PK=" + new Portal().GUIDToString(_MonthCycleStaff.OvertimeMonthCycleStaffGUID);
                Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmDangerPayReceiving + "</a>";
                Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                //string _financeFirstName = _currManagerPersonal.FirstName;
                //string _financeSurName = _currManagerPersonal.Surname;


                string _message = resxEmails.OvertimeByReviewer
                                  .Replace("$StaffName", _staff.FullName.ToString())
                                  .Replace("$month", _monthName)

                                  .Replace("$VerifyLink", Anchor)

                                  ;

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
                string copy_recipients = _issuer.EmailAddress + ";" + _staff.EmailAddress;
                Send(copyEmails, SubjectMessage, _message, isRec, copy_recipients);
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);

            }

            //}
            return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ApproveStaffAuthroizorOvertime(Guid _OvertimeMonthCycleStaffGUID)
        {
            StffCalendarOvertimeUpdateModel model = new StffCalendarOvertimeUpdateModel();
            var _MonthCycleStaff = DbAHD.dataOvertimeMonthCycleStaff.Where(x => x.OvertimeMonthCycleStaffGUID == _OvertimeMonthCycleStaffGUID).FirstOrDefault();
            if (!CMS.HasAction(Permissions.OvertimeReviewForStaff.ValidateData, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }


            //if (!CMS.HasAction(Permissions.OvertimeReviewForDriversCertifying.Create, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            string URL = "";
            string Anchor = "";
            string Link = "";
            DateTime ExecutionTime = DateTime.Now;

            if (_MonthCycleStaff.LastFlowStatusGUID == OvertimeFlowStatus.PendingCertifying)
            {
                var toChange = DbAHD.dataOvertimeMonthCycleStaffFlow.Where(x => x.OvertimeMonthCycleStaffGUID == _OvertimeMonthCycleStaffGUID
                                  && x.IsLastAction == true
                                  ).FirstOrDefault();
                toChange.IsLastAction = false;
                dataOvertimeMonthCycleStaffFlow newFlowToReview = new dataOvertimeMonthCycleStaffFlow
                {

                    OvertimeMonthCycleStaffFlowGUID = Guid.NewGuid(),
                    OvertimeMonthCycleStaffGUID = _OvertimeMonthCycleStaffGUID,
                    CreatedByGUID = UserGUID,
                    FlowStatusGUID = OvertimeFlowStatus.Approved,
                    ActionDate = ExecutionTime,
                    IsLastAction = true,
                    OrderID = toChange.OrderID + 1,


                };
                _MonthCycleStaff.LastFlowStatusGUID = OvertimeFlowStatus.Approved;
                _MonthCycleStaff.CertifiedByGUID = UserGUID;
                _MonthCycleStaff.AuthorizedDate = ExecutionTime;






                DbAHD.Create(newFlowToReview, Permissions.OvertimeReviewForStaff.ValidateDataGuid, ExecutionTime, DbCMS);
                DbAHD.Update(_MonthCycleStaff, Permissions.OvertimeReviewForStaff.ValidateDataGuid, ExecutionTime, DbCMS);
                DbAHD.Update(toChange, Permissions.OvertimeReviewForStaff.ValidateDataGuid, ExecutionTime, DbCMS);
                DbAHD.SaveChanges();



                //var staffCore = DbAHD.StaffCoreData.Where(x => x.UserGUID == Entitlemnt.StaffGUID).FirstOrDefault();

                //if (staffCore.ReportToGUID != null)
                //{

                var currStaff = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == _MonthCycleStaff.UserGUID
                                                                                  && x.LanguageID == LAN).FirstOrDefault();
                Guid _CertifiedPerGUID = Guid.Parse("a0251338-f05d-4185-8c20-e7ed70127ac5");
                var tempPermGUIDs = DbCMS.userPermissions.Where(x => (x.ActionGUID == _CertifiedPerGUID && x.Active == true
                                 ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();
                var _userGuids = DbCMS.userProfiles.Where(x => tempPermGUIDs.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

                var _backupUsers = DbCMS.userServiceHistory.Where(x => _userGuids.Contains(x.UserGUID)).Select(x => x.EmailAddress).Distinct().ToList();


                var _ApproverUser = DbCMS.userServiceHistory.Where(x => x.UserGUID == _MonthCycleStaff.UserGUID).Select(x => x.EmailAddress).Distinct().FirstOrDefault();
                _backupUsers.Add(_ApproverUser);
                string copyEmails = string.Join(" ;", _backupUsers);



                //var _currManagerPersonal = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.FinanceApprovedByGUID
                //                                                               && x.LanguageID == LAN).FirstOrDefault();
                //f
                //var currAccount = DbAHD.userServiceHistory.Where(x => x.UserGUID == Entitlemnt.FinanceApprovedByGUID).FirstOrDefault();

                //var _currManagerAccount = DbAHD.userServiceHistory.Where(x => x.UserGUID == staffCore.ReportToGUID).ToList();

                var _monthName = _MonthCycleStaff.dataOvertimeMonthCycle.Month + " " + _MonthCycleStaff.dataOvertimeMonthCycle.Year;
                var _staff = DbAHD.v_staffCoreDataOverview.Where(x => x.UserGUID == _MonthCycleStaff.UserGUID).FirstOrDefault();
                var _issuer = DbAHD.v_staffCoreDataOverview.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
                string SubjectMessage = resxEmails.OvertimeSubject.Replace("$StaffName", _staff.FullName).Replace("$month", _monthName);



                //to send mail to staff 
                // var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();
                //URL = AppSettingsKeys.Domain + "/AHD/StaffOvertime/StaffOvertimeConfirmByReviewer/?PK=" + new Portal().GUIDToString(_MonthCycleStaff.OvertimeMonthCycleStaffGUID);
                ////URL = AppSettingsKeys.Domain + "/AHD/Reports/PrintInternationalEntitlementReport/?PK=" + new Portal().GUIDToString(_MonthCycleStaff.OvertimeMonthCycleStaffGUID);
                //Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmDangerPayReceiving + "</a>";
                //Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                //string _financeFirstName = _currManagerPersonal.FirstName;
                //string _financeSurName = _currManagerPersonal.Surname;


                string _message = resxEmails.OvertimeApprovalConfirmation
                                  .Replace("$StaffName", _staff.FullName.ToString())
                                  .Replace("$month", _monthName)

                                  //.Replace("$VerifyLink", Anchor)

                                  ;

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
                //string copy_recipients =_staff.EmailAddress;
                Send(_staff.EmailAddress, SubjectMessage, _message, isRec, copyEmails);
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);

            }

            //}
            return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Report
        public ActionResult ExportOverTimeForMonth(Guid id)
        {
            var result = DbAHD.dataStaffOvertime.Where(x => x.OvertimeMonthCycleStaffGUID == id).ToList();
            var _MyCycle = result.FirstOrDefault().dataOvertimeMonthCycleStaff;
            var _cycle = result.FirstOrDefault().dataOvertimeMonthCycleStaff.dataOvertimeMonthCycle.Month + " " + _MyCycle.dataOvertimeMonthCycle.Year;
            var _approve = result.FirstOrDefault().dataOvertimeMonthCycleStaff.ApprovedByGUID;
            var _superviosr = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _approve).FirstOrDefault();

            var _staff = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _MyCycle.UserGUID).FirstOrDefault();

            if (result.Count > 0)
            {
                string sourceFile = Server.MapPath("~/Areas/AHD/Templates/Overtime/OvertimeRequestForm.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/AHD/temp/OvertimeRequestForm" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();
                    dt.Columns.Add("DATE", typeof(string));
                    dt.Columns.Add("REASON FOR OVERTIME", typeof(string));
                    dt.Columns.Add("FROM", typeof(string));
                    dt.Columns.Add("TO", typeof(string));
                    dt.Columns.Add("SUPERVISOR", typeof(string));
                    dt.Columns.Add("HOU/SUPERVISOR", typeof(string));
                    dt.Columns.Add("FROM ", typeof(string));
                    dt.Columns.Add("TO ", typeof(string));
                    dt.Columns.Add("HOURS", typeof(string));
                    dt.Columns.Add("INITIAL", typeof(string));


                    foreach (var item in result)
                    {

                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.ActionDate.ToLongDateString();
                        dr[1] = item.OvertimeReason;

                        dr[2] = item.TimeIn.ToString("H:mm");
                        dr[3] = item.TimeOut.ToString("H:mm");
                        dr[4] = _superviosr != null ? _superviosr.FullName : "";
                        dr[5] = "";
                        dr[6] = "";
                        dr[7] = "";

                        dr[8] = "";
                        dr[9] = "";



                        dt.Rows.Add(dr);
                    }

                    workSheet.Cells["A10"].LoadFromDataTable(dt, true);
                    var _step = result.FirstOrDefault().StepGUID;
                    var _stepName = DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN && x.ValueGUID == _step).FirstOrDefault();
                    workSheet.Cells["F4"].Value = _cycle;
                    workSheet.Cells["B7"].Value = _staff.FullName;
                    workSheet.Cells["B8"].Value = _staff.Grade + "-" + _stepName.ValueDescription;

                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = "Overtime" + _staff.FullName + DateTime.Now + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }

            string success = "No Data Available for this period";
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);

        }
        #endregion


    }
}