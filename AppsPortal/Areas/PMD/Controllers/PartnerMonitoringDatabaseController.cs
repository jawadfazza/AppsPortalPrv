using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AutoMapper;
using PMD_DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppsPortal.ViewModels;
using System.Linq.Expressions;
using LinqKit;
using FineUploader;
using AppsPortal.Library.MimeDetective;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace AppsPortal.Areas.PMD.Controllers
{
    public class PartnerMonitoringDatabaseController : PMDBaseController
    {


        #region Main
        public ActionResult Index()
        {
            HashingHelper.EncryptPassword("P@ssw0rd123", Portal.GUIDToString(Guid.Parse("4D433993-5019-42A4-B3B9-21DC27DC19F4")));
            if (!CMS.HasAction(Permissions.PartnerMonitoringDatabase.Access, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/PMD/Views/PartnerMonitoringDatabase/Index.cshtml");
        }

        [Route("PMD/PartnerMonitoringDatabase/PartnerMonitoringDatabaseDataTable/")]
        public JsonResult PartnerMonitoringDatabaseDataTable(DataTableRecievedOptions options)
        {

            if (!CMS.HasAction(Permissions.PartnerMonitoringDatabase.Access, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<PartnerMonitoringDatabaseDataTableModel, bool>> Predicate = p => true;


            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<PartnerMonitoringDatabaseDataTableModel>(DataTable.Filters);
            }

            List<string> AuthorizedListByPartner = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.PartnerMonitoringDatabase.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            if (CMS.HasAction(Permissions.PartnerMonitoringDatabase.Create, Apps.PMD))
            {
            }
            //loc
            if (CMS.HasAction(Permissions.PartnerMonitoringDatabaseFieldTechVerify.Create, Apps.PMD))
            {
                List<string> AuthorizedListByLocation = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.PartnerMonitoringDatabaseFieldTechVerify.CreateGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

                var All = (from a in DbPMD.v_dataPartnerMonitoringDatabaseDataTable
                       .Where(x => x.LanguageID == LAN)
                       .Where(x => x.Active == true)
                       .Where(x => AuthorizedListByPartner.Contains(x.ImplementingPartnerGUID.ToString()))
                       .Where(x => AuthorizedListByLocation.Contains(x.GovernorateGUID.ToString()))
                           select new PartnerMonitoringDatabaseDataTableModel
                           {
                               PartnerMonitoringDBGUID = a.PartnerMonitoringDBGUID,
                               ObjectiveGUID = a.ObjectiveGUID.ToString(),
                               ObjectiveDescription = a.ObjectiveDescription,
                               OutputGUID = a.OutputGUID.ToString(),
                               OutputDescription = a.OutputDescription,
                               IndicatorGUID = a.IndicatorGUID.ToString(),
                               IndicatorDescription = a.IndicatorDescription,
                               Governorate = a.admin1Name_en,
                               admin1PCode = a.Governorate,
                               District = a.admin2Name_en,
                               SubDistrict = a.admin3Name_en,
                               CommunityName = a.admin4Name_en,
                               ImplementingPartner = a.PartnerDescription,
                               IsVerifiedByFieldTech = a.IsVerifiedByFieldTech.HasValue ? a.IsVerifiedByFieldTech.Value : false,
                               IsApprovedByCountryTech = a.IsApprovedByCountryTech.HasValue ? a.IsApprovedByCountryTech.Value : false,
                               IsVerified = a.IsVerified,
                               Active = a.Active,
                               dataPartnerMonitoringDBRowVersion = a.dataPartnerMonitoringDBRowVersion
                           }).Distinct().Where(Predicate);

                All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

                List<PartnerMonitoringDatabaseDataTableModel> Result = Mapper.Map<List<PartnerMonitoringDatabaseDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

                return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
            }
            //obj
            else if (CMS.HasAction(Permissions.PartnerMonitoringDatabaseSpecificObjective.Create, Apps.PMD))
            {
                List<string> AuthorizedListByObjectives = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.PartnerMonitoringDatabaseSpecificObjective.CreateGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();


                var All = (from a in DbPMD.v_dataPartnerMonitoringDatabaseDataTable
                      .Where(x => x.LanguageID == LAN)
                      .Where(x => x.Active == true)
                      .Where(x => AuthorizedListByPartner.Contains(x.ImplementingPartnerGUID.ToString()))
                      .Where(x => AuthorizedListByObjectives.Contains(x.ObjectiveGUID.ToString()))
                           select new PartnerMonitoringDatabaseDataTableModel
                           {
                               PartnerMonitoringDBGUID = a.PartnerMonitoringDBGUID,
                               ObjectiveGUID = a.ObjectiveGUID.ToString(),
                               ObjectiveDescription = a.ObjectiveDescription,
                               OutputGUID = a.OutputGUID.ToString(),
                               OutputDescription = a.OutputDescription,
                               IndicatorGUID = a.IndicatorGUID.ToString(),
                               IndicatorDescription = a.IndicatorDescription,
                               Governorate = a.admin1Name_en,
                               admin1PCode = a.Governorate,
                               District = a.admin2Name_en,
                               SubDistrict = a.admin3Name_en,
                               CommunityName = a.admin4Name_en,
                               ImplementingPartner = a.PartnerDescription,
                               IsVerifiedByFieldTech = a.IsVerifiedByFieldTech.HasValue ? a.IsVerifiedByFieldTech.Value : false,
                               IsApprovedByCountryTech = a.IsApprovedByCountryTech.HasValue ? a.IsApprovedByCountryTech.Value : false,
                               IsVerified = a.IsVerified,
                               Active = a.Active,
                               dataPartnerMonitoringDBRowVersion = a.dataPartnerMonitoringDBRowVersion
                           }).Distinct().Where(Predicate);

                All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

                List<PartnerMonitoringDatabaseDataTableModel> Result = Mapper.Map<List<PartnerMonitoringDatabaseDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

                return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var All = (from a in DbPMD.v_dataPartnerMonitoringDatabaseDataTable
                       .Where(x => x.LanguageID == LAN)
                       .Where(x => x.Active == true)
                       .Where(x => AuthorizedListByPartner.Contains(x.ImplementingPartnerGUID.ToString()))
                           select new PartnerMonitoringDatabaseDataTableModel
                           {
                               PartnerMonitoringDBGUID = a.PartnerMonitoringDBGUID,
                               ObjectiveGUID = a.ObjectiveGUID.ToString(),
                               ObjectiveDescription = a.ObjectiveDescription,
                               OutputGUID = a.OutputGUID.ToString(),
                               OutputDescription = a.OutputDescription,
                               IndicatorGUID = a.IndicatorGUID.ToString(),
                               IndicatorDescription = a.IndicatorDescription,
                               Governorate = a.admin1Name_en,
                               admin1PCode = a.Governorate,
                               District = a.admin2Name_en,
                               SubDistrict = a.admin3Name_en,
                               CommunityName = a.admin4Name_en,
                               ImplementingPartner = a.PartnerDescription,
                               IsVerifiedByFieldTech = a.IsVerifiedByFieldTech.HasValue ? a.IsVerifiedByFieldTech.Value : false,
                               IsApprovedByCountryTech = a.IsApprovedByCountryTech.HasValue ? a.IsApprovedByCountryTech.Value : false,
                               IsVerified = a.IsVerified,
                               Active = a.Active,
                               dataPartnerMonitoringDBRowVersion = a.dataPartnerMonitoringDBRowVersion
                           }).Distinct().Where(Predicate);

                All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

                List<PartnerMonitoringDatabaseDataTableModel> Result = Mapper.Map<List<PartnerMonitoringDatabaseDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

                return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
            }

        }

        [Route("PMD/PartnerMonitoringDatabase/Create/")]
        public ActionResult PartnerMonitoringDatabaseCreate()
        {
            if (!CMS.HasAction(Permissions.PartnerMonitoringDatabase.Create, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            PartnerMonitoringDatabaseUpdateModel model = new PartnerMonitoringDatabaseUpdateModel();
            model.DateOfReport = DateTime.Now;


            model.PMDHealthUnitOfAchievements = new List<PMDHealthUnitOfAchievement>();
            var tempPMDHealthUnitOfAchievements = (from a in DbPMD.codePmdUnitOfAchievementLanguages
                                                   join b in DbPMD.codePmdUnitOfAchievement on a.UnitOfAchievementGUID equals b.UnitOfAchievementGUID
                                                   where a.LanguageID == LAN && b.UnitOfAchievementCategory == "Health"
                                                   select new
                                                   {
                                                       UnitOfAchievementGUID = a.UnitOfAchievementGUID,
                                                       UnitOfAchievementDescription = a.UnitOfAchievementDescription,
                                                       UnitOfAchievementGuidance = a.UnitOfAchievementGuidance,
                                                       MeasurementTotal = 0
                                                   }).Distinct().OrderBy(x => x.UnitOfAchievementDescription).ToList();
            model.PMDHealthUnitOfAchievements = Mapper.Map(tempPMDHealthUnitOfAchievements, new List<PMDHealthUnitOfAchievement>());

            model.PMDDomesticUnitOfAchievements = new List<PMDDomesticUnitOfAchievement>();
            var tempPMDDomesticUnitOfAchievements = (from a in DbPMD.codePmdUnitOfAchievementLanguages
                                                     join b in DbPMD.codePmdUnitOfAchievement on a.UnitOfAchievementGUID equals b.UnitOfAchievementGUID
                                                     where a.LanguageID == LAN && b.UnitOfAchievementCategory == "Domestic"
                                                     select new
                                                     {
                                                         UnitOfAchievementGUID = a.UnitOfAchievementGUID,
                                                         UnitOfAchievementDescription = a.UnitOfAchievementDescription,
                                                         UnitOfAchievementGuidance = a.UnitOfAchievementGuidance,
                                                         MeasurementTotal = 0
                                                     }).Distinct().OrderBy(x => x.UnitOfAchievementDescription).ToList();
            model.PMDDomesticUnitOfAchievements = Mapper.Map(tempPMDDomesticUnitOfAchievements, new List<PMDDomesticUnitOfAchievement>());


            var OrganizationInstanceGUID = (from a in DbCMS.userProfiles where a.UserProfileGUID == UserProfileGUID select a.OrganizationInstanceGUID).FirstOrDefault();
            model.ImplementingPartnerGUID = OrganizationInstanceGUID;

            model.ObjectiveGUIDBool = false;

            model.MeasurementTotal = 0;
            return View("~/Areas/PMD/Views/PartnerMonitoringDatabase/PartnerMonitoringDatabase.cshtml", model);
        }


        [Route("PMD/PartnerMonitoringDatabase/CreateNew/")]
        public ActionResult PartnerMonitoringDatabaseCreateNew(
    string Governorate,
    string District,
    string SubDistrict,
    string CommunityName,
    string Neighborhood,
    string Location,
    double Longitude,
    double Latitude)
        {
            if (!CMS.HasAction(Permissions.PartnerMonitoringDatabase.Create, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }


            PartnerMonitoringDatabaseUpdateModel model = new PartnerMonitoringDatabaseUpdateModel();
            model.Governorate = Governorate;
            model.District = District;
            model.SubDistrict = SubDistrict;
            model.CommunityName = CommunityName;
            model.Neighborhood = Neighborhood;
            model.Location = Location;
            model.Longitude = Longitude;
            model.Latitude = Latitude;
            model.DateOfReport = DateTime.Now;

            model.PMDHealthUnitOfAchievements = new List<PMDHealthUnitOfAchievement>();
            var tempPMDHealthUnitOfAchievements = (from a in DbPMD.codePmdUnitOfAchievementLanguages
                                                   join b in DbPMD.codePmdUnitOfAchievement on a.UnitOfAchievementGUID equals b.UnitOfAchievementGUID
                                                   where a.LanguageID == LAN && b.UnitOfAchievementCategory == "Health"
                                                   select new
                                                   {
                                                       UnitOfAchievementGUID = a.UnitOfAchievementGUID,
                                                       UnitOfAchievementDescription = a.UnitOfAchievementDescription,
                                                       UnitOfAchievementGuidance = a.UnitOfAchievementGuidance,
                                                       MeasurementTotal = 0
                                                   }).Distinct().OrderBy(x => x.UnitOfAchievementDescription).ToList();
            model.PMDHealthUnitOfAchievements = Mapper.Map(tempPMDHealthUnitOfAchievements, new List<PMDHealthUnitOfAchievement>());

            model.PMDDomesticUnitOfAchievements = new List<PMDDomesticUnitOfAchievement>();
            var tempPMDDomesticUnitOfAchievements = (from a in DbPMD.codePmdUnitOfAchievementLanguages
                                                     join b in DbPMD.codePmdUnitOfAchievement on a.UnitOfAchievementGUID equals b.UnitOfAchievementGUID
                                                     where a.LanguageID == LAN && b.UnitOfAchievementCategory == "Domestic"
                                                     select new
                                                     {
                                                         UnitOfAchievementGUID = a.UnitOfAchievementGUID,
                                                         UnitOfAchievementDescription = a.UnitOfAchievementDescription,
                                                         UnitOfAchievementGuidance = a.UnitOfAchievementGuidance,
                                                         MeasurementTotal = 0
                                                     }).Distinct().OrderBy(x => x.UnitOfAchievementDescription).ToList();
            model.PMDDomesticUnitOfAchievements = Mapper.Map(tempPMDDomesticUnitOfAchievements, new List<PMDDomesticUnitOfAchievement>());


            var OrganizationInstanceGUID = (from a in DbCMS.userProfiles where a.UserProfileGUID == UserProfileGUID select a.OrganizationInstanceGUID).FirstOrDefault();
            model.ImplementingPartnerGUID = OrganizationInstanceGUID;

            model.ObjectiveGUIDBool = false;

            model.MeasurementTotal = 0;


            return View("~/Areas/PMD/Views/PartnerMonitoringDatabase/PartnerMonitoringDatabase.cshtml", model);
        }


        [Route("PMD/PartnerMonitoringDatabase/Update/{PK}")]
        public ActionResult PartnerMonitoringDatabaseUpdate(Guid PK)
        {
            //var OrganizationInstanceGUID = (from a in DbCMS.userProfiles where a.UserProfileGUID == UserProfileGUID select a.OrganizationInstanceGUID).FirstOrDefault();

            if (!CMS.HasAction(Permissions.PartnerMonitoringDatabase.Update, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            PartnerMonitoringDatabaseUpdateModel model = new PartnerMonitoringDatabaseUpdateModel();
            var result = (from a in DbPMD.dataPartnerMonitoringDBs.AsNoTracking().Where(x => x.PartnerMonitoringDBGUID == PK) select a).FirstOrDefault();

            model = Mapper.Map(result, new PartnerMonitoringDatabaseUpdateModel());
            if (String.IsNullOrEmpty(model.Governorate))
            {
                model.GovernorateGUID = (from a in DbPMD.codeOchaLocations.Where(x => x.admin1Pcode == model.Governorate) select a.GovernorateGUID).FirstOrDefault();
            }

            var indicatorGuidance = (from a in DbPMD.codePmdIndicatorLanguages
                                     where a.IndicatorGUID == result.IndicatorGUID && a.LanguageID == LAN
                                     select a.IndicatorGuidance).FirstOrDefault();

            model.IndicatorGuidance = indicatorGuidance;
            if (result.UnitOfAchievementGUID.HasValue)
            {
                var unitOfAchievementGuidance = (from a in DbPMD.codePmdUnitOfAchievementLanguages
                                                 where a.UnitOfAchievementGUID == result.UnitOfAchievementGUID.Value && a.LanguageID == LAN
                                                 select a.UnitOfAchievementGuidance).FirstOrDefault();
                model.UnitOfAchievementGuidance = unitOfAchievementGuidance;
            }

            //Health PPE Indicator
            if (model.IndicatorGUID == Guid.Parse("B3B2B4BF-6D26-4A21-99F4-33F64EF56F3F") || model.IndicatorGUID == Guid.Parse("8FFFC405-BA9B-460B-8FE6-C00A9DBA72D7") || model.IndicatorGUID == Guid.Parse("20FF377C-DB42-4C5F-86A1-228786DFD13B"))
            {
                var tempPMDHealthUnitOfAchievements = (from a in DbPMD.dataPMDHealthUnitOfAchievements
                                                       join b in DbPMD.codePmdUnitOfAchievementLanguages on a.UnitOfAchievementGUID equals b.UnitOfAchievementGUID
                                                       where a.PartnerMonitoringDBGUID == model.PartnerMonitoringDBGUID && b.LanguageID == LAN
                                                       select new
                                                       {
                                                           PMDHealthUnitOfAchievementGUID = a.PMDHealthUnitOfAchievementGUID,
                                                           UnitOfAchievementGUID = a.UnitOfAchievementGUID,
                                                           UnitOfAchievementDescription = b.UnitOfAchievementDescription,
                                                           UnitOfAchievementGuidance = b.UnitOfAchievementGuidance,
                                                           MeasurementTotal = a.MeasurementTotal,
                                                           Active = a.Active,
                                                           dataPMDHealthUnitOfAchievementRowVersion = a.dataPMDHealthUnitOfAchievementRowVersion,
                                                           DeletedOn = a.DeletedOn

                                                       }).Distinct().OrderBy(x => x.UnitOfAchievementDescription).ToList();
                model.PMDHealthUnitOfAchievements = Mapper.Map(tempPMDHealthUnitOfAchievements, new List<PMDHealthUnitOfAchievement>());
            }

            //Domestic
            if (model.ObjectiveGUID == Guid.Parse("09f833d6-f698-4d5b-bb9c-114a56175132"))
            {
                var tempPMDDomesticUnitOfAchievement = (from a in DbPMD.dataPMDDomesticUnitOfAchievement
                                                        join b in DbPMD.codePmdUnitOfAchievementLanguages on a.UnitOfAchievementGUID equals b.UnitOfAchievementGUID
                                                        where a.PartnerMonitoringDBGUID == model.PartnerMonitoringDBGUID && b.LanguageID == LAN
                                                        select new
                                                        {
                                                            PMDDomesticUnitOfAchievementGUID = a.PMDDomesticUnitOfAchievementGUID,
                                                            UnitOfAchievementGUID = a.UnitOfAchievementGUID,
                                                            UnitOfAchievementDescription = b.UnitOfAchievementDescription,
                                                            UnitOfAchievementGuidance = b.UnitOfAchievementGuidance,
                                                            MeasurementTotal = a.MeasurementTotal,
                                                            DistributionDate = a.DistributionDate,
                                                            StockValue = a.StockValue,
                                                            ResponseValue = a.ResponseValue,
                                                            Active = a.Active,
                                                            dataPMDDomesticUnitOfAchievementRowVersion = a.dataPMDDomesticUnitOfAchievementRowVersion,
                                                            DeletedOn = a.DeletedOn

                                                        }).Distinct().OrderBy(x => x.UnitOfAchievementDescription).ToList();
                model.PMDDomesticUnitOfAchievements = Mapper.Map(tempPMDDomesticUnitOfAchievement, new List<PMDDomesticUnitOfAchievement>());
            }


            return View("~/Areas/PMD/Views/PartnerMonitoringDatabase/PartnerMonitoringDatabase.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult PartnerMonitoringDatabaseCreate(PartnerMonitoringDatabaseUpdateModel model)
        {
            Guid GovernorateGUID = (from a in DbPMD.codeOchaLocations.Where(x => x.admin1Pcode == model.Governorate) select a.GovernorateGUID).FirstOrDefault().Value;
            model.GovernorateGUID = GovernorateGUID;

            //var OrganizationInstanceGUID = (from a in DbCMS.userProfiles where a.UserProfileGUID == UserProfileGUID select a.OrganizationInstanceGUID).FirstOrDefault();


            if (!CMS.HasAction(Permissions.PartnerMonitoringDatabase.Create, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            DateTime ExecutionTime = DateTime.Now;
            Guid EntityPK = Guid.NewGuid();
            if (model.PartnerMonitoringDBGUID != Guid.Empty)
            {
                EntityPK = model.PartnerMonitoringDBGUID;
            }

            dataPartnerMonitoringDB dataPartnerMonitoringDB = Mapper.Map(model, new dataPartnerMonitoringDB());
            dataPartnerMonitoringDB.PartnerMonitoringDBGUID = EntityPK;
            dataPartnerMonitoringDB.CreatedBy = UserGUID;
            dataPartnerMonitoringDB.CreatedOn = ExecutionTime;
            dataPartnerMonitoringDB.Active = true;
            dataPartnerMonitoringDB.IsVerified = false;
            dataPartnerMonitoringDB.IsVerifiedByFieldTech = false;
            dataPartnerMonitoringDB.IsApprovedByProgramme = false;
            dataPartnerMonitoringDB.IsApprovedByCountryTech = false;

            if (!String.IsNullOrEmpty(model.Governorate))
            {
                dataPartnerMonitoringDB.GovernorateGUID = model.GovernorateGUID;
            }

            var LonLat = (from a in DbPMD.codeOchaLocations
                          where a.admin4Pcode == model.CommunityName
                          select new
                          {
                              a.Longitude_x,
                              a.Latitude_y
                          }).FirstOrDefault();
            dataPartnerMonitoringDB.Longitude = LonLat.Longitude_x;
            dataPartnerMonitoringDB.Latitude = LonLat.Latitude_y;

            DbPMD.Create(dataPartnerMonitoringDB, Permissions.PartnerMonitoringDatabase.CreateGuid, ExecutionTime, DbCMS);
            //Health PPE Indicator
            if (model.IndicatorGUID == Guid.Parse("AA2A13CB-BF95-4B13-80AF-D87DC811AC2F") ||
                model.IndicatorGUID == Guid.Parse("3B23C7E9-49C2-48AD-A61A-3700D4EBD2EB") ||
                model.IndicatorGUID == Guid.Parse("26249B78-18D4-4BBF-BB9E-430B01D09118") ||
                model.IndicatorGUID == Guid.Parse("273837B8-917F-4024-B942-29A9C78918C5") ||
                model.IndicatorGUID == Guid.Parse("05AB090D-6F5B-40F9-9742-CB96204DDF84"))
            {
                List<dataPMDHealthUnitOfAchievement> dataPMDHealthUnitOfAchievements = new List<dataPMDHealthUnitOfAchievement>();
                dataPMDHealthUnitOfAchievements = Mapper.Map(model.PMDHealthUnitOfAchievements, new List<dataPMDHealthUnitOfAchievement>());
                dataPMDHealthUnitOfAchievements.ForEach(x =>
                {
                    x.PartnerMonitoringDBGUID = EntityPK;
                    x.Active = true;
                });
                DbPMD.CreateBulk(dataPMDHealthUnitOfAchievements, Permissions.PartnerMonitoringDatabase.CreateGuid, ExecutionTime, DbCMS);
            }

            //Domestic
            if (model.ObjectiveGUID == Guid.Parse("09f833d6-f698-4d5b-bb9c-114a56175132"))
            {
                List<dataPMDDomesticUnitOfAchievement> dataPMDDomesticUnitOfAchievements = new List<dataPMDDomesticUnitOfAchievement>();
                dataPMDDomesticUnitOfAchievements = Mapper.Map(model.PMDDomesticUnitOfAchievements, new List<dataPMDDomesticUnitOfAchievement>());
                dataPMDDomesticUnitOfAchievements.ForEach(x =>
                {
                    x.PartnerMonitoringDBGUID = EntityPK;
                    x.Active = true;
                });
                DbPMD.CreateBulk(dataPMDDomesticUnitOfAchievements, Permissions.PartnerMonitoringDatabase.CreateGuid, ExecutionTime, DbCMS);
            }

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.PartnerMonitoringDatabaseAttachementDataTable, ControllerContext, "PartnerMonitoringDatabaseAttachementContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.PartnerMonitoringDatabase.Create, Apps.PMD, new UrlHelper(Request.RequestContext).Action("PartnerMonitoringDatabase/Create", "PartnerMonitoringDatabase", new { Area = "PMD" })), Container = "PartnerMonitoringDatabaseFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.PartnerMonitoringDatabase.Update, Apps.PMD), Container = "PartnerMonitoringDatabaseFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.PartnerMonitoringDatabase.Delete, Apps.PMD), Container = "PartnerMonitoringDatabaseFormControls" });

            try
            {
                DbPMD.SaveChanges();
                DbCMS.SaveChanges();

                List<PrimaryKeyControl> primaryKeyControls = new List<PrimaryKeyControl>();
                primaryKeyControls.Add(DbPMD.PrimaryKeyControl(dataPartnerMonitoringDB));

                return Json(DbPMD.SingleCreateMessage(primaryKeyControls, DbPMD.RowVersionControls(Portal.SingleToList(dataPartnerMonitoringDB)), Partials, null, UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult PartnerMonitoringDatabaseUpdate(PartnerMonitoringDatabaseUpdateModel model)
        {
            Guid GovernorateGUID = (from a in DbPMD.codeOchaLocations.Where(x => x.admin1Pcode == model.Governorate) select a.GovernorateGUID).FirstOrDefault().Value;
            model.GovernorateGUID = GovernorateGUID;
            if (!CMS.HasAction(Permissions.PartnerMonitoringDatabase.Update, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            //if (!ModelState.IsValid || ActiveApplication(model)) return PartialView("~/Areas/CMS/Views/Codes/Applications/_ApplicationForm.cshtml", model);
            DateTime ExecutionTime = DateTime.Now;




            dataPartnerMonitoringDB dataPartnerMonitoringDB = Mapper.Map(model, new dataPartnerMonitoringDB());
            dataPartnerMonitoringDB.UpdatedBy = UserGUID;
            dataPartnerMonitoringDB.UpdatedOn = ExecutionTime;



            if (!String.IsNullOrEmpty(model.Governorate))
            {
                dataPartnerMonitoringDB.GovernorateGUID = model.GovernorateGUID;
            }

            var LonLat = (from a in DbPMD.codeOchaLocations
                          where a.admin4Pcode == model.CommunityName
                          select new
                          {
                              a.Longitude_x,
                              a.Latitude_y
                          }).FirstOrDefault();
            dataPartnerMonitoringDB.Longitude = LonLat.Longitude_x;
            dataPartnerMonitoringDB.Latitude = LonLat.Latitude_y;

            DbPMD.Update(dataPartnerMonitoringDB, Permissions.PartnerMonitoringDatabase.UpdateGuid, ExecutionTime, DbCMS);


            //Health PPE Indicator
            if (model.IndicatorGUID == Guid.Parse("AA2A13CB-BF95-4B13-80AF-D87DC811AC2F") ||
                           model.IndicatorGUID == Guid.Parse("3B23C7E9-49C2-48AD-A61A-3700D4EBD2EB") ||
                           model.IndicatorGUID == Guid.Parse("26249B78-18D4-4BBF-BB9E-430B01D09118") ||
                           model.IndicatorGUID == Guid.Parse("273837B8-917F-4024-B942-29A9C78918C5") ||
                           model.IndicatorGUID == Guid.Parse("05AB090D-6F5B-40F9-9742-CB96204DDF84"))
            {
                List<dataPMDHealthUnitOfAchievement> dataPMDHealthUnitOfAchievements = new List<dataPMDHealthUnitOfAchievement>();
                dataPMDHealthUnitOfAchievements = Mapper.Map(model.PMDHealthUnitOfAchievements, new List<dataPMDHealthUnitOfAchievement>());
                dataPMDHealthUnitOfAchievements.ForEach(x =>
                {
                    x.PartnerMonitoringDBGUID = model.PartnerMonitoringDBGUID;
                    x.Active = true;
                });
                DbPMD.UpdateBulk(dataPMDHealthUnitOfAchievements, Permissions.PartnerMonitoringDatabase.CreateGuid, ExecutionTime, DbCMS);
            }

            //Domestic
            if (model.ObjectiveGUID == Guid.Parse("09f833d6-f698-4d5b-bb9c-114a56175132"))
            {
                List<dataPMDDomesticUnitOfAchievement> dataPMDDomesticUnitOfAchievements = new List<dataPMDDomesticUnitOfAchievement>();
                dataPMDDomesticUnitOfAchievements = Mapper.Map(model.PMDDomesticUnitOfAchievements, new List<dataPMDDomesticUnitOfAchievement>());
                dataPMDDomesticUnitOfAchievements.ForEach(x =>
                {
                    x.PartnerMonitoringDBGUID = model.PartnerMonitoringDBGUID;
                    x.Active = true;
                });
                DbPMD.UpdateBulk(dataPMDDomesticUnitOfAchievements, Permissions.PartnerMonitoringDatabase.CreateGuid, ExecutionTime, DbCMS);
            }

            try
            {
                DbPMD.SaveChanges();
                DbCMS.SaveChanges();

                List<PrimaryKeyControl> primaryKeyControls = new List<PrimaryKeyControl>();
                primaryKeyControls.Add(DbPMD.PrimaryKeyControl(dataPartnerMonitoringDB));

                return Json(DbPMD.SingleUpdateMessage(true, null, primaryKeyControls, DbPMD.RowVersionControls(Portal.SingleToList(dataPartnerMonitoringDB)), null));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PartnerMonitoringDatabaseDelete(dataPartnerMonitoringDB model)
        {
            Guid GovernorateGUID = (from a in DbPMD.codeOchaLocations.Where(x => x.admin1Pcode == model.Governorate) select a.GovernorateGUID).FirstOrDefault().Value;
            model.GovernorateGUID = GovernorateGUID;
            if (!CMS.HasAction(Permissions.PartnerMonitoringDatabase.Delete, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataPartnerMonitoringDB> DeletedPartnerMonitoringDatabaseRecords = DeletePartnerMonitoringDatabaseRecords(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.PartnerMonitoringDatabase.Restore, Apps.PMD), Container = "PartnerMonitoringDatabaseFormControls" });

            try
            {
                int CommitedRows = DbPMD.SaveChanges();
                DbPMD.SaveChanges();
                return Json(DbPMD.SingleDeleteMessage(CommitedRows, DeletedPartnerMonitoringDatabaseRecords.FirstOrDefault(), null, UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PartnerMonitoringDatabaseRestore(dataPartnerMonitoringDB model)
        {
            Guid GovernorateGUID = (from a in DbPMD.codeOchaLocations.Where(x => x.admin1Pcode == model.Governorate) select a.GovernorateGUID).FirstOrDefault().Value;
            model.GovernorateGUID = GovernorateGUID;
            if (!CMS.HasAction(Permissions.PartnerMonitoringDatabase.Restore, Apps.COV))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            //if (ActiveApplication(model))
            //{
            //    return Json(DbCMS.RecordExists());
            //}

            List<dataPartnerMonitoringDB> RestoredPartnerMonitoringDatabaseRecords = RestorePartnerMonitoringDatabaseRecords(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.PartnerMonitoringDatabase.Create, Apps.PMD, new UrlHelper(Request.RequestContext).Action("PartnerMonitoringDatabase/Create", "PartnerMonitoringDatabase", new { Area = "PMD" })), Container = "PartnerMonitoringDatabaseFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.PartnerMonitoringDatabase.Update, Apps.PMD), Container = "PartnerMonitoringDatabaseFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.PartnerMonitoringDatabase.Restore, Apps.PMD), Container = "PartnerMonitoringDatabaseFormControls" });

            try
            {
                int CommitedRows = DbPMD.SaveChanges();
                DbPMD.SaveChanges();
                return Json(DbPMD.SingleRestoreMessage(CommitedRows, RestoredPartnerMonitoringDatabaseRecords, DbPMD.PrimaryKeyControl(RestoredPartnerMonitoringDatabaseRecords.FirstOrDefault()), "", null, UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }


        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult PartnerMonitoringDatabaseDataTableDelete(List<dataPartnerMonitoringDB> models)
        {
            if (!CMS.HasAction(Permissions.PartnerMonitoringDatabase.Delete, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPartnerMonitoringDB> DeletedPartnerMonitoringDatabaseRecords = DeletePartnerMonitoringDatabaseRecords(models);

            try
            {
                DbPMD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPMD.PartialDeleteMessage(DeletedPartnerMonitoringDatabaseRecords, models, DataTableNames.PartnerMonitoringDatabaseDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult PartnerMonitoringDatabaseDataTableRestore(List<dataPartnerMonitoringDB> models)
        {
            if (!CMS.HasAction(Permissions.PartnerMonitoringDatabase.Restore, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPartnerMonitoringDB> RestoredPartnerMonitoringDatabaseRecords = RestorePartnerMonitoringDatabaseRecords(models);

            try
            {
                DbPMD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPMD.PartialRestoreMessage(RestoredPartnerMonitoringDatabaseRecords, models, DataTableNames.PartnerMonitoringDatabaseDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        private List<dataPartnerMonitoringDB> DeletePartnerMonitoringDatabaseRecords(List<dataPartnerMonitoringDB> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataPartnerMonitoringDB> DeletedPartnerMonitoringDatabaseRecords = new List<dataPartnerMonitoringDB>();

            //THIS IS FOR PERMISSION TEST//
            //DO NOT DELETE THIS CODE
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = (from a in DbCMS.codeApplications
            //                    from f in DbCMS.codeApplicationsFactorForTest
            //                    where a.ApplicationGUID == f.ApplicationGUID
            //                    select new { a.ApplicationGUID, a.codeApplicationsRowVersion, f.OperationGUID, f.OrganizationGUID, f.CountryGUID }).AsQueryable().ToString();

            string query = DbPMD.QueryBuilder(models, Permissions.PartnerMonitoringDatabase.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbPMD.Database.SqlQuery<dataPartnerMonitoringDB>(query).ToList();

            foreach (var record in Records)
            {
                DeletedPartnerMonitoringDatabaseRecords.Add(DbPMD.Delete(record, ExecutionTime, Permissions.PartnerMonitoringDatabase.DeleteGuid, DbCMS));
            }

            return DeletedPartnerMonitoringDatabaseRecords;
        }
        private List<dataPartnerMonitoringDB> RestorePartnerMonitoringDatabaseRecords(List<dataPartnerMonitoringDB> models)
        {
            Guid DeleteActionGUID = Permissions.PartnerMonitoringDatabase.DeleteGuid;
            Guid RestoreActionGUID = Permissions.PartnerMonitoringDatabase.RestoreGuid;
            DateTime RestoringTime = DateTime.Now;

            List<dataPartnerMonitoringDB> RestoredPartnerMonitoringDatabaseRecords = new List<dataPartnerMonitoringDB>();

            //THIS IS FOR PERMISSION TEST//
            //DO NOT DELETE THIS CODE
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = (from a in DbCMS.codeApplications
            //                    from f in DbCMS.codeApplicationsFactorForTest
            //                    where a.ApplicationGUID == f.ApplicationGUID
            //                    select new
            //                    {
            //                        a.ApplicationGUID,
            //                        a.codeApplicationsRowVersion,
            //                        C2 = f.OperationGUID + "," + f.OrganizationGUID,
            //                    }).AsQueryable().ToString();//.Replace("[C2]", "[FactorsToken]");


            string query = DbPMD.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, "");

            var Records = DbPMD.Database.SqlQuery<dataPartnerMonitoringDB>(query).ToList();
            foreach (var record in Records)
            {
                //if (!ActiveApplication(record))
                //{
                //    RestoredApplications.Add(DbCMS.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                //}
                RestoredPartnerMonitoringDatabaseRecords.Add(DbPMD.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS));
            }

            return RestoredPartnerMonitoringDatabaseRecords;
        }
        #endregion

        #region Attachement
        public ActionResult PartnerMonitoringDatabaseAttachementDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/PMD/Views/PartnerMonitoringDatabase/_PartnerMonitoringDatabaseAttachementDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<dataPartnerMonitoringAttachement, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<dataPartnerMonitoringAttachement>(DataTable.Filters);
            }

            var Result = (from a in DbPMD.dataPartnerMonitoringAttachement.AsNoTracking().AsExpandable().Where(x => x.Active && x.PartnerMonitoringDBGUID == PK).Where(Predicate)
                          select new
                          {
                              a.PMDAttachementGUID,
                              a.PartnerMonitoringDBGUID,
                              a.FileName,
                              a.FilePath,
                              a.FileExt,
                              a.Active,
                              a.dataPartnerMonitoringAttachementRowVersion,
                          });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result.ToList()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult PartnerMonitoringDatabaseAttachementCreate(Guid FK)
        {
            return PartialView("~/Areas/PMD/Views/PartnerMonitoringDatabase/_PartnerMonitoringDatabaseAttachementUpdateModal.cshtml",
                new dataPartnerMonitoringAttachement { PartnerMonitoringDBGUID = FK });
        }

        [HttpPost]
        public FineUploaderResult PartnerMonitoringDatabaseAttachementCreate(FineUpload upload, Guid PartnerMonitoringDBGUID)
        {
            if (!FileTypeValidator.IsExcel(upload.InputStream) && !FileTypeValidator.IsWord(upload.InputStream) && !FileTypeValidator.IsPDF(upload.InputStream) && !FileTypeValidator.IsImage(upload.InputStream))
            {
                return new FineUploaderResult(false, new { error = "error: allowed files (excel, word, pdf, images)" });
            }
            var _stearm = upload.InputStream;
            string FolderPath = Server.MapPath("~/Areas/PMD/Uploads/" + PartnerMonitoringDBGUID.ToString());
            Directory.CreateDirectory(FolderPath);
            string fileName = "";
            try
            {
                var asfad = upload.FileName.Split('\\');
                fileName = upload.FileName.Split('\\').Last();

                //fileName = 
            }
            catch { fileName = upload.FileName; }
            string FilePath = FolderPath + "/" + fileName;
            using (var fileStream = System.IO.File.Create(FilePath))
            {
                upload.InputStream.Seek(0, SeekOrigin.Begin);
                upload.InputStream.CopyTo(fileStream);
            }
            dataPartnerMonitoringAttachement dataPartnerMonitoringAttachement = new dataPartnerMonitoringAttachement();
            dataPartnerMonitoringAttachement.PartnerMonitoringDBGUID = PartnerMonitoringDBGUID;
            dataPartnerMonitoringAttachement.FilePath = FilePath;
            dataPartnerMonitoringAttachement.FileName = fileName;
            dataPartnerMonitoringAttachement.FileExt = fileName.Split('.').Last();

            try
            {
                DbPMD.CreateNoAudit(dataPartnerMonitoringAttachement);
                DbPMD.SaveChanges();
                return new FineUploaderResult(true, new { path = FilePath, success = true });
            }
            catch (Exception ex)
            {
                return new FineUploaderResult(false, new { error = "error: " + ex.Message });
            }
        }

        [Route("PMD/PartnerMonitoringDatabase/PartnerMonitoringDatabaseAttachementDownload/{PK}")]
        public FileResult PartnerMonitoringDatabaseAttachementDownload(Guid PK)
        {
            Guid AppAttachementGUID = PK;

            dataPartnerMonitoringAttachement dataPartnerMonitoringAttachement = (from a in DbPMD.dataPartnerMonitoringAttachement.Where(x => x.PMDAttachementGUID == PK) select a).FirstOrDefault();
            string FolderPath = Server.MapPath("~/Areas/PMD/Uploads/" + dataPartnerMonitoringAttachement.PartnerMonitoringDBGUID.ToString());

            string FileName = dataPartnerMonitoringAttachement.FileName;
            string DownloadURL = FolderPath + "\\" + FileName;

            byte[] fileBytes = System.IO.File.ReadAllBytes(@"" + DownloadURL + "");

            try
            {
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region Templates and Bulk


        [Route("PMD/PartnerMonitoringDatabase/PartnerMonitoringDatabaseMainTemplatesDownload")]
        public FileResult PartnerMonitoringDatabaseMainTemplatesDownload()
        {


            string FolderPath = Server.MapPath("~/Areas/PMD/Templates/");

            string FileName = "Main Template.xlsx";
            string DownloadURL = FolderPath + "\\" + FileName;

            byte[] fileBytes = System.IO.File.ReadAllBytes(@"" + DownloadURL + "");

            try
            {
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        [Route("PMD/PartnerMonitoringDatabase/PartnerMonitoringDatabaseHealthTemplatesDownload")]
        public FileResult PartnerMonitoringDatabaseHealthTemplatesDownload()
        {


            string FolderPath = Server.MapPath("~/Areas/PMD/Templates/");

            string FileName = "Health Template.xlsx";
            string DownloadURL = FolderPath + "\\" + FileName;

            byte[] fileBytes = System.IO.File.ReadAllBytes(@"" + DownloadURL + "");

            try
            {
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        [Route("PMD/PartnerMonitoringDatabase/PartnerMonitoringDatabaseCRITemplatesDownload")]
        public FileResult PartnerMonitoringDatabaseCRITemplatesDownload()
        {

            string FolderPath = Server.MapPath("~/Areas/PMD/Templates/");

            string FileName = "CRI Template.xlsx";
            string DownloadURL = FolderPath + "\\" + FileName;

            byte[] fileBytes = System.IO.File.ReadAllBytes(@"" + DownloadURL + "");

            try
            {
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        [Route("PMD/PartnerMonitoringDatabase/PartnerMonitoringDatabaseBulkCreate")]
        public ActionResult PartnerMonitoringDatabaseBulkCreate()
        {
            if (!CMS.HasAction(Permissions.PartnerMonitoringDatabase.Create, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }


            //var OrganizationInstanceGUID = (from a in DbCMS.userProfiles where a.UserProfileGUID == UserProfileGUID select a.OrganizationInstanceGUID).FirstOrDefault();
            //model.ImplementingPartnerGUID = OrganizationInstanceGUID;

            return PartialView("~/Areas/PMD/Views/PartnerMonitoringDatabase/_PartnerMonitoringDatabaseBulkCreateModal.cshtml", new dataPMDBulkUpload { PMDBulkUploadGUID = Guid.Empty });
        }

        [HttpPost]
        public FineUploaderResult PartnerMonitoringDatabaseBulkCreate(FineUpload upload, string PMDTemplateType)
        {
            DateTime ExecutionTime = DateTime.Now;

            if (!CMS.HasAction(Permissions.PartnerMonitoringDatabase.Create, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!FileTypeValidator.IsExcel(upload.InputStream))
            {
                return new FineUploaderResult(false, new { error = "error: allowed files (excel)" });
            }
            var _stearm = upload.InputStream;


            dataPMDBulkUpload dataPMDBulkUpload = new dataPMDBulkUpload();
            dataPMDBulkUpload.PMDBulkUploadGUID = Guid.NewGuid();
            dataPMDBulkUpload.PMDTemplateType = PMDTemplateType;
            string FolderPath = Server.MapPath("~/Areas/PMD/BulkUploads/" + dataPMDBulkUpload.PMDBulkUploadGUID.ToString());
            Directory.CreateDirectory(FolderPath);
            string fileName = "";
            try
            {
                var asfad = upload.FileName.Split('\\');
                fileName = upload.FileName.Split('\\').Last();

                //fileName = 
            }
            catch { fileName = upload.FileName; }
            string FilePath = FolderPath + "/" + fileName;
            using (var fileStream = System.IO.File.Create(FilePath))
            {
                upload.InputStream.Seek(0, SeekOrigin.Begin);
                upload.InputStream.CopyTo(fileStream);
            }



            dataPMDBulkUpload.FilePath = FilePath;
            dataPMDBulkUpload.FileName = fileName;
            dataPMDBulkUpload.FileExt = fileName.Split('.').Last();
            dataPMDBulkUpload.CreatedBy = UserGUID;
            dataPMDBulkUpload.CreatedOn = ExecutionTime;

            try
            {
                DbPMD.Create(dataPMDBulkUpload, Permissions.PartnerMonitoringDatabase.CreateGuid, ExecutionTime, DbCMS);

                DbPMD.SaveChanges();
                return new FineUploaderResult(true, new { path = FilePath, success = true });
            }
            catch (Exception ex)
            {
                return new FineUploaderResult(false, new { error = "error: " + ex.Message });
            }
        }

        [Route("PMD/PartnerMonitoringDatabase/PartnerMonitoringDatabaseAttachementDownload/{PK}")]

        #endregion

        #region Data Extraction
        [Route("PMD/PartnerMonitoringDatabase/GenerateExcelVersion")]
        public ActionResult GenerateExcelVersion()
        {
            if (!CMS.HasAction(Permissions.PartnerMonitoringDatabase.Access, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<string> AuthorizedListByPartner = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.PartnerMonitoringDatabase.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            IQueryable<v_BulkExtraction> queryable_records = (from a in DbPMD.v_BulkExtraction
                                                              where a.Active
                                                              select a).Where(x => AuthorizedListByPartner.Contains(x.ImplementingPartnerGUID.ToString()));

            if (CMS.HasAction(Permissions.PartnerMonitoringDatabaseFieldTechVerify.Create, Apps.PMD))
            {

                List<string> AuthorizedListByLocation = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.PartnerMonitoringDatabaseFieldTechVerify.CreateGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

                queryable_records = (from a in DbPMD.v_BulkExtraction
                                     where a.Active
                                     select a)
                           .Where(x => AuthorizedListByPartner.Contains(x.ImplementingPartnerGUID.ToString()))
                           .Where(x => AuthorizedListByLocation.Contains(x.GovernorateGUID.ToString()));
            }
            else if (CMS.HasAction(Permissions.PartnerMonitoringDatabaseSpecificObjective.Create, Apps.PMD))
            {
                List<string> AuthorizedListByObjectives = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.PartnerMonitoringDatabaseSpecificObjective.CreateGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();


                queryable_records = (from a in DbPMD.v_BulkExtraction
                                     where a.Active
                                     select a)
                       .Where(x => AuthorizedListByPartner.Contains(x.ImplementingPartnerGUID.ToString()))
                       .Where(x => AuthorizedListByObjectives.Contains(x.ObjectiveGUID.ToString()));

            }

            string sourceFile = Server.MapPath("~/Areas/PMD/Templates/Extraction.xlsx");

            string DisFolder = Server.MapPath("~/Areas/PMD/Templates/Extraction" + DateTime.Now.ToBinary() + ".xlsx");

            System.IO.File.Copy(sourceFile, DisFolder);

            List<v_BulkExtraction> records = queryable_records.ToList();

            using (var package = new ExcelPackage(new FileInfo(DisFolder)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                worksheet.Row(1).Height = 20;
                worksheet.Row(1).Style.Font.Bold = true;

                int i = 3;

                for (int idx = 0; idx < records.Count; idx++)
                {



                    worksheet.Cells["A" + i].Value = records[idx].DateOfReport;    //Date of Report    A
                    worksheet.Cells["B" + i].Value = records[idx].admin1Name_en;   //Governorate        B
                    worksheet.Cells["C" + i].Value = records[idx].admin2Name_en;   //District           C
                    worksheet.Cells["D" + i].Value = records[idx].admin3Name_en;   //Sub District      D
                    worksheet.Cells["E" + i].Value = records[idx].admin4Name_en;   //Community Name    E
                    worksheet.Cells["F" + i].Value = records[idx].admin5Name_en; //Neighborhood      F
                    worksheet.Cells["G" + i].Value = records[idx].Location;   //Location          G
                    worksheet.Cells["H" + i].Value = records[idx].PMDCommunityCenter;//Community Center Location  H
                    worksheet.Cells["I" + i].Value = records[idx].Longitude;  //Longitude         I
                    worksheet.Cells["J" + i].Value = records[idx].Latitude;   //Latitude           J
                    worksheet.Cells["K" + i].Value = records[idx].ObjectiveDescription; //Objective        K
                    worksheet.Cells["L" + i].Value = records[idx].OutputDescription;    //Output            L
                    worksheet.Cells["M" + i].Value = records[idx].IndicatorDescription;//Indicator         M
                    worksheet.Cells["N" + i].Value = records[idx].ObjectiveDescription;//Objective Status  N  
                    worksheet.Cells["O" + i].Value = records[idx].MeasurementTotal;//Total Achievement O 
                    worksheet.Cells["P" + i].Value = records[idx].IdpTotal;//IDPs          P
                    worksheet.Cells["Q" + i].Value = records[idx].RefTotal;//Refugees       Q
                    worksheet.Cells["R" + i].Value = records[idx].RetTotal;//Returnees       R
                    worksheet.Cells["S" + i].Value = records[idx].HostCommunity;//Host Community     S
                    worksheet.Cells["T" + i].Value = records[idx].Boys;//Boys(less than 18)	T
                    worksheet.Cells["U" + i].Value = records[idx].Girls;//Girls(less than 18)    U
                    worksheet.Cells["V" + i].Value = records[idx].Men;//Men(18 - 59 years old)     V
                    worksheet.Cells["W" + i].Value = records[idx].Women;//Women - (18 - 59 years old)	 W
                    worksheet.Cells["X" + i].Value = records[idx].ElderlyMen; //Elderly Men(60 + years old)	  X
                    worksheet.Cells["Y" + i].Value = records[idx].ElderlyWomen;//Elderly Women(60 + years old)	  Y
                    worksheet.Cells["Z" + i].Value = records[idx].IsDirectActivity == true ? "Yes" : "No";//Direct Activity                Z
                    worksheet.Cells["AA" + i].Value = records[idx].PartnerDescription;//Implementing Partner            AA
                    worksheet.Cells["AB" + i].Value = records[idx].Comments;//Comments                        AB

                    #region Health
                    if (records[idx].IndicatorGUID == Guid.Parse("B3B2B4BF-6D26-4A21-99F4-33F64EF56F3F") || records[idx].IndicatorGUID == Guid.Parse("8FFFC405-BA9B-460B-8FE6-C00A9DBA72D7") || records[idx].IndicatorGUID == Guid.Parse("20FF377C-DB42-4C5F-86A1-228786DFD13B"))
                    {
                        Guid PartnerMonitoringDBGUID = records[idx].PartnerMonitoringDBGUID;
                        var healthData = (from a in DbPMD.dataPMDHealthUnitOfAchievements
                                          where a.Active && a.PartnerMonitoringDBGUID == PartnerMonitoringDBGUID
                                          select a).ToList();

                        //A9482B4A-0FFE-4A78-8A80-0D28B75E4382
                        worksheet.Cells["AC" + i].Value = "";//Fabric Mask          AC
                        worksheet.Cells["AD" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("A9482B4A-0FFE-4A78-8A80-0D28B75E4382")).Select(x => x.MeasurementTotal).FirstOrDefault(); //Total Achievement    AD

                        //E5EC5422-14DA-4A56-A328-82C0EBFBB577
                        worksheet.Cells["AE" + i].Value = "";//Alcohol - Based Hand Rub Solution or gel   AE
                        worksheet.Cells["AF" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("E5EC5422-14DA-4A56-A328-82C0EBFBB577")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                         AF

                        //EA9B2493-63A7-4996-BCDB-E55D2023E339
                        worksheet.Cells["AG" + i].Value = "";//ALCOHOL-BASED surface sterilizer          AG
                        worksheet.Cells["AH" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("EA9B2493-63A7-4996-BCDB-E55D2023E339")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                          AH

                        //4651B736-F84A-48CE-B534-BB32837DC111
                        worksheet.Cells["AI" + i].Value = "";//Apron, disposable                        AI
                        worksheet.Cells["AJ" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("4651B736-F84A-48CE-B534-BB32837DC111")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                     AJ

                        //FA217F48-4A22-463B-845D-F8156C147452
                        worksheet.Cells["AK" + i].Value = "";//Apron, Heavy duty                       AK
                        worksheet.Cells["AL" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("FA217F48-4A22-463B-845D-F8156C147452")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                         AL

                        //63816BF1-BA4A-40EB-93A2-17D9B4F9E359
                        worksheet.Cells["AM" + i].Value = "";//Bag for medical waste                       AM
                        worksheet.Cells["AN" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("63816BF1-BA4A-40EB-93A2-17D9B4F9E359")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                             AN

                        //B8C60840-90A1-4258-B1AD-3441EB2C5630
                        worksheet.Cells["AO" + i].Value = "";//Body bag                                        AO
                        worksheet.Cells["AP" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("B8C60840-90A1-4258-B1AD-3441EB2C5630")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                 AP

                        //89136FA0-E689-41BE-8549-08D07F14F760
                        worksheet.Cells["AQ" + i].Value = "";//Chlorine                                            AQ
                        worksheet.Cells["AR" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("89136FA0-E689-41BE-8549-08D07F14F760")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                     AR

                        //12C3C36C-5748-4E53-A4F7-3130F7C2F37C
                        worksheet.Cells["AS" + i].Value = "";//COVERALL, fluid resist                         AS
                        worksheet.Cells["AT" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("12C3C36C-5748-4E53-A4F7-3130F7C2F37C")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                  AT

                        //25DB9430-1F40-4B6B-B716-7CB8CCA8BBCE
                        worksheet.Cells["AU" + i].Value = "";//COVERALL, fluid resist.Reusable                          AU
                        worksheet.Cells["AV" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("25DB9430-1F40-4B6B-B716-7CB8CCA8BBCE")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                  AV

                        //2AF6E8F7-DA91-4495-BEB7-F87E6AA0385B
                        worksheet.Cells["AW" + i].Value = "";//COVID - 19 Medical - PPE  KIT                        AW
                        worksheet.Cells["AX" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("2AF6E8F7-DA91-4495-BEB7-F87E6AA0385B")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                   AX

                        //DEAECB97-D47D-4346-B88E-CE912B1E9940
                        worksheet.Cells["AY" + i].Value = "";//Disinfectant for spraying and fumigation             AY
                        worksheet.Cells["AZ" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("DEAECB97-D47D-4346-B88E-CE912B1E9940")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                    AZ

                        //A14CAE33-A7BD-457E-BB97-EC8EBF8B3D6B
                        worksheet.Cells["BA" + i].Value = "";//Dispenser                                            BA
                        worksheet.Cells["BB" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("A14CAE33-A7BD-457E-BB97-EC8EBF8B3D6B")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                    BB

                        //0CC0CC2F-B42A-4603-97D4-33BF69258D70
                        worksheet.Cells["BC" + i].Value = "";//Face Shield                                          BC
                        worksheet.Cells["BD" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("0CC0CC2F-B42A-4603-97D4-33BF69258D70")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                      BD

                        //8D690F2A-DC17-4671-9E79-7A6B1758F035
                        worksheet.Cells["BE" + i].Value = "";//Fumigation Machine or spray pump                        BE
                        worksheet.Cells["BF" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("8D690F2A-DC17-4671-9E79-7A6B1758F035")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                       BF

                        //201375F3-413E-4B61-ABB5-C675EDA55AEF
                        worksheet.Cells["BG" + i].Value = "";//Glove Examination                                       BG
                        worksheet.Cells["BH" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("201375F3-413E-4B61-ABB5-C675EDA55AEF")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                       BH

                        //ADA259BD-7044-4D12-8EFB-7C007749F3A3
                        worksheet.Cells["BI" + i].Value = "";//Gloves Surgical                                         BI
                        worksheet.Cells["BJ" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("ADA259BD-7044-4D12-8EFB-7C007749F3A3")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                       BJ

                        //FE9FD561-E394-48E5-A097-419E16FD9BF5
                        worksheet.Cells["BK" + i].Value = "";//Gloves, heavy duty, cleaning                            BK
                        worksheet.Cells["BL" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("FE9FD561-E394-48E5-A097-419E16FD9BF5")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                       BL

                        //D6776B02-D45A-460C-8A52-08B84EDEF8CE
                        worksheet.Cells["BM" + i].Value = "";//Goggles, glasses  Protective                            BM
                        worksheet.Cells["BN" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("D6776B02-D45A-460C-8A52-08B84EDEF8CE")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                       BN

                        //8EE72D68-CABA-48E0-84AA-BF53F2001A4E
                        worksheet.Cells["BO" + i].Value = "";//Gown, protective disposible                             BO
                        worksheet.Cells["BP" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("8EE72D68-CABA-48E0-84AA-BF53F2001A4E")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                       BP

                        //6EB294B3-13DB-419A-A436-9DCE6521EBD1
                        worksheet.Cells["BQ" + i].Value = "";//Gum boots                                               BQ
                        worksheet.Cells["BR" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("6EB294B3-13DB-419A-A436-9DCE6521EBD1")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                       BR

                        //D9BEDD20-D9E4-4438-A3CA-0F4240B075F2
                        worksheet.Cells["BS" + i].Value = "";//Hand Tissue                                             BS
                        worksheet.Cells["BT" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("D9BEDD20-D9E4-4438-A3CA-0F4240B075F2")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                       BT

                        //B2A038B9-1BFC-4C83-AF75-DA555C960868
                        worksheet.Cells["BU" + i].Value = "";//Head cover                                              BU
                        worksheet.Cells["BV" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("B2A038B9-1BFC-4C83-AF75-DA555C960868")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                       BV

                        //344FEE4F-3078-47FC-A53F-6C4988ED4601
                        worksheet.Cells["BW" + i].Value = "";//High level surface Disinfectant(quaternary amonium different concentration or similar)  BW  
                        worksheet.Cells["BX" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("344FEE4F-3078-47FC-A53F-6C4988ED4601")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                                                       BX

                        //9729C18A-33FF-4798-8A49-69BCEA0BF926
                        worksheet.Cells["BY" + i].Value = "";//Mask, particulate respirator                                                            BY
                        worksheet.Cells["BZ" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("9729C18A-33FF-4798-8A49-69BCEA0BF926")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                                                       BZ

                        //DB06DD8D-6301-42DF-9C79-91BEB5019D8D
                        worksheet.Cells["CA" + i].Value = "";//Non - contact remote infrared thermometer for body temperature measurement.             CA
                        worksheet.Cells["CB" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("DB06DD8D-6301-42DF-9C79-91BEB5019D8D")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                                                       CB

                        //5571C401-ABEC-4E04-9397-F1B4AE880317
                        worksheet.Cells["CC" + i].Value = "";//Medical Masks IIR( for health professionals)                                            CC
                        worksheet.Cells["CD" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("5571C401-ABEC-4E04-9397-F1B4AE880317")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                                                       CD

                        //2938ECC0-7A58-4716-AC43-0B87C51E47D9
                        worksheet.Cells["CE" + i].Value = "";//Patient Hospital Gown                                                                   CE
                        worksheet.Cells["CF" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("2938ECC0-7A58-4716-AC43-0B87C51E47D9")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                                                       CF

                        //F4B948B8-5FFA-4122-B7DD-153C971816D8
                        worksheet.Cells["CG" + i].Value = "";//Procedural-surgical  mask(for non health workers)                                       CG
                        worksheet.Cells["CH" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("F4B948B8-5FFA-4122-B7DD-153C971816D8")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                                                       CH

                        //455CA0B3-9369-4E1C-9104-A7C9062EE0EC
                        worksheet.Cells["CI" + i].Value = "";//Scrubs, pants                                                                           CI
                        worksheet.Cells["CJ" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("455CA0B3-9369-4E1C-9104-A7C9062EE0EC")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                                                       CJ

                        //C97ABDA4-F9B1-4482-8F13-4B554D60D4C7
                        worksheet.Cells["CK" + i].Value = "";//Scrubs, tops                                                                            CK
                        worksheet.Cells["CL" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("C97ABDA4-F9B1-4482-8F13-4B554D60D4C7")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                                                       CL

                        //DDF80B75-6918-4E2F-886C-FDCC5FCA838E
                        worksheet.Cells["CM" + i].Value = "";//Shampoo for adults                                                                      CM
                        worksheet.Cells["CN" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("DDF80B75-6918-4E2F-886C-FDCC5FCA838E")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                                                       CN

                        //614BD127-4331-4777-A0B7-646EAD5831F8
                        worksheet.Cells["CO" + i].Value = "";//Shampoo for children                                                                    CO
                        worksheet.Cells["CP" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("614BD127-4331-4777-A0B7-646EAD5831F8")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                                                       CP

                        //4EB22CFE-1D5D-4A9E-90DB-393D301F463C
                        worksheet.Cells["CQ" + i].Value = "";//Shoe Cover                                                                              CQ
                        worksheet.Cells["CR" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("4EB22CFE-1D5D-4A9E-90DB-393D301F463C")).Select(x => x.MeasurementTotal).FirstOrDefault(); //Total Achievement                                                                       CR

                        //19A63E31-B987-4DA3-BB8F-B35337FE05FA
                        worksheet.Cells["CS" + i].Value = "";//Soap - liquid                                                                           CS
                        worksheet.Cells["CT" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("19A63E31-B987-4DA3-BB8F-B35337FE05FA")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                                                       CT

                        //8B33638A-21DE-4819-A397-F42598726E70
                        worksheet.Cells["CU" + i].Value = ""; //Soap - Bars                                                                             CU
                        worksheet.Cells["CV" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("8B33638A-21DE-4819-A397-F42598726E70")).Select(x => x.MeasurementTotal).FirstOrDefault(); //Total Achievement                                                                       CV

                        //02181A2F-D4A7-48B6-B6D9-F22C91FEBD30
                        worksheet.Cells["CW" + i].Value = ""; //Sprayer                                                                                 CW
                        worksheet.Cells["CX" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("02181A2F-D4A7-48B6-B6D9-F22C91FEBD30")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                                                       CX

                        //510029D0-ED78-4B6D-947A-B241C35768B1
                        worksheet.Cells["CY" + i].Value = "";//Wall mount Base for bottles(soap, sanitizers)                                           CY
                        worksheet.Cells["CZ" + i].Value = healthData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("510029D0-ED78-4B6D-947A-B241C35768B1")).Select(x => x.MeasurementTotal).FirstOrDefault();//Total Achievement                                                                       CZ

                    }
                    #endregion

                    #region CRI      
                    if (records[idx].ObjectiveGUID == Guid.Parse("09f833d6-f698-4d5b-bb9c-114a56175132"))
                    {
                        Guid PartnerMonitoringDBGUID = records[idx].PartnerMonitoringDBGUID;
                        var criData = (from a in DbPMD.dataPMDDomesticUnitOfAchievement
                                       where a.Active && a.PartnerMonitoringDBGUID == PartnerMonitoringDBGUID
                                       select a).ToList();


                        // C0EEF205-1CC9-4BF6-AC59-CF74F97023B5
                        worksheet.Cells["DA" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("C0EEF205-1CC9-4BF6-AC59-CF74F97023B5")).Select(x => x.MeasurementTotal).FirstOrDefault(); //Winter NFI kit      DA
                        worksheet.Cells["DB" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("C0EEF205-1CC9-4BF6-AC59-CF74F97023B5")).Select(x => x.DistributionDate).FirstOrDefault(); //Distribution Date   DB
                        worksheet.Cells["DC" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("C0EEF205-1CC9-4BF6-AC59-CF74F97023B5")).Select(x => x.StockValue).FirstOrDefault();      //Stock Value         DC
                        worksheet.Cells["DD" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("C0EEF205-1CC9-4BF6-AC59-CF74F97023B5")).Select(x => x.ResponseValue).FirstOrDefault();   //Response Value     DD

                        //73464AEC-12D0-414B-9D55-9F61401C4D76
                        worksheet.Cells["DE" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("73464AEC-12D0-414B-9D55-9F61401C4D76")).Select(x => x.MeasurementTotal).FirstOrDefault(); //Winter clothing kits  DE  
                        worksheet.Cells["DF" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("73464AEC-12D0-414B-9D55-9F61401C4D76")).Select(x => x.DistributionDate).FirstOrDefault(); //Distribution Date   DF
                        worksheet.Cells["DG" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("73464AEC-12D0-414B-9D55-9F61401C4D76")).Select(x => x.StockValue).FirstOrDefault();       //Stock Value         DG
                        worksheet.Cells["DH" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("73464AEC-12D0-414B-9D55-9F61401C4D76")).Select(x => x.ResponseValue).FirstOrDefault();   //Response Value      DH

                        // C5DA5B9A-F859-45E1-B556-343CB1C9F6EC
                        worksheet.Cells["DI" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("C5DA5B9A-F859-45E1-B556-343CB1C9F6EC")).Select(x => x.MeasurementTotal).FirstOrDefault();// Warm clothes / Footwear - Adults(18 and over)  DI
                        worksheet.Cells["DJ" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("C5DA5B9A-F859-45E1-B556-343CB1C9F6EC")).Select(x => x.DistributionDate).FirstOrDefault();// Distribution Date                            DJ
                        worksheet.Cells["DK" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("C5DA5B9A-F859-45E1-B556-343CB1C9F6EC")).Select(x => x.StockValue).FirstOrDefault();      // Stock Value                                    DK
                        worksheet.Cells["DL" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("C5DA5B9A-F859-45E1-B556-343CB1C9F6EC")).Select(x => x.ResponseValue).FirstOrDefault();   // Response Value                                   DL

                        //48E1FBF0-0943-4ACD-B0E5-29BD02466675
                        worksheet.Cells["DM" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("48E1FBF0-0943-4ACD-B0E5-29BD02466675")).Select(x => x.MeasurementTotal).FirstOrDefault();   // Warm clothes / Footwear - Children(Under 18)       DM
                        worksheet.Cells["DN" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("48E1FBF0-0943-4ACD-B0E5-29BD02466675")).Select(x => x.DistributionDate).FirstOrDefault();// Distribution Date                                    DN
                        worksheet.Cells["DO" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("48E1FBF0-0943-4ACD-B0E5-29BD02466675")).Select(x => x.StockValue).FirstOrDefault();      // Stock Value                                          DO
                        worksheet.Cells["DP" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("48E1FBF0-0943-4ACD-B0E5-29BD02466675")).Select(x => x.ResponseValue).FirstOrDefault();   // Response Value                                        DP

                        //56698CAC-66D5-4ED0-88F3-189E693CDE5D                                                                                                                                                                                                                                 //                                                        
                        worksheet.Cells["DQ" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("56698CAC-66D5-4ED0-88F3-189E693CDE5D")).Select(x => x.MeasurementTotal).FirstOrDefault();// Quilt / High thermal blankets                           DQ
                        worksheet.Cells["DR" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("56698CAC-66D5-4ED0-88F3-189E693CDE5D")).Select(x => x.DistributionDate).FirstOrDefault();// Distribution Date                                       DR
                        worksheet.Cells["DS" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("56698CAC-66D5-4ED0-88F3-189E693CDE5D")).Select(x => x.StockValue).FirstOrDefault();      // Stock Value                                             DS
                        worksheet.Cells["DT" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("56698CAC-66D5-4ED0-88F3-189E693CDE5D")).Select(x => x.ResponseValue).FirstOrDefault();   // Response Value                                          DT

                        //82185B7B-13D6-4BFF-8FA8-3C0B6863CB64                                                                                                                                                                                                                                 // 
                        worksheet.Cells["DU" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("82185B7B-13D6-4BFF-8FA8-3C0B6863CB64")).Select(x => x.MeasurementTotal).FirstOrDefault();// Sleeping Bags                                       DU
                        worksheet.Cells["DV" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("82185B7B-13D6-4BFF-8FA8-3C0B6863CB64")).Select(x => x.DistributionDate).FirstOrDefault();// Distribution Date                                   DV
                        worksheet.Cells["DW" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("82185B7B-13D6-4BFF-8FA8-3C0B6863CB64")).Select(x => x.StockValue).FirstOrDefault();      // Stock Value                                         DW
                        worksheet.Cells["DX" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("82185B7B-13D6-4BFF-8FA8-3C0B6863CB64")).Select(x => x.ResponseValue).FirstOrDefault();   // Response Value                                      DX

                        //8A0FA854-45CC-4BB7-8A3C-18F8797C6BC5                                                                                                                                                                                                                                 // 
                        worksheet.Cells["DY" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("8A0FA854-45CC-4BB7-8A3C-18F8797C6BC5")).Select(x => x.MeasurementTotal).FirstOrDefault();// Scarf                                               DY
                        worksheet.Cells["DZ" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("8A0FA854-45CC-4BB7-8A3C-18F8797C6BC5")).Select(x => x.DistributionDate).FirstOrDefault();// Distribution Date                                   DZ
                        worksheet.Cells["EA" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("8A0FA854-45CC-4BB7-8A3C-18F8797C6BC5")).Select(x => x.StockValue).FirstOrDefault();      // Stock Value                                         EA
                        worksheet.Cells["EB" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("8A0FA854-45CC-4BB7-8A3C-18F8797C6BC5")).Select(x => x.ResponseValue).FirstOrDefault();   // Response Value                                      EB

                        //A84EB6D5-C34D-46B8-9951-3929EF6E9C71                                                                                                                                                                                                                                 // 
                        worksheet.Cells["EC" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("A84EB6D5-C34D-46B8-9951-3929EF6E9C71")).Select(x => x.MeasurementTotal).FirstOrDefault();// Hats                                                EC
                        worksheet.Cells["ED" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("A84EB6D5-C34D-46B8-9951-3929EF6E9C71")).Select(x => x.DistributionDate).FirstOrDefault();// Distribution Date                                   ED
                        worksheet.Cells["EE" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("A84EB6D5-C34D-46B8-9951-3929EF6E9C71")).Select(x => x.StockValue).FirstOrDefault();      // Stock Value                                         EE
                        worksheet.Cells["EF" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("A84EB6D5-C34D-46B8-9951-3929EF6E9C71")).Select(x => x.ResponseValue).FirstOrDefault();   // Response Value                                      EF

                        // 580118C2-167A-4AAE-A66F-9FE7F9E127A8                                                                                                                                                                                                                               // 
                        worksheet.Cells["EG" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("580118C2-167A-4AAE-A66F-9FE7F9E127A8")).Select(x => x.MeasurementTotal).FirstOrDefault(); // Boots                                               EG
                        worksheet.Cells["EH" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("580118C2-167A-4AAE-A66F-9FE7F9E127A8")).Select(x => x.DistributionDate).FirstOrDefault(); // Distribution Date                                   EH
                        worksheet.Cells["EI" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("580118C2-167A-4AAE-A66F-9FE7F9E127A8")).Select(x => x.StockValue).FirstOrDefault();       // Stock Value                                         EI
                        worksheet.Cells["EJ" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("580118C2-167A-4AAE-A66F-9FE7F9E127A8")).Select(x => x.ResponseValue).FirstOrDefault();    // Response Value                                      EJ

                        // 3D3B4C5F-92BD-47BC-AB95-0519C610ABAA                                   // 
                        worksheet.Cells["EK" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("3D3B4C5F-92BD-47BC-AB95-0519C610ABAA")).Select(x => x.MeasurementTotal).FirstOrDefault(); // Jackets                                             EK
                        worksheet.Cells["EL" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("3D3B4C5F-92BD-47BC-AB95-0519C610ABAA")).Select(x => x.DistributionDate).FirstOrDefault(); // Distribution Date                                   EL
                        worksheet.Cells["EM" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("3D3B4C5F-92BD-47BC-AB95-0519C610ABAA")).Select(x => x.StockValue).FirstOrDefault();       // Stock Value                                         EM
                        worksheet.Cells["EN" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("3D3B4C5F-92BD-47BC-AB95-0519C610ABAA")).Select(x => x.ResponseValue).FirstOrDefault();    // Response Value                                      EN

                        // F21EE1B7-85CB-41DD-8CFF-F9974B0DDE5F                                   // 
                        worksheet.Cells["EO" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("F21EE1B7-85CB-41DD-8CFF-F9974B0DDE5F")).Select(x => x.MeasurementTotal).FirstOrDefault(); // Additional plastic sheet / Water proof flooring     EO
                        worksheet.Cells["EP" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("F21EE1B7-85CB-41DD-8CFF-F9974B0DDE5F")).Select(x => x.DistributionDate).FirstOrDefault(); // Distribution Date                                   EP
                        worksheet.Cells["EQ" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("F21EE1B7-85CB-41DD-8CFF-F9974B0DDE5F")).Select(x => x.StockValue).FirstOrDefault();       // Stock Value                                         EQ
                        worksheet.Cells["ER" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("F21EE1B7-85CB-41DD-8CFF-F9974B0DDE5F")).Select(x => x.ResponseValue).FirstOrDefault();    // Response Value                                      ER

                        //  7D1FE739-AE53-47BE-847B-DD829CDD308C                                 // 
                        worksheet.Cells["ES" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("7D1FE739-AE53-47BE-847B-DD829CDD308C")).Select(x => x.MeasurementTotal).FirstOrDefault(); // Thermal Underpants                                  ES
                        worksheet.Cells["ET" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("7D1FE739-AE53-47BE-847B-DD829CDD308C")).Select(x => x.DistributionDate).FirstOrDefault(); // Distribution Date                                   ET
                        worksheet.Cells["EU" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("7D1FE739-AE53-47BE-847B-DD829CDD308C")).Select(x => x.StockValue).FirstOrDefault();       // Stock Value                                         EU
                        worksheet.Cells["EV" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("7D1FE739-AE53-47BE-847B-DD829CDD308C")).Select(x => x.ResponseValue).FirstOrDefault();    // Response Value                                      EV

                        //BD08ED02-182B-479E-A1C6-C1E47BBE0179                                     // 
                        worksheet.Cells["EW" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("BD08ED02-182B-479E-A1C6-C1E47BBE0179")).Select(x => x.MeasurementTotal).FirstOrDefault(); // Heating Fuel quantity                               EW
                        worksheet.Cells["EX" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("BD08ED02-182B-479E-A1C6-C1E47BBE0179")).Select(x => x.DistributionDate).FirstOrDefault(); // Distribution Date                                   EX
                        worksheet.Cells["EY" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("BD08ED02-182B-479E-A1C6-C1E47BBE0179")).Select(x => x.StockValue).FirstOrDefault();       // Stock Value                                         EY
                        worksheet.Cells["EZ" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("BD08ED02-182B-479E-A1C6-C1E47BBE0179")).Select(x => x.ResponseValue).FirstOrDefault();    // Response Value                                      EZ

                        //  C34EF5EA-F230-41EA-A844-A7ED98BC3179                                                                                                                                                                                                                             // 
                        worksheet.Cells["FA" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("C34EF5EA-F230-41EA-A844-A7ED98BC3179")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Heating Fuel unit                                   FA
                        worksheet.Cells["FB" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("C34EF5EA-F230-41EA-A844-A7ED98BC3179")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   FB
                        worksheet.Cells["FC" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("C34EF5EA-F230-41EA-A844-A7ED98BC3179")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         FC
                        worksheet.Cells["FD" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("C34EF5EA-F230-41EA-A844-A7ED98BC3179")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      FD

                        //  33530812-4FAF-4C73-AC3F-465640DF4D56                                  // 
                        worksheet.Cells["FE" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("33530812-4FAF-4C73-AC3F-465640DF4D56")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Heating Fuel type                                   FE
                        worksheet.Cells["FF" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("33530812-4FAF-4C73-AC3F-465640DF4D56")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   FF
                        worksheet.Cells["FG" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("33530812-4FAF-4C73-AC3F-465640DF4D56")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         FG
                        worksheet.Cells["FH" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("33530812-4FAF-4C73-AC3F-465640DF4D56")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      FH

                        // 75645293-2CBD-4F6B-A51E-2B6431A63CAD                                   // 
                        worksheet.Cells["FI" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("75645293-2CBD-4F6B-A51E-2B6431A63CAD")).Select(x => x.MeasurementTotal).FirstOrDefault();  // NFI kits                                            FI
                        worksheet.Cells["FJ" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("75645293-2CBD-4F6B-A51E-2B6431A63CAD")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   FJ
                        worksheet.Cells["FK" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("75645293-2CBD-4F6B-A51E-2B6431A63CAD")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         FK
                        worksheet.Cells["FL" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("75645293-2CBD-4F6B-A51E-2B6431A63CAD")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      FL

                        //9CCD8163-C665-43CA-8480-B4520268F684                                    // 
                        worksheet.Cells["FM" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("9CCD8163-C665-43CA-8480-B4520268F684")).Select(x => x.MeasurementTotal).FirstOrDefault();  // New arrival kits                                    FM
                        worksheet.Cells["FN" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("9CCD8163-C665-43CA-8480-B4520268F684")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   FN
                        worksheet.Cells["FO" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("9CCD8163-C665-43CA-8480-B4520268F684")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         FO
                        worksheet.Cells["FP" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("9CCD8163-C665-43CA-8480-B4520268F684")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      FP

                        // D18C3DD5-62EF-446E-AE17-3F896D79F519                                     // 
                        worksheet.Cells["FQ" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("D18C3DD5-62EF-446E-AE17-3F896D79F519")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Summer kits                                         FQ
                        worksheet.Cells["FR" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("D18C3DD5-62EF-446E-AE17-3F896D79F519")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   FR
                        worksheet.Cells["FS" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("D18C3DD5-62EF-446E-AE17-3F896D79F519")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         FS
                        worksheet.Cells["FT" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("D18C3DD5-62EF-446E-AE17-3F896D79F519")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      FT

                        //9FEA0282-E9BF-442E-B6FE-E55EC0302C30                                       // 
                        worksheet.Cells["FU" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("9FEA0282-E9BF-442E-B6FE-E55EC0302C30")).Select(x => x.MeasurementTotal).FirstOrDefault(); // Newborn kits                                        FU
                        worksheet.Cells["FV" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("9FEA0282-E9BF-442E-B6FE-E55EC0302C30")).Select(x => x.DistributionDate).FirstOrDefault(); // Distribution Date                                   FV
                        worksheet.Cells["FW" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("9FEA0282-E9BF-442E-B6FE-E55EC0302C30")).Select(x => x.StockValue).FirstOrDefault();       // Stock Value                                         FW
                        worksheet.Cells["FX" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("9FEA0282-E9BF-442E-B6FE-E55EC0302C30")).Select(x => x.ResponseValue).FirstOrDefault();    // Response Value                                      FX

                        //1C982871-58EB-427B-9C88-A8768A4C8809                                      // 
                        worksheet.Cells["FY" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("1C982871-58EB-427B-9C88-A8768A4C8809")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Elderly kits                                        FY
                        worksheet.Cells["FZ" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("1C982871-58EB-427B-9C88-A8768A4C8809")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   FZ
                        worksheet.Cells["GA" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("1C982871-58EB-427B-9C88-A8768A4C8809")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         GA
                        worksheet.Cells["GB" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("1C982871-58EB-427B-9C88-A8768A4C8809")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      GB

                        // D4BB9086-5932-46D6-9747-69704BD307EF                                     // 
                        worksheet.Cells["GC" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("D4BB9086-5932-46D6-9747-69704BD307EF")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Mattresses                                          GC
                        worksheet.Cells["GD" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("D4BB9086-5932-46D6-9747-69704BD307EF")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   GD
                        worksheet.Cells["GE" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("D4BB9086-5932-46D6-9747-69704BD307EF")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         GE
                        worksheet.Cells["GF" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("D4BB9086-5932-46D6-9747-69704BD307EF")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      GF

                        // 7E90816F-8346-43B9-9B20-33BFAA422C3B                                  // 
                        worksheet.Cells["GG" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("7E90816F-8346-43B9-9B20-33BFAA422C3B")).Select(x => x.MeasurementTotal).FirstOrDefault();  // IKEA Mattresses                                     GG
                        worksheet.Cells["GH" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("7E90816F-8346-43B9-9B20-33BFAA422C3B")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   GH
                        worksheet.Cells["GI" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("7E90816F-8346-43B9-9B20-33BFAA422C3B")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         GI
                        worksheet.Cells["GJ" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("7E90816F-8346-43B9-9B20-33BFAA422C3B")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      GJ

                        // 2176EBB4-1BC2-49DA-9EC4-8DE44327DB95                                  // 
                        worksheet.Cells["GK" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("2176EBB4-1BC2-49DA-9EC4-8DE44327DB95")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Blankets                                            GK
                        worksheet.Cells["GL" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("2176EBB4-1BC2-49DA-9EC4-8DE44327DB95")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   GL
                        worksheet.Cells["GM" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("2176EBB4-1BC2-49DA-9EC4-8DE44327DB95")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         GM
                        worksheet.Cells["GN" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("2176EBB4-1BC2-49DA-9EC4-8DE44327DB95")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      GN

                        //  FF16AE1E-5EFC-469A-9303-D52E40E37474                                  // 
                        worksheet.Cells["GO" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("FF16AE1E-5EFC-469A-9303-D52E40E37474")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Jerry cans                                          GO
                        worksheet.Cells["GP" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("FF16AE1E-5EFC-469A-9303-D52E40E37474")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   GP
                        worksheet.Cells["GQ" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("FF16AE1E-5EFC-469A-9303-D52E40E37474")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         GQ
                        worksheet.Cells["GR" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("FF16AE1E-5EFC-469A-9303-D52E40E37474")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      GR

                        //  AA33DE22-9F3C-471D-B191-D04ED6268E76                                 // 
                        worksheet.Cells["GS" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("AA33DE22-9F3C-471D-B191-D04ED6268E76")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Kitchen sets                                        GS
                        worksheet.Cells["GT" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("AA33DE22-9F3C-471D-B191-D04ED6268E76")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   GT
                        worksheet.Cells["GU" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("AA33DE22-9F3C-471D-B191-D04ED6268E76")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         GU
                        worksheet.Cells["GV" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("AA33DE22-9F3C-471D-B191-D04ED6268E76")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      GV

                        //  358FAA67-F127-45E2-B696-FEB5CB86DC29                                 // 
                        worksheet.Cells["GW" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("358FAA67-F127-45E2-B696-FEB5CB86DC29")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Plastic sheet                                       GW
                        worksheet.Cells["GX" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("358FAA67-F127-45E2-B696-FEB5CB86DC29")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   GX
                        worksheet.Cells["GY" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("358FAA67-F127-45E2-B696-FEB5CB86DC29")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         GY
                        worksheet.Cells["GZ" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("358FAA67-F127-45E2-B696-FEB5CB86DC29")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      GZ

                        // 48DD36DF-FCBD-4D35-AFFC-D90CAEB696E0                                   // 
                        worksheet.Cells["HA" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("48DD36DF-FCBD-4D35-AFFC-D90CAEB696E0")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Solar lamps / lantern                               HA
                        worksheet.Cells["HB" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("48DD36DF-FCBD-4D35-AFFC-D90CAEB696E0")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   HB
                        worksheet.Cells["HC" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("48DD36DF-FCBD-4D35-AFFC-D90CAEB696E0")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         HC
                        worksheet.Cells["HD" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("48DD36DF-FCBD-4D35-AFFC-D90CAEB696E0")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      HD

                        //  9656A14A-4A2A-4406-B0A0-2A758B5032C5                                   // 
                        worksheet.Cells["HE" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("9656A14A-4A2A-4406-B0A0-2A758B5032C5")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Sleeping mats                                       HE
                        worksheet.Cells["HF" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("9656A14A-4A2A-4406-B0A0-2A758B5032C5")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   HF
                        worksheet.Cells["HG" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("9656A14A-4A2A-4406-B0A0-2A758B5032C5")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         HG
                        worksheet.Cells["HH" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("9656A14A-4A2A-4406-B0A0-2A758B5032C5")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      HH

                        //  D2502671-A1E2775-9E07-C9A7D76A84FE                                   // 
                        worksheet.Cells["HI" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("D2502671-A1E2-4775-9E07-C9A7D76A84FE")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Plastic mats                                        HI
                        worksheet.Cells["HJ" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("D2502671-A1E2-4775-9E07-C9A7D76A84FE")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   HJ
                        worksheet.Cells["HK" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("D2502671-A1E2-4775-9E07-C9A7D76A84FE")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         HK
                        worksheet.Cells["HL" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("D2502671-A1E2-4775-9E07-C9A7D76A84FE")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      HL

                        // ADDBC8B2-E86C-4639-A3C4-FA17186B5CA8                                      // 
                        worksheet.Cells["HM" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("ADDBC8B2-E86C-4639-A3C4-FA17186B5CA8")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Carpets                                             HM
                        worksheet.Cells["HN" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("ADDBC8B2-E86C-4639-A3C4-FA17186B5CA8")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   HN
                        worksheet.Cells["HO" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("ADDBC8B2-E86C-4639-A3C4-FA17186B5CA8")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         HO
                        worksheet.Cells["HP" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("ADDBC8B2-E86C-4639-A3C4-FA17186B5CA8")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      HP

                        //  AC7C05D4-41780AE-9181-DBD8E1647813                                    //                                                     
                        worksheet.Cells["HQ" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("AC7C05D4-4178-40AE-9181-DBD8E1647813")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Plastic tarpaulins                                  HQ
                        worksheet.Cells["HR" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("AC7C05D4-4178-40AE-9181-DBD8E1647813")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   HR
                        worksheet.Cells["HS" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("AC7C05D4-4178-40AE-9181-DBD8E1647813")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         HS
                        worksheet.Cells["HT" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("AC7C05D4-4178-40AE-9181-DBD8E1647813")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      HT

                        //   FAA94CE0-27D4DC1-B3D1-70E0EB37214E                                  // 
                        worksheet.Cells["HU" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("FAA94CE0-27D7-4DC1-B3D1-70E0EB37214E")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Plastic rolls                                       HU
                        worksheet.Cells["HV" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("FAA94CE0-27D7-4DC1-B3D1-70E0EB37214E")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   HV
                        worksheet.Cells["HW" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("FAA94CE0-27D7-4DC1-B3D1-70E0EB37214E")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         HW
                        worksheet.Cells["HX" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("FAA94CE0-27D7-4DC1-B3D1-70E0EB37214E")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      HX

                        //  D031FC96-C2FEAC1-9F7F-0231C40E8EF4                                   // 
                        worksheet.Cells["HY" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("D031FC96-C2FE-4AC1-9F7F-0231C40E8EF4")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Flash light                                         HY
                        worksheet.Cells["HZ" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("D031FC96-C2FE-4AC1-9F7F-0231C40E8EF4")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   HZ
                        worksheet.Cells["IA" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("D031FC96-C2FE-4AC1-9F7F-0231C40E8EF4")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         IA
                        worksheet.Cells["IB" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("D031FC96-C2FE-4AC1-9F7F-0231C40E8EF4")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      IB

                        //  BFE0F9DF-07FE-4FE4-B8AD-0FECC3669AD3                                    // 
                        worksheet.Cells["IC" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("BFE0F9DF-07FE-4FE4-B8AD-0FECC3669AD3")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Heater                                              IC
                        worksheet.Cells["ID" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("BFE0F9DF-07FE-4FE4-B8AD-0FECC3669AD3")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   ID
                        worksheet.Cells["IE" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("BFE0F9DF-07FE-4FE4-B8AD-0FECC3669AD3")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         IE
                        worksheet.Cells["IF" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("BFE0F9DF-07FE-4FE4-B8AD-0FECC3669AD3")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      IF

                        // 4C6D1B55-AE7D-9C-BA24-E6806F0E5F2F                                      // 
                        worksheet.Cells["IG" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("4C6D1B55-AE7D-419C-BA24-E6806F0E5F2F")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Cooking Heater stove                                IG
                        worksheet.Cells["IH" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("4C6D1B55-AE7D-419C-BA24-E6806F0E5F2F")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   IH
                        worksheet.Cells["II" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("4C6D1B55-AE7D-419C-BA24-E6806F0E5F2F")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         II
                        worksheet.Cells["IJ" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("4C6D1B55-AE7D-419C-BA24-E6806F0E5F2F")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      IJ

                        //2D6914AF-F000-4C-8D15-CD7BC6E70075                                      // 
                        worksheet.Cells["IK" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("2D6914AF-F000-49BC-8D15-CD7BC6E70075")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Cooking Fuel quantity                               IK
                        worksheet.Cells["IL" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("2D6914AF-F000-49BC-8D15-CD7BC6E70075")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   IL
                        worksheet.Cells["IM" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("2D6914AF-F000-49BC-8D15-CD7BC6E70075")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         IM
                        worksheet.Cells["IN" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("2D6914AF-F000-49BC-8D15-CD7BC6E70075")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      IN

                        //  3D017795-F68B98C-808C-606283F83695                                     // 
                        worksheet.Cells["IO" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("3D017795-F68B-498C-808C-606283F83695")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Cooking Fuel unit                                   IO
                        worksheet.Cells["IP" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("3D017795-F68B-498C-808C-606283F83695")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   IP
                        worksheet.Cells["IQ" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("3D017795-F68B-498C-808C-606283F83695")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         IQ
                        worksheet.Cells["IR" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("3D017795-F68B-498C-808C-606283F83695")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      IR

                        //  6FE2C282-1FEDB5E-8613-F9BEC05A8668                                    // 
                        worksheet.Cells["IS" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("6FE2C282-1FED-4B5E-8613-F9BEC05A8668")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Cooking Fuel type                                   IS
                        worksheet.Cells["IT" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("6FE2C282-1FED-4B5E-8613-F9BEC05A8668")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   IT
                        worksheet.Cells["IU" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("6FE2C282-1FED-4B5E-8613-F9BEC05A8668")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         IU
                        worksheet.Cells["IV" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("6FE2C282-1FED-4B5E-8613-F9BEC05A8668")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      IV

                        //   5C6C3723-76A40D4-A185-285FE6CAB33D                                   // 
                        worksheet.Cells["IW" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("5C6C3723-76AC-40D4-A185-285FE6CAB33D")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Sanitary napkins                                    IW
                        worksheet.Cells["IX" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("5C6C3723-76AC-40D4-A185-285FE6CAB33D")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   IX
                        worksheet.Cells["IY" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("5C6C3723-76AC-40D4-A185-285FE6CAB33D")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         IY
                        worksheet.Cells["IZ" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("5C6C3723-76AC-40D4-A185-285FE6CAB33D")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      IZ

                        // AB1BAB6A-B805-00-A977-32FA389226FD                                     // 
                        worksheet.Cells["JA" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("AB1BAB6A-B805-4200-A977-32FA389226FD")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Hygiene kits                                        JA
                        worksheet.Cells["JB" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("AB1BAB6A-B805-4200-A977-32FA389226FD")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   JB
                        worksheet.Cells["JC" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("AB1BAB6A-B805-4200-A977-32FA389226FD")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         JC
                        worksheet.Cells["JD" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("AB1BAB6A-B805-4200-A977-32FA389226FD")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      JD

                        // C5182E7A-2071-4FBF-8550-067F397C63B1                                     // 
                        worksheet.Cells["JE" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("C5182E7A-2071-4FBF-8550-067F397C63B1")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Baby hygiene kits                                   JE
                        worksheet.Cells["JF" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("C5182E7A-2071-4FBF-8550-067F397C63B1")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   JF
                        worksheet.Cells["JG" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("C5182E7A-2071-4FBF-8550-067F397C63B1")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         JG
                        worksheet.Cells["JH" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("C5182E7A-2071-4FBF-8550-067F397C63B1")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      JH

                        // 9F43ED4D-E084-84-B3B5-63969C5BE1D7                                     // 
                        worksheet.Cells["JI" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("9F43ED4D-E084-4784-B3B5-63969C5BE1D7")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Dignity kits                                        JI
                        worksheet.Cells["JJ" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("9F43ED4D-E084-4784-B3B5-63969C5BE1D7")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   JJ
                        worksheet.Cells["JK" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("9F43ED4D-E084-4784-B3B5-63969C5BE1D7")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         JK
                        worksheet.Cells["JL" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("9F43ED4D-E084-4784-B3B5-63969C5BE1D7")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      JL

                        // FD3AA48D-97AF-AB-80A7-10A8BE1D61E9                                     // 
                        worksheet.Cells["JM" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("FD3AA48D-97AF-43AB-80A7-10A8BE1D61E9")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Diapers child                                       JM
                        worksheet.Cells["JN" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("FD3AA48D-97AF-43AB-80A7-10A8BE1D61E9")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   JN
                        worksheet.Cells["JO" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("FD3AA48D-97AF-43AB-80A7-10A8BE1D61E9")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         JO
                        worksheet.Cells["JP" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("FD3AA48D-97AF-43AB-80A7-10A8BE1D61E9")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      JP

                        // 6CC26856-DECD-14-92C5-A71480C0876D                                      // 
                        worksheet.Cells["JQ" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("6CC26856-DECD-4F14-92C5-A71480C0876D")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Diapers E D                                         JQ
                        worksheet.Cells["JR" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("6CC26856-DECD-4F14-92C5-A71480C0876D")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   JR
                        worksheet.Cells["JS" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("6CC26856-DECD-4F14-92C5-A71480C0876D")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         JS
                        worksheet.Cells["JT" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("6CC26856-DECD-4F14-92C5-A71480C0876D")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      JT

                        // 3CE45EBA-95B3-E5-B9AD-6B67C56FF60D                                    // 
                        worksheet.Cells["JU" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("3CE45EBA-95B3-4DE5-B9AD-6B67C56FF60D")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Clothes / Footwear - Adults(18 and over)            JU
                        worksheet.Cells["JV" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("3CE45EBA-95B3-4DE5-B9AD-6B67C56FF60D")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   JV
                        worksheet.Cells["JW" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("3CE45EBA-95B3-4DE5-B9AD-6B67C56FF60D")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         JW
                        worksheet.Cells["JX" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("3CE45EBA-95B3-4DE5-B9AD-6B67C56FF60D")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      JX

                        // 85A007A2-58BC-4C-AC46-932E2DDFFCF0                                   // 
                        worksheet.Cells["JY" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("85A007A2-58BC-4F4C-AC46-932E2DDFFCF0")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Clothes / Footwear - Children(Under 18)             JY
                        worksheet.Cells["JZ" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("85A007A2-58BC-4F4C-AC46-932E2DDFFCF0")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   JZ
                        worksheet.Cells["KA" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("85A007A2-58BC-4F4C-AC46-932E2DDFFCF0")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         KA
                        worksheet.Cells["KB" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("85A007A2-58BC-4F4C-AC46-932E2DDFFCF0")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      KB

                        //6B9A33DA-8AFC-49AC-ADAE-B8883B409EB0                                    // 
                        worksheet.Cells["KC" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("6B9A33DA-8AFC-49AC-ADAE-B8883B409EB0")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Adult Female Sweater and Underwear                  KC
                        worksheet.Cells["KD" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("6B9A33DA-8AFC-49AC-ADAE-B8883B409EB0")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   KD
                        worksheet.Cells["KE" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("6B9A33DA-8AFC-49AC-ADAE-B8883B409EB0")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         KE
                        worksheet.Cells["KF" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("6B9A33DA-8AFC-49AC-ADAE-B8883B409EB0")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      KF

                        //EBAA4BAF-E3F3-42-AF83-1F0AA177ABEC                                   // 
                        worksheet.Cells["KG" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("EBAA4BAF-E3F3-4762-AF83-1F0AA177ABEC")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Adult Male Sweater                                  KG
                        worksheet.Cells["KH" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("EBAA4BAF-E3F3-4762-AF83-1F0AA177ABEC")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   KH
                        worksheet.Cells["KI" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("EBAA4BAF-E3F3-4762-AF83-1F0AA177ABEC")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         KI
                        worksheet.Cells["KJ" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("EBAA4BAF-E3F3-4762-AF83-1F0AA177ABEC")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      KJ

                        //38F4D5FF-93A4-4D-935F-68D4D45A0F49                                     // 
                        worksheet.Cells["KK" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("38F4D5FF-93A4-401D-935F-68D4D45A0F49")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Children underwear kit                              KK
                        worksheet.Cells["KL" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("38F4D5FF-93A4-401D-935F-68D4D45A0F49")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   KL
                        worksheet.Cells["KM" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("38F4D5FF-93A4-401D-935F-68D4D45A0F49")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         KM
                        worksheet.Cells["KN" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("38F4D5FF-93A4-401D-935F-68D4D45A0F49")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      KN

                        //  33DB47E3-BAF89EA-A71D-4C88CF681313                                  // 
                        worksheet.Cells["KO" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("33DB47E3-BAF8-49EA-A71D-4C88CF681313")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Underwear kit                                       KO
                        worksheet.Cells["KP" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("33DB47E3-BAF8-49EA-A71D-4C88CF681313")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   KP
                        worksheet.Cells["KQ" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("33DB47E3-BAF8-49EA-A71D-4C88CF681313")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         KQ
                        worksheet.Cells["KR" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("33DB47E3-BAF8-49EA-A71D-4C88CF681313")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      KR

                        // 01CECDF4-5C64-4A-B5CA-7193C6FDAFB2                                      // 
                        worksheet.Cells["KS" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("01CECDF4-5C64-4E4A-B5CA-7193C6FDAFB2")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Cotton sheets                                       KS
                        worksheet.Cells["KT" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("01CECDF4-5C64-4E4A-B5CA-7193C6FDAFB2")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   KT
                        worksheet.Cells["KU" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("01CECDF4-5C64-4E4A-B5CA-7193C6FDAFB2")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         KU
                        worksheet.Cells["KV" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("01CECDF4-5C64-4E4A-B5CA-7193C6FDAFB2")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      KV

                        // E3172E6F-CE36-09-B1BF-7EB1F9A70116                                   // 
                        worksheet.Cells["KW" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("E3172E6F-CE36-4409-B1BF-7EB1F9A70116")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Fan                                                 KW
                        worksheet.Cells["KX" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("E3172E6F-CE36-4409-B1BF-7EB1F9A70116")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   KX
                        worksheet.Cells["KY" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("E3172E6F-CE36-4409-B1BF-7EB1F9A70116")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         KY
                        worksheet.Cells["KZ" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("E3172E6F-CE36-4409-B1BF-7EB1F9A70116")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      KZ

                        // 50B99DB1-866C-45-9EA6-80B59DD2FAAB                                    // 
                        worksheet.Cells["LA" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("50B99DB1-866C-4D45-9EA6-80B59DD2FAAB")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Plastic Bucket                                      LA
                        worksheet.Cells["LB" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("50B99DB1-866C-4D45-9EA6-80B59DD2FAAB")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   LB
                        worksheet.Cells["LC" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("50B99DB1-866C-4D45-9EA6-80B59DD2FAAB")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         LC
                        worksheet.Cells["LD" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("50B99DB1-866C-4D45-9EA6-80B59DD2FAAB")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      LD

                        //  1326AFF5-CF59-4EBA-BF8B-69862CCE2B4F                                  // 
                        worksheet.Cells["LE" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("1326AFF5-CF59-4EBA-BF8B-69862CCE2B4F")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Face Mask                                           LE
                        worksheet.Cells["LF" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("1326AFF5-CF59-4EBA-BF8B-69862CCE2B4F")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   LF
                        worksheet.Cells["LG" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("1326AFF5-CF59-4EBA-BF8B-69862CCE2B4F")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         LG
                        worksheet.Cells["LH" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("1326AFF5-CF59-4EBA-BF8B-69862CCE2B4F")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      LH

                        //1E665B95-249D-42-957D-0B7B6CF525F6                                   // 
                        worksheet.Cells["LI" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("1E665B95-249D-4B42-957D-0B7B6CF525F6")).Select(x => x.MeasurementTotal).FirstOrDefault();  // CRI BAG                                             LI
                        worksheet.Cells["LJ" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("1E665B95-249D-4B42-957D-0B7B6CF525F6")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   LJ
                        worksheet.Cells["LK" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("1E665B95-249D-4B42-957D-0B7B6CF525F6")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         LK
                        worksheet.Cells["LL" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("1E665B95-249D-4B42-957D-0B7B6CF525F6")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      LL

                        // 2789370F-FFD2-4406-9389-70C8E46FC738                                   // 
                        worksheet.Cells["LM" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("2789370F-FFD2-4406-9389-70C8E46FC738")).Select(x => x.MeasurementTotal).FirstOrDefault(); // Transparent plastic sheet                           LM
                        worksheet.Cells["LN" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("2789370F-FFD2-4406-9389-70C8E46FC738")).Select(x => x.DistributionDate).FirstOrDefault(); // Distribution Date                                   LN
                        worksheet.Cells["LO" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("2789370F-FFD2-4406-9389-70C8E46FC738")).Select(x => x.StockValue).FirstOrDefault();       // Stock Value                                         LO
                        worksheet.Cells["LP" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("2789370F-FFD2-4406-9389-70C8E46FC738")).Select(x => x.ResponseValue).FirstOrDefault();    // Response Value                                      LP

                        // 70979B14-71E6-43EF-9F6F-0BF0B525B0A8                                    // 
                        worksheet.Cells["LQ" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("70979B14-71E6-43EF-9F6F-0BF0B525B0A8")).Select(x => x.MeasurementTotal).FirstOrDefault();  // COVID-19 hygiene kits                               LQ
                        worksheet.Cells["LR" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("70979B14-71E6-43EF-9F6F-0BF0B525B0A8")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   LR
                        worksheet.Cells["LS" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("70979B14-71E6-43EF-9F6F-0BF0B525B0A8")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         LS
                        worksheet.Cells["LT" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("70979B14-71E6-43EF-9F6F-0BF0B525B0A8")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      LT

                        //319FFC37-DDC4-48-BE5B-3D6A8045D6E2                                    // 
                        worksheet.Cells["LU" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("319FFC37-DDC4-43A8-BE5B-3D6A8045D6E2")).Select(x => x.MeasurementTotal).FirstOrDefault();  // Portable Gas                                        LU
                        worksheet.Cells["LV" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("319FFC37-DDC4-43A8-BE5B-3D6A8045D6E2")).Select(x => x.DistributionDate).FirstOrDefault();  // Distribution Date                                   LV
                        worksheet.Cells["LW" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("319FFC37-DDC4-43A8-BE5B-3D6A8045D6E2")).Select(x => x.StockValue).FirstOrDefault();        // Stock Value                                         LW
                        worksheet.Cells["LX" + i].Value = criData.Where(x => x.UnitOfAchievementGUID == Guid.Parse("319FFC37-DDC4-43A8-BE5B-3D6A8045D6E2")).Select(x => x.ResponseValue).FirstOrDefault();     // Response Value                                      LX

                    }
                    #endregion


                    i++;
                }
                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                worksheet.Column(6).AutoFit();
                worksheet.Column(7).AutoFit();
                worksheet.Column(8).AutoFit();
                worksheet.Column(9).AutoFit();
                worksheet.Column(10).AutoFit();
                worksheet.Column(11).AutoFit();
                worksheet.Column(12).AutoFit();
                worksheet.Column(13).AutoFit();
                worksheet.Column(14).AutoFit();
                worksheet.Column(15).AutoFit();
                worksheet.Column(16).AutoFit();
                worksheet.Column(17).AutoFit();
                worksheet.Column(18).AutoFit();
                worksheet.Column(19).AutoFit();
                worksheet.Column(20).AutoFit();


                ExcelRange range = worksheet.Cells[1, 1, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column];
                ExcelTable tab = worksheet.Tables.Add(range, "Table1");
                tab.ShowHeader = true;
                tab.TableStyle = TableStyles.Medium2;
                tab.ShowFilter = true;

                package.Save();
            }


            byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);
            string fileName = "PMRD " + DateTime.Now.ToString("dddd, dd MMMM yyyy") + ".xlsx";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        #endregion

        #region Validation and Approvals 
        [HttpPost]
        [Route("PMD/PartnerMonitoringDatabase/PartnerMonitoringDatabaseVerifyField")]
        public ActionResult PartnerMonitoringDatabaseVerifyField(Guid PartnerMonitoringDBGUID)
        {

            dataPartnerMonitoringDB dataPartnerMonitoringDB = (from a in DbPMD.dataPartnerMonitoringDBs
                                                               where a.PartnerMonitoringDBGUID == PartnerMonitoringDBGUID
                                                               select a).FirstOrDefault();

            if (CMS.HasAction(Permissions.PartnerMonitoringDatabaseFieldTechVerify.Create, Apps.PMD))
            {
                dataPartnerMonitoringDB.IsVerifiedByFieldTech = true;
                dataPartnerMonitoringDB.VerifiedByFieldTechGUID = UserGUID;
                dataPartnerMonitoringDB.VerifiedByFieldTechOn = DateTime.Now;

                DbPMD.SaveChanges();

                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });

            }

        }

        [HttpPost]
        [Route("PMD/PartnerMonitoringDatabase/PartnerMonitoringDatabaseVerifyFieldNot")]
        public ActionResult PartnerMonitoringDatabaseVerifyFieldNot(Guid PartnerMonitoringDBGUID, string Reason)
        {

            dataPartnerMonitoringDB dataPartnerMonitoringDB = (from a in DbPMD.dataPartnerMonitoringDBs
                                                               where a.PartnerMonitoringDBGUID == PartnerMonitoringDBGUID
                                                               select a).FirstOrDefault();

            if (CMS.HasAction(Permissions.PartnerMonitoringDatabaseFieldTechVerify.Create, Apps.PMD))
            {
                Guid createdByGUID = dataPartnerMonitoringDB.CreatedBy;
                var createdByUser = (from a in DbCMS.userServiceHistory
                                     join b in DbCMS.userPersonalDetailsLanguage on a.UserGUID equals b.UserGUID
                                     where a.UserGUID == createdByGUID
                                     select new
                                     {
                                         EmailAddress = a.EmailAddress,
                                         FullName = b.FirstName + " " + b.Surname
                                     }).FirstOrDefault();

                dataPartnerMonitoringDB.IsVerifiedByFieldTech = false;
                dataPartnerMonitoringDB.VerifiedByFieldTechGUID = UserGUID;
                dataPartnerMonitoringDB.VerifiedByFieldTechOn = DateTime.Now;
                dataPartnerMonitoringDB.NotVerifiedByFieldReason = Reason;
                DbPMD.SaveChanges();


                new Email().SendPMDRejectEmail(createdByUser.EmailAddress, createdByUser.FullName, "Indicator Not Verified", Reason, null, "Indicator Not Verified", PartnerMonitoringDBGUID);



                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });

            }

        }

        [HttpPost]
        [Route("PMD/PartnerMonitoringDatabase/PartnerMonitoringDatabaseVerifyProgramme")]
        public ActionResult PartnerMonitoringDatabaseVerifyProgramme(Guid PartnerMonitoringDBGUID)
        {

            dataPartnerMonitoringDB dataPartnerMonitoringDB = (from a in DbPMD.dataPartnerMonitoringDBs
                                                               where a.PartnerMonitoringDBGUID == PartnerMonitoringDBGUID
                                                               select a).FirstOrDefault();

            if (dataPartnerMonitoringDB.IsVerifiedByFieldTech == false)
            {
                return Json(new { success = false });
            }
            if (CMS.HasAction(Permissions.PartnerMonitoringDatabaseProgrammeApprove.Create, Apps.PMD))
            {
                dataPartnerMonitoringDB.IsApprovedByProgramme = true;
                dataPartnerMonitoringDB.ApprovedByProgrammeGUID = UserGUID;
                dataPartnerMonitoringDB.ApprovedByProgrammeOn = DateTime.Now;

                DbPMD.SaveChanges();

                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });

            }

        }

        [HttpPost]
        [Route("PMD/PartnerMonitoringDatabase/PartnerMonitoringDatabaseVerifyCountry")]
        public ActionResult PartnerMonitoringDatabaseVerifyCountry(Guid PartnerMonitoringDBGUID)
        {

            dataPartnerMonitoringDB dataPartnerMonitoringDB = (from a in DbPMD.dataPartnerMonitoringDBs
                                                               where a.PartnerMonitoringDBGUID == PartnerMonitoringDBGUID
                                                               select a).FirstOrDefault();

            if (dataPartnerMonitoringDB.IsVerifiedByFieldTech == false)
            {
                return Json(new { success = false });
            }
            //if (dataPartnerMonitoringDB.IsApprovedByProgramme == false)
            //{
            //    return Json(new { success = false });
            //}

            if (CMS.HasAction(Permissions.PartnerMonitoringDatabaseCountryTechVerify.Create, Apps.PMD))
            {
                dataPartnerMonitoringDB.IsApprovedByCountryTech = true;
                dataPartnerMonitoringDB.ApprovedByCountryTechGUID = UserGUID;
                dataPartnerMonitoringDB.ApprovedByCountryTechOn = DateTime.Now;

                DbPMD.SaveChanges();

                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });

            }

        }

        [HttpPost]
        [Route("PMD/PartnerMonitoringDatabase/PartnerMonitoringDatabaseVerifyCountryNot")]
        public ActionResult PartnerMonitoringDatabaseVerifyCountryNot(Guid PartnerMonitoringDBGUID, string Reason)
        {

            dataPartnerMonitoringDB dataPartnerMonitoringDB = (from a in DbPMD.dataPartnerMonitoringDBs
                                                               where a.PartnerMonitoringDBGUID == PartnerMonitoringDBGUID
                                                               select a).FirstOrDefault();

            if (dataPartnerMonitoringDB.IsVerifiedByFieldTech == false)
            {
                return Json(new { success = false });
            }
            //if (dataPartnerMonitoringDB.IsApprovedByProgramme == false)
            //{
            //    return Json(new { success = false });
            //}

            if (CMS.HasAction(Permissions.PartnerMonitoringDatabaseCountryTechVerify.Create, Apps.PMD))
            {
                Guid createdByGUID = dataPartnerMonitoringDB.CreatedBy;
                var createdByUser = (from a in DbCMS.userServiceHistory
                                     join b in DbCMS.userPersonalDetailsLanguage on a.UserGUID equals b.UserGUID
                                     where a.UserGUID == createdByGUID
                                     select new
                                     {
                                         EmailAddress = a.EmailAddress,
                                         FullName = b.FirstName + " " + b.Surname
                                     }).FirstOrDefault();

                dataPartnerMonitoringDB.IsApprovedByCountryTech = false;
                dataPartnerMonitoringDB.ApprovedByCountryTechGUID = UserGUID;
                dataPartnerMonitoringDB.ApprovedByCountryTechOn = DateTime.Now;
                dataPartnerMonitoringDB.NotApprovedByCountryReason = Reason;
                DbPMD.SaveChanges();

                new Email().SendPMDRejectEmail(createdByUser.EmailAddress, createdByUser.FullName, "Indicator Not Approved", Reason, null, "Indicator Not Approved", PartnerMonitoringDBGUID);


                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });

            }

        }
        #endregion

        #region Helpers
        public JsonResult GetIndicatorGuidance(Guid IndicatorGUID)
        {
            var IndicatorGuidance = (from a in DbPMD.codePmdIndicatorLanguages
                                     where a.IndicatorGUID == IndicatorGUID
                                     && a.LanguageID == LAN
                                     select a.IndicatorGuidance).FirstOrDefault();
            return Json(new { IndicatorGuidance = IndicatorGuidance }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetIndicatorUnitGuidance(Guid UnitOfAchievementGUID)
        {
            var UnitGuidance = (from a in DbPMD.codePmdUnitOfAchievementLanguages
                                where a.UnitOfAchievementGUID == UnitOfAchievementGUID && a.LanguageID == LAN
                                select a.UnitOfAchievementGuidance).FirstOrDefault();
            return Json(new { UnitGuidance = UnitGuidance }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCoordinates(string admin4Pcode)
        {
            var result = (from a in DbPMD.codeOchaLocations
                          where a.admin4Pcode == admin4Pcode
                          select new
                          {
                              a.Longitude_x,
                              a.Latitude_y
                          }).FirstOrDefault();

            return Json(new { Longitude = result.Longitude_x, Latitude = result.Latitude_y }, JsonRequestBehavior.AllowGet);
        }
        #endregion




    }
}