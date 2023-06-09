
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SHM_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class userContactDetails
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ContactDetailsGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ContactDetailsGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid UserGUID{ get; set; }
    	
        [StringLength(5, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PrimaryMobileCountryCode", ResourceType = typeof(resxDbFields))]
    	public string PrimaryMobileCountryCode{ get; set; }
    	
        [StringLength(12, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(resxMessages), ErrorMessageResourceName = "InvalidNumber")]
    	[Display(Name = "PrimaryMobileNumber", ResourceType = typeof(resxDbFields))]
    	public string PrimaryMobileNumber{ get; set; }
    	
        [StringLength(5, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "SecondayMobileCountryCode", ResourceType = typeof(resxDbFields))]
    	public string SecondayMobileCountryCode{ get; set; }
    	
        [StringLength(12, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(resxMessages), ErrorMessageResourceName = "InvalidNumber")]
    	[Display(Name = "SecondaryMobileNumber", ResourceType = typeof(resxDbFields))]
    	public string SecondaryMobileNumber{ get; set; }
    	
        [StringLength(5, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OfficeLandlineCountryCode", ResourceType = typeof(resxDbFields))]
    	public string OfficeLandlineCountryCode{ get; set; }
    	
        [StringLength(5, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(resxMessages), ErrorMessageResourceName = "InvalidNumber")]
    	[Display(Name = "OfficeLandlineAreaCode", ResourceType = typeof(resxDbFields))]
    	public string OfficeLandlineAreaCode{ get; set; }
    	
        [StringLength(12, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(resxMessages), ErrorMessageResourceName = "InvalidNumber")]
    	[Display(Name = "OfficeLandlineNumber", ResourceType = typeof(resxDbFields))]
    	public string OfficeLandlineNumber{ get; set; }
    	
        [StringLength(5, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "HomeLandlineCountryCode", ResourceType = typeof(resxDbFields))]
    	public string HomeLandlineCountryCode{ get; set; }
    	
        [StringLength(5, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(resxMessages), ErrorMessageResourceName = "InvalidNumber")]
    	[Display(Name = "HomeLandlineAreaCode", ResourceType = typeof(resxDbFields))]
    	public string HomeLandlineAreaCode{ get; set; }
    	
        [StringLength(12, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(resxMessages), ErrorMessageResourceName = "InvalidNumber")]
    	[Display(Name = "HomeLandlineNumber", ResourceType = typeof(resxDbFields))]
    	public string HomeLandlineNumber{ get; set; }
    	
        [StringLength(5, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IPPhoneCountryCode", ResourceType = typeof(resxDbFields))]
    	public string IPPhoneCountryCode{ get; set; }
    	
        [StringLength(2, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IPPhoneOfficeCode", ResourceType = typeof(resxDbFields))]
    	public string IPPhoneOfficeCode{ get; set; }
    	
        [StringLength(4, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(resxMessages), ErrorMessageResourceName = "InvalidNumber")]
    	[Display(Name = "IPPhoneNumber", ResourceType = typeof(resxDbFields))]
    	public string IPPhoneNumber{ get; set; }
    	
        [StringLength(5, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IPPhoneNumberFromOutsideCountryCode", ResourceType = typeof(resxDbFields))]
    	public string IPPhoneNumberFromOutsideCountryCode{ get; set; }
    	
        [StringLength(5, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(resxMessages), ErrorMessageResourceName = "InvalidNumber")]
    	[Display(Name = "IPPhoneNumberFromOutsideAreaCode", ResourceType = typeof(resxDbFields))]
    	public string IPPhoneNumberFromOutsideAreaCode{ get; set; }
    	
        [StringLength(10, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(resxMessages), ErrorMessageResourceName = "InvalidNumber")]
    	[Display(Name = "IPPhoneNumberFromOutsideInternalCode", ResourceType = typeof(resxDbFields))]
    	public string IPPhoneNumberFromOutsideInternalCode{ get; set; }
    	
        [StringLength(5, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(resxMessages), ErrorMessageResourceName = "InvalidNumber")]
    	[Display(Name = "IPPhoneNumberFromOutsideExtension", ResourceType = typeof(resxDbFields))]
    	public string IPPhoneNumberFromOutsideExtension{ get; set; }
    	
        [DataType(DataType.EmailAddress)]
    	[StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PrivateEmailAddress", ResourceType = typeof(resxDbFields))]
    	public string PrivateEmailAddress{ get; set; }
    	
        [StringLength(10, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "CallSign", ResourceType = typeof(resxDbFields))]
    	public string CallSign{ get; set; }
    	
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> Active{ get; set; }
    	
        public byte[] userContactDetailsRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
    }
}
