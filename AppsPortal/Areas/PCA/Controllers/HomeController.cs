using AppsPortal.BaseControllers;
using AppsPortal.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.PCA.Controllers
{
    public class HomeController : PCABaseController
    {
        // GET: VOT/Home
        public ActionResult Index()
        {
            CMS.SetUserToken(UserProfileGUID, Apps.PCA);
            Session[SessionKeys.CurrentApp] = Apps.PCA;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }
    }
}