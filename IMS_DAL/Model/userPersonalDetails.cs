//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IMS_DAL.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class userPersonalDetails
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public userPersonalDetails()
        {
            this.userPersonalDetailsLanguage = new HashSet<userPersonalDetailsLanguage>();
        }
    
        public System.Guid UserGUID { get; set; }
        public Nullable<System.Guid> CountryGUID { get; set; }
        public Nullable<System.Guid> GenderGUID { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string PreferedLanguageID { get; set; }
        public string BloodGroup { get; set; }
        public bool Active { get; set; }
        public byte[] userPersonalDetailsRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        public virtual userAccounts userAccounts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<userPersonalDetailsLanguage> userPersonalDetailsLanguage { get; set; }
    }
}
