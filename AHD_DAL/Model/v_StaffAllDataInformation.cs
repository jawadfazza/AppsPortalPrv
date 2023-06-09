
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
    
    public partial class v_StaffAllDataInformation
    {
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
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Grade", ResourceType = typeof(resxDbFields))]
    	public string Grade{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "GradeGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid GradeGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid UserGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DutyStation", ResourceType = typeof(resxDbFields))]
    	public string DutyStation{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid DutyStationGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(512, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "JobTitle", ResourceType = typeof(resxDbFields))]
    	public string JobTitle{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "JobTitleGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid JobTitleGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid CountryGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(150, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Nationality", ResourceType = typeof(resxDbFields))]
    	public string Nationality{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "RecruitmentType", ResourceType = typeof(resxDbFields))]
    	public string RecruitmentType{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "RecruitmentTypeGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid RecruitmentTypeGUID{ get; set; }
    	
        [StringLength(150, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DepartmentDescription", ResourceType = typeof(resxDbFields))]
    	public string DepartmentDescription{ get; set; }
    	
        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> DepartmentGUID{ get; set; }
    	
        [Display(Name = "Age", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> Age{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "ContractStartDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> ContractStartDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "ContractStartDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalContractStartDate { get { return new Portal().LocalTime(this.ContractStartDate); } }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "StaffEOD", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> StaffEOD{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "StaffEOD", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalStaffEOD { get { return new Portal().LocalTime(this.StaffEOD); } }
    	
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
    	
        [Display(Name = "ReportToGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ReportToGUID{ get; set; }
    	
        [Display(Name = "StaffStatusGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> StaffStatusGUID{ get; set; }
    	
        [StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "StaffStatus", ResourceType = typeof(resxDbFields))]
    	public string StaffStatus{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid OrganizationInstanceGUID{ get; set; }
    	
    }
}
