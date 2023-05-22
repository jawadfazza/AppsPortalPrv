using COV_DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.BaseControllers
{
    public class COVBaseController : PortalBaseController
    {
        public COVEntities DbCOV;

        public COVBaseController()
        {
            DbCOV = new COVEntities();
        }

    }
}