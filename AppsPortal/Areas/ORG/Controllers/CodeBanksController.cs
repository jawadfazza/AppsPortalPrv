using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using FineUploader;
using LinqKit;
using ORG_DAL.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace AppsPortal.Areas.ORG.Controllers
{
    public class CodeBanksController : ORGBaseController
    {
        // GET: ORG/codeBank
        #region Code Bank

        [Route("ORG/CodeBanks/")]
        public ActionResult CodeBankIndex()
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Access, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            return View("~/Areas/ORG/Views/CodeBanks/Index.cshtml");
        }

        [Route("ORG/CodeBanksDataTable/")]
        public JsonResult CodeBanksDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<CodeBankDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<CodeBankDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.StaffProfile.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
            var All = (

                from a in DbORG.codeBank.Where(x => x.Active).AsExpandable()
                join b in DbORG.codeBankLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeBank.DeletedOn) && x.LanguageID == LAN)
                on a.BankGUID equals b.BankGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()

                select new CodeBankDataTableModel
                {
                    BankGUID = a.BankGUID,
                    Active = a.Active,
                    BankDescription = R1.BankDescription,
                    BankCode = a.BankCode,


                    codeBankRowVersion = a.codeBankRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<CodeBankDataTableModel> Result = Mapper.Map<List<CodeBankDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("ORG/CodeBanks/Create/")]
        public ActionResult CodeBankCreate()
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Create, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            return View("~/Areas/ORG/Views/CodeBanks/CodeBank.cshtml", new CodeBankUpdateModel());
        }

        [Route("ORG/CodeBanks/Update/{PK}")]
        public ActionResult CodeBankUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            var model = (from a in DbORG.codeBank.WherePK(PK)
                         join b in DbORG.codeBankLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeBank.DeletedOn) && x.LanguageID == LAN)
                         on a.BankGUID equals b.BankGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new CodeBankUpdateModel
                         {
                             BankGUID = a.BankGUID,
                             BankDescription = R1.BankDescription,
                             BankCode = a.BankCode,
                             CountryGUID = a.CountryGUID,

                             Active = a.Active,
                             codeBankRowVersion = a.codeBankRowVersion,
                             codeBankLanguageRowVersion = R1.codeBankLanguageRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("CodeBank", "CodeBanks", new { Area = "ORG" }));

            return View("~/Areas/ORG/Views/CodeBanks/CodeBank.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CodeBankCreate(CodeBankUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Create, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            if (!ModelState.IsValid || ActivecodeBank(model)) return PartialView("~/Areas/ORG/Views/CodeBanks/_codeBankForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeBank codeBank = Mapper.Map(model, new codeBank());
            codeBank.BankGUID = EntityPK;

            DbORG.Create(codeBank, Permissions.StaffProfile.CreateGuid, ExecutionTime, DbCMS);

            codeBankLanguage Language = Mapper.Map(model, new codeBankLanguage());
            Language.BankLanguageGUID = EntityPK;
            Language.BankGUID = codeBank.BankGUID;

            DbORG.Create(Language, Permissions.StaffProfile.CreateGuid, ExecutionTime, DbCMS);



            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.CodeBanksDataTable, ControllerContext, "StaffProfileLanguagesFormControls"));


            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.StaffProfile.Create, Apps.ORG, new UrlHelper(Request.RequestContext).Action("Create", "codeBank", new { Area = "ORG" })), Container = "CodeBankFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.StaffProfile.Update, Apps.ORG), Container = "CodeBankFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.StaffProfile.Delete, Apps.ORG), Container = "CodeBankFormControls" });

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleCreateMessage(DbORG.PrimaryKeyControl(codeBank), DbORG.RowVersionControls(codeBank, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CodeBankUpdate(CodeBankUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/ORG/Views/CodeBanks/_CodeBankForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeBank StaffProfile = Mapper.Map(model, new codeBank());
            DbORG.Update(StaffProfile, Permissions.StaffProfile.UpdateGuid, ExecutionTime, DbCMS);

            var Language = DbORG.codeBankLanguage.Where(l => l.BankGUID == model.BankGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.BankGUID = StaffProfile.BankGUID;
                DbORG.Create(Language, Permissions.StaffProfile.CreateGuid, ExecutionTime, DbCMS);
            }
            else if (Language.BankDescription != model.BankDescription)
            {
                Language = Mapper.Map(model, Language);
                DbORG.Update(Language, Permissions.StaffProfile.UpdateGuid, ExecutionTime, DbCMS);
            }

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleUpdateMessage(null, null, DbORG.RowVersionControls(StaffProfile, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencycodeBank(model.BankGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CodeBankDelete(codeBank model)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Delete, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            List<codeBank> DeletedStaffProfile = DeletecodeBank(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.StaffProfile.Restore, Apps.ORG), Container = "codeBankFormControls" });

            try
            {
                int CommitedRows = DbORG.SaveChanges();
                DbORG.SaveChanges();
                return Json(DbORG.SingleDeleteMessage(CommitedRows, DeletedStaffProfile.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencycodeBank(model.BankGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CodeBankRestore(codeBank model)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Restore, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            if (ActivecodeBank(model))
            {
                return Json(DbORG.RecordExists());
            }

            List<codeBank> RestoredStaffProfile = RestorecodeBanks(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.StaffProfile.Create, Apps.ORG, new UrlHelper(Request.RequestContext).Action("StaffProfileCreate", "Configuration", new { Area = "ORG" })), Container = "CodeBankFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.StaffProfile.Update, Apps.ORG), Container = "CodeBankFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.StaffProfile.Delete, Apps.ORG), Container = "CodeBankFormControls" });

            try
            {
                int CommitedRows = DbORG.SaveChanges();
                DbORG.SaveChanges();
                return Json(DbORG.SingleRestoreMessage(CommitedRows, RestoredStaffProfile, DbORG.PrimaryKeyControl(RestoredStaffProfile.FirstOrDefault()), Url.Action(DataTableNames.CodeBanksDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencycodeBank(model.BankGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CodeBanksDataTableDelete(List<codeBank> models)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Delete, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            List<codeBank> DeletedItemClassificaiton = DeletecodeBank(models);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.PartialDeleteMessage(DeletedItemClassificaiton, models, DataTableNames.CodeBanksDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CodeBankDataTableRestore(List<codeBank> models)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Restore, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            List<codeBank> RestoredStaffProfile = DeletecodeBank(models);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.PartialRestoreMessage(RestoredStaffProfile, models, DataTableNames.CodeBanksDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        private List<codeBank> DeletecodeBank(List<codeBank> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<codeBank> DeletedStaffProfile = new List<codeBank>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT WarehouseStaffProfileGUID,CONVERT(varchar(50), WarehouseStaffProfileGUID) as C2 ,codeWarehouseStaffProfileRowVersion FROM code.codeWarehouseStaffProfile where WarehouseStaffProfileGUID in (" + string.Join(",", models.Select(x => "'" + x.WarehouseStaffProfileGUID + "'").ToArray()) + ")";

            string query = DbORG.QueryBuilder(models, Permissions.StaffProfile.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbORG.Database.SqlQuery<codeBank>(query).ToList();
            foreach (var record in Records)
            {
                DeletedStaffProfile.Add(DbORG.Delete(record, ExecutionTime, Permissions.StaffProfile.DeleteGuid, DbCMS));
            }

            var Languages = DeletedStaffProfile.SelectMany(a => a.codeBankLanguage).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbORG.Delete(language, ExecutionTime, Permissions.StaffProfile.DeleteGuid, DbCMS);
            }
            return DeletedStaffProfile;
        }

        private List<codeBank> RestorecodeBanks(List<codeBank> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<codeBank> RestoredcodeBank = new List<codeBank>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT WarehouseStaffProfileGUID,CONVERT(varchar(50), WarehouseStaffProfileGUID) as C2 ,codeWarehouseStaffProfileRowVersion FROM code.codeWarehouseStaffProfile where WarehouseStaffProfileGUID in (" + string.Join(",", models.Select(x => "'" + x.WarehouseStaffProfileGUID + "'").ToArray()) + ")";

            string query = DbORG.QueryBuilder(models, Permissions.StaffProfile.DeleteGuid, SubmitTypes.Restore, "");

            var Records = DbORG.Database.SqlQuery<codeBank>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActivecodeBank(record))
                {
                    RestoredcodeBank.Add(DbORG.Restore(record, Permissions.StaffProfile.DeleteGuid, Permissions.StaffProfile.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var Languages = RestoredcodeBank.SelectMany(x => x.codeBankLanguage.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbORG.Restore(language, Permissions.StaffProfile.DeleteGuid, Permissions.StaffProfile.RestoreGuid, RestoringTime, DbCMS);
            }

            return RestoredcodeBank;
        }

        private JsonResult ConcurrencycodeBank(Guid PK)
        {
            CodeBankUpdateModel dbModel = new CodeBankUpdateModel();

            var StaffProfile = DbORG.codeBank.Where(x => x.BankGUID == PK).FirstOrDefault();
            var dbStaffProfile = DbORG.Entry(StaffProfile).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbStaffProfile, dbModel);

            var Language = DbORG.codeBankLanguage.Where(x => x.BankGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeBank.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbORG.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (StaffProfile.codeBankRowVersion.SequenceEqual(dbModel.codeBankRowVersion) && Language.codeBankLanguageRowVersion.SequenceEqual(dbModel.codeBankLanguageRowVersion))
            {
                return Json(DbORG.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbORG, dbModel, "LanguagesContainer"));
        }

        private bool ActivecodeBank(Object model)
        {
            codeBankLanguage bank = Mapper.Map(model, new codeBankLanguage());
            int ModelDescription = DbORG.codeBankLanguage
                                    .Where(x => x.BankDescription == bank.BankDescription &&
                                                x.LanguageID == LAN &&
                                                x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("ModelDescription", "Bank is already exists");
            }
            return (ModelDescription > 0);
        }

        #endregion


        #region Bank Languages

        //[Route("ORG/CodeBankLanguagesDataTable/{PK}")]
        public ActionResult CodeBankLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/ORG/Views/CodeBanks/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeBankLanguage, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeBankLanguage>(DataTable.Filters);
            }

            var Result = DbORG.codeBankLanguage.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.BankGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.BankLanguageGUID,
                                  x.LanguageID,
                                  BankDescription = x.BankDescription,
                                  x.codeBankLanguageRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CodeBankLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Create, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            return PartialView("~/Areas/ORG/Views/CodeBanks/_LanguageUpdateModal.cshtml",
                new codeBankLanguage { BankGUID = FK });
        }

        public ActionResult CodeBankLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            return PartialView("~/Areas/ORG/Views/CodeBanks/_LanguageUpdateModal.cshtml", DbORG.codeBankLanguage.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CodeBankLanguageCreate(codeBankLanguage model)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Create, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            if (!ModelState.IsValid || ActiveCodeBankLanguage(model)) return PartialView("~/Areas/ORG/Views/CodeBanks/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbORG.Create(model, Permissions.StaffProfile.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleUpdateMessage(DataTableNames.CodeBankLanguagesDataTable, DbORG.PrimaryKeyControl(model), DbORG.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CodeBankLanguageUpdate(codeBankLanguage model)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Update, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            if (!ModelState.IsValid || ActiveCodeBankLanguage(model)) return PartialView("~/Areas/ORG/Views/CodeBanks/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbORG.Update(model, Permissions.StaffProfile.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleUpdateMessage(DataTableNames.CodeBankLanguagesDataTable,
                    DbORG.PrimaryKeyControl(model),
                    DbORG.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyCodeBankLanguage(model.BankLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CodeBankLanguageDelete(codeBankLanguage model)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Delete, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            List<codeBankLanguage> DeletedLanguages = DeleteCodeBankLanguages(new List<codeBankLanguage> { model });

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleDeleteMessage(DeletedLanguages, DataTableNames.CodeBankLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyCodeBankLanguage(model.BankLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CodeBankLanguageRestore(codeBankLanguage model)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Restore, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            if (ActiveCodeBankLanguage(model))
            {
                return Json(DbORG.RecordExists());
            }

            List<codeBankLanguage> RestoredLanguages = RestoreCodeBankLanguages(Portal.SingleToList(model));

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.SingleRestoreMessage(RestoredLanguages, DataTableNames.CodeBankLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyCodeBankLanguage(model.BankLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CodeBankLanguagesDataTableDelete(List<codeBankLanguage> models)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Restore, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            if (!CMS.HasAction(Permissions.StaffProfile.Delete, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            List<codeBankLanguage> DeletedLanguages = DeleteCodeBankLanguages(models);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.CodeBankLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CodeBankLanguagesDataTableRestore(List<codeBankLanguage> models)
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Restore, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            List<codeBankLanguage> RestoredLanguages = RestoreCodeBankLanguages(models);

            try
            {
                DbORG.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbORG.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.CodeBankLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbORG.ErrorMessage(ex.Message));
            }
        }

        private List<codeBankLanguage> DeleteCodeBankLanguages(List<codeBankLanguage> models)
        {
           
            DateTime ExecutionTime = DateTime.Now;

            List<codeBankLanguage> DeletedCodeBankLanguages = new List<codeBankLanguage>();

            string query = DbORG.QueryBuilder(models, Permissions.StaffProfile.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbORG.Database.SqlQuery<codeBankLanguage>(query).ToList();

            foreach (var language in languages)
            {
                DeletedCodeBankLanguages.Add(DbORG.Delete(language, ExecutionTime, Permissions.StaffProfile.DeleteGuid, DbCMS));
            }

            return DeletedCodeBankLanguages;
        }

        private List<codeBankLanguage> RestoreCodeBankLanguages(List<codeBankLanguage> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<codeBankLanguage> RestoredLanguages = new List<codeBankLanguage>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbORG.QueryBuilder(models, Permissions.StaffProfile.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbORG.Database.SqlQuery<codeBankLanguage>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveCodeBankLanguage(language))
                {
                    RestoredLanguages.Add(DbORG.Restore(language, Permissions.StaffProfile.DeleteGuid, Permissions.StaffProfile.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyCodeBankLanguage(Guid PK)
        {
            codeBankLanguage dbModel = new codeBankLanguage();

            var Language = DbORG.codeBankLanguage.Where(l => l.BankLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbORG.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeBankLanguageRowVersion.SequenceEqual(dbModel.codeBankLanguageRowVersion))
            {
                return Json(DbORG.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbORG, dbModel, "LanguagesContainer"));
        }

        private bool ActiveCodeBankLanguage(codeBankLanguage model)
        {
            int LanguageID = DbORG.codeBankLanguage
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.BankDescription == model.BankDescription &&
                                              x.BankGUID == model.BankGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                //Fix
                ModelState.AddModelError("LanguageID", "This Name  already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }

        #endregion Language
    }
}