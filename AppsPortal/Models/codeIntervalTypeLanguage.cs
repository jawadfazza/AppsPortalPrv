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
    
    public partial class codeIntervalTypeLanguage
    {
        public System.Guid IntervalTypeLanguageGUID { get; set; }
        public Nullable<System.Guid> IntervalTypeGUID { get; set; }
        public string Description { get; set; }
        public string LanguageID { get; set; }
        public bool Active { get; set; }
        public byte[] codeIntervalTypeLanguagesRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        public virtual codeIntervalType codeIntervalType { get; set; }
    }
}
