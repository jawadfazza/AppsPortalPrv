//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WMS_DAL.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class codeWarehouseItemModelDeterminant
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeWarehouseItemModelDeterminant()
        {
            this.dataItemInputDeterminant = new HashSet<dataItemInputDeterminant>();
        }
    
        public System.Guid WarehouseItemModelDeterminantGUID { get; set; }
        public Nullable<System.Guid> WarehouseItemModelGUID { get; set; }
        public Nullable<System.Guid> DeterminantGUID { get; set; }
        public Nullable<bool> IsRequired { get; set; }
        public bool Active { get; set; }
        public byte[] codeWarehouseItemModelDeterminantRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
    
        public virtual codeWarehouseItemModel codeWarehouseItemModel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataItemInputDeterminant> dataItemInputDeterminant { get; set; }
    }
}
