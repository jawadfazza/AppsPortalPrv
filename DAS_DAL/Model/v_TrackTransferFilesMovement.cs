
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAS_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class v_TrackTransferFilesMovement
    {
        [Display(Name = "ScannDocumentTransferFlowGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ScannDocumentTransferFlowGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "FileGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid FileGUID{ get; set; }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "FileNumber", ResourceType = typeof(resxDbFields))]
    	public string FileNumber{ get; set; }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "RefugeeStatus", ResourceType = typeof(resxDbFields))]
    	public string RefugeeStatus{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "TransferDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> TransferDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "TransferDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalTransferDate { get { return new Portal().LocalTime(this.TransferDate); } }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(101, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TransferdBy", ResourceType = typeof(resxDbFields))]
    	public string TransferdBy{ get; set; }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "CustodianName", ResourceType = typeof(resxDbFields))]
    	public string CustodianName{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "MovementStatus", ResourceType = typeof(resxDbFields))]
    	public string MovementStatus{ get; set; }
    	
        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> CreateByGUID{ get; set; }
    	
        [Display(Name = "RequesterGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> RequesterGUID{ get; set; }
    	
        [Display(Name = "DocumentFlowStatusGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> DocumentFlowStatusGUID{ get; set; }
    	
    }
}