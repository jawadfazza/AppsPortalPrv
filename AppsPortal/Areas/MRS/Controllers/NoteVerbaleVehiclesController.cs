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
using System.Web;
using System.Web.Mvc;
using MRS_DAL.Model;

namespace AppsPortal.Areas.MRS.Controllers
{
    public class NoteVerbaleVehiclesController : MRSBaseController
    {
        #region NoteVerbale Vehicles

        //[Route("MRS/NoteVerbaleVehiclesDataTable/{PK}")]
        public ActionResult NoteVerbaleVehiclesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/MRS/Views/NoteVerbaleVehicles/_Index.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<NoteVerbaleVehicleDataTable, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<NoteVerbaleVehicleDataTable>(DataTable.Filters);
            }

            var Result = (from a in DbMRS.dataNoteVerbaleVehicle.AsNoTracking().AsExpandable().Where(x => x.NoteVerbaleGUID == PK)
                          join b in DbMRS.codeVehicle on a.VehicleGUID equals b.VehicleGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          join c in DbMRS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.UserGUID equals c.UserGUID into LJ2
                          from R2 in LJ2.DefaultIfEmpty()
                          select new NoteVerbaleVehicleDataTable
                          {
                              NoteVerbaleVehicleGUID = a.NoteVerbaleVehicleGUID,
                              NoteVerbaleGUID = a.NoteVerbaleGUID,
                              VehicleNumber= R1.VehicleNumber,
                              VehicleGUID=a.VehicleGUID,
                              UserGUID = R2 .FirstName + " " + R2.Surname,
                              Active = a.Active,
                              dataNoteVerbaleVehicleRowVersion = a.dataNoteVerbaleVehicleRowVersion,
                          }).Where(Predicate);

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult NoteVerbaleVehicleCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Create, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/MRS/Views/NoteVerbaleVehicles/_NoteVerbaleVehicleForm.cshtml",
                new dataNoteVerbaleVehicle { NoteVerbaleGUID = FK });
        }

        public ActionResult NoteVerbaleVehicleUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Access, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/MRS/Views/NoteVerbaleVehicles/_NoteVerbaleVehicleForm.cshtml", DbMRS.dataNoteVerbaleVehicle.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NoteVerbaleVehicleCreate(dataNoteVerbaleVehicle model)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Create, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveNoteVerbaleVehicle(model)) return PartialView("~/Areas/MRS/Views/NoteVerbaleVehicles/_NoteVerbaleVehicleForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbMRS.Create(model, Permissions.NoteVerbale.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbMRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbMRS.SingleUpdateMessage(DataTableNames.NoteVerbaleVehiclesDataTable, DbMRS.PrimaryKeyControl(model), DbMRS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbMRS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NoteVerbaleVehicleUpdate(dataNoteVerbaleVehicle model)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Update, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveNoteVerbaleVehicle(model)) return PartialView("~/Areas/MRS/Views/NoteVerbaleVehicles/_NoteVerbaleVehicleForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbMRS.Update(model, Permissions.NoteVerbale.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbMRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbMRS.SingleUpdateMessage(DataTableNames.NoteVerbaleVehiclesDataTable,
                    DbMRS.PrimaryKeyControl(model),
                    DbMRS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyNoteVerbaleVehicle(model.NoteVerbaleVehicleGUID);
            }
            catch (Exception ex)
            {
                return Json(DbMRS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NoteVerbaleVehicleDelete(dataNoteVerbaleVehicle model)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Delete, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataNoteVerbaleVehicle> Deleteds = DeleteNoteVerbaleVehicles(new List<dataNoteVerbaleVehicle> { model });

            try
            {
                DbMRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbMRS.SingleDeleteMessage(Deleteds, DataTableNames.NoteVerbaleVehiclesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyNoteVerbaleVehicle(model.NoteVerbaleVehicleGUID);
            }
            catch (Exception ex)
            {
                return Json(DbMRS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NoteVerbaleVehicleRestore(dataNoteVerbaleVehicle model)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Restore, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveNoteVerbaleVehicle(model))
            {
                return Json(DbMRS.RecordExists());
            }

            List<dataNoteVerbaleVehicle> Restoreds = RestoreNoteVerbaleVehicles(Portal.SingleToList(model));

            try
            {
                DbMRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbMRS.SingleRestoreMessage(Restoreds, DataTableNames.NoteVerbaleVehiclesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyNoteVerbaleVehicle(model.NoteVerbaleVehicleGUID);
            }
            catch (Exception ex)
            {
                return Json(DbMRS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult NoteVerbaleVehiclesDataTableDelete(List<dataNoteVerbaleVehicle> models)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Delete, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataNoteVerbaleVehicle> Deleteds = DeleteNoteVerbaleVehicles(models);

            try
            {
                DbMRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbMRS.PartialDeleteMessage(Deleteds, models, DataTableNames.NoteVerbaleVehiclesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbMRS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult NoteVerbaleVehiclesDataTableRestore(List<dataNoteVerbaleVehicle> models)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Restore, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataNoteVerbaleVehicle> Restoreds = RestoreNoteVerbaleVehicles(models);

            try
            {
                DbMRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbMRS.PartialRestoreMessage(Restoreds, models, DataTableNames.NoteVerbaleVehiclesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbMRS.ErrorMessage(ex.Message));
            }
        }

        private List<dataNoteVerbaleVehicle> DeleteNoteVerbaleVehicles(List<dataNoteVerbaleVehicle> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataNoteVerbaleVehicle> DeletedNoteVerbaleVehicles = new List<dataNoteVerbaleVehicle>();

            string query = DbMRS.QueryBuilder(models, Permissions.NoteVerbale.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbMRS.Database.SqlQuery<dataNoteVerbaleVehicle>(query).ToList();

            foreach (var language in languages)
            {
                DeletedNoteVerbaleVehicles.Add(DbMRS.Delete(language, ExecutionTime, Permissions.NoteVerbale.DeleteGuid, DbCMS));
            }

            return DeletedNoteVerbaleVehicles;
        }

        private List<dataNoteVerbaleVehicle> RestoreNoteVerbaleVehicles(List<dataNoteVerbaleVehicle> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataNoteVerbaleVehicle> Restoreds = new List<dataNoteVerbaleVehicle>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbMRS.QueryBuilder(models, Permissions.NoteVerbale.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var NoteVerbaleVehicles = DbMRS.Database.SqlQuery<dataNoteVerbaleVehicle>(query).ToList();
            foreach (var language in NoteVerbaleVehicles)
            {
                if (!ActiveNoteVerbaleVehicle(language))
                {
                    Restoreds.Add(DbMRS.Restore(language, Permissions.NoteVerbale.DeleteGuid, Permissions.NoteVerbale.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return Restoreds;
        }

        private JsonResult ConcrrencyNoteVerbaleVehicle(Guid PK)
        {
            dataNoteVerbaleVehicle dbModel = new dataNoteVerbaleVehicle();

            var NoteVerbaleVehicle = DbMRS.dataNoteVerbaleVehicle.Where(l => l.NoteVerbaleVehicleGUID == PK).FirstOrDefault();
            var dbNoteVerbaleVehicle = DbMRS.Entry(NoteVerbaleVehicle).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbNoteVerbaleVehicle, dbModel);

            if (NoteVerbaleVehicle.dataNoteVerbaleVehicleRowVersion.SequenceEqual(dbModel.dataNoteVerbaleVehicleRowVersion))
            {
                return Json(DbMRS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbMRS, dbModel, "TabContainer"));
        }

        private bool ActiveNoteVerbaleVehicle(dataNoteVerbaleVehicle model)
        {
            int NoteVerbaleDriverID = DbMRS.dataNoteVerbaleVehicle
                                  .Where(x => x.NoteVerbaleVehicleGUID !=model.NoteVerbaleVehicleGUID &&
                                  x.NoteVerbaleGUID == model.NoteVerbaleGUID &&
                                  x.UserGUID == model.UserGUID &&
                                              x.Active).Count();
            if (NoteVerbaleDriverID > 0)
            {
                ModelState.AddModelError("UserGUID", "Driver already exists"); //From resource ?????? Amer  
            }

            int NoteVerbaleVehicleID = DbMRS.dataNoteVerbaleVehicle
                                  .Where(x => x.NoteVerbaleVehicleGUID != model.NoteVerbaleVehicleGUID &&
                                     x.NoteVerbaleGUID == model.NoteVerbaleGUID &&
                                  x.VehicleGUID.Value == model.VehicleGUID.Value &&
                                              x.Active).Count();
            if (NoteVerbaleVehicleID > 0)
            {
                ModelState.AddModelError("VehicleGUID", "Vehicle already exists"); //From resource ?????? Amer  
            }

            return (NoteVerbaleVehicleID + NoteVerbaleDriverID > 0);
        }

        #endregion
    }
}