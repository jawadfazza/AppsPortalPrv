using OVS_DAL.Model;
using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.OVS.ViewModels
{
    public class ElectionUpdateModel
    {
        public System.Guid ElectionGUID { get; set; }
        [DataType(DataType.Date)]
        [Required(ErrorMessageResourceType = typeof(resxMessages), ErrorMessageResourceName = "RequiredStartDate")]
        [Display(Name = "ElectionStartDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessageResourceType = typeof(resxMessages), ErrorMessageResourceName = "RequiredCloseDate")]
        [Display(Name = "ElectionCloseDate", ResourceType = typeof(resxDbFields))]
        //[GreaterThanOrEqualTo("StartDate", ErrorMessageResourceType = typeof(resxMessages), ErrorMessageResourceName = "StartDateGreaterCloseDate")]
        public System.DateTime? CloseDate { get; set; }

        //[DataType(DataType.DateTime)]
        //[Required(ErrorMessageResourceType = typeof(resxMessages), ErrorMessageResourceName = "ReuiredEntry")]
        //[Display(Name = "ResultAvaiableDate", ResourceType = typeof(resxDbFields))]
        //// [GreaterThanOrEqualTo("StartDate", ErrorMessageResourceType = typeof(resxMessages), ErrorMessageResourceName = "StartDateGreaterCloseDate")]
        //public System.DateTime? ResultAvaiableDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(resxMessages), ErrorMessageResourceName = "RequiredTitle")]
        [Display(Name = "ElectionTitle", ResourceType = typeof(resxDbFields))]
        public string Title { get; set; }

        [Display(Name = "ElectionDetails", ResourceType = typeof(resxDbFields))]
        public string Details { get; set; }

        [Required(ErrorMessageResourceType = typeof(resxMessages), ErrorMessageResourceName = "RequiredTimeZone")]
        [Display(Name = "TimeZone", ResourceType = typeof(resxDbFields))]
        public string TimeZone { get; set; }

        [Required(ErrorMessageResourceType = typeof(resxMessages), ErrorMessageResourceName = "RequiredOrganizationInstanceGUID")]
        [Display(Name = "OrganizationName", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> OrganizationInstanceGUID { get; set; }
        public bool IsDisplayReviewPage { get; set; }
        public bool IsDisplayCandidateImage { get; set; }
        [Display(Name = "IsShuffling", ResourceType = typeof(resxDbFields))]
        public bool IsShuffling { get; set; }
        [Display(Name = "IsNeedVoteConfirmationMail", ResourceType = typeof(resxDbFields))]
        public bool IsNeedVoteConfirmationMail { get; set; }
        public bool IsResultShared { get; set; }



        [Required(ErrorMessageResourceType = typeof(resxMessages), ErrorMessageResourceName = "RequiredDutyStationGUID")]
        [Display(Name = "DutyStation", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DutyStationGUID { get; set; }
        public Nullable<System.Guid> ResultShareTypeGUID { get; set; }
        public bool Active { get; set; }
        public byte[] dataElectionRowVersion { get; set; }
        public byte[] dataElectionLanguageRowVersion { get; set; }
    }

    public class ElectionLanguagesUpdateModel
    {
        public Guid ElectionLanguageGUID { get; set; }
        public Guid ElectionGUID { get; set; }
        public string LanguageID { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
        public bool Active { get; set; }
    }


    public class ElectionResulteModel
    {
        public System.Guid ElectionGUID { get; set; }

        public bool Active { get; set; }
        public byte[] dataElectionRowVersion { get; set; }
        public int TotalInvitedForElection { get; set; }
        public int TotalVotedForElection { get; set; }
        public int TotalNotVotedForElection { get; set; }
        public int TotalViewedButNotVotedForElection { get; set; }
        public int TotalNoActionForElection { get; set; }

        public float PercentageVotedForElection { get; set; }
        public float PercentageNotVotedForElection { get; set; }
        public float PercentageViewedButNotVotedForElection { get; set; }
        public float PercentageNoActionForElection { get; set; }
    }

    public class ElectionDataTableModel
    {
        public Guid ElectionGUID { get; set; }

        public string Delails { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CloseDate { get; set; }
        public DateTime? ResultAvaiableDate { get; set; }
        public string Title { get; set; }
        public string OrganizationInstanceGUID { get; set; }
        public string OrganizationInstance { get; set; }

        public string DutyStationGUID { get; set; }
        public string DutyStation { get; set; }
        public string TimeZone { get; set; }
        public bool Active { get; set; }
        public int EligibleVoters { get; set; }
        public int VoteCount { get; set; }
        public byte[] dataElectionRowVersion { get; set; }
    }

    public class ElectionResultDataTableModel
    {
        public Guid? ElectionCandidateGUID { get; set; }
        //public Guid ElectionGUID { get; set; }
        //public bool Active { get; set; }
        public String FullName { get; set; }
        public String CandidateGender { get; set; }
        public int TotalVotes { get; set; }
        public string VoteRate { get; set; }
    }


    [ModelBinder(typeof(ModelBinder))]
    public class ElectionCandidateModel
    {
        public Guid ElectionCandidateGUID { get; set; }
        public Guid ElectionGUID { get; set; }
        public Guid GenderGUID { get; set; }
        //public bool Active { get; set; }
        public string FullName { get; set; }
        public string CampaignPlan { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        [Display(Name = "EmailAddress", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        public bool IsVerified { get; set; }
        public string CandidatePhoto { get; set; }

        public string FileName { get; set; }
        public Stream InputStream { get; set; }
        public void SaveAs(string destination, bool overwrite = false, bool autoCreateDirectory = true)
        {
            if (autoCreateDirectory)
            {
                var directory = new FileInfo(destination).Directory;
                if (directory != null) directory.Create();
            }

            using (var file = new FileStream(destination, overwrite ? FileMode.Create : FileMode.CreateNew))
                InputStream.CopyTo(file);
        }

        public class ModelBinder : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var request = controllerContext.RequestContext.HttpContext.Request;
                var formUpload = request.Files.Count > 0;

                // find filename
                var xFileName = request.Headers["X-File-Name"];
                var qqFile = request["qqfile"];
                var formFilename = formUpload ? request.Files[0].FileName : null;


                var upload = new ElectionCandidateModel
                {
                    FileName = xFileName ?? qqFile ?? formFilename,
                    InputStream = formUpload ? request.Files[0].InputStream : null,
                    FullName = request.Form["FullName"],
                    EmailAddress = request.Form["EmailAddress"],
                    CampaignPlan = request.Form["CampaignPlan"],
                    GenderGUID = Guid.Parse(request.Form["GenderGUID"]),
                    ElectionGUID = Guid.Parse(request.Form["ElectionGUID"]),
                    ElectionCandidateGUID = String.IsNullOrEmpty(request.Form["ElectionCandidateGUID"]) ?
                        Guid.Empty : Guid.Parse(request.Form["ElectionCandidateGUID"])
                };

                return upload;
            }
        }
    }
    public class ConditionTypeUpdateModel
    {
        public System.Guid ConditionTypeGUID { get; set; }
        public string Description { get; set; }
        public System.Guid DataTypeGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeConditionTypeRowVersion { get; set; }
        public byte[] codeConditionTypeLanguageRowVersion { get; set; }
    }

    public class ElectionStaffModel
    {
        public System.Guid ElectionGUID { get; set; }
        public string FullName { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        [Display(Name = "EmailAddress", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        public byte[] dataElectionStaffRowVersion { get; set; }

        public Guid ElectionStaffGUID { get; set; }
        public HttpPostedFileBase file { get; set; }
        public string FileImportStaffWarningMessage { get; set; }
    }

    public class IntervalTypeUpdateModel
    {
        public System.Guid IntervalTypeGUID { get; set; }
        public string Description { get; set; }
        public System.Guid DataTypeGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeIntervalTypesRowVersion { get; set; }
        public byte[] codeIntervalTypeLanguagesRowVersion { get; set; }
    }
    public class ConditionTypesDataTableModel
    {
        public System.Guid ConditionTypeGUID { get; set; }
        public System.Guid DataTypeGUID { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public byte[] codeConditionTypesRowVersion { get; set; }

    }
    public class StaffVoteElectionModel
    {
        public StaffVoteElectionModel()
        {
            this.electionCandidateModel = new List<ElectionCandidateModel>();
        }
        public System.Guid ElectionGUID { get; set; }
        public string ElectionTitle { get; set; }
        public bool IsShuffling { get; set; }
        public bool IsDisplayCandidateImage { get; set; }

        public dataElectionStaff dataElectionStaffs { get; set; }
        public List<ElectionCandidateModel> electionCandidateModel = new List<ElectionCandidateModel>();

    }

    public class VoteCheckerModel
    {
        public Guid ElectionGUID { get; set; }
        public string AccessKey { get; set; }
        public Guid? ElectionStaffGUID { get; set; }
        public string ElectionTitle { get; set; }
        public int? totalCandidates { get; set; }
        public bool ActiveElection { get; set; }
        public string MessageTitle { get; set; }
        public string MessageBody { get; set; }
        public bool status { get; set; }
    }

    public class ElectionStaffImportModel
    {
        public string FullName { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        [Display(Name = "EmailAddress", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        public bool Status { get; set; }
    }

    public class ElectionCorrespondenceModel
    {
        public Guid ElectionGUID { get; set; }
        public Guid ElectionCorrespondenceGUID { get; set; }

        public string MessageTitle { get; set; }
        public string MessageBody { get; set; }
        public DateTime SendDate { get; set; }
        public Guid ElectionCorrespondenceTypeGUID { get; set; }
        public int TotalRecipients { get; set; }
        public bool Active { get; set; }
        public string ElectionCorrespondenceType { get; set; }
    }


    public partial class dataStaffModel
    {

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid StaffGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(150, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FullName", ResourceType = typeof(resxDbFields))]
        public string FullName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.EmailAddress)]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EmailAddress", ResourceType = typeof(resxDbFields))]
        public string EmailAddress { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataStaffRowVersion { get; set; }

        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> OrganizationInstanceGUID { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DutyStationGUID { get; set; }

        [Display(Name = "ElectionGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ElectionGUID { get; set; }

        [Display(Name = "ElectionStaffGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ElectionStaffGUID { get; set; }

    }


}