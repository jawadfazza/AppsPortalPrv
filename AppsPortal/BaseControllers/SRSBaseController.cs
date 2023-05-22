using AppsPortal.Library;
using SRS_DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.BaseControllers
{
    public class SRSBaseController : PortalBaseController
    {
        public SRSEntities DbSRS;
        public SRSBaseController()
        {
            DbSRS = new SRSEntities();
            //return View();
        }
    }
}