
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
    
    public partial class dataOvertimeMonthCycleStaff
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public dataOvertimeMonthCycleStaff()
        {
            this.dataOvertimeMonthCycleStaffFlow = new HashSet<dataOvertimeMonthCycleStaffFlow>();
            this.dataStaffOvertime = new HashSet<dataStaffOvertime>();
        }
    
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OvertimeMonthCycleStaffGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid OvertimeMonthCycleStaffGUID{ get; set; }
    	
        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> UserGUID{ get; set; }
    	
        [Display(Name = "OvertimeMonthCycleGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> OvertimeMonthCycleGUID{ get; set; }
    	
        [Display(Name = "TotalHoursPayed", ResourceType = typeof(resxDbFields))]
    	public Nullable<double> TotalHoursPayed{ get; set; }
    	
        [Display(Name = "TotalPerformedHours", ResourceType = typeof(resxDbFields))]
    	public Nullable<double> TotalPerformedHours{ get; set; }
    	
        [Display(Name = "TotalPay", ResourceType = typeof(resxDbFields))]
    	public Nullable<double> TotalPay{ get; set; }
    	
        [Display(Name = "LastFlowStatusGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> LastFlowStatusGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataOvertimeMonthCycleStaffRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
        [Display(Name = "Archived", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> Archived{ get; set; }
    	
        [Display(Name = "GradeGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> GradeGUID{ get; set; }
    	
        [Display(Name = "JobtitleGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> JobtitleGUID{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ApprovedBy", ResourceType = typeof(resxDbFields))]
    	public string ApprovedBy{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "ApprovedDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> ApprovedDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "ApprovedDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalApprovedDate { get { return new Portal().LocalTime(this.ApprovedDate); } }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "AuthorizedBy", ResourceType = typeof(resxDbFields))]
    	public string AuthorizedBy{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "AuthorizedDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> AuthorizedDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "AuthorizedDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalAuthorizedDate { get { return new Portal().LocalTime(this.AuthorizedDate); } }
    	
        [Display(Name = "SupervisorGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> SupervisorGUID{ get; set; }
    	
        [StringLength(2000, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "SupvisorComment", ResourceType = typeof(resxDbFields))]
    	public string SupvisorComment{ get; set; }
    	
        [StringLength(2000, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "CertifiedComment", ResourceType = typeof(resxDbFields))]
    	public string CertifiedComment{ get; set; }
    	
        [Display(Name = "ApprovedByGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ApprovedByGUID{ get; set; }
    	
        [Display(Name = "CertifiedByGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> CertifiedByGUID{ get; set; }
    	
        [Display(Name = "StepGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> StepGUID{ get; set; }
    	
    
        public virtual dataOvertimeMonthCycle dataOvertimeMonthCycle { get; set; }
        public virtual StaffCoreData StaffCoreData { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataOvertimeMonthCycleStaffFlow> dataOvertimeMonthCycleStaffFlow { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataStaffOvertime> dataStaffOvertime { get; set; }
    }
}
