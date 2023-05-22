using DAS_DAL.Model;
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


namespace AppsPortal.Areas.DAS.Controllers
{
    public class DocumentLinksController : DASBaseController
    {


        #region Document Link

        //[Route("DAS/DocumentLinksDataTable/{PK}")]
        public ActionResult DocumentLinksDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/DAS/Views/ArchivedDocument/DocumentLink/_DocumentLinksDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<dataArchiveTemplateDocumentLink, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<dataArchiveTemplateDocumentLink>(DataTable.Filters);
            }

            var Result =(from a in DbDAS.dataArchiveTemplateDocumentLink.AsNoTracking().AsExpandable().Where(x => x.ScannDocumentGUID == PK).Where(Predicate)
                         join b in DbDAS.dataArchiveTemplateDocument on a.ScannDocumentLinkedWithGUID equals b.FileReferenceGUID select 
                               new
                              {
                                  a.ScannDocumentLinkGUID,
                                  a.ScannDocumentLinkedWithGUID,
                                  a.ScannDocumentGUID,
                                  b.FileReferenceName,
                                  a.Comment,
                                  a.dataArchiveTemplateDocumentLinkRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DocumentLinkCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.AppointmentType.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/DAS/Views/ArchivedDocument/DocumentLink/_DocumentLinksUpdateModal.cshtml",
                new dataArchiveTemplateDocumentLink { ScannDocumentGUID = FK });
        }

        public ActionResult DocumentLinkUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.AppointmentType.Access, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/DAS/Views/ArchivedDocument/DocumentLink/_DocumentLinksUpdateModal.cshtml", DbDAS.dataArchiveTemplateDocumentLink.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DocumentLinkCreate(dataArchiveTemplateDocumentLink model)
        {
            if (!CMS.HasAction(Permissions.AppointmentType.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveDocumentLink(model)) return PartialView("~/Areas/DAS/Views/ArchivedDocument/DocumentLink/_DocumentLinksUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbDAS.Create(model, Permissions.AppointmentType.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleUpdateMessage(DataTableNames.DocumentLinksDataTable, DbDAS.PrimaryKeyControl(model), DbDAS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DocumentLinkUpdate(dataArchiveTemplateDocumentLink model)
        {
            if (!CMS.HasAction(Permissions.AppointmentType.Update, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveDocumentLink(model)) return PartialView("~/Areas/DAS/Views/ArchivedDocument/DocumentLink/_DocumentLinksUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbDAS.Update(model, Permissions.AppointmentType.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleUpdateMessage(DataTableNames.DocumentLinksDataTable,
                    DbDAS.PrimaryKeyControl(model),
                    DbDAS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyDocumentLink(model.ScannDocumentLinkGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DocumentLinkDelete(dataArchiveTemplateDocumentLink model)
        {
            if (!CMS.HasAction(Permissions.AppointmentType.Delete, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataArchiveTemplateDocumentLink> DeletedLinks = DeleteDocumentLinks(new List<dataArchiveTemplateDocumentLink> { model });

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleDeleteMessage(DeletedLinks, DataTableNames.DocumentLinksDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyDocumentLink(model.ScannDocumentLinkGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DocumentLinkRestore(dataArchiveTemplateDocumentLink model)
        {
            if (!CMS.HasAction(Permissions.AppointmentType.Restore, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveDocumentLink(model))
            {
                return Json(DbDAS.RecordExists());
            }

            List<dataArchiveTemplateDocumentLink> RestoredLinks = RestoreDocumentLinks(Portal.SingleToList(model));

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleRestoreMessage(RestoredLinks, DataTableNames.DocumentLinksDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyDocumentLink(model.ScannDocumentLinkGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DocumentLinksDataTableDelete(List<dataArchiveTemplateDocumentLink> models)
        {
            if (!CMS.HasAction(Permissions.AppointmentType.Delete, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataArchiveTemplateDocumentLink> DeletedLinks = DeleteDocumentLinks(models);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.PartialDeleteMessage(DeletedLinks, models, DataTableNames.DocumentLinksDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DocumentLinksDataTableRestore(List<dataArchiveTemplateDocumentLink> models)
        {
            if (!CMS.HasAction(Permissions.AppointmentType.Restore, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataArchiveTemplateDocumentLink> RestoredLinks = RestoreDocumentLinks(models);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.PartialRestoreMessage(RestoredLinks, models, DataTableNames.DocumentLinksDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        private List<dataArchiveTemplateDocumentLink> DeleteDocumentLinks(List<dataArchiveTemplateDocumentLink> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataArchiveTemplateDocumentLink> DeletedDocumentLinks = new List<dataArchiveTemplateDocumentLink>();

            string query = DbDAS.QueryBuilder(models, Permissions.AppointmentType.DeleteGuid, SubmitTypes.Delete, "");

            var Links = DbDAS.Database.SqlQuery<dataArchiveTemplateDocumentLink>(query).ToList();

            foreach (var Link in Links)
            {
                DeletedDocumentLinks.Add(DbDAS.Delete(Link, ExecutionTime, Permissions.AppointmentType.DeleteGuid, DbCMS));
            }

            return DeletedDocumentLinks;
        }

        private List<dataArchiveTemplateDocumentLink> RestoreDocumentLinks(List<dataArchiveTemplateDocumentLink> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataArchiveTemplateDocumentLink> RestoredLinks = new List<dataArchiveTemplateDocumentLink>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbDAS.QueryBuilder(models, Permissions.AppointmentType.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Links = DbDAS.Database.SqlQuery<dataArchiveTemplateDocumentLink>(query).ToList();
            foreach (var Link in Links)
            {
                if (!ActiveDocumentLink(Link))
                {
                    RestoredLinks.Add(DbDAS.Restore(Link, Permissions.AppointmentType.DeleteGuid, Permissions.AppointmentType.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLinks;
        }

        private JsonResult ConcrrencyDocumentLink(Guid PK)
        {
            dataArchiveTemplateDocumentLink dbModel = new dataArchiveTemplateDocumentLink();

            var Link = DbDAS.dataArchiveTemplateDocumentLink.Where(l => l.ScannDocumentLinkGUID == PK).FirstOrDefault();
            var dbLink = DbDAS.Entry(Link).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLink, dbModel);

            if (Link.dataArchiveTemplateDocumentLinkRowVersion.SequenceEqual(dbModel.dataArchiveTemplateDocumentLinkRowVersion))
            {
                return Json(DbDAS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbDAS, dbModel, "LinksContainer"));
        }

        private bool ActiveDocumentLink(dataArchiveTemplateDocumentLink model)
        {
            int LinkID =0;
            if (LinkID > 0)
            {
                ModelState.AddModelError("ScannDocumentLinkedWithGUID", "File selected already linked"); //From resource ?????? Amer  
            }

            return (LinkID > 0);
        }

        #endregion
    }
}