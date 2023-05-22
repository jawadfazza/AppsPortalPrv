
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SRS_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class codeDepartmentsConfigurations
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DepartmentConfigurationGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid DepartmentConfigurationGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid OrganizationInstanceGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid DepartmentGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] codeDepartmentsConfigurationsRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
        [Display(Name = "DepartmentTypeGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> DepartmentTypeGUID{ get; set; }
    	
        [Display(Name = "ParentDepartmentGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ParentDepartmentGUID{ get; set; }
    	
        [Display(Name = "DepartmentParentStaffGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> DepartmentParentStaffGUID{ get; set; }
    	
    
        public virtual codeDepartments codeDepartments { get; set; }
    }
}
