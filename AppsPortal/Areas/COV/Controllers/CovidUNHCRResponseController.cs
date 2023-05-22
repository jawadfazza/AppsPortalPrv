using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AutoMapper;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using COV_DAL.Model;
using System.Web;
using AppsPortal.ViewModels;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;

namespace AppsPortal.Areas.COV.Controllers
{
    public class CovidUNHCRResponseController : COVBaseController
    {
        //[Route("COV/CovidUNHCRResponse/InsertLocations/")]
        //public ActionResult InsertLocations()
        //{

        //    FileInfo locationInfo = new FileInfo(Server.MapPath("~/Areas/COV/Templates/locations.xlsx"));
        //    List<codeOchaLocation> codeOchaLocations = new List<codeOchaLocation>();

        //    using (var package = new ExcelPackage(locationInfo))
        //    {
        //        ExcelWorksheet workSheet = package.Workbook.Worksheets["Sheet1"];
        //        int totalRows = workSheet.Dimension.End.Row;
        //        for (int i = 2; i <= totalRows; i++)
        //        {
        //            codeOchaLocation codeOchaLocation = new codeOchaLocation();
        //            codeOchaLocation.ID = Convert.ToInt32(workSheet.Cells["A" + i].Value.ToString());
        //            codeOchaLocation.admin0Name_en = workSheet.Cells["B" + i].Value == null ? "" : workSheet.Cells["B" + i].Value.ToString();
        //            codeOchaLocation.admin0Name_ar = workSheet.Cells["C" + i].Value == null ? "" : workSheet.Cells["C" + i].Value.ToString();
        //            codeOchaLocation.admin0Pcode = workSheet.Cells["D" + i].Value == null ? "" : workSheet.Cells["D" + i].Value.ToString();

        //            codeOchaLocation.admin1Name_en = workSheet.Cells["E" + i].Value == null ? "" : workSheet.Cells["E" + i].Value.ToString();
        //            codeOchaLocation.admin1Name_ar = workSheet.Cells["F" + i].Value == null ? "" : workSheet.Cells["F" + i].Value.ToString();
        //            codeOchaLocation.admin1Pcode = workSheet.Cells["G" + i].Value == null ? "" : workSheet.Cells["G" + i].Value.ToString();
        //            codeOchaLocation.GovernorateGUID = Guid.Parse(workSheet.Cells["H" + i].Value.ToString());

        //            codeOchaLocation.admin2Name_en = workSheet.Cells["I" + i].Value == null ? "" : workSheet.Cells["I" + i].Value.ToString();
        //            codeOchaLocation.admin2Name_ar = workSheet.Cells["J" + i].Value == null ? "" : workSheet.Cells["J" + i].Value.ToString();
        //            codeOchaLocation.admin2Pcode = workSheet.Cells["K" + i].Value == null ? "" : workSheet.Cells["K" + i].Value.ToString();


        //            codeOchaLocation.admin3Name_en = workSheet.Cells["L" + i].Value == null ? "" : workSheet.Cells["L" + i].Value.ToString();
        //            codeOchaLocation.admin3Name_ar = workSheet.Cells["M" + i].Value == null ? "" : workSheet.Cells["M" + i].Value.ToString();
        //            codeOchaLocation.admin3Pcode = workSheet.Cells["N" + i].Value == null ? "" : workSheet.Cells["N" + i].Value.ToString();


        //            codeOchaLocation.admin4Name_en = workSheet.Cells["O" + i].Value == null ? "" : workSheet.Cells["O" + i].Value.ToString();
        //            codeOchaLocation.admin4Name_ar = workSheet.Cells["P" + i].Value == null ? "" : workSheet.Cells["P" + i].Value.ToString();
        //            codeOchaLocation.admin4Pcode = workSheet.Cells["Q" + i].Value == null ? "" : workSheet.Cells["Q" + i].Value.ToString();

        //            codeOchaLocation.admin5Name_en = workSheet.Cells["R" + i].Value == null ? "" : workSheet.Cells["R" + i].Value.ToString();
        //            codeOchaLocation.admin5Name_ar = workSheet.Cells["S" + i].Value == null ? "" : workSheet.Cells["S" + i].Value.ToString();
        //            codeOchaLocation.admin5Pcode = workSheet.Cells["T" + i].Value == null ? "" : workSheet.Cells["T" + i].Value.ToString();

        //            codeOchaLocation.Latitude_y = Convert.ToDouble(workSheet.Cells["Z" + i].Value.ToString());
        //            codeOchaLocation.Longitude_x = Convert.ToDouble(workSheet.Cells["AA" + i].Value.ToString());


        //            codeOchaLocations.Add(codeOchaLocation);
        //        }
        //    }

        //    DbCOV.BulkInsert(codeOchaLocations);
        //    DbCOV.BulkSaveChanges();

        //    return null;
        //}
        public ActionResult Index()
        {
            if (!CMS.HasAction(Permissions.COVIDUNHCRResponse.Access, Apps.COV))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/COV/Views/CovidUNHCRResponse/Index.cshtml");
        }

        [Route("COV/CovidUNHCRResponse/CovidUNHCRResponseDataTable/")]
        public JsonResult CovidUNHCRResponseDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<CovidUNHCRResponseStrategyDataTableModel, bool>> Predicate = p => true;


            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<CovidUNHCRResponseStrategyDataTableModel>(DataTable.Filters);
            }

            if (!CMS.HasAction(Permissions.COVIDRecordVerify.Create, Apps.COV))
            {
                List<string> AuthorizedListByLocation = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.COVIDUNHCRResponse.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

                var All = (from a in DbCOV.v_dataCovidUNHCRResponseStrategyDataTable.AsExpandable()
                          .Where(x => AuthorizedListByLocation.Contains(x.GovernorateGUID.ToString()))
                           select new CovidUNHCRResponseStrategyDataTableModel
                           {
                               CovidUNHCRResponseStrategyGUID = a.CovidUNHCRResponseStrategyGUID,
                               DateOfReport = a.DateOfReport,
                               CovStatus = a.CovStatus,
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
                               ImplementingPartner = a.ImplementingPartner,
                               CovIndicatorTechnicalUnitGUID = a.CovIndicatorTechnicalUnitGUID.ToString(),
                               CovIndicatorTechnicalUnit = a.CovIndicatorTechnicalUnit,
                               IsVerified = a.IsVerified,
                               Active = a.Active,
                               dataCovidUNHCRResponseStrategyRowVersion = a.dataCovidUNHCRResponseStrategyRowVersion
                           }).Distinct().Where(Predicate);

                //var All = (from a in DbCOV.dataCovidUNHCRResponseStrategies.AsExpandable()
                //           .Where(x => AuthorizedListByLocation.Contains(x.GovernorateGUID.ToString()))
                //           join b in DbCOV.codeCovObjectives.AsExpandable() on a.ObjectiveGUID equals b.ObjectiveGUID
                //           join c in DbCOV.codeCovOutputs.AsExpandable() on a.OutputGUID equals c.OutputGUID
                //           join d in DbCOV.codeCovIndicators.AsExpandable() on a.IndicatorGUID equals d.IndicatorGUID
                //           join e in DbCOV.codeOchaLocations.AsExpandable() on a.Governorate equals e.admin1Pcode
                //           join f in DbCOV.codeOchaLocations.AsExpandable() on a.District equals f.admin2Pcode
                //           join j in DbCOV.codeOchaLocations.AsExpandable() on a.SubDistrict equals j.admin3Pcode
                //           join h in DbCOV.codeOchaLocations.AsExpandable() on a.CommunityName equals h.admin4Pcode
                //           select new CovidUNHCRResponseStrategyDataTableModel
                //           {
                //               CovidUNHCRResponseStrategyGUID = a.CovidUNHCRResponseStrategyGUID,
                //               DateOfReport = a.DateOfReport,
                //               ObjectiveGUID = a.ObjectiveGUID,
                //               ObjectiveDescription = a.ObjectiveDescription,
                //               OutputGUID = a.OutputGUID,
                //               OutputDescription = a.OutputDescription,
                //               IndicatorGUID = a.IndicatorGUID,
                //               IndicatorDescription = a.IndicatorDescription,
                //               Governorate = a.admin1Name_en,
                //               District = a.admin2Name_en,
                //               SubDistrict = a.admin3Name_en,
                //               CommunityName = a.admin4Name_en,
                //               ImplementingPartner = a.ImplementingPartner,
                //               Active = a.Active,
                //               dataCovidUNHCRResponseStrategyRowVersion = a.dataCovidUNHCRResponseStrategyRowVersion
                //           }).Distinct().Where(Predicate);

                All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

                List<CovidUNHCRResponseStrategyDataTableModel> Result = Mapper.Map<List<CovidUNHCRResponseStrategyDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

                return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<string> AuthorizedListByLocation = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.COVIDUNHCRResponse.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
                List<string> AuthorizedListByTechnicalUnit = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.COVIDRecordVerify.CreateGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

                var All = (from a in DbCOV.v_dataCovidUNHCRResponseStrategyDataTable.AsExpandable().Where(x => AuthorizedListByLocation.Contains(x.GovernorateGUID.ToString()))
                           join b in DbCOV.codeCovIndicatorTechnicalUnitMaps.AsExpandable().Where(x => AuthorizedListByTechnicalUnit.Contains(x.IndicatorTechnicalUnitGUID.ToString())) on a.IndicatorGUID equals b.IndicatorGUID
                           select new CovidUNHCRResponseStrategyDataTableModel
                           {
                               CovidUNHCRResponseStrategyGUID = a.CovidUNHCRResponseStrategyGUID,
                               DateOfReport = a.DateOfReport,
                               CovStatus = a.CovStatus,
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
                               ImplementingPartner = a.ImplementingPartner,
                               CovIndicatorTechnicalUnitGUID = a.CovIndicatorTechnicalUnitGUID.ToString(),
                               CovIndicatorTechnicalUnit = a.CovIndicatorTechnicalUnit,
                               IsVerified = a.IsVerified,
                               Active = a.Active,
                               dataCovidUNHCRResponseStrategyRowVersion = a.dataCovidUNHCRResponseStrategyRowVersion
                           }).Distinct().Where(Predicate);

                All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

                List<CovidUNHCRResponseStrategyDataTableModel> Result = Mapper.Map<List<CovidUNHCRResponseStrategyDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

                return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
            }
        }

        [Route("COV/CovidUNHCRResponse/Create/")]
        public ActionResult CovidUNHCRResponseCreate()
        {
            if (!CMS.HasAction(Permissions.COVIDUNHCRResponse.Create, Apps.COV))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            DataCovidUNHCRResponseStrategyUpdateModel model = new DataCovidUNHCRResponseStrategyUpdateModel();
            return View("~/Areas/COV/Views/CovidUNHCRResponse/CovidUNHCRResponse.cshtml", model);
        }

        [Route("COV/CovidUNHCRResponse/CreateNew/")]
        public ActionResult CovidUNHCRResponseCreateNew(
            int? UNHCRStrategy,
            DateTime DateOfReport,
            string Governorate,
            string District,
            string SubDistrict,
            string CommunityName,
            string Neighborhood,
            string Location,
            double Longitude,
            double Latitude,
            Guid ObjectiveGUID)
        {
            if (!CMS.HasAction(Permissions.COVIDUNHCRResponse.Create, Apps.COV))
            {
                throw new HttpException(401, "Unauthorized access");
            }


            DataCovidUNHCRResponseStrategyUpdateModel model = new DataCovidUNHCRResponseStrategyUpdateModel();
            model.UNHCRStrategy = UNHCRStrategy;
            model.DateOfReport = DateOfReport;
            model.Governorate = Governorate;
            model.District = District;
            model.SubDistrict = SubDistrict;
            model.CommunityName = CommunityName;
            model.Neighborhood = Neighborhood;
            model.Location = Location;
            model.Longitude = Longitude;
            model.Latitude = Latitude;
            model.ObjectiveGUID = ObjectiveGUID;

            return View("~/Areas/COV/Views/CovidUNHCRResponse/CovidUNHCRResponse.cshtml", model);
        }

        [Route("COV/CovidUNHCRResponse/Update/{PK}")]
        public ActionResult CovidUNHCRResponseUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.COVIDUNHCRResponse.Access, Apps.COV))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            DataCovidUNHCRResponseStrategyUpdateModel model = new DataCovidUNHCRResponseStrategyUpdateModel();
            var result = (from a in DbCOV.dataCovidUNHCRResponseStrategies.AsNoTracking().Where(x => x.CovidUNHCRResponseStrategyGUID == PK) select a).FirstOrDefault();

            model = Mapper.Map(result, new DataCovidUNHCRResponseStrategyUpdateModel());
            if (String.IsNullOrEmpty(model.Governorate))
            {
                model.GovernorateGUID = (from a in DbCOV.codeOchaLocations.Where(x => x.admin1Pcode == model.Governorate) select a.GovernorateGUID).FirstOrDefault();
            }
            return View("~/Areas/COV/Views/CovidUNHCRResponse/CovidUNHCRResponse.cshtml", model);
        }


        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult CovidUNHCRResponseCreate(DataCovidUNHCRResponseStrategyUpdateModel model)
        {
            Guid GovernorateGUID = (from a in DbCOV.codeOchaLocations.Where(x => x.admin1Pcode == model.Governorate) select a.GovernorateGUID).FirstOrDefault().Value;
            model.GovernorateGUID = GovernorateGUID;
            if (!CMS.HasAction(Permissions.COVIDUNHCRResponse.Create, Apps.COV, model.GovernorateGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            DateTime ExecutionTime = DateTime.Now;
            Guid EntityPK = Guid.NewGuid();
            if (model.CovidUNHCRResponseStrategyGUID != Guid.Empty)
            {
                EntityPK = model.CovidUNHCRResponseStrategyGUID;
            }

            dataCovidUNHCRResponseStrategy dataCovidUNHCRResponseStrategy = Mapper.Map(model, new dataCovidUNHCRResponseStrategy());
            dataCovidUNHCRResponseStrategy.CovidUNHCRResponseStrategyGUID = EntityPK;
            dataCovidUNHCRResponseStrategy.CreatedBy = UserGUID;
            dataCovidUNHCRResponseStrategy.CreatedOn = ExecutionTime;
            dataCovidUNHCRResponseStrategy.Active = true;
            dataCovidUNHCRResponseStrategy.IsVerified = false;

            var TechnicalUnitGUID = (from a in DbCOV.codeCovIndicators
                                     join b in DbCOV.codeTablesValuesLanguages on a.CovIndicatorTechnicalUnit.Trim().ToUpper() equals b.ValueDescription.Trim().ToUpper()
                                     where a.IndicatorGUID == model.IndicatorGUID
                                     select b.ValueGUID).FirstOrDefault();
            dataCovidUNHCRResponseStrategy.CovIndicatorTechnicalUnitGUID = TechnicalUnitGUID;

            // var CovUnitCost = (from a in DbCOV.codeCovUnits
            //                   where a.CovUnitGUID == model.CovUnitGUID
            //                   select a.CovUnitCost).FirstOrDefault();
            //dataCovidUNHCRResponseStrategy.UnitCost = CovUnitCost * model.Quantity;
            //dataCovidUNHCRResponseStrategy.CostEstimationThreeMonthes = CovUnitCost * model.Quantity * 3;

            if (!String.IsNullOrEmpty(model.Governorate))
            {
                dataCovidUNHCRResponseStrategy.GovernorateGUID = model.GovernorateGUID;
            }
            DbCOV.Create(dataCovidUNHCRResponseStrategy, Permissions.COVIDUNHCRResponse.CreateGuid, ExecutionTime, DbCMS);


            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.COVIDUNHCRResponse.Create, Apps.COV, new UrlHelper(Request.RequestContext).Action("CovidUNHCRResponse/Create", "CovidUNHCRResponse", new { Area = "COV" })), Container = "CovidUNHCRResponseFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.COVIDUNHCRResponse.Update, Apps.COV), Container = "CovidUNHCRResponseFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.COVIDUNHCRResponse.Delete, Apps.COV), Container = "CovidUNHCRResponseFormControls" });

            try
            {
                DbCOV.SaveChanges();
                DbCMS.SaveChanges();

                List<PrimaryKeyControl> primaryKeyControls = new List<PrimaryKeyControl>();
                primaryKeyControls.Add(DbCOV.PrimaryKeyControl(dataCovidUNHCRResponseStrategy));

                return Json(DbCOV.SingleCreateMessage(primaryKeyControls, DbCOV.RowVersionControls(Portal.SingleToList(dataCovidUNHCRResponseStrategy)), null, null, UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCOV.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult CovidUNHCRResponseUpdate(DataCovidUNHCRResponseStrategyUpdateModel model)
        {

            Guid GovernorateGUID = (from a in DbCOV.codeOchaLocations.Where(x => x.admin1Pcode == model.Governorate) select a.GovernorateGUID).FirstOrDefault().Value;
            model.GovernorateGUID = GovernorateGUID;
            if (!CMS.HasAction(Permissions.COVIDUNHCRResponse.Update, Apps.COV, model.GovernorateGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            //if (!ModelState.IsValid || ActiveApplication(model)) return PartialView("~/Areas/CMS/Views/Codes/Applications/_ApplicationForm.cshtml", model);
            DateTime ExecutionTime = DateTime.Now;




            dataCovidUNHCRResponseStrategy dataCovidUNHCRResponseStrategy = Mapper.Map(model, new dataCovidUNHCRResponseStrategy());
            dataCovidUNHCRResponseStrategy.UpdatedBy = UserGUID;
            dataCovidUNHCRResponseStrategy.UpdatedOn = ExecutionTime;

            //var CovUnitCost = (from a in DbCOV.codeCovUnits
            //                   where a.CovUnitGUID == model.CovUnitGUID
            //                   select a.CovUnitCost).FirstOrDefault();
            //dataCovidUNHCRResponseStrategy.UnitCost = CovUnitCost * model.Quantity;
            //dataCovidUNHCRResponseStrategy.CostEstimationThreeMonthes = CovUnitCost * model.Quantity * 3;

            var TechnicalUnitGUID = (from a in DbCOV.codeCovIndicators
                                     join b in DbCOV.codeTablesValuesLanguages on a.CovIndicatorTechnicalUnit.Trim().ToUpper() equals b.ValueDescription.Trim().ToUpper()
                                     where a.IndicatorGUID == model.IndicatorGUID
                                     select b.ValueGUID).FirstOrDefault();
            dataCovidUNHCRResponseStrategy.CovIndicatorTechnicalUnitGUID = TechnicalUnitGUID;

            if (!String.IsNullOrEmpty(model.Governorate))
            {
                dataCovidUNHCRResponseStrategy.GovernorateGUID = model.GovernorateGUID;
            }
            DbCOV.Update(dataCovidUNHCRResponseStrategy, Permissions.COVIDUNHCRResponse.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbCOV.SaveChanges();
                DbCMS.SaveChanges();

                List<PrimaryKeyControl> primaryKeyControls = new List<PrimaryKeyControl>();
                primaryKeyControls.Add(DbCOV.PrimaryKeyControl(dataCovidUNHCRResponseStrategy));

                return Json(DbCOV.SingleUpdateMessage(true, null, primaryKeyControls, DbCOV.RowVersionControls(Portal.SingleToList(dataCovidUNHCRResponseStrategy)), null));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost]
        public ActionResult CovidUNHCRResponseVerify(Guid CovidUNHCRResponseStrategyGUID)
        {

            dataCovidUNHCRResponseStrategy dataCovidUNHCRResponseStrategy = (from a in DbCOV.dataCovidUNHCRResponseStrategies
                                                                             where a.CovidUNHCRResponseStrategyGUID == CovidUNHCRResponseStrategyGUID
                                                                             select a).FirstOrDefault();

            if (CMS.HasAction(Permissions.COVIDRecordVerify.Create, Apps.COV))
            {
                dataCovidUNHCRResponseStrategy.IsVerified = true;
                dataCovidUNHCRResponseStrategy.VerifiedBy = UserGUID;
                dataCovidUNHCRResponseStrategy.VerifiedOn = DateTime.Now;

                DbCOV.SaveChanges();

                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });

            }

        }

        [Route("COV/CovidUNHCRResponse/Verify/")]
        public ActionResult CovidUNHCRResponseVerify()
        {
            if (!CMS.HasAction(Permissions.COVIDRecordVerify.Create, Apps.COV))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/COV/Views/CovidUNHCRResponse/_CovidUNHCRResponseVerifyForm.cshtml");
        }

        [HttpPost]
        public ActionResult CovidUNHCRResponseDataTableVerify(List<dataCovidUNHCRResponseStrategy> models)
        {
            if (models == null)
            {
                JsonReturn jr = new JsonReturn()
                {
                    DataTable = DataTableNames.CovidUNHCRResponseDataTable,
                    Notify = new Notify { Type = MessageTypes.Warning, Message = "No record is selected." }
                };

                return Json(jr);
            }
            DateTime ExecutionTime = DateTime.Now;
            List<Guid> CovidUNHCRResponseStrategyGUIDs = models.Select(x => x.CovidUNHCRResponseStrategyGUID).ToList();
            var dataCovidUNHCRResponseStrategiesData = (from a in DbCOV.dataCovidUNHCRResponseStrategies
                                                        where CovidUNHCRResponseStrategyGUIDs.Contains(a.CovidUNHCRResponseStrategyGUID)
                                                        && a.IsVerified == false
                                                        select a).ToList();

            foreach (var item in dataCovidUNHCRResponseStrategiesData)
            {
                item.IsVerified = true;
                item.VerifiedBy = UserGUID;
                item.VerifiedOn = ExecutionTime;
            }

            try
            {
                DbCOV.SaveChanges();
                JsonReturn jr = new JsonReturn()
                {
                    AffectedRecordsGuids = CovidUNHCRResponseStrategyGUIDs,
                    DataTable = DataTableNames.CovidUNHCRResponseDataTable,
                    Notify = new Notify { Type = MessageTypes.Success, Message = "Selected Records Is Verified Successfully" }
                };

                return Json(jr);
            }
            catch (Exception ex)
            {
                return Json(DbCOV.ErrorMessage(ex.Message));
            }

        }


        [HttpPost]
        public ActionResult CovidUNHCRResponseFeedback(Guid CovidUNHCRResponseStrategyGUID, string Feedback)
        {
            dataCovidUNHCRResponseStrategy dataCovidUNHCRResponseStrategy = (from a in DbCOV.dataCovidUNHCRResponseStrategies
                                                                             where a.CovidUNHCRResponseStrategyGUID == CovidUNHCRResponseStrategyGUID
                                                                             select a).FirstOrDefault();

            dataCovidUNHCRResponseStrategy.Feedback = Feedback;

            DbCOV.SaveChanges();
            string recipientEmail = "harkous@unhcr.org;jabasseh@unhcr.org;";
            //string recipientEmail = "karkoush@unhcr.org";
            new Email().SendCovFeedbackEmail(recipientEmail, null, "New Feedback added", CovidUNHCRResponseStrategyGUID);
            return Json(new { success = true });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CovidUNHCRResponseDelete(dataCovidUNHCRResponseStrategy model)
        {
            Guid GovernorateGUID = (from a in DbCOV.codeOchaLocations.Where(x => x.admin1Pcode == model.Governorate) select a.GovernorateGUID).FirstOrDefault().Value;
            model.GovernorateGUID = GovernorateGUID;
            if (!CMS.HasAction(Permissions.COVIDUNHCRResponse.Delete, Apps.COV, model.GovernorateGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataCovidUNHCRResponseStrategy> DeletedCovidUNHCRResponseStrategies = DeleteCovidUNHCRResponseStrategies(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.COVIDUNHCRResponse.Restore, Apps.COV), Container = "CovidUNHCRResponseFormControls" });

            try
            {
                int CommitedRows = DbCOV.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbCOV.SingleDeleteMessage(CommitedRows, DeletedCovidUNHCRResponseStrategies.FirstOrDefault(), null, UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CovidUNHCRResponseRestore(dataCovidUNHCRResponseStrategy model)
        {
            Guid GovernorateGUID = (from a in DbCOV.codeOchaLocations.Where(x => x.admin1Pcode == model.Governorate) select a.GovernorateGUID).FirstOrDefault().Value;
            model.GovernorateGUID = GovernorateGUID;
            if (!CMS.HasAction(Permissions.COVIDUNHCRResponse.Restore, Apps.COV, model.GovernorateGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            //if (ActiveApplication(model))
            //{
            //    return Json(DbCMS.RecordExists());
            //}

            List<dataCovidUNHCRResponseStrategy> RestoredCovidUNHCRResponseStrategies = RestoreCovidUNHCRResponseStrategies(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.COVIDUNHCRResponse.Create, Apps.COV, new UrlHelper(Request.RequestContext).Action("CovidUNHCRResponse/Create", "CovidUNHCRResponse", new { Area = "COV" })), Container = "CovidUNHCRResponseFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.COVIDUNHCRResponse.Update, Apps.COV), Container = "CovidUNHCRResponseFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.COVIDUNHCRResponse.Restore, Apps.COV), Container = "CovidUNHCRResponseFormControls" });

            try
            {
                int CommitedRows = DbCOV.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbCOV.SingleRestoreMessage(CommitedRows, RestoredCovidUNHCRResponseStrategies, DbCOV.PrimaryKeyControl(RestoredCovidUNHCRResponseStrategies.FirstOrDefault()), "", null, UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }


        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CovidUNHCRResponseDataTableDelete(List<dataCovidUNHCRResponseStrategy> models)
        {
            if (!CMS.HasAction(Permissions.COVIDUNHCRResponse.Delete, Apps.COV))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataCovidUNHCRResponseStrategy> DeletedCovidUNHCRResponseStrategies = DeleteCovidUNHCRResponseStrategies(models);

            try
            {
                DbCOV.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbCOV.PartialDeleteMessage(DeletedCovidUNHCRResponseStrategies, models, DataTableNames.CovidUNHCRResponseDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCOV.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CovidUNHCRResponseDataTableRestore(List<dataCovidUNHCRResponseStrategy> models)
        {
            if (!CMS.HasAction(Permissions.COVIDUNHCRResponse.Restore, Apps.COV))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataCovidUNHCRResponseStrategy> RestoredCovidUNHCRResponseStrategies = RestoreCovidUNHCRResponseStrategies(models);

            try
            {
                DbCOV.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbCOV.PartialRestoreMessage(RestoredCovidUNHCRResponseStrategies, models, DataTableNames.CovidUNHCRResponseDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCOV.ErrorMessage(ex.Message));
            }
        }

        private List<dataCovidUNHCRResponseStrategy> DeleteCovidUNHCRResponseStrategies(List<dataCovidUNHCRResponseStrategy> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataCovidUNHCRResponseStrategy> DeletedCovidUNHCRResponseStrategies = new List<dataCovidUNHCRResponseStrategy>();

            //THIS IS FOR PERMISSION TEST//
            //DO NOT DELETE THIS CODE
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = (from a in DbCMS.codeApplications
            //                    from f in DbCMS.codeApplicationsFactorForTest
            //                    where a.ApplicationGUID == f.ApplicationGUID
            //                    select new { a.ApplicationGUID, a.codeApplicationsRowVersion, f.OperationGUID, f.OrganizationGUID, f.CountryGUID }).AsQueryable().ToString();

            string query = DbCOV.QueryBuilder(models, Permissions.COVIDUNHCRResponse.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbCOV.Database.SqlQuery<dataCovidUNHCRResponseStrategy>(query).ToList();

            foreach (var record in Records)
            {
                DeletedCovidUNHCRResponseStrategies.Add(DbCOV.Delete(record, ExecutionTime, Permissions.COVIDUNHCRResponse.DeleteGuid, DbCMS));
            }

            return DeletedCovidUNHCRResponseStrategies;
        }
        private List<dataCovidUNHCRResponseStrategy> RestoreCovidUNHCRResponseStrategies(List<dataCovidUNHCRResponseStrategy> models)
        {
            Guid DeleteActionGUID = Permissions.COVIDUNHCRResponse.DeleteGuid;
            Guid RestoreActionGUID = Permissions.COVIDUNHCRResponse.RestoreGuid;
            DateTime RestoringTime = DateTime.Now;

            List<dataCovidUNHCRResponseStrategy> RestoredCovidUNHCRResponseStrategies = new List<dataCovidUNHCRResponseStrategy>();

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


            string query = DbCOV.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, "");

            var Records = DbCOV.Database.SqlQuery<dataCovidUNHCRResponseStrategy>(query).ToList();
            foreach (var record in Records)
            {
                //if (!ActiveApplication(record))
                //{
                //    RestoredApplications.Add(DbCMS.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                //}
                RestoredCovidUNHCRResponseStrategies.Add(DbCOV.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS));
            }

            return RestoredCovidUNHCRResponseStrategies;
        }



        #region Helpers
        public JsonResult GetCoordinates(string admin4Pcode)
        {
            var result = (from a in DbCOV.codeOchaLocations
                          where a.admin4Pcode == admin4Pcode
                          select new
                          {
                              a.Longitude_x,
                              a.Latitude_y
                          }).FirstOrDefault();

            return Json(new { Longitude = result.Longitude_x, Latitude = result.Latitude_y }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUnitCost(Guid CovUnitGUID)
        {
            var CovUnitCost = (from a in DbCOV.codeCovUnits
                               where a.CovUnitGUID == CovUnitGUID
                               select a.CovUnitCost).FirstOrDefault();
            return Json(new { CovUnitCost = CovUnitCost }, JsonRequestBehavior.AllowGet);

        }

        [Route("COV/CovidUNHCRResponse/GenerateExcelVersion")]
        public ActionResult GenerateExcelVersion()
        {
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.COVIDUNHCRResponse.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var dataCovidUNHCRResponseStrategies = (from a in DbCOV.v_dataCovidUNHCRResponseStrategyDataTable.Where(x => AuthorizedList.Contains(x.GovernorateGUID.ToString()))
                                                    where a.Active
                                                    orderby a.DateOfReport descending
                                                    select a).ToList();
            var dataCovidUNHCRResponseStrategiesX = (from a in DbCOV.dataCovidUNHCRResponseStrategies.Where(x => AuthorizedList.Contains(x.GovernorateGUID.ToString()))
                                                     where a.Active
                                                     orderby a.DateOfReport descending
                                                     select a).ToList();
            var codeOchaLocations = (from a in DbCOV.codeOchaLocations
                                     select a).ToList();
            var codeObjectives = (from a in DbCOV.codeCovObjectives
                                  select a).ToList();
            var codeOutputs = (from a in DbCOV.codeCovOutputs
                               select a).ToList();
            var codeIndicators = (from a in DbCOV.codeCovIndicators
                                  select a).ToList();
            string sourceFile = Server.MapPath("~/Areas/COV/Templates/COVID_19_UNHCR_Emergency_Response_Template.xlsx");

            string DisFolder = Server.MapPath("~/Areas/COV/Templates/COVID_19_UNHCR_Emergency_Response_Template" + DateTime.Now.ToBinary() + ".xlsx");

            System.IO.File.Copy(sourceFile, DisFolder);

            using (var package = new ExcelPackage(new FileInfo(DisFolder)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                worksheet.Row(1).Height = 20;
                worksheet.Row(1).Style.Font.Bold = true;

                int i = 2;
                foreach (var record in dataCovidUNHCRResponseStrategies)
                {
                    worksheet.Cells["A" + i].Value = record.UNHCRStrategy.Value;
                    worksheet.Cells["B" + i].Value = record.DateOfReport;
                    worksheet.Cells["C" + i].Value = record.CovStatus;
                    worksheet.Cells["D" + i].Value = codeOchaLocations.Where(x => x.admin1Pcode == record.Governorate).Select(x => x.admin1Name_en).FirstOrDefault();
                    worksheet.Cells["E" + i].Value = codeOchaLocations.Where(x => x.admin2Pcode == record.District).Select(x => x.admin2Name_en).FirstOrDefault();
                    worksheet.Cells["F" + i].Value = codeOchaLocations.Where(x => x.admin3Pcode == record.SubDistrict).Select(x => x.admin3Name_en).FirstOrDefault();
                    worksheet.Cells["G" + i].Value = codeOchaLocations.Where(x => x.admin4Pcode == record.CommunityName).Select(x => x.admin4Name_en).FirstOrDefault();
                    worksheet.Cells["H" + i].Value = codeOchaLocations.Where(x => x.admin5Pcode == record.Neighborhood).Select(x => x.admin5Name_en).FirstOrDefault();
                    worksheet.Cells["I" + i].Value = record.Location;
                    worksheet.Cells["J" + i].Value = record.Longitude.ToString();
                    worksheet.Cells["K" + i].Value = record.Latitude.ToString();
                    worksheet.Cells["L" + i].Value = codeObjectives.Where(x => x.ObjectiveGUID == record.ObjectiveGUID).Select(x => x.ObjectiveDescription).First();
                    worksheet.Cells["M" + i].Value = codeOutputs.Where(x => x.OutputGUID == record.OutputGUID).Select(x => x.OutputDescription).First();
                    worksheet.Cells["N" + i].Value = codeIndicators.Where(x => x.IndicatorGUID == record.IndicatorGUID).Select(x => x.IndicatorDescription).First();
                    worksheet.Cells["O" + i].Value = codeIndicators.Where(x => x.IndicatorGUID == record.IndicatorGUID).Select(x => x.CovIndicatorUnit).First();
                    worksheet.Cells["P" + i].Value = record.CovIndicatorTechnicalUnit;
                    worksheet.Cells["Q" + i].Value = record.Measure_Achievments;
                    worksheet.Cells["R" + i].Value = record.DirectActivities.HasValue ? record.DirectActivities.Value == true ? "Yes" : "No" : "";
                    worksheet.Cells["S" + i].Value = record.Associated.HasValue ? record.Associated.Value == true ? "Yes" : "No" : "";
                    worksheet.Cells["T" + i].Value = record.ImplementingPartner;

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
            string fileName = "COVID-19 UNHCR Response Strategy " + DateTime.Now.ToString("dddd, dd MMMM yyyy") + ".xlsx";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        #endregion
    }
}