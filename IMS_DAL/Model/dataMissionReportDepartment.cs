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
    
    public partial class dataMissionReportDepartment
    {
        public System.Guid MissionReportDepartmentGUID { get; set; }
        public Nullable<System.Guid> MissionReportFormGUID { get; set; }
        public Nullable<System.Guid> DepartmentGUID { get; set; }
        public bool Active { get; set; }
        public byte[] dataMissionReportDepartmentRowVersion { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
    
        public virtual codeDepartments codeDepartments { get; set; }
        public virtual dataMissionReportForm dataMissionReportForm { get; set; }
    }
}
