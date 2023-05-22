
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
    
    public partial class dataPartnersCapacityAssessmentDocEvaluation
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "PartnersCapacityAssessmentDocEvaluationGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid PartnersCapacityAssessmentDocEvaluationGUID{ get; set; }
    	
        [Display(Name = "PartnersCapacityAssessmentDocGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> PartnersCapacityAssessmentDocGUID{ get; set; }
    	
        [Display(Name = "PartnersCapacityAssessmentGUID", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.Guid> PartnersCapacityAssessmentGUID{ get; set; }
    	
        [Display(Name = "PartnerEvaluation", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> PartnerEvaluation{ get; set; }
    	
        [Display(Name = "AgancyEvaluation", ResourceType = typeof(resxDbFields))]
    	public Nullable<bool> AgancyEvaluation{ get; set; }
    	
        [Display(Name = "Comment", ResourceType = typeof(resxDbFields))]
    	public string Comment{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataPartnersCapacityAssessmentDocEvaluationRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
    
        public virtual codePartnersCapacityAssessmentDoc codePartnersCapacityAssessmentDoc { get; set; }
        public virtual dataPartnersCapacityAssessment dataPartnersCapacityAssessment { get; set; }
    }
}