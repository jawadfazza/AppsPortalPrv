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
    
    public partial class codeElectionCorrespondenceType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeElectionCorrespondenceType()
        {
            this.codeElectionCorrespondenceTypeLanguage = new HashSet<codeElectionCorrespondenceTypeLanguage>();
        }
    
        public System.Guid ElectionCorrespondenceTypeGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeElectionCorrespondenceTypeRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeElectionCorrespondenceTypeLanguage> codeElectionCorrespondenceTypeLanguage { get; set; }
    }
}
