
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AHD_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class v_StaffProfileInformation
    {
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "FirstName", ResourceType = typeof(resxDbFields))]
    	public string FirstName{ get; set; }
    	
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "SurName", ResourceType = typeof(resxDbFields))]
    	public string SurName{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(101, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "FullName", ResourceType = typeof(resxDbFields))]
    	public string FullName{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[DataType(DataType.EmailAddress)]
    	[StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "EmailAddress", ResourceType = typeof(resxDbFields))]
    	public string EmailAddress{ get; set; }
    	
        [Display(Name = "EmploymentID", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> EmploymentID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Gender", ResourceType = typeof(resxDbFields))]
    	public string Gender{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "GenderGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid GenderGUID{ get; set; }
    	
        [StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Grade", ResourceType = typeof(resxDbFields))]
    	public string Grade{ get; set; }
    	
        [Display(Name = "GradeGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> GradeGUID{ get; set; }
    	
        [Display(Name = "GradeID", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> GradeID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid UserGUID{ get; set; }
    	
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DutyStation", ResourceType = typeof(resxDbFields))]
    	public string DutyStation{ get; set; }
    	
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> DutyStationGUID{ get; set; }
    	
        [StringLength(512, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "JobTitle", ResourceType = typeof(resxDbFields))]
    	public string JobTitle{ get; set; }
    	
        [Display(Name = "JobTitleGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> JobTitleGUID{ get; set; }
    	
        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> CountryGUID{ get; set; }
    	
        [StringLength(150, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Nationality", ResourceType = typeof(resxDbFields))]
    	public string Nationality{ get; set; }
    	
        [StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "RecruitmentType", ResourceType = typeof(resxDbFields))]
    	public string RecruitmentType{ get; set; }
    	
        [Display(Name = "RecruitmentTypeGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> RecruitmentTypeGUID{ get; set; }
    	
        [StringLength(150, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DepartmentDescription", ResourceType = typeof(resxDbFields))]
    	public string DepartmentDescription{ get; set; }
    	
        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> DepartmentGUID{ get; set; }
    	
        [StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "StaffStatus", ResourceType = typeof(resxDbFields))]
    	public string StaffStatus{ get; set; }
    	
        [Display(Name = "StaffStatusGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> StaffStatusGUID{ get; set; }
    	
        [StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OtherPositionName", ResourceType = typeof(resxDbFields))]
    	public string OtherPositionName{ get; set; }
    	
        [StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "StaffPrefix", ResourceType = typeof(resxDbFields))]
    	public string StaffPrefix{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "LastDepartureDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LastDepartureDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "LastDepartureDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalLastDepartureDate { get { return new Portal().LocalTime(this.LastDepartureDate); } }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(101, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "SupervisorName", ResourceType = typeof(resxDbFields))]
    	public string SupervisorName{ get; set; }
    	
        [Display(Name = "Age", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> Age{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "StaffEOD", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> StaffEOD{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "StaffEOD", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalStaffEOD { get { return new Portal().LocalTime(this.StaffEOD); } }
    	
        [StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ContractType", ResourceType = typeof(resxDbFields))]
    	public string ContractType{ get; set; }
    	
        [Display(Name = "ContractTypeGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ContractTypeGUID{ get; set; }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "SATPhoneNumber", ResourceType = typeof(resxDbFields))]
    	public string SATPhoneNumber{ get; set; }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OfficialMobileNumber", ResourceType = typeof(resxDbFields))]
    	public string OfficialMobileNumber{ get; set; }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OfficialExtensionNumber", ResourceType = typeof(resxDbFields))]
    	public string OfficialExtensionNumber{ get; set; }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "HQExtensionNumber", ResourceType = typeof(resxDbFields))]
    	public string HQExtensionNumber{ get; set; }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DamascusExtensionNumber", ResourceType = typeof(resxDbFields))]
    	public string DamascusExtensionNumber{ get; set; }
    	
        [Display(Name = "CallSign", ResourceType = typeof(resxDbFields))]
    	public string CallSign{ get; set; }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "HomeTelephoneNumberLandline", ResourceType = typeof(resxDbFields))]
    	public string HomeTelephoneNumberLandline{ get; set; }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PrivateMobileNumber", ResourceType = typeof(resxDbFields))]
    	public string PrivateMobileNumber{ get; set; }
    	
        [Display(Name = "PermanentAddressEn", ResourceType = typeof(resxDbFields))]
    	public string PermanentAddressEn{ get; set; }
    	
        [StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "LastConfirmationStatus", ResourceType = typeof(resxDbFields))]
    	public string LastConfirmationStatus{ get; set; }
    	
        [Display(Name = "LastConfirmationStatusGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> LastConfirmationStatusGUID{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "ContractEndDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> ContractEndDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "ContractEndDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalContractEndDate { get { return new Portal().LocalTime(this.ContractEndDate); } }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "UNLPDateOfExpiry", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> UNLPDateOfExpiry{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "UNLPDateOfExpiry", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalUNLPDateOfExpiry { get { return new Portal().LocalTime(this.UNLPDateOfExpiry); } }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "UNLPDateOfIssue", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> UNLPDateOfIssue{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "UNLPDateOfIssue", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalUNLPDateOfIssue { get { return new Portal().LocalTime(this.UNLPDateOfIssue); } }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DutyStationExtensionNumber", ResourceType = typeof(resxDbFields))]
    	public string DutyStationExtensionNumber{ get; set; }
    	
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "FatherName", ResourceType = typeof(resxDbFields))]
    	public string FatherName{ get; set; }
    	
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "MotherName", ResourceType = typeof(resxDbFields))]
    	public string MotherName{ get; set; }
    	
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ArFirstName", ResourceType = typeof(resxDbFields))]
    	public string ArFirstName{ get; set; }
    	
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ArSurname", ResourceType = typeof(resxDbFields))]
    	public string ArSurname{ get; set; }
    	
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ArMotherName", ResourceType = typeof(resxDbFields))]
    	public string ArMotherName{ get; set; }
    	
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ArFatherName", ResourceType = typeof(resxDbFields))]
    	public string ArFatherName{ get; set; }
    	
        [Display(Name = "WorkingOutsideSyriaGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> WorkingOutsideSyriaGUID{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OfficeRoomNumberBuilding", ResourceType = typeof(resxDbFields))]
    	public string OfficeRoomNumberBuilding{ get; set; }
    	
        [StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OfficeName", ResourceType = typeof(resxDbFields))]
    	public string OfficeName{ get; set; }
    	
        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> OrganizationInstanceGUID{ get; set; }
    	
    }
}
