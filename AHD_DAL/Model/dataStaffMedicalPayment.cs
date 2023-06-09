
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
    
    public partial class dataStaffMedicalPayment
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "StaffMedicalPaymentGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid StaffMedicalPaymentGUID{ get; set; }
    	
        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> UserGUID{ get; set; }
    	
        [Display(Name = "Year", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> Year{ get; set; }
    	
        [StringLength(5, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Month", ResourceType = typeof(resxDbFields))]
    	public string Month{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "ActionDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> ActionDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "ActionDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalActionDate { get { return new Portal().LocalTime(this.ActionDate); } }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "AmountUSD", ResourceType = typeof(resxDbFields))]
    	public double AmountUSD{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ClaimNumber", ResourceType = typeof(resxDbFields))]
    	public string ClaimNumber{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "MedicalReason", ResourceType = typeof(resxDbFields))]
    	public string MedicalReason{ get; set; }
    	
        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> CreatedByGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[DataType(DataType.DateTime)]
    	[Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
    	public System.DateTime CreateDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalCreateDate { get { return new Portal().LocalTime(this.CreateDate); } }
    	
        [Display(Name = "UpdatedByGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> UpdatedByGUID{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> UpdateDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalUpdateDate { get { return new Portal().LocalTime(this.UpdateDate); } }
    	
        [Display(Name = "LastFlowStatusGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> LastFlowStatusGUID{ get; set; }
    	
        [StringLength(2000, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
    	public string Comments{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataStaffMedicalPaymentRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
        [Display(Name = "Archived", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> Archived{ get; set; }
    	
    
        public virtual StaffCoreData StaffCoreData { get; set; }
        public virtual StaffCoreData StaffCoreData1 { get; set; }
        public virtual StaffCoreData StaffCoreData2 { get; set; }
    }
}
