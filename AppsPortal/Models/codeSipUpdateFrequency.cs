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
    
    public partial class codeSipUpdateFrequency
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeSipUpdateFrequency()
        {
            this.codeSipIndicatorUpdateFrequency = new HashSet<codeSipIndicatorUpdateFrequency>();
            this.codeSipUpdateFrequencyLanguages = new HashSet<codeSipUpdateFrequencyLanguages>();
        }
    
        public System.Guid SipUpdateFrequencyGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeSipUpdateFrequencyRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeSipIndicatorUpdateFrequency> codeSipIndicatorUpdateFrequency { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeSipUpdateFrequencyLanguages> codeSipUpdateFrequencyLanguages { get; set; }
    }
}
