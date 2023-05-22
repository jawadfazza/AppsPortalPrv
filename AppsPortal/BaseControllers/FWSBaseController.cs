using FWS_DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.BaseControllers
{
    public class FWSBaseController : PortalBaseController
    {
        public FWSEntities DbFWS;
        public FWSBaseController()
        {
            DbFWS = new FWSEntities();
        }

    }
}