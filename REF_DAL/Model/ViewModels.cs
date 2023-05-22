using Portal_BL.Library;
using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppsPortal.REF.ViewModels
{

   public class ReferralsDataTableModel
    {
        public System.Guid ReferralGUID { get; set; }
        public string ApplicationGUID { get; set; }
        public string ApplicationDescription { get; set; }
        public string ReferralDescription { get; set; }
        public bool Active { get; set; }

        public byte[] configReferralRowVersion { get; set; }
    }

    public class ReferralUpdateModel {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ReferralGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ReferralGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ApplicationGUID { get; set; }

        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ReferralDescription", ResourceType = typeof(resxDbFields))]
        public string ReferralDescription { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PageURL", ResourceType = typeof(resxDbFields))]
        public string PageURL { get; set; }

        [Display(Name = "ReturnToReferralStepGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ReturnToReferralStepGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] configReferralRowVersion { get; set; }
        public byte[] configReferralLanguageRowVersion { get; set; }
    }

    public class ReferralStepsDataTableModel
    {
        public System.Guid ReferralStepGUID { get; set; }
        public string ReferralGUID { get; set; }
        public string ApplicationGUID { get; set; }
        public string ReferralDescription { get; set; }
        public int StepSequence { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public byte[] configReferralStepRowVersion { get; set; }
        public string ApplicationDescription { get; set; }
    }

    public class ReferralStepUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ReferralStepGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ReferralStepGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ReferralGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ReferralGUID { get; set; }

        [Display(Name = "DependOnReferralStepGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DependOnReferralStepGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "StepSequence", ResourceType = typeof(resxDbFields))]
        public int StepSequence { get; set; }

        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Description", ResourceType = typeof(resxDbFields))]
        public string Description { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] configReferralStepRowVersion { get; set; }
        public byte[] configReferralStepLanguageRowVersion { get; set; }
        
    }

    public class ReferralStatusDataTableModel
    {
        public System.Guid ReferralStatusGUID { get; set; }
        public string Description { get; set; }
        public Guid ApplicationGUID { get; set; }
        public string ApplicationDescription { get; set; }
        public Nullable<bool> Active { get; set; }
        public byte[] codeReferralStatusRowVersion { get; set; }
    }


    public class ReferralStatusUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ReferralStatusGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ReferralStatusGUID { get; set; }
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Description", ResourceType = typeof(resxDbFields))]
        public string Description { get; set; }
        public System.Guid ApplicationGUID { get; set; }

        public string Value { get; set; }
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }
        public byte[] codeReferralStatusRowVersion { get; set; }
        public byte[] codeReferralStatusLanguageRowVersion { get; set; }
    }

    public class FocalPointsDataTableModel
    {
        public System.Guid FocalPointGUID { get; set; }
        public string DepartmentGUID { get; set; }
        public string DepartmentDescription { get; set; }
        public string ApplicationGUID { get; set; }
        public string ApplicationDescription { get; set; }
        public string DutyStationGUID { get; set; }
        public string DutyStationDescription { get; set; }
        public bool Active { get; set; }
        public byte[] configFocalPointRowVersion { get; set; }
    }

    public class ReferralStepUserDataTable
    {
        public System.Guid ReferralStepUserGUID { get; set; }
        public string FullName { get; set; }
        public DateTime? ActiveUntil { get; set; }
        public bool Active { get; set; }
        public byte[] configReferralStepUserRowVersion { get; set; }
    }

    public class FocalPointStaffDataTable
    {
        public System.Guid FocalPointStaffGUID { get; set; }
        public string FullName { get; set; }
        public DateTime? ActiveUntil { get; set; }
        public bool Active { get; set; }
        public byte[] configFocalPointStaffRowVersion { get; set; }
    }

    public class ReferralHistoryUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ReferralHistoryGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ReferralHistoryGUID { get; set; }

        [Display(Name = "ReferralStepGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ReferralStepGUID { get; set; }

        [Display(Name = "ReferralStatusGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ReferralStatusGUID { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> CreateDate { get; set; }

        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> OrganizationInstanceGUID { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DutyStationGUID { get; set; }

        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DepartmentGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ReferredByUserGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ReferredByUserGUID { get; set; }
        
        [Display(Name = "Comment", ResourceType = typeof(resxDbFields))]
        public string Comment { get; set; }
        [Display(Name = "Comment", ResourceType = typeof(resxDbFields))]
        public string CommentHistory { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataReferralHistoryRowVersion { get; set; }
    }
}

