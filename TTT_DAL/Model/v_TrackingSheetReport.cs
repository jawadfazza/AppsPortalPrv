
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
    
    public partial class v_TrackingSheetReport
    {
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "EDMXID", ResourceType = typeof(resxDbFields))]
    	public int EDMXID{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(5, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Operation", ResourceType = typeof(resxDbFields))]
    	public string Operation{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TenderSequence", ResourceType = typeof(resxDbFields))]
    	public int TenderSequence{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TenderReference", ResourceType = typeof(resxDbFields))]
    	public string TenderReference{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TenderYear", ResourceType = typeof(resxDbFields))]
    	public int TenderYear{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TenderType", ResourceType = typeof(resxDbFields))]
    	public string TenderType{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ProcurementPlanLine", ResourceType = typeof(resxDbFields))]
    	public string ProcurementPlanLine{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TenderSubject", ResourceType = typeof(resxDbFields))]
    	public string TenderSubject{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(6, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "IncludedInTheOriginalProcurementPlan", ResourceType = typeof(resxDbFields))]
    	public string IncludedInTheOriginalProcurementPlan{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(6, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "EstimatedValueOfTheProcurementUSD", ResourceType = typeof(resxDbFields))]
    	public string EstimatedValueOfTheProcurementUSD{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TenderStatus", ResourceType = typeof(resxDbFields))]
    	public string TenderStatus{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "TenderCancelDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> TenderCancelDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "TenderCancelDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalTenderCancelDate { get { return new Portal().LocalTime(this.TenderCancelDate); } }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(150, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Requesting_Unit", ResourceType = typeof(resxDbFields))]
    	public string Requesting_Unit{ get; set; }
    	
        [StringLength(4000, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "FocalPoints", ResourceType = typeof(resxDbFields))]
    	public string FocalPoints{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "RequestReceivedDate", ResourceType = typeof(resxDbFields))]
    	public System.DateTime RequestReceivedDate{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(6, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ReqNumber", ResourceType = typeof(resxDbFields))]
    	public string ReqNumber{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "BudgetSource", ResourceType = typeof(resxDbFields))]
    	public string BudgetSource{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(6, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ReceptionSoW", ResourceType = typeof(resxDbFields))]
    	public string ReceptionSoW{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(101, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "Buyer", ResourceType = typeof(resxDbFields))]
    	public string Buyer{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TenderLaunchingDate", ResourceType = typeof(resxDbFields))]
    	public System.DateTime TenderLaunchingDate{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "TenderClosingDate", ResourceType = typeof(resxDbFields))]
    	public System.DateTime TenderClosingDate{ get; set; }
    	
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
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(6, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "HandingDateOfProposal", ResourceType = typeof(resxDbFields))]
    	public string HandingDateOfProposal{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(6, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "SubmissionOfTECEvaluation", ResourceType = typeof(resxDbFields))]
    	public string SubmissionOfTECEvaluation{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "TenderBidOpeningDateFinancial", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> TenderBidOpeningDateFinancial{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "TenderBidOpeningDateFinancial", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalTenderBidOpeningDateFinancial { get { return new Portal().LocalTime(this.TenderBidOpeningDateFinancial); } }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(6, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "DatOofFinalizationOfSubmissionPackage", ResourceType = typeof(resxDbFields))]
    	public string DatOofFinalizationOfSubmissionPackage{ get; set; }
    	
        [StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ApprovalAuthority", ResourceType = typeof(resxDbFields))]
    	public string ApprovalAuthority{ get; set; }
    	
        [DataType(DataType.DateTime)]
    	[Display(Name = "DecisionDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> DecisionDate{ get; set; }
    	
    	[NotMapped]
    	[Display(Name = "DecisionDate", ResourceType = typeof(resxDbFields))]
    	public Nullable<System.DateTime> LocalDecisionDate { get { return new Portal().LocalTime(this.DecisionDate); } }
    	
        [StringLength(256, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ReferenceContract", ResourceType = typeof(resxDbFields))]
    	public string ReferenceContract{ get; set; }
    	
        [Required(ErrorMessageResourceName = "RequiredField" , ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[StringLength(6, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "ContractEndDate", ResourceType = typeof(resxDbFields))]
    	public string ContractEndDate{ get; set; }
    	
        [StringLength(4000, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "AwardedCompanies", ResourceType = typeof(resxDbFields))]
    	public string AwardedCompanies{ get; set; }
    	
        [Display(Name = "AwardedAmountOrCeiling", ResourceType = typeof(resxDbFields))]
    	public Nullable<double> AwardedAmountOrCeiling{ get; set; }
    	
        [StringLength(5, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
    	[Display(Name = "AwardedAmountCurrency", ResourceType = typeof(resxDbFields))]
    	public string AwardedAmountCurrency{ get; set; }
    	
        [Display(Name = "ObservationAndRemarks", ResourceType = typeof(resxDbFields))]
    	public string ObservationAndRemarks{ get; set; }
    	
    }
}
