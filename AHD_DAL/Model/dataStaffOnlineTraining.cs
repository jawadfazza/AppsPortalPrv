
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
    
    public partial class dataStaffOnlineTraining
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "StaffOnlineTrainingGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid StaffOnlineTrainingGUID{ get; set; }
    	
        [Display(Name = "OnlineTrainingTypeGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> OnlineTrainingTypeGUID{ get; set; }
    	
        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> UserGUID{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> StartDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalStartDate { get { return new Portal().LocalTime(this.StartDate); } }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "ExpiryDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> ExpiryDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "ExpiryDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalExpiryDate { get { return new Portal().LocalTime(this.ExpiryDate); } }
    	
        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> CreatedByGUID{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> CreateDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalCreateDate { get { return new Portal().LocalTime(this.CreateDate); } }
    	
        [StringLength(1000, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
    	public string Comments{ get; set; }
    	
        [Display(Name = "TrainingStatusGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> TrainingStatusGUID{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "StatusName", ResourceType = typeof(resxDbFields))]
    	public string StatusName{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataStaffOnlineTrainingRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
        [Display(Name = "Archived", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> Archived{ get; set; }
    	
    
        public virtual StaffCoreData StaffCoreData { get; set; }
    }
}