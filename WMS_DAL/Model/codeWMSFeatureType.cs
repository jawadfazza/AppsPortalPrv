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
    
    public partial class codeWMSFeatureType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeWMSFeatureType()
        {
            this.codeWMSFeatureTypeValue = new HashSet<codeWMSFeatureTypeValue>();
        }
    
        public System.Guid FeatureTypeGUID { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public byte[] codeWMSFeatureTypeRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeWMSFeatureTypeValue> codeWMSFeatureTypeValue { get; set; }
    }
}