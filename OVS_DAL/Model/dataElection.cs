
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OVS_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class dataElection
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public dataElection()
        {
            this.dataElectionCandidate = new HashSet<dataElectionCandidate>();
            this.dataElectionCondition = new HashSet<dataElectionCondition>();
            this.dataElectionCorrespondence = new HashSet<dataElectionCorrespondence>();
            this.dataElectionLanguage = new HashSet<dataElectionLanguage>();
            this.dataElectionStaff = new HashSet<dataElectionStaff>();
        }
    
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ElectionGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ElectionGUID{ get; set; }
    	
        [Display(Name = "ElectionCorrespondenceTypeGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ElectionCorrespondenceTypeGUID{ get; set; }
    	
        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> OrganizationInstanceGUID{ get; set; }
    	
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> DutyStationGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(100, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TimeZone", ResourceType = typeof(resxDbFields))]
    	public string TimeZone{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[DataType(DataType.DateTime)]
    	[Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]
    	public System.DateTime StartDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "StartDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalStartDate { get { return new Portal().LocalTime(this.StartDate); } }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[DataType(DataType.DateTime)]
    	[Display(Name = "CloseDate", ResourceType = typeof(resxDbFields))]
    	public System.DateTime CloseDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "CloseDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalCloseDate { get { return new Portal().LocalTime(this.CloseDate); } }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "ResultAvaiableDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> ResultAvaiableDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "ResultAvaiableDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalResultAvaiableDate { get { return new Portal().LocalTime(this.ResultAvaiableDate); } }
    	
        [Display(Name = "CustomMessage", ResourceType = typeof(resxDbFields))]
    	public string CustomMessage{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IsShuffling", ResourceType = typeof(resxDbFields))]
    	public bool IsShuffling{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IsNeedVoteConfirmationMail", ResourceType = typeof(resxDbFields))]
    	public bool IsNeedVoteConfirmationMail{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IsDisplayCandidateImage", ResourceType = typeof(resxDbFields))]
    	public bool IsDisplayCandidateImage{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IsResultShared", ResourceType = typeof(resxDbFields))]
    	public bool IsResultShared{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IsDisplayReviewPage", ResourceType = typeof(resxDbFields))]
    	public bool IsDisplayReviewPage{ get; set; }
    	
        [Display(Name = "IsArchived", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> IsArchived{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataElectionRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataElectionCandidate> dataElectionCandidate { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataElectionCondition> dataElectionCondition { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataElectionCorrespondence> dataElectionCorrespondence { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataElectionLanguage> dataElectionLanguage { get; set; }
        public virtual codeElectionCorrespondenceType codeElectionCorrespondenceType { get; set; }
        public virtual codeDutyStations codeDutyStations { get; set; }
        public virtual codeOrganizationsInstances codeOrganizationsInstances { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataElectionStaff> dataElectionStaff { get; set; }
    }
}
