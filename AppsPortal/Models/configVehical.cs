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
    
    public partial class configVehical
    {
        public System.Guid VehicalGUID { get; set; }
        public string VehicalNumber { get; set; }
        public Nullable<System.Guid> VehicleTypeGUID { get; set; }
        public Nullable<System.Guid> VechileModelGUID { get; set; }
        public Nullable<System.Guid> VehileColorGUID { get; set; }
        public Nullable<int> ManufacturingYear { get; set; }
        public Nullable<int> PlateNumber { get; set; }
        public string ChassisNumber { get; set; }
        public string EngineNumber { get; set; }
        public Nullable<System.DateTime> LastRenewalDate { get; set; }
        public bool Available { get; set; }
        public string Comment { get; set; }
        public Nullable<System.Guid> OrganizationInstanceGUID { get; set; }
        public Nullable<System.Guid> DutyStationGUID { get; set; }
        public bool Active { get; set; }
        public byte[] configVehicalRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    }
}
