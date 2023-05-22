using AppsPortal.BaseControllers;
using AppsPortal.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.PPA.Controllers
{
    public class HomeController : PPABaseController
    {

        public ActionResult Index()
        {
            CMS.SetUserToken(UserProfileGUID, Apps.PPA);
            Session[SessionKeys.CurrentApp] = Apps.PPA;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }
    }
}