
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppsPortal.Extensions;
using AppsPortal.Library;
using REF_DAL.Model;


namespace AppsPortal.BaseControllers
{
    public class REFBaseController : PortalBaseController
    {
        public REFEntities DbREF;
        public FileManager importFile = new FileManager();
        public StringModelBinder StringModel=new StringModelBinder();
        public  Email mail = new Email();
        // GET: RASBase
        public REFBaseController()
        {
            DbREF = new REFEntities();
        }
   
    }
}