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
    
    public partial class codeIMSVisitObjective
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeIMSVisitObjective()
        {
            this.codeIMSVisitObjectiveLanguage = new HashSet<codeIMSVisitObjectiveLanguage>();
        }
    
        public System.Guid VisitObjectiveGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeVisitObjectiveRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
        public Nullable<System.Guid> VisitObjectiveCategoryGUID { get; set; }
    
        public virtual codeIMSVisitObjectiveCategory codeIMSVisitObjectiveCategory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeIMSVisitObjectiveLanguage> codeIMSVisitObjectiveLanguage { get; set; }
    }
}
