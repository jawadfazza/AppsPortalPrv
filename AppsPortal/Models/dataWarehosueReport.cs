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
    
    public partial class dataWarehosueReport
    {
        public System.Guid WarehosueReportGUID { get; set; }
        public Nullable<int> WarehosueReportId { get; set; }
        public string ReportName { get; set; }
        public bool Active { get; set; }
        public byte[] dataWarehosueReportRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
    }
}
