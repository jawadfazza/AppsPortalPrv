﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppsPortal.Extensions;
using AppsPortal.Library;
using WMS_DAL.Model;

namespace AppsPortal.BaseControllers
{
    public class WMSBaseController : PortalBaseController
    {
        // GET: WMS
        public WMSEntities DbWMS;
        public FileManager importFile = new FileManager();
        public StringModelBinder StringModel = new StringModelBinder();
        public Email mail = new Email();
        public WMSBaseController()
        {
            DbWMS = new WMSEntities();
        }
    }
}