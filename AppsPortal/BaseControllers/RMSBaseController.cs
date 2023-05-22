using RMS_DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.BaseControllers
{
    public class RMSBaseController : PortalBaseController
    {
        public RMSEntities DbRMS;
        public RMSBaseController()
        {
            DbRMS = new RMSEntities();
        }

    }
}