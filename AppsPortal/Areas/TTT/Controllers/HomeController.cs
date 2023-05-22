using AppsPortal.BaseControllers;
using AppsPortal.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.TTT.Controllers
{
    public class HomeController : TTTBaseController
    {

        //[Route("TTT/")]
        public ActionResult Index()
        {
            CMS.SetUserToken(UserProfileGUID, Apps.TTT);
            Session[SessionKeys.CurrentApp] = Apps.TTT;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }
    }
}