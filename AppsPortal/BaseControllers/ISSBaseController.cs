using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppsPortal.Extensions;
using ISS_DAL.Model;

namespace AppsPortal.BaseControllers
{
    public class ISSBaseController : PortalBaseController
    {
        // GET: ISSBase
        public ISSEntities DbISS;
        public FileManager importFile = new FileManager();
        //public IMS_DAL.;
        // GET: VOTBase
        public ISSBaseController()
        {
            DbISS = new ISSEntities();

            //DbAMS = new AMSEntities();
            //DbPRG = new PRGEntities();
            //DbREF = new REFEntities();
        }
    }
}