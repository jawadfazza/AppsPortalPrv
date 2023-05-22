using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using DAS_DAL.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DAS_DAL.ViewModels;

namespace AppsPortal.Areas.DAS.Controllers
{
    public class DocumentCabinetShelfController : DASBaseController
    {
        #region DocumentCabinetShelf

        public ActionResult Index()
        {
            return View();
        }

        [Route("DAS/DocumentCabinetShelfs/")]
        public ActionResult DocumentCabinetShelfsIndex()
        {
            if (!CMS.HasAction(Permissions.DocumentCabinetShelf.Access, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/DAS/Views/DocumentCabinetShelfs/Index.cshtml");
        }

        //[Route("DAS/DocumentCabinetShelfsDataTable/")]
        public JsonResult DocumentCabinetShelfsDataTable(DataTableRecievedOptions options)
        {
            var app = DbDAS.codeDASDocumentCabinetShelf.ToList();
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<DocumentCabinetShelfsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<DocumentCabinetShelfsDataTableModel>(DataTable.Filters);
            }
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.RefugeeScannedDocument.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();


            var All = (from a in DbDAS.codeDASDocumentCabinetShelf.AsExpandable()
                       join b in DbDAS.codeDASDocumentCabinet.Where(x => AuthorizedList.Contains(x.OrganizationInstanceGUID + "," + x.DutyStationGUID)) on a.DocumentCabinetGUID equals b.DocumentCabinetGUID
                       select new DocumentCabinetShelfsDataTableModel
                       {
                          DocumentCabinetShelfGUID=a.DocumentCabinetShelfGUID,
                          Active=a.Active,
                          codeDASDocumentCabinetShelfRowVersion=a.codeDASDocumentCabinetShelfRowVersion,
                          Archived=a.Active,
                          DocumentCabinetGUID=a.DocumentCabinetGUID,
                          MaxStorage=a.MaxStorage,
                          ShelfNumber=a.ShelfNumber
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<DocumentCabinetShelfsDataTableModel> Result = Mapper.Map<List<DocumentCabinetShelfsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("DAS/DocumentCabinetShelfs/Create/")]
        public ActionResult DocumentCabinetShelfCreate()
        {
            if (!CMS.HasAction(Permissions.DocumentCabinetShelf.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/DAS/Views/DocumentCabinetShelfs/DocumentCabinetShelf.cshtml", new DocumentCabinetShelfUpdateModel());
        }

        [Route("DAS/DocumentCabinetShelfs/Update/{PK}")]
        public ActionResult DocumentCabinetShelfUpdate(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.DocumentCabinetShelf.Access, Apps.DAS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var model = (from a in DbDAS.codeDASDocumentCabinetShelf.Where(x=>x.DocumentCabinetShelfGUID==PK)
                         select new DocumentCabinetShelfUpdateModel
                         {
                             DocumentCabinetShelfGUID = a.DocumentCabinetShelfGUID,
                             Active = a.Active,
                             codeDASDocumentCabinetShelfRowVersion = a.codeDASDocumentCabinetShelfRowVersion,
                             ShelfNumber = a.ShelfNumber,
                             MaxStorage = a.MaxStorage,
                             DocumentCabinetGUID = a.DocumentCabinetGUID,
                             Archived = a.Archived
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("DocumentCabinetShelf", "DocumentCabinetShelfs", new { Area = "DAS" }));

            return View("~/Areas/DAS/Views/DocumentCabinetShelfs/DocumentCabinetShelf.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DocumentCabinetShelfCreate(DocumentCabinetShelfUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.DocumentCabinetShelf.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveDocumentCabinetShelf(model)) return PartialView("~/Areas/DAS/Views/DocumentCabinetShelfs/_DocumentCabinetShelfForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeDASDocumentCabinetShelf DocumentCabinetShelf = Mapper.Map(model, new codeDASDocumentCabinetShelf());
            DocumentCabinetShelf.DocumentCabinetShelfGUID = EntityPK;
            DbDAS.Create(DocumentCabinetShelf, Permissions.DocumentCabinetShelf.CreateGuid, ExecutionTime, DbCMS);


            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.DocumentCabinetShelf.Create, Apps.DAS, new UrlHelper(Request.RequestContext).Action("Create", "DocumentCabinetShelfs", new { Area = "DAS" })), Container = "DocumentCabinetShelfFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.DocumentCabinetShelf.Update, Apps.DAS), Container = "DocumentCabinetShelfFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.DocumentCabinetShelf.Delete, Apps.DAS), Container = "DocumentCabinetShelfFormControls" });

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleCreateMessage(DbDAS.PrimaryKeyControl(DocumentCabinetShelf), DbDAS.RowVersionControls(Portal.SingleToList(DocumentCabinetShelf)), null, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DocumentCabinetShelfUpdate(DocumentCabinetShelfUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.DocumentCabinetShelf.Update, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveDocumentCabinetShelf(model)) return PartialView("~/Areas/DAS/Views/DocumentCabinetShelfs/_DocumentCabinetShelfForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeDASDocumentCabinetShelf DocumentCabinetShelf = Mapper.Map(model, new codeDASDocumentCabinetShelf());
            DbDAS.Update(DocumentCabinetShelf, Permissions.DocumentCabinetShelf.UpdateGuid, ExecutionTime, DbCMS);
            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleUpdateMessage(null, null, DbDAS.RowVersionControls(DbDAS.RowVersionControls(Portal.SingleToList(DocumentCabinetShelf)))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyDocumentCabinetShelf(model.DocumentCabinetShelfGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DocumentCabinetShelfDelete(codeDASDocumentCabinetShelf model)
        {
            if (!CMS.HasAction(Permissions.DocumentCabinetShelf.Delete, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDASDocumentCabinetShelf> DeletedDocumentCabinetShelf = DeleteDocumentCabinetShelfs(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.DocumentCabinetShelf.Restore, Apps.DAS), Container = "DocumentCabinetShelfFormControls" });

            try
            {
                int CommitedRows = DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleDeleteMessage(CommitedRows, DeletedDocumentCabinetShelf.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyDocumentCabinetShelf(model.DocumentCabinetShelfGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DocumentCabinetShelfRestore(codeDASDocumentCabinetShelf model)
        {
            if (!CMS.HasAction(Permissions.DocumentCabinetShelf.Restore, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveDocumentCabinetShelf(model))
            {
                return Json(DbDAS.RecordExists());
            }

            List<codeDASDocumentCabinetShelf> RestoredDocumentCabinetShelfs = RestoreDocumentCabinetShelfs(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.DocumentCabinetShelf.Create, Apps.DAS, new UrlHelper(Request.RequestContext).Action("DocumentCabinetShelfs/Create", "DocumentCabinetShelf", new { Area = "DAS" })), Container = "DocumentCabinetShelfFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.DocumentCabinetShelf.Update, Apps.DAS), Container = "DocumentCabinetShelfFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.DocumentCabinetShelf.Delete, Apps.DAS), Container = "DocumentCabinetShelfFormControls" });

            try
            {
                int CommitedRows = DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleRestoreMessage(CommitedRows, RestoredDocumentCabinetShelfs, DbDAS.PrimaryKeyControl(RestoredDocumentCabinetShelfs.FirstOrDefault()), null, null, UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyDocumentCabinetShelf(model.DocumentCabinetShelfGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DocumentCabinetShelfsDataTableDelete(List<codeDASDocumentCabinetShelf> models)
        {
            if (!CMS.HasAction(Permissions.DocumentCabinetShelf.Delete, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDASDocumentCabinetShelf> DeletedDocumentCabinetShelfs = DeleteDocumentCabinetShelfs(models);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.PartialDeleteMessage(DeletedDocumentCabinetShelfs, models, DataTableNames.DocumentCabinetShelfsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DocumentCabinetShelfsDataTableRestore(List<codeDASDocumentCabinetShelf> models)
        {
            if (!CMS.HasAction(Permissions.DocumentCabinetShelf.Restore, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDASDocumentCabinetShelf> RestoredDocumentCabinetShelfs = RestoreDocumentCabinetShelfs(models);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.PartialRestoreMessage(RestoredDocumentCabinetShelfs, models, DataTableNames.DocumentCabinetShelfsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        private List<codeDASDocumentCabinetShelf> DeleteDocumentCabinetShelfs(List<codeDASDocumentCabinetShelf> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<codeDASDocumentCabinetShelf> DeletedDocumentCabinetShelfs = new List<codeDASDocumentCabinetShelf>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbDAS.QueryBuilder(models, Permissions.DocumentCabinetShelf.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbDAS.Database.SqlQuery<codeDASDocumentCabinetShelf>(query).ToList();
            foreach (var record in Records)
            {
                DeletedDocumentCabinetShelfs.Add(DbDAS.Delete(record, ExecutionTime, Permissions.DocumentCabinetShelf.DeleteGuid, DbCMS));
            }

           
            return DeletedDocumentCabinetShelfs;
        }

        private List<codeDASDocumentCabinetShelf> RestoreDocumentCabinetShelfs(List<codeDASDocumentCabinetShelf> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<codeDASDocumentCabinetShelf> RestoredDocumentCabinetShelfs = new List<codeDASDocumentCabinetShelf>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbDAS.QueryBuilder(models, Permissions.DocumentCabinetShelf.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbDAS.Database.SqlQuery<codeDASDocumentCabinetShelf>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveDocumentCabinetShelf(record))
                {
                    RestoredDocumentCabinetShelfs.Add(DbDAS.Restore(record, Permissions.DocumentCabinetShelf.DeleteGuid, Permissions.DocumentCabinetShelf.RestoreGuid, RestoringTime, DbCMS));
                }
            }

           

            return RestoredDocumentCabinetShelfs;
        }

        private JsonResult ConcurrencyDocumentCabinetShelf(Guid PK)
        {
            DocumentCabinetShelfUpdateModel dbModel = new DocumentCabinetShelfUpdateModel();

            var DocumentCabinetShelf = DbDAS.codeDASDocumentCabinetShelf.Where(x => x.DocumentCabinetShelfGUID == PK).FirstOrDefault();
            var dbDocumentCabinetShelf = DbDAS.Entry(DocumentCabinetShelf).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbDocumentCabinetShelf, dbModel);

            if (DocumentCabinetShelf.codeDASDocumentCabinetShelfRowVersion.SequenceEqual(dbModel.codeDASDocumentCabinetShelfRowVersion))
            {
                return Json(DbDAS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbDAS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveDocumentCabinetShelf(Object model)
        {
            codeDASDocumentCabinetShelf DocumentCabinetShelf = Mapper.Map(model, new codeDASDocumentCabinetShelf());
            int DocumentCabinetShelfDescription = DbDAS.codeDASDocumentCabinetShelf
                                    .Where(x => x.ShelfNumber == DocumentCabinetShelf.ShelfNumber &&
                                    x.DocumentCabinetShelfGUID!=DocumentCabinetShelf.DocumentCabinetShelfGUID &&
                                    x.DocumentCabinetGUID ==DocumentCabinetShelf.DocumentCabinetGUID &&
                                                x.Active).Count();
            if (DocumentCabinetShelfDescription > 0)
            {
                ModelState.AddModelError("DocumentCabinetShelfDescription", "Shelf is already exists");
            }
            return (DocumentCabinetShelfDescription > 0);
        }

        #endregion
    }
}