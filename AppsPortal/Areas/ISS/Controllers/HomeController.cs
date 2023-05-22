using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppsPortal.BaseControllers;
using AppsPortal.Library;

namespace AppsPortal.Areas.ISS.Controllers
{
    public class HomeController : ISSBaseController
    {
        // GET: ISS/Home
        public ActionResult Index()
        {
            if (!CMS.HasAction(Permissions.ItemOverAllStock.Update, Apps.WMS))
            {
                return Json(DbAHD.PermissionError());
            }
            return View();
        }
        public ActionResult DataUploader()
        {
            //CMS.SetUserToken(UserProfileGUID, Apps.WMS);
            //Session[SessionKeys.CurrentApp] = Apps.WMS;
            //CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }

        public ActionResult OfflineBrowse()
        {
            return View();
        }
    }
}