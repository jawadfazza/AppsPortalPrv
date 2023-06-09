
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
    
    public partial class dataItemPipeline
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ItemPipelineGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ItemPipelineGUID{ get; set; }
    	
        [Display(Name = "ItemPipelineUploadGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ItemPipelineUploadGUID{ get; set; }
    	
        [Display(Name = "ItemGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ItemGUID{ get; set; }
    	
        [Display(Name = "Quantity", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> Quantity{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "ETA", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> ETA{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "ETA", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalETA { get { return new Portal().LocalTime(this.ETA); } }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataItemPipelineRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
        [Display(Name = "Archived", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> Archived{ get; set; }
    	
        [Display(Name = "StockGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> StockGUID{ get; set; }
    	
    
        public virtual codeISSItem codeISSItem { get; set; }
        public virtual codeISSStock codeISSStock { get; set; }
        public virtual dataItemPipelineUpload dataItemPipelineUpload { get; set; }
    }
}
