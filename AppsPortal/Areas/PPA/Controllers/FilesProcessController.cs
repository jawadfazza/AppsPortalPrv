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
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.PPA.Controllers
{
    public class FilesProcessController : PPABaseController
    {

        [Route("PPA/FilesProcess")]
        public ActionResult Index()
        {
            //if (!CMS.HasAction(Permissions.PPAManagement.Access, Apps.PPA))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            return View("~/Areas/PPA/Views/FilesProcess/Index.cshtml");
        }

        [Route("PPA/FilesProcess/PPAForUserDataTable/")]
        public JsonResult PPAForUserDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<PPADataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<PPADataTableModel>(DataTable.Filters);
            }
            if(CMS.HasAction(Permissions.PPAManagement.FullAccess, Apps.PPA))
            {
                var All = (from a in DbPPA.ProjectPartnershipAgreement.AsExpandable()
                           join b in DbPPA.userProfiles.Where(x => x.Active) on a.CreatedByUserProfileGUID equals b.UserProfileGUID
                           join c in DbPPA.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on b.userServiceHistory.UserGUID equals c.UserGUID
                           join d in DbPPA.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.PPATypeGUID equals d.ValueGUID
                           join e in DbPPA.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.PPAStatusGUID equals e.ValueGUID
                           join f in DbPPA.PPADistributionList on a.PPAGUID equals f.PPAGUID
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
                           }).Where(Predicate).Distinct();
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
                           join f in DbPPA.PPADistributionList.Where(x => x.Active && x.UserGUID == UserGUID) on a.PPAGUID equals f.PPAGUID
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
                           }).Where(Predicate).Distinct();
                All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

                List<PPADataTableModel> Result = Mapper.Map<List<PPADataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

                return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
            }

          
        }

        [Route("PPA/FilesProcess/Update/{PK}")]
        public ActionResult PPAForUserUpdate(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.PPAManagement.Create, Apps.PPA))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            if(CMS.HasAction(Permissions.PPAManagement.FullAccess, Apps.PPA) || CMS.HasAction(Permissions.PPAManagement.FullReadWrite, Apps.PPA))
            {
                var model = (from a in DbPPA.ProjectPartnershipAgreement.Where(x => x.PPAGUID == PK)
                             join b in DbPPA.PPADistributionList.Where(x => x.Active) on a.PPAGUID equals b.PPAGUID
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
                                 ReminderInterval = a.ReminderInterval,
                                 Active = a.Active,
                                 ProjectPartnershipAgreementRowVersion = a.ProjectPartnershipAgreementRowVersion,
                                 PPAUserAccessType = b.PPAUserAccessType,
                             }).FirstOrDefault();
                if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("PPA/FilesProcess", "FilesProcess", new { Area = "PPA" }));

                return View("~/Areas/PPA/Views/FilesProcess/PPAFiles.cshtml", model);
            }
            else
            {
                var model = (from a in DbPPA.ProjectPartnershipAgreement.Where(x => x.PPAGUID == PK)
                             join b in DbPPA.PPADistributionList.Where(x => x.Active) on a.PPAGUID equals b.PPAGUID
                             where b.UserGUID == UserGUID
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
                                 ReminderInterval = a.ReminderInterval,
                                 Active = a.Active,
                                 ProjectPartnershipAgreementRowVersion = a.ProjectPartnershipAgreementRowVersion,
                                 PPAUserAccessType = b.PPAUserAccessType,
                             }).FirstOrDefault();
                if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("PPA/FilesProcess", "FilesProcess", new { Area = "PPA" }));

                return View("~/Areas/PPA/Views/FilesProcess/PPAFiles.cshtml", model);
            }
        }

        public ActionResult PPAUserFilesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/PPA/Views/FilesProcess/_PPAFilesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<PPAOriginalFile, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<PPAOriginalFile>(DataTable.Filters);
            }

            var Result = (from a in DbPPA.PPAOriginalFile.AsNoTracking().AsExpandable().Where(x => x.PPAGUID == PK && x.Active).Where(Predicate)
                          join b in DbPPA.userProfiles.AsNoTracking().AsExpandable() on a.UploadByUserGUID equals b.UserProfileGUID
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
                          }).Distinct();


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result.ToList()), JsonRequestBehavior.AllowGet);
        }

        [Route("PPA/FilesProcess/Download/{PK}")]
        public FileResult PPAFileDownload(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.PPAManagement.Create, Apps.PPA))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            Guid PPAOriginalFileGUID = PK;
            PPAOriginalFile ppaOriginalFile = (from a in DbPPA.PPAOriginalFile.Where(x => x.PPAOriginalFileGUID == PPAOriginalFileGUID) select a).FirstOrDefault();
            string FolderPath = Server.MapPath("~/Uploads/PPA/" + ppaOriginalFile.PPAGUID.ToString());
            string FileName = ppaOriginalFile.FileName;
            string FileExtension = ppaOriginalFile.HiddenFileExtension;
            string DownloadURL = FolderPath + "\\" + FileName + "." + FileExtension;

            byte[] fileBytes = System.IO.File.ReadAllBytes(@"" + DownloadURL + "");
            string fileName = FileName + "." + FileExtension;

            //int LatestFileVersion = 0;
            //try { LatestFileVersion = (from a in DbPPA.PPAFileVersion.Where(x => x.PPAOriginalFileGUID == PPAOriginalFileGUID && x.FileActionByUserGUID == UserGUID) select a.FileVersion).Max(); } catch { }
            Guid EntityPK = Guid.NewGuid();
            DateTime ExecutionDate = DateTime.Now;
            PPAFileVersion ppaFileVersion = new PPAFileVersion();
            ppaFileVersion.PPAFileVersionGUID = EntityPK;
            ppaFileVersion.PPAOriginalFileGUID = ppaOriginalFile.PPAOriginalFileGUID;
            ppaFileVersion.FileActionType = "Download";
            ppaFileVersion.FileActionDate = ExecutionDate;
            ppaFileVersion.FileActionByUserGUID = UserGUID;
            ppaFileVersion.CurrentFileVersionStatusGUID = FileVersionStatuses.Downloaded;
            ppaFileVersion.FileVersion = -1;
            DbPPA.Create(ppaFileVersion, Permissions.PPAManagement.CreateGuid, ExecutionDate, DbCMS);

            PPAFileVersionStatusHistory ppaFileVersionStatusHistory = new PPAFileVersionStatusHistory();
            ppaFileVersionStatusHistory.PPAFileVersionStatusHistoryGUID = Guid.NewGuid();
            ppaFileVersionStatusHistory.PPAFileVersionGUID = EntityPK;
            ppaFileVersionStatusHistory.FileVersionStatusGUID = FileVersionStatuses.Downloaded;
            ppaFileVersionStatusHistory.StatusByUserProfileGUID = UserProfileGUID;
            ppaFileVersionStatusHistory.StatusActionTakenDate = ExecutionDate;
            DbPPA.Create(ppaFileVersionStatusHistory, Permissions.PPAManagement.CreateGuid, ExecutionDate, DbCMS);


            ppaOriginalFile.StatusGUID = Guid.Parse("ECD33FE1-3AED-4E0C-ABAA-61C87983ED7F"); //Downloaded
            //insert new record in PPAFileVersion
            //change upload date to accept null
            //change download date to not accept null
            try
            {


                DbPPA.SaveChanges();
                DbCMS.SaveChanges();
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception ex)
            {
                return null;
            }


        }

        public ActionResult PPAFilesCreate(Guid FK)
        {
            //if (!CMS.HasAction(Permissions.PPAManagement.Create, Apps.PPA))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            return PartialView("~/Areas/PPA/Views/FilesProcess/_PPAFilesUpdateModal.cshtml",
                new PPAFileVersionUpdateModel { PPAGUID = FK });
        }

        [HttpPost]
        public FineUploaderResult UploadPPAFiles(FineUpload upload, Guid PPAOriginalFileGUID, Guid FileVersionStatusGUID, string Comments)
        {
            Guid PPAGUID = (from a in DbPPA.PPAOriginalFile.Where(x => x.PPAOriginalFileGUID == PPAOriginalFileGUID) select a.PPAGUID).FirstOrDefault();

            var _stearm = upload.InputStream;
            string _ext = Path.GetExtension(upload.FileName).Split('.')[1];
            string FolderPath = Server.MapPath("~/Uploads/PPA/" + PPAGUID.ToString());
            Directory.CreateDirectory(FolderPath);
            int LatestFileVersion = 0;
            try { LatestFileVersion = (from a in DbPPA.PPAFileVersion.Where(x => x.PPAOriginalFileGUID == PPAOriginalFileGUID && x.FileActionByUserGUID == UserGUID) select a.FileVersion).Max(); } catch { }
            if (LatestFileVersion == -1) LatestFileVersion = 0;

            DateTime ExecutionDate = DateTime.Now;
            Guid EntityPK = Guid.NewGuid();

            string FilePath = FolderPath + "/" + EntityPK.ToString() + "." + _ext;

            using (var fileStream = System.IO.File.Create(FilePath))
            {
                upload.InputStream.Seek(0, SeekOrigin.Begin);
                upload.InputStream.CopyTo(fileStream);
            }

            //PPAFileVersion ppaFileVersion = (from a in DbPPA.PPAFileVersion.Where(x => x.Active)
            //                                 where a.PPAOriginalFileGUID == PPAOriginalFileGUID
            //                                 && a.DownloadedByUserGUID == UserGUID
            //                                 && a.FileVersion == LatestFileVersion
            //                                 select a).FirstOrDefault();
            PPAFileVersion ppaFileVersion = new PPAFileVersion();
            ppaFileVersion.PPAFileVersionGUID = EntityPK;
            ppaFileVersion.PPAOriginalFileGUID = PPAOriginalFileGUID;
            ppaFileVersion.FileActionType = "Upload";
            ppaFileVersion.FileActionDate = DateTime.Now;
            ppaFileVersion.FileActionByUserGUID = UserGUID;
            ppaFileVersion.FileVersion = LatestFileVersion + 1;
            ppaFileVersion.CurrentFileVersionStatusGUID = FileVersionStatusGUID;
            ppaFileVersion.Comments = Comments;
            DbPPA.Create(ppaFileVersion, Permissions.PPAManagement.CreateGuid, ppaFileVersion.FileActionDate, DbCMS);


            PPAFileVersionStatusHistory ppaFileVersionStatusHistory = new PPAFileVersionStatusHistory();
            ppaFileVersionStatusHistory.PPAFileVersionStatusHistoryGUID = Guid.NewGuid();
            ppaFileVersionStatusHistory.PPAFileVersionGUID = EntityPK;
            ppaFileVersionStatusHistory.FileVersionStatusGUID = FileVersionStatuses.Downloaded;
            ppaFileVersionStatusHistory.StatusByUserProfileGUID = UserProfileGUID;
            ppaFileVersionStatusHistory.StatusActionTakenDate = ExecutionDate;
            DbPPA.Create(ppaFileVersionStatusHistory, Permissions.PPAManagement.CreateGuid, ExecutionDate, DbCMS);

            PPAOriginalFile ppaOriginalFile = (from a in DbPPA.PPAOriginalFile.Where(x => x.PPAOriginalFileGUID == PPAOriginalFileGUID) select a).FirstOrDefault();
            ppaOriginalFile.StatusGUID = Guid.Parse("FA20C61B-149A-402B-BB88-24FF624F3020"); //completed

            try
            {

                DbPPA.SaveChanges();
                DbCMS.SaveChanges();
                return new FineUploaderResult(true, new { path = FilePath, success = true });
            }
            catch (Exception ex)
            {
                return new FineUploaderResult(true, new { success = "error: " + ex.Message });
            }
        }

    }
}