
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
    
    public partial class codeAHDOrgnizationWorkingHour
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OrgnizationWorkingHourGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid OrgnizationWorkingHourGUID{ get; set; }
    	
        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> OrganizationInstanceGUID{ get; set; }
    	
        [Display(Name = "WorkingDayTypeGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> WorkingDayTypeGUID{ get; set; }
    	
        [Display(Name = "StaffTypeGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> StaffTypeGUID{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> StartDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalStartDate { get { return new Portal().LocalTime(this.StartDate); } }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "EndDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> EndDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "EndDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalEndDate { get { return new Portal().LocalTime(this.EndDate); } }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "StartWorkingHour", ResourceType = typeof(resxDbFields))]
    	public string StartWorkingHour{ get; set; }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "EndWorkingHour", ResourceType = typeof(resxDbFields))]
    	public string EndWorkingHour{ get; set; }
    	
        [Display(Name = "TotalHour", ResourceType = typeof(resxDbFields))]
    	public Nullable<double> TotalHour{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] codeAHDOrgnizationWorkingHourRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
        [Display(Name = "Archived", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> Archived{ get; set; }
    	
    }
}