using AppsPortal.BaseControllers;
using AppsPortal.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.COV.Controllers
{
    public class HomeController : COVBaseController
    {
        // GET: COV/Home
        public ActionResult Index()
        {
            CMS.SetUserToken(UserProfileGUID, Apps.COV);
            Session[SessionKeys.CurrentApp] = Apps.COV;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }
    }
}