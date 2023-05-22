using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using REF_DAL.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.REF.Controllers
{
    public class ReferralNotificationsController : REFBaseController
    {
        #region Referral Notifications

        //[Route("REF/ReferralNotificationsDataTable/{PK}")]
        public ActionResult ReferralNotificationsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/REF/Views/ReferralNotifications/_Index.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<configReferralNotification, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<configReferralNotification>(DataTable.Filters);
            }

            var Result = DbREF.configReferralNotification.AsNoTracking().AsExpandable().Where(x => x.ReferralStepGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.ReferralNotificationGUID,
                                  x.Icon,
                                  x.ReferralStatusGUID,
                                  x.NotificationResxKey,
                                  x.PageURL,
                                  x.configReferralNotificationRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReferralNotificationCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ReferralNotification.Create, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/REF/Views/ReferralNotifications/_ReferralNotificationForm.cshtml",
                new configReferralNotification { ReferralStepGUID = FK });
        }

        public ActionResult ReferralNotificationUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ReferralNotification.Access, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/REF/Views/ReferralNotifications/_ReferralNotificationForm.cshtml", DbREF.configReferralNotification.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralNotificationCreate(configReferralNotification model)
        {
            if (!CMS.HasAction(Permissions.ReferralNotification.Create, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveReferralNotification(model)) return PartialView("~/Areas/REF/Views/ReferralNotifications/_ReferralNotificationForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbREF.Create(model, Permissions.ReferralNotification.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleUpdateMessage(DataTableNames.ReferralNotificationsDataTable, DbREF.PrimaryKeyControl(model), DbREF.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralNotificationUpdate(configReferralNotification model)
        {
            if (!CMS.HasAction(Permissions.ReferralNotification.Update, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveReferralNotification(model)) return PartialView("~/Areas/REF/Views/ReferralNotifications/_ReferralNotificationForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbREF.Update(model, Permissions.ReferralNotification.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleUpdateMessage(DataTableNames.ReferralNotificationsDataTable,
                    DbREF.PrimaryKeyControl(model),
                    DbREF.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyReferralNotification(model.ReferralNotificationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralNotificationDelete(configReferralNotification model)
        {
            if (!CMS.HasAction(Permissions.ReferralNotification.Delete, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<configReferralNotification> Deleteds = DeleteReferralNotifications(new List<configReferralNotification> { model });

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleDeleteMessage(Deleteds, DataTableNames.ReferralNotificationsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyReferralNotification(model.ReferralNotificationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralNotificationRestore(configReferralNotification model)
        {
            if (!CMS.HasAction(Permissions.ReferralNotification.Restore, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveReferralNotification(model))
            {
                return Json(DbREF.RecordExists());
            }

            List<configReferralNotification> Restoreds = RestoreReferralNotifications(Portal.SingleToList(model));

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleRestoreMessage(Restoreds, DataTableNames.ReferralNotificationsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyReferralNotification(model.ReferralNotificationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ReferralNotificationsDataTableDelete(List<configReferralNotification> models)
        {
            if (!CMS.HasAction(Permissions.ReferralNotification.Delete, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<configReferralNotification> Deleteds = DeleteReferralNotifications(models);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.PartialDeleteMessage(Deleteds, models, DataTableNames.ReferralNotificationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ReferralNotificationsDataTableRestore(List<configReferralNotification> models)
        {
            if (!CMS.HasAction(Permissions.ReferralNotification.Restore, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<configReferralNotification> Restoreds = RestoreReferralNotifications(models);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.PartialRestoreMessage(Restoreds, models, DataTableNames.ReferralNotificationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        private List<configReferralNotification> DeleteReferralNotifications(List<configReferralNotification> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<configReferralNotification> DeletedReferralNotifications = new List<configReferralNotification>();

            string query = DbREF.QueryBuilder(models, Permissions.ReferralNotification.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbREF.Database.SqlQuery<configReferralNotification>(query).ToList();

            foreach (var language in languages)
            {
                DeletedReferralNotifications.Add(DbREF.Delete(language, ExecutionTime, Permissions.ReferralNotification.DeleteGuid, DbCMS));
            }

            return DeletedReferralNotifications;
        }

        private List<configReferralNotification> RestoreReferralNotifications(List<configReferralNotification> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<configReferralNotification> Restoreds = new List<configReferralNotification>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbREF.QueryBuilder(models, Permissions.ReferralNotification.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var ReferralNotifications = DbREF.Database.SqlQuery<configReferralNotification>(query).ToList();
            foreach (var language in ReferralNotifications)
            {
                if (!ActiveReferralNotification(language))
                {
                    Restoreds.Add(DbREF.Restore(language, Permissions.ReferralNotification.DeleteGuid, Permissions.ReferralNotification.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return Restoreds;
        }

        private JsonResult ConcrrencyReferralNotification(Guid PK)
        {
            configReferralNotification dbModel = new configReferralNotification();

            var ReferralNotification = DbREF.configReferralNotification.Where(l => l.ReferralNotificationGUID == PK).FirstOrDefault();
            var dbReferralNotification = DbREF.Entry(ReferralNotification).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbReferralNotification, dbModel);

            if (ReferralNotification.configReferralNotificationRowVersion.SequenceEqual(dbModel.configReferralNotificationRowVersion))
            {
                return Json(DbREF.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbREF, dbModel, "LanguagesContainer"));
        }

        private bool ActiveReferralNotification(configReferralNotification model)
        {
            int ReferralNotificationID = DbREF.configReferralNotification
                                  .Where(x => 
                                              x.ReferralNotificationGUID == model.ReferralNotificationGUID &&
                                              x.ReferralNotificationGUID != model.ReferralNotificationGUID &&
                                              x.Active).Count();
            if (ReferralNotificationID > 0)
            {
                ModelState.AddModelError("LanguageID", "Telecom Company Name in selected language already exists"); //From resource ?????? Amer  
            }

            return (ReferralNotificationID > 0);
        }

        #endregion
    }
}