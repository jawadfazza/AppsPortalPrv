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
    
    public partial class codeDepartments
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeDepartments()
        {
            this.StaffCoreDataHistory = new HashSet<StaffCoreDataHistory>();
            this.userProfiles = new HashSet<userProfiles>();
            this.codeAppointmentType = new HashSet<codeAppointmentType>();
            this.codeDepartmentsLanguages = new HashSet<codeDepartmentsLanguages>();
            this.codeDepartmentsConfigurations = new HashSet<codeDepartmentsConfigurations>();
        }
    
        public System.Guid DepartmentGUID { get; set; }
        public Nullable<System.Guid> FactorGUID { get; set; }
        public string DepartmentCode { get; set; }
        public Nullable<int> SortID { get; set; }
        public bool Active { get; set; }
        public byte[] codeDepartmentsRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<System.Guid> SectionGUID { get; set; }
        public Nullable<int> SubSort { get; set; }
        public Nullable<int> NodeId { get; set; }
        public Nullable<int> SubNodeId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StaffCoreDataHistory> StaffCoreDataHistory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<userProfiles> userProfiles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeAppointmentType> codeAppointmentType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeDepartmentsLanguages> codeDepartmentsLanguages { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeDepartmentsConfigurations> codeDepartmentsConfigurations { get; set; }
    }
}
