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
    
    public partial class codeWarehouseLocation
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeWarehouseLocation()
        {
            this.codeWarehouse = new HashSet<codeWarehouse>();
            this.codeWarehouseLocationLanguage = new HashSet<codeWarehouseLocationLanguage>();
            this.codeWarehouseSubLocation = new HashSet<codeWarehouseSubLocation>();
        }
    
        public System.Guid WarehouseLocationGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeWarehouseLocationRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
        public Nullable<System.Guid> OrganizationInstanceGUID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeWarehouse> codeWarehouse { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeWarehouseLocationLanguage> codeWarehouseLocationLanguage { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeWarehouseSubLocation> codeWarehouseSubLocation { get; set; }
    }
}
