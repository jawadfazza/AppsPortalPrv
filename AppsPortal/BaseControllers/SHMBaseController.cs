using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppsPortal.Extensions;
using AppsPortal.Library;
using SHM_DAL.Model;

namespace AppsPortal.BaseControllers
{
    public class SHMBaseController : PortalBaseController
    {
        public SHMEntities DbSHM;

        // GET: VOTBase
        public SHMBaseController()
        {
            DbSHM = new SHMEntities();
        }

    }
}