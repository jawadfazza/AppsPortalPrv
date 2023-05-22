using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppsPortal.Extensions;
using AppsPortal.Library;

using IMS_DAL.Model;


namespace AppsPortal.BaseControllers
{
    public class IMSBaseController : PortalBaseController
    {
        public IMSEntities DbIMS;
        public FileManager importFile = new FileManager();
        public StringModelBinder StringModel = new StringModelBinder();
        public Email mail = new Email();

   
  
        //public IMS_DAL.;
        // GET: VOTBase
        public IMSBaseController()
        {
            DbIMS=new IMSEntities();

            //DbAMS = new AMSEntities();
            //DbPRG = new PRGEntities();
            //DbREF = new REFEntities();
        }
    }
}