
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
    
    public partial class StaffContactsInformation
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "StaffinformationID", ResourceType = typeof(resxDbFields))]
    	public int StaffinformationID{ get; set; }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "FullName", ResourceType = typeof(resxDbFields))]
    	public string FullName{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "FirstName", ResourceType = typeof(resxDbFields))]
    	public string FirstName{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "FamilyName", ResourceType = typeof(resxDbFields))]
    	public string FamilyName{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "MiddleName", ResourceType = typeof(resxDbFields))]
    	public string MiddleName{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Prefix", ResourceType = typeof(resxDbFields))]
    	public string Prefix{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DepartmentName", ResourceType = typeof(resxDbFields))]
    	public string DepartmentName{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Governorate", ResourceType = typeof(resxDbFields))]
    	public string Governorate{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DutyStation", ResourceType = typeof(resxDbFields))]
    	public string DutyStation{ get; set; }
    	
        [Display(Name = "SupervisorID", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> SupervisorID{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Company", ResourceType = typeof(resxDbFields))]
    	public string Company{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "JobTitle", ResourceType = typeof(resxDbFields))]
    	public string JobTitle{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Address", ResourceType = typeof(resxDbFields))]
    	public string Address{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Mobile1", ResourceType = typeof(resxDbFields))]
    	public string Mobile1{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Home", ResourceType = typeof(resxDbFields))]
    	public string Home{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "work", ResourceType = typeof(resxDbFields))]
    	public string work{ get; set; }
    	
        [DataType(DataType.EmailAddress)]
    	[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Emailwork", ResourceType = typeof(resxDbFields))]
    	public string Emailwork{ get; set; }
    	
        [DataType(DataType.EmailAddress)]
    	[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "EmailHome", ResourceType = typeof(resxDbFields))]
    	public string EmailHome{ get; set; }
    	
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "UpdatedMobile", ResourceType = typeof(resxDbFields))]
    	public string UpdatedMobile{ get; set; }
    	
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> Active{ get; set; }
    	
    }
}