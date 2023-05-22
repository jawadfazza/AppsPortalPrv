
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
    
    public partial class dataOfficeStaffAttendance
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OfficeStaffAttendanceGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid OfficeStaffAttendanceGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid UserGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[DataType(DataType.Date)]
    	[Display(Name = "AttendanceFromDatetime", ResourceType = typeof(resxDbFields))]
    	public System.DateTime AttendanceFromDatetime{ get; set; }
    	
        [DataType(DataType.Date)]
    	[Display(Name = "AttendanceToDatetime", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> AttendanceToDatetime{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IsAttend", ResourceType = typeof(resxDbFields))]
    	public bool IsAttend{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IsConfirmed", ResourceType = typeof(resxDbFields))]
    	public bool IsConfirmed{ get; set; }
    	
        [Display(Name = "ShuttleDepartureMorningTime", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> ShuttleDepartureMorningTime{ get; set; }
    	
        [Display(Name = "ShuttleDepartureEveningTime", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> ShuttleDepartureEveningTime{ get; set; }
    	
        [Display(Name = "OfficeFloorRoomGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> OfficeFloorRoomGUID{ get; set; }
    	
        [Display(Name = "OfficeStaffAttendanceConfirmationGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> OfficeStaffAttendanceConfirmationGUID{ get; set; }
    	
        [Display(Name = "WorkingHours", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> WorkingHours{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataOfficeStaffAttendanceRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
    }
}