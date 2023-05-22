using AppsPortal.BaseControllers;
using AppsPortal.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.CMS.Controllers
{
    public class HomeController : PortalBaseController
    {
        // GET: CMS/Home
        public ActionResult Index()
        {
            CMS.SetUserToken(UserProfileGUID, Apps.CMS);
            Session[SessionKeys.CurrentApp] = Apps.CMS;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }

        public ActionResult TempCodeTables()
        {
            return View("~/Areas/CMS/Views/Home/TestCodeTables.cshtml");
        }
        
    }
}