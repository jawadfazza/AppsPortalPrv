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
    
    public partial class codeSipIndicatorSector
    {
        public System.Guid SipIndicatorSectorGUID { get; set; }
        public System.Guid SipIndicatorGUID { get; set; }
        public System.Guid SipSectorGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeSipIndicatorSectorRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        public virtual codeSipIndicator codeSipIndicator { get; set; }
        public virtual codeSipSector codeSipSector { get; set; }
    }
}
