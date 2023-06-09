
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
    
    public partial class dataMissionRequestTraveler
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "MissionRequestTravelerGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid MissionRequestTravelerGUID{ get; set; }
    	
        [Display(Name = "MissionRequestGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> MissionRequestGUID{ get; set; }
    	
        [Display(Name = "TraverlerTypeGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> TraverlerTypeGUID{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "FamilyMemberName", ResourceType = typeof(resxDbFields))]
    	public string FamilyMemberName{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DateOfBirth", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DateOfBirth{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DateOfBirth", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDateOfBirth { get { return new Portal().LocalTime(this.DateOfBirth); } }
    	
        [Display(Name = "GenderGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> GenderGUID{ get; set; }
    	
        [Display(Name = "RelationGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> RelationGUID{ get; set; }
    	
        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> StaffGUID{ get; set; }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
    	public string StaffName{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "JobTitle", ResourceType = typeof(resxDbFields))]
    	public string JobTitle{ get; set; }
    	
        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> DepartmentGUID{ get; set; }
    	
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> DutyStationGUID{ get; set; }
    	
        [StringLength(2000, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
    	public string Comments{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataMissionRequestTravelerRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
        [Display(Name = "Archived", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> Archived{ get; set; }
    	
    
        public virtual dataMissionRequest dataMissionRequest { get; set; }
        public virtual StaffCoreData StaffCoreData { get; set; }
    }
}
