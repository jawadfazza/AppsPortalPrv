using AppsPortal.Areas.PCR.Rdlc;
using AppsPortal.Areas.PCR.Rdlc.PCRDataSetTableAdapters;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.PCR.ViewModels;
using Microsoft.Reporting.WebForms;
using OfficeOpenXml;
using PCR_DAL.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;



namespace AppsPortal.Areas.PCR.Controllers
{
    public class ReportsController : PCRBaseController
    {

        [HttpGet]
        public ActionResult ReportIndex()
        {

            return View("~/Areas/PCR/Views/Reports/ReportIndex.cshtml", new PCRReportParametersMultiple());
        }

        #region Partner Report
        [HttpGet]
        public ActionResult PartnerReport()
        {
            ReportViewer reportViewer = new ReportViewer();
            ViewBag.ReportViewer = reportViewer;
            return View("~/Areas/PCR/Views/Reports/PartnerReports.cshtml", new PCRReportParametersMultiple());
        }
        [HttpPost]
        public ActionResult PartnerReport(PCRReportParametersMultiple rp)
        {
            string URL = "LoadUrl('ReportViewer','PartnerReportLoad?" +
                "&EndDate=" + rp.EndDate.Value.Ticks +
                "&ReportGUID=" + rp.ReportGUID +
                "&DutyStationGUID=" + string.Join(",", rp.DutyStationGUID) +
                "&OrganizationInstanceGUID=" + string.Join(",", rp.OrganizationInstanceGUID) + "')";
            return Json(DbCMS.SingleUpdateMessage(null, null, null, URL, "Please Wait...."));
        }
        public ActionResult PartnerReportLoad(long? EndDate, Guid ReportGUID, string DutyStationGUID, string OrganizationInstanceGUID)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.ZoomMode = ZoomMode.Percent;
            reportViewer.ZoomPercent = 100;
            reportViewer.Width = System.Web.UI.WebControls.Unit.Pixel(900);
            reportViewer.Height = System.Web.UI.WebControls.Unit.Pixel(700);

            reportViewer.AsyncRendering = true;
            reportViewer.LocalReport.DataSources.Clear();

            RP_PartnerReportsTableAdapter PartnerReport = new RP_PartnerReportsTableAdapter();
            PCRDataSet dataSet = new PCRDataSet();
            PartnerReport.Fill(dataSet.RP_PartnerReports,
                new DateTime(EndDate.Value),
                ReportGUID,
                OrganizationInstanceGUID,
                DutyStationGUID, LAN);
            ReportDataSource reportDataSource = new ReportDataSource("PartnerReport", dataSet.Tables["RP_PartnerReports"]);
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Areas/PCR/Rdlc\PartnerReports.rdlc";
            ViewBag.ReportViewer = reportViewer;
            return PartialView("~/Areas/PCR/Views/Reports/_Report.cshtml");
        }
        #endregion

        #region Standard Reports
        [HttpGet]
        public ActionResult StanderReports()
        {
            ReportViewer reportViewer = new ReportViewer();
            ViewBag.ReportViewer = reportViewer;
            return View("~/Areas/PCR/Views/Reports/StanderReports.cshtml", new PCRReportParametersMultiple());
        }

        private PCRReportParametersMultiple FillRP(PCRReportParametersMultiple rp)
        {
            if (rp.DutyStationGUID == null) { rp.DutyStationGUID = DropDownList.SyriaDutyStationsForPCR().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            if (rp.OrganizationInstanceGUID == null) { rp.OrganizationInstanceGUID = DropDownList.OrganizationsInstancesAcronymByProfile().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            if (rp.PartnerCenterGUID == null) { rp.PartnerCenterGUID = DropDownList.PartnerCenterAll(rp.DutyStationGUID, rp.OrganizationInstanceGUID).Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            if (rp.CategoryGUID2 == null) { rp.CategoryGUID2 = DropDownList.CategoryPartnerReportLevelChart2(rp.ReportGUID).Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            if (rp.CategoryGUID3 == null) { rp.CategoryGUID3 = DropDownList.ParentCategoryPartnerReport(string.Join(",", rp.CategoryGUID2)).Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            if (rp.GenderGUID == null) { rp.GenderGUID = DropDownList.Genders().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            if (rp.AgeGUID == null) { rp.AgeGUID = DropDownList.AggregationAge(string.Join(",", rp.GenderGUID)).OrderBy(x => x.Value).Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            if (rp.ProfileGUID == null) { rp.ProfileGUID = DropDownList.AggregationProfile(string.Join(",", rp.GenderGUID)).OrderBy(x => x.Value).Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            if (rp.ReferralReasonGUID == null) { rp.ReferralReasonGUID = DbPCR.codeAggregation.Where(x => x.ReportGUID == ReportGUIDs.Referral).Select(x => x.AggregationGUID).ToArray(); }
            return rp;
        }
        [HttpPost]
        public FileResult StanderReports(PCRReportParametersMultiple rp)
        {
            List<DataTableFilterOptions> filterOptions = new List<DataTableFilterOptions>();
            if (rp.ReportGUID == ReportGUIDs.Monthly)
            {
                filterOptions.Add(new DataTableFilterOptions() { Field = "Category2", FieldData = "- Registered", Operation = "ne" });
            }
            Expression<Func<RP_PartnerReports_Result, bool>> Predicate = p => true;
            Predicate = SearchHelper.CreateSearchPredicate<RP_PartnerReports_Result>(new DataTableFilters() { FilterRules = filterOptions });
            rp = FillRP(rp);
            var CumulativeTable = DbPCR.RP_PartnerReports(
                     rp.EndDate,
                     Guid.Parse("00000000-0000-0000-0000-000000000002"),
                     string.Join(",", rp.OrganizationInstanceGUID),
                     string.Join(",", rp.DutyStationGUID), "EN").ToList();

            var MonthlyTable = DbPCR.RP_PartnerReports(
                rp.EndDate,
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                string.Join(",", rp.OrganizationInstanceGUID),
                string.Join(",", rp.DutyStationGUID), "EN").AsQueryable().Where(Predicate).ToList();

            var RefferalIssus = DbPCR.RP_PartnerReports(
               rp.EndDate,
               Guid.Parse("00000000-0000-0000-0000-000000000004"),
               string.Join(",", rp.OrganizationInstanceGUID),
               string.Join(",", rp.DutyStationGUID), "EN").AsQueryable().Where(Predicate).ToList();
            if (rp.ReportGUID == Reports.Monthly_Update)
            {
                string sourceFile = Server.MapPath("~/Areas/PCR/Templates/MonthlyUpdate.xlsm");

                string DisFolder = Server.MapPath("~/Areas/PCR/temp/MonthlyUpdate" + DateTime.Now.ToBinary() + ".xlsm");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[2];
                    ExcelWorksheet reportSheet = package.Workbook.Worksheets[1];


                    string partners = string.Join(" , ", (from a in DbPCR.codeOrganizationsInstances
                                                          join b in DbPCR.codeOrganizations on a.OrganizationGUID equals b.OrganizationGUID
                                                          where rp.OrganizationInstanceGUID.Contains(a.OrganizationInstanceGUID)
                                                          select b.OrganizationShortName).ToList());
                    string governorates = string.Join(" , ", (from a in DbPCR.codeDutyStationsLanguages.Where(x => x.Active && rp.DutyStationGUID.Contains(x.DutyStationGUID)
                                                              && x.LanguageID == "EN")
                                                              select a.DutyStationDescription).ToList());



                    workSheet.Cells["R5"].Value = rp.EndDate.Value.ToString("MMM yyyy");
                    workSheet.Cells["U3"].Value = partners;
                    workSheet.Cells["T5"].Value = governorates;

                    var reports = DbPCR.codeCategoryReport.Where(x => x.ReportGUID.ToString() == "00000000-0000-0000-0000-000000000003").ToList();
                    //rp.CategoryGUID3 = Guid.Parse("00000000-0000-0000-0000-000000000039");
                    //query will always return above result
                    rp.CategoryGUID3 = DropDownList.CategoryPartnerReportLevel2ForMonthlyUpdatesReport(Reports.Monthly_Table).Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray();

                    //rp.CategoryGUID3 = DropDownList.ParentCategoryPartnerReport(string.Join(",", rp.CategoryGUID2)).Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray();

                    //profile totals
                    foreach (var rep in reports.Where(x => x.CategoryLevel == 1).ToList())
                    {
                        var GUIDS = rep.codeCategoryAggregation.Where(x => x.CategoryReportGUID == rep.CategoryReportGUID).Select(y => y.AggregationGUID).ToList();
                        int val = MonthlyTable.Where(x => rp.CategoryGUID3.Contains(x.CategoryReportGUID) && GUIDS.Contains(x.AggregationGUID)).Sum(x => x.AggregationValue);
                        workSheet.Cells[rep.ColumnCharacter + rep.RowSequance].Value = val;
                    }
                    //Vunlerability Totals
                    var vulner = reports.Where(x => x.CategoryLevel == 2).ToList();
                    foreach (var rep in vulner)
                    {
                        var GUIDS = DbPCR.codeCategoryReport.Where(x => x.ParentCategoryReportGUID == rep.ParentCategoryReportGUID).Select(x => x.CategoryReportGUID).ToList();
                        int val = MonthlyTable.Where(x => GUIDS.Contains(x.CategoryReportGUID) && x.AggregationGUID.ToString() == "00000000-0000-0000-0000-000000000002").Sum(x => x.AggregationValue);
                        workSheet.Cells[rep.ColumnCharacter + rep.RowSequance].Value = val;
                    }
                    //Cumulative Table Totals
                    foreach (var rep in reports.Where(x => x.CategoryLevel == 3).ToList())
                    {
                        var GUIDS = DbPCR.codeCategoryReport.Where(x => x.CategoryReportGUID == rep.ParentCategoryReportGUID).Select(x => x.CategoryReportGUID).ToList();
                        int val = CumulativeTable.Where(x => GUIDS.Contains(x.CategoryReportGUID) && x.AggregationGUID.ToString() == "00000000-0000-0000-0000-000000000002").Sum(x => x.AggregationValue);
                        workSheet.Cells[rep.ColumnCharacter + rep.RowSequance].Value = val;
                    }
                    //Cumulative Table Age Totals
                    foreach (var rep in reports.Where(x => x.CategoryLevel == 4).ToList())
                    {
                        int index = 1;
                        double Total_Assisted_In_Category = 0;
                        foreach (var ca in rep.codeCategoryAggregation.OrderBy(x => x.RowSequance).Select(y => y).ToList())
                        {

                            Total_Assisted_In_Category = CumulativeTable
                                .Where(x => x.ParentCategoryReportGUID == rep.ParentCategoryReportGUID && x.Category3.StartsWith("Assisted")
                                &&
                                (x.AggregationGUID == Ages.Age_18_Male
                                || x.AggregationGUID == Ages.Age_18_Female
                                || x.AggregationGUID == Ages.Age_18_59_Male
                                || x.AggregationGUID == Ages.Age_18_59_Female
                                || x.AggregationGUID == Ages.Age_60_Male
                                || x.AggregationGUID == Ages.Age_60_Female
                                ))
                                .Sum(x => x.AggregationValue);

                            double val = CumulativeTable.Where(x => x.AggregationGUID == ca.AggregationGUID && x.ParentCategoryReportGUID == rep.ParentCategoryReportGUID && x.Category3.StartsWith("Assisted")).Sum(x => x.AggregationValue) / Total_Assisted_In_Category;
                            double cellValue = Math.Round(val, 4);

                            if (ca.ColumnCharacter == "Y")
                            {
                                workSheet.Cells[ca.ColumnCharacter + ca.RowSequance].Value = cellValue * -1;
                            }
                            else
                            {
                                workSheet.Cells[ca.ColumnCharacter + ca.RowSequance].Value = cellValue;
                            }
                            index++;
                        }
                    }
                    //Cumulative Table Profile Totals
                    foreach (var rep in reports.Where(x => x.CategoryLevel == 5).ToList())
                    {
                        int index = 1;
                        double Total_Male_Female = 0;
                        var Agg = rep.codeCategoryAggregation.OrderBy(x => x.ColumnCharacter).Select(y => y.AggregationGUID).ToList();
                        double Total_Assisted_In_Category = CumulativeTable.Where(x => Agg.Contains(x.AggregationGUID) && x.ParentCategoryReportGUID == rep.ParentCategoryReportGUID && x.Category3.StartsWith("Assisted")).Sum(x => x.AggregationValue);
                        foreach (var ca in rep.codeCategoryAggregation.OrderBy(x => x.ColumnCharacter).Select(y => y).ToList())
                        {
                            if (index == 1)
                            {
                                Total_Male_Female =
                                    CumulativeTable.Where(x => x.AggregationGUID == Profiles.IDP_Male && x.ParentCategoryReportGUID == rep.ParentCategoryReportGUID && x.Category3.StartsWith("Assisted")).Sum(x => x.AggregationValue)
                                    + CumulativeTable.Where(x => x.AggregationGUID == Profiles.IDP_Female && x.ParentCategoryReportGUID == rep.ParentCategoryReportGUID && x.Category3.StartsWith("Assisted")).Sum(x => x.AggregationValue);
                            }
                            else if (index == 3) { Total_Male_Female = CumulativeTable.Where(x => x.AggregationGUID == Profiles.Host_Community_Male && x.ParentCategoryReportGUID == rep.ParentCategoryReportGUID && x.Category3.StartsWith("Assisted")).Sum(x => x.AggregationValue) + CumulativeTable.Where(x => x.AggregationGUID == Profiles.Host_Community_Female && x.ParentCategoryReportGUID == rep.ParentCategoryReportGUID && x.Category3.StartsWith("Assisted")).Sum(x => x.AggregationValue); }
                            else if (index == 5) { Total_Male_Female = CumulativeTable.Where(x => x.AggregationGUID == Profiles.Refugee_asylum_seeker_Male && x.ParentCategoryReportGUID == rep.ParentCategoryReportGUID && x.Category3.StartsWith("Assisted")).Sum(x => x.AggregationValue) + CumulativeTable.Where(x => x.AggregationGUID == Profiles.Refugee_asylum_seeker_Female && x.ParentCategoryReportGUID == rep.ParentCategoryReportGUID && x.Category3.StartsWith("Assisted")).Sum(x => x.AggregationValue); }
                            else if (index == 7) { Total_Male_Female = CumulativeTable.Where(x => x.AggregationGUID == Profiles.Refugee_returnee_Male && x.ParentCategoryReportGUID == rep.ParentCategoryReportGUID && x.Category3.StartsWith("Assisted")).Sum(x => x.AggregationValue) + CumulativeTable.Where(x => x.AggregationGUID == Profiles.Refugee_returnee_Female && x.ParentCategoryReportGUID == rep.ParentCategoryReportGUID && x.Category3.StartsWith("Assisted")).Sum(x => x.AggregationValue); }
                            else if (index == 9) { Total_Male_Female = CumulativeTable.Where(x => x.AggregationGUID == Profiles.IDP_returnee_Male && x.ParentCategoryReportGUID == rep.ParentCategoryReportGUID && x.Category3.StartsWith("Assisted")).Sum(x => x.AggregationValue) + CumulativeTable.Where(x => x.AggregationGUID == Profiles.IDP_returnee_Female && x.ParentCategoryReportGUID == rep.ParentCategoryReportGUID && x.Category3.StartsWith("Assisted")).Sum(x => x.AggregationValue); }

                            double val = Total_Male_Female / Total_Assisted_In_Category;
                            workSheet.Cells[ca.ColumnCharacter + ca.RowSequance].Value = Math.Round(val, 4);
                            index++;
                        }
                    }
                    //Governarete monthy table totle
                    foreach (var rep in reports.Where(x => x.CategoryLevel == 6).ToList())
                    {
                        int val = MonthlyTable.Where(x => rp.CategoryGUID3.Contains(x.CategoryReportGUID) && x.DutyStationDescription.Contains(rep.CategoryDescription) && x.AggregationGUID.ToString() == "00000000-0000-0000-0000-000000000002").Sum(x => x.AggregationValue);
                        workSheet.Cells[rep.ColumnCharacter + rep.RowSequance].Value = val;
                    }
                    //totals
                    var totals = reports.Where(x => x.CategoryLevel == 7).ToList();
                    var asdasd = (from a in MonthlyTable
                                  where a.AggregationDescription == "Total persons referred to external service providers"
                                  select a).ToList();
                    foreach (var rep in totals)
                    {
                        int val = MonthlyTable.Where(x => x.CategoryReportGUID == rep.ParentCategoryReportGUID && x.AggregationGUID.ToString() == "00000000-0000-0000-0000-000000000002").Sum(x => x.AggregationValue);
                        if (val == 0) { val = CumulativeTable.Where(x => x.CategoryReportGUID == rep.ParentCategoryReportGUID && x.AggregationGUID.ToString() == "00000000-0000-0000-0000-000000000002").Sum(x => x.AggregationValue); }
                        if (rep.CategoryDescription == "Total persons referred to external service providers")
                        {
                            val = RefferalIssus.Where(x => x.CategoryReportGUID.ToString() == "00000000-0000-0000-0000-000000000229" && x.AggregationGUID.ToString() == "00000000-0000-0000-0000-000000000028").Sum(x => x.AggregationValue);
                        }
                        workSheet.Cells[rep.ColumnCharacter + rep.RowSequance].Value = val;
                    }
                    //
                    List<Guid> channelsGUIDs = new List<Guid>();
                    channelsGUIDs.Add(Guid.Parse("00000000-0000-0000-0000-000000000222"));
                    channelsGUIDs.Add(Guid.Parse("00000000-0000-0000-0000-000000000223"));
                    channelsGUIDs.Add(Guid.Parse("00000000-0000-0000-0000-000000000224"));
                    channelsGUIDs.Add(Guid.Parse("00000000-0000-0000-0000-000000000225"));
                    channelsGUIDs.Add(Guid.Parse("00000000-0000-0000-0000-000000000226"));
                    channelsGUIDs.Add(Guid.Parse("00000000-0000-0000-0000-000000000227"));
                    //var Guids8 = reports.Where(x => x.CategoryLevel == 8).Select(x => x.ParentCategoryReportGUID.Value).ToList(); ????? WTF THE VALUES COMING FROM ???????????????????????????? WTF WTF
                    double valT8 = MonthlyTable.Where(x => channelsGUIDs.Contains(x.CategoryReportGUID)).Sum(x => x.AggregationValue);
                    foreach (var rep in reports.Where(x => x.CategoryLevel == 8).ToList())
                    {
                        if (rep.CategoryDescription == "Others")
                        {
                            List<Guid> _othersGUID = new List<Guid>(); //WTFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
                            _othersGUID.Add(Guid.Parse("00000000-0000-0000-0000-000000000225"));
                            _othersGUID.Add(Guid.Parse("00000000-0000-0000-0000-000000000226"));
                            _othersGUID.Add(Guid.Parse("00000000-0000-0000-0000-000000000227"));
                            double val = Convert.ToDouble(MonthlyTable.Where(x => _othersGUID.Contains(x.CategoryReportGUID)).Sum(x => x.AggregationValue)) / valT8;
                            workSheet.Cells[rep.ColumnCharacter + rep.RowSequance].Value = val;
                        }
                        else
                        {
                            double val = Convert.ToDouble(MonthlyTable.Where(x => x.CategoryReportGUID == rep.ParentCategoryReportGUID).Sum(x => x.AggregationValue)) / valT8;
                            workSheet.Cells[rep.ColumnCharacter + rep.RowSequance].Value = val;
                        }

                    }
                    //
                    var Guids9_1 = (from a in reports.Where(x => x.CategoryLevel == 9 && x.RowSequance == 9)
                                    join b in DbPCR.codeCategoryAggregation on a.CategoryReportGUID equals b.CategoryReportGUID
                                    select b.AggregationGUID).ToList();
                    //
                    var Guids9_2 = (from a in reports.Where(x => x.CategoryLevel == 9 && x.RowSequance == 11)
                                    join b in DbPCR.codeCategoryAggregation on a.CategoryReportGUID equals b.CategoryReportGUID
                                    select b.AggregationGUID).ToList();
                    //
                    var Guids9_3 = (from a in reports.Where(x => x.CategoryLevel == 9 && x.RowSequance == 13)
                                    join b in DbPCR.codeCategoryAggregation on a.CategoryReportGUID equals b.CategoryReportGUID
                                    select b.AggregationGUID).ToList();
                    int TotalRegisteredIndividuals = MonthlyTable.Where(x => x.CategoryReportGUID.ToString() == "00000000-0000-0000-0000-000000000039" && x.AggregationGUID.ToString() == "00000000-0000-0000-0000-000000000002").Sum(x => x.AggregationValue);

                    foreach (var rep in reports.Where(x => x.CategoryLevel == 9).ToList())
                    {
                        var catage = DbPCR.codeCategoryAggregation.Where(x => x.CategoryReportGUID == rep.CategoryReportGUID).FirstOrDefault();
                        double val = MonthlyTable.Where(x => x.CategoryReportGUID == rep.ParentCategoryReportGUID && x.AggregationGUID == catage.AggregationGUID).Sum(x => x.AggregationValue);
                        if (rep.RowSequance == 9)
                        {
                            double valT9 = Convert.ToDouble(MonthlyTable.Where(x => x.CategoryReportGUID == rep.ParentCategoryReportGUID && Guids9_1.Contains(x.AggregationGUID)).Sum(x => x.AggregationValue));
                            val = val / TotalRegisteredIndividuals;
                        }
                        if (rep.RowSequance == 11)
                        {
                            double valT9 = Convert.ToDouble(MonthlyTable.Where(x => x.CategoryReportGUID == rep.ParentCategoryReportGUID && Guids9_2.Contains(x.AggregationGUID)).Sum(x => x.AggregationValue));
                            val = val / TotalRegisteredIndividuals;
                        }
                        if (rep.RowSequance == 13)
                        {
                            double valT9 = Convert.ToDouble(MonthlyTable.Where(x => x.CategoryReportGUID == rep.ParentCategoryReportGUID && Guids9_3.Contains(x.AggregationGUID)).Sum(x => x.AggregationValue));
                            val = val / TotalRegisteredIndividuals;
                        }
                        if (rep.ColumnCharacter == "Z")
                        {
                            workSheet.Cells[rep.ColumnCharacter + rep.RowSequance].Value = val * -1;
                        }
                        else
                        {
                            workSheet.Cells[rep.ColumnCharacter + rep.RowSequance].Value = val;
                        }

                    }

                    ///////////////Section 10//////////////////.
                    ///
                    //x30 Total services provided  ---> white lines
                    var Total_Services_Provided = CumulativeTable
                        .Where(x => x.AggregationGUID.ToString() == Aggregation.Individuals
                        && x.CategoryReportGUID.ToString() != "00000000-0000-0000-0000-000000000218"
                        && x.CategoryReportGUID.ToString() != "00000000-0000-0000-0000-000000000219"
                        && x.Category3.StartsWith("Assisted"))
                       .Select(x => x.AggregationValue).Sum();
                    workSheet.Cells["X30"].Value = Total_Services_Provided;

                    //x31 Individuals assisted with more  than 1 servcie ---> E70 

                    //var test = CumulativeTable.Where(x => x.CategoryReportGUID.ToString() == "00000000-0000-0000-0000-000000000219").ToList();
                    var Indv_assisted_with_more_than_1_service = CumulativeTable.Where(x => x.AggregationGUID.ToString() == Aggregation.Individuals && x.CategoryReportGUID.ToString() == "00000000-0000-0000-0000-000000000219")
                       .Select(x => x.AggregationValue).Sum();
                    workSheet.Cells["X31"].Value = Indv_assisted_with_more_than_1_service;
                    /// 
                    /// 
                    /// 
                    /// 
                    /// 
                    /// 
                    /// 
                    /// 
                    /// 
                    /// 
                    /// 
                    /// 
                    /// 
                    /// 
                    ///////////////Section 10//////////////////
                    ///
                    #region CC Centers Count On Map

                    var partnerCentersCount = (from a in DbPCR.codePartnerCenter.Where(x => x.Active)
                                               join b in DbPCR.codeDutyStationsLanguages.Where(x => x.Active && x.LanguageID == "EN") on a.DutyStationGUID equals b.DutyStationGUID
                                               where rp.DutyStationGUID.Contains(a.DutyStationGUID)
                                               && rp.OrganizationInstanceGUID.Contains(a.OrganizationInstanceGUID)
                                               group a by new { b.DutyStationDescription, a.OrganizationInstanceGUID } into g
                                               select new
                                               {
                                                   DutyStationDescription = g.Key.DutyStationDescription,
                                                   OrganizationInstanceGUID = g.Key.OrganizationInstanceGUID,
                                                   CommunityCentersCount = g.Count()
                                               }).ToList();


                    int DamascusAndRuralCCCount = 0;
                    int AleppoCCCount = 0;
                    int DaraaCCCount = 0;
                    int DeirZourCCCount = 0;
                    int QuneitraCCCount = 0;
                    int SweidaCCCount = 0;
                    int HamaCCCount = 0;
                    int AlHasakahCCCount = 0;
                    int QamishliCCCount = 0;
                    int RaqqaCCCount = 0;
                    int IdlibCCCount = 0;
                    int LattakiaCCCount = 0;
                    int TartousCCCount = 0;
                    int HomsCCCount = 0;

                    foreach (var item in partnerCentersCount)
                    {
                        if (item.DutyStationDescription == "Damascus")
                        {
                            //AF16
                            DamascusAndRuralCCCount += item.CommunityCentersCount;

                        }
                        if (item.DutyStationDescription == "Rural Damascus")
                        {
                            DamascusAndRuralCCCount += item.CommunityCentersCount;
                        }
                        if (item.DutyStationDescription == "Aleppo")
                        {
                            //AF14
                            AleppoCCCount += item.CommunityCentersCount;

                        }
                        if (item.DutyStationDescription == "Daraa")
                        { //AF18
                            DaraaCCCount += item.CommunityCentersCount;
                        }
                        if (item.DutyStationDescription == "Deir es Zour")
                        { //AF10 
                            DeirZourCCCount += item.CommunityCentersCount;
                        }
                        if (item.DutyStationDescription == "Quneitra")
                        { //AF19
                            QuneitraCCCount += item.CommunityCentersCount;
                        }
                        if (item.DutyStationDescription == "Sweida")
                        { //AF17
                            SweidaCCCount += item.CommunityCentersCount;
                        }
                        if (item.DutyStationDescription == "Hama")
                        { //AF13
                            HamaCCCount += item.CommunityCentersCount;
                        }
                        if (item.DutyStationDescription == "Al Hasakah")
                        { //AF9
                            AlHasakahCCCount += item.CommunityCentersCount;
                        }
                        if (item.DutyStationDescription == "Qamishli")
                        {
                            QamishliCCCount += item.CommunityCentersCount;
                        }
                        if (item.DutyStationDescription == "Raqqa")
                        { //AF11
                            RaqqaCCCount += item.CommunityCentersCount;
                        }
                        if (item.DutyStationDescription == "Idlib")
                        { //AF15
                            IdlibCCCount += item.CommunityCentersCount;
                        }
                        if (item.DutyStationDescription == "Lattakia")
                        { //AF 21
                            LattakiaCCCount += item.CommunityCentersCount;
                        }
                        if (item.DutyStationDescription == "Tartous")
                        { //AF20 
                            TartousCCCount += item.CommunityCentersCount;
                        }
                        if (item.DutyStationDescription == "Homs")
                        { //AF12
                            HomsCCCount += item.CommunityCentersCount;
                        }

                    }

                    workSheet.Cells["AF16"].Value = DamascusAndRuralCCCount;
                    workSheet.Cells["AF14"].Value = AleppoCCCount;
                    workSheet.Cells["AF18"].Value = DaraaCCCount;
                    workSheet.Cells["AF10"].Value = DeirZourCCCount;
                    workSheet.Cells["AF19"].Value = QuneitraCCCount;
                    workSheet.Cells["AF17"].Value = SweidaCCCount;
                    workSheet.Cells["AF13"].Value = HamaCCCount;
                    workSheet.Cells["AF9"].Value = AlHasakahCCCount;
                    workSheet.Cells["AF11"].Value = RaqqaCCCount;
                    workSheet.Cells["AF15"].Value = IdlibCCCount;
                    workSheet.Cells["AF21"].Value = LattakiaCCCount;
                    workSheet.Cells["AF20"].Value = TartousCCCount;
                    workSheet.Cells["AF12"].Value = HomsCCCount;
                    #endregion




                    if (!CMS.HasAction(Permissions.UploadPartnerReport.Access, Apps.PCR))
                    {
                        workSheet.Protection.IsProtected = true;
                        workSheet.Protection.AllowAutoFilter = false;
                        workSheet.Protection.AllowDeleteColumns = false;
                        workSheet.Protection.AllowDeleteRows = false;
                        workSheet.Protection.AllowEditObject = false;
                        workSheet.Protection.AllowEditScenarios = false;
                        workSheet.Protection.AllowFormatCells = false;
                        workSheet.Protection.AllowFormatColumns = false;
                        workSheet.Protection.AllowFormatRows = false;
                        workSheet.Protection.AllowInsertColumns = false;
                        workSheet.Protection.AllowInsertHyperlinks = false;
                        workSheet.Protection.AllowInsertRows = false;
                        workSheet.Protection.AllowPivotTables = false;
                        workSheet.Protection.AllowSelectLockedCells = false;
                        workSheet.Protection.AllowSelectUnlockedCells = false;
                        workSheet.Protection.AllowSort = true;
                        workSheet.Protection.SetPassword("7B@g{}F5CHjNpG*z");
                        workSheet.Hidden = eWorkSheetHidden.Hidden;

                        reportSheet.Protection.IsProtected = true;
                        reportSheet.Protection.AllowAutoFilter = false;
                        reportSheet.Protection.AllowDeleteColumns = false;
                        reportSheet.Protection.AllowDeleteRows = false;
                        reportSheet.Protection.AllowEditObject = false;
                        reportSheet.Protection.AllowEditScenarios = false;
                        reportSheet.Protection.AllowFormatCells = false;
                        reportSheet.Protection.AllowFormatColumns = false;
                        reportSheet.Protection.AllowFormatRows = false;
                        reportSheet.Protection.AllowInsertColumns = false;
                        reportSheet.Protection.AllowInsertHyperlinks = false;
                        reportSheet.Protection.AllowInsertRows = false;
                        reportSheet.Protection.AllowPivotTables = false;
                        reportSheet.Protection.AllowSelectLockedCells = false;
                        reportSheet.Protection.AllowSelectUnlockedCells = false;
                        reportSheet.Protection.AllowSort = true;
                        reportSheet.Protection.SetPassword("7B@g{}F5CHjNpG*z");
                    }

                    //X509Certificate2 certificate = new X509Certificate2(certPath, password);

                    //byte[] signature;

                    //using (RSA rsa = certificate.GetRSAPrivateKey())
                    //{
                    //    signature = rsa.SignData(data, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
                    //}

                    package.Save();



                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);
                string fileName = "MonthlyUpdate " + DateTime.Now.ToString("dddd, dd MMMM yyyy") + ".xlsm";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

                //if (CMS.HasAction(Permissions.UploadPartnerReport.Access, Apps.PCR))
                //{
                //    byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);
                //    string fileName = "MonthlyUpdate " + DateTime.Now.ToString("dddd, dd MMMM yyyy") + ".xlsm";
                //    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                //}
                //else
                //{
                //    //Generate as PDF


                //}



            }
            if (rp.ReportGUID == Reports.Cumulative_Table)
            {
                string sourceFile = Server.MapPath("~/Areas/PCR/Templates/CumulativeTable.xlsx");
                string DisFolder = Server.MapPath("~/Areas/PCR/temp/CumulativeTable" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                    var CategoryReport = DbPCR.codeCategoryReport.Where(x => x.CategoryLevel == 3 && x.ReportGUID == rp.ReportGUID).ToList();
                    var Aggregations = DbPCR.codeAggregation.ToList();

                    foreach (var cat in CategoryReport)
                    {
                        foreach (var agg in Aggregations)
                        {
                            workSheet.Cells[agg.ColumnCharacter + cat.RowSequance].Value = CumulativeTable.Where(x => x.AggregationGUID == agg.AggregationGUID && x.CategoryReportGUID == cat.CategoryReportGUID).Sum(x => x.AggregationValue);
                        }

                    }
                    package.Save();
                    byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);
                    string fileName = "CumulativeTable " + DateTime.Now.ToString("dddd, dd MMMM yyyy") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
            }
            if (rp.ReportGUID == Reports.Monthly_Table)
            {
                string sourceFile = Server.MapPath("~/Areas/PCR/Templates/MonthlyTable.xlsx");
                string DisFolder = Server.MapPath("~/Areas/PCR/temp/MonthlyTable" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                    var CategoryReport = DbPCR.codeCategoryReport.Where(x => x.CategoryLevel == 3 && x.ReportGUID == rp.ReportGUID).ToList();
                    var Aggregations = DbPCR.codeAggregation.ToList();

                    foreach (var cat in CategoryReport)
                    {
                        foreach (var agg in Aggregations)
                        {
                            workSheet.Cells[agg.ColumnCharacter + cat.RowSequance].Value = MonthlyTable.Where(x => x.AggregationGUID == agg.AggregationGUID && x.CategoryReportGUID == cat.CategoryReportGUID).Sum(x => x.AggregationValue);
                        }
                    }


                    package.Save();

                    byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);
                    string fileName = "MonthlyTable " + DateTime.Now.ToString("dddd, dd MMMM yyyy") + ".xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
            }
            return null;
        }

        public class Ages
        {
            public static Guid Age_18_Male = Guid.Parse("00000000-0000-0000-0000-000000000005");
            public static Guid Age_18_Female = Guid.Parse("00000000-0000-0000-0000-000000000008");
            public static Guid Age_18_59_Male = Guid.Parse("00000000-0000-0000-0000-000000000006");
            public static Guid Age_18_59_Female = Guid.Parse("00000000-0000-0000-0000-000000000009");
            public static Guid Age_60_Male = Guid.Parse("00000000-0000-0000-0000-000000000007");
            public static Guid Age_60_Female = Guid.Parse("00000000-0000-0000-0000-000000000010");
        }
        public class Profiles
        {
            public static Guid Host_Community_Female = Guid.Parse("00000000-0000-0000-0000-000000000017");
            public static Guid Host_Community_Male = Guid.Parse("00000000-0000-0000-0000-000000000012");
            public static Guid IDP_Female = Guid.Parse("00000000-0000-0000-0000-000000000016");
            public static Guid IDP_Male = Guid.Parse("00000000-0000-0000-0000-000000000011");
            public static Guid IDP_returnee_Female = Guid.Parse("00000000-0000-0000-0000-000000000018");
            public static Guid IDP_returnee_Male = Guid.Parse("00000000-0000-0000-0000-000000000013");
            public static Guid Refugee_returnee_Female = Guid.Parse("00000000-0000-0000-0000-000000000019");
            public static Guid Refugee_returnee_Male = Guid.Parse("00000000-0000-0000-0000-000000000014");
            public static Guid Refugee_asylum_seeker_Female = Guid.Parse("00000000-0000-0000-0000-000000000020");
            public static Guid Refugee_asylum_seeker_Male = Guid.Parse("00000000-0000-0000-0000-000000000015");
        }

        public class Reports
        {
            public static Guid Cumulative_Table = Guid.Parse("00000000-0000-0000-0000-000000000002");
            public static Guid Monthly_Table = Guid.Parse("00000000-0000-0000-0000-000000000001");
            public static Guid Monthly_Update = Guid.Parse("00000000-0000-0000-0000-000000000003");
        }
        #endregion

        #region Partner Report
        [HttpGet]
        public ActionResult PartnerCenter()
        {
            ReportViewer reportViewer = new ReportViewer();
            ViewBag.ReportViewer = reportViewer;
            return View("~/Areas/PCR/Views/Reports/PartnerCenter.cshtml", new PCRReportParametersMultiple());
        }
        [HttpPost]
        public ActionResult PartnerCenter(PCRReportParametersMultiple rp)
        {
            string URL = "LoadUrl('ReportViewer','PartnerCenterLoad?ReportName=" + rp.ReportName +
                "&EndDate=" + rp.EndDate.Value.Ticks +
                "&ReportGUID=" + rp.ReportGUID +
                "&AggregationGUID=" + string.Join(",", rp.AggregationGUID) +
                "&PartnerCenterGUID=" + string.Join(",", rp.PartnerCenterGUID) +
                "&OrganizationInstanceGUID=" + string.Join(",", rp.OrganizationInstanceGUID) +
                "&CategoryGUID2=" + string.Join(",", rp.CategoryGUID2) +
                "&CategoryGUID3=" + string.Join(",", rp.CategoryGUID3) + "')";
            return Json(DbCMS.SingleUpdateMessage(null, null, null, URL, "Please Wait...."));
        }
        public ActionResult PartnerCenterLoad(string ReportName, long? EndDate, Guid ReportGUID, string PartnerCenterGUID, string OrganizationInstanceGUID, string CategoryGUID2, string CategoryGUID3, string AggregationGUID)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.ZoomMode = ZoomMode.Percent;
            reportViewer.ZoomPercent = 100;
            reportViewer.Width = System.Web.UI.WebControls.Unit.Pixel(900);
            reportViewer.Height = System.Web.UI.WebControls.Unit.Pixel(700);

            reportViewer.AsyncRendering = true;
            reportViewer.LocalReport.DataSources.Clear();
            RP_PartnerCenterTableAdapter PartnerCenter = new RP_PartnerCenterTableAdapter();
            PCRDataSet dataSet = new PCRDataSet();
            PartnerCenter.Fill(dataSet.RP_PartnerCenter,
                new DateTime(EndDate.Value),
                ReportGUID,
                 OrganizationInstanceGUID,
                 PartnerCenterGUID,
                 CategoryGUID2,
                 CategoryGUID3,
                 AggregationGUID,
                LAN);
            ReportDataSource reportDataSource = new ReportDataSource("PartnerCenter", dataSet.Tables["RP_PartnerCenter"]);
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Areas/PCR/Rdlc\" + ReportName + ".rdlc";
            ViewBag.ReportViewer = reportViewer;
            return PartialView("~/Areas/PCR/Views/Reports/_Report.cshtml");
        }
        #endregion

        #region Charts
        [HttpGet]
        public ActionResult Chart()
        {
            ReportViewer reportViewer = new ReportViewer();
            ViewBag.ReportViewer = reportViewer;
            return View("~/Areas/PCR/Views/Reports/Charts.cshtml", new PCRReportParametersMultiple());
        }
        class HieghChartReport
        {
            public List<series> MainReport { get; set; }
            public List<drilldown> DetailsReport { get; set; }
        }
        class series
        {
            public string name { set; get; }
            public bool colorByPoint { get; set; }
            public data data { get; set; }
        }

        class drilldown
        {
            public string name { set; get; }
            public string id { set; get; }
            public bool colorByPoint { get; set; }
            public List<data> data { get; set; }
        }
        class data
        {
            public string name { set; get; }
            public string drilldown { set; get; }
            public double y { set; get; }
            public bool selected { get; set; }
            public bool sliced { get; set; }
        }
        [HttpPost]
        public ActionResult chart(PCRReportParametersMultiple rp)
        {


            var MainData = DbPCR.RP_PartnerCenter(
                rp.EndDate,
                rp.ReportGUID,
                string.Join(",", rp.OrganizationInstanceGUID),
                string.Join(",", rp.PartnerCenterGUID),
                string.Join(",", rp.CategoryGUID2),
                string.Join(",", rp.CategoryGUID3),
                string.Join(",", Aggregation.Individuals),
                LAN).Select(x => new
                {
                    x.Category3,
                    x.AggregationValue
                }).GroupBy(x => x.Category3).Select(x => new series
                {
                    name = x.Key,
                    data = new data
                    {
                        name = x.Key,
                        drilldown = x.Key,
                        y = x.Sum(y => y.AggregationValue),
                        selected = true,
                        sliced = true
                    }
                }).ToList();

            var DetailsData = DbPCR.RP_PartnerCenter(
                rp.EndDate,
                rp.ReportGUID,
                string.Join(",", rp.OrganizationInstanceGUID),
                string.Join(",", rp.PartnerCenterGUID),
                string.Join(",", rp.CategoryGUID2), string.Join(",", rp.CategoryGUID3),
                string.Join(",", rp.AggregationGUID), LAN).Select(x => new
                {
                    x.AggregationDescription,
                    x.Category3,
                    x.AggregationValue
                }).ToList();
            List<drilldown> details = new List<drilldown>();
            foreach (var cat in MainData)
            {
                details.Add(new drilldown
                {
                    id = cat.name,
                    name = cat.name,
                    data = DetailsData.Where(x => x.Category3 == cat.name).GroupBy(x => x.AggregationDescription).Select(x =>
                  new data
                  {
                      name = x.Key,
                      y = x.Sum(y => y.AggregationValue),

                  }).ToList()
                });

            }
            return Json(new HieghChartReport { MainReport = MainData, DetailsReport = details }, JsonRequestBehavior.AllowGet);
        }
        public class Aggregation
        {
            public static string Individuals = "00000000-0000-0000-0000-000000000002";
        }


        #endregion
    }
}