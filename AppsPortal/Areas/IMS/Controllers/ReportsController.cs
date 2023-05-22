using AppsPortal.BaseControllers;
using IMS_DAL.ViewModels;
using OfficeOpenXml;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace AppsPortal.Areas.IMS.Controllers
{
    public class ReportsController : IMSBaseController
    {
        // GET: IMS/Reports
        public ActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public ActionResult ModelTrackReport(ModelTrackReportModel reportParameters)
        {

            DateTime today = DateTime.Now;
            var myQuery = DbIMS.v_IMSTrackMissionReportForm.AsQueryable();


            if (reportParameters.Governorates != null && reportParameters.Governorates.Count > 0)
            {
                myQuery = myQuery.Where(x =>
                   reportParameters.Governorates.Contains(x.GovernorateGUID)
                   );
            }
            if (reportParameters.MissionStartFromDate != null)
            {
                myQuery = myQuery.Where(x =>

                     x.MissionStartDate >= reportParameters.MissionStartFromDate);
            }
            if (reportParameters.MissionStartToDate != null)
            {
                myQuery = myQuery.Where(x =>
                    x.MissionStartDate <= reportParameters.MissionStartToDate);
            }
            if (reportParameters.MissionStatusGUIDs != null && reportParameters.MissionStatusGUIDs.Count > 0)
            {
                myQuery = myQuery.Where(x =>
                    reportParameters.MissionStatusGUIDs.Contains(x.MissionStatusGUID)
                );
            }



            string sourceFile = Server.MapPath("~/Areas/IMS/ExcelTemplates/IMS_MissionReportOverview.xlsx");
            string DisFolder =
                Server.MapPath("~/Areas/IMS/temp/IMS_MissionReportOverview" + DateTime.Now.ToBinary() + ".xlsx");
            System.IO.File.Copy(sourceFile, DisFolder);
            using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
            {
                var cx = package.Workbook.Worksheets.ToList();
                ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                DataTable dt = new DataTable();
                dt.Columns.Add("Mission Code", typeof(string));
                dt.Columns.Add("Mission Status", typeof(string));
                dt.Columns.Add("MissionStartDate", typeof(DateTime));
                dt.Columns.Add("MissionEndDate", typeof(DateTime));
                dt.Columns.Add("Comments", typeof(string));
                dt.Columns.Add("Governorate", typeof(string));
                dt.Columns.Add("District", typeof(string));
                dt.Columns.Add("SubDistrict", typeof(string));
                dt.Columns.Add("Community", typeof(string));
                dt.Columns.Add("Address", typeof(string));//10
                dt.Columns.Add("Longitude", typeof(string));
                dt.Columns.Add("Latitude", typeof(string));
                dt.Columns.Add("CreatedBy", typeof(string));
                dt.Columns.Add("Departments", typeof(string));
                dt.Columns.Add("Members", typeof(string));
                dt.Columns.Add("VisitObjectivies", typeof(string));
                dt.Columns.Add("Humanitarian Needs", typeof(string));
                dt.Columns.Add("OngoingResponse", typeof(string));
                dt.Columns.Add("FormChallenge ", typeof(string));
                var departments = DbIMS.dataMissionReportDepartment.ToList();

                var members = DbIMS.dataMissionReportFormMember.ToList();
                var visitsFormVisitObjectives = DbIMS.dataMissionReportFormVisitObjective.ToList();
                var MissionReportFormHumanitarianNeed = DbIMS.dataMissionReportFormHumanitarianNeed.ToList();
                var MissionReportFormOngoingResponse = DbIMS.dataMissionReportFormOngoingResponse.ToList();
                var MissionReportFormChallenge = DbIMS.dataMissionReportFormChallenge.ToList();
                var res = myQuery.ToList();
                foreach (var item in res)
                {
                    string currentDepartment = "";
                    string currentmember = "";
                    string currentVisit = "";
                    string HumanitarianNeed = "";
                    string OngoingResponse = "";
                    string FormChallenge = "";
                    DataRow dr;
                    dr = dt.NewRow();
                    dr[0] = item.MissionCode;
                    dr[1] = item.MissionStatus;
                    dr[2] = item.MissionStartDate;
                    dr[3] = item.MissionEndDate;
                    dr[4] = item.Comments;

                    dr[5] = item.Governorate;
                    dr[6] = item.District;
                    dr[7] = item.SubDistrict;
                    dr[8] = item.Community;
                    dr[9] = item.Address;//10
                    dr[10] = item.longitude;
                    dr[11] = item.Latitude;
                    dr[12] = item.CreatedBy;

                    if (departments.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID).Count() > 0)
                    {
                        foreach (var department in departments.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID))
                        {
                            currentDepartment += department.codeDepartments.codeDepartmentsLanguages.Where(x => x.LanguageID == LAN).Select(x => x.DepartmentDescription).FirstOrDefault() + ";"
                                                ;
                        }
                        dr[13] = currentDepartment;
                    }
                    else
                    {
                        dr[13] = "";

                    }

                    if (members.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID).Count() > 0)
                    {
                        foreach (var member in members.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID))
                        {
                            currentmember += member.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN)
                                                 .Select(x => x.FirstName).FirstOrDefault() + " " +
                                             member.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN)
                                                 .Select(x => x.Surname).FirstOrDefault() + ";";
                        }
                        dr[14] = currentmember;
                    }
                    else
                    {
                        dr[14] = "";

                    }

                    if (visitsFormVisitObjectives.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID).Count() > 0)
                    {
                        foreach (var visit in visitsFormVisitObjectives.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID))
                        {
                            currentVisit += visit.VisitObjectiveName + ";";
                        }
                        dr[15] = currentVisit;
                    }
                    else
                    {
                        dr[15] = "";

                    }
                    if (MissionReportFormHumanitarianNeed.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID).Count() > 0)
                    {
                        foreach (var visit in MissionReportFormHumanitarianNeed.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID))
                        {
                            HumanitarianNeed += visit.HumanitarianNeedName + ";";
                        }
                        dr[16] = HumanitarianNeed;
                    }
                    else
                    {
                        dr[16] = "";

                    }

                    if (MissionReportFormOngoingResponse.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID).Count() > 0)
                    {
                        foreach (var visit in MissionReportFormOngoingResponse.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID))
                        {
                            OngoingResponse += visit.OngoingResponseName + ";";
                        }
                        dr[17] = OngoingResponse;
                    }
                    else
                    {
                        dr[17] = "";

                    }
                    if (MissionReportFormChallenge.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID).Count() > 0)
                    {
                        foreach (var visit in MissionReportFormChallenge.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID))
                        {
                            FormChallenge += visit.ChallengeName + ";";
                        }
                        dr[18] = FormChallenge;
                    }
                    else
                    {
                        dr[18] = "";

                    }





                    dt.Rows.Add(dr);
                }
                workSheet.Cells["B8"].LoadFromDataTable(dt, true);
                package.Save();

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = DateTime.Now.ToString("yyMMdd") + "_Mission Report" + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

                // return Json(DbCMS.SingleUpdateMessage(null, null, null, null, "Please Wait...."));
            }
        }
    }
}