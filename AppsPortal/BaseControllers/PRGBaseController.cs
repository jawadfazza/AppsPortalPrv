using AppsPortal.Areas.PRG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.BaseControllers
{
    public class PRGBaseController : PortalBaseController
    {
        public PRGEntities DbPRG;
        // GET: VOTBase
        public PRGBaseController()
        {
            DbPRG = new PRGEntities();
        }

        
    }
}