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
    
    public partial class codeConditionType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeConditionType()
        {
            this.codeConditionTypeLanguage = new HashSet<codeConditionTypeLanguage>();
        }
    
        public System.Guid ConditionTypeGUID { get; set; }
        public Nullable<System.Guid> DataTypeGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeConditionTypesRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeConditionTypeLanguage> codeConditionTypeLanguage { get; set; }
    }
}
