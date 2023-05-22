using Portal_BL.Library;
using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppsPortal.SHM.ViewModels
{

    public partial class dataShuttleStaffUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ShuttleStaffGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ShuttleStaffGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ShuttleVehicleGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ShuttleVehicleGUID { get; set; }

        [Display(Name = "UserPassengerGUID", ResourceType = typeof(resxDbFields))]
        public Guid[] UserPassengerGUID { get; set; }

        [Display(Name = "ShuttleRequestGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ShuttleRequestGUID { get; set; }

        [Display(Name = "ShuttleTravelPurposeGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ShuttleTravelPurposeGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "IsUNAgencyStaff", ResourceType = typeof(resxDbFields))]
        public bool IsUNAgencyStaff { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "UNAgencyStaffName", ResourceType = typeof(resxDbFields))]
        public string UNAgencyStaffName { get; set; }

        [DataType(DataType.EmailAddress)]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "UNAgencyEmailAddress", ResourceType = typeof(resxDbFields))]
        public string UNAgencyEmailAddress { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "UNAgencyPhoneNumber", ResourceType = typeof(resxDbFields))]
        public string UNAgencyPhoneNumber { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Confirmed", ResourceType = typeof(resxDbFields))]
        public bool Confirmed { get; set; }

        [Display(Name = "DeparturePointGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DeparturePointGUID { get; set; }

        [Display(Name = "DropOffPointGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DropOffPointGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataShuttleStaffRowVersion { get; set; }

    }
    public class ShuttleRequestRoutesDataTableModel
    {
        public System.Guid ShuttleRequestRouteGUID { get; set; }
        public System.Guid StartLocationGUID { get; set; }
        public System.Guid EndLocationGUID { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public bool Active { get; set; }
        public byte[] dataShuttleRequestRouteRowVersion { get; set; }
    }
    public class ShuttleRequestRouteUpdateModel
    {
        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public Guid CountryGUID { get; set; }

        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public Guid CountryGUID1 { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ShuttleRequestRouteGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ShuttleRequestRouteGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "StartLocationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid StartLocationGUID1 { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EndLocationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid EndLocationGUID1 { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataShuttleRequestRouteRowVersion { get; set; }
    }
    public class ShuttleRequestStaffDataTable
    {
        public System.Guid ShuttleRequestStaffGUID { get; set; }
        public Nullable<System.Guid> ShuttleRequestGUID { get; set; }
        public string StaffName { get; set; }
        public System.Guid? ReferralStatusGUID { get; set; }
        public string ReferralStatusDescription { get; set; }
        public bool Active { get; set; }
        public byte[] dataShuttleRequestStaffRowVersion { get; set; }

    }
    public class SHMReportParameters
    {
        [Display(Name = "Report", ResourceType = typeof(resxDbFields))]
        public string Report { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "EndDate", ResourceType = typeof(resxDbFields))]
        public DateTime? EndDate { get; set; }

        //
        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public Guid[] OrganizationInstanceGUID { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Guid[] DutyStationGUID { get; set; }

        [Display(Name = "UserPassengerGUID", ResourceType = typeof(resxDbFields))]
        public Guid[] UserPassengerGUID { get; set; }

        [Display(Name = "UserDriverGUID", ResourceType = typeof(resxDbFields))]
        public Guid[] UserDriverGUID { get; set; }

        [Display(Name = "VehicleGUID", ResourceType = typeof(resxDbFields))]
        public Guid[]  VehicleGUID { get; set; }

        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public Guid[] CountryGUID { get; set; }

        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public Guid[] CountryGUID1 { get; set; }

        [Display(Name = "StartLocationGUID", ResourceType = typeof(resxDbFields))]
        public Guid[] StartLocationGUID { get; set; }

        [Display(Name = "EndLocationGUID", ResourceType = typeof(resxDbFields))]
        public Guid[] EndLocationGUID { get; set; }

        [Display(Name = "IncludeZero", ResourceType = typeof(resxDbFields))]
        public bool IncludeZero { get; set; }

    }

    public class SHMReportParametersSTR
    {
        public Guid[] CountryGUID1;
        public Guid[] CountryGUID;

        public string DutyStationGUID { get; set; }
        public string UserPassengerGUID { get; set; }
        public string UserDriverGUID { get; set; }
        public string VehicleGUID { get; set; }
        public string StartLocationGUID { get; set; }
        public string EndLocationGUID { get; set; }

    }
    public class ShuttleReport
    {
        public Guid ShuttleGUID { get; set; }
        public DateTime DepartureDate { get; set; }
        public string DepartureTime { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public int Sequence { get; set; }
        public string DeparturePoint { get; set; }
        public string DropOffPoint { get; set; }
        public string PurposeOfTravel { get; set; }
        public string StartLocation { get; set; }
        public string EndtLocation { get; set; } 
        public string SyrianNumber { get; set; }
        public string LebaneseNumber { get; set; }
        public string PlateNumber { get; set; }
    }
    public class ShuttleTravelPurposeUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ShuttleTravelPurposeGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ShuttleTravelPurposeGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Priority", ResourceType = typeof(resxDbFields))]
        public int Priority { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ShuttleTravelPurposeDescription", ResourceType = typeof(resxDbFields))]
        public string ShuttleTravelPurposeDescription { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "WithReturnDate", ResourceType = typeof(resxDbFields))]
        public bool WithReturnDate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] codeShuttleTravelPurposeRowVersion { get; set; }
        public byte[] codeShuttleTravelPurposeLanguageRowVersion { get; set; }

    }
    public class ShuttleTravelPurposesDataTableModel
    {
        public System.Guid ShuttleTravelPurposeGUID { get; set; }
        public int Priority { get; set; }
        public string ShuttleTravelPurposeDescription { get; set; }
        public bool Active { get; set; }
        public byte[] codeShuttleTravelPurposeRowVersion { get; set; }
        public byte[] codeShuttleTravelPurposeLanguageRowVersion { get; set; }

    }
    public class ShuttleRequestUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ShuttleRequestGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ShuttleRequestGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.DateTime)]
        [Display(Name = "DepartureDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime? DepartureDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "ReturnDateTime", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> ReturnDateTime { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ReferralStatusGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ReferralStatusGUID { get; set; }

        /// ///////////////////////////////////////////////////////////
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> CountryGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> CountryGUID1 { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DeparturePointGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DeparturePointGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DropOffPointGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DropOffPointGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "StartLocationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> StartLocationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EndLocationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> EndLocationGUID { get; set; }

        /// <summary>
        /// ----------------------------------------------------------------------------
        /// </summary>
        /// 
        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> CountryReturnGUID { get; set; }

        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> CountryReturnGUID1 { get; set; }
        [Display(Name = "DeparturePointGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DeparturePointReturnGUID { get; set; }

        [Display(Name = "DropOffPointGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DropOffPointReturnGUID { get; set; }

        [Display(Name = "StartLocationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> StartLocationReturnGUID { get; set; }

        [Display(Name = "EndLocationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> EndLocationReturnGUID { get; set; }

        /// <summary>
        ///--------------------------------------------------------------------------------
        /// </summary>

        

       

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid UserGUID { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ShuttleTravelPurposeGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ShuttleTravelPurposeGUID { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "AdminComment", ResourceType = typeof(resxDbFields))]
        public string AdminComment { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "StaffComment", ResourceType = typeof(resxDbFields))]
        public string StaffComment { get; set; }

        [Display(Name = "WithReturnDate", ResourceType = typeof(resxDbFields))]
        public bool WithReturnDate { get; set; }

        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> OrganizationInstanceGUID { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DutyStationGUID { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataShuttleRequestRowVersion { get; set; }
    }
    public class ShuttleRequestsDataTableModel
    {
        public System.Guid ShuttleRequestGUID { get; set; }
        public System.Guid ShuttleRequestBoxStateGUID { get; set; }
        public System.Guid ShuttleGUID { get; set; }
        public int StaffIncluded { get; set; }
        public System.DateTime DepartureDate { get; set; }
        public System.Guid ReferralStatusGUID { get; set; }
        public string ReferralStatusDescription { get; set; }
        
        public Nullable<System.Guid> DeparturePointGUID { get; set; }
        public Nullable<System.Guid> DropOffPointGUID { get; set; }
        public Nullable<System.Guid> StartLocationGUID { get; set; }
        public Nullable<System.Guid> EndLocationGUID { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public System.Guid UserGUID { get; set; }
        public System.Guid ShuttleTravelPurposeGUID { get; set; }
        public string ShuttleTravelPurposeDescription { get; set; }
        public string StaffName { get; set; }
        public bool FileAttached { get; set; }
        public bool Active { get; set; }
        //
        public byte[] dataShuttleRequestRowVersion { get; set; }
    }
    public class VehicleUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "VehicleGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid VehicleGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "VehicleNumber", ResourceType = typeof(resxDbFields))]
        public string VehicleNumber { get; set; }

        [Display(Name = "VehicleTypeGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> VehicleTypeGUID { get; set; }

        [Display(Name = "VechileModelGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> VechileModelGUID { get; set; }

        [Display(Name = "VehileColorGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> VehileColorGUID { get; set; }

        [Display(Name = "ManufacturingYear", ResourceType = typeof(resxDbFields))]
        public Nullable<int> ManufacturingYear { get; set; }

        [Display(Name = "PlateNumber", ResourceType = typeof(resxDbFields))]
        public Nullable<int> PlateNumber { get; set; }

        [StringLength(150, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ChassisNumber", ResourceType = typeof(resxDbFields))]
        public string ChassisNumber { get; set; }

        [StringLength(150, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EngineNumber", ResourceType = typeof(resxDbFields))]
        public string EngineNumber { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "LastRenewalDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> LastRenewalDate { get; set; }

        [NotMapped]
        [Display(Name = "LastRenewalDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> LocalLastRenewalDate { get { return new Portal().LocalTime(this.LastRenewalDate); } }

        [DataType(DataType.Date)]
        [Display(Name = "LicenseExpiryDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> LicenseExpiryDate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Available", ResourceType = typeof(resxDbFields))]
        public bool Available { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Comment", ResourceType = typeof(resxDbFields))]
        public string Comment { get; set; }

        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> OrganizationInstanceGUID { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DutyStationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] codeVehicleRowVersion { get; set; }

    }
    public class VehiclesDataTableModel
    {
        public System.Guid VehicleGUID { get; set; }
        public string VehicleNumber { get; set; }
        public bool Available { get; set; }
        public string Comment { get; set; }
        public string OrganizationInstanceGUID { get; set; }
        public string DutyStationGUID { get; set; }
        public string OrganizationInstanceDescription { get; set; }
        public string DutyStationDescription { get; set; }
        public bool Active { get; set; }
        public byte[] codeVehicleRowVersion { get; set; }
    }
    public class CalendarEvents
    {
        public Guid EventId { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public string EventDescription { get; set; }
        public string Title { get; set; }
        public bool AllDayEvent { get; set; }
        public string ReferralStatus { get; set; }

    }
    public class ShuttleVehicleDataTable
    {
      
        public System.Guid ShuttleVehicleGUID { get; set; }
        public System.Guid ShuttleGUID { get; set; }
        public Guid? UserGUID { get; set; }
        public string DriverName { get; set; }
        public Nullable<System.Guid> VehicleGUID { get; set; }
        public string VehicleNumber { get; set; }

        public bool Active { get; set; }
        public byte[] dataShuttleVehicleRowVersion { get; set; }
    }
    public class ShuttleRouteUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ShuttleRouteGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ShuttleRouteGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ShuttleGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ShuttleGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "StartLocationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid StartLocationGUID { get; set; }

        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid CountryGUID { get; set; }
        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid CountryGUID1 { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EndLocationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid EndLocationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DropOffPointGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DropOffPointGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DeparturePointGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DeparturePointGUID { get; set; }

        [Display(Name = "DefaultRoute", ResourceType = typeof(resxDbFields))]
        public bool DefaultRoute { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataShuttleRouteRowVersion { get; set; }
    }

    public class ShuttleRoutesDataTableModel
    {
        public System.Guid ShuttleRouteGUID { get; set; }
        public System.Guid ShuttleGUID { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }

        public bool Active { get; set; }
        public byte[] dataShuttleRouteRowVersion { get; set; }
        public bool DefaultRoute { get; set; }
    }
    public class ShuttlesDataTableModel
    {
        public System.Guid ShuttleGUID { get; set; }
        public System.DateTime DepartureDateTime { get; set; }
        public Guid DeparturePoint { get; set; }
        public Guid DropOffPoint { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public Nullable<System.Guid> ReferralStatusGUID { get; set; }
        public string ReferralStatusDescription { get; set; }
        public Nullable<System.Guid> ApprovedBy { get; set; }
        public Nullable<System.Guid> OrganizationInstanceGUID { get; set; }
        public Nullable<System.Guid> DutyStationGUID { get; set; }
        public string DutyStationDescription { get; set; }
        public string SharedBy { get; set; }
        public bool Active { get; set; }
        public byte[] dataShuttleRowVersion { get; set; }
    }

    public class ShuttleUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ShuttleGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ShuttleGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.DateTime)]
        [Display(Name = "DepartureDateTime", ResourceType = typeof(resxDbFields))]
        public System.DateTime? DepartureDateTime { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "ReturnDateTime", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> ReturnDateTime { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DeparturePointGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DeparturePointGUID { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DropOffPointGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DropOffPointGUID { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "StartLocationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> StartLocationGUID { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "EndLocationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> EndLocationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> CountryGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CountryGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> CountryGUID1 { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PassByLocations", ResourceType = typeof(resxDbFields))]
        public string PassByLocations { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SharedBy", ResourceType = typeof(resxDbFields))]
        public string SharedBy { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "ShareDatetime", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> ShareDatetime { get; set; }

        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> OrganizationInstanceGUID { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DutyStationGUID { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DeparturePointFreeText", ResourceType = typeof(resxDbFields))]
        public string DeparturePointFreeText { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DropOffPointFreeText", ResourceType = typeof(resxDbFields))]
        public string DropOffPointFreeText { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataShuttleRowVersion { get; set; }
    }

    public class ShuttleStaffDataTable
    {
        public System.Guid ShuttleVehicleGUID { get; set; }
        public System.Guid ShuttleStaffGUID { get; set; }
        public System.Guid ShuttleGUID { get; set; }
        public string UserPassengerGUID { get; set; }
        public Guid? ShuttleTravelPurposeGUID { get; set; }
        public string TravelPurpose { get; set; }
        public string StaffName { get; set; }
        public bool Confirmed { get; set; }
    }
}
