//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IMS_DAL.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class codeDutyStationsLanguages
    {
        public System.Guid DutyStationLanguageGUID { get; set; }
        public System.Guid DutyStationGUID { get; set; }
        public string LanguageID { get; set; }
        public string DutyStationDescription { get; set; }
        public bool Active { get; set; }
        public byte[] codeDutyStationsLanguagesRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    
        public virtual codeDutyStations codeDutyStations { get; set; }
    }
}
