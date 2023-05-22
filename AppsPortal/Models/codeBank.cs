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
    
    public partial class codeBank
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeBank()
        {
            this.dataStaffBankAccount = new HashSet<dataStaffBankAccount>();
            this.StaffCoreDataBank = new HashSet<StaffCoreDataBank>();
            this.codeBankLanguage = new HashSet<codeBankLanguage>();
        }
    
        public System.Guid BankGUID { get; set; }
        public string BankCode { get; set; }
        public Nullable<System.Guid> CountryGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeBankRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataStaffBankAccount> dataStaffBankAccount { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StaffCoreDataBank> StaffCoreDataBank { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeBankLanguage> codeBankLanguage { get; set; }
    }
}
