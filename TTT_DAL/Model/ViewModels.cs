using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTT_DAL.Model
{
    public class TendersDataTableModel
    {
        public Guid TenderGUID { get; set; }
        public int TenderSequence { get; set; }
        public string TenderReference { get; set; }
        public string TenderType { get; set; }
        public string TenderTypeGUID { get; set; }
        public string TenderSubject { get; set; }
        public int TenderYear { get; set; }
        public string ProcurementPlanLine { get; set; }
        public string TenderStatusDescription { get; set; }
        public string CreatedByGUID { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }
        public bool Active { get; set; }
        public byte[] dataTenderRowVersion { get; set; }
    }
    public class TenderUpdateModel
    {
        public TenderUpdateModel()
        {

        }

        public bool FirstOpenCheck { get; set; }

        #region Tender Details
        public Guid TenderGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TenderSequence", ResourceType = typeof(resxDbFields))]
        public int TenderSequence { get; set; }

        [Display(Name = "TenderSequence", ResourceType = typeof(resxDbFields))]
        public int TenderSequenceDisplay { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TenderReference", ResourceType = typeof(resxDbFields))]
        public string TenderReference { get; set; }

        [Display(Name = "TenderReference", ResourceType = typeof(resxDbFields))]
        public string TenderReferenceDisplay { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TenderYear", ResourceType = typeof(resxDbFields))]
        public int TenderYear { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Range(1, Int32.MaxValue)]
        [Display(Name = "TenderReminderDays", ResourceType = typeof(resxDbFields))]
        public int TenderReminderDays { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TenderTypeGUID", ResourceType = typeof(resxDbFields))]
        public Guid TenderTypeGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(200, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TenderSubject", ResourceType = typeof(resxDbFields))]
        public string TenderSubject { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ProcurementPlanLine", ResourceType = typeof(resxDbFields))]
        public Guid ProcurementPlanLine { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.DateTime)]
        [Display(Name = "TenderDeadlineForClarification", ResourceType = typeof(resxDbFields))]
        public DateTime? TenderDeadlineForClarification { get; set; }


        [DataType(DataType.Date)]
        [Display(Name = "TenderSiteVisitDate", ResourceType = typeof(resxDbFields))]
        public DateTime? TenderSiteVisitDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "TenderDateForBidderConference", ResourceType = typeof(resxDbFields))]
        public DateTime? TenderDateForBidderConference { get; set; }


        [DataType(DataType.DateTime)]
        [Display(Name = "TenderCancelDate", ResourceType = typeof(resxDbFields))]
        public DateTime? TenderCancelDate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TenderStatusGUID", ResourceType = typeof(resxDbFields))]
        public Guid TenderStatusGUID { get; set; }

        [Display(Name = "TenderResolutionGUID", ResourceType = typeof(resxDbFields))]
        public Guid? TenderResolutionGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "NotifyBOC", ResourceType = typeof(resxDbFields))]
        public bool? NotifyBOC { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(resxDbFields))]
        public Guid? CreatedBy { get; set; }

        [Display(Name = "LastUpdatedBy", ResourceType = typeof(resxDbFields))]
        public Guid? LastUpdatedBy { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "DeletedOn", ResourceType = typeof(resxDbFields))]
        public DateTime? DeletedOn { get; set; }

        public byte[] dataTenderRowVersion { get; set; }





        #endregion

        #region TenderRequisitionModel






        public Guid TenderRequisitionGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "RequestingUnitGUID", ResourceType = typeof(resxDbFields))]
        public Guid RequestingUnitGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.Date)]
        [Display(Name = "RequestRecieved", ResourceType = typeof(resxDbFields))]
        public DateTime? RequestRecieved { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "IdentityHqOrSyrGUID", ResourceType = typeof(resxDbFields))]
        public Guid IdentityHqOrSyrGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BudgetSourceGUID", ResourceType = typeof(resxDbFields))]
        public Guid BudgetSourceGUID { get; set; }

        public byte[] dataTenderRequisitionRowVersion { get; set; }

        #endregion

        #region TenderRequisitionFocalPointsModel


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "FocalPointRUGUID", ResourceType = typeof(resxDbFields))]
        public List<Guid> FocalPointRUGUID { get; set; }


        #endregion

        #region TenderingAndEvaluationModel
        public Guid TenderingAndEvaluationGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BuyerGUID", ResourceType = typeof(resxDbFields))]
        public Guid BuyerGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "SupervisorGUID", ResourceType = typeof(resxDbFields))]
        public Guid SupervisorGUID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        //[StringLength(10, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TenderBoxNumbers", ResourceType = typeof(resxDbFields))]
        public List<string> TenderBoxNumbers { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [StringLength(10, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "TenderBoxNumber", ResourceType = typeof(resxDbFields))]
        public string TenderBoxNumber { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.DateTime)]
        [Display(Name = "TenderLaunchingDate", ResourceType = typeof(resxDbFields))]
        public DateTime? TenderLaunchingDate { get; set; }


        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [DataType(DataType.DateTime)]
        [Display(Name = "TenderClosingDate", ResourceType = typeof(resxDbFields))]
        public DateTime? TenderClosingDate { get; set; }


        [DataType(DataType.DateTime)]
        [Display(Name = "TenderExtendedClosingDate", ResourceType = typeof(resxDbFields))]
        public DateTime? TenderExtendedClosingDate { get; set; }



        [DataType(DataType.Date)]
        [Display(Name = "TenderBidOpeningDateTechnical", ResourceType = typeof(resxDbFields))]
        public DateTime? TenderBidOpeningDateTechnical { get; set; }


        [DataType(DataType.Date)]
        [Display(Name = "TenderTechnicalEvaluationDate", ResourceType = typeof(resxDbFields))]
        public DateTime? TenderTechnicalEvaluationDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "TenderBidOpeningDateFinancial", ResourceType = typeof(resxDbFields))]
        public DateTime? TenderBidOpeningDateFinancial { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "TenderFinancialEvaluationDate", ResourceType = typeof(resxDbFields))]
        public DateTime? TenderFinancialEvaluationDate { get; set; }


        [DataType(DataType.DateTime)]
        [Display(Name = "TenderSampleSubmissionDeadline", ResourceType = typeof(resxDbFields))]
        public DateTime? TenderSampleSubmissionDeadline { get; set; }

        public byte[] dataTenderingAndEvaluationRowVersion { get; set; }


        #endregion

        #region TenderEndorsementsAndApprovalsModel
        public Guid TenderEndorsementsAndApprovalsGUID { get; set; }

        [Display(Name = "HofoSupplyOfficerRepLccRccHccGUID", ResourceType = typeof(resxDbFields))]
        public Guid? HofoSupplyOfficerRepLccRccHccGUID { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "EndorsmentApprovalDate", ResourceType = typeof(resxDbFields))]
        public DateTime? EndorsmentApprovalDate { get; set; }


        [Display(Name = "EndorsmentApprovalNumber", ResourceType = typeof(resxDbFields))]
        public int? EndorsmentApprovalNumber { get; set; }

        public byte[] dataTenderEndorsementsAndApprovalsRowVersion { get; set; }


        #endregion

        #region TenderContractInformationsModel
        public Guid TenderContractInformationGUID { get; set; }


        [Display(Name = "FA1OfGUID", ResourceType = typeof(resxDbFields))]
        public Guid? FA1OfGUID { get; set; }


        [Display(Name = "AwardedAmountOrCeiling", ResourceType = typeof(resxDbFields))]
        public double? AwardedAmountOrCeiling { get; set; }

        [Display(Name = "AwardedCompanyContractNumber", ResourceType = typeof(resxDbFields))]
        public int? AwardedCompanyContractNumber { get; set; }


        [StringLength(5, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Currency", ResourceType = typeof(resxDbFields))]
        public string Currency { get; set; }

        public byte[] dataTenderContractInformationRowVersion { get; set; }

        #endregion

        #region TenderObservationAndRemarksModel
        public Guid TenderObservationAndRemarksGUID { get; set; }

        [Display(Name = "ObservationAndRemarks", ResourceType = typeof(resxDbFields))]
        public string ObservationAndRemarks { get; set; }

        public byte[] dataTenderObservationAndRemarksRowVersion { get; set; }

        #endregion

        #region Tender Awarded Companies

        [Display(Name = "AwardedCompanyGUIDs", ResourceType = typeof(resxDbFields))]
        public List<Guid> AwardedCompanyGUIDs { get; set; }

        #endregion

    }

    public class TenderBOCsDataTableModel
    {
        public Guid TenderBOCGUID { get; set; }
        public string BOCStaff { get; set; }
        public string DutyStationDescription { get; set; }
        public byte[] dataTenderBOCsRowVersion { get; set; }
    }


    public class PurchasingReportsDataTableModel
    {
        public System.Guid PurchasingReportGUID { get; set; }

        public string ReportID { get; set; }
        public Nullable<System.DateTime> RunDate { get; set; }
        public string BusinessUnit { get; set; }
        public string Buyer { get; set; }
        public string Vendor { get; set; }

        public string PuchaseOrder { get; set; }
        public Nullable<System.DateTime> BudgetDateFrom { get; set; }
        public Nullable<System.DateTime> BudgetDateTo { get; set; }

        public Nullable<double> RemainingAmount { get; set; }
        public Nullable<int> BgtYr { get; set; }

        public Nullable<int> CostCentre { get; set; }
        public bool Active { get; set; }
        public byte[] dataPurchasingReportRowVersion { get; set; }
    }

    public class PurchasingReportUpdateModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PurchasingReportGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid PurchasingReportGUID { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "ReportID", ResourceType = typeof(resxDbFields))]
        public string ReportID { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "RunDate", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> RunDate { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "BusinessUnit", ResourceType = typeof(resxDbFields))]
        public string BusinessUnit { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Buyer", ResourceType = typeof(resxDbFields))]
        public string Buyer { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Vendor", ResourceType = typeof(resxDbFields))]
        public string Vendor { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthMax", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "PuchaseOrder", ResourceType = typeof(resxDbFields))]
        public string PuchaseOrder { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "BudgetDateFrom", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> BudgetDateFrom { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "BudgetDateTo", ResourceType = typeof(resxDbFields))]
        public Nullable<System.DateTime> BudgetDateTo { get; set; }

        [Display(Name = "RemainingAmount", ResourceType = typeof(resxDbFields))]
        public Nullable<double> RemainingAmount { get; set; }

        [Display(Name = "BgtYr", ResourceType = typeof(resxDbFields))]
        public Nullable<int> BgtYr { get; set; }

        [Display(Name = "CostCentre", ResourceType = typeof(resxDbFields))]
        public Nullable<int> CostCentre { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(resxHttpErrors))]
        [Display(Name = "Active", ResourceType = typeof(resxDbFields))]
        public bool Active { get; set; }

        public byte[] dataPurchasingReportRowVersion { get; set; }
    }
}
