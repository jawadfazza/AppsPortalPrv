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
    public class NationalStaffDangerPayCalcualtionController : AHDBaseController
    {
        public ActionResult TrackAllStaffDangerPaymetns()
        {
            if (!CMS.HasAction(Permissions.NationalStaffDangerPayManagement.Access, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return View("~/Areas/AHD/Views/NationalStaffDangerPay/AllDangerPayments.cshtml");
        }
        public ActionResult TrackNationalStaffAllDangerPayDataTable(DataTableRecievedOptions options)
        {
            //ss
            if (!CMS.HasAction(Permissions.NationalStaffDangerPayManagement.Create, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<NationalStaffDangerPayDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<NationalStaffDangerPayDataTableModel>(DataTable.Filters);
            }
            var _dangers = DbAHD.dataDangerPayInformation.ToList();

            var All = (
               from a in DbAHD.dataNationalStaffDangerPay.AsExpandable().Where(x => x.Active)
               join b in DbAHD.dataDangerPayInformation on a.DangerPayInformationGUID equals b.DangerPayInformationGUID into LJ1
               from R1 in LJ1.DefaultIfEmpty()
               join bb in DbAHD.v_staffCoreDataOverview on a.UserGUID equals bb.UserGUID into LJ4
               from R4 in LJ4.DefaultIfEmpty()
               join c in DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on R4.UserGUID equals c.UserGUID into LJ2
               from R2 in LJ2.DefaultIfEmpty()
               join d in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.FlowStatusGUID equals d.ValueGUID into LJ3
               from R3 in LJ3.DefaultIfEmpty()
               join e in DbAHD.dataDangerPayInformation.Where(x => x.Active) on a.ParentDangerPayInformationGUID equals e.DangerPayInformationGUID into LJ5
               from R5 in LJ5.DefaultIfEmpty()
               select new NationalStaffDangerPayDataTableModel
               {
                   NationalStaffDangerPayGUID = a.NationalStaffDangerPayGUID,
                   PaymentDurationName = R1.PaymentDurationName,
                   PayedIn = R5.PaymentDurationName,
                   StaffName = R2.FirstName + " " + R2.Surname,
                   DangerPayAmount = R1.DangerPayAmount,
                   ResponseDate = a.ResponseDate,
                   ActualDangerPayAmount = a.ActualDangerPayAmount,
                   TotalDaysNotPayable = a.TotalDaysNotPayable,
                   TotalDaysPayable = a.TotalDaysPayable,
                   DangerPaymentConfirmationStatus = R3.ValueDescription,
                   ConfirmationStatusGUID = R3.ValueGUID.ToString(),
                   UserGUID = a.UserGUID.ToString(),


                   dataNationalStaffDangerPayRowVersion = a.dataNationalStaffDangerPayRowVersion
               }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<NationalStaffDangerPayDataTableModel> Result = Mapper.Map<List<NationalStaffDangerPayDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);

        }

        // GET: AHD/NationalStaffDangerPayCalcualtion
        #region Danger Pay By lMonth

        [Route("AHD/DangerPayInformation/")]
        public ActionResult DangerPayInformationIndex()
        {
            if (!CMS.HasAction(Permissions.NationalStaffDangerPayManagement.Access, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return View("~/Areas/AHD/Views/NationalStaffDangerPayInformation/Index.cshtml");
        }
        [Route("AHD/NationalStaffDangerPayManagementDataTable/")]
        public JsonResult NationalStaffDangerPayManagementDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<DangerPayInformationDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<DangerPayInformationDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.NationalStaffDangerPayManagement.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix

            var All = (
                from a in DbAHD.dataDangerPayInformation.Where(x => x.Active).AsExpandable()
                join b in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.FlowStatusGUID equals b.ValueGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()

                select new DangerPayInformationDataTableModel
                {
                    DangerPayInformationGUID = a.DangerPayInformationGUID,
                    YearId = a.YearId,
                    TotalDaysInMonth = a.TotalDaysInMonth,
                    Active = a.Active,
                    FlowStatus = R1.ValueDescription,
                    MonthId = a.MonthId,
                    DangerPayAmount = a.DangerPayAmount,
                    IsLastAction = a.IsLastAction,
                    LastAllowedConfirmationDate = a.LastAllowedConfirmationDate,
                    FlowStatusGUID = a.FlowStatusGUID,
                    PaymentDurationName = a.PaymentDurationName,
                    TotalStaffConfirm = a.dataNationalStaffDangerPay1.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Confirmed).Count(),
                    TotalStaffNotConfirm = a.dataNationalStaffDangerPay1.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Pending).Count(),

                    OrderId = a.OrderId,
                    CreateDate = a.CreateDate,
                    dataDangerPayInformationRowVersion = a.dataDangerPayInformationRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<DangerPayInformationDataTableModel> Result = Mapper.Map<List<DangerPayInformationDataTableModel>>(All.OrderByDescending(x => x.OrderId).Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConfirmationView(string result)
        {
            return View("~/Areas/AHD/Views/Confirmation/Confirm.cshtml");

        }
        public ActionResult NationalStaffDangerPayInformationClosePeriod(Guid id)
        {
            if (!CMS.HasAction(Permissions.NationalStaffDangerPayManagement.Create, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            DateTime ExecutionTime = DateTime.Now;
            var nationalInformation = DbAHD.dataDangerPayInformation.Where(x => x.DangerPayInformationGUID == id).FirstOrDefault();
            nationalInformation.FlowStatusGUID = NationalStaffDangerPaConfirmationStatus.Confirmed;
            var staffDangers = DbAHD.dataNationalStaffDangerPay.Where(x =>
             x.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Confirmed
              && x.IsPayed == false).ToList();
            staffDangers.ForEach(x => x.IsPayed = true);
            DbAHD.UpdateBulk(staffDangers, Permissions.NationalStaffDangerPayManagement.CreateGuid, ExecutionTime, DbCMS);


            DbAHD.Update(nationalInformation, Permissions.NationalStaffDangerPayManagement.UpdateGuid, ExecutionTime, DbCMS);


            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return RedirectToAction("ConfirmationView");

                //return Json(DbAHD.SingleUpdateMessage(DataTableNames.NationalStaffDangerPayManagementDataTable, DbAHD.PrimaryKeyControl(nationalInformation), DbAHD.RowVersionControls(Portal.SingleToList(nationalInformation))));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        //[Route("AHD/NationalStaffDangerPayInformation/Create/")]
        public ActionResult NationalStaffDangerPayInformationCreate()
        {
            if (!CMS.HasAction(Permissions.NationalStaffDangerPayManagement.Create, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/AHD/Views/NationalStaffDangerPayInformation/_NationalStaffDangerPayInformation.cshtml", new DangerPayInformationModel { DangerPayInformationGUID = Guid.Empty });

        }

        //[Route("AHD/NationalStaffDangerPayInformation/Update/{PK}")]
        public ActionResult NationalStaffDangerPayInformationUpdate(Guid PK)
        {
            var model = (from a in DbAHD.dataDangerPayInformation.WherePK(PK)
                         select new DangerPayInformationModel
                         {
                             DangerPayInformationGUID = a.DangerPayInformationGUID,
                             YearId = a.YearId,
                             MonthId = a.MonthId,
                             CreatedByGUID = (Guid)a.CreateByGUID,
                             DangerPayAmount = a.DangerPayAmount,
                             IsLastAction = a.IsLastAction,
                             LastAllowedConfirmationDate = a.LastAllowedConfirmationDate,
                             FlowStatusGUID = a.FlowStatusGUID,
                             PaymentDurationName = a.PaymentDurationName,
                             OrderId = a.OrderId,
                             CreateDate = a.CreateDate,

                             Active = a.Active,
                             //DangerPaymentConfirmationStatus= R1.ValueDescription,

                             dataDangerPayInformationRowVersion = a.dataDangerPayInformationRowVersion
                         }).FirstOrDefault();
            ViewBag.DangerPayInformationGUID = PK;
            ViewBag.TotalStaffNotConfirmed = DbAHD.dataNationalStaffDangerPay.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Pending && a.DangerPayInformationGUID == PK).Count();
            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("NationalStaffDangerPayInformation", "NationalStaffDangerPayInformations", new { Area = "AHD" }));
            return View("~/Areas/AHD/Views/NationalStaffDangerPay/Index.cshtml", model);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NationalStaffDangerPayInformationCreate(DangerPayInformationModel model)
        {
            if (!CMS.HasAction(Permissions.NationalStaffDangerPayManagement.Create, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveNationalStaffDangerPayInformation(model)) return PartialView("~/Areas/AHD/Views/NationalStaffDangerPayInformations/_NationalStaffDangerPayInformationForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            var staffDangers = DbAHD.dataNationalStaffDangerPay.Where(x =>
             x.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Confirmed
              && x.IsPayed == false).ToList();
            staffDangers.ForEach(x => x.IsPayed = true);

            DbAHD.UpdateBulk(staffDangers, Permissions.NationalStaffDangerPayManagement.CreateGuid, ExecutionTime, DbCMS);

            Guid EntityPK = Guid.NewGuid();
            var _maxorderId = DbAHD.dataDangerPayInformation.Where(x => x.Active).Select(x => x.OrderId).Max() ?? 0;
            var priInformaiton = DbAHD.dataDangerPayInformation.Where(x => x.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Pending).ToList();
            priInformaiton.ForEach(f => f.FlowStatusGUID = NationalStaffDangerPaConfirmationStatus.Confirmed);
            DbAHD.UpdateBulk(priInformaiton, Permissions.NationalStaffDangerPayManagement.CreateGuid, ExecutionTime, DbCMS);

            dataDangerPayInformation NationalStaffDangerPayInformation = Mapper.Map(model, new dataDangerPayInformation());

            NationalStaffDangerPayInformation.DangerPayInformationGUID = EntityPK;
            NationalStaffDangerPayInformation.CreateDate = ExecutionTime;
            NationalStaffDangerPayInformation.CreateByGUID = UserGUID;
            NationalStaffDangerPayInformation.OrderId = _maxorderId + 1;
            NationalStaffDangerPayInformation.MonthId = model.MonthStartDate.Value.Month;
            NationalStaffDangerPayInformation.YearId = model.MonthStartDate.Value.Year;
            NationalStaffDangerPayInformation.FlowStatusGUID = NationalStaffDangerPaConfirmationStatus.Pending;
            int lastOrder = DbAHD.dataDangerPayInformation.Select(x => x.OrderId).Max() ?? 0;

            NationalStaffDangerPayInformation.PaymentDurationName = model.MonthStartDate.Value.Year + "-" + model.MonthStartDate.Value.ToString("MMMM");
            NationalStaffDangerPayInformation.TotalDaysInMonth = DateTime.DaysInMonth(model.MonthStartDate.Value.Year, model.MonthStartDate.Value.Month);
            NationalStaffDangerPayInformation.OrderId = DbAHD.dataDangerPayInformation.Select(x => x.OrderId).Max() == null ? 1 : DbAHD.dataDangerPayInformation.Select(x => x.OrderId).Max() + 1;
            DbAHD.Create(NationalStaffDangerPayInformation, Permissions.NationalStaffDangerPayManagement.CreateGuid, ExecutionTime, DbCMS);
            // Guid maksoud = Guid.Parse("8F7EF83F-FD3E-4F8C-8735-8A22D3D61B75");
            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.NationalStaffDangerPayManagementDataTable, ControllerContext, "NationalStaffDangerPayInformationLanguagesFormControls"));
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.NationalStaffDangerPayManagement.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("Create", "NationalStaffDangerPayInformations", new { Area = "AHD" })), Container = "NationalStaffDangerPayInformationDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.NationalStaffDangerPayManagement.Update, Apps.AHD), Container = "NationalStaffDangerPayInformationDetailFormControls" });
            Guid staffStatusInActive = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3611");
            Guid licaContract = Guid.Parse("41651262-08AC-42DC-A691-9BBCE28C95D1");
            Guid unposContract = Guid.Parse("1506F90E-F377-49B6-A8CD-116250246A44");
            Guid reqNational = Guid.Parse("41651262-08AC-42DC-A691-9BBCE28C95D1");
            //Guid ans = Guid.Parse("8E58217C-A2BD-4657-ABFD-021F5F3F0589");
            Guid _eligible = Guid.Parse("2DAC5D96-E6A3-48C1-B3F5-17BFD9F62881");
            var staffs = DbAHD.v_staffCoreDataOverview.Where(x => x.RecruitmentTypeGUID == reqNational
            && x.ContractTypeGUID != unposContract
            && x.PaymentEligibilityStatusGUID == _eligible
            && x.StaffStatusGUID == staffStatusInActive && x.ContractTypeGUID != licaContract).ToList();
            List<dataNationalStaffDangerPay> allStaff = new List<dataNationalStaffDangerPay>();
            var StaffPays = DbAHD.dataNationalStaffDangerPayDetail.Where(x => x.IsLinked == true
              && x.dataNationalStaffDangerPay.MaindataDangerPayInformation.OrderId == lastOrder).ToList();
            var CurrentStaffEligibleForDangerPayGUIDs = StaffPays.Select(x => x.dataNationalStaffDangerPay.UserGUID).ToList();
            List<dataNationalStaffDangerPayDetail> dangerDetails = new List<dataNationalStaffDangerPayDetail>();
            dataNationalStaffDangerPay myStaff = new dataNationalStaffDangerPay();
            foreach (var item in staffs)
            {

                myStaff = new dataNationalStaffDangerPay
                {
                    NationalStaffDangerPayGUID = Guid.NewGuid(),
                    UserGUID = item.UserGUID,
                    DangerPayInformationGUID = EntityPK,
                    IsAnswerd = false,
                    IsPayed = false,
                    FlowStatusGUID = NationalStaffDangerPaConfirmationStatus.Pending

                };
                allStaff.Add(myStaff);

                //zzzz
            }
            DbAHD.CreateBulk(allStaff, Permissions.NationalStaffDangerPayManagement.CreateGuid, ExecutionTime, DbCMS);
            DbAHD.SaveChanges();
            DbCMS.SaveChanges();
            var allStaffDangerPay = DbAHD.dataNationalStaffDangerPay.
                Where(x => x.DangerPayInformationGUID == NationalStaffDangerPayInformation.DangerPayInformationGUID).ToList();
            var currentStaffGUIDs = allStaffDangerPay.
                Where(x => CurrentStaffEligibleForDangerPayGUIDs.Contains(x.UserGUID)).Select(x => x.UserGUID).ToList();
            foreach (var myDanger in allStaffDangerPay)
            {
                var mystaffPays = StaffPays.Where(x =>
                x.dataNationalStaffDangerPay.UserGUID == myDanger.UserGUID
                ).ToList();

                dataNationalStaffDangerPay myStaffDangerPay = allStaffDangerPay.
                    Where(x => x.UserGUID == myDanger.UserGUID
                  ).FirstOrDefault();

                //dataNationalStaffDangerPay myNewStaff = DbAHD.dataNationalStaffDangerPay.Find(myStaffDangerPay.NationalStaffDangerPayGUID);



                foreach (var myPay in mystaffPays)
                {

                    dataNationalStaffDangerPayDetail mypayDetail = new dataNationalStaffDangerPayDetail
                    {
                        NationalStaffDangerPayDetailGUID = Guid.NewGuid(),
                        NationalStaffDangerPayGUID = myStaffDangerPay.NationalStaffDangerPayGUID,
                        LeaveTypeGUID = myPay.LeaveTypeGUID,
                        DepartureDate = myPay.DepartureDate,
                        ReturnDate = myPay.ActualReturnDate,
                        ActualReturnDate = myPay.ActualReturnDate,
                        BaseLineReturnDate = myPay.BaseLineReturnDate,
                        DestinationName = myPay.DestinationName,
                        CountryGUID = myPay.CountryGUID,
                        ReturnToDutyStationGUID = myPay.ReturnToDutyStationGUID,
                        Active = true,
                    };


                    //zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz
                    #region t

                    if (myPay.ActualReturnDate > NationalStaffDangerPayInformation.MonthEndDate)
                    {

                        mypayDetail.IsLinked = true;

                        mypayDetail.ReturnDate = NationalStaffDangerPayInformation.MonthEndDate;
                    }
                    else
                    {
                        mypayDetail.IsLinked = false;
                        mypayDetail.ReturnDate = myPay.ActualReturnDate;
                    }


                    int totalDays = mypayDetail.IsLinked == true ?
                        (myPay.ActualReturnDate - NationalStaffDangerPayInformation.MonthStartDate.Value.AddDays(-1)).Value.Days : ((myPay.ActualReturnDate - NationalStaffDangerPayInformation.MonthStartDate.Value.AddDays(-1))).Value.Days - 1;
                    if (mypayDetail.ReturnToDutyStationGUID == Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3112")
                        && mypayDetail.ReturnDate == NationalStaffDangerPayInformation.MonthEndDate)
                    {
                        if (totalDays == NationalStaffDangerPayInformation.TotalDaysInMonth - 1)
                        {
                            totalDays = Int32.Parse(NationalStaffDangerPayInformation.TotalDaysInMonth.ToString());
                        }
                    }
                    if (totalDays <= 0)
                    {
                        totalDays = 0;
                    }
                    if (myPay.LeaveTypeGUID == NationalStaffLeaveType.Mission && totalDays <= 7)
                    {
                        totalDays = 0;
                    }
                    else if (myPay.LeaveTypeGUID == NationalStaffLeaveType.Mission && totalDays > 7)
                    {
                        totalDays = totalDays - 7;
                    }

                    int? _totalDaysNotPayable = (int?)myDanger.TotalDaysNotPayable ?? 0;
                    myDanger.TotalDaysNotPayable = (_totalDaysNotPayable + totalDays) > 31 ? 31 : (_totalDaysNotPayable + totalDays);


                    myDanger.TotalDaysPayable = (NationalStaffDangerPayInformation.TotalDaysInMonth - myDanger.TotalDaysNotPayable) < 0 ? 0 : (NationalStaffDangerPayInformation.TotalDaysInMonth - myDanger.TotalDaysNotPayable);
                    myDanger.ActualDangerPayAmount = myDanger.TotalDaysPayable == myDanger.MaindataDangerPayInformation.TotalDaysInMonth ? myDanger.MaindataDangerPayInformation.DangerPayAmount < 0 ? 0 : myDanger.MaindataDangerPayInformation.DangerPayAmount : myDanger.TotalDaysPayable * 15 < 0 ? 0 : myDanger.TotalDaysPayable * 15;

                    dangerDetails.Add(mypayDetail);


                    DbAHD.Update(myDanger, Permissions.NationalStaffDangerPayManagement.CreateGuid, ExecutionTime, DbCMS);
                    #endregion


                }
                // DbAHD.UpdateBulk(mystaffPays, Permissions.NationalStaffDangerPayManagement.CreateGuid, ExecutionTime, DbCMS);



            }
            DbAHD.CreateBulk(dangerDetails, Permissions.NationalStaffDangerPayManagement.CreateGuid, ExecutionTime, DbCMS);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                SendConfirmationReceivingModelEmail(EntityPK);
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.NationalStaffDangerPayManagementDataTable, DbAHD.PrimaryKeyControl(NationalStaffDangerPayInformation), DbAHD.RowVersionControls(Portal.SingleToList(NationalStaffDangerPayInformation))));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NationalStaffDangerPayInformationUpdate(DangerPayInformationModel model)
        {
            if (!CMS.HasAction(Permissions.NationalStaffDangerPayManagement.Update, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/AHD/Views/NationalStaffDangerPayInformations/_NationalStaffDangerPayInformationForm.cshtml", model);
            DateTime ExecutionTime = DateTime.Now;
            dataDangerPayInformation NationalStaffDangerPayInformation = Mapper.Map(model, new dataDangerPayInformation());
            DbAHD.Update(NationalStaffDangerPayInformation, Permissions.NationalStaffDangerPayManagement.UpdateGuid, ExecutionTime, DbCMS);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.NationalStaffDangerPayManagementDataTable, DbAHD.PrimaryKeyControl(NationalStaffDangerPayInformation), DbAHD.RowVersionControls(Portal.SingleToList(NationalStaffDangerPayInformation))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyNationalStaffDangerPayInformation(model.DangerPayInformationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NationalStaffDangerPayInformationDelete(dataDangerPayInformation model)
        {
            if (!CMS.HasAction(Permissions.NationalStaffDangerPayManagement.Delete, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataDangerPayInformation> DeletedNationalStaffDangerPayInformation = DeleteNationalStaffDangerPayInformation(Portal.SingleToList(model));
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.NationalStaffDangerPayManagement.Restore, Apps.AHD), Container = "NationalStaffDangerPayInformationFormControls" });
            try
            {
                int CommitedRows = DbAHD.SaveChanges();
                DbAHD.SaveChanges();
                return Json(DbAHD.SingleDeleteMessage(CommitedRows, DeletedNationalStaffDangerPayInformation.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyNationalStaffDangerPayInformation(model.DangerPayInformationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NationalStaffDangerPayInformationRestore(dataDangerPayInformation model)
        {
            if (!CMS.HasAction(Permissions.NationalStaffDangerPayManagement.Restore, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveNationalStaffDangerPayInformation(model))
            {
                return Json(DbAHD.RecordExists());
            }
            List<dataDangerPayInformation> RestoredNationalStaffDangerPayInformation = RestoreNationalStaffDangerPayInformations(Portal.SingleToList(model));
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.NationalStaffDangerPayManagement.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("NationalStaffDangerPayInformationCreate", "Configuration", new { Area = "AHD" })), Container = "NationalStaffDangerPayInformationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.NationalStaffDangerPayManagement.Update, Apps.AHD), Container = "NationalStaffDangerPayInformationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.NationalStaffDangerPayManagement.Delete, Apps.AHD), Container = "NationalStaffDangerPayInformationFormControls" });
            try
            {
                int CommitedRows = DbAHD.SaveChanges();
                DbAHD.SaveChanges();
                return Json(DbAHD.SingleRestoreMessage(CommitedRows, RestoredNationalStaffDangerPayInformation, DbAHD.PrimaryKeyControl(RestoredNationalStaffDangerPayInformation.FirstOrDefault()), Url.Action(DataTableNames.NationalStaffDangerPayManagementDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyNationalStaffDangerPayInformation(model.DangerPayInformationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult NationalStaffDangerPayManagementDataTableDelete(List<dataDangerPayInformation> models)
        {
            if (!CMS.HasAction(Permissions.NationalStaffDangerPayManagement.Delete, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataDangerPayInformation> DeletedNationalStaffDangerPayInformation = DeleteNationalStaffDangerPayInformation(models);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialDeleteMessage(DeletedNationalStaffDangerPayInformation, models, DataTableNames.NationalStaffDangerPayManagementDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult NationalStaffDangerPayManagementDataTableRestore(List<dataDangerPayInformation> models)
        {
            if (!CMS.HasAction(Permissions.NationalStaffDangerPayManagement.Restore, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataDangerPayInformation> RestoredNationalStaffDangerPayInformation = DeleteNationalStaffDangerPayInformation(models);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialRestoreMessage(RestoredNationalStaffDangerPayInformation, models, DataTableNames.NationalStaffDangerPayManagementDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        private List<dataDangerPayInformation> DeleteNationalStaffDangerPayInformation(List<dataDangerPayInformation> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataDangerPayInformation> DeletedNationalStaffDangerPayInformation = new List<dataDangerPayInformation>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT DangerPayInformationGUID,CONVERT(varchar(50), DangerPayInformationGUID) as C2 ,dataDangerPayInformationRowVersion FROM code.dataDangerPayInformation where DangerPayInformationGUID in (" + string.Join(",", models.Select(x => "'" + x.DangerPayInformationGUID + "'").ToArray()) + ")";
            string query = DbAHD.QueryBuilder(models, Permissions.NationalStaffDangerPayManagement.DeleteGuid, SubmitTypes.Delete, "");
            var Records = DbAHD.Database.SqlQuery<dataDangerPayInformation>(query).ToList();
            foreach (var record in Records)
            {
                DeletedNationalStaffDangerPayInformation.Add(DbAHD.Delete(record, ExecutionTime, Permissions.NationalStaffDangerPayManagement.DeleteGuid, DbCMS));
            }
            return DeletedNationalStaffDangerPayInformation;
        }
        private List<dataDangerPayInformation> RestoreNationalStaffDangerPayInformations(List<dataDangerPayInformation> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataDangerPayInformation> RestoredNationalStaffDangerPayInformation = new List<dataDangerPayInformation>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT DangerPayInformationGUID,CONVERT(varchar(50), DangerPayInformationGUID) as C2 ,dataDangerPayInformationRowVersion FROM code.dataDangerPayInformation where DangerPayInformationGUID in (" + string.Join(",", models.Select(x => "'" + x.DangerPayInformationGUID + "'").ToArray()) + ")";

            string query = DbAHD.QueryBuilder(models, Permissions.NationalStaffDangerPayManagement.DeleteGuid, SubmitTypes.Restore, "");

            var Records = DbAHD.Database.SqlQuery<dataDangerPayInformation>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveNationalStaffDangerPayInformation(record))
                {
                    RestoredNationalStaffDangerPayInformation.Add(DbAHD.Restore(record, Permissions.NationalStaffDangerPayManagement.DeleteGuid, Permissions.NationalStaffDangerPayManagement.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredNationalStaffDangerPayInformation;
        }

        private JsonResult ConcurrencyNationalStaffDangerPayInformation(Guid PK)
        {
            DangerPayInformationDataTableModel dbModel = new DangerPayInformationDataTableModel();

            var NationalStaffDangerPayInformation = DbAHD.dataDangerPayInformation.Where(x => x.DangerPayInformationGUID == PK).FirstOrDefault();
            var dbNationalStaffDangerPayInformation = DbAHD.Entry(NationalStaffDangerPayInformation).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbNationalStaffDangerPayInformation, dbModel);

            if (NationalStaffDangerPayInformation.dataDangerPayInformationRowVersion.SequenceEqual(dbModel.dataDangerPayInformationRowVersion))
            {
                return Json(DbAHD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbAHD, dbModel, "LanguagesContainer"));
        }

        private bool ActiveNationalStaffDangerPayInformation(Object model)
        {
            dataDangerPayInformation NationalStaffDangerPayInformation = Mapper.Map(model, new dataDangerPayInformation());
            int ModelDescription = DbAHD.dataDangerPayInformation
                                    .Where(x => x.MonthStartDate == NationalStaffDangerPayInformation.MonthStartDate &&
                                                x.MonthEndDate == NationalStaffDangerPayInformation.MonthEndDate &&
                                                x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("New Danger Pay ", " already exists");
            }
            return (ModelDescription > 0);
        }


        #endregion
        #region Confirm
        public ActionResult NationalStaffDangerPayIndex(Guid id)
        {
            //ss
            if (id != UserGUID)
            {
                throw new HttpException(401, "Unauthorized access");
            }
            ViewBag.StaffGUID = id;
            return View(new NationalStaffDangerPayDataTableModel { StaffGUID = id });

        }

        public ActionResult NationalStaffDangerPayDataTable(DataTableRecievedOptions options, Guid PK)
        {
            //ss
            if (PK != UserGUID)
            {
                throw new HttpException(401, "Unauthorized access");
            }
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<DangerPayInformationDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<DangerPayInformationDataTableModel>(DataTable.Filters);
            }

            var All = (


               from a in DbAHD.dataNationalStaffDangerPay.Where(x => x.UserGUID == PK && x.Active)
               join b in DbAHD.dataDangerPayInformation.AsExpandable() on a.DangerPayInformationGUID equals b.DangerPayInformationGUID into LJ1
               from R1 in LJ1.DefaultIfEmpty()
               join c in DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.UserGUID equals c.UserGUID into LJ2
               from R2 in LJ2.DefaultIfEmpty()
               join d in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on R1.FlowStatusGUID equals d.ValueGUID into LJ3
               from R3 in LJ3.DefaultIfEmpty()
               select new NationalStaffDangerPayDataTableModel
               {
                   NationalStaffDangerPayGUID = a.NationalStaffDangerPayGUID,
                   PaymentDurationName = R1.PaymentDurationName,
                   StaffName = R2.FirstName + " " + R2.Surname,
                   DangerPayAmount = R1.DangerPayAmount,
                   ResponseDate = a.ResponseDate,
                   ActualDangerPayAmount = a.ActualDangerPayAmount,
                   TotalDaysNotPayable = a.TotalDaysNotPayable,
                   TotalDaysPayable = a.TotalDaysPayable,
                   DangerPaymentConfirmationStatus = R3.ValueDescription,
                   dataNationalStaffDangerPayRowVersion = a.dataNationalStaffDangerPayRowVersion
               });

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<NationalStaffDangerPayDataTableModel> Result = Mapper.Map<List<NationalStaffDangerPayDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);

        }


        #endregion
        #region National Staff detail payment
        public ActionResult NationalAbsenceDetailIndex(Guid PK)
        {
            var DangerPay = DbAHD.dataNationalStaffDangerPay.Where(x => x.NationalStaffDangerPayGUID == PK
            ).FirstOrDefault();

            //if(DangerPay.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Confirmed)
            //{
            //    return View("~/Areas/AHD/Views/NationalStaffDangerPayDetail/DangerPayConfirmed.cshtml");
            //}
            ViewBag.TotalRows = DbAHD.dataNationalStaffDangerPayDetail.Where(a => a.NationalStaffDangerPayGUID == PK).Count();

            return PartialView("~/Areas/AHD/Views/NationalStaffDangerPayDetail/_NationalAbsenceDetailIndex.cshtml", new NationalStaffDangerPayDataTableModel { NationalStaffDangerPayGUID = PK, FlowStatusGUID = DangerPay.FlowStatusGUID });
        }


        public ActionResult TrackNationalStaffDangerPayDetailIndex(Guid id)
        {
            var DangerPay = DbAHD.dataNationalStaffDangerPay.Where(x => x.NationalStaffDangerPayGUID == id
            ).FirstOrDefault();
            Guid confirmed = Guid.Parse("A40EC252-622E-4FF1-9EF4-E323C7A3CEC5");
            //var _checkConfimred = DbAHD.dataDangerPayInformation.Where(x => x.FlowStatusGUID == confirmed
            //        ).FirstOrDefault();
            //if (_checkConfimred == null)
            //{
            //    return Json(DbAHD.PermissionError());
            //}
            var myStaffInfo = DbAHD.v_staffCoreDataOverview.Where(x => x.UserGUID == DangerPay.UserGUID).FirstOrDefault();
            var Dangerinformation = DbAHD.dataDangerPayInformation.Where(x => x.DangerPayInformationGUID == DangerPay.DangerPayInformationGUID).FirstOrDefault();

            //if(DangerPay.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Confirmed)
            //{
            //    return View("~/Areas/AHD/Views/NationalStaffDangerPayDetail/DangerPayConfirmed.cshtml");
            NationalStaffDangerPayEditDetailDataTableModel model = new NationalStaffDangerPayEditDetailDataTableModel();
            model.NationalStaffDangerPayGUID = id;
            model.FlowStatusGUID = DangerPay.FlowStatusGUID;
            model.PaymentDurationName = Dangerinformation.PaymentDurationName != null ? Dangerinformation.PaymentDurationName : " ";
            model.StaffName = myStaffInfo.FullName;
            model.ResponseDate = DangerPay.ResponseDate;
            model.TotalDaysNotPayable = DangerPay.TotalDaysNotPayable;
            model.TotalDaysPayable = DangerPay.TotalDaysPayable;
            model.ActualDangerPayAmount = DangerPay.ActualDangerPayAmount;
            model.ActualDangerPayAmount = DangerPay.ActualDangerPayAmount;
            model.Comments = DangerPay.Comments;
            model.IsAnswered = DangerPay.IsAnswerd == null ? false : (bool)DangerPay.IsAnswerd;
            //model.IsPayed = DangerPay.IsPayed == null ? false : (bool)DangerPay.IsPayed;
            //}
            return View("~/Areas/AHD/Views/NationalStaffDangerPayDetail/TrackNationalStaffDangerPayDetailIndex.cshtml", model);
        }
        public ActionResult SaveEditNationalDangerPaymentCreate(NationalStaffDangerPayEditDetailDataTableModel model)
        {
            var _myModel = DbAHD.dataNationalStaffDangerPay.Where(x => x.NationalStaffDangerPayGUID == model.NationalStaffDangerPayGUID
                         ).FirstOrDefault();
            if (model.FlowStatusGUID != Guid.Empty)
            {
                _myModel.TotalDaysPayable = model.TotalDaysPayable;
                _myModel.TotalDaysNotPayable = model.TotalDaysNotPayable;
                //_myModel.IsPayed = model.IsPayed;
                _myModel.FlowStatusGUID = model.FlowStatusGUID;
                _myModel.ResponseDate = model.ResponseDate;
                _myModel.Comments = model.Comments;
                _myModel.UpdateByGUID = UserGUID;
                _myModel.UpdateDate = DateTime.Now;
                _myModel.ActualDangerPayAmount = model.ActualDangerPayAmount;
                DbAHD.UpdateNoAudit(_myModel);
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();

            }

            return Json(DbAHD.SingleUpdateMessage(DataTableNames.NationalStaffDangerPayDetailDataTable, DbAHD.PrimaryKeyControl(_myModel), DbAHD.RowVersionControls(Portal.SingleToList(_myModel))));
        }


        public ActionResult NationalStaffDangerPayDetailIndex(Guid id)
        {
            var _dangerPay = DbAHD.dataNationalStaffDangerPay.Where(x => x.NationalStaffDangerPayGUID == id
              && x.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Pending
            ).FirstOrDefault();
            var myStaffInfo = DbAHD.v_staffCoreDataOverview.Where(x => x.UserGUID == _dangerPay.UserGUID).FirstOrDefault();

            var _dangerPayInformation = DbAHD.dataDangerPayInformation.Where(x => x.DangerPayInformationGUID == _dangerPay.DangerPayInformationGUID).FirstOrDefault();
            if ((_dangerPay == null) || (_dangerPay.UserGUID != UserGUID && !CMS.HasAction(Permissions.NationalStaffDangerPayManagement.Access, Apps.AHD)))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var DangerPayOrder = _dangerPayInformation.OrderId;
            var dangerpayPri = DbAHD.dataNationalStaffDangerPay.Where(x => x.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Pending
            && x.UserGUID == UserGUID
            && x.MaindataDangerPayInformation.DangerPayInformationGUID != _dangerPayInformation.DangerPayInformationGUID).Select(f => f.MaindataDangerPayInformation.OrderId).Max();

            if (dangerpayPri != null && DangerPayOrder > dangerpayPri)
            {
                return View("~/Areas/AHD/Views/NationalStaffDangerPayDetail/NationalStaffDangerPayDetailIndex.cshtml", new NationalStaffDangerPayDataTableModel { NationalStaffDangerPayGUID = id, FlowStatusGUID = _dangerPay.FlowStatusGUID, DangerPrivisous = "Yes" });

            }
            ViewBag.MonthName = ProcessData.GetMonthName(_dangerPayInformation.MonthId) + "/" + _dangerPayInformation.YearId;
            ViewBag.StaffName = myStaffInfo.FullName;

            return View("~/Areas/AHD/Views/NationalStaffDangerPayDetail/NationalStaffDangerPayDetailIndex.cshtml", new NationalStaffDangerPayDataTableModel { NationalStaffDangerPayGUID = id, FlowStatusGUID = _dangerPay.FlowStatusGUID, DangerPrivisous = "No" });
        }
        public JsonResult NationalStaffDangerPayDetailDataTable(DataTableRecievedOptions options, Guid PK)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);
            Expression<Func<NationalStaffDangerPayDetailDataTableModel, bool>> Predicate = p => true;
            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<NationalStaffDangerPayDetailDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.NationalStaffDangerPayManagement.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
            var All = (
                  from a in DbAHD.dataNationalStaffDangerPayDetail.Where(x => x.NationalStaffDangerPayGUID == PK).AsExpandable()
                  join b in DbAHD.dataNationalStaffDangerPay on a.NationalStaffDangerPayGUID equals b.NationalStaffDangerPayGUID into LJ1
                  from R1 in LJ1.DefaultIfEmpty()
                  join c in DbAHD.codeCountriesLanguages.Where(x => x.LanguageID == LAN) on a.CountryGUID equals c.CountryGUID into LJ2
                  from R2 in LJ2.DefaultIfEmpty()
                  join d in DbAHD.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN) on a.LeaveTypeGUID equals d.ValueGUID into LJ3
                  from R3 in LJ3.DefaultIfEmpty()

                  join e in DbAHD.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN) on a.ReturnToDutyStationGUID equals e.ValueGUID into LJ4
                  from R4 in LJ4.DefaultIfEmpty()
                  select new NationalStaffDangerPayDetailDataTableModel
                  {
                      NationalStaffDangerPayDetailGUID = a.NationalStaffDangerPayDetailGUID,
                      NationalStaffDangerPayGUID = R1.NationalStaffDangerPayGUID,
                      DepartureDate = a.DepartureDate,
                      Active = a.Active,
                      ReturnDate = a.ReturnDate,
                      Comments = a.Comments,


                      ReturnToDutyStation = R4.ValueDescription,
                      ActualReturnDate = a.ActualReturnDate,
                      Country = R2.CountryDescription,
                      DestinationName = a.DestinationName,
                      LeaveType = R3.ValueDescription,
                      LeaveTypeGUID = a.LeaveTypeGUID.ToString(),


                      dataNationalStaffDangerPayDetailRowVersion = a.dataNationalStaffDangerPayDetailRowVersion
                  }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<NationalStaffDangerPayDetailDataTableModel> Result = Mapper.Map<List<NationalStaffDangerPayDetailDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult NationalStaffDangerPayDetailReset(Guid FK)
        {

            var myResult = DbAHD.dataNationalStaffDangerPay.Where(x => x.NationalStaffDangerPayGUID == FK).FirstOrDefault();
            if (myResult.IsPayed == false)
            {
                var allDetail = DbAHD.dataNationalStaffDangerPayDetail.Where(x => x.NationalStaffDangerPayGUID == FK).ToList();
                DbAHD.dataNationalStaffDangerPayDetail.RemoveRange(allDetail);
                myResult.FlowStatusGUID = NationalStaffDangerPaConfirmationStatus.Pending;
                myResult.ParentDangerPayInformationGUID = null;
                myResult.IsAnswerd = false;
                myResult.IsPayed = false;
                myResult.TotalDaysPayable = 0;
                myResult.TotalDaysNotPayable = 0;
                myResult.ActualDangerPayAmount = 0;
                myResult.ResponseDate = null;
                myResult.UpdateByGUID = UserGUID;
                myResult.UpdateDate = DateTime.Now;

                DbAHD.SaveChanges();

                return PartialView("~/Areas/AHD/Views/NationalStaffDangerPay/_Confirmed.cshtml");
            }
            else
            {
                return Json(DbAHD.PermissionError());
            }

        }

        public ActionResult NationalStaffDangerPayDetailCreate(Guid FK)
        {

            return PartialView("~/Areas/AHD/Views/NationalStaffDangerPayDetail/_NationalStaffDangerPayDetailModel.cshtml",
                new NationalStaffDangerPayDetailModel { NationalStaffDangerPayGUID = FK });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NationalStaffDangerPayDetailCreate(NationalStaffDangerPayDetailModel model)
        {

            if (!ModelState.IsValid) return PartialView("~/Areas/AHD/Views/NationalStaffDangerPayDetail/_NationalStaffDangerPayDetailModel.cshtml", model);
            var reservation = DbAHD.dataNationalStaffDangerPayDetail.Where(x => x.Active
                 && x.NationalStaffDangerPayGUID == model.NationalStaffDangerPayGUID

                 && ((x.DepartureDate >= model.DepartureDate
                      && x.ReturnDate <= model.ReturnDate
                      ))).FirstOrDefault();
            //var CheckStaffDanger=DbAHD.dataNationalStaffDangerPay.Where(x => x.NationalStaffDangerPayGUID == model.NationalStaffDangerPayGUID).FirstOrDefault();
            var naitonalStaff = DbAHD.dataNationalStaffDangerPay.Where(x => x.NationalStaffDangerPayGUID == model.NationalStaffDangerPayGUID).FirstOrDefault();

            if (model.LeaveTypeGUID == null || model.DepartureDate > naitonalStaff.MaindataDangerPayInformation.MonthEndDate || model.DepartureDate < naitonalStaff.MaindataDangerPayInformation.MonthStartDate || reservation != null || model.DepartureDate == null || model.ReturnDate == null || model.CountryGUID == null || model.DepartureDate > model.ReturnDate)
            {
                if (model.LeaveTypeGUID == null)
                {
                    ModelState.AddModelError("Error: ", "Kindly choose your leave type  ");

                }
                if (model.DepartureDate > naitonalStaff.MaindataDangerPayInformation.MonthEndDate || model.DepartureDate < naitonalStaff.MaindataDangerPayInformation.MonthStartDate)
                {
                    ModelState.AddModelError("Error: ", "You have to chose travel dates in the following month: " + ProcessData.GetMonthName(naitonalStaff.MaindataDangerPayInformation.MonthId));
                }
                if (reservation != null)
                {
                    ModelState.AddModelError("Error: ", "There are already travel dates in same period  ");
                }
                if (model.DepartureDate == null || model.ReturnDate == null)
                {
                    ModelState.AddModelError("Error: ", "Departure date and return date must has value ");
                }
                else if (model.CountryGUID == null)
                {
                    ModelState.AddModelError("Error: ", "Country must has value ");
                }
                else if (model.DepartureDate > model.ReturnDate)
                {
                    ModelState.AddModelError("Error: ", "Departure date must be smaller than return date  ");
                }

                return PartialView("~/Areas/AHD/Views/NationalStaffDangerPayDetail/_NationalStaffDangerPayDetailModel.cshtml", model);
            }
            DateTime ExecutionTime = DateTime.Now;
            dataNationalStaffDangerPayDetail newPayDetail = new dataNationalStaffDangerPayDetail();

            newPayDetail.ActualReturnDate = model.ReturnDate;
            if (model.ReturnDate > naitonalStaff.MaindataDangerPayInformation.MonthEndDate)
            {

                newPayDetail.IsLinked = true;

                newPayDetail.ReturnDate = naitonalStaff.MaindataDangerPayInformation.MonthEndDate;
            }
            else
            {
                newPayDetail.IsLinked = false;
                newPayDetail.ReturnDate = model.ReturnDate;
            }


            newPayDetail.NationalStaffDangerPayDetailGUID = Guid.NewGuid();
            newPayDetail.NationalStaffDangerPayGUID = model.NationalStaffDangerPayGUID;
            newPayDetail.LeaveTypeGUID = model.LeaveTypeGUID;
            newPayDetail.DepartureDate = model.DepartureDate;
            //newPayDetail.ReturnDate = model.ReturnDate;
            newPayDetail.CountryGUID = model.CountryGUID;
            newPayDetail.DestinationName = model.DestinationName;
            DbAHD.CreateNoAudit(newPayDetail);
            dataNationalStaffDangerPay staffDangerPay = DbAHD.dataNationalStaffDangerPay.Where(x => x.NationalStaffDangerPayGUID == model.NationalStaffDangerPayGUID).FirstOrDefault();

            staffDangerPay.FlowStatusGUID = NationalStaffDangerPaConfirmationStatus.Confirmed;
            staffDangerPay.ResponseDate = ExecutionTime;

            int totalDays = newPayDetail.IsLinked == true ? (newPayDetail.ReturnDate - model.DepartureDate).Value.Days : (model.ReturnDate - model.DepartureDate).Value.Days - 1;
            if (totalDays <= 0)
            {
                totalDays = 0;
            }
            //checke parent danger pay and check mission 
            if (model.LeaveTypeGUID == NationalStaffLeaveType.Mission && totalDays <= 7)
            {
                totalDays = 0;
            }
            else if (model.LeaveTypeGUID == NationalStaffLeaveType.Mission && totalDays > 7)
            {
                totalDays = totalDays - 7;
            }
            var lastDangerPayOrder = DbAHD.dataDangerPayInformation.Select(x => x.OrderId).Max();
            var myDanger = DbAHD.dataDangerPayInformation.Where(x => x.OrderId == lastDangerPayOrder).FirstOrDefault();
            if (staffDangerPay.IsPayed != true && staffDangerPay.DangerPayInformationGUID != myDanger.DangerPayInformationGUID)
            {
                staffDangerPay.ParentDangerPayInformationGUID = myDanger.DangerPayInformationGUID;
            }

            int? _totalpay = (int?)staffDangerPay.TotalDaysNotPayable ?? 0;
            staffDangerPay.TotalDaysNotPayable = _totalpay + totalDays > 31 ? 31 : _totalpay + totalDays;

            staffDangerPay.TotalDaysPayable = (staffDangerPay.MaindataDangerPayInformation.TotalDaysInMonth - staffDangerPay.TotalDaysNotPayable) < 0 ? 0 : (staffDangerPay.MaindataDangerPayInformation.TotalDaysInMonth - staffDangerPay.TotalDaysNotPayable);
            staffDangerPay.ActualDangerPayAmount = staffDangerPay.TotalDaysPayable == staffDangerPay.MaindataDangerPayInformation.TotalDaysInMonth ? (staffDangerPay.MaindataDangerPayInformation.DangerPayAmount) < 0 ? 0 : (staffDangerPay.MaindataDangerPayInformation.DangerPayAmount) : staffDangerPay.TotalDaysPayable * 15 < 0 ? 0 : staffDangerPay.TotalDaysPayable * 15;
            staffDangerPay.IsAnswerd = true;




            DbAHD.UpdateNoAudit(staffDangerPay);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.NationalStaffDangerPayDetailDataTable, DbAHD.PrimaryKeyControl(newPayDetail), DbAHD.RowVersionControls(Portal.SingleToList(newPayDetail))));
                //return Json(DbAHD.SingleUpdateMessage(DataTableNames.MissionTempActionsDataTable, DbAHD.PrimaryKeyControl(tempMission), DbAHD.RowVersionControls(Portal.SingleToList(tempMission))));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        #endregion
        #region NationalStaffDangerPay
        [Route("AHD/NationalStaffAllDangerPayDataTable/{PK}")]
        public ActionResult NationalStaffAllDangerPayDataTable(DataTableRecievedOptions options, Guid PK)
        {
            //ss
            if (!CMS.HasAction(Permissions.NationalStaffDangerPayManagement.Create, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<NationalStaffDangerPayDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<NationalStaffDangerPayDataTableModel>(DataTable.Filters);
            }

            var All = (
               from a in DbAHD.dataNationalStaffDangerPay.AsExpandable().Where(x => x.DangerPayInformationGUID == PK && x.Active)
               join b in DbAHD.dataDangerPayInformation on a.DangerPayInformationGUID equals b.DangerPayInformationGUID into LJ1
               from R1 in LJ1.DefaultIfEmpty()
               join bb in DbAHD.v_staffCoreDataOverview on a.UserGUID equals bb.UserGUID into LJ4
               from R4 in LJ4.DefaultIfEmpty()
               join c in DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on R4.UserGUID equals c.UserGUID into LJ2
               from R2 in LJ2.DefaultIfEmpty()
               join d in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.FlowStatusGUID equals d.ValueGUID into LJ3
               from R3 in LJ3.DefaultIfEmpty()
               select new NationalStaffDangerPayDataTableModel
               {
                   NationalStaffDangerPayGUID = a.NationalStaffDangerPayGUID,
                   PaymentDurationName = R1.PaymentDurationName,
                   StaffName = R2.FirstName + " " + R2.Surname,
                   DangerPayAmount = R1.DangerPayAmount,
                   ResponseDate = a.ResponseDate,
                   ActualDangerPayAmount = a.ActualDangerPayAmount,
                   TotalDaysNotPayable = a.TotalDaysNotPayable,
                   TotalDaysPayable = a.TotalDaysPayable,
                   DangerPaymentConfirmationStatus = R3.ValueDescription,
                   ConfirmationStatusGUID = R3.ValueGUID.ToString(),
                   UserGUID = a.UserGUID.ToString(),


                   dataNationalStaffDangerPayRowVersion = a.dataNationalStaffDangerPayRowVersion
               }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<NationalStaffDangerPayDataTableModel> Result = Mapper.Map<List<NationalStaffDangerPayDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);

        }

        public ActionResult CycleSalaryNationalStaffAllDangerPayDataTable(DataTableRecievedOptions options, Guid PK)
        {
            //ss
            if (!CMS.HasAction(Permissions.SalaryCycle.Create, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<NationalStaffDangerPayDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<NationalStaffDangerPayDataTableModel>(DataTable.Filters);
            }
            var _cycleSalary = DbAHD.dataSalaryCycle.Where(x => x.SalaryCycleGUID == PK).FirstOrDefault();
            var _codeMonth = DbAHD.codeMonth.Where(x => x.MonthCode == _cycleSalary.MonthName).FirstOrDefault();
            var _dangerPay = DbAHD.dataDangerPayInformation.Where(x => x.MonthId == _codeMonth.MonthId && x.YearId == _cycleSalary.Year).FirstOrDefault();
            var All = (
               from a in DbAHD.dataNationalStaffDangerPay.AsExpandable().Where(x => x.DangerPayInformationGUID == _dangerPay.DangerPayInformationGUID && x.Active)
               join b in DbAHD.dataDangerPayInformation on a.DangerPayInformationGUID equals b.DangerPayInformationGUID into LJ1
               from R1 in LJ1.DefaultIfEmpty()
               join bb in DbAHD.v_staffCoreDataOverview on a.UserGUID equals bb.UserGUID into LJ4
               from R4 in LJ4.DefaultIfEmpty()
               join c in DbAHD.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on R4.UserGUID equals c.UserGUID into LJ2
               from R2 in LJ2.DefaultIfEmpty()
               join d in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.FlowStatusGUID equals d.ValueGUID into LJ3
               from R3 in LJ3.DefaultIfEmpty()
               select new NationalStaffDangerPayDataTableModel
               {
                   NationalStaffDangerPayGUID = a.NationalStaffDangerPayGUID,
                   PaymentDurationName = R1.PaymentDurationName,
                   StaffName = R2.FirstName + " " + R2.Surname,
                   DangerPayAmount = R1.DangerPayAmount,
                   ResponseDate = a.ResponseDate,
                   ActualDangerPayAmount = a.ActualDangerPayAmount,
                   TotalDaysNotPayable = a.TotalDaysNotPayable,
                   TotalDaysPayable = a.TotalDaysPayable,
                   DangerPaymentConfirmationStatus = R3.ValueDescription,
                   ConfirmationStatusGUID = R3.ValueGUID.ToString(),
                   UserGUID = a.UserGUID.ToString(),


                   dataNationalStaffDangerPayRowVersion = a.dataNationalStaffDangerPayRowVersion
               }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<NationalStaffDangerPayDataTableModel> Result = Mapper.Map<List<NationalStaffDangerPayDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);

        }

        #endregion
        #region Mails

        public ActionResult SendConfirmationReceivingModelForPendingConfirmationStaffEmail(Guid FK)
        {

            string URL = "";
            string Anchor = "";
            string Link = "";
            var myDangerPay = DbAHD.dataDangerPayInformation.Where(x => x.DangerPayInformationGUID == FK).FirstOrDefault();
            var staffGUIDs = DbAHD.dataNationalStaffDangerPay.Where(x => x.DangerPayInformationGUID == FK
            && x.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Pending).Select(x => x.UserGUID).ToList();

            var allUsers = DbAHD.userPersonalDetailsLanguage.Where(x => staffGUIDs.Contains(x.UserGUID)
                                                                          && x.LanguageID == LAN).ToList();
            var alluserAccounts = DbAHD.userServiceHistory.Where(x => staffGUIDs.Contains(x.UserGUID)).ToList();
            var allDan = DbAHD.dataNationalStaffDangerPay.Where(x => x.DangerPayInformationGUID == FK).ToList();

            string SubjectMessage = resxEmails.DangerPayStaffConfirmationSubject.Replace("$Month", myDangerPay.MonthStartDate.Value.ToString("MMMM")).Replace("$Year", myDangerPay.YearId.ToString());
            string MonthName = ProcessData.GetMonthName(myDangerPay.MonthId) + "  " + myDangerPay.YearId;

            for (int i = 0; i < allUsers.Count(); i += 30)
            {
                var target = allUsers.Skip(i).Take(30);

                foreach (var user in target)
                {
                    //{
                    var currentDanger = allDan.Where(x => x.UserGUID == user.UserGUID).FirstOrDefault();
                    URL = "https://prv.unhcrsyria.org/" + "/AHD/NationalStaffDangerPayCalcualtion/TrackStaffDangerPayments/?id=" + new Portal().GUIDToString(currentDanger.NationalStaffDangerPayGUID);
                    //URL = AppSettingsKeys.Domain + "/AHD/NationalStaffDangerPayCalcualtion/TrackStaffDangerPayments/?id=" + new Portal().GUIDToString(currentDanger.NationalStaffDangerPayGUID);
                    Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmDangerPayReceiving + "</a>";
                    Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                    string myFirstName = user.FirstName;
                    string mySurName = user.Surname;


                    string _message = resxEmails.DangerPayStaffConfirmation
                        .Replace("$FullName", user.FirstName + " " + user.Surname)
                        .Replace("$VerifyLink", Anchor)
                        .Replace("$MonthYear", MonthName)
                        .Replace("$Deadline", myDangerPay.LastAllowedConfirmationDate.Value.ToLongDateString())
                        .Replace("$Year", myDangerPay.YearId.ToString());
                    if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                    int isRec = 1;
                    var myEmail = alluserAccounts.Where(x => x.UserGUID == user.UserGUID).Select(x => x.EmailAddress).FirstOrDefault();

                    Send(myEmail, SubjectMessage, _message, isRec, null);
                }
            }
            return RedirectToAction("ConfirmationView");


        }
        public void SendConfirmationFoStaffHasLeavesAfterSubmission(Guid nationalStaffDangerPayGUID)
        {
            string URL = "";
            string Anchor = "";
            string Link = "";

            var currNationalStaffDangerPay = DbAHD.dataNationalStaffDangerPay.Where(x => x.NationalStaffDangerPayGUID == nationalStaffDangerPayGUID).FirstOrDefault();
            var staffGUID = currNationalStaffDangerPay.UserGUID;

            var myUser = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == staffGUID
                                                                          && x.LanguageID == LAN).FirstOrDefault();
            var alluserAccounts = DbAHD.userServiceHistory.Where(x => x.UserGUID == staffGUID).FirstOrDefault();

            var allDan = DbAHD.dataNationalStaffDangerPay.Where(x => x.DangerPayInformationGUID == currNationalStaffDangerPay.DangerPayInformationGUID).FirstOrDefault();

            string SubjectMessage = resxEmails.NationStaffDangerPaySubmissionConfirmtion.Replace("$Month", currNationalStaffDangerPay.MaindataDangerPayInformation.MonthStartDate.Value.ToString("MMMM")).Replace("$Year", currNationalStaffDangerPay.MaindataDangerPayInformation.YearId.ToString());
            string MonthName = ProcessData.GetMonthName(currNationalStaffDangerPay.MaindataDangerPayInformation.MonthId) + "  " + currNationalStaffDangerPay.MaindataDangerPayInformation.YearId;




            string myFirstName = myUser.FirstName;
            string mySurName = myUser.Surname;

            var myPayment = DbAHD.v_NationalStaffDangerPayment.Where(x => x.NationalStaffDangerPayGUID == nationalStaffDangerPayGUID
                   ).FirstOrDefault();
            var mypriPayment = DbAHD.dataNationalStaffDangerPay.Where(x => x.IsPayed == false
             && x.DangerPayInformationGUID != currNationalStaffDangerPay.DangerPayInformationGUID
            && x.UserGUID == currNationalStaffDangerPay.UserGUID
            ).ToList();

            string table = "<br><table class='table table-bordered' style='border-collapse: collapse; width: 900px'><tr><th style='border: 1px solid  #333; vertical-align:middle'>Number of days DP is not payable</th><th style='border: 1px solid  #333; vertical-align: middle'>Number of days DP is  payable</th><th style='border: 1px solid  #333; vertical-align: middle'>Current danger pay amount</th><th style='border: 1px solid  #333; vertical-align: middle'>Privious month(s) danger pay amount</th><th style='border: 1px solid  #333; vertical-align: middle'>Mission Outside Recorded</th><th style='border: 1px solid  #333; vertical-align: middle'>Annual Recorded </th><th style='border: 1px solid  #333; vertical-align: middle'>Maternity Paternity Recorded </th><th style='border: 1px solid  #333; vertical-align: middle'>Sick Recorded </th><th style='border: 1px solid  #333; vertical-align: middle'>Weekends Recorded </th><th style='border: 1px solid  #333; vertical-align: middle'> Missions Inside Syria</th><th style='border: 1px solid  #333; vertical-align: middle'>Telecommuting out side duty station</th><th style='border: 1px solid  #333; vertical-align: middle'>SLWOP</th><th style='border: 1px solid  #333; vertical-align: middle'>Comments</th></tr>";

            table += "<tr><td style='border: 1px solid  #333; vertical-align: middle'>" + myPayment.TotalDaysNotPayable + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + myPayment.TotalDaysPayable + "</td><td style='border: 1px solid  #333; vertical-align: middle'> " + myPayment.ActualDangerPayAmount + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + mypriPayment.Select(x => x.ActualDangerPayAmount).Sum() + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + myPayment.Mission_Leaves + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + myPayment.Annual_Leaves + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + myPayment.MaternityPaternity_Leaves + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + myPayment.Sick_Leaves + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + myPayment.Weekends_Leaves + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + myPayment.Mission_Inisde + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + myPayment.Telecommuting_Out_Side_Duty_Station + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + myPayment.SLWOP + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + myPayment.Comments + "</td></tr>";
            table += "</table>";
            string _message = resxEmails.DangerPaySuccessFullySubmissionWithDates
                .Replace("$FullName", myFirstName + " " + mySurName)

                .Replace("$MonthName", MonthName)
                .Replace("$table", table)

                .Replace("$Year", currNationalStaffDangerPay.MaindataDangerPayInformation.YearId.ToString());
            if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
            int isRec = 1;
            var myEmail = alluserAccounts.EmailAddress;
            Guid mahmoudGUID = Guid.Parse("8F7EF83F-FD3E-4F8C-8735-8A22D3D61B75");
            Guid mousaGUID = Guid.Parse("1CE077EE-076C-432C-A1F4-963C07210419");
            Guid _admindeptGuid = Guid.Parse("4778E122-8E60-4ACE-866E-327A505F7B5A");
            var _staffcore = DbAHD.StaffCoreData.Where(x => x.UserGUID == staffGUID).FirstOrDefault();
            var focalpointsGUIDs = DbCMS.v_currentUserPermissions.Where(x => x.UserGUID != mousaGUID && x.Active && x.ActionGUID == Permissions.NationalDangerPaymentFocalPointAccess.AccessGuid).Select(x => x.UserGUID).Distinct().ToList();

            var _foalAccessGUIDs = DbAHD.v_StaffProfileInformation.Where(x => focalpointsGUIDs.Contains(x.UserGUID)
            && x.DepartmentGUID == _admindeptGuid
                            && x.UserGUID != mousaGUID
                        && x.DutyStationGUID == _staffcore.DutyStationGUID).Select(x => x.UserGUID).Distinct().ToList();
            var otheruser = DbAHD.userServiceHistory.Where(x => (x.UserGUID == mousaGUID
           || x.UserGUID == mahmoudGUID || _foalAccessGUIDs.Contains(x.UserGUID)
            )).Select(x => x.EmailAddress).ToList();
            string copyEmails = string.Join(" ;", otheruser);

            Send(myEmail, SubjectMessage, _message, isRec, copyEmails);



        }

        public void SendConfirmationFoStaffAfterSubmission(Guid nationalStaffDangerPayGUID)
        {
            string URL = "";
            string Anchor = "";
            string Link = "";

            var currNationalStaffDangerPay = DbAHD.dataNationalStaffDangerPay.Where(x => x.NationalStaffDangerPayGUID == nationalStaffDangerPayGUID).FirstOrDefault();
            var staffGUID = currNationalStaffDangerPay.UserGUID;

            var myUser = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == staffGUID
                                                                          && x.LanguageID == LAN).FirstOrDefault();
            var alluserAccounts = DbAHD.userServiceHistory.Where(x => x.UserGUID == staffGUID).FirstOrDefault();
            var allDan = DbAHD.dataNationalStaffDangerPay.Where(x => x.DangerPayInformationGUID == currNationalStaffDangerPay.DangerPayInformationGUID).FirstOrDefault();

            string SubjectMessage = resxEmails.NationStaffDangerPaySubmissionConfirmtion.Replace("$Month", currNationalStaffDangerPay.MaindataDangerPayInformation.MonthStartDate.Value.ToString("MMMM")).Replace("$Year", currNationalStaffDangerPay.MaindataDangerPayInformation.YearId.ToString());
            string MonthName = ProcessData.GetMonthName(currNationalStaffDangerPay.MaindataDangerPayInformation.MonthId) + "  " + currNationalStaffDangerPay.MaindataDangerPayInformation.YearId;




            string myFirstName = myUser.FirstName;
            string mySurName = myUser.Surname;


            string _message = resxEmails.DangerPaySuccessFullySubmission
                .Replace("$FullName", myFirstName + " " + mySurName)

                .Replace("$MonthName", MonthName)

                .Replace("$Year", currNationalStaffDangerPay.MaindataDangerPayInformation.YearId.ToString());
            if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
            int isRec = 1;
            var myEmail = alluserAccounts.EmailAddress;
            //Guid mahmoudGUID = Guid.Parse("8F7EF83F-FD3E-4F8C-8735-8A22D3D61B75");
            Guid mousaGUID = Guid.Parse("1CE077EE-076C-432C-A1F4-963C07210419");
            Guid _admindeptGuid = Guid.Parse("4778E122-8E60-4ACE-866E-327A505F7B5A");

            var _staffcore = DbAHD.StaffCoreData.Where(x => x.UserGUID == staffGUID).FirstOrDefault();
            var focalpointsGUIDs = DbCMS.v_currentUserPermissions.Where(x => x.UserGUID != mousaGUID && x.Active && x.ActionGUID == Permissions.NationalDangerPaymentFocalPointAccess.AccessGuid).Select(x => x.UserGUID).Distinct().ToList();

            var _foalAccessGUIDs = DbAHD.v_StaffProfileInformation.Where(x => focalpointsGUIDs.Contains(x.UserGUID)
                            && x.UserGUID != mousaGUID
                            && x.DepartmentGUID == _admindeptGuid
                        && x.DutyStationGUID == _staffcore.DutyStationGUID).Select(x => x.UserGUID).Distinct().ToList();

            var otheruser = DbAHD.userServiceHistory.Where(x => (x.UserGUID == mousaGUID
            // || x.UserGUID == mahmoudGUID
            || _foalAccessGUIDs.Contains(x.UserGUID)
            )).Select(x => x.EmailAddress).ToList();
            string copyEmails = string.Join(" ;", otheruser);

            Send(myEmail, SubjectMessage, _message, isRec, copyEmails);



        }
        public void SendConfirmationReceivingModelEmail(Guid dangerPayInformationGUID)
        {
            string URL = "";
            string Anchor = "";
            string Link = "";
            var myDangerPay = DbAHD.dataDangerPayInformation.Where(x => x.DangerPayInformationGUID == dangerPayInformationGUID).FirstOrDefault();
            var staffGUIDs = DbAHD.dataNationalStaffDangerPay.Where(x => x.DangerPayInformationGUID == dangerPayInformationGUID).Select(x => x.UserGUID).ToList();

            var allUsers = DbAHD.userPersonalDetailsLanguage.Where(x => staffGUIDs.Contains(x.UserGUID)
                                                                          && x.LanguageID == LAN).ToList();
            var alluserAccounts = DbAHD.userServiceHistory.Where(x => staffGUIDs.Contains(x.UserGUID)).ToList();
            var allDan = DbAHD.dataNationalStaffDangerPay.Where(x => x.DangerPayInformationGUID == dangerPayInformationGUID).ToList();

            string SubjectMessage = resxEmails.DangerPayStaffConfirmationSubject.Replace("$Month", myDangerPay.MonthStartDate.Value.ToString("MMMM")).Replace("$Year", myDangerPay.YearId.ToString());
            string MonthName = ProcessData.GetMonthName(myDangerPay.MonthId) + "  " + myDangerPay.YearId;

            for (int i = 0; i < allUsers.Count(); i += 30)
            {
                var target = allUsers.Skip(i).Take(30);

                foreach (var user in target)
                {

                    var currentDanger = allDan.Where(x => x.UserGUID == user.UserGUID).FirstOrDefault();
                    URL = AppSettingsKeys.Domain + "/AHD/NationalStaffDangerPayCalcualtion/TrackStaffDangerPayments/?id=" + new Portal().GUIDToString(currentDanger.NationalStaffDangerPayGUID);
                    Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmDangerPayReceiving + "</a>";
                    Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                    string myFirstName = user.FirstName;
                    string mySurName = user.Surname;


                    string _message = resxEmails.DangerPayStaffConfirmation
                        .Replace("$FullName", user.FirstName + " " + user.Surname)
                        .Replace("$VerifyLink", Anchor)
                        .Replace("$MonthYear", MonthName)
                        .Replace("$Deadline", myDangerPay.LastAllowedConfirmationDate.Value.ToLongDateString())
                        .Replace("$Year", myDangerPay.YearId.ToString());
                    if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                    int isRec = 1;
                    var myEmail = alluserAccounts.Where(x => x.UserGUID == user.UserGUID).Select(x => x.EmailAddress).FirstOrDefault();
                    Send(myEmail, SubjectMessage, _message, isRec, null);
                }
            }


        }
        public void Send(string recipients, string subject, string body, int? isRec, string copy_recipients)
        {

            string blind_copy_recipients = null;
            string body_format = "HTML";
            string importance = "Normal";
            string file_attachments = null;
            string _body = "<div style='font-family:Arial;'>" + body.Replace("\r\n", "<br/>") + "</div>";
            if (LAN == "AR") { _body = "<p align='right'>" + _body + "</p>"; }
           // DbAHD.SendEmailHR("maksoud@unhcr.org", "", "", subject, _body, body_format, importance, file_attachments);
            DbCMS.SendEmailHR(recipients, copy_recipients, blind_copy_recipients, subject, _body, body_format, importance, file_attachments);
        }
        #endregion

        #region Confirm no date out 
        public JsonResult ConfirmStaffDangerPayDetailNoLeave(Guid guid)
        {


            var model = DbAHD.dataNationalStaffDangerPay.Where(x => x.NationalStaffDangerPayGUID == guid).FirstOrDefault();
            var _dangerPay = DbAHD.dataDangerPayInformation.Where(x => x.DangerPayInformationGUID == model.DangerPayInformationGUID).FirstOrDefault();
            Guid _pending = Guid.Parse("A40EC252-622E-4FF1-9EF4-E323C7A3CEC5");
            var _status = DbAHD.dataDangerPayInformation.Where(x => x.FlowStatusGUID == _pending).FirstOrDefault();
            if (_status == null)
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
            }

            if (model != null && model.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Confirmed)
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
            }
            DateTime ExecutionTime = DateTime.Now;


            model.FlowStatusGUID = NationalStaffDangerPaConfirmationStatus.Confirmed;
            model.IsAnswerd = true;
            model.ResponseDate = ExecutionTime;
            var detail = DbAHD.dataNationalStaffDangerPayDetail.Where(x => x.NationalStaffDangerPayGUID == guid).ToList();
            model.TotalDaysPayable = model.MaindataDangerPayInformation.TotalDaysInMonth - model.TotalDaysNotPayable ?? 0;
            if (detail.Count == 0)
            {
                model.TotalDaysNotPayable = 0;
            }
            if (model.TotalDaysNotPayable > 0)
            {
                model.ActualDangerPayAmount = model.TotalDaysPayable * 15;
            }
            else
            {



                model.ActualDangerPayAmount = model.MaindataDangerPayInformation.DangerPayAmount;
            }

            //model.TotalDaysPayable = model.MaindataDangerPayInformation.TotalDaysInMonth- model.TotalDaysNotPayable??0;
            var lastDangerPayOrder = DbAHD.dataDangerPayInformation.Select(x => x.OrderId).Max();
            // var staffDangerPay = DbAHD.dataNationalStaffDangerPay.Find(nationalStaffDangerPayGUID);
            var _dangerInformaiton = DbAHD.dataDangerPayInformation.
              Where(x => x.DangerPayInformationGUID == model.DangerPayInformationGUID).FirstOrDefault();
            var orderNext = DbAHD.dataDangerPayInformation.Select(x => x.OrderId).Max();

            var myDanger = DbAHD.dataDangerPayInformation.Where(x => x.OrderId == orderNext).FirstOrDefault();


            if (myDanger != null && model.IsPayed != true && orderNext != null &&
                model.DangerPayInformationGUID != myDanger.DangerPayInformationGUID)
            {
                model.ParentDangerPayInformationGUID = myDanger.DangerPayInformationGUID;
            }

            DbAHD.Update(model, Permissions.NationalStaffDangerPayManagement.UpdateGuid, ExecutionTime, DbCMS);
            //zzz
            SendConfirmationFoStaffAfterSubmission(model.NationalStaffDangerPayGUID);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
                //return Json(DbAHD.SingleUpdateMessage(DataTableNames.NationalStaffDangerPayManagementDataTable, DbAHD.PrimaryKeyControl(model), DbAHD.RowVersionControls(Portal.SingleToList(NationalStaffDangerPayInformation))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyNationalStaffDangerPayInformation(model.NationalStaffDangerPayGUID);
            }
        }
        #endregion


        #region Report
        public ActionResult ExportDPNationNotEligible()
        {
            Guid staffStatusInActive = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3611");
            Guid licaContract = Guid.Parse("41651262-08AC-42DC-A691-9BBCE28C95D1");
            Guid unposContract = Guid.Parse("1506F90E-F377-49B6-A8CD-116250246A44");
            Guid reqNational = Guid.Parse("41651262-08AC-42DC-A691-9BBCE28C95D1");
            //Guid ans = Guid.Parse("8E58217C-A2BD-4657-ABFD-021F5F3F0589");
            Guid _eligible = Guid.Parse("2DAC5D96-E6A3-48C1-B3F5-17BFD9F62881");

            var result = DbAHD.v_staffCoreDataOverview.Where(x => x.RecruitmentTypeGUID == reqNational
                                && x.ContractTypeGUID != unposContract
                                && x.PaymentEligibilityStatusGUID == null
                                && x.StaffStatusGUID == staffStatusInActive && x.ContractTypeGUID != licaContract).ToList();
            if (result.Count > 0)
            {
                string sourceFile = Server.MapPath("~/Areas/AHD/Templates/DPNationNotEligible.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/AHD/temp/DPNationNotEligible.xlsx" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();
                    dt.Columns.Add("Staff Name", typeof(string));
                    dt.Columns.Add("Status", typeof(string));
                    dt.Columns.Add("EligibleStatus", typeof(string));
                    dt.Columns.Add("Duty Station", typeof(string));


                    foreach (var item in result)
                    {

                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.FullName;
                        dr[1] = item.StaffStatus;
                        dr[2] = item.PaymentEligibilityStatusGUID;
                        dr[3] = item.DutyStation;


                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["A2"].LoadFromDataTable(dt, true);
                    workSheet.Cells["A1"].Value = "List of Staff Not Eligible ";

                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = "Absence table for period" + DateTime.Now + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }

            string success = "Data is ready";
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ExportDangerPayStaffForMonth(Guid id)
        {
            var result = DbAHD.v_NationalStaffDangerPayment.Where(x => x.DangerPayInformationGUID == id).ToList();
            var priResult = DbAHD.dataNationalStaffDangerPay.Where(x => x.IsPayed == false && x.DangerPayInformationGUID != id
            && x.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Confirmed
            && x.ParentDangerPayInformationGUID == id).ToList();
            if (result.Count > 0)
            {
                string sourceFile = Server.MapPath("~/Areas/AHD/Templates/ListOf_DBNationalStaff_DangerPay.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/AHD/temp/DBNationalStaff_DangerPay" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();
                    dt.Columns.Add("Staff Name", typeof(string));
                    dt.Columns.Add("Contract type", typeof(string));
                    dt.Columns.Add("Duty station", typeof(string));
                    dt.Columns.Add("Employment ID", typeof(string));
                    dt.Columns.Add("Number of days DP is not payable", typeof(int));
                    dt.Columns.Add("Number of days DP is  payable", typeof(int));
                    dt.Columns.Add("Current danger pay amount ", typeof(int));
                    dt.Columns.Add("Previous month(s) danger pay amount", typeof(int));
                    dt.Columns.Add("Covered period", typeof(string));
                    dt.Columns.Add("Travel dates confirmation)", typeof(string));
                    dt.Columns.Add("Total)", typeof(string));

                    foreach (var item in result)
                    {

                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.FullName;
                        dr[1] = item.ContractType;
                        dr[2] = item.DutyStation;
                        dr[3] = item.EmploymentID;
                        dr[4] = item.TotalDaysNotPayable ?? 0;
                        dr[5] = item.TotalDaysPayable ?? 0;
                        dr[6] = item.ActualDangerPayAmount ?? 0;
                        dr[7] = priResult.Where(x => x.UserGUID == item.UserGUID).Select(x => x.ActualDangerPayAmount).Sum() ?? 0;
                        //dr[8] = priResult.Count > 0 ? string.Join(",", priResult.Select(x => "'" + x.MaindataDangerPayInformation.PaymentDurationName + "'")) : " ";
                        dr[8] = item.PaymentDurationName;
                        dr[9] = item.FlowStatus;
                        dr[10] = item.ActualDangerPayAmount != null ? (priResult.Where(x => x.UserGUID == item.UserGUID).Select(x => x.ActualDangerPayAmount).Sum() > 0 ? priResult.Where(x => x.UserGUID == item.UserGUID).Select(x => x.ActualDangerPayAmount).Sum() + item.ActualDangerPayAmount : item.ActualDangerPayAmount) : (priResult.Where(x => x.UserGUID == item.UserGUID).Select(x => x.ActualDangerPayAmount).Sum() > 0 ? priResult.Where(x => x.UserGUID == item.UserGUID).Select(x => x.ActualDangerPayAmount).Sum() : 0);
                        //dr[6] = item.Mission_Leaves;
                        //dr[7] = item.Annual_Leaves;
                        //dr[8] = item.MaternityPaternity_Leaves;
                        //dr[9] = item.Sick_Leaves;
                        //dr[10] = item.Weekends_Leaves;

                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["A2"].LoadFromDataTable(dt, true);
                    workSheet.Cells["A1"].Value = "List of  national staff danger pay as of " + result.Select(x => x.PaymentDurationName).FirstOrDefault();

                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = "Absence table for period" + result.Select(x => x.PaymentDurationName).FirstOrDefault() + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }

            string success = "No Data Available for this period";
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ExportStandardDangerPayStaffForMonth(Guid id)
        {
            var result = DbAHD.v_NationalStaffDangerPayment.Where(x => x.DangerPayInformationGUID == id).ToList();
            var priResult = DbAHD.dataNationalStaffDangerPay.Where(x =>  x.DangerPayInformationGUID != id
            && x.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Confirmed
            && x.ParentDangerPayInformationGUID == id).ToList();
            if (result.Count > 0)
            {
                string sourceFile = Server.MapPath("~/Areas/AHD/Templates/DBStandardFinanceNationalStaff_DangerPay_Detail.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/AHD/temp/DBNationalStaff_DangerPay" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();
                    dt.Columns.Add("#", typeof(string));
                    dt.Columns.Add("Emp ID", typeof(string));
                    dt.Columns.Add("Duty Station", typeof(string));
                    dt.Columns.Add("Office", typeof(string));
                    dt.Columns.Add("Staff Name", typeof(string));

                    dt.Columns.Add("Leave days taken (RnR, HL, AL, mission - out of Syria)", typeof(string));
                    dt.Columns.Add("Dates for DP is not Payable", typeof(string));
                    dt.Columns.Add("Number of days DP is not payable", typeof(string));
                    dt.Columns.Add("Number of days DP is  payable", typeof(int));
                    dt.Columns.Add("Danger pay  ", typeof(int));
                    dt.Columns.Add("Comment ", typeof(string));

                    int i = 2;
                    int j = 1;

                    foreach (var item in result.OrderBy(x => x.DutyStation).ThenBy(x=>x.OfficeTypeName).ThenBy(x=>x.FullName))
                    {

                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = j;
                        dr[1] = item.EmploymentID;
                        dr[2] = item.DutyStation;
                        dr[3] = item.OfficeTypeName != null ? item.OfficeTypeName : (item.DutyStation == "Damascus" ? "Country Office" : "");
                        dr[4] = item.FullName;
                        dr[5] = item.TotalDaysNotPayable != 0 ?
                            !string.IsNullOrEmpty(item.Telecommuting_Out_Side_Duty_Station) ? item.Telecommuting_Out_Side_Duty_Station :
                               !string.IsNullOrEmpty(item.Annual_Leaves) ? item.Annual_Leaves :
                               !string.IsNullOrEmpty(item.SLWOP) ? item.SLWOP :
                               !string.IsNullOrEmpty(item.Mission_Leaves) ? item.Mission_Leaves :
                               !string.IsNullOrEmpty(item.MaternityPaternity_Leaves) ? item.MaternityPaternity_Leaves :
                               !string.IsNullOrEmpty(item.Sick_Leaves) ? item.Sick_Leaves :
                               !string.IsNullOrEmpty(item.Weekends_Leaves) ? item.Weekends_Leaves :
                               "" : "";
                        dr[6] = "";
                        dr[7] = item.TotalDaysNotPayable ?? 0;
                        dr[8] = item.TotalDaysPayable ?? 0;
                        dr[9] = item.ActualDangerPayAmount != null ? (priResult.Where(x => x.UserGUID == item.UserGUID).Select(x => x.ActualDangerPayAmount).Sum() > 0 ? priResult.Where(x => x.UserGUID == item.UserGUID).Select(x => x.ActualDangerPayAmount).Sum() + item.ActualDangerPayAmount : item.ActualDangerPayAmount) : (priResult.Where(x => x.UserGUID == item.UserGUID).Select(x => x.ActualDangerPayAmount).Sum() > 0 ? priResult.Where(x => x.UserGUID == item.UserGUID).Select(x => x.ActualDangerPayAmount).Sum() : 0);

                        dr[10] = item.Comments;

                        //dr[6] = item.Mission_Leaves;
                        //dr[7] = item.Annual_Leaves;
                        //dr[8] = item.MaternityPaternity_Leaves;
                        //dr[9] = item.Sick_Leaves;
                        //dr[10] = item.Weekends_Leaves;

                        if (item.ActualDangerPayAmount < 459)
                        {
                            workSheet.Cells["A" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["A" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                            workSheet.Cells["B" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["B" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                            workSheet.Cells["C" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["C" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                            workSheet.Cells["D" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["D" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                            workSheet.Cells["E" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["E" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                            workSheet.Cells["F" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["F" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                            workSheet.Cells["G" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["G" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                            workSheet.Cells["H" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["H" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);

                            workSheet.Cells["I" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["I" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);

                            workSheet.Cells["J" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["J" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);

                            workSheet.Cells["K" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["K" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);



                        }

                        if (item.ActualDangerPayAmount > 459)
                        {

                            workSheet.Cells["A" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["A" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkOrange);
                            workSheet.Cells["B" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["B" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkOrange);
                            workSheet.Cells["C" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["C" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkOrange);
                            workSheet.Cells["D" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["D" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkOrange);
                            workSheet.Cells["E" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["E" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkOrange);
                            workSheet.Cells["F" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["F" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkOrange);

                            workSheet.Cells["G" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["G" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkOrange);
                            workSheet.Cells["H" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["H" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkOrange);

                            workSheet.Cells["I" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["I" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkOrange);

                            workSheet.Cells["J" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["J" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkOrange);

                            workSheet.Cells["K" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["K" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkOrange);

                        }
                        if (item.FlowStatus == "Pending")
                        {


                            workSheet.Cells["A" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["A" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.PaleVioletRed);
                            workSheet.Cells["B" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["B" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.PaleVioletRed);
                            workSheet.Cells["C" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["C" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.PaleVioletRed);
                            workSheet.Cells["D" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["D" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.PaleVioletRed);
                            workSheet.Cells["E" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["E" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.PaleVioletRed);

                            workSheet.Cells["F" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["F" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.PaleVioletRed);


                            workSheet.Cells["G" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["G" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.PaleVioletRed);


                            workSheet.Cells["H" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["H" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.PaleVioletRed);


                            workSheet.Cells["I" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["I" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.PaleVioletRed);

                            workSheet.Cells["J" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["J" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.PaleVioletRed);


                            workSheet.Cells["K" + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells["K" + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.PaleVioletRed);



                        }
                        i++;
                        j++;
                        dt.Rows.Add(dr);
                    }


                    workSheet.Cells["A1"].LoadFromDataTable(dt, true);
                    //workSheet.Cells["F2"].Value = "List of  national staff danger pay as of " + result.Select(x => x.PaymentDurationName).FirstOrDefault();

                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = "Absence table for period" + result.Select(x => x.PaymentDurationName).FirstOrDefault() + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }

            string success = "No Data Available for this period";
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult ExportDetailDangerPayStaffForMonth(Guid id)
        {
            var result = DbAHD.v_NationalStaffDangerPayment.Where(x => x.DangerPayInformationGUID == id).ToList();
            var priResult = DbAHD.dataNationalStaffDangerPay.Where(x => x.IsPayed == false && x.DangerPayInformationGUID != id).ToList();

            if (result.Count > 0)
            {
                string sourceFile = Server.MapPath("~/Areas/AHD/Templates/DBNationalStaff_DangerPay_Detail.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/AHD/temp/DBNationalStaff_DangerPay_Detail" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();
                    dt.Columns.Add("Staff Name", typeof(string));
                    dt.Columns.Add("Contract type", typeof(string));
                    dt.Columns.Add("Duty station", typeof(string));
                    dt.Columns.Add("Employment ID", typeof(string));
                    dt.Columns.Add("Number of days DP is not payable", typeof(int));
                    dt.Columns.Add("Number of days DP is  payable", typeof(int));
                    dt.Columns.Add("Current danger pay amount ", typeof(int));
                    dt.Columns.Add("Privious month(s) danger pay amount ", typeof(int));
                    dt.Columns.Add("Confirmation Status", typeof(string));
                    dt.Columns.Add("Mission Outside leaves", typeof(string));
                    dt.Columns.Add("Annual Leaves", typeof(string));
                    dt.Columns.Add("Maternity Paternity leaves", typeof(string));
                    dt.Columns.Add("Sick Leave", typeof(string));
                    dt.Columns.Add("Weekends Leave", typeof(string));
                    dt.Columns.Add("Missions Inside Syria", typeof(string));
                    dt.Columns.Add("Telecommuting out side duty station", typeof(string));
                    dt.Columns.Add("SLWOP", typeof(string));
                    dt.Columns.Add("Comments", typeof(string));



                    foreach (var item in result)
                    {

                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.FullName;
                        dr[1] = item.ContractType;
                        dr[2] = item.DutyStation;
                        dr[3] = item.EmploymentID;

                        dr[4] = item.TotalDaysNotPayable ?? 0;
                        dr[5] = item.TotalDaysPayable ?? 0;
                        dr[6] = item.ActualDangerPayAmount ?? 0;
                        dr[7] = priResult.Where(x => x.UserGUID == item.UserGUID).Select(x => x.ActualDangerPayAmount).Sum() ?? 0;
                        dr[8] = item.FlowStatus;
                        dr[9] = item.Mission_Leaves;
                        dr[10] = item.Annual_Leaves;
                        dr[11] = item.MaternityPaternity_Leaves;
                        dr[12] = item.Sick_Leaves;
                        dr[13] = item.Weekends_Leaves;
                        dr[14] = item.Mission_Inisde;
                        dr[15] = item.Telecommuting_Out_Side_Duty_Station;
                        dr[16] = item.SLWOP;
                        dr[17] = item.Comments;

                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["B4"].LoadFromDataTable(dt, true);
                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = DateTime.Now.ToString("yyMMdd") + "_List of national staff danger pay Detail " + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }

            string success = "No Data Available for this period";
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Leave dates from full table
        [Route("AHD/BulkCreate/")]
        public ActionResult NationalStaffDangerPayBulk(Guid? id)
        {
            ViewBag.NationalStaffDangerPayGUID = id;
            return View("~/Areas/AHD/Views/NationalStaffDangerPayDetail/NationalStaffDangerPayBulkCreate.cshtml");
        }


        public ActionResult testx(int? x)
        {
            return View();
        }



        //[HttpPost, ValidateAntiForgeryToken]
        public ActionResult NationalStaffDangerPayBulkCreate(List<NationalStaffDangerPayDetailModel> mymodels, Guid nationalStaffDangerPayGUID)
        {
            #region Check
            if (mymodels == null || mymodels.Count() == 0)
            {
                return Json(new { success = -1 }, JsonRequestBehavior.AllowGet);
            }
            Guid _pending = Guid.Parse("A40EC252-622E-4FF1-9EF4-E323C7A3CEC5");
            var _status = DbAHD.dataDangerPayInformation.Where(x => x.FlowStatusGUID == _pending).FirstOrDefault();
            if (_status == null)
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
            }

            if (!ModelState.IsValid) return View("~/Areas/AHD/Views/NationalStaffDangerPayDetail/NationalStaffDangerPayBulk.cshtml", new { id = nationalStaffDangerPayGUID });
            var _naitonalStaff = DbAHD.dataNationalStaffDangerPay.Where(x => x.NationalStaffDangerPayGUID == nationalStaffDangerPayGUID).FirstOrDefault();
            var _dangerInformaiton = DbAHD.dataDangerPayInformation.Where(x => x.DangerPayInformationGUID == _naitonalStaff.DangerPayInformationGUID).FirstOrDefault();

            if (_naitonalStaff.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Confirmed)
            {
                ModelState.AddModelError("Error: ", "This leave already been submitted  ");
                return View("~/Areas/AHD/Views/NationalStaffDangerPayDetail/NationalStaffDangerPayBulk.cshtml", new { id = nationalStaffDangerPayGUID });
            }
            List<NationalStaffDangerPayDetailModel> temp = mymodels.ToList();
            foreach (var model in mymodels)
            {
                temp.Remove(model);
                foreach (var model2 in temp)
                {
                    if ((model.DepartureDate <= model2.ReturnDate && (model.ReturnDate >= model2.DepartureDate))
                            || (model2.DepartureDate <= model.ReturnDate && (model2.ReturnDate >= model.DepartureDate)))
                    {
                        ModelState.AddModelError("Error: ", "Kindly revise the entry data to avoid conflict in dates (check  either entries in this  month or  in previous months if have any  conflict)");
                        return Json(new { success = 0, error = "Kindly revise the entry data to avoid conflict in dates" }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            List<NationalStaffDangerPayDetailModel> _myChekList = mymodels.ToList();
            var temp2 = DbAHD.dataNationalStaffDangerPayDetail.Where(x => x.dataNationalStaffDangerPay.NationalStaffDangerPayGUID == nationalStaffDangerPayGUID).Select(x => new NationalStaffDangerPayDetailModel
            {
                DepartureDate = x.DepartureDate,
                ReturnDate = x.ReturnDate,
                NationalStaffDangerPayDetailGUID = x.NationalStaffDangerPayDetailGUID
            }).ToList();
            if (temp2.Count > 0)
            {
                _myChekList.AddRange(temp2);
                List<NationalStaffDangerPayDetailModel> temp3 = mymodels.ToList();
                temp3.AddRange(temp2);
                foreach (var model in _myChekList)
                {
                    temp3.Remove(model);
                    foreach (var model2 in temp3)
                    {
                        if ((model.DepartureDate <= model2.ReturnDate && (model.ReturnDate >= model2.DepartureDate))
                                || (model2.DepartureDate <= model.ReturnDate && (model2.ReturnDate >= model.DepartureDate)))
                        {
                            ModelState.AddModelError("Error: ", "Kindly revise the entry data to avoid conflict in dates");
                            return Json(new { success = 0, error = "Kindly revise the entry data to avoid conflict in dates" }, JsonRequestBehavior.AllowGet);
                        }

                    }
                }

            }
            foreach (var model in mymodels)
            {
                if (model.LeaveTypeGUID == null || model.DepartureDate > _dangerInformaiton.MonthEndDate
                   || model.DepartureDate < _dangerInformaiton.MonthStartDate || model.DepartureDate == null
                   || model.ReturnDate == null || model.CountryGUID == null || model.DepartureDate > model.ReturnDate)
                {
                    if (model.LeaveTypeGUID == null)
                    {
                        ModelState.AddModelError("Error: ", "Kindly choose your leave type  ");
                        return Json(new { success = 0, error = "Kindly choose your leave type" }, JsonRequestBehavior.AllowGet);


                    }
                    if (model.DepartureDate > _dangerInformaiton.MonthEndDate)
                    {
                        //ModelState.AddModelError("Error: ", "Your departure date must be less than:" + naitonalStaff.dataDangerPayInformation.MonthEndDate);
                        //return Json(new { success = 0, error = "Your departure date must be less than:" + naitonalStaff.dataDangerPayInformation.MonthEndDate }, JsonRequestBehavior.AllowGet);
                        ModelState.AddModelError("Error: ", " The correct month has to be chosen");
                        return Json(new { success = 0, error = " The correct month has to be chosen" }, JsonRequestBehavior.AllowGet);
                    }

                    if (model.DepartureDate == null || model.ReturnDate == null)
                    {
                        ModelState.AddModelError("Error: ", "Departure date and return date must has value ");
                        return Json(new { success = 0, error = "Departure date and return date must has value" }, JsonRequestBehavior.AllowGet);

                    }

                    else if (model.DepartureDate > model.ReturnDate)
                    {
                        ModelState.AddModelError("Error: ", "Departure date must be smaller than return date  ");
                        return Json(new { success = 0, error = "Departure date must be smaller than return date" }, JsonRequestBehavior.AllowGet);

                    }


                }


            }
            #endregion
            int _missionReturnSeq = 0;
            int _missionCurrentSeq = 0;
            int _reminingMission = 7;
            int firstLeave = 0;
            int secondLeave = 0;
            DateTime ExecutionTime = DateTime.Now;
            List<NationalStaffDangerPayDetailModel> newModels = new List<NationalStaffDangerPayDetailModel>();
            #region Check if has mission
            int orderid = 0;
            foreach (var item in mymodels.OrderBy(x => x.DepartureDate))
            {
                NationalStaffDangerPayDetailModel currentModel = item;
                orderid = orderid + 1;
                currentModel.orderId = orderid;
                newModels.Add(currentModel);
            }
            if (mymodels.Count > 0 && mymodels.Where(f => f.LeaveTypeGUID == NationalStaffLeaveType.Mission && f.orderId != 1).Count() > 0)
            {
                foreach (var item in mymodels.OrderBy(x => x.orderId))
                {
                    if (item.LeaveTypeGUID != NationalStaffLeaveType.MissionInsideSyria &&
                             item.LeaveTypeGUID != NationalStaffLeaveType.Mission && item.ReturnToDutyStationGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3112"))
                    {
                        var e = item.orderId + 1;
                        var nextVal = mymodels.Where(x => x.orderId == e).FirstOrDefault();
                        if ((nextVal.DepartureDate - item.ReturnDate).Value.Days == 1)
                        {
                            var current = item;
                            newModels.Remove(item);
                            current.hasPriMission = true;
                            newModels.Add(current);
                        }



                    }
                    else if (
                       item.LeaveTypeGUID == NationalStaffLeaveType.Mission)
                    {
                        var d = item.orderId - 1;
                        var prival = mymodels.Where(x => x.orderId == d).FirstOrDefault();
                        if ((item.DepartureDate - prival.ReturnDate).Value.Days == 1 && (prival.LeaveTypeGUID == NationalStaffLeaveType.AnnualLeave
                            || prival.LeaveTypeGUID == NationalStaffLeaveType.MaternityPaternity
                            || prival.LeaveTypeGUID == NationalStaffLeaveType.SickLeave
                            || prival.LeaveTypeGUID == NationalStaffLeaveType.Weekends
                            || prival.LeaveTypeGUID == NationalStaffLeaveType.TelecommutingOutSide
                            || prival.LeaveTypeGUID == NationalStaffLeaveType.SLWOP))
                        {
                            var current = item;
                            newModels.Remove(item);
                            current.hasMission = true;
                            newModels.Add(current);
                        }
                    }
                }
            }
            #endregion
            int totalRemeingDaysMission = 7;
            #region Action
            var lastDangerPayOrder = DbAHD.dataDangerPayInformation.Select(x => x.OrderId).Max();
            var staffDangerPay = DbAHD.dataNationalStaffDangerPay.Find(nationalStaffDangerPayGUID);
            var orderNext = DbAHD.dataDangerPayInformation.Select(x => x.OrderId).Max();
            var myDanger = DbAHD.dataDangerPayInformation.Where(x => x.OrderId == orderNext).FirstOrDefault();
            foreach (var model in newModels.OrderBy(x => x.orderId))
            {
                _missionCurrentSeq++;
                if (model.ReturnToDutyStationGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3112"))
                {
                    _missionReturnSeq++;
                }
                dataNationalStaffDangerPayDetail newPayDetail = new dataNationalStaffDangerPayDetail
                {
                    ActualReturnDate = model.ReturnDate,
                    NationalStaffDangerPayDetailGUID = Guid.NewGuid(),
                    NationalStaffDangerPayGUID = nationalStaffDangerPayGUID,
                    LeaveTypeGUID = model.LeaveTypeGUID,
                    DepartureDate = model.DepartureDate,
                    CountryGUID = model.CountryGUID,
                    DestinationName = model.DestinationName,
                    Comments = model.Comments,
                    ReturnToDutyStationGUID = model.ReturnToDutyStationGUID
                };
                if (model.ReturnDate > _dangerInformaiton.MonthEndDate)
                {
                    newPayDetail.IsLinked = true;
                    newPayDetail.ReturnDate = _dangerInformaiton.MonthEndDate;
                }
                else
                {
                    newPayDetail.IsLinked = false;
                    newPayDetail.ReturnDate = model.ReturnDate;
                }
                #region Clean Code 
                int totalDays = 0;
                int todalDayTemp = 0;

                if (model.DepartureDate < _dangerInformaiton.MonthStartDate)
                    totalDays = newPayDetail.IsLinked == true ?
                        (newPayDetail.ReturnDate - (_dangerInformaiton.MonthStartDate.Value.AddDays(-1))).Value.Days : (model.ReturnDate - _dangerInformaiton.MonthStartDate.Value.AddDays(-1)).Value.Days - 1;
                else
                {
                    totalDays = newPayDetail.IsLinked == true ? (newPayDetail.ReturnDate - model.DepartureDate).Value.Days : (model.ReturnDate - model.DepartureDate).Value.Days <= 0 ? 0 : (model.ReturnDate - model.DepartureDate).Value.Days - 1;
                }
                if (totalDays <= 0)
                {
                    totalDays = 0;
                }
                if (model.LeaveTypeGUID == NationalStaffLeaveType.MissionInsideSyria)
                {
                    totalDays = 0;
                }
                if (model.ReturnToDutyStationGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3112"))
                {
                    totalDays = totalDays + 1;
                }
                newPayDetail.OrderId = model.orderId;
                var priDetail = DbAHD.dataNationalStaffDangerPayDetail.Where(x => x.OrderId == newPayDetail.OrderId - 1).FirstOrDefault();
                if (priDetail != null && (model.DepartureDate - priDetail.ReturnDate).Value.Days == 1)
                {
                    totalDays = totalDays + 1;
                }
                newPayDetail.TotalDays = totalDays;
                #endregion
                #region To Correct
                #region Has Linked 
                if (newPayDetail.IsLinked == true)
                {
                    //var _maxOrder=DbAHD.dataDangerPayInformation.Select(f => f.OrderId).Max();
                    if (orderNext > _dangerInformaiton.OrderId)
                    {
                        var lastStaffDangerpay = DbAHD.dataNationalStaffDangerPay.Where(f => f.MaindataDangerPayInformation.OrderId == orderNext && f.UserGUID == UserGUID).FirstOrDefault();
                        if (lastStaffDangerpay != null)
                        {
                            dataNationalStaffDangerPayDetail mypayDetail = new dataNationalStaffDangerPayDetail
                            {
                                NationalStaffDangerPayDetailGUID = Guid.NewGuid(),
                                NationalStaffDangerPayGUID = lastStaffDangerpay.NationalStaffDangerPayGUID,
                                LeaveTypeGUID = model.LeaveTypeGUID,
                                DepartureDate = model.DepartureDate,
                                ReturnDate = model.ReturnDate,
                                ActualReturnDate = model.ReturnDate,
                                BaseLineReturnDate = newPayDetail.BaseLineReturnDate,
                                DestinationName = model.DestinationName,
                                CountryGUID = model.CountryGUID,
                                Comments = model.Comments,
                                Active = true,
                            };


                            //zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz
                            #region t

                            if (mypayDetail.ActualReturnDate > lastStaffDangerpay.MaindataDangerPayInformation.MonthEndDate)
                            {

                                mypayDetail.IsLinked = true;

                                mypayDetail.ReturnDate = lastStaffDangerpay.MaindataDangerPayInformation.MonthEndDate;
                            }
                            else
                            {
                                mypayDetail.IsLinked = false;

                            }


                            int ToaddtotalDays = mypayDetail.IsLinked == true ?
                                (mypayDetail.ActualReturnDate - lastStaffDangerpay.MaindataDangerPayInformation.MonthStartDate.Value.AddDays(-1)).Value.Days :
                                ((mypayDetail.ActualReturnDate - lastStaffDangerpay.MaindataDangerPayInformation.MonthStartDate.Value.AddDays(-1))).Value.Days - 1;
                            if (ToaddtotalDays <= 0)
                            {
                                ToaddtotalDays = 0;
                            }
                            if (mypayDetail.LeaveTypeGUID == NationalStaffLeaveType.Mission && totalDays <= 7)
                            {
                                ToaddtotalDays = 0;
                            }
                            else if (mypayDetail.LeaveTypeGUID == NationalStaffLeaveType.Mission && totalDays > 7)
                            {
                                ToaddtotalDays = totalDays - 7;
                            }
                            mypayDetail.TotalDays = (int?)ToaddtotalDays;
                            //int? _totalDaysNotPayable = (int?)lastStaffDangerpay.TotalDaysNotPayable ?? 0;
                            //lastStaffDangerpay.TotalDaysNotPayable = _totalDaysNotPayable + ToaddtotalDays;

                            //lastStaffDangerpay.TotalDaysPayable = lastStaffDangerpay.dataDangerPayInformation.TotalDaysInMonth - lastStaffDangerpay.TotalDaysNotPayable;
                            //lastStaffDangerpay.ActualDangerPayAmount = lastStaffDangerpay.TotalDaysPayable * 15;

                            //DbAHD.CreateNoAudit(mypayDetail);
                            DbAHD.UpdateNoAudit(lastStaffDangerpay);



                        }
                    }
                }

                #endregion







                #endregion
                //DbAHD.UpdateNoAudit(staffDangerPay);
                DbAHD.CreateNoAudit(newPayDetail);
                try
                {
                    DbAHD.SaveChanges();
                    DbCMS.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(DbAHD.ErrorMessage(ex.Message));
                }
                #endregion

            }
            #endregion
            #region Danger Pay for staff 
            var staffDetail = DbAHD.dataNationalStaffDangerPayDetail.Where(a => a.NationalStaffDangerPayGUID == nationalStaffDangerPayGUID).ToList();
            var mission = staffDetail.Where(x => x.LeaveTypeGUID == NationalStaffLeaveType.Mission).FirstOrDefault();
            int? totalLeaveDays = 0;
            if (mission != null)
            {
                if (mission.OrderId == 1)
                {
                    if (mission.TotalDays <= 7)
                    {
                        mission.TotalDays = 0;

                    }
                    else
                        mission.TotalDays = mission.TotalDays - 7;
                    DbAHD.Update(mission, Permissions.NationalStaffDangerPayManagement.UpdateGuid, ExecutionTime, DbCMS);
                    totalLeaveDays = staffDetail.Where(x => x.LeaveTypeGUID != NationalStaffLeaveType.Mission).
                        Select(f => f.TotalDays) != null ? staffDetail.Where(x => x.LeaveTypeGUID != NationalStaffLeaveType.Mission).Select(f => f.TotalDays).Sum() + mission.TotalDays : 0 + mission.TotalDays;
                }
                //to be careful 
                else
                {
                    var checkDate = staffDetail.Where(a => a.OrderId == mission.OrderId - 1).FirstOrDefault().ReturnDate;

                    if ((mission.DepartureDate - checkDate).Value.Days == 1)
                    {
                        int? reminingDay = 0;
                        int? totalpriMissionDays = staffDetail.Where(x => x.OrderId < mission.OrderId).Select(a => a.TotalDays) != null ?
                            staffDetail.Where(x => x.OrderId < mission.OrderId).Select(a => a.TotalDays).Sum() : 0;


                        var afterMissionDays = staffDetail.Where(x => x.OrderId > mission.OrderId).Select(a => a.TotalDays) != null ?
                            staffDetail.Where(x => x.OrderId > mission.OrderId).Select(a => a.TotalDays).Sum() : 0;

                        int? firstPri = 0;
                        int? toAddMission = 0;
                        int? checkdata = mission.TotalDays + totalpriMissionDays;
                        if (checkdata <= 7)
                        {
                            toAddMission = 0;


                        }
                        else
                        {
                            toAddMission = checkdata - 7;
                        }
                        //int? calc = mission.TotalDays - ((7 - totalpriMissionDays));

                        //if ((mission.TotalDays - ((7 - totalpriMissionDays)) > 0))
                        //{
                        //    if (totalpriMissionDays <= 7)
                        //    {
                        //        toAddMission = 7 - totalpriMissionDays;
                        //    }

                        //}

                        //else if (totalpriMissionDays > 7)
                        //    toAddMission = totalpriMissionDays - 7;
                        //else
                        //    toAddMission = 0;


                        //toAddMission = mission.TotalDays-((7 - totalpriMissionDays) > 0 ? (7 - totalpriMissionDays): ( totalpriMissionDays-7)) ??0;
                        totalLeaveDays = afterMissionDays + totalpriMissionDays + toAddMission;
                        //int? temp1 = staffDetail.Where(x => x.OrderId < mission.OrderId).
                        // Select(f => f.TotalDays) != null ? staffDetail.Where(x => x.OrderId < mission.OrderId).Select(f => f.TotalDays).Sum() : 0;
                        //if (temp1 > 7)
                        //{
                        //    reminingDay = temp1 - 7;
                        //    totalLeaveDays = reminingDay + mission.TotalDays + afterMissionDays;
                        //}
                        //else
                        //{
                        //    int? missionReminingDay = 7 - temp1;
                        //    if (mission.TotalDays > missionReminingDay)
                        //    {
                        //        reminingDay = mission.TotalDays - missionReminingDay;
                        //        totalLeaveDays = reminingDay + afterMissionDays;
                        //    }
                        //    else
                        //    {
                        //        reminingDay = 0;
                        //        totalLeaveDays = reminingDay + afterMissionDays;
                        //    }
                        //}
                    }
                    //end of update   
                    else
                    {
                        if (mission.TotalDays <= 7)
                        {
                            mission.TotalDays = 0;

                        }
                        else
                            mission.TotalDays = mission.TotalDays - 7;
                        totalLeaveDays = staffDetail.Where(x => x.LeaveTypeGUID != NationalStaffLeaveType.Mission).
                      Select(f => f.TotalDays) != null ? staffDetail.Where(x => x.LeaveTypeGUID != NationalStaffLeaveType.Mission).Select(f => f.TotalDays).Sum() + mission.TotalDays : 0 + mission.TotalDays;

                    }
                }
            }
            else
            {
                totalLeaveDays = staffDetail.Select(f => f.TotalDays) != null ? staffDetail.Select(f => f.TotalDays).Sum() : 0;
            }
            if (myDanger != null && staffDangerPay.IsPayed != true && orderNext != null &&
                staffDangerPay.DangerPayInformationGUID != myDanger.DangerPayInformationGUID)
            {
                staffDangerPay.ParentDangerPayInformationGUID = myDanger.DangerPayInformationGUID;
            }
            staffDangerPay.FlowStatusGUID = NationalStaffDangerPaConfirmationStatus.Confirmed;
            staffDangerPay.ResponseDate = ExecutionTime;
            int? totaldaysNotPayable = (int?)staffDangerPay.TotalDaysNotPayable ?? 0;
            staffDangerPay.TotalDaysNotPayable = (totaldaysNotPayable + totalLeaveDays) > 31 ? 31 : (totaldaysNotPayable + totalLeaveDays);
            staffDangerPay.TotalDaysPayable = (staffDangerPay.MaindataDangerPayInformation.TotalDaysInMonth - staffDangerPay.TotalDaysNotPayable) < 0 ? 0 : staffDangerPay.MaindataDangerPayInformation.TotalDaysInMonth - staffDangerPay.TotalDaysNotPayable;
            staffDangerPay.ActualDangerPayAmount = staffDangerPay.TotalDaysPayable == staffDangerPay.MaindataDangerPayInformation.TotalDaysInMonth ? staffDangerPay.MaindataDangerPayInformation.DangerPayAmount < 0 ? 0 : staffDangerPay.MaindataDangerPayInformation.DangerPayAmount : staffDangerPay.TotalDaysPayable * 15 < 0 ? 0 : staffDangerPay.TotalDaysPayable * 15;
            staffDangerPay.IsAnswerd = true;
            DbAHD.Update(staffDangerPay, Permissions.NationalStaffDangerPayManagement.UpdateGuid, ExecutionTime, DbCMS);
            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
            #endregion



            SendConfirmationFoStaffHasLeavesAfterSubmission(staffDangerPay.NationalStaffDangerPayGUID);

            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
            //return View("~/Areas/AHD/Views/NationalStaffDangerPayDetail/NationalStaffDangerPayBulk.cshtml", new { id = nationalStaffDangerPayGUID });
        }


        public ActionResult ConfirmStaff()
        {

            return View("~/Areas/AHD/Views/Shared/ConfirmationComplate.cshtml");


        }

        #endregion

        #region Historical data for payments 
        public ActionResult TrackStaffDangerPayments(Guid id)
        {
            var DangerPay = DbAHD.dataNationalStaffDangerPay.Where(x => x.NationalStaffDangerPayGUID == id

                       ).FirstOrDefault();


            if (DangerPay == null || (DangerPay.UserGUID != UserGUID && !CMS.HasAction(Permissions.NationalStaffDangerPayManagement.Access, Apps.AHD)))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return View("~/Areas/AHD/Views/NationalStaffDangerPay/TrackStaffDangerPayments.cshtml", new TrackStaffDangerPaymentsDataTableModel { NationalStaffDangerPayGUID = id });
        }

        [Route("AHD/NationalStaffDangerPayCalcualtion/WorkplaceStaffDangerPayments/")]
        public ActionResult WorkplaceStaffDangerPayments(Guid? id)
        {
            if (id == null)
                id = UserGUID;
            var DangerPay = DbAHD.dataNationalStaffDangerPay.Where(x => x.UserGUID == id

                       ).FirstOrDefault();
            if (DangerPay == null || id != UserGUID)
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return View("~/Areas/AHD/Views/NationalStaffDangerPay/TrackStaffDangerPayments.cshtml", new TrackStaffDangerPaymentsDataTableModel { NationalStaffDangerPayGUID = DangerPay.NationalStaffDangerPayGUID });
        }
        public ActionResult TrackStaffDangerPaymentDataTable(DataTableRecievedOptions options, Guid PK)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);
            Expression<Func<TrackStaffDangerPaymentsDataTableModel, bool>> Predicate = p => true;
            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<TrackStaffDangerPaymentsDataTableModel>(DataTable.Filters);
            }
            var userGuid = DbAHD.dataNationalStaffDangerPay.Where(x => x.NationalStaffDangerPayGUID == PK).Select(f => f.UserGUID).FirstOrDefault();
            var All = (
                  from a in DbAHD.dataNationalStaffDangerPay.Where(a => a.UserGUID == userGuid).AsExpandable()
                  join b in DbAHD.dataDangerPayInformation on a.DangerPayInformationGUID equals b.DangerPayInformationGUID into LJ1
                  from R1 in LJ1.DefaultIfEmpty()

                  join c in DbAHD.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN) on a.FlowStatusGUID equals c.ValueGUID into LJ2
                  from R2 in LJ2.DefaultIfEmpty()

                  join d in DbAHD.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN) on R1.FlowStatusGUID equals d.ValueGUID into LJ3
                  from R3 in LJ3.DefaultIfEmpty()
                  select new TrackStaffDangerPaymentsDataTableModel
                  {
                      NationalStaffDangerPayGUID = a.NationalStaffDangerPayGUID,

                      PaymentDurationName = R1.PaymentDurationName,
                      Active = a.Active,
                      FlowStatus = R2.ValueDescription,
                      DangerPayStatus = R3.ValueDescription,


                      OrderId = R1.OrderId,
                      dataNationalStaffDangerPayRowVersion = a.dataNationalStaffDangerPayRowVersion
                  }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<TrackStaffDangerPaymentsDataTableModel> Result = Mapper.Map<List<TrackStaffDangerPaymentsDataTableModel>>(All.OrderByDescending(x => x.OrderId).Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region old 
        //public ActionResult old_xxxx_NationalStaffDangerPayBulkCreate(List<NationalStaffDangerPayDetailModel> mymodels, Guid nationalStaffDangerPayGUID)
        //{
        //    #region Check
        //    if (mymodels == null || mymodels.Count() == 0)
        //    {
        //        return Json(new { success = -1 }, JsonRequestBehavior.AllowGet);
        //        // ModelState.AddModelError("Error: ", "You have to provide us with your leave dates if any if you have no leave during this period just click on back and chose first option  ");
        //        //return View("~/Areas/AHD/Views/NationalStaffDangerPayDetail/NationalStaffDangerPayBulk.cshtml", new { id = nationalStaffDangerPayGUID });
        //    }
        //    if (!ModelState.IsValid) return View("~/Areas/AHD/Views/NationalStaffDangerPayDetail/NationalStaffDangerPayBulk.cshtml", new { id = nationalStaffDangerPayGUID });
        //    var naitonalStaff = DbAHD.dataNationalStaffDangerPay.Where(x => x.NationalStaffDangerPayGUID == nationalStaffDangerPayGUID).FirstOrDefault();
        //    if (naitonalStaff.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Confirmed)
        //    {
        //        ModelState.AddModelError("Error: ", "This leave already been submitted  ");
        //        return View("~/Areas/AHD/Views/NationalStaffDangerPayDetail/NationalStaffDangerPayBulk.cshtml", new { id = nationalStaffDangerPayGUID });
        //    }
        //    List<NationalStaffDangerPayDetailModel> temp = mymodels.ToList();

        //    foreach (var model in mymodels)
        //    {
        //        temp.Remove(model);
        //        foreach (var model2 in temp)
        //        {
        //            if ((model.DepartureDate <= model2.ReturnDate && (model.ReturnDate >= model2.DepartureDate))
        //                    || (model2.DepartureDate <= model.ReturnDate && (model2.ReturnDate >= model.DepartureDate)))
        //            {
        //                ModelState.AddModelError("Error: ", "Kindly revise the entry data to avoid conflict in dates");
        //                return Json(new { success = 0, error = "Kindly revise the entry data to avoid conflict in dates" }, JsonRequestBehavior.AllowGet);
        //            }

        //        }
        //    }

        //    #endregion
        //    #region Process
        //    List<NationalStaffDangerPayDetailModel> _myChekList = mymodels.ToList();

        //    var temp2 = DbAHD.dataNationalStaffDangerPayDetail.Where(x => x.dataNationalStaffDangerPay.NationalStaffDangerPayGUID == nationalStaffDangerPayGUID).Select(x => new NationalStaffDangerPayDetailModel
        //    {
        //        DepartureDate = x.DepartureDate,
        //        ReturnDate = x.ReturnDate,
        //        NationalStaffDangerPayDetailGUID = x.NationalStaffDangerPayDetailGUID
        //    }).ToList();
        //    if (temp2.Count > 0)
        //    {
        //        _myChekList.AddRange(temp2);
        //        List<NationalStaffDangerPayDetailModel> temp3 = mymodels.ToList();
        //        temp3.AddRange(temp2);
        //        foreach (var model in _myChekList)
        //        {
        //            temp3.Remove(model);
        //            foreach (var model2 in temp3)
        //            {
        //                if ((model.DepartureDate <= model2.ReturnDate && (model.ReturnDate >= model2.DepartureDate))
        //                        || (model2.DepartureDate <= model.ReturnDate && (model2.ReturnDate >= model.DepartureDate)))
        //                {
        //                    ModelState.AddModelError("Error: ", "Kindly revise the entry data to avoid conflict in dates");
        //                    return Json(new { success = 0, error = "Kindly revise the entry data to avoid conflict in dates" }, JsonRequestBehavior.AllowGet);
        //                }

        //            }
        //        }

        //    }

        //    foreach (var model in mymodels)
        //    {
        //        if (model.LeaveTypeGUID == null || model.DepartureDate > naitonalStaff.dataDangerPayInformation.MonthEndDate
        //           || model.DepartureDate < naitonalStaff.dataDangerPayInformation.MonthStartDate || model.DepartureDate == null
        //           || model.ReturnDate == null || model.CountryGUID == null || model.DepartureDate > model.ReturnDate)
        //        {
        //            if (model.LeaveTypeGUID == null)
        //            {
        //                ModelState.AddModelError("Error: ", "Kindly choose your leave type  ");
        //                return Json(new { success = 0, error = "Kindly choose your leave type" }, JsonRequestBehavior.AllowGet);


        //            }
        //            if (model.DepartureDate > naitonalStaff.dataDangerPayInformation.MonthEndDate)
        //            {
        //                //ModelState.AddModelError("Error: ", "Your departure date must be less than:" + naitonalStaff.dataDangerPayInformation.MonthEndDate);
        //                //return Json(new { success = 0, error = "Your departure date must be less than:" + naitonalStaff.dataDangerPayInformation.MonthEndDate }, JsonRequestBehavior.AllowGet);
        //                ModelState.AddModelError("Error: ", " The correct month has to be chosen");
        //                return Json(new { success = 0, error = " The correct month has to be chosen" }, JsonRequestBehavior.AllowGet);
        //            }

        //            if (model.DepartureDate == null || model.ReturnDate == null)
        //            {
        //                ModelState.AddModelError("Error: ", "Departure date and return date must has value ");
        //                return Json(new { success = 0, error = "Departure date and return date must has value" }, JsonRequestBehavior.AllowGet);

        //            }

        //            else if (model.DepartureDate > model.ReturnDate)
        //            {
        //                ModelState.AddModelError("Error: ", "Departure date must be smaller than return date  ");
        //                return Json(new { success = 0, error = "Departure date must be smaller than return date" }, JsonRequestBehavior.AllowGet);

        //            }


        //        }


        //    }

        //    #endregion
        //    int _missionReturnSeq = 0;
        //    int _missionCurrentSeq = 0;
        //    int _reminingMission = 7;
        //    int firstLeave = 0;
        //    int secondLeave = 0;
        //    List<NationalStaffDangerPayDetailModel> newModels = new List<NationalStaffDangerPayDetailModel>();
        //    #region Check if has mission
        //    int orderid = 0;
        //    foreach (var item in mymodels.OrderBy(x => x.DepartureDate))
        //    {
        //        NationalStaffDangerPayDetailModel currentModel = item;
        //        orderid = orderid + 1;
        //        currentModel.orderId = orderid;

        //        newModels.Add(currentModel);

        //    }

        //    if (mymodels.Count > 0 && mymodels.Where(f => f.LeaveTypeGUID == NationalStaffLeaveType.Mission && f.orderId != 1).Count() > 0)
        //    {
        //        foreach (var item in mymodels.OrderBy(x => x.orderId))
        //        {
        //            if (item.LeaveTypeGUID != NationalStaffLeaveType.MissionInsideSyria &&
        //                     item.LeaveTypeGUID != NationalStaffLeaveType.Mission && item.ReturnToDutyStationGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3112"))
        //            {
        //                var e = item.orderId + 1;
        //                var nextVal = mymodels.Where(x => x.orderId == e).FirstOrDefault();
        //                if ((nextVal.DepartureDate - item.ReturnDate).Value.Days == 1)
        //                {
        //                    var current = item;
        //                    newModels.Remove(item);
        //                    current.hasPriMission = true;
        //                    newModels.Add(current);
        //                }



        //            }
        //            else if (
        //               item.LeaveTypeGUID == NationalStaffLeaveType.Mission)
        //            {
        //                var d = item.orderId - 1;
        //                var prival = mymodels.Where(x => x.orderId == d).FirstOrDefault();
        //                if ((item.DepartureDate - prival.ReturnDate).Value.Days == 1 && (prival.LeaveTypeGUID == NationalStaffLeaveType.AnnualLeave
        //                    || prival.LeaveTypeGUID == NationalStaffLeaveType.MaternityPaternity
        //                    || prival.LeaveTypeGUID == NationalStaffLeaveType.SickLeave
        //                    || prival.LeaveTypeGUID == NationalStaffLeaveType.Weekends))
        //                {
        //                    var current = item;
        //                    newModels.Remove(item);
        //                    current.hasMission = true;
        //                    newModels.Add(current);
        //                }
        //            }
        //        }
        //    }
        //    #endregion

        //    int totalRemeingDaysMission = 7;
        //    foreach (var model in newModels.OrderBy(x => x.orderId))
        //    {
        //        _missionCurrentSeq++;

        //        if (model.ReturnToDutyStationGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3112"))
        //        {
        //            _missionReturnSeq++;
        //        }
        //        DateTime ExecutionTime = DateTime.Now;
        //        dataNationalStaffDangerPayDetail newPayDetail = new dataNationalStaffDangerPayDetail
        //        {
        //            ActualReturnDate = model.ReturnDate,
        //            NationalStaffDangerPayDetailGUID = Guid.NewGuid(),
        //            NationalStaffDangerPayGUID = nationalStaffDangerPayGUID,
        //            LeaveTypeGUID = model.LeaveTypeGUID,
        //            DepartureDate = model.DepartureDate,
        //            CountryGUID = model.CountryGUID,
        //            DestinationName = model.DestinationName,
        //            ReturnToDutyStationGUID = model.ReturnToDutyStationGUID

        //        };
        //        if (model.ReturnDate > naitonalStaff.dataDangerPayInformation.MonthEndDate)
        //        {

        //            newPayDetail.IsLinked = true;

        //            newPayDetail.ReturnDate = naitonalStaff.dataDangerPayInformation.MonthEndDate;
        //        }
        //        else
        //        {
        //            newPayDetail.IsLinked = false;
        //            newPayDetail.ReturnDate = model.ReturnDate;
        //        }
        //        var staffDangerPay = DbAHD.dataNationalStaffDangerPay.Find(nationalStaffDangerPayGUID);
        //        staffDangerPay.FlowStatusGUID = NationalStaffDangerPaConfirmationStatus.Confirmed;
        //        staffDangerPay.ResponseDate = ExecutionTime;
        //        int totalDays = 0;
        //        int todalDayTemp = 0;
        //        if (model.DepartureDate < naitonalStaff.dataDangerPayInformation.MonthStartDate)
        //            totalDays = newPayDetail.IsLinked == true ?
        //                (newPayDetail.ReturnDate - (naitonalStaff.dataDangerPayInformation.MonthStartDate.Value.AddDays(-1))).Value.Days : (model.ReturnDate - naitonalStaff.dataDangerPayInformation.MonthStartDate.Value.AddDays(-1)).Value.Days - 1;
        //        else
        //        {

        //            totalDays = newPayDetail.IsLinked == true ? (newPayDetail.ReturnDate - model.DepartureDate).Value.Days : (model.ReturnDate - model.DepartureDate).Value.Days <= 0 ? 0 : (model.ReturnDate - model.DepartureDate).Value.Days - 1;
        //            todalDayTemp = totalDays;
        //        }
        //        if (totalDays <= 0)
        //        {
        //            totalDays = 0;
        //            todalDayTemp = 0;
        //        }
        //        //checke parent danger pay and check mission 
        //        if (model.LeaveTypeGUID == NationalStaffLeaveType.MissionInsideSyria)
        //        {
        //            //if(model.ReturnToDutyStationGUID != Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3112"))
        //            totalDays = 0;
        //            //else
        //            //    totalDays = 1;
        //        }
        //        else if (model.LeaveTypeGUID == NationalStaffLeaveType.Mission && totalDays <= 7)
        //        {
        //            //if(model.ReturnToDutyStationGUID != Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3112"))
        //            totalDays = 0;
        //            //else
        //            //    totalDays = 1;
        //        }
        //        else if (model.LeaveTypeGUID == NationalStaffLeaveType.Mission && totalDays > 7)
        //        {
        //            totalDays = totalDays - 7;
        //            //if (model.ReturnToDutyStationGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3112"))

        //            //    totalDays += 1;

        //        }
        //        //check xx
        //        if ((model.ReturnDate - model.DepartureDate).Value.Days == 0
        //        && (model.ReturnToDutyStationGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3112")))
        //        {
        //            totalDays = totalDays + 1;
        //            todalDayTemp = todalDayTemp + 1;
        //        }
        //        else if (_missionReturnSeq == 1 && model.ReturnToDutyStationGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3112"))
        //        {
        //            //if (model.LeaveTypeGUID != NationalStaffLeaveType.Mission || (model.LeaveTypeGUID == NationalStaffLeaveType.Mission && totalDays >= 7))
        //            totalDays = totalDays + 1;
        //            todalDayTemp = todalDayTemp + 1;


        //        }
        //        else if (_missionReturnSeq > 1 && model.ReturnToDutyStationGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3112"))
        //        {
        //            if (_missionCurrentSeq - _missionReturnSeq == 0 || _missionCurrentSeq - _missionReturnSeq == 1)
        //            {
        //                //if (model.LeaveTypeGUID != NationalStaffLeaveType.Mission || (model.LeaveTypeGUID == NationalStaffLeaveType.Mission && totalDays >= 7))
        //                totalDays = totalDays + 2;
        //                todalDayTemp = todalDayTemp + 2;
        //            }
        //            else if (_missionCurrentSeq - _missionReturnSeq > 2)
        //            {
        //                //if (model.LeaveTypeGUID != NationalStaffLeaveType.Mission || (model.LeaveTypeGUID == NationalStaffLeaveType.Mission && totalDays >= 7))
        //                totalDays = totalDays + 1;
        //                todalDayTemp = todalDayTemp + 1;
        //            }

        //        }
        //        //check if return yes and privious is no
        //        else if (_missionReturnSeq >= 1 && (_missionCurrentSeq - _missionReturnSeq == 1) && model.ReturnToDutyStationGUID == Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3111"))
        //        {
        //            //if (model.LeaveTypeGUID != NationalStaffLeaveType.Mission || (model.LeaveTypeGUID == NationalStaffLeaveType.Mission && totalDays >= 7))
        //            totalDays = totalDays + 1;
        //            todalDayTemp = todalDayTemp + 1;
        //        }

        //        if (model.hasPriMission == true)
        //        {
        //            if (todalDayTemp <= 7)
        //            {
        //                var mytemp = totalRemeingDaysMission - todalDayTemp;
        //                totalDays = 0;
        //                totalRemeingDaysMission = mytemp;

        //            }
        //            else
        //            {
        //                totalDays = todalDayTemp - totalRemeingDaysMission;
        //                totalRemeingDaysMission = 0;
        //            }

        //        }
        //        if (model.hasMission == true)
        //        {
        //            if (totalRemeingDaysMission > 0)
        //            {
        //                totalDays = todalDayTemp < totalRemeingDaysMission ? totalRemeingDaysMission - todalDayTemp : todalDayTemp - totalRemeingDaysMission;
        //                totalRemeingDaysMission = totalDays;
        //            }
        //            else
        //            {
        //                totalDays = todalDayTemp;
        //                totalRemeingDaysMission = 0;
        //            }


        //        }
        //        var lastDangerPayOrder = DbAHD.dataDangerPayInformation.Select(x => x.OrderId).Max();
        //        var orderNext = (int?)staffDangerPay.dataDangerPayInformation.OrderId + 1;
        //        var myDanger = DbAHD.dataDangerPayInformation.Where(x => x.OrderId == orderNext).FirstOrDefault();
        //        if (myDanger != null && staffDangerPay.IsPayed != true && orderNext != null && staffDangerPay.DangerPayInformationGUID != myDanger.DangerPayInformationGUID)
        //        {
        //            staffDangerPay.ParentDangerPayInformationGUID = myDanger.DangerPayInformationGUID;

        //        }
        //        int? totaldaysNotPayable = (int?)staffDangerPay.TotalDaysNotPayable ?? 0;
        //        staffDangerPay.TotalDaysNotPayable = totaldaysNotPayable + totalDays;

        //        staffDangerPay.TotalDaysPayable = staffDangerPay.dataDangerPayInformation.TotalDaysInMonth - staffDangerPay.TotalDaysNotPayable;
        //        staffDangerPay.ActualDangerPayAmount = staffDangerPay.TotalDaysPayable > 30 ? 30 * 15 : staffDangerPay.TotalDaysPayable * 15;
        //        staffDangerPay.IsAnswerd = true;
        //        if (newPayDetail.IsLinked == true)
        //        {
        //            //var _maxOrder=DbAHD.dataDangerPayInformation.Select(f => f.OrderId).Max();
        //            if (orderNext > naitonalStaff.dataDangerPayInformation.OrderId)
        //            {
        //                var lastStaffDangerpay = DbAHD.dataNationalStaffDangerPay.Where(f => f.dataDangerPayInformation.OrderId == orderNext && f.dataStaffEligibleForDangerPay.UserGUID == UserGUID).FirstOrDefault();
        //                if (lastStaffDangerpay != null)
        //                {
        //                    dataNationalStaffDangerPayDetail mypayDetail = new dataNationalStaffDangerPayDetail
        //                    {
        //                        NationalStaffDangerPayDetailGUID = Guid.NewGuid(),
        //                        NationalStaffDangerPayGUID = lastStaffDangerpay.NationalStaffDangerPayGUID,
        //                        LeaveTypeGUID = model.LeaveTypeGUID,
        //                        DepartureDate = model.DepartureDate,
        //                        ReturnDate = model.ReturnDate,
        //                        ActualReturnDate = model.ReturnDate,
        //                        BaseLineReturnDate = newPayDetail.BaseLineReturnDate,
        //                        DestinationName = model.DestinationName,
        //                        CountryGUID = model.CountryGUID,
        //                        Active = true,
        //                    };



        //                    #region t

        //                    if (mypayDetail.ActualReturnDate > lastStaffDangerpay.dataDangerPayInformation.MonthEndDate)
        //                    {

        //                        mypayDetail.IsLinked = true;

        //                        mypayDetail.ReturnDate = lastStaffDangerpay.dataDangerPayInformation.MonthEndDate;
        //                    }
        //                    else
        //                    {
        //                        mypayDetail.IsLinked = false;

        //                    }


        //                    int ToaddtotalDays = mypayDetail.IsLinked == true ?
        //                        (mypayDetail.ActualReturnDate - lastStaffDangerpay.dataDangerPayInformation.MonthStartDate.Value.AddDays(-1)).Value.Days :
        //                        ((mypayDetail.ActualReturnDate - lastStaffDangerpay.dataDangerPayInformation.MonthStartDate.Value.AddDays(-1))).Value.Days - 1;
        //                    if (ToaddtotalDays <= 0)
        //                    {
        //                        ToaddtotalDays = 0;
        //                    }
        //                    if (mypayDetail.LeaveTypeGUID == NationalStaffLeaveType.Mission && totalDays <= 7)
        //                    {
        //                        ToaddtotalDays = 0;
        //                    }
        //                    else if (mypayDetail.LeaveTypeGUID == NationalStaffLeaveType.Mission && totalDays > 7)
        //                    {
        //                        ToaddtotalDays = totalDays - 7;
        //                    }

        //                    int? _totalDaysNotPayable = (int?)lastStaffDangerpay.TotalDaysNotPayable ?? 0;
        //                    lastStaffDangerpay.TotalDaysNotPayable = _totalDaysNotPayable + ToaddtotalDays;

        //                    lastStaffDangerpay.TotalDaysPayable = lastStaffDangerpay.dataDangerPayInformation.TotalDaysInMonth - lastStaffDangerpay.TotalDaysNotPayable;
        //                    lastStaffDangerpay.ActualDangerPayAmount = lastStaffDangerpay.TotalDaysPayable > 30 ? 30 * 15 : lastStaffDangerpay.TotalDaysPayable * 15;

        //                    DbAHD.CreateNoAudit(mypayDetail);
        //                    DbAHD.UpdateNoAudit(lastStaffDangerpay);



        //                }
        //            }
        //        }
        //        //to check
        //        //if (_missionSeq == 1 &&
        //        //    model.ReturnToDutyStationGUID==Guid.Parse( "b9cd375c-a576-4aa4-8af4-ff3c1c4e3112")
        //        //    )
        //        //{
        //        //    if( model.LeaveTypeGUID == NationalStaffLeaveType.Mission)
        //        //    staffDangerPay.TotalDaysNotPayable= staffDangerPay.TotalDaysNotPayable
        //        //}


        //        DbAHD.UpdateNoAudit(staffDangerPay);
        //        DbAHD.CreateNoAudit(newPayDetail);

        //        try
        //        {
        //            DbAHD.SaveChanges();
        //            DbCMS.SaveChanges();
        //        }
        //        catch (Exception ex)
        //        {
        //            return Json(DbAHD.ErrorMessage(ex.Message));
        //        }


        //    }
        //    return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
        //    #endregion
        //    //return View("~/Areas/AHD/Views/NationalStaffDangerPayDetail/NationalStaffDangerPayBulk.cshtml", new { id = nationalStaffDangerPayGUID });
        //}
        #endregion
    }
}
