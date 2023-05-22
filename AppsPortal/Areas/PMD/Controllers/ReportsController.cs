using System;
using System.Linq;
using System.Web.Mvc;
using AppsPortal.Areas.PMD.Views.RDLC;
using AppsPortal.Areas.PMD.Views.RDLC.PMDDataSetTableAdapters;
using AppsPortal.BaseControllers;
using Microsoft.Reporting.WebForms;
using PMD_DAL.Model;

namespace AppsPortal.Areas.PMD.Controllers
{
    public class ReportsController : PMDBaseController
    {

        #region Reports 

        [HttpGet]
        public ActionResult CRIReport()
        {
            return View("~/Areas/PMD/Views/Report/CRIReport.cshtml");
        }

        [HttpPost]
        public ActionResult ReportsBoardLoad(int Report, DateTime EndDate, DateTime StartDate, string[] OrganizationInstance,string[] Governorate)
        {
            int errorCount = 0;
            string FileType = "";
            string[] FilesRDLC = new string[] {
            "SummaryReportYearly",
            "SummaryReportMonthly",
            "ItemsDistribution",
            "ItemsDispatch",
            "ItemsTransfer",
            "ItemsLostAndDamaged",
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
            PMDDataSet dataSet = new PMDDataSet();
            if (ReportName.StartsWith("SummaryReportYearly"))
            {
                if (OrganizationInstance.Length == 1)
                {
                    DbPMD.CalculateClosingBalance(StartDate.Year, StartDate, EndDate, Guid.Parse(OrganizationInstance[0]));
                    FileType = "Excel";
                    // Distribution Pivot
                    PMD_DistributionPivotTableTableAdapter ReportsBoard1 = new PMD_DistributionPivotTableTableAdapter();
                    ReportsBoard1.Fill(dataSet.PMD_DistributionPivotTable,
                       StartDate,
                       EndDate,
                       Guid.Parse(OrganizationInstance[0]),
                       string.Join(",", Governorate));

                    reportDataSource = new ReportDataSource("PMD_DistributionPivotTable", dataSet.Tables["PMD_DistributionPivotTable"]);
                    reportViewer.LocalReport.DataSources.Add(reportDataSource);

                    //Dispatch Pivot Table
                    PMD_DispatchPivotTableTableAdapter ReportsBoard2 = new PMD_DispatchPivotTableTableAdapter();
                    ReportsBoard2.Fill(dataSet.PMD_DispatchPivotTable,
                       StartDate,
                       EndDate,
                       Guid.Parse(OrganizationInstance[0]),
                       string.Join(",", Governorate));

                    reportDataSource = new ReportDataSource("PMD_DispatchPivotTable", dataSet.Tables["PMD_DispatchPivotTable"]);
                    reportViewer.LocalReport.DataSources.Add(reportDataSource);

                    //Transfer Pivot Table
                    PMD_ItemsTransferPivotTableTableAdapter ReportsBoard3 = new PMD_ItemsTransferPivotTableTableAdapter();
                    ReportsBoard3.Fill(dataSet.PMD_ItemsTransferPivotTable,
                       StartDate,
                       EndDate,
                       Guid.Parse(OrganizationInstance[0]),
                       string.Join(",", Governorate));

                    reportDataSource = new ReportDataSource("PMD_ItemsTransferPivotTable", dataSet.Tables["PMD_ItemsTransferPivotTable"]);
                    reportViewer.LocalReport.DataSources.Add(reportDataSource);

                    //Damaged and lost Pivot Table
                    PMD_DamagedLostDistributionPivotTableTableAdapter ReportsBoard4 = new PMD_DamagedLostDistributionPivotTableTableAdapter();
                    ReportsBoard4.Fill(dataSet.PMD_DamagedLostDistributionPivotTable,
                       StartDate,
                       EndDate,
                       Guid.Parse(OrganizationInstance[0]),
                       string.Join(",", Governorate));

                    reportDataSource = new ReportDataSource("PMD_DamagedLostDistributionPivotTable", dataSet.Tables["PMD_DamagedLostDistributionPivotTable"]);
                    reportViewer.LocalReport.DataSources.Add(reportDataSource);



                    //Closing Balance Pivot Table
                    PMD_ClosingBalancePivotTableTableAdapter ReportsBoard6 = new PMD_ClosingBalancePivotTableTableAdapter();
                    ReportsBoard6.Fill(dataSet.PMD_ClosingBalancePivotTable,
                       StartDate.Year,
                       Guid.Parse(OrganizationInstance[0]),
                       string.Join(",", Governorate));

                    reportDataSource = new ReportDataSource("PMD_ClosingBalancePivotTable", dataSet.Tables["PMD_ClosingBalancePivotTable"]);
                    reportViewer.LocalReport.DataSources.Add(reportDataSource);

                    //Opening Balance Pivot Table
                    PMD_OpeningBalancePivotTableTableAdapter ReportsBoard5 = new PMD_OpeningBalancePivotTableTableAdapter();
                    ReportsBoard5.Fill(dataSet.PMD_OpeningBalancePivotTable,
                       StartDate.Year - 1,
                       Guid.Parse(OrganizationInstance[0]),
                       string.Join(",", Governorate));

                    reportDataSource = new ReportDataSource("PMD_OpeningBalancePivotTable", dataSet.Tables["PMD_OpeningBalancePivotTable"]);
                    reportViewer.LocalReport.DataSources.Add(reportDataSource);

                    //Distribution Details

                    PMD_DistributionDetailsTableAdapter ReportsBoard7 = new PMD_DistributionDetailsTableAdapter();
                    ReportsBoard7.Fill(dataSet.PMD_DistributionDetails,
                       StartDate,
                       EndDate,
                       OrganizationInstance[0],
                       string.Join(",", Governorate));

                    reportDataSource = new ReportDataSource("PMD_DistributionDetails", dataSet.Tables["PMD_DistributionDetails"]);
                    reportViewer.LocalReport.DataSources.Add(reportDataSource);

                    //Dispach Details

                    PMD_DispatchDetailsTableAdapter ReportsBoard8 = new PMD_DispatchDetailsTableAdapter();
                    ReportsBoard8.Fill(dataSet.PMD_DispatchDetails,
                       StartDate,
                       EndDate,
                      OrganizationInstance[0],
                       string.Join(",", Governorate));

                    reportDataSource = new ReportDataSource("PMD_DispatchDetails", dataSet.Tables["PMD_DispatchDetails"]);
                    reportViewer.LocalReport.DataSources.Add(reportDataSource);


                    //lost Details
                    PMD_DamagedLostDistributionDetailsTableAdapter ReportsBoard9 = new PMD_DamagedLostDistributionDetailsTableAdapter();
                    ReportsBoard9.Fill(dataSet.PMD_DamagedLostDistributionDetails,
                       StartDate,
                       EndDate,
                       OrganizationInstance[0],
                       string.Join(",", Governorate));

                    reportDataSource = new ReportDataSource("PMD_DamagedLostDistributionDetails", dataSet.Tables["PMD_DamagedLostDistributionDetails"]);
                    reportViewer.LocalReport.DataSources.Add(reportDataSource);

                    //Transfer Details
                    PMD_ItemsTransferDetailsTableAdapter ReportsBoard10 = new PMD_ItemsTransferDetailsTableAdapter();
                    ReportsBoard10.Fill(dataSet.PMD_ItemsTransferDetails,
                       StartDate,
                       EndDate,
                       OrganizationInstance[0],
                       string.Join(",", Governorate));

                    reportDataSource = new ReportDataSource("PMD_ItemsTransferDetails", dataSet.Tables["PMD_ItemsTransferDetails"]);
                    reportViewer.LocalReport.DataSources.Add(reportDataSource);
                }
                else
                {
                    errorCount++;
                    ModelState.AddModelError("OrganizationInstance", "No more than one Implementing Partner for this report topic");
                }
            }
            if (ReportName.StartsWith("SummaryReportMonthly"))
            {
                if (OrganizationInstance.Length == 1)
                {
                DbPMD.CalculateClosingBalanceMonthly(StartDate.Year,StartDate.Month, StartDate, EndDate, Guid.Parse(OrganizationInstance[0]));
                FileType = "Excel";
                // Distribution Pivot
                PMD_DistributionPivotTableTableAdapter ReportsBoard1 = new PMD_DistributionPivotTableTableAdapter();
                ReportsBoard1.Fill(dataSet.PMD_DistributionPivotTable,
                   StartDate,
                   EndDate,
                   Guid.Parse(OrganizationInstance[0]),
                   string.Join(",", Governorate));

                reportDataSource = new ReportDataSource("PMD_DistributionPivotTable", dataSet.Tables["PMD_DistributionPivotTable"]);
                reportViewer.LocalReport.DataSources.Add(reportDataSource);

                //Dispatch Pivot Table
                PMD_DispatchPivotTableTableAdapter ReportsBoard2 = new PMD_DispatchPivotTableTableAdapter();
                ReportsBoard2.Fill(dataSet.PMD_DispatchPivotTable,
                   StartDate,
                   EndDate,
                   Guid.Parse(OrganizationInstance[0]),
                   string.Join(",", Governorate));

                reportDataSource = new ReportDataSource("PMD_DispatchPivotTable", dataSet.Tables["PMD_DispatchPivotTable"]);
                reportViewer.LocalReport.DataSources.Add(reportDataSource);

                //Transfer Pivot Table
                PMD_ItemsTransferPivotTableTableAdapter ReportsBoard3 = new PMD_ItemsTransferPivotTableTableAdapter();
                ReportsBoard3.Fill(dataSet.PMD_ItemsTransferPivotTable,
                   StartDate,
                   EndDate,
                   Guid.Parse(OrganizationInstance[0]),
                   string.Join(",", Governorate));

                reportDataSource = new ReportDataSource("PMD_ItemsTransferPivotTable", dataSet.Tables["PMD_ItemsTransferPivotTable"]);
                reportViewer.LocalReport.DataSources.Add(reportDataSource);

                //Damaged and lost Pivot Table
                PMD_DamagedLostDistributionPivotTableTableAdapter ReportsBoard4 = new PMD_DamagedLostDistributionPivotTableTableAdapter();
                ReportsBoard4.Fill(dataSet.PMD_DamagedLostDistributionPivotTable,
                   StartDate,
                   EndDate,
                   Guid.Parse(OrganizationInstance[0]),
                   string.Join(",", Governorate));

                reportDataSource = new ReportDataSource("PMD_DamagedLostDistributionPivotTable", dataSet.Tables["PMD_DamagedLostDistributionPivotTable"]);
                reportViewer.LocalReport.DataSources.Add(reportDataSource);



                //Closing Balance Pivot Table
                PMD_ClosingBalanceMonthlyPivotTableTableAdapter ReportsBoard6 = new PMD_ClosingBalanceMonthlyPivotTableTableAdapter();
                ReportsBoard6.Fill(dataSet.PMD_ClosingBalanceMonthlyPivotTable,
                   StartDate.Year,
                   StartDate.Month,
                   Guid.Parse(OrganizationInstance[0]),
                   string.Join(",", Governorate));

                reportDataSource = new ReportDataSource("PMD_ClosingBalancePivotTable", dataSet.Tables["PMD_ClosingBalanceMonthlyPivotTable"]);
                reportViewer.LocalReport.DataSources.Add(reportDataSource);

                //Opening Balance Pivot Table
                PMD_OpeningBalanceMonthlyPivotTableTableAdapter ReportsBoard5 = new PMD_OpeningBalanceMonthlyPivotTableTableAdapter();
                ReportsBoard5.Fill(dataSet.PMD_OpeningBalanceMonthlyPivotTable,
                   (StartDate.Month == 1? StartDate.Year - 1:StartDate.Year),
                    StartDate.Month == 1?12 : StartDate.Month-1,
                   Guid.Parse(OrganizationInstance[0]),
                   string.Join(",", Governorate));

                reportDataSource = new ReportDataSource("PMD_OpeningBalancePivotTable", dataSet.Tables["PMD_OpeningBalanceMonthlyPivotTable"]);
                reportViewer.LocalReport.DataSources.Add(reportDataSource);

                //Distribution Details

                PMD_DistributionDetailsTableAdapter ReportsBoard7 = new PMD_DistributionDetailsTableAdapter();
                ReportsBoard7.Fill(dataSet.PMD_DistributionDetails,
                   StartDate,
                   EndDate,
                   OrganizationInstance[0],
                   string.Join(",", Governorate));

                reportDataSource = new ReportDataSource("PMD_DistributionDetails", dataSet.Tables["PMD_DistributionDetails"]);
                reportViewer.LocalReport.DataSources.Add(reportDataSource);

                //Dispach Details

                PMD_DispatchDetailsTableAdapter ReportsBoard8 = new PMD_DispatchDetailsTableAdapter();
                ReportsBoard8.Fill(dataSet.PMD_DispatchDetails,
                   StartDate,
                   EndDate,
                   OrganizationInstance[0],
                   string.Join(",", Governorate));

                reportDataSource = new ReportDataSource("PMD_DispatchDetails", dataSet.Tables["PMD_DispatchDetails"]);
                reportViewer.LocalReport.DataSources.Add(reportDataSource);


                //lost Details
                PMD_DamagedLostDistributionDetailsTableAdapter ReportsBoard9 = new PMD_DamagedLostDistributionDetailsTableAdapter();
                ReportsBoard9.Fill(dataSet.PMD_DamagedLostDistributionDetails,
                   StartDate,
                   EndDate,
                   OrganizationInstance[0],
                   string.Join(",", Governorate));

                reportDataSource = new ReportDataSource("PMD_DamagedLostDistributionDetails", dataSet.Tables["PMD_DamagedLostDistributionDetails"]);
                reportViewer.LocalReport.DataSources.Add(reportDataSource);

                //Transfer Details
                PMD_ItemsTransferDetailsTableAdapter ReportsBoard10 = new PMD_ItemsTransferDetailsTableAdapter();
                ReportsBoard10.Fill(dataSet.PMD_ItemsTransferDetails,
                   StartDate,
                   EndDate,
                   OrganizationInstance[0],
                   string.Join(",", Governorate));

                reportDataSource = new ReportDataSource("PMD_ItemsTransferDetails", dataSet.Tables["PMD_ItemsTransferDetails"]);
                reportViewer.LocalReport.DataSources.Add(reportDataSource);
                }
                else
                {
                    errorCount++;
                    ModelState.AddModelError("OrganizationInstance", "No more than one Implementing Partner for this report topic");
                }
            }
            if (ReportName.StartsWith("ItemsDistribution"))
            {
                //Distribution Details
                FileType = "Excel";
                PMD_DistributionDetailsTableAdapter ReportsBoard7 = new PMD_DistributionDetailsTableAdapter();
                ReportsBoard7.Fill(dataSet.PMD_DistributionDetails,
                   StartDate,
                   EndDate,
                   string.Join(",", OrganizationInstance) ,
                   string.Join(",", Governorate));

                reportDataSource = new ReportDataSource("PMD_DistributionDetails", dataSet.Tables["PMD_DistributionDetails"]);
                reportViewer.LocalReport.DataSources.Add(reportDataSource);
            }
            if (ReportName.StartsWith("ItemsDispatch"))
            {
                //Dispach Details
                FileType = "Excel";
                PMD_DispatchDetailsTableAdapter ReportsBoard8 = new PMD_DispatchDetailsTableAdapter();
                ReportsBoard8.Fill(dataSet.PMD_DispatchDetails,
                   StartDate,
                   EndDate,
                   string.Join(",", OrganizationInstance),
                   string.Join(",", Governorate));

                reportDataSource = new ReportDataSource("PMD_DispatchDetails", dataSet.Tables["PMD_DispatchDetails"]);
                reportViewer.LocalReport.DataSources.Add(reportDataSource);
            }
            if (ReportName.StartsWith("ItemsTransfer"))
            {
                //Transfer Details
                FileType = "Excel";
                PMD_ItemsTransferDetailsTableAdapter ReportsBoard10 = new PMD_ItemsTransferDetailsTableAdapter();
                ReportsBoard10.Fill(dataSet.PMD_ItemsTransferDetails,
                   StartDate,
                   EndDate,
                   string.Join(",", OrganizationInstance),
                   string.Join(",", Governorate));

                reportDataSource = new ReportDataSource("PMD_ItemsTransferDetails", dataSet.Tables["PMD_ItemsTransferDetails"]);
                reportViewer.LocalReport.DataSources.Add(reportDataSource);
            }
            if (ReportName.StartsWith("ItemsLostAndDamaged"))
            {
                //lost Details
                FileType = "Excel";
                PMD_DamagedLostDistributionDetailsTableAdapter ReportsBoard9 = new PMD_DamagedLostDistributionDetailsTableAdapter();
                ReportsBoard9.Fill(dataSet.PMD_DamagedLostDistributionDetails,
                   StartDate,
                   EndDate,
                   string.Join(",", OrganizationInstance),
                   string.Join(",", Governorate));

                reportDataSource = new ReportDataSource("PMD_DamagedLostDistributionDetails", dataSet.Tables["PMD_DamagedLostDistributionDetails"]);
                reportViewer.LocalReport.DataSources.Add(reportDataSource);

            }
            if (errorCount == 0)
            {
                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Areas/PMD/Views/RDLC\" + ReportName + ".rdlc";
                ReportParameter[] params1 = new ReportParameter[4];
                params1[0] = new ReportParameter("StartDate", StartDate.ToString("dd MMM yyyy"), false);
                params1[1] = new ReportParameter("EndDate", EndDate.ToString("dd MMM yyyy"), false);
                params1[2] = new ReportParameter("Org", string.Join(", ", DbPMD.codeOrganizationsInstancesLanguages.Where(x => OrganizationInstance.Contains(x.OrganizationInstanceGUID.ToString()) && x.LanguageID == LAN).Select(x => x.OrganizationInstanceDescription).ToList()), false);
                params1[3] = new ReportParameter("Gov", string.Join("",Governorate));

                reportViewer.LocalReport.SetParameters(params1);
                if(Report==1 || Report == 2)
                {
                }
                Warning[] warnings;
                string[] streamIds;
                string contentType;
                string encoding;
                string extension;

                //Export the RDLC Report to Byte Array.
                byte[] bytes = reportViewer.LocalReport.Render(FileType, null, out contentType, out encoding, out extension, out streamIds, out warnings);

                this.Response.ClearContent();
                this.Response.ClearHeaders();
                this.Response.Clear();
                this.Response.AddHeader("content-disposition", "attachment; filename= " + ReportName + "." + extension);
                this.Response.ContentType = "Application/" + FileType;
                this.Response.BinaryWrite(bytes);
                this.Response.Flush();
                this.Response.End();
            }
            else
            {
                return View("~/Areas/PMD/Views/Report/CRIReport.cshtml",new PMDReportParametersList()
                {
                    Report=Report,
                    StartDate=StartDate,
                    EndDate=EndDate,
                    Governorate=Governorate,
                    OrganizationInstance=OrganizationInstance
                });
            }
            return null;
        }

        #endregion
    }
}