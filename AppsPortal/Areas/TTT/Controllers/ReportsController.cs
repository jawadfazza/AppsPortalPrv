using AppsPortal.BaseControllers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.TTT.Controllers
{
    public class ReportsController : TTTBaseController
    {
        // GET: TTT/Reports
        public ActionResult Index()
        {
            return View("~/Areas/TTT/Views/Reports/Index.cshtml");
        }

        [Route("TTT/Reports/GenerateTrackingSheetReport/{TenderYear}")]
        public ActionResult GenerateTrackingSheetReport(int TenderYear)
        {

            var Tenders = (from a in DbTTT.v_TrackingSheetReport
                           where a.TenderYear == TenderYear
                           orderby a.TenderSequence ascending
                           select a).ToList();

            string sourceFile = Server.MapPath("~/Areas/TTT/Templates/Tracking_Sheet_Report_Template.xlsx");

            string DisFolder = Server.MapPath("~/Areas/TTT/Templates/Tracking Sheet Report" + DateTime.Now.ToBinary() + ".xlsx");

            System.IO.File.Copy(sourceFile, DisFolder);


            using (var package = new ExcelPackage(new FileInfo(DisFolder)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                //
                int i = 4;
                foreach (var tender in Tenders)
                {
                    worksheet.Cells["A" + i].Value = "Syria";//Operation A4
                    try { worksheet.Cells["B" + i].Value = tender.TenderSequence.ToString(); } catch { }//Seq B4
                    try { worksheet.Cells["C" + i].Value = tender.TenderReference.ToString(); } catch { }//Tender Reference C4
                    try { worksheet.Cells["D" + i].Value = tender.TenderType.ToString(); } catch { }//Tender Type D4
                    try { worksheet.Cells["E" + i].Value = tender.ProcurementPlanLine.ToString(); } catch { }//Office E4
                    try { worksheet.Cells["F" + i].Value = tender.TenderSubject.ToString(); } catch { }//Description of goods and/or services F4
                    //try { worksheet.Cells["G" + i].Value = tender.IncludedInTheOriginalProcurementPlan.ToString(); } catch { }//Included in the Original Procurement Plan G4
                    //try { worksheet.Cells["H" + i].Value = tender.EstimatedValueOfTheProcurementUSD.ToString(); } catch { }//Estimated Value of the procurement (USD)  H4
                    try { worksheet.Cells["G" + i].Value = tender.TenderStatus.ToString(); } catch { } //Status I4
                    try { worksheet.Cells["H" + i].Value = tender.TenderCancelDate.HasValue ? tender.TenderCancelDate.Value.ToShortDateString() : ""; } catch { } //If cancelled - Date of cancelation J4
                    try { worksheet.Cells["I" + i].Value = tender.Requesting_Unit.ToString(); } catch { } //Requesting Unit K4
                    try { worksheet.Cells["J" + i].Value = tender.FocalPoints.ToString(); } catch { }//Focal Point RU L4
                    try { worksheet.Cells["K" + i].Value = tender.RequestReceivedDate.ToShortDateString(); } catch { }//Request received Date M4
                    //try { worksheet.Cells["N" + i].Value = tender.ReqNumber.ToString(); } catch { } //REQ # (Identify BU) N4
                    try { worksheet.Cells["L" + i].Value = tender.BudgetSource.ToString(); } catch { } //Budget Source (ABOD/OPS) O4
                    //try { worksheet.Cells["P" + i].Value = tender.ReceptionSoW.ToString(); } catch { }//Reception  SoW / BoQ / ToR Date P4
                    try { worksheet.Cells["M" + i].Value = tender.Buyer.ToString(); } catch { }//Supply Staff Assigned (Buyer) Q4
                    try { worksheet.Cells["N" + i].Value = tender.TenderLaunchingDate.ToShortDateString(); } catch { }//Launching Date R4
                    try
                    { worksheet.Cells["O" + i].Value = tender.TenderExtendedClosingDate.HasValue ? tender.TenderExtendedClosingDate.Value.ToShortDateString() : tender.TenderClosingDate.ToShortDateString(); }
                    catch { }//Closing Date S4
                    try
                    { worksheet.Cells["P" + i].Value = tender.TenderBidOpeningDateTechnical.HasValue ? tender.TenderBidOpeningDateTechnical.Value.ToShortDateString() : ""; }
                    catch { }//Bid Opening Date [Technical] T4
                    //try { worksheet.Cells["U" + i].Value = tender.HandingDateOfProposal.ToString(); } catch { }//Handing Date of Proposal to TEC U4
                    //try { worksheet.Cells["V" + i].Value = tender.SubmissionOfTECEvaluation.ToString(); } catch { }//Submission of TEC evaluation to Supply Date  V4
                    try { worksheet.Cells["Q" + i].Value = tender.TenderBidOpeningDateFinancial.HasValue ? tender.TenderBidOpeningDateFinancial.Value.ToShortDateString() : ""; } catch { }//Bid Opening Date [Financial] W4
                    //try { worksheet.Cells["X" + i].Value = tender.DatOofFinalizationOfSubmissionPackage.ToString(); } catch { }//Date of Finalization of Submission Package X4
                    try { worksheet.Cells["R" + i].Value = tender.ApprovalAuthority.ToString(); } catch { }//Approval Authority Y4
                    try { worksheet.Cells["S" + i].Value = tender.DecisionDate.HasValue ? tender.DecisionDate.Value.ToShortDateString() : ""; } catch { } //Decision Date Z4
                    try { worksheet.Cells["T" + i].Value = tender.ReferenceContract.ToString(); } catch { }//Reference Contract FA / PO  AA4
                    //try { worksheet.Cells["AB" + i].Value = tender.ContractEndDate.ToString(); } catch { }//Contract End Date AB4
                    try { worksheet.Cells["U" + i].Value = tender.AwardedCompanies.ToString(); } catch { }//Awarded Company AC4
                    try { worksheet.Cells["V" + i].Value = tender.AwardedAmountOrCeiling; } catch { }//Awarded Amount/Ceiling (USD) AD4
                    try { worksheet.Cells["W" + i].Value = tender.AwardedAmountCurrency; } catch { }//Awarded Amount/Ceiling (USD) AD4

                    try
                    { worksheet.Cells["X" + i].Value = tender.ObservationAndRemarks; }
                    catch { }//Observations / Remarks AE4
                    i++;
                }
                //foreach (var tender in Tenders)
                //{
                //    worksheet.Cells["A" + i].Value = "Syria";//Operation A4
                //    try { worksheet.Cells["B" + i].Value = tender.TenderSequence.ToString(); } catch { }//Seq B4
                //    try { worksheet.Cells["C" + i].Value = tender.TenderReference.ToString(); } catch { }//Tender Reference C4
                //    try { worksheet.Cells["D" + i].Value = tender.TenderType.ToString(); } catch { }//Tender Type D4
                //    try { worksheet.Cells["E" + i].Value = tender.ProcurementPlanLine.ToString(); } catch { }//Office E4
                //    try { worksheet.Cells["F" + i].Value = tender.TenderSubject.ToString(); } catch { }//Description of goods and/or services F4
                //    //try { worksheet.Cells["G" + i].Value = tender.IncludedInTheOriginalProcurementPlan.ToString(); } catch { }//Included in the Original Procurement Plan G4
                //    //try { worksheet.Cells["H" + i].Value = tender.EstimatedValueOfTheProcurementUSD.ToString(); } catch { }//Estimated Value of the procurement (USD)  H4
                //    try { worksheet.Cells["I" + i].Value = tender.TenderStatus.ToString(); } catch { } //Status I4
                //    try { worksheet.Cells["J" + i].Value = tender.TenderCancelDate.HasValue ? tender.TenderCancelDate.Value.ToShortDateString() : ""; } catch { } //If cancelled - Date of cancelation J4
                //    try { worksheet.Cells["K" + i].Value = tender.Requesting_Unit.ToString(); } catch { } //Requesting Unit K4
                //    try { worksheet.Cells["L" + i].Value = tender.FocalPoints.ToString(); } catch { }//Focal Point RU L4
                //    try { worksheet.Cells["M" + i].Value = tender.RequestReceivedDate.ToShortDateString(); } catch { }//Request received Date M4
                //    //try { worksheet.Cells["N" + i].Value = tender.ReqNumber.ToString(); } catch { } //REQ # (Identify BU) N4
                //    try { worksheet.Cells["O" + i].Value = tender.BudgetSource.ToString(); } catch { } //Budget Source (ABOD/OPS) O4
                //    //try { worksheet.Cells["P" + i].Value = tender.ReceptionSoW.ToString(); } catch { }//Reception  SoW / BoQ / ToR Date P4
                //    try { worksheet.Cells["Q" + i].Value = tender.Buyer.ToString(); } catch { }//Supply Staff Assigned (Buyer) Q4
                //    try { worksheet.Cells["R" + i].Value = tender.TenderLaunchingDate.ToShortDateString(); } catch { }//Launching Date R4
                //    try
                //    { worksheet.Cells["S" + i].Value = tender.TenderExtendedClosingDate.HasValue ? tender.TenderExtendedClosingDate.Value.ToShortDateString() : tender.TenderClosingDate.ToShortDateString(); }
                //    catch { }//Closing Date S4
                //    try
                //    { worksheet.Cells["T" + i].Value = tender.TenderBidOpeningDateTechnical.HasValue ? tender.TenderBidOpeningDateTechnical.Value.ToShortDateString() : ""; }
                //    catch { }//Bid Opening Date [Technical] T4
                //    //try { worksheet.Cells["U" + i].Value = tender.HandingDateOfProposal.ToString(); } catch { }//Handing Date of Proposal to TEC U4
                //    //try { worksheet.Cells["V" + i].Value = tender.SubmissionOfTECEvaluation.ToString(); } catch { }//Submission of TEC evaluation to Supply Date  V4
                //    try { worksheet.Cells["W" + i].Value = tender.TenderBidOpeningDateFinancial.HasValue ? tender.TenderBidOpeningDateFinancial.Value.ToShortDateString() : ""; } catch { }//Bid Opening Date [Financial] W4
                //    //try { worksheet.Cells["X" + i].Value = tender.DatOofFinalizationOfSubmissionPackage.ToString(); } catch { }//Date of Finalization of Submission Package X4
                //    try { worksheet.Cells["Y" + i].Value = tender.ApprovalAuthority.ToString(); } catch { }//Approval Authority Y4
                //    try { worksheet.Cells["Z" + i].Value = tender.DecisionDate.ToString(); } catch { } //Decision Date Z4
                //    try { worksheet.Cells["AA" + i].Value = tender.ReferenceContract.ToString(); } catch { }//Reference Contract FA / PO  AA4
                //    //try { worksheet.Cells["AB" + i].Value = tender.ContractEndDate.ToString(); } catch { }//Contract End Date AB4
                //    try { worksheet.Cells["AC" + i].Value = tender.AwardedCompanies.ToString(); } catch { }//Awarded Company AC4
                //    try { worksheet.Cells["AD" + i].Value = tender.AwardedAmountOrCeiling; } catch { }//Awarded Amount/Ceiling (USD) AD4
                //    try { worksheet.Cells["AE" + i].Value = tender.AwardedAmountCurrency; } catch { }//Awarded Amount/Ceiling (USD) AD4

                //    try
                //    { worksheet.Cells["AF" + i].Value = tender.ObservationAndRemarks; }
                //    catch { }//Observations / Remarks AE4
                //    i++;
                //}
                package.Save();
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);
            string fileName = "Tracking Sheet Report " + DateTime.Now.ToString("dddd, dd MMMM yyyy") + ".xlsx";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}