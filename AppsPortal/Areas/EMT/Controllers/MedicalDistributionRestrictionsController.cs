using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AutoMapper;
using EMT_DAL.Model;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.EMT.Controllers
{
    public class MedicalDistributionRestrictionsController : EMTBaseController
    {
        #region  Medical Distribution Restriction 

        public ActionResult Index()
        {
            return View();
        }

        [Route("EMT/MedicalDistributionRestrictions/")]
        public ActionResult MedicalDistributionRestrictionsIndex()
        {
            if (!CMS.HasAction(Permissions.MedicalDistributionRestriction.Access, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/EMT/Views/MedicalDistributionRestrictions/Index.cshtml");
        }

        //[Route("EMT/MedicalDistributionRestrictionsDataTable/{PK}")]
        public ActionResult MedicalDistributionRestrictionsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MedicalDistributionRestrictionsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MedicalDistributionRestrictionsDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.MedicalDistributionRestriction.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var All = (from a in DbEMT.dataMedicalDistributionRestriction.AsNoTracking().AsExpandable()/*.Where(x => AuthorizedList.Contains(x.ProvideByOrganizationInstanceGUID.ToString()))*/
                       join b in DbEMT.codeOrganizationsInstancesLanguages.Where(x=>x.LanguageID==LAN && x.Active) on a.ProvideByOrganizationInstanceGUID equals b.OrganizationInstanceGUID
                       join c in DbEMT.codeOrganizationsInstancesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ImplementPartnerGUID equals c.OrganizationInstanceGUID
                       select new MedicalDistributionRestrictionsDataTableModel
                       {
                           MedicalDistributionRestrictionGUID = a.MedicalDistributionRestrictionGUID,
                           BeneficiaryTypeGUID=a.BeneficiaryTypeGUID,
                           MedicineNumberPerPrescription=a.MedicineNumberPerPrescription,
                           ProvideByOrganizationInstanceGUID=a.ProvideByOrganizationInstanceGUID.ToString(),
                           OrganizationInstanceDescription=b.OrganizationInstanceDescription,
                           ImplementPartnerGUID=a.ImplementPartnerGUID.ToString(),
                           ImplementPartner=c.OrganizationInstanceDescription,
                           MedicinesExcludedByClassification=a.MedicinesExcludedByClassification,
                           PrescriptionNumberPerMonth=a.PrescriptionNumberPerMonth,
                           MedicinesExcludedByBrand=a.MedicinesExcludedByBrand,
                           MedicinesExpiration=a.MedicinesExpiration,
                           MedicinesQuantity=a.MedicinesQuantity,
                           Active = a.Active,
                           dataMedicalDistributionRestrictionRowVersion = a.dataMedicalDistributionRestrictionRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<MedicalDistributionRestrictionsDataTableModel> Result = Mapper.Map<List<MedicalDistributionRestrictionsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult MedicalDistributionRestrictionCreate()
        {
            if (!CMS.HasAction(Permissions.MedicalDistributionRestriction.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/EMT/Views/MedicalDistributionRestrictions/_MedicalDistributionRestrictionsUpdateModal.cshtml",
                new MedicalDistributionRestrictionUpdateModel());
        }

        public ActionResult MedicalDistributionRestrictionUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.MedicalDistributionRestriction.Access, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var Audit = DbEMT.spAuditHistory(LAN, PK).OrderBy(x => x.ExecutionTime).FirstOrDefault();
            if (Audit == null)
            {
                Audit = DbEMT.spAuditHistoryOld("EN", PK).OrderBy(x => x.ExecutionTime).Select(x =>
                            new spAuditHistory_Result()
                            {
                                ExecutedBy = x.ExecutedBy,
                                ExecutionTime = x.ExecutionTime
                            }).FirstOrDefault();
            }
            var model = DbEMT.dataMedicalDistributionRestriction.AsEnumerable().Where(x=>x.MedicalDistributionRestrictionGUID==PK).Select(x=> 
            new MedicalDistributionRestrictionUpdateModel()
            {
                Active = x.Active,
                dataMedicalDistributionRestrictionRowVersion = x.dataMedicalDistributionRestrictionRowVersion,
                MedicalDistributionRestrictionGUID = x.MedicalDistributionRestrictionGUID,
                BeneficiaryTypeClientGUID = x.BeneficiaryTypeGUID,
                BeneficiaryTypeGUID = x.BeneficiaryTypeGUID.Split(','),
                MedicineNumberPerPrescription = x.MedicineNumberPerPrescription,
                MedicinesExpiration = x.MedicinesExpiration,
                MedicinesQuantity = x.MedicinesQuantity,
                PrescriptionNumberPerMonth = x.PrescriptionNumberPerMonth,
                MedicinesExcludedByBrand = x.MedicinesExcludedByBrand.Split(','),
                MedicinesExcludedByBrandClient = x.MedicinesExcludedByBrand,
                MedicinesExcludedByClassification = x.MedicinesExcludedByClassification.Split(','),
                MedicinesExcludedByClassificationClient = x.MedicinesExcludedByClassification,
                ProvideByOrganizationInstanceGUID = x.ProvideByOrganizationInstanceGUID,
                ImplementPartnerGUID=x.ImplementPartnerGUID,
                CreatedBy = Audit.ExecutedBy,
                CreatedOn = Audit.ExecutionTime,
            }).FirstOrDefault();
            return PartialView("~/Areas/EMT/Views/MedicalDistributionRestrictions/_MedicalDistributionRestrictionsUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalDistributionRestrictionCreate(MedicalDistributionRestrictionUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.MedicalDistributionRestriction.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            dataMedicalDistributionRestriction MedicalItemInput = Mapper.Map(model, new dataMedicalDistributionRestriction());
            if (!ModelState.IsValid || ActiveMedicalDistributionRestriction(MedicalItemInput)) return PartialView("~/Areas/EMT/Views/MedicalDistributionRestrictions/_MedicalDistributionRestrictionsUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            MedicalItemInput.BeneficiaryTypeGUID = string.Join(",", model.BeneficiaryTypeGUID);
            MedicalItemInput.MedicinesExcludedByBrand = string.Join(",", model.MedicinesExcludedByBrand);
            MedicalItemInput.MedicinesExcludedByClassification = string.Join(",", model.MedicinesExcludedByClassification);

            DbEMT.Create(MedicalItemInput, Permissions.MedicalDistributionRestriction.CreateGuid, ExecutionTime, DbCMS);


            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(DataTableNames.MedicalDistributionRestrictionsDataTable,
                    DbEMT.PrimaryKeyControl(MedicalItemInput),
                    DbEMT.RowVersionControls(Portal.SingleToList(MedicalItemInput))));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalDistributionRestrictionUpdate(MedicalDistributionRestrictionUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.MedicalDistributionRestriction.Update, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            dataMedicalDistributionRestriction MedicalItemInput = Mapper.Map(model, new dataMedicalDistributionRestriction());
            if (!ModelState.IsValid || ActiveMedicalDistributionRestriction(MedicalItemInput)) return PartialView("~/Areas/EMT/Views/MedicalDistributionRestrictions/_MedicalDistributionRestrictionsUpdateModal.cshtml", model);
            
            DateTime ExecutionTime = DateTime.Now;

            MedicalItemInput.BeneficiaryTypeGUID = string.Join(",", model.BeneficiaryTypeGUID);
            MedicalItemInput.MedicinesExcludedByBrand = string.Join(",", model.MedicinesExcludedByBrand);
            MedicalItemInput.MedicinesExcludedByClassification = string.Join(",", model.MedicinesExcludedByClassification);
            DbEMT.Update(MedicalItemInput, Permissions.MedicalDistributionRestriction.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(DataTableNames.MedicalDistributionRestrictionsDataTable,
                    DbEMT.PrimaryKeyControl(MedicalItemInput),
                    DbEMT.RowVersionControls(Portal.SingleToList(MedicalItemInput))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalDistributionRestriction(model.MedicalDistributionRestrictionGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalDistributionRestrictionDelete(dataMedicalDistributionRestriction model)
        {
            if (!CMS.HasAction(Permissions.MedicalDistributionRestriction.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataMedicalDistributionRestriction> DeletedLanguages = DeleteMedicalDistributionRestrictions(new List<dataMedicalDistributionRestriction> { model });

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleDeleteMessage(DeletedLanguages, DataTableNames.MedicalDistributionRestrictionsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalDistributionRestriction(model.MedicalDistributionRestrictionGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalDistributionRestrictionRestore(dataMedicalDistributionRestriction model)
        {
            if (!CMS.HasAction(Permissions.MedicalDistributionRestriction.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveMedicalDistributionRestriction(model))
            {
                return Json(DbEMT.RecordExists());
            }

            List<dataMedicalDistributionRestriction> RestoredLanguages = RestoreMedicalDistributionRestrictions(Portal.SingleToList(model));

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleRestoreMessage(RestoredLanguages, DataTableNames.MedicalDistributionRestrictionsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalDistributionRestriction(model.MedicalDistributionRestrictionGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalDistributionRestrictionsDataTableDelete(List<dataMedicalDistributionRestriction> models)
        {
            if (!CMS.HasAction(Permissions.MedicalDistributionRestriction.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataMedicalDistributionRestriction> DeletedLanguages = DeleteMedicalDistributionRestrictions(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.MedicalDistributionRestrictionsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }

        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalDistributionRestrictionsDataTableRestore(List<dataMedicalDistributionRestriction> models)
        {
            if (!CMS.HasAction(Permissions.MedicalDistributionRestriction.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataMedicalDistributionRestriction> RestoredLanguages = RestoreMedicalDistributionRestrictions(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.MedicalDistributionRestrictionsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        private List<dataMedicalDistributionRestriction> DeleteMedicalDistributionRestrictions(List<dataMedicalDistributionRestriction> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataMedicalDistributionRestriction> DeletedMedicalDistributionRestrictions = new List<dataMedicalDistributionRestriction>();

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalDistributionRestriction.DeleteGuid, SubmitTypes.Delete, "");

            var MedicalDistributionRestrictions = DbEMT.Database.SqlQuery<dataMedicalDistributionRestriction>(query).ToList();

            //var MedicalDistributionRestriction = DbEMT.dataMedicalDistributionRestriction.Where(x => x.MedicalDistributionRestrictionGUID == models.FirstOrDefault().MedicalDistributionRestrictionGUID).FirstOrDefault();
            foreach (var MedicalDistributionRestriction in MedicalDistributionRestrictions)
            {
                DeletedMedicalDistributionRestrictions.Add(DbEMT.Delete(MedicalDistributionRestriction, ExecutionTime, Permissions.MedicalDistributionRestriction.DeleteGuid, DbCMS));

            }

            return DeletedMedicalDistributionRestrictions;
        }

        private List<dataMedicalDistributionRestriction> RestoreMedicalDistributionRestrictions(List<dataMedicalDistributionRestriction> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataMedicalDistributionRestriction> RestoredLanguages = new List<dataMedicalDistributionRestriction>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalDistributionRestriction.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var MedicalDistributionRestrictions = DbEMT.Database.SqlQuery<dataMedicalDistributionRestriction>(query).ToList();

            //var MedicalDistributionRestriction = DbEMT.dataMedicalDistributionRestriction.Where(x => x.MedicalDistributionRestrictionGUID == models.FirstOrDefault().MedicalDistributionRestrictionGUID).FirstOrDefault();
            foreach (var MedicalDistributionRestriction in MedicalDistributionRestrictions)
            {
                if (!ActiveMedicalDistributionRestriction(MedicalDistributionRestriction))
                {
                    RestoredLanguages.Add(DbEMT.Restore(MedicalDistributionRestriction, Permissions.MedicalDistributionRestriction.DeleteGuid, Permissions.MedicalDistributionRestriction.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyMedicalDistributionRestriction(Guid PK)
        {
            dataMedicalDistributionRestriction dbModel = new dataMedicalDistributionRestriction();

            var Language = DbEMT.dataMedicalDistributionRestriction.Where(l => l.MedicalDistributionRestrictionGUID == PK).FirstOrDefault();
            var dbLanguage = DbEMT.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataMedicalDistributionRestrictionRowVersion.SequenceEqual(dbModel.dataMedicalDistributionRestrictionRowVersion))
            {
                return Json(DbEMT.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbEMT, dbModel, "MedicalDistributionRestrictionsContainer"));
        }

        private bool ActiveMedicalDistributionRestriction(dataMedicalDistributionRestriction model)
        {
            int LanguageID = DbEMT.dataMedicalDistributionRestriction
                                  .Where(x => x.MedicalDistributionRestrictionGUID == model.MedicalDistributionRestrictionGUID &&
                                              x.MedicalDistributionRestrictionGUID != model.MedicalDistributionRestrictionGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("MedicalDistributionRestrictionGUID", "Medical Item already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }


       
        #endregion
    }
}