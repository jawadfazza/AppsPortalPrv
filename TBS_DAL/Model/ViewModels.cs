using Portal_BL.Library;
using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace TBS_DAL.Model
{

    public class TelecomCompaniesDataTableModel
    {
        public Guid TelecomCompanyGUID { get; set; }
        public string TelecomCompanyAcronym { get; set; }
        public string TelecomCompanyDescription { get; set; }
        public bool Active { get; set; }
        public byte[] codeTelecomCompanyRowVersion { get; set; }
    }

    public class TelecomCompanyUpdateModel
    {
        public Guid TelecomCompanyGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TelecomCompanyAcronym", ResourceType = typeof(resxDbFields))]
        public string TelecomCompanyAcronym { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TelecomCompanyDescription", ResourceType = typeof(resxDbFields))]
        public string TelecomCompanyDescription { get; set; }
        public bool Active { get; set; }
        public byte[] codeTelecomCompanyRowVersion { get; set; }
        public byte[] codeTelecomCompanyLanguagesRowVersion { get; set; }
    }

    public class TelecomCompanyOperationsDataTableModel
    {
        public Guid TelecomCompanyOperationGUID { get; set; }
        public string TelecomCompanyDescription { get; set; }
        public string OperationDescription { get; set; }
        public bool Active { get; set; }
        public byte[] codeTelecomCompanyOperationRowVersion { get; set; }
        public Guid? TelecomCompanyOperationFileColumnGUID { get; set; }
    }

    public class TelecomCompanyOperationConfigUpdateModel
    {
        public MobileConfigModel MobileConfigModel { get; set; }
        public LandLineConfigModel LandLineConfigModel { get; set; }
    }

    public class TelecomCompanyOperationMobileConfigUpdateModel
    {
        public configTelecomCompanyOperation configTelecomCompanyOperation { get; set; }
        public configTelecomCompanyOperationMobileColumn configTelecomCompanyOperationMobileColumns { get; set; }
    }

    public class TelecomCompanyOperationLandlineConfigUpdateModel
    {
        public configTelecomCompanyOperation configTelecomCompanyOperation { get; set; }
        public configTelecomCompanyOperationLandColumn configTelecomCompanyOperationLandColumns { get; set; }
    }

    public class MobileConfigModel
    {
        public configTelecomCompanyOperation configTelecomCompanyOperation { get; set; }
        public configTelecomCompanyOperationMobileColumn configTelecomCompanyOperationMobileColumns { get; set; }
    }

    public class LandLineConfigModel
    {
        public configTelecomCompanyOperation configTelecomCompanyOperation { get; set; }
        public configTelecomCompanyOperationLandColumn configTelecomCompanyOperationLandColumns { get; set; }
    }

    public class DataBillsDataTableModel
    {
        public Guid BillGUID { get; set; }
        public Guid BillForTypeGUID { get; set; }
        public string BillForTypeDescription { get; set; }
        public DateTime BillPeriodStartDate { get; set; }
        public DateTime UploadDate { get; set; }
        public bool Active { get; set; }
        public bool IsProccessed { get; set; }
        public byte[] dataBillRowVersion { get; set; }
        public string TelecomCompanyDescription { get; set; }
        public string OperationDescription { get; set; }

        public Guid UploadedByGUID { get; set; }
        public string UploadedBy { get; set; }
        public Guid? ProcessedByGUID { get; set; }
        public string ProcessedBy { get; set; }
        public DateTime? ProcessedOn { get; set; }
        public int BillForMonth { get; set; }
        public int BillForYear { get; set; }



    }

    public class DataLandLinsBillsDataTableModel
    {
        public Guid BillGUID { get; set; }
        public Guid BillForTypeGUID { get; set; }
        public string BillForTypeDescription { get; set; }
        public DateTime BillPeriodStartDate { get; set; }
        public DateTime UploadDate { get; set; }
        public bool Active { get; set; }
        public byte[] dataBillRowVersion { get; set; }
        public Guid TelecomCompanyGUID { get; set; }
        public string TelecomCompanyDescription { get; set; }
        public Guid OperationGUID { get; set; }
        public string OperationDescription { get; set; }
        public int BillForMonth { get; set; }
        public int BillForYear { get; set; }
    }

    public class DataBillUpdateModel
    {
        public Guid BillGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TelecomCompanyOperationConfigGUID", ResourceType = typeof(resxDbFields))]
        public Guid TelecomCompanyOperationConfigGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Guid DutyStationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BillForTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid BillForTypeGUID { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BillForMonth", ResourceType = typeof(resxDbFields))]
        public int BillForMonth { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BillForYear", ResourceType = typeof(resxDbFields))]
        public int BillForYear { get; set; }
        public bool Active { get; set; }
        public byte[] dataBillsRowVersion { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ProcessingMethod", ResourceType = typeof(resxDbFields))]
        public string ProcessingMethod { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.DateTime)]
        [Display(Name = "BillDeadLine", ResourceType = typeof(resxDbFields))]
        public DateTime BillDeadLine { get; set; }

        public HttpPostedFileBase BillFile { get; set; }

    }

    public class StaffPhonesDataTableModel
    {
        public Guid StaffSimGUID { get; set; }
        public string StaffName { get; set; }
        public string StaffEmail { get; set; }

        public string PhoneTypeDescription { get; set; }
        public string PhoneTypeUsageDescription { get; set; }

        public string DepartmentDescription { get; set; }
        public string DutyStationDescription { get; set; }
        public string PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }
        public bool Active { get; set; }
        public byte[] dataStaffPhoneRowVersion { get; set; }
    }

    public class StaffPhoneUpdateModel
    {
        [Required]
        [Display(Name = "StaffSimGUID", ResourceType = typeof(resxDbFields))]
        public Guid StaffSimGUID { get; set; }

        [Required]
        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
        public Guid UserGUID { get; set; }

        [Required]
        [Display(Name = "PhoneUsageTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid PhoneUsageTypeGUID { get; set; }

        [StringLength(50)]
        [Display(Name = "PhoneNumber", ResourceType = typeof(resxDbFields))]
        public string PhoneNumber { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "FromDate", ResourceType = typeof(resxDbFields))]
        public DateTime FromDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "ToDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ToDate { get; set; }

        [Display(Name = "PhoneTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid PhoneTypeGUID { get; set; }

        [Display(Name = "TelecomCompanyOperationGUID", ResourceType = typeof(resxDbFields))]
        public Guid TelecomCompanyOperationGUID { get; set; }

        [Display(Name = "Note", ResourceType = typeof(resxDbFields))]
        public string Note { get; set; }

        [Required]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataStaffPhoneRowVersion { get; set; }
    }

    public class UserBillsDataTableModel
    {
        public Guid BillGUID { get; set; }
        public string UserBillGUID { get; set; }
        public Guid BillForTypeGUID { get; set; }
        public string BillForTypeDescription { get; set; }
        public int BillForMonth { get; set; }
        public int BillForYear { get; set; }
        public bool Active { get; set; }
        public Guid UserGUID { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime ConfirmationDate { get; set; }
        public byte[] dataUserBillRowVersion { get; set; }
        //public string CallingNumber { get; set; }
        //public string CalledNumber { get; set; }
        //public DateTime CallTime { get; set; }
        //public Guid CallType { get; set; }
        //public int CallSource { get; set; }
        //public Guid Service { get; set; }
        //public int DurationInSeconds { get; set; }
        //public int DurationInMinutes { get; set; }
        //public double CallCost { get; set; }
        //public bool IsOfficial { get; set; }
        //public bool Active { get; set; }
        //public byte[] dataBillDetailsRowVersion { get; set; }
    }

    public class UserBillsUpdateModel
    {
        public Guid BillGUID { get; set; }
        public Guid UserBillGUID { get; set; }
        public Guid UserGUID { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public bool IsConfirmed { get; set; }
        public bool Active { get; set; }
        public byte[] dataUserBillRowVersion { get; set; }
        public List<dataUserBillDetailModel> dataUserBillDetailModel { get; set; }

        public double TotalCost { get; set; }
        public double TotalMinutes { get; set; }
        public double TotalSeconds { get; set; }
        public double PrivateCost { get; set; }
        public double PrivateMinutes { get; set; }
        public double PrivateSeconds { get; set; }
        public double OfficialCost { get; set; }
        public double OfficialMinutes { get; set; }
        public double OfficialSeconds { get; set; }
    }

    public class dataUserBillDetailModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "UserBillDetailGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid UserBillDetailGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "UserBillGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid UserBillGUID { get; set; }

        [Display(Name = "CallingNumber", ResourceType = typeof(resxDbFields))]
        public string CallingNumber { get; set; }

        [Display(Name = "CalledNumber", ResourceType = typeof(resxDbFields))]
        public string CalledNumber { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.DateTime)]
        [Display(Name = "dateTimeConnect", ResourceType = typeof(resxDbFields))]
        public System.DateTime dateTimeConnect { get; set; }

        [NotMapped]
        [Display(Name = "dateTimeConnect", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> LocaldateTimeConnect { get { return new Portal().LocalTime(this.dateTimeConnect); } }

        [DataType(DataType.DateTime)]
        [Display(Name = "dateTimeDisconnect", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> dateTimeDisconnect { get; set; }

        [NotMapped]
        [Display(Name = "dateTimeDisconnect", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> LocaldateTimeDisconnect { get { return new Portal().LocalTime(this.dateTimeDisconnect); } }

        [Display(Name = "CallType", ResourceType = typeof(resxDbFields))]
        public System.Guid CallType { get; set; }

        [Display(Name = "CallSource", ResourceType = typeof(resxDbFields))]
        public int CallSource { get; set; }

        [Display(Name = "Service", ResourceType = typeof(resxDbFields))]
        public System.Guid Service { get; set; }

        [Display(Name = "DurationInSeconds", ResourceType = typeof(resxDbFields))]
        public int DurationInSeconds { get; set; }

        [Display(Name = "DurationInMinutes", ResourceType = typeof(resxDbFields))]
        public int DurationInMinutes { get; set; }

        [Display(Name = "CallCost", ResourceType = typeof(resxDbFields))]
        public double CallCost { get; set; }

        [Display(Name = "IsPrivate", ResourceType = typeof(resxDbFields))]
        public bool IsPrivate { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }

        [NotMapped]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }

        public byte[] dataUserBillDetailRowVersion { get; set; }


        public string DestinationHolderName { get; set; }
        public string CallTypeDescription { get; set; }
    }

    public class ReportsModel
    {
        public Guid ReportTypeGUID { get; set; }
        public Guid TelecomCompanyGUID { get; set; }
        public Guid SimTypeGUID { get; set; }
        public List<Guid> DepartmentGUID { get; set; }
        public List<Guid> UserGUID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }


    public class CDRFTPLocationDataTableModel
    {
        public Guid CDRLocationGUID { get; set; }
        public Guid DutyStationGUID { get; set; }
        public string DutyStationDescription { get; set; }
        public string FTPPath { get; set; }
        public string FTPUsr { get; set; }
        public string FTPPass { get; set; }
        public byte[] codeCDRLocationRowVersion { get; set; }
    }


    public class ProcessNewCDRModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Guid DutyStationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EmailToCC", ResourceType = typeof(resxDbFields))]
        public string EmailToCC { get; set; }

    }

    public class ProcessLandLineBillsCalculation
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Guid DutyStationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BillForMonth", ResourceType = typeof(resxDbFields))]
        public int BillForMonth { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BillForYear", ResourceType = typeof(resxDbFields))]
        public int BillForYear { get; set; }
    }

    public class ProcessMobileBillsCalculation
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TelecomCompanyOperationConfigGUID", ResourceType = typeof(resxDbFields))]
        public Guid TelecomCompanyOperationConfigGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Guid DutyStationGUID { get; set; }

        [Display(Name = "EmailToCC", ResourceType = typeof(resxDbFields))]
        public string EmailToCC { get; set; }
    }
    public class MyPhoneBillsDataTableModel
    {
        public Guid BillGUID { get; set; }
        public Guid UserBillGUID { get; set; }
        public Guid UserGUID { get; set; }
        public string BillForTypeDescription { get; set; }
        public string TelecomCompanyDescription { get; set; }
        public string OperationDescription { get; set; }
        public string BillForMonth { get; set; }
        public string BillForYear { get; set; }
        public string BillTotalOfficialAmount { get; set; }
        public string BillTotalPrivateAmount { get; set; }
        public bool IsConfirmed { get; set; }
        public string OperationsLanguagesLAN { get; set; }
        public string TelecomCompanyLanguagesLAN { get; set; }
        public bool Active { get; set; }
    }


    public class OwnUserBillConfirmModel
    {
        public long EDMXID { get; set; }
        public Guid UserBillGUID { get; set; }
        public string CallingNumber { get; set; }
        public string CalledNumber { get; set; }
        public string CalledStaffName { get; set; }

        public bool? AutomatedBySystem { get; set; }
        public bool? UserOverride { get; set; }
        public int NumberOfCalls { get; set; }
        public double TotalDurationMinutes { get; set; }
        public double TotalDurationInSeconds { get; set; }
        public double TotalCallCost { get; set; }
        public string LastConfirmation { get; set; }
        public bool IsBillLocked { get; set; }


        public bool IsPrivate { get; set; }
        public bool IsPartiallyConfirmed { get; set; }
        public bool IsEntireBillConfirmed { get; set; }
        public int RecordsConfirmedCount { get; set; }

        public double TotalBillMinutes { get; set; }
        public double TotalBillSeconds { get; set; }
        public double TotalBillCost { get; set; }

        public double TotalBillPrivateMinutes { get; set; }
        public double TotalBillPrivateSeconds { get; set; }
        public double TotalBillPrivateCost { get; set; }

        public double TotalBillOfficialMinutes { get; set; }
        public double TotalBillOfficialSeconds { get; set; }
        public double TotalBillOfficialCost { get; set; }

        public double PayInCashAmount { get; set; }
        public double DeductFromSalaryAmount { get; set; }

        public string Comments { get; set; }
    }

    public class FinanceReportParameterModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ReportTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ReportTypeGUID { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[Display(Name = "BillingDutyStationGUID", ResourceType = typeof(resxDbFields))]
        //public Guid? BillingDutyStationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TelecomCompanyGUID", ResourceType = typeof(resxDbFields))]
        public Guid? TelecomCompanyGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BillForMonth", ResourceType = typeof(resxDbFields))]
        public int? BillForMonth { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BillForYear", ResourceType = typeof(resxDbFields))]
        public int? BillForYear { get; set; }

        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
        public Guid? UserGUID { get; set; }
    }
    public class FinanceReportModel
    {
        public Guid UserGUID { get; set; }
        public Guid UserBillGUID { get; set; }
        public string StaffName { get; set; }
        public string CallingNumber { get; set; }
        public double DeductFromSalaryAmount { get; set; }
        public double PayInCashAmount { get; set; }
        public string SIMWarehouseDescription { get; set; }
        public string TelecomCompanyDescription { get; set; }
        public double CallCost { get; set; }
        public double PrivateCallCost { get; set; }
        public double OfficialCallCost { get; set; }
        public IEnumerable<v_FinanceReport> RecordDetails { get; set; }

    }

    public class PendingBillsDataTableModel
    {
        public string UserGUID { get; set; }
        public string EmailAddress { get; set; }
        public string FullName { get; set; }
        public string DutyStationGUID { get; set; }
        public string DutyStationDescription { get; set; }
        public Guid BillGUID { get; set; }
        public string BillForTypeGUID { get; set; }
        public string BillForTypeDescription { get; set; }
        public string TelecomCompanyDescription { get; set; }
        public string TelecomCompanyGUID { get; set; }
        public Guid UserBillGUID { get; set; }
        public int BillForMonth { get; set; }
        public int BillForYear { get; set; }
        public byte[] dataUserBillRowVersion { get; set; }
    }

    public class PendingBillsToProcessModel
    {
        public Guid BillGUID { get; set; }
        public int BillForMonth { get; set; }
        public int BillForYear { get; set; }
    }

    public class SendReminderBulkModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.DateTime)]
        [Display(Name = "BillDeadLine", ResourceType = typeof(resxDbFields))]
        public DateTime BillDeadLine { get; set; }

        public List<PendingBillsDataTableModel> PendingBills { get; set; }
    }

    #region Same Values In Lookup Tables
    public static class BillTypes
    {

        public readonly static Guid MobileLine = Guid.Parse("B2980068-688D-428D-9E5C-494656BA9D2C");
        public readonly static Guid LandLine = Guid.Parse("9D8A1EB9-C2AC-4D78-95FF-874E46074321");
    }

    public static class MobileCallTypes
    {
        public readonly static Guid MobileLine = Guid.Parse("10000000-0000-0000-0000-000000000001");
        public readonly static Guid LandLine = Guid.Parse("20000000-0000-0000-0000-000000000002");
        public readonly static Guid SMS = Guid.Parse("30000000-0000-0000-0000-000000000003");
        public readonly static Guid International = Guid.Parse("40000000-0000-0000-0000-000000000004");
        public readonly static Guid Roaming = Guid.Parse("50000000-0000-0000-0000-000000000005");
        public readonly static Guid Bundle = Guid.Parse("60000000-0000-0000-0000-000000000006");
        public readonly static Guid RoamingInternet = Guid.Parse("70000000-0000-0000-0000-000000000007");
        public readonly static Guid RoamingSMS = Guid.Parse("80000000-0000-0000-0000-000000000008");
        public readonly static Guid Call = Guid.Parse("90000000-0000-0000-0000-000000000009");
        public readonly static Guid Other = Guid.Parse("10000000-0000-0000-0000-000000000010");
        public readonly static Guid Local = Guid.Parse("11000000-0000-0000-0000-000000000011");
        public readonly static Guid CountryWide = Guid.Parse("12000000-0000-0000-0000-000000000012");
        public readonly static Guid Ring = Guid.Parse("A9688DD0-F62C-443C-8C06-D89E773369F2");
        public readonly static Guid Multimedia = Guid.Parse("622B332C-7094-4555-A6E9-1505C3DE0C58");
        public readonly static Guid GPRS = Guid.Parse("8B7FE79B-11D1-4BC8-8581-1B078EFDAA6B");
        public readonly static Guid Service = Guid.Parse("fbe420b2-b49e-4d28-b301-c2a5ef00835f");
    }

    #endregion
}
