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
    public class MedicalManufacturersController : EMTBaseController
    {
        #region Medical Manufacturers

        public ActionResult Index()
        {
            return View();
        }

        [Route("EMT/MedicalManufacturers/")]
        public ActionResult MedicalManufacturersIndex()
        {
            if (!CMS.HasAction(Permissions.MedicalManufacturer.Access, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/EMT/Views/MedicalManufacturers/Index.cshtml");
        }

        [Route("EMT/MedicalManufacturersDataTable/")]
        public JsonResult MedicalManufacturersDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MedicalManufacturersDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MedicalManufacturersDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.MedicalManufacturer.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();


            var All = (from a in DbEMT.codeMedicalManufacturer.AsExpandable()
                       join b in DbEMT.codeMedicalManufacturerLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeMedicalManufacturer.DeletedOn) && x.LanguageID == LAN) on a.MedicalManufacturerGUID equals b.MedicalManufacturerGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       select new MedicalManufacturersDataTableModel
                       {
                          
                           MedicalManufacturerGUID = a.MedicalManufacturerGUID,
                           MedicalManufacturerDescription = R1.MedicalManufacturerDescription,
                           Sort=a.Sort,
                           Active = a.Active,
                           codeMedicalManufacturerRowVersion = a.codeMedicalManufacturerRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<MedicalManufacturersDataTableModel> Result = Mapper.Map<List<MedicalManufacturersDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("EMT/MedicalManufacturers/Create/")]
        public ActionResult MedicalManufacturerCreate()
        {
            if (!CMS.HasAction(Permissions.MedicalManufacturer.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/EMT/Views/MedicalManufacturers/MedicalManufacturer.cshtml", new MedicalManufacturerUpdateModel());
        }

        [Route("EMT/MedicalManufacturers/Update/{PK}")]
        public ActionResult MedicalManufacturerUpdate(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.MedicalManufacturer.Access, Apps.EMT))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var model = (from a in DbEMT.codeMedicalManufacturer.WherePK(PK)
                         join b in DbEMT.codeMedicalManufacturerLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeMedicalManufacturer.DeletedOn) && x.LanguageID == LAN)
                         on a.MedicalManufacturerGUID equals b.MedicalManufacturerGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new MedicalManufacturerUpdateModel
                         {
                             MedicalManufacturerGUID = a.MedicalManufacturerGUID,
                             Sort = a.Sort,
                             MedicalManufacturerDescription = R1.MedicalManufacturerDescription,
                             Active = a.Active,
                             codeMedicalManufacturerRowVersion = a.codeMedicalManufacturerRowVersion,
                             codeMedicalManufacturerLanguageRowVersion = R1.codeMedicalManufacturerLanguageRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("MedicalManufacturer", "MedicalManufacturers", new { Area = "EMT" }));

            return View("~/Areas/EMT/Views/MedicalManufacturers/MedicalManufacturer.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalManufacturerCreate(MedicalManufacturerUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.MedicalManufacturer.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalManufacturer(model)) return PartialView("~/Areas/EMT/Views/MedicalManufacturers/_MedicalManufacturerForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeMedicalManufacturer MedicalManufacturer = Mapper.Map(model, new codeMedicalManufacturer());
            MedicalManufacturer.MedicalManufacturerGUID = EntityPK;
            DbEMT.Create(MedicalManufacturer, Permissions.MedicalManufacturer.CreateGuid, ExecutionTime, DbCMS);

            codeMedicalManufacturerLanguage Language = Mapper.Map(model, new codeMedicalManufacturerLanguage());
            Language.MedicalManufacturerGUID = EntityPK;

            DbEMT.Create(Language, Permissions.MedicalManufacturer.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.MedicalManufacturerLanguagesDataTable, ControllerContext, "LanguagesContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.MedicalManufacturer.Create, Apps.EMT, new UrlHelper(Request.RequestContext).Action("Create", "MedicalManufacturers", new { Area = "EMT" })), Container = "MedicalManufacturerFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.MedicalManufacturer.Update, Apps.EMT), Container = "MedicalManufacturerFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.MedicalManufacturer.Delete, Apps.EMT), Container = "MedicalManufacturerFormControls" });

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleCreateMessage(DbEMT.PrimaryKeyControl(MedicalManufacturer), DbEMT.RowVersionControls(MedicalManufacturer, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalManufacturerUpdate(MedicalManufacturerUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.MedicalManufacturer.Update, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalManufacturer(model)) return PartialView("~/Areas/EMT/Views/MedicalManufacturers/_MedicalManufacturerForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeMedicalManufacturer MedicalManufacturer = Mapper.Map(model, new codeMedicalManufacturer());
            DbEMT.Update(MedicalManufacturer, Permissions.MedicalManufacturer.UpdateGuid, ExecutionTime, DbCMS);

            var Language = DbEMT.codeMedicalManufacturerLanguage.Where(l => l.MedicalManufacturerGUID == model.MedicalManufacturerGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.MedicalManufacturerGUID = MedicalManufacturer.MedicalManufacturerGUID;
                DbEMT.Create(Language, Permissions.MedicalManufacturer.CreateGuid, ExecutionTime, DbCMS);
            }
            else if (Language.MedicalManufacturerDescription != model.MedicalManufacturerDescription)
            {
                Language = Mapper.Map(model, Language);
                DbEMT.Update(Language, Permissions.MedicalManufacturer.UpdateGuid, ExecutionTime, DbCMS);
            }

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(null, null, DbEMT.RowVersionControls(MedicalManufacturer, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMedicalManufacturer(model.MedicalManufacturerGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalManufacturerDelete(codeMedicalManufacturer model)
        {
            if (!CMS.HasAction(Permissions.MedicalManufacturer.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeMedicalManufacturer> DeletedMedicalManufacturer = DeleteMedicalManufacturers(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.MedicalManufacturer.Restore, Apps.EMT), Container = "MedicalManufacturerFormControls" });

            try
            {
                int CommitedRows = DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleDeleteMessage(CommitedRows, DeletedMedicalManufacturer.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMedicalManufacturer(model.MedicalManufacturerGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalManufacturerRestore(codeMedicalManufacturer model)
        {
            if (!CMS.HasAction(Permissions.MedicalManufacturer.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveMedicalManufacturer(model))
            {
                return Json(DbEMT.RecordExists());
            }

            List<codeMedicalManufacturer> RestoredMedicalManufacturers = RestoreMedicalManufacturers(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.MedicalManufacturer.Create, Apps.EMT, new UrlHelper(Request.RequestContext).Action("MedicalManufacturerCreate", "Configuration", new { Area = "EMT" })), Container = "MedicalManufacturerFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.MedicalManufacturer.Update, Apps.EMT), Container = "MedicalManufacturerFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.MedicalManufacturer.Delete, Apps.EMT), Container = "MedicalManufacturerFormControls" });

            try
            {
                int CommitedRows = DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleRestoreMessage(CommitedRows, RestoredMedicalManufacturers, DbEMT.PrimaryKeyControl(RestoredMedicalManufacturers.FirstOrDefault()), Url.Action(DataTableNames.MedicalManufacturerLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMedicalManufacturer(model.MedicalManufacturerGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalManufacturersDataTableDelete(List<codeMedicalManufacturer> models)
        {
            if (!CMS.HasAction(Permissions.MedicalManufacturer.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeMedicalManufacturer> DeletedMedicalManufacturers = DeleteMedicalManufacturers(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialDeleteMessage(DeletedMedicalManufacturers, models, DataTableNames.MedicalManufacturersDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalManufacturersDataTableRestore(List<codeMedicalManufacturer> models)
        {
            if (!CMS.HasAction(Permissions.MedicalManufacturer.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeMedicalManufacturer> RestoredMedicalManufacturers = RestoreMedicalManufacturers(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialRestoreMessage(RestoredMedicalManufacturers, models, DataTableNames.MedicalManufacturersDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        private List<codeMedicalManufacturer> DeleteMedicalManufacturers(List<codeMedicalManufacturer> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<codeMedicalManufacturer> DeletedMedicalManufacturers = new List<codeMedicalManufacturer>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "SELECT MedicalManufacturerGUID,CONVERT(varchar(50), DepartmentGUID) as C2 ,codeMedicalManufacturerRowVersion FROM code.codeMedicalManufacturer where MedicalManufacturerGUID in (" + string.Join(",", models.Select(x => "'" + x.MedicalManufacturerGUID + "'").ToArray()) + ")";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalManufacturer.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbEMT.Database.SqlQuery<codeMedicalManufacturer>(query).ToList();
            foreach (var record in Records)
            {
                DeletedMedicalManufacturers.Add(DbEMT.Delete(record, ExecutionTime, Permissions.MedicalManufacturer.DeleteGuid, DbCMS));
            }

            var Languages = DeletedMedicalManufacturers.SelectMany(a => a.codeMedicalManufacturerLanguage).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbEMT.Delete(language, ExecutionTime, Permissions.MedicalManufacturer.DeleteGuid, DbCMS);
            }
            return DeletedMedicalManufacturers;
        }

        private List<codeMedicalManufacturer> RestoreMedicalManufacturers(List<codeMedicalManufacturer> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<codeMedicalManufacturer> RestoredMedicalManufacturers = new List<codeMedicalManufacturer>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "SELECT MedicalManufacturerGUID,CONVERT(varchar(50), DepartmentGUID) as C2 ,codeMedicalManufacturerRowVersion FROM code.codeMedicalManufacturer where MedicalManufacturerGUID in (" + string.Join(",", models.Select(x => "'" + x.MedicalManufacturerGUID + "'").ToArray()) + ")";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalManufacturer.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbEMT.Database.SqlQuery<codeMedicalManufacturer>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveMedicalManufacturer(record))
                {
                    RestoredMedicalManufacturers.Add(DbEMT.Restore(record, Permissions.MedicalManufacturer.DeleteGuid, Permissions.MedicalManufacturer.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var Languages = RestoredMedicalManufacturers.SelectMany(x => x.codeMedicalManufacturerLanguage.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbEMT.Restore(language, Permissions.MedicalManufacturer.DeleteGuid, Permissions.MedicalManufacturer.RestoreGuid, RestoringTime, DbCMS);
            }

            return RestoredMedicalManufacturers;
        }

        private JsonResult ConcurrencyMedicalManufacturer(Guid PK)
        {
            MedicalManufacturerUpdateModel dbModel = new MedicalManufacturerUpdateModel();

            var MedicalManufacturer = DbEMT.codeMedicalManufacturer.Where(x => x.MedicalManufacturerGUID == PK).FirstOrDefault();
            var dbMedicalManufacturer = DbEMT.Entry(MedicalManufacturer).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbMedicalManufacturer, dbModel);

            var Language = DbEMT.codeMedicalManufacturerLanguage.Where(x => x.MedicalManufacturerGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeMedicalManufacturer.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbEMT.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (MedicalManufacturer.codeMedicalManufacturerRowVersion.SequenceEqual(dbModel.codeMedicalManufacturerRowVersion) && Language.codeMedicalManufacturerLanguageRowVersion.SequenceEqual(dbModel.codeMedicalManufacturerLanguageRowVersion))
            {
                return Json(DbEMT.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbEMT, dbModel, "LanguagesContainer"));
        }

        private bool ActiveMedicalManufacturer(Object model)
        {
            codeMedicalManufacturerLanguage MedicalManufacturer = Mapper.Map(model, new codeMedicalManufacturerLanguage());
            int MedicalManufacturerDescription = DbEMT.codeMedicalManufacturerLanguage
                                    .Where(x => x.MedicalManufacturerDescription == MedicalManufacturer.MedicalManufacturerDescription &&
                                                x.MedicalManufacturerGUID != MedicalManufacturer.MedicalManufacturerGUID &&
                                                x.Active).Count();
            if (MedicalManufacturerDescription > 0)
            {
                ModelState.AddModelError("MedicalManufacturerDescription", "MedicalManufacturer is already exists");
            }
            return (MedicalManufacturerDescription > 0);
        }

        #endregion

        #region  Medical Manufacturer Language

        //[Route("EMT/MedicalManufacturerLanguagesDataTable/{PK}")]
        public ActionResult MedicalManufacturerLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/EMT/Views/MedicalManufacturers/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeMedicalManufacturerLanguage, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeMedicalManufacturerLanguage>(DataTable.Filters);
            }

            var Result = DbEMT.codeMedicalManufacturerLanguage.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.MedicalManufacturerGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.MedicalManufacturerLanguageGUID,
                                  x.LanguageID,
                                  x.MedicalManufacturerDescription,
                                  x.codeMedicalManufacturerLanguageRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult MedicalManufacturerLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.MedicalManufacturer.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/EMT/Views/MedicalManufacturers/_LanguageUpdateModal.cshtml",
                new codeMedicalManufacturerLanguage { MedicalManufacturerGUID = FK });
        }

        public ActionResult MedicalManufacturerLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.MedicalManufacturer.Access, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/EMT/Views/MedicalManufacturers/_LanguageUpdateModal.cshtml", DbEMT.codeMedicalManufacturerLanguage.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalManufacturerLanguageCreate(codeMedicalManufacturerLanguage model)
        {
            if (!CMS.HasAction(Permissions.MedicalManufacturer.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalManufacturerLanguage(model)) return PartialView("~/Areas/EMT/Views/MedicalManufacturers/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbEMT.Create(model, Permissions.MedicalManufacturer.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(DataTableNames.MedicalManufacturerLanguagesDataTable, DbEMT.PrimaryKeyControl(model), DbEMT.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalManufacturerLanguageUpdate(codeMedicalManufacturerLanguage model)
        {
            if (!CMS.HasAction(Permissions.MedicalManufacturer.Update, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalManufacturerLanguage(model)) return PartialView("~/Areas/EMT/Views/MedicalManufacturers/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbEMT.Update(model, Permissions.MedicalManufacturer.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(DataTableNames.MedicalManufacturerLanguagesDataTable,
                    DbEMT.PrimaryKeyControl(model),
                    DbEMT.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalManufacturerLanguage(model.MedicalManufacturerLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalManufacturerLanguageDelete(codeMedicalManufacturerLanguage model)
        {
            if (!CMS.HasAction(Permissions.MedicalManufacturer.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeMedicalManufacturerLanguage> DeletedLanguages = DeleteMedicalManufacturerLanguages(new List<codeMedicalManufacturerLanguage> { model });

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleDeleteMessage(DeletedLanguages, DataTableNames.MedicalManufacturerLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalManufacturerLanguage(model.MedicalManufacturerLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalManufacturerLanguageRestore(codeMedicalManufacturerLanguage model)
        {
            if (!CMS.HasAction(Permissions.MedicalManufacturer.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveMedicalManufacturerLanguage(model))
            {
                return Json(DbEMT.RecordExists());
            }

            List<codeMedicalManufacturerLanguage> RestoredLanguages = RestoreMedicalManufacturerLanguages(Portal.SingleToList(model));

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleRestoreMessage(RestoredLanguages, DataTableNames.MedicalManufacturerLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalManufacturerLanguage(model.MedicalManufacturerLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalManufacturerLanguagesDataTableDelete(List<codeMedicalManufacturerLanguage> models)
        {
            if (!CMS.HasAction(Permissions.MedicalManufacturer.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeMedicalManufacturerLanguage> DeletedLanguages = DeleteMedicalManufacturerLanguages(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.MedicalManufacturerLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalManufacturerLanguagesDataTableRestore(List<codeMedicalManufacturerLanguage> models)
        {
            if (!CMS.HasAction(Permissions.MedicalManufacturer.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeMedicalManufacturerLanguage> RestoredLanguages = RestoreMedicalManufacturerLanguages(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.MedicalManufacturerLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        private List<codeMedicalManufacturerLanguage> DeleteMedicalManufacturerLanguages(List<codeMedicalManufacturerLanguage> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeMedicalManufacturerLanguage> DeletedMedicalManufacturerLanguages = new List<codeMedicalManufacturerLanguage>();

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalManufacturer.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbEMT.Database.SqlQuery<codeMedicalManufacturerLanguage>(query).ToList();

            foreach (var language in languages)
            {
                DeletedMedicalManufacturerLanguages.Add(DbEMT.Delete(language, ExecutionTime, Permissions.MedicalManufacturer.DeleteGuid, DbCMS));
            }

            return DeletedMedicalManufacturerLanguages;
        }

        private List<codeMedicalManufacturerLanguage> RestoreMedicalManufacturerLanguages(List<codeMedicalManufacturerLanguage> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<codeMedicalManufacturerLanguage> RestoredLanguages = new List<codeMedicalManufacturerLanguage>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalManufacturer.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbEMT.Database.SqlQuery<codeMedicalManufacturerLanguage>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveMedicalManufacturerLanguage(language))
                {
                    RestoredLanguages.Add(DbEMT.Restore(language, Permissions.MedicalManufacturer.DeleteGuid, Permissions.MedicalManufacturer.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyMedicalManufacturerLanguage(Guid PK)
        {
            codeMedicalManufacturerLanguage dbModel = new codeMedicalManufacturerLanguage();

            var Language = DbEMT.codeMedicalManufacturerLanguage.Where(l => l.MedicalManufacturerLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbEMT.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeMedicalManufacturerLanguageRowVersion.SequenceEqual(dbModel.codeMedicalManufacturerLanguageRowVersion))
            {
                return Json(DbEMT.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbEMT, dbModel, "LanguagesContainer"));
        }

        private bool ActiveMedicalManufacturerLanguage(codeMedicalManufacturerLanguage model)
        {
            int LanguageID = DbEMT.codeMedicalManufacturerLanguage
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.MedicalManufacturerGUID == model.MedicalManufacturerGUID &&
                                              x.MedicalManufacturerLanguageGUID != model.MedicalManufacturerLanguageGUID &&
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