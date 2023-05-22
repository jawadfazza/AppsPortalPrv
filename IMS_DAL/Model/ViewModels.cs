using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace IMS_DAL.ViewModels
{
    public class MissionReportFormDataTableModel
    {
        public string Departments { get; set; }
        public string members { get; set; }
        public string ReportExtensionType { get; set; }

        public string visitsFormVisitObjectives { get; set; }
        [Display(Name = "MissionSourceOffice", ResourceType = typeof(resxDbFields))]
        public string MissionSourceOffice { get; set; }
        public string MissionSourceOfficeGUID { get; set; }
        public string MissionReportFormHumanitarianNeed { get; set; }
        public string MissionReportFormOngoingResponse { get; set; }
        public string MissionReportFormChallenge { get; set; }

        public List<string> AllDepartments { get; set; }
        public List<string> AllMembers { get; set; }


        public string DutyStationGUID { get; set; }


        public List<string> AllvisitsFormVisitObjectives { get; set; }
        public List<string> AllMissionReportFormHumanitarianNeed { get; set; }
        public List<string> AllMissionReportFormOngoingResponse { get; set; }
        public List<string> AllMissionReportFormChallenge { get; set; }


        [Display(Name = "MissionCode", ResourceType = typeof(resxDbFields))]

        public string MissionCode { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MissionStartDate", ResourceType = typeof(resxDbFields))]


        public DateTime? MissionStartDate { get; set; }

        [Display(Name = "MissionNumber", ResourceType = typeof(resxDbFields))]

        public int? MissionNumber { get; set; }

        public string CreatedByGUID { get; set; }

        public string Address { get; set; }
        [Display(Name = "MissionEndDate", ResourceType = typeof(resxDbFields))]


        public DateTime? MissionEndDate { get; set; }
        public System.Guid MissionReportFormGUID { get; set; }

        public string MissionLeaderGUID { get; set; }


        public string MissionStatusGUID { get; set; }
        [Display(Name = "DutyStation", ResourceType = typeof(resxDbFields))]
        public string DutyStation { get; set; }
        public string VisitObjectiveName { get; set; }

        public string HumanitarianNeeds { get; set; }

        public string OngoingResponses { get; set; }

        public string MissionChallenges { get; set; }
        [Display(Name = "Units", ResourceType = typeof(resxDbFields))]
        public string Units { get; set; }
        [Display(Name = "StaffMembers", ResourceType = typeof(resxDbFields))]
        public string StaffMembers { get; set; }
        public string GovernorateGUID { get; set; }

        public string DistrictGUID { get; set; }
        public string SubDistrictGUID { get; set; }
        public string CommunityGUID { get; set; }

        public byte[] dataMissionReportFormRowVersion { get; set; }

        [Display(Name = "Governorate", ResourceType = typeof(resxDbFields))]
        public string Governorate { get; set; }

        [Display(Name = "District", ResourceType = typeof(resxDbFields))]
        public string District { get; set; }

        [Display(Name = "SubDistrict", ResourceType = typeof(resxDbFields))]
        public string SubDistrict { get; set; }

        [Display(Name = "Community", ResourceType = typeof(resxDbFields))]

        public string Community { get; set; }

        [Display(Name = "MissionStatus", ResourceType = typeof(resxDbFields))]
        public string MissionStatus { get; set; }

        [Display(Name = "MissionLeaderGUID", ResourceType = typeof(resxDbFields))]
        public string MissionLeader { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public string CreatedBy { get; set; }


        [Display(Name = "CreatedDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreatedDate { get; set; }


        public bool Active { get; set; }
    }

    public class MissionReportFormUpdateModel
    {
        public MissionReportFormUpdateModel()
        {
            MissionFormPhotos = new List<MissionPhoto>();
            //missionActions = new List<MissionActions>();
        }
        //public List<MissionActions> missionActions { get; set; }
        public string ReportExtensionType { get; set; }
        [Display(Name = "Browse Images")]
        public List<HttpPostedFileBase> missionPhotos { get; set; }
        public HttpPostedFileBase UploadMissionFormFile { get; set; }

        public HttpPostedFileBase MissionPhoto1 { get; set; }
        public HttpPostedFileBase MissionPhoto2 { get; set; }
        public HttpPostedFileBase MissionPhoto3 { get; set; }
        public HttpPostedFileBase MissionPhoto4 { get; set; }
        [Display(Name = "MissionSourceOffice", ResourceType = typeof(resxDbFields))]
        public System.Guid MissionOfficeSourceGUID { get; set; }

        public List<MissionPhoto> MissionFormPhotos { get; set; }

        public string NVReference { get; set; }
        public bool IsMissionReportPublic { get; set; }

        public string MissionReportFullPath { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }
        [Display(Name = "MissionCode", ResourceType = typeof(resxDbFields))]

        public string MissionCode { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MissionStartDate", ResourceType = typeof(resxDbFields))]


        public DateTime? MissionStartDate { get; set; }

        public string showStaffMembers { get; set; }
        [Display(Name = "MissionNumber", ResourceType = typeof(resxDbFields))]

        public int? MissionNumber { get; set; }

        public string showVisitObjectives { get; set; }
        public string showHumanitarianNeeds { get; set; }
        public string showOngoingResponses { get; set; }


        public string showMissionChallenges { get; set; }

        public string showDepartments { get; set; }
        public string showMembers { get; set; }



        [Display(Name = "MissionEndDate", ResourceType = typeof(resxDbFields))]


        public DateTime? MissionEndDate { get; set; }


        [Display(Name = "MissionLeaderGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> MissionLeaderGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MissionStatusGUID", ResourceType = typeof(resxDbFields))]
        public Guid MissionStatusGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]

        [Display(Name = "GovernorateGUID", ResourceType = typeof(resxDbFields))]

        public System.Guid GovernorateGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]

        [Display(Name = "DistrictGUID", ResourceType = typeof(resxDbFields))]


        public System.Guid DistrictGUID { get; set; }

        [Display(Name = "FromDutyStationGUID", ResourceType = typeof(resxDbFields))]



        public Nullable<System.Guid> DutyStationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]

        [Display(Name = "SubDistrictGUID", ResourceType = typeof(resxDbFields))]

        public System.Guid SubDistrictGUID { get; set; }

        [Display(Name = "CommunityGUID", ResourceType = typeof(resxDbFields))]

        public Nullable<System.Guid> CommunityGUID { get; set; }

        public string Address { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]

        public double Latitude { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Longitude", ResourceType = typeof(resxDbFields))]
        public double Longitude { get; set; }
        //public float altitude { get; set; }
        //public float accuracy { get; set; }
        public Nullable<System.Guid> CreatedByGUID { get; set; }

        public DateTime? CreatedDate { get; set; }
        [Display(Name = "IsAnyPresenceOtherOrganization", ResourceType = typeof(resxDbFields))]
        public bool IsAnyPresenceOtherOrganization { get; set; }
        [Display(Name = "Gaps", ResourceType = typeof(resxDbFields))]
        public string Gaps { get; set; }
        [Display(Name = "Recommendations", ResourceType = typeof(resxDbFields))]
        public string Recommendations { get; set; }

        [Display(Name = "ActionRequired", ResourceType = typeof(resxDbFields))]
        public string ActionRequired { get; set; }




        [Display(Name = "ActionTaken", ResourceType = typeof(resxDbFields))]
        public string ActionTaken { get; set; }
        [Display(Name = "LinkToMissionReport", ResourceType = typeof(resxDbFields))]
        public string LinkToMissionReport { get; set; }
        [Display(Name = "IsThereMissionReport", ResourceType = typeof(resxDbFields))]
        public bool IsThereMissionReport { get; set; }

        [Display(Name = "Department", ResourceType = typeof(resxDbFields))]
        public List<Guid> MissionFormDepartments { get; set; }

        [Display(Name = "StaffParticipants", ResourceType = typeof(resxDbFields))]
        public List<Guid?> MissionStaffMembers { get; set; }

        [Display(Name = "VisitObjectives", ResourceType = typeof(resxDbFields))]
        public List<Guid> VisitObjectives { get; set; }
        [Display(Name = "CustomVisitObjectiveName", ResourceType = typeof(resxDbFields))]
        public string CustomVisitObjectiveName { get; set; }

        [Display(Name = "HumanitarianNeeds", ResourceType = typeof(resxDbFields))]
        public List<Guid?> HumanitarianNeeds { get; set; }

        [Display(Name = "CustomHumanitarianNeedName", ResourceType = typeof(resxDbFields))]

        public string CustomHumanitarianNeedName { get; set; }


        [Display(Name = "OngoingResponses", ResourceType = typeof(resxDbFields))]
        public List<Guid?> OngoingResponses { get; set; }

        [Display(Name = "CustomFormOngoingResponseName", ResourceType = typeof(resxDbFields))]

        public string CustomFormOngoingResponseName { get; set; }

        [Display(Name = "MissionChallenges", ResourceType = typeof(resxDbFields))]
        public List<Guid?> MissionChallenges { get; set; }


        [Display(Name = "CustomFormChallengeName", ResourceType = typeof(resxDbFields))]

        public string CustomFormChallengeName { get; set; }



        public byte[] dataMissionReportFormRowVersion { get; set; }
        public System.Guid MissionReportFormGUID { get; set; }





        public bool Active { get; set; }
    }
    public class MissionActionTempDataTable
    {
        public Nullable<Guid> TempMissionActionGUID { get; set; }
        public Nullable<Guid> MissionActionRequiredGUID { get; set; }
        public Nullable<Guid> MissionReportFormGUID { get; set; }
        public string DepartmentDescription { get; set; }

        public Nullable<Guid> MissionActionTakenGUID { get; set; }
        public string ActionRequiredName { get; set; }
        public string UnitName { get; set; }
        public string FocalPointName { get; set; }

        public Nullable<Guid> DepartmentGUID { get; set; }
        public Nullable<Guid> FocalPointGUID { get; set; }

        public string ActionTakenName { get; set; }
        public string ActionTakendStatus { get; set; }
        public DateTime? ActionTakenDate { get; set; }
        public Nullable<Guid> ActionStatusGUID { get; set; }
        public byte[] dataTempMissionActionRowVersion { get; set; }

    }
    public class MissionActionTempUpdateModel
    {
        [Display(Name = "MissionActionRequiredGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> MissionActionRequiredGUID { get; set; }
        [Display(Name = "MissionActionTakenGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> MissionActionTakenGUID { get; set; }
        [Display(Name = "MissionReportFormGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> MissionReportFormGUID { get; set; }
        [Display(Name = "TempMissionActionGUID", ResourceType = typeof(resxDbFields))]

        public Nullable<Guid> TempMissionActionGUID { get; set; }
        [Display(Name = "ActionRequiredName", ResourceType = typeof(resxDbFields))]
        public string ActionRequiredName { get; set; }
        [Display(Name = "UnitName", ResourceType = typeof(resxDbFields))]
        public string UnitName { get; set; }
        [Display(Name = "FocalPointName", ResourceType = typeof(resxDbFields))]
        public string FocalPointName { get; set; }
        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]

        public Nullable<Guid> DepartmentGUID { get; set; }
        [Display(Name = "FocalPointGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> FocalPointGUID { get; set; }
        [Display(Name = "ActionTakenName", ResourceType = typeof(resxDbFields))]

        public string ActionTakenName { get; set; }
        [Display(Name = "ActionTakendStatus", ResourceType = typeof(resxDbFields))]
        public string ActionTakendStatus { get; set; }
        [Display(Name = "ActionTakenDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ActionTakenDate { get; set; }
        [Display(Name = "ActionStatusGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> ActionStatusGUID { get; set; }

        public byte[] dataTempMissionActionRowVersion { get; set; }
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }
    }

    public class MissionActionDataTableModel
    {
        [Display(Name = "MissionActionRequiredGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> MissionActionRequiredGUID { get; set; }

        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> DepartmentGUID { get; set; }

        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> CreatedByGUID { get; set; }

        [Display(Name = "CreatedDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreatedDate { get; set; }
        [Display(Name = "ActionTakenDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ActionTakenDate { get; set; }
        [Display(Name = "ActionStatusGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> ActionStatusGUID { get; set; }

        [Display(Name = "ActionRequiredName", ResourceType = typeof(resxDbFields))]
        public string ActionRequiredName { get; set; }
        public Nullable<Guid> FocalPointGUID { get; set; }
        [Display(Name = "ActionTakenName", ResourceType = typeof(resxDbFields))]
        public string ActionTakenName { get; set; }


        [Display(Name = "MissionActionTakenGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> MissionActionTakenGUID { get; set; }
        [Display(Name = "MissionReportFormGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> MissionReportFormGUID { get; set; }
        [Display(Name = "TempMissionActionGUID", ResourceType = typeof(resxDbFields))]

        public Nullable<Guid> TempMissionActionGUID { get; set; }

        [Display(Name = "UnitName", ResourceType = typeof(resxDbFields))]
        public string UnitName { get; set; }
        [Display(Name = "FocalPointName", ResourceType = typeof(resxDbFields))]
        public string FocalPointName { get; set; }

        [Display(Name = "ActionTakendStatus", ResourceType = typeof(resxDbFields))]
        public string ActionTakendStatus { get; set; }



        public byte[] dataMissionActionRequiredRowVersion { get; set; }
        public byte[] dataMissionActionTakenRowVersion { get; set; }
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }
    }

    public class MissionActionUpdateModel
    {
        [Display(Name = "MissionActionRequiredGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> MissionActionRequiredGUID { get; set; }

        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> DepartmentGUID { get; set; }

        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> CreatedByGUID { get; set; }

        [Display(Name = "CreatedDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreatedDate { get; set; }
        [Display(Name = "ActionTakenDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ActionTakenDate { get; set; }
        [Display(Name = "ActionStatusGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> ActionStatusGUID { get; set; }

        [Display(Name = "ActionRequiredName", ResourceType = typeof(resxDbFields))]
        public string ActionRequiredName { get; set; }
        public Nullable<Guid> FocalPointGUID { get; set; }
        [Display(Name = "ActionTakenName", ResourceType = typeof(resxDbFields))]
        public string ActionTakenName { get; set; }


        [Display(Name = "MissionActionTakenGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> MissionActionTakenGUID { get; set; }
        [Display(Name = "MissionReportFormGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> MissionReportFormGUID { get; set; }
        [Display(Name = "TempMissionActionGUID", ResourceType = typeof(resxDbFields))]

        public Nullable<Guid> TempMissionActionGUID { get; set; }

        [Display(Name = "UnitName", ResourceType = typeof(resxDbFields))]
        public string UnitName { get; set; }
        [Display(Name = "FocalPointName", ResourceType = typeof(resxDbFields))]
        public string FocalPointName { get; set; }

        [Display(Name = "ActionTakendStatus", ResourceType = typeof(resxDbFields))]
        public string ActionTakendStatus { get; set; }



        public byte[] dataMissionActionRequiredRowVersion { get; set; }
        public byte[] dataMissionActionTakenRowVersion { get; set; }
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }
    }
    public class MissionActions
    {
        [Display(Name = "MissionActionRequiredGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> MissionActionRequiredGUID { get; set; }
        [Display(Name = "MissionActionTakenGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> MissionActionTakenGUID { get; set; }
        [Display(Name = "ActionRequiredName", ResourceType = typeof(resxDbFields))]
        public string ActionRequiredName { get; set; }
        [Display(Name = "UnitName", ResourceType = typeof(resxDbFields))]
        public string UnitName { get; set; }
        [Display(Name = "FocalPointName", ResourceType = typeof(resxDbFields))]
        public string FocalPointName { get; set; }
        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]

        public Nullable<Guid> DepartmentGUID { get; set; }
        [Display(Name = "FocalPointGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> FocalPointGUID { get; set; }
        [Display(Name = "ActionTakendName", ResourceType = typeof(resxDbFields))]
        public string ActionTakendName { get; set; }
        [Display(Name = "ActionTakendStatus", ResourceType = typeof(resxDbFields))]
        public string ActionTakendStatus { get; set; }
        [Display(Name = "ActionTakenDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ActionTakenDate { get; set; }
        [Display(Name = "ActionStatusGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> ActionStatusGUID { get; set; }
    }

    public class MissionPhoto
    {
        public string Photo { get; set; }
    }

    #region Graphs



    public class Months
    {
        public string Name { set; get; }
        public int MonthOrder { set; get; }

    }
    public class drilldown
    {
        public string name { set; get; }
        public Guid Guid { set; get; }
        public data[] data { get; set; }
    }
    public class data
    {
        public double y { set; get; }
    }
    public class HighChartPieModel
    {
        public string name { set; get; }
        public double y { set; get; }
        public bool selected { get; set; }

        public bool sliced { get; set; }
    }
    #endregion

    #region import data table

    public class missonFormDataTableModel
    {
        public Nullable<System.Guid> MissionLeaderGUID { get; set; }
        [Display(Name = "MissionNumber", ResourceType = typeof(resxDbFields))]

        public int? MissionNumber { get; set; }

        [Display(Name = "MissionCode", ResourceType = typeof(resxDbFields))]

        public string MissionCode { get; set; }
        public System.Guid MissionReportFormGUID { get; set; }
        [Display(Name = "MissionStartDate", ResourceType = typeof(resxDbFields))]


        public DateTime? MissionStartDate { get; set; }

        [Display(Name = "MissionEndDate", ResourceType = typeof(resxDbFields))]



        public DateTime? MissionEndDate { get; set; }

        [Display(Name = "DutyStation", ResourceType = typeof(resxDbFields))]
        public string DutyStation { get; set; }


        [Display(Name = "Governorate", ResourceType = typeof(resxDbFields))]
        public string Governorate { get; set; }
        [Display(Name = "District", ResourceType = typeof(resxDbFields))]
        public string District { get; set; }

        [Display(Name = "SubDistrict", ResourceType = typeof(resxDbFields))]
        public string SubDistrict { get; set; }

        [Display(Name = "Community", ResourceType = typeof(resxDbFields))]
        public string Community { get; set; }

        [Display(Name = "Address", ResourceType = typeof(resxDbFields))]
        public string Address { get; set; }

        [Display(Name = "Longitude", ResourceType = typeof(resxDbFields))]
        public float? Longitude { get; set; }

        [Display(Name = "Latitude", ResourceType = typeof(resxDbFields))]
        public float? Latitude { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public string CreatedBy { get; set; }

        [Display(Name = "CreatedDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreatedDate { get; set; }


        public bool? Is_AnyPresence_Other_Organization_ { get; set; }
        public string Gaps { get; set; }
        public string Recommendations { get; set; }
        public string ActionRequired { get; set; }

        public string LinkToMissionReport { get; set; }
        public bool? Is_There_Mission_Report_ { get; set; }
        public string Departments { get; set; }
        public string members { get; set; }
        public string visitsFormVisitObjectives { get; set; }
        public string MissionReportFormHumanitarianNeed { get; set; }
        public string MissionReportFormOngoingResponse { get; set; }
        public string MissionReportFormChallenge { get; set; }
        public string Coordinates { get; set; }

        public List<string> AllDepartments { get; set; }
        public List<string> AllMembers { get; set; }


        public List<string> AllvisitsFormVisitObjectives { get; set; }
        public List<string> AllMissionReportFormHumanitarianNeed { get; set; }
        public List<string> AllMissionReportFormOngoingResponse { get; set; }
        public List<string> AllMissionReportFormChallenge { get; set; }



    }
    #endregion

    #region MissionActions 
    public class MissionActionOverviewDataTable
    {
        public string GovernorateGUID { get; set; }
        public Nullable<Guid> MissionReportFormGUID { get; set; }
        public Nullable<Guid> MissionActionTakenGUID { get; set; }
        public Nullable<Guid> MissionActionRequiredGUID { get; set; }
        [Display(Name = "ActionRequiredName", ResourceType = typeof(resxDbFields))]
        public string ActionRequiredName { get; set; }
        [Display(Name = "UnitName", ResourceType = typeof(resxDbFields))]
        public string UnitName { get; set; }
        [Display(Name = "FocalPointName", ResourceType = typeof(resxDbFields))]
        public string FocalPointName { get; set; }
        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]

        public string DepartmentGUID { get; set; }
        [Display(Name = "FocalPointGUID", ResourceType = typeof(resxDbFields))]
        public string FocalPointGUID { get; set; }
        [Display(Name = "ActionTakenName", ResourceType = typeof(resxDbFields))]

        public string ActionTakenName { get; set; }
        [Display(Name = "ActionTakenStatus", ResourceType = typeof(resxDbFields))]
        public string ActionTakenStatus { get; set; }
        [Display(Name = "ActionTakenDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ActionTakenDate { get; set; }
        [Display(Name = "ActionStatusGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> ActionStatusGUID { get; set; }

        public string MissionCode { get; set; }
        [Display(Name = "Governorate", ResourceType = typeof(resxDbFields))]
        public string Governorate { get; set; }

        [Display(Name = "District", ResourceType = typeof(resxDbFields))]
        public string District { get; set; }

        [Display(Name = "SubDistrict", ResourceType = typeof(resxDbFields))]
        public string SubDistrict { get; set; }

        [Display(Name = "Community", ResourceType = typeof(resxDbFields))]
        public string Community { get; set; }
        public byte[] dataMissionActionTakenRowVersion { get; set; }

        public int? MissionNumber { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? MissionStartDate { get; set; }
        public string MissionStatusGUID { get; set; }

    }

    #endregion

    public class CalanderVM
    {
        public string id { get; set; }
        public string title { get; set; }
        public int d { get; set; }
        public int m { get; set; }
        public int y { get; set; }
        public int totalevent { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public bool? allDay { get; set; }
        public string backgroundColor { get; set; }
        public string borderColor { get; set; }

    }

    #region MissionReports
    public class ModelTrackReportModel
    {
        [Display(Name = "MissionStartDate", ResourceType = typeof(resxDbFields))]


        public DateTime? MissionStartFromDate { get; set; }

        [Display(Name = "MissionEndDate", ResourceType = typeof(resxDbFields))]


        public DateTime? MissionStartToDate { get; set; }


        [Display(Name = "Governorate", ResourceType = typeof(resxDbFields))]

        public List<Guid?> Governorates { get; set; }

        [Display(Name = "MissionStatusGUIDs", ResourceType = typeof(resxDbFields))]

        public List<Guid?> MissionStatusGUIDs { get; set; }





    }
    #endregion

    #region Mission Report Documents

    public class MissionReportFormDocumentDataTable
    {
        [Required]
        [Display(Name = "MissionReportDocumentGUID", ResourceType = typeof(resxDbFields))]
        public Guid MissionReportDocumentGUID { get; set; }

        [Required]
        [Display(Name = "MissionReportFormGUID", ResourceType = typeof(resxDbFields))]
        public string MissionReportFormGUID { get; set; }

        [Required]
        [Display(Name = "MissionReportTypeGUID", ResourceType = typeof(resxDbFields))]
        public string MissionReportTypeGUID { get; set; }


        [Display(Name = "DocumentPath", ResourceType = typeof(resxDbFields))]
        public string DocumentPath { get; set; }


        [Display(Name = "DocumentType", ResourceType = typeof(resxDbFields))]
        public string DocumentType { get; set; }



        [Display(Name = "ReportExtensionType", ResourceType = typeof(resxDbFields))]
        public string ReportExtensionType { get; set; }

        [Display(Name = "DocumentDescription", ResourceType = typeof(resxDbFields))]

        public string DocumentDescription { get; set; }



        [Display(Name = "CreatedDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreatedDate { get; set; }




        [Required]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }


        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataMissionReportDocumentRowVersion { get; set; }
    }
    public class MissionReportFormDocumentUpdateModel
    {
        [Required]
        [Display(Name = "MissionReportDocumentGUID", ResourceType = typeof(resxDbFields))]
        public Guid MissionReportDocumentGUID { get; set; }

        [Required]
        [Display(Name = "MissionReportFormGUID", ResourceType = typeof(resxDbFields))]
        public Guid MissionReportFormGUID { get; set; }

        [Required]
        [Display(Name = "MissionReportTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid MissionReportTypeGUID { get; set; }


        [Display(Name = "DocumentPath", ResourceType = typeof(resxDbFields))]
        public string DocumentPath { get; set; }


        [Display(Name = "DocumentType", ResourceType = typeof(resxDbFields))]
        public string DocumentType { get; set; }



        [Display(Name = "ReportExtensionType", ResourceType = typeof(resxDbFields))]
        public string ReportExtensionType { get; set; }

        [Display(Name = "DocumentDescription", ResourceType = typeof(resxDbFields))]

        public string DocumentDescription { get; set; }



        [Display(Name = "CreatedDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreatedDate { get; set; }




        [Required]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }


        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataMissionReportDocumentRowVersion { get; set; }
    }

    #endregion


}
