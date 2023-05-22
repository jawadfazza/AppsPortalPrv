
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
    
    public partial class dataStaffProfileFeedbackFlow
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "StaffProfileFeedbackFlowGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid StaffProfileFeedbackFlowGUID{ get; set; }
    	
        [Display(Name = "StaffProfileFeedbackGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> StaffProfileFeedbackGUID{ get; set; }
    	
        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> CreatedByGUID{ get; set; }
    	
        [Display(Name = "FlowStatusGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> FlowStatusGUID{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "ActionDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> ActionDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "ActionDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalActionDate { get { return new Portal().LocalTime(this.ActionDate); } }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
    	public string Comments{ get; set; }
    	
        [Display(Name = "IsLastAction", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> IsLastAction{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataStaffProfileFeedbackFlowRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
        [Display(Name = "Archived", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> Archived{ get; set; }
    	
    
        public virtual dataStaffProfileFeedback dataStaffProfileFeedback { get; set; }
        public virtual StaffCoreData StaffCoreData { get; set; }
    }
}
