
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
    
    public partial class dataElectionCorrespondence
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public dataElectionCorrespondence()
        {
            this.dataElectionCorrespondenceStaff = new HashSet<dataElectionCorrespondenceStaff>();
        }
    
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ElectionCorrespondenceGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ElectionCorrespondenceGUID{ get; set; }
    	
        [Display(Name = "ElectionGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> ElectionGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ElectionCorrespondenceTypeGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid ElectionCorrespondenceTypeGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[DataType(DataType.DateTime)]
    	[Display(Name = "SendDate", ResourceType = typeof(resxDbFields))]
    	public System.DateTime SendDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "SendDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalSendDate { get { return new Portal().LocalTime(this.SendDate); } }
    	
        [StringLength(300, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "MessageTitle", ResourceType = typeof(resxDbFields))]
    	public string MessageTitle{ get; set; }
    	
        [Display(Name = "MessageBody", ResourceType = typeof(resxDbFields))]
    	public string MessageBody{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "LanguageID", ResourceType = typeof(resxDbFields))]
    	public string LanguageID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataElectionCorrespondenceRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
    
        public virtual dataElection dataElection { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dataElectionCorrespondenceStaff> dataElectionCorrespondenceStaff { get; set; }
        public virtual codeElectionCorrespondenceType codeElectionCorrespondenceType { get; set; }
    }
}
