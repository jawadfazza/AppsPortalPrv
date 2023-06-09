
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
    
    public partial class userAccounts
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public userAccounts()
        {
            this.codeDASDestinationUnitFocalPoint = new HashSet<codeDASDestinationUnitFocalPoint>();
            this.dataDefaultScannerSetting = new HashSet<dataDefaultScannerSetting>();
            this.dataFileLocationMovement = new HashSet<dataFileLocationMovement>();
            this.dataScannDocument = new HashSet<dataScannDocument>();
            this.dataScannDocumentImage = new HashSet<dataScannDocumentImage>();
            this.dataScannDocumentImageVersionHistory = new HashSet<dataScannDocumentImageVersionHistory>();
            this.dataScannDocumentTransfer = new HashSet<dataScannDocumentTransfer>();
            this.dataScannDocumentTransfer1 = new HashSet<dataScannDocumentTransfer>();
            this.dataScannDocumentTransferFlow = new HashSet<dataScannDocumentTransferFlow>();
            this.dataScannDocumentVersionHistory = new HashSet<dataScannDocumentVersionHistory>();
            this.dataUploadUserFile = new HashSet<dataUploadUserFile>();
            this.userServiceHistory = new HashSet<userServiceHistory>();
            this.dataArchiveTemplate = new HashSet<dataArchiveTemplate>();
            this.dataArchiveTemplate1 = new HashSet<dataArchiveTemplate>();
            this.dataArchiveTemplateDocumentImageVersionHistory = new HashSet<dataArchiveTemplateDocumentImageVersionHistory>();
            this.dataArchiveTemplateDocumentSoftField = new HashSet<dataArchiveTemplateDocumentSoftField>();
            this.dataArchiveTemplateDocumentTag = new HashSet<dataArchiveTemplateDocumentTag>();
            this.dataArchiveTemplateDocumentVersionHistory = new HashSet<dataArchiveTemplateDocumentVersionHistory>();
            this.dataArchiveTemplateDocumentImage = new HashSet<dataArchiveTemplateDocumentImage>();
            this.dataArchiveTemplateDocument = new HashSet<dataArchiveTemplateDocument>();
            this.dataArchiveTemplateDocument1 = new HashSet<dataArchiveTemplateDocument>();
        }
    
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "UserGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid UserGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TimeZone", ResourceType = typeof(resxDbFields))]
    	public string TimeZone{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[DataType(DataType.DateTime)]
    	[Display(Name = "RequestedOn", ResourceType = typeof(resxDbFields))]
    	public System.DateTime RequestedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "RequestedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalRequestedOn { get { return new Portal().LocalTime(this.RequestedOn); } }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "SecurityQuestionGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid SecurityQuestionGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "SecurityAnswer", ResourceType = typeof(resxDbFields))]
    	public string SecurityAnswer{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "AccountStatusID", ResourceType = typeof(resxDbFields))]
    	public int AccountStatusID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IsFirstLogin", ResourceType = typeof(resxDbFields))]
    	public bool IsFirstLogin{ get; set; }
    	
        [Display(Name = "HasPhoto", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> HasPhoto{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] userAccountsRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
        [DataType(DataType.Password)]
    	[Display(Name = "PasswordResetToken", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> PasswordResetToken{ get; set; }
    	
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<codeDASDestinationUnitFocalPoint> codeDASDestinationUnitFocalPoint { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataDefaultScannerSetting> dataDefaultScannerSetting { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataFileLocationMovement> dataFileLocationMovement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataScannDocument> dataScannDocument { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataScannDocumentImage> dataScannDocumentImage { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataScannDocumentImageVersionHistory> dataScannDocumentImageVersionHistory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataScannDocumentTransfer> dataScannDocumentTransfer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataScannDocumentTransfer> dataScannDocumentTransfer1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataScannDocumentTransferFlow> dataScannDocumentTransferFlow { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataScannDocumentVersionHistory> dataScannDocumentVersionHistory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataUploadUserFile> dataUploadUserFile { get; set; }
        public virtual userPersonalDetails userPersonalDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<userServiceHistory> userServiceHistory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataArchiveTemplate> dataArchiveTemplate { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataArchiveTemplate> dataArchiveTemplate1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataArchiveTemplateDocumentImageVersionHistory> dataArchiveTemplateDocumentImageVersionHistory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataArchiveTemplateDocumentSoftField> dataArchiveTemplateDocumentSoftField { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataArchiveTemplateDocumentTag> dataArchiveTemplateDocumentTag { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataArchiveTemplateDocumentVersionHistory> dataArchiveTemplateDocumentVersionHistory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataArchiveTemplateDocumentImage> dataArchiveTemplateDocumentImage { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataArchiveTemplateDocument> dataArchiveTemplateDocument { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataArchiveTemplateDocument> dataArchiveTemplateDocument1 { get; set; }
    }
}
