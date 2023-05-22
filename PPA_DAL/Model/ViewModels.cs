using Portal_BL.Library;
using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPA_DAL.Model
{
    public class PPADataTableModel
    {
        public Guid PPAGUID { get; set; }
        //public Guid PPATypeGUID { get; set; }
        public string PPATypeDescription { get; set; }
        public string PPAStatusDescription { get; set; }
        //public Guid PPAStatusGUID { get; set; }
        public string PPADescription { get; set; }
        public DateTime PPADeadLine { get; set; }
        public string StaffName { get; set; }
        public string PPAImplementationArea { get; set; }
        public bool Active { get; set; }
        public byte[] ProjectPartnershipAgreementRowVersion { get; set; }

    }

    public class PPAUpdateModel
    {
        public Guid PPAGUID { get; set; }
        public Guid? RefferalGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PPATypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid PPATypeGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ImplementationAreaGUID", ResourceType = typeof(resxDbFields))]
        public Guid ImplementationAreaGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PPAStatusGUID", ResourceType = typeof(resxDbFields))]
        public Guid PPAStatusGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PPADescription", ResourceType = typeof(resxDbFields))]
        public string PPADescription { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PPADeadLine", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        public DateTime? PPADeadLine { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ReminderInterval", ResourceType = typeof(resxDbFields))]
        public int ReminderInterval { get; set; }
        public string PPAFolderPath { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ImplementingPartner", ResourceType = typeof(resxDbFields))]
        public Guid OrganizationInstanceGUID { get; set; }
        public Guid CreatedByUserProfileGUID { get; set; }
        public Guid? PPAUserAccessType { get; set; }
        public bool Active { get; set; }
        public byte[] ProjectPartnershipAgreementRowVersion { get; set; }
    }

    public class PPADistributionListUpdateModel
    {
        public Guid PPADistributionListGUID { get; set; }
        public Guid PPAGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Guid DutyStationGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
        public Guid DepartmentGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SiteCategoryGUID", ResourceType = typeof(resxDbFields))]
        public Guid SiteCategoryGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Deadline", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        public DateTime? Deadline { get; set; }
        public List<Guid> SelectedUsers { get; set; }
        public List<Guid> CCUsers { get; set; }
        public List<Guid> CCUsersWithoutAccess { get; set; }
        public bool Active { get; set; }
    }

    public class PPAReviewFileVersionsUpdateModel
    {
        public Guid PPADistributionListGUID { get; set; }
        public Guid PPAGUID { get; set; }

        public List<PPAFileListModel> PPAFileListModel { get; set; }
    }

    public class PPAFileListModel
    {
        public Guid PPAFileVersionGUID { get; set; }
        public Guid PPAOriginalFileGUID { get; set; }
        public string OriginalFileName { get; set; }
        public int FileVersion { get; set; }
        public string CurrentFileVersionStatus { get; set; }
        public string Comments { get; set; }
        public DateTime FileActionDate { get; set; }
    }

    public class PPAReviewerListUpdateModel
    {
        public Guid PPAReviewerListGUID { get; set; }
        public Guid PPAGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Guid DutyStationGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
        public Guid DepartmentGUID { get; set; }
        public List<Guid> SelectedUsers { get; set; }
        public List<Guid> CCUsers { get; set; }
        public bool Active { get; set; }
    }

    public class PPAFileVersionUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PPAFileVersionGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid PPAFileVersionGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PPAOriginalFileGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid PPAOriginalFileGUID { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DownloadDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DownloadDate { get; set; }

        [NotMapped]
        [Display(Name = "DownloadDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> LocalDownloadDate { get { return new Portal().LocalTime(this.DownloadDate); } }

        [Display(Name = "DownloadedByUserGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DownloadedByUserGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.DateTime)]
        [Display(Name = "UploadDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime UploadDate { get; set; }

        [NotMapped]
        [Display(Name = "UploadDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> LocalUploadDate { get { return new Portal().LocalTime(this.UploadDate); } }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "UploadedByUserGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid UploadedByUserGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FileVersion", ResourceType = typeof(resxDbFields))]
        public int FileVersion { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CurrentFileVersionStatusGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid CurrentFileVersionStatusGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] PPAFileVersionRowVersion { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }

        [NotMapped]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }


        ////////////
        public System.Guid PPAGUID { get; set; }

    }

    public class PPAProgressTrackingReportModel
    {
        public Guid PPAGUID { get; set; }
        public string PPADescription { get; set; }
        public Guid PPATypeGUID { get; set; }
        public string PPATypeDescription { get; set; }
        public Guid OrganizationInstanceGUID { get; set; }
        public string OrganizationDescription { get; set; }
        public DateTime PPADeadline { get; set; }
        public Guid CreatedByUserProfileGUID { get; set; }
        public Guid UserGUID { get; set; }
        public string StaffName { get; set; }
        public Guid DepartmentGUID { get; set; }
        public string DepartmentDescription { get; set; }
        public Guid DutyStationGUID { get; set; }
        public string DutyStationDescription { get; set; }
        public int UploadedToCount { get; set; }
        public string UploadedToDepartment { get; set; }
    }


    public class ReviewTrackingReportModel
    {
        public string PPADescription { get; set; }
        public Guid? PPAGUID { get; set; }
        public Guid? PPATypeGUID { get; set; }
        public Guid? ImplementationAreaGUID { get; set; }
        public Guid? OrganizationInstanceGUID { get; set; }
        public string OrganizationDescription { get; set; }
        public DateTime PPADeadLine { get; set; }

        public IEnumerable<PPAFilesReportModel> PPAOriginalFilesLayer { get; set; }
    }

    public class PPAFilesReportModel
    {
        public Guid? PPAOriginalFileGUID { get; set; }
        public string FileName { get; set; }
        public Guid? FileType { get; set; }
        public Guid? UploadByUserGUID { get; set; }

        public IEnumerable<PPAFileUsersReportModel> PPAUserFileVersionLayer { get; set; }
    }

    public class PPAFileUsersReportModel
    {
        public string FileActionByUserName { get; set; }
        public IEnumerable<PPAFileVersion> PPAUserFilesVersionDetailsLayer { get; set; }
    }

    public class ReviewTrackingReportFilterModel
    {
        public string PPADescription { get; set; }
        public Guid? PPATypeGUID { get; set; }
        public Guid? ImplementationAreaGUID { get; set; }
        public Guid? OrganizationInstanceGUID { get; set; }

    }

    public class ProgressTrackingReportActionMoreInfoModel
    {
        public Guid PPAUserAccessTypeGUID { get; set; }
        public IEnumerable<ProgressTrackingReportActionMoreInfoDetailModel> Details { get; set; }
    }
    public class ProgressTrackingReportActionMoreInfoGroupModel
    {
        public Guid FileActionByUserGUID { get; set; }
        public string DepartmentDescription { get; set; }
        public string DutyStationDescription { get; set; }
        public string FullName { get; set; }
        public DateTime FileActionDate { get; set; }
    }
    public class ProgressTrackingReportActionMoreInfoDetailModel
    {
        public string FullName { get; set; }
        public string DutyStationDescription { get; set; }
        public string DepartmentDescription { get; set; }
        public DateTime LastActionDate { get; set; }
    }
    public class ProgressTrackingReportOriginalFilesModel
    {
        public Guid PPAOriginalFileGUID { get; set; }
        public string FileName { get; set; }
        public DateTime UploadDate { get; set; }
        public Guid UploadByUserGUID { get; set; }
        public string FullName { get; set; }
        public string DepartmentDescription { get; set; }
        public string DutyStationDescription { get; set; }
        public Guid PPAFileCategoryGUID { get; set; }
        public string PPAFileCategoryDescription { get; set; }
        public Guid PPAGUID { get; set; }

    }
    public class ProgressTrackingReportFileHistoryModel
    {
        public Guid PPAFileVersionGUID { get; set; }
        public Guid FileActionByUserGUID { get; set; }
        public string FileActionByUserFullName { get; set; }
        public string FileActionType { get; set; }
        public DateTime FileActionDate { get; set; }
        public string Comments { get; set; }
        public string UserDepartment { get; set; }
    }
}
