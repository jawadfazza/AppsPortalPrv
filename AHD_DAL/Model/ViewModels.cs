using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace AHD_DAL.ViewModels
{
    //public class StaffRenwalResidencyDataTableModel
    //{
    //    [Display(Name = "StaffRenwalResidencyGUID", ResourceType = typeof(resxDbFields))]
    //    public Guid StaffRenwalResidencyGUID { get; set; }
    //    [Display(Name = "RefNumber", ResourceType = typeof(resxDbFields))]
    //    public string RefNumber { get; set; }
    //    [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
    //    public string StaffName { get; set; }
    //    [Display(Name = "PassportNumber", ResourceType = typeof(resxDbFields))]
    //    public string PassportNumber { get; set; }
    //    [Display(Name = "PassportValidityDate", ResourceType = typeof(resxDbFields))]
    //    public DateTime? PassportValidityDate { get; set; }

    //    public bool Active { get; set; }

    //    public byte[] dataStaffRenwalResidencyRowVersion { get; set; }
    //}
    public class CalendarEventStaffAbsences
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
        public Guid UserGUID { get; set; }
        public string color { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public bool allDay { get; set; }
    }

    public class StaffAbsencesDataTableModel
    {
        public System.Guid dataStaffAbsenceGuid { get; set; }

        public Nullable<System.Guid> CurrentUserGUID { get; set; }
        public string UserGUID { get; set; }
        public string StaffName { get; set; }
        public string AbsenceTypeGuid { get; set; }
        public string AbsenceType { get; set; }
        public System.DateTime? AbsenceFrom { get; set; }
        public System.DateTime? AbsenceTo { get; set; }
        public string DirectSupervisor { get; set; }
        public string SupervisorComfirmed { get; set; }
        public Nullable<System.DateTime> SupervisorComfirmedDate { get; set; }
        public Nullable<System.Guid> DutyStationGUID { get; set; }
        public double AbsenceDuration { get; set; }
        public bool Active { get; set; }
        public byte[] dataStaffAbsenceRowVersion { get; set; }
    }
    public class StaffAbsenceUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "dataStaffAbsenceGuid", ResourceType = typeof(resxDbFields))]
        public System.Guid dataStaffAbsenceGuid { get; set; }

        public Nullable<System.Guid> CurrentUserGUID { get; set; }

        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> UserGUID { get; set; }

        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }

        [Display(Name = "AbsenceTypeGuid", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> AbsenceTypeGuid { get; set; }

        [Display(Name = "AbsenceType", ResourceType = typeof(resxDbFields))]
        public string AbsenceType { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "AbsenceDuration", ResourceType = typeof(resxDbFields))]
        public double AbsenceDuration { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.Date)]
        [Display(Name = "AbsenceFrom", ResourceType = typeof(resxDbFields))]
        public System.DateTime? AbsenceFrom { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.Date)]
        [Display(Name = "AbsenceTo", ResourceType = typeof(resxDbFields))]
        public System.DateTime? AbsenceTo { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DirectSupervisor", ResourceType = typeof(resxDbFields))]
        public string DirectSupervisor { get; set; }

       

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SupervisorComfirmed", ResourceType = typeof(resxDbFields))]
        public bool SupervisorComfirmed { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "SupervisorComfirmedDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> SupervisorComfirmedDate { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DutyStationGUID { get; set; }

        public Nullable<double> AbsenceDays { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }
        public byte[] dataStaffAbsenceRowVersion { get; set; }

    }

    public class GeneralAttendanceReport
    {


        [Display(Name = "FromDate", ResourceType = typeof(resxDbFields))]
        public DateTime? FromDate { get; set; }

        [Display(Name = "ToDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ToDate { get; set; }


        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public List<Guid?> StaffGuids { get; set; }
    }
    public class NationalStaffDangerPayEditDetailDataTableModel
    {

        [Display(Name = "ConfirmationStatusGUID", ResourceType = typeof(resxDbFields))]
        public string ConfirmationStatusGUID { get; set; }
        [Display(Name = "DangerPrivisous", ResourceType = typeof(resxDbFields))]
        public string DangerPrivisous { get; set; }
        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Display(Name = "NationalStaffDangerPayGUID", ResourceType = typeof(resxDbFields))]
        public Guid NationalStaffDangerPayGUID { get; set; }
        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> StaffGUID { get; set; }
        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }
        [Display(Name = "DangerPayInformationGUID", ResourceType = typeof(resxDbFields))]
        public Guid DangerPayInformationGUID { get; set; }
        [Display(Name = "ResponseDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ResponseDate { get; set; }
        [Display(Name = "ActualDangerPayAmount", ResourceType = typeof(resxDbFields))]
        public decimal? ActualDangerPayAmount { get; set; }
        [Display(Name = "TotalDaysNotPayable", ResourceType = typeof(resxDbFields))]
        public int? TotalDaysNotPayable { get; set; }
        [Display(Name = "TotalDaysPayable", ResourceType = typeof(resxDbFields))]
        public int? TotalDaysPayable { get; set; }


        [Display(Name = "IsPayed", ResourceType = typeof(resxDbFields))]
        public bool IsPayed { get; set; }

        [Display(Name = "IsAnswered", ResourceType = typeof(resxDbFields))]
        public bool IsAnswered { get; set; }


        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string UserGUID { get; set; }

        [Display(Name = "EntitlementStatus", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> FlowStatusGUID { get; set; }

        [Display(Name = "DangerPaymentConfirmationStatus", ResourceType = typeof(resxDbFields))]
        public string DangerPaymentConfirmationStatus { get; set; }

        public decimal? DangerPayAmount { get; set; }



        public byte[] dataNationalStaffDangerPayRowVersion { get; set; }

        public bool Active { get; set; }
        [Display(Name = "PaymentDurationName", ResourceType = typeof(resxDbFields))]
        public string PaymentDurationName { get; set; }
    }
    public class StaffRenwalResidencyDataTableModel
    {
        [Display(Name = "EntitlementStatus", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusGUID { get; set; }

        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
        public string CreateByGUID { get; set; }

        [Display(Name = "FormType", ResourceType = typeof(resxDbFields))]
        public string FormTypeGUID { get; set; }

        [Display(Name = "FormType", ResourceType = typeof(resxDbFields))]
        public string FormType { get; set; }



        [Display(Name = "EntryDateToSyria", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? EntryDateToSyria { get; set; }


        [Display(Name = "StaffRenwalResidencyGUID", ResourceType = typeof(resxDbFields))]
        public Guid StaffRenwalResidencyGUID { get; set; }
        [Display(Name = "RefNumber", ResourceType = typeof(resxDbFields))]
        public string RefNumber { get; set; }
        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffGUID { get; set; }
        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }

        [Display(Name = "DateOfSubmission", ResourceType = typeof(resxDbFields))]
        public DateTime? DateOfSubmission { get; set; }

        [Display(Name = "PassportNumber", ResourceType = typeof(resxDbFields))]
        public string PassportNumber { get; set; }
        [Display(Name = "PassportValidityDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? PassportValidityDate { get; set; }

        [Display(Name = "CurrentStatus", ResourceType = typeof(resxDbFields))]
        public string RenewalFormFlowStatus { get; set; }
        [Display(Name = "ContractStartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ContractStartDate { get; set; }
        [Display(Name = "SALContractEndDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? SALContractEndDate { get; set; }
        [Display(Name = "ResidencyExpiryDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ResidencyExpiryDate { get; set; }
        [Display(Name = "CurrentResidencyEndDateStampd", ResourceType = typeof(resxDbFields))]
        public DateTime? CurrentResidencyEndDateStampd { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]

        [Display(Name = "NextExtensionRequest", ResourceType = typeof(resxDbFields))]
        public DateTime? NextExtensionRequest { get; set; }
        public string CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        [Display(Name = "SentDateToMOFA", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]

        public DateTime? SentDate { get; set; }
        [Display(Name = "ReturnDateToCountry", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ReturnDate { get; set; }
        public string LastFlowStatus { get; set; }

        public bool Active { get; set; }

        public byte[] dataStaffRenwalResidencyRowVersion { get; set; }
    }
    public class StaffRenwalResidencyModel
    {
        [Display(Name = "EntitlementStatus", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> LastFlowStatusGUID { get; set; }

        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> CreateByGUID { get; set; }

        [Display(Name = "FormType", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> FormTypeGUID { get; set; }

        [Display(Name = "FormType", ResourceType = typeof(resxDbFields))]
        public string FormType { get; set; }





        [Display(Name = "EntryDateToSyria", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? EntryDateToSyria { get; set; }

        [Display(Name = "DepartureDateFromSyria", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? DepartureDateFromSyria { get; set; }




        [Display(Name = "StaffRenwalResidencyGUID", ResourceType = typeof(resxDbFields))]
        public Guid StaffRenwalResidencyGUID { get; set; }
        [Display(Name = "RefNumber", ResourceType = typeof(resxDbFields))]
        public string RefNumber { get; set; }
        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public Guid StaffGUID { get; set; }
        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }
        [Display(Name = "PassportNumber", ResourceType = typeof(resxDbFields))]
        public string PassportNumber { get; set; }
        [Display(Name = "PassportValidityDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? PassportValidityDate { get; set; }

        [Display(Name = "RenewalFormFlowStatus", ResourceType = typeof(resxDbFields))]
        public string RenewalFormFlowStatus { get; set; }
        [Display(Name = "ContractStartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ContractStartDate { get; set; }
        [Display(Name = "ContractEndDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ContractEndDate { get; set; }
        [Display(Name = "CurrentResidencyEndDateSent", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CurrentResidencyEndDateSent { get; set; }
        [Display(Name = "CurrentResidencyEndDateStampd", ResourceType = typeof(resxDbFields))]
        public DateTime? CurrentResidencyEndDateStampd { get; set; }


        [Display(Name = "NextExtensionRequest", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? NextExtensionRequest { get; set; }

        public string CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        [Display(Name = "SentDateToMOFA", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]

        public DateTime? SentDate { get; set; }
        [Display(Name = "ReturnDateToCountry", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ReturnDate { get; set; }
        public string LastFlowStatus { get; set; }

        public bool Active { get; set; }

        public byte[] dataStaffRenwalResidencyRowVersion { get; set; }
    }

    public class DangerPayInformationDataTableModel
    {
        [Display(Name = "TotalStaffConfirm", ResourceType = typeof(resxDbFields))]
        public int? TotalStaffConfirm { get; set; }

        [Display(Name = "TotalStaffNotConfirm", ResourceType = typeof(resxDbFields))]
        public int? TotalStaffNotConfirm { get; set; }


        [Display(Name = "DangerPayInformationGUID", ResourceType = typeof(resxDbFields))]
        public Guid DangerPayInformationGUID { get; set; }
        [Display(Name = "YearId", ResourceType = typeof(resxDbFields))]
        public int? YearId { get; set; }
        [Display(Name = "MonthId", ResourceType = typeof(resxDbFields))]
        public int? MonthId { get; set; }
        public bool? IsLastAction { get; set; }
        public byte[] dataDangerPayInformationRowVersion { get; set; }
        public bool Active { get; set; }
        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string FlowStatus { get; set; }
        [Display(Name = "PaymentDurationName", ResourceType = typeof(resxDbFields))]
        public string PaymentDurationName { get; set; }

        [Display(Name = "DangerPayAmount", ResourceType = typeof(resxDbFields))]
        public decimal? DangerPayAmount { get; set; }

        [Display(Name = "LastAllowedConfirmationDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? LastAllowedConfirmationDate { get; set; }

        [Display(Name = "EntitlementStatus", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> FlowStatusGUID { get; set; }

        [Display(Name = "OrderId", ResourceType = typeof(resxDbFields))]
        public int? OrderId { get; set; }
        public DateTime? CreateDate { get; set; }

        [Display(Name = "TotalDaysInMonth", ResourceType = typeof(resxDbFields))]
        public int? TotalDaysInMonth { get; set; }
        public Nullable<Guid> CreatedByGUID { get; set; }


    }

    public class DangerPayInformationModel
    {
        [Display(Name = "MonthStartDate", ResourceType = typeof(resxDbFields))]
        public DateTime? MonthStartDate { get; set; }
        [Display(Name = "MonthEndDate", ResourceType = typeof(resxDbFields))]
        public DateTime? MonthEndDate { get; set; }

        [Display(Name = "PaymentDurationName", ResourceType = typeof(resxDbFields))]
        public string PaymentDurationName { get; set; }





        [Display(Name = "DangerPayInformationGUID", ResourceType = typeof(resxDbFields))]
        public Guid DangerPayInformationGUID { get; set; }
        [Display(Name = "YearId", ResourceType = typeof(resxDbFields))]
        public int? YearId { get; set; }
        [Display(Name = "MonthId", ResourceType = typeof(resxDbFields))]
        public int? MonthId { get; set; }
        public bool? IsLastAction { get; set; }
        public byte[] dataDangerPayInformationRowVersion { get; set; }
        public bool Active { get; set; }

        [Display(Name = "DangerPayAmount", ResourceType = typeof(resxDbFields))]
        public decimal? DangerPayAmount { get; set; }

        [Display(Name = "LastAllowedConfirmationDate", ResourceType = typeof(resxDbFields))]
        public DateTime? LastAllowedConfirmationDate { get; set; }

        [Display(Name = "EntitlementStatus", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> FlowStatusGUID { get; set; }

        [Display(Name = "OrderId", ResourceType = typeof(resxDbFields))]
        public int? OrderId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Nullable<Guid> CreatedByGUID { get; set; }

        [Display(Name = "DangerPaymentConfirmationStatus", ResourceType = typeof(resxDbFields))]
        public string DangerPaymentConfirmationStatus { get; set; }
    }


    public class NationalStaffDangerPayDataTableModel
    {

        [Display(Name = "ConfirmationStatusGUID", ResourceType = typeof(resxDbFields))]
        public string ConfirmationStatusGUID { get; set; }
        [Display(Name = "DangerPrivisous", ResourceType = typeof(resxDbFields))]
        public string DangerPrivisous { get; set; }

        [Display(Name = "NationalStaffDangerPayGUID", ResourceType = typeof(resxDbFields))]
        public Guid NationalStaffDangerPayGUID { get; set; }
        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> StaffGUID { get; set; }
        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }
        [Display(Name = "DangerPayInformationGUID", ResourceType = typeof(resxDbFields))]
        public Guid DangerPayInformationGUID { get; set; }
        [Display(Name = "ResponseDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ResponseDate { get; set; }
        [Display(Name = "ActualDangerPayAmount", ResourceType = typeof(resxDbFields))]
        public decimal? ActualDangerPayAmount { get; set; }
        [Display(Name = "TotalDaysNotPayable", ResourceType = typeof(resxDbFields))]
        public int? TotalDaysNotPayable { get; set; }
        [Display(Name = "TotalDaysPayable", ResourceType = typeof(resxDbFields))]
        public int? TotalDaysPayable { get; set; }

        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string UserGUID { get; set; }

        [Display(Name = "EntitlementStatus", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> FlowStatusGUID { get; set; }

        [Display(Name = "DangerPaymentConfirmationStatus", ResourceType = typeof(resxDbFields))]
        public string DangerPaymentConfirmationStatus { get; set; }

        public decimal? DangerPayAmount { get; set; }



        public byte[] dataNationalStaffDangerPayRowVersion { get; set; }

        public bool Active { get; set; }
        [Display(Name = "PaymentDurationName", ResourceType = typeof(resxDbFields))]
        public string PaymentDurationName { get; set; }
        [Display(Name = "PayedIn", ResourceType = typeof(resxDbFields))]
        public string PayedIn { get; set; }
    }

    public class NationalStaffDangerPayDetailDataTableModel
    {
        public string Comments { get; set; }
        [Display(Name = "NationalStaffDangerPayDetailGUID", ResourceType = typeof(resxDbFields))]
        public Guid NationalStaffDangerPayDetailGUID { get; set; }

        [Display(Name = "NationalStaffDangerPayGUID", ResourceType = typeof(resxDbFields))]
        public Guid NationalStaffDangerPayGUID { get; set; }
        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> StaffGUID { get; set; }
        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }
        [Display(Name = "DepartureDate", ResourceType = typeof(resxDbFields))]
        public DateTime? DepartureDate { get; set; }

        [Display(Name = "ReturnDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ReturnDate { get; set; }

        [Display(Name = "ActualReturnDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ActualReturnDate { get; set; }


        [Display(Name = "ReturnToDutyStation", ResourceType = typeof(resxDbFields))]
        public string ReturnToDutyStation { get; set; }

        [Display(Name = "Country", ResourceType = typeof(resxDbFields))]
        public string Country { get; set; }

        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public string CountryGUID { get; set; }
        [Display(Name = "DestinationName", ResourceType = typeof(resxDbFields))]
        public string DestinationName { get; set; }

        [Display(Name = "LeaveType", ResourceType = typeof(resxDbFields))]
        public string LeaveType { get; set; }


        [Display(Name = "LeaveTypeGUID", ResourceType = typeof(resxDbFields))]
        public string LeaveTypeGUID { get; set; }



        public byte[] dataNationalStaffDangerPayDetailRowVersion { get; set; }

        public bool Active { get; set; }

    }

    public class NationalStaffDangerPayDetailModel
    {
        public string Comments { get; set; }
        public bool? hasPriMission { get; set; }
        public bool? hasMission { get; set; }
        public int? orderId { get; set; }

        [Display(Name = "NationalStaffDangerPayDetailGUID", ResourceType = typeof(resxDbFields))]
        public Guid NationalStaffDangerPayDetailGUID { get; set; }

        [Display(Name = "NationalStaffDangerPayGUID", ResourceType = typeof(resxDbFields))]
        public Guid NationalStaffDangerPayGUID { get; set; }
        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> StaffGUID { get; set; }
        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }
        [Display(Name = "DepartureDate", ResourceType = typeof(resxDbFields))]
        public DateTime? DepartureDate { get; set; }

        [Display(Name = "ReturnDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ReturnDate { get; set; }

        [Display(Name = "Country", ResourceType = typeof(resxDbFields))]
        public string Country { get; set; }

        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> CountryGUID { get; set; }
        [Display(Name = "DestinationName", ResourceType = typeof(resxDbFields))]
        public string DestinationName { get; set; }

        [Display(Name = "LeaveTypeGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> LeaveTypeGUID { get; set; }

        [Display(Name = "ReturnToDutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> ReturnToDutyStationGUID { get; set; }
        public byte[] dataNationalStaffDangerPayDetailRowVersion { get; set; }

        public bool Active { get; set; }

    }

    public class TrackStaffDangerPaymentsDataTableModel
    {

        [Display(Name = "ConfirmationStatusGUID", ResourceType = typeof(resxDbFields))]
        public string ConfirmationStatusGUID { get; set; }
        [Display(Name = "DangerPrivisous", ResourceType = typeof(resxDbFields))]
        public string DangerPrivisous { get; set; }

        [Display(Name = "NationalStaffDangerPayGUID", ResourceType = typeof(resxDbFields))]
        public Guid NationalStaffDangerPayGUID { get; set; }
        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> StaffGUID { get; set; }
        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }
        [Display(Name = "DangerPayInformationGUID", ResourceType = typeof(resxDbFields))]
        public Guid DangerPayInformationGUID { get; set; }
        [Display(Name = "ResponseDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ResponseDate { get; set; }
        [Display(Name = "ActualDangerPayAmount", ResourceType = typeof(resxDbFields))]
        public decimal? ActualDangerPayAmount { get; set; }
        [Display(Name = "TotalDaysNotPayable", ResourceType = typeof(resxDbFields))]
        public int? TotalDaysNotPayable { get; set; }
        [Display(Name = "TotalDaysPayable", ResourceType = typeof(resxDbFields))]
        public int? TotalDaysPayable { get; set; }

        [Display(Name = "EntitlementStatus", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> FlowStatusGUID { get; set; }

        [Display(Name = "DangerPaymentConfirmationStatus", ResourceType = typeof(resxDbFields))]
        public string DangerPaymentConfirmationStatus { get; set; }

        public decimal? DangerPayAmount { get; set; }


        [Display(Name = "YearId", ResourceType = typeof(resxDbFields))]
        public int? YearId { get; set; }
        [Display(Name = "MonthId", ResourceType = typeof(resxDbFields))]
        public int? MonthId { get; set; }
        public bool? IsLastAction { get; set; }
        public byte[] dataDangerPayInformationRowVersion { get; set; }
        public bool Active { get; set; }
        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string FlowStatus { get; set; }
        [Display(Name = "PaymentDurationName", ResourceType = typeof(resxDbFields))]
        public string PaymentDurationName { get; set; }



        [Display(Name = "LastAllowedConfirmationDate", ResourceType = typeof(resxDbFields))]
        public DateTime? LastAllowedConfirmationDate { get; set; }



        [Display(Name = "OrderId", ResourceType = typeof(resxDbFields))]
        public int? OrderId { get; set; }
        public DateTime? CreateDate { get; set; }
        [Display(Name = "TotalDaysInMonth", ResourceType = typeof(resxDbFields))]
        public int? TotalDaysInMonth { get; set; }
        public Nullable<Guid> CreatedByGUID { get; set; }


        [Display(Name = "DangerPayStatus", ResourceType = typeof(resxDbFields))]
        public string DangerPayStatus { get; set; }
        public byte[] dataNationalStaffDangerPayRowVersion { get; set; }



    }
    #region Maintance Request
    public class VehicleMaintenanceRequestDataTableModel
    {
        [Display(Name = "VehicleMaintenanceRequestGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> VehicleMaintenanceRequestGUID { get; set; }

        [Display(Name = "VehicleGUID", ResourceType = typeof(resxDbFields))]
        public string VehicleGUID { get; set; }


        [Display(Name = "VehicleRequestNumber", ResourceType = typeof(resxDbFields))]
        public string RequestNumber { get; set; }

        [Display(Name = "VehicleNumber", ResourceType = typeof(resxDbFields))]
        public string VehicleNumber { get; set; }


        [Display(Name = "RequestYear", ResourceType = typeof(resxDbFields))]
        public string RequestYear { get; set; }

        [Display(Name = "RequestDate", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? RequestDate { get; set; }

        [Display(Name = "LastRenewalDate", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? LastRenewalDate { get; set; }


        [Display(Name = "ConfirmationRenewalDate", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? ConfirmationRenewalDate { get; set; }

        [Display(Name = "EntitlementStatus", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusGUID { get; set; }


        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Display(Name = "LastFlowStatus", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatus { get; set; }
        [Display(Name = "PlateNumber", ResourceType = typeof(resxDbFields))]
        public int? PlateNumber { get; set; }

        [Display(Name = "ChassisNumber", ResourceType = typeof(resxDbFields))]
        public string ChassisNumber { get; set; }

        [Display(Name = "EngineNumber", ResourceType = typeof(resxDbFields))]
        public string EngineNumber { get; set; }

        [Display(Name = "VechileModel", ResourceType = typeof(resxDbFields))]
        public string VechileModel { get; set; }

        [Display(Name = "VehicleColor", ResourceType = typeof(resxDbFields))]
        public string VehicleColor { get; set; }
        [Display(Name = "VehicleTypeGUID", ResourceType = typeof(resxDbFields))]
        public string VehicleTypeGUID { get; set; }
        [Display(Name = "VechileModelGUID", ResourceType = typeof(resxDbFields))]
        public string VechileModelGUID { get; set; }
        [Display(Name = "VehileColorGUID", ResourceType = typeof(resxDbFields))]
        public string VehileColorGUID { get; set; }


        public byte[] dataVehicleMaintenanceRequestRowVersion { get; set; }
        public bool Active { get; set; }



    }

    public class VehicleMaintenanceRequesModel
    {
        [Display(Name = "VehicleMaintenanceRequestGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> VehicleMaintenanceRequestGUID { get; set; }

        [Display(Name = "VehicleGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> VehicleGUID { get; set; }
        [Display(Name = "VehicleNumber", ResourceType = typeof(resxDbFields))]
        public string VehicleNumber { get; set; }

        [Display(Name = "VehicleRequestNumber", ResourceType = typeof(resxDbFields))]
        public string RequestNumber { get; set; }


        [Display(Name = "RequestYear", ResourceType = typeof(resxDbFields))]
        public string RequestYear { get; set; }

        [Display(Name = "RequestDate", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? RequestDate { get; set; }

        [Display(Name = "LastRenewalDate", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? LastRenewalDate { get; set; }


        [Display(Name = "ConfirmationRenewalDate", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? ConfirmationRenewalDate { get; set; }

        [Display(Name = "EntitlementStatus", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> LastFlowStatusGUID { get; set; }


        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Display(Name = "LastFlowStatus", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatus { get; set; }
        [Display(Name = "PlateNumber", ResourceType = typeof(resxDbFields))]
        public int? PlateNumber { get; set; }

        [Display(Name = "ChassisNumber", ResourceType = typeof(resxDbFields))]
        public string ChassisNumber { get; set; }

        [Display(Name = "EngineNumber", ResourceType = typeof(resxDbFields))]
        public string EngineNumber { get; set; }

        [Display(Name = "VechileModel", ResourceType = typeof(resxDbFields))]
        public string VechileModel { get; set; }


        public byte[] dataVehicleMaintenanceRequestRowVersion { get; set; }
        public bool Active { get; set; }
    }
    #endregion
    #region InternationalNational Staff Leave
    public class StaffRAndRLeaveRequestDataTableModel
    {
        [Display(Name = "RestAndRecuperationLeaveGUID", ResourceType = typeof(resxDbFields))]
        public string RestAndRecuperationLeaveGUID { get; set; }

        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public string StaffGUID { get; set; }
        [Display(Name = "SubmittedDate", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? SubmittedDate { get; set; }

        [Display(Name = "EntitlementStatus", ResourceType = typeof(resxDbFields))]
        public string FlowStatusGUID { get; set; }
        [Display(Name = "RequestStatusGUID", ResourceType = typeof(resxDbFields))]
        public string RequestStatusGUID { get; set; }
        [Display(Name = "RequestStatus", ResourceType = typeof(resxDbFields))]
        public string RequestStatus { get; set; }
        [Display(Name = "LeaveNumber", ResourceType = typeof(resxDbFields))]
        public string LeaveNumber { get; set; }

        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }

        [Display(Name = "StaffJobTitle", ResourceType = typeof(resxDbFields))]
        public string StaffJobTitle { get; set; }
        [Display(Name = "StaffLevel", ResourceType = typeof(resxDbFields))]
        public string StaffLevel { get; set; }

        [Display(Name = "StaffLoction", ResourceType = typeof(resxDbFields))]
        public string StaffLoction { get; set; }

        [Display(Name = "BackupArrangementGUID", ResourceType = typeof(resxDbFields))]
        public string BackupArrangementGUID { get; set; }

        [Display(Name = "BackupArrangementName", ResourceType = typeof(resxDbFields))]
        public string BackupArrangementName { get; set; }


        [Display(Name = "ReturnDateFromLastRLeave", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ReturnDateFromLastRLeave { get; set; }


        [Display(Name = "ExpiryDateOfResidency", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ExpiryDateOfResidency { get; set; }

        [Display(Name = "TypeDateOfLastTravelInterrupting", ResourceType = typeof(resxDbFields))]
        public string TypeDateOfLastTravelInterrupting { get; set; }


        [Display(Name = "LeaveInDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? LeaveInDate { get; set; }

        [Display(Name = "LeaveOutDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? LeaveOutDate { get; set; }

        [Display(Name = "ActualLeaveInDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ActualLeaveInDate { get; set; }


        [Display(Name = "ActualLeaveOutDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ActualLeaveOutDate { get; set; }


        [Display(Name = "HRComments", ResourceType = typeof(resxDbFields))]
        public string HRComments { get; set; }


        [Display(Name = "EmployeeComments", ResourceType = typeof(resxDbFields))]
        public string EmployeeComments { get; set; }
        public byte[] dataRestAndRecuperationRequestRowVersion { get; set; }
        public bool Active { get; set; }
    }

    public class StaffRAndRLeaveRequestModel
    {

        [Display(Name = "SupervisorGUID", ResourceType = typeof(resxDbFields))]
        public Guid SupervisorToChangeGUID { get; set; }
        [Display(Name = "RestAndRecuperationLeaveGUID", ResourceType = typeof(resxDbFields))]
        public Guid RestAndRecuperationLeaveGUID { get; set; }

        [Display(Name = "TravelPlanStatusGUID", ResourceType = typeof(resxDbFields))]
        public Guid? TravelPlanStatusGUID { get; set; }

        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public Guid CreatedByGUID { get; set; }
        [Display(Name = "EntitlementStatus", ResourceType = typeof(resxDbFields))]
        public Guid? FlowStatusGUID { get; set; }

        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        [Required]
        public Guid CountryGUID { get; set; }

        [Display(Name = "CountryName", ResourceType = typeof(resxDbFields))]
        public string CountryName { get; set; }

        public int? RejectAccess { get; set; }
        [Display(Name = "Location", ResourceType = typeof(resxDbFields))]
        public string TravelLocaionDescription { get; set; }
        [Display(Name = "ToVerifyBy", ResourceType = typeof(resxDbFields))]
        public string ToVerifyBy { get; set; }


        [Display(Name = "ToCertifyBy", ResourceType = typeof(resxDbFields))]
        public string ToCertifyBy { get; set; }

        public Guid? DeparturePointGUID { get; set; }
        public Guid? DropOffPointGUID { get; set; }
        public Guid? StartLocationGUID { get; set; }
        public Guid? EndLocationGUID { get; set; }
        public string DeparturePoint { get; set; }
        public string DropOffPoint { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DestCountryGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> CountryGUID1 { get; set; }

        public string LastFlowStatusName { get; set; }

        public string RRLeaveDates { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? SubmittedDate { get; set; }

        public string AnualLeaveDates { get; set; }

        public string LeaveInDateStringFormat { get; set; }
        public string LeaveOutDateStringFormat { get; set; }
        public string SubmittedDateFormat { get; set; }

        public string HRReviewDateFormat { get; set; }
        public string SupervisorApprovedDateFormat { get; set; }


        [Display(Name = "SupervisorGUID", ResourceType = typeof(resxDbFields))]
        [Required]

        public Guid SupervisorGUID { get; set; }

        [Display(Name = "ApprovedByHRStaffGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> ApprovedByHRStaffGUID { get; set; }

        [Display(Name = "ReviewedByHRStaffGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> ReviewedByHRStaffGUID { get; set; }



        [Display(Name = "RepresentativeComments", ResourceType = typeof(resxDbFields))]
        public string RepresentativeComments { get; set; }


        [Display(Name = "SupervisorComments", ResourceType = typeof(resxDbFields))]
        public string SupervisorComments { get; set; }

        [Display(Name = "AccessLevel", ResourceType = typeof(resxDbFields))]
        public int? AccessLevel { get; set; }


        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public string StaffGUID { get; set; }
        [Display(Name = "DestinationCountry", ResourceType = typeof(resxDbFields))]
        public string DestinationCountry { get; set; }
        [Display(Name = "EligibleDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? EligibleDate { get; set; }
        [Display(Name = "PT8Number", ResourceType = typeof(resxDbFields))]
        public string PT8Number { get; set; }

        [Display(Name = "RequestStatusGUID", ResourceType = typeof(resxDbFields))]
        public string RequestStatusGUID { get; set; }
        [Display(Name = "RequestStatus", ResourceType = typeof(resxDbFields))]
        public string RequestStatus { get; set; }
        [Display(Name = "LeaveNumber", ResourceType = typeof(resxDbFields))]
        public string LeaveNumber { get; set; }

        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }

        [Display(Name = "CurrentStep", ResourceType = typeof(resxDbFields))]
        public int? CurrentStep { get; set; }

        [Display(Name = "StaffJobTitle", ResourceType = typeof(resxDbFields))]
        public string StaffJobTitle { get; set; }
        [Display(Name = "StaffLevel", ResourceType = typeof(resxDbFields))]
        public string StaffLevel { get; set; }

        [Display(Name = "StaffLoction", ResourceType = typeof(resxDbFields))]
        public string StaffLoction { get; set; }

        [Display(Name = "BackupArrangementGUID", ResourceType = typeof(resxDbFields))]
        [Required]

        public Guid BackupArrangementGUID { get; set; }

        [Display(Name = "BackupArrangementName", ResourceType = typeof(resxDbFields))]
        public string BackupArrangementName { get; set; }


        [Display(Name = "ReturnDateFromLastRLeave", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ReturnDateFromLastRLeave { get; set; }


        [Display(Name = "ExpiryDateOfResidency", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ExpiryDateOfResidency { get; set; }

        [Display(Name = "TypeDateOfLastTravelInterrupting", ResourceType = typeof(resxDbFields))]
        public string TypeDateOfLastTravelInterrupting { get; set; }


        [Display(Name = "LeaveInDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? LeaveInDate { get; set; }

        [Display(Name = "LeaveOutDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? LeaveOutDate { get; set; }

        [Display(Name = "ApprovedByName", ResourceType = typeof(resxDbFields))]

        public string ApprovedByName { get; set; }

        [Display(Name = "SupervisorApprovedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? SupervisorApprovedDate { get; set; }

        [Display(Name = "HRReviewDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? HRReviewDate { get; set; }

        [Display(Name = "ReviewedByHRStaffName", ResourceType = typeof(resxDbFields))]

        public string ReviewedByHRStaffName { get; set; }
        [Display(Name = "SupervisorName", ResourceType = typeof(resxDbFields))]
        public string SupervisorName { get; set; }



        [Display(Name = "RepresentativeDecisionDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? RepresentativeDecisionDate { get; set; }


        [Display(Name = "ActualLeaveInDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ActualLeaveInDate { get; set; }


        [Display(Name = "ActualLeaveOutDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ActualLeaveOutDate { get; set; }


        [Display(Name = "HRComments", ResourceType = typeof(resxDbFields))]
        public string HRComments { get; set; }


        [Display(Name = "EmployeeComments", ResourceType = typeof(resxDbFields))]
        public string EmployeeComments { get; set; }
        [Display(Name = "Supervisor", ResourceType = typeof(resxDbFields))]
        public Guid MySupervisorGUID { get; set; }

        [Display(Name = "Representative", ResourceType = typeof(resxDbFields))]
        public Guid? MyRepresentativeGUID { get; set; }

        [Display(Name = "ApprovedByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ApprovedByGUID { get; set; }
        public byte[] dataRestAndRecuperationRequestRowVersion { get; set; }
        public bool Active { get; set; }
        public int? IsSameUser { get; set; }
    }


    public class TempRestAndRecuperationRequestLeaveDateDataTableModel
    {
        [Display(Name = "TempRestAndRecuperationRequestLeaveDateGUID", ResourceType = typeof(resxDbFields))]
        public string TempRestAndRecuperationRequestLeaveDateGUID { get; set; }

        [Display(Name = "RestAndRecuperationLeaveGUID", ResourceType = typeof(resxDbFields))]
        public string RestAndRecuperationLeaveGUID { get; set; }

        [Display(Name = "LeaveTypeGUID", ResourceType = typeof(resxDbFields))]
        public string LeaveTypeGUID { get; set; }
        [Display(Name = "LeaveTypeName", ResourceType = typeof(resxDbFields))]
        public string LeaveTypeName { get; set; }
        [Display(Name = "TravelTimeOut", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? TravelTimeOut { get; set; }

        [Display(Name = "TravelTimeIn", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? TravelTimeIn { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }




        public byte[] dataTempRestAndRecuperationRequestLeaveDateRowVersion { get; set; }
        public bool Active { get; set; }
    }

    public class TempRestAndRecuperationRequestLeaveDateModel
    {
        [Display(Name = "TempRestAndRecuperationRequestLeaveDateGUID", ResourceType = typeof(resxDbFields))]
        public Guid TempRestAndRecuperationRequestLeaveDateGUID { get; set; }

        [Display(Name = "RestAndRecuperationLeaveGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> RestAndRecuperationLeaveGUID { get; set; }

        [Display(Name = "LeaveTypeGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> LeaveTypeGUID { get; set; }
        [Display(Name = "LeaveTypeName", ResourceType = typeof(resxDbFields))]
        public string LeaveTypeName { get; set; }
        [Display(Name = "TravelTimeOut", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? TravelTimeOut { get; set; }

        [Display(Name = "TravelTimeIn", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? TravelTimeIn { get; set; }


        [Display(Name = "TravelStartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? TravelStartDate { get; set; }


        [Display(Name = "TravelReturnDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? TravelReturnDate { get; set; }


        [Display(Name = "RRStartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? RRStartDate { get; set; }


        [Display(Name = "RRReturnDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? RRReturnDate { get; set; }


        [Display(Name = "ALReturnDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ALReturnDate { get; set; }

        [Display(Name = "ALStartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ALStartDate { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }




        public byte[] dataTempRestAndRecuperationRequestLeaveDateRowVersion { get; set; }
        public bool Active { get; set; }
    }
    #endregion
    #region International staff Attendacne 
    public class InternationalStaffPresenceAttendanceDataTableModel
    {
        [Display(Name = "InternationalStaffAttendanceGUID", ResourceType = typeof(resxDbFields))]
        public string InternationalStaffAttendanceGUID { get; set; }

        [Display(Name = "InternationalStaffAttendanceTypeGUID", ResourceType = typeof(resxDbFields))]
        public string InternationalStaffAttendanceTypeGUID { get; set; }
        [Display(Name = "TotalDays", ResourceType = typeof(resxDbFields))]
        public int? TotalDays { get; set; }

        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public string CountryGUID { get; set; }

        [Display(Name = "LeaveType", ResourceType = typeof(resxDbFields))]
        public string LeaveType { get; set; }


        [Display(Name = "City", ResourceType = typeof(resxDbFields))]
        public string CityGUID { get; set; }
        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public string StaffGUID { get; set; }
        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }
        [Display(Name = "PositionName", ResourceType = typeof(resxDbFields))]
        public string PositionName { get; set; }
        [Display(Name = "LeaveLocation", ResourceType = typeof(resxDbFields))]
        public string LeaveLocation { get; set; }
        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }
        [Display(Name = "FromDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? FromDate { get; set; }

        [Display(Name = "ToDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ToDate { get; set; }
        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> CreatedByGUID { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreateDate { get; set; }

        public byte[] dataInternationalStaffAttendanceRowVersion { get; set; }
        public bool Active { get; set; }
    }

    public class InternationalStaffPresenceAttendanceUpdateModel
    {
        [Display(Name = "InternationalStaffAttendanceGUID", ResourceType = typeof(resxDbFields))]
        public Guid InternationalStaffAttendanceGUID { get; set; }

        [Display(Name = "InternationalStaffAttendanceTypeGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> InternationalStaffAttendanceTypeGUID { get; set; }
        [Display(Name = "TotalDays", ResourceType = typeof(resxDbFields))]
        public int? TotalDays { get; set; }

        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> CountryGUID { get; set; }


        [Display(Name = "City", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> CityGUID { get; set; }
        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> StaffGUID { get; set; }
        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }
        [Display(Name = "PositionName", ResourceType = typeof(resxDbFields))]
        public string PositionName { get; set; }
        [Display(Name = "LeaveLocation", ResourceType = typeof(resxDbFields))]
        public string LeaveLocation { get; set; }
        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }
        [Display(Name = "FromDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? FromDate { get; set; }

        [Display(Name = "ToDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ToDate { get; set; }
        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> CreatedByGUID { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreateDate { get; set; }

        public byte[] dataInternationalStaffAttendanceRowVersion { get; set; }
        public bool Active { get; set; }
    }

    #region Period


    public class InternationalStaffEntitlementPeriodDataTableModel
    {
        [Display(Name = "PeriodEntitlementGUID", ResourceType = typeof(resxDbFields))]
        public string PeriodEntitlementGUID { get; set; }

        [Display(Name = "YearNumber", ResourceType = typeof(resxDbFields))]
        public int? YearNumber { get; set; }


        [Display(Name = "PeriodName", ResourceType = typeof(resxDbFields))]
        public string PeriodName { get; set; }

        [Display(Name = "MonthName", ResourceType = typeof(resxDbFields))]
        public string MonthName { get; set; }
        [Display(Name = "StartMonth", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? StartMonth { get; set; }
        [Display(Name = "EndMonth", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? EndMonth { get; set; }
        [Display(Name = "TotalStaff", ResourceType = typeof(resxDbFields))]
        public int? TotalStaff { get; set; }
        [Display(Name = "TotalStaffConfimed", ResourceType = typeof(resxDbFields))]
        public int? TotalStaffConfimed { get; set; }
        [Display(Name = "TotalStaffPending", ResourceType = typeof(resxDbFields))]
        public int? TotalStaffPending { get; set; }

        [Display(Name = "EntitlementStatus", ResourceType = typeof(resxDbFields))]
        public Guid? FlowStatusGUID { get; set; }

        [Display(Name = "FlowStatus", ResourceType = typeof(resxDbFields))]
        public string FlowStatus { get; set; }

        public byte[] dataAHDPeriodEntitlementRowVersion { get; set; }
        public bool Active { get; set; }
    }

    public class InternationalStaffEntitlementPeriodUpdateModel
    {
        [Display(Name = "PeriodEntitlementGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> PeriodEntitlementGUID { get; set; }

        [Display(Name = "YearNumber", ResourceType = typeof(resxDbFields))]
        public int? YearNumber { get; set; }

        public int? OrderID { get; set; }
        [Display(Name = "PeriodName", ResourceType = typeof(resxDbFields))]
        public string PeriodName { get; set; }

        [Display(Name = "MonthName", ResourceType = typeof(resxDbFields))]
        public string MonthName { get; set; }
        [Display(Name = "StartMonth", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? StartMonth { get; set; }
        [Display(Name = "EndMonth", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? EndMonth { get; set; }
        [Display(Name = "TotalStaff", ResourceType = typeof(resxDbFields))]
        public int? TotalStaff { get; set; }
        [Display(Name = "TotalDaysInMonth", ResourceType = typeof(resxDbFields))]
        public int? TotalDaysInMonth { get; set; }
        [Display(Name = "TotalStaffConfimed", ResourceType = typeof(resxDbFields))]
        public int? TotalStaffConfimed { get; set; }
        [Display(Name = "TotalStaffPending", ResourceType = typeof(resxDbFields))]
        public int? TotalStaffPending { get; set; }

        [Display(Name = "EntitlementStatus", ResourceType = typeof(resxDbFields))]
        public Guid? FlowStatusGUID { get; set; }

        [Display(Name = "FlowStatus", ResourceType = typeof(resxDbFields))]
        public string FlowStatus { get; set; }

        public byte[] dataAHDPeriodEntitlementRowVersion { get; set; }
        public bool Active { get; set; }
    }
    public class InternationalStaffEntitlementDeleteStaffUpdateModel
    {
        [Display(Name = "PeriodEntitlementGUID", ResourceType = typeof(resxDbFields))]
        public Guid PeriodEntitlementGUID { get; set; }

        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public Guid StaffGUID { get; set; }
    }

    public class InternationalStaffEntitlementDataTableModel
    {
        [Display(Name = "ConfirmReceipt", ResourceType = typeof(resxDbFields))]
        public string ConfirmReceipt { get; set; }

        [Display(Name = "FinanceComment", ResourceType = typeof(resxDbFields))]
        public string FinanceComment { get; set; }



        [Display(Name = "ReceiptDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ReceiptDate { get; set; }
        [Display(Name = "InternationalStaffEntitlementGUID", ResourceType = typeof(resxDbFields))]
        public string InternationalStaffEntitlementGUID { get; set; }

        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public string StaffGUID { get; set; }


        [Display(Name = "PeriodEntitlementGUID", ResourceType = typeof(resxDbFields))]
        public string PeriodEntitlementGUID { get; set; }

        [Display(Name = "EntitlementStatus", ResourceType = typeof(resxDbFields))]
        public string FlowStatusGUID { get; set; }
        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }

        [Display(Name = "DutyStation", ResourceType = typeof(resxDbFields))]
        public string DutyStation { get; set; }



        [Display(Name = "DutyStation", ResourceType = typeof(resxDbFields))]
        public string DutyStationGUID { get; set; }

        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
        public string Department { get; set; }

        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
        public string DepartmentGUID { get; set; }

        [Display(Name = "TransferTo", ResourceType = typeof(resxDbFields))]
        public string TransferTo { get; set; }

        [Display(Name = "TransferDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? TransferDate { get; set; }

        [Display(Name = "EntitlementStatus", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusName { get; set; }
        [Display(Name = "Month", ResourceType = typeof(resxDbFields))]
        public string Month { get; set; }

        public int? OrderId { get; set; }
        [Display(Name = "TotalEntitlements", ResourceType = typeof(resxDbFields))]
        public decimal? TotalEntitlements { get; set; }

        [Display(Name = "ReferenceNumber", ResourceType = typeof(resxDbFields))]
        public string ReferenceNumber { get; set; }


        [Display(Name = "IndexNumber", ResourceType = typeof(resxDbFields))]
        public string IndexNumber { get; set; }

        [Display(Name = "PaymentMethodGUID", ResourceType = typeof(resxDbFields))]
        public Guid? PaymentMethodGUID { get; set; }

        [Display(Name = "PaymentMethodGUID", ResourceType = typeof(resxDbFields))]
        public string PaymentMethod { get; set; }



        public byte[] dataAHDInternationalStaffEntitlementRowVersion { get; set; }
        public bool Active { get; set; }
    }


    public class InternationalStaffEntitlementUpdateModel
    {

        [Display(Name = "IsConfirmReceipt", ResourceType = typeof(resxDbFields))]
        public bool? IsConfirmReceipt { get; set; }


        [Display(Name = "ReceiptDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.DateTime)]

        public DateTime? ReceiptDate { get; set; }


        [Display(Name = "InternationalStaffEntitlementGUID", ResourceType = typeof(resxDbFields))]
        public Guid? InternationalStaffEntitlementGUID { get; set; }

        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> StaffGUID { get; set; }

        [Display(Name = "PeriodName", ResourceType = typeof(resxDbFields))]
        public string PeriodName { get; set; }
        [Display(Name = "PeriodEntitlementGUID", ResourceType = typeof(resxDbFields))]
        public Guid? PeriodEntitlementGUID { get; set; }

        [Display(Name = "ApprovedByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ApprovedByGUID { get; set; }

        [Display(Name = "TransferMethodGUID", ResourceType = typeof(resxDbFields))]
        public Guid? TransferMethodGUID { get; set; }

        [Display(Name = "TransferMethodGUID", ResourceType = typeof(resxDbFields))]
        public string TransferMethod { get; set; }

        [Display(Name = "CertifiedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.DateTime)]

        public DateTime? CertifiedDate { get; set; }

        [Display(Name = "CertifierComment", ResourceType = typeof(resxDbFields))]
        public string CertifierComment { get; set; }

        [Display(Name = "FinanceComment", ResourceType = typeof(resxDbFields))]
        public string FinanceComment { get; set; }

        [Display(Name = "Comment", ResourceType = typeof(resxDbFields))]
        public string StaffComment { get; set; }


        [Display(Name = "PreparedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.DateTime)]

        public DateTime? PreparedDate { get; set; }

        [Display(Name = "CertifiedByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? CertifiedByGUID { get; set; }

        [Display(Name = "PreparedByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? PreparedByGUID { get; set; }

        [Display(Name = "PreparedByGUID", ResourceType = typeof(resxDbFields))]
        public string PreparedBy { get; set; }

        [Display(Name = "CertifiedByGUID", ResourceType = typeof(resxDbFields))]
        public string CertifiedBy { get; set; }


        [Display(Name = "FinanceApprovedByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? FinanceApprovedByGUID { get; set; }

        [Display(Name = "FinanceApprovedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.DateTime)]
        public DateTime? FinanceApprovedDate { get; set; }

        [Display(Name = "FinanceApprovedByGUID", ResourceType = typeof(resxDbFields))]
        public string FinanceApprovedBy { get; set; }




        [Display(Name = "EntitlementStatus", ResourceType = typeof(resxDbFields))]
        public Guid? FlowStatusGUID { get; set; }
        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }

        [Display(Name = "TransferTo", ResourceType = typeof(resxDbFields))]
        public string TransferTo { get; set; }

        [Display(Name = "EntitlementStatus", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusName { get; set; }
        [Display(Name = "TotalEntitlements", ResourceType = typeof(resxDbFields))]
        public decimal? TotalEntitlements { get; set; }

        public int? RequestStage { get; set; }

        [Display(Name = "ReferenceNumber", ResourceType = typeof(resxDbFields))]
        public string ReferenceNumber { get; set; }


        [Display(Name = "IndexNumber", ResourceType = typeof(resxDbFields))]
        public string IndexNumber { get; set; }

        [Display(Name = "PaymentMethodGUID", ResourceType = typeof(resxDbFields))]
        public Guid? PaymentMethodGUID { get; set; }



        public byte[] dataAHDInternationalStaffEntitlementRowVersion { get; set; }
        public bool Active { get; set; }
    }
    public class InternationalStaffEntitlDetailementDataTableModel
    {
        [Display(Name = "InternationalStaffEntitlementDetailGUID", ResourceType = typeof(resxDbFields))]
        public string InternationalStaffEntitlementDetailGUID { get; set; }

        [Display(Name = "InternationalStaffEntitlementGUID", ResourceType = typeof(resxDbFields))]
        public string InternationalStaffEntitlementGUID { get; set; }

        [Display(Name = "EntitlementTypeGUID", ResourceType = typeof(resxDbFields))]
        public string EntitlementTypeGUID { get; set; }

        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public string StaffGUID { get; set; }


        [Display(Name = "EntitlementType", ResourceType = typeof(resxDbFields))]
        public string EntitlementType { get; set; }




        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }

        [Display(Name = "BasePeriodAmount", ResourceType = typeof(resxDbFields))]
        public decimal? BasePeriodAmount { get; set; }

        [Display(Name = "TotalAmount", ResourceType = typeof(resxDbFields))]
        public decimal? TotalAmount { get; set; }

        [Display(Name = "TotalDays", ResourceType = typeof(resxDbFields))]
        public int? TotalDays { get; set; }
        [Display(Name = "IsToAdd", ResourceType = typeof(resxDbFields))]
        public bool? IsToAdd { get; set; }

        [Display(Name = "Type", ResourceType = typeof(resxDbFields))]
        public string Type { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }


        [Display(Name = "PeriodName", ResourceType = typeof(resxDbFields))]
        public string PeriodName { get; set; }


        public byte[] dataAHDInternationalStaffEntitlementDetailRowVersion { get; set; }
        public bool Active { get; set; }
    }

    public class InternationalStaffEntitlDetailementUpdateModel
    {
        [Display(Name = "InternationalStaffEntitlementDetailGUID", ResourceType = typeof(resxDbFields))]
        public Guid? InternationalStaffEntitlementDetailGUID { get; set; }

        [Display(Name = "InternationalStaffEntitlementGUID", ResourceType = typeof(resxDbFields))]
        public Guid? InternationalStaffEntitlementGUID { get; set; }

        [Display(Name = "EntitlementTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? EntitlementTypeGUID { get; set; }

        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public Guid? StaffGUID { get; set; }


        [Display(Name = "EntitlementType", ResourceType = typeof(resxDbFields))]
        public string EntitlementType { get; set; }




        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }

        [Display(Name = "BasePeriodAmount", ResourceType = typeof(resxDbFields))]
        public decimal? BasePeriodAmount { get; set; }

        [Display(Name = "TotalAmount", ResourceType = typeof(resxDbFields))]
        public decimal? TotalAmount { get; set; }

        [Display(Name = "TotalDays", ResourceType = typeof(resxDbFields))]
        public int? TotalDays { get; set; }
        [Display(Name = "IsToAdd", ResourceType = typeof(resxDbFields))]
        public bool IsToAdd { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }


        [Display(Name = "PeriodName", ResourceType = typeof(resxDbFields))]
        public string PeriodName { get; set; }


        public byte[] dataAHDInternationalStaffEntitlementDetailRowVersion { get; set; }
        public bool Active { get; set; }
    }




    #endregion

    public class InternationalStaffLeaveEntitlement
    {

        public Nullable<Guid> UserGUID { get; set; }
        public int? TotalDays { get; set; }


    }
    public class CalendarEvents
    {

        public Nullable<Guid> UserGUID { get; set; }
        public Guid EventId { get; set; }
        public DateTime? EventStartDate { get; set; }
        public DateTime? EventEndDate { get; set; }
        public string EventDescription { get; set; }
        public string Title { get; set; }
        public bool AllDayEvent { get; set; }
        //public bool PreventAppointments { get; set; }
        //public bool PublicHolday { get; set; }
        public bool Cancelled { get; set; }
        public bool Arrived { get; set; }

        public string backgroundColor { get; set; }
        public string borderColor { get; set; }
    }

    public class StaffLeaveHolidayDates
    {
        public DateTime? startdateName { get; set; }
        public DateTime? enddateName { get; set; }
        public Guid? LeaveTypeGUID { get; set; }
        public string LeaveName { get; set; }
    }

    #endregion

    #region Staff Documents RR
    public class RestAndRecuperationRequestDocumentDataTableModel
    {

        [Display(Name = "RestAndRecuperationRequestDocumentGUID", ResourceType = typeof(resxDbFields))]
        public Guid RestAndRecuperationRequestDocumentGUID { get; set; }


        [Display(Name = "RestAndRecuperationLeaveGUID", ResourceType = typeof(resxDbFields))]
        public string RestAndRecuperationLeaveGUID { get; set; }


        [Display(Name = "DocumentTypeGUID", ResourceType = typeof(resxDbFields))]
        public string DocumentTypeGUID { get; set; }


        [Display(Name = "DocumentTypeGUID", ResourceType = typeof(resxDbFields))]
        public string DocumentType { get; set; }



        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataRestAndRecuperationRequestDocumentRowVersion { get; set; }
    }


    public class RestAndRecuperationRequestDocumentUpdateModel
    {
        [Required]
        [Display(Name = "RestAndRecuperationRequestDocumentGUID", ResourceType = typeof(resxDbFields))]
        public Guid RestAndRecuperationRequestDocumentGUID { get; set; }

        [Required]
        [Display(Name = "RestAndRecuperationLeaveGUID", ResourceType = typeof(resxDbFields))]
        public Guid RestAndRecuperationLeaveGUID { get; set; }

        [Required]
        [Display(Name = "DocumentTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid DocumentTypeGUID { get; set; }



        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }


        [Required]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataRestAndRecuperationRequestDocumentRowVersion { get; set; }
    }
    #endregion

    public class StaffTeleComutingLeave
    {
        public Guid StaffGUID { get; set; }
        public int? totalDays { get; set; }
    }

    #region BLOM Shuttle Delegation
    public class BlomShuttleDelegationDateUpdateModel
    {
        [Display(Name = "BlomShuttleDelegationDateGUID", ResourceType = typeof(resxDbFields))]
        public Guid BlomShuttleDelegationDateGUID { get; set; }

        [Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "EndDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? EndDate { get; set; }







        public byte[] dataBlomShuttleDelegationDateRowVersion { get; set; }
        public bool Active { get; set; }
    }

    public class ShuttleDelegationTravelerDataTable
    {
        [Display(Name = "BlomShuttleDelegationTravelerGUID", ResourceType = typeof(resxDbFields))]
        public string BlomShuttleDelegationTravelerGUID { get; set; }

        [Display(Name = "BlomShuttleDelegationDateGUID", ResourceType = typeof(resxDbFields))]
        public string BlomShuttleDelegationDateGUID { get; set; }

        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public string StaffGUID { get; set; }

        [Display(Name = "TravelerName", ResourceType = typeof(resxDbFields))]
        public string TravelerName { get; set; }


        [Display(Name = "NationalIDNumber", ResourceType = typeof(resxDbFields))]
        public string NationalIDNumber { get; set; }
        [Display(Name = "SyrianIDNumber", ResourceType = typeof(resxDbFields))]
        public string SyrianIDNumber { get; set; }

        [Display(Name = "DelgationStaffTypeGUID", ResourceType = typeof(resxDbFields))]
        public string DelgationStaffTypeGUID { get; set; }


        [Display(Name = "DelgationStaffTypeGUID", ResourceType = typeof(resxDbFields))]
        public string DelgationType { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }


        public byte[] dataBlomShuttleDelegationTravelerRowVersion { get; set; }
        public bool Active { get; set; }
    }

    public class ShuttleDelegationTravelerUpdateModel
    {
        [Display(Name = "BlomShuttleDelegationTravelerGUID", ResourceType = typeof(resxDbFields))]
        public Guid BlomShuttleDelegationTravelerGUID { get; set; }

        [Display(Name = "BlomShuttleDelegationDateGUID", ResourceType = typeof(resxDbFields))]
        public Guid? BlomShuttleDelegationDateGUID { get; set; }

        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public Guid? StaffGUID { get; set; }

        [Display(Name = "TravelerName", ResourceType = typeof(resxDbFields))]
        public string TravelerName { get; set; }


        [Display(Name = "NationalIDNumber", ResourceType = typeof(resxDbFields))]
        public string NationalIDNumber { get; set; }
        [Display(Name = "SyrianIDNumber", ResourceType = typeof(resxDbFields))]
        public string SyrianIDNumber { get; set; }

        [Display(Name = "DelgationStaffTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? DelgationStaffTypeGUID { get; set; }




        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }


        public byte[] dataBlomShuttleDelegationTravelerRowVersion { get; set; }
        public bool Active { get; set; }
    }
    public class DelegationStaffRequestDataTable
    {
        [Display(Name = "BlomShuttleDelegationStaffRequestGUID", ResourceType = typeof(resxDbFields))]
        public string BlomShuttleDelegationStaffRequestGUID { get; set; }

        [Display(Name = "BlomShuttleDelegationDateGUID", ResourceType = typeof(resxDbFields))]
        public string BlomShuttleDelegationDateGUID { get; set; }
        [Display(Name = "BlomShuttleDelegationTravelerGUID", ResourceType = typeof(resxDbFields))]
        public string BlomShuttleDelegationTravelerGUID { get; set; }

        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public string StaffGUID { get; set; }

        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreateDate { get; set; }
        [Display(Name = "AmountUSD", ResourceType = typeof(resxDbFields))]
        public string AmountUSD { get; set; }


        [Display(Name = "AmountUSDWritten", ResourceType = typeof(resxDbFields))]
        public string AmountUSDWritten { get; set; }


        [Display(Name = "NationalIDNumber", ResourceType = typeof(resxDbFields))]
        public string NationalIDNumber { get; set; }
        [Display(Name = "SyrianIDNumber", ResourceType = typeof(resxDbFields))]
        public string SyrianIDNumber { get; set; }

        [Display(Name = "AccountNumber", ResourceType = typeof(resxDbFields))]
        public string AccountNumber { get; set; }


        [Display(Name = "BranchName", ResourceType = typeof(resxDbFields))]
        public string BranchName { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }


        public byte[] dataBlomShuttleDelegationStaffRequestRowVersion { get; set; }
        public bool Active { get; set; }
    }

    public class DelegationStaffRequestUpdateModel
    {
        [Display(Name = "BlomShuttleDelegationStaffRequestGUID", ResourceType = typeof(resxDbFields))]
        public Guid BlomShuttleDelegationStaffRequestGUID { get; set; }

        [Display(Name = "BlomShuttleDelegationDateGUID", ResourceType = typeof(resxDbFields))]
        public Guid? BlomShuttleDelegationDateGUID { get; set; }
        [Display(Name = "BlomShuttleDelegationTravelerGUID", ResourceType = typeof(resxDbFields))]
        [Required]
        public Guid? BlomShuttleDelegationTravelerGUID { get; set; }

        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public Guid? StaffGUID { get; set; }


        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }

        [Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]
        public DateTime? StartDate { get; set; }

        [Display(Name = "AmountUSD", ResourceType = typeof(resxDbFields))]
        [Required]
        public int AmountUSD { get; set; }


        [Display(Name = "AmountUSDWritten", ResourceType = typeof(resxDbFields))]
        [Required]
        public string AmountUSDWritten { get; set; }


        [Display(Name = "NationalIDNumber", ResourceType = typeof(resxDbFields))]
        [Required]
        public string NationalIDNumber { get; set; }
        [Display(Name = "SyrianIDNumber", ResourceType = typeof(resxDbFields))]
        [Required]
        public string SyrianIDNumber { get; set; }

        [Display(Name = "AccountNumber", ResourceType = typeof(resxDbFields))]
        [Required]
        public string AccountNumber { get; set; }


        [Display(Name = "BranchName", ResourceType = typeof(resxDbFields))]

        public string BranchName { get; set; }

        [Display(Name = "BranchName", ResourceType = typeof(resxDbFields))]
        [Required]
        public Guid? BankBranchGUID { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }


        public byte[] dataBlomShuttleDelegationStaffRequestRowVersion { get; set; }
        public bool Active { get; set; }
    }

    public class UploadDelegationRequestVM
    {
        public HttpPostedFileBase UploadDamagedReport { get; set; }
        public Guid BlomShuttleDelegationStaffRequestGUID { get; set; }
        public Guid UserGUID { get; set; }
    }

    public class FileInfoVM
    {
        public int FileId
        {
            get;
            set;
        }
        public string FileName
        {
            get;
            set;
        }
        public string FilePath
        {
            get;
            set;
        }
    }
    #endregion
    #region Salary Cycle
    public class SalaryCyclePeriodDataTableModel
    {
        [Display(Name = "SalaryCycleGUID", ResourceType = typeof(resxDbFields))]
        public string SalaryCycleGUID { get; set; }

        [Display(Name = "Year", ResourceType = typeof(resxDbFields))]
        public int Year { get; set; }

        [Display(Name = "MonthName", ResourceType = typeof(resxDbFields))]
        public string MonthName { get; set; }

        [Display(Name = "CycleName", ResourceType = typeof(resxDbFields))]
        public string CycleName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public string CreatedBy { get; set; }
        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreateDate { get; set; }


        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
        public string CreateByGUID { get; set; }

        [Display(Name = "FlowStatus", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusGUID { get; set; }

        [Display(Name = "FlowStatus", ResourceType = typeof(resxDbFields))]
        public string LastFlowName { get; set; }



        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]

        public string Comments { get; set; }


        public string LastFlowStatus { get; set; }

        public bool Active { get; set; }

        public byte[] dataSalaryCycleRowVersion { get; set; }
    }
    public class SalaryCyclePeriodUpdateModel
    {
        [Display(Name = "SalaryCycleGUID", ResourceType = typeof(resxDbFields))]
        public Guid SalaryCycleGUID { get; set; }

        [Display(Name = "Year", ResourceType = typeof(resxDbFields))]
        public int Year { get; set; }

        [Display(Name = "MonthName", ResourceType = typeof(resxDbFields))]
        public string MonthName { get; set; }

        [Display(Name = "CycleName", ResourceType = typeof(resxDbFields))]
        public string CycleName { get; set; }



        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreateDate { get; set; }


        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? CreateByGUID { get; set; }

        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public Guid? LastFlowStatusGUID { get; set; }
        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }


        public string LastFlowStatus { get; set; }

        public bool Active { get; set; }

        public byte[] dataSalaryCycleRowVersion { get; set; }
    }
    #endregion
    #region Staff Overtime
    public class OvertimeMonthCycleDataTableModel
    {
        [Display(Name = "OvertimeMonthCycleGUID", ResourceType = typeof(resxDbFields))]
        public string OvertimeMonthCycleGUID { get; set; }
        [Display(Name = "Year", ResourceType = typeof(resxDbFields))]
        public string Year { get; set; }
        [Display(Name = "Month", ResourceType = typeof(resxDbFields))]
        public string Month { get; set; }
        [Display(Name = "CycleName", ResourceType = typeof(resxDbFields))]
        public string CycleName { get; set; }
        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string MonthCycleStatusGUID { get; set; }
        [Display(Name = "TotalStaff", ResourceType = typeof(resxDbFields))]
        public string TotalStaff { get; set; }

        [Display(Name = "OrderId", ResourceType = typeof(resxDbFields))]
        public int? OrderID { get; set; }

        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string Status { get; set; }

        public byte[] dataOvertimeMonthCycleRowVersion { get; set; }

    }

    public class OvertimeMonthCycleUpdateModel
    {
        [Display(Name = "OvertimeMonthCycleGUID", ResourceType = typeof(resxDbFields))]
        public Guid OvertimeMonthCycleGUID { get; set; }
        [Display(Name = "Year", ResourceType = typeof(resxDbFields))]
        public string Year { get; set; }
        [Display(Name = "Month", ResourceType = typeof(resxDbFields))]
        public string Month { get; set; }
        [Display(Name = "CycleName", ResourceType = typeof(resxDbFields))]
        public string CycleName { get; set; }
        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string MonthCycleStatusGUID { get; set; }
        [Display(Name = "TotalStaff", ResourceType = typeof(resxDbFields))]
        public string TotalStaff { get; set; }

        [Display(Name = "OrderId", ResourceType = typeof(resxDbFields))]
        public int? OrderID { get; set; }

        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string Status { get; set; }
        public bool Active { get; set; }

        public byte[] dataOvertimeMonthCycleRowVersion { get; set; }

    }

    public class OvertimeMonthCycleStaffDataTableModel
    {
        [Display(Name = "OvertimeMonthCycleGUID", ResourceType = typeof(resxDbFields))]
        public string OvertimeMonthCycleGUID { get; set; }

        [Display(Name = "OvertimeMonthCycleStaffGUID", ResourceType = typeof(resxDbFields))]
        public string OvertimeMonthCycleStaffGUID { get; set; }

        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
        public string UserGUID { get; set; }
        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }

        [Display(Name = "TotalHoursPayed", ResourceType = typeof(resxDbFields))]
        public double? TotalHoursPayed { get; set; }

        [Display(Name = "TotalPerformedHours", ResourceType = typeof(resxDbFields))]
        public double? TotalPerformedHours { get; set; }

        [Display(Name = "TotalPay", ResourceType = typeof(resxDbFields))]
        public double? TotalPay { get; set; }


        public bool Active { get; set; }

        [Display(Name = "Year", ResourceType = typeof(resxDbFields))]
        public string Year { get; set; }
        [Display(Name = "Month", ResourceType = typeof(resxDbFields))]
        public string Month { get; set; }
        [Display(Name = "CycleName", ResourceType = typeof(resxDbFields))]
        public string CycleName { get; set; }
        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusGUID { get; set; }

        [Display(Name = "OrderId", ResourceType = typeof(resxDbFields))]
        public int? OrderID { get; set; }

        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string Status { get; set; }

        public byte[] dataOvertimeMonthCycleStaffRowVersion { get; set; }

    }

    public class StffCalendarOvertimeUpdateModel
    {
        [Display(Name = "OvertimeMonthCycleGUID", ResourceType = typeof(resxDbFields))]
        public Guid OvertimeMonthCycleGUID { get; set; }

        public DateTime? FirstDateInMonth { get; set; }

        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public Guid UserGUID { get; set; }

        [Display(Name = "JobTitleGUID", ResourceType = typeof(resxDbFields))]
        public Guid? JobTitleGUID { get; set; }

        [Display(Name = "TotalHoursPayed", ResourceType = typeof(resxDbFields))]
        public double? TotalHoursPayed { get; set; }

        [Display(Name = "TotalPerformedHours", ResourceType = typeof(resxDbFields))]
        public double? TotalPerformedHours { get; set; }

        [Display(Name = "TotalPay", ResourceType = typeof(resxDbFields))]
        public double? TotalPay { get; set; }

        [Display(Name = "OvertimeMonthCycleStaffGUID", ResourceType = typeof(resxDbFields))]
        public Guid? OvertimeMonthCycleStaffGUID { get; set; }
        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public Guid? FlowStatusGUID { get; set; }
        [Display(Name = "Grade", ResourceType = typeof(resxDbFields))]
        public string Grade { get; set; }
        [Display(Name = "Step", ResourceType = typeof(resxDbFields))]
        public string Step { get; set; }
        [Display(Name = "JobTitleGUID", ResourceType = typeof(resxDbFields))]
        public string JobTitle { get; set; }
        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public string FullName { get; set; }
        public bool Active { get; set; }
        public byte[] dataOvertimeMonthCycleRowVersion { get; set; }

    }
    public class StaffOvertimeForSalaryDataTableModel
    {
        [Display(Name = "StaffOvertimeGUID", ResourceType = typeof(resxDbFields))]
        public string StaffOvertimeGUID { get; set; }

        [Display(Name = "OvertimeMonthCycleStaffGUID", ResourceType = typeof(resxDbFields))]
        public string OvertimeMonthCycleStaffGUID { get; set; }

        [Display(Name = "OvertimeMonthCycleGUID", ResourceType = typeof(resxDbFields))]
        public string OvertimeMonthCycleGUID { get; set; }
        [Display(Name = "TotalHoursPayed", ResourceType = typeof(resxDbFields))]
        public double? TotalHoursPayed { get; set; }
        [Display(Name = "Month", ResourceType = typeof(resxDbFields))]
        public string CycleName { get; set; }

        [Display(Name = "PerformedHours", ResourceType = typeof(resxDbFields))]
        public double? TotalPerformedHours { get; set; }

        [Display(Name = "TotalPay", ResourceType = typeof(resxDbFields))]
        public double? TotalPay { get; set; }

        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string UserGUID { get; set; }

        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }


        [Display(Name = "DutyStation", ResourceType = typeof(resxDbFields))]
        public string DutyStation { get; set; }


        [Display(Name = "JobTitle", ResourceType = typeof(resxDbFields))]
        public string JobTitle { get; set; }

        [Display(Name = "Year", ResourceType = typeof(resxDbFields))]
        public string Year { get; set; }

        [Display(Name = "MonthName", ResourceType = typeof(resxDbFields))]
        public string Month { get; set; }

        [Display(Name = "ActionDay", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ActionDate { get; set; }

        [Display(Name = "TimeIn", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? TimeIn { get; set; }

        [Display(Name = "ActionDay", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? TimeOut { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreateDate { get; set; }





        [Display(Name = "Reason", ResourceType = typeof(resxDbFields))]

        public string OvertimeReason { get; set; }
        [Display(Name = "CTO", ResourceType = typeof(resxDbFields))]

        public double? CTO { get; set; }


        [Display(Name = "Step", ResourceType = typeof(resxDbFields))]

        public string Step { get; set; }
        [Display(Name = "Step", ResourceType = typeof(resxDbFields))]

        public string StepGUID { get; set; }
        [Display(Name = "Grade", ResourceType = typeof(resxDbFields))]

        public string Grade { get; set; }

        [Display(Name = "StaffGradeGUID", ResourceType = typeof(resxDbFields))]

        public string GradeGUID { get; set; }

        [Display(Name = "DutyStation", ResourceType = typeof(resxDbFields))]

        public string DutyStationGUID { get; set; }
        [Display(Name = "JobTitleGUID", ResourceType = typeof(resxDbFields))]

        public string JobTitleGUID { get; set; }




        [Display(Name = "FlowStatus", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusGUID { get; set; }

        [Display(Name = "FlowStatus", ResourceType = typeof(resxDbFields))]
        public string LastFlowName { get; set; }



        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]

        public string Comments { get; set; }
        public bool Active { get; set; }

        public byte[] dataOvertimeMonthCycleStaffRowVersion { get; set; }
    }

    public class StaffOvertimeDataTableModel
    {
        [Display(Name = "StaffOvertimeGUID", ResourceType = typeof(resxDbFields))]
        public string StaffOvertimeGUID { get; set; }

        [Display(Name = "OvertimeMonthCycleStaffGUID", ResourceType = typeof(resxDbFields))]
        public string OvertimeMonthCycleStaffGUID { get; set; }

        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string UserGUID { get; set; }

        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }


        [Display(Name = "DutyStation", ResourceType = typeof(resxDbFields))]
        public string DutyStation { get; set; }


        [Display(Name = "JobTitle", ResourceType = typeof(resxDbFields))]
        public string JobTitle { get; set; }

        [Display(Name = "Year", ResourceType = typeof(resxDbFields))]
        public string Year { get; set; }

        [Display(Name = "MonthName", ResourceType = typeof(resxDbFields))]
        public string Month { get; set; }
        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public string CreatedByGUID { get; set; }



        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public string CreatedBy { get; set; }
        [Display(Name = "ActionDay", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ActionDate { get; set; }

        [Display(Name = "TimeIn", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? TimeIn { get; set; }

        [Display(Name = "ActionDay", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? TimeOut { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreateDate { get; set; }



        [Display(Name = "TotalHours", ResourceType = typeof(resxDbFields))]

        public float TotalHours { get; set; }

        [Display(Name = "TotalPay", ResourceType = typeof(resxDbFields))]

        public float TotalPay { get; set; }

        [Display(Name = "Reason", ResourceType = typeof(resxDbFields))]

        public string OvertimeReason { get; set; }
        [Display(Name = "CTO", ResourceType = typeof(resxDbFields))]

        public double? CTO { get; set; }

        [Display(Name = "PerformedHours", ResourceType = typeof(resxDbFields))]

        public double? PerformedHours { get; set; }
        [Display(Name = "Step", ResourceType = typeof(resxDbFields))]

        public string Step { get; set; }
        [Display(Name = "Step", ResourceType = typeof(resxDbFields))]

        public string StepGUID { get; set; }
        [Display(Name = "Grade", ResourceType = typeof(resxDbFields))]

        public string Grade { get; set; }

        [Display(Name = "StaffGradeGUID", ResourceType = typeof(resxDbFields))]

        public string GradeGUID { get; set; }

        [Display(Name = "DutyStation", ResourceType = typeof(resxDbFields))]

        public string DutyStationGUID { get; set; }
        [Display(Name = "JobTitleGUID", ResourceType = typeof(resxDbFields))]

        public string JobTitleGUID { get; set; }




        [Display(Name = "FlowStatus", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusGUID { get; set; }

        [Display(Name = "FlowStatus", ResourceType = typeof(resxDbFields))]
        public string LastFlowName { get; set; }



        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]

        public string Comments { get; set; }
        public bool Active { get; set; }

        public byte[] dataStaffOvertimeRowVersion { get; set; }
    }
    public class StaffOvertimeUpdateModel
    {
        [Display(Name = "StaffOvertimeGUID", ResourceType = typeof(resxDbFields))]
        public Guid StaffOvertimeGUID { get; set; }
        [Display(Name = "OvertimeMonthCycleStaffGUID", ResourceType = typeof(resxDbFields))]
        public Guid? OvertimeMonthCycleStaffGUID { get; set; }
        [Display(Name = "OvertimeMonthCycleGUID", ResourceType = typeof(resxDbFields))]
        public Guid? OvertimeMonthCycleGUID { get; set; }
        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> UserGUID { get; set; }

        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }

        [Display(Name = "Year", ResourceType = typeof(resxDbFields))]
        public int? Year { get; set; }

        [Display(Name = "StepGUID", ResourceType = typeof(resxDbFields))]
        public Guid? StepGUID { get; set; }
        [Display(Name = "StaffGradeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? GradeGUID { get; set; }
        [Display(Name = "JobtitleGUID", ResourceType = typeof(resxDbFields))]
        public Guid? JobtitleGUID { get; set; }


        [Display(Name = "DayWorkingTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? DayWorkingTypeGUID { get; set; }

        [Display(Name = "Reason", ResourceType = typeof(resxDbFields))]

        public string OvertimeReason { get; set; }
        [Display(Name = "CTO", ResourceType = typeof(resxDbFields))]

        public double? CTO { get; set; }

        [Display(Name = "PerformedHours", ResourceType = typeof(resxDbFields))]

        public double? PerformedHours { get; set; }





        [Display(Name = "MonthName", ResourceType = typeof(resxDbFields))]
        public string Month { get; set; }
        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> CreatedByGUID { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreateDate { get; set; }


        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public string CreatedBy { get; set; }
        [Display(Name = "ActionDay", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ActionDate { get; set; }

        [Display(Name = "TimeIn", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? TimeIn { get; set; }

        //[Display(Name = "TimeIn", ResourceType = typeof(resxDbFields))]
        //[DataType(DataType.Time)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh:mm tt}")]


        //public DateTime? test { get; set; }

        public string HourIn { get; set; }

        public string HourOut { get; set; }

        [Display(Name = "TimeOut", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? TimeOut { get; set; }



        [Display(Name = "TotalHours", ResourceType = typeof(resxDbFields))]

        public float? TotalHours { get; set; }

        [Display(Name = "TotalPay", ResourceType = typeof(resxDbFields))]

        public float? TotalPay { get; set; }

        [Display(Name = "DayWorkingType", ResourceType = typeof(resxDbFields))]

        public string DayWorkingType { get; set; }

        [Display(Name = "Step", ResourceType = typeof(resxDbFields))]

        public string Step { get; set; }


        [Display(Name = "Grade", ResourceType = typeof(resxDbFields))]

        public string Grade { get; set; }


        [Display(Name = "FlowStatus", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> LastFlowStatusGUID { get; set; }

        [Display(Name = "FlowStatus", ResourceType = typeof(resxDbFields))]
        public string LastFlowName { get; set; }



        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]

        public string Comments { get; set; }
        public bool Active { get; set; }

        public byte[] dataStaffOvertimeRowVersion { get; set; }
    }

    #endregion

    #region HQ Salary

    public class HQStaffSalaryDataTableModel
    {
        [Display(Name = "SalaryCycleGUID", ResourceType = typeof(resxDbFields))]
        public string TempHQStaffSalaryGUID { get; set; }

        [Display(Name = "SalaryCycleGUID", ResourceType = typeof(resxDbFields))]
        public string SalaryCycleGUID { get; set; }

        [Display(Name = "Salary", ResourceType = typeof(resxDbFields))]
        public float HQSalary { get; set; }

        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string UserGUID { get; set; }



        [Display(Name = "VendorID", ResourceType = typeof(resxDbFields))]
        public string VendorID { get; set; }

        [Display(Name = "EmployeeID", ResourceType = typeof(resxDbFields))]
        public string EmployeeID { get; set; }

        [Display(Name = "FullName", ResourceType = typeof(resxDbFields))]
        public string FullName { get; set; }



        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]

        public string Comments { get; set; }


        public string LastFlowStatus { get; set; }

        public bool Active { get; set; }

        public byte[] dataTempHQStaffSalaryowVersion { get; set; }
    }

    public class HQStaffSalaryUpdateModel
    {
        [Display(Name = "SalaryCycleGUID", ResourceType = typeof(resxDbFields))]
        public Guid TempHQStaffSalaryGUID { get; set; }

        [Display(Name = "SalaryCycleGUID", ResourceType = typeof(resxDbFields))]
        public Guid? SalaryCycleGUID { get; set; }

        [Display(Name = "Salary", ResourceType = typeof(resxDbFields))]
        public float HQSalary { get; set; }
        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> UserGUID { get; set; }

        [Display(Name = "VendorID", ResourceType = typeof(resxDbFields))]
        public int VendorID { get; set; }

        [Display(Name = "EmployeeID", ResourceType = typeof(resxDbFields))]
        public string EmployeeID { get; set; }

        [Display(Name = "FullName", ResourceType = typeof(resxDbFields))]
        public string FullName { get; set; }



        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]

        public string Comments { get; set; }




        public bool Active { get; set; }

        public byte[] dataTempHQStaffSalaryowVersion { get; set; }
    }

    #endregion

    #region Staff Medical Payment

    public class StaffMedicalPaymentDataTableModel
    {

        [Display(Name = "StaffMedicalPaymentGUID", ResourceType = typeof(resxDbFields))]
        public string StaffMedicalPaymentGUID { get; set; }

        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string UserGUID { get; set; }

        [Display(Name = "ClaimNumber", ResourceType = typeof(resxDbFields))]
        public string ClaimNumber { get; set; }
        [Display(Name = "Year", ResourceType = typeof(resxDbFields))]
        public string Year { get; set; }

        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]

        public string StaffName { get; set; }

        [Display(Name = "Month", ResourceType = typeof(resxDbFields))]
        public string Month { get; set; }

        [Display(Name = "AmountSYR", ResourceType = typeof(resxDbFields))]
        public float AmountSYR { get; set; }



        [Display(Name = "AmountUSD", ResourceType = typeof(resxDbFields))]
        public float AmountUSD { get; set; }

        [Display(Name = "CurrencyExchangeUSD", ResourceType = typeof(resxDbFields))]
        public float CurrencyExchangeUSD { get; set; }

        [Display(Name = "ActionDate", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? ActionDate { get; set; }




        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public string CreatedByGUID { get; set; }

        [Display(Name = "MedicalReason", ResourceType = typeof(resxDbFields))]
        public string MedicalReason { get; set; }



        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public string CreateBy { get; set; }
        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "UpdatedByGUID", ResourceType = typeof(resxDbFields))]
        public string UpdatedByGUID { get; set; }
        [Display(Name = "UpdatedByGUID", ResourceType = typeof(resxDbFields))]
        public string UpdateBy { get; set; }
        [Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? UpdateDate { get; set; }



        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusGUID { get; set; }

        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string Status { get; set; }





        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]

        public string Comments { get; set; }


        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }


        public byte[] dataStaffMedicalPaymentRowVersion { get; set; }
    }

    public class StaffMedicalPaymentUpdateModel
    {

        [Display(Name = "StaffMedicalPaymentGUID", ResourceType = typeof(resxDbFields))]
        public Guid StaffMedicalPaymentGUID { get; set; }

        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public Guid? UserGUID { get; set; }

        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]

        public string StaffName { get; set; }
        [Display(Name = "ClaimNumber", ResourceType = typeof(resxDbFields))]
        public string ClaimNumber { get; set; }

        [Display(Name = "Year", ResourceType = typeof(resxDbFields))]
        public int? Year { get; set; }
        [Display(Name = "Month", ResourceType = typeof(resxDbFields))]
        public string Month { get; set; }



        [Display(Name = "AmountSYR", ResourceType = typeof(resxDbFields))]
        public float AmountSYR { get; set; }



        [Display(Name = "AmountUSD", ResourceType = typeof(resxDbFields))]
        public float AmountUSD { get; set; }

        [Display(Name = "CurrencyExchangeUSD", ResourceType = typeof(resxDbFields))]
        public float CurrencyExchangeUSD { get; set; }

        [Display(Name = "ActionDate", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? ActionDate { get; set; }




        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? CreatedByGUID { get; set; }

        [Display(Name = "MedicalReason", ResourceType = typeof(resxDbFields))]
        public string MedicalReason { get; set; }





        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "UpdatedByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? UpdatedByGUID { get; set; }
        [Display(Name = "UpdatedByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? UpdateBy { get; set; }
        [Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? UpdateDate { get; set; }



        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public Guid? LastFlowStatusGUID { get; set; }

        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string Status { get; set; }





        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]

        public string Comments { get; set; }


        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }


        public byte[] dataStaffMedicalPaymentRowVersion { get; set; }
    }
    #endregion


    #region Staff  Salary Overview
    public class StaffSalaryDataTableModel
    {
        [Display(Name = "StaffSalaryGUID", ResourceType = typeof(resxDbFields))]
        public string StaffSalaryGUID { get; set; }

        [Display(Name = "SalaryCycleGUID", ResourceType = typeof(resxDbFields))]
        public string SalaryCycleGUID { get; set; }

        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string UserGUID { get; set; }
        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }

        [Display(Name = "HQSalary", ResourceType = typeof(resxDbFields))]
        public double? HQSalary { get; set; }

        [Display(Name = "AdvancedSalary", ResourceType = typeof(resxDbFields))]
        public double? AdvancedSalary { get; set; }

        [Display(Name = "TeleBills", ResourceType = typeof(resxDbFields))]
        public double? TeleBills { get; set; }

        [Display(Name = "Internet4G", ResourceType = typeof(resxDbFields))]
        public double? Internet4G { get; set; }
        [Display(Name = "PhoneCall", ResourceType = typeof(resxDbFields))]
        public double? PhoneCall { get; set; }
        [Display(Name = "AddOthers", ResourceType = typeof(resxDbFields))]
        public double? AddOthers { get; set; }
        [Display(Name = "DangerPay", ResourceType = typeof(resxDbFields))]
        public double? DangerPay { get; set; }
        [Display(Name = "Overtime", ResourceType = typeof(resxDbFields))]
        public double? Overtime { get; set; }

        [Display(Name = "DeductionOthers", ResourceType = typeof(resxDbFields))]
        public double? DeductionOthers { get; set; }

        [Display(Name = "MIP", ResourceType = typeof(resxDbFields))]
        public double? MIP { get; set; }

        [Display(Name = "TotalSalary", ResourceType = typeof(resxDbFields))]
        public double? TotalSalary { get; set; }
        [Display(Name = "BankGUID", ResourceType = typeof(resxDbFields))]
        public string BankGUID { get; set; }

        [Display(Name = "BankName", ResourceType = typeof(resxDbFields))]
        public string BankName { get; set; }
        [Display(Name = "UNFCUBank", ResourceType = typeof(resxDbFields))]
        public string UNFCUBank { get; set; }
        [Display(Name = "VoucherNumber", ResourceType = typeof(resxDbFields))]
        public string VoucherNumber { get; set; }
        [Display(Name = "BranchName", ResourceType = typeof(resxDbFields))]
        public string BranchName { get; set; }
        [Display(Name = "NameInBLOMBank", ResourceType = typeof(resxDbFields))]
        public string NameInBLOMBank { get; set; }

        [Display(Name = "BranchName", ResourceType = typeof(resxDbFields))]
        public Guid? BankBranchGUID { get; set; }
        [Display(Name = "IBANNo", ResourceType = typeof(resxDbFields))]
        public string IBANNo { get; set; }
        [Display(Name = "AccountNumber", ResourceType = typeof(resxDbFields))]
        public string AccountNumber { get; set; }
        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string StaffConfirmationStatusGUID { get; set; }

        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string StaffConfirmationStatus { get; set; }
        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string Status { get; set; }
        [Display(Name = "FlowStatusGUID", ResourceType = typeof(resxDbFields))]
        public string FlowStatusGUID { get; set; }
        [Display(Name = "LastFlowStatusName", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusName { get; set; }
        [Display(Name = "IsLastAction", ResourceType = typeof(resxDbFields))]
        public bool? IsLastAction { get; set; }

        [Display(Name = "ActionDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ActionDate { get; set; }


        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
        public string CreateByGUID { get; set; }

        [Display(Name = "FlowStatus", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusGUID { get; set; }

        [Display(Name = "FlowStatus", ResourceType = typeof(resxDbFields))]
        public string LastFlowName { get; set; }



        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]

        public string Comments { get; set; }




        public bool Active { get; set; }

        public byte[] dataStaffSalaryRowVersion { get; set; }
    }
    public class StaffSalaryUpdateModel
    {
        [Display(Name = "StaffSalaryGUID", ResourceType = typeof(resxDbFields))]
        public Guid StaffSalaryGUID { get; set; }

        [Display(Name = "SalaryCycleGUID", ResourceType = typeof(resxDbFields))]
        public Guid SalaryCycleGUID { get; set; }

        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public Guid UserGUID { get; set; }

        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }
        [Display(Name = "HQSalary", ResourceType = typeof(resxDbFields))]
        public double? HQSalary { get; set; }
        [Display(Name = "BranchName", ResourceType = typeof(resxDbFields))]
        public string BankBranchGUID { get; set; }
        [Display(Name = "BranchName", ResourceType = typeof(resxDbFields))]
        public Guid? StaffConfirmationStatusGUID { get; set; }
        [Display(Name = "AdvancedSalary", ResourceType = typeof(resxDbFields))]
        public double? AdvancedSalary { get; set; }

        [Display(Name = "TeleBills", ResourceType = typeof(resxDbFields))]
        public double? TeleBills { get; set; }

        [Display(Name = "Internet4G", ResourceType = typeof(resxDbFields))]
        public double? Internet4G { get; set; }
        [Display(Name = "PhoneCall", ResourceType = typeof(resxDbFields))]
        public double? PhoneCall { get; set; }
        [Display(Name = "AddOthers", ResourceType = typeof(resxDbFields))]
        public double? AddOthers { get; set; }
        [Display(Name = "DangerPay", ResourceType = typeof(resxDbFields))]
        public double? DangerPay { get; set; }
        [Display(Name = "Overtime", ResourceType = typeof(resxDbFields))]
        public double? Overtime { get; set; }

        [Display(Name = "DeductionOthers", ResourceType = typeof(resxDbFields))]
        public double? DeductionOthers { get; set; }

        [Display(Name = "MIP", ResourceType = typeof(resxDbFields))]
        public double? MIP { get; set; }

        [Display(Name = "TotalSalary", ResourceType = typeof(resxDbFields))]
        public double? TotalSalary { get; set; }
        [Display(Name = "Bank", ResourceType = typeof(resxDbFields))]
        public Guid? BankGUID { get; set; }

        [Display(Name = "Bank", ResourceType = typeof(resxDbFields))]
        public string BankName { get; set; }
        [Display(Name = "UNFCUBank", ResourceType = typeof(resxDbFields))]
        public string UNFCUBank { get; set; }
        [Display(Name = "VoucherNumber", ResourceType = typeof(resxDbFields))]
        public string VoucherNumber { get; set; }
        [Display(Name = "BranchName", ResourceType = typeof(resxDbFields))]
        public string BranchName { get; set; }
        [Display(Name = "NameInBLOMBank", ResourceType = typeof(resxDbFields))]
        public string NameInBLOMBank { get; set; }
        [Display(Name = "IBANNo", ResourceType = typeof(resxDbFields))]
        public string IBANNo { get; set; }
        [Display(Name = "AccountNumber", ResourceType = typeof(resxDbFields))]
        public string AccountNumber { get; set; }


        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string StaffConfirmationStatus { get; set; }
        [Display(Name = "FlowStatusGUID", ResourceType = typeof(resxDbFields))]
        public string FlowStatusGUID { get; set; }
        [Display(Name = "LastFlowStatusName", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusName { get; set; }
        [Display(Name = "IsLastAction", ResourceType = typeof(resxDbFields))]
        public bool? IsLastAction { get; set; }

        [Display(Name = "ActionDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ActionDate { get; set; }


        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? CreateByGUID { get; set; }

        [Display(Name = "FlowStatus", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusGUID { get; set; }

        [Display(Name = "FlowStatus", ResourceType = typeof(resxDbFields))]
        public string LastFlowName { get; set; }



        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]

        public string Comments { get; set; }




        public bool Active { get; set; }

        public byte[] dataStaffSalaryRowVersion { get; set; }
    }
    #endregion

    #region Staff Salary Step

    public class SalaryCycleStepDataTableModel
    {

        [Display(Name = "SalaryCycleStepGUID", ResourceType = typeof(resxDbFields))]
        public string SalaryCycleStepGUID { get; set; }

        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string UserGUID { get; set; }

        [Display(Name = "SalaryCycleGUID", ResourceType = typeof(resxDbFields))]
        public string SalaryCycleGUID { get; set; }

        [Display(Name = "AccessLevl", ResourceType = typeof(resxDbFields))]

        public int? AccessLevl { get; set; }
        [Display(Name = "StepGUID", ResourceType = typeof(resxDbFields))]
        public string StepGUID { get; set; }


        [Display(Name = "StepGUID", ResourceType = typeof(resxDbFields))]
        public string Step { get; set; }

        public int? SortID { get; set; }

        [Display(Name = "ActionDate", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? ActionDate { get; set; }




        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public string CreateByGUID { get; set; }



        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public string CreateBy { get; set; }
        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "UpdatedByGUID", ResourceType = typeof(resxDbFields))]
        public string UpdatedByGUID { get; set; }
        [Display(Name = "UpdatedByGUID", ResourceType = typeof(resxDbFields))]
        public string UpdateBy { get; set; }
        [Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? UpdateDate { get; set; }



        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusGUID { get; set; }

        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string Status { get; set; }





        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]

        public string Comments { get; set; }


        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }


        public byte[] dataSalaryCycleStepRowVersion { get; set; }
    }

    public class SalaryCycleStepUpdateModel
    {

        [Display(Name = "SalaryCycleStepGUID", ResourceType = typeof(resxDbFields))]
        public Guid SalaryCycleStepGUID { get; set; }

        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public Guid? UserGUID { get; set; }

        [Display(Name = "SalaryCycleGUID", ResourceType = typeof(resxDbFields))]
        public Guid? SalaryCycleGUID { get; set; }



        [Display(Name = "StepGUID", ResourceType = typeof(resxDbFields))]
        public Guid? StepGUID { get; set; }




        [Display(Name = "ActionDate", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? ActionDate { get; set; }




        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public string CreateByGUID { get; set; }



        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public string CreateBy { get; set; }
        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "UpdatedByGUID", ResourceType = typeof(resxDbFields))]
        public string UpdatedByGUID { get; set; }
        [Display(Name = "UpdatedByGUID", ResourceType = typeof(resxDbFields))]
        public string UpdateBy { get; set; }
        [Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? UpdateDate { get; set; }



        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusGUID { get; set; }

        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string Status { get; set; }





        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]

        public string Comments { get; set; }


        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }


        public byte[] dataSalaryCycleStepRowVersion { get; set; }
    }
    #endregion
    #region Billing
    public class CycleSalryBillingDataTableModel
    {

        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string UserGUID { get; set; }
        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }

        [Display(Name = "DutyStation", ResourceType = typeof(resxDbFields))]
        public string SIMWarehouseDescription { get; set; }
        [Display(Name = "Month", ResourceType = typeof(resxDbFields))]
        public string BillForMonth { get; set; }
        [Display(Name = "Year", ResourceType = typeof(resxDbFields))]
        public string BillForYear { get; set; }
        [Display(Name = "DeductFromSalaryAmount", ResourceType = typeof(resxDbFields))]
        public double? DeductFromSalaryAmount { get; set; }

        [Display(Name = "PayInCashAmount", ResourceType = typeof(resxDbFields))]
        public double? SumPayInCashAmount { get; set; }
        public byte[] StaffCoreDataRowVersion { get; set; }

        public bool Active { get; set; }

    }
    #endregion
    #region Missions
    #region Mission Request


    public class StaffMissionRequestDataTable
    {
        [Display(Name = "MissionRequestGUID", ResourceType = typeof(resxDbFields))]
        public string MissionRequestGUID { get; set; }
        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }

        [Display(Name = "SequenceNumber", ResourceType = typeof(resxDbFields))]
        public int? SequenceNumber { get; set; }

        [Display(Name = "ReferenceNumber", ResourceType = typeof(resxDbFields))]
        public string ReferenceNumber { get; set; }


        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public string StaffGUID { get; set; }

        [Display(Name = "Grade", ResourceType = typeof(resxDbFields))]
        public string Grade { get; set; }


        [Display(Name = "JobTitle", ResourceType = typeof(resxDbFields))]
        public string JobTitle { get; set; }

        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
        public string DepartmentGUID { get; set; }


        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public string DutyStationGUID { get; set; }

        [Display(Name = "VersionNumber", ResourceType = typeof(resxDbFields))]
        public int? VersionNumber { get; set; }


        [Display(Name = "Year", ResourceType = typeof(resxDbFields))]
        public string MissionYear { get; set; }

        [Display(Name = "MissionCode", ResourceType = typeof(resxDbFields))]
        public string MissionCode { get; set; }


        [Display(Name = "OfficialEventEndDate", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? DepartureDate { get; set; }

        [Display(Name = "OfficialEventStartDate", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ReturnDate { get; set; }
        [Display(Name = "TypeOfTravelGUID", ResourceType = typeof(resxDbFields))]
        public string TypeOfTravelGUID { get; set; }

        [Display(Name = "TypeOfTravelGUID", ResourceType = typeof(resxDbFields))]
        public string TypeOfTravel { get; set; }

        [Display(Name = "TypeOfMissionGUID", ResourceType = typeof(resxDbFields))]
        public string TypeOfMissionGUID { get; set; }

        [Display(Name = "TypeOfMissionGUID", ResourceType = typeof(resxDbFields))]
        public string TypeOfMission { get; set; }

        [Display(Name = "PurposeOfMission", ResourceType = typeof(resxDbFields))]
        public string PurposeOfMission { get; set; }


        [Display(Name = "HasDamascusTransit", ResourceType = typeof(resxDbFields))]
        public bool? HasDamascusTransit { get; set; }


        [Display(Name = "IsMissionCombinedWithLeave", ResourceType = typeof(resxDbFields))]
        public bool? IsMissionCombinedWithLeave { get; set; }


        [Display(Name = "IsMissionCombinedWithRR", ResourceType = typeof(resxDbFields))]
        public bool? IsMissionCombinedWithRR { get; set; }

        [Display(Name = "RestAndRecuperationLeaveGUID", ResourceType = typeof(resxDbFields))]
        public string RestAndRecuperationLeaveGUID { get; set; }

        [Display(Name = "BudgetComments", ResourceType = typeof(resxDbFields))]
        public string BudgetComments { get; set; }

        [Display(Name = "BudgetTypeGUID", ResourceType = typeof(resxDbFields))]
        public string BudgetTypeGUID { get; set; }

        [Display(Name = "AdminApprovedByGUID", ResourceType = typeof(resxDbFields))]
        public string AdminApprovedByGUID { get; set; }

        [Display(Name = "AdminApprovedDate", ResourceType = typeof(resxDbFields))]
        public DateTime? AdminApprovedDate { get; set; }

        [Display(Name = "AdminComments", ResourceType = typeof(resxDbFields))]
        public string AdminComments { get; set; }

        [Display(Name = "AuthorizedByGUID", ResourceType = typeof(resxDbFields))]
        public string AuthorizedByGUID { get; set; }

        [Display(Name = "AuthorizedDate", ResourceType = typeof(resxDbFields))]
        public DateTime? AuthorizedDate { get; set; }

        [Display(Name = "AuthorizedComment", ResourceType = typeof(resxDbFields))]
        public string AuthorizedComment { get; set; }

        [Display(Name = "AuthByDeputyRepresentativeGUID", ResourceType = typeof(resxDbFields))]
        public string AuthByDeputyRepresentativeGUID { get; set; }

        [Display(Name = "AuthByDeputyRepDate", ResourceType = typeof(resxDbFields))]
        public DateTime? AuthByDeputyRepDate { get; set; }

        [Display(Name = "AuthByRepresentativeGUID", ResourceType = typeof(resxDbFields))]
        public string AuthByRepresentativeGUID { get; set; }

        [Display(Name = "AuthByRepDate", ResourceType = typeof(resxDbFields))]
        public DateTime? AuthByRepDate { get; set; }

        [Display(Name = "ReasonOfCancellation", ResourceType = typeof(resxDbFields))]
        public string ReasonOfCancellation { get; set; }

        [Display(Name = "CaneledDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CaneledDate { get; set; }
        [Display(Name = "CanceledByGUID", ResourceType = typeof(resxDbFields))]
        public string CanceledByGUID { get; set; }
        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusGUID { get; set; }

        [Display(Name = "LastFlowStatus", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatus { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreateDate { get; set; }





        public byte[] dataMissionRequestRowVersion { get; set; }

        public bool Active { get; set; }

    }

    public class StaffMissionRequestDataTableModel
    {
        [Display(Name = "MissionRequestGUID", ResourceType = typeof(resxDbFields))]
        public Guid MissionRequestGUID { get; set; }
        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }

        public int? RequestStage { get; set; }

        public Guid? CurrentUserGUID { get; set; }



        [Display(Name = "SequenceNumber", ResourceType = typeof(resxDbFields))]
        public int? SequenceNumber { get; set; }

        [Display(Name = "ReferenceNumber", ResourceType = typeof(resxDbFields))]
        public string ReferenceNumber { get; set; }


        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public Guid? StaffGUID { get; set; }

        [Display(Name = "Grade", ResourceType = typeof(resxDbFields))]
        public string Grade { get; set; }


        [Display(Name = "JobTitle", ResourceType = typeof(resxDbFields))]
        public string JobTitle { get; set; }

        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
        public Guid? DepartmentGUID { get; set; }


        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Guid? DutyStationGUID { get; set; }

        [Display(Name = "VersionNumber", ResourceType = typeof(resxDbFields))]
        public int? VersionNumber { get; set; }

        [Display(Name = "AccessLevel", ResourceType = typeof(resxDbFields))]
        public int? AccessLevel { get; set; }

        [Display(Name = "CurrentStep", ResourceType = typeof(resxDbFields))]
        public int? CurrentStep { get; set; }
        [Display(Name = "RecruitmentTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? RecruitmentTypeGUID { get; set; }



        [Display(Name = "Year", ResourceType = typeof(resxDbFields))]
        public int? MissionYear { get; set; }

        [Display(Name = "MissionCode", ResourceType = typeof(resxDbFields))]
        public string MissionCode { get; set; }

        [Display(Name = "OfficialEventStartDate", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? DepartureDate { get; set; }

        [Display(Name = "OfficialEventEndDate", ResourceType = typeof(resxDbFields))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? ReturnDate { get; set; }
        [Display(Name = "TypeOfTravelGUID", ResourceType = typeof(resxDbFields))]
        public Guid? TypeOfTravelGUID { get; set; }

        [Display(Name = "TypeOfTravelGUID", ResourceType = typeof(resxDbFields))]
        public string TypeOfTravel { get; set; }

        [Display(Name = "TypeOfMissionGUID", ResourceType = typeof(resxDbFields))]
        public Guid? TypeOfMissionGUID { get; set; }

        [Display(Name = "TypeOfMissionGUID", ResourceType = typeof(resxDbFields))]
        public string TypeOfMission { get; set; }

        [Display(Name = "PurposeOfMission", ResourceType = typeof(resxDbFields))]
        public string PurposeOfMission { get; set; }


        [Display(Name = "HasDamascusTransit", ResourceType = typeof(resxDbFields))]
        public bool HasDamascusTransit { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreateDate { get; set; }


        [Display(Name = "IsMissionCombinedWithLeave", ResourceType = typeof(resxDbFields))]
        public bool IsMissionCombinedWithLeave { get; set; }
        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Display(Name = "IsMissionCombinedWithRR", ResourceType = typeof(resxDbFields))]
        public bool IsMissionCombinedWithRR { get; set; }

        [Display(Name = "RestAndRecuperationLeaveGUID", ResourceType = typeof(resxDbFields))]
        public Guid? RestAndRecuperationLeaveGUID { get; set; }

        [Display(Name = "BudgetComments", ResourceType = typeof(resxDbFields))]
        public string BudgetComments { get; set; }

        [Display(Name = "BudgetTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? BudgetTypeGUID { get; set; }

        [Display(Name = "AdminApprovedByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? AdminApprovedByGUID { get; set; }

        [Display(Name = "AdminApprovedByGUID", ResourceType = typeof(resxDbFields))]
        public string AdminApprovedBy { get; set; }

        [Display(Name = "AdminApprovedDate", ResourceType = typeof(resxDbFields))]
        public DateTime? AdminApprovedDate { get; set; }

        [Display(Name = "AdminComments", ResourceType = typeof(resxDbFields))]
        public string AdminComments { get; set; }


        [Display(Name = "HeadOfUnitApprovedByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? HeadOfUnitApprovedByGUID { get; set; }

        [Display(Name = "HeadOfUnitApprovedDate", ResourceType = typeof(resxDbFields))]
        public DateTime? HeadOfUnitApprovedDate { get; set; }



        [Display(Name = "HeadOfUnitComments", ResourceType = typeof(resxDbFields))]
        public string HeadOfUnitComments { get; set; }

        [Display(Name = "HeadOfUnitApprovedByGUID", ResourceType = typeof(resxDbFields))]
        public string HeadOfUnitApprovedName { get; set; }


        [Display(Name = "AuthorizedByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? AuthorizedByGUID { get; set; }
        [Display(Name = "AuthorizedByGUID", ResourceType = typeof(resxDbFields))]
        public string AuthorizedByName { get; set; }



        [Display(Name = "AuthorizedDate", ResourceType = typeof(resxDbFields))]
        public DateTime? AuthorizedDate { get; set; }

        [Display(Name = "AuthorizedComment", ResourceType = typeof(resxDbFields))]
        public string AuthorizedComment { get; set; }

        [Display(Name = "AuthByDeputyRepresentativeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? AuthByDeputyRepresentativeGUID { get; set; }

        [Display(Name = "AuthByDeputyRepresentativeGUID", ResourceType = typeof(resxDbFields))]
        public string AuthByDeputyRepresentativeName { get; set; }

        [Display(Name = "AuthByDeputyRepDate", ResourceType = typeof(resxDbFields))]
        public DateTime? AuthByDeputyRepDate { get; set; }

        [Display(Name = "AuthByRepresentativeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? AuthByRepresentativeGUID { get; set; }

        [Display(Name = "AuthByRepresentativeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? AuthByRepresentativeName { get; set; }

        [Display(Name = "AuthByRepDate", ResourceType = typeof(resxDbFields))]
        public DateTime? AuthByRepDate { get; set; }

        [Display(Name = "ReasonOfCancellation", ResourceType = typeof(resxDbFields))]
        public string ReasonOfCancellation { get; set; }

        [Display(Name = "CaneledDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CaneledDate { get; set; }
        [Display(Name = "CanceledByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? CanceledByGUID { get; set; }
        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public Guid? LastFlowStatusGUID { get; set; }

        [Display(Name = "LastFlowStatus", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatus { get; set; }




        public byte[] dataMissionRequestRowVersion { get; set; }

        public bool Active { get; set; }

    }
    #endregion

    #region Mission Request Iternty
    public class MissionRequestItineraryDataTableModel
    {
        [Display(Name = "MissionRequestItineraryGUID", ResourceType = typeof(resxDbFields))]
        public string MissionRequestItineraryGUID { get; set; }

        [Display(Name = "MissionRequestGUID", ResourceType = typeof(resxDbFields))]
        public string MissionRequestGUID { get; set; }

        [Display(Name = "TravelDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        public DateTime? TravelDate { get; set; }
        [Display(Name = "FromLocationGUID", ResourceType = typeof(resxDbFields))]
        public string FromLocationGUID { get; set; }

        [Display(Name = "ToLocationGUID", ResourceType = typeof(resxDbFields))]
        public string ToLocationGUID { get; set; }

        [Display(Name = "FromLocationName", ResourceType = typeof(resxDbFields))]
        public string FromLocationName { get; set; }

        [Display(Name = "ToLocationName", ResourceType = typeof(resxDbFields))]
        public string ToLocationName { get; set; }



        [Display(Name = "TravelModeGUID", ResourceType = typeof(resxDbFields))]


        public string TravelModeGUID { get; set; }

        [Display(Name = "IsPrivate", ResourceType = typeof(resxDbFields))]
        public bool? IsPrivate { get; set; }

        [Display(Name = "AccommodationProvidedGUID", ResourceType = typeof(resxDbFields))]
        public string AccommodationProvidedGUID { get; set; }

        [Display(Name = "MealsProvided", ResourceType = typeof(resxDbFields))]
        public string MealsProvidedGUID { get; set; }

        [Display(Name = "MealsProvided", ResourceType = typeof(resxDbFields))]
        public string MealsProvidedName { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Display(Name = "ItineraryType", ResourceType = typeof(resxDbFields))]
        public string ItineraryType { get; set; }
        public byte[] dataMissionRequestItineraryRowVersion { get; set; }
        public bool Active { get; set; }
    }

    public class MissionRequestItineraryUpdateModel
    {
        [Display(Name = "MissionRequestItineraryGUID", ResourceType = typeof(resxDbFields))]
        public Guid MissionRequestItineraryGUID { get; set; }

        [Display(Name = "MissionRequestGUID", ResourceType = typeof(resxDbFields))]
        public Guid? MissionRequestGUID { get; set; }



        public bool? IsMissionOwner { get; set; }

        [Display(Name = "ItineraryTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ItineraryTypeGUID { get; set; }

        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public Guid? StaffGUID { get; set; }

        [Display(Name = "TravelDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [Required]
        public DateTime? TravelDate { get; set; }
        [Display(Name = "ReturnDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        public DateTime? ReturnDate { get; set; }
        [Display(Name = "FromLocationGUID", ResourceType = typeof(resxDbFields))]
        [Required]
        public Guid? FromLocationGUID { get; set; }

        [Display(Name = "ToLocationGUID", ResourceType = typeof(resxDbFields))]
        [Required]
        public Guid ToLocationGUID { get; set; }

        [Display(Name = "TravelTypeGUID", ResourceType = typeof(resxDbFields))]
        [Required]
        public Guid? TravelTypeGUID { get; set; }

        [Display(Name = "FromLocationName", ResourceType = typeof(resxDbFields))]
        public string FromLocationName { get; set; }

        [Display(Name = "ToLocationName", ResourceType = typeof(resxDbFields))]
        public string ToLocationName { get; set; }

        [Display(Name = "TravelModeGUID", ResourceType = typeof(resxDbFields))]
        [Required]

        public Guid TravelModeGUID { get; set; }

        [Display(Name = "IsTransportaionArrangedPrivately", ResourceType = typeof(resxDbFields))]
        public bool IsPrivate { get; set; }

        [Display(Name = "AccommodationProvidedGUID", ResourceType = typeof(resxDbFields))]
        public Guid? AccommodationProvidedGUID { get; set; }

        [Display(Name = "MealsProvided", ResourceType = typeof(resxDbFields))]
        public Guid? MealsProvidedGUID { get; set; }

        [Display(Name = "MealsProvided", ResourceType = typeof(resxDbFields))]
        public string MealsProvidedName { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }


        public byte[] dataMissionRequestItineraryRowVersion { get; set; }
        public bool Active { get; set; }
    }
    #endregion

    #region Mission Request Traveler
    public class MissionRequestTravelerDataTableModel
    {
        [Display(Name = "MissionRequestTravelerGUID", ResourceType = typeof(resxDbFields))]
        public string MissionRequestTravelerGUID { get; set; }

        [Display(Name = "MissionRequestGUID", ResourceType = typeof(resxDbFields))]
        public string MissionRequestGUID { get; set; }

        [Display(Name = "TraverlerTypeGUID", ResourceType = typeof(resxDbFields))]
        //[DataType(DataType.Date)]
        public string TraverlerTypeGUID { get; set; }

        [Display(Name = "TraverlerTypeGUID", ResourceType = typeof(resxDbFields))]
        //[DataType(DataType.Date)]
        public string TravelerType { get; set; }
        [Display(Name = "TravelerName", ResourceType = typeof(resxDbFields))]
        //[DataType(DataType.Date)]
        public string TravelerName { get; set; }
        [Display(Name = "FamilyMemberName", ResourceType = typeof(resxDbFields))]
        public string FamilyMemberName { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "DateOfBirth", ResourceType = typeof(resxDbFields))]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "GenderGUID", ResourceType = typeof(resxDbFields))]
        public string GenderGUID { get; set; }

        [Display(Name = "RelationGUID", ResourceType = typeof(resxDbFields))]
        public string RelationGUID { get; set; }

        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]


        public string StaffGUID { get; set; }

        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }

        [Display(Name = "JobTitle", ResourceType = typeof(resxDbFields))]
        public string JobTitle { get; set; }

        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
        public string DepartmentGUID { get; set; }

        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
        public string Department { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public string DutyStationGUID { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public string DutyStation { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }


        public byte[] dataMissionRequestTravelerRowVersion { get; set; }
        public bool Active { get; set; }
    }

    public class MissionRequestTravelerUpdateModel
    {
        [Display(Name = "MissionRequestTravelerGUID", ResourceType = typeof(resxDbFields))]
        public Guid MissionRequestTravelerGUID { get; set; }

        public bool? IsMissionOwner { get; set; }
        public Guid? MissionOwnerGenderGUID { get; set; }
        public Guid? MissionOwnerDutyStationGUID { get; set; }

        [Display(Name = "MissionRequestGUID", ResourceType = typeof(resxDbFields))]
        public Guid? MissionRequestGUID { get; set; }

        [Display(Name = "TraverlerTypeGUID", ResourceType = typeof(resxDbFields))]
        //[DataType(DataType.Date)]
        public Guid? TraverlerTypeGUID { get; set; }
        [Display(Name = "FamilyMemberName", ResourceType = typeof(resxDbFields))]
        public string FamilyMemberName { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "DateOfBirth", ResourceType = typeof(resxDbFields))]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "GenderGUID", ResourceType = typeof(resxDbFields))]
        public Guid? GenderGUID { get; set; }

        [Display(Name = "RelationGUID", ResourceType = typeof(resxDbFields))]
        public Guid? RelationGUID { get; set; }

        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]


        public Guid? StaffGUID { get; set; }

        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }

        [Display(Name = "JobTitle", ResourceType = typeof(resxDbFields))]
        public string JobTitle { get; set; }

        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
        public Guid? DepartmentGUID { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Guid? DutyStationGUID { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }


        public byte[] dataMissionRequestTravelerRowVersion { get; set; }
        public bool Active { get; set; }
    }
    #endregion
    #region Mission Documents
    public class MissionRequestDocumentDataTableModel
    {

        [Display(Name = "MissionRequestDocumentGUID", ResourceType = typeof(resxDbFields))]
        public string MissionRequestDocumentGUID { get; set; }


        [Display(Name = "MissionRequestGUID", ResourceType = typeof(resxDbFields))]
        public string MissionRequestGUID { get; set; }


        [Display(Name = "DocumentTypeGUID", ResourceType = typeof(resxDbFields))]
        public string DocumentTypeGUID { get; set; }


        [Display(Name = "DocumentTypeGUID", ResourceType = typeof(resxDbFields))]
        public string DocumentType { get; set; }


        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public string CreatedByGUID { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public string CreatedBy { get; set; }
        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]

        public DateTime? CreateDate { get; set; }


        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataMissionRequestDocumentRowVersion { get; set; }
    }


    public class MissionRequestDocumentUpdateModel
    {
        [Display(Name = "MissionRequestDocumentGUID", ResourceType = typeof(resxDbFields))]
        public Guid MissionRequestDocumentGUID { get; set; }


        [Display(Name = "MissionRequestGUID", ResourceType = typeof(resxDbFields))]
        public Guid? MissionRequestGUID { get; set; }
        public bool? IsMissionOwner { get; set; }

        [Display(Name = "DocumentTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? DocumentTypeGUID { get; set; }


        [Display(Name = "DocumentTypeGUID", ResourceType = typeof(resxDbFields))]
        public string DocumentType { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public Guid? CreatedByGUID { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public string CreatedBy { get; set; }
        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreateDate { get; set; }


        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
        public string CreateByGUID { get; set; }


        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataMissionRequestDocumentRowVersion { get; set; }
    }
    #endregion
    #endregion

    #region Entitlements Init Calc
    public class EntitlementsInitCalacuationsDataTableModel
    {

        [Display(Name = "EntitlementTypePerDutyStationGUID", ResourceType = typeof(resxDbFields))]
        public string EntitlementTypePerDutyStationGUID { get; set; }


        [Display(Name = "EntitlementTypeGUID", ResourceType = typeof(resxDbFields))]
        public string EntitlementTypeGUID { get; set; }

        [Display(Name = "EntitlementTypeGUID", ResourceType = typeof(resxDbFields))]
        public string EntitlementType { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public string DutyStationGUID { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public string DutyStation { get; set; }

        [Display(Name = "IsCalculatedPerDay", ResourceType = typeof(resxDbFields))]
        public bool? IsCalculatedPerDay { get; set; }


        [Display(Name = "EntitlementValue", ResourceType = typeof(resxDbFields))]
        public decimal? EntitlementValue { get; set; }


        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public string CreatedByGUID { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public string CreatedBy { get; set; }
        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]

        public DateTime? CreateDate { get; set; }



        [Display(Name = "UpdateByGUID", ResourceType = typeof(resxDbFields))]
        public string UpdateByGUID { get; set; }

        [Display(Name = "UpdateBy", ResourceType = typeof(resxDbFields))]
        public string UpdateBy { get; set; }
        [Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]

        public DateTime? UpdateDate { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] codeAHDEntitlementTypePerDutyStationRowVersion { get; set; }
    }


    public class EntitlementsInitCalacuationsUpdateModel
    {

        [Display(Name = "EntitlementTypePerDutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Guid EntitlementTypePerDutyStationGUID { get; set; }


        [Display(Name = "EntitlementTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? EntitlementTypeGUID { get; set; }

        [Display(Name = "EntitlementTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? EntitlementType { get; set; }


        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Guid? DutyStationGUID { get; set; }

        [Display(Name = "IsCalculatedPerDay", ResourceType = typeof(resxDbFields))]
        public bool IsCalculatedPerDay { get; set; }


        [Display(Name = "EntitlementValue", ResourceType = typeof(resxDbFields))]
        public decimal? EntitlementValue { get; set; }


        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public Guid? CreatedByGUID { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public string CreatedBy { get; set; }
        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]

        public DateTime? CreateDate { get; set; }



        [Display(Name = "UpdateByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? UpdateByGUID { get; set; }

        [Display(Name = "UpdateBy", ResourceType = typeof(resxDbFields))]
        public string UpdateBy { get; set; }
        [Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]

        public DateTime? UpdateDate { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] codeAHDEntitlementTypePerDutyStationRowVersion { get; set; }
    }
    #endregion

    #region Staff Check List Separtion

    public class StaffSeparationChecklistDataTableModel
    {

        [Display(Name = "StaffSeparationChecklistGUID", ResourceType = typeof(resxDbFields))]
        public string StaffSeparationChecklistGUID { get; set; }


        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public string StaffGUID { get; set; }
        [Display(Name = "FullName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public string DutyStationGUID { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public string DutyStation { get; set; }

        [Display(Name = "DepartureDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? DepartureDate { get; set; }

        [Display(Name = "ContractStartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public string ContractStartDate { get; set; }


        [Display(Name = "LastFlowStatusGUID", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusGUID { get; set; }


        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatus { get; set; }


        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public string CreateByGUID { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public string CreateBy { get; set; }
        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]

        public DateTime? CreateDate { get; set; }



        [Display(Name = "UpdateByGUID", ResourceType = typeof(resxDbFields))]
        public string UpdateByGUID { get; set; }

        [Display(Name = "UpdateBy", ResourceType = typeof(resxDbFields))]
        public string UpdateBy { get; set; }
        [Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]

        public DateTime? UpdateDate { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataStaffSeparationChecklistRowVersion { get; set; }
    }


    public class StaffSeparationChecklistUpdateModel
    {
        [Display(Name = "StaffSeparationChecklistGUID", ResourceType = typeof(resxDbFields))]
        public Guid StaffSeparationChecklistGUID { get; set; }


        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public Guid? StaffGUID { get; set; }
        [Display(Name = "FullName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }


        [Display(Name = "ContractStartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ContractStartDate { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Guid? DutyStationGUID { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public string DutyStation { get; set; }

        [Display(Name = "DepartureDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? DepartureDate { get; set; }


        [Display(Name = "LastFlowStatusGUID", ResourceType = typeof(resxDbFields))]
        public Guid? LastFlowStatusGUID { get; set; }


        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatus { get; set; }


        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public Guid? CreateByGUID { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public string CreatedBy { get; set; }
        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]

        public DateTime? CreateDate { get; set; }

        [Display(Name = "UpdateByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? UpdateByGUID { get; set; }

        [Display(Name = "UpdateBy", ResourceType = typeof(resxDbFields))]
        public string UpdateBy { get; set; }

        [Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? UpdateDate { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataStaffSeparationChecklistRowVersion { get; set; }
    }

    public class StaffSeparationChecklistDetailDataTableModel
    {

        [Display(Name = "StaffSeparationChecklistDetailGUID", ResourceType = typeof(resxDbFields))]
        public string StaffSeparationChecklistDetailGUID { get; set; }


        [Display(Name = "StaffGUID", ResourceType = typeof(resxDbFields))]
        public string StaffGUID { get; set; }
        [Display(Name = "FullName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }

        [Display(Name = "StaffSeparationChecklistGUID", ResourceType = typeof(resxDbFields))]
        public string StaffSeparationChecklistGUID { get; set; }


        [Display(Name = "SeparationChecklistTypeGUID", ResourceType = typeof(resxDbFields))]
        public string SeparationChecklistTypeGUID { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public string DutyStation { get; set; }

        /*        [Display(Name = "DepartureDate", ResourceType = typeof(resxDbFields))]
                [DataType(DataType.Date)]
                [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
                public DateTime? DepartureDate { get; set; }

                [Display(Name = "ContractStartDate", ResourceType = typeof(resxDbFields))]
                [DataType(DataType.Date)]
                [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
                public string ContractStartDate { get; set; }*/


        [Display(Name = "LastFlowStatusGUID", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusGUID { get; set; }


        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatus { get; set; }


        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public string CreateByGUID { get; set; }

        [Display(Name = "FocalPointComments", ResourceType = typeof(resxDbFields))]
        public string FocalPointComments { get; set; }

        [Display(Name = "StaffComments", ResourceType = typeof(resxDbFields))]
        public string StaffComments { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]

        [Display(Name = "FocalPointUpdateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? FocalPointUpdateDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]

        [Display(Name = "StaffUpdateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? StaffUpdateDate { get; set; }

        [Display(Name = "CreateBy", ResourceType = typeof(resxDbFields))]
        public string CreateBy { get; set; }
        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]

        public DateTime? CreateDate { get; set; }



        [Display(Name = "UpdateByGUID", ResourceType = typeof(resxDbFields))]
        public string UpdateByGUID { get; set; }

        [Display(Name = "UpdateBy", ResourceType = typeof(resxDbFields))]
        public string UpdateBy { get; set; }
        [Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]

        public DateTime? UpdateDate { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Display(Name = "Type", ResourceType = typeof(resxDbFields))]
        public string CheckListType { get; set; }
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataStaffSeparationChecklistDetailRowVersion { get; set; }
    }

    public class StaffSeparationChecklistDetailUpdateModel
    {
        [Display(Name = "StaffSeparationChecklistDetailGUID", ResourceType = typeof(resxDbFields))]
        public Guid StaffSeparationChecklistDetailGUID { get; set; }


        [Display(Name = "StaffSeparationChecklistGUID", ResourceType = typeof(resxDbFields))]
        public Guid? StaffSeparationChecklistGUID { get; set; }

        [Display(Name = "SeparationChecklistTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? SeparationChecklistTypeGUID { get; set; }

        [Display(Name = "Type", ResourceType = typeof(resxDbFields))]
        public string SeparationChecklistType { get; set; }

        [Display(Name = "LastFlowStatusGUID", ResourceType = typeof(resxDbFields))]
        public Guid? LastFlowStatusGUID { get; set; }


        public int? AccessLevel { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreateDate { get; set; }





        [Display(Name = "FlowStatus", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatus { get; set; }


        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public Guid? CreateByGUID { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public string CreatedBy { get; set; }


        [Display(Name = "UpdateByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? UpdateByGUID { get; set; }

        [Display(Name = "UpdateBy", ResourceType = typeof(resxDbFields))]
        public string UpdateBy { get; set; }

        [Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? UpdateDate { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Display(Name = "FocalPointComments", ResourceType = typeof(resxDbFields))]
        public string FocalPointComments { get; set; }

        [Display(Name = "StaffComments", ResourceType = typeof(resxDbFields))]
        public string StaffComments { get; set; }



        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataStaffSeparationChecklistDetailRowVersion { get; set; }
    }








    public class ChecklistTypeFocalPointDataTableModel
    {

        [Display(Name = "SeparationChecklistTypeGUID", ResourceType = typeof(resxDbFields))]
        public string SeparationChecklistTypeGUID { get; set; }


        [Display(Name = "Category", ResourceType = typeof(resxDbFields))]
        public string ChecklistCategoryGUID { get; set; }

        [Display(Name = "Category", ResourceType = typeof(resxDbFields))]
        public string ChecklistCategory { get; set; }
        [Display(Name = "Department", ResourceType = typeof(resxDbFields))]
        public string Department { get; set; }

        [Display(Name = "Name", ResourceType = typeof(resxDbFields))]
        public string Name { get; set; }

        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
        public string DepartmentGUID { get; set; }

        [Display(Name = "StaffSeparationChecklistGUID", ResourceType = typeof(resxDbFields))]
        public string StaffSeparationChecklistGUID { get; set; }




        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public string DutyStation { get; set; }


        [Display(Name = "FocalPoint", ResourceType = typeof(resxDbFields))]
        public string FocalPoint { get; set; }


        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatus { get; set; }



        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] codeAHDSeparationChecklistTypeRowVersion { get; set; }
    }

    #endregion
    #region Outgoing checklist type and focal point

    public class SeparationChecklistTypeDataTable
    {
        [Display(Name = "SeparationChecklistTypeGUID", ResourceType = typeof(resxDbFields))]
        public string SeparationChecklistTypeGUID { get; set; }


        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public string OrganizationInstanceGUID { get; set; }

        [Display(Name = "ChecklistCategoryGUID", ResourceType = typeof(resxDbFields))]
        public string ChecklistCategoryGUID { get; set; }

        [Display(Name = "Category", ResourceType = typeof(resxDbFields))]
        public string CategoryName { get; set; }

        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
        public string DepartmentGUID { get; set; }



        [Display(Name = "Name", ResourceType = typeof(resxDbFields))]
        public string Name { get; set; }

        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] codeAHDSeparationChecklistTypeRowVersion { get; set; }

    }
    public class SeparationChecklistTypeModel
    {
        [Display(Name = "SeparationChecklistTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid SeparationChecklistTypeGUID { get; set; }


        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public Guid? OrganizationInstanceGUID { get; set; }

        [Display(Name = "ChecklistCategoryGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ChecklistCategoryGUID { get; set; }

        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
        public Guid? DepartmentGUID { get; set; }

        [Display(Name = "CheckListName", ResourceType = typeof(resxDbFields))]
        public string Name { get; set; }

        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] codeAHDSeparationChecklistTypeRowVersion { get; set; }

    }
    public class SeparationChecklistFocalPointDataTable
    {
        [Display(Name = "SeparationChecklistFocalPointGUID", ResourceType = typeof(resxDbFields))]
        public string SeparationChecklistFocalPointGUID { get; set; }


        [Display(Name = "SeparationChecklistTypeGUID", ResourceType = typeof(resxDbFields))]
        public string SeparationChecklistTypeGUID { get; set; }

        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
        public string UserGUID { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public string DutyStationGUID { get; set; }

        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }

        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] codeAHDSeparationChecklistFocalPointRowVersion { get; set; }

    }

    public class SeparationChecklistFocalPointModel
    {
        [Display(Name = "SeparationChecklistFocalPointGUID", ResourceType = typeof(resxDbFields))]
        public Guid SeparationChecklistFocalPointGUID { get; set; }


        [Display(Name = "SeparationChecklistTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? SeparationChecklistTypeGUID { get; set; }

        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
        public Guid? UserGUID { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Guid? DutyStationGUID { get; set; }

        //[Display(Name = "Name", ResourceType = typeof(resxDbFields))]
        //public string Name { get; set; }

        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] codeAHDSeparationChecklistFocalPointRowVersion { get; set; }

    }


    #endregion

    #region Entitelment Document
    public class EntitlementDocumentDataTableModel
    {

        [Display(Name = "InternationalStaffEntitlementDocumentGUID", ResourceType = typeof(resxDbFields))]
        public string InternationalStaffEntitlementDocumentGUID { get; set; }


        [Display(Name = "InternationalStaffEntitlementGUID", ResourceType = typeof(resxDbFields))]
        public string InternationalStaffEntitlementGUID { get; set; }


        [Display(Name = "DocumentTypeGUID", ResourceType = typeof(resxDbFields))]
        public string DocumentTypeGUID { get; set; }


        [Display(Name = "DocumentTypeGUID", ResourceType = typeof(resxDbFields))]
        public string DocumentType { get; set; }


        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public string CreatedByGUID { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public string CreatedBy { get; set; }
        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]

        public DateTime? CreateDate { get; set; }


        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataAHDInternationalStaffEntitlementDocumentRowVersion { get; set; }
    }


    public class EntitlementDocumentUpdateModel
    {
        [Display(Name = "InternationalStaffEntitlementDocumentGUID", ResourceType = typeof(resxDbFields))]
        public Guid InternationalStaffEntitlementDocumentGUID { get; set; }


        [Display(Name = "InternationalStaffEntitlementGUID", ResourceType = typeof(resxDbFields))]
        public Guid? InternationalStaffEntitlementGUID { get; set; }


        [Display(Name = "DocumentTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? DocumentTypeGUID { get; set; }


        [Display(Name = "DocumentTypeGUID", ResourceType = typeof(resxDbFields))]
        public string DocumentType { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public Guid? CreatedByGUID { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public string CreatedBy { get; set; }
        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreateDate { get; set; }


        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
        public string CreateByGUID { get; set; }


        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataAHDInternationalStaffEntitlementDocumentRowVersion { get; set; }
    }
    #endregion
    #region ORG Chart
    public class StaffOrgChart
    {
        public StaffOrgChart()
        {
            tags = new List<string>();

        }
        public string id { get; set; }
        public string pid { get; set; }
        public List<string> tags { get; set; }

        public string stpid { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string visa { get; set; }
        public string img { get; set; }
        public string gender { get; set; }
        public string ContractStartDate { get; set; }
        public string ContractEndDate { get; set; }
        public string EmpContractType { get; set; }
        public string EmpReqType { get; set; }
        public string nationality { get; set; }
        public string Department { get; set; }


    }
    #endregion
}
