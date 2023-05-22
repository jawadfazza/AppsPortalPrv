using Portal_BL.Library;
using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSA.Model
{
    public class dataOfficeStaffAttendanceConfirmationsDataTableModel
    {
        public System.Guid OfficeStaffAttendanceConfirmationGUID { get; set; }
        public System.Guid UserGUID { get; set; }
        public System.Guid ReportToGUID { get; set; }
        public string ConfirmedBy { get; set; }
        public System.DateTime ConfirmedDate { get; set; }
        public int ConfirmedYear { get; set; }
        public int ConfirmedMonth { get; set; }
        public string MonthYear { get; set; }
        public Nullable<System.DateTime> PaymentConfirmedDate { get; set; }
        public string PaymentConfirmedBy { get; set; }
        public int PaymentAmount { get; set; }
        public bool Active { get; set; }
        public byte[] dataOfficeStaffAttendanceConfirmationRowVersion { get; set; }

    }
    public class StaffImportModel
    {
        public Guid UserGUID { get; set; }
        public int EmployeeID { get; set; }
        public string FullName { get; set; }
        public DateTime DateKey { get; set; }
        public string WorkingTime { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public DateTime ConfirmedDate { get; set; }
        public string ConfirmedBy { get; set; }
    }
    public class OfficeStaffAttendanceUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OfficeStaffAttendanceGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid OfficeStaffAttendanceGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid UserGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.Date)]
        [Display(Name = "AttendanceFromDatetime", ResourceType = typeof(resxDbFields))]
        public System.DateTime? AttendanceFromDatetime { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "AttendanceToDatetime", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> AttendanceToDatetime { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "IsAttend", ResourceType = typeof(resxDbFields))]
        public bool IsAttend { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "IsConfirmed", ResourceType = typeof(resxDbFields))]
        public bool IsConfirmed { get; set; }

        [Display(Name = "ShuttleDepartureMorningTime", ResourceType = typeof(resxDbFields))]
        public Nullable<int> ShuttleDepartureMorningTime { get; set; }

        [Display(Name = "ShuttleDepartureEveningTime", ResourceType = typeof(resxDbFields))]
        public Nullable<int> ShuttleDepartureEveningTime { get; set; }

        [Display(Name = "OfficeGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> OfficeGUID { get; set; }

        [Display(Name = "OfficeFloorGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> OfficeFloorGUID { get; set; }

        [Display(Name = "OfficeFloorRoomGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> OfficeFloorRoomGUID { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DutyStationGUID { get; set; }

        [Display(Name = "PaymentConfirmedDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> PaymentConfirmedDate { get; set; }

        [Display(Name = "PaymentConfirmedBy", ResourceType = typeof(resxDbFields))]
        public string PaymentConfirmedBy { get; set; }

        [Display(Name = "EmployeeID", ResourceType = typeof(resxDbFields))]
        public string EmployeeID { get; set; }
        [Display(Name = "Month", ResourceType = typeof(resxDbFields))]
        public string Month { get; set; }
        [Display(Name = "Year", ResourceType = typeof(resxDbFields))]
        public string Year { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataOfficeStaffAttendanceRowVersion { get; set; }
    }

    public class CalendarEvents
    {
        public Guid EventId { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public string EventDescription { get; set; }
        public string Title { get; set; }
        public bool AllDayEvent { get; set; }

        /// <summary>
        /// /////////////////////////////////
        /// </summary>
        public Guid id { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string title { get; set; }
        public bool PublicHolday { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsAttend { get; set; }
        public Guid DutyStationGUID { get; set; }
        public Guid? DepartmentGUID { get; set; }
        public Guid? UserGUID { get; set; }
        public string color { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public bool allDay { get; set; }
    }
}
