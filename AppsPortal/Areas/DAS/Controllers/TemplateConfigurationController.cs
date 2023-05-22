using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using DAS_DAL.Model;
using DAS_DAL.ViewModels;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.DAS.Controllers
{
    public class TemplateConfigurationController : DASBaseController
    {
        // GET: DAS/TemplateConfiguration
        #region Templates Types



        public ActionResult TemplateTypeHome()
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Access, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/DAS/Views/TemplateConfiguration/Index.cshtml");
        }


        [Route("DAS/TemplateConfiguration/")]
        public ActionResult TemplateTypeIndex()
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Access, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/DAS/Views/TemplateConfiguration/Index.cshtml");
        }

        [Route("DAS/TemplateTypeDataTable/")]
        public JsonResult TemplateTypeDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<TemplateTypeDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<TemplateTypeDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.DASConfiguration.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
            var All = (

                from a in DbDAS.codeDASTemplateType.Where(x => x.Active).AsExpandable()
                join b in DbDAS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN
                //&& x.codeTablesValues.TableGUID == LookupTables.DASproGresDescription
                ) on a.ReferenceLinkTypeGUID equals b.ValueGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()

                select new TemplateTypeDataTableModel
                {
                    TemplateTypeGUID = a.TemplateTypeGUID.ToString(),
                    Active = a.Active,
                    TemplateName = a.TemplateName,
                    Description = a.Description,
                    ReferenceLinkType = R1.ValueDescription,
                    TemplateCode = a.TemplateCode,
                    codeDASTemplateTypeRowVersion = a.codeDASTemplateTypeRowVersion,

                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<TemplateTypeDataTableModel> Result = Mapper.Map<List<TemplateTypeDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        //[Route("DAS/TemplateConfiguration/Create/")]
        public ActionResult TemplateTypeCreate()
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            TemplateTypeUpdateModel model = new TemplateTypeUpdateModel { TemplateTypeGUID = Guid.Empty };

            return PartialView("~/Areas/DAS/Views/TemplateConfiguration/_TemplateTypeForm.cshtml",
               model);


        }

        [Route("DAS/TemplateConfiguration/Update/{PK}")]
        public ActionResult TemplateTypeUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Access, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbDAS.codeDASTemplateType.WherePK(PK)

                         select new TemplateTypeUpdateModel
                         {
                             TemplateTypeGUID = a.TemplateTypeGUID,
                             TemplateName = a.TemplateName,
                             codeDASTemplateTypeRowVersion = a.codeDASTemplateTypeRowVersion,
                             ReferenceLinkTypeGUID = a.ReferenceLinkTypeGUID,


                             Active = a.Active,

                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("TemplateConfiguration", "TemplateConfiguration", new { Area = "DAS" }));

            return PartialView("~/Areas/DAS/Views/TemplateConfiguration/_TemplateTypeForm.cshtml",
              model);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TemplateTypeCreate(TemplateTypeUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || model.ReferenceLinkTypeGUID == null || string.IsNullOrEmpty(model.TemplateName) || string.IsNullOrEmpty(model.TemplateCode) || ActiveTemplateType(model))
                return PartialView("~/Areas/DAS/Views/TemplateConfiguration/_TemplateTypeForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeDASTemplateType TemplateType = Mapper.Map(model, new codeDASTemplateType());
            TemplateType.TemplateTypeGUID = EntityPK;
            TemplateType.ReferenceLinkTypeGUID = model.ReferenceLinkTypeGUID;
            DbDAS.Create(TemplateType, Permissions.DASConfiguration.CreateGuid, ExecutionTime, DbCMS);


            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.TemplateTypeDataTable, ControllerContext, "TemplateTypeDataTableControls"));


            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.DASConfiguration.Create, Apps.DAS, new UrlHelper(Request.RequestContext).Action("Create", "TemplateConfiguration", new { Area = "DAS" })), Container = "TemplateTypeDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.DASConfiguration.Update, Apps.DAS), Container = "TemplateTypeDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.DASConfiguration.Delete, Apps.DAS), Container = "TemplateTypeDetailFormControls" });

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleUpdateMessage(DataTableNames.TemplateTypeDataTable, DbDAS.PrimaryKeyControl(TemplateType), DbDAS.RowVersionControls(Portal.SingleToList(TemplateType))));
                //return Json(DbDAS.SingleCreateMessage(DbDAS.PrimaryKeyControl(TemplateType), DbDAS.RowVersionControls(TemplateType, TemplateType), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }



        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TemplateTypeUpdate(TemplateTypeUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Update, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || model.ReferenceLinkTypeGUID == null || string.IsNullOrEmpty(model.TemplateName) || string.IsNullOrEmpty(model.TemplateCode) || ActiveTemplateType(model))
                return PartialView("~/Areas/DAS/Views/TemplateConfiguration/_TemplateTypeForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeDASTemplateType TemplateType = Mapper.Map(model, new codeDASTemplateType());
            DbDAS.Update(TemplateType, Permissions.DASConfiguration.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleUpdateMessage(DataTableNames.TemplateTypeDataTable, DbDAS.PrimaryKeyControl(TemplateType), DbDAS.RowVersionControls(Portal.SingleToList(TemplateType))));
                //  return Json(DbDAS.SingleUpdateMessage(null, null, DbDAS.RowVersionControls(TemplateType, TemplateType)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyTemplateType((Guid)model.TemplateTypeGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TemplateTypeDelete(codeDASTemplateType model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Delete, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDASTemplateType> DeletedTemplateType = DeleteTemplateType(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.DASConfiguration.Restore, Apps.DAS), Container = "TemplateTypeFormControls" });

            try
            {
                int CommitedRows = DbDAS.SaveChanges();
                DbDAS.SaveChanges();
                return Json(DbDAS.SingleDeleteMessage(CommitedRows, DeletedTemplateType.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyTemplateType(model.TemplateTypeGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TemplateTypeRestore(codeDASTemplateType model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Restore, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveTemplateType(model))
            {
                return Json(DbDAS.RecordExists());
            }

            List<codeDASTemplateType> RestoredTemplateType = RestoreTemplateType(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.DASConfiguration.Create, Apps.DAS, new UrlHelper(Request.RequestContext).Action("TemplateTypeCreate", "Configuration", new { Area = "DAS" })), Container = "TemplateTypeFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.DASConfiguration.Update, Apps.DAS), Container = "TemplateTypeFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.DASConfiguration.Delete, Apps.DAS), Container = "TemplateTypeFormControls" });

            try
            {
                int CommitedRows = DbDAS.SaveChanges();
                DbDAS.SaveChanges();
                return Json(DbDAS.SingleRestoreMessage(CommitedRows, RestoredTemplateType, DbDAS.PrimaryKeyControl(RestoredTemplateType.FirstOrDefault()), Url.Action(DataTableNames.TemplateTypeDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyTemplateType(model.TemplateTypeGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult TemplateTypeDataTableDelete(List<codeDASTemplateType> models)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Delete, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDASTemplateType> DeletedTemplateType = DeleteTemplateType(models);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.PartialDeleteMessage(DeletedTemplateType, models, DataTableNames.TemplateTypeDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult TemplateTypeDataTableRestore(List<codeDASTemplateType> models)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Restore, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDASTemplateType> RestoredTemplateType = RestoreTemplateType(models);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.PartialRestoreMessage(RestoredTemplateType, models, DataTableNames.TemplateTypeDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        private List<codeDASTemplateType> DeleteTemplateType(List<codeDASTemplateType> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<codeDASTemplateType> DeletedTemplateType = new List<codeDASTemplateType>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT TemplateTypeGUID,CONVERT(varchar(50), TemplateTypeGUID) as C2 ,codeDASTemplateTypeRowVersion FROM code.codeDASTemplateType where TemplateTypeGUID in (" + string.Join(",", models.Select(x => "'" + x.TemplateTypeGUID + "'").ToArray()) + ")";

            string query = DbDAS.QueryBuilder(models, Permissions.DASConfiguration.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbDAS.Database.SqlQuery<codeDASTemplateType>(query).ToList();
            foreach (var record in Records)
            {
                DeletedTemplateType.Add(DbDAS.Delete(record, ExecutionTime, Permissions.DASConfiguration.DeleteGuid, DbCMS));
            }

            return DeletedTemplateType;
        }

        private List<codeDASTemplateType> RestoreTemplateType(List<codeDASTemplateType> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<codeDASTemplateType> RestoredTemplateType = new List<codeDASTemplateType>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT TemplateTypeGUID,CONVERT(varchar(50), TemplateTypeGUID) as C2 ,codeDASTemplateTypeRowVersion FROM code.codeDASTemplateType where TemplateTypeGUID in (" + string.Join(",", models.Select(x => "'" + x.TemplateTypeGUID + "'").ToArray()) + ")";

            string query = DbDAS.QueryBuilder(models, Permissions.DASConfiguration.DeleteGuid, SubmitTypes.Restore, "");

            var Records = DbDAS.Database.SqlQuery<codeDASTemplateType>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveTemplateType(record))
                {
                    RestoredTemplateType.Add(DbDAS.Restore(record, Permissions.DASConfiguration.DeleteGuid, Permissions.DASConfiguration.RestoreGuid, RestoringTime, DbCMS));
                }
            }


            return RestoredTemplateType;
        }

        private JsonResult ConcurrencyTemplateType(Guid PK)
        {
            TemplateTypeUpdateModel dbModel = new TemplateTypeUpdateModel();

            var TemplateType = DbDAS.codeDASTemplateType.Where(x => x.TemplateTypeGUID == PK).FirstOrDefault();
            var dbTemplateType = DbDAS.Entry(TemplateType).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbTemplateType, dbModel);





            if (TemplateType.codeDASTemplateTypeRowVersion.SequenceEqual(dbModel.codeDASTemplateTypeRowVersion))
            {
                return Json(DbDAS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbDAS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveTemplateType(Object model)
        {
            codeDASTemplateType TemplateType = Mapper.Map(model, new codeDASTemplateType());
            int ModelDescription = DbDAS.codeDASTemplateType
                                    .Where(x => x.TemplateName == TemplateType.TemplateName &&

                                                x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("ModelDescription", "TemplateType is already exists");
            }
            return (ModelDescription > 0);
        }

        #endregion

        #region Documents

        public ActionResult TemplateTypeDocumentIndex(Guid PK)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Access, Apps.DAS))
            {
                return Json(DbDAS.PermissionError());
            }
            TemplateTypeDocumentDataTableModel model = new TemplateTypeDocumentDataTableModel();
            model.TemplateTypeGUID = PK;
            return View("~/Areas/DAS/Views/TemplateConfiguration/Documents/Index.cshtml", model);
        }


        //[Route("DAS/TemplateTypeDocumentsDataTable/{PK}")]
        public ActionResult TemplateTypeDocumentsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/DAS/Views/TemplateConfiguration/Documents/_TemplateTypeDocumentsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeDASTemplateTypeDocument, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeDASTemplateTypeDocument>(DataTable.Filters);
            }

            var Result = DbDAS.codeDASTemplateTypeDocument.AsNoTracking().AsExpandable().Where(x =>
            x.TemplateTypeGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.TemplateTypeDocumentGUID,
                                  x.TemplateTypeGUID,
                                  x.DocumentName,
                                  Description = x.Description,
                                  TemplateName = x.codeDASTemplateType.TemplateName,
                                  x.codeDASTemplateTypeDocumentRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        [Route("DAS/TemplateConfiguration/TemplateTypeDocumentCreate/{PK}")]
        public ActionResult TemplateTypeDocumentCreate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            TemplateTypeDocumentModel model = new TemplateTypeDocumentModel { TemplateTypeGUID = PK, HasConfidentialData = false, };

            return View("~/Areas/DAS/Views/TemplateConfiguration/Documents/DocumentForm.cshtml", model);

            //return PartialView("~/Areas/DAS/Views/TemplateConfiguration/Documents/_TemplateTypeDocumentUpdateModal.cshtml",
            //   model);
        }

        public ActionResult TemplateTypeDocumentUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Access, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            TemplateTypeDocumentModel model = DbDAS.codeDASTemplateTypeDocument.Where(a => a.TemplateTypeDocumentGUID == PK).
                Select(a => new TemplateTypeDocumentModel
                {
                    TemplateTypeDocumentGUID = a.TemplateTypeDocumentGUID,
                    TemplateTypeGUID = a.TemplateTypeGUID,
                    TemplateName = a.codeDASTemplateType.TemplateName,
                    DocumentName = a.DocumentName,
                    DocumentCode = a.DocumentCode,
                    Description = a.Description,
                    //IsSupervisor = (bool)a.IsSupervisor,
                    CreateByGUID = a.CreateByGUID,
                    Active = a.Active,
                    HasConfidentialData = (bool)a.HasConfidentialData


                }
                ).FirstOrDefault();
            return View("~/Areas/DAS/Views/TemplateConfiguration/Documents/DocumentForm.cshtml", model);
            ////return PartialView("~/Areas/DAS/Views/TemplateConfiguration/Documents/_TemplateTypeDocumentUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TemplateTypeDocumentCreate(codeDASTemplateTypeDocument model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveTemplateTypeDocument(model)) return PartialView("~/Areas/DAS/Views/TemplateConfiguration/Documents/_TemplateTypeDocumentUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            //var user = DbDAS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.UserGUID == model.UserGUID).FirstOrDefault();
            //model.Description = user.FirstName + " " + user.Surname;
            //model.IsSupervisor = model.IsSupervisor;


            DbDAS.Create(model, Permissions.DASConfiguration.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleUpdateMessage(DataTableNames.TemplateTypeDocumentsDataTable, DbDAS.PrimaryKeyControl(model), DbDAS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TemplateTypeDocumentUpdate(codeDASTemplateTypeDocument model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Update, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveTemplateTypeDocument(model)) return PartialView("~/Areas/DAS/Views/TemplateConfiguration/Documents/_TemplateTypeDocumentUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbDAS.Update(model, Permissions.DASConfiguration.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleUpdateMessage(DataTableNames.TemplateTypeDocumentsDataTable,
                    DbDAS.PrimaryKeyControl(model),
                    DbDAS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyTemplateTypeDocument(model.TemplateTypeDocumentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TemplateTypeDocumentDelete(codeDASTemplateTypeDocument model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Delete, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDASTemplateTypeDocument> DeletedLanguages = DeleteTemplateTypeDocument(new List<codeDASTemplateTypeDocument> { model });

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleDeleteMessage(DeletedLanguages, DataTableNames.TemplateTypeDocumentsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyTemplateTypeDocument(model.TemplateTypeDocumentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TemplateTypeDocumentRestore(codeDASTemplateTypeDocument model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Restore, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveTemplateTypeDocument(model))
            {
                return Json(DbDAS.RecordExists());
            }

            List<codeDASTemplateTypeDocument> RestoredLanguages = RestoreTemplateTypeDocument(Portal.SingleToList(model));

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleRestoreMessage(RestoredLanguages, DataTableNames.TemplateTypeDocumentsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyTemplateTypeDocument(model.TemplateTypeDocumentGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult TemplateTypeDocumentsDataTableDelete(List<codeDASTemplateTypeDocument> models)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Delete, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDASTemplateTypeDocument> DeletedLanguages = DeleteTemplateTypeDocument(models);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.TemplateTypeDocumentsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult TemplateTypeDocumentsDataTableRestore(List<codeDASTemplateTypeDocument> models)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Restore, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDASTemplateTypeDocument> RestoredLanguages = RestoreTemplateTypeDocument(models);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.TemplateTypeDocumentsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        private List<codeDASTemplateTypeDocument> DeleteTemplateTypeDocument(List<codeDASTemplateTypeDocument> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeDASTemplateTypeDocument> DeletedTemplateTypeDocument = new List<codeDASTemplateTypeDocument>();

            string query = DbDAS.QueryBuilder(models, Permissions.DASConfiguration.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbDAS.Database.SqlQuery<codeDASTemplateTypeDocument>(query).ToList();

            foreach (var language in languages)
            {
                DeletedTemplateTypeDocument.Add(DbDAS.Delete(language, ExecutionTime, Permissions.DASConfiguration.DeleteGuid, DbCMS));
            }

            return DeletedTemplateTypeDocument;
        }

        private List<codeDASTemplateTypeDocument> RestoreTemplateTypeDocument(List<codeDASTemplateTypeDocument> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<codeDASTemplateTypeDocument> RestoredLanguages = new List<codeDASTemplateTypeDocument>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbDAS.QueryBuilder(models, Permissions.DASConfiguration.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbDAS.Database.SqlQuery<codeDASTemplateTypeDocument>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveTemplateTypeDocument(language))
                {
                    RestoredLanguages.Add(DbDAS.Restore(language, Permissions.DASConfiguration.DeleteGuid, Permissions.DASConfiguration.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyTemplateTypeDocument(Guid PK)
        {
            codeDASTemplateTypeDocument dbModel = new codeDASTemplateTypeDocument();

            var Language = DbDAS.codeDASTemplateTypeDocument.Where(l => l.TemplateTypeDocumentGUID == PK).FirstOrDefault();
            var dbLanguage = DbDAS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeDASTemplateTypeDocumentRowVersion.SequenceEqual(dbModel.codeDASTemplateTypeDocumentRowVersion))
            {
                return Json(DbDAS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbDAS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveTemplateTypeDocument(codeDASTemplateTypeDocument model)
        {
            int LanguageID = DbDAS.codeDASTemplateTypeDocument
                                  .Where(x =>
                                              x.DocumentName == model.DocumentName &&
                                              x.Description == model.Description &&
                                              x.TemplateTypeGUID == model.TemplateTypeGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                //Fix
                ModelState.AddModelError("LanguageID", "Already exits");
            }

            return (LanguageID > 0);
        }

        #endregion Language

        #region Tags


        //[Route("DAS/TemplateTypeDocumentTagsDataTable/{PK}")]
        public ActionResult TemplateTypeDocumentTagsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/DAS/Views/TemplateConfiguration/Tags/_TemplateTypeDocumentTagsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeDASTemplateTypeDocumentTag, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeDASTemplateTypeDocumentTag>(DataTable.Filters);
            }

            var Result = DbDAS.codeDASTemplateTypeDocumentTag.AsNoTracking().AsExpandable().Where(x =>
            x.TemplateTypeDocumentGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.TemplateTypeDocumentTagGUID,
                                  x.TemplateTypeDocumentGUID,
                                  x.TagName,
                                  Description = x.Description,
                                  x.codeDASTemplateTypeDocumentTagRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        //[Route("DAS/TemplateConfiguration/TemplateTypeDocumentTagCreate/")]
        public ActionResult TemplateTypeDocumentTagCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            TemplateTypeDocumentTagModel model = new TemplateTypeDocumentTagModel
            {
                TemplateTypeDocumentGUID = FK,
                IsMandatury = false
            };

            return PartialView("~/Areas/DAS/Views/TemplateConfiguration/Tags/_TagsUpdateModal.cshtml",
               model);
        }

        public ActionResult TemplateTypeDocumentTagUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Access, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            TemplateTypeDocumentTagModel model = DbDAS.codeDASTemplateTypeDocumentTag.Where(a => a.TemplateTypeDocumentTagGUID == PK).
                Select(a => new TemplateTypeDocumentTagModel
                {
                    TemplateTypeDocumentTagGUID = a.TemplateTypeDocumentTagGUID,
                    TemplateTypeDocumentGUID = a.TemplateTypeDocumentGUID,
                    TagName = a.TagName,
                    Description = a.Description,
                    IsMandatury = (bool)a.IsMandatury,
                    CreateByGUID = a.CreateByGUID,
                    Active = a.Active


                }
                ).FirstOrDefault();
            return PartialView("~/Areas/DAS/Views/TemplateConfiguration/Tags/_TagsUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TemplateTypeDocumentTagCreate(TemplateTypeDocumentTagModel model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            codeDASTemplateTypeDocumentTag _tag = Mapper.Map(model, new codeDASTemplateTypeDocumentTag());
            if (!ModelState.IsValid || ActiveTemplateTypeDocumentTag(_tag)) return PartialView("~/Areas/DAS/Views/TemplateConfiguration/Tags/_TagsUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            _tag.Description = model.Description;
            _tag.IsMandatury = model.IsMandatury;


            DbDAS.Create(_tag, Permissions.DASConfiguration.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleUpdateMessage(DataTableNames.TemplateTypeDocumentTagsDataTable, DbDAS.PrimaryKeyControl(_tag), DbDAS.RowVersionControls(Portal.SingleToList(_tag))));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TemplateTypeDocumentTagUpdate(TemplateTypeDocumentTagModel model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Update, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            codeDASTemplateTypeDocumentTag _tag = Mapper.Map(model, new codeDASTemplateTypeDocumentTag());
            if (!ModelState.IsValid || ActiveTemplateTypeDocumentTag(_tag)) return PartialView("~/Areas/DAS/Views/TemplateConfiguration/Tags/_TagsUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbDAS.Update(_tag, Permissions.DASConfiguration.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleUpdateMessage(DataTableNames.TemplateTypeDocumentTagsDataTable,
                    DbDAS.PrimaryKeyControl(_tag),
                    DbDAS.RowVersionControls(Portal.SingleToList(_tag))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyTemplateTypeDocumentTag(model.TemplateTypeDocumentTagGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TemplateTypeDocumentTagDelete(codeDASTemplateTypeDocumentTag model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Delete, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDASTemplateTypeDocumentTag> DeletedLanguages = DeleteTemplateTypeDocumentTag(new List<codeDASTemplateTypeDocumentTag> { model });

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleDeleteMessage(DeletedLanguages, DataTableNames.TemplateTypeDocumentTagsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyTemplateTypeDocumentTag(model.TemplateTypeDocumentTagGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TemplateTypeDocumentTagRestore(codeDASTemplateTypeDocumentTag model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Restore, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveTemplateTypeDocumentTag(model))
            {
                return Json(DbDAS.RecordExists());
            }

            List<codeDASTemplateTypeDocumentTag> RestoredLanguages = RestoreTemplateTypeDocumentTag(Portal.SingleToList(model));

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleRestoreMessage(RestoredLanguages, DataTableNames.TemplateTypeDocumentTagsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyTemplateTypeDocumentTag(model.TemplateTypeDocumentTagGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult TemplateTypeDocumentTagsDataTableDelete(List<codeDASTemplateTypeDocumentTag> models)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Delete, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDASTemplateTypeDocumentTag> DeletedLanguages = DeleteTemplateTypeDocumentTag(models);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.TemplateTypeDocumentTagsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult TemplateTypeDocumentTagsDataTableRestore(List<codeDASTemplateTypeDocumentTag> models)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Restore, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDASTemplateTypeDocumentTag> RestoredLanguages = RestoreTemplateTypeDocumentTag(models);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.TemplateTypeDocumentTagsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        private List<codeDASTemplateTypeDocumentTag> DeleteTemplateTypeDocumentTag(List<codeDASTemplateTypeDocumentTag> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeDASTemplateTypeDocumentTag> DeletedTemplateTypeDocumentTag = new List<codeDASTemplateTypeDocumentTag>();

            string query = DbDAS.QueryBuilder(models, Permissions.DASConfiguration.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbDAS.Database.SqlQuery<codeDASTemplateTypeDocumentTag>(query).ToList();

            foreach (var language in languages)
            {
                DeletedTemplateTypeDocumentTag.Add(DbDAS.Delete(language, ExecutionTime, Permissions.DASConfiguration.DeleteGuid, DbCMS));
            }

            return DeletedTemplateTypeDocumentTag;
        }

        private List<codeDASTemplateTypeDocumentTag> RestoreTemplateTypeDocumentTag(List<codeDASTemplateTypeDocumentTag> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<codeDASTemplateTypeDocumentTag> RestoredLanguages = new List<codeDASTemplateTypeDocumentTag>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbDAS.QueryBuilder(models, Permissions.DASConfiguration.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbDAS.Database.SqlQuery<codeDASTemplateTypeDocumentTag>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveTemplateTypeDocumentTag(language))
                {
                    RestoredLanguages.Add(DbDAS.Restore(language, Permissions.DASConfiguration.DeleteGuid, Permissions.DASConfiguration.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyTemplateTypeDocumentTag(Guid PK)
        {
            codeDASTemplateTypeDocumentTag dbModel = new codeDASTemplateTypeDocumentTag();

            var Language = DbDAS.codeDASTemplateTypeDocumentTag.Where(l => l.TemplateTypeDocumentTagGUID == PK).FirstOrDefault();
            var dbLanguage = DbDAS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeDASTemplateTypeDocumentTagRowVersion.SequenceEqual(dbModel.codeDASTemplateTypeDocumentTagRowVersion))
            {
                return Json(DbDAS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbDAS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveTemplateTypeDocumentTag(codeDASTemplateTypeDocumentTag model)
        {
            int LanguageID = DbDAS.codeDASTemplateTypeDocumentTag
                                  .Where(x =>
                                              x.TagName == model.TagName &&
                                              x.Description == model.Description &&
                                              x.TemplateTypeDocumentGUID == model.TemplateTypeDocumentGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                //Fix
                ModelState.AddModelError("LanguageID", "Already exits");
            }

            return (LanguageID > 0);
        }

        #endregion Language

        #region SoftFields


        //[Route("DAS/TemplateTypeDocumentSoftFieldsDataTable/{PK}")]
        public ActionResult TemplateTypeDocumentSoftFieldsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/DAS/Views/TemplateConfiguration/SoftFields/_SoftFieldsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeDASTemplateTypeDocumentSoftField, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeDASTemplateTypeDocumentSoftField>(DataTable.Filters);
            }

            var Result = DbDAS.codeDASTemplateTypeDocumentSoftField.AsNoTracking().AsExpandable().Where(x =>
            x.TemplateTypeDocumentGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.TemplateTypeDocumentSoftFieldGUID,
                                  x.TemplateTypeDocumentGUID,
                                  x.SoftFieldName,
                                  x.FieldTypeGUID,

                                  IsMandatury = x.IsMandatury,
                                  x.codeDASTemplateTypeDocumentSoftFieldRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        //[Route("DAS/TemplateConfiguration/TemplateTypeDocumentSoftFieldCreate/")]
        public ActionResult TemplateTypeDocumentSoftFieldCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            TemplateTypeDocumentSoftFieldModel model = new TemplateTypeDocumentSoftFieldModel { TemplateTypeDocumentGUID = FK, IsMandatury = false };

            return PartialView("~/Areas/DAS/Views/TemplateConfiguration/SoftFields/_SoftFieldUpdateModal.cshtml",
               model);

        }

        public ActionResult TemplateTypeDocumentSoftFieldUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Access, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            TemplateTypeDocumentSoftFieldModel model = DbDAS.codeDASTemplateTypeDocumentSoftField.Where(a => a.TemplateTypeDocumentSoftFieldGUID == PK).
                Select(a => new TemplateTypeDocumentSoftFieldModel
                {
                    TemplateTypeDocumentSoftFieldGUID = a.TemplateTypeDocumentSoftFieldGUID,
                    TemplateTypeDocumentGUID = (Guid)a.TemplateTypeDocumentGUID,
                    SoftFieldName = a.SoftFieldName,
                    FieldTypeGUID = a.FieldTypeGUID,
                    IsMandatury = (bool)a.IsMandatury,
                    SoftFieldSourceTypeGUID = a.SoftFieldSourceTypeGUID,
                    CreateByGUID = a.CreateByGUID,
                    Active = a.Active


                }
                ).FirstOrDefault();
            return PartialView("~/Areas/DAS/Views/TemplateConfiguration/SoftFields/_SoftFieldUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TemplateTypeDocumentSoftFieldCreate(TemplateTypeDocumentSoftFieldModel model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            codeDASTemplateTypeDocumentSoftField _soft = Mapper.Map(model, new codeDASTemplateTypeDocumentSoftField());
            if (!ModelState.IsValid || ActiveTemplateTypeDocumentSoftField(_soft)) return PartialView("~/Areas/DAS/Views/TemplateConfiguration/SoftFields/_SoftFieldsUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            _soft.FieldTypeGUID = model.FieldTypeGUID;
            _soft.IsMandatury = model.IsMandatury;
            _soft.SoftFieldSourceTypeGUID = model.SoftFieldSourceTypeGUID;


            DbDAS.Create(_soft, Permissions.DASConfiguration.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleUpdateMessage(DataTableNames.TemplateTypeDocumentSoftFieldsDataTable, DbDAS.PrimaryKeyControl(_soft), DbDAS.RowVersionControls(Portal.SingleToList(_soft))));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TemplateTypeDocumentSoftFieldUpdate(TemplateTypeDocumentSoftFieldModel model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Update, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            codeDASTemplateTypeDocumentSoftField _soft = Mapper.Map(model, new codeDASTemplateTypeDocumentSoftField());
            if (!ModelState.IsValid || ActiveTemplateTypeDocumentSoftField(_soft)) return PartialView("~/Areas/DAS/Views/TemplateConfiguration/SoftFields/_SoftFieldUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbDAS.Update(_soft, Permissions.DASConfiguration.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleUpdateMessage(DataTableNames.TemplateTypeDocumentSoftFieldsDataTable,
                    DbDAS.PrimaryKeyControl(_soft),
                    DbDAS.RowVersionControls(Portal.SingleToList(_soft))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyTemplateTypeDocumentSoftField((Guid)model.TemplateTypeDocumentSoftFieldGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TemplateTypeDocumentSoftFieldDelete(codeDASTemplateTypeDocumentSoftField model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Delete, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDASTemplateTypeDocumentSoftField> DeletedLanguages = DeleteTemplateTypeDocumentSoftField(new List<codeDASTemplateTypeDocumentSoftField> { model });

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleDeleteMessage(DeletedLanguages, DataTableNames.TemplateTypeDocumentSoftFieldsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyTemplateTypeDocumentSoftField(model.TemplateTypeDocumentSoftFieldGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TemplateTypeDocumentSoftFieldRestore(codeDASTemplateTypeDocumentSoftField model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Restore, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveTemplateTypeDocumentSoftField(model))
            {
                return Json(DbDAS.RecordExists());
            }

            List<codeDASTemplateTypeDocumentSoftField> RestoredLanguages = RestoreTemplateTypeDocumentSoftField(Portal.SingleToList(model));

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleRestoreMessage(RestoredLanguages, DataTableNames.TemplateTypeDocumentSoftFieldsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyTemplateTypeDocumentSoftField(model.TemplateTypeDocumentSoftFieldGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult TemplateTypeDocumentSoftFieldsDataTableDelete(List<codeDASTemplateTypeDocumentSoftField> models)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Delete, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDASTemplateTypeDocumentSoftField> DeletedLanguages = DeleteTemplateTypeDocumentSoftField(models);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.TemplateTypeDocumentSoftFieldsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult TemplateTypeDocumentSoftFieldsDataTableRestore(List<codeDASTemplateTypeDocumentSoftField> models)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Restore, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDASTemplateTypeDocumentSoftField> RestoredLanguages = RestoreTemplateTypeDocumentSoftField(models);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.TemplateTypeDocumentSoftFieldsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        private List<codeDASTemplateTypeDocumentSoftField> DeleteTemplateTypeDocumentSoftField(List<codeDASTemplateTypeDocumentSoftField> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeDASTemplateTypeDocumentSoftField> DeletedTemplateTypeDocumentSoftField = new List<codeDASTemplateTypeDocumentSoftField>();

            string query = DbDAS.QueryBuilder(models, Permissions.DASConfiguration.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbDAS.Database.SqlQuery<codeDASTemplateTypeDocumentSoftField>(query).ToList();

            foreach (var language in languages)
            {
                DeletedTemplateTypeDocumentSoftField.Add(DbDAS.Delete(language, ExecutionTime, Permissions.DASConfiguration.DeleteGuid, DbCMS));
            }

            return DeletedTemplateTypeDocumentSoftField;
        }

        private List<codeDASTemplateTypeDocumentSoftField> RestoreTemplateTypeDocumentSoftField(List<codeDASTemplateTypeDocumentSoftField> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<codeDASTemplateTypeDocumentSoftField> RestoredLanguages = new List<codeDASTemplateTypeDocumentSoftField>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbDAS.QueryBuilder(models, Permissions.DASConfiguration.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbDAS.Database.SqlQuery<codeDASTemplateTypeDocumentSoftField>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveTemplateTypeDocumentSoftField(language))
                {
                    RestoredLanguages.Add(DbDAS.Restore(language, Permissions.DASConfiguration.DeleteGuid, Permissions.DASConfiguration.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyTemplateTypeDocumentSoftField(Guid PK)
        {
            codeDASTemplateTypeDocumentSoftField dbModel = new codeDASTemplateTypeDocumentSoftField();

            var Language = DbDAS.codeDASTemplateTypeDocumentSoftField.Where(l => l.TemplateTypeDocumentSoftFieldGUID == PK).FirstOrDefault();
            var dbLanguage = DbDAS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeDASTemplateTypeDocumentSoftFieldRowVersion.SequenceEqual(dbModel.codeDASTemplateTypeDocumentSoftFieldRowVersion))
            {
                return Json(DbDAS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbDAS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveTemplateTypeDocumentSoftField(codeDASTemplateTypeDocumentSoftField model)
        {
            int LanguageID = DbDAS.codeDASTemplateTypeDocumentSoftField
                                  .Where(x =>
                                              x.SoftFieldName == model.SoftFieldName &&
                                              x.FieldTypeGUID == model.FieldTypeGUID &&
                                              x.TemplateTypeDocumentGUID == model.TemplateTypeDocumentGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                //Fix
                ModelState.AddModelError("LanguageID", "Staff Already exits");
            }

            return (LanguageID > 0);
        }

        #endregion Language

    }
}