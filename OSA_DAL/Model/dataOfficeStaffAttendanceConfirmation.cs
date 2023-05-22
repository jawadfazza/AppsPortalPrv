
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
    
    public partial class dataOfficeStaffAttendanceConfirmation
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OfficeStaffAttendanceConfirmationGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid OfficeStaffAttendanceConfirmationGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid UserGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ReportToGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ReportToGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ConfirmedBy", ResourceType = typeof(resxDbFields))]
    	public string ConfirmedBy{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[DataType(DataType.DateTime)]
    	[Display(Name = "ConfirmedDate", ResourceType = typeof(resxDbFields))]
    	public System.DateTime ConfirmedDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "ConfirmedDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalConfirmedDate { get { return new Portal().LocalTime(this.ConfirmedDate); } }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ConfirmedYear", ResourceType = typeof(resxDbFields))]
    	public int ConfirmedYear{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ConfirmedMonth", ResourceType = typeof(resxDbFields))]
    	public int ConfirmedMonth{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "PaymentConfirmedDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> PaymentConfirmedDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "PaymentConfirmedDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalPaymentConfirmedDate { get { return new Portal().LocalTime(this.PaymentConfirmedDate); } }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PaymentConfirmedBy", ResourceType = typeof(resxDbFields))]
    	public string PaymentConfirmedBy{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataOfficeStaffAttendanceConfirmationRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
    }
}