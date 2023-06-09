
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace REF_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class dataReferralHistory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public dataReferralHistory()
        {
            this.dataReferralHistoryComment = new HashSet<dataReferralHistoryComment>();
        }
    
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ReferralHistoryGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ReferralHistoryGUID{ get; set; }
    	
        [Display(Name = "ReferralStepGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ReferralStepGUID{ get; set; }
    	
        [Display(Name = "ReferralStatusGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ReferralStatusGUID{ get; set; }
    	
        [StringLength(500, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Title", ResourceType = typeof(resxDbFields))]
    	public string Title{ get; set; }
    	
        [Display(Name = "OrganizationInstanceGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> OrganizationInstanceGUID{ get; set; }
    	
        [Display(Name = "DutyStationGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> DutyStationGUID{ get; set; }
    	
        [Display(Name = "DepartmentGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> DepartmentGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataReferralHistoryRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
    
        public virtual configReferralStep configReferralStep { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataReferralHistoryComment> dataReferralHistoryComment { get; set; }
        public virtual codeReferralStatus codeReferralStatus { get; set; }
    }
}
