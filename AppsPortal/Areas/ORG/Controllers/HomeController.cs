using AppsPortal.BaseControllers;
using AppsPortal.Library;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.ORG.Controllers
{
    public class HomeController : ORGBaseController
    {
        // GET: ORG/Home
        public ActionResult Index()
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Update, Apps.ORG))
            {
                return Json(DbAHD.PermissionError());
            }
            CMS.SetUserToken(UserProfileGUID, Apps.ORG);
            Session[SessionKeys.CurrentApp] = Apps.ORG;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }

        public ActionResult Configuration()
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Update, Apps.ORG))
            {
                return Json(DbAHD.PermissionError());
            }
            CMS.SetUserToken(UserProfileGUID, Apps.ORG);
            Session[SessionKeys.CurrentApp] = Apps.ORG;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }
    }
}