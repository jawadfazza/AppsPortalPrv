using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using WMS_DAL.Model;


namespace WMS_DAL.ViewModels
{
    public class ItemEntryDataTableModel
    {
        public Guid ItemInputGUID { get; set; }

        public int? BillNumber { get; set; }
        public int? SequenceNumber { get; set; }
        public DateTime? InputDate { get; set; }

        public bool Active { get; set; }

        public DateTime? DeletedOn { get; set; }
        public byte[] dataItemInputRowVersion { get; set; }
    }

    public class PendingRequestCheckVM
    {
        public int pendingForMyWarehosue { get; set; }
        public int pendingAll { get; set; }
        public int PendingForOtherWarehouse { get; set; }
        public int PendingForStaff { get; set; }
        public int DelayInReturn { get; set; }

        public System.DateTime? StartDate { get; set; }

        public System.DateTime? EndDate { get; set; }

    }


    public class ModelTrackReportModel
    {
        //public Guid ReportGUID { get; set; }
        //public DateTime? StartDate { get; set; }
        //public DateTime? EndDate { get; set; }
        public int WarehosueReportId { get; set; }
        public List<Guid?> CustodianGUIDs { get; set; }
        [Display(Name = "WarehouseItemGUID", ResourceType = typeof(resxDbFields))]

        public List<Guid?> WarehouseItemGUID { get; set; }

        [Display(Name = "LocationDescription", ResourceType = typeof(resxDbFields))]

        public List<Guid?> LocationGuids { get; set; }


        public bool? equationType { get; set; }

        public List<Guid?> CustodianStaffGUID { get; set; }
        public List<Guid?> CustodianTypesGUID { get; set; }
        public List<Guid?> CustodianWarehouseGUID { get; set; }
        public List<Guid?> ItemGUIDs { get; set; }
        public List<Guid?> ModelGUIDs { get; set; }
        public List<Guid?> BrandGuids { get; set; }
        public List<Guid?> WarehouseItemClassificationGUID { get; set; }

        public List<Guid?> ServicesStatusGUIDs { get; set; }






    }

    public class TrackCustodianVM
    {
        //public Guid ReportGUID { get; set; }
        //public DateTime? StartDate { get; set; }
        //public DateTime? EndDate { get; set; }
        public string CustodianName { get; set; }
        public string ItemName { get; set; }
        public string itemDetail { get; set; }




    }
    public class ItemModelDataTableModel
    {
        public System.Guid WarehouseItemModelGUID { get; set; }
        public string ModelDescription { get; set; }
        public string ItemDescription { get; set; }

        public string BrandDescription { get; set; }

        public string WarehouseItemGUID { get; set; }

        [Display(Name = "ItemClassification", ResourceType = typeof(resxDbFields))]
        public string WarehouseItemClassificationDescription { get; set; }
        [Display(Name = "ItemClassification", ResourceType = typeof(resxDbFields))]
        public string WarehouseItemClassificationGUID { get; set; }

        public System.Guid ItemGUID { get; set; }
        public string BrandGUID { get; set; }
        public bool Active { get; set; }
        public byte[] codeWarehouseItemModelRowVersion { get; set; }

    }

    public class ItemModelUpdateModel
    {
        [Display(Name = "ItemModelRelationTypeGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemModelRelationTypeGUID { get; set; }

        [Display(Name = "ParentWarehouseItemModelGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> ParentWarehouseItemModelGUID { get; set; }
        public HttpPostedFileBase UploadModelImage { get; set; }

        public string ItemImage { get; set; }

        public bool? IsEdit { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "WarehouseItemModelGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid WarehouseItemModelGUID { get; set; }

        [Display(Name = "ItemModelWarehouseGUID", ResourceType = typeof(resxDbFields))]

        public Nullable<System.Guid> ItemModelWarehouseGUID { get; set; }
        [Display(Name = "ItemSubClassification", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> WarehouseItemGUID { get; set; }

        [Display(Name = "ItemClassification", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> WarehouseItemClassificationGUID { get; set; }




        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BrandGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> BrandGUID { get; set; }
        [Display(Name = "ModelDescription", ResourceType = typeof(resxDbFields))]
        public string ModelDescription { get; set; }
        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]


        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BrandDescription", ResourceType = typeof(resxDbFields))]
        public string BrandDescription { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "WarehouseItemDescription", ResourceType = typeof(resxDbFields))]
        public string WarehouseItemDescription { get; set; }



        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemDescription", ResourceType = typeof(resxDbFields))]
        public string ItemDescription { get; set; }



        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] codeWarehouseItemModelRowVersion { get; set; }
        public byte[] codeWarehouseItemModelLanguageRowVersion { get; set; }
    }




    public class ItemClassificaitonDataTableModel
    {
        public System.Guid WarehouseItemClassificationGUID { get; set; }
        public string WarehouseItemClassificationDescription { get; set; }

        //public System.Guid WarehouseTypeGUID { get; set; }
        public string WarehouseType { get; set; }



        public bool Active { get; set; }
        public byte[] codeWarehouseItemClassificationRowVersion { get; set; }

    }



    public class ItemClassificationUpdateModel
    {

        [Display(Name = "WarehouseItemClassificationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid WarehouseItemClassificationGUID { get; set; }


        [Display(Name = "WarehouseItemClassificationDescription", ResourceType = typeof(resxDbFields))]
        public string WarehouseItemClassificationDescription { get; set; }

        public bool Active { get; set; }
        public byte[] codeWarehouseItemClassificationRowVersion { get; set; }
        public byte[] codeWarehouseItemClassificationLanguageRowVersion { get; set; }
    }

    public class ItemDataTableModel
    {
        public System.Guid WarehouseItemGUID { get; set; }
        public System.Guid WarehouseItemClassificationGUID { get; set; }
        [Display(Name = "WarehouseItemDescription", ResourceType = typeof(resxDbFields))]
        public string WarehouseItemDescription { get; set; }
        [Display(Name = "WarehouseItemClassificationDescription", ResourceType = typeof(resxDbFields))]
        public string WarehouseItemClassificationDescription { get; set; }

        //public System.Guid WarehouseTypeGUID { get; set; }
        public string WarehouseType { get; set; }

        public bool IsDeterminanted { get; set; }

        public bool Active { get; set; }
        public byte[] codeWarehouseItemRowVersion { get; set; }
        public byte[] codeWarehouseItemLanguageRowVersion { get; set; }

    }

    public class ItemUpdateModel
    {

        [Display(Name = "WarehouseItemClassificationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid WarehouseItemClassificationGUID { get; set; }


        public bool IsDeterminanted { get; set; }


        [Display(Name = "WarehouseItemGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid WarehouseItemGUID { get; set; }

        [Display(Name = "WarehouseItemDescription", ResourceType = typeof(resxDbFields))]
        public string WarehouseItemDescription { get; set; }

        public bool Active { get; set; }
        public byte[] codeWarehouseItemRowVersion { get; set; }
        public byte[] codeWarehouseItemLanguageRowVersion { get; set; }

    }

    public class ModelMovementModel
    {



        public string EmptyColumn { get; set; }




        public string ModelStatus { get; set; }
        public string RequestStatus { get; set; }

        [Display(Name = "ItemOutputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemOutputDetailGUID { get; set; }

        [Display(Name = "ItemInputGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemInputGUID { get; set; }



        [Display(Name = "ItemOutputGUID", ResourceType = typeof(resxDbFields))]

        public Nullable<System.Guid> ItemOutputGUID { get; set; }




        [Display(Name = "RequestType", ResourceType = typeof(resxDbFields))]
        public string RequestType { get; set; }
        [Display(Name = "RequestedQunatity", ResourceType = typeof(resxDbFields))]
        public int? RequestedQunatity { get; set; }


        [Display(Name = "BarcodeNumber", ResourceType = typeof(resxDbFields))]
        public string BarcodeNumber { get; set; }
        [Display(Name = "SerialNumber", ResourceType = typeof(resxDbFields))]
        public string SerialNumber { get; set; }

        [Display(Name = "IME1", ResourceType = typeof(resxDbFields))]
        public string IME1 { get; set; }

        //[Display(Name = "PartNumber", ResourceType = typeof(resxDbFields))]
        //public string PartNumber { get; set; }


        public DateTime? ExpectedStartDate { get; set; }
        public DateTime? ExpectedReturnedDate { get; set; }
        public DateTime? ActualReturnedDate { get; set; }

        public string Comments { get; set; }
        public byte[] dataItemOutputDetailRowVersion { get; set; }
        public bool Active { get; set; }
        public string CustodianType { get; set; }
        public string Custodian { get; set; }
        public string ReleaseType { get; set; }
        public int? Qunatity { get; set; }
        public string DeliveryStatus { get; set; }
        public DateTime DeliveryActionDate { get; set; }
    }


    public class ModelDeterminantVM
    {
        public string DeterminantName { get; set; }
        public Guid WarehouseItemModelDeterminantGUID { get; set; }



        public string DeterminantValue { get; set; }
    }
    public class ModelDeterminantOtherVM
    {
        public string DeterminantName { get; set; }
        public Guid WarehouseItemModelDeterminantGUID { get; set; }

        public Guid ItemInputDetailGUID { get; set; }

        public string DeterminantValue { get; set; }
    }

    public class Determinants
    {
        public Guid ItemInputDetailGUID { get; set; }


        public Nullable<Guid> BarcodeGUID { get; set; }

        public Nullable<Guid> SerilaGUID { get; set; }

        public Nullable<Guid> IMEI1GUID { get; set; }

        public Nullable<Guid> IMEI2GUID { get; set; }
        public Nullable<Guid> GSMGUID { get; set; }
        public Nullable<Guid> MACGUID { get; set; }
        public Nullable<Guid> MSRPIDGUID { get; set; }
        public Nullable<Guid> SeqenceNumberGUID { get; set; }
        public string BarcodeNumber { get; set; }
        public string SequenceNumber { get; set; }
        public bool? IsHasBarcode { get; set; }
        public bool? IsHasSeq { get; set; }
        public string SerialNumber { get; set; }
        public bool? IsHasSerialNumber { get; set; }
        public string IMEI1 { get; set; }
        public bool? IsHasIMEI1 { get; set; }
        public string IMEI2 { get; set; }
        public bool? IsHasIMEI2 { get; set; }
        public string MAC { get; set; }
        public bool? IsHasMAC { get; set; }
        public string GSM { get; set; }
        public bool? IsHasGSM { get; set; }

        public string MSRPID { get; set; }
        public bool? IsHasMSRPID { get; set; }
    }
    public class ItemModelDeterminantModel
    {
        public Guid WarehouseItemModelDeterminantGUID { get; set; }
        public Guid WarehouseItemModelGUID { get; set; }
        public Guid DeterminantGUID { get; set; }

        public string DeterminatDescription { get; set; }

        public bool Active { get; set; }


    }

    public class ItemModelWarehouseModel
    {
        public Guid ItemModelWarehouseGUID { get; set; }
        public Guid WarehouseItemModelGUID { get; set; }
        public Guid WarehouseGUID { get; set; }

        public int LowestAmountAllowed { get; set; }

        public int ReOrderedLimit { get; set; }

        public string dataItemModelWarehouseRowVersion { get; set; }

        public bool Active { get; set; }


    }

    public class EntryModelsDataTableModel
    {
        [Display(Name = "ItemInputGUID", ResourceType = typeof(resxDbFields))]
        public Guid ItemInputGUID { get; set; }
        [Display(Name = "BillNumber", ResourceType = typeof(resxDbFields))]
        public string BillNumber { get; set; }
        [Display(Name = "BillSourceType", ResourceType = typeof(resxDbFields))]
        public string BillSourceType { get; set; }
        [Display(Name = "BillSourceName", ResourceType = typeof(resxDbFields))]
        public string BillSourceName { get; set; }
        [Display(Name = "SequenceNumber", ResourceType = typeof(resxDbFields))]
        public int? SequenceNumber { get; set; }
        [Display(Name = "InputDate", ResourceType = typeof(resxDbFields))]
        public DateTime? InputDate { get; set; }

        [Display(Name = "SourceGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> SourceGUID { get; set; }
        public bool Active { get; set; }
        public byte[] dataItemInputRowVersion { get; set; }
    }



    public class EntryModelUpdateModel
    {
        [Display(Name = "PONumber", ResourceType = typeof(resxDbFields))]
        public string BillNumber { get; set; }

        [Display(Name = "InputDate", ResourceType = typeof(resxDbFields))]
        public DateTime? InputDate { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SourceGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid SourceGUID { get; set; }



        [Display(Name = "SourceNameGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> SourceNameGUID { get; set; }

        public Guid CreatedByGUID { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "ItemInputGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ItemInputGUID { get; set; }
        public int? SequenceNumber { get; set; }

        public bool Active { get; set; }

        public byte[] dataItemInputRowVersion { get; set; }
    }



    public class ReleaseModelUpdateModel
    {

        public Guid CreatedByGUID { get; set; }

        [Display(Name = "CreatedDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreatedDate { get; set; }


        [Display(Name = "OutputDate", ResourceType = typeof(resxDbFields))]
        public DateTime? OutputDate { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RequesterGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> RequesterGUID { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RequesterNameGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> RequesterNameGUID { get; set; }

        [Display(Name = "WarehouseGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> WarehouseGUID { get; set; }


        [Display(Name = "ItemOutputGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ItemOutputGUID { get; set; }
        [Display(Name = "OutputNumber", ResourceType = typeof(resxDbFields))]
        public int? OutputNumber { get; set; }

        public bool Active { get; set; }
        public string Comments { get; set; }

        public byte[] dataItemOutputRowVersion { get; set; }
    }

    public class ReleaseModelDetailUpdateModel
    {
        public DateTime? ExpectedReturenedDate;

        public DateTime? ActualReturenedDate;

        public Guid CreatedByGUID { get; set; }

        [Display(Name = "CreatedDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreatedDate { get; set; }


        [Display(Name = "OutputDate", ResourceType = typeof(resxDbFields))]
        public DateTime? OutputDate { get; set; }


        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemOutputGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemOutputGUID { get; set; }


        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemOutputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemOutputDetailGUID { get; set; }


        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemInputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemInputDetailGUID { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemRequestTypeGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemRequestTypeGUID { get; set; }





        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemModelWarehouseGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemModelWarehouseGUID { get; set; }



        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]

        [Display(Name = "ModelDescription", ResourceType = typeof(resxDbFields))]
        public string ModelDescription { get; set; }



        [Display(Name = "BarcodeNumber", ResourceType = typeof(resxDbFields))]
        public string BarcodeNumber { get; set; }
        [Display(Name = "SerialNumber", ResourceType = typeof(resxDbFields))]
        public string SerialNumber { get; set; }

        [Display(Name = "IME1", ResourceType = typeof(resxDbFields))]
        public string IME1 { get; set; }

        //[Display(Name = "PartNumber", ResourceType = typeof(resxDbFields))]
        //public string PartNumber { get; set; }

        [Display(Name = "RequestedQunatity", ResourceType = typeof(resxDbFields))]
        public int? RequestedQunatity { get; set; }
        [Display(Name = "ExpectedStartDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ExpectedStartDate { get; set; }

        [Display(Name = "ExpectedReturnedDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ExpectedReturnedDate { get; set; }
        [Display(Name = "ActualReturnedDate", ResourceType = typeof(resxDbFields))]

        public DateTime? ActualReturnedDate { get; set; }

        public string Comments { get; set; }
        [Display(Name = "PackageSize", ResourceType = typeof(resxDbFields))]
        public int? PackageSize { get; set; }
        [Display(Name = "SIMPackageSize", ResourceType = typeof(resxDbFields))]
        public string SIMPackageSize { get; set; }
        [Display(Name = "HasCharger", ResourceType = typeof(resxDbFields))]
        public bool? HasCharger { get; set; }
        [Display(Name = "HasLaptopMouse", ResourceType = typeof(resxDbFields))]
        public bool? HasLaptopMouse { get; set; }
        [Display(Name = "HasHeadPhone", ResourceType = typeof(resxDbFields))]
        public bool? HasHeadPhone { get; set; }
        [Display(Name = "HasBag", ResourceType = typeof(resxDbFields))]
        public bool? HasBag { get; set; }

        [Display(Name = "HasHeadsetUSB", ResourceType = typeof(resxDbFields))]
        public bool? HasHasHeadsetUSB { get; set; }

        [Display(Name = "HasHeadsetStereoJack", ResourceType = typeof(resxDbFields))]
        public bool? HasHeadsetStereoJack { get; set; }

        [Display(Name = "HasBackbag", ResourceType = typeof(resxDbFields))]
        public bool? HasBackbag { get; set; }

        [Display(Name = "HasHandbag", ResourceType = typeof(resxDbFields))]
        public bool? HasHandbag { get; set; }
        [Display(Name = "HasFlashMemory", ResourceType = typeof(resxDbFields))]
        public bool? HasFlashMemory { get; set; }
        public byte[] dataItemOutputDetailRowVersion { get; set; }
        public bool Active { get; set; }


    }

    public class ReleaseSingleItemUpdateModalUpdateModel
    {
        [Display(Name = "ItemTagGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ItemTagGUID { get; set; }

        public Guid? LastFlowTypeGUID { get; set; }



        [Display(Name = "NotifyStaffByEmailGUID", ResourceType = typeof(resxDbFields))]
        public Guid? NotifyStaffByEmailGUID { get; set; }

        [Display(Name = "ItemServiceStatusGUID", ResourceType = typeof(resxDbFields))]
        public string ItemServiceStatusGUID { get; set; }
        public bool? Validation { get; set; }
        public Nullable<Guid> LastWarehouseLocationGUID { get; set; }
        public Nullable<Guid> LastWarehouseSubLocationGUID { get; set; }
        public bool? isChildrenKit { get; set; }
        [Display(Name = "ParentItemModelWarehouseGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ParentItemModelWarehouseGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RequesterGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid RequesterGUID { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RequesterNameGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> RequesterNameGUID { get; set; }

        [Display(Name = "OutputNumber", ResourceType = typeof(resxDbFields))]
        public int? OutputNumber { get; set; }
        [Display(Name = "ItemStatusGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemStatusGUID { get; set; }

        public Nullable<System.Guid> CurrentFlowTypeGUID { get; set; }

        public Guid CreatedByGUID { get; set; }

        [Display(Name = "CreatedDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreatedDate { get; set; }


        [Display(Name = "OutputDate", ResourceType = typeof(resxDbFields))]
        public DateTime? OutputDate { get; set; }

        [Display(Name = "WarehouseItemDescription", ResourceType = typeof(resxDbFields))]
        public string WarehouseItemDescription { get; set; }

        [Display(Name = "BrandDescription", ResourceType = typeof(resxDbFields))]
        public string BrandDescription { get; set; }

        public string IssuedBy { get; set; }


        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemOutputGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemOutputGUID { get; set; }

        [Display(Name = "LocationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> LocationGUID { get; set; }

        [Display(Name = "LocationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> WarehouseLocationGUID { get; set; }

        [Display(Name = "WarehouseSubLocationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> WarehouseSubLocationGUID { get; set; }



        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemOutputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemOutputDetailGUID { get; set; }


        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemInputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemInputDetailGUID { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemRequestTypeGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemRequestTypeGUID { get; set; }



        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemModelWarehouseGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemModelWarehouseGUID { get; set; }



        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]

        [Display(Name = "ModelDescription", ResourceType = typeof(resxDbFields))]
        public string ModelDescription { get; set; }



        [Display(Name = "BarcodeNumber", ResourceType = typeof(resxDbFields))]
        public string BarcodeNumber { get; set; }
        [Display(Name = "SerialNumber", ResourceType = typeof(resxDbFields))]
        public string SerialNumber { get; set; }

        [Display(Name = "IME1", ResourceType = typeof(resxDbFields))]
        public string IME1 { get; set; }
        [Display(Name = "GSM", ResourceType = typeof(resxDbFields))]
        public string GSM { get; set; }
        public string MAC { get; set; }
        public string SequenceNumber { get; set; }


        //[Display(Name = "PartNumber", ResourceType = typeof(resxDbFields))]
        //public string PartNumber { get; set; }

        [Display(Name = "RequestedQunatity", ResourceType = typeof(resxDbFields))]
        public int? RequestedQunatity { get; set; }
        [Display(Name = "ExpectedStartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ExpectedStartDate { get; set; }

        [Display(Name = "LicenseStartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? LicenseStartDate { get; set; }


        [Display(Name = "LicenseExpiryDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? LicenseExpiryDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [Display(Name = "ExpectedReturnedDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ExpectedReturenedDate { get; set; }

        [Display(Name = "ActualReturnedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]

        public DateTime? ActualReturenedDate { get; set; }

        [Display(Name = "PackageSize", ResourceType = typeof(resxDbFields))]
        public int? PackageSize { get; set; }
        [Display(Name = "SIMPackageSize", ResourceType = typeof(resxDbFields))]
        public Guid? SIMPackageSizeGUID { get; set; }


        [Display(Name = "SIMPackageSize", ResourceType = typeof(resxDbFields))]
        public string SIMPackageSize { get; set; }
        [Display(Name = "HasCharger", ResourceType = typeof(resxDbFields))]
        public bool HasCharger { get; set; }
        [Display(Name = "HasLaptopMouse", ResourceType = typeof(resxDbFields))]
        public bool HasLaptopMouse { get; set; }

        [Display(Name = "HasRoaming", ResourceType = typeof(resxDbFields))]
        public bool HasRoaming { get; set; }


        [Display(Name = "HasInternationalAccess", ResourceType = typeof(resxDbFields))]
        public bool HasInternationalAccess { get; set; }
        [Display(Name = "HasHeadPhone", ResourceType = typeof(resxDbFields))]
        public bool HasHeadPhone { get; set; }
        [Display(Name = "HasHeadsetUSB", ResourceType = typeof(resxDbFields))]

        public bool HasHeadsetUSB { get; set; }
        [Display(Name = "HasBag", ResourceType = typeof(resxDbFields))]

        public bool HasBag { get; set; }

        [Display(Name = "HasHeadsetStereoJack", ResourceType = typeof(resxDbFields))]

        public bool HasHeadsetStereoJack { get; set; }

        [Display(Name = "HasBackbag", ResourceType = typeof(resxDbFields))]

        public bool HasBackbag { get; set; }

        [Display(Name = "HasHandbag", ResourceType = typeof(resxDbFields))]

        public bool HasHandbag { get; set; }

        [Display(Name = "HasFlashMemory", ResourceType = typeof(resxDbFields))]

        public bool HasFlashMemory { get; set; }




        public string Comments { get; set; }
        public byte[] dataItemOutputDetailRowVersion { get; set; }
        public bool Active { get; set; }
        public byte[] dataItemOutputRowVersion { get; set; }


    }
    public class ItemBulkStatusVM
    {
        public List<Guid?> ItemInputModelGuids { get; set; }
        public Guid ItemStatusGUID { get; set; }
        public Guid ItemInputDetailGUID { get; set; }
        public bool Active { get; set; }
    }
    public class ReleaseBulkItemUpdateModalUpdateModel
    {
        public ReleaseBulkItemUpdateModalUpdateModel()
        {
            ItemInputModelGuids = new List<Guid?>();
        }
        [Display(Name = "LocationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> WarehouseLocationGUID { get; set; }

        [Display(Name = "WarehouseSubLocationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> WarehouseSubLocationGUID { get; set; }
        [Display(Name = "ItemStatusGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemStatusGUID { get; set; }

        public bool? isChildrenKit { get; set; }
        [Display(Name = "ParentItemModelWarehouseGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ParentItemModelWarehouseGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RequesterGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid RequesterGUID { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RequesterNameGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> RequesterNameGUID { get; set; }

        [Display(Name = "OutputNumber", ResourceType = typeof(resxDbFields))]
        public int? OutputNumber { get; set; }
        public Guid CreatedByGUID { get; set; }

        [Display(Name = "CreatedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreatedDate { get; set; }


        [Display(Name = "OutputDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? OutputDate { get; set; }


        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemOutputGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemOutputGUID { get; set; }

        [Display(Name = "LocationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> LocationGUID { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemOutputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemOutputDetailGUID { get; set; }


        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemInputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemInputDetailGUID { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemRequestTypeGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemRequestTypeGUID { get; set; }

        [Display(Name = "SerialNumber", ResourceType = typeof(resxDbFields))]
        public List<Guid?> ItemInputModelGuids { get; set; }

        [Display(Name = "SerialNumber", ResourceType = typeof(resxDbFields))]
        public string SerialNumbers { get; set; }


        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemModelWarehouseGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemModelWarehouseGUID { get; set; }



        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]

        [Display(Name = "ModelDescription", ResourceType = typeof(resxDbFields))]
        public string ModelDescription { get; set; }



        [Display(Name = "BarcodeNumber", ResourceType = typeof(resxDbFields))]
        public string BarcodeNumber { get; set; }
        [Display(Name = "SerialNumber", ResourceType = typeof(resxDbFields))]
        public string SerialNumber { get; set; }

        [Display(Name = "IME1", ResourceType = typeof(resxDbFields))]
        public string IME1 { get; set; }
        [Display(Name = "GSM", ResourceType = typeof(resxDbFields))]
        public string GSM { get; set; }


        //[Display(Name = "PartNumber", ResourceType = typeof(resxDbFields))]
        //public string PartNumber { get; set; }

        [Display(Name = "RequestedQunatity", ResourceType = typeof(resxDbFields))]
        public int? RequestedQunatity { get; set; }
        [Display(Name = "ExpectedStartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ExpectedStartDate { get; set; }

        [Display(Name = "ExpectedReturnedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ExpectedReturnedDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ExpectedReturenedDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ActualReturenedDate { get; set; }
        [Display(Name = "ActualReturnedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ActualReturnedDate { get; set; }


        [Display(Name = "NotifyStaffByEmailGUID", ResourceType = typeof(resxDbFields))]
        public Guid? NotifyStaffByEmailGUID { get; set; }

        public string Comments { get; set; }
        public byte[] dataItemOutputDetailRowVersion { get; set; }
        public bool Active { get; set; }
        public byte[] dataItemOutputRowVersion { get; set; }


    }



    public class ReleaseModelsDataTableModel
    {
        public Guid ItemOutputGUID { get; set; }
        public int? OutputNumber { get; set; }

        public string RequsterType { get; set; }
        public string RequsterName { get; set; }

        public string RequestStatus { get; set; }

        public DateTime? OutputDate { get; set; }
        public bool Active { get; set; }
        public byte[] dataItemOutputRowVersion { get; set; }
    }

    public class ModelManagementModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "WarehouseItemModelGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid WarehouseItemModelGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemInputDetailGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ItemInputDetailGUID { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]

        [Display(Name = "ModelDescription", ResourceType = typeof(resxDbFields))]
        public string ModelDescription { get; set; }

        [Display(Name = "ItemDescription", ResourceType = typeof(resxDbFields))]
        public string ItemDescription { get; set; }


        [Display(Name = "BarcodeNumber", ResourceType = typeof(resxDbFields))]
        public string BarcodeNumber { get; set; }
        [Display(Name = "SerialNumber", ResourceType = typeof(resxDbFields))]
        public string SerialNumber { get; set; }

        [Display(Name = "IME1", ResourceType = typeof(resxDbFields))]
        public string IME1 { get; set; }

        [Display(Name = "PartNumber", ResourceType = typeof(resxDbFields))]
        public string PartNumber { get; set; }

        [Display(Name = "Custodian", ResourceType = typeof(resxDbFields))]
        public string Custodian { get; set; }

        [Display(Name = "ModelStatus", ResourceType = typeof(resxDbFields))]
        public string ModelStatus { get; set; }








        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataItemInputDetailRowVersion { get; set; }
        public byte[] codeWarehouseItemModelLanguageRowVersion { get; set; }
    }

    public class ItemRequestUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemRequestGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ItemRequestGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RequesterGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid RequesterGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RequesterNameGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid RequesterNameGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RequestTypeGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid RequestTypeGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]

        [Display(Name = "RequestStartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? RequestStartDate { get; set; }


        [Display(Name = "RequestEndDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? RequestEndDate { get; set; }


        [Display(Name = "LastFlowStatusGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> LastFlowStatusGUID { get; set; }

        public string Comments { get; set; }



        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public DateTime? CreatedByGUID { get; set; }
        public DateTime CreatedDate { get; set; }

        public byte[] dataItemRequestRowVersion { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }
    }

    public class ItemRequestDataTableModel
    {
        public Guid ItemRequestGUID { get; set; }
        public byte[] dataItemRequestRowVersion { get; set; }
        public bool Active { get; set; }

        [Display(Name = "Requester", ResourceType = typeof(resxDbFields))]
        public string Requester { get; set; }

        [Display(Name = "RequesterName", ResourceType = typeof(resxDbFields))]
        public string RequesterName { get; set; }


        [Display(Name = "RequestType", ResourceType = typeof(resxDbFields))]
        public string RequestType { get; set; }


        [Display(Name = "RequestEndDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? RequestEndDate { get; set; }


        [Display(Name = "RequestStartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? RequestStartDate { get; set; }
        [Display(Name = "RequestStatus", ResourceType = typeof(resxDbFields))]
        public string RequestStatus { get; set; }

        public string Comments { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }




        public int SortID { get; set; }

    }

    public class ItemRequestDetailUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemRequestDetailGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ItemRequestDetailGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemRequestGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ItemRequestGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "WarehouseItemGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid WarehouseItemGUID { get; set; }

        [Display(Name = "QuantityOrdered", ResourceType = typeof(resxDbFields))]
        public int QuantityOrdered { get; set; }

        public string Comments { get; set; }
        [Display(Name = "RequestedItem", ResourceType = typeof(resxDbFields))]
        public string RequestedItem { get; set; }


        public byte[] dataItemRequestDetailRowVersion { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }




    }

    public class ItemRequestDetailDataTableModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemRequestDetailGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ItemRequestDetailGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemRequestGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemRequestGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "WarehouseItemGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid WarehouseItemGUID { get; set; }

        [Display(Name = "QuantityOrdered", ResourceType = typeof(resxDbFields))]
        public int? QuantityOrdered { get; set; }

        public string Comments { get; set; }

        [Display(Name = "RequestedItem", ResourceType = typeof(resxDbFields))]
        public string RequestedItem { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }
        public byte[] dataItemRequestDetailRowVersion { get; set; }




    }

    public class ItemOutputDetailFlowModel
    {
        public System.Guid ItemOutputDetailFlowGUID { get; set; }

        [Display(Name = "LocationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> WarehouseLocationGUID { get; set; }


        public Nullable<System.Guid> ItemOutputDetailGUID { get; set; }
        public Nullable<System.Guid> ItemStatusGUID { get; set; }

        public Nullable<System.Guid> ItemStatuGUID { get; set; }
        public Nullable<System.Guid> FlowTypeGUID { get; set; }
        public Nullable<bool> IsLastAction { get; set; }
        public Nullable<bool> IsLastMove { get; set; }
        public Nullable<System.Guid> CreatedByGUID { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ReturnedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreatedDate { get; set; }
        public bool Active { get; set; }
        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }
        public byte[] dataItemOutputDetailFlowRowVersion { get; set; }
    }

    public class ItemInputVerificationModel
    {
        public Nullable<System.Guid> ItemInputDetailGUID { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreateDate { get; set; }
        public bool? Active { get; set; }
        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

    }

    public class ItemOutputDetailNotificationModel
    {
        public System.Guid ItemOutputNotificationGUID { get; set; }
        public Nullable<System.Guid> ItemOutputDetailGUID { get; set; }

        public string NotificationMessage { get; set; }

        public DateTime? NotficationDate { get; set; }
        public DateTime? CreatedDate { get; set; }


        public Nullable<bool> IsRecevied { get; set; }
        public Nullable<bool> IsLastMove { get; set; }
        public Nullable<System.Guid> CreatedByGUID { get; set; }

        public bool Active { get; set; }
        public byte[] dataItemOutputNotificationRowVersion { get; set; }
    }

    #region entry

    public class WarehouseModelEntryMovementDataTableModel
    {
        [Display(Name = "LicenseTypeGUID", ResourceType = typeof(resxDbFields))]
        public string LicenseTypeGUID { get; set; }

        [Display(Name = "CostCenterGUID", ResourceType = typeof(resxDbFields))]
        public string CostCenterGUID { get; set; }

        [Display(Name = "SequenceNumber", ResourceType = typeof(resxDbFields))]
        public string SequenceNumber { get; set; }
        
        [Display(Name = "Identifier", ResourceType = typeof(resxDbFields))]
        public string Identifier { get; set; }

        [Display(Name = "BudgetYear", ResourceType = typeof(resxDbFields))]
        public string BudgetYear { get; set; }

        [Display(Name = "LastIssueDate", ResourceType = typeof(resxDbFields))]
        public DateTime? LastIssueDate { get; set; }

        [Display(Name = "LastExpiryDate", ResourceType = typeof(resxDbFields))]
        public DateTime? LastExpiryDate { get; set; }
        [Display(Name = "RecruitmentTypeGUID", ResourceType = typeof(resxDbFields))]
        public string RecruitmentTypeName { get; set; }

        [Display(Name = "RecruitmentTypeGUID", ResourceType = typeof(resxDbFields))]
        public string RecruitmentTypeGUID { get; set; }

        [Display(Name = "LastVerifiedByGUID", ResourceType = typeof(resxDbFields))]
        public string LastVerifiedByGUID { get; set; }
        [Display(Name = "EmailAddress", ResourceType = typeof(resxDbFields))]
        public string EmailAddress { get; set; }
        [Display(Name = "BillNumber", ResourceType = typeof(resxDbFields))]
        public string BillNumber { get; set; }
        [Display(Name = "ModelAge", ResourceType = typeof(resxDbFields))]

        public double? ModelAge { get; set; }

        [Display(Name = "ItemServiceStatusGUID", ResourceType = typeof(resxDbFields))]
        public string ServiceItemStatus { get; set; }

        [Display(Name = "TransferFromCountryGUID", ResourceType = typeof(resxDbFields))]
        public string TransferFromCountryGUID { get; set; }

        [Display(Name = "TransferToCountryGUID", ResourceType = typeof(resxDbFields))]
        public string TransferToCountryGUID { get; set; }

        [Display(Name = "TransferFromCountryGUID", ResourceType = typeof(resxDbFields))]
        public string TransferFromCountry { get; set; }

        [Display(Name = "TransferToCountryGUID", ResourceType = typeof(resxDbFields))]
        public string TransferToCountry { get; set; }

        [Display(Name = "TransferToDutyStationGUID", ResourceType = typeof(resxDbFields))]
        public string TransferToDutyStationGUID { get; set; }

        [Display(Name = "TransferFromDutyStationGUID", ResourceType = typeof(resxDbFields))]
        public string TransferFromDutyStationGUID { get; set; }

        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
        public string DepartmentGUID { get; set; }

        [Display(Name = "DepartmentName", ResourceType = typeof(resxDbFields))]
        public string DepartmentDescription { get; set; }

        public string ItemImage { get; set; }



        [Display(Name = "VerificationStatusGUID", ResourceType = typeof(resxDbFields))]
        public string VerificationStatusGUID { get; set; }

        [Display(Name = "ItemConditionGUID", ResourceType = typeof(resxDbFields))]
        public string ItemCondition { get; set; }
        [Display(Name = "ItemConditionGUID", ResourceType = typeof(resxDbFields))]
        public string ItemConditionGUID { get; set; }

        [Display(Name = "ItemServiceStatusGUID", ResourceType = typeof(resxDbFields))]
        public string ItemServiceStatusGUID { get; set; }

        public Nullable<Guid> PurposeofuseGUID { get; set; }

        public string Purposeofuse { get; set; }
        [Display(Name = "AcquisitionDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? AcquisitionDate { get; set; }

        [Display(Name = "DeliveryStatus", ResourceType = typeof(resxDbFields))]
        public string DeliveryStatus { get; set; }
        public string ModelStatusGUID { get; set; }

        public string CustodianStaffGUID { get; set; }
        public string CustodianWarehouseGUID { get; set; }
        public string DeliveryStatusGUID { get; set; }

        public string WarehouseLocationGUID { get; set; }

        public string LastCustdianGUID { get; set; }


        public string CreatedByGUID { get; set; }

        [Display(Name = "CreatedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "WarehouseItemGUID", ResourceType = typeof(resxDbFields))]
        public string WarehouseItemGUID { get; set; }
        public Guid LastCustodianType { get; set; }

        public Guid LastCustodianName { get; set; }


        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemInputDetailGUID", ResourceType = typeof(resxDbFields))]

        public Nullable<System.Guid> ItemInputDetailGUID { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemInputGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemInputGUID { get; set; }

        [Display(Name = "WarehouseItemClassificationGUID", ResourceType = typeof(resxDbFields))]
        public string WarehouseItemClassificationGUID { get; set; }
        [Display(Name = "BrandGUID", ResourceType = typeof(resxDbFields))]
        public string BrandGUID { get; set; }


        [Display(Name = "ItemStatusGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemStatusGUID { get; set; }

        [Display(Name = "ItemModelWarehouseGUID", ResourceType = typeof(resxDbFields))]
        public string ItemModelWarehouseGUID { get; set; }

        [Display(Name = "WarehouseStoreGUID", ResourceType = typeof(resxDbFields))]
        public string WarehouseStoreGUID { get; set; }

        [Display(Name = "WarehouseOwner", ResourceType = typeof(resxDbFields))]
        public string WarehouseOwner { get; set; }

        [Display(Name = "Governorate", ResourceType = typeof(resxDbFields))]
        public string Governorate { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public string CreatedBy { get; set; }

        [Display(Name = "LastVerifiedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? LastVerifiedDate { get; set; }
        [Display(Name = "VerifiedBy", ResourceType = typeof(resxDbFields))]
        public string VerifiedBy { get; set; }



        [Display(Name = "Qunatity", ResourceType = typeof(resxDbFields))]
        public int Qunatity { get; set; }

        [Display(Name = "LocationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> LocationGUID { get; set; }


        [Display(Name = "PriceTypeGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> PriceTypeGUID { get; set; }

        [Display(Name = "PriceValue", ResourceType = typeof(resxDbFields))]
        public decimal? PriceValue { get; set; }

        [Display(Name = "InventoryStatusGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> InventoryStatusGUID { get; set; }
        [Display(Name = "ItemBrandModelColorGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemBrandModelColorGUID { get; set; }

        public string WarehouseItemClassificationDescription { get; set; }

        public string WarehouseLocationDescription { get; set; }
        public string WarehouseOwnerGUID { get; set; }

        public string Comments { get; set; }

        public byte[] dataItemInputDetailRowVersion { get; set; }

        public byte[] dataItemInputRowVersion { get; set; }



        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]

        [Display(Name = "ModelDescription", ResourceType = typeof(resxDbFields))]
        public string ModelDescription { get; set; }

        [Display(Name = "ItemDescription", ResourceType = typeof(resxDbFields))]
        public string ItemDescription { get; set; }

        [StringLength(50, MinimumLength = 5, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BarcodeNumber", ResourceType = typeof(resxDbFields))]
        public string BarcodeNumber { get; set; }
        [Display(Name = "SerialNumber", ResourceType = typeof(resxDbFields))]
        [StringLength(50, MinimumLength = 5, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public string SerialNumber { get; set; }

        [Display(Name = "IME1", ResourceType = typeof(resxDbFields))]
        [StringLength(500, MinimumLength = 5, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public string IME1 { get; set; }

        [Display(Name = "GSM", ResourceType = typeof(resxDbFields))]
        [StringLength(500, MinimumLength = 5, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public string GSM { get; set; }

        [Display(Name = "MSRPID", ResourceType = typeof(resxDbFields))]
        [StringLength(500, MinimumLength = 5, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public string MSRPID { get; set; }
        public string MAC { get; set; }



        [Display(Name = "GSMNumber", ResourceType = typeof(resxDbFields))]
        public string GSMNumber { get; set; }

        [Display(Name = "PartNumber", ResourceType = typeof(resxDbFields))]
        public string PartNumber { get; set; }

        [Display(Name = "Custodian", ResourceType = typeof(resxDbFields))]
        public string Custodian { get; set; }

        [Display(Name = "ModelStatus", ResourceType = typeof(resxDbFields))]
        public string ModelStatus { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }
        public byte[] codeWarehouseItemModelLanguageRowVersion { get; set; }


    }

    public class ItemInputDetailModel
    {
        public HttpPostedFileBase UploadDamagedReportFile { get; set; }

        public int? TotalAvaiable { get; set; }
        public string Location { get; set; }
        public string WarehouseName { get; set; }

        public string ItemImage { get; set; }
        public bool? IsOwner { get; set; }

        public double? ModelAge { get; set; }
        public string DamagedReportName { get; set; }
        public DateTime? LastVerifiedDate { get; set; }
        public Nullable<Guid> LastVerifiedByGUID { get; set; }
        public bool? IsVerified { get; set; }
        public Nullable<Guid> VerificationStatusGUID { get; set; }

        public string SequenceNumber { get; set; }

        public string ItemClassification { get; set; }

        public string ItemSubClassification { get; set; }

        //to add 
        public string ItemBrand { get; set; }

        [Display(Name = "DamagedBy", ResourceType = typeof(resxDbFields))]
        public Guid? DamagedByGUID { get; set; }


        [Display(Name = "LastReservedByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? LastReservedByGUID { get; set; }

        [Display(Name = "LastReservedByTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? LastReservedByTypeGUID { get; set; }


        [Display(Name = "LastReservedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? LastReservedDate { get; set; }

        [Display(Name = "LicenseTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? LicenseTypeGUID { get; set; }

        [Display(Name = "CostCenterGUID", ResourceType = typeof(resxDbFields))]
        public Guid? CostCenterGUID { get; set; }

        [Display(Name = "BudgetYear", ResourceType = typeof(resxDbFields))]
        public int? BudgetYear { get; set; }

        [Display(Name = "LastIssueDate", ResourceType = typeof(resxDbFields))]
        public DateTime? LastIssueDate { get; set; }

        [Display(Name = "LastExpiryDate", ResourceType = typeof(resxDbFields))]
        public DateTime? LastExpiryDate { get; set; }


        [Display(Name = "DamagedDate", ResourceType = typeof(resxDbFields))]

        public DateTime? DamagedDate { get; set; }

        [Display(Name = "DamagedPart", ResourceType = typeof(resxDbFields))]


        public string DamagedPart { get; set; }


        [Display(Name = "DamagedComment", ResourceType = typeof(resxDbFields))]
        public string DamagedComment { get; set; }

        public string LastCustodianName { get; set; }
        public string BarcodeNumber { get; set; }
        public string SerialNumber { get; set; }
        public string IMEI1 { get; set; }
        public string IMEI2 { get; set; }
        public string GSM { get; set; }
        public string MAC { get; set; }
        public bool? IsDeterminanted { get; set; }

        [Display(Name = "TransferFromCountryGUID", ResourceType = typeof(resxDbFields))]
        public Guid? TransferFromCountryGUID { get; set; }

        [Display(Name = "TransferToCountryGUID", ResourceType = typeof(resxDbFields))]
        public Guid? TransferToCountryGUID { get; set; }

        [Display(Name = "TransferToDutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Guid? TransferToDutyStationGUID { get; set; }

        [Display(Name = "TransferFromDutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Guid? TransferFromDutyStationGUID { get; set; }

        [Display(Name = "TransferDescription", ResourceType = typeof(resxDbFields))]
        public string TransferDescription { get; set; }



        [Display(Name = "ItemModelRelationTypeGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ItemModelRelationTypeGUID { get; set; }

        [Display(Name = "ParentItemModelWarehouseGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> ParentItemModelWarehouseGUID { get; set; }
        public Nullable<Guid> PurposeofuseGUID { get; set; }
        public Nullable<Guid> ItemStatusGUID { get; set; }
        public Nullable<Guid> ItemServiceStatusGUID { get; set; }
        public Nullable<Guid> ItemConditionGUID { get; set; }
        public Nullable<Guid> WarehouseOwnerGUID { get; set; }
        public Nullable<Guid> LastCustdianGUID { get; set; }
        public Nullable<Guid> LastCustdianNameGUID { get; set; }
        public Nullable<Guid> LastWarehouseLocationGUID { get; set; }
        public Nullable<Guid> LastFlowTypeGUID { get; set; }
        [Display(Name = "ColorGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> ColorGUID { get; set; }

        [Display(Name = "AcquisitionDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? AcquisitionDate { get; set; }
        public string Comments { get; set; }
        [Display(Name = "PriceValue", ResourceType = typeof(resxDbFields))]
        public decimal? PriceValue { get; set; }

        public string ModelNameDetermiant { get; set; }

        public string WarehouseItemGUID { get; set; }


        [Display(Name = "PriceTypeGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> PriceTypeGUID { get; set; }

        [Display(Name = "PONumber", ResourceType = typeof(resxDbFields))]
        public string BillNumber { get; set; }

        [Display(Name = "InputDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? InputDate { get; set; }


        [Display(Name = "Warrantylength", ResourceType = typeof(resxDbFields))]
        public int? Warrantylength { get; set; }

        [Display(Name = "WarrantyEndDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public int? WarrantyEndDate { get; set; }


        [Display(Name = "DepreciatedYears", ResourceType = typeof(resxDbFields))]
        public int? DepreciatedYears { get; set; }



        [Display(Name = "DepreciatedYears", ResourceType = typeof(resxDbFields))]
        public string DepreciatedYearsText { get; set; }

        [Display(Name = "RetirementYears", ResourceType = typeof(resxDbFields))]
        public int? RetirementYears { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SourceGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> SourceGUID { get; set; }
        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> CreatedByGUID { get; set; }

        [Display(Name = "CreatedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.Guid> LastCustodianType { get; set; }



        [Display(Name = "ItemInputDetailGUID", ResourceType = typeof(resxDbFields))]

        public Nullable<System.Guid> ItemInputDetailGUID { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemInputGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemInputGUID { get; set; }
        public byte[] dataItemInputDetailRowVersion { get; set; }

        [Display(Name = "ItemModelWarehouseGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemModelWarehouseGUID { get; set; }
        public bool Active { get; set; }

        public bool IsAvaliable { get; set; }

        [Display(Name = "LocationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> LocationGUID { get; set; }


        [Display(Name = "InventoryStatusGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> InventoryStatusGUID { get; set; }
        public List<ModelDeterminantVM> ModelDeterminantVM { get; set; }
        public List<ModelDeterminantOtherVM> ModelDeterminantOtherVM { get; set; }


        public codeItemModelWarehouse codeItemModelWarehouse { get; set; }

        [Display(Name = "Qunatity", ResourceType = typeof(resxDbFields))]
        public int Qunatity { get; set; }
        public ItemInputDetailModel()
        {
            ModelDeterminantVM = new List<ModelDeterminantVM>();
            ModelDeterminantOtherVM = new List<ModelDeterminantOtherVM>();


        }

    }

    public class ItemInputDetailDataTableModel
    {
        public ItemInputDetailDataTableModel()
        {
            ModelDeterminantVM = new List<ModelDeterminantVM>();


        }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SourceGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid SourceGUID { get; set; }

        [Display(Name = "PONumber", ResourceType = typeof(resxDbFields))]
        public string BillNumber { get; set; }

        [Display(Name = "InputDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? InputDate { get; set; }


        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> CreatedByGUID { get; set; }

        [Display(Name = "CreatedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreatedDate { get; set; }

        public Nullable<System.Guid> LastCustodianType { get; set; }

        public Nullable<System.Guid> LastCustodianName { get; set; }


        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemInputDetailGUID", ResourceType = typeof(resxDbFields))]

        public Nullable<System.Guid> ItemInputDetailGUID { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemInputGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemInputGUID { get; set; }


        [Display(Name = "ItemStatusGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemStatusGUID { get; set; }

        [Display(Name = "ItemModelWarehouseGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemModelWarehouseGUID { get; set; }

        [Display(Name = "WarehouseStoreGUID", ResourceType = typeof(resxDbFields))]
        public string WarehouseStoreGUID { get; set; }

        [Display(Name = "WarehouseOwner", ResourceType = typeof(resxDbFields))]
        public string WarehouseOwner { get; set; }

        [Display(Name = "Governorate", ResourceType = typeof(resxDbFields))]
        public string Governorate { get; set; }




        [Display(Name = "Qunatity", ResourceType = typeof(resxDbFields))]
        public int Qunatity { get; set; }

        [Display(Name = "LocationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> LocationGUID { get; set; }


        [Display(Name = "PriceTypeGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> PriceTypeGUID { get; set; }

        [Display(Name = "PriceValue", ResourceType = typeof(resxDbFields))]
        public decimal? PriceValue { get; set; }

        [Display(Name = "InventoryStatusGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> InventoryStatusGUID { get; set; }
        [Display(Name = "ItemBrandModelColorGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemBrandModelColorGUID { get; set; }

        public string Comments { get; set; }

        public byte[] dataItemInputDetailRowVersion { get; set; }

        public byte[] dataItemInputRowVersion { get; set; }



        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]

        [Display(Name = "ModelDescription", ResourceType = typeof(resxDbFields))]
        public string ModelDescription { get; set; }

        [Display(Name = "ItemDescription", ResourceType = typeof(resxDbFields))]
        public string ItemDescription { get; set; }

        [StringLength(50, MinimumLength = 5, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BarcodeNumber", ResourceType = typeof(resxDbFields))]
        public string BarcodeNumber { get; set; }
        [Display(Name = "SerialNumber", ResourceType = typeof(resxDbFields))]
        [StringLength(50, MinimumLength = 5, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public string SerialNumber { get; set; }

        [Display(Name = "IME1", ResourceType = typeof(resxDbFields))]
        [StringLength(50, MinimumLength = 5, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public string IME1 { get; set; }


        [Display(Name = "GSMNumber", ResourceType = typeof(resxDbFields))]
        public string GSMNumber { get; set; }

        [Display(Name = "PartNumber", ResourceType = typeof(resxDbFields))]
        public string PartNumber { get; set; }

        [Display(Name = "Custodian", ResourceType = typeof(resxDbFields))]
        public string Custodian { get; set; }

        [Display(Name = "ModelStatus", ResourceType = typeof(resxDbFields))]
        public string ModelStatus { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }
        public byte[] codeWarehouseItemModelLanguageRowVersion { get; set; }

        public List<ModelDeterminantVM> ModelDeterminantVM { get; set; }



    }


    public class ModelReleaseMovementsUpdateModel
    {


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RequesterGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid RequesterGUID { get; set; }

        [Display(Name = "LocationGUID", ResourceType = typeof(resxDbFields))]

        public Nullable<System.Guid> LocationGUID { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RequesterNameGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> RequesterNameGUID { get; set; }

        [Display(Name = "OutputNumber", ResourceType = typeof(resxDbFields))]
        public int? OutputNumber { get; set; }
        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public Guid CreatedByGUID { get; set; }

        [Display(Name = "CreatedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreatedDate { get; set; }


        [Display(Name = "OutputDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? OutputDate { get; set; }


        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemOutputGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemOutputGUID { get; set; }


        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemOutputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemOutputDetailGUID { get; set; }


        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemInputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemInputDetailGUID { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemRequestTypeGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemRequestTypeGUID { get; set; }





        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemModelWarehouseGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemModelWarehouseGUID { get; set; }



        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]

        [Display(Name = "ModelDescription", ResourceType = typeof(resxDbFields))]
        public string ModelDescription { get; set; }



        [Display(Name = "BarcodeNumber", ResourceType = typeof(resxDbFields))]
        public string BarcodeNumber { get; set; }
        [Display(Name = "SerialNumber", ResourceType = typeof(resxDbFields))]
        public string SerialNumber { get; set; }

        [Display(Name = "IME1", ResourceType = typeof(resxDbFields))]
        public string IME1 { get; set; }

        //[Display(Name = "PartNumber", ResourceType = typeof(resxDbFields))]
        //public string PartNumber { get; set; }

        [Display(Name = "RequestedQunatity", ResourceType = typeof(resxDbFields))]
        public int? RequestedQunatity { get; set; }
        [Display(Name = "ExpectedStartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ExpectedStartDate { get; set; }

        [Display(Name = "ExpectedReturnedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ExpectedReturnedDate { get; set; }
        [Display(Name = "ActualReturnedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]

        public DateTime? ActualReturnedDate { get; set; }

        public string Comments { get; set; }
        public byte[] dataItemOutputDetailRowVersion { get; set; }
        public bool Active { get; set; }
        public byte[] dataItemOutputRowVersion { get; set; }


    }



    public class WarehouseModelReleaseMovementDataTableModel
    {


        [Display(Name = "ParentItemModelWarehouseGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ParentItemModelWarehouseGUID { get; set; }
        public string EmptyColumn { get; set; }
        public string ModelStatus { get; set; }
        [Display(Name = "StatusOnReturn", ResourceType = typeof(resxDbFields))]
        public string StatusOnReturn { get; set; }


        public string RequestStatus { get; set; }
        [Display(Name = "ItemOutputDetailFlowGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemOutputDetailFlowGUID { get; set; }

        [Display(Name = "ItemOutputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemOutputDetailGUID { get; set; }

        [Display(Name = "ItemInputGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemInputGUID { get; set; }



        [Display(Name = "ItemOutputGUID", ResourceType = typeof(resxDbFields))]

        public Nullable<System.Guid> ItemOutputGUID { get; set; }

        [Display(Name = "ItemOutputGUID", ResourceType = typeof(resxDbFields))]

        public string ItemStatus { get; set; }

        [Display(Name = "IssuedBy", ResourceType = typeof(resxDbFields))]
        public string IssuedBy { get; set; }

        [Display(Name = "IssuedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? IssuedDate { get; set; }


        [Display(Name = "ReturnedBy", ResourceType = typeof(resxDbFields))]
        public string ReturnedBy { get; set; }

        [Display(Name = "ReturnedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ReturnedDate { get; set; }


        [Display(Name = "RequestType", ResourceType = typeof(resxDbFields))]
        public string RequestType { get; set; }
        [Display(Name = "RequestedQunatity", ResourceType = typeof(resxDbFields))]
        public int? RequestedQunatity { get; set; }


        [Display(Name = "BarcodeNumber", ResourceType = typeof(resxDbFields))]
        public string BarcodeNumber { get; set; }
        [Display(Name = "SerialNumber", ResourceType = typeof(resxDbFields))]
        public string SerialNumber { get; set; }

        [Display(Name = "IME1", ResourceType = typeof(resxDbFields))]
        public string IME1 { get; set; }

        //[Display(Name = "PartNumber", ResourceType = typeof(resxDbFields))]
        //public string PartNumber { get; set; }


        public DateTime? ExpectedStartDate { get; set; }
        public DateTime? ExpectedReturenedDate { get; set; }
        public DateTime? ActualReturenedDate { get; set; }

        public string Comments { get; set; }
        public byte[] dataItemOutputDetailRowVersion { get; set; }
        public bool Active { get; set; }

        public bool? IsAvaliable { get; set; }
        public string CustodianType { get; set; }
        public string Custodian { get; set; }
        public string ReleaseType { get; set; }
        public int? Qunatity { get; set; }
        public string DeliveryStatus { get; set; }
        public DateTime DeliveryActionDate { get; set; }
    }



    public class WarehouseModelDamagedMovementDataTableModel
    {

        public string EmptyColumn { get; set; }
        [Display(Name = "ItemOutputDetailDamagedTrackGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemOutputDetailDamagedTrackGUID { get; set; }

        [Display(Name = "ItemInputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemInputDetailGUID { get; set; }

        [Display(Name = "ItemOutputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemOutputDetailGUID { get; set; }



        [Display(Name = "ItemOutputGUID", ResourceType = typeof(resxDbFields))]

        public Nullable<System.Guid> ItemOutputGUID { get; set; }




        [Display(Name = "DamagedByName", ResourceType = typeof(resxDbFields))]
        public string DamagedByName { get; set; }
        [Display(Name = "DamagedBy", ResourceType = typeof(resxDbFields))]
        public string DamagedBy { get; set; }





        public string Comments { get; set; }
        public byte[] dataItemOutputDetailDamagedTrackRowVersion { get; set; }
        public bool Active { get; set; }

    }

    public class WarehouseModelDamageMovementsUpdateModel
    {


        public string EmptyColumn { get; set; }
        [Display(Name = "ItemOutputDetailDamagedTrackGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemOutputDetailDamagedTrackGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]

        [Display(Name = "DamagedTypeGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid DamagedTypeGUID { get; set; }

        [Display(Name = "ItemInputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemInputDetailGUID { get; set; }

        [Display(Name = "ItemOutputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemOutputDetailGUID { get; set; }



        [Display(Name = "ItemOutputGUID", ResourceType = typeof(resxDbFields))]

        public Nullable<System.Guid> ItemOutputGUID { get; set; }

        [Display(Name = "LocationGUID", ResourceType = typeof(resxDbFields))]

        public Nullable<System.Guid> LocationGUID { get; set; }




        [Display(Name = "DamagedByNameGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DamagedByNameGUID { get; set; }
        [Display(Name = "DamagedByGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DamagedByGUID { get; set; }

        [Display(Name = "DamagedByName", ResourceType = typeof(resxDbFields))]
        public string DamagedByName { get; set; }
        [Display(Name = "DamagedBy", ResourceType = typeof(resxDbFields))]
        public string DamagedBy { get; set; }

        [Display(Name = "DocumentReference", ResourceType = typeof(resxDbFields))]
        public string DocumentReference { get; set; }

        [Display(Name = "DamagedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? DamagedDate { get; set; }

        [Display(Name = "IncidentLocationOccureed", ResourceType = typeof(resxDbFields))]
        public string IncidentLocationOccureed { get; set; }

        [Display(Name = "PresentLocation", ResourceType = typeof(resxDbFields))]
        public string PresentLocation { get; set; }
        [Display(Name = "DamagedReason", ResourceType = typeof(resxDbFields))]
        public string DamagedReason { get; set; }







        public string Comments { get; set; }
        public byte[] dataItemOutputDetailDamagedTrackRowVersion { get; set; }
        public bool Active { get; set; }



    }

    #region Reserv
    public class WarehouseModelReservationMovementDataTableModel
    {

        public string EmptyColumn { get; set; }
        [Display(Name = "ItemReservationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemReservationGUID { get; set; }

        [Display(Name = "ItemInputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemInputDetailGUID { get; set; }


        [Display(Name = "ReservationStartDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ReservationStartDate { get; set; }

        [Display(Name = "ReservationEndDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ReservationEndDate { get; set; }



        [Display(Name = "ReservedForGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ReservedForGUID { get; set; }

        [Display(Name = "ReservedNameForGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ReservedNameForGUID { get; set; }




        [Display(Name = "ReservedForGUID", ResourceType = typeof(resxDbFields))]
        public string ReservedFor { get; set; }

        [Display(Name = "ReservedNameForGUID", ResourceType = typeof(resxDbFields))]
        public string ReservedNameFor { get; set; }
        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]

        public Guid CreatedByGUID { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CreatedDate { get; set; }
        [Display(Name = "IsPendingReserved", ResourceType = typeof(resxDbFields))]

        public bool? IsPendingReserved { get; set; }



        public string Comments { get; set; }
        public byte[] dataItemReservationRowVersion { get; set; }
        public bool Active { get; set; }

    }
    public class WarehouseModelReservationMovementsUpdateModel
    {

        public string EmptyColumn { get; set; }
        [Display(Name = "ItemReservationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemReservationGUID { get; set; }

        [Display(Name = "ItemInputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemInputDetailGUID { get; set; }


        [Display(Name = "ReservationStartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ReservationStartDate { get; set; }

        [Display(Name = "ReservationEndDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ReservationEndDate { get; set; }



        [Display(Name = "ReservedForGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ReservedForGUID { get; set; }

        [Display(Name = "ReservedNameForGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ReservedNameForGUID { get; set; }




        [Display(Name = "ReservedForGUID", ResourceType = typeof(resxDbFields))]
        public string ReservedFor { get; set; }

        [Display(Name = "ReservedNameForGUID", ResourceType = typeof(resxDbFields))]
        public string ReservedNameFor { get; set; }
        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]

        public Guid CreatedByGUID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreatedDate { get; set; }
        [Display(Name = "IsPendingReserved", ResourceType = typeof(resxDbFields))]

        public bool? IsPendingReserved { get; set; }



        public string Comments { get; set; }
        public byte[] dataItemReservationRowVersion { get; set; }
        public bool Active { get; set; }

    }
    #endregion
    #endregion

    #region Dashboard

    public class DashboardItemVM
    {
        public DashboardItemVM()
        {
            items = new List<ItemsVM>();
        }
        public int TotalDelayItems { get; set; }

        public int TotalExpiredItems { get; set; }

        public int TotalDepreciatedItems { get; set; }
        public int TotalPendingConfirmationdItems { get; set; }

        public List<ItemsVM> items { get; set; }


    }

    public class ItemsVM
    {
        public ItemsVM()
        {
            models = new List<ModelVM>();
        }
        public string ItemName { get; set; }
        public Guid? WarehouseItemGUID { get; set; }

        public int? TotalAvaiable { get; set; }
        public int? TotalItems { get; set; }

        public List<ModelVM> models { get; set; }

    }

    public class ModelVM
    {
        public string ModelName { get; set; }
        public string BrandName { get; set; }
        public Guid? BrandGUID { get; set; }
        public int? TotalItems { get; set; }
        public int? TotalAvaiable { get; set; }
        public int? TotalDamaged { get; set; }
        public int? TotalLongTerm { get; set; }

        public int? TotalShortTerm { get; set; }

        public Guid WarehouseItemModelGUID { get; set; }

    }
    #endregion

    #region DashBoardVM

    public class DashboardVMFilter
    {
        public List<Guid> brandGuids { get; set; }
        public List<Guid> itemGuids { get; set; }
        public List<Guid> itemClassificationGuids { get; set; }
        public List<Guid> ItemServiceGUIDs { get; set; }
        public List<Guid> DeliveryStatusGUIDs { get; set; }
        public List<Guid> warehouseGuids { get; set; }

        public List<Guid> WarehouseLocationGUIDs { get; set; }
    }
    #endregion

    #region org chart 
    public class ItemorgChart
    {
        public int id { get; set; }
        public Nullable<int> parentID { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string visa { get; set; }
        public string image { get; set; }
    }
    #endregion

    #region Item Custdioan

    public class ItemCustodianVM
    {
        public string ItemName { get; set; }
        public string ModelName { get; set; }
        public string BarcodeNumber { get; set; }
        public string SerialNumber { get; set; }
        public string GSM { get; set; }
        public string IMEI1 { get; set; }
        public string MSRPID { get; set; }
        public Guid ItemInputDetailGUID { get; set; }
    }
    #endregion
    public class ItemWarehouseInformationDetail
    {
        public string Warehousename { get; set; }
        public int? totalItems { get; set; }
        public int? totalAvailable { get; set; }
        public int? totalCustody { get; set; }
        public int? totalDamaged { get; set; }
        public int? totalLost { get; set; }
        public int? totalGS { get; set; }
        public int? totalAvailableNew { get; set; }
        public int? totalAvailablUsed { get; set; }
    }

    #region Item Track flow
    public class WarehouseModelTrackFlowMovementDataTableModel
    {
        public string CreatedByGUID { get; set; }
        public Guid? ItemInputDetailGUID { get; set; }
        public string Comments { get; set; }
        public string Custodian { get; set; }
        public string ItemModelWarehouseGUID { get; set; }
        public string LastCustdianNameGUID { get; set; }
        public string ItemDescription { get; set; }
        public string WarehouseItemClassificationGUID { get; set; }
        public string WarehouseItemGUID { get; set; }

        public string BrandGUID { get; set; }

        public string WarehouseOwnerGUID { get; set; }

        public string IME1 { get; set; }

        public string CustodianStaffGUID { get; set; }
        public string CustodianWarehouseGUID { get; set; }

        public string WarehouseLocationGUID { get; set; }
        public string Governorate { get; set; }
        public string LastCustdianGUID { get; set; }
        [Display(Name = "LastLocation", ResourceType = typeof(resxDbFields))]
        public string LastLocation { get; set; }
        [Display(Name = "ItemStatusGUID", ResourceType = typeof(resxDbFields))]
        public string ItemStatus { get; set; }
        [Display(Name = "ExpectedStartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ExpectedStartDate { get; set; }
        [Display(Name = "ExpectedReturenedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ExpectedReturenedDate { get; set; }
        [Display(Name = "ActualReturenedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ActualReturenedDate { get; set; }


        [Display(Name = "ReleasedBy", ResourceType = typeof(resxDbFields))]
        public string ReleasedByName { get; set; }
        [Display(Name = "OutputCustodianName", ResourceType = typeof(resxDbFields))]
        public string OutputCustodianName { get; set; }
        [Display(Name = "OutputCustodian", ResourceType = typeof(resxDbFields))]
        public string OutputCustodian { get; set; }
        [Display(Name = "IsLastAction", ResourceType = typeof(resxDbFields))]
        public bool? IsLastAction { get; set; }
        [Display(Name = "FlowTypeGUID", ResourceType = typeof(resxDbFields))]
        public string FlowTypeName { get; set; }
        [Display(Name = "CreateBy", ResourceType = typeof(resxDbFields))]
        public string FlowCreatedByName { get; set; }
        [Display(Name = "IsAvaliable", ResourceType = typeof(resxDbFields))]
        public bool? IsAvaliable { get; set; }
        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        public DateTime? FlowCreatedDate { get; set; }

        [Display(Name = "ItemOutputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ItemOutputDetailGUID { get; set; }
        [Display(Name = "ItemOutputDetailFlowGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ItemOutputDetailFlowGUID { get; set; }
        [Display(Name = "ModelDescription", ResourceType = typeof(resxDbFields))]
        public string ModelDescription { get; set; }
        [Display(Name = "LastCustdianNameGUID", ResourceType = typeof(resxDbFields))]
        public string LastCustodianName { get; set; }
        [Display(Name = "LastCustodian", ResourceType = typeof(resxDbFields))]
        public string LastCustodian { get; set; }


        [Display(Name = "ItemInputGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemInputGUID { get; set; }



        [Display(Name = "ItemOutputGUID", ResourceType = typeof(resxDbFields))]

        public Nullable<System.Guid> ItemOutputGUID { get; set; }

        [Display(Name = "IssuedBy", ResourceType = typeof(resxDbFields))]
        public string IssuedBy { get; set; }

        [Display(Name = "IssuedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? IssuedDate { get; set; }


        [Display(Name = "ReturnedBy", ResourceType = typeof(resxDbFields))]
        public string ReturnedBy { get; set; }

        [Display(Name = "ReturnedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ReturnedDate { get; set; }


        //[Display(Name = "RequestType", ResourceType = typeof(resxDbFields))]
        //public string RequestType { get; set; }
        //[Display(Name = "RequestedQunatity", ResourceType = typeof(resxDbFields))]
        //public int? RequestedQunatity { get; set; }


        [Display(Name = "BarcodeNumber", ResourceType = typeof(resxDbFields))]
        public string BarcodeNumber { get; set; }
        [Display(Name = "SerialNumber", ResourceType = typeof(resxDbFields))]
        public string SerialNumber { get; set; }

        [Display(Name = "IME1", ResourceType = typeof(resxDbFields))]
        public string IMEI { get; set; }
        [Display(Name = "GSM", ResourceType = typeof(resxDbFields))]
        public string GSM { get; set; }
        [Display(Name = "MAC", ResourceType = typeof(resxDbFields))]
        public string MAC { get; set; }
        [Display(Name = "LastFlow", ResourceType = typeof(resxDbFields))]
        public string LastFlow { get; set; }

        [Display(Name = "PartNumber", ResourceType = typeof(resxDbFields))]
        public string PartNumber { get; set; }


    }

    #endregion

    #region Calander
    public class WarehouseCalanderVM
    {

        public string FlowCreatedByName { get; set; }
        public Guid? ItemOutputDetailFlowGUID { get; set; }
        [Display(Name = "LocationDescription", ResourceType = typeof(resxDbFields))]

        public string LocationDescription { get; set; }
        [Display(Name = "WarehouseItemDescription", ResourceType = typeof(resxDbFields))]
        public string WarehouseItemDescription { get; set; }

        [Display(Name = "ParentItemModelWarehouseGUID", ResourceType = typeof(resxDbFields))]

        public string EmptyColumn { get; set; }
        public string ModelStatus { get; set; }
        public string RequestStatus { get; set; }

        [Display(Name = "ItemOutputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemOutputDetailGUID { get; set; }

        [Display(Name = "ItemInputGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemInputGUID { get; set; }



        [Display(Name = "ItemOutputGUID", ResourceType = typeof(resxDbFields))]

        public Nullable<System.Guid> ItemOutputGUID { get; set; }

        [Display(Name = "IssuedBy", ResourceType = typeof(resxDbFields))]
        public string IssuedBy { get; set; }

        [Display(Name = "IssuedDate", ResourceType = typeof(resxDbFields))]
        public DateTime? IssuedDate { get; set; }


        [Display(Name = "ReturnedBy", ResourceType = typeof(resxDbFields))]
        public string ReturnedBy { get; set; }

        [Display(Name = "ReturnedDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ReturnedDate { get; set; }


        [Display(Name = "RequestType", ResourceType = typeof(resxDbFields))]
        public string RequestType { get; set; }
        [Display(Name = "RequestedQunatity", ResourceType = typeof(resxDbFields))]
        public int? RequestedQunatity { get; set; }


        [Display(Name = "BarcodeNumber", ResourceType = typeof(resxDbFields))]
        public string BarcodeNumber { get; set; }
        [Display(Name = "SerialNumber", ResourceType = typeof(resxDbFields))]
        public string SerialNumber { get; set; }

        [Display(Name = "IME1", ResourceType = typeof(resxDbFields))]
        public string IME1 { get; set; }

        //[Display(Name = "PartNumber", ResourceType = typeof(resxDbFields))]
        //public string PartNumber { get; set; }


        public DateTime? ExpectedStartDate { get; set; }
        public DateTime? ExpectedReturenedDate { get; set; }
        public DateTime? ActualReturenedDate { get; set; }

        public string Comments { get; set; }
        public byte[] dataItemOutputDetailRowVersion { get; set; }
        public bool Active { get; set; }

        public bool? IsAvaliable { get; set; }
        public string CustodianType { get; set; }
        [Display(Name = "Custodian", ResourceType = typeof(resxDbFields))]
        public string Custodian { get; set; }
        public string ReleaseType { get; set; }
        public int? Qunatity { get; set; }
        public string DeliveryStatus { get; set; }
        public DateTime DeliveryActionDate { get; set; }
    }
    #endregion

    public class UploadDamagedItemVM
    {
        public HttpPostedFileBase UploadDamagedReport { get; set; }
        public Guid ItemInputDetailGUID { get; set; }
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
    #region Calander
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
    #endregion
    #region Consumable Item
    public class ConsumableOverviewItemsDataTableModel
    {



        [Display(Name = "WarehouseItemClassificationGUID", ResourceType = typeof(resxDbFields))]
        public string WarehouseItemClassificationGUID { get; set; }

        [Display(Name = "ItemModelWarehouseGUID", ResourceType = typeof(resxDbFields))]
        public Guid ItemModelWarehouseGUID { get; set; }
        [Display(Name = "WarehouseItemModelGUID", ResourceType = typeof(resxDbFields))]
        public string WarehouseItemModelGUID { get; set; }
        [Display(Name = "WarehouseGUID", ResourceType = typeof(resxDbFields))]
        public string WarehouseGUID { get; set; }
        [Display(Name = "TotalAvaiable", ResourceType = typeof(resxDbFields))]
        public int? TotalAvaiable { get; set; }
        [Display(Name = "TotalEntry", ResourceType = typeof(resxDbFields))]
        public int? TotalEntry { get; set; }
        [Display(Name = "TotalRelease", ResourceType = typeof(resxDbFields))]
        public int? TotalRelease { get; set; }
        [Display(Name = "ModelDescription", ResourceType = typeof(resxDbFields))]
        public string ModelDescription { get; set; }

        [Display(Name = "WarehouseDescription", ResourceType = typeof(resxDbFields))]
        public string WarehouseDescription { get; set; }



        [Display(Name = "ItemClassification", ResourceType = typeof(resxDbFields))]
        public string WarehouseItemClassificationDescription { get; set; }

        [Display(Name = "ItemSubClassification", ResourceType = typeof(resxDbFields))]
        public string WarehouseItemGUID { get; set; }

        [Display(Name = "WarehouseItemDescription", ResourceType = typeof(resxDbFields))]
        public string WarehouseItemDescription { get; set; }

        public bool? Active { get; set; }
        public byte[] codeItemModelWarehouseRowVersion { get; set; }
    }
    #endregion

    #region Consumable Item 


    public class ConsumableItemInputDetailDataTable
    {
        [Display(Name = "ItemInputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemInputDetailGUID { get; set; }

        [Display(Name = "CreatedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreatedDate { get; set; }
        [Display(Name = "SourceName", ResourceType = typeof(resxDbFields))]
        public string SourceName { get; set; }
        [Display(Name = "BillNumber", ResourceType = typeof(resxDbFields))]
        public string BillNumber { get; set; }

        [Display(Name = "ItemInputGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemInputGUID { get; set; }

        [Display(Name = "ModelDescription", ResourceType = typeof(resxDbFields))]
        public string ModelDescription { get; set; }

        [Display(Name = "ItemModelWarehouseGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemModelWarehouseGUID { get; set; }

        [Display(Name = "Qunatity", ResourceType = typeof(resxDbFields))]
        public int? Qunatity { get; set; }
        [Display(Name = "WarehouseOwnerGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> WarehouseOwnerGUID { get; set; }

        [Display(Name = "LastWarehouseLocationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> LastWarehouseLocationGUID { get; set; }

        public bool Active { get; set; }

        public byte[] dataItemInputDetailRowVersion { get; set; }
    }
    public class ConsumableItemDetailModel
    {
        [Display(Name = "ItemInputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Guid ItemInputDetailGUID { get; set; }
        [Display(Name = "ItemInputGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> ItemInputGUID { get; set; }
        [Display(Name = "ItemModelWarehouseGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> ItemModelWarehouseGUID { get; set; }
        [Display(Name = "Qunatity", ResourceType = typeof(resxDbFields))]
        public int Qunatity { get; set; }
        public bool Active { get; set; }

        [Display(Name = "WarehouseOwnerGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> UserWarhouseGUID { get; set; }

        public byte[] dataItemInputDetailRowVersion { get; set; }
    }

    public class ConsumableItemReleaseDetailDataTable
    {

        [Display(Name = "WarehouseLocationDescription", ResourceType = typeof(resxDbFields))]
        public string WarehouseLocationDescription { get; set; }

        [Display(Name = "ItemModelWarehouseGUID", ResourceType = typeof(resxDbFields))]
        public string ItemModelWarehouseGUID { get; set; }

        [Display(Name = "WarehouseLocationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> WarehouseLocationGUID { get; set; }


        public Guid CreatedByGUID { get; set; }

        [Display(Name = "CreatedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreatedDate { get; set; }


        [Display(Name = "OutputDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? OutputDate { get; set; }


        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemOutputGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemOutputGUID { get; set; }


        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemOutputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemOutputDetailGUID { get; set; }


        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemInputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemInputDetailGUID { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemRequestTypeGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemRequestTypeGUID { get; set; }


        [Display(Name = "ModelDescription", ResourceType = typeof(resxDbFields))]
        public string ModelDescription { get; set; }



        //[Display(Name = "PartNumber", ResourceType = typeof(resxDbFields))]
        //public string PartNumber { get; set; }

        [Display(Name = "RequestedQunatity", ResourceType = typeof(resxDbFields))]
        public int? RequestedQunatity { get; set; }
        [Display(Name = "ExpectedStartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ExpectedStartDate { get; set; }



        public string Comments { get; set; }
        public byte[] dataItemOutputDetailRowVersion { get; set; }
        public bool Active { get; set; }


    }
    public class ConsumableReleaseModelDetailUpdateModel
    {
        [Display(Name = "UserWarhouseGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> UserWarhouseGUID { get; set; }
        [Display(Name = "WarehouseLocationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> WarehouseLocationGUID { get; set; }

        [Display(Name = "ItemModelWarehouseGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> ItemModelWarehouseGUID { get; set; }
        public Guid CreatedByGUID { get; set; }

        [Display(Name = "CreatedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreatedDate { get; set; }


        [Display(Name = "OutputDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? OutputDate { get; set; }


        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemOutputGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemOutputGUID { get; set; }


        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemOutputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemOutputDetailGUID { get; set; }


        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemInputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemInputDetailGUID { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemRequestTypeGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemRequestTypeGUID { get; set; }





        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]



        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]

        [Display(Name = "ModelDescription", ResourceType = typeof(resxDbFields))]
        public string ModelDescription { get; set; }



        //[Display(Name = "PartNumber", ResourceType = typeof(resxDbFields))]
        //public string PartNumber { get; set; }

        [Display(Name = "RequestedQunatity", ResourceType = typeof(resxDbFields))]
        public int? RequestedQunatity { get; set; }
        [Display(Name = "ExpectedStartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ExpectedStartDate { get; set; }



        public string Comments { get; set; }
        public byte[] dataItemOutputDetailRowVersion { get; set; }
        public bool Active { get; set; }


    }

    public class WarehouseConsumableItemReleaseDetailInfoIDataTable
    {


        [Display(Name = "ParentItemModelWarehouseGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ParentItemModelWarehouseGUID { get; set; }
        public string EmptyColumn { get; set; }
        public string ModelStatus { get; set; }
        public string RequestStatus { get; set; }

        [Display(Name = "ItemOutputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemOutputDetailGUID { get; set; }

        [Display(Name = "ItemInputGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemInputGUID { get; set; }



        [Display(Name = "ItemOutputGUID", ResourceType = typeof(resxDbFields))]

        public Nullable<System.Guid> ItemOutputGUID { get; set; }

        [Display(Name = "IssuedBy", ResourceType = typeof(resxDbFields))]
        public string IssuedBy { get; set; }

        [Display(Name = "IssuedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? IssuedDate { get; set; }


        [Display(Name = "ReturnedBy", ResourceType = typeof(resxDbFields))]
        public string ReturnedBy { get; set; }

        [Display(Name = "ReturnedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ReturnedDate { get; set; }


        [Display(Name = "RequestType", ResourceType = typeof(resxDbFields))]
        public string RequestType { get; set; }
        [Display(Name = "RequestedQunatity", ResourceType = typeof(resxDbFields))]
        public int? RequestedQunatity { get; set; }


        [Display(Name = "BarcodeNumber", ResourceType = typeof(resxDbFields))]
        public string BarcodeNumber { get; set; }
        [Display(Name = "SerialNumber", ResourceType = typeof(resxDbFields))]
        public string SerialNumber { get; set; }

        [Display(Name = "IME1", ResourceType = typeof(resxDbFields))]
        public string IME1 { get; set; }

        //[Display(Name = "PartNumber", ResourceType = typeof(resxDbFields))]
        //public string PartNumber { get; set; }


        public DateTime? ExpectedStartDate { get; set; }
        public DateTime? ExpectedReturenedDate { get; set; }
        public DateTime? ActualReturenedDate { get; set; }

        public string Comments { get; set; }
        public byte[] dataItemOutputDetailRowVersion { get; set; }
        public bool Active { get; set; }

        public bool? IsAvaliable { get; set; }
        public string CustodianType { get; set; }
        public string Custodian { get; set; }
        public string ReleaseType { get; set; }
        public int? Qunatity { get; set; }
        public string DeliveryStatus { get; set; }
        public DateTime DeliveryActionDate { get; set; }
    }


    #endregion

    #region Con
    public class ConsumableSingleReleaseModelDetailUpdateModel
    {
        [Display(Name = "OutputDate", ResourceType = typeof(resxDbFields))]
        public DateTime? OutputDate { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RequesterGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> RequesterGUID { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RequesterNameGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> RequesterNameGUID { get; set; }

        [Display(Name = "WarehouseGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> WarehouseGUID { get; set; }


        [Display(Name = "ItemOutputGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ItemOutputGUID { get; set; }
        [Display(Name = "OutputNumber", ResourceType = typeof(resxDbFields))]
        public int? OutputNumber { get; set; }


        public string Comments { get; set; }

        public byte[] dataItemOutputRowVersion { get; set; }

        [Display(Name = "UserWarhouseGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> UserWarhouseGUID { get; set; }
        [Display(Name = "WarehouseLocationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> WarehouseLocationGUID { get; set; }

        [Display(Name = "ItemModelWarehouseGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> ItemModelWarehouseGUID { get; set; }
        public Guid CreatedByGUID { get; set; }

        [Display(Name = "CreatedDate", ResourceType = typeof(resxDbFields))]
        public DateTime? CreatedDate { get; set; }



        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemOutputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemOutputDetailGUID { get; set; }


        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemInputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemInputDetailGUID { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemRequestTypeGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemRequestTypeGUID { get; set; }





        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]



        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]

        [Display(Name = "ModelDescription", ResourceType = typeof(resxDbFields))]
        public string ModelDescription { get; set; }



        //[Display(Name = "PartNumber", ResourceType = typeof(resxDbFields))]
        //public string PartNumber { get; set; }

        [Display(Name = "RequestedQunatity", ResourceType = typeof(resxDbFields))]
        public int? RequestedQunatity { get; set; }
        [Display(Name = "ExpectedStartDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ExpectedStartDate { get; set; }




        public byte[] dataItemOutputDetailRowVersion { get; set; }
        public bool Active { get; set; }


    }
    public class ConsumableItemPendingConfirmationDataTableModel
    {
        [Display(Name = "ItemOutputDetailFlowGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> ItemOutputDetailFlowGUID { get; set; }
        [Display(Name = "RequestedQunatity", ResourceType = typeof(resxDbFields))]
        public int? RequestedQunatity { get; set; }
        [Display(Name = "RequesterGUID", ResourceType = typeof(resxDbFields))]

        public string RequesterGUID { get; set; }
        [Display(Name = "RequesterNameGUID", ResourceType = typeof(resxDbFields))]

        public string RequesterNameGUID { get; set; }
        [Display(Name = "ItemOutputDetailGUID", ResourceType = typeof(resxDbFields))]

        public Nullable<Guid> ItemOutputDetailGUID { get; set; }
        [Display(Name = "WarehouseItemClassificationGUID", ResourceType = typeof(resxDbFields))]
        public string WarehouseItemClassificationGUID { get; set; }

        [Display(Name = "ItemModelWarehouseGUID", ResourceType = typeof(resxDbFields))]
        public Guid ItemModelWarehouseGUID { get; set; }
        [Display(Name = "WarehouseItemModelGUID", ResourceType = typeof(resxDbFields))]
        public string WarehouseItemModelGUID { get; set; }
        [Display(Name = "WarehouseGUID", ResourceType = typeof(resxDbFields))]
        public string WarehouseGUID { get; set; }
        [Display(Name = "TotalAvaiable", ResourceType = typeof(resxDbFields))]
        public int? TotalAvaiable { get; set; }
        [Display(Name = "TotalEntry", ResourceType = typeof(resxDbFields))]
        public int? TotalEntry { get; set; }
        [Display(Name = "TotalRelease", ResourceType = typeof(resxDbFields))]
        public int? TotalRelease { get; set; }
        [Display(Name = "ModelDescription", ResourceType = typeof(resxDbFields))]
        public string ModelDescription { get; set; }

        [Display(Name = "WarehouseDescription", ResourceType = typeof(resxDbFields))]
        public string WarehouseDescription { get; set; }

        [Display(Name = "Custodian", ResourceType = typeof(resxDbFields))]
        public string Custodian { get; set; }

        [Display(Name = "OutputCustodianName", ResourceType = typeof(resxDbFields))]
        public string OutputCustodianName { get; set; }


        [Display(Name = "ItemClassification", ResourceType = typeof(resxDbFields))]
        public string WarehouseItemClassificationDescription { get; set; }

        [Display(Name = "WarehouseItemGUID", ResourceType = typeof(resxDbFields))]
        public string WarehouseItemGUID { get; set; }

        [Display(Name = "WarehouseItemDescription", ResourceType = typeof(resxDbFields))]
        public string WarehouseItemDescription { get; set; }

        public bool? Active { get; set; }
        public byte[] dataItemOutputDetailFlowRowVersion { get; set; }
    }
    #endregion

    public class EntryModelSingleConsumableUpdateModel
    {
        [Display(Name = "PONumber", ResourceType = typeof(resxDbFields))]
        public string BillNumber { get; set; }

        [Display(Name = "InputDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? InputDate { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SourceGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid SourceGUID { get; set; }



        [Display(Name = "SourceNameGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> SourceNameGUID { get; set; }

        public Guid CreatedByGUID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "ItemInputGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ItemInputGUID { get; set; }
        public int? SequenceNumber { get; set; }

        public bool Active { get; set; }
        [Display(Name = "ItemInputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Guid ItemInputDetailGUID { get; set; }

        [Display(Name = "ItemModelWarehouseGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> ItemModelWarehouseGUID { get; set; }
        [Display(Name = "Qunatity", ResourceType = typeof(resxDbFields))]
        public int Qunatity { get; set; }


        [Display(Name = "WarehouseOwnerGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> UserWarhouseGUID { get; set; }

        public byte[] dataItemInputDetailRowVersion { get; set; }

        public byte[] dataItemInputRowVersion { get; set; }
    }

    #region Verification item

    public class WarehouseItemVerficationPeriodDataTableModel
    {
        [Display(Name = "ItemVerificationPeriodGUID", ResourceType = typeof(resxDbFields))]
        public string ItemVerificationPeriodGUID { get; set; }

        [Display(Name = "OrderId", ResourceType = typeof(resxDbFields))]
        public int? OrderId { get; set; }


        [Display(Name = "VerificationPeriodName", ResourceType = typeof(resxDbFields))]
        public string VerificationPeriodName { get; set; }
        [Display(Name = "IsLastPeriod", ResourceType = typeof(resxDbFields))]
        public bool? IsLastPeriod { get; set; }
        [Display(Name = "IsClosed", ResourceType = typeof(resxDbFields))]
        public bool? IsClosed { get; set; }


        [Display(Name = "WarehouseName", ResourceType = typeof(resxDbFields))]
        public string WarehouseName { get; set; }

        [Display(Name = "VerificationStartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? VerificationStartDate { get; set; }
        [Display(Name = "VerificationEndDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? VerificationEndDate { get; set; }
        public bool? Active { get; set; }
        public byte[] dataItemVerificationPeriodVersion { get; set; }
    }
    public class WarehouseItemVerficationPeriodModel
    {
        [Display(Name = "ItemVerificationPeriodGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> ItemVerificationPeriodGUID { get; set; }

        [Display(Name = "OrderId", ResourceType = typeof(resxDbFields))]
        public int? OrderId { get; set; }


        [Display(Name = "VerificationPeriodName", ResourceType = typeof(resxDbFields))]
        public string VerificationPeriodName { get; set; }
        [Display(Name = "IsLastPeriod", ResourceType = typeof(resxDbFields))]
        public bool? IsLastPeriod { get; set; }
        [Display(Name = "IsClosed", ResourceType = typeof(resxDbFields))]
        public bool? IsClosed { get; set; }


        [Display(Name = "WarehouseName", ResourceType = typeof(resxDbFields))]
        public string WarehouseName { get; set; }

        [Display(Name = "VerificationStartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? VerificationStartDate { get; set; }
        [Display(Name = "VerificationEndDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? VerificationEndDate { get; set; }
        public bool Active { get; set; }
        public byte[] dataItemVerificationPeriodVersion { get; set; }
    }

    public class TrackWarehouseVeriifationItem
    {
        public Nullable<Guid> VerificationWarehousePeriodGUID { get; set; }
        [Display(Name = "RequesterNameGUID", ResourceType = typeof(resxDbFields))]
        public string WarehouseName { get; set; }
        [Display(Name = "TotalItem", ResourceType = typeof(resxDbFields))]
        public int? TotalItem { get; set; }
        [Display(Name = "TotalItemVerified", ResourceType = typeof(resxDbFields))]
        public int? TotalItemVerified { get; set; }
        [Display(Name = "TotalItemNotVerified", ResourceType = typeof(resxDbFields))]

        public int? TotalItemNotVerified { get; set; }
        public byte[] dataItemVerificationWarehousePeriodVersion { get; set; }
    }
    public class WarehouseItemVerficationDataTableModel
    {
        [Display(Name = "ItemVerificationPeriodDetailGUID", ResourceType = typeof(resxDbFields))]
        public string ItemVerificationPeriodDetailGUID { get; set; }

        [Display(Name = "OrderId", ResourceType = typeof(resxDbFields))]
        public int? OrderId { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Display(Name = "VerificationPeriodName", ResourceType = typeof(resxDbFields))]
        public string VerificationPeriodName { get; set; }
        [Display(Name = "IsLastPeriod", ResourceType = typeof(resxDbFields))]
        public bool? IsLastPeriod { get; set; }
        [Display(Name = "IsClosed", ResourceType = typeof(resxDbFields))]
        public bool? IsClosed { get; set; }


        [Display(Name = "WarehouseName", ResourceType = typeof(resxDbFields))]
        public string WarehouseName { get; set; }

        [Display(Name = "VerifyDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? VerifyDate { get; set; }
        [Display(Name = "VerifyBy", ResourceType = typeof(resxDbFields))]
        public string VerifyBy { get; set; }
        public bool? Active { get; set; }
        public byte[] dataItemVerificationPeriodDetailVersion { get; set; }
    }
    #endregion

    #region search
    public class ReleaseMultiSearchUpdateModel
    {

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Display(Name = "RequesterNameGUID", ResourceType = typeof(resxDbFields))]

        public string RequesterNameGUID { get; set; }







    }
    #endregion

    #region Track History
    public class WarehouseModelTrackStaffHistoricalDataTableModel
    {
        [Display(Name = "RequesterNameGUID", ResourceType = typeof(resxDbFields))]
        public string RequesterNameGUID { get; set; }
        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string Status { get; set; }
        [Display(Name = "LicenseStartDate", ResourceType = typeof(resxDbFields))]
        public DateTime? LicenseStartDate { get; set; }

        [Display(Name = "LicenseExpiryDate", ResourceType = typeof(resxDbFields))]
        public DateTime? LicenseExpiryDate { get; set; }

        [Display(Name = "IssueDate", ResourceType = typeof(resxDbFields))]
        public DateTime? IssueDate { get; set; }
        [Display(Name = "ReturnDate", ResourceType = typeof(resxDbFields))]
        public DateTime? ReturnDate { get; set; }



        [Display(Name = "ItemOutputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Guid ItemOutputDetailGUID { get; set; }

        [Display(Name = "dataItemOutputDetailRowVersion", ResourceType = typeof(resxDbFields))]
        public byte[] dataItemOutputDetailRowVersion { get; set; }


        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string CommentsReleas { get; set; }
        [Display(Name = "ItemServiceStatusGUID", ResourceType = typeof(resxDbFields))]
        public string ServiceItemStatus { get; set; }

        [Display(Name = "VerificationStatusGUID", ResourceType = typeof(resxDbFields))]
        public string VerificationStatusGUID { get; set; }

        [Display(Name = "ItemConditionGUID", ResourceType = typeof(resxDbFields))]
        public string ItemCondition { get; set; }
        [Display(Name = "ItemConditionGUID", ResourceType = typeof(resxDbFields))]
        public string ItemConditionGUID { get; set; }

        [Display(Name = "ItemServiceStatusGUID", ResourceType = typeof(resxDbFields))]
        public string ItemServiceStatusGUID { get; set; }

        public Nullable<Guid> PurposeofuseGUID { get; set; }

        public string Purposeofuse { get; set; }
        [Display(Name = "AcquisitionDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? AcquisitionDate { get; set; }

        [Display(Name = "DeliveryStatus", ResourceType = typeof(resxDbFields))]
        public string DeliveryStatus { get; set; }
        public string ModelStatusGUID { get; set; }

        public string CustodianStaffGUID { get; set; }
        public string CustodianWarehouseGUID { get; set; }
        public string DeliveryStatusGUID { get; set; }

        public string WarehouseLocationGUID { get; set; }

        public string LastCustdianGUID { get; set; }


        public string CreatedByGUID { get; set; }

        [Display(Name = "CreatedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "WarehouseItemGUID", ResourceType = typeof(resxDbFields))]
        public string WarehouseItemGUID { get; set; }
        public Guid LastCustodianType { get; set; }

        public string LastCustodianName { get; set; }


        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemInputDetailGUID", ResourceType = typeof(resxDbFields))]

        public Nullable<System.Guid> ItemInputDetailGUID { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ItemInputGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemInputGUID { get; set; }

        [Display(Name = "WarehouseItemClassificationGUID", ResourceType = typeof(resxDbFields))]
        public string WarehouseItemClassificationGUID { get; set; }
        [Display(Name = "BrandGUID", ResourceType = typeof(resxDbFields))]
        public string BrandGUID { get; set; }


        [Display(Name = "ItemStatusGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemStatusGUID { get; set; }

        [Display(Name = "ItemModelWarehouseGUID", ResourceType = typeof(resxDbFields))]
        public string ItemModelWarehouseGUID { get; set; }

        [Display(Name = "WarehouseStoreGUID", ResourceType = typeof(resxDbFields))]
        public string WarehouseStoreGUID { get; set; }

        [Display(Name = "WarehouseOwner", ResourceType = typeof(resxDbFields))]
        public string WarehouseOwner { get; set; }

        [Display(Name = "Governorate", ResourceType = typeof(resxDbFields))]
        public string Governorate { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public string CreatedBy { get; set; }

        [Display(Name = "LastVerifiedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? LastVerifiedDate { get; set; }
        [Display(Name = "VerifiedBy", ResourceType = typeof(resxDbFields))]
        public string VerifiedBy { get; set; }



        [Display(Name = "Qunatity", ResourceType = typeof(resxDbFields))]
        public int Qunatity { get; set; }

        [Display(Name = "LocationGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> LocationGUID { get; set; }


        [Display(Name = "PriceTypeGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> PriceTypeGUID { get; set; }

        [Display(Name = "PriceValue", ResourceType = typeof(resxDbFields))]
        public decimal? PriceValue { get; set; }

        [Display(Name = "InventoryStatusGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> InventoryStatusGUID { get; set; }
        [Display(Name = "ItemBrandModelColorGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemBrandModelColorGUID { get; set; }

        public string WarehouseItemClassificationDescription { get; set; }

        public string WarehouseLocationDescription { get; set; }
        public string WarehouseOwnerGUID { get; set; }

        public string Comments { get; set; }

        public byte[] dataItemInputDetailRowVersion { get; set; }

        public byte[] dataItemInputRowVersion { get; set; }



        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]

        [Display(Name = "ModelDescription", ResourceType = typeof(resxDbFields))]
        public string ModelDescription { get; set; }

        [Display(Name = "ItemDescription", ResourceType = typeof(resxDbFields))]
        public string ItemDescription { get; set; }

        [StringLength(50, MinimumLength = 5, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BarcodeNumber", ResourceType = typeof(resxDbFields))]
        public string BarcodeNumber { get; set; }
        [Display(Name = "SerialNumber", ResourceType = typeof(resxDbFields))]
        [StringLength(50, MinimumLength = 5, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public string SerialNumber { get; set; }

        [Display(Name = "IME1", ResourceType = typeof(resxDbFields))]
        [StringLength(500, MinimumLength = 5, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public string IME1 { get; set; }

        [Display(Name = "GSM", ResourceType = typeof(resxDbFields))]
        [StringLength(500, MinimumLength = 5, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public string GSM { get; set; }

        [Display(Name = "MSRPID", ResourceType = typeof(resxDbFields))]
        [StringLength(500, MinimumLength = 5, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(resxHttpErrors))]
        public string MSRPID { get; set; }
        public string MAC { get; set; }



        [Display(Name = "GSMNumber", ResourceType = typeof(resxDbFields))]
        public string GSMNumber { get; set; }

        [Display(Name = "PartNumber", ResourceType = typeof(resxDbFields))]
        public string PartNumber { get; set; }

        [Display(Name = "Custodian", ResourceType = typeof(resxDbFields))]
        public string Custodian { get; set; }

        [Display(Name = "ModelStatus", ResourceType = typeof(resxDbFields))]
        public string ModelStatus { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }
        public byte[] codeWarehouseItemModelLanguageRowVersion { get; set; }


    }
    #endregion

    #region Upload Documents for item
    public class WarehouseItemUploadedDocumentsDataTable
    {


        [Display(Name = "ItemIntpuDetailUploadedDocumentGUID", ResourceType = typeof(resxDbFields))]
        public string ItemIntpuDetailUploadedDocumentGUID { get; set; }



        [Display(Name = "ItemInputDetailGUID", ResourceType = typeof(resxDbFields))]
        public string ItemInputDetailGUID { get; set; }

        [Display(Name = "FileTypeGUID", ResourceType = typeof(resxDbFields))]
        public string FileTypeGUID { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }
        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public string CreatedBy { get; set; }
        [Display(Name = "CreatedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public string CreatedDate { get; set; }


        [Display(Name = "DocumentName", ResourceType = typeof(resxDbFields))]
        public string DocumentName { get; set; }
        [Display(Name = "DocumentOrderId", ResourceType = typeof(resxDbFields))]
        public int? DocumentOrderId { get; set; }
        public byte[] dataItemIntpuDetailUploadedDocumentRowVersion { get; set; }
        public bool Active { get; set; }

    }
    #endregion
    #region STI Contacts

    public class STIContactsDataTable
    {
        [Display(Name = "StaffinformationID", ResourceType = typeof(resxDbFields))]
        public string StaffinformationID { get; set; }

        [Display(Name = "FullName", ResourceType = typeof(resxDbFields))]
        public string FullName { get; set; }


        [Display(Name = "Mobile", ResourceType = typeof(resxDbFields))]
        public string Mobile { get; set; }

        [Display(Name = "EmailAddress", ResourceType = typeof(resxDbFields))]
        public string EmailAddress { get; set; }

        public bool Active { get; set; }



    }
    public class STIContactsUpdateModel
    {
        [Display(Name = "StaffinformationID", ResourceType = typeof(resxDbFields))]
        public int? StaffinformationID { get; set; }

        [Display(Name = "FullName", ResourceType = typeof(resxDbFields))]
        public string FullName { get; set; }
        [Display(Name = "FirstName", ResourceType = typeof(resxDbFields))]
        public string FirstName { get; set; }
        [Display(Name = "FamilyName", ResourceType = typeof(resxDbFields))]
        public string FamilyName { get; set; }

        [Display(Name = "MiddleName", ResourceType = typeof(resxDbFields))]
        public string MiddleName { get; set; }

        [Display(Name = "DepartmentName", ResourceType = typeof(resxDbFields))]
        public string DepartmentName { get; set; }

        [Display(Name = "Governorate", ResourceType = typeof(resxDbFields))]
        public string Governorate { get; set; }

        [Display(Name = "DutyStation", ResourceType = typeof(resxDbFields))]
        public string DutyStation { get; set; }

        [Display(Name = "SupervisorID", ResourceType = typeof(resxDbFields))]
        public string SupervisorID { get; set; }

        [Display(Name = "Address", ResourceType = typeof(resxDbFields))]
        public string Address { get; set; }

        [Display(Name = "Mobile", ResourceType = typeof(resxDbFields))]
        public string Mobile1 { get; set; }


        [Display(Name = "Emailwork", ResourceType = typeof(resxDbFields))]
        public string Emailwork { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        public bool Active { get; set; }



    }

    #endregion

    #region Licence and subscription contracts
    public class LicenseSubscriptionContractDataTableModel
    {
        public System.Guid LicenseSubscriptionContractGUID { get; set; }

        [Display(Name = "ContractClassGUID", ResourceType = typeof(resxDbFields))]
        public string ContractClassGUID { get; set; }

        [Display(Name = "VendorGUID", ResourceType = typeof(resxDbFields))]
        public string VendorGUID { get; set; }

        [Display(Name = "VendorGUID", ResourceType = typeof(resxDbFields))]
        public string VendorName { get; set; }

        [Display(Name = "ContractClassGUID", ResourceType = typeof(resxDbFields))]
        public string ContractClass { get; set; }

        [Display(Name = "DutyStation", ResourceType = typeof(resxDbFields))]
        public string LocationGUID { get; set; }

        [Display(Name = "DutyStation", ResourceType = typeof(resxDbFields))]
        public string Location { get; set; }
        [Display(Name = "ContractNumber", ResourceType = typeof(resxDbFields))]
        public string ContractNumber { get; set; }

        [Display(Name = "ContractCode", ResourceType = typeof(resxDbFields))]
        public string ContractCode { get; set; }

        [Display(Name = "ContrayTypeGUID", ResourceType = typeof(resxDbFields))]
        public string ContractTypeGUID { get; set; }


        [Display(Name = "ContrayTypeGUID", ResourceType = typeof(resxDbFields))]
        public string ContractType { get; set; }

        [Display(Name = "ContractCategoryGUID", ResourceType = typeof(resxDbFields))]
        public string ContractCategoryGUID { get; set; }

        [Display(Name = "ContractCategoryGUID", ResourceType = typeof(resxDbFields))]
        public string ContractCategory { get; set; }
        [Display(Name = "PurchaseDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]

        public DateTime? PurchaseDate { get; set; }
        [Display(Name = "ContractStartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? StartDate { get; set; }
        [Display(Name = "ContractExpiryDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ExpiryDate { get; set; }
        [Display(Name = "RemindDateType", ResourceType = typeof(resxDbFields))]
        public string RemindDateType { get; set; }

        [Display(Name = "RemindValue", ResourceType = typeof(resxDbFields))]
        public int? RemindValue { get; set; }

        [Display(Name = "NextRemindDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? NextRemindDate { get; set; }

        [Display(Name = "BrandGUID", ResourceType = typeof(resxDbFields))]
        public string BrandGUID { get; set; }

        [Display(Name = "BrandGUID", ResourceType = typeof(resxDbFields))]
        public string BrandName { get; set; }

        [Display(Name = "LocalExchangeRate", ResourceType = typeof(resxDbFields))]
        public double? LocalExchangeRate { get; set; }

        [Display(Name = "LocalCurrencyContractCost", ResourceType = typeof(resxDbFields))]
        public double? LocalCurrencyContractCost { get; set; }

        [Display(Name = "DollarCurrencyContractCost", ResourceType = typeof(resxDbFields))]
        public double? DollarCurrencyContractCost { get; set; }



        [Display(Name = "LatestPONumber", ResourceType = typeof(resxDbFields))]
        public string LatestPONumber { get; set; }

        [Display(Name = "FlowStatusGUID", ResourceType = typeof(resxDbFields))]
        public string FlowStatusGUID { get; set; }

        [Display(Name = "LastFlowStatusName", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusName { get; set; }

        [Display(Name = "VendorInformation", ResourceType = typeof(resxDbFields))]
        public string VendorInformation { get; set; }

        [Display(Name = "ContractDescription", ResourceType = typeof(resxDbFields))]
        public string ContractDescription { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }


        public bool Active { get; set; }

        public byte[] dataLicenseSubscriptionContractRowVersion { get; set; }

    }

    public class LicenseSubscriptionContractUpdateModel
    {
        public System.Guid LicenseSubscriptionContractGUID { get; set; }

        [Display(Name = "ContractClassGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ContractClassGUID { get; set; }

        [Display(Name = "VendorGUID", ResourceType = typeof(resxDbFields))]
        public Guid? VendorGUID { get; set; }


        [Display(Name = "DutyStation", ResourceType = typeof(resxDbFields))]
        public Guid? LocationGUID { get; set; }

        [Display(Name = "DutyStation", ResourceType = typeof(resxDbFields))]
        public string Location { get; set; }
        [Display(Name = "ContractNumber", ResourceType = typeof(resxDbFields))]
        public string ContractNumber { get; set; }

        [Display(Name = "ContractCode", ResourceType = typeof(resxDbFields))]
        public string ContractCode { get; set; }

        [Display(Name = "ContrayTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ContractTypeGUID { get; set; }



        [Display(Name = "ContractCategoryGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ContractCategoryGUID { get; set; }

        [Display(Name = "ContractCategoryGUID", ResourceType = typeof(resxDbFields))]
        public string ContractCategory { get; set; }
        [Display(Name = "PurchaseDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? PurchaseDate { get; set; }
        [Display(Name = "ContractStartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? StartDate { get; set; }
        [Display(Name = "ContractExpiryDateLicence", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ExpiryDate { get; set; }
        [Display(Name = "RemindDateType", ResourceType = typeof(resxDbFields))]
        public Guid? RemindDateType { get; set; }

        [Display(Name = "RemindValue", ResourceType = typeof(resxDbFields))]
        public int? RemindValue { get; set; }

        [Display(Name = "NextRemindDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? NextRemindDate { get; set; }

        [Display(Name = "BrandGUID", ResourceType = typeof(resxDbFields))]
        public Guid? BrandGUID { get; set; }

        [Display(Name = "BrandGUID", ResourceType = typeof(resxDbFields))]
        public string BrandName { get; set; }

        [Display(Name = "LocalExchangeRate", ResourceType = typeof(resxDbFields))]
        public double? LocalExchangeRate { get; set; }

        [Display(Name = "LocalCurrencyContractCost", ResourceType = typeof(resxDbFields))]
        public double? LocalCurrencyContractCost { get; set; }

        [Display(Name = "DollarCurrencyContractCost", ResourceType = typeof(resxDbFields))]
        public double? DollarCurrencyContractCost { get; set; }



        [Display(Name = "LatestPONumber", ResourceType = typeof(resxDbFields))]
        public string LatestPONumber { get; set; }

        [Display(Name = "FlowStatusGUID", ResourceType = typeof(resxDbFields))]
        public Guid? FlowStatusGUID { get; set; }

        [Display(Name = "LastFlowStatusName", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusName { get; set; }

        [Display(Name = "VendorInformation", ResourceType = typeof(resxDbFields))]
        public string VendorInformation { get; set; }

        [Display(Name = "ContractDescription", ResourceType = typeof(resxDbFields))]
        public string ContractDescription { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }


        public bool Active { get; set; }

        public byte[] dataLicenseSubscriptionContractRowVersion { get; set; }

    }


    public class WMSPurchaseOrderContractDataTableModel
    {
        [Display(Name = "LicenseSubscriptionContractPOGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid LicenseSubscriptionContractPOGUID { get; set; }

        [Display(Name = "LicenseSubscriptionContractGUID", ResourceType = typeof(resxDbFields))]
        public string LicenseSubscriptionContractGUID { get; set; }

        [Display(Name = "PONumber", ResourceType = typeof(resxDbFields))]
        public string PONumber { get; set; }


        [Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "ExpiryDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ExpiryDate { get; set; }


        [Display(Name = "Description", ResourceType = typeof(resxDbFields))]
        public string PODescription { get; set; }

        [Display(Name = "Price", ResourceType = typeof(resxDbFields))]
        public double? Price { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }


        [Display(Name = "CreateBy", ResourceType = typeof(resxDbFields))]
        public string CreateBy { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "CreateBy", ResourceType = typeof(resxDbFields))]
        public string UpdateBy { get; set; }

        [Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? UpdateDate { get; set; }


        public bool Active { get; set; }

        public byte[] dataLicenseSubscriptionContractPORowVersion { get; set; }

    }

    public class WMSPurchaseOrderContractUpdateModel
    {
        [Display(Name = "LicenseSubscriptionContractPOGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid LicenseSubscriptionContractPOGUID { get; set; }

        [Display(Name = "LicenseSubscriptionContractGUID", ResourceType = typeof(resxDbFields))]
        public Guid? LicenseSubscriptionContractGUID { get; set; }

        [Display(Name = "PONumber", ResourceType = typeof(resxDbFields))]
        public string PONumber { get; set; }


        [Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "ExpiryDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ExpiryDate { get; set; }


        [Display(Name = "Description", ResourceType = typeof(resxDbFields))]
        public string PODescription { get; set; }

        [Display(Name = "Price", ResourceType = typeof(resxDbFields))]
        public double? Price { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }


        [Display(Name = "CreateBy", ResourceType = typeof(resxDbFields))]
        public Guid? CreateByGUID { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "CreateBy", ResourceType = typeof(resxDbFields))]
        public Guid? UpdateByGUID { get; set; }

        [Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? UpdateDate { get; set; }


        public bool Active { get; set; }

        public byte[] dataLicenseSubscriptionContractPORowVersion { get; set; }

    }

    public class WMSLicenseSubscriptionContractPOFileDataTableModel
    {
        [Display(Name = "LicenseSubscriptionContractPOFileGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid LicenseSubscriptionContractPOFileGUID { get; set; }

        [Display(Name = "LicenseSubscriptionContractPOGUID", ResourceType = typeof(resxDbFields))]
        public string LicenseSubscriptionContractPOGUID { get; set; }

        [Display(Name = "FileTypeGUID", ResourceType = typeof(resxDbFields))]
        public string FileTypeGUID { get; set; }
        [Display(Name = "FileTypeGUID", ResourceType = typeof(resxDbFields))]
        public string FileType { get; set; }


        [Display(Name = "FileExtension", ResourceType = typeof(resxDbFields))]
        public string FileExtension { get; set; }

        [Display(Name = "ContractFileName", ResourceType = typeof(resxDbFields))]
        public string ContractFileName { get; set; }


        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
        public string CreateByGUID { get; set; }



        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }


        [Display(Name = "CreateBy", ResourceType = typeof(resxDbFields))]
        public string CreateBy { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "CreateBy", ResourceType = typeof(resxDbFields))]
        public string UpdateBy { get; set; }

        [Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? UpdateDate { get; set; }


        public bool Active { get; set; }

        public byte[] dataLicenseSubscriptionContractPOFileRowVersion { get; set; }

    }


    public class WMSContractSTIItemDataTableModel
    {
        [Display(Name = "LicenseSubscriptionContractSTIItemGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid LicenseSubscriptionContractSTIItemGUID { get; set; }

        [Display(Name = "LicenseSubscriptionContractGUID", ResourceType = typeof(resxDbFields))]
        public string LicenseSubscriptionContractGUID { get; set; }

        [Display(Name = "ItemInputDetailGUID", ResourceType = typeof(resxDbFields))]
        public string ItemInputDetailGUID { get; set; }

        [Display(Name = "ModelName", ResourceType = typeof(resxDbFields))]
        public string ModelName { get; set; }


        [Display(Name = "BarcodeNumber", ResourceType = typeof(resxDbFields))]
        public string BarcodeNumber { get; set; }

        [Display(Name = "SerialNumber", ResourceType = typeof(resxDbFields))]
        public string SerialNumber { get; set; }

        [Display(Name = "GSM", ResourceType = typeof(resxDbFields))]
        public string GSMNumber { get; set; }


        //[Display(Name = "IMEINumber", ResourceType = typeof(resxDbFields))]
        //public string IMEINumber { get; set; }

        [Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "ExpiryDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ExpiryDate { get; set; }


        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Display(Name = "CreateBy", ResourceType = typeof(resxDbFields))]
        public string CreateBy { get; set; }

        [Display(Name = "CreateBy", ResourceType = typeof(resxDbFields))]
        public string CreateByGUID { get; set; }

        [Display(Name = "LicenseSubscriptionContractPOGUID", ResourceType = typeof(resxDbFields))]
        public string LicenseSubscriptionContractPOGUID { get; set; }

        [Display(Name = "PONumber", ResourceType = typeof(resxDbFields))]
        public string PONumber { get; set; }


        [Display(Name = "CreateBy", ResourceType = typeof(resxDbFields))]
        public string UpdateByGUID { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "CreateBy", ResourceType = typeof(resxDbFields))]
        public string UpdateBy { get; set; }

        [Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? UpdateDate { get; set; }


        public bool Active { get; set; }

        public byte[] dataLicenseSubscriptionContractSTIItemRowVersion { get; set; }

    }

    public class WMSContractSTIItemUpdateModel
    {





        [Display(Name = "LicenseSubscriptionContractSTIItemGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid LicenseSubscriptionContractSTIItemGUID { get; set; }

        [Display(Name = "LicenseSubscriptionContractGUID", ResourceType = typeof(resxDbFields))]
        public Guid? LicenseSubscriptionContractGUID { get; set; }

        [Display(Name = "ItemInputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ItemInputDetailGUID { get; set; }





        [Display(Name = "ModelName", ResourceType = typeof(resxDbFields))]
        public string ModelName { get; set; }


        [Display(Name = "BarcodeNumber", ResourceType = typeof(resxDbFields))]
        public string BarcodeNumber { get; set; }

        [Display(Name = "SerialNumber", ResourceType = typeof(resxDbFields))]
        public string SerialNumber { get; set; }

        [Display(Name = "GSM", ResourceType = typeof(resxDbFields))]
        public string GSMNumber { get; set; }


        //[Display(Name = "IMEINumber", ResourceType = typeof(resxDbFields))]
        //public string IMEINumber { get; set; }

        [Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "ExpiryDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ExpiryDate { get; set; }


        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Display(Name = "CreateBy", ResourceType = typeof(resxDbFields))]
        public string CreateBy { get; set; }

        [Display(Name = "CreateBy", ResourceType = typeof(resxDbFields))]
        public Guid? CreateByGUID { get; set; }

        [Display(Name = "LicenseSubscriptionContractPOGUID", ResourceType = typeof(resxDbFields))]
        public Guid? LicenseSubscriptionContractPOGUID { get; set; }

        [Display(Name = "PONumber", ResourceType = typeof(resxDbFields))]
        public string PONumber { get; set; }


        [Display(Name = "UpdateBy", ResourceType = typeof(resxDbFields))]
        public Guid? UpdateByGUID { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "CreateBy", ResourceType = typeof(resxDbFields))]
        public string UpdateBy { get; set; }

        [Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? UpdateDate { get; set; }


        public bool Active { get; set; }

        public byte[] dataLicenseSubscriptionContractSTIItemRowVersion { get; set; }

    }

    public class WMSItemDetail
    {
        public Guid ItemInputDetailGUID { get; set; }
        public string ModelName { get; set; }
        public string BarcodeNumber { get; set; }
        public string SerialNumber { get; set; }

    }

    #endregion

    #region Item Feature Detail
    public class WarehouseItemDetailFeatureDataTableModel
    {

        public string EmptyColumn { get; set; }
        [Display(Name = "ItemInputDetailFeatureGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemInputDetailFeatureGUID { get; set; }

        [Display(Name = "ItemInputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemInputDetailGUID { get; set; }


        [Display(Name = "FeatureTypeValueGUID", ResourceType = typeof(resxDbFields))]
        public string FeatureTypeValueGUID { get; set; }

        [Display(Name = "FeatureTypeGUID", ResourceType = typeof(resxDbFields))]

        public string FeatureTypeGUID { get; set; }

        [Display(Name = "FileExtType", ResourceType = typeof(resxDbFields))]
        public string FileExtType { get; set; }
        [Display(Name = "FeatureValue", ResourceType = typeof(resxDbFields))]
        public string FeatureValue { get; set; }

        [Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? StartDate { get; set; }
        [Display(Name = "EndDate", ResourceType = typeof(resxDbFields))]
        public DateTime? EndDate { get; set; }
        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> CreatedByGUID { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "CreateBy", ResourceType = typeof(resxDbFields))]
        public string CreateBy { get; set; }

        [Display(Name = "FeatureTypeCategory", ResourceType = typeof(resxDbFields))]
        public string FeatureTypeCategory { get; set; }
        [Display(Name = "FeatureName", ResourceType = typeof(resxDbFields))]
        public string FeatureName { get; set; }








        public string Comments { get; set; }
        public byte[] dataItemInputDetailFeatureRowVersion { get; set; }
        public bool Active { get; set; }

    }

    public class WarehouseItemDetailFeatureUpdateModel
    {

        public string EmptyColumn { get; set; }
        [Display(Name = "ItemInputDetailFeatureGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemInputDetailFeatureGUID { get; set; }

        [Display(Name = "ItemInputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ItemInputDetailGUID { get; set; }


        [Display(Name = "FeatureTypeValueGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> FeatureTypeValueGUID { get; set; }

        [Display(Name = "FeatureTypeValueGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> FeatureTypeValueGUIDDoc { get; set; }

        [Display(Name = "FeatureTypeGUID", ResourceType = typeof(resxDbFields))]

        public Nullable<System.Guid> FeatureTypeGUID { get; set; }

        [Display(Name = "FileExtType", ResourceType = typeof(resxDbFields))]
        public string FileExtType { get; set; }
        [Display(Name = "FeatureValue", ResourceType = typeof(resxDbFields))]
        public string FeatureValue { get; set; }

        [Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? StartDate { get; set; }
        [Display(Name = "EndDate", ResourceType = typeof(resxDbFields))]
        public DateTime? EndDate { get; set; }
        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> CreatedByGUID { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "CreateBy", ResourceType = typeof(resxDbFields))]
        public string CreateBy { get; set; }

        [Display(Name = "FeatureTypeCategory", ResourceType = typeof(resxDbFields))]
        public string FeatureTypeCategory { get; set; }
        [Display(Name = "FeatureName", ResourceType = typeof(resxDbFields))]
        public string FeatureName { get; set; }








        public string Comments { get; set; }
        public byte[] dataItemInputDetailFeatureRowVersion { get; set; }
        public bool Active { get; set; }

    }
    #endregion

    #region Warehouse Item

    public class WarehouseFocalPointDataTableModel
    {
        [Display(Name = "WarehouseFocalPointGUID", ResourceType = typeof(resxDbFields))]
        public string WarehouseFocalPointGUID { get; set; }
        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }
        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public string OrganizationInstance { get; set; }
        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public string OrganizationInstanceGUID { get; set; }
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public string DutyStationGUID { get; set; }
        [Display(Name = "LocationGUID", ResourceType = typeof(resxDbFields))]
        public string LocationGUID { get; set; }

        [Display(Name = "ParentGUID", ResourceType = typeof(resxDbFields))]
        public string ParentGUID { get; set; }

        [Display(Name = "WarehouseTypeGUID", ResourceType = typeof(resxDbFields))]
        public string WarehouseTypeGUID { get; set; }
        [Display(Name = "WarehouseLocationGUID", ResourceType = typeof(resxDbFields))]
        public string WarehouseLocationGUID { get; set; }

        public bool Active { get; set; }
        public byte[] codeWarehouseFocalPointRowVersion { get; set; }

    }
    public class WarehouseDataTableModel
    {
        [Display(Name = "WarehouseGUID", ResourceType = typeof(resxDbFields))]
        public string WarehouseGUID { get; set; }
        [Display(Name = "WarehouseDescription", ResourceType = typeof(resxDbFields))]
        public string WarehouseDescription { get; set; }
        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public string OrganizationInstance { get; set; }
        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public string OrganizationInstanceGUID { get; set; }
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public string DutyStationGUID { get; set; }
        [Display(Name = "LocationGUID", ResourceType = typeof(resxDbFields))]
        public string LocationGUID { get; set; }

        [Display(Name = "ParentGUID", ResourceType = typeof(resxDbFields))]
        public string ParentGUID { get; set; }

        [Display(Name = "WarehouseTypeGUID", ResourceType = typeof(resxDbFields))]
        public string WarehouseTypeGUID { get; set; }
        [Display(Name = "WarehouseLocationGUID", ResourceType = typeof(resxDbFields))]
        public string WarehouseLocationGUID { get; set; }

        public bool Active { get; set; }
        public byte[] codeWarehouseRowVersion { get; set; }

    }

    public class WarehouseUpdateModel
    {

        [Display(Name = "WarehouseGUID", ResourceType = typeof(resxDbFields))]
        public Guid WarehouseGUID { get; set; }
        [Display(Name = "WarehouseDescription", ResourceType = typeof(resxDbFields))]
        public string WarehouseDescription { get; set; }
        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public Guid? OrganizationInstanceGUID { get; set; }
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Guid? DutyStationGUID { get; set; }
        [Display(Name = "LocationGUID", ResourceType = typeof(resxDbFields))]
        public Guid? LocationGUID { get; set; }

        [Display(Name = "ParentGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ParentGUID { get; set; }

        [Display(Name = "WarehouseTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid? WarehouseTypeGUID { get; set; }
        [Display(Name = "WarehouseLocationGUID", ResourceType = typeof(resxDbFields))]
        public Guid? WarehouseLocationGUID { get; set; }

        public bool Active { get; set; }
        public byte[] codeWarehouseRowVersion { get; set; }
    }

    #endregion

    #region Damaged Staff Report

    public class STIItemDamagedTrackDataTableModel
    {
        [Display(Name = "ItemOutputDetailDamagedTrackGUID", ResourceType = typeof(resxDbFields))]
        public string ItemOutputDetailDamagedTrackGUID { get; set; }
        [Display(Name = "ItemInputDetailGUID", ResourceType = typeof(resxDbFields))]
        public string ItemInputDetailGUID { get; set; }
        [Display(Name = "ItemOutputDetailGUID", ResourceType = typeof(resxDbFields))]
        public string ItemOutputDetailGUID { get; set; }
        [Display(Name = "DamagedTypeGUID", ResourceType = typeof(resxDbFields))]
        public string DamagedTypeGUID { get; set; }
        [Display(Name = "DamagedByGUID", ResourceType = typeof(resxDbFields))]
        public string DamagedByGUID { get; set; }
        [Display(Name = "DamagedByNameGUID", ResourceType = typeof(resxDbFields))]
        public string DamagedByNameGUID { get; set; }
        [Display(Name = "IncidentType", ResourceType = typeof(resxDbFields))]
        public string IncidentType { get; set; }

        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public string DutyStationGUID { get; set; }

        [Display(Name = "LastFlowStatusGUID", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatusGUID { get; set; }
        [Display(Name = "Status", ResourceType = typeof(resxDbFields))]
        public string LastFlowStatus { get; set; }
        [Display(Name = "LocationGUID", ResourceType = typeof(resxDbFields))]
        public string LocationGUID { get; set; }
        [Display(Name = "DocumentReference", ResourceType = typeof(resxDbFields))]
        public string DocumentReference { get; set; }

        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }

        [Display(Name = "ItemName", ResourceType = typeof(resxDbFields))]
        public string ItemName { get; set; }
        [Display(Name = "ModelName", ResourceType = typeof(resxDbFields))]
        public string ModelName { get; set; }
        [Display(Name = "Barcode", ResourceType = typeof(resxDbFields))]
        public string Barcode { get; set; }
        [Display(Name = "SerialNumber", ResourceType = typeof(resxDbFields))]
        public string SerialNumber { get; set; }
        [Display(Name = "IME1", ResourceType = typeof(resxDbFields))]
        public string IMEI { get; set; }
        [Display(Name = "GSM", ResourceType = typeof(resxDbFields))]
        public string GSM { get; set; }
        [Display(Name = "MAC", ResourceType = typeof(resxDbFields))]
        public string MAC { get; set; }
        [Display(Name = "IncidentDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? IncidentDate { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Display(Name = "IncidentLocationOccureed", ResourceType = typeof(resxDbFields))]
        public string IncidentLocationOccureed { get; set; }

        [Display(Name = "PresentLocation", ResourceType = typeof(resxDbFields))]
        public string PresentLocation { get; set; }





        [Display(Name = "IncidentReason", ResourceType = typeof(resxDbFields))]
        public string DamagedReason { get; set; }
        [Display(Name = "Identifier", ResourceType = typeof(resxDbFields))]
        public string Identifier { get; set; }
        [Display(Name = "DutyStation", ResourceType = typeof(resxDbFields))]
        public string DutyStation { get; set; }



        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public string CreatedByGUID { get; set; }



        [Display(Name = "CreatedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public string CreatedDate { get; set; }


        [Display(Name = "ReviewerByGUID", ResourceType = typeof(resxDbFields))]
        public string ReviewerByGUID { get; set; }


        [Display(Name = "ReviwingDate", ResourceType = typeof(resxDbFields))]
        public string ReviwingDate { get; set; }


        [Display(Name = "ReviewerRecommendation", ResourceType = typeof(resxDbFields))]
        public string ReviewerRecommendation { get; set; }

        //[Display(Name = "IsStaffReimburse", ResourceType = typeof(resxDbFields))]
        //public bool IsStaffReimburse { get; set; }
        [Display(Name = "ReimburseStatus", ResourceType = typeof(resxDbFields))]
        public Guid StaffReimburseStatusGUID { get; set; }
        [Display(Name = "ReimburseStatus", ResourceType = typeof(resxDbFields))]
        public string ReimburseStatusDecision { get; set; }


        [Display(Name = "ReimburseStatus", ResourceType = typeof(resxDbFields))]
        public string ReimburseStatus { get; set; }

        [Display(Name = "ICTApprovedBy", ResourceType = typeof(resxDbFields))]
        public string ICTApprovedBy { get; set; }

        [Display(Name = "AdminApprovedBy", ResourceType = typeof(resxDbFields))]
        public string AdminApprovedBy { get; set; }

        [Display(Name = "FinanceApprovedBy", ResourceType = typeof(resxDbFields))]
        public string FinanceApprovedBy { get; set; }

        [Display(Name = "ReimbursementAmount", ResourceType = typeof(resxDbFields))]
        public string ReimbursementAmount { get; set; }



        public bool Active { get; set; }
        public byte[] dataItemOutputDetailDamagedTrackRowVersion { get; set; }

    }


    public class STIItemDamagedTrackUpdateModel
    {

        [Display(Name = "ItemOutputDetailDamagedTrackGUID", ResourceType = typeof(resxDbFields))]
        public Guid ItemOutputDetailDamagedTrackGUID { get; set; }
        [Display(Name = "ItemInputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Guid ItemInputDetailGUID { get; set; }
        [Display(Name = "ItemOutputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ItemOutputDetailGUID { get; set; }
        [Display(Name = "IncidentType", ResourceType = typeof(resxDbFields))]
        public Guid? DamagedTypeGUID { get; set; }

        [Display(Name = "IncidentType", ResourceType = typeof(resxDbFields))]
        public string IncidentType { get; set; }
        [Display(Name = "ItemName", ResourceType = typeof(resxDbFields))]
        public string ItemName { get; set; }
        [Display(Name = "ReimburseStatus", ResourceType = typeof(resxDbFields))]
        public Guid StaffReimburseStatusGUID { get; set; }
        [Display(Name = "ReimburseStatus", ResourceType = typeof(resxDbFields))]
        public string ReimburseStatusDecision { get; set; }

        [Display(Name = "DamagedByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? DamagedByGUID { get; set; }
        [Display(Name = "ItemConditionWhenReceivedGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ItemConditionWhenReceivedGUID { get; set; }

        [Display(Name = "ItemConditionWhenReceivedGUID", ResourceType = typeof(resxDbFields))]
        public string ItemCondition { get; set; }
        [Display(Name = "PurposeOfUseGUID", ResourceType = typeof(resxDbFields))]
        public Guid? PurposeOfUseGUID { get; set; }
        [Display(Name = "PurposeOfUseGUID", ResourceType = typeof(resxDbFields))]
        public string PurposeOfUse { get; set; }
        [Display(Name = "DamagedByNameGUID", ResourceType = typeof(resxDbFields))]
        public Guid? DamagedByNameGUID { get; set; }

        [Display(Name = "StaffName", ResourceType = typeof(resxDbFields))]
        public string StaffName { get; set; }
        [Display(Name = "AccessLevel", ResourceType = typeof(resxDbFields))]
        public int AccessLevel { get; set; }
        [Display(Name = "CurrentStep", ResourceType = typeof(resxDbFields))]
        public int CurrentStep { get; set; }
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
        public Guid? DutyStationGUID { get; set; }

        [Display(Name = "LocationGUID", ResourceType = typeof(resxDbFields))]
        public Guid? LocationGUID { get; set; }
        [Display(Name = "DocumentReference", ResourceType = typeof(resxDbFields))]
        public string DocumentReference { get; set; }

        [Display(Name = "DamagedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? DamagedDate { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Display(Name = "IncidentLocationOccureed", ResourceType = typeof(resxDbFields))]
        public string IncidentLocationOccureed { get; set; }

        [Display(Name = "PresentLocation", ResourceType = typeof(resxDbFields))]
        public string PresentLocation { get; set; }





        [Display(Name = "DamagedReason", ResourceType = typeof(resxDbFields))]
        public string DamagedReason { get; set; }


        [Display(Name = "CreatedByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? CreatedByGUID { get; set; }


        [Display(Name = "CreatedDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public string CreatedDate { get; set; }

        [Display(Name = "LastFlowStatusGUID", ResourceType = typeof(resxDbFields))]
        public Guid? LastFlowStatusGUID { get; set; }


        [Display(Name = "ICTOfficerReviewerGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ICTOfficerReviewerGUID { get; set; }




        [Display(Name = "ICTOfficerReviewerComment", ResourceType = typeof(resxDbFields))]
        public string ICTOfficerReviewerComment { get; set; }

        [Display(Name = "ICTOfficerReviewerDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public string ICTOfficerReviewerDate { get; set; }


        [Display(Name = "ICTFocalPointGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ICTFocalPointGUID { get; set; }

        [Display(Name = "ICTFocalPointGUID", ResourceType = typeof(resxDbFields))]
        public string ICTFocalPoint { get; set; }

        [Display(Name = "ICTFocalPointComment", ResourceType = typeof(resxDbFields))]
        public string ICTFocalPointComment { get; set; }

        [Display(Name = "ICTFocalPointReviewDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public string ICTFocalPointReviewDate { get; set; }


        [Display(Name = "FinanceFocalPointReviewerGUID", ResourceType = typeof(resxDbFields))]
        public Guid? FinanceFocalPointReviewerGUID { get; set; }

        [Display(Name = "FinanceFocalPointReviewerGUID", ResourceType = typeof(resxDbFields))]
        public string FinanceFocalPoint { get; set; }

        [Display(Name = "FinanceFocalPointReviewerComment", ResourceType = typeof(resxDbFields))]
        public string FinanceFocalPointReviewerComment { get; set; }

        [Display(Name = "FinanceFocalPointReviewerDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? FinanceFocalPointReviewerDate { get; set; }

        [Display(Name = "AdminFocalPointReviewerGUID", ResourceType = typeof(resxDbFields))]
        public Guid? AdminFocalPointReviewerGUID { get; set; }


        [Display(Name = "AdminFocalPointReviewerGUID", ResourceType = typeof(resxDbFields))]
        public string AdminFocalPoint { get; set; }

        [Display(Name = "AdminFocalPointReviewerComment", ResourceType = typeof(resxDbFields))]
        public string AdminFocalPointReviewerComment { get; set; }

        [Display(Name = "AdminFocalPointReviewerDate", ResourceType = typeof(resxDbFields))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? AdminFocalPointReviewerDate { get; set; }


        [Display(Name = "ReviewerByGUID", ResourceType = typeof(resxDbFields))]
        public Guid? ReviewerByGUID { get; set; }


        [Display(Name = "ReviwingDate", ResourceType = typeof(resxDbFields))]
        public string ReviwingDate { get; set; }


        [Display(Name = "ReviewerRecommendation", ResourceType = typeof(resxDbFields))]
        public string ReviewerRecommendation { get; set; }

        //[Display(Name = "IsStaffReimburse", ResourceType = typeof(resxDbFields))]
        //public bool IsStaffReimburse { get; set; }

        [Display(Name = "ReimbursementAmount", ResourceType = typeof(resxDbFields))]
        public double? ReimbursementAmount { get; set; }

        [Display(Name = "AcquisitionDate", ResourceType = typeof(resxDbFields))]
        public DateTime? AcquisitionDate { get; set; }


        [Display(Name = "PriceValue", ResourceType = typeof(resxDbFields))]
        public decimal? PriceValue { get; set; }

        [Display(Name = "PriceTypeGUID", ResourceType = typeof(resxDbFields))]
        public string PriceType { get; set; }



        [Display(Name = "MSRPValue", ResourceType = typeof(resxDbFields))]
        public double? MSRPValue { get; set; }



        public bool Active { get; set; }
        public byte[] dataItemOutputDetailDamagedTrackRowVersion { get; set; }
    }
    #endregion

}
