using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using AMS_DAL.Model;
using AppsPortal.AMS.ViewModels;
using AppsPortal.Areas.AMS.Rdlc;
using AppsPortal.Areas.AMS.Rdlc.AMSDataSetTableAdapters;
using AppsPortal.BaseControllers;
using AppsPortal.Library;
using Microsoft.Reporting.WebForms;


namespace AppsPortal.Areas.AMS.Controllers
{
    public class ReportsController : AMSBaseController
    {
        // GET: AMS/Reports

        #region Appointments Slip
        public void AppointmentsSlip(Guid PK)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.ZoomMode = ZoomMode.Percent;
            reportViewer.ZoomPercent = 90;
            reportViewer.Width = System.Web.UI.WebControls.Unit.Pixel(850);
            reportViewer.Height = System.Web.UI.WebControls.Unit.Pixel(600);

            reportViewer.AsyncRendering = true;
            reportViewer.LocalReport.DataSources.Clear();
            var appointment = DbAMS.dataAppointment.Where(x => x.AppointmentGUID == PK).FirstOrDefault();
            RP_AppointmentSlepTableAdapter appointmentsSlipTableAdapter = new RP_AppointmentSlepTableAdapter();
            AMSDataSet amsDataSet = new AMSDataSet();
            appointmentsSlipTableAdapter.Fill(amsDataSet.RP_AppointmentSlep, PK, LAN);
            ReportDataSource reportDataSource = new ReportDataSource("AppointmentsSlip", amsDataSet.Tables["RP_AppointmentSlep"]);
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Areas/AMS/Rdlc/AppointmentsSlip\"+appointment.DutyStationGUID+".rdlc";
           // ViewBag.ReportViewer = reportViewer;
            Warning[] warnings;
            string[] streamIds;
            string contentType;
            string encoding;
            string extension;

            //Export the RDLC Report to Byte Array.
            byte[] bytes = reportViewer.LocalReport.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);

            //Download the RDLC Report in Word, Excel, PDF and Image formats.
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = contentType;
            Response.AppendHeader("Content-Disposition", "attachment; filename=RDLC." + extension);
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
           // return PartialView("~/Areas/AMS/Views/Reports/_AppointmentsSlip.cshtml");

        }
        #endregion

        #region Reports Board

        [HttpGet]
        public ActionResult SchedularReport()
        {
            Guid UNHCR = Guid.Parse("E156C022-EC72-4A5A-BE09-163BD85C68EF");
            return View("~/Areas/AMS/Views/Reports/SchedularReports.cshtml",new ReportParametersList() { OrganizationInstanceGUID=UNHCR});
        }

        [HttpPost]
        public ActionResult ReportsBoard(int Report, DateTime? EndDate, DateTime? StartDate, Guid? DepartmentGUID)
        {
            if (StartDate == null) { StartDate = new DateTime(DateTime.Now.Year, 1, 1); }
            if (EndDate == null) { EndDate = DateTime.Now; }
            string URL = "LoadUrl('ReportViewer','../Reports/ReportsBoardLoad?Report=" + Report +
                "&EndDate=" + EndDate.Value.Ticks +
                "&StartDate=" + StartDate.Value.Ticks +
                "&DepartmentGUID=" + DepartmentGUID  + "')";
            return Json(DbCMS.SingleUpdateMessage(null, null, null, URL, "Please Wait...."));
        }

        public ActionResult ReportsBoardLoad(int Report, long? EndDate, long? StartDate, string DepartmentGUID)
        {
            string FileType = "";
            string[] FilesRDLC = new string[] {
            "AppointmentByDate",
            "AppointmentByExecutionTime",
            "AppointmentOverview",
            "AppointmentSlots",
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
            Guid DutyStationGUID = DbAMS.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault().DutyStationGUID;

            if (ReportName.StartsWith("AppointmentByDate"))
            {
                FileType = "Excel";
                RP_AppointmentByDateTableAdapter AppointmentByDate = new RP_AppointmentByDateTableAdapter();
                AMSDataSet amsDataSet = new AMSDataSet();
                AppointmentByDate.Fill(amsDataSet.RP_AppointmentByDate,
                    Guid.Parse( DepartmentGUID),
                    new DateTime( StartDate.Value),
                    new DateTime(EndDate.Value)
                    , LAN,
                    DutyStationGUID);
                 reportDataSource = new ReportDataSource("AppointmentByDate", amsDataSet.Tables["RP_AppointmentByDate"]);
            }
            if (ReportName.StartsWith("AppointmentSlots"))
            {
                FileType = "Excel";
                RP_AppointmentSlotsTableAdapter AppointmentSlots = new RP_AppointmentSlotsTableAdapter();
                AMSDataSet amsDataSet = new AMSDataSet();
                AppointmentSlots.Fill(amsDataSet.RP_AppointmentSlots,
                    Guid.Parse(DepartmentGUID),
                    DutyStationGUID,
                   new DateTime(StartDate.Value),
                    new DateTime(EndDate.Value)
                    , LAN);
                reportDataSource = new ReportDataSource("AppointmentSlots", amsDataSet.Tables["RP_AppointmentSlots"]);
            }
            if (ReportName.StartsWith("AppointmentOverview"))
            {
                FileType = "Excel";
                RP_AppointmentOverviewTableAdapter AppointmentOverview = new RP_AppointmentOverviewTableAdapter();
                AMSDataSet amsDataSet = new AMSDataSet();
                AppointmentOverview.Fill(amsDataSet.RP_AppointmentOverview,
                   new DateTime(StartDate.Value),
                    new DateTime(EndDate.Value)
                    ,LAN
                    ,DutyStationGUID);
                reportDataSource = new ReportDataSource("AppointmentOverview", amsDataSet.Tables["RP_AppointmentOverview"]);
            }
            if (ReportName.StartsWith("AppointmentByExecutionTime"))
            {
                FileType = "Excel";
                RP_AppointmentByExecutionTimeTableAdapter AppointmentByExecutionTime = new RP_AppointmentByExecutionTimeTableAdapter();

                AMSDataSet amsDataSet = new AMSDataSet();
                AppointmentByExecutionTime.Fill(amsDataSet.RP_AppointmentByExecutionTime,
                    Guid.Parse(DepartmentGUID),

                    new DateTime(StartDate.Value),
                    new DateTime(EndDate.Value)
                    ,LAN
                    ,DutyStationGUID);
                reportDataSource = new ReportDataSource("AppointmentByExecutionTime", amsDataSet.Tables["RP_AppointmentByExecutionTime"]);
            }
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Areas/AMS/Rdlc\" + ReportName + ".rdlc";

            Warning[] warnings;
            string[] streamIds;
            string contentType;
            string encoding;
            string extension;


            //Export the RDLC Report to Byte Array.
            byte[] bytes = reportViewer.LocalReport.Render(FileType, null, out contentType, out encoding, out extension, out streamIds, out warnings);


            //this.Response.ClearContent();
            //this.Response.ClearHeaders();
            //this.Response.Clear();
            //this.Response.AddHeader("content-disposition", "attachment; filename= " + ReportName + "." + extension);
            //this.Response.ContentType = "Application/" + FileType;
            //this.Response.BinaryWrite(bytes);
            //this.Response.Flush();
            //this.Response.End();
            ViewBag.ReportViewer = reportViewer;
            return PartialView("~/Areas/DAS/Views/Reports/_Report.cshtml");
        }

        #endregion

       

      

   

      


    }
}