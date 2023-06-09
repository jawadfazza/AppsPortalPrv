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
    
    public partial class dataMedicalItemInput
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public dataMedicalItemInput()
        {
            this.dataMedicalItemInputDetail = new HashSet<dataMedicalItemInputDetail>();
        }
    
        public System.Guid MedicalItemInputGUID { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public bool ConfirmedReceived { get; set; }
        public Nullable<System.Guid> ProvidedByOrganizationInstanceGUID { get; set; }
        public Nullable<System.Guid> ProcuredByOrganizationInstanceGUID { get; set; }
        public Nullable<System.Guid> MedicalPharmacyGUID { get; set; }
        public bool Active { get; set; }
        public byte[] dataMedicalItemInputRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        public virtual codeMedicalPharmacy codeMedicalPharmacy { get; set; }
        public virtual codeOrganizationsInstances codeOrganizationsInstances { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataMedicalItemInputDetail> dataMedicalItemInputDetail { get; set; }
    }
}
