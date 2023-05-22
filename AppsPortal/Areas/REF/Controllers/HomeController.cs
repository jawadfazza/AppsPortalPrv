using AppsPortal.BaseControllers;
using AppsPortal.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.REF.Controllers
{
    public class HomeController : REFBaseController
    {
        // GET: REF/Home
        public ActionResult Index()
        {
            CMS.SetUserToken(UserProfileGUID, Apps.REF);
            Session[SessionKeys.CurrentApp] = Apps.REF;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }
    }
}