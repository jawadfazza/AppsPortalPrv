using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AMS_DAL.Model;
using REF_DAL.Model;
using AppsPortal.Areas.PRG.Models;

namespace AppsPortal.BaseControllers
{
    public class AMSBaseController : PortalBaseController
    {
        public AMSEntities DbAMS;
        public PRGEntities DbPRG;
        public REFEntities DbREF;
        public FileManager importFile = new FileManager();
        public StringModelBinder StringModel = new StringModelBinder();
        public Email mail = new Email();
        // GET: VOTBase
        public AMSBaseController()
        {
            DbAMS = new AMSEntities();
            DbPRG = new PRGEntities();
            DbREF = new REFEntities();
        }

    }
}