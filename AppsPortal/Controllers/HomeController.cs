using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Models;
using AppsPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Controllers
{
    public class HomeController : PortalBaseController
    {
        // GET: Home
        public ActionResult Index()
        {
            this.ControllerContext.HttpContext.Request.Cookies["__RequestVerificationToken"].Secure = true;
            this.ControllerContext.HttpContext.Request.Cookies["ASP.NET_SessionId"].Secure = true;

            try
            {
                if (System.Web.HttpContext.Current.Session["FirstLogin"].ToString() == "True")
                {
                    System.Web.HttpContext.Current.Session["FirstLogin"] = "No";
                    ViewBag.IsFirstLogin = "True";
                }
                else
                {
                    ViewBag.IsFirstLogin = "No";
                }
            }
            catch
            {
                System.Web.HttpContext.Current.Session["FirstLogin"] = "No";
                ViewBag.IsFirstLogin = "True";
            }

            if (System.Web.HttpContext.Current.Session[SessionKeys.EmailAddress].ToString().ToUpper().EndsWith("UNHCR.ORG"))
            {
                ViewBag.UNHCRUser = "UNHCR";
            }
            else
            {
                ViewBag.UNHCRUser = "Non-UNHCR";
            }

            ViewBag.TotalPendingSTIITems = DbWMS.dataItemOutputDetailFlow.Where(x => x.dataItemOutputDetail.dataItemInputDetail.LastCustdianNameGUID == UserGUID
                               && x.Active
                               && x.IsLastAction == true
                               && x.FlowTypeGUID ==
                               WarehouseRequestFlowType.PendingConfirmed).Count();
            ViewBag.TotalPendingRR = DbAHD.dataRestAndRecuperationRequest.Where(x => 
                                        (x.StaffGUID == UserGUID && x.FlowStatusGUID != InternationalStaffRAndRLeaveFlowStatus.Closed) 
                                        || (x.SupervisorGUID == UserGUID && x.FlowStatusGUID == InternationalStaffRAndRLeaveFlowStatus.PendingSupervisorApproval)
                                        || (x.ApprovedByGUID == UserGUID && x.FlowStatusGUID == InternationalStaffRAndRLeaveFlowStatus.PendingRepresentativeApproval))
                                    .Count();
            ViewBag.TotalPendingDangerPay = DbAHD.dataNationalStaffDangerPay.Where(x => (x.UserGUID == UserGUID
                       ) && x.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Pending).Count();
            

            var _staffCore = DbAHD.StaffCoreData.Where(x => (x.UserGUID == UserGUID));
            ViewBag.TotalPendinfStaffProfile = _staffCore.Where(x => x.LastConfirmationStatusGUID == StaffProfileConfirmation.Pending).Count();
            Guid _internationalStaffGUID = Guid.Parse("FC4B2E79-2B97-4252-A50B-915B07A12310");
            ViewBag.recruitmentType = _staffCore.Where(x => x.RecruitmentTypeGUID == _internationalStaffGUID).Count() > 0 ? 1 : 0;

            return View();

        }

        public ActionResult ShowMyInfo()
        {
            return View("~/Views/Home/ShowMyIp.cshtml");
        }

        public ActionResult ShowMyIp()
        {
            ShowMyInfoModel model = new ShowMyInfoModel();
            model.BrowserName = Request.UserAgent;
            model.UserHostAddress = Request.UserHostAddress;
            model.UserHostName = Request.UserHostName;
            return PartialView("~/Views/Home/_ShowMyIp.cshtml", model);
        }
    }
}