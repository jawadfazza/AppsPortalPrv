using AppsPortal.BaseControllers;
using AppsPortal.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.TBS.Controllers
{
    public class HomeController : TBSBaseController
    {
        public ActionResult Index()
        {
            CMS.SetUserToken(UserProfileGUID, Apps.TBS);
            Session[SessionKeys.CurrentApp] = Apps.TBS;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }
    }
}