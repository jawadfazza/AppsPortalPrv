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
    
    public partial class userWorkAddressLanguage
    {
        public System.Guid WorkAddressLanguageGUID { get; set; }
        public System.Guid UserGUID { get; set; }
        public string LanguageID { get; set; }
        public string AreaName { get; set; }
        public string BuildingName { get; set; }
        public bool Active { get; set; }
        public byte[] userWorkAddressLanguageRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        public virtual userAccounts userAccounts { get; set; }
    }
}