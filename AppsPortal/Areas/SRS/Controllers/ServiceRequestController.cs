using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using FineUploader;
using LinqKit;
using SRS_DAL.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.SRS.Controllers
{
    public class ServiceRequestController : SRSBaseController
    {
        #region
        public static class AuthorizedEmailAccounts
        {
            public const string rampersa = "rampersa@unhcr.org";
            public const string isac = "isac@unhcr.org";
            public const string karkoush = "karkoush@unhcr.org";
        }
        #endregion

        #region New Application Request

        #region Main Details
        [Route("SRS/ApplicationRequests")]
        public ActionResult ApplicationRequestIndex()
        {
            return View("~/Areas/SRS/Views/ApplicationRequest/Index.cshtml");
        }

        [Route("SRS/ServiceRequest/ApplicationRequestsDataTable")]
        public ActionResult ApplicationRequestsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ApplicationRequestDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ApplicationRequestDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbSRS.dataApplicationRequest.AsExpandable().Where(x => x.Active)
                       join b in DbSRS.dataAppRequestAuthorization.AsExpandable().Where(x => x.Active) on a.AppRequestGUID equals b.AppRequestGUID into LJ1
                       join c in DbSRS.userProfiles.AsExpandable().Where(x => x.Active) on a.RequestedByProfileGUID equals c.UserProfileGUID
                       join d in DbSRS.userServiceHistory.AsExpandable().Where(x => x.Active) on c.ServiceHistoryGUID equals d.ServiceHistoryGUID
                       join e in DbSRS.userPersonalDetailsLanguage.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on d.UserGUID equals e.UserGUID
                       from R1 in LJ1.DefaultIfEmpty()
                       //join f in DbSRS.userProfiles.AsExpandable().Where(x => x.Active) on R1.ApprovedByProfileGUID equals f.UserProfileGUID into LJ2
                       //from R2 in LJ2.DefaultIfEmpty()
                       //join j in DbSRS.userServiceHistory.AsExpandable().Where(x => x.Active) on R2.ServiceHistoryGUID equals j.ServiceHistoryGUID
                       //join h in DbSRS.userPersonalDetailsLanguage.AsExpandable().Where(x=>x.Active && x.LanguageID == LAN) on j.UserGUID equals h.UserGUID
                       select new ApplicationRequestDataTableModel
                       {
                           AppRequestGUID = a.AppRequestGUID,
                           RequestedAppName = a.RequestedAppName,
                           AppRequestDate = a.AppRequestDate,
                           RequestedByProfileGUID = a.RequestedByProfileGUID,
                           RequestedBy = e.FirstName + " " + e.Surname,
                           dataApplicationRequestRowVersion = a.dataApplicationRequestRowVersion,
                           Active = a.Active,
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ApplicationRequestDataTableModel> Result = Mapper.Map<List<ApplicationRequestDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("SRS/ServiceRequest/Create/")]
        public ActionResult ApplicationRequestCreate()
        {
            byte[] xx = (from a in DbSRS.codeApplications select a.codeApplicationsRowVersion).FirstOrDefault();
            string asd = Encoding.ASCII.GetString(xx);
            string asdasdasd = "\0\0\0\0\0\u0010??";
            byte[] dsa = Encoding.ASCII.GetBytes(asdasdasd);
            return View("~/Areas/SRS/Views/ApplicationRequest/ApplicationRequest.cshtml", new ApplicationRequestModel());
        }

        [Route("SRS/ServiceRequest/Update/{PK}")]
        public ActionResult ApplicationRequestUpdate(Guid PK)
        {
            var model = (from a in DbSRS.dataApplicationRequest.Where(x => x.Active && x.AppRequestGUID == PK)
                         join b in DbSRS.dataAppRequestAuthorization.Where(x=>x.Active) on a.AppRequestGUID equals b.AppRequestGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new ApplicationRequestModel
                         {
                             AppRequestGUID = a.AppRequestGUID,
                             RequestedAppName = a.RequestedAppName,
                             IsNewDbForTracking = a.IsNewDbForTracking,
                             IsToAutomateManual = a.IsToAutomateManual,
                             IsToReplaceOldApp = a.IsToReplaceOldApp,
                             OtherPurposForApp = a.OtherPurposForApp,
                             AppRequestDate = a.AppRequestDate,
                             RequestedByProfileGUID = a.RequestedByProfileGUID,
                             AppBenefits = a.AppBenefits,
                             AppDescription = a.AppDescription,
                             Active = a.Active,
                             dataApplicationRequestRowVersion = a.dataApplicationRequestRowVersion,

                             AppRequestAuthorizationGUID = R1.AppRequestAuthorizationGUID,
                             ApprovedByProfileGUID = R1.ApprovedByProfileGUID,
                             ApprovalStatusGUID = R1.ApprovalStatusGUID,
                             ApprovalComments = R1.ApprovalComments,
                             EstimatedResourcesNum = R1.EstimatedResourcesNum,
                             EstimatedTime = R1.EstimatedTime,
                             CriticalityGUID = R1.CriticalityGUID,
                             ApplicationStartDate = R1.ApplicationStartDate,
                             ApplicationEndDate = R1.ApplicationEndDate,
                             AssignedToGUID = R1.AssignedToGUID,
                             dataAppRequestAuthorizationRowVersion = R1.dataAppRequestAuthorizationRowVersion,

                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("ApplicationRequests", "ServiceRequest", new { Area = "SRS" }));

            return View("~/Areas/SRS/Views/ApplicationRequest/ApplicationRequest.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ApplicationRequestCreate(ApplicationRequestModel model)
        {
            if (!ModelState.IsValid) return PartialView("~/Areas/SRS/Views/ApplicationRequest/_ApplicationRequestForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            dataApplicationRequest dataApplicationRequest = Mapper.Map(model, new dataApplicationRequest());
            dataApplicationRequest.AppRequestGUID = EntityPK;
            dataApplicationRequest.AppRequestDate = DateTime.Now;
            dataApplicationRequest.RequestedByProfileGUID = UserProfileGUID;
            DbSRS.CreateNoAudit(dataApplicationRequest);


            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ApplicationRequestAttachementDataTable, ControllerContext, "ApplicationRequestAttachementContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButtonNoPermission(new UrlHelper(Request.RequestContext).Action("Create", "ServiceRequest", new { Area = "SRS" })), Container = "ApplicationRequestFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButtonNoPermission(), Container = "ApplicationRequestFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButtonNoPermission(), Container = "ApplicationRequestFormControls" });

            try
            {
                DbSRS.SaveChanges();
                new Email().SendHelpDeskRequest("DLSyria-AppsSupport@unhcr.org", "", "New Application Request", "ApplicationRequest", "Application Request", model.AppRequestGUID);
                return Json(DbSRS.SingleCreateMessage(DbSRS.PrimaryKeyControl(dataApplicationRequest), DbSRS.RowVersionControls(Portal.SingleToList(dataApplicationRequest)), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbSRS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult ApplicationRequestUpdate(ApplicationRequestModel model)
        {

            DateTime ExecutionTime = DateTime.Now;

            dataApplicationRequest dataApplicationRequest = Mapper.Map(model, new dataApplicationRequest());
            dataAppRequestAuthorization dataAppRequestAuthorization = Mapper.Map(model, new dataAppRequestAuthorization());

            if (CMS.HasAction(Permissions.Newservicerequest.Update, Apps.SRS))
            {
                
                dataAppRequestAuthorization.ApprovedByProfileGUID = UserProfileGUID;
                DbSRS.UpdateNoAudit(dataApplicationRequest);
                if (model.AppRequestAuthorizationGUID.HasValue)
                {
                    DbSRS.Update(dataAppRequestAuthorization, Permissions.Newservicerequest.UpdateGuid, ExecutionTime, DbCMS);

                }
                else
                {
                    DbSRS.Create(dataAppRequestAuthorization, Permissions.Newservicerequest.UpdateGuid, ExecutionTime, DbCMS);
                }
            }
            else if (model.RequestedByProfileGUID == UserProfileGUID)
            {
                DbSRS.UpdateNoAudit(dataApplicationRequest);
            }
            else
            {
                return Json(DbSRS.ErrorMessage("Unauthorized access"));
            }
            try
            {
                DbSRS.SaveChanges();
                return Json(DbSRS.SingleUpdateMessage(null, null, DbSRS.RowVersionControls(dataApplicationRequest, dataAppRequestAuthorization)));
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
        public ActionResult ApplicationRequestDelete(dataApplicationRequest model)
        {
            if (model.RequestedByProfileGUID != UserProfileGUID)
            {
                return Json(DbSRS.ErrorMessage("Unauthorized access,this record was not created by you"));
            }

            List<dataApplicationRequest> DeletedApplicationsRequests = DeleteApplicationsRequests(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButtonNoPermission(), Container = "ApplicationRequestFormControls" });

            try
            {
                int CommitedRows = DbSRS.SaveChanges();
                return Json(DbSRS.SingleDeleteMessage(CommitedRows, DeletedApplicationsRequests.FirstOrDefault(), "ApplicationRequestAttachementContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Json(DbSRS.ErrorMessage(ex.Message));
                //return ConcrrencyApplication(model.ApplicationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSRS.ErrorMessage(ex.Message));
            }
        }

        private List<dataApplicationRequest> DeleteApplicationsRequests(List<dataApplicationRequest> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataApplicationRequest> DeletedApplicationsRequests = new List<dataApplicationRequest>();

            //THIS IS FOR PERMISSION TEST//
            //DO NOT DELETE THIS CODE
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = (from a in DbCMS.codeApplications
            //                    from f in DbCMS.codeApplicationsFactorForTest
            //                    where a.ApplicationGUID == f.ApplicationGUID
            //                    select new { a.ApplicationGUID, a.codeApplicationsRowVersion, f.OperationGUID, f.OrganizationGUID, f.CountryGUID }).AsQueryable().ToString();

            string query = DbSRS.QueryBuilder(models, Permissions.Newservicerequest.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbSRS.Database.SqlQuery<dataApplicationRequest>(query).ToList();
            foreach (var record in Records)
            {
                DeletedApplicationsRequests.Add(DbSRS.Delete(record, ExecutionTime, Permissions.Newservicerequest.DeleteGuid));
            }
            var Authorizations = DeletedApplicationsRequests.SelectMany(x => x.dataAppRequestAuthorization).Where(x => x.Active).ToList();
            foreach (var auth in Authorizations)
            {
                DbSRS.Delete(auth, ExecutionTime, Permissions.Newservicerequest.DeleteGuid);
            }
            var Attachements = DeletedApplicationsRequests.SelectMany(x => x.dataAppRequestAttachement).Where(x => x.Active).ToList();
            foreach (var attachement in Attachements)
            {
                DbSRS.Delete(attachement, ExecutionTime, Permissions.Newservicerequest.DeleteGuid);
            }

            return DeletedApplicationsRequests;
        }
        #endregion

        #region Attachements
        public ActionResult ApplicationRequestAttachementDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/SRS/Views/ApplicationRequest/_ApplicationRequestAttachementDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<dataAppRequestAttachement, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<dataAppRequestAttachement>(DataTable.Filters);
            }

            var Result = (from a in DbSRS.dataAppRequestAttachement.AsNoTracking().AsExpandable().Where(x => x.Active && x.AppRequestGUID == PK).Where(Predicate)
                          select new
                          {
                              a.AppAttachementGUID,
                              a.AppRequestGUID,
                              a.FileName,
                              a.Active,
                              a.dataAppRequestAttachementRowVersion,
                          });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result.ToList()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ApplicationRequestAttachementCreate(Guid FK)
        {
            return PartialView("~/Areas/SRS/Views/ApplicationRequest/_ApplicationRequestAttachementUpdateModal.cshtml",
                new dataAppRequestAttachement { AppRequestGUID = FK });
        }

        [HttpPost]
        public FineUploaderResult ApplicationRequestAttachementCreate(FineUpload upload, Guid AppRequestGUID)
        {
            var _stearm = upload.InputStream;
            string FolderPath = Server.MapPath("~/Uploads/SRS/" + AppRequestGUID.ToString());
            Directory.CreateDirectory(FolderPath);
            string FilePath = FolderPath + "/" + upload.FileName;
            using (var fileStream = System.IO.File.Create(FilePath))
            {
                upload.InputStream.Seek(0, SeekOrigin.Begin);
                upload.InputStream.CopyTo(fileStream);
            }
            dataAppRequestAttachement dataAppRequestAttachement = new dataAppRequestAttachement();
            dataAppRequestAttachement.AppRequestGUID = AppRequestGUID;
            dataAppRequestAttachement.AttachementPath = FilePath;
            dataAppRequestAttachement.FileName = upload.FileName;

            try
            {
                DbSRS.CreateNoAudit(dataAppRequestAttachement);
                DbSRS.SaveChanges();
                return new FineUploaderResult(true, new { path = FilePath, success = true });
            }
            catch (Exception ex)
            {
                return new FineUploaderResult(false, new { error = "error: " + ex.Message });
            }
        }

        [Route("SRS/ServiceRequest/ApplicationRequestAttachementDownload/{PK}")]
        public FileResult ApplicationRequestAttachementDownload(Guid PK)
        {
            Guid AppAttachementGUID = PK;

            dataAppRequestAttachement dataAppRequestAttachement = (from a in DbSRS.dataAppRequestAttachement.Where(x => x.AppAttachementGUID == AppAttachementGUID) select a).FirstOrDefault();
            string FolderPath = Server.MapPath("~/Uploads/SRS/" + dataAppRequestAttachement.AppRequestGUID.ToString());

            string FileName = dataAppRequestAttachement.FileName;
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
        #endregion

        #region Authorization

        #endregion

        #endregion



        //#region Application Enhancement Request
        //[Route("SRS/ApplicationEnhancements")]
        //public ActionResult ApplicationEnhancementIndex()
        //{
        //    return View("~/Areas/SRS/Views/ApplicationEnhancements/Index.cshtml");
        //}

        //[Route("SRS/ApplicationEnhancement/ApplicationEnhancementsDataTable")]
        //public ActionResult ApplicationEnhancementsDataTable(DataTableRecievedOptions options)
        //{
        //    DataTableOptions DataTable = ConvertOptions.Fill(options);

        //    Expression<Func<ApplicationEnhancementsDataTableModel, bool>> Predicate = p => true;

        //    if (DataTable.Filters.FilterRules != null)
        //    {
        //        Predicate = SearchHelper.CreateSearchPredicate<ApplicationEnhancementsDataTableModel>(DataTable.Filters);
        //    }

        //    var All = (from a in DbSRS.dataAppEnhancementRequest.AsExpandable().Where(x => x.Active)
        //               join b in DbSRS.codeApplicationsLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.ApplicationGUID equals b.ApplicationGUID
        //               select new ApplicationEnhancementsDataTableModel
        //               {
        //                   AppEnhancementGUID = a.AppEnhancementGUID,
        //                   ApplicationDescription = b.ApplicationDescription,
        //                   EnhancementRequestedByProfileGUID = a.EnhancementRequestedByProfileGUID,
        //                   dataAppEnhancementRequestRowVersion = a.dataAppEnhancementRequestRowVersion,
        //                   Active = a.Active,
        //               }).Where(Predicate);

        //    All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

        //    List<ApplicationEnhancementsDataTableModel> Result = Mapper.Map<List<ApplicationEnhancementsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

        //    return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        //}

        //[Route("SRS/ApplicationEnhancement/Create/")]
        //public ActionResult ApplicationEnhancementCreate()
        //{
        //    return View("~/Areas/SRS/Views/ApplicationEnhancements/ApplicationEnhancement.cshtml", new ApplicationEnhancementRequestModel());
        //}

        //[Route("SRS/ApplicationEnhancement/Update/{PK}")]
        //public ActionResult ApplicationEnhancementUpdate(Guid PK)
        //{
        //    var model = (from a in DbSRS.dataAppEnhancementRequest.Where(x => x.Active && x.AppEnhancementGUID == PK)
        //                 select new ApplicationEnhancementRequestModel
        //                 {
        //                     AppEnhancementGUID = a.AppEnhancementGUID,
        //                     ApplicationGUID = a.ApplicationGUID,
        //                     EnhancementRequestedByProfileGUID = a.EnhancementRequestedByProfileGUID,
        //                     EnhancementDetails = a.EnhancementDetails,
        //                     EnhancementRequestDate = a.EnhancementRequestDate,
        //                     EstimatedResourcesAndCosts = a.EstimatedResourcesAndCosts,
        //                     ApprovalStatus = a.ApprovalStatus,
        //                     ApprovedByProfileGUID = a.ApprovedByProfileGUID.Value,
        //                     ApprovalComments = a.ApprovalComments,
        //                     dataAppEnhancementRequestRowVersion = a.dataAppEnhancementRequestRowVersion,
        //                     Active = a.Active,
        //                 }).FirstOrDefault();

        //    if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("ApplicationEnhancements", "ServiceRequest", new { Area = "SRS" }));

        //    return View("~/Areas/SRS/Views/ApplicationEnhancements/ApplicationEnhancement.cshtml", model);
        //}

        //[HttpPost, ValidateAntiForgeryToken]
        //public ActionResult ApplicationEnhancementCreate(ApplicationEnhancementRequestModel model)
        //{
        //    if (!ModelState.IsValid) return PartialView("~/Areas/SRS/Views/ApplicationEnhancements/_ApplicationEnhancementForm.cshtml", model);

        //    DateTime ExecutionTime = DateTime.Now;

        //    Guid EntityPK = Guid.NewGuid();

        //    dataAppEnhancementRequest dataAppEnhancementRequest = Mapper.Map(model, new dataAppEnhancementRequest());

        //    dataAppEnhancementRequest.AppEnhancementGUID = EntityPK;
        //    dataAppEnhancementRequest.EnhancementRequestDate = DateTime.Now;
        //    dataAppEnhancementRequest.EnhancementRequestedByProfileGUID = UserProfileGUID;

        //    DbSRS.CreateNoAudit(dataAppEnhancementRequest);


        //    List<PartialViewModel> Partials = new List<PartialViewModel>();
        //    Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ApplicationEnhancementAttachementDataTable, ControllerContext, "ApplicationEnhancementAttachementContainer"));

        //    List<UIButtons> UIButtons = new List<UIButtons>();
        //    UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButtonNoPermission(new UrlHelper(Request.RequestContext).Action("Create", "ApplicationEnhancement", new { Area = "SRS" })), Container = "ApplicationEnhancementFormControls" });
        //    UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButtonNoPermission(), Container = "ApplicationEnhancementFormControls" });
        //    UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButtonNoPermission(), Container = "ApplicationEnhancementFormControls" });

        //    try
        //    {
        //        DbSRS.SaveChanges();
        //        new Email().SendHelpDeskRequest("DLSyria-AppsSupport@unhcr.org", "", "New Application Enhancement", "ApplicationEnhancement", "Application Enhancement", model.AppEnhancementGUID);
        //        return Json(DbSRS.SingleCreateMessage(DbSRS.PrimaryKeyControl(dataAppEnhancementRequest), DbSRS.RowVersionControls(Portal.SingleToList(dataAppEnhancementRequest)), Partials, "", UIButtons));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(DbSRS.ErrorMessage(ex.Message));
        //    }
        //}

        //[HttpPost, ValidateAntiForgeryToken()]
        //public ActionResult ApplicationEnhancementUpdate(ApplicationEnhancementRequestModel model)
        //{
        //    //if (model.RequestedByProfileGUID != UserProfileGUID)
        //    //{
        //    //    throw new HttpException(401, "Unauthorized access");
        //    //}

        //    DateTime ExecutionTime = DateTime.Now;

        //    dataAppEnhancementRequest dataAppEnhancementRequest = Mapper.Map(model, new dataAppEnhancementRequest());

        //    if (CMS.HasAction(Permissions.Newservicerequest.Update, Apps.SRS))
        //    {
        //        dataAppEnhancementRequest.ApprovedByProfileGUID = UserProfileGUID;
        //        DbSRS.Update(dataAppEnhancementRequest, Permissions.Serviceenhancementrequest.UpdateGuid, ExecutionTime, DbCMS);
        //    }
        //    else
        //    {
        //        DbSRS.UpdateNoAudit(dataAppEnhancementRequest);
        //    }

        //    try
        //    {

        //        DbSRS.SaveChanges();
        //        return Json(DbSRS.SingleUpdateMessage(null, null, DbSRS.RowVersionControls(Portal.SingleToList(dataAppEnhancementRequest))));
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(DbSRS.ErrorMessage(ex.Message));
        //    }
        //}

        //[HttpPost, ValidateAntiForgeryToken]
        //public ActionResult ApplicationEnhancementDelete(dataAppEnhancementRequest model)
        //{
        //    if (model.EnhancementRequestedByProfileGUID != UserProfileGUID)
        //    {
        //        return Json(DbSRS.ErrorMessage("Unauthorized access,this record was not created by you"));
        //    }

        //    List<dataAppEnhancementRequest> DeletedApplicationEnhancements = DeleteApplicationEnhancements(Portal.SingleToList(model));

        //    List<UIButtons> UIButtons = new List<UIButtons>();
        //    UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButtonNoPermission(), Container = "ApplicationEnhancementFormControls" });

        //    try
        //    {
        //        int CommitedRows = DbSRS.SaveChanges();
        //        return Json(DbSRS.SingleDeleteMessage(CommitedRows, DeletedApplicationEnhancements.FirstOrDefault(), "ApplicationEnhancementAttachementContainer", UIButtons));
        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {
        //        return Json(DbSRS.ErrorMessage(ex.Message));
        //        //return ConcrrencyApplication(model.ApplicationGUID);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(DbSRS.ErrorMessage(ex.Message));
        //    }
        //}

        //private List<dataAppEnhancementRequest> DeleteApplicationEnhancements(List<dataAppEnhancementRequest> models)
        //{
        //    DateTime ExecutionTime = DateTime.Now;
        //    List<dataAppEnhancementRequest> DeletedApplicationEnhancements = new List<dataAppEnhancementRequest>();

        //    //THIS IS FOR PERMISSION TEST//
        //    //DO NOT DELETE THIS CODE
        //    //Select the table and all the factors from other tables into one query.
        //    //string baseQuery = (from a in DbCMS.codeApplications
        //    //                    from f in DbCMS.codeApplicationsFactorForTest
        //    //                    where a.ApplicationGUID == f.ApplicationGUID
        //    //                    select new { a.ApplicationGUID, a.codeApplicationsRowVersion, f.OperationGUID, f.OrganizationGUID, f.CountryGUID }).AsQueryable().ToString();

        //    string query = DbSRS.QueryBuilder(models, Permissions.Serviceenhancementrequest.DeleteGuid, SubmitTypes.Delete, "");

        //    var Records = DbSRS.Database.SqlQuery<dataAppEnhancementRequest>(query).ToList();
        //    foreach (var record in Records)
        //    {
        //        DeletedApplicationEnhancements.Add(DbSRS.Delete(record, ExecutionTime, Permissions.Serviceenhancementrequest.DeleteGuid));
        //    }

        //    //var Languages = DeletedApplicationsRequests.SelectMany(a => a.).Where(l => l.Active).ToList();
        //    //foreach (var language in Languages)
        //    //{
        //    //    DbCMS.Delete(language, ExecutionTime, Permissions.ApplicationsLanguages.DeleteGuid);
        //    //}
        //    return DeletedApplicationEnhancements;
        //}

        //public ActionResult ApplicationEnhancementAttachementDataTable(DataTableRecievedOptions options, Guid PK)
        //{
        //    if (options.columns == null) return PartialView("~/Areas/SRS/Views/ApplicationEnhancements/_ApplicationEnhancementAttachementDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

        //    DataTableOptions DataTable = ConvertOptions.Fill(options);

        //    Expression<Func<dataAppEnhancementAttachement, bool>> Predicate = p => true;

        //    if (DataTable.Filters.FilterRules != null)
        //    {
        //        Predicate = SearchHelper.CreateSearchPredicate<dataAppEnhancementAttachement>(DataTable.Filters);
        //    }

        //    var Result = (from a in DbSRS.dataAppEnhancementAttachement.AsNoTracking().AsExpandable().Where(x => x.Active && x.AppEnhancementGUID == PK).Where(Predicate)
        //                  select new
        //                  {
        //                      a.AppAttachementGUID,
        //                      a.AppEnhancementGUID,
        //                      a.FileName,
        //                      a.Active,
        //                      a.dataAppEnhancementAttachementRowVersion,
        //                  });

        //    Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

        //    return Json(Portal.DataTable(Result.Count(), Result.ToList()), JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult ApplicationEnhancementAttachementCreate(Guid FK)
        //{
        //    return PartialView("~/Areas/SRS/Views/ApplicationEnhancements/_ApplicationEnhancementAttachementUpdateModal.cshtml",
        //        new dataAppEnhancementAttachement { AppEnhancementGUID = FK });
        //}

        //[HttpPost]
        //public FineUploaderResult ApplicationEnhancementAttachementCreate(FineUpload upload, Guid AppEnhancementGUID)
        //{
        //    var _stearm = upload.InputStream;
        //    string FolderPath = Server.MapPath("~/Uploads/SRS/" + AppEnhancementGUID.ToString());
        //    Directory.CreateDirectory(FolderPath);
        //    string FilePath = FolderPath + "/" + upload.FileName;
        //    using (var fileStream = System.IO.File.Create(FilePath))
        //    {
        //        upload.InputStream.Seek(0, SeekOrigin.Begin);
        //        upload.InputStream.CopyTo(fileStream);
        //    }
        //    dataAppEnhancementAttachement dataAppEnhancementAttachement = new dataAppEnhancementAttachement();
        //    dataAppEnhancementAttachement.AppEnhancementGUID = AppEnhancementGUID;
        //    dataAppEnhancementAttachement.AttachementPath = FilePath;
        //    dataAppEnhancementAttachement.FileName = upload.FileName;

        //    try
        //    {
        //        DbSRS.CreateNoAudit(dataAppEnhancementAttachement);
        //        DbSRS.SaveChanges();
        //        return new FineUploaderResult(true, new { path = FilePath, success = true });
        //    }
        //    catch (Exception ex)
        //    {
        //        return new FineUploaderResult(false, new { error = "error: " + ex.Message });
        //    }
        //}

        //[Route("SRS/ServiceRequest/ApplicationEnhancementAttachementDownload/{PK}")]
        //public FileResult ApplicationEnhancementAttachementDownload(Guid PK)
        //{
        //    Guid AppAttachementGUID = PK;

        //    dataAppRequestAttachement dataAppRequestAttachement = (from a in DbSRS.dataAppRequestAttachement.Where(x => x.AppAttachementGUID == AppAttachementGUID) select a).FirstOrDefault();
        //    string FolderPath = Server.MapPath("~/Uploads/SRS/" + dataAppRequestAttachement.AppRequestGUID.ToString());

        //    string FileName = dataAppRequestAttachement.FileName;
        //    string DownloadURL = FolderPath + "\\" + FileName;

        //    byte[] fileBytes = System.IO.File.ReadAllBytes(@"" + DownloadURL + "");

        //    try
        //    {
        //        return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        //#endregion



        //#region Application Bug Report
        //[Route("SRS/BugReports")]
        //public ActionResult BugReportsIndex()
        //{
        //    return View("~/Areas/SRS/Views/BugReports/Index.cshtml");
        //}

        //[Route("SRS/BugReports/BugReportsDataTable")]
        //public ActionResult BugReportsDataTable(DataTableRecievedOptions options)
        //{
        //    DataTableOptions DataTable = ConvertOptions.Fill(options);

        //    Expression<Func<BugReportsDataTableModel, bool>> Predicate = p => true;

        //    if (DataTable.Filters.FilterRules != null)
        //    {
        //        Predicate = SearchHelper.CreateSearchPredicate<BugReportsDataTableModel>(DataTable.Filters);
        //    }

        //    var All = (from a in DbSRS.dataAppBugReport.AsExpandable().Where(x => x.Active)
        //               join b in DbSRS.codeApplicationsLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.ApplicationGUID equals b.ApplicationGUID
        //               select new BugReportsDataTableModel
        //               {
        //                   AppBugReportGUID = a.AppBugReportGUID,
        //                   ApplicationDescription = b.ApplicationDescription,
        //                   BugReportByProfileGUID = a.BugReportByProfileGUID,
        //                   dataAppBugReportRowVersion = a.dataAppBugReportRowVersion,
        //                   Active = a.Active,
        //               }).Where(Predicate);

        //    All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

        //    List<BugReportsDataTableModel> Result = Mapper.Map<List<BugReportsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

        //    return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        //}

        //[Route("SRS/BugReports/Create/")]
        //public ActionResult BugReportsCreate()
        //{
        //    return View("~/Areas/SRS/Views/BugReports/BugReports.cshtml", new BugReportModel());
        //}

        //[Route("SRS/BugReports/Update/{PK}")]
        //public ActionResult BugReportsUpdate(Guid PK)
        //{
        //    var model = (from a in DbSRS.dataAppBugReport.Where(x => x.Active && x.AppBugReportGUID == PK)
        //                 select new BugReportModel
        //                 {
        //                     AppBugReportGUID = a.AppBugReportGUID,
        //                     ApplicationGUID = a.ApplicationGUID,
        //                     BugReportByProfileGUID = a.BugReportByProfileGUID,
        //                     BugDetails = a.BugDetails,
        //                     BugReporttDate = a.BugReporttDate,
        //                     BugFixDate = a.BugFixDate.Value,
        //                     BugStatus = a.BugStatus,
        //                     SolvedBugComments = a.SolvedBugComments,
        //                     dataAppBugReportRowVersion = a.dataAppBugReportRowVersion,
        //                     Active = a.Active,
        //                 }).FirstOrDefault();

        //    if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("BugReports", "ServiceRequest", new { Area = "SRS" }));

        //    return View("~/Areas/SRS/Views/BugReports/BugReports.cshtml", model);
        //}

        //[HttpPost, ValidateAntiForgeryToken]
        //public ActionResult BugReportsCreate(BugReportModel model)
        //{
        //    if (!ModelState.IsValid) return PartialView("~/Areas/SRS/Views/BugReports/_BugReportsForm.cshtml", model);

        //    DateTime ExecutionTime = DateTime.Now;

        //    Guid EntityPK = Guid.NewGuid();


        //    dataAppBugReport dataAppBugReport = Mapper.Map(model, new dataAppBugReport());

        //    dataAppBugReport.AppBugReportGUID = EntityPK;
        //    dataAppBugReport.BugReporttDate = DateTime.Now;
        //    dataAppBugReport.BugReportByProfileGUID = UserProfileGUID;

        //    DbSRS.CreateNoAudit(dataAppBugReport);

        //    List<PartialViewModel> Partials = new List<PartialViewModel>();
        //    Partials.Add(Portal.PartialView(EntityPK, DataTableNames.BugReportsDataTable, ControllerContext, "ApplicationBugAttachementContainer"));

        //    List<UIButtons> UIButtons = new List<UIButtons>();
        //    UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButtonNoPermission(new UrlHelper(Request.RequestContext).Action("Create", "BugReports", new { Area = "SRS" })), Container = "ApplicationBugReportFormControls" });
        //    UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButtonNoPermission(), Container = "ApplicationBugReportFormControls" });
        //    UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButtonNoPermission(), Container = "ApplicationBugReportFormControls" });

        //    try
        //    {
        //        DbSRS.SaveChanges();

        //        new Email().SendHelpDeskRequest("DLSyria-AppsSupport@unhcr.org", "", "New Bug Report", "BugReports", "Bug Report", model.AppBugReportGUID);

        //        return Json(DbSRS.SingleCreateMessage(DbSRS.PrimaryKeyControl(dataAppBugReport), DbSRS.RowVersionControls(Portal.SingleToList(dataAppBugReport)), Partials, "", UIButtons));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(DbSRS.ErrorMessage(ex.Message));
        //    }
        //}

        //[HttpPost, ValidateAntiForgeryToken()]
        //public ActionResult BugReportsUpdate(BugReportModel model)
        //{
        //    //if (model.RequestedByProfileGUID != UserProfileGUID)
        //    //{
        //    //    throw new HttpException(401, "Unauthorized access");
        //    //}

        //    DateTime ExecutionTime = DateTime.Now;

        //    dataAppBugReport dataAppBugReport = Mapper.Map(model, new dataAppBugReport());

        //    if (CMS.HasAction(Permissions.Newservicerequest.Update, Apps.SRS))
        //    {
        //        DbSRS.Update(dataAppBugReport, Permissions.Bugreportmanagement.UpdateGuid, ExecutionTime, DbCMS);
        //    }
        //    else
        //    {
        //        DbSRS.UpdateNoAudit(dataAppBugReport);
        //    }

        //    try
        //    {

        //        DbSRS.SaveChanges();
        //        return Json(DbSRS.SingleUpdateMessage(null, null, DbSRS.RowVersionControls(Portal.SingleToList(dataAppBugReport))));
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(DbSRS.ErrorMessage(ex.Message));
        //    }
        //}

        //[HttpPost, ValidateAntiForgeryToken]
        //public ActionResult BugReportsDelete(dataAppBugReport model)
        //{
        //    if (model.BugReportByProfileGUID != UserProfileGUID)
        //    {
        //        return Json(DbSRS.ErrorMessage("Unauthorized access,this record was not created by you"));
        //    }

        //    List<dataAppBugReport> DeletedApplicationBugs = DeleteApplicationBugs(Portal.SingleToList(model));

        //    List<UIButtons> UIButtons = new List<UIButtons>();
        //    UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButtonNoPermission(), Container = "ApplicationBugReportFormControls" });

        //    try
        //    {
        //        int CommitedRows = DbSRS.SaveChanges();
        //        return Json(DbSRS.SingleDeleteMessage(CommitedRows, DeletedApplicationBugs.FirstOrDefault(), "ApplicationBugAttachementContainer", UIButtons));
        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {
        //        return Json(DbSRS.ErrorMessage(ex.Message));
        //        //return ConcrrencyApplication(model.ApplicationGUID);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(DbSRS.ErrorMessage(ex.Message));
        //    }
        //}

        //private List<dataAppBugReport> DeleteApplicationBugs(List<dataAppBugReport> models)
        //{
        //    DateTime ExecutionTime = DateTime.Now;
        //    List<dataAppBugReport> DeletedApplicationBugs = new List<dataAppBugReport>();

        //    //THIS IS FOR PERMISSION TEST//
        //    //DO NOT DELETE THIS CODE
        //    //Select the table and all the factors from other tables into one query.
        //    //string baseQuery = (from a in DbCMS.codeApplications
        //    //                    from f in DbCMS.codeApplicationsFactorForTest
        //    //                    where a.ApplicationGUID == f.ApplicationGUID
        //    //                    select new { a.ApplicationGUID, a.codeApplicationsRowVersion, f.OperationGUID, f.OrganizationGUID, f.CountryGUID }).AsQueryable().ToString();

        //    string query = DbSRS.QueryBuilder(models, Permissions.Bugreportmanagement.DeleteGuid, SubmitTypes.Delete, "");

        //    var Records = DbSRS.Database.SqlQuery<dataAppBugReport>(query).ToList();
        //    foreach (var record in Records)
        //    {
        //        DeletedApplicationBugs.Add(DbSRS.Delete(record, ExecutionTime, Permissions.Bugreportmanagement.DeleteGuid));
        //    }

        //    //var Languages = DeletedApplicationsRequests.SelectMany(a => a.).Where(l => l.Active).ToList();
        //    //foreach (var language in Languages)
        //    //{
        //    //    DbCMS.Delete(language, ExecutionTime, Permissions.ApplicationsLanguages.DeleteGuid);
        //    //}
        //    return DeletedApplicationBugs;
        //}

        //public ActionResult BugReportsAttachementDataTable(DataTableRecievedOptions options, Guid PK)
        //{
        //    if (options.columns == null) return PartialView("~/Areas/SRS/Views/BugReports/_BugReportAttachementDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

        //    DataTableOptions DataTable = ConvertOptions.Fill(options);

        //    Expression<Func<dataAppBugReportAttachement, bool>> Predicate = p => true;

        //    if (DataTable.Filters.FilterRules != null)
        //    {
        //        Predicate = SearchHelper.CreateSearchPredicate<dataAppBugReportAttachement>(DataTable.Filters);
        //    }

        //    var Result = (from a in DbSRS.dataAppBugReportAttachement.AsNoTracking().AsExpandable().Where(x => x.Active && x.AppBugReportGUID == PK).Where(Predicate)
        //                  select new
        //                  {
        //                      a.BugAttachementGUID,
        //                      a.AppBugReportGUID,
        //                      a.FileName,
        //                      a.Active,
        //                      a.dataAppBugReportAttachementRowVersion,
        //                  });

        //    Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

        //    return Json(Portal.DataTable(Result.Count(), Result.ToList()), JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult BugReportsAttachementCreate(Guid FK)
        //{
        //    return PartialView("~/Areas/SRS/Views/BugReports/_ApplicationEnhancementAttachementUpdateModal.cshtml",
        //        new dataAppBugReportAttachement { AppBugReportGUID = FK });
        //}

        //[HttpPost]
        //public FineUploaderResult BugReportsAttachementCreate(FineUpload upload, Guid AppBugReportGUID)
        //{
        //    var _stearm = upload.InputStream;
        //    string FolderPath = Server.MapPath("~/Uploads/SRS/" + AppBugReportGUID.ToString());
        //    Directory.CreateDirectory(FolderPath);
        //    string FilePath = FolderPath + "/" + upload.FileName;
        //    using (var fileStream = System.IO.File.Create(FilePath))
        //    {
        //        upload.InputStream.Seek(0, SeekOrigin.Begin);
        //        upload.InputStream.CopyTo(fileStream);
        //    }
        //    dataAppBugReportAttachement dataAppBugReportAttachement = new dataAppBugReportAttachement();
        //    dataAppBugReportAttachement.AppBugReportGUID = AppBugReportGUID;
        //    dataAppBugReportAttachement.AttachementPath = FilePath;
        //    dataAppBugReportAttachement.FileName = upload.FileName;

        //    try
        //    {
        //        DbSRS.CreateNoAudit(dataAppBugReportAttachement);
        //        DbSRS.SaveChanges();
        //        return new FineUploaderResult(true, new { path = FilePath, success = true });
        //    }
        //    catch (Exception ex)
        //    {
        //        return new FineUploaderResult(false, new { error = "error: " + ex.Message });
        //    }
        //}

        //[Route("SRS/ServiceRequest/BugReportsAttachementDownload/{PK}")]
        //public FileResult BugReportsAttachementDownload(Guid PK)
        //{
        //    Guid BugAttachementGUID = PK;

        //    dataAppBugReportAttachement dataAppBugReportAttachement = (from a in DbSRS.dataAppBugReportAttachement.Where(x => x.BugAttachementGUID == BugAttachementGUID) select a).FirstOrDefault();
        //    string FolderPath = Server.MapPath("~/Uploads/SRS/" + dataAppBugReportAttachement.AppBugReportGUID.ToString());

        //    string FileName = dataAppBugReportAttachement.FileName;
        //    string DownloadURL = FolderPath + "\\" + FileName;

        //    byte[] fileBytes = System.IO.File.ReadAllBytes(@"" + DownloadURL + "");

        //    try
        //    {
        //        return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        //#endregion

    }
}