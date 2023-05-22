using Portal_BL.Library;
using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAS_DAL.ViewModels
{

    public class DASReportParametersList
    {
        public int Report { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.Date)]
        [Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime? StartDate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.Date)]
        [Display(Name = "EndDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime? EndDate { get; set; }
    }
    public class HistorysDataTable
    {
        [Display(Name = "ArchiveTemplateDocumentGUID", ResourceType = typeof(resxDbFields))]
        public Guid ArchiveTemplateDocumentGUID { get; set; }

        [Display(Name = "FullName", ResourceType = typeof(resxDbFields))]
        public string FullName { get; set; }

        [Display(Name = "ActionTime", ResourceType = typeof(resxDbFields))]
        public DateTime? ActionTime { get; set; }

        [Display(Name = "Action", ResourceType = typeof(resxDbFields))]
        public string Action { get; set; }
    }
    public class DocumentCabinetShelfUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DocumentCabinetShelfGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid DocumentCabinetShelfGUID { get; set; }

        [Display(Name = "DocumentCabinetGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DocumentCabinetGUID { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ShelfNumber", ResourceType = typeof(resxDbFields))]
        public string ShelfNumber { get; set; }

        [Display(Name = "MaxStorage", ResourceType = typeof(resxDbFields))]
        public Nullable<int> MaxStorage { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] codeDASDocumentCabinetShelfRowVersion { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeletedOn { get; set; }

        [NotMapped]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }

        [Display(Name = "Archived", ResourceType = typeof(resxDbFields))]
        public Nullable<bool> Archived { get; set; }
    }

    public class DocumentCabinetShelfsDataTableModel
    {
        public System.Guid DocumentCabinetShelfGUID { get; set; }

        public Nullable<System.Guid> DocumentCabinetGUID { get; set; }

        public string ShelfNumber { get; set; }
        public Nullable<int> MaxStorage { get; set; }
        public bool Active { get; set; }

        public byte[] codeDASDocumentCabinetShelfRowVersion { get; set; }
        public Nullable<bool> Archived { get; set; }
    }
    public class FileRequestModel
    {
        [Display(Name = "FileGUID", ResourceType = typeof(resxDbFields))]
        public Guid? FileGUID { get; set; }
        [Display(Name = "RequesterTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? RequesterTypeGUID { get; set; }
        [Display(Name = "RequesterGUID", ResourceType = typeof(resxDbFields))]
        public Guid? RequesterGUID { get; set; }
        [Display(Name = "RequestDate", ResourceType = typeof(resxDbFields))]
        public DateTime? RequestDate { get; set; }
        [Display(Name = "RequestDurationDate", ResourceType = typeof(resxDbFields))]
        public DateTime? RequestDurationDate { get; set; }
        public string Comments { get; set; }

    }
    public class FileTrackingReportModel
    {
        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public Guid? LastFlowStatusGUID { get; set; }

        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public Guid? StaffGUID { get; set; }

        public DateTime? TransferFromDate { get; set; }

        public DateTime? TransferToDate { get; set; }

    }
    public class RefugeeScannDocumentDataTableModel
    {
        [Display(Name = "ScannDocumentGUID", ResourceType = typeof(resxDbFields))]
        public string ScannDocumentGUID { get; set; }
        [Display(Name = "FileGUID", ResourceType = typeof(resxDbFields))]
        public string FileGUID { get; set; }

        [Display(Name = "FileNumber", ResourceType = typeof(resxDbFields))]
        public string FileNumber { get; set; }

        [Display(Name = "OrigionCountry", ResourceType = typeof(resxDbFields))]
        public string OrigionCountry { get; set; }


        [Display(Name = "RefugeeStatus", ResourceType = typeof(resxDbFields))]
        public string RefugeeStatus { get; set; }
        [Display(Name = "DocumentCabinetShelfGUID", ResourceType = typeof(resxDbFields))]
        public string DocumentCabinetShelfGUID { get; set; }

        [Display(Name = "IsAvailable", ResourceType = typeof(resxDbFields))]
        public bool? IsAvailable { get; set; }

        [Display(Name = "LastCustodianType", ResourceType = typeof(resxDbFields))]
        public string LastCustodianType { get; set; }

        [Display(Name = "LastCustodianTypeName", ResourceType = typeof(resxDbFields))]
        public string LastCustodianTypeName { get; set; }

        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
        public string CreateByGUID { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreateDate { get; set; }

        public byte[] dataFileRowVersion { get; set; }
        public bool Active { get; set; }

        public byte[] dataScannDocumentRowVersion { get; set; }
    }

    public class FileDataTableModel
    {
        [Display(Name = "FileMergeStatusGUID", ResourceType = typeof(resxDbFields))]
        public string FileMergeStatusGUID { get; set; }

        [Display(Name = "SiteGUID", ResourceType = typeof(resxDbFields))]
        public string SiteGUID { get; set; }

        [Display(Name = "CurrentSiteCode", ResourceType = typeof(resxDbFields))]
        public string SiteCode { get; set; }

        [Display(Name = "RequestDate", ResourceType = typeof(resxDbFields))]
        public DateTime? RequestDate { get; set; }

        [Display(Name = "FileMergeStatusGUID", ResourceType = typeof(resxDbFields))]
        public string FileMergeStatus { get; set; }
        [Display(Name = "OwnedByStaff", ResourceType = typeof(resxDbFields))]
        public bool? OwnedByStaff { get; set; }
        [Display(Name = "DestinationUnitGUID", ResourceType = typeof(resxDbFields))]
        public string LastDestinationUnitGUID { get; set; }

        [Display(Name = "UnitName", ResourceType = typeof(resxDbFields))]
        public string LastUnitName { get; set; }
        [Display(Name = "SiteOwner", ResourceType = typeof(resxDbFields))]
        public string SiteOwner { get; set; }


        [Display(Name = "TransferLocationGUID", ResourceType = typeof(resxDbFields))]
        public string TransferLocationGUID { get; set; }
        [Display(Name = "CaseLocation", ResourceType = typeof(resxDbFields))]
        public string CaseLocation { get; set; }
        [Display(Name = "ProcessStatusName", ResourceType = typeof(resxDbFields))]
        public string ProcessStatusName { get; set; }
        [Display(Name = "CaseSize", ResourceType = typeof(resxDbFields))]
        public int? CaseSize { get; set; }
        [Display(Name = "RefugeeStatus", ResourceType = typeof(resxDbFields))]
        public string RefugeeStatus { get; set; }
        [Display(Name = "OrigionCountry", ResourceType = typeof(resxDbFields))]
        public string OrigionCountry { get; set; }

        [Display(Name = "LastCurrentOwner", ResourceType = typeof(resxDbFields))]
        public string CurrentOwner { get; set; }
        [Display(Name = "UploadUserFileGUID", ResourceType = typeof(resxDbFields))]
        public string UploadUserFileGUID { get; set; }


        [Display(Name = "UserTransferStatus", ResourceType = typeof(resxDbFields))]
        public string UserTransferStatus { get; set; }


        [Display(Name = "LastTransferFromName", ResourceType = typeof(resxDbFields))]
        public string LastTransferFromName { get; set; }

        [Display(Name = "LastTransferFromNameGUID", ResourceType = typeof(resxDbFields))]
        public string LastTransferFromNameGUID { get; set; }

        [Display(Name = "LatestAppointmentDate", ResourceType = typeof(resxDbFields))]
        public DateTime? LatestAppointmentDate { get; set; }

        [Display(Name = "TotalDaysReminigForAppointment", ResourceType = typeof(resxDbFields))]
        public int? TotalDaysReminigForAppointment { get; set; }

        [Display(Name = "LastRequesterName", ResourceType = typeof(resxDbFields))]
        public string LastRequesterName { get; set; }
        [Display(Name = "LastRequesterNameGUID", ResourceType = typeof(resxDbFields))]
        public string LastRequesterNameGUID { get; set; }

        [Display(Name = "RequestStatusName", ResourceType = typeof(resxDbFields))]
        public string RequestStatusName { get; set; }


        [Display(Name = "IsRequested", ResourceType = typeof(resxDbFields))]
        public bool? IsRequested { get; set; }


        [Display(Name = "FileGUID", ResourceType = typeof(resxDbFields))]
        public string FileGUID { get; set; }

        [Display(Name = "FileNumber", ResourceType = typeof(resxDbFields))]
        public string FileNumber { get; set; }


        [Display(Name = "LastCustodianType", ResourceType = typeof(resxDbFields))]
        public string LastCustodianType { get; set; }

        [Display(Name = "LastCustodianTypeName", ResourceType = typeof(resxDbFields))]
        public string LastCustodianTypeName { get; set; }

        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
        public string CreateByGUID { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "LastFlowStatusGUID", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusGUID { get; set; }
        [Display(Name = "LastCustodianTypeGUID", ResourceType = typeof(resxDbFields))]
        public string LastCustodianTypeGUID { get; set; }
        [Display(Name = "LastCustodianTypeNameGUID", ResourceType = typeof(resxDbFields))]
        public string LastCustodianTypeNameGUID { get; set; }

        [Display(Name = "LastFlowStatusName", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusName { get; set; }

        public byte[] dataUploadUserFileRowVersion { get; set; }
        public byte[] dataFileRowVersion { get; set; }
        public bool Active { get; set; }


    }

    public class UserFileDataTableModel
    {
        [Display(Name = "FileMergeStatusGUID", ResourceType = typeof(resxDbFields))]
        public string FileMergeStatusGUID { get; set; }

        [Display(Name = "FileMergeStatusGUID", ResourceType = typeof(resxDbFields))]
        public string FileMergeStatus { get; set; }
        [Display(Name = "DestinationUnitGUID", ResourceType = typeof(resxDbFields))]
        public string LastDestinationUnitGUID { get; set; }

        [Display(Name = "SiteOwner", ResourceType = typeof(resxDbFields))]
        public string SiteOwner { get; set; }

        [Display(Name = "UnitName", ResourceType = typeof(resxDbFields))]
        public string LastUnitName { get; set; }


        [Display(Name = "TransferLocationGUID", ResourceType = typeof(resxDbFields))]
        public string TransferLocationGUID { get; set; }
        [Display(Name = "CaseLocation", ResourceType = typeof(resxDbFields))]
        public string CaseLocation { get; set; }
        [Display(Name = "ProcessStatusName", ResourceType = typeof(resxDbFields))]
        public string ProcessStatusName { get; set; }
        [Display(Name = "CaseSize", ResourceType = typeof(resxDbFields))]
        public int? CaseSize { get; set; }
        [Display(Name = "RefugeeStatus", ResourceType = typeof(resxDbFields))]
        public string RefugeeStatus { get; set; }
        [Display(Name = "OrigionCountry", ResourceType = typeof(resxDbFields))]
        public string OrigionCountry { get; set; }

        [Display(Name = "CurrentOwner", ResourceType = typeof(resxDbFields))]
        public string CurrentOwner { get; set; }
        [Display(Name = "UploadUserFileGUID", ResourceType = typeof(resxDbFields))]
        public string UploadUserFileGUID { get; set; }


        [Display(Name = "UserTransferStatus", ResourceType = typeof(resxDbFields))]
        public string UserTransferStatus { get; set; }


        [Display(Name = "LastTransferFromName", ResourceType = typeof(resxDbFields))]
        public string LastTransferFromName { get; set; }

        [Display(Name = "LastTransferFromNameGUID", ResourceType = typeof(resxDbFields))]
        public string LastTransferFromNameGUID { get; set; }

        [Display(Name = "LatestAppointmentDate", ResourceType = typeof(resxDbFields))]
        public DateTime? LatestAppointmentDate { get; set; }

        [Display(Name = "TotalDaysReminigForAppointment", ResourceType = typeof(resxDbFields))]
        public int? TotalDaysReminigForAppointment { get; set; }

        [Display(Name = "LastRequesterName", ResourceType = typeof(resxDbFields))]
        public string LastRequesterName { get; set; }
        [Display(Name = "LastRequesterNameGUID", ResourceType = typeof(resxDbFields))]
        public string LastRequesterNameGUID { get; set; }

        [Display(Name = "RequestStatusName", ResourceType = typeof(resxDbFields))]
        public string RequestStatusName { get; set; }


        [Display(Name = "IsRequested", ResourceType = typeof(resxDbFields))]
        public bool? IsRequested { get; set; }


        [Display(Name = "FileGUID", ResourceType = typeof(resxDbFields))]
        public string FileGUID { get; set; }

        [Display(Name = "FileNumber", ResourceType = typeof(resxDbFields))]
        public string FileNumber { get; set; }


        [Display(Name = "LastCustodianType", ResourceType = typeof(resxDbFields))]
        public string LastCustodianType { get; set; }

        [Display(Name = "LastCustodianTypeName", ResourceType = typeof(resxDbFields))]
        public string LastCustodianTypeName { get; set; }

        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
        public string CreateByGUID { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "LastFlowStatusGUID", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusGUID { get; set; }
        [Display(Name = "LastCustodianTypeGUID", ResourceType = typeof(resxDbFields))]
        public string LastCustodianTypeGUID { get; set; }
        [Display(Name = "LastCustodianTypeNameGUID", ResourceType = typeof(resxDbFields))]
        public string LastCustodianTypeNameGUID { get; set; }

        [Display(Name = "LastFlowStatusName", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusName { get; set; }

        public byte[] dataUploadUserFileRowVersion { get; set; }
        public byte[] dataFileRowVersion { get; set; }
        public bool Active { get; set; }


    }
    public class RefugeeCasesHaveScheduledAppointmentModel
    {
        [Display(Name = "FileGUID", ResourceType = typeof(resxDbFields))]
        public string FileGUID { get; set; }

        [Display(Name = "CurrentOwner", ResourceType = typeof(resxDbFields))]
        public string CurrentOwner { get; set; }

        [Display(Name = "LastTransferFromName", ResourceType = typeof(resxDbFields))]
        public string LastTransferFromName { get; set; }

        [Display(Name = "LastTransferFromNameGUID", ResourceType = typeof(resxDbFields))]
        public string LastTransferFromNameGUID { get; set; }




        [Display(Name = "ScannDocumentGUID", ResourceType = typeof(resxDbFields))]
        public string ScannDocumentGUID { get; set; }

        [Display(Name = "AppointmentDate", ResourceType = typeof(resxDbFields))]
        public DateTime? AppointmentDate { get; set; }

        [Display(Name = "AppointmentRecordedDate", ResourceType = typeof(resxDbFields))]
        public DateTime? AppointmentRecordedDate { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Display(Name = "FileNumber", ResourceType = typeof(resxDbFields))]
        public string FileNumber { get; set; }

        [Display(Name = "OrigionCountry", ResourceType = typeof(resxDbFields))]
        public string OrigionCountry { get; set; }


        [Display(Name = "RefugeeStatus", ResourceType = typeof(resxDbFields))]
        public string RefugeeStatus { get; set; }



        [Display(Name = "LastCustodianType", ResourceType = typeof(resxDbFields))]
        public string LastCustodianType { get; set; }

        [Display(Name = "LastCustodianTypeName", ResourceType = typeof(resxDbFields))]
        public string LastCustodianTypeName { get; set; }

        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
        public string CreateByGUID { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "LastFlowStatusGUID", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusGUID { get; set; }
        [Display(Name = "LastCustodianTypeGUID", ResourceType = typeof(resxDbFields))]
        public string LastCustodianTypeGUID { get; set; }
        [Display(Name = "LastCustodianTypeNameGUID", ResourceType = typeof(resxDbFields))]
        public string LastCustodianTypeNameGUID { get; set; }

        [Display(Name = "LastFlowStatusName", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusName { get; set; }


        public byte[] dataFileRowVersion { get; set; }
        public bool Active { get; set; }


    }
    public class RefugeeCasesHaveScheduledAppointmentDataTableModel
    {
        [Display(Name = "FileGUID", ResourceType = typeof(resxDbFields))]
        public string FileGUID { get; set; }


        [Display(Name = "ScannDocumentGUID", ResourceType = typeof(resxDbFields))]
        public string ScannDocumentGUID { get; set; }

        [Display(Name = "AppointmentDate", ResourceType = typeof(resxDbFields))]
        public DateTime? AppointmentDate { get; set; }

        [Display(Name = "AppointmentRecordedDate", ResourceType = typeof(resxDbFields))]
        public DateTime? AppointmentRecordedDate { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Display(Name = "FileNumber", ResourceType = typeof(resxDbFields))]
        public string FileNumber { get; set; }

        [Display(Name = "OrigionCountry", ResourceType = typeof(resxDbFields))]
        public string OrigionCountry { get; set; }


        [Display(Name = "RefugeeStatus", ResourceType = typeof(resxDbFields))]
        public string RefugeeStatus { get; set; }



        [Display(Name = "LastCustodianType", ResourceType = typeof(resxDbFields))]
        public string LastCustodianType { get; set; }

        [Display(Name = "LastCustodianTypeName", ResourceType = typeof(resxDbFields))]
        public string LastCustodianTypeName { get; set; }

        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
        public string CreateByGUID { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "LastFlowStatusGUID", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusGUID { get; set; }
        [Display(Name = "LastCustodianTypeGUID", ResourceType = typeof(resxDbFields))]
        public string LastCustodianTypeGUID { get; set; }
        [Display(Name = "LastCustodianTypeNameGUID", ResourceType = typeof(resxDbFields))]
        public string LastCustodianTypeNameGUID { get; set; }

        [Display(Name = "LastFlowStatusName", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusName { get; set; }


        public byte[] dataFileRowVersion { get; set; }
        public bool Active { get; set; }


    }

    public class RefugeeScannDocumentModel
    {
        [Display(Name = "ScannDocumentGUID", ResourceType = typeof(resxDbFields))]
        public Guid ScannDocumentGUID { get; set; }
        [Display(Name = "FileGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> FileGUID { get; set; }

        [Display(Name = "FileNumber", ResourceType = typeof(resxDbFields))]
        public string FileNumber { get; set; }

        [Display(Name = "OrigionCountry", ResourceType = typeof(resxDbFields))]
        public string OrigionCountry { get; set; }


        [Display(Name = "RefugeeStatus", ResourceType = typeof(resxDbFields))]
        public string RefugeeStatus { get; set; }
        [Display(Name = "DocumentCabinetShelfGUID", ResourceType = typeof(resxDbFields))]
        public string DocumentCabinetShelfGUID { get; set; }

        [Display(Name = "IsAvailable", ResourceType = typeof(resxDbFields))]
        public bool? IsAvailable { get; set; }

        [Display(Name = "LastCustodianType", ResourceType = typeof(resxDbFields))]
        public string LastCustodianType { get; set; }

        [Display(Name = "LastCustodianTypeName", ResourceType = typeof(resxDbFields))]
        public string LastCustodianTypeName { get; set; }

        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
        public string CreateByGUID { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreateDate { get; set; }


        public byte[] dataFileRowVersion { get; set; }
        public bool Active { get; set; }

        public byte[] dataScannDocumentRowVersion { get; set; }
    }

    public class ScannNewDocumentModel
    {
        [Display(Name = "OwnedByStaff", ResourceType = typeof(resxDbFields))]
        public bool? OwnedByStaff { get; set; }
        public Nullable<Guid> ScannDocumentGUID { get; set; }

        public Nullable<Guid> FileGUID { get; set; }

        public string FileNumber { get; set; }
        public int? TotalScanDoc { get; set; }
        public string LastCustodianType { get; set; }
        public string LastCustodianTypeName { get; set; }
        public bool? IsAvailable { get; set; }

        public string RefugeeStatus { get; set; }
        public string ProcessStatusName { get; set; }
        public string OrigionCountry { get; set; }
        public byte[] dataFileRowVersion { get; set; }
        public bool Active { get; set; }

        public byte[] dataScannDocumentRowVersion { get; set; }
    }

    public class CaseInformatioModel
    {
        public string FileNumber { get; set; }
        public string Country { get; set; }
        public Nullable<Guid> FileGUID { get; set; }
    }

    public class ScannerSettingVM
    {
        public Guid? DocumentSourceGUID { get; set; }
        public string ScanningType { get; set; }
        public string PaperFormate { get; set; }
        public string Resolution { get; set; }
        public string PaperSize { get; set; }
        public string ColorMode { get; set; }

        public Guid? ScanningTypeGUID { get; set; }
        public Guid? PaperFormateGUID { get; set; }
        public Guid? ResolutionGUID { get; set; }
        public Guid? PaperSizeGUID { get; set; }
        public Guid? ColorModeGUID { get; set; }

    }

    public class TransferFileModel
    {
        public Guid? FileGUID { get; set; }
        [Display(Name = "RequesterTypeGUID", ResourceType = typeof(resxDbFields))]

        public Nullable<Guid> RequesterTypeGUID { get; set; }

        [Display(Name = "TransferLocationGUID", ResourceType = typeof(resxDbFields))]

        public Nullable<Guid> TransferLocationGUID { get; set; }
        public Guid? FileRequestGUID { get; set; }
        [Display(Name = "LastCustodianTypeGUID", ResourceType = typeof(resxDbFields))]

        public Nullable<Guid> LastCustodianTypeGUID { get; set; }
        [Display(Name = "LastCustodianTypeNameGUID", ResourceType = typeof(resxDbFields))]

        public Nullable<Guid> LastCustodianTypeNameGUID { get; set; }

        [Display(Name = "RequesterGUID", ResourceType = typeof(resxDbFields))]
        public Guid? RequesterGUID { get; set; }

        [Display(Name = "TransferEndDate", ResourceType = typeof(resxDbFields))]
        public DateTime? TransferEndDate { get; set; }

        [Display(Name = "RequestDurationDate", ResourceType = typeof(resxDbFields))]
        public DateTime? RequestDurationDate { get; set; }

        [Display(Name = "RequestDate", ResourceType = typeof(resxDbFields))]
        public DateTime? RequestDate { get; set; }
        [Display(Name = "ReceiveDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ReceiveDate { get; set; }

        public string Comments { get; set; }



    }
    public class ConfirmReturnFileModel
    {
        public Guid? FileGUID { get; set; }
        [Display(Name = "FileReturnDate", ResourceType = typeof(resxDbFields))]

        public DateTime? FileReturnDate { get; set; }

        public bool? Active { get; set; }



    }
    public class FileModel
    {
        [Display(Name = "FileNumber", ResourceType = typeof(resxDbFields))]
        public string FileNumber { get; set; }
        public string FileNumbers { get; set; }

        [Display(Name = "DocumentCabinetShelfGUID", ResourceType = typeof(resxDbFields))]
        public Guid DocumentCabinetShelfGUID { get; set; }
        
        [Display(Name = "CabinetGUID", ResourceType = typeof(resxDbFields))]
        public Guid CabinetGUID { get; set; }

        [Display(Name = "LastCustodianTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid LastCustodianTypeGUID { get; set; }


        [Display(Name = "LastCustodianTypeNameGUID", ResourceType = typeof(resxDbFields))]
        public Guid LastCustodianTypeNameGUID { get; set; }
        public bool? Active { get; set; }

    }

    public class FileTransferHistoryDataTableModel
    {

        [Display(Name = "FileGUID", ResourceType = typeof(resxDbFields))]
        public string FileGUID { get; set; }

        [Display(Name = "FileNumber", ResourceType = typeof(resxDbFields))]
        public string FileNumber { get; set; }



        [Display(Name = "ScannDocumentTransferGUID", ResourceType = typeof(resxDbFields))]
        public string ScannDocumentTransferGUID { get; set; }





        [Display(Name = "CustodianType", ResourceType = typeof(resxDbFields))]
        public string CustodianType { get; set; }

        [Display(Name = "CustodianName", ResourceType = typeof(resxDbFields))]
        public string CustodianName { get; set; }

        [Display(Name = "DeliveryStatus", ResourceType = typeof(resxDbFields))]
        public string DeliveryStatus { get; set; }

        [Display(Name = "TransferDate", ResourceType = typeof(resxDbFields))]
        public DateTime? TransferDate { get; set; }

        [Display(Name = "TransferBy", ResourceType = typeof(resxDbFields))]
        public string TransferBy { get; set; }
        [Display(Name = "ReceiveDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ReceiveDate { get; set; }
        [Display(Name = "ReceiveBy", ResourceType = typeof(resxDbFields))]
        public string ReceiveBy { get; set; }


        [Display(Name = "RequesterGUID", ResourceType = typeof(resxDbFields))]
        public string RequesterGUID { get; set; }
        [Display(Name = "RequesterTypeGUID", ResourceType = typeof(resxDbFields))]
        public string RequesterTypeGUID { get; set; }
        [Display(Name = "ReceiveByGUID", ResourceType = typeof(resxDbFields))]
        public string ReceiveByGUID { get; set; }
        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
        public string CreateByGUID { get; set; }



        public byte[] dataScannDocumentTransferRowVersion { get; set; }
        public bool Active { get; set; }


    }
    #region Confirm Receving File
    public class ConfirmReceivingFileModel
    {
        public bool? Validation { get; set; }
        public Guid FileGUID { get; set; }
        public string FileNumber { get; set; }

        public string IssuedBy { get; set; }

        public DateTime? IssuedDate { get; set; }
        public Guid ScannDocumentTransferGUID { get; set; }
        public Guid? DocumentFlowStatusGUID { get; set; }
        public Guid? LastCustodianTypeNameGUID { get; set; }

    }
    #endregion

    #region File request


    public class FileRequestConfirmationModel
    {
        public string ConfirmationStatus { get; set; }
    }
    #endregion
    #region Dashboard
    public class FileDashboardModel
    {
        public int? TotalCasesDelayInReturn { get; set; }
        public int? TotalCasesPendingConfirmation { get; set; }
        public int? TotalCasesUserPendingConfirmation { get; set; }
        public int? TotalCasesHaveAppointmentNotInfiling { get; set; }
    }

    public class RefugeeCasesDataTableFilter
    {
        public string cases { get; set; }

    }
    #endregion
    #region Units 
    public class UnitDataTableModel
    {
        [Display(Name = "SiteOwner", ResourceType = typeof(resxDbFields))]
        public string SiteOwner { get; set; }
        [Display(Name = "DestinationUnitName", ResourceType = typeof(resxDbFields))]
        public string DestinationUnitName { get; set; }
        [Display(Name = "DestinationUnitGUID", ResourceType = typeof(resxDbFields))]
        public string DestinationUnitGUID { get; set; }

        public bool? Active { get; set; }
        public byte[] codeDASDestinationUnitRowVersion { get; set; }
    }
    public class UnitUpdateModel
    {
        [Display(Name = "DestinationUnitName", ResourceType = typeof(resxDbFields))]
        public string DestinationUnitName { get; set; }
        [Display(Name = "DestinationUnitGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> DestinationUnitGUID { get; set; }
        [Display(Name = "SiteOwner", ResourceType = typeof(resxDbFields))]
        public string SiteOwner { get; set; }
        [Display(Name = "SiteOwner", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> SiteOwnerGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeDASDestinationUnitRowVersion { get; set; }
    }
    public class FileTrackingOverviewModel
    {
        public FileTrackingOverviewModel()
        {
            unitCasesDistModel = new List<UnitCasesDistModel>();
            staffCustodyMoedl = new List<StaffCustodyModel>();
        }
        public int? TotalCasesHoldByAllUnits { get; set; }
        public int? TotalCasesHoldByAllStaff { get; set; }
        public List<UnitCasesDistModel> unitCasesDistModel { get; set; }
        public List<StaffCustodyModel> staffCustodyMoedl { get; set; }
    }
    public class UnitCasesDistModel
    {
        public int? TotalCases { get; set; }
        public string UnitName { get; set; }
        public Guid DestinationUnitGUID { get; set; }
    }
    public class StaffCustodyModel
    {
        public int? TotalCases { get; set; }
        public string StaffName { get; set; }
        public Guid? UserGUID { get; set; }
    }
    public class CasesOverviewModel
    {
        public int? TotalCasesDelay { get; set; }
        public int? TotalCasesPendingConfirmation { get; set; }
        public int? TotalCasesConfirmed { get; set; }
        public int? TotalCasesRequested { get; set; }
    }
    #endregion

    public partial class DestinationUnitFocalPointModel
    {
        [Display(Name = "DestinationUnitFocalPointGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid DestinationUnitFocalPointGUID { get; set; }
        [Display(Name = "DestinationUnitGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DestinationUnitGUID { get; set; }
        [Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> UserGUID { get; set; }
        [Display(Name = "EmailAddress", ResourceType = typeof(resxDbFields))]
        public string EmailAddress { get; set; }
        [Display(Name = "FullName", ResourceType = typeof(resxDbFields))]
        public string FullName { get; set; }



        public bool Active { get; set; }
        public byte[] codeDASDestinationUnitFocalPointRowVersion { get; set; }

        [Display(Name = "IsSupervisor", ResourceType = typeof(resxDbFields))]
        public bool IsSupervisor { get; set; }

    }

    public class FileLocationMovementDataTableModel
    {
        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public string CreatedByGUID { get; set; }
        [Display(Name = "FileGUID", ResourceType = typeof(resxDbFields))]
        public string FileGUID { get; set; }

        [Display(Name = "FileNumber", ResourceType = typeof(resxDbFields))]
        public string FileNumber { get; set; }


        [Display(Name = "TransferLocationGUID", ResourceType = typeof(resxDbFields))]
        public string TransferLocationGUID { get; set; }

        [Display(Name = "FileLocationMovementGUID", ResourceType = typeof(resxDbFields))]
        public string FileLocationMovementGUID { get; set; }


        [Display(Name = "IsLastAction", ResourceType = typeof(resxDbFields))]
        public string IsLastAction { get; set; }

        [Display(Name = "ActionDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ActionDate { get; set; }
        [Display(Name = "CreateBy", ResourceType = typeof(resxDbFields))]
        public string CreateBy { get; set; }
        [Display(Name = "TransferLocationName", ResourceType = typeof(resxDbFields))]
        public string TransferLocationName { get; set; }

        [Display(Name = "LocationName", ResourceType = typeof(resxDbFields))]
        public string LocationName { get; set; }


        public byte[] dataFileLocationMovementRowVersion { get; set; }
        public bool Active { get; set; }


    }

    public partial class FileLocationMovementModel
    {
        [Display(Name = "FileLocationMovementGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid FileLocationMovementGUID { get; set; }
        [Display(Name = "FileGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> FileGUID { get; set; }
        [Display(Name = "TransferLocationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> TransferLocationGUID { get; set; }
        [Display(Name = "IsLastAction", ResourceType = typeof(resxDbFields))]
        public bool? IsLastAction { get; set; }
        [Display(Name = "ActionDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ActionDate { get; set; }
        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }
        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> CreatedByGUID { get; set; }
        public bool Active { get; set; }
        public byte[] dataFileLocationMovementRowVersion { get; set; }



    }
    public class OldFTSFileMovementDataTableModel
    {
        [Display(Name = "Id", ResourceType = typeof(resxDbFields))]
        public string Id { get; set; }
        [Display(Name = "ProcessingGroupNumber", ResourceType = typeof(resxDbFields))]
        public string ProcessingGroupNumber { get; set; }
        [Display(Name = "Returned", ResourceType = typeof(resxDbFields))]
        public bool? Returned { get; set; }

        [Display(Name = "StaffID", ResourceType = typeof(resxDbFields))]
        public string StaffID { get; set; }


        [Display(Name = "TransDate", ResourceType = typeof(resxDbFields))]
        public DateTime? TransDate { get; set; }

        [Display(Name = "ReturnDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ReturnDate { get; set; }


        [Display(Name = "DueDate", ResourceType = typeof(resxDbFields))]
        public DateTime? DueDate { get; set; }

        [Display(Name = "FileFrom", ResourceType = typeof(resxDbFields))]
        public string FileFrom { get; set; }
        [Display(Name = "LoanPeriod", ResourceType = typeof(resxDbFields))]
        public string LoanPeriod { get; set; }
        [Display(Name = "TransType", ResourceType = typeof(resxDbFields))]
        public string TransType { get; set; }

        [Display(Name = "NewCaseNo", ResourceType = typeof(resxDbFields))]
        public string NewCaseNo { get; set; }
        [Display(Name = "TransCreator", ResourceType = typeof(resxDbFields))]
        public string TransCreator { get; set; }

        public byte[] dataFileLocationMovementRowVersion { get; set; }
        public bool Active { get; set; }


    }
    #region Templates Configuration
    public class TemplateTypeDataTableModel
    {
        [Display(Name = "TemplateTypeGUID", ResourceType = typeof(resxDbFields))]
        public string TemplateTypeGUID { get; set; }
        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public string OrganizationInstanceGUID { get; set; }
        [Display(Name = "TemplateName", ResourceType = typeof(resxDbFields))]
        public string TemplateName { get; set; }

        [Display(Name = "TemplateCode", ResourceType = typeof(resxDbFields))]
        public string TemplateCode { get; set; }
        [Display(Name = "Description", ResourceType = typeof(resxDbFields))]
        public string Description { get; set; }

        [Display(Name = "ReferenceLinkTypeGUID", ResourceType = typeof(resxDbFields))]
        public string ReferenceLinkTypeGUID { get; set; }

        [Display(Name = "ReferenceLinkType", ResourceType = typeof(resxDbFields))]
        public string ReferenceLinkType { get; set; }


        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreateDate { get; set; }


        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
        public string CreateByGUID { get; set; }
        [Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? UpdateDate { get; set; }


        [Display(Name = "UpdateByGUID", ResourceType = typeof(resxDbFields))]
        public string UpdateByGUID { get; set; }
        public bool? Active { get; set; }
        public byte[] codeDASTemplateTypeRowVersion { get; set; }
    }
    public class TemplateTypeUpdateModel
    {
        [Display(Name = "TemplateTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid TemplateTypeGUID { get; set; }
        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public Guid? OrganizationInstanceGUID { get; set; }
        [Display(Name = "TemplateName", ResourceType = typeof(resxDbFields))]
        public string TemplateName { get; set; }

        [Display(Name = "TemplateCode", ResourceType = typeof(resxDbFields))]
        public string TemplateCode { get; set; }
        [Display(Name = "Description", ResourceType = typeof(resxDbFields))]
        public string Description { get; set; }

        [Display(Name = "ReferenceLinkTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ReferenceLinkTypeGUID { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreateDate { get; set; }


        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? CreateByGUID { get; set; }
        [Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? UpdateDate { get; set; }


        [Display(Name = "UpdateByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? UpdateByGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeDASTemplateTypeRowVersion { get; set; }
    }

    public class TemplateTypeDocumentDataTableModel
    {
        [Display(Name = "TemplateTypeDocumentGUID", ResourceType = typeof(resxDbFields))]
        public Guid? TemplateTypeDocumentGUID { get; set; }
        [Display(Name = "TemplateTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? TemplateTypeGUID { get; set; }
        [Display(Name = "DocumentName", ResourceType = typeof(resxDbFields))]
        public string DocumentName { get; set; }
        [Display(Name = "TemplateName", ResourceType = typeof(resxDbFields))]
        public string TemplateName { get; set; }
        [Display(Name = "Description", ResourceType = typeof(resxDbFields))]
        public string Description { get; set; }

        [Display(Name = "ReferenceLinkTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ReferenceLinkTypeGUID { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreateDate { get; set; }


        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? CreateByGUID { get; set; }
        [Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? UpdateDate { get; set; }


        [Display(Name = "UpdateByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? UpdateByGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeDASTemplateTypeDocumentRowVersion { get; set; }
    }

    public class TemplateTypeDocumentModel
    {
        [Display(Name = "TemplateTypeDocumentGUID", ResourceType = typeof(resxDbFields))]
        public Guid TemplateTypeDocumentGUID { get; set; }
        [Display(Name = "TemplateTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? TemplateTypeGUID { get; set; }
        [Display(Name = "DocumentName", ResourceType = typeof(resxDbFields))]
        public string DocumentName { get; set; }
        [Display(Name = "TemplateName", ResourceType = typeof(resxDbFields))]
        public string TemplateName { get; set; }
        [Display(Name = "DocumentCode", ResourceType = typeof(resxDbFields))]
        public string DocumentCode { get; set; }
        [Display(Name = "HasConfidentialData", ResourceType = typeof(resxDbFields))]
        public bool HasConfidentialData { get; set; }
        [Display(Name = "Description", ResourceType = typeof(resxDbFields))]
        public string Description { get; set; }

        [Display(Name = "ReferenceLinkTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ReferenceLinkTypeGUID { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreateDate { get; set; }


        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? CreateByGUID { get; set; }
        [Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? UpdateDate { get; set; }


        [Display(Name = "UpdateByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? UpdateByGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeDASTemplateTypeDocumentRowVersion { get; set; }
    }

    public class TemplateTypeDocumentTagModel
    {
        [Display(Name = "TemplateTypeDocumentTagGUID", ResourceType = typeof(resxDbFields))]
        public Guid TemplateTypeDocumentTagGUID { get; set; }
        [Display(Name = "TemplateTypeDocumentGUID", ResourceType = typeof(resxDbFields))]
        public Guid? TemplateTypeDocumentGUID { get; set; }
        [Display(Name = "TagName", ResourceType = typeof(resxDbFields))]
        public string TagName { get; set; }
        [Display(Name = "Description", ResourceType = typeof(resxDbFields))]
        public string Description { get; set; }

        [Display(Name = "IsMandatury", ResourceType = typeof(resxDbFields))]
        public bool IsMandatury { get; set; }



        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreateDate { get; set; }


        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? CreateByGUID { get; set; }
        [Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? UpdateDate { get; set; }


        [Display(Name = "UpdateByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? UpdateByGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeDASTemplateTypeDocumentTagRowVersion { get; set; }
    }

    public class TemplateTypeDocumentSoftFieldModel
    {
        [Display(Name = "TemplateTypeDocumentSoftFieldGUID", ResourceType = typeof(resxDbFields))]
        public Guid TemplateTypeDocumentSoftFieldGUID { get; set; }
        [Display(Name = "TemplateTypeDocumentGUID", ResourceType = typeof(resxDbFields))]
        public Guid? TemplateTypeDocumentGUID { get; set; }
        [Display(Name = "SoftFieldSourceTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? SoftFieldSourceTypeGUID { get; set; }

        [Display(Name = "SoftFieldName", ResourceType = typeof(resxDbFields))]
        public string SoftFieldName { get; set; }
        [Display(Name = "FieldTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? FieldTypeGUID { get; set; }

        [Display(Name = "IsMandatury", ResourceType = typeof(resxDbFields))]
        public bool IsMandatury { get; set; }



        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreateDate { get; set; }


        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? CreateByGUID { get; set; }
        [Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? UpdateDate { get; set; }


        [Display(Name = "UpdateByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? UpdateByGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeDASTemplateTypeDocumentSoftFieldRowVersion { get; set; }
    }

    #endregion

    #region Archive Templates
    public class ArchiveTemplateDataTableModel
    {
        [Display(Name = "ArchiveTemplateGUID", ResourceType = typeof(resxDbFields))]
        public string ArchiveTemplateGUID { get; set; }
        [Display(Name = "ArchiveTemplateCodeNumber", ResourceType = typeof(resxDbFields))]
        public string ArchiveTemplateCodeNumber { get; set; }
        [Display(Name = "TemplateTypeGUID", ResourceType = typeof(resxDbFields))]
        public string TemplateTypeGUID { get; set; }
        [Display(Name = "TemplateName", ResourceType = typeof(resxDbFields))]
        public string TemplateName { get; set; }
        [Display(Name = "FileReferenceGUID", ResourceType = typeof(resxDbFields))]
        public string FileReferenceGUID { get; set; }

        [Display(Name = "FileReferenceGUID", ResourceType = typeof(resxDbFields))]
        public string FileReference { get; set; }

        [Display(Name = "FileReferenceTypeGUID", ResourceType = typeof(resxDbFields))]
        public string FileReferenceTypeGUID { get; set; }

        [Display(Name = "FileReferenceTypeName", ResourceType = typeof(resxDbFields))]
        public string FileReferenceTypeName { get; set; }

        [Display(Name = "DocumentCabinetShelfGUID", ResourceType = typeof(resxDbFields))]
        public string DocumentCabinetShelfGUID { get; set; }

        [Display(Name = "IsAvailable", ResourceType = typeof(resxDbFields))]
        public bool? IsAvailable { get; set; }

        [Display(Name = "LastCustodianType", ResourceType = typeof(resxDbFields))]
        public string LastCustodianType { get; set; }

        [Display(Name = "LastCustodianTypeName", ResourceType = typeof(resxDbFields))]
        public string LastCustodianTypeName { get; set; }


        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreateDate { get; set; }


        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
        public string CreateByGUID { get; set; }
        [Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? UpdateDate { get; set; }


        [Display(Name = "UpdateByGUID", ResourceType = typeof(resxDbFields))]
        public string UpdateByGUID { get; set; }
        public bool Active { get; set; }
        public byte[] dataArchiveTemplateRowVersion { get; set; }
    }
    public class ArchiveTemplateUpdateModel
    {
        [Display(Name = "ArchiveTemplateGUID", ResourceType = typeof(resxDbFields))]
        public Guid ArchiveTemplateGUID { get; set; }
        [Display(Name = "TemplateTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? TemplateTypeGUID { get; set; }
        [Display(Name = "ArchiveTemplateCodeNumber", ResourceType = typeof(resxDbFields))]
        public string ArchiveTemplateCodeNumber { get; set; }
        [Display(Name = "TemplateName", ResourceType = typeof(resxDbFields))]

        public string TemplateName { get; set; }
        [Display(Name = "FileReferenceGUID", ResourceType = typeof(resxDbFields))]
        public Guid? FileReferenceGUID { get; set; }

        [Display(Name = "FileReferenceTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? FileReferenceTypeGUID { get; set; }

        [Display(Name = "FileReferenceTypeName", ResourceType = typeof(resxDbFields))]
        public string FileReferenceTypeName { get; set; }

        [Display(Name = "DocumentCabinetShelfGUID", ResourceType = typeof(resxDbFields))]
        public string DocumentCabinetShelfGUID { get; set; }

        [Display(Name = "IsAvailable", ResourceType = typeof(resxDbFields))]
        public bool? IsAvailable { get; set; }

        [Display(Name = "LastCustodianType", ResourceType = typeof(resxDbFields))]
        public string LastCustodianType { get; set; }

        [Display(Name = "LastCustodianTypeName", ResourceType = typeof(resxDbFields))]
        public string LastCustodianTypeName { get; set; }


        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreateDate { get; set; }


        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? CreateByGUID { get; set; }
        [Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? UpdateDate { get; set; }


        [Display(Name = "UpdateByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? UpdateByGUID { get; set; }
        public bool Active { get; set; }
        public byte[] dataArchiveTemplateRowVersion { get; set; }
    }
    #endregion

    #region Archive Documents
    public class ArchiveTemplateDocumentDataTableModel
    {
        [Display(Name = "ArchiveTemplateDocumentGUID", ResourceType = typeof(resxDbFields))]
        public string ArchiveTemplateDocumentGUID { get; set; }

        [Display(Name = "TemplateTypeDocumentGUID", ResourceType = typeof(resxDbFields))]
        public string TemplateTypeDocumentGUID { get; set; }

        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string LastVerificationStatusGUID { get; set; }

        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string Status { get; set; }


        [Display(Name = "TemplateTypeGUID", ResourceType = typeof(resxDbFields))]
        public string TemplateTypeGUID { get; set; }
        [Display(Name = "ArchiveTemplateDocumentCodeNumber", ResourceType = typeof(resxDbFields))]
        public string ArchiveTemplateDocumentCodeNumber { get; set; }


        [Display(Name = "TemplateName", ResourceType = typeof(resxDbFields))]

        public string TemplateName { get; set; }

        [Display(Name = "DocumentName", ResourceType = typeof(resxDbFields))]

        public string DocumentName { get; set; }

        [Display(Name = "FileReferenceGUID", ResourceType = typeof(resxDbFields))]
        public string FileReferenceGUID { get; set; }

        [Display(Name = "FileReferenceTypeGUID", ResourceType = typeof(resxDbFields))]
        public string FileReferenceTypeGUID { get; set; }

        [Display(Name = "FileReferenceTypeName", ResourceType = typeof(resxDbFields))]
        public string FileReferenceTypeName { get; set; }

        [Display(Name = "FileReferenceName", ResourceType = typeof(resxDbFields))]
        public string FileReferenceName { get; set; }
        [Display(Name = "DocumentCabinetShelfGUID", ResourceType = typeof(resxDbFields))]
        public string DocumentCabinetShelfGUID { get; set; }

        [Display(Name = "IsAvailable", ResourceType = typeof(resxDbFields))]
        public bool? IsAvailable { get; set; }

        [Display(Name = "LastCustodianType", ResourceType = typeof(resxDbFields))]
        public string LastCustodianType { get; set; }

        [Display(Name = "LastCustodianTypeName", ResourceType = typeof(resxDbFields))]
        public string LastCustodianTypeName { get; set; }


        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreateDate { get; set; }
        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
        public string CreateBy { get; set; }

        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
        public string CreateByGUID { get; set; }
        [Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? UpdateDate { get; set; }

        [Display(Name = "PhysicalLocation", ResourceType = typeof(resxDbFields))]
        public string PhysicalLocation { get; set; }

        [Display(Name = "UpdateByGUID", ResourceType = typeof(resxDbFields))]
        public string UpdateBy { get; set; }
        public bool Active { get; set; }
        public byte[] dataArchiveTemplateDocumentRowVersion { get; set; }
    }
    public class ArchiveTemplateDocumentUpdateModel
    {
        public ArchiveTemplateDocumentUpdateModel()
        {
            DocumentSoftFieldVM = new List<DocumentSoftFieldVM>();
            DocumentTagGUIDs = new List<Guid?>();
        }

        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public Guid OrganizationInstanceGUID { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DutyStationGUID { get; set; }

        public int? TotalImages { get; set; }
        public Guid? LastVerificationStatusGUID { get; set; }
        public List<DocumentSoftFieldVM> DocumentSoftFieldVM { get; set; }
        [Display(Name = "ArchiveTemplateDocumentGUID", ResourceType = typeof(resxDbFields))]
        public Guid ArchiveTemplateDocumentGUID { get; set; }

        [Display(Name = "TemplateTypeDocumentGUID", ResourceType = typeof(resxDbFields))]
        public Guid? TemplateTypeDocumentGUID { get; set; }



        [Display(Name = "TemplateTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? TemplateTypeGUID { get; set; }
        [Display(Name = "ArchiveTemplateDocumentCodeNumber", ResourceType = typeof(resxDbFields))]
        public string ArchiveTemplateDocumentCodeNumber { get; set; }


        [Display(Name = "TemplateName", ResourceType = typeof(resxDbFields))]

        public string TemplateName { get; set; }

        [Display(Name = "DocumentName", ResourceType = typeof(resxDbFields))]

        public string DocumentName { get; set; }

        [Display(Name = "FileReferenceGUID", ResourceType = typeof(resxDbFields))]
        public Guid? FileReferenceGUID { get; set; }

        [Display(Name = "FileReferenceTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? FileReferenceTypeGUID { get; set; }

        [Display(Name = "FileReferenceTypeName", ResourceType = typeof(resxDbFields))]
        public string FileReferenceTypeName { get; set; }

        [Display(Name = "FileReferenceName", ResourceType = typeof(resxDbFields))]
        public string FileReferenceName { get; set; }
        [Display(Name = "CabinetGUID", ResourceType = typeof(resxDbFields))]
        public Guid? CabinetGUID { get; set; }
        [Display(Name = "DocumentTagGUIDs", ResourceType = typeof(resxDbFields))]
        public List<Guid?> DocumentTagGUIDs { get; set; }

        [Display(Name = "DocumentCabinetShelfGUID", ResourceType = typeof(resxDbFields))]
        public Guid? DocumentCabinetShelfGUID { get; set; }

        [Display(Name = "IsAvailable", ResourceType = typeof(resxDbFields))]
        public bool? IsAvailable { get; set; }

        [Display(Name = "LastCustodianType", ResourceType = typeof(resxDbFields))]
        public string LastCustodianType { get; set; }

        [Display(Name = "LastCustodianTypeName", ResourceType = typeof(resxDbFields))]
        public string LastCustodianTypeName { get; set; }


        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreateDate { get; set; }


        //FullName
        [Display(Name = "FullName", ResourceType = typeof(resxDbFields))]
        public string CreatedBy { get; set; }

        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? CreateByGUID { get; set; }

        [Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? UpdateDate { get; set; }


        [Display(Name = "UpdateByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? UpdateByGUID { get; set; }

        [Display(Name = "UpdateByGUID", ResourceType = typeof(resxDbFields))]
        public string UpdateBy { get; set; }


        //FullName
        public string VerifiedUser { get; set; }
        public DateTime VerifiedDatetime { get; set; }
        public bool Active { get; set; }
        public byte[] dataArchiveTemplateDocumentRowVersion { get; set; }
    }

    public class DocumentSoftFieldVM
    {
        public string SoftFieldName { get; set; }
        public Guid TemplateTypeDocumentSoftFieldGUID { get; set; }



        public string SoftFieldNameValue { get; set; }
    }

    public class TemplateDocumentVM
    {

        public Guid ArchiveTemplateDocumentGUID { get; set; }
    }
    #endregion
}
