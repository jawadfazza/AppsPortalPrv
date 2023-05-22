using AppsPortal.BaseControllers;
using AppsPortal.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.PRG.Controllers
{
    public class HomeController : PRGBaseController
    {
        // GET: PRG/Home
        public ActionResult Index()
        {
            CMS.SetUserToken(UserProfileGUID, Apps.PRG);
            Session[SessionKeys.CurrentApp] = Apps.PRG;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }
    }
}