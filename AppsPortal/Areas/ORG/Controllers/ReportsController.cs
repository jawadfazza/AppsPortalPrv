using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using FineUploader;
using LinqKit;
using Microsoft.Reporting.WebForms;
using OfficeOpenXml;
using ORG_DAL.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace AppsPortal.Areas.ORG.Controllers
{
    public class ReportsController : ORGBaseController
    {
        // GET: ORG/Reports
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExportStaffPhoneDirectory()
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.WMS))
            {
                return Json(DbORG.PermissionError());
            }
            var result = DbORG.v_dataSyriaPhoneDirectory.ToList();

            if (result.Count > 0)
            {
                string sourceFile = Server.MapPath("~/Areas/ORG/Templates/SyriaStaffPhoneDirectory.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/ORG/Temp/ListOfStaffPhoneDirectory" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();
                    dt.Columns.Add("Duty Station", typeof(string));
                    dt.Columns.Add("Section", typeof(string));
                    dt.Columns.Add("Staff Name", typeof(string));
                    dt.Columns.Add("Email Address", typeof(string));
                    dt.Columns.Add("Title", typeof(string));
                    dt.Columns.Add("New Ext", typeof(string));
                    dt.Columns.Add("Mobile No.", typeof(string));
                    dt.Columns.Add("SAT Phone ", typeof(string));
                    dt.Columns.Add("HQ EXT", typeof(string));
                    dt.Columns.Add("Duty Station Ext", typeof(string));
                    dt.Columns.Add("Call Sign", typeof(string));


                    foreach (var item in result.OrderBy(x => x.DutyStation).ThenBy(x => x.DepartmentDescription))
                    {

                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.DutyStation;
                        dr[1] = item.DepartmentDescription;
                        dr[2] = item.FullName;
                        dr[3] = item.EmailAddress;
                        dr[4] = item.JobTitle;
                        dr[5] = item.OfficialExtensionNumber;
                        dr[6] = item.OfficialMobileNumber;
                        dr[7] = item.SATPhoneNumber;
                        dr[8] = item.HQExtensionNumber;
                        dr[9] = item.DutyStationExtensionNumber;
                        dr[10] = item.CallSign;

                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["B4"].LoadFromDataTable(dt, true);
                    //workSheet.Cells["A1"].Value = "_List of  national staff danger pay as of " + result.Select(x => x.PaymentDurationName).FirstOrDefault();

                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = "Staff Phone Directory" + Guid.NewGuid() + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }

            string success = "No Data Available for this period";
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult ExportStaffProfileAllInformation()
        {
            if (!CMS.HasAction(Permissions.StaffProfileHRSection.Update, Apps.ORG))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var result = DbORG.v_StaffProfileInformation.ToList();

            if (result.Count > 0)
            {
                string sourceFile = Server.MapPath("~/Areas/ORG/Templates/StaffProfileInformation.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/ORG/Temp/listofSyriaStaff" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Staff Status", typeof(string));

                    dt.Columns.Add("Full Name", typeof(string));
                    dt.Columns.Add("Full Name Arabic", typeof(string));
                    dt.Columns.Add("Email Address", typeof(string));
                    dt.Columns.Add("Contract Type", typeof(string));
                    dt.Columns.Add("Gender", typeof(string));
                    dt.Columns.Add("Department", typeof(string));
                    dt.Columns.Add("Duty Station", typeof(string));

                    dt.Columns.Add("Recruitment Type", typeof(string));
                    dt.Columns.Add("Grade", typeof(string));
                    dt.Columns.Add("Job Title(MOFA) EN", typeof(string));
                    dt.Columns.Add("Job Title(MOFA) AR", typeof(string));
                    dt.Columns.Add("Last Job En", typeof(string));
                    dt.Columns.Add("Last Job Ar", typeof(string));
                    dt.Columns.Add("Place Of Birth", typeof(string));
                    dt.Columns.Add("Place Birth City En", typeof(string));
                    dt.Columns.Add("Place Birth City Ar", typeof(string));
                    dt.Columns.Add("Date Of Birth", typeof(DateTime));
                    dt.Columns.Add("Nationality1 ", typeof(string));
                    dt.Columns.Add("Nationality1 Ar", typeof(string));
                    dt.Columns.Add("Nationality2", typeof(string));
                    dt.Columns.Add("Nationality2 Ar", typeof(string));
                    dt.Columns.Add("Nationality3", typeof(string));
                    dt.Columns.Add("Staff Prefix", typeof(string));
                    dt.Columns.Add("Blood Group", typeof(string));
                    dt.Columns.Add("Syrian National ID Number", typeof(string));

                    dt.Columns.Add("UNHCR ID Number", typeof(string));
                    dt.Columns.Add("Report To", typeof(string));
                    dt.Columns.Add("Staff EOD", typeof(DateTime));
                    dt.Columns.Add("Contract End Date", typeof(DateTime));
                    dt.Columns.Add("Date of Departure From Syria", typeof(DateTime));
                    dt.Columns.Add("Position Number", typeof(string));
                    dt.Columns.Add("Permanent Address En", typeof(string));
                    dt.Columns.Add("Permanent Address Ar", typeof(string));
                    dt.Columns.Add("Current Address En", typeof(string));
                    dt.Columns.Add("Current Address Ar", typeof(string));
                    dt.Columns.Add("Home TelephoneNumber Land Line", typeof(string));
                    dt.Columns.Add("Home TelephoneNumber Mobile", typeof(string));
                    dt.Columns.Add("Official Mobile Number", typeof(string));
                    dt.Columns.Add("Office Room Number", typeof(string));
                    dt.Columns.Add("Bank Accounts", typeof(string));
                    dt.Columns.Add("Staff Documents", typeof(string));




                    //dt.Columns.Add("HQ Extension Number", typeof(string));
                    //dt.Columns.Add("DutyStationExtensionNumber", typeof(string));
                    //dt.Columns.Add("OfficialExtensionNumber", typeof(string));
                    //dt.Columns.Add("CallSign", typeof(string));
                    //dt.Columns.Add("OfficeRoomNumberBuilding", typeof(string));
                    //dt.Columns.Add("SATPhoneNumber", typeof(string));

                    var _jobtitle = DbAHD.codeJobTitlesLanguages.Where(x => x.Active).ToList();
                    var _PersonalInfo = DbAHD.userPersonalDetailsLanguage.Where(x => x.Active).ToList();
                    var _countris = DbAHD.codeCountriesLanguages.Where(x => x.Active).ToList();
                    var _staffCore = DbAHD.StaffCoreData.Where(x => x.Active).ToList();
                    foreach (var item in result.OrderBy(x => x.DutyStation).ThenBy(x => x.DepartmentDescription).ThenBy(x => x.FullName))
                    {

                        DataRow dr;
                        dr = dt.NewRow();
                        var _mystaff = _staffCore.Where(x => x.UserGUID == item.UserGUID).FirstOrDefault();
                        dr[0] = item.StaffStatus;
                        dr[1] = item.FullName;
                        dr[2] = _PersonalInfo.Where(x => x.LanguageID == "AR" && x.UserGUID == item.UserGUID).FirstOrDefault() != null ? (_PersonalInfo.Where(x => x.LanguageID == "AR" && x.UserGUID == item.UserGUID).FirstOrDefault().FirstName + " " + _PersonalInfo.Where(x => x.LanguageID == "AR" && x.UserGUID == item.UserGUID).FirstOrDefault().Surname) : "";

                        dr[3] = item.EmailAddress;
                        dr[4] = item.ContractType;
                        dr[5] = item.Gender;
                        dr[6] = item.DepartmentDescription;
                        dr[7] = item.DutyStation;
                        dr[8] = item.RecruitmentType;
                        dr[9] = item.Grade;
                        dr[10] = item.JobTitle;
                        dr[11] = item.GenderGUID == Guid.Parse("688B11E0-24FB-44B8-94CE-D8568C4742C7") ? _jobtitle.Where(x => x.LanguageID == "AR" && x.JobTitleGUID == item.JobTitleGUID).FirstOrDefault().JobTitleDescription : _jobtitle.Where(x => x.LanguageID == "AR" && x.JobTitleGUID == item.JobTitleGUID).FirstOrDefault().JobTitleDescriptionFemale;
                        dr[12] = item.LastJobEn;
                        //if (item.GenderGUID != Guid.Parse("688B11E0-24FB-44B8-94CE-D8568C4742C7")) {
                        //    var test_ = item.GenderGUID == Guid.Parse("688B11E0-24FB-44B8-94CE-D8568C4742C7")   ? _jobtitle.Where(x => x.LanguageID == "AR" && x.JobTitleGUID == item.JobTitleGUID).FirstOrDefault().JobTitleDescription : _jobtitle.Where(x => x.LanguageID == "AR" && x.JobTitleGUID == item.JobTitleGUID).FirstOrDefault().JobTitleDescriptionFemale;
                        //}

                        dr[13] = item.LastJobAr;
                        dr[14] = item.PlaceOfBirth;
                        dr[15] = item.PlaceBirthCityEn;
                        dr[16] = item.PlaceBirthCityAr;
                        dr[17] = item.DateOfBirth == null ? (object)DBNull.Value : Convert.ToDateTime(item.DateOfBirth);
                        dr[18] = item.Nationality1;
                        dr[19] = _countris.Where(x => x.CountryGUID == _mystaff.NationalityGUID && x.LanguageID == "AR").FirstOrDefault() != null ? _countris.Where(x => x.CountryGUID == _mystaff.NationalityGUID && x.LanguageID == "AR").FirstOrDefault().Nationality : ""; ;
                        dr[20] = item.Nationality2;
                        dr[21] = _countris.Where(x => x.CountryGUID == _mystaff.Nationality2GUID && x.LanguageID == "AR").FirstOrDefault() != null ? _countris.Where(x => x.CountryGUID == _mystaff.Nationality2GUID && x.LanguageID == "AR").FirstOrDefault().Nationality : ""; ;
                        dr[22] = item.Nationality3;
                        dr[23] = item.StaffPrefix;
                        dr[24] = item.BloodGroup;
                        dr[25] = item.SyrianNationalIDNumber;


                        dr[26] = item.UNHCRIDNumber;
                        dr[27] = item.ReportTo;
                        dr[28] = item.StaffEOD == null ? (object)DBNull.Value : Convert.ToDateTime(item.StaffEOD).ToShortDateString();
                        dr[29] = item.ContractEndDate == null ? (object)DBNull.Value : Convert.ToDateTime(item.ContractEndDate).ToShortDateString();
                        dr[30] = item.LastDepartureDate == null ? (object)DBNull.Value : Convert.ToDateTime(item.LastDepartureDate).ToShortDateString();
                        dr[31] = item.PositionNumber;
                        dr[32] = item.PermanentAddressEn;
                        dr[33] = item.PermanentAddressAr;
                        dr[34] = item.CurrentAddressEn;
                        dr[35] = item.CurrentAddressAr;

                        dr[36] = item.HomeTelephoneNumberLandline;
                        dr[37] = item.HomeTelephoneNumberMobile;
                        dr[38] = item.OfficialMobileNumber;
                        dr[39] = item.OfficeRoomNumberBuilding;
                        dr[40] = item.BankAccounts;

                        dr[41] = item.StaffDocuments;





                        //dr[37] = item.HQExtensionNumber;
                        //dr[38] = item.DutyStationExtensionNumber;
                        //dr[39] = item.CallSign;
                        //dr[40] = item.OfficeRoomNumberBuilding;
                        //dr[41] = item.SATPhoneNumber;




                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["B4"].LoadFromDataTable(dt, true);
                    //workSheet.Cells["A1"].Value = "_List of  national staff danger pay as of " + result.Select(x => x.PaymentDurationName).FirstOrDefault();

                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = "Syria Staff Information" + Guid.NewGuid() + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }

            string success = "No Data Available for this period";
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);

        }
    }
}