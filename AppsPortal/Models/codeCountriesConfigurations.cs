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
    
    public partial class codeCountriesConfigurations
    {
        public System.Guid CountryConfigurationGUID { get; set; }
        public System.Guid CountryGUID { get; set; }
        public System.Guid OrganizationInstanceGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeCountriesConfigurationsRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        public virtual codeCountries codeCountries { get; set; }
        public virtual codeOrganizationsInstances codeOrganizationsInstances { get; set; }
    }
}
