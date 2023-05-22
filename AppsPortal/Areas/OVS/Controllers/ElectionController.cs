using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.UI;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AppsPortal.OVS.ViewModels;
using AutoMapper;
using LinqKit;
using OfficeOpenXml;
using OVS_DAL.Model;
using RES_Repo.Globalization;
using iTextSharp.text.pdf.qrcode;

namespace AppsPortal.Areas.OVS.Controllers
{
    public class ElectionController : OVSBaseController
    {
        #region Election 

        [Route("OVS/Election/")]
        public ActionResult ElectionIndex()
        {
            //if (!CMS.HasAction(Permissions.ElectionsManagement.Access, Apps.OVS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            return View("~/Areas/OVS/Views/Election/Index.cshtml");
        }

        [Route("OVS/Election/ElectionsDataTable/")]
        public JsonResult ElectionsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ElectionDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ElectionDataTableModel>(DataTable.Filters);
            }
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.ElectionsManagement.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var All = (from a in DbOVS.dataElection.Where(x => AuthorizedList.Contains(x.DutyStationGUID.ToString()))
                       join b in DbOVS.dataElectionLanguage.Where(x =>
                               (x.Active == true ? x.Active : x.DeletedOn == x.dataElection.DeletedOn) &&
                               x.LanguageID == LAN) on
                           a.ElectionGUID equals b.ElectionGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join c in DbOVS.codeOrganizationsInstances on a.OrganizationInstanceGUID equals c.OrganizationInstanceGUID
                       join d in DbOVS.codeOrganizationsInstancesLanguages.Where(x => (x.Active == true) && x.LanguageID == LAN)
                           on c.OrganizationInstanceGUID equals d.OrganizationInstanceGUID into LJ2
                       from R2 in LJ2.DefaultIfEmpty()
                       join e in DbOVS.codeDutyStations on a.DutyStationGUID equals e.DutyStationGUID
                       join f in DbOVS.codeDutyStationsLanguages.Where(x => (x.Active == true) && x.LanguageID == LAN) on e.DutyStationGUID equals f.DutyStationGUID into LJ3
                       from R3 in LJ3.DefaultIfEmpty()

                       select new ElectionDataTableModel
                       {
                           ElectionGUID = a.ElectionGUID,
                           StartDate = a.StartDate,
                           CloseDate = a.CloseDate,
                           Active = a.Active,
                           dataElectionRowVersion = a.dataElectionRowVersion,
                           Delails = R1.Details,
                           TimeZone = a.TimeZone,
                           OrganizationInstance = R2.OrganizationInstanceDescription,
                           OrganizationInstanceGUID = R2.OrganizationInstanceGUID.ToString(),
                           DutyStation = R3.DutyStationDescription,
                           DutyStationGUID = R3.DutyStationGUID.ToString(),
                           Title = R1.Title == null ? resxDbFields.UnknownValue : R1.Title
                       }).Where(Predicate);
            if (!String.IsNullOrEmpty(DataTable.Order.OrderBy))
                All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ElectionDataTableModel> Result =
                Mapper.Map<List<ElectionDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result.OrderByDescending(x => x.CloseDate)), JsonRequestBehavior.AllowGet);
        }


        [Route("OVS/Election/Create/")]
        public ActionResult ElectionCreate()
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Create, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var userProfiles = DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            return View("~/Areas/OVS/Views/Election/Election.cshtml", new ElectionUpdateModel()
            {
                DutyStationGUID= userProfiles.DutyStationGUID!=null? userProfiles.DutyStationGUID:null,
                OrganizationInstanceGUID= userProfiles.OrganizationInstanceGUID
            });
        }

        [Route("OVS/Election/Update/{PK}")]
        public ActionResult ElectionUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Access, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            var model = (from a in DbOVS.dataElection.WherePK(PK)
                         join b in DbOVS.dataElectionLanguage.Where(x =>
                                 (x.Active == true ? x.Active : x.DeletedOn == x.dataElection.DeletedOn) && x.LanguageID == LAN)
                             on a.ElectionGUID equals b.ElectionGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new ElectionUpdateModel
                         {
                             ElectionGUID = a.ElectionGUID,
                             Active = a.Active,
                             dataElectionRowVersion = a.dataElectionRowVersion,
                             Details = R1.Details,
                             Title = R1.Title,
                             StartDate = a.StartDate,
                             CloseDate = a.CloseDate,
                             TimeZone = a.TimeZone,
                             OrganizationInstanceGUID = a.OrganizationInstanceGUID,
                             DutyStationGUID = a.DutyStationGUID,
                             dataElectionLanguageRowVersion = R1.dataElectionLanguageRowVersion
                         }).FirstOrDefault();

            if (model == null)
                throw new HttpException((int)HttpStatusCode.NotFound,
                    Url.Action("Election", "Election", new { Area = "OVS" }));

            return View("~/Areas/OVS/Views/Election/Election.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionCreate(ElectionUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Create, Apps.OVS,model.DutyStationGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid || ActiveElections(model))
                return PartialView("~/Areas/OVS/Views/Election/_ElectionForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            dataElection Election = Mapper.Map(model, new dataElection());
            Election.ElectionGUID = EntityPK;
            DbOVS.Create(Election, Permissions.ElectionsManagement.CreateGuid, ExecutionTime, DbCMS);

            dataElectionLanguage Language = Mapper.Map(model, new dataElectionLanguage());
            Language.ElectionGUID = EntityPK;

            DbOVS.Create(Language, Permissions.ElectionLanguages.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ElectionLanguagesDataTable, ControllerContext,
                "LanguagesContainer"));
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ElectionCandidatesDataTable, ControllerContext,
                "CandidatesContainer"));
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ElectionStaffsDataTable, ControllerContext,
                "StaffContainer"));
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ElectionConditionsDataTable, ControllerContext,
                "ConditionsContainer"));
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons
            {
                Button = HtmlExtension.CreateNewButton(Permissions.ElectionsManagement.Create, Apps.OVS,
                    new UrlHelper(Request.RequestContext).Action("ElectionCreate", "Election", new { Area = "OVS" })),
                Container = "ElectionFormControls"
            });
            UIButtons.Add(new UIButtons
            {
                Button = HtmlExtension.UpdateButton(Permissions.ElectionsManagement.Update, Apps.OVS),
                Container = "ElectionsFormControls"
            });
            UIButtons.Add(new UIButtons
            {
                Button = HtmlExtension.DeleteButton(Permissions.ElectionsManagement.Delete, Apps.OVS),
                Container = "ElectionsFormControls"
            });

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleCreateMessage(DbOVS.PrimaryKeyControl(Election),
                    DbOVS.RowVersionControls(Election, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionUpdate(ElectionUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Update, Apps.OVS, model.DutyStationGUID.ToString()))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid || ActiveElections(model))
                return PartialView("~/Areas/OVS/Views/Election/_ElectionForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataElection Election = Mapper.Map(model, new dataElection());
            DbOVS.Update(Election, Permissions.ElectionsManagement.UpdateGuid, ExecutionTime, DbCMS);

            var Language = DbOVS.dataElectionLanguage
                .Where(l => l.ElectionGUID == model.ElectionGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.ElectionGUID = Election.ElectionGUID;
                DbOVS.Create(Language, Permissions.ElectionLanguages.CreateGuid, ExecutionTime, DbCMS);
            }
            else if (Language.Details != model.Details)
            {
                Language = Mapper.Map(model, Language);
                DbOVS.Update(Language, Permissions.ElectionLanguages.UpdateGuid, ExecutionTime, DbCMS);
            }

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleUpdateMessage(null, null, DbOVS.RowVersionControls(Election, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyElections(model.ElectionGUID);
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionsDelete(dataElection model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Delete, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataElection> DeletedElections = DeleteElections(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons
            {
                Button = HtmlExtension.RestoreButton(Permissions.ElectionsManagement.Restore, Apps.OVS),
                Container = "ElectionsFormControls"
            });


            try
            {
                int CommitedRows = DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleDeleteMessage(CommitedRows, DeletedElections.FirstOrDefault(),
                    "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyElections(model.ElectionGUID);
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionsRestore(dataElection model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Restore, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (ActiveElections(model))
            {
                return Json(DbOVS.RecordExists());
            }

            List<dataElection> RestoredElections = RestoreElections(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons
            {
                Button = HtmlExtension.CreateNewButton(Permissions.ElectionsManagement.Create, Apps.OVS,
                    new UrlHelper(Request.RequestContext).Action("ElectionCreate", "Election", new { Area = "OVS" })),
                Container = "ElectionFormControls"
            });
            UIButtons.Add(new UIButtons
            {
                Button = HtmlExtension.UpdateButton(Permissions.ElectionsManagement.Update, Apps.OVS),
                Container = "ElectionsFormControls"
            });
            UIButtons.Add(new UIButtons
            {
                Button = HtmlExtension.DeleteButton(Permissions.ElectionsManagement.Delete, Apps.OVS),
                Container = "ElectionsFormControls"
            });

            try
            {
                int CommitedRows = DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleRestoreMessage(CommitedRows, RestoredElections,
                    DbOVS.PrimaryKeyControl(RestoredElections.FirstOrDefault()),
                    Url.Action(DataTableNames.ElectionLanguagesDataTable, Portal.GetControllerName(ControllerContext)),
                    "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyElections(model.ElectionGUID);
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ElectionsDataTableDelete(List<dataElection> models)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Delete, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataElection> DeletedElections = DeleteElections(models);

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.PartialDeleteMessage(DeletedElections, models, DataTableNames.ElectionsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ElectionsDataTableRestore(List<dataElection> models)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Restore, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataElection> RestoredElections = RestoreElections(models);

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.PartialRestoreMessage(RestoredElections, models, DataTableNames.ElectionsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        private List<dataElection> DeleteElections(List<dataElection> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataElection> DeletedElections = new List<dataElection>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbOVS.QueryBuilder(models, Permissions.ElectionsManagement.DeleteGuid, SubmitTypes.Delete,
                baseQuery);

            var Records = DbOVS.Database.SqlQuery<dataElection>(query).ToList();
            foreach (var record in Records)
            {
                DeletedElections.Add(DbOVS.Delete(record, ExecutionTime, Permissions.ElectionsManagement.DeleteGuid,
                    DbCMS));
            }

            var Languages = DeletedElections.SelectMany(a => a.dataElectionLanguage).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbOVS.Delete(language, ExecutionTime, Permissions.ElectionsManagement.DeleteGuid, DbCMS);
            }

            return DeletedElections;
        }

        private List<dataElection> RestoreElections(List<dataElection> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataElection> RestoredElections = new List<dataElection>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbOVS.QueryBuilder(models, Permissions.ElectionsManagement.DeleteGuid, SubmitTypes.Restore,
                baseQuery);

            var Records = DbOVS.Database.SqlQuery<dataElection>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveElections(record))
                {
                    RestoredElections.Add(DbOVS.Restore(record, Permissions.ElectionsManagement.DeleteGuid,
                        Permissions.ElectionsManagement.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var Languages = RestoredElections
                .SelectMany(x => x.dataElectionLanguage.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbOVS.Restore(language, Permissions.ElectionsManagement.DeleteGuid,
                    Permissions.ElectionsManagement.RestoreGuid, RestoringTime, DbCMS);
            }

            return RestoredElections;
        }

        private JsonResult ConcurrencyElections(Guid PK)
        {
            ElectionUpdateModel dbModel = new ElectionUpdateModel();

            var Elections = DbOVS.dataElection.Where(x => x.ElectionGUID == PK).FirstOrDefault();
            var dbElections = DbOVS.Entry(Elections).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbElections, dbModel);

            var Language = DbOVS.dataElectionLanguage.Where(x => x.ElectionGUID == PK).Where(x =>
                    (x.Active == true ? x.Active : x.DeletedOn == x.dataElection.DeletedOn) && x.LanguageID == LAN)
                .FirstOrDefault();
            var dbLanguage = DbOVS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Elections.dataElectionRowVersion.SequenceEqual(dbModel.dataElectionRowVersion) &&
                Language.dataElectionLanguageRowVersion.SequenceEqual(dbModel.dataElectionLanguageRowVersion))
            {
                return Json(DbOVS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbOVS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveElections(Object model)
        {
            dataElectionLanguage Elections = Mapper.Map(model, new dataElectionLanguage());
            int Details = DbOVS.dataElectionLanguage
                .Where(x => x.Details == Elections.Details &&
                            x.ElectionGUID != Elections.ElectionGUID &&
                            x.Active).Count();
            if (Details > 0)
            {
                ModelState.AddModelError("Details", "Elections is already exists");
            }

            return (Details > 0);
        }

        #endregion

        #region Elections Language

        //[Route("Configuration/ElectionLanguagesDataTable/{PK}")]
        public ActionResult ElectionLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null)
                return PartialView("~/Areas/OVS/Views/Election/_LanguagesDataTable.cshtml",
                    new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<dataElectionLanguage, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<dataElectionLanguage>(DataTable.Filters);
            }

            var Result = DbOVS.dataElectionLanguage.AsNoTracking().AsExpandable()
                .Where(x => x.LanguageID != LAN && x.ElectionGUID == PK).Where(Predicate)
                .Select(x => new
                {
                    x.ElectionLanguageGUID,
                    x.LanguageID,
                    x.Details,
                    x.Title,
                    x.dataElectionLanguageRowVersion
                });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }


        public ActionResult ElectionLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Create, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return PartialView("~/Areas/OVS/Views/Election/_LanguageUpdateModal.cshtml",
                new dataElectionLanguage { ElectionGUID = FK });
        }

        public ActionResult ElectionLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Access, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return PartialView("~/Areas/OVS/Views/Election/_LanguageUpdateModal.cshtml",
                DbOVS.dataElectionLanguage.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionLanguageCreate(dataElectionLanguage model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Create, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid || ActiveElectionLanguage(model))
                return PartialView("~/Areas/OVS/Views/Election/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbOVS.Create(model, Permissions.ElectionLanguages.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleUpdateMessage(DataTableNames.ElectionLanguagesDataTable,
                    DbOVS.PrimaryKeyControl(model), DbOVS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionLanguageUpdate(dataElectionLanguage model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Update, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid || ActiveElectionLanguage(model))
                return PartialView("~/Areas/OVS/Views/Election/_LanguageUpdateModal.cshtml", model);

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
                return ConcrrencyElectionLanguage(model.ElectionLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionLanguageDelete(dataElectionLanguage model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Delete, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataElectionLanguage> DeletedLanguages =
                DeleteElectionLanguages(new List<dataElectionLanguage> { model });

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleDeleteMessage(DeletedLanguages, DataTableNames.ElectionLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyElectionLanguage(model.ElectionLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionLanguageRestore(dataElectionLanguage model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Restore, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (ActiveElectionLanguage(model))
            {
                return Json(DbOVS.RecordExists());
            }

            List<dataElectionLanguage> RestoredLanguages = RestoreElectionLanguages(Portal.SingleToList(model));

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleRestoreMessage(RestoredLanguages, DataTableNames.ElectionLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyElectionLanguage(model.ElectionLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ElectionLanguagesDataTableDelete(List<dataElectionLanguage> models)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Delete, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataElectionLanguage> DeletedLanguages = DeleteElectionLanguages(models);

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.PartialDeleteMessage(DeletedLanguages, models,
                    DataTableNames.ElectionLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ElectionLanguagesDataTableRestore(List<dataElectionLanguage> models)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Restore, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataElectionLanguage> RestoredLanguages = RestoreElectionLanguages(models);

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.PartialRestoreMessage(RestoredLanguages, models,
                    DataTableNames.ElectionLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        private List<dataElectionLanguage> DeleteElectionLanguages(List<dataElectionLanguage> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataElectionLanguage> DeletedElectionLanguages = new List<dataElectionLanguage>();

            string query = DbOVS.QueryBuilder(models, Permissions.ElectionLanguages.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbOVS.Database.SqlQuery<dataElectionLanguage>(query).ToList();

            foreach (var language in languages)
            {
                DeletedElectionLanguages.Add(DbOVS.Delete(language, ExecutionTime,
                    Permissions.ElectionLanguages.DeleteGuid, DbCMS));
            }

            return DeletedElectionLanguages;
        }

        private List<dataElectionLanguage> RestoreElectionLanguages(List<dataElectionLanguage> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataElectionLanguage> RestoredLanguages = new List<dataElectionLanguage>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbOVS.QueryBuilder(models, Permissions.ElectionLanguages.DeleteGuid, SubmitTypes.Restore,
                baseQuery);

            var Languages = DbOVS.Database.SqlQuery<dataElectionLanguage>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveElectionLanguage(language))
                {
                    RestoredLanguages.Add(DbOVS.Restore(language, Permissions.ElectionLanguages.DeleteGuid,
                        Permissions.ElectionLanguages.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyElectionLanguage(Guid PK)
        {
            dataElectionLanguage dbModel = new dataElectionLanguage();

            var Language = DbOVS.dataElectionLanguage.Where(l => l.ElectionLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbOVS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataElectionLanguageRowVersion.SequenceEqual(dbModel.dataElectionLanguageRowVersion))
            {
                return Json(DbOVS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbOVS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveElectionLanguage(dataElectionLanguage model)
        {
            int LanguageID = DbOVS.dataElectionLanguage
                .Where(x => x.LanguageID == model.LanguageID &&
                            x.ElectionGUID == model.ElectionGUID &&
                            x.ElectionLanguageGUID != model.ElectionLanguageGUID &&
                            x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", resxMessages.ElecetionAlreadyExists);
            }

            return (LanguageID > 0);
        }

        #endregion

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
                              dataElectionConditionsRowVersion = a.dataElectionConditionRowVersion
                          });


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ElectionConditionCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return PartialView("~/Areas/OVS/Views/Condition/_ConditionUpdateModal.cshtml",
                new dataElectionCondition { ElectionGUID = FK });
        }

        public ActionResult ElectionConditionUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            var model = DbOVS.dataElectionCondition.Find(PK);

            return PartialView("~/Areas/OVS/Views/Condition/_ConditionUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionConditionCreate(dataElectionCondition model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Create, Apps.CMS))
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
            DbOVS.Create(model, Permissions.ElectionsManagement.CreateGuid, ExecutionTime, DbCMS);
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
            if (!CMS.HasAction(Permissions.ElectionsManagement.Update, Apps.CMS))
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
            DbOVS.Update(model, Permissions.ElectionsManagement.UpdateGuid, ExecutionTime, DbCMS);

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
            if (!CMS.HasAction(Permissions.ElectionsManagement.Delete, Apps.OVS))
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
                return ConcrrencyElectionLanguage(model.ElectionConditionGUID);
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionConditionRestore(dataElectionCondition model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Restore, Apps.OVS))
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
                return ConcrrencyElectionLanguage(model.ElectionConditionGUID);
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ElectionConditionsDataTableDelete(List<dataElectionCondition> models)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Delete, Apps.OVS))
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
            if (!CMS.HasAction(Permissions.ElectionsManagement.Restore, Apps.OVS))
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

            string query = DbOVS.QueryBuilder(models, Permissions.ElectionsManagement.DeleteGuid, SubmitTypes.Delete, "");

            var conditionses = DbOVS.Database.SqlQuery<dataElectionCondition>(query).ToList();

            foreach (var condition in conditionses)
            {
                DeletedElectionConditions.Add(DbOVS.Delete(condition, ExecutionTime,
                    Permissions.ElectionsManagement.DeleteGuid, DbCMS));
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
                    RestoredConditions.Add(DbOVS.Restore(condition, Permissions.ElectionsManagement.DeleteGuid,
                        Permissions.ElectionsManagement.RestoreGuid, RestoringTime, DbCMS));
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

        private string checkElectionCanidates(List<dataElectionCondition> Conditions, List<dataElectionCandidate> Candidateses)
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
                        && (candidatesCount < value || value<=0))
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

        #region Election Candidate

        public ActionResult ElectionCandidatesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null)
                return PartialView("~/Areas/OVS/Views/Candidate/_CandidatesDataTable.cshtml",
                    new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<dataElectionCandidate, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<dataElectionCandidate>(DataTable.Filters);
            }

            var Result = (from a in DbOVS.dataElectionCandidate.AsExpandable()
                    .Where(x => x.ElectionGUID == PK).Where(Predicate)

                          select new
                          {
                              ElectionCandidateGUID = a.ElectionCandidateGUID,
                              EmailAddress = a.EmailAddress,
                              FullName = a.FullName,
                              CampaignPlan = a.CampaignPlan,
                              dataElectionCandidatesRowVersion = a.dataElectionCandidateRowVersion
                          });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ElectionCandidateCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return PartialView("~/Areas/OVS/Views/Candidate/_CandidateUpdateModal.cshtml",
                new dataElectionCandidate { ElectionGUID = FK });
        }

        public ActionResult ElectionCandidateUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            var model = DbOVS.dataElectionCandidate.Find(PK);

            return PartialView("~/Areas/OVS/Views/Candidate/_CandidateUpdateModal.cshtml", model);
        }

        [HttpPost]
        public ActionResult ElectionCandidateCreate(ElectionCandidateModel upload)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            dataElectionCandidate model = Mapper.Map(upload, new dataElectionCandidate());
            List<dataElectionCondition> conditions = DbOVS.dataElectionCondition.Where(x=>x.ElectionGUID== upload.ElectionGUID).ToList();
            List<dataElectionCandidate> candidateses = DbOVS.dataElectionCandidate.Where(x => x.ElectionGUID == upload.ElectionGUID).ToList();
            candidateses.Add(model);
            string checkMessage = checkElectionCanidates(conditions, candidateses);
            if (checkMessage != "1")
            {
                return Json(DbOVS.ErrorMessage(checkMessage));
            }

            //dataElectionLanguage model = Mapper.Map<dataElectionLanguage>(upload);
            //dataElectionCandidate model = Mapper.Map(upload, new dataElectionCandidate());
            DateTime ExecutionTime = DateTime.Now;
            if (model.ElectionCandidateGUID == Guid.Empty)
            {
                bool checkCandidate = ActiveElectionCandidate(model);
                if (!ModelState.IsValid || checkCandidate)
                    return PartialView("~/Areas/OVS/Views/Candidate/_CandidateUpdateModal.cshtml", model);
                DbOVS.Create(model, Permissions.ElectionsManagement.CreateGuid, ExecutionTime, DbCMS);
            }
            else
                DbOVS.Update(model, Permissions.ElectionsManagement.UpdateGuid, ExecutionTime, DbCMS);

            try
            {

                //to change it to take from Webconfig
                Image image = Image.FromFile(Server.MapPath(@"\\Uploads") + "\\OVS\\CanidiatePhotos\\00000000-0000-0000-0000-000000000000.jpg");

                if (upload.InputStream != null)
                {
                    image = Image.FromStream(upload.InputStream);
                }

                //Original Size
                var dir = new FileInfo(WebConfigurationManager.AppSettings["DataFolder"] +
                                        "OVS\\CanidiatePhotos\\").Directory;
                if (dir != null) dir.Create();
                image.Save(
                    WebConfigurationManager.AppSettings["DataFolder"] + "\\OVS\\CanidiatePhotos\\LG_" +
                    model.ElectionCandidateGUID + ".jpg", ImageFormat.Jpeg);
                //1024 x 1024 pixel
                using (var newImage = ScaleImage(image, 300, 300))
                {
                    var directory =
                        new FileInfo(WebConfigurationManager.AppSettings["DataFolder"] +
                                        "\\OVS\\CanidiatePhotos\\").Directory;
                    if (directory != null) directory.Create();
                    newImage.Save(
                        WebConfigurationManager.AppSettings["DataFolder"] + "\\OVS\\CanidiatePhotos\\" +
                        model.ElectionCandidateGUID + ".jpg", ImageFormat.Jpeg);
                    //this is to remove no need it is just for testing at local
                    //newImage.Save(
                    //    Server.MapPath(@"\\Uploads") + "\\OVS\\CanidiatePhotos\\" +
                    //    model.ElectionCandidateGUID + ".jpg", ImageFormat.Jpeg);
                }

                //100 x 100 pixel
                using (var newImage = ScaleImage(image, 100, 100))
                {
                    var directory =
                        new FileInfo(WebConfigurationManager.AppSettings["DataFolder"] +
                                        "\\OVS\\CanidiatePhotos\\").Directory;
                    if (directory != null) directory.Create();
                    newImage.Save(
                        WebConfigurationManager.AppSettings["DataFolder"] + "\\OVS\\CanidiatePhotos\\XS_" +
                        model.ElectionCandidateGUID + ".jpg", ImageFormat.Jpeg);
                }


            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }
            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleUpdateMessage(DataTableNames.ElectionCandidatesDataTable,
            DbOVS.PrimaryKeyControl(model), DbOVS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionCandidateUpdate(dataElectionCandidate model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid)
                return PartialView("~/Areas/OVS/Views/Election/_CandidateUpdateModal.cshtml", model);


            DateTime ExecutionTime = DateTime.Now;

            DbOVS.Update(model, Permissions.ElectionsManagement.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleUpdateMessage(DataTableNames.ElectionCandidatesDataTable,
                    DbOVS.PrimaryKeyControl(model),
                    DbOVS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyElectionLanguage(model.ElectionCandidateGUID);
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }


        }


        public ActionResult ElectionCandidateDelete(dataElectionCandidate model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Delete, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataElectionCandidate> DeletedCandidate =
                DeleteElectionCandidates(new List<dataElectionCandidate> { model });

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleDeleteMessage(DeletedCandidate, DataTableNames.ElectionCandidatesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyElectionCandidate(model.ElectionCandidateGUID);
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionCandidateRestore(dataElectionCandidate model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Restore, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (ActiveElectionCandidate(model))
            {
                return Json(DbOVS.RecordExists());
            }

            List<dataElectionCandidate> RestoredCandidates = RestoreElectionCandidate(Portal.SingleToList(model));

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleRestoreMessage(RestoredCandidates, DataTableNames.ElectionLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyElectionCandidate(model.ElectionCandidateGUID);
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ElectionCandidatesDataTableDelete(List<dataElectionCandidate> models)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Delete, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataElectionCandidate> DeletedCandidates = DeleteElectionCandidates(models);

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.PartialDeleteMessage(DeletedCandidates, models,
                    DataTableNames.ElectionCandidatesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ElectionCandidatesDataTableRestore(List<dataElectionCandidate> models)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Restore, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataElectionCandidate> RestoredCandidates = RestoreElectionCandidate(models);

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.PartialRestoreMessage(RestoredCandidates, models,
                    DataTableNames.ElectionCandidatesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        private List<dataElectionCandidate> DeleteElectionCandidates(List<dataElectionCandidate> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataElectionCandidate> DeletedElectionCandidates = new List<dataElectionCandidate>();

            string query = DbOVS.QueryBuilder(models, Permissions.ElectionsManagement.DeleteGuid, SubmitTypes.Delete, "");

            var candidates = DbOVS.Database.SqlQuery<dataElectionCandidate>(query).ToList();

            foreach (var candidate in candidates)
            {
                DeletedElectionCandidates.Add(DbOVS.Delete(candidate, ExecutionTime,
                    Permissions.ElectionsManagement.DeleteGuid, DbCMS));
            }

            return DeletedElectionCandidates;
        }

        private List<dataElectionCandidate> RestoreElectionCandidate(List<dataElectionCandidate> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataElectionCandidate> RestoredCandidates = new List<dataElectionCandidate>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbOVS.QueryBuilder(models, Permissions.ElectionLanguages.DeleteGuid, SubmitTypes.Restore,
                baseQuery);

            var candidates = DbOVS.Database.SqlQuery<dataElectionCandidate>(query).ToList();
            foreach (var candidate in candidates)
            {
                if (!ActiveElectionCandidate(candidate))
                {
                    RestoredCandidates.Add(DbOVS.Restore(candidate, Permissions.ElectionsManagement.DeleteGuid,
                        Permissions.ElectionsManagement.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredCandidates;
        }

        private JsonResult ConcrrencyElectionCandidate(Guid PK)
        {
            dataElectionCandidate dbModel = new dataElectionCandidate();

            var candidate = DbOVS.dataElectionCandidate.Where(l => l.ElectionCandidateGUID == PK).FirstOrDefault();
            var dbcandidate = DbOVS.Entry(candidate).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbcandidate, dbModel);

            if (candidate.dataElectionCandidateRowVersion.SequenceEqual(dbModel.dataElectionCandidateRowVersion))
            {
                return Json(DbOVS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbOVS, dbModel, "CandidatesContainer"));
        }

        private bool ActiveElectionCandidate(dataElectionCandidate model)
        {
            int electionCandidate = DbOVS.dataElectionCandidate
                .Where(x => x.EmailAddress == model.EmailAddress &&
                            x.ElectionGUID == model.ElectionGUID &&

                            x.Active).Count();
            if (electionCandidate > 0)
            {
                ModelState.AddModelError("Condition", "Candidate is already exists");
            }

            return (electionCandidate > 0);
        }

        #endregion

        #region Election Staff
        public ActionResult ElectionStaffsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null)
                return PartialView("~/Areas/OVS/Views/Staff/_StaffsDataTable.cshtml",
                    new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<dataElectionStaff, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<dataElectionStaff>(DataTable.Filters);
            }

            var All = DbOVS.dataElectionStaff.AsNoTracking().AsExpandable().Where(x => x.ElectionGUID == PK)
                .Where(Predicate)
                .Select(a => new ElectionStaffModel
                {
                    ElectionStaffGUID = a.ElectionStaffGUID,
                    FullName = a.dataStaff.FullName,
                    EmailAddress = a.dataStaff.EmailAddress,
                    dataElectionStaffRowVersion = a.dataElectionStaffRowVersion
                });
            if (!String.IsNullOrEmpty(DataTable.Order.OrderBy))
                All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ElectionStaffModel> Result =
                Mapper.Map<List<ElectionStaffModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());
            //Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);




        }

        public ActionResult ElectionStaffCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Create, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return PartialView("~/Areas/OVS/Views/Staff/_StaffUpdateModal.cshtml",
                new dataElectionStaff { ElectionGUID = FK });
        }

        public ActionResult ElectionStaffUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Access, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return PartialView("~/Areas/OVS/Views/Staff/_StaffUpdateModal.cshtml",
                DbOVS.dataElectionStaff.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionStaffCreate(dataElectionStaff model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid || ActiveElectionStaff(model))
                return PartialView("~/Areas/OVS/Views/Staff/_StaffUpdateModal.cshtml", model);

            Guid ActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;
            if (DbOVS.dataStaff != null)
            {
                dataStaff currStaff =
                    DbOVS.dataStaff.FirstOrDefault(x => x.EmailAddress == model.dataStaff.EmailAddress);
                if (currStaff != null)
                {
                    model.StaffGUID = currStaff.StaffGUID;
                }

            }

            if (model.StaffGUID == null || model.StaffGUID == Guid.Empty)
            {
                DbOVS.Create(model.dataStaff, Permissions.ElectionsManagement.CreateGuid, ExecutionTime, DbCMS);
                DbOVS.SaveChanges();
                dataStaff newstaff = DbOVS.dataStaff.FirstOrDefault(x => x.EmailAddress == model.dataStaff.EmailAddress);
                if (newstaff != null) model.StaffGUID = newstaff.StaffGUID;
            }
            model.AccessKey = StringModel.RandomString(30, false);
            model.dataStaff = null;
            DbOVS.Create(model, Permissions.ElectionsManagement.CreateGuid, ExecutionTime, DbCMS);
            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleUpdateMessage(DataTableNames.ElectionStaffsDataTable,
                    DbOVS.PrimaryKeyControl(model), DbOVS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionStaffUpdate(dataElectionStaff model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid)
                return PartialView("~/Areas/OVS/Views/Staff/_StaffUpdateModal.cshtml", model);

            Guid ActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;

            DbOVS.Update(model, ActionGUID, ExecutionTime);

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleUpdateMessage(DataTableNames.ElectionStaffsDataTable,
                    DbOVS.PrimaryKeyControl(model),
                    DbOVS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyElectionStaff(model.ElectionStaffGUID);
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        public ActionResult ElectionStaffDelete(dataElectionStaff model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Delete, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataElectionStaff> DeletedStaffs =
                DeleteElectionStaffs(new List<dataElectionStaff> { model });

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleDeleteMessage(DeletedStaffs, DataTableNames.ElectionStaffsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyElectionStaff(model.ElectionStaffGUID);
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionStaffRestore(dataElectionStaff model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Restore, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (ActiveElectionStaff(model))
            {
                return Json(DbOVS.RecordExists());
            }

            List<dataElectionStaff> RestoredStaffs = RestoreElectionStaff(Portal.SingleToList(model));

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleRestoreMessage(RestoredStaffs, DataTableNames.ElectionStaffsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyElectionStaff(model.ElectionStaffGUID);
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ElectionStaffsDataTableDelete(List<dataElectionStaff> models)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Delete, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataElectionStaff> DeletedStaffs = DeleteElectionStaffs(models);

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.PartialDeleteMessage(DeletedStaffs, models,
                    DataTableNames.ElectionStaffsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ElectionStaffsDataTableRestore(List<dataElectionStaff> models)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Restore, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataElectionStaff> RestoredStaffs = RestoreElectionStaff(models);

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.PartialRestoreMessage(RestoredStaffs, models,
                    DataTableNames.ElectionStaffsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        private List<dataElectionStaff> DeleteElectionStaffs(List<dataElectionStaff> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataElectionStaff> DeletedElectionStaffs = new List<dataElectionStaff>();

            string query = DbOVS.QueryBuilder(models, Permissions.ElectionsManagement.DeleteGuid, SubmitTypes.Delete, "");

            var staffs = DbOVS.Database.SqlQuery<dataElectionStaff>(query).ToList();

            foreach (var staff in staffs)
            {
                DeletedElectionStaffs.Add(DbOVS.Delete(staff, ExecutionTime,
                    Permissions.ElectionsManagement.DeleteGuid, DbCMS));
            }

            return DeletedElectionStaffs;
        }

        private List<dataElectionStaff> RestoreElectionStaff(List<dataElectionStaff> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataElectionStaff> RestoredStaffs = new List<dataElectionStaff>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbOVS.QueryBuilder(models, Permissions.ElectionsManagement.DeleteGuid, SubmitTypes.Restore,
                baseQuery);

            var staffs = DbOVS.Database.SqlQuery<dataElectionStaff>(query).ToList();
            foreach (var staff in staffs)
            {
                if (!ActiveElectionStaff(staff))
                {
                    RestoredStaffs.Add(DbOVS.Restore(staff, Permissions.ElectionsManagement.DeleteGuid,
                        Permissions.ElectionsManagement.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredStaffs;
        }

        private JsonResult ConcrrencyElectionStaff(Guid PK)
        {
            dataElectionStaff dbModel = new dataElectionStaff();

            var staff = DbOVS.dataElectionCondition.Where(l => l.ElectionConditionGUID == PK).FirstOrDefault();
            var dbStaff = DbOVS.Entry(staff).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbStaff, dbModel);

            if (staff.dataElectionConditionRowVersion.SequenceEqual(dbModel.dataElectionStaffRowVersion))
            {
                return Json(DbOVS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbOVS, dbModel, "StaffsContainer"));
        }

        private bool ActiveElectionStaff(dataElectionStaff model)
        {
            int electionStaff = DbOVS.dataElectionStaff
                .Where(x => x.dataStaff.EmailAddress == model.dataStaff.EmailAddress &&
                            x.ElectionGUID == model.ElectionGUID &&

                            x.Active).Count();
            if (electionStaff > 0)
            {
                ModelState.AddModelError("Staff", "this staff is already exists");
            }

            return (electionStaff > 0);
        }



        public ActionResult ElectionStaffImport(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Create, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return PartialView("~/Areas/OVS/Views/Staff/_StaffImportModal.cshtml",
                new ElectionStaffModel { ElectionGUID = FK, FileImportStaffWarningMessage=resxMessages.FileImportStaffWarning });
        }
        [HttpPost]
        public ActionResult ElectionStaffImportCheck(ElectionStaffModel model)
        {
            if (HttpContext != null && HttpContext.Request.Files["file"].ContentLength > 0)
            {
                DataTable ds = importFile.ImportDataSet();
                List<ElectionStaffImportModel> staffs = new List<ElectionStaffImportModel>();
                for (int i = 0; i < ds.Rows.Count; i++)
                {
                    ElectionStaffImportModel staff = new ElectionStaffImportModel();
                    staff.FullName = ds.Rows[i][0].ToString();
                    staff.EmailAddress = ds.Rows[i][1].ToString();
                    if (string.IsNullOrEmpty(staff.FullName))
                        break;
                    staff.Status =
                    (DbOVS.dataStaff.Count(a => a.EmailAddress.ToLower().Equals(staff.EmailAddress.ToLower())) >
                     0);
                    staffs.Add(staff);
                }

                return Json(new { data = staffs }, JsonRequestBehavior.AllowGet);
            }
            return Json(DbOVS.ErrorMessage("Please Chose file"));

        }
        [HttpPost]
        public ActionResult ElectionStaffImport(List<ElectionStaffImportModel> model, Guid ElectionGUID)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Create, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (ElectionGUID == null || ElectionGUID == Guid.Empty)
            {
                throw new HttpException(401, "No Election");
            }
            else
            {
                //dataElection election = DbOVS.dataElection.Find(ElectionGUID);
                int totalRecords = 0;
                int totalaAddedRecord = 0;
                //List<string[]> notAddedData = new List<string[]>();
                List<dataStaff> nodAddedStaffs = new List<dataStaff>();
                List<dataStaff> staffsToBeAdded = new List<dataStaff>();
                List<dataStaff> staffsToBeUpdated = new List<dataStaff>();
                List<dataElectionStaff> electionStaffsToBeAdded = new List<dataElectionStaff>();
                DateTime executionTime = DateTime.Now;

                foreach (var item in model)
                {
                    dataStaff staff =
                            DbOVS.dataStaff.FirstOrDefault(a => a.EmailAddress.ToLower().Equals(item.EmailAddress.ToLower()));
                    if (staff == null)
                    {
                        staff = new dataStaff();
                        staff.StaffGUID = Guid.NewGuid();
                        staff.EmailAddress = item.EmailAddress;
                        staff.FullName = item.FullName;
                        staff.Active = true;
                        staffsToBeAdded.Add(staff);
                    }
                    else
                    {
                        staff.Active = true;
                        staffsToBeUpdated.Add(staff);
                    }
                    if (DbOVS.dataElectionStaff != null &&
                        DbOVS.dataElectionStaff.Count(x => x.StaffGUID == staff.StaffGUID && x.ElectionGUID == ElectionGUID) == 0)
                    {
                        dataElectionStaff electionStaff = new dataElectionStaff();
                        electionStaff.ElectionStaffGUID = Guid.NewGuid();
                        electionStaff.ElectionGUID = ElectionGUID;
                        electionStaff.StaffGUID = staff.StaffGUID;
                        electionStaff.AccessKey = StringModel.RandomString(30, false);
                        electionStaff.Active = true;
                        electionStaffsToBeAdded.Add(electionStaff);
                    }
                    else
                    {
                        dataStaff notAddedStaff = new dataStaff();
                        notAddedStaff.FullName = item.FullName;
                        notAddedStaff.EmailAddress = item.EmailAddress;
                        nodAddedStaffs.Add(notAddedStaff);
                    }

                }


                try
                {
                    DbOVS.CreateBulk(staffsToBeAdded, Permissions.ElectionsManagement.CreateGuid, executionTime,
                         DbCMS);
                    DbOVS.UpdateBulk(staffsToBeUpdated, Permissions.ElectionsManagement.CreateGuid, executionTime,
                        DbCMS);
                    DbOVS.CreateBulk(electionStaffsToBeAdded, Permissions.ElectionsManagement.CreateGuid,
                        executionTime, DbCMS);
                    DbOVS.SaveChanges();
                    DbCMS.SaveChanges();

                    JsonReturn jr = new JsonReturn()
                    {
                        Notify = new Notify { Type = MessageTypes.Success, Message = "Successfully Imported" }
                    };
                    return Json(jr);


                    //return Json(DbOVS.SuccessMessage("File is Imported  successfully"));
                }
                catch (Exception ex)
                {
                    return Json(DbOVS.ErrorMessage(ex.Message));
                }
            }


        }



        #region Mail
        public ActionResult ElectionStaffMailBrodcastCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Create, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            dataElection model = DbOVS.dataElection.WherePK(FK).FirstOrDefault();
            string URL = AppSettingsKeys.Domain + "OVS/VoteChecker/?accessKey=";
            string Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.VoteInvitaion + "</a>";
            string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
            ViewBag.MailMessage = resxEmails.VoteInvitationFormat.Replace("$InvitationDate", model.StartDate.ToString())
                 .Replace("$VoteName", model.dataElectionLanguage.FirstOrDefault(x => x.Active && x.LanguageID == LAN).Title)
                 .Replace("$CloseDate", model.CloseDate.ToString())
                 .Replace("$VoteLink", Anchor)
                 .Replace("$Link", Link);
            return PartialView("~/Areas/OVS/Views/Staff/_StaffMailModal.cshtml",
                model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionStaffMailBrodcastCreate(dataElection model)
        {
            string _message = "";
            var election = (from a in DbOVS.dataElection.WherePK(model.ElectionGUID)
                            join b in DbOVS.dataElectionLanguage.Where(x =>
                                    (x.Active == true ? x.Active : x.DeletedOn == x.dataElection.DeletedOn) &&
                                    x.LanguageID == LAN) on
                                a.ElectionGUID equals b.ElectionGUID into LJ1
                            from R1 in LJ1.DefaultIfEmpty()
                            select new ElectionUpdateModel
                            {
                                ElectionGUID = a.ElectionGUID,
                                Active = a.Active,
                                Details = R1.Details,
                                Title = R1.Title,
                                StartDate = a.StartDate,
                                CloseDate = a.CloseDate,
                            }).FirstOrDefault();
            List<dataElectionStaff> staffs =
                DbOVS.dataElectionStaff.Where(x => x.ElectionGUID == election.ElectionGUID && x.Active).ToList();
            if (staffs != null && staffs.Count > 0)
            {
                if (!string.IsNullOrEmpty(model.CustomMessage)) _message = model.CustomMessage;
                foreach (var item in staffs)
                {
                    string URL = AppSettingsKeys.Domain + "OVS/OVS/VoteChecker/?accessKey=" + item.AccessKey;
                    string Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.VoteInvitaion + "</a>";
                    string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                    if (string.IsNullOrEmpty(model.CustomMessage))
                        _message = resxEmails.VoteInvitationMessage.Replace("$FullName", item.dataStaff.FullName)
                            .Replace("$InvitationDate", election.StartDate.ToString())
                            .Replace("$CloseDate", election.CloseDate.ToString())
                            .Replace("$VoteName", election.Title)
                            .Replace("$VoteLink", Anchor)
                            .Replace("$Link", Link);
                    mail.Send(item.dataStaff.EmailAddress, resxEmails.VoteInvitationSubject, _message,"Voting System<Voting@unhcr.org>");
                }

                return Json(DbOVS.SuccessMessage("Vote invitation are sent successfully"));
            }
            else
            {
                return Json(DbOVS.ErrorMessage("No staff in the list "));
            }

        }

        public ActionResult ElectionStaffSingleMessgeCreate(dataElectionStaff model)
        {
           dataElectionStaff staff =
                DbOVS.dataElectionStaff.Where(x => x.ElectionGUID == model.ElectionGUID && x.Active).FirstOrDefault();
            if (staff != null )
            {
                string URL = AppSettingsKeys.Domain + "OVS/OVS/VoteChecker/?accessKey=" + staff.AccessKey;
                    string Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.VoteInvitaion + "</a>";
                    string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                 
                      string _message = resxEmails.VoteInvitationMessage.Replace("$FullName", staff.dataStaff.FullName)
                            .Replace("$InvitationDate", staff.dataElection.StartDate.ToString())
                            .Replace("$CloseDate", staff.dataElection.CloseDate.ToString())
                            .Replace("$VoteName", staff.dataElection.dataElectionLanguage.FirstOrDefault(x=>x.LanguageID==LAN).Title)
                            .Replace("$VoteLink", Anchor)
                            .Replace("$Link", Link);
                    mail.Send(staff.dataStaff.EmailAddress, resxEmails.VoteInvitationSubject, _message, "Voting System<Voting@unhcr.org>");
                

                return Json(DbOVS.SuccessMessage("Vote invitation are sent successfully"));
            }
            else
            {
                return Json(DbOVS.ErrorMessage("No staff in the list "));
            }

        }
        #endregion



        #endregion

        #region ElectionResult

        [Route("OVS/Election/Result/{PK}")]
        public ActionResult ElectionResultIndex(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Access, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbOVS.dataElection.WherePK(PK)

                         select new ElectionResulteModel
                         {
                             ElectionGUID = a.ElectionGUID,
                             Active = a.Active,
                             dataElectionRowVersion = a.dataElectionRowVersion

                         }).FirstOrDefault();
            if (model != null)
            {
                model.TotalInvitedForElection =
                    DbOVS.dataElectionStaff.Count(x => x.ElectionGUID == PK && x.Active);
                model.TotalVotedForElection =
                    DbOVS.dataElectionStaff.Count(x => x.ElectionGUID == PK && x.Active && x.VoteDate != null);
                model.TotalNotVotedForElection =
                    DbOVS.dataElectionStaff.Count(x => x.ElectionGUID == PK && x.Active && x.VoteDate == null);
            }
            return View("~/Areas/OVS/Views/Election/ElectionResult.cshtml", model);

        }



        public ActionResult ElectionResultsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null)
                return PartialView("~/Areas/OVS/Views/Election/_ResultsDataTable.cshtml",
                    new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<dataElectionCandidate, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<dataElectionCandidate>(DataTable.Filters);
            }

            int electionTotalVotes = DbOVS.dataStaffVote.Count(a => a.dataElectionCandidate.ElectionGUID == PK);

            
            var result = (
                from b in DbOVS.dataElectionCandidate.Where(x => x.ElectionGUID == PK)
                join c in DbOVS.dataStaffVote
                    on b.ElectionCandidateGUID equals c.ElectionCandidateGUID into Votes
                join d in DbOVS.codeTablesValues.Where(x => x.TableGUID == LookupTables.Gender && x.Active) on b.GenderGUID equals d.ValueGUID
                join e in DbOVS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on d.ValueGUID equals e.ValueGUID
                from R1 in Votes
                group R1 by new
                {
                    R1.ElectionCandidateGUID,
                    R1.dataElectionCandidate.FullName,
                    e.ValueDescription
                }
                into Votegroup

                select new ElectionResultDataTableModel()
                {
                    ElectionCandidateGUID = Votegroup.Key.ElectionCandidateGUID,
                    FullName = Votegroup.Key.FullName,
                    CandidateGender = Votegroup.Key.ValueDescription.ToString(),
                    TotalVotes = Votegroup.Count(),
                    VoteRate = electionTotalVotes == 0 ? "0" : Votegroup.Count() * 100 / electionTotalVotes + " %"
                });
            result = SearchHelper.OrderByDynamic(result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            //foreach (var v in result)
            //{
            //    if ( v.CandidateGender != null && v.CandidateGender!="")
            //    {
            //        Guid genderGUID = Guid.Parse(v.CandidateGender);
            //        v.CandidateGender = DbCMS.codeTablesValues
            //            .FirstOrDefault(c => c.TableGUID == LookupTables.Gender && c.ValueGUID == genderGUID)
            //            .codeTablesValuesLanguages.FirstOrDefault(xx => xx.LanguageID == LAN).ValueDescription;
            //    }
            //}
            return Json(Portal.DataTable(result.Count(), result), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FK"></param>
        /// <returns></returns>
        //[HttpPost]
        public FileResult ExportVoteResult(Guid FK)
        {
            DataSourceSelectArguments args = new DataSourceSelectArguments();
            string myReportName = "";
            string header1 = resxDbFields.ElectionResult + " " + DbOVS.dataElectionLanguage
                                 .FirstOrDefault(x => x.ElectionGUID == FK && x.LanguageID == LAN).Title;

            DataTable dt = new DataTable();
            myReportName = "Election_Result.xlsx";
            //header1 = "Data OVS";
            int electionTotalVotes = DbOVS.dataStaffVote.Count(a => a.dataElectionCandidate.ElectionGUID == FK);
            var result = (
                from b in DbOVS.dataElectionCandidate.Where(x => x.ElectionGUID == FK)
                join c in DbOVS.dataStaffVote
                    on b.ElectionCandidateGUID equals c.ElectionCandidateGUID into Votes
                from R1 in Votes
                group R1 by new
                {
                    R1.ElectionCandidateGUID,
                    R1.dataElectionCandidate.FullName,
                    R1.dataElectionCandidate.GenderGUID
                }
                into Votegroup
                select new ElectionResultDataTableModel()
                {
                    ElectionCandidateGUID = Votegroup.Key.ElectionCandidateGUID,
                    FullName = Votegroup.Key.FullName,
                    CandidateGender = Votegroup.Key.GenderGUID.ToString(),
                    TotalVotes = Votegroup.Count(),
                    VoteRate = electionTotalVotes == 0 ? "0" : Votegroup.Count() * 100 / electionTotalVotes + " %"
                }).ToList();
            dt.Columns.Add("Candidate Name", typeof(string));
            dt.Columns.Add("Total Votes", typeof(int));
            dt.Columns.Add("OVS Rate", typeof(string));
            foreach (var item in result)
            {
                DataRow dr;
                dr = dt.NewRow();
                dr[0] = item.FullName;
                dr[1] = item.TotalVotes;
                dr[2] = item.VoteRate;
                dt.Rows.Add(dr);
            }
            string oFileName = Guid.NewGuid().ToString() + ".xlsx";
            //File 
            System.IO.File.Copy(HttpContext.Server.MapPath("~/Areas/OVS/Templates/Excel/" + myReportName), HttpContext.Server.MapPath("~/Areas/OVS/Templates/temp/" + oFileName));


            FileInfo file = new FileInfo(HttpContext.Server.MapPath("~/Areas/OVS/Templates/temp/" + oFileName));
            //System.IO.File.Copy(
            //    HttpContext.Server.MapPath(WebConfigurationManager.AppSettings["DataFolder"] +
            //                               "\\OVS\\ExcelTemplates\\" + myReportNam),
            //    HttpContext.Server.MapPath(WebConfigurationManager.AppSettings["DataFolder"] +
            //                               "\\OVS\\Temp\\" + oFileName));
            //FileInfo file1 = new FileInfo(HttpContext.Server.MapPath(WebConfigurationManager.AppSettings["DataFolder"] +
            //                                                        "\\OVS\\Temp\\" + oFileName));
            ExcelPackage pck = new ExcelPackage(file);
            //Add the content sheet
            var ws = pck.Workbook.Worksheets["Report"];
            ws.View.ShowGridLines = false;
            ws.Cells["B11"].LoadFromDataTable(dt, true);
            ws.Cells["B6"].Value = DbOVS.dataElectionLanguage.FirstOrDefault(x => x.ElectionGUID == FK && x.LanguageID == LAN).Title;
            ws.Cells["C6"].Value = DbOVS.dataElection.FirstOrDefault(x => x.ElectionGUID == FK).StartDate;
            ws.Cells["D6"].Value = DbOVS.dataElection.FirstOrDefault(x => x.ElectionGUID == FK).CloseDate;
            ws.Cells["B9"].Value = DbOVS.dataElectionStaff.Count(x => x.ElectionGUID == FK && x.Active);
            ws.Cells["C9"].Value = DbOVS.dataElectionStaff.Count(x => x.ElectionGUID == FK && x.Active && x.VoteDate == null);
            ws.Cells["D9"].Value = DbOVS.dataElectionStaff.Count(x => x.ElectionGUID == FK && x.Active && x.VoteDate != null);


            //ws.Cells["B1"].Value = header1;
            //ws.Cells["B2"].Value = header2 + Db.dataCycles.Where(c => c.CycleGUID == cycleGuid).Select(c => c.CycleNumber).FirstOrDefault();
            ws.Cells["B3"].Value = resxDbFields.ReportExcecutionTime + " " + DateTime.Now.ToString("MMMM dd, yyyy hh:ss:mm tt");
            ws.Cells["F6:O6"].Style.Numberformat.Format = "#,##0";
            if (file.Exists)
            {
                HttpContext.Response.Clear();
                HttpContext.Response.Buffer = true;
                HttpContext.Response.Charset = "";
                HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Response.AddHeader("content-disposition", "attachment;filename=Reprot" + DateTime.Now.ToShortDateString() + ".xlsx");
                using (MemoryStream myMemoryStream = new MemoryStream())
                {
                    pck.SaveAs(myMemoryStream);
                    myMemoryStream.WriteTo(HttpContext.Response.OutputStream);
                    HttpContext.Response.Flush();
                    HttpContext.Response.End();
                    return File(myMemoryStream, "application/vnd.ms-excel", "temp.xlsx");
                }

            }

            return null;
        }

        #endregion

        #region Image Scale
        private Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }
        #endregion

    }
}
