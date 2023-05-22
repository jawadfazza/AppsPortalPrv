using AppsPortal.BaseControllers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.FWS.Controllers
{
    public class MasterTableController : FWSBaseController
    {
        // GET: FWS/MasterTable
        public ActionResult Index()
        {
            return View();
        }

        [Route("FWS/MasterTable/GenerateMasterTable")]
        public ActionResult GenerateMasterTable()
        {
            string sourceFile = Server.MapPath("~/Areas/FWS/Templates/Master.xlsx");

            string DisFolder = Server.MapPath("~/Areas/FWS/Templates/Master" + DateTime.Now.ToBinary() + ".xlsx");

            System.IO.File.Copy(sourceFile, DisFolder);

            var MasterTableRecords = (from a in DbFWS.v_MasterTable select a).ToList();

            using (var package = new ExcelPackage(new FileInfo(DisFolder)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                int i = 4;
                foreach (var record in MasterTableRecords)
                {
                    worksheet.Cells["A" + i].Value = record.HubDescription;
                    worksheet.Cells["B" + i].Value = record.Organization;
                    worksheet.Cells["C" + i].Value = record.ImplementingPartner;
                    worksheet.Cells["D" + i].Value = record.Funding_UN;
                    worksheet.Cells["E" + i].Value = record.Admin1RefName;
                    worksheet.Cells["F" + i].Value = record.Admin2RefName;
                    worksheet.Cells["G" + i].Value = record.Admin3RefName;
                    worksheet.Cells["H" + i].Value = record.NeibourhoodRefName;
                    worksheet.Cells["I" + i].Value = record.Camps;
                    worksheet.Cells["J" + i].Value = record.ReportingMonth; ////////////////////
                    worksheet.Cells["K" + i].Value = record.HRPProject;
                    worksheet.Cells["L" + i].Value = record.FWSSubSector;
                    worksheet.Cells["M" + i].Value = record.FWSActivity;
                    worksheet.Cells["N" + i].Value = record.FWSSubActivity;
                    worksheet.Cells["O" + i].Value = record.DeliveryMode;
                    worksheet.Cells["P" + i].Value = record.FWSUnit;
                    worksheet.Cells["Q" + i].Value = record.TotalReach;
                    worksheet.Cells["R" + i].Value = record.BeneficiaryTypeDescription;
                    worksheet.Cells["S" + i].Value = record.PWD;
                    worksheet.Cells["T" + i].Value = record.Instructions;
                    worksheet.Cells["U" + i].Value = record.TotalSumBreakdown;
                    worksheet.Cells["V" + i].Value = record.Girls;
                    worksheet.Cells["W" + i].Value = record.Boys;
                    worksheet.Cells["X" + i].Value = record.Women;
                    worksheet.Cells["Y" + i].Value = record.Men;
                    worksheet.Cells["Z" + i].Value = record.ElderlyWomen;
                    worksheet.Cells["AA" + i].Value = record.ElderlyMen;
                    worksheet.Cells["AB" + i].Value = record.NewTotalSumBreakdownNew;
                    worksheet.Cells["AC" + i].Value = record.GirlsNew;
                    worksheet.Cells["AD" + i].Value = record.BoysNew;
                    worksheet.Cells["AE" + i].Value = record.WomenNew;
                    worksheet.Cells["AF" + i].Value = record.MenNew;
                    worksheet.Cells["AG" + i].Value = record.ElderlyWomenNew;
                    worksheet.Cells["AH" + i].Value = record.ElderlyMenNew;
                    worksheet.Cells["AI" + i].Value = record.Admin1Pcode;
                    worksheet.Cells["AJ" + i].Value = record.Admin2Pcode;
                    worksheet.Cells["AK" + i].Value = record.Admin3Pcode;
                    worksheet.Cells["AL" + i].Value = record.Admin4Pcode2;
                    worksheet.Cells["AM" + i].Value = record.NeibourhoodPcode2;
                    worksheet.Cells["AN" + i].Value = record.Code_Camps;
                    worksheet.Cells["AO" + i].Value = record.SubSectorID;
                    worksheet.Cells["AP" + i].Value = record.ActivityID;
                    worksheet.Cells["AQ" + i].Value = record.SubActivityID;
                    worksheet.Cells["AR" + i].Value = record.FWSAnalysisUnit;
                    worksheet.Cells["AS" + i].Value = record.PWDActivity;
                    worksheet.Cells["AT" + i].Value = record.ImplementingPartner_ALL;
                    worksheet.Cells["AU" + i].Value = record.Organization;
                    worksheet.Cells["AV" + i].Value = record.ImplementingPartner_Type;
                    worksheet.Cells["AW" + i].Value = record.Organization_Type;
                    worksheet.Cells["AX" + i].Value = record.ImplementingPartner_Type;
                    worksheet.Cells["AY" + i].Value = record.Access_Status;
                    worksheet.Cells["AZ" + i].Value = record.Delivery_Modality;
                    worksheet.Cells["BA" + i].Value = record.Severity_Ranking;
                    worksheet.Cells["BB" + i].Value = record.Cross_AoR_Category;
                    worksheet.Cells["BC" + i].Value = record.HRP_Indicator;
                    worksheet.Cells["BD" + i].Value = record.Direct_Addition;
                    worksheet.Cells["BE" + i].Value = record.Total_Cumulative_Interventions;
                    worksheet.Cells["BF" + i].Value = record.Girls_Cumu;
                    worksheet.Cells["BG" + i].Value = record.Boys_Cumu;
                    worksheet.Cells["BH" + i].Value = record.Women_Cumu;
                    worksheet.Cells["BI" + i].Value = record.Men_Cumu;
                    i++;
                }
                package.Save();
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);
            string fileName = "MasterTable " + DateTime.Now.ToString("dddd, dd MMMM yyyy") + ".xlsx";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}