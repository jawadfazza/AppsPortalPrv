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
    
    public partial class codeSipAnalyticalValueLanguages
    {
        public System.Guid SipAnalyticalValueLanguageGUID { get; set; }
        public System.Guid SipAnalyticalValueGUID { get; set; }
        public string LanguageID { get; set; }
        public string SipAnalyticalValueDescription { get; set; }
        public bool Active { get; set; }
        public byte[] codeSipAnalyticalValueLanguagesRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        public virtual codeSipAnalyticalValue codeSipAnalyticalValue { get; set; }
    }
}