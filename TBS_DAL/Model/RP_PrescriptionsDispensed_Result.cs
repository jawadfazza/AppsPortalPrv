
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TBS_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class RP_PrescriptionsDispensed_Result
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "MedicalBeneficiaryItemOutGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid MedicalBeneficiaryItemOutGUID{ get; set; }
    	
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "MedicalBeneficiaryGUID", ResourceType = typeof(resxDbFields))]
    	public string MedicalBeneficiaryGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "MedicalPharmacyGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid MedicalPharmacyGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DeliveryDate", ResourceType = typeof(resxDbFields))]
    	public System.DateTime DeliveryDate{ get; set; }
    	
        [Display(Name = "Cost", ResourceType = typeof(resxDbFields))]
    	public Nullable<double> Cost{ get; set; }
    	
        [Display(Name = "DiagnosisGUID", ResourceType = typeof(resxDbFields))]
    	public string DiagnosisGUID{ get; set; }
    	
        [Display(Name = "DiseaseTypeGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> DiseaseTypeGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid OrganizationInstanceGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "MainWarehouse", ResourceType = typeof(resxDbFields))]
    	public bool MainWarehouse{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BeneficiaryType", ResourceType = typeof(resxDbFields))]
    	public string BeneficiaryType{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BeneficiaryTypeGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid BeneficiaryTypeGUID{ get; set; }
    	
        [StringLength(10, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "NationalityCode", ResourceType = typeof(resxDbFields))]
    	public string NationalityCode{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "GenderGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid GenderGUID{ get; set; }
    	
        [Display(Name = "Expr1", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> Expr1{ get; set; }
    	
        [Display(Name = "Brithday", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> Brithday{ get; set; }
    	
    }
}
