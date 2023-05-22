using AHD_DAL.Model;
using AppsPortal.BaseControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.AHD.Controllers
{
    public class PowerBIController : Controller
    {
        // GET: AHD/PowerBI
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult StaffCoreViewPowerBI()
        {
            AHDEntities DbAHD = new AHDEntities();
            //string[] StatusGUIDs = { "B9CD375C-A576-4AA4-8AF4-FF3C1C4E3445", "B9CD375C-A576-4AA4-8AF4-FF3C1C4E3611", "B9CD375C-A576-4AA4-8AF4-FF3C1C4E3622" };
            var Result = DbAHD.v_staffCoreDataOverview/*.Where(x => StatusGUIDs.Contains(x.StaffStatusGUID.ToString().ToUpper()))*/.Select(x => x).ToList();

            return View("~/Areas/AHD/Views/PowerBI/StaffCoreViewPowerBI.cshtml", Result);
        }

    }
}