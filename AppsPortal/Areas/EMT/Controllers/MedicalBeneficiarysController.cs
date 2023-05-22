using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using EMT_DAL.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AppsPortal.Areas.PRG.Models;


namespace AppsPortal.Areas.EMT.Controllers
{
    public class MedicalBeneficiarysController : EMTBaseController
    {
        #region  Medical Beneficiary Item Out Details

        public ActionResult Index()
        {
            return View();
        }

        [Route("EMT/MedicalBeneficiarys/")]
        public ActionResult MedicalBeneficiarysIndex()
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiary.Access, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/EMT/Views/MedicalBeneficiarys/Index.cshtml");
        }

        //[Route("EMT/MedicalBeneficiarysDataTable/{PK}")]
        public ActionResult MedicalBeneficiarysDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MedicalBeneficiarysDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MedicalBeneficiarysDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.MedicalBeneficiary.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var All = (from a in DbEMT.dataMedicalBeneficiary.AsNoTracking().AsExpandable().Where(x => AuthorizedList.Contains(x.OrganizationInstanceGUID.ToString()))
                       select new MedicalBeneficiarysDataTableModel
                          {
                              MedicalBeneficiaryGUID = a.MedicalBeneficiaryGUID,
                              BeneficiaryTypeGUID = a.BeneficiaryTypeGUID,
                              FullName=a.RefugeeFullName,
                              Brithday = a.Brithday,
                              GenderGUID = a.GenderGUID,
                              IDNumber = a.IDNumber,
                              UNHCRNumber = a.UNHCRNumber!=null? a.UNHCRNumber: a.IDNumber,
                              BeneficiaryType=a.BeneficiaryTypeGUID.ToString(),
                              Gender=a.GenderGUID.ToString(),
                              OrganizationInstanceGUID=a.OrganizationInstanceGUID,
                              DocumentType=a.DocumentType,
                              Active = a.Active,
                              dataMedicalBeneficiaryRowVersion = a.dataMedicalBeneficiaryRowVersion
                          }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<MedicalBeneficiarysDataTableModel> Result = Mapper.Map<List<MedicalBeneficiarysDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult MedicalBeneficiaryCreate()
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiary.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.MedicalBeneficiary.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            return PartialView("~/Areas/EMT/Views/MedicalBeneficiarys/_MedicalBeneficiarysUpdateModal.cshtml",
                new dataMedicalBeneficiary() { BeneficiaryTypeGUID= Guid.Parse("abc17000-0000-0000-0000-000000000005"),OrganizationInstanceGUID=Guid.Parse( AuthorizedList.FirstOrDefault()),NationalityCode= "SYR" } );;
        }

        public ActionResult MedicalBeneficiaryUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiary.Access, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = DbEMT.dataMedicalBeneficiary.Find(PK);
            return PartialView("~/Areas/EMT/Views/MedicalBeneficiarys/_MedicalBeneficiarysUpdateModal.cshtml",model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalBeneficiaryCreate(dataMedicalBeneficiary model)
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiary.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalBeneficiary(model)) return PartialView("~/Areas/EMT/Views/MedicalBeneficiarys/_MedicalBeneficiarysUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            model.MedicalBeneficiaryGUID = Guid.NewGuid();
            if (model.NationalityCode == null) { model.NationalityCode = "SYR"; }
            DbEMT.Create(model, Permissions.MedicalBeneficiary.CreateGuid, ExecutionTime, DbCMS);


            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(DataTableNames.MedicalBeneficiarysDataTable, DbEMT.PrimaryKeyControl(model), DbEMT.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalBeneficiaryUpdate(dataMedicalBeneficiary model)
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiary.Update, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid ) return PartialView("~/Areas/EMT/Views/MedicalBeneficiarys/_MedicalBeneficiarysUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            DbEMT.Update(model, Permissions.MedicalBeneficiary.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(DataTableNames.MedicalBeneficiarysDataTable,
                    DbEMT.PrimaryKeyControl(model),
                    DbEMT.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalBeneficiary(model.MedicalBeneficiaryGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalBeneficiaryDelete(dataMedicalBeneficiary model)
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiary.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataMedicalBeneficiary> DeletedLanguages = DeleteMedicalBeneficiarys(new List<dataMedicalBeneficiary> { model });

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleDeleteMessage(DeletedLanguages, DataTableNames.MedicalBeneficiarysDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalBeneficiary(model.MedicalBeneficiaryGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalBeneficiaryRestore(dataMedicalBeneficiary model)
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiary.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveMedicalBeneficiary(model))
            {
                return Json(DbEMT.RecordExists());
            }

            List<dataMedicalBeneficiary> RestoredLanguages = RestoreMedicalBeneficiarys(Portal.SingleToList(model));

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleRestoreMessage(RestoredLanguages, DataTableNames.MedicalBeneficiarysDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalBeneficiary(model.MedicalBeneficiaryGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalBeneficiarysDataTableDelete(List<dataMedicalBeneficiary> models)
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiary.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataMedicalBeneficiary> DeletedLanguages = DeleteMedicalBeneficiarys(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.MedicalBeneficiarysDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }

        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalBeneficiarysDataTableRestore(List<dataMedicalBeneficiary> models)
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiary.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataMedicalBeneficiary> RestoredLanguages = RestoreMedicalBeneficiarys(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.MedicalBeneficiarysDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        private List<dataMedicalBeneficiary> DeleteMedicalBeneficiarys(List<dataMedicalBeneficiary> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataMedicalBeneficiary> DeletedMedicalBeneficiarys = new List<dataMedicalBeneficiary>();

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalBeneficiary.DeleteGuid, SubmitTypes.Delete, "");

            var MedicalBeneficiarys = DbEMT.Database.SqlQuery<dataMedicalBeneficiary>(query).ToList();

            //var MedicalBeneficiary = DbEMT.dataMedicalBeneficiary.Where(x => x.MedicalBeneficiaryGUID == models.FirstOrDefault().MedicalBeneficiaryGUID).FirstOrDefault();
            foreach (var MedicalBeneficiary in MedicalBeneficiarys)
            {
                DeletedMedicalBeneficiarys.Add(DbEMT.Delete(MedicalBeneficiary, ExecutionTime, Permissions.MedicalBeneficiary.DeleteGuid, DbCMS));

            }

            return DeletedMedicalBeneficiarys;
        }

        private List<dataMedicalBeneficiary> RestoreMedicalBeneficiarys(List<dataMedicalBeneficiary> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataMedicalBeneficiary> RestoredLanguages = new List<dataMedicalBeneficiary>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalBeneficiary.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var MedicalBeneficiarys = DbEMT.Database.SqlQuery<dataMedicalBeneficiary>(query).ToList();

            //var MedicalBeneficiary = DbEMT.dataMedicalBeneficiary.Where(x => x.MedicalBeneficiaryGUID == models.FirstOrDefault().MedicalBeneficiaryGUID).FirstOrDefault();
            foreach (var MedicalBeneficiary in MedicalBeneficiarys)
            {
                if (!ActiveMedicalBeneficiary(MedicalBeneficiary))
                {
                    RestoredLanguages.Add(DbEMT.Restore(MedicalBeneficiary, Permissions.MedicalBeneficiary.DeleteGuid, Permissions.MedicalBeneficiary.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyMedicalBeneficiary(Guid PK)
        {
            dataMedicalBeneficiary dbModel = new dataMedicalBeneficiary();

            var Language = DbEMT.dataMedicalBeneficiary.Where(l => l.MedicalBeneficiaryGUID == PK).FirstOrDefault();
            var dbLanguage = DbEMT.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataMedicalBeneficiaryRowVersion.SequenceEqual(dbModel.dataMedicalBeneficiaryRowVersion))
            {
                return Json(DbEMT.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbEMT, dbModel, "MedicalBeneficiarysContainer"));
        }

        private bool ActiveMedicalBeneficiary(dataMedicalBeneficiary model)
        {
            int LanguageID = DbEMT.dataMedicalBeneficiary
                                  .Where(x => x.IDNumber == model.IDNumber &&
                                              x.RefugeeFullName == model.RefugeeFullName &&
                                              x.OrganizationInstanceGUID ==model.OrganizationInstanceGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("MedicalBeneficiaryGUID", "Beneficiary already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }


        [HttpPost]
        public ActionResult MedicalBeneficiaryTransfer()
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiary.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            Guid UNHCR = Guid.Parse("E156C022-EC72-4A5A-BE09-163BD85C68EF");
            using (var DbPRG = new PRGEntities())
            {
                var Inds = DbEMT.dataMedicalBeneficiary.Select(x => x.MedicalBeneficiaryGUID).ToList();
                var Result = DbPRG.dataIndividual.AsNoTracking().Where(x => x.ProcessStatusCode == "A" && x.RefugeeStatusCode != "NOC").Select(x =>
                     new dataMedicalBeneficiary()
                     {
                         MedicalBeneficiaryGUID=x.IndividualGUID,
                         UNHCRNumber=x.IndividualID,
                         DocumentType=1,
                         NationalityCode=x.NationalityCode,
                         RefugeeFullName=x.VerbalName,
                         BeneficiaryTypeGUID= x.RefugeeStatusCode=="REF"? BeneficiaryType .REF: BeneficiaryType.ASR,
                         Brithday=x.DateofBirth.Value,
                         GenderGUID= x.SexCode == "F" ? Gender.F : Gender.M,
                         OrganizationInstanceGUID= UNHCR,
                         Active =true
                         
                     }).ToList();
                Result = Result.Where(x => !Inds.Contains(x.MedicalBeneficiaryGUID)).ToList();
                DbEMT.dataMedicalBeneficiary.AddRange(Result);
            }
            try
            {
                DbEMT.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(DataTableNames.MedicalBeneficiarysDataTable, null, null));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        public ActionResult GetBeneficiaryDetails(string ID)
        {
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.MedicalBeneficiary.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //UNHCR List
            AuthorizedList.Add("E156C022-EC72-4A5A-BE09-163BD85C68EF");
            var Result = DbEMT.dataMedicalBeneficiary.Where(x => x.MedicalBeneficiaryGUID.ToString() == ID ).Where(x => AuthorizedList.Contains(x.OrganizationInstanceGUID.ToString())).
                Select(x=> new
                {
                    x.RefugeeFullName,
                    x.Brithday,
                    x.NationalityCode

                }).FirstOrDefault();

            return Json(new { Result = Result }, JsonRequestBehavior.AllowGet);
        }

        public class  Gender
        {
            public static Guid F = Guid.Parse("DBF9D307-CE9F-4029-BD1E-D7AF6739975C");
            public static Guid M = Guid.Parse("688B11E0-24FB-44B8-94CE-D8568C4742C7");
        }
        public class BeneficiaryType
        {
            public static Guid REF = Guid.Parse("ABC17000-0000-0000-0000-000000000003");
            public static Guid ASR = Guid.Parse("ABC17000-0000-0000-0000-000000000001");
        }
        #endregion
    }
}