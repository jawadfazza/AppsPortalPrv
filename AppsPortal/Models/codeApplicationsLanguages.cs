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
    
    public partial class codeApplicationsLanguages
    {
        public System.Guid ApplicationLanguageGUID { get; set; }
        public System.Guid ApplicationGUID { get; set; }
        public string LanguageID { get; set; }
        public string ApplicationDescription { get; set; }
        public bool Active { get; set; }
        public byte[] codeApplicationsLanguagesRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        public virtual codeApplications codeApplications { get; set; }
    }
}
