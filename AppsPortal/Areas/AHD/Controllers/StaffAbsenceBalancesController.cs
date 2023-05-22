using AHD_DAL.Model;
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
using iTextSharp.text.pdf.qrcode;

namespace AppsPortal.Areas.AHD.Controllers
{
    public class StaffAbsenceBalancesController : AHDBaseController
    {
     

        #region Shuttle Request Route Step

        //[Route("AHD/StaffAbsenceBalancesDataTable/{PK}")]
        public ActionResult StaffAbsenceBalancesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/AHD/Views/StaffAbsence/_StaffAbsenceBalancesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<dataStaffAbsenceBalance, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<dataStaffAbsenceBalance>(DataTable.Filters);
            }

            var Result =
                (from a in DbAHD.dataStaffAbsenceBalance.AsExpandable().Where(x => x.UserGUID == PK).Where(Predicate)
                 join b in DbAHD.codeTablesValuesLanguages.Where(x => x.LanguageID ==LAN && x.Active) on a.AbsenceTypeGuid equals b.ValueGUID
                 select new
                 {
                     StaffAbsenceBalanceGUID= a.StaffAbsenceBalanceGUID,
                     Balance=a.Balance,
                     AbsenceType= b.ValueDescription,
                     a.MaxBalancePerYear,
                     a.ResetDate,
                     a.Active,
                     a.dataStaffAbsenceBalanceRowVersion
                 }) ;

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult StaffAbsenceBalanceCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.StaffAbsenceBalance.Create, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/AHD/Views/StaffAbsence/_StaffAbsenceBalanceUpdateModal.cshtml",
                new dataStaffAbsenceBalance { UserGUID = FK });
        }

        public ActionResult StaffAbsenceBalanceUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.StaffAbsenceBalance.Access, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/AHD/Views/StaffAbsence/_StaffAbsenceBalanceUpdateModal.cshtml", DbAHD.dataStaffAbsenceBalance.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffAbsenceBalanceCreate(dataStaffAbsenceBalance model)
        {
            if (!CMS.HasAction(Permissions.StaffAbsenceBalance.Create, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveStaffAbsenceBalance(model)) return PartialView("~/Areas/AHD/Views/StaffAbsence/_StaffAbsenceBalanceUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            model.InitialBalance = model.Balance;
            DbAHD.Create(model, Permissions.StaffAbsenceBalance.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.StaffAbsenceBalancesDataTable, DbAHD.PrimaryKeyControl(model), DbAHD.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffAbsenceBalanceUpdate(dataStaffAbsenceBalance model)
        {
            if (!CMS.HasAction(Permissions.StaffAbsenceBalance.Update, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid ) return PartialView("~/Areas/AHD/Views/StaffAbsence/_StaffAbsenceBalanceUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbAHD.Update(model, Permissions.StaffAbsenceBalance.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.StaffAbsenceBalancesDataTable,
                    DbAHD.PrimaryKeyControl(model),
                    DbAHD.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffAbsenceBalance(model.StaffAbsenceBalanceGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffAbsenceBalanceDelete(dataStaffAbsenceBalance model)
        {
            if (!CMS.HasAction(Permissions.StaffAbsenceBalance.Delete, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataStaffAbsenceBalance> DeletedLanguages = DeleteStaffAbsenceBalances(new List<dataStaffAbsenceBalance> { model });

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleDeleteMessage(DeletedLanguages, DataTableNames.StaffAbsenceBalancesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffAbsenceBalance(model.StaffAbsenceBalanceGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffAbsenceBalanceRestore(dataStaffAbsenceBalance model)
        {
            if (!CMS.HasAction(Permissions.StaffAbsenceBalance.Restore, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveStaffAbsenceBalance(model))
            {
                return Json(DbAHD.RecordExists());
            }

            List<dataStaffAbsenceBalance> RestoredLanguages = RestoreStaffAbsenceBalances(Portal.SingleToList(model));

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleRestoreMessage(RestoredLanguages, DataTableNames.StaffAbsenceBalancesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyStaffAbsenceBalance(model.StaffAbsenceBalanceGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffAbsenceBalancesDataTableDelete(List<dataStaffAbsenceBalance> models)
        {
            if (!CMS.HasAction(Permissions.StaffAbsenceBalance.Delete, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataStaffAbsenceBalance> DeletedLanguages = DeleteStaffAbsenceBalances(models);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.StaffAbsenceBalancesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult StaffAbsenceBalancesDataTableRestore(List<dataStaffAbsenceBalance> models)
        {
            if (!CMS.HasAction(Permissions.StaffAbsenceBalance.Restore, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataStaffAbsenceBalance> RestoredLanguages = RestoreStaffAbsenceBalances(models);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.StaffAbsenceBalancesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        private List<dataStaffAbsenceBalance> DeleteStaffAbsenceBalances(List<dataStaffAbsenceBalance> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataStaffAbsenceBalance> DeletedStaffAbsenceBalances = new List<dataStaffAbsenceBalance>();

            string query = DbAHD.QueryBuilder(models, Permissions.StaffAbsenceBalance.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbAHD.Database.SqlQuery<dataStaffAbsenceBalance>(query).ToList();

            foreach (var language in languages)
            {
                DeletedStaffAbsenceBalances.Add(DbAHD.Delete(language, ExecutionTime, Permissions.StaffAbsenceBalance.DeleteGuid, DbCMS));
            }

            return DeletedStaffAbsenceBalances;
        }

        private List<dataStaffAbsenceBalance> RestoreStaffAbsenceBalances(List<dataStaffAbsenceBalance> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataStaffAbsenceBalance> RestoredLanguages = new List<dataStaffAbsenceBalance>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbAHD.QueryBuilder(models, Permissions.StaffAbsenceBalance.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbAHD.Database.SqlQuery<dataStaffAbsenceBalance>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveStaffAbsenceBalance(language))
                {
                    RestoredLanguages.Add(DbAHD.Restore(language, Permissions.StaffAbsenceBalance.DeleteGuid, Permissions.StaffAbsenceBalance.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyStaffAbsenceBalance(Guid PK)
        {
            dataStaffAbsenceBalance dbModel = new dataStaffAbsenceBalance();

            var Language = DbAHD.dataStaffAbsenceBalance.Where(l => l.StaffAbsenceBalanceGUID == PK).FirstOrDefault();
            var dbLanguage = DbAHD.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataStaffAbsenceBalanceRowVersion.SequenceEqual(dbModel.dataStaffAbsenceBalanceRowVersion))
            {
                return Json(DbAHD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbAHD, dbModel, "LanguagesContainer"));
        }

        private bool ActiveStaffAbsenceBalance(dataStaffAbsenceBalance model)
        {
            int LanguageID = DbAHD.dataStaffAbsenceBalance
                                  .Where(x => 
                                              x.UserGUID == model.UserGUID &&
                                              x.AbsenceTypeGuid == model.AbsenceTypeGuid &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Absence Type already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }

        #endregion
    }
}