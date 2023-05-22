using AppsPortal.BaseControllers;
using AppsPortal.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.GTP.Controllers
{
    public class HomeController : GTPBaseController
    {
        // GET: GTP/Home
        public ActionResult Index()
        {
            CMS.SetUserToken(UserProfileGUID, Apps.GTP);
            Session[SessionKeys.CurrentApp] = Apps.GTP;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }
    }
}