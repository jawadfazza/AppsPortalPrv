using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AutoMapper;
using IDC_DAL.Model;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.IDC.Controllers
{
    public class CardValidationsController : IDCBaseController
    {
        #region Card Issueds

        [Route("IDC/CardValidations/")]
        public ActionResult CardValidationsIndex()
        {
            if (!CMS.HasAction(Permissions.CardValidation.Access, Apps.IDC))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/IDC/Views/CardValidations/Index.cshtml");
        }

        [Route("IDC/CardValidationsDataTable/")]
        public JsonResult CardValidationsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<CardValidationsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<CardValidationsDataTableModel>(DataTable.Filters);
            }
            ///

            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.CardIssued.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var All = (from a in DbIDC.dataCardIndividualInfo.AsExpandable()
                       join b in DbIDC.dataCardIssued on a.CardIndividualInfoGUID equals b.CardIndividualInfoGUID
                       where !b.Approved.Value && b.PrintedBy == null && AuthorizedList.Contains("e156c022-ec72-4a5a-be09-163bd85c68ef," + b.DutyStationGUID.ToString())
                       select new CardValidationsDataTableModel
                       {
                           CardIssuedGUID = b.CardIssuedGUID,
                           CaseNumber = a.CaseNumber,
                           ArrivalDate = a.ArrivalDate,
                           Category = a.Category,
                           DateOfBrith = a.DateOfBrith,
                           FullName_AR = a.FullName_AR,
                           FullName_EN = a.FullName_EN,
                           IndividualID = a.IndividualID,
                           IssueCode = DbIDC.codeCardIssueReason.Where(y => y.IssueCode == b.IssueCode).FirstOrDefault().IssueDescription,
                           Sex = a.Sex,
                           CreateBy=b.CreateBy,
                           CreateDate=b.CreateDate,
                           Active = b.Active,
                           dataCardIssuedRowVersion = b.dataCardIssuedRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<CardValidationsDataTableModel> Result = Mapper.Map<List<CardValidationsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CardValidationsDataTableSubmit(List<dataCardIssued> models)
        {
            if (!CMS.HasAction(Permissions.CardValidation.Confirm, Apps.IDC))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<Guid> guids = models.Select(y => y.CardIssuedGUID).ToList();
            var ConfirmedList= DbIDC.dataCardIssued.Where(x => guids.Contains(x.CardIssuedGUID)).ToList();
            ConfirmedList.ForEach(x => x.Approved=true);
            List<dataCardIssued> ConfirmCardValidations = ConfirmedList;

            try
            {
                DbIDC.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIDC.PartialDeleteMessage(ConfirmCardValidations, models, DataTableNames.CardValidationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbIDC.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CardValidationsDataTableDelete(List<dataCardIssued> models)
        {
            if (!CMS.HasAction(Permissions.CardIssued.Delete, Apps.IDC))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataCardIssued> DeletedCardValidations = DeleteCardValidations(models);

            try
            {
                DbIDC.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIDC.PartialDeleteMessage(DeletedCardValidations, models, DataTableNames.CardValidationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbIDC.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CardValidationsDataTableRestore(List<dataCardIssued> models)
        {
            if (!CMS.HasAction(Permissions.CardIssued.Restore, Apps.IDC))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataCardIssued> RestoredCardValidations = RestoreCardValidations(models);

            try
            {
                DbIDC.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbIDC.PartialRestoreMessage(RestoredCardValidations, models, DataTableNames.CardValidationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbIDC.ErrorMessage(ex.Message));
            }
        }

        private List<dataCardIssued> DeleteCardValidations(List<dataCardIssued> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataCardIssued> DeletedCardValidations = new List<dataCardIssued>();

            string query = DbIDC.QueryBuilder(models, Permissions.CardIssued.DeleteGuid, SubmitTypes.Delete, "");

            var CardValidations = DbIDC.Database.SqlQuery<dataCardIssued>(query).ToList();

            foreach (var CardIssued in CardValidations)
            {
                DeletedCardValidations.Add(DbIDC.Delete(CardIssued, ExecutionTime, Permissions.CardIndividualInfo.DeleteGuid, DbCMS));
            }

            return DeletedCardValidations;
        }

        private List<dataCardIssued> RestoreCardValidations(List<dataCardIssued> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataCardIssued> RestoredCardValidations = new List<dataCardIssued>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbIDC.QueryBuilder(models, Permissions.CardIssued.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var CardValidations = DbIDC.Database.SqlQuery<dataCardIssued>(query).ToList();
            foreach (var CardIssued in CardValidations)
            {
                if (!ActiveCardIssued(CardIssued))
                {
                    RestoredCardValidations.Add(DbIDC.Restore(CardIssued, Permissions.CardIssued.DeleteGuid, Permissions.CardIssued.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredCardValidations;
        }
        private bool ActiveCardIssued(dataCardIssued model)
        {
            int CardIssuedID = DbIDC.dataCardIssued
                                  .Where(x =>
                                              x.CardIndividualInfoGUID == model.CardIndividualInfoGUID &&
                                              x.IssueCode == model.IssueCode &&
                                              x.IssueDate == model.IssueDate &&
                                              x.CardIssuedGUID != model.CardIssuedGUID &&
                                              x.Active).Count();
            if (CardIssuedID > 0)
            {
                ModelState.AddModelError("CardIssuedID", "Card Issued with same Reason already exists"); //From resource ?????? Amer  
            }

            return (CardIssuedID > 0);
        }


        #endregion
    }
}