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
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using FineUploader;
using AppsPortal.Library.MimeDetective;

namespace AppsPortal.Areas.AHD.Controllers
{
    public class InternationalStaffEntitlementController : AHDBaseController
    {
        // GET: AHD/InternationalStaffEntitlement
        public ActionResult Index()
        {
            return View();
        }
        #region Staff Historical Data
        [Route("AHD/InternationalStaffEntitlementHistoricalIndex/")]
        public ActionResult InternationalStaffEntitlementHistoricalIndex()
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Access, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }

            return View("~/Areas/AHD/Views/InternationalStaffEntitlement/StaffHistorical/Index.cshtml");
        }
        [Route("AHD/InternationalStaffHistoricalEntitlementDataTable/")]
        public JsonResult InternationalStaffHistoricalEntitlementDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<InternationalStaffEntitlementDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<InternationalStaffEntitlementDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.InternationalStaffEntitlements.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix



            var All = (
                from a in DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.Active == true).AsExpandable()
                join b in DbAHD.v_StaffProfileInformation on a.StaffGUID equals b.UserGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbAHD.dataAHDInternationalStaffEntitlementFlow.Where(x => x.IsLastAction == true && x.Active) on a.InternationalStaffEntitlementGUID equals c.InternationalStaffEntitlementGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()
                join d in DbAHD.codeTablesValuesLanguages.Where(x => true && x.Active) on a.PaymentMethodGUID equals d.ValueGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()

                select new InternationalStaffEntitlementDataTableModel
                {
                    InternationalStaffEntitlementGUID = a.InternationalStaffEntitlementGUID.ToString(),
                    StaffGUID = a.StaffGUID.ToString(),
                    StaffName = a.StaffName,
                    Month = a.dataAHDPeriodEntitlement.MonthName,
                    OrderId = a.dataAHDPeriodEntitlement.OrderId,


                    PeriodEntitlementGUID = a.PeriodEntitlementGUID.ToString(),
                    FlowStatusGUID = a.FlowStatusGUID.ToString(),
                    LastFlowStatusName = a.LastFlowStatusName,

                    DutyStation = R1.DutyStation,
                    DutyStationGUID = R1.DutyStationGUID.ToString(),
                    Department = R1.DepartmentDescription,
                    DepartmentGUID = R1.DepartmentGUID.ToString(),
                    FinanceComment=a.FinanceComment,
                    TotalEntitlements = a.dataAHDInternationalStaffEntitlementDetail.Where(f => f.IsToAdd == true).Select(f => f.TotalAmount).Sum() != null ? a.dataAHDInternationalStaffEntitlementDetail.Where(f => f.IsToAdd == true).Select(f => f.TotalAmount).Sum() - (a.dataAHDInternationalStaffEntitlementDetail.Where(f => f.IsToAdd == false).Select(f => f.TotalAmount).Sum() ?? 0) : 0 - a.dataAHDInternationalStaffEntitlementDetail.Where(f => f.IsToAdd == false).Select(f => f.TotalAmount).Sum() != null ? a.dataAHDInternationalStaffEntitlementDetail.Where(f => f.IsToAdd == false).Select(f => f.TotalAmount).Sum() : 0,
                    Active = a.Active,
                    ReferenceNumber = a.ReferenceNumber,
                    IndexNumber = a.IndexNumber,
                    TransferTo = R2.userPersonalDetails != null ? R2.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN).FirstOrDefault().FirstName + " " + R2.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN).FirstOrDefault().Surname : "",
                    TransferDate = R2.ActionDate,
                    PaymentMethod = R3.ValueDescription,

                    PaymentMethodGUID = a.PaymentMethodGUID,

                    dataAHDInternationalStaffEntitlementRowVersion = a.dataAHDInternationalStaffEntitlementRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<InternationalStaffEntitlementDataTableModel> Result = Mapper.Map<List<InternationalStaffEntitlementDataTableModel>>(All.OrderByDescending(x => x.OrderId).ThenBy(x => x.StaffName).Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Period
        public JsonResult InternationalMyStaffEntitlementPriviousDataTable(DataTableRecievedOptions options, Guid PK)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<InternationalStaffEntitlementDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<InternationalStaffEntitlementDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.InternationalStaffEntitlements.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix



            var All = (
                from a in DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.StaffGUID == PK).AsExpandable()
                join b in DbAHD.v_StaffProfileInformation on a.StaffGUID equals b.UserGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbAHD.dataAHDInternationalStaffEntitlementFlow.Where(x => x.IsLastAction == true && x.Active) on a.InternationalStaffEntitlementGUID equals c.InternationalStaffEntitlementGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()
                join d in DbAHD.codeTablesValuesLanguages.Where(x => true && x.Active) on a.PaymentMethodGUID equals d.ValueGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()

                select new InternationalStaffEntitlementDataTableModel
                {
                    InternationalStaffEntitlementGUID = a.InternationalStaffEntitlementGUID.ToString(),
                    StaffGUID = a.StaffGUID.ToString(),
                    StaffName = a.StaffName,
                    PeriodEntitlementGUID = a.PeriodEntitlementGUID.ToString(),
                    FlowStatusGUID = a.FlowStatusGUID.ToString(),
                    LastFlowStatusName = a.LastFlowStatusName,
                    Month = a.dataAHDPeriodEntitlement.MonthName,

                    TotalEntitlements = a.dataAHDInternationalStaffEntitlementDetail.Where(f => f.IsToAdd == true).Select(f => f.TotalAmount).Sum() != null ? a.dataAHDInternationalStaffEntitlementDetail.Where(f => f.IsToAdd == true).Select(f => f.TotalAmount).Sum() - (a.dataAHDInternationalStaffEntitlementDetail.Where(f => f.IsToAdd == false).Select(f => f.TotalAmount).Sum() ?? 0) : 0 - a.dataAHDInternationalStaffEntitlementDetail.Where(f => f.IsToAdd == false).Select(f => f.TotalAmount).Sum() != null ? a.dataAHDInternationalStaffEntitlementDetail.Where(f => f.IsToAdd == false).Select(f => f.TotalAmount).Sum() : 0,
                    Active = a.Active,
                    OrderId = a.dataAHDPeriodEntitlement.OrderId,


                    dataAHDInternationalStaffEntitlementRowVersion = a.dataAHDInternationalStaffEntitlementRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All.OrderByDescending(x => x.OrderId), DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<InternationalStaffEntitlementDataTableModel> Result = Mapper.Map<List<InternationalStaffEntitlementDataTableModel>>(All.OrderBy(x => x.StaffName).Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result.OrderByDescending(x => x.OrderId).Take(6)), JsonRequestBehavior.AllowGet);
        }
        [Route("AHD/InternationalStaffEntitlementPeriodIndex/")]
        public ActionResult InternationalStaffEntitlementPeriodIndex()
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Access, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }

            return View("~/Areas/AHD/Views/InternationalStaffEntitlementPeriod/Index.cshtml");
        }

        [Route("AHD/InternationalStaffEntitlement/InternationalStaffEntitlementFinanceIndex/")]
        public ActionResult InternationalStaffEntitlementFinanceIndex()
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlementsCertifying.Access, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }

            return View("~/Areas/AHD/Views/InternationalStaffEntitlementPeriod/Index.cshtml");
        }


        [Route("AHD/InternationalStaffEntitlementPeriodDataTable/")]
        public JsonResult InternationalStaffEntitlementPeriodDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<InternationalStaffEntitlementPeriodDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<InternationalStaffEntitlementPeriodDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.InternationalStaffEntitlements.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix



            var All = (
                from a in DbAHD.dataAHDPeriodEntitlement.AsExpandable()
                    //join b in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.FlowStatusGUID equals b.ValueGUID into LJ1
                    //from R1 in LJ1.DefaultIfEmpty()

                select new InternationalStaffEntitlementPeriodDataTableModel
                {
                    PeriodEntitlementGUID = a.PeriodEntitlementGUID.ToString(),
                    PeriodName = a.MonthName,
                    YearNumber = a.YearNumber,
                    MonthName = a.MonthName,
                    StartMonth = a.StartMonth,
                    EndMonth = a.StartMonth,
                    TotalStaff = a.dataAHDInternationalStaffEntitlement.Count(),
                    Active = a.Active,
                    TotalStaffConfimed = a.dataAHDInternationalStaffEntitlement.Where(x => x.FlowStatusGUID != InternationalStaffEntitlmentFlowStatus.Submitted).Count(),
                    TotalStaffPending = a.dataAHDInternationalStaffEntitlement.Where(x => x.FlowStatusGUID == InternationalStaffEntitlmentFlowStatus.Submitted).Count(),
                    FlowStatus = a.FlowStatus,
                    FlowStatusGUID = a.FlowStatusGUID,

                    dataAHDPeriodEntitlementRowVersion = a.dataAHDPeriodEntitlementRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<InternationalStaffEntitlementPeriodDataTableModel> Result = Mapper.Map<List<InternationalStaffEntitlementPeriodDataTableModel>>(All.OrderByDescending(x => x.StartMonth).Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }


        //[Route("AHD/InternationalStaffEntitlementPeriod/Create/")]
        public ActionResult InternationalStaffEntitlementPeriodCreate()
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            return PartialView("~/Areas/AHD/Views/InternationalStaffEntitlementPeriod/_InternationalStaffEntitlementPeriodModal.cshtml", new InternationalStaffEntitlementPeriodUpdateModel { PeriodEntitlementGUID = Guid.Empty });

        }

        //[Route("AHD/InternationalStaffEntitlementPeriod/Update/{PK}")]
        public ActionResult InternationalStaffEntitlementPeriodUpdate(Guid PK)
        {
            var model = (from a in DbAHD.dataAHDPeriodEntitlement.WherePK(PK)
                         select new InternationalStaffEntitlementPeriodUpdateModel
                         {
                             PeriodEntitlementGUID = a.PeriodEntitlementGUID,
                             YearNumber = a.YearNumber,
                             MonthName = a.MonthName,
                             StartMonth = a.StartMonth,
                             EndMonth = a.EndMonth,
                             TotalDaysInMonth = a.TotalDaysInMonth,
                             TotalStaff = a.TotalStaff,
                             TotalStaffConfimed = a.TotalStaffConfimed,

                             TotalStaffPending = a.TotalStaffPending,
                             FlowStatus = a.FlowStatus,
                             FlowStatusGUID = a.FlowStatusGUID,

                             Active = a.Active,
                             OrderID = a.OrderId,
                             //DangerPaymentConfirmationStatus= R1.ValueDescription,

                             dataAHDPeriodEntitlementRowVersion = a.dataAHDPeriodEntitlementRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action(" InternationalStaffEntitlementPeriod", "InternationalStaffEntitlement", new { Area = "AHD" }));
            return View("~/Areas/AHD/Views/InternationalStaffEntitlementPeriod/Index.cshtml", model);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult InternationalStaffEntitlementPeriodCreate(InternationalStaffEntitlementPeriodUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            if (!ModelState.IsValid || ActiveInternationalStaffEntitlementPeriod(model)) return PartialView("~/Areas/AHD/Views/InternationalStaffEntitlementPeriods/_InternationalStaffEntitlementPeriodForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            var periods = DbAHD.dataAHDPeriodEntitlement.Select(x => new { Month = x.StartMonth.Value.Month, Year = x.StartMonth.Value.Year }).ToList();
            foreach (var model2 in periods)
            {
                if ((model.StartMonth.Value.Month == model2.Month) && (model.StartMonth.Value.Year == model2.Year)
                      )
                {
                    ModelState.AddModelError("Error: ", "Peroid Already Exist");
                    return PartialView("~/Areas/AHD/Views/InternationalStaffEntitlementPeriods/_InternationalStaffEntitlementPeriodForm.cshtml", model);
                }


            }
            dataAHDPeriodEntitlement Period = Mapper.Map(model, new dataAHDPeriodEntitlement());
            //Guid _activeStaff = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3611");
            //Guid _activeStaff = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3611");
            Guid _eligibleGUID = Guid.Parse("2dac5d96-e6a3-48c1-b3f5-17bfd9f62881");
            Guid internationaStaffGUID = Guid.Parse("FC4B2E79-2B97-4252-A50B-915B07A12310");
            var order = DbAHD.dataAHDPeriodEntitlement.OrderByDescending(x => x.OrderId).Select(x => x.OrderId).FirstOrDefault();
            Period.OrderId = order != null ? order + 1 : 1;
            //Guid testuser = Guid.Parse("E96E1CD3-4910-4C8C-A9C9-41138D516ACD");


            var _mydaily = DbAHD.dataStaffLeaveAttendanceDaily.ToList();
            DbAHD.dataStaffLeaveAttendanceDaily.RemoveRange(_mydaily);
            DbAHD.SaveChanges();
            var _allattendaces = DbAHD.dataInternationalStaffAttendance.ToList();

            var allstaffs = DbAHD.StaffCoreData.Where(x => x.RecruitmentTypeGUID == internationaStaffGUID
            && x.Active == true
            && x.PaymentEligibilityStatusGUID == _eligibleGUID
                           ).ToList();
            List<dataStaffLeaveAttendanceDaily> alldailytoadd = new List<dataStaffLeaveAttendanceDaily>();

            foreach (var item in allstaffs)
            {
                var temp = _allattendaces.Where(x => x.StaffGUID == item.UserGUID).ToList();

                foreach (var myatt in temp.OrderBy(x => x.FromDate))
                {
                    for (var day = myatt.FromDate; day <= myatt.ToDate; day = day.Value.AddDays(1))
                    {
                        dataStaffLeaveAttendanceDaily toAddaily = new dataStaffLeaveAttendanceDaily
                        {
                            StaffLeaveAttendanceDailyGUID = Guid.NewGuid(),
                            DayDate = day,
                            StaffGUID = item.UserGUID,
                            InternationalStaffAttendanceTypeGUID = myatt.InternationalStaffAttendanceTypeGUID,

                        };
                        alldailytoadd.Add(toAddaily);

                    }
                }

            }
            DbAHD.CreateBulkNoAudit(alldailytoadd);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }

            Guid EntityPK = Guid.NewGuid();



            Period.PeriodEntitlementGUID = EntityPK;
            Period.MonthName = ProcessData.GetMonthName(model.StartMonth.Value.Month) + "/" + model.StartMonth.Value.Year;
            Period.YearNumber = model.StartMonth.Value.Year;
            Period.TotalDaysInMonth = (int)(model.EndMonth - model.StartMonth).Value.TotalDays + 1;
            Period.FlowStatus = "Submitted";
            Period.FlowStatusGUID = InternationalStaffEntitlmentFlowStatus.Submitted;
            Period.TotalStaffPending = allstaffs.Count;
            Period.TotalStaff = allstaffs.Count;
            Period.TotalStaffConfimed = allstaffs.Count;

            //Guid testNicoloas = Guid.Parse("2815DD19-2387-4953-B0ED-95C1184CFB61");
            //Guid amelGUID = Guid.Parse("AEF8CC39-49F4-4C53-9F2F-194DD06918F7");

            var personalDetaillan = DbAHD.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).ToList();
            List<dataAHDInternationalStaffEntitlement> staffEntitlements = new List<dataAHDInternationalStaffEntitlement>();
            List<StaffTeleComutingLeave> allStaffTele = new List<StaffTeleComutingLeave>();
            var countrieNotAllowedGUIDs = DbAHD.codeAHDLocationNoRAndRTicket.Select(x => x.LocationGUID).ToList();

            var allattendacLeaves = DbAHD.dataInternationalStaffAttendance.ToList();


            #region tele process
            var teleuserGuids = allattendacLeaves.Where(x => x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting).Select(x => x.StaffGUID).Distinct();
            var beforeAttendaceLeaves = allattendacLeaves.Where(x => x.FromDate < model.StartMonth && x.ToDate >= model.StartMonth).ToList();
            var afterAttendaceLeaves = allattendacLeaves.Where(x => x.FromDate >= model.StartMonth && x.ToDate > model.EndMonth
                                                      && (x.FromDate.Value.Month == model.StartMonth.Value.Month && x.FromDate.Value.Year == model.StartMonth.Value.Year)).ToList();
            var currentMonthAttendaceLeaves = allattendacLeaves.Where(x => x.FromDate >= model.StartMonth
                                         && x.ToDate <= model.EndMonth).ToList();

            //new tele 
            var allLeavesDaily = DbAHD.dataStaffLeaveAttendanceDaily.ToList();
            int totlaDays = 0;
            //foreach (var item in teleuserGuids.Distinct())
            //{
            //    totlaDays = 0;

            //    var staffLeavethismonth = allLeavesDaily.Where(x => x.StaffGUID == item &&
            //       x.DayDate >= model.StartMonth && x.DayDate <= model.EndMonth).ToList();
            //    var firsttele = staffLeavethismonth.Where(x =>
            //    x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting).OrderBy(x => x.DayDate).FirstOrDefault();
            //    var beforetele = staffLeavethismonth.Where(x => x.DayDate < firsttele.DayDate ).ToList();
            //    var aftertele = staffLeavethismonth.Where(x => x.DayDate > firsttele.DayDate).ToList();
            //    DateTime? currenttelDate = firsttele != null ? firsttele.DayDate.Value.AddDays(-1) : (DateTime?)null;
            //    var checkRRBeforeFor = beforetele.Where(x => x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR).OrderByDescending(x => x.DayDate).ToList();
            //    var checkRRAfterFor = aftertele.Where(x => x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR).OrderBy(x => x.DayDate).ToList();

            //    foreach (var mycurr in beforetele.OrderByDescending(x => x.DayDate))
            //    {
            //        if (currenttelDate == mycurr.DayDate )
            //        {


            //            totlaDays++;
            //            currenttelDate = currenttelDate.Value.AddDays(-1);


            //        }

            //    }
            //    currenttelDate = firsttele != null ? firsttele.DayDate.Value.AddDays(1) : (DateTime?)null;
            //    foreach (var mycurr in aftertele.OrderBy(x => x.DayDate))
            //    {
            //        if (currenttelDate == mycurr.DayDate)
            //        {


            //            totlaDays++;
            //            currenttelDate = currenttelDate.Value.AddDays(1);


            //        }

            //    }
            //    if (totlaDays > 0)
            //    {

            //        StaffTeleComutingLeave myStaffTeleDay = new StaffTeleComutingLeave
            //        {
            //            totalDays = totlaDays + 1,
            //            StaffGUID = (Guid)item
            //        };
            //        if (checkRRBeforeFor.Count > 0)
            //        {
            //            myStaffTeleDay.totalDays = myStaffTeleDay.totalDays - checkRRBeforeFor.Count;


            //        }
            //        if (checkRRAfterFor.Count > 0)
            //        {
            //            myStaffTeleDay.totalDays = myStaffTeleDay.totalDays - checkRRAfterFor.Count;
            //        }
            //        allStaffTele.Add(myStaffTeleDay);
            //    }



            //}






            #endregion


            var entilementtypes = DbAHD.codeAHDEntitlementType.Where(x => x.EntitlementTypeGUID != InternationalStaffEntitlementType.AddedRecorvery
            && x.EntitlementTypeGUID != InternationalStaffEntitlementType.DeductedRecovery
            && x.EntitlementTypeGUID!= InternationalStaffEntitlementType.BreakfastDeduction).ToList();
            var entitlementTypePerDutyStation = DbAHD.codeAHDEntitlementTypePerDutyStation.ToList();
            personalDetaillan = personalDetaillan.Where(x => allstaffs.Select(f => f.UserGUID).Contains(x.UserGUID)).ToList();
            //Guid IsacmyUser = Guid.Parse("F83879A9-2113-4606-8B01-2B28FC82D536");
            Guid AleepocmyUser = Guid.Parse("F1B5F1A4-280F-405D-8305-11C5CBF28179");

            //var s = currentMonthAttendaceLeaves.Where(d => d.StaffGUID == myUser && (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave
            //                                                                     || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave ||
            //                                                                     d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave)).Select(x => x.TotalDays).Sum();


            Guid aleppoStationGUID = Guid.Parse("6D7397D6-3D7F-48FC-BFD2-18E69673AC94");
            Guid tartusStationGUID = Guid.Parse("6CD6D68D-EAC1-440B-904F-7D34B4FD3863");


            staffEntitlements = (from x in allstaffs
                                     //.Where(x=>x.UserGUID== testuser)

                                 let staffName = personalDetaillan.Where(f => f.UserGUID == x.UserGUID)
                                 let myInternationalStaffEntitlementGUID = Guid.NewGuid()

                                 //let myalltelesDaysWithleavs = allStaffTele.Where(f => f.StaffGUID == x.UserGUID).ToList()
                                 let mycurrenmonthtattendacLeaves = currentMonthAttendaceLeaves.Where(f => f.StaffGUID == x.UserGUID)
                                 let mybeforeAttendaceLeaves = beforeAttendaceLeaves.Where(f => f.StaffGUID == x.UserGUID)
                                 let myafterAttendaceLeaves = afterAttendaceLeaves.Where(f => f.StaffGUID == x.UserGUID)
                                 let myallattendacLeaves = allattendacLeaves.Where(f => f.StaffGUID == x.UserGUID)
                                 let teleworking = allattendacLeaves.Where(f => f.StaffGUID == x.UserGUID &&
                                                                    f.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting)
                                 let myAllLeavesCountForLeaveMoreThanMonth = myallattendacLeaves.Where(x => x.FromDate < model.StartMonth && x.ToDate > model.EndMonth

                                                                                              && (
                                                                                               x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave
                                                                                               || x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave
                                                                                               || x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.HomeLeave
                                                                                               || x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.PaternityLeave
                                                                                                      || x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave
                                                                                                      || x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave
                                                                                                      || x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.SLWFP
                                                                                                            || x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR
                                                                                                            //|| x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave
                                                                                                            || x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting
                                                                                                            || x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend
                                                                                                            || x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday
                                                                                              )).Count()
                                 #region Without R_R
                                 let currLeaveWithoutR_R = mycurrenmonthtattendacLeaves.Where(d => d.StaffGUID == x.UserGUID && (
                                 d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave
                                 || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave
                                 || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.HomeLeave
                                 || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.PaternityLeave
                                                                                 || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave
                                                                                 || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave
                                                                                 || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.SLWFP
                                                                                 || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting
                                                                                 || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend
                                                                                 || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday

                                                                                 )).Select(x => x.TotalDays).Sum()


                                 let beforeLeaveWithoutR_R = mybeforeAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                                            && (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave
                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave
                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.HomeLeave
                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.PaternityLeave

                                                                               || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave
                                                                               || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave
                                                                               || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.SLWFP
                                                                               || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting
                                                                               || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend
                                                                               || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday
                                                                               )).
                                                                               Select(x => (x.ToDate - model.StartMonth).Value.TotalDays - 1).FirstOrDefault()


                                 #endregion

                                 let currLeavesSumDays = mycurrenmonthtattendacLeaves.Where(d => d.StaffGUID == x.UserGUID &&
                                                                                            (
                                                                                            d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave
                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave
                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.HomeLeave
                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.PaternityLeave
                                                                                                 || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR
                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave
                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave
                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.SLWFP
                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting
                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend
                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday
                                                                                            )).Select(x => x.TotalDays).Sum()
                                 #region Days for rental between month and date

                                 let totalDaysLeaveBeforeTillEndMonth = mybeforeAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                                                                      && (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave
                                                                                                      || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave
                                                                                                      || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.HomeLeave
                                                                                                      || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.PaternityLeave
                                                                                                      || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR
                                                                                                         || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave
                                                                                                         || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave
                                                                                                         || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.SLWFP
                                                                                                         || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting
                                                                                                         || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend
                                                                                                         || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday
                                                                                                         )).
                                                                                                         Select(x => (x.ToDate - model.StartMonth).Value.TotalDays - 1).FirstOrDefault()
                                 let totalDaysLeaveAfterTillEndMonth = myafterAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                                                                     && (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave
                                                                                                     || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave
                                                                                                     || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.HomeLeave
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.PaternityLeave
                                                                                                      || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.SLWFP
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday
                                                                                                        )).
                                                                                                        Select(x => (model.EndMonth - x.FromDate).Value.TotalDays - 1).FirstOrDefault()



                                 let totalDaysLeaveBeforeWithoutR_R_TillEndMonth = mybeforeAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                                                                      && (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave
                                                                                                      || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave
                                                                                                                 || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.HomeLeave
                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.PaternityLeave

                                                                                                         || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave
                                                                                                         || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave
                                                                                                         || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.SLWFP
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday
                                                                                                         )).
                                                                                                         Select(x => (x.ToDate - model.StartMonth).Value.TotalDays - 1).FirstOrDefault()
                                 let totalDaysLeaveAfterWithoutR_R_TillEndMonth = myafterAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                                                                     && (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave
                                                                                                     || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave
                                                                                                     || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.HomeLeave
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.PaternityLeave

                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.SLWFP
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday
                                                                                                        )).
                                                                                                        Select(x => (model.EndMonth - x.FromDate).Value.TotalDays).FirstOrDefault()
                                 let totalDaysLeaveBeforeR_R_TillEndMonth = mybeforeAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                                                                     && (
                                                                                                     d.InternationalStaffAttendanceTypeGUID ==
                                                                                                     coddeInternationalStaffAttendanceTypeAttendanceTable.RR

                                                                                                        )).
                                                                                                        Select(x => (x.ToDate - model.StartMonth).Value.TotalDays).FirstOrDefault()
                                 let totalDaysLeaveAfterR_R_TillEndMonth = myafterAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                                                                     && (d.InternationalStaffAttendanceTypeGUID ==
                                                                                                     coddeInternationalStaffAttendanceTypeAttendanceTable.RR

                                                                                                        )).
                                                                                                        Select(x => (model.EndMonth - x.FromDate).Value.TotalDays).FirstOrDefault()


                                 #endregion

                                 #region Leaves to count except r_r
                                 let totaldaysLeavsAfterWithout_R_RMonth = myafterAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                                                                    && (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave
                                                                                                    || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave
                                                                                                    || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.HomeLeave

                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.PaternityLeave

                                                                                                       || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave
                                                                                                       || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave
                                                                                                       || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.SLWFP
                                                                                                       || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting
                                                                                                       || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend
                                                                                                       || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday
                                                                                                       )).
                                                                                                       Select(x => x.TotalDays).FirstOrDefault()

                                 #endregion
                                 let totalCurrR_RLeave = (int?)mycurrenmonthtattendacLeaves.Where(d => d.StaffGUID == x.UserGUID && d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR
                                                                                                            ).Select(d => d.TotalDays).Sum()

                                 let totalCurTravel_TimeTTLeave = (int?)mycurrenmonthtattendacLeaves.Where(d => d.StaffGUID == x.UserGUID && d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TravelTime
                               ).Select(d => d.TotalDays).Sum()


                                 let totalBeforeR_RLeave = mybeforeAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                         && (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR)

                                       ).Select(x => (x.ToDate - model.StartMonth).Value.TotalDays).Sum()

                                 let totalAfterR_RLeave = myafterAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                                 && (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR)

                                                                    ).Select(x => (model.EndMonth - x.FromDate).Value.TotalDays).FirstOrDefault()
                                 let totalBeforeR_RLeaveAllPeriod = mybeforeAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                                 && (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR)

                                                               ).Select(x => x.TotalDays).Sum()

                                 let totalAfterR_RLeaveAllPeriod = myafterAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                          && (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR)

                                                     ).Select(x => x.TotalDays).Sum()


                                 let totalBeforeAllLeavesAllPeriod = mybeforeAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                                                                         && (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave
                                                                                                         || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave
                                                                                                         || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.HomeLeave
                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.PaternityLeave
                                                                                                          || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR
                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave ||
                                                                                                            d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave
                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.SLWFP
                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting
                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend
                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday
                                                                                                            )).Select(x => x.TotalDays).Sum()

                                 let totalAfterAllLeavesAllPeriod = myafterAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                                                                            && (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave
                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave
                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.HomeLeave
                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.PaternityLeave
                                                                                                             || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR
                                                                                                               || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave ||
                                                                                                               d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave
                                                                                                               || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.SLWFP
                                                                                                               || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting
                                                                                                               || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend
                                                                                                               || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday
                                                                                                               )).Select(x => x.TotalDays).Sum()

                                 let totalAfterR_RTicker = myafterAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                                                                               && (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR)
                                                                                                                  && !countrieNotAllowedGUIDs.Contains(d.CountryGUID)
                                                                                             ).Select(x => x.TotalDays).FirstOrDefault()

                                 let myentitlementTypePerDutyStation = entitlementTypePerDutyStation.Where(f => f.DutyStationGUID == x.DutyStationGUID)
                                 select new dataAHDInternationalStaffEntitlement
                                 {
                                     InternationalStaffEntitlementGUID = myInternationalStaffEntitlementGUID,
                                     StaffGUID = x.UserGUID,
                                     PeriodEntitlementGUID = Period.PeriodEntitlementGUID,
                                     FlowStatusGUID = InternationalStaffEntitlmentFlowStatus.Submitted,
                                     LastFlowStatusName = "Submitted",
                                     Active = true,
                                     StaffName = staffName.Select(f => f.FirstName).FirstOrDefault() + " " + staffName.Select(f => f.Surname).FirstOrDefault(),

                                     dataAHDInternationalStaffEntitlementDetail = entilementtypes.Select(f =>
                                                                                          new dataAHDInternationalStaffEntitlementDetail
                                                                                          {
                                                                                              InternationalStaffEntitlementDetailGUID = Guid.NewGuid(),
                                                                                              InternationalStaffEntitlementGUID = myInternationalStaffEntitlementGUID,
                                                                                              EntitlementTypeGUID = f.EntitlementTypeGUID,
                                                                                              TotalDays = myAllLeavesCountForLeaveMoreThanMonth > 0 ?
                                                                                                          myAllLeavesCountForLeaveMoreThanMonth
                                                                                                          :
                                                                                                         (
                                                                                                          //total days
                                                                                                          myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).FirstOrDefault().EntitlementTypeGUID == InternationalStaffEntitlementType.R_RTicket ?
                                                                                                           //(((int?)totalCurrR_RLeave>7? (int?)totalCurrR_RLeave : totalCurrR_RLeave)  +
                                                                                                           //((int?)totalBeforeR_RLeaveAllPeriod > 7? (int?)totalDaysLeaveBeforeR_R_TillEndMonth > 0 ? (int?)totalDaysLeaveBeforeR_R_TillEndMonth : totalBeforeR_RLeaveAllPeriod : 0)
                                                                                                           //)
                                                                                                           (
                                                                                                          mycurrenmonthtattendacLeaves
                                                                                                                      .Where(d => d.StaffGUID == x.UserGUID
                                                                                                          && d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR
                                                                                                          && (d.CountryGUID == null || !countrieNotAllowedGUIDs.Contains(d.CountryGUID))
                                                                                                            ).Select(d => d.TotalDays).Sum() ?? 0 +

                                                                                                             totalAfterR_RTicker ?? 0
                                                                                                             )

                                                                                                          ///start 
                                                                                                          : myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).FirstOrDefault().EntitlementTypeGUID == InternationalStaffEntitlementType.RentalDeduction ?
                                                                                                                    (
                                                                                                                    (x.DutyStationGUID == tartusStationGUID)
                                                                                                                    ?
                                                                                                                    0


                                                                                                                    :
                                                                                                                    (1 == 2)
                                                                                                                    //(myalltelesDaysWithleavs != null && myalltelesDaysWithleavs.Select(x=>x.totalDays).Sum() > 0)
                                                                                                                    ?
                                                                                                                    0
                                                                                                                    //Period.TotalDaysInMonth-myalltelesDaysWithleavs.Select(x => x.totalDays).Sum()
                                                                                                                    //myalltelesDaysWithleavs.FirstOrDefault().totalDays
                                                                                                                    :
                                                                                                                   x.DutyStationGUID == aleppoStationGUID
                                                                                                                   ?
                                                                                                                    ((currLeavesSumDays > 0 ? currLeavesSumDays : 0) + (totalDaysLeaveBeforeTillEndMonth > 0 ? totalDaysLeaveBeforeTillEndMonth : 0) +
                                                                                                                    ((totalCurTravel_TimeTTLeave > 0 ? totalCurTravel_TimeTTLeave : 0)) > 15

                                                                                                                         ?
                                                                                                                         ((Period.TotalDaysInMonth - (currLeavesSumDays ?? 0 + totalDaysLeaveBeforeTillEndMonth + totalCurTravel_TimeTTLeave)) > Period.TotalDaysInMonth ?
                                                                                                           0 :
                                                                                                           (int?)((Period.TotalDaysInMonth - (currLeavesSumDays ?? 0 + totalDaysLeaveBeforeTillEndMonth + totalCurTravel_TimeTTLeave))
                                                                                                           + ((currLeavesSumDays ?? 0 + totalDaysLeaveBeforeTillEndMonth + totalCurTravel_TimeTTLeave) - 15)
                                                                                                           )
                                                                                                           )
                                                                                                                          //(int?)(Period.TotalDaysInMonth - (currLeavesSumDays ?? 0 + totalDaysLeaveBeforeTillEndMonth + totalCurTravel_TimeTTLeave))

                                                                                                                          :
                                                                                                                              (Period.TotalDaysInMonth - (currLeavesSumDays > 0 ? currLeavesSumDays : 0 +
                                                                                                                                 (int?)totalDaysLeaveBeforeTillEndMonth > 0 ? (int?)totalDaysLeaveBeforeTillEndMonth : 0 + totalCurTravel_TimeTTLeave))
                                                                                                                                 )

                                                                                                                   :
                                                                                                                   Period.TotalDaysInMonth
                                                                                                                    //end rental 



                                                                                                                    )
                                                                                                                    //other r leaves
                                                                                                                    :

                                                                                                                  (myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).FirstOrDefault().EntitlementTypeGUID == InternationalStaffEntitlementType.DangerPayPerDay ?
                                                                                                                                                     (Period.TotalDaysInMonth - (int?)
                                                                                                                                                                       (
                                                                                                                                                                                       ((int?)totalCurrR_RLeave > 7 ? totalCurrR_RLeave - 7 : 0) +
                                                                                                                                                                                       ((int?)totalBeforeR_RLeaveAllPeriod > 7 ? totalDaysLeaveBeforeR_R_TillEndMonth > 0 ? totalDaysLeaveBeforeR_R_TillEndMonth : 0 : 0) +
                                                                                                                                                                                       /////no need for after
                                                                                                                                                                                       ((int?)totalAfterR_RLeaveAllPeriod > 7 ? totalDaysLeaveAfterR_R_TillEndMonth > 0 ? totalDaysLeaveAfterR_R_TillEndMonth : 0 : 0) +
                                                                                                                                                                                       ///////
                                                                                                                                                                                       ((int?)currLeaveWithoutR_R > 0 ? currLeaveWithoutR_R : 0) +
                                                                                                                                                                                       ((int?)totalDaysLeaveBeforeWithoutR_R_TillEndMonth > 0 ? totalDaysLeaveBeforeWithoutR_R_TillEndMonth : 0) +
                                                                                                                                                                                       ((int?)totalDaysLeaveAfterWithoutR_R_TillEndMonth > 0 ? totalDaysLeaveAfterWithoutR_R_TillEndMonth : 0)

                                                                                                                                                                                       )) :

                                                                                                                       0)


                                                                                                         ),
                                                                                              BasePeriodAmount = myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).FirstOrDefault() != null ? myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).FirstOrDefault().EntitlementValue : 0,
                                                                                              TotalAmount = myAllLeavesCountForLeaveMoreThanMonth > 0 ?
                                                                                              0
                                                                                              :
                                                                                                          //##############################################################################################################################################################################################################################################
                                                                                                          //--A    //R_R Ticket
                                                                                                          myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).FirstOrDefault().EntitlementTypeGUID == InternationalStaffEntitlementType.R_RTicket
                                                                                                          ?
                                                                                                                    //checkxxrr
                                                                                                                    //((
                                                                                                                    //mycurrenmonthtattendacLeaves
                                                                                                                    //            .Where(d => d.StaffGUID == x.UserGUID
                                                                                                                    //&& d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR
                                                                                                                    //&& (d.CountryGUID == null || !countrieNotAllowedGUIDs.Contains(d.CountryGUID))
                                                                                                                    //  ).Select(d => d.TotalDays).Sum() > 0
                                                                                                                    //   ||
                                                                                                                    //   totalAfterR_RTicker > 0
                                                                                                                    //   ) ?
                                                                                                                    //    myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).FirstOrDefault().
                                                                                                                    //    EntitlementValue : 0)

                                                                                                                    ((

                                                                                                          mycurrenmonthtattendacLeaves
                                                                                                                      .Where(d => d.StaffGUID == x.UserGUID
                                                                                                          && d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR
                                                                                                          && (d.CountryGUID == null || !countrieNotAllowedGUIDs.Contains(d.CountryGUID))
                                                                                                            ).Select(d => d.TotalDays).Sum() > 0
                                                                                                             ||
                                                                                                             totalAfterR_RTicker > 0
                                                                                                             ) ?
                                                                                                              myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).
                                                                                                              FirstOrDefault().EntitlementValue : 0

                                                                                                              )
                                                                                                        //##############################################################################################################################################################################################################################################       

                                                                                                        //--B         // Rental Deducation

                                                                                                        :

                                                                                                        myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).FirstOrDefault().
                                                                                                                   EntitlementTypeGUID == InternationalStaffEntitlementType.RentalDeduction
                                                                                                  ? (x.DutyStationGUID == tartusStationGUID)
                                                                                                                   ?
                                                                                                                   0


                                                                                                                   :


                                                                                                       ///////// //tele working

                                                                                                       (
                                                                                                       1 == 2
                                                                                                                   //myalltelesDaysWithleavs != null && myalltelesDaysWithleavs.Select(x => x.totalDays).Sum() > 0

                                                                                                                   )

                                                                                                                   ?
                                                                                                                   0
                                                                                                       //(myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).
                                                                                                       // FirstOrDefault().EntitlementValue / Period.TotalDaysInMonth)

                                                                                                       // * ((myalltelesDaysWithleavs != null && myalltelesDaysWithleavs.Select(x => x.totalDays).Sum() > Period.TotalDaysInMonth)
                                                                                                       //              ?
                                                                                                       //             0
                                                                                                       //                :
                                                                                                       //          (Period.TotalDaysInMonth - (myalltelesDaysWithleavs != null ? myalltelesDaysWithleavs.Select(x => x.totalDays).Sum() : 0)))

                                                                                                       //end tele wroking
                                                                                                       :


                                                                                                      //Aleppo

                                                                                                      x.DutyStationGUID == aleppoStationGUID
                                                                                                      ?
                                                                                                        (

                                                                                                       ((currLeavesSumDays > 0 ? currLeavesSumDays : 0) + (totalDaysLeaveBeforeTillEndMonth > 0 ? totalDaysLeaveBeforeTillEndMonth : 0) + ((totalCurTravel_TimeTTLeave > 0 ? totalCurTravel_TimeTTLeave : 0)) > 15
                                                                                                       //totalBeforeAllLeavesAllPeriod > 15
                                                                                                       )
                                                                                                          ///above 15 days
                                                                                                          ?
                                                                                                          (myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).FirstOrDefault().EntitlementValue
                                                                                                             / Period.TotalDaysInMonth) *
                                                                                                           ((Period.TotalDaysInMonth - (currLeavesSumDays ?? 0 + totalDaysLeaveBeforeTillEndMonth + totalCurTravel_TimeTTLeave)) > Period.TotalDaysInMonth ?
                                                                                                           0 :
                                                                                                           (int?)((Period.TotalDaysInMonth - (currLeavesSumDays ?? 0 + totalDaysLeaveBeforeTillEndMonth + totalCurTravel_TimeTTLeave))
                                                                                                           + ((currLeavesSumDays ?? 0 + totalDaysLeaveBeforeTillEndMonth + totalCurTravel_TimeTTLeave) - 15)
                                                                                                           )
                                                                                                           )
                                                                                                      //((currLeavesSumDays > 15?((Period.TotalDaysInMonth - currLeavesSumDays) + (currLeavesSumDays - 15))
                                                                                                      //                                                   : Period.TotalDaysInMonth - currLeavesSumDays)+(totalDaysLeaveBeforeTillEndMonth > 15 ?
                                                                                                      //                                                   ((int)totalDaysLeaveBeforeTillEndMonth - 15) : 0 ))


                                                                                                      :
                                                                                                        //less 15

                                                                                                        (((currLeavesSumDays > 0 ? currLeavesSumDays : 0) + (totalDaysLeaveBeforeTillEndMonth > 0 ?
                                                                                                        totalDaysLeaveBeforeTillEndMonth : 0) + (totalCurTravel_TimeTTLeave > 0 ?
                                                                                                        totalCurTravel_TimeTTLeave : 0) <= 15)
                                                                                                        &&
                                                                                                        (
                                                                                                        (currLeavesSumDays > 0 ? currLeavesSumDays : 0) + (totalDaysLeaveBeforeTillEndMonth > 0 ?
                                                                                                        totalDaysLeaveBeforeTillEndMonth : 0) + (totalCurTravel_TimeTTLeave > 0 ?
                                                                                                        totalCurTravel_TimeTTLeave : 0) > 0
                                                                                                        )
                                                                                                       //totalBeforeAllLeavesAllPeriod > 15
                                                                                                       )
                                                                                                      ?
                                                                                                         (myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).FirstOrDefault().EntitlementValue / Period.TotalDaysInMonth)
                                                                                                         * (Period.TotalDaysInMonth - (currLeavesSumDays + (int?)totalDaysLeaveBeforeTillEndMonth + totalCurTravel_TimeTTLeave))






                                                                                                        //////////////////////////////////////////////

                                                                                                        //default option

                                                                                                        :
                                                                                                        myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).FirstOrDefault().EntitlementValue

                                                                                                        /// Period.TotalDaysInMonth
                                                                                                        //* (Period.TotalDaysInMonth - (currLeavesSumDays > 0 ? currLeavesSumDays : 0 +
                                                                                                        //(int?)totalDaysLeaveBeforeTillEndMonth > 0 ? (int?)totalDaysLeaveBeforeTillEndMonth : 0

                                                                                                        //+ (int?)totalCurTravel_TimeTTLeave > 0 ? (int?)totalCurTravel_TimeTTLeave : 0
                                                                                                        //))



                                                                                                        )

                                                                                                        //end Aleepo
                                                                                                        :




                                                                                                        myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).FirstOrDefault().EntitlementValue

                                                                                                        //##############################################################################################################################################################################################################################################     
                                                                                                        // Danger Pay 
                                                                                                        :
                                                                                                   myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).FirstOrDefault().EntitlementTypeGUID == InternationalStaffEntitlementType.DangerPayPerDay

                                                                                                   ?

                                                                                                   (
                                                                                                   ((totalBeforeR_RLeave == 0 || totalBeforeR_RLeave <= 7 || totalBeforeR_RLeave == null)
                                                                                                   && (totalCurrR_RLeave == 0 || totalCurrR_RLeave <= 7 || totalCurrR_RLeave == null)
                                                                                                   && (totalAfterR_RLeave == 0 || totalAfterR_RLeave <= 7 || totalAfterR_RLeave == null)
                                                                                                   &&
                                                                                                   (currLeaveWithoutR_R == 0 || currLeaveWithoutR_R == null) && (totalDaysLeaveBeforeWithoutR_R_TillEndMonth == 0
                                                                                                          || totalDaysLeaveBeforeWithoutR_R_TillEndMonth == null)
                                                                                                   && (totalDaysLeaveAfterWithoutR_R_TillEndMonth == 0 || totalDaysLeaveAfterWithoutR_R_TillEndMonth == null)
                                                                                                   )
                                                                                                        ?
                                                                                                         1645
                                                                                                        :


                                                                                                       (
                                                                                                      (Period.TotalDaysInMonth -
                                                                                                       (int?)
                                                                                                       (
                                                                                                                       ((int?)totalCurrR_RLeave > 7 ? totalCurrR_RLeave - 7 : 0) +
                                                                                                                       ((int?)totalBeforeR_RLeaveAllPeriod > 7 ? totalDaysLeaveBeforeR_R_TillEndMonth > 0 ? totalDaysLeaveBeforeR_R_TillEndMonth : 0 : 0) +
                                                                                                                       /////no need for after
                                                                                                                       ((int?)totalAfterR_RLeaveAllPeriod > 7 ? totalDaysLeaveAfterR_R_TillEndMonth > 0 ? totalDaysLeaveAfterR_R_TillEndMonth : 0 : 0) +
                                                                                                                       ///////
                                                                                                                       ((int?)currLeaveWithoutR_R > 0 ? currLeaveWithoutR_R : 0) +
                                                                                                                       ((int?)totalDaysLeaveBeforeWithoutR_R_TillEndMonth > 0 ? totalDaysLeaveBeforeWithoutR_R_TillEndMonth : 0) +
                                                                                                                       ((int?)totalDaysLeaveAfterWithoutR_R_TillEndMonth > 0 ? totalDaysLeaveAfterWithoutR_R_TillEndMonth : 0)

                                                                                                                       ))

                                                                                                                        * myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).FirstOrDefault().
                                                                                                                        EntitlementValue
                                                                                                                        )


                                                                                                       )


                                                                                                       : 0,
                                                                                              //###########################
                                                                                              IsToAdd = (bool)myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).Select(x => x.codeAHDEntitlementType.IsToAdd).FirstOrDefault(),
                                                                                              Active = true,

                                                                                          }).ToList(),
                                     dataAHDInternationalStaffEntitlementFlow = new List<dataAHDInternationalStaffEntitlementFlow>
                                     {
                                         new dataAHDInternationalStaffEntitlementFlow
                                         {
                                         InternationalStaffEntitlementFlowGUID = Guid.NewGuid(),
                                         InternationalStaffEntitlementGUID= myInternationalStaffEntitlementGUID,
                                         CreatedByGUID=UserGUID,

                                         FlowStatusGUID=InternationalStaffEntitlmentFlowStatus.Submitted,
                                         ActionDate=ExecutionTime,
                                         IsLastAction=true,
                                         OrderId=1,
                                         Active=true,

                                         }
}


                                 }).ToList();



            DbAHD.Create(Period, Permissions.InternationalStaffEntitlements.CreateGuid, ExecutionTime, DbCMS);
            DbAHD.CreateBulk(staffEntitlements, Permissions.InternationalStaffEntitlements.CreateGuid, ExecutionTime, DbCMS);


            // Guid maksoud = Guid.Parse("8F7EF83F-FD3E-4F8C-8735-8A22D3D61B75");
            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.InternationalStaffEntitlementPeriodDataTable, ControllerContext, "InternationalStaffEntitlementPeriodLanguagesFormControls"));
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.InternationalStaffEntitlements.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("Create", "InternationalStaffEntitlementPeriods", new { Area = "AHD" })), Container = "InternationalStaffEntitlementPeriodDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.InternationalStaffEntitlements.Update, Apps.AHD), Container = "InternationalStaffEntitlementPeriodDetailFormControls" });
            //var staffs = DbAHD.dataStaffEligibleForDangerPay.ToList();

            #region Export
            string sourceFile = Server.MapPath("~/Areas/AHD/Templates/StaffEntitlements/StaffEntitlementMonth.xlsx");
            string DisFolder =
                Server.MapPath("~/Areas/AHD/Templates/StaffEntitlements/MonthlyEntitlements/" + EntityPK + ".xlsx");
            System.IO.File.Copy(sourceFile, DisFolder);
            using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
            {
                var cx = package.Workbook.Worksheets.ToList();
                ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                DataTable dt = new DataTable();
                dt.Columns.Add("Staff Name", typeof(string));
                dt.Columns.Add("Contract type", typeof(string));
                dt.Columns.Add("Duty station", typeof(string));
                dt.Columns.Add("Number of days DP is not payable", typeof(int));
                dt.Columns.Add("Number of days DP is  payable", typeof(int));
                dt.Columns.Add("Current danger pay amount ", typeof(int));
                dt.Columns.Add("Previous month(s) danger pay amount", typeof(int));
                dt.Columns.Add("Covered period", typeof(string));
                dt.Columns.Add("Travel dates confirmation)", typeof(string));
                dt.Columns.Add("Total)", typeof(string));
                int rowIndex = 16;
                int rowIndexNumber = 17;
                int col = 2;
                int colNumber = 2;
                workSheet.Cells["AJ12"].Value = Period.TotalDaysInMonth;
                var dates = new List<DateTime>();

                // Loop from the first day of the month until we hit the next month, moving forward a day at a time
                for (var date = new DateTime(Period.StartMonth.Value.Year, Period.StartMonth.Value.Month, 1); date.Month == Period.StartMonth.Value.Month; date = date.AddDays(1))
                {
                    dates.Add(date);
                }


                foreach (var item in dates)
                {

                    workSheet.Cells[rowIndex, col].Value = item.ToString("ddd");

                    col = col + 1;

                    workSheet.Cells[rowIndexNumber, colNumber].Value = item.Day;
                    colNumber++;
                    //workSheet.Cells["G3"].Value = weekPeriod.WeekStatrdate;
                }




                package.Save();
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

            //string fileName = "Absence table for period" + result.Select(x => x.PaymentDurationName).FirstOrDefault() + ".xlsx";
            //return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);



            #endregion


            #region Generate for each the file 

            #endregion




            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                // SendConfirmationReceivingModelEmail(EntityPK);
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.InternationalStaffEntitlementPeriodDataTable, DbAHD.PrimaryKeyControl(Period), DbAHD.RowVersionControls(Portal.SingleToList(Period))));
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            //catch (Exception ex)
            //{
            //    return Json(DbAHD.ErrorMessage(ex.Message));
            //}
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult InternationalStaffEntitlementPeriodUpdate(InternationalStaffEntitlementPeriodUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Update, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/AHD/Views/InternationalStaffEntitlementPeriods/_InternationalStaffEntitlementPeriodForm.cshtml", model);
            DateTime ExecutionTime = DateTime.Now;
            dataAHDPeriodEntitlement InternationalStaffEntitlementPeriod = Mapper.Map(model, new dataAHDPeriodEntitlement());
            DbAHD.Update(InternationalStaffEntitlementPeriod, Permissions.InternationalStaffEntitlements.UpdateGuid, ExecutionTime, DbCMS);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.InternationalStaffEntitlementPeriodDataTable, DbAHD.PrimaryKeyControl(InternationalStaffEntitlementPeriod), DbAHD.RowVersionControls(Portal.SingleToList(InternationalStaffEntitlementPeriod))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }




        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult InternationalStaffEntitlementPeriodDelete(dataAHDPeriodEntitlement model)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Delete, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            List<dataAHDPeriodEntitlement> DeletedInternationalStaffEntitlementPeriod = DeleteInternationalStaffEntitlementPeriod(Portal.SingleToList(model));
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.InternationalStaffEntitlements.Restore, Apps.AHD), Container = " InternationalStaffEntitlementPeriodFormControls" });
            try
            {
                int CommitedRows = DbAHD.SaveChanges();
                DbAHD.SaveChanges();
                return Json(DbAHD.SingleDeleteMessage(CommitedRows, DeletedInternationalStaffEntitlementPeriod.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyInternationalStaffEntitlementPeriod((Guid)model.PeriodEntitlementGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult InternationalStaffEntitlementPeriodInternationalStaffEntitlementPeriodRestore(dataAHDPeriodEntitlement model)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Restore, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            if (ActiveInternationalStaffEntitlementPeriod(model))
            {
                return Json(DbAHD.RecordExists());
            }
            List<dataAHDPeriodEntitlement> RestoredInternationalStaffEntitlementPeriod = RestoreInternationalStaffEntitlementPeriods(Portal.SingleToList(model));
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.InternationalStaffEntitlements.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action(" InternationalStaffEntitlementPeriodCreate", "Configuration", new { Area = "AHD" })), Container = " InternationalStaffEntitlementPeriodFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.InternationalStaffEntitlements.Update, Apps.AHD), Container = " InternationalStaffEntitlementPeriodFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.InternationalStaffEntitlements.Delete, Apps.AHD), Container = " InternationalStaffEntitlementPeriodFormControls" });
            try
            {
                int CommitedRows = DbAHD.SaveChanges();
                DbAHD.SaveChanges();
                return Json(DbAHD.SingleRestoreMessage(CommitedRows, RestoredInternationalStaffEntitlementPeriod, DbAHD.PrimaryKeyControl(RestoredInternationalStaffEntitlementPeriod.FirstOrDefault()), Url.Action(DataTableNames.InternationalStaffEntitlementPeriodDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyInternationalStaffEntitlementPeriod(model.PeriodEntitlementGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult InternationalStaffEntitlementPeriodDataTableDelete(List<dataAHDPeriodEntitlement> models)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Delete, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            List<dataAHDPeriodEntitlement> DeletedInternationalStaffEntitlementPeriod = DeleteInternationalStaffEntitlementPeriod(models);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialDeleteMessage(DeletedInternationalStaffEntitlementPeriod, models, DataTableNames.InternationalStaffEntitlementPeriodDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult InternationalStaffEntitlementPeriodDataTableRestore(List<dataAHDPeriodEntitlement> models)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Restore, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            List<dataAHDPeriodEntitlement> RestoredInternationalStaffEntitlementPeriod = DeleteInternationalStaffEntitlementPeriod(models);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialRestoreMessage(RestoredInternationalStaffEntitlementPeriod, models, DataTableNames.InternationalStaffEntitlementPeriodDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        private List<dataAHDPeriodEntitlement> DeleteInternationalStaffEntitlementPeriod(List<dataAHDPeriodEntitlement> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataAHDPeriodEntitlement> DeletedInternationalStaffEntitlementPeriod = new List<dataAHDPeriodEntitlement>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT PeriodEntitlementGUID,CONVERT(varchar(50), PeriodEntitlementGUID) as C2 ,dataAHDPeriodEntitlementRowVersion FROM code.dataAHDPeriodEntitlement where PeriodEntitlementGUID in (" + string.Join(",", models.Select(x => "'" + x.PeriodEntitlementGUID + "'").ToArray()) + ")";
            string query = DbAHD.QueryBuilder(models, Permissions.InternationalStaffEntitlements.DeleteGuid, SubmitTypes.Delete, "");
            var Records = DbAHD.Database.SqlQuery<dataAHDPeriodEntitlement>(query).ToList();
            foreach (var record in Records)
            {
                DeletedInternationalStaffEntitlementPeriod.Add(DbAHD.Delete(record, ExecutionTime, Permissions.InternationalStaffEntitlements.DeleteGuid, DbCMS));
            }
            return DeletedInternationalStaffEntitlementPeriod;
        }
        private List<dataAHDPeriodEntitlement> RestoreInternationalStaffEntitlementPeriods(List<dataAHDPeriodEntitlement> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataAHDPeriodEntitlement> RestoredInternationalStaffEntitlementPeriod = new List<dataAHDPeriodEntitlement>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT PeriodEntitlementGUID,CONVERT(varchar(50), PeriodEntitlementGUID) as C2 ,dataAHDPeriodEntitlementRowVersion FROM code.dataAHDPeriodEntitlement where PeriodEntitlementGUID in (" + string.Join(",", models.Select(x => "'" + x.PeriodEntitlementGUID + "'").ToArray()) + ")";

            string query = DbAHD.QueryBuilder(models, Permissions.InternationalStaffEntitlements.DeleteGuid, SubmitTypes.Restore, "");

            var Records = DbAHD.Database.SqlQuery<dataAHDPeriodEntitlement>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveInternationalStaffEntitlementPeriod(record))
                {
                    RestoredInternationalStaffEntitlementPeriod.Add(DbAHD.Restore(record, Permissions.InternationalStaffEntitlements.DeleteGuid, Permissions.InternationalStaffEntitlements.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredInternationalStaffEntitlementPeriod;
        }

        private JsonResult ConcurrencyInternationalStaffEntitlementPeriod(Guid PK)
        {
            dataAHDPeriodEntitlement dbModel = new dataAHDPeriodEntitlement();

            var InternationalStaffEntitlementPeriod = DbAHD.dataAHDPeriodEntitlement.Where(x => x.PeriodEntitlementGUID == PK).FirstOrDefault();
            var dbInternationalStaffEntitlementPeriod = DbAHD.Entry(InternationalStaffEntitlementPeriod).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbInternationalStaffEntitlementPeriod, dbModel);

            if (InternationalStaffEntitlementPeriod.dataAHDPeriodEntitlementRowVersion.SequenceEqual(dbModel.dataAHDPeriodEntitlementRowVersion))
            {
                return Json(DbAHD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbAHD, dbModel, "LanguagesContainer"));
        }
        private bool ActiveInternationalStaffEntitlementPeriod(Object model)
        {
            dataAHDPeriodEntitlement period = Mapper.Map(model, new dataAHDPeriodEntitlement());
            int ModelDescription = DbAHD.dataAHDPeriodEntitlement
                                    .Where(x => x.StartMonth == period.StartMonth &&
                                                x.EndMonth == period.EndMonth &&
                                                x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("New Entitlemtns ", " already exists");
            }
            return (ModelDescription > 0);
        }

        //public List<dataAHDInternationalStaffEntitlementDetail> CalculateEntitlementDetailForStaff(Guid StaffGUID,Guid InternationalStaffEntitlementGUID, dataAHDPeriodEntitlement period)
        //{
        //    List<dataAHDInternationalStaffEntitlementDetail> model = new List<dataAHDInternationalStaffEntitlementDetail>();
        //    DbAHD.dataInternationalStaffAttendance.Where(x => x.FromDate >= period.StartMonth && x.FromDate < period.EndMonth && x.StaffGUID == StaffGUID).ToList()
        //    return model;

        //}
        #endregion

        #region Entitlement
        //[Route("AHD/InternationalStaffEntitlementIndexAll/{PK}")]
        public ActionResult InternationalStaffEntitlementIndex(Guid PK)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Access, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            InternationalStaffEntitlementUpdateModel model = new InternationalStaffEntitlementUpdateModel();
            model.PeriodEntitlementGUID = PK;
            return View("~/Areas/AHD/Views/InternationalStaffEntitlement/Index.cshtml", model);
        }

        [Route("AHD/InternationalStaffEntitlement/WorkplaceInternationalStaffEntitlement/")]
        public ActionResult WorkplaceInternationalStaffEntitlement(Guid? id)
        {
            if (id == null)
                id = UserGUID;
            var myModel = DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.StaffGUID == id

                       ).FirstOrDefault();
            if (myModel == null || id != UserGUID)
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return View("~/Areas/AHD/Views/InternationalStaffEntitlement/StaffIndex.cshtml", new InternationalStaffEntitlementUpdateModel { StaffGUID = myModel.StaffGUID });
        }
        public JsonResult InternationalMyStaffEntitlementDataTable(DataTableRecievedOptions options, Guid PK)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<InternationalStaffEntitlementDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<InternationalStaffEntitlementDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.InternationalStaffEntitlements.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix




            var All = (
                from a in DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.StaffGUID == PK && x.FlowStatusGUID != InternationalStaffEntitlmentFlowStatus.Submitted).AsExpandable()
                join b in DbAHD.v_StaffProfileInformation on a.StaffGUID equals b.UserGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbAHD.dataAHDInternationalStaffEntitlementFlow.Where(x => x.IsLastAction == true && x.Active) on a.InternationalStaffEntitlementGUID equals c.InternationalStaffEntitlementGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()

                select new InternationalStaffEntitlementDataTableModel
                {
                    InternationalStaffEntitlementGUID = a.InternationalStaffEntitlementGUID.ToString(),
                    StaffGUID = a.StaffGUID.ToString(),
                    StaffName = a.StaffName,
                    PeriodEntitlementGUID = a.PeriodEntitlementGUID.ToString(),
                    FlowStatusGUID = a.FlowStatusGUID.ToString(),
                    LastFlowStatusName = a.LastFlowStatusName,
                    Month = a.dataAHDPeriodEntitlement.MonthName,
                    OrderId = a.dataAHDPeriodEntitlement.OrderId,
                    ReceiptDate = a.ReceiptDate,
                    ConfirmReceipt = a.IsConfirmReceipt == true ? "Yes" : "No",

                    DutyStation = R1.DutyStation,
                    DutyStationGUID = R1.DutyStationGUID.ToString(),
                    Department = R1.DepartmentDescription,
                    DepartmentGUID = R1.DepartmentGUID.ToString(),
                    TotalEntitlements = a.dataAHDInternationalStaffEntitlementDetail.Where(f => f.IsToAdd == true).Select(f => f.TotalAmount).Sum() != null ? a.dataAHDInternationalStaffEntitlementDetail.Where(f => f.IsToAdd == true).Select(f => f.TotalAmount).Sum() - (a.dataAHDInternationalStaffEntitlementDetail.Where(f => f.IsToAdd == false).Select(f => f.TotalAmount).Sum() ?? 0) : 0 - a.dataAHDInternationalStaffEntitlementDetail.Where(f => f.IsToAdd == false).Select(f => f.TotalAmount).Sum() != null ? a.dataAHDInternationalStaffEntitlementDetail.Where(f => f.IsToAdd == false).Select(f => f.TotalAmount).Sum() : 0,
                    Active = a.Active,
                    ReferenceNumber = a.ReferenceNumber,
                    IndexNumber = a.IndexNumber,
                    TransferTo = R2.userPersonalDetails != null ? R2.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN).FirstOrDefault().FirstName + " " + R2.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN).FirstOrDefault().Surname : "",
                    TransferDate = R2.ActionDate,

                    PaymentMethodGUID = a.PaymentMethodGUID,

                    dataAHDInternationalStaffEntitlementRowVersion = a.dataAHDInternationalStaffEntitlementRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All.OrderByDescending(x => x.OrderId), DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<InternationalStaffEntitlementDataTableModel> Result = Mapper.Map<List<InternationalStaffEntitlementDataTableModel>>(All.OrderByDescending(x => x.OrderId).OrderByDescending(x => x.Month).Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result.OrderByDescending(x => x.OrderId)), JsonRequestBehavior.AllowGet);
        }
        //[Route("AHD/InternationalStaffEntitlementDataTable/")]
        public JsonResult InternationalStaffEntitlementDataTable(DataTableRecievedOptions options, Guid PK)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<InternationalStaffEntitlementDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<InternationalStaffEntitlementDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.InternationalStaffEntitlements.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix



            var All = (
                from a in DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.PeriodEntitlementGUID == PK).AsExpandable()
                join b in DbAHD.v_StaffProfileInformation on a.StaffGUID equals b.UserGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbAHD.dataAHDInternationalStaffEntitlementFlow.Where(x => x.IsLastAction == true && x.Active) on a.InternationalStaffEntitlementGUID equals c.InternationalStaffEntitlementGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()
                join d in DbAHD.codeTablesValuesLanguages.Where(x => true && x.Active) on a.PaymentMethodGUID equals d.ValueGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()

                select new InternationalStaffEntitlementDataTableModel
                {
                    InternationalStaffEntitlementGUID = a.InternationalStaffEntitlementGUID.ToString(),
                    StaffGUID = a.StaffGUID.ToString(),
                    StaffName = a.StaffName,
                    PeriodEntitlementGUID = a.PeriodEntitlementGUID.ToString(),
                    FlowStatusGUID = a.FlowStatusGUID.ToString(),
                    LastFlowStatusName = a.LastFlowStatusName,
                    Month = a.dataAHDPeriodEntitlement.MonthName,
                    FinanceComment=a.FinanceComment,

                    DutyStation = R1.DutyStation,
                    DutyStationGUID = R1.DutyStationGUID.ToString(),
                    Department = R1.DepartmentDescription,
                    DepartmentGUID = R1.DepartmentGUID.ToString(),
                    TotalEntitlements = a.dataAHDInternationalStaffEntitlementDetail.Where(f => f.IsToAdd == true).Select(f => f.TotalAmount).Sum() != null ? a.dataAHDInternationalStaffEntitlementDetail.Where(f => f.IsToAdd == true).Select(f => f.TotalAmount).Sum() - (a.dataAHDInternationalStaffEntitlementDetail.Where(f => f.IsToAdd == false).Select(f => f.TotalAmount).Sum() ?? 0) : 0 - a.dataAHDInternationalStaffEntitlementDetail.Where(f => f.IsToAdd == false).Select(f => f.TotalAmount).Sum() != null ? a.dataAHDInternationalStaffEntitlementDetail.Where(f => f.IsToAdd == false).Select(f => f.TotalAmount).Sum() : 0,
                    Active = a.Active,
                    ReferenceNumber = a.ReferenceNumber,
                    IndexNumber = a.IndexNumber,
                    TransferTo = R2.userPersonalDetails != null ? R2.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN).FirstOrDefault().FirstName + " " + R2.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN).FirstOrDefault().Surname : "",
                    TransferDate = R2.ActionDate,
                    PaymentMethod = R3.ValueDescription,

                    PaymentMethodGUID = a.PaymentMethodGUID,

                    dataAHDInternationalStaffEntitlementRowVersion = a.dataAHDInternationalStaffEntitlementRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<InternationalStaffEntitlementDataTableModel> Result = Mapper.Map<List<InternationalStaffEntitlementDataTableModel>>(All.OrderBy(x => x.StaffName).Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }




        public ActionResult InternationalStaffEntitlementUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Access, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            InternationalStaffEntitlementUpdateModel model = new InternationalStaffEntitlementUpdateModel();
            var details = DbAHD.dataAHDInternationalStaffEntitlementDetail.Where(x => x.dataAHDInternationalStaffEntitlement.InternationalStaffEntitlementGUID == PK).ToList();
            var results = DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.InternationalStaffEntitlementGUID == PK).FirstOrDefault();
            var pers = DbAHD.userPersonalDetailsLanguage.Where(x => (x.UserGUID == results.PreparedByGUID || x.UserGUID == results.CertifiedByGUID || x.UserGUID == results.FinanceApprovedByGUID) && x.LanguageID == LAN).ToList();
            model.InternationalStaffEntitlementGUID = PK;
            model.StaffGUID = results.StaffGUID;
            model.StaffName = results.StaffName;
            model.CertifierComment = results.CertifierComment;
            model.StaffComment = results.StaffComment;
            model.LastFlowStatusName = results.LastFlowStatusName;
            model.FlowStatusGUID = results.FlowStatusGUID;
            // model.TransferTo = results.dataAHDInternationalStaffEntitlementFlow.Where(x=>x.Active && x.IsLastAction==true).FirstOrDefault().userPersonalDetails!=null? results.dataAHDInternationalStaffEntitlementFlow.Where(x => x.Active && x.IsLastAction == true).FirstOrDefault().userPersonalDetails.userPersonalDetailsLanguage.Where(x=>x.Active && x.LanguageID==LAN).FirstOrDefault().FirstName + " "+ results.dataAHDInternationalStaffEntitlementFlow.Where(x => x.Active && x.IsLastAction == true).FirstOrDefault().userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN).FirstOrDefault().Surname: "";
            model.TotalEntitlements = details.Where(f => f.IsToAdd == true).Select(f => f.TotalAmount).Sum() != null ? details.Where(f => f.IsToAdd == true).Select(f => f.TotalAmount).Sum() - (details.Where(f => f.IsToAdd == false).Select(f => f.TotalAmount).Sum() ?? 0) : 0 - details.Where(f => f.IsToAdd == false).Select(f => f.TotalAmount).Sum() != null ? details.Where(f => f.IsToAdd == false).Select(f => f.TotalAmount).Sum() : 0;
            model.PeriodName = results.dataAHDPeriodEntitlement.MonthName;
            model.PreparedBy = results.PreparedByGUID != null ? pers.Where(x => x.UserGUID == results.PreparedByGUID).FirstOrDefault().FirstName + " " + pers.Where(x => x.UserGUID == results.PreparedByGUID).FirstOrDefault().Surname : " ";
            model.CertifiedBy = results.CertifiedByGUID != null ? pers.Where(x => x.UserGUID == results.CertifiedByGUID).FirstOrDefault().FirstName + " " + pers.Where(x => x.UserGUID == results.CertifiedByGUID).FirstOrDefault().Surname : " ";
            model.FinanceApprovedBy = results.FinanceApprovedByGUID != null ? pers.Where(x => x.UserGUID == results.FinanceApprovedByGUID).FirstOrDefault().FirstName + " " +
                            pers.Where(x => x.UserGUID == results.FinanceApprovedByGUID).FirstOrDefault().Surname : " ";
            model.FinanceApprovedDate = results.FinanceApprovedDate;
            model.FinanceComment = results.FinanceComment;

            model.CertifiedDate = results.CertifiedDate;
            model.PreparedDate = results.PreparedDate;
            model.RequestStage = model.FlowStatusGUID == InternationalStaffEntitlmentFlowStatus.PendingVerification ? 2 :
              (model.FlowStatusGUID == InternationalStaffEntitlmentFlowStatus.PendingCertify ? 3 :
              (model.FlowStatusGUID == InternationalStaffEntitlmentFlowStatus.PendingFinanceApproval ? 4 :
              (model.FlowStatusGUID == InternationalStaffEntitlmentFlowStatus.Closed ? 5 : 0
              ))
              );
            ViewBag.Stage = model.RequestStage;
            return View("~/Areas/AHD/Views/InternationalStaffEntitlement/InternationalStaffEntitlement.cshtml", model);
        }


        public ActionResult InternationalStaffEntitlementForStaffUpdate(Guid PK)
        {
            InternationalStaffEntitlementUpdateModel model = new InternationalStaffEntitlementUpdateModel();
            var results = DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.InternationalStaffEntitlementGUID == PK).FirstOrDefault();
            if (results.StaffGUID != UserGUID)
            {
                return Json(DbAHD.PermissionError());
            }

            model.InternationalStaffEntitlementGUID = PK;
            model.StaffGUID = results.StaffGUID;
            model.StaffName = results.StaffName;
            model.PeriodName = results.dataAHDPeriodEntitlement.MonthName;
            model.FlowStatusGUID = results.FlowStatusGUID;
            model.LastFlowStatusName = results.LastFlowStatusName;
            model.IsConfirmReceipt = results.IsConfirmReceipt;
            model.ReceiptDate = results.ReceiptDate;

            var details = DbAHD.dataAHDInternationalStaffEntitlementDetail.Where(x => x.dataAHDInternationalStaffEntitlement.InternationalStaffEntitlementGUID == PK).ToList();
            model.TotalEntitlements = details.Where(f => f.IsToAdd == true).Select(f => f.TotalAmount).Sum() != null ? details.Where(f => f.IsToAdd == true).Select(f => f.TotalAmount).Sum() - (details.Where(f => f.IsToAdd == false).Select(f => f.TotalAmount).Sum() ?? 0) : 0 - details.Where(f => f.IsToAdd == false).Select(f => f.TotalAmount).Sum() != null ? details.Where(f => f.IsToAdd == false).Select(f => f.TotalAmount).Sum() : 0;

            model.RequestStage = model.FlowStatusGUID == InternationalStaffEntitlmentFlowStatus.PendingVerification ? 2 :
                   (model.FlowStatusGUID == InternationalStaffEntitlmentFlowStatus.PendingCertify ? 3 :
                   (model.FlowStatusGUID == InternationalStaffEntitlmentFlowStatus.PendingFinanceApproval ? 4 :
                   (model.FlowStatusGUID == InternationalStaffEntitlmentFlowStatus.Closed ? 5 : 0
                   ))
                   );
            ViewBag.Stage = model.RequestStage;
            var pers = DbAHD.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).ToList();

            model.PreparedBy = results.PreparedByGUID != null ? pers.Where(x => x.UserGUID == results.PreparedByGUID).FirstOrDefault().FirstName + " " + pers.Where(x => x.UserGUID == results.PreparedByGUID).FirstOrDefault().Surname : " ";
            model.CertifiedBy = results.CertifiedByGUID != null ? pers.Where(x => x.UserGUID == results.CertifiedByGUID).FirstOrDefault().FirstName + " " + pers.Where(x => x.UserGUID == results.CertifiedByGUID).FirstOrDefault().Surname : " ";
            model.FinanceApprovedBy = results.FinanceApprovedByGUID != null ? pers.Where(x => x.UserGUID == results.FinanceApprovedByGUID).FirstOrDefault().FirstName + " " +
                            pers.Where(x => x.UserGUID == results.FinanceApprovedByGUID).FirstOrDefault().Surname : " ";
            model.FinanceApprovedDate = results.FinanceApprovedDate;
            model.CertifiedDate = results.CertifiedDate;
            model.PreparedDate = results.PreparedDate;



            return View("~/Areas/AHD/Views/InternationalStaffEntitlement/InternationalStaffEntitlementReview.cshtml", model);
        }

        public JsonResult InternationalStaffEntitlementDetailDataTable(DataTableRecievedOptions options, Guid PK)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<InternationalStaffEntitlDetailementDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<InternationalStaffEntitlDetailementDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.InternationalStaffEntitlements.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
            //var r = DbAHD.dataAHDInternationalStaffEntitlementDetail.Where(x=>x.InternationalStaffEntitlementGUID==PK).ToList();
            Guid rentalGUID = Guid.Parse("C328588B-33C5-4E38-85B0-6409704BE434");

            var All = (
                from a in DbAHD.dataAHDInternationalStaffEntitlementDetail.Where(x => x.InternationalStaffEntitlementGUID == PK).AsExpandable()
                    //join b in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.FlowStatusGUID equals b.ValueGUID into LJ1
                    //from R1 in LJ1.DefaultIfEmpty()

                select new InternationalStaffEntitlDetailementDataTableModel
                {
                    InternationalStaffEntitlementDetailGUID = a.InternationalStaffEntitlementDetailGUID.ToString(),
                    InternationalStaffEntitlementGUID = a.InternationalStaffEntitlementGUID.ToString(),
                    EntitlementTypeGUID = a.EntitlementTypeGUID.ToString(),
                    BasePeriodAmount = a.codeAHDEntitlementType.codeAHDEntitlementTypePerDutyStation.FirstOrDefault() != null ? a.codeAHDEntitlementType.codeAHDEntitlementTypePerDutyStation.FirstOrDefault().EntitlementValue : 0,
                    StaffGUID = a.dataAHDInternationalStaffEntitlement.StaffGUID.ToString(),
                    StaffName = a.dataAHDInternationalStaffEntitlement.StaffName,
                    PeriodName = a.dataAHDInternationalStaffEntitlement.dataAHDPeriodEntitlement.MonthName.ToString(),
                    EntitlementType = a.codeAHDEntitlementType.EntitlementTypeName,
                    TotalAmount = a.IsToAdd == false ? -1 * (a.TotalAmount == null ? 0 : a.TotalAmount) : (a.TotalAmount == null ? 0 : a.TotalAmount),
                    TotalDays = a.TotalDays,
                    IsToAdd = a.IsToAdd,
                    Type = a.IsToAdd == false ? "Deduction(-)" : "Added(+)",
                    Comments = a.Comments,
                    Active = a.Active,

                    dataAHDInternationalStaffEntitlementDetailRowVersion = a.dataAHDInternationalStaffEntitlementDetailRowVersion
                }).Where(Predicate);
            //var s=All.ToList();
            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<InternationalStaffEntitlDetailementDataTableModel> Result = Mapper.Map<List<InternationalStaffEntitlDetailementDataTableModel>>(All.OrderByDescending(x => x.IsToAdd).Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public JsonResult InternationalStaffAttendanceDetailDataTable(DataTableRecievedOptions options, Guid PK)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<InternationalStaffPresenceAttendanceDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<InternationalStaffPresenceAttendanceDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.InternationalStaffEntitlements.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
            //var r = DbAHD.dataAHDInternationalStaffEntitlementDetail.Where(x=>x.InternationalStaffEntitlementGUID==PK).ToList();
            var _entit = DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.InternationalStaffEntitlementGUID == PK).FirstOrDefault();
            var allattendaces = DbAHD.dataInternationalStaffAttendance.Where(x => x.StaffGUID == _entit.StaffGUID).ToList();
            var mycurrAttendances = allattendaces.Where(x => x.FromDate >= _entit.dataAHDPeriodEntitlement.StartMonth
                                              && x.ToDate <= _entit.dataAHDPeriodEntitlement.EndMonth).ToList();

            var All = (
                from a in DbAHD.dataInternationalStaffAttendance.Where(x => x.StaffGUID == _entit.StaffGUID
                && x.FromDate >= _entit.dataAHDPeriodEntitlement.StartMonth
                                              && x.ToDate <= _entit.dataAHDPeriodEntitlement.EndMonth).AsExpandable()
                join b in DbAHD.codeAHDInternationalStaffAttendanceType.Where(x => x.Active) on a.InternationalStaffAttendanceTypeGUID equals b.InternationalStaffAttendanceTypeGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()

                select new InternationalStaffPresenceAttendanceDataTableModel
                {
                    InternationalStaffAttendanceGUID = a.InternationalStaffAttendanceGUID.ToString(),
                    InternationalStaffAttendanceTypeGUID = a.InternationalStaffAttendanceTypeGUID.ToString(),

                    LeaveType = R1.AttendanceTypeName.ToString(),
                    FromDate = a.FromDate,
                    ToDate = a.ToDate,
                    Comments = a.Comments,
                    Active = a.Active,

                    dataInternationalStaffAttendanceRowVersion = a.dataInternationalStaffAttendanceRowVersion
                }).Where(Predicate);
            //var s=All.ToList();
            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<InternationalStaffPresenceAttendanceDataTableModel> Result = Mapper.Map<List<InternationalStaffPresenceAttendanceDataTableModel>>(All.OrderBy(x => x.FromDate).Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        //[Route("AHD/InternationalStaffEntitlementDetail/Create/")]
        public ActionResult InternationalStaffEntitlementDetailCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            var myInternationalStaffEntitlement = DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.InternationalStaffEntitlementGUID == FK).FirstOrDefault();
            return PartialView("~/Areas/AHD/Views/InternationalStaffEntitlement/_InternationalStaffEntitlementDetailModal.cshtml", new InternationalStaffEntitlDetailementUpdateModel { InternationalStaffEntitlementDetailGUID = Guid.Empty, InternationalStaffEntitlementGUID = FK, IsToAdd = false, StaffGUID = myInternationalStaffEntitlement.StaffGUID });

        }

        //[Route("AHD/NationalStaffDangerPayInformation/Update/{PK}")]
        public ActionResult InternationalStaffEntitlementDetailUpdate(Guid PK)
        {
            var model = (from a in DbAHD.dataAHDInternationalStaffEntitlementDetail.WherePK(PK)
                         select new InternationalStaffEntitlDetailementUpdateModel
                         {
                             InternationalStaffEntitlementDetailGUID = a.InternationalStaffEntitlementDetailGUID,
                             InternationalStaffEntitlementGUID = a.InternationalStaffEntitlementGUID,
                             EntitlementTypeGUID = a.EntitlementTypeGUID,
                             BasePeriodAmount = a.BasePeriodAmount,
                             TotalAmount = a.TotalAmount,
                             TotalDays = a.TotalDays,
                             IsToAdd = a.IsToAdd,
                             Comments = a.Comments,
                             dataAHDInternationalStaffEntitlementDetailRowVersion = a.dataAHDInternationalStaffEntitlementDetailRowVersion,

                             Active = a.Active,

                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("InternationalStaffEntitlementDetailUpdate", "InternationalStaffEntitlement", new { Area = "AHD" }));
            return PartialView("~/Areas/AHD/Views/InternationalStaffEntitlement/_InternationalStaffEntitlementDetailModal.cshtml", model);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult InternationalStaffEntitlementDetailCreate(InternationalStaffEntitlDetailementUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/AHD/Views/InternationalStaffEntitlement/_InternationalStaffEntitlementDetailModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();
            var UserStation = DbAHD.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            dataAHDInternationalStaffEntitlementDetail detail = Mapper.Map(model, new dataAHDInternationalStaffEntitlementDetail());

            detail.InternationalStaffEntitlementDetailGUID = EntityPK;
            detail.IsToAdd = model.IsToAdd == true ? true : false;
            detail.BasePeriodAmount = DbAHD.codeAHDEntitlementTypePerDutyStation.Where(x => x.EntitlementTypeGUID == model.EntitlementTypeGUID && x.DutyStationGUID == UserStation.DutyStationGUID).FirstOrDefault() != null ? DbAHD.codeAHDEntitlementTypePerDutyStation.Where(x => x.EntitlementTypeGUID == model.EntitlementTypeGUID && x.DutyStationGUID == UserStation.DutyStationGUID).FirstOrDefault().EntitlementValue : 0;
            DbAHD.Create(detail, Permissions.InternationalStaffEntitlements.CreateGuid, ExecutionTime, DbCMS);


            // Guid maksoud = Guid.Parse("8F7EF83F-FD3E-4F8C-8735-8A22D3D61B75");
            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.InternationalStaffEntitlementDetailDataTable, ControllerContext, "NationalStaffDangerPayInformationLanguagesFormControls"));
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.InternationalStaffEntitlements.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("Create", "NationalStaffDangerPayInformations", new { Area = "AHD" })), Container = "NationalStaffDangerPayInformationDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.InternationalStaffEntitlements.Update, Apps.AHD), Container = "NationalStaffDangerPayInformationDetailFormControls" });

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                //SendConfirmationReceivingModelEmail(EntityPK);
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.InternationalStaffEntitlementDetailDataTable, DbAHD.PrimaryKeyControl(detail), DbAHD.RowVersionControls(Portal.SingleToList(detail))));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult InternationalStaffEntitlementDetailUpdate(InternationalStaffEntitlDetailementUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Update, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/AHD/Views/InternationalStaffEntitlement/_InternationalStaffEntitlementDetailModal.cshtml", model);
            DateTime ExecutionTime = DateTime.Now;
            dataAHDInternationalStaffEntitlementDetail detail = Mapper.Map(model, new dataAHDInternationalStaffEntitlementDetail());
            DbAHD.Update(detail, Permissions.InternationalStaffEntitlements.UpdateGuid, ExecutionTime, DbCMS);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.InternationalStaffEntitlementDetailDataTable, DbAHD.PrimaryKeyControl(detail), DbAHD.RowVersionControls(Portal.SingleToList(detail))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyInternationalStaffEntitlementDetail((Guid)model.InternationalStaffEntitlementDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult InternationalStaffEntitlementDetailDelete(dataAHDInternationalStaffEntitlementDetail model)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Delete, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            List<dataAHDInternationalStaffEntitlementDetail> deleted = DeleteInternationalStaffEntitlementDetail(Portal.SingleToList(model));
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.InternationalStaffEntitlements.Restore, Apps.AHD), Container = "NationalStaffDangerPayInformationFormControls" });
            try
            {
                int CommitedRows = DbAHD.SaveChanges();
                DbAHD.SaveChanges();
                return Json(DbAHD.SingleDeleteMessage(CommitedRows, deleted.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyInternationalStaffEntitlementDetail(model.InternationalStaffEntitlementDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NationalStaffDangerPayInformationRestore(dataAHDInternationalStaffEntitlementDetail model)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Restore, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            if (ActiveInternationalStaffEntitlementDetail(model))
            {
                return Json(DbAHD.RecordExists());
            }
            List<dataAHDInternationalStaffEntitlementDetail> RestoredNationalStaffDangerPayInformation = RestoreNationalStaffDangerPayInformations(Portal.SingleToList(model));
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.InternationalStaffEntitlements.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("NationalStaffDangerPayInformationCreate", "Configuration", new { Area = "AHD" })), Container = "NationalStaffDangerPayInformationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.InternationalStaffEntitlements.Update, Apps.AHD), Container = "NationalStaffDangerPayInformationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.InternationalStaffEntitlements.Delete, Apps.AHD), Container = "NationalStaffDangerPayInformationFormControls" });
            try
            {
                int CommitedRows = DbAHD.SaveChanges();
                DbAHD.SaveChanges();
                return Json(DbAHD.SingleRestoreMessage(CommitedRows, RestoredNationalStaffDangerPayInformation, DbAHD.PrimaryKeyControl(RestoredNationalStaffDangerPayInformation.FirstOrDefault()), Url.Action(DataTableNames.InternationalStaffEntitlementDetailDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyInternationalStaffEntitlementDetail(model.InternationalStaffEntitlementDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult InternationalStaffEntitlementsDataTableDelete(List<dataAHDInternationalStaffEntitlementDetail> models)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Delete, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            List<dataAHDInternationalStaffEntitlementDetail> deleted = DeleteInternationalStaffEntitlementDetail(models);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialDeleteMessage(deleted, models, DataTableNames.InternationalStaffEntitlementDetailDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult InternationalStaffEntitlementsDataTableRestore(List<dataAHDInternationalStaffEntitlementDetail> models)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Restore, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            List<dataAHDInternationalStaffEntitlementDetail> RestoredNationalStaffDangerPayInformation = DeleteInternationalStaffEntitlementDetail(models);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialRestoreMessage(RestoredNationalStaffDangerPayInformation, models, DataTableNames.InternationalStaffEntitlementDetailDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        private List<dataAHDInternationalStaffEntitlementDetail> DeleteInternationalStaffEntitlementDetail(List<dataAHDInternationalStaffEntitlementDetail> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataAHDInternationalStaffEntitlementDetail> DeletedNationalStaffDangerPayInformation = new List<dataAHDInternationalStaffEntitlementDetail>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT DangerPayInformationGUID,CONVERT(varchar(50), DangerPayInformationGUID) as C2 ,dataAHDInternationalStaffEntitlementDetailRowVersion FROM code.dataAHDInternationalStaffEntitlementDetail where DangerPayInformationGUID in (" + string.Join(",", models.Select(x => "'" + x.DangerPayInformationGUID + "'").ToArray()) + ")";
            string query = DbAHD.QueryBuilder(models, Permissions.InternationalStaffEntitlements.DeleteGuid, SubmitTypes.Delete, "");
            var Records = DbAHD.Database.SqlQuery<dataAHDInternationalStaffEntitlementDetail>(query).ToList();
            foreach (var record in Records)
            {
                DeletedNationalStaffDangerPayInformation.Add(DbAHD.Delete(record, ExecutionTime, Permissions.InternationalStaffEntitlements.DeleteGuid, DbCMS));
            }
            return DeletedNationalStaffDangerPayInformation;
        }
        private List<dataAHDInternationalStaffEntitlementDetail> RestoreNationalStaffDangerPayInformations(List<dataAHDInternationalStaffEntitlementDetail> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataAHDInternationalStaffEntitlementDetail> RestoredNationalStaffDangerPayInformation = new List<dataAHDInternationalStaffEntitlementDetail>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT DangerPayInformationGUID,CONVERT(varchar(50), DangerPayInformationGUID) as C2 ,dataAHDInternationalStaffEntitlementDetailRowVersion FROM code.dataAHDInternationalStaffEntitlementDetail where DangerPayInformationGUID in (" + string.Join(",", models.Select(x => "'" + x.DangerPayInformationGUID + "'").ToArray()) + ")";

            string query = DbAHD.QueryBuilder(models, Permissions.InternationalStaffEntitlements.DeleteGuid, SubmitTypes.Restore, "");

            var Records = DbAHD.Database.SqlQuery<dataAHDInternationalStaffEntitlementDetail>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveInternationalStaffEntitlementDetail(record))
                {
                    RestoredNationalStaffDangerPayInformation.Add(DbAHD.Restore(record, Permissions.InternationalStaffEntitlements.DeleteGuid, Permissions.InternationalStaffEntitlements.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredNationalStaffDangerPayInformation;
        }

        private JsonResult ConcurrencyInternationalStaffEntitlementDetail(Guid PK)
        {
            DangerPayInformationDataTableModel dbModel = new DangerPayInformationDataTableModel();

            var entitlement = DbAHD.dataAHDInternationalStaffEntitlementDetail.Where(x => x.InternationalStaffEntitlementDetailGUID == PK).FirstOrDefault();
            var dbNationalStaffDangerPayInformation = DbAHD.Entry(entitlement).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbNationalStaffDangerPayInformation, dbModel);

            if (entitlement.dataAHDInternationalStaffEntitlementDetailRowVersion.SequenceEqual(dbModel.dataDangerPayInformationRowVersion))
            {
                return Json(DbAHD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbAHD, dbModel, "LanguagesContainer"));
        }

        private bool ActiveInternationalStaffEntitlementDetail(Object model)
        {
            dataAHDInternationalStaffEntitlementDetail detail = Mapper.Map(model, new dataAHDInternationalStaffEntitlementDetail());
            int ModelDescription = DbAHD.dataAHDInternationalStaffEntitlementDetail
                                    .Where(x => x.InternationalStaffEntitlementGUID == detail.InternationalStaffEntitlementDetailGUID &&
                                                x.EntitlementTypeGUID == detail.EntitlementTypeGUID &&
                                                x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("New Detail ", " already exists");
            }
            return (ModelDescription > 0);
        }


        #endregion

        #region Entitlement Review Flow 

        public JsonResult ChangeStaffPaymentMethodByHR(Guid myInternationalStaffEntitlementGUID, Guid PaymentMethodGUID)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }

            string URL = "";
            string Anchor = "";
            string Link = "";
            DateTime ExecutionTime = DateTime.Now;
            var Entitlemnt = DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.InternationalStaffEntitlementGUID == myInternationalStaffEntitlementGUID).FirstOrDefault();
            Entitlemnt.PaymentMethodGUID = PaymentMethodGUID;


            DbAHD.Update(Entitlemnt, Permissions.InternationalStaffEntitlements.UpdateGuid, ExecutionTime, DbCMS);

            DbAHD.SaveChanges();
            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ReCallEntitlement(Guid myInternationalStaffEntitlementGUID, Guid FlowStatusGUID)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }

            string URL = "";
            string Anchor = "";
            string Link = "";
            DateTime ExecutionTime = DateTime.Now;
            var Entitlemnt = DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.InternationalStaffEntitlementGUID == myInternationalStaffEntitlementGUID).FirstOrDefault();
            Entitlemnt.FlowStatusGUID = FlowStatusGUID;
            var _table = DbAHD.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.ValueGUID == FlowStatusGUID).FirstOrDefault();
            Entitlemnt.FlowStatusGUID = FlowStatusGUID;
            Entitlemnt.LastFlowStatusName = _table.ValueDescription;

            var toChange = DbAHD.dataAHDInternationalStaffEntitlementFlow.Where(x => x.InternationalStaffEntitlementGUID == myInternationalStaffEntitlementGUID

                             && x.IsLastAction == true).FirstOrDefault();
            toChange.IsLastAction = false;
            dataAHDInternationalStaffEntitlementFlow newFlowToReview = new dataAHDInternationalStaffEntitlementFlow
            {

                InternationalStaffEntitlementFlowGUID = Guid.NewGuid(),
                InternationalStaffEntitlementGUID = myInternationalStaffEntitlementGUID,
                CreatedByGUID = UserGUID,
                FlowStatusGUID = FlowStatusGUID,
                ActionDate = ExecutionTime,
                IsLastAction = true,
                OrderId = toChange.OrderId + 1,


            };
            DbAHD.Update(toChange, Permissions.InternationalStaffEntitlements.UpdateGuid, ExecutionTime, DbCMS);
            DbAHD.Create(newFlowToReview, Permissions.InternationalStaffEntitlements.UpdateGuid, ExecutionTime, DbCMS);

            DbAHD.Update(Entitlemnt, Permissions.InternationalStaffEntitlements.UpdateGuid, ExecutionTime, DbCMS);

            DbAHD.SaveChanges();
            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult SendEntitlementToStaffForReview(Guid myInternationalStaffEntitlementGUID, Guid myCertifiedByGUID)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }

            string URL = "";
            string Anchor = "";
            string Link = "";
            DateTime ExecutionTime = DateTime.Now;
            var Entitlemnt = DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.InternationalStaffEntitlementGUID == myInternationalStaffEntitlementGUID).FirstOrDefault();
            if (Entitlemnt.FlowStatusGUID == InternationalStaffEntitlmentFlowStatus.Submitted)
            {
                var toChange = DbAHD.dataAHDInternationalStaffEntitlementFlow.Where(x => x.InternationalStaffEntitlementGUID == myInternationalStaffEntitlementGUID

                  && x.IsLastAction == true).FirstOrDefault();
                toChange.IsLastAction = false;
                dataAHDInternationalStaffEntitlementFlow newFlowToReview = new dataAHDInternationalStaffEntitlementFlow
                {

                    InternationalStaffEntitlementFlowGUID = Guid.NewGuid(),
                    InternationalStaffEntitlementGUID = myInternationalStaffEntitlementGUID,
                    CreatedByGUID = UserGUID,
                    FlowStatusGUID = InternationalStaffEntitlmentFlowStatus.PendingVerification,
                    ActionDate = ExecutionTime,
                    IsLastAction = true,
                    OrderId = toChange.OrderId + 1,


                };
                Entitlemnt.FlowStatusGUID = InternationalStaffEntitlmentFlowStatus.PendingVerification;
                Entitlemnt.LastFlowStatusName = "Pending Verification By Staff";
                Entitlemnt.PreparedByGUID = UserGUID;
                Entitlemnt.PreparedDate = ExecutionTime;
                Entitlemnt.CertifiedByGUID = myCertifiedByGUID;
                //Entitlemnt.FinanceApprovedByGUID = financeApprovedGUID;
                DbAHD.Create(newFlowToReview, Permissions.InternationalStaffEntitlements.CreateGuid, ExecutionTime, DbCMS);
                DbAHD.Update(Entitlemnt, Permissions.InternationalStaffEntitlements.UpdateGuid, ExecutionTime, DbCMS);
                DbAHD.Update(toChange, Permissions.InternationalStaffEntitlements.UpdateGuid, ExecutionTime, DbCMS);
                DbAHD.SaveChanges();

            }
            var monthName = ProcessData.GetMonthName(Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Month);
            var YearName = Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Year;
            var currStaff = DbAHD.userPersonalDetailsLanguage.Where(x => Entitlemnt.StaffGUID == x.UserGUID
                                                                          && x.LanguageID == LAN).FirstOrDefault();
            var currPrepare = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.PreparedByGUID
                                                                     && x.LanguageID == LAN).FirstOrDefault();
            var currPrepartion = DbAHD.userServiceHistory.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            var currAccount = DbAHD.userServiceHistory.Where(x => Entitlemnt.StaffGUID == x.UserGUID).ToList();



            string SubjectMessage = resxEmails.EntitlemenReviewByStaffSubject.Replace("$SurName", currStaff.Surname).Replace("$Month", monthName).Replace("$Year", Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Year.ToString());


            //to send mail to staff 
            // var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();
            URL = AppSettingsKeys.Domain + "/AHD/InternationalStaffEntitlement/EntitlementToReviewByStaffUpdate/?PK=" + new Portal().GUIDToString(myInternationalStaffEntitlementGUID);
            Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmDangerPayReceiving + "</a>";
            Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
            string myFirstName = currStaff.FirstName;
            string mySurName = currStaff.Surname;


            string _message = resxEmails.EntitlementReviewByStaffConfirmation
                .Replace("$FullName", currStaff.FirstName + " " + currStaff.Surname)
                .Replace("$VerifyLink", Anchor)
                .Replace("$SentBy", currPrepare.FirstName + " " + currPrepare.Surname)
                .Replace("$Period ", monthName + " " + YearName);

            if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
            int isRec = 1;
            var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
            string copy_recipients = currPrepartion.EmailAddress;
            Send(myEmail, SubjectMessage, _message, isRec, copy_recipients);
            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);

        }


        public JsonResult SendReminderEntitlementToStaff(Guid myInternationalStaffEntitlementGUID)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }

            string URL = "";
            string Anchor = "";
            string Link = "";
            DateTime ExecutionTime = DateTime.Now;
            var Entitlemnt = DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.InternationalStaffEntitlementGUID == myInternationalStaffEntitlementGUID).FirstOrDefault();
            if (Entitlemnt.FlowStatusGUID == InternationalStaffEntitlmentFlowStatus.PendingVerification)
            {
                var monthName = ProcessData.GetMonthName(Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Month);
                var YearName = Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Year;
                var currStaff = DbAHD.userPersonalDetailsLanguage.Where(x => Entitlemnt.StaffGUID == x.UserGUID
                                                                              && x.LanguageID == LAN).FirstOrDefault();
                var currPrepare = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.PreparedByGUID
                                                                         && x.LanguageID == LAN).FirstOrDefault();
                var currPrepartion = DbAHD.userServiceHistory.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
                var currAccount = DbAHD.userServiceHistory.Where(x => Entitlemnt.StaffGUID == x.UserGUID).ToList();



                //string SubjectMessage = resxEmails.EntitlemenReviewByStaffSubject.Replace("$Month", monthName).Replace("$Year", Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Year.ToString());
                string SubjectMessage = resxEmails.EntitlemenReviewByStaffSubject.Replace("$SurName", currStaff.Surname).Replace("$Month", monthName).Replace("$Year", Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Year.ToString());

                //to send mail to staff 
                // var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();
                URL = AppSettingsKeys.Domain + "/AHD/InternationalStaffEntitlement/EntitlementToReviewByStaffUpdate/?PK=" + new Portal().GUIDToString(myInternationalStaffEntitlementGUID);
                Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmDangerPayReceiving + "</a>";
                Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                string myFirstName = currStaff.FirstName;
                string mySurName = currStaff.Surname;


                string _message = resxEmails.EntitlementReviewByStaffConfirmation
                    .Replace("$FullName", currStaff.FirstName + " " + currStaff.Surname)
                    .Replace("$VerifyLink", Anchor)
                    .Replace("$SentBy", currPrepare.FirstName + " " + currPrepare.Surname)
                    .Replace("$Period ", monthName + " " + YearName);

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
                string copy_recipients = currPrepartion.EmailAddress;
                Send(myEmail, SubjectMessage, _message, isRec, copy_recipients);
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);

            }

            return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);

        }

        //public JsonResult EntitlementToReviewByStaff(Guid id)
        //{
        //    var DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.InternationalStaffEntitlementGUID == id).FirstOrDefault();
        //}
        //[HttpPost]
        public ActionResult EntitlementToReviewByStaffUpdate(Guid PK)
        {

            //if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Access, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            InternationalStaffEntitlementUpdateModel model = new InternationalStaffEntitlementUpdateModel();
            var results = DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.InternationalStaffEntitlementGUID == PK).FirstOrDefault();
            if (results.StaffGUID != UserGUID)
            {
                return Json(DbAHD.PermissionError());
            }
            model.InternationalStaffEntitlementGUID = PK;
            model.StaffGUID = results.StaffGUID;
            model.StaffName = results.StaffName;
            model.PeriodName = results.dataAHDPeriodEntitlement.MonthName;
            model.FlowStatusGUID = results.FlowStatusGUID;
            model.LastFlowStatusName = results.LastFlowStatusName;
            var details = DbAHD.dataAHDInternationalStaffEntitlementDetail.Where(x => x.dataAHDInternationalStaffEntitlement.InternationalStaffEntitlementGUID == PK).ToList();
            model.TotalEntitlements = details.Where(f => f.IsToAdd == true).Select(f => f.TotalAmount).Sum() != null ? details.Where(f => f.IsToAdd == true).Select(f => f.TotalAmount).Sum() - (details.Where(f => f.IsToAdd == false).Select(f => f.TotalAmount).Sum() ?? 0) : 0 - details.Where(f => f.IsToAdd == false).Select(f => f.TotalAmount).Sum() != null ? details.Where(f => f.IsToAdd == false).Select(f => f.TotalAmount).Sum() : 0;


            return View("~/Areas/AHD/Views/InternationalStaffEntitlement/InternationalStaffEntitlementReview.cshtml", model);
        }
        //to add

        public JsonResult SendEntitlementApprovedByStaff(Guid myInternationalStaffEntitlementGUID, string StaffComment, Guid PaymentMethodGUID)
        {
            string URL = "";
            string Anchor = "";
            string Link = "";
            DateTime ExecutionTime = DateTime.Now;
            //Guid hrcertify = Guid.Parse("0A463FE4-FA3F-4000-8F9E-D7217AFDFA7C");

            var Entitlemnt = DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.InternationalStaffEntitlementGUID == myInternationalStaffEntitlementGUID).FirstOrDefault();
            if (Entitlemnt.StaffGUID != UserGUID)
            {
                return Json(DbAHD.PermissionError());
            }
            //var hasCertifyProfileGUIDs = DbCMS.userPermissions.Where(x => x.ActionGUID == hrcertify && x.Active).
            //    Select(x => x.UserProfileGUID).Distinct().ToList();

            // var usersAccounts = DbCMS.userProfiles.Where(x => hasCertifyProfileGUIDs.Contains(x.UserProfileGUID) && x.userServiceHistory.UserGUID== Entitlemnt.CertifiedByGUID).Select(x => new { x.userServiceHistory.EmailAddress, x.userServiceHistory.UserGUID }).Distinct().FirstOrDefault();
            //var userGUIds = usersAccounts;
            var userPers = DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active && x.UserGUID == Entitlemnt.CertifiedByGUID).FirstOrDefault();




            if (Entitlemnt.FlowStatusGUID == InternationalStaffEntitlmentFlowStatus.PendingVerification)
            {
                var toChange = DbAHD.dataAHDInternationalStaffEntitlementFlow.Where(x => x.InternationalStaffEntitlementGUID == myInternationalStaffEntitlementGUID
                                  && x.IsLastAction == true
                                  ).FirstOrDefault();
                toChange.IsLastAction = false;
                List<dataAHDInternationalStaffEntitlementFlow> allFlows = new List<dataAHDInternationalStaffEntitlementFlow>();
                //foreach (var item in usersAccounts)
                //{
                dataAHDInternationalStaffEntitlementFlow newFlowToReview = new dataAHDInternationalStaffEntitlementFlow
                {

                    InternationalStaffEntitlementFlowGUID = Guid.NewGuid(),
                    InternationalStaffEntitlementGUID = myInternationalStaffEntitlementGUID,
                    CreatedByGUID = Entitlemnt.CertifiedByGUID,
                    FlowStatusGUID = InternationalStaffEntitlmentFlowStatus.PendingCertify,
                    ActionDate = ExecutionTime,
                    IsLastAction = true,
                    OrderId = toChange.OrderId + 1,


                };
                allFlows.Add(newFlowToReview);

                //}

                Entitlemnt.FlowStatusGUID = InternationalStaffEntitlmentFlowStatus.PendingCertify;
                Entitlemnt.LastFlowStatusName = "Pending Certifying";
                Entitlemnt.ApprovedByGUID = UserGUID;
                Entitlemnt.ApprovedDate = ExecutionTime;
                Entitlemnt.StaffComment = StaffComment;
                Entitlemnt.PaymentMethodGUID = PaymentMethodGUID;
                DbAHD.CreateBulk(allFlows, Permissions.InternationalStaffEntitlements.CreateGuid, ExecutionTime, DbCMS);
                DbAHD.Update(Entitlemnt, Permissions.InternationalStaffEntitlements.UpdateGuid, ExecutionTime, DbCMS);
                DbAHD.Update(toChange, Permissions.InternationalStaffEntitlements.UpdateGuid, ExecutionTime, DbCMS);
                DbAHD.SaveChanges();
                var monthName = ProcessData.GetMonthName(Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Month);
                var YearName = Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Year;
                //var staffCore = DbAHD.StaffCoreData.Where(x => x.UserGUID == Entitlemnt.StaffGUID).FirstOrDefault();

                if (Entitlemnt != null)
                {

                    //foreach (var myitem in usersAccounts)
                    //{


                    var currStaff = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.StaffGUID
                                                                                  && x.LanguageID == LAN).FirstOrDefault();
                    var _currManagerPersonal = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.CertifiedByGUID
                                                                                   && x.LanguageID == LAN).FirstOrDefault();
                    var currAccount = DbAHD.userServiceHistory.Where(x => x.UserGUID == Entitlemnt.CertifiedByGUID).FirstOrDefault();
                    var _issuer = DbAHD.userServiceHistory.Where(x => x.UserGUID == Entitlemnt.PreparedByGUID).FirstOrDefault();
                    var _staff = DbAHD.userServiceHistory.Where(x => x.UserGUID == Entitlemnt.StaffGUID).FirstOrDefault();

                    var currPrepare = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.PreparedByGUID
                                                                  && x.LanguageID == LAN).FirstOrDefault();

                    //var _currManagerAccount = DbAHD.userServiceHistory.Where(x => x.UserGUID == staffCore.ReportToGUID).ToList();


                    //string SubjectMessage = resxEmails.EntitlemenReviewByStaffSubject.Replace("$Month", monthName).Replace("$Year", Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Year.ToString());
                    string SubjectMessage = resxEmails.EntitlemenReviewByStaffSubject.Replace("$SurName", currStaff.Surname).Replace("$Month", monthName).Replace("$Year", Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Year.ToString());






                    //to send mail to staff 
                    // var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();
                    URL = AppSettingsKeys.Domain + "/AHD/InternationalStaffEntitlement/EntitlementToReviewByManager/?id=" + new Portal().GUIDToString(myInternationalStaffEntitlementGUID);
                    Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmDangerPayReceiving + "</a>";
                    Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                    string mangerFirstName = _currManagerPersonal.FirstName;
                    string managerSurName = _currManagerPersonal.Surname;


                    string _message = resxEmails.EntitlementReviewByManagerConfirmation
                        .Replace("$FullName", mangerFirstName + " " + managerSurName)
                        .Replace("$VerifyLink", Anchor)
                        .Replace("$StaffName", currStaff.FirstName + " " + currStaff.Surname)
                        .Replace("$SentBy", currPrepare.FirstName + " " + currPrepare.Surname)

                        .Replace("$Period ", monthName + " " + YearName);

                    if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                    int isRec = 1;
                    //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
                    string copy_recipients = _issuer.EmailAddress + ";" + _staff.EmailAddress;
                    Send(currAccount.EmailAddress, SubjectMessage, _message, isRec, copy_recipients);
                    return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);

                }

            }

            return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EntitlementToReviewByManager(Guid id)
        {

            InternationalStaffEntitlementUpdateModel model = new InternationalStaffEntitlementUpdateModel();
            var results = DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.InternationalStaffEntitlementGUID == id).FirstOrDefault();
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlementsCertifying.Access, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            var _lastflow = DbAHD.dataAHDInternationalStaffEntitlementFlow.Where(x => x.InternationalStaffEntitlementGUID
            == id && x.Active && x.IsLastAction == true).FirstOrDefault();
            //if (_lastflow.sta != UserGUID)
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            model.InternationalStaffEntitlementGUID = id;
            model.StaffGUID = results.StaffGUID;
            model.StaffName = results.StaffName;
            model.PeriodName = results.dataAHDPeriodEntitlement.MonthName;

            model.FlowStatusGUID = results.FlowStatusGUID;
            model.LastFlowStatusName = results.LastFlowStatusName;

            var details = DbAHD.dataAHDInternationalStaffEntitlementDetail.Where(x => x.dataAHDInternationalStaffEntitlement.InternationalStaffEntitlementGUID == id).ToList();

            var pers = DbAHD.userPersonalDetailsLanguage.Where(x => (x.UserGUID == results.PreparedByGUID || x.UserGUID == results.CertifiedByGUID || x.UserGUID == results.FinanceApprovedByGUID) && x.LanguageID == LAN).ToList();


            model.RequestStage = model.FlowStatusGUID == InternationalStaffEntitlmentFlowStatus.PendingVerification ? 2 :
              (model.FlowStatusGUID == InternationalStaffEntitlmentFlowStatus.PendingCertify ? 3 :
              (model.FlowStatusGUID == InternationalStaffEntitlmentFlowStatus.PendingFinanceApproval ? 4 :
              (model.FlowStatusGUID == InternationalStaffEntitlmentFlowStatus.Closed ? 5 : 0
              ))
              );
            ViewBag.Stage = model.RequestStage;
            model.TotalEntitlements = details.Where(f => f.IsToAdd == true).Select(f => f.TotalAmount).Sum() != null ? details.Where(f => f.IsToAdd == true).Select(f => f.TotalAmount).Sum() - (details.Where(f => f.IsToAdd == false).Select(f => f.TotalAmount).Sum() ?? 0) : 0 - details.Where(f => f.IsToAdd == false).Select(f => f.TotalAmount).Sum() != null ? details.Where(f => f.IsToAdd == false).Select(f => f.TotalAmount).Sum() : 0;
            model.PeriodName = results.dataAHDPeriodEntitlement.MonthName;
            model.PreparedBy = results.PreparedByGUID != null ? pers.Where(x => x.UserGUID == results.PreparedByGUID).FirstOrDefault().FirstName + " " + pers.Where(x => x.UserGUID == results.PreparedByGUID).FirstOrDefault().Surname : " ";
            model.CertifiedBy = results.CertifiedByGUID != null ? pers.Where(x => x.UserGUID == results.CertifiedByGUID).FirstOrDefault().FirstName + " " + pers.Where(x => x.UserGUID == results.CertifiedByGUID).FirstOrDefault().Surname : " ";
            model.FinanceApprovedBy = results.FinanceApprovedByGUID != null ? pers.Where(x => x.UserGUID == results.FinanceApprovedByGUID).FirstOrDefault().FirstName + " " +
                            pers.Where(x => x.UserGUID == results.FinanceApprovedByGUID).FirstOrDefault().Surname : " ";
            model.FinanceApprovedDate = results.FinanceApprovedDate;
            model.CertifiedDate = results.CertifiedDate;
            model.PreparedDate = results.PreparedDate;



            return View("~/Areas/AHD/Views/InternationalStaffEntitlement/EntitlementToReviewByManager.cshtml", model);
        }


        public JsonResult SendEntitlementApprovedByManager(Guid myInternationalStaffEntitlementGUID, string CertifierComment)
        {
            //if (!CMS.HasAction(Permissions.InternationalStaffEntitlementsCertifying.Create, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            string URL = "";
            string Anchor = "";
            string Link = "";
            DateTime ExecutionTime = DateTime.Now;
            var Entitlemnt = DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.InternationalStaffEntitlementGUID == myInternationalStaffEntitlementGUID).FirstOrDefault();
            if (Entitlemnt.FlowStatusGUID == InternationalStaffEntitlmentFlowStatus.PendingCertify)
            {
                var toChange = DbAHD.dataAHDInternationalStaffEntitlementFlow.Where(x => x.InternationalStaffEntitlementGUID == myInternationalStaffEntitlementGUID
                                  && x.IsLastAction == true
                                  ).FirstOrDefault();
                toChange.IsLastAction = false;
                dataAHDInternationalStaffEntitlementFlow newFlowToReview = new dataAHDInternationalStaffEntitlementFlow
                {

                    InternationalStaffEntitlementFlowGUID = Guid.NewGuid(),
                    InternationalStaffEntitlementGUID = myInternationalStaffEntitlementGUID,
                    CreatedByGUID = UserGUID,
                    FlowStatusGUID = InternationalStaffEntitlmentFlowStatus.PendingFinanceApproval,
                    ActionDate = ExecutionTime,
                    IsLastAction = true,
                    OrderId = toChange.OrderId + 1,


                };
                Entitlemnt.FlowStatusGUID = InternationalStaffEntitlmentFlowStatus.PendingFinanceApproval;
                Entitlemnt.LastFlowStatusName = "Sent to Finance";
                Entitlemnt.CertifiedByGUID = UserGUID;
                Entitlemnt.CertifiedDate = ExecutionTime;
                Entitlemnt.CertifierComment = CertifierComment;

                DbAHD.Create(newFlowToReview, Permissions.InternationalStaffEntitlements.CreateGuid, ExecutionTime, DbCMS);
                DbAHD.Update(Entitlemnt, Permissions.InternationalStaffEntitlements.UpdateGuid, ExecutionTime, DbCMS);
                DbAHD.Update(toChange, Permissions.InternationalStaffEntitlements.UpdateGuid, ExecutionTime, DbCMS);
                DbAHD.SaveChanges();

                var monthName = ProcessData.GetMonthName(Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Month);
                var YearName = Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Year;
                //var staffCore = DbAHD.StaffCoreData.Where(x => x.UserGUID == Entitlemnt.StaffGUID).FirstOrDefault();

                //if (staffCore.ReportToGUID != null)
                //{

                var currStaff = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.StaffGUID
                                                                                  && x.LanguageID == LAN).FirstOrDefault();
                Guid financeGUID = Guid.Parse("B08322F1-5FCF-4F63-95B3-0C51F7EC8E85");
                var tempPermGUIDs = DbCMS.userPermissions.Where(x => (x.ActionGUID == financeGUID && x.Active == true
                                 ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();
                var _userGuids = DbCMS.userProfiles.Where(x => tempPermGUIDs.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

                var _backupUsers = DbCMS.userServiceHistory.Where(x => _userGuids.Contains(x.UserGUID)).Select(x => x.EmailAddress).Distinct().ToList();


                string copyEmails = string.Join(" ;", _backupUsers);



                //var _currManagerPersonal = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.FinanceApprovedByGUID
                //                                                               && x.LanguageID == LAN).FirstOrDefault();
                //f
                //var currAccount = DbAHD.userServiceHistory.Where(x => x.UserGUID == Entitlemnt.FinanceApprovedByGUID).FirstOrDefault();
                var _issuer = DbAHD.userServiceHistory.Where(x => x.UserGUID == Entitlemnt.PreparedByGUID).FirstOrDefault();
                var _staff = DbAHD.userServiceHistory.Where(x => x.UserGUID == Entitlemnt.StaffGUID).FirstOrDefault();

                var currPrepare = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.PreparedByGUID
                                                              && x.LanguageID == LAN).FirstOrDefault();
                var curCertifier = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.CertifiedByGUID
                                                         && x.LanguageID == LAN).FirstOrDefault();

                //var _currManagerAccount = DbAHD.userServiceHistory.Where(x => x.UserGUID == staffCore.ReportToGUID).ToList();


                //string SubjectMessage = resxEmails.EntitlemenReviewByStaffSubject.Replace("$Month", monthName).Replace("$Year", Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Year.ToString());
                string SubjectMessage = resxEmails.EntitlemenReviewByStaffSubject.Replace("$SurName", currStaff.Surname).Replace("$Month", monthName).Replace("$Year", Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Year.ToString());
                URL = AppSettingsKeys.Domain + "/AHD/Reports/PrintInternationalEntitlementReport/?PK=" + new Portal().GUIDToString(myInternationalStaffEntitlementGUID);
                Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmDangerPayReceiving + "</a>";
                Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                //string _financeFirstName = _currManagerPersonal.FirstName;
                //string _financeSurName = _currManagerPersonal.Surname;


                string _message = resxEmails.EntitlementReviewByManagerConfirmationFinance
                    //.Replace("$FullName", _financeFirstName + " " + _financeSurName)
                    .Replace("$VerifyLink", Anchor)
                    .Replace("$StaffName", currStaff.FirstName + " " + currStaff.Surname)
                    .Replace("$SentBy", currPrepare.FirstName + " " + currPrepare.Surname)
                    .Replace("$CertifiedBy", curCertifier.FirstName + " " + curCertifier.Surname)

                    .Replace("$Period ", monthName + " " + YearName);

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

        public JsonResult SendRejectToHRByStaff(Guid myInternationalStaffEntitlementGUID, string StaffComment)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlementsCertifying.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            string URL = "";
            string Anchor = "";
            string Link = "";
            DateTime ExecutionTime = DateTime.Now;
            var Entitlemnt = DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.InternationalStaffEntitlementGUID == myInternationalStaffEntitlementGUID).FirstOrDefault();
            if (Entitlemnt.FlowStatusGUID == InternationalStaffEntitlmentFlowStatus.PendingVerification)
            {
                var toChange = DbAHD.dataAHDInternationalStaffEntitlementFlow.Where(x => x.InternationalStaffEntitlementGUID == myInternationalStaffEntitlementGUID
                                  && x.IsLastAction == true
                                  ).FirstOrDefault();
                toChange.IsLastAction = false;
                dataAHDInternationalStaffEntitlementFlow newFlowToReview = new dataAHDInternationalStaffEntitlementFlow
                {

                    InternationalStaffEntitlementFlowGUID = Guid.NewGuid(),
                    InternationalStaffEntitlementGUID = myInternationalStaffEntitlementGUID,
                    CreatedByGUID = UserGUID,
                    FlowStatusGUID = InternationalStaffEntitlmentFlowStatus.Submitted,
                    ActionDate = ExecutionTime,
                    IsLastAction = true,
                    OrderId = toChange.OrderId + 1,


                };
                Entitlemnt.FlowStatusGUID = InternationalStaffEntitlmentFlowStatus.Submitted;
                Entitlemnt.LastFlowStatusName = "Submitted";
                Entitlemnt.StaffComment = StaffComment;


                DbAHD.Create(newFlowToReview, Permissions.InternationalStaffEntitlements.CreateGuid, ExecutionTime, DbCMS);
                DbAHD.Update(Entitlemnt, Permissions.InternationalStaffEntitlements.UpdateGuid, ExecutionTime, DbCMS);
                DbAHD.Update(toChange, Permissions.InternationalStaffEntitlements.UpdateGuid, ExecutionTime, DbCMS);
                DbAHD.SaveChanges();

                //var monthName = ProcessData.GetMonthName(Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Month);
                //var YearName = Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Year;
                ////var staffCore = DbAHD.StaffCoreData.Where(x => x.UserGUID == Entitlemnt.StaffGUID).FirstOrDefault();

                ////if (staffCore.ReportToGUID != null)
                ////{

                //var currStaff = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.StaffGUID
                //                                                                  && x.LanguageID == LAN).FirstOrDefault();
                //var _currManagerPersonal = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.FinanceApprovedByGUID
                //                                                               && x.LanguageID == LAN).FirstOrDefault();
                //var currAccount = DbAHD.userServiceHistory.Where(x => x.UserGUID == Entitlemnt.FinanceApprovedByGUID).FirstOrDefault();
                //var _issuer = DbAHD.userServiceHistory.Where(x => x.UserGUID == Entitlemnt.PreparedByGUID).FirstOrDefault();
                //var _staff = DbAHD.userServiceHistory.Where(x => x.UserGUID == Entitlemnt.StaffGUID).FirstOrDefault();

                //var currPrepare = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.PreparedByGUID
                //                                              && x.LanguageID == LAN).FirstOrDefault();
                //var curCertifier = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.CertifiedByGUID
                //                                         && x.LanguageID == LAN).FirstOrDefault();

                ////var _currManagerAccount = DbAHD.userServiceHistory.Where(x => x.UserGUID == staffCore.ReportToGUID).ToList();
                var _curremailstaff = DbAHD.StaffCoreData.Where(x => x.UserGUID == Entitlemnt.StaffGUID).FirstOrDefault();
                var _currStaffPer = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == Entitlemnt.StaffGUID).FirstOrDefault();
                var monthName = ProcessData.GetMonthName(Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Month);
                var YearName = Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Year;

                //string SubjectMessage = resxEmails.EntitlemenReviewByStaffSubject.Replace("$Month", monthName).Replace("$Year", Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Year.ToString());

                string SubjectMessage = resxEmails.EntitlemenReviewByStaffSubject.Replace("$SurName", _currStaffPer.SurName).Replace("$Month", monthName).Replace("$Year", Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Year.ToString());




                ////to send mail to staff 
                // var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();
                URL = AppSettingsKeys.Domain + "/AHD/InternationalStaffEntitlement/EntitlementToReviewByManager/?id=" + new Portal().GUIDToString(myInternationalStaffEntitlementGUID);
                Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmDangerPayReceiving + "</a>";
                Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                var currStaff = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.StaffGUID
                                                                                  && x.LanguageID == LAN).FirstOrDefault();
                var _issuer = DbAHD.userServiceHistory.Where(x => x.UserGUID == Entitlemnt.PreparedByGUID).FirstOrDefault();
                var _issuerName = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.PreparedByGUID
                                                          && x.LanguageID == LAN).FirstOrDefault();







                string _message = resxEmails.EntitlementRetunToHRToReviewByStaff
                    .Replace("$FullName", _issuerName.FirstName + " " + _issuerName.Surname)

                    .Replace("$StaffName", currStaff.FirstName + " " + currStaff.Surname)


                    .Replace("$staffcomments", Entitlemnt.StaffComment)
                    .Replace("$Period ", monthName + " " + YearName);

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
                string copy_recipients = _curremailstaff.EmailAddress;
                Send(_issuer.EmailAddress, SubjectMessage, _message, isRec, copy_recipients);
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);

            }

            //}
            return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveConfirmReceiptEntitlement(Guid myInternationalStaffEntitlementGUID)
        {

            string URL = "";
            string Anchor = "";
            string Link = "";
            DateTime ExecutionTime = DateTime.Now;
            var Entitlemnt = DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.InternationalStaffEntitlementGUID == myInternationalStaffEntitlementGUID).FirstOrDefault();
            if (Entitlemnt.StaffGUID != UserGUID)
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
            }
            if (Entitlemnt.FlowStatusGUID == InternationalStaffEntitlmentFlowStatus.Closed)
            {


                Entitlemnt.IsConfirmReceipt = true;
                Entitlemnt.ReceiptDate = ExecutionTime;



                DbAHD.Update(Entitlemnt, Permissions.InternationalStaffEntitlements.UpdateGuid, ExecutionTime, DbCMS);

                DbAHD.SaveChanges();


                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);

            }

            //}
            return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendRejectToHRByManager(Guid myInternationalStaffEntitlementGUID, string CertifierComment)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlementsCertifying.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            string URL = "";
            string Anchor = "";
            string Link = "";
            DateTime ExecutionTime = DateTime.Now;
            var Entitlemnt = DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.InternationalStaffEntitlementGUID == myInternationalStaffEntitlementGUID).FirstOrDefault();
            if (Entitlemnt.FlowStatusGUID == InternationalStaffEntitlmentFlowStatus.PendingCertify)
            {
                var toChange = DbAHD.dataAHDInternationalStaffEntitlementFlow.Where(x => x.InternationalStaffEntitlementGUID == myInternationalStaffEntitlementGUID
                                  && x.IsLastAction == true
                                  ).FirstOrDefault();
                toChange.IsLastAction = false;
                dataAHDInternationalStaffEntitlementFlow newFlowToReview = new dataAHDInternationalStaffEntitlementFlow
                {

                    InternationalStaffEntitlementFlowGUID = Guid.NewGuid(),
                    InternationalStaffEntitlementGUID = myInternationalStaffEntitlementGUID,
                    CreatedByGUID = UserGUID,
                    FlowStatusGUID = InternationalStaffEntitlmentFlowStatus.Submitted,
                    ActionDate = ExecutionTime,
                    IsLastAction = true,
                    OrderId = toChange.OrderId + 1,


                };
                Entitlemnt.FlowStatusGUID = InternationalStaffEntitlmentFlowStatus.Submitted;
                Entitlemnt.LastFlowStatusName = "Submitted";
                Entitlemnt.CertifierComment = CertifierComment;


                DbAHD.Create(newFlowToReview, Permissions.InternationalStaffEntitlements.CreateGuid, ExecutionTime, DbCMS);
                DbAHD.Update(Entitlemnt, Permissions.InternationalStaffEntitlements.UpdateGuid, ExecutionTime, DbCMS);
                DbAHD.Update(toChange, Permissions.InternationalStaffEntitlements.UpdateGuid, ExecutionTime, DbCMS);
                DbAHD.SaveChanges();

                //var monthName = ProcessData.GetMonthName(Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Month);
                //var YearName = Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Year;
                ////var staffCore = DbAHD.StaffCoreData.Where(x => x.UserGUID == Entitlemnt.StaffGUID).FirstOrDefault();

                ////if (staffCore.ReportToGUID != null)
                ////{

                //var currStaff = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.StaffGUID
                //                                                                  && x.LanguageID == LAN).FirstOrDefault();
                //var _currManagerPersonal = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.FinanceApprovedByGUID
                //                                                               && x.LanguageID == LAN).FirstOrDefault();
                //var currAccount = DbAHD.userServiceHistory.Where(x => x.UserGUID == Entitlemnt.FinanceApprovedByGUID).FirstOrDefault();
                //var _issuer = DbAHD.userServiceHistory.Where(x => x.UserGUID == Entitlemnt.PreparedByGUID).FirstOrDefault();
                //var _staff = DbAHD.userServiceHistory.Where(x => x.UserGUID == Entitlemnt.StaffGUID).FirstOrDefault();

                //var currPrepare = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.PreparedByGUID
                //                                              && x.LanguageID == LAN).FirstOrDefault();
                //var curCertifier = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.CertifiedByGUID
                //                                         && x.LanguageID == LAN).FirstOrDefault();

                ////var _currManagerAccount = DbAHD.userServiceHistory.Where(x => x.UserGUID == staffCore.ReportToGUID).ToList();

                var monthName = ProcessData.GetMonthName(Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Month);
                var YearName = Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Year;

                //string SubjectMessage = resxEmails.EntitlemenReviewByStaffSubject.Replace("$Month", monthName).Replace("$Year", Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Year.ToString());
                var currStaff = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.StaffGUID
                                                                        && x.LanguageID == LAN).FirstOrDefault();

                string SubjectMessage = resxEmails.EntitlemenReviewByStaffSubject.Replace("$SurName", currStaff.Surname).Replace("$Month", monthName).Replace("$Year", Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Year.ToString());



                ////to send mail to staff 
                // var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();
                URL = AppSettingsKeys.Domain + "/AHD/InternationalStaffEntitlement/EntitlementToReviewByManager/?id=" + new Portal().GUIDToString(myInternationalStaffEntitlementGUID);
                Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmDangerPayReceiving + "</a>";
                Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";

                var _issuer = DbAHD.userServiceHistory.Where(x => x.UserGUID == Entitlemnt.PreparedByGUID).FirstOrDefault();
                var _certiferemail = DbAHD.StaffCoreData.Where(x => x.UserGUID == Entitlemnt.CertifiedByGUID).FirstOrDefault();

                var curCertifier = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.CertifiedByGUID
                                                          && x.LanguageID == LAN).FirstOrDefault();

                var _issuerName = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.PreparedByGUID
                                                          && x.LanguageID == LAN).FirstOrDefault();


                string _message = resxEmails.EntitlementRetunToHRToReviewByCertifier
                    .Replace("$FullName", _issuerName.FirstName + " " + _issuerName.Surname)

                    .Replace("$StaffName", currStaff.FirstName + " " + currStaff.Surname)

                    .Replace("$certifer", curCertifier.FirstName + " " + curCertifier.Surname)
                    .Replace("$certiferComments", Entitlemnt.CertifierComment)
                    .Replace("$Period ", monthName + " " + YearName);

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
                string copy_recipients = _certiferemail.EmailAddress;
                Send(_issuer.EmailAddress, SubjectMessage, _message, isRec, copy_recipients);
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);

            }

            //}
            return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendRejectToHRByFinance(Guid myInternationalStaffEntitlementGUID, string FinanceComment)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlementsCertifying.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            string URL = "";
            string Anchor = "";
            string Link = "";
            DateTime ExecutionTime = DateTime.Now;
            var Entitlemnt = DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.InternationalStaffEntitlementGUID == myInternationalStaffEntitlementGUID).FirstOrDefault();
            if (Entitlemnt.FlowStatusGUID == InternationalStaffEntitlmentFlowStatus.PendingFinanceApproval)
            {
                var toChange = DbAHD.dataAHDInternationalStaffEntitlementFlow.Where(x => x.InternationalStaffEntitlementGUID == myInternationalStaffEntitlementGUID
                                  && x.IsLastAction == true
                                  ).FirstOrDefault();
                toChange.IsLastAction = false;
                dataAHDInternationalStaffEntitlementFlow newFlowToReview = new dataAHDInternationalStaffEntitlementFlow
                {

                    InternationalStaffEntitlementFlowGUID = Guid.NewGuid(),
                    InternationalStaffEntitlementGUID = myInternationalStaffEntitlementGUID,
                    CreatedByGUID = UserGUID,
                    FlowStatusGUID = InternationalStaffEntitlmentFlowStatus.Submitted,
                    ActionDate = ExecutionTime,
                    IsLastAction = true,
                    OrderId = toChange.OrderId + 1,


                };
                Entitlemnt.FlowStatusGUID = InternationalStaffEntitlmentFlowStatus.Submitted;
                Entitlemnt.LastFlowStatusName = "Submitted";
                Entitlemnt.CertifierComment = FinanceComment;


                DbAHD.Create(newFlowToReview, Permissions.InternationalStaffEntitlements.CreateGuid, ExecutionTime, DbCMS);
                DbAHD.Update(Entitlemnt, Permissions.InternationalStaffEntitlements.UpdateGuid, ExecutionTime, DbCMS);
                DbAHD.Update(toChange, Permissions.InternationalStaffEntitlements.UpdateGuid, ExecutionTime, DbCMS);
                DbAHD.SaveChanges();

                //var monthName = ProcessData.GetMonthName(Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Month);
                //var YearName = Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Year;
                ////var staffCore = DbAHD.StaffCoreData.Where(x => x.UserGUID == Entitlemnt.StaffGUID).FirstOrDefault();

                ////if (staffCore.ReportToGUID != null)
                ////{

                //var currStaff = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.StaffGUID
                //                                                                  && x.LanguageID == LAN).FirstOrDefault();
                //var _currManagerPersonal = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.FinanceApprovedByGUID
                //                                                               && x.LanguageID == LAN).FirstOrDefault();
                //var currAccount = DbAHD.userServiceHistory.Where(x => x.UserGUID == Entitlemnt.FinanceApprovedByGUID).FirstOrDefault();
                //var _issuer = DbAHD.userServiceHistory.Where(x => x.UserGUID == Entitlemnt.PreparedByGUID).FirstOrDefault();
                //var _staff = DbAHD.userServiceHistory.Where(x => x.UserGUID == Entitlemnt.StaffGUID).FirstOrDefault();

                //var currPrepare = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.PreparedByGUID
                //                                              && x.LanguageID == LAN).FirstOrDefault();
                //var curCertifier = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.CertifiedByGUID
                //                                         && x.LanguageID == LAN).FirstOrDefault();

                ////var _currManagerAccount = DbAHD.userServiceHistory.Where(x => x.UserGUID == staffCore.ReportToGUID).ToList();

                var monthName = ProcessData.GetMonthName(Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Month);
                var YearName = Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Year;
                var currStaff = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.StaffGUID
                                                                                && x.LanguageID == LAN).FirstOrDefault();
                //string SubjectMessage = resxEmails.EntitlemenReviewByStaffSubject.Replace("$Month", monthName).Replace("$Year", Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Year.ToString());

                string SubjectMessage = resxEmails.EntitlemenReviewByStaffSubject.Replace("$SurName", currStaff.Surname).Replace("$Month", monthName).Replace("$Year", Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Year.ToString());




                ////to send mail to staff 
                // var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();
                URL = AppSettingsKeys.Domain + "/AHD/InternationalStaffEntitlement/EntitlementToReviewByManager/?id=" + new Portal().GUIDToString(myInternationalStaffEntitlementGUID);
                Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmDangerPayReceiving + "</a>";
                Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";

                var _issuer = DbAHD.userServiceHistory.Where(x => x.UserGUID == Entitlemnt.PreparedByGUID).FirstOrDefault();
                var _certiferemail = DbAHD.StaffCoreData.Where(x => x.UserGUID == Entitlemnt.CertifiedByGUID).FirstOrDefault();

                var curCertifier = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.CertifiedByGUID
                                                          && x.LanguageID == LAN).FirstOrDefault();

                var _financeemail = DbAHD.StaffCoreData.Where(x => x.UserGUID == Entitlemnt.FinanceApprovedByGUID).FirstOrDefault();

                var _financename = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.FinanceApprovedByGUID
                                                          && x.LanguageID == LAN).FirstOrDefault();

                var _issuerName = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.PreparedByGUID
                                                          && x.LanguageID == LAN).FirstOrDefault();


                string _message = resxEmails.EntitlementRetunToHRToReviewByCertifier
                    .Replace("$FullName", _issuerName.FirstName + " " + _issuerName.Surname)

                    .Replace("$StaffName", currStaff.FirstName + " " + currStaff.Surname)

                    .Replace("$financename", _financename.FirstName + " " + _financename.Surname)
                    .Replace("$financeComments", Entitlemnt.FinanceComment)
                    .Replace("$Period ", monthName + " " + YearName);

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
                string copy_recipients = _certiferemail.EmailAddress + " " + _financeemail.EmailAddress;
                Send(_issuer.EmailAddress, SubjectMessage, _message, isRec, copy_recipients);
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);

            }

            //}
            return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveFinanceComment(Guid myInternationalStaffEntitlementGUID, string FinanceComment)
        {
            //if (!CMS.HasAction(Permissions.InternationalStaffEntitlementsCertifying.Create, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}

            DateTime ExecutionTime = DateTime.Now;
            var Entitlemnt = DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.InternationalStaffEntitlementGUID == myInternationalStaffEntitlementGUID).FirstOrDefault();

            Entitlemnt.FinanceComment = FinanceComment;
            //Entitlemnt.UpdateByGUID = UserGUID.ToString();



            DbAHD.Update(Entitlemnt, Permissions.InternationalStaffEntitlements.UpdateGuid, ExecutionTime, DbCMS);

            DbAHD.SaveChanges();



            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);



            //}

        }
        public JsonResult SendEntitlementApprovedByFinance(Guid myInternationalStaffEntitlementGUID, string FinanceComment)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlementsCertifying.Confirm, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            string URL = "";
            string Anchor = "";
            string Link = "";
            DateTime ExecutionTime = DateTime.Now;
            var Entitlemnt = DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.InternationalStaffEntitlementGUID == myInternationalStaffEntitlementGUID).FirstOrDefault();
            if (Entitlemnt.FlowStatusGUID == InternationalStaffEntitlmentFlowStatus.PendingFinanceApproval)
            {
                var toChange = DbAHD.dataAHDInternationalStaffEntitlementFlow.Where(x => x.InternationalStaffEntitlementGUID == myInternationalStaffEntitlementGUID
                                  && x.IsLastAction == true
                                  ).FirstOrDefault();
                toChange.IsLastAction = false;
                dataAHDInternationalStaffEntitlementFlow newFlowToReview = new dataAHDInternationalStaffEntitlementFlow
                {

                    InternationalStaffEntitlementFlowGUID = Guid.NewGuid(),
                    InternationalStaffEntitlementGUID = myInternationalStaffEntitlementGUID,
                    CreatedByGUID = UserGUID,
                    FlowStatusGUID = InternationalStaffEntitlmentFlowStatus.Closed,
                    ActionDate = ExecutionTime,
                    IsLastAction = true,
                    OrderId = toChange.OrderId + 1,


                };
                Entitlemnt.FlowStatusGUID = InternationalStaffEntitlmentFlowStatus.Closed;
                Entitlemnt.LastFlowStatusName = "Closed";
                Entitlemnt.FinanceApprovedByGUID = UserGUID;
                Entitlemnt.FinanceApprovedDate = ExecutionTime;
                Entitlemnt.FinanceComment = FinanceComment;
                DbAHD.Create(newFlowToReview, Permissions.InternationalStaffEntitlements.CreateGuid, ExecutionTime, DbCMS);
                DbAHD.Update(Entitlemnt, Permissions.InternationalStaffEntitlements.UpdateGuid, ExecutionTime, DbCMS);
                DbAHD.Update(toChange, Permissions.InternationalStaffEntitlements.UpdateGuid, ExecutionTime, DbCMS);
                DbAHD.SaveChanges();
                #region Email Send to activate if any
                //var monthName = ProcessData.GetMonthName(Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Month);
                //var YearName = Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Year;
                ////var staffCore = DbAHD.StaffCoreData.Where(x => x.UserGUID == Entitlemnt.StaffGUID).FirstOrDefault();

                ////if (staffCore.ReportToGUID != null)
                ////{

                //var currPrepare = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.PreparedByGUID
                //                                                              && x.LanguageID == LAN).FirstOrDefault();
                //var _currAuth = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.CertifiedByGUID
                //                                                                && x.LanguageID == LAN).FirstOrDefault();
                //var _currFinance = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.FinanceApprovedByGUID
                //                                                            && x.LanguageID == LAN).FirstOrDefault();

                //var currAccount = DbAHD.userServiceHistory.Where(x => x.UserGUID == Entitlemnt.StaffGUID).FirstOrDefault();
                //var _emailPrepar = DbAHD.userServiceHistory.Where(x => x.UserGUID == Entitlemnt.PreparedByGUID).FirstOrDefault();
                //var _emailAuth = DbAHD.userServiceHistory.Where(x => x.UserGUID == Entitlemnt.CertifiedByGUID).FirstOrDefault();

                //var _emailFinance = DbAHD.userServiceHistory.Where(x => x.UserGUID == Entitlemnt.FinanceApprovedByGUID).FirstOrDefault();

                //var _currManagerAccount = DbAHD.userServiceHistory.Where(x => x.UserGUID == Entitlemnt.StaffGUID).FirstOrDefault();
                //var _currstaffPers = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == Entitlemnt.StaffGUID
                //                                                                && x.LanguageID == LAN).FirstOrDefault();


                //string SubjectMessage = resxEmails.EntitlemenReviewByStaffSubject.Replace("$Month", monthName).Replace("$Year", Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Year.ToString());


                ////to send mail to staff 
                //// var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();




                //string _message = resxEmails.EntitlementReviewByClosedConfirmation
                //    .Replace("$FullName", _currstaffPers.FirstName + " " + _currstaffPers.Surname)

                //    //.Replace("$SentBy", currPrepare.FirstName + " " + currPrepare.Surname)
                //    //.Replace("$certifiedBy", _currAuth.FirstName + " " + _currAuth.Surname)
                //    //.Replace("$FinanceApproveddBy", _currFinance.FirstName + " " + _currFinance.Surname)

                //    .Replace("$Period ", monthName + " " + YearName);

                //if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                //int isRec = 1;
                //var myEmail = currAccount.EmailAddress;
                //string copy_recipients = _emailPrepar.EmailAddress + " " + _emailAuth.EmailAddress+" "+ _emailFinance.EmailAddress;
                //Send(myEmail, SubjectMessage, _message, isRec, copy_recipients);

                #endregion


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
            //DbAHD.SendEmailHR("maksoud@unhcr.org", "", blind_copy_recipients, subject, _body, body_format, importance, file_attachments);
             DbCMS.SendEmailHR(recipients, copy_recipients, blind_copy_recipients, subject, _body, body_format, importance, file_attachments);
        }
        #endregion

        public ActionResult TestDelete()
        {

            DbAHD.sp_deleteEntitlementTables();
            var period = DbAHD.dataAHDPeriodEntitlement.FirstOrDefault();
            return Json(DbAHD.SingleUpdateMessage(DataTableNames.InternationalStaffEntitlementPeriodDataTable, DbAHD.PrimaryKeyControl(period), DbAHD.RowVersionControls(Portal.SingleToList(period))));
            //  return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
        }

        #region Export Staff Report

        public ActionResult GenerateStaffEntitlementsReport(Guid id)
        {
            var result = DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.InternationalStaffEntitlementGUID == id).FirstOrDefault();
            var staffCore = DbAHD.StaffCoreData.Where(x => x.UserGUID == result.StaffGUID).FirstOrDefault();
            var codeJobtitles = DbCMS.codeJobTitlesLanguages.Where(x => x.LanguageID == LAN && x.Active).ToList();
            var currDutyStation = DbCMS.codeDutyStationsLanguages.Where(x => x.DutyStationGUID == staffCore.DutyStationGUID && x.LanguageID == LAN).FirstOrDefault();
            if (result != null)
            {

                string sourceFile = Server.MapPath("~/Areas/AHD/Templates/StaffEntitlements/MonthlyEntitlements/" + result.PeriodEntitlementGUID + ".xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/AHD/temp/staffEntilement" + result.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).FirstOrDefault().FirstName + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                    int rowIndex = 16;
                    int rowIndexNumber = 17;
                    int attendanceIndexNumber = 18;
                    int rentalIndexNumber = 19;
                    int dsaHotelIndexNumber = 20;
                    int dsaAleppoIndexNumber = 21;
                    int r_rIndexNumber = 22;
                    int solarIndexNumber = 23;
                    int DPIndexNumber = 24;
                    int MissionIndexNumber = 25;
                    int col = 2;
                    int colNumber = 2;
                    var dates = new List<DateTime>();

                    // Loop from the first day of the month until we hit the next month, moving forward a day at a time
                    for (var date = new DateTime(result.dataAHDPeriodEntitlement.StartMonth.Value.Year, result.dataAHDPeriodEntitlement.StartMonth.Value.Month, 1); date.Month == result.dataAHDPeriodEntitlement.StartMonth.Value.Month; date = date.AddDays(1))
                    {
                        dates.Add(date);
                    }


                    var allattendaces = DbAHD.dataInternationalStaffAttendance.Where(x => x.StaffGUID == result.StaffGUID).ToList();
                    var mycurrAttendances = allattendaces.Where(x => x.FromDate >= result.dataAHDPeriodEntitlement.StartMonth
                                                      && x.ToDate <= result.dataAHDPeriodEntitlement.EndMonth).ToList();
                    //var beforeLeaves= allattendaces.Where()



                    //Query each type of attendance in one query 
                    var currR_R = mycurrAttendances.Where(x => x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR).ToList();
                    var currannualLeave = mycurrAttendances.Where(x => x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave).ToList();
                    var currfamilyLeave = mycurrAttendances.Where(x => x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave).ToList();
                    var currsickLeave = mycurrAttendances.Where(x => x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave).ToList();
                    var currSLWFPLeave = mycurrAttendances.Where(x => x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.SLWFP).ToList();
                    var currUnSickLeave = mycurrAttendances.Where(x => x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave).ToList();


                    var beforeAttendaceLeaves = allattendaces.Where(x => x.FromDate < result.dataAHDPeriodEntitlement.StartMonth
                                 && x.ToDate >= result.dataAHDPeriodEntitlement.StartMonth).ToList();

                    var beforeR_R = beforeAttendaceLeaves.Where(x => x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR).ToList();
                    var beforeannualLeave = beforeAttendaceLeaves.Where(x => x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave).ToList();
                    var beforeFamilyLeave = beforeAttendaceLeaves.Where(x => x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave).ToList();
                    var beforesickLeave = beforeAttendaceLeaves.Where(x => x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave).ToList();
                    var beforeUnSickLeave = beforeAttendaceLeaves.Where(x => x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave).ToList();


                    var afterAttendaceLeaves = allattendaces.Where(x => x.FromDate >= result.dataAHDPeriodEntitlement.StartMonth &&
                                                    x.ToDate > result.dataAHDPeriodEntitlement.EndMonth
                                                    && (x.FromDate.Value.Month == result.dataAHDPeriodEntitlement.StartMonth.Value.Month
                                                    && x.FromDate.Value.Year == result.dataAHDPeriodEntitlement.StartMonth.Value.Year)).ToList();


                    var afterR_R = afterAttendaceLeaves.Where(x => x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR).ToList();
                    var afterannualLeave = afterAttendaceLeaves.Where(x => x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave).ToList();
                    var afterfamilyLeave = afterAttendaceLeaves.Where(x => x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave).ToList();
                    var aftersickLeave = afterAttendaceLeaves.Where(x => x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave).ToList();
                    var afterUnSickLeave = afterAttendaceLeaves.Where(x => x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave).ToList();

                    #region r_r process
                    List<DateTime> noR_RDates = new List<DateTime>();
                    if (currR_R.Select(x => x.TotalDays).FirstOrDefault() > 7)
                    {
                        var last7date = currR_R.FirstOrDefault().FromDate.Value.AddDays(7);
                        var remingDays = (currR_R.FirstOrDefault().ToDate - last7date).Value.TotalDays;
                        for (int i = 0; i < remingDays; i++)
                        {
                            noR_RDates.Add(last7date);
                            last7date.AddDays(1);
                        }


                    }
                    var totalAfterR_RLeaveAllPeriod = afterAttendaceLeaves.Where(d => (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR)

                                                               ).Select(x => x.TotalDays).Sum();
                    var afterR_RTillendMonth = afterAttendaceLeaves.Where(d => (d.InternationalStaffAttendanceTypeGUID ==
                                                                                                                        coddeInternationalStaffAttendanceTypeAttendanceTable.RR)).
                                                                                                                          Select(x => (result.dataAHDPeriodEntitlement.EndMonth - x.FromDate).Value.TotalDays).FirstOrDefault();
                    if (totalAfterR_RLeaveAllPeriod > 7 //&& afterR_RTillendMonth-7 > 0
                                )
                    {
                        //add 7 days for first day of leave 23+7 =30 ..31-30=1 
                        var lastAfter7date = afterR_R.FirstOrDefault().FromDate.Value.AddDays(7);
                        var remingAfterDays = (result.dataAHDPeriodEntitlement.EndMonth - lastAfter7date).Value.TotalDays;
                        for (int i = 0; i < remingAfterDays; i++)
                        {
                            noR_RDates.Add(lastAfter7date);
                            lastAfter7date.AddDays(1);
                        }

                    }

                    var totalBeforeR_RLeaveAllPeriod = beforeAttendaceLeaves.Where(d => (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR)

                                                              ).Select(x => x.TotalDays).Sum();
                    var beforeR_RTillStartMonth = beforeAttendaceLeaves.Where(d => (d.InternationalStaffAttendanceTypeGUID ==
                                                                                                                        coddeInternationalStaffAttendanceTypeAttendanceTable.RR)).

                                                                                                                          Select(x => (x.ToDate - result.dataAHDPeriodEntitlement.StartMonth).Value.TotalDays).FirstOrDefault();
                    if (totalBeforeR_RLeaveAllPeriod > 7)
                    {

                        //from date 25+7=2 apr .. 2apr -1 apr ==1
                        var lastbefoere7date = beforeR_R.FirstOrDefault().FromDate.Value.AddDays(7);
                        var remingBeforerDays = (lastbefoere7date - result.dataAHDPeriodEntitlement.StartMonth).Value.TotalDays;
                        for (int i = 0; i < remingBeforerDays; i++)
                        {
                            noR_RDates.Add(lastbefoere7date);
                            lastbefoere7date.AddDays(1);
                        }

                    }

                    #endregion

                    foreach (var item in dates)
                    {
                        workSheet.Cells[rowIndex, col].Value = item.ToString("ddd");
                        //R_R records

                        if (currR_R.Where(x => x.FromDate <= item && x.ToDate >= item).FirstOrDefault() != null ||
                              beforeR_R.Where(x => x.FromDate <= item && x.ToDate >= item).FirstOrDefault() != null ||
                              afterR_R.Where(x => x.FromDate <= item && x.ToDate >= item).FirstOrDefault() != null
                               )
                        {
                            workSheet.Cells[r_rIndexNumber, col].Value = 1;
                            workSheet.Cells[attendanceIndexNumber, col].Value = "R";
                            if (noR_RDates.Where(x => x == item).FirstOrDefault() == null)
                            {
                                workSheet.Cells[DPIndexNumber, col].Value = 1;
                            }
                        }


                        if (
                            (currR_R.Where(x => x.FromDate <= item && x.ToDate >= item).FirstOrDefault() == null &&
                            currannualLeave.Where(x => x.FromDate <= item && x.ToDate >= item).FirstOrDefault() == null &&
                            currfamilyLeave.Where(x => x.FromDate <= item && x.ToDate >= item).FirstOrDefault() == null &&
                            currsickLeave.Where(x => x.FromDate <= item && x.ToDate >= item).FirstOrDefault() == null &&
                            currSLWFPLeave.Where(x => x.FromDate <= item && x.ToDate >= item).FirstOrDefault() == null &&

                            currUnSickLeave.Where(x => x.FromDate <= item && x.ToDate >= item).FirstOrDefault() == null)
                            &&
                            (beforeR_R.Where(x => x.FromDate <= item && x.ToDate >= item).FirstOrDefault() == null &&
                            beforeannualLeave.Where(x => x.FromDate <= item && x.ToDate >= item).FirstOrDefault() == null &&
                            beforeFamilyLeave.Where(x => x.FromDate <= item && x.ToDate >= item).FirstOrDefault() == null &&
                            beforesickLeave.Where(x => x.FromDate <= item && x.ToDate >= item).FirstOrDefault() == null &&
                            beforeUnSickLeave.Where(x => x.FromDate <= item && x.ToDate >= item).FirstOrDefault() == null)
                            &&
                          (afterR_R.Where(x => x.FromDate <= item && x.ToDate >= item).FirstOrDefault() == null &&
                            afterannualLeave.Where(x => x.FromDate <= item && x.ToDate >= item).FirstOrDefault() == null &&
                            afterfamilyLeave.Where(x => x.FromDate <= item && x.ToDate >= item).FirstOrDefault() == null &&
                            aftersickLeave.Where(x => x.FromDate <= item && x.ToDate >= item).FirstOrDefault() == null &&
                            afterUnSickLeave.Where(x => x.FromDate <= item && x.ToDate >= item).FirstOrDefault() == null)

                            )
                        {
                            workSheet.Cells[attendanceIndexNumber, col].Value = 1;
                            workSheet.Cells[rentalIndexNumber, col].Value = 1;
                            workSheet.Cells[DPIndexNumber, col].Value = 1;
                        }
                        col = col + 1;

                        workSheet.Cells[rowIndexNumber, colNumber].Value = item.Day;
                        colNumber++;
                        //workSheet.Cells["G3"].Value = weekPeriod.WeekStatrdate;
                    }


                    workSheet.Cells["B3"].Value = result.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).FirstOrDefault().FirstName + " " + result.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).FirstOrDefault().Surname;
                    workSheet.Cells["B4"].Value = codeJobtitles.Where(x => x.JobTitleGUID == staffCore.JobTitleGUID) != null ? codeJobtitles.Where(x => x.JobTitleGUID == staffCore.JobTitleGUID).FirstOrDefault().JobTitleDescription : "";
                    //workSheet.Cells["B5"].Value = staffCore.IDNumber;
                    workSheet.Cells["B8"].Value = result.dataAHDPeriodEntitlement.MonthName;
                    workSheet.Cells["AG19"].Value = result.dataAHDInternationalStaffEntitlementDetail.Where(x => x.EntitlementTypeGUID == InternationalStaffEntitlementType.RentalDeduction).FirstOrDefault().TotalDays;
                    workSheet.Cells["AG22"].Value = result.dataAHDInternationalStaffEntitlementDetail.Where(x => x.EntitlementTypeGUID == InternationalStaffEntitlementType.R_RTicket).FirstOrDefault().TotalDays;
                    workSheet.Cells["AH19"].Value = result.dataAHDInternationalStaffEntitlementDetail.Where(x => x.EntitlementTypeGUID == InternationalStaffEntitlementType.RentalDeduction).FirstOrDefault().TotalAmount * -1;
                    workSheet.Cells["AH22"].Value = result.dataAHDInternationalStaffEntitlementDetail.Where(x => x.EntitlementTypeGUID == InternationalStaffEntitlementType.R_RTicket).FirstOrDefault().TotalAmount;
                    workSheet.Cells["AH24"].Value = result.dataAHDInternationalStaffEntitlementDetail.Where(x => x.EntitlementTypeGUID == InternationalStaffEntitlementType.DangerPayPerDay).FirstOrDefault().TotalAmount;
                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = "Absence table for staff" + result.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).FirstOrDefault().FirstName + " " + result.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).FirstOrDefault().Surname + " " + DateTime.Now + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }

            string success = "No Data Available for this period";
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Update Regeneratte for one staff
        public ActionResult ConfirmationView(string result)
        {
            return View("~/Areas/AHD/Views/Confirmation/Confirm.cshtml");

        }
        //[HttpPost, ValidateAntiForgeryToken]
        public ActionResult InternationalStaffEntitlementPerStaffDeleteAndUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }


            DateTime ExecutionTime = DateTime.Now;
            var mycurrEntitlement = DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.InternationalStaffEntitlementGUID == PK).FirstOrDefault();
            var staffGuid = mycurrEntitlement.StaffGUID;
            var periodGUID = mycurrEntitlement.PeriodEntitlementGUID;
            var period = DbAHD.dataAHDPeriodEntitlement.Where(x => x.PeriodEntitlementGUID == periodGUID).FirstOrDefault();
            //var model = DbAHD.dataAHDPeriodEntitlement.Where(x => x.PeriodEntitlementGUID == mymodel.PeriodEntitlementGUID).FirstOrDefault();


            var _mydaily = DbAHD.dataStaffLeaveAttendanceDaily.Where(x => x.StaffGUID == staffGuid).ToList();
            DbAHD.dataStaffLeaveAttendanceDaily.RemoveRange(_mydaily);
            var _myflows = DbAHD.dataAHDInternationalStaffEntitlementFlow.Where(x => x.dataAHDInternationalStaffEntitlement.PeriodEntitlementGUID == periodGUID && x.dataAHDInternationalStaffEntitlement.StaffGUID == staffGuid).ToList();
            var _myDetail = DbAHD.dataAHDInternationalStaffEntitlementDetail.Where(x => x.dataAHDInternationalStaffEntitlement.PeriodEntitlementGUID == periodGUID && x.dataAHDInternationalStaffEntitlement.StaffGUID == staffGuid).ToList();
            var _myRoom = DbAHD.dataAHDInternationalStaffEntitlementRoomTaken.Where(x => x.dataAHDInternationalStaffEntitlementDetail.dataAHDInternationalStaffEntitlement.PeriodEntitlementGUID == periodGUID && x.dataAHDInternationalStaffEntitlementDetail.dataAHDInternationalStaffEntitlement.StaffGUID == staffGuid).ToList();
            var _myentit = DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.PeriodEntitlementGUID == periodGUID && x.StaffGUID == staffGuid).FirstOrDefault();
            DbAHD.dataStaffLeaveAttendanceDaily.RemoveRange(_mydaily);
            DbAHD.dataAHDInternationalStaffEntitlementFlow.RemoveRange(_myflows);
            DbAHD.dataAHDInternationalStaffEntitlementDetail.RemoveRange(_myDetail);
            DbAHD.dataAHDInternationalStaffEntitlementRoomTaken.RemoveRange(_myRoom);
            DbAHD.dataAHDInternationalStaffEntitlement.Remove(_myentit);
            DbAHD.SaveChanges();

            var _allattendaces = DbAHD.dataInternationalStaffAttendance.Where(x => x.StaffGUID == staffGuid).ToList();

            var allstaffs = DbAHD.StaffCoreData.Where(x => x.UserGUID == staffGuid
                           ).ToList();
            List<dataStaffLeaveAttendanceDaily> alldailytoadd = new List<dataStaffLeaveAttendanceDaily>();

            foreach (var item in allstaffs)
            {
                var temp = _allattendaces.Where(x => x.StaffGUID == item.UserGUID).ToList();

                foreach (var myatt in temp.OrderBy(x => x.FromDate))
                {
                    for (var day = myatt.FromDate; day <= myatt.ToDate; day = day.Value.AddDays(1))
                    {
                        dataStaffLeaveAttendanceDaily toAddaily = new dataStaffLeaveAttendanceDaily
                        {
                            StaffLeaveAttendanceDailyGUID = Guid.NewGuid(),
                            DayDate = day,
                            StaffGUID = item.UserGUID,
                            InternationalStaffAttendanceTypeGUID = myatt.InternationalStaffAttendanceTypeGUID,

                        };
                        alldailytoadd.Add(toAddaily);

                    }
                }

            }
            DbAHD.CreateBulkNoAudit(alldailytoadd);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }

            Guid EntityPK = Guid.NewGuid();


            var personalDetaillan = DbAHD.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.UserGUID == staffGuid).ToList();
            List<dataAHDInternationalStaffEntitlement> staffEntitlements = new List<dataAHDInternationalStaffEntitlement>();
            List<StaffTeleComutingLeave> allStaffTele = new List<StaffTeleComutingLeave>();
            var countrieNotAllowedGUIDs = DbAHD.codeAHDLocationNoRAndRTicket.Select(x => x.LocationGUID).ToList();

            var allattendacLeaves = DbAHD.dataInternationalStaffAttendance.Where(x => x.StaffGUID == staffGuid).ToList();


            #region tele process
            var teleuserGuids = allattendacLeaves.Where(x => x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting).Select(x => x.StaffGUID).Distinct();
            var beforeAttendaceLeaves = allattendacLeaves.Where(x => x.FromDate < period.StartMonth && x.ToDate >= period.StartMonth).ToList();
            var afterAttendaceLeaves = allattendacLeaves.Where(x => x.FromDate >= period.StartMonth && x.ToDate > period.EndMonth
                                                      && (x.FromDate.Value.Month == period.StartMonth.Value.Month && x.FromDate.Value.Year == period.StartMonth.Value.Year)).ToList();
            var currentMonthAttendaceLeaves = allattendacLeaves.Where(x => x.FromDate >= period.StartMonth
                                         && x.ToDate <= period.EndMonth).ToList();

            //new tele 
            var allLeavesDaily = DbAHD.dataStaffLeaveAttendanceDaily.Where(x => x.StaffGUID == staffGuid).ToList();
            int totlaDays = 0;
            //foreach (var item in teleuserGuids.Distinct())
            //{
            //    totlaDays = 0;

            //    var staffLeavethismonth = allLeavesDaily.Where(x => x.StaffGUID == item &&
            //       x.DayDate >= period.StartMonth && x.DayDate <= period.EndMonth).ToList();
            //    var firsttele = staffLeavethismonth.Where(x =>
            //    x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting).OrderBy(x => x.DayDate).FirstOrDefault();
            //    var beforetele = staffLeavethismonth.Where(x => x.DayDate < firsttele.DayDate).ToList();
            //    var aftertele = staffLeavethismonth.Where(x => x.DayDate > firsttele.DayDate).ToList();
            //    DateTime? currenttelDate = firsttele != null ? firsttele.DayDate.Value.AddDays(-1) : (DateTime?)null;
            //    var checkRRBeforeFor = beforetele.Where(x => x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR).OrderByDescending(x => x.DayDate).ToList();
            //    var checkRRAfterFor = aftertele.Where(x => x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR).OrderBy(x => x.DayDate).ToList();

            //    foreach (var mycurr in beforetele.OrderByDescending(x => x.DayDate))
            //    {
            //        if (currenttelDate == mycurr.DayDate)
            //        {


            //            totlaDays++;
            //            currenttelDate = currenttelDate.Value.AddDays(-1);


            //        }

            //    }
            //    currenttelDate = firsttele != null ? firsttele.DayDate.Value.AddDays(1) : (DateTime?)null;
            //    foreach (var mycurr in aftertele.OrderBy(x => x.DayDate))
            //    {
            //        if (currenttelDate == mycurr.DayDate)
            //        {


            //            totlaDays++;
            //            currenttelDate = currenttelDate.Value.AddDays(1);


            //        }

            //    }
            //    if (totlaDays > 0)
            //    {

            //        StaffTeleComutingLeave myStaffTeleDay = new StaffTeleComutingLeave
            //        {
            //            totalDays = totlaDays + 1,
            //            StaffGUID = (Guid)item
            //        };
            //        if (checkRRBeforeFor.Count > 0)
            //        {
            //            myStaffTeleDay.totalDays = myStaffTeleDay.totalDays - checkRRBeforeFor.Count;


            //        }
            //        if (checkRRAfterFor.Count > 0)
            //        {
            //            myStaffTeleDay.totalDays = myStaffTeleDay.totalDays - checkRRAfterFor.Count;
            //        }
            //        allStaffTele.Add(myStaffTeleDay);
            //    }



            //}






            #endregion


            var entilementtypes = DbAHD.codeAHDEntitlementType.Where(x => x.EntitlementTypeGUID != InternationalStaffEntitlementType.AddedRecorvery
            && x.EntitlementTypeGUID != InternationalStaffEntitlementType.DeductedRecovery
                && x.EntitlementTypeGUID != InternationalStaffEntitlementType.BreakfastDeduction).ToList();
            var entitlementTypePerDutyStation = DbAHD.codeAHDEntitlementTypePerDutyStation.ToList();
            personalDetaillan = personalDetaillan.Where(x => allstaffs.Select(f => f.UserGUID).Contains(x.UserGUID)).ToList();
            //Guid IsacmyUser = Guid.Parse("F83879A9-2113-4606-8B01-2B28FC82D536");
            Guid AleepocmyUser = Guid.Parse("F1B5F1A4-280F-405D-8305-11C5CBF28179");

            //var s = currentMonthAttendaceLeaves.Where(d => d.StaffGUID == myUser && (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave
            //                                                                     || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave ||
            //                                                                     d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave)).Select(x => x.TotalDays).Sum();


            Guid aleppoStationGUID = Guid.Parse("6D7397D6-3D7F-48FC-BFD2-18E69673AC94");
            Guid tartusStationGUID = Guid.Parse("6CD6D68D-EAC1-440B-904F-7D34B4FD3863");


            staffEntitlements = (from x in allstaffs
                                     //.Where(x=>x.UserGUID== testuser)

                                 let staffName = personalDetaillan.Where(f => f.UserGUID == x.UserGUID)
                                 let myInternationalStaffEntitlementGUID = Guid.NewGuid()

                                 // let myalltelesDaysWithleavs = allStaffTele.Where(f => f.StaffGUID == x.UserGUID).ToList()
                                 let mycurrenmonthtattendacLeaves = currentMonthAttendaceLeaves.Where(f => f.StaffGUID == x.UserGUID)
                                 let mybeforeAttendaceLeaves = beforeAttendaceLeaves.Where(f => f.StaffGUID == x.UserGUID)
                                 let myafterAttendaceLeaves = afterAttendaceLeaves.Where(f => f.StaffGUID == x.UserGUID)
                                 let myallattendacLeaves = allattendacLeaves.Where(f => f.StaffGUID == x.UserGUID)
                                 let teleworking = allattendacLeaves.Where(f => f.StaffGUID == x.UserGUID &&
                                                                    f.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting)
                                 let myAllLeavesCountForLeaveMoreThanMonth = myallattendacLeaves.Where(x => x.FromDate < period.StartMonth && x.ToDate > period.EndMonth

                                                                                              && (
                                                                                               x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave
                                                                                               || x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave
                                                                                               || x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.HomeLeave
                                                                                                            || x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.PaternityLeave
                                                                                                      || x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave
                                                                                                      || x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave
                                                                                                      || x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.SLWFP
                                                                                                            || x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR

                                                                                                            || x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting
                                                                                                            || x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend
                                                                                                            || x.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday
                                                                                              )).Count()
                                 #region Without R_R
                                 let currLeaveWithoutR_R = mycurrenmonthtattendacLeaves.Where(d => d.StaffGUID == x.UserGUID && (
                                 d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave
                                 || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave
                                 || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.HomeLeave

                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.PaternityLeave
                                                                                 || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave
                                                                                 || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave
                                                                                 || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.SLWFP
                                                                                 || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting
                                                                                 || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend
                                                                                 || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday

                                                                                 )).Select(x => x.TotalDays).Sum()


                                 let beforeLeaveWithoutR_R = mybeforeAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                                            && (
                                                                            d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave
                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave
                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.HomeLeave
                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.PaternityLeave

                                                                               || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave
                                                                               || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave
                                                                               || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.SLWFP
                                                                               || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting
                                                                               || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend
                                                                               || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday
                                                                               )).
                                                                               Select(x => (x.ToDate - period.StartMonth).Value.TotalDays - 1).FirstOrDefault()


                                 #endregion

                                 let currLeavesSumDays = mycurrenmonthtattendacLeaves.Where(d => d.StaffGUID == x.UserGUID &&
                                                                                            (
                                                                                            d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave
                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave
                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.HomeLeave
                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.PaternityLeave
                                                                                                 || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR
                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave
                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave
                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.SLWFP
                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting
                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend
                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday
                                                                                            )).Select(x => x.TotalDays).Sum()
                                 #region Days for rental between month and date

                                 let totalDaysLeaveBeforeTillEndMonth = mybeforeAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                                                                      && (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave
                                                                                                      || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave
                                                                                                      || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.HomeLeave
                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.PaternityLeave
                                                                                                      || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR
                                                                                                         || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave
                                                                                                         || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave
                                                                                                         || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.SLWFP
                                                                                                         || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting
                                                                                                         || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend
                                                                                                         || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday
                                                                                                         )).
                                                                                                         Select(x => (x.ToDate - period.StartMonth).Value.TotalDays - 1).FirstOrDefault()
                                 let totalDaysLeaveAfterTillEndMonth = myafterAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                                                                     && (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave
                                                                                                     || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave
                                                                                                     || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.HomeLeave
                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.PaternityLeave
                                                                                                      || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.SLWFP
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday
                                                                                                        )).
                                                                                                        Select(x => (period.EndMonth - x.FromDate).Value.TotalDays - 1).FirstOrDefault()



                                 let totalDaysLeaveBeforeWithoutR_R_TillEndMonth = mybeforeAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                                                                      && (
                                                                                                      d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave
                                                                                                      || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave
                                                                                                      || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.HomeLeave
                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.PaternityLeave

                                                                                                         || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave
                                                                                                         || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave
                                                                                                         || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.SLWFP
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday
                                                                                                         )).
                                                                                                         Select(x => (x.ToDate - period.StartMonth).Value.TotalDays - 1).FirstOrDefault()
                                 let totalDaysLeaveAfterWithoutR_R_TillEndMonth = myafterAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                                                                     && (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave
                                                                                                     || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave
                                                                                                     || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.HomeLeave
                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.PaternityLeave
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.SLWFP
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend
                                                                                                        || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday
                                                                                                        )).
                                                                                                        Select(x => (period.EndMonth - x.FromDate).Value.TotalDays).FirstOrDefault()
                                 let totalDaysLeaveBeforeR_R_TillEndMonth = mybeforeAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                                                                     && (
                                                                                                     d.InternationalStaffAttendanceTypeGUID ==
                                                                                                     coddeInternationalStaffAttendanceTypeAttendanceTable.RR

                                                                                                        )).
                                                                                                        Select(x => (x.ToDate - period.StartMonth).Value.TotalDays).FirstOrDefault()
                                 let totalDaysLeaveAfterR_R_TillEndMonth = myafterAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                                                                     && (d.InternationalStaffAttendanceTypeGUID ==
                                                                                                     coddeInternationalStaffAttendanceTypeAttendanceTable.RR

                                                                                                        )).
                                                                                                        Select(x => (period.EndMonth - x.FromDate).Value.TotalDays).FirstOrDefault()


                                 #endregion

                                 #region Leaves to count except r_r
                                 let totaldaysLeavsAfterWithout_R_RMonth = myafterAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                                                                    && (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave
                                                                                                    || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave
                                                                                                    || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.HomeLeave
                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.PaternityLeave

                                                                                                       || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave
                                                                                                       || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave
                                                                                                       || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.SLWFP
                                                                                                       || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting
                                                                                                       || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend
                                                                                                       || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday
                                                                                                       )).
                                                                                                       Select(x => x.TotalDays).FirstOrDefault()

                                 #endregion
                                 let totalCurrR_RLeave = (int?)mycurrenmonthtattendacLeaves.Where(d => d.StaffGUID == x.UserGUID && d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR
                                                                                                            ).Select(d => d.TotalDays).Sum()

                                 let totalCurTravel_TimeTTLeave = (int?)mycurrenmonthtattendacLeaves.Where(d => d.StaffGUID == x.UserGUID && d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TravelTime
                               ).Select(d => d.TotalDays).Sum()


                                 let totalBeforeR_RLeave = mybeforeAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                         && (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR)

                                       ).Select(x => (x.ToDate - period.StartMonth).Value.TotalDays).Sum()

                                 let totalAfterR_RLeave = myafterAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                                 && (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR)

                                                                    ).Select(x => (period.EndMonth - x.FromDate).Value.TotalDays).FirstOrDefault()
                                 let totalBeforeR_RLeaveAllPeriod = mybeforeAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                                 && (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR)

                                                               ).Select(x => x.TotalDays).Sum()

                                 let totalAfterR_RLeaveAllPeriod = myafterAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                          && (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR)

                                                     ).Select(x => x.TotalDays).Sum()


                                 let totalBeforeAllLeavesAllPeriod = mybeforeAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                                                                         && (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave
                                                                                                         || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave
                                                                                                         || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.HomeLeave
                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.PaternityLeave
                                                                                                          || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR
                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave ||
                                                                                                            d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave
                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.SLWFP
                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting
                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend
                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday
                                                                                                            )).Select(x => x.TotalDays).Sum()

                                 let totalAfterAllLeavesAllPeriod = myafterAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                                                                            && (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave
                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.FamilyLeave
                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.HomeLeave
                                                                                                            || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.PaternityLeave
                                                                                                             || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR
                                                                                                               || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.CertificatedSickLeave ||
                                                                                                               d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.UncerticSickLeave
                                                                                                               || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.SLWFP
                                                                                                               || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.TeleCommuniting
                                                                                                               || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend
                                                                                                               || d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday
                                                                                                               )).Select(x => x.TotalDays).Sum()

                                 let totalAfterR_RTicker = myafterAttendaceLeaves.Where(d => d.StaffGUID == x.UserGUID
                                                                                                               && (d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR)
                                                                                                                  && !countrieNotAllowedGUIDs.Contains(d.CountryGUID)
                                                                                             ).Select(x => x.TotalDays).FirstOrDefault()

                                 let myentitlementTypePerDutyStation = entitlementTypePerDutyStation.Where(f => f.DutyStationGUID == x.DutyStationGUID)
                                 select new dataAHDInternationalStaffEntitlement
                                 {
                                     InternationalStaffEntitlementGUID = myInternationalStaffEntitlementGUID,
                                     StaffGUID = x.UserGUID,
                                     PeriodEntitlementGUID = period.PeriodEntitlementGUID,
                                     FlowStatusGUID = InternationalStaffEntitlmentFlowStatus.Submitted,
                                     LastFlowStatusName = "Submitted",
                                     Active = true,
                                     StaffName = staffName.Select(f => f.FirstName).FirstOrDefault() + " " + staffName.Select(f => f.Surname).FirstOrDefault(),

                                     dataAHDInternationalStaffEntitlementDetail = entilementtypes.Select(f =>
                                                                                          new dataAHDInternationalStaffEntitlementDetail
                                                                                          {
                                                                                              InternationalStaffEntitlementDetailGUID = Guid.NewGuid(),
                                                                                              InternationalStaffEntitlementGUID = myInternationalStaffEntitlementGUID,
                                                                                              EntitlementTypeGUID = f.EntitlementTypeGUID,
                                                                                              TotalDays = myAllLeavesCountForLeaveMoreThanMonth > 0 ?
                                                                                                          myAllLeavesCountForLeaveMoreThanMonth
                                                                                                          :
                                                                                                         (
                                                                                                          //total days
                                                                                                          myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).FirstOrDefault().EntitlementTypeGUID == InternationalStaffEntitlementType.R_RTicket ?
                                                                                                           //(((int?)totalCurrR_RLeave>7? (int?)totalCurrR_RLeave : totalCurrR_RLeave)  +
                                                                                                           //((int?)totalBeforeR_RLeaveAllPeriod > 7? (int?)totalDaysLeaveBeforeR_R_TillEndMonth > 0 ? (int?)totalDaysLeaveBeforeR_R_TillEndMonth : totalBeforeR_RLeaveAllPeriod : 0)
                                                                                                           //)
                                                                                                           (
                                                                                                          mycurrenmonthtattendacLeaves
                                                                                                                      .Where(d => d.StaffGUID == x.UserGUID
                                                                                                          && d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR
                                                                                                          && (d.CountryGUID == null || !countrieNotAllowedGUIDs.Contains(d.CountryGUID))
                                                                                                            ).Select(d => d.TotalDays).Sum() ?? 0 +

                                                                                                             totalAfterR_RTicker ?? 0
                                                                                                             )

                                                                                                          ///start 
                                                                                                          : myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).FirstOrDefault().EntitlementTypeGUID == InternationalStaffEntitlementType.RentalDeduction ?
                                                                                                                    (
                                                                                                                    (x.DutyStationGUID == tartusStationGUID)
                                                                                                                    ?
                                                                                                                    0


                                                                                                                    :
                                                                                                                    (1 == 2)
                                                                                                                    //(myalltelesDaysWithleavs != null && myalltelesDaysWithleavs.Select(x => x.totalDays).Sum() > 0)
                                                                                                                    ?
                                                                                                                    0
                                                                                                                    //period.TotalDaysInMonth - myalltelesDaysWithleavs.Select(x => x.totalDays).Sum()
                                                                                                                    //myalltelesDaysWithleavs.FirstOrDefault().totalDays
                                                                                                                    :
                                                                                                                   x.DutyStationGUID == aleppoStationGUID
                                                                                                                   ?
                                                                                                                    ((currLeavesSumDays > 0 ? currLeavesSumDays : 0) + (totalDaysLeaveBeforeTillEndMonth > 0 ? totalDaysLeaveBeforeTillEndMonth : 0) +
                                                                                                                    ((totalCurTravel_TimeTTLeave > 0 ? totalCurTravel_TimeTTLeave : 0)) > 15

                                                                                                                         ?
                                                                                                                         ((period.TotalDaysInMonth - (currLeavesSumDays ?? 0 + totalDaysLeaveBeforeTillEndMonth + totalCurTravel_TimeTTLeave)) > period.TotalDaysInMonth ?
                                                                                                           0 :
                                                                                                           (int?)((period.TotalDaysInMonth - (currLeavesSumDays ?? 0 + totalDaysLeaveBeforeTillEndMonth + totalCurTravel_TimeTTLeave))
                                                                                                           + ((currLeavesSumDays ?? 0 + totalDaysLeaveBeforeTillEndMonth + totalCurTravel_TimeTTLeave) - 15)
                                                                                                           )
                                                                                                           )
                                                                                                                          //(int?)(Period.TotalDaysInMonth - (currLeavesSumDays ?? 0 + totalDaysLeaveBeforeTillEndMonth + totalCurTravel_TimeTTLeave))

                                                                                                                          :
                                                                                                                              (period.TotalDaysInMonth - (currLeavesSumDays > 0 ? currLeavesSumDays : 0 +
                                                                                                                                 (int?)totalDaysLeaveBeforeTillEndMonth > 0 ? (int?)totalDaysLeaveBeforeTillEndMonth : 0 + totalCurTravel_TimeTTLeave))
                                                                                                                                 )

                                                                                                                   :
                                                                                                                   period.TotalDaysInMonth
                                                                                                                    //end rental 



                                                                                                                    )
                                                                                                                    //other r leaves
                                                                                                                    :

                                                                                                                  (myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).FirstOrDefault().EntitlementTypeGUID == InternationalStaffEntitlementType.DangerPayPerDay ?
                                                                                                                                                     (period.TotalDaysInMonth - (int?)
                                                                                                                                                                       (
                                                                                                                                                                                       ((int?)totalCurrR_RLeave > 7 ? totalCurrR_RLeave - 7 : 0) +
                                                                                                                                                                                       ((int?)totalBeforeR_RLeaveAllPeriod > 7 ? totalDaysLeaveBeforeR_R_TillEndMonth > 0 ? totalDaysLeaveBeforeR_R_TillEndMonth : 0 : 0) +
                                                                                                                                                                                       /////no need for after
                                                                                                                                                                                       ((int?)totalAfterR_RLeaveAllPeriod > 7 ? totalDaysLeaveAfterR_R_TillEndMonth > 0 ? totalDaysLeaveAfterR_R_TillEndMonth : 0 : 0) +
                                                                                                                                                                                       ///////
                                                                                                                                                                                       ((int?)currLeaveWithoutR_R > 0 ? currLeaveWithoutR_R : 0) +
                                                                                                                                                                                       ((int?)totalDaysLeaveBeforeWithoutR_R_TillEndMonth > 0 ? totalDaysLeaveBeforeWithoutR_R_TillEndMonth : 0) +
                                                                                                                                                                                       ((int?)totalDaysLeaveAfterWithoutR_R_TillEndMonth > 0 ? totalDaysLeaveAfterWithoutR_R_TillEndMonth : 0)

                                                                                                                                                                                       )) :

                                                                                                                       0)


                                                                                                         ),
                                                                                              BasePeriodAmount = myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).FirstOrDefault() != null ? myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).FirstOrDefault().EntitlementValue : 0,
                                                                                              TotalAmount = myAllLeavesCountForLeaveMoreThanMonth > 0 ?
                                                                                              0
                                                                                              :
                                                                                                          //##############################################################################################################################################################################################################################################
                                                                                                          //--A    //R_R Ticket
                                                                                                          myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID ==
                                                                                                          f.EntitlementTypeGUID).FirstOrDefault().EntitlementTypeGUID == InternationalStaffEntitlementType.R_RTicket
                                                                                                          ?
                                                                                                          //checkxRR
                                                                                                          ((

                                                                                                          mycurrenmonthtattendacLeaves
                                                                                                                      .Where(d => d.StaffGUID == x.UserGUID
                                                                                                          && d.InternationalStaffAttendanceTypeGUID == coddeInternationalStaffAttendanceTypeAttendanceTable.RR
                                                                                                          && (d.CountryGUID == null || !countrieNotAllowedGUIDs.Contains(d.CountryGUID))
                                                                                                            ).Select(d => d.TotalDays).Sum() > 0
                                                                                                             ||
                                                                                                             totalAfterR_RTicker > 0
                                                                                                             ) ?
                                                                                                              myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).
                                                                                                              FirstOrDefault().EntitlementValue : 0

                                                                                                              )
                                                                                                        //##############################################################################################################################################################################################################################################       

                                                                                                        //--B         // Rental Deducation

                                                                                                        :

                                                                                                        myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).FirstOrDefault().
                                                                                                                   EntitlementTypeGUID == InternationalStaffEntitlementType.RentalDeduction
                                                                                                  ? (x.DutyStationGUID == tartusStationGUID)
                                                                                                                   ?
                                                                                                                   0


                                                                                                                   :


                                                                                                       ///////// //tele working

                                                                                                       (
                                                                                                                 //myalltelesDaysWithleavs != null && myalltelesDaysWithleavs.Select(x => x.totalDays).Sum() > 0
                                                                                                                 1 == 2

                                                                                                                   )

                                                                                                                   ?
                                                                                                                   0
                                                                                                       //(myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).
                                                                                                       // FirstOrDefault().EntitlementValue / period.TotalDaysInMonth)

                                                                                                       // * ((myalltelesDaysWithleavs != null && myalltelesDaysWithleavs.Select(x => x.totalDays).Sum() > period.TotalDaysInMonth)
                                                                                                       //              ?
                                                                                                       //             0
                                                                                                       //                :
                                                                                                       //          (period.TotalDaysInMonth - (myalltelesDaysWithleavs != null ? myalltelesDaysWithleavs.Select(x => x.totalDays).Sum() : 0)
                                                                                                       //          ))

                                                                                                       //end tele wroking
                                                                                                       :


                                                                                                      //Aleppo

                                                                                                      x.DutyStationGUID == aleppoStationGUID
                                                                                                      ?
                                                                                                        (

                                                                                                       ((currLeavesSumDays > 0 ? currLeavesSumDays : 0) + (totalDaysLeaveBeforeTillEndMonth > 0 ?
                                                                                                               totalDaysLeaveBeforeTillEndMonth : 0) + ((totalCurTravel_TimeTTLeave > 0 ? totalCurTravel_TimeTTLeave : 0)) > 15
                                                                                                       //totalBeforeAllLeavesAllPeriod > 15
                                                                                                       )
                                                                                                          ///above 15 days
                                                                                                          ?
                                                                                                          (myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).FirstOrDefault().EntitlementValue
                                                                                                             / period.TotalDaysInMonth) *
                                                                                                           ((period.TotalDaysInMonth - (currLeavesSumDays ?? 0 + totalDaysLeaveBeforeTillEndMonth + totalCurTravel_TimeTTLeave)) > period.TotalDaysInMonth ?
                                                                                                           0 :
                                                                                                           (int?)((period.TotalDaysInMonth - (currLeavesSumDays ?? 0 + totalDaysLeaveBeforeTillEndMonth + totalCurTravel_TimeTTLeave))
                                                                                                           + ((currLeavesSumDays ?? 0 + totalDaysLeaveBeforeTillEndMonth + totalCurTravel_TimeTTLeave) - 15)
                                                                                                           )
                                                                                                           )
                                                                                                      //((currLeavesSumDays > 15?((Period.TotalDaysInMonth - currLeavesSumDays) + (currLeavesSumDays - 15))
                                                                                                      //                                                   : Period.TotalDaysInMonth - currLeavesSumDays)+(totalDaysLeaveBeforeTillEndMonth > 15 ?
                                                                                                      //                                                   ((int)totalDaysLeaveBeforeTillEndMonth - 15) : 0 ))


                                                                                                      :
                                                                                                        //less 15

                                                                                                        ((currLeavesSumDays > 0 ? currLeavesSumDays : 0) + (totalDaysLeaveBeforeTillEndMonth > 0 ? totalDaysLeaveBeforeTillEndMonth : 0) + (totalCurTravel_TimeTTLeave > 0 ? totalCurTravel_TimeTTLeave : 0)
                                                                                                        <= 15
                                                                                                        &&
                                                                                                        ((currLeavesSumDays > 0 ? currLeavesSumDays : 0) +
                                                                                                        (totalDaysLeaveBeforeTillEndMonth > 0 ?
                                                                                                        totalDaysLeaveBeforeTillEndMonth : 0) +
                                                                                                        (totalCurTravel_TimeTTLeave > 0 ? totalCurTravel_TimeTTLeave : 0)) > 0
                                                                                                       //totalBeforeAllLeavesAllPeriod > 15
                                                                                                       )
                                                                                                      ?
                                                                                                         (myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).FirstOrDefault().EntitlementValue / period.TotalDaysInMonth)
                                                                                                         * (period.TotalDaysInMonth - (currLeavesSumDays + (int?)totalDaysLeaveBeforeTillEndMonth + totalCurTravel_TimeTTLeave))






                                                                                                        //////////////////////////////////////////////

                                                                                                        //default option

                                                                                                        :
                                                                                                        myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).FirstOrDefault().EntitlementValue
                                                                                                        /// period.TotalDaysInMonth
                                                                                                        //* (period.TotalDaysInMonth - (currLeavesSumDays > 0 ? currLeavesSumDays : 0 +
                                                                                                        //(int?)totalDaysLeaveBeforeTillEndMonth > 0 ? (int?)totalDaysLeaveBeforeTillEndMonth : 0

                                                                                                        //+ (int?)totalCurTravel_TimeTTLeave > 0 ? (int?)totalCurTravel_TimeTTLeave : 0
                                                                                                        //))



                                                                                                        )

                                                                                                        //end Aleepo
                                                                                                        :




                                                                                                        myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).FirstOrDefault().EntitlementValue

                                                                                                        //##############################################################################################################################################################################################################################################     
                                                                                                        // Danger Pay 
                                                                                                        :
                                                                                                   myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).FirstOrDefault().EntitlementTypeGUID == InternationalStaffEntitlementType.DangerPayPerDay

                                                                                                   ?

                                                                                                   (
                                                                                                   ((totalBeforeR_RLeave == 0 || totalBeforeR_RLeave <= 7 || totalBeforeR_RLeave == null)
                                                                                                   && (totalCurrR_RLeave == 0 || totalCurrR_RLeave <= 7 || totalCurrR_RLeave == null)
                                                                                                   && (totalAfterR_RLeave == 0 || totalAfterR_RLeave <= 7 || totalAfterR_RLeave == null)
                                                                                                   &&
                                                                                                   (currLeaveWithoutR_R == 0 || currLeaveWithoutR_R == null) && (totalDaysLeaveBeforeWithoutR_R_TillEndMonth == 0
                                                                                                          || totalDaysLeaveBeforeWithoutR_R_TillEndMonth == null)
                                                                                                   && (totalDaysLeaveAfterWithoutR_R_TillEndMonth == 0 || totalDaysLeaveAfterWithoutR_R_TillEndMonth == null)
                                                                                                   )
                                                                                                        ?
                                                                                                         1645
                                                                                                        :


                                                                                                       (
                                                                                                      (period.TotalDaysInMonth -
                                                                                                       (int?)
                                                                                                       (
                                                                                                                       ((int?)totalCurrR_RLeave > 7 ? totalCurrR_RLeave - 7 : 0) +
                                                                                                                       ((int?)totalBeforeR_RLeaveAllPeriod > 7 ? totalDaysLeaveBeforeR_R_TillEndMonth > 0 ? totalDaysLeaveBeforeR_R_TillEndMonth : 0 : 0) +
                                                                                                                       /////no need for after
                                                                                                                       ((int?)totalAfterR_RLeaveAllPeriod > 7 ? totalDaysLeaveAfterR_R_TillEndMonth > 0 ? totalDaysLeaveAfterR_R_TillEndMonth : 0 : 0) +
                                                                                                                       ///////
                                                                                                                       ((int?)currLeaveWithoutR_R > 0 ? currLeaveWithoutR_R : 0) +
                                                                                                                       ((int?)totalDaysLeaveBeforeWithoutR_R_TillEndMonth > 0 ? totalDaysLeaveBeforeWithoutR_R_TillEndMonth : 0) +
                                                                                                                       ((int?)totalDaysLeaveAfterWithoutR_R_TillEndMonth > 0 ? totalDaysLeaveAfterWithoutR_R_TillEndMonth : 0)

                                                                                                                       ))

                                                                                                                        * myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).FirstOrDefault().
                                                                                                                        EntitlementValue
                                                                                                                        )


                                                                                                       )


                                                                                                       : 0,
                                                                                              //###########################
                                                                                              IsToAdd = (bool)myentitlementTypePerDutyStation.Where(a => a.EntitlementTypeGUID == f.EntitlementTypeGUID).Select(x => x.codeAHDEntitlementType.IsToAdd).FirstOrDefault(),
                                                                                              Active = true,

                                                                                          }).ToList(),
                                     dataAHDInternationalStaffEntitlementFlow = new List<dataAHDInternationalStaffEntitlementFlow>
                                     {
                                         new dataAHDInternationalStaffEntitlementFlow
                                         {
                                         InternationalStaffEntitlementFlowGUID = Guid.NewGuid(),
                                         InternationalStaffEntitlementGUID= myInternationalStaffEntitlementGUID,
                                         CreatedByGUID=UserGUID,

                                         FlowStatusGUID=InternationalStaffEntitlmentFlowStatus.Submitted,
                                         ActionDate=ExecutionTime,
                                         IsLastAction=true,
                                         OrderId=1,
                                         Active=true,

                                         }
}


                                 }).ToList();




            DbAHD.CreateBulk(staffEntitlements, Permissions.InternationalStaffEntitlements.CreateGuid, ExecutionTime, DbCMS);


            // Guid maksoud = Guid.Parse("8F7EF83F-FD3E-4F8C-8735-8A22D3D61B75");
            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.InternationalStaffEntitlementPeriodDataTable, ControllerContext, "InternationalStaffEntitlementPeriodLanguagesFormControls"));
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.InternationalStaffEntitlements.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("Create", "InternationalStaffEntitlementPeriods", new { Area = "AHD" })), Container = "InternationalStaffEntitlementPeriodDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.InternationalStaffEntitlements.Update, Apps.AHD), Container = "InternationalStaffEntitlementPeriodDetailFormControls" });
            //var staffs = DbAHD.dataStaffEligibleForDangerPay.ToList();







            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                // SendConfirmationReceivingModelEmail(EntityPK);
                return RedirectToAction("ConfirmationView");
                //  return Json(DbAHD.SingleUpdateMessage(DataTableNames.InternationalStaffEntitlementPeriodDataTable, DbAHD.PrimaryKeyControl(model), DbAHD.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            //catch (Exception ex)
            //{
            //    return Json(DbAHD.ErrorMessage(ex.Message));
            //}
        }

        public ActionResult SendEmailForEntitlementNotConfirmed(Guid FK)
        {

            string URL = "";
            string Anchor = "";
            string Link = "";
            var _myperiod = DbAHD.dataAHDPeriodEntitlement.Where(x => x.PeriodEntitlementGUID == FK).FirstOrDefault();
            var staffs = DbAHD.dataAHDInternationalStaffEntitlement.Where(x => x.PeriodEntitlementGUID == FK && x.FlowStatusGUID == InternationalStaffEntitlmentFlowStatus.Submitted).ToList();
            var staffGUIDs = staffs.Select(x => x.StaffGUID).Distinct().ToList();

            var allPers = DbAHD.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).ToList();
            var allUsers = DbAHD.userPersonalDetailsLanguage.Where(x => staffGUIDs.Contains(x.UserGUID)
                                                                          && x.LanguageID == LAN).ToList();
            var alluserAccounts = DbAHD.userServiceHistory.Where(x => staffGUIDs.Contains(x.UserGUID)).ToList();
            var monthName = ProcessData.GetMonthName(_myperiod.StartMonth.Value.Month);
            var YearName = _myperiod.StartMonth.Value.Year;




            for (int i = 0; i < allUsers.Count(); i += 30)
            {
                var target = staffs.Skip(i).Take(30);

                foreach (var user in target)
                {

                    var currPrepare = allPers.Where(x => x.UserGUID == user.PreparedByGUID
                                                                             && x.LanguageID == LAN).FirstOrDefault();

                    var currPrepartion = DbAHD.userServiceHistory.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
                    var currAccount = DbAHD.userServiceHistory.Where(x => x.UserGUID == user.StaffGUID).ToList();

                    //{
                    var _currstaffPers = allPers.Where(x => x.UserGUID == user.StaffGUID
                                                                        && x.LanguageID == LAN).FirstOrDefault();


                    // string SubjectMessage = resxEmails.EntitlemenReviewByStaffSubject.Replace("$Month", monthName).Replace("$Year", _myperiod.StartMonth.Value.Year.ToString());
                    string SubjectMessage = resxEmails.EntitlemenReviewByStaffSubject.Replace("$SurName", _currstaffPers.Surname).Replace("$Month", monthName).Replace("$Year", _myperiod.StartMonth.Value.Year.ToString());
                    // string SubjectMessage = resxEmails.EntitlemenReviewByStaffSubject.Replace("$SurName", _currstaffPers.Surname).Replace("$Month", monthName).Replace("$Year", Entitlemnt.dataAHDPeriodEntitlement.StartMonth.Value.Year.ToString());

                    //to send mail to staff 
                    // var currentDanger = allDan.Where(x => x.dataStaffEligibleForDangerPay.UserGUID == user.UserGUID).FirstOrDefault();
                    URL = AppSettingsKeys.Domain + "/AHD/InternationalStaffEntitlement/EntitlementToReviewByStaffUpdate/?PK=" + new Portal().GUIDToString(user.InternationalStaffEntitlementGUID);
                    Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmDangerPayReceiving + "</a>";
                    Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";


                    string _message = resxEmails.EntitlementReviewByStaffConfirmation
             .Replace("$FullName", _currstaffPers.FirstName + " " + _currstaffPers.Surname)
             .Replace("$VerifyLink", Anchor)
             .Replace("$SentBy", currPrepare.FirstName + " " + currPrepare.Surname)
             .Replace("$Period ", monthName + " " + YearName);
                    if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                    int isRec = 1;
                    var myEmail = alluserAccounts.Where(x => x.UserGUID == user.StaffGUID).Select(x => x.EmailAddress).FirstOrDefault();

                    Send(myEmail, SubjectMessage, _message, isRec, null);
                }
            }
            return RedirectToAction("ConfirmationView");


        }
        #endregion

        #region Init Values per duty stations
        // GET: AHD/EntitlementsInitCalacuationss
        [Route("AHD/EntitlementsInitCalacuationsIndex/")]
        public ActionResult EntitlementsInitCalacuationsIndex()
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Access, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }

            return View("~/Areas/AHD/Views/InternationalStaffEntitlementPeriod/EntitlementsInitCalacuations/Index.cshtml");
        }


        [Route("AHD/EntitlementsInitCalacuationDataTable/")]

        public JsonResult EntitlementsInitCalacuationDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<EntitlementsInitCalacuationsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<EntitlementsInitCalacuationsDataTableModel>(DataTable.Filters);
            }
            var All = (
               from a in DbAHD.codeAHDEntitlementTypePerDutyStation.AsExpandable()
               join b in DbAHD.codeAHDEntitlementType.Where(x => x.Active) on a.EntitlementTypeGUID equals b.EntitlementTypeGUID into LJ1
               from R1 in LJ1.DefaultIfEmpty()
               join c in DbAHD.codeDutyStationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DutyStationGUID equals c.DutyStationGUID into LJ2
               from R2 in LJ2.DefaultIfEmpty()
               join d in DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.CreateByGUID equals d.UserGUID into LJ3
               from R3 in LJ3.DefaultIfEmpty()
               join e in DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.UpdateByGUID equals e.UserGUID into LJ4
               from R4 in LJ4.DefaultIfEmpty()
               select new EntitlementsInitCalacuationsDataTableModel
               {
                   EntitlementTypePerDutyStationGUID = a.EntitlementTypePerDutyStationGUID.ToString(),
                   EntitlementTypeGUID = a.EntitlementTypeGUID.ToString(),
                   EntitlementType = R1.EntitlementTypeName,
                   DutyStationGUID = a.DutyStationGUID.ToString(),
                   DutyStation = R2.DutyStationDescription,
                   EntitlementValue = a.EntitlementValue,

                   CreateDate = a.CreateDate,
                   CreatedByGUID = a.CreateByGUID.ToString(),
                   CreatedBy = R3.FirstName + " " + R3.Surname,
                   UpdateDate = a.UpdateDate,
                   UpdateByGUID = a.UpdateByGUID.ToString(),
                   UpdateBy = R4.FirstName + " " + R4.Surname,


                   Active = a.Active,


                   codeAHDEntitlementTypePerDutyStationRowVersion = a.codeAHDEntitlementTypePerDutyStationRowVersion
               }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<EntitlementsInitCalacuationsDataTableModel> Result = Mapper.Map<List<EntitlementsInitCalacuationsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).OrderBy(x => x.DutyStation).OrderBy(x => x.EntitlementTypeGUID).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        //[Route("AHD/EntitlementsInitCalacuation/Create/")]
        public ActionResult EntitlementsInitCalacuationCreate()
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            return PartialView("~/Areas/AHD/Views/InternationalStaffEntitlementPeriod/EntitlementsInitCalacuations/_EntitlementsInitCalacuationsModal.cshtml", new EntitlementsInitCalacuationsUpdateModel { EntitlementTypePerDutyStationGUID = Guid.Empty, IsCalculatedPerDay = false });

        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EntitlementsInitCalacuationCreate(EntitlementsInitCalacuationsUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/AHD/Views/InternationalStaffEntitlementPeriod/EntitlementsInitCalacuations/_EntitlementsInitCalacuationsModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeAHDEntitlementTypePerDutyStation _entitlementType = Mapper.Map(model, new codeAHDEntitlementTypePerDutyStation());
            _entitlementType.EntitlementTypePerDutyStationGUID = EntityPK;
            _entitlementType.EntitlementTypeGUID = model.EntitlementTypeGUID;
            _entitlementType.EntitlementValue = model.EntitlementValue;
            _entitlementType.IsCalculatedPerDay = model.IsCalculatedPerDay;
            _entitlementType.DutyStationGUID = model.DutyStationGUID;



            _entitlementType.CreateByGUID = UserGUID;
            _entitlementType.CreateDate = ExecutionTime;




            DbAHD.Create(_entitlementType, Permissions.InternationalStaffEntitlements.CreateGuid, ExecutionTime, DbCMS);



            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.EntitlementsInitCalacuationDataTable, ControllerContext, "EntitlementsInitCalacuationsLanguagesFormControls"));
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.InternationalStaffEntitlements.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("Create", "EntitlementsInitCalacuationss", new { Area = "AHD" })), Container = "EntitlementsInitCalacuationsDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.InternationalStaffEntitlements.Update, Apps.AHD), Container = "EntitlementsInitCalacuationsDetailFormControls" });

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbAHD.SingleUpdateMessage(DataTableNames.EntitlementsInitCalacuationDataTable, DbAHD.PrimaryKeyControl(_entitlementType), DbAHD.RowVersionControls(Portal.SingleToList(_entitlementType))));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }


        public ActionResult EntitlementsInitCalacuationUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Access, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            var model = (from a in DbAHD.codeAHDEntitlementTypePerDutyStation.WherePK(PK)

                         select new EntitlementsInitCalacuationsUpdateModel
                         {
                             EntitlementTypePerDutyStationGUID = a.EntitlementTypePerDutyStationGUID,
                             EntitlementTypeGUID = a.EntitlementTypeGUID,
                             EntitlementValue = a.EntitlementValue,
                             DutyStationGUID = a.DutyStationGUID,
                             CreatedByGUID = a.CreateByGUID,
                             IsCalculatedPerDay = a.IsCalculatedPerDay != null ? (bool)a.IsCalculatedPerDay : false,

                             Active = a.Active,
                             codeAHDEntitlementTypePerDutyStationRowVersion = a.codeAHDEntitlementTypePerDutyStationRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("EntitlementsInitCalacuations", "EntitlementsInitCalacuationss", new { Area = "AHD" }));
            return PartialView("~/Areas/AHD/Views/InternationalStaffEntitlementPeriod/EntitlementsInitCalacuations/_EntitlementsInitCalacuationsModal.cshtml", model);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EntitlementsInitCalacuationUpdate(EntitlementsInitCalacuationsUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Update, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/AHD/Views/InternationalStaffEntitlementPeriod/EntitlementsInitCalacuations/_EntitlementsInitCalacuationsModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;


            var _myModel = DbAHD.codeAHDEntitlementTypePerDutyStation.Where(x => x.EntitlementTypePerDutyStationGUID == model.EntitlementTypePerDutyStationGUID).FirstOrDefault();
            _myModel.EntitlementValue = model.EntitlementValue;
            _myModel.DutyStationGUID = model.DutyStationGUID;
            _myModel.EntitlementTypeGUID = model.EntitlementTypeGUID;
            _myModel.IsCalculatedPerDay = model.IsCalculatedPerDay;
            _myModel.UpdateByGUID = UserGUID;
            _myModel.UpdateDate = ExecutionTime;
            DbAHD.Update(_myModel, Permissions.InternationalStaffEntitlements.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                codeAHDEntitlementTypePerDutyStation EntitlementsInitCalacuations = Mapper.Map(model, new codeAHDEntitlementTypePerDutyStation());
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.EntitlementsInitCalacuationDataTable, DbAHD.PrimaryKeyControl(EntitlementsInitCalacuations
), DbAHD.RowVersionControls(Portal.SingleToList(EntitlementsInitCalacuations))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyEntitlementsInitCalacuation((Guid)model.EntitlementTypePerDutyStationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EntitlementsInitCalacuationDelete(codeAHDEntitlementTypePerDutyStation model)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Delete, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            List<codeAHDEntitlementTypePerDutyStation> DeletedEntitlementsInitCalacuation = DeleteEntitlementsInitCalacuation(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.InternationalStaffEntitlements.Restore, Apps.AHD), Container = "EntitlementsInitCalacuationFormControls" });

            try
            {
                int CommitedRows = DbAHD.SaveChanges();
                DbAHD.SaveChanges();
                return Json(DbAHD.SingleDeleteMessage(CommitedRows, DeletedEntitlementsInitCalacuation.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyEntitlementsInitCalacuation(model.EntitlementTypePerDutyStationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EntitlementsInitCalacuationRestore(codeAHDEntitlementTypePerDutyStation model)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Restore, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            if (ActiveEntitlementsInitCalacuation(model))
            {
                return Json(DbAHD.RecordExists());
            }

            List<codeAHDEntitlementTypePerDutyStation> RestoredEntitlementsInitCalacuation = RestoreEntitlementsInitCalacuations(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.InternationalStaffEntitlements.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("EntitlementsInitCalacuationCreate", "Configuration", new { Area = "AHD" })), Container = "EntitlementsInitCalacuationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.InternationalStaffEntitlements.Update, Apps.AHD), Container = "EntitlementsInitCalacuationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.InternationalStaffEntitlements.Delete, Apps.AHD), Container = "EntitlementsInitCalacuationFormControls" });

            try
            {
                int CommitedRows = DbAHD.SaveChanges();
                DbAHD.SaveChanges();
                return Json(DbAHD.SingleRestoreMessage(CommitedRows, RestoredEntitlementsInitCalacuation, DbAHD.PrimaryKeyControl(RestoredEntitlementsInitCalacuation.FirstOrDefault()), Url.Action(DataTableNames.EntitlementsInitCalacuationDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyEntitlementsInitCalacuation(model.EntitlementTypePerDutyStationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult EntitlementsInitCalacuationDataTableDelete(List<codeAHDEntitlementTypePerDutyStation> models)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Delete, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            List<codeAHDEntitlementTypePerDutyStation> DeletedEntitlementsInitCalacuation = DeleteEntitlementsInitCalacuation(models);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialDeleteMessage(DeletedEntitlementsInitCalacuation, models, DataTableNames.EntitlementsInitCalacuationDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult EntitlementsInitCalacuationDataTableRestore(List<codeAHDEntitlementTypePerDutyStation> models)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Restore, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            List<codeAHDEntitlementTypePerDutyStation> RestoredEntitlementsInitCalacuation = DeleteEntitlementsInitCalacuation(models);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialRestoreMessage(RestoredEntitlementsInitCalacuation, models, DataTableNames.EntitlementsInitCalacuationDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        private List<codeAHDEntitlementTypePerDutyStation> DeleteEntitlementsInitCalacuation(List<codeAHDEntitlementTypePerDutyStation> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<codeAHDEntitlementTypePerDutyStation> DeletedEntitlementsInitCalacuation = new List<codeAHDEntitlementTypePerDutyStation>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT EntitlementTypePerDutyStationGUID,CONVERT(varchar(50), EntitlementTypePerDutyStationGUID) as C2 ,codeAHDEntitlementTypePerDutyStationRowVersion FROM code.codeAHDEntitlementTypePerDutyStation where EntitlementTypePerDutyStationGUID in (" + string.Join(",", models.Select(x => "'" + x.EntitlementTypePerDutyStationGUID + "'").ToArray()) + ")";

            string query = DbAHD.QueryBuilder(models, Permissions.InternationalStaffEntitlements.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbAHD.Database.SqlQuery<codeAHDEntitlementTypePerDutyStation>(query).ToList();
            foreach (var record in Records)
            {
                DeletedEntitlementsInitCalacuation.Add(DbAHD.Delete(record, ExecutionTime, Permissions.InternationalStaffEntitlements.DeleteGuid, DbCMS));
            }


            return DeletedEntitlementsInitCalacuation;
        }

        private List<codeAHDEntitlementTypePerDutyStation> RestoreEntitlementsInitCalacuations(List<codeAHDEntitlementTypePerDutyStation> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<codeAHDEntitlementTypePerDutyStation> RestoredEntitlementsInitCalacuation = new List<codeAHDEntitlementTypePerDutyStation>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT EntitlementTypePerDutyStationGUID,CONVERT(varchar(50), EntitlementTypePerDutyStationGUID) as C2 ,codeAHDEntitlementTypePerDutyStationRowVersion FROM code.codeAHDEntitlementTypePerDutyStation where EntitlementTypePerDutyStationGUID in (" + string.Join(",", models.Select(x => "'" + x.EntitlementTypePerDutyStationGUID + "'").ToArray()) + ")";

            string query = DbAHD.QueryBuilder(models, Permissions.InternationalStaffEntitlements.DeleteGuid, SubmitTypes.Restore, "");

            var Records = DbAHD.Database.SqlQuery<codeAHDEntitlementTypePerDutyStation>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveEntitlementsInitCalacuation(record))
                {
                    RestoredEntitlementsInitCalacuation.Add(DbAHD.Restore(record, Permissions.InternationalStaffEntitlements.DeleteGuid, Permissions.InternationalStaffEntitlements.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredEntitlementsInitCalacuation;
        }

        private JsonResult ConcurrencyEntitlementsInitCalacuation(Guid PK)
        {
            StaffRenwalResidencyModel dbModel = new StaffRenwalResidencyModel();

            var EntitlementsInitCalacuation = DbAHD.codeAHDEntitlementTypePerDutyStation.Where(x => x.EntitlementTypePerDutyStationGUID == PK).FirstOrDefault();
            var dbEntitlementsInitCalacuation = DbAHD.Entry(EntitlementsInitCalacuation).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbEntitlementsInitCalacuation, dbModel);

            if (EntitlementsInitCalacuation.codeAHDEntitlementTypePerDutyStationRowVersion.SequenceEqual(dbModel.dataStaffRenwalResidencyRowVersion))
            {
                return Json(DbAHD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbAHD, dbModel, "LanguagesContainer"));
        }

        private bool ActiveEntitlementsInitCalacuation(Object model)
        {
            codeAHDEntitlementTypePerDutyStation EntitlementsInitCalacuation = Mapper.Map(model, new codeAHDEntitlementTypePerDutyStation());
            int ModelDescription = DbAHD.codeAHDEntitlementTypePerDutyStation
                                    .Where(x => x.EntitlementValue == EntitlementsInitCalacuation.EntitlementValue &&
                                                x.EntitlementTypePerDutyStationGUID == EntitlementsInitCalacuation.EntitlementTypePerDutyStationGUID &&
                                                x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("Vehicle Request", "Vehicle Request is already exists");
            }
            return (ModelDescription > 0);
        }
        #endregion

        #region Reports

        #endregion


        #region Entitlement Documents



        public ActionResult EntitlementDocumentsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/AHD/Views/InternationalStaffEntitlement/EntitlementDocuments/_DocumentDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<EntitlementDocumentDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<EntitlementDocumentDataTableModel>(DataTable.Filters);
            }

            Guid myDocumentTypeGUID = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e9756");
            var Result = (from a in DbAHD.dataAHDInternationalStaffEntitlementDocument.AsExpandable().Where(x => x.Active && (x.InternationalStaffEntitlementGUID == PK))

                          join b in DbAHD.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN && x.codeTablesValues.TableGUID == myDocumentTypeGUID) on a.DocumentTypeGUID equals b.ValueGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          select new EntitlementDocumentDataTableModel
                          {
                              InternationalStaffEntitlementDocumentGUID = a.InternationalStaffEntitlementDocumentGUID.ToString(),
                              InternationalStaffEntitlementGUID = a.InternationalStaffEntitlementGUID.ToString(),

                              DocumentTypeGUID = a.DocumentTypeGUID.ToString(),
                              DocumentType = R1.ValueDescription,
                              Comments = a.Comments,

                              Active = a.Active,
                              dataAHDInternationalStaffEntitlementDocumentRowVersion = a.dataAHDInternationalStaffEntitlementDocumentRowVersion

                          }).Where(Predicate);


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult EntitlementDocumentsCreate(Guid FK)
        {
            //if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Create, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            return PartialView("~/Areas/AHD/Views/InternationalStaffEntitlement/EntitlementDocuments/_DocumentUpdateModal.cshtml",
                new EntitlementDocumentUpdateModel { InternationalStaffEntitlementGUID = FK });
        }

        public ActionResult DownloadEntitlementDocumentFile(Guid id)
        {



            var model = DbAHD.dataAHDInternationalStaffEntitlementDocument.Where(x => x.InternationalStaffEntitlementDocumentGUID == id).FirstOrDefault();
            var fullPath = model.InternationalStaffEntitlementDocumentGUID + "." + model.DocumentExtension;


            string sourceFile = Server.MapPath("~/Areas/AHD/UploadedDocuments/Entitlement/" + fullPath);


            byte[] fileBytes = System.IO.File.ReadAllBytes(sourceFile);

            string fileName = DateTime.Now.ToString("yyMMdd") + fullPath;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);








            // return Json(DbCMS.SingleUpdateMessage(null, null, null, null, "Please Wait...."));
        }



        [HttpPost]
        public FineUploaderResult UploadEntitlementDocuments(FineUpload upload, Guid InternationalStaffEntitlementGUID, Guid DocumentTypeGUID, string comments)
        {
            string error = "Error ";
            if (upload.FileName.EndsWith(".exe"))
            {
                return new FineUploaderResult((error == "" ? true : false), new { Error = error, path = "", success = (error == "" ? true : false) });

            }
            return new FineUploaderResult(true, new { path = UploadDocument(upload, InternationalStaffEntitlementGUID, DocumentTypeGUID, comments), success = true });
            //if (FileTypeValidator.IsPDF(upload.InputStream) || FileTypeValidator.IsImage(upload.InputStream) ||
            //    FileTypeValidator.IsExcel(upload.InputStream) ||
            //    FileTypeValidator.IsWord(upload.InputStream)
            //    )
            //{

            //    return new FineUploaderResult(true, new { path = UploadDocument(upload, InternationalStaffEntitlementGUID, DocumentTypeGUID, comments), success = true });
            //}

        }


        public string UploadDocument(FineUpload upload, Guid InternationalStaffEntitlementGUID, Guid DocumentTypeGUID, string comments)
        {
            var _stearm = upload.InputStream;
            DateTime ExecutionTime = DateTime.Now;
            //string _ext = Path.GetExtension(upload.FileName).Split('.')[1];
            dataAHDInternationalStaffEntitlementDocument documentUplod = new dataAHDInternationalStaffEntitlementDocument();
            documentUplod.InternationalStaffEntitlementDocumentGUID = Guid.NewGuid();
            //string FilePath = Server.MapPath("~/Areas/AHD/UploadedDocuments/" + documentUplod.ItemIntpuDetailUploadedDocumentGUID + _ext);

            string _ext = Path.GetExtension(upload.FileName).Split('.')[1];

            string FolderPath = Server.MapPath("~/Areas/AHD/UploadedDocuments/Entitlement/");
            Directory.CreateDirectory(FolderPath);
            //int LatestFileVersion = 0;
            //try { LatestFileVersion = (from a in DbPPA.PPAFileVersion.Where(x => x.PPAOriginalFileGUID == PPAOriginalFileGUID && x.FileActionByUserGUID == UserGUID) select a.FileVersion).Max(); } catch { }
            //if (LatestFileVersion == -1) LatestFileVersion = 0;



            string FilePath = FolderPath + "/" + documentUplod.InternationalStaffEntitlementDocumentGUID.ToString() + "." + _ext;

            using (var fileStream = System.IO.File.Create(FilePath))
            {
                upload.InputStream.Seek(0, SeekOrigin.Begin);
                upload.InputStream.CopyTo(fileStream);
            }

            documentUplod.InternationalStaffEntitlementGUID = InternationalStaffEntitlementGUID;

            documentUplod.DocumentExtension = _ext;
            documentUplod.DocumentTypeGUID = DocumentTypeGUID;
            documentUplod.Comments = comments;
            documentUplod.CreatedByGUID = UserGUID;
            documentUplod.CreateDate = ExecutionTime;

            //documentUplod.Comments = ItemInputDetailGUID;
            //documentUplod.CreatedByGUID = UserGUID;
            //documentUplod.CreatedDate = ExecutionTime;
            DbAHD.Create(documentUplod, Permissions.InternationalStaffEntitlements.CreateGuid, ExecutionTime, DbCMS);
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


            return "~/Areas/AHD/UploadedDocuments/Entitlement/" + documentUplod.InternationalStaffEntitlementDocumentGUID + ".xlsx";
        }




        public ActionResult EntitlementDocumentsUpdate(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Access, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            EntitlementDocumentUpdateModel model = DbAHD.dataAHDInternationalStaffEntitlementDocument.Where(x => x.InternationalStaffEntitlementDocumentGUID == PK).Select(f => new EntitlementDocumentUpdateModel
            {

                DocumentTypeGUID = (Guid)f.DocumentTypeGUID,
                InternationalStaffEntitlementGUID = (Guid)f.InternationalStaffEntitlementGUID,

                InternationalStaffEntitlementDocumentGUID = f.InternationalStaffEntitlementDocumentGUID,
                Comments = f.Comments,
                Active = f.Active

            }).FirstOrDefault();
            return PartialView("~/Areas/AHD/Views/InternationalStaffEntitlement/EntitlementDocuments/_DocumentUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EntitlementDocumentsCreate(dataAHDInternationalStaffEntitlementDocument model)
        {
            //if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Create, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            if (!ModelState.IsValid || (model.DocumentTypeGUID == null)) return PartialView("~/Areas/AHD/Views/InternationalStaffEntitlement/EntitlementDocuments/_DocumentUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbAHD.Create(model, Permissions.InternationalStaffEntitlements.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.EntitlementDocumentsDataTable, DbAHD.PrimaryKeyControl(model), DbAHD.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EntitlementDocumentsUpdate(dataAHDInternationalStaffEntitlementDocument model)
        {
            //if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Update, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            if (!ModelState.IsValid || model.DocumentTypeGUID == null) return PartialView("~/Areas/AHD/Views/InternationalStaffEntitlement/EntitlementDocuments/_DocumentUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbAHD.Update(model, Permissions.InternationalStaffEntitlements.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.EntitlementDocumentsDataTable,
                    DbAHD.PrimaryKeyControl(model),
                    DbAHD.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyEntitlementDocuments(model.InternationalStaffEntitlementDocumentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EntitlementDocumentsDelete(dataAHDInternationalStaffEntitlementDocument model)
        {
            //if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Delete, Apps.AHD))
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            List<dataAHDInternationalStaffEntitlementDocument> DeletedLanguages = DeleteEntitlementDocuments(new List<dataAHDInternationalStaffEntitlementDocument> { model });

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleDeleteMessage(DeletedLanguages, DataTableNames.EntitlementDocumentsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyEntitlementDocuments(model.InternationalStaffEntitlementDocumentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EntitlementDocumentsRestore(dataAHDInternationalStaffEntitlementDocument model)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Restore, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            if (ActiveEntitlementDocuments(model))
            {
                return Json(DbAHD.RecordExists());
            }

            List<dataAHDInternationalStaffEntitlementDocument> RestoredLanguages = RestoreStaffDocument(Portal.SingleToList(model));

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleRestoreMessage(RestoredLanguages, DataTableNames.EntitlementDocumentsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyEntitlementDocuments(model.InternationalStaffEntitlementDocumentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult EntitlementDocumentsDataTableDelete(List<dataAHDInternationalStaffEntitlementDocument> models)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Delete, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            List<dataAHDInternationalStaffEntitlementDocument> DeletedLanguages = DeleteEntitlementDocuments(models);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.EntitlementDocumentsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult EntitlementDocumentDataTableModelRestore(List<dataAHDInternationalStaffEntitlementDocument> models)
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Restore, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            List<dataAHDInternationalStaffEntitlementDocument> RestoredLanguages = RestoreStaffDocument(models);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.EntitlementDocumentsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        private List<dataAHDInternationalStaffEntitlementDocument> DeleteEntitlementDocuments(List<dataAHDInternationalStaffEntitlementDocument> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataAHDInternationalStaffEntitlementDocument> DeletedStaffBankAccount = new List<dataAHDInternationalStaffEntitlementDocument>();

            string query = DbAHD.QueryBuilder(models, Permissions.InternationalStaffEntitlements.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbAHD.Database.SqlQuery<dataAHDInternationalStaffEntitlementDocument>(query).ToList();

            foreach (var language in languages)
            {
                DeletedStaffBankAccount.Add(DbAHD.Delete(language, ExecutionTime, Permissions.InternationalStaffEntitlements.DeleteGuid, DbCMS));
            }

            return DeletedStaffBankAccount;
        }

        private List<dataAHDInternationalStaffEntitlementDocument> RestoreStaffDocument(List<dataAHDInternationalStaffEntitlementDocument> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataAHDInternationalStaffEntitlementDocument> RestoredLanguages = new List<dataAHDInternationalStaffEntitlementDocument>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbAHD.QueryBuilder(models, Permissions.InternationalStaffEntitlements.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbAHD.Database.SqlQuery<dataAHDInternationalStaffEntitlementDocument>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveEntitlementDocuments(language))
                {
                    RestoredLanguages.Add(DbAHD.Restore(language, Permissions.InternationalStaffEntitlements.DeleteGuid, Permissions.InternationalStaffEntitlements.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyEntitlementDocuments(Guid PK)
        {
            dataAHDInternationalStaffEntitlementDocument dbModel = new dataAHDInternationalStaffEntitlementDocument();

            var Language = DbAHD.dataAHDInternationalStaffEntitlementDocument.Where(l => l.InternationalStaffEntitlementDocumentGUID == PK).FirstOrDefault();
            var dbLanguage = DbAHD.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataAHDInternationalStaffEntitlementDocumentRowVersion.SequenceEqual(dbModel.dataAHDInternationalStaffEntitlementDocumentRowVersion))
            {
                return Json(DbAHD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbAHD, dbModel, "LanguagesContainer"));
        }

        private bool ActiveEntitlementDocuments(dataAHDInternationalStaffEntitlementDocument model)
        {
            int LanguageID = DbAHD.dataAHDInternationalStaffEntitlementDocument
                                  .Where(x =>
                                              x.InternationalStaffEntitlementGUID == model.InternationalStaffEntitlementGUID &&
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
    }
}