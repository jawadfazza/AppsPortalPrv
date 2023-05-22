
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ISS_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class dataItemStockEmergencyReserve
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ItemStockEmergencyReserveGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ItemStockEmergencyReserveGUID{ get; set; }
    	
        [Display(Name = "ItemStockEmergencyUploadGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ItemStockEmergencyUploadGUID{ get; set; }
    	
        [Display(Name = "ItemGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ItemGUID{ get; set; }
    	
        [Display(Name = "QuantityToBeReserved", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> QuantityToBeReserved{ get; set; }
    	
        [StringLength(400, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ForWhere", ResourceType = typeof(resxDbFields))]
    	public string ForWhere{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "ExpectedDateToDispatch", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> ExpectedDateToDispatch{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "ExpectedDateToDispatch", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalExpectedDateToDispatch { get { return new Portal().LocalTime(this.ExpectedDateToDispatch); } }
    	
        [Display(Name = "TotalItemInAllStock", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> TotalItemInAllStock{ get; set; }
    	
        [Display(Name = "PipelineOrdersPlaced", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> PipelineOrdersPlaced{ get; set; }
    	
        [Display(Name = "TotalStockWithPipeline", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> TotalStockWithPipeline{ get; set; }
    	
        [Display(Name = "TotalReservedforEmergency", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> TotalReservedforEmergency{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataItemStockEmergencyReserveRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
        [Display(Name = "Archived", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> Archived{ get; set; }
    	
    
        public virtual codeISSItem codeISSItem { get; set; }
        public virtual dataItemStockEmergencyUpload dataItemStockEmergencyUpload { get; set; }
    }
}