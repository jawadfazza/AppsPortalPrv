using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using OVS_DAL.Model;
using RES_Repo.Globalization;

namespace AppsPortal.Areas.OVS.Controllers
{
    public class ConditionController : OVSBaseController
    {
        #region Election Conditions

        public ActionResult ElectionConditionsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null)
                return PartialView("~/Areas/OVS/Views/Condition/_ConditionsDataTable.cshtml",
                    new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<dataElectionCondition, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<dataElectionCondition>(DataTable.Filters);
            }

            var Result = (from a in DbOVS.dataElectionCondition.AsNoTracking().AsExpandable().Where(Predicate)
                          where a.ElectionGUID == PK
                          join b in DbOVS.codeConditionType on a.ConditionTypeGUID equals b.ConditionTypeGUID
                          join c in DbOVS.codeConditionTypeLanguage.Where(x => x.LanguageID == LAN) on b.ConditionTypeGUID equals c.ConditionTypeGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()

                          select new
                          {
                              Active = a.Active,
                              ElectionConditionGUID = a.ElectionConditionGUID,
                              Description = R1.Description,
                              ConditionValue = a.ConditionValue,
                              dataElectionConditionRowVersion = a.dataElectionConditionRowVersion
                          });


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ElectionConditionCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ConditionTypes.Create, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return PartialView("~/Areas/OVS/Views/Condition/_ConditionUpdateModal.cshtml",
                new dataElectionCondition { ElectionGUID = FK });
        }

        public ActionResult ElectionConditionUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ConditionTypes.Access, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            var model = DbOVS.dataElectionCondition.Find(PK);

            return PartialView("~/Areas/OVS/Views/Condition/_ConditionUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionConditionCreate(dataElectionCondition model)
        {
            if (!CMS.HasAction(Permissions.ConditionTypes.Create, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataElectionCondition> condtions = DbOVS.dataElectionCondition
                .Where(x => x.ElectionGUID == model.ElectionGUID).ToList();
            condtions.Add(model);
            List<dataElectionCandidate> candidateses =
                DbOVS.dataElectionCandidate.Where(x => x.ElectionGUID == model.ElectionGUID).ToList();
            string checkMessage = checkElectionCanidates(condtions, candidateses);
            if (checkMessage != "1")
            {
                return Json(DbOVS.ErrorMessage(checkMessage));
            }

            if (!ModelState.IsValid || ActiveElectionCondition(model))
                return PartialView("~/Areas/OVS/Views/Condition/_ConditionUpdateModal.cshtml", model);
            Guid ActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;
            DbOVS.Create(model, Permissions.ConditionTypes.CreateGuid, ExecutionTime, DbCMS);
            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleUpdateMessage(DataTableNames.ElectionConditionsDataTable,
                    DbOVS.PrimaryKeyControl(model), DbOVS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionConditionUpdate(dataElectionCondition model)
        {
            if (!CMS.HasAction(Permissions.ConditionTypes.Update, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataElectionCondition> condtions = DbOVS.dataElectionCondition
                .Where(x => x.ElectionGUID == model.ElectionGUID).ToList();
            condtions.Remove(model);
            condtions.Add(model);
            List<dataElectionCandidate> candidateses =
                DbOVS.dataElectionCandidate.Where(x => x.ElectionGUID == model.ElectionGUID).ToList();

            string checkMessage = checkElectionCanidates(condtions, candidateses);
            if (checkMessage != "1")
            {
                return Json(DbOVS.ErrorMessage(checkMessage));
            }


            if (!ModelState.IsValid || ActiveElectionCondition(model))
                return PartialView("~/Areas/OVS/Views/Condition/_ConditionUpdateModal.cshtml", model);

            Guid ActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;

            //DbOVS.Update(model, ActionGUID, ExecutionTime);
            DbOVS.Update(model, Permissions.ConditionTypes.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleUpdateMessage(DataTableNames.ElectionConditionsDataTable,
                    DbOVS.PrimaryKeyControl(model),
                    DbOVS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyElectionConidtion(model.ElectionConditionGUID);
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        public ActionResult ElectionConditionDelete(dataElectionCondition model)
        {
            if (!CMS.HasAction(Permissions.ConditionTypes.Delete, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataElectionCondition> DeletedConditions =
                DeleteElectionConditions(new List<dataElectionCondition> { model });

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleDeleteMessage(DeletedConditions, DataTableNames.ElectionConditionsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyElectionConidtion(model.ElectionConditionGUID);
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionConditionRestore(dataElectionCondition model)
        {
            if (!CMS.HasAction(Permissions.ConditionTypes.Restore, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (ActiveElectionCondition(model))
            {
                return Json(DbOVS.RecordExists());
            }

            List<dataElectionCondition> RestoredLanguages = RestoreElectionCondition(Portal.SingleToList(model));

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleRestoreMessage(RestoredLanguages, DataTableNames.ElectionLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyElectionConidtion(model.ElectionConditionGUID);
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ElectionConditionsDataTableDelete(List<dataElectionCondition> models)
        {
            if (!CMS.HasAction(Permissions.ConditionTypes.Delete, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataElectionCondition> DeletedConditions = DeleteElectionConditions(models);

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.PartialDeleteMessage(DeletedConditions, models,
                    DataTableNames.ElectionConditionsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ElectionConditionsDataTableRestore(List<dataElectionCondition> models)
        {
            if (!CMS.HasAction(Permissions.ConditionTypes.Restore, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataElectionCondition> RestoredConditions = RestoreElectionCondition(models);

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.PartialRestoreMessage(RestoredConditions, models,
                    DataTableNames.ElectionConditionsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        private List<dataElectionCondition> DeleteElectionConditions(List<dataElectionCondition> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataElectionCondition> DeletedElectionConditions = new List<dataElectionCondition>();

            string query = DbOVS.QueryBuilder(models, Permissions.ConditionTypes.DeleteGuid, SubmitTypes.Delete, "");

            var conditionses = DbOVS.Database.SqlQuery<dataElectionCondition>(query).ToList();

            foreach (var condition in conditionses)
            {
                DeletedElectionConditions.Add(DbOVS.Delete(condition, ExecutionTime,
                    Permissions.ConditionTypes.DeleteGuid, DbCMS));
            }

            return DeletedElectionConditions;
        }

        private List<dataElectionCondition> RestoreElectionCondition(List<dataElectionCondition> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataElectionCondition> RestoredConditions = new List<dataElectionCondition>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbOVS.QueryBuilder(models, Permissions.ElectionLanguages.DeleteGuid, SubmitTypes.Restore,
                baseQuery);

            var conditions = DbOVS.Database.SqlQuery<dataElectionCondition>(query).ToList();
            foreach (var condition in conditions)
            {
                if (!ActiveElectionCondition(condition))
                {
                    RestoredConditions.Add(DbOVS.Restore(condition, Permissions.ConditionTypes.DeleteGuid,
                        Permissions.ConditionTypes.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredConditions;
        }

        private JsonResult ConcrrencyElectionConidtion(Guid PK)
        {
            dataElectionCondition dbModel = new dataElectionCondition();

            var condition = DbOVS.dataElectionCondition.Where(l => l.ElectionConditionGUID == PK).FirstOrDefault();
            var dbCondition = DbOVS.Entry(condition).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbCondition, dbModel);

            if (condition.dataElectionConditionRowVersion.SequenceEqual(dbModel.dataElectionConditionRowVersion))
            {
                return Json(DbOVS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbOVS, dbModel, "ConditionsContainer"));
        }

        private bool ActiveElectionCondition(dataElectionCondition model)
        {
            int electionCond = 0;
            if (model.ElectionConditionGUID == Guid.Empty)
            {

                electionCond = DbOVS.dataElectionCondition
                   .Where(x => x.ConditionTypeGUID == model.ConditionTypeGUID &&
                               x.ElectionGUID == model.ElectionGUID &&

                               x.Active).Count();
                if (electionCond > 0)
                {
                    ModelState.AddModelError("Condition", "Election Condition already exists");
                }
            }

            if (model.ConditionTypeGUID == Guid.Parse("00000000-0000-0000-0000-000000000001"))
            {
                Guid maxOVSs = Guid.Parse("00000000-0000-0000-0000-000000000002");

                var maxAllowedVotes = DbOVS.dataElectionCondition
                    .FirstOrDefault(x => x.ConditionTypeGUID == maxOVSs)
                    ?.ConditionValue;

                if ((maxAllowedVotes != null) && (int.Parse(model.ConditionValue) > int.Parse(maxAllowedVotes.ToString())))
                {
                    ModelState.AddModelError("Condition", resxMessages.CheckMinimumVotes);
                    return false;
                }
            }

            return (electionCond > 0);
        }

        public string checkElectionCanidates(List<dataElectionCondition> Conditions, List<dataElectionCandidate> Candidateses)
        {
            string result = "1";
            if ((Conditions != null) && (Conditions.Count > 0))
            {
                int candidatesCount = Candidateses.Count;
                //?????????????
                int femalcandidatesCount = Candidateses
                    .Where(x => x.GenderGUID == Guid.Parse("DBF9D307-CE9F-4029-BD1E-D7AF6739975C")).Count();
                int maleCandidatesCount = candidatesCount - femalcandidatesCount;
                foreach (dataElectionCondition condition in Conditions)
                {
                    int value = int.Parse(condition.ConditionValue);
                    //Min Votes
                    if (condition.ConditionTypeGUID == Guid.Parse("00000000-0000-0000-0000-000000000001")
                        && (candidatesCount < value || value <= 0))
                    {
                        return resxMessages.CheckMinimumCandiates;
                    }
                    //Max votes
                    if (condition.ConditionTypeGUID == Guid.Parse("00000000-0000-0000-0000-000000000002")
                        && (candidatesCount < value || value <= 0))
                    {
                        return resxMessages.CheckMaxmimumCandiates;
                    }

                    if (condition.ConditionTypeGUID == Guid.Parse("00000000-0000-0000-0000-000000000003")
                        && (maleCandidatesCount < value || value <= 0))
                    {
                        return resxMessages.CheckMaxmimumMaleCandiates;
                    }

                    if (condition.ConditionTypeGUID == Guid.Parse("00000000-0000-0000-0000-000000000004")
                        && (femalcandidatesCount < value || value <= 0))
                    {
                        return resxMessages.CheckMaxmimumFemaleCandiates;
                    }
                }

            }

            return result;
        }



        #endregion
    }
}