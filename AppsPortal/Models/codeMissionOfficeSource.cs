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
    
    public partial class codeMissionOfficeSource
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeMissionOfficeSource()
        {
            this.codeMissionOfficeSourceLanguage = new HashSet<codeMissionOfficeSourceLanguage>();
            this.dataMissionReportForm = new HashSet<dataMissionReportForm>();
        }
    
        public System.Guid MissionOfficeSourceGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeMissionOfficeSourceRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
        public string OfficeSourceCode { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeMissionOfficeSourceLanguage> codeMissionOfficeSourceLanguage { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataMissionReportForm> dataMissionReportForm { get; set; }
    }
}
