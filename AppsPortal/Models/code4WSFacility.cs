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
    
    public partial class code4WSFacility
    {
        public System.Guid FacilityGUID { get; set; }
        public string FacilityOperatedBy { get; set; }
        public string FacilityCode { get; set; }
        public string FacilityAbbreviation { get; set; }
        public string FacilityType { get; set; }
        public string Governorate { get; set; }
        public string SubDistrict { get; set; }
        public string Community { get; set; }
        public bool Active { get; set; }
        public byte[] code4WSFacilityRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    }
}