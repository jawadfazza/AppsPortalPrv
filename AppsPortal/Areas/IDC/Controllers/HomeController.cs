using AppsPortal.BaseControllers;
using AppsPortal.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.IDC.Controllers
{
    public class HomeController : IDCBaseController
    {
        // GET: IDC/Home
        public ActionResult Index()
        {
            CMS.SetUserToken(UserProfileGUID, Apps.IDC);
            Session[SessionKeys.CurrentApp] = Apps.IDC;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }

        [HttpGet]
        public ActionResult CodeTable()
        {
            return View("~/Areas/IDC/Views/Home/Index.cshtml");
        }
    }
}