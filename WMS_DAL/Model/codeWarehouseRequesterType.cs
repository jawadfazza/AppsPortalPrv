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
    
    public partial class codeWarehouseRequesterType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeWarehouseRequesterType()
        {
            this.codeWarehouseRequesterTypeLanguage = new HashSet<codeWarehouseRequesterTypeLanguage>();
        }
    
        public System.Guid WarehouseRequesterTypeGUID { get; set; }
        public Nullable<bool> Active { get; set; }
        public byte[] codeWarehouseRequesterTypeRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
        public Nullable<bool> IsDriver { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeWarehouseRequesterTypeLanguage> codeWarehouseRequesterTypeLanguage { get; set; }
    }
}
