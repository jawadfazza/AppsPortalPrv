using AMS_DAL.Model;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.AMS.ViewModels;
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

namespace AppsPortal.Areas.AMS.Controllers
{
    public class ContactInfosController : AMSBaseController
    {
        #region ContactInfo Types Language

        //[Route("AMS/ContactInfosDataTable/{PK}")]
        public ActionResult ContactInfosDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/AMS/Views/ContactInfos/_Index.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<dataContactInfo, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<dataContactInfo>(DataTable.Filters);
            }

            var Result = (from a in DbAMS.dataContactInfo.AsNoTracking().AsExpandable().Where(x => x.CaseGUID == PK).Where(Predicate)
                          join b in DbAMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active && x.codeTablesValues.TableGUID==LookupTables.PhoneType) on a.PhoneType.Value equals b.ValueGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          select new ContactInfosDataTableModel
                          {
                              ContactInfoGUID = a.ContactInfoGUID,
                              PhoneNumber=a.PhoneNumber,
                              PhoneType=R1.ValueDescription,
                              Active = a.Active,
                              dataContactInfoRowVersion = a.dataContactInfoRowVersion

                          });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ContactInfoCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ContactInfo.Create, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/AMS/Views/ContactInfos/_ContactInfoForm.cshtml", new ContactInfoUpdateModel { CaseGUID= FK });

        }

        public ActionResult ContactInfoUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ContactInfo.Access, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/AMS/Views/ContactInfos/_ContactInfoForm.cshtml", Mapper.Map(DbAMS.dataContactInfo.Find(PK), new ContactInfoUpdateModel()));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ContactInfoCreate(dataContactInfo model)
        {
            if (!CMS.HasAction(Permissions.ContactInfo.Create, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            model.ContactInfoGUID = Guid.NewGuid();
            if (!ModelState.IsValid || ActiveContactInfo(model)) return PartialView("~/Areas/AMS/Views/ContactInfos/_ContactInfoForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbAMS.Create(model, Permissions.ContactInfo.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.SingleUpdateMessage(DataTableNames.ContactInfosDataTable, DbAMS.PrimaryKeyControl(model), DbAMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ContactInfoUpdate(dataContactInfo model)
        {
            if (!CMS.HasAction(Permissions.ContactInfo.Update, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            //dataContactInfo ContactInfo = Mapper.Map(model,DbAMS.dataContactInfo.Where(x=>x.ContactInfoGUID==model.ContactInfoGUID).FirstOrDefault());
            if (!ModelState.IsValid || ActiveContactInfo(model)) return PartialView("~/Areas/AMS/Views/ContactInfos/_Index.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbAMS.Update(model, Permissions.ContactInfo.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.SingleUpdateMessage(DataTableNames.ContactInfosDataTable,
                    DbAMS.PrimaryKeyControl(model),
                    DbAMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyContactInfo(model.ContactInfoGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ContactInfoDelete(dataContactInfo model)
        {
            if (!CMS.HasAction(Permissions.ContactInfo.Delete, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataContactInfo> DeletedLanguages = DeleteContactInfos(new List<dataContactInfo> { model });

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.ContactInfosDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyContactInfo(model.ContactInfoGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ContactInfoRestore(dataContactInfo model)
        {
            if (!CMS.HasAction(Permissions.ContactInfo.Restore, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveContactInfo(model))
            {
                return Json(DbAMS.RecordExists());
            }

            List<dataContactInfo> RestoredLanguages = RestoreContactInfos(Portal.SingleToList(model));

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.ContactInfosDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyContactInfo(model.ContactInfoGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ContactInfosDataTableDelete(List<dataContactInfo> models)
        {
            if (!CMS.HasAction(Permissions.ContactInfo.Delete, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataContactInfo> DeletedLanguages = DeleteContactInfos(models);

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.ContactInfosDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ContactInfosDataTableRestore(List<dataContactInfo> models)
        {
            if (!CMS.HasAction(Permissions.ContactInfo.Restore, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataContactInfo> RestoredLanguages = RestoreContactInfos(models);

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.ContactInfosDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        private List<dataContactInfo> DeleteContactInfos(List<dataContactInfo> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataContactInfo> DeletedContactInfos = new List<dataContactInfo>();

            string query = DbAMS.QueryBuilder(models, Permissions.ContactInfo.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbAMS.Database.SqlQuery<dataContactInfo>(query).ToList();

            foreach (var language in languages)
            {
                DeletedContactInfos.Add(DbAMS.Delete(language, ExecutionTime, Permissions.ContactInfo.DeleteGuid, DbCMS));
            }

            return DeletedContactInfos;
        }

        private List<dataContactInfo> RestoreContactInfos(List<dataContactInfo> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataContactInfo> RestoredLanguages = new List<dataContactInfo>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbAMS.QueryBuilder(models, Permissions.ContactInfo.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbAMS.Database.SqlQuery<dataContactInfo>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveContactInfo(language))
                {
                    RestoredLanguages.Add(DbAMS.Restore(language, Permissions.ContactInfo.DeleteGuid, Permissions.ContactInfo.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyContactInfo(Guid PK)
        {
            dataContactInfo dbModel = new dataContactInfo();

            var Language = DbAMS.dataContactInfo.Where(l => l.ContactInfoGUID == PK).FirstOrDefault();
            var dbLanguage = DbAMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataContactInfoRowVersion.SequenceEqual(dbModel.dataContactInfoRowVersion))
            {
                return Json(DbAMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbAMS, dbModel, "ContactInfosContainer"));
        }

        private bool ActiveContactInfo(dataContactInfo model)
        {
            int LanguageID = DbAMS.dataContactInfo
                                  .Where(x => x.ContactInfoGUID == model.ContactInfoGUID &&
                                              x.ContactInfoGUID != model.ContactInfoGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Telecom Company Name in selected language already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }

        #endregion
    }
}