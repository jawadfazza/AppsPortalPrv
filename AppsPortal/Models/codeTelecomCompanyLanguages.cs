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
    
    public partial class codeTelecomCompanyLanguages
    {
        public System.Guid TelecomCompanyLanguageGUID { get; set; }
        public System.Guid TelecomCompanyGUID { get; set; }
        public string TelecomCompanyDescription { get; set; }
        public string LanguageID { get; set; }
        public bool Active { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public byte[] codeTelecomCompanyLanguagesRowVersion { get; set; }
    
        public virtual codeTelecomCompany codeTelecomCompany { get; set; }
    }
}