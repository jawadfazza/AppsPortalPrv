//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IMS_DAL.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class codeIMSHumanitarianNeed
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeIMSHumanitarianNeed()
        {
            this.codeIMSHumanitarianNeedLanguage = new HashSet<codeIMSHumanitarianNeedLanguage>();
            this.dataMissionReportFormHumanitarianNeed = new HashSet<dataMissionReportFormHumanitarianNeed>();
        }
    
        public System.Guid HumanitarianNeedGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeHumanitarianNeedRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeIMSHumanitarianNeedLanguage> codeIMSHumanitarianNeedLanguage { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataMissionReportFormHumanitarianNeed> dataMissionReportFormHumanitarianNeed { get; set; }
    }
}
