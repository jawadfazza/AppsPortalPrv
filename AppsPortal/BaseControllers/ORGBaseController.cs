using ORG_DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.BaseControllers
{
    public class ORGBaseController : PortalBaseController
    {
        public ORGEntities DbORG;
        public ORGBaseController()
        {
            DbORG = new ORGEntities();
        }
    }
}