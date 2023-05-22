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
    
    public partial class userServiceHistory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public userServiceHistory()
        {
            this.userProfiles = new HashSet<userProfiles>();
        }
    
        public System.Guid ServiceHistoryGUID { get; set; }
        public System.Guid UserGUID { get; set; }
        public System.Guid OrganizationGUID { get; set; }
        public string EmployeeNumber { get; set; }
        public string IndexNumber { get; set; }
        public string EmailAddress { get; set; }
        public bool Active { get; set; }
        public byte[] userServiceHistoryRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        public virtual userAccounts userAccounts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<userProfiles> userProfiles { get; set; }
    }
}