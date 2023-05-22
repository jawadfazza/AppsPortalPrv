using GTP_DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.BaseControllers
{
    public class GTPBaseController : PortalBaseController
    {
        public GTPEntities DbGTP;
        public GTPBaseController()
        {
            DbGTP = new GTPEntities();
        }
    }
}