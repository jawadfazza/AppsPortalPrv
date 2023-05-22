using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppsPortal.Extensions;
using AppsPortal.Library;
using OVS_DAL.Model;

namespace AppsPortal.BaseControllers
{
    public class OVSBaseController : PortalBaseController
    {
        public OVSEntities DbOVS;
        public FileManager importFile = new FileManager();
        public StringModelBinder StringModel=new StringModelBinder();
        public  Email mail = new Email();
        // GET: VOTBase
        public OVSBaseController()
        {
            DbOVS = new OVSEntities();
        }
   
    }
}