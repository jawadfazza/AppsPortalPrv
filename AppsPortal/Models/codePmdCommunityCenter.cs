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
    
    public partial class codePmdCommunityCenter
    {
        public System.Guid PMDCommunityCenterGUID { get; set; }
        public string admin1Pcode { get; set; }
        public string admin2Pcode { get; set; }
        public string admin3Pcode { get; set; }
        public string admin4Pcode { get; set; }
        public string PMDCommunityCenter { get; set; }
        public Nullable<double> Latitude_y { get; set; }
        public Nullable<double> Longitude_x { get; set; }
        public bool Active { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public byte[] codePmdCommunityCenterRowVersion { get; set; }
    }
}
