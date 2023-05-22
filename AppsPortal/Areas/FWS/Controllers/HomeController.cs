using AppsPortal.BaseControllers;
using AppsPortal.Library;
using FWS_DAL.Model;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.FWS.Controllers
{
    public class HomeController : FWSBaseController
    {
        // GET: FWS/Home
        public ActionResult Index()
        {
            CMS.SetUserToken(UserProfileGUID, Apps.FWS);
            Session[SessionKeys.CurrentApp] = Apps.FWS;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }
    }
}

#region rubish
//FileInfo locationInfo = new FileInfo(Server.MapPath("~/Areas/FWS/Uploads/Locations.xlsx"));
//List<code4WSLocation> code4WSLocations = new List<code4WSLocation>();

//            using (var package = new ExcelPackage(locationInfo))
//            {
//                ExcelWorksheet workSheet = package.Workbook.Worksheets["Sheet1"];
//int totalRows = workSheet.Dimension.End.Row;
//                for (int i = 2; i <= totalRows; i++)
//                {
//                    code4WSLocation code4WSLocation = new code4WSLocation();
//code4WSLocation.LocationGUID = Guid.NewGuid();

//                    code4WSLocation.Location = workSheet.Cells["B" + i].Value == null ? "" : workSheet.Cells["B" + i].Value.ToString();
//                    code4WSLocation.NeibourhoodPcode2 = workSheet.Cells["C" + i].Value == null ? "" : workSheet.Cells["C" + i].Value.ToString();
//                    code4WSLocation.NeibourhoodRefName = workSheet.Cells["D" + i].Value == null ? "" : workSheet.Cells["D" + i].Value.ToString();
//                    code4WSLocation.neibourhoodName_en = workSheet.Cells["E" + i].Value == null ? "" : workSheet.Cells["E" + i].Value.ToString();
//                    code4WSLocation.neibourhoodName_ar = workSheet.Cells["F" + i].Value == null ? "" : workSheet.Cells["F" + i].Value.ToString();
//                    code4WSLocation.admin4Pcode2 = workSheet.Cells["G" + i].Value == null ? "" : workSheet.Cells["G" + i].Value.ToString();
//                    code4WSLocation.Admin4RefName = workSheet.Cells["H" + i].Value == null ? "" : workSheet.Cells["H" + i].Value.ToString();
//                    code4WSLocation.admin4Name_en = workSheet.Cells["I" + i].Value == null ? "" : workSheet.Cells["I" + i].Value.ToString();
//                    code4WSLocation.admin4Name_ar = workSheet.Cells["J" + i].Value == null ? "" : workSheet.Cells["J" + i].Value.ToString();
//                    //string a = workSheet.Cells["K" + i].Value.ToString();
//                    code4WSLocation.admin3Pcode = workSheet.Cells["L" + i].Value == null ? "" : workSheet.Cells["L" + i].Value.ToString();
//                    code4WSLocation.Admin3RefName = workSheet.Cells["M" + i].Value == null ? "" : workSheet.Cells["M" + i].Value.ToString();
//                    code4WSLocation.admin3Name_en = workSheet.Cells["N" + i].Value == null ? "" : workSheet.Cells["N" + i].Value.ToString();
//                    code4WSLocation.admin3Name_ar = workSheet.Cells["O" + i].Value == null ? "" : workSheet.Cells["O" + i].Value.ToString();
//                    code4WSLocation.admin2Pcode = workSheet.Cells["P" + i].Value == null ? "" : workSheet.Cells["P" + i].Value.ToString();
//                    code4WSLocation.Admin2RefName = workSheet.Cells["Q" + i].Value == null ? "" : workSheet.Cells["Q" + i].Value.ToString();
//                    code4WSLocation.admin2Name_en = workSheet.Cells["R" + i].Value == null ? "" : workSheet.Cells["R" + i].Value.ToString();
//                    code4WSLocation.admin2Name_ar = workSheet.Cells["S" + i].Value == null ? "" : workSheet.Cells["S" + i].Value.ToString();
//                    code4WSLocation.admin1Pcode = workSheet.Cells["T" + i].Value == null ? "" : workSheet.Cells["T" + i].Value.ToString();
//                    code4WSLocation.Admin1RefName = workSheet.Cells["U" + i].Value == null ? "" : workSheet.Cells["U" + i].Value.ToString();
//                    code4WSLocation.admin1Name_en = workSheet.Cells["V" + i].Value == null ? "" : workSheet.Cells["V" + i].Value.ToString();
//                    code4WSLocation.admin1Name_ar = workSheet.Cells["W" + i].Value == null ? "" : workSheet.Cells["W" + i].Value.ToString();
//                    code4WSLocation.Latitude = Convert.ToDouble(workSheet.Cells["X" + i].Value.ToString());
//                    code4WSLocation.Longitude = Convert.ToDouble(workSheet.Cells["Y" + i].Value.ToString());
//                    code4WSLocation.Active = true;
//                    code4WSLocations.Add(code4WSLocation);
//                }
//            }

//            DbFWS.BulkInsert(code4WSLocations);
//            DbFWS.SaveChanges();


#endregion