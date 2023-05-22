using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRS_DAL.Model
{

    public class MRSReportParametersList
    {
        [Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime? StartDate { get; set; }
        [Display(Name = "EndDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime? EndDate { get; set; }
    }
    public class NoteVerbalesDataTableModel
    {
        public System.Guid NoteVerbaleGUID { get; set; }
        public string Reference { get; set; }
        public System.Guid LocationGUID { get; set; }
        public string LocationDescription { get; set; }
        public System.DateTime VisitDate { get; set; }
        public Nullable<System.DateTime> NoteVerbaleDate { get; set; }
        public string ReferralStatusGUID { get; set; }
        public string ReferralStatusDescription { get; set; }
        public Nullable<System.Guid> OrganizationInstanceGUID { get; set; }
        public string DutyStationGUID { get; set; }
        public string DutyStationDescription { get; set; }
        public string MissionCategoryGUID { get; set; }
        public string MissionCategoryDescription { get; set; }
        public string OrganizationMissionTypeGUID { get; set; }
        public string OrganizationMissionTypeDescription { get; set; }
        public string TeamLeaderGUID { get; set; }
        public string TeamLeaderName { get; set; }
        public Nullable<System.DateTime> ResponseDateMFA { get; set; }
        public Nullable<System.DateTime> RescheduledDate { get; set; }
        public bool MissionAccomplished { get; set; }
        public bool MissionReport { get; set; }
        public bool Active { get; set; }
        public bool Map { get; set; }
        public byte[] dataNoteVerbaleRowVersion { get; set; }
    }

    public class NoteVerbaleUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "NoteVerbaleGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid NoteVerbaleGUID { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Reference", ResourceType = typeof(resxDbFields))]
        public string Reference { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "LocationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid LocationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.Date)]
        [Display(Name = "VisitDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime? VisitDate { get; set; }

        [StringLength(1000, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "VisitPurpose_AR", ResourceType = typeof(resxDbFields))]
        public string VisitPurpose_AR { get; set; }

        [StringLength(1000, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "VisitPurpose_EN", ResourceType = typeof(resxDbFields))]
        public string VisitPurpose_EN { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "NoteVerbaleDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> NoteVerbaleDate { get; set; }

        [Display(Name = "ReferralStatusGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ReferralStatusGUID { get; set; }

        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> OrganizationInstanceGUID { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DutyStationGUID { get; set; }

        [Display(Name = "MissionCategoryGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> MissionCategoryGUID { get; set; }

        [Display(Name = "OrganizationMissionTypeGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> OrganizationMissionTypeGUID { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "ResponseDateMFA", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> ResponseDateMFA { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "RescheduledDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> RescheduledDate { get; set; }

        [Display(Name = "TeamLeaderGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> TeamLeaderGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MissionAccomplished", ResourceType = typeof(resxDbFields))]
        public bool MissionAccomplished { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MissionReport", ResourceType = typeof(resxDbFields))]
        public bool MissionReport { get; set; }

        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public bool Map { get; set; }

        public byte[] dataNoteVerbaleRowVersion { get; set; }
    }

    public class NoteVerbaleStaffDataTable
    {
        public Guid NoteVerbaleStaffGUID { get; set; }
        public System.Guid NoteVerbaleGUID { get; set; }
        public string UserGUID { get; set; }
        public Nullable<int> Sequance { get; set; }
        public byte[] dataNoteVerbaleStaffRowVersion { get; set; }
        public bool Active { get; set; }
    }

    public class NoteVerbaleVehicleDataTable
    {
        public System.Guid NoteVerbaleVehicleGUID { get; set; }
        public System.Guid NoteVerbaleGUID { get; set; }
        public string UserGUID { get; set; }
        public Nullable<System.Guid> VehicleGUID { get; set; }
        public string VehicleNumber { get; set; }
        public bool Active { get; set; }
        public byte[] dataNoteVerbaleVehicleRowVersion { get; set; }
    }

    public class NoteVerbaleOrganizationsDataTableModel
    {
        public System.Guid NoteVerbaleOrganizationGUID { get; set; }
        public System.Guid NoteVerbaleGUID { get; set; }
        public Nullable<System.Guid> OrganizationInstanceGUID { get; set; }
        public string OrganizationInstanceDescription { get; set; }
        public bool Active { get; set; }
        public byte[] dataNoteVerbaleOrganizationRowVersion { get; set; }

    }

    public class NoteVerbaleOrganizationUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "NoteVerbaleOrganizationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid NoteVerbaleOrganizationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "NoteVerbaleGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid NoteVerbaleGUID { get; set; }

        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> OrganizationInstanceGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataNoteVerbaleOrganizationRowVersion { get; set; }

    }
}
