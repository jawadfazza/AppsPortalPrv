using PCR_DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.PCR.Controllers
{
    public class CommunityCentersAllDataController : Controller
    {
        [Route("PCR/CommunityCentersAllData/")]
        public ActionResult CommunityCentersAllData()
        {
            using (var DbPCR = new PCREntities())
            {
                var CommunityCentersAllData = DbPCR.v_CommunityCenters.Take(1000).ToList();
                return View("~/Areas/PCR/Views/PartnerReports/CommunityCentersAllData.cshtml", CommunityCentersAllData);
            }
        }
           
    }
}