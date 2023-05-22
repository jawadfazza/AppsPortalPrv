using AppsPortal.Areas.EMT.RDLC;
using AppsPortal.Areas.EMT.RDLC.EMTDataSetTableAdapters;
using AppsPortal.BaseControllers;
using AppsPortal.Data;
using AppsPortal.Library;
using EMT_DAL.Model;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.EMT.Controllers
{
    public class ReportsController : EMTBaseController
    {
        // GET: EMT/Reports
        public ActionResult Index()
        {
            return View();
        }
        private EMTReportParametersList FillRP(EMTReportParametersList rp)
        {
            if (rp.OrganizationInstanceGUID == null) { rp.OrganizationInstanceGUID = DropDownList.OrganizationsInstancesPharmacyAcronymByProfileAll().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            if (rp.MedicalPharmacyGUID == null) { rp.MedicalPharmacyGUID = DropDownList.PharmacyByOrganizationInstance(string.Join(",", rp.OrganizationInstanceGUID)).Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray() ; }
            //if (rp.MedicalItemGUID == null) { rp.MedicalItemGUID = DropDownList.MedicalItems().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            //if (rp.MedicalPharmacologicalFormGUID == null) { rp.MedicalPharmacologicalFormGUID = DropDownList.LookupValues(LookupTables.MedicalPharmacologicalForm).Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }

            return rp;
        }
        [HttpGet]
        public ActionResult ReportsBoard()
        {
            ReportViewer reportViewer = new ReportViewer();
            ViewBag.ReportViewer = reportViewer;
            return View("~/Areas/EMT/Views/Reports/ReportsBoard.cshtml", new EMTReportParametersList());
        }
        [HttpPost]
        public ActionResult ReportsBoard(EMTReportParametersList rp)
        {
            if (rp.StartDate == null) { rp.StartDate = new DateTime(2019, 1, 1); }
            if (rp.EndDate == null) { rp.EndDate = DateTime.Now; }
            rp = FillRP(rp);
            string URL = "LoadUrl('ReportViewer','Reports/ReportsBoardLoad?Report=" + rp.Report +
                "&EndDate=" + rp.EndDate.Value.Ticks +
                "&StartDate=" + rp.StartDate.Value.Ticks +
                "&MedicalBeneficiaryGUID=" + rp.MedicalBeneficiaryGUID +
                "&MedicalPharmacyGUIDs=" + string.Join(",", rp.MedicalPharmacyGUID) +
                "&MedicalItemGUIDs=" + (rp.Sequance!=null? string.Join(",", rp.Sequance):"" ) + "')";
            return Json(DbCMS.SingleUpdateMessage(null, null, null, URL, "Please Wait...."));
        }


        public ActionResult ReportsBoardLoad(int Report, long? EndDate, long? StartDate, string MedicalPharmacyGUIDs, string MedicalItemGUIDs, string MedicalBeneficiaryGUID)
        {
            try
            {
                if (MedicalItemGUIDs == "") { MedicalItemGUIDs = string.Join(",", DropDownList.MedicalItemsSequance().Select(x => x.Value).ToList().ConvertAll(int.Parse).ToArray()); }

                string[] FilesRDLC = new string[] {
            "DistributedGroupByIndividuals",
            "DistributedGroupByItems",
            "Dispatched",
            "DispatchedDetails",
            "PharmacyRemainingItems",
            "PharmacyRemainingItemsMatrix",
            "DispatchedRemainingItemsMatrix",
            "DistributedGroupByItemsPharmacyMatrix",
            "DispatchedWarehouseExpiredMedicine",
            "PharmacyExpiredMedicine",
            "DiscrepancyDetails",
            "DiscrepancyDetails",
            "BeneficiaryDetails",
            //"DispatchedWarehouseExpiredMedicineSixMonths",
            //"PharmacyExpiredMedicineSixMonths",
            "CalaculateDetailClosingBalanceEMT",
            "CalaculateDetailClosingBalanceEMT",
            };
                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.ZoomMode = ZoomMode.Percent;
                reportViewer.ZoomPercent = 100;
                reportViewer.Width = System.Web.UI.WebControls.Unit.Percentage(900);
                reportViewer.Height = System.Web.UI.WebControls.Unit.Pixel(600);

                reportViewer.AsyncRendering = true;
                reportViewer.LocalReport.DataSources.Clear();
                ReportDataSource reportDataSource = new ReportDataSource();
                string ReportName = FilesRDLC[Report];
                if (ReportName.StartsWith("Distributed"))
                {
                    RP_ConsumptionMedicineTableAdapter ReportsBoard = new RP_ConsumptionMedicineTableAdapter();
                    EMTDataSet dataSet = new EMTDataSet();
                    ReportsBoard.Fill(dataSet.RP_ConsumptionMedicine,
                       MedicalPharmacyGUIDs,
                       MedicalItemGUIDs,
                       "",
                       new DateTime(StartDate.Value),
                       new DateTime(EndDate.Value).AddHours(23).AddMinutes(59),
                       LAN);
                    if (MedicalBeneficiaryGUID != "")
                    {
                        reportDataSource = new ReportDataSource(ReportName, dataSet.Tables["RP_ConsumptionMedicine"].Select("MedicalBeneficiaryGUID='" + MedicalBeneficiaryGUID + "'"));
                    }

                    else
                    {
                        reportDataSource = new ReportDataSource(ReportName, dataSet.Tables["RP_ConsumptionMedicine"]);
                    }
                }
                if (ReportName.StartsWith("Dispatched"))
                {
                    RP_DispatchedMedicineTableAdapter ReportsBoard = new RP_DispatchedMedicineTableAdapter();
                    EMTDataSet dataSet = new EMTDataSet();
                    ReportsBoard.Fill(dataSet.RP_DispatchedMedicine,
                       MedicalPharmacyGUIDs,
                       MedicalItemGUIDs,
                       "",
                       new DateTime(StartDate.Value),
                       new DateTime(EndDate.Value),
                       LAN);
                    if (ReportName == "DispatchedWarehouseExpiredMedicine")
                    {
                        reportDataSource = new ReportDataSource(ReportName, dataSet.Tables["RP_DispatchedMedicine"].Select("RemainingItems > 0 and ExpirationDate<='" + DateTime.Now + "'"));

                    }
                    else if (ReportName == "DispatchedWarehouseExpiredMedicineSixMonths")
                    {
                        reportDataSource = new ReportDataSource(ReportName, dataSet.Tables["RP_DispatchedMedicine"].Select("RemainingItems > 0 and ExpirationDate>='" + DateTime.Now + "'" + " and ExpirationDate<='" + DateTime.Now.AddMonths(6) + "'"));
                    }
                    else
                    {
                        reportDataSource = new ReportDataSource(ReportName, dataSet.Tables["RP_DispatchedMedicine"]);

                    }
                }
                if (ReportName.StartsWith("Pharmacy"))
                {
                    RP_TransferMedicineTableAdapter ReportsBoard = new RP_TransferMedicineTableAdapter();
                    EMTDataSet dataSet = new EMTDataSet();
                    ReportsBoard.Fill(dataSet.RP_TransferMedicine,
                       MedicalPharmacyGUIDs,
                       MedicalItemGUIDs,
                       "",
                       new DateTime(StartDate.Value),
                       new DateTime(EndDate.Value),
                       LAN);
                    if (ReportName == "PharmacyExpiredMedicine")
                    {
                        reportDataSource = new ReportDataSource(ReportName, dataSet.Tables["RP_TransferMedicine"].Select("RemainingItems > 0 and ExpirationDate<='" + DateTime.Now + "'"));
                    }
                    else if (ReportName == "PharmacyExpiredMedicineSixMonths")
                    {
                        reportDataSource = new ReportDataSource(ReportName, dataSet.Tables["RP_TransferMedicine"].Select("RemainingItems > 0 and ExpirationDate>='" + DateTime.Now + "'" + "and ExpirationDate<='" + DateTime.Now.AddMonths(6) + "'"));
                    }
                    else
                    {
                        reportDataSource = new ReportDataSource(ReportName, dataSet.Tables["RP_TransferMedicine"]);
                    }
                }
                if (ReportName.StartsWith("Discrepancy"))
                {
                    RP_DiscrepancyMedicineTableAdapter ReportsBoard = new RP_DiscrepancyMedicineTableAdapter();
                    EMTDataSet dataSet = new EMTDataSet();
                    ReportsBoard.Fill(dataSet.RP_DiscrepancyMedicine,
                       MedicalPharmacyGUIDs,
                       MedicalItemGUIDs,
                       "",
                       new DateTime(StartDate.Value),
                       new DateTime(EndDate.Value),
                       LAN);
                    if (ReportName == "DiscrepancyDetails")
                    {
                        reportDataSource = new ReportDataSource(ReportName, dataSet.Tables["RP_DiscrepancyMedicine"]);
                    }
                }
                if (ReportName.StartsWith("Beneficiary"))
                {
                    RP_BeneficiaryDetailsTableAdapter ReportsBoard = new RP_BeneficiaryDetailsTableAdapter();
                    EMTDataSet dataSet = new EMTDataSet();
                    ReportsBoard.Fill(dataSet.RP_BeneficiaryDetails,
                       MedicalPharmacyGUIDs,

                       new DateTime(StartDate.Value),
                       new DateTime(EndDate.Value),
                       LAN);
                    if (ReportName == "BeneficiaryDetails")
                    {
                        if (MedicalBeneficiaryGUID != "")
                        {
                            reportDataSource = new ReportDataSource(ReportName, dataSet.Tables["RP_BeneficiaryDetails"].Select("MedicalBeneficiaryGUID='" + MedicalBeneficiaryGUID + "'"));
                        }

                        else
                        {
                            reportDataSource = new ReportDataSource(ReportName, dataSet.Tables["RP_BeneficiaryDetails"]);
                        }
                    }
                }
                if (ReportName.StartsWith("Beneficiary"))
                {
                    RP_BeneficiaryDetailsTableAdapter ReportsBoard = new RP_BeneficiaryDetailsTableAdapter();
                    EMTDataSet dataSet = new EMTDataSet();
                    ReportsBoard.Fill(dataSet.RP_BeneficiaryDetails,
                       MedicalPharmacyGUIDs,

                       new DateTime(StartDate.Value),
                       new DateTime(EndDate.Value),
                       LAN);
                    if (ReportName == "BeneficiaryDetails")
                    {
                        if (MedicalBeneficiaryGUID != "")
                        {
                            reportDataSource = new ReportDataSource(ReportName, dataSet.Tables["RP_BeneficiaryDetails"].Select("MedicalBeneficiaryGUID='" + MedicalBeneficiaryGUID + "'"));
                        }

                        else
                        {
                            reportDataSource = new ReportDataSource(ReportName, dataSet.Tables["RP_BeneficiaryDetails"]);
                        }
                    }
                }
                if (ReportName.StartsWith("Calaculate"))
                {

                    DbEMT.CalculateClosingBalanceEMT(new DateTime(StartDate.Value).Year,
                      new DateTime(StartDate.Value),
                      new DateTime(EndDate.Value),
                      MedicalPharmacyGUIDs);



                    CalaculateDetailClosingBalanceEMTTableAdapter ReportsBoard = new CalaculateDetailClosingBalanceEMTTableAdapter();
                    EMTDataSet dataSet = new EMTDataSet();
                    ReportsBoard.Fill(dataSet.CalaculateDetailClosingBalanceEMT,
                       new DateTime(StartDate.Value).Year,
                       new DateTime(StartDate.Value),
                       new DateTime(EndDate.Value),
                       MedicalPharmacyGUIDs,
                       MedicalItemGUIDs);
                    if (ReportName == "CalaculateDetailClosingBalanceEMT")
                    {
                        reportDataSource = new ReportDataSource(ReportName, dataSet.Tables["CalaculateDetailClosingBalanceEMT"]);
                    }
                }
                reportViewer.LocalReport.DataSources.Add(reportDataSource);
                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Areas/EMT/Rdlc\" + ReportName + ".rdlc";
                ViewBag.ReportViewer = reportViewer;
                return PartialView("~/Areas/EMT/Views/Reports/_Report.cshtml");
            }
            catch(Exception ex)
            {
                return PartialView("~/Areas/EMT/Views/Reports/_Report.cshtml");
            }
        }
    }
}