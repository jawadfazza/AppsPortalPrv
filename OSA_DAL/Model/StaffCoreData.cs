
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OSA.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class StaffCoreData
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid UserGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[DataType(DataType.EmailAddress)]
    	[StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "EmailAddress", ResourceType = typeof(resxDbFields))]
    	public string EmailAddress{ get; set; }
    	
        [Display(Name = "IsInternational", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> IsInternational{ get; set; }
    	
        [Display(Name = "StaffPhoto", ResourceType = typeof(resxDbFields))]
    	public string StaffPhoto{ get; set; }
    	
        [Display(Name = "PositionInOrganigram", ResourceType = typeof(resxDbFields))]
    	public string PositionInOrganigram{ get; set; }
    	
        [Display(Name = "JobTitleGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> JobTitleGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid DutyStationGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid DepartmentGUID{ get; set; }
    	
        [Display(Name = "ReportToGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ReportToGUID{ get; set; }
    	
        [Display(Name = "NationalityGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> NationalityGUID{ get; set; }
    	
        [Display(Name = "Nationality2GUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> Nationality2GUID{ get; set; }
    	
        [Display(Name = "Nationality3GUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> Nationality3GUID{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "ExpiryOfResidencyVisa", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> ExpiryOfResidencyVisa{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "ExpiryOfResidencyVisa", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalExpiryOfResidencyVisa { get { return new Portal().LocalTime(this.ExpiryOfResidencyVisa); } }
    	
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "SyrianNationalIDNumber", ResourceType = typeof(resxDbFields))]
    	public string SyrianNationalIDNumber{ get; set; }
    	
        [Display(Name = "SyrianNationalIDPhoto", ResourceType = typeof(resxDbFields))]
    	public string SyrianNationalIDPhoto{ get; set; }
    	
        [StringLength(10, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "StaffPrefix", ResourceType = typeof(resxDbFields))]
    	public string StaffPrefix{ get; set; }
    	
        [Display(Name = "PlaceOfBirthGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> PlaceOfBirthGUID{ get; set; }
    	
        [Display(Name = "EmploymentID", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> EmploymentID{ get; set; }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "UNHCRIDNumber", ResourceType = typeof(resxDbFields))]
    	public string UNHCRIDNumber{ get; set; }
    	
        [Display(Name = "CallSign", ResourceType = typeof(resxDbFields))]
    	public string CallSign{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "StaffLastWork", ResourceType = typeof(resxDbFields))]
    	public string StaffLastWork{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "UNLPNumber", ResourceType = typeof(resxDbFields))]
    	public string UNLPNumber{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "UNLPDateOfIssue", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> UNLPDateOfIssue{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "UNLPDateOfIssue", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalUNLPDateOfIssue { get { return new Portal().LocalTime(this.UNLPDateOfIssue); } }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "UNLPDateOfExpiry", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> UNLPDateOfExpiry{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "UNLPDateOfExpiry", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalUNLPDateOfExpiry { get { return new Portal().LocalTime(this.UNLPDateOfExpiry); } }
    	
        [Display(Name = "UNLPPhoto", ResourceType = typeof(resxDbFields))]
    	public string UNLPPhoto{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "NationalPassportNumber", ResourceType = typeof(resxDbFields))]
    	public string NationalPassportNumber{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "NationalPassportDateOfIssue", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> NationalPassportDateOfIssue{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "NationalPassportDateOfIssue", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalNationalPassportDateOfIssue { get { return new Portal().LocalTime(this.NationalPassportDateOfIssue); } }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "NationalPassportDateOfExpiry", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> NationalPassportDateOfExpiry{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "NationalPassportDateOfExpiry", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalNationalPassportDateOfExpiry { get { return new Portal().LocalTime(this.NationalPassportDateOfExpiry); } }
    	
        [Display(Name = "IsEligibileForNationalDangerPay", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> IsEligibileForNationalDangerPay{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "NationalPassportNumber2", ResourceType = typeof(resxDbFields))]
    	public string NationalPassportNumber2{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "NationalPassportDateOfIssue2", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> NationalPassportDateOfIssue2{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "NationalPassportDateOfIssue2", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalNationalPassportDateOfIssue2 { get { return new Portal().LocalTime(this.NationalPassportDateOfIssue2); } }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "NationalPassportDateOfExpiry2", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> NationalPassportDateOfExpiry2{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "NationalPassportDateOfExpiry2", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalNationalPassportDateOfExpiry2 { get { return new Portal().LocalTime(this.NationalPassportDateOfExpiry2); } }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "NationalPassportNumber3", ResourceType = typeof(resxDbFields))]
    	public string NationalPassportNumber3{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "NationalPassportDateOfIssue3", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> NationalPassportDateOfIssue3{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "NationalPassportDateOfIssue3", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalNationalPassportDateOfIssue3 { get { return new Portal().LocalTime(this.NationalPassportDateOfIssue3); } }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "NationalPassportDateOfExpiry3", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> NationalPassportDateOfExpiry3{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "NationalPassportDateOfExpiry3", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalNationalPassportDateOfExpiry3 { get { return new Portal().LocalTime(this.NationalPassportDateOfExpiry3); } }
    	
        [Display(Name = "NationalPassportPhoto", ResourceType = typeof(resxDbFields))]
    	public string NationalPassportPhoto{ get; set; }
    	
        [Display(Name = "BankLebNameGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> BankLebNameGUID{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankNameARLeb", ResourceType = typeof(resxDbFields))]
    	public string BankNameARLeb{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankAccountHolderNameLebEN", ResourceType = typeof(resxDbFields))]
    	public string BankAccountHolderNameLebEN{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankAccountHolderNameLebAR", ResourceType = typeof(resxDbFields))]
    	public string BankAccountHolderNameLebAR{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankNameLeb", ResourceType = typeof(resxDbFields))]
    	public string BankNameLeb{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankAccountNumberLeb", ResourceType = typeof(resxDbFields))]
    	public string BankAccountNumberLeb{ get; set; }
    	
        [Display(Name = "BankNameSyrGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> BankNameSyrGUID{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankNameSyr", ResourceType = typeof(resxDbFields))]
    	public string BankNameSyr{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankAccountNumberSyr", ResourceType = typeof(resxDbFields))]
    	public string BankAccountNumberSyr{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankAccountHolderNameEn", ResourceType = typeof(resxDbFields))]
    	public string BankAccountHolderNameEn{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankAccountHolderNameAr", ResourceType = typeof(resxDbFields))]
    	public string BankAccountHolderNameAr{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankSYRAccountNumberSyr2", ResourceType = typeof(resxDbFields))]
    	public string BankSYRAccountNumberSyr2{ get; set; }
    	
        [Display(Name = "BankNameSyrGUID2", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> BankNameSyrGUID2{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankSYRNameEN2", ResourceType = typeof(resxDbFields))]
    	public string BankSYRNameEN2{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankSYRNameAR2", ResourceType = typeof(resxDbFields))]
    	public string BankSYRNameAR2{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankSYRAccountHolderNameEn2", ResourceType = typeof(resxDbFields))]
    	public string BankSYRAccountHolderNameEn2{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankSYRAccountHolderNameAr2", ResourceType = typeof(resxDbFields))]
    	public string BankSYRAccountHolderNameAr2{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankSYRAccountNumberSyr3", ResourceType = typeof(resxDbFields))]
    	public string BankSYRAccountNumberSyr3{ get; set; }
    	
        [Display(Name = "BankSYRNameGUID3", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> BankSYRNameGUID3{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankSYRNameEN3", ResourceType = typeof(resxDbFields))]
    	public string BankSYRNameEN3{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankSYRNameAR3", ResourceType = typeof(resxDbFields))]
    	public string BankSYRNameAR3{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankSYRAccountHolderNameEn3", ResourceType = typeof(resxDbFields))]
    	public string BankSYRAccountHolderNameEn3{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankSYRAccountHolderNameAr3", ResourceType = typeof(resxDbFields))]
    	public string BankSYRAccountHolderNameAr3{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankSYRAccountNumberSyr4", ResourceType = typeof(resxDbFields))]
    	public string BankSYRAccountNumberSyr4{ get; set; }
    	
        [Display(Name = "BankSYRNameGUID4", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> BankSYRNameGUID4{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankSYRNameEN4", ResourceType = typeof(resxDbFields))]
    	public string BankSYRNameEN4{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankSYRNameAR4", ResourceType = typeof(resxDbFields))]
    	public string BankSYRNameAR4{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankSYRAccountHolderNameEn4", ResourceType = typeof(resxDbFields))]
    	public string BankSYRAccountHolderNameEn4{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankSYRAccountHolderNameAr4", ResourceType = typeof(resxDbFields))]
    	public string BankSYRAccountHolderNameAr4{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankSYRAccountNumberSyr5", ResourceType = typeof(resxDbFields))]
    	public string BankSYRAccountNumberSyr5{ get; set; }
    	
        [Display(Name = "BankSYRNameGUID5", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> BankSYRNameGUID5{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankSYRNameEN5", ResourceType = typeof(resxDbFields))]
    	public string BankSYRNameEN5{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankSYRNameAR5", ResourceType = typeof(resxDbFields))]
    	public string BankSYRNameAR5{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankSYRAccountHolderNameEn5", ResourceType = typeof(resxDbFields))]
    	public string BankSYRAccountHolderNameEn5{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BankSYRAccountHolderNameAr5", ResourceType = typeof(resxDbFields))]
    	public string BankSYRAccountHolderNameAr5{ get; set; }
    	
        [Display(Name = "ContractTypeGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ContractTypeGUID{ get; set; }
    	
        [Display(Name = "RecruitmentTypeGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> RecruitmentTypeGUID{ get; set; }
    	
        [Display(Name = "StaffGradeGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> StaffGradeGUID{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "ContractStartDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> ContractStartDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "ContractStartDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalContractStartDate { get { return new Portal().LocalTime(this.ContractStartDate); } }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "ContractEndDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> ContractEndDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "ContractEndDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalContractEndDate { get { return new Portal().LocalTime(this.ContractEndDate); } }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "StaffEOD", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> StaffEOD{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "StaffEOD", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalStaffEOD { get { return new Portal().LocalTime(this.StaffEOD); } }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PositionNumber", ResourceType = typeof(resxDbFields))]
    	public string PositionNumber{ get; set; }
    	
        [Display(Name = "PermanentAddressEn", ResourceType = typeof(resxDbFields))]
    	public string PermanentAddressEn{ get; set; }
    	
        [Display(Name = "PermanentAddressAr", ResourceType = typeof(resxDbFields))]
    	public string PermanentAddressAr{ get; set; }
    	
        [Display(Name = "CurrentAddressEn", ResourceType = typeof(resxDbFields))]
    	public string CurrentAddressEn{ get; set; }
    	
        [Display(Name = "CurrentAddressAr", ResourceType = typeof(resxDbFields))]
    	public string CurrentAddressAr{ get; set; }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "HomeTelephoneNumberLandline", ResourceType = typeof(resxDbFields))]
    	public string HomeTelephoneNumberLandline{ get; set; }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "HomeTelephoneNumberMobile", ResourceType = typeof(resxDbFields))]
    	public string HomeTelephoneNumberMobile{ get; set; }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OfficialMobileNumber", ResourceType = typeof(resxDbFields))]
    	public string OfficialMobileNumber{ get; set; }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OfficialExtensionNumber", ResourceType = typeof(resxDbFields))]
    	public string OfficialExtensionNumber{ get; set; }
    	
        [Display(Name = "OfficeGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> OfficeGUID{ get; set; }
    	
        [Display(Name = "OfficeFloorGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> OfficeFloorGUID{ get; set; }
    	
        [Display(Name = "OfficeRoomGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> OfficeRoomGUID{ get; set; }
    	
        [Display(Name = "IsOld", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> IsOld{ get; set; }
    	
        [Display(Name = "Gender", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> Gender{ get; set; }
    	
        [Display(Name = "IDNumber", ResourceType = typeof(resxDbFields))]
    	public string IDNumber{ get; set; }
    	
        [Display(Name = "PersonalIDPhoto", ResourceType = typeof(resxDbFields))]
    	public string PersonalIDPhoto{ get; set; }
    	
        [Display(Name = "PassportPhoto", ResourceType = typeof(resxDbFields))]
    	public string PassportPhoto{ get; set; }
    	
        [Display(Name = "AttendancePhoto", ResourceType = typeof(resxDbFields))]
    	public string AttendancePhoto{ get; set; }
    	
        [Display(Name = "DangerPay", ResourceType = typeof(resxDbFields))]
    	public string DangerPay{ get; set; }
    	
        [Display(Name = "WifeHusbandName", ResourceType = typeof(resxDbFields))]
    	public string WifeHusbandName{ get; set; }
    	
        [Display(Name = "AssignmentDuration", ResourceType = typeof(resxDbFields))]
    	public string AssignmentDuration{ get; set; }
    	
        [Display(Name = "BankAccountNumber", ResourceType = typeof(resxDbFields))]
    	public string BankAccountNumber{ get; set; }
    	
        [Display(Name = "CurrentLocationByDate", ResourceType = typeof(resxDbFields))]
    	public string CurrentLocationByDate{ get; set; }
    	
        [Display(Name = "StaffMemberOnGTA", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> StaffMemberOnGTA{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "GTAStartDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> GTAStartDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "GTAStartDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalGTAStartDate { get { return new Portal().LocalTime(this.GTAStartDate); } }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "GTAEndDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> GTAEndDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "GTAEndDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalGTAEndDate { get { return new Portal().LocalTime(this.GTAEndDate); } }
    	
        [Display(Name = "Accommodation", ResourceType = typeof(resxDbFields))]
    	public string Accommodation{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "AccommodationStartingDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> AccommodationStartingDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "AccommodationStartingDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalAccommodationStartingDate { get { return new Portal().LocalTime(this.AccommodationStartingDate); } }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "StartOfMissionAssignmentDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> StartOfMissionAssignmentDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "StartOfMissionAssignmentDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalStartOfMissionAssignmentDate { get { return new Portal().LocalTime(this.StartOfMissionAssignmentDate); } }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "EndOfMissionAssignmentDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> EndOfMissionAssignmentDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "EndOfMissionAssignmentDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalEndOfMissionAssignmentDate { get { return new Portal().LocalTime(this.EndOfMissionAssignmentDate); } }
    	
        [Display(Name = "DetailsOfMissionAssignment", ResourceType = typeof(resxDbFields))]
    	public string DetailsOfMissionAssignment{ get; set; }
    	
        [Display(Name = "DurationOfStayInSyria", ResourceType = typeof(resxDbFields))]
    	public string DurationOfStayInSyria{ get; set; }
    	
        [Display(Name = "EmploymentType", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> EmploymentType{ get; set; }
    	
        [Display(Name = "StaffAccessCategoryGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> StaffAccessCategoryGUID{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DateOfBirth", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DateOfBirth{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DateOfBirth", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDateOfBirth { get { return new Portal().LocalTime(this.DateOfBirth); } }
    	
        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PlaceBirthCityEn", ResourceType = typeof(resxDbFields))]
    	public string PlaceBirthCityEn{ get; set; }
    	
        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PlaceBirthCityAr", ResourceType = typeof(resxDbFields))]
    	public string PlaceBirthCityAr{ get; set; }
    	
        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "LastJobEn", ResourceType = typeof(resxDbFields))]
    	public string LastJobEn{ get; set; }
    	
        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "LastJobAr", ResourceType = typeof(resxDbFields))]
    	public string LastJobAr{ get; set; }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "SATPhoneNumber", ResourceType = typeof(resxDbFields))]
    	public string SATPhoneNumber{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OfficeRoomNumberBuilding", ResourceType = typeof(resxDbFields))]
    	public string OfficeRoomNumberBuilding{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DegreeAr", ResourceType = typeof(resxDbFields))]
    	public string DegreeAr{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DegreeEn", ResourceType = typeof(resxDbFields))]
    	public string DegreeEn{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "ReturnDateFromLastRAndRLeave", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> ReturnDateFromLastRAndRLeave{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "ReturnDateFromLastRAndRLeave", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalReturnDateFromLastRAndRLeave { get { return new Portal().LocalTime(this.ReturnDateFromLastRAndRLeave); } }
    	
        [Display(Name = "StaffStatus", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> StaffStatus{ get; set; }
    	
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BloodGroup", ResourceType = typeof(resxDbFields))]
    	public string BloodGroup{ get; set; }
    	
        [Display(Name = "LastUpdatedByGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> LastUpdatedByGUID{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "LastUpdatedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LastUpdatedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "LastUpdatedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalLastUpdatedOn { get { return new Portal().LocalTime(this.LastUpdatedOn); } }
    	
        [Display(Name = "LastConfirmedByGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> LastConfirmedByGUID{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "LastConfirmedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LastConfirmedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "LastConfirmedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalLastConfirmedOn { get { return new Portal().LocalTime(this.LastConfirmedOn); } }
    	
        [Display(Name = "ConfirmationStatusGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ConfirmationStatusGUID{ get; set; }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ConfirmationComment", ResourceType = typeof(resxDbFields))]
    	public string ConfirmationComment{ get; set; }
    	
        [Display(Name = "StaffStatusGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> StaffStatusGUID{ get; set; }
    	
        [Display(Name = "IsOldRecord", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> IsOldRecord{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] StaffCoreDataRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
        [Display(Name = "StaffPrefixGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> StaffPrefixGUID{ get; set; }
    	
        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TitleEnglishMSRP", ResourceType = typeof(resxDbFields))]
    	public string TitleEnglishMSRP{ get; set; }
    	
        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TitleArabicMSRP", ResourceType = typeof(resxDbFields))]
    	public string TitleArabicMSRP{ get; set; }
    	
        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TitleEnglishMOFA", ResourceType = typeof(resxDbFields))]
    	public string TitleEnglishMOFA{ get; set; }
    	
        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TitleArabicMOFA", ResourceType = typeof(resxDbFields))]
    	public string TitleArabicMOFA{ get; set; }
    	
        [DataType(DataType.EmailAddress)]
    	[StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PersonalEmail", ResourceType = typeof(resxDbFields))]
    	public string PersonalEmail{ get; set; }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "HQExtensionNumber", ResourceType = typeof(resxDbFields))]
    	public string HQExtensionNumber{ get; set; }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DamascusExtensionNumber", ResourceType = typeof(resxDbFields))]
    	public string DamascusExtensionNumber{ get; set; }
    	
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "LastConfirmationStatus", ResourceType = typeof(resxDbFields))]
    	public string LastConfirmationStatus{ get; set; }
    	
        [Display(Name = "LastConfirmationStatusGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> LastConfirmationStatusGUID{ get; set; }
    	
        [Display(Name = "OfficeTypeGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> OfficeTypeGUID{ get; set; }
    	
        [StringLength(2000, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ICTComments", ResourceType = typeof(resxDbFields))]
    	public string ICTComments{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "NextOfKinName", ResourceType = typeof(resxDbFields))]
    	public string NextOfKinName{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "KinMobileNumber", ResourceType = typeof(resxDbFields))]
    	public string KinMobileNumber{ get; set; }
    	
        [Display(Name = "BSAFECertAcquired", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> BSAFECertAcquired{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "BSAFEExpiryDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> BSAFEExpiryDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "BSAFEExpiryDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalBSAFEExpiryDate { get { return new Portal().LocalTime(this.BSAFEExpiryDate); } }
    	
        [Display(Name = "NumberOfDependants", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> NumberOfDependants{ get; set; }
    	
        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DependantsName", ResourceType = typeof(resxDbFields))]
    	public string DependantsName{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "LastDepartureDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LastDepartureDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "LastDepartureDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalLastDepartureDate { get { return new Portal().LocalTime(this.LastDepartureDate); } }
    	
        [Display(Name = "StaffPositionGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> StaffPositionGUID{ get; set; }
    	
        [Display(Name = "ICTServiceProvidedCount", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> ICTServiceProvidedCount{ get; set; }
    	
        [Display(Name = "VendorID", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> VendorID{ get; set; }
    	
        [Display(Name = "PaymentEligibilityStatusGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> PaymentEligibilityStatusGUID{ get; set; }
    	
        [Display(Name = "Photo", ResourceType = typeof(resxDbFields))]
    	public byte[] Photo{ get; set; }
    	
        [Display(Name = "WorkingOutsideSyriaGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> WorkingOutsideSyriaGUID{ get; set; }
    	
        [Display(Name = "HasToDeductFSHBreakfastFromEntitlementGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> HasToDeductFSHBreakfastFromEntitlementGUID{ get; set; }
    	
        [StringLength(1000, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "StaffFunctionality", ResourceType = typeof(resxDbFields))]
    	public string StaffFunctionality{ get; set; }
    	
        [Display(Name = "HasFixedOfficalNumber", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> HasFixedOfficalNumber{ get; set; }
    	
        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> OrganizationInstanceGUID{ get; set; }
    	
    }
}
