using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AutoMapper;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using TTT_DAL.Model;
using AppsPortal.ViewModels;
using DocumentFormat.OpenXml.Packaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Data.Entity.Infrastructure;

namespace AppsPortal.Areas.TTT.Controllers
{
    public class BOCsController : TTTBaseController
    {
        public ActionResult Index()
        {
            if (!CMS.HasAction(Permissions.TenderBOCs.Access, Apps.TTT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/TTT/Views/BOCs/Index.cshtml");
        }

        [Route("TTT/BOCs/TenderBOCsDataTable/")]
        public JsonResult TenderBOCsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<TenderBOCsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<TenderBOCsDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbTTT.dataTenderBOCs.AsExpandable().Where(x => x.Active)
                       join b in DbTTT.userPersonalDetailsLanguage.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.UserGUID equals b.UserGUID
                       join c in DbTTT.codeDutyStationsLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.DutyStationGUID equals c.DutyStationGUID

                       select new TenderBOCsDataTableModel
                       {
                           TenderBOCGUID = a.TenderBOCGUID,
                           BOCStaff = b.FirstName + " " + b.Surname,
                           DutyStationDescription = c.DutyStationDescription,
                           dataTenderBOCsRowVersion = a.dataTenderBOCsRowVersion,

                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<TenderBOCsDataTableModel> Result = Mapper.Map<List<TenderBOCsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);

        }

        public ActionResult TenderBOCCreate()
        {
            if (!CMS.HasAction(Permissions.TenderBOCs.Create, Apps.TTT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/TTT/Views/BOCs/_TenderBOCUpdateModal.cshtml", new dataTenderBOCs());
        }

        public ActionResult TenderBOCUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.TenderBOCs.Access, Apps.TTT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/TTT/Views/BOCs/_TenderBOCUpdateModal.cshtml", DbTTT.dataTenderBOCs.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TenderBOCCreate(dataTenderBOCs model)
        {
            if (!CMS.HasAction(Permissions.TenderBOCs.Create, Apps.TTT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveTenderBOC(model)) return PartialView("~/Areas/TTT/Views/BOCs/_TenderBOCUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbTTT.Create(model, Permissions.TenderBOCs.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbTTT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTTT.SingleUpdateMessage(DataTableNames.TenderBOCsDataTable,
                   DbTTT.PrimaryKeyControl(model),
                   DbTTT.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbTTT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TenderBOCUpdate(dataTenderBOCs model)
        {
            if (!CMS.HasAction(Permissions.TenderBOCs.Update, Apps.TTT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveTenderBOC(model)) return PartialView("~/Areas/TTT/Views/BOCs/_TenderBOCUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbTTT.Update(model, Permissions.TenderBOCs.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbTTT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTTT.SingleUpdateMessage(DataTableNames.TenderBOCsDataTable,
                    DbTTT.PrimaryKeyControl(model),
                    DbTTT.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyTenderBOC(model.TenderBOCGUID);
            }
            catch (Exception ex)
            {
                return Json(DbTTT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TenderBOCDelete(dataTenderBOCs model)
        {
            if (!CMS.HasAction(Permissions.TenderBOCs.Delete, Apps.TTT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataTenderBOCs> DeletedBOCs = DeleteTenderBOC(new List<dataTenderBOCs> { model });

            try
            {
                DbTTT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTTT.SingleDeleteMessage(DeletedBOCs, DataTableNames.TenderBOCsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyTenderBOC(model.TenderBOCGUID);
            }
            catch (Exception ex)
            {
                return Json(DbTTT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TenderBOCRestore(dataTenderBOCs model)
        {
            if (!CMS.HasAction(Permissions.TenderBOCs.Restore, Apps.TTT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveTenderBOC(model))
            {
                return Json(DbCMS.RecordExists());
            }

            List<dataTenderBOCs> RestoredBOCs = RestoreTenderBOC(Portal.SingleToList(model));

            try
            {
                DbTTT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTTT.SingleRestoreMessage(RestoredBOCs, DataTableNames.TenderBOCsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyTenderBOC(model.TenderBOCGUID);
            }
            catch (Exception ex)
            {
                return Json(DbTTT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult TenderBOCsDataTableDelete(List<dataTenderBOCs> models)
        {
            if (!CMS.HasAction(Permissions.TenderBOCs.Delete, Apps.TTT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataTenderBOCs> DeletedBOCs = DeleteTenderBOC(models);

            try
            {
                DbTTT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTTT.PartialDeleteMessage(DeletedBOCs, models, DataTableNames.TenderBOCsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbTTT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult TenderBOCsDataTableRestore(List<dataTenderBOCs> models)
        {

            if (!CMS.HasAction(Permissions.TenderBOCs.Restore, Apps.TTT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataTenderBOCs> RestoredBOCs = RestoreTenderBOC(models);

            try
            {
                DbTTT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTTT.PartialRestoreMessage(RestoredBOCs, models, DataTableNames.TenderBOCsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbTTT.ErrorMessage(ex.Message));
            }
        }

        private List<dataTenderBOCs> DeleteTenderBOC(List<dataTenderBOCs> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataTenderBOCs> DeletedTenderBOCs = new List<dataTenderBOCs>();

            string query = DbTTT.QueryBuilder(models, Permissions.TenderBOCs.DeleteGuid, SubmitTypes.Delete, "");

            var TenderBOCs = DbTTT.Database.SqlQuery<dataTenderBOCs>(query).ToList();

            foreach (var tenderBOC in TenderBOCs)
            {
                DeletedTenderBOCs.Add(DbTTT.Delete(tenderBOC, ExecutionTime, Permissions.TenderBOCs.DeleteGuid, DbCMS));
            }

            return DeletedTenderBOCs;
        }

        private List<dataTenderBOCs> RestoreTenderBOC(List<dataTenderBOCs> models)
        {
            Guid DeleteActionGUID = Permissions.TenderBOCs.DeleteGuid;
            Guid RestoreActionGUID = Permissions.TenderBOCs.RestoreGuid;
            DateTime RestoringTime = DateTime.Now;

            List<dataTenderBOCs> RestoredTenderBOCs = new List<dataTenderBOCs>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbTTT.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var TenderBOCs = DbTTT.Database.SqlQuery<dataTenderBOCs>(query).ToList();
            foreach (var tenderBOC in TenderBOCs)
            {
                if (!ActiveTenderBOC(tenderBOC))
                {
                    RestoredTenderBOCs.Add(DbTTT.Restore(tenderBOC, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS));
                }
            }

            return RestoredTenderBOCs;
        }

        private JsonResult ConcrrencyTenderBOC(Guid PK)
        {
            dataTenderBOCs dbModel = new dataTenderBOCs();

            var TenderBOC = DbTTT.dataTenderBOCs.Where(x => x.TenderBOCGUID == PK).FirstOrDefault();
            var dbLanguage = DbTTT.Entry(TenderBOC).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (TenderBOC.dataTenderBOCsRowVersion.SequenceEqual(dbModel.dataTenderBOCsRowVersion))
            {
                return Json(DbTTT.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbTTT, dbModel, "LanguagesContainer"));
        }

        private bool ActiveTenderBOC(dataTenderBOCs model)
        {
            int LanguageID = DbTTT.dataTenderBOCs
                                  .Where(l => l.UserGUID == model.UserGUID &&
                                              l.DutyStationGUID == model.DutyStationGUID &&
                                              l.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("UserGUID", "Same member already exists in same duty station"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }
    }
}