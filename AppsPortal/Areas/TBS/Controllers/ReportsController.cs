using AppsPortal.Areas.TBS.Rdlc;
using AppsPortal.Areas.TBS.Rdlc.TBSDataSetTableAdapters;
using AppsPortal.BaseControllers;
using AppsPortal.Data;
using AppsPortal.Models;
using AppsPortal.ViewModels;
using Microsoft.Reporting.WebForms;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using TBS_DAL.Model;

namespace AppsPortal.Areas.TBS.Controllers
{
    public class ReportsController : TBSBaseController
    {
        // GET: TBS/Reports
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult FinanceReport()
        {

            return View("~/Areas/TBS/Views/Reports/FinanceReport.cshtml", new FinanceReportParameterModel());
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult GetFinanceReportData(FinanceReportParameterModel model)
        {
            //query splitted for timeout issue
            var data = (from a in DbTBS.v_FinanceReport
                        where a.BillForTypeGUID == model.ReportTypeGUID.Value
                          //&& a.SIMWarehouseGUID == model.BillingDutyStationGUID.Value
                          && a.BillForMonth == model.BillForMonth.Value
                          && a.BillForYear == model.BillForYear.Value
                          && (model.TelecomCompanyGUID.HasValue ? a.TelecomCompanyGUID == model.TelecomCompanyGUID.Value : true)
                          && (model.UserGUID.HasValue ? a.UserGUID == model.UserGUID.Value : true)
                        select a).ToList();
            var result = (from a in data
                          group a by new { a.UserGUID, a.StaffName, a.CallingNumber, a.DeductFromSalaryAmount, a.PayInCashAmount, a.SIMWarehouseDescription } into grp
                          select new FinanceReportModel
                          {
                              UserGUID = grp.Key.UserGUID,
                              StaffName = grp.Key.StaffName,
                              SIMWarehouseDescription = grp.Key.SIMWarehouseDescription,
                              CallingNumber = grp.Key.CallingNumber,
                              DeductFromSalaryAmount = grp.Key.DeductFromSalaryAmount.Value,
                              PayInCashAmount = grp.Key.PayInCashAmount.Value,
                              RecordDetails = grp
                          }).ToList();

            return PartialView("~/Areas/TBS/Views/Reports/_FinanceReport.cshtml", result);
        }


        public ActionResult DownloadFinanceReportExcel(FinanceReportParameterModel model)
        {


            string sourceFile = Server.MapPath("~/Areas/TBS/ReportTemplate/TBS_Finance_Report.xlsx");

            string receiptFileName = "TBS_Finance_Report_" + DateTime.Now.ToBinary() + ".xlsx";

            string DisFolder = Server.MapPath("~/Areas/TBS/GeneratedReports/" + receiptFileName);

            System.IO.File.Copy(sourceFile, DisFolder);


            //query splitted for timeout issue
            var data = (from a in DbTBS.v_FinanceReport
                        where a.BillForTypeGUID == model.ReportTypeGUID.Value
                          //&& a.SIMWarehouseGUID == model.BillingDutyStationGUID.Value
                          && a.BillForMonth == model.BillForMonth.Value
                          && a.BillForYear == model.BillForYear.Value
                          && (model.TelecomCompanyGUID.HasValue ? a.TelecomCompanyGUID == model.TelecomCompanyGUID.Value : true)
                          && (model.UserGUID.HasValue ? a.UserGUID == model.UserGUID.Value : true)
                        select a).ToList();

            var result = (from a in data
                          group a by new { a.UserGUID, a.UserBillGUID, a.StaffName, a.CallingNumber, a.DeductFromSalaryAmount, a.PayInCashAmount, a.SIMWarehouseDescription, a.TelecomCompanyDescription } into grp
                          select new FinanceReportModel
                          {
                              UserGUID = grp.Key.UserGUID,
                              UserBillGUID = grp.Key.UserBillGUID,
                              StaffName = grp.Key.StaffName,
                              SIMWarehouseDescription = grp.Key.SIMWarehouseDescription,
                              CallingNumber = grp.Key.CallingNumber,
                              DeductFromSalaryAmount = grp.Key.DeductFromSalaryAmount.Value,
                              PayInCashAmount = grp.Key.PayInCashAmount.Value,
                              RecordDetails = grp,
                              CallCost = grp.Select(x => x.CallCost).Sum(),
                              PrivateCallCost = grp.Where(x => x.IsPrivate).Select(x => x.CallCost).Sum(),
                              OfficialCallCost = grp.Where(x => x.IsPrivate == false).Select(x => x.CallCost).Sum(),
                              TelecomCompanyDescription = grp.Key.TelecomCompanyDescription
                          }).ToList();

            string BillMonthName = new System.Globalization.DateTimeFormatInfo().GetMonthName(model.BillForMonth.Value);
            string MonthYear = BillMonthName + " " + model.BillForYear.Value;

            using (var package = new ExcelPackage(new FileInfo(DisFolder)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                worksheet.Cells["B2"].Value = "Totals Summary Of 3G / 2G SIM  Between [1 " + MonthYear + " - " + MonthYear + "]";

                int i = 6;
                foreach (var record in result)
                {
                    worksheet.Cells["B" + i].Value = record.SIMWarehouseDescription; //Duty Name
                    worksheet.Cells["C" + i].Value = record.CallingNumber.ToString(); //Office Number
                    worksheet.Cells["D" + i].Value = record.StaffName.ToString(); //Staff Name
                    worksheet.Cells["E" + i].Value = record.TelecomCompanyDescription; //Company
                    worksheet.Cells["F" + i].Value = "2G"; //Type
                    worksheet.Cells["G" + i].Value = record.CallCost; //Cost Value
                    worksheet.Cells["H" + i].Value = record.PrivateCallCost; //Private
                    worksheet.Cells["I" + i].Value = record.OfficialCallCost; //Offical
                    worksheet.Cells["J" + i].Value = ""; //Table deducted
                    worksheet.Cells["L" + i].Value = record.PayInCashAmount; //PayInCashAmount


                    dataUserBillDetail automated = (from a in DbTBS.dataUserBillDetail where a.UserBillGUID == record.UserBillGUID where a.AutomatedBySystem == true select a).FirstOrDefault();
                    if (automated != null)
                    {
                        worksheet.Cells["K" + i].Value = "No Reply";
                        worksheet.Cells["H" + i].Value = 0; //Private
                        worksheet.Cells["I" + i].Value = 0; //Offical
                        worksheet.Cells["L" + i].Value = 0; //PayInCashAmount
                    }
                    else if (record.PrivateCallCost == record.CallCost)
                    {
                        worksheet.Cells["K" + i].Value = "All Calls Private";
                    }
                    else if (record.OfficialCallCost == record.CallCost)
                    {
                        worksheet.Cells["K" + i].Value = "All Calls Official";
                    }

                    else
                    {
                        worksheet.Cells["K" + i].Value = "Reply With private";
                    }


                    i++;
                }
                worksheet.Cells["G" + i].Formula = "=SUM(G6:G" + (i - 1).ToString() + ")";  //Table deducted
                worksheet.Cells["H" + i].Formula = "=SUM(H6:H" + (i - 1).ToString() + ")";    //Table deducted
                worksheet.Cells["I" + i].Formula = "=SUM(I6:I" + (i - 1).ToString() + ")";    //Table deducted


                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                worksheet.Column(6).AutoFit();
                worksheet.Column(7).AutoFit();
                worksheet.Column(8).AutoFit();
                worksheet.Column(9).AutoFit();
                worksheet.Column(10).AutoFit();
                worksheet.Column(11).AutoFit();
                worksheet.Column(12).AutoFit();

                ////create a range for the table
                //ExcelRange range = worksheet.Cells[5, 2, i-1, 11];

                ////add a table to the range
                //ExcelTable tab = worksheet.Tables.Add(range, "Table1");

                ////format the table
                //tab.TableStyle = TableStyles.Medium2;

                package.Save();
            }
            byte[] fileBytes = System.IO.File.ReadAllBytes(@"" + DisFolder + "");
            return File(fileBytes, "application/octet-stream", "Finance Report.xlsx");
        }

        public ActionResult TestReport()
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.ZoomMode = ZoomMode.PageWidth;
            reportViewer.Width = System.Web.UI.WebControls.Unit.Pixel(960);
            reportViewer.Height = System.Web.UI.WebControls.Unit.Pixel(500);
            reportViewer.AsyncRendering = true;
            reportViewer.LocalReport.DataSources.Clear();
            TBSDataSet tBSDataSet = new TBSDataSet();
            codeTelecomCompanyTableAdapter ta = new codeTelecomCompanyTableAdapter();
            ta.Fill(tBSDataSet.codeTelecomCompany);
            //var result = (from a in DbTBS.codeTelecomCompany select new { a.TelecomCompanyGUID, a.TelecomCompanyAcronym, a.Active }).ToList();
            //ReportDataSource reportDataSource = new ReportDataSource("TestDataSet", result);
            ReportDataSource reportDataSource = new ReportDataSource("TestDataSet", tBSDataSet.Tables[0]);
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Areas/TBS/Rdlc\Report1.rdlc";
            reportViewer.LocalReport.SetParameters(new ReportParameter[]
            {
                 new ReportParameter("TestParam", "Local Report Example"),
                 new ReportParameter("TestParam2", "Amer Karkoush")
            });
            ViewBag.ReportViewer = reportViewer;
            return View();

        }

        private ReportParameter[] GetParametersLocal()
        {
            ReportParameter p1 = new ReportParameter("ReportTitle", "Local Report Example");
            return new ReportParameter[] { p1 };
        }

        public ActionResult HeadOfUnitReport()
        {
            return View();
        }

        public ActionResult AdminSectionReport()
        {
            return View();
        }

        public ActionResult UnknownNumbersReport()
        {
            return View();
        }

        public ActionResult ComparisonReport()
        {
            return View();
        }
    }
}