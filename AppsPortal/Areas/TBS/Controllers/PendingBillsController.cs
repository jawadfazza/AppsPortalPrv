using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using TBS_DAL.Model;

namespace AppsPortal.Areas.TBS.Controllers
{
    public class PendingBillsController : TBSBaseController
    {

        [Route("TBS/PendingBills/Index")]
        public ActionResult PendingBills()
        {
            return View("~/Areas/TBS/Views/PendingBills/Index.cshtml");
        }

        [Route("TBS/PendingBills/PendingBillsDataTable/")]
        public JsonResult PendingBillsDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<PendingBillsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<PendingBillsDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbTBS.v_PendingBills.AsNoTracking()
                       select new PendingBillsDataTableModel
                       {

                           UserBillGUID = a.UserBillGUID,
                           UserGUID = a.UserGUID.ToString(),
                           EmailAddress = a.EmailAddress,
                           FullName = a.FullName,
                           BillForTypeGUID = a.BillForTypeGUID.ToString(),
                           BillForTypeDescription = a.BillForTypeDescription,
                           BillForMonth = a.BillForMonth,
                           BillForYear = a.BillForYear,
                           TelecomCompanyGUID = a.TelecomCompanyGUID.ToString(),
                           TelecomCompanyDescription = a.TelecomCompanyDescription,
                           BillGUID = a.BillGUID,
                           dataUserBillRowVersion = a.dataUserBillRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<PendingBillsDataTableModel> Result = Mapper.Map<List<PendingBillsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SendBillReminder(Guid UserBillGUID)
        {
            return Json(new { success = true });
        }


        [Route("TBS/PendingBills/Reminder/")]
        public ActionResult PendingBillsReminder()
        {
            return PartialView("~/Areas/TBS/Views/PendingBills/_PendingBulkReminder.cshtml");
        }

        [HttpPost]
        public ActionResult PendingBillsDataTableReminder(List<PendingBillsDataTableModel> models, DateTime NewBillDeadLine)
        {
            if (models == null)
            {
                JsonReturn jr = new JsonReturn()
                {
                    DataTable = DataTableNames.PendingBillsDataTable,
                    Notify = new Notify { Type = MessageTypes.Warning, Message = "No record is selected." }
                };

                return Json(jr);
            }
            DateTime ExecutionTime = DateTime.Now;
            List<Guid> userBillGUIDs = models.Select(x => x.UserBillGUID).ToList();

            var listOfUsers = (from a in DbTBS.dataUserBill
                               join b in DbTBS.dataBill on a.BillGUID equals b.BillGUID
                               join c in DbTBS.StaffCoreData on a.UserGUID equals c.UserGUID
                               join d in DbTBS.userPersonalDetailsLanguage on c.UserGUID equals d.UserGUID

                               where userBillGUIDs.Contains(a.UserBillGUID)
                               && d.LanguageID == "EN"
                               && d.Active
                               select new
                               {
                                   UserBillGUID = a.UserBillGUID,
                                   StaffName = d.FirstName + " " + d.Surname,
                                   EmailAddress = c.EmailAddress,
                                   DataBill = b
                               }).ToList();

            #region Sending Emails
            foreach (var item in listOfUsers)
            {
                string BillDeadLine = NewBillDeadLine.ToShortDateString();
                int BillMonthNumber = item.DataBill.BillForMonth;
                string BillMonthName = new System.Globalization.DateTimeFormatInfo().GetMonthName(BillMonthNumber);
                string BillYear = item.DataBill.BillForYear.ToString();

                string TelecomCompanyName = (from a in DbTBS.configTelecomCompanyOperation
                                             join b in DbTBS.codeTelecomCompanyOperation on a.TelecomCompanyOperationGUID equals b.TelecomCompanyOperationGUID
                                             join c in DbTBS.codeTelecomCompanyLanguages on b.TelecomCompanyGUID equals c.TelecomCompanyGUID
                                             where c.LanguageID == "EN"
                                             && a.TelecomCompanyOperationConfigGUID == item.DataBill.TelecomCompanyOperationConfigGUID
                                             select c.TelecomCompanyDescription).FirstOrDefault();

                string subject = "DO NOT reply | SYRIA Telephony Billing System - " + TelecomCompanyName + " Mobile Bills --" + BillMonthName + "/" + BillYear + " | Reminder Email";



                string URL = AppSettingsKeys.Domain + "/TBS/UserBills/Update/" + item.UserBillGUID;
                string Anchor = "<a href='" + URL + "' target='_blank'>Here</a>";
                string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                string UserGuidURL = "https://unhcr365.sharepoint.com/teams/mena-syr-ICTUnit/Shared%20Documents/ICT%20Unit/Documentation/Guides%20User/Telephone%20Bills_V2_Usermanual.pdf";
                string UserGuidAnchor = "<a href='" + UserGuidURL + "' target='_blank'>User Guide</a>";
                string body_format = "HTML";
                string importance = "Normal";

                string Message = resxEmails.TBSBillReadyEmail
                    .Replace("$FullName", item.StaffName)
                    .Replace("$BillType", TelecomCompanyName + " mobile")
                    .Replace("$BillMonthYear", BillMonthName + " " + BillYear)
                    .Replace("$Deadline", BillDeadLine)
                    .Replace("$IdentifyBillLinkIn", Anchor)
                    .Replace("$UserGuidLink", UserGuidAnchor);


                string _body = "<div style='font-family:Arial;'>" + Message.Replace("\r\n", "<br/>") + "</div>";


                //DbCMS.SendEmail("karkoush@unhcr.org", "karkoush@unhcr.org", "karkoush@unhcr.org", subject, _body, body_format, importance, null, "Telephony Billing System <billing@unhcr.org>");

                DbCMS.SendEmail(item.EmailAddress, "billing@unhcr.org", "karkoush@unhcr.org", subject, _body, body_format, importance, null, "Telephony Billing System <billing@unhcr.org>");

                //DbCMS.SendEmail(item.EmailAddress, null, null, subject, _body, body_format, importance, null, "Telephony Billing System");

            }
            #endregion

            try
            {
                JsonReturn jr = new JsonReturn()
                {
                    AffectedRecordsGuids = userBillGUIDs,
                    DataTable = DataTableNames.PendingBillsDataTable,
                    Notify = new Notify { Type = MessageTypes.Success, Message = "Email reminder has been send successfully" }
                };

                return Json(jr);
            }
            catch (Exception ex)
            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }

        [Route("TBS/PendingBills/SetPrivate/")]
        public ActionResult PendingBillsSetPrivate()
        {
            return PartialView("~/Areas/TBS/Views/PendingBills/_PendingBulkSetPrivate.cshtml");
        }


        [HttpPost]
        public ActionResult PendingBillsDataTableSetPrivate(List<PendingBillsDataTableModel> models)
        {
            if (models == null)
            {
                JsonReturn jr = new JsonReturn()
                {
                    DataTable = DataTableNames.PendingBillsDataTable,
                    Notify = new Notify { Type = MessageTypes.Warning, Message = "No record is selected." }
                };

                return Json(jr);
            }
            DateTime ExecutionTime = DateTime.Now;
            List<Guid> userBillGUIDs = models.Select(x => x.UserBillGUID).ToList();
            foreach (var userBillGUID in userBillGUIDs)
            {
                dataUserBill dataUserBill = (from a in DbTBS.dataUserBill where a.UserBillGUID == userBillGUID select a).FirstOrDefault();
                List<dataUserBillDetail> dataUserBillDetails = dataUserBill.dataUserBillDetail.ToList();
                dataUserBillDetails.ForEach(x => { x.IsPrivate = true; x.IsConfirmed = true; x.AutomatedBySystem = true; });
                dataUserBill.IsConfirmed = true;
                dataUserBill.IsFirstOpen = false;
                dataUserBill.ConfirmationDate = DateTime.Now;
                dataUserBill.DoPayInCash = false;
                dataUserBill.PayInCashAmount = 0;
                dataUserBill.DeductFromSalaryAmount = 0;
                //dataUserBill.DeductFromSalaryAmount = dataUserBillDetails.Sum(x => x.CallCost);

            }

            DbTBS.SaveChanges();

            try
            {
                JsonReturn jr = new JsonReturn()
                {
                    AffectedRecordsGuids = userBillGUIDs,
                    DataTable = DataTableNames.PendingBillsDataTable,
                    Notify = new Notify { Type = MessageTypes.Success, Message = "Records has been marked as private successfully" }
                };

                return Json(jr);
            }
            catch (Exception ex)
            {
                return Json(DbTBS.ErrorMessage(ex.Message));
            }
        }


        [HttpPost]
        public JsonResult MarkUnconfirmedPrivate(Guid UserBillGUID)
        {
            dataUserBill dataUserBill = (from a in DbTBS.dataUserBill.Where(x => x.UserBillGUID == UserBillGUID)
                                         select a).FirstOrDefault();
            List<dataUserBillDetail> dataUserBillDetails = dataUserBill.dataUserBillDetail.ToList();

            dataUserBillDetails.Where(x => x.IsConfirmed == false).ToList().ForEach(x => { x.IsConfirmed = true; x.IsPrivate = true; x.AutomatedBySystem = true; });
            dataUserBill.IsConfirmed = true;
            dataUserBill.IsFirstOpen = false;
            dataUserBill.ConfirmationDate = DateTime.Now;
            dataUserBill.DoPayInCash = false;
            dataUserBill.PayInCashAmount = 0;
            dataUserBill.DeductFromSalaryAmount = 0;
            //dataUserBill.DeductFromSalaryAmount = dataUserBillDetails.Sum(x => x.CallCost);
            DbTBS.SaveChanges();
            return Json(new { success = true });
        }


    }
}