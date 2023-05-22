using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Services;
using AppsPortal.ViewModels;
using AutoMapper;
using DAS_DAL.Model;
using DAS_DAL.ViewModels;
using iTextSharp.text;
using RES_Repo.Globalization;
using iTextSharp.text.pdf;
using LinqKit;
using static AppsPortal.Library.DataTableNames;
using FineUploader;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Image = iTextSharp.text.Image;

namespace AppsPortal.Areas.DAS.Controllers
{
    public class ConfigurationController : DASBaseController
    {
        // GET: DAS/Configuration
        #region Units

        public ActionResult UnitHome()
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Access, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/DAS/Views/Units/Index.cshtml");
        }


        [Route("DAS/Units/")]
        public ActionResult UnitIndex()
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Access, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/DAS/Views/Units/Index.cshtml");
        }

        [Route("DAS/UnitsDataTable/")]
        public JsonResult UnitsDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<UnitDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<UnitDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.DASConfiguration.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
            var All = (

                from a in DbDAS.codeDASDestinationUnit.Where(x => x.Active).AsExpandable()
                join b in DbDAS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN && x.codeTablesValues.TableGUID == LookupTables.DASproGresSiteOwner) on a.SiteOwnerGUID equals b.ValueGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()

                select new UnitDataTableModel
                {
                    DestinationUnitGUID = a.DestinationUnitGUID.ToString(),
                    Active = a.Active,
                    DestinationUnitName = a.DestinationUnitName,
                    SiteOwner = R1.ValueDescription,
                    codeDASDestinationUnitRowVersion = a.codeDASDestinationUnitRowVersion,

                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<UnitDataTableModel> Result = Mapper.Map<List<UnitDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("DAS/Units/Create/")]
        public ActionResult UnitCreate()
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/DAS/Views/Units/Units.cshtml", new UnitUpdateModel { DestinationUnitGUID = Guid.Empty });
        }

        [Route("DAS/Units/Update/{PK}")]
        public ActionResult UnitUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Update, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbDAS.codeDASDestinationUnit.WherePK(PK)

                         select new UnitUpdateModel
                         {
                             DestinationUnitGUID = a.DestinationUnitGUID,
                             DestinationUnitName = a.DestinationUnitName,
                             codeDASDestinationUnitRowVersion = a.codeDASDestinationUnitRowVersion,
                             SiteOwnerGUID = a.SiteOwnerGUID,


                             Active = a.Active,

                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("Units", "Units", new { Area = "DAS" }));

            return View("~/Areas/DAS/Views/Units/Units.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UnitCreate(UnitUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveUnit(model)) return PartialView("~/Areas/DAS/Views/Units/_UnitForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeDASDestinationUnit Unit = Mapper.Map(model, new codeDASDestinationUnit());
            Unit.DestinationUnitGUID = EntityPK;
            Unit.SiteOwnerGUID = model.SiteOwnerGUID;
            DbDAS.Create(Unit, Permissions.DASConfiguration.CreateGuid, ExecutionTime, DbCMS);


            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.UnitFocalPointsDataTable, ControllerContext, "UnitFocalPointFormControls"));


            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.DASConfiguration.Create, Apps.DAS, new UrlHelper(Request.RequestContext).Action("Create", "Units", new { Area = "DAS" })), Container = "UnitDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.DASConfiguration.Update, Apps.DAS), Container = "UnitDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.DASConfiguration.Delete, Apps.DAS), Container = "UnitDetailFormControls" });

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleCreateMessage(DbDAS.PrimaryKeyControl(Unit), DbDAS.RowVersionControls(Unit, Unit), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }



        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UnitUpdate(UnitUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Update, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/DAS/Views/Units/_UnitForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeDASDestinationUnit Unit = Mapper.Map(model, new codeDASDestinationUnit());
            DbDAS.Update(Unit, Permissions.DASConfiguration.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleUpdateMessage(null, null, DbDAS.RowVersionControls(Unit, Unit)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyUnit((Guid)model.DestinationUnitGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UnitDelete(codeDASDestinationUnit model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Delete, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDASDestinationUnit> DeletedUnit = DeleteUnit(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.DASConfiguration.Restore, Apps.DAS), Container = "UnitFormControls" });

            try
            {
                int CommitedRows = DbDAS.SaveChanges();
                DbDAS.SaveChanges();
                return Json(DbDAS.SingleDeleteMessage(CommitedRows, DeletedUnit.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyUnit(model.DestinationUnitGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UnitRestore(codeDASDestinationUnit model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Restore, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveUnit(model))
            {
                return Json(DbDAS.RecordExists());
            }

            List<codeDASDestinationUnit> RestoredUnit = RestoreUnit(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.DASConfiguration.Create, Apps.DAS, new UrlHelper(Request.RequestContext).Action("UnitCreate", "Configuration", new { Area = "DAS" })), Container = "UnitFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.DASConfiguration.Update, Apps.DAS), Container = "UnitFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.DASConfiguration.Delete, Apps.DAS), Container = "UnitFormControls" });

            try
            {
                int CommitedRows = DbDAS.SaveChanges();
                DbDAS.SaveChanges();
                return Json(DbDAS.SingleRestoreMessage(CommitedRows, RestoredUnit, DbDAS.PrimaryKeyControl(RestoredUnit.FirstOrDefault()), Url.Action(DataTableNames.UnitFocalPointsDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyUnit(model.DestinationUnitGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult UnitsDataTableDelete(List<codeDASDestinationUnit> models)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Delete, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDASDestinationUnit> DeletedUnit = DeleteUnit(models);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.PartialDeleteMessage(DeletedUnit, models, DataTableNames.UnitsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult UnitDataTableRestore(List<codeDASDestinationUnit> models)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Restore, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDASDestinationUnit> RestoredUnit = RestoreUnit(models);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.PartialRestoreMessage(RestoredUnit, models, DataTableNames.UnitsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        private List<codeDASDestinationUnit> DeleteUnit(List<codeDASDestinationUnit> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<codeDASDestinationUnit> DeletedUnit = new List<codeDASDestinationUnit>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT DestinationUnitGUID,CONVERT(varchar(50), DestinationUnitGUID) as C2 ,codeDASDestinationUnitRowVersion FROM code.codeDASDestinationUnit where DestinationUnitGUID in (" + string.Join(",", models.Select(x => "'" + x.DestinationUnitGUID + "'").ToArray()) + ")";

            string query = DbDAS.QueryBuilder(models, Permissions.DASConfiguration.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbDAS.Database.SqlQuery<codeDASDestinationUnit>(query).ToList();
            foreach (var record in Records)
            {
                DeletedUnit.Add(DbDAS.Delete(record, ExecutionTime, Permissions.DASConfiguration.DeleteGuid, DbCMS));
            }

            return DeletedUnit;
        }

        private List<codeDASDestinationUnit> RestoreUnit(List<codeDASDestinationUnit> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<codeDASDestinationUnit> RestoredUnit = new List<codeDASDestinationUnit>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT DestinationUnitGUID,CONVERT(varchar(50), DestinationUnitGUID) as C2 ,codeDASDestinationUnitRowVersion FROM code.codeDASDestinationUnit where DestinationUnitGUID in (" + string.Join(",", models.Select(x => "'" + x.DestinationUnitGUID + "'").ToArray()) + ")";

            string query = DbDAS.QueryBuilder(models, Permissions.DASConfiguration.DeleteGuid, SubmitTypes.Restore, "");

            var Records = DbDAS.Database.SqlQuery<codeDASDestinationUnit>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveUnit(record))
                {
                    RestoredUnit.Add(DbDAS.Restore(record, Permissions.DASConfiguration.DeleteGuid, Permissions.DASConfiguration.RestoreGuid, RestoringTime, DbCMS));
                }
            }


            return RestoredUnit;
        }

        private JsonResult ConcurrencyUnit(Guid PK)
        {
            UnitUpdateModel dbModel = new UnitUpdateModel();

            var Unit = DbDAS.codeDASDestinationUnit.Where(x => x.DestinationUnitGUID == PK).FirstOrDefault();
            var dbUnit = DbDAS.Entry(Unit).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbUnit, dbModel);





            if (Unit.codeDASDestinationUnitRowVersion.SequenceEqual(dbModel.codeDASDestinationUnitRowVersion))
            {
                return Json(DbDAS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbDAS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveUnit(Object model)
        {
            codeDASDestinationUnit Unit = Mapper.Map(model, new codeDASDestinationUnit());
            int ModelDescription = DbDAS.codeDASDestinationUnit
                                    .Where(x => x.DestinationUnitName == Unit.DestinationUnitName &&

                                                x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("ModelDescription", "Unit is already exists");
            }
            return (ModelDescription > 0);
        }

        #endregion

        #region Focal Points

        //[Route("DAS/UnitFocalPointsDataTable/{PK}")]
        public ActionResult UnitFocalPointsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/DAS/Views/Units/_UnitFocalPointsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeDASDestinationUnitFocalPoint, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeDASDestinationUnitFocalPoint>(DataTable.Filters);
            }

            var Result = DbDAS.codeDASDestinationUnitFocalPoint.AsNoTracking().AsExpandable().Where(x =>
            x.DestinationUnitGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.DestinationUnitFocalPointGUID,
                                  x.DestinationUnitGUID,
                                  x.EmailAddress,
                                  FullName = x.FullName,
                                  x.codeDASDestinationUnitFocalPointRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        [Route("DAS/Configurations/UnitFocalPointCreate/")]
        public ActionResult UnitFocalPointCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            DestinationUnitFocalPointModel model = new DestinationUnitFocalPointModel { DestinationUnitGUID = FK, };

            return PartialView("~/Areas/DAS/Views/Units/_UnitFocalPointUpdateModal.cshtml",
               model);
        }

        public ActionResult UnitFocalPointUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Update, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            DestinationUnitFocalPointModel model = DbDAS.codeDASDestinationUnitFocalPoint.Where(a => a.DestinationUnitFocalPointGUID == PK).
                Select(a => new DestinationUnitFocalPointModel
                {
                    DestinationUnitFocalPointGUID = a.DestinationUnitFocalPointGUID,
                    DestinationUnitGUID = a.DestinationUnitGUID,
                    EmailAddress = a.EmailAddress,
                    FullName = a.FullName,
                    IsSupervisor = (bool)a.IsSupervisor,
                    UserGUID = a.UserGUID,
                    Active = a.Active


                }
                ).FirstOrDefault();
            return PartialView("~/Areas/DAS/Views/Units/_UnitFocalPointUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UnitFocalPointCreate(codeDASDestinationUnitFocalPoint model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveUnitFocalPoint(model)) return PartialView("~/Areas/DAS/Views/Units/_UnitFocalPointUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            var user = DbDAS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.UserGUID == model.UserGUID).FirstOrDefault();
            model.FullName = user.FirstName + " " + user.Surname;
            model.IsSupervisor = model.IsSupervisor;

            var email = DbDAS.userServiceHistory.Where(x => x.UserGUID == model.UserGUID).FirstOrDefault();
            model.EmailAddress = email.EmailAddress;
            DbDAS.Create(model, Permissions.DASConfiguration.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleUpdateMessage(DataTableNames.UnitFocalPointsDataTable, DbDAS.PrimaryKeyControl(model), DbDAS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UnitFocalPointUpdate(codeDASDestinationUnitFocalPoint model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Update, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveUnitFocalPoint(model)) return PartialView("~/Areas/DAS/Views/Units/_UnitFocalPointUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbDAS.Update(model, Permissions.DASConfiguration.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleUpdateMessage(DataTableNames.UnitFocalPointsDataTable,
                    DbDAS.PrimaryKeyControl(model),
                    DbDAS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyUnitFocalPoint(model.DestinationUnitFocalPointGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UnitFocalPointDelete(codeDASDestinationUnitFocalPoint model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Delete, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDASDestinationUnitFocalPoint> DeletedLanguages = DeleteUnitFocalPoint(new List<codeDASDestinationUnitFocalPoint> { model });

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleDeleteMessage(DeletedLanguages, DataTableNames.UnitFocalPointsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyUnitFocalPoint(model.DestinationUnitFocalPointGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UnitFocalPointRestore(codeDASDestinationUnitFocalPoint model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Restore, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveUnitFocalPoint(model))
            {
                return Json(DbDAS.RecordExists());
            }

            List<codeDASDestinationUnitFocalPoint> RestoredLanguages = RestoreUnitFocalPoint(Portal.SingleToList(model));

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleRestoreMessage(RestoredLanguages, DataTableNames.UnitFocalPointsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyUnitFocalPoint(model.DestinationUnitFocalPointGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult UnitFocalPointsDataTableDelete(List<codeDASDestinationUnitFocalPoint> models)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Delete, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDASDestinationUnitFocalPoint> DeletedLanguages = DeleteUnitFocalPoint(models);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.UnitFocalPointsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult UnitFocalPointsDataTableRestore(List<codeDASDestinationUnitFocalPoint> models)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Restore, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeDASDestinationUnitFocalPoint> RestoredLanguages = RestoreUnitFocalPoint(models);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.UnitFocalPointsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        private List<codeDASDestinationUnitFocalPoint> DeleteUnitFocalPoint(List<codeDASDestinationUnitFocalPoint> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeDASDestinationUnitFocalPoint> DeletedUnitFocalPoint = new List<codeDASDestinationUnitFocalPoint>();

            string query = DbDAS.QueryBuilder(models, Permissions.DASConfiguration.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbDAS.Database.SqlQuery<codeDASDestinationUnitFocalPoint>(query).ToList();

            foreach (var language in languages)
            {
                DeletedUnitFocalPoint.Add(DbDAS.Delete(language, ExecutionTime, Permissions.DASConfiguration.DeleteGuid, DbCMS));
            }

            return DeletedUnitFocalPoint;
        }

        private List<codeDASDestinationUnitFocalPoint> RestoreUnitFocalPoint(List<codeDASDestinationUnitFocalPoint> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<codeDASDestinationUnitFocalPoint> RestoredLanguages = new List<codeDASDestinationUnitFocalPoint>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbDAS.QueryBuilder(models, Permissions.DASConfiguration.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbDAS.Database.SqlQuery<codeDASDestinationUnitFocalPoint>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveUnitFocalPoint(language))
                {
                    RestoredLanguages.Add(DbDAS.Restore(language, Permissions.DASConfiguration.DeleteGuid, Permissions.DASConfiguration.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyUnitFocalPoint(Guid PK)
        {
            codeDASDestinationUnitFocalPoint dbModel = new codeDASDestinationUnitFocalPoint();

            var Language = DbDAS.codeDASDestinationUnitFocalPoint.Where(l => l.DestinationUnitFocalPointGUID == PK).FirstOrDefault();
            var dbLanguage = DbDAS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeDASDestinationUnitFocalPointRowVersion.SequenceEqual(dbModel.codeDASDestinationUnitFocalPointRowVersion))
            {
                return Json(DbDAS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbDAS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveUnitFocalPoint(codeDASDestinationUnitFocalPoint model)
        {
            int LanguageID = DbDAS.codeDASDestinationUnitFocalPoint
                                  .Where(x =>
                                              x.EmailAddress == model.EmailAddress &&
                                              x.FullName == model.FullName &&
                                              x.DestinationUnitGUID == model.DestinationUnitGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                //Fix
                ModelState.AddModelError("LanguageID", " Already exits"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }

        #endregion Language
    }
}