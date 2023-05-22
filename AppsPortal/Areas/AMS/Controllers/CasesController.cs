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
    public class CasesController : AMSBaseController
    {
        #region Cases
        public ActionResult Index()
        {
            return View();
        }

        [Route("AMS/Cases/")]
        public ActionResult CasesIndex()
        {
            if (!CMS.HasAction(Permissions.Case.Access, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/AMS/Views/Cases/Index.cshtml");
        }

        [Route("AMS/CasesDataTable/")]
        public JsonResult CasesDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<CasesDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<CasesDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.Appointment.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();
 
            var All = (from a in DbAMS.v_CaseAppointments.AsExpandable().Where(x => AuthorizedList.Contains(x.DutyStationGUID.ToString()))
                       select new CasesDataTableModel
                       {
                           CaseGUID=a.CaseGUID,
                           CaseSize=a.CaseSize,
                           CountryDescription= a.CountryDescription,
                           CountryGUID=a.CountryGUID.ToString(),
                           FileNumber=a.FileNumber!=string.Empty?a.FileNumber:"000-00C00000",
                           ICNameEN=a.ICNameEN,
                           ICNameOtherLanguages=a.ICNameOtherLanguages,
                           PhoneNumber =  a.PhoneNumber,
                           dataCaseRowVersion =a.dataCaseRowVersion,
                           Active=a.Active
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<CasesDataTableModel> Result = Mapper.Map<List<CasesDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("AMS/Cases/Create/")]
        public ActionResult CaseCreate()
        {
            if (!CMS.HasAction(Permissions.Case.Create, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            string DutyStationGUID = DbAMS.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault().DutyStationGUID.ToString();

            return View("~/Areas/AMS/Views/Cases/Case.cshtml", new CaseUpdateModel() { DutyStationGUID = Guid.Parse( DutyStationGUID) });
        }

        [Route("AMS/Cases/Update/{PK}")]
        public ActionResult CaseUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.Case.Access, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbAMS.dataCase.WherePK(PK)
                         join b in DbAMS.dataFile on a.FileNumber equals b.FileNumber into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new CaseUpdateModel
                         {
                             CaseGUID = a.CaseGUID,
                             CaseSize=a.CaseSize,
                             FileNumber=a.FileNumber,
                             ICNameOtherLanguages=a.ICNameOtherLanguages,
                             ICNameEN=a.ICNameEN,
                             HouseHold=a.HouseHold,
                             Comments=a.Comments,
                             CountryGUID=a.CountryGUID,
                             DutyStationGUID = a.DutyStationGUID,
                             Active = a.Active,
                             dataCaseRowVersion = a.dataCaseRowVersion,
                             ProcessStatusName=R1.ProcessStatusName
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("Case", "Cases", new { Area = "AMS" }));

            return View("~/Areas/AMS/Views/Cases/Case.cshtml", model);
        }

        public ActionResult LoadPrgresCase(string FileNumber)
        {
            Guid DutyStationGUID =Guid.Parse( DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.Appointment.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().FirstOrDefault());
            //var userProfiles = DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID && x.Active).FirstOrDefault();
            var model = (from a in DbPRG.dataIndividualV4.Where(x=>x.Relationship== "FP" && x.ProcessingGroupNumber==FileNumber)
                         join b in DbPRG.dataProcessGroupV4 on a.ProcessingGroupGUID equals b.ProcessingGroupGUID
                         select new CaseUpdateModel
                         {
                             CaseGUID = Guid.Empty,
                             CaseSize = b.ProcessingGroupSize,
                             FileNumber = b.ProcessingGroupNumber,
                             ICNameOtherLanguages = a.VerbalName,
                             ICNameEN = a.GivenName +" "+ a.FamilyName,
                             HouseHold = "0",
                             CountryCode = a.OriginCountryCode,
                             DutyStationGUID = DutyStationGUID
                         }).FirstOrDefault();
            if (model != null)
            {
                model.CountryGUID = (from a in DbCMS.codeCountries.Where(x => x.CountryA3Code == model.CountryCode && x.Active) select a).FirstOrDefault().CountryGUID;
                if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("Case", "Cases", new { Area = "AMS" }));
                return PartialView("~/Areas/AMS/Views/Cases/_CaseForm.cshtml", model);
            }
            else
            {
                ModelState.AddModelError("FileNumber", "File Number Not Found !");
                return PartialView("~/Areas/AMS/Views/Cases/_CaseForm.cshtml", new CaseUpdateModel() { DutyStationGUID = DutyStationGUID });
               // return Json(DbAMS.ErrorMessage("Case Not Found !"));
            }

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CaseCreate(CaseUpdateModel model)
        {
            //if (!CMS.HasAction(Permissions.Case.Create, Apps.AMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            if (!ModelState.IsValid || ActiveCase(model)) return PartialView("~/Areas/AMS/Views/Cases/_CaseForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            dataCase Case = Mapper.Map(model, new dataCase());
            Case.CaseGUID = EntityPK;
            DbAMS.Create(Case, Permissions.Case.CreateGuid, ExecutionTime, DbCMS);


            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.AppointmentsDataTable, "Appointments", "TabContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Case.Create, Apps.AMS, new UrlHelper(Request.RequestContext).Action("Create", "Cases", new { Area = "AMS" })), Container = "CaseFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Case.Update, Apps.AMS), Container = "CaseFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Case.Delete, Apps.AMS), Container = "CaseFormControls" });
            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.SingleCreateMessage(DbAMS.PrimaryKeyControl(Case), 
                    DbAMS.RowVersionControls(new List<dataCase>() { Case }), Partials, "", UIButtons));
               
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CaseUpdate(CaseUpdateModel model)
        {
            Guid DutyStationGUID = Guid.Parse(DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.Appointment.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().FirstOrDefault());
            if (!CMS.HasAction(Permissions.Case.Update, Apps.AMS, model.DutyStationGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveCase(model)) return PartialView("~/Areas/AMS/Views/Cases/_CaseForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataCase Case = Mapper.Map(model, new dataCase());
            DbAMS.Update(Case, Permissions.Case.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.SingleUpdateMessage(null, null, DbAMS.RowVersionControls(new List<dataCase>() { Case })));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyCase(model.CaseGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CaseDelete(dataCase model)
        {
            Guid DutyStationGUID = Guid.Parse(DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.Appointment.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().FirstOrDefault());

            if (!CMS.HasAction(Permissions.Case.Delete, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataCase> DeletedCase = DeleteCases(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.Case.Restore, Apps.AMS), Container = "CaseFormControls" });

            try
            {
                int CommitedRows = DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.SingleDeleteMessage(CommitedRows, DeletedCase.FirstOrDefault(), "TabContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyCase(model.CaseGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CaseRestore(dataCase model)
        {
            Guid DutyStationGUID = Guid.Parse(DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.Appointment.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().FirstOrDefault());

            if (!CMS.HasAction(Permissions.Case.Restore, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveCase(model))
            {
                return Json(DbAMS.RecordExists());
            }

            List<dataCase> RestoredCases = RestoreCases(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.Case.Create, Apps.AMS, new UrlHelper(Request.RequestContext).Action("CaseCreate", "Cases", new { Area = "AMS" })), Container = "CaseFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.Case.Update, Apps.AMS), Container = "CaseFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.Case.Delete, Apps.AMS), Container = "CaseFormControls" });

            try
            {
                int CommitedRows = DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.SingleRestoreMessage(CommitedRows, RestoredCases, DbAMS.PrimaryKeyControl(RestoredCases.FirstOrDefault()), 
                    Url.Action(DataTableNames.AppointmentsDataTable, "Appointments")
                    , "TabContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyCase(model.CaseGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CasesDataTableDelete(List<dataCase> models)
        {
            if (!CMS.HasAction(Permissions.Case.Delete, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataCase> DeletedCases = DeleteCases(models);

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.PartialDeleteMessage(DeletedCases, models, DataTableNames.CasesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CasesDataTableRestore(List<dataCase> models)
        {
            if (!CMS.HasAction(Permissions.Case.Restore, Apps.AMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataCase> RestoredCases = RestoreCases(models);

            try
            {
                DbAMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAMS.PartialRestoreMessage(RestoredCases, models, DataTableNames.CasesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAMS.ErrorMessage(ex.Message));
            }
        }

        private List<dataCase> DeleteCases(List<dataCase> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataCase> DeletedCases = new List<dataCase>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";


            string query = DbAMS.QueryBuilder(models, Permissions.Case.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbAMS.Database.SqlQuery<dataCase>(query).ToList();
            foreach (var record in Records)
            {
                DeletedCases.Add(DbAMS.Delete(record, ExecutionTime, Permissions.Case.DeleteGuid, DbCMS));
            }

            var appointments = DeletedCases.SelectMany(a => a.dataAppointment).Where(l => l.Active).ToList();
            foreach (var appointment in appointments)
            {
                DbAMS.Delete(appointment, ExecutionTime, Permissions.Case.DeleteGuid, DbCMS);
            }
            return DeletedCases;
        }

        private List<dataCase> RestoreCases(List<dataCase> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataCase> RestoredCases = new List<dataCase>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbAMS.QueryBuilder(models, Permissions.Case.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbAMS.Database.SqlQuery<dataCase>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveCase(record))
                {
                    RestoredCases.Add(DbAMS.Restore(record, Permissions.Appointment.DeleteGuid, Permissions.Case.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var appointments = RestoredCases.SelectMany(x => x.dataAppointment.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var appointment in appointments)
            {
                DbAMS.Restore(appointment, Permissions.Appointment.DeleteGuid, Permissions.Appointment.RestoreGuid, RestoringTime, DbCMS);
            }

            return RestoredCases;
        }

        private JsonResult ConcurrencyCase(Guid PK)
        {
            CaseUpdateModel dbModel = new CaseUpdateModel();

            var Case = DbAMS.dataCase.Where(x => x.CaseGUID == PK).FirstOrDefault();
            var dbCase = DbAMS.Entry(Case).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbCase, dbModel);

            var Language = DbAMS.dataCase.Where(x => x.CaseGUID == PK).Where(x => x.Active == true ).FirstOrDefault();
            var dbLanguage = DbAMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Case.dataCaseRowVersion.SequenceEqual(dbModel.dataCaseRowVersion) && Language.dataCaseRowVersion.SequenceEqual(dbModel.dataCaseRowVersion))
            {
                return Json(DbAMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbAMS, dbModel, "TabContainer"));
        }

        private bool ActiveCase(Object model)
        {
            dataCase Case = Mapper.Map(model, new dataCase());
            int CaseDescription = DbAMS.dataCase
                                    .Where(x => x.ICNameEN == Case.ICNameEN &&
                                                x.CaseGUID != Case.CaseGUID &&
                                                x.Active).Count();
            if (CaseDescription > 0)
            {
                ModelState.AddModelError("CaseDescription", "Case is already exists");
            }
            return (CaseDescription > 0);
        }

        #endregion
    }
}