//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AppsPortal.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class codeDepartmentsConfigurations
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeDepartmentsConfigurations()
        {
            this.configFocalPoint = new HashSet<configFocalPoint>();
        }
    
        public System.Guid DepartmentConfigurationGUID { get; set; }
        public System.Guid OrganizationInstanceGUID { get; set; }
        public System.Guid DepartmentGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeDepartmentsConfigurationsRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<System.Guid> DepartmentTypeGUID { get; set; }
        public Nullable<System.Guid> ParentDepartmentGUID { get; set; }
        public Nullable<System.Guid> DepartmentParentStaffGUID { get; set; }
    
        public virtual codeOrganizationsInstances codeOrganizationsInstances { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<configFocalPoint> configFocalPoint { get; set; }
        public virtual codeDepartments codeDepartments { get; set; }
    }
}
