using RES_Repo.Globalization;
using System;
using System.ComponentModel.DataAnnotations;


namespace AppsPortal.AMS.ViewModels
{
    public class ReportParametersList
    {
        public string Report { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string[] AppointmentTypeGUID { get; set; }
        public Guid? DepartmentGUID { get; set; }
        public Guid? DutyStationGUID { get; set; }
        public Guid? OrganizationInstanceGUID { get; set; }
    }
    public class AppointmentTypesDataTableModel
    {
        public Guid AppointmentTypeGUID { get; set; }
        public string Code { get; set; }
        public string AppointmentTypeDescription { get; set; }
        public string DepartmentDescription { get; set; }
        public bool Active { get; set; }
        public byte[] codeAppointmentTypeRowVersion { get; set; }
    }

    public class AppointmentTypeUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "AppointmentTypeGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid AppointmentTypeGUID { get; set; }

        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DepartmentGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(20, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Code", ResourceType = typeof(resxDbFields))]
        public string Code { get; set; }

        [Display(Name = "Sort", ResourceType = typeof(resxDbFields))]
        public Nullable<int> Sort { get; set; }

        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(250, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "AppointmentTypeDescription", ResourceType = typeof(resxDbFields))]
        public string AppointmentTypeDescription { get; set; }

        public byte[] codeAppointmentTypeRowVersion { get; set; }
        public byte[] codeAppointmentTypeLanguageRowVersion { get; set; }
    }

    public class CaseUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CaseGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid CaseGUID { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "HouseHold", ResourceType = typeof(resxDbFields))]
        public string HouseHold { get; set; }

        [Display(Name = "FileNumber", ResourceType = typeof(resxDbFields))]
        public string FileNumber { get; set; }

        [StringLength(250, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ICNameOtherLanguages", ResourceType = typeof(resxDbFields))]
        public string ICNameOtherLanguages { get; set; }

        [StringLength(250, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ICNameEN", ResourceType = typeof(resxDbFields))]
        public string ICNameEN { get; set; }

        [Display(Name = "CaseSize", ResourceType = typeof(resxDbFields))]
        public Nullable<int> CaseSize { get; set; }

        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> CountryGUID { get; set; }
        public string CountryCode { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DutyStationGUID { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Display(Name = "ProcessStatusName", ResourceType = typeof(resxDbFields))]
        public string ProcessStatusName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataCaseRowVersion { get; set; }
    }

    public class AppointmentUpdateModel
    {
        public string FileNumber { get; set; }
        public string ICNameOtherLanguages { get; set; }
        public string ICNameEN { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "AppointmentGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid AppointmentGUID { get; set; }

        [Display(Name = "AppointmentTypeCalenderGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid? AppointmentTypeCalenderGUID { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CaseGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid CaseGUID { get; set; }

        [Display(Name = "AppointmentWithUserGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> AppointmentWithUserGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "AppointmentTypeGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid AppointmentTypeGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.DateTime)]
        [Display(Name = "AppointmentDateTime", ResourceType = typeof(resxDbFields))]
        public System.DateTime? AppointmentDateTime { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Arrived", ResourceType = typeof(resxDbFields))]
        public bool Arrived { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Cancelled", ResourceType = typeof(resxDbFields))]
        public bool Cancelled { get; set; }

        [Display(Name = "Priority", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> Priority { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TokenNumber", ResourceType = typeof(resxDbFields))]
        public string TokenNumber { get; set; }

        [Display(Name = "TokenColourGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> TokenColourGUID { get; set; }

        [Display(Name = "StatusGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> StatusGUID { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DutyStationGUID { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Reason", ResourceType = typeof(resxDbFields))]
        public string Reason { get; set; }
        public string ReasonHistory { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public string CreatedBy { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "CreatedDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> CreatedDate { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "UpdatedBy", ResourceType = typeof(resxDbFields))]
        public string UpdatedBy { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "UpdatedDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> UpdatedDate { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ArrivedBy", ResourceType = typeof(resxDbFields))]
        public string ArrivedBy { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "ArrivedDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> ArrivedDate { get; set; }


        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CancelledBy", ResourceType = typeof(resxDbFields))]
        public string CancelledBy { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "CancelledDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> CancelledDate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataAppointmentRowVersion { get; set; }
    }

    public class CasesDataTableModel
    {
        public System.Guid CaseGUID { get; set; }
        public string FileNumber { get; set; }
        public string ICNameOtherLanguages { get; set; }
        public string ICNameEN { get; set; }
        public Nullable<int> CaseSize { get; set; }
        public string CountryDescription { get; set; }

        public string PhoneNumber { get; set; }
        public string CountryGUID { get; set; }
        public bool Active { get; set; }
        public byte[] dataCaseRowVersion { get; set; }
    }

    public class AppointmentsDataTableModel
    {
        public System.Guid AppointmentGUID { get; set; }
        public System.Guid CaseGUID { get; set; }
        public  string AppointmentTypeDescription { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public bool Arrived { get; set; }
        public bool Cancelled { get; set; }
        public bool Active { get; set; }
        public byte[] dataAppointmentRowVersion { get; set; }
    }

    public class ContactInfoUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ContactInfoGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ContactInfoGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CaseGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid CaseGUID { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PhoneNumber", ResourceType = typeof(resxDbFields))]
        public string PhoneNumber { get; set; }

        [Display(Name = "PhoneType", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> PhoneType { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataContactInfoRowVersion { get; set; }

    }

    public class ContactInfosDataTableModel
    {
        public System.Guid ContactInfoGUID { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneType { get; set; }
        public bool Active { get; set; }
        public byte[] dataContactInfoRowVersion { get; set; }
    }

    public class AppointmentTypeCalenderUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "AppointmentTypeCalenderGUID", ResourceType = typeof(resxDbFields))]
        public Guid AppointmentTypeCalenderGUID { get; set; }

        
        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
        public Guid DepartmentGUID { get; set; }

        
        [Display(Name = "AppointmentTypeGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> AppointmentTypeGUID { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.DateTime)]
        [Display(Name = "EventStartDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime? EventStartDate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.DateTime)]
        [Display(Name = "EventEndDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime? EventEndDate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SlotAvailable", ResourceType = typeof(resxDbFields))]
        public int SlotAvailable { get; set; }

        [Display(Name = "EventEachDay", ResourceType = typeof(resxDbFields))]
        public bool EventEachDay { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SlotClosed", ResourceType = typeof(resxDbFields))]
        public int SlotClosed { get; set; }

        [Display(Name = "PreventAppointments", ResourceType = typeof(resxDbFields))]
        public bool PreventAppointments { get; set; }

        [Display(Name = "PublicHolday", ResourceType = typeof(resxDbFields))]
        public bool PublicHolday { get; set; }

        [StringLength(250, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Comment", ResourceType = typeof(resxDbFields))]
        public string Comment { get; set; }

        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> OrganizationInstanceGUID { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DutyStationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }
        public byte[] dataAppointmentTypeCalendarRowVersion { get; set; }
    }

    public class CalendarEvents
    {
        public Guid EventId { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public string EventDescription { get; set; }
        public string Title { get; set; }
        public bool AllDayEvent { get; set; }
        public bool PreventAppointments { get; set; }
        public bool PublicHolday { get; set; }
        public bool Cancelled { get; set; }
        public bool Arrived { get; set; }
    }
}
