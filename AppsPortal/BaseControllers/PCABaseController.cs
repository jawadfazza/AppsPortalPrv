using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppsPortal.Extensions;
using AppsPortal.Library;
using PCA_DAL.Model;

namespace AppsPortal.BaseControllers
{
    public class PCABaseController : PortalBaseController
    {
        public PCAEntities DbPCA;
        public PCABaseController()
        {
            DbPCA = new PCAEntities();
        }
    }
 
    }
