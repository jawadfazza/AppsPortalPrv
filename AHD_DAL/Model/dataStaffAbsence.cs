
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
    
    public partial class dataStaffAbsence
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "dataStaffAbsenceGuid", ResourceType = typeof(resxDbFields))]
    	public System.Guid dataStaffAbsenceGuid{ get; set; }
    	
        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> UserGUID{ get; set; }
    	
        [Display(Name = "AbsenceTypeGuid", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> AbsenceTypeGuid{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "AbsenceDuration", ResourceType = typeof(resxDbFields))]
    	public double AbsenceDuration{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[DataType(DataType.Date)]
    	[Display(Name = "AbsenceFrom", ResourceType = typeof(resxDbFields))]
    	public System.DateTime AbsenceFrom{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[DataType(DataType.Date)]
    	[Display(Name = "AbsenceTo", ResourceType = typeof(resxDbFields))]
    	public System.DateTime AbsenceTo{ get; set; }
    	
        [Display(Name = "AbsenceDays", ResourceType = typeof(resxDbFields))]
    	public Nullable<double> AbsenceDays{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DirectSupervisor", ResourceType = typeof(resxDbFields))]
    	public System.Guid DirectSupervisor{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "SupervisorComfirmed", ResourceType = typeof(resxDbFields))]
    	public bool SupervisorComfirmed{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "SupervisorComfirmedDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> SupervisorComfirmedDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "SupervisorComfirmedDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalSupervisorComfirmedDate { get { return new Portal().LocalTime(this.SupervisorComfirmedDate); } }
    	
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> DutyStationGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataStaffAbsenceRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
    }
}
