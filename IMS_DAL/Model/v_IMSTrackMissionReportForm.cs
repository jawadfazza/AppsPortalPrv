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
    
    public partial class v_IMSTrackMissionReportForm
    {
        public System.Guid MissionReportFormGUID { get; set; }
        public string MissionCode { get; set; }
        public Nullable<int> MissionNumber { get; set; }
        public Nullable<System.Guid> MissionStatusGUID { get; set; }
        public Nullable<System.Guid> MissionLeaderGUID { get; set; }
        public Nullable<System.Guid> CreatedByGUID { get; set; }
        public string Address { get; set; }
        public System.DateTime MissionStartDate { get; set; }
        public Nullable<System.DateTime> MissionEndDate { get; set; }
        public byte[] dataMissionReportFormRowVersion { get; set; }
        public string Comments { get; set; }
        public Nullable<double> longitude { get; set; }
        public Nullable<double> Latitude { get; set; }
        public string Governorate { get; set; }
        public Nullable<System.Guid> GovernorateGUID { get; set; }
        public bool Active { get; set; }
        public string ReportExtensionType { get; set; }
        public Nullable<System.Guid> MissionOfficeSourceGUID { get; set; }
        public string MissionOfficeSource { get; set; }
        public Nullable<System.Guid> DistrictGUID { get; set; }
        public string District { get; set; }
        public Nullable<System.Guid> SubDistrictGUID { get; set; }
        public string SubDistrict { get; set; }
        public Nullable<System.Guid> CommunityGUID { get; set; }
        public string Community { get; set; }
        public Nullable<int> MonthMissionStartDate { get; set; }
        public Nullable<int> YearMissionStartDate { get; set; }
        public string gaps { get; set; }
        public string Recommendations { get; set; }
        public string MissionLeader { get; set; }
        public string MissionStatus { get; set; }
        public string CreatedBy { get; set; }
        public string VisitObjectives { get; set; }
        public string VisitCategoryObjectives { get; set; }
        public string HumanitarianNeeds { get; set; }
        public string OngoingResponses { get; set; }
        public string MissionChallenges { get; set; }
        public string Units { get; set; }
        public string StaffMembers { get; set; }
    }
}
