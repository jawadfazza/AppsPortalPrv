
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
    
    public partial class codeDepartments
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeDepartments()
        {
            this.codeDepartmentsLanguages = new HashSet<codeDepartmentsLanguages>();
        }
    
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid DepartmentGUID{ get; set; }
    	
        [Display(Name = "FactorGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> FactorGUID{ get; set; }
    	
        [StringLength(150, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DepartmentCode", ResourceType = typeof(resxDbFields))]
    	public string DepartmentCode{ get; set; }
    	
        [Display(Name = "SortID", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> SortID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] codeDepartmentsRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
        [Display(Name = "SectionGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> SectionGUID{ get; set; }
    	
        [Display(Name = "SubSort", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> SubSort{ get; set; }
    	
        [Display(Name = "NodeId", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> NodeId{ get; set; }
    	
        [Display(Name = "SubNodeId", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> SubNodeId{ get; set; }
    	
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeDepartmentsLanguages> codeDepartmentsLanguages { get; set; }
    }
}
