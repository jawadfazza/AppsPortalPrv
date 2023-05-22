using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Library.MimeDetective;
using AppsPortal.ViewModels;
using AutoMapper;
using FineUploader;
using LinqKit;
using SRS_DAL.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.SRS.Controllers
{
    public class HelpDeskController : SRSBaseController
    {

        private Guid newApplication = Guid.Parse("daaeaa0c-f441-4543-b533-b08bbf41a657");
        private Guid applicationEnhancement = Guid.Parse("40c09a21-596b-480d-9350-0c9b089ccc72");
        private Guid bugReport = Guid.Parse("81008ea8-9b63-4b76-bff3-24ee0b41f0a6");
        private Guid changeRequest = Guid.Parse("AE99EF9D-6831-461D-8A02-3692AD160F59");

        private Guid RequestStatusGUID_Pending = Guid.Parse("4E8E7CEB-D3B3-4FE8-AFB4-4DB512F19AFD");
        private Guid RequestStatusGUID_InProcess = Guid.Parse("AB62ADB5-E157-4CF7-B1E9-C2DF85F310BD");
        private Guid RequestStatusGUID_Completed = Guid.Parse("171386F5-274E-4E4F-AB4E-AB58E5CBC1B3");

        //private string ToEmail = "karkoush@unhcr.org";
        private string ToEmail = "karkoush@unhcr.org;alfazzaa@unhcr.org;isac@unhcr.org;maksoud@unhcr.org;shaban@unhcr.org;";
        //private string ToEmail = "karkoush@unhcr.org;alfazzaa@unhcr.org;isac@unhcr.org;maksoud@unhcr.org;shaban@unhcr.org;shaglil@unhcr.org;HOMSSI@unhcr.org;AYDI@unhcr.org;sahhar@unhcr.org;";
        // GET: SRS/HelpDesk
        public ActionResult Index()
        {
            return View();
        }

        [Route("SRS/HelpDesk")]
        public ActionResult HelpDeskIndex()
        {
            return View("~/Areas/SRS/Views/HelpDesk/Index.cshtml");
        }

        [Route("SRS/HelpDesk/HelpDeskDataTable")]
        public ActionResult HelpDeskDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<HelpDeskDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<HelpDeskDataTableModel>(DataTable.Filters);
            }

            if (CMS.HasAction(Permissions.Bugreportmanagement.Update, Apps.SRS)
                || CMS.HasAction(Permissions.Newservicerequest.Update, Apps.SRS)
                || CMS.HasAction(Permissions.Serviceenhancementrequest.Update, Apps.SRS))
            {
                var All = (from a in DbSRS.dataHelpDesk.AsExpandable().Where(x => x.Active)
                           join b in DbSRS.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.ConfigItemGUID equals b.ValueGUID
                           join c in DbSRS.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.CriticalityGUID equals c.ValueGUID
                           join d in DbSRS.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.RequestStatusGUID equals d.ValueGUID
                           join e in DbSRS.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.WorkGroupGUID equals e.ValueGUID
                           join f in DbSRS.userProfiles.AsExpandable().Where(x => x.Active) on a.RequestedByProfileGUID equals f.UserProfileGUID
                           join g in DbSRS.userServiceHistory.AsExpandable().Where(x => x.Active) on f.ServiceHistoryGUID equals g.ServiceHistoryGUID
                           join h in DbSRS.userPersonalDetailsLanguage.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on g.UserGUID equals h.UserGUID
                           join i in DbSRS.codeDutyStationsLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on f.DutyStationGUID equals i.DutyStationGUID
                           join j in DbSRS.codeDepartmentsLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on f.DepartmentGUID equals j.DepartmentGUID
                           join k in DbSRS.userProfiles.AsExpandable().Where(x => x.Active) on a.AssignedToGUID equals k.UserProfileGUID into LJ1
                           from R1 in LJ1.DefaultIfEmpty()
                           join l in DbSRS.userServiceHistory.AsExpandable().Where(x => x.Active) on R1.ServiceHistoryGUID equals l.ServiceHistoryGUID into LJ2
                           from R2 in LJ2.DefaultIfEmpty()
                           join m in DbSRS.userPersonalDetailsLanguage.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on R2.UserGUID equals m.UserGUID into LJ3
                           from R3 in LJ3.DefaultIfEmpty()
                           select new HelpDeskDataTableModel
                           {
                               HelpDeskGUID = a.HelpDeskGUID,
                               RequestNumber = a.RequestNumber,
                               RequestedByProfileGUID = a.RequestedByProfileGUID,
                               RequestedByFullName = h.FirstName + " " + h.Surname,
                               RequestorDutyStationGUID = f.DutyStationGUID.Value,
                               RequestorDutyStationDescription = i.DutyStationDescription,
                               RequestorDepartmentGUID = f.DepartmentGUID.Value,
                               RequestorDepartmentDescription = j.DepartmentDescription,
                               ConfigItemGUID = a.ConfigItemGUID,
                               ConfigItemDescription = b.ValueDescription,
                               CriticalityGUID = a.CriticalityGUID,
                               CriticalityDescription = c.ValueDescription,
                               WorkGroupGUID = a.WorkGroupGUID,
                               WorkGroupDescription = e.ValueDescription,
                               RequestStatusGUID = a.RequestStatusGUID,
                               RequestStatusDescription = d.ValueDescription,
                               AssignedToGUID = a.AssignedToGUID,
                               AssignedToFullName = R3.FirstName + " " + R3.Surname,
                               RequestCreateDate = a.RequestCreateDate,
                               RequestUpdateDate = a.RequestUpdateDate,
                               Active = a.Active,
                               dataHelpDeskRowVersion = a.dataHelpDeskRowVersion
                           }).Where(Predicate).Distinct();

                All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

                List<HelpDeskDataTableModel> Result = Mapper.Map<List<HelpDeskDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

                return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var All = (from a in DbSRS.dataHelpDesk.AsExpandable().Where(x => x.Active && x.RequestedByProfileGUID == UserProfileGUID)
                           join b in DbSRS.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.ConfigItemGUID equals b.ValueGUID
                           join c in DbSRS.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.CriticalityGUID equals c.ValueGUID
                           join d in DbSRS.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.RequestStatusGUID equals d.ValueGUID
                           join e in DbSRS.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.WorkGroupGUID equals e.ValueGUID
                           join f in DbSRS.userProfiles.AsExpandable().Where(x => x.Active) on a.RequestedByProfileGUID equals f.UserProfileGUID
                           join g in DbSRS.userServiceHistory.AsExpandable().Where(x => x.Active) on f.ServiceHistoryGUID equals g.ServiceHistoryGUID
                           join h in DbSRS.userPersonalDetailsLanguage.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on g.UserGUID equals h.UserGUID
                           join i in DbSRS.codeDutyStationsLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on f.DutyStationGUID equals i.DutyStationGUID
                           join j in DbSRS.codeDepartmentsLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on f.DepartmentGUID equals j.DepartmentGUID
                           join k in DbSRS.userProfiles.AsExpandable().Where(x => x.Active) on a.AssignedToGUID equals k.UserProfileGUID into LJ1
                           from R1 in LJ1.DefaultIfEmpty()
                           join l in DbSRS.userServiceHistory.AsExpandable().Where(x => x.Active) on R1.ServiceHistoryGUID equals l.ServiceHistoryGUID into LJ2
                           from R2 in LJ2.DefaultIfEmpty()
                           join m in DbSRS.userPersonalDetailsLanguage.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on R2.UserGUID equals m.UserGUID into LJ3
                           from R3 in LJ3.DefaultIfEmpty()
                           select new HelpDeskDataTableModel
                           {
                               HelpDeskGUID = a.HelpDeskGUID,
                               RequestNumber = a.RequestNumber,
                               RequestedByProfileGUID = a.RequestedByProfileGUID,
                               RequestedByFullName = h.FirstName + " " + h.Surname,
                               RequestorDutyStationGUID = f.DutyStationGUID.Value,
                               RequestorDutyStationDescription = i.DutyStationDescription,
                               RequestorDepartmentGUID = f.DepartmentGUID.Value,
                               RequestorDepartmentDescription = j.DepartmentDescription,
                               ConfigItemGUID = a.ConfigItemGUID,
                               ConfigItemDescription = b.ValueDescription,
                               CriticalityGUID = a.CriticalityGUID,
                               CriticalityDescription = c.ValueDescription,
                               WorkGroupGUID = a.WorkGroupGUID,
                               WorkGroupDescription = e.ValueDescription,
                               RequestStatusGUID = a.RequestStatusGUID,
                               RequestStatusDescription = d.ValueDescription,
                               AssignedToGUID = a.AssignedToGUID,
                               AssignedToFullName = R3.FirstName + " " + R3.Surname,
                               RequestCreateDate = a.RequestCreateDate,
                               RequestUpdateDate = a.RequestUpdateDate,
                               Active = a.Active,
                               dataHelpDeskRowVersion = a.dataHelpDeskRowVersion
                           }).Where(Predicate).Distinct();



                All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

                List<HelpDeskDataTableModel> Result = Mapper.Map<List<HelpDeskDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

                return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
            }
            return null;

        }


        [Route("SRS/HelpDesk/Create")]
        public ActionResult HelpDeskCreate()
        {
            int RequestNumber = 1;
            try { RequestNumber = (from a in DbSRS.dataHelpDesk.AsNoTracking() select a.RequestNumber).Max() + 1; } catch { }
            return View("~/Areas/SRS/Views/HelpDesk/User/HelpDesk.cshtml",
                new HelpDeskModel
                {
                    RequestNumber = RequestNumber,
                    RequestStatusGUID = RequestStatusGUID_Pending,

                });
        }

        [Route("SRS/HelpDesk/Update/{PK}")]
        public ActionResult HelpDeskUpdate(Guid PK)
        {
            HelpDeskModel helpDeskModel = (from a in DbSRS.dataHelpDesk.Where(x => x.HelpDeskGUID == PK)
                                           join b in DbSRS.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.ConfigItemGUID equals b.ValueGUID
                                           join c in DbSRS.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.CriticalityGUID equals c.ValueGUID
                                           join d in DbSRS.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.RequestStatusGUID equals d.ValueGUID
                                           join e in DbSRS.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.WorkGroupGUID equals e.ValueGUID
                                           join f in DbSRS.userProfiles.AsExpandable().Where(x => x.Active) on a.AssignedToGUID equals f.UserProfileGUID into LJ1
                                           from R1 in LJ1.DefaultIfEmpty()
                                           join j in DbSRS.userServiceHistory.Where(x => x.Active) on R1.ServiceHistoryGUID equals j.ServiceHistoryGUID into LJ2
                                           from R2 in LJ2.DefaultIfEmpty()
                                           join h in DbSRS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on R2.UserGUID equals h.UserGUID into LJ3
                                           from R3 in LJ3.DefaultIfEmpty()
                                           select new HelpDeskModel
                                           {
                                               HelpDeskGUID = a.HelpDeskGUID,
                                               RequestNumber = a.RequestNumber,
                                               RequestedByProfileGUID = a.RequestedByProfileGUID,
                                               ConfigItemGUID = a.ConfigItemGUID,
                                               ConfigItemDescription = b.ValueDescription,
                                               CriticalityGUID = a.CriticalityGUID,
                                               CriticalityDescription = c.ValueDescription,
                                               RequestStatusGUID = a.RequestStatusGUID,
                                               RequestStatusDescription = d.ValueDescription,
                                               WorkGroupGUID = a.WorkGroupGUID,
                                               WorkGroupDescription = e.ValueDescription,
                                               RequestCreateDate = a.RequestCreateDate,
                                               RequestUpdateDate = a.RequestUpdateDate,
                                               AssignedToGUID = a.AssignedToGUID,
                                               AssignedToFullName = R3.FirstName + " " + R3.Surname,
                                               Active = a.Active,
                                               dataHelpDeskRowVersion = a.dataHelpDeskRowVersion,
                                           }).FirstOrDefault();

            if (helpDeskModel.ConfigItemGUID == newApplication)
            {
                helpDeskModel.HelpDeskNewApplicationModel = (from a in DbSRS.dataApplicationRequest.Where(x => x.HelpDeskGUID == helpDeskModel.HelpDeskGUID && x.Active)
                                                             select new HelpDeskNewApplicationModel
                                                             {
                                                                 AppRequestGUID = a.AppRequestGUID,
                                                                 HelpDeskGUID = a.HelpDeskGUID,
                                                                 IsNewDbForTracking = a.IsNewDbForTracking,
                                                                 IsToAutomateManual = a.IsToAutomateManual,
                                                                 IsToReplaceOldApp = a.IsToReplaceOldApp,
                                                                 RequestedAppName = a.RequestedAppName,
                                                                 PurposDescriptionBenefits = a.PurposDescriptionBenefits,
                                                                 Active = a.Active,
                                                                 dataApplicationRequestRowVersion = a.dataApplicationRequestRowVersion,

                                                                 CPUDescription = a.CPUDescription,
                                                                 OffsiteBackup = a.OffsiteBackup,
                                                                 OnsiteBackup = a.OnsiteBackup,
                                                                 RAMDescription = a.RAMDescription,
                                                                 ServerOrVM = a.ServerOrVM,
                                                                 StorageDescription = a.StorageDescription,
                                                                 OfflineVersion = a.OfflineVersion.Value,
                                                                 AndroidVersion = a.AndroidVersion.Value,
                                                                 IOSVersion = a.IOSVersion.Value

                                                             }).FirstOrDefault();
                if (helpDeskModel.HelpDeskNewApplicationModel != null)
                {
                    helpDeskModel.HelpDeskNewApplicationAuthorizationModel = (from a in DbSRS.dataAppRequestAuthorization.Where(x => x.AppRequestGUID == helpDeskModel.HelpDeskNewApplicationModel.AppRequestGUID && x.Active)
                                                                              join b in DbSRS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.ApprovalStatusGUID equals b.ValueGUID into LJ1
                                                                              from R1 in LJ1.DefaultIfEmpty()
                                                                                  //join c in DbSRS.userProfiles.Where(x=>x.Active) on a.ApprovedByProfileGUID equals c.UserProfileGUID
                                                                                  //join d in DbSRS.userServiceHistory.Where(x=>x.Active) on c.ServiceHistoryGUID equals d.ServiceHistoryGUID
                                                                                  //join e in DbSRS.userPersonalDetailsLanguage.Where(x=>x.Active && x.LanguageID == LAN ) on d.UserGUID equals e.UserGUID
                                                                              join f in DbSRS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.EstimatedTime equals f.ValueGUID into LJ2
                                                                              from R2 in LJ2.DefaultIfEmpty()
                                                                              select new HelpDeskNewApplicationAuthorizationModel
                                                                              {
                                                                                  AppRequestAuthorizationGUID = a.AppRequestAuthorizationGUID,
                                                                                  AppRequestGUID = a.AppRequestGUID,
                                                                                  ApprovalStatusGUID = a.ApprovalStatusGUID,
                                                                                  ApprovalStatusDescription = R1.ValueDescription,
                                                                                  ApprovedByProfileGUID = a.ApprovedByProfileGUID,
                                                                                  //ApprovedByFullName = e.FirstName + " " + e.Surname,
                                                                                  ApprovalComments = a.ApprovalComments,
                                                                                  ApplicationEndDate = a.ApplicationEndDate,
                                                                                  ApplicationStartDate = a.ApplicationStartDate,
                                                                                  EstimatedResourcesNum = a.EstimatedResourcesNum,
                                                                                  EstimatedTime = a.EstimatedTime,
                                                                                  EstimatedTimeDescription = R2.ValueDescription,
                                                                                  Active = a.Active,
                                                                                  dataAppRequestAuthorizationRowVersion = a.dataAppRequestAuthorizationRowVersion
                                                                              }).FirstOrDefault();
                }
            }
            else if (helpDeskModel.ConfigItemGUID == applicationEnhancement)
            {
                helpDeskModel.HelpDeskApplicationEnhancementModel = (from a in DbSRS.dataAppEnhancementRequest.Where(x => x.HelpDeskGUID == helpDeskModel.HelpDeskGUID && x.Active)
                                                                     join b in DbSRS.codeApplicationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.ApplicationGUID equals b.ApplicationGUID
                                                                     select new HelpDeskApplicationEnhancementModel
                                                                     {
                                                                         AppEnhancementGUID = a.AppEnhancementGUID,
                                                                         HelpDeskGUID = a.HelpDeskGUID,
                                                                         ApplicationGUID = a.ApplicationGUID,
                                                                         ApplicationDescription = b.ApplicationDescription,
                                                                         EnhancementPurpos = a.EnhancementPurpos,
                                                                         EnhancementBenefits = a.EnhancementBenefits,
                                                                         EnhancementDetails = a.EnhancementDetails,
                                                                         Active = a.Active,
                                                                         dataAppEnhancementRequestRowVersion = a.dataAppEnhancementRequestRowVersion
                                                                     }).FirstOrDefault();
                if (helpDeskModel.HelpDeskApplicationEnhancementModel != null)
                {
                    helpDeskModel.HelpDeskApplicationEnhancementAuthorizationModel = (from a in DbSRS.dataAppEnhancementAuthorization.Where(x => x.Active && x.AppEnhancementGUID == helpDeskModel.HelpDeskApplicationEnhancementModel.AppEnhancementGUID)
                                                                                      join b in DbSRS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.ApprovalStatusGUID equals b.ValueGUID into LJ1
                                                                                      from R1 in LJ1.DefaultIfEmpty()
                                                                                          //join c in DbSRS.userProfiles.Where(x => x.Active) on a.ApprovedByProfileGUID equals c.UserProfileGUID
                                                                                          //join d in DbSRS.userServiceHistory.Where(x => x.Active) on c.ServiceHistoryGUID equals d.ServiceHistoryGUID
                                                                                          //join e in DbSRS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on d.UserGUID equals e.UserGUID
                                                                                      join f in DbSRS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.EstimatedTime equals f.ValueGUID into LJ2
                                                                                      from R2 in LJ2.DefaultIfEmpty()
                                                                                      select new HelpDeskApplicationEnhancementAuthorizationModel
                                                                                      {
                                                                                          AppEnhancementAuthorizationGUID = a.AppEnhancementAuthorizationGUID,
                                                                                          AppEnhancementGUID = a.AppEnhancementGUID,
                                                                                          ApprovalStatusGUID = a.ApprovalStatusGUID,
                                                                                          ApprovalStatusDescription = R1.ValueDescription,
                                                                                          ApprovedByProfileGUID = a.ApprovedByProfileGUID,
                                                                                          //ApprovedByFullName = e.FirstName + " " + e.Surname,
                                                                                          ApprovalComments = a.ApprovalComments,
                                                                                          EnhancementStartDate = a.EnhancementStartDate,
                                                                                          EnhancementEndDate = a.EnhancementEndDate,
                                                                                          EstimatedResourcesNum = a.EstimatedResourcesNum,
                                                                                          EstimatedTime = a.EstimatedTime,
                                                                                          EstimatedTimeDescription = R2.ValueDescription,
                                                                                          Active = a.Active,
                                                                                          dataAppEnhancementAuthorizationRowVersion = a.dataAppEnhancementAuthorizationRowVersion
                                                                                      }).FirstOrDefault();
                }
            }
            else if (helpDeskModel.ConfigItemGUID == bugReport)
            {
                helpDeskModel.HelpDeskBugReportModel = (from a in DbSRS.dataAppBugReport.Where(x => x.Active && x.HelpDeskGUID == helpDeskModel.HelpDeskGUID)
                                                        join b in DbSRS.codeApplicationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.ApplicationGUID equals b.ApplicationGUID
                                                        select new HelpDeskBugReportModel
                                                        {
                                                            AppBugReportGUID = a.AppBugReportGUID,
                                                            HelpDeskGUID = a.HelpDeskGUID,
                                                            ApplicationGUID = a.ApplicationGUID,

                                                            ApplicationDescription = b.ApplicationDescription,
                                                            BugDetails = a.BugDetails,
                                                            Active = a.Active,
                                                            dataAppBugReportRowVersion = a.dataAppBugReportRowVersion
                                                        }).FirstOrDefault();

                if (helpDeskModel.HelpDeskBugReportModel != null)
                {
                    helpDeskModel.HelpDeskBugReportAuthorizationModel = (from a in DbSRS.dataAppBugAuthorization.Where(x => x.Active && x.AppBugReportGUID == helpDeskModel.HelpDeskBugReportModel.AppBugReportGUID)
                                                                         join b in DbSRS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.ApprovalStatusGUID equals b.ValueGUID into LJ1
                                                                         from R1 in LJ1.DefaultIfEmpty()
                                                                             //join c in DbSRS.userProfiles.Where(x => x.Active) on a.ApprovedByProfileGUID equals c.UserProfileGUID
                                                                             //join d in DbSRS.userServiceHistory.Where(x => x.Active) on c.ServiceHistoryGUID equals d.ServiceHistoryGUID
                                                                             //join e in DbSRS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on d.UserGUID equals e.UserGUID
                                                                         join f in DbSRS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.EstimatedTime equals f.ValueGUID into LJ2
                                                                         from R2 in LJ2.DefaultIfEmpty()
                                                                         select new HelpDeskBugReportAuthorizationModel
                                                                         {
                                                                             AppBugAuthorizationGUID = a.AppBugAuthorizationGUID,
                                                                             AppBugReportGUID = a.AppBugReportGUID,
                                                                             ApprovedByProfileGUID = a.ApprovedByProfileGUID,
                                                                             //ApprovedByFullName = e.FirstName + " " + e.Surname,
                                                                             ApprovalStatusGUID = a.ApprovalStatusGUID,
                                                                             ApprovalStatusDescription = R1.ValueDescription,
                                                                             ApprovalComments = a.ApprovalComments,
                                                                             BugAnalysis = a.BugAnalysis,
                                                                             BugResolution = a.BugResolution,
                                                                             BugFixStartDate = a.BugFixStartDate,
                                                                             BugFixEndDate = a.BugFixEndDate,
                                                                             EstimatedResourcesNum = a.EstimatedResourcesNum,
                                                                             EstimatedTime = a.EstimatedTime,
                                                                             EstimatedTimeDescription = R2.ValueDescription,
                                                                             Active = a.Active,
                                                                             dataAppBugAuthorizationRowVersion = a.dataAppBugAuthorizationRowVersion
                                                                         }).FirstOrDefault();
                }
            }
            else if (helpDeskModel.ConfigItemGUID == changeRequest)
            {

            }
            if (CMS.HasAction(Permissions.Newservicerequest.Update, Apps.SRS))
            {
                return View("~/Areas/SRS/Views/HelpDesk/Admin/HelpDesk.cshtml", helpDeskModel);
            }
            else
            {
                return View("~/Areas/SRS/Views/HelpDesk/User/HelpDesk.cshtml", helpDeskModel);
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult HelpDeskCreate(HelpDeskModel model)
        {
            dataHelpDesk dataHelpDesk = new dataHelpDesk();
            dataApplicationRequest dataApplicationRequest = new dataApplicationRequest();
            dataAppEnhancementRequest dataAppEnhancementRequest = new dataAppEnhancementRequest();
            dataAppBugReport dataAppBugReport = new dataAppBugReport();
            dataChangeRequest dataChangeRequest = new dataChangeRequest();
            Guid HelpDeskGUID = Guid.NewGuid();
            dataHelpDesk = Mapper.Map(model, new dataHelpDesk());
            try
            {
                dataHelpDesk.RequestNumber = (from a in DbSRS.dataHelpDesk.AsNoTracking() select a.RequestNumber).Max() + 1;
            }
            catch
            {
                dataHelpDesk.RequestNumber = 1;
            }
            dataHelpDesk.RequestedByProfileGUID = UserProfileGUID;
            dataHelpDesk.RequestCreateDate = DateTime.Now;
            dataHelpDesk.HelpDeskGUID = HelpDeskGUID;
            dataHelpDesk.RequestStatusGUID = RequestStatusGUID_Pending;
            DbSRS.CreateNoAudit(dataHelpDesk);

            if (model.ConfigItemGUID == newApplication)
            {
                dataApplicationRequest = Mapper.Map(model.HelpDeskNewApplicationModel, new dataApplicationRequest());
                dataApplicationRequest.HelpDeskGUID = HelpDeskGUID;
                DbSRS.CreateNoAudit(dataApplicationRequest);
            }
            else if (model.ConfigItemGUID == applicationEnhancement)
            {
                dataAppEnhancementRequest = Mapper.Map(model.HelpDeskApplicationEnhancementModel, new dataAppEnhancementRequest());
                dataAppEnhancementRequest.HelpDeskGUID = HelpDeskGUID;
                DbSRS.CreateNoAudit(dataAppEnhancementRequest);
            }
            else if (model.ConfigItemGUID == bugReport)
            {
                dataAppBugReport = Mapper.Map(model.HelpDeskBugReportModel, new dataAppBugReport());
                dataAppBugReport.HelpDeskGUID = HelpDeskGUID;
                DbSRS.CreateNoAudit(dataAppBugReport);
            }
            else if (model.ConfigItemGUID == changeRequest)
            {
                dataChangeRequest = Mapper.Map(model.helpDeskChangeRequestModel, new dataChangeRequest());
                dataChangeRequest.HelpDeskGUID = HelpDeskGUID;
                DbSRS.CreateNoAudit(dataChangeRequest);
                List<dataChangeRequestActionPlan> dataChangeRequestActionPlans = new List<dataChangeRequestActionPlan>();
                foreach (var item in model.helpDeskChangeRequestModel.CRActionPlanList)
                {
                    dataChangeRequestActionPlan dataChangeRequestActionPlan = Mapper.Map(item, new dataChangeRequestActionPlan());
                    dataChangeRequestActionPlan.ChangeRequestGUID = dataChangeRequest.ChangeRequestGUID;
                    dataChangeRequestActionPlans.Add(dataChangeRequestActionPlan);
                }
                DbSRS.CreateBulkNoAudit(dataChangeRequestActionPlans);

            }

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(HelpDeskGUID, DataTableNames.HelpDeskAttachementDataTable, ControllerContext, "HelpDeskAttachementContainer"));
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButtonNoPermission(new UrlHelper(Request.RequestContext).Action("Create", "HelpDesk", new { Area = "SRS" })), Container = "HelpDeskFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButtonNoPermission(), Container = "HelpDeskFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButtonNoPermission(), Container = "HelpDeskFormControls" });

            try
            {
                DbSRS.SaveChanges();
                DbCMS.SaveChanges();
                UserNameAndEmailModel UserNameAndEmailModel = (from a in DbSRS.userProfiles.Where(x => x.Active && x.UserProfileGUID == UserProfileGUID)
                                                               join b in DbSRS.userServiceHistory.Where(x => x.Active) on a.ServiceHistoryGUID equals b.ServiceHistoryGUID
                                                               join c in DbSRS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == "EN") on b.UserGUID equals c.UserGUID
                                                               orderby a.FromDate descending
                                                               select new UserNameAndEmailModel
                                                               {
                                                                   UserFullName = c.FirstName + " " + c.Surname,
                                                                   EmailAddress = b.EmailAddress
                                                               }).FirstOrDefault();


                if (model.ConfigItemGUID == newApplication)
                {
                    new Email().SendHelpDeskRequest(
                       RecipientEmail: ToEmail,
                       CCList: "",
                       Type: "New Application Request",
                       Action: "HelpDesk",
                       subject: "Ticket Number " + dataHelpDesk.RequestNumber.ToString() + " || Application Request - " + model.HelpDeskNewApplicationModel.RequestedAppName.ToString() + " - Created by: " + UserNameAndEmailModel.UserFullName,
                       PK: dataHelpDesk.HelpDeskGUID);
                    new Email().SendHelpDeskNotify(
                        RecipientEmail: UserNameAndEmailModel.EmailAddress,
                        RecipientName: UserNameAndEmailModel.UserFullName,
                        CCList: "",
                        Type: "Application Request Notification",
                        subject: "Ticket Number " + dataHelpDesk.RequestNumber.ToString() + " || Application Request - " + model.HelpDeskNewApplicationModel.RequestedAppName.ToString() + " - Created by: " + UserNameAndEmailModel.UserFullName,
                        Action: "HelpDesk",
                        PK: dataHelpDesk.HelpDeskGUID,
                        RequestNumber: dataHelpDesk.RequestNumber);
                    return Json(DbSRS.SingleCreateMessage(DbSRS.PrimaryKeyControl(dataHelpDesk), DbSRS.RowVersionControls(dataHelpDesk, dataApplicationRequest), Partials, "", UIButtons));
                }
                else if (model.ConfigItemGUID == applicationEnhancement)
                {
                    string appName = (from a in DbSRS.codeApplicationsLanguages.Where(x => x.Active && x.LanguageID == LAN && x.ApplicationGUID == model.HelpDeskApplicationEnhancementModel.ApplicationGUID)
                                      select "[" + a.codeApplications.ApplicationAcrynom + "]" + " " + a.ApplicationDescription).FirstOrDefault();
                    new Email().SendHelpDeskRequest(
                       RecipientEmail: ToEmail,
                       CCList: "",
                       Type: "New Enhancement Request",
                       Action: "HelpDesk",
                       subject: "Ticket Number " + dataHelpDesk.RequestNumber.ToString() + " || Enhancement Request - " + appName + " - Created by: " + UserNameAndEmailModel.UserFullName,
                       PK: dataHelpDesk.HelpDeskGUID);
                    new Email().SendHelpDeskNotify(
                       RecipientEmail: UserNameAndEmailModel.EmailAddress,
                       RecipientName: UserNameAndEmailModel.UserFullName,
                       CCList: "",
                       Type: "Enhancement Request Notification",
                       subject: "Ticket Number " + dataHelpDesk.RequestNumber.ToString() + " || Enhancement Request - " + appName + " - Created by: " + UserNameAndEmailModel.UserFullName,
                       Action: "HelpDesk",
                       PK: dataHelpDesk.HelpDeskGUID,
                       RequestNumber: dataHelpDesk.RequestNumber);
                    return Json(DbSRS.SingleCreateMessage(DbSRS.PrimaryKeyControl(dataHelpDesk), DbSRS.RowVersionControls(dataHelpDesk, dataAppEnhancementRequest), Partials, "", UIButtons));
                }
                else if (model.ConfigItemGUID == bugReport)
                {
                    string appName = (from a in DbSRS.codeApplicationsLanguages.Where(x => x.Active && x.LanguageID == LAN && x.ApplicationGUID == model.HelpDeskBugReportModel.ApplicationGUID)
                                      select "[" + a.codeApplications.ApplicationAcrynom + "]" + " " + a.ApplicationDescription).FirstOrDefault();
                    new Email().SendHelpDeskRequest(
                       RecipientEmail: ToEmail,
                       CCList: "",
                       Type: "New Bug Report",
                       Action: "HelpDesk",
                       subject: "Ticket Number " + dataHelpDesk.RequestNumber.ToString() + " || Bug Report - " + appName + " - Created by: " + UserNameAndEmailModel.UserFullName,
                       PK: dataHelpDesk.HelpDeskGUID);
                    new Email().SendHelpDeskNotify(
                        RecipientEmail: UserNameAndEmailModel.EmailAddress,
                        RecipientName: UserNameAndEmailModel.UserFullName,
                        CCList: "",
                        Type: "New Bug Report",
                        subject: "Ticket Number " + dataHelpDesk.RequestNumber.ToString() + " || Bug Report - " + appName + " - Created by: " + UserNameAndEmailModel.UserFullName,
                        Action: "HelpDesk",
                        PK: dataHelpDesk.HelpDeskGUID,
                        RequestNumber: dataHelpDesk.RequestNumber);
                    return Json(DbSRS.SingleCreateMessage(DbSRS.PrimaryKeyControl(dataHelpDesk), DbSRS.RowVersionControls(dataHelpDesk, dataAppBugReport), Partials, "", UIButtons));
                }

            }
            catch (Exception ex)
            {

            }
            return View("~/Areas/SRS/Views/HelpDesk/HelpDesk.cshtml", new HelpDeskModel());
        }
        public class UserNameAndEmailModel
        {
            public string UserFullName { get; set; }
            public string EmailAddress { get; set; }
        }
        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult HelpDeskAdminUpdate(HelpDeskModel model)
        {
            DateTime ExecutionTime = DateTime.Now;
            dataHelpDesk dataHelpDesk = (from a in DbSRS.dataHelpDesk.Where(x => x.HelpDeskGUID == model.HelpDeskGUID) select a).FirstOrDefault();
            dataHelpDesk.RequestUpdateDate = ExecutionTime;
            dataHelpDesk.AssignedToGUID = model.AssignedToGUID;
            dataHelpDesk.RequestStatusGUID = model.RequestStatusGUID;
            DbSRS.UpdateNoAudit(dataHelpDesk);

            dataAppRequestAuthorization dataAppRequestAuthorization = new dataAppRequestAuthorization();
            dataAppEnhancementAuthorization dataAppEnhancementAuthorization = new dataAppEnhancementAuthorization();
            dataAppBugAuthorization dataAppBugAuthorization = new dataAppBugAuthorization();
            dataChangeRequest dataChangeRequest = new dataChangeRequest();

            if (model.ConfigItemGUID == newApplication)
            {
                if (model.HelpDeskNewApplicationAuthorizationModel.AppRequestGUID == Guid.Empty)
                {
                    dataAppRequestAuthorization = Mapper.Map(model.HelpDeskNewApplicationAuthorizationModel, new dataAppRequestAuthorization());
                    dataAppRequestAuthorization.AppRequestGUID = model.HelpDeskNewApplicationModel.AppRequestGUID;
                    dataAppRequestAuthorization.ApprovedByProfileGUID = UserProfileGUID;
                    DbSRS.CreateNoAudit(dataAppRequestAuthorization);
                }
                else
                {
                    dataAppRequestAuthorization = Mapper.Map(model.HelpDeskNewApplicationAuthorizationModel, new dataAppRequestAuthorization());
                    dataAppRequestAuthorization.ApprovedByProfileGUID = UserProfileGUID;
                    DbSRS.UpdateNoAudit(dataAppRequestAuthorization);
                }
                dataApplicationRequest dataApplicationRequest = dataHelpDesk.dataApplicationRequest.FirstOrDefault();
                dataApplicationRequest.ServerOrVM = model.HelpDeskNewApplicationModel.ServerOrVM;
                dataApplicationRequest.CPUDescription = model.HelpDeskNewApplicationModel.CPUDescription;
                dataApplicationRequest.RAMDescription = model.HelpDeskNewApplicationModel.RAMDescription;
                dataApplicationRequest.StorageDescription = model.HelpDeskNewApplicationModel.StorageDescription;
                dataApplicationRequest.OnsiteBackup = model.HelpDeskNewApplicationModel.OnsiteBackup;
                dataApplicationRequest.OffsiteBackup = model.HelpDeskNewApplicationModel.OffsiteBackup;
                DbSRS.UpdateNoAudit(dataApplicationRequest);
            }
            else if (model.ConfigItemGUID == applicationEnhancement)
            {
                if (model.HelpDeskApplicationEnhancementAuthorizationModel.AppEnhancementGUID == Guid.Empty)
                {
                    dataAppEnhancementAuthorization = Mapper.Map(model.HelpDeskApplicationEnhancementAuthorizationModel, new dataAppEnhancementAuthorization());
                    dataAppEnhancementAuthorization.AppEnhancementGUID = model.HelpDeskApplicationEnhancementModel.AppEnhancementGUID;
                    dataAppEnhancementAuthorization.ApprovedByProfileGUID = UserProfileGUID;
                    DbSRS.CreateNoAudit(dataAppEnhancementAuthorization);
                }
                else
                {
                    dataAppEnhancementAuthorization = Mapper.Map(model.HelpDeskApplicationEnhancementAuthorizationModel, new dataAppEnhancementAuthorization());
                    dataAppEnhancementAuthorization.ApprovedByProfileGUID = UserProfileGUID;
                    DbSRS.UpdateNoAudit(dataAppRequestAuthorization);
                }
            }
            else if (model.ConfigItemGUID == bugReport)
            {
                if (model.HelpDeskBugReportAuthorizationModel.AppBugReportGUID == Guid.Empty)
                {
                    dataAppBugAuthorization = Mapper.Map(model.HelpDeskBugReportAuthorizationModel, new dataAppBugAuthorization());
                    dataAppBugAuthorization.AppBugReportGUID = model.HelpDeskBugReportModel.AppBugReportGUID;
                    dataAppBugAuthorization.ApprovedByProfileGUID = UserProfileGUID;
                    DbSRS.CreateNoAudit(dataAppBugAuthorization);
                }
                else
                {
                    dataAppBugAuthorization = Mapper.Map(model.HelpDeskBugReportAuthorizationModel, new dataAppBugAuthorization());
                    dataAppBugAuthorization.ApprovedByProfileGUID = UserProfileGUID;
                    DbSRS.UpdateNoAudit(dataAppBugAuthorization);
                }
            }
            else if (model.ConfigItemGUID == changeRequest)
            {
                dataChangeRequest = Mapper.Map(model.helpDeskChangeRequestModel, new dataChangeRequest());
                DbSRS.UpdateNoAudit(dataChangeRequest);
                foreach (var item in model.helpDeskChangeRequestModel.CRActionPlanList)
                {
                    if (item.ChangeRequestAPGUID == Guid.Empty)
                    {

                    }
                    else
                    {

                    }
                }
            }
            try
            {
                DbSRS.SaveChanges();
                UserNameAndEmailModel UserNameAndEmailModel = (from a in DbSRS.userProfiles.Where(x => x.Active && x.UserProfileGUID == model.AssignedToGUID)
                                                               join b in DbSRS.userServiceHistory.Where(x => x.Active) on a.ServiceHistoryGUID equals b.ServiceHistoryGUID
                                                               join c in DbSRS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == "EN") on b.UserGUID equals c.UserGUID
                                                               orderby a.FromDate descending
                                                               select new UserNameAndEmailModel
                                                               {
                                                                   UserFullName = c.FirstName + " " + c.Surname,
                                                                   EmailAddress = b.EmailAddress
                                                               }).FirstOrDefault();

                UserNameAndEmailModel RecipientUserNameAndEmailModel = (from a in DbSRS.userProfiles.Where(x => x.Active && x.UserProfileGUID == model.RequestedByProfileGUID)
                                                                        join b in DbSRS.userServiceHistory.Where(x => x.Active) on a.ServiceHistoryGUID equals b.ServiceHistoryGUID
                                                                        join c in DbSRS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == "EN") on b.UserGUID equals c.UserGUID
                                                                        orderby a.FromDate descending
                                                                        select new UserNameAndEmailModel
                                                                        {
                                                                            UserFullName = c.FirstName + " " + c.Surname,
                                                                            EmailAddress = b.EmailAddress
                                                                        }).FirstOrDefault();
                if (model.ConfigItemGUID == newApplication)
                {
                    if (model.RequestStatusGUID != RequestStatusGUID_Completed)
                    {


                        new Email().SendHelpDeskAssignRequest(
                            RecipientEmail: UserNameAndEmailModel.EmailAddress,
                            RecipientName: UserNameAndEmailModel.UserFullName,
                            CCList: ToEmail,
                            Type: "Application Request",
                            Action: "HelpDesk",
                            subject: "Ticket Number " + dataHelpDesk.RequestNumber.ToString() + " || Application Request - " + dataHelpDesk.dataApplicationRequest.FirstOrDefault().RequestedAppName.ToString() + " - Created by: " + UserNameAndEmailModel.UserFullName,
                            PK: dataHelpDesk.HelpDeskGUID);
                    }

                    if (model.RequestStatusGUID == RequestStatusGUID_Completed)
                    {
                        new Email().SendHelpDeskResolution(
                            RecipientEmail: RecipientUserNameAndEmailModel.EmailAddress,
                            RecipientName: RecipientUserNameAndEmailModel.UserFullName,
                            CCList: ToEmail,
                            RequestNumber: "New Application/Service Request Resolution",
                            Action: "HelpDesk",
                            Subject: "Ticket Number " + dataHelpDesk.RequestNumber.ToString() + " || Application Request Resolution- " + dataHelpDesk.dataApplicationRequest.FirstOrDefault().RequestedAppName.ToString(),
                            PK: dataHelpDesk.HelpDeskGUID
                            );
                    }



                    return Json(DbSRS.SingleUpdateMessage(null, null, DbSRS.RowVersionControls(dataHelpDesk, dataAppRequestAuthorization)));
                }
                else if (model.ConfigItemGUID == applicationEnhancement)
                {

                    Guid appGUID = dataHelpDesk.dataAppEnhancementRequest.FirstOrDefault().ApplicationGUID;
                    string appName = (from a in DbSRS.codeApplicationsLanguages.Where(x => x.Active && x.LanguageID == LAN && x.ApplicationGUID == appGUID)
                                      select "[" + a.codeApplications.ApplicationAcrynom + "]" + " " + a.ApplicationDescription).FirstOrDefault();
                    if (model.RequestStatusGUID != RequestStatusGUID_Completed)
                    {
                        new Email().SendHelpDeskAssignRequest(
                           RecipientEmail: UserNameAndEmailModel.EmailAddress,
                          RecipientName: UserNameAndEmailModel.UserFullName,
                          CCList: ToEmail,
                           Type: "New Enhancement Request",
                           Action: "HelpDesk",
                            subject: "Ticket Number " + dataHelpDesk.RequestNumber.ToString() + " || Enhancement Request - " + appName,
                          PK: dataHelpDesk.HelpDeskGUID);
                    }

                    if (model.RequestStatusGUID == RequestStatusGUID_Completed)
                    {
                        new Email().SendHelpDeskResolution(
                            RecipientEmail: RecipientUserNameAndEmailModel.EmailAddress,
                            RecipientName: RecipientUserNameAndEmailModel.UserFullName,
                            CCList: ToEmail,
                            RequestNumber: "Enhancement Request Resolution",
                            Action: "HelpDesk",
                            Subject: "Ticket Number " + dataHelpDesk.RequestNumber.ToString() + " || Enhancement Request Resolution- " + appName,
                            PK: dataHelpDesk.HelpDeskGUID
                            );
                    }

                    return Json(DbSRS.SingleUpdateMessage(null, null, DbSRS.RowVersionControls(dataHelpDesk, dataAppEnhancementAuthorization)));
                }
                else if (model.ConfigItemGUID == bugReport)
                {
                    Guid appGUID = dataHelpDesk.dataAppBugReport.FirstOrDefault().ApplicationGUID;
                    string appName = (from a in DbSRS.codeApplicationsLanguages.Where(x => x.Active && x.LanguageID == LAN && x.ApplicationGUID == appGUID)
                                      select "[" + a.codeApplications.ApplicationAcrynom + "]" + " " + a.ApplicationDescription).FirstOrDefault();
                    if (model.RequestStatusGUID != RequestStatusGUID_Completed)
                    {
                        new Email().SendHelpDeskAssignRequest(
                           RecipientEmail: UserNameAndEmailModel.EmailAddress,
                            RecipientName: UserNameAndEmailModel.UserFullName,
                           CCList: ToEmail,
                           Type: "New Bug Report",
                          Action: "HelpDesk",
                          subject: "Ticket Number " + dataHelpDesk.RequestNumber.ToString() + " || Bug Report - " + appName,
                          PK: dataHelpDesk.HelpDeskGUID);
                    }
                    if (model.RequestStatusGUID == RequestStatusGUID_Completed)
                    {
                        new Email().SendHelpDeskResolution(
                            RecipientEmail: RecipientUserNameAndEmailModel.EmailAddress,
                            RecipientName: RecipientUserNameAndEmailModel.UserFullName,
                            CCList: ToEmail,
                            RequestNumber: dataHelpDesk.RequestNumber.ToString(),
                            Action: "HelpDesk",
                            Subject: "Ticket Number " + dataHelpDesk.RequestNumber.ToString() + " || Bug Report Resolution - " + appName,
                            PK: dataHelpDesk.HelpDeskGUID
                            );
                    }

                    return Json(DbSRS.SingleUpdateMessage(null, null, DbSRS.RowVersionControls(dataHelpDesk, dataAppBugAuthorization)));
                }
                return null;
            }
            catch (Exception ex)
            {
                return Json(DbSRS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult HelpDeskDelete(dataHelpDesk model)
        {
            if (!CMS.HasAction(Permissions.Newservicerequest.Delete, Apps.SRS))
            {
                return Json(DbSRS.ErrorMessage("Unauthorized access"));
            }
            if (model.RequestedByProfileGUID != UserProfileGUID)
            {
                return Json(DbSRS.ErrorMessage("Unauthorized access,this record was not created by you"));
            }
            List<dataHelpDesk> DeletedHelDeskRequests = DeleteHelpDekRequest(Portal.SingleToList(model));


            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.Newservicerequest.Restore, Apps.SRS), Container = "HelpDeskFormControls" });

            try
            {
                int CommitedRows = DbSRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSRS.SingleDeleteMessage(CommitedRows, DeletedHelDeskRequests.FirstOrDefault(), "HelpDeskAttachementContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
                //return ConcrrencyApplication(model.ApplicationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSRS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult HelpDeskAdminDelete(dataHelpDesk model)
        {
            if (!CMS.HasAction(Permissions.Newservicerequest.Delete, Apps.SRS))
            {
                return Json(DbSRS.ErrorMessage("Unauthorized access"));
            }
            if (model.RequestedByProfileGUID != UserProfileGUID)
            {
                return Json(DbSRS.ErrorMessage("Unauthorized access,this record was not created by you"));
            }
            List<dataHelpDesk> DeletedHelDeskRequests = DeleteHelpDekRequest(Portal.SingleToList(model));


            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.Newservicerequest.Restore, Apps.SRS), Container = "HelpDeskFormControls" });

            try
            {
                int CommitedRows = DbSRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSRS.SingleDeleteMessage(CommitedRows, DeletedHelDeskRequests.FirstOrDefault(), "HelpDeskAttachementContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
                //return ConcrrencyApplication(model.ApplicationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSRS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult HelpDeskDataTableDelete(List<dataHelpDesk> models)
        {
            if (!CMS.HasAction(Permissions.Newservicerequest.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataHelpDesk> DeletedHelDeskRequests = DeleteHelpDekRequest(models);

            try
            {
                DbSRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSRS.PartialDeleteMessage(DeletedHelDeskRequests, models, DataTableNames.HelpDeskDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbSRS.ErrorMessage(ex.Message));
            }
        }

        private List<dataHelpDesk> DeleteHelpDekRequest(List<dataHelpDesk> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataHelpDesk> DeletedHelpDeskRequests = new List<dataHelpDesk>();

            //THIS IS FOR PERMISSION TEST//
            //DO NOT DELETE THIS CODE
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = (from a in DbCMS.codeApplications
            //                    from f in DbCMS.codeApplicationsFactorForTest
            //                    where a.ApplicationGUID == f.ApplicationGUID
            //                    select new { a.ApplicationGUID, a.codeApplicationsRowVersion, f.OperationGUID, f.OrganizationGUID, f.CountryGUID }).AsQueryable().ToString();

            string query = DbSRS.QueryBuilder(models, Permissions.Newservicerequest.DeleteGuid, SubmitTypes.Delete, "");
            ////////////////
            var Records = DbSRS.Database.SqlQuery<dataHelpDesk>(query).ToList();
            foreach (var record in Records)
            {
                DeletedHelpDeskRequests.Add(DbSRS.Delete(record, ExecutionTime, Permissions.Newservicerequest.DeleteGuid, DbCMS));
            }
            ////////////////
            var applicationRequests = DeletedHelpDeskRequests.SelectMany(a => a.dataApplicationRequest).Where(l => l.Active).ToList();
            foreach (var applicationRequest in applicationRequests)
            {
                DbSRS.Delete(applicationRequest, ExecutionTime, Permissions.Newservicerequest.DeleteGuid, DbCMS);

            }
            var applicationRequestsAuth = applicationRequests.SelectMany(a => a.dataAppRequestAuthorization).Where(l => l.Active).ToList();
            foreach (var appRequestsAuth in applicationRequestsAuth)
            {
                DbSRS.Delete(appRequestsAuth, ExecutionTime, Permissions.Newservicerequest.DeleteGuid, DbCMS);
            }
            ////////////////
            var enhancementRequests = DeletedHelpDeskRequests.SelectMany(a => a.dataAppEnhancementRequest).Where(l => l.Active).ToList();
            foreach (var enhancementRequest in enhancementRequests)
            {
                DbSRS.Delete(enhancementRequest, ExecutionTime, Permissions.Newservicerequest.DeleteGuid, DbCMS);
            }
            var enhancementRequestsAuth = enhancementRequests.SelectMany(a => a.dataAppEnhancementAuthorization).Where(l => l.Active).ToList();
            foreach (var enhRequestsAuth in enhancementRequestsAuth)
            {
                DbSRS.Delete(enhRequestsAuth, ExecutionTime, Permissions.Newservicerequest.DeleteGuid, DbCMS);
            }
            ////////////////
            var bugReports = DeletedHelpDeskRequests.SelectMany(a => a.dataAppBugReport).Where(l => l.Active).ToList();
            foreach (var bugReport in bugReports)
            {
                DbSRS.Delete(bugReports, ExecutionTime, Permissions.Newservicerequest.DeleteGuid, DbCMS);
            }
            var bugReportsAuth = bugReports.SelectMany(a => a.dataAppBugAuthorization).Where(l => l.Active).ToList();
            foreach (var bugRepsAuth in bugReportsAuth)
            {
                DbSRS.Delete(bugRepsAuth, ExecutionTime, Permissions.Newservicerequest.DeleteGuid, DbCMS);
            }
            ////////////////
            var dataHelpDeskAttachements = DeletedHelpDeskRequests.SelectMany(a => a.dataHelpDeskAttachement).Where(l => l.Active).ToList();
            foreach (var dataHelpDeskAttachement in dataHelpDeskAttachements)
            {
                DbSRS.Delete(dataHelpDeskAttachement, ExecutionTime, Permissions.Newservicerequest.DeleteGuid, DbCMS);
            }
            ////////////////


            return DeletedHelpDeskRequests;
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult HelpDeskRestore(dataHelpDesk model)
        {
            if (!CMS.HasAction(Permissions.Newservicerequest.Restore, Apps.SRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataHelpDesk> RestoredHelpDeskRequests = RestoreHelpDeskRequests(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButtonNoPermission(new UrlHelper(Request.RequestContext).Action("HelpDesk/Create", "HelpDesk", new { Area = "SRS" })), Container = "HelpDeskFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButtonNoPermission(), Container = "HelpDeskFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButtonNoPermission(), Container = "HelpDeskFormControls" });

            try
            {
                int CommitedRows = DbSRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSRS.SingleRestoreMessage(CommitedRows, RestoredHelpDeskRequests, DbSRS.PrimaryKeyControl(RestoredHelpDeskRequests.FirstOrDefault()), Url.Action(DataTableNames.ApplicationLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
            }
            catch (Exception ex)
            {
                return Json(DbSRS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult HelpDeskAdminRestore(dataHelpDesk model)
        {
            if (!CMS.HasAction(Permissions.Newservicerequest.Restore, Apps.SRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataHelpDesk> RestoredHelpDeskRequests = RestoreHelpDeskRequests(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButtonNoPermission(new UrlHelper(Request.RequestContext).Action("HelpDesk/Create", "HelpDesk", new { Area = "SRS" })), Container = "HelpDeskFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButtonNoPermission(), Container = "HelpDeskFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButtonNoPermission(), Container = "HelpDeskFormControls" });

            try
            {
                int CommitedRows = DbSRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSRS.SingleRestoreMessage(CommitedRows, RestoredHelpDeskRequests, DbSRS.PrimaryKeyControl(RestoredHelpDeskRequests.FirstOrDefault()), Url.Action(DataTableNames.ApplicationLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
            }
            catch (Exception ex)
            {
                return Json(DbSRS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult HelpDeskDataTableRestore(List<dataHelpDesk> models)
        {
            if (!CMS.HasAction(Permissions.Applications.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataHelpDesk> RestoredHelpDeskRequests = RestoreHelpDeskRequests(models);

            try
            {
                DbSRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSRS.PartialRestoreMessage(RestoredHelpDeskRequests, models, DataTableNames.HelpDeskAttachementDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbSRS.ErrorMessage(ex.Message));
            }
        }

        private List<dataHelpDesk> RestoreHelpDeskRequests(List<dataHelpDesk> models)
        {
            Guid DeleteActionGUID = Permissions.Newservicerequest.DeleteGuid;
            Guid RestoreActionGUID = Permissions.Newservicerequest.RestoreGuid;
            DateTime RestoringTime = DateTime.Now;

            List<dataHelpDesk> RestoreHelpDeskRequests = new List<dataHelpDesk>();

            //THIS IS FOR PERMISSION TEST//
            //DO NOT DELETE THIS CODE
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = (from a in DbCMS.codeApplications
            //                    from f in DbCMS.codeApplicationsFactorForTest
            //                    where a.ApplicationGUID == f.ApplicationGUID
            //                    select new
            //                    {
            //                        a.ApplicationGUID,
            //                        a.codeApplicationsRowVersion,
            //                        C2 = f.OperationGUID + "," + f.OrganizationGUID,
            //                    }).AsQueryable().ToString();//.Replace("[C2]", "[FactorsToken]");

            string query = DbSRS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, "");

            ////////////////
            var Records = DbSRS.Database.SqlQuery<dataHelpDesk>(query).ToList();
            foreach (var record in Records)
            {
                RestoreHelpDeskRequests.Add(DbSRS.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS));
            }
            ////////////////
            var dataApplicationRequests = RestoreHelpDeskRequests.SelectMany(x => x.dataApplicationRequest.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var dataApplicationRequest in dataApplicationRequests)
            {
                DbSRS.Restore(dataApplicationRequest, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS);
            }
            var dataAppRequestAuthorizations = dataApplicationRequests.SelectMany(x => x.dataAppRequestAuthorization.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var dataAppRequestAuthorization in dataAppRequestAuthorizations)
            {
                DbSRS.Restore(dataAppRequestAuthorization, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS);
            }
            ////////////////
            var dataAppEnhancementRequests = RestoreHelpDeskRequests.SelectMany(x => x.dataAppEnhancementRequest.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var dataAppEnhancementRequest in dataAppEnhancementRequests)
            {
                DbSRS.Restore(dataAppEnhancementRequest, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS);
            }
            var dataAppEnhancementAuthorizations = dataAppEnhancementRequests.SelectMany(x => x.dataAppEnhancementAuthorization.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var dataAppEnhancementAuthorization in dataAppEnhancementAuthorizations)
            {
                DbSRS.Restore(dataAppEnhancementAuthorization, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS);
            }
            ////////////////
            var dataAppBugReports = RestoreHelpDeskRequests.SelectMany(x => x.dataAppBugReport.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var dataAppBugReport in dataAppBugReports)
            {
                DbSRS.Restore(dataAppBugReport, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS);
            }
            var dataAppBugAuthorizations = dataAppBugReports.SelectMany(x => x.dataAppBugAuthorization.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var dataAppBugAuthorization in dataAppBugAuthorizations)
            {
                DbSRS.Restore(dataAppBugAuthorization, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS);
            }
            ////////////////
            var dataHelpDeskAttachements = RestoreHelpDeskRequests.SelectMany(x => x.dataHelpDeskAttachement.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var dataHelpDeskAttachement in dataHelpDeskAttachements)
            {
                DbSRS.Restore(dataHelpDeskAttachement, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS);
            }
            ////////////////
            return RestoreHelpDeskRequests;
        }






        [Route("SRS/HelpDesk/HelpDeskLoadPartial")]
        public ActionResult HelpDeskLoadPartial(Guid ConfigItemGUID)
        {
            if (ConfigItemGUID == newApplication)
            {
                return PartialView("~/Areas/SRS/Views/HelpDesk/User/_NewApplicationForm.cshtml");
            }
            else if (ConfigItemGUID == applicationEnhancement)
            {
                return PartialView("~/Areas/SRS/Views/HelpDesk/User/_ApplicationEnhancementForm.cshtml");
            }
            else if (ConfigItemGUID == bugReport)
            {
                return PartialView("~/Areas/SRS/Views/HelpDesk/User/_BugReportForm.cshtml");
            }
            else if (ConfigItemGUID == changeRequest)
            {
                return PartialView("~/Areas/SRS/Views/HelpDesk/User/_ChangeRequestForm.cshtml");
            }
            return null;
        }


        #region CR Action Plan
        [Route("SRS/HelpDesk/HelpDeskCRActionPlan")]
        public ActionResult HelpDeskCRActionPlan()
        {
            return PartialView("~/Areas/SRS/Views/HelpDesk/User/_CRAPModal.cshtml");
        }

        #endregion





        #region Attachements
        public ActionResult HelpDeskAttachementDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/SRS/Views/HelpDesk/_HelpDeskAttachementDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<dataHelpDeskAttachement, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<dataHelpDeskAttachement>(DataTable.Filters);
            }

            var Result = (from a in DbSRS.dataHelpDeskAttachement.AsNoTracking().AsExpandable().Where(x => x.Active && x.HelpDeskGUID == PK).Where(Predicate)
                          select new
                          {
                              a.HelpDeskAttachementGUID,
                              a.HelpDeskGUID,
                              a.FileName,
                              a.Active,
                              a.dataHelpDeskAttachementRowVersion,
                          });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result.ToList()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult HelpDeskAttachementCreate(Guid FK)
        {
            return PartialView("~/Areas/SRS/Views/HelpDesk/_HelpDeskAttachementUpdateModal.cshtml",
                new dataHelpDeskAttachement { HelpDeskGUID = FK });
        }

        [HttpPost]
        public FineUploaderResult HelpDeskAttachementCreate(FineUpload upload, Guid HelpDeskGUID)
        {
            if (!FileTypeValidator.IsExcel(upload.InputStream) || !FileTypeValidator.IsWord(upload.InputStream) || !FileTypeValidator.IsPDF(upload.InputStream) || !FileTypeValidator.IsImage(upload.InputStream))
            {
                return new FineUploaderResult(false, new { error = "error: allowed files (excel, word, pdf, images)" });
            }
            var _stearm = upload.InputStream;
           
            string FolderPath = Server.MapPath("~/Uploads/SRS/" + HelpDeskGUID.ToString());
            Directory.CreateDirectory(FolderPath);
            string fileName = "";
            try
            {
                var asfad = upload.FileName.Split('\\');
                fileName = upload.FileName.Split('\\').Last();

                //fileName = 
            }
            catch { fileName = upload.FileName; }
            string FilePath = FolderPath + "/" + fileName;
            
            using (var fileStream = System.IO.File.Create(FilePath))
            {
                upload.InputStream.Seek(0, SeekOrigin.Begin);
                upload.InputStream.CopyTo(fileStream);
            }
            dataHelpDeskAttachement dataHelpDeskAttachement = new dataHelpDeskAttachement();
            dataHelpDeskAttachement.HelpDeskGUID = HelpDeskGUID;
            dataHelpDeskAttachement.AttachementPath = FilePath;
            dataHelpDeskAttachement.FileName = fileName;

            try
            {
                DbSRS.CreateNoAudit(dataHelpDeskAttachement);
                DbSRS.SaveChanges();
                return new FineUploaderResult(true, new { path = FilePath, success = true });
            }
            catch (Exception ex)
            {
                return new FineUploaderResult(false, new { error = "error: " + ex.Message });
            }
        }

        [Route("SRS/HelpDesk/HelpDeskAttachementDownload/{PK}")]
        public FileResult HelpDeskAttachementDownload(Guid PK)
        {
            Guid AppAttachementGUID = PK;

            dataHelpDeskAttachement dataHelpDeskAttachement = (from a in DbSRS.dataHelpDeskAttachement.Where(x => x.HelpDeskAttachementGUID == PK) select a).FirstOrDefault();
            string FolderPath = Server.MapPath("~/Uploads/SRS/" + dataHelpDeskAttachement.HelpDeskGUID.ToString());

            string FileName = dataHelpDeskAttachement.FileName;
            string DownloadURL = FolderPath + "\\" + FileName;

            byte[] fileBytes = System.IO.File.ReadAllBytes(@"" + DownloadURL + "");

            try
            {
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult HelpDeskAttachementDataTableDelete(List<dataHelpDeskAttachement> models)
        {
            if (!CMS.HasAction(Permissions.Applications.Delete, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataHelpDeskAttachement> DeletedHelpDeskAttachement = DeleteHelpDeskAttachement(models);

            try
            {
                DbSRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSRS.PartialDeleteMessage(DeletedHelpDeskAttachement, models, DataTableNames.HelpDeskAttachementDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbSRS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult HelpDeskAttachementDataTableRestore(List<dataHelpDeskAttachement> models)
        {

            if (!CMS.HasAction(Permissions.Applications.Restore, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataHelpDeskAttachement> RestoredHelpDeskAttachement = RestoreHelpDeskAttachement(models);

            try
            {
                DbSRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSRS.PartialRestoreMessage(RestoredHelpDeskAttachement, models, DataTableNames.HelpDeskAttachementDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbSRS.ErrorMessage(ex.Message));
            }
        }

        private List<dataHelpDeskAttachement> DeleteHelpDeskAttachement(List<dataHelpDeskAttachement> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataHelpDeskAttachement> DeletedHelpDeskAttachements = new List<dataHelpDeskAttachement>();

            string query = DbSRS.QueryBuilder(models, Permissions.Newservicerequest.DeleteGuid, SubmitTypes.Delete, "");

            var dataHelpDeskAttachements = DbSRS.Database.SqlQuery<dataHelpDeskAttachement>(query).ToList();

            foreach (var dataHelpDeskAttachement in dataHelpDeskAttachements)
            {
                DeletedHelpDeskAttachements.Add(DbSRS.Delete(dataHelpDeskAttachement, ExecutionTime, Permissions.ApplicationsLanguages.DeleteGuid));
            }

            return DeletedHelpDeskAttachements;
        }

        private List<dataHelpDeskAttachement> RestoreHelpDeskAttachement(List<dataHelpDeskAttachement> models)
        {
            Guid DeleteActionGUID = Permissions.ApplicationsLanguages.DeleteGuid;
            Guid RestoreActionGUID = Permissions.ApplicationsLanguages.RestoreGuid;
            DateTime RestoringTime = DateTime.Now;

            List<dataHelpDeskAttachement> RestoredHelpDeskAttachements = new List<dataHelpDeskAttachement>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbSRS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var dataHelpDeskAttachements = DbSRS.Database.SqlQuery<dataHelpDeskAttachement>(query).ToList();
            foreach (var dataHelpDeskAttachement in dataHelpDeskAttachements)
            {
                RestoredHelpDeskAttachements.Add(DbSRS.Restore(dataHelpDeskAttachement, DeleteActionGUID, RestoreActionGUID, RestoringTime));
            }

            return RestoredHelpDeskAttachements;
        }

        #endregion

        #region Global Bug Report
        [Route("SRS/HelpDesk/GlobalBugReportCreate")]
        public ActionResult GlobalBugReportCreate()
        {
            int RequestNumber = 1;
            Guid _bugReport = Guid.Parse("81008ea8-9b63-4b76-bff3-24ee0b41f0a6");

            try { RequestNumber = (from a in DbSRS.dataHelpDesk.AsNoTracking() select a.RequestNumber).Max() + 1; } catch { }
            return PartialView("~/Areas/SRS/Views/GlobalBugReport/_GlobalBugReport.cshtml",
                new HelpDeskModel
                {
                    RequestNumber = RequestNumber,
                    WorkGroupGUID = Guid.Parse("bf57651c-efac-439c-a3e6-1623b031c891"),
                    RequestStatusGUID = RequestStatusGUID_Pending,
                    ConfigItemGUID = bugReport
                });
        }
        #endregion
    }
}