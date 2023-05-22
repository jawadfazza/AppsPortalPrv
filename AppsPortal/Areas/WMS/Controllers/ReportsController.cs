using AppsPortal.Areas.WMS.RDLC.DataSet1TableAdapters;
using AppsPortal.BaseControllers;
using AppsPortal.Library;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Reporting.WebForms;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WMS_DAL.ViewModels;

namespace AppsPortal.Areas.WMS.Controllers
{
    public class ReportsController : WMSBaseController
    {
        public ActionResult Home()
        {
            return View();
        }
        #region Damageed Report
        public ActionResult PrintDamagedReportPDF(Guid id)
        {

            ReportViewer reportViewer = new ReportViewer();
            LocalReport localReport = new LocalReport();
            //localReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Reports/VoucherReport.rdlc";
            //localReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Areas/AHD/Rdlc/VehicalForms\VehicleMaintenanceRequest.rdlc";
            localReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Areas/WMS/RdlC/ITEquipmentDamageReport.rdlc";

            v_ITEquipmentDamageReportTableAdapter result = new v_ITEquipmentDamageReportTableAdapter();
            var results = result.GetData().Where(c => c.ItemOutputDetailDamagedTrackGUID == id).ToList();

            results = results.ToList();

            if (results == null)
            {
                return PartialView("_Empty");
            }

            DataTable dt = results.ToList().CopyToDataTable();



            localReport.DataSources.Add(new ReportDataSource("DataSet1", dt));


            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension = "pdf";
            //The DeviceInfo settings should be changed based on the reportType 
            string deviceInfo = @"<DeviceInfo>              
                                         <OutputFormat>PDF</OutputFormat>              
                                         <PageWidth>21cm</PageWidth>              
                                         <PageHeight>29.7cm</PageHeight>          
                                         <MarginTop>0cm</MarginTop>          
                                         <MarginLeft>0cm</MarginLeft>            
                                         <MarginRight>0cm</MarginRight>       
                                         <MarginBottom>0cm</MarginBottom></DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;


            renderedBytes = localReport.Render(
                reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension,
                out streams, out warnings);

            var doc = new Document();
            doc.SetPageSize(iTextSharp.text.PageSize.A4);

            var reader = new PdfReader(renderedBytes);
            using (FileStream fs =
                new FileStream(
                    Server.MapPath("~/Uploads/AHD/Temp/Report_" + Convert.ToString(Session[SessionKeys.UserGUID]) +
                                   ".pdf"), FileMode.Create))
            {
                PdfStamper stamper = new PdfStamper(reader, fs);

                stamper.JavaScript =
                    "var pp = getPrintParams();pp.interactive = pp.constants.interactionLevel.automatic;pp.printerName = getPrintParams().printerName;print(pp);\r";
                stamper.Close();
            }

            reader.Close();
            FileStream fss =
                new FileStream(
                    Server.MapPath("~/Uploads/AHD/Temp/Report_" + Convert.ToString(Session[SessionKeys.UserGUID]) +
                                   ".pdf"), FileMode.Open);
            byte[] bytes = new byte[fss.Length];
            fss.Read(bytes, 0, Convert.ToInt32(fss.Length));
            fss.Close();
            System.IO.File.Delete(
                Server.MapPath("~/Uploads/AHD/Temp/Report_" + Convert.ToString(Session[SessionKeys.UserGUID]) +
                               ".pdf"));

            //Here we returns the file result for view(PDF)

            return File(bytes, "application/pdf");

        }
        #endregion
        #region Print Issue Return Item
        public ActionResult PrintIssueReturnItemPDF(Guid PK)
        {

            ReportViewer reportViewer = new ReportViewer();
            LocalReport localReport = new LocalReport();
            //localReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Reports/VoucherReport.rdlc";
            //localReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Areas/AHD/Rdlc/VehicalForms\VehicleMaintenanceRequest.rdlc";
            localReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Areas/WMS/RdlC/ItemIssueReturn.rdlc";

            v_ItemOutputFlowDetailReportTableAdapter result = new v_ItemOutputFlowDetailReportTableAdapter();
            var results = result.GetData().Where(c => c.ItemOutputDetailFlowGUID == PK).ToList();

            results = results.ToList();

            if (results == null)
            {
                return PartialView("_Empty");
            }

            DataTable dt = results.ToList().CopyToDataTable();



            localReport.DataSources.Add(new ReportDataSource("DataSet1", dt));


            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension = "pdf";
            //The DeviceInfo settings should be changed based on the reportType 
            string deviceInfo = @"<DeviceInfo>              
                                         <OutputFormat>PDF</OutputFormat>              
                                         <PageWidth>21cm</PageWidth>              
                                         <PageHeight>29.7cm</PageHeight>          
                                         <MarginTop>0cm</MarginTop>          
                                         <MarginLeft>0cm</MarginLeft>            
                                         <MarginRight>0cm</MarginRight>       
                                         <MarginBottom>0cm</MarginBottom></DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;


            renderedBytes = localReport.Render(
                reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension,
                out streams, out warnings);

            var doc = new Document();
            doc.SetPageSize(iTextSharp.text.PageSize.A4);

            var reader = new PdfReader(renderedBytes);
            using (FileStream fs =
                new FileStream(
                    Server.MapPath("~/Uploads/AHD/Temp/Report_" + Convert.ToString(Session[SessionKeys.UserGUID]) +
                                   ".pdf"), FileMode.Create))
            {
                PdfStamper stamper = new PdfStamper(reader, fs);

                stamper.JavaScript =
                    "var pp = getPrintParams();pp.interactive = pp.constants.interactionLevel.automatic;pp.printerName = getPrintParams().printerName;print(pp);\r";
                stamper.Close();
            }

            reader.Close();
            FileStream fss =
                new FileStream(
                    Server.MapPath("~/Uploads/AHD/Temp/Report_" + Convert.ToString(Session[SessionKeys.UserGUID]) +
                                   ".pdf"), FileMode.Open);
            byte[] bytes = new byte[fss.Length];
            fss.Read(bytes, 0, Convert.ToInt32(fss.Length));
            fss.Close();
            System.IO.File.Delete(
                Server.MapPath("~/Uploads/AHD/Temp/Report_" + Convert.ToString(Session[SessionKeys.UserGUID]) +
                               ".pdf"));

            //Here we returns the file result for view(PDF)

            return File(bytes, "application/pdf");

        }
        #endregion

        #region Init Calander

        public JsonResult InitCalander()
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            Guid orgGuid = (Guid)Session[SessionKeys.OrganizationInstanceGUID];
            List<CalanderVM> result = new List<CalanderVM>();
            var flows = DbWMS.v_trackItemOutputFlow.Where(x => x.IsLastAction == true && x.OrganizationInstanceGUID == orgGuid).ToList();
            var flowsDates = flows.ToList().Select(x => x.FlowCreatedDate.ToShortDateString()).Distinct();
            Guid syriaCountryGUID = Guid.Parse("710BFD1B-50CC-4F1E-92A9-A70583CFA5E0");
            var warehouses = DbWMS.codeWarehouseLocationLanguage.Where(x =>
                  x.LanguageID == LAN && x.Active).ToList();
            string format = "yyyy-MM-dd";
            foreach (var item in flowsDates)
            {
                DateTime time = Convert.ToDateTime(item);             // Use current time.
                // Use this format.
                var myDate = time.ToString(format);
                string setdate = time.ToString("dd/MM/yyyy");
                foreach (var myWarehoue in warehouses)
                {
                    var myFlows = flows.Where(x => x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID).ToList();
                    if (myWarehoue.WarehouseLocationDescription == "Damascus")
                    {




                        if (myFlows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                                    && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                .Select(x => x.ItemOutputDetailFlowGUID).Count() > 0)
                        {



                            CalanderVM myResult = new CalanderVM
                            {
                                id = myWarehoue.WarehouseLocationGUID.ToString() + "x" + setdate,
                                title = "Damascus: " + " " + myFlows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                                                                && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                            .Select(x => x.ItemOutputDetailFlowGUID).Count(),
                                start = myDate.ToString(),
                                end = null,
                                d = time.Day,
                                m = time.Month,
                                y = time.Year,
                                backgroundColor = "#f44271",
                                borderColor = "#00c0ef"
                            };
                            result.Add(myResult);

                        }

                    }

                    if (myWarehoue.WarehouseLocationDescription == "Aleppo")
                    {
                        if (myFlows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                               && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                .Select(x => x.ItemOutputDetailFlowGUID).Count() > 0)
                        {



                            CalanderVM myResult = new CalanderVM
                            {
                                id = myWarehoue.WarehouseLocationGUID.ToString() + "x" + setdate,
                                title = "Aleppo: " + " " + flows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                                                            && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                            .Select(x => x.ItemOutputDetailFlowGUID).Count(),
                                start = myDate.ToString(),
                                end = null,
                                d = time.Day,
                                m = time.Month,
                                y = time.Year,
                                backgroundColor = "#DEBDC3",
                                borderColor = "#00c0ef"
                            };
                            result.Add(myResult);

                        }

                    }

                    if (myWarehoue.WarehouseLocationDescription == "Homs")
                    {
                        if (myFlows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                               && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                .Select(x => x.ItemOutputDetailFlowGUID).Count() > 0)
                        {



                            CalanderVM myResult = new CalanderVM
                            {
                                id = myWarehoue.WarehouseLocationGUID.ToString() + "x" + setdate,
                                title = "Homs: " + " " + flows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                                                          && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                            .Select(x => x.ItemOutputDetailFlowGUID).Count(),
                                start = myDate.ToString(),
                                end = null,
                                d = time.Day,
                                m = time.Month,
                                y = time.Year,
                                backgroundColor = "#3c8dbc",
                                borderColor = "#00c0ef"
                            };
                            result.Add(myResult);

                        }

                    }

                    if (myWarehoue.WarehouseLocationDescription == "Tartous")
                    {
                        if (myFlows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                               && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                .Select(x => x.ItemOutputDetailFlowGUID).Count() > 0)
                        {


                            CalanderVM myResult = new CalanderVM
                            {
                                id = myWarehoue.WarehouseLocationGUID.ToString() + "x" + setdate,
                                title = "Tartous: " + " " + flows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                                                             && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                            .Select(x => x.ItemOutputDetailFlowGUID).Count(),
                                start = myDate.ToString(),
                                end = null,
                                d = time.Day,
                                m = time.Month,
                                y = time.Year,
                                backgroundColor = "#3c8dbc",
                                borderColor = "#00c0ef"
                            };
                            result.Add(myResult);

                        }

                    }

                    if (myWarehoue.WarehouseLocationDescription == "Qamishli")
                    {
                        if (myFlows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                               && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                .Select(x => x.ItemOutputDetailFlowGUID).Count() > 0)
                        {



                            CalanderVM myResult = new CalanderVM
                            {
                                id = myWarehoue.WarehouseLocationGUID.ToString() + "x" + setdate,
                                title = "Qamishli: " + " " + flows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                                                                 && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                            .Select(x => x.ItemOutputDetailFlowGUID).Count(),
                                start = myDate.ToString(),
                                end = null,
                                d = time.Day,
                                m = time.Month,
                                y = time.Year,
                                backgroundColor = "#f44141",
                                borderColor = "#00c0ef"
                            };
                            result.Add(myResult);

                        }

                    }

                    if (myWarehoue.WarehouseLocationDescription == "Sweida")
                    {
                        if (myFlows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                               && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                .Select(x => x.ItemOutputDetailFlowGUID).Count() > 0)
                        {



                            CalanderVM myResult = new CalanderVM
                            {
                                id = myWarehoue.WarehouseLocationGUID.ToString() + "x" + setdate,
                                title = "Sweida: " + " " + flows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                                                             && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                            .Select(x => x.ItemOutputDetailFlowGUID).Count(),
                                start = myDate.ToString(),
                                end = null,
                                d = time.Day,
                                m = time.Month,
                                y = time.Year,
                                backgroundColor = "#9daccb",
                                borderColor = "#00c0ef"
                            };
                            result.Add(myResult);

                        }

                    }

                    if (myWarehoue.WarehouseLocationDescription == "Hama")
                    {
                        if (myFlows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                               && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                .Select(x => x.ItemOutputDetailFlowGUID).Count() > 0)
                        {



                            CalanderVM myResult = new CalanderVM
                            {
                                id = myWarehoue.WarehouseLocationGUID.ToString() + "x" + setdate,
                                title = "Hama: " + " " + flows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                                                           && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                            .Select(x => x.ItemOutputDetailFlowGUID).Count(),
                                start = myDate.ToString(),
                                end = null,
                                d = time.Day,
                                m = time.Month,
                                y = time.Year,
                                backgroundColor = "#13bda6",
                                borderColor = "#00c0ef"
                            };
                            result.Add(myResult);

                        }

                    }



                }
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMovementItemDetails(string PK)
        {
            Guid dutyGuid = Guid.Parse(PK.Substring(0, PK.IndexOf("x")));
            string date = PK.Substring(PK.IndexOf("x") + 1).ToString();
            DateTime inputdate = DateTime.ParseExact(date.ToString(), @"dd/MM/yyyy", CultureInfo.InvariantCulture);

            //DateTime date1 = Convert.ToDateTime(date);
            var model = (

              from a in DbWMS.v_trackItemOutputFlow.Where(x => x.LastWarehouseLocationGUID == dutyGuid
                                                                        && x.IsLastAction == true).ToList()
                                                                        .Where(
                                                                        x => x.FlowCreatedDate.ToShortDateString() == inputdate.ToShortDateString())


              select new WarehouseCalanderVM
              {
                  ItemOutputDetailFlowGUID = a.ItemOutputDetailFlowGUID,

                  Custodian = a.LastCustodianName.ToString(),
                  WarehouseItemDescription = a.WarehouseItemDescription,
                  BarcodeNumber = a.BarcodeNumber,
                  SerialNumber = a.SerialNumber,
                  IME1 = a.IMEI1,
                  LocationDescription = a.LastLocation,
                  IssuedDate = a.FlowCreatedDate,
                  FlowCreatedByName = a.FlowCreatedByName,

              }).ToList();


            return PartialView("~/Areas/WMS/Views/Reports/_ItemMovementCalandarDetails.cshtml", model);
        }


        #endregion

        // GET: WMS/Reports
        public ActionResult Index()
        {
            return View();
        }

        #region Warehouse Models Reports
        [HttpGet]
        [Route("WMS/Reports/")]
        public ActionResult ModelTrackReport()
        {
            ReportViewer reportViewer = new ReportViewer();
            ViewBag.ReportViewer = reportViewer;
            return View("~/Areas/WMS/Views/Reports/ModelTrackReport.cshtml", new ModelTrackReportModel());
        }

        [HttpPost]
        public ActionResult ModelTrackReport(ModelTrackReportModel reportParameters)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            Guid orgGuid = (Guid)Session[SessionKeys.OrganizationInstanceGUID];
            string custType = "Staff";
            if (reportParameters.WarehosueReportId == 1)
            {
                HttpContext.Response.Clear();
                string sourceFile = Server.MapPath("~/Areas/WMS/Templates/ItemsCustodianReport.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/WMS/temp/ItemModelTrackCustdoy" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);

                //Guid user = Guid.Parse("6FE969C7-C61E-4E06-B7E8-B037974DD79E");
                var result = DbWMS.v_trackItemOutputFlow.Where(x => x.IsLastAction == true && x.LastCustodian == custType
                && x.OrganizationInstanceGUID == orgGuid
                //&& x.LastCustdianNameGUID==user
                ).AsQueryable();
                if (reportParameters.CustodianGUIDs != null && reportParameters.CustodianGUIDs.Count > 0)
                    result = result.Where(x => reportParameters.CustodianGUIDs.Contains(x.LastCustdianNameGUID));
                if (reportParameters.WarehouseItemGUID != null && reportParameters.WarehouseItemGUID.Count > 0)
                    result = result.Where(x => reportParameters.WarehouseItemGUID.Contains(x.WarehouseItemGuid));

                var listOfStaff = result.Select(x => x.LastCustodianName).Distinct().ToList();
                string items = String.Join(",",
                    result.Where(x => reportParameters.WarehouseItemGUID.Contains(x.WarehouseItemGuid))
                        .Select(x => x.WarehouseItemDescription).Distinct().ToList());

                List<TrackCustodianVM> trackCusAll = new List<TrackCustodianVM>();
                if (reportParameters.WarehouseItemGUID.Count > 0)
                {

                    //var checkAll = result.Where(x => reportParameters.WarehouseItemGUID
                    //                                  && x.LastCustodianName == staff).FirstOrDefault();
                    //string myItemDetails = result.Select(x => new { Item="Barcode:"+x.BarcodeNumber+" Serial: "+x.SerialNumber+" IMEI"+x.IMEI }).ToString();
                    if (reportParameters.equationType == true)
                    {
                        var CustodianItems = result.GroupBy(x => new { x.WarehouseItemDescription, x.LastCustodianName })
                            .Select(x => new { Count = x.Count(), Name = x.Key.LastCustodianName, x.Key.WarehouseItemDescription }).Distinct().ToList();

                        var final = result.Select(x => new { x.LastCustodianName, x.WarehouseItemDescription }).Distinct().GroupBy(x => new { x.WarehouseItemDescription, x.LastCustodianName })
                            .Select(x => new { Count = x.Count(), Name = x.Key.LastCustodianName, x.Key.WarehouseItemDescription }).Distinct().ToList();

                        var check = final.GroupBy(f => f.Name).Select(f => new { Sum = f.Sum(xx => xx.Count), Name = f.First().Name }).Where(x => x.Sum >= reportParameters.WarehouseItemGUID.Count).ToList();

                        if (check.Count > 0)
                        {


                            using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                            {
                                var cx = package.Workbook.Worksheets.ToList();
                                ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                                var _staff = DbAHD.v_StaffProfileInformation.ToList();

                                DataTable dt = new DataTable();

                                dt.Columns.Add("Custodian", typeof(string));
                                dt.Columns.Add("Items", typeof(string));
                                dt.Columns.Add("Location", typeof(string));
                                dt.Columns.Add("Email Address", typeof(string));
                                //dt.Columns.Add("Employment ID", typeof(string));
                                //dt.Columns.Add("Items Details", typeof(string));


                                foreach (var item in check)
                                {
                                    var stringToJoin = CustodianItems.Where(x => x.Name == item.Name).Select(x =>
                                         new { Item = x.WarehouseItemDescription + ": " + x.Count }).Distinct().ToList();
                                    string CustodianItem = String.Join(",", stringToJoin.Select(x => x.Item));

                                    DataRow dr;
                                    dr = dt.NewRow();
                                    dr[0] = item.Name;


                                    dr[1] = CustodianItem;
                                    dr[2] = _staff.Where(x => x.FullName == item.Name).FirstOrDefault().DutyStation;
                                    dr[3] = _staff.Where(x => x.FullName == item.Name).FirstOrDefault().EmailAddress;
                                    //dr[4] = _staff.Where(x => x.FullName == item.Name).FirstOrDefault().EmploymentID;

                                    //dr[2] = "";

                                    dt.Rows.Add(dr);
                                }
                                workSheet.Cells["B5"].LoadFromDataTable(dt, true);
                                workSheet.Cells["B1"].Value = " List of Custodians For:";
                                workSheet.Cells["B2"].Value = items;
                                package.Save();
                            }

                            byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                            string fileName = DateTime.Now.ToString("yyMMdd") + "_Items-Custodian with multiple items " + items + ".xlsx";
                            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);


                        }
                    }

                    else
                    {
                        var checkItmeAll = result.Where(x => reportParameters.WarehouseItemGUID.Contains(x.WarehouseItemGuid)
                                                          ).ToList();

                        var CustodianItems = checkItmeAll.GroupBy(x => new { x.WarehouseItemDescription, x.LastCustodianName })
                            .Select(x => new { Count = x.Count(), Name = x.Key.LastCustodianName, x.Key.WarehouseItemDescription }).Distinct().ToList();

                        if (checkItmeAll.Count > 0)
                        {

                            sourceFile = Server.MapPath("~/Areas/WMS/Templates/ItemsCustodianReport.xlsx");
                            DisFolder =
                               Server.MapPath("~/Areas/WMS/temp/ItemModelTrackCustdoy" + DateTime.Now.ToBinary() + ".xlsx");
                            System.IO.File.Copy(sourceFile, DisFolder);
                            using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                            {
                                var cx = package.Workbook.Worksheets.ToList();
                                ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                                DataTable dt = new DataTable();

                                dt.Columns.Add("Custodian", typeof(string));
                                dt.Columns.Add("Items", typeof(string));
                                //dt.Columns.Add("Items Details", typeof(string));


                                foreach (var item in checkItmeAll.Select(x => x.LastCustodianName))
                                {
                                    var stringToJoin = CustodianItems.Where(x => x.Name == item).Select(x =>
                                        new { Item = x.WarehouseItemDescription + ": " + x.Count }).Distinct().ToList();
                                    string CustodianItem = String.Join(",", stringToJoin.Select(x => x.Item));
                                    DataRow dr;
                                    dr = dt.NewRow();
                                    dr[0] = item;
                                    dr[1] = CustodianItem;
                                    //dr[2] = "";

                                    dt.Rows.Add(dr);
                                }
                                workSheet.Cells["B5"].LoadFromDataTable(dt, true);
                                workSheet.Cells["B1"].Value = " List of Custodians For:";
                                workSheet.Cells["B2"].Value = items;

                                package.Save();
                            }

                            byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);
                            string fileName = DateTime.Now.ToString("yyMMdd") + "_Items-Custodian with multiple items " + items + ".xlsx";
                            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);


                        }
                    }



                }
            }
            else if (reportParameters.WarehosueReportId == 2)
            {
                DateTime today = DateTime.Now;
                var result = DbWMS.v_trackItemOutputFlow.Where(x => x.LastCustodian == custType
                                                         && x.ExpectedReturenedDate <= today
                                                                    && x.IsLastAction == true
                                                                      && x.OrganizationInstanceGUID == orgGuid
                                                                   ).ToList();
                string sourceFile = Server.MapPath("~/Areas/WMS/Templates/CustodainDelayInReturnItems.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/WMS/temp/CustodainDelayInReturnItems" + DateTime.Now.ToBinary() + ".xlsx");


                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();

                    dt.Columns.Add("Custodian", typeof(string));
                    dt.Columns.Add("Items", typeof(string));
                    dt.Columns.Add("Expected Returened Date ", typeof(string));
                    dt.Columns.Add("Barcode number ", typeof(string));
                    dt.Columns.Add("Serial number ", typeof(string));
                    dt.Columns.Add("IMEI ", typeof(string));
                    dt.Columns.Add("GSM ", typeof(string));
                    dt.Columns.Add("MAC ", typeof(string));


                    foreach (var item in result)
                    {


                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.LastCustodianName;
                        dr[1] = item.ModelDescription;
                        dr[2] = item.ExpectedReturenedDate.ToString();
                        dr[3] = item.BarcodeNumber;
                        dr[4] = item.SerialNumber;
                        dr[5] = item.IMEI1;
                        dr[6] = item.GSM;
                        dr[7] = item.MAC;


                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["B5"].LoadFromDataTable(dt, true);
                    workSheet.Cells["B1"].Value = " List of Items-Delayed return:";
                    //workSheet.Cells["B2"].Value = items;
                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = DateTime.Now.ToString("yyMMdd") + "_Items-Delayed return" + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

            }

            else if (reportParameters.WarehosueReportId == 3)
            {
                DateTime today = DateTime.Now;
                var myQuery = DbWMS.v_EntryMovementDataTable.Where(x => x.OrganizationInstanceGUID == orgGuid).AsQueryable();
                if (reportParameters.CustodianStaffGUID != null && reportParameters.CustodianStaffGUID.Count > 0)
                {
                    myQuery = myQuery.Where(x =>
                       reportParameters.CustodianStaffGUID.Contains(x.LastCustdianNameGUID) &&
                       x.LastCustdianGUID == WarehouseRequestSourceTypes.Staff);
                }
                if (reportParameters.CustodianTypesGUID != null && reportParameters.CustodianTypesGUID.Count > 0)
                {
                    myQuery = myQuery.Where(x =>
                       reportParameters.CustodianTypesGUID.Contains(x.LastCustdianGUID)
                       );
                }
                if (reportParameters.CustodianWarehouseGUID != null && reportParameters.CustodianWarehouseGUID.Count > 0)
                {
                    myQuery = myQuery.Where(x =>
                         reportParameters.CustodianWarehouseGUID.Contains(x.LastCustdianNameGUID) &&
                         x.LastCustdianGUID == WarehouseRequestSourceTypes.Warehouse);
                }
                if (reportParameters.ItemGUIDs != null && reportParameters.ItemGUIDs.Count > 0)
                {
                    myQuery = myQuery.Where(x =>
                        reportParameters.ItemGUIDs.Contains(x.WarehouseItemGuid)
                        );
                }
                if (reportParameters.ModelGUIDs != null && reportParameters.ModelGUIDs.Count > 0)
                {
                    myQuery = myQuery.Where(x =>
                        reportParameters.ModelGUIDs.Contains(x.ItemModelWarehouseGUID)
                    );
                }
                if (reportParameters.WarehouseItemClassificationGUID != null && reportParameters.WarehouseItemClassificationGUID.Count > 0)
                {
                    myQuery = myQuery.Where(x =>
                        reportParameters.WarehouseItemClassificationGUID.Contains(x.WarehouseItemClassificationGUID)
                    );
                }


                if (reportParameters.BrandGuids != null && reportParameters.BrandGuids.Count > 0)
                {
                    myQuery = myQuery.Where(x =>
                        reportParameters.BrandGuids.Contains(x.BrandGUID)
                    );
                }

                if (reportParameters.LocationGuids != null && reportParameters.LocationGuids.Count > 0)
                {
                    myQuery = myQuery.Where(x =>
                        reportParameters.LocationGuids.Contains(x.WarehouseLocationGUID)
                    );
                }

                if (reportParameters.ServicesStatusGUIDs != null && reportParameters.ServicesStatusGUIDs.Count > 0)
                {
                    myQuery = myQuery.Where(x =>
                        reportParameters.ServicesStatusGUIDs.Contains(x.ItemServiceStatusGUID)
                    );
                }


                string sourceFile = Server.MapPath("~/Areas/WMS/Templates/ItemModelTrackDetail.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/WMS/temp/ItemModelTrackDetail" + DateTime.Now.ToBinary() + ".xlsx");


                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();

                    dt.Columns.Add("Model Classification", typeof(string));
                    dt.Columns.Add("Sub Classification", typeof(string));
                    dt.Columns.Add("Brand ", typeof(string));
                    dt.Columns.Add("Model ", typeof(string));
                    dt.Columns.Add("Warehouse Owner ", typeof(string));
                    dt.Columns.Add("Custodian", typeof(string));
                    dt.Columns.Add("Email", typeof(string));
                    dt.Columns.Add("Department", typeof(string));
                    dt.Columns.Add("Staff Status", typeof(string));
                    dt.Columns.Add("BarcodeNumber", typeof(string));
                    dt.Columns.Add("SN ", typeof(string));
                    dt.Columns.Add("IMEI ", typeof(string));
                    dt.Columns.Add("GSM ", typeof(string));
                    dt.Columns.Add("MAC ", typeof(string));
                    dt.Columns.Add("Location ", typeof(string));
                    dt.Columns.Add("Service Status ", typeof(string));
                    dt.Columns.Add("Movement Status ", typeof(string));
                    dt.Columns.Add("Physical Status ", typeof(string));
                    dt.Columns.Add("Age–Acquisition", typeof(string));
                    dt.Columns.Add("Depreciation Value", typeof(string));
                    dt.Columns.Add("Tranfered To Country ", typeof(string));
                    dt.Columns.Add("Transfered To Duty Station ", typeof(string));
                    dt.Columns.Add("Tranfered From Country  ", typeof(string));
                    dt.Columns.Add("Tranfered From Duty Station ", typeof(string));
                    dt.Columns.Add("Comments ", typeof(string));

                    var result = myQuery.ToList();
                    var _guids = result.Select(x => x.LastCustdianNameGUID).Distinct().ToList();
                    var _staffInfo = DbAHD.v_staffCoreDataOverview.Where(x => _guids.Contains(x.UserGUID)).ToList();
                    foreach (var item in result)
                    {


                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.WarehouseItemClassificationDescription;
                        dr[1] = item.WarehouseItemDescription;
                        dr[2] = item.BrandDescription;
                        dr[3] = item.ModelDescription;

                        dr[4] = item.WarehosueOwnerName;
                        dr[5] = item.LastCustodianName;
                        dr[6] = item.EmailAddress;
                        dr[7] = _staffInfo.Where(x => x.UserGUID == item.LastCustdianNameGUID).FirstOrDefault() != null ? _staffInfo.Where(x => x.UserGUID == item.LastCustdianNameGUID).FirstOrDefault().DepartmentDescription : "-";
                        dr[8] = _staffInfo.Where(x => x.UserGUID == item.LastCustdianNameGUID).FirstOrDefault() != null ? _staffInfo.Where(x => x.UserGUID == item.LastCustdianNameGUID).FirstOrDefault().StaffStatus : "-";
                        dr[9] = item.BarcodeNumber;
                        dr[10] = item.SerialNumber;
                        dr[11] = item.IMEI;
                        dr[12] = item.GSM;
                        dr[13] = item.MAC;
                        dr[14] = item.LastLocation;
                        dr[15] = item.ServiceItemStatus;
                        dr[16] = item.LastFlow;
                        dr[17] = item.ItemStatus;
                        dr[18] = item.ModelAge;
                        dr[19] = item.ModelAge >= 4 ? "100%" : (item.ModelAge < 1 ? "0%" : item.ModelAge == 1 ? "20%" : item.ModelAge == 2 ? "40%" : item.ModelAge == 3 ? "60%" : "-");
                        dr[20] = item.CountryTransferTo;
                        dr[21] = item.DutyStationTranferTo;
                        dr[22] = item.CountryTransferFrom;
                        dr[23] = item.DutyStationTranferFrom;
                        dr[24] = item.Comments;
                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["B5"].LoadFromDataTable(dt, true);
                    workSheet.Cells["B1"].Value = "Items-Detail reprot:";
                    //workSheet.Cells["B2"].Value = items;
                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = DateTime.Now.ToString("yyMMdd") + "_Items-Detail reprot" + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

            }

            else if (reportParameters.WarehosueReportId == 4)
            {
                DateTime today = DateTime.Now;
                var myQuery = DbWMS.v_trackStaffHistorical.Where(x => x.OrganizationInstanceGUID == orgGuid).AsQueryable();
                if (reportParameters.CustodianStaffGUID != null && reportParameters.CustodianStaffGUID.Count > 0)
                {
                    myQuery = myQuery.Where(x =>
                       reportParameters.CustodianStaffGUID.Contains(x.RequesterNameGUID)
                       );
                }



                string sourceFile = Server.MapPath("~/Areas/WMS/Templates/ItemModelTrackDetail.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/WMS/temp/ItemModelTrackDetail" + DateTime.Now.ToBinary() + ".xlsx");


                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();

                    dt.Columns.Add("Item Classification", typeof(string));
                    dt.Columns.Add("Sub Classification", typeof(string));
                    dt.Columns.Add("Brand ", typeof(string));
                    dt.Columns.Add("Item ", typeof(string));
                    dt.Columns.Add("Movement Status ", typeof(string));
                    dt.Columns.Add("Warehouse Owner ", typeof(string));
                    dt.Columns.Add("Current Custodian", typeof(string));
                    dt.Columns.Add("BarcodeNumber", typeof(string));
                    dt.Columns.Add("SN ", typeof(string));
                    dt.Columns.Add("IMEI ", typeof(string));
                    dt.Columns.Add("GSM ", typeof(string));
                    dt.Columns.Add("MAC ", typeof(string));
                    dt.Columns.Add("Location ", typeof(string));
                    dt.Columns.Add("Service Status ", typeof(string));

                    dt.Columns.Add("Comments ", typeof(string));

                    var result = myQuery.ToList();

                    foreach (var item in result)
                    {


                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.WarehouseItemClassificationDescription;
                        dr[1] = item.WarehouseItemDescription;
                        dr[2] = item.BrandDescription;
                        dr[3] = item.ModelDescription;
                        dr[4] = item.LastFlow;

                        dr[5] = item.WarehosueOwnerName;
                        dr[6] = item.LastCustodianName;
                        dr[7] = item.BarcodeNumber;
                        dr[8] = item.SerialNumber;
                        dr[9] = item.IMEI;
                        dr[10] = item.GSM;
                        dr[11] = item.MAC;
                        dr[12] = item.LastLocation;
                        dr[13] = item.ServiceItemStatus;

                        dr[14] = item.Comments;
                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["B5"].LoadFromDataTable(dt, true);
                    workSheet.Cells["B1"].Value = "Items-Detail reprot:";
                    //workSheet.Cells["B2"].Value = items;
                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = DateTime.Now.ToString("yyMMdd") + "_Items-Detail reprot" + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

            }

            else if (reportParameters.WarehosueReportId == 5)
            {
                DateTime today = DateTime.Now;
                var myQuery = DbWMS.v_trackConsumableItemDetail.Where(x => x.OrganizationInstanceGUID == orgGuid).AsQueryable();
                //xx
                // var res = DbWMS.v_trackConsumableItemDetail.ToList();
                if (reportParameters.CustodianStaffGUID != null && reportParameters.CustodianStaffGUID.Count > 0)
                {
                    myQuery = myQuery.Where(x =>
                       reportParameters.CustodianStaffGUID.Contains(x.RequesterNameGUID) &&
                       x.RequesterGUID == WarehouseRequestSourceTypes.Staff);
                }
                if (reportParameters.CustodianWarehouseGUID != null && reportParameters.CustodianWarehouseGUID.Count > 0)
                {
                    myQuery = myQuery.Where(x =>
                         reportParameters.CustodianWarehouseGUID.Contains(x.RequesterNameGUID) &&
                         x.RequesterGUID == WarehouseRequestSourceTypes.Warehouse);
                }
                if (reportParameters.ItemGUIDs != null && reportParameters.ItemGUIDs.Count > 0)
                {
                    myQuery = myQuery.Where(x =>
                        reportParameters.ItemGUIDs.Contains(x.WarehouseItemGUID)
                        );
                }
                if (reportParameters.ModelGUIDs != null && reportParameters.ModelGUIDs.Count > 0)
                {
                    myQuery = myQuery.Where(x =>
                        reportParameters.ModelGUIDs.Contains(x.ItemModelWarehouseGUID)
                    );
                }
                if (reportParameters.WarehouseItemClassificationGUID != null && reportParameters.WarehouseItemClassificationGUID.Count > 0)
                {
                    myQuery = myQuery.Where(x =>
                        reportParameters.WarehouseItemClassificationGUID.Contains(x.WarehouseItemClassificationGUID)
                    );
                }


                //if (reportParameters.BrandGuids != null && reportParameters.BrandGuids.Count > 0)
                //{
                //    myQuery = myQuery.Where(x =>
                //        reportParameters.BrandGuids.Contains(x.BrandGUID)
                //    );
                //}

                if (reportParameters.LocationGuids != null && reportParameters.LocationGuids.Count > 0)
                {
                    myQuery = myQuery.Where(x =>
                        reportParameters.LocationGuids.Contains(x.WarehouseLocationGUID)
                    );
                }

                //if (reportParameters.ServicesStatusGUIDs != null && reportParameters.ServicesStatusGUIDs.Count > 0)
                //{
                //    myQuery = myQuery.Where(x =>
                //        reportParameters.ServicesStatusGUIDs.Contains(x.ItemServiceStatusGUID)
                //    );
                //}


                string sourceFile = Server.MapPath("~/Areas/WMS/Templates/ConsumableItemModelTrackDetail.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/WMS/temp/ConsumableItemModelTrackDetail" + DateTime.Now.ToBinary() + ".xlsx");


                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();

                    dt.Columns.Add("Item Classification", typeof(string));
                    dt.Columns.Add("Sub Classification", typeof(string));
                    //dt.Columns.Add("Brand ", typeof(string));
                    dt.Columns.Add("Item ", typeof(string));
                    //dt.Columns.Add("Warehouse Owner ", typeof(string));
                    dt.Columns.Add("Custodian", typeof(string));
                    dt.Columns.Add("Custodian Name", typeof(string));

                    dt.Columns.Add("Location ", typeof(string));
                    dt.Columns.Add("Quantity ", typeof(string));
                    dt.Columns.Add("Release Date ", typeof(string));


                    var result = myQuery.ToList();

                    foreach (var item in result)
                    {


                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.WarehouseItemClassificationDescription;
                        dr[1] = item.WarehouseItemDescription;
                        //dr[2] = item.BrandDescription;
                        dr[2] = item.ModelDescription;

                        //dr[4] = item.WarehosueOwnerName;
                        dr[3] = item.LastCustodian;
                        dr[4] = item.LastCustodianName;

                        dr[5] = item.WarehouseLocationDescription;
                        dr[6] = item.RequestedQunatity;
                        dr[7] = item.ExpectedStartDate;

                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["B5"].LoadFromDataTable(dt, true);
                    workSheet.Cells["B1"].Value = "Items-Consumable-Detail reprot:";
                    //workSheet.Cells["B2"].Value = items;
                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = DateTime.Now.ToString("yyMMdd") + "_Items-Detail reprot" + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

            }
            else if (reportParameters.WarehosueReportId == 6)
            {
                DateTime today = DateTime.Now;
                var result = DbWMS.v_EntryMovementDataTable.Where(x => x.OrganizationInstanceGUID == orgGuid).OrderByDescending(x => x.LastVerifiedDate).ToList();
                string sourceFile = Server.MapPath("~/Areas/WMS/Templates/TrackVerificationItem.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/WMS/temp/TrackVerificationItem" + DateTime.Now.ToBinary() + ".xlsx");


                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();

                    dt.Columns.Add("Item Classification", typeof(string));
                    dt.Columns.Add("Sub Classification", typeof(string));
                    dt.Columns.Add("Brand ", typeof(string));
                    dt.Columns.Add("Item ", typeof(string));
                    dt.Columns.Add("Warehouse Owner ", typeof(string));
                    dt.Columns.Add("Custodian", typeof(string));
                    dt.Columns.Add("BarcodeNumber", typeof(string));
                    dt.Columns.Add("SN ", typeof(string));
                    dt.Columns.Add("IMEI ", typeof(string));
                    dt.Columns.Add("GSM ", typeof(string));
                    dt.Columns.Add("MAC ", typeof(string));
                    dt.Columns.Add("Location ", typeof(string));
                    dt.Columns.Add("Service Status ", typeof(string));
                    dt.Columns.Add("Movement Status ", typeof(string));
                    dt.Columns.Add("Verification date ", typeof(string));
                    dt.Columns.Add("Verify by ", typeof(string));
                    dt.Columns.Add("Comments ", typeof(string));


                    foreach (var item in result)
                    {


                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.WarehouseItemClassificationDescription;
                        dr[1] = item.WarehouseItemDescription;
                        dr[2] = item.BrandDescription;
                        dr[3] = item.ModelDescription;

                        dr[4] = item.WarehosueOwnerName;
                        dr[5] = item.LastCustodianName;
                        dr[6] = item.BarcodeNumber;
                        dr[7] = item.SerialNumber;
                        dr[8] = item.IMEI1;
                        dr[9] = item.GSM;
                        dr[10] = item.MAC;
                        dr[11] = item.LastLocation;
                        dr[12] = item.ServiceItemStatus;
                        dr[13] = item.LastFlow;
                        dr[14] = item.LastVerifiedDate;
                        dr[15] = item.VerifiedBy;
                        dr[16] = item.Comments;
                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["B5"].LoadFromDataTable(dt, true);
                    workSheet.Cells["B1"].Value = "Items-Verification reprot:";
                    //workSheet.Cells["B2"].Value = items;
                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = DateTime.Now.ToString("yyMMdd") + "_Items-Verification return" + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

            }
            //damaged 
            else if (reportParameters.WarehosueReportId == 7)
            {
                HttpContext.Response.Clear();
                DateTime today = DateTime.Now;
                Guid damageGUID = Guid.Parse("675DE853-151B-4C2F-93F4-DA1434EEE766");
                Guid lostGuid = Guid.Parse("675DE853-151B-4C2F-93F4-DA1434EEE765");
                Guid _donation = Guid.Parse("0A31225F-2064-4F02-93D2-FE1519CF6AC7");
                var result = DbWMS.v_EntryMovementDataTable.Where(x => x.Active == true
                                                         && (x.ItemStatusGUID == damageGUID || x.ItemStatusGUID == lostGuid || x.LastCustdianNameGUID == _donation)

                                                         && x.OrganizationInstanceGUID == orgGuid
                                                                   ).ToList();
                string sourceFile = Server.MapPath("~/Areas/WMS/Templates/GS45Report.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/WMS/temp/GS45Report" + DateTime.Now.ToBinary() + ".xlsx");

                var _confimed = Guid.Parse("0C44822F-A898-476D-B291-CAF1B055AAC2");
                var _inputGUID = result.Where(x => x.LastCustdianNameGUID == _donation).Select(x => x.ItemInputDetailGUID).Distinct().ToList();
                var _history = DbWMS.dataItemOutputDetail.Where(x => _inputGUID.Contains((Guid)x.ItemInputDetailGUID)
                        && x.dataItemOutputDetailFlow.FirstOrDefault().FlowTypeGUID == _confimed).ToList();
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];


                    DataTable dt = new DataTable();
                    dt.Columns.Add("Item Category", typeof(string));
                    dt.Columns.Add("Item Group", typeof(string));

                    dt.Columns.Add("Model", typeof(string));
                    dt.Columns.Add("Reason", typeof(string));

                    dt.Columns.Add("Staff Name", typeof(string));
                    dt.Columns.Add("Location", typeof(string));
                    dt.Columns.Add("SN ", typeof(string));
                    dt.Columns.Add("BC ", typeof(string));
                    dt.Columns.Add("Damaged Part", typeof(string));
                    dt.Columns.Add("Action Date ", typeof(string));
                    dt.Columns.Add("Comments ", typeof(string));
                    // dt.Columns.Add("User  comments as submitted in the staff report ", typeof(string));



                    foreach (var item in result.OrderBy(x => x.ItemStatusGUID))
                    {


                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.WarehouseItemClassificationDescription;
                        dr[1] = item.WarehouseItemDescription;
                        dr[2] = item.ModelDescription;
                        dr[3] = item.ItemStatusGUID == Guid.Parse("675DE853-151B-4C2F-93F4-DA1434EEE766") ? "Damaged" : item.ItemStatusGUID == lostGuid ? "Lost" : "Donation";

                        dr[4] = DbWMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == item.DamagedByGUID && x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + DbWMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == item.DamagedByGUID && x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault();
                        dr[5] = item.LastLocation;
                        dr[6] = item.SerialNumber;
                        dr[7] = item.BarcodeNumber;
                        dr[8] = item.DamagedPart;
                        dr[9] = item.LastCustdianNameGUID == _donation ? _history.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID).FirstOrDefault().ActualReturenedDate == null ? (object)DBNull.Value : Convert.ToDateTime(_history.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID).FirstOrDefault().ActualReturenedDate).ToShortDateString()
                            : item.DamagedDate == null ? (object)DBNull.Value : Convert.ToDateTime(item.DamagedDate).ToShortDateString();
                        dr[10] = item.DamagedComment;
                        //dr[6] = item.DamagedDate;
                        //dr[6] = item.GSM;
                        //dr[7] = item.MAC;


                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["B5"].LoadFromDataTable(dt, true);
                    workSheet.Cells["B1"].Value = " List of Items-GS45:";
                    //workSheet.Cells["B2"].Value = items;
                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = DateTime.Now.ToString("yyMMdd") + "_GS45" + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

            }
            //lost
            else if (reportParameters.WarehosueReportId == 8)
            {
                DateTime today = DateTime.Now;
                Guid lostGuid = Guid.Parse("675DE853-151B-4C2F-93F4-DA1434EEE765");
                var result = DbWMS.v_EntryMovementDataTable.Where(x => x.Active == true
                                                         && x.ItemStatusGUID == lostGuid
                                                         && x.OrganizationInstanceGUID == orgGuid
                                                                   ).ToList();
                string sourceFile = Server.MapPath("~/Areas/WMS/Templates/LostReport.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/WMS/temp/DamagedReport" + DateTime.Now.ToBinary() + ".xlsx");


                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();

                    dt.Columns.Add("Model", typeof(string));
                    dt.Columns.Add("Staff Name", typeof(string));
                    dt.Columns.Add("SN ", typeof(string));
                    dt.Columns.Add("BC ", typeof(string));
                    //dt.Columns.Add("Damaged Part ", typeof(string));
                    dt.Columns.Add("Comments ", typeof(string));
                    //dt.Columns.Add("User  comments as submitted in the staff report ", typeof(string));



                    foreach (var item in result)
                    {


                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.ModelDescription;
                        dr[1] = item.LastCustodianName;
                        dr[2] = item.SerialNumber;
                        dr[3] = item.BarcodeNumber;
                        //dr[4] = item.DamagedPart;
                        dr[4] = item.Comments;


                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["B5"].LoadFromDataTable(dt, true);
                    workSheet.Cells["B1"].Value = " List of Items-Lost :";
                    //workSheet.Cells["B2"].Value = items;
                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = DateTime.Now.ToString("yyMMdd") + "_Items-Lost" + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

            }

            //Donation
            else if (reportParameters.WarehosueReportId == 9)
            {
                DateTime today = DateTime.Now;
                Guid _donation = Guid.Parse("0A31225F-2064-4F02-93D2-FE1519CF6AC7");
                var result = DbWMS.v_EntryMovementDataTable.Where(x => x.Active == true
                                                         && x.LastCustdianNameGUID == _donation
                                                           && x.OrganizationInstanceGUID == orgGuid
                                                                   ).ToList();
                var _inputGUID = result.Select(x => x.ItemInputDetailGUID).Distinct().ToList();
                var _confimed = Guid.Parse("0C44822F-A898-476D-B291-CAF1B055AAC2");
                var _history = DbWMS.dataItemOutputDetail.Where(x => _inputGUID.Contains((Guid)x.ItemInputDetailGUID)
                        && x.dataItemOutputDetailFlow.FirstOrDefault().FlowTypeGUID == _confimed).ToList();
                string sourceFile = Server.MapPath("~/Areas/WMS/Templates/DonationReport.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/WMS/temp/DonationReport" + DateTime.Now.ToBinary() + ".xlsx");


                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();

                    dt.Columns.Add("Item Category", typeof(string));
                    dt.Columns.Add("Item Group", typeof(string));

                    dt.Columns.Add("Model", typeof(string));
                    dt.Columns.Add("Target", typeof(string));


                    dt.Columns.Add("Location", typeof(string));
                    dt.Columns.Add("SN ", typeof(string));
                    dt.Columns.Add("BC ", typeof(string));

                    dt.Columns.Add("Donation Date ", typeof(string));
                    dt.Columns.Add("Comments ", typeof(string));
                    //dt.Columns.Add("User  comments as submitted in the staff report ", typeof(string));



                    foreach (var item in result)
                    {


                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.WarehouseItemClassificationDescription;
                        dr[1] = item.WarehouseItemDescription;
                        dr[2] = item.ModelDescription;
                        dr[3] = "Donation";


                        dr[4] = item.LastLocation;
                        dr[5] = item.SerialNumber;
                        dr[6] = item.BarcodeNumber;

                        dr[7] = _history.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID).FirstOrDefault().ActualReturenedDate == null ? (object)DBNull.Value : Convert.ToDateTime(_history.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID).FirstOrDefault().ActualReturenedDate).ToShortDateString(); ;
                        dr[8] = _history.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID).FirstOrDefault().Comments;


                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["B5"].LoadFromDataTable(dt, true);
                    workSheet.Cells["B1"].Value = " List of Items-Donation :";
                    //workSheet.Cells["B2"].Value = items;
                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = DateTime.Now.ToString("yyMMdd") + "_Items-Donation return" + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

            }

            //SIM Card Usage
            else if (reportParameters.WarehosueReportId == 10)
            {
                DateTime today = DateTime.Now;
                //Guid _donation = Guid.Parse("0A31225F-2064-4F02-93D2-FE1519CF6AC7");
                var result = DbWMS.v_TrackSIMCardRoamingDetail.Where(x => x.OrganizationInstanceGUID == orgGuid).ToList();
                string sourceFile = Server.MapPath("~/Areas/WMS/Templates/SIMCardRoamingDetail.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/WMS/temp/SIMCardRoamingDetail" + DateTime.Now.ToBinary() + ".xlsx");


                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();

                    dt.Columns.Add("Model", typeof(string));
                    dt.Columns.Add("GSM", typeof(string));
                    dt.Columns.Add("SN", typeof(string));
                    dt.Columns.Add("Current Custodian ", typeof(string));
                    dt.Columns.Add("Package Size ", typeof(string));
                    dt.Columns.Add("HasRoaming ", typeof(string));
                    dt.Columns.Add("HasInternationalAccess ", typeof(string));
                    dt.Columns.Add("Duty Station ", typeof(string));
                    //dt.Columns.Add("User  comments as submitted in the staff report ", typeof(string));



                    foreach (var item in result)
                    {


                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.ModelName;
                        dr[1] = item.GSM;
                        dr[2] = item.SerialNumber;
                        dr[3] = item.CurrentCustodian;
                        dr[4] = item.PackageSizeMain;
                        dr[5] = item.HasRoaming;
                        dr[6] = item.HasInternationalAccess;
                        dr[7] = item.DutyStation;



                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["B5"].LoadFromDataTable(dt, true);
                    workSheet.Cells["B1"].Value = " Items_SIM_Cards_Mobile_Data :";
                    //workSheet.Cells["B2"].Value = items;
                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = DateTime.Now.ToString("yyMMdd") + "_Items_SIM_Cards_Mobile_Data" + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

            }


            //List of staff with items in custody
            else if (reportParameters.WarehosueReportId == 11)
            {
                DateTime today = DateTime.Now;
                //Guid _donation = Guid.Parse("0A31225F-2064-4F02-93D2-FE1519CF6AC7");
                var result = DbWMS.v_StaffWithItemsInCustody.Where(x => x.Items != null).ToList();
                string sourceFile = Server.MapPath("~/Areas/WMS/Templates/StaffItemsReport.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/WMS/temp/StaffItemsReport" + DateTime.Now.ToBinary() + ".xlsx");


                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();

                    dt.Columns.Add("Staff Name", typeof(string));
                    dt.Columns.Add("Department", typeof(string));
                    dt.Columns.Add("EmailAddress", typeof(string));
                    dt.Columns.Add("Duty Station ", typeof(string));
                    dt.Columns.Add("Recruitment Type ", typeof(string));
                    dt.Columns.Add("Job Title ", typeof(string));
                    dt.Columns.Add("Staff Status ", typeof(string));
                    dt.Columns.Add("Items ", typeof(string));
                    //dt.Columns.Add("User  comments as submitted in the staff report ", typeof(string));



                    foreach (var item in result)
                    {


                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.FullName;
                        dr[1] = item.DepartmentDescription;
                        dr[2] = item.EmailAddress;
                        dr[3] = item.DutyStation;
                        dr[4] = item.RecruitmentType;
                        dr[5] = item.JobTitle;
                        dr[6] = item.StaffStatus;
                        dr[7] = item.Items;



                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["B5"].LoadFromDataTable(dt, true);
                    workSheet.Cells["B1"].Value = "Staff-Items-Report :";
                    //workSheet.Cells["B2"].Value = items;
                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = DateTime.Now.ToString("yyMMdd") + "_Staff-Items-Report " + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

            }

            else if (reportParameters.WarehosueReportId == 12)
            {
                DateTime today = DateTime.Now;
                Guid _staff = Guid.Parse("D5E2A4DF-C67E-4F4D-A242-7E33C381EC11");
                var test = Guid.Parse("5CEF6D09-DCA2-45C2-927C-8DBEBEE0C522");
                var result = DbWMS.v_EntryMovementDataTable.Where(x => x.LastCustdianGUID == _staff
                && x.OrganizationInstanceGUID == orgGuid


                ).ToList();
                string sourceFile = Server.MapPath("~/Areas/WMS/Templates/StaffMainItemOverview.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/WMS/temp/StaffMainItemOverview" + DateTime.Now.ToBinary() + ".xlsx");


                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                    Guid _sim = Guid.Parse("4CBD2590-E7AA-4464-AFA1-2A0BC301840C");
                    string _2g = "2G";
                    //string _3g = "4G";
                    //string _4g = "3G";
                    Guid _mobile = Guid.Parse("55DD6C58-3D31-4DCE-A094-FA52FAD4DC56");
                    Guid _laptop = Guid.Parse("7ACEF0A6-5932-474E-BFD0-70780C7B8331");
                    Guid _desktop = Guid.Parse("54EE2583-7D58-4214-89B3-2AA680C68228");
                    //string _routerSyriatel = "Syriatel";
                    //string _routerZain = "Zain";
                    //string _routerMTN = "MTN";
                    Guid _powerBank = Guid.Parse("8E78FDD4-858F-488D-81EA-1CEF10B0056C");
                    Guid _solarKit = Guid.Parse("4B34C6F2-1502-4217-9458-B11BB113283D");
                    Guid _internetRouter = Guid.Parse("22079CBB-A533-4BDB-8AD9-28651CEBEB35");
                    //string _sim = "SIM";
                    //string _sim = "SIM";
                    //string _sim = "SIM";
                    //string _sim = "SIM"; 
                    //string _sim = "SIM";
                    var _allstaffs = DbAHD.v_StaffProfileInformation.ToList();
                    var _ids = result.Select(x => x.ItemInputDetailGUID).Distinct().ToList();
                    var _4gFlowGUIDs = DbWMS.dataItemOutputDetailFlow.Where(x => _ids.Contains((Guid)x.dataItemOutputDetail.ItemInputDetailGUID)
                      && x.IsLastAction == true
                      && x.IsLastMove == true

                    ).Select(x => x.ItemOutputDetailGUID).Distinct().ToList();

                    var _det = DbWMS.dataItemOutputDetail.Where(x => _4gFlowGUIDs.Contains((Guid)x.ItemOutputDetailGUID)
                      && x.SIMPackageSizeGUID != null).Distinct().ToList();

                    DataTable dt = new DataTable();

                    dt.Columns.Add("Staff Name", typeof(string));
                    dt.Columns.Add("Job Title", typeof(string));
                    dt.Columns.Add("Duty Station", typeof(string));
                    dt.Columns.Add("Recruitment Type", typeof(string));
                    dt.Columns.Add("Status", typeof(string));
                    dt.Columns.Add("Has 2G(Calls)", typeof(string));
                    dt.Columns.Add("2G(Calls) Info", typeof(string));
                    dt.Columns.Add("Has Internet Service", typeof(string));
                    dt.Columns.Add("Internet Packages", typeof(string));

                    dt.Columns.Add("Has Internt Router", typeof(string));
                    dt.Columns.Add("Internet Router Info", typeof(string));


                    dt.Columns.Add("Has Mobile", typeof(string));
                    dt.Columns.Add("Mobile Info", typeof(string));
                    dt.Columns.Add("Has Laptop", typeof(string));
                    dt.Columns.Add("Laptop Info", typeof(string));
                    dt.Columns.Add("Has Desktop", typeof(string));
                    dt.Columns.Add("Desktop Info", typeof(string));

                    dt.Columns.Add("Has Power-Bank ", typeof(string));
                    dt.Columns.Add("Power-Bank Info", typeof(string));

                    dt.Columns.Add("Has Solar-Kit", typeof(string));
                    dt.Columns.Add("Solar-Kit Info", typeof(string));

                    Guid _pa = Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E7777");
                    var _tablePac = DbWMS.codeTablesValuesLanguages.Where(x => x.codeTablesValues.TableGUID == _pa).ToList();

                    foreach (var item in result.OrderBy(x => x.LastCustodianName).Select(x => x.LastCustdianNameGUID).Distinct())
                    {
                        var _myStaff = result.Where(x => x.LastCustdianNameGUID == item).FirstOrDefault();



                        string _sim2g = String.Join(",",
                                 result.Where(x => x.WarehouseItemGuid == _sim
                                 && x.LastCustdianNameGUID == item && x.ModelDescription.Contains(_2g))
                                     .Select(x => x.ModelDescription).Distinct().ToList());

                        //string _sim4g = String.Join(",",
                        //     result.Where(x => x.WarehouseItemDescription == _sim && x.LastCustdianNameGUID == item
                        //     && ((x.ModelDescription.Contains(_3g)) || x.ModelDescription.Contains(_4g))
                        //     &&(x.ModelDescription.Contains(_routerSyriatel) ||
                        //     x.ModelDescription.Contains(_routerZain) ||
                        //     x.ModelDescription.Contains(_routerMTN)
                        //     )
                        //     )

                        //         .Select(x => x.ModelDescription).Distinct().ToList());

                        string _mobileinfo = String.Join(",",
                             result.Where(x => x.LastCustdianNameGUID == item
                             && x.WarehouseItemGuid == _mobile)
                                 .Select(x => x.ModelDescription).Distinct().ToList());


                        string _powerbankInfo = String.Join(",",
                           result.Where(x => x.LastCustdianNameGUID == item
                           && x.WarehouseItemGuid == _powerBank)
                               .Select(x => x.ModelDescription).Distinct().ToList());



                        string __solarKitInfo = String.Join(",",
                        result.Where(x => x.LastCustdianNameGUID == item
                        && x.WarehouseItemGuid == _solarKit)
                            .Select(x => x.ModelDescription).Distinct().ToList());


                        string _internetRouterInfo = String.Join(",",
                        result.Where(x => x.LastCustdianNameGUID == item
                        && x.WarehouseItemGuid == _internetRouter)
                            .Select(x => x.ModelDescription).Distinct().ToList());


                        string _laptopinfo = String.Join(",",
                                 result.Where(x => x.LastCustdianNameGUID == item
                                 && x.WarehouseItemGuid == _laptop)
                                     .Select(x => x.ModelDescription).Distinct().ToList());

                        string _desktopinfo = String.Join(",",
                   result.Where(x => x.LastCustdianNameGUID == item
                   && x.WarehouseItemGuid == _desktop)
                       .Select(x => x.ModelDescription).Distinct().ToList());

                        var _check_values = _det.Where(x => x.dataItemOutput.RequesterNameGUID == item)
                        .Select(x => x.SIMPackageSizeGUID).Distinct();

                        var _staffPackg = _det.Where(x => x.dataItemOutput.RequesterNameGUID == item)
                          .Select(x => x.SIMPackageSizeGUID).Distinct().ToList();
                        var _checkSize = _tablePac.Where(x => _staffPackg.Contains(x.ValueGUID)).ToList();

                        if (result.Where(x => x.LastCustdianNameGUID == item && x.ModelDescription.Contains(_2g)).FirstOrDefault() == null
                                &&
                                _det.Where(x => x.dataItemOutput.RequesterNameGUID == item && x.SIMPackageSize != null).FirstOrDefault() == null
                                &&
                                 result.Where(x => x.LastCustdianNameGUID == item && x.WarehouseItemGuid == _mobile).FirstOrDefault() == null
                                &&
                                result.Where(x => x.LastCustdianNameGUID == item && x.WarehouseItemGuid == _laptop).FirstOrDefault() == null

                                &&
                                 result.Where(x => x.LastCustdianNameGUID == item && x.WarehouseItemGuid == _desktop).FirstOrDefault() == null)
                        {
                            continue;
                        }

                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = _myStaff.LastCustodianName;
                        dr[1] = _allstaffs.Where(x => x.UserGUID == item).FirstOrDefault() != null ? _allstaffs.Where(x => x.UserGUID == item).FirstOrDefault().JobTitle : "";
                        dr[2] = _myStaff.LastLocation;
                        dr[3] = _allstaffs.Where(x => x.UserGUID == item).FirstOrDefault() != null ? _allstaffs.Where(x => x.UserGUID == item).FirstOrDefault().RecruitmentType : "";
                        dr[4] = _allstaffs.Where(x => x.UserGUID == item).FirstOrDefault() != null ? _allstaffs.Where(x => x.UserGUID == item).FirstOrDefault().StaffStatus : "";
                        dr[5] = result.Where(x => x.LastCustdianNameGUID == item && x.ModelDescription.Contains(_2g)).FirstOrDefault() != null ? "Yes" : "No";
                        dr[6] = _sim2g;
                        dr[7] = _det.Where(x => x.dataItemOutput.RequesterNameGUID == item && x.SIMPackageSize != null).FirstOrDefault() != null ? "Yes" : "No";
                        dr[8] = _det.Where(x => x.dataItemOutput.RequesterNameGUID == item).FirstOrDefault() != null ?
                            String.Join("", _checkSize.Select(x => x.ValueDescription)) : "";

                        dr[9] = result.Where(x => x.LastCustdianNameGUID == item && x.WarehouseItemGuid == _internetRouter).FirstOrDefault() != null ? "Yes" : "No";
                        dr[10] = _internetRouterInfo;



                        dr[11] = result.Where(x => x.LastCustdianNameGUID == item && x.WarehouseItemGuid == _mobile).FirstOrDefault() != null ? "Yes" : "No";
                        dr[12] = _mobileinfo;
                        dr[13] = result.Where(x => x.LastCustdianNameGUID == item && x.WarehouseItemGuid == _laptop).FirstOrDefault() != null ? "Yes" : "No";
                        dr[14] = _laptopinfo;

                        dr[15] = result.Where(x => x.LastCustdianNameGUID == item && x.WarehouseItemGuid == _desktop).FirstOrDefault() != null ? "Yes" : "No";
                        dr[16] = _desktopinfo;

                        dr[17] = result.Where(x => x.LastCustdianNameGUID == item && x.WarehouseItemGuid == _powerBank).FirstOrDefault() != null ? "Yes" : "No";
                        dr[18] = _powerbankInfo;

                        dr[19] = result.Where(x => x.LastCustdianNameGUID == item && x.WarehouseItemGuid == _solarKit).FirstOrDefault() != null ? "Yes" : "No";
                        dr[20] = __solarKitInfo;


                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["B5"].LoadFromDataTable(dt, true);
                    workSheet.Cells["B1"].Value = "Staff-Main-Items-Report :";
                    //workSheet.Cells["B2"].Value = items;
                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = DateTime.Now.ToString("yyMMdd") + "_Staff-Main-Items-Report " + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

            }
            string sourceFile1 = Server.MapPath("~/Areas/WMS/Templates/ItemModelTrackDetail.xlsx");
            string DisFolder1 =
                Server.MapPath("~/Areas/WMS/temp/ItemModelTrackDetail" + DateTime.Now.ToBinary() + ".xlsx");
            byte[] fileBytes1 = System.IO.File.ReadAllBytes(DisFolder1);
            string fileName1 = DateTime.Now.ToString("yyMMdd") + "List of Custodian " + ".xlsx";

            return File(fileBytes1, System.Net.Mime.MediaTypeNames.Application.Octet, fileName1);
            // return Json(DbCMS.SingleUpdateMessage(null, null, null, null, "Please Wait...."));
        }


        #endregion
        [Route("WMS/Reports/OverviewReportIndex/")]
        public ActionResult ReportsIndex()
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/Reports/OverviewReportIndex.cshtml");
        }
        public JsonResult InitReportIndex()
        {
            Guid orgGuid = (Guid)Session[SessionKeys.OrganizationInstanceGUID];
            var dataReportTemplates = DbWMS.dataReportTemplate.Where(x => x.OrganizationInstanceGUID == orgGuid).Select(x => new
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
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            int Start = 0;
            int Length = 3000000;
            string QueryStatment = "";
            var columns = new List<string>();
            //DataSystem dataSystem =  DbDAS.DataSystems.Find(systemId);
            List<string[]> result = new List<string[]>();
            //var system = DbDAS.dataReportTemplateConfiguration.Where(x => x.Name == "PortalDB").FirstOrDefault();
            var conncetion = "Data Source=10.244.8.14;Initial Catalog=PortalDB;User ID=CMSUser;Password=KjP4~7R`FbTjXbWiKo:Z";

            long resultCount = 0;

            try
            {
                Guid orgGuid = (Guid)Session[SessionKeys.OrganizationInstanceGUID];
                var Report = DbWMS.dataReportTemplate.Where(x => x.ReportTemplateId == ReportTemplateId && x.OrganizationInstanceGUID == orgGuid).FirstOrDefault();
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

    }
}