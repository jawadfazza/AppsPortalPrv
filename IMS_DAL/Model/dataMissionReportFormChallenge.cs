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
    
    public partial class dataMissionReportFormChallenge
    {
        public System.Guid MissionReportFormChallengeGUID { get; set; }
        public string ChallengeName { get; set; }
        public Nullable<System.Guid> MissionChallengeGUID { get; set; }
        public Nullable<System.Guid> MissionReportFormGUID { get; set; }
        public byte[] dataMissionReportFormChallengeRowVersion { get; set; }
        public bool Active { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> Archived { get; set; }
    
        public virtual codeIMSMissionChallenge codeIMSMissionChallenge { get; set; }
        public virtual dataMissionReportForm dataMissionReportForm { get; set; }
    }
}