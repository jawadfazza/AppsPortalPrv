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
    
    public partial class codeReferralStatusLanguage
    {
        public System.Guid ReferralStatusLanguageGUID { get; set; }
        public Nullable<System.Guid> ReferralStatusGUID { get; set; }
        public string LanguageID { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public byte[] codeReferralStatusLanguageRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        public virtual codeReferralStatus codeReferralStatus { get; set; }
    }
}
