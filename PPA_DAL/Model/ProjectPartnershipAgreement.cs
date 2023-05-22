
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PPA_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class ProjectPartnershipAgreement
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProjectPartnershipAgreement()
        {
            this.PPADistributionList = new HashSet<PPADistributionList>();
            this.PPAOriginalFile = new HashSet<PPAOriginalFile>();
            this.PPAReviewerList = new HashSet<PPAReviewerList>();
        }
    
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PPAGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid PPAGUID{ get; set; }
    	
        [Display(Name = "RefferalGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> RefferalGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PPATypeGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid PPATypeGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ImplementationAreaGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ImplementationAreaGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PPAStatusGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid PPAStatusGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ReminderInterval", ResourceType = typeof(resxDbFields))]
    	public int ReminderInterval{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PPADescription", ResourceType = typeof(resxDbFields))]
    	public string PPADescription{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[DataType(DataType.DateTime)]
    	[Display(Name = "PPADeadLine", ResourceType = typeof(resxDbFields))]
    	public System.DateTime PPADeadLine{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "PPADeadLine", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalPPADeadLine { get { return new Portal().LocalTime(this.PPADeadLine); } }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PPAFolderPath", ResourceType = typeof(resxDbFields))]
    	public string PPAFolderPath{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid OrganizationInstanceGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "CreatedByUserProfileGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid CreatedByUserProfileGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] ProjectPartnershipAgreementRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PPADistributionList> PPADistributionList { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PPAOriginalFile> PPAOriginalFile { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PPAReviewerList> PPAReviewerList { get; set; }
    }
}