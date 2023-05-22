using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppsPortal.BaseControllers;
using AppsPortal.Library;
using DAS_DAL.ViewModels;


namespace AppsPortal.Areas.DAS.Controllers
{
    public class HomeController : DASBaseController
    {
        // GET: DAS/Home
        public ActionResult Index()
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Update, Apps.DAS))
            {
                return Json(DbAHD.PermissionError());
            }
            FileDashboardModel model = new FileDashboardModel();
            DateTime ExecutionTime = DateTime.Now;
            DateTime dayAfterTenDays = ExecutionTime.AddDays(10);
            Guid filingUnitGUID = Guid.Parse("bb915454-05fb-4e4d-b033-e01c7c790997");
            var files=DbDAS.dataFile.AsQueryable();
            var flows = DbDAS.dataScannDocumentTransferFlow.Where(x => x.IsLastAction == true && x.dataScannDocumentTransfer.TransferEndDate != null && x.dataScannDocumentTransfer.dataFileRequest.dataFile.LastCustodianTypeGUID == DASDocumentCustodianType.Staff).AsQueryable();
            var fileRequest = DbDAS.dataFileRequest.AsQueryable();
            model.TotalCasesDelayInReturn = flows.Count();
            model.TotalCasesPendingConfirmation = files.Where(x=>x.LastFlowStatusGUID== ScanDocumentTransferFlowStatus.Pending).Count();
            model.TotalCasesUserPendingConfirmation  = files.Where(x => x.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Pending && x.LastCustodianTypeNameGUID==UserGUID).Count(); ;
            model.TotalCasesHaveAppointmentNotInfiling = files.Where(x=>x.LastCustodianTypeGUID!= filingUnitGUID
            && (x.LatestAppointmentDate!=null && (x.LatestAppointmentDate>= ExecutionTime && x.LatestAppointmentDate<= dayAfterTenDays))).Count();
            
            //var result=DbDAS.codeActionsEntities.ToList();
            return View(model);
        }
        public ActionResult Configuration()
        {
            CMS.SetUserToken(UserProfileGUID, Apps.WMS);
            Session[SessionKeys.CurrentApp] = Apps.WMS;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }

        public ActionResult FileTracking()
        {
            CMS.SetUserToken(UserProfileGUID, Apps.WMS);
            Session[SessionKeys.CurrentApp] = Apps.WMS;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }
        public ActionResult Reports()
        {
            CMS.SetUserToken(UserProfileGUID, Apps.WMS);
            Session[SessionKeys.CurrentApp] = Apps.WMS;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }

    }
}