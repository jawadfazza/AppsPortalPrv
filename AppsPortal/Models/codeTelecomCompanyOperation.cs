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
    
    public partial class codeTelecomCompanyOperation
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeTelecomCompanyOperation()
        {
            this.configTelecomCompanyOperation = new HashSet<configTelecomCompanyOperation>();
        }
    
        public System.Guid TelecomCompanyOperationGUID { get; set; }
        public System.Guid TelecomCompanyGUID { get; set; }
        public System.Guid OperationGUID { get; set; }
        public bool Active { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public byte[] codeTelecomCompanyOperationRowVersion { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<configTelecomCompanyOperation> configTelecomCompanyOperation { get; set; }
    }
}
