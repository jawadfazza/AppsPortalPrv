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
    
    public partial class codeWarehouseSubLocation
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeWarehouseSubLocation()
        {
            this.codeWarehouseSubLocationLanguage = new HashSet<codeWarehouseSubLocationLanguage>();
        }
    
        public System.Guid WarehouseSubLocationGUID { get; set; }
        public Nullable<System.Guid> WarehouseLocationGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeWarehouseSubLocationRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeWarehouseSubLocationLanguage> codeWarehouseSubLocationLanguage { get; set; }
        public virtual codeWarehouseLocation codeWarehouseLocation { get; set; }
    }
}