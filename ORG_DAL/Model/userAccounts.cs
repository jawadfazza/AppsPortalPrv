//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ORG_DAL.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class userAccounts
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public userAccounts()
        {
            this.dataStaffBankAccount = new HashSet<dataStaffBankAccount>();
            this.userPasswords = new HashSet<userPasswords>();
            this.userServiceHistory = new HashSet<userServiceHistory>();
        }
    
        public System.Guid UserGUID { get; set; }
        public string TimeZone { get; set; }
        public System.DateTime RequestedOn { get; set; }
        public System.Guid SecurityQuestionGUID { get; set; }
        public string SecurityAnswer { get; set; }
        public int AccountStatusID { get; set; }
        public bool IsFirstLogin { get; set; }
        public Nullable<bool> HasPhoto { get; set; }
        public bool Active { get; set; }
        public byte[] userAccountsRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataStaffBankAccount> dataStaffBankAccount { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<userPasswords> userPasswords { get; set; }
        public virtual userPersonalDetails userPersonalDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<userServiceHistory> userServiceHistory { get; set; }
    }
}