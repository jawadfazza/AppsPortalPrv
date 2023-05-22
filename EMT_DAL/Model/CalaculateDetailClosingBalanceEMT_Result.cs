
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
    
    public partial class CalaculateDetailClosingBalanceEMT_Result
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(12, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Action", ResourceType = typeof(resxDbFields))]
    	public string Action{ get; set; }
    	
        [Display(Name = "Record", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> Record{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(201, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BrandName", ResourceType = typeof(resxDbFields))]
    	public string BrandName{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "MedicalPharmacyDescription", ResourceType = typeof(resxDbFields))]
    	public string MedicalPharmacyDescription{ get; set; }
    	
        [Display(Name = "Quantity", ResourceType = typeof(resxDbFields))]
    	public Nullable<double> Quantity{ get; set; }
    	
    }
}