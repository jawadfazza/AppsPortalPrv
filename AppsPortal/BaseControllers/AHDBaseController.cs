using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AHD_DAL.Model;

namespace AppsPortal.BaseControllers
{
    public class AHDBaseController : PortalBaseController
    {
        public AHDEntities DbAHD;
        public AHDBaseController()
        {
            DbAHD = new AHDEntities();
        }
    }
 
    }
