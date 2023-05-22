
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PPA_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class userRegistrationQueue
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "UserRegistrationQueueGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid UserRegistrationQueueGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid UserGUID{ get; set; }
    	
        [Display(Name = "SponsorUserProfileGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> SponsorUserProfileGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "SponsorConfirmed", ResourceType = typeof(resxDbFields))]
    	public bool SponsorConfirmed{ get; set; }
    	
        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> OrganizationInstanceGUID{ get; set; }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TimeZone", ResourceType = typeof(resxDbFields))]
    	public string TimeZone{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "RequestedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> RequestedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "RequestedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalRequestedOn { get { return new Portal().LocalTime(this.RequestedOn); } }
    	
        [Display(Name = "SecurityQuestionGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> SecurityQuestionGUID{ get; set; }
    	
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "SecurityAnswer", ResourceType = typeof(resxDbFields))]
    	public string SecurityAnswer{ get; set; }
    	
        [Display(Name = "AccountStatusID", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> AccountStatusID{ get; set; }
    	
        [Display(Name = "LanguageID", ResourceType = typeof(resxDbFields))]
    	public string LanguageID{ get; set; }
    	
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "FirstName", ResourceType = typeof(resxDbFields))]
    	public string FirstName{ get; set; }
    	
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Surname", ResourceType = typeof(resxDbFields))]
    	public string Surname{ get; set; }
    	
        [Display(Name = "OrganizationGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> OrganizationGUID{ get; set; }
    	
        [DataType(DataType.EmailAddress)]
    	[StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "EmailAddress", ResourceType = typeof(resxDbFields))]
    	public string EmailAddress{ get; set; }
    	
        [Display(Name = "FromDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> FromDate{ get; set; }
    	
        [Display(Name = "ToDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ToDate{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] userRegistrationQueueRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
        [Display(Name = "JobTitleGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> JobTitleGUID{ get; set; }
    	
    }
}