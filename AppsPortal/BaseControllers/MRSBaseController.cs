using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppsPortal.Extensions;
using AppsPortal.Library;
using MRS_DAL.Model;

namespace AppsPortal.BaseControllers
{
    public class MRSBaseController : PortalBaseController
    {
        public MRSEntities DbMRS;

        // GET: VOTBase
        public MRSBaseController()
        {
            DbMRS = new MRSEntities();
        }

    }
}