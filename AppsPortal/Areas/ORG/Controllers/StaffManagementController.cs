using AppsPortal.BaseControllers;
using AppsPortal.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.ORG.Controllers
{
    public class StaffManagementController : ORGBaseController
    {
        // GET: ORG/StaffManagement
        public ActionResult Index()
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Update, Apps.ORG))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View();
        }

        public ActionResult StaffProfileCreate()
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Update, Apps.ORG))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View();
        }
    }
}