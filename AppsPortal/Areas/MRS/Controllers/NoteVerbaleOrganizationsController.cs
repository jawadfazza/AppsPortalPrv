using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using MRS_DAL.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.MRS.Controllers
{
    public class NoteVerbaleOrganizationsController : MRSBaseController
    {
        #region NoteVerbale Route 
        //[Route("MRS/NoteVerbaleOrganizationsDataTable/{PK}")]
        public ActionResult NoteVerbaleOrganizationsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/MRS/Views/NoteVerbaleOrganizations/_Index.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<dataNoteVerbaleOrganization, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<dataNoteVerbaleOrganization>(DataTable.Filters);
            }

            var Result = (from a in DbMRS.dataNoteVerbaleOrganization.AsNoTracking().AsExpandable().Where(x => x.NoteVerbaleGUID == PK).Where(Predicate) 
                          join b in DbMRS.codeOrganizationsInstancesLanguages.Where(x=>x.Active && x.LanguageID==LAN) on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          select new NoteVerbaleOrganizationsDataTableModel
                          {
                              NoteVerbaleOrganizationGUID = a.NoteVerbaleOrganizationGUID,
                              OrganizationInstanceDescription =R1.OrganizationInstanceDescription,
                              OrganizationInstanceGUID=a.OrganizationInstanceGUID,
                              NoteVerbaleGUID = a.NoteVerbaleGUID,
                              Active = a.Active,
                              dataNoteVerbaleOrganizationRowVersion = a.dataNoteVerbaleOrganizationRowVersion
                          });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult NoteVerbaleOrganizationCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Create, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/MRS/Views/NoteVerbaleOrganizations/_NoteVerbaleOrganizationForm.cshtml", new NoteVerbaleOrganizationUpdateModel { NoteVerbaleGUID = FK });

        }

        public ActionResult NoteVerbaleOrganizationUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Access, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            var Organization = (from a in DbMRS.dataNoteVerbaleOrganization.Where(x => x.NoteVerbaleOrganizationGUID == PK)
                         select new NoteVerbaleOrganizationUpdateModel
                         {
                             Active=a.Active,
                             NoteVerbaleOrganizationGUID=a.NoteVerbaleOrganizationGUID,
                             OrganizationInstanceGUID=a.OrganizationInstanceGUID,
                             dataNoteVerbaleOrganizationRowVersion=a.dataNoteVerbaleOrganizationRowVersion,
                             NoteVerbaleGUID=a.NoteVerbaleGUID,
                         }
                       ).FirstOrDefault();
            return PartialView("~/Areas/MRS/Views/NoteVerbaleOrganizations/_NoteVerbaleOrganizationForm.cshtml", Organization);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NoteVerbaleOrganizationCreate(dataNoteVerbaleOrganization model)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Create, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

           
            if (!ModelState.IsValid || ActiveNoteVerbaleOrganization(model)) return PartialView("~/Areas/MRS/Views/NoteVerbaleOrganizations/_NoteVerbaleOrganizationForm.cshtml", model);
            model.NoteVerbaleOrganizationGUID = Guid.NewGuid();
            DateTime ExecutionTime = DateTime.Now;

            DbMRS.Create(model, Permissions.NoteVerbale.CreateGuid, ExecutionTime, DbCMS);
            try
            {
                DbMRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbMRS.SingleUpdateMessage(DataTableNames.NoteVerbaleOrganizationsDataTable, DbMRS.PrimaryKeyControl(model), DbMRS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbMRS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NoteVerbaleOrganizationUpdate(dataNoteVerbaleOrganization model)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Update, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            //dataNoteVerbaleOrganization NoteVerbaleOrganization = Mapper.Map(model,DbMRS.dataNoteVerbaleOrganization.Where(x=>x.NoteVerbaleOrganizationGUID==model.NoteVerbaleOrganizationGUID).FirstOrDefault());
            if (!ModelState.IsValid ) return PartialView("~/Areas/MRS/Views/NoteVerbaleOrganizations/_Index.cshtml", model);
            DateTime ExecutionTime = DateTime.Now;
            DbMRS.Update(model, Permissions.NoteVerbale.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbMRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbMRS.SingleUpdateMessage(DataTableNames.NoteVerbaleOrganizationsDataTable,
                    DbMRS.PrimaryKeyControl(model),
                    DbMRS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyNoteVerbaleOrganization(model.NoteVerbaleOrganizationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbMRS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NoteVerbaleOrganizationDelete(dataNoteVerbaleOrganization model)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Delete, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataNoteVerbaleOrganization> DeletedLanguages = DeleteNoteVerbaleOrganizations(new List<dataNoteVerbaleOrganization> { model });

            try
            {
                DbMRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbMRS.SingleDeleteMessage(DeletedLanguages, DataTableNames.NoteVerbaleOrganizationsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyNoteVerbaleOrganization(model.NoteVerbaleOrganizationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbMRS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NoteVerbaleOrganizationRestore(dataNoteVerbaleOrganization model)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Restore, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveNoteVerbaleOrganization(model))
            {
                return Json(DbMRS.RecordExists());
            }

            List<dataNoteVerbaleOrganization> RestoredLanguages = RestoreNoteVerbaleOrganizations(Portal.SingleToList(model));

            try
            {
                DbMRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbMRS.SingleRestoreMessage(RestoredLanguages, DataTableNames.NoteVerbaleOrganizationsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyNoteVerbaleOrganization(model.NoteVerbaleOrganizationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbMRS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult NoteVerbaleOrganizationsDataTableDelete(List<dataNoteVerbaleOrganization> models)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Delete, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataNoteVerbaleOrganization> DeletedLanguages = DeleteNoteVerbaleOrganizations(models);

            try
            {
                DbMRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbMRS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.NoteVerbaleOrganizationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbMRS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult NoteVerbaleOrganizationsDataTableRestore(List<dataNoteVerbaleOrganization> models)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Restore, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataNoteVerbaleOrganization> RestoredLanguages = RestoreNoteVerbaleOrganizations(models);

            try
            {
                DbMRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbMRS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.NoteVerbaleOrganizationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbMRS.ErrorMessage(ex.Message));
            }
        }

        private List<dataNoteVerbaleOrganization> DeleteNoteVerbaleOrganizations(List<dataNoteVerbaleOrganization> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataNoteVerbaleOrganization> DeletedNoteVerbaleOrganizations = new List<dataNoteVerbaleOrganization>();

            string query = DbMRS.QueryBuilder(models, Permissions.NoteVerbale.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbMRS.Database.SqlQuery<dataNoteVerbaleOrganization>(query).ToList();

            foreach (var language in languages)
            {
                DeletedNoteVerbaleOrganizations.Add(DbMRS.Delete(language, ExecutionTime, Permissions.NoteVerbale.DeleteGuid, DbCMS));
            }

            return DeletedNoteVerbaleOrganizations;
        }

        private List<dataNoteVerbaleOrganization> RestoreNoteVerbaleOrganizations(List<dataNoteVerbaleOrganization> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataNoteVerbaleOrganization> RestoredLanguages = new List<dataNoteVerbaleOrganization>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbMRS.QueryBuilder(models, Permissions.NoteVerbale.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbMRS.Database.SqlQuery<dataNoteVerbaleOrganization>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveNoteVerbaleOrganization(language))
                {
                    RestoredLanguages.Add(DbMRS.Restore(language, Permissions.NoteVerbale.DeleteGuid, Permissions.NoteVerbale.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyNoteVerbaleOrganization(Guid PK)
        {
            dataNoteVerbaleOrganization dbModel = new dataNoteVerbaleOrganization();

            var Language = DbMRS.dataNoteVerbaleOrganization.Where(l => l.NoteVerbaleOrganizationGUID == PK).FirstOrDefault();
            var dbLanguage = DbMRS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataNoteVerbaleOrganizationRowVersion.SequenceEqual(dbModel.dataNoteVerbaleOrganizationRowVersion))
            {
                return Json(DbMRS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbMRS, dbModel, "NoteVerbaleOrganizationsContainer"));
        }

        private bool ActiveNoteVerbaleOrganization(dataNoteVerbaleOrganization model)
        {
            int LanguageID = DbMRS.dataNoteVerbaleOrganization
                                  .Where(x => x.NoteVerbaleGUID==model.NoteVerbaleGUID &&
                                              x.OrganizationInstanceGUID == model.OrganizationInstanceGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("EndLocationGUID", "NoteVerbale Route already exists"); 
            }

            return (LanguageID > 0);
        }



        #endregion
    }
}