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
    
    public partial class codeOfficesLanguages
    {
        public System.Guid OfficeLanguageGUID { get; set; }
        public System.Guid OfficeGUID { get; set; }
        public string LanguageID { get; set; }
        public string OfficeDescription { get; set; }
        public string OfficeAddress { get; set; }
        public bool Active { get; set; }
        public byte[] codeOfficesLanguagesRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        public virtual codeOffices codeOffices { get; set; }
    }
}
