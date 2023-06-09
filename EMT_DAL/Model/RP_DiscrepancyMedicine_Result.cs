
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EMT_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class RP_DiscrepancyMedicine_Result
    {
        [Display(Name = "LanguageID", ResourceType = typeof(resxDbFields))]
    	public string LanguageID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BrandName", ResourceType = typeof(resxDbFields))]
    	public string BrandName{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DoseQuantity", ResourceType = typeof(resxDbFields))]
    	public string DoseQuantity{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "MedicalDoseUnitGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid MedicalDoseUnitGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "MedicalPharmacologicalFormGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid MedicalPharmacologicalFormGUID{ get; set; }
    	
        [Display(Name = "PackingUnit", ResourceType = typeof(resxDbFields))]
    	public Nullable<double> PackingUnit{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "MedicalPackingUnitGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid MedicalPackingUnitGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "MedicalManufacturerGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid MedicalManufacturerGUID{ get; set; }
    	
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "LicenseNoDate", ResourceType = typeof(resxDbFields))]
    	public string LicenseNoDate{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "SourceGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid SourceGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "MedicalTreatmentGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid MedicalTreatmentGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "MedicalRouteAdministrationGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid MedicalRouteAdministrationGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TotalDispatchedItems", ResourceType = typeof(resxDbFields))]
    	public int TotalDispatchedItems{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "RemainingItemsQuantity", ResourceType = typeof(resxDbFields))]
    	public double RemainingItemsQuantity{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "MedicalGenericNameGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid MedicalGenericNameGUID{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "MedicalGenericNameDescription", ResourceType = typeof(resxDbFields))]
    	public string MedicalGenericNameDescription{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "MedicalPharmacyDescription", ResourceType = typeof(resxDbFields))]
    	public string MedicalPharmacyDescription{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "MainWarehouse", ResourceType = typeof(resxDbFields))]
    	public bool MainWarehouse{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "MedicalRouteAdministration", ResourceType = typeof(resxDbFields))]
    	public string MedicalRouteAdministration{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid OrganizationInstanceGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ClassificationDescription", ResourceType = typeof(resxDbFields))]
    	public string ClassificationDescription{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DiscrepancyDate", ResourceType = typeof(resxDbFields))]
    	public System.DateTime DiscrepancyDate{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DiscrepancyType", ResourceType = typeof(resxDbFields))]
    	public int DiscrepancyType{ get; set; }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ConfirmedBy", ResourceType = typeof(resxDbFields))]
    	public string ConfirmedBy{ get; set; }
    	
        [Display(Name = "ConfirmedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> ConfirmedOn{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DiscrepancyQuantity", ResourceType = typeof(resxDbFields))]
    	public double DiscrepancyQuantity{ get; set; }
    	
        [Display(Name = "RemainingItems", ResourceType = typeof(resxDbFields))]
    	public Nullable<double> RemainingItems{ get; set; }
    	
        [Display(Name = "OriginalQuantity", ResourceType = typeof(resxDbFields))]
    	public Nullable<double> OriginalQuantity{ get; set; }
    	
        [StringLength(250, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Comment", ResourceType = typeof(resxDbFields))]
    	public string Comment{ get; set; }
    	
    }
}
