using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppsPortal.BaseControllers;
using AppsPortal.Library;
using IMS_DAL.Model;
using IMS_DAL.ViewModels;

namespace AppsPortal.Areas.IMS.Controllers
{
    public class ImportDataController : Controller
    {

        public IMSEntities DbIMS = new IMSEntities();
        // GET: IMS/ImportData
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MissionForm()
        {
            //List<MissionReportFormDataTableModel> model = DbIMS.dataMissionReportForm.ToList().Select(x =>
            //    new MissionReportFormDataTableModel
            //        {MissionStartDate = x.MissionStartDate.Date,
            //            MissionEndDate = x.MissionEndDate.Value.Date}).ToList();



            List<missonFormDataTableModel> modela = (from a in DbIMS.dataMissionReportForm
                                                     join b in DbIMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == "EN") on a.DistrictGUID equals b.LocationGUID into LJ1
                                                     from R1 in LJ1.DefaultIfEmpty()
                                                     join c in DbIMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == "EN" && x.codeTablesValues.TableGUID == LookupTables.IMSMissionStatus) on a.MissionStatusGUID equals c.ValueGUID into LJ2
                                                     from R2 in LJ2.DefaultIfEmpty()

                                                     join d in DbIMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == "EN") on a.SubDistrictGUID equals d.LocationGUID into LJ3
                                                     from R3 in LJ3.DefaultIfEmpty()

                                                     join d in DbIMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == "EN") on a.CommunityGUID equals d.LocationGUID into LJ4
                                                     from R4 in LJ4.DefaultIfEmpty()

                                                     join e in DbIMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == "EN") on a.GovernorateGUID equals e.LocationGUID into LJ5
                                                     from R5 in LJ5.DefaultIfEmpty()

                                                     join f in DbIMS.codeMissionOfficeSourceLanguage.Where(x => x.Active && x.LanguageID == "EN") on a.MissionOfficeSourceGUID equals f.MissionOfficeSourceGUID into LJ6
                                                     from R6 in LJ6.DefaultIfEmpty()
                                                     select new missonFormDataTableModel
                                                     {
                                                         MissionReportFormGUID = a.MissionReportFormGUID,
                                                         MissionStartDate = a.MissionStartDate,

                                                         MissionEndDate = a.MissionEndDate,
                                                         DutyStation = R6.MissionOfficeSourceDescription,
                                                         Governorate = R5.LocationDescription,
                                                         District = R1.LocationDescription,
                                                         SubDistrict = R3.LocationDescription,
                                                         Community = R4.LocationDescription,
                                                         Address = a.Address,
                                                         Longitude = (float?)a.longitude,
                                                         Latitude = (float?)a.Latitude,
                                                         Coordinates = ((float?)a.Latitude + " " + (float?)a.longitude),
                                                         CreatedBy = a.userAccounts1.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == "EN").Select(x => x.FirstName).FirstOrDefault() + " " + a.userAccounts1.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == "EN").Select(x => x.Surname).FirstOrDefault(),
                                                         CreatedDate = a.CreatedDate,
                                                         Is_AnyPresence_Other_Organization_ = a.IsAnyPresenceOtherOrganization,
                                                         Gaps = a.Gaps,
                                                         Recommendations = a.Recommendations,
                                                         ActionRequired = a.ActionRequired,
                                                         LinkToMissionReport = a.LinkToMissionReport,
                                                         Is_There_Mission_Report_ = a.IsThereMissionReport,
                                                         AllDepartments =
                                                               (from aa in DbIMS.dataMissionReportDepartment.Where(xx =>
                                                                       xx.MissionReportFormGUID == a.MissionReportFormGUID)
                                                                join bb in DbIMS.codeDepartmentsLanguages.Where(x => x.Active && x.LanguageID == "EN") on aa
                             .DepartmentGUID equals bb.DepartmentGUID
                                                                select bb.DepartmentDescription).ToList(),
                                                         AllMembers =
                                                           (from aa in DbIMS.dataMissionReportFormMember.Where(xx =>
                                                                   xx.MissionReportFormGUID == a.MissionReportFormGUID)
                                                            join bb in DbIMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == "EN") on aa.UserGUID equals bb.UserGUID
                                                            select bb.FirstName + " " + bb.Surname).ToList(),

                                                         AllvisitsFormVisitObjectives =
                                                           (from aa in DbIMS.dataMissionReportFormVisitObjective.Where(xx =>
                                                                   xx.MissionReportFormGUID == a.MissionReportFormGUID)
                                                            join bb in DbIMS.codeIMSVisitObjectiveLanguage.Where(x => x.Active && x.LanguageID == "EN") on aa.VisitObjectiveGUID equals bb.VisitObjectiveGUID
                                                            select bb.VisitObjectiveDescription).ToList(),

                                                         AllMissionReportFormHumanitarianNeed =
                                                           (from aa in DbIMS.dataMissionReportFormHumanitarianNeed.Where(xx =>
                                                                   xx.MissionReportFormGUID == a.MissionReportFormGUID)
                                                            join bb in DbIMS.codeIMSHumanitarianNeedLanguage.Where(x => x.Active && x.LanguageID == "EN") on aa.HumanitarianNeedGUID equals bb.HumanitarianNeedGUID
                                                            select bb.HumanitarianNeedeDescription).ToList(),

                                                         AllMissionReportFormOngoingResponse =
                                                           (from aa in DbIMS.dataMissionReportFormOngoingResponse.Where(xx =>
                                                                   xx.MissionReportFormGUID == a.MissionReportFormGUID)
                                                            join bb in DbIMS.codeIMSOngoingResponseLanguage.Where(x => x.Active && x.LanguageID == "EN") on aa.OngoingResponseGUID equals bb.OngoingResponseGUID
                                                            select bb.OngoingResponseDescription).ToList(),


                                                         AllMissionReportFormChallenge =
                                                           (from aa in DbIMS.dataMissionReportFormChallenge.Where(xx =>
                                                                   xx.MissionReportFormGUID == a.MissionReportFormGUID)
                                                            join bb in DbIMS.codeIMSMissionChallengeLanguage.Where(x => x.Active && x.LanguageID == "EN") on aa.MissionChallengeGUID equals bb.MissionChallengeGUID
                                                            select bb.MissionChallengeDescription).ToList(),

                                                     }

            ).ToList();

            List<missonFormDataTableModel> myModel = (from a in modela
                                                      select new missonFormDataTableModel
                                                      {
                                                          MissionReportFormGUID = a.MissionReportFormGUID,
                                                          MissionStartDate = a.MissionStartDate,

                                                          MissionEndDate = a.MissionEndDate,
                                                          DutyStation = a.DutyStation,
                                                          Governorate = a.Governorate,
                                                          District = a.District,
                                                          SubDistrict = a.SubDistrict,
                                                          Community = a.Community,
                                                          Address = a.Address,
                                                          Longitude = a.Longitude,
                                                          Latitude = a.Latitude,
                                                          CreatedBy = a.CreatedBy,
                                                          Coordinates = a.Coordinates,
                                                          CreatedDate = a.CreatedDate,
                                                          Is_AnyPresence_Other_Organization_ = a.Is_AnyPresence_Other_Organization_,
                                                          Gaps = a.Gaps,
                                                          Recommendations = a.Recommendations,
                                                          ActionRequired = a.ActionRequired,
                                                          LinkToMissionReport = a.LinkToMissionReport,
                                                          Is_There_Mission_Report_ = a.Is_There_Mission_Report_,
                                                          Departments = string.Join(",", a.AllDepartments),
                                                          members = string.Join(",", a.AllMembers),
                                                          visitsFormVisitObjectives = string.Join(",", a.AllvisitsFormVisitObjectives),
                                                          MissionReportFormHumanitarianNeed = string.Join(",", a.AllMissionReportFormHumanitarianNeed),
                                                          MissionReportFormOngoingResponse = string.Join(",", a.AllMissionReportFormOngoingResponse),
                                                          MissionReportFormChallenge = string.Join(",", a.AllMissionReportFormChallenge),
                                                      }).ToList();







            //}).ToList();




            return View("~/Areas/IMS/Views/ImportData/MissionForm.cshtml", myModel);
        }

        //public JsonResult InitMissionFormData()
        //{
        //    var missionForms = DbIMS.dataMissionReportForm.Select(x => new {x.MissionStartDate, x.MissionEndDate}).ToList();
        //    return Json(new { missionForm = missionForms} , JsonRequestBehavior.AllowGet);
        //}
    }
}