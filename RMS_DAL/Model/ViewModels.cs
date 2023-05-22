using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS_DAL.Model
{
    public class RMSReportParametersList
    {

        public int Report { get; set; }

        [Display(Name = "PrinterConfigurationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid PrinterConfigurationGUID { get; set; }

        [Display(Name = "OidGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid[] OidGUID { get; set; }


        [DataType(DataType.Date)]
        [Display(Name = "LogDateTimeStart", ResourceType = typeof(resxDbFields))]
        public System.DateTime? LogDateTimeStart { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "LogDateTimeEnd", ResourceType = typeof(resxDbFields))]
        public System.DateTime? LogDateTimeEnd { get; set; }

    }
    public class PrintersDataTableModel
    {
        public Guid PrinterConfigurationGUID { get; set; }
        public Guid PrinterModelGUID { get; set; }
        public string PrinterModel { get; set; }
        public Guid PrinterTypeGUID { get; set; }
        public string PrinterType { get; set; }
        public string PrinterName { get; set; }
        public string IPAddress { get; set; }
        public Guid OrganizationInstanceGUID { get; set; }
        public Guid DutyStationGUID { get; set; }
        public string DutyStationDescription { get; set; }
        public string FloorNumber { get; set; }
        public string OfficeNumber { get; set; }
        public bool Active { get; set; }
        public byte[] dataPrinterConfigurationRowVersion { get; set; }
    }
    public class PrintersConfigUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PrinterConfigurationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid PrinterConfigurationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PrinterName", ResourceType = typeof(resxDbFields))]
        public string PrinterName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PrinterModelGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid PrinterModelGUID { get; set; }

        [Display(Name = "PrinterTypeGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> PrinterTypeGUID { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(20, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "IPAddress", ResourceType = typeof(resxDbFields))]
        public string IPAddress { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid OrganizationInstanceGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid DutyStationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Building", ResourceType = typeof(resxDbFields))]
        public Guid? BuildingGUID { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OfficeNumber", ResourceType = typeof(resxDbFields))]
        public string OfficeNumber { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FloorNumber", ResourceType = typeof(resxDbFields))]
        public string FloorNumber { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataPrinterConfigurationRowVersion { get; set; }

    }

    public class OidsDataTableModel
    {

        public System.Guid OIDGUID { get; set; }
        public string PrinterModelGUID { get; set; }
        public string PrinterModelDescription { get; set; }
        public string PrinterTypeGUID { get; set; }
        public string PrinterTypeDescription { get; set; }
        public string OIDTypeGUID { get; set; }
        public string OIDTypeDescription { get; set; }
        public string OIDDescription { get; set; }
        public string OIDNumber { get; set; }
        public string ValueType { get; set; }
       
        public bool Active { get; set; }
        public byte[] codeOIDRowVersion { get; set; }
    }

    public class OidUpdateModel
    {

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OidGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid OidGUID { get; set; }

        [Display(Name = "PrinterModelGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> PrinterModelGUID { get; set; }

        [Display(Name = "PrinterTypeGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> PrinterTypeGUID { get; set; }

        [Display(Name = "OIDTypeGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> OIDTypeGUID { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OIDDescription", ResourceType = typeof(resxDbFields))]
        public string OIDDescription { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OIDNumber", ResourceType = typeof(resxDbFields))]
        public string OIDNumber { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ValueType", ResourceType = typeof(resxDbFields))]
        public string ValueType { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "IsImport", ResourceType = typeof(resxDbFields))]
        public bool IsImport { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] codeOIDRowVersion { get; set; }
    }

    //public class PrinterManagemetn
    //10.240.231.253 //b&w // HP 402dn
    //10.240.230.249 //colo //richo
    //10.240.231.237 // HP Color LaserJet M553  // color
    //10.240.230.215 //RICOH MP 5054 //B&W
}
