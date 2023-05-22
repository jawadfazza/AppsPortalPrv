
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
    
    public partial class codeAHDEntitlementTypePerDutyStation
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "EntitlementTypePerDutyStationGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid EntitlementTypePerDutyStationGUID{ get; set; }
    	
        [Display(Name = "EntitlementTypeGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> EntitlementTypeGUID{ get; set; }
    	
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> DutyStationGUID{ get; set; }
    	
        [Display(Name = "EntitlementValue", ResourceType = typeof(resxDbFields))]
    	public Nullable<decimal> EntitlementValue{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] codeAHDEntitlementTypePerDutyStationRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
        [Display(Name = "Archived", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> Archived{ get; set; }
    	
        [Display(Name = "IsCalculatedPerDay", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> IsCalculatedPerDay{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> CreateDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalCreateDate { get { return new Portal().LocalTime(this.CreateDate); } }
    	
        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> CreateByGUID{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> UpdateDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalUpdateDate { get { return new Portal().LocalTime(this.UpdateDate); } }
    	
        [Display(Name = "UpdateByGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> UpdateByGUID{ get; set; }
    	
    
        public virtual codeAHDEntitlementType codeAHDEntitlementType { get; set; }
        public virtual codeDutyStations codeDutyStations { get; set; }
    }
}