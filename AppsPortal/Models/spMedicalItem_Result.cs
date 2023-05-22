//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AppsPortal.Models
{
    using System;
    
    public partial class spMedicalItem_Result
    {
        public string MedicalGenericNameDescription { get; set; }
        public string MedicalManufacturerDescription { get; set; }
        public string Dose { get; set; }
        public string MedicalPharmacologicalForm { get; set; }
        public string MedicalPackingUnit { get; set; }
        public string MedicalTreatment { get; set; }
        public string LanguageID { get; set; }
        public string MedicalRouteAdministration { get; set; }
        public System.Guid MedicalItemGUID { get; set; }
        public string BrandName { get; set; }
        public string DoseQuantity { get; set; }
        public string LicenseNoDate { get; set; }
        public double RemainingItemsQuantity { get; set; }
        public string Barcode { get; set; }
        public byte[] codeMedicalItemRowVersion { get; set; }
        public System.Guid MedicalPharmacologicalFormGUID { get; set; }
        public System.Guid MedicalPackingUnitGUID { get; set; }
        public System.Guid MedicalManufacturerGUID { get; set; }
        public System.Guid SourceGUID { get; set; }
        public System.Guid MedicalTreatmentGUID { get; set; }
        public System.Guid MedicalRouteAdministrationGUID { get; set; }
        public System.Guid MedicalGenericNameGUID { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<double> PackingUnit { get; set; }
        public int TotalDispatchedItems { get; set; }
        public bool Active { get; set; }
    }
}