using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using EMT_DAL.Model;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.EMT.Controllers
{
    public class MedicalPharmacysController : EMTBaseController
    {
        #region Medical Pharmacys

        public ActionResult Index()
        {
            return View();
        }

        [Route("EMT/MedicalPharmacys/")]
        public ActionResult MedicalPharmacysIndex()
        {
            if (!CMS.HasAction(Permissions.MedicalPharmacy.Access, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/EMT/Views/MedicalPharmacys/Index.cshtml");
        }

        [Route("EMT/MedicalPharmacysDataTable/")]
        public JsonResult MedicalPharmacysDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MedicalPharmacysDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MedicalPharmacysDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.MedicalPharmacy.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var All = (from a in DbEMT.codeMedicalPharmacy.AsExpandable()/*.Where(x=> AuthorizedList.Contains(x.MedicalPharmacyGUID.ToString()))*/
                       join b in DbEMT.codeMedicalPharmacyLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeMedicalPharmacy.DeletedOn) && x.LanguageID == LAN) on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join c in DbEMT.codeOrganizationsInstancesLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeOrganizationsInstances.DeletedOn) && x.LanguageID == LAN) on a.OrganizationInstanceGUID equals c.OrganizationInstanceGUID into LJ2
                       from R2 in LJ2.DefaultIfEmpty()
                       select new MedicalPharmacysDataTableModel
                       {
                          
                           MedicalPharmacyGUID = a.MedicalPharmacyGUID,
                           MedicalPharmacyDescription = R1.MedicalPharmacyDescription,
                           OrganizationInstanceDescription = R2.OrganizationInstanceDescription,
                           Active = a.Active,
                           codeMedicalPharmacyRowVersion = a.codeMedicalPharmacyRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<MedicalPharmacysDataTableModel> Result = Mapper.Map<List<MedicalPharmacysDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("EMT/MedicalPharmacys/Create/")]
        public ActionResult MedicalPharmacyCreate()
        {
            if (!CMS.HasAction(Permissions.MedicalPharmacy.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/EMT/Views/MedicalPharmacys/MedicalPharmacy.cshtml", new MedicalPharmacyUpdateModel());
        }

        [Route("EMT/MedicalPharmacys/Update/{PK}")]
        public ActionResult MedicalPharmacyUpdate(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.MedicalPharmacy.Access, Apps.EMT))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var model = (from a in DbEMT.codeMedicalPharmacy.WherePK(PK)
                         join b in DbEMT.codeMedicalPharmacyLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeMedicalPharmacy.DeletedOn) && x.LanguageID == LAN)
                         on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new MedicalPharmacyUpdateModel
                         {
                             MedicalPharmacyGUID = a.MedicalPharmacyGUID,
                             MainWarehouse=a.MainWarehouse,
                             OrganizationInstanceGUID = a.OrganizationInstanceGUID,
                             Sort = a.Sort,
                             MedicalPharmacyDescription = R1.MedicalPharmacyDescription,
                             Active = a.Active,
                             codeMedicalPharmacyRowVersion = a.codeMedicalPharmacyRowVersion,
                             codeMedicalPharmacyLanguageRowVersion = R1.codeMedicalPharmacyLanguageRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("MedicalPharmacy", "MedicalPharmacys", new { Area = "EMT" }));

            return View("~/Areas/EMT/Views/MedicalPharmacys/MedicalPharmacy.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalPharmacyCreate(MedicalPharmacyUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.MedicalPharmacy.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalPharmacy(model)) return PartialView("~/Areas/EMT/Views/MedicalPharmacys/_MedicalPharmacyForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeMedicalPharmacy MedicalPharmacy = Mapper.Map(model, new codeMedicalPharmacy());
            MedicalPharmacy.MedicalPharmacyGUID = EntityPK;
            DbEMT.Create(MedicalPharmacy, Permissions.MedicalPharmacy.CreateGuid, ExecutionTime, DbCMS);

            codeMedicalPharmacyLanguage Language = Mapper.Map(model, new codeMedicalPharmacyLanguage());
            Language.MedicalPharmacyGUID = EntityPK;

            DbEMT.Create(Language, Permissions.MedicalPharmacy.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.MedicalPharmacyLanguagesDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.MedicalPharmacy.Create, Apps.EMT, new UrlHelper(Request.RequestContext).Action("Create", "MedicalPharmacys", new { Area = "EMT" })), Container = "MedicalPharmacyFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.MedicalPharmacy.Update, Apps.EMT), Container = "MedicalPharmacyFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.MedicalPharmacy.Delete, Apps.EMT), Container = "MedicalPharmacyFormControls" });

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleCreateMessage(DbEMT.PrimaryKeyControl(MedicalPharmacy), DbEMT.RowVersionControls(MedicalPharmacy, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalPharmacyUpdate(MedicalPharmacyUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.MedicalPharmacy.Update, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalPharmacy(model)) return PartialView("~/Areas/EMT/Views/MedicalPharmacys/_MedicalPharmacyForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeMedicalPharmacy MedicalPharmacy = Mapper.Map(model, new codeMedicalPharmacy());
            DbEMT.Update(MedicalPharmacy, Permissions.MedicalPharmacy.UpdateGuid, ExecutionTime, DbCMS);

            var Language = DbEMT.codeMedicalPharmacyLanguage.Where(l => l.MedicalPharmacyGUID == model.MedicalPharmacyGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.MedicalPharmacyGUID = MedicalPharmacy.MedicalPharmacyGUID;
                DbEMT.Create(Language, Permissions.MedicalPharmacy.CreateGuid, ExecutionTime, DbCMS);
            }
            else if (Language.MedicalPharmacyDescription != model.MedicalPharmacyDescription)
            {
                Language = Mapper.Map(model, Language);
                DbEMT.Update(Language, Permissions.MedicalPharmacy.UpdateGuid, ExecutionTime, DbCMS);
            }

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(null, null, DbEMT.RowVersionControls(MedicalPharmacy, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMedicalPharmacy(model.MedicalPharmacyGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalPharmacyDelete(codeMedicalPharmacy model)
        {
            if (!CMS.HasAction(Permissions.MedicalPharmacy.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeMedicalPharmacy> DeletedMedicalPharmacy = DeleteMedicalPharmacys(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.MedicalPharmacy.Restore, Apps.EMT), Container = "MedicalPharmacyFormControls" });

            try
            {
                int CommitedRows = DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleDeleteMessage(CommitedRows, DeletedMedicalPharmacy.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMedicalPharmacy(model.MedicalPharmacyGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalPharmacyRestore(codeMedicalPharmacy model)
        {
            if (!CMS.HasAction(Permissions.MedicalPharmacy.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveMedicalPharmacy(model))
            {
                return Json(DbEMT.RecordExists());
            }

            List<codeMedicalPharmacy> RestoredMedicalPharmacys = RestoreMedicalPharmacys(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.MedicalPharmacy.Create, Apps.EMT, new UrlHelper(Request.RequestContext).Action("MedicalPharmacyCreate", "Configuration", new { Area = "EMT" })), Container = "MedicalPharmacyFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.MedicalPharmacy.Update, Apps.EMT), Container = "MedicalPharmacyFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.MedicalPharmacy.Delete, Apps.EMT), Container = "MedicalPharmacyFormControls" });

            try
            {
                int CommitedRows = DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleRestoreMessage(CommitedRows, RestoredMedicalPharmacys, DbEMT.PrimaryKeyControl(RestoredMedicalPharmacys.FirstOrDefault()), Url.Action(DataTableNames.MedicalPharmacyLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMedicalPharmacy(model.MedicalPharmacyGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalPharmacysDataTableDelete(List<codeMedicalPharmacy> models)
        {
            if (!CMS.HasAction(Permissions.MedicalPharmacy.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeMedicalPharmacy> DeletedMedicalPharmacys = DeleteMedicalPharmacys(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialDeleteMessage(DeletedMedicalPharmacys, models, DataTableNames.MedicalPharmacysDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalPharmacysDataTableRestore(List<codeMedicalPharmacy> models)
        {
            if (!CMS.HasAction(Permissions.MedicalPharmacy.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeMedicalPharmacy> RestoredMedicalPharmacys = RestoreMedicalPharmacys(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialRestoreMessage(RestoredMedicalPharmacys, models, DataTableNames.MedicalPharmacysDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        private List<codeMedicalPharmacy> DeleteMedicalPharmacys(List<codeMedicalPharmacy> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<codeMedicalPharmacy> DeletedMedicalPharmacys = new List<codeMedicalPharmacy>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalPharmacy.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbEMT.Database.SqlQuery<codeMedicalPharmacy>(query).ToList();
            foreach (var record in Records)
            {
                DeletedMedicalPharmacys.Add(DbEMT.Delete(record, ExecutionTime, Permissions.MedicalPharmacy.DeleteGuid, DbCMS));
            }

            var Languages = DeletedMedicalPharmacys.SelectMany(a => a.codeMedicalPharmacyLanguage).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbEMT.Delete(language, ExecutionTime, Permissions.MedicalPharmacy.DeleteGuid, DbCMS);
            }
            return DeletedMedicalPharmacys;
        }

        private List<codeMedicalPharmacy> RestoreMedicalPharmacys(List<codeMedicalPharmacy> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<codeMedicalPharmacy> RestoredMedicalPharmacys = new List<codeMedicalPharmacy>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalPharmacy.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbEMT.Database.SqlQuery<codeMedicalPharmacy>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveMedicalPharmacy(record))
                {
                    RestoredMedicalPharmacys.Add(DbEMT.Restore(record, Permissions.MedicalPharmacy.DeleteGuid, Permissions.MedicalPharmacy.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var Languages = RestoredMedicalPharmacys.SelectMany(x => x.codeMedicalPharmacyLanguage.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbEMT.Restore(language, Permissions.MedicalPharmacy.DeleteGuid, Permissions.MedicalPharmacy.RestoreGuid, RestoringTime, DbCMS);
            }

            return RestoredMedicalPharmacys;
        }

        private JsonResult ConcurrencyMedicalPharmacy(Guid PK)
        {
            MedicalPharmacyUpdateModel dbModel = new MedicalPharmacyUpdateModel();

            var MedicalPharmacy = DbEMT.codeMedicalPharmacy.Where(x => x.MedicalPharmacyGUID == PK).FirstOrDefault();
            var dbMedicalPharmacy = DbEMT.Entry(MedicalPharmacy).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbMedicalPharmacy, dbModel);

            var Language = DbEMT.codeMedicalPharmacyLanguage.Where(x => x.MedicalPharmacyGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeMedicalPharmacy.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbEMT.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (MedicalPharmacy.codeMedicalPharmacyRowVersion.SequenceEqual(dbModel.codeMedicalPharmacyRowVersion) && Language.codeMedicalPharmacyLanguageRowVersion.SequenceEqual(dbModel.codeMedicalPharmacyLanguageRowVersion))
            {
                return Json(DbEMT.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbEMT, dbModel, "LanguagesContainer"));
        }

        private bool ActiveMedicalPharmacy(Object model)
        {
            codeMedicalPharmacyLanguage MedicalPharmacy = Mapper.Map(model, new codeMedicalPharmacyLanguage());
            int MedicalPharmacyDescription = DbEMT.codeMedicalPharmacyLanguage
                                    .Where(x => x.MedicalPharmacyDescription == MedicalPharmacy.MedicalPharmacyDescription &&
                                                x.MedicalPharmacyGUID != MedicalPharmacy.MedicalPharmacyGUID &&
                                                x.Active).Count();
            if (MedicalPharmacyDescription > 0)
            {
                ModelState.AddModelError("MedicalPharmacyDescription", "MedicalPharmacy is already exists");
            }
            return (MedicalPharmacyDescription > 0);
        }

        #endregion

        #region Medical Pharmacys Language

        //[Route("EMT/MedicalPharmacyLanguagesDataTable/{PK}")]
        public ActionResult MedicalPharmacyLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/EMT/Views/MedicalPharmacys/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeMedicalPharmacyLanguage, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeMedicalPharmacyLanguage>(DataTable.Filters);
            }

            var Result = DbEMT.codeMedicalPharmacyLanguage.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.MedicalPharmacyGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.MedicalPharmacyLanguageGUID,
                                  x.LanguageID,
                                  x.MedicalPharmacyDescription,
                                  x.codeMedicalPharmacyLanguageRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult MedicalPharmacyLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.MedicalPharmacy.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/EMT/Views/MedicalPharmacys/_LanguageUpdateModal.cshtml",
                new codeMedicalPharmacyLanguage { MedicalPharmacyGUID = FK });
        }

        public ActionResult MedicalPharmacyLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.MedicalPharmacy.Access, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/EMT/Views/MedicalPharmacys/_LanguageUpdateModal.cshtml", DbEMT.codeMedicalPharmacyLanguage.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalPharmacyLanguageCreate(codeMedicalPharmacyLanguage model)
        {
            if (!CMS.HasAction(Permissions.MedicalPharmacy.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalPharmacyLanguage(model)) return PartialView("~/Areas/EMT/Views/MedicalPharmacys/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbEMT.Create(model, Permissions.MedicalPharmacy.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(DataTableNames.MedicalPharmacyLanguagesDataTable, DbEMT.PrimaryKeyControl(model), DbEMT.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalPharmacyLanguageUpdate(codeMedicalPharmacyLanguage model)
        {
            if (!CMS.HasAction(Permissions.MedicalPharmacy.Update, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalPharmacyLanguage(model)) return PartialView("~/Areas/EMT/Views/MedicalPharmacys/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbEMT.Update(model, Permissions.MedicalPharmacy.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(DataTableNames.MedicalPharmacyLanguagesDataTable,
                    DbEMT.PrimaryKeyControl(model),
                    DbEMT.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalPharmacyLanguage(model.MedicalPharmacyLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalPharmacyLanguageDelete(codeMedicalPharmacyLanguage model)
        {
            if (!CMS.HasAction(Permissions.MedicalPharmacy.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeMedicalPharmacyLanguage> DeletedLanguages = DeleteMedicalPharmacyLanguages(new List<codeMedicalPharmacyLanguage> { model });

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleDeleteMessage(DeletedLanguages, DataTableNames.MedicalPharmacyLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalPharmacyLanguage(model.MedicalPharmacyLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalPharmacyLanguageRestore(codeMedicalPharmacyLanguage model)
        {
            if (!CMS.HasAction(Permissions.MedicalPharmacy.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveMedicalPharmacyLanguage(model))
            {
                return Json(DbEMT.RecordExists());
            }

            List<codeMedicalPharmacyLanguage> RestoredLanguages = RestoreMedicalPharmacyLanguages(Portal.SingleToList(model));

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleRestoreMessage(RestoredLanguages, DataTableNames.MedicalPharmacyLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalPharmacyLanguage(model.MedicalPharmacyLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalPharmacyLanguagesDataTableDelete(List<codeMedicalPharmacyLanguage> models)
        {
            if (!CMS.HasAction(Permissions.MedicalPharmacy.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeMedicalPharmacyLanguage> DeletedLanguages = DeleteMedicalPharmacyLanguages(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.MedicalPharmacyLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalPharmacyLanguagesDataTableRestore(List<codeMedicalPharmacyLanguage> models)
        {
            if (!CMS.HasAction(Permissions.MedicalPharmacy.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeMedicalPharmacyLanguage> RestoredLanguages = RestoreMedicalPharmacyLanguages(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.MedicalPharmacyLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        private List<codeMedicalPharmacyLanguage> DeleteMedicalPharmacyLanguages(List<codeMedicalPharmacyLanguage> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeMedicalPharmacyLanguage> DeletedMedicalPharmacyLanguages = new List<codeMedicalPharmacyLanguage>();

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalPharmacy.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbEMT.Database.SqlQuery<codeMedicalPharmacyLanguage>(query).ToList();

            foreach (var language in languages)
            {
                DeletedMedicalPharmacyLanguages.Add(DbEMT.Delete(language, ExecutionTime, Permissions.MedicalPharmacy.DeleteGuid, DbCMS));
            }

            return DeletedMedicalPharmacyLanguages;
        }

        private List<codeMedicalPharmacyLanguage> RestoreMedicalPharmacyLanguages(List<codeMedicalPharmacyLanguage> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<codeMedicalPharmacyLanguage> RestoredLanguages = new List<codeMedicalPharmacyLanguage>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalPharmacy.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbEMT.Database.SqlQuery<codeMedicalPharmacyLanguage>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveMedicalPharmacyLanguage(language))
                {
                    RestoredLanguages.Add(DbEMT.Restore(language, Permissions.MedicalPharmacy.DeleteGuid, Permissions.MedicalPharmacy.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyMedicalPharmacyLanguage(Guid PK)
        {
            codeMedicalPharmacyLanguage dbModel = new codeMedicalPharmacyLanguage();

            var Language = DbEMT.codeMedicalPharmacyLanguage.Where(l => l.MedicalPharmacyLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbEMT.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeMedicalPharmacyLanguageRowVersion.SequenceEqual(dbModel.codeMedicalPharmacyLanguageRowVersion))
            {
                return Json(DbEMT.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbEMT, dbModel, "LanguagesContainer"));
        }

        private bool ActiveMedicalPharmacyLanguage(codeMedicalPharmacyLanguage model)
        {
            int LanguageID = DbEMT.codeMedicalPharmacyLanguage
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.MedicalPharmacyGUID == model.MedicalPharmacyGUID &&
                                              x.MedicalPharmacyLanguageGUID != model.MedicalPharmacyLanguageGUID &&
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