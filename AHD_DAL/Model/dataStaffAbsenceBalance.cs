
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
    
    public partial class dataStaffAbsenceBalance
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "StaffAbsenceBalanceGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid StaffAbsenceBalanceGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid UserGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "AbsenceTypeGuid", ResourceType = typeof(resxDbFields))]
    	public System.Guid AbsenceTypeGuid{ get; set; }
    	
        [Display(Name = "Balance", ResourceType = typeof(resxDbFields))]
    	public Nullable<double> Balance{ get; set; }
    	
        [Display(Name = "MaxBalancePerYear", ResourceType = typeof(resxDbFields))]
    	public Nullable<double> MaxBalancePerYear{ get; set; }
    	
        [Display(Name = "InitialBalance", ResourceType = typeof(resxDbFields))]
    	public Nullable<double> InitialBalance{ get; set; }
    	
        [DataType(DataType.Date)]
    	[Display(Name = "ResetDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> ResetDate{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataStaffAbsenceBalanceRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
    }
}