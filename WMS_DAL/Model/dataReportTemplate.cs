//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WMS_DAL.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class dataReportTemplate
    {
        public int ReportTemplateId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> GeneratedDate { get; set; }
        public Nullable<int> ReportCategoryId { get; set; }
        public Nullable<int> ReviewsCount { get; set; }
        public Nullable<int> ReportTypeId { get; set; }
        public string ReportQuery { get; set; }
        public int ReportId { get; set; }
        public bool Active { get; set; }
        public byte[] dataReportTemplateRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
        public Nullable<System.Guid> OrganizationInstanceGUID { get; set; }
    }
}