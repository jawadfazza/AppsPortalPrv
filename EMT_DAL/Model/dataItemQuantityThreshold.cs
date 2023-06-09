
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
    
    public partial class dataItemQuantityThreshold
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ItemQuantityThresholdGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ItemQuantityThresholdGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "MedicalPharmacyGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid MedicalPharmacyGUID{ get; set; }
    	
        [Display(Name = "MedicalItemGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> MedicalItemGUID{ get; set; }
    	
        [Display(Name = "QuantityThreshold", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> QuantityThreshold{ get; set; }
    	
        [Display(Name = "QuantityTotalRemainingItems", ResourceType = typeof(resxDbFields))]
    	public Nullable<double> QuantityTotalRemainingItems{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataItemQuantityThresholdRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
    
        public virtual codeMedicalPharmacy codeMedicalPharmacy { get; set; }
        public virtual codeMedicalItem codeMedicalItem { get; set; }
    }
}
