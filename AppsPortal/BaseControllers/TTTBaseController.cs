using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TTT_DAL.Model;

namespace AppsPortal.BaseControllers
{
    public class TTTBaseController : PortalBaseController
    {
        public TTTEntities DbTTT;
        public TTTBaseController()
        {
            DbTTT = new TTTEntities();
        }
       
    }
}