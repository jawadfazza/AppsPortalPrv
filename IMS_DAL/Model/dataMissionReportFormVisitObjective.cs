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
    
    public partial class dataMissionReportFormVisitObjective
    {
        public System.Guid MissionReportFormVisitObjectiveGUID { get; set; }
        public string VisitObjectiveName { get; set; }
        public Nullable<System.Guid> VisitObjectiveGUID { get; set; }
        public Nullable<System.Guid> MissionReportFormGUID { get; set; }
        public byte[] dataMissionReportFormVisitObjectiveRowVersion { get; set; }
        public bool Active { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
    
        public virtual codeIMSVisitObjective codeIMSVisitObjective { get; set; }
        public virtual dataMissionReportForm dataMissionReportForm { get; set; }
    }
}
