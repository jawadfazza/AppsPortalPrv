using Portal_BL.Library;
using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMT_DAL.Model
{
    #region CRI
    public class DispatchDataTableModel
    {

    }
    #endregion
    public class MedicalDistributionRestrictionUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalDistributionRestrictionGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalDistributionRestrictionGUID { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ImplementPartnerGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ImplementPartnerGUID { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ProvideByOrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ProvideByOrganizationInstanceGUID { get; set; }

        [Display(Name = "BeneficiaryTypeGUID", ResourceType = typeof(resxDbFields))]
        public string[] BeneficiaryTypeGUID { get; set; }
        public string BeneficiaryTypeClientGUID { get; set; }

        [Display(Name = "PrescriptionNumberPerMonth", ResourceType = typeof(resxDbFields))]
        public Nullable<int> PrescriptionNumberPerMonth { get; set; }

        [Display(Name = "MedicineNumberPerPrescription", ResourceType = typeof(resxDbFields))]
        public Nullable<int> MedicineNumberPerPrescription { get; set; }

        [Display(Name = "MedicinesQuantity", ResourceType = typeof(resxDbFields))]
        public Nullable<int> MedicinesQuantity { get; set; }

        [Display(Name = "MedicinesExpiration", ResourceType = typeof(resxDbFields))]
        public Nullable<int> MedicinesExpiration { get; set; }

        [Display(Name = "MedicinesExcludedByClassification", ResourceType = typeof(resxDbFields))]
        public string[] MedicinesExcludedByClassification { get; set; }
        public string MedicinesExcludedByClassificationClient { get; set; }

        [Display(Name = "MedicinesExcludedByBrand", ResourceType = typeof(resxDbFields))]
        public string[] MedicinesExcludedByBrand { get; set; }
        public string MedicinesExcludedByBrandClient { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataMedicalDistributionRestrictionRowVersion { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
    public class MedicalDistributionRestrictionsDataTableModel
    {
        public System.Guid MedicalDistributionRestrictionGUID { get; set; }
        public string ProvideByOrganizationInstanceGUID { get; set; }
        public string OrganizationInstanceDescription { get; set; }
        public string ImplementPartnerGUID { get; set; }
        public string ImplementPartner { get; set; }
        public string BeneficiaryTypeGUID { get; set; }
        public Nullable<int> PrescriptionNumberPerMonth { get; set; }
        public Nullable<int> MedicineNumberPerPrescription { get; set; }
        public Nullable<int> MedicinesQuantity { get; set; }
        public Nullable<int> MedicinesExpiration { get; set; }
        public string MedicinesExcludedByClassification { get; set; }
        public string MedicinesExcludedByBrand { get; set; }
        public bool Active { get; set; }
        public byte[] dataMedicalDistributionRestrictionRowVersion { get; set; }
    }
    public class MedicalDiscrepancyDetailsDataTableModel
    {
        public System.Guid MedicalDiscrepancyDetailGUID { get; set; }
        public System.Guid MedicalDiscrepancyGUID { get; set; }
        public System.Guid ReferenceItemGUID { get; set; }
        public System.Guid MedicalItemInputGUID { get; set; }
        public string BrandName { get; set; }
        public string MedicalItemGUID { get; set; }
        public double DiscrepancyQuantity { get; set; }
        public double OriginalQuantity { get; set; }
        public double RemainingQuaintity { get; set; }
        public string Comment { get; set; }
        public bool Active { get; set; }
        public byte[] dataMedicalDiscrepancyDetailRowVersion { get; set; }
    }
    public class MedicalDiscrepancysDataTableModel
    {
        public System.Guid MedicalDiscrepancyGUID { get; set; }
        public System.Guid MedicalPharmacyGUID { get; set; }
        public string MedicalPharmacyDescription { get; set; }
        public System.DateTime DiscrepancyDate { get; set; }
        public Nullable<System.DateTime> LocalDiscrepancyDate { get { return new Portal().LocalTime(this.DiscrepancyDate); } }
        public string DiscrepancyType { get; set; }
        public bool Active { get; set; }
        public byte[] dataMedicalDiscrepancyRowVersion { get; set; }
    }
    public class MedicalDiscrepancyUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalDiscrepancyGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalDiscrepancyGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalPharmacyGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalPharmacyGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.DateTime)]
        [Display(Name = "DiscrepancyDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime DiscrepancyDate { get; set; }

        [Display(Name = "DiscrepancyType", ResourceType = typeof(resxDbFields))]
        public Nullable<int> DiscrepancyType { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ConfirmedBy", ResourceType = typeof(resxDbFields))]
        public string ConfirmedBy { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "ConfirmedOn", ResourceType = typeof(resxDbFields))]
        public System.DateTime ConfirmedOn { get; set; }

        [StringLength(250, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Comment", ResourceType = typeof(resxDbFields))]
        public string Comment { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataMedicalDiscrepancyRowVersion { get; set; }

        public List<MedicalDiscrepancyDetailsDataTableModel> MedicalDiscrepancyDetails { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }
    public class MedicalDiscrepancyDetailsUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalDiscrepancyDetailGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalDiscrepancyDetailGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalDiscrepancyGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalDiscrepancyGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalItemGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalItemGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DiscrepancyQuantity", ResourceType = typeof(resxDbFields))]
        public double DiscrepancyQuantity { get; set; }

        [Display(Name = "RemainingItems", ResourceType = typeof(resxDbFields))]
        public Nullable<double> RemainingItems { get; set; }

        [Display(Name = "OriginalQuantity", ResourceType = typeof(resxDbFields))]
        public Nullable<double> OriginalQuantity { get; set; }

        [Display(Name = "ReferenceItemGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ReferenceItemGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalPharmacyGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalPharmacyGUID { get; set; }

        [StringLength(250, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Comment", ResourceType = typeof(resxDbFields))]
        public string Comment { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataMedicalDiscrepancyDetailRowVersion { get; set; }
    }
    public class EMTReportParametersList
    {
        public int Report { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "EndDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime? EndDate { get; set; }

        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public Guid[] OrganizationInstanceGUID { get; set; }

        [Display(Name = "MedicalPharmacyGUID", ResourceType = typeof(resxDbFields))]
        public Guid[] MedicalPharmacyGUID { get; set; }

        [Display(Name = "MedicalBeneficiaryGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid? MedicalBeneficiaryGUID { get; set; }

        [Display(Name = "Sequance", ResourceType = typeof(resxDbFields))]
        public int[] Sequance { get; set; }

        [Display(Name = "MedicalPharmacologicalFormGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid[] MedicalPharmacologicalFormGUID { get; set; }

        [Display(Name = "MedicalTreatmentGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid[] MedicalTreatmentGUID { get; set; }

        [Display(Name = "MedicalGenericNameGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid[] MedicalGenericNameGUID { get; set; }

        [Display(Name = "IncludeZero", ResourceType = typeof(resxDbFields))]
        public bool IncludeZero { get; set; }

    }
    public class MedicalPharmacyUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalPharmacyGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalPharmacyGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid OrganizationInstanceGUID { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalPharmacyDescription", ResourceType = typeof(resxDbFields))]
        public string MedicalPharmacyDescription { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MainWarehouse", ResourceType = typeof(resxDbFields))]
        public bool MainWarehouse { get; set; }

        [Display(Name = "Sort", ResourceType = typeof(resxDbFields))]
        public Nullable<int> Sort { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] codeMedicalPharmacyRowVersion { get; set; }
        public byte[] codeMedicalPharmacyLanguageRowVersion { get; set; }


    }
    public class MedicalPharmacysDataTableModel
    {
        public System.Guid MedicalPharmacyGUID { get; set; }
        public System.Guid OrganizationInstanceGUID { get; set; }
        public string OrganizationInstanceDescription { get; set; }
        public string MedicalPharmacyDescription { get; set; }

        public bool MainWarehouse { get; set; }
        public bool Active { get; set; }
        public byte[] codeMedicalPharmacyRowVersion { get; set; }
    }
    public class MedicalItemsDataTableModel
    {
        public string MedicalItemGUID { get; set; }
        public System.Guid FK { get; set; }
        public string MedicalGenericNameGUID { get; set; }
        public string MedicalPharmacyGUID { get; set; }
        public string MedicalGenericNameDescription { get; set; }
        public string BrandName { get; set; }
        public string BatchNumber { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string DoseQuantity { get; set; }
        public string MedicalDoseUnitGUID { get; set; }
        public string MedicalDoseUnitDescription { get; set; }
        public string MedicalPharmacologicalFormGUID { get; set; }
        public string MedicalPharmacologicalFormDescription { get; set; }
        public double? PackingUnit { get; set; }
        public string MedicalPackingUnitGUID { get; set; }
        public string MedicalPackingUnitDescription { get; set; }
        public string MedicalManufacturerGUID { get; set; }
        public string MedicalManufacturerDescription { get; set; }
        public string LicenseNoDate { get; set; }
        public string SourceGUID { get; set; }
        public string MedicalTreatmentGUID { get; set; }
        public string MedicalTreatmentDescription { get; set; }
        public string MedicalRouteAdministrationGUID { get; set; }
        public string MedicalRouteAdministrationDescription { get; set; }
        public double RemainingItemsQuantity { get; set; }
        public int TotalDispatchedItems { get; set; }
        public string Comment { get; set; }
        public string Barcode { get; set; }
        public bool Active { get; set; }

        public byte[] codeMedicalItemRowVersion { get; set; }
        public double Quantity { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string ProvidedBy { get; set; }
    }
    public class ItemQuantityThresholdDataTableModel
    {
        public System.Guid ItemQuantityThresholdGUID { get; set; }
        public System.Guid MedicalPharmacyGUID { get; set; }
        public string MedicalPharmacyDescription { get; set; }
        public Nullable<System.Guid> MedicalItemGUID { get; set; }
        public Nullable<int> QuantityThreshold { get; set; }
        public Nullable<double> QuantityTotalRemainingItems { get; set; }
        public bool Active { get; set; }
        public byte[] dataItemQuantityThresholdRowVersion { get; set; }
    }
    public class MedicalItemUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalItemGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalItemGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalGenericNameGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalGenericNameGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BrandName", ResourceType = typeof(resxDbFields))]
        public string BrandName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "DoseQuantity", ResourceType = typeof(resxDbFields))]
        public string DoseQuantity { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalDoseUnitGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalDoseUnitGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalPharmacologicalFormGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalPharmacologicalFormGUID { get; set; }

        [Display(Name = "PackingUnit", ResourceType = typeof(resxDbFields))]
        public double? PackingUnit { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalPackingUnitGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalPackingUnitGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalManufacturerGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalManufacturerGUID { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "LicenseNoDate", ResourceType = typeof(resxDbFields))]
        public string LicenseNoDate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SourceGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid SourceGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalTreatmentGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalTreatmentGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalRouteAdministrationGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalRouteAdministrationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RemainingItemsQuantity", ResourceType = typeof(resxDbFields))]
        public double RemainingItemsQuantity { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TotalDispatchedItems", ResourceType = typeof(resxDbFields))]
        public int TotalDispatchedItems { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Barcode", ResourceType = typeof(resxDbFields))]
        public string Barcode { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] codeMedicalItemRowVersion { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
    public class MedicalManufacturerUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalManufacturerGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalManufacturerGUID { get; set; }

        [Display(Name = "Sort", ResourceType = typeof(resxDbFields))]
        public Nullable<int> Sort { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalManufacturerDescription", ResourceType = typeof(resxDbFields))]
        public string MedicalManufacturerDescription { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] codeMedicalManufacturerRowVersion { get; set; }
        public byte[] codeMedicalManufacturerLanguageRowVersion { get; set; }
    }
    public class MedicalManufacturersDataTableModel
    {
        public System.Guid MedicalManufacturerGUID { get; set; }

        public Nullable<int> Sort { get; set; }
        public string MedicalManufacturerDescription { get; set; }

        public bool Active { get; set; }

        public byte[] codeMedicalManufacturerRowVersion { get; set; }
    }
    public class MedicalGenericNamesDataTableModel
    {
        public System.Guid MedicalGenericNameGUID { get; set; }
        public Nullable<int> Sort { get; set; }
        public string MedicalGenericNameDescription { get; set; }
        public string Dose { get; set; }
        public string Form { get; set; }
        public string Classification { get; set; }
        public bool Active { get; set; }
        public byte[] codeMedicalGenericNameRowVersion { get; set; }
    }
    public class MedicalGenericNameUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalGenericNameGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalGenericNameGUID { get; set; }

        [Display(Name = "Sort", ResourceType = typeof(resxDbFields))]
        public Nullable<int> Sort { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalGenericNameDescription", ResourceType = typeof(resxDbFields))]
        public string MedicalGenericNameDescription { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Dose", ResourceType = typeof(resxDbFields))]
        public string Dose { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Form", ResourceType = typeof(resxDbFields))]
        public string Form { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Classification", ResourceType = typeof(resxDbFields))]
        public string Classification { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] codeMedicalGenericNameRowVersion { get; set; }
        public byte[] codeMedicalGenericNameLanguageRowVersion { get; set; }
    }
    public class MedicalItemInputUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalItemInputGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalItemInputGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.Date)]
        [Display(Name = "DeliveryDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DeliveryDate { get; set; }

        [Display(Name = "ConfirmedReceived", ResourceType = typeof(resxDbFields))]
        public bool ConfirmedReceived { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ProcuredByOrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ProcuredByOrganizationInstanceGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ProvidedByOrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> ProvidedByOrganizationInstanceGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalPharmacyGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> MedicalPharmacyGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataMedicalItemInputRowVersion { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public List<MedicalItemInputSupplyDetailsDataTableModel> medicalItemInputSupplyDetailsDataTableModels { get; set; }
        public List<MedicalItemInputDetailsDataTableModel> MedicalItemInputDetailsDataTableModel { get; set; }
    }
    public class MedicalItemInputsDataTableModel
    {
        public System.Guid MedicalItemInputGUID { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public Nullable<bool> ConfirmedReceived { get; set; }
        public int ItemsConfirmedCount { get; set; }
        public string ProcuredByOrganizationInstanceGUID { get; set; }
        public string OrganizationInstanceDescription { get; set; }
        public string MedicalPharmacyGUID { get; set; }
        public string MedicalPharmacyDescription { get; set; }
        public bool Active { get; set; }
        public byte[] dataMedicalItemInputRowVersion { get; set; }
    }
    public class MedicalItemInputDetailsDataTableModel
    {
        public string MedicalGenericNameGUID { get; set; }
        public System.Guid MedicalItemTransferDetailGUID { get; set; }
        public System.Guid MedicalItemInputDetailGUID { get; set; }
        public System.Guid MedicalItemInputGUID { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public string MedicalItemGUID { get; set; }
        public string BrandName { get; set; }
        public string MedicalGenericNameDescription { get; set; }
        public Nullable<System.DateTime> ManufacturingDate { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }
        public string BatchNumber { get; set; }
        public int QuantityBySmallestUnit { get; set; }
        public int QuantityByPackingUnit { get; set; }
        public int QuantityByPackingTransfer { get; set; }
        public Nullable<double> RemainingItems { get; set; }
        public double PriceOfSmallestUnit { get; set; }
        public double PriceOfPackingUnit { get; set; }
        public bool Active { get; set; }
        public byte[] dataMedicalItemInputDetailRowVersion { get; set; }
        public byte[] dataMedicalItemTransferDetailRowVersion { get; set; }

        public bool Confirmed { get; set; }
        public string ConfirmedBy { get; set; }
        public Nullable<System.DateTime> ConfirmedDate { get; set; }
        public string Comments { get; set; }
    }
    public class MedicalItemInputSupplyUpdateModel
    {
        

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalItemInputSupplyGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalItemInputSupplyGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.Date)]
        [Display(Name = "DispatchDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> DispatchDate { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SupplierName", ResourceType = typeof(resxDbFields))]
        public string SupplierName { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PurchaseOrder", ResourceType = typeof(resxDbFields))]
        public string PurchaseOrder { get; set; }

        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
        public string Comments { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataMedicalItemInputSupplyRowVersion { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }
    public class MedicalItemInputSupplysDataTableModel
    {
        public System.Guid MedicalItemInputSupplyGUID { get; set; }
        public Nullable<System.DateTime> DispatchDate { get; set; }
        public string SupplierName { get; set; }
        public string PurchaseOrder { get; set; }
        public bool Active { get; set; }
        public byte[] dataMedicalItemInputSupplyRowVersion { get; set; }
    }
    public class MedicalItemInputSupplyDetailsDataTableModel
    {
        public System.Guid MedicalItemInputSupplyDetailGUID { get; set; }
        public System.Guid MedicalItemInputSupplyGUID { get; set; }
        public string MedicalItemGUID { get; set; }
        public string BrandName { get; set; }
        public string MedicalGenericNameDescription { get; set; }

        public Nullable<System.DateTime> ManufacturingDate { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }
        public string BatchNumber { get; set; }
        public int QuantityBySmallestUnit { get; set; }
        public int QuantityByPackingUnit { get; set; }
        public Nullable<int> RemainingItems { get; set; }
        public double PriceOfSmallestUnit { get; set; }
        public double PriceOfPackingUnit { get; set; }
        public DateTime? DispatchDate { get; set; }
        public bool Active { get; set; }
        public byte[] dataMedicalItemInputSupplyDetailRowVersion { get; set; }
        public string MedicalGenericNameGUID { get; set; }
    }
    public class MedicalItemTransferUpdateModel
    {

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalItemTransferGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalItemTransferGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalPharmacyGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalPharmacyGUID { get; set; }

        [Display(Name = "ProvidedByOrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid? ProvidedByOrganizationInstanceGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.Date)]
        [Display(Name = "DeliveryDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime? DeliveryDate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ConfirmedReceived", ResourceType = typeof(resxDbFields))]
        public bool ConfirmedReceived { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataMedicalItemTransferRowVersion { get; set; }

        public Guid? OrganizationInstanceGUID { get; set; }

        public List<MedicalItemInputDetailsDataTableModel> MedicalItemInputDetailsDataTableModel { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }



    public class MedicalItemTransfersDataTableModel
    {
        public System.Guid MedicalItemTransferGUID { get; set; }

        public string MedicalPharmacyGUID { get; set; }
        public string MedicalPharmacyDescription { get; set; }
        public string OrganizationInstanceGUID { get; set; }
        public string OrganizationInstanceDescription { get; set; }
        public System.DateTime DeliveryDate { get; set; }
        public bool ConfirmedReceived { get; set; }
        public bool Active { get; set; }

        public byte[] dataMedicalItemTransferRowVersion { get; set; }
        public int ItemsConfirmedCount { get; set; }
    }
    public class MedicalItemTransferDetailsDataTableModel
    {
        public Guid MedicalItemTransferDetailGUID { get; set; }
        public Nullable<System.Guid> MedicalItemTransferGUID { get; set; }
        public Nullable<System.Guid> MedicalItemInputDetailGUID { get; set; }
        public Nullable<System.Guid> MedicalItemGUID { get; set; }
        public string BrandName { get; set; }
        public string MedicalGenericNameDescription { get; set; }

        public int QuantityByPackingUnit { get; set; }
        public int QuantityByPackingTransfer { get; set; }
        public double RemainingItems { get; set; }
        public bool Active { get; set; }

        public byte[] dataMedicalItemTransferDetailRowVersion { get; set; }

    }

    public class TransferDetailsDataTableModel
    {
        public string Pharmacy { get; set; }
        public string Text { get; set; }
        public int QuantityByPackingUnit { get; set; }
        public int? QuantityByPackingTransferUnit { get; set; }
        public double RemainingItems { get; set; }
        public System.DateTime DeliveryDate { get; set; }
    }
    public class MedicalItemTransferDetailUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalItemTransferDetailGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalItemTransferDetailGUID { get; set; }

        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> OrganizationInstanceGUID { get; set; }

        [Display(Name = "MedicalItemTransferGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> MedicalItemTransferGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalItemInputDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> MedicalItemInputDetailGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalItemGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> MedicalItemGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "QuantityByPackingUnit", ResourceType = typeof(resxDbFields))]
        public int QuantityByPackingUnit { get; set; }

        [Display(Name = "QuantityByPackingTransfer", ResourceType = typeof(resxDbFields))]
        public Nullable<int> QuantityByPackingTransfer { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RemainingItems", ResourceType = typeof(resxDbFields))]
        public double RemainingItems { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataMedicalItemTransferDetailRowVersion { get; set; }
    }
    public class MedicalBeneficiaryItemOutUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalBeneficiaryItemOutGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalBeneficiaryItemOutGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalBeneficiaryGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalBeneficiaryGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalPharmacyGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalPharmacyGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.DateTime)]
        [Display(Name = "DeliveryDate", ResourceType = typeof(resxDbFields))]
        public System.DateTime? DeliveryDate { get; set; }

        [Display(Name = "Cost", ResourceType = typeof(resxDbFields))]
        public Nullable<double> Cost { get; set; }

        [Display(Name = "DiagnosisGUID", ResourceType = typeof(resxDbFields))]
        public string[] DiagnosisGUID { get; set; }
        public string DiagnosisClientGUID { get; set; }

        [Display(Name = "DiseaseTypeGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> DiseaseTypeGUID { get; set; }

        [Display(Name = "MedicineDeliveryStatus", ResourceType = typeof(resxDbFields))]
        public bool DeliveryStatus { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataMedicalBeneficiaryItemOutRowVersion { get; set; }

        public List<MedicalBeneficiaryItemOutDetailsDataTableModel> medicalBeneficiaryItemOutDetails { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
    public class MedicalBeneficiaryItemOutsDataTableModel
    {
        public System.Guid MedicalBeneficiaryItemOutGUID { get; set; }
        public System.Guid MedicalBeneficiaryGUID { get; set; }
        public string MedicalPharmacyGUID { get; set; }
        public string MedicalPharmacyDescription { get; set; }
        public System.DateTime DeliveryDate { get; set; }
        public Nullable<double> Cost { get; set; }
        public string DiagnosisGUID { get; set; }
        public string DiseaseTypeGUID { get; set; }
        public string FullName { get; set; }
        public string UNHCRNumber { get; set; }
        public bool DeliveryStatus { get; set; }
        public bool Active { get; set; }
        public byte[] dataMedicalBeneficiaryItemOutRowVersion { get; set; }
    }
    public class MedicalBeneficiaryItemOutDetailsUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalBeneficiaryItemOutDetailGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalBeneficiaryItemOutDetailGUID { get; set; }

        [Display(Name = "MedicalBeneficiaryItemOutGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> MedicalBeneficiaryItemOutGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalItemTransferDetailGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> MedicalItemTransferDetailGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalItemGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> MedicalItemGUID { get; set; }

        [Display(Name = "MedicalPharmacyGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<System.Guid> MedicalPharmacyGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "QuantityByPackingUnit", ResourceType = typeof(resxDbFields))]
        public double QuantityByPackingUnit { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataMedicalBeneficiaryItemOutDetailRowVersion { get; set; }
    }
    public class MedicalBeneficiaryItemOutDetailsDataTableModel
    {
        public System.Guid MedicalBeneficiaryItemOutDetailGUID { get; set; }

        public Nullable<System.Guid> MedicalBeneficiaryItemOutGUID { get; set; }
        public Nullable<System.Guid> MedicalItemTransferDetailGUID { get; set; }
        public string MedicalItemGUID { get; set; }
        public double QuantityByPackingUnit { get; set; }
        public double Cost { get; set; }
        public double RemainingItems { get; set; }
        public double PriceOfPackingUnit { get; set; }
        public string BrandName { get; set; }
        public string BatchNumber { get; set; }
        public bool Active { get; set; }
        public byte[] dataMedicalBeneficiaryItemOutDetailRowVersion { get; set; }
    }
    public class MedicalBeneficiaryUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "MedicalBeneficiaryGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid MedicalBeneficiaryGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BeneficiaryTypeGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid BeneficiaryTypeGUID { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "IDNumber", ResourceType = typeof(resxDbFields))]
        public string IDNumber { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "UNHCRNumber", ResourceType = typeof(resxDbFields))]
        public string UNHCRNumber { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "GenderGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid GenderGUID { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Brithday", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> Brithday { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataMedicalBeneficiaryRowVersion { get; set; }
    }
    public class MedicalBeneficiarysDataTableModel
    {
        public System.Guid MedicalBeneficiaryGUID { get; set; }
        public System.Guid BeneficiaryTypeGUID { get; set; }
        public string FullName { get; set; }
        public Nullable<int> DocumentType { get; set; }
        public string BeneficiaryType { get; set; }
        public string IDNumber { get; set; }
        public string UNHCRNumber { get; set; }
        public System.Guid GenderGUID { get; set; }
        public string Gender { get; set; }
        public Nullable<System.DateTime> Brithday { get; set; }
        public Nullable<System.Guid> OrganizationInstanceGUID { get; set; }
        public bool Active { get; set; }
        public byte[] dataMedicalBeneficiaryRowVersion { get; set; }
    }
}
