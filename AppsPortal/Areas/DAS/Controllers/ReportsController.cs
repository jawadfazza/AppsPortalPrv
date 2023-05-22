using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Services;
using AppsPortal.ViewModels;
using AutoMapper;
using DAS_DAL.Model;
using DAS_DAL.ViewModels;
using iTextSharp.text;
using RES_Repo.Globalization;
using iTextSharp.text.pdf;
using LinqKit;
using static AppsPortal.Library.DataTableNames;
using FineUploader;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Image = iTextSharp.text.Image;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Reporting.WebForms;
using AppsPortal.Areas.DAS.RDLC.DASDataSetTableAdapters;
using AppsPortal.Areas.DAS.RDLC;

namespace AppsPortal.Areas.DAS.Controllers
{
    public class ReportsController : DASBaseController
    {
        // GET: DAS/Reports
        public ActionResult Index()
        {
            return View();
        }
        [Route("DAS/OverviewReportIndex/")]
        public ActionResult ReportsIndex()
        {
            if (!CMS.HasAction(Permissions.DASDashboard.Access, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/DAS/Views/Reports/OverviewReportIndex.cshtml");
        }
        public JsonResult InitReportIndex()
        {
            var dataReportTemplates = DbDAS.dataReportTemplate.Select(x => new
            {
                Name = x.Name,
                ReportTemplateId = x.ReportTemplateId,
                x.ReportId
            }).OrderBy(x => x.ReportId).ToList();
            return Json(new { dataReportTemplates = dataReportTemplates }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        //[SessionExpireFilterAttribute.SessionExpireAttribute]
        public ActionResult ShowExternalPivotResultData(int ReportTemplateId)
        {

            int Start = 0;
            int Length = 3000000;
            string QueryStatment = "";
            var columns = new List<string>();
            //DataSystem dataSystem =  DbDAS.DataSystems.Find(systemId);
            List<string[]> result = new List<string[]>();
            var system =  DbDAS.dataReportTemplateConfiguration.Where(x => x.Name == "PortalDB").FirstOrDefault();
            var conncetion = system.ConnectionString;

            long resultCount = 0;

            try
            {
                var Report =  DbDAS.dataReportTemplate.Where(x => x.ReportTemplateId == ReportTemplateId).FirstOrDefault();
                QueryStatment = Report.ReportQuery;


                SqlConnection con = new SqlConnection(conncetion);
                con.Open();
                if (con.State == ConnectionState.Open)
                {
                    //Get Result Count
                    SqlCommand countCmd = new SqlCommand
                    {
                        CommandText = "SELECT COUNT_BIG(*) FROM (" + QueryStatment + ")  as t",
                        CommandType = CommandType.Text,
                        Connection = con
                    };
                    SqlDataReader readerCount = countCmd.ExecuteReader();
                    if (readerCount.HasRows)
                        if (readerCount.Read())
                        {
                            resultCount = readerCount.GetInt64(0);
                        }
                    con.Close();

                    //Get Result Page
                    con.Open();
                    SqlCommand cmd = new SqlCommand
                    {
                        CommandText = QueryStatment,
                        CommandType = CommandType.Text,
                        Connection = con
                    };
                    SqlDataReader reader = cmd.ExecuteReader();

                    int CountFields = reader.FieldCount;

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        columns.Add(reader.GetName(i));
                    }

                    var fieldsNames = columns.ToArray();
                    result.Add(fieldsNames);

                    int readerIndex = 0;
                    if (reader.HasRows)
                        while (reader.Read())
                        {
                            readerIndex++;
                            if (readerIndex <= Start)
                                continue;
                            string[] temp = new string[CountFields];
                            for (int i = 0; i < CountFields; i++)
                            {
                                temp[i] = reader[i].ToString();
                            }
                            result.Add(temp);
                            if (result.Count() == Length)
                                break;
                        }

                    ///////////////////////////////////////////////////////////////////
                    con.Close();
                }

            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message.ToString());
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        #region Reports
        [HttpGet]
        [Route("DAS/GeneralReports/")]
        public ActionResult FileTrackingReport()
        {
            ReportViewer reportViewer = new ReportViewer();
            ViewBag.ReportViewer = reportViewer;
            FileTrackingReportModel model = new FileTrackingReportModel();
            return View("~/Areas/DAS/Views/Reports/GeneralReports.cshtml", model);
        }

        [HttpPost]
        public ActionResult ModelTrackFileReport(FileTrackingReportModel reportParameters)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Update, Apps.DAS))
            {
                return Json(DbDAS.PermissionError());
            }
            Guid orgGuid = (Guid)Session[SessionKeys.OrganizationInstanceGUID];
            DateTime today = DateTime.Now;
            var myQuery = DbDAS.v_TrackTransferFilesMovement.AsQueryable();
            //List<Guid> dateFilterEdGUID = new List<Guid>();
            if (reportParameters.LastFlowStatusGUID != null)
            {
                myQuery = myQuery.Where(x =>
                 x.DocumentFlowStatusGUID == reportParameters.LastFlowStatusGUID);
            }
            if (reportParameters.StaffGUID != null)
            {
                myQuery = myQuery.Where(x => x.RequesterGUID == reportParameters.StaffGUID
                     );
            }

            if (reportParameters.TransferFromDate != null)
            {
                myQuery = myQuery.Where(x => x.TransferDate >= reportParameters.TransferFromDate);
            }
            if (reportParameters.TransferToDate != null)
            {
                myQuery = myQuery.Where(x => x.TransferDate <= reportParameters.TransferToDate);
            }






            string sourceFile = Server.MapPath("~/Areas/DAS/Templates/FileTrackingTransferDetail.xlsx");
            string DisFolder =
                Server.MapPath("~/Areas/DAS/temp/FileTrackingTransferDetail" + DateTime.Now.ToBinary() + ".xlsx");

            //var _staffs = DbDAS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active).ToList();
            //var _fileStatus = DbDAS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active && x.codeTablesValues.TableGUID == LookupTables.DASFileMovementStatus).ToList();
            System.IO.File.Copy(sourceFile, DisFolder);
            using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
            {
                var cx = package.Workbook.Worksheets.ToList();
                ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                DataTable dt = new DataTable();

                dt.Columns.Add("Case Number", typeof(string));
                dt.Columns.Add("Transfered To", typeof(string));
                dt.Columns.Add("Transfered By ", typeof(string));
                dt.Columns.Add("Transfer Date ", typeof(DateTime));
                dt.Columns.Add("Status ", typeof(string));


                var result = myQuery.ToList();


                foreach (var item in result)
                {



                    //if (item.RequesterGUID == item.CreateByGUID || _status==null)
                    //{
                    //    continue;
                    //}
                    DataRow dr;
                    dr = dt.NewRow();
                    dr[0] = item.FileNumber;
                    dr[1] = item.CustodianName;
                    dr[2] = item.TransferdBy;
                    dr[3] = item.TransferDate;
                    dr[4] = item.MovementStatus;


                    dt.Rows.Add(dr);
                }
                workSheet.Cells["B5"].LoadFromDataTable(dt, true);
                workSheet.Cells["B1"].Value = "FileTracking-Detail- reprot:";
                //workSheet.Cells["B2"].Value = items;
                package.Save();
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

            string fileName = DateTime.Now.ToString("yyMMdd") + "_FileTracking-Detail reprot" + ".xlsx";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

            // return Json(DbCMS.SingleUpdateMessage(null, null, null, null, "Please Wait...."));
        }
        #endregion

        #region Reports Staff

        [HttpGet]
        public ActionResult ArchiveReport()
        {
            return View("~/Areas/DAS/Views/Reports/ArchiveReports.cshtml");
        }

        public void ReportsBoardLoad(int Report, DateTime EndDate, DateTime StartDate)
        {
            string FileType = "";
            string[] FilesRDLC = new string[] {
            "DocumentsUpdatesSummary",
            "DocumentsUpdatesDetail",
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
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.RefugeeScannedDocument.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            string permissions = string.Join(",", AuthorizedList).Replace(Session[SessionKeys.OrganizationInstanceGUID] + ",", "");
            if (ReportName.StartsWith("DocumentsUpdatesSummary"))
            {
                FileType = "PDF";
                DAS_DocumentsUpdatesSummaryTableAdapter ReportsBoard = new DAS_DocumentsUpdatesSummaryTableAdapter();
                DASDataSet dataSet = new DASDataSet();
                ReportsBoard.Fill(dataSet.DAS_DocumentsUpdatesSummary,
                   StartDate,
                   EndDate.AddHours(23).AddMinutes(59),
                   permissions);
                    reportDataSource = new ReportDataSource(ReportName, dataSet.Tables["DAS_DocumentsUpdatesSummary"]);
            }
            if (ReportName.StartsWith("DocumentsUpdatesDetail"))
            {
                FileType = "Excel";
                DAS_DocumentsUpdatesDetailTableAdapter ReportsBoard = new DAS_DocumentsUpdatesDetailTableAdapter();
                DASDataSet dataSet = new DASDataSet();
                ReportsBoard.Fill(dataSet.DAS_DocumentsUpdatesDetail,
                   StartDate,
                   EndDate.AddHours(23).AddMinutes(59),
                    permissions);

                reportDataSource = new ReportDataSource(ReportName, dataSet.Tables["DAS_DocumentsUpdatesDetail"]);
            }
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Areas/DAS/Rdlc\" + ReportName + ".rdlc";

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
            this.Response.ContentType = "Application/"+ FileType;
            this.Response.BinaryWrite(bytes);
            this.Response.Flush();
            this.Response.End();
            //ViewBag.ReportViewer = reportViewer;
            //return PartialView("~/Areas/DAS/Views/Reports/_Report.cshtml");
        }

        #endregion
    }
}