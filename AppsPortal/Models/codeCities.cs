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
    
    public partial class codeCities
    {
        public System.Guid CityGUID { get; set; }
        public System.Guid CountryGUID { get; set; }
        public string CityDescription { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool Active { get; set; }
        public byte[] codeCitiesRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    }
}
