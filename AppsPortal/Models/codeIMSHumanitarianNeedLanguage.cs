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
    
    public partial class codeIMSHumanitarianNeedLanguage
    {
        public System.Guid HumanitarianNeedLanguageLanguageGUID { get; set; }
        public Nullable<System.Guid> HumanitarianNeedGUID { get; set; }
        public string LanguageID { get; set; }
        public string HumanitarianNeedeDescription { get; set; }
        public bool Active { get; set; }
        public byte[] codeHumanitarianNeedLanguageRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
    
        public virtual codeIMSHumanitarianNeed codeIMSHumanitarianNeed { get; set; }
    }
}
