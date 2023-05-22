using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.REF.ViewModels;
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
    public class ReferralStepUsersController : REFBaseController
    {
        #region Referral StepUsers

        //[Route("REF/ReferralStepUsersDataTable/{PK}")]
        public ActionResult ReferralStepUsersDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/REF/Views/ReferralStepUsers/_Index.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ReferralStepUserDataTable, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ReferralStepUserDataTable>(DataTable.Filters);
            }

            var Result = (from a in DbREF.configReferralStepUser.AsNoTracking().AsExpandable().Where(x => x.ReferralStepGUID == PK)
                          join b in DbREF.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.UserGUID equals b.UserGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          select new ReferralStepUserDataTable
                          {
                              ReferralStepUserGUID = a.ReferralStepUserGUID,
                              ActiveUntil = a.ActiveUntil,
                              Active = a.Active,
                              configReferralStepUserRowVersion = a.configReferralStepUserRowVersion,
                              FullName = R1.FirstName + " " + R1.Surname
                          }).Where(Predicate);

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReferralStepUserCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ReferralStepUser.Create, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/REF/Views/ReferralStepUsers/_ReferralStepUserForm.cshtml",
                new configReferralStepUser { ReferralStepGUID = FK });
        }

        public ActionResult ReferralStepUserUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ReferralStepUser.Access, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/REF/Views/ReferralStepUsers/_ReferralStepUserForm.cshtml", DbREF.configReferralStepUser.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralStepUserCreate(configReferralStepUser model)
        {
            if (!CMS.HasAction(Permissions.ReferralStepUser.Create, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveReferralStepUser(model)) return PartialView("~/Areas/REF/Views/ReferralStepUsers/_ReferralStepUserForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbREF.Create(model, Permissions.ReferralStepUser.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleUpdateMessage(DataTableNames.ReferralStepUsersDataTable, DbREF.PrimaryKeyControl(model), DbREF.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralStepUserUpdate(configReferralStepUser model)
        {
            if (!CMS.HasAction(Permissions.ReferralStepUser.Update, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveReferralStepUser(model)) return PartialView("~/Areas/REF/Views/ReferralStepUsers/_ReferralStepUserForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbREF.Update(model, Permissions.ReferralStepUser.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleUpdateMessage(DataTableNames.ReferralStepUsersDataTable,
                    DbREF.PrimaryKeyControl(model),
                    DbREF.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyReferralStepUser(model.ReferralStepUserGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralStepUserDelete(configReferralStepUser model)
        {
            if (!CMS.HasAction(Permissions.ReferralStepUser.Delete, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<configReferralStepUser> Deleteds = DeleteReferralStepUsers(new List<configReferralStepUser> { model });

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleDeleteMessage(Deleteds, DataTableNames.ReferralStepUsersDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyReferralStepUser(model.ReferralStepUserGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReferralStepUserRestore(configReferralStepUser model)
        {
            if (!CMS.HasAction(Permissions.ReferralStepUser.Restore, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveReferralStepUser(model))
            {
                return Json(DbREF.RecordExists());
            }

            List<configReferralStepUser> Restoreds = RestoreReferralStepUsers(Portal.SingleToList(model));

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.SingleRestoreMessage(Restoreds, DataTableNames.ReferralStepUsersDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyReferralStepUser(model.ReferralStepUserGUID);
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ReferralStepUsersDataTableDelete(List<configReferralStepUser> models)
        {
            if (!CMS.HasAction(Permissions.ReferralStepUser.Delete, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<configReferralStepUser> Deleteds = DeleteReferralStepUsers(models);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.PartialDeleteMessage(Deleteds, models, DataTableNames.ReferralStepUsersDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ReferralStepUsersDataTableRestore(List<configReferralStepUser> models)
        {
            if (!CMS.HasAction(Permissions.ReferralStepUser.Restore, Apps.REF))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<configReferralStepUser> Restoreds = RestoreReferralStepUsers(models);

            try
            {
                DbREF.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbREF.PartialRestoreMessage(Restoreds, models, DataTableNames.ReferralStepUsersDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbREF.ErrorMessage(ex.Message));
            }
        }

        private List<configReferralStepUser> DeleteReferralStepUsers(List<configReferralStepUser> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<configReferralStepUser> DeletedReferralStepUsers = new List<configReferralStepUser>();

            string query = DbREF.QueryBuilder(models, Permissions.ReferralStepUser.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbREF.Database.SqlQuery<configReferralStepUser>(query).ToList();

            foreach (var language in languages)
            {
                DeletedReferralStepUsers.Add(DbREF.Delete(language, ExecutionTime, Permissions.ReferralStepUser.DeleteGuid, DbCMS));
            }

            return DeletedReferralStepUsers;
        }

        private List<configReferralStepUser> RestoreReferralStepUsers(List<configReferralStepUser> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<configReferralStepUser> Restoreds = new List<configReferralStepUser>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbREF.QueryBuilder(models, Permissions.ReferralStepUser.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var ReferralStepUsers = DbREF.Database.SqlQuery<configReferralStepUser>(query).ToList();
            foreach (var language in ReferralStepUsers)
            {
                if (!ActiveReferralStepUser(language))
                {
                    Restoreds.Add(DbREF.Restore(language, Permissions.ReferralStepUser.DeleteGuid, Permissions.ReferralStepUser.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return Restoreds;
        }

        private JsonResult ConcrrencyReferralStepUser(Guid PK)
        {
            configReferralStepUser dbModel = new configReferralStepUser();

            var ReferralStepUser = DbREF.configReferralStepUser.Where(l => l.ReferralStepUserGUID == PK).FirstOrDefault();
            var dbReferralStepUser = DbREF.Entry(ReferralStepUser).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbReferralStepUser, dbModel);

            if (ReferralStepUser.configReferralStepUserRowVersion.SequenceEqual(dbModel.configReferralStepUserRowVersion))
            {
                return Json(DbREF.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbREF, dbModel, "LanguagesContainer"));
        }

        private bool ActiveReferralStepUser(configReferralStepUser model)
        {
            int ReferralStepUserID = DbREF.configReferralStepUser
                                  .Where(x =>
                                              x.ReferralStepUserGUID == model.ReferralStepUserGUID &&
                                              x.ReferralStepUserGUID != model.ReferralStepUserGUID &&
                                              x.Active).Count();
            if (ReferralStepUserID > 0)
            {
                ModelState.AddModelError("LanguageID", "Telecom Company Name in selected language already exists"); //From resource ?????? Amer  
            }

            return (ReferralStepUserID > 0);
        }

        #endregion
    }
}