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
    
    public partial class codeOperations
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeOperations()
        {
            this.codeOperationsLanguages = new HashSet<codeOperationsLanguages>();
            this.codeOrganizationsInstances = new HashSet<codeOrganizationsInstances>();
        }
    
        public System.Guid OperationGUID { get; set; }
        public Nullable<System.Guid> BureauGUID { get; set; }
        public Nullable<System.Guid> CountryGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeOperationsRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeOperationsLanguages> codeOperationsLanguages { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeOrganizationsInstances> codeOrganizationsInstances { get; set; }
    }
}