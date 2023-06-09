
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
    
    public partial class dataFileRequest
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public dataFileRequest()
        {
            this.dataScannDocumentTransfer = new HashSet<dataScannDocumentTransfer>();
        }
    
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "FileRequestGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid FileRequestGUID{ get; set; }
    	
        [Display(Name = "FileGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> FileGUID{ get; set; }
    	
        [Display(Name = "RequesterTypeGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> RequesterTypeGUID{ get; set; }
    	
        [Display(Name = "RequesterGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> RequesterGUID{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "RequestDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> RequestDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "RequestDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalRequestDate { get { return new Portal().LocalTime(this.RequestDate); } }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "RequestDurationDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> RequestDurationDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "RequestDurationDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalRequestDurationDate { get { return new Portal().LocalTime(this.RequestDurationDate); } }
    	
        [Display(Name = "RequestStatusGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> RequestStatusGUID{ get; set; }
    	
        [Display(Name = "IsLastRequest", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> IsLastRequest{ get; set; }
    	
        [StringLength(1000, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Comments", ResourceType = typeof(resxDbFields))]
    	public string Comments{ get; set; }
    	
        [Display(Name = "DocumentStatusOnReturnGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> DocumentStatusOnReturnGUID{ get; set; }
    	
        [Display(Name = "RequsterFromNameGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> RequsterFromNameGUID{ get; set; }
    	
        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "RequsterFromName", ResourceType = typeof(resxDbFields))]
    	public string RequsterFromName{ get; set; }
    	
        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "RequsterName", ResourceType = typeof(resxDbFields))]
    	public string RequsterName{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataFileRequestRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
        [Display(Name = "Archived", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> Archived{ get; set; }
    	
        [Display(Name = "OrderId", ResourceType = typeof(resxDbFields))]
    	public Nullable<int> OrderId{ get; set; }
    	
    
        public virtual dataFile dataFile { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataScannDocumentTransfer> dataScannDocumentTransfer { get; set; }
    }
}
