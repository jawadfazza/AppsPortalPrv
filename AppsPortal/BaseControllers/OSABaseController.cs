using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppsPortal.Extensions;
using AppsPortal.Library;
using OSA.Model;

namespace AppsPortal.BaseControllers
{
    public class OSABaseController : PortalBaseController
    {
        public Entities DbOSA;
        public FileManager importFile = new FileManager();
        public OSABaseController()
        {
            DbOSA = new Entities();
        }
    }
 
    }
