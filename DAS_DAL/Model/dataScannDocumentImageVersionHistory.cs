
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
    
    public partial class dataScannDocumentImageVersionHistory
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ScannDocumentImageVersionHistoryGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ScannDocumentImageVersionHistoryGUID{ get; set; }
    	
        [Display(Name = "ScannDocumentVersionHistoryGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ScannDocumentVersionHistoryGUID{ get; set; }
    	
        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> CreateByGUID{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> CreateDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalCreateDate { get { return new Portal().LocalTime(this.CreateDate); } }
    	
        [StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ImageName", ResourceType = typeof(resxDbFields))]
    	public string ImageName{ get; set; }
    	
        [Display(Name = "ImageNumber", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> ImageNumber{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataScannDocumentImageVersionHistoryRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
        [Display(Name = "Archived", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> Archived{ get; set; }
    	
    
        public virtual userAccounts userAccounts { get; set; }
        public virtual dataScannDocumentVersionHistory dataScannDocumentVersionHistory { get; set; }
    }
}
