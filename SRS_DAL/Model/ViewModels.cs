using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRS_DAL.Model
{

    public class ApplicationRequestDataTableModel
    {
        public Guid AppRequestGUID { get; set; }
        public string RequestedAppName { get; set; }
        public DateTime AppRequestDate { get; set; }
        public Guid RequestedByProfileGUID { get; set; }
        public string RequestedBy { get; set; }
        public bool Active { get; set; }
        public byte[] dataApplicationRequestRowVersion { get; set; }
    }







    public class ApplicationRequestModel
    {
        [Display(Name = "AppRequestGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid AppRequestGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RequestedAppName", ResourceType = typeof(resxDbFields))]
        public string RequestedAppName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RequestedByProfileGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid RequestedByProfileGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.DateTime)]
        [Display(Name = "AppRequestDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime AppRequestDate { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "IsNewDbForTracking", ResourceType = typeof(resxDbFields))]
        public bool IsNewDbForTracking { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "IsToReplaceOldApp", ResourceType = typeof(resxDbFields))]
        public bool IsToReplaceOldApp { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "IsToAutomateManual", ResourceType = typeof(resxDbFields))]
        public bool IsToAutomateManual { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OtherPurposForApp", ResourceType = typeof(resxDbFields))]
        public string OtherPurposForApp { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "AppDescription", ResourceType = typeof(resxDbFields))]
        public string AppDescription { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "AppBenefits", ResourceType = typeof(resxDbFields))]
        public string AppBenefits { get; set; }

        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataApplicationRequestRowVersion { get; set; }


        [Display(Name = "AppRequestAuthorizationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid? AppRequestAuthorizationGUID { get; set; }


        [Display(Name = "ApprovedByProfileGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid? ApprovedByProfileGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ApprovalStatusGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid? ApprovalStatusGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ApprovalComments", ResourceType = typeof(resxDbFields))]
        public string ApprovalComments { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EstimatedResourcesNum", ResourceType = typeof(resxDbFields))]
        public int? EstimatedResourcesNum { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EstimatedTime", ResourceType = typeof(resxDbFields))]
        public System.Guid? EstimatedTime { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CriticalityGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid? CriticalityGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.Date)]
        [Display(Name = "ApplicationStartDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime? ApplicationStartDate { get; set; }


        [DataType(DataType.Date)]
        [Display(Name = "ApplicationEndDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime? ApplicationEndDate { get; set; }


        [Display(Name = "AssignedToGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid? AssignedToGUID { get; set; }

        public byte[] dataAppRequestAuthorizationRowVersion { get; set; }


    }



    public class ApplicationEnhancementsDataTableModel
    {
        public Guid AppEnhancementGUID { get; set; }
        public string ApplicationDescription { get; set; }
        public Guid EnhancementRequestedByProfileGUID { get; set; }
        public string RequestedBy { get; set; }
        public bool Active { get; set; }
        public byte[] dataAppEnhancementRequestRowVersion { get; set; }
    }

    public class ApplicationEnhancementRequestModel
    {
        public Guid AppEnhancementGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ApplicationGUID", ResourceType = typeof(resxDbFields))]
        public Guid ApplicationGUID { get; set; }
        public Guid EnhancementRequestedByProfileGUID { get; set; }
        public DateTime EnhancementRequestDate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EnhancementDetails", ResourceType = typeof(resxDbFields))]
        public string EnhancementDetails { get; set; }
        public bool Active { get; set; }
        public byte[] dataAppEnhancementRequestRowVersion { get; set; }


        //Approval Region

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ApprovedByProfileGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ApprovedByProfileGUID { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ApprovalStatus", ResourceType = typeof(resxDbFields))]
        public Guid? ApprovalStatus { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ApprovalComments", ResourceType = typeof(resxDbFields))]
        public string ApprovalComments { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EnhancementAssignedTo", ResourceType = typeof(resxDbFields))]
        public int EnhancementAssignedTo { get; set; }

        [Display(Name = "EnhancementCriticalityGUID", ResourceType = typeof(resxDbFields))]
        public Guid? EnhancementCriticalityGUID { get; set; }

        [Display(Name = "EstimatedResourcesNum", ResourceType = typeof(resxDbFields))]
        public int EstimatedResourcesNum { get; set; }

        [Display(Name = "EstimatedTime", ResourceType = typeof(resxDbFields))]
        public Guid? EstimatedTime { get; set; }

        [Display(Name = "EnhancementStartDate", ResourceType = typeof(resxDbFields))]
        public DateTime? EnhancementStartDate { get; set; }

        [Display(Name = "EnhancementEndDate", ResourceType = typeof(resxDbFields))]
        public DateTime? EnhancementEndDate { get; set; }
    }


    public class BugReportsDataTableModel
    {
        public Guid AppBugReportGUID { get; set; }
        public string ApplicationDescription { get; set; }
        public Guid BugReportByProfileGUID { get; set; }
        public bool Active { get; set; }
        public byte[] dataAppBugReportRowVersion { get; set; }
    }


    public class BugReportModel
    {
        public Guid AppBugReportGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ApplicationGUID", ResourceType = typeof(resxDbFields))]
        public Guid ApplicationGUID { get; set; }

        public Guid BugReportByProfileGUID { get; set; }

        public DateTime BugReporttDate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BugDetails", ResourceType = typeof(resxDbFields))]
        public string BugDetails { get; set; }

        [Display(Name = "BugStatus", ResourceType = typeof(resxDbFields))]
        public Guid? BugStatus { get; set; }

        public bool Active { get; set; }

        public byte[] dataAppBugReportRowVersion { get; set; }

        [Display(Name = "SolvedBugComments", ResourceType = typeof(resxDbFields))]
        public string SolvedBugComments { get; set; }

        [Display(Name = "BugFixDate", ResourceType = typeof(resxDbFields))]
        public DateTime? BugFixDate { get; set; }

        [Display(Name = "BugAssignedTo", ResourceType = typeof(resxDbFields))]
        public Guid? BugAssignedTo { get; set; }

        [Display(Name = "BugCriticalityGUID", ResourceType = typeof(resxDbFields))]
        public Guid? BugCriticalityGUID { get; set; }

        [Display(Name = "BugFixStartDate", ResourceType = typeof(resxDbFields))]
        public Guid? BugFixStartDate { get; set; }

        [Display(Name = "BugFixEndDate", ResourceType = typeof(resxDbFields))]
        public Guid? BugFixEndDate { get; set; }
    }




    public class HelpDeskDataTableModel
    {
        public Guid HelpDeskGUID { get; set; }
        public int RequestNumber { get; set; }
        public Guid RequestedByProfileGUID { get; set; }
        public Guid RequestorDutyStationGUID { get; set; }
        public string RequestorDutyStationDescription { get; set; }
        public Guid RequestorDepartmentGUID { get; set; }
        public string RequestorDepartmentDescription { get; set; }
        public string RequestedByFullName { get; set; }
        public Guid ConfigItemGUID { get; set; }
        public string ConfigItemDescription { get; set; }
        public Guid CriticalityGUID { get; set; }
        public string CriticalityDescription { get; set; }
        public Guid WorkGroupGUID { get; set; }
        public string WorkGroupDescription { get; set; }
        public Guid RequestStatusGUID { get; set; }
        public string RequestStatusDescription { get; set; }
        public DateTime RequestCreateDate { get; set; }
        public DateTime? RequestUpdateDate { get; set; }
        public Guid? AssignedToGUID { get; set; }
        public string AssignedToFullName { get; set; }
        public bool Active { get; set; }
        public byte[] dataHelpDeskRowVersion { get; set; }
    }

    public class HelpDeskModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RequestedAppName", ResourceType = typeof(resxDbFields))]
        public string RequestedAppName { get; set; }

        [Display(Name = "HelpDeskGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid HelpDeskGUID { get; set; }

        [Display(Name = "RequestNumber", ResourceType = typeof(resxDbFields))]
        public int RequestNumber { get; set; }

        [Display(Name = "RequestedByProfileGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid RequestedByProfileGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ConfigItemGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ConfigItemGUID { get; set; }
        public string ConfigItemDescription { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CriticalityGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid CriticalityGUID { get; set; }
        public string CriticalityDescription { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "WorkGroupGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid WorkGroupGUID { get; set; }
        public string WorkGroupDescription { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RequestStatusGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid RequestStatusGUID { get; set; }
        public string RequestStatusDescription { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "RequestCreateDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime RequestCreateDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "RequestCreateDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime? RequestUpdateDate { get; set; }

        [Display(Name = "AssignedToGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid? AssignedToGUID { get; set; }

        [Display(Name = "AssignedToGUID", ResourceType = typeof(resxDbFields))]
        public string AssignedToFullName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataHelpDeskRowVersion { get; set; }


        public HelpDeskNewApplicationModel HelpDeskNewApplicationModel { get; set; }
        public HelpDeskNewApplicationAuthorizationModel HelpDeskNewApplicationAuthorizationModel { get; set; }



        public HelpDeskApplicationEnhancementModel HelpDeskApplicationEnhancementModel { get; set; }
        public HelpDeskApplicationEnhancementAuthorizationModel HelpDeskApplicationEnhancementAuthorizationModel { get; set; }



        public HelpDeskBugReportModel HelpDeskBugReportModel { get; set; }
        public HelpDeskBugReportAuthorizationModel HelpDeskBugReportAuthorizationModel { get; set; }


        public HelpDeskChangeRequestModel helpDeskChangeRequestModel { get; set; }
    }

    public class HelpDeskAttachementDataTable
    {
        public Guid HelpDeskAttachementGUID { get; set; }
        public Guid HelpDeskGUID { get; set; }
        public string AttachementPath { get; set; }
        public string FileName { get; set; }
        public bool Active { get; set; }
        public byte[] dataHelpDeskAttachementRowVersion { get; set; }
    }


    public class HelpDeskNewApplicationModel
    {
        [Display(Name = "AppRequestGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid AppRequestGUID { get; set; }

        [Display(Name = "HelpDeskGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid HelpDeskGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RequestedAppName", ResourceType = typeof(resxDbFields))]
        public string RequestedAppName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "IsNewDbForTracking", ResourceType = typeof(resxDbFields))]
        public bool IsNewDbForTracking { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "IsToReplaceOldApp", ResourceType = typeof(resxDbFields))]
        public bool IsToReplaceOldApp { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "IsToAutomateManual", ResourceType = typeof(resxDbFields))]
        public bool IsToAutomateManual { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PurposDescriptionBenefits", ResourceType = typeof(resxDbFields))]
        public string PurposDescriptionBenefits { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataApplicationRequestRowVersion { get; set; }

        [Display(Name = "ServerOrVM", ResourceType = typeof(resxDbFields))]
        public bool ServerOrVM { get; set; }

        [Display(Name = "CPUDescription", ResourceType = typeof(resxDbFields))]
        public string CPUDescription { get; set; }

        [Display(Name = "RAMDescription", ResourceType = typeof(resxDbFields))]
        public string RAMDescription { get; set; }

        [Display(Name = "StorageDescription", ResourceType = typeof(resxDbFields))]
        public string StorageDescription { get; set; }

        [Display(Name = "OnsiteBackup", ResourceType = typeof(resxDbFields))]
        public bool OnsiteBackup { get; set; }

        [Display(Name = "OffsiteBackup", ResourceType = typeof(resxDbFields))]
        public bool OffsiteBackup { get; set; }

        [Display(Name = "OfflineVersion", ResourceType = typeof(resxDbFields))]
        public bool OfflineVersion { get; set; }
        [Display(Name = "AndroidVersion", ResourceType = typeof(resxDbFields))]
        public bool AndroidVersion { get; set; }
        [Display(Name = "IOSVersion", ResourceType = typeof(resxDbFields))]
        public bool IOSVersion { get; set; }
    }

    public class HelpDeskNewApplicationAuthorizationModel
    {
        [Display(Name = "AppRequestAuthorizationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid AppRequestAuthorizationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "AppRequestGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid AppRequestGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ApprovedByProfileGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ApprovedByProfileGUID { get; set; }

        [Display(Name = "ApprovedByFullName", ResourceType = typeof(resxDbFields))]
        public System.String ApprovedByFullName { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ApprovalStatusGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ApprovalStatusGUID { get; set; }

        [Display(Name = "ApprovalStatusDescription", ResourceType = typeof(resxDbFields))]
        public System.String ApprovalStatusDescription { get; set; }

        [Display(Name = "ApprovalComments", ResourceType = typeof(resxDbFields))]
        public string ApprovalComments { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EstimatedResourcesNum", ResourceType = typeof(resxDbFields))]
        public int EstimatedResourcesNum { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EstimatedTime", ResourceType = typeof(resxDbFields))]
        public System.Guid EstimatedTime { get; set; }

        [Display(Name = "EstimatedTime", ResourceType = typeof(resxDbFields))]
        public string EstimatedTimeDescription { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.DateTime)]
        [Display(Name = "ApplicationStartDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime ApplicationStartDate { get; set; }


        [DataType(DataType.DateTime)]
        [Display(Name = "ApplicationEndDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> ApplicationEndDate { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataAppRequestAuthorizationRowVersion { get; set; }

    }




    public class HelpDeskApplicationEnhancementModel
    {
        [Display(Name = "AppEnhancementGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid AppEnhancementGUID { get; set; }

        [Display(Name = "HelpDeskGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid HelpDeskGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ApplicationGUID { get; set; }

        [Display(Name = "ApplicationDescription", ResourceType = typeof(resxDbFields))]
        public string ApplicationDescription { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EnhancementPurpos", ResourceType = typeof(resxDbFields))]
        public string EnhancementPurpos { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EnhancementDetails", ResourceType = typeof(resxDbFields))]
        public string EnhancementDetails { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EnhancementBenefits", ResourceType = typeof(resxDbFields))]
        public string EnhancementBenefits { get; set; }

        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataAppEnhancementRequestRowVersion { get; set; }

    }
    public class HelpDeskApplicationEnhancementAuthorizationModel
    {
        [Display(Name = "AppEnhancementAuthorizationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid AppEnhancementAuthorizationGUID { get; set; }

        [Display(Name = "AppEnhancementGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid AppEnhancementGUID { get; set; }

        [Display(Name = "ApprovedByProfileGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ApprovedByProfileGUID { get; set; }

        [Display(Name = "ApprovedByFullName", ResourceType = typeof(resxDbFields))]
        public System.String ApprovedByFullName { get; set; }

        [Display(Name = "ApprovalStatusGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ApprovalStatusGUID { get; set; }

        [Display(Name = "ApprovalStatusDescription", ResourceType = typeof(resxDbFields))]
        public System.String ApprovalStatusDescription { get; set; }

        [Display(Name = "ApprovalComments", ResourceType = typeof(resxDbFields))]
        public string ApprovalComments { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EstimatedResourcesNum", ResourceType = typeof(resxDbFields))]
        public int EstimatedResourcesNum { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EstimatedTime", ResourceType = typeof(resxDbFields))]
        public System.Guid EstimatedTime { get; set; }

        [Display(Name = "EstimatedTime", ResourceType = typeof(resxDbFields))]
        public string EstimatedTimeDescription { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.DateTime)]
        [Display(Name = "EnhancementStartDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime EnhancementStartDate { get; set; }


        [DataType(DataType.DateTime)]
        [Display(Name = "EnhancementEndDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> EnhancementEndDate { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataAppEnhancementAuthorizationRowVersion { get; set; }

    }



    public class HelpDeskBugReportModel
    {
        [Display(Name = "AppBugReportGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid AppBugReportGUID { get; set; }

        [Display(Name = "HelpDeskGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid HelpDeskGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ApplicationGUID { get; set; }

        [Display(Name = "ApplicationDescription", ResourceType = typeof(resxDbFields))]
        public string ApplicationDescription { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BugDetails", ResourceType = typeof(resxDbFields))]
        public string BugDetails { get; set; }

        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataAppBugReportRowVersion { get; set; }
    }
    public class HelpDeskBugReportAuthorizationModel
    {
        [Display(Name = "AppBugAuthorizationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid AppBugAuthorizationGUID { get; set; }

        [Display(Name = "AppBugReportGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid AppBugReportGUID { get; set; }

        [Display(Name = "ApprovedByProfileGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ApprovedByProfileGUID { get; set; }

        [Display(Name = "ApprovedByFullName", ResourceType = typeof(resxDbFields))]
        public System.String ApprovedByFullName { get; set; }

        [Display(Name = "ApprovalStatusGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ApprovalStatusGUID { get; set; }

        [Display(Name = "ApprovalStatusDescription", ResourceType = typeof(resxDbFields))]
        public string ApprovalStatusDescription { get; set; }

        [Display(Name = "ApprovalComments", ResourceType = typeof(resxDbFields))]
        public string ApprovalComments { get; set; }

        [Display(Name = "BugAnalysis", ResourceType = typeof(resxDbFields))]
        public string BugAnalysis { get; set; }

        [Display(Name = "BugResolution", ResourceType = typeof(resxDbFields))]
        public string BugResolution { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EstimatedResourcesNum", ResourceType = typeof(resxDbFields))]
        public int EstimatedResourcesNum { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EstimatedTime", ResourceType = typeof(resxDbFields))]
        public System.Guid EstimatedTime { get; set; }

        [Display(Name = "EstimatedTime", ResourceType = typeof(resxDbFields))]
        public string EstimatedTimeDescription { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.DateTime)]
        [Display(Name = "BugFixStartDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime BugFixStartDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "BugFixEndDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> BugFixEndDate { get; set; }



        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataAppBugAuthorizationRowVersion { get; set; }


    }


    public class HelpDeskChangeRequestModel
    {

        [Display(Name = "ChangeRequestGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ChangeRequestGUID { get; set; }

        [Display(Name = "HelpDeskGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid HelpDeskGUID { get; set; }

        [Display(Name = "CRIntroduction", ResourceType = typeof(resxDbFields))]
        public string CRIntroduction { get; set; }

        [Display(Name = "CROutcome", ResourceType = typeof(resxDbFields))]
        public string CROutcome { get; set; }

        [Display(Name = "CRNotes", ResourceType = typeof(resxDbFields))]
        public string CRNotes { get; set; }

        [Display(Name = "CRPrerequisites", ResourceType = typeof(resxDbFields))]
        public string CRPrerequisites { get; set; }

        [Display(Name = "CRRollbackPlan", ResourceType = typeof(resxDbFields))]
        public string CRRollbackPlan { get; set; }

        [Display(Name = "CRApprovedBy", ResourceType = typeof(resxDbFields))]
        public string CRApprovedBy { get; set; }

        [Display(Name = "CRActionPlanList", ResourceType = typeof(resxDbFields))]
        public List<HelpDeskChangeRequestActionPlanModel> CRActionPlanList { get; set; }
        public byte[] dataChangeRequestRowVersion { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }

        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }



    }
    public class HelpDeskChangeRequestActionPlanModel
    {
        [Display(Name = "ChangeRequestAPGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ChangeRequestAPGUID { get; set; }

        [Display(Name = "ChangeRequestGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ChangeRequestGUID { get; set; }

        [Display(Name = "APUnit", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> APUnit { get; set; }

        [Display(Name = "APWho", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> APWho { get; set; }

        [Display(Name = "APWhat", ResourceType = typeof(resxDbFields))]
        public string APWhat { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "APWhen", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> APWhen { get; set; }

        [Display(Name = "APHow", ResourceType = typeof(resxDbFields))]
        public string APHow { get; set; }

        [Display(Name = "APDuration", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> APDuration { get; set; }

        [Display(Name = "APComments", ResourceType = typeof(resxDbFields))]
        public string APComments { get; set; }

        [Display(Name = "APCompleted", ResourceType = typeof(resxDbFields))]
        public Nullable<bool> APCompleted { get; set; }

        public byte[] dataChangeRequestActionPlanRowVersion { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }

        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }


    }

}
