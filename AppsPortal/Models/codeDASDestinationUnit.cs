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
    
    public partial class codeDASDestinationUnit
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeDASDestinationUnit()
        {
            this.codeDASDestinationUnitFocalPoint = new HashSet<codeDASDestinationUnitFocalPoint>();
        }
    
        public System.Guid DestinationUnitGUID { get; set; }
        public string DestinationUnitName { get; set; }
        public bool Active { get; set; }
        public byte[] codeDASDestinationUnitRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
        public Nullable<System.Guid> SiteOwnerGUID { get; set; }
        public Nullable<bool> IsFilingUnit { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeDASDestinationUnitFocalPoint> codeDASDestinationUnitFocalPoint { get; set; }
    }
}
