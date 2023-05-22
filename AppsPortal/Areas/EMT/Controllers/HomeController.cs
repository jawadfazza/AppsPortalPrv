using AppsPortal.BaseControllers;
using AppsPortal.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.EMT.Controllers
{
    public class HomeController : EMTBaseController
    {
        // GET: EMT/Home
        public ActionResult Index()
        {
            CMS.SetUserToken(UserProfileGUID, Apps.EMT);
            Session[SessionKeys.CurrentApp] = Apps.EMT;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }

        [HttpGet]
        public ActionResult CodeTable()
        {
            return View("~/Areas/EMT/Views/Home/CodeTable.cshtml");
        }
    }
}