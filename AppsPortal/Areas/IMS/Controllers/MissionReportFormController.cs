using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using FineUploader;
using IMS_DAL.Model;
using IMS_DAL.ViewModels;
using LinqKit;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;


namespace AppsPortal.Areas.IMS.Controllers
{
    public class MissionReportFormController : IMSBaseController
    {
        // GET: IMS/MissionReportForm


        #region Mission Report Form


        public ActionResult Index()
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            return View("~/Areas/IMS/Views/MissionReportForm/Index.cshtml");
        }

        [Route("IMS/MissionReportForm/")]
        public ActionResult MissionReportFormIndex()
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            return View("~/Areas/IMS/Views/MissionReportForm/Index.cshtml");
        }


        public JsonResult MissionReportFormDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MissionReportFormDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MissionReportFormDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.MissionReport.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
            var All = (

                from a in DbIMS.dataMissionReportForm.Where(x => x.CreatedByGUID == UserGUID).AsExpandable()
                join b in DbIMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DistrictGUID equals b.LocationGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbIMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN && x.codeTablesValues.TableGUID == LookupTables.IMSMissionStatus) on a.MissionStatusGUID equals c.ValueGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()

                join d in DbIMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.SubDistrictGUID equals d.LocationGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()

                join d in DbIMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.CommunityGUID equals d.LocationGUID into LJ4
                from R4 in LJ4.DefaultIfEmpty()

                join e in DbIMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.GovernorateGUID equals e.LocationGUID into LJ5
                from R5 in LJ5.DefaultIfEmpty()

                join f in DbIMS.codeMissionOfficeSourceLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.MissionOfficeSourceGUID equals f.MissionOfficeSourceGUID into LJ6
                from R6 in LJ6.DefaultIfEmpty()


                join g in DbIMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.MissionLeaderGUID equals g.UserGUID into LJ7
                from R7 in LJ7.DefaultIfEmpty()

                join k in DbIMS.codeMissionOfficeSourceLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.MissionOfficeSourceGUID equals k.MissionOfficeSourceGUID into LJ8
                from R8 in LJ8.DefaultIfEmpty()


                select new MissionReportFormDataTableModel
                {
                    MissionReportFormGUID = a.MissionReportFormGUID,
                    MissionSourceOffice = R8.MissionOfficeSourceDescription,
                    GovernorateGUID = a.GovernorateGUID.ToString(),
                    DistrictGUID = a.DistrictGUID.ToString(),
                    SubDistrictGUID = a.SubDistrictGUID.ToString(),
                    MissionStatusGUID = a.MissionStatusGUID.ToString(),
                    MissionSourceOfficeGUID = a.MissionOfficeSourceGUID.ToString(),
                    Address = a.Address,
                    CommunityGUID = a.CommunityGUID.ToString(),
                    //DutyStation = R6.MissionOfficeSourceDescription,
                    Governorate = R5.LocationDescription,
                    District = R1.LocationDescription,
                    SubDistrict = R3.LocationDescription,
                    MissionLeaderGUID = a.MissionLeaderGUID.ToString(),
                    MissionLeader = R7.FirstName + " " + R7.Surname,
                    Community = R4.LocationDescription,
                    MissionStartDate = a.MissionStartDate,
                    MissionEndDate = a.MissionEndDate,
                    MissionCode = a.MissionCode,
                    MissionNumber = a.MissionNumber,
                    ReportExtensionType = a.ReportExtensionType,
                    VisitObjectiveName = a.dataMissionReportFormVisitObjective.Select(x => x.VisitObjectiveName).FirstOrDefault(),
                    CreatedBy = a.userAccounts1.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + a.userAccounts1.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault(),
                    CreatedByGUID = a.CreatedByGUID.ToString(),
                    Active = a.Active,
                    MissionStatus = R2.ValueDescription,
                    dataMissionReportFormRowVersion = a.dataMissionReportFormRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<MissionReportFormDataTableModel> Result = Mapper.Map<List<MissionReportFormDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result.OrderByDescending(x => x.MissionNumber).ThenByDescending(x => x.CreatedDate)), JsonRequestBehavior.AllowGet);
        }


        #region Track ALl missions need to be Factored 
        [Route("IMS/IndexForms/")]
        public ActionResult IndexForms()
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            return View("~/Areas/IMS/Views/MissionReportForm/IndexForm.cshtml");
        }
        public JsonResult MissionReportFormTrackingDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);
            Expression<Func<MissionReportFormDataTableModel, bool>> Predicate = p => true;
            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MissionReportFormDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.MissionReport.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
            var All = (
               from a in DbIMS.v_IMSTrackMissionReportForm
               select new MissionReportFormDataTableModel
               {
                   MissionReportFormGUID = a.MissionReportFormGUID,

                   GovernorateGUID = a.GovernorateGUID.ToString(),
                   DistrictGUID = a.DistrictGUID.ToString(),
                   SubDistrictGUID = a.SubDistrictGUID.ToString(),
                   MissionStatusGUID = a.MissionStatusGUID.ToString(),
                   Address = a.Address,
                   CommunityGUID = a.CommunityGUID.ToString(),
                   MissionSourceOffice = a.MissionOfficeSource,
                   MissionSourceOfficeGUID = a.MissionOfficeSourceGUID.ToString(),
                   //DutyStationGUID = a.DutyStationGUID.ToString(),
                   Governorate = a.Governorate,
                   District = a.District,
                   SubDistrict = a.SubDistrict,
                   MissionLeaderGUID = a.MissionLeaderGUID.ToString(),
                   MissionLeader = a.MissionLeader,
                   ReportExtensionType = a.ReportExtensionType,



                   Community = a.Community,
                   MissionStartDate = a.MissionStartDate,
                   MissionEndDate = a.MissionEndDate,
                   MissionCode = a.MissionCode,
                   MissionNumber = a.MissionNumber,
                   VisitObjectiveName = a.VisitObjectives,
                   HumanitarianNeeds = a.HumanitarianNeeds,
                   OngoingResponses = a.OngoingResponses,
                   MissionChallenges = a.MissionChallenges,
                   StaffMembers = a.HumanitarianNeeds,
                   Units = a.Units,
                   CreatedBy = a.CreatedBy,
                   CreatedByGUID = a.CreatedByGUID.ToString(),
                   Active = a.Active,
                   MissionStatus = a.MissionStatus,
                   dataMissionReportFormRowVersion = a.dataMissionReportFormRowVersion
               }).Where(Predicate);
            //var result = (

            //    from a in DbIMS.dataMissionReportForm.AsExpandable()
            //    join b in DbIMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DistrictGUID equals b.LocationGUID into LJ1
            //    from R1 in LJ1.DefaultIfEmpty()
            //    join c in DbIMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN && x.codeTablesValues.TableGUID == LookupTables.IMSMissionStatus) on a.MissionStatusGUID equals c.ValueGUID into LJ2
            //    from R2 in LJ2.DefaultIfEmpty()

            //    join d in DbIMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.SubDistrictGUID equals d.LocationGUID into LJ3
            //    from R3 in LJ3.DefaultIfEmpty()

            //    join d in DbIMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.CommunityGUID equals d.LocationGUID into LJ4
            //    from R4 in LJ4.DefaultIfEmpty()

            //    join e in DbIMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.GovernorateGUID equals e.LocationGUID into LJ5
            //    from R5 in LJ5.DefaultIfEmpty()

            //        //join f in DbIMS.codeDu.Where(x => x.Active && x.LanguageID == LAN) on a.GovernorateGUID equals e.LocationGUID into LJ5
            //        //from R5 in LJ5.DefaultIfEmpty()

            //    join f in DbIMS.codeDutyStationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DutyStationGUID equals f.DutyStationGUID into LJ6
            //    from R6 in LJ6.DefaultIfEmpty()

            //    join g in DbIMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.MissionLeaderGUID equals g.UserGUID into LJ7
            //    from R7 in LJ7.DefaultIfEmpty()


            //    select new MissionReportFormDataTableModel
            //    {
            //        MissionReportFormGUID = a.MissionReportFormGUID,
            //        DutyStation = R6.DutyStationDescription,
            //        GovernorateGUID = a.GovernorateGUID.ToString(),
            //        DistrictGUID = a.DistrictGUID.ToString(),
            //        SubDistrictGUID = a.SubDistrictGUID.ToString(),
            //        MissionStatusGUID = a.MissionStatusGUID.ToString(),
            //        Address = a.Address,
            //        MissionCode = a.MissionCode,
            //        MissionNumber = a.MissionNumber,
            //        MissionLeaderGUID = a.MissionLeaderGUID.ToString(),

            //        MissionLeader = R7.FirstName + " " + R7.Surname,
            //        CreatedByGUID = a.CreatedByGUID.ToString(),

            //        CommunityGUID = a.CommunityGUID.ToString(),
            //        Governorate = R5.LocationDescription,
            //        District = R1.LocationDescription,
            //        SubDistrict = R3.LocationDescription,
            //        Community = R4.LocationDescription,
            //        MissionStartDate = a.MissionStartDate,
            //        MissionEndDate = a.MissionEndDate,
            //        AllDepartments =
            //        (from aa in DbIMS.dataMissionReportDepartment.Where(xx =>
            //                xx.MissionReportFormGUID == a.MissionReportFormGUID)
            //         join bb in DbIMS.codeDepartmentsLanguages.Where(x => x.Active && x.LanguageID == "EN") on aa
            //             .DepartmentGUID equals bb.DepartmentGUID
            //         select bb.DepartmentDescription).ToList(),
            //        AllMembers =
            //        (from aa in DbIMS.dataMissionReportFormMember.Where(xx =>
            //                xx.MissionReportFormGUID == a.MissionReportFormGUID)
            //         join bb in DbIMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == "EN") on
            //             aa.UserGUID equals bb.UserGUID
            //         select bb.FirstName + " " + bb.Surname).ToList(),

            //        AllvisitsFormVisitObjectives =
            //        (from aa in DbIMS.dataMissionReportFormVisitObjective.Where(xx =>
            //                xx.MissionReportFormGUID == a.MissionReportFormGUID)
            //         join bb in DbIMS.codeIMSVisitObjectiveLanguage.Where(x => x.Active && x.LanguageID == "EN") on
            //             aa.VisitObjectiveGUID equals bb.VisitObjectiveGUID
            //         select bb.VisitObjectiveDescription).ToList(),

            //        AllMissionReportFormHumanitarianNeed =
            //        (from aa in DbIMS.dataMissionReportFormHumanitarianNeed.Where(xx =>
            //                xx.MissionReportFormGUID == a.MissionReportFormGUID)
            //         join bb in DbIMS.codeIMSHumanitarianNeedLanguage.Where(x => x.Active && x.LanguageID == "EN") on
            //             aa.HumanitarianNeedGUID equals bb.HumanitarianNeedGUID
            //         select bb.HumanitarianNeedeDescription).ToList(),

            //        AllMissionReportFormOngoingResponse =
            //        (from aa in DbIMS.dataMissionReportFormOngoingResponse.Where(xx =>
            //                xx.MissionReportFormGUID == a.MissionReportFormGUID)
            //         join bb in DbIMS.codeIMSOngoingResponseLanguage.Where(x => x.Active && x.LanguageID == "EN") on
            //             aa.OngoingResponseGUID equals bb.OngoingResponseGUID
            //         select bb.OngoingResponseDescription).ToList(),


            //        AllMissionReportFormChallenge =
            //        (from aa in DbIMS.dataMissionReportFormChallenge.Where(xx =>
            //                xx.MissionReportFormGUID == a.MissionReportFormGUID)
            //         join bb in DbIMS.codeIMSMissionChallengeLanguage.Where(x => x.Active && x.LanguageID == "EN") on
            //             aa.MissionChallengeGUID equals bb.MissionChallengeGUID
            //         select bb.MissionChallengeDescription).ToList(),

            //        CreatedBy = a.userAccounts1.userPersonalDetails.userPersonalDetailsLanguage.Where(x=>x.LanguageID==LAN).Select(x => x.FirstName).FirstOrDefault() + " " + a.userAccounts1.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault(),
            //        Active = a.Active,
            //        MissionStatus = R2.ValueDescription,
            //        dataMissionReportFormRowVersion = a.dataMissionReportFormRowVersion
            //    }).Where(Predicate);
            //var myResult = result.AsEnumerable().Select(a=> new MissionReportFormDataTableModel
            //{
            //    MissionReportFormGUID = a.MissionReportFormGUID,
            //    DutyStation = a.DutyStation,
            //    GovernorateGUID = a.GovernorateGUID,
            //    DistrictGUID = a.DistrictGUID,
            //    SubDistrictGUID = a.SubDistrictGUID,
            //    MissionStatusGUID = a.MissionStatusGUID,
            //    Address = a.Address,
            //    MissionCode = a.MissionCode,
            //    MissionNumber = a.MissionNumber,
            //    MissionLeaderGUID = a.MissionLeaderGUID,

            //    MissionLeader = a.MissionLeader,
            //    CreatedByGUID = a.CreatedByGUID,

            //    CommunityGUID = a.CommunityGUID.ToString(),
            //    Governorate = a.Governorate,
            //    District = a.District,
            //    SubDistrict = a.SubDistrict,
            //    Community = a.Community,
            //    MissionStartDate = a.MissionStartDate,
            //    MissionEndDate = a.MissionEndDate,

            //    Departments = string.Join(",", a.AllDepartments),
            //    members = string.Join(",", a.AllMembers),
            //    visitsFormVisitObjectives = string.Join(",", a.AllvisitsFormVisitObjectives),
            //    MissionReportFormHumanitarianNeed = string.Join(",", a.AllMissionReportFormHumanitarianNeed),
            //    MissionReportFormOngoingResponse = string.Join(",", a.AllMissionReportFormOngoingResponse),
            //    MissionReportFormChallenge = string.Join(",", a.AllMissionReportFormChallenge),
            //});
            //var All = myResult.AsExpandable().Select(a => new MissionReportFormDataTableModel
            //{
            //    MissionReportFormGUID = a.MissionReportFormGUID,
            //    DutyStation = a.DutyStation,
            //    GovernorateGUID = a.GovernorateGUID,
            //    DistrictGUID = a.DistrictGUID,
            //    SubDistrictGUID = a.SubDistrictGUID,
            //    MissionStatusGUID = a.MissionStatusGUID,
            //    Address = a.Address,
            //    MissionCode = a.MissionCode,
            //    MissionNumber = a.MissionNumber,
            //    MissionLeaderGUID = a.MissionLeaderGUID,

            //    MissionLeader = a.MissionLeader,
            //    CreatedByGUID = a.CreatedByGUID,

            //    CommunityGUID = a.CommunityGUID.ToString(),
            //    Governorate = a.Governorate,
            //    District = a.District,
            //    SubDistrict = a.SubDistrict,
            //    Community = a.Community,
            //    MissionStartDate = a.MissionStartDate,
            //    MissionEndDate = a.MissionEndDate,

            //    Departments = a.Departments,
            //    members =a.members,
            //    visitsFormVisitObjectives =a.visitsFormVisitObjectives,
            //    MissionReportFormHumanitarianNeed = a.MissionReportFormHumanitarianNeed,
            //    MissionReportFormOngoingResponse = a.MissionReportFormOngoingResponse,
            //    MissionReportFormChallenge =a.MissionReportFormChallenge,
            //}).Where(Predicate);
            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<MissionReportFormDataTableModel> Result = Mapper.Map<List<MissionReportFormDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result.OrderByDescending(x => x.MissionNumber).ThenByDescending(x => x.CreatedDate)), JsonRequestBehavior.AllowGet);
        }


        #endregion

        [Route("IMS/MissionForm/Create/")]
        public ActionResult MissionFormCreate()
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            MissionReportFormUpdateModel model = new MissionReportFormUpdateModel();
            model.MissionReportFormGUID = Guid.NewGuid();
            return View("~/Areas/IMS/Views/MissionReportForm/MissionForm.cshtml", model);
        }



        [Route("IMS/MissionReportForm/Create/")]
        public ActionResult MissionReportFormCreate()
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            return View("~/Areas/IMS/Views/MissionReportForm/MissionReportForm.cshtml", new MissionReportFormUpdateModel());
        }

        [Route("IMS/MissionReportForm/Update/{PK}")]
        public ActionResult MissionReportFormUpdate(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Update, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            var form = DbIMS.dataMissionReportForm.Find(PK);

            MissionReportFormUpdateModel model = Mapper.Map(form, new MissionReportFormUpdateModel());
            model.MissionReportFullPath = model.MissionReportFormGUID + model.ReportExtensionType;
            model.MissionStaffMembers = new List<Guid?>();
            var missionReportPhotos = DbIMS.dataMissionReportPhoto.Where(x => x.MissionReportFormGUID == PK)
                .Select(x => new { x.MissionReportPhotoGUID, x.PhotoPath }).ToList();
            string photoPath = @"/Uploads/IMS/MissionPhotos/" + PK + "/";
            //foreach (var item in missionReportPhotos)
            //{
            //    MissionPhoto myPhoto = new MissionPhoto
            //    {
            //        Photo = photoPath + item.ToString()
            //    };


            //}
            if (missionReportPhotos != null && missionReportPhotos.Count > 0)
                missionReportPhotos.ForEach(myDet =>
                    model.MissionFormPhotos.Add(new MissionPhoto
                    {
                        Photo = photoPath + myDet.MissionReportPhotoGUID.ToString() +
                                myDet.PhotoPath.Substring(myDet.PhotoPath.Length - 4)
                    })
                );


            //var staff = form.dataMissionReportFormMember.Select(x => x.UserGUID).ToList();
            var users = DbCMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN).ToList();
            model.showStaffMembers = " ";
            model.showVisitObjectives = " ";
            model.showHumanitarianNeeds = " ";
            model.showOngoingResponses = " ";
            model.showMissionChallenges = " ";
            model.showDepartments = " ";

            //foreach (var item in form.dataMissionReportFormMember)
            //{
            //    model.showStaffMembers += users.Where(x => x.UserGUID == item.UserGUID).Select(x => x.FirstName).FirstOrDefault() + " " +
            //        users.Where(x => x.UserGUID == item.UserGUID).Select(x => x.Surname).FirstOrDefault();
            //}

            Guid visitGuid = Guid.Parse("CEC1E764-1282-4C71-B4D3-00063D470001");
            foreach (var item in form.dataMissionReportDepartment)
            {
                model.showDepartments += " " + item.codeDepartments.codeDepartmentsLanguages
                                             .Where(x => x.LanguageID == LAN).Select(x => x.DepartmentDescription)
                                             .FirstOrDefault();

            }

            foreach (var item in form.dataMissionReportFormMember)
            {
                model.showStaffMembers +=
                    " " + item.userAccounts.userPersonalDetails.userPersonalDetailsLanguage
                        .Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + item
                        .userAccounts.userPersonalDetails.userPersonalDetailsLanguage
                        .Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault();

            }


            foreach (var item in form.dataMissionReportFormVisitObjective)
            {
                model.showVisitObjectives += " " + item.VisitObjectiveName;

            }

            Guid humanGuid = Guid.Parse("4EC1E764-1282-4C71-B4D3-00069D080112");
            foreach (var item in form.dataMissionReportFormHumanitarianNeed)
            {
                model.showHumanitarianNeeds += " " + item.HumanitarianNeedName;

            }

            Guid ongoinctGuid = Guid.Parse("CEC1E764-1282-4C71-B4D3-00069D080113");
            foreach (var item in form.dataMissionReportFormOngoingResponse)
            {
                model.showOngoingResponses += " " + item.OngoingResponseName;

            }

            Guid challangerGuid = Guid.Parse("4EC1E764-1282-4C71-B4D3-02069D0801B6");

            foreach (var item in form.dataMissionReportFormChallenge)
            {
                model.showMissionChallenges += " " + item.ChallengeName;

            }
            //foreach (var item in staff)
            //{
            //    model.MissionStaffMembers.Add((Guid)item);

            //}

            //var model = (from a in DbIMS.dataMissionReportForm.WherePK(PK)
            //             join b in DbIMS.codeWarehouseMissionReportLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataMissionReportForm.DeletedOn) && x.LanguageID == LAN)
            //             on a.MissionReportFormGUID equals b.MissionReportFormGUID into LJ1
            //             from R1 in LJ1.DefaultIfEmpty()
            //             select new MissionReportFormUpdateModel
            //             {
            //                 MissionReportFormGUID = a.MissionReportFormGUID,
            //                 BrandGUID = a.BrandGUID,
            //                 WarehouseItemGUID = a.WarehouseItemGUID,
            //                 ModelDescription = R1.ModelDescription,
            //                 Active = a.Active,
            //                 codeWarehouseMissionReportRowVersion = a.codeWarehouseMissionReportRowVersion,
            //                 codeWarehouseMissionReportLanguageRowVersion = R1.codeWarehouseMissionReportLanguageRowVersion
            //             }).FirstOrDefault();

            //if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("MissionReportForm", "MissionReportForm", new { Area = "IMS" }));

            return View("~/Areas/IMS/Views/MissionReportForm/EditMissionForm.cshtml", model);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MissionReportFormCreate(MissionReportFormUpdateModel model)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            //if (ActiveMissionReportForm(model)) return View("~/Areas/IMS/Views/MissionReportForm/MissionForm.cshtml", model);
            //if (model.MissionStartDate == null || model.MissionEndDate == null) return View("~/Areas/IMS/Views/MissionReportForm/MissionForm.cshtml", model);
            //if (model.MissionOfficeSourceGUID == null) return View("~/Areas/IMS/Views/MissionReportForm/MissionForm.cshtml", model);
            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = model.MissionReportFormGUID;
            model.DutyStationGUID = null;
            var checkDuplication = DbIMS.dataMissionReportForm.Where(x =>
                  x.MissionStartDate == model.MissionStartDate && x.CommunityGUID == model.CommunityGUID
                                                               && x.MissionLeaderGUID == model.MissionLeaderGUID &&
                                                               x.MissionLeaderGUID != null).FirstOrDefault();

            dataMissionReportForm MissionReport = Mapper.Map(model, new dataMissionReportForm());
            MissionReport.MissionReportFormGUID = EntityPK;
            MissionReport.CreatedDate = ExecutionTime;
            MissionReport.CreatedByGUID = UserGUID;
            MissionReport.MissionStatusGUID = model.MissionStatusGUID;
            if (checkDuplication != null)
                MissionReport.IsDuplicated = true;

            MissionReport.MissionNumber = DbIMS.dataMissionReportForm.Select(x => x.MissionNumber).Max() != null
                ? DbIMS.dataMissionReportForm.Select(x => x.MissionNumber).Max() + 1
                : 0;
            var office = DbIMS.codeMissionOfficeSource.Where(x => x.Active && x.MissionOfficeSourceGUID == MissionReport.MissionOfficeSourceGUID)
                .Select(x => x.OfficeSourceCode).FirstOrDefault();

            MissionReport.MissionCode = office + DateTime.Now.Year.ToString().Substring(2) + "-" +
                                        MissionReport.MissionNumber;




            Guid othervisitGuid = Guid.Parse("CEC1E764-1282-4C71-B4D3-00063D470001");
            Guid otherhumanGuid = Guid.Parse("4EC1E764-1282-4C71-B4D3-00069D080112");
            Guid otherongoinctGuid = Guid.Parse("CEC1E764-1282-4C71-B4D3-00069D080113");

            Guid otherchallangerGuid = Guid.Parse("4EC1E764-1282-4C71-B4D3-02069D0801B6");


            //List<dataMissionReportFormMember> missionReportFormMembers=new List<dataMissionReportFormMember>();
            //this code writtern in this way becuase we dont have CreateBulk and for lazy Reasons 
            if (model.MissionFormDepartments != null && model.MissionFormDepartments.Count > 0)
            {
                foreach (var item in model.MissionFormDepartments)
                {
                    dataMissionReportDepartment department = new dataMissionReportDepartment
                    {
                        MissionReportDepartmentGUID = Guid.NewGuid(),
                        MissionReportFormGUID = MissionReport.MissionReportFormGUID,
                        DepartmentGUID = item,

                        Active = true,

                    };
                    DbIMS.CreateNoAudit(department);
                }
            }


            if (model.MissionStaffMembers != null && model.MissionStaffMembers.Count > 0)
            {
                foreach (var item in model.MissionStaffMembers)
                {
                    dataMissionReportFormMember missionStaff = new dataMissionReportFormMember
                    {
                        MissionReportFormMemberGUID = Guid.NewGuid(),
                        MissionReportFormGUID = MissionReport.MissionReportFormGUID,
                        UserGUID = item,


                        Active = true,

                    };
                    DbIMS.CreateNoAudit(missionStaff);
                }
            }




            if (model.VisitObjectives != null && model.VisitObjectives.Count > 0)
            {


                var allVisits = DbIMS.codeIMSVisitObjectiveLanguage.ToList();
                foreach (var item in model.VisitObjectives)
                {
                    if (item != othervisitGuid)
                    {
                        dataMissionReportFormVisitObjective missionVisit = new dataMissionReportFormVisitObjective
                        {
                            MissionReportFormVisitObjectiveGUID = Guid.NewGuid(),
                            MissionReportFormGUID = MissionReport.MissionReportFormGUID,
                            VisitObjectiveGUID = item,
                            VisitObjectiveName =
                                allVisits.Where(x => x.VisitObjectiveGUID == item && x.LanguageID == LAN)
                                    .Select(x => x.VisitObjectiveDescription).FirstOrDefault(),

                            //Active = true,

                        };

                        DbIMS.CreateNoAudit(missionVisit);
                    }
                }

            }

            if (model.CustomVisitObjectiveName != null)
            {
                dataMissionReportFormVisitObjective missionVisit = new dataMissionReportFormVisitObjective
                {
                    MissionReportFormVisitObjectiveGUID = Guid.NewGuid(),
                    MissionReportFormGUID = MissionReport.MissionReportFormGUID,
                    VisitObjectiveGUID = othervisitGuid,
                    VisitObjectiveName = model.CustomVisitObjectiveName,

                    //Active = true,

                };

                DbIMS.CreateNoAudit(missionVisit);
            }



            if (model.HumanitarianNeeds != null && model.HumanitarianNeeds.Count > 0)
            {
                var allHumanitrianNeeds = DbIMS.codeIMSHumanitarianNeedLanguage.ToList();
                foreach (var item in model.HumanitarianNeeds)
                {
                    if (item != otherhumanGuid)
                    {
                        dataMissionReportFormHumanitarianNeed myModel = new dataMissionReportFormHumanitarianNeed
                        {
                            MissionReportFormHumanitarianNeedGUID = Guid.NewGuid(),
                            MissionReportFormGUID = MissionReport.MissionReportFormGUID,
                            HumanitarianNeedGUID = item,
                            HumanitarianNeedName =
                                allHumanitrianNeeds.Where(x => x.LanguageID == LAN && x.HumanitarianNeedGUID == item)
                                    .Select(x => x.HumanitarianNeedeDescription).FirstOrDefault(),

                            //Active = true,

                        };
                        DbIMS.CreateNoAudit(myModel);
                    }
                }



            }

            if (model.CustomHumanitarianNeedName != null)
            {
                dataMissionReportFormHumanitarianNeed missionVisit = new dataMissionReportFormHumanitarianNeed
                {
                    MissionReportFormHumanitarianNeedGUID = Guid.NewGuid(),
                    MissionReportFormGUID = MissionReport.MissionReportFormGUID,
                    HumanitarianNeedGUID = otherhumanGuid,
                    HumanitarianNeedName = model.CustomHumanitarianNeedName,

                    //Active = true,

                };

                DbIMS.CreateNoAudit(missionVisit);
            }


            if (model.OngoingResponses != null && model.OngoingResponses.Count > 0)
            {
                var allOngoingResponces = DbIMS.codeIMSOngoingResponseLanguage.ToList();
                foreach (var item in model.OngoingResponses)
                {
                    if (item != otherongoinctGuid)
                    {
                        dataMissionReportFormOngoingResponse mymodel = new dataMissionReportFormOngoingResponse
                        {
                            MissionReportFormOngoingResponseGUID = Guid.NewGuid(),
                            MissionReportFormGUID = MissionReport.MissionReportFormGUID,
                            OngoingResponseGUID = item,
                            OngoingResponseName =
                                allOngoingResponces.Where(x => x.LanguageID == LAN && x.OngoingResponseGUID == item)
                                    .Select(x => x.OngoingResponseDescription).FirstOrDefault(),

                            //Active = true,

                        };
                        DbIMS.CreateNoAudit(mymodel);
                    }
                }

            }

            if (model.CustomFormOngoingResponseName != null)
            {
                dataMissionReportFormOngoingResponse missionVisit = new dataMissionReportFormOngoingResponse
                {
                    MissionReportFormOngoingResponseGUID = Guid.NewGuid(),
                    MissionReportFormGUID = MissionReport.MissionReportFormGUID,
                    OngoingResponseGUID = otherongoinctGuid,
                    OngoingResponseName = model.CustomFormOngoingResponseName,


                    //Active = true,

                };

                DbIMS.CreateNoAudit(missionVisit);
            }
            var tempMissionActions = DbIMS.dataTempMissionAction.Where(x => x.MissionReportFormGUID == model.MissionReportFormGUID).ToList();
            if (tempMissionActions.Count > 0)
            {
                //List<dataMissionActionRequired> missionRequiredActions = new List<dataMissionActionRequired>();
                //List<dataMissionActionTaken> missionTakendActions = new List<dataMissionActionTaken>();
                foreach (var item in tempMissionActions)
                {
                    dataMissionActionRequired myActionRequired = new dataMissionActionRequired
                    {
                        MissionActionRequiredGUID = Guid.NewGuid(),
                        MissionReportFormGUID = model.MissionReportFormGUID,
                        DepartmentGUID = item.DepartmentGUID,
                        ActionRequiredName = item.ActionRequiredName,
                        FocalPointGUID = item.FocalPointGUID,
                        CreatedByGUID = UserGUID,
                        CreatedDate = ExecutionTime
                    };
                    //missionRequiredActions.Add(myActionRequired);
                    DbIMS.CreateNoAudit(myActionRequired);
                    dataMissionActionTaken myActionTaken = new dataMissionActionTaken
                    {
                        MissionActionTakenGUID = Guid.NewGuid(),
                        MissionActionRequiredGUID = myActionRequired.MissionActionRequiredGUID,
                        ActionStatusGUID = item.ActionStatusGUID,
                        ActionTakenName = item.ActionTakenName,
                        ActionTakenDate = item.ActionTakenDate,

                        CreatedByGUID = UserGUID,
                        CreatedDate = ExecutionTime
                    };
                    DbIMS.CreateNoAudit(myActionTaken);

                }
            }

            if (model.MissionChallenges != null && model.MissionChallenges.Count > 0)
            {
                var allChaLanguages = DbIMS.codeIMSMissionChallengeLanguage.ToList();
                foreach (var item in model.MissionChallenges)
                {
                    if (item != otherchallangerGuid)
                    {
                        dataMissionReportFormChallenge mymodel = new dataMissionReportFormChallenge
                        {
                            MissionReportFormChallengeGUID = Guid.NewGuid(),
                            MissionReportFormGUID = MissionReport.MissionReportFormGUID,
                            MissionChallengeGUID = item,
                            ChallengeName =
                                allChaLanguages.Where(x => x.LanguageID == LAN && x.MissionChallengeGUID == item)
                                    .Select(x => x.MissionChallengeDescription).FirstOrDefault(),

                            //Active = true,

                        };
                        DbIMS.CreateNoAudit(mymodel);
                    }
                }



            }
            if (model.CustomFormChallengeName != null)
            {
                dataMissionReportFormChallenge missionVisit = new dataMissionReportFormChallenge
                {
                    MissionReportFormChallengeGUID = Guid.NewGuid(),
                    MissionReportFormGUID = MissionReport.MissionReportFormGUID,
                    MissionChallengeGUID = otherchallangerGuid,
                    ChallengeName = model.CustomFormChallengeName



                    //Active = true,

                };

                DbIMS.CreateNoAudit(missionVisit);
            }

            if (model.UploadMissionFormFile != null)
            {

                var filePath = Server.MapPath("~\\Uploads\\IMS\\MissionForms\\");
                string extension = Path.GetExtension(model.UploadMissionFormFile.FileName);
                string fullFileName = filePath + "\\" + EntityPK + extension;
                MissionReport.ReportExtensionType = extension;
                model.UploadMissionFormFile.SaveAs(fullFileName);
            }

            string folderName = "";
            if (model.MissionPhoto1 != null || model.MissionPhoto2 != null || model.MissionPhoto3 != null ||
                model.MissionPhoto4 != null)
            {
                var folderPath = Server.MapPath("~\\Uploads\\IMS\\MissionPhotos\\");
                folderName = @"~/Uploads/IMS/MissionPhotos/" + EntityPK.ToString();
                Directory.CreateDirectory(folderPath + EntityPK.ToString());
            }


            if (model.MissionPhoto1 != null)
            {
                Guid photoGuid = Guid.NewGuid();
                var filePath = Server.MapPath(folderName);
                string extension = Path.GetExtension(model.MissionPhoto1.FileName);
                string fullFileName = filePath + "\\" + photoGuid + extension;
                //Save file to server folder  
                model.MissionPhoto1.SaveAs(fullFileName);
                dataMissionReportPhoto photo = new dataMissionReportPhoto
                {
                    MissionReportPhotoGUID = photoGuid,
                    MissionReportFormGUID = EntityPK,
                    PhotoPath = fullFileName,
                    CreatedDate = ExecutionTime
                };
                DbIMS.CreateNoAudit(photo);
            }
            if (model.MissionPhoto2 != null)
            {
                Guid photoGuid = Guid.NewGuid();
                var filePath = Server.MapPath(folderName);
                string extension = Path.GetExtension(model.MissionPhoto2.FileName);
                string fullFileName = filePath + "\\" + photoGuid + extension;
                //Save file to server folder  
                model.MissionPhoto2.SaveAs(fullFileName);
                dataMissionReportPhoto photo = new dataMissionReportPhoto
                {
                    MissionReportPhotoGUID = photoGuid,
                    MissionReportFormGUID = EntityPK,
                    PhotoPath = fullFileName,
                    CreatedDate = ExecutionTime
                };
                DbIMS.CreateNoAudit(photo);
            }
            if (model.MissionPhoto3 != null)
            {
                Guid photoGuid = Guid.NewGuid();
                var filePath = Server.MapPath(folderName);
                string extension = Path.GetExtension(model.MissionPhoto3.FileName);
                string fullFileName = filePath + "\\" + photoGuid + extension;
                //Save file to server folder  
                model.MissionPhoto3.SaveAs(fullFileName);
                dataMissionReportPhoto photo = new dataMissionReportPhoto
                {
                    MissionReportPhotoGUID = photoGuid,
                    MissionReportFormGUID = EntityPK,
                    PhotoPath = fullFileName,
                    CreatedDate = ExecutionTime
                };
                DbIMS.CreateNoAudit(photo);
            }

            if (model.MissionPhoto4 != null)
            {
                Guid photoGuid = Guid.NewGuid();
                var filePath = Server.MapPath(folderName);
                string extension = Path.GetExtension(model.MissionPhoto4.FileName);
                string fullFileName = filePath + "\\" + photoGuid + extension;
                //Save file to server folder  
                model.MissionPhoto4.SaveAs(fullFileName);
                dataMissionReportPhoto photo = new dataMissionReportPhoto
                {
                    MissionReportPhotoGUID = photoGuid,
                    MissionReportFormGUID = EntityPK,
                    PhotoPath = fullFileName,
                    CreatedDate = ExecutionTime
                };
                DbIMS.CreateNoAudit(photo);
            }

            DbIMS.CreateNoAudit(MissionReport);
            List<PartialViewModel> Partials = new List<PartialViewModel>();
            List<UIButtons> UIButtons = new List<UIButtons>();
            //UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.MissionReport.Create, Apps.IMS, new UrlHelper(Request.RequestContext).Action("Create", "MissionReportForm", new { Area = "IMS" })), Container = "MissionReportDetailFormControls" });
            //UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.MissionReport.Update, Apps.IMS), Container = "MissionReportDetailFormControls" });
            //UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.MissionReport.Delete, Apps.IMS), Container = "MissionReportDetailFormControls" });
            try
            {
                DbIMS.SaveChanges();
                //DbCMS.SaveChanges();
                return RedirectToAction("Index", "MissionReportForm", false);
                //return RedirectToAction("MissionFormComplete", new { id = model.MissionReportFormGUID });
                //return Json(DbIMS.SingleCreateMessage(DbIMS.PrimaryKeyControl(MissionReport), null, Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbIMS.ErrorMessage(ex.Message));
            }
        }

        public ActionResult MissionFormComplete(Guid id)
        {
            return View("~/Areas/IMS/Views/MissionReportForm/MissionFormComplete.cshtml", new MissionReportFormUpdateModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MissionReportFormUpdate(MissionReportFormUpdateModel model)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            if (ActiveMissionReportForm(model))
            {
                return View("~/Areas/IMS/Views/MissionReportForm/MissionForm.cshtml", model);
            }

            var form = DbIMS.dataMissionReportForm.Find(model.MissionReportFormGUID);

            if (form.CreatedByGUID == UserGUID)
            {
                //if (!ModelState.IsValid ) return View("~/Areas/IMS/Views/MissionReportForm/MissionForm.cshtml", model);

                DateTime ExecutionTime = DateTime.Now;
                dataMissionReportForm MissionReport = DbIMS.dataMissionReportForm.Find(model.MissionReportFormGUID);
                MissionReport.MissionOfficeSourceGUID = model.MissionOfficeSourceGUID;
                MissionReport.MissionStartDate = (DateTime)model.MissionStartDate;
                MissionReport.MissionEndDate = (DateTime)model.MissionEndDate;
                MissionReport.GovernorateGUID = model.GovernorateGUID;
                MissionReport.DistrictGUID = model.DistrictGUID;
                MissionReport.SubDistrictGUID = model.SubDistrictGUID;
                MissionReport.CommunityGUID = model.CommunityGUID;
                MissionReport.MissionLeaderGUID = model.MissionLeaderGUID;
                MissionReport.NVReference = model.NVReference;
                MissionReport.Address = model.Address;
                MissionReport.longitude = model.Longitude;
                MissionReport.Latitude = model.Latitude;
                MissionReport.Gaps = model.Gaps;
                MissionReport.Recommendations = model.Recommendations;
                MissionReport.ActionRequired = model.ActionRequired;
                MissionReport.ActionTaken = model.ActionTaken;
                MissionReport.LinkToMissionReport = model.LinkToMissionReport;
                MissionReport.IsAnyPresenceOtherOrganization = model.IsAnyPresenceOtherOrganization;
                MissionReport.IsThereMissionReport = model.IsThereMissionReport;
                MissionReport.MissionStatusGUID = model.MissionStatusGUID;
                MissionReport.MissionStatusGUID = model.MissionStatusGUID;
                MissionReport.IsAnyPresenceOtherOrganization = model.IsAnyPresenceOtherOrganization;
                // MissionReport = Mapper.Map(model, new dataMissionReportForm());
                MissionReport.DutyStationGUID = model.DutyStationGUID;
                if (model.UploadMissionFormFile != null)
                {

                    var filePath = Server.MapPath("~\\Uploads\\IMS\\MissionForms\\");
                    string extension = Path.GetExtension(model.UploadMissionFormFile.FileName);
                    string fullFileName = filePath + "\\" + model.MissionReportFormGUID + extension;
                    model.UploadMissionFormFile.SaveAs(fullFileName);
                    MissionReport.ReportExtensionType = extension;
                }
                DbIMS.UpdateNoAudit(MissionReport);

                string folderName = "";
                if (model.MissionPhoto1 != null || model.MissionPhoto2 != null || model.MissionPhoto3 != null ||
                    model.MissionPhoto4 != null)
                {
                    var folderPath = Server.MapPath("~\\Uploads\\IMS\\MissionPhotos\\");
                    folderName = @"~/Uploads/IMS/MissionPhotos/" + model.MissionReportFormGUID.ToString();
                    //string pathString = Path.Combine(folderName, EntityPK.ToString());
                    //Directory.Delete(folderPath+ model.MissionReportFormGUID.ToString());

                    if (Directory.Exists(@folderPath + model.MissionReportFormGUID.ToString()))
                    {
                        Directory.Delete(@folderPath + model.MissionReportFormGUID.ToString(), true);
                    }

                    var missionReportPhotos = DbIMS.dataMissionReportPhoto
                        .Where(x => x.MissionReportFormGUID == model.MissionReportFormGUID)
                        .ToList();
                    DbIMS.dataMissionReportPhoto.RemoveRange(missionReportPhotos);

                    // To create a string that specifies the path to a subfolder under your 
                    // top-level folder, add a name for the subfolder to folderName.

                    Directory.CreateDirectory(folderPath + model.MissionReportFormGUID.ToString());
                }


                if (model.MissionPhoto1 != null && model.MissionPhoto1.ContentLength > 0)
                {
                    Guid photoGuid = Guid.NewGuid();
                    var filePath = Server.MapPath(folderName);
                    string extension = Path.GetExtension(model.MissionPhoto1.FileName);
                    string fullFileName = filePath + "\\" + photoGuid + extension;
                    //Save file to server folder  
                    model.MissionPhoto1.SaveAs(fullFileName);
                    dataMissionReportPhoto photo = new dataMissionReportPhoto
                    {
                        MissionReportPhotoGUID = photoGuid,
                        MissionReportFormGUID = model.MissionReportFormGUID,
                        PhotoPath = fullFileName,
                        CreatedDate = ExecutionTime
                    };
                    DbIMS.CreateNoAudit(photo);
                }

                if (model.MissionPhoto2 != null && model.MissionPhoto2.ContentLength > 0)
                {
                    Guid photoGuid = Guid.NewGuid();
                    var filePath = Server.MapPath(folderName);
                    string extension = Path.GetExtension(model.MissionPhoto2.FileName);
                    string fullFileName = filePath + "\\" + photoGuid + extension;
                    //Save file to server folder  
                    model.MissionPhoto2.SaveAs(fullFileName);
                    dataMissionReportPhoto photo = new dataMissionReportPhoto
                    {
                        MissionReportPhotoGUID = photoGuid,
                        MissionReportFormGUID = model.MissionReportFormGUID,
                        PhotoPath = fullFileName,
                        CreatedDate = ExecutionTime
                    };
                    DbIMS.CreateNoAudit(photo);
                }

                if (model.MissionPhoto3 != null && model.MissionPhoto3.ContentLength > 0)
                {
                    Guid photoGuid = Guid.NewGuid();
                    var filePath = Server.MapPath(folderName);
                    string extension = Path.GetExtension(model.MissionPhoto3.FileName);
                    string fullFileName = filePath + "\\" + photoGuid + extension;
                    //Save file to server folder  
                    model.MissionPhoto3.SaveAs(fullFileName);
                    dataMissionReportPhoto photo = new dataMissionReportPhoto
                    {
                        MissionReportPhotoGUID = photoGuid,
                        MissionReportFormGUID = model.MissionReportFormGUID,
                        PhotoPath = fullFileName,
                        CreatedDate = ExecutionTime
                    };
                    DbIMS.CreateNoAudit(photo);
                }

                if (model.MissionPhoto4 != null && model.MissionPhoto4.ContentLength > 0)
                {
                    Guid photoGuid = Guid.NewGuid();
                    var filePath = Server.MapPath(folderName);
                    string extension = Path.GetExtension(model.MissionPhoto4.FileName);
                    string fullFileName = filePath + "\\" + photoGuid + extension;
                    //Save file to server folder  
                    model.MissionPhoto4.SaveAs(fullFileName);
                    dataMissionReportPhoto photo = new dataMissionReportPhoto
                    {
                        MissionReportPhotoGUID = photoGuid,
                        MissionReportFormGUID = model.MissionReportFormGUID,
                        PhotoPath = fullFileName,
                        CreatedDate = ExecutionTime
                    };
                    DbIMS.CreateNoAudit(photo);
                }
                //var tempmissionActions=DbIMS.dataTempMissionAction.Where(x => x.MissionReportFormGUID == model.MissionReportFormGUID).ToList();
                //var missionActionsRequired=DbIMS.dataMissionActionRequired.Where(x => x.MissionReportFormGUID == model.MissionReportFormGUID).ToList();
                //var missionActionsTaken = DbIMS.dataMissionActionTaken.Where(x => x.dataMissionActionRequired.MissionReportFormGUID == model.MissionReportFormGUID).ToList();
                //DbIMS.dataMissionActionRequired.RemoveRange(missionActionsRequired);
                //DbIMS.dataMissionActionTaken.RemoveRange(missionActionsTaken);

                //foreach (var item in tempmissionActions)
                //{
                //    dataMissionActionRequired myActionRequired = new dataMissionActionRequired
                //    {
                //        MissionActionRequiredGUID = Guid.NewGuid(),
                //        MissionReportFormGUID = model.MissionReportFormGUID,
                //        DepartmentGUID = item.DepartmentGUID,
                //        ActionRequiredName = item.ActionRequiredName,
                //        FocalPointGUID = item.FocalPointGUID,
                //        CreatedByGUID = UserGUID,
                //        CreatedDate = ExecutionTime
                //    };
                //    //missionRequiredActions.Add(myActionRequired);
                //    DbIMS.CreateNoAudit(myActionRequired);
                //    dataMissionActionTaken myActionTaken = new dataMissionActionTaken
                //    {
                //        MissionActionTakenGUID = Guid.NewGuid(),
                //        MissionActionRequiredGUID = myActionRequired.MissionActionRequiredGUID,
                //        ActionStatusGUID = item.ActionStatusGUID,
                //        ActionTakenName = item.ActionTakenName,
                //        ActionTakenDate = item.ActionTakenDate,

                //        CreatedByGUID = UserGUID,
                //        CreatedDate = ExecutionTime
                //    };
                //    DbIMS.CreateNoAudit(myActionTaken);
                //}
                try
                {
                    DbIMS.SaveChanges();
                    DbCMS.SaveChanges();
                    return RedirectToAction("Index", "MissionReportForm", false);

                }
                catch (DbUpdateConcurrencyException)
                {
                    return ConcurrencyMissionReportForm(model.MissionReportFormGUID);
                }
                catch (Exception ex)
                {
                    return Json(DbIMS.ErrorMessage(ex.Message));
                }
            }
            return RedirectToAction("Index", "MissionReportForm", false);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MissionReportFormDelete(dataMissionReportForm model)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Delete, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            List<dataMissionReportForm> DeletedMissionReport = DeleteMissionReportForm(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.WarehouseItemsEntry.Restore, Apps.IMS), Container = "MissionReportFormControls" });

            try
            {
                int CommitedRows = DbIMS.SaveChanges();
                DbIMS.SaveChanges();
                return Json(DbIMS.SingleDeleteMessage(CommitedRows, DeletedMissionReport.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMissionReportForm(model.MissionReportFormGUID);
            }
            catch (Exception ex)
            {
                return Json(DbIMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MissionReportFormRestore(dataMissionReportForm model)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            if (ActiveMissionReportForm(model))
            {
                return Json(DbIMS.RecordExists());
            }

            List<dataMissionReportForm> RestoredMissionReport = RestoreMissionReportForm(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            //UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.MissionReport.Create, Apps.IMS, new UrlHelper(Request.RequestContext).Action("MissionReportCreate", "Configuration", new { Area = "IMS" })), Container = "MissionReportFormControls" });
            //UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.MissionReport.Update, Apps.IMS), Container = "MissionReportFormControls" });
            //UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.MissionReport.Delete, Apps.IMS), Container = "MissionReportFormControls" });

            try
            {
                int CommitedRows = DbIMS.SaveChanges();
                DbIMS.SaveChanges();
                return Json(DbIMS.SingleRestoreMessage(CommitedRows, RestoredMissionReport, DbIMS.PrimaryKeyControl(RestoredMissionReport.FirstOrDefault()), Url.Action(DataTableNames.MissionReportFormDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMissionReportForm(model.MissionReportFormGUID);
            }
            catch (Exception ex)
            {
                return Json(DbIMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MissionReportFormDataTableDelete(List<dataMissionReportForm> models)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Delete, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            List<dataMissionReportForm> DeletedMissionReport = DeleteMissionReportForm(models);
            var guids = models.Select(x => x.MissionReportFormGUID).ToList();
            var missionChallanges = DbIMS.dataMissionReportFormChallenge.Where(x => guids.Contains((Guid)x.MissionReportFormGUID)).ToList();
            DbIMS.dataMissionReportFormChallenge.RemoveRange(missionChallanges);

            var OngoingResponse = DbIMS.dataMissionReportFormOngoingResponse.Where(x => guids.Contains((Guid)x.MissionReportFormGUID)).ToList();
            DbIMS.dataMissionReportFormOngoingResponse.RemoveRange(OngoingResponse);

            var humanitrainNeeds = DbIMS.dataMissionReportFormHumanitarianNeed.Where(x => guids.Contains((Guid)x.MissionReportFormGUID)).ToList();
            DbIMS.dataMissionReportFormHumanitarianNeed.RemoveRange(humanitrainNeeds);


            var visitObjectives = DbIMS.dataMissionReportFormVisitObjective.Where(x => guids.Contains((Guid)x.MissionReportFormGUID)).ToList();
            DbIMS.dataMissionReportFormVisitObjective.RemoveRange(visitObjectives);


            var missionReportFormFlowNotification = DbIMS.dataMissionReportFormFlowNotification.Where(x => guids.Contains((Guid)x.MissionReportFormGUID)).ToList();
            DbIMS.dataMissionReportFormFlowNotification.RemoveRange(missionReportFormFlowNotification);


            var dataMissionReportDepartment = DbIMS.dataMissionReportDepartment.Where(x => guids.Contains((Guid)x.MissionReportFormGUID)).ToList();
            DbIMS.dataMissionReportDepartment.RemoveRange(dataMissionReportDepartment);


            var dataMissionActionTemps = DbIMS.dataTempMissionAction.Where(x => guids.Contains((Guid)x.MissionReportFormGUID)).ToList();
            DbIMS.dataTempMissionAction.RemoveRange(dataMissionActionTemps);

            var dataMissionActionTakens = DbIMS.dataMissionActionTaken.Where(x => guids.Contains((Guid)x.dataMissionActionRequired.MissionReportFormGUID)).ToList();
            DbIMS.dataMissionActionTaken.RemoveRange(dataMissionActionTakens);

            var dataMissionActionRequireds = DbIMS.dataMissionActionRequired.Where(x => guids.Contains((Guid)x.MissionReportFormGUID)).ToList();
            DbIMS.dataMissionActionRequired.RemoveRange(dataMissionActionRequireds);


            var dataMissionReportFormFlow = DbIMS.dataMissionReportFormFlow.Where(x => guids.Contains((Guid)x.MissionReportFormGUID)).ToList();
            DbIMS.dataMissionReportFormFlow.RemoveRange(dataMissionReportFormFlow);
            var dataMissionReportFormMember = DbIMS.dataMissionReportFormMember.Where(x => guids.Contains((Guid)x.MissionReportFormGUID)).ToList();
            DbIMS.dataMissionReportFormMember.RemoveRange(dataMissionReportFormMember);

            var photos = DbIMS.dataMissionReportPhoto.Where(x => guids.Contains((Guid)x.MissionReportFormGUID)).ToList();
            DbIMS.dataMissionReportPhoto.RemoveRange(photos);

            var dataMissionReportForm = DbIMS.dataMissionReportForm.Where(x => guids.Contains((Guid)x.MissionReportFormGUID)).ToList();
            DbIMS.dataMissionReportForm.RemoveRange(dataMissionReportForm);





            try
            {
                DbIMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIMS.PartialDeleteMessage(DeletedMissionReport, models, DataTableNames.MissionReportFormDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbIMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MissionReportDataTableRestore(List<dataMissionReportForm> models)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Delete, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            List<dataMissionReportForm> RestoredMissionReport = RestoreMissionReportForm(models);

            try
            {
                DbIMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIMS.PartialRestoreMessage(RestoredMissionReport, models, DataTableNames.MissionReportFormDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbIMS.ErrorMessage(ex.Message));
            }
        }

        private List<dataMissionReportForm> DeleteMissionReportForm(List<dataMissionReportForm> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataMissionReportForm> DeletedMissionReport = new List<dataMissionReportForm>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT MissionReportFormGUID,CONVERT(varchar(50), MissionReportFormGUID) as C2 ,codeWarehouseMissionReportRowVersion FROM code.dataMissionReportForm where MissionReportFormGUID in (" + string.Join(",", models.Select(x => "'" + x.MissionReportFormGUID + "'").ToArray()) + ")";

            string query = DbIMS.QueryBuilder(models, Permissions.Notifications.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbIMS.Database.SqlQuery<dataMissionReportForm>(query).ToList();
            foreach (var record in Records)
            {
                DeletedMissionReport.Add(DbIMS.Delete(record, ExecutionTime, Permissions.Notifications.DeleteGuid, DbCMS));
            }


            return DeletedMissionReport;
        }

        private List<dataMissionReportForm> RestoreMissionReportForm(List<dataMissionReportForm> models)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            DateTime RestoringTime = DateTime.Now;

            List<dataMissionReportForm> RestoredMissionReport = new List<dataMissionReportForm>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT MissionReportFormGUID,CONVERT(varchar(50), MissionReportFormGUID) as C2 ,codeWarehouseMissionReportRowVersion FROM code.dataMissionReportForm where MissionReportFormGUID in (" + string.Join(",", models.Select(x => "'" + x.MissionReportFormGUID + "'").ToArray()) + ")";

            string query = DbIMS.QueryBuilder(models, Permissions.Notifications.DeleteGuid, SubmitTypes.Restore, "");

            var Records = DbIMS.Database.SqlQuery<dataMissionReportForm>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveMissionReportForm(record))
                {
                    RestoredMissionReport.Add(DbIMS.Restore(record, Permissions.Notifications.DeleteGuid, Permissions.Notifications.RestoreGuid, RestoringTime, DbCMS));
                }
            }


            return RestoredMissionReport;
        }

        private JsonResult ConcurrencyMissionReportForm(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            MissionReportFormUpdateModel dbModel = new MissionReportFormUpdateModel();

            var MissionReport = DbIMS.dataMissionReportForm.Where(x => x.MissionReportFormGUID == PK).FirstOrDefault();
            var dbMissionReport = DbIMS.Entry(MissionReport).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbMissionReport, dbModel);

            //var Language = DbIMS.codeWarehouseMissionReportLanguage.Where(x => x.MissionReportFormGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataMissionReportForm.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            //var dbLanguage = DbIMS.Entry(Language).GetDatabaseValues().ToObject();
            //dbModel = Mapper.Map(dbLanguage, dbModel);

            //if (MissionReport.codeWarehouseMissionReportRowVersion.SequenceEqual(dbModel.codeWarehouseMissionReportRowVersion) && Language.codeWarehouseMissionReportLanguageRowVersion.SequenceEqual(dbModel.codeWarehouseMissionReportLanguageRowVersion))
            //{
            //    return Json(DbIMS.PermissionError());
            //}

            return Json(JsonMessages.ConcurrencyError(DbIMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveMissionReportForm(Object model)
        {
            dataMissionReportForm MissionReport = Mapper.Map(model, new dataMissionReportForm());
            if (MissionReport.MissionStartDate == null || MissionReport.MissionEndDate == null)
            {
                ModelState.AddModelError("ModelDescription", "Mission Start Date and Mission End Date required");
                return true;
            }
            if ((MissionReport.longitude.Value.ToString().Length < 5) || (MissionReport.Latitude.Value.ToString().Length < 5))
            {
                ModelState.AddModelError("ModelDescription", "longitude or Latitude is wrong ");
                return true;
            }
            if (MissionReport.MissionStartDate > MissionReport.MissionEndDate)
            {
                ModelState.AddModelError("ModelDescription", "Start Date must be bigger than End date");
                return true;
            }
            if ((MissionReport.MissionStartDate > DateTime.Now || MissionReport.MissionEndDate > DateTime.Now) && MissionReport.MissionStatusGUID.ToString().ToLower() == "0c44822f-a898-476d-b291-caf1b017aac6")
            {
                ModelState.AddModelError("ModelDescription", "Check Mission Dates (future dates) while mission status is completed");
                return true;
            }

            int missionReport = DbIMS.dataMissionReportForm
                                    .Where(x => x.MissionStartDate == MissionReport.MissionStartDate &&
                                                x.CreatedDate == null &&
                                                x.Active).Count();
            if (missionReport > 0)
            {
                ModelState.AddModelError("ModelDescription", "MissionReport is already exists");
            }
            return (missionReport > 0);
        }

        #endregion

        #region Mission Validate

        public JsonResult ValidateMissionData(Guid communityGUID, DateTime MissionStartDate, DateTime missionEndDate, Guid missionStatusGUID)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var checkMission = DbIMS.dataMissionReportForm
                  .Where(x => x.CommunityGUID == communityGUID && x.MissionStartDate == MissionStartDate).Select(x => new MissionReportFormDataTableModel
                  {
                      MissionCode = x.MissionCode,
                      MissionReportFormGUID = x.MissionReportFormGUID

                  })
                  .FirstOrDefault();


            if ((MissionStartDate > DateTime.Now || missionEndDate > DateTime.Now) && missionStatusGUID == Guid.Parse("0C44822F-A898-476D-B291-CAF1B017AAC6"))
            {
                return Json(new { success = -2 });

            }

            else if (checkMission != null)
            {
                return Json(new { success = 1, checkMission = checkMission });
            }

            else
            {
                return Json(new { success = 0 });
            }
        }
        #endregion

        #region Mission Information
        [HttpPost]
        public JsonResult GetLocationCoordinates(Guid id)
        {
            var location = DbIMS.codeLocations.Where(x => x.LocationGUID == id).FirstOrDefault();
            return Json(new { Longitude = location.Longitude, Latitude = location.Latitude }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportMissionReportForm(int? type)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            string header1 = "";
            string header2 = "";
            if (type == 7)
            {
                header1 = "Mission Reports";
                header2 = ": ";
                string sourceFile = Server.MapPath("~/Areas/IMS/Templates/MissionCategoryForRep.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/IMS/temp/MissionReportRepOffice" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();

                    var results = DbIMS.v_trackMissionByCategoryING.ToList().Distinct().ToList();

                    dt.Columns.Add(" Mission Code", typeof(string));
                    dt.Columns.Add("Participants/staff member", typeof(string));
                    dt.Columns.Add("Type of mission", typeof(string));
                    dt.Columns.Add("Detailed purpose of the mission", typeof(string));
                    dt.Columns.Add("MissionStartDate", typeof(string));
                    dt.Columns.Add("Mission Month", typeof(string));
                    dt.Columns.Add("Mission Year", typeof(string));
                    dt.Columns.Add("MissionStatus", typeof(string));
                    dt.Columns.Add("Governorate", typeof(string));
                    dt.Columns.Add("Community", typeof(string));
                    foreach (var item in results)
                    {
                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.MissionCode;
                        dr[1] = item.Participants_staff_member;
                        dr[2] = item.Type_of_mission_;
                        dr[3] = item.Detailed_purpose_of_the_mission_;
                        dr[4] = item.MissionStartDate;
                        dr[5] = item.MonthMissionStartDate;
                        dr[6] = item.YearMissionStartDate;
                        dr[7] = item.MissionStatus;
                        dr[8] = item.Governorate;
                        dr[9] = item.Community;



                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["B8"].LoadFromDataTable(dt, true);
                    workSheet.Cells["B1"].Value = header1;
                    workSheet.Cells["B2"].Value = header2;
                    workSheet.Cells["B3"].Value = "Report Excecution Time: " + DateTime.Now.ToString("MMMM dd, yyyy hh:ss:mm tt");



                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = DateTime.Now.ToString("yyMMdd") + "_MissionReport " + DateTime.Now.ToBinary() + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }

            else if (type == 0)
            {
                header1 = "Mission Reports";
                header2 = ": ";
                string sourceFile = Server.MapPath("~/Areas/IMS/Templates/IMS_MissionReportForm.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/IMS/temp/MissionReport_Overview" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();



                    header1 = "Data Missions";

                    header2 = "List of Missions Report: ";

                    var results = DbIMS.IMS_vMissionReportForm.ToList();
                    dt.Columns.Add("Mission Code", typeof(string));
                    dt.Columns.Add("Mission Status", typeof(string));
                    dt.Columns.Add("MissionStrtDate", typeof(DateTime));
                    dt.Columns.Add("MissionEndDate", typeof(DateTime));
                    dt.Columns.Add("DutyStation", typeof(string));
                    dt.Columns.Add("Governorate", typeof(string));
                    dt.Columns.Add("District", typeof(string));
                    dt.Columns.Add("SubDistrict", typeof(string));
                    dt.Columns.Add("Community", typeof(string));
                    dt.Columns.Add("Address", typeof(string));
                    dt.Columns.Add("Longitude", typeof(string));
                    dt.Columns.Add("Latitude", typeof(string));
                    dt.Columns.Add("CreatedBy", typeof(string));
                    dt.Columns.Add("CreatedDate", typeof(string));
                    dt.Columns.Add("Is_AnyPresence_Other_Organization_", typeof(string));

                    dt.Columns.Add("Gaps", typeof(string));
                    dt.Columns.Add("Recommendations", typeof(string));
                    dt.Columns.Add("ActionTaken", typeof(string));
                    dt.Columns.Add("ActionRequired", typeof(string));
                    dt.Columns.Add("LinkToMissionReport", typeof(string));

                    dt.Columns.Add("Is_There_Mission_Report_", typeof(string));
                    dt.Columns.Add("Departments", typeof(string));
                    dt.Columns.Add("Members", typeof(string));
                    dt.Columns.Add("VisitObjectivies", typeof(string));
                    dt.Columns.Add("Humanitarian Needs", typeof(string));
                    dt.Columns.Add("OngoingResponse", typeof(string));
                    dt.Columns.Add("FormChallenge ", typeof(string));

                    dt.Columns.Add("Coordinates ", typeof(string));


                    var departments = DbIMS.dataMissionReportDepartment.ToList();

                    var members = DbIMS.dataMissionReportFormMember.ToList();
                    var visitsFormVisitObjectives = DbIMS.dataMissionReportFormVisitObjective.ToList();
                    var MissionReportFormHumanitarianNeed = DbIMS.dataMissionReportFormHumanitarianNeed.ToList();
                    var MissionReportFormOngoingResponse = DbIMS.dataMissionReportFormOngoingResponse.ToList();
                    var MissionReportFormChallenge = DbIMS.dataMissionReportFormChallenge.ToList();

                    foreach (var item in results)
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
                        dr[4] = item.FromOffice;
                        dr[5] = item.Governorate;
                        dr[6] = item.District;
                        dr[7] = item.SubDistrict;
                        dr[8] = item.Community;
                        dr[9] = item.Address;
                        dr[10] = item.Longitude;
                        dr[11] = item.Latitude;
                        dr[12] = item.CreatedBy;
                        dr[13] = item.CreatedDate;
                        dr[14] = item.Is_AnyPresence_Other_Organization_;
                        dr[15] = item.Gaps;
                        dr[16] = item.Recommendations;
                        dr[16] = item.ActionTaken;
                        dr[17] = item.ActionRequired;
                        dr[18] = item.LinkToMissionReport;
                        dr[19] = item.Is_There_Mission_Report_;
                        if (departments.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID).Count() > 0)
                        {
                            foreach (var department in departments.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID))
                            {
                                currentDepartment += department.codeDepartments.codeDepartmentsLanguages.Where(x => x.LanguageID == LAN).Select(x => x.DepartmentDescription).FirstOrDefault()
                                                    ;
                            }
                            dr[20] = currentDepartment;
                        }
                        else
                        {
                            dr[20] = "";

                        }

                        if (members.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID).Count() > 0)
                        {
                            foreach (var member in members.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID))
                            {
                                currentmember += member.userAccounts.userPersonalDetails.userPersonalDetailsLanguage
                                                     .Select(x => x.FirstName).FirstOrDefault() +
                                                 member.userAccounts.userPersonalDetails.userPersonalDetailsLanguage
                                                     .Select(x => x.Surname).FirstOrDefault();
                            }
                            dr[21] = currentmember;
                        }
                        else
                        {
                            dr[21] = "";

                        }

                        if (visitsFormVisitObjectives.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID).Count() > 0)
                        {
                            foreach (var visit in visitsFormVisitObjectives.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID))
                            {
                                currentVisit += visit.VisitObjectiveName;
                            }
                            dr[22] = currentVisit;
                        }
                        else
                        {
                            dr[22] = "";

                        }
                        if (MissionReportFormHumanitarianNeed.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID).Count() > 0)
                        {
                            foreach (var visit in MissionReportFormHumanitarianNeed.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID))
                            {
                                HumanitarianNeed += visit.HumanitarianNeedName;
                            }
                            dr[23] = HumanitarianNeed;
                        }
                        else
                        {
                            dr[23] = "";

                        }

                        if (MissionReportFormOngoingResponse.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID).Count() > 0)
                        {
                            foreach (var visit in MissionReportFormOngoingResponse.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID))
                            {
                                OngoingResponse += visit.OngoingResponseName;
                            }
                            dr[24] = OngoingResponse;
                        }
                        else
                        {
                            dr[24] = "";

                        }
                        if (MissionReportFormChallenge.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID).Count() > 0)
                        {
                            foreach (var visit in MissionReportFormChallenge.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID))
                            {
                                FormChallenge += visit.ChallengeName;
                            }
                            dr[25] = FormChallenge;
                        }
                        else
                        {
                            dr[25] = "";

                        }


                        dr[26] = item.Coordinates;

                        //dr[1] = item.dataCycleProcessGroupIndividuals.Count();
                        //dr[2] = item.codeCaseCategory.Name;
                        //dr[3] = item.dataProcessGroup.codeProcessStatu.Name;
                        //dr[4] = item.codeCountry.Name;
                        //dr[5] = item.configLocationLevel.LocationLevelID;
                        //dr[6] = item.codeRefugeeStatu.Name;
                        //dr[7] = item.dataProcessGroup.codeSite.Name;
                        dt.Rows.Add(dr);

                    }
                    workSheet.Cells["B8"].LoadFromDataTable(dt, true);
                    workSheet.Cells["B1"].Value = header1;
                    workSheet.Cells["B2"].Value = header2;
                    workSheet.Cells["B3"].Value = "Report Excecution Time: " + DateTime.Now.ToString("MMMM dd, yyyy hh:ss:mm tt");



                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = DateTime.Now.ToString("yyMMdd") + "_MissionReport " + DateTime.Now.ToBinary() + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            else if (type == 2)
            {
                header1 = "Data Missions";

                header2 = "List of Visits Report: ";
                string sourceFile = Server.MapPath("~/Areas/IMS/Templates/dataViews.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/IMS/temp/MissionReport_View" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();




                    //var results = DbIMS.dataMissionReportFormVisitObjective.ToList();
                    var myResults = DbIMS.dataMissionReportFormVisitObjective.ToList().Select(x => new
                    {
                        CorrectedMissionStartDate = x.dataMissionReportForm.MissionStartDate.Date,
                        VisitObjectiveDescription = x.codeIMSVisitObjective.codeIMSVisitObjectiveLanguage.Where(f => f.LanguageID == LAN)
                            .Select(fx => fx.VisitObjectiveDescription).FirstOrDefault(),
                        x.dataMissionReportForm.longitude,
                        x.dataMissionReportForm.Latitude
                        ,
                        Corrdinates = x.dataMissionReportForm.longitude + " " + x.dataMissionReportForm.Latitude
                    }).Distinct().ToList();
                    dt.Columns.Add(" Name", typeof(string));
                    dt.Columns.Add("Mission Start Date", typeof(DateTime));
                    dt.Columns.Add("longitude", typeof(string));
                    dt.Columns.Add("Latitude", typeof(string));
                    dt.Columns.Add("Corrdinates", typeof(string));



                    foreach (var item in myResults)
                    {

                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.VisitObjectiveDescription;
                        dr[1] = item.CorrectedMissionStartDate;
                        dr[2] = item.longitude;
                        dr[3] = item.Latitude;
                        dr[4] = item.Corrdinates;


                        dt.Rows.Add(dr);

                    }
                    workSheet.Cells["B8"].LoadFromDataTable(dt, true);
                    workSheet.Cells["B1"].Value = header1;
                    workSheet.Cells["B2"].Value = header2;
                    workSheet.Cells["B3"].Value = "Report Excecution Time: " + DateTime.Now.ToString("MMMM dd, yyyy hh:ss:mm tt");



                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = DateTime.Now.ToString("yyMMdd") + "_MissionReport " + DateTime.Now.ToBinary() + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            else if (type == 3)
            {

                string sourceFile = Server.MapPath("~/Areas/IMS/Templates/dataViews.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/IMS/temp/MissionReport_View_Humanitarian" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();

                    //myReportNam = "dataViews.xlsx";
                    header1 = "Data Humanitarian";
                    header2 = "View of Humanitarian Need Report: ";
                    var myResults = DbIMS.dataMissionReportFormHumanitarianNeed.ToList().Select(x => new
                    {
                        MissionStartDate = x.dataMissionReportForm.MissionStartDate.Date,
                        HumanitarianNeedeDescription = x.codeIMSHumanitarianNeed.codeIMSHumanitarianNeedLanguage.Where(f => f.LanguageID == LAN)
                            .Select(fx => fx.HumanitarianNeedeDescription).FirstOrDefault(),
                        x.dataMissionReportForm.longitude,
                        x.dataMissionReportForm.Latitude
                        ,
                        Corrdinates = x.dataMissionReportForm.longitude + " " + x.dataMissionReportForm.Latitude
                    }).Distinct().ToList();
                    dt.Columns.Add("Name", typeof(string));
                    dt.Columns.Add("Mission Start Date", typeof(DateTime));
                    dt.Columns.Add("longitude", typeof(string));
                    dt.Columns.Add("Latitude", typeof(string));
                    dt.Columns.Add("Corrdinates", typeof(string));



                    foreach (var item in myResults)
                    {
                        string currentDepartment = "";
                        string currentmember = "";
                        string currentVisit = "";
                        string HumanitarianNeed = "";
                        string OngoingResponse = "";
                        string FormChallenge = "";
                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.HumanitarianNeedeDescription;
                        dr[1] = item.MissionStartDate;
                        dr[2] = item.longitude;
                        dr[3] = item.Latitude;
                        dr[4] = item.Corrdinates;


                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["B8"].LoadFromDataTable(dt, true);
                    workSheet.Cells["B1"].Value = header1;
                    workSheet.Cells["B2"].Value = header2;
                    workSheet.Cells["B3"].Value = "Report Excecution Time: " + DateTime.Now.ToString("MMMM dd, yyyy hh:ss:mm tt");



                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = DateTime.Now.ToString("yyMMdd") + "_MissionReport " + DateTime.Now.ToBinary() + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }

            else if (type == 4)
            {

                string sourceFile = Server.MapPath("~/Areas/IMS/Templates/dataViews.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/IMS/temp/MissionReport_View_OngoingResponce" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();

                    //myReportNam = "dataViews.xlsx";

                    header1 = "Data Ongoing Response";
                    header2 = "View of Ongoing Response  Report: ";
                    var results = DbIMS.dataMissionReportFormOngoingResponse.Where(x => x.OngoingResponseGUID != null).ToList().Select(x => new
                    {
                        MissionStartDate = x.dataMissionReportForm.MissionStartDate.Date,
                        OngoingResponseDescription = x.OngoingResponseGUID != null ? x.codeIMSOngoingResponse.codeIMSOngoingResponseLanguage.Where(f => f.LanguageID == LAN)
                                .Select(fx => fx.OngoingResponseDescription).FirstOrDefault() : null,
                        x.dataMissionReportForm.longitude,
                        x.dataMissionReportForm.Latitude
                            ,
                        Corrdinates = x.dataMissionReportForm.longitude + " " + x.dataMissionReportForm.Latitude
                    }).Distinct().ToList();
                    dt.Columns.Add(" Name", typeof(string));
                    dt.Columns.Add("Mission Start Date", typeof(DateTime));
                    dt.Columns.Add("longitude", typeof(string));
                    dt.Columns.Add("Latitude", typeof(string));
                    dt.Columns.Add("Corrdinates", typeof(string));



                    foreach (var item in results)
                    {
                        string currentDepartment = "";
                        string currentmember = "";
                        string currentVisit = "";
                        string HumanitarianNeed = "";
                        string OngoingResponse = "";
                        string FormChallenge = "";
                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.OngoingResponseDescription;
                        dr[1] = item.MissionStartDate;
                        dr[2] = item.longitude;
                        dr[3] = item.Latitude;
                        dr[4] = item.Corrdinates;


                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["B8"].LoadFromDataTable(dt, true);
                    workSheet.Cells["B1"].Value = header1;
                    workSheet.Cells["B2"].Value = header2;
                    workSheet.Cells["B3"].Value = "Report Excecution Time: " + DateTime.Now.ToString("MMMM dd, yyyy hh:ss:mm tt");



                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = DateTime.Now.ToString("yyMMdd") + "_MissionReport " + DateTime.Now.ToBinary() + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }

            else if (type == 5)
            {

                string sourceFile = Server.MapPath("~/Areas/IMS/Templates/dataViews.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/IMS/temp/MissionReport_View_VisitChallengs" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();

                    //myReportNam = "dataViews.xlsx";


                    header1 = "Data Form Challenge";
                    header2 = "View of Visit Challenge  Report: ";
                    var results = DbIMS.dataMissionReportFormChallenge.Where(x => x.MissionChallengeGUID != null).ToList().Select(x => new
                    {
                        MissionStartDate = x.dataMissionReportForm.MissionStartDate.Date,
                        MissionChallengeDescription = x.codeIMSMissionChallenge.codeIMSMissionChallengeLanguage.Where(f => f.LanguageID == LAN)
                                .Select(fx => fx.MissionChallengeDescription).FirstOrDefault(),
                        x.dataMissionReportForm.longitude,
                        x.dataMissionReportForm.Latitude,
                        Corrdinates = x.dataMissionReportForm.longitude + " " + x.dataMissionReportForm.Latitude
                    }).Distinct().ToList();
                    dt.Columns.Add(" Name", typeof(string));
                    dt.Columns.Add("Mission Start Date", typeof(DateTime));
                    dt.Columns.Add("longitude", typeof(string));
                    dt.Columns.Add("Latitude", typeof(string));
                    dt.Columns.Add("Corrdinates", typeof(string));



                    foreach (var item in results)
                    {
                        string currentDepartment = "";
                        string currentmember = "";
                        string currentVisit = "";
                        string HumanitarianNeed = "";
                        string OngoingResponse = "";
                        string FormChallenge = "";
                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.MissionChallengeDescription;
                        dr[1] = item.MissionStartDate;
                        dr[2] = item.longitude;
                        dr[3] = item.Latitude;
                        dr[4] = item.Corrdinates;


                        dt.Rows.Add(dr);
                    }

                    workSheet.Cells["B8"].LoadFromDataTable(dt, true);
                    workSheet.Cells["B1"].Value = header1;
                    workSheet.Cells["B2"].Value = header2;
                    workSheet.Cells["B3"].Value = "Report Excecution Time: " + DateTime.Now.ToString("MMMM dd, yyyy hh:ss:mm tt");



                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = DateTime.Now.ToString("yyMMdd") + "_MissionReport " + DateTime.Now.ToBinary() + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }

            else if (type == 6)
            {

                string sourceFile = Server.MapPath("~/Areas/IMS/Templates/dataViews.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/IMS/temp/MissionReport_View_VisitChallengs" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();

                    //myReportNam = "dataViews.xlsx";


                    header1 = "Data  Departments";
                    header2 = "View of Departments  Report: ";
                    var results = DbIMS.dataMissionReportDepartment.Where(x => x.DepartmentGUID != null).ToList().Select(x => new
                    {
                        MissionStartDate = x.dataMissionReportForm.MissionStartDate.Date,
                        DepartmentDescription = x.codeDepartments.codeDepartmentsLanguages.Where(f => f.LanguageID == LAN)
                                .Select(fx => fx.DepartmentDescription).FirstOrDefault(),
                        x.dataMissionReportForm.longitude,
                        x.dataMissionReportForm.Latitude,
                        Corrdinates = x.dataMissionReportForm.longitude + " " + x.dataMissionReportForm.Latitude
                    }).Distinct().ToList();



                    dt.Columns.Add(" Name", typeof(string));
                    dt.Columns.Add("Mission Start Date", typeof(DateTime));
                    dt.Columns.Add("longitude", typeof(string));
                    dt.Columns.Add("Latitude", typeof(string));
                    dt.Columns.Add("Corrdinates", typeof(string));



                    foreach (var item in results)
                    {
                        string currentDepartment = "";
                        string currentmember = "";
                        string currentVisit = "";
                        string HumanitarianNeed = "";
                        string OngoingResponse = "";
                        string FormChallenge = "";
                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.DepartmentDescription;
                        dr[1] = item.MissionStartDate;
                        dr[2] = item.longitude;
                        dr[3] = item.Latitude;
                        dr[4] = item.Corrdinates;


                        dt.Rows.Add(dr);
                    }

                    workSheet.Cells["B8"].LoadFromDataTable(dt, true);
                    workSheet.Cells["B1"].Value = header1;
                    workSheet.Cells["B2"].Value = header2;
                    workSheet.Cells["B3"].Value = "Report Excecution Time: " + DateTime.Now.ToString("MMMM dd, yyyy hh:ss:mm tt");



                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = DateTime.Now.ToString("yyMMdd") + "_MissionReport " + DateTime.Now.ToBinary() + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }

            string success = "No Data Available for this items";
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);

        }


        //public FileInfo ExportMissionReportForm(int? id)
        //{

        //    FileInfo file = GetReportForm((int)id);

        //    return file;
        //}

        public ActionResult ExportMissionReportFormExcel(int? id)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var result = DbIMS.IMS_vMissionReportForm.ToList();


            if (result.Count > 0)
            {
                string sourceFile = Server.MapPath("~/Areas/IMS/ExcelTemplates/IMS_MissionReportForm.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/IMS/temp/IMS_MissionReportForm" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Mission Code", typeof(string));
                    dt.Columns.Add("Mission Status", typeof(string));
                    dt.Columns.Add("MissionStartDate", typeof(string));
                    dt.Columns.Add("MissionEndDate", typeof(string));
                    dt.Columns.Add("MissionMonth", typeof(string));
                    dt.Columns.Add("MissionYear", typeof(string));
                    dt.Columns.Add("DutyStation", typeof(string));
                    dt.Columns.Add("Governorate", typeof(string));
                    dt.Columns.Add("District", typeof(string));
                    dt.Columns.Add("SubDistrict", typeof(string));
                    dt.Columns.Add("Community", typeof(string));
                    dt.Columns.Add("Address", typeof(string));//10
                    dt.Columns.Add("Longitude", typeof(string));
                    dt.Columns.Add("Latitude", typeof(string));
                    dt.Columns.Add("CreatedBy", typeof(string));
                    dt.Columns.Add("CreatedDate", typeof(string));
                    dt.Columns.Add("Is_AnyPresence_Other_Organization_", typeof(string));

                    dt.Columns.Add("Gaps", typeof(string));
                    dt.Columns.Add("Recommendations", typeof(string));
                    dt.Columns.Add("ActionTaken", typeof(string));
                    dt.Columns.Add("ActionRequired", typeof(string));
                    dt.Columns.Add("LinkToMissionReport", typeof(string));

                    dt.Columns.Add("Is_There_Mission_Report_", typeof(string));
                    dt.Columns.Add("Departments", typeof(string));
                    dt.Columns.Add("Members", typeof(string));
                    dt.Columns.Add("VisitObjectivies", typeof(string));
                    dt.Columns.Add("Humanitarian Needs", typeof(string));
                    dt.Columns.Add("OngoingResponse", typeof(string));
                    dt.Columns.Add("FormChallenge ", typeof(string));
                    dt.Columns.Add("Coordinates ", typeof(string));


                    var departments = DbIMS.dataMissionReportDepartment.ToList();

                    var members = DbIMS.dataMissionReportFormMember.ToList();
                    var visitsFormVisitObjectives = DbIMS.dataMissionReportFormVisitObjective.ToList();
                    var MissionReportFormHumanitarianNeed = DbIMS.dataMissionReportFormHumanitarianNeed.ToList();
                    var MissionReportFormOngoingResponse = DbIMS.dataMissionReportFormOngoingResponse.ToList();
                    var MissionReportFormChallenge = DbIMS.dataMissionReportFormChallenge.ToList();
                    foreach (var item in result)
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
                        dr[4] = item.MonthMissionStartDate;
                        dr[5] = item.YearMissionStartDate;
                        dr[6] = item.FromOffice;
                        dr[7] = item.Governorate;
                        dr[8] = item.District;
                        dr[9] = item.SubDistrict;
                        dr[10] = item.Community;
                        dr[11] = item.Address;//10
                        dr[12] = item.Longitude;
                        dr[13] = item.Latitude;
                        dr[14] = item.CreatedBy;
                        dr[15] = item.CreatedDate;
                        dr[16] = item.Is_AnyPresence_Other_Organization_;
                        dr[17] = item.Gaps;
                        dr[18] = item.Recommendations;
                        dr[19] = item.ActionTaken;
                        dr[20] = item.ActionRequired;
                        dr[21] = item.LinkToMissionReport;
                        dr[22] = item.Is_There_Mission_Report_;
                        if (departments.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID).Count() > 0)
                        {
                            foreach (var department in departments.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID))
                            {
                                currentDepartment += department.codeDepartments.codeDepartmentsLanguages.Where(x => x.LanguageID == LAN).Select(x => x.DepartmentDescription).FirstOrDefault()
                                                    ;
                            }
                            dr[23] = currentDepartment;
                        }
                        else
                        {
                            dr[23] = "";

                        }

                        if (members.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID).Count() > 0)
                        {
                            foreach (var member in members.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID))
                            {
                                if (member != null && member.userAccounts != null)
                                    currentmember += member.userAccounts.userPersonalDetails.userPersonalDetailsLanguage
                                                         .Select(x => x.FirstName).FirstOrDefault() +
                                                     member.userAccounts.userPersonalDetails.userPersonalDetailsLanguage
                                                         .Select(x => x.Surname).FirstOrDefault();
                            }
                            dr[24] = currentmember;
                        }
                        else
                        {
                            dr[24] = "";

                        }

                        if (visitsFormVisitObjectives.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID).Count() > 0)
                        {
                            foreach (var visit in visitsFormVisitObjectives.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID))
                            {
                                currentVisit += visit.VisitObjectiveName;
                            }
                            dr[25] = currentVisit;
                        }
                        else
                        {
                            dr[25] = "";

                        }
                        if (MissionReportFormHumanitarianNeed.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID).Count() > 0)
                        {
                            foreach (var visit in MissionReportFormHumanitarianNeed.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID))
                            {
                                HumanitarianNeed += visit.HumanitarianNeedName;
                            }
                            dr[26] = HumanitarianNeed;
                        }
                        else
                        {
                            dr[26] = "";

                        }

                        if (MissionReportFormOngoingResponse.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID).Count() > 0)
                        {
                            foreach (var visit in MissionReportFormOngoingResponse.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID))
                            {
                                OngoingResponse += visit.OngoingResponseName;
                            }
                            dr[27] = OngoingResponse;
                        }
                        else
                        {
                            dr[27] = "";

                        }
                        if (MissionReportFormChallenge.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID).Count() > 0)
                        {
                            foreach (var visit in MissionReportFormChallenge.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID))
                            {
                                FormChallenge += visit.ChallengeName;
                            }
                            dr[28] = FormChallenge;
                        }
                        else
                        {
                            dr[28] = "";

                        }


                        dr[29] = item.Coordinates;

                        //dr[1] = item.dataCycleProcessGroupIndividuals.Count();
                        //dr[2] = item.codeCaseCategory.Name;
                        //dr[3] = item.dataProcessGroup.codeProcessStatu.Name;
                        //dr[4] = item.codeCountry.Name;
                        //dr[5] = item.configLocationLevel.LocationLevelID;
                        //dr[6] = item.codeRefugeeStatu.Name;
                        //dr[7] = item.dataProcessGroup.codeSite.Name;
                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["B8"].LoadFromDataTable(dt, true);
                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);
                string fileName = "MissionTrack " + DateTime.Now.ToString("dddd, dd MMMM yyyy") + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }

            bool success = false;
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);

        }

        //public FileInfo GetReportForm(int type)
        //{
        //    DataSourceSelectArguments args = new DataSourceSelectArguments();
        //    string myReportNam = "";
        //    string header1 = "";
        //    string header2 = "";
        //    DataTable dt = new DataTable();

        //    if (type == 0)

        //    {

        //        myReportNam = "IMS_MissionReportForm.xlsx";

        //        header1 = "Data Missions";

        //        header2 = "List of Missions Report: ";

        //        var results = DbIMS.IMS_vMissionReportForm.ToList();
        //        dt.Columns.Add("Mission Code", typeof(string));
        //        dt.Columns.Add("Mission Status", typeof(string));
        //        dt.Columns.Add("MissionStrtDate", typeof(DateTime));
        //        dt.Columns.Add("MissionEndDate", typeof(DateTime));
        //        dt.Columns.Add("DutyStation", typeof(string));
        //        dt.Columns.Add("Governorate", typeof(string));
        //        dt.Columns.Add("District", typeof(string));
        //        dt.Columns.Add("SubDistrict", typeof(string));
        //        dt.Columns.Add("Community", typeof(string));
        //        dt.Columns.Add("Address", typeof(string));
        //        dt.Columns.Add("Longitude", typeof(string));
        //        dt.Columns.Add("Latitude", typeof(string));
        //        dt.Columns.Add("CreatedBy", typeof(string));
        //        dt.Columns.Add("CreatedDate", typeof(string));
        //        dt.Columns.Add("Is_AnyPresence_Other_Organization_", typeof(string));

        //        dt.Columns.Add("Gaps", typeof(string));
        //        dt.Columns.Add("Recommendations", typeof(string));
        //        dt.Columns.Add("ActionTaken", typeof(string));
        //        dt.Columns.Add("ActionRequired", typeof(string));
        //        dt.Columns.Add("LinkToMissionReport", typeof(string));

        //        dt.Columns.Add("Is_There_Mission_Report_", typeof(string));
        //        dt.Columns.Add("Departments", typeof(string));
        //        dt.Columns.Add("Members", typeof(string));
        //        dt.Columns.Add("VisitObjectivies", typeof(string));
        //        dt.Columns.Add("Humanitarian Needs", typeof(string));
        //        dt.Columns.Add("OngoingResponse", typeof(string));
        //        dt.Columns.Add("FormChallenge ", typeof(string));

        //        dt.Columns.Add("Coordinates ", typeof(string));


        //        var departments = DbIMS.dataMissionReportDepartment.ToList();

        //        var members = DbIMS.dataMissionReportFormMember.ToList();
        //        var visitsFormVisitObjectives = DbIMS.dataMissionReportFormVisitObjective.ToList();
        //        var MissionReportFormHumanitarianNeed = DbIMS.dataMissionReportFormHumanitarianNeed.ToList();
        //        var MissionReportFormOngoingResponse = DbIMS.dataMissionReportFormOngoingResponse.ToList();
        //        var MissionReportFormChallenge = DbIMS.dataMissionReportFormChallenge.ToList();

        //        foreach (var item in results)
        //        {
        //            string currentDepartment = "";
        //            string currentmember = "";
        //            string currentVisit = "";
        //            string HumanitarianNeed = "";
        //            string OngoingResponse = "";
        //            string FormChallenge = "";
        //            DataRow dr;
        //            dr = dt.NewRow();
        //            dr[0] = item.MissionCode;
        //            dr[1] = item.MissionStatus;
        //            dr[2] = item.MissionStartDate;
        //            dr[3] = item.MissionEndDate;
        //            dr[4] = item.FromOffice;
        //            dr[5] = item.Governorate;
        //            dr[6] = item.District;
        //            dr[7] = item.SubDistrict;
        //            dr[8] = item.Community;
        //            dr[9] = item.Address;
        //            dr[10] = item.Longitude;
        //            dr[11] = item.Latitude;
        //            dr[12] = item.CreatedBy;
        //            dr[13] = item.CreatedDate;
        //            dr[14] = item.Is_AnyPresence_Other_Organization_;
        //            dr[15] = item.Gaps;
        //            dr[16] = item.Recommendations;
        //            dr[16] = item.ActionTaken;
        //            dr[17] = item.ActionRequired;
        //            dr[18] = item.LinkToMissionReport;
        //            dr[19] = item.Is_There_Mission_Report_;
        //            if (departments.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID).Count() > 0)
        //            {
        //                foreach (var department in departments.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID))
        //                {
        //                    currentDepartment += department.codeDepartments.codeDepartmentsLanguages.Where(x => x.LanguageID == LAN).Select(x => x.DepartmentDescription).FirstOrDefault()
        //                                        ;
        //                }
        //                dr[20] = currentDepartment;
        //            }
        //            else
        //            {
        //                dr[20] = "";

        //            }

        //            if (members.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID).Count() > 0)
        //            {
        //                foreach (var member in members.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID))
        //                {
        //                    currentmember += member.userAccounts.userPersonalDetails.userPersonalDetailsLanguage
        //                                         .Select(x => x.FirstName).FirstOrDefault() +
        //                                     member.userAccounts.userPersonalDetails.userPersonalDetailsLanguage
        //                                         .Select(x => x.Surname).FirstOrDefault();
        //                }
        //                dr[21] = currentmember;
        //            }
        //            else
        //            {
        //                dr[21] = "";

        //            }

        //            if (visitsFormVisitObjectives.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID).Count() > 0)
        //            {
        //                foreach (var visit in visitsFormVisitObjectives.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID))
        //                {
        //                    currentVisit += visit.VisitObjectiveName;
        //                }
        //                dr[22] = currentVisit;
        //            }
        //            else
        //            {
        //                dr[22] = "";

        //            }
        //            if (MissionReportFormHumanitarianNeed.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID).Count() > 0)
        //            {
        //                foreach (var visit in MissionReportFormHumanitarianNeed.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID))
        //                {
        //                    HumanitarianNeed += visit.HumanitarianNeedName;
        //                }
        //                dr[23] = HumanitarianNeed;
        //            }
        //            else
        //            {
        //                dr[23] = "";

        //            }

        //            if (MissionReportFormOngoingResponse.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID).Count() > 0)
        //            {
        //                foreach (var visit in MissionReportFormOngoingResponse.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID))
        //                {
        //                    OngoingResponse += visit.OngoingResponseName;
        //                }
        //                dr[24] = OngoingResponse;
        //            }
        //            else
        //            {
        //                dr[24] = "";

        //            }
        //            if (MissionReportFormChallenge.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID).Count() > 0)
        //            {
        //                foreach (var visit in MissionReportFormChallenge.Where(x => x.MissionReportFormGUID == item.MissionReportFormGUID))
        //                {
        //                    FormChallenge += visit.ChallengeName;
        //                }
        //                dr[25] = FormChallenge;
        //            }
        //            else
        //            {
        //                dr[25] = "";

        //            }


        //            dr[26] = item.Coordinates;

        //            //dr[1] = item.dataCycleProcessGroupIndividuals.Count();
        //            //dr[2] = item.codeCaseCategory.Name;
        //            //dr[3] = item.dataProcessGroup.codeProcessStatu.Name;
        //            //dr[4] = item.codeCountry.Name;
        //            //dr[5] = item.configLocationLevel.LocationLevelID;
        //            //dr[6] = item.codeRefugeeStatu.Name;
        //            //dr[7] = item.dataProcessGroup.codeSite.Name;
        //            dt.Rows.Add(dr);
        //        }
        //    }


        //    if (type == 2)

        //    {

        //        myReportNam = "dataViews.xlsx";

        //        header1 = "Data Missions";

        //        header2 = "List of Visits Report: ";
        //        //var results = DbIMS.dataMissionReportFormVisitObjective.ToList();
        //        var myResults = DbIMS.dataMissionReportFormVisitObjective.ToList().Select(x => new
        //        {
        //            CorrectedMissionStartDate = x.dataMissionReportForm.MissionStartDate.Date,
        //            VisitObjectiveDescription = x.codeIMSVisitObjective.codeIMSVisitObjectiveLanguage.Where(f => f.LanguageID == LAN)
        //                .Select(fx => fx.VisitObjectiveDescription).FirstOrDefault(),
        //            x.dataMissionReportForm.longitude,
        //            x.dataMissionReportForm.Latitude
        //            ,
        //            Corrdinates = x.dataMissionReportForm.longitude + " " + x.dataMissionReportForm.Latitude
        //        }).Distinct().ToList();
        //        dt.Columns.Add(" Name", typeof(string));
        //        dt.Columns.Add("Mission Start Date", typeof(DateTime));
        //        dt.Columns.Add("longitude", typeof(string));
        //        dt.Columns.Add("Latitude", typeof(string));
        //        dt.Columns.Add("Corrdinates", typeof(string));



        //        foreach (var item in myResults)
        //        {

        //            DataRow dr;
        //            dr = dt.NewRow();
        //            dr[0] = item.VisitObjectiveDescription;
        //            dr[1] = item.CorrectedMissionStartDate;
        //            dr[2] = item.longitude;
        //            dr[3] = item.Latitude;
        //            dr[4] = item.Corrdinates;


        //            dt.Rows.Add(dr);
        //        }
        //    }

        //    if (type == 3)

        //    {

        //        myReportNam = "dataViews.xlsx";

        //        header1 = "Data Humanitarian";

        //        header2 = "View of Humanitarian Need Report: ";
        //        var myResults = DbIMS.dataMissionReportFormHumanitarianNeed.ToList().Select(x => new
        //        {
        //            MissionStartDate = x.dataMissionReportForm.MissionStartDate.Date,
        //            HumanitarianNeedeDescription = x.codeIMSHumanitarianNeed.codeIMSHumanitarianNeedLanguage.Where(f => f.LanguageID == LAN)
        //                .Select(fx => fx.HumanitarianNeedeDescription).FirstOrDefault(),
        //            x.dataMissionReportForm.longitude,
        //            x.dataMissionReportForm.Latitude
        //            ,
        //            Corrdinates = x.dataMissionReportForm.longitude + " " + x.dataMissionReportForm.Latitude
        //        }).Distinct().ToList();
        //        dt.Columns.Add("Name", typeof(string));
        //        dt.Columns.Add("Mission Start Date", typeof(DateTime));
        //        dt.Columns.Add("longitude", typeof(string));
        //        dt.Columns.Add("Latitude", typeof(string));
        //        dt.Columns.Add("Corrdinates", typeof(string));



        //        foreach (var item in myResults)
        //        {
        //            string currentDepartment = "";
        //            string currentmember = "";
        //            string currentVisit = "";
        //            string HumanitarianNeed = "";
        //            string OngoingResponse = "";
        //            string FormChallenge = "";
        //            DataRow dr;
        //            dr = dt.NewRow();
        //            dr[0] = item.HumanitarianNeedeDescription;
        //            dr[1] = item.MissionStartDate;
        //            dr[2] = item.longitude;
        //            dr[3] = item.Latitude;
        //            dr[4] = item.Corrdinates;


        //            dt.Rows.Add(dr);
        //        }
        //    }

        //    if (type == 4)

        //    {

        //        myReportNam = "dataViews.xlsx";

        //        header1 = "Data Ongoing Response";

        //        header2 = "View of Ongoing Response  Report: ";


        //        var results = DbIMS.dataMissionReportFormOngoingResponse.Where(x => x.OngoingResponseGUID != null).ToList().Select(x => new
        //        {
        //            MissionStartDate = x.dataMissionReportForm.MissionStartDate.Date,
        //            OngoingResponseDescription = x.OngoingResponseGUID != null ? x.codeIMSOngoingResponse.codeIMSOngoingResponseLanguage.Where(f => f.LanguageID == LAN)
        //                    .Select(fx => fx.OngoingResponseDescription).FirstOrDefault() : null,
        //            x.dataMissionReportForm.longitude,
        //            x.dataMissionReportForm.Latitude
        //                ,
        //            Corrdinates = x.dataMissionReportForm.longitude + " " + x.dataMissionReportForm.Latitude
        //        }).Distinct().ToList();
        //        dt.Columns.Add(" Name", typeof(string));
        //        dt.Columns.Add("Mission Start Date", typeof(DateTime));
        //        dt.Columns.Add("longitude", typeof(string));
        //        dt.Columns.Add("Latitude", typeof(string));
        //        dt.Columns.Add("Corrdinates", typeof(string));



        //        foreach (var item in results)
        //        {
        //            string currentDepartment = "";
        //            string currentmember = "";
        //            string currentVisit = "";
        //            string HumanitarianNeed = "";
        //            string OngoingResponse = "";
        //            string FormChallenge = "";
        //            DataRow dr;
        //            dr = dt.NewRow();
        //            dr[0] = item.OngoingResponseDescription;
        //            dr[1] = item.MissionStartDate;
        //            dr[2] = item.longitude;
        //            dr[3] = item.Latitude;
        //            dr[4] = item.Corrdinates;


        //            dt.Rows.Add(dr);
        //        }
        //    }

        //    if (type == 5)

        //    {

        //        myReportNam = "dataViews.xlsx";

        //        header1 = "Data Form Challenge";

        //        header2 = "View of Visit Challenge  Report: ";


        //        var results = DbIMS.dataMissionReportFormChallenge.Where(x => x.MissionChallengeGUID != null).ToList().Select(x => new
        //        {
        //            MissionStartDate = x.dataMissionReportForm.MissionStartDate.Date,
        //            MissionChallengeDescription = x.codeIMSMissionChallenge.codeIMSMissionChallengeLanguage.Where(f => f.LanguageID == LAN)
        //                    .Select(fx => fx.MissionChallengeDescription).FirstOrDefault(),
        //            x.dataMissionReportForm.longitude,
        //            x.dataMissionReportForm.Latitude,
        //            Corrdinates = x.dataMissionReportForm.longitude + " " + x.dataMissionReportForm.Latitude
        //        }).Distinct().ToList();
        //        dt.Columns.Add(" Name", typeof(string));
        //        dt.Columns.Add("Mission Start Date", typeof(DateTime));
        //        dt.Columns.Add("longitude", typeof(string));
        //        dt.Columns.Add("Latitude", typeof(string));
        //        dt.Columns.Add("Corrdinates", typeof(string));



        //        foreach (var item in results)
        //        {
        //            string currentDepartment = "";
        //            string currentmember = "";
        //            string currentVisit = "";
        //            string HumanitarianNeed = "";
        //            string OngoingResponse = "";
        //            string FormChallenge = "";
        //            DataRow dr;
        //            dr = dt.NewRow();
        //            dr[0] = item.MissionChallengeDescription;
        //            dr[1] = item.MissionStartDate;
        //            dr[2] = item.longitude;
        //            dr[3] = item.Latitude;
        //            dr[4] = item.Corrdinates;


        //            dt.Rows.Add(dr);
        //        }
        //    }



        //    if (type == 6)

        //    {

        //        myReportNam = "dataViews.xlsx";

        //        header1 = "Data  Departments";

        //        header2 = "View of Departments  Report: ";

        //        var results = DbIMS.dataMissionReportDepartment.Where(x => x.DepartmentGUID != null).ToList().Select(x => new
        //        {
        //            MissionStartDate = x.dataMissionReportForm.MissionStartDate.Date,
        //            DepartmentDescription = x.codeDepartments.codeDepartmentsLanguages.Where(f => f.LanguageID == LAN)
        //                    .Select(fx => fx.DepartmentDescription).FirstOrDefault(),
        //            x.dataMissionReportForm.longitude,
        //            x.dataMissionReportForm.Latitude,
        //            Corrdinates = x.dataMissionReportForm.longitude + " " + x.dataMissionReportForm.Latitude
        //        }).Distinct().ToList();



        //        dt.Columns.Add(" Name", typeof(string));
        //        dt.Columns.Add("Mission Start Date", typeof(DateTime));
        //        dt.Columns.Add("longitude", typeof(string));
        //        dt.Columns.Add("Latitude", typeof(string));
        //        dt.Columns.Add("Corrdinates", typeof(string));



        //        foreach (var item in results)
        //        {
        //            string currentDepartment = "";
        //            string currentmember = "";
        //            string currentVisit = "";
        //            string HumanitarianNeed = "";
        //            string OngoingResponse = "";
        //            string FormChallenge = "";
        //            DataRow dr;
        //            dr = dt.NewRow();
        //            dr[0] = item.DepartmentDescription;
        //            dr[1] = item.MissionStartDate;
        //            dr[2] = item.longitude;
        //            dr[3] = item.Latitude;
        //            dr[4] = item.Corrdinates;


        //            dt.Rows.Add(dr);
        //        }
        //    }

        //    if (type == 7)

        //    {


        //        myReportNam = "MissionCategoryForRep.xlsx";

        //        header1 = "Data  Missions";

        //        header2 = "View of Missions  Report: ";

        //        var results = DbIMS.v_trackMissionByCategoryING.ToList().Distinct().ToList();



        //        dt.Columns.Add(" Mission Code", typeof(string));
        //        dt.Columns.Add("Participants/staff member", typeof(string));
        //        dt.Columns.Add("Type of mission", typeof(string));
        //        dt.Columns.Add("Detailed purpose of the mission", typeof(string));
        //        dt.Columns.Add("MissionStartDate", typeof(string));
        //        dt.Columns.Add("Mission Month", typeof(string));
        //        dt.Columns.Add("Mission Year", typeof(string));
        //        dt.Columns.Add("MissionStatus", typeof(string));

        //        dt.Columns.Add("Governorate", typeof(string));
        //        dt.Columns.Add("Community", typeof(string));



        //        foreach (var item in results)
        //        {

        //            DataRow dr;
        //            dr = dt.NewRow();
        //            dr[0] = item.MissionCode;
        //            dr[1] = item.Participants_staff_member;
        //            dr[2] = item.Type_of_mission_;
        //            dr[3] = item.Detailed_purpose_of_the_mission_;
        //            dr[4] = item.MissionStartDate;
        //            dr[5] = item.MonthMissionStartDate;
        //            dr[6] = item.YearMissionStartDate;
        //            dr[7] = item.MissionStatus;
        //            dr[8] = item.Governorate;
        //            dr[9] = item.Community;



        //            dt.Rows.Add(dr);
        //        }
        //    }


        //    //if (dt.Rows.Count == 0)
        //    //{
        //    //    return Json("Index");
        //    //}
        //    string oFileName = Guid.NewGuid().ToString() + ".xlsx";
        //    System.IO.File.Copy(HttpContext.Server.MapPath("~/Uploads/IMS/" + myReportNam),
        //        HttpContext.Server.MapPath("~/Uploads/IMS/" + oFileName));
        //    FileInfo file = new FileInfo(HttpContext.Server.MapPath("~/Uploads/IMS/" + oFileName));
        //    ExcelPackage pck = new ExcelPackage(file);
        //    //Add the content sheet
        //    var ws = pck.Workbook.Worksheets["Report"];
        //    ws.View.ShowGridLines = false;

        //    ws.Cells["B8"].LoadFromDataTable(dt, true);
        //    ws.Cells["B1"].Value = header1;
        //    ws.Cells["B2"].Value = header2;
        //    ws.Cells["B3"].Value = "Report Excecution Time: " + DateTime.Now.ToString("MMMM dd, yyyy hh:ss:mm tt");
        //    ws.Cells["F6:O6"].Style.Numberformat.Format = "#,##0";
        //    if (file.Exists)
        //    {

        //        HttpContext.Response.Clear();
        //        HttpContext.Response.Buffer = true;
        //        HttpContext.Response.Charset = "";
        //        HttpContext.Response.ContentType =
        //            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        HttpContext.Response.AddHeader("content-disposition",

        //            "attachment;filename=Report " + header2 + " " + DateTime.Now.ToShortDateString() + ".xlsx");
        //        using (MemoryStream myMemoryStream = new MemoryStream())
        //        {

        //            pck.SaveAs(myMemoryStream);
        //            myMemoryStream.WriteTo(HttpContext.Response.OutputStream);
        //            //HttpContext.Response.Flush();

        //            //HttpContext.Response.End();
        //        }
        //    }


        //    return file;
        //}


       
        #region Edit Views

        public ActionResult UpdateMissionDepartments(List<string> departments, Guid MissionReportFormGUID)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            if (MissionReportFormGUID != null)
            {
                var missionDepts = DbIMS.dataMissionReportDepartment.Where(x => x.MissionReportFormGUID == MissionReportFormGUID).ToList();
                DbIMS.dataMissionReportDepartment.RemoveRange(missionDepts);
                DbIMS.SaveChanges();
            }
            if (departments != null && departments.Count > 0)
            {

                foreach (var item in departments)
                {
                    dataMissionReportDepartment department = new dataMissionReportDepartment
                    {
                        MissionReportDepartmentGUID = Guid.NewGuid(),
                        MissionReportFormGUID = MissionReportFormGUID,
                        DepartmentGUID = Guid.Parse(item),


                        Active = true,

                    };
                    DbIMS.CreateNoAudit(department);
                }

                DbIMS.SaveChanges();
            }
            //var form = DbIMS.dataMissionReportForm.Find(MissionReportFormGUID);
            //MissionReportFormUpdateModel model = Mapper.Map(form, new MissionReportFormUpdateModel());


            //var staff = form.dataMissionReportFormMember.Select(x => x.UserGUID).ToList();

            var updatedepartments = DbIMS.dataMissionReportDepartment.Where(x => x.MissionReportFormGUID == MissionReportFormGUID)
                .ToList();

            var departmentsLang = DbIMS.codeDepartmentsLanguages.Where(x => x.LanguageID == LAN).ToList();

            string showDepartments = " ";

            foreach (var item in updatedepartments)
            {
                showDepartments += departmentsLang.Where(x => x.DepartmentGUID == item.DepartmentGUID).FirstOrDefault().DepartmentDescription;

            }

            return Json(new { showDepartments = showDepartments }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult UpdateMissionMembers(List<string> staffs, Guid MissionReportFormGUID)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            if (MissionReportFormGUID != null)
            {
                var missionDepts = DbIMS.dataMissionReportFormMember.Where(x => x.MissionReportFormGUID == MissionReportFormGUID).ToList();
                DbIMS.dataMissionReportFormMember.RemoveRange(missionDepts);
                DbIMS.SaveChanges();
            }
            if (staffs != null && staffs.Count > 0)
            {

                foreach (var item in staffs)
                {
                    dataMissionReportFormMember department = new dataMissionReportFormMember
                    {
                        MissionReportFormMemberGUID = Guid.NewGuid(),
                        MissionReportFormGUID = MissionReportFormGUID,
                        UserGUID = Guid.Parse(item),


                        Active = true,

                    };
                    DbIMS.CreateNoAudit(department);
                }

                DbIMS.SaveChanges();
            }
            //var form = DbIMS.dataMissionReportForm.Find(MissionReportFormGUID);
            //MissionReportFormUpdateModel model = Mapper.Map(form, new MissionReportFormUpdateModel());


            //var staff = form.dataMissionReportFormMember.Select(x => x.UserGUID).ToList();

            var updates = DbIMS.dataMissionReportFormMember.Where(x => x.MissionReportFormGUID == MissionReportFormGUID)
                .ToList();

            var users = DbIMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).ToList();

            string showStaffMembers = " ";

            foreach (var item in updates)
            {
                showStaffMembers += users.Where(x => x.UserGUID == item.UserGUID).FirstOrDefault().FirstName + " " + users.Where(x => x.UserGUID == item.UserGUID).FirstOrDefault().Surname;

            }

            return Json(new { showStaffMembers = showStaffMembers }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult UpdateVisitObjectives(List<string> changes, Guid MissionReportFormGUID)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            if (MissionReportFormGUID != null)
            {
                var dels = DbIMS.dataMissionReportFormVisitObjective.Where(x => x.MissionReportFormGUID == MissionReportFormGUID).ToList();
                DbIMS.dataMissionReportFormVisitObjective.RemoveRange(dels);
                DbIMS.SaveChanges();
            }
            if (changes != null && changes.Count > 0)
            {
                var allVisits = DbIMS.codeIMSVisitObjectiveLanguage.Where(x => x.LanguageID == LAN).ToList();
                foreach (var item in changes)
                {
                    dataMissionReportFormVisitObjective department = new dataMissionReportFormVisitObjective
                    {
                        MissionReportFormVisitObjectiveGUID = Guid.NewGuid(),
                        MissionReportFormGUID = MissionReportFormGUID,
                        VisitObjectiveGUID = Guid.Parse(item),
                        VisitObjectiveName =
                            allVisits.Where(x => x.VisitObjectiveGUID == Guid.Parse(item) && x.LanguageID == LAN)
                                .Select(x => x.VisitObjectiveDescription).FirstOrDefault(),


                        Active = true,

                    };
                    DbIMS.CreateNoAudit(department);
                }

                DbIMS.SaveChanges();
            }


            var updates = DbIMS.dataMissionReportFormVisitObjective.Where(x => x.MissionReportFormGUID == MissionReportFormGUID)
                .ToList();



            string afterChanges = " ";

            foreach (var item in updates)
            {
                afterChanges += " " + item.VisitObjectiveName;

            }

            return Json(new { afterChanges = afterChanges }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult UpdateHumanitarianNeeds(List<string> changes, Guid MissionReportFormGUID)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            if (MissionReportFormGUID != null)
            {
                var dels = DbIMS.dataMissionReportFormHumanitarianNeed.Where(x => x.MissionReportFormGUID == MissionReportFormGUID).ToList();
                DbIMS.dataMissionReportFormHumanitarianNeed.RemoveRange(dels);
                DbIMS.SaveChanges();
            }
            if (changes != null && changes.Count > 0)
            {
                var allVisits = DbIMS.codeIMSHumanitarianNeedLanguage.Where(x => x.LanguageID == LAN).ToList();
                foreach (var item in changes)
                {
                    dataMissionReportFormHumanitarianNeed department = new dataMissionReportFormHumanitarianNeed
                    {
                        MissionReportFormHumanitarianNeedGUID = Guid.NewGuid(),
                        MissionReportFormGUID = MissionReportFormGUID,
                        HumanitarianNeedGUID = Guid.Parse(item),
                        HumanitarianNeedName =
                            allVisits.Where(x => x.HumanitarianNeedGUID == Guid.Parse(item) && x.LanguageID == LAN)
                                .Select(x => x.HumanitarianNeedeDescription).FirstOrDefault(),


                        Active = true,

                    };
                    DbIMS.CreateNoAudit(department);
                }

                DbIMS.SaveChanges();
            }


            var updates = DbIMS.dataMissionReportFormHumanitarianNeed.Where(x => x.MissionReportFormGUID == MissionReportFormGUID)
                .ToList();



            string afterChanges = " ";

            foreach (var item in updates)
            {
                afterChanges += " " + item.HumanitarianNeedName;

            }

            return Json(new { afterChanges = afterChanges }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult UpdateOngoingResponses(List<string> changes, Guid MissionReportFormGUID)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            if (MissionReportFormGUID != null)
            {
                var dels = DbIMS.dataMissionReportFormOngoingResponse.Where(x => x.MissionReportFormGUID == MissionReportFormGUID).ToList();
                DbIMS.dataMissionReportFormOngoingResponse.RemoveRange(dels);
                DbIMS.SaveChanges();
            }
            if (changes != null && changes.Count > 0)
            {
                var allVisits = DbIMS.codeIMSOngoingResponseLanguage.Where(x => x.LanguageID == LAN).ToList();
                foreach (var item in changes)
                {
                    dataMissionReportFormOngoingResponse department = new dataMissionReportFormOngoingResponse
                    {
                        MissionReportFormOngoingResponseGUID = Guid.NewGuid(),
                        MissionReportFormGUID = MissionReportFormGUID,
                        OngoingResponseGUID = Guid.Parse(item),
                        OngoingResponseName =
                            allVisits.Where(x => x.OngoingResponseGUID == Guid.Parse(item) && x.LanguageID == LAN)
                                .Select(x => x.OngoingResponseDescription).FirstOrDefault(),


                        Active = true,

                    };
                    DbIMS.CreateNoAudit(department);
                }

                DbIMS.SaveChanges();
            }


            var updates = DbIMS.dataMissionReportFormOngoingResponse.Where(x => x.MissionReportFormGUID == MissionReportFormGUID)
                .ToList();



            string afterChanges = " ";

            foreach (var item in updates)
            {
                afterChanges += " " + item.OngoingResponseName;

            }

            return Json(new { afterChanges = afterChanges }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult UpdateMissionChallenges(List<string> changes, Guid MissionReportFormGUID)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            if (MissionReportFormGUID != null)
            {
                var dels = DbIMS.dataMissionReportFormChallenge.Where(x => x.MissionReportFormGUID == MissionReportFormGUID).ToList();
                DbIMS.dataMissionReportFormChallenge.RemoveRange(dels);
                DbIMS.SaveChanges();
            }
            if (changes != null && changes.Count > 0)
            {
                var allVisits = DbIMS.codeIMSMissionChallengeLanguage.Where(x => x.LanguageID == LAN).ToList();
                foreach (var item in changes)
                {
                    dataMissionReportFormChallenge department = new dataMissionReportFormChallenge
                    {
                        MissionReportFormChallengeGUID = Guid.NewGuid(),
                        MissionReportFormGUID = MissionReportFormGUID,
                        MissionChallengeGUID = Guid.Parse(item),
                        ChallengeName =
                            allVisits.Where(x => x.MissionChallengeGUID == Guid.Parse(item) && x.LanguageID == LAN)
                                .Select(x => x.MissionChallengeDescription).FirstOrDefault(),


                        Active = true,

                    };
                    DbIMS.CreateNoAudit(department);
                }

                DbIMS.SaveChanges();
            }


            var updates = DbIMS.dataMissionReportFormChallenge.Where(x => x.MissionReportFormGUID == MissionReportFormGUID)
                .ToList();



            string afterChanges = " ";

            foreach (var item in updates)
            {
                afterChanges += " " + item.ChallengeName;

            }

            return Json(new { afterChanges = afterChanges }, JsonRequestBehavior.AllowGet);

        }






        #endregion

        #region Upload 

        [HttpGet]
        public ActionResult GetUploadFile(Guid id)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var missionForm = DbIMS.dataMissionReportForm.Find(id);
            return PartialView("~/Areas/IMS/Views/MissionReportForm/_UploadFile.cshtml",
                missionForm);
        }

        [HttpPost]
        public FineUploaderResult UploadFiles(FineUpload upload, dataMissionReportForm model)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            return new FineUploaderResult(true, new { path = Upload(upload, model.MissionReportFormGUID), success = true });
        }
        public string Upload(FineUpload upload, Guid MissionReportFormGUID)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var _stearm = upload.InputStream;
            string _ext = Path.GetExtension(upload.FileName).Split('.')[1];

            dataMissionReportForm itemInput = new dataMissionReportForm();
            itemInput.MissionReportFormGUID = Guid.NewGuid();
            string FilePath = Server.MapPath("~/Uploads/IMS/MissionForms/" + MissionReportFormGUID + _ext);

            using (var fileStream = System.IO.File.Create(FilePath))
            {
                upload.InputStream.Seek(0, SeekOrigin.Begin);
                upload.InputStream.CopyTo(fileStream);
            }

            return "Success";
        }
        #endregion

        #endregion
        #region Mission Action Main 
        public ActionResult MissionActionsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            if (options.columns == null) return PartialView("~/Areas/IMS/Views/ItemModels/_MissionActionsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MissionActionDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MissionActionDataTableModel>(DataTable.Filters);
            }

            var Result = (

                    from a in DbIMS.dataMissionActionRequired.Where(x => x.MissionReportFormGUID == PK).AsExpandable()
                    join aa in DbIMS.dataMissionActionTaken on a.MissionActionRequiredGUID equals aa.MissionActionRequiredGUID
                    join b in DbIMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN) on a.FocalPointGUID equals b.UserGUID
                    into LJ1
                    from R1 in LJ1.DefaultIfEmpty()
                    join c in DbIMS.codeDepartmentsLanguages.Where(x => x.LanguageID == LAN) on a.DepartmentGUID equals c.DepartmentGUID into LJ2
                    from R2 in LJ2.DefaultIfEmpty()
                        //join c in DbIMS.codeTablesValues.Where(x => x.TableGUID == LookupTables.IMSMissionActionTakenStatus) on a.ActionStatusGUID equals c.ValueGUID 
                        //join d in DbIMS.codeTablesValuesLanguages.Where(x=>x.LanguageID==LAN) on c.ValueGUID equals d.ValueGUID into LJ3
                        //from R3 in LJ3.DefaultIfEmpty()

                    select new MissionActionDataTableModel
                    {
                        MissionActionRequiredGUID = a.MissionActionRequiredGUID,
                        MissionReportFormGUID = a.MissionReportFormGUID,
                        ActionRequiredName = a.ActionRequiredName,
                        ActionTakenName = aa.ActionTakenName,
                        FocalPointName = R1.FirstName + " " + R1.Surname,
                        UnitName = R2.DepartmentDescription,
                        //ActionTakendStatus = R3.ValueDescription,
                        ActionTakenDate = aa.ActionTakenDate,
                        Active = a.Active,


                        dataMissionActionRequiredRowVersion = a.dataMissionActionRequiredRowVersion
                    }).Where(Predicate);


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        #endregion




        #region Mission Action Temps

        public ActionResult MissionTempActionsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/IMS/Views/ItemModels/_TempMissionActionsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MissionActionTempDataTable, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MissionActionTempDataTable>(DataTable.Filters);
            }

            var Result = (

                    from a in DbIMS.dataTempMissionAction.Where(x => x.MissionReportFormGUID == PK).AsExpandable()
                    join b in DbIMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN) on a.FocalPointGUID equals b.UserGUID into LJ1
                    from R1 in LJ1.DefaultIfEmpty()
                    join c in DbIMS.codeDepartmentsLanguages.Where(x => x.LanguageID == LAN) on a.DepartmentGUID equals c.DepartmentGUID into LJ2
                    from R2 in LJ2.DefaultIfEmpty()
                        //join c in DbIMS.codeTablesValues.Where(x => x.TableGUID == LookupTables.IMSMissionActionTakenStatus) on a.ActionStatusGUID equals c.ValueGUID 
                        //join d in DbIMS.codeTablesValuesLanguages.Where(x=>x.LanguageID==LAN) on c.ValueGUID equals d.ValueGUID into LJ3
                        //from R3 in LJ3.DefaultIfEmpty()

                    select new MissionActionTempDataTable
                    {
                        TempMissionActionGUID = a.TempMissionActionGUID,
                        MissionReportFormGUID = a.MissionReportFormGUID,
                        ActionRequiredName = a.ActionRequiredName,
                        ActionTakenName = a.ActionTakenName,
                        FocalPointName = R1.FirstName + " " + R1.Surname,
                        UnitName = R2.DepartmentDescription,
                        //ActionTakendStatus = R3.ValueDescription,
                        ActionTakenDate = a.ActionTakenDate,


                        dataTempMissionActionRowVersion = a.dataTempMissionActionRowVersion
                    }).Where(Predicate);


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        [Route("IMS/MissionAction/Update/{PK}")]
        public ActionResult MissionActionsUpdate(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var model = (from a in DbIMS.dataMissionActionRequired.WherePK(PK)
                         join b in DbIMS.codeDepartmentsLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeDepartments.DeletedOn) && x.LanguageID == LAN) on a.DepartmentGUID equals b.DepartmentGUID
                         join bb in DbIMS.dataMissionActionTaken on a.MissionActionRequiredGUID equals bb.MissionActionRequiredGUID
                         join c in DbIMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN) on a.FocalPointGUID equals c.UserGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         join d in DbIMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN) on bb.ActionStatusGUID equals d.ValueGUID into LJ2
                         from R2 in LJ2.DefaultIfEmpty()
                         select new MissionActionUpdateModel
                         {
                             MissionActionRequiredGUID = a.MissionActionRequiredGUID,
                             MissionReportFormGUID = a.MissionReportFormGUID,
                             ActionStatusGUID = bb.ActionStatusGUID,
                             DepartmentGUID = b.DepartmentGUID,
                             FocalPointGUID = a.FocalPointGUID,
                             MissionActionTakenGUID = bb.MissionActionTakenGUID,
                             CreatedByGUID = a.CreatedByGUID,
                             CreatedDate = a.CreatedDate,



                             ActionRequiredName = a.ActionRequiredName,
                             ActionTakenName = bb.ActionTakenName,
                             ActionTakenDate = bb.ActionTakenDate,
                             Active = a.Active,
                             dataMissionActionRequiredRowVersion = a.dataMissionActionRequiredRowVersion,

                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("TempMissionAction", "MissionActionsUpdate", new { Area = "IMS" }));

            return PartialView("~/Areas/IMS/Views/TempMissionAction/_MissionActionsUpdateModal.cshtml", model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MissionActionsUpdate(MissionActionUpdateModel model)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            if (!ModelState.IsValid) return PartialView("~/Areas/IMS/Views/TempMissionAction/_MissionActionsUpdateModal.cshtml", model);
            if (model.ActionRequiredName == null || model.DepartmentGUID == null || model.FocalPointGUID == null)
            {
                return PartialView("~/Areas/IMS/Views/TempMissionAction/_MissionActionsUpdateModal.cshtml", model);
            }

            DateTime ExecutionTime = DateTime.Now;

            var missionTaken = DbIMS.dataMissionActionTaken.Where(x => x.MissionActionTakenGUID == model.MissionActionTakenGUID).FirstOrDefault();
            missionTaken.ActionStatusGUID = model.ActionStatusGUID;
            missionTaken.ActionTakenDate = model.ActionTakenDate;
            missionTaken.ActionTakenName = model.ActionTakenName;

            var missionRequired = DbIMS.dataMissionActionRequired.Where(x => x.MissionActionRequiredGUID == model.MissionActionRequiredGUID).FirstOrDefault();
            missionRequired.ActionRequiredName = model.ActionRequiredName;
            missionRequired.DepartmentGUID = model.DepartmentGUID;
            missionRequired.FocalPointGUID = model.FocalPointGUID;


            //dataMissionActionTaken missionAction = Mapper.Map(model, new dataMissionActionTaken());
            //missionAction.ActionTakenName = model.ActionTakenName;
            //dataMissionActionRequired missionRequired = Mapper.Map(model, new dataMissionActionRequired());
            //missionRequired.ActionRequiredName = model.ActionRequiredName;
            ///DbIMS.Update(missionAction, Permissions.MissionReportForm.DeleteGuid, ExecutionTime, DbCMS);

            DbIMS.Update(missionRequired, Permissions.MissionReportForm.DeleteGuid, ExecutionTime, DbCMS);
            DbIMS.Update(missionTaken, Permissions.MissionReportForm.DeleteGuid, ExecutionTime, DbCMS);

            //dataTempMissionAction tempActions = Mapper.Map(model, new dataTempMissionAction());
            //DbIMS.Update(tempMissionActions, Permissions.MissionReportForm.DeleteGuid, ExecutionTime, DbCMS);

            try
            {
                DbIMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIMS.SingleUpdateMessage(DataTableNames.MissionActionsDataTable, DbIMS.PrimaryKeyControl(missionRequired), DbIMS.RowVersionControls(Portal.SingleToList(missionRequired))));
                // return Json(DbIMS.SingleUpdateMessage(null, null, DbIMS.RowVersionControls(tempActions, tempActions)));
            }

            catch (Exception ex)
            {
                return Json(DbIMS.ErrorMessage(ex.Message));
            }
            //catch (Exception ex)
            //{
            //    return Json(DbIMS.ErrorMessage(ex.Message));
            //}
            return Json(DbIMS.SingleUpdateMessage(null, null, DbIMS.RowVersionControls(missionRequired, missionTaken)));
        }


        [Route("IMS/TempMissionAction/Update/{PK}")]
        public ActionResult MissionTempActionsUpdate(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var model = (from a in DbIMS.dataTempMissionAction.WherePK(PK)
                         join b in DbIMS.codeDepartmentsLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeDepartments.DeletedOn) && x.LanguageID == LAN) on a.DepartmentGUID equals b.DepartmentGUID
                         join c in DbIMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN) on a.FocalPointGUID equals c.UserGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         join d in DbIMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN) on a.ActionStatusGUID equals d.ValueGUID into LJ2
                         from R2 in LJ2.DefaultIfEmpty()
                         select new MissionActionTempUpdateModel
                         {
                             TempMissionActionGUID = a.TempMissionActionGUID,
                             MissionReportFormGUID = a.MissionReportFormGUID,
                             MissionActionRequiredGUID = a.MissionActionRequiredGUID,
                             DepartmentGUID = b.DepartmentGUID,
                             FocalPointGUID = a.FocalPointGUID,
                             MissionActionTakenGUID = a.MissionActionTakenGUID,
                             ActionStatusGUID = a.ActionStatusGUID,
                             ActionRequiredName = a.ActionRequiredName,
                             ActionTakenName = a.ActionTakenName,
                             ActionTakenDate = a.ActionTakenDate,
                             Active = a.Active,

                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("TempMissionAction", "MissionTempActionsUpdate", new { Area = "IMS" }));

            return PartialView("~/Areas/IMS/Views/TempMissionAction/_TempMissionActionsUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MissionTempActionsUpdate(MissionActionTempUpdateModel model)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            if (!ModelState.IsValid) return PartialView("~/Areas/IMS/Views/TempMissionAction/_TempMissionActionsUpdateModal.cshtml", model);
            if (model.ActionRequiredName == null || model.DepartmentGUID == null || model.FocalPointGUID == null)
            {
                return PartialView("~/Areas/IMS/Views/TempMissionAction/_TempMissionActionsUpdateModal.cshtml", model);
            }

            DateTime ExecutionTime = DateTime.Now;

            dataTempMissionAction tempActions = Mapper.Map(model, new dataTempMissionAction());
            DbIMS.Update(tempActions, Permissions.MissionReportForm.DeleteGuid, ExecutionTime, DbCMS);



            try
            {
                DbIMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIMS.SingleUpdateMessage(DataTableNames.MissionTempActionsDataTable, DbIMS.PrimaryKeyControl(tempActions), DbIMS.RowVersionControls(Portal.SingleToList(tempActions))));
                return Json(DbIMS.SingleUpdateMessage(null, null, DbIMS.RowVersionControls(tempActions, tempActions)));
            }
            catch (DbUpdateConcurrencyException)
            {
                //return ConcurrencyItemModel(model.WarehouseItemModelGUID);
            }
            catch (Exception ex)
            {
                return Json(DbIMS.ErrorMessage(ex.Message));
            }
            return Json(DbIMS.SingleUpdateMessage(null, null, DbIMS.RowVersionControls(tempActions, tempActions)));
        }


        public ActionResult MissionActionsCreate(Guid FK)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var mission = DbIMS.dataMissionActionRequired.Where(x => x.MissionActionRequiredGUID == FK).FirstOrDefault();
            return PartialView("~/Areas/IMS/Views/TempMissionAction/_MissionActionsUpdateModal.cshtml",
                new MissionActionUpdateModel
                {
                    MissionReportFormGUID = FK,
                });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MissionActionsCreate(MissionActionUpdateModel model)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            if (!ModelState.IsValid) return PartialView("~/Areas/IMS/Views/TempMissionAction/_MissionActionsUpdateModal.cshtml", model);
            if (model.ActionRequiredName == null || model.DepartmentGUID == null || model.FocalPointGUID == null)
            {
                return PartialView("~/Areas/IMS/Views/TempMissionAction/_MissionActionsUpdateModal.cshtml", model);
            }
            DateTime ExecutionTime = DateTime.Now;
            dataMissionActionRequired myActionRequired = new dataMissionActionRequired
            {
                MissionActionRequiredGUID = Guid.NewGuid(),
                MissionReportFormGUID = model.MissionReportFormGUID,
                DepartmentGUID = model.DepartmentGUID,
                ActionRequiredName = model.ActionRequiredName,
                FocalPointGUID = model.FocalPointGUID,
                CreatedByGUID = UserGUID,
                CreatedDate = ExecutionTime
            };
            //missionRequiredActions.Add(myActionRequired);
            DbIMS.CreateNoAudit(myActionRequired);
            dataMissionActionTaken myActionTaken = new dataMissionActionTaken
            {
                MissionActionTakenGUID = Guid.NewGuid(),
                MissionActionRequiredGUID = myActionRequired.MissionActionRequiredGUID,
                ActionStatusGUID = model.ActionStatusGUID,
                ActionTakenName = model.ActionTakenName,
                ActionTakenDate = model.ActionTakenDate,

                CreatedByGUID = UserGUID,
                CreatedDate = ExecutionTime
            };
            DbIMS.CreateNoAudit(myActionTaken);



            dataTempMissionAction tempMission = new dataTempMissionAction();
            //tempMission.TempMissionActionGUID = Guid.NewGuid();
            //tempMission.MissionReportFormGUID = model.MissionReportFormGUID;
            //tempMission.DepartmentGUID = model.DepartmentGUID;
            //tempMission.ActionRequiredName = model.ActionRequiredName;
            //tempMission.FocalPointGUID = model.FocalPointGUID;
            //tempMission.MissionActionTakenGUID = model.MissionActionTakenGUID;
            //tempMission.ActionRequiredName = model.ActionRequiredName;
            //tempMission.ActionTakenName = model.ActionTakenName;
            //tempMission.ActionTakenDate = model.ActionTakenDate;
            //tempMission.ActionStatusGUID = model.ActionStatusGUID;
            //DbIMS.Create(tempMission, Permissions.MissionReportForm.DeleteGuid, ExecutionTime, DbCMS);
            //var mission=DbIMS.dataMissionReportForm.Where(x => x.MissionReportFormGUID == model.MissionReportFormGUID).FirstOrDefault();
            try
            {
                DbIMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIMS.SingleUpdateMessage(DataTableNames.MissionActionsDataTable, DbIMS.PrimaryKeyControl(myActionRequired), DbIMS.RowVersionControls(Portal.SingleToList(myActionRequired))));
                //return Json(DbIMS.SingleUpdateMessage(DataTableNames.MissionTempActionsDataTable, DbIMS.PrimaryKeyControl(tempMission), DbIMS.RowVersionControls(Portal.SingleToList(tempMission))));
            }
            catch (Exception ex)
            {
                return Json(DbIMS.ErrorMessage(ex.Message));
            }
        }


        public ActionResult MissionTempActionsCreate(Guid FK)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            return PartialView("~/Areas/IMS/Views/TempMissionAction/_TempMissionActionsUpdateModal.cshtml",
                new MissionActionTempUpdateModel { MissionReportFormGUID = FK });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MissionTempActionsCreate(MissionActionTempUpdateModel model)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            if (!ModelState.IsValid) return PartialView("~/Areas/IMS/Views/TempMissionAction/_TempMissionActionsUpdateModal.cshtml", model);
            if (model.ActionRequiredName == null || model.DepartmentGUID == null || model.FocalPointGUID == null)
            {
                return PartialView("~/Areas/IMS/Views/TempMissionAction/_TempMissionActionsUpdateModal.cshtml", model);
            }
            DateTime ExecutionTime = DateTime.Now;
            dataTempMissionAction tempMission = new dataTempMissionAction();
            tempMission.TempMissionActionGUID = Guid.NewGuid();
            tempMission.MissionReportFormGUID = model.MissionReportFormGUID;
            tempMission.DepartmentGUID = model.DepartmentGUID;
            tempMission.ActionRequiredName = model.ActionRequiredName;
            tempMission.FocalPointGUID = model.FocalPointGUID;
            tempMission.MissionActionTakenGUID = model.MissionActionTakenGUID;
            tempMission.ActionRequiredName = model.ActionRequiredName;
            tempMission.ActionTakenName = model.ActionTakenName;
            tempMission.ActionTakenDate = model.ActionTakenDate;
            tempMission.ActionStatusGUID = model.ActionStatusGUID;
            DbIMS.Create(tempMission, Permissions.MissionReportForm.DeleteGuid, ExecutionTime, DbCMS);

            try
            {
                DbIMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIMS.SingleUpdateMessage(DataTableNames.MissionTempActionsDataTable, DbIMS.PrimaryKeyControl(tempMission), DbIMS.RowVersionControls(Portal.SingleToList(tempMission))));
                //return Json(DbIMS.SingleUpdateMessage(DataTableNames.MissionTempActionsDataTable, DbIMS.PrimaryKeyControl(tempMission), DbIMS.RowVersionControls(Portal.SingleToList(tempMission))));
            }
            catch (Exception ex)
            {
                return Json(DbIMS.ErrorMessage(ex.Message));
            }
        }


        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MissionTempActionsDataTableDelete(List<dataTempMissionAction> models)
        {
            if (!CMS.HasAction(Permissions.MissionReportForm.Delete, Apps.IMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataTempMissionAction> DeletedtempActions = DeleteTempActionMission(models);
            var guids = models.Select(x => x.MissionReportFormGUID).ToList();
            var tempActions = DbIMS.dataTempMissionAction.Where(x => guids.Contains((Guid)x.MissionReportFormGUID)).ToList();
            DbIMS.dataTempMissionAction.RemoveRange(tempActions);




            try
            {
                DbIMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIMS.PartialDeleteMessage(DeletedtempActions, models, DataTableNames.MissionTempActionsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbIMS.ErrorMessage(ex.Message));
            }
        }

        private List<dataTempMissionAction> DeleteTempActionMission(List<dataTempMissionAction> models)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            DateTime ExecutionTime = DateTime.Now;
            List<dataTempMissionAction> DeletedMissionReport = new List<dataTempMissionAction>();

            string query = DbIMS.QueryBuilder(models, Permissions.Notifications.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbIMS.Database.SqlQuery<dataTempMissionAction>(query).ToList();
            foreach (var record in Records)
            {
                DeletedMissionReport.Add(DbIMS.Delete(record, ExecutionTime, Permissions.MissionReportActionsForm.DeleteGuid, DbCMS));
            }


            return DeletedMissionReport;
        }
        #endregion

        #region mission Actions overview

        [Route("IMS/MissionActions/")]
        public ActionResult MissionActionsIndex()
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            return View("~/Areas/IMS/Views/MissionActions/Index.cshtml");
        }


        public JsonResult MissionActionOverviewDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MissionActionOverviewDataTable, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MissionActionOverviewDataTable>(DataTable.Filters);
            }

            var All = (

                from a in DbIMS.dataMissionActionRequired.AsExpandable()
                join b in DbIMS.dataMissionReportForm on a.MissionReportFormGUID equals b.MissionReportFormGUID
                join c in DbIMS.codeLocationsLanguages.Where(x => x.LanguageID == LAN) on b.GovernorateGUID equals c.LocationGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbIMS.dataMissionActionTaken on a.MissionActionRequiredGUID equals c.MissionActionRequiredGUID
                join d in DbIMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN) on c.ActionStatusGUID equals d.ValueGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()
                join e in DbIMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN) on a.FocalPointGUID equals e.UserGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()
                join f in DbIMS.codeDepartmentsLanguages.Where(x => x.LanguageID == LAN) on a.DepartmentGUID equals f.DepartmentGUID into LJ4
                from R4 in LJ4.DefaultIfEmpty()
                select new MissionActionOverviewDataTable
                {
                    MissionReportFormGUID = b.MissionReportFormGUID,
                    MissionCode = b.MissionCode,
                    MissionNumber = b.MissionNumber,
                    MissionStartDate = b.MissionStartDate,
                    MissionStatusGUID = b.MissionStatusGUID.ToString(),


                    GovernorateGUID = b.GovernorateGUID.ToString(),
                    Governorate = R1.LocationDescription.ToString(),
                    ActionRequiredName = a.ActionRequiredName,
                    ActionTakenName = c.ActionTakenName,
                    ActionTakenStatus = R2.ValueDescription,
                    ActionStatusGUID = R2.ValueGUID,
                    FocalPointName = R3.FirstName + " " + R3.Surname,
                    UnitName = R4.DepartmentDescription,
                    DepartmentGUID = a.DepartmentGUID.ToString(),
                    FocalPointGUID = a.FocalPointGUID.ToString(),
                    ActionTakenDate = c.ActionTakenDate,
                    dataMissionActionTakenRowVersion = c.dataMissionActionTakenRowVersion,
                    CreatedDate = b.CreatedDate,
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<MissionActionOverviewDataTable> Result = Mapper.Map<List<MissionActionOverviewDataTable>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result.OrderByDescending(x => x.MissionNumber).ThenByDescending(x => x.CreatedDate)), JsonRequestBehavior.AllowGet);
        }


        [Route("IMS/MissionActionsOngoing/")]
        public ActionResult MissionActionsOngoing()
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            return View("~/Areas/IMS/Views/MissionActions/MissionActionsOngoing.cshtml");
        }


        public JsonResult MissionActionOngoingDataTable(DataTableRecievedOptions options)
        {
           

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MissionActionOverviewDataTable, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MissionActionOverviewDataTable>(DataTable.Filters);
            }

            var All = (

                from a in DbIMS.dataMissionActionRequired.AsExpandable()
                join f in DbIMS.dataMissionActionTaken.Where(x => x.ActionStatusGUID == MissionActionTakenStatus.Ongoing) on a.MissionActionRequiredGUID equals f.MissionActionRequiredGUID
                join b in DbIMS.dataMissionReportForm on a.MissionReportFormGUID equals b.MissionReportFormGUID
                join c in DbIMS.codeLocationsLanguages.Where(x => x.LanguageID == LAN) on b.GovernorateGUID equals c.LocationGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()

                join d in DbIMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN) on f.ActionStatusGUID equals d.ValueGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()
                join e in DbIMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN) on a.FocalPointGUID equals e.UserGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()
                join ff in DbIMS.codeDepartmentsLanguages.Where(x => x.LanguageID == LAN) on a.DepartmentGUID equals ff.DepartmentGUID into LJ4
                from R4 in LJ4.DefaultIfEmpty()
                select new MissionActionOverviewDataTable
                {
                    MissionReportFormGUID = b.MissionReportFormGUID,
                    MissionCode = b.MissionCode,
                    MissionNumber = b.MissionNumber,
                    MissionStartDate = b.MissionStartDate,
                    MissionStatusGUID = b.MissionStatusGUID.ToString(),
                    GovernorateGUID = b.GovernorateGUID.ToString(),
                    Governorate = R1.LocationDescription.ToString(),
                    ActionRequiredName = a.ActionRequiredName,
                    ActionTakenName = f.ActionTakenName,
                    ActionTakenStatus = R2.ValueDescription,
                    ActionStatusGUID = R2.ValueGUID,
                    FocalPointName = R3.FirstName + " " + R3.Surname,
                    UnitName = R4.DepartmentDescription,
                    DepartmentGUID = a.DepartmentGUID.ToString(),
                    FocalPointGUID = a.FocalPointGUID.ToString(),
                    ActionTakenDate = f.ActionTakenDate,
                    dataMissionActionTakenRowVersion = f.dataMissionActionTakenRowVersion,
                    CreatedDate = b.CreatedDate,
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<MissionActionOverviewDataTable> Result = Mapper.Map<List<MissionActionOverviewDataTable>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result.OrderByDescending(x => x.MissionNumber).ThenByDescending(x => x.CreatedDate)), JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult DownloadMissionReportFile(Guid id)
        {

            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            var model = DbIMS.dataMissionReportForm.Where(x => x.MissionReportFormGUID == id).FirstOrDefault();
            var MissionReportFullPath = model.MissionReportFormGUID + model.ReportExtensionType;


            string sourceFile = Server.MapPath("~/Uploads/IMS/MissionForms/" + MissionReportFullPath);


            byte[] fileBytes = System.IO.File.ReadAllBytes(sourceFile);

            string fileName = DateTime.Now.ToString("yyMMdd") + model.MissionReportFormGUID + model.ReportExtensionType;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

            // return Json(DbCMS.SingleUpdateMessage(null, null, null, null, "Please Wait...."));
        }

        #region Mission Document
        public ActionResult MissionReportDocumentCreate(Guid FK)
        {

            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //  return Json(DbIMS.PermissionError());
            //}
            return PartialView("~/Areas/IMS/Views/MissionReportForm/_DocumentUpdateModal.cshtml",
                new MissionReportFormDocumentUpdateModel { MissionReportFormGUID = FK });
        }

        [HttpPost]
        public FineUploaderResult UploadMissionReportDocuments(FineUpload upload, Guid _missionReportFormGUID, Guid _missionReportTypeGUID)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            return new FineUploaderResult(true, new { path = UploadDocument(upload, _missionReportFormGUID, _missionReportTypeGUID), success = true });
        }

        public string UploadDocument(FineUpload upload, Guid _missionReportFormGUID, Guid _missionReportTypeGUID)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var _stearm = upload.InputStream;
            DateTime ExecutionTime = DateTime.Now;
            //string _ext = Path.GetExtension(upload.FileName).Split('.')[1];
            dataMissionReportDocument documentUplod = new dataMissionReportDocument();
            documentUplod.MissionReportDocumentGUID = Guid.NewGuid();
            //string FilePath = Server.MapPath("~/Areas/IMS/UploadedDocuments/" + documentUplod.ItemIntpuDetailUploadedDocumentGUID + _ext);

            string _ext = Path.GetExtension(upload.FileName).Split('.')[1];

            string FolderPath = Server.MapPath("~/Uploads/IMS/MissionDocuments/" + documentUplod.MissionReportDocumentGUID.ToString());
            Directory.CreateDirectory(FolderPath);
            //int LatestFileVersion = 0;
            //try { LatestFileVersion = (from a in DbPPA.PPAFileVersion.Where(x => x.PPAOriginalFileGUID == PPAOriginalFileGUID && x.FileActionByUserGUID == UserGUID) select a.FileVersion).Max(); } catch { }
            //if (LatestFileVersion == -1) LatestFileVersion = 0;



            string FilePath = FolderPath + "/" + documentUplod.MissionReportDocumentGUID.ToString() + "." + _ext;

            using (var fileStream = System.IO.File.Create(FilePath))
            {
                upload.InputStream.Seek(0, SeekOrigin.Begin);
                upload.InputStream.CopyTo(fileStream);
            }
            documentUplod.MissionReportFormGUID = _missionReportFormGUID;

            documentUplod.ReportExtensionType = _ext;
            documentUplod.MissionReportTypeGUID = _missionReportTypeGUID;


            //documentUplod.Comments = ItemInputDetailGUID;
            //documentUplod.CreatedByGUID = UserGUID;
            //documentUplod.CreatedDate = ExecutionTime;
            DbIMS.Create(documentUplod, Permissions.MissionReportForm.CreateGuid, ExecutionTime, DbCMS);
            try
            {
                DbIMS.SaveChanges();
                DbCMS.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }


            //Server.MapPath("~/Areas/IMS/temp/Standard_Operating_Procedure_for_issuing_ICT_equipment_to_staff" + DateTime.Now.ToBinary() + ".pdf");


            return "~/Areas/IMS/UploadedDocuments/" + documentUplod.MissionReportDocumentGUID + _ext;
        }

        public ActionResult MissionReportDocumentUpdate(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //  return Json(DbIMS.PermissionError());
            //}
            MissionReportFormDocumentUpdateModel model = DbIMS.dataMissionReportDocument.Where(x => x.MissionReportDocumentGUID == PK).Select(f => new MissionReportFormDocumentUpdateModel
            {

                MissionReportDocumentGUID = (Guid)f.MissionReportDocumentGUID,

                MissionReportTypeGUID = (Guid)f.MissionReportTypeGUID,
                //DocumentNumber = f.DocumentNumber,
                MissionReportFormGUID = f.MissionReportFormGUID,

                Active = f.Active

            }).FirstOrDefault();
            return PartialView("~/Areas/IMS/Views/StaffProfile/_DocumentUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MissionReportDocumentCreate(dataMissionReportDocument model)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //  return Json(DbIMS.PermissionError());
            //}
            if (!ModelState.IsValid || (model.MissionReportTypeGUID == null)) return PartialView("~/Areas/IMS/Views/MissionReportForm/_DocumentUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbIMS.Create(model, Permissions.MissionReportForm.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbIMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIMS.SingleUpdateMessage(DataTableNames.MissionReportDocumentDataTable, DbIMS.PrimaryKeyControl(model), DbIMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbIMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MissionReportDocumentUpdate(dataMissionReportDocument model)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Update, Apps.IMS))
            //{
            //  return Json(DbIMS.PermissionError());
            //}
            if (!ModelState.IsValid || model.MissionReportFormGUID == null || model.MissionReportTypeGUID == null)
                return PartialView("~/Areas/IMS/Views/MissionReportForm/_DocumentUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbIMS.Update(model, Permissions.MissionReportForm.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbIMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIMS.SingleUpdateMessage(DataTableNames.MissionReportDocumentDataTable,
                    DbIMS.PrimaryKeyControl(model),
                    DbIMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMissionReportDocument(model.MissionReportDocumentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbIMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MissionReportDocumentDelete(dataMissionReportDocument model)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Delete, Apps.IMS))
            //{
            //  return Json(DbIMS.PermissionError());
            //}
            List<dataMissionReportDocument> DeletedLanguages = DeleteMissionReportDocument(new List<dataMissionReportDocument> { model });

            try
            {
                DbIMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.MissionReportDocumentDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMissionReportDocument(model.MissionReportDocumentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbIMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MissionReportDocumentRestore(dataMissionReportDocument model)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //  return Json(DbIMS.PermissionError());
            //}
            if (ActiveMissionReportDocument(model))
            {
                return Json(DbIMS.RecordExists());
            }

            List<dataMissionReportDocument> RestoredLanguages = RestoreStaffBankAccount(Portal.SingleToList(model));

            try
            {
                DbIMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.MissionReportDocumentDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMissionReportDocument(model.MissionReportDocumentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbIMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MissionReportDocumentDataTableDelete(List<dataMissionReportDocument> models)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Delete, Apps.IMS))
            //{
            //  return Json(DbIMS.PermissionError());
            //}
            List<dataMissionReportDocument> DeletedLanguages = DeleteMissionReportDocument(models);

            try
            {
                DbIMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.MissionReportDocumentDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbIMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MissionReportDocumentDataTableModelRestore(List<dataMissionReportDocument> models)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //  return Json(DbIMS.PermissionError());
            //}
            List<dataMissionReportDocument> RestoredLanguages = RestoreStaffBankAccount(models);

            try
            {
                DbIMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.MissionReportDocumentDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbIMS.ErrorMessage(ex.Message));
            }
        }

        private List<dataMissionReportDocument> DeleteMissionReportDocument(List<dataMissionReportDocument> models)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            DateTime ExecutionTime = DateTime.Now;

            List<dataMissionReportDocument> DeletedStaffBankAccount = new List<dataMissionReportDocument>();

            string query = DbIMS.QueryBuilder(models, Permissions.MissionReportForm.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbIMS.Database.SqlQuery<dataMissionReportDocument>(query).ToList();

            foreach (var language in languages)
            {
                DeletedStaffBankAccount.Add(DbIMS.Delete(language, ExecutionTime, Permissions.MissionReportForm.DeleteGuid, DbCMS));
            }

            return DeletedStaffBankAccount;
        }

        private List<dataMissionReportDocument> RestoreStaffBankAccount(List<dataMissionReportDocument> models)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            DateTime RestoringTime = DateTime.Now;

            List<dataMissionReportDocument> RestoredLanguages = new List<dataMissionReportDocument>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbIMS.QueryBuilder(models, Permissions.MissionReportForm.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbIMS.Database.SqlQuery<dataMissionReportDocument>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveMissionReportDocument(language))
                {
                    RestoredLanguages.Add(DbIMS.Restore(language, Permissions.MissionReportForm.DeleteGuid, Permissions.MissionReportForm.CreateGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyMissionReportDocument(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            dataMissionReportDocument dbModel = new dataMissionReportDocument();

            var Language = DbIMS.dataMissionReportDocument.Where(l => l.MissionReportDocumentGUID == PK).FirstOrDefault();
            var dbLanguage = DbIMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataMissionReportDocumentRowVersion.SequenceEqual(dbModel.dataMissionReportDocumentRowVersion))
            {
                return Json(DbIMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbIMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveMissionReportDocument(dataMissionReportDocument model)
        {
            int LanguageID = DbIMS.dataMissionReportDocument
                                  .Where(x =>
                                              x.MissionReportFormGUID == model.MissionReportFormGUID &&
                                              x.MissionReportTypeGUID == model.MissionReportTypeGUID &&

                                             (bool)x.Active).Count();
            if (LanguageID > 0)
            {
                //Fix
                ModelState.AddModelError("LanguageID", "Already Exist"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }


        public ActionResult DownloadMissionReportDocumentFile(Guid id)
        {



            var model = DbIMS.dataMissionReportDocument.Where(x => x.MissionReportDocumentGUID == id).FirstOrDefault();
            var fullPath = model.MissionReportDocumentGUID + "." + model.ReportExtensionType;


            string sourceFile = Server.MapPath("~/Uploads/IMS/MissionDocuments/" + model.MissionReportDocumentGUID + "/" + fullPath);


            byte[] fileBytes = System.IO.File.ReadAllBytes(sourceFile);

            string fileName = DateTime.Now.ToString("yyMMdd") + fullPath;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);








            // return Json(DbCMS.SingleUpdateMessage(null, null, null, null, "Please Wait...."));
        }
        public ActionResult MissionReportDocumentDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/IMS/Views/MissionReportForm/_DocumentDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MissionReportFormDocumentDataTable, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MissionReportFormDocumentDataTable>(DataTable.Filters);
            }

            Guid myDocumentTypeGUID = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3999");
            var Result = (from a in DbIMS.dataMissionReportDocument.AsExpandable().Where(x => x.Active && (x.MissionReportFormGUID == PK))

                          join b in DbIMS.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN && x.codeTablesValues.TableGUID == myDocumentTypeGUID) on a.MissionReportTypeGUID equals b.ValueGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          select new MissionReportFormDocumentDataTable
                          {
                              MissionReportDocumentGUID = a.MissionReportDocumentGUID,
                              MissionReportFormGUID = a.MissionReportFormGUID.ToString(),

                              MissionReportTypeGUID = a.MissionReportTypeGUID.ToString(),
                              DocumentType = R1.ValueDescription,

                              Active = a.Active,
                              dataMissionReportDocumentRowVersion = a.dataMissionReportDocumentRowVersion

                          }).Where(Predicate);


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}