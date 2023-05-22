
using AHD_DAL.Model;
using AMS_DAL.Model;
using AppsPortal.Data;

using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Models;
using DAS_DAL.Model;
using EMT_DAL.Model;
using SHM_DAL.Model;
using System;
using System.Web.Mvc;
using WMS_DAL.Model;

namespace AppsPortal.BaseControllers
{

    public class PortalBaseController : Controller
    {
        public CMSEntities DbCMS;
        public WMSEntities DbWMS;
        public AHDEntities DbAHD;
        public AMSEntities DbAMS;
        public EMTEntities DbEMT;
        public DASEntities DbDAS;


        public CMS CMS;
        public FactorsCollector FactorsCollector;
        public DropDownList DropDownList;
        public Portal Portal;
        public string LAN;
        public Guid UserGUID;
        public Guid UserProfileGUID;
        public PortalBaseController()
        {
            DbCMS = new CMSEntities();
            DbWMS = new WMSEntities();
            DbAHD = new AHDEntities();
            DbAMS = new AMSEntities();
            DbEMT = new EMTEntities();
            DbDAS = new DASEntities();
            CMS = new CMS(DbCMS);
            FactorsCollector = new FactorsCollector(DbCMS);
            DropDownList = new DropDownList(DbCMS);
            Portal = new Portal();
            LAN = Languages.CurrentLanguage();
            UserGUID = Portal.UserID();
            UserProfileGUID = Portal.UserProfileGUID(); 
        }
    }
}