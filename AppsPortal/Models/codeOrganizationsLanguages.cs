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
    
    public partial class codeOrganizationsLanguages
    {
        public System.Guid OrganizationLanguageGUID { get; set; }
        public System.Guid OrganizationGUID { get; set; }
        public string LanguageID { get; set; }
        public string OrganizationDescription { get; set; }
        public bool Active { get; set; }
        public byte[] codeOrganizationsLanguagesRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        public virtual codeOrganizations codeOrganizations { get; set; }
    }
}
