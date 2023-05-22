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
    public class NoteVerbaleStaffsController : MRSBaseController
    {
        #region NoteVerbale Staffs

        //[Route("MRS/NoteVerbaleStaffsDataTable/{PK}")]
        public ActionResult NoteVerbaleStaffsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/MRS/Views/NoteVerbaleStaffs/_Index.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<NoteVerbaleStaffDataTable, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<NoteVerbaleStaffDataTable>(DataTable.Filters);
            }

            var Result = (from a in DbMRS.dataNoteVerbaleStaff.AsNoTracking().AsExpandable().Where(x => x.NoteVerbaleGUID == PK)
                          join b in DbMRS.userPersonalDetailsLanguage.Where(x=>x.Active && x.LanguageID==LAN)on a.UserGUID equals b.UserGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                         
                          select new NoteVerbaleStaffDataTable
                          {
                              NoteVerbaleStaffGUID = a.NoteVerbaleStaffGUID,
                              NoteVerbaleGUID=a.NoteVerbaleGUID,
                              UserGUID = R1.FirstName + " " +R1.Surname,
                              Sequance = a.Sequance,
                              Active = a.Active,
                              dataNoteVerbaleStaffRowVersion = a.dataNoteVerbaleStaffRowVersion,
                          }).Where(Predicate);

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult NoteVerbaleStaffCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Create, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var staffCount = DbMRS.dataNoteVerbaleStaff.Where(x => x.NoteVerbaleGUID == FK).Count() + 1;
            return PartialView("~/Areas/MRS/Views/NoteVerbaleStaffs/_NoteVerbaleStaffForm.cshtml",
                new dataNoteVerbaleStaff { NoteVerbaleGUID = FK ,Sequance=staffCount});
        }

        public ActionResult NoteVerbaleStaffUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Access, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/MRS/Views/NoteVerbaleStaffs/_NoteVerbaleStaffForm.cshtml", DbMRS.dataNoteVerbaleStaff.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NoteVerbaleStaffCreate(dataNoteVerbaleStaff model)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Create, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveNoteVerbaleStaff(model)) return PartialView("~/Areas/MRS/Views/NoteVerbaleStaffs/_NoteVerbaleStaffForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            var staffCount = DbMRS.dataNoteVerbaleStaff.Where(x => x.NoteVerbaleGUID == model.NoteVerbaleGUID).Count()+1;
            model.Sequance = staffCount;
            DbMRS.Create(model, Permissions.NoteVerbale.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbMRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbMRS.SingleUpdateMessage(DataTableNames.NoteVerbaleStaffsDataTable, DbMRS.PrimaryKeyControl(model), DbMRS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbMRS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NoteVerbaleStaffUpdate(dataNoteVerbaleStaff model)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Update, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid ) return PartialView("~/Areas/MRS/Views/NoteVerbaleStaffs/_NoteVerbaleStaffForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbMRS.Update(model, Permissions.NoteVerbale.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbMRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbMRS.SingleUpdateMessage(DataTableNames.NoteVerbaleStaffsDataTable,
                    DbMRS.PrimaryKeyControl(model),
                    DbMRS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyNoteVerbaleStaff(model.NoteVerbaleStaffGUID);
            }
            catch (Exception ex)
            {
                return Json(DbMRS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NoteVerbaleStaffDelete(dataNoteVerbaleStaff model)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Delete, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataNoteVerbaleStaff> Deleteds = DeleteNoteVerbaleStaffs(new List<dataNoteVerbaleStaff> { model });

            try
            {
                DbMRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbMRS.SingleDeleteMessage(Deleteds, DataTableNames.NoteVerbaleStaffsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyNoteVerbaleStaff(model.NoteVerbaleStaffGUID);
            }
            catch (Exception ex)
            {
                return Json(DbMRS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NoteVerbaleStaffRestore(dataNoteVerbaleStaff model)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Restore, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveNoteVerbaleStaff(model))
            {
                return Json(DbMRS.RecordExists());
            }

            List<dataNoteVerbaleStaff> Restoreds = RestoreNoteVerbaleStaffs(Portal.SingleToList(model));

            try
            {
                DbMRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbMRS.SingleRestoreMessage(Restoreds, DataTableNames.NoteVerbaleStaffsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyNoteVerbaleStaff(model.NoteVerbaleStaffGUID);
            }
            catch (Exception ex)
            {
                return Json(DbMRS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult NoteVerbaleStaffsDataTableDelete(List<dataNoteVerbaleStaff> models)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Delete, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataNoteVerbaleStaff> Deleteds = DeleteNoteVerbaleStaffs(models);

            try
            {
                DbMRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbMRS.PartialDeleteMessage(Deleteds, models, DataTableNames.NoteVerbaleStaffsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbMRS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult NoteVerbaleStaffsDataTableRestore(List<dataNoteVerbaleStaff> models)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Restore, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataNoteVerbaleStaff> Restoreds = RestoreNoteVerbaleStaffs(models);

            try
            {
                DbMRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbMRS.PartialRestoreMessage(Restoreds, models, DataTableNames.NoteVerbaleStaffsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbMRS.ErrorMessage(ex.Message));
            }
        }

        private List<dataNoteVerbaleStaff> DeleteNoteVerbaleStaffs(List<dataNoteVerbaleStaff> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataNoteVerbaleStaff> DeletedNoteVerbaleStaffs = new List<dataNoteVerbaleStaff>();

            string query = DbMRS.QueryBuilder(models, Permissions.NoteVerbale.DeleteGuid, SubmitTypes.Delete, "");

            var staffs = DbMRS.Database.SqlQuery<dataNoteVerbaleStaff>(query).ToList();

            foreach (var staff in staffs)
            {
                DeletedNoteVerbaleStaffs.Add(DbMRS.Delete(staff, ExecutionTime, Permissions.NoteVerbale.DeleteGuid, DbCMS));
              
            }

            return DeletedNoteVerbaleStaffs;
        }

        private List<dataNoteVerbaleStaff> RestoreNoteVerbaleStaffs(List<dataNoteVerbaleStaff> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataNoteVerbaleStaff> Restoreds = new List<dataNoteVerbaleStaff>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbMRS.QueryBuilder(models, Permissions.NoteVerbale.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var staffs = DbMRS.Database.SqlQuery<dataNoteVerbaleStaff>(query).ToList();
            foreach (var staff in staffs)
            {
                if (!ActiveNoteVerbaleStaff(staff))
                {
                    Restoreds.Add(DbMRS.Restore(staff, Permissions.NoteVerbale.DeleteGuid, Permissions.NoteVerbale.RestoreGuid, RestoringTime, DbCMS));
                  
                }
            }

            return Restoreds;
        }

        private JsonResult ConcrrencyNoteVerbaleStaff(Guid PK)
        {
            dataNoteVerbaleStaff dbModel = new dataNoteVerbaleStaff();

            var NoteVerbaleStaff = DbMRS.dataNoteVerbaleStaff.Where(l => l.NoteVerbaleStaffGUID == PK).FirstOrDefault();
            var dbNoteVerbaleStaff = DbMRS.Entry(NoteVerbaleStaff).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbNoteVerbaleStaff, dbModel);

            if (NoteVerbaleStaff.dataNoteVerbaleStaffRowVersion.SequenceEqual(dbModel.dataNoteVerbaleStaffRowVersion))
            {
                return Json(DbMRS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbMRS, dbModel, "TabContainer"));
        }

        private bool ActiveNoteVerbaleStaff(dataNoteVerbaleStaff model)
        {
            int NoteVerbaleStaffID = DbMRS.dataNoteVerbaleStaff
                                  .Where(x => x.NoteVerbaleGUID == model.NoteVerbaleGUID &&
                                              x.UserGUID == model.UserGUID &&
                                              x.Active).Count();
            if (NoteVerbaleStaffID > 0)
            {
                ModelState.AddModelError("UserGUID", "Staff already exists"); //From resource ?????? Amer  
            }

            return (NoteVerbaleStaffID > 0);
        }

        #endregion
    }
}