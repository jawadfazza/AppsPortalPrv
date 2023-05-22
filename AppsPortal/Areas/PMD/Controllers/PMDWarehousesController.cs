using PMD_DAL.Model;
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

namespace AppsPortal.Areas.PMD.Controllers
{
    public class PMDWarehousesController : PMDBaseController
    {
        #region Appointment Types

        public ActionResult Index()
        {
            return View();
        }

        [Route("PMD/PMDWarehouses/")]
        public ActionResult PMDWarehousesIndex()
        {
            if (!CMS.HasAction(Permissions.PMDWarehouse.Access, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/PMD/Views/PMDWarehouses/Index.cshtml");
        }

        [Route("PMD/PMDWarehousesDataTable/")]
        public JsonResult PMDWarehousesDataTable(DataTableRecievedOptions options)
        {
            var app = DbPMD.codePmdWarehouse.ToList();
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<PMDWarehousesDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<PMDWarehousesDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
           // List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.PMDWarehouse.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();


            var All = (from a in DbPMD.codePmdWarehouse.AsExpandable()//.Where(x => AuthorizedList.Contains(x.ImplementingPartnerGUID.ToString()+","+ x.GovernorateGUID.ToString()))
                       join b in DbPMD.codePmdWarehouseLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codePmdWarehouse.DeletedOn) && x.LanguageID == LAN) on a.PmdWarehouseGUID equals b.PmdWarehouseGUID 
                       join c in DbPMD.codeOchaLocationGovernorate on a.GovernorateGUID equals c.GovernorateGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join d in DbPMD.codeOrganizationsInstancesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.ImplementingPartnerGUID equals d.OrganizationInstanceGUID into LJ2
                       from R2 in LJ2.DefaultIfEmpty()

                       select new PMDWarehousesDataTableModel
                       {
                           PmdWarehouseGUID = a.PmdWarehouseGUID,   
                           GovernorateGUID=a.GovernorateGUID.ToString(),
                           ImplementingPartnerGUID=a.ImplementingPartnerGUID.ToString(),
                           admin1Pcode=R1.admin1Name_en,
                           ImplementingPartner=R2.OrganizationInstanceDescription,
                           PMDWarehouseDescription=b.PMDWarehouseDescription,
                           codePmdWarehouseRowVersion=a.codePmdWarehouseRowVersion,

                           Active = a.Active,
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<PMDWarehousesDataTableModel> Result = Mapper.Map<List<PMDWarehousesDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("PMD/PMDWarehouses/Create/")]
        public ActionResult PMDWarehouseCreate()
        {
            if (!CMS.HasAction(Permissions.PMDWarehouse.Create, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/PMD/Views/PMDWarehouses/PMDWarehouse.cshtml", new PMDWarehouseUpdateModel());
        }

        [Route("PMD/PMDWarehouses/Update/{PK}")]
        public ActionResult PMDWarehouseUpdate(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.PMDWarehouse.Access, Apps.PMD))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var model = (from a in DbPMD.codePmdWarehouse.WherePK(PK)
                         join b in DbPMD.codePmdWarehouseLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codePmdWarehouse.DeletedOn) && x.LanguageID == LAN)
                         on a.PmdWarehouseGUID equals b.PmdWarehouseGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new PMDWarehouseUpdateModel
                         {
                             PmdWarehouseGUID = a.PmdWarehouseGUID,
                             GovernorateGUID=a.GovernorateGUID,
                             ImplementingPartnerGUID=a.ImplementingPartnerGUID,
                             PMDWarehouseDescription = R1.PMDWarehouseDescription,
                             Active = a.Active,
                             codePmdWarehouseRowVersion = a.codePmdWarehouseRowVersion,
                             admin1Pcode=a.admin1Pcode,
                             admin4Pcode=a.admin4Pcode
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("PMDWarehouse", "PMDWarehouses", new { Area = "PMD" }));

            return View("~/Areas/PMD/Views/PMDWarehouses/PMDWarehouse.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PMDWarehouseCreate(PMDWarehouseUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.PMDWarehouse.Create, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActivePMDWarehouse(model)) return PartialView("~/Areas/PMD/Views/PMDWarehouses/_PMDWarehouseForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();
            model.GovernorateGUID = DbPMD.codeOchaLocationGovernorate.Where(x => x.admin1Pcode == model.admin1Pcode).FirstOrDefault().GovernorateGUID.Value;

            codePmdWarehouse PMDWarehouse = Mapper.Map(model, new codePmdWarehouse());
            PMDWarehouse.PmdWarehouseGUID = EntityPK;
            DbPMD.Create(PMDWarehouse, Permissions.PMDWarehouse.CreateGuid, ExecutionTime, DbCMS);

            codePmdWarehouseLanguage Language = Mapper.Map(model, new codePmdWarehouseLanguage());
            Language.PmdWarehouseGUID = EntityPK;

            DbPMD.Create(Language, Permissions.PMDWarehouse.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.PMDWarehouseLanguagesDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.PMDWarehouse.Create, Apps.PMD, new UrlHelper(Request.RequestContext).Action("Create", "PMDWarehouses", new { Area = "PMD" })), Container = "PMDWarehouseFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.PMDWarehouse.Update, Apps.PMD), Container = "PMDWarehouseFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.PMDWarehouse.Delete, Apps.PMD), Container = "PMDWarehouseFormControls" });

            try
            {
                DbPMD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPMD.SingleCreateMessage(DbPMD.PrimaryKeyControl(PMDWarehouse), DbPMD.RowVersionControls(PMDWarehouse, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PMDWarehouseUpdate(PMDWarehouseUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.PMDWarehouse.Update, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActivePMDWarehouse(model)) return PartialView("~/Areas/PMD/Views/PMDWarehouses/_PMDWarehouseForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            model.GovernorateGUID = DbPMD.codeOchaLocationGovernorate.Where(x => x.admin1Pcode == model.admin1Pcode).FirstOrDefault().GovernorateGUID.Value;

            codePmdWarehouse PMDWarehouse = Mapper.Map(model, new codePmdWarehouse());
            DbPMD.Update(PMDWarehouse, Permissions.PMDWarehouse.UpdateGuid, ExecutionTime, DbCMS);

            var Language = DbPMD.codePmdWarehouseLanguage.Where(l => l.PmdWarehouseGUID == model.PmdWarehouseGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.PmdWarehouseGUID = PMDWarehouse.PmdWarehouseGUID;
                DbPMD.Create(Language, Permissions.PMDWarehouse.CreateGuid, ExecutionTime, DbCMS);
            }
            else if (Language.PMDWarehouseDescription != model.PMDWarehouseDescription)
            {
                Language = Mapper.Map(model, Language);
                DbPMD.Update(Language, Permissions.PMDWarehouse.UpdateGuid, ExecutionTime, DbCMS);
            }

            try
            {
                DbPMD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPMD.SingleUpdateMessage(null, null, DbPMD.RowVersionControls(PMDWarehouse, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyPMDWarehouse(model.PmdWarehouseGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PMDWarehouseDelete(codePmdWarehouse model)
        {
            if (!CMS.HasAction(Permissions.PMDWarehouse.Delete, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codePmdWarehouse> DeletedPMDWarehouse = DeletePMDWarehouses(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.PMDWarehouse.Restore, Apps.PMD), Container = "PMDWarehouseFormControls" });

            try
            {
                int CommitedRows = DbPMD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPMD.SingleDeleteMessage(CommitedRows, DeletedPMDWarehouse.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyPMDWarehouse(model.PmdWarehouseGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PMDWarehouseRestore(codePmdWarehouse model)
        {
            if (!CMS.HasAction(Permissions.PMDWarehouse.Restore, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActivePMDWarehouse(model))
            {
                return Json(DbPMD.RecordExists());
            }

            List<codePmdWarehouse> RestoredPMDWarehouses = RestorePMDWarehouses(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.PMDWarehouse.Create, Apps.PMD, new UrlHelper(Request.RequestContext).Action("PMDWarehouseCreate", "Configuration", new { Area = "PMD" })), Container = "PMDWarehouseFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.PMDWarehouse.Update, Apps.PMD), Container = "PMDWarehouseFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.PMDWarehouse.Delete, Apps.PMD), Container = "PMDWarehouseFormControls" });

            try
            {
                int CommitedRows = DbPMD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPMD.SingleRestoreMessage(CommitedRows, RestoredPMDWarehouses, DbPMD.PrimaryKeyControl(RestoredPMDWarehouses.FirstOrDefault()), Url.Action(DataTableNames.PMDWarehouseLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyPMDWarehouse(model.PmdWarehouseGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult PMDWarehousesDataTableDelete(List<codePmdWarehouse> models)
        {
            if (!CMS.HasAction(Permissions.PMDWarehouse.Delete, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codePmdWarehouse> DeletedPMDWarehouses = DeletePMDWarehouses(models);

            try
            {
                DbPMD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPMD.PartialDeleteMessage(DeletedPMDWarehouses, models, DataTableNames.PMDWarehousesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult PMDWarehousesDataTableRestore(List<codePmdWarehouse> models)
        {
            if (!CMS.HasAction(Permissions.PMDWarehouse.Restore, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codePmdWarehouse> RestoredPMDWarehouses = RestorePMDWarehouses(models);

            try
            {
                DbPMD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPMD.PartialRestoreMessage(RestoredPMDWarehouses, models, DataTableNames.PMDWarehousesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        private List<codePmdWarehouse> DeletePMDWarehouses(List<codePmdWarehouse> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<codePmdWarehouse> DeletedPMDWarehouses = new List<codePmdWarehouse>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbPMD.QueryBuilder(models, Permissions.PMDWarehouse.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbPMD.Database.SqlQuery<codePmdWarehouse>(query).ToList();
            foreach (var record in Records)
            {
                DeletedPMDWarehouses.Add(DbPMD.Delete(record, ExecutionTime, Permissions.PMDWarehouse.DeleteGuid, DbCMS));
            }

            var Languages = DeletedPMDWarehouses.SelectMany(a => a.codePmdWarehouseLanguage).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbPMD.Delete(language, ExecutionTime, Permissions.PMDWarehouse.DeleteGuid, DbCMS);
            }
            return DeletedPMDWarehouses;
        }

        private List<codePmdWarehouse> RestorePMDWarehouses(List<codePmdWarehouse> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<codePmdWarehouse> RestoredPMDWarehouses = new List<codePmdWarehouse>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbPMD.QueryBuilder(models, Permissions.PMDWarehouse.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbPMD.Database.SqlQuery<codePmdWarehouse>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActivePMDWarehouse(record))
                {
                    RestoredPMDWarehouses.Add(DbPMD.Restore(record, Permissions.PMDWarehouse.DeleteGuid, Permissions.PMDWarehouse.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var Languages = RestoredPMDWarehouses.SelectMany(x => x.codePmdWarehouseLanguage.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbPMD.Restore(language, Permissions.PMDWarehouse.DeleteGuid, Permissions.PMDWarehouse.RestoreGuid, RestoringTime, DbCMS);
            }

            return RestoredPMDWarehouses;
        }

        private JsonResult ConcurrencyPMDWarehouse(Guid PK)
        {
            PMDWarehouseUpdateModel dbModel = new PMDWarehouseUpdateModel();

            var PMDWarehouse = DbPMD.codePmdWarehouse.Where(x => x.PmdWarehouseGUID == PK).FirstOrDefault();
            var dbPMDWarehouse = DbPMD.Entry(PMDWarehouse).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbPMDWarehouse, dbModel);

            var Language = DbPMD.codePmdWarehouseLanguage.Where(x => x.PmdWarehouseGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codePmdWarehouse.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbPMD.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (PMDWarehouse.codePmdWarehouseRowVersion.SequenceEqual(dbModel.codePmdWarehouseRowVersion) && Language.codePmdWarehouseLanguageRowVersion.SequenceEqual(dbModel.codePmdWarehouseRowVersion))
            {
                return Json(DbPMD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbPMD, dbModel, "LanguagesContainer"));
        }

        private bool ActivePMDWarehouse(Object model)
        {
            codePmdWarehouseLanguage PMDWarehouse = Mapper.Map(model, new codePmdWarehouseLanguage());
            int PMDWarehouseDescription = DbPMD.codePmdWarehouseLanguage
                                    .Where(x => x.PMDWarehouseDescription == PMDWarehouse.PMDWarehouseDescription &&
                                                x.PmdWarehouseGUID != PMDWarehouse.PmdWarehouseGUID &&
                                                x.Active).Count();
            if (PMDWarehouseDescription > 0)
            {
                ModelState.AddModelError("PMDWarehouseDescription", "PMDWarehouse is already exists");
            }
            return (PMDWarehouseDescription > 0);
        }

        #endregion

        #region Appointment Types Language

        //[Route("PMD/PMDWarehouseLanguagesDataTable/{PK}")]
        public ActionResult PMDWarehouseLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/PMD/Views/PMDWarehouses/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codePmdWarehouseLanguage, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codePmdWarehouseLanguage>(DataTable.Filters);
            }

            var Result = DbPMD.codePmdWarehouseLanguage.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.PmdWarehouseGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.PmdWarehouseLanguageGUID,
                                  x.LanguageID,
                                  x.PMDWarehouseDescription,
                                  x.codePmdWarehouseLanguageRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult PMDWarehouseLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.PMDWarehouse.Create, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/PMD/Views/PMDWarehouses/_LanguageUpdateModal.cshtml",
                new codePmdWarehouseLanguage { PmdWarehouseGUID = FK });
        }

        public ActionResult PMDWarehouseLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.PMDWarehouse.Access, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/PMD/Views/PMDWarehouses/_LanguageUpdateModal.cshtml", DbPMD.codePmdWarehouseLanguage.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PMDWarehouseLanguageCreate(codePmdWarehouseLanguage model)
        {
            if (!CMS.HasAction(Permissions.PMDWarehouse.Create, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActivePMDWarehouseLanguage(model)) return PartialView("~/Areas/PMD/Views/PMDWarehouses/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbPMD.Create(model, Permissions.PMDWarehouse.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbPMD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPMD.SingleUpdateMessage(DataTableNames.PMDWarehouseLanguagesDataTable, DbPMD.PrimaryKeyControl(model), DbPMD.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PMDWarehouseLanguageUpdate(codePmdWarehouseLanguage model)
        {
            if (!CMS.HasAction(Permissions.PMDWarehouse.Update, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActivePMDWarehouseLanguage(model)) return PartialView("~/Areas/PMD/Views/PMDWarehouses/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbPMD.Update(model, Permissions.PMDWarehouse.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbPMD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPMD.SingleUpdateMessage(DataTableNames.PMDWarehouseLanguagesDataTable,
                    DbPMD.PrimaryKeyControl(model),
                    DbPMD.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyPMDWarehouseLanguage(model.PmdWarehouseLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PMDWarehouseLanguageDelete(codePmdWarehouseLanguage model)
        {
            if (!CMS.HasAction(Permissions.PMDWarehouse.Delete, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codePmdWarehouseLanguage> DeletedLanguages = DeletePMDWarehouseLanguages(new List<codePmdWarehouseLanguage> { model });

            try
            {
                DbPMD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPMD.SingleDeleteMessage(DeletedLanguages, DataTableNames.PMDWarehouseLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyPMDWarehouseLanguage(model.PmdWarehouseLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PMDWarehouseLanguageRestore(codePmdWarehouseLanguage model)
        {
            if (!CMS.HasAction(Permissions.PMDWarehouse.Restore, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActivePMDWarehouseLanguage(model))
            {
                return Json(DbPMD.RecordExists());
            }

            List<codePmdWarehouseLanguage> RestoredLanguages = RestorePMDWarehouseLanguages(Portal.SingleToList(model));

            try
            {
                DbPMD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPMD.SingleRestoreMessage(RestoredLanguages, DataTableNames.PMDWarehouseLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyPMDWarehouseLanguage(model.PmdWarehouseLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult PMDWarehouseLanguagesDataTableDelete(List<codePmdWarehouseLanguage> models)
        {
            if (!CMS.HasAction(Permissions.PMDWarehouse.Delete, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codePmdWarehouseLanguage> DeletedLanguages = DeletePMDWarehouseLanguages(models);

            try
            {
                DbPMD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPMD.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.PMDWarehouseLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult PMDWarehouseLanguagesDataTableRestore(List<codePmdWarehouseLanguage> models)
        {
            if (!CMS.HasAction(Permissions.PMDWarehouse.Restore, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codePmdWarehouseLanguage> RestoredLanguages = RestorePMDWarehouseLanguages(models);

            try
            {
                DbPMD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPMD.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.PMDWarehouseLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        private List<codePmdWarehouseLanguage> DeletePMDWarehouseLanguages(List<codePmdWarehouseLanguage> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codePmdWarehouseLanguage> DeletedPMDWarehouseLanguages = new List<codePmdWarehouseLanguage>();

            string query = DbPMD.QueryBuilder(models, Permissions.PMDWarehouse.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbPMD.Database.SqlQuery<codePmdWarehouseLanguage>(query).ToList();

            foreach (var language in languages)
            {
                DeletedPMDWarehouseLanguages.Add(DbPMD.Delete(language, ExecutionTime, Permissions.PMDWarehouse.DeleteGuid, DbCMS));
            }

            return DeletedPMDWarehouseLanguages;
        }

        private List<codePmdWarehouseLanguage> RestorePMDWarehouseLanguages(List<codePmdWarehouseLanguage> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<codePmdWarehouseLanguage> RestoredLanguages = new List<codePmdWarehouseLanguage>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbPMD.QueryBuilder(models, Permissions.PMDWarehouse.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbPMD.Database.SqlQuery<codePmdWarehouseLanguage>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActivePMDWarehouseLanguage(language))
                {
                    RestoredLanguages.Add(DbPMD.Restore(language, Permissions.PMDWarehouse.DeleteGuid, Permissions.PMDWarehouse.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyPMDWarehouseLanguage(Guid PK)
        {
            codePmdWarehouseLanguage dbModel = new codePmdWarehouseLanguage();

            var Language = DbPMD.codePmdWarehouseLanguage.Where(l => l.PmdWarehouseLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbPMD.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codePmdWarehouseLanguageRowVersion.SequenceEqual(dbModel.codePmdWarehouseLanguageRowVersion))
            {
                return Json(DbPMD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbPMD, dbModel, "LanguagesContainer"));
        }

        private bool ActivePMDWarehouseLanguage(codePmdWarehouseLanguage model)
        {
            int LanguageID = DbPMD.codePmdWarehouseLanguage
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.PmdWarehouseGUID == model.PmdWarehouseGUID &&
                                              x.PmdWarehouseLanguageGUID != model.PmdWarehouseLanguageGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Warehouse Name in selected language already exists"); 
            }

            return (LanguageID > 0);
        }

        #endregion
    }
}