using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using FineUploader;
using LinqKit;
using PPA_DAL.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.PPA.Controllers
{
    public class ManagementController : PPABaseController
    {

        [Route("RTT")]
        public ActionResult PPAHome()
        {

            CMS.SetUserToken(UserProfileGUID, Apps.PPA);
            Session[SessionKeys.CurrentApp] = Apps.PPA;
            CMS.BuildUserMenus(UserGUID, LAN);
            if (!CMS.HasAction(Permissions.PPAManagement.Access, Apps.PPA) && !CMS.HasAction(Permissions.PPAManagement.FullAccess, Apps.PPA))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/PPA/Views/Management/Home.cshtml");
        }

        [Route("PPA/")]
        public ActionResult PPAIndex()
        {
            if (!CMS.HasAction(Permissions.PPAManagement.Access, Apps.PPA) && !CMS.HasAction(Permissions.PPAManagement.FullAccess, Apps.PPA))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/PPA/Views/Management/Index.cshtml");
        }

        #region PPA
        [Route("PPA/Management/PPADataTable/")]
        public JsonResult PPADataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<PPADataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<PPADataTableModel>(DataTable.Filters);
            }




            if (CMS.HasAction(Permissions.PPAManagement.FullReadWrite, Apps.PPA))
            {
                var All = (from a in DbPPA.ProjectPartnershipAgreement.AsExpandable().Where(x=>x.Active)
                           join b in DbPPA.userProfiles.Where(x => x.Active) on a.CreatedByUserProfileGUID equals b.UserProfileGUID
                           join c in DbPPA.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on b.userServiceHistory.UserGUID equals c.UserGUID
                           join d in DbPPA.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.PPATypeGUID equals d.ValueGUID
                           join e in DbPPA.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.PPAStatusGUID equals e.ValueGUID
                           join f in DbPPA.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.ImplementationAreaGUID equals f.ValueGUID
                           select new PPADataTableModel
                           {
                               PPAGUID = a.PPAGUID,
                               PPATypeDescription = d.ValueDescription,
                               PPAStatusDescription = e.ValueDescription,
                               PPADescription = a.PPADescription,
                               PPADeadLine = a.PPADeadLine,
                               StaffName = c.FirstName + " " + c.Surname,
                               PPAImplementationArea = f.ValueDescription,
                               Active = a.Active,
                               ProjectPartnershipAgreementRowVersion = a.ProjectPartnershipAgreementRowVersion,
                           }).Where(Predicate);

                All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

                List<PPADataTableModel> Result = Mapper.Map<List<PPADataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

                return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
            }
            else if (CMS.HasAction(Permissions.PPAManagement.FullAccess, Apps.PPA))
            {
                var All = (from a in DbPPA.ProjectPartnershipAgreement.AsExpandable().Where(x => x.Active)
                           join b in DbPPA.userProfiles.Where(x => x.Active) on a.CreatedByUserProfileGUID equals b.UserProfileGUID
                           join c in DbPPA.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on b.userServiceHistory.UserGUID equals c.UserGUID
                           join d in DbPPA.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.PPATypeGUID equals d.ValueGUID
                           join e in DbPPA.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.PPAStatusGUID equals e.ValueGUID
                           join f in DbPPA.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.ImplementationAreaGUID equals f.ValueGUID
                           select new PPADataTableModel
                           {
                               PPAGUID = a.PPAGUID,
                               PPATypeDescription = d.ValueDescription,
                               PPAStatusDescription = e.ValueDescription,
                               PPADescription = a.PPADescription,
                               PPADeadLine = a.PPADeadLine,
                               StaffName = c.FirstName + " " + c.Surname,
                               PPAImplementationArea = f.ValueDescription,
                               Active = a.Active,
                               ProjectPartnershipAgreementRowVersion = a.ProjectPartnershipAgreementRowVersion,
                           }).Where(Predicate);

                All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

                List<PPADataTableModel> Result = Mapper.Map<List<PPADataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

                return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var All = (from a in DbPPA.ProjectPartnershipAgreement.AsExpandable()
                           join b in DbPPA.userProfiles.Where(x => x.Active) on a.CreatedByUserProfileGUID equals b.UserProfileGUID
                           join c in DbPPA.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on b.userServiceHistory.UserGUID equals c.UserGUID
                           join d in DbPPA.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.PPATypeGUID equals d.ValueGUID
                           join e in DbPPA.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.PPAStatusGUID equals e.ValueGUID
                           where a.CreatedByUserProfileGUID == UserProfileGUID
                           select new PPADataTableModel
                           {
                               PPAGUID = a.PPAGUID,
                               PPATypeDescription = d.ValueDescription,
                               PPAStatusDescription = e.ValueDescription,
                               PPADescription = a.PPADescription,
                               PPADeadLine = a.PPADeadLine,
                               StaffName = c.FirstName + " " + c.Surname,
                               Active = a.Active,
                               ProjectPartnershipAgreementRowVersion = a.ProjectPartnershipAgreementRowVersion,
                           }).Where(Predicate);

                All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

                List<PPADataTableModel> Result = Mapper.Map<List<PPADataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

                return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
            }

        }

        [Route("PPA/Management/Create/")]
        public ActionResult PPACreate()
        {
            if (!CMS.HasAction(Permissions.PPAManagement.Create, Apps.PPA))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/PPA/Views/Management/PPA.cshtml", new PPAUpdateModel());
        }

        [Route("PPA/Management/Update/{PK}")]
        public ActionResult PPAUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.PPAManagement.Update, Apps.PPA) && !CMS.HasAction(Permissions.PPAManagement.FullAccess, Apps.PPA))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            var model = (from a in DbPPA.ProjectPartnershipAgreement.Where(x => x.PPAGUID == PK)
                         select new PPAUpdateModel
                         {
                             PPAGUID = a.PPAGUID,
                             PPADescription = a.PPADescription,
                             PPATypeGUID = a.PPATypeGUID,
                             ImplementationAreaGUID = a.ImplementationAreaGUID,
                             PPAStatusGUID = a.PPAStatusGUID,
                             PPADeadLine = a.PPADeadLine,
                             PPAFolderPath = a.PPAFolderPath,
                             CreatedByUserProfileGUID = a.CreatedByUserProfileGUID,
                             OrganizationInstanceGUID = a.OrganizationInstanceGUID,
                             Active = a.Active,
                             ReminderInterval = a.ReminderInterval,
                             ProjectPartnershipAgreementRowVersion = a.ProjectPartnershipAgreementRowVersion
                         }).FirstOrDefault();


            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("PPA", "Management", new { Area = "PPA" }));

            return View("~/Areas/PPA/Views/Management/PPA.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult PPACreate(PPAUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.PPAManagement.Create, Apps.PPA))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            Guid EntityPK = Guid.NewGuid();
            DateTime ExecutionTime = DateTime.Now;

            ProjectPartnershipAgreement projectPartnershipAgreement = Mapper.Map(model, new ProjectPartnershipAgreement());
            projectPartnershipAgreement.PPAGUID = EntityPK;
            projectPartnershipAgreement.CreatedByUserProfileGUID = UserProfileGUID;
            projectPartnershipAgreement.PPAStatusGUID = Guid.Parse("E325612C-1D84-4770-97AE-51681A8F1533"); //pending
            projectPartnershipAgreement.PPAFolderPath = "~/Uploads/PPA/" + EntityPK.ToString();
            DbPPA.Create(projectPartnershipAgreement, Permissions.PPAManagement.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.PPAFilesDataTable, ControllerContext, "FilesContainer"));
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.PPADistributionListDataTable, ControllerContext, "DistributionListContainer"));
            //Partials.Add(Portal.PartialView(EntityPK, DataTableNames.PPAReviewerListDataTable, ControllerContext, "ReviewerListContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.PPAManagement.Create, Apps.PPA, new UrlHelper(Request.RequestContext).Action("Management/Create", "Management", new { Area = "PPA" })), Container = "PPAFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.PPAManagement.Update, Apps.PPA), Container = "PPAFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.PPAManagement.Delete, Apps.PPA), Container = "PPAFormControls" });

            try
            {
                DbPPA.SaveChanges();
                return Json(DbPPA.SingleCreateMessage(DbPPA.PrimaryKeyControl(projectPartnershipAgreement), DbPPA.RowVersionControls(Portal.SingleToList(projectPartnershipAgreement)), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbPPA.ErrorMessage(ex.Message));
            }

        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult PPAUpdate(PPAUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.PPAManagement.Update, Apps.PPA))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (model.CreatedByUserProfileGUID != UserProfileGUID)
            {
                throw new HttpException(401, "Unauthorized access");
            }
            DateTime ExecutionTime = DateTime.Now;
            ProjectPartnershipAgreement projectPartnershipAgreement = (from a in DbPPA.ProjectPartnershipAgreement.Where(x => x.PPAGUID == model.PPAGUID) select a).FirstOrDefault();
            projectPartnershipAgreement.PPADeadLine = model.PPADeadLine.Value;
            projectPartnershipAgreement.PPATypeGUID = model.PPATypeGUID;
            projectPartnershipAgreement.ImplementationAreaGUID = model.ImplementationAreaGUID;
            projectPartnershipAgreement.PPADescription = model.PPADescription;
            DbPPA.Update(projectPartnershipAgreement, Permissions.PPAManagement.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbPPA.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, DbCMS.RowVersionControls(Portal.SingleToList(projectPartnershipAgreement))));
            }
            catch (Exception ex)
            {
                return Json(DbPPA.ErrorMessage(ex.Message));
            }

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PPADelete(ProjectPartnershipAgreement model)
        {
            if (!CMS.HasAction(Permissions.PPAManagement.Delete, Apps.PPA))
            {
                return Json(DbPPA.ErrorMessage("Unauthorized access,this record was not created by you"));
            }
            if (model.CreatedByUserProfileGUID != UserProfileGUID)
            {
                return Json(DbPPA.ErrorMessage("Unauthorized access,this record was not created by you"));
            }
            List<ProjectPartnershipAgreement> DeletedApplications = DeletePPA(Portal.SingleToList(model));

           
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.PPAManagement.Restore, Apps.PPA), Container = "PPAFormControls" });

            try
            {
                int CommitedRows = DbPPA.SaveChanges();
                return Json(DbPPA.SingleDeleteMessage(CommitedRows, DeletedApplications.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
                //return ConcrrencyApplication(model.ApplicationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPPA.ErrorMessage(ex.Message));
            }
        }

        private List<ProjectPartnershipAgreement> DeletePPA(List<ProjectPartnershipAgreement> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<ProjectPartnershipAgreement> DeletedPPA = new List<ProjectPartnershipAgreement>();

            //THIS IS FOR PERMISSION TEST//
            //DO NOT DELETE THIS CODE
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = (from a in DbCMS.codeApplications
            //                    from f in DbCMS.codeApplicationsFactorForTest
            //                    where a.ApplicationGUID == f.ApplicationGUID
            //                    select new { a.ApplicationGUID, a.codeApplicationsRowVersion, f.OperationGUID, f.OrganizationGUID, f.CountryGUID }).AsQueryable().ToString();

            string query = DbPPA.QueryBuilder(models, Permissions.PPAManagement.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbPPA.Database.SqlQuery<ProjectPartnershipAgreement>(query).ToList();
            foreach (var record in Records)
            {
                DeletedPPA.Add(DbPPA.Delete(record, ExecutionTime, Permissions.PPAManagement.DeleteGuid,DbCMS));
            }

            return DeletedPPA;
        }
        #endregion

        #region PPA Files
        public ActionResult PPAFilesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/PPA/Views/PPAFiles/_PPAFilesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<PPAOriginalFile, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<PPAOriginalFile>(DataTable.Filters);
            }

            var Result = (from a in DbPPA.PPAOriginalFile.AsNoTracking().AsExpandable().Where(x => x.Active && x.PPAGUID == PK).Where(Predicate)
                          join b in DbPPA.userProfiles.AsNoTracking().AsExpandable().Where(x => x.Active) on a.UploadByUserGUID equals b.UserProfileGUID
                          join c in DbPPA.userPersonalDetailsLanguage.AsNoTracking().AsExpandable().Where(x => x.LanguageID == LAN && x.Active) on b.userServiceHistory.UserGUID equals c.UserGUID
                          join d in DbPPA.codeTablesValuesLanguages.AsNoTracking().AsExpandable().Where(x => x.LanguageID == LAN && x.Active) on a.PPAFileCategoryGUID equals d.ValueGUID
                          select new
                          {
                              a.PPAOriginalFileGUID,
                              a.FileName,
                              FileType = a.FileType.ToString() == FileTypes.Excel.ToString() ? "Excel" : a.FileType.ToString() == FileTypes.Word.ToString() ? "Word" : "PDF",
                              PPAFileCategory = d.ValueDescription,
                              a.UploadDate,
                              UploadByUserGUID = c.FirstName + " " + c.Surname,
                              a.Active,
                              a.PPAOriginalFileRowVersion
                          });


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result.ToList()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult PPAFilesCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.PPAManagement.Create, Apps.PPA))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/PPA/Views/PPAFiles/_PPAFilesUpdateModal.cshtml",
                new PPAOriginalFile { PPAGUID = FK });
        }

        [HttpPost]
        public FineUploaderResult UploadPPAFiles(FineUpload upload, Guid PPAGUID,Guid PPAFileCategoryGUID)
        {
            if(PPAFileCategoryGUID == null)
            {
                return new FineUploaderResult(false, new { error = "error: Kindly select the document category for the file: " + upload.FileName });
            }
            var ppaRecordCreatedByUserProfileGUID = (from a in DbPPA.ProjectPartnershipAgreement.Where(x => x.Active && x.PPAGUID == PPAGUID) select a.CreatedByUserProfileGUID).FirstOrDefault();
            if (ppaRecordCreatedByUserProfileGUID != UserProfileGUID)
            {
                return new FineUploaderResult(false, new { error = "error: Unauthorized access" });
            }
            var _stearm = upload.InputStream;
            string _ext = Path.GetExtension(upload.FileName).Split('.')[1];
            string FolderPath = Server.MapPath("~/Uploads/PPA/" + PPAGUID.ToString());
            Directory.CreateDirectory(FolderPath);
            string FilePath = FolderPath + "/" + upload.FileName;
            using (var fileStream = System.IO.File.Create(FilePath))
            {
                upload.InputStream.Seek(0, SeekOrigin.Begin);
                upload.InputStream.CopyTo(fileStream);
            }
            PPAOriginalFile ppaOriginalFile = new PPAOriginalFile();
            ppaOriginalFile.PPAOriginalFileGUID = Guid.NewGuid();
            ppaOriginalFile.PPAGUID = PPAGUID;
            ppaOriginalFile.FileName = upload.FileName.Split('.')[0];
            ppaOriginalFile.StatusGUID = Guid.Parse("536CC824-8B86-4211-A65D-109B9489EE0E");//pending
            ppaOriginalFile.PPAFileCategoryGUID = PPAFileCategoryGUID;
            ppaOriginalFile.UploadDate = DateTime.Now;
            ppaOriginalFile.UploadByUserGUID = UserProfileGUID;
            ppaOriginalFile.HiddenFileExtension = _ext;
            if (_ext.ToLower() == "xls" || _ext.ToLower() == "xlsx")
            {
                ppaOriginalFile.FileType = FileTypes.Excel;
            }
            else if (_ext.ToLower() == "docx")
            {
                ppaOriginalFile.FileType = FileTypes.Word;
            }
            else if (_ext.ToLower() == "pdf")
            {
                ppaOriginalFile.FileType = FileTypes.PDF;
            }
            try
            {
                DbPPA.Create(ppaOriginalFile, Permissions.PPAManagement.CreateGuid, ppaOriginalFile.UploadDate, DbCMS);
                DbPPA.SaveChanges();
                DbCMS.SaveChanges();
                return new FineUploaderResult(true, new { path = FilePath, success = true });
            }
            catch (Exception ex)
            {
                return new FineUploaderResult(false, new { error = "error: " + ex.Message });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult PPAFilesDataTableDelete(List<PPAOriginalFile> models)
        {
            var PPAOriginalFileGUID = models.First().PPAOriginalFileGUID;
            var CreatedByUserProfileGUID = (from a in DbPPA.PPAOriginalFile.AsNoTracking().Where(x => x.Active && x.PPAOriginalFileGUID == PPAOriginalFileGUID)
                           select a.ProjectPartnershipAgreement.CreatedByUserProfileGUID).FirstOrDefault();
            if(CreatedByUserProfileGUID != UserProfileGUID)
            {
                return Json(DbPPA.ErrorMessage("Unauthorized access, this record was not added by you"));
            }
            if (!CMS.HasAction(Permissions.PPAManagement.Delete, Apps.PPA))
            {
                return Json(DbPPA.ErrorMessage("Unauthorized access"));
            }
            List<PPAOriginalFile> DeletedLanguages = DeletePPAFiles(models);

            try
            {
                DbPPA.SaveChanges();
                return Json(DbPPA.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.PPAFilesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPPA.ErrorMessage(ex.Message));
            }
        }

        private List<PPAOriginalFile> DeletePPAFiles(List<PPAOriginalFile> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<PPAOriginalFile> DeletedPPAFiles = new List<PPAOriginalFile>();

            string query = DbPPA.QueryBuilder(models, Permissions.PPAManagement.DeleteGuid, SubmitTypes.Delete, "");

            var ppaFiles = DbPPA.Database.SqlQuery<PPAOriginalFile>(query).ToList();

            foreach (var ppaFile in ppaFiles)
            {
                DeletedPPAFiles.Add(DbPPA.Delete(ppaFile, ExecutionTime, Permissions.PPAManagement.DeleteGuid,DbCMS));
            }

            var Fileversions = DeletedPPAFiles.SelectMany(a => a.PPAFileVersion).Where(l => l.Active).ToList();

            foreach (var Fileversion in Fileversions)
            {
                DbPPA.Delete(Fileversion, ExecutionTime, Permissions.PPAManagement.DeleteGuid, DbCMS);
            }

            return DeletedPPAFiles;
        }
        #endregion

        #region PPA Distribution List
        //[Route("PPA/Management/PPADistributionListDataTable/{PK}")]
        public ActionResult PPADistributionListDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/PPA/Views/PPADistributionList/_PPADistributionListDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<PPADistributionList, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<PPADistributionList>(DataTable.Filters);
            }
            var Result = (from a in DbPPA.PPADistributionList.AsNoTracking().Where(x => x.PPAGUID == PK).Where(Predicate)
                          join b in DbPPA.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.UserGUID equals b.UserGUID
                          join c in DbPPA.codeTablesValuesLanguages.Where(x=>x.LanguageID == LAN && x.Active) on a.WorkOnFileStatusGUID equals c.ValueGUID
                          where a.PPAUserAccessType == PPAUserAccessType.FullAccess
                          select new
                          {
                              a.PPADistributionListGUID,
                              StaffName = b.FirstName + " " + b.Surname,
                              UnitName = (from x in DbPPA.userServiceHistory.Where(x => x.UserGUID == a.UserGUID)
                                          join y in DbPPA.userProfiles.Where(x => x.Active) on x.ServiceHistoryGUID equals y.ServiceHistoryGUID
                                          join z in DbPPA.codeDepartmentsLanguages.Where(x => x.LanguageID == LAN && x.Active) on y.DepartmentGUID equals z.DepartmentGUID
                                          orderby y.FromDate descending
                                          select z.DepartmentDescription).FirstOrDefault(),
                              JobTitle = (from x in DbPPA.userServiceHistory.Where(x => x.UserGUID == a.UserGUID)
                                          join y in DbPPA.userProfiles.Where(x => x.Active) on x.ServiceHistoryGUID equals y.ServiceHistoryGUID
                                          join z in DbPPA.codeJobTitlesLanguages.Where(x => x.LanguageID == LAN && x.Active) on y.JobTitleGUID equals z.JobTitleGUID
                                          orderby y.FromDate descending
                                          select z.JobTitleDescription).FirstOrDefault(),
                              DutyStation = (from x in DbPPA.userServiceHistory.Where(x => x.UserGUID == a.UserGUID)
                                             join y in DbPPA.userProfiles.Where(x => x.Active) on x.ServiceHistoryGUID equals y.ServiceHistoryGUID
                                             join z in DbPPA.codeDutyStationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on y.DutyStationGUID equals z.DutyStationGUID
                                             orderby y.FromDate descending
                                             select z.DutyStationDescription).FirstOrDefault(),
                              WorkOnPPAFileStatus = c.ValueDescription,
                              a.Active,
                              a.PPADistributionListRowVersion
                          });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result.ToList()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult PPADistributionListCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.PPAManagement.Create, Apps.PPA))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/PPA/Views/PPADistributionList/_PPADistributionListUpdateModel.cshtml",
                new PPADistributionListUpdateModel { PPAGUID = FK });
        }

       

        public class UserNameAndEmailMode
        {
            public string UserFullName { get; set; }
            public string EmailAddress { get; set; }
        }

        public class PartnerAndArea
        {
            public string OrganizationDescription { get; set; }
            public Guid ImplementationAreaGUID { get; set; }
            public Guid CreatedByUserProfileGUID { get; set; }

}

        [HttpPost]
        public ActionResult PPADistributionListCreate(PPADistributionListUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.PPAManagement.Create, Apps.PPA))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            Guid PendingStatusGUID = Guid.Parse("536CC824-8B86-4211-A65D-109B9489EE0E");
            DateTime UploadDate = DateTime.Now;
            string PPACreatorName = (from a in DbPPA.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN && x.UserGUID == UserGUID)
                                     select a.FirstName + " " + a.Surname).FirstOrDefault();
            List<PPADistributionList> pPADistributionLists = new List<PPADistributionList>();

            List<UserNameAndEmailMode> UserNameAndEmailModels = new List<UserNameAndEmailMode>();
            string CCEmails = "";
            PartnerAndArea PartnerNameAndArea = (from a in DbPPA.ProjectPartnershipAgreement.Where(x => x.Active && x.PPAGUID == model.PPAGUID)
                                                 join b in DbPPA.codeOrganizationsLanguages.Where(x => x.Active && x.LanguageID == "EN") on a.OrganizationInstanceGUID equals b.OrganizationGUID
                                                 select new PartnerAndArea
                                                 {
                                                     OrganizationDescription = b.OrganizationDescription,
                                                     ImplementationAreaGUID = a.ImplementationAreaGUID,
                                                     CreatedByUserProfileGUID = a.CreatedByUserProfileGUID
                                                     
                                                 }
                                  ).FirstOrDefault();
            if (PartnerNameAndArea.CreatedByUserProfileGUID != UserProfileGUID)
            {
                return Json(DbPPA.ErrorMessage("Unauthorized access"));
            }

            #region Workers
            if (model.SelectedUsers != null)
            {
                foreach (var userGUID in model.SelectedUsers)
                {
                    bool alreadyAdded = (from a in DbPPA.PPADistributionList
                                         where a.PPAGUID == model.PPAGUID
                                         && a.UserGUID == userGUID
                                         && a.PPAUserAccessType == PPAUserAccessType.FullAccess
                                         && a.Active
                                         select a).Count() > 0;

                    if (alreadyAdded) continue;

                    pPADistributionLists.Add(new PPADistributionList
                    {
                        PPAGUID = model.PPAGUID,
                        UserGUID = userGUID,
                        WorkOnFileStatusGUID = PendingStatusGUID,
                        Deadline = model.Deadline.Value,
                        ListUploadedOn = UploadDate,
                        Active = true,
                        PPAUserAccessType = PPAUserAccessType.FullAccess
                    });
                    UserNameAndEmailMode RecipientNameEmail = (from a in DbPPA.userServiceHistory.Where(x => x.Active && x.UserGUID == userGUID)
                                                               join b in DbPPA.userProfiles.Where(x => x.Active) on a.ServiceHistoryGUID equals b.ServiceHistoryGUID
                                                               join c in DbPPA.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == "EN") on a.UserGUID equals c.UserGUID
                                                               orderby b.FromDate descending
                                                               select new UserNameAndEmailMode
                                                               {
                                                                   UserFullName = c.FirstName + " " + c.Surname,
                                                                   EmailAddress = a.EmailAddress
                                                               }).FirstOrDefault();

                    UserNameAndEmailModels.Add(RecipientNameEmail);
                }
            }
            #endregion

            #region CC Users With Access
            if(model.CCUsers != null)
            {
                foreach (var userGUID in model.CCUsers)
                {
                    UserNameAndEmailMode RecipientNameEmail = (from a in DbPPA.userServiceHistory.Where(x => x.Active && x.UserGUID == userGUID)
                                                               join b in DbPPA.userProfiles.Where(x => x.Active) on a.ServiceHistoryGUID equals b.ServiceHistoryGUID
                                                               join c in DbPPA.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == "EN") on a.UserGUID equals c.UserGUID
                                                               orderby b.FromDate descending
                                                               select new UserNameAndEmailMode
                                                               {
                                                                   UserFullName = c.FirstName + " " + c.Surname,
                                                                   EmailAddress = a.EmailAddress
                                                               }).FirstOrDefault();

                    CCEmails = CCEmails + RecipientNameEmail.EmailAddress + ";";

                    bool alreadyAdded = (from a in DbPPA.PPADistributionList
                                         where a.PPAGUID == model.PPAGUID
                                         && a.UserGUID == userGUID
                                         && a.PPAUserAccessType == PPAUserAccessType.CC1WithAccess
                                         && a.Active
                                         select a).Count() > 0;

                    if (alreadyAdded) continue;

                    pPADistributionLists.Add(new PPADistributionList
                    {
                        PPAGUID = model.PPAGUID,
                        UserGUID = userGUID,
                        Deadline = model.Deadline.Value,
                        ListUploadedOn = UploadDate,
                        PPAUserAccessType = PPAUserAccessType.CC1WithAccess
                    });
                }

            }
            #endregion

            #region CC User Without Access
            if(model.CCUsersWithoutAccess != null)
            {
                foreach (var userGUID in model.CCUsersWithoutAccess)
                {
                    UserNameAndEmailMode RecipientNameEmail = (from a in DbPPA.userServiceHistory.Where(x => x.Active && x.UserGUID == userGUID)
                                                               join b in DbPPA.userProfiles.Where(x => x.Active) on a.ServiceHistoryGUID equals b.ServiceHistoryGUID
                                                               join c in DbPPA.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == "EN") on a.UserGUID equals c.UserGUID
                                                               orderby b.FromDate descending
                                                               select new UserNameAndEmailMode
                                                               {
                                                                   UserFullName = c.FirstName + " " + c.Surname,
                                                                   EmailAddress = a.EmailAddress
                                                               }).FirstOrDefault();

                    CCEmails = CCEmails + RecipientNameEmail.EmailAddress + ";";

                    bool alreadyAdded = (from a in DbPPA.PPADistributionList
                                         where a.PPAGUID == model.PPAGUID
                                         && a.UserGUID == userGUID
                                         && a.PPAUserAccessType == PPAUserAccessType.CC2WithoutAccess
                                         && a.Active
                                         select a).Count() > 0;

                    if (alreadyAdded) continue;

                    pPADistributionLists.Add(new PPADistributionList
                    {
                        PPAGUID = model.PPAGUID,
                        UserGUID = userGUID,
                        Deadline = model.Deadline.Value,
                        ListUploadedOn = UploadDate,
                        PPAUserAccessType = PPAUserAccessType.CC2WithoutAccess
                    });
                }
            }
            #endregion

            DbPPA.CreateBulk(pPADistributionLists, Permissions.PPAManagement.CreateGuid, DateTime.Now, DbCMS);
            try
            {
                DbPPA.SaveChanges();
                DbCMS.SaveChanges();
                string toList = "";
                foreach (var recipient in UserNameAndEmailModels)
                {
                    toList = toList + recipient.EmailAddress + ";";
                }
                string dateInDamascus = Portal.LocalTime(model.Deadline.Value).Value.ToString("dd MMMM yyyy");
                string AreaOfImplementation = GetPPAAreaOfImplementation(PartnerNameAndArea.ImplementationAreaGUID);
                new Email().SendUploadedPPAFilesEmail(toList, CCEmails, "", dateInDamascus, PPACreatorName, "Auto Subject - Review of PPA for (" + PartnerNameAndArea.OrganizationDescription + " – " + AreaOfImplementation + ")", model.PPAGUID);
                return Json(DbPPA.SingleUpdateMessage(DataTableNames.PPADistributionListDataTable, null, null));
            }
            catch (Exception ex)
            {
                return Json(DbPPA.ErrorMessage(ex.Message));
            }
        }

        private string GetPPAAreaOfImplementation(Guid ImplementationAreaGUID)
        {
            string implementationAreaGUID = (from a in DbPPA.codeTablesValuesLanguages.AsNoTracking().Where(x => x.Active && x.ValueGUID == ImplementationAreaGUID) select a.ValueDescription).FirstOrDefault();
            return implementationAreaGUID;
        }

        [Route("PPA/Management/ReviewFileVersion/{FK}")]
        public ActionResult ReviewFileVersion(Guid FK)
        {
            if (!CMS.HasAction(Permissions.PPAManagement.Create, Apps.PPA) && !CMS.HasAction(Permissions.PPAManagement.FullAccess, Apps.PPA))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            Guid PPADistributionListGUID = FK;
            var temp = (from a in DbPPA.PPADistributionList.AsNoTracking().Where(x => x.PPADistributionListGUID == FK)
                        select a).FirstOrDefault();
            Guid PPAGUID = temp.PPAGUID;

            PPAReviewFileVersionsUpdateModel model = new PPAReviewFileVersionsUpdateModel();
            model.PPAGUID = PPAGUID;
            model.PPADistributionListGUID = PPADistributionListGUID;
            model.PPAFileListModel = (from a in DbPPA.PPAOriginalFile.AsNoTracking().Where(x => x.PPAGUID == PPAGUID && x.Active)
                                      join b in DbPPA.PPAFileVersion.AsNoTracking().Where(x => x.Active && x.FileActionType == "Upload") on a.PPAOriginalFileGUID equals b.PPAOriginalFileGUID
                                      join c in DbPPA.codeTablesValuesLanguages.AsNoTracking().Where(x => x.Active && x.LanguageID == LAN) on b.CurrentFileVersionStatusGUID equals c.ValueGUID
                                      select new PPAFileListModel
                                      {
                                          OriginalFileName = a.FileName,
                                          PPAFileVersionGUID = b.PPAFileVersionGUID,
                                          PPAOriginalFileGUID = b.PPAOriginalFileGUID,
                                          FileVersion = b.FileVersion,
                                          Comments = b.Comments,
                                          FileActionDate = b.FileActionDate,
                                          CurrentFileVersionStatus = c.ValueDescription
                                      }).ToList();


            return PartialView("~/Areas/PPA/Views/PPADistributionList/_ReviewFileVersions.cshtml", model);
        }

        [Route("PPA/Management/DownloadFileVersionForAudit/{FK}")]
        public FileResult DownloadFileVersionForAudit(Guid FK)
        {
            if (!CMS.HasAction(Permissions.PPAManagement.Create, Apps.PPA) && !CMS.HasAction(Permissions.PPAManagement.FullAccess, Apps.PPA))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            Guid PPAFileVersionGUID = FK;
            PPAOriginalFile PPAOriginalFile = (from a in DbPPA.PPAFileVersion.Where(x => x.PPAFileVersionGUID == PPAFileVersionGUID) select a.PPAOriginalFile).FirstOrDefault();
            Guid PPAGUID = PPAOriginalFile.PPAGUID;
            string FileExtension = PPAOriginalFile.HiddenFileExtension;
            string FileName = PPAOriginalFile.FileName + "." + FileExtension;

            string FolderPath = Server.MapPath("~/Uploads/PPA/" + PPAGUID + "/" + PPAFileVersionGUID.ToString() + "." + FileExtension);
        
            byte[] fileBytes = System.IO.File.ReadAllBytes(@"" + FolderPath + "");



            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PPADistributionListDataTableDelete(List<PPADistributionList> models)
        {
            var PPADistributionListGUID = models.First().PPADistributionListGUID;
            var CreatedByUserProfileGUID = (from a in DbPPA.PPADistributionList.AsNoTracking().Where(x => x.Active && x.PPADistributionListGUID == PPADistributionListGUID)
                                            select a.ProjectPartnershipAgreement.CreatedByUserProfileGUID).FirstOrDefault();
            if (CreatedByUserProfileGUID != UserProfileGUID)
            {
                return Json(DbPPA.ErrorMessage("Unauthorized access, this record was not added by you"));
            }
            if (!CMS.HasAction(Permissions.PPAManagement.Delete, Apps.PPA))
            {
                return Json(DbPPA.ErrorMessage("Unauthorized access"));
            }
            List<PPADistributionList> DeletedPPADistributionList = DeletePPADistributionList(models);

            try
            {
                DbPPA.SaveChanges();
                return Json(DbPPA.PartialDeleteMessage(DeletedPPADistributionList, models, DataTableNames.PPADistributionListDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPPA.ErrorMessage(ex.Message));
            }
        }

        private List<PPADistributionList> DeletePPADistributionList(List<PPADistributionList> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<PPADistributionList> DeletedPPADistributionList = new List<PPADistributionList>();

            string query = DbPPA.QueryBuilder(models, Permissions.PPAManagement.DeleteGuid, SubmitTypes.Delete, "");

            var ppaDistributionList = DbPPA.Database.SqlQuery<PPADistributionList>(query).ToList();

            foreach (var ppaDL in ppaDistributionList)
            {
                DeletedPPADistributionList.Add(DbPPA.Delete(ppaDL, ExecutionTime, Permissions.PPAManagement.DeleteGuid, DbCMS));
            }

            //var Fileversions = ppaDistributionList.SelectMany(a => a.PPAFileVersion).Where(l => l.Active).ToList();

            //foreach (var Fileversion in Fileversions)
            //{
            //    DbPPA.Delete(Fileversion, ExecutionTime, Permissions.PPAManagement.DeleteGuid, DbCMS);
            //}

            return DeletedPPADistributionList;
        }
        #endregion

        #region PPA Reviewer List
        public ActionResult PPAReviewerListDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/PPA/Views/PPAReviewerList/_PPAReviewerListDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<PPAReviewerList, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<PPAReviewerList>(DataTable.Filters);
            }
            var Result = (from a in DbPPA.PPAReviewerList.AsNoTracking().Where(x => x.PPAGUID == PK).Where(Predicate)
                          join b in DbPPA.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.UserGUID equals b.UserGUID
                          select new
                          {
                              a.PPAReviewerListGUID,
                              StaffName = b.FirstName + " " + b.Surname,
                              UnitName = (from x in DbPPA.userServiceHistory.Where(x => x.UserGUID == a.UserGUID)
                                          join y in DbPPA.userProfiles.Where(x => x.Active) on x.ServiceHistoryGUID equals y.ServiceHistoryGUID
                                          join z in DbPPA.codeDepartmentsLanguages.Where(x => x.LanguageID == LAN && x.Active) on y.DepartmentGUID equals z.DepartmentGUID
                                          orderby y.FromDate descending
                                          select z.DepartmentDescription).FirstOrDefault(),
                              JobTitle = (from x in DbPPA.userServiceHistory.Where(x => x.UserGUID == a.UserGUID)
                                          join y in DbPPA.userProfiles.Where(x => x.Active) on x.ServiceHistoryGUID equals y.ServiceHistoryGUID
                                          join z in DbPPA.codeJobTitlesLanguages.Where(x => x.LanguageID == LAN && x.Active) on y.JobTitleGUID equals z.JobTitleGUID
                                          orderby y.FromDate descending
                                          select z.JobTitleDescription).FirstOrDefault(),
                              DutyStation = (from x in DbPPA.userServiceHistory.Where(x => x.UserGUID == a.UserGUID)
                                             join y in DbPPA.userProfiles.Where(x => x.Active) on x.ServiceHistoryGUID equals y.ServiceHistoryGUID
                                             join z in DbPPA.codeDutyStationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on y.DutyStationGUID equals z.DutyStationGUID
                                             orderby y.FromDate descending
                                             select z.DutyStationDescription).FirstOrDefault(),
                              WorkOnFileStatus = "N/A",
                              a.Active,
                              a.PPAReviewerListRowVersion
                          });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result.ToList()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult PPAReviewerListCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.PPAManagement.Create, Apps.PPA))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/PPA/Views/PPAReviewerList/_PPAReviewerListUpdateModel.cshtml",
                new PPAReviewerListUpdateModel { PPAGUID = FK });
        }

        [HttpPost]
        public ActionResult PPAReviewerListCreate(PPAReviewerListUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.PPAManagement.Create, Apps.PPA))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var ppaRecordCreatedByUserProfileGUID = (from a in DbPPA.ProjectPartnershipAgreement.Where(x => x.Active && x.PPAGUID == model.PPAGUID) select a.CreatedByUserProfileGUID).FirstOrDefault();
            if (ppaRecordCreatedByUserProfileGUID != UserProfileGUID)
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<PPAReviewerList> ppaReviewerList = new List<PPAReviewerList>();
            foreach (var userGUID in model.SelectedUsers)
            {
                bool alreadyAdded = (from a in DbPPA.PPAReviewerList
                                     where a.PPAGUID == model.PPAGUID
                                     && a.UserGUID == userGUID
                                     && a.Active
                                     select a).Count() > 0;

                if (alreadyAdded) continue;

                ppaReviewerList.Add(new PPAReviewerList
                {
                    PPAGUID = model.PPAGUID,
                    UserGUID = userGUID,
                });
            }
            DbPPA.CreateBulk(ppaReviewerList, Permissions.PPAManagement.CreateGuid, DateTime.Now, DbCMS);
            try
            {
                DbPPA.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPPA.SingleUpdateMessage(DataTableNames.PPAReviewerListDataTable, null, null));

            }
            catch (Exception ex)
            {
                return Json(DbPPA.ErrorMessage(ex.Message));
            }
        }
        #endregion

        #region Shared
        public ActionResult GetDepartmentsFocalPoints(Guid[] DutyStationGUID, Guid[] SiteCategoryGUID, Guid DepartmentGUID)
        {
            Guid _OperationGUID = Guid.Parse("699287E8-754D-4A63-B8DD-5344CFBAFD22");
            Guid _OrganizationInstanceGUID = Guid.Parse("E156C022-EC72-4A5A-BE09-163BD85C68EF");


            Guid DepartmentConfigurationGUID = (from a in DbPPA.codeDepartmentsConfigurations.Where(x => x.Active)
                                                where a.DepartmentGUID == DepartmentGUID && a.OrganizationInstanceGUID == _OrganizationInstanceGUID
                                                select a.DepartmentConfigurationGUID).FirstOrDefault();

            var UsersList = (from a in DbPPA.configFocalPoint.Where(x => x.ApplicationGUID == Apps.PPA && x.Active)
                             join b in DbPPA.configFocalPointStaff.Where(x => x.Active) on a.FocalPointGUID equals b.FocalPointGUID
                             join c in DbPPA.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on b.UserGUID equals c.UserGUID
                             join d in DbPPA.userServiceHistory.Where(x => x.Active) on b.UserGUID equals d.UserGUID
                             where a.DepartmentConfigurationGUID == DepartmentConfigurationGUID
                             && DutyStationGUID.Contains(a.DutyStationGUID)
                             && SiteCategoryGUID.Contains(b.OfficeSiteCategoryGUID.Value)

                             orderby c.FirstName
                             select new CheckBoxList
                             {
                                 Value = b.UserGUID.ToString(),
                                 Description = c.FirstName + " " + c.Surname,
                                 SearchKey = c.FirstName
                             }).ToList();

            ConfigurationModel ConfigurationModel = new ConfigurationModel
            {
                ValueGuid = _OperationGUID,
                CheckBoxList = UsersList,
            };

            return PartialView("~/Areas/PPA/Views/PPADistributionList/_PPADepartmentsFocalPoints.cshtml", ConfigurationModel);
        }
        #endregion

    }
}