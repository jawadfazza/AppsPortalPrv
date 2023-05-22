using AppsPortal.BaseControllers;
using AppsPortal.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.SRS.Controllers
{
    public class HomeController : SRSBaseController
    {
        // GET: SRS/Home

        public ActionResult Index()
        {
            CMS.SetUserToken(UserProfileGUID, Apps.SRS);
            Session[SessionKeys.CurrentApp] = Apps.SRS;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }


        [Route("SRS/Home/UserManual")]
        public ActionResult HelpDeskAttachementCreate()
        {
            return PartialView("~/Areas/SRS/Views/Home/_UserManual.cshtml");
        }
    }
}