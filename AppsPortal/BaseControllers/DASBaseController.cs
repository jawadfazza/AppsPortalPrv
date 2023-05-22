using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppsPortal.Areas.PRG.Models;
using AppsPortal.Extensions;
using AppsPortal.Library;

using DAS_DAL.Model;


namespace AppsPortal.BaseControllers
{
    public class DASBaseController : PortalBaseController
    {
        public DASEntities DbDAS;
        public PRGEntities DbPRG;
        //public proGresEntities dbProGres;
        public DASBaseController()
        {
            DbPRG = new PRGEntities();
            DbDAS = new DASEntities();
        }
    }
}