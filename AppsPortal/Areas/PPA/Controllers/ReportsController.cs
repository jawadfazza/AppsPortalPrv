using AppsPortal.BaseControllers;
using AppsPortal.Library;
using AppsPortal.Models;
using PPA_DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.PPA.Controllers
{
    public class ReportsController : PPABaseController
    {

        [Route("PPA/Reports")]
        public ActionResult Index()
        {

            if (!CMS.HasAction(Permissions.PPAManagement.Access, Apps.PPA) && !CMS.HasAction(Permissions.PPAManagement.FullAccess, Apps.PPA))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/PPA/Views/Reports/Home.cshtml");
        }

        [Route("PPA/Reports/ReviewTrackingReport")]
        public ActionResult ReviewTrackingReport(string PPADescription = null, string PPATypeGUID = null, string ImplementationAreaGUID = null, string OrganizationInstanceGUID = null)
        {

            Guid _PPATypeGUID = Guid.Empty;
            Guid _ImplementationAreaGUID = Guid.Empty;
            Guid _OrganizationInstanceGUID = Guid.Empty;
            if (PPATypeGUID != null) _PPATypeGUID = Guid.Parse(PPATypeGUID);
            if (ImplementationAreaGUID != null) _ImplementationAreaGUID = Guid.Parse(ImplementationAreaGUID);
            if (OrganizationInstanceGUID != null) _OrganizationInstanceGUID = Guid.Parse(OrganizationInstanceGUID);

            List<ReviewTrackingReportModel> reviewTrackingReportModel = (from a in DbPPA.ProjectPartnershipAgreement.Where(x => x.Active)
                                                                         join o in DbPPA.codeOrganizationsLanguages.Where(x => x.Active && x.LanguageID == "EN") on a.OrganizationInstanceGUID equals o.OrganizationGUID
                                                                         join b in DbPPA.PPAOriginalFile.Where(x => x.Active) on a.PPAGUID equals b.PPAGUID into LJ1
                                                                         from RJ1 in LJ1.DefaultIfEmpty()
                                                                         join c in DbPPA.PPAFileVersion.Where(x => x.Active) on RJ1.PPAOriginalFileGUID equals c.PPAOriginalFileGUID into LJ2
                                                                         from RJ2 in LJ2.DefaultIfEmpty()
                                                                         join d in DbPPA.userProfiles.Where(x => x.Active) on a.CreatedByUserProfileGUID equals d.UserProfileGUID into LJ3
                                                                         from RJ3 in LJ3.DefaultIfEmpty()
                                                                         join e in DbPPA.userServiceHistory.Where(x => x.Active) on RJ3.ServiceHistoryGUID equals e.ServiceHistoryGUID into LJ4
                                                                         from RJ4 in LJ4.DefaultIfEmpty()
                                                                         join f in DbPPA.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == "EN") on RJ4.UserGUID equals f.UserGUID into LJ5
                                                                         from RJ5 in LJ5.DefaultIfEmpty()
                                                                         join j in DbPPA.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == "EN") on RJ2.FileActionByUserGUID equals j.UserGUID into LJ6
                                                                         from RJ6 in LJ6.DefaultIfEmpty()
                                                                             //where (_OrganizationInstanceGUID == Guid.Empty || a.OrganizationInstanceGUID == _OrganizationInstanceGUID)
                                                                             //&& (_PPATypeGUID == Guid.Empty || a.PPATypeGUID == _PPATypeGUID)
                                                                             //&& (_ImplementationAreaGUID == Guid.Empty || a.ImplementationAreaGUID == _ImplementationAreaGUID)
                                                                             //&& (OrganizationInstanceGUID.Length == 0 || OrganizationInstanceGUID.Contains(a.PPADescription))
                                                                         group new { RJ1, RJ2 } by new { a.PPAGUID, a.PPATypeGUID, a.ImplementationAreaGUID, a.OrganizationInstanceGUID, a.PPADescription, a.PPADeadLine, o.OrganizationDescription } into g1
                                                                         select new ReviewTrackingReportModel
                                                                         {
                                                                             PPADescription = g1.Key.PPADescription,
                                                                             PPAGUID = g1.Key.PPAGUID,
                                                                             PPATypeGUID = g1.Key.PPATypeGUID,
                                                                             ImplementationAreaGUID = g1.Key.ImplementationAreaGUID,
                                                                             OrganizationInstanceGUID = g1.Key.OrganizationInstanceGUID,
                                                                             OrganizationDescription = g1.Key.OrganizationDescription,
                                                                             PPADeadLine = g1.Key.PPADeadLine,
                                                                             PPAOriginalFilesLayer = (from x in g1
                                                                                                      group x by new { x.RJ1.PPAOriginalFileGUID, x.RJ1.FileName, x.RJ1.FileType, x.RJ1.UploadByUserGUID } into g2
                                                                                                      select new PPAFilesReportModel
                                                                                                      {
                                                                                                          PPAOriginalFileGUID = g2.Key.PPAOriginalFileGUID,
                                                                                                          FileName = g2.Key.FileName,
                                                                                                          FileType = g2.Key.FileType,
                                                                                                          UploadByUserGUID = g2.Key.UploadByUserGUID,
                                                                                                          PPAUserFileVersionLayer = (from y in g2
                                                                                                                                     group y by new { y.RJ2.PPAOriginalFileGUID, y.RJ2.FileActionByUserGUID } into g3
                                                                                                                                     select new PPAFileUsersReportModel
                                                                                                                                     {
                                                                                                                                         FileActionByUserName = g3.Key.FileActionByUserGUID.HasValue ? (from u in DbPPA.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == "EN" && x.UserGUID == g3.Key.FileActionByUserGUID.Value) select u.FirstName + " " + u.Surname).FirstOrDefault() : "",
                                                                                                                                         PPAUserFilesVersionDetailsLayer = (from z in g3
                                                                                                                                                                            select z.RJ2)
                                                                                                                                     })

                                                                                                      })

                                                                         }).ToList();


            return View("~/Areas/PPA/Views/Reports/ReviewTrackingReport.cshtml", reviewTrackingReportModel);
        }

        #region Progres Tracking Report
        [Route("PPA/Reports/ProgressTrackingReport")]
        public ActionResult ProgressTrackingReport()
        {
            var PPADataModel = (from a in DbPPA.ProjectPartnershipAgreement.Where(x => x.Active)
                                join b in DbPPA.codeOrganizationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OrganizationInstanceGUID equals b.OrganizationGUID
                                join c in DbPPA.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN).Where(x => x.codeTablesValues.TableGUID == LookupTables.PPAType) on a.PPATypeGUID equals c.ValueGUID
                                join d in DbPPA.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN).Where(x => x.codeTablesValues.TableGUID == LookupTables.PPAAreasOfImplementation) on a.ImplementationAreaGUID equals d.ValueGUID
                                join e in DbPPA.codeOrganizationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OrganizationInstanceGUID equals e.OrganizationGUID
                                join f in DbPPA.userProfiles.Where(x => x.Active) on a.CreatedByUserProfileGUID equals f.UserProfileGUID
                                join g in DbPPA.userServiceHistory.Where(x => x.Active) on f.ServiceHistoryGUID equals g.ServiceHistoryGUID
                                join h in DbPPA.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on g.UserGUID equals h.UserGUID
                                join i in DbPPA.codeDepartmentsLanguages.Where(x => x.Active && x.LanguageID == LAN) on f.DepartmentGUID equals i.DepartmentGUID
                                join j in DbPPA.codeDutyStationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on f.DutyStationGUID equals j.DutyStationGUID
                                select new PPAProgressTrackingReportModel
                                {
                                    PPAGUID = a.PPAGUID,
                                    PPADescription = a.PPADescription,
                                    PPATypeGUID = a.PPATypeGUID,
                                    PPATypeDescription = c.ValueDescription,
                                    OrganizationInstanceGUID = a.OrganizationInstanceGUID,
                                    OrganizationDescription = e.OrganizationDescription,
                                    PPADeadline = a.PPADeadLine,
                                    CreatedByUserProfileGUID = a.CreatedByUserProfileGUID,
                                    UserGUID = h.UserGUID,
                                    StaffName = h.FirstName + " " + h.Surname,
                                    DepartmentGUID = i.DepartmentGUID,
                                    DepartmentDescription = i.DepartmentDescription,
                                    DutyStationGUID = j.DutyStationGUID,
                                    DutyStationDescription = j.DutyStationDescription,
                                    UploadedToCount = a.PPADistributionList.Where(x => x.Active && x.PPAUserAccessType == PPAUserAccessType.FullAccess).Count(),
                                    UploadedToDepartment = (from a in a.PPADistributionList.Where(x => x.Active && (x.PPAUserAccessType == PPAUserAccessType.FullAccess || x.PPAUserAccessType == PPAUserAccessType.CC1WithAccess))
                                                            join b in DbPPA.userServiceHistory.Where(x => x.Active) on a.UserGUID equals b.UserGUID
                                                            join c in DbPPA.userProfiles.Where(x => x.Active) on b.ServiceHistoryGUID equals c.ServiceHistoryGUID
                                                            join d in DbPPA.codeDepartmentsLanguages.Where(x => x.Active && x.LanguageID == LAN) on c.DepartmentGUID equals d.DepartmentGUID
                                                            orderby c.FromDate descending
                                                            select d.DepartmentDescription
                                                            ).FirstOrDefault()
                                }).ToList();

            return View("~/Areas/PPA/Views/Reports/ProgressTrackingReport/PPAProgressReport.cshtml", PPADataModel);
        }

        [Route("PPA/Reports/ActionsOnPackageMoreInfo")]
        public ActionResult ActionsOnPackageMoreInfo(Guid PPAGUID)
        {

            List<ProgressTrackingReportActionMoreInfoModel> model = (from a in DbPPA.PPADistributionList.AsNoTracking().Where(x => x.Active && x.PPAGUID == PPAGUID)
                                                                     join b in DbPPA.userServiceHistory.AsNoTracking().Where(x => x.Active) on a.UserGUID equals b.UserGUID
                                                                     join c in DbPPA.userProfiles.AsNoTracking().Where(x => x.Active) on b.ServiceHistoryGUID equals c.ServiceHistoryGUID
                                                                     join d in DbPPA.codeDepartmentsLanguages.AsNoTracking().Where(x => x.Active && x.LanguageID == LAN) on c.DepartmentGUID equals d.DepartmentGUID
                                                                     join e in DbPPA.codeDutyStationsLanguages.AsNoTracking().Where(x => x.Active && x.LanguageID == LAN) on c.DutyStationGUID equals e.DutyStationGUID
                                                                     join f in DbPPA.userPersonalDetailsLanguage.AsNoTracking().Where(x => x.Active && x.LanguageID == LAN) on b.UserGUID equals f.UserGUID
                                                                     join j in DbPPA.PPAFileVersion.AsNoTracking().Where(x => x.Active) on b.UserGUID equals j.FileActionByUserGUID
                                                                     group new ProgressTrackingReportActionMoreInfoGroupModel
                                                                     {
                                                                         FileActionByUserGUID = j.FileActionByUserGUID.Value,
                                                                         DepartmentDescription = d.DepartmentDescription,
                                                                         DutyStationDescription = e.DutyStationDescription,
                                                                         FullName = f.FirstName + " " + f.Surname,
                                                                         FileActionDate = j.FileActionDate
                                                                     } by a.PPAUserAccessType into G
                                                                     select new ProgressTrackingReportActionMoreInfoModel
                                                                     {
                                                                         PPAUserAccessTypeGUID = G.Key,
                                                                         Details = (from x in G
                                                                                    select new ProgressTrackingReportActionMoreInfoDetailModel
                                                                                    {
                                                                                        FullName = x.FullName,
                                                                                        DutyStationDescription = x.DutyStationDescription,
                                                                                        DepartmentDescription = x.DepartmentDescription,
                                                                                        LastActionDate = (from asd in G where asd.FileActionByUserGUID == x.FileActionByUserGUID orderby asd.FileActionDate descending select asd.FileActionDate).FirstOrDefault()
                                                                                    }).Distinct()
                                                                     }).ToList();

            return PartialView("~/Areas/PPA/Views/Reports/ProgressTrackingReport/_ActionsOnPackageMoreInfo.cshtml", model);
        }

        public ActionResult GetPPAFiles(Guid FK)
        {
            Guid PPAGUID = FK;
            var model = (from a in DbPPA.PPAOriginalFile.AsNoTracking().Where(x => x.PPAGUID == PPAGUID)
                         join b in DbPPA.userProfiles.AsNoTracking().Where(x => x.Active) on a.UploadByUserGUID equals b.UserProfileGUID
                         join c in DbPPA.userServiceHistory.AsNoTracking().Where(x => x.Active) on b.ServiceHistoryGUID equals c.ServiceHistoryGUID
                         join d in DbPPA.userPersonalDetailsLanguage.AsNoTracking().Where(x => x.Active && x.LanguageID == LAN) on c.UserGUID equals d.UserGUID
                         join e in DbPPA.codeDepartmentsLanguages.AsNoTracking().Where(x => x.Active && x.LanguageID == LAN) on b.DepartmentGUID equals e.DepartmentGUID
                         join f in DbPPA.codeDutyStationsLanguages.AsNoTracking().Where(x => x.Active && x.LanguageID == LAN) on b.DutyStationGUID equals f.DutyStationGUID
                         join j in DbPPA.codeTablesValuesLanguages.AsNoTracking().Where(x => x.Active && x.LanguageID == LAN) on a.PPAFileCategoryGUID equals j.ValueGUID
                         select new ProgressTrackingReportOriginalFilesModel
                         {
                             PPAOriginalFileGUID = a.PPAOriginalFileGUID,
                             FileName = a.FileName,
                             UploadDate = a.UploadDate,
                             UploadByUserGUID = a.UploadByUserGUID,
                             FullName = d.FirstName + " " + d.Surname,
                             DepartmentDescription = e.DepartmentDescription,
                             DutyStationDescription = f.DutyStationDescription,
                             PPAFileCategoryGUID = a.PPAFileCategoryGUID,
                             PPAFileCategoryDescription = j.ValueDescription,
                             PPAGUID = a.PPAGUID
                         }).ToList();
            return PartialView("~/Areas/PPA/Views/Reports/ProgressTrackingReport/_PPAProgresReportOriginaFiles.cshtml", model);
        }

        public ActionResult GetPPAFilesHistory(Guid FK)
        {
            Guid PPAOriginalFileGUID = FK;

            var model = (from a in DbPPA.PPAFileVersion.AsNoTracking().Where(x => x.PPAOriginalFileGUID == PPAOriginalFileGUID)
                         join b in DbPPA.userPersonalDetailsLanguage.AsNoTracking().Where(x => x.Active && x.LanguageID == LAN) on a.FileActionByUserGUID equals b.UserGUID

                         select new ProgressTrackingReportFileHistoryModel
                         {
                             PPAFileVersionGUID = a.PPAFileVersionGUID,
                             FileActionByUserGUID = a.FileActionByUserGUID.Value,
                             FileActionByUserFullName = b.FirstName + " " + b.Surname,
                             FileActionDate = a.FileActionDate,
                             Comments = a.Comments,
                             FileActionType = a.FileActionType,
                             UserDepartment = (from r in DbPPA.userServiceHistory.Where(x => x.Active && x.UserGUID == a.FileActionByUserGUID)
                                               join t in DbPPA.userProfiles.Where(x => x.Active) on r.ServiceHistoryGUID equals t.ServiceHistoryGUID
                                               join y in DbPPA.codeDepartmentsLanguages.Where(x => x.Active && x.LanguageID == LAN) on t.DepartmentGUID equals y.DepartmentGUID
                                               orderby t.FromDate descending
                                               select y.DepartmentDescription).FirstOrDefault()
                         }).ToList();

            return PartialView("~/Areas/PPA/Views/Reports/ProgressTrackingReport/_PPAProgresReportFileHistory.cshtml", model);
        }
        #endregion


        #region PPA Progress Dashboard
        public ActionResult PPADashboard()
        {
            return View("~/Areas/PPA/Views/Reports/PPADashboard/PPADashboard.cshtml");
        }

        class HighChartPieModel
        {
            public string name { set; get; }
            public double y { set; get; }
            public bool selected { get; set; }

            public bool sliced { get; set; }
        }

        public JsonResult PPAProgressDashboard()
        {
            var test = (from a in DbPPA.ProjectPartnershipAgreement.AsNoTracking().Where(x => x.Active)
                        join b in DbPPA.codeTablesValuesLanguages.AsNoTracking().Where(x => x.LanguageID == LAN) on a.PPAStatusGUID equals b.ValueGUID
                        group a by b.ValueDescription into g
                        select new HighChartPieModel
                        {
                            name = g.Key,
                            y = g.Count(),
                            selected = false,
                            sliced = false
                        }).ToList();

 

            return Json(test, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region
        public ActionResult PPAProgressLevelDashboard()
        {
            //E325612C-1D84-4770-97AE-51681A8F1533 pending
            //
            //43344AA3-E340-4CAF-9045-CB87DACE808E cleared
            var result = (from a in DbPPA.ProjectPartnershipAgreement.AsNoTracking().Where(x => x.Active)
                          join b in DbPPA.codeTablesValuesLanguages.AsNoTracking().Where(x => x.LanguageID == LAN) on a.PPAStatusGUID equals b.ValueGUID
                          select new
                          {
                              text = a.PPADescription,
                              value = a.PPAStatusGUID == Guid.Parse("E325612C-1D84-4770-97AE-51681A8F1533") ? 25 : a.PPAStatusGUID == Guid.Parse("43344AA3-E340-4CAF-9045-CB87DACE808E") ? 100 : 75,
                          }).ToList();
            return View("~/Areas/PPA/Views/Reports/PPAProgressLevelDashboard/PPAProgressLevelDashboard.cshtml");
        }
        #endregion

        [Route("PPA/Reports/PPAStatusReport")]
        public ActionResult PPAStatusReport()
        {

            Guid PPAGUID = Guid.Parse("918E40FC-99C3-423F-BE7B-B29EB02C5971");

            //var Result = (from a in DbPPA.ProjectPartnershipAgreement.Where(x=>x.Active && x.PPAGUID == PPAGUID)
            //              )
            return View("~/Areas/PPA/Views/Reports/PPAStatusReport.cshtml");
        }
    }
}