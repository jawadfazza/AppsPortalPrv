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
    public class CorrespondenceController : OVSBaseController
    {
        #region Elections Corrspondeces

        //[Route("Configuration/ElectionLanguagesDataTable/{PK}")]
        public ActionResult ElectionCorrespondencesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null)
                return PartialView("~/Areas/OVS/Views/Corrspondece/_CorrespondencesDataTable.cshtml",
                    new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<dataElectionCorrespondence, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<dataElectionCorrespondence>(DataTable.Filters);
            }

            var Result = DbOVS.dataElectionCorrespondence.AsNoTracking().AsExpandable()
                .Where(x => x.ElectionGUID == PK).Where(Predicate)
                .Select(x => new
                {
                    x.ElectionCorrespondenceGUID,
                    x.MessageBody,
                    x.MessageTitle,
                    x.SendDate,
                    x.codeElectionCorrespondenceType.codeElectionCorrespondenceTypeLanguage.FirstOrDefault(f => f.LanguageID == LAN).LanguageID,
                    x.dataElectionCorrespondenceRowVersion
                });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }


        public ActionResult ElectionCorrespondenceCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Create, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return PartialView("~/Areas/OVS/Views/Corrspondece/_CorrespondenceUpdateModal.cshtml",
                new dataElectionCorrespondence { ElectionGUID = FK });
        }

        public ActionResult ElectionCorrespondenceUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Access, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return PartialView("~/Areas/OVS/Views/Corrspondece/_CorrespondenceUpdateModal.cshtml",
                DbOVS.dataElectionCorrespondence.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionCorrespondenceCreate(dataElectionCorrespondence model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Create, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid)
                return PartialView("~/Areas/OVS/Views/Corrspondece/_CorrespondenceUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbOVS.Create(model, Permissions.ElectionLanguages.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                foreach (var item in DbOVS.dataElectionStaff.Where(x => x.ElectionGUID == model.ElectionGUID && x.Active).ToList())
                {
                    string URL = AppSettingsKeys.Domain + "OVS/OVS/VoteChecker/?accessKey=" + item.AccessKey;
                    string Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.VoteInvitaion + "</a>";
                    string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";

                    string _message = resxEmails.VoteInvitationMessage.Replace("$FullName", item.dataStaff.FullName)
                         .Replace("$InvitationDate", item.dataElection.StartDate.ToString())
                         .Replace("$CloseDate", item.dataElection.CloseDate.ToString())
                         .Replace("$VoteName", item.dataElection.dataElectionLanguage.FirstOrDefault(x => x.Active && x.LanguageID == LAN).Title)
                         .Replace("$VoteLink", Anchor)
                         .Replace("$Link", Link);
                    mail.Send(item.dataStaff.EmailAddress, resxEmails.VoteInvitationSubject, _message);
                }
                return Json(DbOVS.SingleUpdateMessage(DataTableNames.ElectionCorrespondencesDataTable,
                    DbOVS.PrimaryKeyControl(model), DbOVS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionCorrespondenceUpdate(dataElectionCorrespondence model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Update, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid)
                return PartialView("~/Areas/OVS/Views/Corrspondece/_CorrespondenceUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbOVS.Update(model, Permissions.ElectionLanguages.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleUpdateMessage(DataTableNames.ElectionLanguagesDataTable,
                    DbOVS.PrimaryKeyControl(model),
                    DbOVS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyElectionCorrespondence(model.ElectionCorrespondenceGUID);
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionCorrespondenceDelete(dataElectionCorrespondence model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Delete, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataElectionCorrespondence> DeletedCorrespondences =
                DeleteElectionCorrespondences(new List<dataElectionCorrespondence> { model });

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleDeleteMessage(DeletedCorrespondences, DataTableNames.ElectionCorrespondencesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyElectionCorrespondence(model.ElectionCorrespondenceGUID);
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionCorrespondencRestore(dataElectionCorrespondence model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Restore, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (ActiveElectionCorrespondence(model))
            {
                return Json(DbOVS.RecordExists());
            }

            List<dataElectionCorrespondence> RestoredCorrespondencs = RestoreElectionCorrespondences(Portal.SingleToList(model));

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleRestoreMessage(RestoredCorrespondencs, DataTableNames.ElectionCorrespondencesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyElectionCorrespondence(model.ElectionCorrespondenceGUID);
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ElectionCorrespondencDataTableDelete(List<dataElectionCorrespondence> models)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Delete, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataElectionCorrespondence> DeletedCorrespondencs = DeleteElectionCorrespondences(models);

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.PartialDeleteMessage(DeletedCorrespondencs, models,
                    DataTableNames.ElectionCorrespondencesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ElectionCorrespondencsDataTableRestore(List<dataElectionCorrespondence> models)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Restore, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataElectionCorrespondence> RestoredCorrespondences = RestoreElectionCorrespondences(models);

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.PartialRestoreMessage(RestoredCorrespondences, models,
                    DataTableNames.ElectionCorrespondencesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        private List<dataElectionCorrespondence> DeleteElectionCorrespondences(List<dataElectionCorrespondence> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataElectionCorrespondence> DeletedElectionCorrespondences = new List<dataElectionCorrespondence>();

            string query = DbOVS.QueryBuilder(models, Permissions.ElectionsManagement.DeleteGuid, SubmitTypes.Delete, "");

            var correspondeces = DbOVS.Database.SqlQuery<dataElectionCorrespondence>(query).ToList();

            foreach (var correspondece in correspondeces)
            {
                DeletedElectionCorrespondences.Add(DbOVS.Delete(correspondece, ExecutionTime,
                    Permissions.ElectionsManagement.DeleteGuid, DbCMS));
            }

            return DeletedElectionCorrespondences;
        }

        private List<dataElectionCorrespondence> RestoreElectionCorrespondences(List<dataElectionCorrespondence> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataElectionCorrespondence> RestoredElectionCorrespondences = new List<dataElectionCorrespondence>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbOVS.QueryBuilder(models, Permissions.ElectionsManagement.DeleteGuid, SubmitTypes.Restore,
                baseQuery);

            var correspondences = DbOVS.Database.SqlQuery<dataElectionCorrespondence>(query).ToList();
            foreach (var correspondence in correspondences)
            {
                if (!ActiveElectionCorrespondence(correspondence))
                {
                    RestoredElectionCorrespondences.Add(DbOVS.Restore(correspondence, Permissions.ElectionsManagement.DeleteGuid,
                        Permissions.ElectionsManagement.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredElectionCorrespondences;
        }

        private JsonResult ConcrrencyElectionCorrespondence(Guid PK)
        {
            dataElectionCorrespondence dbModel = new dataElectionCorrespondence();

            var correspondence = DbOVS.dataElectionCorrespondence.Where(l => l.ElectionCorrespondenceGUID == PK).FirstOrDefault();
            var dbcorrespondence = DbOVS.Entry(correspondence).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbcorrespondence, dbModel);

            if (correspondence.dataElectionCorrespondenceRowVersion.SequenceEqual(dbModel.dataElectionCorrespondenceRowVersion))
            {
                return Json(DbOVS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbOVS, dbModel, "CorrespondenceContainer"));
        }

        private bool ActiveElectionCorrespondence(dataElectionCorrespondence model)
        {
            int LanguageID = DbOVS.dataElectionCorrespondence
                .Where(x => x.MessageTitle == model.MessageTitle && x.SendDate == model.SendDate).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("CorrespondenceID", resxMessages.ElecetionAlreadyExists);
            }

            return (LanguageID > 0);
        }

        #endregion
    }
}