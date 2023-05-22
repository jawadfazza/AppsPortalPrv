using ExpressiveAnnotations.Attributes;
using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTP_DAL.Model
{
    public class GTPApplicationUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPApplicationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPCategoryGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPCategoryGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid UserGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.DateTime)]
        [Display(Name = "GTPApplicationDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime GTPApplicationDate { get; set; }


        [DataType(DataType.DateTime)]
        [Display(Name = "GTPApplicationExpiryDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> GTPApplicationExpiryDate { get; set; }


        [DataType(DataType.DateTime)]
        [Display(Name = "GTPApplicationEligibleAs", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> GTPApplicationEligibleAs { get; set; }


        [Display(Name = "IsReviewed", ResourceType = typeof(resxDbFields))]
        public Nullable<bool> IsReviewed { get; set; }

        [Display(Name = "ReviewedBy", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ReviewedBy { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "ReviewedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> ReviewedOn { get; set; }


        [Display(Name = "IsEligible", ResourceType = typeof(resxDbFields))]
        public Nullable<bool> IsEligible { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "UpdatedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> UpdatedOn { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataGTPApplicationRowVersion { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }

        public bool IsNew { get; set; }

    }

    #region P11 Form 
    public class GTPPersonalHistoryFormUpdateModel
    {
        public Guid GTPApplicationGUID { get; set; }
        public bool Active { get; set; }
        public byte[] dataPersonalHistoryGeneralInfoRowVersion { get; set; }

        public dataPersonalHistoryGeneralInfoUpdateModel dataPersonalHistoryGeneralInfoUpdateModel { get; set; }
        public dataPersonalHistoryPersonalInfoUpdateModel dataPersonalHistoryPersonalInfoUpdateModel { get; set; }
        public dataPersonalHistoryContactInfoUpdateModel dataPersonalHistoryContactInfoUpdateModel { get; set; }
        public dataPersonalHistoryPhoneNumberUpdateModel dataPersonalHistoryPhoneNumberUpdateModel { get; set; }
        public dataPersonalHistoryEmailAddressUpdateModel dataPersonalHistoryEmailAddressUpdateModel { get; set; }
        public dataPersonalHistoryNationalityInfoUpdateModel dataPersonalHistoryNationalityInfoUpdateModel { get; set; }
        public dataPersonalHistoryLetterOfInterestUpdateModel dataPersonalHistoryLetterOfInterestUpdateModel { get; set; }
        public List<dataPersonalHistoryWorkExperienceUpdateModel> dataPersonalHistoryWorkExperienceUpdateModel { get; set; }
        public List<dataPersonalHistorySpecializedTrainingUpdateModel> dataPersonalHistorySpecializedTrainingUpdateModel { get; set; }
        public List<dataPersonalHistoryEducationUpdateModel> dataPersonalHistoryEducationUpdateModel { get; set; }
        public List<dataPersonalHistorySkillUpdateModel> dataPersonalHistorySkillUpdateModel { get; set; }
        public dataPersonalHistoryLanguageUpdateModel dataPersonalHistoryLanguageUpdateModel { get; set; }
        public List<dataPersonalHistoryLicenceAndCertificateUpdateModel> dataPersonalHistoryLicenceAndCertificateUpdateModel { get; set; }
        public dataPersonalHistoryProfessionalReferenceUpdateModel dataPersonalHistoryProfessionalReferenceUpdateModel { get; set; }
        public dataPersonalHistoryQuestionnaireUpdateModel dataPersonalHistoryQuestionnaireUpdateModel { get; set; }
        public List<dataPersonalHistoryRelativeUpdateModel> dataPersonalHistoryRelativeUpdateModel { get; set; }
        public List<dataPersonalHistoryRelativeWorkerUpdateModel> dataPersonalHistoryRelativeWorkerUpdateModel { get; set; }
        public dataPersonalHistoryConfirmationAndConsentUpdateModel dataPersonalHistoryConfirmationAndConsentUpdateModel { get; set; }
    }

    public class dataPersonalHistoryGeneralInfoUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPPHGeneralInfoGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPPHGeneralInfoGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPApplicationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "LastName", ResourceType = typeof(resxDbFields))]
        public string LastName { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MiddleName", ResourceType = typeof(resxDbFields))]
        public string MiddleName { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MaidenName", ResourceType = typeof(resxDbFields))]
        public string MaidenName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FirstName", ResourceType = typeof(resxDbFields))]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataPersonalHistoryGeneralInfoRowVersion { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }
    }

    public class dataPersonalHistoryPersonalInfoUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPPHPersonalInfoGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPPHPersonalInfoGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPApplicationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.Date)]
        [Display(Name = "DateOfBirth", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DateOfBirth { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Gender", ResourceType = typeof(resxDbFields))]
        public string Gender { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MaritalStatus", ResourceType = typeof(resxDbFields))]
        public Guid MaritalStatus { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataPersonalHistoryPersonalInfoRowVersion { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }

    }

    public class dataPersonalHistoryContactInfoUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPPHContactInfoGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPPHContactInfoGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPApplicationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CurrentAddressLine1", ResourceType = typeof(resxDbFields))]
        public string CurrentAddressLine1 { get; set; }

        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CurrentAddressLine2", ResourceType = typeof(resxDbFields))]
        public string CurrentAddressLine2 { get; set; }

        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CurrentAddressLine3", ResourceType = typeof(resxDbFields))]
        public string CurrentAddressLine3 { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CurrentAddressCityGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid CurrentAddressCityGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CurrentAddressCountryGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid CurrentAddressCountryGUID { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CurrentAddressPostalCode", ResourceType = typeof(resxDbFields))]
        public string CurrentAddressPostalCode { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PermanentAddressLine1", ResourceType = typeof(resxDbFields))]
        public string PermanentAddressLine1 { get; set; }

        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PermanentAddressLine2", ResourceType = typeof(resxDbFields))]
        public string PermanentAddressLine2 { get; set; }

        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PermanentAddressLine3", ResourceType = typeof(resxDbFields))]
        public string PermanentAddressLine3 { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PermanentAddressCityGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid PermanentAddressCityGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PermanentAddressCountryGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid PermanentAddressCountryGUID { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PermanentAddressPostalCode", ResourceType = typeof(resxDbFields))]
        public string PermanentAddressPostalCode { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PreferredContactMethod", ResourceType = typeof(resxDbFields))]
        public int PreferredContactMethod { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataPersonalHistoryContactInfoRowVersion { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }

    }

    public class dataPersonalHistoryPhoneNumberUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPPHPhoneNumberGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPPHPhoneNumberGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPApplicationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HomePhoneCountryCode", ResourceType = typeof(resxDbFields))]
        public string HomePhoneCountryCode { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HomePhoneTelephoneNumber", ResourceType = typeof(resxDbFields))]
        public string HomePhoneTelephoneNumber { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HomePhoneTelephoneExt", ResourceType = typeof(resxDbFields))]
        public string HomePhoneTelephoneExt { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BusinessPhoneCountryCode", ResourceType = typeof(resxDbFields))]
        public string BusinessPhoneCountryCode { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BusinessPhoneTelephoneNumber", ResourceType = typeof(resxDbFields))]
        public string BusinessPhoneTelephoneNumber { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BusinessPhoneExt", ResourceType = typeof(resxDbFields))]
        public string BusinessPhoneExt { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MobilePhoneCountryCode", ResourceType = typeof(resxDbFields))]
        public string MobilePhoneCountryCode { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MobilePhoneTelephoneNumber", ResourceType = typeof(resxDbFields))]
        public string MobilePhoneTelephoneNumber { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MobilePhoneExt", ResourceType = typeof(resxDbFields))]
        public string MobilePhoneExt { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PreferedPhone", ResourceType = typeof(resxDbFields))]
        public int PreferedPhone { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataPersonalHistoryPhoneNumberRowVersion { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }

    }

    public class dataPersonalHistoryEmailAddressUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "GTPPHEmailAddressGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPPHEmailAddressGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPApplicationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.EmailAddress)]
        [StringLength(150, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HomeEmailAddress", ResourceType = typeof(resxDbFields))]
        public string HomeEmailAddress { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.EmailAddress)]
        [StringLength(150, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BusinessEmailAddress", ResourceType = typeof(resxDbFields))]
        public string BusinessEmailAddress { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "PreferedEmailAddress", ResourceType = typeof(resxDbFields))]
        public int PreferedEmailAddress { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataPersonalHistoryEmailAddressRowVersion { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }

    }

    public class dataPersonalHistoryNationalityInfoUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPPHNationalityInfoGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPPHNationalityInfoGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPApplicationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "NationalityAtBirth", ResourceType = typeof(resxDbFields))]
        public List<string> NationalityAtBirth { get; set; }
        [Display(Name = "NationalityAtBirth", ResourceType = typeof(resxDbFields))]
        public string ClientNationalityAtBirth { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CurrentNationality", ResourceType = typeof(resxDbFields))]
        public List<string> CurrentNationality { get; set; }
        [Display(Name = "CurrentNationality", ResourceType = typeof(resxDbFields))]
        public string ClientCurrentNationality { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PermanentResidency", ResourceType = typeof(resxDbFields))]
        public System.Guid PermanentResidency { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataPersonalHistoryNationalityInfoRowVersion { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }



    }

    public class dataPersonalHistoryLetterOfInterestUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPPHLetterOfInterestGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPPHLetterOfInterestGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPApplicationGUID { get; set; }

        [Display(Name = "PositionDescription", ResourceType = typeof(resxDbFields))]
        public string PositionDescription { get; set; }

        [Display(Name = "AchievementsDescription", ResourceType = typeof(resxDbFields))]
        public string AchievementsDescription { get; set; }

        [Display(Name = "SkillsDescription", ResourceType = typeof(resxDbFields))]
        public string SkillsDescription { get; set; }

        [Display(Name = "PositionAlignDescription", ResourceType = typeof(resxDbFields))]
        public string PositionAlignDescription { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataPersonalHistoryLetterOfInterestRowVersion { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }


    }

    public class dataPersonalHistoryWorkExperienceUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPPHWorkExperienceGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPPHWorkExperienceGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPApplicationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.Date)]
        [Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> StartDate { get; set; }

        [RequiredIf("OnGoingJob == true", ErrorMessage = "End date is required when Still Working is set to Yes")]
        [DataType(DataType.Date)]
        [Display(Name = "EndDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> EndDate { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FullTimeOrPartTime", ResourceType = typeof(resxDbFields))]
        public int FullTimeOrPartTime { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Employer", ResourceType = typeof(resxDbFields))]
        public string Employer { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OnGoingJob", ResourceType = typeof(resxDbFields))]
        public Nullable<bool> OnGoingJob { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "JobTitle", ResourceType = typeof(resxDbFields))]
        public string JobTitle { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SupervisorName", ResourceType = typeof(resxDbFields))]
        public string SupervisorName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TypeOfEmployment", ResourceType = typeof(resxDbFields))]
        public System.Guid TypeOfEmployment { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OtherTypeOfEmploymentDesc", ResourceType = typeof(resxDbFields))]
        public string OtherTypeOfEmploymentDesc { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TypeOfWorkersSupervised", ResourceType = typeof(resxDbFields))]
        public string TypeOfWorkersSupervised { get; set; }

        [Display(Name = "NumberOfPersonSupervised", ResourceType = typeof(resxDbFields))]
        public Nullable<int> NumberOfPersonSupervised { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.EmailAddress)]
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SupervisorEmail", ResourceType = typeof(resxDbFields))]
        public string SupervisorEmail { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SupervisorPhone", ResourceType = typeof(resxDbFields))]
        public string SupervisorPhone { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EndingPayRateAmount", ResourceType = typeof(resxDbFields))]
        public Nullable<double> EndingPayRateAmount { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(10, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Currency", ResourceType = typeof(resxDbFields))]
        public string Currency { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ReasonForLeaving", ResourceType = typeof(resxDbFields))]
        public string ReasonForLeaving { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DescriptionOfDuties", ResourceType = typeof(resxDbFields))]
        public string DescriptionOfDuties { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EmployerAddressLine1", ResourceType = typeof(resxDbFields))]
        public string EmployerAddressLine1 { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EmployerAddressLine2", ResourceType = typeof(resxDbFields))]
        public string EmployerAddressLine2 { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EmployerAddressLine3", ResourceType = typeof(resxDbFields))]
        public string EmployerAddressLine3 { get; set; }

        [Display(Name = "EmployerAddressCityGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> EmployerAddressCityGUID { get; set; }

        [Display(Name = "EmployerAddressCountryGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> EmployerAddressCountryGUID { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EmployerAddressPostalCode", ResourceType = typeof(resxDbFields))]
        public string EmployerAddressPostalCode { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TypeOfBusinessGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> TypeOfBusinessGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "IsUNExperience", ResourceType = typeof(resxDbFields))]
        public bool IsUNExperience { get; set; }

        [Display(Name = "GradeGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> GradeGUID { get; set; }

        [Display(Name = "UNIndexNumber", ResourceType = typeof(resxDbFields))]
        public Nullable<int> UNIndexNumber { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "IsUNHCRExperience", ResourceType = typeof(resxDbFields))]
        public bool IsUNHCRExperience { get; set; }

        [Display(Name = "UNHCRMsrpID", ResourceType = typeof(resxDbFields))]
        public Nullable<int> UNHCRMsrpID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ContractTypeGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ContractTypeGUID { get; set; }

        [Display(Name = "IfOtherContractTypeDesc", ResourceType = typeof(resxDbFields))]
        public string IfOtherContractTypeDesc { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataPersonalHistoryWorkExperienceRowVersion { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }

    }

    public class dataPersonalHistorySpecializedTrainingUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPPHSpecializedTrainingGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPPHSpecializedTrainingGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPApplicationGUID { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CourseTitle", ResourceType = typeof(resxDbFields))]
        public string CourseTitle { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SchoolName", ResourceType = typeof(resxDbFields))]
        public string SchoolName { get; set; }

        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> CountryGUID { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "CourseStartDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> CourseStartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "CourseEndDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> CourseEndDate { get; set; }

        [Display(Name = "CourseTopicAreaDescription", ResourceType = typeof(resxDbFields))]
        public string CourseTopicAreaDescription { get; set; }

        [Display(Name = "CourseTrainingMethodologyGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> CourseTrainingMethodologyGUID { get; set; }

        [Display(Name = "CourseDescription", ResourceType = typeof(resxDbFields))]
        public string CourseDescription { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataPersonalHistorySpecializedTrainingRowVersion { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }


    }

    public class dataPersonalHistoryEducationUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPPHEducationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPPHEducationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPApplicationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ExactTitleOfDegreeOrCertification", ResourceType = typeof(resxDbFields))]
        public string ExactTitleOfDegreeOrCertification { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.Date)]
        [Display(Name = "BegintDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> BegintDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "EndDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> EndDate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MainMajorOrTopic", ResourceType = typeof(resxDbFields))]
        public string MainMajorOrTopic { get; set; }

        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OtherMajorsOrTopics", ResourceType = typeof(resxDbFields))]
        public string OtherMajorsOrTopics { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EducationLevelGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid EducationLevelGUID { get; set; }

        [Display(Name = "IfOtherEducationDescription", ResourceType = typeof(resxDbFields))]
        public string IfOtherEducationDescription { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MainLanguageOfStudyGUID", ResourceType = typeof(resxDbFields))]
        public string MainLanguageOfStudyGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SchoolNameDescription", ResourceType = typeof(resxDbFields))]
        public string SchoolNameDescription { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CityGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid CityGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid CountryGUID { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "IsEducationCompleted", ResourceType = typeof(resxDbFields))]
        public bool IsEducationCompleted { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataPersonalHistoryEducationRowVersion { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }
    }

    public class dataPersonalHistorySkillUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPPHSkillGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPPHSkillGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPApplicationGUID { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SkillDescription", ResourceType = typeof(resxDbFields))]
        public string SkillDescription { get; set; }

        [Display(Name = "SkillLevelGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> SkillLevelGUID { get; set; }

        [Display(Name = "SkillNumberOfYears", ResourceType = typeof(resxDbFields))]
        public Nullable<int> SkillNumberOfYears { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataPersonalHistorySkillRowVersion { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }


    }

    public class dataPersonalHistoryLanguageUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPPHLanguageGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPPHLanguageGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPApplicationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MotherTongueLanguageGUID", ResourceType = typeof(resxDbFields))]
        public string MotherTongueLanguageGUID { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "MotherTongueLanguageReadingGUID", ResourceType = typeof(resxDbFields))]
        //public System.Guid MotherTongueLanguageReadingGUID { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "MotherTongueLanguageSpeakingGUID", ResourceType = typeof(resxDbFields))]
        //public System.Guid MotherTongueLanguageSpeakingGUID { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "MotherTongueLanguageWritingGUID", ResourceType = typeof(resxDbFields))]
        //public System.Guid MotherTongueLanguageWritingGUID { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "MotherTongueLanguageListeningGUID", ResourceType = typeof(resxDbFields))]
        //public System.Guid MotherTongueLanguageListeningGUID { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SecondLanguageGUID", ResourceType = typeof(resxDbFields))]
        public string SecondLanguageGUID { get; set; }

        [Display(Name = "SecondLanguageReadingGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> SecondLanguageReadingGUID { get; set; }

        [Display(Name = "SecondLanguageSpeakingGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> SecondLanguageSpeakingGUID { get; set; }

        [Display(Name = "SecondLanguageWritingGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> SecondLanguageWritingGUID { get; set; }

        [Display(Name = "SecondLanguageListeningGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> SecondLanguageListeningGUID { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "SecondLanguageEvaluationDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> SecondLanguageEvaluationDate { get; set; }


        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ThirdLanguageGUID", ResourceType = typeof(resxDbFields))]
        public string ThirdLanguageGUID { get; set; }

        [Display(Name = "ThirdLanguageReadingGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ThirdLanguageReadingGUID { get; set; }

        [Display(Name = "ThirdLanguageSpeakingGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ThirdLanguageSpeakingGUID { get; set; }

        [Display(Name = "ThirdLanguageWritingGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ThirdLanguageWritingGUID { get; set; }

        [Display(Name = "ThirdLanguageListeningGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ThirdLanguageListeningGUID { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "ThirdLanguageEvaluationDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> ThirdLanguageEvaluationDate { get; set; }


        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FourthLanguageGUID", ResourceType = typeof(resxDbFields))]
        public string FourthLanguageGUID { get; set; }

        [Display(Name = "FourthLanguageReadingGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> FourthLanguageReadingGUID { get; set; }

        [Display(Name = "FourthLanguageSpeakingGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> FourthLanguageSpeakingGUID { get; set; }

        [Display(Name = "FourthLanguageWritingGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> FourthLanguageWritingGUID { get; set; }

        [Display(Name = "FourthLanguageListeningGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> FourthLanguageListeningGUID { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "FourthLanguageEvaluationDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> FourthLanguageEvaluationDate { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataPersonalHistoryLanguageRowVersion { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }


    }

    public class dataPersonalHistoryLicenceAndCertificateUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPPHLicenceGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPPHLicenceGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPApplicationGUID { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "LicenceIssueDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> LicenceIssueDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "LicenceExpirationDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> LicenceExpirationDate { get; set; }

        [Display(Name = "LicenceDescription", ResourceType = typeof(resxDbFields))]
        public string LicenceDescription { get; set; }

        [Display(Name = "CityGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> CityGUID { get; set; }

        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> CountryGUID { get; set; }

        [Display(Name = "IsLicenceRenewalInProgres", ResourceType = typeof(resxDbFields))]
        public Nullable<bool> IsLicenceRenewalInProgres { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "LicenceNumber", ResourceType = typeof(resxDbFields))]
        public string LicenceNumber { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "LicenceIssuedBy", ResourceType = typeof(resxDbFields))]
        public string LicenceIssuedBy { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataPersonalHistoryLicenceAndCertificateRowVersion { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }

    }

    public class dataPersonalHistoryProfessionalReferenceUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPPHProfReferenceGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPPHProfReferenceGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPApplicationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FirstReferenceName", ResourceType = typeof(resxDbFields))]
        public string FirstReferenceName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FirstReferenceTitle", ResourceType = typeof(resxDbFields))]
        public string FirstReferenceTitle { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FirstReferenceEmployer", ResourceType = typeof(resxDbFields))]
        public string FirstReferenceEmployer { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FirstReferenceCountryCode", ResourceType = typeof(resxDbFields))]
        public string FirstReferenceCountryCode { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FirstReferenceTelephoneNumber", ResourceType = typeof(resxDbFields))]
        public string FirstReferenceTelephoneNumber { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FirstReferenceExt", ResourceType = typeof(resxDbFields))]
        public string FirstReferenceExt { get; set; }

        [DataType(DataType.EmailAddress)]
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FirstReferenceEmailAddress", ResourceType = typeof(resxDbFields))]
        public string FirstReferenceEmailAddress { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FirstReferenceAddressLine1", ResourceType = typeof(resxDbFields))]
        public string FirstReferenceAddressLine1 { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FirstReferenceAddressLine2", ResourceType = typeof(resxDbFields))]
        public string FirstReferenceAddressLine2 { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FirstReferenceAddressLine3", ResourceType = typeof(resxDbFields))]
        public string FirstReferenceAddressLine3 { get; set; }

        [Display(Name = "FirstReferenceAddressCityGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> FirstReferenceAddressCityGUID { get; set; }

        [Display(Name = "FirstReferenceAddressCountryGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> FirstReferenceAddressCountryGUID { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FirstReferenceAddressPostalCode", ResourceType = typeof(resxDbFields))]
        public string FirstReferenceAddressPostalCode { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SecondReferenceName", ResourceType = typeof(resxDbFields))]
        public string SecondReferenceName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SecondReferenceTitle", ResourceType = typeof(resxDbFields))]
        public string SecondReferenceTitle { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SecondReferenceEmployer", ResourceType = typeof(resxDbFields))]
        public string SecondReferenceEmployer { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SecondReferenceCountryCode", ResourceType = typeof(resxDbFields))]
        public string SecondReferenceCountryCode { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SecondReferenceTelephoneNumber", ResourceType = typeof(resxDbFields))]
        public string SecondReferenceTelephoneNumber { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SecondReferenceExt", ResourceType = typeof(resxDbFields))]
        public string SecondReferenceExt { get; set; }

        [DataType(DataType.EmailAddress)]
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SecondReferenceEmailAddress", ResourceType = typeof(resxDbFields))]
        public string SecondReferenceEmailAddress { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SecondReferenceAddressLine1", ResourceType = typeof(resxDbFields))]
        public string SecondReferenceAddressLine1 { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SecondReferenceAddressLine2", ResourceType = typeof(resxDbFields))]
        public string SecondReferenceAddressLine2 { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SecondReferenceAddressLine3", ResourceType = typeof(resxDbFields))]
        public string SecondReferenceAddressLine3 { get; set; }

        [Display(Name = "SecondReferenceAddressCityGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> SecondReferenceAddressCityGUID { get; set; }

        [Display(Name = "SecondReferenceAddressCountryGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> SecondReferenceAddressCountryGUID { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SecondReferenceAddressPostalCode", ResourceType = typeof(resxDbFields))]
        public string SecondReferenceAddressPostalCode { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ThirdReferenceName", ResourceType = typeof(resxDbFields))]
        public string ThirdReferenceName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ThirdReferenceTitle", ResourceType = typeof(resxDbFields))]
        public string ThirdReferenceTitle { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ThirdReferenceEmployer", ResourceType = typeof(resxDbFields))]
        public string ThirdReferenceEmployer { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ThirdReferenceCountryCode", ResourceType = typeof(resxDbFields))]
        public string ThirdReferenceCountryCode { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ThirdReferenceTelephoneNumber", ResourceType = typeof(resxDbFields))]
        public string ThirdReferenceTelephoneNumber { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ThirdReferenceExt", ResourceType = typeof(resxDbFields))]
        public string ThirdReferenceExt { get; set; }

        [DataType(DataType.EmailAddress)]
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ThirdReferenceEmailAddress", ResourceType = typeof(resxDbFields))]
        public string ThirdReferenceEmailAddress { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ThirdReferenceAddressLine1", ResourceType = typeof(resxDbFields))]
        public string ThirdReferenceAddressLine1 { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ThirdReferenceAddressLine2", ResourceType = typeof(resxDbFields))]
        public string ThirdReferenceAddressLine2 { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ThirdReferenceAddressLine3", ResourceType = typeof(resxDbFields))]
        public string ThirdReferenceAddressLine3 { get; set; }

        [Display(Name = "ThirdReferenceAddressCityGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ThirdReferenceAddressCityGUID { get; set; }

        [Display(Name = "ThirdReferenceAddressCountryGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ThirdReferenceAddressCountryGUID { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ThirdReferenceAddressPostalCode", ResourceType = typeof(resxDbFields))]
        public string ThirdReferenceAddressPostalCode { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataPersonalHistoryProfessionalReferenceRowVersion { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }


    }

    public class dataPersonalHistoryQuestionnaireUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPPHQuestionnaireGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPPHQuestionnaireGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPApplicationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FirstQuestionAnswer", ResourceType = typeof(resxDbFields))]
        public bool FirstQuestionAnswer { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "FirstQuestionAnswerFromDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> FirstQuestionAnswerFromDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "FirstQuestionAnswerToDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> FirstQuestionAnswerToDate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SecondQuestionAnswer", ResourceType = typeof(resxDbFields))]
        public bool SecondQuestionAnswer { get; set; }

        [Display(Name = "SecondQuestionAnswerDetails", ResourceType = typeof(resxDbFields))]
        public string SecondQuestionAnswerDetails { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ThirdQuestionAnswer", ResourceType = typeof(resxDbFields))]
        public bool ThirdQuestionAnswer { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FourthQuestionAnswer", ResourceType = typeof(resxDbFields))]
        public bool FourthQuestionAnswer { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FifthQuestionAnswer", ResourceType = typeof(resxDbFields))]
        public bool FifthQuestionAnswer { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SixthQuestionAnswer", ResourceType = typeof(resxDbFields))]
        public bool SixthQuestionAnswer { get; set; }

        [Display(Name = "SixthQuestionAnswerDetails", ResourceType = typeof(resxDbFields))]
        public string SixthQuestionAnswerDetails { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SeventhQuestionAnswer", ResourceType = typeof(resxDbFields))]
        public bool SeventhQuestionAnswer { get; set; }

        [Display(Name = "SeventhQuestionAnswerDetails", ResourceType = typeof(resxDbFields))]
        public string SeventhQuestionAnswerDetails { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EighthQuestionAnswerDetails", ResourceType = typeof(resxDbFields))]
        public bool EighthQuestionAnswerDetails { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "NinthQuestionAnswer", ResourceType = typeof(resxDbFields))]
        public bool NinthQuestionAnswer { get; set; }

        [Display(Name = "NinthQuestionAnswerDetails", ResourceType = typeof(resxDbFields))]
        public string NinthQuestionAnswerDetails { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TenthQuestionAnswer", ResourceType = typeof(resxDbFields))]
        public bool TenthQuestionAnswer { get; set; }

        [Display(Name = "TenthQuestionAnswerDetails", ResourceType = typeof(resxDbFields))]
        public string TenthQuestionAnswerDetails { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ElevenQuestionAnswer", ResourceType = typeof(resxDbFields))]
        public bool ElevenQuestionAnswer { get; set; }

        [Display(Name = "ElevenQuestionAnswerDetails", ResourceType = typeof(resxDbFields))]
        public string ElevenQuestionAnswerDetails { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TwelveQuestionAnswerDetails", ResourceType = typeof(resxDbFields))]
        public bool TwelveQuestionAnswerDetails { get; set; }

        [Display(Name = "ThirteenQuestionAnswer", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ThirteenQuestionAnswer { get; set; }

        [Display(Name = "ThirteenQuestionAnswerDetails", ResourceType = typeof(resxDbFields))]
        public string ThirteenQuestionAnswerDetails { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataPersonalHistoryQuestionnaireRowVersion { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }

    }

    public class dataPersonalHistoryQuestionnaireWithRelativesUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPPHQuestionnaireGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPPHQuestionnaireGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPApplicationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FirstQuestionAnswer", ResourceType = typeof(resxDbFields))]
        public bool FirstQuestionAnswer { get; set; }

        [RequiredIf("FirstQuestionAnswer == true", ErrorMessage = "Field is required when answer is set to Yes")]
        [DataType(DataType.Date)]
        [Display(Name = "FirstQuestionAnswerFromDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> FirstQuestionAnswerFromDate { get; set; }

        [RequiredIf("FirstQuestionAnswer == true", ErrorMessage = "Field is required when answer is set to Yes")]
        [DataType(DataType.Date)]
        [Display(Name = "FirstQuestionAnswerToDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> FirstQuestionAnswerToDate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SecondQuestionAnswer", ResourceType = typeof(resxDbFields))]
        public bool SecondQuestionAnswer { get; set; }

        [RequiredIf("SecondQuestionAnswer == true", ErrorMessage = "Field is required when answer is set to Yes")]

        [Display(Name = "SecondQuestionAnswerDetails", ResourceType = typeof(resxDbFields))]
        public string SecondQuestionAnswerDetails { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ThirdQuestionAnswer", ResourceType = typeof(resxDbFields))]
        public bool ThirdQuestionAnswer { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FourthQuestionAnswer", ResourceType = typeof(resxDbFields))]
        public bool FourthQuestionAnswer { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FifthQuestionAnswer", ResourceType = typeof(resxDbFields))]
        public bool FifthQuestionAnswer { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SixthQuestionAnswer", ResourceType = typeof(resxDbFields))]
        public bool SixthQuestionAnswer { get; set; }

        [RequiredIf("SixthQuestionAnswer == true", ErrorMessage = "Field is required when answer is set to Yes")]
        [Display(Name = "SixthQuestionAnswerDetails", ResourceType = typeof(resxDbFields))]
        public string SixthQuestionAnswerDetails { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SeventhQuestionAnswer", ResourceType = typeof(resxDbFields))]
        public bool SeventhQuestionAnswer { get; set; }

        [RequiredIf("SeventhQuestionAnswer == true", ErrorMessage = "Field is required when answer is set to Yes")]
        [Display(Name = "SeventhQuestionAnswerDetails", ResourceType = typeof(resxDbFields))]
        public string SeventhQuestionAnswerDetails { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EighthQuestionAnswerDetails", ResourceType = typeof(resxDbFields))]
        public bool EighthQuestionAnswerDetails { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "NinthQuestionAnswer", ResourceType = typeof(resxDbFields))]
        public bool NinthQuestionAnswer { get; set; }

        [RequiredIf("NinthQuestionAnswer == true", ErrorMessage = "Field is required when answer is set to Yes")]
        [Display(Name = "NinthQuestionAnswerDetails", ResourceType = typeof(resxDbFields))]
        public string NinthQuestionAnswerDetails { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TenthQuestionAnswer", ResourceType = typeof(resxDbFields))]
        public bool TenthQuestionAnswer { get; set; }

        [RequiredIf("TenthQuestionAnswer == true", ErrorMessage = "Field is required when answer is set to Yes")]
        [Display(Name = "TenthQuestionAnswerDetails", ResourceType = typeof(resxDbFields))]
        public string TenthQuestionAnswerDetails { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ElevenQuestionAnswer", ResourceType = typeof(resxDbFields))]
        public bool ElevenQuestionAnswer { get; set; }

        [RequiredIf("ElevenQuestionAnswer == true", ErrorMessage = "Field is required when answer is set to Yes")]
        [Display(Name = "ElevenQuestionAnswerDetails", ResourceType = typeof(resxDbFields))]
        public string ElevenQuestionAnswerDetails { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TwelveQuestionAnswerDetails", ResourceType = typeof(resxDbFields))]
        public bool TwelveQuestionAnswerDetails { get; set; }

        [Display(Name = "ThirteenQuestionAnswer", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ThirteenQuestionAnswer { get; set; }

        [Display(Name = "ThirteenQuestionAnswerDetails", ResourceType = typeof(resxDbFields))]
        public string ThirteenQuestionAnswerDetails { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataPersonalHistoryQuestionnaireRowVersion { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public List<dataPersonalHistoryRelativeUpdateModel> dataPersonalHistoryRelatives { get; set; }
        public List<dataPersonalHistoryRelativeWorkerUpdateModel> dataPersonalHistoryRelativeWorkers { get; set; }
    }

    public class dataPersonalHistoryRelativeUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPPHRelativeGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPPHRelativeGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPApplicationGUID { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RelativeName", ResourceType = typeof(resxDbFields))]
        public string RelativeName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "RelativeBirthDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> RelativeBirthDate { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RelativeRelationship", ResourceType = typeof(resxDbFields))]
        public string RelativeRelationship { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataPersonalHistoryRelativeRowVersion { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }



    }

    public class dataPersonalHistoryRelativeWorkerUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPPHRelativeWorkerGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPPHRelativeWorkerGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPApplicationGUID { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RelativeName", ResourceType = typeof(resxDbFields))]
        public string RelativeName { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RelativeRelationship", ResourceType = typeof(resxDbFields))]
        public string RelativeRelationship { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RelativeOrganization", ResourceType = typeof(resxDbFields))]
        public string RelativeOrganization { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataPersonalHistoryRelativeWorkerRowVersion { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }

    }

    public class dataPersonalHistoryConfirmationAndConsentUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPPHConfirmationConsentGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPPHConfirmationConsentGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GTPApplicationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GTPApplicationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "IsConfirmedByUser", ResourceType = typeof(resxDbFields))]
        public bool IsConfirmedByUser { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.DateTime)]
        [Display(Name = "ConfirmationDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime ConfirmationDate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataPersonalHistoryConfirmationAndConsentRowVersion { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }

        public bool FormHasError { get; set; }
    }

    #endregion

    public class GroupTwoApplicationsDataTableModel
    {
        public Guid GTPApplicationGUID { get; set; }
        public string FullName { get; set; }
        public string GTPCategoryGUID { get; set; }
        public string GTPApplicationCategory { get; set; }
        public bool IsReviewed { get; set; }
        public bool Active { get; set; }
        public string GTPEligibility { get; set; }
        public bool? IsEligible { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool? IsConfirmedByUser { get; set; }
        public byte[] dataGTPApplicationRowVersion { get; set; }
    }

    public class ApplicationReviewUpdateModel
    {
        [Display(Name = "GTPApplicationGUID", ResourceType = typeof(resxDbFields))]
        public Guid GTPApplicationGUID { get; set; }

        [Display(Name = "GTPCategoryGUID", ResourceType = typeof(resxDbFields))]
        public Guid GTPCategoryGUID { get; set; }

        [Display(Name = "FullName", ResourceType = typeof(resxDbFields))]
        public string FullName { get; set; }

        [Display(Name = "EmailAddress", ResourceType = typeof(resxDbFields))]
        public string EmailAddress { get; set; }

        [Display(Name = "IsEligible", ResourceType = typeof(resxDbFields))]
        public bool? IsEligible { get; set; }

        [RequiredIf("IsEligible == true", ErrorMessage = "Field is required when Eligibility is set to Yes")]
        [Display(Name = "GTPApplicationExpiryDate", ResourceType = typeof(resxDbFields))]
        public DateTime? GTPApplicationExpiryDate { get; set; }

        [RequiredIf("IsEligible == true", ErrorMessage = "Field is required when Eligibility is set to Yes")]
        [Display(Name = "EmailBody", ResourceType = typeof(resxDbFields))]
        public string EmailBody { get; set; }


        public bool Active { get; set; }

        public byte[] dataGTPApplicationRowVersion { get; set; }
    }

    public class PersonalHistoryFormWizardUpdateModel
    {
        public Guid GTPApplicationGUID { get; set; }
        public bool GeneralInfoCompleted { get; set; }
        public bool PersonalInfoCompleted { get; set; }
        public bool ContactInfoCompleted { get; set; }
        public bool PhoneNumberCompleted { get; set; }
        public bool EmailAddressCompleted { get; set; }
        public bool NationalityInfoCompleted { get; set; }
        public bool LetterOfInteresCompleted { get; set; }
        public bool WorkExperienceCompleted { get; set; }
        public bool SpecializedTrainingCompleted { get; set; }
        public bool EducationsCompleted { get; set; }
        public bool SkillsCompleted { get; set; }
        public bool LanguagesCompleted { get; set; }
        public bool LicenceAndCertificateCompleted { get; set; }
        public bool ProfessionalReferencesCompleted { get; set; }
        public bool QuestionnaireCompleted { get; set; }
        public bool ConfirmationAndConsentCompleted { get; set; }

    }
}
