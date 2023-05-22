using PCR_DAL.Model;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AppsPortal.PCR.ViewModels;

namespace AppsPortal.Areas.PCR.Controllers
{
    public class PartnerCentersController : PCRBaseController
    {
        #region Partner Center

        //public ActionResult Index()
        //{
        //    return View();
        //}

        [Route("PCR/PartnerCenters/")]
        public ActionResult PartnerCentersIndex()
        {
            if (!CMS.HasAction(Permissions.CommunityCenterCode.Access, Apps.PCR))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/PCR/Views/PartnerCenters/Index.cshtml");
        }

        [Route("PCR/PartnerCentersDataTable/")]
        public JsonResult PartnerCentersDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<PartnerCentersDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<PartnerCentersDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.CommunityCenterCode.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();

            var All = (from a in DbPCR.codePartnerCenter.AsExpandable().Where(x => AuthorizedList.Contains(x.OrganizationInstanceGUID + "," + x.DutyStationGUID.ToString()))
                       join b in DbPCR.codePartnerCenterLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codePartnerCenter.DeletedOn) && x.LanguageID == LAN) on a.PartnerCenterGUID equals b.PartnerCenterGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join c in DbPCR.codeDutyStationsLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeDutyStations.DeletedOn) && x.LanguageID == LAN) on a.DutyStationGUID equals c.DutyStationGUID into LJ2
                       from R2 in LJ2.DefaultIfEmpty()
                       join d in DbPCR.codeOrganizationsInstancesLanguages.Where(x => (x.Active) && x.LanguageID == LAN) on a.OrganizationInstanceGUID equals d.OrganizationInstanceGUID into LJ3
                       from R3 in LJ3.DefaultIfEmpty()
                       join e in DbPCR.codeOrganizations on R3.codeOrganizationsInstances.OrganizationGUID equals e.OrganizationGUID
                       select new PartnerCentersDataTableModel
                       {
                           PartnerCenterGUID = a.PartnerCenterGUID,
                           DutyStationDescription = R2.DutyStationDescription,
                           OrganizationInstanceDescription = e.OrganizationShortName,
                           DutyStationGUID = R2.DutyStationGUID.ToString(),
                           OrganizationInstanceGUID = R3.OrganizationInstanceGUID.ToString(),
                           PartnerCenterDescription = R1.PartnerCenterDescription,
                           Sequence = a.Sequence,
                           Active = a.Active,
                           codePartnerCenterRowVersion = a.codePartnerCenterRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<PartnerCentersDataTableModel> Result = Mapper.Map<List<PartnerCentersDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("PCR/PartnerCenters/Create/")]
        public ActionResult PartnerCenterCreate()
        {
            if (!CMS.HasAction(Permissions.CommunityCenterCode.Create, Apps.PCR))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/PCR/Views/PartnerCenters/PartnerCenter.cshtml", new PartnerCenterUpdateModel());
        }

        [Route("PCR/PartnerCenters/Update/{PK}")]
        public ActionResult PartnerCenterUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.CommunityCenterCode.Access, Apps.PCR))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbPCR.codePartnerCenter.WherePK(PK)
                         join b in DbPCR.codePartnerCenterLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codePartnerCenter.DeletedOn) && x.LanguageID == LAN) on a.PartnerCenterGUID equals b.PartnerCenterGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new PartnerCenterUpdateModel
                         {
                             PartnerCenterGUID = a.PartnerCenterGUID,
                             DutyStationGUID = a.DutyStationGUID,
                             OrganizationInstanceGUID = a.OrganizationInstanceGUID,
                             PartnerCenterDescription = R1.PartnerCenterDescription,
                             Sequence = a.Sequence,
                             Active = a.Active,
                             codePartnerCenterRowVersion = a.codePartnerCenterRowVersion,
                             codePartnerCenterLanguageRowVersion = R1.codePartnerCenterLanguageRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("PartnerCenter", "PartnerCenters", new { Area = "PCR" }));

            return View("~/Areas/PCR/Views/PartnerCenters/PartnerCenter.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PartnerCenterCreate(PartnerCenterUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.CommunityCenterCode.Create, Apps.PCR, model.OrganizationInstanceGUID + "," + model.DutyStationGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActivePartnerCenter(model)) return PartialView("~/Areas/PCR/Views/PartnerCenters/_PartnerCenterForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codePartnerCenter PartnerCenter = Mapper.Map(model, new codePartnerCenter());
            PartnerCenter.PartnerCenterGUID = EntityPK;
            DbPCR.Create(PartnerCenter, Permissions.CommunityCenterCode.CreateGuid, ExecutionTime, DbCMS);

            codePartnerCenterLanguage Language = Mapper.Map(model, new codePartnerCenterLanguage());
            Language.PartnerCenterGUID = EntityPK;

            DbPCR.Create(Language, Permissions.CommunityCenterCode.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.PartnerCenterLanguagesDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.CommunityCenterCode.Create, Apps.PCR, new UrlHelper(Request.RequestContext).Action("Create", "PartnerCenters", new { Area = "PCR" })), Container = "PartnerCenterFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.CommunityCenterCode.Update, Apps.PCR), Container = "PartnerCenterFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.CommunityCenterCode.Delete, Apps.PCR), Container = "PartnerCenterFormControls" });

            try
            {
                DbPCR.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPCR.SingleCreateMessage(DbPCR.PrimaryKeyControl(PartnerCenter), DbPCR.RowVersionControls(PartnerCenter, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbPCR.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PartnerCenterUpdate(PartnerCenterUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.CommunityCenterCode.Update, Apps.PCR, model.OrganizationInstanceGUID + "," + model.DutyStationGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActivePartnerCenter(model)) return PartialView("~/Areas/PCR/Views/PartnerCenters/_PartnerCenterForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codePartnerCenter PartnerCenter = Mapper.Map(model, new codePartnerCenter());
            DbPCR.Update(PartnerCenter, Permissions.CommunityCenterCode.UpdateGuid, ExecutionTime, DbCMS);

            var Language = DbPCR.codePartnerCenterLanguage.Where(l => l.PartnerCenterGUID == model.PartnerCenterGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.PartnerCenterGUID = PartnerCenter.PartnerCenterGUID;
                DbPCR.Create(Language, Permissions.CommunityCenterCode.CreateGuid, ExecutionTime, DbCMS);
            }
            else if (Language.PartnerCenterDescription != model.PartnerCenterDescription)
            {
                Language = Mapper.Map(model, Language);
                DbPCR.Update(Language, Permissions.CommunityCenterCode.UpdateGuid, ExecutionTime, DbCMS);
            }

            try
            {
                DbPCR.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPCR.SingleUpdateMessage(null, null, DbPCR.RowVersionControls(PartnerCenter, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyPartnerCenter(model.PartnerCenterGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPCR.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PartnerCenterDelete(codePartnerCenter model)
        {
            if (!CMS.HasAction(Permissions.CommunityCenterCode.Delete, Apps.PCR, model.OrganizationInstanceGUID + "," + model.DutyStationGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codePartnerCenter> DeletedPartnerCenter = DeletePartnerCenters(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.CommunityCenterCode.Restore, Apps.PCR), Container = "PartnerCenterFormControls" });

            try
            {
                int CommitedRows = DbPCR.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPCR.SingleDeleteMessage(CommitedRows, DeletedPartnerCenter.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyPartnerCenter(model.PartnerCenterGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPCR.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PartnerCenterRestore(codePartnerCenter model)
        {
            if (!CMS.HasAction(Permissions.CommunityCenterCode.Restore, Apps.PCR, model.OrganizationInstanceGUID + "," + model.DutyStationGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActivePartnerCenter(model))
            {
                return Json(DbPCR.RecordExists());
            }

            List<codePartnerCenter> RestoredPartnerCenters = RestorePartnerCenters(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.CommunityCenterCode.Create, Apps.PCR, new UrlHelper(Request.RequestContext).Action("PartnerCenterCreate", "Configuration", new { Area = "PCR" })), Container = "PartnerCenterFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.CommunityCenterCode.Update, Apps.PCR), Container = "PartnerCenterFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.CommunityCenterCode.Delete, Apps.PCR), Container = "PartnerCenterFormControls" });

            try
            {
                int CommitedRows = DbPCR.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPCR.SingleRestoreMessage(CommitedRows, RestoredPartnerCenters, DbPCR.PrimaryKeyControl(RestoredPartnerCenters.FirstOrDefault()), Url.Action(DataTableNames.PartnerCenterLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyPartnerCenter(model.PartnerCenterGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPCR.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult PartnerCentersDataTableDelete(List<codePartnerCenter> models)
        {
            if (!CMS.HasAction(Permissions.CommunityCenterCode.Delete, Apps.PCR))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codePartnerCenter> DeletedPartnerCenters = DeletePartnerCenters(models);

            try
            {
                DbPCR.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPCR.PartialDeleteMessage(DeletedPartnerCenters, models, DataTableNames.PartnerCentersDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPCR.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult PartnerCentersDataTableRestore(List<codePartnerCenter> models)
        {
            if (!CMS.HasAction(Permissions.CommunityCenterCode.Restore, Apps.PCR))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codePartnerCenter> RestoredPartnerCenters = RestorePartnerCenters(models);

            try
            {
                DbPCR.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPCR.PartialRestoreMessage(RestoredPartnerCenters, models, DataTableNames.PartnerCentersDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPCR.ErrorMessage(ex.Message));
            }
        }

        private List<codePartnerCenter> DeletePartnerCenters(List<codePartnerCenter> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<codePartnerCenter> DeletedPartnerCenters = new List<codePartnerCenter>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";//"SELECT PartnerCenterGUID,CONVERT(varchar(50), DepartmentGUID) as C2 ,codePartnerCenterRowVersion FROM code.codePartnerCenter where PartnerCenterGUID in (" + string.Join(",", models.Select(x => "'" + x.PartnerCenterGUID + "'").ToArray()) + ")";

            string query = DbPCR.QueryBuilder(models, Permissions.CommunityCenterCode.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbPCR.Database.SqlQuery<codePartnerCenter>(query).ToList();
            foreach (var record in Records)
            {
                DeletedPartnerCenters.Add(DbPCR.Delete(record, ExecutionTime, Permissions.CommunityCenterCode.DeleteGuid, DbCMS));
            }

            var Languages = DeletedPartnerCenters.SelectMany(a => a.codePartnerCenterLanguage).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbPCR.Delete(language, ExecutionTime, Permissions.CommunityCenterCode.DeleteGuid, DbCMS);
            }
            return DeletedPartnerCenters;
        }

        private List<codePartnerCenter> RestorePartnerCenters(List<codePartnerCenter> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<codePartnerCenter> RestoredPartnerCenters = new List<codePartnerCenter>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";//"SELECT PartnerCenterGUID,CONVERT(varchar(50), DepartmentGUID) as C2 ,codePartnerCenterRowVersion FROM code.codePartnerCenter where PartnerCenterGUID in (" + string.Join(",", models.Select(x => "'" + x.PartnerCenterGUID + "'").ToArray()) + ")";

            string query = DbPCR.QueryBuilder(models, Permissions.CommunityCenterCode.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbPCR.Database.SqlQuery<codePartnerCenter>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActivePartnerCenter(record))
                {
                    RestoredPartnerCenters.Add(DbPCR.Restore(record, Permissions.CommunityCenterCode.DeleteGuid, Permissions.CommunityCenterCode.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var Languages = RestoredPartnerCenters.SelectMany(x => x.codePartnerCenterLanguage.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbPCR.Restore(language, Permissions.CommunityCenterCode.DeleteGuid, Permissions.CommunityCenterCode.RestoreGuid, RestoringTime, DbCMS);
            }

            return RestoredPartnerCenters;
        }

        private JsonResult ConcurrencyPartnerCenter(Guid PK)
        {
            PartnerCenterUpdateModel dbModel = new PartnerCenterUpdateModel();

            var PartnerCenter = DbPCR.codePartnerCenter.Where(x => x.PartnerCenterGUID == PK).FirstOrDefault();
            var dbPartnerCenter = DbPCR.Entry(PartnerCenter).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbPartnerCenter, dbModel);

            var Language = DbPCR.codePartnerCenterLanguage.Where(x => x.PartnerCenterGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codePartnerCenter.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbPCR.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (PartnerCenter.codePartnerCenterRowVersion.SequenceEqual(dbModel.codePartnerCenterRowVersion) && Language.codePartnerCenterLanguageRowVersion.SequenceEqual(dbModel.codePartnerCenterLanguageRowVersion))
            {
                return Json(DbPCR.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbPCR, dbModel, "LanguagesContainer"));
        }

        private bool ActivePartnerCenter(Object model)
        {
            codePartnerCenterLanguage PartnerCenter = Mapper.Map(model, new codePartnerCenterLanguage());
            int PartnerCenterDescription = DbPCR.codePartnerCenterLanguage
                                    .Where(x => x.PartnerCenterDescription == PartnerCenter.PartnerCenterDescription &&
                                                x.PartnerCenterGUID != PartnerCenter.PartnerCenterGUID &&
                                                x.Active).Count();
            if (PartnerCenterDescription > 0)
            {
                ModelState.AddModelError("PartnerCenterDescription", "PartnerCenter is already exists");
            }
            return (PartnerCenterDescription > 0);
        }

        #endregion

        #region Partner Center Language

        //[Route("PCR/PartnerCenterLanguagesDataTable/{PK}")]
        public ActionResult PartnerCenterLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/PCR/Views/PartnerCenters/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codePartnerCenterLanguage, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codePartnerCenterLanguage>(DataTable.Filters);
            }

            var Result = DbPCR.codePartnerCenterLanguage.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.PartnerCenterGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.PartnerCenterLanguageGUID,
                                  x.LanguageID,
                                  x.PartnerCenterDescription,
                                  x.codePartnerCenterLanguageRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult PartnerCenterLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.CommunityCenterCode.Create, Apps.PCR))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/PCR/Views/PartnerCenters/_LanguageUpdateModal.cshtml",
                new codePartnerCenterLanguage { PartnerCenterGUID = FK });
        }

        public ActionResult PartnerCenterLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.CommunityCenterCode.Access, Apps.PCR))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/PCR/Views/PartnerCenters/_LanguageUpdateModal.cshtml", DbPCR.codePartnerCenterLanguage.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PartnerCenterLanguageCreate(codePartnerCenterLanguage model)
        {
            if (!CMS.HasAction(Permissions.CommunityCenterCode.Create, Apps.PCR))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActivePartnerCenterLanguage(model)) return PartialView("~/Areas/PCR/Views/PartnerCenters/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbPCR.Create(model, Permissions.CommunityCenterCode.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbPCR.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPCR.SingleUpdateMessage(DataTableNames.PartnerCenterLanguagesDataTable, DbPCR.PrimaryKeyControl(model), DbPCR.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbPCR.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PartnerCenterLanguageUpdate(codePartnerCenterLanguage model)
        {
            if (!CMS.HasAction(Permissions.CommunityCenterCode.Update, Apps.PCR))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActivePartnerCenterLanguage(model)) return PartialView("~/Areas/PCR/Views/PartnerCenters/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbPCR.Update(model, Permissions.CommunityCenterCode.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbPCR.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPCR.SingleUpdateMessage(DataTableNames.PartnerCenterLanguagesDataTable,
                    DbPCR.PrimaryKeyControl(model),
                    DbPCR.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyPartnerCenterLanguage(model.PartnerCenterLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPCR.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PartnerCenterLanguageDelete(codePartnerCenterLanguage model)
        {
            if (!CMS.HasAction(Permissions.CommunityCenterCode.Delete, Apps.PCR))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codePartnerCenterLanguage> DeletedLanguages = DeletePartnerCenterLanguages(new List<codePartnerCenterLanguage> { model });

            try
            {
                DbPCR.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPCR.SingleDeleteMessage(DeletedLanguages, DataTableNames.PartnerCenterLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyPartnerCenterLanguage(model.PartnerCenterLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPCR.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PartnerCenterLanguageRestore(codePartnerCenterLanguage model)
        {
            if (!CMS.HasAction(Permissions.CommunityCenterCode.Restore, Apps.PCR))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActivePartnerCenterLanguage(model))
            {
                return Json(DbPCR.RecordExists());
            }

            List<codePartnerCenterLanguage> RestoredLanguages = RestorePartnerCenterLanguages(Portal.SingleToList(model));

            try
            {
                DbPCR.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPCR.SingleRestoreMessage(RestoredLanguages, DataTableNames.PartnerCenterLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyPartnerCenterLanguage(model.PartnerCenterLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPCR.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult PartnerCenterLanguagesDataTableDelete(List<codePartnerCenterLanguage> models)
        {
            if (!CMS.HasAction(Permissions.CommunityCenterCode.Delete, Apps.PCR))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codePartnerCenterLanguage> DeletedLanguages = DeletePartnerCenterLanguages(models);

            try
            {
                DbPCR.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPCR.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.PartnerCenterLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPCR.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult PartnerCenterLanguagesDataTableRestore(List<codePartnerCenterLanguage> models)
        {
            if (!CMS.HasAction(Permissions.CommunityCenterCode.Restore, Apps.PCR))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codePartnerCenterLanguage> RestoredLanguages = RestorePartnerCenterLanguages(models);

            try
            {
                DbPCR.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPCR.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.PartnerCenterLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPCR.ErrorMessage(ex.Message));
            }
        }

        private List<codePartnerCenterLanguage> DeletePartnerCenterLanguages(List<codePartnerCenterLanguage> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codePartnerCenterLanguage> DeletedPartnerCenterLanguages = new List<codePartnerCenterLanguage>();

            string query = DbPCR.QueryBuilder(models, Permissions.CommunityCenterCode.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbPCR.Database.SqlQuery<codePartnerCenterLanguage>(query).ToList();

            foreach (var language in languages)
            {
                DeletedPartnerCenterLanguages.Add(DbPCR.Delete(language, ExecutionTime, Permissions.CommunityCenterCode.DeleteGuid, DbCMS));
            }

            return DeletedPartnerCenterLanguages;
        }

        private List<codePartnerCenterLanguage> RestorePartnerCenterLanguages(List<codePartnerCenterLanguage> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<codePartnerCenterLanguage> RestoredLanguages = new List<codePartnerCenterLanguage>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbPCR.QueryBuilder(models, Permissions.CommunityCenterCode.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbPCR.Database.SqlQuery<codePartnerCenterLanguage>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActivePartnerCenterLanguage(language))
                {
                    RestoredLanguages.Add(DbPCR.Restore(language, Permissions.CommunityCenterCode.DeleteGuid, Permissions.CommunityCenterCode.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyPartnerCenterLanguage(Guid PK)
        {
            codePartnerCenterLanguage dbModel = new codePartnerCenterLanguage();

            var Language = DbPCR.codePartnerCenterLanguage.Where(l => l.PartnerCenterLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbPCR.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codePartnerCenterLanguageRowVersion.SequenceEqual(dbModel.codePartnerCenterLanguageRowVersion))
            {
                return Json(DbPCR.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbPCR, dbModel, "LanguagesContainer"));
        }

        private bool ActivePartnerCenterLanguage(codePartnerCenterLanguage model)
        {
            int LanguageID = DbPCR.codePartnerCenterLanguage
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.PartnerCenterGUID == model.PartnerCenterGUID &&
                                              x.PartnerCenterLanguageGUID != model.PartnerCenterLanguageGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Telecom Company Name in selected language already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }

        #endregion
    }
}