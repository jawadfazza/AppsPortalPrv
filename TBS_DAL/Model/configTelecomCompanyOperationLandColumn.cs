
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TBS_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class configTelecomCompanyOperationLandColumn
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TelecomCompanyOperationFileColumnGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid TelecomCompanyOperationFileColumnGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TelecomCompanyOperationConfigGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid TelecomCompanyOperationConfigGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "CallingPartyNumberColumnIndex", ResourceType = typeof(resxDbFields))]
    	public int CallingPartyNumberColumnIndex{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "CallingPartyNumberColumnName", ResourceType = typeof(resxDbFields))]
    	public string CallingPartyNumberColumnName{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OriginalCalledPartyNumberColumnIndex", ResourceType = typeof(resxDbFields))]
    	public int OriginalCalledPartyNumberColumnIndex{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OriginalCalledPartyNumberColumnName", ResourceType = typeof(resxDbFields))]
    	public string OriginalCalledPartyNumberColumnName{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "FinalCalledPartyNumberColumnIndex", ResourceType = typeof(resxDbFields))]
    	public int FinalCalledPartyNumberColumnIndex{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "FinalCalledPartyNumberColumnName", ResourceType = typeof(resxDbFields))]
    	public string FinalCalledPartyNumberColumnName{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DateTimeConnectColumnIndex", ResourceType = typeof(resxDbFields))]
    	public int DateTimeConnectColumnIndex{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DateTimeConnectColumnName", ResourceType = typeof(resxDbFields))]
    	public string DateTimeConnectColumnName{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DateTimeDisconnectColumnIndex", ResourceType = typeof(resxDbFields))]
    	public int DateTimeDisconnectColumnIndex{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DateTimeDisconnectColumnName", ResourceType = typeof(resxDbFields))]
    	public string DateTimeDisconnectColumnName{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OriginalCalledPartyNumberPartitionColumnIndex", ResourceType = typeof(resxDbFields))]
    	public int OriginalCalledPartyNumberPartitionColumnIndex{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OriginalCalledPartyNumberPartitionColumnName", ResourceType = typeof(resxDbFields))]
    	public string OriginalCalledPartyNumberPartitionColumnName{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "FinalCalledPartyNumberPartitionColumnIndex", ResourceType = typeof(resxDbFields))]
    	public int FinalCalledPartyNumberPartitionColumnIndex{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "FinalCalledPartyNumberPartitionColumnName", ResourceType = typeof(resxDbFields))]
    	public string FinalCalledPartyNumberPartitionColumnName{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DurationInMinutesColumnIndex", ResourceType = typeof(resxDbFields))]
    	public int DurationInMinutesColumnIndex{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DurationInMinutesColumnName", ResourceType = typeof(resxDbFields))]
    	public string DurationInMinutesColumnName{ get; set; }
    	
        [Display(Name = "DurationInSecondsColumnIndex", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> DurationInSecondsColumnIndex{ get; set; }
    	
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DurationInSecondsColumnName", ResourceType = typeof(resxDbFields))]
    	public string DurationInSecondsColumnName{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "AuthCodeDescriptionColumnIndex", ResourceType = typeof(resxDbFields))]
    	public int AuthCodeDescriptionColumnIndex{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "AuthCodeDescriptionColumnName", ResourceType = typeof(resxDbFields))]
    	public string AuthCodeDescriptionColumnName{ get; set; }
    	
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "CallingPartyUnicodeLoginUserIDColumnName", ResourceType = typeof(resxDbFields))]
    	public string CallingPartyUnicodeLoginUserIDColumnName{ get; set; }
    	
        [Display(Name = "CallingPartyUnicodeLoginUserIDColumnIndex", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> CallingPartyUnicodeLoginUserIDColumnIndex{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
        public byte[] configTelecomCompanyOperationLandColumnRowVersion{ get; set; }
    	
    
        public virtual configTelecomCompanyOperation configTelecomCompanyOperation { get; set; }
    }
}
