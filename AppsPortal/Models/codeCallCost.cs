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
    
    public partial class codeCallCost
    {
        public System.Guid CallCostGUID { get; set; }
        public System.Guid SourceCountryGUID { get; set; }
        public int SourceCountryCode { get; set; }
        public Nullable<int> SourceAreaCode { get; set; }
        public System.Guid DestinationCountryGUID { get; set; }
        public int DestinationCountryCode { get; set; }
        public Nullable<int> DestinationAreaCode { get; set; }
        public Nullable<System.Guid> CallTypeGUID { get; set; }
        public double CallCost { get; set; }
        public bool Active { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public byte[] codeCallCostRowVersion { get; set; }
    }
}
