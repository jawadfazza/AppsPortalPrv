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
    
    public partial class codeSipTheme
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeSipTheme()
        {
            this.codeSipIndicatorTheme = new HashSet<codeSipIndicatorTheme>();
            this.codeSipThemeLanguages = new HashSet<codeSipThemeLanguages>();
        }
    
        public System.Guid SipThemeGUID { get; set; }
        public Nullable<System.Guid> SipParentThemeGUID { get; set; }
        public string SipThemeCode { get; set; }
        public bool Active { get; set; }
        public byte[] codeSipThemeRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeSipIndicatorTheme> codeSipIndicatorTheme { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeSipThemeLanguages> codeSipThemeLanguages { get; set; }
    }
}
