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
    
    public partial class codeWarehouseGeneralFeature
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeWarehouseGeneralFeature()
        {
            this.codeWarehouseGeneralFeatureLanguage = new HashSet<codeWarehouseGeneralFeatureLanguage>();
            this.codeWarehouseItemFeatur = new HashSet<codeWarehouseItemFeatur>();
            this.codeWarehouseItemModelFeature = new HashSet<codeWarehouseItemModelFeature>();
        }
    
        public System.Guid WarehouseGeneralFeatureGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeWarehouseGeneralFeatureRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeWarehouseGeneralFeatureLanguage> codeWarehouseGeneralFeatureLanguage { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeWarehouseItemFeatur> codeWarehouseItemFeatur { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeWarehouseItemModelFeature> codeWarehouseItemModelFeature { get; set; }
    }
}
