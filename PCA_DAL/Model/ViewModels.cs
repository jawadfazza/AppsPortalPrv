using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PCA_DAL.Model
{

    public class PartnersCapacityAssessmentsTableModel
    {
        public System.Guid PartnersCapacityAssessmentGUID { get; set; }
        public string PartnerName { get; set; }
        public string Name_JobTitleOfPersone { get; set; }
        public string EmailAddress { get; set; }
        public Nullable<System.DateTime> IssueDate { get; set; }
        public bool PartnerConfirm { get; set; }
        public bool AgancyConfirm { get; set; }

        [Display(Name = "PartnerEvaluation", ResourceType = typeof(resxDbFields))]
        public int PartnerEvaluation { get; set; }

        [Display(Name = "AgancyEvaluation", ResourceType = typeof(resxDbFields))]
        public int AgancyEvaluation { get; set; }
        public bool Active { get; set; }
        public byte[] dataPartnersCapacityAssessmentRowVersion { get; set; }

    }
    public class PartnersCapacityAssessmentUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PartnersCapacityAssessmentGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid PartnersCapacityAssessmentGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(250, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PartnerName", ResourceType = typeof(resxDbFields))]
        public string PartnerName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(250, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Name_JobTitleOfPersone", ResourceType = typeof(resxDbFields))]
        public string Name_JobTitleOfPersone { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.EmailAddress)]
        [StringLength(250, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EmailAddress", ResourceType = typeof(resxDbFields))]
        public string EmailAddress { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [Display(Name = "PartnerConfirm", ResourceType = typeof(resxDbFields))]
        public bool PartnerConfirm { get; set; }

        [Display(Name = "AgancyConfirm", ResourceType = typeof(resxDbFields))]
        public bool AgancyConfirm { get; set; }
        public byte[] dataPartnersCapacityAssessmentRowVersion { get; set; }

        public  List<PartnersCapacityAssessmentPartnershipAgency> PartnersCapacityAssessmentPartnershipAgency { get; set; }

    }
    public class PartnersCapacityAssessmentPartnershipAgency
    {

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OrganizationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid OrganizationGUID { get; set; }

        [Display(Name = "OrganizationShortName", ResourceType = typeof(resxDbFields))]
        public string OrganizationShortName { get; set; }

        [Display(Name = "Checked", ResourceType = typeof(resxDbFields))]
        public bool Checked { get; set; }

    }
    public class PartnersCapacityAssessmentDocUpdateModel
    {
        [Display(Name = "PartnersCapacityAssessmentDocGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid PartnersCapacityAssessmentDocGUID { get; set; }

        [Display(Name = "AssessmentTitle", ResourceType = typeof(resxDbFields))]
        public string AssessmentTitle { get; set; }

        [Display(Name = "AssessmentDescription", ResourceType = typeof(resxDbFields))]
        public string AssessmentDescription { get; set; }

        [Display(Name = "Sort", ResourceType = typeof(resxDbFields))]
        public int Sort { get; set; }

        public int index { get; set; }

        [Display(Name = "PartnerEvaluation", ResourceType = typeof(resxDbFields))]
        public Nullable<bool> PartnerEvaluation { get; set; }

        [Display(Name = "AgancyEvaluation", ResourceType = typeof(resxDbFields))]
        public Nullable<bool> AgancyEvaluation { get; set; }

       
        [Display(Name = "Comment", ResourceType = typeof(resxDbFields))]
        public string Comment { get; set; }

        public bool ContainSupportDoc { get; set; }

        [Display(Name = "PartnerConfirm", ResourceType = typeof(resxDbFields))]
        public bool PartnerConfirm { get; set; }

        [Display(Name = "AgancyConfirm", ResourceType = typeof(resxDbFields))]
        public bool AgancyConfirm { get; set; }
        public  List<PartnersCapacityAssessmentDocTitleUpdateModel> partnersCapacityAssessmentDocTitle { get; set; }
    
    
    }

    public partial class PartnersCapacityAssessmentDocTitleUpdateModel
    {

        [Display(Name = "PartnersCapacityAssessmentDocTitleGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid PartnersCapacityAssessmentDocTitleGUID { get; set; }

        [Display(Name = "PartnersCapacityAssessmentDocGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid PartnersCapacityAssessmentDocGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SupportDocTitle", ResourceType = typeof(resxDbFields))]
        public string SupportDocTitle { get; set; }

        [Display(Name = "Sort", ResourceType = typeof(resxDbFields))]
        public Nullable<int> Sort { get; set; }

        [Display(Name = "Checked", ResourceType = typeof(resxDbFields))]
        public bool Checked { get; set; }

        public HttpPostedFileBase SupportFile { get; set; }

    }

    public class PartnerConfirmUpdateModel
    {
        [Display(Name = "PartnersCapacityAssessmentGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid PartnersCapacityAssessmentGUID { get; set; }

        [Display(Name = "AssessmentTitle", ResourceType = typeof(resxDbFields))]
        public string AssessmentTitle { get; set; }

        [Display(Name = "PartnerEvaluation", ResourceType = typeof(resxDbFields))]
        public Nullable<bool> PartnerEvaluation { get; set; }

        [Display(Name = "AgancyEvaluation", ResourceType = typeof(resxDbFields))]
        public Nullable<bool> AgancyEvaluation { get; set; }

        [Display(Name = "Comment", ResourceType = typeof(resxDbFields))]
        public string Comment { get; set; }

        public bool ContainSupportDoc { get; set; }

        [Display(Name = "PartnerConfirm", ResourceType = typeof(resxDbFields))]
        public bool PartnerConfirm { get; set; }

        [Display(Name = "AgancyConfirm", ResourceType = typeof(resxDbFields))]
        public bool AgancyConfirm { get; set; }
    }
}
