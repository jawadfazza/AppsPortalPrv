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
    
    public partial class codeDutyStationsConfigurations
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeDutyStationsConfigurations()
        {
            this.codeWorkingDaysConfigurations = new HashSet<codeWorkingDaysConfigurations>();
        }
    
        public System.Guid DutyStationConfigurationGUID { get; set; }
        public System.Guid OrganizationInstanceGUID { get; set; }
        public System.Guid DutyStationGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeDutyStationsConfigurationsRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        public virtual codeDutyStations codeDutyStations { get; set; }
        public virtual codeOrganizationsInstances codeOrganizationsInstances { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeWorkingDaysConfigurations> codeWorkingDaysConfigurations { get; set; }
    }
}