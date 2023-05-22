using AppsPortal.BaseControllers;
using AppsPortal.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.OVS.Controllers
{
    public class HomeController : OVSBaseController
    {
        // GET: VOT/Home
        public ActionResult Index()
        {
            CMS.SetUserToken(UserProfileGUID, Apps.OVS);
            Session[SessionKeys.CurrentApp] = Apps.OVS;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }
    }
}