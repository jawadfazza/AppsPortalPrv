using AppsPortal.BaseControllers;
using AppsPortal.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.AMS.Controllers
{
    public class HomeController : AMSBaseController
    {
        // GET: AMS/Home
        public ActionResult Index()
        {
            CMS.SetUserToken(UserProfileGUID, Apps.AMS);
            Session[SessionKeys.CurrentApp] = Apps.AMS;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }
        [HttpGet]
        public ActionResult ReportIndex()
        {
            return View("~/Areas/AMS/Views/Reports/SchedularReports.cshtml");
        }
    }
}