using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AutoMapper;
using LinqKit;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using TBS_DAL.Model;

namespace AppsPortal.Areas.TBS.Controllers
{
    public class UserBillsController : TBSBaseController
    {
        private Guid _BillForTypeGUID_Mobile = Guid.Parse("9D8A1EB9-C2AC-4D78-95FF-874E46074321");

        [Route("TBS/UserBills/MyBills")]
        public ActionResult MyPhoneBills()
        {
            return View("~/Areas/TBS/Views/UserBills/MyPhoneBills/Index.cshtml");
        }

        [Route("TBS/UserBills/MyPhoneBillsDataTable/")]
        public JsonResult MyPhoneBillsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MyPhoneBillsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MyPhoneBillsDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbTBS.v_MyOwnBillsMain
                       where a.BillForTypeLAN == LAN && a.OperationsLanguagesLAN == LAN && a.TelecomCompanyLanguagesLAN == LAN
                       && a.UserGUID == UserGUID
                       select new MyPhoneBillsDataTableModel
                       {
                           BillGUID = a.BillGUID,
                           UserBillGUID = a.UserBillGUID,
                           BillForMonth = a.BillForMonth.ToString(),
                           BillForYear = a.BillForYear.ToString(),
                           BillForTypeDescription = a.BillForTypeDescription,
                           OperationDescription = a.OperationDescription,
                           TelecomCompanyDescription = a.TelecomCompanyDescription,
                           UserGUID = a.UserGUID,
                           IsConfirmed = a.IsConfirmed,
                           Active = a.Active
                       }).OrderByDescending(x => x.BillForYear).ThenByDescending(x => x.BillForMonth).Where(Predicate);

            List<MyPhoneBillsDataTableModel> Result = Mapper.Map<List<MyPhoneBillsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("TBS/UserBills/Update_WITHOUT_AUTO_CHECK_ORIGINAL/{PK}")]
        public ActionResult UserBillDetailsUpdateWITHOUT_AUTO_CHECK_ORIGINAL(Guid PK)
        {
            List<OwnUserBillConfirmModel> model = (from a in DbTBS.v_UserBillDetailsGrouped.Where(x => x.UserBillGUID == PK)
                                                   where a.TotalCallCost > 0
                                                   select new OwnUserBillConfirmModel
                                                   {
                                                       EDMXID = a.EDMXID,
                                                       UserBillGUID = a.UserBillGUID,
                                                       CallingNumber = a.CallingNumber,
                                                       CalledNumber = a.CalledNumber,
                                                       NumberOfCalls = a.NumberOfCalls.Value,
                                                       CalledStaffName = a.CalledStaffName,
                                                       TotalDurationMinutes = a.TotalDurationMinutes.HasValue ? a.TotalDurationMinutes.Value : 0,
                                                       TotalDurationInSeconds = a.TotalDurationInSeconds.HasValue ? a.TotalDurationInSeconds.Value : 0,
                                                       TotalCallCost = a.TotalCallCost.HasValue ? a.TotalCallCost.Value : 0,
                                                       IsPartiallyConfirmed = a.IsConfirmed,
                                                       IsPrivate = a.IsPrivate,
                                                       LastConfirmation = a.LastConfirmation.HasValue ? (a.LastConfirmation.Value == true ? "Private" : "Official") : "N/A",
                                                       AutomatedBySystem = a.AutomatedBySystem,
                                                       UserOverride = a.UserOverride,
                                                       IsBillLocked = a.BillLocked
                                                   }).ToList();




            model.First().TotalBillMinutes = model.Select(x => x.TotalDurationMinutes).Sum();
            model.First().TotalBillSeconds = model.Select(x => x.TotalDurationInSeconds).Sum();
            model.First().TotalBillCost = Convert.ToDouble(String.Format("{0:0.00}", model.Select(x => x.TotalCallCost).Sum()));

            model.First().TotalBillOfficialMinutes = model.Where(x => !x.IsPrivate && x.IsPartiallyConfirmed).Select(x => x.TotalDurationMinutes).Sum();
            model.First().TotalBillOfficialSeconds = model.Where(x => !x.IsPrivate && x.IsPartiallyConfirmed).Select(x => x.TotalDurationInSeconds).Sum();
            model.First().TotalBillOfficialCost = Convert.ToDouble(String.Format("{0:0.00}", model.Where(x => !x.IsPrivate && x.IsPartiallyConfirmed).Select(x => x.TotalCallCost).Sum()));


            model.First().TotalBillPrivateMinutes = model.Where(x => x.IsPrivate && x.IsPartiallyConfirmed).Select(x => x.TotalDurationMinutes).Sum();
            model.First().TotalBillPrivateSeconds = model.Where(x => x.IsPrivate && x.IsPartiallyConfirmed).Select(x => x.TotalDurationInSeconds).Sum();
            model.First().TotalBillPrivateCost = Convert.ToDouble(String.Format("{0:0.00}", model.Where(x => x.IsPrivate && x.IsPartiallyConfirmed).Select(x => x.TotalCallCost).Sum()));

            model.First().RecordsConfirmedCount = model.Where(x => x.IsPartiallyConfirmed).Count();

            var temp = (from a in DbTBS.dataUserBill.Where(x => x.Active && x.UserBillGUID == PK)
                        select a).FirstOrDefault();
            model.First().PayInCashAmount = temp.PayInCashAmount.Value;
            model.First().DeductFromSalaryAmount = temp.DeductFromSalaryAmount.Value;

            //if (model.First().IsEntireBillConfirmed)
            //{
            //    var temp = (from a in DbTBS.dataUserBill.Where(x => x.Active && x.UserBillGUID == PK)
            //                select a).FirstOrDefault();
            //    model.First().PayInCashAmount = temp.PayInCashAmount.Value;
            //    model.First().DeductFromSalaryAmount = temp.DeductFromSalaryAmount.Value;

            //}
            //else
            //{
            //    model.First().PayInCashAmount = 0;
            //    model.First().DeductFromSalaryAmount = 0;
            //}

            return View("~/Areas/TBS/Views/UserBills/_UserBillForm.cshtml", model);


        }


        [Route("TBS/UserBills/Update/{PK}")]
        public ActionResult UserBillDetailsUpdate(Guid PK)
        {
            var userguidofbill = (from a in DbTBS.dataUserBill where a.UserBillGUID == PK select a.UserGUID).FirstOrDefault();
            if(userguidofbill != UserGUID)
            {
                throw new HttpException(401, "Unauthorized access");
            }
            //Add First Open To DB
            List<OwnUserBillConfirmModel> model = (from a in DbTBS.v_UserBillDetailsGrouped.Where(x => x.UserBillGUID == PK)
                                                   where
                                                  a.TotalCallCost > 0 &&
                                                  (a.CalledNumber != "INTERNET" && a.CalledNumber != "net.syriatel.com")
                                                   select new OwnUserBillConfirmModel
                                                   {
                                                       EDMXID = a.EDMXID,
                                                       UserBillGUID = a.UserBillGUID,
                                                       CallingNumber = a.CallingNumber,
                                                       CalledNumber = a.CalledNumber,
                                                       NumberOfCalls = a.NumberOfCalls.Value,
                                                       CalledStaffName = a.CalledStaffName,
                                                       Comments = a.Comments,

                                                       TotalDurationMinutes = a.TotalDurationMinutes.HasValue ? a.TotalDurationMinutes.Value : 0,
                                                       TotalDurationInSeconds = a.TotalDurationInSeconds.HasValue ? a.TotalDurationInSeconds.Value : 0,
                                                       TotalCallCost = a.TotalCallCost.HasValue ? a.TotalCallCost.Value : 0,

                                                       IsPartiallyConfirmed = a.IsFirstOpen ? (a.LastConfirmation.HasValue ? true : false) : a.IsConfirmed,
                                                       IsPrivate = a.IsFirstOpen ? (a.LastConfirmation.HasValue ? (a.LastConfirmation.Value == true ? true : false) : a.IsPrivate) : a.IsPrivate,
                                                       LastConfirmation = a.LastConfirmation.HasValue ? (a.LastConfirmation.Value == true ? "Private" : "Official") : "N/A",
                                                       AutomatedBySystem = (a.IsFirstOpen && a.LastConfirmation.HasValue) ? true : false,

                                                       UserOverride = a.UserOverride,
                                                       IsBillLocked = a.BillLocked
                                                   }).ToList();

            var currentUserTags = (from a in DbTBS.dataStaffCallsTag where a.UserGUID == UserGUID select a).ToList();
            foreach (var item in model)
            {
                item.Comments = (from a in currentUserTags where a.CallingNumber == item.CallingNumber && a.CalledNumber == item.CalledNumber select a.TagText).FirstOrDefault();
            }


            model.First().TotalBillMinutes = model.Select(x => x.TotalDurationMinutes).Sum();
            model.First().TotalBillSeconds = model.Select(x => x.TotalDurationInSeconds).Sum();
            model.First().TotalBillCost = Convert.ToDouble(String.Format("{0:0.00}", model.Select(x => x.TotalCallCost).Sum()));

            model.First().TotalBillOfficialMinutes = model.Where(x => !x.IsPrivate && x.IsPartiallyConfirmed).Select(x => x.TotalDurationMinutes).Sum();
            model.First().TotalBillOfficialSeconds = model.Where(x => !x.IsPrivate && x.IsPartiallyConfirmed).Select(x => x.TotalDurationInSeconds).Sum();
            model.First().TotalBillOfficialCost = Convert.ToDouble(String.Format("{0:0.00}", model.Where(x => !x.IsPrivate && x.IsPartiallyConfirmed).Select(x => x.TotalCallCost).Sum()));


            model.First().TotalBillPrivateMinutes = model.Where(x => x.IsPrivate && x.IsPartiallyConfirmed).Select(x => x.TotalDurationMinutes).Sum();
            model.First().TotalBillPrivateSeconds = model.Where(x => x.IsPrivate && x.IsPartiallyConfirmed).Select(x => x.TotalDurationInSeconds).Sum();
            model.First().TotalBillPrivateCost = Convert.ToDouble(String.Format("{0:0.00}", model.Where(x => x.IsPrivate && x.IsPartiallyConfirmed).Select(x => x.TotalCallCost).Sum()));

            model.First().RecordsConfirmedCount = model.Where(x => x.IsPartiallyConfirmed).Count();

            var temp = (from a in DbTBS.dataUserBill.Where(x => x.Active && x.UserBillGUID == PK)
                        select a).FirstOrDefault();
            model.First().PayInCashAmount = temp.PayInCashAmount.Value;
            model.First().DeductFromSalaryAmount = temp.DeductFromSalaryAmount.Value;


            //get confirmed by system
            var automatedBySystem = model.Where(x => x.AutomatedBySystem == true).Select(x => new
            {
                x.UserBillGUID,
                x.CallingNumber,
                x.CalledNumber,
                x.IsPrivate,
                x.IsPartiallyConfirmed,
                x.TotalBillPrivateCost,
                x.TotalBillOfficialCost,
                x.TotalBillPrivateMinutes,
                x.TotalBillOfficialMinutes
            }).ToList();

            List<dataUserBillDetail> dataUserBillDetail = (from a in DbTBS.dataUserBillDetail
                                                           where a.UserBillGUID == PK && a.Active
                                                           select a).ToList();
            foreach (var item in automatedBySystem)
            {
                dataUserBillDetail.Where(x => x.CallingNumber == item.CallingNumber && x.CalledNumber == item.CalledNumber).ToList()
                    .ForEach(x => { x.IsPrivate = item.IsPrivate; x.IsConfirmed = true; });

                //DbTBS.dataUserBillDetail
                // .Where(x => x.UserBillGUID == PK && x.Active && x.CallingNumber == item.CallingNumber && x.CalledNumber == item.CalledNumber).ToList()
                // .ForEach(x =>
                // {
                //     x.IsPrivate = item.IsPrivate;
                //     x.dataUserBill.IsFirstOpen = false;
                //     x.IsConfirmed = true;
                // });
            }
            dataUserBillDetail.First().dataUserBill.IsFirstOpen = false;
            DbTBS.SaveChanges();
            DbCMS.SaveChanges();
            //if (model.First().IsEntireBillConfirmed)
            //{
            //    var temp = (from a in DbTBS.dataUserBill.Where(x => x.Active && x.UserBillGUID == PK)
            //                select a).FirstOrDefault();
            //    model.First().PayInCashAmount = temp.PayInCashAmount.Value;
            //    model.First().DeductFromSalaryAmount = temp.DeductFromSalaryAmount.Value;

            //}
            //else
            //{
            //    model.First().PayInCashAmount = 0;
            //    model.First().DeductFromSalaryAmount = 0;
            //}

            return View("~/Areas/TBS/Views/UserBills/_UserBillForm.cshtml", model);

        }


        public JsonResult GetUserBillDetailsJson(Guid PK)
        {
            UserBillsUpdateModel model = (from a in DbTBS.dataUserBill.Where(x => x.UserBillGUID == PK && x.Active)
                                          join b in DbTBS.dataUserBillDetail.Where(x => x.Active) on a.UserBillGUID equals b.UserBillGUID
                                          group b by new
                                          {
                                              a.BillGUID,
                                              a.UserGUID,
                                              a.UserBillGUID,
                                              a.ConfirmationDate,
                                              a.IsConfirmed,
                                              a.Active,
                                              a.dataUserBillRowVersion,
                                          } into c
                                          select new UserBillsUpdateModel
                                          {
                                              BillGUID = c.Key.BillGUID,
                                              UserBillGUID = c.Key.UserBillGUID,
                                              UserGUID = c.Key.UserGUID,
                                              ConfirmationDate = c.Key.ConfirmationDate,
                                              Active = c.Key.Active,
                                              dataUserBillRowVersion = c.Key.dataUserBillRowVersion,
                                              IsConfirmed = c.Key.IsConfirmed,
                                              dataUserBillDetailModel = (from cc in c
                                                                         select new dataUserBillDetailModel
                                                                         {
                                                                             UserBillDetailGUID = cc.UserBillDetailGUID,
                                                                             IsPrivate = (from aa in DbTBS.dataUserCustomPhoneDirectory.Where(x => x.Active && x.UserGUID == c.Key.UserGUID)
                                                                                          where aa.DestinationNumber == cc.CalledNumber
                                                                                          select aa.IsPrivate).FirstOrDefault() == true ? true : false,
                                                                             DestinationHolderName = (from aa in DbTBS.dataUserCustomPhoneDirectory.Where(x => x.Active && x.UserGUID == c.Key.UserGUID)
                                                                                                      where aa.DestinationNumber == cc.CalledNumber
                                                                                                      select aa.DestinationHolderName).FirstOrDefault(),
                                                                             UserBillGUID = cc.UserBillGUID,
                                                                             CallingNumber = cc.CallingNumber,
                                                                             CalledNumber = cc.CalledNumber,
                                                                             dateTimeConnect = cc.dateTimeConnect,
                                                                             dateTimeDisconnect = cc.dateTimeDisconnect,
                                                                             CallType = cc.CallType,
                                                                             CallTypeDescription = (from a in DbTBS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN
                                                                                                    && x.ValueGUID == cc.CallType)
                                                                                                    select a.ValueDescription).FirstOrDefault(),


                                                                             CallSource = cc.CallSource,
                                                                             Service = cc.Service,
                                                                             DurationInSeconds = cc.DurationInSeconds,
                                                                             DurationInMinutes = cc.DurationInMinutes,
                                                                             CallCost = cc.CallCost,
                                                                             Comments = cc.Comments,
                                                                             Active = cc.Active
                                                                         }).ToList()
                                          }).FirstOrDefault();

            model.TotalCost = model.dataUserBillDetailModel.Sum(x => x.CallCost);
            model.TotalMinutes = model.dataUserBillDetailModel.Sum(x => x.DurationInMinutes);
            model.TotalSeconds = model.dataUserBillDetailModel.Sum(x => x.DurationInSeconds);

            model.PrivateCost = model.dataUserBillDetailModel.Where(x => x.IsPrivate).Sum(x => x.CallCost);
            model.PrivateMinutes = model.dataUserBillDetailModel.Where(x => x.IsPrivate).Sum(x => x.DurationInMinutes);
            model.PrivateSeconds = model.dataUserBillDetailModel.Where(x => x.IsPrivate).Sum(x => x.DurationInSeconds);

            model.OfficialCost = model.dataUserBillDetailModel.Where(x => !x.IsPrivate).Sum(x => x.CallCost);
            model.OfficialMinutes = model.dataUserBillDetailModel.Where(x => !x.IsPrivate).Sum(x => x.DurationInMinutes);
            model.OfficialSeconds = model.dataUserBillDetailModel.Where(x => !x.IsPrivate).Sum(x => x.DurationInSeconds);


            return Json(new
            {
                TableData = model,
                TotalCost = model.TotalCost,
                TotalMinutes = model.TotalMinutes,
                TotalSeconds = model.TotalSeconds,
                PrivateCost = model.PrivateCost,
                PrivateMinutes = model.PrivateMinutes,
                PrivateSeconds = model.PrivateSeconds,
                OfficialCost = model.OfficialCost,
                OfficialMinutes = model.OfficialMinutes,
                OfficialSeconds = model.OfficialSeconds
            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ShowMoreCallInfo(Guid UserBillGUID, string CallingNumber, string CalledNumber)
        {
            var result = (from a in DbTBS.dataUserBillDetail
                          join b in DbTBS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.CallType equals b.ValueGUID
                          where a.UserBillGUID == UserBillGUID
                          && a.CallingNumber == CallingNumber
                          && a.CalledNumber == CalledNumber
                          && a.Active
                          orderby a.dateTimeConnect descending
                          select new
                          {
                              a.CallingNumber,
                              a.CalledNumber,
                              a.dateTimeConnect,
                              b.ValueDescription,
                              a.DurationInMinutes,
                              a.DurationInSeconds,
                              a.CallCost
                          }).ToList();
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UserBillDetailsUpdate(UserBillsUpdateModel model)
        {
            try
            {
                Guid ActionGUID = Permissions.TelecomCompanies.UpdateGuid;
                DateTime ExecutionTime = DateTime.Now;
                dataUserBill dataUserBill = (from a in DbTBS.dataUserBill.Where(x => x.UserBillGUID == model.UserBillGUID && x.Active) select a).FirstOrDefault();
                dataUserBill.ConfirmationDate = DateTime.Now;
                dataUserBill.IsConfirmed = true;

                DbTBS.Update(dataUserBill, ActionGUID, ExecutionTime, DbCMS);
                List<dataUserCustomPhoneDirectory> dataUserCustomPhoneDirectory = new List<dataUserCustomPhoneDirectory>();


                var toDirectory = (from a in model.dataUserBillDetailModel
                                   select new { a.IsPrivate, a.CalledNumber, a.DestinationHolderName }).Distinct().ToList();

                //var toDirectory = (from a in model.dataUserBillDetailModel
                //                   where a.IsPrivate
                //                   || (a.DestinationHolderName != null && a.DestinationHolderName.Length > 0)
                //                   select a).ToList();
                foreach (var item in toDirectory)
                {

                    var found = (from a in DbTBS.dataUserCustomPhoneDirectory
                                 where a.UserGUID == model.UserGUID
                                 && a.DestinationNumber == item.CalledNumber
                                 && a.Active
                                 select a).FirstOrDefault();

                    if (found == null)
                    {
                        if (dataUserCustomPhoneDirectory.Where(x => x.DestinationHolderName == item.DestinationHolderName && x.DestinationNumber == item.CalledNumber).Count() > 0)
                        {
                            continue;
                        }
                        dataUserCustomPhoneDirectory ducpd = new dataUserCustomPhoneDirectory();
                        ducpd.DestinationHolderName = item.DestinationHolderName;
                        ducpd.IsPrivate = item.IsPrivate;
                        ducpd.UserGUID = model.UserGUID;
                        ducpd.DestinationNumber = item.CalledNumber;
                        dataUserCustomPhoneDirectory.Add(ducpd);
                    }
                    //check if exists -> update
                    else
                    {
                        found.IsPrivate = item.IsPrivate;
                        found.DestinationHolderName = item.DestinationHolderName;
                        found.DestinationNumber = item.CalledNumber;
                    }

                }

                DbTBS.CreateBulk(dataUserCustomPhoneDirectory, ActionGUID, ExecutionTime, DbCMS);

                var privateGUIDs = model.dataUserBillDetailModel.Where(x => x.IsPrivate).Select(x => x.UserBillDetailGUID).ToList();
                var officialsGUIDs = model.dataUserBillDetailModel.Where(x => !x.IsPrivate).Select(x => x.UserBillDetailGUID).ToList();

                DbTBS.dataUserBillDetail.Where(x => privateGUIDs.Contains(x.UserBillDetailGUID)).ToList().ForEach(x => x.IsPrivate = true);
                DbTBS.dataUserBillDetail.Where(x => officialsGUIDs.Contains(x.UserBillDetailGUID)).ToList().ForEach(x => x.IsPrivate = false);

                try
                {
                    DbTBS.SaveChanges();
                    DbCMS.SaveChanges();
                    return Json(DbTBS.SingleUpdateMessage());
                }
                catch (Exception ex)
                {
                    return Json(DbTBS.ErrorMessage(ex.Message));
                }
            }
            catch (Exception ex)
            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost]
        public ActionResult PartialConfirmPrivateOfficial(Guid UserBillGUID, string CallingNumber, string CalledNumber, string type)
        {
            var dataToUpdate = (from a in DbTBS.dataUserBillDetail
                                where a.UserBillGUID == UserBillGUID
                                && a.CallingNumber == CallingNumber
                                && a.CalledNumber == CalledNumber
                                && a.Active
                                select a).ToList();
            dataStaffPrivateCall dataStaffPrivateCall = new dataStaffPrivateCall();
            dataStaffPrivateCall.StaffPrivateCallsGUID = Guid.NewGuid();
            dataStaffPrivateCall.UserGUID = UserGUID;
            dataStaffPrivateCall.DestinationNumber = CalledNumber;
            dataStaffPrivateCall.CreatedOn = DateTime.Now;
            dataStaffPrivateCall.Active = true;
            if (type == "private")
            {
                dataStaffPrivateCall.IsPrivate = true;
                dataToUpdate.ForEach(x => { x.IsPrivate = true; x.IsConfirmed = true; });
            }
            else if (type == "official")
            {
                dataStaffPrivateCall.IsPrivate = false;
                dataToUpdate.ForEach(x => { x.IsPrivate = false; x.IsConfirmed = true; });
            }
            DbTBS.dataStaffPrivateCall.Add(dataStaffPrivateCall);


            try
            {
                DbTBS.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }


        }

        [HttpPost]
        public ActionResult UserTagsUpdage(string Tag, string CallingNumber, string CalledNumber)
        {
            dataStaffCallsTag dataStaffCallsTag = (from a in DbTBS.dataStaffCallsTag
                                                   where a.UserGUID == UserGUID && a.CallingNumber == CallingNumber && a.CalledNumber == CalledNumber
                                                   select a).FirstOrDefault();

            if (dataStaffCallsTag != null)
            {
                dataStaffCallsTag.TagText = Tag;
            }
            else
            {
                dataStaffCallsTag NewdataStaffCallsTag = new dataStaffCallsTag();
                NewdataStaffCallsTag.StaffCallTagGUID = Guid.NewGuid();
                NewdataStaffCallsTag.UserGUID = UserGUID;
                NewdataStaffCallsTag.CallingNumber = CallingNumber;
                NewdataStaffCallsTag.CalledNumber = CalledNumber;
                NewdataStaffCallsTag.TagText = Tag;
                NewdataStaffCallsTag.Active = true;
                DbTBS.dataStaffCallsTag.Add(NewdataStaffCallsTag);
            }

            try
            {
                DbTBS.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }
        public bool IsBillConfirmed(Guid UserBillGUID)
        {
            var notConfirmedCount = (from a in DbTBS.dataUserBillDetail.Where(x => x.Active && x.UserBillGUID == UserBillGUID)
                                     select a).Count();
            if (notConfirmedCount > 0)
            {
                return false;
            }
            else
            {
                MakeBillConfirmed(UserBillGUID);
            }
            return true;
        }

        public void MakeBillConfirmed(Guid UserBillGUID)
        {
            dataUserBill dataUserBill = (from a in DbTBS.dataUserBill.Where(x => x.Active && x.UserBillGUID == UserBillGUID)
                                         select a).FirstOrDefault();
            dataUserBill.IsConfirmed = true;
            dataUserBill.DeductFromSalaryAmount = dataUserBill.dataUserBillDetail.Where(x => x.Active && x.IsPrivate).Select(x => x.CallCost).Sum();
            DbTBS.SaveChanges();
        }

        [HttpPost]
        public ActionResult FullConfirmPrivateOfficial(Guid UserBillGUID, string type)
        {
            DateTime ExecutionTime = DateTime.Now;
            var dataToUpdate = (from a in DbTBS.dataUserBillDetail
                                where a.UserBillGUID == UserBillGUID
                                && a.Active
                                && a.IsConfirmed == false
                                select a).ToList();
            var distinctNumbers = (from a in dataToUpdate
                                   select a.CalledNumber).Distinct().ToList();
            List<dataStaffPrivateCall> dataStaffPrivateCalls = new List<dataStaffPrivateCall>();
            foreach (var number in distinctNumbers)
            {
                dataStaffPrivateCall dataStaffPrivateCall = new dataStaffPrivateCall();
                dataStaffPrivateCall.StaffPrivateCallsGUID = Guid.NewGuid();
                dataStaffPrivateCall.UserGUID = UserGUID;
                dataStaffPrivateCall.DestinationNumber = number;
                dataStaffPrivateCall.CreatedOn = ExecutionTime;
                dataStaffPrivateCall.Active = true;
                if (type == "private")
                {
                    dataStaffPrivateCall.IsPrivate = true;
                }
                else if (type == "official")
                {
                    dataStaffPrivateCall.IsPrivate = false;
                }
                dataStaffPrivateCalls.Add(dataStaffPrivateCall);
            }
            if (type == "private")
            {
                dataToUpdate.ForEach(x => { x.IsPrivate = true; x.IsConfirmed = true; });
            }
            else if (type == "official")
            {
                dataToUpdate.ForEach(x => { x.IsPrivate = false; x.IsConfirmed = true; });
            }
            DbTBS.dataStaffPrivateCall.AddRange(dataStaffPrivateCalls);
            try
            {
                DbTBS.SaveChanges();
                return Json(new { success = true, isEntireBillConfirmed = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }


        }

        [HttpPost]
        public ActionResult AuthorizeDeductionAmounts(Guid UserBillGUID, float DeductFromSalaryAmount, float CashPayAmount)
        {

            dataUserBill dataUserBill = (from a in DbTBS.dataUserBill.Where(x => x.Active && x.UserBillGUID == UserBillGUID)
                                         select a).FirstOrDefault();
            dataUserBill.IsConfirmed = true;
            dataUserBill.ConfirmationDate = DateTime.Now;
            dataUserBill.dataUserBillDetail.Where(x => x.CallCost == 0).ToList().ForEach(x => { x.IsPrivate = false; x.IsConfirmed = true; });

            var totalPrivateCallCost = dataUserBill.dataUserBillDetail.Where(x => x.IsPrivate).Select(x => x.CallCost).Sum();
            Double roundedTotalPrivateCallCost = Math.Round((Double)totalPrivateCallCost, 2);

            if (CashPayAmount < 0)
            {
                return Json(new { success = false });
            }
            if (DeductFromSalaryAmount < 0)
            {
                return Json(new { success = false });
            }
            if (Math.Round((Double)CashPayAmount + DeductFromSalaryAmount, 2) > roundedTotalPrivateCallCost)
            {
                return Json(new { success = false });
            }
            if (CashPayAmount > 0)
            {
                dataUserBill.DoPayInCash = true;
            }

            dataUserBill.PayInCashAmount = CashPayAmount;
            dataUserBill.DeductFromSalaryAmount = DeductFromSalaryAmount;

            int BillMonth = dataUserBill.dataBill.BillForMonth;
            int BillYear = dataUserBill.dataBill.BillForYear;

            string TelecomCompanyName = (from a in DbTBS.configTelecomCompanyOperation
                                         join b in DbTBS.codeTelecomCompanyOperation on a.TelecomCompanyOperationGUID equals b.TelecomCompanyOperationGUID
                                         join c in DbTBS.codeTelecomCompanyLanguages on b.TelecomCompanyGUID equals c.TelecomCompanyGUID
                                         where c.LanguageID == "EN"
                                         && a.TelecomCompanyOperationConfigGUID == dataUserBill.dataBill.TelecomCompanyOperationConfigGUID
                                         select c.TelecomCompanyDescription).FirstOrDefault();

            string StaffName = (from a in DbTBS.userPersonalDetailsLanguage.Where(x => x.UserGUID == dataUserBill.UserGUID && x.LanguageID == "EN" && x.Active)
                                select a.FirstName + "_" + a.Surname).FirstOrDefault();

            List<dataUserBillDetail> dataUserBillDetails = dataUserBill.dataUserBillDetail.ToList();
            var callTypes = (from a in DbCMS.codeTablesValuesLanguages
                             join b in DbCMS.codeTablesValues on a.ValueGUID equals b.ValueGUID
                             where b.TableGUID == LookupTables.PhoneCallTypes
                             && a.Active && a.LanguageID == "EN"
                             select a).ToList();
            //generate file and send email

            string sourceFile = Server.MapPath("~/Areas/TBS/ReportTemplate/TBS_Receipt.xlsx");




            string receiptFileName = "TBS_Receipt_" + StaffName + "_" + TelecomCompanyName + "_" + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(BillMonth) + "_" + BillYear + "_" + DateTime.Now.ToBinary() + ".xlsx";

            string DisFolder = Server.MapPath("~/Areas/TBS/GeneratedReports/" + receiptFileName);

            System.IO.File.Copy(sourceFile, DisFolder);

            using (var package = new ExcelPackage(new FileInfo(DisFolder)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                worksheet.Cells["B2"].Value = TelecomCompanyName;

                worksheet.Cells["B5"].Value = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(BillMonth) + " ," + BillYear;

                worksheet.Cells["C8"].Value = dataUserBillDetails.Where(x => x.IsPrivate == false).Sum(x => x.DurationInMinutes);
                worksheet.Cells["C9"].Value = dataUserBillDetails.Where(x => x.IsPrivate == false).Sum(x => x.CallCost);

                worksheet.Cells["F8"].Value = dataUserBillDetails.Where(x => x.IsPrivate == true).Sum(x => x.DurationInMinutes);
                worksheet.Cells["F9"].Value = dataUserBillDetails.Where(x => x.IsPrivate == true).Sum(x => x.CallCost);

                worksheet.Cells["K8"].Value = dataUserBillDetails.Sum(x => x.DurationInMinutes);
                worksheet.Cells["K9"].Value = dataUserBillDetails.Sum(x => x.CallCost);

                worksheet.Cells["K4"].Value = dataUserBill.DeductFromSalaryAmount;
                worksheet.Cells["K5"].Value = dataUserBill.PayInCashAmount;


                int i = 14;
                foreach (var record in dataUserBillDetails)
                {
                    worksheet.Cells["B" + i].Value = record.dateTimeConnect.ToString("MMMM dd yyyy"); //Date
                    worksheet.Cells["C" + i].Value = record.dateTimeConnect.ToString("h:mm tt"); //From
                    worksheet.Cells["D" + i].Value = record.dateTimeDisconnect.HasValue ? record.dateTimeDisconnect.Value.ToString("h:mm tt") : record.dateTimeConnect.AddSeconds(record.DurationInSeconds).ToString("h:mm tt"); //To
                    worksheet.Cells["E" + i].Value = record.DurationInSeconds; //Duration In Seconds
                    worksheet.Cells["F" + i].Value = record.DurationInMinutes; //Charge Time In Minutes/KB
                    worksheet.Cells["G" + i].Value = record.CallCost; //Cost Value
                    worksheet.Cells["H" + i].Value = record.CallingNumber.ToString(); //Calling Number
                    worksheet.Cells["I" + i].Value = record.CalledNumber.ToString(); //Called Number
                    worksheet.Cells["J" + i].Value = callTypes.Where(x => x.ValueGUID == record.CallType).Select(x => x.ValueDescription).FirstOrDefault().ToString(); //Calling Type
                    worksheet.Cells["K" + i].Value = record.IsPrivate ? "Private" : "Official";
                    worksheet.Cells["L" + i].Value = record.CalledStaffName == null ? "" : record.CalledStaffName.ToString(); //Caller Identity

                    i++;
                }

                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                worksheet.Column(6).AutoFit();
                worksheet.Column(7).AutoFit();
                worksheet.Column(8).AutoFit();
                worksheet.Column(9).AutoFit();
                worksheet.Column(10).AutoFit();
                worksheet.Column(11).AutoFit();
                worksheet.Column(12).AutoFit();


                //number with 2 decimal places and thousand separator
                //worksheet.Cells["C8"].Style.Numberformat.Format = "#,##0.00";
                //worksheet.Cells["C9"].Style.Numberformat.Format = "#,##0.00";
                //worksheet.Cells["F9"].Style.Numberformat.Format = "#,##0.00";
                //worksheet.Cells["F9"].Style.Numberformat.Format = "#,##0.00";
                //worksheet.Cells["K4"].Style.Numberformat.Format = "#,##0.00";
                //worksheet.Cells["K5"].Style.Numberformat.Format = "#,##0.00";
                //worksheet.Cells["K8"].Style.Numberformat.Format = "#,##0.00";
                //worksheet.Cells["K9"].Style.Numberformat.Format = "#,##0.00";
                //worksheet.Cells["E14:E" + i.ToString()].Style.Numberformat.Format = "#,##0.00";
                //worksheet.Cells["F14:F" + i.ToString()].Style.Numberformat.Format = "#,##0.00";
                //worksheet.Cells["G14:G" + i.ToString()].Style.Numberformat.Format = "#,##0.00";



                //ExcelRange range = worksheet.Cells[12, 2, worksheet.Dimension.End.Row, 12];
                //ExcelTable tab = worksheet.Tables.Add(range, "Table1");
                //tab.ShowHeader = true;
                //tab.TableStyle = TableStyles.Medium9;
                //tab.ShowFilter = true;

                package.Save();
            }


            try
            {
                DbTBS.SaveChanges();
                return Json(new { success = true, receiptFileName = receiptFileName });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }

        [Route("TBS/UserBills/UpdatePrivate/{PK}")]
        public ActionResult UserPrivateCallsUpdate(Guid PK)
        {
            List<dataStaffPrivateCall> model = (from a in DbTBS.dataStaffPrivateCall.Where(x => x.UserGUID == UserGUID && x.Active) select a).ToList();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("WHERE", "WHERE", new { Area = "TBS" }));

            return View("~/Areas/TBS/Views/UserBills/_UserPrivateCalls.cshtml", model);

        }

        public ActionResult DownloadUserGuide()
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(@"" + Server.MapPath("~/Areas/TBS/UserManual/Telephone Bills_V2_Usermanual.docx") + "");
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Telephone Bills_V2_Usermanual.docx");
        }

        public ActionResult ReceiptFileDownload(string FileName)
        {

            string fileDownloadPath = Server.MapPath("~/Areas/TBS/GeneratedReports/" + FileName);
            string ext = FileName.Split('.').Last();
            string fileName = FileName;
            byte[] fileBytes = System.IO.File.ReadAllBytes(@"" + fileDownloadPath + "");
            try
            {
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        [Route("TBS/UserBills/ReceiptDownloadFromDT/{PK}")]
        public ActionResult ReceiptDownloadFromDT(Guid PK)
        {

            dataUserBill dataUserBill = (from a in DbTBS.dataUserBill.Where(x => x.Active && x.UserBillGUID == PK)
                                         select a).FirstOrDefault();

            string StaffName = (from a in DbTBS.userPersonalDetailsLanguage.Where(x => x.UserGUID == dataUserBill.UserGUID && x.LanguageID == "EN" && x.Active)
                                select a.FirstName + "_" + a.Surname).FirstOrDefault();

            int BillMonth = dataUserBill.dataBill.BillForMonth;
            int BillYear = dataUserBill.dataBill.BillForYear;


            string TelecomCompanyName = (from a in DbTBS.configTelecomCompanyOperation
                                         join b in DbTBS.codeTelecomCompanyOperation on a.TelecomCompanyOperationGUID equals b.TelecomCompanyOperationGUID
                                         join c in DbTBS.codeTelecomCompanyLanguages on b.TelecomCompanyGUID equals c.TelecomCompanyGUID
                                         where c.LanguageID == "EN"
                                         && a.TelecomCompanyOperationConfigGUID == dataUserBill.dataBill.TelecomCompanyOperationConfigGUID
                                         select c.TelecomCompanyDescription).FirstOrDefault();

            List<dataUserBillDetail> dataUserBillDetails = dataUserBill.dataUserBillDetail.ToList();
            var callTypes = (from a in DbCMS.codeTablesValuesLanguages
                             join b in DbCMS.codeTablesValues on a.ValueGUID equals b.ValueGUID
                             where b.TableGUID == LookupTables.PhoneCallTypes
                             && a.Active && a.LanguageID == "EN"
                             select a).ToList();
            //generate file and send email

            string sourceFile = Server.MapPath("~/Areas/TBS/ReportTemplate/TBS_Receipt.xlsx");

            string receiptFileName = "TBS_Receipt_" + StaffName + "_" + TelecomCompanyName + "_" + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(BillMonth) + "_" + BillYear + "_" + DateTime.Now.ToBinary() + ".xlsx";

            string DisFolder = Server.MapPath("~/Areas/TBS/GeneratedReports/" + receiptFileName);

            System.IO.File.Copy(sourceFile, DisFolder);

            using (var package = new ExcelPackage(new FileInfo(DisFolder)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                worksheet.Cells["B2"].Value = TelecomCompanyName;

                worksheet.Cells["B5"].Value = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(BillMonth) + " ," + BillYear;

                worksheet.Cells["C8"].Value = dataUserBillDetails.Where(x => x.IsPrivate == false).Sum(x => x.DurationInMinutes);
                worksheet.Cells["C9"].Value = dataUserBillDetails.Where(x => x.IsPrivate == false).Sum(x => x.CallCost);

                worksheet.Cells["F8"].Value = dataUserBillDetails.Where(x => x.IsPrivate == true).Sum(x => x.DurationInMinutes);
                worksheet.Cells["F9"].Value = dataUserBillDetails.Where(x => x.IsPrivate == true).Sum(x => x.CallCost);

                worksheet.Cells["K8"].Value = dataUserBillDetails.Sum(x => x.DurationInMinutes);
                worksheet.Cells["K9"].Value = dataUserBillDetails.Sum(x => x.CallCost);

                worksheet.Cells["K4"].Value = dataUserBill.DeductFromSalaryAmount;
                worksheet.Cells["K5"].Value = dataUserBill.PayInCashAmount;


                int i = 14;
                foreach (var record in dataUserBillDetails)
                {
                    worksheet.Cells["B" + i].Value = record.dateTimeConnect.ToString("MMMM dd yyyy"); //Date
                    worksheet.Cells["C" + i].Value = record.dateTimeConnect.ToString("h:mm tt"); //From
                    worksheet.Cells["D" + i].Value = record.dateTimeDisconnect.HasValue ? record.dateTimeDisconnect.Value.ToString("h:mm tt") : record.dateTimeConnect.AddSeconds(record.DurationInSeconds).ToString("h:mm tt"); //To
                    worksheet.Cells["E" + i].Value = record.DurationInSeconds; //Duration In Seconds
                    worksheet.Cells["F" + i].Value = record.DurationInMinutes; //Charge Time In Minutes/KB
                    worksheet.Cells["G" + i].Value = record.CallCost; //Cost Value
                    worksheet.Cells["H" + i].Value = record.CallingNumber.ToString(); //Calling Number
                    worksheet.Cells["I" + i].Value = record.CalledNumber.ToString(); //Called Number
                    worksheet.Cells["J" + i].Value = callTypes.Where(x => x.ValueGUID == record.CallType).Select(x => x.ValueDescription).FirstOrDefault().ToString(); //Calling Type
                    worksheet.Cells["K" + i].Value = record.IsPrivate ? "Private" : "Official";
                    worksheet.Cells["L" + i].Value = record.CalledStaffName == null ? "" : record.CalledStaffName.ToString(); //Caller Identity

                    i++;
                }

                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                worksheet.Column(6).AutoFit();
                worksheet.Column(7).AutoFit();
                worksheet.Column(8).AutoFit();
                worksheet.Column(9).AutoFit();
                worksheet.Column(10).AutoFit();
                worksheet.Column(11).AutoFit();
                worksheet.Column(12).AutoFit();

                package.Save();
            }


            try
            {
                string fileDownloadPath = Server.MapPath("~/Areas/TBS/GeneratedReports/" + receiptFileName);
                string ext = receiptFileName.Split('.').Last();
                byte[] fileBytes = System.IO.File.ReadAllBytes(@"" + fileDownloadPath + "");
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, receiptFileName);
            }
            catch (Exception ex)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        #region unit test
        public ActionResult TestExcelReceipt()
        {
            Guid UserBillGUID = Guid.Parse("D2154BB8-9041-48AE-BDA3-E7B87C9462C2");
            dataUserBill dataUserBill = (from a in DbTBS.dataUserBill.Where(x => x.Active && x.UserBillGUID == UserBillGUID)
                                         select a).FirstOrDefault();

            List<dataUserBillDetail> dataUserBillDetails = dataUserBill.dataUserBillDetail.ToList();
            var callTypes = (from a in DbCMS.codeTablesValuesLanguages
                             join b in DbCMS.codeTablesValues on a.ValueGUID equals b.ValueGUID
                             where b.TableGUID == LookupTables.PhoneCallTypes
                             && a.Active && a.LanguageID == "EN"
                             select a).ToList();

            int BillMonth = dataUserBill.dataBill.BillForMonth;
            int BillYear = dataUserBill.dataBill.BillForYear;

            string TelecomCompanyName = (from a in DbTBS.configTelecomCompanyOperation
                                         join b in DbTBS.codeTelecomCompanyOperation on a.TelecomCompanyOperationGUID equals b.TelecomCompanyOperationGUID
                                         join c in DbTBS.codeTelecomCompanyLanguages on b.TelecomCompanyGUID equals c.TelecomCompanyGUID
                                         where c.LanguageID == "EN"
                                         && a.TelecomCompanyOperationConfigGUID == dataUserBill.dataBill.TelecomCompanyOperationConfigGUID
                                         select c.TelecomCompanyDescription).FirstOrDefault();

            //generate file and send email

            string sourceFile = Server.MapPath("~/Areas/TBS/ReportTemplate/TBS_Receipt.xlsx");

            string DisFolder = Server.MapPath("~/Areas/TBS/GeneratedReports/TBS_Receipt" + DateTime.Now.ToBinary() + ".xlsx");

            System.IO.File.Copy(sourceFile, DisFolder);

            using (var package = new ExcelPackage(new FileInfo(DisFolder)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                worksheet.Cells["B2"].Value = TelecomCompanyName;

                worksheet.Cells["B5"].Value = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(BillMonth) + " ," + BillYear;

                worksheet.Cells["C8"].Value = dataUserBillDetails.Where(x => x.IsPrivate == false).Sum(x => x.DurationInMinutes);
                worksheet.Cells["C9"].Value = dataUserBillDetails.Where(x => x.IsPrivate == false).Sum(x => x.CallCost);

                worksheet.Cells["F8"].Value = dataUserBillDetails.Where(x => x.IsPrivate == true).Sum(x => x.DurationInMinutes);
                worksheet.Cells["F9"].Value = dataUserBillDetails.Where(x => x.IsPrivate == true).Sum(x => x.CallCost);

                worksheet.Cells["K8"].Value = dataUserBillDetails.Sum(x => x.DurationInMinutes);
                worksheet.Cells["K9"].Value = dataUserBillDetails.Sum(x => x.CallCost);

                worksheet.Cells["K4"].Value = dataUserBill.DeductFromSalaryAmount;
                worksheet.Cells["K5"].Value = dataUserBill.PayInCashAmount;


                int i = 14;
                foreach (var record in dataUserBillDetails)
                {
                    worksheet.Cells["B" + i].Value = record.dateTimeConnect.ToString("MMMM dd yyyy"); //Date
                    worksheet.Cells["C" + i].Value = record.dateTimeConnect.ToString("h:mm tt"); //From
                    worksheet.Cells["D" + i].Value = record.dateTimeDisconnect.HasValue ? record.dateTimeDisconnect.Value.ToString("h:mm tt") : record.dateTimeConnect.AddSeconds(record.DurationInSeconds).ToString("h:mm tt"); //To
                    worksheet.Cells["E" + i].Value = record.DurationInSeconds; //Duration In Seconds
                    worksheet.Cells["F" + i].Value = record.DurationInMinutes; //Charge Time In Minutes/KB
                    worksheet.Cells["G" + i].Value = record.CallCost; //Cost Value
                    worksheet.Cells["H" + i].Value = record.CallingNumber.ToString(); //Calling Number
                    worksheet.Cells["I" + i].Value = record.CalledNumber.ToString(); //Called Number
                    worksheet.Cells["J" + i].Value = callTypes.Where(x => x.ValueGUID == record.CallType).Select(x => x.ValueDescription).FirstOrDefault().ToString(); //Calling Type
                    worksheet.Cells["K" + i].Value = record.IsPrivate ? "Private" : "Official";
                    worksheet.Cells["L" + i].Value = record.CalledStaffName == null ? "" : record.CalledStaffName.ToString(); //Caller Identity

                    i++;
                }

                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                worksheet.Column(6).AutoFit();
                worksheet.Column(7).AutoFit();
                worksheet.Column(8).AutoFit();
                worksheet.Column(9).AutoFit();
                worksheet.Column(10).AutoFit();
                worksheet.Column(11).AutoFit();
                worksheet.Column(12).AutoFit();


                //number with 2 decimal places and thousand separator
                //worksheet.Cells["C8"].Style.Numberformat.Format = "#,##0.00";
                //worksheet.Cells["C9"].Style.Numberformat.Format = "#,##0.00";
                //worksheet.Cells["F9"].Style.Numberformat.Format = "#,##0.00";
                //worksheet.Cells["F9"].Style.Numberformat.Format = "#,##0.00";
                //worksheet.Cells["K4"].Style.Numberformat.Format = "#,##0.00";
                //worksheet.Cells["K5"].Style.Numberformat.Format = "#,##0.00";
                //worksheet.Cells["K8"].Style.Numberformat.Format = "#,##0.00";
                //worksheet.Cells["K9"].Style.Numberformat.Format = "#,##0.00";
                //worksheet.Cells["E14:E" + i.ToString()].Style.Numberformat.Format = "#,##0.00";
                //worksheet.Cells["F14:F" + i.ToString()].Style.Numberformat.Format = "#,##0.00";
                //worksheet.Cells["G14:G" + i.ToString()].Style.Numberformat.Format = "#,##0.00";



                //ExcelRange range = worksheet.Cells[12, 2, worksheet.Dimension.End.Row, 12];
                //ExcelTable tab = worksheet.Tables.Add(range, "Table1");
                //tab.ShowHeader = true;
                //tab.TableStyle = TableStyles.Medium9;
                //tab.ShowFilter = true;

                package.Save();
            }

            return null;
        }
        #endregion
    }


}


#region                    Trash

//UserBillsUpdateModel modelX = (from a in DbTBS.dataUserBill.Where(x => x.UserBillGUID == PK && x.Active)
//                               join b in DbTBS.dataUserBillDetail.Where(x => x.Active) on a.UserBillGUID equals b.UserBillGUID
//                               group b by new
//                               {
//                                   a.BillGUID,
//                                   a.UserGUID,
//                                   a.UserBillGUID,
//                                   a.ConfirmationDate,
//                                   a.IsConfirmed,
//                                   a.Active,
//                                   a.dataUserBillRowVersion,
//                               } into c
//                               select new UserBillsUpdateModel
//                               {
//                                   BillGUID = c.Key.BillGUID,
//                                   UserBillGUID = c.Key.UserBillGUID,
//                                   UserGUID = c.Key.UserGUID,
//                                   ConfirmationDate = c.Key.ConfirmationDate,
//                                   Active = c.Key.Active,
//                                   dataUserBillRowVersion = c.Key.dataUserBillRowVersion,
//                                   IsConfirmed = c.Key.IsConfirmed,
//                                   dataUserBillDetailModel = (from cc in c
//                                                              select new dataUserBillDetailModel
//                                                              {
//                                                                  UserBillDetailGUID = cc.UserBillDetailGUID,
//                                                                  IsPrivate = (from aa in DbTBS.dataUserCustomPhoneDirectory.Where(x => x.Active && x.UserGUID == c.Key.UserGUID)
//                                                                               where aa.DestinationNumber == cc.CalledNumber
//                                                                               select aa.IsPrivate).FirstOrDefault() == true ? true : false,
//                                                                  DestinationHolderName = (from aa in DbTBS.dataUserCustomPhoneDirectory.Where(x => x.Active && x.UserGUID == c.Key.UserGUID)
//                                                                                           where aa.DestinationNumber == cc.CalledNumber
//                                                                                           select aa.DestinationHolderName).FirstOrDefault(),
//                                                                  UserBillGUID = cc.UserBillGUID,
//                                                                  CallingNumber = cc.CallingNumber,
//                                                                  CalledNumber = cc.CalledNumber,
//                                                                  dateTimeConnect = cc.dateTimeConnect,
//                                                                  dateTimeDisconnect = cc.dateTimeDisconnect,
//                                                                  CallType = cc.CallType,
//                                                                  CallTypeDescription = (from a in DbTBS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN
//                                                                                         && x.ValueGUID == cc.CallType)
//                                                                                         select a.ValueDescription).FirstOrDefault(),


//                                                                  CallSource = cc.CallSource,
//                                                                  Service = cc.Service,
//                                                                  DurationInSeconds = cc.DurationInSeconds,
//                                                                  DurationInMinutes = cc.DurationInMinutes,
//                                                                  CallCost = cc.CallCost,
//                                                                  Comments = cc.Comments,
//                                                                  Active = cc.Active
//                                                              }).ToList()
//                               }).FirstOrDefault();

//modelX.TotalCost = modelX.dataUserBillDetailModel.Sum(x => x.CallCost);
//    modelX.TotalMinutes = modelX.dataUserBillDetailModel.Sum(x => x.DurationInMinutes);
//    modelX.TotalSeconds = modelX.dataUserBillDetailModel.Sum(x => x.DurationInSeconds);

//    modelX.PrivateCost = modelX.dataUserBillDetailModel.Where(x => x.IsPrivate).Sum(x => x.CallCost);
//modelX.PrivateMinutes = modelX.dataUserBillDetailModel.Where(x => x.IsPrivate).Sum(x => x.DurationInMinutes);
//modelX.PrivateSeconds = modelX.dataUserBillDetailModel.Where(x => x.IsPrivate).Sum(x => x.DurationInSeconds);

//modelX.OfficialCost = modelX.dataUserBillDetailModel.Where(x => !x.IsPrivate).Sum(x => x.CallCost);
//modelX.OfficialMinutes = modelX.dataUserBillDetailModel.Where(x => !x.IsPrivate).Sum(x => x.DurationInMinutes);
//modelX.OfficialSeconds = modelX.dataUserBillDetailModel.Where(x => !x.IsPrivate).Sum(x => x.DurationInSeconds);

//    if (modelX == null) throw new HttpException((int) HttpStatusCode.NotFound, Url.Action("Bills", "UserBills", new { Area = "TBS" }));

//    return View("~/Areas/TBS/Views/UserBills/_UserBillForm.cshtml", model);

#endregion