
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
    
    public partial class codeDASTemplateTypeDocumentTag
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public codeDASTemplateTypeDocumentTag()
        {
            this.dataArchiveTemplateDocumentTag = new HashSet<dataArchiveTemplateDocumentTag>();
        }
    
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TemplateTypeDocumentTagGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid TemplateTypeDocumentTagGUID{ get; set; }
    	
        [Display(Name = "TemplateTypeDocumentGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> TemplateTypeDocumentGUID{ get; set; }
    	
        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TagName", ResourceType = typeof(resxDbFields))]
    	public string TagName{ get; set; }
    	
        [StringLength(4000, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Description", ResourceType = typeof(resxDbFields))]
    	public string Description{ get; set; }
    	
        [Display(Name = "IsMandatury", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> IsMandatury{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> CreateDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "CreateDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalCreateDate { get { return new Portal().LocalTime(this.CreateDate); } }
    	
        [Display(Name = "CreateByGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> CreateByGUID{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> UpdateDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "UpdateDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalUpdateDate { get { return new Portal().LocalTime(this.UpdateDate); } }
    	
        [Display(Name = "UpdateByGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> UpdateByGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] codeDASTemplateTypeDocumentTagRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
        [Display(Name = "Archived", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> Archived{ get; set; }
    	
    
        public virtual codeDASTemplateTypeDocument codeDASTemplateTypeDocument { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataArchiveTemplateDocumentTag> dataArchiveTemplateDocumentTag { get; set; }
    }
}
