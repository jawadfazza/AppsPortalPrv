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
    
    public partial class codeWarehouseItem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeWarehouseItem()
        {
            this.codeWarehouseItemModel = new HashSet<codeWarehouseItemModel>();
            this.codeWarehouseItemDeterminant = new HashSet<codeWarehouseItemDeterminant>();
            this.codeWarehouseItemFeatur = new HashSet<codeWarehouseItemFeatur>();
            this.codeWarehouseItemLanguage = new HashSet<codeWarehouseItemLanguage>();
            this.codeWarehouseItemPackingType = new HashSet<codeWarehouseItemPackingType>();
        }
    
        public System.Guid WarehouseItemGUID { get; set; }
        public Nullable<System.Guid> WarehouseItemClassificationGUID { get; set; }
        public Nullable<System.Guid> InventoryTypeGUID { get; set; }
        public Nullable<System.Guid> MeasurementUnitGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeWarehouseItemRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
        public Nullable<bool> IsDeterminanted { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeWarehouseItemModel> codeWarehouseItemModel { get; set; }
        public virtual codeWarehouseItemClassification codeWarehouseItemClassification { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeWarehouseItemDeterminant> codeWarehouseItemDeterminant { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeWarehouseItemFeatur> codeWarehouseItemFeatur { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeWarehouseItemLanguage> codeWarehouseItemLanguage { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeWarehouseItemPackingType> codeWarehouseItemPackingType { get; set; }
    }
}
