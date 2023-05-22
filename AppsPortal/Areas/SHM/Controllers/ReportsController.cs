using AppsPortal.Areas.SHM.RDLC;
using AppsPortal.Areas.SHM.RDLC.SHMDataSetTableAdapters;
using AppsPortal.BaseControllers;
using AppsPortal.Library;
using AppsPortal.SHM.ViewModels;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.SHM.Controllers
{
    public class ReportsController : SHMBaseController
    {

        #region Partner Report

        private SHMReportParametersSTR FillRP(SHMReportParametersSTR rp)
        {
            if (rp.DutyStationGUID == "") { rp.DutyStationGUID =string.Join(",", DropDownList.SyriaDutyStations().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray()); }
            if (rp.UserDriverGUID == "") { rp.UserDriverGUID = string.Join(",", DropDownList.ShuttleDrivers().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray()); }
            if (rp.UserPassengerGUID == "") { rp.UserPassengerGUID = string.Join(",", DropDownList.ShuttlePassanger().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray()); }
            if (rp.VehicleGUID == "") { rp.VehicleGUID = string.Join(",", DropDownList.Vehicles().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray()); }
            if (rp.CountryGUID == null) { rp.CountryGUID = DropDownList.CountriesSyriaShuttle().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            Guid LocationType = Guid.Parse("C970DDF5-31E5-47A1-BE76-AF15833D4E6A");
            if (rp.StartLocationGUID == "") { rp.StartLocationGUID = string.Join(",", DropDownList.LocationsByCountries(rp.CountryGUID, LocationType).Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray()); }
            if (rp.CountryGUID1 == null) { rp.CountryGUID1 = DropDownList.CountriesSyriaShuttle().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            if (rp.EndLocationGUID == "") { rp.EndLocationGUID = string.Join(",", DropDownList.LocationsByCountries(rp.CountryGUID1, LocationType).Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray()); }

            return rp;
        }

        [HttpGet]
        public ActionResult Index()
        {
            ReportViewer reportViewer = new ReportViewer();
            ViewBag.ReportViewer = reportViewer;
            return View("~/Areas/SHM/Views/Reports/Index.cshtml", new SHMReportParameters());
        }

        [HttpPost]
        public ActionResult ReportsBoard(SHMReportParameters rp)
        {
            
            if (rp.StartDate == null) { rp.StartDate = new DateTime(2020, 1, 1); }
            if (rp.EndDate == null) { rp.EndDate = DateTime.Now; }
            
            string URL = "LoadUrl('ReportViewer','Reports/ReportsBoardLoad?Report=" + rp.Report +
                "&EndDate=" + rp.EndDate.Value.Ticks +
                "&StartDate=" + rp.StartDate.Value.Ticks +
                "&VehicleGUID=" +(rp.VehicleGUID != null ?  string.Join(",",   rp.VehicleGUID):"" )+
                "&UserDriverGUID=" + (rp.UserDriverGUID != null ? string.Join(",", rp.UserDriverGUID) : "") +
                "&DutyStationGUID=" + (rp.DutyStationGUID != null ? string.Join(",", rp.DutyStationGUID) : "") +
                "&StartLocationGUID=" + (rp.StartLocationGUID != null ? string.Join(",", rp.StartLocationGUID) : "") +
                "&EndLocationGUID=" + (rp.EndLocationGUID != null ? string.Join(",", rp.EndLocationGUID) : "") +
                "&UserPassengerGUID=" + (rp.UserPassengerGUID != null ? string.Join(",", rp.UserPassengerGUID) : "") + "')";
            return Json(DbCMS.SingleUpdateMessage(null, null, null, URL, "Please Wait...."));
        }
        public ActionResult ReportsBoardLoad(int Report, long? EndDate, long? StartDate, string VehicleGUID, string UserDriverGUID, string DutyStationGUID, string StartLocationGUID, string EndLocationGUID, string UserPassengerGUID)
        {
            SHMReportParametersSTR rp = FillRP(new SHMReportParametersSTR() { 
            VehicleGUID=VehicleGUID,
            UserDriverGUID=UserDriverGUID,
            DutyStationGUID=DutyStationGUID,
            StartLocationGUID=StartLocationGUID,
            EndLocationGUID=EndLocationGUID,
            UserPassengerGUID=UserPassengerGUID
            });
            FillRP(rp);
            string[] FilesRDLC = new string[] {
            "ShuttleInfo",
            "ShuttleReport01"
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
            if (ReportName.StartsWith("ShuttleInfo"))
            {
                RP_ShuttleReportTableAdapter ReportsBoard = new RP_ShuttleReportTableAdapter();
                SHMDataSet dataSet = new SHMDataSet();
                ReportsBoard.Fill(dataSet.RP_ShuttleReport,
                     new DateTime(StartDate.Value),
                   new DateTime(EndDate.Value),
                   LAN,
                   rp.DutyStationGUID,
                   rp.UserPassengerGUID,
                   rp.UserDriverGUID,
                   rp.VehicleGUID,
                   rp.StartLocationGUID,
                   rp.EndLocationGUID);
                   reportDataSource = new ReportDataSource(ReportName, dataSet.Tables["RP_ShuttleReport"]);
            }
            if (ReportName.StartsWith("ShuttleReport01"))
            {
                RP_ShuttleReport01TableAdapter ReportsBoard = new RP_ShuttleReport01TableAdapter();
                SHMDataSet dataSet = new SHMDataSet();
                ReportsBoard.Fill(dataSet.RP_ShuttleReport01,
                     new DateTime(StartDate.Value),
                   new DateTime(EndDate.Value),
                   LAN,
                   rp.DutyStationGUID,
                   rp.UserPassengerGUID,
                   rp.UserDriverGUID,
                   rp.VehicleGUID,
                   rp.StartLocationGUID,
                   rp.EndLocationGUID);
                reportDataSource = new ReportDataSource(ReportName, dataSet.Tables["RP_ShuttleReport01"]);
            }
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Areas/SHM/Rdlc\" + ReportName + ".rdlc";
            ViewBag.ReportViewer = reportViewer;
            return PartialView("~/Areas/EMT/Views/Reports/_Report.cshtml");
        }
        #endregion
    }
}