
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
    
    public partial class dataStaffLeaveAttendanceDaily
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "StaffLeaveAttendanceDailyGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid StaffLeaveAttendanceDailyGUID{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DayDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DayDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DayDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDayDate { get { return new Portal().LocalTime(this.DayDate); } }
    	
        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> StaffGUID{ get; set; }
    	
        [Display(Name = "InternationalStaffAttendanceTypeGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> InternationalStaffAttendanceTypeGUID{ get; set; }
    	
    
        public virtual codeAHDInternationalStaffAttendanceType codeAHDInternationalStaffAttendanceType { get; set; }
        public virtual StaffCoreData StaffCoreData { get; set; }
    }
}
