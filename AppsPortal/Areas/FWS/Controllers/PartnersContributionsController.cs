using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using FWS_DAL.Model;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.FWS.Controllers
{
    public class PartnersContributionsController : FWSBaseController
    {
        //[Route("FWS/PartnersContributions/")]
        public ActionResult Index()
        {
            if (!CMS.HasAction(Permissions.PartnersContribution.Access, Apps.FWS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/FWS/Views/PartnersContributions/Index.cshtml");

        }

        [Route("FWS/PartnersContributions/PartnersContributionsDataTable/")]
        public JsonResult PartnersContributionsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<PartnersContributionsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<PartnersContributionsDataTableModel>(DataTable.Filters);
            }

            List<string> AuthorizedListSector = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.PartnersContribution.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();


            var All = (from a in DbFWS.dataPartnerContributions.AsNoTracking()
                      .Where(x => AuthorizedListSector.Contains(x.SubSectorGUID.ToString()))
                       join b in DbFWS.code4WSPartner.AsNoTracking() on a.AgencyGUID equals b.PartnerGUID
                       join c in DbFWS.code4WSPartner.AsNoTracking() on a.ImplementingPartnerGUID equals c.PartnerGUID into LJ1
                       join d in DbFWS.code4WSBeneficiaryType.AsNoTracking() on a.BeneficiariesTypeGUID equals d.BeneficiaryTypeGUID
                       join e in DbFWS.userPersonalDetailsLanguages.AsNoTracking().Where(x => x.LanguageID == "EN") on a.ApprovedBy equals e.UserGUID into LJ2
                       from RJ1 in LJ1.DefaultIfEmpty()
                       from RJ2 in LJ2.DefaultIfEmpty()
                       select new PartnersContributionsDataTableModel
                       {
                           PartnerContributionGUID = a.PartnerContributionGUID,
                           AgencyGUID = a.AgencyGUID.ToString(),
                           AgencyDescription = b.PartnerDescription,
                           ImplementingPartnerGUID = RJ1.PartnerGUID.ToString(),
                           ImplementingPartnerDescription = RJ1.PartnerDescription,
                           LocationLong = a.LocationLong,
                           ReportingMonth = a.ReportingMonth,
                           HRPProject = a.HRPProject ? "Yes" : "No",
                           ActivityLong = a.ActivityLong,
                           ActivityDetails = a.ActivityDetails,
                           DeliveryMode = a.DeliveryMode,
                           FacilityGUID = a.FacilityGUID.HasValue ? a.FacilityGUID.Value.ToString() : "",
                           DeliveryFacility = a.DeliveryFacility,
                           FWSUnit = a.FWSUnit,
                           FWSAnalysisUnit = a.FWSAnalysisUnit,
                           BeneficiariesTypeGUID = a.BeneficiariesTypeGUID.ToString(),
                           BeneficiariesTypeDescription = d.BeneficiaryTypeDescription,
                           PWD = a.PWD.HasValue ? (a.PWD.Value ? "Yes" : "No") : "",
                           SurvivorsOfExplosiveHazards = a.SurvivorsOfExplosiveHazards.HasValue ? (a.SurvivorsOfExplosiveHazards.Value ? "Yes" : "No") : "",
                           TotalReach = a.TotalReach,
                           SumBreakdown = a.SumBreakdown.HasValue ? a.SumBreakdown.Value.ToString() : "",
                           Girls = a.Girls.HasValue ? a.Girls.Value.ToString() : "",
                           Boys = a.Boys.HasValue ? a.Boys.Value.ToString() : "",
                           AdolescentGirls = a.AdolescentGirls.HasValue ? a.AdolescentGirls.Value.ToString() : "",
                           AdolescentBoys = a.AdolescentBoys.HasValue ? a.AdolescentBoys.Value.ToString() : "",
                           Women = a.Women.HasValue ? a.Women.Value.ToString() : "",
                           Men = a.Men.HasValue ? a.Men.Value.ToString() : "",
                           ElderlyWomen = a.ElderlyWomen.HasValue ? a.ElderlyWomen.Value.ToString() : "",
                           ElderlyMen = a.ElderlyMen.HasValue ? a.ElderlyMen.Value.ToString() : "",
                           SumBreakdownNew = a.SumBreakdownNew.HasValue ? a.SumBreakdownNew.Value.ToString() : "",
                           GirlsNew = a.GirlsNew.HasValue ? a.GirlsNew.Value.ToString() : "",
                           BoysNew = a.BoysNew.HasValue ? a.BoysNew.Value.ToString() : "",
                           AdolescentGirlsNew = a.AdolescentGirlsNew.HasValue ? a.AdolescentGirlsNew.Value.ToString() : "",
                           AdolescentBoysNew = a.AdolescentBoysNew.HasValue ? a.AdolescentBoysNew.Value.ToString() : "",
                           WomenNew = a.WomenNew.HasValue ? a.WomenNew.Value.ToString() : "",
                           MenNew = a.MenNew.HasValue ? a.MenNew.Value.ToString() : "",
                           ElderlyWomenNew = a.ElderlyWomenNew.HasValue ? a.ElderlyWomenNew.Value.ToString() : "",
                           ElderlyMenNew = a.ElderlyMenNew.HasValue ? a.ElderlyMenNew.Value.ToString() : "",
                           Admin1Code = a.Admin1Code,
                           Admin2Code = a.Admin2Code,
                           Admin3Code = a.Admin3Code,
                           Admin4Code = a.Admin4Code,
                           LocationCode = a.LocationCode,
                           Admin1Name = a.Admin1Name,
                           Admin2Name = a.Admin2Name,
                           Admin3Name = a.Admin3Name,
                           Admin4Name = a.Admin4Name,
                           LocationName = a.LocationName,
                           FWSSubSector = a.FWSSubSector,
                           FWSActivity = a.FWSActivity,
                           FWSSubActivity = a.FWSSubActivity,
                           IsDataApproved = a.IsDataApproved,
                           ApprovedOn = a.ApprovedOn,
                           ApprovedBy = a.ApprovedBy,
                           ApprovedByName = RJ2.FirstName + " " + RJ2.Surname,
                           Active = a.Active,
                           dataPartnerContributionRowVersion = a.dataPartnerContributionRowVersion,
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<PartnersContributionsDataTableModel> Result = Mapper.Map<List<PartnersContributionsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("FWS/PartnersContributions/Create/")]
        public ActionResult PartnersContributionsCreate()
        {
            if (!CMS.HasAction(Permissions.PartnersContribution.Create, Apps.FWS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            PartnersContributionsUpdateModel model = new PartnersContributionsUpdateModel();
            model.HubGUID = Guid.Parse("0BE5E550-A638-4AFD-8856-94DEF25690C2");
            return View("~/Areas/FWS/Views/PartnersContributions/PartnersContributions.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PartnersContributionsCreate(PartnersContributionsUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.PartnersContribution.Create, Apps.FWS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            DateTime ExecutionTime = DateTime.Now;
            dataPartnerContribution dataPartnerContribution = Mapper.Map(model, new dataPartnerContribution());
            dataPartnerContribution.HubGUID = Guid.Parse("0BE5E550-A638-4AFD-8856-94DEF25690C2");
            dataPartnerContribution.LocationGUID = Guid.Parse(model.LocationLong);
            dataPartnerContribution.LocationLong = (from a in DbFWS.code4WSLocation where a.LocationGUID == dataPartnerContribution.LocationGUID select a.Location).FirstOrDefault();
            dataPartnerContribution.DeliveryMode = (from a in DbFWS.code4WSActivityTag where a.ActivityTagGUID == model.DeliveryModeGUID select a.ActivityTagDescription).FirstOrDefault();
            dataPartnerContribution.DeliveryFacility = (from a in DbFWS.code4WSFacility where a.FacilityGUID == model.FacilityGUID select a.FacilityCode).FirstOrDefault();
            dataPartnerContribution.IsDataApproved = false;
            dataPartnerContribution.Active = true;
            DbFWS.Create(dataPartnerContribution, Permissions.PartnersContribution.CreateGuid, ExecutionTime, DbCMS);

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.PartnersContribution.Create, Apps.FWS, new UrlHelper(Request.RequestContext).Action("PartnersContributions/Create", "PartnersContributions", new { Area = "FWS" })), Container = "PartnersContributionsFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.PartnersContribution.Update, Apps.FWS), Container = "PartnersContributionsFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.PartnersContribution.Delete, Apps.FWS), Container = "PartnersContributionsFormControls" });

            try
            {
                DbFWS.SaveChanges();
                DbCMS.SaveChanges();

                List<PrimaryKeyControl> primaryKeyControls = new List<PrimaryKeyControl>();

                primaryKeyControls.Add(DbFWS.PrimaryKeyControl(dataPartnerContribution));
                string url = "/FWS/PartnersContributions/Update/" + dataPartnerContribution.PartnerContributionGUID.ToString();
                string callBackFunc = "$(location).attr('href', '" + url + "')";
                return Json(DbFWS.SingleCreateMessage(primaryKeyControls, DbFWS.RowVersionControls(Portal.SingleToList(dataPartnerContribution)), null, callBackFunc, UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbFWS.ErrorMessage(ex.Message));
            }
        }

        [Route("FWS/PartnersContributions/Update/{PK}")]
        public ActionResult PartnersContributionsUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.PartnersContribution.Update, Apps.FWS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var data = (from a in DbFWS.dataPartnerContributions.AsNoTracking() where a.PartnerContributionGUID == PK select a).FirstOrDefault();
            PartnersContributionsUpdateModel model = Mapper.Map(data, new PartnersContributionsUpdateModel());
            model.HubGUID = Guid.Parse("0BE5E550-A638-4AFD-8856-94DEF25690C2");
            model.LocationGUID = (from a in DbFWS.code4WSLocation where a.Location == data.LocationLong select a.LocationGUID).FirstOrDefault();
            model.LocationLong = model.LocationGUID.ToString();
            model.DeliveryModeGUID = (from a in DbFWS.code4WSActivityTag where a.ActivityTagDescription.Trim().ToUpper() == model.DeliveryMode.Trim().ToUpper() select a.ActivityTagGUID).FirstOrDefault();
            return View("~/Areas/FWS/Views/PartnersContributions/PartnersContributions.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PartnersContributionsUpdate(PartnersContributionsUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.PartnersContribution.Update, Apps.FWS, model.SubSectorGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            DateTime ExecutionTime = DateTime.Now;
            dataPartnerContribution dataPartnerContribution = Mapper.Map(model, new dataPartnerContribution());
            dataPartnerContribution.HubGUID = Guid.Parse("0BE5E550-A638-4AFD-8856-94DEF25690C2");
            dataPartnerContribution.DeliveryMode = (from a in DbFWS.code4WSActivityTag where a.ActivityTagGUID == model.DeliveryModeGUID select a.ActivityTagDescription).FirstOrDefault();
            dataPartnerContribution.LocationGUID = Guid.Parse(model.LocationLong);
            dataPartnerContribution.LocationLong = (from a in DbFWS.code4WSLocation where a.LocationGUID == dataPartnerContribution.LocationGUID select a.Location).FirstOrDefault();
            dataPartnerContribution.DeliveryFacility = (from a in DbFWS.code4WSFacility where a.FacilityGUID == model.FacilityGUID select a.FacilityCode).FirstOrDefault();

            DbFWS.Update(dataPartnerContribution, Permissions.PartnersContribution.UpdateGuid, ExecutionTime, DbCMS);
            try
            {
                DbFWS.SaveChanges();
                DbCMS.SaveChanges();
                List<PrimaryKeyControl> primaryKeyControls = new List<PrimaryKeyControl>();
                primaryKeyControls.Add(DbFWS.PrimaryKeyControl(dataPartnerContribution));
                string url = "/FWS/PartnersContributions/Update/" + dataPartnerContribution.PartnerContributionGUID.ToString();
                string callBackFunc = "$(location).attr('href', '" + url + "')";
                return Json(DbFWS.SingleUpdateMessage(true, null, primaryKeyControls, DbFWS.RowVersionControls(Portal.SingleToList(model)), callBackFunc, "Record Updated Successfully"));
            }
            catch (Exception ex)
            {
                return Json(DbFWS.ErrorMessage(ex.Message));
            }

        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PartnersContributionsDelete(dataPartnerContribution model)
        {

            if (!CMS.HasAction(Permissions.PartnersContribution.Delete, Apps.FWS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataPartnerContribution> DeletedPartnerContribution = DeletePartnerContributionRecords(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.PartnersContribution.Restore, Apps.FWS), Container = "PartnersContributionsFormControls" });

            try
            {
                int CommitedRows = DbFWS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbFWS.SingleDeleteMessage(CommitedRows, DeletedPartnerContribution.FirstOrDefault(), null, UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PartnersContributionsRestore(dataPartnerContribution model)
        {

            if (!CMS.HasAction(Permissions.PartnersContribution.Restore, Apps.FWS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            //if (ActiveApplication(model))
            //{
            //    return Json(DbCMS.RecordExists());
            //}

            List<dataPartnerContribution> RestoredPartnerContribution = RestorePartnerContributionRecords(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.PartnersContribution.Create, Apps.FWS, new UrlHelper(Request.RequestContext).Action("PartnersContributions/Create", "PartnersContributions", new { Area = "FWS" })), Container = "PartnersContributionsFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.PartnersContribution.Update, Apps.FWS), Container = "PartnersContributionsFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.PartnersContribution.Restore, Apps.FWS), Container = "PartnersContributionsFormControls" });

            try
            {
                int CommitedRows = DbFWS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbFWS.SingleRestoreMessage(CommitedRows, RestoredPartnerContribution, DbFWS.PrimaryKeyControl(RestoredPartnerContribution.FirstOrDefault()), "", null, UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult PartnersContributionsDataTableDelete(List<dataPartnerContribution> models)
        {
            if (!CMS.HasAction(Permissions.PartnersContribution.Delete, Apps.FWS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPartnerContribution> DeletedPartnerContributions = DeletePartnerContributionRecords(models);

            try
            {
                DbFWS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbFWS.PartialDeleteMessage(DeletedPartnerContributions, models, DataTableNames.PartnersContributionsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbFWS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult PartnersContributionsDataTableRestore(List<dataPartnerContribution> models)
        {
            if (!CMS.HasAction(Permissions.PartnersContribution.Restore, Apps.FWS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPartnerContribution> RestoredPartnerContribution = RestorePartnerContributionRecords(models);

            try
            {
                DbFWS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbFWS.PartialRestoreMessage(RestoredPartnerContribution, models, DataTableNames.PartnersContributionsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbFWS.ErrorMessage(ex.Message));
            }
        }
        private List<dataPartnerContribution> DeletePartnerContributionRecords(List<dataPartnerContribution> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataPartnerContribution> DeletedPartnerContributions = new List<dataPartnerContribution>();

            //THIS IS FOR PERMISSION TEST//
            //DO NOT DELETE THIS CODE
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = (from a in DbCMS.codeApplications
            //                    from f in DbCMS.codeApplicationsFactorForTest
            //                    where a.ApplicationGUID == f.ApplicationGUID
            //                    select new { a.ApplicationGUID, a.codeApplicationsRowVersion, f.OperationGUID, f.OrganizationGUID, f.CountryGUID }).AsQueryable().ToString();

            string query = DbFWS.QueryBuilder(models, Permissions.PartnersContribution.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbFWS.Database.SqlQuery<dataPartnerContribution>(query).ToList();

            foreach (var record in Records)
            {
                DeletedPartnerContributions.Add(DbFWS.Delete(record, ExecutionTime, Permissions.PartnersContribution.DeleteGuid, DbCMS));
            }

            return DeletedPartnerContributions;
        }
        private List<dataPartnerContribution> RestorePartnerContributionRecords(List<dataPartnerContribution> models)
        {
            Guid DeleteActionGUID = Permissions.PartnersContribution.DeleteGuid;
            Guid RestoreActionGUID = Permissions.PartnersContribution.RestoreGuid;
            DateTime RestoringTime = DateTime.Now;

            List<dataPartnerContribution> RestoredPartnerContributions = new List<dataPartnerContribution>();

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


            string query = DbFWS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, "");

            var Records = DbFWS.Database.SqlQuery<dataPartnerContribution>(query).ToList();
            foreach (var record in Records)
            {
                //if (!ActiveApplication(record))
                //{
                //    RestoredApplications.Add(DbCMS.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime));
                //}
                RestoredPartnerContributions.Add(DbFWS.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS));
            }

            return RestoredPartnerContributions;
        }



        [Route("FWS/PartnersContributions/Upload/")]
        public ActionResult PartnersContributionsUpload()
        {
            if (!CMS.HasAction(Permissions.PartnersContribution.Upload, Apps.FWS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/FWS/Views/PartnersContributions/_PartnersContributionsUploadForm.cshtml", new PartnersContributionsUploadFormModel());
        }

        private static IDictionary<Guid, int> uploadFormTask = new Dictionary<Guid, int>();

        [HttpPost]
        public ActionResult PartnersContributionsUpload(PartnersContributionsUploadFormModel model)
        {
            if (!CMS.HasAction(Permissions.PartnersContribution.Upload, Apps.FWS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            DateTime ExecutionTime = DateTime.Now;
            Guid EntityPK = Guid.NewGuid();

            var uploadFormTaskID = Guid.NewGuid();
            uploadFormTask.Add(uploadFormTaskID, 0);

            int _totalRowsCount = 0;
            var _tempfile = Request.Files[0];
            var _tempstearm = _tempfile.InputStream;

            using (var package = new ExcelPackage(_tempstearm))
            {
                ExcelWorksheet ws;
                try
                {
                    ws = package.Workbook.Worksheets[1];
                }
                catch (Exception)
                {
                    ws = package.Workbook.Worksheets[1];
                }
                _totalRowsCount = ws.Dimension.End.Row;
            }


            var file = Request.Files[0];
            var _stearm = file.InputStream;
            string _ext = Path.GetExtension(file.FileName);

            file.SaveAs(Server.MapPath("~/Areas/FWS/Uploads/" + EntityPK.ToString() + _ext));

            Task.Factory.StartNew(() =>
            {

                List<dataPartnerContributionFile> dataPartnerContributionFiles = new List<dataPartnerContributionFile>();
                List<dataPartnerContribution> dataPartnerContributions = new List<dataPartnerContribution>();

                dataPartnerContributionFile dataPartnerContributionFile = new dataPartnerContributionFile();
                dataPartnerContributionFile.PartnerContributionFileGUID = EntityPK;
                dataPartnerContributionFile.UploadedOn = ExecutionTime;
                dataPartnerContributionFile.UploadedBy = UserGUID;
                dataPartnerContributionFile.FilePath = "~/Areas/FWS/Uploads/" + EntityPK.ToString() + _ext;
                dataPartnerContributionFile.Active = true;
                dataPartnerContributionFiles.Add(dataPartnerContributionFile);

                List<code4WSPartner> _code4WSPartners = (from a in DbFWS.code4WSPartner select a).ToList();
                List<code4WSBeneficiaryType> _code4WSBeneficiaryTypes = (from a in DbFWS.code4WSBeneficiaryType select a).ToList();
                List<code4WSActivityTag> _code4WSActivityTag = (from a in DbFWS.code4WSActivityTag select a).ToList();
                List<code4WSFacility> _code4WSFacilities = (from a in DbFWS.code4WSFacility select a).ToList();
                List<code4WSSubSector> _code4WSSubSector = (from a in DbFWS.code4WSSubSector select a).ToList();
                List<code4WSLocation> _code4WSLocation = (from a in DbFWS.code4WSLocation select a).ToList();

                FileInfo protectionSectorFile = new FileInfo(Server.MapPath(dataPartnerContributionFile.FilePath));
                using (var package = new ExcelPackage(protectionSectorFile))
                {
                    ExcelWorksheet worksheet;
                    try
                    {
                        worksheet = package.Workbook.Worksheets[1];
                    }
                    catch (Exception)
                    {
                        worksheet = package.Workbook.Worksheets[1];
                    }

                    for (int i = 4; i <= _totalRowsCount; i++)
                    {
                        dataPartnerContribution dataPartnerContribution = new dataPartnerContribution();
                        dataPartnerContribution.PartnerContributionGUID = Guid.NewGuid();
                        dataPartnerContribution.PartnerContributionFileGUID = EntityPK;
                        /////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////
                        dataPartnerContribution.HubGUID = Guid.Parse("0BE5E550-A638-4AFD-8856-94DEF25690C2");//Always SYR
                        /////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////
                        try
                        {
                            string Agency = worksheet.Cells["A" + i].Value.ToString();
                            Guid AgencyGUID = _code4WSPartners.Where(x => x.PartnerDescription.Trim().ToUpper() == Agency.Trim().ToUpper()).Select(x => x.PartnerGUID).FirstOrDefault();
                            dataPartnerContribution.AgencyGUID = AgencyGUID;
                        }
                        catch { continue; }

                        /////////////////////////////////////////////////////
                        try
                        {
                            string ImplementingPartner = worksheet.Cells["B" + i].Value.ToString();
                            Guid ImplementingPartnerGUID = _code4WSPartners.Where(x => x.PartnerDescription.Trim().ToUpper() == ImplementingPartner.Trim().ToUpper()).Select(x => x.PartnerGUID).FirstOrDefault();
                            dataPartnerContribution.ImplementingPartnerGUID = ImplementingPartnerGUID;
                        }
                        catch
                        {
                            dataPartnerContribution.ImplementingPartnerGUID = null;
                        }

                        /////////////////////////////////////////////////////
                        try
                        {
                            string LocationLong = worksheet.Cells["C" + i].Value.ToString();
                            dataPartnerContribution.LocationLong = LocationLong;
                            dataPartnerContribution.LocationGUID = (from a in _code4WSLocation where a.Location == LocationLong select a.LocationGUID).FirstOrDefault();
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string ReportingMonth = worksheet.Cells["D" + i].Value.ToString();
                            dataPartnerContribution.ReportingMonth = Convert.ToInt32(ReportingMonth.Split('-')[0].Trim());
                            dataPartnerContribution.ReportingYear = 2020;
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string HRPProject = worksheet.Cells["E" + i].Value.ToString();
                            dataPartnerContribution.HRPProject = HRPProject.Trim().ToUpper() == "YES" ? true : false;
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string ActivityLong = worksheet.Cells["F" + i].Value.ToString();
                            dataPartnerContribution.ActivityLong = ActivityLong;
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string ActivityDetails = worksheet.Cells["G" + i].Value.ToString();
                            dataPartnerContribution.ActivityDetails = ActivityDetails;
                        }
                        catch
                        {
                            dataPartnerContribution.ActivityDetails = "";
                        }
                        /////////////////////////////////////////////////////
                        string DeliveryMode = worksheet.Cells["H" + i].Value.ToString();
                        dataPartnerContribution.DeliveryMode = DeliveryMode;
                        dataPartnerContribution.DeliveryModeGUID = (from a in _code4WSActivityTag where a.ActivityTagDescription.Trim().ToUpper() == DeliveryMode.Trim().ToUpper() select a.ActivityTagGUID).FirstOrDefault();
                        /////////////////////////////////////////////////////
                        try
                        {
                            string Facility = worksheet.Cells["I" + i].Value.ToString();
                            dataPartnerContribution.DeliveryFacility = Facility;
                            dataPartnerContribution.FacilityGUID = (from a in _code4WSFacilities where a.FacilityCode.Trim().ToUpper() == Facility.Trim().ToUpper() select a.FacilityGUID).FirstOrDefault();
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string Unit = worksheet.Cells["J" + i].Value.ToString();
                            dataPartnerContribution.FWSUnit = Unit;
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string AnalysisUnit = worksheet.Cells["K" + i].Value.ToString();
                            dataPartnerContribution.FWSAnalysisUnit = AnalysisUnit;
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string BeneficiariesType = worksheet.Cells["L" + i].Value.ToString();
                            Guid BeneficiariesTypeGUID = _code4WSBeneficiaryTypes.Where(x => x.BeneficiaryTypeDescription.Trim().ToUpper() == BeneficiariesType.Trim().ToUpper()).Select(x => x.BeneficiaryTypeGUID).FirstOrDefault();
                            dataPartnerContribution.BeneficiariesTypeGUID = BeneficiariesTypeGUID;
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string PWD = worksheet.Cells["M" + i].Value.ToString();
                            if (String.IsNullOrWhiteSpace(PWD.Trim().ToUpper()))
                            {
                                dataPartnerContribution.PWD = null;
                            }
                            else if (PWD.Trim().ToUpper() == "YES")
                            {
                                dataPartnerContribution.PWD = true;
                            }
                            else if (PWD.Trim().ToUpper() == "NO")
                            {
                                dataPartnerContribution.PWD = false;
                            }
                        }
                        catch { dataPartnerContribution.PWD = null; }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string SurvivorsOfExplosives = worksheet.Cells["N" + i].Value.ToString();
                            if (String.IsNullOrWhiteSpace(SurvivorsOfExplosives.Trim().ToUpper()))
                            {
                                dataPartnerContribution.SurvivorsOfExplosiveHazards = null;
                            }
                            else if (SurvivorsOfExplosives.Trim().ToUpper() == "YES")
                            {
                                dataPartnerContribution.SurvivorsOfExplosiveHazards = true;
                            }
                            else if (SurvivorsOfExplosives.Trim().ToUpper() == "NO")
                            {
                                dataPartnerContribution.SurvivorsOfExplosiveHazards = false;
                            }
                        }
                        catch { dataPartnerContribution.SurvivorsOfExplosiveHazards = null; }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string TotalReach = worksheet.Cells["O" + i].Value.ToString();
                            dataPartnerContribution.TotalReach = Convert.ToInt32(TotalReach);
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string SumBreakdown = worksheet.Cells["P" + i].Value.ToString();
                            dataPartnerContribution.SumBreakdown = Convert.ToInt32(SumBreakdown);
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string Girls = worksheet.Cells["Q" + i].Value.ToString();
                            dataPartnerContribution.Girls = Convert.ToInt32(Girls);
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string Boys = worksheet.Cells["R" + i].Value.ToString();
                            dataPartnerContribution.Boys = Convert.ToInt32(Boys);
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string AdolescentGirls = worksheet.Cells["S" + i].Value.ToString();
                            dataPartnerContribution.AdolescentGirls = Convert.ToInt32(AdolescentGirls);
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string AdolescentBoys = worksheet.Cells["T" + i].Value.ToString();
                            dataPartnerContribution.AdolescentBoys = Convert.ToInt32(AdolescentBoys);
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string Women = worksheet.Cells["U" + i].Value.ToString();
                            dataPartnerContribution.Women = Convert.ToInt32(Women);
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string Men = worksheet.Cells["V" + i].Value.ToString();
                            dataPartnerContribution.Men = Convert.ToInt32(Men);
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string ElderlyWomen = worksheet.Cells["W" + i].Value.ToString();
                            dataPartnerContribution.ElderlyWomen = Convert.ToInt32(ElderlyWomen);
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string ElderlyMen = worksheet.Cells["X" + i].Value.ToString();
                            dataPartnerContribution.ElderlyMen = Convert.ToInt32(ElderlyMen);
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string SumBreakdownNew = worksheet.Cells["Y" + i].Value.ToString();
                            dataPartnerContribution.SumBreakdownNew = Convert.ToInt32(SumBreakdownNew);
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string GirlsNew = worksheet.Cells["Z" + i].Value.ToString();
                            dataPartnerContribution.GirlsNew = Convert.ToInt32(GirlsNew);
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string BoysNew = worksheet.Cells["AA" + i].Value.ToString();
                            dataPartnerContribution.BoysNew = Convert.ToInt32(BoysNew);
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string AdolescentGirlsNew = worksheet.Cells["AB" + i].Value.ToString();
                            dataPartnerContribution.AdolescentGirlsNew = Convert.ToInt32(AdolescentGirlsNew);
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string AdolescentBoysNew = worksheet.Cells["AC" + i].Value.ToString();
                            dataPartnerContribution.AdolescentBoysNew = Convert.ToInt32(AdolescentBoysNew);
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string WomenNew = worksheet.Cells["AD" + i].Value.ToString();
                            dataPartnerContribution.WomenNew = Convert.ToInt32(WomenNew);
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string MenNew = worksheet.Cells["AE" + i].Value.ToString();
                            dataPartnerContribution.MenNew = Convert.ToInt32(MenNew);
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string ElderlyWomenNew = worksheet.Cells["AF" + i].Value.ToString();
                            dataPartnerContribution.ElderlyWomenNew = Convert.ToInt32(ElderlyWomenNew);
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string ElderlyMenNew = worksheet.Cells["AG" + i].Value.ToString();
                            dataPartnerContribution.ElderlyMenNew = Convert.ToInt32(ElderlyMenNew);
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string Admin1Code = worksheet.Cells["AH" + i].Value.ToString();
                            dataPartnerContribution.Admin1Code = Admin1Code;
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string Admin2Code = worksheet.Cells["AI" + i].Value.ToString();
                            dataPartnerContribution.Admin2Code = Admin2Code;
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string Admin3Code = worksheet.Cells["AJ" + i].Value.ToString();
                            dataPartnerContribution.Admin3Code = Admin3Code;
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string Admin4Code = worksheet.Cells["AK" + i].Value.ToString();
                            dataPartnerContribution.Admin4Code = Admin4Code;
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string LocationCode = worksheet.Cells["AL" + i].Value.ToString();
                            dataPartnerContribution.LocationCode = LocationCode;
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string Admin1Name = worksheet.Cells["AM" + i].Value.ToString();
                            dataPartnerContribution.Admin1Name = Admin1Name;
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string Admin2Name = worksheet.Cells["AN" + i].Value.ToString();
                            dataPartnerContribution.Admin2Name = Admin2Name;
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string Admin3Name = worksheet.Cells["AO" + i].Value.ToString();
                            dataPartnerContribution.Admin3Name = Admin3Name;
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string Admin4Name = worksheet.Cells["AP" + i].Value.ToString();
                            dataPartnerContribution.Admin4Name = Admin4Name;
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string LocationName = worksheet.Cells["AQ" + i].Value.ToString();
                            dataPartnerContribution.LocationName = LocationName;
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string SubSector = worksheet.Cells["AR" + i].Value.ToString();
                            dataPartnerContribution.FWSSubSector = SubSector;
                            var _subSector = (from a in _code4WSSubSector where a.SubSectorDescription.Trim().ToUpper() == SubSector.Trim().ToUpper() select new { a.SubSectorID, a.SubSectorGUID }).FirstOrDefault();
                            dataPartnerContribution.SubSectorID = _subSector.SubSectorID;
                            dataPartnerContribution.SubSectorGUID = _subSector.SubSectorGUID;
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string Activity = worksheet.Cells["AS" + i].Value.ToString();
                            dataPartnerContribution.FWSActivity = Activity;
                        }
                        catch { }
                        /////////////////////////////////////////////////////
                        try
                        {
                            string SubActivity = worksheet.Cells["AT" + i].Value.ToString();
                            dataPartnerContribution.FWSSubActivity = SubActivity;
                        }
                        catch { }
                        /////////////////////////////////////////////////////


                        dataPartnerContribution.IsDataApproved = false;
                        dataPartnerContribution.Active = true;
                        dataPartnerContributions.Add(dataPartnerContribution);
                        uploadFormTask[uploadFormTaskID] = i;
                    }
                }
                DbFWS.BulkInsert(dataPartnerContributionFiles);
                DbFWS.BulkInsert(dataPartnerContributions);
                try
                {
                    DbFWS.BulkSaveChanges();
                    uploadFormTask.Remove(uploadFormTaskID);
                    return Json(DbFWS.SingleCreateMessage("Form uploaded successfully"));
                }
                catch (Exception ex)
                {
                    return Json(DbFWS.ErrorMessage(ex.Message));
                }

            });
            return Json(new { uploadFormTaskID = uploadFormTaskID, TotalRecords = _totalRowsCount });
        }

        public ActionResult UploadFormProgressBar(Guid id)
        {
            return Json(uploadFormTask.Keys.Where(x => x.ToString() == id.ToString()).Contains(id) ? uploadFormTask[id] : -100);
        }

        [Route("FWS/PartnersContributions/Validate/")]
        public ActionResult PartnersContributionsValidate()
        {
            if (!CMS.HasAction(Permissions.PartnersContribution.ValidateData, Apps.FWS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/FWS/Views/PartnersContributions/_PartnersContributionsValidateForm.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PartnersContributionsDataTableValidate(List<dataPartnerContribution> models)
        {
            if (!CMS.HasAction(Permissions.PartnersContribution.ValidateData, Apps.FWS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            DateTime ExecutionTime = DateTime.Now;
            List<Guid> PartnerContributionGUIDs = models.Select(x => x.PartnerContributionGUID).ToList();
            var dataPartnerContributionsData = (from a in DbFWS.dataPartnerContributions
                                                where PartnerContributionGUIDs.Contains(a.PartnerContributionGUID)
                                                && a.IsDataApproved == false
                                                select a).ToList();

            foreach (var item in dataPartnerContributionsData)
            {
                item.IsDataApproved = true;
                item.ApprovedBy = UserGUID;
                item.ApprovedOn = ExecutionTime;
            }

            try
            {
                DbFWS.BulkSaveChanges();
                DbFWS.SaveChanges();
                JsonReturn jr = new JsonReturn()
                {
                    AffectedRecordsGuids = PartnerContributionGUIDs,
                    DataTable = DataTableNames.PartnersContributionsDataTable,
                    Notify = new Notify { Type = MessageTypes.Success, Message = "Selected Records Approved Successfully" }
                };

                return Json(jr);
            }
            catch (Exception ex)
            {
                return Json(DbFWS.ErrorMessage(ex.Message));
            }
            List<code4WSSeverityRanking> _code4WSSeverityRankings = (from a in DbFWS.code4WSSeverityRanking select a).ToList();
            List<code4WSActivity> _code4WSActivity = (from a in DbFWS.code4WSActivity select a).ToList();
            List<code4WSLocation> _code4WSLocation = (from a in DbFWS.code4WSLocation select a).ToList();
            List<code4WSPartner> _code4WSPartner = (from a in DbFWS.code4WSPartner select a).ToList();
            List<code4WSHub> _code4WSHub = (from a in DbFWS.code4WSHub select a).ToList();
            List<code4WSOrgTypeByHub> _code4WSOrgTypeByHub = (from a in DbFWS.code4WSOrgTypeByHub select a).ToList();
            List<code4WSAccessStatus> _code4WSAccessStatus = (from a in DbFWS.code4WSAccessStatus select a).ToList();


            List<dataMasterTable> MasterRecords = new List<dataMasterTable>();
            for (int i = 0; i < dataPartnerContributionsData.Count; i++)
            {
                dataPartnerContributionsData[i].IsDataApproved = true;
                dataPartnerContributionsData[i].ApprovedBy = UserGUID;
                dataPartnerContributionsData[i].ApprovedOn = ExecutionTime;

                //insert data into 4WS Master Table
                dataMasterTable dataMasterTable = new dataMasterTable();
                dataMasterTable.MasterTableGUID = Guid.NewGuid();
                dataMasterTable.PartnerContributionGUID = dataPartnerContributionsData[i].PartnerContributionGUID;
                dataMasterTable.HubGUID = dataPartnerContributionsData[i].HubGUID;
                dataMasterTable.AgencyGUID = dataPartnerContributionsData[i].AgencyGUID;
                dataMasterTable.ImplementingPartnerGUID = dataPartnerContributionsData[i].ImplementingPartnerGUID;
                dataMasterTable.Funding_UN = "???";
                dataMasterTable.Admin1RefName = dataPartnerContributionsData[i].Admin1Name;
                dataMasterTable.Admin2RefName = dataPartnerContributionsData[i].Admin2Name;
                dataMasterTable.Admin3RefName = dataPartnerContributionsData[i].Admin3Name;
                dataMasterTable.NeibourhoodRefName = dataPartnerContributionsData[i].LocationName;
                dataMasterTable.Camps = "???";
                dataMasterTable.ReportingYear = dataPartnerContributionsData[i].ReportingYear;
                dataMasterTable.ReportingMonth = dataPartnerContributionsData[i].ReportingMonth;
                dataMasterTable.HRPProject = dataPartnerContributionsData[i].HRPProject;
                dataMasterTable.FWSSubSector = dataPartnerContributionsData[i].FWSSubSector;
                dataMasterTable.FWSActivity = dataPartnerContributionsData[i].FWSActivity;
                dataMasterTable.FWSSubActivity = dataPartnerContributionsData[i].FWSSubActivity;
                dataMasterTable.DeliveryMode = dataPartnerContributionsData[i].DeliveryMode;
                dataMasterTable.FWSUnit = dataPartnerContributionsData[i].FWSUnit;

                dataMasterTable.BeneficiariesTypeGUID = dataPartnerContributionsData[i].BeneficiariesTypeGUID;
                dataMasterTable.PWD = dataPartnerContributionsData[i].PWD;
                dataMasterTable.Instructions = (from a in _code4WSActivity where a.SubActivityOrg.Trim().ToUpper() == dataPartnerContributionsData[i].FWSSubActivity.Trim().ToUpper() select a.Instructions).FirstOrDefault();

                dataMasterTable.Girls = dataPartnerContributionsData[i].Girls;
                dataMasterTable.Boys = dataPartnerContributionsData[i].Boys;
                dataMasterTable.Women = dataPartnerContributionsData[i].Women;
                dataMasterTable.Men = dataPartnerContributionsData[i].Men;
                dataMasterTable.ElderlyWomen = dataPartnerContributionsData[i].ElderlyWomen;
                dataMasterTable.ElderlyMen = dataPartnerContributionsData[i].ElderlyMen;
                dataMasterTable.TotalReach = -1;
                dataMasterTable.TotalSumBreakdown = dataMasterTable.Girls + dataMasterTable.Boys + dataMasterTable.Women + dataMasterTable.Men + dataMasterTable.ElderlyWomen + dataMasterTable.ElderlyMen;
                dataMasterTable.Total_Women = -1;  ///// ?????????
                dataMasterTable.Total_Men = -1;   /////  ?????????
                dataMasterTable.GirlsNew = dataPartnerContributionsData[i].GirlsNew;
                dataMasterTable.BoysNew = dataPartnerContributionsData[i].BoysNew;
                dataMasterTable.WomenNew = dataPartnerContributionsData[i].WomenNew;
                dataMasterTable.MenNew = dataPartnerContributionsData[i].MenNew;
                dataMasterTable.ElderlyWomenNew = dataPartnerContributionsData[i].ElderlyWomenNew;
                dataMasterTable.ElderlyMenNew = dataPartnerContributionsData[i].ElderlyMenNew;
                dataMasterTable.NewTotalSumBreakdownNew = dataMasterTable.GirlsNew + dataMasterTable.BoysNew + dataMasterTable.WomenNew + dataMasterTable.MenNew + dataMasterTable.ElderlyWomenNew + dataMasterTable.ElderlyMenNew;
                dataMasterTable.Total_New_Women = -1;    ///// ?????????
                dataMasterTable.Total_New_Men = -1;      ///// ?????????
                dataMasterTable.Admin1Pcode = dataPartnerContributionsData[i].Admin1Code;
                dataMasterTable.Admin2Pcode = dataPartnerContributionsData[i].Admin2Code;
                dataMasterTable.Admin3Pcode = dataPartnerContributionsData[i].Admin3Code;
                dataMasterTable.Admin4Pcode2 = dataPartnerContributionsData[i].Admin4Code;
                dataMasterTable.NeibourhoodPcode2 = dataPartnerContributionsData[i].LocationCode;
                dataMasterTable.Code_Camps = "???";
                dataMasterTable.SubSectorID = (from a in _code4WSActivity where a.SubSectorEn.Trim().ToUpper() == dataPartnerContributionsData[i].FWSSubSector.Trim().ToUpper() select a.SubSectorID).FirstOrDefault();
                dataMasterTable.ActivityID = (from a in _code4WSActivity where a.ActivityEn.Trim().ToUpper() == dataPartnerContributionsData[i].FWSActivity.Trim().ToUpper() select a.ActivityID).FirstOrDefault();
                dataMasterTable.SubActivityID = (from a in _code4WSActivity where a.SubActivityOrg.Trim().ToUpper() == dataPartnerContributionsData[i].FWSSubActivity.Trim().ToUpper() select a.SubActivityID).FirstOrDefault();
                dataMasterTable.FWSAnalysisUnit = dataPartnerContributionsData[i].FWSAnalysisUnit;
                dataMasterTable.PWDActivity = true;//AMER Calculate
                dataMasterTable.ImplementingPartner_ALL =
                 dataPartnerContributionsData[i].ImplementingPartnerGUID.HasValue ?
                _code4WSPartner.Where(x => x.PartnerGUID == dataPartnerContributionsData[i].ImplementingPartnerGUID.Value).Select(x => x.PartnerDescription).FirstOrDefault() :
                _code4WSPartner.Where(x => x.PartnerGUID == dataPartnerContributionsData[i].AgencyGUID).Select(x => x.PartnerDescription).FirstOrDefault();
                dataMasterTable.Organization = _code4WSHub.Where(x => x.HubGUID == dataPartnerContributionsData[i].HubGUID).Select(x => x.HubDescription).FirstOrDefault() + "_" + _code4WSPartner.Where(x => x.PartnerGUID == dataPartnerContributionsData[i].AgencyGUID).Select(x => x.PartnerDescription).FirstOrDefault();
                dataMasterTable.IPs = _code4WSHub.Where(x => x.HubGUID == dataPartnerContributionsData[i].HubGUID).Select(x => x.HubDescription).FirstOrDefault() + "_" + dataMasterTable.ImplementingPartner_ALL;

                dataMasterTable.Organization_Type = _code4WSOrgTypeByHub.Where(x => x.Hub_Org_IP.Trim().ToUpper() == dataMasterTable.Organization.Trim().ToUpper()).Select(x => x.OrgType).FirstOrDefault();
                if (dataMasterTable.Organization_Type == null) { dataMasterTable.Organization_Type = "unidentified"; };
                dataMasterTable.ImplementingPartner_Type = _code4WSOrgTypeByHub.Where(x => x.Hub_Org_IP.Trim().ToUpper() == dataMasterTable.IPs.Trim().ToUpper()).Select(x => x.OrgType).FirstOrDefault();
                if (dataMasterTable.ImplementingPartner_Type == null) { dataMasterTable.ImplementingPartner_Type = "unidentified"; };
                var NeiLoc = (from a in _code4WSLocation where a.admin3Pcode == dataPartnerContributionsData[i].Admin3Code && a.NeibourhoodRefName == dataMasterTable.NeibourhoodRefName select a.NeibourhoodPcode2).FirstOrDefault();
                string AccessStatus = (from a in _code4WSAccessStatus where a.LocationCode == NeiLoc select a.HTR_BS_status).FirstOrDefault();
                if (AccessStatus == null) { AccessStatus = "Accessible"; }
                dataMasterTable.Access_Status = AccessStatus;
                string DeliveryModality = (from a in _code4WSActivity where a.SubActivityID == dataMasterTable.SubActivityID select a.DeliveryModality).FirstOrDefault();
                if (DeliveryModality == null) { AccessStatus = "?"; }
                dataMasterTable.Delivery_Modality = DeliveryModality;
                dataMasterTable.Severity_Ranking = (from a in _code4WSSeverityRankings where a.Sd_pcode.Trim().ToUpper() == dataPartnerContributionsData[i].Admin3Code.Trim().ToUpper() select a.FinalSR).FirstOrDefault();
                string CrossAoRCategory = (from a in _code4WSActivity where a.SubActivityID == dataMasterTable.SubActivityID select a.CrossCuttingActivity).FirstOrDefault();
                if (CrossAoRCategory == null) { CrossAoRCategory = "?"; }
                dataMasterTable.Cross_AoR_Category = CrossAoRCategory;
                dataMasterTable.HRP_Indicator = (from a in _code4WSActivity where a.SubActivity.Trim().ToUpper() == dataPartnerContributionsData[i].ActivityLong.Trim().ToUpper() select a.HRPIndicator).FirstOrDefault();

                string DirectAddition = (from a in _code4WSActivity where a.SubActivityID == dataMasterTable.SubActivityID select a.DirectAddition).FirstOrDefault();
                if (DirectAddition == null) { DirectAddition = "?"; }
                dataMasterTable.Direct_Addition = DirectAddition;


                dataMasterTable.Total_Cumulative_Interventions = CalculateTotalCumulativeInterventions(dataMasterTable);


                dataMasterTable.Girls_Cumu = CalculateGirlsCummu(dataMasterTable);
                dataMasterTable.Boys_Cumu = CalculateBoysCummu(dataMasterTable);
                dataMasterTable.Women_Cumu = CalculateWomenCummu(dataMasterTable);
                dataMasterTable.Men_Cumu = CalculateMenCummu(dataMasterTable);
                dataMasterTable.Elderly_Women_Cumu = -1;
                dataMasterTable.Elderly_Men_Cumu = -1;
                dataMasterTable.Total_Women_Cumu = -1;
                dataMasterTable.Total_Men_Cumu = -1;
                dataMasterTable.Active = true;
                MasterRecords.Add(dataMasterTable);
            }


            DbFWS.BulkInsert(MasterRecords);




            try
            {
                DbFWS.BulkSaveChanges();
                DbFWS.SaveChanges();
                JsonReturn jr = new JsonReturn()
                {
                    AffectedRecordsGuids = PartnerContributionGUIDs,
                    DataTable = DataTableNames.PartnersContributionsDataTable,
                    Notify = new Notify { Type = MessageTypes.Success, Message = "Selected Records Approved Successfully" }
                };

                return Json(jr);
            }
            catch (Exception ex)
            {
                return Json(DbFWS.ErrorMessage(ex.Message));
            }
        }

        public int? CalculateTotalCumulativeInterventions(dataMasterTable dataMasterTable)
        {
            int? result = null;
            if (dataMasterTable.FWSAnalysisUnit.Trim().ToUpper() != "# people".Trim().ToUpper())
            {
                result = dataMasterTable.TotalReach;
            }
            else
            {
                if (dataMasterTable.HRP_Indicator == "5.1.1" || dataMasterTable.HRP_Indicator == "5.2.1")
                {
                    result = dataMasterTable.Girls + dataMasterTable.Boys + dataMasterTable.Women + dataMasterTable.Men;
                }
                else
                {
                    if (dataMasterTable.HRP_Indicator == "5.1.2" || dataMasterTable.HRP_Indicator == "5.3.1")
                    {
                        result = dataMasterTable.ElderlyWomen + dataMasterTable.ElderlyMen;
                    }
                    else
                    {
                        if (dataMasterTable.ReportingMonth == 1)
                        {
                            result = dataMasterTable.Girls + dataMasterTable.Boys + dataMasterTable.Women + dataMasterTable.Men + dataMasterTable.ElderlyWomen + dataMasterTable.ElderlyMen;
                        }
                        else
                        {
                            if (dataMasterTable.HRP_Indicator == "3.1.3" || dataMasterTable.HRP_Indicator == "3.2.1" || dataMasterTable.HRP_Indicator == "3.3.1")
                            {
                                result = dataMasterTable.GirlsNew + dataMasterTable.BoysNew + dataMasterTable.WomenNew + dataMasterTable.MenNew + dataMasterTable.ElderlyWomenNew + dataMasterTable.ElderlyMenNew;
                            }
                            else
                            {
                                result = dataMasterTable.Girls + dataMasterTable.Boys + dataMasterTable.Women + dataMasterTable.Men + dataMasterTable.ElderlyWomen + dataMasterTable.ElderlyMen;
                            }
                        }
                    }
                }
            }

            return result;


        }

        public int? CalculateGirlsCummu(dataMasterTable dataMasterTable)
        {
            int? result = null;
            if (dataMasterTable.HRP_Indicator == "5.1.2" || dataMasterTable.HRP_Indicator == "5.3.1")
            {
                result = 0;
            }
            else
            {
                if (dataMasterTable.ReportingMonth == 1)
                {
                    result = dataMasterTable.Girls + dataMasterTable.Women;
                }
                else
                {
                    if (dataMasterTable.HRP_Indicator == "3.1.3" || dataMasterTable.HRP_Indicator == "3.2.1" || dataMasterTable.HRP_Indicator == "3.3.1")
                    {
                        result = dataMasterTable.GirlsNew + dataMasterTable.WomenNew;
                    }
                    else
                    {
                        result = dataMasterTable.Girls;
                    }
                }
            }

            return result;
        }

        public int? CalculateBoysCummu(dataMasterTable dataMasterTable)
        {
            int? result = null;
            if (dataMasterTable.HRP_Indicator == "5.1.2" || dataMasterTable.HRP_Indicator == "5.3.1")
            {
                result = 0;
            }
            else
            {
                if (dataMasterTable.ReportingMonth == 1)
                {
                    result = dataMasterTable.Boys + dataMasterTable.Men;
                }
                else
                {
                    if (dataMasterTable.HRP_Indicator == "3.1.3" || dataMasterTable.HRP_Indicator == "3.2.1" || dataMasterTable.HRP_Indicator == "3.3.1")
                    {
                        result = dataMasterTable.BoysNew + dataMasterTable.MenNew;
                    }
                    else
                    {
                        result = dataMasterTable.Boys;
                    }
                }
            }

            return result;
        }

        public int? CalculateWomenCummu(dataMasterTable dataMasterTable)
        {
            int? result = null;
            if (dataMasterTable.HRP_Indicator == "5.1.1" || dataMasterTable.HRP_Indicator == "5.2.1")
            {
                result = 0;
            }
            else
            {
                if (dataMasterTable.ReportingMonth == 1)
                {
                    result = dataMasterTable.ElderlyWomen;
                }
                else
                {
                    if (dataMasterTable.HRP_Indicator == "3.1.3" || dataMasterTable.HRP_Indicator == "3.2.1" || dataMasterTable.HRP_Indicator == "3.3.1")
                    {
                        result = dataMasterTable.ElderlyWomenNew;
                    }
                    else
                    {
                        result = dataMasterTable.ElderlyWomen;
                    }
                }
            }
            return result;
        }

        public int? CalculateMenCummu(dataMasterTable dataMasterTable)
        {
            int? result = null;
            if (dataMasterTable.HRP_Indicator == "5.1.1" || dataMasterTable.HRP_Indicator == "5.2.1")
            {
                result = 0;
            }
            else
            {
                if (dataMasterTable.ReportingMonth == 1)
                {
                    result = dataMasterTable.ElderlyMen;
                }
                else
                {
                    if (dataMasterTable.HRP_Indicator == "3.1.3" || dataMasterTable.HRP_Indicator == "3.2.1" || dataMasterTable.HRP_Indicator == "3.3.1")
                    {
                        result = dataMasterTable.ElderlyMenNew;
                    }
                    else
                    {
                        result = dataMasterTable.ElderlyMen;
                    }
                }
            }
            return result;
        }
    }


}