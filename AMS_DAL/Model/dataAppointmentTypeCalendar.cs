
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AMS_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class dataAppointmentTypeCalendar
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public dataAppointmentTypeCalendar()
        {
            this.dataAppointmentCalendarHolded = new HashSet<dataAppointmentCalendarHolded>();
        }
    
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "AppointmentTypeCalenderGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid AppointmentTypeCalenderGUID{ get; set; }
    	
        [Display(Name = "AppointmentTypeGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> AppointmentTypeGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[DataType(DataType.DateTime)]
    	[Display(Name = "EventStartDate", ResourceType = typeof(resxDbFields))]
    	public System.DateTime EventStartDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "EventStartDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalEventStartDate { get { return new Portal().LocalTime(this.EventStartDate); } }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[DataType(DataType.DateTime)]
    	[Display(Name = "EventEndDate", ResourceType = typeof(resxDbFields))]
    	public System.DateTime EventEndDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "EventEndDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalEventEndDate { get { return new Portal().LocalTime(this.EventEndDate); } }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "SlotAvailable", ResourceType = typeof(resxDbFields))]
    	public int SlotAvailable{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "SlotClosed", ResourceType = typeof(resxDbFields))]
    	public int SlotClosed{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PreventAppointments", ResourceType = typeof(resxDbFields))]
    	public bool PreventAppointments{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PublicHolday", ResourceType = typeof(resxDbFields))]
    	public bool PublicHolday{ get; set; }
    	
        [StringLength(250, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Comment", ResourceType = typeof(resxDbFields))]
    	public string Comment{ get; set; }
    	
        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> OrganizationInstanceGUID{ get; set; }
    	
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> DutyStationGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataAppointmentTypeCalendarRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataAppointmentCalendarHolded> dataAppointmentCalendarHolded { get; set; }
        public virtual codeAppointmentType codeAppointmentType { get; set; }
    }
}
