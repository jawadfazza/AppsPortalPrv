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
    
    public partial class codeOrganizationHoliday
    {
        public System.Guid OrganizationHolidayGUID { get; set; }
        public System.Guid OrganizationInstanceGUID { get; set; }
        public string HolidayName { get; set; }
        public System.DateTime HolidayStartDate { get; set; }
        public System.DateTime HolidayEndDate { get; set; }
        public byte[] codeOrganizationHolidayRowVersion { get; set; }
        public bool Active { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    }
}