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
    
    public partial class codePmdIndicator
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codePmdIndicator()
        {
            this.codePmdIndicatorLanguages = new HashSet<codePmdIndicatorLanguage>();
        }
    
        public System.Guid IndicatorGUID { get; set; }
        public System.Guid OutputGUID { get; set; }
        public bool Active { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public byte[] codePmdIndicatorRowVersion { get; set; }
    
        public virtual codePmdOutput codePmdOutput { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codePmdIndicatorLanguage> codePmdIndicatorLanguages { get; set; }
    }
}
