using PMD_DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.BaseControllers
{
    public class PMDBaseController : PortalBaseController
    {
        public PMDEntities DbPMD;
        public PMDBaseController()
        {
            DbPMD = new PMDEntities();
        }
    }
}