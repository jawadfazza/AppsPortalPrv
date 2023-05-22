using Portal_BL.Library;
using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace ORG_DAL.Model
{
    public class CodeBankDataTableModel
    {
        public Guid BankGUID { get; set; }
        public string BankCode { get; set; }
        public string BankDescription { get; set; }
        public Guid CountryGUID { get; set; }
        public byte[] codeBankRowVersion { get; set; }
        public bool Active { get; set; }



    }
    public class UserPhotoUpdateModel
    {
        public Guid UserGUID { get; set; }
    }
    public class CodeBankUpdateModel
    {

        [Display(Name = "BankGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid BankGUID { get; set; }

        [Display(Name = "BankCode", ResourceType = typeof(resxDbFields))]
        public string BankCode { get; set; }
        [Display(Name = "Country", ResourceType = typeof(resxDbFields))]

        public Guid? CountryGUID { get; set; }

        [Display(Name = "BankDescription", ResourceType = typeof(resxDbFields))]
        public string BankDescription { get; set; }

        public bool Active { get; set; }
        public byte[] codeBankRowVersion { get; set; }
        public byte[] codeBankLanguageRowVersion { get; set; }
    }
    public class StaffProfileDataTableModel
    {

        public Guid UserGUID { get; set; }
        public string StaffName { get; set; }
        public string EmailAddress { get; set; }
        public string EmploymentID { get; set; }

        public string StaffStatus { get; set; }
        public string StaffPhoto { get; set; }
        public Nullable<Guid> JobTitleGUID { get; set; }

        [Display(Name = "JobTitleDescription", ResourceType = typeof(resxDbFields))]
        public string JobTitleDescription { get; set; }
        public Guid DutyStationGUID { get; set; }
        public string DutyStationDescription { get; set; }
        public string DepartmentGUID { get; set; }
        [Display(Name = "StaffStatus", ResourceType = typeof(resxDbFields))]
        public string StaffStatusGUID { get; set; }



        [Display(Name = "ContractType", ResourceType = typeof(resxDbFields))]
        public string ContractType { get; set; }


        [Display(Name = "ContractType", ResourceType = typeof(resxDbFields))]
        public string ContractTypeGUID { get; set; }


        public string DepartmentDescription { get; set; }

        [Display(Name = "RecruitmentType", ResourceType = typeof(resxDbFields))]
        public string RecruitmentTypeGUID { get; set; }

        [Display(Name = "RecruitmentType", ResourceType = typeof(resxDbFields))]
        public string RecruitmentType { get; set; }
        public Guid? Gender { get; set; }
        public bool Active { get; set; }
        public byte[] StaffCoreDataRowVersion { get; set; }
    }


    public class StaffProfileDataTable
    {
        public int? AccessLevel { get; set; }
        public string UserGUID { get; set; }
        public string StaffName { get; set; }
        public string EmailAddress { get; set; }
        public string EmploymentID { get; set; }
        [Display(Name = "Grade", ResourceType = typeof(resxDbFields))]
        public string StaffGradeGUID { get; set; }

        [Display(Name = "OfficeTypeGUID", ResourceType = typeof(resxDbFields))]
        public string OfficeTypeGUID { get; set; }

        [Display(Name = "ReportToGUID", ResourceType = typeof(resxDbFields))]
        public string ReportToGUID { get; set; }


        [Display(Name = "StaffEOD", ResourceType = typeof(resxDbFields))]
        public DateTime? StaffEOD { get; set; }

        [Display(Name = "ContractEndDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ContractEndDate { get; set; }

        [Display(Name = "LastDepartureDate", ResourceType = typeof(resxDbFields))]
        public DateTime? LastDepartureDate { get; set; }



        [Display(Name = "LastConfirmationStatus", ResourceType = typeof(resxDbFields))]
        public string LastConfirmationStatus { get; set; }

        [Display(Name = "LastConfirmationStatus", ResourceType = typeof(resxDbFields))]
        public string LastConfirmationStatusGUID { get; set; }

        public string StaffStatus { get; set; }
        public string StaffPhoto { get; set; }
        public string JobTitleGUID { get; set; }

        [Display(Name = "JobTitleDescription", ResourceType = typeof(resxDbFields))]
        public string JobTitleDescription { get; set; }
        public string DutyStationGUID { get; set; }
        public string DutyStationDescription { get; set; }
        public string DepartmentGUID { get; set; }
        [Display(Name = "StaffStatus", ResourceType = typeof(resxDbFields))]
        public string StaffStatusGUID { get; set; }



        [Display(Name = "ContractType", ResourceType = typeof(resxDbFields))]
        public string ContractType { get; set; }


        [Display(Name = "ContractType", ResourceType = typeof(resxDbFields))]
        public string ContractTypeGUID { get; set; }


        public string DepartmentDescription { get; set; }

        [Display(Name = "RecruitmentType", ResourceType = typeof(resxDbFields))]
        public string RecruitmentTypeGUID { get; set; }

        [Display(Name = "RecruitmentType", ResourceType = typeof(resxDbFields))]
        public string RecruitmentType { get; set; }
        public string Gender { get; set; }
        public bool Active { get; set; }
        public byte[] StaffCoreDataRowVersion { get; set; }
    }
    public class StaffContactsInformationDataTableModel
    {
        public Guid StaffSimGUID { get; set; }
        public Guid UserGUID { get; set; }
        public Guid PhoneTypeGUID { get; set; }
        public string PhoneTypeDescription { get; set; }
        public Guid PhoneUsageTypeGUID { get; set; }
        public string PhoneUsageTypeDescription { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool Active { get; set; }


        public byte[] dataStaffPhoneRowVersion { get; set; }
    }

    public class StaffAddressInformationDataTableModel
    {

    }
    public class StaffProfileUpdateModel
    {
        public StaffProfileUpdateModel()
        {
            MediaPath = "/Assets/Images/img.png";
            AllStaffPositionsGUID = new List<Guid?>();
        }
        public string ActiveDirectorySearchInput { get; set; }

        [Display(Name = "PaymentEligibilityStatusGUID", ResourceType = typeof(resxDbFields))]
        public Guid? PaymentEligibilityStatusGUID { get; set; }

        public string MediaPath { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "AssignmentType", ResourceType = typeof(resxDbFields))]
        [Display(Name = "StaffPositionGUID", ResourceType = typeof(resxDbFields))]

        public List<Guid?> AllStaffPositionsGUID { get; set; }


        public string showPositions { get; set; }

        public Guid UserGUID { get; set; }
        [Display(Name = "ICTComments", ResourceType = typeof(resxDbFields))]

        public string ICTComments { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "EmailAddress", ResourceType = typeof(resxDbFields))]
        public string EmailAddress { get; set; }

        [Display(Name = "OfficeType", ResourceType = typeof(resxDbFields))]
        public Guid? OfficeTypeGUID { get; set; }

        [Display(Name = "IsInternational", ResourceType = typeof(resxDbFields))]
        public bool? IsInternational { get; set; }

        [Display(Name = "StaffPhoto", ResourceType = typeof(resxDbFields))]
        public string StaffPhoto { get; set; }

        [Display(Name = "NextOfKinName", ResourceType = typeof(resxDbFields))]
        public string NextOfKinName { get; set; }

        [Display(Name = "KinMobileNumber", ResourceType = typeof(resxDbFields))]
        public string KinMobileNumber { get; set; }

        [Display(Name = "BSAFECertAcquired", ResourceType = typeof(resxDbFields))]
        public Nullable<bool> BSAFECertAcquired { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "BSAFEExpiryDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> BSAFEExpiryDate { get; set; }

        [Display(Name = "NumberOfDependants", ResourceType = typeof(resxDbFields))]
        public Nullable<int> NumberOfDependants { get; set; }

        [Display(Name = "DependantsName", ResourceType = typeof(resxDbFields))]
        public string DependantsName { get; set; }

        [Display(Name = "JobTitleGUID", ResourceType = typeof(resxDbFields))]
        public string JobTitle { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public string DutyStation { get; set; }


        [Display(Name = "StaffPositionGUID", ResourceType = typeof(resxDbFields))]
        public string PositionName { get; set; }


        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
        public string Department { get; set; }

        [StringLength(13, MinimumLength = 13, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HomeTelephoneNumberMobile", ResourceType = typeof(resxDbFields))]
        public string PrivateNumberSection2 { get; set; }

        public HttpPostedFileBase StaffPhotoFile { get; set; }


        [Display(Name = "StaffStatus", ResourceType = typeof(resxDbFields))]
        public bool StaffStatus { get; set; }


        [Display(Name = "PositionInOrganigram", ResourceType = typeof(resxDbFields))]
        public string PositionInOrganigram { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "JobTilteMOFAEN", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> JobTitleGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Guid DutyStationGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Guid? StaffStatusGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "StaffPrefixGUID", ResourceType = typeof(resxDbFields))]
        public Guid? StaffPrefixGUID { get; set; }




        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
        public Guid? DepartmentGUID { get; set; }

        [Display(Name = "ReportToGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ReportToGUID { get; set; }

        [Display(Name = "NationalityGUID", ResourceType = typeof(resxDbFields))]
        public Guid? NationalityGUID { get; set; }

        [Display(Name = "Nationality2GUID", ResourceType = typeof(resxDbFields))]
        public Guid? Nationality2GUID { get; set; }

        [Display(Name = "Nationality3GUID", ResourceType = typeof(resxDbFields))]
        public Guid? Nationality3GUID { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "ExpiryOfResidencyVisa", ResourceType = typeof(resxDbFields))]
        public DateTime? ExpiryOfResidencyVisa { get; set; }

        [NotMapped]
        [Display(Name = "ExpiryOfResidencyVisa", ResourceType = typeof(resxDbFields))]
        public DateTime? LocalExpiryOfResidencyVisa { get { return new Portal().LocalTime(this.ExpiryOfResidencyVisa); } }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SyrianNationalIDNumber", ResourceType = typeof(resxDbFields))]
        public string SyrianNationalIDNumber { get; set; }

        [Display(Name = "SyrianNationalIDPhoto", ResourceType = typeof(resxDbFields))]
        public string SyrianNationalIDPhoto { get; set; }

        public HttpPostedFileBase SyrianNationalIDPhotoFile { get; set; }



        [StringLength(10, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "StaffPrefix", ResourceType = typeof(resxDbFields))]
        public string StaffPrefix { get; set; }

        [Display(Name = "PlaceOfBirthGUID", ResourceType = typeof(resxDbFields))]
        public Guid? PlaceOfBirthGUID { get; set; }


        [Display(Name = "EmploymentID", ResourceType = typeof(resxDbFields))]
        public int? EmploymentID { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "UNHCRIDNumber", ResourceType = typeof(resxDbFields))]
        public string UNHCRIDNumber { get; set; }

        [Display(Name = "CallSign", ResourceType = typeof(resxDbFields))]
        public string CallSign { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "StaffLastWork", ResourceType = typeof(resxDbFields))]
        public string StaffLastWork { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "UNLPNumber", ResourceType = typeof(resxDbFields))]
        public string UNLPNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "UNLPDateOfIssue", ResourceType = typeof(resxDbFields))]
        public DateTime? UNLPDateOfIssue { get; set; }

        [NotMapped]
        [Display(Name = "UNLPDateOfIssue", ResourceType = typeof(resxDbFields))]
        public DateTime? LocalUNLPDateOfIssue { get { return new Portal().LocalTime(this.UNLPDateOfIssue); } }

        [DataType(DataType.Date)]
        [Display(Name = "UNLPDateOfExpiry", ResourceType = typeof(resxDbFields))]
        public DateTime? UNLPDateOfExpiry { get; set; }

        [NotMapped]
        [Display(Name = "UNLPDateOfExpiry", ResourceType = typeof(resxDbFields))]
        public DateTime? LocalUNLPDateOfExpiry { get { return new Portal().LocalTime(this.UNLPDateOfExpiry); } }

        [Display(Name = "UNLPPhoto", ResourceType = typeof(resxDbFields))]
        public string UNLPPhoto { get; set; }
        public HttpPostedFileBase UNLPPhotoFile { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "NationalPassportNumber", ResourceType = typeof(resxDbFields))]
        public string NationalPassportNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "NationalPassportDateOfIssue", ResourceType = typeof(resxDbFields))]
        public DateTime? NationalPassportDateOfIssue { get; set; }

        [NotMapped]
        [Display(Name = "NationalPassportDateOfIssue", ResourceType = typeof(resxDbFields))]
        public DateTime? LocalNationalPassportDateOfIssue { get { return new Portal().LocalTime(this.NationalPassportDateOfIssue); } }

        [DataType(DataType.Date)]
        [Display(Name = "NationalPassportDateOfExpiry", ResourceType = typeof(resxDbFields))]
        public DateTime? NationalPassportDateOfExpiry { get; set; }

        [NotMapped]
        [Display(Name = "NationalPassportDateOfExpiry", ResourceType = typeof(resxDbFields))]
        public DateTime? LocalNationalPassportDateOfExpiry { get { return new Portal().LocalTime(this.NationalPassportDateOfExpiry); } }

        [Display(Name = "NationalPassportPhoto", ResourceType = typeof(resxDbFields))]
        public string NationalPassportPhoto { get; set; }
        public HttpPostedFileBase NationalPassportPhotoFile { get; set; }




        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]

        [Display(Name = "ContractTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ContractTypeGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RecruitmentTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? RecruitmentTypeGUID { get; set; }

        [Display(Name = "StaffGradeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? StaffGradeGUID { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "ContractStartDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ContractStartDate { get; set; }

        [DataType(DataType.Date)]
        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ContractEndDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ContractEndDate { get; set; }

        [NotMapped]
        [Display(Name = "ContractEndDate", ResourceType = typeof(resxDbFields))]
        public DateTime? LocalContractEndDate { get { return new Portal().LocalTime(this.ContractEndDate); } }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        [Display(Name = "StaffEOD", ResourceType = typeof(resxDbFields))]
        public DateTime? StaffEOD { get; set; }

        [NotMapped]
        [Display(Name = "StaffEOD", ResourceType = typeof(resxDbFields))]
        public DateTime? LocalStaffEOD { get { return new Portal().LocalTime(this.StaffEOD); } }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PositionNumber", ResourceType = typeof(resxDbFields))]
        public string PositionNumber { get; set; }

        [Display(Name = "PermanentAddressEn", ResourceType = typeof(resxDbFields))]
        public string PermanentAddressEn { get; set; }

        [Display(Name = "PermanentAddressAr", ResourceType = typeof(resxDbFields))]
        public string PermanentAddressAr { get; set; }

        [Display(Name = "CurrentAddressEn", ResourceType = typeof(resxDbFields))]
        public string CurrentAddressEn { get; set; }

        [Display(Name = "CurrentAddressAr", ResourceType = typeof(resxDbFields))]
        public string CurrentAddressAr { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HomeTelephoneNumberLandline", ResourceType = typeof(resxDbFields))]
        public string HomeTelephoneNumberLandline { get; set; }

        [StringLength(13, MinimumLength = 13, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HomeTelephoneNumberMobile", ResourceType = typeof(resxDbFields))]



        [DataType(DataType.PhoneNumber)]
        public string HomeTelephoneNumberMobile { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OfficialMobileNumber", ResourceType = typeof(resxDbFields))]

        [DataType(DataType.PhoneNumber)]

        public string OfficialMobileNumber { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HQExtensionNumber", ResourceType = typeof(resxDbFields))]
        public string HQExtensionNumber { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DamascusExtensionNumber", ResourceType = typeof(resxDbFields))]
        public string DamascusExtensionNumber { get; set; }


        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OfficialExtensionNumber", ResourceType = typeof(resxDbFields))]
        public string OfficialExtensionNumber { get; set; }

        [Display(Name = "OfficeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? OfficeGUID { get; set; }

        [Display(Name = "OfficeFloorGUID", ResourceType = typeof(resxDbFields))]
        public Guid? OfficeFloorGUID { get; set; }

        [Display(Name = "OfficeRoomGUID", ResourceType = typeof(resxDbFields))]
        public Guid? OfficeRoomGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] StaffCoreDataRowVersion { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        //personal details table
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FirstNameEN", ResourceType = typeof(resxDbFields))]
        public string FirstNameEN { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SurnameEN", ResourceType = typeof(resxDbFields))]
        public string SurnameEN { get; set; }


        [Display(Name = "FirstNameAR", ResourceType = typeof(resxDbFields))]
        public string FirstNameAR { get; set; }


        [Display(Name = "SurnameAR", ResourceType = typeof(resxDbFields))]
        public string SurnameAR { get; set; }

        [Display(Name = "PlaceOfBirthCountryAR", ResourceType = typeof(resxDbFields))]
        public string PlaceOfBirthCountryAR { get; set; }


        [Display(Name = "FatherNameEN", ResourceType = typeof(resxDbFields))]
        public string FatherNameEN { get; set; }


        [Display(Name = "FatherNameAR", ResourceType = typeof(resxDbFields))]
        public string FatherNameAR { get; set; }


        [Display(Name = "MotherNameEN", ResourceType = typeof(resxDbFields))]
        public string MotherNameEN { get; set; }


        [Display(Name = "MotherNameAR", ResourceType = typeof(resxDbFields))]
        public string MotherNameAR { get; set; }


        [Display(Name = "GenderGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> GenderGUID { get; set; }


        [Display(Name = "BloodGroup", ResourceType = typeof(resxDbFields))]
        public string BloodGroup { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        [Display(Name = "DateOfBirth", ResourceType = typeof(resxDbFields))]
        public DateTime? DateOfBirth { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        [Display(Name = "LastDepartureDate", ResourceType = typeof(resxDbFields))]
        public DateTime? LastDepartureDate { get; set; }

        [Display(Name = "StaffPositionGUID", ResourceType = typeof(resxDbFields))]
        public Guid? StaffPositionGUID { get; set; }



        [Display(Name = "PlaceBirthCityEn", ResourceType = typeof(resxDbFields))]
        public string PlaceBirthCityEn { get; set; }

        [Display(Name = "PlaceBirthCityAr", ResourceType = typeof(resxDbFields))]
        public string PlaceBirthCityAr { get; set; }
        [Display(Name = "LastJobEn", ResourceType = typeof(resxDbFields))]
        public string LastJobEn { get; set; }
        [Display(Name = "LastJobAr", ResourceType = typeof(resxDbFields))]
        public string LastJobAr { get; set; }
        [Display(Name = "DegreeEn", ResourceType = typeof(resxDbFields))]
        public string DegreeEn { get; set; }

        [Display(Name = "JobTilteMOFAAR", ResourceType = typeof(resxDbFields))]
        public string JobTitleAR { get; set; }
        [Display(Name = "DegreeAr", ResourceType = typeof(resxDbFields))]
        public string DegreeAr { get; set; }
        [Display(Name = "OfficeRoomNumberBuilding", ResourceType = typeof(resxDbFields))]
        public string OfficeRoomNumberBuilding { get; set; }
        [Display(Name = "SATPhoneNumber", ResourceType = typeof(resxDbFields))]
        public string SATPhoneNumber { get; set; }

        [Display(Name = "Nationality1Arabic", ResourceType = typeof(resxDbFields))]
        public string Nationality1Arabic { get; set; }
        [Display(Name = "Nationality3Arabic", ResourceType = typeof(resxDbFields))]

        public string Nationality3Arabic { get; set; }
        [Display(Name = "Nationality2Arabic", ResourceType = typeof(resxDbFields))]
        public string Nationality2Arabic { get; set; }

        #region Bank Syria

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BankNameSyrGUID", ResourceType = typeof(resxDbFields))]
        public string BankNameSyrGUID { get; set; }



        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BankNameSyr", ResourceType = typeof(resxDbFields))]
        public string BankNameSyr { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BankAccountNumberSyr", ResourceType = typeof(resxDbFields))]
        public string BankAccountNumberSyr { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BankAccountHolderNameEn", ResourceType = typeof(resxDbFields))]
        public string BankAccountHolderNameEn { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BankAccountHolderNameAr", ResourceType = typeof(resxDbFields))]
        public string BankAccountHolderNameAr { get; set; }


        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TitleEnglishMSRP", ResourceType = typeof(resxDbFields))]
        public string TitleEnglishMSRP { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TitleArabicMSRP", ResourceType = typeof(resxDbFields))]
        public string TitleArabicMSRP { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TitleEnglishMOFA", ResourceType = typeof(resxDbFields))]
        public string TitleEnglishMOFA { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TitleArabicMOFA", ResourceType = typeof(resxDbFields))]
        public string TitleArabicMOFA { get; set; }




        #endregion
        //#region leb Bank

        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankLebNameGUID", ResourceType = typeof(resxDbFields))]
        //public string BankLebNameGUID { get; set; }


        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankNameLeb", ResourceType = typeof(resxDbFields))]
        //public string BankNameLeb { get; set; }

        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankAccountNumberLeb", ResourceType = typeof(resxDbFields))]
        //public string BankAccountNumberLeb { get; set; }

        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankAccountHolderNameLebAR", ResourceType = typeof(resxDbFields))]
        //public string BankAccountHolderNameLebAR { get; set; }

        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankAccountHolderNameLebEN", ResourceType = typeof(resxDbFields))]
        //public string BankAccountHolderNameLebEN { get; set; }


        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankNameARLeb", ResourceType = typeof(resxDbFields))]
        //public string BankNameARLeb { get; set; }

        //#endregion





        //#region Bank Syria2

        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankNameSyrGUID2", ResourceType = typeof(resxDbFields))]
        //public string BankNameSyrGUID2 { get; set; }


        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankAccountNumberSyr2", ResourceType = typeof(resxDbFields))]
        //public string BankSYRAccountNumberSyr2 { get; set; }

        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankNameSyr2", ResourceType = typeof(resxDbFields))]
        //public string BankSYRNameEN2 { get; set; }

        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankSYRNameAR2", ResourceType = typeof(resxDbFields))]
        //public string BankSYRNameAR2 { get; set; }

        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankAccountHolderNameEn2", ResourceType = typeof(resxDbFields))]
        //public string BankSYRAccountHolderNameEn2 { get; set; }

        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankAccountHolderNameAr2", ResourceType = typeof(resxDbFields))]
        //public string BankSYRAccountHolderNameAr2 { get; set; }

        //#endregion


        //#region Bank Syria3

        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankSYRNameGUID3", ResourceType = typeof(resxDbFields))]
        //public string BankSYRNameGUID3 { get; set; }


        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankSYRAccountNumberSyr3", ResourceType = typeof(resxDbFields))]
        //public string BankSYRAccountNumberSyr3 { get; set; }

        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankSYRNameEN3", ResourceType = typeof(resxDbFields))]
        //public string BankSYRNameEN3 { get; set; }

        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankSYRNameAR3", ResourceType = typeof(resxDbFields))]
        //public string BankSYRNameAR3 { get; set; }

        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankSYRAccountHolderNameEn3", ResourceType = typeof(resxDbFields))]
        //public string BankSYRAccountHolderNameEn3 { get; set; }

        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankSYRAccountHolderNameAr3", ResourceType = typeof(resxDbFields))]
        //public string BankSYRAccountHolderNameAr3 { get; set; }

        //#endregion



        //#region Bank Syria4

        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankSYRNameGUID4", ResourceType = typeof(resxDbFields))]
        //public string BankSYRNameGUID4 { get; set; }


        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankSYRAccountNumberSyr4", ResourceType = typeof(resxDbFields))]
        //public string BankSYRAccountNumberSyr4 { get; set; }

        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankSYRNameEN4", ResourceType = typeof(resxDbFields))]
        //public string BankSYRNameEN4 { get; set; }

        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankSYRNameAR4", ResourceType = typeof(resxDbFields))]
        //public string BankSYRNameAR4 { get; set; }

        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankSYRAccountHolderNameEn4", ResourceType = typeof(resxDbFields))]
        //public string BankSYRAccountHolderNameEn4 { get; set; }

        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankSYRAccountHolderNameAr4", ResourceType = typeof(resxDbFields))]
        //public string BankSYRAccountHolderNameAr4 { get; set; }

        //#endregion


        //#region Bank Syria5

        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankSYRNameGUID5", ResourceType = typeof(resxDbFields))]
        //public string BankSYRNameGUID5 { get; set; }


        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankSYRAccountNumberSyr5", ResourceType = typeof(resxDbFields))]
        //public string BankSYRAccountNumberSyr5 { get; set; }

        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankSYRNameEN5", ResourceType = typeof(resxDbFields))]
        //public string BankSYRNameEN5 { get; set; }

        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankSYRNameAR5", ResourceType = typeof(resxDbFields))]
        //public string BankSYRNameAR5 { get; set; }

        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankSYRAccountHolderNameEn5", ResourceType = typeof(resxDbFields))]
        //public string BankSYRAccountHolderNameEn5 { get; set; }

        //[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BankSYRAccountHolderNameAr5", ResourceType = typeof(resxDbFields))]
        //public string BankSYRAccountHolderNameAr5 { get; set; }

        //#endregion

    }


    public class StaffProfileSecuritySectionUpdateModel
    {

        [Display(Name = "BloodGroup", ResourceType = typeof(resxDbFields))]
        public string BloodGroup { get; set; }



        public Guid UserGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "EmailAddress", ResourceType = typeof(resxDbFields))]
        public string EmailAddress { get; set; }


        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "UNHCRIDNumber", ResourceType = typeof(resxDbFields))]
        public string UNHCRIDNumber { get; set; }

        [Display(Name = "CallSign", ResourceType = typeof(resxDbFields))]
        public string CallSign { get; set; }





        [Display(Name = "PermanentAddressEn", ResourceType = typeof(resxDbFields))]
        public string PermanentAddressEn { get; set; }

        [Display(Name = "PermanentAddressAr", ResourceType = typeof(resxDbFields))]
        public string PermanentAddressAr { get; set; }

        [Display(Name = "CurrentAddressEn", ResourceType = typeof(resxDbFields))]
        public string CurrentAddressEn { get; set; }

        [Display(Name = "CurrentAddressAr", ResourceType = typeof(resxDbFields))]
        public string CurrentAddressAr { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HomeTelephoneNumberLandline", ResourceType = typeof(resxDbFields))]
        public string HomeTelephoneNumberLandline { get; set; }

        [StringLength(13, MinimumLength = 13, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HomeTelephoneNumberMobile", ResourceType = typeof(resxDbFields))]
        public string HomeTelephoneNumberMobile { get; set; }

        
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] StaffCoreDataRowVersion { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        //personal details table



    }

    #region Feedback

    public class StaffProfileFeedbackDataTableModel
    {

        [Display(Name = "StaffProfileFeedbackGUID", ResourceType = typeof(resxDbFields))]
        public Guid StaffProfileFeedbackGUID { get; set; }


        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
        public string UserGUID { get; set; }





        [Display(Name = "FeedbackTypeGUID", ResourceType = typeof(resxDbFields))]
        public string FeedbackTypeGUID { get; set; }


        [Display(Name = "FeedbackType", ResourceType = typeof(resxDbFields))]
        public string FeedbackType { get; set; }

        [Display(Name = "FeedbackDescription", ResourceType = typeof(resxDbFields))]
        public string FeedbackDescription { get; set; }
        [Display(Name = "ResloveDescription", ResourceType = typeof(resxDbFields))]
        public string ResloveDescription { get; set; }

        [Display(Name = "LastFlowStatusGUID", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusGUID { get; set; }

        [Display(Name = "LastFeedbackStatus", ResourceType = typeof(resxDbFields))]
        public string LastFeedbackStatus { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreateDate { get; set; }

        [DataType(DataType.Date)]


        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataStaffProfileFeedbackRowVersion { get; set; }
    }

    public class StaffProfileFeedbackUpdateModel
    {

        [Display(Name = "StaffProfileFeedbackGUID", ResourceType = typeof(resxDbFields))]
        public Guid StaffProfileFeedbackGUID { get; set; }


        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
        public Guid UserGUID { get; set; }


        public int? flowStep { get; set; }

        public int? sameUser { get; set; }


        [Display(Name = "FeedbackTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid FeedbackTypeGUID { get; set; }


        [Display(Name = "FeedbackType", ResourceType = typeof(resxDbFields))]
        public string FeedbackType { get; set; }

        [Display(Name = "FeedbackDescription", ResourceType = typeof(resxDbFields))]
        public string FeedbackDescription { get; set; }
        [Display(Name = "ResloveDescription", ResourceType = typeof(resxDbFields))]
        public string ResloveDescription { get; set; }

        [Display(Name = "LastFlowStatusGUID", ResourceType = typeof(resxDbFields))]
        public Guid? LastFlowStatusGUID { get; set; }

        [Display(Name = "LastFlowStatusName", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreateDate { get; set; }

        [DataType(DataType.Date)]


        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataStaffProfileFeedbackRowVersion { get; set; }
    }
    #endregion

    public class StaffCoreDocumentDataTableModel
    {

        [Display(Name = "StaffCoreDocumentGUID", ResourceType = typeof(resxDbFields))]
        public Guid StaffCoreDocumentGUID { get; set; }


        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
        public string UserGUID { get; set; }


        [Display(Name = "DocumentTypeGUID", ResourceType = typeof(resxDbFields))]
        public string DocumentTypeGUID { get; set; }


        [Display(Name = "DocumentTypeGUID", ResourceType = typeof(resxDbFields))]
        public string DocumentType { get; set; }


        [Display(Name = "DocumentNumber", ResourceType = typeof(resxDbFields))]
        public string DocumentNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DocumentDateOfIssue", ResourceType = typeof(resxDbFields))]
        public DateTime? DocumentDateOfIssue { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        [Display(Name = "DocumentDateOfExpiry", ResourceType = typeof(resxDbFields))]
        public DateTime? DocumentDateOfExpiry { get; set; }

        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataStaffCoreDocumentRowVersion { get; set; }
    }


    public class StaffCoreDocumentUpdateModel
    {
        [Required]
        [Display(Name = "StaffCoreDocumentGUID", ResourceType = typeof(resxDbFields))]
        public Guid StaffCoreDocumentGUID { get; set; }

        [Required]
        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
        public Guid UserGUID { get; set; }

        [Required]
        [Display(Name = "DocumentTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid DocumentTypeGUID { get; set; }


        [Display(Name = "DocumentNumber", ResourceType = typeof(resxDbFields))]
        public string DocumentNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DocumentDateOfIssue", ResourceType = typeof(resxDbFields))]
        public DateTime? DocumentDateOfIssue { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        [Display(Name = "DocumentDateOfExpiry", ResourceType = typeof(resxDbFields))]
        public DateTime? DocumentDateOfExpiry { get; set; }



        [Required]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataStaffCoreDocumentRowVersion { get; set; }
    }


    public class StaffCorePassportDataTableModel
    {
        [Required]
        [Display(Name = "StaffCorePassportGUID", ResourceType = typeof(resxDbFields))]
        public Guid StaffCorePassportGUID { get; set; }

        [Required]
        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
        public string UserGUID { get; set; }

        [Required]
        [Display(Name = "NationalPassportNumber", ResourceType = typeof(resxDbFields))]
        public string NationalPassportNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "NationalPassportDateOfIssue", ResourceType = typeof(resxDbFields))]
        public DateTime? NationalPassportDateOfIssue { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "NationalPassportDateOfExpiry", ResourceType = typeof(resxDbFields))]
        public DateTime? NationalPassportDateOfExpiry { get; set; }

        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataStaffCorePassportRowVersion { get; set; }
    }
    public class StaffCorePassportUpdateModel
    {
        [Required]
        [Display(Name = "StaffCorePassportGUID", ResourceType = typeof(resxDbFields))]
        public Guid StaffCorePassportGUID { get; set; }

        [Required]
        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
        public Guid UserGUID { get; set; }

        [Required]
        [Display(Name = "NationalPassportNumber", ResourceType = typeof(resxDbFields))]
        public string NationalPassportNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "NationalPassportDateOfIssue", ResourceType = typeof(resxDbFields))]
        public DateTime? NationalPassportDateOfIssue { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "NationalPassportDateOfExpiry", ResourceType = typeof(resxDbFields))]
        public DateTime? NationalPassportDateOfExpiry { get; set; }



        [Required]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataStaffCorePassportRowVersion { get; set; }
    }


    public class StaffPhoneUpdateModel
    {
        [Required]
        [Display(Name = "StaffSimGUID", ResourceType = typeof(resxDbFields))]
        public Guid StaffSimGUID { get; set; }

        [Required]
        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
        public Guid UserGUID { get; set; }

        [Required]
        [Display(Name = "PhoneUsageTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid PhoneUsageTypeGUID { get; set; }

        [StringLength(50)]
        [Display(Name = "PhoneNumber", ResourceType = typeof(resxDbFields))]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "FromDate", ResourceType = typeof(resxDbFields))]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "ToDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ToDate { get; set; }

        [Display(Name = "PhoneTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid PhoneTypeGUID { get; set; }

        [Display(Name = "TelecomCompanyOperationGUID", ResourceType = typeof(resxDbFields))]
        public Guid TelecomCompanyOperationGUID { get; set; }

        [Display(Name = "Note", ResourceType = typeof(resxDbFields))]
        public string Note { get; set; }

        [Required]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataStaffPhoneRowVersion { get; set; }
    }

    public class StaffDataQualityModel
    {
        public LocalDBUserModel LocalUser { get; set; }
        public ADUser ADUser { get; set; }
    }
    public class LocalDBUserModel
    {
        public Guid UserGUID { get; set; }
        public string EmailAddress { get; set; }

        public string DutyStationDescriptionEN { get; set; }
        public string DutyStationDescriptionAR { get; set; }

        public string DepartmentDescriptionEN { get; set; }
        public string DepartmentDescriptionAR { get; set; }

        public string JobTitleDescriptionEN { get; set; }
        public string JobTitleDescriptionAR { get; set; }

        public string FirstNameEN { get; set; }
        public string SurnameEN { get; set; }

        public string FirstNameAR { get; set; }
        public string SurnameAR { get; set; }

    }

    public class PhoneDirectoryDataTable
    {
        [Display(Name = "ContactType", ResourceType = typeof(resxDbFields))]
        public string ContactType { get; set; }
        public string EmailAddress { get; set; }


        [Display(Name = "CallSign", ResourceType = typeof(resxDbFields))]
        public string CallSign { get; set; }
        public string PhoneDirectoryGUID { get; set; }

        public string PhoneHolderTypeGUID { get; set; }

        public string FullName { get; set; }
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public string DutyStationGUID { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public string DutyStation { get; set; }

        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
        public string DepartmentGUID { get; set; }

        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
        public string DepartmentDescription { get; set; }


        [Display(Name = "JobTitleGUID", ResourceType = typeof(resxDbFields))]
        public string JobTitleGUID { get; set; }

        [Display(Name = "JobTitleGUID", ResourceType = typeof(resxDbFields))]
        public string JobTitle { get; set; }


        [Display(Name = "DutyStationExtensionNumber", ResourceType = typeof(resxDbFields))]
        public string DutyStationExtensionNumber { get; set; }


        public string UserGUID { get; set; }
        [Display(Name = "SATPhoneNumber", ResourceType = typeof(resxDbFields))]
        public string SATPhoneNumber { get; set; }

        [Display(Name = "JobTitleDescription", ResourceType = typeof(resxDbFields))]
        public string JobTitleDescription { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HomeTelephoneNumberLandline", ResourceType = typeof(resxDbFields))]
        public string HomeTelephoneNumberLandline { get; set; }

        [StringLength(13, MinimumLength = 13, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HomeTelephoneNumberMobile", ResourceType = typeof(resxDbFields))]
        public string HomeTelephoneNumberMobile { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OfficialMobileNumber", ResourceType = typeof(resxDbFields))]
        public string OfficialMobileNumber { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HQExtensionNumber", ResourceType = typeof(resxDbFields))]
        public string HQExtensionNumber { get; set; }



        [Display(Name = "OfficialExtensionNumber", ResourceType = typeof(resxDbFields))]
        public string OfficialExtensionNumber { get; set; }
        public bool Active { get; set; }

        public byte[] dataPhoneDirectoryRowVersion { get; set; }


    }

    public class StaffPhoneDirectoryDataTable
    {
        public string EmailAddress { get; set; }


        public Guid UserGUID { get; set; }
        [Display(Name = "SATPhoneNumber", ResourceType = typeof(resxDbFields))]
        public string SATPhoneNumber { get; set; }

        [Display(Name = "JobTitleDescription", ResourceType = typeof(resxDbFields))]
        public string JobTitleDescription { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HomeTelephoneNumberLandline", ResourceType = typeof(resxDbFields))]
        public string HomeTelephoneNumberLandline { get; set; }

        [StringLength(13, MinimumLength = 13, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HomeTelephoneNumberMobile", ResourceType = typeof(resxDbFields))]
        public string HomeTelephoneNumberMobile { get; set; }

        [StringLength(13, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OfficialMobileNumber", ResourceType = typeof(resxDbFields))]
        public string OfficialMobileNumber { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HQExtensionNumber", ResourceType = typeof(resxDbFields))]
        public string HQExtensionNumber { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DamascusExtensionNumber", ResourceType = typeof(resxDbFields))]
        public string DamascusExtensionNumber { get; set; }

        [Display(Name = "OfficialExtensionNumber", ResourceType = typeof(resxDbFields))]
        public string OfficialExtensionNumber { get; set; }
        public bool Active { get; set; }

        public byte[] StaffCoreDataRowVersion { get; set; }
        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
        public string DepartmentDescription { get; set; }

    }

    public class StaffPhoneDirectoryUpdateModel
    {
        public string EmailAddress { get; set; }


        public Guid UserGUID { get; set; }
        [Display(Name = "SATPhoneNumber", ResourceType = typeof(resxDbFields))]
        public string SATPhoneNumber { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HomeTelephoneNumberLandline", ResourceType = typeof(resxDbFields))]
        public string HomeTelephoneNumberLandline { get; set; }

        [StringLength(13, MinimumLength = 13, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HomeTelephoneNumberMobile", ResourceType = typeof(resxDbFields))]
        public string HomeTelephoneNumberMobile { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OfficialMobileNumber", ResourceType = typeof(resxDbFields))]
        public string OfficialMobileNumber { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HQExtensionNumber", ResourceType = typeof(resxDbFields))]
        public string HQExtensionNumber { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DamascusExtensionNumber", ResourceType = typeof(resxDbFields))]
        public string DamascusExtensionNumber { get; set; }

        [Display(Name = "OfficialExtensionNumber", ResourceType = typeof(resxDbFields))]
        public string OfficialExtensionNumber { get; set; }
        public bool Active { get; set; }

        public byte[] StaffCoreDataRowVersion { get; set; }



    }

    public class PhoneDirectoryUpdateModel
    {

        [Display(Name = "DutyStation", ResourceType = typeof(resxDbFields))]
        public string DutyStation { get; set; }

        [Display(Name = "CallSign", ResourceType = typeof(resxDbFields))]
        public string CallSign { get; set; }
        public Guid? UserGUID { get; set; }
        [Display(Name = "SATPhoneNumber", ResourceType = typeof(resxDbFields))]
        public string SATPhoneNumber { get; set; }

        [Display(Name = "JobTitleDescription", ResourceType = typeof(resxDbFields))]
        public string JobTitleDescription { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HomeTelephoneNumberLandline", ResourceType = typeof(resxDbFields))]
        public string HomeTelephoneNumberLandline { get; set; }

        [StringLength(13, MinimumLength = 13, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HomeTelephoneNumberMobile", ResourceType = typeof(resxDbFields))]
        public string HomeTelephoneNumberMobile { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OfficialMobileNumber", ResourceType = typeof(resxDbFields))]
        public string OfficialMobileNumber { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HQExtensionNumber", ResourceType = typeof(resxDbFields))]
        public string HQExtensionNumber { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DamascusExtensionNumber", ResourceType = typeof(resxDbFields))]
        public string DamascusExtensionNumber { get; set; }

        [Display(Name = "OfficialExtensionNumber", ResourceType = typeof(resxDbFields))]
        public string OfficialExtensionNumber { get; set; }
        public bool Active { get; set; }

        public byte[] StaffCoreDataRowVersion { get; set; }
        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
        public string DepartmentDescription { get; set; }



        public Guid PhoneDirectoryGUID { get; set; }
        public Nullable<System.Guid> PhoneHolderTypeGUID { get; set; }

        public string FullName { get; set; }
        public Nullable<System.Guid> DutyStationGUID { get; set; }
        public Nullable<System.Guid> DepartmentGUID { get; set; }
        public string EmailAddress { get; set; }

        public string DutyStationExtensionNumber { get; set; }

        public byte[] dataPhoneDirectoryRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
        public Nullable<System.Guid> JobTitleGUID { get; set; }

    }

    public class STAFFWarehouseModelEntryMovementDataTableModel
    {



        [Display(Name = "DeliveryStatus", ResourceType = typeof(resxDbFields))]
        public string DeliveryStatus { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }




        public byte[] dataItemInputDetailRowVersion { get; set; }





        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]

        [Display(Name = "ModelDescription", ResourceType = typeof(resxDbFields))]
        public string ModelDescription { get; set; }

        [Display(Name = "ItemDescription", ResourceType = typeof(resxDbFields))]
        public string ItemDescription { get; set; }

        [StringLength(50, MinimumLength = 5, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BarcodeNumber", ResourceType = typeof(resxDbFields))]
        public string BarcodeNumber { get; set; }
        [Display(Name = "SerialNumber", ResourceType = typeof(resxDbFields))]
        [StringLength(50, MinimumLength = 5, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public string SerialNumber { get; set; }

        [Display(Name = "IME1", ResourceType = typeof(resxDbFields))]
        [StringLength(500, MinimumLength = 5, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public string IME1 { get; set; }

        [Display(Name = "GSM", ResourceType = typeof(resxDbFields))]
        [StringLength(500, MinimumLength = 5, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public string GSM { get; set; }

        [Display(Name = "MSRPID", ResourceType = typeof(resxDbFields))]
        [StringLength(500, MinimumLength = 5, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public string MSRPID { get; set; }
        public string MAC { get; set; }



        [Display(Name = "GSMNumber", ResourceType = typeof(resxDbFields))]
        public string GSMNumber { get; set; }



        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }



    }

    public class STAFFGrantedSoftwareApplicationDataTableModel
    {



        [Display(Name = "ApplicationName", ResourceType = typeof(resxDbFields))]
        public string ApplicationName { get; set; }


        public string ApplicationLanguageGUID { get; set; }

        public string ApplicationGUID { get; set; }


        public byte[] codeApplicationsLanguagesRowVersion { get; set; }






        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }



    }

    #region Online Training
    public class StaffOnlineTrainingDataTableModel
    {

        [Display(Name = "StaffOnlineTrainingGUID", ResourceType = typeof(resxDbFields))]
        public Guid StaffOnlineTrainingGUID { get; set; }


        [Display(Name = "OnlineTrainingTypeGUID", ResourceType = typeof(resxDbFields))]
        public string OnlineTrainingTypeGUID { get; set; }



        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]

        public string UserGUID { get; set; }

        [Display(Name = "TrainingName", ResourceType = typeof(resxDbFields))]
        public string TrainingName { get; set; }


        [Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }


        [Display(Name = "ExpiryDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }


        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public string CreatedByGUID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreateDate { get; set; }


        [Display(Name = "FlowStatus", ResourceType = typeof(resxDbFields))]

        public string TrainingStatusGUID { get; set; }


        [Display(Name = "TrainingStatusGUID", ResourceType = typeof(resxDbFields))]

        public string StatusName { get; set; }

        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataStaffOnlineTrainingRowVersion { get; set; }
    }

    public class StaffOnlineTrainingUpdateModel
    {

        [Display(Name = "StaffOnlineTrainingGUID", ResourceType = typeof(resxDbFields))]
        public Guid StaffOnlineTrainingGUID { get; set; }


        [Display(Name = "OnlineTrainingTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? OnlineTrainingTypeGUID { get; set; }


        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]

        public Guid? UserGUID { get; set; }


        [Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }


        [Display(Name = "ExpiryDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }


        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? CreatedByGUID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreateDate { get; set; }


        [Display(Name = "TrainingStatusGUID", ResourceType = typeof(resxDbFields))]

        public Guid? TrainingStatusGUID { get; set; }
        [Display(Name = "TrainingStatusGUID", ResourceType = typeof(resxDbFields))]

        public string StatusName { get; set; }


        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]

        public string Comments { get; set; }
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataStaffOnlineTrainingRowVersion { get; set; }
    }
    #endregion


    #region Staff Relative
    public class StaffRelativeDataTableModel
    {

        [Display(Name = "StaffRelativeGUID", ResourceType = typeof(resxDbFields))]
        public Guid StaffRelativeGUID { get; set; }


        [Display(Name = "RelativeTypeGUID", ResourceType = typeof(resxDbFields))]
        public string RelativeTypeGUID { get; set; }


        [Display(Name = "RelativeTypeGUID", ResourceType = typeof(resxDbFields))]
        public string RelativeType { get; set; }

        [Display(Name = "Phone", ResourceType = typeof(resxDbFields))]
        public string Phone { get; set; }


        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
        public string UserGUID { get; set; }



        [Display(Name = "RelativeName", ResourceType = typeof(resxDbFields))]
        public string RelativeName { get; set; }


        [Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]

        public DateTime? StartDate { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]

        public string Comments { get; set; }





        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public string CreatedByGUID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreateDate { get; set; }



        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataStaffRelativeRowVersion { get; set; }
    }

    public class StaffRelativeUpdateModel
    {

        [Display(Name = "StaffRelativeGUID", ResourceType = typeof(resxDbFields))]
        public Guid StaffRelativeGUID { get; set; }


        [Display(Name = "RelativeTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? RelativeTypeGUID { get; set; }



        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
        public Guid? UserGUID { get; set; }



        [Display(Name = "RelativeName", ResourceType = typeof(resxDbFields))]
        public string RelativeName { get; set; }

        [Display(Name = "Phone", ResourceType = typeof(resxDbFields))]

        public string Phone { get; set; }


        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]

        public string Comments { get; set; }




        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? CreatedByGUID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreateDate { get; set; }



        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataStaffRelativeRowVersion { get; set; }
    }
    #endregion

    #region Staff Service Provided

    public class StaffServiceProvidedDataTableModel
    {

        [Display(Name = "StaffServiceProvidedGUID", ResourceType = typeof(resxDbFields))]
        public Guid StaffServiceProvidedGUID { get; set; }

        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
        public string UserGUID { get; set; }


        [Display(Name = "ServiceTypeGUID", ResourceType = typeof(resxDbFields))]
        public string ServiceTypeGUID { get; set; }

        [Display(Name = "ServiceStatus", ResourceType = typeof(resxDbFields))]
        public string ServiceStatus { get; set; }
        [Display(Name = "ServiceName", ResourceType = typeof(resxDbFields))]
        public string ServiceName { get; set; }

        [Display(Name = "ServiceStatus", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusGUID { get; set; }

        [Display(Name = "CancelledBy", ResourceType = typeof(resxDbFields))]
        public string CancelledBy { get; set; }
        [Display(Name = "ActivatedBy", ResourceType = typeof(resxDbFields))]
        public string ActivatedBy { get; set; }

        [Display(Name = "ActivatedDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ActivatedDate { get; set; }


        [Display(Name = "CancelledDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CancelledDate { get; set; }




        [Display(Name = "ServiceStatus", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusName { get; set; }

        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public string CreateBy { get; set; }


        [Display(Name = "UpdateByGUID", ResourceType = typeof(resxDbFields))]
        public string UpdateBy { get; set; }



        [Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]

        public DateTime? StartDate { get; set; }


        [Display(Name = "ExpiryDate", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]

        public DateTime? ExpiryDate { get; set; }



        [Display(Name = "UpdateByGUID", ResourceType = typeof(resxDbFields))]

        public string UpdateByGUID { get; set; }



        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public string CreatedByGUID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]

        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreateDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]

        [Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? UpdateDate { get; set; }


        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]

        public string Comments { get; set; }


        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataStaffServiceProvidedRowVersion { get; set; }
    }

    public class StaffServiceProvidedUpdateModel
    {

        [Display(Name = "StaffServiceProvidedGUID", ResourceType = typeof(resxDbFields))]
        public Guid StaffServiceProvidedGUID { get; set; }
        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
        public Guid? UserGUID { get; set; }

        [Display(Name = "ServiceTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ServiceTypeGUID { get; set; }


        [Display(Name = "ServiceName", ResourceType = typeof(resxDbFields))]
        public string ServiceName { get; set; }

        [Display(Name = "LastFlowStatusGUID", ResourceType = typeof(resxDbFields))]
        public Guid? LastFlowStatusGUID { get; set; }


        [Display(Name = "LastFlowStatusName", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusName { get; set; }




        [Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]

        public DateTime? StartDate { get; set; }


        [Display(Name = "ExpiryDate", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]

        public DateTime? ExpiryDate { get; set; }



        [Display(Name = "UpdateByGUID", ResourceType = typeof(resxDbFields))]

        public Guid? UpdateByGUID { get; set; }



        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? CreateByGUID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]

        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreateDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]

        [Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? UpdateDate { get; set; }


        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]

        public string Comments { get; set; }


        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataStaffServiceProvidedRowVersion { get; set; }
    }
    #endregion


    public class ADUser
    {
        public string sn { get; set; }
        public string samaccountname { get; set; }
        public string mail { get; set; }
        public string displayname { get; set; }
        public string telephoneNumber { get; set; }
        public string title { get; set; }
        public string department { get; set; }
        public string givenName { get; set; }
        public string co { get; set; }
        public string l { get; set; }
        public string company { get; set; }
        public string memberOf { get; set; }
        public string firstName { get; set; }
        public string surName { get; set; }

    }
    public class StaffOrgChart
    {
        public StaffOrgChart()
        {
            tags = new List<string>();

        }
        public string id { get; set; }
        public string pid { get; set; }
        public List<string> tags { get; set; }

        public string stpid { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string visa { get; set; }
        public string img { get; set; }
    }

}
