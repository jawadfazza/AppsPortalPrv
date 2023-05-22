
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TTT_DAL.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using RES_Repo.Globalization;
    using System.ComponentModel.DataAnnotations.Schema;
    using Portal_BL.Library;
    
    public partial class dataTenderingAndEvaluation
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TenderingAndEvaluationGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid TenderingAndEvaluationGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TenderGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid TenderGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BuyerGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid BuyerGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "SupervisorGUID", ResourceType = typeof(resxDbFields))]
    	public System.Guid SupervisorGUID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TenderBoxNumber", ResourceType = typeof(resxDbFields))]
    	public string TenderBoxNumber{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[DataType(DataType.DateTime)]
    	[Display(Name = "TenderLaunchingDate", ResourceType = typeof(resxDbFields))]
    	public System.DateTime TenderLaunchingDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "TenderLaunchingDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalTenderLaunchingDate { get { return new Portal().LocalTime(this.TenderLaunchingDate); } }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[DataType(DataType.DateTime)]
    	[Display(Name = "TenderClosingDate", ResourceType = typeof(resxDbFields))]
    	public System.DateTime TenderClosingDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "TenderClosingDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalTenderClosingDate { get { return new Portal().LocalTime(this.TenderClosingDate); } }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "TenderExtendedClosingDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> TenderExtendedClosingDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "TenderExtendedClosingDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalTenderExtendedClosingDate { get { return new Portal().LocalTime(this.TenderExtendedClosingDate); } }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "TenderBidOpeningDateTechnical", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> TenderBidOpeningDateTechnical{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "TenderBidOpeningDateTechnical", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalTenderBidOpeningDateTechnical { get { return new Portal().LocalTime(this.TenderBidOpeningDateTechnical); } }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "TenderTechnicalEvaluationDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> TenderTechnicalEvaluationDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "TenderTechnicalEvaluationDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalTenderTechnicalEvaluationDate { get { return new Portal().LocalTime(this.TenderTechnicalEvaluationDate); } }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "TenderBidOpeningDateFinancial", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> TenderBidOpeningDateFinancial{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "TenderBidOpeningDateFinancial", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalTenderBidOpeningDateFinancial { get { return new Portal().LocalTime(this.TenderBidOpeningDateFinancial); } }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "TenderFinancialEvaluationDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> TenderFinancialEvaluationDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "TenderFinancialEvaluationDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalTenderFinancialEvaluationDate { get { return new Portal().LocalTime(this.TenderFinancialEvaluationDate); } }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "TenderSampleSubmissionDeadline", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> TenderSampleSubmissionDeadline{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "TenderSampleSubmissionDeadline", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalTenderSampleSubmissionDeadline { get { return new Portal().LocalTime(this.TenderSampleSubmissionDeadline); } }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Active", ResourceType = typeof(resxDbFields))]
    	public bool Active{ get; set; }
    	
        public byte[] dataTenderingAndEvaluationRowVersion{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DeletedOn{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDeletedOn { get { return new Portal().LocalTime(this.DeletedOn); } }
    	
    
        public virtual dataTender dataTender { get; set; }
    }
}
