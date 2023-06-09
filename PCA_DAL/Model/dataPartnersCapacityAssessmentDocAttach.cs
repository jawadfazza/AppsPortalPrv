
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PCA_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class dataPartnersCapacityAssessmentDocAttach
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PartnersCapacityAssessmentDocAttachGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid PartnersCapacityAssessmentDocAttachGUID{ get; set; }
    	
        [Display(Name = "PartnersCapacityAssessmentDocTitleGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> PartnersCapacityAssessmentDocTitleGUID{ get; set; }
    	
        [Display(Name = "PartnersCapacityAssessmentGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> PartnersCapacityAssessmentGUID{ get; set; }
    	
        [Display(Name = "Checked", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> Checked{ get; set; }
    	
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Ext", ResourceType = typeof(resxDbFields))]
    	public string Ext{ get; set; }
    	
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "SupportDocTitle", ResourceType = typeof(resxDbFields))]
    	public string SupportDocTitle{ get; set; }
    	
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> Active{ get; set; }
    	
        public string dataPartnersCapacityAssessmentDocAttachRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
    
        public virtual codePartnersCapacityAssessmentDocTitle codePartnersCapacityAssessmentDocTitle { get; set; }
        public virtual dataPartnersCapacityAssessment dataPartnersCapacityAssessment { get; set; }
    }
}
